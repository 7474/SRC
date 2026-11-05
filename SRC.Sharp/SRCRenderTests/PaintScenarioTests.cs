using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using System;
using System.IO;
using SRCPlay;

namespace SRCRender.Tests
{
    /// <summary>
    /// 描画コマンド（PaintPicture / PaintString / Line / Circle / ClearPicture など）のテスト。
    /// </summary>
    [TestClass]
    public class PaintScenarioTests
    {
        [TestMethod]
        public void PlayScenario_PaintCommands()
        {
            using (var workspace = new ScenarioWorkspace())
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                workspace.WriteMap("test.map", 20, 20, (x, y) => x <= 10 ? 0 : 1);
                workspace.WriteTestImage(Path.Combine("Bitmap", "Event", "test.png"));
                workspace.Write("test.eve",
                    "プロローグ:",
                    "ChangeMap test.map",
                    // 元の SRC と同じく、プロローグではメインウィンドウを隠しているため、表示してから描く
                    "Show",
                    "Color #FF0000",
                    "Line 10 10 200 10",
                    "Line 20 30 120 100 BF #00FF00",
                    "Circle 300 100 50 #00FFFF",
                    "Font 20pt",
                    "PaintString 40 200 描画テスト",
                    "PaintPicture test.png 300 250 透過 左右反転",
                    "PaintPicture test.png 380 250 セピア",
                    "Refresh",
                    "Talk システム",
                    "描画テスト",
                    "End",
                    "ClearPicture",
                    "Refresh",
                    "Talk システム",
                    "消去後",
                    "End",
                    "Quit",
                    "");

                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));

                Assert.AreEqual("描画テスト", runner.WaitForMessage());
                var painted = runner.SnapshotPng();
                using (var bitmap = SKBitmap.Decode(painted))
                {
                    // Line（赤）
                    Assert.AreEqual(SKColors.Red, bitmap.GetPixel(100, 10));
                    // Line BF（緑の塗りつぶし）
                    Assert.AreEqual(new SKColor(0, 255, 0), bitmap.GetPixel(70, 60));
                    // 透過＋左右反転: 左側が青、右側が赤、白は透過して地形（海）が見える
                    Assert.AreEqual(SKColors.Blue, bitmap.GetPixel(300 + 16, 250 + 32));
                    Assert.AreEqual(SKColors.Red, bitmap.GetPixel(300 + 48, 250 + 32));
                    Assert.AreNotEqual(SKColors.White, bitmap.GetPixel(300 + 2, 250 + 2));
                    // セピア: 白地はセピア調（R > G > B）
                    var sepia = bitmap.GetPixel(380 + 2, 250 + 2);
                    Assert.IsTrue(sepia.Red > sepia.Green && sepia.Green > sepia.Blue, sepia.ToString());
                }
                VisualAssert.MatchesBaseline(painted, "paint-01-painted");
                runner.ClickMessage();

                Assert.AreEqual("消去後", runner.WaitForMessage());
                var cleared = runner.SnapshotPng();
                using (var bitmap = SKBitmap.Decode(cleared))
                {
                    // ClearPicture 後は地形（平地）に戻る
                    var pixel = bitmap.GetPixel(70, 60);
                    Assert.IsTrue(pixel.Green > pixel.Red && pixel.Green > pixel.Blue && pixel != new SKColor(0, 255, 0), pixel.ToString());
                }
                VisualAssert.MatchesBaseline(cleared, "paint-02-cleared");
                runner.ClickMessage();

