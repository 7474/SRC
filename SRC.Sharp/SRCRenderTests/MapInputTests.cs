using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using System.IO;
using System.Threading;
using SRCPlay;

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
        public void BeginMapDrag_ScrollsMapWithoutClick()
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
                runner.Session.Game.Post(() => runner.GUI.UnlockGUI());
                runner.WaitFor(s => !runner.GUI.IsGUILocked, "unlocked");
                var mapX = runner.GUI.MapX;
                var mapY = runner.GUI.MapY;

                // タッチで指を滑らせた場合: クリックにせずドラッグだけを始める
                runner.GUI.BeginMapDrag(300, 300);
                runner.GUI.PointerMoved(300 - 32 * 3, 300 - 32 * 2);
                runner.WaitFor(s => runner.GUI.MapX == mapX + 3 && runner.GUI.MapY == mapY + 2, "map scrolled");
                runner.GUI.EndMapDrag();
                // クリックにはなっていない（Wait Click のまま）
                Assert.IsTrue(runner.GUI.IsWaitingClick);

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

        [TestMethod]
        public void PopupMenuAndDialog_LookLikeWindows()
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

                // ポップアップメニュー（カーソル位置に表示し、カーソル下の項目を強調する）
                runner.GUI.PointerMoved(100, 100);
                runner.Session.Game.Post(() => runner.GUI.ShowMapCommandMenu(new[]
                {
                    new SRCCore.Commands.UiCommand(1, "部隊表"),
                    new SRCCore.Commands.UiCommand(2, "スペシャルパワー"),
                    new SRCCore.Commands.UiCommand(3, "自動反撃モード", true),
                    new SRCCore.Commands.UiCommand(4, "ターン終了"),
                }));
                runner.WaitFor(s => s.Menu != null, "menu");
                var box = runner.GUI.Renderer.LayoutMenu(runner.State.Menu);
                runner.GUI.PointerMoved(box.MidX, box.Top + 3 + 20 * 1 + 10);
                runner.WaitFor(s => s.Menu.Highlight == 1, "highlight");
                Assert.AreEqual(1, runner.GUI.Renderer.HitTestMenu(runner.State.Menu, box.MidX, box.Top + 3 + 20 * 1 + 10));
                runner.Sync();
                VisualAssert.MatchesBaseline(runner.SnapshotPng(), "map-03-menu");
                runner.Session.Game.Post(() => runner.State.Menu = null);
                runner.WaitFor(s => s.Menu == null, "menu closed");

                // メッセージボックス
                var result = SRCCore.GuiDialogResult.Cancel;
                var closed = new ManualResetEventSlim();
                runner.Session.Game.Post(() =>
                {
                    result = runner.GUI.Confirm("データを保存しますか？", "確認", SRCCore.GuiConfirmOption.OkCancel);
                    closed.Set();
                });
                runner.WaitFor(s => s.Dialog != null, "dialog");
                runner.Sync();
                VisualAssert.MatchesBaseline(runner.SnapshotPng(), "map-04-dialog");
                var ok = runner.GUI.Renderer.LayoutDialog(runner.State.Dialog).ButtonRect(0);
                runner.GUI.PointerPressed(ok.MidX, ok.MidY, GuiButton.Left);
                Assert.IsTrue(closed.Wait(ScreenScenarioRunner.Timeout), "dialog closed");
                Assert.AreEqual(SRCCore.GuiDialogResult.Ok, result);
            }
        }

        [TestMethod]
        public void MoveCursorPos_MovesToMessageWindowAndRestores()
        {
            workspace.Write("test.eve",
                "プロローグ:",
                "ChangeMap test.map",
                "Talk システム",
                "カーソルのテスト",
                "End",
                "Quit",
                "");
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.CursorEnabled = true;
                runner.CursorX = 100;
                runner.CursorY = 100;
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitForMessage();

                using (var done = new ManualResetEventSlim())
                {
                    runner.Session.Game.Post(() =>
                    {
                        runner.GUI.SaveCursorPos();
                        runner.GUI.MoveCursorPos("メッセージウィンドウ");
                        done.Set();
                    });
                    Assert.IsTrue(done.Wait(ScreenScenarioRunner.Timeout));
                }
                // 少しずつ動かしてメッセージウィンドウの文字の部分で止まる
                Assert.IsTrue(runner.CursorMoves.Count > 1, runner.CursorMoves.Count.ToString());
                var origin = runner.GUI.Renderer.MessageWindowOrigin(runner.GUI.MessageWindow);
                Assert.IsTrue(runner.CursorY >= origin.Y + runner.GUI.MessageWindow.picMessage.Top, runner.CursorY.ToString());
                Assert.IsTrue(runner.CursorX >= origin.X, runner.CursorX.ToString());

                // 元の位置に戻す
                using (var done = new ManualResetEventSlim())
                {
                    runner.Session.Game.Post(() =>
                    {
                        runner.GUI.RestoreCursorPos();
                        done.Set();
                    });
                    Assert.IsTrue(done.Wait(ScreenScenarioRunner.Timeout));
                }
                Assert.AreEqual(100f, runner.CursorX, 0.5f);
                Assert.AreEqual(100f, runner.CursorY, 0.5f);
            }
        }
    }
}
