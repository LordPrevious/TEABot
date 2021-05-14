using System;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements.Storage
{
    /// <summary>
    /// Check if the open storage object contains a value for the given key
    /// </summary>
    [TSKeyword("storage:has")]
    public class TSISStorageHas : TSIStatement
    {
        /// <summary>
        /// Key of the storage value
        /// </summary>
        private ITSValueArgument mKeyName = new TSConstantStringArgument();

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
                && SingleValueArgument(words[1], out mKeyName))
            {
                mTargetName = words[0];
                return true;
            }
            return false;
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            if ((a_context.Storage != null)
                && a_context.Storage.HoldsLock(a_context)
                && a_context.Storage.Has(a_context, mKeyName.GetValue(a_context.Values)))
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
