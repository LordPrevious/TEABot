using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.TEAScript
{
    /// <summary>
    /// Helps parsing the parameter arguments for a script execution
    /// </summary>
    public class TSScriptArgumentParser
    {
        /// <summary>
        /// Initialize an argument parser
        /// </summary>
        /// <param name="a_script">The script for which to parse arguments</param>
        /// <param name="a_arguments">The script invocation arguments</param>
        public TSScriptArgumentParser(TSCompiledScript a_script, string a_arguments)
        {
            mScript = a_script ?? throw new ArgumentNullException(nameof(a_script));
            mArguments = a_arguments ?? String.Empty;
        }

        /// <summary>
        /// Parse the arguments and store the values in the given execution context
        /// </summary>
        /// <param name="a_context">The context in which to set the parameter argument values</param>
        /// <returns>True iff the arguments were parsed successfully</returns>
        public bool ParseIntoContext(TSExecutionContext a_context)
        {
            if (a_context == null)
            {
                return false;
            }
            // reset internal state
            Reset();
            // iterate over parameters and try to get arguments for each
            foreach (var parameter in mScript.Parameters)
            {
                // get argument string
                string argument;
                if (parameter.Type == TSEParameterType.TAIL)
                {
                    argument = GetArgumentTail();
                }
                else
                {
                    argument = GetNextArgument();
                }
                // check if argument was given
                if (String.IsNullOrEmpty(argument))
                {
                    if (parameter.Required)
                    {
                        // missing required parameter, abort
                        a_context.Broadcaster.Error("Missing argument for required parameter {0}",
                            parameter.Name);
                        return false;
                    }
                    // else use default value for optional parameters
                    argument = parameter.DefaultValue;
                }
                // get prefixed parameter name
                var prefixedName = TSConstants.ArgumentPrefix + parameter.Name;
                // check parameter type
                switch (parameter.Type)
                {
                    case TSEParameterType.STRING:
                    case TSEParameterType.TAIL:
                        // set string value
                        a_context.Values[prefixedName] = new TSValue(argument);
                        break;
                    case TSEParameterType.NUMBER:
                        // convert to number
                        {
                            if (!Int64.TryParse(argument, out long numberValue))
                            {
                                a_context.Broadcaster.Error("Parameter {0} of NUMBER type has invalid argument \"{1}\"",
                                    parameter.Name,
                                    argument);
                                return false;
                            }
                            // set number value
                            a_context.Values[prefixedName] = new TSValue(numberValue);
                        }
                        break;
                    case TSEParameterType.USER:
                        // TODO check against known users and adjust capitalization?
                        a_context.Values[prefixedName] = new TSValue(argument);
                        break;
                    case TSEParameterType.UNKNOWN:
                    default:
                        // that's not allowed!
                        a_context.Broadcaster.Error("Parameter {0} has invalid type",
                            parameter.Name);
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// The script for which to parse arguments
        /// </summary>
        private readonly TSCompiledScript mScript;

        /// <summary>
        /// The script arguments string
        /// </summary>
        private readonly string mArguments;

        /// <summary>
        /// Current parsing position / start of next argument in mArguments
        /// </summary>
        private int mPosition;

        /// <summary>
        /// Separator character for a script's argument list
        /// </summary>
        private static readonly char ARGUMENT_SEPARATOR = ' ';

        /// <summary>
        /// Reset internal parsing state
        /// </summary>
        private void Reset()
        {
            mPosition = 0;
        }

        /// <summary>
        /// Get the next argument from the original arguments string
        /// </summary>
        /// <returns>The next argument, or empty string of no more arguments are present</returns>
        private string GetNextArgument()
        {
            // skip repeated separator characters
            while ((mPosition < mArguments.Length)
                && (mArguments[mPosition] == ARGUMENT_SEPARATOR))
            {
                ++mPosition;
            }
            // check if we're still within the string
            if (mPosition >= mArguments.Length)
            {
                return String.Empty;
            }
            // get position of next separator
            var nextPosition = mArguments.IndexOf(ARGUMENT_SEPARATOR, mPosition);
            if (nextPosition < 0)
            {
                // take rest of string
                nextPosition = mArguments.Length;
            }
            // check if there's an argument left
            if ((nextPosition - mPosition) <= 0)
            {
                mPosition = mArguments.Length;
                return String.Empty;
            }
            // get substring
            var result = mArguments[mPosition..nextPosition];
            // update position
            mPosition = nextPosition;
            // return substrung
            return result;
        }


        /// <summary>
        /// Get the remainder of the original arguments string
        /// </summary>
        /// <returns>The argument tail</returns>
        private string GetArgumentTail()
        {
            var result = mArguments[mPosition..];
            mPosition = mArguments.Length;
            return result.Trim();
        }
    }
}
