using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TEABot.Twitch;

namespace TEABotTests.Twitch
{
    [TestClass]
    public class TTwMessageTagsTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException),
            "Missing tags dictionary was incorrectly accepted.")]
        public void TTwEmotes_Construct_Null()
        {
            new TTwMessageTags(null, null);
        }

        [TestMethod]
        public void TTwEmotes_Construct_Badges()
        {
            TTwMessageTags parsedTags = new(
                new Dictionary<string, string>()
                {
                    { "badges", "broadcaster/1,subscriber/0,premium/1" }
                },
                null);

            Assert.IsNotNull(parsedTags.Badges);
            Assert.IsNull(parsedTags.UserColor);
            Assert.IsNull(parsedTags.DisplayName);
            Assert.IsNull(parsedTags.Emotes);
            Assert.IsNull(parsedTags.MessageId);
            Assert.IsNull(parsedTags.UserId);

            Assert.IsNotNull(parsedTags.Badges.Badges);
            Assert.AreEqual(3, parsedTags.Badges.Badges.Count);
            Assert.IsTrue(parsedTags.Badges.IsBroadcaster);
            Assert.IsFalse(parsedTags.Badges.IsModerator);
            Assert.IsTrue(parsedTags.Badges.IsPrivileged);
        }

        [TestMethod]
        public void TTwEmotes_Construct_UserColor()
        {
            TTwMessageTags parsedTags = new(
                new Dictionary<string, string>()
                {
                    { "color", "#9C15B5" }
                },
                null);

            Assert.IsNull(parsedTags.Badges);
            Assert.IsNotNull(parsedTags.UserColor);
            Assert.IsNull(parsedTags.DisplayName);
            Assert.IsNull(parsedTags.Emotes);
            Assert.IsNull(parsedTags.MessageId);
            Assert.IsNull(parsedTags.UserId);

            Assert.IsTrue(parsedTags.UserColor.HasValue);
            Assert.AreEqual(0x9C, parsedTags.UserColor.Value.R);
            Assert.AreEqual(0x15, parsedTags.UserColor.Value.G);
            Assert.AreEqual(0xB5, parsedTags.UserColor.Value.B);
        }

        [TestMethod]
        public void TTwEmotes_Construct_DisplayName()
        {
            TTwMessageTags parsedTags = new(
                new Dictionary<string, string>()
                {
                    { "display-name", "MarcMarkusMafaldaBjorn" }
                },
                null);

            Assert.IsNull(parsedTags.Badges);
            Assert.IsNull(parsedTags.UserColor);
            Assert.IsNotNull(parsedTags.DisplayName);
            Assert.IsNull(parsedTags.Emotes);
            Assert.IsNull(parsedTags.MessageId);
            Assert.IsNull(parsedTags.UserId);

            Assert.AreEqual("MarcMarkusMafaldaBjorn", parsedTags.DisplayName);
        }

        [TestMethod]
        public void TTwEmotes_Construct_Emotes()
        {
            TTwMessageTags parsedTags = new(
                new Dictionary<string, string>()
                {
                    { "emotes", "306981865:11-22,24-35,49-60/emotesv2_b7c104b7df764573b503257a8965631f:37-47,62-72/307070118:0-9" }
                },
                "gamepa13Hi lordpr2Hello lordpr2Hello lordpr2Coco lordpr2Hello lordpr2Coco");

            Assert.IsNull(parsedTags.Badges);
            Assert.IsNull(parsedTags.UserColor);
            Assert.IsNull(parsedTags.DisplayName);
            Assert.IsNotNull(parsedTags.Emotes);
            Assert.IsNull(parsedTags.MessageId);
            Assert.IsNull(parsedTags.UserId);

            Assert.IsNotNull(parsedTags.Emotes.Emotes);
            Assert.AreEqual(6, parsedTags.Emotes.Emotes.Count);

            var emote = parsedTags.Emotes.Emotes[0];
            Assert.AreEqual("306981865", emote.Id);
            Assert.AreEqual("lordpr2Hello", emote.Name);
            Assert.AreEqual(11, emote.Start);
            Assert.AreEqual(22, emote.End);
        }

        [TestMethod]
        public void TTwEmotes_Construct_MessageId()
        {
            TTwMessageTags parsedTags = new(
                new Dictionary<string, string>()
                {
                    { "id", "1234567890abcdef" }
                },
                null);

            Assert.IsNull(parsedTags.Badges);
            Assert.IsNull(parsedTags.UserColor);
            Assert.IsNull(parsedTags.DisplayName);
            Assert.IsNull(parsedTags.Emotes);
            Assert.IsNotNull(parsedTags.MessageId);
            Assert.IsNull(parsedTags.UserId);

            Assert.AreEqual("1234567890abcdef", parsedTags.MessageId);
        }

        [TestMethod]
        public void TTwEmotes_Construct_UserId()
        {
            TTwMessageTags parsedTags = new(
                new Dictionary<string, string>()
                {
                    { "user-id", "abcdef1234567890" }
                },
                null);

            Assert.IsNull(parsedTags.Badges);
            Assert.IsNull(parsedTags.UserColor);
            Assert.IsNull(parsedTags.DisplayName);
            Assert.IsNull(parsedTags.Emotes);
            Assert.IsNull(parsedTags.MessageId);
            Assert.IsNotNull(parsedTags.UserId);

            Assert.AreEqual("abcdef1234567890", parsedTags.UserId);
        }

        [TestMethod]
        public void TTwEmotes_Construct_Unknown()
        {
            TTwMessageTags parsedTags = new(
                new Dictionary<string, string>()
                {
                    { "unknown-tag", "value we don't care about" }
                },
                null);

            Assert.IsNull(parsedTags.Badges);
            Assert.IsNull(parsedTags.UserColor);
            Assert.IsNull(parsedTags.DisplayName);
            Assert.IsNull(parsedTags.Emotes);
            Assert.IsNull(parsedTags.MessageId);
            Assert.IsNull(parsedTags.UserId);
        }

        [TestMethod]
        public void TTwEmotes_Construct_Mixed()
        {
            TTwMessageTags parsedTags = new(
                new Dictionary<string, string>()
                {
                    { "badges", "broadcaster/1,subscriber/0,premium/1" },
                    { "color", "#9C15B5" },
                    { "emotes", "306981865:11-22,24-35,49-60/emotesv2_b7c104b7df764573b503257a8965631f:37-47,62-72/307070118:0-9" }
                },
                "gamepa13Hi lordpr2Hello lordpr2Hello lordpr2Coco lordpr2Hello lordpr2Coco");

            Assert.IsNotNull(parsedTags.Badges);
            Assert.IsNotNull(parsedTags.UserColor);
            Assert.IsNull(parsedTags.DisplayName);
            Assert.IsNotNull(parsedTags.Emotes);
            Assert.IsNull(parsedTags.MessageId);
            Assert.IsNull(parsedTags.UserId);

            Assert.IsNotNull(parsedTags.Badges.Badges);
            Assert.AreEqual(3, parsedTags.Badges.Badges.Count);

            Assert.IsNotNull(parsedTags.Emotes.Emotes);
            Assert.AreEqual(6, parsedTags.Emotes.Emotes.Count);
        }
    }
}
