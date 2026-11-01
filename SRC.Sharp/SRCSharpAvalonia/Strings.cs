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
        public static string ViewMenu => Rm.GetString(nameof(ViewMenu));
        public static string SettingsMenu => Rm.GetString(nameof(SettingsMenu));
        public static string AssetSettingsMenu => Rm.GetString(nameof(AssetSettingsMenu));
        public static string AssetSettingsTitle => Rm.GetString(nameof(AssetSettingsTitle));
        public static string AssetSettingsDescription => Rm.GetString(nameof(AssetSettingsDescription));
        public static string ExtDataPathLabel => Rm.GetString(nameof(ExtDataPathLabel));
        public static string ExtDataPath2Label => Rm.GetString(nameof(ExtDataPath2Label));
        public static string SoundFontLabel => Rm.GetString(nameof(SoundFontLabel));
        public static string Browse => Rm.GetString(nameof(Browse));
        public static string Clear => Rm.GetString(nameof(Clear));
        public static string Save => Rm.GetString(nameof(Save));
        public static string FolderMissing => Rm.GetString(nameof(FolderMissing));
        public static string FolderIsAssetFolder => Rm.GetString(nameof(FolderIsAssetFolder));
        public static string FolderHasNoAssets => Rm.GetString(nameof(FolderHasNoAssets));
        public static string FolderFound => Rm.GetString(nameof(FolderFound));
        public static string FileMissing => Rm.GetString(nameof(FileMissing));
        public static string SoundFontAuto => Rm.GetString(nameof(SoundFontAuto));
        public static string SoundFontNotFound => Rm.GetString(nameof(SoundFontNotFound));
        public static string AssetSettingsApplyNote => Rm.GetString(nameof(AssetSettingsApplyNote));
    }
}
