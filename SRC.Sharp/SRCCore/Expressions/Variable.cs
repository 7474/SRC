// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using System;
using System.Collections.Generic;
using System.Text;

namespace SRC.Core.Expressions
{
    // === 変数に関する処理 ===
    public partial class Expression
    {
        // 変数の値を評価
        public static ValueType GetVariable(ref string var_name, ref ValueType etype, ref string str_result, ref double num_result)
        {
            ValueType GetVariableRet = default;
            string vname;
            short i, num;
            Unit u;
            int ret;
            string ipara, idx, buf = default;
            short start_idx, depth;
            bool in_single_quote = default, in_double_quote = default;
            bool is_term;
            vname = var_name;

            // 未定義値の設定
            str_result = var_name;

            // 変数が配列？
            ret = Strings.InStr(vname, "[");
            if (ret == 0)
            {
                goto SkipArrayHandling;
            }

            if (Strings.Right(vname, 1) != "]")
            {
                goto SkipArrayHandling;
            }

            // ここから配列専用の処理

            // インデックス部分の切りだし
            idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);

            // 多次元配列の処理
            if (Strings.InStr(idx, ",") > 0)
            {
                start_idx = 1;
                depth = 0;
                is_term = true;
                var loopTo = (short)Strings.Len(idx);
                for (i = 1; i <= loopTo; i++)
                {
                    if (in_single_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 96) // `
                        {
                            in_single_quote = false;
                        }
                    }
                    else if (in_double_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 34) // "
                        {
                            in_double_quote = false;
                        }
                    }
                    else
                    {
                        switch (Strings.Asc(Strings.Mid(idx, i, 1)))
                        {
                            case 9:
                            case 32: // タブ, 空白
                                {
                                    if (start_idx == i)
                                    {
                                        start_idx = (short)(i + 1);
                                    }
                                    else
                                    {
                                        is_term = false;
                                    }

                                    break;
                                }

                            case 40:
                            case 91: // (, [
                                {
                                    depth = (short)(depth + 1);
                                    break;
                                }

                            case 41:
                            case 93: // ), ]
                                {
                                    depth = (short)(depth - 1);
                                    break;
                                }

                            case 44: // ,
                                {
                                    if (depth == 0)
                                    {
                                        if (Strings.Len(buf) > 0)
                                        {
                                            buf = buf + ",";
                                        }

                                        ipara = Strings.Trim(Strings.Mid(idx, start_idx, i - start_idx));
                                        buf = buf + GetValueAsString(ref ipara, is_term);
                                        start_idx = (short)(i + 1);
                                        is_term = true;
                                    }

                                    break;
                                }

                            case 96: // `
                                {
                                    in_single_quote = true;
                                    break;
                                }

                            case 34: // "
                                {
                                    in_double_quote = true;
                                    break;
                                }
                        }
                    }
                }

