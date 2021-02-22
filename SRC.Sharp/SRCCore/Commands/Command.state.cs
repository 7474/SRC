// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

namespace SRCCore.Commands
{
    // ユニット＆マップコマンドの実行を行うモジュール
    public partial class Command
    {
        //// 現在の選択状況を記録
        //public void SaveSelections()
        //{
        //    // スタックのインデックスを増やす
        //    SelectionStackIndex = (SelectionStackIndex + 1);

        //    // スタック領域確保
        //    Array.Resize(ref SavedSelectedUnit, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedTarget, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedUnitForEvent, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedTargetForEvent, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedWeapon, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedWeaponName, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedTWeapon, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedTWeaponName, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedDefenseOption, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedAbility, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedAbilityName, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedX, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedY, SelectionStackIndex + 1);

        //    // 確保した領域に選択状況を記録
        //    SavedSelectedUnit[SelectionStackIndex] = SelectedUnit;
        //    SavedSelectedTarget[SelectionStackIndex] = SelectedTarget;
        //    SavedSelectedUnitForEvent[SelectionStackIndex] = Event_Renamed.SelectedUnitForEvent;
        //    SavedSelectedTargetForEvent[SelectionStackIndex] = Event_Renamed.SelectedTargetForEvent;
        //    SavedSelectedWeapon[SelectionStackIndex] = SelectedWeapon;
        //    SavedSelectedWeaponName[SelectionStackIndex] = SelectedWeaponName;
        //    SavedSelectedTWeapon[SelectionStackIndex] = SelectedTWeapon;
        //    SavedSelectedTWeaponName[SelectionStackIndex] = SelectedTWeaponName;
        //    SavedSelectedDefenseOption[SelectionStackIndex] = SelectedDefenseOption;
        //    SavedSelectedAbility[SelectionStackIndex] = SelectedAbility;
        //    SavedSelectedAbilityName[SelectionStackIndex] = SelectedAbilityName;
        //    SavedSelectedX[SelectionStackIndex] = SelectedX;
        //    SavedSelectedY[SelectionStackIndex] = SelectedY;
        //}

        //// 選択状況を復元
        //public void RestoreSelections()
        //{
        //    // スタックに積まれていない？
        //    if (SelectionStackIndex == 0)
        //    {
        //        return;
        //    }

        //    // スタックトップから記録された選択状況を取り出す
        //    if (SavedSelectedUnit[SelectionStackIndex] is object)
        //    {
        //        SelectedUnit = SavedSelectedUnit[SelectionStackIndex].CurrentForm();
        //    }
        //    else
        //    {
        //        // UPGRADE_NOTE: オブジェクト SelectedUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
        //        SelectedUnit = null;
        //    }

        //    if (SavedSelectedTarget[SelectionStackIndex] is object)
        //    {
        //        SelectedTarget = SavedSelectedTarget[SelectionStackIndex].CurrentForm();
        //    }
        //    else
        //    {
        //        // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
        //        SelectedTarget = null;
        //    }

        //    if (SavedSelectedUnitForEvent[SelectionStackIndex] is object)
        //    {
        //        Event_Renamed.SelectedUnitForEvent = SavedSelectedUnitForEvent[SelectionStackIndex].CurrentForm();
        //    }
        //    else
        //    {
        //        // UPGRADE_NOTE: オブジェクト SelectedUnitForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
        //        Event_Renamed.SelectedUnitForEvent = null;
        //    }

        //    if (SavedSelectedTargetForEvent[SelectionStackIndex] is object)
        //    {
        //        Event_Renamed.SelectedTargetForEvent = SavedSelectedTargetForEvent[SelectionStackIndex].CurrentForm();
        //    }
        //    else
        //    {
        //        // UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
        //        Event_Renamed.SelectedTargetForEvent = null;
        //    }

        //    SelectedWeapon = SavedSelectedWeapon[SelectionStackIndex];
        //    SelectedWeaponName = SavedSelectedWeaponName[SelectionStackIndex];
        //    SelectedTWeapon = SavedSelectedTWeapon[SelectionStackIndex];
        //    SelectedTWeaponName = SavedSelectedTWeaponName[SelectionStackIndex];
        //    SelectedDefenseOption = SavedSelectedDefenseOption[SelectionStackIndex];
        //    SelectedAbility = SavedSelectedAbility[SelectionStackIndex];
        //    SelectedAbilityName = SavedSelectedAbilityName[SelectionStackIndex];
        //    SelectedX = SavedSelectedX[SelectionStackIndex];
        //    SelectedY = SavedSelectedY[SelectionStackIndex];

        //    // スタックのインデックスを１減らす
        //    SelectionStackIndex = (SelectionStackIndex - 1);

        //    // スタックの領域を開放
        //    Array.Resize(ref SavedSelectedUnit, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedTarget, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedUnitForEvent, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedTargetForEvent, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedWeapon, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedWeaponName, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedTWeapon, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedTWeaponName, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedDefenseOption, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedAbility, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedAbilityName, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedX, SelectionStackIndex + 1);
        //    Array.Resize(ref SavedSelectedY, SelectionStackIndex + 1);
        //}

        //// 選択を入れ替える
        //public void SwapSelections()
        //{
        //    Unit u, t;
        //    int w, tw;
        //    string wname, twname;
        //    u = SelectedUnit;
        //    t = SelectedTarget;
        //    SelectedUnit = t;
        //    SelectedTarget = u;
        //    w = SelectedWeapon;
        //    tw = SelectedTWeapon;
        //    SelectedWeapon = tw;
        //    SelectedTWeapon = w;
        //    wname = SelectedWeaponName;
        //    twname = SelectedTWeaponName;
        //    SelectedWeaponName = twname;
        //    SelectedTWeaponName = wname;
        //}
    }
}