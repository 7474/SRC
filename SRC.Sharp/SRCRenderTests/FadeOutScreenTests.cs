using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using SRCPlay;
using System.IO;
using System.Linq;
using System.Threading;

namespace SRCRender.Tests
{
    /// <summary>
    /// FadeOut・WhiteOut のあとの画面（VB 版と同じ）。
    /// - 塗りつぶした画面のまま残る（暗転前の画面に戻らない）
    /// - 続けて描いた文字などは塗りつぶした画面に重なる
    /// - ClearPicture で暗転前の画面に戻る
    /// </summary>
    [TestClass]
    public class FadeOutScreenTests
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
        [DataRow("FadeOut", 0x00)]
        [DataRow("WhiteOut", 0xff)]
        public void FadeOut_LeavesFilledScreenUntilClearPicture(string command, int level)
        {
            var fill = new SKColor((byte)level, (byte)level, (byte)level);
            workspace.Write("test.eve",
                "スタート:",
                "ChangeMap test.map",
                "Create 味方 標的機 0 テスト 1 3 3",
                "Refresh",
                "Talk システム",
                "暗転前",
                "End",
                command + " 2",
                "Font #FF0000",
                "PaintString 0 0 文字",
                "Talk システム",
                "暗転後",
                "End",
                "ClearPicture",
                "Talk システム",
                "元の画面",
                "End",
                "Quit",
                "");
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                var gui = runner.GUI;
                StringAssert.Contains(runner.WaitForMessage(), "暗転前");
                var before = runner.SnapshotPng();
                var unitCell = Cell(before, gui.MapToPixelX(3), gui.MapToPixelY(3));
                Assert.IsFalse(unitCell.All(x => x == fill), "暗転前はユニットが見える");

                runner.ClickMessage();
                StringAssert.Contains(runner.WaitForMessage(), "暗転後");
                var after = runner.SnapshotPng();
                Assert.IsTrue(Cell(after, gui.MapToPixelX(3), gui.MapToPixelY(3)).All(x => x == fill), "暗転前の画面に戻らない");
                Assert.IsTrue(Cell(after, gui.MapToPixelX(8), gui.MapToPixelY(3)).All(x => x == fill), "暗転前の画面に戻らない");
                Assert.IsFalse(Cell(after, 0, 0).All(x => x == fill), "塗りつぶした画面に文字が描かれる");

                runner.ClickMessage();
                StringAssert.Contains(runner.WaitForMessage(), "元の画面");
                // ClearPicture は退避した画面を描き戻すだけで表示は更新しない（VB 版と同じ）ので、表示を更新してから見る
                using (var done = new ManualResetEventSlim())
                {
                    runner.Session.Game.Post(() =>
                    {
                        gui.UpdateScreen();
                        done.Set();
                    });
                    Assert.IsTrue(done.Wait(ScreenScenarioRunner.Timeout));
                }
                var restored = runner.SnapshotPng();
                CollectionAssert.AreEqual(unitCell, Cell(restored, gui.MapToPixelX(3), gui.MapToPixelY(3)), "ClearPicture で暗転前の画面に戻る");
            }
        }
    }
}
