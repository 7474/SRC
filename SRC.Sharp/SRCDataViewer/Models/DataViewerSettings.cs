namespace SRCDataViewer.Models
{
    public enum ThemeMode
    {
        System,
        Light,
        Dark,
    }

    public enum DataDisplayOrder
    {
        UnitFirst,
        PilotFirst,
    }

    public class DataViewerSettings
    {
        public ThemeMode ThemeMode { get; set; } = ThemeMode.System;
        public bool ShowComment { get; set; } = true;
        public bool ShowRawButton { get; set; } = true;
        public bool ShowPilotMessage { get; set; } = true;
        public DataDisplayOrder DataDisplayOrder { get; set; } = DataDisplayOrder.UnitFirst;
        /// <summary>表示の言語（ja・en などのカルチャ名か auto）。auto はブラウザの設定に従う。変えたらページを再読み込みする</summary>
        public string Language { get; set; } = UiLanguage.Japanese;
    }

    public static class UiLanguage
    {
        public const string Japanese = "ja";
        public const string English = "en";
        public const string Auto = "auto";

        /// <summary>
        /// 言語の設定から、起動時に設定するカルチャ名を返す（SRCCore.UiLanguages）。
        /// auto はブラウザの言語（訳が無ければ日本語）、未設定や不明な値は日本語
        /// </summary>
        public static string ToCultureName(string language, string browserCultureName)
        {
            return SRCCore.UiLanguages.Resolve(language, browserCultureName);
        }
    }
}
