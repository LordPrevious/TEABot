using System;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements.List
{
    /// <summary>
    /// Load a temporary list.
    /// Creates a new empty list of no list of the given name exists yet.
    /// Internally provided special lists use '#' as a name prefix.
    /// </summary>
    [TSKeyword("list:load")]
    public class TSISListLoad : TSIStatement
    {
        /// <summary>
        /// Name of the list to open
        /// </summary>
        private string mListName = String.Empty;

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            if (a_context.Lists == null)
            {
                a_context.Broadcaster.Error("A list manager is required.");
            }
            else
            {
                a_context.Lists.LoadList(mListName);
            }
            return TSFlow.Next;
        }

        protected override bool Parse(string a_instructionArguments)
        {
            mListName = TrimmedInstructionArguments(a_instructionArguments);
            if (String.IsNullOrWhiteSpace(mListName))
            {
                ParsingBroadcaster?.Error("A list name must be specified.");
                return false;
            }
            return true;
        }
    }
}
