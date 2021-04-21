using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// Subtract values and store the result in a variable.
    /// </summary>
    [TSKeyword("subtract")]
    public class TSISSubtract : TSISMath
    {
        protected override long Operator(long a_operandA, long a_operandB)
        {
            return a_operandA - a_operandB;
        }
    }
}
