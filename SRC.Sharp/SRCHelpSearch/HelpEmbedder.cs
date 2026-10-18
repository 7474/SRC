using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.Tokenizers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCHelpSearch
{
    /// <summary>
    /// 文の埋め込み（ruri-v3-30m、256 次元、平均プーリングして正規化）。
    /// 検索語は「検索クエリ: 」、文書は「検索文書: 」を前に付ける（https://huggingface.co/cl-nagoya/ruri-v3-30m）
    /// </summary>
    public sealed class HelpEmbedder : IDisposable
    {
        public const string QueryPrefix = "検索クエリ: ";
        public const string DocumentPrefix = "検索文書: ";
        public const int Dimension = 256;
        /// <summary>1 つの文の最大のトークン数（モデルは 8192 まで受けるが、断片は短いので抑える）</summary>
        private const int MaxTokens = 512;

        private readonly SentencePieceTokenizer tokenizer;
        private readonly InferenceSession session;

        private HelpEmbedder(SentencePieceTokenizer tokenizer, InferenceSession session)
        {
            this.tokenizer = tokenizer;
            this.session = session;
        }

        /// <summary>実行ファイルの隣の help-model（SRCHelpSearch.csproj がビルドの時に置く）</summary>
        public static string DefaultModelDirectory => Path.Combine(AppContext.BaseDirectory, "help-model");

        /// <summary>モデルが無いか、ONNX Runtime が動かない環境なら null</summary>
        public static HelpEmbedder TryCreate(string directory = null)
        {
            directory = directory ?? DefaultModelDirectory;
            var model = Path.Combine(directory, "model_int8.onnx");
            var tokenizerModel = Path.Combine(directory, "tokenizer.model");
            if (!File.Exists(model) || !File.Exists(tokenizerModel))
            {
                return null;
            }
            try
            {
                SentencePieceTokenizer tokenizer;
                using (var stream = File.OpenRead(tokenizerModel))
                {
                    tokenizer = SentencePieceTokenizer.Create(stream, addBeginningOfSentence: true, addEndOfSentence: true);
                }
                return new HelpEmbedder(tokenizer, new InferenceSession(model));
            }
            catch (Exception ex) when (ex is DllNotFoundException || ex is TypeInitializationException || ex is OnnxRuntimeException || ex is EntryPointNotFoundException || ex is BadImageFormatException)
            {
                // ONNX Runtime のネイティブのライブラリが無い環境（osx-x64・linux-musl など）
                return null;
            }
        }

        public float[] Embed(string text)
        {
            return Embed(new[] { text })[0];
        }

        public float[][] Embed(IReadOnlyList<string> texts)
        {
            var ids = texts.Select(x => tokenizer.EncodeToIds(x).Take(MaxTokens).Select(id => (long)id).ToArray()).ToList();
            var length = ids.Max(x => x.Length);
            var input = new DenseTensor<long>(new[] { ids.Count, length });
            var mask = new DenseTensor<long>(new[] { ids.Count, length });
            for (var i = 0; i < ids.Count; i++)
            {
                for (var t = 0; t < ids[i].Length; t++)
                {
                    input[i, t] = ids[i][t];
                    mask[i, t] = 1;
                }
            }
            using (var outputs = session.Run(new[]
            {
                NamedOnnxValue.CreateFromTensor("input_ids", input),
                NamedOnnxValue.CreateFromTensor("attention_mask", mask),
            }))
            {
                var hidden = outputs.First().AsTensor<float>();
                var result = new float[ids.Count][];
                for (var i = 0; i < ids.Count; i++)
                {
                    var v = new float[Dimension];
                    for (var t = 0; t < ids[i].Length; t++)
                    {
                        for (var d = 0; d < Dimension; d++)
                        {
                            v[d] += hidden[i, t, d];
                        }
                    }
                    var norm = (float)Math.Sqrt(v.Sum(x => x * x));
                    for (var d = 0; d < Dimension; d++)
                    {
                        v[d] /= norm == 0 ? 1 : norm;
                    }
                    result[i] = v;
                }
                return result;
            }
        }

        public void Dispose()
        {
            session.Dispose();
        }
    }
}
