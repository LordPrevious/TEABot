using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// Copy instruction to copy a value to a variable
    /// </summary>
    [TSKeyword("range")]
    public class TSISRange : TSIStatement
    {
        /// <summary>
        /// Name of the value to force into bounds
        /// </summary>
        private string mValueName = String.Empty;

        /// <summary>
        /// One bound of the range to limit the value to
        /// </summary>
        private ITSValueArgument mFirstLimit = new TSConstantNumberArgument(0L);

        /// <summary>
        /// The other bound of the range to limit the value to
        /// </summary>
        private ITSValueArgument mSecondLimit = new TSConstantNumberArgument(0L);

        protected override bool Parse(string a_instructionArguments)
        {
            if (!SplitValueArguments(a_instructionArguments, out ITSValueArgument[] valueArguments)) return false;

            var validator = new TSValidator(ParsingBroadcaster);
            if (!validator.ContainsEnoughArguments(valueArguments, 3)) return false;

            if (!VariableNameValueArgument(valueArguments[0], out mValueName)) return false;

            mFirstLimit = valueArguments[1];
            mSecondLimit = valueArguments[2];
            return true;
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            // get current values
            long currentValue = a_context.Values[mValueName];
            long firstLimit = mFirstLimit.GetValue(a_context.Values);
            long secondLimit = mSecondLimit.GetValue(a_context.Values);
            // get upper and lower limit
            long lowerLimit;
            long upperLimit;
            if (firstLimit > secondLimit)
            {
                lowerLimit = secondLimit;
                upperLimit = firstLimit;
            }
            else
            {
                lowerLimit = firstLimit;
                upperLimit = secondLimit;
            }
            // apply limits
            if (currentValue < lowerLimit)
            {
                a_context.Values[mValueName] = lowerLimit;
            }
            else if (currentValue > upperLimit)
            {
                a_context.Values[mValueName] = upperLimit;
            }
            // proceed
            return TSFlow.Next;
        }
    }
}
