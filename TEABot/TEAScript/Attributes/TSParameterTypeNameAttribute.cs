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
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class TSParameterTypeNameAttribute : System.Attribute
    {
        /// <summary>
        /// The first word in an instruction line that specifies the corresponding instruction type
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        /// Create a new parameter type name attribute
        /// </summary>
        /// <param name="a_typeName">The string value corresponding to the enum value in a script</param>
        public TSParameterTypeNameAttribute(string a_typeName)
        {
            TypeName = a_typeName;
        }
    }
}
