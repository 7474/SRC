using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Linq;

namespace SRCCore.Expressions.Functions
{
    // Format(数値,書式)
    // InStr(文字列１,文字列２[,検索開始位置])
    // InStrRev(文字列１,文字列２[,検索開始位置])
    // IsNumeric(文字列)
    // Left(文字列,文字数)
    // Len(文字列)
    // LSet(文字列,文字数)
    // Mid(文字列,位置[,文字数])
    // Replace(文字列１,文字列２,文字列３[,開始位置[,文字数]])
    // Right(文字列,文字数)
    // RSet(文字列,文字数)
    // StrComp(文字列１,文字列２)
    // String(回数,文字列)
    // Wide(文字列)
    // InStrB(文字列１,文字列２[,検索開始位置])
    // InStrRevB(文字列１,文字列２[,検索開始位置])
    // LenB(文字列)
    // LeftB(文字列,バイト数)
    // MidB(文字列,位置[,バイト数])
    // RightB(文字列,バイト数)

    public class Format : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // XXX 元は String -> String フォーマット
            // XXX 書式は多分合っていないのでヘルプ更新したほうが良いかも
            str_result = SrcFormatter.Format(SRC.Expression.GetValueAsDouble(@params[1], is_term[1]), SRC.Expression.GetValueAsString(@params[2], is_term[2]));
            if (etype == ValueType.NumericType)
            {
                num_result = GeneralLib.StrToDbl(str_result);
                return ValueType.NumericType;
            }
            else
            {
                return ValueType.StringType;
            }
        }
    }

    public class InStr : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            if (pcount == 2)
            {
                num_result = Strings.InStr(
                    SRC.Expression.GetValueAsString(@params[1], is_term[1]),
                    SRC.Expression.GetValueAsString(@params[2], is_term[2]));
            }
            else
            {
                // params(3)が指定されている場合は、それを検索開始位置似設定
                // VBのInStrは引数1が開始位置になりますが、現仕様との兼ね合いを考え、
                // eve上では引数3に設定するようにしています
                num_result = Strings.InStr(
                    SRC.Expression.GetValueAsLong(@params[3], is_term[3]),
                    SRC.Expression.GetValueAsString(@params[1], is_term[1]),
                    SRC.Expression.GetValueAsString(@params[2], is_term[2]));
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

    public class InStrRev : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
            var buf2 = SRC.Expression.GetValueAsString(@params[2], is_term[2]);
            int i;
            if (Strings.Len(buf2) > 0 && Strings.Len(buf) >= Strings.Len(buf2))
            {
                int num;
                if (pcount == 2)
                {
                    num = Strings.Len(buf);
                }
                else
                {
                    num = SRC.Expression.GetValueAsLong(@params[3], is_term[3]);
                }

                i = (num - Strings.Len(buf2) + 1);
                do
                {
                    var j = Strings.InStr(i, buf, buf2);
                    if (i == j)
                    {
                        break;
                    }

                    i = (i - 1);
                }
                while (i != 0);
            }
            else
            {
                i = 0;
            }

            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum((double)i);
                return ValueType.StringType;
            }
            else
            {
                num_result = (double)i;
                return ValueType.NumericType;
            }
        }
    }
    public class IsNumeric : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            if (GeneralLib.IsNumber(SRC.Expression.GetValueAsString(@params[1], is_term[1])))
            {
                str_result = "1";
                num_result = 1d;
            }
            else
            {
                str_result = "0";
                num_result = 0d;
            }

            if (etype == ValueType.StringType)
            {
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class Left : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            str_result = Strings.Left(SRC.Expression.GetValueAsString(@params[1], is_term[1]), SRC.Expression.GetValueAsLong(@params[2], is_term[2]));
            if (etype == ValueType.NumericType)
            {
                num_result = GeneralLib.StrToDbl(str_result);
                return ValueType.NumericType;
            }
            else
            {
                return ValueType.StringType;
            }
        }
    }

    public class Len : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = (double)Strings.Len(SRC.Expression.GetValueAsString(@params[1], is_term[1]));
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

    public class LSet : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            num_result = 0d;

            var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
            var i = SRC.Expression.GetValueAsLong(@params[2], is_term[2]);
            str_result = GeneralLib.LeftPaddedString(buf, i);
            return ValueType.StringType;
        }
    }

    public class Mid : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
            switch (pcount)
            {
                case 3:
                    {
                        var i = SRC.Expression.GetValueAsLong(@params[2], is_term[2]);
                        var j = SRC.Expression.GetValueAsLong(@params[3], is_term[3]);
                        str_result = Strings.Mid(buf, i, j);
                        break;
                    }

                case 2:
                    {
                        var i = SRC.Expression.GetValueAsLong(@params[2], is_term[2]);
                        str_result = Strings.Mid(buf, i);
                        break;
                    }
            }

            if (etype == ValueType.NumericType)
            {
                num_result = GeneralLib.StrToDbl(str_result);
                return ValueType.NumericType;
            }
            else
            {
                return ValueType.StringType;
            }
        }
    }

    public class Right : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            num_result = 0d;
            str_result = Strings.Right(SRC.Expression.GetValueAsString(@params[1], is_term[1]), SRC.Expression.GetValueAsLong(@params[2], is_term[2]));
            if (etype == ValueType.NumericType)
            {
                num_result = GeneralLib.StrToDbl(str_result);
                return ValueType.NumericType;
            }
            else
            {
                return ValueType.StringType;
            }
        }
    }

    public class RSet : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            num_result = 0d;

            var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
            var i = SRC.Expression.GetValueAsLong(@params[2], is_term[2]);
            str_result = GeneralLib.RightPaddedString(buf, i);
            return ValueType.StringType;
        }
    }

    public class StrComp : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = (double)Strings.StrComp(SRC.Expression.GetValueAsString(@params[1], is_term[1]), SRC.Expression.GetValueAsString(@params[2], is_term[2]));
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

    public class String : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            var buf = SRC.Expression.GetValueAsString(@params[2], is_term[2]);
            var num = SRC.Expression.GetValueAsLong(@params[1], is_term[1]);
            str_result = string.Join("", Enumerable.Range(0, Math.Max(0, num)).Select(x => buf));

            if (etype == ValueType.NumericType)
            {
                num_result = GeneralLib.StrToDbl(str_result);
                return ValueType.NumericType;
            }
            else
            {
                return ValueType.StringType;
            }
        }
    }

    public class Wide : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = Strings.StrConv(SRC.Expression.GetValueAsString(@params[1], is_term[1]), VbStrConv.Wide);
            num_result = 0d;
            return ValueType.StringType;
        }
    }

    // https://github.com/7474/SRC/issues/175

    public class InStrB : InStr
    {
        // TODO Impl Instrb
    }

    public class InStrRevB : InStrRev
    {
        // TODO Impl Instrrevb
    }

    public class LeftB : Left
    {
        // TODO Impl Leftb
    }

    public class LenB : Len
    {
        // TODO Impl Lenb
    }

    public class MidB : Mid
    {
        // TODO Impl Midb
    }

    public class RightB : Right
    {
        // TODO Impl Rightb
    }

    public class Replace : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            switch (pcount)
            {
                case 4:
                    {
                        var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                        var num = SRC.Expression.GetValueAsLong(@params[4], is_term[4]);
                        var buf2 = Strings.Right(buf, Strings.Len(buf) - num + 1);
                        buf2 = buf2.Replace(SRC.Expression.GetValueAsString(@params[2], is_term[2]), SRC.Expression.GetValueAsString(@params[3], is_term[3]));
                        str_result = Strings.Left(buf, num - 1) + buf2;
                        break;
                    }

                case 5:
                    {
                        var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                        var num = SRC.Expression.GetValueAsLong(@params[4], is_term[4]);
                        var num2 = SRC.Expression.GetValueAsLong(@params[5], is_term[5]);
                        var buf2 = Strings.Mid(buf, num, num2);
                        buf2 = buf2.Replace(SRC.Expression.GetValueAsString(@params[2], is_term[2]), SRC.Expression.GetValueAsString(@params[3], is_term[3]));
                        str_result = Strings.Left(buf, num - 1) + buf2 + Strings.Right(buf, Strings.Len(buf) - (num + num2 - 1) - 1);
                        break;
                    }

                default:
                    {
                        str_result = SRC.Expression.GetValueAsString(@params[1], is_term[1]).Replace(
                             SRC.Expression.GetValueAsString(@params[2], is_term[2]),
                             SRC.Expression.GetValueAsString(@params[3], is_term[3]));
                        break;
                    }
            }

            if (etype == ValueType.NumericType)
            {
                num_result = GeneralLib.StrToDbl(str_result);
                return ValueType.NumericType;
            }
            else
            {
                return ValueType.StringType;
            }
        }
    }

    public class Asc : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = Strings.Asc(SRC.Expression.GetValueAsString(@params[1], is_term[1]));
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

    public class Chr : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            num_result = 0d;
            // XXX 文字コード
            str_result = Conversions.ToString((char)SRC.Expression.GetValueAsLong(@params[1], is_term[1]));
            return ValueType.StringType;
        }
    }

    public class LCase : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = Strings.LCase(SRC.Expression.GetValueAsString(@params[1], is_term[1]));
            num_result = 0d;
            return ValueType.StringType;
        }
    }

    public class Trim : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = Strings.Trim(SRC.Expression.GetValueAsString(@params[1], is_term[1]));
            num_result = 0d;
            return ValueType.StringType;
        }
    }
}
