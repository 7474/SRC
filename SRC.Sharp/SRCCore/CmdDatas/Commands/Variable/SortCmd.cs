using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using ExprValueType = SRCCore.Expressions.ValueType;

namespace SRCCore.CmdDatas.Commands
{
    public class SortCmd : CmdData
    {
        public SortCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SortCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum < 2)
            {
                throw new EventErrorException(this, "Sortコマンドの引数の数が違います");
            }

            // 初期値
            var isAscOrder = true;
            var isStringKey = false;
            var isStringValue = false;
            var isKeySort = false;

            for (var i = 3; i <= ArgNum; i++)
            {
                var buf = GetArgAsString(i);
                switch (buf ?? "")
                {
                    case "昇順":
                        isAscOrder = true;
                        break;
                    case "降順":
                        isAscOrder = false;
                        break;
                    case "数値":
                        isStringValue = false;
                        break;
                    case "文字":
                        isStringValue = true;
                        break;
                    case "インデックスのみ":
                        isKeySort = true;
                        break;
                    case "文字インデックス":
                        isStringKey = true;
                        break;
                    default:
                        throw new EventErrorException(this, "Sortコマンドに不正なオプション「" + buf + "」が使われています");
                }
            }

            // ソートする配列変数名
            var vname = GetArg(2);
            if (Strings.Left(vname, 1) == "$")
            {
                vname = Strings.Mid(vname, 2);
            }
            // Eval関数
            if (Strings.LCase(Strings.Left(vname, 5)) == "eval(")
            {
                if (Strings.Right(vname, 1) == ")")
                {
                    vname = Strings.Mid(vname, 6, Strings.Len(vname) - 6);
                    vname = Expression.GetValueAsString(vname);
                }
            }

            // array_buf(opt, value): 0…配列の添字、1…変数の ValueType、2…変数の値
            var arrayBuf = new List<object[]>();

            void CollectArrayElement(Expressions.VarData v)
            {
                // VB: InStr(.Name, vname & "[") = 1（大文字・小文字を区別する）
                if (v.Name.StartsWith(vname + "[", StringComparison.Ordinal))
                {
                    var key = Strings.Mid(v.Name, Strings.InStr(v.Name, "[") + 1,
                        GeneralLib.InStr2(v.Name, "]") - Strings.InStr(v.Name, "[") - 1);
                    if (!Information.IsNumeric(key))
                    {
                        isStringKey = true;
                    }
                    object val = v.VariableType == ExprValueType.StringType
                        ? (object)v.StringValue
                        : (object)v.NumericValue;
                    if (!Information.IsNumeric(val))
                    {
                        isStringValue = true;
                    }
                    arrayBuf.Add(new object[] { key, v.VariableType, val });
                }
            }

            if (Expression.IsSubLocalVariableDefined(vname))
            {
                foreach (var v in Event.SubLocalVars())
                {
                    CollectArrayElement(v);
                }
            }
            else if (Expression.IsLocalVariableDefined(vname))
            {
                foreach (var v in Event.LocalVariableList.Values)
                {
                    CollectArrayElement(v);
                }
            }
            else if (Expression.IsGlobalVariableDefined(vname))
            {
                foreach (var v in Event.GlobalVariableList.Values)
                {
                    CollectArrayElement(v);
                }
            }

            if (arrayBuf.Count == 0)
            {
                return EventData.NextID;
            }

            var num = arrayBuf.Count - 1;

            // VB と同じ交換ソート（安定ではないので、同じ値の要素の順も VB に合わせる）。
            // 文字列の比較は StrComp(…, vbTextCompare)（大文字・小文字、全角・半角、ひらがな・カタカナを区別しない）
            bool IsSwap(object a, object b, bool isString)
            {
                if (isString)
                {
                    var cmp = TextCompare.Compare(ToText(a), ToText(b));
                    return isAscOrder ? cmp > 0 : cmp < 0;
                }
                else
                {
                    var da = ToNumber(a);
                    var db = ToNumber(b);
                    return isAscOrder ? da > db : da < db;
                }
            }

            void Swap(int i, int j, int fromK)
            {
                for (var k = fromK; k <= 2; k++)
                {
                    var var_buf = arrayBuf[i][k];
                    arrayBuf[i][k] = arrayBuf[j][k];
                    arrayBuf[j][k] = var_buf;
                }
            }

            if (!isStringKey || isKeySort)
            {
                // 添字が数値の場合、またはインデックスのみのソートの場合、
                // 先に添字の昇順に並び替える
                for (var i = 0; i <= num - 1; i++)
                {
                    for (var j = num; j >= i + 1; j--)
                    {
                        if (IsSwap(arrayBuf[i][0], arrayBuf[j][0], isStringKey))
                        {
                            Swap(i, j, 0);
                        }
                    }
                }
            }

            if (!isKeySort)
            {
                // 改めて要素をソート（添字が数値なら添字は動かさず、型と値だけを入れ替える）
                for (var i = 0; i <= num - 1; i++)
                {
                    for (var j = num; j >= i + 1; j--)
                    {
                        if (IsSwap(arrayBuf[i][2], arrayBuf[j][2], isStringValue))
                        {
                            Swap(i, j, isStringKey ? 0 : 1);
                        }
                    }
                }
            }

            // SRC変数に再配置
            for (var i = 0; i <= num; i++)
            {
                var buf = vname + "[" + ToText(arrayBuf[i][0]) + "]";
                Expression.UndefineVariable(buf);
                if ((ExprValueType)arrayBuf[i][1] == ExprValueType.StringType)
                {
                    Expression.SetVariable(buf, ExprValueType.StringType, ToText(arrayBuf[i][2]), 0d);
                }
                else
                {
                    Expression.SetVariable(buf, ExprValueType.NumericType, "", ToNumber(arrayBuf[i][2]));
                }
            }

            return EventData.NextID;
        }

        private static readonly StringComparer TextCompare = SrcCollection<string>.KeyComparer;

        // VB の CStr に当たる
        private static string ToText(object value)
        {
            return Conversions.ToString(value);
        }

        // VB の CDbl に当たる（数値と判定された値だけを渡す）
        private static double ToNumber(object value)
        {
            return value is double d ? d : Conversions.ToDouble(Conversions.ToString(value));
        }
    }
}
