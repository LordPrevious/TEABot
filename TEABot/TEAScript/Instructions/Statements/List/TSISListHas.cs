using System;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements.List
{
    /// <summary>
    /// Check if the given index is within bounds [0, $list.lastIndex]
    /// </summary>
    [TSKeyword("list:has")]
    public class TSISListHas : TSIStatement
    {
        /// <summary>
        /// Value with the index to check for
        /// </summary>
        private ITSValueArgument mIndexValue = new TSConstantNumberArgument();

        /// <summary>
        /// Name of the variable to store the result value in
        /// </summary>
        private string mTargetName = String.Empty;

        protected override bool Parse(string a_instructionArguments)
        {
            var validator = new TSValidator(ParsingBroadcaster);

            var words = SplitWordsArguments(a_instructionArguments);
            if (validator.ContainsEnoughArguments(words, 2)
                && validator.IsVariableName(words[0])
                && SingleValueArgument(words[1], out mIndexValue))
            {
                mTargetName = words[0];
                return true;
            }
            return false;
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            if ((a_context.Lists != null)
                && a_context.Lists.HasListItem(mIndexValue.GetValue(a_context.Values)))
            {
                a_context.Values[mTargetName] = 1L;
            }
            else
            {
                a_context.Values[mTargetName] = 0L;
            }
            return TSFlow.Next;
        }
    }
}
