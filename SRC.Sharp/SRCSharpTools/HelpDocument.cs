using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using SRCHelpSearch;

namespace SRCSharpTools
{
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

        private const string VectorsResource = "help-vectors.bin";

        private static readonly Lazy<HelpSearcher> searcher = new Lazy<HelpSearcher>(() =>
        {
            HelpVectors vectors = null;
            using (var stream = typeof(HelpDocument).Assembly.GetManifestResourceStream(VectorsResource))
            {
                if (stream != null)
                {
                    vectors = HelpVectors.Read(stream);
                }
            }
            return new HelpSearcher(Topics, vectors, vectors == null ? null : HelpEmbedder.TryCreate());
        });

        /// <summary>ベクトルの検索ができるか（ビルドの時にモデルを取れて、help-vectors.bin を埋め込めたか）</summary>
        public static bool CanSearchVector => searcher.Value.CanSearchVector;

        /// <summary>
        /// 関連の高い順にページを探す。既定は全文検索（BM25）とベクトルの検索（ruri-v3-30m）のハイブリッド。
        /// ベクトルの検索ができなければ全文検索だけ
        /// </summary>
        public static IReadOnlyList<HelpSearchResult> Search(string query, int limit = 10, HelpSearchMethod method = HelpSearchMethod.Hybrid)
        {
            return searcher.Value.Search(query, limit, method);
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
