using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TEABot.Twitch;

namespace TEABotTests.Twitch
{
    [TestClass]
    public class TTwEmotesTests
    {
        [TestMethod]
        public void TTwEmotes_Construct_Parse_Null()
        {
            TTwEmotes parsedEmotes = new(null, null);

            Assert.IsNotNull(parsedEmotes.Emotes);
            Assert.AreEqual(0, parsedEmotes.Emotes.Count);
        }

        [TestMethod]
        public void TTwEmotes_Construct_Parse_SingleEmote()
        {
            TTwEmotes parsedEmotes = new(
                "306981865:0-11",
                "lordpr2Hello");

            Assert.IsNotNull(parsedEmotes.Emotes);
            Assert.AreEqual(1, parsedEmotes.Emotes.Count);

            var emote = parsedEmotes.Emotes[0];
            Assert.AreEqual("306981865", emote.Id);
            Assert.AreEqual("lordpr2Hello", emote.Name);
            Assert.AreEqual(0, emote.Start);
            Assert.AreEqual(11, emote.End);
        }

        [TestMethod]
        public void TTwEmotes_Construct_Parse_SingleEmote_NoMessage()
        {
            TTwEmotes parsedEmotes = new(
                "306981865:0-11",
                null);

            Assert.IsNotNull(parsedEmotes.Emotes);
            Assert.AreEqual(1, parsedEmotes.Emotes.Count);

            var emote = parsedEmotes.Emotes[0];
            Assert.AreEqual("306981865", emote.Id);
            // No name could be extracted
            Assert.AreEqual(String.Empty, emote.Name);
            Assert.AreEqual(0, emote.Start);
            // should be adjusted so it can be safely used to access a source message later
            // without requiring checking the message bounds
            Assert.AreEqual(0, emote.End);
        }

        [TestMethod]
        public void TTwEmotes_Construct_Parse_SingleEmote_EmptyMessage()
        {
            TTwEmotes parsedEmotes = new(
                "306981865:0-11",
                String.Empty);

            Assert.IsNotNull(parsedEmotes.Emotes);
            Assert.AreEqual(1, parsedEmotes.Emotes.Count);

            var emote = parsedEmotes.Emotes[0];
            Assert.AreEqual("306981865", emote.Id);
            Assert.AreEqual(string.Empty, emote.Name);
            Assert.AreEqual(0, emote.Start);
            Assert.AreEqual(0, emote.End);
        }

        [TestMethod]
        public void TTwEmotes_Construct_Parse_MultiEmotes()
        {
            TTwEmotes parsedEmotes = new(
                "307683255:28-44/306981865:0-11",
                "lordpr2Hello frens this has gamepa13Mildpanic 'motes");

            Assert.IsNotNull(parsedEmotes.Emotes);
            Assert.AreEqual(2, parsedEmotes.Emotes.Count);

            var emote = parsedEmotes.Emotes[0];
            Assert.AreEqual("307683255", emote.Id);
            Assert.AreEqual("gamepa13Mildpanic", emote.Name);
            Assert.AreEqual(28, emote.Start);
            Assert.AreEqual(44, emote.End);

            emote = parsedEmotes.Emotes[1];
            Assert.AreEqual("306981865", emote.Id);
            Assert.AreEqual("lordpr2Hello", emote.Name);
            Assert.AreEqual(0, emote.Start);
            Assert.AreEqual(11, emote.End);
        }

