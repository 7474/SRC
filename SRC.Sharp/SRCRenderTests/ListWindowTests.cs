using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using SRCRender.Compat;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace SRCRender.Tests
{
    /// <summary>
    /// リストボックス（通常・多段・出撃選択）の操作と表示。
    /// </summary>
    [TestClass]
    public class ListWindowTests
    {
        private ScenarioWorkspace workspace;

        [TestInitialize]
        public void Initialize()
        {
            workspace = new ScenarioWorkspace();
            workspace.WriteMap("test.map", 20, 20, (x, y) => x <= 10 ? 0 : 1);
            workspace.Write("test.eve",
                "プロローグ:",
                "ChangeMap test.map",
                "Talk システム",
                "リストのテスト",
                "End",
                "Quit",
                "");
        }

        [TestCleanup]
        public void Cleanup()
        {
            workspace.Dispose();
        }

        private static ListBoxArgs Args(int count, string caption, bool disableSecond = false)
        {
            return new ListBoxArgs
            {
                lb_caption = caption,
                lb_info = "名称",
                HasFlag = true,
                Items = Enumerable.Range(1, count)
                    .Select(i => new ListBoxItem($"項目{i:00}", $"id{i}") { ListItemFlag = disableSecond && i == 2, ListItemComment = $"解説{i}" })
                    .ToList<ListBoxItem>(),
            };
        }

        /// <summary>ゲームスレッドで GUI を呼び、結果を返すまで待てるようにする。</summary>
        private static Func<int> RunOnGame(ScreenScenarioRunner runner, Func<int> call)
        {
            var result = -1;
            Exception error = null;
            var done = new ManualResetEventSlim();
            runner.Session.Game.Post(() =>
            {
                try
                {
                    result = call();
                }
                catch (Exception ex)
                {
                    error = ex;
                }
                done.Set();
            });
            return () =>
            {
                Assert.IsTrue(done.Wait(ScreenScenarioRunner.Timeout), "list result");
                if (error != null)
                {
                    throw new AssertFailedException("list failed: " + error);
                }
                return result;
            };
        }

        private static void Click(ScreenScenarioRunner runner, float x, float y, GuiButton button = GuiButton.Left)
        {
            runner.GUI.PointerPressed(x, y, button);
            runner.GUI.PointerReleased(x, y, button);
        }

        private static void Click(ScreenScenarioRunner runner, Form form, Control control)
        {
            Click(runner, form.ScreenLeft + control.Left + control.Width / 2f, form.ScreenTop + Form.TitleBarHeight + control.Top + control.Height / 2f);
        }

        [TestMethod]
        public void ListBox_DisabledItemCannotBeSelected_ScrollsWithKeys()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitForMessage();
                var result = RunOnGame(runner, () => runner.GUI.ListBox(Args(10, "通常", disableSecond: true)));
                var list = runner.WaitForList();
                Assert.IsInstanceOfType(list.Window, typeof(ListWindow));
                Assert.IsTrue(list.Window.lstItems.HasScrollBar);
                Assert.AreEqual("×項目02", list.Window.lstItems.Items[1].Trim());

                // 無効な項目は選べない
                var layout = runner.GUI.Renderer.LayoutList(list);
                var disabled = layout.ItemRect(1);
                Click(runner, disabled.MidX, disabled.MidY);
                Thread.Sleep(300);
                Assert.IsTrue(runner.GUI.IsWaitingList);

                // クリックした項目にカーソルは移る。下キーで表示範囲外まで移動するとスクロールする
                runner.WaitFor(s => s.List.Highlight == 1, "highlight on clicked item");
                for (var i = 0; i < 8; i++)
                {
                    runner.GUI.KeyPressed(ScreenKey.Down);
                }
                runner.WaitFor(s => s.List.Highlight == 9, "highlight moved");
                Assert.IsTrue(list.Window.lstItems.TopIndex > 0);
                runner.GUI.KeyPressed(ScreenKey.Enter);
                Assert.AreEqual(10, result());
            }
        }

        [TestMethod]
        public void MultiColumnListBox_SelectsItemInSecondColumn()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitForMessage();
                var result = RunOnGame(runner, () => runner.GUI.MultiColumnListBox(Args(40, "多段"), true));
                var list = runner.WaitForList();
                var box = (MultiColumnListWindow)list.Window;
                Assert.AreEqual(25, box.lstItems.Rows);
                VisualAssert.MatchesBaseline(runner.SnapshotPng(), "list-01-multicolumn");

                // 2 列目の先頭（26 番目）にカーソルを合わせると解説を表示し、クリックで選ぶ
                var rect = runner.GUI.Renderer.LayoutList(list).ItemRect(25);
                runner.GUI.PointerMoved(rect.MidX, rect.MidY);
                runner.WaitFor(s => box.labCaption.Text == "解説26", "comment");
                Click(runner, rect.MidX, rect.MidY);
                Assert.AreEqual(26, result());
                Assert.IsNull(runner.State.List);
            }
        }

        [TestMethod]
        public void MultiSelectListBox_SelectsAndFinishes()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitForMessage();
                var args = Args(5, "出撃選択");
                var result = RunOnGame(runner, () => runner.GUI.MultiSelectListBox(args, 2));
                var list = runner.WaitForList();
                var box = (MultiSelectListWindow)list.Window;
                Assert.IsFalse(box.cmdFinish.Enabled);
                Assert.AreEqual("0/2", box.lblNumber.Text);

                // 項目のクリックで選択を切り替える
                var layout = runner.GUI.Renderer.LayoutList(list);
                var first = layout.ItemRect(0);
                Click(runner, first.MidX, first.MidY);
                runner.WaitFor(s => box.lblNumber.Text == "1/2", "one selected");
                Assert.AreEqual("○項目01", box.lstItems.Items[0]);
                VisualAssert.MatchesBaseline(runner.SnapshotPng(), "list-02-multiselect");

                // 最後から選択して終了
                Click(runner, box, box.cmdSelectAll2);
                runner.WaitFor(s => box.lblNumber.Text == "2/2", "selected from last");
                Click(runner, box, box.cmdFinish);
                Assert.AreEqual(2, result());
                CollectionAssert.AreEqual(new[] { false, false, false, true, true }, args.Items.Select(x => x.ListItemFlag).ToArray());
            }
        }
    }
}
