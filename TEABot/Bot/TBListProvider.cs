using System.Collections.Generic;
using TEABot.TEAScript;

namespace TEABot.Bot
{
    /// <summary>
    /// List provider implementation for the chat bot,
    /// may supply a list of usernames or other shenanigans.
    /// Manges custom temporary lists as expected.
    /// </summary>
    public class TBListProvider : ITSListProvider
    {
        #region Public data

        /// <summary>
        /// List of encountered users.
        /// 
        /// Keys:
        /// * "name": IRC nickname
        /// * "channel": IRC channel name the user was encountered in
        /// </summary>
        public TSValueList Users = new();
        public static readonly string cListNameUsers = TSConstants.SpecialListPrefix + "users";

        #endregion

        #region Constructor
        
        /// <summary>
        /// Initializes the provider with the default lists
        /// </summary>
        public TBListProvider()
        {
            mLists[cListNameUsers] = Users;
        }

        #endregion

        #region Private data

        /// <summary>
        /// The lists we can provide
        /// </summary>
        private readonly Dictionary<string, TSValueList> mLists = new();

        #endregion

        #region ITSListProvider implementation

        public TSValueList CreateList(string a_listName)
        {
            lock (mLists)
            {
                if (a_listName.StartsWith(TSConstants.SpecialListPrefix)
                    || mLists.ContainsKey(a_listName))
                {
                    // list name is not allowed for custom lists
                    // or list name is already used
                    return null;
                }
                TSValueList result = new();
                mLists.Add(a_listName, result);
                return result;
            }
        }

        public TSValueList GetList(string a_listName)
        {
            lock (mLists)
            {
                if (mLists.TryGetValue(a_listName, out TSValueList result))
                {
                    return result;
                }
            }
            return null;
        }

        #endregion
    }
}
