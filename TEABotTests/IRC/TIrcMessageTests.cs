using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TEABot.IRC;

namespace TEABotTests.IRC
{
    [TestClass]
    public class TIrcMessageTests
    {
        [TestMethod]
        public void MessageTags_Parse_SingleTag()
        {
            // arrange test data
            var rawTags = "tagName=tagValue";

            // call method under test
            var parsedTags = TIrcMessage.MessageTags.Parse(rawTags);

            // assert method effects
            Assert.AreEqual(1, parsedTags.Tags.Count, "Incorrect number of tags parsed");
            var tagValue = parsedTags.Tags["tagName"];
            Assert.AreEqual("tagValue", tagValue, "Incorrect value for parsed tag");
        }

        [TestMethod]
        public void MessageTags_Parse_SingleTagWithEmpty()
        {
            // single valid tag with empty tag following the separator
            var rawTagsTrailing = "tagName=tagValue;";
            var parsedTagsTrailing = TIrcMessage.MessageTags.Parse(rawTagsTrailing);
            MessageTags_Parse_SingleTagWithEmpty_AssertTagValues(parsedTagsTrailing);
            // single valid tag with multiple empty tags following the separator
            var rawTagsMulti = "tagName=tagValue;";
            var parsedTagsMulti = TIrcMessage.MessageTags.Parse(rawTagsMulti);
            MessageTags_Parse_SingleTagWithEmpty_AssertTagValues(parsedTagsMulti);
            // single valid tag with empty tag before the separator
            var rawTagsLeading = ";tagName=tagValue";
            var parsedTagsLeading = TIrcMessage.MessageTags.Parse(rawTagsLeading);
            MessageTags_Parse_SingleTagWithEmpty_AssertTagValues(parsedTagsLeading);
        }

        /// <summary>
        /// Assert parsed tags for MessageTags_Parse_SingleTagWithEmpty() as we expect the same result multiple times
        /// </summary>
        /// <param name="a_parsedTags">Parsed message tags</param>
        private void MessageTags_Parse_SingleTagWithEmpty_AssertTagValues(TIrcMessage.MessageTags a_parsedTags)
        {
            Assert.AreEqual(1, a_parsedTags.Tags.Count);
            var tagValueMulti = a_parsedTags.Tags["tagName"];
            Assert.AreEqual("tagValue", tagValueMulti);
        }

        [TestMethod]
        public void MessageTags_Parse_EmptyTags()
        {
            // empty raw string
            var rawTagsEmpty = "";
            var parsedTagsEmpty = TIrcMessage.MessageTags.Parse(rawTagsEmpty);
            Assert.AreEqual(0, parsedTagsEmpty.Tags.Count);
            // single separator
            var rawTagsOneSep = ";";
            var parsedTagsOneSep = TIrcMessage.MessageTags.Parse(rawTagsOneSep);
            Assert.AreEqual(0, parsedTagsOneSep.Tags.Count);
            // multiple separators
            var rawTagsMultiSep = ";;;";
            var parsedTagsMultiSep = TIrcMessage.MessageTags.Parse(rawTagsMultiSep);
            Assert.AreEqual(0, parsedTagsMultiSep.Tags.Count);
            // empty tag value is to be treated the same as a missing tag
            var rawTagsMissing = "tagName=";
            var parsedTagsMissing = TIrcMessage.MessageTags.Parse(rawTagsMissing);
            Assert.AreEqual(0, parsedTagsMissing.Tags.Count);
        }

        [TestMethod]
        public void MessageTags_Parse_MultipleTags()
        {
            var rawTags = "tagName=tagValue;toast=buttered;up=down";

            var parsedTags = TIrcMessage.MessageTags.Parse(rawTags);

            Assert.AreEqual(3, parsedTags.Tags.Count);

            var tagValue_tagName = parsedTags.Tags["tagName"];
            Assert.AreEqual("tagValue", tagValue_tagName);
            var tagValue_toast = parsedTags.Tags["toast"];
            Assert.AreEqual("buttered", tagValue_toast);
            var tagValue_up = parsedTags.Tags["up"];
            Assert.AreEqual("down", tagValue_up);
        }

        [TestMethod]
        public void MessageTags_Parse_MultipleTagsWithEmpty()
        {
            // trailing empty tag
            var rawTagsTrailing = "tagName=tagValue;toast=buttered;up=down;";
            var parsedTagsTrailing = TIrcMessage.MessageTags.Parse(rawTagsTrailing);
            MessageTags_Parse_MultipleTagsWithEmpty_AssertTagValues(parsedTagsTrailing);
            // leading empty tag
            var rawTagsLeading = ";tagName=tagValue;toast=buttered;up=down";
            var parsedTagsLeading = TIrcMessage.MessageTags.Parse(rawTagsLeading);
            MessageTags_Parse_MultipleTagsWithEmpty_AssertTagValues(parsedTagsLeading);
            // empty tags in between
            var rawTagsMiddle = "tagName=tagValue;;toast=buttered;;;up=down";
            var parsedTagsMiddle = TIrcMessage.MessageTags.Parse(rawTagsMiddle);
            MessageTags_Parse_MultipleTagsWithEmpty_AssertTagValues(parsedTagsMiddle);
            // missing tag value
            var rawTagsMissing = "tagName=tagValue;toast=buttered;noValue=;up=down";
            var parsedTagsMissing = TIrcMessage.MessageTags.Parse(rawTagsMissing);
            MessageTags_Parse_MultipleTagsWithEmpty_AssertTagValues(parsedTagsMissing);
        }

