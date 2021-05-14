using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEABot.TEAScript.Attributes;
using TEABot.TEAScript.Instructions.Metadata;
using TEABot.TEAScript.Instructions.Statements;

namespace TEABot.TEAScript
{
    /// <summary>
    /// Handles parsing of tea scripts
    /// </summary>
    public static class TSScriptParser
    {
        #region Public properties

        /// <summary>
        /// A broadcaster that's used during script parsing. Listen on it to receive parse messages including warnings and errors
        /// </summary>
        public static readonly TSBroadcaster Broadcaster = new();

        #endregion

        #region Public methods

        /// <summary>
        /// Parse a tea script from a file
        /// </summary>
        /// <param name="a_filename">The filename</param>
        /// <returns>The parsed script</returns>
        public static TSCompiledScript ParseScriptFromFile(string a_filename)
        {
            try
            {
                using var sr = File.OpenText(a_filename);
                return ParseScriptFromReader(sr);
            }
            catch (Exception e)
            {
                Broadcaster.Error("Failed to open script file \"{0}\": {1}",
                    a_filename,
                    e.Message);
            }
            return null;
        }

        /// <summary>
        /// Parse a tea script from a string
        /// </summary>
        /// <param name="a_scriptText">The script contents</param>
        /// <returns>The parsed script</returns>
        public static TSCompiledScript ParseScriptFromText(string a_scriptText)
        {
            try
            {
                using var sr = new StringReader(a_scriptText);
                return ParseScriptFromReader(sr);
            }
            catch (Exception e)
            {
                Broadcaster.Error("Failed to load script from string: {0}",
                    e.Message);
            }
            return null;
        }

        /// <summary>
        /// Parse a tea script from a text reader
        /// </summary>
        /// <param name="a_reader">The script reader</param>
        /// <returns>The parsed script</returns>
        public static TSCompiledScript ParseScriptFromReader(TextReader a_reader)
        {
            // ensure instruction dictionary is set up
            InitializeInstructionTypes();

            // initialize lists to hold parsed instructions
            var metadata = new List<TSIScriptMetadata>();
            var statements = new List<TSIStatement>();
            var jumpLabels = new Dictionary<string, int>();

            // read script line by line
            string line;
            var lineCount = 0;
            bool hasParseError = false;
            while ((line = a_reader.ReadLine()) != null)
            {
                ++lineCount;

                // skip empty lines and comments
                if ((line.Length == 0)
                    || (line[0] == COMMENT_INDICATOR))
                {
                    continue;
                }

                if (line[0] == TSConstants.JumpLabelPrefix)
                {
                    var jumpLabel = line.Trim();
                    var validator = new TSValidator(Broadcaster);
                    if (validator.IsJumpLabelName(jumpLabel))
                    {
                        if (jumpLabels.ContainsKey(jumpLabel))
                        {
                            hasParseError = true;
                            Broadcaster.Error(
                                "Repeated jump label at line {0}: \"{1}\"",
                                lineCount,
                                jumpLabel);
                        }
                        else
                        {
                            // jump label points to next statement that will be added
                            jumpLabels[jumpLabel] = statements.Count;
                        }
                    }
                    else
                    {
                        hasParseError = true;
                        Broadcaster.Error(
                            "Failed to parse jump label at line {0}: \"{1}\"",
                            lineCount,
                            jumpLabel);
                    }
                    continue;
                }

                // separate keyword and arguments
                string keyword;
                string arguments;
                var spaceIndex = line.IndexOf(KEYWORD_SEPARATOR);
                if (spaceIndex < 0)
                {
                    keyword = line;
                    arguments = String.Empty;
                }
                else
                {
                    keyword = line.Substring(0, spaceIndex);
                    arguments = line[(spaceIndex + 1)..];
                }

                // find corresponding instruction class
                if (sInstructionTypes.TryGetValue(keyword.ToLowerInvariant(), out Type instructionType))
                {
                    var instruction = (TSInstruction)(Activator.CreateInstance(instructionType));
                    if (instruction.Parse(Broadcaster, arguments))
                    {
                        if (instruction is TSIScriptMetadata scriptMetadata)
                        {
                            metadata.Add(scriptMetadata);
                        }
                        else if (instruction is TSIStatement statement)
                        {
                            statements.Add(statement);
                        }
                        else
                        {
                            hasParseError = true;
                            Broadcaster.Error(
                                "Instruction class for keyword \"{0}\", {1}, is neither metadata not statement.",
                                keyword,
                                instructionType);
                        }
                    }
                    else
                    {
                        hasParseError = true;
                        Broadcaster.Error(
                            "Failed to parse instruction line {0} for {1}: \"{2}\"",
                            lineCount,
                            keyword,
                            arguments);
                    }
                }
                else
                {
                    hasParseError = true;
                    Broadcaster.Error(
                        "Failed to parse instruction line {0}: \"{1}\"",
                        lineCount,
                        line);
                }
            }

            if (hasParseError)
            {
                // do not output failed scripts
                return null;
            }

            // ensure script ends with END statement
            if ((statements.Count > 0)
                && !(statements.Last() is TSISEnd))
            {
                Broadcaster.Info(
                    "Appending omitted end statement.");
                statements.Add(new TSISEnd());
            }

            // create new script instance with the parsed statements
            var result = new TSCompiledScript(statements.ToArray(), jumpLabels);

            // apply metadata
            foreach (var m in metadata)
            {
                m.Apply(result);
            }

            return result;
        }

        #endregion

        #region Private data

        /// <summary>
        /// Map from keyword to instruction class type
        /// </summary>
        private static Dictionary<string, Type> sInstructionTypes = null;

        /// <summary>
        /// Separator character between instruction keyword and arguments
        /// </summary>
        private static readonly char KEYWORD_SEPARATOR = ' ';

        /// <summary>
        /// Character at line start that indicates a comment line to be ignored while parsing
        /// </summary>
        private static readonly char COMMENT_INDICATOR = '#';

        #endregion

        #region Private methods

        /// <summary>
        /// Initialized the static class fields with instruction type info if not initialized already
        /// </summary>
        private static void InitializeInstructionTypes()
        {
            if (sInstructionTypes != null) return;

            sInstructionTypes = new Dictionary<string, Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var attributes = type.GetCustomAttributes(typeof(TSKeywordAttribute), false).Cast<TSKeywordAttribute>().ToArray();
                    if (attributes.Length > 0)
                    {
                        sInstructionTypes[attributes.First().Identifier.ToLowerInvariant()] = type;
                    }
                }
            }
        }

        #endregion
    }
}
