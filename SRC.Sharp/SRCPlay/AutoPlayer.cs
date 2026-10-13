using SRCCore;
using SRCRender;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace SRCPlay
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

        // 各リストを選んだ回数。リストが交互に出る循環や、リスト → 説明のメッセージ → リストの循環も検出する。
        // マップで操作を受け付ける状態になったら数え直す（ResetLoopDetection）
        private readonly Dictionary<string, int> listVisits = new Dictionary<string, int>();

        /// <summary>循環の検出を数え直す（マップで操作を受け付ける状態になったときなど）</summary>
        public void ResetLoopDetection()
        {
            listVisits.Clear();
        }

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

        /// <summary>
        /// 一度だけ使う選択肢の規則。ListRules より優先し、使ったら取り除く。
        /// インターミッションのように、同じリストで順に別の項目を選ぶ（「ショップ」に入ってから「次のステージへ」）場合に使う
        /// </summary>
        public IList<(string Caption, string Item)> OnceRules { get; } = new List<(string Caption, string Item)>();

        /// <summary>
        /// 順に選ぶ規則。見出しの合うリスト（またはクリック領域）が出るたびに、Items を先頭から順に選び、最後まで選んだら先頭に戻る。
        /// 今の位置の項目が無ければ、次にある項目を選ぶ。インターミッションのように同じメニューへ何度も戻る場面で、毎回同じ手順を踏むのに使う
        /// </summary>
        public sealed class SequenceRule
        {
            public string Caption;
            public IList<string> Items;
            public int Index;

            /// <summary>exists(項目) が真になる項目を今の位置から探し、見つかれば位置を進めてその項目を返す</summary>
            public string Next(Func<string, bool> exists)
            {
                for (var k = 0; k < Items.Count; k++)
                {
                    var i = (Index + k) % Items.Count;
                    if (exists(Items[i]))
                    {
                        Index = (i + 1) % Items.Count;
                        return Items[i];
                    }
                }
                return null;
            }
        }

        public IList<SequenceRule> SequenceRules { get; } = new List<SequenceRule>();

        /// <summary>リストで項目を選んだときの通知（見出し, 選んだ項目）</summary>
        public Action<string, string> ListChosen { get; set; }

        private static bool CaptionMatches(ListState list, string caption)
            => (list.Caption ?? "").Contains(caption) || (list.Info ?? "").Contains(caption);

        private static int FindItem(ListState list, string item)
        {
            for (var i = 0; i < list.Items.Count; i++)
            {
                if (!list.Items[i].Disabled && (list.Items[i].Text ?? "").Contains(item))
                {
                    return i;
                }
            }
            return -1;
        }

        private int ChooseByRule(ListState list)
        {
            for (var r = 0; r < OnceRules.Count; r++)
            {
                var (caption, item) = OnceRules[r];
                if (CaptionMatches(list, caption))
                {
                    var found = FindItem(list, item);
                    if (found >= 0)
                    {
                        OnceRules.RemoveAt(r);
                        return found;
                    }
                }
            }
            foreach (var rule in SequenceRules.Where(x => CaptionMatches(list, x.Caption)))
            {
                var item = rule.Next(x => FindItem(list, x) >= 0);
                if (item != null)
                {
                    return FindItem(list, item);
                }
            }
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
        private void Click(SRCRender.Compat.Form form, SRCRender.Compat.Control control)
        {
            var x = form.ScreenLeft + control.Left + control.Width / 2f;
            var y = form.ScreenTop + SRCRender.Compat.Form.TitleBarHeight + control.Top + control.Height / 2f;
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
                    if (list.Window is SRCRender.Compat.MultiSelectListWindow multiSelect)
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
                    var waitCount = state.Message.WaitCount;
                    gui.KeyPressed(ScreenKey.Enter);
                    acted = WaitChange(() => !runner.State.Message.Waiting || runner.State.Message.WaitCount != waitCount);
                }
                else if (gui.IsWaitingClick)
                {
                    // ホットポイントがあれば、そのどれかをクリックする（Wait Click で「選択」にホットポイントの名前が入る）
                    var hotPoint = ChooseHotPoint();
                    if (hotPoint.HasValue)
                    {
                        var hp = hotPoint.Value;
                        var (hx, hy) = (hp.Left + hp.Width / 2f, hp.Top + hp.Height / 2f);
                        ListChosen?.Invoke(HotPointCaption, hp.Name);
                        gui.PointerMoved(hx, hy);
                        gui.PointerPressed(hx, hy, GuiButton.Left);
                        gui.PointerReleased(hx, hy, GuiButton.Left);
                    }
                    else
                    {
                        gui.KeyPressed(ScreenKey.Enter);
                    }
                    acted = true;
                    Thread.Sleep(30);
                }
                else if ((DateTime.UtcNow - lastActed).TotalMilliseconds > StallClickMillis && ChooseMousePoint() is MousePoint point)
                {
                    // スクリプトが描いたクリック領域（MousePoint.eve の MP_ClickPoint）のうち、規則に合うものを押す
                    var (mx, my) = ((point.X1 + point.X2) / 2f, (point.Y1 + point.Y2) / 2f);
                    ListChosen?.Invoke(MousePointCaption, point.ID);
                    Snapshot("mousepoint");
                    gui.PointerMoved(mx, my);
                    gui.PointerPressed(mx, my, GuiButton.Left);
                    Thread.Sleep(300);
                    gui.PointerReleased(mx, my, GuiButton.Left);
                    acted = true;
                }
                else if ((DateTime.UtcNow - lastActed).TotalMilliseconds > StallClickMillis)
                {
                    // 待ちの状態でないまま進まない（KeyState でボタンが押されるのを待つループなど）。
                    // 人の操作と同じく、ボタンを押してしばらく保ってから離す
                    var (cx, cy) = StallClickPositions[Stalls % StallClickPositions.Count];
                    var pointIds = string.Join(" | ", MousePoints().Select(x => x.ID));
                    Trace?.Invoke($"stall: hold left button at ({cx}, {cy})" + (pointIds.Length > 0 ? " points: " + pointIds : ""));
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

        /// <summary>
        /// クリック待ちのホットポイントを選ぶ。選択肢の規則（ListRules）の項目に名前か説明が合うものを優先し、
        /// 無ければ同じホットポイントの組が続く間は順に別のものを選ぶ。ホットポイントが無ければ null。
        /// </summary>
        private string lastHotPointSignature;
        private int hotPointVisits;

        private SRCCore.Events.HotPoint? ChooseHotPoint()
        {
            List<SRCCore.Events.HotPoint> points;
            try
            {
                points = (runner.Session.SRC.Event.HotPointList ?? new List<SRCCore.Events.HotPoint>())
                    .Where(x => x.Width > 0 && x.Height > 0)
                    .ToList();
            }
            catch (InvalidOperationException)
            {
                // ゲームスレッドが更新している最中
                return null;
            }
            if (points.Count == 0)
            {
                return null;
            }
            // 見出し「ホットポイント」の一度だけの規則・順に選ぶ規則は、名前で選ぶ（末尾が * なら前方一致）
            bool NameMatches(SRCCore.Events.HotPoint x, string item)
                => item.EndsWith("*") ? (x.Name ?? "").StartsWith(item.Substring(0, item.Length - 1)) : x.Name == item;
            for (var r = 0; r < OnceRules.Count; r++)
            {
                var (caption, item) = OnceRules[r];
                var once = caption == HotPointCaption ? points.FindIndex(x => NameMatches(x, item)) : -1;
                if (once >= 0)
                {
                    OnceRules.RemoveAt(r);
                    return points[once];
                }
            }
            foreach (var rule in SequenceRules.Where(x => x.Caption == HotPointCaption))
            {
                var item = rule.Next(x => points.Any(p => NameMatches(p, x)));
                if (item != null)
                {
                    return points.First(p => NameMatches(p, item));
                }
            }
            foreach (var (_, item) in ListRules)
            {
                var matched = points.FindIndex(x => (x.Name ?? "").Contains(item) || (x.Caption ?? "").Contains(item));
                if (matched >= 0)
                {
                    return points[matched];
                }
            }
            // 同じホットポイントの組が続く間は順に別のものを選ぶ
            var signature = string.Join("\n", points.Select(x => x.Name));
            hotPointVisits = signature == lastHotPointSignature ? hotPointVisits + 1 : 0;
            lastHotPointSignature = signature;
            return points[hotPointVisits % points.Count];
        }

        /// <summary>一度だけの規則・順に選ぶ規則の見出しにこれを指定すると、ホットポイントを名前で選ぶ</summary>
        public const string HotPointCaption = "ホットポイント";

        /// <summary>規則の見出しにこれを指定すると、スクリプトが描いたクリック領域（MousePoint.eve）を ID で選ぶ</summary>
        public const string MousePointCaption = "クリック";

        public sealed class MousePoint
        {
            public string ID;
            public int X1;
            public int Y1;
            public int X2;
            public int Y2;
        }

        /// <summary>
        /// MousePoint.eve（ロボダンなど）が登録したクリック領域。変数 MP_ClickPoint[n]（呼び出し元で Local にしていればローカル変数）に
        /// 「(ID) キー x1 y1 x2 y2 (イベント)」のリストで入っている。ゲームスレッドで更新中なら空。
        /// </summary>
        public IList<MousePoint> MousePoints()
        {
            var result = new List<MousePoint>();
            try
            {
                var ev = runner.Session.SRC.Event;
                var vars = ev.LocalVariableList.ContainsKey("MP_ClickPoint[1]") ? ev.LocalVariableList : ev.GlobalVariableList;
                for (var i = 1; vars.ContainsKey($"MP_ClickPoint[{i}]"); i++)
                {
                    var data = vars[$"MP_ClickPoint[{i}]"].StringValue ?? "";
                    var id = SRCCore.Lib.GeneralLib.ListIndex(data, 1).Trim('(', ')');
                    if (int.TryParse(SRCCore.Lib.GeneralLib.ListIndex(data, 3), out var x1)
                        && int.TryParse(SRCCore.Lib.GeneralLib.ListIndex(data, 4), out var y1)
                        && int.TryParse(SRCCore.Lib.GeneralLib.ListIndex(data, 5), out var x2)
                        && int.TryParse(SRCCore.Lib.GeneralLib.ListIndex(data, 6), out var y2))
                    {
                        result.Add(new MousePoint { ID = id, X1 = x1, Y1 = y1, X2 = x2, Y2 = y2 });
                    }
                }
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException || ex is NullReferenceException)
            {
                // ゲームスレッドが更新している最中
                return new List<MousePoint>();
            }
            return result;
        }

        /// <summary>クリック領域の ID が規則の項目に合うか。項目の末尾が * なら前方一致</summary>
        private static bool PointMatches(MousePoint point, string item)
            => item.EndsWith("*") ? point.ID.StartsWith(item.Substring(0, item.Length - 1)) : point.ID == item;

        /// <summary>見出しが MousePointCaption の規則（一度だけのものを優先）に ID が合うクリック領域。無ければ null</summary>
        private MousePoint ChooseMousePoint()
        {
            var points = MousePoints();
            if (points.Count == 0)
            {
                return null;
            }
            for (var r = 0; r < OnceRules.Count; r++)
            {
                var (caption, item) = OnceRules[r];
                var found = caption == MousePointCaption ? points.FirstOrDefault(x => PointMatches(x, item)) : null;
                if (found != null)
                {
                    OnceRules.RemoveAt(r);
                    return found;
                }
            }
            foreach (var rule in SequenceRules.Where(x => x.Caption == MousePointCaption))
            {
                var item = rule.Next(x => points.Any(p => PointMatches(p, x)));
                if (item != null)
                {
                    return points.First(p => PointMatches(p, item));
                }
            }
            foreach (var (caption, item) in ListRules)
            {
                var found = caption == MousePointCaption ? points.FirstOrDefault(x => PointMatches(x, item)) : null;
                if (found != null)
                {
                    return found;
                }
            }
            return null;
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
