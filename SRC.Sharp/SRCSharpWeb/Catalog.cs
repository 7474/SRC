using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SRCSharpWeb
{
    /// <summary>
    /// サイトの設定（srcweb.json）。置かなければ既定値で動く。
    /// SRC#Web を外部のサイトに置くときに、配信するシナリオや利用者に許す操作を決める（docs/gui/web-hosting.md）。
    /// </summary>
    public class SiteConfig
    {
        public const string FileName = "srcweb.json";

        /// <summary>画面の見出し</summary>
        public string Title { get; set; } = "SRC#Web";

        /// <summary>読み込むカタログの URL（サイトからの相対 URL か絶対 URL）</summary>
        public List<string> Catalogs { get; set; } = new List<string> { "scenarios/catalog.json" };

        /// <summary>
        /// URL の play で開いたときに読むシナリオごとのカタログの URL（{id} をシナリオの id に置き換える）。
        /// 指定すると、すべてのカタログ（catalogs）を読まずにそのシナリオのカタログだけを読んで始める。読めなければ catalogs を読む
        /// </summary>
        public string PlayCatalog { get; set; }

        /// <summary>利用者が zip（シナリオ・素材）を追加できるか</summary>
        public bool AllowUserArchives { get; set; } = true;

        /// <summary>利用者がカタログの URL を追加できるか（URL の catalog・archive パラメータも含む）</summary>
        public bool AllowUserCatalogs { get; set; } = true;

        /// <summary>開いたらすぐに始めるシナリオ（カタログの id）。シナリオ 1 本だけを配信するサイト向け</summary>
        public string Start { get; set; }

        /// <summary>表示言語の既定（"ja"・"en" などのカルチャ名か "auto"）。利用者が画面で選んだ言語が優先する（wwwroot/index.html が読む）</summary>
        public string Language { get; set; }

        public static async Task<SiteConfig> LoadAsync(HttpClient http)
        {
            try
            {
                using var response = await http.GetAsync(FileName);
                if (!response.IsSuccessStatusCode)
                {
                    return new SiteConfig();
                }
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<SiteConfig>(json, Catalog.JsonOptions) ?? new SiteConfig();
            }
            catch (Exception ex)
            {
                WebLogging.Log.LogWarning(ex, "{File} を読めません", FileName);
                return new SiteConfig();
            }
        }
    }

    /// <summary>カタログ（配信しているシナリオ・素材の一覧）</summary>
    public class Catalog
    {
        internal static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        public int Format { get; set; } = 1;
        public string Name { get; set; }
        public List<CatalogEntry> Entries { get; set; } = new List<CatalogEntry>();

        /// <summary>カタログを読んだ URL</summary>
        [JsonIgnore]
        public string Url { get; set; }

        [JsonIgnore]
        public IEnumerable<CatalogEntry> Scenarios => Entries.Where(x => x.Kind == CatalogEntry.KindScenario);

        /// <summary>
        /// カタログを読む。配列だけの形式（エントリの一覧）と、名前などを持つオブジェクトの形式の両方を受け付ける。
        /// アーカイブの URL はカタログの URL からの相対 URL として絶対 URL にする。
        /// ただし配列だけの形式（以前のサイト組み込みの形式）はサイトからの相対 URL とする。
        /// </summary>
        public static Catalog Parse(string json, Uri catalogUri, Uri siteUri)
        {
            Catalog catalog;
            Uri baseUri;
            using (var doc = JsonDocument.Parse(json, new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true }))
            {
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    catalog = new Catalog { Entries = JsonSerializer.Deserialize<List<CatalogEntry>>(json, JsonOptions) };
                    baseUri = siteUri;
                }
                else
                {
                    catalog = JsonSerializer.Deserialize<Catalog>(json, JsonOptions);
                    baseUri = catalogUri;
                }
            }
            catalog ??= new Catalog();
            catalog.Url = catalogUri.ToString();
            catalog.Entries = (catalog.Entries ?? new List<CatalogEntry>()).Where(x => x != null && !string.IsNullOrEmpty(x.Id)).ToList();
            foreach (var entry in catalog.Entries)
            {
                entry.Kind = string.IsNullOrEmpty(entry.Kind) ? CatalogEntry.KindScenario : entry.Kind;
                entry.Title = string.IsNullOrEmpty(entry.Title) ? entry.Id : entry.Title;
                if (!string.IsNullOrEmpty(entry.Archive))
                {
                    entry.Archive = new Uri(baseUri, entry.Archive).ToString();
                }
                entry.Requires ??= new List<string>();
            }
            return catalog;
        }

        public static async Task<Catalog> LoadAsync(HttpClient http, string url)
        {
            var uri = new Uri(http.BaseAddress, url);
            var json = await http.GetStringAsync(uri);
            return Parse(json, uri, http.BaseAddress);
        }
    }

    public class CatalogEntry
    {
        public const string KindScenario = "scenario";
        public const string KindAsset = "asset";

        public string Id { get; set; }

        /// <summary>scenario（シナリオ）・asset（汎用グラフィック集などの素材）</summary>
        public string Kind { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }
        public string License { get; set; }
        public string Description { get; set; }

        /// <summary>zip の URL（カタログからの相対 URL か絶対 URL）</summary>
        public string Archive { get; set; }

        /// <summary>
        /// zip の版。取得した zip はブラウザにキャッシュし、同じ URL ならネットワークを使わない。
        /// zip を同じ URL のまま差し替えたら、これを変えると取得し直す（ハッシュ値や日付など、変わればよい）
        /// </summary>
        public string Version { get; set; }

        /// <summary>はじめに実行するシナリオ（zip の中のパス）</summary>
        public string Eve { get; set; }

        /// <summary>zip の中でこのフォルダ以下だけを使う（zip が 1 段フォルダに入っている場合など）</summary>
        public string Root { get; set; }

        /// <summary>一緒に重ねる素材の id</summary>
        public List<string> Requires { get; set; } = new List<string>();

        public const string RatingGeneral = "general";
        public const string RatingR15 = "r15";
        public const string RatingR18 = "r18";

        /// <summary>年齢の区分（general・r15・r18）。r15・r18 は始める前に年齢を確かめる</summary>
        public string Rating { get; set; }

        /// <summary>年齢の区分の対象の年齢。区分の無いもの（general）は 0</summary>
        [JsonIgnore]
        public int RatingAge => RatingAgeOf(Rating);

        public static int RatingAgeOf(string rating) => rating?.ToLowerInvariant() switch
        {
            RatingR15 => 15,
            RatingR18 => 18,
            _ => 0,
        };
    }
}
