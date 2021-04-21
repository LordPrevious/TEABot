using System;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// Write instruction to add statically defined text to the output buffer
    /// </summary>
    [TSKeyword("write")]
    public class TSISWrite : TSIStatement
    {
        /// <summary>
        /// The text to add to the output buffer
        /// </summary>
        private string mText = String.Empty;

        protected override bool Parse(string a_instructionArguments)
        {
            mText = a_instructionArguments;
            return true;
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            a_context.Output.Append(mText);
            return TSFlow.Next;
        }
    }
}
