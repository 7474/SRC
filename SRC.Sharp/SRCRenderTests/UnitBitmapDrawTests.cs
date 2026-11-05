using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using SRCPlay;
using System.IO;
using System.Linq;

namespace SRCRender.Tests
{
    /// <summary>
    /// ユニット画像の描画（地形ユニット・非表示）。
    /// </summary>
    [TestClass]
    public class UnitBitmapDrawTests
    {
        private ScenarioWorkspace workspace;

        [TestInitialize]
        public void Initialize()
        {
            workspace = new ScenarioWorkspace();
            workspace.WriteMap("test.map", 20, 20, (x, y) => 0);
            workspace.Write(Path.Combine("Data", "pilot.txt"),
                Pilot("テスト1"),
                "",
                Pilot("テスト2"),
                "");
        }

        [TestCleanup]
        public void Cleanup()
        {
            workspace.Dispose();
        }

        private static string Pilot(string name)
        {
            return string.Join("\r\n",
                name,
                name + ", てすと, 男性, 汎用, AAAA, 100",
                "特殊能力なし",
                "100, 100, 100, 100, 100, 100, 普通",
                "ＳＰなし",
                "-.bmp, -.mid");
        }

        private static string Unit(string name, string feature, string bitmap)
        {
            return string.Join("\r\n",
                name,
                name + ", てすと, 汎用, 1, 2",
                "陸, 4, M, 1000, 100",
                feature == null ? "特殊能力なし" : "特殊能力\r\n" + feature,
                "1000, 100, 1000, 60",
                "AAAA, " + bitmap);
        }

        /// <summary>32x32 の白地の中央に赤い四角の画像を書く（拡張子が .bmp でも中身は PNG）</summary>
        private void WriteUnitImage(params string[] paths)
        {
            var path = Path.Combine(workspace.AppPath, Path.Combine(paths));
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (var bitmap = new SKBitmap(32, 32))
            using (var canvas = new SKCanvas(bitmap))
            using (var paint = new SKPaint { Color = SKColors.Red })
            {
                canvas.Clear(SKColors.White);
                canvas.DrawRect(8, 8, 16, 16, paint);
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = File.Create(path))
                {
                    data.SaveTo(stream);
                }
            }
        }

        private static SKColor[] Cell(SKBitmap bitmap, int px, int py)
        {
            return Enumerable.Range(0, ScreenLayout.CellPx * ScreenLayout.CellPx)
                .Select(i => bitmap.GetPixel(px + i % ScreenLayout.CellPx, py + i / ScreenLayout.CellPx))
                .ToArray();
        }

