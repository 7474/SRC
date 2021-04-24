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

        // 現在選択されているもの
        private SelectedState SelectedState = new SelectedState();
        public Unit SelectedUnit { get => SelectedState.SelectedUnit; set { SelectedState.SelectedUnit = value; } } // ユニット
        public string SelectedCommand { get => SelectedState.SelectedCommand; set { SelectedState.SelectedCommand = value; } } // コマンド
        public Unit SelectedTarget { get => SelectedState.SelectedTarget; set { SelectedState.SelectedTarget = value; } } // ターゲット
        public int SelectedX { get => SelectedState.SelectedX; set { SelectedState.SelectedX = value; } } // Ｘ座標
        public int SelectedY { get => SelectedState.SelectedY; set { SelectedState.SelectedY = value; } } // Ｙ座標
        public int SelectedWeapon { get => SelectedState.SelectedWeapon; set { SelectedState.SelectedWeapon = value; } } // 武器
        public string SelectedWeaponName { get => SelectedState.SelectedWeaponName; set { SelectedState.SelectedWeaponName = value; } }
        public int SelectedTWeapon { get => SelectedState.SelectedTWeapon; set { SelectedState.SelectedTWeapon = value; } } // 反撃武器
        public string SelectedTWeaponName { get => SelectedState.SelectedTWeaponName; set { SelectedState.SelectedTWeaponName = value; } }
        public string SelectedDefenseOption { get => SelectedState.SelectedDefenseOption; set { SelectedState.SelectedDefenseOption = value; } } // 防御方法
        public int SelectedAbility { get => SelectedState.SelectedAbility; set { SelectedState.SelectedAbility = value; } } // アビリティ
        public string SelectedAbilityName { get => SelectedState.SelectedAbilityName; set { SelectedState.SelectedAbilityName = value; } }
        public Pilot SelectedPilot { get => SelectedState.SelectedPilot; set { SelectedState.SelectedPilot = value; } } // パイロット
        public int SelectedItem { get => SelectedState.SelectedItem; set { SelectedState.SelectedItem = value; } } // リストボックス中のアイテム
        public string SelectedSpecialPower { get => SelectedState.SelectedSpecialPower; set { SelectedState.SelectedSpecialPower = value; } } // スペシャルパワー
        public Unit[] SelectedPartners { get => SelectedState.SelectedPartners; set { SelectedState.SelectedPartners = value; } } // 合体技のパートナー
        public int SelectedUnitMoveCost { get => SelectedState.SelectedUnitMoveCost; set { SelectedState.SelectedUnitMoveCost = value; } } // 選択したユニットの移動力消費量

        // 選択状況の記録用変数
        private Stack<SelectedState> SavedState = new Stack<SelectedState>();

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
