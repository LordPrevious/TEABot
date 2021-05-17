using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.TEAScript
{
    /// <summary>
    /// Interface for hurler providers, which allow sending some data over a secondary
    /// communication channel, e.g. a websocket.
    /// </summary>
    public interface ITSHurler
    {
        /// <summary>
        /// Hurl some data.
        /// </summary>
        /// <param name="a_projectile">The data to hurl.</param>
        /// <returns>True on successful hurling, false on failure.</returns>
        bool Hurl(Dictionary<string, TSValue> a_projectile);
    }
}
