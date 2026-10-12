using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using System.IO;
using System.Threading;

namespace SRCRender.Tests
{
    /// <summary>
    /// マップ画面のマウス操作（ホットポイント・ドラッグ）。
    /// </summary>
    [TestClass]
    public class MapInputTests
    {
        private ScenarioWorkspace workspace;

        [TestInitialize]
        public void Initialize()
        {
            workspace = new ScenarioWorkspace();
            workspace.WriteMap("test.map", 40, 40, (x, y) => (x + y) % 2);
        }

        [TestCleanup]
        public void Cleanup()
        {
            workspace.Dispose();
        }

        [TestMethod]
        public void HotPoint_ShowsTooltipWhileWaitingClick()
        {
            workspace.Write("test.eve",
                "プロローグ:",
                "ChangeMap test.map",
                "HotPoint 出口 100 100 64 32 出口へ行く",
                "Wait Click",
                "Talk システム",
                "クリックされた",
                "End",
                "Quit",
                "");
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitFor(s => runner.GUI.IsWaitingClick, "wait click");

                runner.GUI.PointerMoved(110, 110);
                runner.WaitFor(s => s.Tooltip?.Text == "出口へ行く", "tooltip");
                VisualAssert.MatchesBaseline(runner.SnapshotPng(), "map-01-hotpoint");

                runner.GUI.PointerMoved(300, 300);
                runner.WaitFor(s => s.Tooltip == null, "tooltip hidden");

                runner.GUI.PointerPressed(110, 110, GuiButton.Left);
                runner.GUI.PointerReleased(110, 110, GuiButton.Left);
                Assert.AreEqual("クリックされた", runner.WaitForMessage());
            }
        }

        [TestMethod]
        public void Drag_ScrollsMap()
        {
            workspace.Write("test.eve",
                "プロローグ:",
                "ChangeMap test.map",
                "Wait Click",
                "Quit",
                "");
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitFor(s => runner.GUI.IsWaitingClick, "wait click");
                // ユニットの居ないマップではコマンド待ちにならないため、ＧＵＩのロックを外してマップ操作の状態にする
                runner.Session.Game.Post(() => runner.GUI.UnlockGUI());
                runner.WaitFor(s => !runner.GUI.IsGUILocked, "unlocked");
                var mapX = runner.GUI.MapX;
                var mapY = runner.GUI.MapY;

                // 左に 3 マス、上に 2 マスドラッグすると、表示位置は右下へ動く
                runner.GUI.PointerPressed(300, 300, GuiButton.Left);
                runner.GUI.PointerMoved(300 - 32 * 3, 300 - 32 * 2);
                runner.WaitFor(s => runner.GUI.MapX == mapX + 3 && runner.GUI.MapY == mapY + 2, "map scrolled");
                runner.GUI.PointerReleased(300 - 32 * 3, 300 - 32 * 2, GuiButton.Left);
                VisualAssert.MatchesBaseline(runner.SnapshotPng(), "map-02-dragged");

                // ドラッグ終了後は動かない
                runner.GUI.PointerMoved(300, 300);
                Thread.Sleep(200);
                Assert.AreEqual(mapX + 3, runner.GUI.MapX);

                runner.Session.Game.Post(() => runner.GUI.LockGUI());
                runner.WaitFor(s => runner.GUI.IsGUILocked, "locked");
                runner.GUI.KeyPressed(ScreenKey.Enter);
                runner.WaitForTermination();
            }
        }

        [TestMethod]
        public void KeyState_ReflectsPressedKeys()
        {
            workspace.Write("test.eve",
                "プロローグ:",
                "ChangeMap test.map",
                "Wait Click",
                "Talk システム",
                "Z=$(KeyState(90)) X=$(KeyState(88))",
                "End",
                "Quit",
                "");
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitFor(s => runner.GUI.IsWaitingClick, "wait click");
                // Z キー（仮想キーコード 0x5A）を押したままクリックする
                runner.GUI.KeyStateChanged(0x5A, true);
                runner.GUI.KeyPressed(ScreenKey.Enter);
                Assert.AreEqual("Z=1 X=0", runner.WaitForMessage());
                runner.GUI.KeyStateChanged(0x5A, false);
                Assert.IsFalse(runner.GUI.GetKeyState(0x5A));
            }
        }
    }
}
