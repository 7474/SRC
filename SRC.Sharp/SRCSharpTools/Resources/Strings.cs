using System.Linq;
using System.Globalization;
using System.Resources;

namespace SRCSharpTools.Resources
{
    /// <summary>
    /// 表示する文言（Strings.resx が既定の日本語、Strings.en.resx が英語）。
    /// CultureInfo.CurrentUICulture で選ぶ。SARIF などの機械向けの出力には使わない
    /// </summary>
    internal static class Strings
    {
        private static readonly ResourceManager ResourceManager = new ResourceManager("SRCSharpTools.Resources.Strings", typeof(Strings).Assembly);

        private static string Get(string name)
        {
            return ResourceManager.GetString(name, CultureInfo.CurrentUICulture) ?? name;
        }

        private static string Format(string name, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, Get(name), args);
        }

        public static string Usage => Get(nameof(Usage));

        public static string PlayRequiresOneEventFile => Get(nameof(PlayRequiresOneEventFile));

        public static string UnknownCommand(string command) => Format(nameof(UnknownCommand), command);

        public static string UnknownLanguage(string language) => Format(nameof(UnknownLanguage), language,
            string.Join(", ", SRCCore.UiLanguages.Supported.Select(x => x.Code).Append(SRCCore.UiLanguages.Auto)));

        public static string HelpNotBundled => Get(nameof(HelpNotBundled));

        public static string HelpNotFound(string topic) => Format(nameof(HelpNotFound), topic);

        public static string HelpAmbiguous(string topic) => Format(nameof(HelpAmbiguous), topic);

        public static string HelpExported(int count, string directory) => Format(nameof(HelpExported), count, directory);

        public static string HelpVectorUnavailable => Get(nameof(HelpVectorUnavailable));

        public static string UnknownSearchMethod(string method) => Format(nameof(UnknownSearchMethod), method);

        public static string LintSummary(int files, int problems) => Format(nameof(LintSummary), files, problems);
    }
}
