using SRCCore.Lib;
using System.Linq;

namespace SRCCore.Expressions.Functions
{
    public class Action : AUnitFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, Units.Unit unit, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;
            if (unit != null)
            {
                if (unit.Status == "出撃" || unit.Status == "格納")
                {
                    num_result = unit.Action;
                }
            }
            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class Area : AUnitFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, Units.Unit unit, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = unit?.Area ?? "";
            num_result = 0d;
            return ValueType.StringType;
        }
    }

    public class Condition : AUnitFunction
    {
        protected override int OptionArgCount => 1;
        protected override ValueType InvokeInternal(SRC SRC, Units.Unit unit, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            var cname = pcount == 1
                ? SRC.Expression.GetValueAsString(@params[1], is_term[1])
                : SRC.Expression.GetValueAsString(@params[2], is_term[2]);
            if (unit?.IsConditionSatisfied(cname) ?? false)
            {
                num_result = 1d;
            }
            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class CountItem : AUnitFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, Units.Unit unit, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            num_result = unit?.CountItem() ?? 0d;
            if (pcount == 1)
            {
                var pname = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                if (pname == "未装備")
                {
                    num_result = SRC.IList.List.Count(itm => itm.Unit == null && itm.Exist);
                }
            }
            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class CountPartner : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = SRC.Commands.SelectedPartners.Count;

            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class CountPilot : AUnitFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, Units.Unit unit, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = unit?.AllRawPilots.Count() ?? 0d;
            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class Damage : AUnitFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, Units.Unit unit, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 100d;

            if (unit != null)
            {
                num_result = (100 * (unit.MaxHP - unit.HP) / unit.MaxHP);
            }

            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class EN : AUnitFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, Units.Unit unit, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = unit?.EN ?? 0d;
            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class HP : AUnitFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, Units.Unit unit, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = unit?.HP ?? 0d;
            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class IsAvailable : AUnitFunction
    {
        protected override int OptionArgCount => 1;
        protected override ValueType InvokeInternal(SRC SRC, Units.Unit unit, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;
            var name = pcount == 1
                ? SRC.Expression.GetValueAsString(@params[1], is_term[1])
                : SRC.Expression.GetValueAsString(@params[2], is_term[2]);
            // エリアスが定義されている？
            if (SRC.ALDList.IsDefined(name))
            {
                name = SRC.ALDList.Item(name).ReplaceTypeName(name);
            }
            num_result = unit?.IsFeatureAvailable(name) ?? false ? 1d : 0d;
            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class IsEquiped : AUnitFunction
    {
        protected override int OptionArgCount => 1;
        protected override ValueType InvokeInternal(SRC SRC, Units.Unit unit, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;
            var name = pcount == 1
                ? SRC.Expression.GetValueAsString(@params[1], is_term[1])
                : SRC.Expression.GetValueAsString(@params[2], is_term[2]);
            num_result = unit?.IsEquiped(name) ?? false ? 1d : 0d;
            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class Item : AUnitFunction
    {
        protected override int OptionArgCount => 1;
        protected override ValueType InvokeInternal(SRC SRC, Units.Unit unit, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            var index = pcount == 1
                ? SRC.Expression.GetValueAsLong(@params[1], is_term[1])
                : SRC.Expression.GetValueAsLong(@params[2], is_term[2]);
            if (index <= 0)
            {
                return ValueType.StringType;
            }
            if (pcount == 2)
            {
                var pname = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                if (pname == "未装備")
                {
                    var items = SRC.IList.List.Where(itm => itm.Unit == null && itm.Exist).ToList();
                    str_result = items.Count > index - 1 ? items[index - 1].Name : "";
                    return ValueType.StringType;
                }
            }
            if (unit != null)
            {
                str_result = unit.ItemList.Count > index - 1 ? unit.ItemList[index - 1].Name : "";
            }
            return ValueType.StringType;
        }
    }

    public class ItemID : AUnitFunction
    {
        protected override int OptionArgCount => 1;
        protected override ValueType InvokeInternal(SRC SRC, Units.Unit unit, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            var index = pcount == 1
                ? SRC.Expression.GetValueAsLong(@params[1], is_term[1])
                : SRC.Expression.GetValueAsLong(@params[2], is_term[2]);
            if (index <= 0)
            {
                return ValueType.StringType;
            }
            if (pcount == 2)
            {
                var pname = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                if (pname == "未装備")
                {
                    var items = SRC.IList.List.Where(itm => itm.Unit == null && itm.Exist).ToList();
                    str_result = items.Count > index - 1 ? items[index - 1].ID : "";
                    return ValueType.StringType;
                }
            }
            if (unit != null)
            {
                str_result = unit.ItemList.Count > index - 1 ? unit.ItemList[index - 1].ID : "";
            }
            return ValueType.StringType;
        }
    }

    public class Partner : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            var index = SRC.Expression.GetValueAsLong(@params[1], is_term[1]);
            if (index == 0)
            {
                str_result = SRC.Event.SelectedUnitForEvent?.ID ?? "";
            }
            else if (1 <= index && index - 1 < SRC.Commands.SelectedPartners.Count)
            {
                str_result = SRC.Commands.SelectedPartners[index - 1].ID;
            }
            return ValueType.StringType;
        }
    }

    public class Party : AUnitFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, Units.Unit unit, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = unit?.Party0 ?? "";
            num_result = 0d;
            return ValueType.StringType;
        }
    }

    public class Pilot : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            Units.Unit unit = null;
            var pname = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
            if (SRC.UList.IsDefined(pname))
            {
                unit = SRC.UList.Item(pname);
            }

            var index = pcount == 1
                ? 0
                : SRC.Expression.GetValueAsLong(@params[2], is_term[2]);
            if (unit != null && unit.CountPilot() > 0)
            {
                if (index > 0)
                {
                    var pilots = unit.AllRawPilots.ToList();
                    str_result = pilots.Count > index - 1 ? pilots[index - 1].Name : "";
                }
                else
                {
                    str_result = unit.MainPilot().Name;
                }
            }
            return ValueType.StringType;
        }
    }

    public class PilotID : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            Units.Unit unit = null;
            var pname = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
            if (SRC.UList.IsDefined(pname))
            {
                unit = SRC.UList.Item(pname);
            }

            var index = pcount == 1
                ? 0
                : SRC.Expression.GetValueAsLong(@params[2], is_term[2]);
            if (unit != null && unit.CountPilot() > 0)
            {
                if (index > 0)
                {
                    var pilots = unit.AllRawPilots.ToList();
                    str_result = pilots.Count > index - 1 ? pilots[index - 1].ID : "";
                }
                else
                {
                    str_result = unit.MainPilot().ID;
                }
            }
            return ValueType.StringType;
        }
    }

    public class Rank : AUnitFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, Units.Unit unit, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = unit?.Rank ?? 0d;
            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class SpecialPower : AUnitFunction
    {
        protected override int OptionArgCount => 1;
        protected override ValueType InvokeInternal(SRC SRC, Units.Unit unit, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            var name = pcount == 1
                ? SRC.Expression.GetValueAsString(@params[1], is_term[1])
                : SRC.Expression.GetValueAsString(@params[2], is_term[2]);
            num_result = unit?.IsSpecialPowerInEffect(name) ?? false ? 1d : 0d;

            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class Mind : SpecialPower
    {
    }

    public class Status : AUnitFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, Units.Unit unit, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = unit?.Status ?? "";
            num_result = 0d;
            return ValueType.StringType;
        }
    }

    public class Unit : AUnitFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, Units.Unit unit, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = unit?.Name ?? "";
            num_result = 0d;
            return ValueType.StringType;
        }
    }

    public class UnitID : AUnitFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, Units.Unit unit, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = unit?.ID ?? "";
            num_result = 0d;
            return ValueType.StringType;
        }
    }
}
