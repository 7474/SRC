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
        private readonly Dictionary<string, string> situations = new Dictionary<string, string>(StringComparer.Ordinal);

        public static readonly HelpReference Empty = new HelpReference(Array.Empty<(string, string)>());

        /// <summary>特殊能力・属性の名前（補完に出す）</summary>
        public IReadOnlyCollection<string> Keywords => keywords.Keys;

        /// <summary>メッセージデータのシチュエーション・タグの名前（補完に出す）</summary>
        public IReadOnlyCollection<string> Situations => situations.Keys;

        private static readonly Regex Link = new Regex(@"\[([^\]]*)\]\([^)]*\)");
        private static readonly Regex Bold = new Regex(@"^\*\*([^*]{1,20})\*\*(.*)$");
        private static readonly Regex MessageEntryBold = new Regex(@"^\*\*([^*]+?)\*\*(.*)$");
        private static readonly Regex MessageParenBold = new Regex(@"^\(\*\*([^*]+?)\*\*([^)]*)\)(.*)$");
        private static readonly HashSet<string> NotKeywords = new HashSet<string>
        {
            "書式", "例", "例１", "例２", "例３", "例４", "例５", "例６", "解説", "項目解説", "指定項目説明", "注意", "備考", "元の SRC との違い",
            "メッセージデータ", "ダイアログデータ", "格闘型の武器の場合", "射撃型の武器の場合", "武器名", "アビリティ名",
            "カットイン", "カットインの消去", "カットインの持続", "パイロット画像切り替え", "カットインアニメ", "効果音", "ディレイ", "戦闘アニメ表示",
        };

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
                if (name == "メッセージデータ")
                {
                    IndexSituations(name, lines);
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

        private void IndexSituations(string topic, string[] lines)
        {
            var entries = new List<(int line, string[] keys)>();
            for (var i = 0; i < lines.Length; i++)
            {
                var trimmed = lines[i].Trim();
                if (trimmed.StartsWith("```", StringComparison.Ordinal))
                {
                    i++;
                    while (i < lines.Length && !lines[i].Trim().StartsWith("```", StringComparison.Ordinal))
                    {
                        i++;
                    }
                    continue;
                }
                string[] keys = null;
                var mp = MessageParenBold.Match(trimmed);
                if (mp.Success)
                {
                    var head = mp.Groups[1].Value.Trim();
                    keys = new[] { head };
                }
                else
                {
                    var mb = MessageEntryBold.Match(trimmed);
                    if (mb.Success)
                    {
                        var head = mb.Groups[1].Value.Trim();
                        var parenIdx = head.IndexOf('(');
                        if (parenIdx > 0)
                        {
                            head = head.Substring(0, parenIdx).Trim();
                        }
                        if (!NotKeywords.Contains(head) && !head.StartsWith("(", StringComparison.Ordinal) && head.Length <= 24)
                        {
                            if (head.Contains('、'))
                            {
                                keys = head.Split('、').Select(x => x.Trim()).Where(x => x != "").ToArray();
                            }
                            else if (!head.Contains(' '))
                            {
                                keys = new[] { head };
                            }
                        }
                    }
                }
                if (keys != null && keys.Length > 0)
                {
                    entries.Add((i, keys));
                }
            }
            for (var k = 0; k < entries.Count; k++)
            {
                var (start, keys) = entries[k];
                var nextStart = k + 1 < entries.Count ? entries[k + 1].line : lines.Length;
                var section = new List<string>();
                for (var j = start; j < Math.Min(nextStart, start + 10); j++)
                {
                    var cleaned = Clean(lines[j]);
                    if (cleaned.StartsWith("```", StringComparison.Ordinal))
                    {
                        break;
                    }
                    if (cleaned != "")
                    {
                        section.Add(cleaned);
                    }
                }
                var text = string.Join("  \n", section) + $"\n\n（ヘルプ「{topic}」）";
                foreach (var key in keys)
                {
                    if (!situations.TryGetValue(key, out var existing) || existing.Length < text.Length)
                    {
                        situations[key] = text;
                    }
                }
            }
            // 「攻撃」「格闘」「射撃」「アビリティ」など項目見出しになっていない標準語の説明を補う
            situations.TryAdd("攻撃", "**攻撃**  \n武器名ごとのメッセージが無いときに使う、攻撃の汎用シチュエーションです。\n\n（ヘルプ「メッセージデータ」）");
            situations.TryAdd("格闘", "**格闘**  \n格闘型の武器で攻撃したとき、武器名のメッセージが無ければ使われます（「攻撃」より優先）。\n\n（ヘルプ「メッセージデータ」）");
            situations.TryAdd("射撃", "**射撃**  \n射撃型の武器で攻撃したとき、武器名のメッセージが無ければ使われます（「攻撃」より優先）。\n\n（ヘルプ「メッセージデータ」）");
            situations.TryAdd("アビリティ", "**アビリティ**  \nアビリティ名のメッセージが無いときに使う、アビリティ使用時の汎用シチュエーションです。\n\n（ヘルプ「メッセージデータ」）");
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

        /// <summary>
        /// メッセージデータのシチュエーション・サブシチュエーション・タグ・変数の説明（Markdown）。無ければ null
        /// </summary>
        public string Situation(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return null;
            }
            var trimmed = word.Trim().Trim('(', ')');
            if (situations.TryGetValue(word.Trim(), out var exact))
            {
                return exact;
            }
            if (situations.TryGetValue(trimmed, out var s))
            {
                return s;
            }
            // (SIZE=14) -> <SIZE=数値> や (対弱点=機) -> 対弱点 など
            var eq = trimmed.IndexOf('=');
            if (eq > 0 && situations.TryGetValue(trimmed.Substring(0, eq), out var eqDoc))
            {
                return eqDoc;
            }
            if (trimmed.StartsWith("対", StringComparison.Ordinal) && situations.TryGetValue("対", out var taiDoc))
            {
                return taiDoc;
            }
            if (trimmed.StartsWith("<", StringComparison.Ordinal) && trimmed.EndsWith(">", StringComparison.Ordinal))
            {
                var inner = trimmed.Trim('<', '>').TrimStart('/');
                if (situations.TryGetValue($"<{inner}>", out var tagDoc))
                {
                    return tagDoc;
                }
                if (inner.StartsWith("COLOR=", StringComparison.OrdinalIgnoreCase) && situations.TryGetValue("<COLOR=色指定>", out var colorDoc))
                {
                    return colorDoc;
                }
                if (inner.StartsWith("SIZE=", StringComparison.OrdinalIgnoreCase) && situations.TryGetValue("<SIZE=数値>", out var sizeDoc))
                {
                    return sizeDoc;
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
