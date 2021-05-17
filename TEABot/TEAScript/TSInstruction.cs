using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.TEAScript
{
    /// <summary>
    /// An instruction of a TEAScript, representing a single script line
    /// </summary>
    public abstract class TSInstruction
    {

        /// <summary>
        /// Parse an instruction with matching identifier from an instruction line
        /// </summary>
        /// <param name="a_broadcaster">Broadcaster to send parsing messages to</param>
        /// <param name="a_instructionArguments">The rest of the instruction line past the identifier</param>
        /// <returns>True on successful parsing, false on severe error.</returns>
        public bool Parse(TSBroadcaster a_broadcaster, string a_instructionArguments)
        {
            ParsingBroadcaster = a_broadcaster;
            var result = Parse(a_instructionArguments);
            ParsingBroadcaster = null;
            return result;
        }

        /// <summary>
        /// A broadcaster to send parsing messages to
        /// </summary>
        protected TSBroadcaster ParsingBroadcaster { get; private set; }

        /// <summary>
        /// Parse an instruction with matching identifier from an instruction line
        /// </summary>
        /// <param name="a_instructionArguments">The rest of the instruction line past the identifier</param>
        /// <returns>True on successful parsing, false on severe error.</returns>
        protected abstract bool Parse(string a_instructionArguments);

        #region Parsing helpers

        /// <summary>
        /// Get the instruction arguments without leading or trailing whitespace
        /// </summary>
        /// <param name="a_instructionArguments">Original instruction arguments</param>
        /// <returns>The trimmed arguments.</returns>
        protected string TrimmedInstructionArguments(string a_instructionArguments)
        {
            string trimmedArguments = a_instructionArguments.Trim();
            if (trimmedArguments.Length == 0)
            {
                ParsingBroadcaster?.Warn("Trimmed instruction arguments are empty.");
            }
            return trimmedArguments;
        }

        /// <summary>
        /// Get the single first word from the arguments string
        /// </summary>
        /// <param name="a_instructionArguments">Original instruction arguments</param>
        /// <returns>The first word of the arguments.</returns>
        protected string SingleWordArgument(string a_instructionArguments)
        {
            var trimmedArguments = TrimmedInstructionArguments(a_instructionArguments);
            var spaceIndex = trimmedArguments.IndexOf(' ');
            if (spaceIndex < 0)
            {
                return trimmedArguments;
            }
            return trimmedArguments.Substring(0, spaceIndex);
        }

        /// <summary>
        /// Split the arguments string into separate non-empty words
        /// </summary>
        /// <param name="a_instructionArguments">Original instruction arguments</param>
        /// <returns>The separated words of the arguments string.</returns>
        protected static string[] SplitWordsArguments(string a_instructionArguments)
        {
            return a_instructionArguments.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Get the single first word from the arguments string as a 64-bit signed integer
        /// </summary>
        /// <param name="a_instructionArguments">Original instruction arguments</param>
        /// <param name="a_number">The first number of the arguments</param>
        /// <returns>True on successful parsing.</returns>
        protected bool SingleNumberArgument(string a_instructionArguments, out long a_number)
        {
            var firstWord = SingleWordArgument(a_instructionArguments);
            if (Int64.TryParse(firstWord, out a_number)) return true;
            ParsingBroadcaster?.Error("Cannot parse \"{0}\" as a number.", firstWord);
            a_number = 0L;
            return false;
        }

        /// <summary>
        /// Get the words from the arguments string as 64-bit signed integers
        /// </summary>
        /// <param name="a_instructionArguments">Original instruction arguments</param>
        /// <param name="a_numbers">The number arguments</param>
        /// <returns>True on successful parsing</returns>
        protected bool SplitNumberArguments(string a_instructionArguments, out long[] a_numbers)
        {
            var words = SplitWordsArguments(a_instructionArguments);
            bool result = true;
            a_numbers = words.Select(w => {
                if (!Int64.TryParse(w, out long number))
                {
                    ParsingBroadcaster?.Error("Cannot parse \"{0}\" as a number.", w);
                    number = 0L;
                    result = false;
                }
                return number;
            }).ToArray();
            return result;
        }


        /// <summary>
        /// Get the single first word from the arguments string as a value argument
        /// </summary>
        /// <param name="a_instructionArguments">Original instruction arguments</param>
        /// <param name="a_valueArgument">The first value of the arguments</param>
        /// <param name="a_allowWildcard">True to allow wildcards with named value arguments</param>
        /// <returns>True on successful parsing.</returns>
        protected bool SingleValueArgument(string a_instructionArguments, out ITSValueArgument a_valueArgument, bool a_allowWildcard = false)
        {
            var firstWord = SingleWordArgument(a_instructionArguments);

            if (TSConstants.ValuePrefixes.Any(p => p == firstWord[0]))
            {
                // got a value reference
                if (new TSValidator(ParsingBroadcaster).IsValueName(firstWord, a_allowWildcard))
                {
                    a_valueArgument = new TSNamedValueArgument(firstWord);
                    return true;
                }
            }
            else if (firstWord.StartsWith(TSConstants.StringConstantPrefix))
            {
                // single-word string constant, drop prefix
                a_valueArgument = new TSConstantStringArgument(firstWord[1..]);
                return true;
            }
            // must be a number otherwise
            else if (SingleNumberArgument(firstWord, out long number))
            {
                a_valueArgument = new TSConstantNumberArgument(number);
                return true;
            }
            // error: fallback to 0 and mark parse failure
            a_valueArgument = new TSConstantNumberArgument(0L);
            return false;
        }

        /// <summary>
        /// Get the words from the arguments string as value arguments
        /// </summary>
        /// <param name="a_instructionArguments">Original instruction arguments</param>
        /// <param name="a_valueArgument">The value arguments</param>
        /// <returns>True on successful parsing</returns>
        protected bool SplitValueArguments(string a_instructionArguments, out ITSValueArgument[] a_valueArguments)
        {
            var words = SplitWordsArguments(a_instructionArguments);
            bool result = true;
            a_valueArguments = words.Select<string, ITSValueArgument>(w => {
                if (TSConstants.ValuePrefixes.Any(p => p == w[0]))
                {
                    // got a value reference
                    if (new TSValidator(ParsingBroadcaster).IsValueName(w))
                    {
                        return new TSNamedValueArgument(w);
                    }
                }
                else if (w.StartsWith(TSConstants.StringConstantPrefix))
                {
                    // single-word string constant, drop prefix
                    return new TSConstantStringArgument(w[1..]);
                }
                // must be a number otherwise
                else if (SingleNumberArgument(w, out long number))
                {
                    return new TSConstantNumberArgument(number);
                }
                // error: fallback to 0 and mark parse failure
                result = false;
                return new TSConstantNumberArgument(0L);
            }).ToArray();
            return result;
        }

        /// <summary>
        /// Gets the value argument as a variable name
        /// </summary>
        /// <param name="a_valueArgument">The value argument from which to extract a variable name</param>
        /// <param name="a_variableName">The extracted variable name</param>
        /// <returns>True iff the value argument references a variable name</returns>
        protected bool VariableNameValueArgument(ITSValueArgument a_valueArgument, out string a_variableName)
        {
            var VariableNameArgument = a_valueArgument as TSNamedValueArgument?;
            if ((null == VariableNameArgument)
                || (VariableNameArgument.Value.ValueName.Length == 0)
                || (VariableNameArgument.Value.ValueName[0] != TSConstants.VariablePrefix))
            {
                ParsingBroadcaster?.Error("Given value is not a variable name.");
                a_variableName = String.Empty;
                return false;
            }
            a_variableName = VariableNameArgument.Value.ValueName;
            return true;
        }

        #endregion
    }
}
