using SRCCore;
using System;
using System.Linq;
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

        /// <summary>待ちの状態でないまま、この時間操作が無ければボタンを押す（ミリ秒）</summary>
        public int StallClickMillis { get; set; } = 5000;

        /// <summary>
        /// そのときに押す位置。順に使う。スクリプトが描いたメニュー（マウスの位置で選ぶもの）を進めるため、マップ上の複数の場所を含める
        /// </summary>
        public IList<(float x, float y)> StallClickPositions { get; set; } = new List<(float x, float y)>
        {
            (240, 240), (130, 130), (350, 230), (240, 380), (240, 472), (600, 300),
        };

        // リスト以外の操作をするまでに各リストを選んだ回数（リストが交互に出る循環も検出するため）
        private readonly Dictionary<string, int> listVisits = new Dictionary<string, int>();

        /// <summary>ボタンを押した回数</summary>
        public int Stalls { get; private set; }

        /// <summary>進行状況の通知（監視用）</summary>
        public Action<string> Trace { get; set; }

        /// <summary>スナップショットの保存先（null なら保存しない）</summary>
        public string SnapshotDirectory { get; set; }

        /// <summary>
        /// 選択肢の規則。見出し（またはリスト上部の説明）に Caption を含むリストでは、Item を含む有効な項目を選ぶ。
        /// インターミッションで「次のステージへ」を選ぶ、シナリオ独自の選択肢で特定の答えを選ぶ、などに使う。先に追加したものを優先する
        /// </summary>
        public IList<(string Caption, string Item)> ListRules { get; } = new List<(string Caption, string Item)>();

        /// <summary>リストで項目を選んだときの通知（見出し, 選んだ項目）</summary>
        public Action<string, string> ListChosen { get; set; }

        private int ChooseByRule(ListState list)
        {
            foreach (var (caption, item) in ListRules)
            {
                if (!(list.Caption ?? "").Contains(caption) && !(list.Info ?? "").Contains(caption))
                {
                    continue;
                }
                for (var i = 0; i < list.Items.Count; i++)
                {
                    if (!list.Items[i].Disabled && (list.Items[i].Text ?? "").Contains(item))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

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

        public void Run(int maxSteps, int idleMillis = 10000) => Run(maxSteps, idleMillis, null);

        /// <summary>
        /// stopWhen が真になったら（例: マップで操作を受け付ける状態）その場で戻る。真で戻った場合は true。
        /// </summary>
        public bool Run(int maxSteps, int idleMillis, Func<bool> stopWhen)
        {
            var gui = runner.GUI;
            var idleSince = DateTime.UtcNow;
            var lastFrame = runner.FrameCount;
            var lastActed = DateTime.UtcNow;
            while (Steps < maxSteps && !runner.IsFinished)
            {
                if (stopWhen != null && stopWhen())
                {
                    return true;
                }
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
                        // 同じリストが繰り返し出る場合（選んだ結果で元に戻る、並べ替えのリストと交互に出るなど）は、次の有効な項目を選ぶ。
                        // 有効な項目をすべて選んでも抜けられなければキャンセルする
                        var signature = list.Caption + "\n" + list.Info + "\n" + string.Join("\n", list.Items.Select(x => x.Text));
                        listVisits.TryGetValue(signature, out var visits);
                        listVisits[signature] = visits + 1;
                        var index = ChooseByRule(list);
                        if (index >= 0)
                        {
                            // 規則で選ぶ場合は繰り返しても同じ項目を選ぶ
                        }
                        else if ((index = ChooseList(list)) >= 0 && visits > 0)
                        {
                            var enabled = Enumerable.Range(0, list.Items.Count).Where(i => !list.Items[i].Disabled).ToList();
                            index = visits >= enabled.Count + 1 ? -1 : enabled[(enabled.IndexOf(index) + visits) % enabled.Count];
                        }
                        if (index < 0)
                        {
                            gui.KeyPressed(ScreenKey.Escape);
                        }
                        else
                        {
                            ListChosen?.Invoke(list.Info ?? list.Caption, list.Items[index].Text);
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
                else if ((DateTime.UtcNow - lastActed).TotalMilliseconds > StallClickMillis)
                {
                    // 待ちの状態でないまま進まない（KeyState でボタンが押されるのを待つループなど）。
                    // 人の操作と同じく、ボタンを押してしばらく保ってから離す
                    var (cx, cy) = StallClickPositions[Stalls % StallClickPositions.Count];
                    Trace?.Invoke($"stall: hold left button at ({cx}, {cy})");
                    Snapshot("stall");
                    gui.PointerMoved(cx, cy);
                    gui.PointerPressed(cx, cy, GuiButton.Left);
                    Thread.Sleep(300);
                    gui.PointerReleased(cx, cy, GuiButton.Left);
                    Stalls++;
                    acted = true;
                }

                if (runner.FrameCount != lastFrame)
                {
                    // 画面が更新されている間は待ちとみなさない
                    lastFrame = runner.FrameCount;
                    idleSince = DateTime.UtcNow;
                }
                if (acted && !(state.List != null && state.Dialog == null))
                {
                    listVisits.Clear();
                }
                if (acted)
                {
                    Steps++;
                    idleSince = DateTime.UtcNow;
                    lastActed = DateTime.UtcNow;
                    Trace?.Invoke($"step {Steps}");
                }
                else
                {
                    if ((DateTime.UtcNow - idleSince).TotalMilliseconds > idleMillis)
                    {
                        Snapshot("idle");
                        return false;
                    }
                    Thread.Sleep(20);
                }
            }
            return false;
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

        public void Snapshot(string kind)
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
