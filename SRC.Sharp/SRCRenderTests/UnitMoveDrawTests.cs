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
    /// 移動アニメーション（MoveUnitBitmap・MoveUnitBitmap2）のあとも、移動先にユニットが見えること。
    /// 最後のコマを表示用のバッファにだけ描いていたため、呼び出し元が画面を描き直さないと、次に画面を更新したときにユニットが消えていた。
    /// </summary>
    [TestClass]
    public class UnitMoveDrawTests
    {
        private ScenarioWorkspace workspace;

        [TestInitialize]
        public void Initialize()
        {
            workspace = new ScenarioWorkspace();
            workspace.WriteMap("test.map", 20, 20, (x, y) => 0);
            workspace.Write(Path.Combine("Data", "robot.txt"),
                "標的機",
                "標的機, ひょうてきき, 汎用, 1, 2",
                "陸, 4, M, 1000, 100",
                "特殊能力なし",
                "1000, 100, 1000, 60",
                "AAAA, -.bmp",
                "===",
                "");
            workspace.Write(Path.Combine("Data", "pilot.txt"),
                "テスト",
                "テスト, てすと, 男性, 汎用, AAAA, 100",
                "特殊能力なし",
                "100, 100, 100, 100, 100, 100, 普通",
                "ＳＰなし",
                "-.bmp, -.mid",
                "");
            workspace.Write("test.eve",
                "スタート:",
                "ChangeMap test.map",
                "Create 味方 標的機 0 テスト 1 3 3",
                "Refresh",
                "Talk システム",
                "移動",
                "End",
                "Quit",
                "");
        }

        [TestCleanup]
        public void Cleanup()
        {
            workspace.Dispose();
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

        [TestMethod]
        public void MoveUnitBitmap_LeavesUnitAtDestinationAfterScreenUpdate()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                StringAssert.Contains(runner.WaitForMessage(), "移動");
                var gui = runner.GUI;
                var before = runner.SnapshotPng();
                var unitCell = Cell(before, gui.MapToPixelX(3), gui.MapToPixelY(3));
                var emptyCell = Cell(before, gui.MapToPixelX(8), gui.MapToPixelY(3));
                CollectionAssert.AreNotEqual(emptyCell, unitCell, "移動前のマスにユニットが描かれている");

                Exception error = null;
                using (var done = new ManualResetEventSlim())
                {
                    runner.Session.Game.Post(() =>
                    {
                        try
                        {
                            var u = runner.Session.SRC.Map.MapDataForUnit[3, 3];
                            gui.MoveUnitBitmap(u, 3, 3, 8, 3, 0);
                            // 呼び出し元が描き直さずに画面を更新する（メッセージの表示など）
                            gui.UpdateScreen();
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

                var after = runner.SnapshotPng();
                CollectionAssert.AreEqual(unitCell, Cell(after, gui.MapToPixelX(8), gui.MapToPixelY(3)), "移動先にユニットが残る");
                CollectionAssert.AreEqual(emptyCell, Cell(after, gui.MapToPixelX(3), gui.MapToPixelY(3)), "移動元は消える");
            }
        }
    }
}
