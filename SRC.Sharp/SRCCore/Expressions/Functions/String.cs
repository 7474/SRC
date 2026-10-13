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

            // VB版は Format$(文字列, 書式)
            str_result = FormatString(SRC.Expression.GetValueAsString(@params[1], is_term[1]), SRC.Expression.GetValueAsString(@params[2], is_term[2]));
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

        // VB の Format$ と同様に文字列を書式化する
        // - 書式に文字列用の記号（@, &）が含まれる場合は文字列として書式化する
        // - 数値として解釈できる場合は数値として書式化する
        // - それ以外の場合は文字列をそのまま返す（<, > による大文字・小文字変換のみ行う）
        public static string FormatString(string value, string format)
        {
            value = value ?? "";
            format = format ?? "";
            if (format.IndexOf('@') >= 0 || format.IndexOf('&') >= 0)
            {
                return FormatStringWithPlaceholder(value, format);
            }
            if (Information.IsNumeric(value))
            {
                return SrcFormatter.Format(Conversions.ToDouble(value), format);
            }
            return ConvertCase(value, format);
        }

        private static string ConvertCase(string value, string format)
        {
            if (format.IndexOf('<') >= 0)
            {
                return value.ToLower();
            }
            if (format.IndexOf('>') >= 0)
            {
                return value.ToUpper();
            }
            return value;
        }

        private static string FormatStringWithPlaceholder(string value, string format)
        {
            value = ConvertCase(value, format);
            var leftToRight = format.IndexOf('!') >= 0;
            var pattern = format.Replace("!", "").Replace("<", "").Replace(">", "");
            var placeholders = pattern.Count(c => c == '@' || c == '&');
            var result = new System.Text.StringBuilder();
            if (leftToRight)
            {
                // 左から順に文字を割り当てる
                var idx = 0;
                foreach (var c in pattern)
                {
                    if (c == '@' || c == '&')
                    {
                        if (idx < value.Length)
                        {
                            result.Append(value[idx++]);
                        }
                        else if (c == '@')
                        {
                            result.Append(' ');
                        }
                    }
                    else
                    {
                        result.Append(c);
                    }
                }
                // 割り当てられなかった文字は末尾に付ける
                if (idx < value.Length)
                {
                    result.Append(value.Substring(idx));
                }
            }
            else
            {
                // 右から順に文字を割り当てる
                var idx = value.Length - 1;
                for (var k = pattern.Length - 1; k >= 0; k--)
                {
                    var c = pattern[k];
                    if (c == '@' || c == '&')
                    {
                        if (idx >= 0)
                        {
                            result.Insert(0, value[idx--]);
                        }
                        else if (c == '@')
                        {
                            result.Insert(0, ' ');
                        }
                    }
                    else
                    {
                        result.Insert(0, c);
                    }
                }
                // 割り当てられなかった文字は先頭に付ける
                if (idx >= 0)
                {
                    result.Insert(0, value.Substring(0, idx + 1));
                }
            }
            return result.ToString();
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
                // 開始位置が1未満の場合は見つからない（VB版では実行時エラーとなる）
                while (i > 0)
                {
                    var j = Strings.InStr(i, buf, buf2);
                    if (i == j)
                    {
                        break;
                    }

                    i = (i - 1);
                }
                if (i < 0)
                {
                    i = 0;
                }
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
            str_result = GeneralLib.RightPaddedString(buf, i);
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
            str_result = GeneralLib.LeftPaddedString(buf, i);
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

    public class InStrB : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            if (pcount == 2)
            {
                num_result = Strings.InStrB(
                    SRC.Expression.GetValueAsString(@params[1], is_term[1]),
                    SRC.Expression.GetValueAsString(@params[2], is_term[2]));
            }
            else
            {
                num_result = Strings.InStrB(
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

    public class InStrRevB : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
            var buf2 = SRC.Expression.GetValueAsString(@params[2], is_term[2]);
            int i;
            
            if (pcount == 2)
            {
                i = Strings.InStrRevB(buf, buf2);
            }
            else
            {
                i = Strings.InStrRevB(
                    SRC.Expression.GetValueAsLong(@params[3], is_term[3]),
                    buf,
                    buf2);
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

    public class LeftB : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            str_result = Strings.LeftB(
                SRC.Expression.GetValueAsString(@params[1], is_term[1]),
                SRC.Expression.GetValueAsLong(@params[2], is_term[2]));
            
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

    public class LenB : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = (double)Strings.LenB(SRC.Expression.GetValueAsString(@params[1], is_term[1]));
            
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

            var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
            switch (pcount)
            {
                case 3:
                    {
                        var i = SRC.Expression.GetValueAsLong(@params[2], is_term[2]);
                        var j = SRC.Expression.GetValueAsLong(@params[3], is_term[3]);
                        str_result = Strings.MidB(buf, i, j);
                        break;
                    }

                case 2:
                    {
                        var i = SRC.Expression.GetValueAsLong(@params[2], is_term[2]);
                        str_result = Strings.MidB(buf, i);
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

    public class RightB : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            num_result = 0d;
            str_result = Strings.RightB(
                SRC.Expression.GetValueAsString(@params[1], is_term[1]),
                SRC.Expression.GetValueAsLong(@params[2], is_term[2]));
            
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
                        buf2 = GeneralLib.ReplaceString(buf2, SRC.Expression.GetValueAsString(@params[2], is_term[2]), SRC.Expression.GetValueAsString(@params[3], is_term[3]));
                        str_result = Strings.Left(buf, num - 1) + buf2;
                        break;
                    }

                case 5:
                    {
                        var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                        var num = SRC.Expression.GetValueAsLong(@params[4], is_term[4]);
                        var num2 = SRC.Expression.GetValueAsLong(@params[5], is_term[5]);
                        var buf2 = Strings.Mid(buf, num, num2);
                        buf2 = GeneralLib.ReplaceString(buf2, SRC.Expression.GetValueAsString(@params[2], is_term[2]), SRC.Expression.GetValueAsString(@params[3], is_term[3]));
                        str_result = Strings.Left(buf, num - 1) + buf2 + Strings.Right(buf, Strings.Len(buf) - (num + num2 - 1) - 1);
                        break;
                    }

                default:
                    {
                        str_result = GeneralLib.ReplaceString(
                             SRC.Expression.GetValueAsString(@params[1], is_term[1]),
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
            // Shift_JIS の文字コードとして扱う
            str_result = Strings.Chr(SRC.Expression.GetValueAsLong(@params[1], is_term[1]));
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
