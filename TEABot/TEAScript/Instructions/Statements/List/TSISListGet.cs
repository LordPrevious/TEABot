using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements.List
{
    /// <summary>
    /// Get the list item with at given index
    /// </summary>
    [TSKeyword("list:get")]
    public class TSISListGet : TSIStatement
    {
        /// <summary>
        /// Value with the desired list index
        /// </summary>
        private ITSValueArgument mIndexValue = new TSConstantNumberArgument();

        /// <summary>
        /// Name of the variable to store the data in, may use wildcards for complex objects
        /// </summary>
        private TSNamedValueArgument mTargetValue = new();

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            if (a_context.Lists == null)
            {
                a_context.Broadcaster.Error("A list manager is required.");
            }
            else
            {
                a_context.Lists?.GetListItem(mIndexValue.GetValue(a_context.Values), mTargetValue);
            }
            return TSFlow.Next;
        }

        protected override bool Parse(string a_instructionArguments)
        {
            var validator = new TSValidator(ParsingBroadcaster);

            var words = SplitWordsArguments(a_instructionArguments);
            if (validator.ContainsEnoughArguments(words, 2)
                && SingleValueArgument(words[0], out ITSValueArgument tmpTargetValue, true)
                && (tmpTargetValue is TSNamedValueArgument namedTargetValue)
                && VariableNameValueArgument(namedTargetValue, out string _)
                && SingleValueArgument(words[1], out mIndexValue))
            {
                mTargetValue = namedTargetValue;
                return true;
            }
            return false;
        }
    }
}
