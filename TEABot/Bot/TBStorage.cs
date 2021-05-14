using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using TEABot.TEAScript;

namespace TEABot.Bot
{
    /// <summary>
    /// Storage provider which saves
    /// </summary>
    class TBStorage : ITSStorage
    {
        #region Public data

        /// <summary>
        /// A broadcaster that's used during storage access. Listen on it to receive storage warnings and errors.
        /// </summary>
        public readonly TSBroadcaster Broadcaster = new();

        /// <summary>
        /// Path of the directory in which persistent data files are stored
        /// </summary>
        public string StorageDirectory = String.Empty;

        #endregion

        #region Private data

        /// <summary>
        /// Mutex for lock management
        /// </summary>
        private readonly Mutex mLock = new();

        /// <summary>
        /// Lock owner tracking
        /// </summary>
        private object mOwner = null;

        /// <summary>
        /// Data associated with the currently opened storage object,
        /// null when no object is open.
        /// </summary>
        private MultiValueData mOpenData = null;

        /// <summary>
        /// Name of the opened storage object
        /// </summary>
        private string mOpenStorageName = String.Empty;

        /// <summary>
        /// File extension for storage objects
        /// </summary>
        private static readonly string sStorageFileExtension = ".json";

        #endregion

        #region Constructor

        #endregion

        #region Private methods

