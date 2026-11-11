using SRCCore.Exceptions;
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

            // VB版は Format$(文字列, 書式)。書式の名前・日付の書式文字も VB5 の Format$ と同じに解釈する（VbFormat）
            str_result = VbFormat.Format(SRC.Expression.GetValueAsString(@params[1], is_term[1]), SRC.Expression.GetValueAsString(@params[2], is_term[2]));
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

    // 元の SRC の CallFunction が呼ぶ VB の文字列関数の、範囲外の引数の確かめ。
    // VB の関数は範囲外の引数で実行時エラー 5 を起こし、CallFunction はエラーを処理しないので、
    // CmdData の Exec が「イベントデータが不正です」を出してイベントを止める（CmdData.cls の Exec の ErrorHandler）
    internal static class VbArgs
    {
        // Mid$(s, start[, length])・MidB$: start は 1 以上、length は 0 以上
        public static void Mid(string name, int start, int? length = null)
        {
            if (start <= 0 || length < 0)
            {
                throw VbRuntimeErrorException.InvalidProcedureCall(name);
            }
        }

        // Left$・Right$・LeftB$・RightB$・String$ の文字数・回数は 0 以上
        public static void NonNegative(string name, int length)
        {
            if (length < 0)
            {
                throw VbRuntimeErrorException.InvalidProcedureCall(name);
            }
        }

        // InStr・InStrB の開始位置は 1 以上
        public static void Start(string name, int start)
        {
            if (start <= 0)
            {
                throw VbRuntimeErrorException.InvalidProcedureCall(name);
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
                var start = SRC.Expression.GetValueAsLong(@params[3], is_term[3]);
                var string1 = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                var string2 = SRC.Expression.GetValueAsString(@params[2], is_term[2]);
                VbArgs.Start("InStr", start);
                num_result = Strings.InStr(start, string1, string2);
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
                // 元の SRC は InStr(i, buf, buf2) から始めるので、開始位置が 1 未満なら InStr のエラー 5
                VbArgs.Start("InStrRev", i);
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

            var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
            var length = SRC.Expression.GetValueAsLong(@params[2], is_term[2]);
            VbArgs.NonNegative("Left", length);
            str_result = Strings.Left(buf, length);
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
            // 元の SRC: Shift_JIS のバイト数（LenB(StrConv(buf, vbFromUnicode))）が i に足りない分だけ空白を付ける
            var len = Strings.LenB(buf);
            str_result = len < i ? buf + Strings.Space(i - len) : buf;
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
                        VbArgs.Mid("Mid", i, j);
                        str_result = Strings.Mid(buf, i, j);
                        break;
                    }

                case 2:
                    {
                        var i = SRC.Expression.GetValueAsLong(@params[2], is_term[2]);
                        VbArgs.Mid("Mid", i);
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
            var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
            var length = SRC.Expression.GetValueAsLong(@params[2], is_term[2]);
            VbArgs.NonNegative("Right", length);
            str_result = Strings.Right(buf, length);
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
            // 元の SRC: Shift_JIS のバイト数（LenB(StrConv(buf, vbFromUnicode))）が i に足りない分だけ空白を付ける
            var len = Strings.LenB(buf);
            str_result = len < i ? Strings.Space(i - len) + buf : buf;
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
            if (Strings.Len(buf) <= 1)
            {
                // 元の SRC は String$(回数, buf) を呼ぶ。回数が負か、buf が長さ 0 なら VB のエラー 5
                VbArgs.NonNegative("String", num);
                if (buf.Length == 0)
                {
                    throw VbRuntimeErrorException.InvalidProcedureCall("String");
                }
            }
            // 2 文字以上は For ループで繋ぐ（負の回数は空文字列）
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
                var start = SRC.Expression.GetValueAsLong(@params[3], is_term[3]);
                var string1 = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                var string2 = SRC.Expression.GetValueAsString(@params[2], is_term[2]);
                VbArgs.Start("InStrB", start);
                num_result = Strings.InStrB(start, string1, string2);
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

            var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
            var length = SRC.Expression.GetValueAsLong(@params[2], is_term[2]);
            VbArgs.NonNegative("LeftB", length);
            str_result = Strings.LeftB(buf, length);
            
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
                        VbArgs.Mid("MidB", i, j);
                        str_result = Strings.MidB(buf, i, j);
                        break;
                    }

                case 2:
                    {
                        var i = SRC.Expression.GetValueAsLong(@params[2], is_term[2]);
                        VbArgs.Mid("MidB", i);
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
            var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
            var length = SRC.Expression.GetValueAsLong(@params[2], is_term[2]);
            VbArgs.NonNegative("RightB", length);
            str_result = Strings.RightB(buf, length);
            
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
                        VbArgs.NonNegative("Right", Strings.Len(buf) - num + 1);
                        var buf2 = Strings.Right(buf, Strings.Len(buf) - num + 1);
                        buf2 = GeneralLib.ReplaceString(buf2, SRC.Expression.GetValueAsString(@params[2], is_term[2]), SRC.Expression.GetValueAsString(@params[3], is_term[3]));
                        VbArgs.NonNegative("Left", num - 1);
                        str_result = Strings.Left(buf, num - 1) + buf2;
                        break;
                    }

                case 5:
                    {
                        var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                        var num = SRC.Expression.GetValueAsLong(@params[4], is_term[4]);
                        var num2 = SRC.Expression.GetValueAsLong(@params[5], is_term[5]);
                        VbArgs.Mid("Mid", num, num2);
                        var buf2 = Strings.Mid(buf, num, num2);
                        buf2 = GeneralLib.ReplaceString(buf2, SRC.Expression.GetValueAsString(@params[2], is_term[2]), SRC.Expression.GetValueAsString(@params[3], is_term[3]));
                        VbArgs.NonNegative("Left", num - 1);
                        VbArgs.NonNegative("Right", Strings.Len(buf) - (num + num2 - 1) - 1);
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
            var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
            if (buf.Length == 0)
            {
                // VB の Asc は長さ 0 の文字列でエラー 5
                throw VbRuntimeErrorException.InvalidProcedureCall("Asc");
            }
            num_result = Strings.Asc(buf);
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
            var code = SRC.Expression.GetValueAsLong(@params[1], is_term[1]);
            // 日本語版の Windows（DBCS）の Chr$ が受け付ける文字コードは -32768〜65535。範囲の外は VB のエラー 5
            if (code < -32768 || code > 65535)
            {
                throw VbRuntimeErrorException.InvalidProcedureCall("Chr");
            }
            str_result = Strings.Chr(code);
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
