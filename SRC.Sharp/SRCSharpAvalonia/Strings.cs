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
        public static string PacksLabel => Rm.GetString(nameof(PacksLabel));
        public static string PacksDescription => Rm.GetString(nameof(PacksDescription));
        public static string AddPack => Rm.GetString(nameof(AddPack));
        public static string OpenPacksFolder => Rm.GetString(nameof(OpenPacksFolder));
        public static string NoPacks => Rm.GetString(nameof(NoPacks));
        public static string PackHasNoAssets => Rm.GetString(nameof(PackHasNoAssets));
        public static string PackUnreadable => Rm.GetString(nameof(PackUnreadable));
        public static string PackCopyFailed => Rm.GetString(nameof(PackCopyFailed));
        public static string FileMenu => Rm.GetString(nameof(FileMenu));
        public static string OpenScenarioMenu => Rm.GetString(nameof(OpenScenarioMenu));
        public static string RestartMenu => Rm.GetString(nameof(RestartMenu));
        public static string ExitMenu => Rm.GetString(nameof(ExitMenu));
        public static string HelpMenu => Rm.GetString(nameof(HelpMenu));
        public static string HelpContentsMenu => Rm.GetString(nameof(HelpContentsMenu));
        public static string WebsiteMenu => Rm.GetString(nameof(WebsiteMenu));
        public static string AboutMenu => Rm.GetString(nameof(AboutMenu));
        public static string ConfirmTitle => Rm.GetString(nameof(ConfirmTitle));
        public static string ConfirmDiscardProgress => Rm.GetString(nameof(ConfirmDiscardProgress));
        public static string Yes => Rm.GetString(nameof(Yes));
        public static string No => Rm.GetString(nameof(No));
        public static string AboutTitle => Rm.GetString(nameof(AboutTitle));
        public static string AboutVersion => Rm.GetString(nameof(AboutVersion));
        public static string AboutLicense => Rm.GetString(nameof(AboutLicense));
        public static string SettingsTitle => Rm.GetString(nameof(SettingsTitle));
        public static string SettingsMenuItem => Rm.GetString(nameof(SettingsMenuItem));
        public static string AssetsTab => Rm.GetString(nameof(AssetsTab));
        public static string DisplayTab => Rm.GetString(nameof(DisplayTab));
        public static string LogTab => Rm.GetString(nameof(LogTab));
        public static string LanguageLabel => Rm.GetString(nameof(LanguageLabel));
        public static string LanguageAuto => Rm.GetString(nameof(LanguageAuto));
        public static string DisplayScaleNote => Rm.GetString(nameof(DisplayScaleNote));
        public static string LogLevelLabel => Rm.GetString(nameof(LogLevelLabel));
        public static string LogLevelDefault => Rm.GetString(nameof(LogLevelDefault));
        public static string LogLevelFixedNote => Rm.GetString(nameof(LogLevelFixedNote));
        public static string LogFileLabel => Rm.GetString(nameof(LogFileLabel));
        public static string LogFolderLabel => Rm.GetString(nameof(LogFolderLabel));
        public static string LogFolderDefault => Rm.GetString(nameof(LogFolderDefault));
        public static string LogFolderWillBeCreated => Rm.GetString(nameof(LogFolderWillBeCreated));
        public static string LogRetainDaysLabel => Rm.GetString(nameof(LogRetainDaysLabel));
        public static string OpenLogFolder => Rm.GetString(nameof(OpenLogFolder));
        public static string UsageMenu => Rm.GetString(nameof(UsageMenu));
        public static string SettingsApplyNote => Rm.GetString(nameof(SettingsApplyNote));
        public static string GameTab => Rm.GetString(nameof(GameTab));
        public static string BgmTab => Rm.GetString(nameof(BgmTab));
        public static string DeveloperTab => Rm.GetString(nameof(DeveloperTab));
        public static string DeveloperSettingsMenu => Rm.GetString(nameof(DeveloperSettingsMenu));
        public static string AutoDefenseLabel => Rm.GetString(nameof(AutoDefenseLabel));
        public static string BgmDescriptionText => Rm.GetString(nameof(BgmDescriptionText));
        public static string BgmImport => Rm.GetString(nameof(BgmImport));
        public static string BgmImported => Rm.GetString(nameof(BgmImported));
        public static string BgmImportNone => Rm.GetString(nameof(BgmImportNone));
        public static string BgmOpening => Rm.GetString(nameof(BgmOpening));
        public static string BgmMap1 => Rm.GetString(nameof(BgmMap1));
        public static string BgmMap2 => Rm.GetString(nameof(BgmMap2));
        public static string BgmMap3 => Rm.GetString(nameof(BgmMap3));
        public static string BgmMap4 => Rm.GetString(nameof(BgmMap4));
        public static string BgmMap5 => Rm.GetString(nameof(BgmMap5));
        public static string BgmMap6 => Rm.GetString(nameof(BgmMap6));
        public static string BgmBriefing => Rm.GetString(nameof(BgmBriefing));
        public static string BgmIntermission => Rm.GetString(nameof(BgmIntermission));
        public static string BgmSubtitle => Rm.GetString(nameof(BgmSubtitle));
        public static string BgmEnd => Rm.GetString(nameof(BgmEnd));
        public static string BgmBattle => Rm.GetString(nameof(BgmBattle));
        public static string DeveloperDescription => Rm.GetString(nameof(DeveloperDescription));
        public static string DebugModeLabel => Rm.GetString(nameof(DebugModeLabel));
        public static string DebugModeNote => Rm.GetString(nameof(DebugModeNote));
        public static string CompatibleReadLabel => Rm.GetString(nameof(CompatibleReadLabel));
        public static string CompatibleWriteLabel => Rm.GetString(nameof(CompatibleWriteLabel));
        public static string CompatibleNote => Rm.GetString(nameof(CompatibleNote));
        public static string ImageBufferSizeLabel => Rm.GetString(nameof(ImageBufferSizeLabel));
        public static string MaxImageBufferByteSizeLabel => Rm.GetString(nameof(MaxImageBufferByteSizeLabel));
        public static string KeepStretchedImageLabel => Rm.GetString(nameof(KeepStretchedImageLabel));
        public static string ImageBufferNote => Rm.GetString(nameof(ImageBufferNote));
    }
}
