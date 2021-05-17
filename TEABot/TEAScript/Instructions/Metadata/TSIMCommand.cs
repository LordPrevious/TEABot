using System.Collections.Generic;
using System.Linq;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Metadata
{
    /// <summary>
    /// Sets the script trigger command
    /// </summary>
    [TSKeyword("command")]
    public class TSIMCommand : TSIScriptMetadata
    {
        private List<string> mCommands = new();

        public override void Apply(TSCompiledScript a_targetScript)
        {
            a_targetScript.Commands = mCommands;
        }

        protected override bool Parse(string a_instructionArguments)
        {
            var validator = new TSValidator(ParsingBroadcaster);
            mCommands = SplitWordsArguments(a_instructionArguments).Select(
                c => c.ToLowerInvariant()).ToList();
            validator.ContainsEnoughArguments(mCommands, 1, true);
            mCommands.ForEach(c => validator.IsTriggerCommand(c));
            return validator;
        }
    }
}
