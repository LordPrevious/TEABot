using System;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements.Storage
{
    /// <summary>
    /// Get a value from the open storage object
    /// </summary>
    [TSKeyword("storage:get")]
    public class TSISStorageGet : TSIStatement
    {
        /// <summary>
        /// Key of the storage value
        /// </summary>
        private ITSValueArgument mKeyName = new TSConstantStringArgument();

        /// <summary>
        /// Name of the variable to store the value in, may use wildcards for complex objects
        /// </summary>
        private TSNamedValueArgument mTargetValue = new();

        protected override bool Parse(string a_instructionArguments)
        {
            var validator = new TSValidator(ParsingBroadcaster);


            var words = SplitWordsArguments(a_instructionArguments);
            if (validator.ContainsEnoughArguments(words, 2)
                && SingleValueArgument(words[0], out ITSValueArgument tmpTargetValue, true)
                && (tmpTargetValue is TSNamedValueArgument namedTargetValue)
                && SingleValueArgument(words[1], out mKeyName))
            {
                mTargetValue = namedTargetValue;
                return true;
            }
            return false;
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            a_context.Storage?.Get(a_context, mKeyName.GetValue(a_context), mTargetValue, a_context.Values);
            return TSFlow.Next;
        }
    }
}
