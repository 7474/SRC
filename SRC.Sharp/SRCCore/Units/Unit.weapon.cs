// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

namespace SRCCore.Units
{
    public partial class Unit
    {
        // 武器
        // 1オフセット
        public UnitWeapon Weapon(int w)
        {
            var index = w - 1;
            return index >= 0 && index < WData.Count ? WData[index] : null;
        }

        public int CountWeapon()
        {
            return Weapons.Count;
        }
    }
}