                ipara = Strings.Trim(Strings.Mid(idx, start_idx, i - start_idx));
                if (Strings.Len(buf) > 0)
                {
                    idx = buf + "," + GetValueAsString(ref ipara, is_term);
                }
                else
                {
                    idx = GetValueAsString(ref ipara, is_term);
                }
            }
            else
            {
                idx = GetValueAsString(ref idx);
            }

            // 変数名を配列のインデックス部を計算して再構築
            vname = Strings.Left(vname, ret) + idx + "]";

            // 定義されていない要素を使って配列を読み出した場合は空文字列を返す
            str_result = "";

        // 配列専用の処理が終了

        SkipArrayHandling:
            ;


            // ここから配列と通常変数の共通処理

            // サブルーチンローカル変数
            if (Event_Renamed.CallDepth > 0)
            {
                var loopTo1 = Event_Renamed.VarIndex;
                for (i = (short)(Event_Renamed.VarIndexStack[Event_Renamed.CallDepth - 1] + 1); i <= loopTo1; i++)
                {
                    {
                        var withBlock = Event_Renamed.VarStack[i];
                        if ((vname ?? "") == (withBlock.Name ?? ""))
                        {
                            switch (etype)
                            {
                                case ValueType.NumericType:
                                    {
                                        if (withBlock.VariableType == ValueType.NumericType)
                                        {
                                            num_result = withBlock.NumericValue;
                                        }
                                        else
                                        {
                                            num_result = GeneralLib.StrToDbl(ref withBlock.StringValue);
                                        }

                                        GetVariableRet = ValueType.NumericType;
                                        break;
                                    }

                                case ValueType.StringType:
                                    {
                                        if (withBlock.VariableType == ValueType.StringType)
                                        {
                                            str_result = withBlock.StringValue;
                                        }
                                        else
                                        {
                                            str_result = GeneralLib.FormatNum(withBlock.NumericValue);
                                        }

                                        GetVariableRet = ValueType.StringType;
                                        break;
                                    }

                                case ValueType.UndefinedType:
                                    {
                                        if (withBlock.VariableType == ValueType.StringType)
                                        {
                                            str_result = withBlock.StringValue;
                                            GetVariableRet = ValueType.StringType;
                                        }
                                        else
                                        {
                                            num_result = withBlock.NumericValue;
                                            GetVariableRet = ValueType.NumericType;
                                        }

                                        break;
                                    }
                            }

                            return GetVariableRet;
                        }
                    }
                }
            }

            // ローカル変数
            if (IsLocalVariableDefined(ref vname))
            {
                {
                    var withBlock1 = Event_Renamed.LocalVariableList[vname];
                    switch (etype)
                    {
                        case ValueType.NumericType:
                            {
                                // UPGRADE_WARNING: オブジェクト LocalVariableList.Item(vname).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(withBlock1.VariableType, ValueType.NumericType, false)))
                                {
                                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    num_result = Conversions.ToDouble(withBlock1.NumericValue);
                                }
                                else
                                {
                                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    string argexpr = Conversions.ToString(withBlock1.StringValue);
                                    num_result = GeneralLib.StrToDbl(ref argexpr);
                                }

                                GetVariableRet = ValueType.NumericType;
                                break;
                            }

                        case ValueType.StringType:
                            {
                                // UPGRADE_WARNING: オブジェクト LocalVariableList.Item(vname).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(withBlock1.VariableType, ValueType.StringType, false)))
                                {
                                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    str_result = Conversions.ToString(withBlock1.StringValue);
                                }
                                else
                                {
                                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    str_result = GeneralLib.FormatNum(Conversions.ToDouble(withBlock1.NumericValue));
                                }

                                GetVariableRet = ValueType.StringType;
                                break;
                            }

                        case ValueType.UndefinedType:
                            {
                                // UPGRADE_WARNING: オブジェクト LocalVariableList.Item(vname).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(withBlock1.VariableType, ValueType.StringType, false)))
                                {
                                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    str_result = Conversions.ToString(withBlock1.StringValue);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    num_result = Conversions.ToDouble(withBlock1.NumericValue);
                                    GetVariableRet = ValueType.NumericType;
                                }

                                break;
                            }
                    }
                }

                return GetVariableRet;
            }

            // グローバル変数
            if (IsGlobalVariableDefined(ref vname))
            {
                {
                    var withBlock2 = Event_Renamed.GlobalVariableList[vname];
                    switch (etype)
                    {
                        case ValueType.NumericType:
                            {
                                // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item(vname).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(withBlock2.VariableType, ValueType.NumericType, false)))
                                {
                                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    num_result = Conversions.ToDouble(withBlock2.NumericValue);
                                }
                                else
                                {
                                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    string argexpr1 = Conversions.ToString(withBlock2.StringValue);
                                    num_result = GeneralLib.StrToDbl(ref argexpr1);
                                }

                                GetVariableRet = ValueType.NumericType;
                                break;
                            }

                        case ValueType.StringType:
                            {
                                // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item(vname).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(withBlock2.VariableType, ValueType.StringType, false)))
                                {
                                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    str_result = Conversions.ToString(withBlock2.StringValue);
                                }
                                else
                                {
                                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    str_result = GeneralLib.FormatNum(Conversions.ToDouble(withBlock2.NumericValue));
                                }

                                GetVariableRet = ValueType.StringType;
                                break;
                            }

                        case ValueType.UndefinedType:
                            {
                                // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item(vname).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(withBlock2.VariableType, ValueType.StringType, false)))
                                {
                                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    str_result = Conversions.ToString(withBlock2.StringValue);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    num_result = Conversions.ToDouble(withBlock2.NumericValue);
                                    GetVariableRet = ValueType.NumericType;
                                }

                                break;
                            }
                    }
                }

                return GetVariableRet;
            }

            // システム変数？
            switch (vname ?? "")
            {
                case "対象ユニット":
                case "対象パイロット":
                    {
                        if (Event_Renamed.SelectedUnitForEvent is object)
                        {
                            {
                                var withBlock3 = Event_Renamed.SelectedUnitForEvent;
                                if (withBlock3.CountPilot() > 0)
                                {
                                    str_result = withBlock3.MainPilot().ID;
                                }
                                else
                                {
                                    str_result = "";
                                }
                            }
                        }
                        else
                        {
                            str_result = "";
                        }

                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "相手ユニット":
                case "相手パイロット":
                    {
                        if (Event_Renamed.SelectedTargetForEvent is object)
                        {
                            {
                                var withBlock4 = Event_Renamed.SelectedTargetForEvent;
                                if (withBlock4.CountPilot() > 0)
                                {
                                    str_result = withBlock4.MainPilot().ID;
                                }
                                else
                                {
                                    str_result = "";
                                }
                            }
                        }
                        else
                        {
                            str_result = "";
                        }

                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "対象ユニットＩＤ":
                    {
                        if (Event_Renamed.SelectedUnitForEvent is object)
                        {
                            str_result = Event_Renamed.SelectedUnitForEvent.ID;
                        }
                        else
                        {
                            str_result = "";
                        }

                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "相手ユニットＩＤ":
                    {
                        if (Event_Renamed.SelectedTargetForEvent is object)
                        {
                            str_result = Event_Renamed.SelectedTargetForEvent.ID;
                        }
                        else
                        {
                            str_result = "";
                        }

                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "対象ユニット使用武器":
                    {
                        str_result = "";
                        if (ReferenceEquals(Event_Renamed.SelectedUnitForEvent, Commands.SelectedUnit))
                        {
                            {
                                var withBlock5 = Event_Renamed.SelectedUnitForEvent;
                                if (Commands.SelectedWeapon > 0)
                                {
                                    str_result = Commands.SelectedWeaponName;
                                }
                                else
                                {
                                    str_result = "";
                                }
                            }
                        }
                        else if (ReferenceEquals(Event_Renamed.SelectedUnitForEvent, Commands.SelectedTarget))
                        {
                            {
                                var withBlock6 = Event_Renamed.SelectedUnitForEvent;
                                if (Commands.SelectedTWeapon > 0)
                                {
                                    str_result = Commands.SelectedTWeaponName;
                                }
                                else
                                {
                                    str_result = Commands.SelectedDefenseOption;
                                }
                            }
                        }

                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "相手ユニット使用武器":
                    {
                        str_result = "";
                        if (ReferenceEquals(Event_Renamed.SelectedTargetForEvent, Commands.SelectedTarget))
                        {
                            {
                                var withBlock7 = Event_Renamed.SelectedTargetForEvent;
                                if (Commands.SelectedTWeapon > 0)
                                {
                                    str_result = Commands.SelectedTWeaponName;
                                }
                                else
                                {
                                    str_result = Commands.SelectedDefenseOption;
                                }
                            }
                        }
                        else if (ReferenceEquals(Event_Renamed.SelectedTargetForEvent, Commands.SelectedUnit))
                        {
                            {
                                var withBlock8 = Event_Renamed.SelectedTargetForEvent;
                                if (Commands.SelectedWeapon > 0)
                                {
                                    str_result = Commands.SelectedWeaponName;
                                }
                                else
                                {
                                    str_result = "";
                                }
                            }
                        }

                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "対象ユニット使用武器番号":
                    {
                        str_result = "";
                        if (ReferenceEquals(Event_Renamed.SelectedUnitForEvent, Commands.SelectedUnit))
                        {
                            {
                                var withBlock9 = Event_Renamed.SelectedUnitForEvent;
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(Commands.SelectedWeapon);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = Commands.SelectedWeapon;
                                    GetVariableRet = ValueType.NumericType;
                                }
                            }
                        }
                        else if (ReferenceEquals(Event_Renamed.SelectedUnitForEvent, Commands.SelectedTarget))
                        {
                            {
                                var withBlock10 = Event_Renamed.SelectedUnitForEvent;
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(Commands.SelectedTWeapon);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = Commands.SelectedTWeapon;
                                    GetVariableRet = ValueType.NumericType;
                                }
                            }
                        }

                        return GetVariableRet;
                    }

                case "相手ユニット使用武器番号":
                    {
                        str_result = "";
                        if (ReferenceEquals(Event_Renamed.SelectedTargetForEvent, Commands.SelectedTarget))
                        {
                            {
                                var withBlock11 = Event_Renamed.SelectedTargetForEvent;
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(Commands.SelectedTWeapon);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = Commands.SelectedTWeapon;
                                    GetVariableRet = ValueType.NumericType;
                                }
                            }
                        }
                        else if (ReferenceEquals(Event_Renamed.SelectedTargetForEvent, Commands.SelectedUnit))
                        {
                            {
                                var withBlock12 = Event_Renamed.SelectedTargetForEvent;
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(Commands.SelectedWeapon);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = Commands.SelectedWeapon;
                                    GetVariableRet = ValueType.NumericType;
                                }
                            }
                        }

                        return GetVariableRet;
                    }

                case "対象ユニット使用アビリティ":
                    {
                        str_result = "";
                        if (ReferenceEquals(Event_Renamed.SelectedUnitForEvent, Commands.SelectedUnit))
                        {
                            {
                                var withBlock13 = Event_Renamed.SelectedUnitForEvent;
                                if (Commands.SelectedAbility > 0)
                                {
                                    str_result = Commands.SelectedAbilityName;
                                }
                                else
                                {
                                    str_result = "";
                                }
                            }
                        }

                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "対象ユニット使用アビリティ番号":
                    {
                        str_result = "";
                        if (ReferenceEquals(Event_Renamed.SelectedUnitForEvent, Commands.SelectedUnit))
                        {
                            {
                                var withBlock14 = Event_Renamed.SelectedUnitForEvent;
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(Commands.SelectedAbility);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = Commands.SelectedAbility;
                                    GetVariableRet = ValueType.NumericType;
                                }
                            }
                        }

                        return GetVariableRet;
                    }

                case "対象ユニット使用スペシャルパワー":
                    {
                        str_result = "";
                        if (ReferenceEquals(Event_Renamed.SelectedUnitForEvent, Commands.SelectedUnit))
                        {
                            str_result = Commands.SelectedSpecialPower;
                        }

                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "サポートアタックユニットＩＤ":
                    {
                        if (Commands.SupportAttackUnit is object)
                        {
                            str_result = Commands.SupportAttackUnit.ID;
                        }
                        else
                        {
                            str_result = "";
                        }

                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "サポートガードユニットＩＤ":
                    {
                        if (Commands.SupportGuardUnit is object)
                        {
                            str_result = Commands.SupportGuardUnit.ID;
                        }
                        else
                        {
                            str_result = "";
                        }

                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "選択":
                    {
                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(ref Event_Renamed.SelectedAlternative);
                            GetVariableRet = ValueType.NumericType;
                        }
                        else
                        {
                            str_result = Event_Renamed.SelectedAlternative;
                            GetVariableRet = ValueType.StringType;
                        }

                        return GetVariableRet;
                    }

                case "ターン数":
                    {
                        if (etype == ValueType.StringType)
                        {
                            str_result = VB.Compatibility.VB6.Support.Format(SRC.Turn);
                            GetVariableRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = SRC.Turn;
                            GetVariableRet = ValueType.NumericType;
                        }

                        return GetVariableRet;
                    }

                case "総ターン数":
                    {
                        if (etype == ValueType.StringType)
                        {
                            str_result = VB.Compatibility.VB6.Support.Format(SRC.TotalTurn);
                            GetVariableRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = SRC.TotalTurn;
                            GetVariableRet = ValueType.NumericType;
                        }

                        return GetVariableRet;
                    }

                case "フェイズ":
                    {
                        str_result = SRC.Stage;
                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "味方数":
                    {
                        num = 0;
                        foreach (Unit currentU in SRC.UList)
                        {
                            u = currentU;
                            if (u.Party0 == "味方" & (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納"))
                            {
                                num = (short)(num + 1);
                            }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = VB.Compatibility.VB6.Support.Format(num);
                            GetVariableRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = num;
                            GetVariableRet = ValueType.NumericType;
                        }

                        return GetVariableRet;
                    }

                case "ＮＰＣ数":
                    {
                        num = 0;
                        foreach (Unit currentU1 in SRC.UList)
                        {
                            u = currentU1;
                            if (u.Party0 == "ＮＰＣ" & (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納"))
                            {
                                num = (short)(num + 1);
                            }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = VB.Compatibility.VB6.Support.Format(num);
                            GetVariableRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = num;
                            GetVariableRet = ValueType.NumericType;
                        }

                        return GetVariableRet;
                    }

                case "敵数":
                    {
                        num = 0;
                        foreach (Unit currentU2 in SRC.UList)
                        {
                            u = currentU2;
                            if (u.Party0 == "敵" & (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納"))
                            {
                                num = (short)(num + 1);
                            }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = VB.Compatibility.VB6.Support.Format(num);
                            GetVariableRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = num;
                            GetVariableRet = ValueType.NumericType;
                        }

                        return GetVariableRet;
                    }

                case "中立数":
                    {
                        num = 0;
                        foreach (Unit currentU3 in SRC.UList)
                        {
                            u = currentU3;
                            if (u.Party0 == "中立" & (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納"))
                            {
                                num = (short)(num + 1);
                            }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = VB.Compatibility.VB6.Support.Format(num);
                            GetVariableRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = num;
                            GetVariableRet = ValueType.NumericType;
                        }

                        return GetVariableRet;
                    }

                case "資金":
                    {
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(SRC.Money);
                            GetVariableRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = SRC.Money;
                            GetVariableRet = ValueType.NumericType;
                        }

                        return GetVariableRet;
                    }

                default:
                    {
                        // アルファベットの変数名はlow caseで判別
                        switch (Strings.LCase(vname) ?? "")
                        {
                            case "apppath":
                                {
                                    str_result = SRC.AppPath;
                                    GetVariableRet = ValueType.StringType;
                                    return GetVariableRet;
                                }

                            case "appversion":
                                {
                                    // UPGRADE_ISSUE: App オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
                                    {
                                        var withBlock15 = App;
                                        num = (short)(10000 * My.MyProject.Application.Info.Version.Major + 100 * My.MyProject.Application.Info.Version.Minor + My.MyProject.Application.Info.Version.Revision);
                                    }

                                    if (etype == ValueType.StringType)
                                    {
                                        str_result = VB.Compatibility.VB6.Support.Format(num);
                                        GetVariableRet = ValueType.StringType;
                                    }
                                    else
                                    {
                                        num_result = num;
                                        GetVariableRet = ValueType.NumericType;
                                    }

                                    return GetVariableRet;
                                }

                            case "argnum":
                                {
                                    // UpVarの呼び出し回数を累計
                                    num = Event_Renamed.UpVarLevel;
                                    i = Event_Renamed.CallDepth;
                                    while (num > 0)
                                    {
                                        i = (short)(i - num);
                                        if (i < 1)
                                        {
                                            i = 1;
                                            break;
                                        }

                                        num = Event_Renamed.UpVarLevelStack[i];
                                    }

                                    num = (short)(Event_Renamed.ArgIndex - Event_Renamed.ArgIndexStack[i - 1]);
                                    if (etype == ValueType.StringType)
                                    {
                                        str_result = VB.Compatibility.VB6.Support.Format(num);
                                        GetVariableRet = ValueType.StringType;
                                    }
                                    else
                                    {
                                        num_result = num;
                                        GetVariableRet = ValueType.NumericType;
                                    }

                                    return GetVariableRet;
                                }

                            case "basex":
                                {
                                    if (etype == ValueType.StringType)
                                    {
                                        str_result = VB.Compatibility.VB6.Support.Format(Event_Renamed.BaseX);
                                        GetVariableRet = ValueType.StringType;
                                    }
                                    else
                                    {
                                        num_result = Event_Renamed.BaseX;
                                        GetVariableRet = ValueType.NumericType;
                                    }

                                    return GetVariableRet;
                                }

                            case "basey":
                                {
                                    if (etype == ValueType.StringType)
                                    {
                                        str_result = VB.Compatibility.VB6.Support.Format(Event_Renamed.BaseY);
                                        GetVariableRet = ValueType.StringType;
                                    }
                                    else
                                    {
                                        num_result = Event_Renamed.BaseY;
                                        GetVariableRet = ValueType.NumericType;
                                    }

                                    return GetVariableRet;
                                }

                            case "extdatapath":
                                {
                                    str_result = SRC.ExtDataPath;
                                    GetVariableRet = ValueType.StringType;
                                    return GetVariableRet;
                                }

                            case "extdatapath2":
                                {
                                    str_result = SRC.ExtDataPath2;
                                    GetVariableRet = ValueType.StringType;
                                    return GetVariableRet;
                                }

                            case "mousex":
                                {
                                    if (etype == ValueType.StringType)
                                    {
                                        str_result = VB.Compatibility.VB6.Support.Format(GUI.MouseX);
                                        GetVariableRet = ValueType.StringType;
                                    }
                                    else
                                    {
                                        num_result = GUI.MouseX;
                                        GetVariableRet = ValueType.NumericType;
                                    }

                                    return GetVariableRet;
                                }

                            case "mousey":
                                {
                                    if (etype == ValueType.StringType)
                                    {
                                        str_result = VB.Compatibility.VB6.Support.Format(GUI.MouseY);
                                        GetVariableRet = ValueType.StringType;
                                    }
                                    else
                                    {
                                        num_result = GUI.MouseY;
                                        GetVariableRet = ValueType.NumericType;
                                    }

                                    return GetVariableRet;
                                }

                            case "now":
                                {
                                    str_result = Conversions.ToString(DateAndTime.Now);
                                    GetVariableRet = ValueType.StringType;
                                    return GetVariableRet;
                                }

                            case "scenariopath":
                                {
                                    str_result = SRC.ScenarioPath;
                                    GetVariableRet = ValueType.StringType;
                                    return GetVariableRet;
                                }
                        }

                        break;
                    }
            }

            // コンフィグ変数？
            if (BCVariable.IsConfig)
            {
                switch (vname ?? "")
                {
                    case "攻撃値":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = VB.Compatibility.VB6.Support.Format(BCVariable.AttackExp);
                                GetVariableRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.AttackExp;
                                GetVariableRet = ValueType.NumericType;
                            }

                            return GetVariableRet;
                        }

                    case "攻撃側ユニットＩＤ":
                        {
                            str_result = BCVariable.AtkUnit.ID;
                            GetVariableRet = ValueType.StringType;
                            return GetVariableRet;
                        }

                    case "防御側ユニットＩＤ":
                        {
                            if (BCVariable.DefUnit is object)
                            {
                                str_result = BCVariable.DefUnit.ID;
                                GetVariableRet = ValueType.StringType;
                                return GetVariableRet;
                            }

                            break;
                        }

                    case "武器番号":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = VB.Compatibility.VB6.Support.Format(BCVariable.WeaponNumber);
                                GetVariableRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.WeaponNumber;
                                GetVariableRet = ValueType.NumericType;
                            }

                            return GetVariableRet;
                        }

                    case "地形適応":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = VB.Compatibility.VB6.Support.Format(BCVariable.TerrainAdaption);
                                GetVariableRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.TerrainAdaption;
                                GetVariableRet = ValueType.NumericType;
                            }

                            return GetVariableRet;
                        }

                    case "武器威力":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = VB.Compatibility.VB6.Support.Format(BCVariable.WeaponPower);
                                GetVariableRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.WeaponPower;
                                GetVariableRet = ValueType.NumericType;
                            }

                            return GetVariableRet;
                        }

                    case "サイズ補正":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = VB.Compatibility.VB6.Support.Format(BCVariable.SizeMod);
                                GetVariableRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.SizeMod;
                                GetVariableRet = ValueType.NumericType;
                            }

                            return GetVariableRet;
                        }

                    case "装甲値":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = VB.Compatibility.VB6.Support.Format(BCVariable.Armor);
                                GetVariableRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.Armor;
                                GetVariableRet = ValueType.NumericType;
                            }

                            return GetVariableRet;
                        }

                    case "最終値":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = VB.Compatibility.VB6.Support.Format(BCVariable.LastVariable);
                                GetVariableRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.LastVariable;
                                GetVariableRet = ValueType.NumericType;
                            }

                            return GetVariableRet;
                        }

                    case "攻撃側補正":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = VB.Compatibility.VB6.Support.Format(BCVariable.AttackVariable);
                                GetVariableRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.AttackVariable;
                                GetVariableRet = ValueType.NumericType;
                            }

                            return GetVariableRet;
                        }

                    case "防御側補正":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = VB.Compatibility.VB6.Support.Format(BCVariable.DffenceVariable);
                                GetVariableRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.DffenceVariable;
                                GetVariableRet = ValueType.NumericType;
                            }

                            return GetVariableRet;
                        }

                    case "ザコ補正":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = VB.Compatibility.VB6.Support.Format(BCVariable.CommonEnemy);
                                GetVariableRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.CommonEnemy;
                                GetVariableRet = ValueType.NumericType;
                            }

                            return GetVariableRet;
                        }
                }

                // パイロットに関する変数
                {
                    var withBlock16 = BCVariable.MeUnit.MainPilot();
                    switch (vname ?? "")
                    {
                        case "気力":
                            {
                                num = 0;
                                string argoname = "気力効果小";
                                if (IsOptionDefined(ref argoname))
                                {
                                    num = (short)(50 + (withBlock16.Morale + withBlock16.MoraleMod) / 2); // 気力の補正込み値を代入
                                }
                                else
                                {
                                    num = (short)(withBlock16.Morale + withBlock16.MoraleMod);
                                } // 気力の補正込み値を代入

                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(num);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = num;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "耐久":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Defense);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Defense;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "ＬＶ":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Level);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Level;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "経験":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Exp);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Exp;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "ＳＰ":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.SP);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.SP;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "霊力":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Plana);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Plana;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "格闘":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Infight);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Infight;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "射撃":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Shooting);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Shooting;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "命中":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Hit);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Hit;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "回避":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Dodge);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Dodge;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "技量":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Technique);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Technique;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "反応":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Intuition);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Intuition;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }
                    }
                }

                // ユニットに関する変数
                {
                    var withBlock17 = BCVariable.MeUnit;
                    switch (vname ?? "")
                    {
                        case "最大ＨＰ":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock17.MaxHP);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock17.MaxHP;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "現在ＨＰ":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock17.HP);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock17.HP;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "最大ＥＮ":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock17.MaxEN);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock17.MaxEN;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "現在ＥＮ":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock17.EN);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock17.EN;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "移動力":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock17.Speed);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock17.Speed;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "装甲":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock17.get_Armor(""));
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock17.get_Armor("");
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "運動性":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock17.get_Mobility(""));
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock17.get_Mobility("");
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }
                    }
                }
            }

            if (etype == ValueType.NumericType)
            {
                num_result = 0d;
                GetVariableRet = ValueType.NumericType;
            }
            else
            {
                GetVariableRet = ValueType.StringType;
            }

            return GetVariableRet;
        }

        // 指定した変数が定義されているか？
        public static bool IsVariableDefined(ref string var_name)
        {
            bool IsVariableDefinedRet = default;
            string vname;
            short i, ret;
            string ipara, idx, buf = default;
            short start_idx, depth;
            bool in_single_quote = default, in_double_quote = default;
            bool is_term;
            switch (Strings.Asc(var_name))
            {
                case 36: // $
                    {
                        vname = Strings.Mid(var_name, 2);
                        break;
                    }

                default:
                    {
                        vname = var_name;
                        break;
                    }
            }

            // 変数が配列？
            ret = (short)Strings.InStr(vname, "[");
            if (ret == 0)
            {
                goto SkipArrayHandling;
            }

            if (Strings.Right(vname, 1) != "]")
            {
                goto SkipArrayHandling;
            }

            // ここから配列専用の処理

            // インデックス部分の切りだし
            idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);

            // 多次元配列の処理
            if (Strings.InStr(idx, ",") > 0)
            {
                start_idx = 1;
                depth = 0;
                is_term = true;
                var loopTo = (short)Strings.Len(idx);
                for (i = 1; i <= loopTo; i++)
                {
                    if (in_single_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 96) // `
                        {
                            in_single_quote = false;
                        }
                    }
                    else if (in_double_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 34) // "
                        {
                            in_double_quote = false;
                        }
                    }
                    else
                    {
                        switch (Strings.Asc(Strings.Mid(idx, i, 1)))
                        {
                            case 9:
                            case 32: // タブ, 空白
                                {
                                    if (start_idx == i)
                                    {
                                        start_idx = (short)(i + 1);
                                    }
                                    else
                                    {
                                        is_term = false;
                                    }

                                    break;
                                }

                            case 40:
                            case 91: // (, 
                                {
                                    depth = (short)(depth + 1);
                                    break;
                                }

                            case 41:
                            case 93: // ), 
                                {
                                    depth = (short)(depth - 1);
                                    break;
                                }

                            case 44: // ,
                                {
                                    if (depth == 0)
                                    {
                                        if (Strings.Len(buf) > 0)
                                        {
                                            buf = buf + ",";
                                        }

                                        ipara = Strings.Trim(Strings.Mid(idx, start_idx, i - start_idx));
                                        buf = buf + GetValueAsString(ref ipara, is_term);
                                        start_idx = (short)(i + 1);
                                        is_term = true;
                                    }

                                    break;
                                }

                            case 96: // `
                                {
                                    in_single_quote = true;
                                    break;
                                }

                            case 34: // "
                                {
                                    in_double_quote = true;
                                    break;
                                }
                        }
                    }
                }

                ipara = Strings.Trim(Strings.Mid(idx, start_idx, i - start_idx));
                if (Strings.Len(buf) > 0)
                {
                    idx = buf + "," + GetValueAsString(ref ipara, is_term);
                }
                else
                {
                    idx = GetValueAsString(ref ipara, is_term);
                }
            }
            else
            {
                string argexpr = Strings.Trim(idx);
                idx = GetValueAsString(ref argexpr);
            }

            // 変数名を配列のインデックス部を計算して再構築
            vname = Strings.Left(vname, ret) + idx + "]";

        // 配列専用の処理が終了

        SkipArrayHandling:
            ;


            // ここから配列と通常変数の共通処理

            // サブルーチンローカル変数
            if (Event_Renamed.CallDepth > 0)
            {
                var loopTo1 = Event_Renamed.VarIndex;
                for (i = (short)(Event_Renamed.VarIndexStack[Event_Renamed.CallDepth - 1] + 1); i <= loopTo1; i++)
                {
                    if ((vname ?? "") == (Event_Renamed.VarStack[i].Name ?? ""))
                    {
                        IsVariableDefinedRet = true;
                        return IsVariableDefinedRet;
                    }
                }
            }

            // ローカル変数
            if (IsLocalVariableDefined(ref vname))
            {
                IsVariableDefinedRet = true;
                return IsVariableDefinedRet;
            }

            // グローバル変数
            if (IsGlobalVariableDefined(ref vname))
            {
                IsVariableDefinedRet = true;
                return IsVariableDefinedRet;
            }

            return IsVariableDefinedRet;
        }

        // 指定した名前のサブルーチンローカル変数が定義されているか？
        public static bool IsSubLocalVariableDefined(ref string vname)
        {
            bool IsSubLocalVariableDefinedRet = default;
            short i;
            if (Event_Renamed.CallDepth > 0)
            {
                var loopTo = Event_Renamed.VarIndex;
                for (i = (short)(Event_Renamed.VarIndexStack[Event_Renamed.CallDepth - 1] + 1); i <= loopTo; i++)
                {
                    if ((vname ?? "") == (Event_Renamed.VarStack[i].Name ?? ""))
                    {
                        IsSubLocalVariableDefinedRet = true;
                        return IsSubLocalVariableDefinedRet;
                    }
                }
            }

            return IsSubLocalVariableDefinedRet;
        }

        // 指定した名前のローカル変数が定義されているか？
        public static bool IsLocalVariableDefined(ref string vname)
        {
            bool IsLocalVariableDefinedRet = default;
            VarData dummy;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 296278


            Input:

                    On Error GoTo ErrorHandler

             */
            dummy = (VarData)Event_Renamed.LocalVariableList[vname];
            IsLocalVariableDefinedRet = true;
            return IsLocalVariableDefinedRet;
        ErrorHandler:
            ;
            IsLocalVariableDefinedRet = false;
        }

        // 指定した名前のグローバル変数が定義されているか？
        public static bool IsGlobalVariableDefined(ref string vname)
        {
            bool IsGlobalVariableDefinedRet = default;
            VarData dummy;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 296649


            Input:

                    On Error GoTo ErrorHandler

             */
            dummy = (VarData)Event_Renamed.GlobalVariableList[vname];
            IsGlobalVariableDefinedRet = true;
            return IsGlobalVariableDefinedRet;
        ErrorHandler:
            ;
            IsGlobalVariableDefinedRet = false;
        }

        // 変数の値を設定
        public static void SetVariable(ref string var_name, ref ValueType etype, ref string str_value, ref double num_value)
        {
            VarData new_var;
            string vname;
            short i, ret;
            string ipara, idx, buf = default;
            var vname0 = default(string);
            Pilot p;
            Unit u;
            short start_idx, depth;
            bool in_single_quote = default, in_double_quote = default;
            bool is_term;
            var is_subroutine_local_array = default(bool);

            // Debug.Print "Set " & vname & " " & new_value

            vname = var_name;

            // 左辺値を伴う関数
            ret = (short)Strings.InStr(vname, "(");
            if (ret > 1 & Strings.Right(vname, 1) == ")")
            {
                switch (Strings.LCase(Strings.Left(vname, ret - 1)) ?? "")
                {
                    case "hp":
                        {
                            idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);
                            idx = GetValueAsString(ref idx);
                            bool localIsDefined() { object argIndex1 = idx; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                            object argIndex2 = idx;
                            if (SRC.UList.IsDefined2(ref argIndex2))
                            {
                                object argIndex1 = idx;
                                u = SRC.UList.Item2(ref argIndex1);
                            }
                            else if (localIsDefined())
                            {
                                Pilot localItem() { object argIndex1 = idx; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                u = localItem().Unit_Renamed;
                            }
                            else
                            {
                                u = Event_Renamed.SelectedUnitForEvent;
                            }

                            if (u is object)
                            {
                                if (etype == ValueType.NumericType)
                                {
                                    u.HP = (int)num_value;
                                }
                                else
                                {
                                    u.HP = GeneralLib.StrToLng(ref str_value);
                                }

                                if (u.HP <= 0)
                                {
                                    u.HP = 1;
                                }
                            }

                            return;
                        }

                    case "en":
                        {
                            idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);
                            idx = GetValueAsString(ref idx);
                            bool localIsDefined1() { object argIndex1 = idx; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                            object argIndex4 = idx;
                            if (SRC.UList.IsDefined2(ref argIndex4))
                            {
                                object argIndex3 = idx;
                                u = SRC.UList.Item2(ref argIndex3);
                            }
                            else if (localIsDefined1())
                            {
                                Pilot localItem1() { object argIndex1 = idx; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                u = localItem1().Unit_Renamed;
                            }
                            else
                            {
                                u = Event_Renamed.SelectedUnitForEvent;
                            }

                            if (u is object)
                            {
                                if (etype == ValueType.NumericType)
                                {
                                    u.EN = (int)num_value;
                                }
                                else
                                {
                                    u.EN = GeneralLib.StrToLng(ref str_value);
                                }

                                if (u.EN == 0 & u.Status_Renamed == "出撃")
                                {
                                    GUI.PaintUnitBitmap(ref u);
                                }
                            }

                            return;
                        }

                    case "sp":
                        {
                            idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);
                            idx = GetValueAsString(ref idx);
                            bool localIsDefined2() { object argIndex1 = idx; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                            object argIndex6 = idx;
                            if (SRC.UList.IsDefined2(ref argIndex6))
                            {
                                Unit localItem2() { object argIndex1 = idx; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                p = localItem2().MainPilot();
                            }
                            else if (localIsDefined2())
                            {
                                object argIndex5 = idx;
                                p = SRC.PList.Item(ref argIndex5);
                            }
                            else
                            {
                                p = Event_Renamed.SelectedUnitForEvent.MainPilot();
                            }

                            if (p is object)
                            {
                                {
                                    var withBlock = p;
                                    if (withBlock.MaxSP > 0)
                                    {
                                        if (etype == ValueType.NumericType)
                                        {
                                            withBlock.SP = (int)num_value;
                                        }
                                        else
                                        {
                                            withBlock.SP = GeneralLib.StrToLng(ref str_value);
                                        }
                                    }
                                }
                            }

                            return;
                        }

                    case "plana":
                        {
                            idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);
                            idx = GetValueAsString(ref idx);
                            bool localIsDefined3() { object argIndex1 = idx; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                            object argIndex8 = idx;
                            if (SRC.UList.IsDefined2(ref argIndex8))
                            {
                                Unit localItem21() { object argIndex1 = idx; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                p = localItem21().MainPilot();
                            }
                            else if (localIsDefined3())
                            {
                                object argIndex7 = idx;
                                p = SRC.PList.Item(ref argIndex7);
                            }
                            else
                            {
                                p = Event_Renamed.SelectedUnitForEvent.MainPilot();
                            }

                            if (p is object)
                            {
                                if (p.MaxPlana() > 0)
                                {
                                    if (etype == ValueType.NumericType)
                                    {
                                        p.Plana = (int)num_value;
                                    }
                                    else
                                    {
                                        p.Plana = GeneralLib.StrToLng(ref str_value);
                                    }
                                }
                            }

                            return;
                        }

                    case "action":
                        {
                            idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);
                            idx = GetValueAsString(ref idx);
                            bool localIsDefined4() { object argIndex1 = idx; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                            object argIndex10 = idx;
                            if (SRC.UList.IsDefined2(ref argIndex10))
                            {
                                object argIndex9 = idx;
                                u = SRC.UList.Item2(ref argIndex9);
                            }
                            else if (localIsDefined4())
                            {
                                Pilot localItem3() { object argIndex1 = idx; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                u = localItem3().Unit_Renamed;
                            }
                            else
                            {
                                u = Event_Renamed.SelectedUnitForEvent;
                            }

                            if (u is object)
                            {
                                if (etype == ValueType.NumericType)
                                {
                                    u.UsedAction = (short)(u.MaxAction() - num_value);
                                }
                                else
                                {
                                    u.UsedAction = (short)(u.MaxAction() - GeneralLib.StrToLng(ref str_value));
                                }
                            }

                            return;
                        }

                    case "eval":
                        {
                            vname = Strings.Trim(Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1));
                            vname = GetValueAsString(ref vname);
                            break;
                        }
                }
            }

            // 変数が配列？
            ret = (short)Strings.InStr(vname, "[");
            if (ret == 0)
            {
                goto SkipArrayHandling;
            }

            if (Strings.Right(vname, 1) != "]")
            {
                goto SkipArrayHandling;
            }

            // ここから配列専用の処理

            // インデックス部分の切りだし
            idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);

            // 多次元配列の処理
            if (Strings.InStr(idx, ",") > 0)
            {
                start_idx = 1;
                depth = 0;
                is_term = true;
                var loopTo = (short)Strings.Len(idx);
                for (i = 1; i <= loopTo; i++)
                {
                    if (in_single_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 96) // `
                        {
                            in_single_quote = false;
                        }
                    }
                    else if (in_double_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 34) // "
                        {
                            in_double_quote = false;
                        }
                    }
                    else
                    {
                        switch (Strings.Asc(Strings.Mid(idx, i, 1)))
                        {
                            case 9:
                            case 32: // タブ, 空白
                                {
                                    if (start_idx == i)
                                    {
                                        start_idx = (short)(i + 1);
                                    }
                                    else
                                    {
                                        is_term = false;
                                    }

                                    break;
                                }

                            case 40:
                            case 91: // (, [
                                {
                                    depth = (short)(depth + 1);
                                    break;
                                }

                            case 41:
                            case 93: // ), ]
                                {
                                    depth = (short)(depth - 1);
                                    break;
                                }

                            case 44: // ,
                                {
                                    if (depth == 0)
                                    {
                                        if (Strings.Len(buf) > 0)
                                        {
                                            buf = buf + ",";
                                        }

                                        ipara = Strings.Trim(Strings.Mid(idx, start_idx, i - start_idx));
                                        buf = buf + GetValueAsString(ref ipara, is_term);
                                        start_idx = (short)(i + 1);
                                        is_term = true;
                                    }

                                    break;
                                }

                            case 96: // `
                                {
                                    in_single_quote = true;
                                    break;
                                }

                            case 34: // "
                                {
                                    in_double_quote = true;
                                    break;
                                }
                        }
                    }
                }

                ipara = Strings.Trim(Strings.Mid(idx, start_idx, i - start_idx));
                if (Strings.Len(buf) > 0)
                {
                    idx = buf + "," + GetValueAsString(ref ipara, is_term);
                }
                else
                {
                    idx = GetValueAsString(ref ipara, is_term);
                }
            }
            else
            {
                string argexpr = Strings.Trim(idx);
                idx = GetValueAsString(ref argexpr);
            }

            // 変数名を配列のインデックス部を計算して再構築
            vname = Strings.Left(vname, ret) + idx + "]";

            // 配列名
            vname0 = Strings.Left(vname, ret - 1);

            // サブルーチンローカルな配列として定義済みかどうかチェック
            if (IsSubLocalVariableDefined(ref vname0))
            {
                is_subroutine_local_array = true;
            }

        // 配列専用の処理が終了

        SkipArrayHandling:
            ;


            // ここから配列と通常変数の共通処理

            // サブルーチンローカル変数として定義済み？
            if (Event_Renamed.CallDepth > 0)
            {
                var loopTo1 = Event_Renamed.VarIndex;
                for (i = (short)(Event_Renamed.VarIndexStack[Event_Renamed.CallDepth - 1] + 1); i <= loopTo1; i++)
                {
                    {
                        var withBlock1 = Event_Renamed.VarStack[i];
                        if ((vname ?? "") == (withBlock1.Name ?? ""))
                        {
                            withBlock1.VariableType = etype;
                            withBlock1.StringValue = str_value;
                            withBlock1.NumericValue = num_value;
                            return;
                        }
                    }
                }
            }

            if (is_subroutine_local_array)
            {
                // サブルーチンローカル変数の配列の要素として定義
                Event_Renamed.VarIndex = (short)(Event_Renamed.VarIndex + 1);
                if (Event_Renamed.VarIndex > Event_Renamed.MaxVarIndex)
                {
                    Event_Renamed.VarIndex = Event_Renamed.MaxVarIndex;
                    Event_Renamed.DisplayEventErrorMessage(Event_Renamed.CurrentLineNum, "作成したサブルーチンローカル変数の総数が" + VB.Compatibility.VB6.Support.Format(Event_Renamed.MaxVarIndex) + "個を超えています");
                    return;
                }

                {
                    var withBlock2 = Event_Renamed.VarStack[Event_Renamed.VarIndex];
                    withBlock2.Name = vname;
                    withBlock2.VariableType = etype;
                    withBlock2.StringValue = str_value;
                    withBlock2.NumericValue = num_value;
                }

                return;
            }

            // ローカル変数として定義済み？
            if (IsLocalVariableDefined(ref vname))
            {
                {
                    var withBlock3 = Event_Renamed.LocalVariableList[vname];
                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().Name の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock3.Name = vname;
                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock3.VariableType = etype;
                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock3.StringValue = str_value;
                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock3.NumericValue = num_value;
                }

                return;
            }

            // グローバル変数として定義済み？
            if (IsGlobalVariableDefined(ref vname))
            {
                {
                    var withBlock4 = Event_Renamed.GlobalVariableList[vname];
                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().Name の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock4.Name = vname;
                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock4.VariableType = etype;
                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock4.StringValue = str_value;
                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock4.NumericValue = num_value;
                }

                return;
            }

            // システム変数？
            switch (Strings.LCase(vname) ?? "")
            {
                case "basex":
                    {
                        if (etype == ValueType.NumericType)
                        {
                            Event_Renamed.BaseX = (int)num_value;
                        }
                        else
                        {
                            Event_Renamed.BaseX = GeneralLib.StrToLng(ref str_value);
                        }
                        // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                        GUI.MainForm.picMain(0).CurrentX = Event_Renamed.BaseX;
                        return;
                    }

                case "basey":
                    {
                        if (etype == ValueType.NumericType)
                        {
                            Event_Renamed.BaseY = (int)num_value;
                        }
                        else
                        {
                            Event_Renamed.BaseY = GeneralLib.StrToLng(ref str_value);
                        }
                        // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                        GUI.MainForm.picMain(0).CurrentY = Event_Renamed.BaseY;
                        return;
                    }

                case "ターン数":
                    {
                        if (etype == ValueType.NumericType)
                        {
                            SRC.Turn = (short)num_value;
                        }
                        else
                        {
                            SRC.Turn = (short)GeneralLib.StrToLng(ref str_value);
                        }

                        return;
                    }

                case "総ターン数":
                    {
                        if (etype == ValueType.NumericType)
                        {
                            SRC.TotalTurn = (int)num_value;
                        }
                        else
                        {
                            SRC.TotalTurn = GeneralLib.StrToLng(ref str_value);
                        }

                        return;
                    }

                case "資金":
                    {
                        SRC.Money = 0;
                        if (etype == ValueType.NumericType)
                        {
                            SRC.IncrMoney((int)num_value);
                        }
                        else
                        {
                            SRC.IncrMoney(GeneralLib.StrToLng(ref str_value));
                        }

                        return;
                    }
            }

            // 未定義だった場合

            // 配列の要素として作成
            VarData new_var2;
            if (Strings.Len(vname0) != 0)
            {
                // ローカル変数の配列の要素として定義
                if (IsLocalVariableDefined(ref vname0))
                {
                }
                // Nop
                // グローバル変数の配列の要素として定義
                else if (IsGlobalVariableDefined(ref vname0))
                {
                    DefineGlobalVariable(ref vname);
                    {
                        var withBlock5 = Event_Renamed.GlobalVariableList[vname];
                        // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().Name の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        withBlock5.Name = vname;
                        // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        withBlock5.VariableType = etype;
                        // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        withBlock5.StringValue = str_value;
                        // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        withBlock5.NumericValue = num_value;
                    }

                    return;
                }
                // 未定義の配列なのでローカル変数の配列を作成
                else
                {
                    // ローカル変数の配列のメインＩＤを作成
                    new_var2 = new VarData();
                    new_var2.Name = vname0;
                    new_var2.VariableType = ValueType.StringType;
                    if (Strings.InStr(new_var2.Name, "\"") > 0)
                    {
                        Event_Renamed.DisplayEventErrorMessage(Event_Renamed.CurrentLineNum, "不正な変数「" + new_var2.Name + "」が作成されました");
                    }

                    Event_Renamed.LocalVariableList.Add(new_var2, vname0);
                }
            }

            // ローカル変数として作成
            new_var = new VarData();
            new_var.Name = vname;
            new_var.VariableType = etype;
            new_var.StringValue = str_value;
            new_var.NumericValue = num_value;
            if (Strings.InStr(new_var.Name, "\"") > 0)
            {
                Event_Renamed.DisplayEventErrorMessage(Event_Renamed.CurrentLineNum, "不正な変数「" + new_var.Name + "」が作成されました");
            }

            Event_Renamed.LocalVariableList.Add(new_var, vname);
        }

        public static void SetVariableAsString(ref string vname, ref string new_value)
        {
            double argnum_value = 0d;
            SetVariable(ref vname, ref ValueType.StringType, ref new_value, ref argnum_value);
        }

        public static void SetVariableAsDouble(ref string vname, double new_value)
        {
            string argstr_value = "";
            SetVariable(ref vname, ref ValueType.NumericType, ref argstr_value, ref new_value);
        }

        public static void SetVariableAsLong(ref string vname, int new_value)
        {
            string argstr_value = "";
            double argnum_value = new_value;
            SetVariable(ref vname, ref ValueType.NumericType, ref argstr_value, ref argnum_value);
        }

        // グローバル変数を定義
        public static void DefineGlobalVariable(ref string vname)
        {
            var new_var = new VarData();
            new_var.Name = vname;
            new_var.VariableType = ValueType.StringType;
            new_var.StringValue = "";
            Event_Renamed.GlobalVariableList.Add(new_var, vname);
        }

        // ローカル変数を定義
        public static void DefineLocalVariable(ref string vname)
        {
            var new_var = new VarData();
            new_var.Name = vname;
            new_var.VariableType = ValueType.StringType;
            new_var.StringValue = "";
            Event_Renamed.LocalVariableList.Add(new_var, vname);
        }

        // 変数を消去
        public static void UndefineVariable(ref string var_name)
        {
            VarData var;
            string vname, vname2;
            short i, ret;
            string idx, buf = default;
            short start_idx, depth;
            bool in_single_quote = default, in_double_quote = default;
            bool is_term;
            if (Strings.Asc(var_name) == 36) // $
            {
                vname = Strings.Mid(var_name, 2);
            }
            else
            {
                vname = var_name;
            }

            // Eval関数
            if (Strings.LCase(Strings.Left(vname, 5)) == "eval(")
            {
                if (Strings.Right(vname, 1) == ")")
                {
                    vname = Strings.Mid(vname, 6, Strings.Len(vname) - 6);
                    vname = GetValueAsString(ref vname);
                }
            }

            // 配列の要素？
            ret = (short)Strings.InStr(vname, "[");
            if (ret == 0)
            {
                goto SkipArrayHandling;
            }

            if (Strings.Right(vname, 1) != "]")
            {
                goto SkipArrayHandling;
            }

            // 配列の要素を指定された場合

            // インデックス部分の切りだし
            idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);

            // 多次元配列の処理
            if (Strings.InStr(idx, ",") > 0)
            {
                start_idx = 1;
                depth = 0;
                is_term = true;
                var loopTo = (short)Strings.Len(idx);
                for (i = 1; i <= loopTo; i++)
                {
                    if (in_single_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 96) // `
                        {
                            in_single_quote = false;
                        }
                    }
                    else if (in_double_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 34) // "
                        {
                            in_double_quote = false;
                        }
                    }
                    else
                    {
                        switch (Strings.Asc(Strings.Mid(idx, i, 1)))
                        {
                            case 9:
                            case 32: // タブ, 空白
                                {
                                    if (start_idx == i)
                                    {
                                        start_idx = (short)(i + 1);
                                    }
                                    else
                                    {
                                        is_term = false;
                                    }

                                    break;
                                }

                            case 40:
                            case 91: // (, [
                                {
                                    depth = (short)(depth + 1);
                                    break;
                                }

                            case 41:
                            case 93: // ), ]
                                {
                                    depth = (short)(depth - 1);
                                    break;
                                }

                            case 44: // ,
                                {
                                    if (depth == 0)
                                    {
                                        if (Strings.Len(buf) > 0)
                                        {
                                            buf = buf + ",";
                                        }

                                        string localGetValueAsString() { string argexpr = Strings.Mid(idx, start_idx, i - start_idx); var ret = GetValueAsString(ref argexpr, is_term); return ret; }

                                        buf = buf + localGetValueAsString();
                                        start_idx = (short)(i + 1);
                                        is_term = true;
                                    }

                                    break;
                                }

                            case 96: // `
                                {
                                    in_single_quote = true;
                                    break;
                                }

                            case 34: // "
                                {
                                    in_double_quote = true;
                                    break;
                                }
                        }
                    }
                }

                if (Strings.Len(buf) > 0)
                {
                    string localGetValueAsString1() { string argexpr = Strings.Mid(idx, start_idx, i - start_idx); var ret = GetValueAsString(ref argexpr, is_term); return ret; }

                    idx = buf + "," + localGetValueAsString1();
                }
                else
                {
                    string argexpr = Strings.Mid(idx, start_idx, i - start_idx);
                    idx = GetValueAsString(ref argexpr, is_term);
                }
            }
            else
            {
                idx = GetValueAsString(ref idx);
            }

            // インデックス部分を評価して変数名を置き換え
            vname = Strings.Left(vname, ret) + idx + "]";

            // サブルーチンローカル変数？
            if (IsSubLocalVariableDefined(ref vname))
            {
                var loopTo1 = Event_Renamed.VarIndex;
                for (i = (short)(Event_Renamed.VarIndexStack[Event_Renamed.CallDepth - 1] + 1); i <= loopTo1; i++)
                {
                    {
                        var withBlock = Event_Renamed.VarStack[i];
                        if ((vname ?? "") == (withBlock.Name ?? ""))
                        {
                            withBlock.Name = "";
                            return;
                        }
                    }
                }
            }

            // ローカル変数？
            if (IsLocalVariableDefined(ref vname))
            {
                Event_Renamed.LocalVariableList.Remove(vname);
                return;
            }

            // グローバル変数？
            if (IsGlobalVariableDefined(ref vname))
            {
                Event_Renamed.GlobalVariableList.Remove(vname);
            }

            // 配列の場合はここで終了
            return;
        SkipArrayHandling:
            ;


            // 通常の変数名を指定された場合

            // 配列要素の判定用
            vname2 = vname + "[";

            // サブルーチンローカル変数？
            if (IsSubLocalVariableDefined(ref vname))
            {
                var loopTo2 = Event_Renamed.VarIndex;
                for (i = (short)(Event_Renamed.VarIndexStack[Event_Renamed.CallDepth - 1] + 1); i <= loopTo2; i++)
                {
                    {
                        var withBlock1 = Event_Renamed.VarStack[i];
                        if ((vname ?? "") == (withBlock1.Name ?? "") | Strings.InStr(withBlock1.Name, vname2) == 1)
                        {
                            withBlock1.Name = "";
                        }
                    }
                }

                return;
            }

            // ローカル変数？
            if (IsLocalVariableDefined(ref vname))
            {
                Event_Renamed.LocalVariableList.Remove(vname);
                foreach (VarData currentVar in Event_Renamed.LocalVariableList)
                {
                    var = currentVar;
                    if (Strings.InStr(var.Name, vname2) == 1)
                    {
                        Event_Renamed.LocalVariableList.Remove(var.Name);
                    }
                }

                return;
            }

            // グローバル変数？
            if (IsGlobalVariableDefined(ref vname))
            {
                Event_Renamed.GlobalVariableList.Remove(vname);
                foreach (VarData currentVar1 in Event_Renamed.GlobalVariableList)
                {
                    var = currentVar1;
                    if (Strings.InStr(var.Name, vname2) == 1)
                    {
                        Event_Renamed.GlobalVariableList.Remove(var.Name);
                    }
                }

                return;
            }
        }

    }
}
