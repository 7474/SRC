using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SRCDataViewer.Models
{
    /// <summary>
    /// 組の評価（7474/SRC-Data の Pages の analysis/*.json）を読み、区分をまたいで引く（「比べる」の頁が使う）。
    /// 一覧の場所は viewer.json の analysisIndex、無ければ 7474/SRC-Data の Pages
    /// </summary>
    public class AnalysisStore
    {
        public const string DefaultIndexUri = "https://7474.github.io/SRC-Data/analysis/index.json";

        private Task loading;

        public bool Loaded { get; private set; }
        public string Error { get; private set; }
        public string GeneratedAt { get; private set; }

        /// <summary>区分（ロボ・拡張・Sharp）ごとの評価</summary>
        public IReadOnlyDictionary<string, AnalysisSet> Sets { get; private set; } = new Dictionary<string, AnalysisSet>();

        /// <summary>すべての区分の組</summary>
        public IReadOnlyList<AnalysisPair> Pairs { get; private set; } = Array.Empty<AnalysisPair>();

        public IReadOnlyList<string> Uses { get; private set; } = Array.Empty<string>();
        public IReadOnlyList<string> Indices { get; private set; } = Array.Empty<string>();
        public IReadOnlyList<string> Bands { get; private set; } = Array.Empty<string>();

        /// <summary>読み込む（一度だけ）。読めなければ Error に理由を入れる</summary>
        public Task EnsureLoadedAsync(HttpClient http, string indexUri = null)
        {
            return loading ??= LoadAsync(http, indexUri ?? DefaultIndexUri);
        }

        private async Task LoadAsync(HttpClient http, string indexUri)
        {
            try
            {
                var baseUri = new Uri(http.BaseAddress, indexUri);
                var index = await http.GetFromJsonAsync(baseUri, AnalysisJsonContext.Default.AnalysisIndex);
                var sets = new Dictionary<string, AnalysisSet>();
                foreach (var file in index?.Files ?? new List<AnalysisIndexFile>())
                {
                    var set = await http.GetFromJsonAsync(new Uri(baseUri, file.File), AnalysisJsonContext.Default.AnalysisSet);
                    if (set == null) continue;
                    foreach (var pair in set.Pairs)
                    {
                        pair.Section = file.Section;
                    }
                    sets[file.Section] = set;
                }
                Load(sets, index?.GeneratedAt);
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
            finally
            {
                Loaded = true;
            }
        }

        /// <summary>読み込んだ評価を入れる（テストでも使う）</summary>
        public void Load(Dictionary<string, AnalysisSet> sets, string generatedAt)
        {
            Sets = sets;
            GeneratedAt = generatedAt;
            Pairs = sets.Values.SelectMany(x => x.Pairs).ToList();
            var first = sets.Values.FirstOrDefault();
            Uses = first?.Uses ?? new List<string>();
            Indices = first?.Indices ?? new List<string>();
            Bands = first?.Bands ?? new List<string>();
        }

        public AnalysisPair Find(string key) => Pairs.FirstOrDefault(x => x.Key == key);
    }
}
