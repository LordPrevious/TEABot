using System;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements.Storage
{
    /// <summary>
    /// Open a storage object so it can be written to or read from,
    /// acquiring a mutex to prevent multiple threads to access storage at the same time.
    /// Storage remains open until closed via storage.close, end or sleep.
    /// </summary>
    [TSKeyword("storage:open")]
    public class TSISStorageOpen : TSIStatement
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
                a_context.Broadcaster.Error("Unable to open storage as no provider is given.");
            }
            else if (a_context.Storage.HoldsLock(a_context))
            {
                a_context.Broadcaster.Error("Opening storage while another is already opened... closing old storage.");
                a_context.Storage.Close(a_context);
                a_context.Storage.Open(a_context, mStorageName);
            }
            else if (a_context.Storage.AcquireLock(a_context))
            {
                a_context.Storage.Open(a_context, mStorageName);
            }
            return TSFlow.Next;
        }
    }
}
