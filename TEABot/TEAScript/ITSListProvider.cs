using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.TEAScript
{
    /// <summary>
    /// Interface for list providers which can supply on-demand internal special lists
    /// </summary>
    public interface ITSListProvider
    {
        /// <summary>
        /// Request an on-demand list.
        /// Which lists are available is up to the implementation.
        /// </summary>
        /// <param name="a_listName">Name of the requested list.</param>
        /// <returns>The requested list or null if no such list is available.</returns>
        public TSValueList GetList(string a_listName);

        /// <summary>
        /// Create a new named list which may then be accessed by other scripts / executions.
        /// These lists are expected to have a lifespan for the current program execution only,
        /// with now data persistence between bot runs.
        /// </summary>
        /// <param name="a_listName">Name of the list to create</param>
        /// <returns>The list if it was created, null if not, e.g. due to a name collision</returns>
        public TSValueList CreateList(string a_listName);
    }
}
