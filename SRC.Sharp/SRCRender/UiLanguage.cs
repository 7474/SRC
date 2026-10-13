using SRCCore;
using System.Globalization;

namespace SRCRender
{
    /// <summary>
    /// 画面の表示言語（CultureInfo.CurrentUICulture）を設定する。
    /// 数値の書式などに関わる CurrentCulture は変えない。
    /// </summary>
    public static class UiLanguage
    {
        /// <summary>設定ファイル（srcs.json）で表示言語を指定する項目のセクション名</summary>
        public const string ConfigSection = "Option";
        /// <summary>設定ファイル（srcs.json）で表示言語を指定する項目名</summary>
        public const string ConfigKey = "Language";

        /// <summary>既定の言語（日本語）</summary>
        public const string Default = "ja";

        // 起動時（最初に参照した時点）の UI カルチャ。"auto" で使う
        private static readonly CultureInfo OsUICulture = CultureInfo.CurrentUICulture;

        /// <summary>
        /// 言語の指定から UI カルチャを決める。
        /// 空・未指定・"ja" は日本語（OS の言語によらない）、"auto" は OS の UI カルチャ、それ以外はそのカルチャ名。
        /// 解釈できない指定は日本語とする。
        /// </summary>
        public static CultureInfo Resolve(string language)
        {
            var name = (language ?? "").Trim();
            if (name == "")
            {
                name = Default;
            }
            if (string.Equals(name, "auto", System.StringComparison.OrdinalIgnoreCase))
            {
                return OsUICulture;
            }
            try
            {
                return CultureInfo.GetCultureInfo(name);
            }
            catch (CultureNotFoundException)
            {
                return CultureInfo.GetCultureInfo(Default);
            }
        }

        /// <summary>
        /// 言語の指定（""・"ja"・"en"・"auto" など）を現在のスレッドと以降に作られるスレッドの UI カルチャに設定する。
        /// </summary>
        public static void Apply(string language)
        {
            var culture = Resolve(language);
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        /// <summary>
        /// 設定（Option/Language）の言語を設定する。
        /// </summary>
        public static void Apply(ISystemConfig config)
        {
            Apply(config?.GetItem(ConfigSection, ConfigKey));
        }
    }
}