        /// <summary>
        /// Assert parsed tags for MessageTags_Parse_SingleTagWithEmpty() as we expect the same result multiple times
        /// </summary>
        /// <param name="a_parsedTags">Parsed message tags</param>
        private void MessageTags_Parse_MultipleTagsWithEmpty_AssertTagValues(TIrcMessage.MessageTags a_parsedTags)
        {
            Assert.AreEqual(3, a_parsedTags.Tags.Count);
            var tagValueTrailing_tagName = a_parsedTags.Tags["tagName"];
            Assert.AreEqual("tagValue", tagValueTrailing_tagName);
            var tagValueTrailing_toast = a_parsedTags.Tags["toast"];
            Assert.AreEqual("buttered", tagValueTrailing_toast);
            var tagValueTrailing_up = a_parsedTags.Tags["up"];
            Assert.AreEqual("down", tagValueTrailing_up);
        }

        [TestMethod]
        public void MessageTags_Parse_MultipleTags_Repeated()
        {
            // if tags are repeated, the value of the last occurrence should be used
            var rawTags = @"tagName=tagValue;toast=buttered;tagName=actually\ssome\r\nother\svalue;up=down";

            var parsedTags = TIrcMessage.MessageTags.Parse(rawTags);

            Assert.AreEqual(3, parsedTags.Tags.Count);

            var tagValue_tagName = parsedTags.Tags["tagName"];
            Assert.AreEqual("actually some\r\nother value", tagValue_tagName);
            var tagValue_toast = parsedTags.Tags["toast"];
            Assert.AreEqual("buttered", tagValue_toast);
            var tagValue_up = parsedTags.Tags["up"];
            Assert.AreEqual("down", tagValue_up);

            // if final occurrence has no value, drop tag
            var rawTagsMissing = @"tagName=tagValue;tagName=second;toast=buttered;tagName=;up=down";
            var parsedTagsMissing = TIrcMessage.MessageTags.Parse(rawTagsMissing);
            Assert.AreEqual(2, parsedTagsMissing.Tags.Count);
            Assert.AreEqual(false, parsedTagsMissing.Tags.ContainsKey("tagName"));
            var tagValueMissing_toast = parsedTagsMissing.Tags["toast"];
            Assert.AreEqual("buttered", tagValueMissing_toast);
            var tagValueMissing_up = parsedTagsMissing.Tags["up"];
            Assert.AreEqual("down", tagValueMissing_up);
        }

        [TestMethod]
        public void MessageTags_EscapeValue()
        {
            var plainValue = "a;weird value\r\nwith characters to \\replace\\";

            var escapedValue = TIrcMessage.MessageTags.EscapeValue(plainValue);

            Assert.AreEqual(@"a\:weird\svalue\r\nwith\scharacters\sto\s\\replace\\", escapedValue);
        }

        [TestMethod]
        public void MessageTags_UnescapeValue()
        {
            // common case without irregularities (errors in the escaped string)
            var escapedValue = @"a\:weird\svalue\r\nwith\scharacters\sto\s\\replace\\";
            var plainValue = TIrcMessage.MessageTags.UnescapeValue(escapedValue);
            Assert.AreEqual("a;weird value\r\nwith characters to \\replace\\", plainValue);

            // traling escape character should be dropped
            var escapedTrailing = @"oops\sI\sdropped\smy\";
            var plainTrailing = TIrcMessage.MessageTags.UnescapeValue(escapedTrailing);
            Assert.AreEqual("oops I dropped my", plainTrailing);

            // invalid escape sequences should ignore the escape character
            var escapedInvalid = @"it\swas\dropped\shere";
            var plainInvalid = TIrcMessage.MessageTags.UnescapeValue(escapedInvalid);
            Assert.AreEqual("it wasdropped here", plainInvalid);
        }

        [TestMethod]
        public void MessageTags_ToString()
        {
            // empty tags
            var tagsEmpty = new TIrcMessage.MessageTags();
            var stringEmpty = tagsEmpty.ToString();
            Assert.AreEqual(String.Empty, stringEmpty);

            // single tag
            var valuesSingle = new Dictionary<String, String>
            {
                { "tagName", "tagValue" }
            };
            var tagsSingle = new TIrcMessage.MessageTags(valuesSingle);
            var stringSingle = tagsSingle.ToString();
            Assert.AreEqual("tagName=tagValue", stringSingle);

            // multiple tags & escaping
            var valuesMulti = new Dictionary<String, String>
            {
                { "tagName", "tagValue" },
                { "anotherTag", "something" },
                { "third", "other thing" }
            };
            var tagsMulti = new TIrcMessage.MessageTags(valuesMulti);
            var stringMultie = tagsMulti.ToString();
            // sequence is ordered ascending by key name
            Assert.AreEqual(@"anotherTag=something;tagName=tagValue;third=other\sthing", stringMultie);
        }
    }
}