                runner.WaitForTermination();
            }
        }

        /// <summary>
        /// Wait コマンドの待ち（DoEvents）で画面が更新されるか。
        /// 元の SRC では picMain(0) は AutoRedraw で、PaintString・Line・Circle・PSet など VB のグラフィックメソッドは
        /// 描画とともに再描画を要求するため、Refresh しなくても次の DoEvents（Wait など）で画面に出る。
        /// API（BitBlt）で描く PaintPicture は再描画を要求しないため、PaintPicture だけでは Refresh まで出ない
        /// （ヘルプ「Waitコマンド」「PaintPictureコマンド」）。
        /// </summary>
        [TestMethod]
        public void PlayScenario_WaitShowsPaintedScreen()
        {
            using (var workspace = new ScenarioWorkspace())
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                workspace.WriteMap("test.map", 20, 20, (x, y) => 0);
                workspace.WriteTestImage(Path.Combine("Bitmap", "Event", "test.png"));
                workspace.Write("test.eve",
                    "プロローグ:",
                    "ChangeMap test.map",
                    // メインウィンドウを表示しておく（非表示なら Wait Click が表示とともに画面を更新する）
                    "Show",
                    // PaintPicture だけ: Wait では表示されない
                    "PaintPicture test.png 100 100",
                    "Wait Click",
                    // PaintString を続けると Wait で（その前の PaintPicture も含めて）表示される
                    "Font 20pt",
                    "PaintString 40 300 描画テスト",
                    "Wait Click",
                    // Line でも同じ
                    "ClearPicture",
                    "PaintPicture test.png 200 100",
                    "Line 0 400 50 400 #00FF00",
                    "Wait 1",
                    "Wait Click",
                    "Quit",
                    "");

                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));

                using (var bitmap = WaitClick(runner))
                {
                    Assert.AreNotEqual(SKColors.Red, bitmap.GetPixel(100 + 16, 100 + 32));
                }
                using (var bitmap = WaitClick(runner))
                {
                    Assert.AreEqual(SKColors.Red, bitmap.GetPixel(100 + 16, 100 + 32));
                    Assert.AreEqual(SKColors.Blue, bitmap.GetPixel(100 + 48, 100 + 32));
                }
                using (var bitmap = WaitClick(runner))
                {
                    // ClearPicture で消した画像は消え、新しい画像と線が出る
                    Assert.AreNotEqual(SKColors.Red, bitmap.GetPixel(100 + 16, 100 + 32));
                    Assert.AreEqual(SKColors.Red, bitmap.GetPixel(200 + 16, 100 + 32));
                    Assert.AreEqual(new SKColor(0, 255, 0), bitmap.GetPixel(25, 400));
                }

                runner.WaitForTermination();
            }
        }

        /// <summary>
        /// Oval の縦横比は VB の Circle メソッドの aspect（縦÷横）と同じ。
        /// 1 未満なら半径は横で横長、1 より大きいなら半径は縦で縦長の楕円になる（ヘルプ「Ovalコマンド」）。
        /// </summary>
        [TestMethod]
        public void PlayScenario_OvalRatioMatchesVbAspect()
        {
            using (var workspace = new ScenarioWorkspace())
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                workspace.WriteMap("test.map", 20, 20, (x, y) => 0);
                workspace.Write("test.eve",
                    "プロローグ:",
                    "ChangeMap test.map",
                    "Show",
                    "Oval 150 120 80 0.5 #FF0000",
                    "Oval 350 300 80 2 #FF0000",
                    "Refresh",
                    "Wait Click",
                    "Quit",
                    "");

                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));

                using (var bitmap = WaitClick(runner))
                {
                    // 0.5: 横の半径 80、縦の半径 40
                    Assert.IsTrue(HasColorNear(bitmap, 230, 120, SKColors.Red), "0.5 の右端");
                    Assert.IsTrue(HasColorNear(bitmap, 150, 80, SKColors.Red), "0.5 の上端");
                    Assert.IsFalse(HasColorNear(bitmap, 150, 40, SKColors.Red), "0.5 は縦に半径 80 まで伸びない");
                    // 2: 縦の半径 80、横の半径 40
                    Assert.IsTrue(HasColorNear(bitmap, 350, 220, SKColors.Red), "2 の上端");
                    Assert.IsTrue(HasColorNear(bitmap, 390, 300, SKColors.Red), "2 の右端");
                    Assert.IsFalse(HasColorNear(bitmap, 430, 300, SKColors.Red), "2 は横に半径 80 まで伸びない");
                }

                runner.WaitForTermination();
            }
        }

        /// <summary>
        /// PaintPicture の透過色（#rrggbb）と、部分表示の塗りつぶし。
        /// 元の SRC（CmdData.cls の PaintPictureCmd、GUI.bas の DrawPicture）では、白以外の色は透過色（背景色）になり、
        /// 部分表示は背景色で塗りつぶしてから透過色を抜く（7474/SRC#1467）。
        /// </summary>
        [TestMethod]
        public void PlayScenario_PaintPictureTransparentColor()
        {
            using (var workspace = new ScenarioWorkspace())
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                workspace.WriteMap("test.map", 20, 20, (x, y) => 0);
                workspace.WriteTestImage(Path.Combine("Bitmap", "Event", "test.png"));
                workspace.Write("test.eve",
                    "プロローグ:",
                    "ChangeMap test.map",
                    "Show",
                    // 描画色は部分表示の塗りつぶしに使わない
                    "Color #00FF00",
                    "PaintPicture test.png 0 0 透過 #FF0000",
                    "PaintPicture test.png 100 0 #FF0000",
                    "PaintPicture test.png 200 0 透過 上半分",
                    "PaintPicture test.png 300 0 上半分",
                    "PaintPicture test.png 0 100 透過 #0000FF 下半分",
                    "Refresh",
                    "Wait Click",
                    "Quit",
                    "");

                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));

                using (var bitmap = WaitClick(runner))
                {
                    bool IsTerrain(int x, int y)
                    {
                        var c = bitmap.GetPixel(x, y);
                        return c != SKColors.White && c != SKColors.Red && c != SKColors.Blue && c != new SKColor(0, 255, 0);
                    }
                    // 透過 #FF0000: 赤が抜け、白は残る
                    Assert.IsTrue(IsTerrain(16, 32), "赤を透過する");
                    Assert.AreEqual(SKColors.White, bitmap.GetPixel(2, 2), "白は透過しない");
                    Assert.AreEqual(SKColors.Blue, bitmap.GetPixel(48, 32));
                    // 透過を付けなければ色を指定しても抜かない
                    Assert.AreEqual(SKColors.Red, bitmap.GetPixel(100 + 16, 32));
                    Assert.AreEqual(SKColors.White, bitmap.GetPixel(100 + 2, 2));
                    // 透過 上半分: 下半分は背景色（白）で塗りつぶして透過する
                    Assert.AreEqual(SKColors.Red, bitmap.GetPixel(200 + 16, 16));
                    Assert.IsTrue(IsTerrain(200 + 16, 48), "塗りつぶした下半分を透過する");
                    // 上半分（透過なし）: 下半分は背景色（白）で塗りつぶす
                    Assert.AreEqual(SKColors.White, bitmap.GetPixel(300 + 16, 48));
                    // 透過 #0000FF 下半分: 上半分は背景色（青）で塗りつぶして透過し、下半分の青も抜ける
                    Assert.IsTrue(IsTerrain(16, 100 + 16), "塗りつぶした上半分を透過する");
                    Assert.AreEqual(SKColors.Red, bitmap.GetPixel(16, 100 + 48));
                    Assert.IsTrue(IsTerrain(48, 100 + 48), "青を透過する");
                    Assert.AreEqual(SKColors.White, bitmap.GetPixel(2, 100 + 60));
                }

                runner.WaitForTermination();
            }
        }

        [TestMethod]
        public void FillStylePatterns_DrawLinesEvery8Pixels()
        {
            // 元の SRC（VB の FillStyle。Windows のハッチブラシ）と同じく、8 ピクセルごとの線だけを塗り、線の間は塗らない
            var cases = new (SRCCore.VB.FillStyle Style, Func<int, int, bool> IsLine)[]
            {
                (SRCCore.VB.FillStyle.VbHorizontalLine, (x, y) => y % 8 == 7),
                (SRCCore.VB.FillStyle.VbVerticalLine, (x, y) => x % 8 == 4),
                (SRCCore.VB.FillStyle.VbUpwardDiagonal, (x, y) => x % 8 == y % 8),
                (SRCCore.VB.FillStyle.VbDownwardDiagonal, (x, y) => (x + y) % 8 == 7),
                (SRCCore.VB.FillStyle.VbCross, (x, y) => x % 8 == 4 || y % 8 == 7),
                (SRCCore.VB.FillStyle.VbDiagonalCross, (x, y) => x % 8 == y % 8 || (x + y) % 8 == 7),
            };
            foreach (var (style, isLine) in cases)
            {
                using (var bitmap = new SKBitmap(32, 32))
                using (var canvas = new SKCanvas(bitmap))
                using (var paint = ScreenGUI.CreateFillPaint(style, SKColors.Red))
                {
                    canvas.Clear(SKColors.White);
                    canvas.DrawRect(SKRect.Create(4, 4, 24, 24), paint);
                    for (var y = 0; y < 32; y++)
                    {
                        for (var x = 0; x < 32; x++)
                        {
                            var inside = x >= 4 && x < 28 && y >= 4 && y < 28;
                            var expected = inside && isLine(x, y) ? SKColors.Red : SKColors.White;
                            Assert.AreEqual(expected, bitmap.GetPixel(x, y), $"{style} ({x}, {y})");
                        }
                    }
                }
            }
            Assert.IsNull(ScreenGUI.CreateFillPaint(SRCCore.VB.FillStyle.VbFSTransparent, SKColors.Red));
            using (var solid = ScreenGUI.CreateFillPaint(SRCCore.VB.FillStyle.VbFSSolid, SKColors.Red))
            {
                Assert.IsNull(solid.Shader);
            }
        }

        [TestMethod]
        public void PlayScenario_FillStyleCrossOnBox()
        {
            using (var workspace = new ScenarioWorkspace())
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                workspace.WriteMap("test.map", 20, 20, (x, y) => 0);
                workspace.Write("test.eve",
                    "プロローグ:",
                    "ChangeMap test.map",
                    "Show",
                    "Line 0 0 199 199 BF #FFFFFF",
                    "FillStyle クロス",
                    "FillColor #FF0000",
                    "Line 40 40 120 120 B #0000FF",
                    "Refresh",
                    "Wait Click",
                    "Quit",
                    "");

                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));

                using (var bitmap = WaitClick(runner))
                {
                    // 縦線（x が 8 の倍数 + 4）と横線（y が 8 の倍数 + 7）だけを塗り、間は元の白のまま
                    Assert.AreEqual(SKColors.Red, bitmap.GetPixel(60, 50));
                    Assert.AreEqual(SKColors.Red, bitmap.GetPixel(50, 63));
                    Assert.AreEqual(SKColors.White, bitmap.GetPixel(50, 50));
                    Assert.AreEqual(SKColors.White, bitmap.GetPixel(61, 60));
                    // 枠は線の色
                    Assert.AreEqual(SKColors.Blue, bitmap.GetPixel(40, 80));
                }

                runner.WaitForTermination();
            }
        }

        private static bool HasColorNear(SKBitmap bitmap, int x, int y, SKColor color)
        {
            for (var dx = -2; dx <= 2; dx++)
            {
                for (var dy = -2; dy <= 2; dy++)
                {
                    if (bitmap.GetPixel(x + dx, y + dy) == color)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>Wait Click の待ちになるまで待ち、その時点の画面を返してクリックする。</summary>
        private static SKBitmap WaitClick(ScreenScenarioRunner runner)
        {
            var src = runner.Session.SRC;
            runner.WaitFor(_ => src.Commands.WaitClickMode && !runner.GUI.IsFormClicked, "Wait Click");
            var line = src.Event.CurrentLineNum;
            var bitmap = SKBitmap.Decode(runner.SnapshotPng());
            runner.GUI.IsFormClicked = true;
            runner.WaitFor(_ => runner.IsTerminated || src.Event.CurrentLineNum != line, "end of Wait Click");
            return bitmap;
        }
    }
}
