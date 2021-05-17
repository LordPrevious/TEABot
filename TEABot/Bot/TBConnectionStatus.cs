using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.Bot
{
    /// <summary>
    /// Bot connection status information
    /// </summary>
    public struct TBConnectionStatus
    {
        /// <summary>
        /// True iff the IRC client is running and the bot is trying to connect to the IRC host
        /// </summary>
        public bool IrcClientRunning;
        /// <summary>
        /// True iff the bot is connected to the IRC host
        /// </summary>
        public bool IrcClientConnected;
        /// <summary>
        /// True iff the WebSocket Server is running and listening for client connections
        /// </summary>
        public bool WebSocketServerRunning;
        /// <summary>
        /// Number of clients connected to the WebSocket Server
        /// </summary>
        public int WebSocketClientCount;
    }
}
