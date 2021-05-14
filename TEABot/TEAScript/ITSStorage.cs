
namespace TEABot.TEAScript
{
    /// <summary>
    /// Interface for storage access providers,
    /// see Instructions.Statements.Storage for applications
    /// </summary>
    public interface ITSStorage
    {
        /// <summary>
        /// Acquire a lock on storage access until ReleaseLock() is called.
        /// May block until a lock is acquired.
        /// 
        /// A lock should be acquired and held for the entire duration of storage access
        /// to prevent data corruption in a multi-threaded environment.
        /// </summary>
        /// <param name="a_owner">The owner who wants to hold the lock.</param>
        /// <returns>True iff a lock was acquired.</returns>
        public bool AcquireLock(object a_owner);

        /// <summary>
        /// Release the lock acquired by AcquireLock() so someone else may access the storage.
        /// </summary>
        /// <param name="a_owner">The owner holding the lock.</param>
        /// <returns>True iff a held lock was released.</returns>
        public bool ReleaseLock(object a_owner);

        /// <summary>
        /// Check if a specific owner currently holds the access lock, see AcquireLock().
        /// </summary>
        /// <param name="a_owner">The owner who should hold the lock.</param>
        /// <returns>True iff the given owner holds the lock.</returns>
        public bool HoldsLock(object a_owner);

        /// <summary>
        /// Open a named storage object.
        /// If no such object exists yet, an empty object will be created.
        /// </summary>
        /// <param name="a_owner">The lock owner to enable internal lock holder checking.</param>
        /// <param name="a_storageName">Name of the storage object to open.</param>
        /// <returns>True iff the storage object has been opened.</returns>
        public bool Open(object a_owner, string a_storageName);

        /// <summary>
        /// Close the storage object opened with Open() so internal resources can be freed.
        /// </summary>
        /// <param name="a_owner">The lock owner to enable internal lock holder checking.</param>
        /// <returns>True iff the storage object has been closed.</returns>
        public bool Close(object a_owner);

        /// <summary>
        /// Clear a named storage object, discarding all data stored in it.
        /// </summary>
        /// <param name="a_owner">The lock owner to enable internal lock holder checking.</param>
        /// <param name="a_storageName">Name of the storage object to clear.</param>
        /// <returns>True iff the storage object has been cleared.</returns>
        public bool Clear(object a_owner, string a_storageName);

        /// <summary>
        /// Save the opened storage object persistently, e.g. to disk.
        /// </summary>
        /// <param name="a_owner">The lock owner to enable internal lock holder checking.</param>
        /// <returns>True iff the storage object has been saved.</returns>
        public bool Save(object a_owner);

        /// <summary>
        /// Set some data in the storage object.
        /// </summary>
        /// <param name="a_owner">The lock owner to enable internal lock holder checking.</param>
        /// <param name="a_keyName">Identifier for the data to store.</param>
        /// <param name="a_valueName">Identifier for the data in a_values which is to be stored.</param>
        /// <param name="a_values">Environment from which to fetch the values to store.</param>
        /// <returns>True iff the data has been set.</returns>
        public bool Set(object a_owner, string a_keyName, ITSValueArgument a_valueName, TSValueDictionary a_values);

        /// <summary>
        /// Get some data from the storage object.
        /// </summary>
        /// <param name="a_owner">The lock owner to enable internal lock holder checking.</param>
        /// <param name="a_keyName">Identifier for the stored data.</param>
        /// <param name="a_valueName">Identifier for where the data should be stored in a_values.</param>
        /// <param name="a_values">Environment to which to write the fetched values.</param>
        /// <returns>True iff the data has been fetched.</returns>
        public bool Get(object a_owner, string a_keyName, TSNamedValueArgument a_valueName, TSValueDictionary a_values);

        /// <summary>
        /// Check if the storage object has any data for the given key.
        /// </summary>
        /// <param name="a_owner">The lock owner to enable internal lock holder checking.</param>
        /// <param name="a_keyName">Identifier for the stored data.</param>
        /// <returns>True iff data with said identifier is present.</returns>
        public bool Has(object a_owner, string a_keyName);

        /// <summary>
        /// Remove the data for the given key from the storage object.
        /// </summary>
        /// <param name="a_owner">The lock owner to enable internal lock holder checking.</param>
        /// <param name="a_keyName">Identifier for the stored data.</param>
        /// <returns>True iff data with said identifier has been removed.</returns>
        public bool Remove(object a_owner, string a_keyName);
    }
}
