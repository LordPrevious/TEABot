using System;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Metadata
{
    /// <summary>
    /// Starts a new parameter specification for a required parameter.
    /// </summary>
    [TSKeyword("required")]
    public class TSIMRequiredParameter : TSIScriptMetadata
    {
        private string mName = String.Empty;

        public override void Apply(TSCompiledScript a_targetScript)
        {
            a_targetScript.Parameters.Add(new TSParameter(mName, true));
        }

        protected override bool Parse(string a_instructionArguments)
        {
            mName = SingleWordArgument(a_instructionArguments);
            return new TSValidator(ParsingBroadcaster).IsUnprefixedValueName(mName);
        }
    }
}
