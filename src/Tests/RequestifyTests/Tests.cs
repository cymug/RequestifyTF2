﻿namespace RequestifyTF2.Tests

{
    using NUnit.Framework;

    [TestFixture]
    public static class RequestifyTest
    {
        [Test]
        public static void TestCommandExecute()
        {
            var sut = ReaderThread.TextChecker(
                "nickname test lyl:) : !request https://www.youtube.com/watch?v=DZV3Xtp-BK0");
            Assert.That(sut, Is.EqualTo(ReaderThread.Result.CommandExecute));
        }

        [Test]
        public static void TestConnect()
        {
            var sut = ReaderThread.TextChecker("DllMain | aimware connected");
            Assert.That(sut, Is.EqualTo(ReaderThread.Result.Connected));
        }

        public static void TestKill()
        {
            var sut = ReaderThread.TextChecker("BoyPussi killed dat boi 28 with sniperrifle. (crit)");
            Assert.That(sut, Is.EqualTo(ReaderThread.Result.KillCrit));
           sut = ReaderThread.TextChecker(
                "DllMain | aimware killed One-Man Cheeseburger Apocalypse with tf_projectile_rocket.");
            Assert.That(sut, Is.EqualTo(ReaderThread.Result.Kill));
        }

        [Test]
        public static void TestKillCrit()
        {
            var sut = ReaderThread.TextChecker(
                "DllMain | aimware killed One-Man Cheeseburger Apocalypse with sniperrifle. (crit)");
            Assert.That(sut, Is.EqualTo(ReaderThread.Result.KillCrit));
        }

        [Test]
        public static void TestSuicide()
        {
            var sut = ReaderThread.TextChecker("DllMain | aimware suicided.");
            Assert.That(sut, Is.EqualTo(ReaderThread.Result.Suicide));
        }
    }
}