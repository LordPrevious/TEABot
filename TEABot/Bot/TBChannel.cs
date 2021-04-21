using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TEABot.TEAScript;

namespace TEABot.Bot
{
    /// <summary>
    /// Describes an IRC channel the bot operates on
    /// </summary>
    public class TBChannel
    {
        /// <summary>
        /// The channel name
        /// </summary>
        public string Name
        {
            get { return mName; }
            set
            {
                mName = value;
                IrcName = "#" + mName;
            }
        }
        private string mName = String.Empty;

        /// <summary>
        /// The channel name in IRC format
        /// </summary>
        public string IrcName { get; private set; } = String.Empty;

        /// <summary>
        /// The channel configuration
        /// </summary>
        public TBConfiguration Configuration = new TBConfiguration();

        /// <summary>
        /// Map from trigger command to script
        /// </summary>
        public Dictionary<string, TSCompiledScript> TriggeredScripts = new Dictionary<string, TSCompiledScript>();

        /// <summary>
        /// Map from trigger regex to script
        /// </summary>
        public Dictionary<Regex, TSCompiledScript> RegexScripts = new Dictionary<Regex, TSCompiledScript>();

        /// <summary>
        /// Periodically executed scripts
        /// </summary>
        public List<TSCompiledScript> PeriodicScripts = new List<TSCompiledScript>();

        /// <summary>
        /// Whether the channel has been joined and can be sent to or not
        /// </summary>
        public bool Joined = false;

        /// <summary>
        /// Timestamps for the last couple of sent messages for flood control
        /// </summary>
        public List<DateTime> SendTimestamps = new List<DateTime>();
    }
}
