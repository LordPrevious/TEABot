using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements.List
{
    /// <summary>
    /// Get the previous item in the list
    /// </summary>
    [TSKeyword("list:previous")]
    public class TSISListPrevious : TSIStatement
    {
        /// <summary>
        /// Variable to store the result in
        /// </summary>
        private TSNamedValueArgument mTargetValue = new();

        protected override bool Parse(string a_instructionArguments)
        {
            if (SingleValueArgument(a_instructionArguments, out ITSValueArgument tmpTargetValue, true)
                && (tmpTargetValue is TSNamedValueArgument namedTargetValue)
                && VariableNameValueArgument(namedTargetValue, out string _))
            {
                mTargetValue = namedTargetValue;
                return true;
            }
            return false;
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            if (a_context.Lists == null)
            {
                a_context.Broadcaster.Error("A list manager is required.");
            }
            else
            {
                a_context.Lists.GetPreviousItem(mTargetValue);
            }
            return TSFlow.Next;
        }
    }
}
