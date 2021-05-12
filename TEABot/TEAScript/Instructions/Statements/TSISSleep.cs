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
        private ITSValueArgument mIntervalSeconds = new TSConstantNumberArgument(0L);

        protected override bool Parse(string a_instructionArguments)
        {
            return SingleValueArgument(a_instructionArguments, out mIntervalSeconds);
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            // ensure lock is released to prevent slowing down anyone else trying to access storage
            if (a_context.Storage?.HoldsLock(a_context)??false)
            {
                a_context.Broadcaster.Warn("Delayed execution closes open storage.");
                a_context.Storage.Close(a_context);
                a_context.Storage.ReleaseLock(a_context);
            }
            // delay further execution
            return TSFlow.DelayNext(mIntervalSeconds.GetValue(a_context));
        }
    }
}
