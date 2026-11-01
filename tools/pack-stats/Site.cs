// GitHub Pages（Jekyll）で見るための、作品ごとのデータと評価のページを作る（site）
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PackStats
{
    public static partial class Program
    {
        private static bool IsTitleDir(string dir) =>
            Directory.EnumerateFiles(dir).Select(f => Path.GetFileName(f).ToLowerInvariant())
                .Any(n => n == "pilot.txt" || n == "unit.txt" || n == "robot.txt");

        private static string Esc(string s) => Uri.EscapeDataString(s);

        private static string FrontMatter(string title) =>
            $"---\ntitle: \"{title.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"\n---\n\n";

        // Liquid の記法に見える文字列をそのまま出す
        private static string Raw(string body) => "{% raw %}\n" + body.Replace("{% endraw %}", "{ % endraw % }") + "\n{% endraw %}\n";

        /// <summary>
        /// site &lt;出力フォルダ&gt; &lt;GSCデータパックのフォルダ&gt; [作品のルート …]
        /// パックの作品はそのパックと、作品のルート（Sharp/Data など）の作品は、robot.txt があればロボ、無ければ拡張と比べる
        /// </summary>
        private static void Site(string outDir, string gscRoot, IEnumerable<string> extraRoots)
        {
            Directory.CreateDirectory(outDir);
            var packs = new Dictionary<string, Data>();
            foreach (var name in new[] { "ロボ", "拡張" })
            {
                var dir = Path.Combine(gscRoot, name);
                if (!Directory.Exists(dir)) continue;
                Console.Error.WriteLine($"読み込み: {dir}");
                packs[name] = Data.Load(dir);
            }

            var sections = new List<(string name, string dir, List<(string title, string pack, (int pilots, int units, int must, int info) c)> rows)>();
            foreach (var (name, pack) in packs)
            {
                sections.Add((name, name, WriteTitles(outDir, name, pack, Directory.EnumerateDirectories(pack.Root), _ => name, packs)));
            }
            foreach (var root in extraRoots)
            {
                var label = Path.GetFileName(Path.GetFullPath(root).TrimEnd('/', '\\'));
                sections.Add((label, label, WriteTitles(outDir, label, null, Directory.EnumerateDirectories(root),
                    d => Directory.EnumerateFiles(d).Any(f => Path.GetFileName(f).ToLowerInvariant() == "robot.txt") ? "ロボ" : "拡張", packs)));
            }

            // トップページ
            var sb = new StringBuilder();
            sb.Append(FrontMatter("SRC Data"));
            sb.AppendLine("SRC 向けのデータを、作品ごとに閲覧し、GSC データパックの実態と比べた評価を見るためのページ。");
            sb.AppendLine();
            var guide = Path.Combine(outDir, "guideline.md");
            if (File.Exists(guide)) sb.AppendLine("- [データ作成ガイドライン](guideline.html)");
            var evalDir = Path.Combine(outDir, "evaluation");
            if (Directory.Exists(evalDir))
            {
                foreach (var f in Directory.EnumerateFiles(evalDir, "*.md").OrderBy(f => f, StringComparer.Ordinal))
                {
                    var n = Path.GetFileNameWithoutExtension(f);
                    sb.AppendLine($"- [組の評価: {n}](evaluation/{Esc(n)}.html)");
                }
            }
            sb.AppendLine();
            foreach (var (name, dir, rows) in sections)
            {
                sb.AppendLine($"- [{name}]({Esc(dir)}/)（{rows.Count} 作品）");
            }
            sb.AppendLine();
            sb.AppendLine("評価は SRC の [tools/pack-stats](https://github.com/7474/SRC/tree/master/tools/pack-stats)（作品の評価）と [tools/pack-analysis](https://github.com/7474/SRC/tree/master/tools/pack-analysis)（組の評価）で作っている。");
            File.WriteAllText(Path.Combine(outDir, "index.md"), sb.ToString());
        }

        private static List<(string title, string pack, (int pilots, int units, int must, int info) c)> WriteTitles(
            string outDir, string section, Data ownPack, IEnumerable<string> dirs, Func<string, string> packOf, Dictionary<string, Data> packs)
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
                File.WriteAllText(Path.Combine(sectionDir, title + ".md"), FrontMatter(title) + Raw("[一覧](./)・[トップ](../)\n\n" + sw.ToString()));
                rows.Add((title, packName, lastCheck));
            }
            var sb = new StringBuilder();
            sb.Append(FrontMatter(section));
            sb.AppendLine("[トップ](../)");
            sb.AppendLine();
            sb.AppendLine("| 作品 | 基準 | パイロット | ユニット | 要確認 | 参考 |");
            sb.AppendLine("|---|---|--:|--:|--:|--:|");
            foreach (var (title, packName, c) in rows)
            {
                sb.AppendLine($"| [{title.Replace("|", "\\|")}]({Esc(title)}.html) | {packName} | {c.pilots} | {c.units} | {c.must} | {c.info} |");
            }
            // front matter は raw の外に置く
            File.WriteAllText(Path.Combine(sectionDir, "index.md"), FrontMatter(section) + Raw(sb.ToString().Substring(FrontMatter(section).Length)));
            Console.Error.WriteLine($"{section}: {rows.Count} 作品");
            return rows;
        }
    }
}
