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

        /// <summary>すべての区分のパイロット（組にしない比較）</summary>
        public IReadOnlyList<AnalysisPilot> Pilots { get; private set; } = Array.Empty<AnalysisPilot>();

        /// <summary>すべての区分のユニット（組にしない比較）</summary>
        public IReadOnlyList<AnalysisUnit> Units { get; private set; } = Array.Empty<AnalysisUnit>();

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
                    foreach (var x in set.Pilots ?? new List<AnalysisPilot>())
                    {
                        x.Section = file.Section;
                    }
                    foreach (var x in set.Units ?? new List<AnalysisUnit>())
                    {
                        x.Section = file.Section;
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
            medians.Clear();
            GeneratedAt = generatedAt;
            Pairs = sets.Values.SelectMany(x => x.Pairs).ToList();
            Pilots = sets.Values.SelectMany(x => x.Pilots ?? new List<AnalysisPilot>()).ToList();
            Units = sets.Values.SelectMany(x => x.Units ?? new List<AnalysisUnit>()).ToList();
            var first = sets.Values.FirstOrDefault();
            Uses = first?.Uses ?? new List<string>();
            Indices = first?.Indices ?? new List<string>();
            Bands = first?.Bands ?? new List<string>();
        }

        public AnalysisPair Find(string key) => Pairs.FirstOrDefault(x => x.Key == key);

        /// <summary>パイロットかユニットを鍵で引く</summary>
        public AnalysisSingle FindSingle(string mode, string key) =>
            mode == "unit" ? Units.FirstOrDefault(x => x.Key == key) : Pilots.FirstOrDefault(x => x.Key == key);

        /// <summary>
        /// 組にしない比較の指数（同じ区分・同じ帯の、0 より大きい値の中央値 = 100）。ユニットは帯を問わず同じ区分の中央値。
        /// 中央値が無ければ null
        /// </summary>
        public double? SingleIndex(AnalysisSingle x, string metric)
        {
            var v = x.ValueOf(metric);
            if (!v.HasValue) return null;
            var unit = x is AnalysisUnit;
            var key = string.Join("|", unit ? "unit" : "pilot", x.Section, unit ? "" : x.Band, metric);
            if (!medians.TryGetValue(key, out var m))
            {
                IEnumerable<AnalysisSingle> peers = unit
                    ? Units.Where(u => u.Section == x.Section)
                    : Pilots.Where(p => p.Section == x.Section && p.Band == x.Band);
                var values = peers.Select(p => p.ValueOf(metric)).Where(y => y > 0).Select(y => y.Value).OrderBy(y => y).ToList();
                m = values.Count == 0 ? 0 : values[values.Count / 2];
                medians[key] = m;
            }
            return m <= 0 ? null : 100 * v.Value / m;
        }

        private readonly Dictionary<string, double> medians = new Dictionary<string, double>();
    }
}
