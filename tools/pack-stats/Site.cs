// GitHub Pages（Jekyll。テーマは Just the Docs）で見るための、作品ごとのデータと評価のページを作る（site）
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace PackStats
{
    public static partial class Program
    {
        private static bool IsTitleDir(string dir) =>
            Directory.EnumerateFiles(dir).Select(f => Path.GetFileName(f).ToLowerInvariant())
                .Any(n => n == "pilot.txt" || n == "unit.txt" || n == "robot.txt");

        private static string Esc(string s) => Uri.EscapeDataString(s);

        private static readonly JsonSerializerOptions YamlString = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

        // YAML の二重引用符の文字列（JSON の文字列は YAML の二重引用符の文字列として読める）
        private static string Q(string s) => JsonSerializer.Serialize(s ?? "", YamlString);

        private static string FrontMatter(string title, params (string key, string value)[] extra)
        {
            var sb = new StringBuilder();
            sb.Append("---\n");
            sb.Append($"title: {Q(title)}\n");
            foreach (var (k, v) in extra) sb.Append($"{k}: {v}\n");
            sb.Append("---\n\n");
            return sb.ToString();
        }

        // Liquid の記法に見える文字列をそのまま出す
        private static string Raw(string body) => "{% raw %}\n" + body.Replace("{% endraw %}", "{ % endraw % }") + "\n{% endraw %}\n";

        /// <summary>組の評価（tools/pack-analysis の出力）を、作品ごとの節と、それ以外（前提・格ごとの指数）に分ける</summary>
        private static (string head, Dictionary<string, string> works, string tail) SplitAnalysis(string path)
        {
            var works = new Dictionary<string, string>();
            var head = new StringBuilder();
            var tail = new StringBuilder();
            string cur = null;
            var body = new StringBuilder();
            var inTail = false;
            void Flush()
            {
                if (cur != null) works[cur] = body.ToString();
                cur = null;
                body.Clear();
            }
            foreach (var line in File.ReadLines(path))
            {
                var m = Regex.Match(line, @"^### (.+)$");
                if (m.Success)
                {
                    Flush();
                    if (m.Groups[1].Value == "格ごとの指数") inTail = true;
                    if (!inTail)
                    {
                        cur = m.Groups[1].Value.Trim();
                        continue;
                    }
                }
                if (inTail) tail.AppendLine(line);
                else if (cur != null) body.AppendLine(line);
                else head.AppendLine(line);
            }
            Flush();
            return (head.ToString(), works, tail.ToString());
        }

        /// <summary>
        /// site &lt;出力フォルダ&gt; &lt;GSCデータパックのフォルダ&gt; [作品のルート …] [--analysis 組の評価.md …]
        /// パックの作品はそのパックと、作品のルート（Sharp/Data など）の作品は、robot.txt があればロボ、無ければ拡張と比べる。
        /// 組の評価を渡すと、作品ごとのページに組の評価（指数・レーダーチャート・総評）を載せ、前提と格ごとの指数を evaluation/ に出す
        /// </summary>
        private static void Site(string outDir, string gscRoot, IEnumerable<string> rest)
        {
            var args = rest.ToList();
            var ai = args.IndexOf("--analysis");
            var extraRoots = ai < 0 ? args : args.Take(ai).ToList();
            var analysisFiles = ai < 0 ? new List<string>() : args.Skip(ai + 1).ToList();

            Directory.CreateDirectory(outDir);
            var packs = new Dictionary<string, Data>();
            foreach (var name in new[] { "ロボ", "拡張" })
            {
                var dir = Path.Combine(gscRoot, name);
                if (!Directory.Exists(dir)) continue;
                Console.Error.WriteLine($"読み込み: {dir}");
                packs[name] = Data.Load(dir);
            }

            // 組の評価: 「ファイル名（拡張子なし）」ごとに、作品の節を集める
            var analysis = new Dictionary<string, Dictionary<string, string>>();
            var evalDir = Path.Combine(outDir, "evaluation");
            var evalPages = new List<string>();
            foreach (var f in analysisFiles)
            {
                var label = Path.GetFileNameWithoutExtension(f);
                var (head, works, tail) = SplitAnalysis(f);
                analysis[label] = works;
                Directory.CreateDirectory(evalDir);
                var page = new StringBuilder();
                page.AppendLine("[トップ](../)");
                page.AppendLine();
                page.AppendLine(head.TrimEnd());
                page.AppendLine();
                page.AppendLine("作品ごとの評価（使いどころごとの評価、指数、組ごとのレーダーチャートと総評）は、各作品のページにある。");
                page.AppendLine();
                page.Append(tail);
                File.WriteAllText(Path.Combine(evalDir, label + ".md"),
                    FrontMatter($"組の評価: {label}", ("parent", Q("組の評価")), ("nav_order", (evalPages.Count + 1).ToString())) + Raw(page.ToString()));
                evalPages.Add(label);
            }
            if (evalPages.Any())
            {
                var sb0 = new StringBuilder();
                sb0.AppendLine("[トップ](../)");
                sb0.AppendLine();
                sb0.AppendLine("SRC の [tools/pack-analysis](https://github.com/7474/SRC/tree/master/tools/pack-analysis) で、ユニットとパイロットの組を、ゲームと同じ計算で使いどころごとに評価した結果。作品ごとの評価は各作品のページにある。");
                sb0.AppendLine();
                foreach (var n in evalPages) sb0.AppendLine($"- [{n}]({Esc(n)}.html)");
                File.WriteAllText(Path.Combine(evalDir, "index.md"), FrontMatter("組の評価", ("nav_order", "3"), ("has_children", "true")) + sb0.ToString());
            }

            var sections = new List<(string name, List<(string title, string pack, (int pilots, int units, int must, int info) c)> rows)>();
            var order = 10;
            foreach (var (name, pack) in packs)
            {
                sections.Add((name, WriteTitles(outDir, name, Directory.EnumerateDirectories(pack.Root), _ => name, packs,
                    analysis.GetValueOrDefault(name), order++)));
            }
            foreach (var root in extraRoots)
            {
                var full = Path.GetFullPath(root).TrimEnd('/', '\\');
                var label = Path.GetFileName(full);
                // Sharp/Data は「Sharp」と呼ぶ
                if (label == "Data") label = Path.GetFileName(Path.GetDirectoryName(full));
                sections.Add((label, WriteTitles(outDir, label, Directory.EnumerateDirectories(root),
                    d => Directory.EnumerateFiles(d).Any(f => Path.GetFileName(f).ToLowerInvariant() == "robot.txt") ? "ロボ" : "拡張", packs,
                    analysis.GetValueOrDefault(label), order++)));
            }

            // トップページ
            var sb = new StringBuilder();
            sb.AppendLine("SRC 向けのデータを、作品ごとに閲覧し、GSC データパックの実態と比べた評価を見るためのページ。上の検索欄で、作品・パイロット・ユニット・武器の名前を検索できる（名前の先頭から一致）。");
            sb.AppendLine();
            sb.AppendLine("| 区分 | 作品 | 内容 |");
            sb.AppendLine("|---|--:|---|");
            foreach (var (name, rows) in sections)
            {
                sb.AppendLine($"| [{name}]({Esc(name)}/) | {rows.Count} | 作品ごとのデータ（パイロット・SP・ユニット・武器・マップ兵器・メッセージ）と評価{(analysis.ContainsKey(name) ? "、組の評価とレーダーチャート" : "")} |");
            }
            sb.AppendLine();
            if (File.Exists(Path.Combine(outDir, "guideline.md"))) sb.AppendLine("- [データ作成ガイドライン](guideline.html)");
            if (evalPages.Any()) sb.AppendLine("- [組の評価](evaluation/)（前提と、格ごとの指数）");
            sb.AppendLine();
            sb.AppendLine("評価は SRC の [tools/pack-stats](https://github.com/7474/SRC/tree/master/tools/pack-stats)（作品の評価）と [tools/pack-analysis](https://github.com/7474/SRC/tree/master/tools/pack-analysis)（組の評価）で作っている。");
            File.WriteAllText(Path.Combine(outDir, "index.md"), FrontMatter("SRC Data", ("nav_order", "1"), ("permalink", "/")) + sb.ToString());
        }

        private static List<(string title, string pack, (int pilots, int units, int must, int info) c)> WriteTitles(
            string outDir, string section, IEnumerable<string> dirs, Func<string, string> packOf, Dictionary<string, Data> packs,
            Dictionary<string, string> analysis, int navOrder)
        {
            var rows = new List<(string title, string pack, (int pilots, int units, int must, int info) c)>();
            var sectionDir = Path.Combine(outDir, section);
            Directory.CreateDirectory(sectionDir);
            foreach (var d in dirs.OrderBy(x => x, StringComparer.Ordinal))
            {
                if (!IsTitleDir(d)) continue;
                var title = Path.GetFileName(d);
                var packName = packOf(d);
                if (!packs.TryGetValue(packName, out var pack)) continue;
                var sw = new StringWriter();
                var old = Console.Out;
                try
                {
                    Console.SetOut(sw);
                    Check(pack, d);
                }
                catch (Exception ex)
                {
                    Console.SetOut(old);
                    Console.Error.WriteLine($"評価できない: {d}: {ex.Message}");
                    continue;
                }
                finally
                {
                    Console.SetOut(old);
                }
                var body = sw.ToString();
                if (analysis != null && analysis.TryGetValue(title, out var a))
                {
                    // 作品の評価の見出しの直後に、組の評価を入れる（データページの見出しは h4 のまま）
                    var block = "## 組の評価\n\n使いどころごとの評価と指数（同じ帯の中央値 = 100。実効性能／単純数字）、組ごとのレーダーチャートと総評。計算は [tools/pack-analysis](https://github.com/7474/SRC/tree/master/tools/pack-analysis)。\n\n" + a.Trim() + "\n\n";
                    var at = body.IndexOf("\n## パイロット", StringComparison.Ordinal);
                    body = at < 0 ? body + "\n" + block : body.Substring(0, at + 1) + block + body.Substring(at + 1);
                }
                File.WriteAllText(Path.Combine(sectionDir, title + ".md"),
                    FrontMatter(title, ("nav_exclude", "true"), ("search_terms", Q(lastTerms))) + Raw($"[{section}](./)・[トップ](../)\n\n" + body));
                rows.Add((title, packName, lastCheck));
            }
            var sb = new StringBuilder();
            sb.AppendLine("[トップ](../)");
            sb.AppendLine();
            sb.AppendLine("| 作品 | 基準 | パイロット | ユニット | 要確認 | 参考 |");
            sb.AppendLine("|---|---|--:|--:|--:|--:|");
            foreach (var (title, packName, c) in rows)
            {
                sb.AppendLine($"| [{title.Replace("|", "\\|")}]({Esc(title)}.html) | {packName} | {c.pilots} | {c.units} | {c.must} | {c.info} |");
            }
            File.WriteAllText(Path.Combine(sectionDir, "index.md"),
                FrontMatter(section, ("nav_order", navOrder.ToString()), ("search_terms", Q(section))) + Raw(sb.ToString()));
            Console.Error.WriteLine($"{section}: {rows.Count} 作品");
            return rows;
        }
    }
}
