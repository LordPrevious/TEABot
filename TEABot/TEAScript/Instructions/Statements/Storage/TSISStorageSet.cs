using System;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements.Storage
{
    /// <summary>
    /// Set a value in the open storage object
    /// </summary>
    [TSKeyword("storage:set")]
    public class TSISStorageSet : TSIStatement
    {
        /// <summary>
        /// Key of the storage value
        /// </summary>
        private ITSValueArgument mKeyName = new TSConstantStringArgument();

        /// <summary>
        /// Value to store
        /// </summary>
        private ITSValueArgument mSourceValue = new TSConstantNumberArgument(0L);

        protected override bool Parse(string a_instructionArguments)
        {
            var validator = new TSValidator(ParsingBroadcaster);

            var words = SplitWordsArguments(a_instructionArguments);
            return (validator.ContainsEnoughArguments(words, 2)
                && SingleValueArgument(words[0], out mKeyName)
                && SingleValueArgument(words[1], out mSourceValue, true));
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            a_context.Storage?.Set(a_context, mKeyName.GetValue(a_context), mSourceValue, a_context.Values);
            return TSFlow.Next;
        }
    }
}
