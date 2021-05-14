using System;
using System.Linq;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// End script execution, flushing the output buffer.
    /// </summary>
    [TSKeyword("end")]
    public class TSISEnd : TSISNoArguments
    {
        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            // ensure storage is cleaned up and lock is released
            if (a_context.Storage?.HoldsLock(a_context)??false)
            {
                a_context.Broadcaster.Warn("Ending script while storage ist still open.");
                a_context.Storage.Close(a_context);
                a_context.Storage.ReleaseLock(a_context);
            }
            // flush any pending output
            a_context.Flush();
            return TSFlow.Exit;
        }
    }
}
