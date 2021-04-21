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
            a_context.Flush();
            return TSFlow.Exit;
        }
    }
}
