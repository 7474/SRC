using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using SRCPlay;
using System;
using System.IO;

namespace SRCRender.Tests
{
    /// <summary>
    /// クリック待ち（Wait Click）の間のキー操作のポインタ（SRC#Web の仮想パッドの十字キー）と、
    /// メインウィンドウを隠している間（プロローグなど）の表示。
    /// </summary>
    [TestClass]
    public class KeyPointerTests
    {
        private ScenarioWorkspace workspace;

        [TestInitialize]
        public void Initialize()
        {
            workspace = new ScenarioWorkspace();
            workspace.WriteMap("test.map", 20, 20, (x, y) => 0);
            workspace.Write("test.eve",
                "プロローグ:",
                "Talk システム",
                "メインウィンドウを隠している",
                "End",
                "ChangeMap test.map",
                "HotPoint 上 100 100 80 24 上のボタン",
                "HotPoint 右 300 100 80 24 右のボタン",
                "HotPoint 下 100 200 80 24 下のボタン",
                "Refresh",
                "Wait Click",
                "Talk システム",
                "選択=$(選択)",
                "End",
                "Quit",
                "");
        }

        [TestCleanup]
        public void Cleanup()
        {
            workspace.Dispose();
        }

        private static SKColor Pixel(ScreenScenarioRunner runner, int x, int y)
        {
            using (var png = new MemoryStream())
            {
                runner.GUI.SnapshotPng(png);
                using (var bitmap = SKBitmap.Decode(png.ToArray()))
                {
                    return bitmap.GetPixel(x, y);
                }
            }
        }

        [TestMethod]
        public void HiddenMainForm_PaintsMapAndStatusBlack()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                StringAssert.Contains(runner.WaitForMessage(), "メインウィンドウを隠している");
                Assert.IsFalse(runner.State.MainFormVisible);
                // マップ（左上）とステータス（右）を描かない
                Assert.AreEqual(SKColors.Black, Pixel(runner, 100, 100));
                Assert.AreEqual(SKColors.Black, Pixel(runner, ScreenLayout.StatusArea.MidX, 100));
            }
        }

        [TestMethod]
        public void KeyPointer_JumpsBetweenHotPointsAndClicks()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitForMessage();
                runner.ClickMessage();
                runner.WaitFor(s => runner.GUI.IsWaitingClick, "click wait");
                Assert.IsTrue(runner.State.MainFormVisible);

                void Press(ScreenKey key)
                {
                    runner.GUI.KeyPressed(key);
                }

                // 最初はホットポイントの先頭を指す
                Press(ScreenKey.Down);
                runner.WaitFor(s => s.KeyPointerTarget == SKRectI.Create(100, 100, 80, 24), "pointer on 上");
                runner.WaitFor(s => s.Tooltip?.Text == "上のボタン", "tooltip");
                // → で右のホットポイント、← で戻り、↓ で下のホットポイントへ跳ぶ
                Press(ScreenKey.Right);
                runner.WaitFor(s => s.KeyPointerTarget == SKRectI.Create(300, 100, 80, 24), "pointer on 右");
                Press(ScreenKey.Left);
                runner.WaitFor(s => s.KeyPointerTarget == SKRectI.Create(100, 100, 80, 24), "pointer back on 上");
                Press(ScreenKey.Down);
                runner.WaitFor(s => s.KeyPointerTarget == SKRectI.Create(100, 200, 80, 24), "pointer on 下");
                // その方向にホットポイントが無ければ、少しずつ動く（ホットポイントの外に出る）
                Press(ScreenKey.Down);
                runner.WaitFor(s => s.KeyPointer?.Y == 200 + 12 + 16 && s.KeyPointerTarget == null, "free move");
                Press(ScreenKey.Up);
                runner.WaitFor(s => s.KeyPointerTarget == SKRectI.Create(100, 200, 80, 24), "pointer on 下 again");
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "TestResults"));
                using (var png = File.Create(Path.Combine(AppContext.BaseDirectory, "TestResults", "key-pointer.png")))
                {
                    runner.GUI.SnapshotPng(png);
                }

                // Enter でその位置をクリックする（選択にホットポイントの名前が入る）
                Press(ScreenKey.Enter);
                StringAssert.Contains(runner.WaitForMessage(), "選択=下");
                Assert.IsNull(runner.State.KeyPointer, "クリック待ちが終わればポインタは出さない");
            }
        }
    }
}
