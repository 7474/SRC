// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Lib;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRCCore.Expressions
{
    // === 変数に関する処理 ===
    public partial class Expression
    {
        private static bool isArray(string vname)
        {
            return !string.IsNullOrEmpty(vname)
                && Strings.InStr(vname, "[") > 0
                && Strings.Right(vname, 1) == "]";
        }

        // 変数の値を評価
        public ValueType GetVariable(string var_name, ValueType etype, out string str_result, out double num_result)
        {
            ValueType GetVariableRet = default;
            string vname = var_name;

            // 未定義値の設定
            str_result = var_name;
            num_result = 0d;

            // 変数が配列？
            if (isArray(vname))
            {
                vname = ResolveArrayVarName(vname);
            }

            // ここから配列と通常変数の共通処理

            // サブルーチンローカル変数
            if (Event.CallDepth > 0)
            {
                foreach (var v in Event.SubLocalVars())
                {
                    if ((vname ?? "") == (v.Name ?? ""))
                    {
                        return v.ReferenceValue(etype, out str_result, out num_result);
                    }
                }
            }

            // ローカル変数
            if (IsLocalVariableDefined(vname))
            {
                return Event.LocalVariableList[vname].ReferenceValue(etype, out str_result, out num_result);
            }

            // グローバル変数
            if (IsGlobalVariableDefined(vname))
            {
                return Event.GlobalVariableList[vname].ReferenceValue(etype, out str_result, out num_result);
            }

            // システム変数？
            // TODO Impl
            switch (vname ?? "")
            {
                //    case "対象ユニット":
                //    case "対象パイロット":
                //        {
                //            if (Event.SelectedUnitForEvent is object)
                //            {
                //                {
                //                    var withBlock3 = Event.SelectedUnitForEvent;
                //                    if (withBlock3.CountPilot() > 0)
                //                    {
                //                        str_result = withBlock3.MainPilot().ID;
                //                    }
                //                    else
                //                    {
                //                        str_result = "";
                //                    }
                //                }
                //            }
                //            else
                //            {
                //                str_result = "";
                //            }

                //            GetVariableRet = ValueType.StringType;
                //            return GetVariableRet;
                //        }

                //    case "相手ユニット":
                //    case "相手パイロット":
                //        {
                //            if (Event.SelectedTargetForEvent is object)
                //            {
                //                {
                //                    var withBlock4 = Event.SelectedTargetForEvent;
                //                    if (withBlock4.CountPilot() > 0)
                //                    {
                //                        str_result = withBlock4.MainPilot().ID;
                //                    }
                //                    else
                //                    {
                //                        str_result = "";
                //                    }
                //                }
                //            }
                //            else
                //            {
                //                str_result = "";
                //            }

                //            GetVariableRet = ValueType.StringType;
                //            return GetVariableRet;
                //        }

                //    case "対象ユニットＩＤ":
                //        {
                //            if (Event.SelectedUnitForEvent is object)
                //            {
                //                str_result = Event.SelectedUnitForEvent.ID;
                //            }
                //            else
                //            {
                //                str_result = "";
                //            }

                //            GetVariableRet = ValueType.StringType;
                //            return GetVariableRet;
                //        }

                //    case "相手ユニットＩＤ":
                //        {
                //            if (Event.SelectedTargetForEvent is object)
                //            {
                //                str_result = Event.SelectedTargetForEvent.ID;
                //            }
                //            else
                //            {
                //                str_result = "";
                //            }

                //            GetVariableRet = ValueType.StringType;
                //            return GetVariableRet;
                //        }

                //    case "対象ユニット使用武器":
                //        {
                //            str_result = "";
                //            if (ReferenceEquals(Event.SelectedUnitForEvent, Commands.SelectedUnit))
                //            {
                //                {
                //                    var withBlock5 = Event.SelectedUnitForEvent;
                //                    if (Commands.SelectedWeapon > 0)
                //                    {
                //                        str_result = Commands.SelectedWeaponName;
                //                    }
                //                    else
                //                    {
                //                        str_result = "";
                //                    }
                //                }
                //            }
                //            else if (ReferenceEquals(Event.SelectedUnitForEvent, Commands.SelectedTarget))
                //            {
                //                {
                //                    var withBlock6 = Event.SelectedUnitForEvent;
                //                    if (Commands.SelectedTWeapon > 0)
                //                    {
                //                        str_result = Commands.SelectedTWeaponName;
                //                    }
                //                    else
                //                    {
                //                        str_result = Commands.SelectedDefenseOption;
                //                    }
                //                }
                //            }

                //            GetVariableRet = ValueType.StringType;
                //            return GetVariableRet;
                //        }

                //    case "相手ユニット使用武器":
                //        {
                //            str_result = "";
                //            if (ReferenceEquals(Event.SelectedTargetForEvent, Commands.SelectedTarget))
                //            {
                //                {
                //                    var withBlock7 = Event.SelectedTargetForEvent;
                //                    if (Commands.SelectedTWeapon > 0)
                //                    {
                //                        str_result = Commands.SelectedTWeaponName;
                //                    }
                //                    else
                //                    {
                //                        str_result = Commands.SelectedDefenseOption;
                //                    }
                //                }
                //            }
                //            else if (ReferenceEquals(Event.SelectedTargetForEvent, Commands.SelectedUnit))
                //            {
                //                {
                //                    var withBlock8 = Event.SelectedTargetForEvent;
                //                    if (Commands.SelectedWeapon > 0)
                //                    {
                //                        str_result = Commands.SelectedWeaponName;
                //                    }
                //                    else
                //                    {
                //                        str_result = "";
                //                    }
                //                }
                //            }

                //            GetVariableRet = ValueType.StringType;
                //            return GetVariableRet;
                //        }

                //    case "対象ユニット使用武器番号":
                //        {
                //            str_result = "";
                //            if (ReferenceEquals(Event.SelectedUnitForEvent, Commands.SelectedUnit))
                //            {
                //                {
                //                    var withBlock9 = Event.SelectedUnitForEvent;
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(Commands.SelectedWeapon);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = Commands.SelectedWeapon;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }
                //                }
                //            }
                //            else if (ReferenceEquals(Event.SelectedUnitForEvent, Commands.SelectedTarget))
                //            {
                //                {
                //                    var withBlock10 = Event.SelectedUnitForEvent;
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(Commands.SelectedTWeapon);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = Commands.SelectedTWeapon;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }
                //                }
                //            }

                //            return GetVariableRet;
                //        }

                //    case "相手ユニット使用武器番号":
                //        {
                //            str_result = "";
                //            if (ReferenceEquals(Event.SelectedTargetForEvent, Commands.SelectedTarget))
                //            {
                //                {
                //                    var withBlock11 = Event.SelectedTargetForEvent;
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(Commands.SelectedTWeapon);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = Commands.SelectedTWeapon;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }
                //                }
                //            }
                //            else if (ReferenceEquals(Event.SelectedTargetForEvent, Commands.SelectedUnit))
                //            {
                //                {
                //                    var withBlock12 = Event.SelectedTargetForEvent;
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(Commands.SelectedWeapon);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = Commands.SelectedWeapon;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }
                //                }
                //            }

                //            return GetVariableRet;
                //        }

                //    case "対象ユニット使用アビリティ":
                //        {
                //            str_result = "";
                //            if (ReferenceEquals(Event.SelectedUnitForEvent, Commands.SelectedUnit))
                //            {
                //                {
                //                    var withBlock13 = Event.SelectedUnitForEvent;
                //                    if (Commands.SelectedAbility > 0)
                //                    {
                //                        str_result = Commands.SelectedAbilityName;
                //                    }
                //                    else
                //                    {
                //                        str_result = "";
                //                    }
                //                }
                //            }

                //            GetVariableRet = ValueType.StringType;
                //            return GetVariableRet;
                //        }

                //    case "対象ユニット使用アビリティ番号":
                //        {
                //            str_result = "";
                //            if (ReferenceEquals(Event.SelectedUnitForEvent, Commands.SelectedUnit))
                //            {
                //                {
                //                    var withBlock14 = Event.SelectedUnitForEvent;
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(Commands.SelectedAbility);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = Commands.SelectedAbility;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }
                //                }
                //            }

                //            return GetVariableRet;
                //        }

                //    case "対象ユニット使用スペシャルパワー":
                //        {
                //            str_result = "";
                //            if (ReferenceEquals(Event.SelectedUnitForEvent, Commands.SelectedUnit))
                //            {
                //                str_result = Commands.SelectedSpecialPower;
                //            }

                //            GetVariableRet = ValueType.StringType;
                //            return GetVariableRet;
                //        }

                //    case "サポートアタックユニットＩＤ":
                //        {
                //            if (Commands.SupportAttackUnit is object)
                //            {
                //                str_result = Commands.SupportAttackUnit.ID;
                //            }
                //            else
                //            {
                //                str_result = "";
                //            }

                //            GetVariableRet = ValueType.StringType;
                //            return GetVariableRet;
                //        }

                //    case "サポートガードユニットＩＤ":
                //        {
                //            if (Commands.SupportGuardUnit is object)
                //            {
                //                str_result = Commands.SupportGuardUnit.ID;
                //            }
                //            else
                //            {
                //                str_result = "";
                //            }

                //            GetVariableRet = ValueType.StringType;
                //            return GetVariableRet;
                //        }

                case "選択":
                    if (etype == ValueType.NumericType)
                    {
                        num_result = Conversions.ToDouble(Event.SelectedAlternative);
                        GetVariableRet = ValueType.NumericType;
                    }
                    else
                    {
                        str_result = Event.SelectedAlternative;
                        GetVariableRet = ValueType.StringType;
                    }

                    return GetVariableRet;

                //    case "ターン数":
                //        {
                //            if (etype == ValueType.StringType)
                //            {
                //                str_result = VB.Compatibility.VB6.Support.Format(SRC.Turn);
                //                GetVariableRet = ValueType.StringType;
                //            }
                //            else
                //            {
                //                num_result = SRC.Turn;
                //                GetVariableRet = ValueType.NumericType;
                //            }

                //            return GetVariableRet;
                //        }

                //    case "総ターン数":
                //        {
                //            if (etype == ValueType.StringType)
                //            {
                //                str_result = VB.Compatibility.VB6.Support.Format(SRC.TotalTurn);
                //                GetVariableRet = ValueType.StringType;
                //            }
                //            else
                //            {
                //                num_result = SRC.TotalTurn;
                //                GetVariableRet = ValueType.NumericType;
                //            }

                //            return GetVariableRet;
                //        }

                //    case "フェイズ":
                //        {
                //            str_result = SRC.Stage;
                //            GetVariableRet = ValueType.StringType;
                //            return GetVariableRet;
                //        }

                //    case "味方数":
                //        {
                //            num = 0;
                //            foreach (Unit currentU in SRC.UList)
                //            {
                //                u = currentU;
                //                if (u.Party0 == "味方" & (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納"))
                //                {
                //                    num = (num + 1);
                //                }
                //            }

                //            if (etype == ValueType.StringType)
                //            {
                //                str_result = VB.Compatibility.VB6.Support.Format(num);
                //                GetVariableRet = ValueType.StringType;
                //            }
                //            else
                //            {
                //                num_result = num;
                //                GetVariableRet = ValueType.NumericType;
                //            }

                //            return GetVariableRet;
                //        }

                //    case "ＮＰＣ数":
                //        {
                //            num = 0;
                //            foreach (Unit currentU1 in SRC.UList)
                //            {
                //                u = currentU1;
                //                if (u.Party0 == "ＮＰＣ" & (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納"))
                //                {
                //                    num = (num + 1);
                //                }
                //            }

                //            if (etype == ValueType.StringType)
                //            {
                //                str_result = VB.Compatibility.VB6.Support.Format(num);
                //                GetVariableRet = ValueType.StringType;
                //            }
                //            else
                //            {
                //                num_result = num;
                //                GetVariableRet = ValueType.NumericType;
                //            }

                //            return GetVariableRet;
                //        }

                //    case "敵数":
                //        {
                //            num = 0;
                //            foreach (Unit currentU2 in SRC.UList)
                //            {
                //                u = currentU2;
                //                if (u.Party0 == "敵" & (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納"))
                //                {
                //                    num = (num + 1);
                //                }
                //            }

                //            if (etype == ValueType.StringType)
                //            {
                //                str_result = VB.Compatibility.VB6.Support.Format(num);
                //                GetVariableRet = ValueType.StringType;
                //            }
                //            else
                //            {
                //                num_result = num;
                //                GetVariableRet = ValueType.NumericType;
                //            }

                //            return GetVariableRet;
                //        }

                //    case "中立数":
                //        {
                //            num = 0;
                //            foreach (Unit currentU3 in SRC.UList)
                //            {
                //                u = currentU3;
                //                if (u.Party0 == "中立" & (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納"))
                //                {
                //                    num = (num + 1);
                //                }
                //            }

                //            if (etype == ValueType.StringType)
                //            {
                //                str_result = VB.Compatibility.VB6.Support.Format(num);
                //                GetVariableRet = ValueType.StringType;
                //            }
                //            else
                //            {
                //                num_result = num;
                //                GetVariableRet = ValueType.NumericType;
                //            }

                //            return GetVariableRet;
                //        }

                //    case "資金":
                //        {
                //            if (etype == ValueType.StringType)
                //            {
                //                str_result = GeneralLib.FormatNum(SRC.Money);
                //                GetVariableRet = ValueType.StringType;
                //            }
                //            else
                //            {
                //                num_result = SRC.Money;
                //                GetVariableRet = ValueType.NumericType;
                //            }

                //            return GetVariableRet;
                //        }

                //    default:
                //        {
                //            // アルファベットの変数名はlow caseで判別
                //            switch (Strings.LCase(vname) ?? "")
                //            {
                //                case "apppath":
                //                    {
                //                        str_result = SRC.AppPath;
                //                        GetVariableRet = ValueType.StringType;
                //                        return GetVariableRet;
                //                    }

                //                case "appversion":
                //                    {
                //                        {
                //                            var withBlock15 = App;
                //                            num = (10000 * My.MyProject.Application.Info.Version.Major + 100 * My.MyProject.Application.Info.Version.Minor + My.MyProject.Application.Info.Version.Revision);
                //                        }

                //                        if (etype == ValueType.StringType)
                //                        {
                //                            str_result = VB.Compatibility.VB6.Support.Format(num);
                //                            GetVariableRet = ValueType.StringType;
                //                        }
                //                        else
                //                        {
                //                            num_result = num;
                //                            GetVariableRet = ValueType.NumericType;
                //                        }

                //                        return GetVariableRet;
                //                    }

                //                case "argnum":
                //                    {
                //                        // UpVarの呼び出し回数を累計
                //                        num = Event.UpVarLevel;
                //                        i = Event.CallDepth;
                //                        while (num > 0)
                //                        {
                //                            i = (i - num);
                //                            if (i < 1)
                //                            {
                //                                i = 1;
                //                                break;
                //                            }

                //                            num = Event.UpVarLevelStack[i];
                //                        }

                //                        num = (Event.ArgIndex - Event.ArgIndexStack[i - 1]);
                //                        if (etype == ValueType.StringType)
                //                        {
                //                            str_result = VB.Compatibility.VB6.Support.Format(num);
                //                            GetVariableRet = ValueType.StringType;
                //                        }
                //                        else
                //                        {
                //                            num_result = num;
                //                            GetVariableRet = ValueType.NumericType;
                //                        }

                //                        return GetVariableRet;
                //                    }

                //                case "basex":
                //                    {
                //                        if (etype == ValueType.StringType)
                //                        {
                //                            str_result = VB.Compatibility.VB6.Support.Format(Event.BaseX);
                //                            GetVariableRet = ValueType.StringType;
                //                        }
                //                        else
                //                        {
                //                            num_result = Event.BaseX;
                //                            GetVariableRet = ValueType.NumericType;
                //                        }

                //                        return GetVariableRet;
                //                    }

                //                case "basey":
                //                    {
                //                        if (etype == ValueType.StringType)
                //                        {
                //                            str_result = VB.Compatibility.VB6.Support.Format(Event.BaseY);
                //                            GetVariableRet = ValueType.StringType;
                //                        }
                //                        else
                //                        {
                //                            num_result = Event.BaseY;
                //                            GetVariableRet = ValueType.NumericType;
                //                        }

                //                        return GetVariableRet;
                //                    }

                //                case "extdatapath":
                //                    {
                //                        str_result = SRC.ExtDataPath;
                //                        GetVariableRet = ValueType.StringType;
                //                        return GetVariableRet;
                //                    }

                //                case "extdatapath2":
                //                    {
                //                        str_result = SRC.ExtDataPath2;
                //                        GetVariableRet = ValueType.StringType;
                //                        return GetVariableRet;
                //                    }

                //                case "mousex":
                //                    {
                //                        if (etype == ValueType.StringType)
                //                        {
                //                            str_result = VB.Compatibility.VB6.Support.Format(GUI.MouseX);
                //                            GetVariableRet = ValueType.StringType;
                //                        }
                //                        else
                //                        {
                //                            num_result = GUI.MouseX;
                //                            GetVariableRet = ValueType.NumericType;
                //                        }

                //                        return GetVariableRet;
                //                    }

                //                case "mousey":
                //                    {
                //                        if (etype == ValueType.StringType)
                //                        {
                //                            str_result = VB.Compatibility.VB6.Support.Format(GUI.MouseY);
                //                            GetVariableRet = ValueType.StringType;
                //                        }
                //                        else
                //                        {
                //                            num_result = GUI.MouseY;
                //                            GetVariableRet = ValueType.NumericType;
                //                        }

                //                        return GetVariableRet;
                //                    }

                //                case "now":
                //                    {
                //                        str_result = Conversions.ToString(DateAndTime.Now);
                //                        GetVariableRet = ValueType.StringType;
                //                        return GetVariableRet;
                //                    }

                //                case "scenariopath":
                //                    {
                //                        str_result = SRC.ScenarioPath;
                //                        GetVariableRet = ValueType.StringType;
                //                        return GetVariableRet;
                //                    }
                //            }

                //            break;
                //        }
                default:
                    break;
            }

            // コンフィグ変数？
            // TODO Impl
            //if (BCVariable.IsConfig)
            {
                //    switch (vname ?? "")
                //    {
                //        case "攻撃値":
                //            {
                //                if (etype == ValueType.StringType)
                //                {
                //                    str_result = VB.Compatibility.VB6.Support.Format(BCVariable.AttackExp);
                //                    GetVariableRet = ValueType.StringType;
                //                }
                //                else
                //                {
                //                    num_result = BCVariable.AttackExp;
                //                    GetVariableRet = ValueType.NumericType;
                //                }

                //                return GetVariableRet;
                //            }

                //        case "攻撃側ユニットＩＤ":
                //            {
                //                str_result = BCVariable.AtkUnit.ID;
                //                GetVariableRet = ValueType.StringType;
                //                return GetVariableRet;
                //            }

                //        case "防御側ユニットＩＤ":
                //            {
                //                if (BCVariable.DefUnit is object)
                //                {
                //                    str_result = BCVariable.DefUnit.ID;
                //                    GetVariableRet = ValueType.StringType;
                //                    return GetVariableRet;
                //                }

                //                break;
                //            }

                //        case "武器番号":
                //            {
                //                if (etype == ValueType.StringType)
                //                {
                //                    str_result = VB.Compatibility.VB6.Support.Format(BCVariable.WeaponNumber);
                //                    GetVariableRet = ValueType.StringType;
                //                }
                //                else
                //                {
                //                    num_result = BCVariable.WeaponNumber;
                //                    GetVariableRet = ValueType.NumericType;
                //                }

                //                return GetVariableRet;
                //            }

                //        case "地形適応":
                //            {
                //                if (etype == ValueType.StringType)
                //                {
                //                    str_result = VB.Compatibility.VB6.Support.Format(BCVariable.TerrainAdaption);
                //                    GetVariableRet = ValueType.StringType;
                //                }
                //                else
                //                {
                //                    num_result = BCVariable.TerrainAdaption;
                //                    GetVariableRet = ValueType.NumericType;
                //                }

                //                return GetVariableRet;
                //            }

                //        case "武器威力":
                //            {
                //                if (etype == ValueType.StringType)
                //                {
                //                    str_result = VB.Compatibility.VB6.Support.Format(BCVariable.WeaponPower);
                //                    GetVariableRet = ValueType.StringType;
                //                }
                //                else
                //                {
                //                    num_result = BCVariable.WeaponPower;
                //                    GetVariableRet = ValueType.NumericType;
                //                }

                //                return GetVariableRet;
                //            }

                //        case "サイズ補正":
                //            {
                //                if (etype == ValueType.StringType)
                //                {
                //                    str_result = VB.Compatibility.VB6.Support.Format(BCVariable.SizeMod);
                //                    GetVariableRet = ValueType.StringType;
                //                }
                //                else
                //                {
                //                    num_result = BCVariable.SizeMod;
                //                    GetVariableRet = ValueType.NumericType;
                //                }

                //                return GetVariableRet;
                //            }

                //        case "装甲値":
                //            {
                //                if (etype == ValueType.StringType)
                //                {
                //                    str_result = VB.Compatibility.VB6.Support.Format(BCVariable.Armor);
                //                    GetVariableRet = ValueType.StringType;
                //                }
                //                else
                //                {
                //                    num_result = BCVariable.Armor;
                //                    GetVariableRet = ValueType.NumericType;
                //                }

                //                return GetVariableRet;
                //            }

                //        case "最終値":
                //            {
                //                if (etype == ValueType.StringType)
                //                {
                //                    str_result = VB.Compatibility.VB6.Support.Format(BCVariable.LastVariable);
                //                    GetVariableRet = ValueType.StringType;
                //                }
                //                else
                //                {
                //                    num_result = BCVariable.LastVariable;
                //                    GetVariableRet = ValueType.NumericType;
                //                }

                //                return GetVariableRet;
                //            }

                //        case "攻撃側補正":
                //            {
                //                if (etype == ValueType.StringType)
                //                {
                //                    str_result = VB.Compatibility.VB6.Support.Format(BCVariable.AttackVariable);
                //                    GetVariableRet = ValueType.StringType;
                //                }
                //                else
                //                {
                //                    num_result = BCVariable.AttackVariable;
                //                    GetVariableRet = ValueType.NumericType;
                //                }

                //                return GetVariableRet;
                //            }

                //        case "防御側補正":
                //            {
                //                if (etype == ValueType.StringType)
                //                {
                //                    str_result = VB.Compatibility.VB6.Support.Format(BCVariable.DffenceVariable);
                //                    GetVariableRet = ValueType.StringType;
                //                }
                //                else
                //                {
                //                    num_result = BCVariable.DffenceVariable;
                //                    GetVariableRet = ValueType.NumericType;
                //                }

                //                return GetVariableRet;
                //            }

                //        case "ザコ補正":
                //            {
                //                if (etype == ValueType.StringType)
                //                {
                //                    str_result = VB.Compatibility.VB6.Support.Format(BCVariable.CommonEnemy);
                //                    GetVariableRet = ValueType.StringType;
                //                }
                //                else
                //                {
                //                    num_result = BCVariable.CommonEnemy;
                //                    GetVariableRet = ValueType.NumericType;
                //                }

                //                return GetVariableRet;
                //            }
                //    }

                //    // パイロットに関する変数
                //    {
                //        var withBlock16 = BCVariable.MeUnit.MainPilot();
                //        switch (vname ?? "")
                //        {
                //            case "気力":
                //                {
                //                    num = 0;
                //                    string argoname = "気力効果小";
                //                    if (IsOptionDefined(argoname))
                //                    {
                //                        num = (50 + (withBlock16.Morale + withBlock16.MoraleMod) / 2); // 気力の補正込み値を代入
                //                    }
                //                    else
                //                    {
                //                        num = (withBlock16.Morale + withBlock16.MoraleMod);
                //                    } // 気力の補正込み値を代入

                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(num);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = num;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }

                //                    return GetVariableRet;
                //                }

                //            case "耐久":
                //                {
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Defense);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = withBlock16.Defense;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }

                //                    return GetVariableRet;
                //                }

                //            case "ＬＶ":
                //                {
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Level);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = withBlock16.Level;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }

                //                    return GetVariableRet;
                //                }

                //            case "経験":
                //                {
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Exp);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = withBlock16.Exp;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }

                //                    return GetVariableRet;
                //                }

                //            case "ＳＰ":
                //                {
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(withBlock16.SP);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = withBlock16.SP;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }

                //                    return GetVariableRet;
                //                }

                //            case "霊力":
                //                {
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Plana);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = withBlock16.Plana;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }

                //                    return GetVariableRet;
                //                }

                //            case "格闘":
                //                {
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Infight);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = withBlock16.Infight;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }

                //                    return GetVariableRet;
                //                }

                //            case "射撃":
                //                {
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Shooting);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = withBlock16.Shooting;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }

                //                    return GetVariableRet;
                //                }

                //            case "命中":
                //                {
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Hit);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = withBlock16.Hit;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }

                //                    return GetVariableRet;
                //                }

                //            case "回避":
                //                {
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Dodge);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = withBlock16.Dodge;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }

                //                    return GetVariableRet;
                //                }

                //            case "技量":
                //                {
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Technique);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = withBlock16.Technique;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }

                //                    return GetVariableRet;
                //                }

                //            case "反応":
                //                {
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Intuition);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = withBlock16.Intuition;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }

                //                    return GetVariableRet;
                //                }
                //        }
                //    }

                //    // ユニットに関する変数
                //    {
                //        var withBlock17 = BCVariable.MeUnit;
                //        switch (vname ?? "")
                //        {
                //            case "最大ＨＰ":
                //                {
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(withBlock17.MaxHP);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = withBlock17.MaxHP;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }

                //                    return GetVariableRet;
                //                }

                //            case "現在ＨＰ":
                //                {
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(withBlock17.HP);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = withBlock17.HP;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }

                //                    return GetVariableRet;
                //                }

                //            case "最大ＥＮ":
                //                {
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(withBlock17.MaxEN);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = withBlock17.MaxEN;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }

                //                    return GetVariableRet;
                //                }

                //            case "現在ＥＮ":
                //                {
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(withBlock17.EN);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = withBlock17.EN;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }

                //                    return GetVariableRet;
                //                }

                //            case "移動力":
                //                {
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(withBlock17.Speed);
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = withBlock17.Speed;
                //                        GetVariableRet = ValueType.NumericType;
                //                    }

                //                    return GetVariableRet;
                //                }

                //            case "装甲":
                //                {
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(withBlock17.get_Armor(""));
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = withBlock17.get_Armor("");
                //                        GetVariableRet = ValueType.NumericType;
                //                    }

                //                    return GetVariableRet;
                //                }

                //            case "運動性":
                //                {
                //                    if (etype == ValueType.StringType)
                //                    {
                //                        str_result = VB.Compatibility.VB6.Support.Format(withBlock17.get_Mobility(""));
                //                        GetVariableRet = ValueType.StringType;
                //                    }
                //                    else
                //                    {
                //                        num_result = withBlock17.get_Mobility("");
                //                        GetVariableRet = ValueType.NumericType;
                //                    }

                //                    return GetVariableRet;
                //                }
                //        }
                //    }
            }

            if (etype == ValueType.NumericType)
            {
                GetVariableRet = ValueType.NumericType;
            }
            else
            {
                GetVariableRet = ValueType.StringType;
            }

            return GetVariableRet;
        }

        private string ResolveArrayVarName(string vname)
        {
            // XXX 正規表現のほうが楽に書けそうではある
            // インデックス部分の切りだし
            var bracketPos = Strings.InStr(vname, "[");
            var idx = Strings.Mid(vname, bracketPos + 1, Strings.Len(vname) - bracketPos - 1);

            // 多次元配列の処理
            if (Strings.InStr(idx, ",") > 0)
            {
                int start_idx = 1;
                int depth = 0;
                bool is_term = true;
                bool in_single_quote = false;
                bool in_double_quote = false;
                // XXX StringBuilder
                string buf = "";
                string ipara;
                var loopTo = Strings.Len(idx);
                int i;
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
                                        start_idx = (i + 1);
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
                                    depth = (depth + 1);
                                    break;
                                }

                            case 41:
                            case 93: // ), ]
                                {
                                    depth = (depth - 1);
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
                                        buf = buf + GetValueAsString(ipara, is_term);
                                        start_idx = (i + 1);
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
                    idx = buf + "," + GetValueAsString(ipara, is_term);
                }
                else
                {
                    idx = GetValueAsString(ipara, is_term);
                }
            }
            else
            {
                idx = GetValueAsString(idx);
            }

            // 変数名を配列のインデックス部を計算して再構築
            return Strings.Left(vname, bracketPos) + idx + "]";
        }

        // 指定した変数が定義されているか？
        public bool IsVariableDefined(string var_name)
        {
            bool IsVariableDefinedRet = default;
            string vname;
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
            if (isArray(vname))
            {
                vname = ResolveArrayVarName(vname);
            }

            // ここから配列と通常変数の共通処理

            // サブルーチンローカル変数
            if (IsSubLocalVariableDefined(vname))
            {
                return true;
            }

            // ローカル変数
            if (IsLocalVariableDefined(vname))
            {
                IsVariableDefinedRet = true;
                return IsVariableDefinedRet;
            }

            // グローバル変数
            if (IsGlobalVariableDefined(vname))
            {
                IsVariableDefinedRet = true;
                return IsVariableDefinedRet;
            }

            return IsVariableDefinedRet;
        }

        // 指定した名前のサブルーチンローカル変数が定義されているか？
        public bool IsSubLocalVariableDefined(string vname)
        {
            return Event.SubLocalVar(vname) != null;
        }

        // 指定した名前のローカル変数が定義されているか？
        public bool IsLocalVariableDefined(string vname)
        {
            return Event.LocalVariableList.ContainsKey(vname);
        }

        // 指定した名前のグローバル変数が定義されているか？
        public bool IsGlobalVariableDefined(string vname)
        {
            return Event.GlobalVariableList.ContainsKey(vname);
        }

        // 変数の値を設定
        public void SetVariable(string var_name, ValueType etype, string str_value, double num_value)
        {
            VarData new_var;
            string vname = var_name;
            int i, ret;
            var vname0 = default(string);
            var is_subroutine_local_array = default(bool);

            // Debug.Print "Set " & vname & " " & new_value

            // 左辺値を伴う関数
            // TODO Impl
            ret = Strings.InStr(vname, "(");
            if (ret > 1 & Strings.Right(vname, 1) == ")")
            {
                //    switch (Strings.LCase(Strings.Left(vname, ret - 1)) ?? "")
                //    {
                //        case "hp":
                //            {
                //                idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);
                //                idx = GetValueAsString(idx);
                //                bool localIsDefined() { object argIndex1 = idx; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                //                object argIndex2 = idx;
                //                if (SRC.UList.IsDefined2(argIndex2))
                //                {
                //                    object argIndex1 = idx;
                //                    u = SRC.UList.Item2(argIndex1);
                //                }
                //                else if (localIsDefined())
                //                {
                //                    Pilot localItem() { object argIndex1 = idx; var ret = SRC.PList.Item(argIndex1); return ret; }

                //                    u = localItem().Unit_Renamed;
                //                }
                //                else
                //                {
                //                    u = Event.SelectedUnitForEvent;
                //                }

                //                if (u is object)
                //                {
                //                    if (etype == ValueType.NumericType)
                //                    {
                //                        u.HP = num_value;
                //                    }
                //                    else
                //                    {
                //                        u.HP = GeneralLib.StrToLng(str_value);
                //                    }

                //                    if (u.HP <= 0)
                //                    {
                //                        u.HP = 1;
                //                    }
                //                }

                //                return;
                //            }

                //        case "en":
                //            {
                //                idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);
                //                idx = GetValueAsString(idx);
                //                bool localIsDefined1() { object argIndex1 = idx; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                //                object argIndex4 = idx;
                //                if (SRC.UList.IsDefined2(argIndex4))
                //                {
                //                    object argIndex3 = idx;
                //                    u = SRC.UList.Item2(argIndex3);
                //                }
                //                else if (localIsDefined1())
                //                {
                //                    Pilot localItem1() { object argIndex1 = idx; var ret = SRC.PList.Item(argIndex1); return ret; }

                //                    u = localItem1().Unit_Renamed;
                //                }
                //                else
                //                {
                //                    u = Event.SelectedUnitForEvent;
                //                }

                //                if (u is object)
                //                {
                //                    if (etype == ValueType.NumericType)
                //                    {
                //                        u.EN = num_value;
                //                    }
                //                    else
                //                    {
                //                        u.EN = GeneralLib.StrToLng(str_value);
                //                    }

                //                    if (u.EN == 0 & u.Status_Renamed == "出撃")
                //                    {
                //                        GUI.PaintUnitBitmap(u);
                //                    }
                //                }

                //                return;
                //            }

                //        case "sp":
                //            {
                //                idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);
                //                idx = GetValueAsString(idx);
                //                bool localIsDefined2() { object argIndex1 = idx; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                //                object argIndex6 = idx;
                //                if (SRC.UList.IsDefined2(argIndex6))
                //                {
                //                    Unit localItem2() { object argIndex1 = idx; var ret = SRC.UList.Item2(argIndex1); return ret; }

                //                    p = localItem2().MainPilot();
                //                }
                //                else if (localIsDefined2())
                //                {
                //                    object argIndex5 = idx;
                //                    p = SRC.PList.Item(argIndex5);
                //                }
                //                else
                //                {
                //                    p = Event.SelectedUnitForEvent.MainPilot();
                //                }

                //                if (p is object)
                //                {
                //                    {
                //                        var withBlock = p;
                //                        if (withBlock.MaxSP > 0)
                //                        {
                //                            if (etype == ValueType.NumericType)
                //                            {
                //                                withBlock.SP = num_value;
                //                            }
                //                            else
                //                            {
                //                                withBlock.SP = GeneralLib.StrToLng(str_value);
                //                            }
                //                        }
                //                    }
                //                }

                //                return;
                //            }

                //        case "plana":
                //            {
                //                idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);
                //                idx = GetValueAsString(idx);
                //                bool localIsDefined3() { object argIndex1 = idx; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                //                object argIndex8 = idx;
                //                if (SRC.UList.IsDefined2(argIndex8))
                //                {
                //                    Unit localItem21() { object argIndex1 = idx; var ret = SRC.UList.Item2(argIndex1); return ret; }

                //                    p = localItem21().MainPilot();
                //                }
                //                else if (localIsDefined3())
                //                {
                //                    object argIndex7 = idx;
                //                    p = SRC.PList.Item(argIndex7);
                //                }
                //                else
                //                {
                //                    p = Event.SelectedUnitForEvent.MainPilot();
                //                }

                //                if (p is object)
                //                {
                //                    if (p.MaxPlana() > 0)
                //                    {
                //                        if (etype == ValueType.NumericType)
                //                        {
                //                            p.Plana = num_value;
                //                        }
                //                        else
                //                        {
                //                            p.Plana = GeneralLib.StrToLng(str_value);
                //                        }
                //                    }
                //                }

                //                return;
                //            }

                //        case "action":
                //            {
                //                idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);
                //                idx = GetValueAsString(idx);
                //                bool localIsDefined4() { object argIndex1 = idx; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                //                object argIndex10 = idx;
                //                if (SRC.UList.IsDefined2(argIndex10))
                //                {
                //                    object argIndex9 = idx;
                //                    u = SRC.UList.Item2(argIndex9);
                //                }
                //                else if (localIsDefined4())
                //                {
                //                    Pilot localItem3() { object argIndex1 = idx; var ret = SRC.PList.Item(argIndex1); return ret; }

                //                    u = localItem3().Unit_Renamed;
                //                }
                //                else
                //                {
                //                    u = Event.SelectedUnitForEvent;
                //                }

                //                if (u is object)
                //                {
                //                    if (etype == ValueType.NumericType)
                //                    {
                //                        u.UsedAction = (u.MaxAction() - num_value);
                //                    }
                //                    else
                //                    {
                //                        u.UsedAction = (u.MaxAction() - GeneralLib.StrToLng(str_value));
                //                    }
                //                }

                //                return;
                //            }

                //        case "eval":
                //            {
                //                vname = Strings.Trim(Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1));
                //                vname = GetValueAsString(vname);
                //                break;
                //            }
                //    }
            }

            // 変数が配列？
            if (isArray(vname))
            {
                vname = ResolveArrayVarName(vname);

                // 配列名
                vname0 = Strings.Left(vname, Strings.InStr(vname, "[") - 1);

                // サブルーチンローカルな配列として定義済みかどうかチェック
                if (IsSubLocalVariableDefined(vname0))
                {
                    is_subroutine_local_array = true;
                }
            }

            // ここから配列と通常変数の共通処理

            // サブルーチンローカル変数として定義済み？
            if (IsSubLocalVariableDefined(vname))
            {
                var v = Event.SubLocalVar(vname);
                if (v != null)
                {
                    v.VariableType = etype;
                    v.StringValue = str_value;
                    v.NumericValue = num_value;
                    return;
                }
            }

            if (is_subroutine_local_array)
            {
                // サブルーチンローカル変数の配列の要素として定義
                Event.VarIndex = Event.VarIndex + 1;
                if (Event.VarIndex >= Events.Event.MaxVarIndex)
                {
                    Event.VarIndex = Events.Event.MaxVarIndex;
                    Event.DisplayEventErrorMessage(Event.CurrentLineNum, "作成したサブルーチンローカル変数の総数が" + SrcFormatter.Format(Events.Event.MaxVarIndex) + "個を超えています");
                    return;
                }

                var v = Event.VarStack[Event.VarIndex];
                v.Name = vname;
                v.VariableType = etype;
                v.StringValue = str_value;
                v.NumericValue = num_value;
                return;
            }

            // ローカル変数として定義済み？
            if (IsLocalVariableDefined(vname))
            {
                var v = Event.LocalVariableList[vname];
                v.Name = vname;
                v.VariableType = etype;
                v.StringValue = str_value;
                v.NumericValue = num_value;
                return;
            }

            // グローバル変数として定義済み？
            if (IsGlobalVariableDefined(vname))
            {
                var v = Event.GlobalVariableList[vname];
                v.Name = vname;
                v.VariableType = etype;
                v.StringValue = str_value;
                v.NumericValue = num_value;
                return;
            }

            // システム変数？
            switch (Strings.LCase(vname) ?? "")
            {
                case "basex":
                    {
                        if (etype == ValueType.NumericType)
                        {
                            Event.BaseX = (int)num_value;
                        }
                        else
                        {
                            Event.BaseX = GeneralLib.StrToLng(str_value);
                        }
                        // TODO GUI
                        //GUI.MainForm.picMain(0).CurrentX = Event.BaseX;
                        return;
                    }

                case "basey":
                    {
                        if (etype == ValueType.NumericType)
                        {
                            Event.BaseY = (int)num_value;
                        }
                        else
                        {
                            Event.BaseY = GeneralLib.StrToLng(str_value);
                        }
                        // TODO GUI
                        //GUI.MainForm.picMain(0).CurrentY = Event.BaseY;
                        return;
                    }

                case "ターン数":
                    {
                        if (etype == ValueType.NumericType)
                        {
                            SRC.Turn = (int)num_value;
                        }
                        else
                        {
                            SRC.Turn = GeneralLib.StrToLng(str_value);
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
                            SRC.TotalTurn = GeneralLib.StrToLng(str_value);
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
                            SRC.IncrMoney(GeneralLib.StrToLng(str_value));
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
                if (IsLocalVariableDefined(vname0))
                {
                }
                // Nop
                // グローバル変数の配列の要素として定義
                else if (IsGlobalVariableDefined(vname0))
                {
                    DefineGlobalVariable(vname);
                    var v = Event.GlobalVariableList[vname];
                    v.Name = vname;
                    v.VariableType = etype;
                    v.StringValue = str_value;
                    v.NumericValue = num_value;

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
                        Event.DisplayEventErrorMessage(Event.CurrentLineNum, "不正な変数「" + new_var2.Name + "」が作成されました");
                    }

                    Event.LocalVariableList.Add(vname0, new_var2);
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
                Event.DisplayEventErrorMessage(Event.CurrentLineNum, "不正な変数「" + new_var.Name + "」が作成されました");
            }

            Event.LocalVariableList.Add(vname, new_var);
        }

        public void SetVariableAsString(string vname, string new_value)
        {
            double argnum_value = 0d;
            SetVariable(vname, ValueType.StringType, new_value, argnum_value);
        }

        public void SetVariableAsDouble(string vname, double new_value)
        {
            string argstr_value = "";
            SetVariable(vname, ValueType.NumericType, argstr_value, new_value);
        }

        public void SetVariableAsLong(string vname, int new_value)
        {
            string argstr_value = "";
            double argnum_value = new_value;
            SetVariable(vname, ValueType.NumericType, argstr_value, argnum_value);
        }

        // グローバル変数を定義
        public void DefineGlobalVariable(string vname)
        {
            var new_var = new VarData();
            new_var.Name = vname;
            new_var.VariableType = ValueType.StringType;
            new_var.StringValue = "";
            Event.GlobalVariableList.Add(vname, new_var);
        }

        // ローカル変数を定義
        public void DefineLocalVariable(string vname)
        {
            var new_var = new VarData();
            new_var.Name = vname;
            new_var.VariableType = ValueType.StringType;
            new_var.StringValue = "";
            Event.LocalVariableList.Add(vname, new_var);
        }

        // 変数を消去
        public void UndefineVariable(string var_name)
        {
            string vname, vname2;
            int i, ret;
            string idx, buf = default;
            int start_idx, depth;
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
                    vname = GetValueAsString(vname);
                }
            }

            // 配列の要素？
            ret = Strings.InStr(vname, "[");
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
                var loopTo = Strings.Len(idx);
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
                                        start_idx = (i + 1);
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
                                    depth = (depth + 1);
                                    break;
                                }

                            case 41:
                            case 93: // ), ]
                                {
                                    depth = (depth - 1);
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

                                        string argexpr = Strings.Mid(idx, start_idx, i - start_idx);
                                        buf = buf + GetValueAsString(argexpr, is_term);
                                        start_idx = (i + 1);
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
                    string argexpr = Strings.Mid(idx, start_idx, i - start_idx);
                    idx = buf + "," + GetValueAsString(argexpr, is_term);
                }
                else
                {
                    string argexpr = Strings.Mid(idx, start_idx, i - start_idx);
                    idx = GetValueAsString(argexpr, is_term);
                }
            }
            else
            {
                idx = GetValueAsString(idx);
            }

            // インデックス部分を評価して変数名を置き換え
            vname = Strings.Left(vname, ret) + idx + "]";

            // サブルーチンローカル変数？
            if (IsSubLocalVariableDefined(vname))
            {
                // XXX 名前消すだけでいいの？
                var v = Event.SubLocalVar(vname);
                if (v != null)
                {
                    v.Name = "";
                    return;
                }
            }

            // ローカル変数？
            if (IsLocalVariableDefined(vname))
            {
                Event.LocalVariableList.Remove(vname);
                return;
            }

            // グローバル変数？
            if (IsGlobalVariableDefined(vname))
            {
                Event.GlobalVariableList.Remove(vname);
            }

            // 配列の場合はここで終了
            return;
        SkipArrayHandling:
            ;


            // 通常の変数名を指定された場合

            // 配列要素の判定用
            vname2 = vname + "[";

            // サブルーチンローカル変数？
            if (IsSubLocalVariableDefined(vname))
            {
                // XXX 配列の取得
                foreach (var v in Event.SubLocalVars())
                {

                    if ((vname ?? "") == (v.Name ?? "") | Strings.InStr(v.Name, vname2) == 1)
                    {
                        v.Name = "";
                    }
                }
                return;
            }

            // ローカル変数？
            if (IsLocalVariableDefined(vname))
            {
                Event.LocalVariableList.Remove(vname);
                foreach (VarData currentVar in Event.LocalVariableList.Values)
                {
                    if (Strings.InStr(currentVar.Name, vname2) == 1)
                    {
                        Event.LocalVariableList.Remove(currentVar.Name);
                    }
                }

                return;
            }

            // グローバル変数？
            if (IsGlobalVariableDefined(vname))
            {
                Event.GlobalVariableList.Remove(vname);
                foreach (VarData currentVar in Event.GlobalVariableList.Values)
                {
                    if (Strings.InStr(currentVar.Name, vname2) == 1)
                    {
                        Event.GlobalVariableList.Remove(currentVar.Name);
                    }
                }

                return;
            }
        }
    }
}
