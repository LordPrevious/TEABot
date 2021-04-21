using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.TEAScript.Instructions.Metadata
{
    /// <summary>
    /// Script metadata instructions do not perform anything upon execution, but can be applied to
    /// a compiled script to set its various properties, such as name, description or parameter
    /// specifications.
    /// </summary>
    public abstract class TSIScriptMetadata : TSInstruction
    {
        /// <summary>
        /// Apply the metatdata instruction to the given script.
        /// </summary>
        /// <param name="a_targetScript">The script to update with the contained metadata</param>
        public abstract void Apply(TSCompiledScript a_targetScript);
    }
}
