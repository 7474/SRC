using System.Resources;

namespace SRCSharpAvalonia
{
    /// <summary>
    /// 画面の表示文字列（Resources/Strings.resx）。言語は CultureInfo.CurrentUICulture に従う（SRCRender.UiLanguage）。
    /// </summary>
    internal static class Strings
    {
        private static readonly ResourceManager Rm = new ResourceManager("SRCSharpAvalonia.Resources.Strings", typeof(Strings).Assembly);

        public static string SelectScenario => Rm.GetString(nameof(SelectScenario));
        public static string AllFiles => Rm.GetString(nameof(AllFiles));
        public static string Cancel => Rm.GetString(nameof(Cancel));
    }
}
