using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TEABot.IRC;
using TEABot.TEAScript;
using TEABot.Twitch;
using TEABot.WebSocket;

namespace TEABot.Bot
{
    /// <summary>
    /// Core chat bot stuff
    /// </summary>
    public class TBChatBot
    {
        #region Public properties

        /// <summary>
        /// Global scripts and configuration
        /// </summary>
        public TBChannel Global { get; } = new TBChannel();

        /// <summary>
        /// Per-channel scripts and configuration, map from name to channel data
        /// </summary>
        public Dictionary<string, TBChannel> Channels { get; } = new Dictionary<string, TBChannel>();

        /// <summary>
        /// Connection status of the bot's IRC client and WebSocket Server
        /// </summary>
        public TBConnectionStatus ConnectionStatus { get { return mConnectionStatus; } }
        private TBConnectionStatus mConnectionStatus;

        #endregion

        #region Constructors

        public TBChatBot()
        {
            mStorage.Broadcaster.Broadcast += BroadcastListener;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Reload bot configuration
        /// </summary>
        /// <param name="a_rootDirectory">The bot configuration root directory</param>
        public void ReloadConfig(string a_rootDirectory)
        {
            // reset old config
            Global.Configuration = new TBConfiguration();

            // load global config
            LoadConfig(a_rootDirectory, Global);

            // per-channel
            LoadDataFromAllChannelDirectories(a_rootDirectory,
                (la_channel, la_directory) =>
                {
                    // init channel config from global config
                    la_channel.Configuration = new TBConfiguration(Global.Configuration);
                    // load channel config
                    LoadConfig(la_directory, la_channel);
                });

            // set up config-dependent data
            if (Path.IsPathRooted(Global.Configuration.StorageDirectory))
            {
                // assume absolute path
                mStorage.StorageDirectory = Global.Configuration.StorageDirectory;
            }
            else
            {
                // assume path relative to config directory
                mStorage.StorageDirectory = Path.Combine(a_rootDirectory, Global.Configuration.StorageDirectory);
            }
        }

        /// <summary>
        /// Reload and recompile scripts
        /// </summary>
        /// <param name="a_rootDirectory">The bot configuration root directory</param>
        public void ReloadScripts(string a_rootDirectory)
        {
            // remove old global scripts
            Global.CommandScripts.Clear();
            Global.RegexScripts.Clear();
            Global.PeriodicScripts.Clear();

            // load global scripts
            LoadScripts(a_rootDirectory, Global);

            // per-channel
            LoadDataFromAllChannelDirectories(a_rootDirectory,
                (la_channel, la_directory) =>
                {
                    // remove old channel scripts
                    la_channel.CommandScripts.Clear();
                    la_channel.RegexScripts.Clear();
                    la_channel.PeriodicScripts.Clear();
                    // load channel scripts
                    LoadScripts(la_directory, la_channel);
                });
        }

        /// <summary>
        /// Cancel running executors
        /// </summary>
        public void Cancel()
        {
            mCTSource.Cancel();
            // create a new token source for new async tasks
            mCTSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Send a manual message
        /// </summary>
        /// <param name="a_channel">The channel the message should be sent to</param>
        /// <param name="a_message">The message to send</param>
        public void SendMessage(TBChannel a_channel, string a_message)
        {
            SendMessage(a_channel, a_message, true);
        }

        /// <summary>
        /// Handle a received message, executing any scripts the message should trigger
        /// </summary>
        /// <param name="a_channel">The channel the message was received on</param>
        /// <param name="a_message">The received message</param>
        /// <param name="a_sender">The message sender's username</param>
        /// <param name="a_tags">Optional message tags, may be null</param>
        public void HandleReceivedMessage(TBChannel a_channel, string a_message, string a_sender, IReadOnlyDictionary<string, string> a_tags)
        {
            OnChatMessage?.Invoke(a_channel, TBMessageDirection.RECEIVED, a_sender, a_message);

            // parse twitch tags
            TTwMessageTags twitchTags = null;
            if ((Global.Configuration.TwitchCaps)
                && (a_tags != null))
            {
                twitchTags = new(a_tags, a_message);
            }

            // update list of chatters
            AddChatterToList(a_channel.Name, a_sender, twitchTags?.DisplayName ?? a_sender);

            HandleCommandMessage(a_channel, a_message, a_sender, twitchTags);
            HandlePatternMessage(a_channel, a_channel, a_message, a_sender, twitchTags);
            HandleTwitchEmoteMessage(a_channel, a_message, a_sender, twitchTags);
        }

        /// <summary>
        /// Connect to the IRC server
        /// </summary>
        public void Connect()
        {
            if (mConnection != null)
            {
                // already connected, ignore connect request
                OnWarning?.Invoke(Global, "Already connected");
                return;
            }

            mConnectionStatus.IrcClientRunning = true;
            RaiseConnectionStatusChanged();

            OnInfo?.Invoke(Global, "Connecting...");

            // mark all channels as not joined
            foreach (var channel in Channels)
            {
                channel.Value.Joined = false;
            }

            // set up connection instance
            mConnection = new TIrcConnection();
            mConnection.OnConnected += Connection_OnConnected;
            mConnection.OnDisconnected += Connection_OnDisconnected;
            mConnection.OnError += Connection_OnError;
            mConnection.OnInfo += Connection_OnInfo;
            mConnection.OnJoin += Connection_OnJoin;
            mConnection.OnLogin += Connection_OnLogin;
            mConnection.OnMessage += Connection_OnMessage;
            mConnection.OnNotice += Connection_OnNotice;
            mConnection.OnPart += Connection_OnPart;
            mConnection.OnPing += Connection_OnPing;
            mConnection.OnPong += Connection_OnPong;
            mConnection.OnCapabilitiesGranted += Connection_OnCapabilitiesGranted;
            mConnection.OnRaw += Connection_OnRaw;

            // connect to server
            mConnection.Connect(Global.Configuration.Host, (ushort)Global.Configuration.Port,
                Global.Configuration.SSL, mCTSource.Token);

            if (Global.Configuration.UseWebSocketServer)
            {
                // set up web socket server
                mWebSockets = new TWSServer((ushort)Global.Configuration.WebSocketPort);
                mWebSockets.OnMessageReceived += WebSockets_OnMessageReceived;
                mWebSockets.OnError += WebSockets_OnError;
                mWebSockets.OnClientConnection += WebSockets_OnClientConnection;
                mWebSockets.Start();
                mHurler = new TBWebSocketHurler(mWebSockets);

                mConnectionStatus.WebSocketServerRunning = true;
                RaiseConnectionStatusChanged();
            }
        }

        /// <summary>
        /// Disconnect from the IRC server
        /// </summary>
        public void Disconnect()
        {
            // also cancel all running tasks
            Cancel();

            if (mConnection == null)
            {
                // not connected, ignore disconnect request
                OnWarning?.Invoke(Global, "Cannot disconnect without being connected");
                return;
            }

            OnNotice?.Invoke(Global, "Disconnecting...");

            // tear down web socket server
            mHurler = null;
            if (mWebSockets != null)
            {
                mWebSockets.Stop();
                mWebSockets.OnMessageReceived -= WebSockets_OnMessageReceived;
                mWebSockets.OnError -= WebSockets_OnError;
                mWebSockets.OnClientConnection -= WebSockets_OnClientConnection;
                mWebSockets = null;

                mConnectionStatus.WebSocketServerRunning = false;
                RaiseConnectionStatusChanged();
            }

            // disconnect from server
            mConnection.Disconnect();

            // clear connection instance
            mConnection.OnConnected -= Connection_OnConnected;
            mConnection.OnDisconnected -= Connection_OnDisconnected;
            mConnection.OnError -= Connection_OnError;
            mConnection.OnInfo -= Connection_OnInfo;
            mConnection.OnJoin -= Connection_OnJoin;
            mConnection.OnLogin -= Connection_OnLogin;
            mConnection.OnMessage -= Connection_OnMessage;
            mConnection.OnNotice -= Connection_OnNotice;
            mConnection.OnPart -= Connection_OnPart;
            mConnection.OnPing -= Connection_OnPing;
            mConnection.OnPong -= Connection_OnPong;
            mConnection.OnCapabilitiesGranted -= Connection_OnCapabilitiesGranted;
            mConnection.OnRaw -= Connection_OnRaw;
            mConnection = null;

            // mark all channels as parted
            foreach (var channel in Channels)
            {
                channel.Value.Joined = false;
            }

            mConnectionStatus.IrcClientRunning = false;
            RaiseConnectionStatusChanged();
        }

        /// <summary>
        /// Reconnect to the IRC host. Cancels all running scripts.
        /// </summary>
        public void ReconnectIrc()
        {
            //  cancel all running tasks
            Cancel();

            if (mConnection == null)
            {
                // not connected, ignore resconnect request
                OnWarning?.Invoke(Global, "Cannot reconnect without being connected");
                return;
            }

            // disconnect from IRC host
            mConnection.Disconnect();
            mConnectionStatus.IrcClientRunning = false;
            RaiseConnectionStatusChanged();

            // mark all channels as parted
            foreach (var channel in Channels)
            {
                channel.Value.Joined = false;
            }

            // reconnect to IRC host
            mConnection.Connect(Global.Configuration.Host, (ushort)Global.Configuration.Port,
                Global.Configuration.SSL, mCTSource.Token);
            mConnectionStatus.IrcClientRunning = true;
            RaiseConnectionStatusChanged();
        }

        /// <summary>
        /// Restart the WebSocket server
        /// </summary>
        public void RestartWebSocketServer()
        {
            if (mWebSockets == null)
            {
                OnWarning?.Invoke(Global, "Cannot restart WebSocket server when it's not running.");
                return;
            }

            mConnectionStatus.WebSocketServerRunning = false;
            RaiseConnectionStatusChanged();
            mWebSockets.Stop();

            mWebSockets.Start();
            mConnectionStatus.WebSocketServerRunning = false;
            RaiseConnectionStatusChanged();
        }

        #endregion

        #region Events

        /// <summary>
        /// A connection status change handler
        /// </summary>
        /// <param name="a_sender">The event sender</param>
        /// <param name="a_connectionStatus">The new connection statuus</param>
        public delegate void ConnectionStatusChangeHandler(TBChatBot a_sender, TBConnectionStatus a_connectionStatus);
        /// <summary>
        /// Connection status change event
        /// </summary>
        public event ConnectionStatusChangeHandler OnConnectionStatusChanged;
        /// <summary>
        /// Raise a connection status change event
        /// </summary>
        private void RaiseConnectionStatusChanged()
        {
            OnConnectionStatusChanged?.Invoke(this, mConnectionStatus);
        }

        /// <summary>
        /// A notice message handler
        /// </summary>
        /// <param name="a_channel">The channel related to the notice, may be null</param>
        /// <param name="a_message">The message</param>
        public delegate void NoticeHandler(TBChannel a_channel, string a_message);

        /// <summary>
        /// Info notice event
        /// </summary>
        public event NoticeHandler OnInfo;

        /// <summary>
        /// Important info notice event
        /// </summary>
        public event NoticeHandler OnNotice;

        /// <summary>
        /// Warning notice event
        /// </summary>
        public event NoticeHandler OnWarning;

        /// <summary>
        /// Error notice event
        /// </summary>
        public event NoticeHandler OnError;

        /// <summary>
        /// A chat message handler
        /// </summary>
        /// <param name="a_channel">The channel to which the message belongs</param>
        /// <param name="a_direction">Whether the message was sent or received</param>
        /// <param name="a_sender">The message sender</param>
        /// <param name="a_message">The message</param>
        public delegate void ChatMessageHandler(TBChannel a_channel, TBMessageDirection a_direction, string a_sender, string a_message);

        /// <summary>
        /// Chat message event
        /// </summary>
        public event ChatMessageHandler OnChatMessage;

        #endregion

        #region Private data

        /// <summary>
        /// Used to cancel running tasks on exit
        /// </summary>
        private CancellationTokenSource mCTSource = new();

        /// <summary>
        /// Maximal time for regex pattern matching when checking chat messages for script triggers
        /// </summary>
        private static readonly TimeSpan cRegexTimeout = TimeSpan.FromSeconds(2);

        /// <summary>
        /// The IRC connection
        /// </summary>
        private TIrcConnection mConnection = null;

        /// <summary>
        /// Lock for message sending as multiple threads may request accessing the IRC connection
        /// </summary>
        private readonly object mMessageSendLock = new();

        /// <summary>
        /// Web socket server
        /// </summary>
        private TWSServer mWebSockets = null;

        /// <summary>
        /// A storage provider for access to persistent data
        /// </summary>
        private readonly TBStorage mStorage = new();

        /// <summary>
        /// Hurler provider
        /// </summary>
        private TBWebSocketHurler mHurler = null;

        /// <summary>
        /// List provider
        /// </summary>
        private readonly TBListProvider mLists = new();

        #endregion

        #region Private methods

        /// <summary>
        /// Perform a loading operation for each channel subdirectory of the given root directory,
        /// creating the respective channel in Channels if needed.
        /// Channels starting with a period ('.') will be skipped.
        /// </summary>
        /// <param name="a_rootDirectory">The root directory for which to process subdirectories</param>
        /// <param name="a_loader">A loader function invoked for each subdirectory, being passed the corresponding channel and directory name</param>
        private void LoadDataFromAllChannelDirectories(string a_rootDirectory, Action<TBChannel, string> a_loader)
        {
            try
            {
                var subDirectories = Directory.GetDirectories(a_rootDirectory);
                foreach (var d in subDirectories)
                {
                    // get channel data
                    var channelName = Path.GetFileName(d).ToLowerInvariant();
                    if (!channelName.StartsWith('.'))
                    {
                        if (!(Channels.TryGetValue(channelName, out TBChannel channel)))
                        {
                            channel = new TBChannel()
                            {
                                Name = channelName
                            };
                            Channels[channelName] = channel;
                        }
                        // invoke loader
                        a_loader?.Invoke(channel, d);
                    }
                }
            }
            catch (Exception e)
            {
                OnError?.Invoke(null, String.Format("Failed to read from channel root directory: {0}", e.Message));
            }
        }

        /// <summary>
        /// Load configuration files from the given directory into the given channel's configuration configuration
        /// </summary>
        /// <param name="a_directory">The directory from which to load configuration files</param>
        /// <param name="a_channel">The channel to store the configuration in</param>
        private void LoadConfig(string a_directory, TBChannel a_channel)
        {
            OnInfo?.Invoke(a_channel, String.Format("Loading config for {1} from {2}...{0}",
                Environment.NewLine,
                a_channel.Name,
                a_directory));

            // iterate all config files in the directory
            try
            {
                var files = Directory.GetFiles(a_directory, "*.tss", SearchOption.TopDirectoryOnly);
                foreach (var f in files)
                {
                    OnInfo?.Invoke(a_channel, String.Format("Parsing configuration file {1}...{0}",
                        Environment.NewLine,
                        f));

                    void errorMessageHandler(string la_errorMessage)
                    {
                        OnError?.Invoke(a_channel, la_errorMessage);
                    }

                    a_channel.Configuration.ErrorMessage += errorMessageHandler;
                    a_channel.Configuration.ParseFromFile(f);
                    a_channel.Configuration.ErrorMessage -= errorMessageHandler;
                }
            }
            catch (Exception e)
            {
                OnError?.Invoke(a_channel, String.Format("Failed to read from channel directory: {0}", e.Message));
            }
        }

        /// <summary>
        /// Load and compile scripts from the given directory for the given channel
        /// </summary>
        /// <param name="a_directory">The directory from which to load script files</param>
        /// <param name="a_channel">The channel to store the scripts in</param>
        private void LoadScripts(string a_directory, TBChannel a_channel)
        {
            OnInfo?.Invoke(a_channel, String.Format("Loading scripts for {1} from {2}...{0}",
                Environment.NewLine,
                a_channel.Name,
                a_directory));

            // listen on parser broadcaster
            TSScriptParser.Broadcaster.Context = Global;
            TSScriptParser.Broadcaster.Broadcast += BroadcastListener;

            // iterate all script files in the directory
            try
            {
                var files = Directory.GetFiles(a_directory, "*.tsc", SearchOption.TopDirectoryOnly);
                foreach (var f in files)
                {
                    OnInfo?.Invoke(a_channel, String.Format("Parsing script {1}...{0}",
                        Environment.NewLine,
                        f));
                    var script = TSScriptParser.ParseScriptFromFile(f);
                    if (null != script)
                    {
                        // add trigger commands
                        script.Commands.ForEach(c => a_channel.CommandScripts[c] = script);
                        // add regex patterns
                        if (!String.IsNullOrEmpty(script.RegexPattern))
                        {
                            // compile regex for faster use on many inputs
                            var regex = new Regex(script.RegexPattern, RegexOptions.Compiled, cRegexTimeout);
                            a_channel.RegexScripts[regex] = script;
                        }
                        // add to periodic execution list
                        if (script.Interval > 0)
                        {
                            a_channel.PeriodicScripts.Add(script);
                        }
                        // add to special trigger lists
                        if (script.TwitchEmotes)
                        {
                            a_channel.TwitchEmoteScripts.Add(script);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                OnError?.Invoke(a_channel, String.Format("Failed to read from channel directory: {0}", e.Message));
            }

            // detach parser broadcaster
            TSScriptParser.Broadcaster.Broadcast -= BroadcastListener;
        }

        /// <summary>
        /// Forward a message from a script broadcaster
        /// </summary>
        /// <param name="a_channel">The associated channel</param>
        /// <param name="a_type">The broadcast message type</param>
        /// <param name="a_message">The message text</param>
        private void LogBroadcastMessage(TBChannel a_channel, TSBroadcaster.MessageType a_type, string a_message)
        {
            switch (a_type)
            {
                case TSBroadcaster.MessageType.ERROR:
                    OnError?.Invoke(a_channel, a_message);
                    break;
                case TSBroadcaster.MessageType.WARNING:
                    OnWarning?.Invoke(a_channel, a_message);
                    break;
                case TSBroadcaster.MessageType.FLUSH:
                    SendMessage(a_channel, a_message, false);
                    break;
                case TSBroadcaster.MessageType.INFO:
                default:
                    if (!(a_channel.Configuration.InfoLog))
                    {
                        // skip logging of INFO messages
                        return;
                    }
                    OnInfo?.Invoke(a_channel, a_message);
                    break;
            }
        }

        /// <summary>
        /// Handle a received message, executing any scripts the message should trigger via commands
        /// </summary>
        /// <param name="a_channel">The channel with scripts to execute</param>
        /// <param name="a_message">The received message</param>
        /// <param name="a_sender">The message sender's username</param>
        /// <param name="a_twitchTags">Optional twitch tags, may be null</param>
        private void HandleCommandMessage(TBChannel a_channel, string a_message, string a_sender, TTwMessageTags a_twitchTags)
        {
            if (!(a_message.StartsWith(a_channel.Configuration.Prefix))) return;

            // get command
            var spaceIndex = a_message.IndexOf(' ');
            string command;
            string arguments;
            if (spaceIndex > 0)
            {
                // command is after prefix until first space
                command = a_message[a_channel.Configuration.Prefix.Length..spaceIndex];
                // arguments are remaining input
                arguments = a_message[(spaceIndex + 1)..];
            }
            else
            {
                // command is entire input except for command prefix
                command = a_message[a_channel.Configuration.Prefix.Length..];
                // no arguments
                arguments = String.Empty;
            }

            // check if any script is triggered by this command
            command = command.ToLowerInvariant();
            if (a_channel.CommandScripts.TryGetValue(command, out TSCompiledScript script)
                || ((a_channel != Global) && Global.CommandScripts.TryGetValue(command, out script)))
            {
                ExecuteScript(a_channel, script, a_sender, arguments, a_twitchTags);
            }
        }

        /// <summary>
        /// Handle a received message, executing any scripts the message should trigger via regex patterns
        /// </summary>
        /// <param name="a_channel">The channel with scripts to execute</param>
        /// <param name="a_executionChannel">The channel on which to execute the script</param>
        /// <param name="a_message">The received message</param>
        /// <param name="a_sender">The message sender's username</param>
        /// <param name="a_twitchTags">Optional twitch tags, may be null</param>
        private void HandlePatternMessage(TBChannel a_channel, TBChannel a_executionChannel, string a_message, string a_sender, TTwMessageTags a_twitchTags)
        {
            // check for regex pattern triggers
            foreach (var regexScript in a_channel.RegexScripts)
            {
                if (regexScript.Key.IsMatch(a_message))
                {
                    // execute script
                    ExecuteScript(a_executionChannel, regexScript.Value, a_sender, String.Empty, a_twitchTags);
                }
            }
            // check global context as well
            if (a_channel != Global)
            {
                HandlePatternMessage(Global, a_executionChannel, a_message, a_sender, a_twitchTags);
            }
        }

        /// <summary>
        /// Handle a received message, executing any scripts the message should trigger via twitch emotes
        /// </summary>
        /// <param name="a_channel">The channel with scripts to execute</param>
        /// <param name="a_message">The received message</param>
        /// <param name="a_sender">The message sender's username</param>
        /// <param name="a_twitchTags">Optional twitch tags, may be null</param>
        private void HandleTwitchEmoteMessage(TBChannel a_channel, string a_message, string a_sender, TTwMessageTags a_twitchTags)
        {
            // message must contain emotes to trigger
            if ((a_twitchTags == null)
                || (a_twitchTags.Emotes == null)
                || (a_twitchTags.Emotes.Emotes == null)
                || !(a_twitchTags.Emotes.Emotes.Any())
                // skip further preparations if no emote trigger scripts are present
                || (!a_channel.TwitchEmoteScripts.Any() && !Global.TwitchEmoteScripts.Any())) return;

            // strip emotes from original message by replacing all occurrences with whitespace,
            // so script parameter parsing will ignore them
            var strippedMessage = new StringBuilder(a_message);
            if (strippedMessage.Length > 0)
            {
                foreach (var emote in a_twitchTags.Emotes.Emotes)
                {
                    for (var index = Math.Max(emote.Start, 0);
                        index < Math.Min(emote.End + 1, strippedMessage.Length);
                        ++index)
                    {
                        strippedMessage[index] = ' ';
                    }
                }
            }
            var arguments = strippedMessage.ToString();

            // execute channel emote scripts
            foreach (var localEmoteScript in a_channel.TwitchEmoteScripts)
            {
                ExecuteScript(a_channel, localEmoteScript, a_sender, arguments, a_twitchTags);
            }
            // execute global channel emote scripts as well
            if (a_channel != Global)
            {
                foreach (var globalEmoteScript in Global.TwitchEmoteScripts)
                {
                    ExecuteScript(a_channel, globalEmoteScript, a_sender, arguments, a_twitchTags);
                }
            }
        }

        /// <summary>
        /// Add a chatter to the list of chat users;
        /// perform list cut-off as required.
        /// </summary>
        /// <param name="a_channel">The channel the user chatted on</param>
        /// <param name="a_nick">The user's IRC nick</param>
        /// <param name="a_chatter">The user's display name</param>
        private void AddChatterToList(string a_channel, string a_nick, string a_chatter)
        {
            lock (mLists.Users)
            {
                // add to encountered users
                mLists.Users.Add(new()
                {
                    { "channel", a_channel },
                    { "user", a_nick },
                    { "name", a_chatter },
                    { "timestamp", DateTime.Now.ToString(Global.Configuration.TimestampFormat) }
                });
                // check if list is getting too long
                if ((mLists.Users.Count > Global.Configuration.MaxLogSize)
                    && (Global.Configuration.LogCutoffSize < mLists.Users.Count))
                {
                    // reduce list to a smaller size
                    mLists.Users.RemoveRange(0, mLists.Users.Count - (int)Global.Configuration.LogCutoffSize);
                }
            }
        }

        /// <summary>
        /// Execute a script
        /// </summary>
        /// <param name="a_channel">The channel for which the script is executed</param>
        /// <param name="a_script">The script to execute</param>
        /// <param name="a_sender">The original message sender</param>
        /// <param name="a_arguments">Any script arguments</param>
        /// <param name="a_twitchTags">Optional twitch tags, may be null</param>
        private void ExecuteScript(TBChannel a_channel, TSCompiledScript a_script, string a_sender, string a_arguments, TTwMessageTags a_twitchTags)
        {
            var executor = new TBTaskedExecutor(mStorage, mHurler, mLists, a_twitchTags, a_channel, a_script, a_arguments, a_sender, mCTSource.Token);
            if (executor.InitializeContext())
            {
                executor.Broadcaster.Context = a_channel;
                executor.Broadcaster.Broadcast += BroadcastListener;
                executor.OnExecutionCompleted += Executor_ExecutionCompleted;
                executor.Execute();
            }
        }

        /// <summary>
        /// Start periodically executing all periodic scripts of a channel
        /// </summary>
        /// <param name="a_channel">The channel for which the scripts are to be executed</param>
        private void StartPeriodicScriptExecution(TBChannel a_channel)
        {
            foreach (var script in a_channel.PeriodicScripts)
            {
                StartPeriodicScriptExecution(a_channel, script);
            }
        }

        /// <summary>
        /// Start periodically executing a script
        /// </summary>
        /// <param name="a_channel">The channel for which the script is to be executed</param>
        /// <param name="a_script">The script to execute periodically</param>
        private void StartPeriodicScriptExecution(TBChannel a_channel, TSCompiledScript a_script)
        {
            if (a_script.Interval > 0)
            {
                var executor = new TBTaskedExecutor(mStorage, mHurler, mLists, null, a_channel, a_script, String.Empty, String.Empty, mCTSource.Token);
                if (executor.InitializeContext())
                {
                    executor.Broadcaster.Context = a_channel;
                    executor.Broadcaster.Broadcast += BroadcastListener;
                    executor.OnExecutionCompleted += Periodic_Executor_ExecutionCompleted;
                    executor.DelayExecution(a_script.Interval);
                }
            }
            else
            {
                OnError?.Invoke(a_channel, "Cannot periodically execute script without specified interval");
            }
        }

        /// <summary>
        /// Send a message over IRC
        /// </summary>
        /// <param name="a_channel">The channel the message should be sent to</param>
        /// <param name="a_message">The message to send</param>
        /// <param name="a_manual">True if the message was sent manually, false if it was issued by a script</param>
        public void SendMessage(TBChannel a_channel, string a_message, bool a_manual)
        {
            if (String.IsNullOrEmpty(a_channel.Name))
            {
                OnError?.Invoke(a_channel, "Cannot send to channel without name. Global pseudochannel can not receive messages.");
                return;
            }

            lock (mMessageSendLock)
            {
                if (!a_channel.Joined)
                {
                    OnError?.Invoke(a_channel, "Not sending message as channel has not been joined");
                    return;
                }

                // flood control
                var timestampThreshold = DateTime.Now - TimeSpan.FromSeconds(a_channel.Configuration.MaxMessageInterval);
                a_channel.SendTimestamps.RemoveAll((dt) => dt < timestampThreshold);
                if (a_channel.SendTimestamps.Count >= a_channel.Configuration.MaxMessageCount)
                {
                    // must delay to prevent flooding, which may result in the server rejecting further messages

                    // determine delay until we can send another message
                    var oldestTimestamp = a_channel.SendTimestamps.Min();
                    var passedTime = DateTime.Now - oldestTimestamp;
                    var delaySeconds = a_channel.Configuration.MaxMessageInterval - passedTime.Seconds + 1;
                    // start async task for delayed sending
                    DelaySendingMessage(a_channel, a_message, a_manual, delaySeconds);
                    return;
                }

                // send IRC message
                if ((mConnection != null) && mConnection.Connected)
                {
                    var ircMessage = new TIrcMessage(
                        a_channel.Configuration.Self,
                        TIrcCommand.PRIVMSG,
                        new List<string> { a_channel.IrcName },
                        a_message);
                    mConnection.Send(ircMessage);
                }
                else
                {
                    OnError?.Invoke(a_channel, "Not sending message as IRC is not connected");
                }

                // log message after sending
                OnChatMessage?.Invoke(
                    a_channel,
                    a_manual ? TBMessageDirection.MANUAL : TBMessageDirection.SENT,
                    a_channel.Configuration.Self,
                    a_message);
            }
        }

        /// <summary>
        /// Send a message over IRC with a delay
        /// </summary>
        /// <param name="a_channel">The channel the message should be sent to</param>
        /// <param name="a_message">The message to send</param>
        /// <param name="a_manual">True if the message was sent manually, false if it was issued by a script</param>
        /// <param name="a_delaySeconds">The delay in seconds</param>
        private void DelaySendingMessage(TBChannel a_channel, string a_message, bool a_manual, long a_delaySeconds)
        {
            _ = SendMessageDelayed(a_channel, a_message, a_manual, a_delaySeconds);
        }

        /// <summary>
        /// Send a message over IRC with a delay
        /// </summary>
        /// <param name="a_channel">The channel the message should be sent to</param>
        /// <param name="a_message">The message to send</param>
        /// <param name="a_manual">True if the message was sent manually, false if it was issued by a script</param>
        /// <param name="a_delaySeconds">The delay in seconds</param>
        private async Task SendMessageDelayed(TBChannel a_channel, string a_message, bool a_manual, long a_delaySeconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(a_delaySeconds), mCTSource.Token);
            SendMessage(a_channel, a_message, a_manual);
        }

        private void BroadcastListener(object a_context, TSBroadcaster.MessageType a_type, string a_message)
        {
            var channel = a_context as TBChannel;
            LogBroadcastMessage(channel ?? Global, a_type, a_message);
        }

        private void Executor_ExecutionCompleted(TBTaskedExecutor sender, TBChannel channel, TSCompiledScript script)
        {
            // remove callbacks
            sender.Broadcaster.Broadcast -= BroadcastListener;
            sender.OnExecutionCompleted -= Executor_ExecutionCompleted;
        }

        private void Periodic_Executor_ExecutionCompleted(TBTaskedExecutor sender, TBChannel channel, TSCompiledScript script)
        {
            // remove callbacks
            sender.Broadcaster.Broadcast -= BroadcastListener;
            sender.OnExecutionCompleted -= Executor_ExecutionCompleted;

            // check if channel is still joined
            if (channel.Joined)
            {
                // restart execution with configured delay on a new executor
                StartPeriodicScriptExecution(channel, script);
            }
        }

        #endregion

        #region Connection event handlers

        void Connection_OnConnected(object sender, string host)
        {
            OnNotice?.Invoke(Global, String.Format("Connected to {0}", host));

            if (Global.Configuration.TwitchCaps)
            {
                mConnection.RequestCapabilities(
                    // message tags w/ extra info like display names, emotes
                    "twitch.tv/tags",
                    //! membership: join/part for other users (periodic, cached)
                    // "twitch.tv/membership",
                    //! twitch-specific commands
                    "twitch.tv/commands"
                    );
            }

            mConnection.Login(Global.Configuration.Self, Global.Configuration.Auth);
        }

        void Connection_OnDisconnected(object sender, string host)
        {
            mConnectionStatus.IrcClientConnected = false;
            RaiseConnectionStatusChanged();

            OnNotice?.Invoke(Global, String.Format("Disconnected from {0}", host));
        }

        void Connection_OnError(object sender, TIrcConnection.IrcConnectionError error, string message)
        {
            OnError?.Invoke(Global, String.Format("{0}: {1}", error, message));

            switch (error)
            {
                case TIrcConnection.IrcConnectionError.ConnectionFailed:
                case TIrcConnection.IrcConnectionError.ConnectionLost:
                    Disconnect();
                    break;
                default:
                    break;
            }
        }

        private void Connection_OnInfo(object sender, string message)
        {
            OnNotice?.Invoke(Global, message);
        }

        void Connection_OnJoin(object sender, string channel, TIrcMessage.MessagePrefix nickname)
        {
            if (!nickname.IsNick
                || !nickname.NickName.Equals(Global.Configuration.Login, StringComparison.InvariantCultureIgnoreCase))
            {
                // join message for a different user
                return;
            }

            var message = String.Format("Joined channel {0}", channel);
            OnNotice?.Invoke(Global, message);

            if (Channels.TryGetValue(channel[1..], out TBChannel context))
            {
                // mark channel as joined
                context.Joined = true;

                // start executing periodic scripts
                StartPeriodicScriptExecution(context);

                // send hello message
                if (!String.IsNullOrWhiteSpace(context.Configuration.Hello))
                {
                    SendMessage(context, context.Configuration.Hello, false);
                }
            }
            else
            {
                OnWarning.Invoke(Global, String.Format("Received join for unknown channel {0}", channel));
            }
        }

        void Connection_OnLogin(object sender, string host, bool successful)
        {
            if (successful)
            {
                mConnectionStatus.IrcClientConnected = true;
                RaiseConnectionStatusChanged();

                OnNotice?.Invoke(Global, String.Format("Successfully logged in to {0}", host));

                foreach (var channel in Channels)
                {
                    if (channel.Value.Configuration.Join)
                    {
                        mConnection.Send(new TIrcMessage(TIrcCommand.JOIN, new List<string> { channel.Value.IrcName }, null));
                    }
                }
            }
            else
            {
                OnNotice?.Invoke(Global, String.Format("Login to {0} failed. Closing connection.", host));

                Disconnect();
            }
        }

        void Connection_OnMessage(object sender, TIrcConnection.IrcDirection a_direction, string a_channel,
            string a_nickname, string a_message, IReadOnlyDictionary<string, string> a_tags)
        {
            if (a_direction == TIrcConnection.IrcDirection.ServerToClient)
            {
                if (Channels.TryGetValue(a_channel[1..], out TBChannel context))
                {
                    HandleReceivedMessage(context, a_message, a_nickname, a_tags);
                }
                else
                {
                    OnWarning.Invoke(Global, String.Format("Received message for unknown channel {0} from {1}: {2}", a_channel, a_nickname, a_message));
                }
            }
        }

        void Connection_OnNotice(object sender, TIrcConnection.IrcDirection a_direction, string a_channel,
            string a_nickname, string a_message, IReadOnlyDictionary<string, string> a_tags)
        {
            if (a_direction == TIrcConnection.IrcDirection.ServerToClient)
            {
                if (Channels.TryGetValue(a_channel[1..], out TBChannel context))
                {
                    OnNotice?.Invoke(context, String.Format("NOTICE: {0}: {1}", a_nickname, a_message));
                }
                else
                {
                    OnWarning.Invoke(Global, String.Format("Received notice for unknown channel {0} from {1}: {2}", a_channel, a_nickname, a_message));
                }
            }
        }

        void Connection_OnPart(object sender, string channel, TIrcMessage.MessagePrefix nickname)
        {
            if (!nickname.IsNick
                || !nickname.NickName.Equals(Global.Configuration.Login, StringComparison.InvariantCultureIgnoreCase))
            {
                // part message for a different user
                return;
            }

            var message = String.Format("Parted from channel {0}", channel);
            OnNotice?.Invoke(Global, message);

            if (Channels.TryGetValue(channel[1..], out TBChannel context))
            {
                OnNotice?.Invoke(context, message);

                // mark channel as parted
                context.Joined = false;
            }
            else
            {
                OnWarning.Invoke(Global, String.Format("Received part for unknown channel {0}", channel));
            }
        }

        void Connection_OnPing(object sender, TIrcConnection.IrcDirection direction, string parameters)
        {
            OnInfo?.Invoke(
                Global,
                String.Format("PING [{0}]: {1}",
                    (direction == TIrcConnection.IrcDirection.ClientToServer) ? TBMessageDirection.SENT : TBMessageDirection.RECEIVED,
                    parameters));
        }

        void Connection_OnPong(object sender, TIrcConnection.IrcDirection direction, string parameters)
        {
            OnInfo?.Invoke(
                Global,
                String.Format("PONG [{0}]: {1}",
                    (direction == TIrcConnection.IrcDirection.ClientToServer) ? TBMessageDirection.SENT : TBMessageDirection.RECEIVED,
                    parameters));
        }
        void Connection_OnCapabilitiesGranted(object a_sender, string a_capabilities)
        {
            OnInfo?.Invoke(
                Global,
                String.Format("Capabilities have been granted: {0}", a_capabilities));
        }

        void Connection_OnRaw(object sender, TIrcConnection.IrcDirection direction, string rawmessage)
        {
            OnInfo?.Invoke(Global, String.Format("RAW[{0}]: {1}", direction, rawmessage));
        }

        private void WebSockets_OnMessageReceived(TWSServer a_sender, string a_message)
        {
            OnInfo?.Invoke(Global, String.Format("WSS received: {0}", a_message));
        }

        private void WebSockets_OnError(TWSServer a_sender, string a_message)
        {
            OnError?.Invoke(Global, String.Format("WSS error: {0}", a_message));
        }

        private void WebSockets_OnClientConnection(TWSServer a_sender, bool a_connected, int a_connectionCount)
        {
            mConnectionStatus.WebSocketClientCount = a_connectionCount;
            RaiseConnectionStatusChanged();

            OnInfo?.Invoke(Global, String.Format("WSS: Client connection {0} ({1} total)",
                a_connected ? "established" : "dropped",
                a_connectionCount));
        }

        #endregion
    }

    /// <summary>
    /// Chat message direction
    /// </summary>
    public enum TBMessageDirection
    {
        /// <summary>
        /// Message sent by bot
        /// </summary>
        SENT,
        /// <summary>
        /// Message received
        /// </summary>
        RECEIVED,
        /// <summary>
        /// Manually sent message
        /// </summary>
        MANUAL
    }
}
