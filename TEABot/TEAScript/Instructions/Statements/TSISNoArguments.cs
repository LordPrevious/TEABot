using System;
using System.Linq;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// Base for commands which take no arguments
    /// </summary>
    public abstract class TSISNoArguments : TSIStatement
    {
        protected override bool Parse(string a_instructionArguments)
        {
            if (a_instructionArguments.Length != 0)
            {
                ParsingBroadcaster?.Warn("Instruction takes no arguments, \"{0}\" will be ignored.", a_instructionArguments);
            }
            return true;
        }
    }
}
