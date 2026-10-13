using SRCCore;
using System;
using System.Globalization;

namespace SRCSharpWeb
{
    /// <summary>
    /// 表示言語。wwwroot/index.html が localStorage の srcweb.language（無ければ srcweb.json の language）から決め、
    /// 環境変数で渡す（マルチスレッドの .NET は起動前に JS とやりとりできないため）。値は SRCCore.UiLanguages のとおり。
    /// </summary>
    public static class WebLocalization
    {
        public const string StorageKey = "srcweb.language";

        /// <summary>使う言語のカルチャ名（index.html が決めたもの）</summary>
        public const string LanguageVariable = "SRCWEB_LANGUAGE";

        /// <summary>言語の設定（"" / "auto" / カルチャ名。サイトの既定を含む）</summary>
        public const string SettingVariable = "SRCWEB_LANGUAGE_SETTING";

        /// <summary>言語の選択欄（select）に出す値</summary>
        public static string SelectorValue { get; private set; } = UiLanguages.Default;

        /// <summary>
        /// 表示言語の UI カルチャを設定する。RunAsync より前に呼ぶ。
        /// DefaultThreadCurrentUICulture にするので、ゲームのスレッド（Web Worker）にも及ぶ。
        /// CurrentCulture（数値・日付の書式）は変えない。
        /// </summary>
        public static void Apply()
        {
            SelectorValue = UiLanguages.SelectorValue(Environment.GetEnvironmentVariable(SettingVariable));
            var culture = CultureInfo.GetCultureInfo(UiLanguages.Resolve(Environment.GetEnvironmentVariable(LanguageVariable), null));
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
    }
}
