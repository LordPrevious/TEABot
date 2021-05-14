using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// Base class for calculation statements.
    /// </summary>
    public abstract class TSISMath : TSIStatement
    {
        /// <summary>
        /// Name of the value to write the result to.
        /// </summary>
        private string mTargetName = String.Empty;

        /// <summary>
        /// Arguments to use in the calculation.
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
                ParsingBroadcaster?.Warn("Math instruction with single operand will always simply write said operand.", valueArguments.Length);
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
            var numberArguments = mArguments.Select(a => a.GetValue(a_context.Values)).ToArray();
            // operate on the actual values
            switch (mArguments.Length)
            {
                case 0:
                    return 0L;
                case 1:
                    return numberArguments[0];
                case 2:
                    return Operator(numberArguments[0], numberArguments[1]);
                default:
                    {
                        var first = numberArguments[0];
                        var rest = numberArguments.Skip(1);
                        return rest.Aggregate(first, (a, b) => Operator(a, b));
                    }
            }
        }

        /// <summary>
        /// Subclasses must provide the operator to perform the actual operations.
        /// </summary>
        /// <param name="a_operandA">The first operand</param>
        /// <param name="a_operandB">The second operand</param>
        /// <returns>The result of applying the specific implementation's operator to the operands.</returns>
        protected abstract long Operator(long a_operandA, long a_operandB);
        
    }
}
