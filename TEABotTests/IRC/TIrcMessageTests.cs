using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var rawTags = @"tagName=tagValue";

            // call method under test
            var parsedTags = TIrcMessage.MessageTags.Parse(rawTags);

            // assert method effects
            // TODO Assert.AreEqual(1, parsedTags.Tags.Count, "Incorrect number of tags parsed");
        }
    }
}
