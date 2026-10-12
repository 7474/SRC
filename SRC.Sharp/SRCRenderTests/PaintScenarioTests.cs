using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using System.IO;

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
    }
}
