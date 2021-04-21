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
        /// Get the numerical value
        /// </summary>
        /// <param name="a_context">The script execution contect</param>
        /// <returns>The numerical value</returns>
        long GetValue(TSExecutionContext a_context);
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
        /// Initialize a named value argument
        /// </summary>
        /// <param name="a_valueName">Name of the referenced value</param>
        public TSNamedValueArgument(string a_valueName)

        {
            ValueName = a_valueName;
        }

        public long GetValue(TSExecutionContext a_context)
        {
            return a_context.Values[ValueName];
        }
    }

    /// <summary>
    /// Constant value is provided directly from instruction line
    /// </summary>
    public readonly struct TSConstantValueArgument : ITSValueArgument
    {
        public long ConstantValue { get; }

        /// <summary>
        /// Initialize a constant value argument
        /// </summary>
        /// <param name="a_constantValue">The constant value</param>
        public TSConstantValueArgument(long a_constantValue)

        {
            ConstantValue = a_constantValue;
        }

        public long GetValue(TSExecutionContext a_context)
        {
            return ConstantValue;
        }
    }
}
