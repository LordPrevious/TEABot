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
        /// A hurling provider for sending via a secondary communication channel, e.g. a websocket.
        /// </summary>
        public ITSHurler Hurler { get; set; } = null;

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

        /// <summary>
        /// Send the given value on a secondary communication channel, e.g. via websocket.
        /// </summary>
        public void Hurl(ITSValueArgument a_value)
        {
            // prepare data to hurl
            Dictionary<string, TSValue> projectile = new();
            if (a_value is TSNamedValueArgument namedValue)
            {
                if (namedValue.HasWildcard)
                {
                    // add multiple values with common prefix
                    var wildcardValues = Values.Where(kvp => kvp.Key.StartsWith(namedValue.ValueName));
                    foreach (var currentValue in wildcardValues)
                    {
                        // add without common prefix
                        projectile[currentValue.Key[namedValue.ValueName.Length..]] = currentValue.Value;
                    }
                }
                else
                {
                    // add single named value w/o prefix
                    var valueName = (TSConstants.ValuePrefixes.Any(p => p == namedValue.ValueName[0]))
                        ? namedValue.ValueName[1..]
                        : namedValue.ValueName;
                    projectile.Add(valueName, a_value.GetValue(Values));
                }
            }
            else
            {
                // single unnamed constant
                projectile.Add("value", a_value.GetValue(Values));
            }
            // hurl data
            Hurler?.Hurl(projectile);
        }
    }
}
