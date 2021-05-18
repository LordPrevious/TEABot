using System;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// Set the $timestamp context value to the current time using the provided formatting string
    /// </summary>
    [TSKeyword("timestamp")]
    public class TSITimestamp: TSIStatement
    {
        /// <summary>
        /// The timestamp format
        /// </summary>
        private string mFormat = String.Empty;

        protected override bool Parse(string a_instructionArguments)
        {
            mFormat = a_instructionArguments;
            return true;
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            a_context.Values["$timestamp"] = DateTime.Now.ToString(mFormat);
            return TSFlow.Next;
        }
    }
}
