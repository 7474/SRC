using SRCCore;
using SRCSharpTools.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SRCSharpTools
{
    /// <summary>
    /// 表示の言語の選択。--lang（どこに書いてもよい）か環境変数 SRCSHARP_LANG で ja・en・auto などを指定する（--lang が優先）。
    /// 既定は ja（OS の設定には従わない。CI などで英語の OS でも出力が変わらないように）。auto は OS の UI の言語
    /// </summary>
    public static class UiLanguage
    {
        public const string EnvironmentVariable = "SRCSHARP_LANG";
        public const string Option = "--lang";

        /// <summary>引数から --lang を取り除き、言語を CurrentUICulture に設定する。残りの引数を返す</summary>
        public static string[] Apply(string[] args)
        {
            string value = null;
            var rest = new List<string>();
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == Option)
                {
                    value = i + 1 < args.Length ? args[++i] : "";
                }
                else if (args[i].StartsWith(Option + "=", StringComparison.Ordinal))
                {
                    value = args[i].Substring(Option.Length + 1);
                }
                else
                {
                    rest.Add(args[i]);
                }
            }
            if (value == null)
            {
                value = Environment.GetEnvironmentVariable(EnvironmentVariable);
            }

            var culture = Resolve(value, out var known);
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            if (!known)
            {
                Console.Error.WriteLine(Strings.UnknownLanguage(value));
            }
            return rest.ToArray();
        }

        /// <summary>言語の指定から UI のカルチャを返す（SRCCore.UiLanguages）。auto は OS の UI の言語（訳が無ければ日本語）</summary>
        public static CultureInfo Resolve(string value, out bool known)
        {
            return CultureInfo.GetCultureInfo(UiLanguages.Resolve(value, CultureInfo.CurrentUICulture.Name, out known));
        }
    }
}
