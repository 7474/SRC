using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;
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

            // array_buf: list of (key, varType, value)
            var arrayBuf = new List<(string key, ExprValueType varType, object value)>();

            void CollectArrayElement(Expressions.VarData v)
            {
                if (v.Name.StartsWith(vname + "[", StringComparison.OrdinalIgnoreCase))
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
                    arrayBuf.Add((key, v.VariableType, val));
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

            // 添字が数値の場合、またはインデックスのみのソートの場合、先に添字の昇順に並べ替える
            if (!isStringKey || isKeySort)
            {
                if (isStringKey)
                {
                    arrayBuf = isAscOrder
                        ? arrayBuf.OrderBy(x => x.key, StringComparer.CurrentCultureIgnoreCase).ToList()
                        : arrayBuf.OrderByDescending(x => x.key, StringComparer.CurrentCultureIgnoreCase).ToList();
                }
                else
                {
                    arrayBuf = isAscOrder
                        ? arrayBuf.OrderBy(x => Convert.ToDouble(x.key)).ToList()
                        : arrayBuf.OrderByDescending(x => Convert.ToDouble(x.key)).ToList();
                }
            }

            // インデックスのみのソートでない場合、要素をソート
            if (!isKeySort)
            {
                if (isStringKey)
                {
                    // 文字インデックスの場合はキーも一緒に移動する
                    arrayBuf = isStringValue
                        ? (isAscOrder
                            ? arrayBuf.OrderBy(x => x.value?.ToString(), StringComparer.CurrentCultureIgnoreCase).ToList()
                            : arrayBuf.OrderByDescending(x => x.value?.ToString(), StringComparer.CurrentCultureIgnoreCase).ToList())
                        : (isAscOrder
                            ? arrayBuf.OrderBy(x => Convert.ToDouble(x.value)).ToList()
                            : arrayBuf.OrderByDescending(x => Convert.ToDouble(x.value)).ToList());
                }
                else
                {
                    // 数値インデックスの場合はキーは固定してバリューのみ並び替える
                    var sortedKeys = arrayBuf.Select(x => x.key).ToList();
                    var sortedValues = isStringValue
                        ? (isAscOrder
                            ? arrayBuf.Select(x => (x.varType, x.value)).OrderBy(x => x.value?.ToString(), StringComparer.CurrentCultureIgnoreCase).ToList()
                            : arrayBuf.Select(x => (x.varType, x.value)).OrderByDescending(x => x.value?.ToString(), StringComparer.CurrentCultureIgnoreCase).ToList())
                        : (isAscOrder
                            ? arrayBuf.Select(x => (x.varType, x.value)).OrderBy(x => Convert.ToDouble(x.value)).ToList()
                            : arrayBuf.Select(x => (x.varType, x.value)).OrderByDescending(x => Convert.ToDouble(x.value)).ToList());
                    arrayBuf = sortedKeys.Zip(sortedValues, (k, v) => (k, v.varType, v.value)).ToList();
                }
            }

            // SRC変数に再配置
            for (var i = 0; i < arrayBuf.Count; i++)
            {
                var (key, varType, val) = arrayBuf[i];
                var buf = vname + "[" + key + "]";
                Expression.UndefineVariable(buf);
                if (varType == ExprValueType.StringType)
                {
                    Expression.SetVariable(buf, ExprValueType.StringType, val?.ToString() ?? "", 0d);
                }
                else
                {
                    Expression.SetVariable(buf, ExprValueType.NumericType, "", Convert.ToDouble(val));
                }
            }

            return EventData.NextID;
        }
    }
}
