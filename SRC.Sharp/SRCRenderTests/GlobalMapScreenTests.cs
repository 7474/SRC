using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using SRCPlay;
using System.IO;

namespace SRCRender.Tests
{
    /// <summary>
    /// 「全体マップ」コマンドの画面（VB 版 Command.bas の GlobalMapCommand と同じ）。
    /// VB は画面を灰色（&amp;HC0C0C0）で塗り、その中央に縮小したマップを描く（7474/SRC#1556 の読み直しで見つけた違い）
    /// </summary>
    [TestClass]
    public class GlobalMapScreenTests
    {
        private ScenarioWorkspace workspace;

        [TestInitialize]
        public void Initialize()
        {
            workspace = new ScenarioWorkspace();
            workspace.WriteMap("test.map", 20, 10, (x, y) => 0);
            workspace.Write("test.eve",
                "スタート:",
                "ChangeMap test.map",
                "Talk システム",
                "始め",
                "End",
                "Quit",
                "");
        }

        [TestCleanup]
        public void Cleanup()
        {
            workspace.Dispose();
        }

        [TestMethod]
        public void GlobalMap_PaintsGrayBackgroundAndCentersMap()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                var gui = runner.GUI;
                StringAssert.Contains(runner.WaitForMessage(), "始め");

                runner.Session.Game.Post(() => gui.DisplayGlobalMap());
                runner.WaitFor(s => s.Message.Waiting && string.Join("\n", s.Message.Lines).Contains("味方ユニット"), "global map message");

                using (var bitmap = SKBitmap.Decode(runner.SnapshotPng()))
                {
                    // 縮小マップの外は灰色
                    Assert.AreEqual(new SKColor(0xC0, 0xC0, 0xC0), bitmap.GetPixel(2, 2));
                    // 20×10 のマップは 300×150 に縮めて画面の中央に描く（上下の余白は灰色のまま）
                    var cx = gui.MainPWidth / 2;
                    var top = (gui.MainPHeight - 150) / 2;
                    Assert.AreEqual(new SKColor(0xC0, 0xC0, 0xC0), bitmap.GetPixel(cx, top - 2));
                    Assert.AreNotEqual(new SKColor(0xC0, 0xC0, 0xC0), bitmap.GetPixel(cx, top + 2));
                }

                runner.ClickMessage();
                runner.ClickMessage();
                runner.WaitForTermination();
            }
        }
    }
}
