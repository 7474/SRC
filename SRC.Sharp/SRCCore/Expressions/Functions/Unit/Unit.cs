using SRCCore.Lib;
using System.Linq;

namespace SRCCore.Expressions.Functions
{
    public class Action : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;
            switch (pcount)
            {
                case 1:
                    {
                        var pname = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                        if (SRC.UList.IsDefined2(pname))
                        {
                            num_result = SRC.UList.Item2(pname).Action;
                        }
                        else if (SRC.PList.IsDefined(pname))
                        {
                            // パイロット名で指定した場合のみ出撃中かどうかを判定する
                            var u = SRC.PList.Item(pname).Unit;
                            if (u != null)
                            {
                                if (u.Status == "出撃" || u.Status == "格納")
                                {
                                    num_result = u.Action;
                                }
                            }
                        }
                        break;
                    }

                case 0:
                    {
                        if (SRC.Event.SelectedUnitForEvent != null)
                        {
                            num_result = SRC.Event.SelectedUnitForEvent.Action;
                        }
                        break;
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
            // パイロット名で指定したときはパイロットの陣営（ユニットに乗っていなくても返す。VB 版と同じ）
            if (pcount == 1)
            {
                var pname = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                if (!SRC.UList.IsDefined2(pname) && SRC.PList.IsDefined(pname))
                {
                    str_result = SRC.PList.Item(pname).Party;
                }
            }
            return ValueType.StringType;
        }
    }

    public class Pilot : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            num_result = 0d;
            var p = PilotFunctionHelper.FindPilot(SRC, @params, pcount, is_term);
            str_result = p?.Name ?? "";
            return ValueType.StringType;
        }
    }

    public class PilotID : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            num_result = 0d;
            var p = PilotFunctionHelper.FindPilot(SRC, @params, pcount, is_term);
            str_result = p?.ID ?? "";
            return ValueType.StringType;
        }
    }

    internal static class PilotFunctionHelper
    {
        // Pilot関数、PilotID関数の対象となるパイロットを返す
        public static Pilots.Pilot FindPilot(SRC SRC, string[] @params, int pcount, bool[] is_term)
        {
            switch (pcount)
            {
                case 2:
                    {
                        var uname = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                        if (SRC.UList.IsDefined(uname))
                        {
                            var i = SRC.Expression.GetValueAsLong(@params[2], is_term[2]);
                            return NthPilot(SRC.UList.Item(uname), i);
                        }
                        return null;
                    }

                case 1:
                    {
                        var uname = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                        if (GeneralLib.IsNumber(uname))
                        {
                            // 選択中のユニットのi番目のパイロット
                            if (SRC.Event.SelectedUnitForEvent != null)
                            {
                                var i = (int)System.Math.Round(VB.Conversions.ToDouble(uname), System.MidpointRounding.ToEven);
                                return NthPilot(SRC.Event.SelectedUnitForEvent, i);
                            }
                        }
                        else if (SRC.UList.IsDefined(uname))
                        {
                            var u = SRC.UList.Item(uname);
                            if (u.CountPilot() > 0)
                            {
                                return u.MainPilot();
                            }
                        }
                        return null;
                    }

                case 0:
                    {
                        // 選択中のユニットのメインパイロット
                        var u = SRC.Event.SelectedUnitForEvent;
                        if (u != null && u.CountPilot() > 0)
                        {
                            return u.MainPilot();
                        }
                        return null;
                    }
            }
            return null;
        }

        // パイロット、サポートパイロットの順で数えたi番目のパイロット
        private static Pilots.Pilot NthPilot(Units.Unit u, int i)
        {
            if (0 < i && i <= u.CountPilot())
            {
                return u.Pilots[i - 1];
            }
            else if (u.CountPilot() < i && i <= u.CountPilot() + u.CountSupport())
            {
                return u.SupportPilots[i - u.CountPilot() - 1];
            }
            return null;
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
