using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.Bot
{
    /// <summary>
    /// TEABot configuration, may be global or per channel
    /// </summary>
    public class TBConfiguration
    {
        #region Public properties

        /// <summary>
        /// List of users with special admin permissions
        /// </summary>
        [TBOptionName("superUser")]
        public List<string> SuperUsers { get; private set; } = new List<string>();

        /// <summary>
        /// The bot's own user name for display
        /// </summary>
        [TBOptionName("self")]
        public string Self { get; private set; } = String.Empty;

        /// <summary>
        /// The IRC host address
        /// Ignored on per-channel configurations
        /// Not inherited
        /// </summary>
        [TBOptionName("host")]
        public string Host { get; private set; } = String.Empty;

        /// <summary>
        /// The IRC host port
        /// Ignored on per-channel configurations
        /// Not inherited
        /// </summary>
        [TBOptionName("port")]
        public long Port { get; private set; } = 0L;

        /// <summary>
        /// The IRC login name
        /// Ignored on per-channel configurations
        /// Not inherited
        /// </summary>
        [TBOptionName("login")]
        public string Login { get; private set; } = String.Empty;

        /// <summary>
        /// Authentication string for IRC login
        /// Ignored on per-channel configurations
        /// Not inherited
        /// </summary>
        [TBOptionName("auth")]
        public string Auth { get; private set; } = String.Empty;

        /// <summary>
        /// Whether to use SSL when connecting to the IRC host
        /// Ignored on per-channel configurations
        /// Not inherited
        /// </summary>
        [TBOptionName("ssl")]
        public bool SSL { get; private set; } = false;

        /// <summary>
        /// Whether to support Twitch IRC Capabilities (extensions)
        /// Ignored on per-channel configurations
        /// Not inherited
        /// </summary>
        [TBOptionName("twitchCaps")]
        public bool TwitchCaps { get; private set; } = false;

        /// <summary>
        /// IRC bot command prefix
        /// </summary>
        [TBOptionName("prefix")]
        public string Prefix { get; private set; } = String.Empty;

        /// <summary>
        /// Time interval in seconds for flood prevention
        /// </summary>
        [TBOptionName("maxMessageInterval")]
        public long MaxMessageInterval { get; private set; } = 30L;

        /// <summary>
        /// Maximal number of messages that may be sent within the MaxMessageInterval
        /// </summary>
        [TBOptionName("maxMessageCount")]
        public long MaxMessageCount { get; private set; } = 20L;

        /// <summary>
        /// Whether to join the channel after connecting to the IRC host or after reloading the config.
        /// Otherwise, the channel configuration has no effect and the channel will be ignored.
        /// Ignored on global configuration
        /// Not inherited
        /// </summary>
        [TBOptionName("join")]
        public bool Join { get; private set; } = false;

        /// <summary>
        /// Static message to send after joining the channel.
        /// Ignored on global configuration, but inherited
        /// </summary>
        [TBOptionName("hello")]
        public string Hello { get; private set; } = String.Empty;

        /// <summary>
        /// Whether to log INFO messages of bot execution
        /// </summary>
        [TBOptionName("infoLog")]
        public bool InfoLog { get; private set; } = false;

        /// <summary>
        /// DateTime.ToString() format for log timestamps
        /// </summary>
        [TBOptionName("timestampFormat")]
        public string TimestampFormat { get; private set; } = "HH:mm:ss";

        /// <summary>
        /// Maximal number of messages to be kept in the log
        /// </summary>
        [TBOptionName("maxLogSize")]
        public long MaxLogSize { get; private set; } = 5000L;

        /// <summary>
        /// Number of messages to be keep when cutting down the log after exceeding @MaxLogSize
        /// </summary>
        [TBOptionName("logCutoffSize")]
        public long LogCutoffSize { get; private set; } = 3000L;

        /// <summary>
        /// Path to directory where persistent data files are to be stored.
        /// If relative, will be assumed to be relative to config base directory.
        /// Ignored on per-channel configurations
        /// Not inherited
        /// </summary>
        [TBOptionName("storageDirectory")]
        public string StorageDirectory { get; private set; } = ".data";

        /// <summary>
        /// Enable or disable the WebSocket Server to use with the hurl instruction.
        /// Ignored on per-channel configurations
        /// Not inherited
        /// </summary>
        [TBOptionName("webSocketServer")]
        public bool UseWebSocketServer { get; private set; } = true;

        /// <summary>
        /// The web socket server port
        /// Ignored on per-channel configurations
        /// Not inherited
        /// </summary>
        [TBOptionName("webSocketPort")]
        public long WebSocketPort { get; private set; } = 8080L;

        #endregion Public properties

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public TBConfiguration() { }

        /// <summary>
        /// Create a configuration which inherits values from the given ancestor
        /// </summary>
        /// <param name="a_ancestor">An ancestor configuration to inherit from</param>
        public TBConfiguration(TBConfiguration a_ancestor)
        {
            SuperUsers = new List<string>(a_ancestor.SuperUsers);
            Self = a_ancestor.Self;
            Prefix = a_ancestor.Prefix;
            MaxMessageInterval = a_ancestor.MaxMessageInterval;
            MaxMessageCount = a_ancestor.MaxMessageCount;
            Hello = a_ancestor.Hello;
            InfoLog = a_ancestor.InfoLog;
            TimestampFormat = a_ancestor.TimestampFormat;
            MaxLogSize = a_ancestor.MaxLogSize;
            LogCutoffSize = a_ancestor.LogCutoffSize;
        }

        #endregion Constructors

        #region Public methods

        /// <summary>
        /// Parse configuration from a file
        /// </summary>
        /// <param name="a_filename">The filename, ideally absolute</param>
        /// <returns>True on successful parsing, false on severe error.</returns>
        public bool ParseFromFile(string a_filename)
        {
            try
            {
                using var sr = File.OpenText(a_filename);
                return ParseFromReader(sr);
            }
            catch (Exception e)
            {
                ErrorMessage?.Invoke(String.Format(
                    "Failed to open configuration file \"{0}\": {1}",
                    a_filename,
                    e.Message));
            }
            return false;
        }

        /// <summary>
        /// Parse configuration from a string
        /// </summary>
        /// <param name="a_configuration">The configuration to parse, with one option per line</param>
        /// <returns>True on successful parsing, false on severe error.</returns>
        public bool ParseFromText(string a_configuration)
        {
            try
            {
                using var sr = new StringReader(a_configuration);
                return ParseFromReader(sr);
            }
            catch (Exception e)
            {
                ErrorMessage?.Invoke(String.Format(
                    "Failed to parse configuration from string: {0}",
                    e.Message));
            }
            return false;
        }

        /// <summary>
        /// Parse configuration from a text reader
        /// </summary>
        /// <param name="a_reader">The configuration to parse, with one option per line</param>
        /// <returns>True on successful parsing, false on severe error.</returns>
        public bool ParseFromReader(TextReader a_reader)
        {
            InitializeOptionMap();

            // read script line by line
            string line;
            var lineCount = 0;
            bool hasParseError = false;
            while ((line = a_reader.ReadLine()) != null)
            {
                ++lineCount;

                // skip empty lines and comments
                if ((line.Length == 0)
                    || (line[0] == COMMENT_INDICATOR))
                {
                    continue;
                }

                // separate names and values
                string name;
                string value;
                var separatorIndex = line.IndexOf(VALUE_SEPARATOR);
                if (separatorIndex < 0)
                {
                    hasParseError = true;
                    ErrorMessage?.Invoke(String.Format(
                        "Configuration line {0} contains no value separator: \"{1}\"",
                        lineCount,
                        line));
                    continue;
                }
                name = line.Substring(0, separatorIndex);
                value = line[(separatorIndex + 1)..];

                // find corresponding instruction class
                if (sOptionProperties.TryGetValue(name.ToLowerInvariant(), out PropertyInfo optionProperty))
                {
                    if (optionProperty.PropertyType == typeof(string))
                    {
                        optionProperty.SetValue(this, value);
                    }
                    else if (optionProperty.PropertyType == typeof(long))
                    {
                        if (ParseNumberValue(value, out long parsedNMumberValue))
                        {
                            optionProperty.SetValue(this, parsedNMumberValue);
                        }
                        else
                        {
                            ErrorMessage?.Invoke(String.Format(
                                "Failed to parse numerical option in configuration line {0}, option {1}: \"{2}\"",
                                lineCount,
                                name,
                                value));
                        }
                    }
                    else if (optionProperty.PropertyType == typeof(bool))
                    {
                        if (ParseBoolValue(value, out bool parsedBoolValue))
                        {
                            optionProperty.SetValue(this, parsedBoolValue);
                        }
                        else
                        {
                            ErrorMessage?.Invoke(String.Format(
                                "Failed to parse boolean option in configuration line {0}, option {1}: \"{2}\"",
                                lineCount,
                                name,
                                value));
                        }
                    }
                    else if (optionProperty.PropertyType == typeof(List<string>))
                    {
                        var valueList = (List<string>)(optionProperty.GetValue(this));
                        valueList.Add(value);
                    }
                    else
                    {
                        hasParseError = true;
                        ErrorMessage?.Invoke(String.Format(
                            "Unsupported option type for configuration line {0} option {1}: \"{2}\"",
                            lineCount,
                            name,
                            value));
                    }
                }
                else
                {
                    hasParseError = true;
                    ErrorMessage?.Invoke(String.Format(
                        "Unknown option in configuration line {0}: \"{1}\"",
                        lineCount,
                        line));
                }
            }

            return !hasParseError;
        }

        /// <summary>
        /// Parse configuration from a string
        /// </summary>
        /// <param name="a_configuration">The configuration to parse, with one option per line</param>
        /// <returns>True on successful parsing, false on severe error.</returns>
        public bool Parse(string a_configuration)
        {
            throw new NotImplementedException();
        }

        #endregion Public methods

        #region Events


        /// <summary>
        /// Listeners receive error messages
        /// </summary>
        /// <param name="a_errorMessage">Error message text</param>
        public delegate void ErrorMessageHandler(string a_errorMessage);

        /// <summary>
        /// Error message event will be raised when reporting an error
        /// </summary>
        public event ErrorMessageHandler ErrorMessage;

        #endregion

        #region Private data

        /// <summary>
        /// Map from option name to option property info
        /// </summary>
        private static Dictionary<string, PropertyInfo> sOptionProperties = null;

        /// <summary>
        /// Known boolean option value strings and their boolean value,
        /// all strings should be lowercase as input will be converted ToLowerInvariant() for lookups
        /// </summary>
        private static readonly Dictionary<string, bool> sBooleanValues = new()
        {
            { "on", true },
            { "off", false },
            { "yes", true },
            { "no", false },
            { "true", true },
            { "false", false },
            { "1", true },
            { "0", false }
        };

        /// <summary>
        /// Separator character between option name and value
        /// </summary>
        private static readonly char VALUE_SEPARATOR = '=';

        /// <summary>
        /// Character at line start that indicates a comment line to be ignored while parsing
        /// </summary>
        private static readonly char COMMENT_INDICATOR = '#';

        #endregion

        #region Private methods

        /// <summary>
        /// Initialize the option name to property info map if not done already
        /// </summary>
        private static void InitializeOptionMap()
        {
            if (sOptionProperties != null) return;

            sOptionProperties = new Dictionary<string, PropertyInfo>();

            foreach (var property in typeof(TBConfiguration).GetProperties())
            {
                var attributes = property.GetCustomAttributes(typeof(TBOptionNameAttribute), false).Cast<TBOptionNameAttribute>().ToArray();
                if (attributes.Length > 0)
                {
                    sOptionProperties[attributes.First().Name.ToLowerInvariant()] = property;
                }
            }
        }

        /// <summary>
        /// Parse a numerical value
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <param name="a_parsedNumberValue">The parsed value</param>
        /// <returns>True iff the value was parsed successfully</returns>
        private static bool ParseNumberValue(string value, out long a_parsedNumberValue)
        {
            return Int64.TryParse(value.Trim(), out a_parsedNumberValue);
        }

        /// <summary>
        /// Parse a boolean value
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <param name="a_parsedBoolValue">The parsed value</param>
        /// <returns>True iff the value was parsed successfully</returns>
        private static bool ParseBoolValue(string value, out bool a_parsedBoolValue)
        {
            return sBooleanValues.TryGetValue(value.Trim().ToLowerInvariant(), out a_parsedBoolValue);
        }

        #endregion Private methods
    }
}
