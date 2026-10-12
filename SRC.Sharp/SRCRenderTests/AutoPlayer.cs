using SRCCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace SRCRender.Tests
{
    /// <summary>
    /// 画面の状態に応じて機械的に入力を与え、シナリオを進める。
    /// メッセージは送り、選択肢・メニューは先頭の有効な項目を選び、ダイアログは OK を押す。
    /// 表示したダイアログ（エラーメッセージなど）は記録する。
    /// </summary>
    public class AutoPlayer
    {
        private readonly ScreenScenarioRunner runner;

        public AutoPlayer(ScreenScenarioRunner runner)
        {
            this.runner = runner;
        }

        public IList<string> Dialogs { get; } = new List<string>();
        public IList<string> Messages { get; } = new List<string>();
        public int Steps { get; private set; }

        /// <summary>スナップショットの保存先（null なら保存しない）</summary>
        public string SnapshotDirectory { get; set; }

        /// <summary>選択肢の選び方。既定は先頭の有効な項目</summary>
        public Func<ListState, int> ChooseList { get; set; } = list =>
        {
            for (var i = 0; i < list.Items.Count; i++)
            {
                if (!list.Items[i].Disabled)
                {
                    return i;
                }
            }
            return -1;
        };

        /// <summary>
        /// 入力待ちが idleMillis 続いたら終了する。終了・最大ステップ数でも終了する。
        /// </summary>
        private void Click(Compat.Form form, Compat.Control control)
        {
            var x = form.ScreenLeft + control.Left + control.Width / 2f;
            var y = form.ScreenTop + Compat.Form.TitleBarHeight + control.Top + control.Height / 2f;
            runner.GUI.PointerPressed(x, y, GuiButton.Left);
            runner.GUI.PointerReleased(x, y, GuiButton.Left);
            Thread.Sleep(50);
        }

        public void Run(int maxSteps, int idleMillis = 10000)
        {
            var gui = runner.GUI;
            var idleSince = DateTime.UtcNow;
            var lastFrame = runner.FrameCount;
            while (Steps < maxSteps && !runner.IsFinished)
            {
                var state = runner.State;
                var acted = false;
                if (state.Dialog != null)
                {
                    var dialog = state.Dialog;
                    Dialogs.Add(dialog.Title + ": " + dialog.Text);
                    Snapshot("dialog");
                    gui.KeyPressed(ScreenKey.Enter);
                    acted = WaitChange(() => runner.State.Dialog != dialog);
                }
                else if (state.List != null && gui.IsWaitingList)
                {
                    var list = state.List;
                    Snapshot("list");
                    if (list.Window is Compat.MultiSelectListWindow multiSelect)
                    {
                        // 出撃選択は「先頭から選択」して「終了」する
                        Click(multiSelect, multiSelect.cmdSelectAll);
                        Click(multiSelect, multiSelect.cmdFinish);
                    }
                    else
                    {
                        var index = ChooseList(list);
                        if (index < 0)
                        {
                            gui.KeyPressed(ScreenKey.Escape);
                        }
                        else
                        {
                            list.Highlight = index;
                            gui.KeyPressed(ScreenKey.Enter);
                        }
                    }
                    acted = WaitChange(() => runner.State.List != list || !gui.IsWaitingList);
                }
                else if (state.Menu != null)
                {
                    var menu = state.Menu;
                    Snapshot("menu");
                    var box = gui.Renderer.LayoutMenu(menu);
                    gui.PointerPressed(box.Left + 10, box.Top + 12, GuiButton.Left);
                    gui.PointerReleased(box.Left + 10, box.Top + 12, GuiButton.Left);
                    acted = WaitChange(() => runner.State.Menu != menu);
                }
                else if (state.Message.Visible && state.Message.Waiting)
                {
                    var text = string.Join("/", state.Message.Lines);
                    if (Messages.Count == 0 || Messages[Messages.Count - 1] != text)
                    {
                        Messages.Add(text);
                    }
                    Snapshot("message");
                    gui.KeyPressed(ScreenKey.Enter);
                    acted = WaitChange(() => !runner.State.Message.Waiting);
                }
                else if (gui.IsWaitingClick)
                {
                    gui.KeyPressed(ScreenKey.Enter);
                    acted = true;
                    Thread.Sleep(30);
                }

                if (runner.FrameCount != lastFrame)
                {
                    // 画面が更新されている間は待ちとみなさない
                    lastFrame = runner.FrameCount;
                    idleSince = DateTime.UtcNow;
                }
                if (acted)
                {
                    Steps++;
                    idleSince = DateTime.UtcNow;
                }
                else
                {
                    if ((DateTime.UtcNow - idleSince).TotalMilliseconds > idleMillis)
                    {
                        Snapshot("idle");
                        return;
                    }
                    Thread.Sleep(20);
                }
            }
        }

        private static bool WaitChange(Func<bool> changed)
        {
            var deadline = DateTime.UtcNow.AddSeconds(5);
            while (!changed())
            {
                if (DateTime.UtcNow > deadline)
                {
                    return false;
                }
                Thread.Sleep(5);
            }
            return true;
        }

        private int snapshotNo;

        private void Snapshot(string kind)
        {
            if (SnapshotDirectory == null)
            {
                return;
            }
            Directory.CreateDirectory(SnapshotDirectory);
            var path = Path.Combine(SnapshotDirectory, $"{++snapshotNo:0000}-{kind}.png");
            File.WriteAllBytes(path, runner.SnapshotPng());
        }
    }
}
