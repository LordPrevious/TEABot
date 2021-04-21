using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.TEAScript
{
    /// <summary>
    /// Provides some constant values required throughout.
    /// </summary>
    public static class TSConstants
    {
        /// <summary>
        /// Prefix for context value names
        /// </summary>
        public const char ContextPrefix = '$';

        /// <summary>
        /// Prefix for argument value names
        /// </summary>
        public const char ArgumentPrefix = '?';

        /// <summary>
        /// Prefix for variable value names
        /// </summary>
        public const char VariablePrefix = '!';

        /// <summary>
        /// Prefix for jump label names
        /// </summary>
        public const char JumpLabelPrefix = '@';

        /// <summary>
        /// All known value prefixes
        /// </summary>
        public static readonly char[] ValuePrefixes = new char[]
        {
            ContextPrefix,
            ArgumentPrefix,
            VariablePrefix
        };
    }
}
