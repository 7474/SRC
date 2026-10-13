using System.Resources;

namespace SRCRender.Resources
{
    /// <summary>
    /// 画面の表示文字列（Resources/Strings.resx）。言語は CultureInfo.CurrentUICulture に従う（UiLanguage）。
    /// SRCCore.VB.Strings と名前が重なるため、利用側では Resources.Strings として参照する。
    /// </summary>
    internal static class Strings
    {
        private static readonly ResourceManager Rm = new ResourceManager("SRCRender.Resources.Strings", typeof(Strings).Assembly);

        public static string ErrorTitle => Rm.GetString(nameof(ErrorTitle));
        public static string DataErrorLine => Rm.GetString(nameof(DataErrorLine));
        public static string DataErrorInvalidData => Rm.GetString(nameof(DataErrorInvalidData));
        public static string Cancel => Rm.GetString(nameof(Cancel));
        public static string InputDefaultValue => Rm.GetString(nameof(InputDefaultValue));
        public static string SaveDialogTitle => Rm.GetString(nameof(SaveDialogTitle));
        public static string InvalidColor => Rm.GetString(nameof(InvalidColor));
        public static string AutoAdvance => Rm.GetString(nameof(AutoAdvance));
        public static string SelectFromTop => Rm.GetString(nameof(SelectFromTop));
        public static string SelectFromBottom => Rm.GetString(nameof(SelectFromBottom));
        public static string SortByName => Rm.GetString(nameof(SortByName));
        public static string SortByLevel => Rm.GetString(nameof(SortByLevel));
        public static string ViewMap => Rm.GetString(nameof(ViewMap));
        public static string Finish => Rm.GetString(nameof(Finish));
        public static string ConfigTitle => Rm.GetString(nameof(ConfigTitle));
        public static string ConfigBattleAnimation => Rm.GetString(nameof(ConfigBattleAnimation));
        public static string ConfigExtendedAnimation => Rm.GetString(nameof(ConfigExtendedAnimation));
        public static string ConfigWeaponAnimation => Rm.GetString(nameof(ConfigWeaponAnimation));
        public static string ConfigSpecialPowerAnimation => Rm.GetString(nameof(ConfigSpecialPowerAnimation));
        public static string ConfigMoveAnimation => Rm.GetString(nameof(ConfigMoveAnimation));
        public static string ConfigAutoMoveCursor => Rm.GetString(nameof(ConfigAutoMoveCursor));
        public static string ConfigShowSquareLine => Rm.GetString(nameof(ConfigShowSquareLine));
        public static string ConfigShowTurn => Rm.GetString(nameof(ConfigShowTurn));
        public static string ConfigKeepEnemyBGM => Rm.GetString(nameof(ConfigKeepEnemyBGM));
        public static string ConfigMP3Volume => Rm.GetString(nameof(ConfigMP3Volume));
    }
}