        /// <summary>
        /// Verify that the given owner holds the lock
        /// </summary>
        /// <param name="a_owner">The lock owner who wants to access the storage</param>
        /// <param name="a_reason">The reason why storage access is required</param>
        /// <returns>True iff the given owner holds the lock</returns>
        private bool VerifyOwner(object a_owner, string a_reason)
        {
            if (a_owner == null) throw new ArgumentNullException(nameof(a_owner));
            if (a_owner != mOwner)
            {
                Broadcaster.Error("Lock must be held to access storage for operation: {0}", a_reason);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Verify that a storage object is open
        /// </summary>
        /// <param name="a_reason">The reason why a storage object is required</param>
        /// <returns>True iff a storage object is open</returns>
        private bool VerifyOpen(string a_reason)
        {
            if (mOpenData == null)
            {
                Broadcaster.Error("Storage object must be open for operation: {0}", a_reason);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Verify that an open storage object can be accessed by the given owner
        /// </summary>
        /// <param name="a_owner">The lock owner who wants to access the storage</param>
        /// <param name="a_reason">The reason why storage access is required</param>
        /// <returns>True iff a storage object can be accessed</returns>
        private bool VerifyStorageAccess(object a_owner, string a_reason)
        {
            return VerifyOwner(a_owner, a_reason) && VerifyOpen(a_reason);
        }

        /// <summary>
        /// Get the file path for the given storage object
        /// </summary>
        /// <param name="a_storageName">Name of the storage object to access</param>
        /// <returns>The full path of the storage object's file</returns>
        private string GetStorageFilePath(string a_storageName)
        {
            return Path.Combine(StorageDirectory, a_storageName + sStorageFileExtension);
        }

        #endregion

        #region Internal data structures

        /// <summary>
        /// Common interface for storage data structs.
        /// </summary>
        private interface IStorageData
        {
            /// <summary>
            /// Converts the storage data to an unspecifically typed object for JSON serialization.
            /// </summary>
            /// <returns>Some object for JSON serialization.</returns>
            object ToUnspecificObject();
        }

        /// <summary>
        /// Storage data containing a single value.
        /// </summary>
        private class SingleValueData : IStorageData
        {
            /// <summary>
            /// The single storage value.
            /// </summary>
            public TSValue Value { get; set; }

            /// <summary>
            /// Create a single value data object for a given value
            /// </summary>
            /// <param name="a_value">The value</param>
            public SingleValueData(TSValue a_value)
            {
                Value = a_value;
            }

            /// <summary>
            /// Create a single value data object from a json element
            /// </summary>
            /// <param name="a_json">The Json element from which to load a value</param>
            public SingleValueData(JsonElement a_json)
            {
                Value = a_json.ValueKind switch
                {
                    JsonValueKind.String => new TSValue(a_json.GetString()),
                    JsonValueKind.Number => new TSValue(a_json.GetInt64()),
                    _ => throw new ArgumentException(
                        String.Format("Json is of unsupported element type {0}", a_json.ValueKind),
                        nameof(a_json)),
                };
            }

            public object ToUnspecificObject()
            {
                if (Value.IsText)
                {
                    return Value.TextValue;
                }
                else
                {
                    return Value.NumericalValue;
                }
            }
        }

        /// <summary>
        /// Storage data containing a map from keys to further storage data.
        /// </summary>
        private class MultiValueData : IStorageData
        {
            /// <summary>
            /// The data map.
            /// </summary>
            public Dictionary<string, IStorageData> Values { get; set; }

            /// <summary>
            /// Create an empty multi value data object
            /// </summary>
            public MultiValueData()
            {
                Values = new Dictionary<string, IStorageData>();
            }

            /// <summary>
            /// Create a multi value data object from a json element
            /// </summary>
            /// <param name="a_json">The Json element from which to load values</param>
            public MultiValueData(JsonElement a_json)
            {
                Values = new Dictionary<string, IStorageData>();
                if (a_json.ValueKind == JsonValueKind.Object)
                {
                    foreach (var element in a_json.EnumerateObject())
                    {
                        Values[element.Name] = element.Value.ValueKind switch
                        {
                            JsonValueKind.Object => new MultiValueData(element.Value),
                            JsonValueKind.String or JsonValueKind.Number => new SingleValueData(element.Value),
                            _ => throw new ArgumentException(
                                String.Format("Json contains unsupported element type {0} for \"{1}\"", element.Value.ValueKind, element.Name),
                                nameof(a_json)),
                        };
                    }
                }
                else
                {
                    throw new ArgumentException(
                        String.Format("Json is of unsupported element type {0}",
                            a_json.ValueKind),
                        nameof(a_json));
                }
            }

            public object ToUnspecificObject()
            {
                Dictionary<string, object> result = new();

                foreach (var kvp in Values)
                {
                    result[kvp.Key] = kvp.Value.ToUnspecificObject();
                }

                return result;
            }
        }

        #endregion

        #region ITSStorage implementation

        public bool AcquireLock(object a_owner)
        {
            if (a_owner == null) throw new ArgumentNullException(nameof(a_owner));
            if (a_owner == mOwner)
            {
                Broadcaster.Warn("Lock has already been acquired by this owner.");
                return false;
            }
            mLock.WaitOne();
            mOwner = a_owner;
            return true;
        }

        public bool Clear(object a_owner, string a_storageName)
        {
            if (!VerifyOwner(a_owner, nameof(Clear))) return false;

            if (mOpenData != null)
            {
                Broadcaster.Warn("Clearing storage when another is open. Closing open storage.");
                Close(a_owner);
            }

            // get storage file path
            var fullStoragePath = GetStorageFilePath(a_storageName);

            // delete file if it exists
            if (File.Exists(fullStoragePath))
            {
                try
                {
                    File.Delete(fullStoragePath);
                }
                catch (Exception ex)
                {
                    Broadcaster.Error("Failed to delete storage file \"{0}\" due to {1}: {2}", fullStoragePath, ex.GetType(), ex.Message);
                    return false;
                }
            }

            return true;
        }

        public bool Close(object a_owner)
        {
            if (!VerifyOwner(a_owner, nameof(Close))) return false;
            if (mOpenData == null)
            {
                Broadcaster.Warn("No storage object is open, so there is nothing to close.");
                return false;
            }
            mOpenData = null;
            mOpenStorageName = String.Empty;
            return true;
        }

        public bool Get(object a_owner, string a_keyName, TSNamedValueArgument a_valueName, TSValueDictionary a_values)
        {
            if (a_values == null) throw new ArgumentNullException(nameof(a_values));
            if (!VerifyStorageAccess(a_owner, nameof(Get))) return false;
            if (mOpenData.Values.TryGetValue(a_keyName, out IStorageData data))
            {
                // single value, no wildcard
                if (data is SingleValueData singleData)
                {
                    if (a_valueName.HasWildcard)
                    {
                        Broadcaster.Error("Storage holds a single value for \"{0}\", but a wildcard value name was given.", a_keyName);
                        return false;
                    }
                    a_values[a_valueName.ValueName] = singleData.Value;
                    return true;
                }
                // multiple values, with wildcard
                else if (data is MultiValueData multiData)
                {
                    if (!a_valueName.HasWildcard)
                    {
                        Broadcaster.Error("Storage holds multiple values for \"{0}\", but no wildcard value name was given.", a_keyName);
                        return false;
                    }
                    foreach (var currentValue in multiData.Values)
                    {
                        if (currentValue.Value is SingleValueData currentSingleValue)
                        {
                            // store under concatenated value name
                            a_values[a_valueName.ValueName + currentValue.Key] = currentSingleValue.Value;
                        }
                        else
                        {
                            Broadcaster.Warn("Storage data for \"{0}\" contains unsupported value at \"{1}\".", a_keyName, currentValue.Key);
                        }
                    }
                    return true;
                }
                // unsupported storage data type
                Broadcaster.Error("Storage value type for \"{0}\" is not supported.", a_keyName);
                return false;
            }
            Broadcaster.Warn("Storage object does not contain a value for \"{0}\".", a_keyName);
            return false;
        }

        public bool Has(object a_owner, string a_keyName)
        {
            if (!VerifyStorageAccess(a_owner, nameof(Has))) return false;
            return mOpenData.Values.ContainsKey(a_keyName);
        }

        public bool HoldsLock(object a_owner)
        {
            if (a_owner == null) throw new ArgumentNullException(nameof(a_owner));
            return (a_owner == mOwner);
        }

        public bool Open(object a_owner, string a_storageName)
        {
            if (!VerifyOwner(a_owner, nameof(Open))) return false;

            if (mOpenData != null)
            {
                Broadcaster.Error("Opening storage when another is already open. Closing both do avoid data corruption.");
                Close(a_owner);
                return false;
            }

            // ensure storage directory exists
            if (!Directory.Exists(StorageDirectory))
            {
                try
                {
                    var directoryInfo = Directory.CreateDirectory(StorageDirectory);
                    if (!directoryInfo.Exists)
                    {
                        Broadcaster.Error("Unable to access storage directory.");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Broadcaster.Error("Failed to create storage directory due to {0}: {1}", ex.GetType(), ex.Message);
                    return false;
                }
            }

            // get storage file path
            var fullStoragePath = GetStorageFilePath(a_storageName);

            // load file if it already exists
            if (File.Exists(fullStoragePath))
            {
                try
                {
                    using var fileStream = File.OpenRead(fullStoragePath);
                    var jsonData = JsonDocument.Parse(fileStream);
                    mOpenData = new MultiValueData(jsonData.RootElement);
                }
                catch (Exception ex)
                {
                    Broadcaster.Error("Failed to load storage file \"{0}\" due to {1}: {2}", fullStoragePath, ex.GetType(), ex.Message);
                    return false;
                }
            }
            else
            {
                // initialize new empty storage object
                mOpenData = new MultiValueData();
            }

            mOpenStorageName = a_storageName;
            return true;
        }

        public bool ReleaseLock(object a_owner)
        {
            if (!VerifyOwner(a_owner, nameof(ReleaseLock))) return false;
            mLock.ReleaseMutex();
            mOwner = null;
            return true;
        }

        public bool Remove(object a_owner, string a_keyName)
        {
            if (!VerifyStorageAccess(a_owner, nameof(Remove))) return false;
            return mOpenData.Values.Remove(a_keyName);
        }

        public bool Save(object a_owner)
        {
            if (!VerifyStorageAccess(a_owner, nameof(Save))) return false;

            var fullStoragePath = GetStorageFilePath(mOpenStorageName);
            try
            {
                using var fileStream = File.Create(fullStoragePath);
                using var jsonWriter = new Utf8JsonWriter(fileStream);
                var jsonData = mOpenData.ToUnspecificObject();
                JsonSerializer.Serialize(jsonWriter, jsonData);
            }
            catch (Exception ex)
            {
                Broadcaster.Error("Failed to save storage file \"{0}\" due to {1}: {2}", fullStoragePath, ex.GetType(), ex.Message);
                return false;
            }

            return true;
        }

        public bool Set(object a_owner, string a_keyName, ITSValueArgument a_valueName, TSValueDictionary a_values)
        {
            if (a_values == null) throw new ArgumentNullException(nameof(a_values));
            if (!VerifyStorageAccess(a_owner, nameof(Set))) return false;
            // named values
            if ((a_valueName is TSNamedValueArgument namedValue)
                && namedValue.HasWildcard)
            {
                // multiple values, with wildcard
                var multiValues = new MultiValueData();
                // find all values via wildcarded prefix
                var wildcardValues = a_values.Where(kvp => kvp.Key.StartsWith(namedValue.ValueName));
                foreach (var currentValue in wildcardValues)
                {
                    // add without common prefix
                    multiValues.Values[currentValue.Key[namedValue.ValueName.Length..]] = new SingleValueData(currentValue.Value);
                }
                // add to storage
                mOpenData.Values[a_keyName] = multiValues;
            }
            // single value, no wildcard
            else
            {
                mOpenData.Values[a_keyName] = new SingleValueData(a_valueName.GetValue(a_values));
            }
            return true;
        }

        #endregion
    }
}
