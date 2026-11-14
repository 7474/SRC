using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SRCAuthoring.Lsp
{
    /// <summary>
    /// 同梱のヘルプ（SRC.Sharp.Help の Markdown）から、エディタのホバー・補完に出す説明を引く（7474/SRC#1642）。
    /// - イベントコマンド: 「Xコマンド」の頁の 1 行目の説明と書式
    /// - 特殊能力・武器の属性: 「…特殊能力…」「…属性…」の頁の、「**名前**」で始まる節
    /// ヘルプの本体は srcsharp（SRCSharpTools）に埋め込まれているので、頁（名前・本文）を渡して作る
    /// </summary>
    public sealed class HelpReference
    {
        private readonly Dictionary<string, string> commands = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> keywords = new Dictionary<string, string>(StringComparer.Ordinal);

        public static readonly HelpReference Empty = new HelpReference(Array.Empty<(string, string)>());

        /// <summary>特殊能力・属性の名前（補完に出す）</summary>
        public IReadOnlyCollection<string> Keywords => keywords.Keys;

        private static readonly Regex Link = new Regex(@"\[([^\]]*)\]\([^)]*\)");
        private static readonly Regex Bold = new Regex(@"^\*\*([^*]{1,20})\*\*(.*)$");
        private static readonly HashSet<string> NotKeywords = new HashSet<string> { "書式", "例", "解説", "指定項目説明", "注意", "備考", "元の SRC との違い" };

        public HelpReference(IEnumerable<(string name, string body)> topics)
        {
            foreach (var (name, body) in topics)
            {
                var lines = StripFrontMatter(body).Replace("\r\n", "\n").Split('\n');
                if (name.EndsWith("コマンド", StringComparison.Ordinal))
                {
                    commands[name.Substring(0, name.Length - "コマンド".Length)] = CommandSummary(name, lines);
                }
                if (name.Contains("特殊能力") || name.Contains("属性") || name == "アビリティ効果")
                {
                    IndexKeywords(name, lines);
                }
            }
        }

        private static string StripFrontMatter(string body)
        {
            if (body.StartsWith("---", StringComparison.Ordinal))
            {
                var end = body.IndexOf("\n---", 3, StringComparison.Ordinal);
                if (end > 0)
                {
                    return body.Substring(body.IndexOf('\n', end + 1) + 1);
                }
            }
            return body;
        }

        public static string Clean(string line) => Link.Replace(line, "$1").Trim();

        private static string CommandSummary(string topic, string[] lines)
        {
            // 「**Xコマンド**」の次の空でない行が説明。「**書式**」の次の空でない行が書式
            string description = null;
            string syntax = null;
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (description == null && line == $"**{topic}**")
                {
                    description = NextNonEmpty(lines, i + 1);
                }
                else if (syntax == null && line == "**書式**")
                {
                    syntax = NextNonEmpty(lines, i + 1);
                }
            }
            var parts = new List<string>();
            if (!string.IsNullOrEmpty(syntax))
            {
                parts.Add(Clean(syntax));
            }
            if (!string.IsNullOrEmpty(description))
            {
                parts.Add(Clean(description));
            }
            parts.Add($"（`srcsharp help {topic}`）");
            return string.Join("\n\n", parts);
        }

        private static string NextNonEmpty(string[] lines, int from)
        {
            for (var i = from; i < lines.Length; i++)
            {
                if (lines[i].Trim() != "")
                {
                    return lines[i].Trim();
                }
            }
            return null;
        }

        private void IndexKeywords(string topic, string[] lines)
        {
            var starts = new List<(int line, string name)>();
            for (var i = 0; i < lines.Length; i++)
            {
                var m = Bold.Match(lines[i].Trim());
                if (!m.Success)
                {
                    continue;
                }
                var name = m.Groups[1].Value.Trim();
                if (NotKeywords.Contains(name) || name == topic || name.Contains(' ') || name.StartsWith("(", StringComparison.Ordinal) || name.Contains("内容は"))
                {
                    continue;
                }
                starts.Add((i, name));
            }
            for (var k = 0; k < starts.Count; k++)
            {
                var (start, name) = starts[k];
                var end = k + 1 < starts.Count ? starts[k + 1].line : lines.Length;
                var section = lines.Skip(start).Take(Math.Min(end - start, 14)).Select(Clean).Where(x => x != "");
                var text = string.Join("  \n", section) + $"\n\n（ヘルプ「{topic}」）";
                // 一覧の頁（武器属性）と詳しい頁（ダメージ算出方法に関する属性）の両方にあれば、詳しいほう（長いほう）
                if (!keywords.TryGetValue(name, out var existing) || existing.Length < text.Length)
                {
                    keywords[name] = text;
                }
            }
        }

        /// <summary>イベントコマンドの説明（Markdown）。無ければ null</summary>
        public string Command(string name) => commands.TryGetValue(name ?? "", out var s) ? s : null;

        /// <summary>
        /// 特殊能力・属性の説明（Markdown）。「ＨＰ回復Lv2」「弱点=風」のように、レベル・値を付けて書いた語でも引く。無ければ null
        /// </summary>
        public string Keyword(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return null;
            }
            foreach (var candidate in Candidates(word))
            {
                if (keywords.TryGetValue(candidate, out var s))
                {
                    return s;
                }
            }
            return null;
        }

        private static IEnumerable<string> Candidates(string word)
        {
            yield return word;
            var w = word;
            var eq = w.IndexOf('=');
            if (eq > 0)
            {
                w = w.Substring(0, eq);
                yield return w;
            }
            var lv = Regex.Match(w, @"^(.+?)(?:Lv|ＬＶ|Ｌｖ|L)[-\d.]+$");
            if (lv.Success)
            {
                yield return lv.Groups[1].Value;
            }
        }
    }
}
