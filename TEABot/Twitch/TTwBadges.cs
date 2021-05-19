using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.Twitch
{
    /// <summary>
    /// User badge information.
    /// Badges may indicate a user's state (e.g. broadcaster, moderator, ...)
    /// or be merely decorative (bits leader, ...)
    /// </summary>
    public class TTwBadges
    {
        #region Constructors

        /// <summary>
        /// initialize from a list of badges
        /// </summary>
        /// <param name="a_badges">The badges</param>
        public TTwBadges(IEnumerable<Badge> a_badges)
        {
            mBadges.AddRange(a_badges);
        }

        /// <summary>
        /// Parse twitch tags to extract badge information.
        /// </summary>
        /// <param name="a_badgeList">The "badges" tag value</param>
        /// <param name="a_badgeInfo">The "badge-info" tag value</param>
        public TTwBadges(string a_badgeList, string a_badgeInfo)
        {
            // allow null arguments
            var badgeList = a_badgeList ?? String.Empty;
            var badgeInfo = a_badgeInfo ?? String.Empty;

            // parse badge info
            var splitBadgeInfo = badgeInfo.Split(cAListSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            Dictionary<string, int> parsedBagdeInfo = new();
            foreach (var infoItem in splitBadgeInfo)
            {
                var splitInfoItem = infoItem.Split(cAVersionSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if ((splitInfoItem.Length >= 2)
                    && int.TryParse(splitInfoItem[1], out int infoVersion))
                {
                    parsedBagdeInfo[splitInfoItem[0]] = infoVersion;
                }
            }

            // parse badge list
            var splitBadgeList = badgeList.Split(cAListSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var badgeItem in splitBadgeList)
            {
                var splitBadgeItem = badgeItem.Split(cAVersionSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (splitBadgeItem.Any())
                {
                    mBadges.Add(new(
                        splitBadgeItem[0],
                        ((splitBadgeItem.Length > 1) && int.TryParse(splitBadgeItem[1], out int badgeVersion)) ? badgeVersion : 0,
                        parsedBagdeInfo.TryGetValue(splitBadgeItem[0], out int itemInfo) ? itemInfo : 0));
                }
            }
        }

        #endregion

        #region Public data

        /// <summary>
        /// List of all the badges specified by the source tags
        /// </summary>
        public IReadOnlyList<Badge> Badges { get { return mBadges; } }
        private readonly List<Badge> mBadges = new();

        #endregion

        #region Internal data

        private static readonly char cListSeparator = ',';
        private static readonly char[] cAListSeparator = new[] { cListSeparator };
        private static readonly char cVersionSeparator = '/';
        private static readonly char[] cAVersionSeparator = new[] { cVersionSeparator };

        #endregion

        #region Internal structures

        public readonly struct Badge
        {
            /// <summary>
            /// Badge name, e.g. "subscriber", "admin"...
            /// </summary>
            public readonly string Name;
            /// <summary>
            /// Badge version, e.g. subscription duration variants
            /// </summary>
            public readonly int Version;
            /// <summary>
            /// Badge info, e.g. number of months for "subscriber"
            /// </summary>
            public readonly int Info;

            /// <summary>
            /// Initialize a badge struct
            /// </summary>
            /// <param name="a_name">Badge name</param>
            /// <param name="a_version">Version</param>
            /// <param name="a_info">Info</param>
            public Badge(string a_name, int a_version, int a_info)
            {
                Name = a_name ?? throw new ArgumentNullException(nameof(a_name));
                Version = a_version;
                Info = a_info;
            }
        }

        #endregion
    }
}
