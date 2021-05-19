using Microsoft.VisualStudio.TestTools.UnitTesting;
using TEABot.Twitch;

namespace TEABotTests.Twitch
{
    [TestClass]
    public class TTwBadgesTests
    {
        [TestMethod]
        public void TTwBadges_Construct_Parse_Null()
        {
            TTwBadges parsedBadges = new(null, null);

            Assert.IsNotNull(parsedBadges.Badges);
            Assert.AreEqual(0, parsedBadges.Badges.Count);
        }

        [TestMethod]
        public void TTwBadges_Construct_Parse_SingleBadge_NoInfo()
        {
            TTwBadges parsedBadges = new(
                "moderator/1",
                null);

            Assert.IsNotNull(parsedBadges.Badges);
            Assert.AreEqual(1, parsedBadges.Badges.Count);
            var badge = parsedBadges.Badges[0];
            Assert.AreEqual("moderator", badge.Name);
            Assert.AreEqual(1, badge.Version);
            Assert.AreEqual(0, badge.Info);

            Assert.IsFalse(parsedBadges.IsBroadcaster);
            Assert.IsTrue(parsedBadges.IsModerator);
            Assert.IsTrue(parsedBadges.IsPrivileged);
        }

        [TestMethod]
        public void TTwBadges_Construct_Parse_SingleBadge_WithInfo()
        {
            TTwBadges parsedBadges = new(
                "subscriber/3",
                "subscriber/146");

            Assert.IsNotNull(parsedBadges.Badges);
            Assert.AreEqual(1, parsedBadges.Badges.Count);
            var badge = parsedBadges.Badges[0];
            Assert.AreEqual("subscriber", badge.Name);
            Assert.AreEqual(3, badge.Version);
            Assert.AreEqual(146, badge.Info);

            Assert.IsFalse(parsedBadges.IsBroadcaster);
            Assert.IsFalse(parsedBadges.IsModerator);
            Assert.IsFalse(parsedBadges.IsPrivileged);
        }

        [TestMethod]
        public void TTwBadges_Construct_Parse_SingleBadge_WithOtherInfo()
        {
            TTwBadges parsedBadges = new(
                "broadcaster/27",
                "subscriber/146");

            Assert.IsNotNull(parsedBadges.Badges);
            Assert.AreEqual(1, parsedBadges.Badges.Count);
            var badge = parsedBadges.Badges[0];
            Assert.AreEqual("broadcaster", badge.Name);
            Assert.AreEqual(27, badge.Version);
            Assert.AreEqual(0, badge.Info);

            Assert.IsTrue(parsedBadges.IsBroadcaster);
            Assert.IsFalse(parsedBadges.IsModerator);
            Assert.IsTrue(parsedBadges.IsPrivileged);
        }

        [TestMethod]
        public void TTwBadges_Construct_Parse_MultiBadges_NoInfo()
        {
            TTwBadges parsedBadges = new(
                "broadcaster/1,subscriber/0,premium/1",
                null);

            Assert.IsNotNull(parsedBadges.Badges);
            Assert.AreEqual(3, parsedBadges.Badges.Count);
            var badge = parsedBadges.Badges[0];
            Assert.AreEqual("broadcaster", badge.Name);
            Assert.AreEqual(1, badge.Version);
            Assert.AreEqual(0, badge.Info);
            badge = parsedBadges.Badges[1];
            Assert.AreEqual("subscriber", badge.Name);
            Assert.AreEqual(0, badge.Version);
            Assert.AreEqual(0, badge.Info);
            badge = parsedBadges.Badges[2];
            Assert.AreEqual("premium", badge.Name);
            Assert.AreEqual(1, badge.Version);
            Assert.AreEqual(0, badge.Info);

            Assert.IsTrue(parsedBadges.IsBroadcaster);
            Assert.IsFalse(parsedBadges.IsModerator);
            Assert.IsTrue(parsedBadges.IsPrivileged);
        }

