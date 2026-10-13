using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.Drawing;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// FontCmd のユニットテスト
    /// </summary>
    [TestClass]
    public class FontCmdTests
    {
        private (SRC src, MockGUI gui) CreateSrc()
        {
            var gui = new MockGUI
            {
                CurrentPaintFont = new PaintFont { Name = "ＭＳ Ｐ明朝", Size = 16, Bold = true, Italic = false },
                CurrentPaintColor = Color.White,
            };
            var src = new SRC { GUI = gui };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            return (src, gui);
        }

        private CmdData CreateCmd(SRC src, string cmdText, int id = 0)
        {
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, cmdText);
            src.Event.EventData.Add(line);
            var cmd = new CmdParser().Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        [TestMethod]
        public void FontCmd_NoArgs_ResetsDrawString()
        {
            var (src, gui) = CreateSrc();
            var reset = false;
            DrawFontOption captured = null;
            gui.ResetDrawStringHandler = () => reset = true;
            gui.SetDrawFontHandler = o => captured = o;
            var result = CreateCmd(src, "Font").Exec();
            Assert.AreEqual(1, result);
            Assert.IsTrue(reset);
            Assert.IsNotNull(captured);
            Assert.AreEqual("ＭＳ Ｐ明朝", captured.FontFamily);
            Assert.AreEqual(16f, captured.Size);
            Assert.IsTrue(captured.Bold);
            Assert.IsFalse(captured.Italic);
            Assert.AreEqual(Color.White, captured.Color);
        }

        [TestMethod]
        public void FontCmd_SizeAndFamily_StartsFromCurrentPaintFont()
        {
            var (src, gui) = CreateSrc();
            DrawFontOption captured = null;
            gui.SetDrawFontHandler = o => captured = o;
            var result = CreateCmd(src, "Font 12pt ゴシック Italic").Exec();
            Assert.AreEqual(1, result);
            Assert.AreEqual("ＭＳ ゴシック", captured.FontFamily);
            Assert.AreEqual(12f, captured.Size);
            Assert.IsTrue(captured.Bold);
            Assert.IsTrue(captured.Italic);
        }

        [TestMethod]
        public void FontCmd_Color_SetsColor()
        {
            var (src, gui) = CreateSrc();
            DrawFontOption captured = null;
            gui.SetDrawFontHandler = o => captured = o;
            CreateCmd(src, "Font #ff0000").Exec();
            Assert.AreEqual(Color.FromArgb(255, 0, 0).ToArgb(), captured.Color.ToArgb());
        }
    }
}
