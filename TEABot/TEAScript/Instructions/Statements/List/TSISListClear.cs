using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements.List
{
    /// <summary>
    /// Clear the current list, discarding all items
    /// </summary>
    [TSKeyword("list:clear")]
    public class TSISListClear : TSISNoArguments
    {
        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            if (a_context.Lists == null)
            {
                a_context.Broadcaster.Error("A list manager is required.");
            }
            else
            {
                a_context.Lists.ClearList();
            }
            return TSFlow.Next;
        }
    }
}
