using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using SRCPlay;
using System.IO;
using System.Linq;

namespace SRCRender.Tests
{
    /// <summary>
    /// 撃墜・爆発のアニメーション（元の SRC の Effect.bas の DieAnimation・ExplodeAnimation）。
    /// </summary>
    [TestClass]
    public class EffectDieExplodeAnimationTests
    {
        private ScenarioWorkspace workspace;

        [TestInitialize]
        public void Initialize()
        {
            workspace = new ScenarioWorkspace();
            workspace.WriteMap("test.map", 20, 20, (x, y) => 0);
            workspace.Write(Path.Combine("Data", "pilot.txt"),
                "テスト1",
                "テスト1, てすと, 男性, 汎用, AAAA, 100",
                "特殊能力なし",
                "100, 100, 100, 100, 100, 100, 普通",
                "ＳＰなし",
                "-.bmp, -.mid",
                "");
            workspace.Write(Path.Combine("Data", "robot.txt"),
                "歩兵",
                "歩兵, ほへい, (人間), 1, 2",
                "陸, 4, M, 1000, 100",
                "特殊能力なし",
                "1000, 100, 1000, 60",
                "AAAA, target.bmp",
                "===",
                "");
            var path = Path.Combine(workspace.AppPath, "Bitmap", "Unit", "target.bmp");
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

        [TestCleanup]
        public void Cleanup()
        {
            workspace.Dispose();
        }

        private static SKColor[] Cell(SKBitmap bitmap, int px, int py)
        {
            return Enumerable.Range(0, ScreenLayout.CellPx * ScreenLayout.CellPx)
                .Select(i => bitmap.GetPixel(px + i % ScreenLayout.CellPx, py + i / ScreenLayout.CellPx))
                .ToArray();
        }

        /// <summary>
        /// DieAnimation はユニットを消した直後に画面へ出す。元の SRC は EraseUnitBitmap の do_refresh を省略して呼ぶ（既定は True）ので、
        /// メッセージのウエイトが無く撃墜のアニメーションを飛ばすときも、消えたユニットは画面に残らない（7474/SRC#1498）。
        /// </summary>
        [TestMethod]
        public void DieAnimation_RefreshesScreenAfterErasingUnit()
        {
            workspace.Write("test.eve",
                "スタート:",
                "ChangeMap test.map",
                "Create 敵 歩兵 0 テスト1 1 3 3",
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
                var x = gui.MapToPixelX(3);
                var y = gui.MapToPixelY(3);
                SKColor[] empty;
                using (var bitmap = SKBitmap.Decode(runner.SnapshotPng()))
                {
                    empty = Cell(bitmap, gui.MapToPixelX(8), y);
                    Assert.AreEqual(SKColors.Red, bitmap.GetPixel(x + 16, y + 16));
                }

                System.Exception error = null;
                using (var done = new System.Threading.ManualResetEventSlim())
                {
                    runner.Session.Game.Post(() =>
                    {
                        try
                        {
                            var u = runner.Session.SRC.Map.MapDataForUnit[3, 3];
                            Assert.IsTrue(u.IsHero());
                            gui.MessageWait = 0;
                            gui.DieAnimation(u);
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
                using (var bitmap = SKBitmap.Decode(runner.SnapshotPng()))
                {
                    CollectionAssert.AreEqual(empty, Cell(bitmap, x, y), "消したユニットはすぐ画面に出す");
                }
                runner.ClickMessage();
                runner.WaitForTermination();
            }
        }

        /// <summary>
        /// 爆発の画像が戦闘アニメ版かどうかは、パスに \Anime\ があるかで見分ける。
        /// シナリオのフォルダの名前に Anime を含むだけでは戦闘アニメ版としない（元の SRC の ExplodeAnimation。7474/SRC#1498）。
        /// </summary>
        [TestMethod]
        public void IsAnimeExplodeImagePath_MatchesAnimeFolderOnly()
        {
            Assert.IsTrue(ScreenGUI.IsAnimeExplodeImagePath(@"C:\SRC\Bitmap\Anime\Explode\EFFECT_Explode"));
            Assert.IsTrue(ScreenGUI.IsAnimeExplodeImagePath("/srv/src/Bitmap/Anime/Explode/EFFECT_Explode"));
            Assert.IsFalse(ScreenGUI.IsAnimeExplodeImagePath(@"C:\AnimeWars\Bitmap\Event\Explode"));
            Assert.IsFalse(ScreenGUI.IsAnimeExplodeImagePath("/home/u/MyAnimeScenario/Bitmap/Event/Explode"));
        }
    }
}
