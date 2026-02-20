using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    [TestClass]
    public class PortedCommandTests
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
            src.Event.EventQue = new System.Collections.Generic.Queue<string>();
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
        // PaintSysStringCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PaintSysStringCmd_CallsDrawSysString()
        {
            var src = CreateSrc();
            var gui = (MockGUI)src.GUI;
            string capturedMsg = null;
            int capturedX = -99, capturedY = -99;
            bool capturedWithoutRefresh = true;
            gui.DrawSysStringHandler = (x, y, msg, wr) =>
            {
                capturedX = x;
                capturedY = y;
                capturedMsg = msg;
                capturedWithoutRefresh = wr;
            };

            var cmd = CreateCmd(src, "PaintSysString 3 5 \"こんにちは\"");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.AreEqual(3, capturedX);
            Assert.AreEqual(5, capturedY);
            Assert.AreEqual("こんにちは", capturedMsg);
            Assert.IsFalse(capturedWithoutRefresh);
        }

        [TestMethod]
        public void PaintSysStringCmd_AsyncOption_SetsWithoutRefresh()
        {
            var src = CreateSrc();
            var gui = (MockGUI)src.GUI;
            bool capturedWithoutRefresh = false;
            gui.DrawSysStringHandler = (x, y, msg, wr) => capturedWithoutRefresh = wr;

            var cmd = CreateCmd(src, "PaintSysString 1 2 \"テスト\" 非同期");
            cmd.Exec();

            Assert.IsTrue(capturedWithoutRefresh);
        }

        [TestMethod]
        public void PaintSysStringCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "PaintSysString 1 2");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SetStatusStringColorCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetStatusStringColorCmd_NormalTarget_StoresGlobalVar()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetStatusStringColor #FF0000 通常");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("StatusWindow(StringColor)"));
            // #FF0000 → COLORREF 0x0000FF (R=0xFF, G=0x00, B=0x00 → BBGGRR = 0x0000FF)
            Assert.AreEqual(255, src.Expression.GetValueAsLong("StatusWindow(StringColor)"));
        }

        [TestMethod]
        public void SetStatusStringColorCmd_AbilityNameTarget_StoresGlobalVar()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetStatusStringColor #00FF00 能力名");
            cmd.Exec();

            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("StatusWindow(ANameColor)"));
            // #00FF00 → COLORREF = 0x00FF00 (R=0, G=0xFF, B=0 → BBGGRR = 0x00FF00)
            Assert.AreEqual(65280, src.Expression.GetValueAsLong("StatusWindow(ANameColor)"));
        }

        [TestMethod]
        public void SetStatusStringColorCmd_InvalidTarget_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetStatusStringColor #FF0000 無効な対象");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void SetStatusStringColorCmd_InvalidColorFormat_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetStatusStringColor NOTACOLOR 通常");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SetWindowColorCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetWindowColorCmd_NoTarget_SetsFrameAndBackground()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetWindowColor #0000FF");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            // #0000FF → COLORREF = 0xFF0000 (R=0, G=0, B=0xFF → BBGGRR = 0xFF0000)
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("StatusWindow(FrameColor)"));
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("StatusWindow(BackBolor)"));
            Assert.AreEqual(16711680, src.Expression.GetValueAsLong("StatusWindow(FrameColor)"));
            Assert.AreEqual(16711680, src.Expression.GetValueAsLong("StatusWindow(BackBolor)"));
        }

        [TestMethod]
        public void SetWindowColorCmd_FrameTarget_OnlySetsFrame()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetWindowColor #FF0000 枠");
            cmd.Exec();

            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("StatusWindow(FrameColor)"));
            Assert.IsFalse(src.Expression.IsGlobalVariableDefined("StatusWindow(BackBolor)"));
        }

        [TestMethod]
        public void SetWindowColorCmd_BackgroundTarget_OnlySetsBackground()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetWindowColor #FF0000 背景");
            cmd.Exec();

            Assert.IsFalse(src.Expression.IsGlobalVariableDefined("StatusWindow(FrameColor)"));
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("StatusWindow(BackBolor)"));
        }

        [TestMethod]
        public void SetWindowColorCmd_InvalidTarget_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetWindowColor #FF0000 不明");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SetWindowFrameWidthCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetWindowFrameWidthCmd_StoresWidthInGlobalVar()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetWindowFrameWidth 3");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("StatusWindow(FrameWidth)"));
            Assert.AreEqual(3, src.Expression.GetValueAsLong("StatusWindow(FrameWidth)"));
        }

        [TestMethod]
        public void SetWindowFrameWidthCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetWindowFrameWidth");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ClearImageCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ClearImageCmd_CallsClearPicture()
        {
            var src = CreateSrc();
            var gui = (MockGUI)src.GUI;
            var called = false;
            gui.ClearPictureHandler = () => called = true;

            var cmd = CreateCmd(src, "ClearImage");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.IsTrue(called);
        }

        // ──────────────────────────────────────────────
        // ShowImageCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ShowImageCmd_CallsDrawPictureWithDefaultSize()
        {
            var src = CreateSrc();
            var gui = (MockGUI)src.GUI;
            string capturedFname = null;
            int capturedDw = 0, capturedDh = 0;
            gui.DrawPictureHandler = (fname, dx, dy, dw, dh, sx, sy, sw, sh, opt) =>
            {
                capturedFname = fname;
                capturedDw = dw;
                capturedDh = dh;
                return true;
            };

            var cmd = CreateCmd(src, "ShowImage test.bmp");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.AreEqual("test.bmp", capturedFname);
            Assert.AreEqual(Constants.DEFAULT_LEVEL, capturedDw);
            Assert.AreEqual(Constants.DEFAULT_LEVEL, capturedDh);
        }

        [TestMethod]
        public void ShowImageCmd_CallsDrawPictureWithExplicitSize()
        {
            var src = CreateSrc();
            var gui = (MockGUI)src.GUI;
            int capturedDw = 0, capturedDh = 0;
            gui.DrawPictureHandler = (fname, dx, dy, dw, dh, sx, sy, sw, sh, opt) =>
            {
                capturedDw = dw;
                capturedDh = dh;
                return true;
            };

            var cmd = CreateCmd(src, "ShowImage test.png 320 240");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.AreEqual(320, capturedDw);
            Assert.AreEqual(240, capturedDh);
        }

        [TestMethod]
        public void ShowImageCmd_InvalidExtension_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ShowImage test.txt");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ReadCmd / WriteCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WriteCmd_WritesValuesToFile()
        {
            var src = CreateSrc();
            using var ms = new System.IO.MemoryStream();
            var handle = src.FileHandleManager.Add(
                SRCCore.Filesystem.SafeOpenMode.Write,
                SRCCompatibilityMode.None,
                ms);

            var cmd = CreateCmd(src, $"Write {handle.Handle} \"Hello\" \"World\"");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            handle.Writer.Flush();
            var written = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            Assert.IsTrue(written.Contains("Hello"));
            Assert.IsTrue(written.Contains("World"));
        }

        [TestMethod]
        public void ReadCmd_ReadsValuesFromFile()
        {
            var src = CreateSrc();
            var content = System.Text.Encoding.UTF8.GetBytes("Hello\r\nWorld\r\n");
            using var ms = new System.IO.MemoryStream(content);
            var handle = src.FileHandleManager.Add(
                SRCCore.Filesystem.SafeOpenMode.Read,
                SRCCompatibilityMode.None,
                ms);

            var cmd = CreateCmd(src, $"Read {handle.Handle} A B");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.AreEqual("Hello", src.Expression.GetValueAsString("A"));
            Assert.AreEqual("World", src.Expression.GetValueAsString("B"));
        }

        [TestMethod]
        public void ReadCmd_TooFewArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Read 1");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void WriteCmd_TooFewArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Write 1");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }
    }
}
