using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCPlay;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace SRCRender.Tests
{
    /// <summary>
    /// キー操作（物理キー・SRC#Web の仮想キー）だけで遊べること。
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
                    // ↑↓ で選び、Esc で取り消す
                    Press(ScreenKey.Down);
                    Assert.AreEqual(0, state.Menu.Highlight);
                    Press(ScreenKey.Down);
                    Assert.AreEqual(1, state.Menu.Highlight);
                    Press(ScreenKey.Escape);
                    WaitUntil(() => state.Menu == null, "メニューの取り消し");

                    // 何も無いマスで Esc（右クリック）→ マップのコマンドのメニュー →「ターン終了」→ 確認のダイアログ
                    MoveTo(3, 3);
                    Press(ScreenKey.Escape);
                    WaitUntil(() => state.Menu != null, "マップのコマンドのメニュー");
                    var end = state.Menu.Items.IndexOf("ターン終了");
                    Assert.IsTrue(end >= 0, string.Join(",", state.Menu.Items));
                    for (var i = 0; i <= end; i++)
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
                    for (var i = 0; i <= move; i++)
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
    }
}
