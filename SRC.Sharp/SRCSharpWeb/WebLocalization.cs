using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Runtime.InteropServices.JavaScript;

namespace SRCSharpWeb
{
    /// <summary>
    /// 表示言語。localStorage の srcweb.language で選ぶ（wwwroot/index.html の起動前の文言も同じ設定を読む）。
    /// 未設定・"" ・"ja" は日本語、"en" は英語、"auto" はブラウザの言語（navigator.language）に従う。
    /// </summary>
    public static partial class WebLocalization
    {
        public const string StorageKey = "srcweb.language";

        /// <summary>起動時に読んだ設定（"" / "ja" / "en" / "auto"）</summary>
        public static string Setting { get; private set; } = "";

        /// <summary>サイトの既定を含めて実際に使う設定</summary>
        private static string Effective { get; set; } = "";

        /// <summary>言語の選択欄（select）に出す値</summary>
        public static string SelectorValue
        {
            get
            {
                var value = Effective.Trim().ToLowerInvariant();
                return value == "en" || value == "auto" ? value : "ja";
            }
        }

        // UI のスレッド以外から同期で JS を呼べないため、Promise で受け取る
        [JSImport("getLanguageSetting", WebInterop.ModuleName)]
        private static partial Task<string> GetLanguageSetting();

        [JSImport("getBrowserLanguage", WebInterop.ModuleName)]
        private static partial Task<string> GetBrowserLanguage();

        /// <summary>設定とブラウザの言語から、使う言語（"ja" か "en"）を決める</summary>
        public static string Resolve(string setting, string browserLanguage)
        {
            switch ((setting ?? "").Trim().ToLowerInvariant())
            {
                case "en":
                    return "en";
                case "auto":
                    return (browserLanguage ?? "").StartsWith("ja", StringComparison.OrdinalIgnoreCase) ? "ja" : "en";
                default:
                    return "ja";
            }
        }

        /// <summary>
        /// 表示言語を決めて UI カルチャを設定する。RunAsync より前に呼ぶ。
        /// DefaultThreadCurrentUICulture にするので、ゲームのスレッド（Web Worker）にも及ぶ。
        /// CurrentCulture（数値・日付の書式）は変えない。
        /// 利用者が選んでいなければ、サイトの既定（srcweb.json の language）に従う。
        /// </summary>
        public static async Task ApplyAsync(string siteDefault)
        {
            Setting = await GetLanguageSetting() ?? "";
            Effective = (string.IsNullOrWhiteSpace(Setting) ? siteDefault : Setting) ?? "";
            var culture = CultureInfo.GetCultureInfo(Resolve(Effective, await GetBrowserLanguage()));
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
    }
}
