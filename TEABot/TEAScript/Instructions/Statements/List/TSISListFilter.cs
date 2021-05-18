using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements.List
{
    /// <summary>
    /// Filter the current list by one of the item's keys
    /// </summary>
    [TSKeyword("list:filter")]
    public class TSISListFilter : TSIStatement
    {
        /// <summary>
        /// Key to filter by
        /// </summary>
        private ITSValueArgument mKeyName = new TSConstantStringArgument();

        /// <summary>
        /// Value to check for
        /// </summary>
        private ITSValueArgument mDesiredValue = new TSConstantNumberArgument();

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            if (a_context.Lists == null)
            {
                a_context.Broadcaster.Error("A list manager is required.");
            }
            else
            {
                a_context.Lists.FilterList(mKeyName.GetValue(a_context.Values), mDesiredValue);
            }
            return TSFlow.Next;
        }

        protected override bool Parse(string a_instructionArguments)
        {
            var validator = new TSValidator(ParsingBroadcaster);

            var words = SplitWordsArguments(a_instructionArguments);
            return (validator.ContainsEnoughArguments(words, 2)
                && SingleValueArgument(words[0], out mKeyName)
                && SingleValueArgument(words[1], out mDesiredValue));
        }
    }
}
