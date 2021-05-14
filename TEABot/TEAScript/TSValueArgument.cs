using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.TEAScript
{
    /// <summary>
    /// Interface for value arguments in script statements.
    /// A value argument may be a named value or a constant number.
    /// </summary>
    public interface ITSValueArgument
    {
        /// <summary>
        /// Get the actual value
        /// </summary>
        /// <param name="a_values">Value dictionary for dependencies and dynamic values</param>
        /// <returns>The numerical value</returns>
        TSValue GetValue(TSValueDictionary a_values);
    }

    /// <summary>
    /// Value is fetched via name from execution context.
    /// </summary>
    public readonly struct TSNamedValueArgument : ITSValueArgument
    {
        /// <summary>
        /// Name of the referenced value
        /// </summary>
        public string ValueName { get; }

        /// <summary>
        /// True if the value name may be used for wildcard access to all values with a common prefix
        /// </summary>
        public bool HasWildcard { get; }

        /// <summary>
        /// Initialize a named value argument
        /// </summary>
        /// <param name="a_valueName">Name of the referenced value</param>
        public TSNamedValueArgument(string a_valueName)

        {
            HasWildcard = a_valueName.EndsWith(TSConstants.WildcardPostfix);
            if (HasWildcard)
            {
                ValueName = a_valueName[..^1];
            }
            else
            {
                ValueName = a_valueName;
            }
        }

        public TSValue GetValue(TSValueDictionary a_values)
        {
            return a_values[ValueName];
        }
    }

    /// <summary>
    /// Constant value is provided directly from instruction line
    /// </summary>
    public readonly struct TSConstantNumberArgument : ITSValueArgument
    {
        public long ConstantValue { get; }

        /// <summary>
        /// Initialize a constant number value argument
        /// </summary>
        /// <param name="a_constantValue">The constant value</param>
        public TSConstantNumberArgument(long a_constantValue)

        {
            ConstantValue = a_constantValue;
        }

        public TSValue GetValue(TSValueDictionary a_values)
        {
            return ConstantValue;
        }
    }

    /// <summary>
    /// Constant value is provided directly from instruction line
    /// </summary>
    public readonly struct TSConstantStringArgument : ITSValueArgument
    {
        public string ConstantValue { get; }

        /// <summary>
        /// Initialize a constant string value argument
        /// </summary>
        /// <param name="a_constantValue">The constant value</param>
        public TSConstantStringArgument(string a_constantValue)

        {
            ConstantValue = a_constantValue;
        }

        public TSValue GetValue(TSValueDictionary a_values)
        {
            return ConstantValue;
        }
    }
}
