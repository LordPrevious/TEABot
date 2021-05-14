using System;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements.Storage
{
    /// <summary>
    /// Remove the data for a given key from the storage object.
    /// </summary>
    [TSKeyword("storage:remove")]
    public class TSISStorageRemove : TSIStatement
    {
        /// <summary>
        /// Key of the storage value
        /// </summary>
        private ITSValueArgument mKeyName = new TSConstantStringArgument();

        protected override bool Parse(string a_instructionArguments)
        {
            if (SingleValueArgument(a_instructionArguments, out mKeyName))
            {
                return true;
            }
            return false;
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            a_context.Storage?.Remove(a_context, mKeyName.GetValue(a_context.Values));
            return TSFlow.Next;
        }
    }
}
