using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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
        public void FontCmd_NoArgs_ResetsFromChangedFont()
        {
            // 元の SRC（CmdData.cls ExecFontCmd）は引数が無ければ白・ＭＳ Ｐ明朝・16pt・太字・斜体なしに戻す。
            // いまの書式が既定と違っても、既定に戻す
            var (src, gui) = CreateSrc();
            gui.CurrentPaintFont = new PaintFont { Name = "ＭＳ ゴシック", Size = 24, Bold = false, Italic = true };
            gui.CurrentPaintColor = Color.Red;
            gui.ResetDrawStringHandler = () => { };
            DrawFontOption captured = null;
            gui.SetDrawFontHandler = o => captured = o;
            Assert.AreEqual(1, CreateCmd(src, "Font").Exec());
            Assert.IsNotNull(captured);
            Assert.AreEqual("ＭＳ Ｐ明朝", captured.FontFamily);
            Assert.AreEqual(16f, captured.Size);
            Assert.IsTrue(captured.Bold);
            Assert.IsFalse(captured.Italic);
            Assert.AreEqual(Color.White.ToArgb(), captured.Color.ToArgb());
        }

        [TestMethod]
        public void FontCmd_Size_RoundsToIntegerLikeCInt()
        {
            // 元の SRC は .Font.Size = CInt(opt) なので、小数の大きさは偶数丸めで整数にする
            var (src, gui) = CreateSrc();
            DrawFontOption captured = null;
            gui.SetDrawFontHandler = o => captured = o;
            Assert.AreEqual(1, CreateCmd(src, "Font 12.5pt").Exec());
            Assert.AreEqual(12f, captured.Size);
            Assert.AreEqual(2, CreateCmd(src, "Font 13.5pt", 1).Exec());
            Assert.AreEqual(14f, captured.Size);
        }

        [TestMethod]
        public void FontCmd_NonNumericSize_IsError()
        {
            // 元の SRC は CInt("abc") が型の不一致の実行時エラーになり、「イベントデータが不正です」で止まる
            var (src, gui) = CreateSrc();
            DrawFontOption captured = null;
            gui.SetDrawFontHandler = o => captured = o;
            Assert.AreEqual(-1, CreateCmd(src, "Font abcpt").Exec());
            Assert.IsNull(captured);
            Assert.AreEqual("イベントデータが不正です", src.DataErrors.Last().msg);
        }

        [TestMethod]
        public void Exec_StopSignal_IsNotEventError()
        {
            // ゲームスレッドを止める合図（OperationCanceledException。シナリオの終了・画面を閉じたとき）は、
            // 「イベントデータが不正です」にせずに外へ通す（握りつぶすと、SRC#Web で GameClear の後もゲームが進んだ。#1552）
            var (src, gui) = CreateSrc();
            gui.SetDrawFontHandler = o => throw new System.OperationCanceledException();
            Assert.ThrowsExactly<System.OperationCanceledException>(() => CreateCmd(src, "Font 12pt").Exec());
            Assert.AreEqual(0, src.DataErrors.Count);
        }

        [TestMethod]
        public void FontCmd_Regular_ClearsBoldAndItalic()
        {
            // 元の SRC（CmdData.cls の ExecFontCmd）は Regular で太字・斜体を解除する（#905）
            var (src, gui) = CreateSrc();
            gui.CurrentPaintFont = new PaintFont { Name = "ＭＳ Ｐ明朝", Size = 16, Bold = true, Italic = true };
            DrawFontOption captured = null;
            gui.SetDrawFontHandler = o => captured = o;
            CreateCmd(src, "Font Regular").Exec();
            Assert.IsNotNull(captured);
            Assert.IsFalse(captured.Bold);
            Assert.IsFalse(captured.Italic);
        }

        [TestMethod]
        public void FontCmd_BoldAfterRegular_SetsBoldOnly()
        {
            var (src, gui) = CreateSrc();
            DrawFontOption captured = null;
            gui.SetDrawFontHandler = o => captured = o;
            CreateCmd(src, "Font Regular Bold").Exec();
            Assert.IsTrue(captured.Bold);
            Assert.IsFalse(captured.Italic);
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
