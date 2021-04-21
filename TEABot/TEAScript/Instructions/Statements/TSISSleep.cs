using System;
using System.Linq;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// Set the value of variable
    /// </summary>
    [TSKeyword("sleep")]
    public class TSISSleep : TSIStatement
    {
        /// <summary>
        /// Number of seconds to wait before executing the next statement
        /// </summary>
        private ITSValueArgument mIntervalSeconds = new TSConstantValueArgument(0L);

        protected override bool Parse(string a_instructionArguments)
        {
            return SingleValueArgument(a_instructionArguments, out mIntervalSeconds);
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            return TSFlow.DelayNext(mIntervalSeconds.GetValue(a_context));
        }
    }
}
