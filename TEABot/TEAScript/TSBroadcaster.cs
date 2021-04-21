using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.TEAScript
{
    /// <summary>
    /// Used to broadcast messages which may be listened to by various observers.
    /// </summary>
    public class TSBroadcaster
    {
        /// <summary>
        /// Type of a broadcast message.
        /// </summary>
        public enum MessageType
        {
            /// <summary>
            /// Informational messages
            /// </summary>
            INFO,
            /// <summary>
            /// Error notifications
            /// </summary>
            ERROR,
            /// <summary>
            /// Warning notifications
            /// </summary>
            WARNING,
            /// <summary>
            /// Flush output buffer of script execution
            /// </summary>
            FLUSH
        }

        /// <summary>
        /// Listener methods receive messages
        /// </summary>
        /// <param name="a_context">The broadcaster context object, see Context</param>
        /// <param name="a_type">Message type</param>
        /// <param name="a_message">Message text</param>
        public delegate void Listener(object a_context, MessageType a_type, string a_message);

        /// <summary>
        /// Broadcast event will be raised when broadcasting messages
        /// </summary>
        public event Listener Broadcast;

        /// <summary>
        /// An arbitrary context object which will be passed along with the broadcast event
        /// </summary>
        public object Context { get; set; } = null;

        /// <summary>
        /// Broadcast a message
        /// </summary>
        /// <param name="a_type">The type of the message</param>
        /// <param name="a_messageFormat">The message to send</param>
        /// <param name="a_messageArguments">Optional arguments to pass to String.Format() with the message as the format</param>
        public void BroadcastMessage(MessageType a_type, string a_messageFormat, params object[] a_messageArguments)
        {
            Broadcast?.Invoke(Context, a_type, String.Format(a_messageFormat, a_messageArguments));
        }

        /// <summary>
        /// Broadcast an info message
        /// </summary>
        /// <param name="a_messageFormat">The message to send</param>
        /// <param name="a_messageArguments">Optional arguments to pass to String.Format() with the message as the format</param>
        public void Info(string a_messageFormat, params object[] a_messageArguments)
        {
            BroadcastMessage(MessageType.INFO, a_messageFormat, a_messageArguments);
        }

        /// <summary>
        /// Broadcast an error message
        /// </summary>
        /// <param name="a_messageFormat">The message to send</param>
        /// <param name="a_messageArguments">Optional arguments to pass to String.Format() with the message as the format</param>
        public void Error(string a_messageFormat, params object[] a_messageArguments)
        {
            BroadcastMessage(MessageType.ERROR, a_messageFormat, a_messageArguments);
        }

        /// <summary>
        /// Broadcast a warning message
        /// </summary>
        /// <param name="a_messageFormat">The message to send</param>
        /// <param name="a_messageArguments">Optional arguments to pass to String.Format() with the message as the format</param>
        public void Warn(string a_messageFormat, params object[] a_messageArguments)
        {
            BroadcastMessage(MessageType.WARNING, a_messageFormat, a_messageArguments);
        }

        /// <summary>
        /// Broadcast a flushing message
        /// </summary>
        /// <param name="a_messageFormat">The message to send</param>
        /// <param name="a_messageArguments">Optional arguments to pass to String.Format() with the message as the format</param>
        public void Flush(string a_messageFormat, params object[] a_messageArguments)
        {
            BroadcastMessage(MessageType.FLUSH, a_messageFormat, a_messageArguments);
        }
    }
}
