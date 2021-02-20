using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.Commands
{
    public partial class Command
    {
        // 現在のコマンドの進行状況
        public static string CommandState;

        // クリック待ちモード
        public static bool WaitClickMode;
        // 閲覧モード
        public static bool ViewMode;

        // マップコマンドラベルのリスト
        private static string[] MapCommandLabelList = new string[11];
        // ユニットコマンドラベルのリスト
        private static string[] UnitCommandLabelList = new string[11];

        // 現在選択されているもの
        public static Unit SelectedUnit; // ユニット
        public static string SelectedCommand; // コマンド
        public static Unit SelectedTarget; // ターゲット
        public static short SelectedX; // Ｘ座標
        public static short SelectedY; // Ｙ座標
        public static short SelectedWeapon; // 武器
        public static string SelectedWeaponName;
        public static short SelectedTWeapon; // 反撃武器
        public static string SelectedTWeaponName;
        public static string SelectedDefenseOption; // 防御方法
        public static short SelectedAbility; // アビリティ
        public static string SelectedAbilityName;
        public static Pilot SelectedPilot; // パイロット
        public static short SelectedItem; // リストボックス中のアイテム
        public static string SelectedSpecialPower; // スペシャルパワー
        public static Unit[] SelectedPartners; // 合体技のパートナー
                                               // ADD START MARGE
        public static short SelectedUnitMoveCost; // 選択したユニットの移動力消費量
                                                  // ADD END MARGE

        // 選択状況の記録用変数
        public static short SelectionStackIndex;
        public static Unit[] SavedSelectedUnit;
        public static Unit[] SavedSelectedTarget;
        public static Unit[] SavedSelectedUnitForEvent;
        public static Unit[] SavedSelectedTargetForEvent;
        public static short[] SavedSelectedWeapon;
        public static string[] SavedSelectedWeaponName;
        public static short[] SavedSelectedTWeapon;
        public static string[] SavedSelectedTWeaponName;
        public static string[] SavedSelectedDefenseOption;
        public static short[] SavedSelectedAbility;
        public static string[] SavedSelectedAbilityName;
        public static short[] SavedSelectedX;
        public static short[] SavedSelectedY;

        // 援護を使うかどうか
        public static bool UseSupportAttack;
        public static bool UseSupportGuard;

        // 「味方スペシャルパワー実行」を使ってスペシャルパワーを使用するかどうか
        private static bool WithDoubleSPConsumption;

        // 攻撃を行うユニット
        public static Unit AttackUnit;
        // 援護攻撃を行うユニット
        public static Unit SupportAttackUnit;
        // 援護防御を行うユニット
        public static Unit SupportGuardUnit;
        // 援護防御を行うユニットのＨＰ値
        public static double SupportGuardUnitHPRatio;
        // 援護防御を行うユニット(反撃時)
        public static Unit SupportGuardUnit2;
        // 援護防御を行うユニットのＨＰ値(反撃時)
        public static double SupportGuardUnitHPRatio2;

        // 移動前のユニットの情報
        private static short PrevUnitX;
        private static short PrevUnitY;
        private static string PrevUnitArea;
        private static short PrevUnitEN;
        private static string PrevCommand;

        // 移動したユニットの情報
        public static Unit MovedUnit;
        public static short MovedUnitSpeed;
    }
}
