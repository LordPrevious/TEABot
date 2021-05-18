using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements.List
{
    /// <summary>
    /// Add an item to the current list
    /// </summary>
    [TSKeyword("list:add")]
    public class TSISListAdd : TSIStatement
    {
        /// <summary>
        /// Value to add to the list
        /// </summary>
        private TSNamedValueArgument mSourceValue = new();

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            if (a_context.Lists == null)
            {
                a_context.Broadcaster.Error("A list manager is required.");
            }
            else
            {
                a_context.Lists.AddToList(mSourceValue);
            }
            return TSFlow.Next;
        }

        protected override bool Parse(string a_instructionArguments)
        {
            if (SingleValueArgument(a_instructionArguments, out ITSValueArgument tmpTargetValue, true)
                && (tmpTargetValue is TSNamedValueArgument namedTargetValue))
            {
                mSourceValue = namedTargetValue;
                return true;
            }
            return false;
        }
    }
}
