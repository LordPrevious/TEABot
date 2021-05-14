using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.TEAScript
{
    /// <summary>
    /// Dictionary<string, TSValue> but automatically creates values for missing keys upon accessing them.
    /// </summary>
    public class TSValueDictionary : IDictionary<string, TSValue>
    {
        private readonly Dictionary<string, TSValue> mBackingDictionary = new();

        public TSValue this[string key]
        {
            get
            {
                if (!mBackingDictionary.TryGetValue(key, out TSValue result))
                {
                    result = TSValue.Empty;
                    mBackingDictionary[key] = result;
                }
                return result;
            }
            set
            {
                mBackingDictionary[key] = value;
            }
        }

        public ICollection<string> Keys => mBackingDictionary.Keys;

        public ICollection<TSValue> Values => mBackingDictionary.Values;

        public int Count => mBackingDictionary.Count;

        public bool IsReadOnly => false;

        public void Add(string key, TSValue value)
        {
            mBackingDictionary.Add(key, value);
        }

        public void Add(KeyValuePair<string, TSValue> item)
        {
            ((IDictionary<string, TSValue>)mBackingDictionary).Add(item);
        }

        public void Clear()
        {
            mBackingDictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, TSValue> item)
        {
            return mBackingDictionary.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return mBackingDictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, TSValue>[] array, int arrayIndex)
        {
            ((IDictionary<string, TSValue>)mBackingDictionary).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, TSValue>> GetEnumerator()
        {
            return mBackingDictionary.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return mBackingDictionary.Remove(key);
        }

        public bool Remove(KeyValuePair<string, TSValue> item)
        {
            return ((IDictionary<string, TSValue>)mBackingDictionary).Remove(item);
        }

        public bool TryGetValue(string key, out TSValue value)
        {
            return mBackingDictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)mBackingDictionary).GetEnumerator();
        }
    }
}
