using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Tests
{
    /// <summary>
    /// Sound クラスの基本プロパティのユニットテスト
    /// </summary>
    [TestClass]
    public class SoundPropsTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // BGMFileName
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BGMFileName_DefaultIsNull()
        {
            var src = CreateSrc();
            Assert.IsNull(src.Sound.BGMFileName);
        }

        [TestMethod]
        public void BGMFileName_CanBeSet()
        {
            var src = CreateSrc();
            src.Sound.BGMFileName = "battle.mp3";
            Assert.AreEqual("battle.mp3", src.Sound.BGMFileName);
        }

        [TestMethod]
        public void BGMFileName_CanBeSetToEmpty()
        {
            var src = CreateSrc();
            src.Sound.BGMFileName = "";
            Assert.AreEqual("", src.Sound.BGMFileName);
        }

        // ──────────────────────────────────────────────
        // RepeatMode
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RepeatMode_DefaultIsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.Sound.RepeatMode);
        }

        [TestMethod]
        public void RepeatMode_CanBeSetTrue()
        {
            var src = CreateSrc();
            src.Sound.RepeatMode = true;
            Assert.IsTrue(src.Sound.RepeatMode);
        }

        [TestMethod]
        public void RepeatMode_CanBeToggled()
        {
            var src = CreateSrc();
            src.Sound.RepeatMode = true;
            src.Sound.RepeatMode = false;
            Assert.IsFalse(src.Sound.RepeatMode);
        }

        // ──────────────────────────────────────────────
        // KeepBGM
        // ──────────────────────────────────────────────

        [TestMethod]
        public void KeepBGM_DefaultIsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.Sound.KeepBGM);
        }

        [TestMethod]
        public void KeepBGM_CanBeSetTrue()
        {
            var src = CreateSrc();
            src.Sound.KeepBGM = true;
            Assert.IsTrue(src.Sound.KeepBGM);
        }

        // ──────────────────────────────────────────────
        // BossBGM
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BossBGM_DefaultIsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.Sound.BossBGM);
        }

        [TestMethod]
        public void BossBGM_CanBeSetTrue()
        {
            var src = CreateSrc();
            src.Sound.BossBGM = true;
            Assert.IsTrue(src.Sound.BossBGM);
        }

        // ──────────────────────────────────────────────
        // IsWavePlayed
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsWavePlayed_DefaultIsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.Sound.IsWavePlayed);
        }

        [TestMethod]
        public void IsWavePlayed_CanBeSetTrue()
        {
            var src = CreateSrc();
            src.Sound.IsWavePlayed = true;
            Assert.IsTrue(src.Sound.IsWavePlayed);
        }

        // ──────────────────────────────────────────────
        // Player プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Player_DefaultIsNull()
        {
            var src = CreateSrc();
            Assert.IsNull(src.Sound.Player);
        }

        // ──────────────────────────────────────────────
        // StopBGM (KeepBGM=trueの時の動作確認)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StopBGM_WithNullPlayer_DoesNotThrow()
        {
            var src = CreateSrc();
            src.Sound.Player = null;
            // Player が null の場合でも StopBGM が例外を投げないことを確認
            src.Sound.StopBGM();
        }
    }
}
