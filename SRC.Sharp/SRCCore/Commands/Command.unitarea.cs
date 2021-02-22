// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.VB;
using System;

namespace SRCCore.Commands
{
    public partial class Command
    {

        // 「移動範囲」コマンド
        private void ShowAreaInSpeedCommand()
        {
            SelectedCommand = "移動範囲";
            //// If MainWidth <> 15 Then
            //if (GUI.NewGUIMode)
            //{
            //    // MOD END MARGE
            //    Status.ClearUnitStatus();
            //}

            Map.AreaInSpeed(SelectedUnit);
            GUI.Center(SelectedUnit.x, SelectedUnit.y);
            GUI.MaskScreen();
            CommandState = "ターゲット選択";
        }
    }
}