using System;
using System.Linq;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// Drop a variable or wildcarded set of variables
    /// </summary>
    [TSKeyword("drop")]
    public class TSISDrop : TSIStatement
    {
        /// <summary>
        /// Name of the variable to drop, may use wildcards to drop multiple values
        /// </summary>
        private TSNamedValueArgument mTargetValue = new();

        protected override bool Parse(string a_instructionArguments)
        {
            if (SingleValueArgument(a_instructionArguments, out ITSValueArgument tmpTargetValue, true)
                && (tmpTargetValue is TSNamedValueArgument namedTargetValue)
                && VariableNameValueArgument(namedTargetValue, out string _))
            {
                mTargetValue = namedTargetValue;
                return true;
            }
            return false;
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            if (mTargetValue.HasWildcard)
            {
                // remove multiple values with common prefix
                var wildcardValues = a_context.Values.Keys.Where(k => k.StartsWith(mTargetValue.ValueName)).ToArray();
                foreach (var currentKey in wildcardValues)
                {
                    // add without common prefix
                    a_context.Values.Remove(currentKey);
                }
            }
            else
            {
                // remove single variable
                a_context.Values.Remove(mTargetValue.ValueName);
            }
            return TSFlow.Next;
        }
    }
}
