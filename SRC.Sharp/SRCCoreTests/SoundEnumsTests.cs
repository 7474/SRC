using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Tests
{
    /// <summary>
    /// SoundType / BGMStatus / PlaySoundMode 列挙型のユニットテスト
    /// </summary>
    [TestClass]
    public class SoundEnumsTests
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
        public void SoundType_Mp3_IsThree()
        {
            Assert.AreEqual(3, (int)SoundType.Mp3);
        }

        [TestMethod]
        public void SoundType_Aiff_IsFour()
        {
            Assert.AreEqual(4, (int)SoundType.Aiff);
        }

        [TestMethod]
        public void SoundType_HasFiveMembers()
        {
            Assert.AreEqual(5, Enum.GetValues(typeof(SoundType)).Length);
        }

        [TestMethod]
        public void SoundType_IsDefined_ForAllValues()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(SoundType), SoundType.Unknown));
            Assert.IsTrue(Enum.IsDefined(typeof(SoundType), SoundType.Midi));
            Assert.IsTrue(Enum.IsDefined(typeof(SoundType), SoundType.Wave));
            Assert.IsTrue(Enum.IsDefined(typeof(SoundType), SoundType.Mp3));
            Assert.IsTrue(Enum.IsDefined(typeof(SoundType), SoundType.Aiff));
        }

        [TestMethod]
        public void SoundType_Parse_AllMembers()
        {
            Assert.AreEqual(SoundType.Unknown, Enum.Parse<SoundType>("Unknown"));
            Assert.AreEqual(SoundType.Midi, Enum.Parse<SoundType>("Midi"));
            Assert.AreEqual(SoundType.Wave, Enum.Parse<SoundType>("Wave"));
            Assert.AreEqual(SoundType.Mp3, Enum.Parse<SoundType>("Mp3"));
            Assert.AreEqual(SoundType.Aiff, Enum.Parse<SoundType>("Aiff"));
        }

        [TestMethod]
        public void SoundType_AllValuesAreUnique()
        {
            var values = Enum.GetValues(typeof(SoundType)).Cast<SoundType>().Select(v => (int)v);
            var set = new HashSet<int>(values);
            Assert.AreEqual(Enum.GetValues(typeof(SoundType)).Length, set.Count);
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
        public void BGMStatus_HasTwoMembers()
        {
            Assert.AreEqual(2, Enum.GetValues(typeof(BGMStatus)).Length);
        }

        [TestMethod]
        public void BGMStatus_IsDefined_ForAllValues()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(BGMStatus), BGMStatus.Stopped));
            Assert.IsTrue(Enum.IsDefined(typeof(BGMStatus), BGMStatus.Playing));
        }

        [TestMethod]
        public void BGMStatus_Parse_AllMembers()
        {
            Assert.AreEqual(BGMStatus.Stopped, Enum.Parse<BGMStatus>("Stopped"));
            Assert.AreEqual(BGMStatus.Playing, Enum.Parse<BGMStatus>("Playing"));
        }

        [TestMethod]
        public void BGMStatus_AllValuesAreUnique()
        {
            var values = Enum.GetValues(typeof(BGMStatus)).Cast<BGMStatus>().Select(v => (int)v);
            var set = new HashSet<int>(values);
            Assert.AreEqual(Enum.GetValues(typeof(BGMStatus)).Length, set.Count);
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
        public void PlaySoundMode_HasTwoMembers()
        {
            Assert.AreEqual(2, Enum.GetValues(typeof(PlaySoundMode)).Length);
        }

        [TestMethod]
        public void PlaySoundMode_IsDefined_ForAllValues()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(PlaySoundMode), PlaySoundMode.None));
            Assert.IsTrue(Enum.IsDefined(typeof(PlaySoundMode), PlaySoundMode.Repeat));
        }

        [TestMethod]
        public void PlaySoundMode_Parse_AllMembers()
        {
            Assert.AreEqual(PlaySoundMode.None, Enum.Parse<PlaySoundMode>("None"));
            Assert.AreEqual(PlaySoundMode.Repeat, Enum.Parse<PlaySoundMode>("Repeat"));
        }

        [TestMethod]
        public void PlaySoundMode_AllValuesAreUnique()
        {
            var values = Enum.GetValues(typeof(PlaySoundMode)).Cast<PlaySoundMode>().Select(v => (int)v);
            var set = new HashSet<int>(values);
            Assert.AreEqual(Enum.GetValues(typeof(PlaySoundMode)).Length, set.Count);
        }

        // ──────────────────────────────────────────────
        // PlaySoundConstants (CH_BGM は PlaySoundConstantsTests で検証済み)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PlaySoundConstants_CH_BGM_IsConst()
        {
            // const であることを間接的に確認（変数に代入できる）
            const int expected = PlaySoundConstants.CH_BGM;
            Assert.AreEqual(1, expected);
        }
    }
}
