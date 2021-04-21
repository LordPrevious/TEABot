using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.IRC
{
    /// <summary>
    /// IRC commands as per RFC 1459
    /// </summary>
    public enum TIrcCommand
    {
        invalid,

        PASS,
        NICK,
        USER,
        QUIT,
        JOIN,
        PART,
        PRIVMSG,
        NOTICE,
        MOTD,
        PING,
        PONG,
        ERROR,
        CAP
    }

    /// <summary>
    /// Extensions for TIrcCommand convenience
    /// </summary>
    public static class TIrcCommandExtensions
    {
        /// <summary>
        /// Convert a string to the corresponding IRC command, if any match
        /// </summary>
        /// <param name="a_command">The command to parse</param>
        /// <returns>The corresponding enum value, or TIrcCommand.invalid if no proper value matches</returns>
        public static TIrcCommand ParseIrcCommand(this string a_command)
        {
            foreach (TIrcCommand enumValue in Enum.GetValues(typeof(TIrcCommand)))
            {
                if (a_command.Equals(enumValue.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    return enumValue;
                }
            }
            return TIrcCommand.invalid;
        }
    }
}
