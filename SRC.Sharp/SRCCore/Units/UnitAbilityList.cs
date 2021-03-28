using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRCCore.Units
{
    public class UnitAbilityListItem
    {
        public UnitAbility Ability { get; set; }
        public bool CanUse { get; set; }
    }

    public class UnitAbilityList
    {
        public UnitAbilityList(AbilityListMode mode, Unit unit, Unit targetUnit = null)
        {
            Mode = mode;
            Items = unit.Abilities.Where(x => x.IdDisplayFor(Mode))
                .Select(x => new UnitAbilityListItem
                {
                    Ability = x,
                    CanUse = x.CanUseFor(mode, targetUnit),
                })
                .ToList();
        }

        public AbilityListMode Mode { get; set; }
        public IList<UnitAbilityListItem> Items { get; set; }
    }
}
