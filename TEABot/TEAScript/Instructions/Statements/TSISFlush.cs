using System;
using System.Linq;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// Flush the output buffer.
    /// </summary>
    [TSKeyword("flush")]
    public class TSISFlush : TSISNoArguments
    {
        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            a_context.Flush();
            return TSFlow.Next;
        }
    }
}
