using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.Twitch
{
    /// <summary>
    /// Message emote information.
    /// An emote has an ID which is required to find its image on Twitch
    /// and a name by which it is embedded in a message.
    /// </summary>
    public class TTwEmotes
    {
        #region Constructors

        /// <summary>
        /// initialize from a list of emotes
        /// </summary>
        /// <param name="a_emotes">The emotes</param>
        public TTwEmotes(IEnumerable<Emote> a_emotes)
        {
            mEmotes.AddRange(a_emotes);
        }

        /// <summary>
        /// Parse twitch tags to extract emote information.
        /// </summary>
        /// <param name="a_emotes">The "emotes" tag value</param>
        /// <param name="a_message">The chat message to extract emote names from</param>
        public TTwEmotes(string a_emotes, string a_message)
        {
            // allow null arguments
            var emoteList = a_emotes ?? String.Empty;
            var message = a_message ?? String.Empty;

            // parse emote list
            var splitEmoteList = emoteList.Split(cAEmoteListSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var emoteItem in splitEmoteList)
            {
                // split "id:position"
                var splitEmoteItem = emoteItem.Split(cPositionSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (splitEmoteItem.Length >= 2)
                {
                    var emoteId = splitEmoteItem[0];
                    // split position list
                    var splitPositionList = splitEmoteItem[1].Split(cPositionListSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var positionItem in splitPositionList)
                    {
                        // split "start-end"
                        var splitPositionItem = positionItem.Split(cARangeSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        if ((splitPositionItem.Length == 2)
                            && int.TryParse(splitPositionItem[0], out int rangeStart)
                            && int.TryParse(splitPositionItem[1], out int rangeEnd))
                        {
                            // ensure ranges are within the message string and range < rangeEnd
                            var emoteName = String.Empty;
                            if (String.IsNullOrEmpty(message))
                            {
                                rangeStart = 0;
                                rangeEnd = 0;
                            }
                            else
                            {
                                rangeEnd = (rangeEnd < 0) ? 0 : Math.Min(rangeEnd, message.Length - 1);
                                rangeStart = (rangeStart < 0) ? 0 : Math.Min(rangeStart, rangeEnd);
                                // extract name from message
                                emoteName = message[rangeStart..(rangeEnd + 1)];
                            }

                            // add to emote list
                            mEmotes.Add(new Emote(emoteId, emoteName, rangeStart, rangeEnd));
                        }
                    }
                }
            }
        }

        #endregion

        #region Public data

        /// <summary>
        /// List of all the emotes specified by the source tags
        /// </summary>
        public IReadOnlyList<Emote> Emotes { get { return mEmotes; } }
        private readonly List<Emote> mEmotes = new();

        #endregion

        #region Internal data

        private static readonly char cEmoteListSeparator = '/';
        private static readonly char[] cAEmoteListSeparator = new[] { cEmoteListSeparator };
        private static readonly char cPositionSeparator = ':';
        private static readonly char[] cAPositionSeparator = new[] { cPositionSeparator };
        private static readonly char cPositionListSeparator = ',';
        private static readonly char[] cAPositionListSeparator = new[] { cPositionListSeparator };
        private static readonly char cRangeSeparator = '-';
        private static readonly char[] cARangeSeparator = new[] { cRangeSeparator };

        #endregion

        #region Internal structures

        public readonly struct Emote
        {
            /// <summary>
            /// Emote ID via which its images can be accessed on Twitch
            /// </summary>
            public readonly string Id;
            /// <summary>
            /// Emote name via which it is embedded in the message
            /// </summary>
            public readonly string Name;
            /// <summary>
            /// Position of the first character of the emote occurrence within the chat message
            /// </summary>
            public readonly int Start;
            /// <summary>
            /// Position of the last character of the emote occurrence within the chat message
            /// </summary>
            public readonly int End;

            /// <summary>
            /// Initialize an emote struct
            /// </summary>
            /// <param name="a_id">Emote ID</param>
            /// <param name="a_name">Emote name</param>
            /// <param name="a_start">Position of the first character in the chat message</param>
            /// <param name="a_end">Position of the last character in the chat message</param>
            public Emote(string a_id, string a_name, int a_start, int a_end)
            {
                Id = a_id ?? throw new ArgumentNullException(nameof(a_id));
                Name = a_name ?? throw new ArgumentNullException(nameof(a_name)); ;
                Start = a_start;
                End = a_end;
            }
        }

        #endregion
    }
}
