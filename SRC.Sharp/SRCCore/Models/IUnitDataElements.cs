// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using System.Collections.Generic;

namespace SRCCore.Models
{
    public interface IUnitDataElements
    {
        IList<AbilityData> Abilities { get; }
        IList<FeatureData> Features { get; }
        IList<WeaponData> Weapons { get; }

        AbilityData AddAbility(string aname);
        void AddFeature(string fdef);
        WeaponData AddWeapon(string wname);
    }
}
