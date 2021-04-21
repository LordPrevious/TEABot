using System;
using System.Linq;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Metadata
{
    /// <summary>
    /// Sets the default value for the last parameter.
    /// </summary>
    [TSKeyword("default")]
    public class TSIMParameterDefaultValue : TSIScriptMetadata
    {
        private string mValue = String.Empty;

        public override void Apply(TSCompiledScript a_targetScript)
        {
            if (a_targetScript.Parameters.Count > 0)
            {
                a_targetScript.Parameters.Last().DefaultValue = mValue;
            }
            else
            {
                ParsingBroadcaster?.Error("No parameter found to apply a default value to.");
            }
        }

        protected override bool Parse(string a_instructionArguments)
        {
            mValue = a_instructionArguments;
            return true;
        }
    }
}
