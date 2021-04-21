using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.TEAScript.Attributes
{
    /// <summary>
    /// Specifies the identification string of the instruction, that is, the first word in the line
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TSKeywordAttribute : System.Attribute
    {
        /// <summary>
        /// The first word in an instruction line that specifies the corresponding instruction type
        /// </summary>
        public string Identifier { get; private set; }

        /// <summary>
        /// Create a new instruction attribute
        /// </summary>
        /// <param name="a_identifier">The first word in an instruction line that specifies the corresponding instruction type</param>
        public TSKeywordAttribute(string a_identifier)
        {
            Identifier = a_identifier;
        }
    }
}
