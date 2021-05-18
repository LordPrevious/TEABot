using System;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// Similar to flushing, but stores the output in a variable rather than sending it as a message.
    /// Can be used to construct complex strings.
    /// </summary>
    [TSKeyword("stash")]
    public class TSISStash : TSIStatement
    {
        /// <summary>
        /// Name of variable to store the result in
        /// </summary>
        private string mTargetName = String.Empty;

        protected override bool Parse(string a_instructionArguments)
        {
            return (SingleValueArgument(a_instructionArguments, out ITSValueArgument tmpTargetValue)
                && VariableNameValueArgument(tmpTargetValue, out mTargetName));
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            a_context.Values[mTargetName] = a_context.Output.ToString();
            a_context.Output.Clear();
            return TSFlow.Next;
        }
    }
}
