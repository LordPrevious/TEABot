using System;
using System.Linq;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// Conditional jump to a label when a value is not zero
    /// </summary>
    [TSKeyword("jump")]
    public class TSISJump : TSIStatement
    {
        /// <summary>
        /// Name of the jump target label
        /// </summary>
        private string mLabelName = String.Empty;

        /// <summary>
        /// The value to check
        /// </summary>
        private ITSValueArgument mSourceValue = new TSConstantValueArgument(0L);

        protected override bool Parse(string a_instructionArguments)
        {
            var validator = new TSValidator(ParsingBroadcaster);
            var words = SplitWordsArguments(a_instructionArguments);
            if (!validator.ContainsEnoughArguments(words, 2)) return false;
            mLabelName = words[0];
            return validator.IsJumpLabelName(mLabelName)
                && SingleValueArgument(words[1], out mSourceValue);
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            long currentValue = mSourceValue.GetValue(a_context);
            if (currentValue != 0)
            {
                return TSFlow.JumpToLabel(mLabelName);
            }
            return TSFlow.Next;
        }
    }
}
