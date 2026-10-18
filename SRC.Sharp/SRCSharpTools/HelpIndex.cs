using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SRCSharpTools
{
    /// <summary>ヘルプの検索の結果。Lines は語をよく含む本文の行（行番号は Body の 1 始まり）</summary>
    public class HelpSearchResult
    {
        public HelpTopic Topic { get; }
        public double Score { get; }
        public IReadOnlyList<(int line, string text)> Lines { get; }

        public HelpSearchResult(HelpTopic topic, double score, IReadOnlyList<(int line, string text)> lines)
        {
            Topic = topic;
            Score = score;
            Lines = lines;
        }
    }

    /// <summary>
    /// ヘルプの全文検索の索引（BM25、https://en.wikipedia.org/wiki/Okapi_BM25）。
    /// 外部のライブラリやモデルを使わず、起動のたびにメモリに作る（428 ページで数十ミリ秒）
    /// </summary>
    public class HelpIndex
    {
        private const double K1 = 1.2;
        private const double B = 0.75;
        /// <summary>見出しの語を本文の何回分に数えるか</summary>
        private const int TitleWeight = 5;
        private const int SnippetLines = 3;

        private readonly IReadOnlyList<HelpTopic> topics;
        private readonly List<Dictionary<string, int>> termFrequencies = new List<Dictionary<string, int>>();
        private readonly List<int> lengths = new List<int>();
        private readonly Dictionary<string, int> documentFrequencies = new Dictionary<string, int>();
        private readonly double averageLength;

        public HelpIndex(IReadOnlyList<HelpTopic> topics)
        {
            this.topics = topics;
            foreach (var topic in topics)
            {
                var tf = new Dictionary<string, int>();
                var length = 0;
                foreach (var term in Tokenize(topic.Body))
                {
                    tf[term] = tf.GetValueOrDefault(term) + 1;
                    length++;
                }
                foreach (var term in Tokenize(topic.Name + " " + topic.Title))
                {
                    tf[term] = tf.GetValueOrDefault(term) + TitleWeight;
                    length += TitleWeight;
                }
                foreach (var term in tf.Keys)
                {
                    documentFrequencies[term] = documentFrequencies.GetValueOrDefault(term) + 1;
                }
                termFrequencies.Add(tf);
                lengths.Add(length);
            }
            averageLength = lengths.Count == 0 ? 1 : lengths.Average();
        }

        public IReadOnlyList<HelpSearchResult> Search(string query, int limit)
        {
            var terms = Tokenize(query).Distinct().ToList();
            if (terms.Count == 0)
            {
                return Array.Empty<HelpSearchResult>();
            }
            var n = topics.Count;
            var idf = terms.ToDictionary(t => t, t =>
            {
                var df = documentFrequencies.GetValueOrDefault(t);
                return Math.Log(1 + (n - df + 0.5) / (df + 0.5));
            });
            var results = new List<(int index, double score)>();
            for (var i = 0; i < n; i++)
            {
                var tf = termFrequencies[i];
                var score = 0.0;
                foreach (var term in terms)
                {
                    if (tf.TryGetValue(term, out var f))
                    {
                        score += idf[term] * f * (K1 + 1) / (f + K1 * (1 - B + B * lengths[i] / averageLength));
                    }
                }
                if (score > 0)
                {
                    results.Add((i, score));
                }
            }
            return results
                .OrderByDescending(x => x.score)
                .ThenBy(x => topics[x.index].Name, StringComparer.Ordinal)
                .Take(limit)
                .Select(x => new HelpSearchResult(topics[x.index], x.score, Snippet(topics[x.index], terms, idf)))
                .ToList();
        }

        /// <summary>語（の重み）を多く含む行</summary>
        private static IReadOnlyList<(int line, string text)> Snippet(HelpTopic topic, IReadOnlyCollection<string> terms, Dictionary<string, double> idf)
        {
            return topic.Body.Split('\n')
                .Select((text, i) => (line: i + 1, text: text.TrimEnd('\r')))
                .Where(x => x.text.Trim() != "")
                .Select(x =>
                {
                    var lineTerms = new HashSet<string>(Tokenize(x.text));
                    return (x.line, x.text, score: terms.Where(lineTerms.Contains).Sum(t => idf[t]));
                })
                .Where(x => x.score > 0)
                .OrderByDescending(x => x.score)
                .ThenBy(x => x.line)
                .Take(SnippetLines)
                .OrderBy(x => x.line)
                .Select(x => (x.line, x.text))
                .ToList();
        }

        /// <summary>
        /// 語に分ける。NFKC で全角の英数字を半角にし、小文字にする。
        /// 英数字の続きは 1 語、それ以外（漢字・かななど）は文字の 2-gram（1 文字だけなら 1-gram）
        /// </summary>
        public static IEnumerable<string> Tokenize(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormKC).ToLowerInvariant();
            var word = new StringBuilder();
            var run = new StringBuilder();
            foreach (var c in normalized.Append(' '))
            {
                if (IsWordChar(c))
                {
                    foreach (var t in FlushRun(run)) yield return t;
                    word.Append(c);
                }
                else if (IsRunChar(c))
                {
                    if (word.Length > 0) { yield return word.ToString(); word.Clear(); }
                    run.Append(c);
                }
                else
                {
                    if (word.Length > 0) { yield return word.ToString(); word.Clear(); }
                    foreach (var t in FlushRun(run)) yield return t;
                }
            }
        }

        private static IEnumerable<string> FlushRun(StringBuilder run)
        {
            if (run.Length == 0)
            {
                yield break;
            }
            var s = run.ToString();
            run.Clear();
            if (s.Length == 1)
            {
                yield return s;
                yield break;
            }
            for (var i = 0; i + 1 < s.Length; i++)
            {
                yield return s.Substring(i, 2);
            }
        }

        private static bool IsWordChar(char c)
        {
            return c < 0x80 && (char.IsLetterOrDigit(c) || c == '_');
        }

        private static bool IsRunChar(char c)
        {
            var category = char.GetUnicodeCategory(c);
            return c >= 0x80 && (char.IsLetterOrDigit(c) || category == UnicodeCategory.NonSpacingMark || c == 'ー');
        }
    }
}
