using SRCCore.Lib;
using SRCCore.VB;

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

            int i;
            if (pcount == 2)
            {
                i = Strings.InStr(
                    SRC.Expression.GetValueAsString(@params[1], is_term[1]),
                    SRC.Expression.GetValueAsString(@params[2], is_term[2]));
            }
            else
            {
                // params(3)が指定されている場合は、それを検索開始位置似設定
                // VBのInStrは引数1が開始位置になりますが、現仕様との兼ね合いを考え、
                // eve上では引数3に設定するようにしています
                i = Strings.InStr(
                    SRC.Expression.GetValueAsLong(@params[3], is_term[3]),
                    SRC.Expression.GetValueAsString(@params[1], is_term[1]),
                    SRC.Expression.GetValueAsString(@params[2], is_term[2]));
            }


            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(i);
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
            if (Strings.Len(buf2) > 0 & Strings.Len(buf) >= Strings.Len(buf2))
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
            str_result = Strings.Right(SRC.Expression. GetValueAsString(@params[1], is_term[1]), SRC.Expression.GetValueAsLong(@params[2], is_term[2]));
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
            num_result = 0d;

            // TODO Impl Strcomp

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

            // TODO Impl String

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


    public class Wide : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Wide

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





    public class InStrB : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Instrb

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
    public class InStrRevB : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Instrrevb

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

    public class LeftB : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Leftb

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


    public class LenB : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Lenb

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


    public class MidB : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Midb

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
    public class RightB : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Rightb

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

    public class Replace : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Replace

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

    public class Asc : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Asc

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
            str_result = "";
            num_result = 0d;

            // TODO Impl Chr

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
            str_result = "";
            num_result = 0d;

            // TODO Impl Trim

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
