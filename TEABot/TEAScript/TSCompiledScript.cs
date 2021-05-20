using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEABot.TEAScript.Instructions.Statements;

namespace TEABot.TEAScript
{
    /// <summary>
    /// A compiled TEAScript
    /// </summary>
    public class TSCompiledScript
    {
        /// <summary>
        /// The script's name for dynamic documentation
        /// </summary>
        public string Name { get; set; } = String.Empty;

        /// <summary>
        /// The script's description for dynamic documentation
        /// </summary>
        public string Description { get; set; } = String.Empty;

        /// <summary>
        /// Chat commands to trigger execution
        /// </summary>
        public List<string> Commands { get; set; } = new List<string>();

        /// <summary>
        /// Regex pattern to trigger execution
        /// </summary>
        public string RegexPattern { get; set; } = String.Empty;

        /// <summary>
        /// Interval in seconds for periodically executed scripts
        /// </summary>
        public long Interval { get; set; } = 0L;

        /// <summary>
        /// True iff the script should be triggered on received twitch emotes
        /// </summary>
        public bool TwitchEmotes { get; set; } = false;

        /// <summary>
        /// The script's parameter specifications
        /// </summary>
        public List<TSParameter> Parameters { get; } = new List<TSParameter>();

        /// <summary>
        /// The script's statements
        /// </summary>
        public TSIStatement[] Statements { get; } = null;

        /// <summary>
        /// The script's jump labels.
        /// Maps from label name (including prefix '@') to index in Statements.
        /// </summary>
        public Dictionary<string, int> Labels { get; } = null;

        /// <summary>
        /// Create a new script with the given statements.
        /// </summary>
        /// <param name="a_statements">The script's statements</param>
        /// <param name="a_labels">The jump labels</param>
        public TSCompiledScript(TSIStatement[] a_statements, Dictionary<string, int> a_labels)
        {
            Statements = a_statements ?? throw new ArgumentNullException(nameof(a_statements));
            Labels = a_labels ?? throw new ArgumentNullException(nameof(a_labels));
        }
    }
}
