using SRCCore;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Runtime.InteropServices.JavaScript;

namespace SRCSharpWeb
{
    /// <summary>
    /// 表示言語。localStorage の srcweb.language で選ぶ（wwwroot/index.html の起動前の文言も同じ設定を読む）。
    /// 値は SRCCore.UiLanguages のとおり。未設定・"" は日本語、"auto" はブラウザの言語（navigator.language）に従う。
    /// </summary>
    public static partial class WebLocalization
    {
        public const string StorageKey = "srcweb.language";

        /// <summary>起動時に読んだ設定（"" / "auto" / カルチャ名）</summary>
        public static string Setting { get; private set; } = "";

        /// <summary>サイトの既定を含めて実際に使う設定</summary>
        private static string Effective { get; set; } = "";

        /// <summary>言語の選択欄（select）に出す値</summary>
        public static string SelectorValue => UiLanguages.SelectorValue(Effective);

        // UI のスレッド以外から同期で JS を呼べないため、Promise で受け取る
        [JSImport("getLanguageSetting", WebInterop.ModuleName)]
        private static partial Task<string> GetLanguageSetting();

        [JSImport("getBrowserLanguage", WebInterop.ModuleName)]
        private static partial Task<string> GetBrowserLanguage();

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
            var culture = CultureInfo.GetCultureInfo(UiLanguages.Resolve(Effective, await GetBrowserLanguage()));
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
    }
}
