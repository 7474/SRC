
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Extensions
{
    public static class UnitsExtension
    {
        public static bool SwapOnBardUnit(this Units.Units UList, Unit unloadUnit, Unit loadUnit)
        {
            foreach (Unit eu in UList.Items)
            {
                if (eu.UnitOnBoards.Any(x => x.ID == unloadUnit.ID))
                {
                    eu.UnloadUnit(unloadUnit.ID);
                    eu.LoadUnit(loadUnit);
                    return true;
                }
            }
            return false;
        }

        public static bool LoadSameUnit(this Units.Units UList, Unit baseUnit, Unit loadUnit)
        {
            foreach (Unit eu in UList.Items)
            {
                if (eu.UnitOnBoards.Any(x => x.ID == baseUnit.ID))
                {
                    eu.LoadUnit(loadUnit);
                    return true;
                }
            }
            return false;
        }

        public static bool Unload(this Units.Units UList, Unit unloadUnit)
        {
            foreach (Unit eu in UList.Items)
            {
                if (eu.UnitOnBoards.Any(x => x.ID == unloadUnit.ID))
                {
                    eu.UnloadUnit(unloadUnit.ID);
                    return true;
                }
            }
            return false;
        }
    }
}
