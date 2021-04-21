using System;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// Jump to a randomly selected label
    /// </summary>
    [TSKeyword("leap")]
    public class TSISLeap : TSIStatement
    {
        /// <summary>
        /// A randomizer shared across all instances
        /// </summary>
        private static readonly Random sRandomizer = new Random();

        /// <summary>
        /// Names of the jump target labels
        /// </summary>
        private string[] mLabelNames = new string[0];

        protected override bool Parse(string a_instructionArguments)
        {
            var validator = new TSValidator(ParsingBroadcaster);
            mLabelNames = SplitWordsArguments(a_instructionArguments);
            if (!validator.ContainsEnoughArguments(mLabelNames, 2, true)) return false;
            foreach (var labelName in mLabelNames)
            {
                if (!validator.IsJumpLabelName(labelName)) return false;
            }
            return true;
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            var index = sRandomizer.Next(0, mLabelNames.Length);
            return TSFlow.JumpToLabel(mLabelNames[index]);
        }
    }
}
