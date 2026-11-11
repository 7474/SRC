using SRCCore;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.Expressions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCPlay
{
    /// <summary>
    /// 自動のプレイで、各話の終わりの状態（資金・ユニットとパイロットの一覧・グローバル変数）とイベントのエラーを記録する（#1551）。
    /// <see cref="Event.Debugger"/> に付け、実行するイベントファイル（ScenarioFileName）が変わったときに、
    /// その前の話の終わりの状態を記録する。最後の話の終わりは <see cref="CaptureFinal"/> で記録する。
    /// 結果は、同じ手順・同じ乱数の種なら毎回同じ文字列になる（ゴールデンとして比べる）。
    /// </summary>
    public sealed class ScenarioStateRecorder : IEventDebugger
    {
        private readonly SRC src;
        private readonly ScreenScenarioRunner runner;
        private readonly IEventDebugger inner;
        private readonly List<string> sections = new List<string>();
        private readonly List<string> errors = new List<string>();
        private string currentFile;

        public ScenarioStateRecorder(ScreenScenarioRunner runner)
        {
            this.runner = runner;
            src = runner.Session.SRC;
            // ほかのデバッガが付いていれば、そちらにも渡す
            inner = src.Event.Debugger;
            src.Event.Debugger = this;
        }

        /// <summary>イベントのエラー（表示した順）</summary>
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
            var file = src.ScenarioFileName ?? "";
            if (currentFile == null)
            {
                currentFile = file;
            }
            else if (!string.Equals(currentFile, file, StringComparison.Ordinal))
            {
                // 次の話のイベントファイルの最初のコマンドの前。前の話（エピローグ・インターミッションまで）が終わっている
                Add(Capture(Path.GetFileName(currentFile) + " の終わり（次は " + Path.GetFileName(file) + "）"));
                currentFile = file;
            }
            inner?.BeforeExecute(command);
        }

        public void OnError(EventDataLine line, string message)
        {
            // 終了の後に、打ち切りの例外を実行中のコマンドが捕まえて出すエラーは数えない（PlayScript の expect error none と同じ）
            if (runner.IsTerminated)
            {
                inner?.OnError(line, message);
                return;
            }
            lock (errors)
            {
                errors.Add($"{Path.GetFileName(line.File ?? "")}:{line.LineNum}: {message}");
            }
            inner?.OnError(line, message);
        }

        /// <summary>
        /// 最後の話の終わりの状態を記録する。ゲームのスレッドが止まっている（シナリオが終わった）ときか、ゲームのスレッドから呼ぶ
        /// </summary>
        public void CaptureFinal(bool finished)
        {
            var file = Path.GetFileName(currentFile ?? src.ScenarioFileName ?? "");
            Add(Capture(file + (finished ? " の終わり（シナリオの終わり）" : " の途中（シナリオが終わっていない）")));
        }

        private void Add(string section)
        {
            lock (sections)
            {
                sections.Add(section);
            }
        }

        /// <summary>これまでに記録した状態とエラー</summary>
        public string ToText()
        {
            var sb = new StringBuilder();
            lock (sections)
            {
                foreach (var section in sections)
                {
                    sb.Append(section);
                    sb.Append('\n');
                }
            }
            sb.Append("## イベントのエラー\n");
            var list = Errors;
            if (list.Count == 0)
            {
                sb.Append("なし\n");
            }
            foreach (var e in list)
            {
                sb.Append(OneLine(e)).Append('\n');
            }
            return sb.ToString();
        }

        private static string OneLine(string s)
        {
            return (s ?? "").Replace("\r\n", "\\n").Replace("\r", "\\n").Replace("\n", "\\n");
        }

        private static string Num(double d)
        {
            return d.ToString("R", CultureInfo.InvariantCulture);
        }

        private static bool IsListed(string party)
        {
            return party == "味方" || party == "ＮＰＣ";
        }

        /// <summary>いまの状態を文字列にする</summary>
        public string Capture(string title)
        {
            var sb = new StringBuilder();
            sb.Append("## ").Append(title).Append('\n');
            sb.Append("資金: ").Append(src.Money.ToString(CultureInfo.InvariantCulture)).Append('\n');
            sb.Append("ターン: ").Append(src.Turn.ToString(CultureInfo.InvariantCulture))
                .Append("（総ターン数 ").Append(src.TotalTurn.ToString(CultureInfo.InvariantCulture)).Append("）\n");

            // 味方・ＮＰＣのユニット。ほかの形態（他形態・旧主形態・旧形態）と破棄したものは出さない
            sb.Append("### ユニット\n");
            var units = src.UList.Items
                .Where(u => IsListed(u.Party0))
                .Where(u => u.Status != "他形態" && u.Status != "旧主形態" && u.Status != "旧形態" && u.Status != "破棄")
                .OrderBy(u => u.ID, StringComparer.Ordinal)
                .ToList();
            foreach (var u in units)
            {
                var pilots = string.Join("・", u.Pilots.Select(p => p.Name));
                var supports = string.Join("・", u.Supports.Select(p => p.Name));
                var items = string.Join("・", u.ItemList.Select(i => i.Name));
                var line = $"{u.ID}\t{u.Name}\t{u.Party0}\t{u.Status}\tランク{u.Rank}\tHP {u.HP}/{u.MaxHP}\tEN {u.EN}/{u.MaxEN}";
                if (u.Status == "出撃")
                {
                    line += $"\t({u.x},{u.y})";
                }
                line += $"\tパイロット {pilots}";
                if (supports.Length > 0)
                {
                    line += $"\tサポート {supports}";
                }
                if (items.Length > 0)
                {
                    line += $"\tアイテム {items}";
                }
                sb.Append(line).Append('\n');
            }
            var enemies = src.UList.Items
                .Where(u => !IsListed(u.Party0))
                .GroupBy(u => u.Party0 + " " + u.Status)
                .OrderBy(g => g.Key, StringComparer.Ordinal)
                .Select(g => $"{g.Key} {g.Count()}");
            sb.Append("ほかの陣営: ").Append(string.Join("、", enemies)).Append('\n');

            sb.Append("### パイロット\n");
            var pilotList = src.PList.Items
                .Where(p => IsListed(p.Party))
                .OrderBy(p => p.ID, StringComparer.Ordinal);
            foreach (var p in pilotList)
            {
                var unit = p.Unit;
                sb.Append($"{p.ID}\t{p.Name}\tレベル{p.Level}\t経験値{p.Exp}\t気力{p.Morale}\tSP {p.SP}/{p.MaxSP}\t{(p.Alive ? "生存" : "死亡")}{(p.Away ? "\t離脱" : "")}\t{(unit == null ? "未搭乗" : unit.ID)}\n");
            }

            sb.Append("### 手持ちのアイテム\n");
            var stock = src.IList.List
                .Where(i => i.Exist && i.Unit == null)
                .Select(i => i.Name)
                .OrderBy(x => x, StringComparer.Ordinal);
            sb.Append(string.Join("・", stock)).Append('\n');

            sb.Append("### グローバル変数\n");
            foreach (var v in src.Event.GlobalVariableList.List.OrderBy(v => v.Name, StringComparer.Ordinal))
            {
                var value = v.VariableType == SRCCore.Expressions.ValueType.StringType
                    ? "\"" + OneLine(v.StringValue) + "\""
                    : Num(v.NumericValue);
                sb.Append(v.Name).Append(" = ").Append(value).Append('\n');
            }
            return sb.ToString();
        }
    }
}