        [TestMethod]
        public void TTwEmotes_Construct_Parse_DuplicateEmotes()
        {
            TTwEmotes parsedEmotes = new(
                "306981865:11-22,24-35,49-60/emotesv2_b7c104b7df764573b503257a8965631f:37-47,62-72/307070118:0-9",
                "gamepa13Hi lordpr2Hello lordpr3Hello lordpr2Coco lordpr4Hello lordpr2Cofe");

            Assert.IsNotNull(parsedEmotes.Emotes);
            Assert.AreEqual(6, parsedEmotes.Emotes.Count);

            var emote = parsedEmotes.Emotes[0];
            Assert.AreEqual("306981865", emote.Id);
            Assert.AreEqual("lordpr2Hello", emote.Name);
            Assert.AreEqual(11, emote.Start);
            Assert.AreEqual(22, emote.End);

            emote = parsedEmotes.Emotes[1];
            Assert.AreEqual("306981865", emote.Id);
            Assert.AreEqual("lordpr3Hello", emote.Name);
            Assert.AreEqual(24, emote.Start);
            Assert.AreEqual(35, emote.End);

            emote = parsedEmotes.Emotes[2];
            Assert.AreEqual("306981865", emote.Id);
            Assert.AreEqual("lordpr4Hello", emote.Name);
            Assert.AreEqual(49, emote.Start);
            Assert.AreEqual(60, emote.End);

            emote = parsedEmotes.Emotes[3];
            Assert.AreEqual("emotesv2_b7c104b7df764573b503257a8965631f", emote.Id);
            Assert.AreEqual("lordpr2Coco", emote.Name);
            Assert.AreEqual(37, emote.Start);
            Assert.AreEqual(47, emote.End);

            emote = parsedEmotes.Emotes[4];
            Assert.AreEqual("emotesv2_b7c104b7df764573b503257a8965631f", emote.Id);
            Assert.AreEqual("lordpr2Cofe", emote.Name);
            Assert.AreEqual(62, emote.Start);
            Assert.AreEqual(72, emote.End);

            emote = parsedEmotes.Emotes[5];
            Assert.AreEqual("307070118", emote.Id);
            Assert.AreEqual("gamepa13Hi", emote.Name);
            Assert.AreEqual(0, emote.Start);
            Assert.AreEqual(9, emote.End);
        }

        [TestMethod]
        public void TTwEmotes_Construct_Parse_Overlap()
        {
            TTwEmotes parsedEmotes = new(
                "306981865:11-22/emotesv2_b7c104b7df764573b503257a8965631f:24-34,15-29/307070118:0-9",
                "gamepa13Hi lordpr2Hello lordpr2Coco");

            Assert.IsNotNull(parsedEmotes.Emotes);
            Assert.AreEqual(4, parsedEmotes.Emotes.Count);

            var emote = parsedEmotes.Emotes[0];
            Assert.AreEqual("306981865", emote.Id);
            Assert.AreEqual("lordpr2Hello", emote.Name);
            Assert.AreEqual(11, emote.Start);
            Assert.AreEqual(22, emote.End);

            emote = parsedEmotes.Emotes[1];
            Assert.AreEqual("emotesv2_b7c104b7df764573b503257a8965631f", emote.Id);
            Assert.AreEqual("lordpr2Coco", emote.Name);
            Assert.AreEqual(24, emote.Start);
            Assert.AreEqual(34, emote.End);

            emote = parsedEmotes.Emotes[2];
            Assert.AreEqual("emotesv2_b7c104b7df764573b503257a8965631f", emote.Id);
            Assert.AreEqual("pr2Hello lordpr", emote.Name);
            Assert.AreEqual(15, emote.Start);
            Assert.AreEqual(29, emote.End);

            emote = parsedEmotes.Emotes[3];
            Assert.AreEqual("307070118", emote.Id);
            Assert.AreEqual("gamepa13Hi", emote.Name);
            Assert.AreEqual(0, emote.Start);
            Assert.AreEqual(9, emote.End);
        }

        [TestMethod]
        public void TTwEmotes_Construct_Parse_OutOfBounds()
        {
            TTwEmotes parsedEmotes = new(
                "306981865:11-22/emotesv2_b7c104b7df764573b503257a8965631f:24-34/307070118:0-9",
                "gamepa13Hi lordpr2Hello lordpr2C");

            Assert.IsNotNull(parsedEmotes.Emotes);
            Assert.AreEqual(3, parsedEmotes.Emotes.Count);

            var emote = parsedEmotes.Emotes[0];
            Assert.AreEqual("306981865", emote.Id);
            Assert.AreEqual("lordpr2Hello", emote.Name);
            Assert.AreEqual(11, emote.Start);
            Assert.AreEqual(22, emote.End);

            emote = parsedEmotes.Emotes[1];
            Assert.AreEqual("emotesv2_b7c104b7df764573b503257a8965631f", emote.Id);
            Assert.AreEqual("lordpr2C", emote.Name);
            Assert.AreEqual(24, emote.Start);
            Assert.AreEqual(31, emote.End);

            emote = parsedEmotes.Emotes[2];
            Assert.AreEqual("307070118", emote.Id);
            Assert.AreEqual("gamepa13Hi", emote.Name);
            Assert.AreEqual(0, emote.Start);
            Assert.AreEqual(9, emote.End);
        }
    }
}
