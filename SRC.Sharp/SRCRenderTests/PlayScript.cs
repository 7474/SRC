using SRCCore;
using SRCCore.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace SRCRender.Tests
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
    ///   print 式              SRC の式の値を記録する（進行状況の確認用。例: print ダンジョン階数）
    ///   policy advance|hold    phase の方針。advance は攻撃できなければ敵へ近づく（既定）、hold は移動せず届く敵だけ攻撃する
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

        /// <summary>phase で移動せず、届く敵だけ攻撃する</summary>
        public bool HoldPosition { get; set; }

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
                case "print":
                    {
                        var expr = Rest(args, 1);
                        var value = OnGame(() => Src.Expression.GetValueAsString(expr));
                        Record($"{DateTime.Now:HH:mm:ss} PRINT {expr} = {value}");
                    }
                    break;
                case "policy":
                    HoldPosition = args[1] == "hold";
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
                if (!done.Wait(DefaultTimeout))
                {
                    throw new TimeoutException("ゲームスレッドが応答しません");
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
                // 状態が落ち着いてから判定する（入力の処理中の一瞬を拾わない）
                if (IsReady)
                {
                    Thread.Sleep(150);
                    if (IsReady)
                    {
                        return true;
                    }
                }
                // 停滞時にボタンを押す（StallClickMillis）より長く待つ
                auto.Run(int.MaxValue, 60000, () => IsReady);
            }
            return IsReady;
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
            if (!script.HoldPosition && script.Runner.State.Menu != null && script.Runner.State.Menu.Items.Contains("移動") && TryMoveTowardEnemy(unit))
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
