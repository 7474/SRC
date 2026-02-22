using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// 描画オプション/表示コマンドのユニットテスト
    /// (DrawOption, DrawWidth, UpVar, ColorFilter, ClearLayer)
    /// </summary>
    [TestClass]
    public class DrawCmdTests
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
        // DrawOptionCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DrawOptionCmd_SetsObjDrawOption()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "DrawOption 透過");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.AreEqual("透過", src.Event.ObjDrawOption);
        }

        [TestMethod]
        public void DrawOptionCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "DrawOption");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void DrawOptionCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "DrawOption a b");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void DrawOptionCmd_EmptyOption_SetsEmpty()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "DrawOption \"\"");
            cmd.Exec();
            Assert.AreEqual("", src.Event.ObjDrawOption);
        }

        // ──────────────────────────────────────────────
        // DrawWidthCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DrawWidthCmd_SetsObjDrawWidth()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "DrawWidth 3");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.AreEqual(3, src.Event.ObjDrawWidth);
        }

        [TestMethod]
        public void DrawWidthCmd_SetWidth1_IsOne()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "DrawWidth 1");
            cmd.Exec();
            Assert.AreEqual(1, src.Event.ObjDrawWidth);
        }

        [TestMethod]
        public void DrawWidthCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "DrawWidth");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void DrawWidthCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "DrawWidth 1 2");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // UpVarCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UpVarCmd_IncreasesUpVarLevel()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "UpVar x");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.AreEqual(1, src.Event.UpVarLevel);
        }

        [TestMethod]
        public void UpVarCmd_CalledTwice_LevelIsTwo()
        {
            var src = CreateSrc();
            var cmd1 = CreateCmd(src, "UpVar x", 0);
            var cmd2 = CreateCmd(src, "UpVar y", 1);
            cmd1.Exec();
            cmd2.Exec();
            Assert.AreEqual(2, src.Event.UpVarLevel);
        }

        [TestMethod]
        public void UpVarCmd_InitialLevelIsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Event.UpVarLevel);
        }

        // ──────────────────────────────────────────────
        // ColorFilterCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ColorFilterCmd_WithValidHexColor_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ColorFilter #ff0000");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ColorFilterCmd_WithColorAndAsync_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ColorFilter #0000ff 非同期");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ColorFilterCmd_WithColorAndMapOnly_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ColorFilter #00ff00 マップ限定");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ColorFilterCmd_WithColorAndTransparency_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ColorFilter #ff0000 50%");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ColorFilterCmd_WithInvalidColor_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ColorFilter notacolor");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ColorFilterCmd_NoArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ColorFilter");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ColorFilterCmd_WithInvalidOption_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ColorFilter #ff0000 不正オプション");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }
    }
}
