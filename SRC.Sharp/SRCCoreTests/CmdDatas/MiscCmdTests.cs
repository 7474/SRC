using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// Money/Debug/Global コマンドのユニットテスト
    /// </summary>
    [TestClass]
    public class MiscCmdTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC
            {
                GUI = new MockGUI(),
            };
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
        // MoneyCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MoneyCmd_IncreasesMoney()
        {
            var src = CreateSrc();
            src.Money = 1000;
            var cmd = CreateCmd(src, "Money 500");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.AreEqual(1500, src.Money);
        }

        [TestMethod]
        public void MoneyCmd_DecreasesMoney()
        {
            var src = CreateSrc();
            src.Money = 1000;
            var cmd = CreateCmd(src, "Money -500");
            cmd.Exec();
            Assert.AreEqual(500, src.Money);
        }

        [TestMethod]
        public void MoneyCmd_ClampsToZero_WhenMoneyGoesNegative()
        {
            var src = CreateSrc();
            src.Money = 100;
            var cmd = CreateCmd(src, "Money -1000");
            cmd.Exec();
            Assert.AreEqual(0, src.Money);
        }

        [TestMethod]
        public void MoneyCmd_ClampsToMax()
        {
            var src = CreateSrc();
            src.Money = 999999000;
            var cmd = CreateCmd(src, "Money 9999");
            cmd.Exec();
            Assert.AreEqual(999999999, src.Money);
        }

        // ──────────────────────────────────────────────
        // DebugCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DebugCmd_ExecutesAndReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Debug テストメッセージ", 5);
            var result = cmd.Exec();
            // NextID = ID + 1 = 6
            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void DebugCmd_WithMultipleArgs_ExecutesSuccessfully()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Debug arg1 arg2 arg3", 3);
            var result = cmd.Exec();
            Assert.AreEqual(4, result);
        }

        // ──────────────────────────────────────────────
        // GlobalCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GlobalCmd_DefinesGlobalVariable()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Global myVar");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("myVar"));
        }

        [TestMethod]
        public void GlobalCmd_WithInitialValue_SetsValue()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Global score = 100");
            cmd.Exec();
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("score"));
        }
    }
}
