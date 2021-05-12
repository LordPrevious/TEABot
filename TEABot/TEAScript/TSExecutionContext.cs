using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.TEAScript
{
    /// <summary>
    /// Context and state information of a script execution
    /// </summary>
    public class TSExecutionContext
    {
        /// <summary>
        /// The output buffer to write to
        /// </summary>
        public StringBuilder Output { get; } = new StringBuilder();

        /// <summary>
        /// Current values, such as
        /// - metadata (prefix '$')
        /// - script arguments (prefix '?')
        /// - variables (prefix '!')
        /// </summary>
        public TSValueDictionary Values { get; } = new TSValueDictionary();

        /// <summary>
        /// A storage provider for data persistence.
        /// </summary>
        public ITSStorage Storage { get; set; } = null;

        /// <summary>
        /// A broadcaster to send execution messages to.
        /// </summary>
        public TSBroadcaster Broadcaster { get; set; } = null;

        /// <summary>
        /// Flush the output buffer and reset it to empty.
        /// </summary>
        public void Flush()
        {
            var currentOutput = Output.ToString();
            if (!String.IsNullOrWhiteSpace(currentOutput))
            {
                Broadcaster?.Flush(currentOutput);
            }
            Output.Clear();
        }
    }
}
