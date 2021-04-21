using System;
using System.Linq;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript
{
    /// <summary>
    /// Expected type of a parameter's value
    /// </summary>
    public enum TSEParameterType
    {
        /// <summary>
        /// No specified type / invalid specification
        /// </summary>
        [TSParameterTypeName("unknown")]
        UNKNOWN,
        /// <summary>
        /// String with arbitrary text content
        /// </summary>
        [TSParameterTypeName("string")]
        STRING,
        /// <summary>
        /// Numerical value (64bit signed integer)
        /// </summary>
        [TSParameterTypeName("number")]
        NUMBER,
        /// <summary>
        /// Username
        /// </summary>
        [TSParameterTypeName("user")]
        USER,
        /// <summary>
        /// Whatever is left of the input message as a string.
        /// Can only be the last parameter specification of a script
        /// </summary>
        [TSParameterTypeName("tail")]
        TAIL
    }

    public static class TSEParameterTypeExtensions
    {
        public static string GetTypeName(this TSEParameterType a_type)
        {
            var enumType = typeof(TSEParameterType);
            var memberInfos = enumType.GetMember(a_type.ToString());
            if (memberInfos.Length == 0)
            {
                return String.Empty;
            }
            var valueInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
            var valueAttributes = valueInfo.GetCustomAttributes(typeof(TSParameterTypeNameAttribute), false);
            if (valueAttributes.Length == 0)
            {
                return String.Empty;
            }
            return ((TSParameterTypeNameAttribute)valueAttributes[0]).TypeName;
        }

        public static TSEParameterType FromString(string a_typeString)
        {
            var parameterTypes = Enum.GetValues(typeof(TSEParameterType)).Cast<TSEParameterType>();
            return parameterTypes.FirstOrDefault(t => t.GetTypeName().Equals(a_typeString, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
