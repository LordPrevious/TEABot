using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TEABot.WebSocket
{
    /// <summary>
    /// Web socket server for sending events to browser source stream overlays and similar.
    /// </summary>
    class TWSServer
    {
        #region Constructors

        /// <summary>
        /// Create a new browser source server
        /// </summary>
        /// <param name="a_port">The port on which to listen for HTTP connections</param>
        public TWSServer(ushort a_port)
        {
            // copy arguments
            Port = a_port;

            // Specify a prefix to listen on
            mListener.Prefixes.Add(String.Format(@"http://localhost:{0}/", Port));
        }

        ~TWSServer()
        {
            Stop();
            mListener.Close();
        }

        #endregion

        #region Properties

        /// <summary>
        /// True if the server is running and serving a website
        /// </summary>
        public bool IsRunning { get { return mIsRunning; } }

        /// <summary>
        /// The port on which to listen for HTTP connections
        /// </summary>
        public ushort Port { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Start the server and listen for requests
        /// </summary>
        public void Start()
        {
            // check if already running
            if (mIsRunning) return;

            mIsRunning = true;

            // initialize cancellation token source for later async tasks
            mCTSource = new CancellationTokenSource();

            try
            {
                // start listening for HTTP requests
                mListener.Start();

                // listen for HTTP connections
                ListenHttp();
            }
            catch (Exception ex)
            {
                RaiseError(String.Format("Failed to start web socket server: {0}: {1}", ex.GetType(), ex.Message));
                mCTSource.Cancel();
                mCTSource.Dispose();
                mCTSource = null;
                mIsRunning = false;
            }
        }

        public void Stop()
        {
            // check if already stopped
            if (!mIsRunning) return;

            mIsRunning = false;

            // close web ockets
            lock (mConnectionLock)
            {
                foreach (var connection in mConnections)
                {
                    if (connection.Context.WebSocket.State == WebSocketState.Open)
                    {
                        var taskWebSocketClose = connection.Context.WebSocket.CloseOutputAsync(
                            WebSocketCloseStatus.NormalClosure, String.Empty, mCTSource.Token);
                        taskWebSocketClose.Wait();
                    }
                    connection.Context.WebSocket.Dispose();
                }
                mConnections.Clear();
                RaiseClientConnection(false, mConnections.Count);
            }

            // close HTTP listener
            mListener.Stop();

            // cancel all pending tasks
            mCTSource.Cancel();

            mCTSource.Dispose();
            mCTSource = null;
        }

        /// <summary>
        /// Send a message over the connected websockets
        /// </summary>
        /// <param name="a_message">The message to send</param>
        public void SendMessage(string a_message)
        {
            lock (mConnectionLock)
            {
                if (mConnections.Count > 0)
                {
                    var messageBytes = Encoding.UTF8.GetBytes(a_message);
                    var openConnections = mConnections.Where(c => c.Context.WebSocket.State == WebSocketState.Open);
                    foreach (var connection in openConnections)
                    {
                        connection.Context.WebSocket.SendAsync(
                            new ArraySegment<byte>(messageBytes),
                            WebSocketMessageType.Text,
                            true,
                            mCTSource.Token);
                    }
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event handler for received messages
        /// </summary>
        /// <param name="a_sender">The event sender</param>
        /// <param name="a_message">The received message</param>
        public delegate void MessageReceiveHandler(TWSServer a_sender, string a_message);
        /// <summary>
        /// Event raised when a message is received on the web socket
        /// </summary>
        public event MessageReceiveHandler OnMessageReceived;
        /// <summary>
        /// Raise an event that a message has been received
        /// </summary>
        /// <param name="a_message">The received message</param>
        private void RaiseMessageReceived(string a_message)
        {
            OnMessageReceived?.Invoke(this, a_message);
        }

        /// <summary>
        /// Handler signature for error events
        /// </summary>
        /// <param name="a_sender">Event sender</param>
        /// <param name="a_message">Error message</param>
        public delegate void ErrorHandler(TWSServer a_sender, string a_message);
        /// <summary>
        /// Event raised for errors
        /// </summary>
        public event ErrorHandler OnError;
        /// <summary>
        /// Raise OnError event
        /// </summary>
        /// <param name="a_message">Error message</param>
        private void RaiseError(string a_message)
        {
            OnError?.Invoke(this, a_message);
        }

        /// <summary>
        /// Handler signature for client connection events
        /// </summary>
        /// <param name="a_sender">Event sender</param>
        /// <param name="a_connected">True when a new connection has been established, false if one has been dropped</param>
        /// <param name="a_connectionCount">Current connection count</param>
        public delegate void ClientConnectionHandler(TWSServer a_sender, bool a_connected, int a_connectionCount);
        /// <summary>
        /// Event raised when a client connection has been established or dropped
        /// </summary>
        public event ClientConnectionHandler OnClientConnection;
        /// <summary>
        /// Raise OnClientConnection event
        /// </summary>
        /// <param name="a_connected">True when a new connection has been established, false if one has been dropped</param>
        /// <param name="a_connectionCount">Current connection count</param>
        private void RaiseClientConnection(bool a_connected, int a_connectionCount)
        {
            OnClientConnection?.Invoke(this, a_connected, a_connectionCount);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Listen for a HTTP connection
        /// </summary>
        private void ListenHttp()
        {
            var httpListenTask = mListener.GetContextAsync();
            httpListenTask.ContinueWith(FinishGetContext, mCTSource.Token);
        }

        /// <summary>
        /// Finish up after a completed task getting a HTTP listener context
        /// </summary>
        /// <param name="a_task">The task fetching the context</param>
        private void FinishGetContext(Task<HttpListenerContext> a_task)
        {
            // abort if task was canceled
            if (a_task.IsCanceled || !mIsRunning) return;
            // listen for the next connection
            ListenHttp();
            // continue processing if task succeeded
            if (!a_task.IsFaulted)
            {
                HandleListenerContext(a_task.Result);
            }
        }

        /// <summary>
        /// Handle a received HTTP listener context
        /// </summary>
        /// <param name="a_context">The context with an incoming connection / request</param>
        private void HandleListenerContext(HttpListenerContext a_context)
        {
            if (null == a_context) return;

            // check if still running
            if (!mIsRunning)
            {
                // respond with error
                a_context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                a_context.Response.OutputStream.Close();
            }

            // check for web socket requests
            if (a_context.Request.IsWebSocketRequest)
            {
                AcceptWebSocket(a_context);
                return;
            }

            // We don't serve anything else
            a_context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            a_context.Response.OutputStream.Close();
        }

        /// <summary>
        /// Accept an incomping web socket connection request
        /// </summary>
        /// <param name="a_context">The received request</param>
        private void AcceptWebSocket(HttpListenerContext a_context)
        {
            if (null == a_context) return;

            try
            {
                var webSocketAcceptTask = a_context.AcceptWebSocketAsync(null);
                webSocketAcceptTask.ContinueWith(FinishAcceptWebSocket, mCTSource.Token);
            }
            catch
            {
                // respond with error
                a_context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                a_context.Response.OutputStream.Close();
            }
        }

        /// <summary>
        /// Finish up after a completed task getting a web socket context
        /// </summary>
        /// <param name="a_task">The task fetching the context</param>
        private void FinishAcceptWebSocket(Task<HttpListenerWebSocketContext> a_task)
        {
            // abort if task was canceled
            if (a_task.IsCanceled || !mIsRunning) return;
            // continue processing if task succeeded
            if (!a_task.IsFaulted)
            {
                // initialize connection
                WebSocketConnection connection = new(a_task.Result);
                // add to list
                lock (mConnectionLock)
                {
                    mConnections.Add(connection);
                    RaiseClientConnection(true, mConnections.Count);
                }
                // start listening
                ListenWebSocket(connection);
            }
        }

        /// <summary>
        /// Listen on a web socket
        /// </summary>
        private void ListenWebSocket(WebSocketConnection a_connection)
        {
            if (a_connection.Context.WebSocket.State == WebSocketState.Open)
            {
                var receiveTask = a_connection.Context.WebSocket.ReceiveAsync(
                    new ArraySegment<byte>(a_connection.ReceiveBuffer),
                    mCTSource.Token);
                receiveTask.ContinueWith(FinishWebSocketReceive, a_connection, mCTSource.Token);
            }
            else
            {
                // remove from list
                lock (mConnectionLock)
                {
                    a_connection.Context.WebSocket.Dispose();
                    mConnections.Remove(a_connection);
                    RaiseClientConnection(false, mConnections.Count);
                }
            }
        }

        /// <summary>
        /// Finish up after a completed task receiving data from a web socket
        /// </summary>
        /// <param name="a_task">The task receiving data</param>
        /// <param name="a_state">Associated web socket connection, should be a WebSocketConnection</param>
        private void FinishWebSocketReceive(Task<WebSocketReceiveResult> a_task, object a_state)
        {
            // state must be the associated connection
            if (a_state is not WebSocketConnection connection) return;
            // check if task was successful
            if (mIsRunning && !a_task.IsCanceled && !a_task.IsFaulted)
            {
                // convert received bytes to string, remove null termination
                try
                {
                    var receivedMessage = Encoding.UTF8.GetString(connection.ReceiveBuffer).TrimEnd('\0');
                    RaiseMessageReceived(receivedMessage);
                }
                catch
                {
                    // broken message, discard
                }
                // continue receiving further data
                if (mIsRunning)
                {
                    ListenWebSocket(connection);
                }
            }
            else
            {
                // remove broken connection from list
                lock (mConnectionLock)
                {
                    connection.Context.WebSocket.Dispose();
                    mConnections.Remove(connection);
                    RaiseClientConnection(false, mConnections.Count);
                }
            }
        }

        #endregion

        #region Private data

        /// <summary>
        /// Track whether the server is running or not
        /// </summary>
        private bool mIsRunning = false;

        /// <summary>
        /// Cancellation token source to stop async processes when shutting down
        /// </summary>
        private CancellationTokenSource mCTSource = null;

        /// <summary>
        /// HTTP listener for website requests
        /// </summary>
        private readonly HttpListener mListener = new();

        /// <summary>
        /// Lock object for thread-safe access to mConnections
        /// </summary>
        private readonly object mConnectionLock = new();

        /// <summary>
        /// List of open web socket connections
        /// </summary>
        private readonly List<WebSocketConnection> mConnections = new();

        #endregion

        #region Internal structures

        /// <summary>
        /// Data required per web socket connection
        /// </summary>
        private class WebSocketConnection
        {
            public HttpListenerWebSocketContext Context { get; private set; }

            public byte[] ReceiveBuffer = new byte[2048];

            public WebSocketConnection(HttpListenerWebSocketContext a_context)
            {
                Context = a_context;
            }
        }

        #endregion
    }
}
