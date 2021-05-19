using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements.List
{
    /// <summary>
    /// Shuffle the list items
    /// </summary>
    [TSKeyword("list:shuffle")]
    public class TSISListShuffle : TSISNoArguments
    {
        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            if (a_context.Lists == null)
            {
                a_context.Broadcaster.Error("A list manager is required.");
            }
            else
            {
                a_context.Lists.ShuffleListItems();
            }
            return TSFlow.Next;
        }
    }
}
