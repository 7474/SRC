using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using SRCCore.VB;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.Tests
{
    /// <summary>
    /// SRCSaveData / SRCSuspendData クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class SRCSaveDataTests
    {
        // ──────────────────────────────────────────────
        // SRCSaveData プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SRCSaveData_Version_CanBeSetAndRead()
        {
            var sd = new SRCSaveData { Version = "1.0.0" };
            Assert.AreEqual("1.0.0", sd.Version);
        }

        [TestMethod]
        public void SRCSaveData_Kind_CanBeSetAndRead()
        {
            var sd = new SRCSaveData { Kind = SRCSaveKind.Suspend };
            Assert.AreEqual(SRCSaveKind.Suspend, sd.Kind);
        }

        [TestMethod]
        public void SRCSaveData_NextStage_CanBeSetAndRead()
        {
            var sd = new SRCSaveData { NextStage = "stage2" };
            Assert.AreEqual("stage2", sd.NextStage);
        }

        [TestMethod]
        public void SRCSaveData_TotalTurn_CanBeSetAndRead()
        {
            var sd = new SRCSaveData { TotalTurn = 42 };
            Assert.AreEqual(42, sd.TotalTurn);
        }

        [TestMethod]
        public void SRCSaveData_Money_CanBeSetAndRead()
        {
            var sd = new SRCSaveData { Money = 9999 };
            Assert.AreEqual(9999, sd.Money);
        }

        [TestMethod]
        public void SRCSaveData_Titles_CanBeSetAndRead()
        {
            var titles = new List<string> { "タイトル1", "タイトル2" };
            var sd = new SRCSaveData { Titles = titles };
            Assert.AreEqual(2, sd.Titles.Count);
            Assert.AreEqual("タイトル1", sd.Titles[0]);
        }

        [TestMethod]
        public void SRCSaveData_DefaultKind_IsNormal()
        {
            var sd = new SRCSaveData();
            Assert.AreEqual(SRCSaveKind.Normal, sd.Kind);
        }

        // ──────────────────────────────────────────────
        // SRCSuspendData プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SRCSuspendData_ScenarioFileName_CanBeSetAndRead()
        {
            var sd = new SRCSuspendData { ScenarioFileName = "test_scenario.eve" };
            Assert.AreEqual("test_scenario.eve", sd.ScenarioFileName);
        }

        [TestMethod]
        public void SRCSuspendData_Turn_CanBeSetAndRead()
        {
            var sd = new SRCSuspendData { Turn = 5 };
            Assert.AreEqual(5, sd.Turn);
        }

        [TestMethod]
        public void SRCSuspendData_MapX_CanBeSetAndRead()
        {
            var sd = new SRCSuspendData { MapX = 10 };
            Assert.AreEqual(10, sd.MapX);
        }

        [TestMethod]
        public void SRCSuspendData_MapY_CanBeSetAndRead()
        {
            var sd = new SRCSuspendData { MapY = 7 };
            Assert.AreEqual(7, sd.MapY);
        }

        [TestMethod]
        public void SRCSuspendData_RndSeed_CanBeSetAndRead()
        {
            var sd = new SRCSuspendData { RndSeed = 12345 };
            Assert.AreEqual(12345, sd.RndSeed);
        }

        [TestMethod]
        public void SRCSuspendData_RndIndex_CanBeSetAndRead()
        {
            var sd = new SRCSuspendData { RndIndex = 99 };
            Assert.AreEqual(99, sd.RndIndex);
        }

        [TestMethod]
        public void SRCSuspendData_BGMFileName_CanBeSetAndRead()
        {
            var sd = new SRCSuspendData { BGMFileName = "battle.mp3" };
            Assert.AreEqual("battle.mp3", sd.BGMFileName);
        }

        [TestMethod]
        public void SRCSuspendData_RepeatMode_CanBeSetAndRead()
        {
            var sd = new SRCSuspendData { RepeatMode = true };
            Assert.IsTrue(sd.RepeatMode);
        }

        [TestMethod]
        public void SRCSuspendData_InheritsFromSRCSaveData()
        {
            var sd = new SRCSuspendData { Version = "2.0.0", Kind = SRCSaveKind.Suspend };
            Assert.AreEqual("2.0.0", sd.Version);
            Assert.AreEqual(SRCSaveKind.Suspend, sd.Kind);
        }
    }
}
