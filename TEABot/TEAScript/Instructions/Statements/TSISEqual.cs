using System;
using System.Linq;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// Checks if some numbers are equal to each other
    /// </summary>
    [TSKeyword("equal")]
    public class TSISEqual : TSIStatement
    {
        /// <summary>
        /// Name of the value to write the result to.
        /// </summary>
        private string mTargetName = String.Empty;

        /// <summary>
        /// Arguments to check for equality
        /// </summary>
        private ITSValueArgument[] mArguments = new ITSValueArgument[0];

        protected override bool Parse(string a_instructionArguments)
        {
            var validator = new TSValidator(ParsingBroadcaster);

            ITSValueArgument[] valueArguments;
            if (!SplitValueArguments(a_instructionArguments, out valueArguments)) return false;

            // check for argument count
            if (!validator.ContainsEnoughArguments(valueArguments, 2, true))
            {
                return false;
            }
            if (valueArguments.Length == 2)
            {
                ParsingBroadcaster?.Warn("Equal instruction with single operand will always simply write 1.", valueArguments.Length);
            }

            // first argument must be a variable name, as that's the target to write to
            if (!VariableNameValueArgument(valueArguments[0], out mTargetName)) return false;

            // actual calculation arguments are the rest
            mArguments = valueArguments.Skip(1).ToArray();

            // ensure the target is a variable name
            validator.IsVariableName(mTargetName);
            return validator;
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            a_context.Values[mTargetName] = CalculateResult(a_context);
            return TSFlow.Next;
        }

        /// <summary>
        /// Calculates the result value of applying the math instruction on the given context.
        /// </summary>
        /// <param name="a_context">The execution context</param>
        /// <returns>The result of the calculation.</returns>
        private long CalculateResult(TSExecutionContext a_context)
        {
            // get numerical values via context
            var numberArguments = mArguments.Select(a => a.GetValue(a_context)).ToArray();
            // operate on the actual values
            switch (mArguments.Length)
            {
                case 0:
                    return 0L;
                case 1:
                    return 1L;
                case 2:
                    if (numberArguments[0] == numberArguments[1]) return 1L;
                    return 0L;
                default:
                    {
                        var first = numberArguments[0];
                        var rest = numberArguments.Skip(1);
                        if (rest.All(a => a == first)) return 1L;
                        return 0L;
                    }
            }
        }

    }
}
