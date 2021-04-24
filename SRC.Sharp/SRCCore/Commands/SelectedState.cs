using SRCCore.Pilots;
using SRCCore.Units;

namespace SRCCore.Commands
{
    public class SelectedState
    {
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

        public SelectedState Clone()
        {
            var clone = (SelectedState)MemberwiseClone();
            // TODO SelectedPartners
            return clone;
        }
    }
}
