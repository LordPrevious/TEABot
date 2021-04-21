using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// Logical OR operation on values, storing the result in a variable.
    /// 0 is false, everything else is true
    /// </summary>
    [TSKeyword("or")]
    public class TSISOr : TSISMath
    {
        protected override long Operator(long a_operandA, long a_operandB)
        {
            if ((a_operandA != 0) || (a_operandB != 0))
            {
                return 1;
            }
            return 0;
        }
    }
}
