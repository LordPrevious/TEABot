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

            if (!SplitValueArguments(a_instructionArguments, out ITSValueArgument[] valueArguments)
                || !validator.ContainsEnoughArguments(valueArguments, 2)
                || !VariableNameValueArgument(valueArguments[0], out mTargetName))
            {
                return false;
            }
            mKeyName = valueArguments[1];
            return validator;
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            if ((a_context.Storage != null)
                && a_context.Storage.HoldsLock(a_context)
                && a_context.Storage.Has(a_context, mKeyName.GetValue(a_context)))
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
