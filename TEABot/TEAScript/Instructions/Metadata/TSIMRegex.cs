using System;
using System.Text.RegularExpressions;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Metadata
{
    /// <summary>
    /// Sets the script trigger regex pattern
    /// </summary>
    [TSKeyword("regex")]
    public class TSIMRegex : TSIScriptMetadata
    {
        private string mPattern = String.Empty;

        public override void Apply(TSCompiledScript a_targetScript)
        {
            a_targetScript.RegexPattern = mPattern;
        }

        protected override bool Parse(string a_instructionArguments)
        {
            mPattern = a_instructionArguments;
            if (String.IsNullOrWhiteSpace(mPattern))
            {
                ParsingBroadcaster?.Error("Regex pattern must not be empty.");
                return false;
            }
            // check if regex pattern can be parsed
            try
            {
                // parse and compile if valid, so later usage can be faster (compiled regex will be cached)
                Regex.Match(String.Empty, mPattern, RegexOptions.Compiled);
            }
            catch (ArgumentException ex)
            {
                ParsingBroadcaster?.Error("Failed to parse regex pattern: {0}", ex.Message);
                return false;
            }
            // pattern appears to be OK
            return true;
        }
    }
}
