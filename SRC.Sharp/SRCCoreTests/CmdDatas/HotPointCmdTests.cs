using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// HotPointCmd のユニットテスト
    /// </summary>
    [TestClass]
    public class HotPointCmdTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new Queue<string>();
            src.Event.HotPointList = new List<HotPoint>();
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
        // HotPointCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HotPointCmd_6Args_AddsHotPoint()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "HotPoint ボタン 10 20 80 30");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.AreEqual(1, src.Event.HotPointList.Count);
        }

        [TestMethod]
        public void HotPointCmd_6Args_SetsName()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "HotPoint ボタン 10 20 80 30");
            cmd.Exec();
            var hp = src.Event.HotPointList[0];
            Assert.AreEqual("ボタン", hp.Name);
        }

        [TestMethod]
        public void HotPointCmd_6Args_SetsCoordinates()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "HotPoint test 5 10 100 50");
            cmd.Exec();
            var hp = src.Event.HotPointList[0];
            Assert.AreEqual(5, hp.Left);
            Assert.AreEqual(10, hp.Top);
            Assert.AreEqual(100, hp.Width);
            Assert.AreEqual(50, hp.Height);
        }

        [TestMethod]
        public void HotPointCmd_6Args_CaptionEqualsName()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "HotPoint ok 0 0 50 20");
            cmd.Exec();
            var hp = src.Event.HotPointList[0];
            Assert.AreEqual(hp.Name, hp.Caption);
        }

        [TestMethod]
        public void HotPointCmd_7Args_SetsSeparateCaption()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "HotPoint ok 0 0 50 20 「OK」");
            cmd.Exec();
            var hp = src.Event.HotPointList[0];
            Assert.AreEqual("ok", hp.Name);
            Assert.AreEqual("「OK」", hp.Caption);
        }

        [TestMethod]
        public void HotPointCmd_7Args_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "HotPoint ok 10 20 100 50 OK表示");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void HotPointCmd_MultiplePoints_CountsCorrectly()
        {
            var src = CreateSrc();
            var cmd1 = CreateCmd(src, "HotPoint btn1 0 0 50 20", 0);
            var cmd2 = CreateCmd(src, "HotPoint btn2 60 0 50 20", 1);
            cmd1.Exec();
            cmd2.Exec();
            Assert.AreEqual(2, src.Event.HotPointList.Count);
        }

        [TestMethod]
        public void HotPointCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "HotPoint btn1 0 0");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void HotPointCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "HotPoint btn1 0 0 50 20 caption extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void HotPointCmd_WithBaseX_AdjustsLeft()
        {
            var src = CreateSrc();
            src.Event.BaseX = 100;
            var cmd = CreateCmd(src, "HotPoint test 10 0 50 20");
            cmd.Exec();
            var hp = src.Event.HotPointList[0];
            Assert.AreEqual(110, hp.Left); // 10 + BaseX(100)
        }

        [TestMethod]
        public void HotPointCmd_WithBaseY_AdjustsTop()
        {
            var src = CreateSrc();
            src.Event.BaseY = 50;
            var cmd = CreateCmd(src, "HotPoint test 0 5 50 20");
            cmd.Exec();
            var hp = src.Event.HotPointList[0];
            Assert.AreEqual(55, hp.Top); // 5 + BaseY(50)
        }
    }
}
