using System;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements.Storage
{
    /// <summary>
    /// Open a storage object as a list and configure the list manager to use it
    /// </summary>
    [TSKeyword("storage:list")]
    public class TSISStorageList : TSIStatement
    {
        /// <summary>
        /// Name of the storage object to open
        /// </summary>
        private string mStorageName = String.Empty;

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            if (a_context.Storage == null)
            {
                a_context.Broadcaster.Error("Unable to open storage as no provider is given.");
            }
            if (a_context.Lists == null)
            {
                a_context.Broadcaster.Error("Unable to open list as no manager is given.");
            }
            else if (a_context.Storage.HoldsLock(a_context))
            {
                a_context.Broadcaster.Error("Opening storage while another is already opened... closing old storage.");
                a_context.Storage.Close(a_context);
                a_context.Lists.OpenStorageList(mStorageName, a_context.Storage, a_context);
            }
            else if (a_context.Storage.AcquireLock(a_context))
            {
                a_context.Lists.OpenStorageList(mStorageName, a_context.Storage, a_context);
            }
            return TSFlow.Next;
        }

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
    }
}
