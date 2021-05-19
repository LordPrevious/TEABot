using System;
using System.Collections.Generic;
using System.Drawing;

namespace TEABot.Twitch
{
    /// <summary>
    /// Regular chat message tags w/ extended sender and message information
    /// </summary>
    class TTwMessageTags
    {
        #region Public data

        /// <summary>
        /// Sender badges
        /// </summary>
        public TTwBadges Badges { get; private set; } = null;

        /// <summary>
        /// Username color
        /// </summary>
        public Color? UserColor { get; private set; } = null;

        /// <summary>
        /// User's display name
        /// </summary>
        public string DisplayName { get; private set; } = null;

        /// <summary>
        /// Message emotes
        /// </summary>
        public TTwEmotes Emotes { get; private set; } = null;

        /// <summary>
        /// Unique message ID, e.g. for later deleting a specific message
        /// </summary>
        public string MessageId { get; private set; } = null;

        /// <summary>
        /// The user's ID
        /// </summary>
        public string UserId { get; private set; } = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct from IRC message tags
        /// </summary>
        /// <param name="a_tags">The message tags</param>
        /// <param name="a_message">The message contents for which the tags are parsed</param>
        public TTwMessageTags(Dictionary<string, string> a_tags, string a_message)
        {
            if (a_tags == null) throw new ArgumentNullException(nameof(a_tags));

            // badges
            if (a_tags.TryGetValue(TTwTagNames.BADGES, out string tagBadges))
            {
                if (!a_tags.TryGetValue(TTwTagNames.BADGES, out string tagBadgeInfo))
                {
                    tagBadgeInfo = null;
                }
                Badges = new (tagBadges, tagBadgeInfo);
            }
            // color
            if (a_tags.TryGetValue(TTwTagNames.COLOR, out string tagColor))
            {
                // try to parse tag value as HTML color, which supports hex color codes
                try
                {
                    UserColor = ColorTranslator.FromHtml(tagColor);
                }
                catch
                {
                    UserColor = null;
                }
            }
            // display name
            if (a_tags.TryGetValue(TTwTagNames.DISPLAY_NAME, out string tagDisplayName))
            {
                DisplayName = tagDisplayName;
            }
            // emotes
            if (a_tags.TryGetValue(TTwTagNames.EMOTES, out string tagEmotes))
            {
                Emotes = new(tagEmotes, a_message);
            }
            // message id
            if (a_tags.TryGetValue(TTwTagNames.MSG_ID, out string tagMessageId))
            {
                MessageId = tagMessageId;
            }
            // user id
            if (a_tags.TryGetValue(TTwTagNames.USER_ID, out string tagUserId))
            {
                UserId = tagUserId;
            }
        }

        #endregion
    }
}
