using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.TEAScript
{
    /// <summary>
    /// Manages the currently open TSValueList for list:* commands.
    /// Provides methods for setting the current list status values $list.* in a script execution context.
    /// </summary>
    public class TSListManager
    {
        #region Constructor

        /// <summary>
        /// Initialize a list manager
        /// </summary>
        /// <param name="a_provider">List provider for special lists</param>
        /// <param name="a_context">Context values to write list status to</param>
        public TSListManager(ITSListProvider a_provider, TSValueDictionary a_contextValues)
        {
            mProvider = a_provider ?? throw new ArgumentNullException(nameof(a_provider));
            mContextValues = a_contextValues ?? throw new ArgumentNullException(nameof(a_contextValues));
            ClearListStatus();
        }

        #endregion

        #region Public data

        /// <summary>
        /// A broadcaster that's used during list access. Listen on it to receive list warnings and errors.
        /// </summary>
        public readonly TSBroadcaster Broadcaster = new();

        #endregion

        #region Public methods

        /// <summary>
        /// Check if the manager has an open list.
        /// </summary>
        /// <returns>True iff a list is available.</returns>
        public bool HasList()
        {
            return mCurrentList != null;
        }

        /// <summary>
        /// Add a value or multiple wildcarded values to the current list.
        /// </summary>
        /// <param name="a_valueName">Name of the value(s) to add.</param>
        /// <returns>True iff the values have been added.</returns>
        public bool AddToList(TSNamedValueArgument a_valueName)
        {
            if (!VerifyListOpen(nameof(AddToList))) return false;

            Dictionary<string, TSValue> valueItem = new();
            if (a_valueName.HasWildcard)
            {
                // add multiple values with common prefix
                var wildcardValues = mContextValues.Where(kvp => kvp.Key.StartsWith(a_valueName.ValueName));
                if (!wildcardValues.Any()) return false;
                foreach (var currentValue in wildcardValues)
                {
                    // add without common prefix
                    valueItem[currentValue.Key[a_valueName.ValueName.Length..]] = currentValue.Value;
                }
            }
            else
            {
                if (!a_valueName.HasValue(mContextValues)) return false;
                // add single named value w/o prefix
                var valueName = (TSConstants.ValuePrefixes.Any(p => p == a_valueName.ValueName[0]))
                    ? a_valueName.ValueName[1..]
                    : a_valueName.ValueName;
                valueItem.Add(valueName, a_valueName.GetValue(mContextValues));
            }
            lock (mCurrentList)
            {
                mCurrentList.Add(valueItem);
            }
            SetCurrentListStatus();
            return true;
        }

        /// <summary>
        /// Clear the current list, removing all items
        /// </summary>
        /// <returns>True iff the list was cleared</returns>
        public bool ClearList()
        {
            if (!VerifyListOpen(nameof(ClearList))) return false;
            lock (mCurrentList)
            {
                mCurrentList.Clear();
            }
            return true;
        }

        /// <summary>
        /// Check if the given index is valid for the current list
        /// </summary>
        /// <param name="a_index">The index to check</param>
        /// <returns>True iff the index is valid</returns>
        public bool HasListItem(long a_index)
        {
            if (!VerifyListOpen(nameof(HasListItem))) return false;
            lock (mCurrentList)
            {
                var intIndex = (int)a_index;
                return (0 <= intIndex) && (intIndex < mCurrentList.Count);
            }
        }

        /// <summary>
        /// Get the list item with the given index
        /// </summary>
        /// <param name="a_index">Index of the item to get</param>
        /// <param name="a_target">Name of the variable where the data should be written to</param>
        /// <returns>True iff the index is valid</returns>
        public bool GetListItem(long a_index, TSNamedValueArgument a_target)
        {
            if (!VerifyListOpen(nameof(GetListItem))) return false;
            lock (mCurrentList)
            {
                return GetItemAtIndex((int)a_index, a_target);
            }
        }

        /// <summary>
        /// Get the first list item
        /// </summary>
        /// <param name="a_target">Name of the variable where the data should be written to</param>
        /// <returns>True iff the list has a first item</returns>
        public bool GetFirstItem(TSNamedValueArgument a_target)
        {
            if (!VerifyListOpen(nameof(GetFirstItem))) return false;
            lock (mCurrentList)
            {
                return GetItemAtIndex(0, a_target);
            }
        }

        /// <summary>
        /// Get the last list item
        /// </summary>
        /// <param name="a_target">Name of the variable where the data should be written to</param>
        /// <returns>True iff the list has a last item</returns>
        public bool GetLastItem(TSNamedValueArgument a_target)
        {
            if (!VerifyListOpen(nameof(GetLastItem))) return false;
            lock (mCurrentList)
            {
                return GetItemAtIndex(mCurrentList.Count - 1, a_target);
            }
        }

        /// <summary>
        /// Get the next list item
        /// </summary>
        /// <param name="a_target">Name of the variable where the data should be written to</param>
        /// <returns>True iff the list has a next item</returns>
        public bool GetNextItem(TSNamedValueArgument a_target)
        {
            if (!VerifyListOpen(nameof(GetNextItem))) return false;
            lock (mCurrentList)
            {
                var currentIndex = (int)(mContextValues[cValueNameListIndex].NumericalValue);
                return GetItemAtIndex(currentIndex + 1, a_target);
            }
        }

        /// <summary>
        /// Get the previous list item
        /// </summary>
        /// <param name="a_target">Name of the variable where the data should be written to</param>
        /// <returns>True iff the list has a previous item</returns>
        public bool GetPreviousItem(TSNamedValueArgument a_target)
        {
            if (!VerifyListOpen(nameof(GetPreviousItem))) return false;
            lock (mCurrentList)
            {
                var currentIndex = (int)(mContextValues[cValueNameListIndex].NumericalValue);
                return GetItemAtIndex(currentIndex - 1, a_target);
            }
        }

        /// <summary>
        /// Get a random list item
        /// </summary>
        /// <param name="a_target">Name of the variable where the data should be written to</param>
        /// <returns>True iff the list has any items</returns>
        public bool GetRandomItem(TSNamedValueArgument a_target)
        {
            if (!VerifyListOpen(nameof(GetRandomItem))) return false;
            lock (mCurrentList)
            {
                var randomIndex = (mCurrentList.Count == 0) ? 0 : sRandomizer.Next(0, mCurrentList.Count);
                return GetItemAtIndex(randomIndex, a_target);
            }
        }

        /// <summary>
        /// Remove the item with the given index from the list
        /// </summary>
        /// <param name="a_index">Index of the item to get</param>
        /// <returns>True iff the index is valid</returns>
        public bool RemoveListItem(long a_index)
        {
            if (!VerifyListOpen(nameof(RemoveListItem))) return false;
            lock (mCurrentList)
            {
                var intIndex = (int)a_index;
                if ((intIndex < 0) || (intIndex >= mCurrentList.Count)) return false;
                // remove item
                mCurrentList.RemoveAt(intIndex);
                // update context status
                SetCurrentListStatus();
                return true;
            }
        }

        /// <summary>
        /// Filter the list, creating a new list containing only items where the given key matches the given value.
        /// Modifying the new list will not affect the original list.
        /// </summary>
        /// <param name="a_key">The key to filter by.</param>
        /// <param name="a_value">The value to filter for.</param>
        /// <returns>True iff the list has been filtered.</returns>
        public bool FilterList(string a_key, ITSValueArgument a_value)
        {
            if (!VerifyListOpen(nameof(RemoveListItem))) return false;
            lock (mCurrentList)
            {
                // remember original list
                var originalList = mCurrentList;
                // create new list
                mCurrentList = new();
                // value to filter for
                var desiredValue = a_value.GetValue(mContextValues);
                // filter items
                foreach (var originalItem in originalList)
                {
                    if (originalItem.TryGetValue(a_key, out TSValue itemValue)
                        && itemValue.Equals(desiredValue))
                    {
                        // create copy
                        Dictionary<string, TSValue> filteredItem = new();
                        foreach (var kvp in originalItem)
                        {
                            filteredItem[kvp.Key] = kvp.Value;
                        }
                        // add to filtered list
                        mCurrentList.Add(filteredItem);
                    }
                }
                // update context status
                SetCurrentListStatus();
                return true;
            }
        }

        /// <summary>
        /// Filter the list, creating a new list containing only the first item for each value of the given key.
        /// Modifying the new list will not affect the original list.
        /// </summary>
        /// <param name="a_key">The key to filter by.</param>
        /// <returns>True iff the list has been filtered.</returns>
        public bool MakeListItemsUnique(string a_key)
        {
            if (!VerifyListOpen(nameof(RemoveListItem))) return false;
            lock (mCurrentList)
            {
                // remember original list
                var originalList = mCurrentList;
                // create new list
                mCurrentList = new();
                // track values we have already encountered
                HashSet<TSValue> encounteredValues = new();
                // filter items
                foreach (var originalItem in originalList)
                {
                    if (originalItem.TryGetValue(a_key, out TSValue itemValue)
                        && !encounteredValues.Contains(itemValue))
                    {
                        // remember value
                        encounteredValues.Add(itemValue);
                        // create copy
                        Dictionary<string, TSValue> filteredItem = new();
                        foreach (var kvp in originalItem)
                        {
                            filteredItem[kvp.Key] = kvp.Value;
                        }
                        // add to filtered list
                        mCurrentList.Add(filteredItem);
                    }
                }
                // update context status
                SetCurrentListStatus();
                return true;
            }
        }

        /// <summary>
        /// Load a named list.
        /// Named lists may refer to special internal lists from the list provider,
        /// or additional lists this manager is aware of. These lists should use the
        /// '#' name prefix.
        /// Additional lists may be created as needed and will persist for the
        /// lifespan of the program, but not between bot executions.
        /// </summary>
        /// <param name="a_listName">Name of the list to load</param>
        /// <returns>True iff a list with the given name was loaded.</returns>
        public bool LoadList(string a_listName)
        {
            // try to load from additional lists first
            if (!mAdditionalLists.TryGetValue(a_listName, out mCurrentList))
            {
                // get list from provider
                mCurrentList = mProvider.GetList(a_listName);
                // fallback for custom lists
                if ((mCurrentList == null) && !a_listName.StartsWith(TSConstants.SpecialListPrefix))
                {
                    // create missing list
                    mCurrentList = mProvider.CreateList(a_listName);
                }
            }
            // update status values
            SetCurrentListStatus();
            // return whether list has been loaded
            return (mCurrentList != null);
        }

        /// <summary>
        /// Open a list from persistent storage. Storage lock must be held.
        /// </summary>
        /// <param name="a_listName">Name of the list to load, corresponding to the file name minus extension</param>
        /// <param name="a_storage">The storage provider with which to open the list</param>
        /// <param name="a_owner">Owner of the storage lock</param>
        /// <returns>True iff a list with the given name was loaded.</returns>
        public bool OpenStorageList(string a_listName, ITSStorage a_storage, object a_owner)
        {
            if (a_storage == null)
            {
                throw new ArgumentNullException(nameof(a_storage));
            }
            if (a_storage.Open(a_owner, a_listName, true))
            {
                mCurrentList = a_storage.GetList(a_owner);
                SetCurrentListStatus();
                return (mCurrentList != null);
            }
            return false;
        }

        /// <summary>
        /// Add an additional list for the script execution this manager belongs to
        /// </summary>
        /// <param name="a_name">The list name via which it can be accessed from the script</param>
        /// <param name="a_list">The list data</param>
        public void AddAdditionalList(string a_name, TSValueList a_list)
        {
            mAdditionalLists[a_name] = a_list;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Set the list status context values to indicate no list is open
        /// </summary>
        private void ClearListStatus()
        {
            mContextValues[cValueNameListCount] = 0;
            mContextValues[cValueNameListLastIndex] = -1;
            mContextValues[cValueNameListHasItem] = 0;
            mContextValues[cValueNameListIndex] = -1;
            mContextValues[cValueNameListOutOfBounds] = 1;
        }

        /// <summary>
        /// Set the list status context values for the current list
        /// </summary>
        private void SetCurrentListStatus()
        {
            if (mCurrentList == null)
            {
                ClearListStatus();
                return;
            }
            lock (mCurrentList)
            {
                mContextValues[cValueNameListCount] = mCurrentList.Count;
                mContextValues[cValueNameListLastIndex] = mCurrentList.Count - 1;
            }
        }

        /// <summary>
        /// Verify that a list is open, broadcasts an error otherwise
        /// </summary>
        /// <param name="a_reason">Reason why the list is needed</param>
        /// <returns>True iff a list is open</returns>
        private bool VerifyListOpen(string a_reason)
        {
            if (mCurrentList != null) return true;
            Broadcaster.Error("A list must be loaded for operation: {0}", a_reason);
            return false;
        }

        /// <summary>
        /// Get the list item with the given index without locking the list
        /// </summary>
        /// <param name="a_index">Index of the item to get</param>
        /// <param name="a_target">Name of the variable where the data should be written to</param>
        /// <returns>True iff the index is valid</returns>
        private bool GetItemAtIndex(int a_index, TSNamedValueArgument a_target)
        {
            if ((mCurrentList == null) || (a_index < 0) || (a_index >= mCurrentList.Count))
            {
                // update context status
                mContextValues[cValueNameListHasItem] = 0;
                mContextValues[cValueNameListIndex] = -1;
                mContextValues[cValueNameListOutOfBounds] = 1;
                return false;
            }
            // get value and write to target
            var value = mCurrentList[a_index];
            if (a_target.HasWildcard)
            {
                foreach (var kvp in value)
                {
                    mContextValues[a_target.ValueName + kvp.Key] = kvp.Value;
                }
            }
            else
            {
                if (value.Count != 1)
                {
                    Broadcaster.Warn("Multiple values in list item but single target name given.");
                }
                mContextValues[a_target.ValueName] = value.Values.FirstOrDefault();
            }
            // update context status
            mContextValues[cValueNameListHasItem] = 1;
            mContextValues[cValueNameListIndex] = a_index;
            mContextValues[cValueNameListOutOfBounds] = 0;
            return true;
        }

        #endregion

        #region Value name constants

        /// <summary>
        /// Number of items in the open list
        /// </summary>
        public static readonly string cValueNameListCount = TSConstants.ContextPrefix + "list.count";
        /// <summary>
        /// Index of the last list item, aka $list.count-1
        /// </summary>
        public static readonly string cValueNameListLastIndex = TSConstants.ContextPrefix + "list.lastIndex";
        /// <summary>
        /// 1 iff the last accessed item had a valid index, 0 otherwise
        /// </summary>
        public static readonly string cValueNameListHasItem = TSConstants.ContextPrefix + "list.hasItem";
        /// <summary>
        /// Last accessed item index
        /// </summary>
        public static readonly string cValueNameListIndex = TSConstants.ContextPrefix + "list.index";
        /// <summary>
        /// 1 iff the last accessed item had an invalid index, 0 otherwise; aka not $list.hasItem
        /// </summary>
        public static readonly string cValueNameListOutOfBounds = TSConstants.ContextPrefix + "list.outOfBounds";

        #endregion

        #region Private data

        /// <summary>
        /// The current list if one is open
        /// </summary>
        private TSValueList mCurrentList = null;

        /// <summary>
        /// Provider for special global lists
        /// </summary>
        private readonly ITSListProvider mProvider;

        /// <summary>
        /// Context value dictionary to write list status data to
        /// </summary>
        private readonly TSValueDictionary mContextValues;

        /// <summary>
        /// Additional named lists which can be supplied for a single script execution
        /// </summary>
        private readonly Dictionary<string, TSValueList> mAdditionalLists = new();

        /// <summary>
        /// A randomizer shared across all instances
        /// </summary>
        private static readonly Random sRandomizer = new();

        #endregion
    }
}
