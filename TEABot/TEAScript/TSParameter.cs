using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.TEAScript
{
    /// <summary>
    /// Script parameter specification
    /// </summary>
    public class TSParameter
    {
        /// <summary>
        /// Parameter name with which it will be referenced in the script
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Whether the parameter is required or optional
        /// </summary>
        public bool Required { get; private set; }

        /// <summary>
        /// Description of the parameter for dynamic documentation
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Expected type of the value for pre-execution validity checking
        /// </summary>
        public TSEParameterType Type { get; set; }

        /// <summary>
        /// The default value of this parameter if it is optional
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Initialize a new parameter specification
        /// </summary>
        /// <param name="a_name">The parameter name</param>
        /// <param name="a_required">Whether the parameter is required or optional</param>
        public TSParameter(String a_name, bool a_required)
        {
            Name = a_name;
            Required = a_required;
            Description = String.Empty;
            Type = TSEParameterType.UNKNOWN;
            DefaultValue = String.Empty;
        }
    }
}
