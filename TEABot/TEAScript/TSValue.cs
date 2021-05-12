using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.TEAScript
{
    /// <summary>
    /// Represents a value during script execution,
    /// e.g. a script argument, variable, or metadata
    /// </summary>
    public readonly struct TSValue
    {
        /// <summary>
        /// The value as a string
        /// </summary>
        public string TextValue { get; }

        /// <summary>
        /// The value as a number, falling back to 0 if the string value cannot be parsed as a 64-bit signed integer
        /// </summary>
        public long NumericalValue { get; }

        /// <summary>
        /// Empty value to use as default in automatic generation of dictionary entries
        /// </summary>
        public static readonly TSValue Empty = new(String.Empty, 0L);

        /// <summary>
        /// Explicity set each field.
        /// </summary>
        /// <param name="a_textValue">The string value</param>
        /// <param name="a_numericalValue">The numerical value</param>
        private TSValue(string a_textValue, long a_numericalValue)
        {
            TextValue = a_textValue;
            NumericalValue = a_numericalValue;
        }

        /// <summary>
        /// Create a new value for a string.
        /// </summary>
        /// <param name="a_textValue">The string value</param>
        public TSValue(string a_textValue)
        {
            TextValue = a_textValue;
            if (Int64.TryParse(a_textValue, out long numerical))
            {
                NumericalValue = numerical;
            }
            else
            {
                NumericalValue = 0L;
            }
        }

        /// <summary>
        /// Create a new value for a number.
        /// </summary>
        /// <param name="a_numericalValue">The numerical value</param>
        public TSValue(long a_numericalValue)
        {
            NumericalValue = a_numericalValue;
            TextValue = a_numericalValue.ToString();
        }

        /// <summary>
        /// Convert to long.
        /// </summary>
        /// <param name="a_value">The value to convert</param>
        public static implicit operator long(TSValue a_value)
        {
            return a_value.NumericalValue;
        }

        /// <summary>
        /// Convert to string.
        /// </summary>
        /// <param name="a_value">The value to convert</param>
        public static implicit operator string(TSValue a_value)
        {
            return a_value.TextValue;
        }

        /// <summary>
        /// Convert from long.
        /// </summary>
        /// <param name="a_value">The value to convert</param>
        public static implicit operator TSValue(long a_value)
        {
            return new TSValue(a_value);
        }

        /// <summary>
        /// Convert from string.
        /// </summary>
        /// <param name="a_value">The value to convert</param>
        public static implicit operator TSValue(string a_value)
        {
            return new TSValue(a_value);
        }
    }
}
