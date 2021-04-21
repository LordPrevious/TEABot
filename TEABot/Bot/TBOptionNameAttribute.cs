using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.Bot
{
    /// <summary>
    /// Specifies the name string of the option
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TBOptionNameAttribute : Attribute
    {
        /// <summary>
        /// The option name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Create a new option name attribute
        /// </summary>
        /// <param name="a_name">The option name</param>
        public TBOptionNameAttribute(string a_name)
        {
            Name = a_name;
        }
    }
}
