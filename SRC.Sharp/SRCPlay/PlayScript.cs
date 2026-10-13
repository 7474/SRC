using SRCCore;
using SRCRender;
using SRCCore.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace SRCPlay
{
    /// <summary>
    /// テキスト形式の操作スクリプトでシナリオをプレイする（docs/gui/e2e-testing.md の入力スクリプト）。
    /// 1 行 1 コマンド。# 以降はコメント。
    ///
    ///   auto [秒]              メッセージ・ダイアログ・リスト等を自動で進め、マップで操作を受け付ける状態になるまで待つ
    ///   wait ready [秒]        マップで操作を受け付ける状態になるまで待つ（自動では進めない）
    ///   wait message [文字列]  メッセージ（指定があればその文字列を含むもの）が出るまで待つ
    ///   wait list [文字列]     リスト（指定があれば見出しにその文字列を含むもの）が出るまで待つ
    ///   wait menu|dialog|click 各状態になるまで待つ
    ///   click X Y [right]      画面座標をクリックする
    ///   cell X Y [right]       マップのマス（必要ならそのマスを画面の中央に寄せる）をクリックする
    ///   menu 項目名            ポップアップメニューの項目を選ぶ
    ///   list 文字列|#番号      リストの項目を選ぶ（番号は 1 始まり）
    ///   dialog ボタン名        ダイアログのボタンを押す
    ///   key Enter|Escape|...   キーを押す
    ///   set 変数 値            SRC の変数に値を設定する（検証の前提を整える。例: set 設定[難易度] ベリーイージー）
    ///   print 式              SRC の式の値を記録する（進行状況の確認用。例: print ダンジョン階数）
    ///   policy smart|advance|hold  phase の方針。smart（既定）は SRC の CPU と同じ見積もり（COM.SelectWeapon）で目標と武器を選び、
    ///                          移動先は攻撃の見込みと敵から受ける危険で決め、HP が少なければ下がる。advance は攻撃できなければ最も近い敵へ近づく、
    ///                          hold は移動せず届く敵だけ攻撃する
    ///   autodefense on|off     敵の攻撃への反撃・防御・回避を SRC に任せる（設定「自動反撃」。CPU と同じ判断で選ぶ）
    ///   stall 秒               auto・turns で、待ちの状態でないまま進まないときにボタンを押すまでの時間（既定 5 秒）
    ///   choose 見出し 項目     auto・turns で、見出し（または説明）にその文字列を含むリストが出たら、その文字列を含む項目を選ぶ
    ///                          （例: choose コマンド 次のステージへ … インターミッションで次のステージへ進む）
    ///   snapshot 名前          画面を保存する
    ///   phase                  味方フェイズを簡単な方針で自動で行い、ターンを終える（ScriptedPhase）
    ///   turns 回数             「auto → phase」を繰り返す
    ///   expect finished        シナリオが終わっていることを確かめる
    ///   expect message 文字列  その文字列を含むメッセージが表示されたことを確かめる
    ///   expect log 文字列      操作の記録（phase の「攻撃: 敵の名前」など）にその文字列があることを確かめる
    ///   expect error none      エラーが起きていないことを確かめる
    ///   expect eval 式 演算子 式  SRC の式を評価して比べる（例: expect eval HP(標的) &lt; MaxHP(標的)）
    /// </summary>
    public sealed class PlayScript
    {
        private readonly ScreenScenarioRunner runner;
        private readonly AutoPlayer auto;
        private int snapshotNo;

        public PlayScript(ScreenScenarioRunner runner)
        {
            this.runner = runner;
            auto = new AutoPlayer(runner);
            auto.ListChosen = (caption, item) => Record($"選択: {caption?.Trim()} → {item?.Trim()}");
            // 止まって押した位置（スクリプトが描いたクリック領域があればその ID）を記録する
            auto.Trace = m =>
            {
                if (m.StartsWith("stall"))
                {
                    Record($"{DateTime.Now:HH:mm:ss} {m}");
                }
            };
        }

        /// <summary>スナップショットの保存先（null なら保存しない）</summary>
        public string SnapshotDirectory
        {
            get => auto.SnapshotDirectory;
            set => auto.SnapshotDirectory = value;
        }

        /// <summary>進行状況の通知</summary>
        public Action<string> Trace { get; set; }

        /// <summary>これまでの進行状況（expect log で使う）</summary>
        public List<string> TraceLog { get; } = new List<string>();

        /// <summary>これまでに表示されたメッセージ（自動で進めたもの）</summary>
        public IList<string> Messages => auto.Messages;

        internal void Record(string line)
        {
            TraceLog.Add(line);
            Trace?.Invoke(line);
        }

        /// <summary>phase の方針（smart・advance・hold）</summary>
        public string Policy { get; set; } = "smart";

        /// <summary>1 つの待ちの既定の上限</summary>
        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(60);

        public ScreenScenarioRunner Runner => runner;
        public SRC Src => runner.Session.SRC;
        private ScreenGUI GUI => runner.GUI;

        private void Log(string message)
        {
            Record($"{DateTime.Now:HH:mm:ss} SCRIPT {message}");
        }

        public void Run(IEnumerable<string> lines)
        {
            var lineNo = 0;
            foreach (var raw in lines)
            {
                lineNo++;
                var line = raw;
                var comment = line.IndexOf('#');
                if (comment >= 0)
                {
                    line = line.Substring(0, comment);
                }
                var args = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (args.Length == 0)
                {
                    continue;
                }
                if (runner.IsFinished && args[0] != "expect" && args[0] != "snapshot")
                {
                    Log($"line {lineNo}: シナリオが終わっているため以降を実行しない");
                    return;
                }
                Log($"line {lineNo}: {line.Trim()}");
                try
                {
                    Execute(args);
                }
                catch (Exception ex) when (!(ex is PlayScriptException))
                {
                    throw new PlayScriptException($"line {lineNo}: {line.Trim()}: {ex.Message}", ex);
                }
            }
        }

        private TimeSpan Seconds(string[] args, int index, TimeSpan defaultValue)
        {
            return args.Length > index ? TimeSpan.FromSeconds(double.Parse(args[index])) : defaultValue;
        }

        private static string Rest(string[] args, int index)
        {
            return args.Length > index ? string.Join(" ", args.Skip(index)) : null;
        }

        private void Execute(string[] args)
        {
            switch (args[0])
            {
                case "auto":
                    AutoUntilReady(Seconds(args, 1, TimeSpan.FromMinutes(5)));
                    break;
                case "wait":
                    Wait(args);
                    break;
                case "click":
                    Click(float.Parse(args[1]), float.Parse(args[2]), args.Length > 3 && args[3] == "right" ? GuiButton.Right : GuiButton.Left);
                    break;
                case "cell":
                    ClickCell(int.Parse(args[1]), int.Parse(args[2]), args.Length > 3 && args[3] == "right" ? GuiButton.Right : GuiButton.Left);
                    break;
                case "menu":
                    if (!SelectMenu(Rest(args, 1)))
                    {
                        throw new PlayScriptException($"メニューに「{Rest(args, 1)}」がありません: {string.Join(",", runner.State.Menu?.Items ?? new List<string>())}");
                    }
                    break;
                case "list":
                    SelectList(Rest(args, 1));
                    break;
                case "set":
                    {
                        var name = args[1];
                        var value = Rest(args, 2) ?? "";
                        OnGame(() =>
                        {
                            if (double.TryParse(value, out var number))
                            {
                                Src.Expression.SetVariableAsDouble(name, number);
                            }
                            else
                            {
                                Src.Expression.SetVariableAsString(name, value);
                            }
                            return true;
                        });
                        Record($"{DateTime.Now:HH:mm:ss} SET {name} = {value}");
                    }
                    break;
                case "print":
                    {
                        var expr = Rest(args, 1);
                        var value = OnGame(() => Src.Expression.GetValueAsString(expr));
                        Record($"{DateTime.Now:HH:mm:ss} PRINT {expr} = {value}");
                    }
                    break;
                case "policy":
                    if (!new[] { "smart", "advance", "hold" }.Contains(args[1]))
                    {
                        throw new PlayScriptException("policy smart|advance|hold");
                    }
                    Policy = args[1];
                    break;
                case "autodefense":
                    {
                        var on = args.Length > 1 && args[1] == "on";
                        OnGame(() => Src.SystemConfig.AutoDefense = on);
                        Record($"{DateTime.Now:HH:mm:ss} AUTODEFENSE {(on ? "on" : "off")}");
                    }
                    break;
                case "stall":
                    auto.StallClickMillis = (int)(double.Parse(args[1]) * 1000);
                    break;
                case "choose":
                    if (args.Length < 3)
                    {
                        throw new PlayScriptException("choose 見出し 項目");
                    }
                    auto.ListRules.Add((args[1], Rest(args, 2)));
                    break;
                case "choose1":
                    // 一度だけ使う規則（順に使う）。見出しに「クリック」を指定すると、スクリプトが描いたクリック領域を ID で選ぶ
                    if (args.Length < 3)
                    {
                        throw new PlayScriptException("choose1 見出し 項目");
                    }
                    auto.OnceRules.Add((args[1], Rest(args, 2)));
                    break;
                case "chooseseq":
                    // 見出しの合うリスト（クリック領域）が出るたびに、| で区切った項目を順に選ぶ
                    if (args.Length < 3)
                    {
                        throw new PlayScriptException("chooseseq 見出し 項目1|項目2|...");
                    }
                    auto.SequenceRules.Add(new AutoPlayer.SequenceRule { Caption = args[1], Items = Rest(args, 2).Split('|').Select(x => x.Trim()).ToList() });
                    break;
                case "points":
                    // スクリプトが描いたクリック領域の一覧を記録する（規則を書くための調査用）
                    Record($"{DateTime.Now:HH:mm:ss} POINTS " + string.Join(" ", auto.MousePoints().Select(p => $"{p.ID}({p.X1},{p.Y1})-({p.X2},{p.Y2})")));
                    break;
                case "dialog":
                    PressDialog(Rest(args, 1));
                    break;
                case "key":
                    GUI.KeyPressed((ScreenKey)Enum.Parse(typeof(ScreenKey), args[1]));
                    runner.Sync();
                    break;
                case "snapshot":
                    Snapshot(args.Length > 1 ? args[1] : "snapshot");
                    break;
                case "phase":
                    new ScriptedPhase(this).Run();
                    break;
                case "turns":
                    var turns = int.Parse(args[1]);
                    for (var i = 0; i < turns && !runner.IsFinished; i++)
                    {
                        if (!AutoUntilReady(TimeSpan.FromMinutes(5)))
                        {
                            return;
                        }
                        new ScriptedPhase(this).Run();
                    }
                    break;
                case "expect":
                    Expect(args);
                    break;
                default:
                    throw new PlayScriptException($"不明なコマンド: {args[0]}");
            }
        }

        private static readonly string[] Operators = { "<=", ">=", "<>", "<", ">", "=" };

        /// <summary>
        /// expect finished | expect message 文字列 | expect log 文字列 | expect error none | expect eval 式 演算子 式
        /// </summary>
        private void Expect(string[] args)
        {
            var text = Rest(args, 2);
            switch (args[1])
            {
                case "finished":
                    if (!runner.IsFinished)
                    {
                        throw new PlayScriptException("シナリオが終わっていません");
                    }
                    break;
                case "message":
                    if (!Messages.Any(x => x.Contains(text)))
                    {
                        throw new PlayScriptException($"メッセージ「{text}」が表示されていません: {string.Join(" / ", Messages)}");
                    }
                    break;
                case "log":
                    if (!TraceLog.Any(x => x.Contains(text)))
                    {
                        throw new PlayScriptException($"操作の記録に「{text}」がありません");
                    }
                    break;
                case "error":
                    if (runner.Error != null && !(runner.Error is SRCCore.TestLib.HeadlessTerminatedException))
                    {
                        throw new PlayScriptException("エラーが発生しています: " + runner.Error);
                    }
                    break;
                case "eval":
                    {
                        var tokens = args.Skip(2).ToList();
                        var opIndex = tokens.FindIndex(x => Operators.Contains(x));
                        if (opIndex <= 0 || opIndex == tokens.Count - 1)
                        {
                            throw new PlayScriptException("expect eval 式 演算子 式 の形で書いてください");
                        }
                        var op = tokens[opIndex];
                        var lhs = string.Join(" ", tokens.Take(opIndex));
                        var rhs = string.Join(" ", tokens.Skip(opIndex + 1));
                        var (l, r) = OnGame(() => (Src.Expression.GetValueAsDouble(lhs), Src.Expression.GetValueAsDouble(rhs)));
                        bool ok;
                        switch (op)
                        {
                            case "<": ok = l < r; break;
                            case "<=": ok = l <= r; break;
                            case ">": ok = l > r; break;
                            case ">=": ok = l >= r; break;
                            case "<>": ok = l != r; break;
                            default: ok = l == r; break;
                        }
                        if (!ok)
                        {
                            throw new PlayScriptException($"{lhs}（{l}） {op} {rhs}（{r}） が成り立ちません");
                        }
                    }
                    break;
                default:
                    throw new PlayScriptException($"不明な expect: {args[1]}");
            }
        }

        #region 状態

        /// <summary>マップで操作（ユニットの選択・マップコマンド）を受け付ける状態か</summary>
        public bool IsReady
        {
            get
            {
                var state = runner.State;
                return !runner.IsFinished
                    && !GUI.IsGUILocked
                    && state.Menu == null && state.List == null && state.Dialog == null && state.Config == null
                    && !(state.Message.Visible && state.Message.Waiting)
                    && !GUI.IsWaitingClick
                    // イベントの途中（インターミッションのスクリプトが描いた画面で KeyState を待つループなど）は操作を受け付ける状態ではない
                    && !runner.Session.Game.IsBusy
                    && Src.Commands.CommandState == "ユニット選択";
            }
        }

        public string CommandState => Src.Commands.CommandState;

        /// <summary>ゲームスレッドで値を求める（ゲームの状態を読むため）</summary>
        public T OnGame<T>(Func<T> func)
        {
            var result = default(T);
            Exception error = null;
            using (var done = new ManualResetEventSlim())
            {
                runner.Session.Game.Post(() =>
                {
                    try
                    {
                        result = func();
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }
                    done.Set();
                });
                // デバッガが止めている間は待ち時間に数えない
                var deadline = DateTime.UtcNow + DefaultTimeout;
                while (!done.Wait(50))
                {
                    if (runner.IsSuspended)
                    {
                        deadline += runner.WaitWhileSuspended();
                    }
                    if (DateTime.UtcNow > deadline)
                    {
                        throw new TimeoutException("ゲームスレッドが応答しません");
                    }
                }
            }
            if (error != null)
            {
                throw new PlayScriptException(error.Message, error);
            }
            return result;
        }

        public bool WaitUntil(Func<bool> condition, TimeSpan timeout)
        {
            var deadline = DateTime.UtcNow + timeout;
            while (!condition())
            {
                deadline += runner.WaitWhileSuspended();
                if (runner.IsFinished || DateTime.UtcNow > deadline)
                {
                    return condition();
                }
                Thread.Sleep(20);
            }
            return true;
        }

        /// <summary>マップで操作を受け付ける状態になるまで、メッセージ等を自動で進める。</summary>
        public bool AutoUntilReady(TimeSpan timeout)
        {
            var deadline = DateTime.UtcNow + timeout;
            while (DateTime.UtcNow < deadline && !runner.IsFinished)
            {
                deadline += runner.WaitWhileSuspended();
                // 状態が落ち着いてから判定する（入力の処理中の一瞬を拾わない）
                if (IsReady)
                {
                    Thread.Sleep(150);
                    if (IsReady)
                    {
                        auto.ResetLoopDetection();
                        return true;
                    }
                }
                // 停滞時にボタンを押す（StallClickMillis）より長く待つ。
                // 停滞時のクリックで画面が更新され続けると戻らないので、期限もここで判定する
                var suspendedBefore = runner.SuspendedTime;
                auto.Run(int.MaxValue, 60000, () => IsReady || DateTime.UtcNow >= deadline + (runner.SuspendedTime - suspendedBefore));
                deadline += runner.SuspendedTime - suspendedBefore;
            }
            if (!IsReady && !runner.IsFinished)
            {
                Record($"{DateTime.Now:HH:mm:ss} NOT READY {NotReadyReason()}");
            }
            return IsReady;
        }

        /// <summary>マップで操作を受け付ける状態でない理由（調査用）</summary>
        public string NotReadyReason()
        {
            var state = runner.State;
            var reasons = new List<string>();
            if (GUI.IsGUILocked) reasons.Add("GUI locked");
            if (state.Menu != null) reasons.Add("menu");
            if (state.List != null) reasons.Add("list " + state.List.Caption);
            if (state.Dialog != null) reasons.Add("dialog " + state.Dialog.Title);
            if (state.Config != null) reasons.Add("config");
            if (state.Message.Visible && state.Message.Waiting) reasons.Add("message");
            if (GUI.IsWaitingClick) reasons.Add("waiting click");
            if (runner.Session.Game.IsBusy) reasons.Add("event busy");
            reasons.Add("command state " + Src.Commands.CommandState);
            return string.Join(", ", reasons);
        }

        private void Wait(string[] args)
        {
            var what = args[1];
            var text = Rest(args, 2);
            bool ok;
            switch (what)
            {
                case "ready":
                    ok = WaitUntil(() => IsReady, Seconds(args, 2, DefaultTimeout));
                    break;
                case "message":
                    ok = WaitUntil(() => runner.State.Message.Visible && runner.State.Message.Waiting
                        && (text == null || string.Join("", runner.State.Message.Lines).Contains(text)), DefaultTimeout);
                    break;
                case "list":
                    ok = WaitUntil(() => runner.State.List != null && GUI.IsWaitingList
                        && (text == null || (runner.State.List.Caption ?? "").Contains(text) || (runner.State.List.Info ?? "").Contains(text)), DefaultTimeout);
                    break;
                case "menu":
                    ok = WaitUntil(() => runner.State.Menu != null, DefaultTimeout);
                    break;
                case "dialog":
                    ok = WaitUntil(() => runner.State.Dialog != null, DefaultTimeout);
                    break;
                case "click":
                    ok = WaitUntil(() => GUI.IsWaitingClick, DefaultTimeout);
                    break;
                default:
                    throw new PlayScriptException($"不明な待ち: {what}");
            }
            if (!ok)
            {
                Snapshot("timeout");
                throw new PlayScriptException($"wait {what} がタイムアウトしました");
            }
        }

        #endregion

        #region 操作

        public void Snapshot(string name)
        {
            if (SnapshotDirectory == null)
            {
                return;
            }
            Directory.CreateDirectory(SnapshotDirectory);
            File.WriteAllBytes(Path.Combine(SnapshotDirectory, $"script-{++snapshotNo:0000}-{name}.png"), runner.SnapshotPng());
        }

        public void Click(float x, float y, GuiButton button = GuiButton.Left)
        {
            GUI.PointerMoved(x, y);
            GUI.PointerPressed(x, y, button);
            GUI.PointerReleased(x, y, button);
            runner.Sync();
        }

        /// <summary>マップのマスをクリックする。画面外なら先にそのマスを中央に寄せる。</summary>
        public void ClickCell(int x, int y, GuiButton button = GuiButton.Left)
        {
            var (px, py) = OnGame(() =>
            {
                var cx = GUI.MapToPixelX(x);
                var cy = GUI.MapToPixelY(y);
                if (cx < 0 || cy < 0 || cx >= ScreenLayout.MapWidthPx || cy >= ScreenLayout.MapHeightPx)
                {
                    GUI.Center(x, y);
                    GUI.RefreshScreen();
                    cx = GUI.MapToPixelX(x);
                    cy = GUI.MapToPixelY(y);
                }
                return (cx + ScreenLayout.CellPx / 2f, cy + ScreenLayout.CellPx / 2f);
            });
            Click(px, py, button);
        }

        public bool SelectMenu(string label)
        {
            if (!WaitUntil(() => runner.State.Menu != null, TimeSpan.FromSeconds(10)))
            {
                return false;
            }
            var menu = runner.State.Menu;
            var index = menu.Items.IndexOf(label);
            if (index < 0)
            {
                index = menu.Items.Select((x, i) => (x, i)).Where(t => t.x.Contains(label)).Select(t => t.i).DefaultIfEmpty(-1).First();
            }
            if (index < 0)
            {
                return false;
            }
            var p = GUI.Renderer.MenuItemCenter(menu, index);
            Click(p.X, p.Y);
            return true;
        }

        public void SelectList(string item)
        {
            if (!WaitUntil(() => runner.State.List != null && GUI.IsWaitingList, DefaultTimeout))
            {
                throw new PlayScriptException("リストが出ません");
            }
            var list = runner.State.List;
            int index;
            if (item.StartsWith("#"))
            {
                index = int.Parse(item.Substring(1)) - 1;
            }
            else
            {
                index = Enumerable.Range(0, list.Items.Count).FirstOrDefault(i => (list.Items[i].Text ?? "").Contains(item), -1);
            }
            if (index < 0 || index >= list.Items.Count)
            {
                throw new PlayScriptException($"リストに「{item}」がありません: {string.Join(",", list.Items.Select(x => x.Text))}");
            }
            ChooseListIndex(list, index);
        }

        /// <summary>リストの項目を選ぶ（キー操作）。</summary>
        public void ChooseListIndex(ListState list, int index)
        {
            list.Highlight = index;
            GUI.KeyPressed(ScreenKey.Enter);
            WaitUntil(() => runner.State.List != list || !GUI.IsWaitingList, TimeSpan.FromSeconds(10));
        }

        public void PressDialog(string button)
        {
            if (!WaitUntil(() => runner.State.Dialog != null, DefaultTimeout))
            {
                throw new PlayScriptException("ダイアログが出ません");
            }
            var dialog = runner.State.Dialog;
            var index = button == null ? 0 : dialog.Buttons.IndexOf(button);
            if (index < 0)
            {
                throw new PlayScriptException($"ダイアログに「{button}」がありません");
            }
            var rect = GUI.Renderer.LayoutDialog(dialog).ButtonRect(index);
            Click(rect.MidX, rect.MidY);
        }

        /// <summary>右クリック（キャンセル）</summary>
        public void Cancel()
        {
            Click(ScreenLayout.MapWidthPx / 2f, ScreenLayout.MapHeightPx / 2f, GuiButton.Right);
        }

        #endregion
    }

    /// <summary>
    /// 味方フェイズを簡単な方針で自動で行う。行動できる味方ユニットごとに、
    /// 攻撃できる敵がいれば攻撃し、いなければ最も近い敵へ近づいて（近づいた先で攻撃できれば攻撃し）待機する。最後にターンを終える。
    /// </summary>
    public sealed class ScriptedPhase
    {
        private readonly PlayScript script;

        public ScriptedPhase(PlayScript script)
        {
            this.script = script;
        }

        private static readonly TimeSpan StepTimeout = TimeSpan.FromSeconds(30);

        private void Log(string message) => script.Record($"{DateTime.Now:HH:mm:ss} PHASE {message}");

        private sealed class UnitInfo
        {
            public string ID;
            public string Name;
            public int X;
            public int Y;
        }

        private List<UnitInfo> Units(Func<Unit, bool> filter)
        {
            return script.OnGame(() => SrcOf(script).UList.Items
                .Where(u => u.Status == "出撃" && filter(u))
                .Select(u => new UnitInfo { ID = u.ID, Name = u.Name, X = u.x, Y = u.y })
                .ToList());
        }

        private static SRC SrcOf(PlayScript script) => script.Src;

        private static bool IsEnemyOf(Unit u) => u.Party0 == "敵" || u.Party0 == "中立";

        public void Run()
        {
            var acted = new HashSet<string>();
            for (var guard = 0; guard < 50; guard++)
            {
                if (!script.AutoUntilReady(TimeSpan.FromMinutes(5)))
                {
                    Log("操作を受け付ける状態になりません");
                    return;
                }
                var unit = Units(u => u.Party0 == "味方" && u.Action > 0 && !acted.Contains(u.ID)).FirstOrDefault();
                if (unit == null)
                {
                    break;
                }
                acted.Add(unit.ID);
                ActUnit(unit);
            }
            EndTurn();
        }

        private void ActUnit(UnitInfo unit)
        {
            if (script.Policy == "smart")
            {
                ActUnitSmart(unit);
                return;
            }
            Log($"{unit.Name} ({unit.X},{unit.Y})");
            script.ClickCell(unit.X, unit.Y);
            if (!script.WaitUntil(() => script.Runner.State.Menu != null, TimeSpan.FromSeconds(10)))
            {
                Log("ユニットのメニューが出ません");
                script.Cancel();
                return;
            }
            var items = script.Runner.State.Menu.Items;
            if (items.Contains("攻撃") && TryAttack())
            {
                return;
            }
            if (script.Runner.State.Menu == null)
            {
                script.ClickCell(unit.X, unit.Y);
                script.WaitUntil(() => script.Runner.State.Menu != null, TimeSpan.FromSeconds(10));
            }
            if (script.Policy == "advance" && script.Runner.State.Menu != null && script.Runner.State.Menu.Items.Contains("移動") && TryMoveTowardEnemy(unit))
            {
                // 移動後のメニュー
                if (script.WaitUntil(() => script.Runner.State.Menu != null, TimeSpan.FromSeconds(10)))
                {
                    if (script.Runner.State.Menu.Items.Contains("攻撃") && TryAttack())
                    {
                        return;
                    }
                    if (script.Runner.State.Menu == null)
                    {
                        // 攻撃をやめた後は移動後のメニューに戻る
                        script.WaitUntil(() => script.Runner.State.Menu != null, TimeSpan.FromSeconds(10));
                    }
                    if (!script.SelectMenu("待機"))
                    {
                        script.Cancel();
                    }
                }
                return;
            }
            if (!script.SelectMenu("待機"))
            {
                script.Cancel();
            }
        }

        // ── smart: SRC の CPU と同じ見積もりで目標と武器を選ぶ ──

        private sealed class AttackPlan
        {
            public string Weapon;
            public UnitInfo Target;
            public double Score;
        }

        /// <summary>HP がこの割合を下回ったら、倒しきれる敵がいなければ下がる</summary>
        private const double RetreatRatio = 0.35;

        private static UnitInfo Info(Unit u) => new UnitInfo { ID = u.ID, Name = u.Name, X = u.x, Y = u.y };

        /// <summary>
        /// 攻撃の評価。命中の見込みで重みを付けた与ダメージの割合に、撃破とボス（ステージの勝利条件になりやすい）の分を加える。
        /// </summary>
        private static double AttackScore(int prob, int dmg, Unit t)
        {
            var p = Math.Min(prob, 100) / 100d;
            var hp = Math.Max(t.HP, 1);
            var score = p * Math.Min(dmg, hp) / hp;
            if (dmg >= hp)
            {
                score += p;
            }
            if (t.BossRank >= 0)
            {
                // ボスは撃破か一定の損傷でステージが終わることが多いので、削るだけでも重く見る
                score = score * 2 + 0.5 * p;
            }
            return score;
        }

        private static int Distance(int x1, int y1, int x2, int y2) => Math.Abs(x1 - x2) + Math.Abs(y1 - y2);

        /// <summary>敵がこのマスに次のターンで攻撃できるか（移動力と最大射程による概算）</summary>
        private static int Danger(IEnumerable<Unit> enemies, int x, int y)
        {
            return enemies.Count(e =>
            {
                var range = e.Weapons.Where(w => w.IsWeaponAvailable("移動前")).Select(w => w.WeaponMaxRange()).DefaultIfEmpty(1).Max();
                return Distance(e.x, e.y, x, y) <= e.Speed + range;
            });
        }

        /// <summary>その場から攻撃する場合の最善（SRC の COM.SelectWeapon で武器を選ぶ）</summary>
        private AttackPlan PlanAttackInPlace(string unitId)
        {
            return script.OnGame(() =>
            {
                var src = script.Src;
                var u = src.UList.Item(unitId);
                AttackPlan best = null;
                foreach (var t in src.UList.Items.Where(x => x.Status == "出撃" && IsEnemyOf(x)))
                {
                    var w = src.COM.SelectWeapon(u, t, "移動前", out var prob, out var dmg);
                    if (w <= 0)
                    {
                        continue;
                    }
                    var score = AttackScore(prob, dmg, t);
                    if (best == null || score > best.Score)
                    {
                        best = new AttackPlan { Weapon = u.Weapon(w).WeaponNickname(), Target = Info(t), Score = score };
                    }
                }
                return best;
            });
        }

        /// <summary>
        /// 移動先を選ぶ（「移動」を選んで移動できる範囲が示された状態で呼ぶ）。
        /// 移動後に攻撃できるマスは攻撃の評価、できなければ最も近い敵への近さで選び、どちらも敵から受ける危険を差し引く。
        /// 下がる場合は敵から離れるマスを選ぶ。
        /// </summary>
        private (int X, int Y, AttackPlan Attack)? PlanMove(string unitId, bool retreat)
        {
            return script.OnGame(() =>
            {
                var src = script.Src;
                var map = src.Map;
                var u = src.UList.Item(unitId);
                var enemies = src.UList.Items.Where(x => x.Status == "出撃" && IsEnemyOf(x)).ToList();
                if (!enemies.Any())
                {
                    return ((int, int, AttackPlan)?)null;
                }
                var hpRatio = u.HP / (double)u.MaxHP;
                // HP が少ないほど危険を重く見る
                var dangerWeight = 0.2 + (1 - hpRatio);
                var weapons = u.Weapons.Where(w => w.IsWeaponAvailable("移動後")).ToList();
                var bosses = enemies.Where(e => e.BossRank >= 0).ToList();
                var goals = bosses.Any() ? bosses : enemies;
                (int, int, AttackPlan)? best = null;
                var bestScore = double.MinValue;
                for (var x = 1; x <= map.MapWidth; x++)
                {
                    for (var y = 1; y <= map.MapHeight; y++)
                    {
                        if (map.MaskData[x, y] || (map.MapDataForUnit[x, y] != null && map.MapDataForUnit[x, y] != u))
                        {
                            continue;
                        }
                        var danger = Danger(enemies, x, y);
                        var nearest = enemies.Min(e => Distance(e.x, e.y, x, y));
                        // 攻撃できないときはボスへ向かう（ボスがいなければ最も近い敵）
                        var goal = goals.Min(e => Distance(e.x, e.y, x, y));
                        double score;
                        AttackPlan attack = null;
                        if (retreat)
                        {
                            score = nearest - danger * 2;
                        }
                        else
                        {
                            foreach (var w in weapons)
                            {
                                foreach (var t in enemies)
                                {
                                    var d = Distance(x, y, t.x, t.y);
                                    if (d > w.WeaponMaxRange() || d < w.WeaponData.MinRange)
                                    {
                                        continue;
                                    }
                                    var s = AttackScore(w.HitProbability(t, false), w.ExpDamage(t, false), t);
                                    if (attack == null || s > attack.Score)
                                    {
                                        attack = new AttackPlan { Weapon = w.WeaponNickname(), Target = Info(t), Score = s };
                                    }
                                }
                            }
                            score = attack != null
                                ? 10 + attack.Score * 10 - danger * dangerWeight
                                : -goal - danger * dangerWeight;
                        }
                        if (score > bestScore)
                        {
                            bestScore = score;
                            best = (x, y, attack);
                        }
                    }
                }
                return best;
            });
        }

        /// <summary>メニューの「攻撃」から、指定の武器で指定の敵を攻撃する</summary>
        private bool Attack(AttackPlan plan)
        {
            if (!script.SelectMenu("攻撃"))
            {
                return false;
            }
            if (!script.WaitUntil(() => script.Runner.State.List != null && script.Runner.GUI.IsWaitingList, TimeSpan.FromSeconds(10)))
            {
                return false;
            }
            var list = script.Runner.State.List;
            var index = Enumerable.Range(0, list.Items.Count)
                .FirstOrDefault(i => !list.Items[i].Disabled && (list.Items[i].Text ?? "").TrimStart('×', ' ').StartsWith(plan.Weapon), -1);
            if (index < 0)
            {
                Log($"武器がありません: {plan.Weapon}");
                script.Runner.GUI.KeyPressed(ScreenKey.Escape);
                return false;
            }
            script.ChooseListIndex(list, index);
            if (!script.WaitUntil(() => script.CommandState.Contains("ターゲット選択"), TimeSpan.FromSeconds(10)))
            {
                return false;
            }
            // 移動後などで計画した相手を選べない（マスクされている）ならクリックしない
            if (script.OnGame(() => script.Src.Map.MaskData[plan.Target.X, plan.Target.Y]))
            {
                Log($"攻撃できない: {plan.Target.Name} ({plan.Target.X},{plan.Target.Y}) {plan.Weapon}");
                script.Cancel();
                script.WaitUntil(() => !script.CommandState.Contains("ターゲット選択"), TimeSpan.FromSeconds(10));
                return false;
            }
            Log($"攻撃: {plan.Target.Name} ({plan.Target.X},{plan.Target.Y}) {plan.Weapon}");
            script.ClickCell(plan.Target.X, plan.Target.Y);
            script.AutoUntilReady(TimeSpan.FromMinutes(3));
            return true;
        }

        private bool OpenUnitMenu(UnitInfo unit)
        {
            script.ClickCell(unit.X, unit.Y);
            return script.WaitUntil(() => script.Runner.State.Menu != null, TimeSpan.FromSeconds(10));
        }

        private void Wait()
        {
            if (!script.SelectMenu("待機"))
            {
                script.Cancel();
            }
        }

        /// <summary>
        /// 自分に使うスペシャルパワー（精神コマンド）から、いま使うものを選ぶ。
        /// HP が減っていれば回復、敵の攻撃が届く位置で HP が減り始めていれば回避・防御を使う。回復の分の SP は残す。
        /// </summary>
        private string PlanSpecialPower(string unitId)
        {
            return script.OnGame(() =>
            {
                var src = script.Src;
                var u = src.UList.Item(unitId);
                var p = u.MainPilot();
                if (p == null || u.IsConditionSatisfied("スペシャルパワー使用不能"))
                {
                    return null;
                }
                var candidates = p.SpecialPowerNames
                    .Select(name => (Name: name, Data: src.SPDList.Item(name), Cost: p.SpecialPowerCost(name)))
                    .Where(x => x.Data != null && x.Data.TargetType == "自分" && x.Cost <= p.SP)
                    .ToList();
                var heals = candidates.Where(x => x.Data.IsEffectAvailable("ＨＰ回復")).ToList();
                var hpRatio = u.HP / (double)u.MaxHP;
                if (hpRatio < 0.5)
                {
                    var heal = heals.Where(x => p.IsSpecialPowerUseful(x.Name)).OrderBy(x => x.Cost).FirstOrDefault();
                    if (heal.Name != null)
                    {
                        return heal.Name;
                    }
                }
                var enemies = src.UList.Items.Where(x => x.Status == "出撃" && IsEnemyOf(x)).ToList();
                if (hpRatio < 0.8 && Danger(enemies, u.x, u.y) > 0)
                {
                    var reserve = heals.Select(x => x.Cost).DefaultIfEmpty(0).Min();
                    var guard = candidates
                        .Where(x => x.Data.IsEffectAvailable("絶対回避") || x.Data.IsEffectAvailable("回避強化")
                            || x.Data.IsEffectAvailable("装甲強化") || x.Data.IsEffectAvailable("ダメージ低下")
                            || x.Data.IsEffectAvailable("被ダメージ低下"))
                        .Where(x => p.SP - x.Cost >= reserve && p.IsSpecialPowerUseful(x.Name))
                        .OrderBy(x => x.Cost)
                        .FirstOrDefault();
                    if (guard.Name != null)
                    {
                        return guard.Name;
                    }
                }
                return null;
            });
        }

        /// <summary>メニューの「精神」（スペシャルパワー）から name を使う。使えたら true。</summary>
        private bool UseSpecialPower(UnitInfo unit, string name)
        {
            var label = script.OnGame(() => script.Src.Expression.Term("スペシャルパワー", script.Src.UList.Item(unit.ID)));
            if (!script.SelectMenu(label))
            {
                return false;
            }
            if (!script.WaitUntil(() => script.Runner.State.List != null && script.Runner.GUI.IsWaitingList, TimeSpan.FromSeconds(10)))
            {
                return false;
            }
            var list = script.Runner.State.List;
            var index = Enumerable.Range(0, list.Items.Count)
                .FirstOrDefault(i => !list.Items[i].Disabled && (list.Items[i].Text ?? "").StartsWith(name), -1);
            if (index < 0)
            {
                Log($"{label}がありません: {name}");
                script.Runner.GUI.KeyPressed(ScreenKey.Escape);
                script.AutoUntilReady(TimeSpan.FromSeconds(30));
                return false;
            }
            Log($"{label}: {name}");
            script.ChooseListIndex(list, index);
            script.AutoUntilReady(TimeSpan.FromMinutes(1));
            return true;
        }

        private void ActUnitSmart(UnitInfo unit)
        {
            // 行動の前にスペシャルパワーを使う（使うとメニューが閉じるので、開き直す）
            for (var i = 0; i < 2; i++)
            {
                var sp = PlanSpecialPower(unit.ID);
                if (sp == null || !OpenUnitMenu(unit) || !UseSpecialPower(unit, sp))
                {
                    if (script.Runner.State.Menu != null)
                    {
                        script.Cancel();
                    }
                    break;
                }
            }
            var (hpRatio, allies, spInfo) = script.OnGame(() =>
            {
                var u = script.Src.UList.Item(unit.ID);
                var p = u.MainPilot();
                var spText = p == null ? "" : $" SP {p.SP}/{p.MaxSP} {string.Join(" ", p.SpecialPowerNames)}";
                return (u.HP / (double)u.MaxHP, script.Src.UList.Items.Count(x => x.Status == "出撃" && x.Party0 == "味方"), spText);
            });
            Log($"{unit.Name} ({unit.X},{unit.Y}) HP {hpRatio:P0}{spInfo}");
            if (!OpenUnitMenu(unit))
            {
                Log("ユニットのメニューが出ません");
                script.Cancel();
                return;
            }
            // 修理の手段が無いので、下がるのは他に戦える味方がいるときだけ（一人で下がっても敵が近づくのを待つだけになる）
            var retreat = hpRatio < RetreatRatio && allies > 1;

            // その場から攻撃する。HP が少なくても、倒しきれる見込みが高ければ攻撃する
            var inPlace = script.Runner.State.Menu.Items.Contains("攻撃") ? PlanAttackInPlace(unit.ID) : null;
            if (inPlace != null && (!retreat || inPlace.Score >= 1.5))
            {
                // 移動できない場合と、倒しきれる見込みが高い場合はすぐ攻撃する。それ以外は移動して攻撃する候補と比べる
                if (!script.Runner.State.Menu.Items.Contains("移動") || inPlace.Score >= 1.5)
                {
                    if (Attack(inPlace))
                    {
                        return;
                    }
                    if (script.Runner.State.Menu == null && !OpenUnitMenu(unit))
                    {
                        return;
                    }
                }
            }

            if (script.Runner.State.Menu != null && script.Runner.State.Menu.Items.Contains("移動"))
            {
                script.SelectMenu("移動");
                if (!script.WaitUntil(() => script.CommandState.Contains("ターゲット選択"), TimeSpan.FromSeconds(10)))
                {
                    script.Cancel();
                    return;
                }
                var move = PlanMove(unit.ID, retreat);
                var stay = move == null || (move.Value.X == unit.X && move.Value.Y == unit.Y);
                var moveScore = move?.Attack?.Score ?? 0;
                if (stay || (inPlace != null && !retreat && inPlace.Score >= moveScore))
                {
                    // 動かないほうが良い。移動を取り消して、その場で攻撃するか待機する
                    script.Cancel();
                    script.WaitUntil(() => !script.CommandState.Contains("ターゲット選択"), TimeSpan.FromSeconds(10));
                    if (script.Runner.State.Menu == null && !OpenUnitMenu(unit))
                    {
                        return;
                    }
                    if (inPlace != null && Attack(inPlace))
                    {
                        return;
                    }
                    if (script.Runner.State.Menu == null && !OpenUnitMenu(unit))
                    {
                        return;
                    }
                    Wait();
                    return;
                }
                Log($"{(retreat ? "後退" : "移動")}: ({move.Value.X},{move.Value.Y})");
                script.ClickCell(move.Value.X, move.Value.Y);
                if (!script.WaitUntil(() => script.Runner.State.Menu != null, TimeSpan.FromSeconds(10)))
                {
                    return;
                }
                if (move.Value.Attack != null && script.Runner.State.Menu.Items.Contains("攻撃") && Attack(move.Value.Attack))
                {
                    return;
                }
                if (script.Runner.State.Menu == null)
                {
                    script.WaitUntil(() => script.Runner.State.Menu != null, TimeSpan.FromSeconds(10));
                }
                Wait();
                return;
            }

            if (inPlace != null && Attack(inPlace))
            {
                return;
            }
            if (script.Runner.State.Menu == null && !OpenUnitMenu(unit))
            {
                return;
            }
            Wait();
        }

        /// <summary>メニューの「攻撃」から、届く敵を攻撃する。攻撃できなければキャンセルして false。</summary>
        private bool TryAttack()
        {
            script.SelectMenu("攻撃");
            // 武器の一覧から使える最初の武器を選ぶ
            if (!script.WaitUntil(() => script.Runner.State.List != null && script.Runner.GUI.IsWaitingList, TimeSpan.FromSeconds(10)))
            {
                return false;
            }
            var list = script.Runner.State.List;
            var weapon = Enumerable.Range(0, list.Items.Count).FirstOrDefault(i => !list.Items[i].Disabled, -1);
            if (weapon < 0)
            {
                script.Runner.GUI.KeyPressed(ScreenKey.Escape);
                return false;
            }
            script.ChooseListIndex(list, weapon);
            if (!script.WaitUntil(() => script.CommandState.Contains("ターゲット選択"), TimeSpan.FromSeconds(10)))
            {
                return false;
            }
            // 選べる（マスクされていない）マスにいる敵
            var target = script.OnGame(() =>
            {
                var map = script.Src.Map;
                return script.Src.UList.Items
                    .Where(u => u.Status == "出撃" && IsEnemyOf(u) && !map.MaskData[u.x, u.y])
                    .Select(u => new UnitInfo { ID = u.ID, Name = u.Name, X = u.x, Y = u.y })
                    .FirstOrDefault();
            });
            if (target == null)
            {
                Log("攻撃できる敵がいない");
                script.Cancel();
                script.WaitUntil(() => !script.CommandState.Contains("ターゲット選択"), TimeSpan.FromSeconds(10));
                return false;
            }
            Log($"攻撃: {target.Name} ({target.X},{target.Y})");
            script.ClickCell(target.X, target.Y);
            // 反撃の選択などの確認は自動で進める
            script.AutoUntilReady(TimeSpan.FromMinutes(3));
            return true;
        }

        /// <summary>最も近い敵へ近づくマスへ移動する。</summary>
        private bool TryMoveTowardEnemy(UnitInfo unit)
        {
            script.SelectMenu("移動");
            if (!script.WaitUntil(() => script.CommandState.Contains("ターゲット選択"), TimeSpan.FromSeconds(10)))
            {
                return false;
            }
            var dest = script.OnGame(() =>
            {
                var src = script.Src;
                var map = src.Map;
                var enemies = src.UList.Items.Where(u => u.Status == "出撃" && IsEnemyOf(u)).ToList();
                if (!enemies.Any())
                {
                    return ((int, int)?)null;
                }
                (int, int)? best = null;
                var bestDistance = int.MaxValue;
                for (var x = 1; x <= map.MapWidth; x++)
                {
                    for (var y = 1; y <= map.MapHeight; y++)
                    {
                        if (map.MaskData[x, y] || (map.MapDataForUnit[x, y] != null && !(x == unit.X && y == unit.Y)))
                        {
                            continue;
                        }
                        var distance = enemies.Min(e => Math.Abs(e.x - x) + Math.Abs(e.y - y));
                        if (distance < bestDistance)
                        {
                            bestDistance = distance;
                            best = (x, y);
                        }
                    }
                }
                return best;
            });
            if (dest == null)
            {
                script.Cancel();
                return false;
            }
            Log($"移動: ({dest.Value.Item1},{dest.Value.Item2})");
            script.ClickCell(dest.Value.Item1, dest.Value.Item2);
            return true;
        }

        private void EndTurn()
        {
            if (!script.AutoUntilReady(TimeSpan.FromMinutes(5)))
            {
                return;
            }
            // ユニットのいないマスを右クリックしてマップコマンドを出す（元の SRC と同じく左クリックはドラッグになる）
            var empty = script.OnGame(() =>
            {
                var gui = script.Runner.GUI;
                var map = script.Src.Map;
                for (var y = gui.MapY - 6; y <= gui.MapY + 6; y++)
                {
                    for (var x = gui.MapX - 6; x <= gui.MapX + 6; x++)
                    {
                        if (map.IsInside(x, y) && map.MapDataForUnit[x, y] == null)
                        {
                            return ((int, int)?)(x, y);
                        }
                    }
                }
                return null;
            });
            if (empty == null)
            {
                Log("空きマスがありません");
                return;
            }
            script.ClickCell(empty.Value.Item1, empty.Value.Item2, GuiButton.Right);
            if (!script.SelectMenu("ターン終了"))
            {
                Log("ターン終了がメニューにありません");
                script.Cancel();
                return;
            }
            Log("ターン終了");
            // 確認ダイアログは自動で OK を押す（AutoPlayer が Enter を押す）
        }
    }

    public class PlayScriptException : Exception
    {
        public PlayScriptException(string message, Exception inner = null) : base(message, inner)
        {
        }
    }
}
