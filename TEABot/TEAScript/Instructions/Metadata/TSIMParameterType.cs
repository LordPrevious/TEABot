using System.Linq;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Metadata
{
    /// <summary>
    /// Sets the expected value type for the last parameter.
    /// </summary>
    [TSKeyword("type")]
    public class TSIMParameterType : TSIScriptMetadata
    {
        private readonly TSEParameterType mType = TSEParameterType.STRING;

        public override void Apply(TSCompiledScript a_targetScript)
        {
            if (a_targetScript.Parameters.Count > 0)
            {
                a_targetScript.Parameters.Last().Type = mType;
            }
            else
            {
                ParsingBroadcaster?.Error("No parameter found to apply a parameter type to.");
            }
        }

        protected override bool Parse(string a_instructionArguments)
        {
            var typeWord = SingleWordArgument(a_instructionArguments);

            var parameterType = TSEParameterTypeExtensions.FromString(typeWord);

            if (parameterType == TSEParameterType.UNKNOWN)
            {
                ParsingBroadcaster?.Error("Given parameter type \"{0}\" is unknown", typeWord);
                return false;
            }

            return true;
        }
    }
}
