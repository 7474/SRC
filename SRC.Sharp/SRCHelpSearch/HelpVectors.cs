using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SRCHelpSearch
{
    /// <summary>ヘルプの断片。Line は Body の 1 始まりの行番号</summary>
    public class HelpChunk
    {
        public string Topic { get; }
        public int Line { get; }
        public string Text { get; }

        public HelpChunk(string topic, int line, string text)
        {
            Topic = topic;
            Line = line;
            Text = text;
        }
    }

    /// <summary>
    /// ヘルプの断片の埋め込みのベクトル（help-vectors.bin）。ビルドの時に SRCHelpSearch を実行して作り、
    /// srcsharp に埋め込む。ベクトルは正規化して int8（×127）で持ち、検索は全件の内積（数千件なので索引は要らない）
    /// </summary>
    public class HelpVectors
    {
        private const string Magic = "SRCHV1";
        /// <summary>断片の目安の文字数。空行で区切った段落をこの長さまでまとめる</summary>
        private const int ChunkLength = 300;

        private readonly List<(string topic, int line)> chunks;
        private readonly sbyte[] vectors;

        private HelpVectors(List<(string topic, int line)> chunks, sbyte[] vectors)
        {
            this.chunks = chunks;
            this.vectors = vectors;
        }

        public int Count => chunks.Count;

        /// <summary>断片に分ける。各断片の前に見出しを付ける。Markdown の記号とリンク先は除く</summary>
        public static IEnumerable<HelpChunk> Split(HelpTopic topic)
        {
            var lines = topic.Body.Split('\n').Select(x => x.TrimEnd('\r')).ToArray();
            var buffer = new StringBuilder();
            var start = 0;
            for (var i = 0; i <= lines.Length; i++)
            {
                var end = i == lines.Length || lines[i].Trim() == "";
                if (!end)
                {
                    if (buffer.Length == 0)
                    {
                        start = i + 1;
                    }
                    buffer.Append(Clean(lines[i])).Append('\n');
                }
                if ((end && buffer.Length >= ChunkLength) || (i == lines.Length && buffer.Length > 0))
                {
                    yield return new HelpChunk(topic.Name, start, topic.Title + " " + buffer);
                    buffer.Clear();
                }
                else if (end && buffer.Length > 0)
                {
                    buffer.Append('\n');
                }
            }
        }

        private static string Clean(string line)
        {
            line = Regex.Replace(line, @"\]\([^)]*\)", "]");
            return Regex.Replace(line, @"[*#>|`\[\]]", " ");
        }

        public static void Write(Stream stream, IReadOnlyList<HelpChunk> chunks, IReadOnlyList<float[]> vectors)
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.Write(Magic);
                writer.Write(HelpEmbedder.Dimension);
                writer.Write(chunks.Count);
                for (var i = 0; i < chunks.Count; i++)
                {
                    writer.Write(chunks[i].Topic);
                    writer.Write(chunks[i].Line);
                    foreach (var x in vectors[i])
                    {
                        writer.Write((sbyte)Math.Clamp(Math.Round(x * 127), -127, 127));
                    }
                }
            }
        }

        public static HelpVectors Read(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                if (reader.ReadString() != Magic || reader.ReadInt32() != HelpEmbedder.Dimension)
                {
                    throw new InvalidDataException("help-vectors.bin の形式が違う");
                }
                var count = reader.ReadInt32();
                var chunks = new List<(string, int)>(count);
                var vectors = new sbyte[count * HelpEmbedder.Dimension];
                for (var i = 0; i < count; i++)
                {
                    chunks.Add((reader.ReadString(), reader.ReadInt32()));
                    for (var d = 0; d < HelpEmbedder.Dimension; d++)
                    {
                        vectors[i * HelpEmbedder.Dimension + d] = reader.ReadSByte();
                    }
                }
                return new HelpVectors(chunks, vectors);
            }
        }

        /// <summary>ページを近い順に返す。ページの近さは断片の近さ（コサイン類似度）の最大。line は最も近い断片の行</summary>
        public IReadOnlyList<(string topic, double score, int line)> Search(float[] query, int limit)
        {
            var best = new Dictionary<string, (double score, int line)>();
            for (var i = 0; i < chunks.Count; i++)
            {
                var dot = 0.0;
                var offset = i * HelpEmbedder.Dimension;
                for (var d = 0; d < HelpEmbedder.Dimension; d++)
                {
                    dot += vectors[offset + d] * query[d];
                }
                dot /= 127;
                var (topic, line) = chunks[i];
                if (!best.TryGetValue(topic, out var current) || current.score < dot)
                {
                    best[topic] = (dot, line);
                }
            }
            return best
                .OrderByDescending(x => x.Value.score)
                .ThenBy(x => x.Key, StringComparer.Ordinal)
                .Take(limit)
                .Select(x => (x.Key, x.Value.score, x.Value.line))
                .ToList();
        }
    }
}
