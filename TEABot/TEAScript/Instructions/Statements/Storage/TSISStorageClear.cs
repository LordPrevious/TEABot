using System;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements.Storage
{
    /// <summary>
    /// Clear a storage object and discard its contents.
    /// </summary>
    [TSKeyword("storage:clear")]
    public class TSISStorageClear : TSIStatement
    {
        /// <summary>
        /// Name of the storage object to open
        /// </summary>
        private string mStorageName = String.Empty;

        protected override bool Parse(string a_instructionArguments)
        {
            mStorageName = TrimmedInstructionArguments(a_instructionArguments);
            if (String.IsNullOrWhiteSpace(mStorageName))
            {
                ParsingBroadcaster?.Error("A storage object name must be specified.");
                return false;
            }
            return true;
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            if (a_context.Storage == null)
            {
                a_context.Broadcaster.Error("Unable to clear storage as no provider is given.");
            }
            else if (a_context.Storage.AcquireLock(a_context))
            {
                a_context.Storage.Clear(a_context, mStorageName);
                a_context.Storage.ReleaseLock(a_context);
            }
            return TSFlow.Next;
        }
    }
}
