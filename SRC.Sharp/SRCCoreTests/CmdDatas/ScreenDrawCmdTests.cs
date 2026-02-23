using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// 画面描画・エフェクト系コマンドのユニットテスト
    /// (Exit, FillColor, FadeIn, FadeOut, WhiteIn, WhiteOut, Arc, Circle, Oval, Line, Polygon, PSet, Explode)
    /// ヘルプの記載を期待値として検証する
    /// </summary>
    [TestClass]
    public class ScreenDrawCmdTests
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
        // ExitCmd
        // ヘルプ: そのイベントを終了します。
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExitCmd_NoArgs_ReturnsMinusOne()
        {
            // ヘルプ: イベントを終了します
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Exit");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // FillColorCmd
        // ヘルプ: 塗りつぶし色を変更します
        // 書式: FillColor rgb
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FillColorCmd_ValidColor_SetsObjFillColor()
        {
            // ヘルプ: FillColor rgb — 塗りつぶし色をrgbに変更する
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FillColor #FF0000");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            // #FF0000 → COLORREF 方式で赤
            Assert.AreEqual(255, src.Event.ObjFillColor.R);
            Assert.AreEqual(0, src.Event.ObjFillColor.G);
            Assert.AreEqual(0, src.Event.ObjFillColor.B);
        }

        [TestMethod]
        public void FillColorCmd_BlackColor_SetsBlack()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FillColor #000000");
            cmd.Exec();
            Assert.AreEqual(0, src.Event.ObjFillColor.R);
            Assert.AreEqual(0, src.Event.ObjFillColor.G);
            Assert.AreEqual(0, src.Event.ObjFillColor.B);
        }

        [TestMethod]
        public void FillColorCmd_InvalidColor_ReturnsError()
        {
            // ヘルプ: rgbはRGB16進で指定
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FillColor NOTACOLOR");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void FillColorCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FillColor");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void FillColorCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FillColor #FF0000 #00FF00");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // FadeInCmd
        // ヘルプ: 画面をフェードイン(真っ黒な画面から段階的に画像を浮かび上がらせる)します
        // 書式: FadeIn [times]
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FadeInCmd_NoArgs_ReturnsNextId()
        {
            // ヘルプ: timesを省略した場合は10段階でフェードイン
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FadeIn", 5);
            var result = cmd.Exec();
            // GUI.IsRButtonPressed() に対するハンドラが未注入のため GUINotImplementedException がスローされ NextID が返る
            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void FadeInCmd_WithTimes_ReturnsNextId()
        {
            // ヘルプ: timesを指定すると指定した段階数でフェードイン
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FadeIn 20", 3);
            var result = cmd.Exec();
            Assert.AreEqual(4, result);
        }

        // ──────────────────────────────────────────────
        // FadeOutCmd
        // ヘルプ: 画面をフェードアウトします
        // 書式: FadeOut [times]
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FadeOutCmd_NoArgs_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FadeOut", 2);
            var result = cmd.Exec();
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void FadeOutCmd_WithTimes_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FadeOut 5", 0);
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void FadeOutCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FadeOut 10 20");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // WhiteInCmd
        // ヘルプ: 画面をホワイトイン(真っ白な画面から段階的に画像を浮かび上がらせる)します
        // 書式: WhiteIn [times]
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WhiteInCmd_NoArgs_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "WhiteIn", 0);
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void WhiteInCmd_WithTimes_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "WhiteIn 15", 0);
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        // ──────────────────────────────────────────────
        // WhiteOutCmd
        // ヘルプ: 画面をホワイトアウトします
        // 書式: WhiteOut [times]
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WhiteOutCmd_NoArgs_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "WhiteOut", 0);
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void WhiteOutCmd_WithTimes_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "WhiteOut 5", 0);
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void WhiteOutCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "WhiteOut 10 20");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ArcCmd
        // ヘルプ: Arc x y r start end [color] — 円弧を描画
        // 書式: Arc x y r start end [color]
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArcCmd_TooFewArgs_ReturnsError()
        {
            // ヘルプ: Arc x y r start end [color] — 引数が5個未満はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Arc 100 100 50 0");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ArcCmd_ValidArgs_ReturnsNextId()
        {
            // ヘルプ: Arc x y r start end — 円弧を描画
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Arc 100 100 50 0 90", 0);
            var result = cmd.Exec();
            // GUI.SaveScreen() に対するハンドラが未注入のため GUINotImplementedException がスローされ NextID が返る
            Assert.AreEqual(1, result);
        }

        // ──────────────────────────────────────────────
        // CircleCmd
        // ヘルプ: Circle x y r [color] — 円を描画
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CircleCmd_TooFewArgs_ReturnsError()
        {
            // ヘルプ: Circle x y r [color] — 引数が3個未満はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Circle 100 100");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void CircleCmd_ValidArgs_ReturnsNextId()
        {
            // ヘルプ: Circle x y r — マップウィンドウの(x,y)を中心とする半径rの円を描く
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Circle 200 200 50", 0);
            var result = cmd.Exec();
            // GUI.SaveScreen() に対するハンドラが未注入のため GUINotImplementedException がスローされ NextID が返る
            Assert.AreEqual(1, result);
        }

        // ──────────────────────────────────────────────
        // OvalCmd
        // ヘルプ: Oval x y r ratio [color] — 楕円を描画
        // ──────────────────────────────────────────────

        [TestMethod]
        public void OvalCmd_TooFewArgs_ReturnsError()
        {
            // ヘルプ: Oval x y r ratio [color] — 引数が4個未満はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Oval 100 100 50");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void OvalCmd_ValidArgs_ReturnsNextId()
        {
            // ヘルプ: Oval x y r ratio — 楕円を描く
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Oval 100 100 50 0.5", 0);
            var result = cmd.Exec();
            // GUI.SaveScreen() に対するハンドラが未注入のため GUINotImplementedException がスローされ NextID が返る
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void OvalCmd_InvalidColorOption_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Oval 100 100 50 0.5 BADCOLOR");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // LineCmd
        // ヘルプ: Line x1 y1 x2 y2 [color] [B|BF] — 直線や矩形を描画
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LineCmd_TooFewArgs_ReturnsError()
        {
            // ヘルプ: Line x1 y1 x2 y2 — 引数が4個未満はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Line 0 0 100");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void LineCmd_ValidArgs_ReturnsNextId()
        {
            // ヘルプ: Line x1 y1 x2 y2 — 2点間に直線を描く
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Line 0 0 100 100", 0);
            var result = cmd.Exec();
            // GUI.SaveScreen() に対するハンドラが未注入のため GUINotImplementedException がスローされ NextID が返る
            Assert.AreEqual(1, result);
        }

        // ──────────────────────────────────────────────
        // PolygonCmd
        // ヘルプ: Polygon x1 y1 x2 y2 ... — 多角形を描画
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PolygonCmd_TooFewPoints_ReturnsError()
        {
            // ヘルプ: 頂点数が少なすぎる場合はエラー (1点未満 = 引数なし)
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Polygon");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void PolygonCmd_OnePoint_ReturnsError()
        {
            // 頂点が1個(x,y1組)のみは"頂点数が少なすぎます"エラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Polygon 100");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void PolygonCmd_ValidArgs_ReturnsNextId()
        {
            // ヘルプ: Polygon x1 y1 x2 y2 x3 y3 — 三角形を描く
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Polygon 0 0 100 0 50 100", 0);
            var result = cmd.Exec();
            // GUI.SaveScreen() に対するハンドラが未注入のため GUINotImplementedException がスローされ NextID が返る
            Assert.AreEqual(1, result);
        }

        // ──────────────────────────────────────────────
        // PSetCmd
        // ヘルプ: PSet x y [color] — 点を描画
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PSetCmd_TooFewArgs_ReturnsError()
        {
            // ヘルプ: PSet x y [color] — 引数が2個未満はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "PSet 100");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void PSetCmd_ValidArgs_ReturnsNextId()
        {
            // ヘルプ: PSet x y — 指定座標に点を描画
            var src = CreateSrc();
            var cmd = CreateCmd(src, "PSet 100 100", 0);
            var result = cmd.Exec();
            // MockGUI.MapPWidth はデフォルト 0 を返すため、座標 (100, 100) が範囲外と判定され NextID が返る
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void PSetCmd_InvalidColor_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "PSet 100 100 BADCOLOR");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ExplodeCmd
        // ヘルプ: Explode size [x y] — 爆発アニメーションを表示
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExplodeCmd_WithSize_ReturnsNextId()
        {
            // ヘルプ: Explode size — 現在のカーソル位置で爆発アニメーション
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Explode 中", 0);
            var result = cmd.Exec();
            // MockGUI.MapX/MapY はデフォルト 0 を返し、GUI.ExplodeAnimation に対するハンドラが未注入のため GUINotImplementedException がスローされ NextID が返る
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ExplodeCmd_WithSizeAndCoords_ReturnsNextId()
        {
            // ヘルプ: Explode size x y — 指定座標で爆発アニメーション
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Explode 大 5 5", 0);
            var result = cmd.Exec();
            // GUI.ExplodeAnimation に対するハンドラが未注入のため GUINotImplementedException がスローされ NextID が返る
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ExplodeCmd_WrongArgCount_ReturnsError()
        {
            // 引数が1個(サイズなし)はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Explode");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }
    }
}
