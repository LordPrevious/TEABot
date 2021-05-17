using System;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// Hurl instruction to forward a value - or wildcarded set of values - via a second
    /// communication channel, e.g. a websocket connection.
    /// </summary>
    [TSKeyword("hurl")]
    public class TSISHurl : TSIStatement
    {
        /// <summary>
        /// Value to hurl
        /// </summary>
        private ITSValueArgument mSourceValue = new TSConstantNumberArgument(0L);

        protected override bool Parse(string a_instructionArguments)
        {
            var words = SplitWordsArguments(a_instructionArguments);
            return ((new TSValidator(ParsingBroadcaster).ContainsEnoughArguments(words, 1))
                && SingleValueArgument(words[0], out mSourceValue, true));
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            a_context.Hurl(mSourceValue);
            return TSFlow.Next;
        }
    }
}
