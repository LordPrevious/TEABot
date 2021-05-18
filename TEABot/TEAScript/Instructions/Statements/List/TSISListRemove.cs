using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements.List
{
    /// <summary>
    /// Remove the list item at the given index.
    /// </summary>
    [TSKeyword("list:remove")]
    public class TSISListRemove : TSIStatement
    {
        /// <summary>
        /// Index to remove
        /// </summary>
        private ITSValueArgument mIndexValue = new TSConstantNumberArgument();

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            if (a_context.Lists == null)
            {
                a_context.Broadcaster.Error("A list manager is required.");
            }
            else
            {
                a_context.Lists.RemoveListItem(mIndexValue.GetValue(a_context.Values));
            }
            return TSFlow.Next;
        }

        protected override bool Parse(string a_instructionArguments)
        {
            return SingleValueArgument(a_instructionArguments, out mIndexValue);
        }
    }
}
