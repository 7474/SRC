// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.VB;
using System.Collections.Generic;

namespace SRCCore.Units
{
    // === アビリティ関連処理 ===
    public partial class Unit
    {
        // アビリティ
        public UnitAbility Ability(int a)
        {
            return Abilities[a];
        }

        // アビリティ総数
        public int CountAbility()
        {
            return Abilities.Count;
        }
    }
}
