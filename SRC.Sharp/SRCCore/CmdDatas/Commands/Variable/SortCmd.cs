using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Expressions;
using SRCCore.VB;
using System;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Commands
{
    public class SortCmd : CmdData
    {
        public SortCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SortCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();

            // TODO 配列操作のユーティリティ（ちょっとだるくなった）
            //object ExecSortCmdRet = default;
            //short j, i, k;
            //bool isStringkey, isStringValue;
            //bool isSwap, isAscOrder, isKeySort;
            //string vname, buf;
            //object value_buf;
            //short num;
            //VarData var;
            //var array_buf = default(object[]);
            //var var_buf = new object[3];

            // array_buf(opt, value)
            // opt=0…配列の添字
            // =1…変数のValueTyep
            // =2…変数の値

            //if (ArgNum < 2)
            //{
            //    throw new EventErrorException(this, "Sortコマンドの引数の数が違います");
            //}

            //// 初期値
            //var isAscOrder = true; // ソート順を昇順似設定
            //var isStringkey = false; // 配列のインデックスを数値として扱う
            //var isStringValue = false; // 配列の要素を数値として扱う
            //var isKeySort = false; // インデックスのみのソートではない
            //var loopTo = ArgNum;
            //for (var i = 3; i <= loopTo; i++)
            //{
            //    var buf = GetArgAsString(i);
            //    switch (buf ?? "")
            //    {
            //        case "昇順":
            //            {
            //                isAscOrder = true;
            //                break;
            //            }

            //        case "降順":
            //            {
            //                isAscOrder = false;
            //                break;
            //            }

            //        case "数値":
            //            {
            //                isStringValue = false;
            //                break;
            //            }

            //        case "文字":
            //            {
            //                isStringValue = true;
            //                break;
            //            }

            //        case "インデックスのみ":
            //            {
            //                isKeySort = true;
            //                break;
            //            }

            //        case "文字インデックス":
            //            {
            //                isStringkey = true;
            //                break;
            //            }

            //        default:
            //            throw new EventErrorException(this, "Sortコマンドに不正なオプション「" + buf + "」が使われています");
            //    }
            //}

            //// ソートする配列変数名
            //var vname = GetArg(2);
            //if (Strings.Left(vname, 1) == "$")
            //{
            //    vname = Strings.Mid(vname, 2);
            //}
            //// Eval関数
            //if (Strings.LCase(Strings.Left(vname, 5)) == "eval(")
            //{
            //    if (Strings.Right(vname, 1) == ")")
            //    {
            //        vname = Strings.Mid(vname, 6, Strings.Len(vname) - 6);
            //        vname = Expression.GetValueAsString(vname);
            //    }
            //}

            //// 配列を検索し、配列要素を見つける
            //var num = 0;
            //IList<VarData> array_buf = new List<VarData>();
            //if (Expression.IsSubLocalVariableDefined(vname))
            //{
            //    // サブルーチンローカルな配列
            //    // TODO ここに限らず VarIndexStack 参照見直し
            //    var loopTo1 = Event.VarIndex;
            //    for (var i = (Event.VarIndexStack[Event.CallDepth - 1] + 1); i <= loopTo1; i++)
            //    {
            //        {
            //            var withBlock = Event.VarStack[i];
            //            if (Strings.InStr(withBlock.Name, vname + "[") == 1)
            //            {
            //                var oldArray_buf = array_buf;
            //                array_buf = new object[3, (num + 1)];
            //                if (oldArray_buf is object)
            //                    for (var i1 = 0; i1 <= oldArray_buf.Length / oldArray_buf.GetLength(1) - 1; ++i1)
            //                        Array.Copy(oldArray_buf, i1 * oldArray_buf.GetLength(1), array_buf, i1 * array_buf.GetLength(1), Math.Min(oldArray_buf.GetLength(1), array_buf.GetLength(1)));
            //                buf = Strings.Mid(withBlock.Name, Strings.InStr(withBlock.Name, "[") + 1, GeneralLib.InStr2(withBlock.Name, "]") - Strings.InStr(withBlock.Name, "[") - 1);
            //                if (!Information.IsNumeric(buf))
            //                {
            //                    isStringkey = true;
            //                }

            //                if (withBlock.VariableType == Expressions.ValueType.StringType)
            //                {
            //                    value_buf = withBlock.StringValue;
            //                }
            //                else
            //                {
            //                    value_buf = withBlock.NumericValue;
            //                }

            //                if (!Information.IsNumeric(value_buf))
            //                {
            //                    isStringValue = true;
            //                }

            //                array_buf[0, num] = buf;
            //                array_buf[1, num] = withBlock.VariableType;
            //                array_buf[2, num] = value_buf;
            //                num = (num + 1);
            //            }
            //        }
            //    }

            //    if (num == 0)
            //    {
            //        return EventData.NextID;
            //    }
            //}
            //else if (Expression.IsLocalVariableDefined(vname))
            //{
            //    // ローカルな配列
            //    foreach (VarData currentVar in Event.LocalVariableList)
            //    {
            //        var = currentVar;
            //        if (Strings.InStr(var.Name, vname + "[") == 1)
            //        {
            //            var oldArray_buf1 = array_buf;
            //            array_buf = new object[3, (num + 1)];
            //            if (oldArray_buf1 is object)
            //                for (var i2 = 0; i2 <= oldArray_buf1.Length / oldArray_buf1.GetLength(1) - 1; ++i2)
            //                    Array.Copy(oldArray_buf1, i2 * oldArray_buf1.GetLength(1), array_buf, i2 * array_buf.GetLength(1), Math.Min(oldArray_buf1.GetLength(1), array_buf.GetLength(1)));
            //            buf = Strings.Mid(var.Name, Strings.InStr(var.Name, "[") + 1, GeneralLib.InStr2(var.Name, "]") - Strings.InStr(var.Name, "[") - 1);
            //            if (!Information.IsNumeric(buf))
            //            {
            //                isStringkey = true;
            //            }

            //            if (var.VariableType == Expression.ValueType.StringType)
            //            {
            //                // UPGRADE_WARNING: オブジェクト value_buf の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                value_buf = var.StringValue;
            //            }
            //            else
            //            {
            //                // UPGRADE_WARNING: オブジェクト value_buf の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                value_buf = var.NumericValue;
            //            }

            //            if (!Information.IsNumeric(value_buf))
            //            {
            //                isStringValue = true;
            //            }

            //            // UPGRADE_WARNING: オブジェクト array_buf(0, num) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //            array_buf[0, num] = buf;
            //            // UPGRADE_WARNING: オブジェクト array_buf(1, num) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //            array_buf[1, num] = var.VariableType;
            //            // UPGRADE_WARNING: オブジェクト value_buf の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //            // UPGRADE_WARNING: オブジェクト array_buf(2, num) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //            array_buf[2, num] = value_buf;
            //            num = (num + 1);
            //        }
            //    }

            //    if (num == 0)
            //    {
            //        return EventData.NextID;
            //    }
            //}
            //else if (Expression.IsGlobalVariableDefined(vname))
            //{
            //    // グローバルな配列
            //    foreach (VarData currentVar1 in Event.GlobalVariableList)
            //    {
            //        var = currentVar1;
            //        if (Strings.InStr(var.Name, vname + "[") == 1)
            //        {
            //            var oldArray_buf2 = array_buf;
            //            array_buf = new object[3, (num + 1)];
            //            if (oldArray_buf2 is object)
            //                for (var i3 = 0; i3 <= oldArray_buf2.Length / oldArray_buf2.GetLength(1) - 1; ++i3)
            //                    Array.Copy(oldArray_buf2, i3 * oldArray_buf2.GetLength(1), array_buf, i3 * array_buf.GetLength(1), Math.Min(oldArray_buf2.GetLength(1), array_buf.GetLength(1)));
            //            buf = Strings.Mid(var.Name, Strings.InStr(var.Name, "[") + 1, GeneralLib.InStr2(var.Name, "]") - Strings.InStr(var.Name, "[") - 1);
            //            if (!Information.IsNumeric(buf))
            //            {
            //                isStringkey = true;
            //            }

            //            if (var.VariableType == Expression.ValueType.StringType)
            //            {
            //                // UPGRADE_WARNING: オブジェクト value_buf の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                value_buf = var.StringValue;
            //            }
            //            else
            //            {
            //                // UPGRADE_WARNING: オブジェクト value_buf の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                value_buf = var.NumericValue;
            //            }

            //            if (!Information.IsNumeric(value_buf))
            //            {
            //                isStringValue = true;
            //            }

            //            // UPGRADE_WARNING: オブジェクト array_buf(0, num) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //            array_buf[0, num] = buf;
            //            // UPGRADE_WARNING: オブジェクト array_buf(1, num) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //            array_buf[1, num] = var.VariableType;
            //            // UPGRADE_WARNING: オブジェクト value_buf の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //            // UPGRADE_WARNING: オブジェクト array_buf(2, num) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //            array_buf[2, num] = value_buf;
            //            num = (num + 1);
            //        }
            //    }

            //    if (num == 0)
            //    {
            //        return EventData.NextID;
            //    }
            //}

            //num = (num - 1);
            //if (!isStringkey || isKeySort)
            //{
            //    // 添字が数値の場合、またはインデックスのみのソートの場合、
            //    // 先に添字の昇順に並び替える
            //    var loopTo2 = (num - 1);
            //    for (i = 0; i <= loopTo2; i++)
            //    {
            //        var loopTo3 = (i + 1);
            //        for (j = num; j >= loopTo3; j += -1)
            //        {
            //            isSwap = false;
            //            if (isStringkey)
            //            {
            //                if (isAscOrder)
            //                {
            //                    // UPGRADE_WARNING: オブジェクト array_buf(0, j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    // UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    isSwap = Conversions.ToBoolean(Interaction.IIf(Strings.StrComp(array_buf[0, i], array_buf[0, j], CompareMethod.Text) == 1, true, false));
            //                }
            //                else
            //                {
            //                    // UPGRADE_WARNING: オブジェクト array_buf(0, j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    // UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    isSwap = Conversions.ToBoolean(Interaction.IIf(Strings.StrComp(array_buf[0, i], array_buf[0, j], CompareMethod.Text) == -1, true, false));
            //                }
            //            }
            //            else if (isAscOrder)
            //            {
            //                // UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                isSwap = Conversions.ToBoolean(Interaction.IIf(Conversions.ToDouble(array_buf[0, i]) > Conversions.ToDouble(array_buf[0, j]), true, false));
            //            }
            //            else
            //            {
            //                // UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                isSwap = Conversions.ToBoolean(Interaction.IIf(Conversions.ToDouble(array_buf[0, i]) < Conversions.ToDouble(array_buf[0, j]), true, false));
            //            }

            //            if (isSwap)
            //            {
            //                for (k = 0; k <= 2; k++)
            //                {
            //                    // UPGRADE_WARNING: オブジェクト array_buf(k, i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    // UPGRADE_WARNING: オブジェクト var_buf(k) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    var_buf[k] = array_buf[k, i];
            //                    // UPGRADE_WARNING: オブジェクト array_buf(k, j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    // UPGRADE_WARNING: オブジェクト array_buf(k, i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    array_buf[k, i] = array_buf[k, j];
            //                    // UPGRADE_WARNING: オブジェクト var_buf(k) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    // UPGRADE_WARNING: オブジェクト array_buf(k, j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    array_buf[k, j] = var_buf[k];
            //                }
            //            }
            //        }
            //    }
            //}

            //if (!isKeySort)
            //{
            //    // 改めて要素をソート
            //    var loopTo4 = (num - 1);
            //    for (i = 0; i <= loopTo4; i++)
            //    {
            //        var loopTo5 = (i + 1);
            //        for (j = num; j >= loopTo5; j += -1)
            //        {
            //            isSwap = false;
            //            if (isStringValue)
            //            {
            //                if (isAscOrder)
            //                {
            //                    // UPGRADE_WARNING: オブジェクト array_buf(2, j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    // UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    isSwap = Conversions.ToBoolean(Interaction.IIf(Strings.StrComp(array_buf[2, i], array_buf[2, j], CompareMethod.Text) == 1, true, false));
            //                }
            //                else
            //                {
            //                    // UPGRADE_WARNING: オブジェクト array_buf(2, j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    // UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    isSwap = Conversions.ToBoolean(Interaction.IIf(Strings.StrComp(array_buf[2, i], array_buf[2, j], CompareMethod.Text) == -1, true, false));
            //                }
            //            }
            //            else if (isAscOrder)
            //            {
            //                // UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                isSwap = Conversions.ToBoolean(Interaction.IIf(Conversions.ToDouble(array_buf[2, i]) > Conversions.ToDouble(array_buf[2, j]), true, false));
            //            }
            //            else
            //            {
            //                // UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                isSwap = Conversions.ToBoolean(Interaction.IIf(Conversions.ToDouble(array_buf[2, i]) < Conversions.ToDouble(array_buf[2, j]), true, false));
            //            }

            //            if (isSwap)
            //            {
            //                for (k = Conversions.ToShort(Interaction.IIf(isStringkey, 0, 1)); k <= 2; k++)
            //                {
            //                    // UPGRADE_WARNING: オブジェクト array_buf(k, i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    // UPGRADE_WARNING: オブジェクト var_buf(k) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    var_buf[k] = array_buf[k, i];
            //                    // UPGRADE_WARNING: オブジェクト array_buf(k, j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    // UPGRADE_WARNING: オブジェクト array_buf(k, i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    array_buf[k, i] = array_buf[k, j];
            //                    // UPGRADE_WARNING: オブジェクト var_buf(k) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    // UPGRADE_WARNING: オブジェクト array_buf(k, j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    array_buf[k, j] = var_buf[k];
            //                }
            //            }
            //        }
            //    }
            //}

            //// SRC変数に再配置
            //var loopTo6 = num;
            //for (i = 0; i <= loopTo6; i++)
            //{
            //    // UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //    buf = vname + "[" + Conversions.ToString(array_buf[0, i]) + "]";
            //    Expression.UndefineVariable(buf);
            //    if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(array_buf[1, i], Expression.ValueType.StringType, false)))
            //    {
            //        // UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //        Expression.SetVariable(buf, Expression.ValueType.StringType, Conversions.ToString(array_buf[2, i]), 0);
            //    }
            //    else
            //    {
            //        // UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //        Expression.SetVariable(buf, Expression.ValueType.NumericType, "", Conversions.ToDouble(array_buf[2, i]));
            //    }
            //}

            //return EventData.NextID;
        }
    }
}
