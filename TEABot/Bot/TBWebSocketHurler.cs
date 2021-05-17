using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TEABot.TEAScript;
using TEABot.WebSocket;

namespace TEABot.Bot
{
    /// <summary>
    /// Hurler whuch forwards data as JSON to all connected web sockets
    /// </summary>
    class TBWebSocketHurler : ITSHurler
    {
        #region Public data

        /// <summary>
        /// A broadcaster used for warnings and errors.
        /// </summary>
        public readonly TSBroadcaster Broadcaster = new();

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new web socket hurler
        /// </summary>
        /// <param name="a_webSockets">The web sockets over which to send hurled data</param>
        public TBWebSocketHurler(TWSServer a_webSockets)
        {
            mWebSockets = a_webSockets ?? throw new ArgumentNullException(nameof(a_webSockets));
        }

        #endregion

        #region Private data

        /// <summary>
        /// Web socket server to send hurled JSON over
        /// </summary>
        private readonly TWSServer mWebSockets;

        #endregion

        #region ITSHurler implementation

        public bool Hurl(Dictionary<string, TSValue> a_projectile)
        {
            // prepare data for JSON conversion
            Dictionary<string, object> jsonData = new();
            foreach (var kvp in a_projectile)
            {
                if (kvp.Value.IsText)
                {
                    jsonData[kvp.Key] = kvp.Value.TextValue;
                }
                else
                {
                    jsonData[kvp.Key] = kvp.Value.NumericalValue;
                }
            }
            // convert to JSON and send via websockets
            try
            {
                mWebSockets.SendMessage(JsonSerializer.Serialize(jsonData));
            }
            catch (Exception ex)
            {
                Broadcaster.Error("Failed to hurl JSON data due to {0}: {1}", ex.GetType(), ex.Message);
                return false;
            }

            return true;
        }

        #endregion
    }
}
