using System;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements.Storage
{
    /// <summary>
    /// Save the storage object to disk
    /// </summary>
    [TSKeyword("storage:save")]
    public class TSISStorageSave : TSISNoArguments
    {
        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            a_context.Storage?.Save(a_context);
            return TSFlow.Next;
        }
    }
}
