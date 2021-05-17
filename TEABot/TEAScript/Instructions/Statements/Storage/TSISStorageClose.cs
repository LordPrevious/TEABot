using System;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements.Storage
{
    /// <summary>
    /// Close the storage object, releasing the mutex
    /// </summary>
    [TSKeyword("storage:close")]
    public class TSISStorageClose : TSISNoArguments
    {
        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            a_context.Storage?.Close(a_context);
            a_context.Storage?.ReleaseLock(a_context);
            return TSFlow.Next;
        }
    }
}