        /// <summary>
        /// 地形ユニットは、陣営色の背景を描かず、画像（白も透過しない）をそのまま描く。
        /// 画像は Bitmap\Map から、地形画像の 4 桁の番号付きのファイル名にも読み替えて探す。
        /// 元の SRC の GUI.bas の FindUnitBitmap・LoadUnitBitmap（7474/SRC#1482）。
        /// </summary>
        [TestMethod]
        public void TerrainUnit_DrawsMapImageAsIs()
        {
            workspace.Write(Path.Combine("Data", "robot.txt"),
                Unit("壁", "地形ユニット", "plain1.bmp"),
                "",
                Unit("標的機", null, "target.bmp"),
                "===",
                "");
            WriteUnitImage("Bitmap", "Map", "plain", "plain0001.bmp");
            WriteUnitImage("Bitmap", "Unit", "target.bmp");
            workspace.Write("test.eve",
                "スタート:",
                "ChangeMap test.map",
                "Create 敵 壁 0 テスト1 1 3 3",
                "Create 敵 標的機 0 テスト2 1 5 3",
                "Refresh",
                "Talk システム",
                "描画",
                "End",
                "Quit",
                "");

            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                StringAssert.Contains(runner.WaitForMessage(), "描画");
                var gui = runner.GUI;
                using (var bitmap = SKBitmap.Decode(runner.SnapshotPng()))
                {
                    // 地形ユニット: 白は透過せず、陣営色の背景も描かない
                    var x = gui.MapToPixelX(3);
                    var y = gui.MapToPixelY(3);
                    Assert.AreEqual(SKColors.White, bitmap.GetPixel(x + 2, y + 2));
                    Assert.AreEqual(SKColors.White, bitmap.GetPixel(x + 29, y + 16));
                    Assert.AreEqual(SKColors.Red, bitmap.GetPixel(x + 16, y + 16));

                    // ふつうのユニット: 白を透過して陣営色の背景に重ねる
                    x = gui.MapToPixelX(5);
                    Assert.AreNotEqual(SKColors.White, bitmap.GetPixel(x + 2, y + 2));
                    Assert.AreEqual(SKColors.Red, bitmap.GetPixel(x + 16, y + 16));
                }
                runner.ClickMessage();
                runner.WaitForTermination();
            }
        }

        /// <summary>
        /// ユニット用特殊能力「非表示」・ChangeUnitBitmap の「非表示」のユニットは、マップには地形だけを描き、
        /// メッセージウィンドウにはユニットのいる地形のタイルを出す。
        /// 元の SRC の GUI.bas の MakeUnitBitmap（BitmapID = -1）・UpdateMessageForm（7474/SRC#1481）。
        /// </summary>
        [TestMethod]
        public void HiddenUnit_DrawsTerrainOnly()
        {
            workspace.Write(Path.Combine("Data", "robot.txt"),
                Unit("隠れ", "非表示", "target.bmp"),
                "",
                Unit("標的機", null, "target.bmp"),
                "===",
                "");
            WriteUnitImage("Bitmap", "Unit", "target.bmp");
            workspace.Write("test.eve",
                "スタート:",
                "ChangeMap test.map",
                "Create 敵 隠れ 0 テスト1 1 3 3",
                "Create 敵 標的機 0 テスト2 1 5 3",
                "ChangeUnitBitmap テスト2 非表示",
                "Refresh",
                "Talk システム",
                "描画",
                "End",
                "Quit",
                "");

            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                StringAssert.Contains(runner.WaitForMessage(), "描画");
                var gui = runner.GUI;
                var y = gui.MapToPixelY(3);
                SKColor[] empty;
                using (var bitmap = SKBitmap.Decode(runner.SnapshotPng()))
                {
                    empty = Cell(bitmap, gui.MapToPixelX(8), y);
                    CollectionAssert.AreEqual(empty, Cell(bitmap, gui.MapToPixelX(3), y), "特殊能力「非表示」のユニットは地形だけを描く");
                    CollectionAssert.AreEqual(empty, Cell(bitmap, gui.MapToPixelX(5), y), "ChangeUnitBitmap の「非表示」のユニットは地形だけを描く");
                }

                SKColor[] face = null;
                System.Exception error = null;
                using (var done = new System.Threading.ManualResetEventSlim())
                {
                    runner.Session.Game.Post(() =>
                    {
                        try
                        {
                            var u = runner.Session.SRC.Map.MapDataForUnit[3, 3];
                            // 再描画しても描かない
                            gui.PaintUnitBitmap(u);
                            // メッセージウィンドウにはユニットのいる地形のタイルを出す
                            gui.CloseMessageForm();
                            gui.OpenMessageForm(u, null);
                            gui.UpdateMessageForm(u);
                            face = Cell(gui.MessageWindow.picUnit1.Image, 0, 0);
                        }
                        catch (System.Exception ex)
                        {
                            error = ex;
                        }
                        finally
                        {
                            done.Set();
                        }
                    });
                    Assert.IsTrue(done.Wait(ScreenScenarioRunner.Timeout));
                }
                Assert.IsNull(error, error?.ToString());
                CollectionAssert.AreEqual(empty, face, "メッセージウィンドウには地形のタイルを出す");
                using (var bitmap = SKBitmap.Decode(runner.SnapshotPng()))
                {
                    CollectionAssert.AreEqual(empty, Cell(bitmap, gui.MapToPixelX(3), y), "PaintUnitBitmap でも描かない");
                }
                runner.ClickMessage();
                runner.WaitForTermination();
            }
        }
    }
}
