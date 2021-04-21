using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace TEABot.IRC
{
    /// <summary>
    /// A parsed IRC message
    /// </summary>
    public class TIrcMessage
    {
        #region Constructors

        /// <summary>
        /// Construct a message from parsed parts
        /// </summary>
        /// <param name="a_tags">Message tags from IRC extensions / capabilities</param>
        /// <param name="a_prefix">Message prefix, e.g. a server or user name</param>
        /// <param name="a_command">Command keyword</param>
        /// <param name="a_pars">Command parameters</param>
        public TIrcMessage(MessageTags a_tags, MessagePrefix a_prefix, MessageCommand a_command, MessageParams a_pars)
        {
            Tags = a_tags;
            Prefix = a_prefix;
            Command = a_command;
            Params = a_pars;
        }

        /// <summary>
        /// Construct a message from a command only
        /// </summary>
        /// <param name="a_command">Command keyword</param>
        public TIrcMessage(TIrcCommand a_command)
        {
            Tags = null;
            Prefix = null;
            Command = new MessageCommand(a_command);
            Params = null;
        }

        /// <summary>
        /// Construct a message from a command and parameters
        /// </summary>
        /// <param name="a_command">Command keyword</param>
        /// <param name="a_middles">Middle parameters, which do not include spaces</param>
        /// <param name="a_trailing">Trailing parameter, may contain spaces</param>
        public TIrcMessage(TIrcCommand a_command, IEnumerable<string> a_middles, string a_trailing)
        {
            Tags = null;
            Prefix = null;
            Command = new MessageCommand(a_command);
            Params = new MessageParams(a_middles, a_trailing);
        }

        /// <summary>
        /// Construct a message from a nickname, command and parameters
        /// </summary>
        /// <param name="a_nickname">Sender nickname</param>
        /// <param name="a_command">Command keyword</param>
        /// <param name="a_middles">Middle parameters, which do not include spaces</param>
        /// <param name="a_trailing">Trailing parameter, may contain spaces</param>
        public TIrcMessage(string a_nickname, TIrcCommand a_command, IEnumerable<string> a_middles, string a_trailing)
        {
            Tags = null;
            Prefix = new MessagePrefix(a_nickname);
            Command = new MessageCommand(a_command);
            Params = new MessageParams(a_middles, a_trailing);
        }

        #endregion

        #region Properties

        // message    =  [ "@" tags SPACE ] [ ":" prefix SPACE ] command [ params ] crlf
        // Max length is 512 INCLUDING CRLF

        public MessageTags Tags { get; private set; }
        // tags     =  [ name "=" value ";" ]* [ name "=" value ]

        public MessagePrefix Prefix { get; private set; }
        // prefix     =  servername / ( nickname [ [ "!" user ] "@" host ] )

        public MessageCommand Command { get; private set; }
        // command    =  1*letter / 3digit
        // At least one letter or exactly three digits

        public MessageParams Params { get; private set; }
        // params     =  *14( SPACE middle ) [ SPACE ":" trailing ]
        //            =/ 14( SPACE middle ) [ SPACE [ ":" ] trailing ]
        /* That is...
         * none or at most 14 "middle" with a space in front,
         * optional: space, colon (optional if exactly 14 middle), exactly one trailing
         * 
         * middle     =  nospcrlfcl *( ":" / nospcrlfcl )
         *      that is, at least one, but infinite "non null, CR, LF, space, colon" characters,
         *      after the first character, colons may be included
         * trailing   =  *( ":" / " " / nospcrlfcl )
         *      that is, any number of (colon, space or (not null or CR or LF))
         */

        #endregion

        #region Private data

        private static readonly char sTagHead = '@';
        private static readonly char sTagTail = ' ';
        private static readonly char[] sTagDelimiterArray = new char[] { sTagHead, sTagTail };
        private static readonly char sPrefixHead = ':';
        private static readonly char sPrefixTail = ' ';
        private static readonly char[] sPrefixDelimiterArray = new char[] { sPrefixHead, sPrefixTail };
        private static readonly char sParamsDelimiter = ' ';
        private static readonly char[] sParamsDelimiterArray = new char[] { sParamsDelimiter };
        private static readonly string sMessageTail = "\r\n";

        #endregion

        #region Public static functions

        /// <summary>
        /// Parse an IRC message from a raw string
        /// </summary>
        /// <param name="a_rawMessage">The raw IRC message</param>
        /// <returns>The parsed message, or null on parse error</returns>
        public static TIrcMessage Parse(string a_rawMessage)
        {
            MessageTags tags = null;
            MessagePrefix prefix = null;
            MessageCommand command = null;
            MessageParams pars = null;

            var digestedMessage = a_rawMessage;

            if (digestedMessage.EndsWith(sMessageTail))
                digestedMessage = digestedMessage.Remove(digestedMessage.Length - 2);

            if (digestedMessage[0] == sTagHead)
            {
                var tagSplit = digestedMessage.Split(sTagDelimiterArray, 2, StringSplitOptions.RemoveEmptyEntries);

                if (tagSplit.Length < 2)
                    return null;

                tags = MessageTags.Parse(tagSplit[0]);

                digestedMessage = tagSplit[1];
            }

            if (digestedMessage[0] == sPrefixHead)
            {
                var prefixSplit = digestedMessage.Split(sPrefixDelimiterArray, 2, StringSplitOptions.RemoveEmptyEntries);

                if (prefixSplit.Length < 2)
                    return null;

                prefix = MessagePrefix.Parse(prefixSplit[0]);

                digestedMessage = prefixSplit[1];
            }

            var commandSplit = digestedMessage.Split(sParamsDelimiterArray, 2, StringSplitOptions.RemoveEmptyEntries);

            if (commandSplit.Length < 1)
                return null;

            command = MessageCommand.Parse(commandSplit[0]);

            var rawparams = String.Empty;
            if (commandSplit.Length >= 2)
            {
                rawparams = commandSplit[1];
            }

            if (rawparams.Length > 0)
            {
                pars = MessageParams.Parse(rawparams);
            }

            return new TIrcMessage(tags, prefix, command, pars);
        }

        #endregion

        #region Public functions

        /// <summary>
        /// Convert an IRC message into the corresponding raw message string
        /// </summary>
        /// <returns>The raw message string</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Prefix != null)
            {
                sb.Append(sPrefixHead);
                sb.Append(Prefix.ToString());
                sb.Append(sPrefixTail);
            }

            if (Command != null)
            {
                sb.Append(Command.ToString());
            }

            if (Params != null)
            {
                sb.Append(Params.ToString());
            }

            sb.Append(sMessageTail);

            return sb.ToString();
        }

        #endregion

        #region Internal classes

        /// <summary>
        /// Message tags from IRC extensions / capabilities
        /// </summary>
        public class MessageTags
        {
            /// <summary>
            /// Parse from raw message tag section
            /// </summary>
            /// <param name="a_rawTags">Raw tags</param>
            /// <returns>The parsed tags or null on parse failure</returns>
            internal static MessageTags Parse(string a_rawTags)
            {
                // TODO
                return null;
            }

            public override string ToString()
            {
                // TODO
                return String.Empty;
            }
        }

        /// <summary>
        /// Message prefix, e.g. server or user name
        /// </summary>
        public class MessagePrefix
        {
            /// <summary>
            /// Parse from raw message prefix section
            /// </summary>
            /// <param name="a_rawPrefix">Raw prefix</param>
            /// <returns>The parsed prefix, empty on failure</returns>
            internal static MessagePrefix Parse(string a_rawPrefix)
            {
                var result = new MessagePrefix();

                var trimmedPrefix = a_rawPrefix.Trim();

                if (trimmedPrefix.Contains(sHostIndicator))
                {
                    if (trimmedPrefix.Contains(sHostDelimiter))
                    {
                        result.IsNick = true;

                        // nickname [ ! user ] @ host
                        if (trimmedPrefix.Contains(sUserDelimiter))
                        {
                            // nickname ! user @ host
                            var userSplit = trimmedPrefix.Split(sUserDelimiterArray);
                            if (userSplit.Length >= 1)
                            {
                                result.NickName = userSplit[0];
                                if (userSplit.Length >= 2)
                                {
                                    var hostSplit = userSplit[1].Split(sUserDelimiterArray);
                                    if (hostSplit.Length >= 1)
                                    {
                                        result.UserName = hostSplit[0];
                                        if (hostSplit.Length >= 2)
                                        {
                                            result.HostName = hostSplit[1];
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // nickname @ host
                            var split = trimmedPrefix.Split(sHostDelimiterArray);
                            if (split.Length >= 1)
                            {
                                result.NickName = split[0];
                                if (split.Length >= 2)
                                {
                                    result.HostName = split[1];
                                }
                            }
                        }
                    }
                    else
                    {
                        // server name
                        result.IsServer = true;
                        result.ServerName = trimmedPrefix;
                    }
                }
                else
                {
                    result.IsNick = true;
                    result.NickName = trimmedPrefix;
                }

                return result;
            }

            /// <summary>
            /// Construct new empty prefix
            /// </summary>
            public MessagePrefix()
            {
            }

            /// <summary>
            /// Construct from a user nickname
            /// </summary>
            /// <param name="a_nickname">User nickname</param>
            public MessagePrefix(string a_nickname)
            {
                IsNick = true;
                NickName = a_nickname;
            }

            private static readonly char sHostIndicator = '.';
            private static readonly char sHostDelimiter = '@';
            private static readonly char[] sHostDelimiterArray = new char[] { sHostDelimiter };
            private static readonly char sUserDelimiter = '!';
            private static readonly char[] sUserDelimiterArray = new char[] { sUserDelimiter };

            /// <summary>
            /// Server name if specified
            /// </summary>
            public string ServerName { get; private set; } = String.Empty;
            /// <summary>
            /// True if the prefix contains a server name
            /// </summary>
            public bool IsServer { get; private set; } = false;

            /// <summary>
            /// User's nickname if specified
            /// </summary>
            public string NickName { get; private set; } = String.Empty;
            /// <summary>
            /// Actual username if specified
            /// </summary>
            public string UserName { get; private set; } = String.Empty;
            /// <summary>
            /// User's hostname if specified
            /// </summary>
            public string HostName { get; private set; } = String.Empty;
            /// <summary>
            /// True if the prefix contains a user name, consisting of a combination of
            /// nickname, username and hostname, with each part being optional
            /// </summary>
            public bool IsNick { get; private set; } = false;

            public override string ToString()
            {
                if (IsServer)
                {
                    return ServerName;
                }
                else if (IsNick)
                {
                    var sb = new StringBuilder();

                    sb.Append(NickName);

                    if (UserName != null)
                    {
                        sb.Append(sUserDelimiter);
                        sb.Append(UserName);
                    }

                    if (HostName != null)
                    {
                        sb.Append(sHostDelimiter);
                        sb.Append(HostName);
                    }

                    return sb.ToString();
                }

                return null;
            }
        }

        /// <summary>
        /// IRC command keyword
        /// </summary>
        public class MessageCommand
        {
            /// <summary>
            /// Parse command from raw section
            /// </summary>
            /// <param name="a_rawCommand">Raw command section</param>
            /// <returns>The parsed command keyword or response code</returns>
            internal static MessageCommand Parse(string a_rawCommand)
            {
                var trimmedCommand = a_rawCommand.Trim();

                int code;
                if ((trimmedCommand.Length == 3) && int.TryParse(trimmedCommand, out code))
                {
                    return new MessageCommand((TIrcResponseCode)code);
                }
                return new MessageCommand(trimmedCommand.ParseIrcCommand());
            }

            /// <summary>
            /// Construct from known command keyword
            /// </summary>
            /// <param name="a_command">Command keyword</param>
            public MessageCommand(TIrcCommand a_command)
            {
                IsCommand = true;
                Command = a_command;
            }

            /// <summary>
            /// Construct from known response code
            /// </summary>
            /// <param name="a_response">Response code</param>
            public MessageCommand(TIrcResponseCode a_response)
            {
                IsResponse = true;
                Response = a_response;
            }

            /// <summary>
            /// Response code
            /// </summary>
            public TIrcResponseCode Response { get; private set; } = TIrcResponseCode.invalid;
            /// <summary>
            /// True if the command is a response code
            /// </summary>
            public bool IsResponse { get; private set; } = false;

            /// <summary>
            /// Command keyword
            /// </summary>
            public TIrcCommand Command { get; private set; } = TIrcCommand.invalid;
            /// <summary>
            /// True if the command is a command keyword
            /// </summary>
            public bool IsCommand { get; private set; } = false;

            public override string ToString()
            {
                if (IsResponse)
                {
                    return String.Format("{0:D3}", ((int)Response));
                }
                else if (IsCommand)
                {
                    return Command.ToString();
                }

                return null;
            }
        }

        /// <summary>
        /// Message parameters, meaning depends on command.
        /// Contains middle parts, each without spaces, and a traling part which may contain spaces
        /// </summary>
        public class MessageParams
        {
            /// <summary>
            /// Parse message parameters from the parameters section
            /// </summary>
            /// <param name="a_rawParams">The raw parameters section</param>
            /// <returns>The params, empty on failure</returns>
            internal static MessageParams Parse(string a_rawParams)
            {
                var result = new MessageParams();

                var middles = new List<string>();
                var trailing = String.Empty;

                var trailingSplit = a_rawParams.Split(sTrailingDelimiterArray, 2, StringSplitOptions.None);

                if (trailingSplit.Length >= 1)
                {
                    var middleSplit = trailingSplit[0].Split(sMiddleDelimiterArray, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var m in middleSplit)
                    {
                        middles.Add(m);
                    }

                    if (trailingSplit.Length >= 2)
                    {
                        trailing = trailingSplit[1];
                    }
                }

                result.Middles = new List<string>(middles);
                result.Trailing = trailing;

                return result;
            }

            /// <summary>
            /// Construct empty params
            /// </summary>
            private MessageParams()
            {
            }

            /// <summary>
            /// Construct from middle and trailing parts
            /// </summary>
            /// <param name="a_middles">Middle parts, must not contain spaces</param>
            /// <param name="a_trailing">Trailing part, may contain spaces</param>
            public MessageParams(IEnumerable<string> a_middles, string a_trailing)
            {
                Middles = (a_middles != null) ? new List<string>(a_middles) : new List<string>();
                Trailing = String.IsNullOrEmpty(a_trailing) ? String.Empty : a_trailing;
            }

            private static readonly char sMiddleDelimiter = ' ';
            private static readonly char[] sMiddleDelimiterArray = new char[] { sMiddleDelimiter };
            private static readonly string sTrailingDelimiter = " :";
            private static readonly string[] sTrailingDelimiterArray = new string[] { sTrailingDelimiter };

            /// <summary>
            /// Middle parts, must not contain spaces
            /// </summary>
            public List<string> Middles { get; private set; } = new List<string>();
            /// <summary>
            /// Trailing part, may contain spaces
            /// </summary>
            public string Trailing { get; private set; } = String.Empty;

            public override string ToString()
            {
                var sb = new StringBuilder();

                foreach (var m in Middles)
                {
                    sb.Append(sMiddleDelimiter);
                    sb.Append(m);
                }

                if (!String.IsNullOrEmpty(Trailing))
                {
                    sb.Append(sTrailingDelimiter);
                    sb.Append(Trailing);
                }

                return sb.ToString();
            }
        }

        #endregion
    }
}
