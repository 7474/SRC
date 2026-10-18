using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCHelpSearch
{
    public enum HelpSearchMethod
    {
        /// <summary>全文検索とベクトルの検索の順位を合わせる（Reciprocal Rank Fusion）</summary>
        Hybrid,
        /// <summary>全文検索（BM25）だけ</summary>
        Bm25,
        /// <summary>ベクトルの検索だけ</summary>
        Vector,
    }

    /// <summary>
    /// ヘルプの検索。ベクトル（help-vectors.bin）とモデルがあればハイブリッド、無ければ全文検索だけ
    /// </summary>
    public sealed class HelpSearcher : IDisposable
    {
        /// <summary>それぞれの検索から合わせる候補の数</summary>
        private const int Candidates = 50;
        /// <summary>RRF の定数（https://plg.uwaterloo.ca/~gvcormac/cormacksigir09-rrf.pdf）</summary>
        private const double RrfK = 60;
        private const int SnippetLines = 3;

        private readonly Dictionary<string, HelpTopic> topics;
        private readonly HelpIndex index;
        private readonly HelpVectors vectors;
        private readonly HelpEmbedder embedder;

        public HelpSearcher(IReadOnlyList<HelpTopic> topics, HelpVectors vectors, HelpEmbedder embedder)
        {
            this.topics = topics.ToDictionary(x => x.Name);
            index = new HelpIndex(topics);
            if (vectors != null && embedder != null)
            {
                this.vectors = vectors;
                this.embedder = embedder;
            }
            else
            {
                embedder?.Dispose();
            }
        }

        public bool CanSearchVector => vectors != null;

        public IReadOnlyList<HelpSearchResult> Search(string query, int limit, HelpSearchMethod method = HelpSearchMethod.Hybrid)
        {
            if (!CanSearchVector || method == HelpSearchMethod.Bm25 || string.IsNullOrWhiteSpace(query))
            {
                return index.Search(query, limit);
            }
            var semantic = vectors.Search(embedder.Embed(HelpEmbedder.QueryPrefix + query), method == HelpSearchMethod.Vector ? limit : Candidates);
            if (method == HelpSearchMethod.Vector)
            {
                return semantic.Select(x => new HelpSearchResult(topics[x.topic], x.score, ChunkLines(topics[x.topic], x.line))).ToList();
            }
            var keyword = index.Search(query, Candidates);
            var scores = new Dictionary<string, double>();
            for (var i = 0; i < keyword.Count; i++)
            {
                scores[keyword[i].Topic.Name] = scores.GetValueOrDefault(keyword[i].Topic.Name) + 1 / (RrfK + i + 1);
            }
            for (var i = 0; i < semantic.Count; i++)
            {
                scores[semantic[i].topic] = scores.GetValueOrDefault(semantic[i].topic) + 1 / (RrfK + i + 1);
            }
            var keywordLines = keyword.ToDictionary(x => x.Topic.Name, x => x.Lines);
            var semanticLines = semantic.ToDictionary(x => x.topic, x => x.line);
            return scores
                .OrderByDescending(x => x.Value)
                .ThenBy(x => x.Key, StringComparer.Ordinal)
                .Take(limit)
                .Select(x => new HelpSearchResult(topics[x.Key], x.Value,
                    keywordLines.TryGetValue(x.Key, out var lines) && lines.Count > 0 ? lines : ChunkLines(topics[x.Key], semanticLines[x.Key])))
                .ToList();
        }

        /// <summary>断片の最初の空でない行</summary>
        private static IReadOnlyList<(int line, string text)> ChunkLines(HelpTopic topic, int line)
        {
            return topic.Body.Split('\n')
                .Select((text, i) => (line: i + 1, text: text.TrimEnd('\r')))
                .Where(x => x.line >= line && x.text.Trim() != "")
                .Take(SnippetLines)
                .ToList();
        }

        public void Dispose()
        {
            embedder?.Dispose();
        }
    }
}
