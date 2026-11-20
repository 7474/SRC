using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace SRCAuthoring.Templates
{
    /// <summary>かんたん作成のキャンペーン（話を並べたもの。7474/SRC#1691）</summary>
    public sealed class QuickCampaign
    {
        public string Title { get; set; }
        public List<string> DataTitles { get; set; } = new List<string>();
        /// <summary>話（1 つ目が第1話）。それぞれの題名・作品は使わない（キャンペーンのもの）</summary>
        public List<QuickBattle> Episodes { get; set; } = new List<QuickBattle>();
    }

    /// <summary>
    /// かんたん作成で話を足したシナリオ（7474/SRC#1691）。srcsharp new --episodes と同じ形で出す:
    /// 第N話.eve・第N話.map・第N話.play（単体）・全話.play（通し）・.vscode/launch.json・.github/workflows/src-check.yml・README.md・.gitignore。
    /// 勝ったら次の話へ Continue（最後の話は GameClear）。前の話に出た味方は Launch で続けて出す（機体が変われば乗り換える）。
    /// 各話は、前の話のデータが無くても（単体でも）始められる
    /// </summary>
    public static class QuickCampaignTemplate
    {
        public const int MaxEpisodes = 20;

        public static string EpisodeName(int index) => $"第{index + 1}話";

        /// <summary>{ title, dataTitles, episodes: [かんたん作成の場面の JSON（title・dataTitles は除く）] }</summary>
        public static QuickCampaign FromJson(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var r = doc.RootElement;
            var c = new QuickCampaign
            {
                Title = r.TryGetProperty("title", out var t) && t.ValueKind == JsonValueKind.String ? t.GetString() : null,
            };
            if (r.TryGetProperty("dataTitles", out var titles) && titles.ValueKind == JsonValueKind.Array)
            {
                c.DataTitles = titles.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.String).Select(x => x.GetString()).ToList();
            }
            if (r.TryGetProperty("episodes", out var episodes) && episodes.ValueKind == JsonValueKind.Array)
            {
                foreach (var e in episodes.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.Object))
                {
                    var b = QuickBattleTemplate.FromJson(e.GetRawText());
                    b.Title = c.Title;
                    b.DataTitles = c.DataTitles;
                    c.Episodes.Add(b);
                }
            }
            return c;
        }

        public static IReadOnlyList<string> Validate(QuickCampaign c)
        {
            var errors = new List<string>();
            if (c.Episodes.Count == 0)
            {
                errors.Add("話がありません");
            }
            if (c.Episodes.Count > MaxEpisodes)
            {
                errors.Add($"話は {MaxEpisodes} までにしてください");
            }
            for (var i = 0; i < c.Episodes.Count; i++)
            {
                errors.AddRange(QuickBattleTemplate.Validate(c.Episodes[i]).Select(e => $"{EpisodeName(i)}: {e}"));
            }
            return errors;
        }

        /// <summary>前の話までに出た味方（パイロット → 最後に乗った機体）</summary>
        private static Dictionary<string, string> Carried(QuickCampaign c, int index)
        {
            var carried = new Dictionary<string, string>(StringComparer.Ordinal);
            for (var i = 0; i < index; i++)
            {
                foreach (var u in c.Episodes[i].Allies.Concat(c.Episodes[i].Events.SelectMany(e => e.Allies)).Where(u => QuickBattleTemplate.IsNamed(u.Pilot)))
                {
                    carried[u.Pilot] = u.Unit;
                }
            }
            return carried;
        }

        public static IReadOnlyDictionary<string, string> Files(QuickCampaign c)
        {
            if (c.Episodes.Count == 1)
            {
                return QuickBattleTemplate.Files(c.Episodes[0]);
            }
            var name = QuickBattleTemplate.FileName(new QuickBattle { Title = c.Title });
            var names = Enumerable.Range(0, c.Episodes.Count).Select(EpisodeName).ToList();
            var files = new Dictionary<string, string>();
            for (var i = 0; i < c.Episodes.Count; i++)
            {
                var last = i == c.Episodes.Count - 1;
                var b = c.Episodes[i];
                files[names[i] + ".eve"] = QuickBattleTemplate.EpisodeEvent(b, name, names[i], last ? "GameClear" : $"Continue {names[i + 1]}.eve", Carried(c, i));
                files[names[i] + ".map"] = QuickBattleTemplate.MapText(b);
                files[names[i] + ".play"] = ScenarioTemplate.Lines(
                    $"# {names[i]} を単体で遊ぶ手順（前の話のデータが無いので、続けて出る味方はその場で作る）",
                    $"#   srcsharp play {names[i]}.eve --script {names[i]}.play --app-path <SRC のフォルダか最小の汎用データ> --skip-wait",
                    "# 30 ターンまで遊び、イベントのエラーが無いことを確かめる。勝ったら次の話の前のインターミッションで止まる",
                    "holdlist コマンド",
                    "auto 400",
                    "turns 30",
                    "expect error none");
            }
            files["全話.play"] = ScenarioTemplate.Lines(
                $"# 第1話から{names[^1]}まで通しで遊ぶ手順（インターミッションでは「次のステージへ」を選ぶ）",
                $"#   srcsharp play 第1話.eve --script 全話.play --app-path <SRC のフォルダか最小の汎用データ> --skip-wait",
                "# 勝ち負けは乱数で変わるので確かめない。各話の強さは srcsharp playtest で見る",
                "choose コマンド 次のステージへ",
                "auto 400",
                $"turns {30 * c.Episodes.Count}",
                "expect error none");
            files["README.md"] = Readme(name, names);
            files[".vscode/launch.json"] = ScenarioTemplate.MultiLaunch(names);
            files[".github/workflows/src-check.yml"] = ScenarioTemplate.MultiWorkflow(names);
            files[".gitignore"] = QuickBattleTemplate.Gitignore;
            return files;
        }

        private static string Readme(string name, IReadOnlyList<string> names)
        {
            var lines = new List<string>
            {
                $"# {name}",
                "",
                $"SRC#Studio の「かんたん作成」で、{names.Count} 話を並べて作ったシナリオです。中身は `srcsharp new --episodes` のひな形と同じ形の、普通の SRC のシナリオです。",
                "",
                "| ファイル | 内容 |",
                "|---|---|",
            };
            foreach (var n in names)
            {
                lines.Add($"| `{n}.eve`・`{n}.map`・`{n}.play` | {n}のイベント・マップ・単体で遊ぶ手順 |");
            }
            lines.Add("| `全話.play` | 第1話から通しで遊ぶ手順 |");
            lines.Add("| `Data/作品/…` | 選んだ作品のデータ（GSC データパック） |");
            lines.Add("| `.vscode/launch.json`・`.github/workflows/src-check.yml` | VS Code で F5、GitHub で push のたびに検査と自動プレイ |");
            lines.Add("");
            lines.Add("## 話のつなぎ方");
            lines.Add("");
            lines.Add("- 勝つと `Continue 次の話.eve` でインターミッションに入り、「次のステージへ」で次の話に進む。最後の話は `GameClear`");
            lines.Add("- 前の話に出た味方は、次の話では `Launch` で続けて出す（機体を変えたら `Unit`・`Ride` で乗り換える）。初めて出る味方は `Create`（加入）。前の話に出て、この話で置かなかった味方は、部隊に残ったまま出撃しない");
            lines.Add("- 各話は単体でも始められる（前の話のデータが無ければ、続けて出る味方をその場で作る）");
            lines.Add("");
            lines.Add("## 次にやること");
            lines.Add("");
            lines.Add("1. **VS Code で開く**: 拡張「SRC# Authoring」を入れ、このフォルダを開く");
            lines.Add("2. **遊ぶ**: `第1話.eve` を開いて F5。各話を開いて F5 すれば、その話から遊べる");
            lines.Add("3. **話のつながりを見る**: 「SRC#: 話のつながりの図を表示」");
            lines.Add("4. **分岐・フラグ・加入と離脱を書く**: 選択肢で話を分ける、話をまたぐ変数（`Global`）などは VS Code で書く（かんたん作成は話を並べるところまで。VS Code で直したものをかんたん作成に戻すことはできない）");
            lines.Add("5. **強さを確かめる**: 各話を開いて「SRC#: この話の強さを確かめる」（`srcsharp playtest`）");
            lines.Add("6. **公開する**: SRC#Web で遊べるように置く・zip にして配る");
            return ScenarioTemplate.Lines(lines.ToArray());
        }
    }
}
