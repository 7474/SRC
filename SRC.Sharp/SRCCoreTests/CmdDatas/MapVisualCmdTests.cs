using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// マップビジュアルコマンドのユニットテスト
    /// (Night, Noon, Sunset, Water, Monotone, Sepia)
    /// </summary>
    [TestClass]
    public class MapVisualCmdTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new Queue<string>();
            return src;
        }

        private CmdData CreateCmd(SRC src, string cmdText, int id = 0)
        {
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, cmdText);
            src.Event.EventData.Add(line);
            var parser = new CmdParser();
            var cmd = parser.Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        // ──────────────────────────────────────────────
        // NightCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NightCmd_NoArgs_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Night");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void NightCmd_WithAsync_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Night 非同期");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void NightCmd_WithMapOnly_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Night マップ限定");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.IsTrue(src.Map.MapDrawIsMapOnly);
        }

        [TestMethod]
        public void NightCmd_WithInvalidOption_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Night 不正オプション");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // NoonCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NoonCmd_NoArgs_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Noon");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void NoonCmd_WithAsync_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Noon 非同期");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void NoonCmd_WithInvalidOption_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Noon 不正");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SunsetCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SunsetCmd_NoArgs_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Sunset");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void SunsetCmd_WithAsync_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Sunset 非同期");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void SunsetCmd_WithMapOnly_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Sunset マップ限定");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void SunsetCmd_WithInvalidOption_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Sunset 不正");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // WaterCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WaterCmd_NoArgs_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Water");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void WaterCmd_WithAsync_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Water 非同期");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void WaterCmd_WithInvalidOption_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Water 不正");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // MonotoneCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MonotoneCmd_NoArgs_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Monotone");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void MonotoneCmd_WithAsync_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Monotone 非同期");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void MonotoneCmd_WithMapOnly_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Monotone マップ限定");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void MonotoneCmd_WithInvalidOption_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Monotone 不正");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SepiaCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SepiaCmd_NoArgs_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Sepia");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void SepiaCmd_WithAsync_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Sepia 非同期");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void SepiaCmd_WithMapOnly_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Sepia マップ限定");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void SepiaCmd_WithInvalidOption_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Sepia 不正");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // NightCmd - MapDrawIsMapOnly の状態確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NightCmd_WithoutMapOnly_MapDrawIsMapOnlyIsFalse()
        {
            var src = CreateSrc();
            src.Map.MapDrawIsMapOnly = true;
            var cmd = CreateCmd(src, "Night");
            cmd.Exec();
            Assert.IsFalse(src.Map.MapDrawIsMapOnly);
        }

        [TestMethod]
        public void NoonCmd_MapDrawIsMapOnlyIsFalse()
        {
            var src = CreateSrc();
            src.Map.MapDrawIsMapOnly = true;
            var cmd = CreateCmd(src, "Noon");
            cmd.Exec();
            Assert.IsFalse(src.Map.MapDrawIsMapOnly);
        }
    }
}
