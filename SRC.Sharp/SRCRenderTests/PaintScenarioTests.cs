using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
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
