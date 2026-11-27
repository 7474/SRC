using SRCAuthoring.Analysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCAuthoring.Templates
{
    /// <summary>
    /// 途中の話を、手で決めた始めの値から遊ぶ（7474/SRC#1692）。.srcsharp/start/話.eve に置くイベントのひな形を作る。
    /// プロローグで部隊（前の話までに加わった味方の機体とレベル）・資金・話をまたぐ変数を作り、Continue でその話へ進む。
    /// 普通のイベントなので、作者が値を書き換えて使う（VS Code の「この話から遊ぶ」が、通しのセーブデータが無いときに使う）
    /// </summary>
    public static class StartStateTemplate
    {
        public const string Folder = ".srcsharp/start";

        /// <summary>episode（シナリオのフォルダからの相対パス）を始めるイベント。話が見つからなければ null</summary>
        public static string Build(ScenarioOutline outline, string episode)
        {
            static string Norm(string p) => (p ?? "").Replace('\\', '/');
            var index = outline.Episodes.FindIndex(e => string.Equals(Norm(e.File), Norm(episode), StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                return null;
            }
            var title = outline.Episodes[index].Title;
            // 前の話までに加わって、まだいる味方（加入順）
            var party = new List<string>();
            var units = new Dictionary<string, (string unit, int level)>(StringComparer.Ordinal);
            foreach (var e in outline.Episodes.Take(index))
            {
                foreach (var p in e.Joins)
                {
                    if (!party.Contains(p))
                    {
                        party.Add(p);
                    }
                    if (e.JoinUnits.TryGetValue(p, out var u))
                    {
                        units[p] = u;
                    }
                }
                party.RemoveAll(e.Leaves.Contains);
            }
            // シナリオ全体の設定: はじめの話の @ の行（データのフォルダ）と、前の話の Option・Load・RenameTerm・RenameBGM・IntermissionCommand
            var heads = new List<string>();
            var setup = new List<string>();
            var setupCommands = new[] { "option", "load", "renameterm", "renamebgm", "intermissioncommand" };
            foreach (var e in outline.Episodes.Take(index))
            {
                var path = Path.Combine(outline.ScenarioRoot ?? "", e.File ?? "");
                if (!File.Exists(path))
                {
                    continue;
                }
                var first = true;
                foreach (var raw in File.ReadAllLines(path))
                {
                    var t = raw.Trim();
                    if (first && t.StartsWith("@", StringComparison.Ordinal))
                    {
                        if (!heads.Contains(t))
                        {
                            heads.Add(t);
                        }
                        continue;
                    }
                    first = false;
                    var command = t.Split(' ', '\t')[0].ToLowerInvariant();
                    if (setupCommands.Contains(command) && !setup.Contains(t))
                    {
                        setup.Add(t);
                    }
                }
            }
            var lines = new List<string>(heads)
            {
                $"# {title}を、手で決めた始めの値から遊ぶ（VS Code の「SRC#: この話から遊ぶ」が使う。srcsharp start-state で作った）",
                "# 部隊・資金・話をまたぐ変数を書き換えてから、もう一度「この話から遊ぶ」を選ぶ。書き方は普通のイベントと同じ",
                $"# 最後の Continue で、インターミッション（「次のステージへ」）を通って{title}が始まる",
                "",
                "プロローグ:",
            };
            if (setup.Count > 0)
            {
                lines.Add("# シナリオ全体の設定（前の話から）");
                lines.AddRange(setup);
                lines.Add("");
            }
            lines.Add("# 部隊（前の話までに加わって、離れていない味方。機体とレベルは加入したときのもの）");
            if (party.Count == 0)
            {
                lines.Add("# （前の話で加わった味方はいない）");
            }
            foreach (var p in party)
            {
                if (units.TryGetValue(p, out var u) && !string.IsNullOrEmpty(u.unit))
                {
                    lines.Add($"Unit {Quote(u.unit)} 0");
                    lines.Add($"Pilot {Quote(p)} {Math.Max(1, u.level)}");
                    lines.Add($"Ride {Quote(p)}");
                }
                else
                {
                    lines.Add($"# {p}（Join などで加わった。機体を決めて Unit・Pilot・Ride を書く）");
                }
            }
            lines.Add("");
            lines.Add("# 資金");
            lines.Add("Money 0");
            var globals = outline.Variables.Where(v => v.Global && v.Sets.Any(s => outline.Episodes.Take(index).Any(e => Norm(e.File) == Norm(s.file)))).ToList();
            if (globals.Count > 0)
            {
                lines.Add("");
                lines.Add("# 話をまたぐ変数（前の話で設定するもの。値を決める）");
                foreach (var v in globals)
                {
                    var where = string.Join("・", v.Sets.Select(s => $"{s.file} {s.line} 行").Take(3));
                    lines.Add($"# {v.Name}: {where}で設定");
                    lines.Add($"Global {Quote(v.Name)}");
                    lines.Add($"Set {Quote(v.Name)} 0");
                }
            }
            lines.Add("");
            lines.Add($"Continue {episode.Replace('/', '\\')}");
            return string.Join("\r\n", lines) + "\r\n";
        }

        private static string Quote(string s) => s.Any(c => char.IsWhiteSpace(c) || c == ',') ? "\"" + s + "\"" : s;
    }
}
