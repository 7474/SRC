using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    public class InStr : AFunction
    {
        public override ValueType Invoke(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
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
    public class IsNumeric : AFunction
    {
        public override ValueType Invoke(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
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
}
