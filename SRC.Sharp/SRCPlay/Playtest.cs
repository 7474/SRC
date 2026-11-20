using SRCCore;
using SRCCore.CmdDatas;
using SRCCore.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace SRCPlay
{
    /// <summary>1 回の強さの確かめの結果（7474/SRC#1690）</summary>
    public sealed class PlaytestRun
    {
        public int Seed { get; set; }
        /// <summary>win（GameClear・Continue に着いた）・lose（GameOver・味方の全滅）・timeout（ターンの上限まで決着しない）・stall（自動プレイが進めなくなった）・stop（それ以外で終わった）</summary>
        public string Outcome { get; set; }
        /// <summary>終わったときのターン</summary>
        public int Turns { get; set; }
        /// <summary>落ちた味方（ユニットの名前とパイロット）</summary>
        public List<string> Lost { get; set; } = new List<string>();
        /// <summary>残った味方の HP（%）</summary>
        public List<(string name, int hp)> Survivors { get; set; } = new List<(string, int)>();
        /// <summary>イベントのエラー</summary>
        public List<string> Errors { get; set; } = new List<string>();
    }

    /// <summary>強さの確かめのまとめ</summary>
    public sealed class PlaytestSummary
    {
        public string Scenario { get; set; }
        public int TurnLimit { get; set; }
        public string Policy { get; set; }
        public List<PlaytestRun> Runs { get; set; } = new List<PlaytestRun>();

        public int Count(string outcome) => Runs.Count(r => r.Outcome == outcome);

        /// <summary>勝てた回の割合（止まった回は数に入れない。#1668）</summary>
        public double WinRate => Runs.Count(r => r.Outcome != "stall") is var n && n > 0 ? (double)Count("win") / n : 0;

        /// <summary>
        /// 直し方の手がかり（スパロボ・SRC の慣習: 味方と敵のレベルに極端な差は付けず、敵をやや上にする）。
        /// 自動で遊ぶ AI は人より弱いので、目安は「5 回中 3 回くらい勝つ」
        /// </summary>
        public IReadOnlyList<string> Hints()
        {
            var hints = new List<string>();
            var judged = Runs.Count(r => r.Outcome != "stall");
            if (judged == 0)
            {
                hints.Add("どの回も自動プレイが進めなくなりました。イベントの途中で入力を待っていないか（Ask・Input など）を確かめてください");
                return hints;
            }
            if (Count("stall") > 0)
            {
                hints.Add($"{Count("stall")} 回は自動プレイが途中で進めなくなりました（勝ち負けの数には入れていません）");
            }
            if (Count("timeout") * 2 >= judged)
            {
                hints.Add($"{TurnLimit} ターンで決着しない回が多いです。勝利の条件に届くか（倒す相手・着くマス）、敵が下がって回復し続けていないか（強敵にはボスランクを付ける）を確かめてください");
            }
            if (WinRate < 0.5)
            {
                hints.Add("負けやすいです。敵のレベルを 1〜2 下げる、敵を減らす、強敵のボスランクを下げる、味方の置く位置を散らす、などを試してください（味方のレベルを大きく上げる直し方は、ほかの話との釣り合いが崩れるので勧めません）");
            }
            else if (WinRate >= 1 && Runs.Where(r => r.Outcome == "win").All(r => r.Lost.Count == 0) && Runs.Where(r => r.Outcome == "win").Average(r => r.Turns) <= 3)
            {
                hints.Add("楽に勝てます（誰も落ちず、すぐに決着します）。山場にしたいなら、敵のレベルを 1〜2 上げる、敵を足す、強敵にボスランクを付ける、などを試してください");
            }
            else
            {
                hints.Add("自動で遊ぶ AI でもおおむね勝てます。人が遊ぶと、これより勝ちやすくなります");
            }
            return hints;
        }

        public string ToMarkdown()
        {
            var sb = new StringBuilder();
            sb.Append("# 強さの確かめ: ").Append(Scenario).Append('\n').Append('\n');
            sb.Append($"自動で {Runs.Count} 回遊んだ（方針 {Policy}、{TurnLimit} ターンまで）。勝ち {Count("win")}・負け {Count("lose")}・時間切れ {Count("timeout")}");
            if (Count("stall") > 0)
            {
                sb.Append($"・止まった {Count("stall")}");
            }
            if (Count("stop") > 0)
            {
                sb.Append($"・そのほか {Count("stop")}");
            }
            sb.Append('\n').Append('\n');
            var wins = Runs.Where(r => r.Outcome == "win").ToList();
            if (wins.Count > 0)
            {
                sb.Append($"勝った回のターン数: {wins.Min(r => r.Turns)}〜{wins.Max(r => r.Turns)}（平均 {wins.Average(r => r.Turns).ToString("0.#", CultureInfo.InvariantCulture)}）\n\n");
            }
            sb.Append("| 乱数 | 結果 | ターン | 落ちた味方 | 残った味方の HP |\n|---|---|---|---|---|\n");
            foreach (var r in Runs)
            {
                sb.Append($"| {r.Seed} | {Label(r.Outcome)} | {r.Turns} | {(r.Lost.Count == 0 ? "なし" : string.Join("、", r.Lost))} | {string.Join("、", r.Survivors.Select(s => $"{s.name} {s.hp}%"))} |\n");
            }
            sb.Append('\n').Append("## 手がかり\n\n");
            foreach (var hint in Hints())
            {
                sb.Append("- ").Append(hint).Append('\n');
            }
            var errors = Runs.SelectMany(r => r.Errors).Distinct().ToList();
            if (errors.Count > 0)
            {
                sb.Append('\n').Append("## イベントのエラー\n\n");
                foreach (var e in errors)
                {
                    sb.Append("- ").Append(e.Replace("\n", " ")).Append('\n');
                }
            }
            sb.Append('\n').Append("自動で遊ぶ AI はエースが一機で突っ込みがちで、人より負けやすい。結果は「人ならこれより勝ちやすい」目安として見る。\n");
            return sb.ToString();
        }

        public static string Label(string outcome) => outcome switch
        {
            "win" => "勝ち",
            "lose" => "負け",
            "timeout" => "時間切れ",
            "stall" => "止まった",
            _ => "そのほか",
        };

        public string ToJson()
        {
            using var stream = new System.IO.MemoryStream();
            using (var w = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }))
            {
                w.WriteStartObject();
                w.WriteString("scenario", Scenario);
                w.WriteNumber("turnLimit", TurnLimit);
                w.WriteString("policy", Policy);
                w.WriteNumber("winRate", WinRate);
                w.WriteStartObject("counts");
                foreach (var o in new[] { "win", "lose", "timeout", "stall", "stop" })
                {
                    w.WriteNumber(o, Count(o));
                }
                w.WriteEndObject();
                w.WriteStartArray("runs");
                foreach (var r in Runs)
                {
                    w.WriteStartObject();
                    w.WriteNumber("seed", r.Seed);
                    w.WriteString("outcome", r.Outcome);
                    w.WriteNumber("turns", r.Turns);
                    w.WriteStartArray("lost");
                    r.Lost.ForEach(w.WriteStringValue);
                    w.WriteEndArray();
                    w.WriteStartArray("survivors");
                    foreach (var (name, hp) in r.Survivors)
                    {
                        w.WriteStartObject();
                        w.WriteString("name", name);
                        w.WriteNumber("hp", hp);
                        w.WriteEndObject();
                    }
                    w.WriteEndArray();
                    w.WriteStartArray("errors");
                    r.Errors.ForEach(w.WriteStringValue);
                    w.WriteEndArray();
                    w.WriteEndObject();
                }
                w.WriteEndArray();
                w.WriteStartArray("hints");
                foreach (var h in Hints())
                {
                    w.WriteStringValue(h);
                }
                w.WriteEndArray();
                w.WriteEndObject();
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }

    /// <summary>
    /// 強さの確かめ（7474/SRC#1690。srcsharp playtest）。1 話を、乱数を変えて何回か自動で遊び、勝ち負け・ターン数・落ちた味方を見る。
    /// 勝ち負けは、GameClear・GameOver（と Continue）に着いたか、味方が全滅したかで決める（「最後まで進んだ」を勝ちにしない。負けも最後まで進むため）。
    /// 1 話だけを見るので、Continue に着いたら勝ちとして、そこで終える
    /// </summary>
    public sealed class PlaytestObserver : IEventDebugger
    {
        private SRC src;
        private IEventDebugger inner;
        private readonly List<string> errors = new List<string>();

        /// <summary>勝ち（win）か負け（lose）のコマンドに着いたら、その結果</summary>
        public string Outcome { get; private set; }

        public PlaytestObserver(ScreenScenarioRunner runner)
        {
            Attach(runner.Session.SRC);
            runner.SessionCreated += session => Attach(session.SRC);
        }

        private void Attach(SRC newSrc)
        {
            src = newSrc;
            inner = src.Event.Debugger;
            src.Event.Debugger = this;
        }

        public IReadOnlyList<string> Errors
        {
            get
            {
                lock (errors)
                {
                    return errors.ToList();
                }
            }
        }

        public void BeforeExecute(CmdData command)
        {
            if (Outcome == null)
            {
                switch (command.Name)
                {
                    case CmdType.GameClearCmd:
                        Outcome = "win";
                        break;
                    case CmdType.GameOverCmd:
                        Outcome = "lose";
                        break;
                    case CmdType.ContinueCmd:
                        // 次の話へ進む。1 話だけを見るので、勝ちとしてここで終える（GameClear と同じく終了させる）
                        Outcome = "win";
                        inner?.BeforeExecute(command);
                        src.GameClear();
                        return;
                }
            }
            inner?.BeforeExecute(command);
        }

        public void OnError(EventDataLine line, string message)
        {
            if (Outcome == null)
            {
                lock (errors)
                {
                    errors.Add($"{System.IO.Path.GetFileName(line.File ?? "")}:{line.LineNum}: {message}");
                }
            }
            inner?.OnError(line, message);
        }
    }

    public static class Playtest
    {
        /// <summary>1 回遊ぶ。runner は Start 前のもの（乱数の種は呼ぶ側で決める）</summary>
        public static PlaytestRun RunOnce(ScreenScenarioRunner runner, string eve, string scenarioRoot, int seed, int turns, string policy, Action<string> trace = null)
        {
            runner.Session.SRC.InitialRndSeed = seed;
            var observer = new PlaytestObserver(runner);
            var stalled = false;
            runner.Start(eve, scenarioRoot);
            var script = new PlayScript(runner)
            {
                Trace = line =>
                {
                    if (line.Contains("操作を受け付ける状態になりません"))
                    {
                        stalled = true;
                    }
                    trace?.Invoke(line);
                },
            };
            try
            {
                script.Run(new[] { "auto 400", $"policy {policy}", $"turns {turns}" });
            }
            catch (PlayScriptException e)
            {
                trace?.Invoke("error: " + e.Message);
                stalled = true;
            }

            var run = new PlaytestRun { Seed = seed, Errors = observer.Errors.ToList() };
            void Capture()
            {
                var src = runner.Session.SRC;
                run.Turns = src.Turn;
                foreach (var u in src.UList.Items.Where(u => u.Party0 == "味方"))
                {
                    var pilot = u.CountPilot() > 0 ? u.MainPilot(true)?.get_Nickname(false) : null;
                    var name = string.IsNullOrEmpty(pilot) ? u.Nickname0 : $"{u.Nickname0}（{pilot}）";
                    if (u.Status == "破壊")
                    {
                        run.Lost.Add(name);
                    }
                    else if (u.Status == "出撃")
                    {
                        run.Survivors.Add((name, u.MaxHP > 0 ? (int)Math.Round(100.0 * u.HP / u.MaxHP) : 0));
                    }
                }
            }
            if (runner.IsFinished)
            {
                Capture();
            }
            else
            {
                runner.Session.Game.Post(Capture);
                runner.TrySync(TimeSpan.FromSeconds(10));
            }

            if (observer.Outcome != null)
            {
                run.Outcome = observer.Outcome;
            }
            else if (runner.IsFinished)
            {
                // 全滅 味方 のイベントが無ければ、SRC は GameOver のコマンドを通らずにゲームオーバーにする
                run.Outcome = run.Survivors.Count == 0 && run.Lost.Count > 0 ? "lose" : "stop";
            }
            else
            {
                run.Outcome = stalled ? "stall" : "timeout";
            }
            return run;
        }
    }
}
