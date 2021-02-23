// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Pilots;
using SRCCore.Units;

namespace SRCCore
{
    // ステータス表示のGUIインタフェース
    public interface IGUIStatus
    {
        Unit DisplayedUnit { get; set; }
        Pilot DisplayedPilot { get; set; }

        // 現在の状況をステータスウィンドウに表示
        void DisplayGlobalStatus();

        // ユニットステータスを表示
        // pindexはステータス表示に使うパイロットを指定
        void DisplayUnitStatus(Unit u, int pindex = 0);

        // 指定されたパイロットのステータスをステータスウィンドウに表示
        void DisplayPilotStatus(Pilot p);

        // 指定したマップ座標にいるユニットのステータスをステータスウィンドウに表示
        void InstantUnitStatusDisplay(int X, int Y);

        // ステータスウィンドウをクリア
        void ClearUnitStatus();
    }
}
