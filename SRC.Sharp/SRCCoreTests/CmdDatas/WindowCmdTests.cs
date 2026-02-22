using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// ウィンドウ設定コマンドのユニットテスト
    /// (SetWindowColor, SetWindowFrameWidth)
    /// </summary>
    [TestClass]
    public class WindowCmdTests
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
        // SetWindowFrameWidthCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetWindowFrameWidthCmd_SetsFrameWidth()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetWindowFrameWidth 3");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("StatusWindow(FrameWidth)"));
            Assert.AreEqual(3, src.Expression.GetValueAsLong("StatusWindow(FrameWidth)"));
        }

        [TestMethod]
        public void SetWindowFrameWidthCmd_Width1_IsOne()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetWindowFrameWidth 1");
            cmd.Exec();
            Assert.AreEqual(1, src.Expression.GetValueAsLong("StatusWindow(FrameWidth)"));
        }

        [TestMethod]
        public void SetWindowFrameWidthCmd_Width0_IsZero()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetWindowFrameWidth 0");
            cmd.Exec();
            Assert.AreEqual(0, src.Expression.GetValueAsLong("StatusWindow(FrameWidth)"));
        }

        [TestMethod]
        public void SetWindowFrameWidthCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetWindowFrameWidth");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void SetWindowFrameWidthCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetWindowFrameWidth 1 2");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SetWindowColorCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetWindowColorCmd_NoTarget_SetsBothFrameAndBgColor()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetWindowColor #ff0000");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("StatusWindow(FrameColor)"));
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("StatusWindow(BackBolor)"));
        }

        [TestMethod]
        public void SetWindowColorCmd_WithFrameTarget_SetsFrameColor()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetWindowColor #00ff00 枠");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("StatusWindow(FrameColor)"));
        }

        [TestMethod]
        public void SetWindowColorCmd_WithBgTarget_SetsBgColor()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetWindowColor #0000ff 背景");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("StatusWindow(BackBolor)"));
        }

        [TestMethod]
        public void SetWindowColorCmd_InvalidColor_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetWindowColor NotAColor");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void SetWindowColorCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetWindowColor");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void SetWindowColorCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetWindowColor #ff0000 枠 extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void SetWindowColorCmd_InvalidTarget_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetWindowColor #ff0000 不正ターゲット");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // 色変換の検証 (#rrggbb → COLORREF bgr)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetWindowColorCmd_RedColor_ConvertsToColorRef()
        {
            var src = CreateSrc();
            // #ff0000 (R=0xFF, G=0, B=0) → COLORREF = 0x0000FF (B=0, G=0, R=0xFF)
            var cmd = CreateCmd(src, "SetWindowColor #ff0000");
            cmd.Exec();
            Assert.AreEqual(255, src.Expression.GetValueAsLong("StatusWindow(FrameColor)"));
        }

        [TestMethod]
        public void SetWindowColorCmd_BlueColor_ConvertsToColorRef()
        {
            var src = CreateSrc();
            // #0000ff (R=0, G=0, B=0xFF) → COLORREF = 0xFF0000 (B=0xFF, G=0, R=0) = 16711680
            var cmd = CreateCmd(src, "SetWindowColor #0000ff");
            cmd.Exec();
            Assert.AreEqual(16711680, src.Expression.GetValueAsLong("StatusWindow(FrameColor)"));
        }

        [TestMethod]
        public void SetWindowColorCmd_BlackColor_IsZero()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetWindowColor #000000");
            cmd.Exec();
            Assert.AreEqual(0, src.Expression.GetValueAsLong("StatusWindow(FrameColor)"));
        }
    }
}
