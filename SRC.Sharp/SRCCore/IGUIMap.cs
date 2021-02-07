// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRC.Core.Units;

namespace SRC.Core
{
    // マップのGUIインタフェース
    public interface IGUIMap
    {
        void InitMapSize(int w, int h);
        void SetMapSize(int w, int h);
    }
}
