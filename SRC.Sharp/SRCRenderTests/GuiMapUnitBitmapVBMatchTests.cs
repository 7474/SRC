using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using SRCPlay;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace SRCRender.Tests
{
    /// <summary>
    /// マップの描画を元の SRC（GUI.bas の SetupBackground・RefreshScreen・MakeUnitBitmap・PaintUnitBitmap・
    /// EraseUnitBitmap・MoveUnitBitmap）に合わせたことのテスト（7474/SRC#1498）。
    /// </summary>
    [TestClass]
    public class GuiMapUnitBitmapVBMatchTests
    {
        private static readonly SKColor MaskGray = new SKColor(140, 140, 140);
        private static readonly SKColor LineGray = new SKColor(100, 100, 100);

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
                "",
                Pilot("テスト3"),
                "");
            workspace.Write(Path.Combine("Data", "robot.txt"),
                Unit("標的機", "target.bmp"),
                "",
                Unit("甲機", "-.bmp"),
                "",
                Unit("乙機", "-.bmp"),
                "===",
                "");
            WriteUnitImage("Bitmap", "Unit", "target.bmp");
            workspace.Write("test.eve",
                "スタート:",
                "ChangeMap test.map",
                "Create 味方 標的機 0 テスト1 1 3 3",
                "Create 味方 甲機 0 テスト2 1 5 3",
                "Create 味方 乙機 0 テスト3 1 7 3",
                "Refresh",
                "Talk システム",
                "描画",
                "End",
                "Quit",
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

        private static string Unit(string name, string bitmap)
        {
            return string.Join("\r\n",
                name,
                name + ", てすと, 汎用, 1, 2",
                "陸, 4, M, 1000, 100",
                "特殊能力なし",
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

        private static SKColor[] Cell(byte[] png, int px, int py)
        {
            using (var bitmap = SKBitmap.Decode(png))
            {
                return Enumerable.Range(0, ScreenLayout.CellPx * ScreenLayout.CellPx)
                    .Select(i => bitmap.GetPixel(px + i % ScreenLayout.CellPx, py + i / ScreenLayout.CellPx))
                    .ToArray();
            }
        }

        private static void Run(ScreenScenarioRunner runner, Action action)
        {
            Exception error = null;
            using (var done = new ManualResetEventSlim())
            {
                runner.Session.Game.Post(() =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
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
        }

        /// <summary>
        /// 行動済みのユニットは、x + y が奇数のピクセルだけを黒にした網掛け（picMask との AND）。
        /// マスクしたマスは、地形もユニットも x + y が奇数のピクセルを RGB(140, 140, 140) にする（picMask2 との XOR）。
        /// マスクしたユニットは行動済みかに関わらず同じ絵になる。
        /// </summary>
        [TestMethod]
        public void ActedAndMaskedCells_UseCheckerPatternOfVB()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                StringAssert.Contains(runner.WaitForMessage(), "描画");
                var gui = runner.GUI;
                var src = runner.Session.SRC;
                var ux = gui.MapToPixelX(3);
                var uy = gui.MapToPixelY(3);
                var tx = gui.MapToPixelX(10);
                var normalUnit = Cell(runner.SnapshotPng(), ux, uy);
                var normalTerrain = Cell(runner.SnapshotPng(), tx, uy);

                Run(runner, () =>
                {
                    var u = src.Map.MapDataForUnit[3, 3];
                    u.UsedAction = u.MaxAction();
                    gui.RedrawScreen();
                });
                var acted = Cell(runner.SnapshotPng(), ux, uy);
                for (var i = 0; i < acted.Length; i++)
                {
                    var odd = (i % ScreenLayout.CellPx + i / ScreenLayout.CellPx) % 2 == 1;
                    Assert.AreEqual(odd ? SKColors.Black : normalUnit[i], acted[i], $"行動済み ({i % 32}, {i / 32})");
                }

                Run(runner, () =>
                {
                    src.Map.MaskData[3, 3] = true;
                    src.Map.MaskData[10, 3] = true;
                    gui.MaskScreen();
                });
                var png = runner.SnapshotPng();
                var maskedActed = Cell(png, ux, uy);
                var maskedTerrain = Cell(png, tx, uy);
                for (var i = 0; i < maskedActed.Length; i++)
                {
                    var odd = (i % ScreenLayout.CellPx + i / ScreenLayout.CellPx) % 2 == 1;
                    Assert.AreEqual(odd ? MaskGray : normalUnit[i], maskedActed[i], $"マスクしたユニット ({i % 32}, {i / 32})");
                    Assert.AreEqual(odd ? MaskGray : normalTerrain[i], maskedTerrain[i], $"マスクした地形 ({i % 32}, {i / 32})");
                }

                Run(runner, () =>
                {
                    var u = src.Map.MapDataForUnit[3, 3];
                    u.UsedAction = 0;
                    gui.MaskScreen();
                });
                CollectionAssert.AreEqual(maskedActed, Cell(runner.SnapshotPng(), ux, uy), "マスクしたユニットは行動済みかに関わらない");

                runner.ClickMessage();
                runner.WaitForTermination();
            }
        }

        /// <summary>
        /// マス目の線は、外枠が (0, 0)-(MapPWidth - 1, MapPHeight - 1)、縦線が各マスの左端の列、横線が各マスの下端の行。
        /// </summary>
        [TestMethod]
        public void SquareLines_AreAtSamePixelsAsVB()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                StringAssert.Contains(runner.WaitForMessage(), "描画");
                var gui = runner.GUI;
                var src = runner.Session.SRC;
                Run(runner, () =>
                {
                    src.Config.ShowSquareLine = true;
                    gui.SetupBackground();
                    gui.Center(1, 1);
                    gui.RefreshScreen();
                });
                using (var bitmap = SKBitmap.Decode(runner.SnapshotPng()))
                {
                    var x = gui.MapToPixelX(4);
                    var y = gui.MapToPixelY(4);
                    // 横線は 1 つ上のマスの下端（y - 1）で、マスの上端（y）には引かない
                    Assert.AreEqual(LineGray, bitmap.GetPixel(x + 10, y - 1));
                    Assert.AreNotEqual(LineGray, bitmap.GetPixel(x + 10, y));
                    // 縦線はマスの左端
                    Assert.AreEqual(LineGray, bitmap.GetPixel(x, y + 10));
                    Assert.AreNotEqual(LineGray, bitmap.GetPixel(x - 1, y + 10));
                    // 外枠の上端・左端
                    Assert.AreEqual(LineGray, bitmap.GetPixel(gui.MapToPixelX(1) + 10, gui.MapToPixelY(1)));
                    Assert.AreEqual(LineGray, bitmap.GetPixel(gui.MapToPixelX(1), gui.MapToPixelY(1) + 10));
                }

                Run(runner, () =>
                {
                    gui.Center(20, 20);
                    gui.RefreshScreen();
                });
                using (var bitmap = SKBitmap.Decode(runner.SnapshotPng()))
                {
                    // 外枠の右端・下端はマップの最後のピクセル
                    var x = gui.MapToPixelX(20);
                    var y = gui.MapToPixelY(20);
                    Assert.AreEqual(LineGray, bitmap.GetPixel(x + 31, y + 10));
                    Assert.AreEqual(LineGray, bitmap.GetPixel(x + 10, y + 31));
                }

                runner.ClickMessage();
                runner.WaitForTermination();
            }
        }

        /// <summary>
        /// 画像に「-」を指定したユニットは陣営のタイルだけを描く（元の SRC の FindUnitBitmap は「\-.bmp」を
        /// 見つからなかったものとする）。素材なしのときの名前の頭文字は出さない。
        /// </summary>
        [TestMethod]
        public void NoImageUnit_DrawsPartyTileOnly()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                StringAssert.Contains(runner.WaitForMessage(), "描画");
                var gui = runner.GUI;
                var png = runner.SnapshotPng();
                var y = gui.MapToPixelY(3);
                CollectionAssert.AreEqual(Cell(png, gui.MapToPixelX(5), y), Cell(png, gui.MapToPixelX(7), y),
                    "名前の違う「-」のユニットは同じ絵（頭文字を描かない）");
                runner.ClickMessage();
                runner.WaitForTermination();
            }
        }

        /// <summary>
        /// PaintUnitBitmap（リフレッシュ無しでない）と EraseUnitBitmap（do_refresh）は画面が書き換えられたことを記録する。
        /// 画面外のマスでは何もしない（SaveScreen もしない）。
        /// </summary>
        [TestMethod]
        public void PaintAndErase_RecordScreenIsSavedAsVB()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                StringAssert.Contains(runner.WaitForMessage(), "描画");
                var gui = runner.GUI;
                var src = runner.Session.SRC;
                bool afterPaint = true, afterErase = true, afterOutside = true;
                Run(runner, () =>
                {
                    var u = src.Map.MapDataForUnit[3, 3];
                    gui.SaveScreen();
                    gui.PaintUnitBitmap(u);
                    afterPaint = gui.ScreenIsSaved;

                    gui.SaveScreen();
                    gui.EraseUnitBitmap(3, 3);
                    afterErase = gui.ScreenIsSaved;

                    gui.ScreenIsSaved = false;
                    gui.Center(1, 1);
                    gui.RefreshScreen();
                    gui.EraseUnitBitmap(20, 20, false);
                    afterOutside = gui.ScreenIsSaved;
                });
                Assert.IsFalse(afterPaint, "PaintUnitBitmap");
                Assert.IsFalse(afterErase, "EraseUnitBitmap");
                Assert.IsFalse(afterOutside, "画面外の EraseUnitBitmap は SaveScreen しない");
                runner.ClickMessage();
                runner.WaitForTermination();
            }
        }

        /// <summary>
        /// 移動のアニメーションは、行動済みのユニットでも網掛けしていない画像（picUnitBitmap の 1 段目）で動かす。
        /// </summary>
        [TestMethod]
        public void MoveUnitBitmap_UsesNotActedImage()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                StringAssert.Contains(runner.WaitForMessage(), "描画");
                var gui = runner.GUI;
                var src = runner.Session.SRC;
                var normalUnit = Cell(runner.SnapshotPng(), gui.MapToPixelX(3), gui.MapToPixelY(3));
                Run(runner, () =>
                {
                    var u = src.Map.MapDataForUnit[3, 3];
                    u.UsedAction = u.MaxAction();
                    gui.RedrawScreen();
                    gui.MoveUnitBitmap(u, 3, 3, 10, 5, 0);
                    gui.UpdateScreen();
                });
                CollectionAssert.AreEqual(normalUnit, Cell(runner.SnapshotPng(), gui.MapToPixelX(10), gui.MapToPixelY(5)));
                runner.ClickMessage();
                runner.WaitForTermination();
            }
        }

        /// <summary>
        /// 描画モードは地形とレイヤーに別々にかけ、レイヤーは背景色を白として透過で処理する
        /// （GUI.bas の SetupBackground の ColorFilter ..., True）。描画色が白のときはレイヤーの色を (254, 255, 255) に寄せる。
        /// </summary>
        [TestMethod]
        public void ColorFilter_LayerIsFilteredAsTransparent()
        {
            var path = Path.Combine(workspace.AppPath, "Bitmap", "Map", "sea0000.bmp");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (var bitmap = new SKBitmap(32, 32))
            using (var canvas = new SKCanvas(bitmap))
            using (var paint = new SKPaint { Color = new SKColor(201, 100, 50) })
            {
                canvas.Clear(SKColors.White);
                canvas.DrawRect(0, 0, 32, 16, paint);
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = File.Create(path))
                {
                    data.SaveTo(stream);
                }
            }
            workspace.Write("test.eve",
                "スタート:",
                "ChangeMap test.map",
                "ChangeLayer 7 5 海 0",
                "ColorFilter #FFFFFF 50%",
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
                    // (201 * 50 + 254 * 50) \ 100 = 227（白に寄せずに混ぜると 228）
                    Assert.AreEqual(new SKColor(227, 177, 152), bitmap.GetPixel(gui.MapToPixelX(7) + 5, gui.MapToPixelY(5) + 5));
                }
                runner.ClickMessage();
                runner.WaitForTermination();
            }
        }
    }
}
