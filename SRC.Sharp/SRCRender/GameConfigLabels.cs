using System.Collections.Generic;

namespace SRCRender
{
    /// <summary>
    /// ゲームの設定（設定変更の画面）の項目名。フロントエンドの設定の画面でも同じ訳を使う（Resources/Strings.resx）。
    /// </summary>
    public static class GameConfigLabels
    {
        public static string Title => Resources.Strings.ConfigTitle;
        public static string BattleAnimation => Resources.Strings.ConfigBattleAnimation;
        public static string ExtendedAnimation => Resources.Strings.ConfigExtendedAnimation;
        public static string WeaponAnimation => Resources.Strings.ConfigWeaponAnimation;
        public static string SpecialPowerAnimation => Resources.Strings.ConfigSpecialPowerAnimation;
        public static string MoveAnimation => Resources.Strings.ConfigMoveAnimation;
        public static string AutoMoveCursor => Resources.Strings.ConfigAutoMoveCursor;
        public static string ShowSquareLine => Resources.Strings.ConfigShowSquareLine;
        public static string ShowTurn => Resources.Strings.ConfigShowTurn;
        public static string KeepEnemyBGM => Resources.Strings.ConfigKeepEnemyBGM;
        public static string Volume => Resources.Strings.ConfigMP3Volume;
        public static string MessageSpeed => Resources.Strings.ConfigMessageSpeed;

        /// <summary>
        /// メッセージスピードの選択肢の名前。並びは <see cref="SRCCore.Config.MessageSpeeds.Items"/> と同じ
        /// （元の SRC の設定変更の画面の「手動送り」～「神の領域」）
        /// </summary>
        public static IReadOnlyList<string> MessageSpeedNames => new[]
        {
            Resources.Strings.ConfigMessageSpeedManual,
            Resources.Strings.ConfigMessageSpeedSlow,
            Resources.Strings.ConfigMessageSpeedNormal,
            Resources.Strings.ConfigMessageSpeedFast,
            Resources.Strings.ConfigMessageSpeedVeryFast,
            Resources.Strings.ConfigMessageSpeedInstant,
        };
    }
}
