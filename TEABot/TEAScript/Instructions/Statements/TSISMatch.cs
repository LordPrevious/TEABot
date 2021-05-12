using System;
using System.Linq;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// Checks if a value matches a specific string, ignoring letter case
    /// </summary>
    [TSKeyword("match")]
    public class TSISMatch : TSIStatement
    {
        /// <summary>
        /// Name of the value to write the result to.
        /// </summary>
        private string mTargetName = String.Empty;

        /// <summary>
        /// Value to match against a string
        /// </summary>
        private string mValueToCheck = String.Empty;

        /// <summary>
        /// String to match the value against
        /// </summary>
        private string mStringToMatch = String.Empty;

        protected override bool Parse(string a_instructionArguments)
        {
            var validator = new TSValidator(ParsingBroadcaster);

            // split with whitespace preserved in tail
            var arguments = a_instructionArguments.Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);

            // check for argument count
            if (!validator.ContainsEnoughArguments(arguments, 3))
            {
                return false;
            }

            mTargetName = arguments[0];
            mValueToCheck = arguments[1];
            mStringToMatch = arguments[2];

            // first argument must be a variable name, as that's the target to write to
            validator.IsVariableName(mTargetName);
            // second argument must be a value name, as that's the value to match
            validator.IsValueName(mValueToCheck);
            return validator;
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            if (a_context.Values[mValueToCheck].TextValue.Equals(mStringToMatch, StringComparison.InvariantCultureIgnoreCase))
            {
                a_context.Values[mTargetName] = 1L;
            }
            else
            {
                a_context.Values[mTargetName] = 0L;
            }
            return TSFlow.Next;
        }

    }
}
