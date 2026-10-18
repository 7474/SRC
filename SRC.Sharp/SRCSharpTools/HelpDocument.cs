using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SRCSharpTools
{
    /// <summary>ヘルプの 1 ページ（SRC.Sharp.Help/src の *.md）</summary>
    public class HelpTopic
    {
        /// <summary>ファイル名から .md を除いたもの。srcsharp help に渡す名前で、ページの中のリンク先とも同じ</summary>
        public string Name { get; }
        /// <summary>見出し。front matter の title（HLP000164 などの番号だけのものは本文の最初の太字）</summary>
        public string Title { get; }
        /// <summary>ファイルの内容（front matter を含む）</summary>
        public string Source { get; }
        /// <summary>front matter を除いた本文</summary>
        public string Body { get; }

        public HelpTopic(string name, string source)
        {
            Name = name;
            Source = source;
            var title = name;
            var match = Regex.Match(source, @"\A---\r?\n(.*?)\r?\n---\r?\n?", RegexOptions.Singleline);
            if (match.Success)
            {
                var titleLine = Regex.Match(match.Groups[1].Value, @"^title:\s*(.+?)\s*$", RegexOptions.Multiline);
                if (titleLine.Success)
                {
                    title = titleLine.Groups[1].Value;
                }
                Body = source.Substring(match.Length);
            }
            else
            {
                Body = source;
            }
            if (Regex.IsMatch(title, @"^HLP\d+$"))
            {
                var heading = Regex.Match(Body, @"^\*\*(?!\s)([^*]+)\*\*\s*$", RegexOptions.Multiline);
                if (heading.Success)
                {
                    title = heading.Groups[1].Value;
                }
            }
            Title = title;
        }
    }

    /// <summary>
    /// 同梱のヘルプ（SRCSharpTools.csproj で SRC.Sharp.Help/src の *.md を埋め込む）。
    /// srcsharp help で、人や AI がデータ・イベントコマンド・関数などのリファレンスを引くのに使う
    /// </summary>
    public static class HelpDocument
    {
        private const string ResourcePrefix = "help/";

        private static readonly Lazy<IReadOnlyList<HelpTopic>> topics = new Lazy<IReadOnlyList<HelpTopic>>(Load);

        public static IReadOnlyList<HelpTopic> Topics => topics.Value;

        private static IReadOnlyList<HelpTopic> Load()
        {
            var assembly = typeof(HelpDocument).Assembly;
            return assembly.GetManifestResourceNames()
                .Where(x => x.StartsWith(ResourcePrefix, StringComparison.Ordinal) && x.EndsWith(".md", StringComparison.Ordinal))
                .Select(x =>
                {
                    using (var reader = new StreamReader(assembly.GetManifestResourceStream(x), Encoding.UTF8))
                    {
                        return new HelpTopic(x.Substring(ResourcePrefix.Length, x.Length - ResourcePrefix.Length - ".md".Length), reader.ReadToEnd());
                    }
                })
                .OrderBy(x => x.Name, StringComparer.Ordinal)
                .ToList();
        }

        /// <summary>
        /// 名前か見出しで探す。ページの中のリンク（Askコマンド.md#書式）もそのまま渡せる。
        /// 大文字・小文字は区別しない。一致が無ければ前方一致（Ask → Askコマンド）、それも無ければ部分一致の候補を返す
        /// </summary>
        public static IReadOnlyList<HelpTopic> Find(string query)
        {
            var key = query.Trim();
            var hash = key.IndexOf('#');
            if (hash >= 0)
            {
                key = key.Substring(0, hash);
            }
            if (key.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            {
                key = key.Substring(0, key.Length - ".md".Length);
            }
            if (key == "")
            {
                return Array.Empty<HelpTopic>();
            }
            var exact = Topics.Where(x => Matches(x, key, (a, b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase))).ToList();
            if (exact.Count > 0)
            {
                return exact;
            }
            var prefix = Topics.Where(x => Matches(x, key, (a, b) => a.StartsWith(b, StringComparison.OrdinalIgnoreCase))).ToList();
            if (prefix.Count > 0)
            {
                return prefix;
            }
            return Topics.Where(x => Matches(x, key, (a, b) => a.Contains(b, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        private static bool Matches(HelpTopic topic, string key, Func<string, string, bool> match)
        {
            return match(topic.Name, key) || match(topic.Title, key);
        }

        private static readonly Lazy<HelpIndex> index = new Lazy<HelpIndex>(() => new HelpIndex(Topics));

        /// <summary>
        /// 関連の高い順にページを探す（BM25。日本語は文字の 2-gram、英数字は単語。見出しは重く数える）。
        /// 空白で区切った複数の語も、語順や表記の揺れが少しあっても当たる
        /// </summary>
        public static IReadOnlyList<HelpSearchResult> Search(string query, int limit = 10)
        {
            return index.Value.Search(query, limit);
        }

        /// <summary>本文の行を探す（大文字・小文字は区別しない）。行番号は Body の 1 始まり</summary>
        public static IEnumerable<(HelpTopic topic, int line, string text)> Grep(string text)
        {
            foreach (var topic in Topics)
            {
                var lines = topic.Body.Split('\n');
                for (var i = 0; i < lines.Length; i++)
                {
                    var line = lines[i].TrimEnd('\r');
                    if (line.Contains(text, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return (topic, i + 1, line);
                    }
                }
            }
        }

        /// <summary>すべてのページを元のファイル名（*.md）で書き出す。書き出した数を返す</summary>
        public static int Export(string directory)
        {
            Directory.CreateDirectory(directory);
            foreach (var topic in Topics)
            {
                File.WriteAllText(Path.Combine(directory, topic.Name + ".md"), topic.Source, new UTF8Encoding(false));
            }
            return Topics.Count;
        }
    }
}
