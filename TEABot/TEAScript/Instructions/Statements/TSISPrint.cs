using System;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// Print instruction to add a dynamic value to the output buffer
    /// </summary>
    [TSKeyword("print")]
    public class TSISPrint : TSIStatement
    {
        /// <summary>
        /// Name of the value to add to the output buffer, including prefix
        /// </summary>
        private string mValueName = String.Empty;

        protected override bool Parse(string a_instructionArguments)
        {
            mValueName = SingleWordArgument(a_instructionArguments);
            return new TSValidator(ParsingBroadcaster).IsValueName(mValueName);
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            a_context.Output.Append(a_context.Values[mValueName].TextValue);
            return TSFlow.Next;
        }
    }
}
