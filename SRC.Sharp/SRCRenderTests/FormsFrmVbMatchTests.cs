using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using SRCCore;
using SRCPlay;
using SRCRender.Compat;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace SRCRender.Tests
{
    /// <summary>
    /// VB のフォーム（Main.frm・ListBox.frm・Message.frm）の手続きに当たる入力の処理を、VB の動作を期待値にして確かめる（#1498）。
    /// </summary>
    [TestClass]
    public class FormsFrmVbMatchTests
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

        private static void Unlock(ScreenScenarioRunner runner)
        {
            // ユニットの居ないマップではコマンド待ちにならないため、ＧＵＩのロックを外してマップ操作の状態にする
            runner.Session.Game.Post(() => runner.GUI.UnlockGUI());
            runner.WaitFor(s => !runner.GUI.IsGUILocked, "unlocked");
        }

        private static void Finish(ScreenScenarioRunner runner)
        {
            runner.Session.Game.Post(() => runner.GUI.LockGUI());
            runner.WaitFor(s => runner.GUI.IsGUILocked, "locked");
            runner.GUI.KeyPressed(ScreenKey.Enter);
            runner.WaitForTermination();
        }

        // Main.frm の picMain_MouseMove: MapX = PrevMapX - (X - PrevMouseX) \ 32。
        // 右へ 40 ピクセルのドラッグは 40 \ 32 = 1 マス（小数部を切り捨てるのは商で、MapX ではない）
        [TestMethod]
        public void Drag_RightByLessThanTwoCells_MovesOneCell()
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
                Unlock(runner);
                var mapX = runner.GUI.MapX;
                var mapY = runner.GUI.MapY;
                Assert.IsTrue(mapX >= 3 && mapY >= 3);

                runner.GUI.PointerPressed(300, 300, GuiButton.Left);
                runner.GUI.PointerMoved(300 + 40, 300 + 40);
                runner.WaitFor(s => runner.GUI.MapX != mapX, "map scrolled");
                Thread.Sleep(200);
                Assert.AreEqual(mapX - 1, runner.GUI.MapX);
                Assert.AreEqual(mapY - 1, runner.GUI.MapY);

                // 1 マスに満たない分は動かない（16 ピクセルでも動かない）
                runner.GUI.PointerMoved(300 + 40 + 16, 300 + 40 + 16);
                Thread.Sleep(200);
                Assert.AreEqual(mapX - 1, runner.GUI.MapX);
                runner.GUI.PointerReleased(300 + 56, 300 + 56, GuiButton.Left);

                Finish(runner);
            }
        }

        private static byte[] StatusPixels(ScreenScenarioRunner runner)
        {
            using (var bitmap = SKBitmap.Decode(runner.SnapshotPng()))
            using (var status = new SKBitmap(ScreenLayout.StatusArea.Width, ScreenLayout.StatusArea.Height))
            {
                bitmap.ExtractSubset(status, ScreenLayout.StatusArea);
                return status.Copy().Bytes;
            }
        }

        // Main.frm の picMain_MouseMove: マップ外のマスにカーソルがあれば DisplayGlobalStatus（ターン数と資金だけ）を出す
        [TestMethod]
        public void Hover_OutsideMap_DisplaysGlobalStatus()
        {
            workspace.Write("test.eve",
                "プロローグ:",
                "ChangeMap test.map",
                "Center 1 1",
                "Wait Click",
                "Quit",
                "");
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitFor(s => runner.GUI.IsWaitingClick, "wait click");
                Unlock(runner);
                Assert.AreEqual(1, runner.GUI.MapX);

                // マップの中のマス（地形の情報も出る）
                runner.GUI.PointerMoved(ScreenLayout.MapWidthPx - 16, ScreenLayout.MapHeightPx - 16);
                Thread.Sleep(300);
                runner.Sync();
                var inside = StatusPixels(runner);

                // 左上はマップの外（MapX = 1 なので左の 7 列はマップの外）
                runner.GUI.PointerMoved(16, 16);
                Thread.Sleep(300);
                runner.Sync();
                var outside = StatusPixels(runner);
                Assert.IsFalse(inside.SequenceEqual(outside), "マップ外ではターン数と資金だけの表示に変わる");

                Finish(runner);
            }
        }

        // ListBox.frm の lstItems_MouseMove: 最後の項目より下では ListIndex = -1（どの項目もハイライトしない）
        [TestMethod]
        public void ListBox_HoverBelowLastItem_ClearsHighlight()
        {
            workspace.Write("test.eve",
                "プロローグ:",
                "ChangeMap test.map",
                "Talk システム",
                "リストのテスト",
                "End",
                "Quit",
                "");
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitForMessage();
                var args = new ListBoxArgs
                {
                    lb_caption = "通常",
                    lb_info = "名称",
                    Items = Enumerable.Range(1, 3).Select(i => new ListBoxItem($"項目{i}", $"id{i}")).ToList<ListBoxItem>(),
                };
                var result = -1;
                var done = new ManualResetEventSlim();
                runner.Session.Game.Post(() =>
                {
                    result = runner.GUI.ListBox(args);
                    done.Set();
                });
                var list = runner.WaitForList();
                var w = list.Window;
                Assert.IsFalse(w.lstItems.HasScrollBar);
                var layout = runner.GUI.Renderer.LayoutList(list);
                var item = layout.ItemRect(1);
                runner.GUI.PointerMoved(item.MidX, item.MidY);
                runner.WaitFor(s => s.List.Highlight == 1, "highlight");

                // 最後の項目より下（リストの中）
                var below = w.ScreenTop + Form.TitleBarHeight + w.lstItems.Top + w.lstItems.Height - 4;
                Assert.IsTrue(below > layout.ItemRect(2).Bottom);
                runner.GUI.PointerMoved(item.MidX, below);
                runner.WaitFor(s => s.List.Highlight == -1, "highlight cleared");

                runner.GUI.KeyPressed(ScreenKey.Escape);
                Assert.IsTrue(done.Wait(ScreenScenarioRunner.Timeout));
                Assert.AreEqual(0, result);
            }
        }
    }
}
