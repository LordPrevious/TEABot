using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// A statement of a TEAScript, being an executable instruction
    /// </summary>
    public abstract class TSIStatement : TSInstruction
    {
        /// <summary>
        /// Execute the statement.
        /// Implementations should use the context's broadcaster for messaging within,
        /// rather than the instances own.
        /// </summary>
        /// <param name="a_context">Mutable context/state information</param>
        /// <returns>How to proceed after the statement.</returns>
        public abstract ITSControlFlow Execute(TSExecutionContext a_context);
    }
}
