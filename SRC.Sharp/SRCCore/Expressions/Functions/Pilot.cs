using SRCCore.Lib;
using System.Linq;

namespace SRCCore.Expressions.Functions
{
    //パイロット情報関数
    //Levelパイロットのレベル
    //Moraleパイロットの気力
    //Planaパイロットの残り霊力
    //Relationパイロット間の信頼度
    //Skillパイロットが指定した特殊能力を持っているか
    //SPパイロットの残りＳＰ

    public class Level : APilotFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, Pilots.Pilot pilot, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = pilot?.Level ?? 0d;

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

    public class Morale : APilotFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, Pilots.Pilot pilot, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = pilot?.Morale ?? 0d;

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

    public class Plana : APilotFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, Pilots.Pilot pilot, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = pilot?.Plana ?? 0d;

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

    public class Relation : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            var pname = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
            var pname2 = SRC.Expression.GetValueAsString(@params[2], is_term[2]);
            var p1 = SRC.PList.Item(pname);
            var p2 = SRC.PList.Item(pname2);
            if (p1 != null && p2 != null)
            {
                num_result = SRC.Expression.GetValueAsLong("関係:" + p1.Name + ":" + p2.Name);
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

    public class Skill : APilotFunction
    {
        protected override int OptionArgCount => 1;
        protected override ValueType InvokeInternal(SRC SRC, Pilots.Pilot pilot, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
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
            num_result = pilot?.SkillLevel(name) ?? 0d;
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

    public class SP : APilotFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, Pilots.Pilot pilot, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = pilot?.SP ?? 0d;

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
}