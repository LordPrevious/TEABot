using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements.List
{
    /// <summary>
    /// Sort the list by the given key
    /// </summary>
    [TSKeyword("list:sort")]
    public class TSISListSort : TSIStatement
    {
        /// <summary>
        /// Key to order by
        /// </summary>
        private ITSValueArgument mKeyName = new TSConstantStringArgument();

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            if (a_context.Lists == null)
            {
                a_context.Broadcaster.Error("A list manager is required.");
            }
            else
            {
                a_context.Lists.SortListItems(mKeyName.GetValue(a_context.Values));
            }
            return TSFlow.Next;
        }

        protected override bool Parse(string a_instructionArguments)
        {
            return SingleValueArgument(a_instructionArguments, out mKeyName);
        }
    }
}
