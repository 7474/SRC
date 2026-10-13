using Microsoft.Extensions.Localization;
using SRCSharpWeb.Resources;
using System;
using System.Globalization;
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

        /// <summary>言語の選択欄（select）に出す値</summary>
        public static string SelectorValue
        {
            get
            {
                var value = Setting.Trim().ToLowerInvariant();
                return value == "en" || value == "auto" ? value : "ja";
            }
        }

        [JSImport("getLanguageSetting", WebInterop.ModuleName)]
        private static partial string GetLanguageSetting();

        [JSImport("getBrowserLanguage", WebInterop.ModuleName)]
        private static partial string GetBrowserLanguage();

        [JSImport("setModalLabels", WebInterop.ModuleName)]
        private static partial void SetModalLabels(string input, string ok, string cancel, string choice);

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
        /// </summary>
        public static void Apply()
        {
            Setting = GetLanguageSetting() ?? "";
            // TODO: サイトの設定（srcweb.json の language）を既定にする。
            // localStorage が未設定（Setting == ""）のときは、ここでサイトの既定の値を Setting の代わりに使う。
            var culture = CultureInfo.GetCultureInfo(Resolve(Setting, GetBrowserLanguage()));
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        /// <summary>JS のモーダル（srcweb.js）の既定の文言を渡す</summary>
        public static void ApplyModalLabels(IStringLocalizer<WebStrings> l)
        {
            SetModalLabels(l["ModalInput"], l["ModalOk"], l["ModalCancel"], l["ModalChoice"]);
        }
    }
}
