using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCHelpSearch
{
    /// <summary>
    /// ヘルプの断片のベクトルを作る。SRCSharpTools のビルドで実行する。
    ///   SRCHelpSearch &lt;ヘルプの *.md のフォルダ&gt; &lt;出力（help-vectors.bin）&gt;
    /// </summary>
    public static class Program
    {
        private const int BatchSize = 16;

        public static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Error.WriteLine("usage: SRCHelpSearch <help-src-dir> <output>");
                return 2;
            }
            using (var embedder = HelpEmbedder.TryCreate())
            {
                if (embedder == null)
                {
                    Console.Error.WriteLine("埋め込みのモデルがありません: " + HelpEmbedder.DefaultModelDirectory);
                    return 1;
                }
                var watch = Stopwatch.StartNew();
                var chunks = Directory.GetFiles(args[0], "*.md")
                    .Where(x => Path.GetFileName(x) != "404.md")
                    .OrderBy(x => x, StringComparer.Ordinal)
                    .Select(x => new HelpTopic(Path.GetFileNameWithoutExtension(x), File.ReadAllText(x, Encoding.UTF8)))
                    .SelectMany(HelpVectors.Split)
                    .ToList();
                // 長さの近いものをまとめると詰め物が減って速い
                var order = Enumerable.Range(0, chunks.Count).OrderBy(i => chunks[i].Text.Length).ToList();
                var vectors = new float[chunks.Count][];
                for (var i = 0; i < order.Count; i += BatchSize)
                {
                    var batch = order.Skip(i).Take(BatchSize).ToList();
                    var embedded = embedder.Embed(batch.Select(x => HelpEmbedder.DocumentPrefix + chunks[x].Text).ToList());
                    for (var j = 0; j < batch.Count; j++)
                    {
                        vectors[batch[j]] = embedded[j];
                    }
                }
                Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(args[1])));
                using (var stream = File.Create(args[1]))
                {
                    HelpVectors.Write(stream, chunks, vectors);
                }
                Console.WriteLine($"{chunks.Count} chunks, {watch.Elapsed.TotalSeconds:0.0}s");
                return 0;
            }
        }
    }
}
