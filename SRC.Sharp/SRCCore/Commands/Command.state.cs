// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Units;

namespace SRCCore.Commands
{
    // ユニット＆マップコマンドの実行を行うモジュール
    public partial class Command
    {
        // 現在の選択状況を記録
        public void SaveSelections()
        {
            SavedStateStack.Push(SelectedState.Clone());
        }

        // 選択状況を復元
        public void RestoreSelections()
        {
            // スタックに積まれていない？
            if (SavedStateStack.Count == 0)
            {
                return;
            }

            // スタックトップから記録された選択状況を取り出す
            // 元の SRC が記録するのはユニット・ターゲット（イベント用を含む）・武器・反撃武器・防御方法・アビリティ・座標だけ。
            // コマンド・パイロット・合体技のパートナーなどは戻さない。
            // ユニットは記録した後に変形・分離などで形態が変わっていることがあるので、今の形態を選ぶ
            var saved = SavedStateStack.Pop();
            SelectedUnit = saved.SelectedUnit?.CurrentForm();
            SelectedTarget = saved.SelectedTarget?.CurrentForm();
            SelectedState.SelectedUnitForEvent = saved.SelectedUnitForEvent?.CurrentForm();
            SelectedState.SelectedTargetForEvent = saved.SelectedTargetForEvent?.CurrentForm();
            SelectedWeapon = saved.SelectedWeapon;
            SelectedWeaponName = saved.SelectedWeaponName;
            SelectedTWeapon = saved.SelectedTWeapon;
            SelectedTWeaponName = saved.SelectedTWeaponName;
            SelectedDefenseOption = saved.SelectedDefenseOption;
            SelectedAbility = saved.SelectedAbility;
            SelectedAbilityName = saved.SelectedAbilityName;
            SelectedX = saved.SelectedX;
            SelectedY = saved.SelectedY;
        }

        // 選択を入れ替える
        public void SwapSelections()
        {
            Unit u, t;
            int w, tw;
            string wname, twname;
            u = SelectedUnit;
            t = SelectedTarget;
            SelectedUnit = t;
            SelectedTarget = u;
            w = SelectedWeapon;
            tw = SelectedTWeapon;
            SelectedWeapon = tw;
            SelectedTWeapon = w;
            wname = SelectedWeaponName;
            twname = SelectedTWeaponName;
            SelectedWeaponName = twname;
            SelectedTWeaponName = wname;
        }
    }
}
