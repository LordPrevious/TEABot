using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TEABot.TEAScript
{
    /// <summary>
    /// Validation helper to check various value properties.
    /// </summary>
    public class TSValidator
    {
        /// <summary>
        /// True iff the validation(s) determined that whatever was checked was valid
        /// </summary>
        public bool IsValid { get; private set; } = true;

        /// <summary>
        /// Broadcaster to send validation messages to
        /// </summary>
        private readonly TSBroadcaster mBroadcaster = null;

        /// <summary>
        /// Initialize a new validator
        /// </summary>
        /// <param name="a_broadcaster">A broadcaster to send validation messages to</param>
        public TSValidator(TSBroadcaster a_broadcaster)
        {
            mBroadcaster = a_broadcaster;
        }

        /// <summary>
        /// Convert to bool
        /// </summary>
        /// <param name="a_result">The validation result to convert</param>
        public static implicit operator bool(TSValidator a_result)
        {
            return a_result.IsValid;
        }

        /// <summary>
        /// Check if the given word is a valid value name without scope prefix,
        /// containing only allowed characters
        /// </summary>
        /// <param name="a_word">The word to check</param>
        /// <returns>The validation result</returns>
        public TSValidator IsUnprefixedValueName(string a_word)
        {
            if (a_word.Length == 0)
            {
                mBroadcaster?.Error("A value name must not be empty.");
            }

            if (a_word.All(IsAllowedValueNameCharacter))
            {
                return this;
            }

            mBroadcaster?.Error("The value name \"{0}\" contains invalid characters.", a_word);
            IsValid = false;
            return this;
        }

        /// <summary>
        /// Checks if a character is allowed as part of an unprefixed value name
        /// </summary>
        /// <param name="a_char">The character to check</param>
        /// <returns>True iff the character is allowed</returns>
        private static bool IsAllowedValueNameCharacter(Char a_char)
        {
            return (Char.IsLetterOrDigit(a_char)
                || (a_char == TSConstants.VariableGroupIndicator));
        }

        /// <summary>
        /// Check if the given word is a valid value name,
        /// starting with a scope prefix (see TSConstants.ValuePrefixes)
        /// and containing only allowed characters
        /// </summary>
        /// <param name="a_word">The word to check</param>
        /// <param name="a_allowWildcard">True to allow wildcard postfix</param>
        /// <returns>The validation result</returns>
        public TSValidator IsValueName(string a_word, bool a_allowWildcard = false)
        {
            if (a_word.Length < 1)
            {
                mBroadcaster?.Error("A value name must not be empty and must start with a prefix.");
                IsValid = false;
                return this;
            }
            if (!TSConstants.ValuePrefixes.Any(p => p == a_word[0]))
            {
                mBroadcaster?.Error("The value name \"{0}\" has an invalid prefix '{1}'", a_word, a_word[0]);
                IsValid = false;
                return this;
            }
            if (a_allowWildcard && a_word.EndsWith(TSConstants.WildcardPostfix))
            {
                return IsUnprefixedValueName(a_word[1..^1]);
            }
            return IsUnprefixedValueName(a_word[1..]);
        }

        /// <summary>
        /// Check if the given word is a valid variable name,
        /// starting with a the correct prefix (see TSConstants.VariablePrefix)
        /// and containing only allowed characters
        /// </summary>
        /// <param name="a_word">The word to check</param>
        /// <param name="a_allowWildcard">True to allow wildcard postfix</param>
        /// <returns>The validation result</returns>
        public TSValidator IsVariableName(string a_word, bool a_allowWildcard = false)
        {
            if (a_word.Length < 1)
            {
                mBroadcaster?.Error("A value name must not be empty and must start with a prefix.");
                IsValid = false;
                return this;
            }
            if (a_word[0] != TSConstants.VariablePrefix)
            {
                mBroadcaster?.Error("The value name \"{0}\" has an invalid prefix '{1}', expected '{2}'", a_word, a_word[0], TSConstants.VariablePrefix);
                IsValid = false;
                return this;
            }
            if (a_allowWildcard && a_word.EndsWith(TSConstants.WildcardPostfix))
            {
                return IsUnprefixedValueName(a_word[1..^1]);
            }
            return IsUnprefixedValueName(a_word[1..]);
        }

        /// <summary>
        /// Check if the given word is a valid jump label name,
        /// starting with a the correct prefix (see TSConstants.JumpLabelPrefix)
        /// and containing only allowed characters
        /// </summary>
        /// <param name="a_word">The word to check</param>
        /// <returns>The validation result</returns>
        public TSValidator IsJumpLabelName(string a_word)
        {
            if (a_word.Length < 1)
            {
                mBroadcaster?.Error("A jump label name must not be empty and must start with the prefix '{0}'.", TSConstants.JumpLabelPrefix);
                IsValid = false;
                return this;
            }
            if (a_word[0] != TSConstants.JumpLabelPrefix)
            {
                mBroadcaster?.Error("The jump label name \"{0}\" has an invalid prefix '{1}', expected '{2}'", a_word, a_word[0], TSConstants.JumpLabelPrefix);
                IsValid = false;
                return this;
            }
            // same naming rules as for value names
            return IsUnprefixedValueName(a_word[1..]);
        }

        /// <summary>
        /// Check if the given arguments list contains at least a minimal number of elements
        /// </summary>
        /// <param name="a_arguments">The arguments list to check</param>
        /// <param name="a_minimalCount">The minimal number of elements that are expected</param>
        /// <param name="a_expectMore">If true, having more arguments than a_minimalCount won't raise a warning</param>
        /// <returns>The validation result</returns>
        public TSValidator ContainsEnoughArguments(IList a_arguments, int a_minimalCount, bool a_expectMore = false)
        {
            if (a_arguments.Count < a_minimalCount)
            {
                mBroadcaster?.Error("Not enough arguments given, got {0}, expected {1}.", a_arguments.Count, a_minimalCount);
                IsValid = false;
                return this;
            }
            if (!a_expectMore && (a_arguments.Count > a_minimalCount))
            {
                mBroadcaster?.Warn("Too many arguments given, got {0}, expected {1}.", a_arguments.Count, a_minimalCount);
                // too many elements gives a warning, but is not a problem
            }
            return this;
        }

        /// <summary>
        /// Check if the given word is a valid trigger command,
        /// containing only allowed characters
        /// </summary>
        /// <param name="a_word">The word to check</param>
        /// <returns>The validation result</returns>
        public TSValidator IsTriggerCommand(string a_word)
        {
            if (a_word.Length == 0)
            {
                mBroadcaster?.Error("A trigger command must not be empty.");
                IsValid = false;
                return this;
            }
            if (a_word.Any(c => !char.IsLetterOrDigit(c)))
            {
                mBroadcaster?.Error("The trigger command \"{0}\" contains invalid characters.", a_word);
                IsValid = false;
            }
            return this;
        }
    }
}
