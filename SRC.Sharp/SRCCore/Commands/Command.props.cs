using SRCCore.Events;
using SRCCore.Pilots;
using SRCCore.Units;
using System.Collections.Generic;

namespace SRCCore.Commands
{
    public partial class Command
    {
        // 現在のコマンドの進行状況
        public string CommandState;

        // クリック待ちモード
        public bool WaitClickMode;
        // 閲覧モード
        public bool ViewMode;

        // マップコマンドラベルのリスト
        private IDictionary<int, LabelData> MapCommandLabelList = new Dictionary<int, LabelData>();
        // ユニットコマンドラベルのリスト
        private IDictionary<int, LabelData> UnitCommandLabelList = new Dictionary<int, LabelData>();

        // 現在選択されているもの
        public Unit SelectedUnit; // ユニット
        public string SelectedCommand; // コマンド
        public Unit SelectedTarget; // ターゲット
        public int SelectedX; // Ｘ座標
        public int SelectedY; // Ｙ座標
        public int SelectedWeapon; // 武器
        public string SelectedWeaponName;
        public int SelectedTWeapon; // 反撃武器
        public string SelectedTWeaponName;
        public string SelectedDefenseOption; // 防御方法
        public int SelectedAbility; // アビリティ
        public string SelectedAbilityName;
        public Pilot SelectedPilot; // パイロット
        public int SelectedItem; // リストボックス中のアイテム
        public string SelectedSpecialPower; // スペシャルパワー
        public Unit[] SelectedPartners; // 合体技のパートナー
        public int SelectedUnitMoveCost; // 選択したユニットの移動力消費量

        // 選択状況の記録用変数
        public int SelectionStackIndex;
        public Unit[] SavedSelectedUnit;
        public Unit[] SavedSelectedTarget;
        public Unit[] SavedSelectedUnitForEvent;
        public Unit[] SavedSelectedTargetForEvent;
        public int[] SavedSelectedWeapon;
        public string[] SavedSelectedWeaponName;
        public int[] SavedSelectedTWeapon;
        public string[] SavedSelectedTWeaponName;
        public string[] SavedSelectedDefenseOption;
        public int[] SavedSelectedAbility;
        public string[] SavedSelectedAbilityName;
        public int[] SavedSelectedX;
        public int[] SavedSelectedY;

        // 援護を使うかどうか
        public bool UseSupportAttack;
        public bool UseSupportGuard;

        // 「味方スペシャルパワー実行」を使ってスペシャルパワーを使用するかどうか
        private bool WithDoubleSPConsumption;

        // 攻撃を行うユニット
        public Unit AttackUnit;
        // 援護攻撃を行うユニット
        public Unit SupportAttackUnit;
        // 援護防御を行うユニット
        public Unit SupportGuardUnit;
        // 援護防御を行うユニットのＨＰ値
        public double SupportGuardUnitHPRatio;
        // 援護防御を行うユニット(反撃時)
        public Unit SupportGuardUnit2;
        // 援護防御を行うユニットのＨＰ値(反撃時)
        public double SupportGuardUnitHPRatio2;

        // 移動前のユニットの情報
        private int PrevUnitX;
        private int PrevUnitY;
        private string PrevUnitArea;
        private int PrevUnitEN;
        private string PrevCommand;

        // 移動したユニットの情報
        public Unit MovedUnit;
        public int MovedUnitSpeed;
    }
}
