using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Metadata
{
    /// <summary>
    /// Sets special execution triggers
    /// </summary>
    [TSKeyword("special")]
    public class TSIMSpecialTrigger : TSIScriptMetadata
    {
        #region Public data

        /// <summary>
        /// True iff the script should be triggered on received twitch emotes
        /// </summary>
        public bool TwitchEmotes { get; private set; } = false;

        #endregion

        #region TSIScriptMetadata implementation

        public override void Apply(TSCompiledScript a_targetScript)
        {
            a_targetScript.TwitchEmotes = TwitchEmotes;
        }

        protected override bool Parse(string a_instructionArguments)
        {
            var words = SplitWordsArguments(a_instructionArguments);
            if (!(new TSValidator(ParsingBroadcaster).ContainsEnoughArguments(words, 1, true))) return false;
            foreach (var trigger in words)
            {
                switch (trigger.ToLowerInvariant())
                {
                    case "twitch:emotes":
                        TwitchEmotes = true;
                        break;
                    default:
                        ParsingBroadcaster.Warn("Unknown special trigger encountered: \"{0}\"", trigger);
                        break;
                }
            }
            return true;
        }

        #endregion
    }
}
