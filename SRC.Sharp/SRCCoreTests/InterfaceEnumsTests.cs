using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;

namespace SRCCore.Tests
{
    /// <summary>
    /// SoundType / BGMStatus / PlaySoundMode / DrawStringMode / TransionPattern / GuiStatus / SRCSaveKind enum のユニットテスト
    /// </summary>
    [TestClass]
    public class InterfaceEnumsTests
    {
        // ──────────────────────────────────────────────
        // SoundType
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SoundType_Unknown_IsZero()
        {
            Assert.AreEqual(0, (int)SoundType.Unknown);
        }

        [TestMethod]
        public void SoundType_Midi_IsOne()
        {
            Assert.AreEqual(1, (int)SoundType.Midi);
        }

        [TestMethod]
        public void SoundType_Wave_IsTwo()
        {
            Assert.AreEqual(2, (int)SoundType.Wave);
        }

        [TestMethod]
        public void SoundType_HasFiveValues()
        {
            Assert.AreEqual(5, System.Enum.GetValues(typeof(SoundType)).Length);
        }

        // ──────────────────────────────────────────────
        // BGMStatus
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BGMStatus_Stopped_IsZero()
        {
            Assert.AreEqual(0, (int)BGMStatus.Stopped);
        }

        [TestMethod]
        public void BGMStatus_Playing_IsOne()
        {
            Assert.AreEqual(1, (int)BGMStatus.Playing);
        }

        [TestMethod]
        public void BGMStatus_HasTwoValues()
        {
            Assert.AreEqual(2, System.Enum.GetValues(typeof(BGMStatus)).Length);
        }

        // ──────────────────────────────────────────────
        // PlaySoundMode
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PlaySoundMode_None_IsZero()
        {
            Assert.AreEqual(0, (int)PlaySoundMode.None);
        }

        [TestMethod]
        public void PlaySoundMode_Repeat_IsOne()
        {
            Assert.AreEqual(1, (int)PlaySoundMode.Repeat);
        }

        [TestMethod]
        public void PlaySoundMode_HasTwoValues()
        {
            Assert.AreEqual(2, System.Enum.GetValues(typeof(PlaySoundMode)).Length);
        }

        // ──────────────────────────────────────────────
        // DrawStringMode
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DrawStringMode_Default_IsZero()
        {
            Assert.AreEqual(0, (int)DrawStringMode.Default);
        }

        [TestMethod]
        public void DrawStringMode_Status_IsOne()
        {
            Assert.AreEqual(1, (int)DrawStringMode.Status);
        }

        [TestMethod]
        public void DrawStringMode_HasTwoValues()
        {
            Assert.AreEqual(2, System.Enum.GetValues(typeof(DrawStringMode)).Length);
        }

        // ──────────────────────────────────────────────
        // TransionPattern
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TransionPattern_FadeIn_IsZero()
        {
            Assert.AreEqual(0, (int)TransionPattern.FadeIn);
        }

        [TestMethod]
        public void TransionPattern_FadeOut_IsOne()
        {
            Assert.AreEqual(1, (int)TransionPattern.FadeOut);
        }

        [TestMethod]
        public void TransionPattern_HasTwoValues()
        {
            Assert.AreEqual(2, System.Enum.GetValues(typeof(TransionPattern)).Length);
        }

        // ──────────────────────────────────────────────
        // GuiStatus
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GuiStatus_Default_IsZero()
        {
            Assert.AreEqual(0, (int)GuiStatus.Default);
        }

        [TestMethod]
        public void GuiStatus_WaitCursor_IsOne()
        {
            Assert.AreEqual(1, (int)GuiStatus.WaitCursor);
        }

        [TestMethod]
        public void GuiStatus_IBeam_IsTwo()
        {
            Assert.AreEqual(2, (int)GuiStatus.IBeam);
        }

        [TestMethod]
        public void GuiStatus_HasThreeValues()
        {
            Assert.AreEqual(3, System.Enum.GetValues(typeof(GuiStatus)).Length);
        }

        // ──────────────────────────────────────────────
        // SRCSaveKind
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SRCSaveKind_Normal_IsZero()
        {
            Assert.AreEqual(0, (int)SRCSaveKind.Normal);
        }

        [TestMethod]
        public void SRCSaveKind_Suspend_IsOne()
        {
            Assert.AreEqual(1, (int)SRCSaveKind.Suspend);
        }

        [TestMethod]
        public void SRCSaveKind_Quik_IsTwo()
        {
            Assert.AreEqual(2, (int)SRCSaveKind.Quik);
        }

        [TestMethod]
        public void SRCSaveKind_Restart_IsThree()
        {
            Assert.AreEqual(3, (int)SRCSaveKind.Restart);
        }

        [TestMethod]
        public void SRCSaveKind_HasFourValues()
        {
            Assert.AreEqual(4, System.Enum.GetValues(typeof(SRCSaveKind)).Length);
        }
    }
}
