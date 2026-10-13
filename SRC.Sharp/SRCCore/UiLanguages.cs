using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore
{
    /// <summary>
    /// 画面の表示言語の一覧と、言語の指定の解釈（各アプリの Resources/*.resx の訳と揃える）。
    /// docs/i18n.md
    /// </summary>
    public static class UiLanguages
    {
        /// <summary>既定（ニュートラル）の言語</summary>
        public const string Default = "ja";

        /// <summary>OS・ブラウザの言語に従う指定</summary>
        public const string Auto = "auto";

        /// <summary>
        /// 対応している言語（カルチャ名と、その言語での名前）。選択欄はこの順に並べる。
        /// Steam の利用者の言語の割合（https://store.steampowered.com/hwsurvey/ ）が 0.5% 以上の言語を選んでいる。
        /// es・pt-BR は地域違い（中南米のスペイン語・ポルトガルのポルトガル語）にも使う（Match）
        /// </summary>
        public static IReadOnlyList<UiLanguageInfo> Supported { get; } = new[]
        {
            new UiLanguageInfo("ja", "日本語"),
            new UiLanguageInfo("en", "English"),
            new UiLanguageInfo("zh-Hans", "简体中文"),
            new UiLanguageInfo("zh-Hant", "繁體中文"),
            new UiLanguageInfo("ko", "한국어"),
            new UiLanguageInfo("fr", "Français"),
            new UiLanguageInfo("de", "Deutsch"),
            new UiLanguageInfo("es", "Español"),
            new UiLanguageInfo("pt-BR", "Português (Brasil)"),
            new UiLanguageInfo("it", "Italiano"),
            new UiLanguageInfo("ru", "Русский"),
            new UiLanguageInfo("pl", "Polski"),
            new UiLanguageInfo("tr", "Türkçe"),
            new UiLanguageInfo("th", "ไทย"),
            new UiLanguageInfo("uk", "Українська"),
        };

        /// <summary>
        /// カルチャ名（"en-US"・"zh-TW"・"pt-PT" など）に当たる対応言語のカルチャ名。無ければ null。
        /// ICU のカルチャのデータに頼らず、名前だけで判定する（WebAssembly でも同じ結果になるように）。
        /// </summary>
        public static string Match(string cultureName)
        {
            var name = (cultureName ?? "").Trim().Replace('_', '-');
            if (name == "")
            {
                return null;
            }
            var exact = Supported.FirstOrDefault(x => string.Equals(x.Code, name, StringComparison.OrdinalIgnoreCase));
            if (exact != null)
            {
                return exact.Code;
            }
            var parts = name.Split('-');
            var language = parts[0].ToLowerInvariant();
            if (language == "zh")
            {
                // 繁体字の地域（台湾・香港・マカオ）か Hant の指定なら繁体字、それ以外は簡体字
                var traditional = parts.Skip(1).Any(x => x.Equals("Hant", StringComparison.OrdinalIgnoreCase)
                    || x.Equals("TW", StringComparison.OrdinalIgnoreCase)
                    || x.Equals("HK", StringComparison.OrdinalIgnoreCase)
                    || x.Equals("MO", StringComparison.OrdinalIgnoreCase));
                return traditional ? "zh-Hant" : "zh-Hans";
            }
            // 地域違い（en-GB・pt-PT など）は同じ言語の訳を使う
            return Supported.FirstOrDefault(x => x.Code.Split('-')[0].Equals(language, StringComparison.OrdinalIgnoreCase))?.Code;
        }

        /// <summary>
        /// 言語の指定（空・"auto"・カルチャ名）から使う言語のカルチャ名を決める。
        /// 空は日本語（OS の言語によらない）、"auto" は platformCultureName（OS・ブラウザの言語）に当たる言語で、訳が無ければ日本語。
        /// 解釈できない指定は日本語とし、known を false にする。
        /// </summary>
        public static string Resolve(string setting, string platformCultureName, out bool known)
        {
            known = true;
            var value = (setting ?? "").Trim();
            if (value == "")
            {
                return Default;
            }
            if (value.Equals(Auto, StringComparison.OrdinalIgnoreCase))
            {
                return Match(platformCultureName) ?? Default;
            }
            var matched = Match(value);
            known = matched != null;
            return matched ?? Default;
        }

        /// <summary><see cref="Resolve(string, string, out bool)"/></summary>
        public static string Resolve(string setting, string platformCultureName)
        {
            return Resolve(setting, platformCultureName, out _);
        }

        /// <summary>言語の選択欄に出す値（対応言語のカルチャ名か "auto"）。空・不明は日本語</summary>
        public static string SelectorValue(string setting)
        {
            var value = (setting ?? "").Trim();
            return value.Equals(Auto, StringComparison.OrdinalIgnoreCase) ? Auto : Match(value) ?? Default;
        }
    }

    /// <summary>表示言語</summary>
    public class UiLanguageInfo
    {
        public UiLanguageInfo(string code, string nativeName)
        {
            Code = code;
            NativeName = nativeName;
        }

        /// <summary>カルチャ名（resx の接尾辞）</summary>
        public string Code { get; }

        /// <summary>その言語での名前</summary>
        public string NativeName { get; }
    }
}
