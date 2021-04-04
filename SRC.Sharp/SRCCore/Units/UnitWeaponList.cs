using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRCCore.Units
{
    public class UnitWeaponListItem
    {
        public UnitWeapon Weapon { get; set; }
        public bool CanUse { get; set; }
    }

    public class UnitWeaponList
    {
        public UnitWeaponList(WeaponListMode mode, Unit unit, Unit targetUnit = null)
        {
            Mode = mode;
            Items = unit.Weapons.Where(x => x.IsDisplayFor(Mode))
                .Select(x => new UnitWeaponListItem
                {
                    Weapon = x,
                    CanUse = x.CanUseFor(mode, targetUnit),
                })
                .ToList();
        }

        public WeaponListMode Mode { get; set; }
        public IList<UnitWeaponListItem> Items { get; set; }
    }
}
