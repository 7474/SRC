using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCPlay;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace SRCRender.Tests
{
    /// <summary>
    /// キー操作（物理キー・SRC#Web の仮想パッド）だけで遊べること。
    /// マップカーソル（矢印で選び Enter でクリック、Esc で右クリック）、ポップアップメニュー（↑↓・Enter・Esc）、ダイアログ（←→・Enter）。
    /// </summary>
    [TestClass]
    public class KeyboardControlTests
    {
        private static void Copy(string from, string to)
        {
            Directory.CreateDirectory(to);
            foreach (var file in Directory.GetFiles(from))
            {
                File.Copy(file, Path.Combine(to, Path.GetFileName(file)), true);
            }
            foreach (var dir in Directory.GetDirectories(from))
            {
                Copy(dir, Path.Combine(to, Path.GetFileName(dir)));
            }
        }

        private static void WaitUntil(Func<bool> condition, string what)
        {
            var deadline = DateTime.UtcNow.AddSeconds(10);
            while (!condition())
            {
                if (DateTime.UtcNow > deadline)
                {
                    Assert.Fail(what + " になりません");
                }
                Thread.Sleep(20);
            }
        }

        [TestMethod]
        public void MapCursor_ShowsUnitStatus()
        {
            using (var workspace = new ScenarioWorkspace())
            {
                Copy(Path.Combine(E2ECorpusTests.CorpusDirectory, "battle-basic"), workspace.AppPath);
                using (var runner = new ScreenScenarioRunner(workspace.AppPath))
                {
                    var gui = runner.GUI;
                    var state = runner.State;
                    var script = new PlayScript(runner);
                    runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                    script.Run(new[] { "auto 60" });

                    void Press(ScreenKey key)
                    {
                        gui.KeyPressed(key);
                        runner.Sync();
                    }

                    (int x, int y) Cursor()
                    {
                        WaitUntil(() => state.MapCursor != null, "マップカーソルの表示");
                        var rect = state.MapCursor.Value;
                        return (gui.PixelToMapX(rect.Left), gui.PixelToMapY(rect.Top));
                    }

                    void MoveTo(int x, int y)
                    {
                        Press(ScreenKey.Right);
                        Press(ScreenKey.Left);
                        for (var i = 0; i < 40 && Cursor() != (x, y); i++)
                        {
                            var (cx, cy) = Cursor();
                            Press(cx < x ? ScreenKey.Right : cx > x ? ScreenKey.Left : cy < y ? ScreenKey.Down : ScreenKey.Up);
                        }
                        Assert.AreEqual((x, y), Cursor());
                        runner.Sync();
                    }

                    // マウスを置いたときと同じく、カーソルのマスのユニットをステータスに出す
                    MoveTo(6, 6);
                    WaitUntil(() => gui.DisplayedUnit?.Name == "テスト機", "味方ユニットのステータス（" + gui.DisplayedUnit?.Name + "）");
                    MoveTo(12, 6);
                    WaitUntil(() => gui.DisplayedUnit?.Name == "標的機", "敵ユニットのステータス（" + gui.DisplayedUnit?.Name + "）");
                    // ユニットのいないマスでは、そのマスの地形を出す（マウスの位置もカーソルのマスに合わせる）
                    MoveTo(9, 8);
                    WaitUntil(() => gui.DisplayedUnit == null, "地形の表示");
                    Assert.AreEqual((9, 8), (gui.PixelToMapX((int)gui.MouseX), gui.PixelToMapY((int)gui.MouseY)));
                    MoveTo(6, 6);
                    WaitUntil(() => gui.DisplayedUnit?.Name == "テスト機", "味方ユニットのステータスに戻る（" + gui.DisplayedUnit?.Name + "）");

                    // 移動先を選んでいる間も、カーソルのマスのユニットを出す
                    Press(ScreenKey.Enter);
                    WaitUntil(() => state.Menu != null, "ユニットのコマンドのメニュー");
                    var move = state.Menu.Items.IndexOf("移動");
                    for (var i = 0; i < move; i++)
                    {
                        Press(ScreenKey.Down);
                    }
                    Press(ScreenKey.Enter);
                    WaitUntil(() => state.Menu == null, "移動を選ぶ");
                    MoveTo(12, 6);
                    WaitUntil(() => gui.DisplayedUnit?.Name == "標的機", "移動先の選択中の敵ユニットのステータス（" + gui.DisplayedUnit?.Name + "）");
                    Assert.IsNull(runner.Error);
                }
            }
        }

        [TestMethod]
        public void MapCursorMenuAndDialog_CanBeOperatedWithKeysOnly()
        {
            using (var workspace = new ScenarioWorkspace())
            {
                Copy(Path.Combine(E2ECorpusTests.CorpusDirectory, "battle-basic"), workspace.AppPath);
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "TestResults"));
                using (var runner = new ScreenScenarioRunner(workspace.AppPath))
                {
                    var gui = runner.GUI;
                    var state = runner.State;
                    var script = new PlayScript(runner);
                    runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                    // 「戦闘開始」を送り、味方フェイズにする
                    script.Run(new[] { "auto 60" });

                    void Press(ScreenKey key)
                    {
                        gui.KeyPressed(key);
                        runner.Sync();
                    }

                    (int x, int y) Cursor()
                    {
                        WaitUntil(() => state.MapCursor != null, "マップカーソルの表示");
                        var rect = state.MapCursor.Value;
                        return (gui.PixelToMapX(rect.Left), gui.PixelToMapY(rect.Top));
                    }

                    void MoveTo(int x, int y)
                    {
                        Press(ScreenKey.Right);
                        Press(ScreenKey.Left);
                        for (var i = 0; i < 40 && Cursor() != (x, y); i++)
                        {
                            var (cx, cy) = Cursor();
                            Press(cx < x ? ScreenKey.Right : cx > x ? ScreenKey.Left : cy < y ? ScreenKey.Down : ScreenKey.Up);
                        }
                        Assert.AreEqual((x, y), Cursor());
                    }

                    // 味方ユニット（6,6）にカーソルを合わせ、Enter でコマンドのメニューを出す
                    MoveTo(6, 6);
                    Press(ScreenKey.Enter);
                    WaitUntil(() => state.Menu != null, "ユニットのコマンドのメニュー");
                    CollectionAssert.Contains(state.Menu.Items.ToList(), "移動");
                    // キーで開いたメニューは先頭の項目を選んでいる。↑↓ で選び、Esc で取り消す
                    Assert.AreEqual(0, state.Menu.Highlight);
                    Press(ScreenKey.Down);
                    Assert.AreEqual(1, state.Menu.Highlight);
                    Press(ScreenKey.Up);
                    Assert.AreEqual(0, state.Menu.Highlight);
                    Press(ScreenKey.Escape);
                    WaitUntil(() => state.Menu == null, "メニューの取り消し");

                    // 何も無いマスで Esc（右クリック）→ マップのコマンドのメニュー →「ターン終了」→ 確認のダイアログ
                    MoveTo(3, 3);
                    Press(ScreenKey.Escape);
                    WaitUntil(() => state.Menu != null, "マップのコマンドのメニュー");
                    var end = state.Menu.Items.IndexOf("ターン終了");
                    Assert.IsTrue(end >= 0, string.Join(",", state.Menu.Items));
                    Assert.AreEqual(0, state.Menu.Highlight);
                    for (var i = 0; i < end; i++)
                    {
                        Press(ScreenKey.Down);
                    }
                    Assert.AreEqual(end, state.Menu.Highlight);
                    Press(ScreenKey.Enter);
                    WaitUntil(() => state.Dialog != null, "確認のダイアログ");
                    Assert.IsTrue(state.Dialog.Buttons.Count >= 2);
                    // → で「キャンセル」を選び、Enter で押す（ターンは終わらない）
                    Press(ScreenKey.Right);
                    Assert.AreEqual(1, state.Dialog.Highlight);
                    Press(ScreenKey.Enter);
                    WaitUntil(() => state.Dialog == null, "ダイアログを閉じる");
                    script.Run(new[] { "wait ready" });

                    // 移動: メニューで「移動」を選び、移動先のマスをカーソルで選んで Enter
                    MoveTo(6, 6);
                    Press(ScreenKey.Enter);
                    WaitUntil(() => state.Menu != null, "ユニットのコマンドのメニュー");
                    var move = state.Menu.Items.IndexOf("移動");
                    for (var i = 0; i < move; i++)
                    {
                        Press(ScreenKey.Down);
                    }
                    Press(ScreenKey.Enter);
                    WaitUntil(() => state.Menu == null, "移動を選ぶ");
                    MoveTo(9, 6);
                    using (var png = File.Create(Path.Combine(AppContext.BaseDirectory, "TestResults", "keyboard-cursor.png")))
                    {
                        gui.SnapshotPng(png);
                    }
                    Press(ScreenKey.Enter);
                    // 移動した後のコマンドのメニュー（攻撃・待機など）が出る
                    WaitUntil(() => state.Menu != null, "移動後のコマンドのメニュー");
                    CollectionAssert.Contains(state.Menu.Items.ToList(), "待機", string.Join(",", state.Menu.Items));
                    Assert.IsNull(runner.Error);
                }
            }
        }

        [TestMethod]
        public void TouchTap_PlacesMapCursor_AndKeysContinueFromIt()
        {
            using (var workspace = new ScenarioWorkspace())
            {
                Copy(Path.Combine(E2ECorpusTests.CorpusDirectory, "battle-basic"), workspace.AppPath);
                using (var runner = new ScreenScenarioRunner(workspace.AppPath))
                {
                    var gui = runner.GUI;
                    var state = runner.State;
                    var script = new PlayScript(runner);
                    runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                    script.Run(new[] { "auto 60" });

                    void Press(ScreenKey key)
                    {
                        gui.KeyPressed(key);
                        runner.Sync();
                    }

                    (int x, int y) Cursor()
                    {
                        WaitUntil(() => state.MapCursor != null, "マップカーソルの表示");
                        var rect = state.MapCursor.Value;
                        return (gui.PixelToMapX(rect.Left), gui.PixelToMapY(rect.Top));
                    }

                    // 味方ユニット（6,6）の左隣のマスをタップした（指が隣のマスに当たった）
                    float px = gui.MapToPixelX(5) + 16;
                    float py = gui.MapToPixelY(6) + 16;
                    gui.PointerMoved(px, py, touch: true);
                    gui.PointerPressed(px, py, SRCCore.GuiButton.Left, 1, touch: true);
                    gui.PointerReleased(px, py, SRCCore.GuiButton.Left);
                    runner.Sync();
                    // 当たったマスにカーソルが出る（何も無いマスなので、ほかには何も起きない）
                    Assert.AreEqual((5, 6), Cursor());
                    Assert.IsNull(state.Menu);

                    // 十字キーで 1 マス直して、A（Enter）で選ぶ
                    Press(ScreenKey.Right);
                    Assert.AreEqual((6, 6), Cursor());
                    Press(ScreenKey.Enter);
                    WaitUntil(() => state.Menu != null, "ユニットのコマンドのメニュー");
                    CollectionAssert.Contains(state.Menu.Items.ToList(), "移動");
                    Press(ScreenKey.Escape);
                    WaitUntil(() => state.Menu == null, "メニューの取り消し");

                    // ユニットをタップして開いたメニューも先頭を選んでいて、A を 1 回押せば選べる
                    float ux = gui.MapToPixelX(6) + 16;
                    float uy = gui.MapToPixelY(6) + 16;
                    gui.PointerMoved(ux, uy, touch: true);
                    gui.PointerPressed(ux, uy, SRCCore.GuiButton.Left, 1, touch: true);
                    gui.PointerReleased(ux, uy, SRCCore.GuiButton.Left);
                    runner.Sync();
                    WaitUntil(() => state.Menu != null, "タップで開いたユニットのコマンドのメニュー");
                    Assert.AreEqual(0, state.Menu.Highlight);
                    Press(ScreenKey.Escape);
                    WaitUntil(() => state.Menu == null, "メニューの取り消し");

                    // マウスの操作ではカーソルを消す（これまでどおり）
                    gui.PointerMoved(px, py);
                    WaitUntil(() => state.MapCursor == null, "マップカーソルを消す");
                    Assert.IsNull(runner.Error);
                }
            }
        }

        [TestMethod]
        public void Enter_WithoutMapCursor_ShowsCursorAndSelects()
        {
            using (var workspace = new ScenarioWorkspace())
            {
                Copy(Path.Combine(E2ECorpusTests.CorpusDirectory, "battle-basic"), workspace.AppPath);
                using (var runner = new ScreenScenarioRunner(workspace.AppPath))
                {
                    var gui = runner.GUI;
                    var state = runner.State;
                    var script = new PlayScript(runner);
                    runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                    script.Run(new[] { "auto 60" });

                    // 味方ユニット（6,6）にポインタを置いてから離した（カーソルは出ていない）
                    gui.PointerMoved(gui.MapToPixelX(6) + 16, gui.MapToPixelY(6) + 16);
                    runner.Sync();
                    Assert.IsNull(state.MapCursor);

                    // 1 回の A（Enter）で、そのマスにカーソルを出して選ぶ
                    gui.KeyPressed(ScreenKey.Enter);
                    runner.Sync();
                    WaitUntil(() => state.Menu != null, "ユニットのコマンドのメニュー");
                    CollectionAssert.Contains(state.Menu.Items.ToList(), "移動");
                    WaitUntil(() => state.MapCursor != null, "マップカーソルの表示");
                    var rect = state.MapCursor.Value;
                    Assert.AreEqual((6, 6), (gui.PixelToMapX(rect.Left), gui.PixelToMapY(rect.Top)));
                    Assert.IsNull(runner.Error);
                }
            }
        }

        [TestMethod]
        public void MapCommandMenu_AfterCancel_OpensAgainWithOnePress()
        {
            using (var workspace = new ScenarioWorkspace())
            {
                Copy(Path.Combine(E2ECorpusTests.CorpusDirectory, "battle-basic"), workspace.AppPath);
                using (var runner = new ScreenScenarioRunner(workspace.AppPath))
                {
                    var gui = runner.GUI;
                    var state = runner.State;
                    var script = new PlayScript(runner);
                    runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                    script.Run(new[] { "auto 60" });

                    void Press(ScreenKey key)
                    {
                        gui.KeyPressed(key);
                        runner.Sync();
                    }

                    // 何も無いマス（3,3）にカーソルを置く
                    Press(ScreenKey.Right);
                    for (var i = 0; i < 40; i++)
                    {
                        WaitUntil(() => state.MapCursor != null, "マップカーソルの表示");
                        var rect = state.MapCursor.Value;
                        var (cx, cy) = (gui.PixelToMapX(rect.Left), gui.PixelToMapY(rect.Top));
                        if ((cx, cy) == (3, 3))
                        {
                            break;
                        }
                        Press(cx > 3 ? ScreenKey.Left : cx < 3 ? ScreenKey.Right : cy > 3 ? ScreenKey.Up : ScreenKey.Down);
                    }

                    // B（Esc）でマップのコマンドのメニューを開いて閉じる。もう一度 B を 1 回押せば、また開く
                    for (var round = 0; round < 3; round++)
                    {
                        Press(ScreenKey.Escape);
                        WaitUntil(() => state.Menu != null, "マップのコマンドのメニュー（" + round + " 回目）");
                        CollectionAssert.Contains(state.Menu.Items.ToList(), "ターン終了");
                        Press(ScreenKey.Escape);
                        WaitUntil(() => state.Menu == null, "メニューを閉じる");
                        runner.Sync();
                    }
                    Assert.IsNull(runner.Error);
                }
            }
        }

        [TestMethod]
        public void EscapeHeld_CountsAsRightButtonHeld()
        {
            using (var workspace = new ScenarioWorkspace())
            {
                Copy(Path.Combine(E2ECorpusTests.CorpusDirectory, "battle-basic"), workspace.AppPath);
                using (var runner = new ScreenScenarioRunner(workspace.AppPath))
                {
                    var gui = runner.GUI;
                    runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                    Assert.IsFalse(gui.IsRButtonPressed(true));

                    // 仮想パッドの B（Esc）を押している間は、右ボタンを押し続けているのと同じ（メッセージを飛ばす）
                    gui.KeyStateChanged(0x1B, true);
                    Assert.IsTrue(gui.IsRButtonPressed(true));
                    gui.KeyStateChanged(0x1B, false);
                    Assert.IsFalse(gui.IsRButtonPressed(true));

                    // ほかのキーは右ボタンにしない
                    gui.KeyStateChanged(0x0D, true);
                    Assert.IsFalse(gui.IsRButtonPressed(true));
                    gui.KeyStateChanged(0x0D, false);
                }
            }
        }

        [TestMethod]
        public void EnterHeld_CountsAsLeftButtonHeld()
        {
            using (var workspace = new ScenarioWorkspace())
            {
                Copy(Path.Combine(E2ECorpusTests.CorpusDirectory, "battle-basic"), workspace.AppPath);
                using (var runner = new ScreenScenarioRunner(workspace.AppPath))
                {
                    var gui = runner.GUI;
                    runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                    Assert.IsFalse(gui.IsLButtonPressed());

                    // 仮想パッドの A（Enter）を押している間は、左ボタンを押し続けているのと同じ（ウェイト付きでメッセージを送る）
                    gui.KeyStateChanged(0x0D, true);
                    Assert.IsTrue(gui.IsLButtonPressed());
                    gui.KeyStateChanged(0x0D, false);
                    Assert.IsFalse(gui.IsLButtonPressed());

                    // ほかのキーは左ボタンにしない
                    gui.KeyStateChanged(0x1B, true);
                    Assert.IsFalse(gui.IsLButtonPressed());
                    gui.KeyStateChanged(0x1B, false);
                }
            }
        }
    }
}