        [TestMethod]
        public void TTwBadges_Construct_Parse_MultiBadges_WithInfo()
        {
            TTwBadges parsedBadges = new(
                "broadcaster/1,subscriber/3,premium/28",
                "subscriber/28");

            Assert.IsNotNull(parsedBadges.Badges);
            Assert.AreEqual(3, parsedBadges.Badges.Count);
            var badge = parsedBadges.Badges[0];
            Assert.AreEqual("broadcaster", badge.Name);
            Assert.AreEqual(1, badge.Version);
            Assert.AreEqual(0, badge.Info);
            badge = parsedBadges.Badges[1];
            Assert.AreEqual("subscriber", badge.Name);
            Assert.AreEqual(3, badge.Version);
            Assert.AreEqual(28, badge.Info);
            badge = parsedBadges.Badges[2];
            Assert.AreEqual("premium", badge.Name);
            Assert.AreEqual(28, badge.Version);
            Assert.AreEqual(0, badge.Info);

            Assert.IsTrue(parsedBadges.IsBroadcaster);
            Assert.IsFalse(parsedBadges.IsModerator);
            Assert.IsTrue(parsedBadges.IsPrivileged);
        }

        [TestMethod]
        public void TTwBadges_Construct_Parse_MultiBadges_WithOtherInfo()
        {
            TTwBadges parsedBadges = new(
                "broadcaster/1,moderator/1,premium/7",
                "subscriber/28");

            Assert.IsNotNull(parsedBadges.Badges);
            Assert.AreEqual(3, parsedBadges.Badges.Count);
            var badge = parsedBadges.Badges[0];
            Assert.AreEqual("broadcaster", badge.Name);
            Assert.AreEqual(1, badge.Version);
            Assert.AreEqual(0, badge.Info);
            badge = parsedBadges.Badges[1];
            Assert.AreEqual("moderator", badge.Name);
            Assert.AreEqual(1, badge.Version);
            Assert.AreEqual(0, badge.Info);
            badge = parsedBadges.Badges[2];
            Assert.AreEqual("premium", badge.Name);
            Assert.AreEqual(7, badge.Version);
            Assert.AreEqual(0, badge.Info);

            Assert.IsTrue(parsedBadges.IsBroadcaster);
            Assert.IsTrue(parsedBadges.IsModerator);
            Assert.IsTrue(parsedBadges.IsPrivileged);
        }

        [TestMethod]
        public void TTwBadges_Construct_Parse_MultiBadges_WithMultiInfo()
        {
            TTwBadges parsedBadges = new(
                "broadcaster/1,subscriber/3,premium/28,bot/12",
                "subscriber/12,premium/72,bot/9");

            Assert.IsNotNull(parsedBadges.Badges);
            Assert.AreEqual(4, parsedBadges.Badges.Count);
            var badge = parsedBadges.Badges[0];
            Assert.AreEqual("broadcaster", badge.Name);
            Assert.AreEqual(1, badge.Version);
            Assert.AreEqual(0, badge.Info);
            badge = parsedBadges.Badges[1];
            Assert.AreEqual("subscriber", badge.Name);
            Assert.AreEqual(3, badge.Version);
            Assert.AreEqual(12, badge.Info);
            badge = parsedBadges.Badges[2];
            Assert.AreEqual("premium", badge.Name);
            Assert.AreEqual(28, badge.Version);
            Assert.AreEqual(72, badge.Info);
            badge = parsedBadges.Badges[3];
            Assert.AreEqual("bot", badge.Name);
            Assert.AreEqual(12, badge.Version);
            Assert.AreEqual(9, badge.Info);

            Assert.IsTrue(parsedBadges.IsBroadcaster);
            Assert.IsFalse(parsedBadges.IsModerator);
            Assert.IsTrue(parsedBadges.IsPrivileged);
        }

    }
}
