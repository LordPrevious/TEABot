using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements.List
{
    /// <summary>
    /// Limit the number of list items, keeping only the tail
    /// </summary>
    [TSKeyword("list:limit")]
    public class TSISListLimit : TSIStatement
    {
        /// <summary>
        /// Maximal number of items to keep
        /// </summary>
        private ITSValueArgument mCountValue = new TSConstantStringArgument();

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            if (a_context.Lists == null)
            {
                a_context.Broadcaster.Error("A list manager is required.");
            }
            else
            {
                a_context.Lists.LimitListSize(mCountValue.GetValue(a_context.Values));
            }
            return TSFlow.Next;
        }

        protected override bool Parse(string a_instructionArguments)
        {
            return SingleValueArgument(a_instructionArguments, out mCountValue);
        }
    }
}
