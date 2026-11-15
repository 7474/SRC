using SRCAuthoring.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace SRCAuthoring.Templates
{
    /// <summary>かんたん作成で置くユニット（ユニット・パイロット・レベル・位置）</summary>
    public sealed class QuickUnit
    {
        public string Unit { get; set; }
        public string Pilot { get; set; }
        public int Level { get; set; } = 1;
        /// <summary>マップの位置（1 から）</summary>
        public int X { get; set; }
        public int Y { get; set; }
    }

    /// <summary>セリフの 1 行。Speaker が空ならナレーション（名前を出さない）</summary>
    public sealed class QuickLine
    {
        public string Speaker { get; set; }
        public string Text { get; set; }
    }

    /// <summary>かんたん作成の場面（誰が・どこで・何を言うか）</summary>
    public sealed class QuickBattle
    {
        /// <summary>シナリオの名前（ファイル名にも使う）</summary>
        public string Title { get; set; }
        /// <summary>読み込む作品のデータ（シナリオの Data の下のフォルダの名前。イベントの「@作品」）</summary>
        public List<string> DataTitles { get; set; } = new List<string>();
        public int Width { get; set; } = 20;
        public int Height { get; set; } = 15;
        /// <summary>マップ（.map のテキスト）。空なら Width×Height の平地</summary>
        public string Map { get; set; }
        public List<QuickUnit> Allies { get; set; } = new List<QuickUnit>();
        public List<QuickUnit> Enemies { get; set; } = new List<QuickUnit>();
        /// <summary>戦闘前の会話（マップを出してユニットを置いた後）</summary>
        public List<QuickLine> Opening { get; set; } = new List<QuickLine>();
        /// <summary>勝ったとき（敵の全滅）</summary>
        public List<QuickLine> Victory { get; set; } = new List<QuickLine>();
        /// <summary>負けたとき（味方の全滅）</summary>
        public List<QuickLine> Defeat { get; set; } = new List<QuickLine>();
    }

    /// <summary>
    /// かんたん作成（7474/SRC#1659）。ブラウザの SRC#Studio で選んだ作品・ユニット・配置・セリフから、
    /// 普通のイベント（.eve）とマップ（.map）を作る。データ（作品のフォルダ）は画面の側がシナリオの Data の下に置く。
    /// 作ったものは VS Code でそのまま開いて続きを書ける。
    /// </summary>
    public static class QuickBattleTemplate
    {
        public const int MaxSize = 50;

        /// <summary>
        /// 画面（JavaScript）から渡す JSON を読む。名前は camelCase（title・dataTitles・width・height・map・allies・enemies・opening・victory・defeat、
        /// ユニットは unit・pilot・level・x・y、セリフは speaker・text）。WebAssembly の切り詰めでも動くよう、リフレクションを使わずに読む
        /// </summary>
        public static QuickBattle FromJson(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var r = doc.RootElement;
            var b = new QuickBattle
            {
                Title = Str(r, "title"),
                Width = Int(r, "width", 20),
                Height = Int(r, "height", 15),
                Map = Str(r, "map"),
            };
            if (r.TryGetProperty("dataTitles", out var titles) && titles.ValueKind == JsonValueKind.Array)
            {
                b.DataTitles = titles.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.String).Select(x => x.GetString()).ToList();
            }
            b.Allies = Units(r, "allies");
            b.Enemies = Units(r, "enemies");
            b.Opening = Talks(r, "opening");
            b.Victory = Talks(r, "victory");
            b.Defeat = Talks(r, "defeat");
            return b;
        }

        private static string Str(JsonElement e, string name) =>
            e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;

        private static int Int(JsonElement e, string name, int fallback) =>
            e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Number && v.TryGetInt32(out var n) ? n : fallback;

        private static List<QuickUnit> Units(JsonElement e, string name) =>
            e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Array
                ? v.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.Object).Select(x => new QuickUnit
                {
                    Unit = Str(x, "unit"),
                    Pilot = Str(x, "pilot"),
                    Level = Int(x, "level", 1),
                    X = Int(x, "x", 0),
                    Y = Int(x, "y", 0),
                }).ToList()
                : new List<QuickUnit>();

        private static List<QuickLine> Talks(JsonElement e, string name) =>
            e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Array
                ? v.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.Object).Select(x => new QuickLine
                {
                    Speaker = Str(x, "speaker"),
                    Text = Str(x, "text"),
                }).ToList()
                : new List<QuickLine>();

        /// <summary>ファイル名に使えない文字を除いたシナリオの名前（空なら「かんたん作成」）</summary>
        public static string FileName(QuickBattle b)
        {
            var invalid = "\\/:*?\"<>|".ToCharArray();
            var name = new string((b.Title ?? "").Where(c => !invalid.Contains(c) && !char.IsControl(c)).ToArray()).Trim().TrimEnd('.');
            return string.IsNullOrEmpty(name) ? "かんたん作成" : name;
        }

        /// <summary>場面が揃っているか。足りなければ理由（日本語）の一覧</summary>
        public static IReadOnlyList<string> Validate(QuickBattle b)
        {
            var errors = new List<string>();
            if (b.Width < 1 || b.Height < 1 || b.Width > MaxSize || b.Height > MaxSize)
            {
                errors.Add($"マップの大きさは 1〜{MaxSize} にしてください");
            }
            if (b.Allies.Count == 0)
            {
                errors.Add("味方のユニットがありません");
            }
            if (b.Enemies.Count == 0)
            {
                errors.Add("敵のユニットがありません");
            }
            var places = new HashSet<(int, int)>();
            foreach (var (u, side) in b.Allies.Select(x => (x, "味方")).Concat(b.Enemies.Select(x => (x, "敵"))))
            {
                if (string.IsNullOrWhiteSpace(u.Unit) || string.IsNullOrWhiteSpace(u.Pilot))
                {
                    errors.Add($"{side}のユニットかパイロットが選ばれていません");
                    continue;
                }
                if (u.X < 1 || u.Y < 1 || u.X > b.Width || u.Y > b.Height)
                {
                    errors.Add($"{side}の「{u.Unit}」がマップの外にあります");
                }
                else if (!places.Add((u.X, u.Y)))
                {
                    errors.Add($"{side}の「{u.Unit}」がほかのユニットと同じ場所にあります");
                }
            }
            var speakers = new HashSet<string>(b.Allies.Concat(b.Enemies).Select(x => x.Pilot ?? ""));
            foreach (var line in b.Opening.Concat(b.Victory).Concat(b.Defeat))
            {
                if (!string.IsNullOrEmpty(line.Speaker) && line.Speaker != "システム" && !speakers.Contains(line.Speaker))
                {
                    errors.Add($"セリフの「{line.Speaker}」は、置いたユニットのパイロットではありません");
                }
            }
            return errors.Distinct().ToList();
        }

        /// <summary>作るファイル（シナリオのフォルダからの相対パスと中身）。イベントは「名前.eve」、マップは「名前.map」</summary>
        public static IReadOnlyDictionary<string, string> Files(QuickBattle b)
        {
            var name = FileName(b);
            var lines = new List<string>
            {
                $"# {name}",
                "# SRC#Studio の「かんたん作成」で作ったシナリオ。このまま VS Code で開いて続きを書ける（ヘルプの「SRC#でシナリオ・データを作る」）",
                "",
                "# 読み込む作品のデータ（Data の下のフォルダ）",
            };
            lines.AddRange(b.DataTitles.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().Select(x => "@" + x.Trim()));
            lines.Add("");
            lines.Add("# マップを出し、ユニットを置いて、戦闘前の会話");
            lines.Add("スタート:");
            lines.Add($"ChangeMap {name}.map");
            lines.AddRange(b.Allies.Select(u => Create("味方", u)));
            lines.AddRange(b.Enemies.Select(u => Create("敵", u)));
            lines.AddRange(Talk(b.Opening));
            lines.Add("Exit");
            lines.Add("");
            lines.Add("# 敵が全滅したら勝ち");
            lines.Add("全滅 敵:");
            lines.AddRange(Talk(b.Victory.Count > 0 ? b.Victory : new List<QuickLine> { new QuickLine { Speaker = "システム", Text = "勝利！" } }));
            lines.Add("GameClear");
            lines.Add("");
            lines.Add("# 味方が全滅したら負け");
            lines.Add("全滅 味方:");
            lines.AddRange(Talk(b.Defeat));
            lines.Add("GameOver");

            return new Dictionary<string, string>
            {
                [name + ".eve"] = string.Join("\r\n", lines) + "\r\n",
                [name + ".map"] = string.IsNullOrWhiteSpace(b.Map) ? new MapDocument(b.Width, b.Height, new MapTile(0, 0)).Serialize() : b.Map,
            };
        }

        private static string Create(string side, QuickUnit u) =>
            $"Create {side} {Quote(u.Unit)} 0 {Quote(u.Pilot)} {Math.Max(1, Math.Min(99, u.Level))} {u.X} {u.Y}";

        /// <summary>名前を引数に書く。空白・カンマ・引用符を含む名前は "…" で囲む（"" は SRC の引数で書けないので除く）</summary>
        private static string Quote(string s)
        {
            s = (s ?? "").Trim().Replace("\"", "");
            return s.Any(c => char.IsWhiteSpace(c) || c == ',' || c == '(' || c == ')') ? "\"" + s + "\"" : s;
        }

        /// <summary>セリフを Talk の塊にする。話し手が変わるたびに Talk を書く。空なら何も書かない</summary>
        private static IEnumerable<string> Talk(IEnumerable<QuickLine> talk)
        {
            var result = new List<string>();
            string current = null;
            foreach (var line in talk)
            {
                var text = MessageLines(line.Text).ToList();
                if (text.Count == 0)
                {
                    continue;
                }
                var speaker = (line.Speaker ?? "").Trim();
                if (speaker != current)
                {
                    result.Add(speaker.Length == 0 ? "Talk" : "Talk " + Quote(speaker));
                    current = speaker;
                }
                result.AddRange(text);
            }
            if (result.Count > 0)
            {
                result.Add("End");
            }
            return result;
        }

        private static readonly string[] Keywords = { "end", "suspend", "talk", "autotalk", "exit" };

        /// <summary>
        /// セリフの文。空行を除き、イベントの命令や注釈に読まれる行（End・Talk などで始まる行・# や ' で始まる行）は、頭に全角の空白を足して文として読ませる
        /// </summary>
        private static IEnumerable<string> MessageLines(string text)
        {
            foreach (var raw in (text ?? "").Replace("\r\n", "\n").Split('\n'))
            {
                var line = raw.TrimEnd();
                if (line.Trim().Length == 0)
                {
                    continue;
                }
                var first = line.TrimStart().Split(' ', '\t')[0].ToLowerInvariant();
                if (Keywords.Contains(first) || line.TrimStart().StartsWith("#", StringComparison.Ordinal) || line.TrimStart().StartsWith("'", StringComparison.Ordinal))
                {
                    line = "　" + line;
                }
                yield return line;
            }
        }
    }
}
