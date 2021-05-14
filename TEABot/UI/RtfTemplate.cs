using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.UI
{
    /// <summary>
    /// RTF-based log template
    /// </summary>
    public class RtfTemplate
    {
        #region Constructors

        /// <summary>
        /// Generate a default RTF template
        /// </summary>
        public RtfTemplate()
        {
            mSeparatorString = cSeparator.ToString();

            mParts = new Tuple<string, GetProcessedTemplatePart>[]
            {
                new Tuple<string, GetProcessedTemplatePart>(String.Empty, GetTimestampTemplatePart),
                new Tuple<string, GetProcessedTemplatePart>(" ", GetStaticTemplatePart),
                new Tuple<string, GetProcessedTemplatePart>(String.Empty, GetMessageTemplatePart)
            };
        }

        /// <summary>
        /// Generate a parsed RTF template from the given template string
        /// </summary>
        /// <param name="a_template">The raw template string</param>
        public RtfTemplate(string a_template)
        {
            mSeparatorString = cSeparator.ToString();

            // split template and get processing delegates
            var splitTemplate = a_template.Split('|');
            mParts = splitTemplate.Select(p =>
                    new Tuple<string, GetProcessedTemplatePart>(
                        p, GetProcessorDelegateForTemplatePart(p))
                    ).ToArray();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Apply the given arguments to the template
        /// </summary>
        /// <param name="a_message">The log message for MESSAGE placeholders</param>
        /// <param name="a_sender">The message sender for SENDER placeholders</param>
        /// <param name="a_timestampFormat">The DateTime format for TIMESTAMP placeholders</param>
        /// <returns>The processed template with placeholders replaced</returns>
        public string Apply(string a_message, string a_sender, string a_timestampFormat)
        {
            var sb = new StringBuilder();
            foreach (var p in mParts)
            {
                sb.Append(p.Item2(p.Item1, a_message, a_sender, a_timestampFormat));
            }
            return sb.ToString();
        }

        #endregion Public methods

        #region Delegates

        /// <summary>
        /// Delegate to process a template part, possibly substituting placeholders
        /// </summary>
        /// <param name="a_part">The part to process</param>
        /// <param name="a_message">The log message for MESSAGE placeholders</param>
        /// <param name="a_sender">The message sender for SENDER placeholders</param>
        /// <param name="a_timestampFormat">The DateTime format for TIMESTAMP placeholders</param>
        /// <returns>The processed template part</returns>
        private delegate string GetProcessedTemplatePart(string a_part, string a_message, string a_sender, string a_timestampFormat);

        #endregion

        #region Private data

        /// <summary>
        /// Separator character to split template into parts
        /// </summary>
        private const char cSeparator = '|';

        private readonly string mSeparatorString;

        /// <summary>
        /// Template parts and their processing delegates
        /// </summary>
        private readonly Tuple<string, GetProcessedTemplatePart>[] mParts;

        #endregion Private data

        #region Private methods

        /// <summary>
        /// Get the processor delegate which is appropriate for the given template part
        /// </summary>
        /// <param name="a_part">The template part</param>
        /// <returns>The appropriate delegate</returns>
        private GetProcessedTemplatePart GetProcessorDelegateForTemplatePart(string a_part)
        {
            if (a_part.Length == 0)
            {
                return GetSeparatorTemplatePart;
            }
            if (a_part.Equals("MESSAGE"))
            {
                return GetMessageTemplatePart;
            }
            if (a_part.Equals("SENDER"))
            {
                return GetSenderTemplatePart;
            }
            if (a_part.Equals("TIMESTAMP"))
            {
                return GetTimestampTemplatePart;
            }
            return GetStaticTemplatePart;
        }

        /// <summary>
        /// GetProcessedTemplatePart for static template parts
        /// </summary>
        /// <param name="a_part">The template part</param>
        /// <param name="a_message">Not needed</param>
        /// <param name="a_sender">Not needed</param>
        /// <param name="a_timestampFormat">Not needed</param>
        /// <returns>The unchanged template part</returns>
        private string GetStaticTemplatePart(string a_part, string a_message, string a_sender, string a_timestampFormat)
        {
            return a_part;
        }

        /// <summary>
        /// GetProcessedTemplatePart for MESSAGE placeholders
        /// </summary>
        /// <param name="a_part">Not needed</param>
        /// <param name="a_message">The log message</param>
        /// <param name="a_sender">Not needed</param>
        /// <param name="a_timestampFormat">Not needed</param>
        /// <returns>The unchanged template part</returns>
        private string GetMessageTemplatePart(string a_part, string a_message, string a_sender, string a_timestampFormat)
        {
            // escape for RTF
            return a_message.Replace("\\", "\\\\");
        }

        /// <summary>
        /// GetProcessedTemplatePart for SENDER placeholders
        /// </summary>
        /// <param name="a_part">Not needed</param>
        /// <param name="a_message">Not needed</param>
        /// <param name="a_sender">The message sender</param>
        /// <param name="a_timestampFormat">Not needed</param>
        /// <returns>The unchanged template part</returns>
        private string GetSenderTemplatePart(string a_part, string a_message, string a_sender, string a_timestampFormat)
        {
            // not escaping as we don't expect any special characters here
            return a_sender;
        }

        /// <summary>
        /// GetProcessedTemplatePart for TIMESTAMP placeholders
        /// </summary>
        /// <param name="a_part">Not needed</param>
        /// <param name="a_message">Not needed</param>
        /// <param name="a_sender">Not needed</param>
        /// <param name="a_timestampFormat">The DateTime format</param>
        /// <returns>The current time stamp</returns>
        private string GetTimestampTemplatePart(string a_part, string a_message, string a_sender, string a_timestampFormat)
        {
            return DateTime.Now.ToString(a_timestampFormat);
        }

        /// <summary>
        /// GetProcessedTemplatePart for empty parts, which can be used to produce the separator character itself
        /// </summary>
        /// <param name="a_part">Not needed</param>
        /// <param name="a_message">Not needed</param>
        /// <param name="a_sender">Not needed</param>
        /// <param name="a_timestampFormat">Not needed</param>
        /// <returns>The separator character</returns>
        private string GetSeparatorTemplatePart(string a_part, string a_message, string a_sender, string a_timestampFormat)
        {
            return mSeparatorString;
        }

        #endregion
    }
}
