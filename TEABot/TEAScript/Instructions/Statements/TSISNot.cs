using System;
using System.Linq;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// Logical NOT operation on a value, storing the result in a variable.
    /// 0 is false, everything else is true
    /// </summary>
    [TSKeyword("not")]
    public class TSISNot : TSIStatement
    {
        /// <summary>
        /// Name of the target variable to store the value in
        /// </summary>
        private string mTargetName = String.Empty;

        /// <summary>
        /// The value to store
        /// </summary>
        private ITSValueArgument mSourceValue = new TSConstantValueArgument(0L);

        protected override bool Parse(string a_instructionArguments)
        {
            var validator = new TSValidator(ParsingBroadcaster);

            ITSValueArgument[] valueArguments;
            if (!SplitValueArguments(a_instructionArguments, out valueArguments)) return false;
            if (!validator.ContainsEnoughArguments(valueArguments, 2))
            {
                return false;
            }
            if (valueArguments.Length == 1)
            {
                // source is target
                mSourceValue = valueArguments[0];
            }
            else
            {
                // source and target are separate
                mSourceValue = valueArguments[1];
            }
            if (!VariableNameValueArgument(valueArguments[0], out mTargetName)) return false;
            mSourceValue = valueArguments[1];
            return true;
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            a_context.Values[mTargetName] = mSourceValue.GetValue(a_context);
            return TSFlow.Next;
        }
    }
}
