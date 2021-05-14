using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TEABot.IRC
{
    /// <summary>
    /// IRC server connection establishing and message translating
    /// </summary>
    class TIrcConnection
    {
        #region Constructors

        /// <summary>
        /// Initialize an IRC connection handler
        /// </summary>
        public TIrcConnection()
        {
            mSendBuffer = new byte[cBufferSize];
            mReceiveBuffer = new byte[cBufferSize];

            mReceivedData = String.Empty;
        }

        #endregion

        #region Private data

        /// <summary>
        /// TCP client to connect to the IRC server
        /// </summary>
        private readonly TcpClient mTcpClient = new();
        /// <summary>
        /// Input/output stream for server communications,
        /// points to either the TCP client's network stream directly, or to the SSL encryption stream
        /// </summary>
        private Stream mStream = null;
        /// <summary>
        /// SSL encryption stream to wrap the TCP client's network stream for SSL/TLS support
        /// </summary>
        private SslStream mSslStream = null;
        /// <summary>
        /// Buffer for data to send to the server
        /// </summary>
        private readonly byte[] mSendBuffer;
        /// <summary>
        /// Buffer for data received from the server
        /// </summary>
        private readonly byte[] mReceiveBuffer;
        /// <summary>
        /// Decoded received data
        /// </summary>
        private string mReceivedData;

        /// <summary>
        /// Cancellation token for async tasks
        /// </summary>
        private CancellationToken mCt;

        /// <summary>
        /// IRC server hostname
        /// </summary>
        private string mHostname;
        /// <summary>
        /// IRC server port
        /// </summary>
        private ushort mPort;
        /// <summary>
        /// TRUE to enable SSL/TLS encryption for the IRC server connection
        /// </summary>
        private bool mUseSSL;
        /// <summary>
        /// TRUE when a connection to the IRC server has been established
        /// </summary>
        private bool mConnected;
        /// <summary>
        /// TRUE while establishing a connection to the IRC server
        /// </summary>
        private bool mConnecting;

        /// <summary>
        /// Delimiter for IRC messages
        /// </summary>
        private static readonly string cMessageDelimiter = "\r\n";
        /// <summary>
        /// Delimiter in a string array as that's what some of those string functions take
        /// </summary>
        private static readonly string[] cMessageDelimiterArray = new string[] { cMessageDelimiter };

        /// <summary>
        /// Size for send and receive buffers in bytes
        /// </summary>
        private static readonly int cBufferSize = 1024;

        #endregion

        #region Private functions

        /// <summary>
        /// Check if we got a connection to the IRC server
        /// </summary>
        /// <returns>TRUE if a connection to the IRC server has been established</returns>
        private bool CheckConnected()
        {
            if (!mConnected)
                return false;

            if ((mTcpClient == null)
                || (!mTcpClient.Connected))
            {
                mConnected = false;
                RaiseError(IrcConnectionError.ConnectionLost, "Lost connection.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Process a received IRC message
        /// </summary>
        /// <param name="a_rawMessage">The raw IRC message string</param>
        private void ProcessIncomingMessage(string a_rawMessage)
        {
            var message = TIrcMessage.Parse(a_rawMessage);

            // check for successful parsing
            if (message == null)
            {
                RaiseError(IrcConnectionError.ParsingFailed, a_rawMessage);
                return;
            }

            var command = message.Command;

            if (command.IsResponse)
            {
                switch (command.Response)
                {
                    case TIrcResponseCode.RPL_WELCOME:
                        // TODO: Stop timeout / end login phase
                        RaiseLogin(mHostname, true);
                        break;
                }
            }
            else if (command.IsCommand)
            {
                switch (command.Command)
                {
                    case TIrcCommand.JOIN:
                        // user joined a channel
                        if ((message.Params != null) && (message.Params.Middles.Count > 0))
                        {
                            RaiseJoin(message.Params.Middles[0], message.Prefix);
                        }
                        break;
                    case TIrcCommand.PART:
                        // user disconnect from channel
                        if ((message.Params != null) && (message.Params.Middles.Count > 0))
                        {
                            RaisePart(message.Params.Middles[0], message.Prefix);
                        }
                        break;
                    case TIrcCommand.PRIVMSG:
                        // regular chat message
                        if ((message.Params != null) && (message.Params.Middles.Count >= 1))
                        {
                            RaiseMessage(IrcDirection.ServerToClient, message.Params.Middles[0],
                                message.Prefix,
                                String.Join(" ", message.Params.Middles.Skip(1).Append(message.Params.Trailing)));
                        }
                        break;
                    case TIrcCommand.NOTICE:
                        // Notice message reported by server
                        if ((message.Params != null) && (message.Params.Middles.Count >= 1))
                        {
                            RaiseNotice(IrcDirection.ServerToClient, message.Params.Middles[0],
                                message.Prefix,
                                String.Join(" ", message.Params.Middles.Skip(1).Append(message.Params.Trailing)));
                        }
                        break;
                    case TIrcCommand.ERROR:
                        // Error reported by server
                        RaiseError(IrcConnectionError.ServerError, (message.Params != null) ? message.Params.ToString() : String.Empty);
                        break;
                    case TIrcCommand.PING:
                        // PING request / connection keep-alive
                        if (message.Params != null)
                        {
                            RaisePing(IrcDirection.ServerToClient,
                                String.Join(" ", message.Params.Middles.Append(message.Params.Trailing)));
                            // send the same params back
                            Send(new TIrcMessage(TIrcCommand.PONG, message.Params.Middles, message.Params.Trailing));
                        }
                        break;
                    case TIrcCommand.PONG:
                        // PING response
                        if (message.Params != null)
                        {
                            RaisePong(IrcDirection.ServerToClient,
                            String.Join(" ", message.Params.Middles.Append(message.Params.Trailing)));
                        }
                        break;
                    case TIrcCommand.CAP:
                        // IRC extension request confirmation
                        if ((message.Params != null)
                            && (message.Params.Middles.Count >= 2)
                            && (message.Params.Middles[1] == "ACK"))
                        {
                            RaiseCapabilitiesGranted(message.Params.Trailing);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Process an outgoing IRC message for logging purposes
        /// </summary>
        /// <param name="a_message">The message that is being sent to the IRC server</param>
        private void ProcessOutgoingMessage(TIrcMessage a_message)
        {
            var command = a_message.Command;

            if (command.IsCommand)
            {
                switch (command.Command)
                {
                    // regular chat message
                    case TIrcCommand.PRIVMSG:
                        if ((a_message.Params != null) && (a_message.Params.Middles.Count >= 1))
                        {
                            RaiseMessage(IrcDirection.ClientToServer, a_message.Params.Middles[0],
                                a_message.Prefix,
                                String.Join(" ", a_message.Params.Middles.Skip(1).Append(a_message.Params.Trailing)));
                        }
                        break;
                    // notice message sent to the server
                    case TIrcCommand.NOTICE:
                        if ((a_message.Params != null) && (a_message.Params.Middles.Count >= 1))
                        {
                            RaiseNotice(IrcDirection.ClientToServer, a_message.Params.Middles[0],
                                a_message.Prefix,
                                String.Join(" ", a_message.Params.Middles.Skip(1).Append(a_message.Params.Trailing)));
                        }
                        break;
                    // PING sent to the server for connection keep-alive
                    case TIrcCommand.PING:
                        if (a_message.Params != null)
                        {
                            RaisePing(IrcDirection.ClientToServer,
                            String.Join(" ", a_message.Params.Middles.Append(a_message.Params.Trailing)));
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Continues after async connect task completed
        /// </summary>
        /// <param name="a_connectTask">The connect task</param>
        private void FinishConnect(Task a_connectTask)
        {
            if (a_connectTask.IsCanceled)
            {
                mConnecting = false;
                return;
            }

            if (mTcpClient.Connected)
            {
                // set up stream
                if (mUseSSL)
                {
                    try
                    {
                        mSslStream = new SslStream(mTcpClient.GetStream(), false,
                            new RemoteCertificateValidationCallback(ValidateServerCerts_AcceptAll), null);
                        var taskAuthenticate = mSslStream.AuthenticateAsClientAsync(mHostname);
                        taskAuthenticate.ContinueWith(FinishSslAuthentication);
                    }
                    catch
                    {
                        RaiseError(IrcConnectionError.ConnectionFailed, "Failed SSL setup.");
                    }
                }
                else
                {
                    mStream = mTcpClient.GetStream();
                    mConnecting = false;
                    mConnected = true;
                    RaiseConnected(String.Format("{0}:{1}", mHostname, mPort));
                    BeginReceive();
                }
            }
            else
            {
                mConnecting = false;
                RaiseError(IrcConnectionError.ConnectionFailed, "Failed to connect.");
            }
        }

        /// <summary>
        /// Server cert validation that accepts any certificate
        /// </summary>
        /// <param name="a_sender">Requester</param>
        /// <param name="a_cert">Certificate to validate</param>
        /// <param name="a_chain">Certificate chain</param>
        /// <param name="a_errors">Possible SSL issues</param>
        /// <returns>true to accept the cert</returns>
        private static bool ValidateServerCerts_AcceptAll(object a_sender, X509Certificate a_cert, X509Chain a_chain, SslPolicyErrors a_errors)
        {
            return true;
        }

        /// <summary>
        /// Continues after async authentication task has completed
        /// </summary>
        /// <param name="a_AuthenticationTask">The authentication task</param>
        private void FinishSslAuthentication(Task a_AuthenticationTask)
        {
            mConnecting = false;

            if (a_AuthenticationTask.IsCanceled) return;

            if (mSslStream.IsAuthenticated
                && mSslStream.IsEncrypted
                && mSslStream.IsSigned)
            {
                RaiseInfo("SSL connection has been authenticated.");
                mStream = mSslStream;
                mConnected = true;
                RaiseConnected(String.Format("{0}:{1}", mHostname, mPort));
                BeginReceive();
            }
            else
            {
                mTcpClient.Close();
                RaiseError(IrcConnectionError.ConnectionFailed, "Failed SSL authentication.");
            }
        }

        /// <summary>
        /// Continues after an async task to send raw data has completed
        /// </summary>
        /// <param name="a_writeTask">The task that has completed</param>
        /// <param name="a_state">The message string that has been sent</param>
        private void FinishSendRaw(Task a_writeTask, object a_state)
        {
            if (!Connected || a_writeTask.IsCanceled) return;

            var rawMsg = (String)a_state;

            if (rawMsg == null)
            {
                RaiseError(IrcConnectionError.SendingFailed, "*Unknown raw message*");
                return;
            }

            if (a_writeTask.Status != TaskStatus.RanToCompletion)
            {
                RaiseError(IrcConnectionError.SendingFailed, rawMsg);
                return;
            }

            RaiseRaw(IrcDirection.ClientToServer, rawMsg);
        }

        /// <summary>
        /// Continues after an async task to receive data has completed
        /// </summary>
        /// <param name="a_receiveTask">The task that has completed</param>
        private void FinishReceive(Task<int> a_receiveTask)
        {
            if (!Connected || a_receiveTask.IsCanceled) return;

            var length = a_receiveTask.Result;

            if (length > 0)
            {
                var newData = Encoding.UTF8.GetString(mReceiveBuffer, 0, length);

                /* received data doesn't necessarily split at new lines, but the IRC protocol
                 * uses one line per message. Thus, we split by new lines (CRLF), and remember unfinished
                 * messages so we can process them once we receive the rest */

                var fullData = mReceivedData + newData;
                var splitData = fullData.Split(cMessageDelimiterArray, StringSplitOptions.RemoveEmptyEntries);

                var hasHalfMessage = !(fullData.EndsWith(cMessageDelimiter));

                var breakAt = (hasHalfMessage ? (splitData.Length - 1) : splitData.Length);
                if (splitData.Length > 0)
                {
                    for (var i = 0; i < breakAt; i++)
                    {
                        var message = splitData[i];

                        // raise before processing, so automatic responses won't be logged before the incoming message
                        RaiseRaw(IrcDirection.ServerToClient, message);

                        ProcessIncomingMessage(message);
                    }
                }

                if (hasHalfMessage)
                {
                    mReceivedData = splitData.Last();
                }
                else
                {
                    mReceivedData = String.Empty;
                }
            }

            BeginReceive();
        }

        /// <summary>
        /// Start an async task to receive data
        /// </summary>
        private void BeginReceive()
        {
            try
            {
                var taskRead = mStream.ReadAsync(mReceiveBuffer, 0, mReceiveBuffer.Length, mCt);
                taskRead.ContinueWith(FinishReceive);
            }
            catch (SocketException ex)
            {
                mConnected = false;
                RaiseError(IrcConnectionError.SendingFailed, String.Format("{0}: {1}", ex.GetType(), ex.Message));
            }
        }

        #endregion

        #region Public functions

        /// <summary>
        /// Begin connecting to an IRC server
        /// </summary>
        /// <param name="a_hostname">Hostname of the server</param>
        /// <param name="a_port">Server port</param>
        /// <param name="a_useSSL">Whether to use SSL/TLS encryption</param>
        /// <param name="a_ct">Cancellation token for async tasks</param>
        public void Connect(string a_hostname, ushort a_port, bool a_useSSL, CancellationToken a_ct)
        {
            if (mConnecting || Connected) return;

            mConnecting = true;

            mHostname = a_hostname;
            mPort = a_port;
            mUseSSL = a_useSSL;
            mCt = a_ct;

            try
            {
                var taskTcpConnect = mTcpClient.ConnectAsync(mHostname, mPort, mCt);
                taskTcpConnect.AsTask().ContinueWith(FinishConnect, mCt);
            }
            catch
            {
                mConnected = false;
                RaiseError(IrcConnectionError.ConnectionFailed, "Unable to connect.");
                mConnecting = false;
            }
        }

        /// <summary>
        /// Disconnect from the IRC server
        /// </summary>
        public void Disconnect()
        {
            mConnected = mConnecting = false;
            mTcpClient.Close();

            RaiseDisconnected(mHostname);
        }

        /// <summary>
        /// Send a login request to the IRC server
        /// </summary>
        /// <param name="a_username">Login username</param>
        /// <param name="a_password">Login password</param>
        public void Login(string a_username, string a_password)
        {
            if (!Connected) return;

            SendRaw(String.Format("PASS {0}{2}USER {1}{2}NICK {1}", a_password, a_username, cMessageDelimiter));

            // TODO: Timeout/fail
        }

        /// <summary>
        /// Join an IRC channel
        /// </summary>
        /// <param name="a_channel">Name of the channel, include "#" where appropriate</param>
        public void Join(string a_channel)
        {
            if (!Connected) return;

            SendRaw(String.Format("JOIN {0}", a_channel));
        }

        /// <summary>
        /// Part from an IRC channel
        /// </summary>
        /// <param name="a_channel">Name of the channel, include "#" where appropriate</param>
        public void Part(string a_channel)
        {
            if (!Connected) return;

            SendRaw(String.Format("PART {0}", a_channel));
        }

        /// <summary>
        /// Request IRC extension capabilities
        /// </summary>
        /// <param name="a_capabilities">List of capabilities to request</param>
        public void RequestCapabilities(params string[] a_capabilities)
        {
            if (!Connected || (a_capabilities.Length <= 0)) return;

            SendRaw(String.Format("CAP REQ :{0}", String.Join(" ", a_capabilities)));
        }

        /// <summary>
        /// Send a raw message to the IRC server
        /// </summary>
        /// <param name="a_rawMessage">The message to send as-is</param>
        public void SendRaw(string a_rawMessage)
        {
            if (!a_rawMessage.EndsWith(cMessageDelimiter))
            {
                SendRaw(a_rawMessage + cMessageDelimiter);
                return;
            }

            if (a_rawMessage.Length > cBufferSize)
            {
                RaiseError(IrcConnectionError.MessageTooLong, a_rawMessage);
                return;
            }

            var lenght = Encoding.UTF8.GetBytes(a_rawMessage, 0, a_rawMessage.Length, mSendBuffer, 0);

            var taskWrite = mStream.WriteAsync(mSendBuffer, 0, lenght, mCt);
            taskWrite.ContinueWith(FinishSendRaw, a_rawMessage);
        }

        /// <summary>
        /// Send an IRC message to the server
        /// </summary>
        /// <param name="a_message">The message to send</param>
        public void Send(TIrcMessage a_message)
        {
            SendRaw(a_message.ToString());

            ProcessOutgoingMessage(a_message);
        }

        #endregion

        #region Properties

        /// <summary>
        /// TRUE if a connection to the IRC server has been established
        /// </summary>
        public bool Connected
        {
            get
            {
                return CheckConnected();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handler signature for raw message events
        /// </summary>
        /// <param name="a_sender">Event sender</param>
        /// <param name="a_direction">Message direction</param>
        /// <param name="a_rawmessage">Raw message which was sent or received</param>
        public delegate void RawMessageHandler(object a_sender, IrcDirection a_direction, string a_rawmessage);
        /// <summary>
        /// Event raised for raw messages, both sent and received
        /// </summary>
        public event RawMessageHandler OnRaw;
        /// <summary>
        /// Raise OnRaw event
        /// </summary>
        /// <param name="a_direction">Message direction</param>
        /// <param name="a_rawmessage">Raw message which was sent or received</param>
        private void RaiseRaw(IrcDirection a_direction, string a_rawMessage)
        {
            OnRaw?.Invoke(this, a_direction, a_rawMessage);
        }

        /// <summary>
        /// Handler signature for IRC message events
        /// </summary>
        /// <param name="a_sender">Event sender</param>
        /// <param name="a_direction">Message direction</param>
        /// <param name="a_channel">Name of associated channel</param>
        /// <param name="a_nickname">Nickname of sender</param>
        /// <param name="a_message">Message text</param>
        public delegate void MessageHandler(object a_sender, IrcDirection a_direction, string a_channel,
            TIrcMessage.MessagePrefix a_nickname, string a_message);
        /// <summary>
        /// Event raised for regular chat messages, both sent and received
        /// </summary>
        public event MessageHandler OnMessage;
        /// <summary>
        /// Raise onMessage event
        /// </summary>
        /// <param name="a_direction">Message direction</param>
        /// <param name="a_channel">Name of associated channel</param>
        /// <param name="a_nickname">Nickname of sender</param>
        /// <param name="a_message">Message text</param>
        private void RaiseMessage(IrcDirection a_direction, string a_channel, TIrcMessage.MessagePrefix a_nickname, string a_message)
        {
            OnMessage?.Invoke(this, a_direction, a_channel, a_nickname, a_message);
        }
        /// <summary>
        /// Event raised for notice messages, both sent and received
        /// </summary>
        public event MessageHandler OnNotice;
        /// <summary>
        /// Raise OnNotice event
        /// </summary>
        /// <param name="a_direction">Message direction</param>
        /// <param name="a_channel">Name of associated channel</param>
        /// <param name="a_nickname">Nickname of sender</param>
        /// <param name="a_message">Message text</param>
        private void RaiseNotice(IrcDirection a_direction, string a_channel, TIrcMessage.MessagePrefix a_nickname, string a_message)
        {
            OnNotice?.Invoke(this, a_direction, a_channel, a_nickname, a_message);
        }

        /// <summary>
        /// Handler signature for error events
        /// </summary>
        /// <param name="a_sender">Event sender</param>
        /// <param name="a_error">Error value</param>
        /// <param name="a_message">Error message</param>
        public delegate void IrcErrorHandler(object a_sender, IrcConnectionError a_error, string a_message);
        /// <summary>
        /// Event raised for errors
        /// </summary>
        public event IrcErrorHandler OnError;
        /// <summary>
        /// Raise OnError event
        /// </summary>
        /// <param name="a_error">Error value</param>
        /// <param name="a_message">Error message</param>
        private void RaiseError(IrcConnectionError a_error, string a_message)
        {
            OnError?.Invoke(this, a_error, a_message);
        }

        /// <summary>
        /// Handler signature for info messages
        /// </summary>
        /// <param name="a_sender">Event sender</param>
        /// <param name="a_message">Info message</param>
        public delegate void IrcInfoHandler(object a_sender, string a_message);
        /// <summary>
        /// Event raised for misc info messages
        /// </summary>
        public event IrcInfoHandler OnInfo;
        /// <summary>
        /// Raise OnInfo event
        /// </summary>
        /// <param name="a_message">Info message</param>
        private void RaiseInfo(string a_message)
        {
            OnInfo?.Invoke(this, a_message);
        }

        /// <summary>
        /// Handler signature for connection events
        /// </summary>
        /// <param name="a_sender">Event sender</param>
        /// <param name="a_host">Host for which a connection has been established or separated</param>
        public delegate void ConnectedHandler(object a_sender, string a_host);
        /// <summary>
        /// Event raised when a connection has been established
        /// </summary>
        public event ConnectedHandler OnConnected;
        /// <summary>
        /// Raise OnConnected event
        /// </summary>
        /// <param name="a_host">Host to which a connection has been established</param>
        private void RaiseConnected(string a_host)
        {
            OnConnected?.Invoke(this, a_host);
        }
        /// <summary>
        /// Event raised when a connection has been separated
        /// </summary>
        public event ConnectedHandler OnDisconnected;
        /// <summary>
        /// Raise OnDisconnected event
        /// </summary>
        /// <param name="a_host">Host from which a connection has been separated</param>
        private void RaiseDisconnected(string a_host)
        {
            OnDisconnected?.Invoke(this, a_host);
        }

        /// <summary>
        /// Handler signature for IRC login events
        /// </summary>
        /// <param name="a_sender">Event sender</param>
        /// <param name="a_host">Host for which the login has been performed</param>
        /// <param name="a_successful">Whether the login succeeded or not</param>
        public delegate void LoginHandler(object a_sender, string a_host, bool a_successful);
        /// <summary>
        /// Event raised after IRC login
        /// </summary>
        public event LoginHandler OnLogin;
        /// <summary>
        /// Raise OnLogin event
        /// </summary>
        /// <param name="a_host">Host for which the login has been performed</param>
        /// <param name="a_successful">Whether the login succeeded or not</param>
        private void RaiseLogin(string a_host, bool a_success)
        {
            OnLogin?.Invoke(this, a_host, a_success);
        }

        /// <summary>
        /// Handler signature for IRC join and part events
        /// </summary>
        /// <param name="a_sender">Event sender</param>
        /// <param name="a_channel">Channel which has been joined or parted</param>
        /// <param name="a_nickname">Nickname of the user who joined or parted</param>
        public delegate void JoinPartHandler(object a_sender, string a_channel, TIrcMessage.MessagePrefix a_nickname);
        /// <summary>
        /// Event raised when a user joined an IRC channel
        /// </summary>
        public event JoinPartHandler OnJoin;
        /// <summary>
        /// Raise OnJoin event
        /// </summary>
        /// <param name="a_channel">Channel which has been joined</param>
        /// <param name="a_nickname">Nickname of the user who joined</param>
        private void RaiseJoin(string a_channel, TIrcMessage.MessagePrefix a_nickname)
        {
            OnJoin?.Invoke(this, a_channel, a_nickname);
        }
        /// <summary>
        /// Event raised when a user parted from an IRC channel
        /// </summary>
        public event JoinPartHandler OnPart;
        /// <summary>
        /// Raise OnPart event
        /// </summary>
        /// <param name="a_channel">Channel from which was parted</param>
        /// <param name="a_nickname">Nickname of the user who parted</param>
        private void RaisePart(string a_channel, TIrcMessage.MessagePrefix a_nickname)
        {
            OnPart?.Invoke(this, a_channel, a_nickname);
        }

        /// <summary>
        /// Handler signature for PING and PONG events
        /// </summary>
        /// <param name="a_sender">Event sender</param>
        /// <param name="a_direction">Message direction</param>
        /// <param name="a_parameters">Message parameters</param>
        public delegate void PingPongHandler(object a_sender, IrcDirection a_direction, string a_parameters);
        /// <summary>
        /// Event raised for PING messages
        /// </summary>
        public event PingPongHandler OnPing;
        /// <summary>
        /// Raise OnPing event
        /// </summary>
        /// <param name="a_direction">Message direction</param>
        /// <param name="a_parameters">Message parameters</param>
        private void RaisePing(IrcDirection a_direction, string a_parameters)
        {
            OnPing?.Invoke(this, a_direction, a_parameters);
        }
        /// <summary>
        /// Event raised for PONG messages
        /// </summary>
        public event PingPongHandler OnPong;
        /// <summary>
        /// Raise OnPong event
        /// </summary>
        /// <param name="a_direction">Message direction</param>
        /// <param name="a_parameters">Message parameters</param>
        private void RaisePong(IrcDirection a_direction, string a_parameters)
        {
            OnPong?.Invoke(this, a_direction, a_parameters);
        }

        /// <summary>
        /// Handler signature for capability events
        /// </summary>
        /// <param name="a_sender">Event sender</param>
        /// <param name="a_capabilities">Capabilities which have been granted, possibly multiple separated by space</param>
        public delegate void CapabilitiesGrantedHandler(object a_sender, string a_capabilities);
        /// <summary>
        /// Event raised for capability granted messages
        /// </summary>
        public event CapabilitiesGrantedHandler OnCapabilitiesGranted;
        /// <summary>
        /// Raise OnCapabilitiesGranted event
        /// </summary>
        /// <param name="a_capabilities">Capabilities which have been granted, possibly multiple separated by space</param>
        private void RaiseCapabilitiesGranted(string a_capabilities)
        {
            OnCapabilitiesGranted?.Invoke(this, a_capabilities);
        }

        #endregion

        # region Convenience enums

        /// <summary>
        /// IRC error values
        /// </summary>
        public enum IrcConnectionError
        {
            ConnectionFailed,
            ConnectionLost,
            LoginFailed,
            MessageTooLong,
            ParsingFailed,
            SendingFailed,
            ReceivingFailed,
            ServerError,
        }

        public enum IrcDirection
        {
            ServerToClient,
            ClientToServer,
            None,
        }

        #endregion
    }
}
