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

            SelectedState = SavedStateStack.Pop();
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
