using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Metadata
{
    /// <summary>
    /// Sets the script execution interval
    /// </summary>
    [TSKeyword("periodic")]
    public class TSIMPeriodicExecutionInterval : TSIScriptMetadata
    {
        private long mIntervalSeconds = 0L;

        public override void Apply(TSCompiledScript a_targetScript)
        {
            a_targetScript.Interval = mIntervalSeconds;
        }

        protected override bool Parse(string a_instructionArguments)
        {
            if (!SingleNumberArgument(a_instructionArguments, out mIntervalSeconds))
            {
                return false;
            }
            if (mIntervalSeconds < 0)
            {
                ParsingBroadcaster?.Error("The periodic execution interval must not be negative: Got {0}.", mIntervalSeconds);
                return false;
            }
            if (mIntervalSeconds == 0)
            {
                ParsingBroadcaster?.Warn("A periodic execution interval of 0 disables periodic execution and can be omitted.");
            }
            return true;
        }
    }
}
