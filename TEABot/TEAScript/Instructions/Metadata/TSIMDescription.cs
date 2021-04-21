using System;
using System.Linq;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Metadata
{
    /// <summary>
    /// Sets the script or a parameter description.
    /// </summary>
    [TSKeyword("description")]
    public class TSIMDescription : TSIScriptMetadata
    {
        private string mDescription = String.Empty;

        public override void Apply(TSCompiledScript a_targetScript)
        {
            if (a_targetScript.Parameters.Count > 0)
            {
                // if any parameters exist, set the last parameter's data
                a_targetScript.Parameters.Last().Description = mDescription;
            }
            else
            {
                // if no parameter specifications exist yet, set the script description
                a_targetScript.Description = mDescription;
            }
        }

        protected override bool Parse(string a_instructionArguments)
        {
            mDescription = a_instructionArguments;
            return true;
        }
    }
}
