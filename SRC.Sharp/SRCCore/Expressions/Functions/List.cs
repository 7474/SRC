using SRCCore.Lib;
using SRCCore.VB;
using System.Linq;

namespace SRCCore.Expressions.Functions
{
    public class List : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            str_result = string.Join(" ", Enumerable.Range(1, pcount)
                .Select(i => SRC.Expression.GetValueAsString(@params[i], is_term[i])));

            return ValueType.StringType;
        }
    }

    public class LLength : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";

            num_result = GeneralLib.ListLength(SRC.Expression.GetValueAsString(@params[1], is_term[1]));

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

    public class LIndex : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            num_result = 0d;

            str_result = GeneralLib.ListIndex(
                SRC.Expression.GetValueAsString(@params[1], is_term[1]),
                SRC.Expression.GetValueAsLong(@params[2], is_term[2]));

            // 全体が()で囲まれている場合は()を外す
            if (Strings.Left(str_result, 1) == "(" & Strings.Right(str_result, 1) == ")")
            {
                str_result = Strings.Mid(str_result, 2, Strings.Len(str_result) - 2);
            }

            if (etype == ValueType.StringType)
            {
                return ValueType.StringType;
            }
            else
            {
                num_result = GeneralLib.StrToDbl(str_result);
                return ValueType.NumericType;
            }
        }
    }

    public class LSearch : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
            var buf2 = SRC.Expression.GetValueAsString(@params[2], is_term[2]);
            var num = pcount < 3 ? 1 : SRC.Expression.GetValueAsLong(@params[3], is_term[3]);
            var num2 = GeneralLib.ListLength(buf);
            for (var i = num; i <= num2; i++)
            {
                if (GeneralLib.ListIndex(buf, i) == buf2)
                {
                    if (etype == ValueType.StringType)
                    {
                        str_result = SrcFormatter.Format(i);
                        return ValueType.StringType;
                    }
                    else
                    {
                        num_result = i;
                        return ValueType.NumericType;
                    }
                }
            }

            if (etype == ValueType.StringType)
            {
                str_result = "0";
                return ValueType.StringType;
            }
            else
            {
                num_result = 0d;
                return ValueType.NumericType;
            }
        }
    }
}