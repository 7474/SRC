using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using SRCCore.VB;
using System.Collections.Generic;
using System.Drawing;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// FillStyle, Color, DrawString関連コマンドのユニットテスト
    /// </summary>
    [TestClass]
    public class FillStyleColorCmdTests
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
        // FillStyleCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FillStyleCmd_Solid_SetsSolid()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FillStyle 塗りつぶし");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.AreEqual(FillStyle.VbFSSolid, src.Event.ObjFillStyle);
        }

        [TestMethod]
        public void FillStyleCmd_Transparent_SetsTransparent()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FillStyle 透明");
            cmd.Exec();
            Assert.AreEqual(FillStyle.VbFSTransparent, src.Event.ObjFillStyle);
        }

        [TestMethod]
        public void FillStyleCmd_HorizontalLine_SetsHorizontalLine()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FillStyle 横線");
            cmd.Exec();
            Assert.AreEqual(FillStyle.VbHorizontalLine, src.Event.ObjFillStyle);
        }

        [TestMethod]
        public void FillStyleCmd_VerticalLine_SetsVerticalLine()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FillStyle 縦線");
            cmd.Exec();
            Assert.AreEqual(FillStyle.VbVerticalLine, src.Event.ObjFillStyle);
        }

        [TestMethod]
        public void FillStyleCmd_Diagonal_SetsDiagonal()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FillStyle 斜線");
            cmd.Exec();
            Assert.AreEqual(FillStyle.VbUpwardDiagonal, src.Event.ObjFillStyle);
        }

        [TestMethod]
        public void FillStyleCmd_Diagonal2_SetsDiagonal2()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FillStyle 斜線２");
            cmd.Exec();
            Assert.AreEqual(FillStyle.VbDownwardDiagonal, src.Event.ObjFillStyle);
        }

        [TestMethod]
        public void FillStyleCmd_Cross_SetsCross()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FillStyle クロス");
            cmd.Exec();
            Assert.AreEqual(FillStyle.VbCross, src.Event.ObjFillStyle);
        }

        [TestMethod]
        public void FillStyleCmd_DiagonalCross_SetsDiagonalCross()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FillStyle 網かけ");
            cmd.Exec();
            Assert.AreEqual(FillStyle.VbDiagonalCross, src.Event.ObjFillStyle);
        }

        [TestMethod]
        public void FillStyleCmd_InvalidStyle_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FillStyle 不正");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void FillStyleCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FillStyle");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void FillStyleCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FillStyle 透明 extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ColorCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ColorCmd_ValidHexColor_SetsColor()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Color #ff0000");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            // 赤色が設定される
            Assert.AreEqual(255, src.Event.ObjColor.R);
            Assert.AreEqual(0, src.Event.ObjColor.G);
            Assert.AreEqual(0, src.Event.ObjColor.B);
        }

        [TestMethod]
        public void ColorCmd_BlackColor_SetsBlack()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Color #000000");
            cmd.Exec();
            Assert.AreEqual(0, src.Event.ObjColor.R);
            Assert.AreEqual(0, src.Event.ObjColor.G);
            Assert.AreEqual(0, src.Event.ObjColor.B);
        }

        [TestMethod]
        public void ColorCmd_WhiteColor_SetsWhite()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Color #ffffff");
            cmd.Exec();
            Assert.AreEqual(255, src.Event.ObjColor.R);
            Assert.AreEqual(255, src.Event.ObjColor.G);
            Assert.AreEqual(255, src.Event.ObjColor.B);
        }

        [TestMethod]
        public void ColorCmd_InvalidColor_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Color NotAColor");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ColorCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Color");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ColorCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Color #ff0000 extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }
    }
}
