using SRCCore.Lib;
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
