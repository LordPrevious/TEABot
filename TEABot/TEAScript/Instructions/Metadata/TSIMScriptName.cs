using System;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Metadata
{
    /// <summary>
    /// Sets the script name
    /// </summary>
    [TSKeyword("script")]
    public class TSIMScriptName : TSIScriptMetadata
    {
        private string mName = String.Empty;

        public override void Apply(TSCompiledScript a_targetScript)
        {
            a_targetScript.Name = mName;
        }

        protected override bool Parse(string a_instructionArguments)
        {
            mName = TrimmedInstructionArguments(a_instructionArguments);
            return (mName.Length > 0);
        }
    }
}
