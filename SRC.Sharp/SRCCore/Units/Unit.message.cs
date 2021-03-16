// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Units
{
    // TODO Impl
    // === メッセージ関連処理 ===
    public partial class Unit
    {
        // 状況 Situation に応じたパイロットメッセージを表示
        public void PilotMessage(string Situation, string msg_mode = "")
        {
        //    int k, i, j, w;
        //    Pilot p;
        //    MessageData md;
        //    Dialog dd;
        //    string msg;
        //    string[] situations;
        //    var wname = default(string);
        //    string[] pnames;
        //    string buf;
        //    var selected_pilot = default(string);
        //    var selected_situation = default(string);
        //    var selected_msg = default(string);

        //    // WAVEを演奏したかチェックするため、あらかじめクリア
        //    Sound.IsWavePlayed = false;

        //    // 対応メッセージが定義されていなかった場合に使用するシチュエーションを設定
        //    situations = new string[2];
        //    situations[1] = Situation;
        //    switch (Situation ?? "")
        //    {
        //        case "分身":
        //        case "切り払い":
        //        case "迎撃":
        //        case "反射":
        //        case "当て身技":
        //        case "阻止":
        //        case "ダミー":
        //        case "緊急テレポート":
        //            {
        //                Array.Resize(situations, 3);
        //                situations[2] = "回避";
        //                break;
        //            }

        //        case "ビーム無効化":
        //        case "攻撃無効化":
        //        case "シールド防御":
        //            {
        //                Array.Resize(situations, 3);
        //                situations[2] = "ダメージ小";
        //                break;
        //            }

        //        case "回避":
        //        case "破壊":
        //        case "ダメージ大":
        //        case "ダメージ中":
        //        case "ダメージ小":
        //        case "かけ声":
        //            {
        //                break;
        //            }

        //        default:
        //            {
        //                if (msg_mode == "攻撃" | msg_mode == "カウンター")
        //                {
        //                    // 攻撃メッセージ
        //                    wname = situations[1];

        //                    // 武器番号を検索
        //                    var loopTo = CountWeapon();
        //                    for (w = 1; w <= loopTo; w++)
        //                    {
        //                        {
        //                            var withBlock = Weapon(w);
        //                            if ((Situation ?? "") == (withBlock.Name ?? "") | (Situation ?? "") == (withBlock.Nickname() ?? ""))
        //                            {
        //                                break;
        //                            }
        //                        }
        //                    }

        //                    if (!IsDefense())
        //                    {
        //                        Array.Resize(situations, 3);
        //                        string argattr = "格闘系";
        //                        if (IsWeaponClassifiedAs(w, argattr))
        //                        {
        //                            situations[2] = "格闘";
        //                        }
        //                        else
        //                        {
        //                            situations[2] = "射撃";
        //                        }
        //                    }
        //                    else if (msg_mode == "カウンター")
        //                    {
        //                        Array.Resize(situations, 4);
        //                        situations[1] = Situation + "(反撃)";
        //                        situations[2] = Situation;
        //                        string argattr2 = "格闘系";
        //                        if (IsWeaponClassifiedAs(w, argattr2))
        //                        {
        //                            situations[3] = "格闘";
        //                        }
        //                        else
        //                        {
        //                            situations[3] = "射撃";
        //                        }
        //                    }
        //                    else
        //                    {
        //                        Array.Resize(situations, 5);
        //                        situations[1] = Situation + "(反撃)";
        //                        situations[2] = Situation;
        //                        string argattr1 = "格闘系";
        //                        if (IsWeaponClassifiedAs(w, argattr1))
        //                        {
        //                            situations[3] = "格闘(反撃)";
        //                            situations[4] = "格闘";
        //                        }
        //                        else
        //                        {
        //                            situations[3] = "射撃(反撃)";
        //                            situations[4] = "射撃";
        //                        }
        //                    }
        //                }
        //                else if (msg_mode == "アビリティ")
        //                {
        //                    Array.Resize(situations, 3);
        //                    situations[2] = "アビリティ";
        //                }
        //                else if (Strings.InStr(Situation, "(命中)") > 0 | Strings.InStr(Situation, "(回避)") > 0 | Strings.InStr(Situation, "(とどめ)") > 0 | Strings.InStr(Situation, "(クリティカル)") > 0)
        //                {
        //                    // サブシチュエーション付きの攻撃メッセージ

        //                    // 武器番号を検索
        //                    string argstr2 = "(";
        //                    wname = Strings.Left(Situation, GeneralLib.InStr2(Situation, argstr2) - 1);
        //                    var loopTo1 = CountWeapon();
        //                    for (w = 1; w <= loopTo1; w++)
        //                    {
        //                        {
        //                            var withBlock1 = Weapon(w);
        //                            if ((wname ?? "") == (withBlock1.Name ?? "") | (wname ?? "") == (withBlock1.Nickname() ?? ""))
        //                            {
        //                                break;
        //                            }
        //                        }
        //                    }

        //                    if (!IsDefense())
        //                    {
        //                        Array.Resize(situations, 3);
        //                        string argattr3 = "格闘系";
        //                        if (IsWeaponClassifiedAs(w, argattr3))
        //                        {
        //                            string argstr21 = "(";
        //                            situations[2] = "格闘" + Strings.Mid(Situation, GeneralLib.InStr2(Situation, argstr21));
        //                        }
        //                        else
        //                        {
        //                            string argstr22 = "(";
        //                            situations[2] = "射撃" + Strings.Mid(Situation, GeneralLib.InStr2(Situation, argstr22));
        //                        }
        //                    }
        //                    else if (msg_mode == "カウンター")
        //                    {
        //                        Array.Resize(situations, 4);
        //                        situations[1] = Situation + "(反撃)";
        //                        situations[2] = Situation;
        //                        string argattr5 = "格闘系";
        //                        if (IsWeaponClassifiedAs(w, argattr5))
        //                        {
        //                            string argstr27 = "(";
        //                            situations[3] = "格闘" + Strings.Mid(Situation, GeneralLib.InStr2(Situation, argstr27));
        //                        }
        //                        else
        //                        {
        //                            string argstr28 = "(";
        //                            situations[3] = "射撃" + Strings.Mid(Situation, GeneralLib.InStr2(Situation, argstr28));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        Array.Resize(situations, 5);
        //                        situations[1] = Situation + "(反撃)";
        //                        situations[2] = Situation;
        //                        string argattr4 = "格闘系";
        //                        if (IsWeaponClassifiedAs(w, argattr4))
        //                        {
        //                            string argstr23 = "(";
        //                            situations[3] = "格闘" + Strings.Mid(Situation, GeneralLib.InStr2(Situation, argstr23)) + "(反撃)";
        //                            string argstr24 = "(";
        //                            situations[4] = "格闘" + Strings.Mid(Situation, GeneralLib.InStr2(Situation, argstr24));
        //                        }
        //                        else
        //                        {
        //                            string argstr25 = "(";
        //                            situations[3] = "射撃" + Strings.Mid(Situation, GeneralLib.InStr2(Situation, argstr25)) + "(反撃)";
        //                            string argstr26 = "(";
        //                            situations[4] = "射撃" + Strings.Mid(Situation, GeneralLib.InStr2(Situation, argstr26));
        //                        }
        //                    }
        //                }
        //                // 攻撃メッセージでなくても一応攻撃武器名を設定
        //                else if (ReferenceEquals(Commands.SelectedUnit, this))
        //                {
        //                    if (0 < Commands.SelectedWeapon & Commands.SelectedWeapon <= CountWeapon())
        //                    {
        //                        wname = Weapon(Commands.SelectedWeapon).Name;
        //                    }
        //                }
        //                else if (ReferenceEquals(Commands.SelectedTarget, this))
        //                {
        //                    if (0 < Commands.SelectedTWeapon & Commands.SelectedTWeapon <= CountWeapon())
        //                    {
        //                        wname = Weapon(Commands.SelectedTWeapon).Name;
        //                    }
        //                }

        //                break;
        //            }
        //    }

        //    // SelectMessageコマンド
        //    var loopTo2 = Information.UBound(situations);
        //    for (i = 1; i <= loopTo2; i++)
        //    {
        //        buf = "Message(" + MainPilot().ID + "," + situations[i] + ")";
        //        if (Expression.IsLocalVariableDefined(buf))
        //        {
        //            // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //            selected_msg = Conversions.ToString(Event_Renamed.LocalVariableList[buf].StringValue);
        //            selected_situation = situations[i];
        //            Expression.UndefineVariable(buf);
        //            break;
        //        }

        //        if (situations[i] == "格闘" | situations[i] == "射撃")
        //        {
        //            buf = "Message(" + MainPilot().ID + ",攻撃)";
        //            if (Expression.IsLocalVariableDefined(buf))
        //            {
        //                // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //                selected_msg = Conversions.ToString(Event_Renamed.LocalVariableList[buf].StringValue);
        //                selected_situation = "攻撃";
        //                Expression.UndefineVariable(buf);
        //                break;
        //            }
        //        }

        //        if (situations[i] == "格闘(反撃)" | situations[i] == "射撃(反撃)")
        //        {
        //            buf = "Message(" + MainPilot().ID + ",攻撃(反撃))";
        //            if (Expression.IsLocalVariableDefined(buf))
        //            {
        //                // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //                selected_msg = Conversions.ToString(Event_Renamed.LocalVariableList[buf].StringValue);
        //                selected_situation = "攻撃(反撃)";
        //                Expression.UndefineVariable(buf);
        //                break;
        //            }
        //        }
        //    }

        //    if (Strings.InStr(selected_msg, "::") > 0)
        //    {
        //        selected_pilot = Strings.Left(selected_msg, Strings.InStr(selected_msg, "::") - 1);
        //        selected_msg = Strings.Mid(selected_msg, Strings.InStr(selected_msg, "::") + 2);
        //    }

        //    // かけ声は３分の２の確率で表示
        //    if (string.IsNullOrEmpty(selected_msg))
        //    {
        //        if (Strings.InStr(Situation, "かけ声") == 1)
        //        {
        //            if (GeneralLib.Dice(3) == 1)
        //            {
        //                return;
        //            }
        //        }
        //    }

        //    // しゃべれない場合
        //    // ただしSetMessageコマンドでメッセージが設定されている場合は
        //    // そちらを使用。
        //    if (string.IsNullOrEmpty(selected_msg))
        //    {
        //        object argIndex1 = "石化";
        //        object argIndex2 = "凍結";
        //        object argIndex3 = "麻痺";
        //        if (IsConditionSatisfied(argIndex1) | IsConditionSatisfied(argIndex2) | IsConditionSatisfied(argIndex3))
        //        {
        //            // 意識不明
        //            return;
        //        }

        //        object argIndex6 = "沈黙";
        //        object argIndex7 = "憑依";
        //        if (IsConditionSatisfied(argIndex6) | IsConditionSatisfied(argIndex7))
        //        {
        //            // 無言
        //            if (Strings.InStr(Situation, "(") == 0)
        //            {
        //                switch (Situation ?? "")
        //                {
        //                    case "ダメージ中":
        //                    case "ダメージ大":
        //                    case "破壊":
        //                        {
        //                            object argIndex4 = MainPilot().Name + "(ダメージ)";
        //                            if (SRC.NPDList.IsDefined(argIndex4))
        //                            {
        //                                string argpname = MainPilot().Name + "(ダメージ)";
        //                                string argmsg_mode = "";
        //                                GUI.DisplayBattleMessage(argpname, "…………！", msg_mode: argmsg_mode);
        //                                return;
        //                            }

        //                            break;
        //                        }

        //                    case "かけ声":
        //                        {
        //                            return;
        //                        }
        //                }

        //                if (!string.IsNullOrEmpty(wname))
        //                {
        //                    object argIndex5 = MainPilot().Name + "(攻撃)";
        //                    if (SRC.NPDList.IsDefined(argIndex5))
        //                    {
        //                        string argpname1 = MainPilot().Name + "(攻撃)";
        //                        string argmsg_mode1 = "";
        //                        GUI.DisplayBattleMessage(argpname1, "…………！", msg_mode: argmsg_mode1);
        //                        return;
        //                    }
        //                }

        //                string argmsg_mode2 = "";
        //                GUI.DisplayBattleMessage(MainPilot().ID, "…………", msg_mode: argmsg_mode2);
        //            }

        //            return;
        //        }

        //        object argIndex8 = "睡眠";
        //        if (IsConditionSatisfied(argIndex8))
        //        {
        //            // 寝言
        //            if (Strings.InStr(Situation, "(") == 0)
        //            {
        //                string argmsg_mode3 = "";
        //                GUI.DisplayBattleMessage(MainPilot().ID, "ＺＺＺ……", msg_mode: argmsg_mode3);
        //            }

        //            return;
        //        }

        //        object argIndex10 = "恐怖";
        //        if (IsConditionSatisfied(argIndex10))
        //        {
        //            string argmain_situation = "恐怖";
        //            if (IsMessageDefined(argmain_situation))
        //            {
        //                // 恐怖状態用メッセージが定義されていればそちらを使う
        //                situations = new string[2];
        //                situations[1] = "恐怖";
        //            }
        //            else
        //            {
        //                // パニック時のメッセージを作成して表示
        //                if (Strings.InStr(Situation, "(") == 0)
        //                {
        //                    msg = "";
        //                    switch (MainPilot().Sex ?? "")
        //                    {
        //                        case "男性":
        //                            {
        //                                switch (GeneralLib.Dice(4))
        //                                {
        //                                    case 1:
        //                                        {
        //                                            msg = "う、うわああああっ！";
        //                                            break;
        //                                        }

        //                                    case 2:
        //                                        {
        //                                            msg = "うわあああっ！";
        //                                            break;
        //                                        }

        //                                    case 3:
        //                                        {
        //                                            msg = "ひ、ひいいいっ！";
        //                                            break;
        //                                        }

        //                                    case 4:
        //                                        {
        //                                            msg = "ひいいいっ！";
        //                                            break;
        //                                        }
        //                                }

        //                                break;
        //                            }

        //                        case "女性":
        //                            {
        //                                switch (GeneralLib.Dice(4))
        //                                {
        //                                    case 1:
        //                                        {
        //                                            msg = "きゃあああああっ！";
        //                                            break;
        //                                        }

        //                                    case 2:
        //                                        {
        //                                            msg = "きゃああっ！";
        //                                            break;
        //                                        }

        //                                    case 3:
        //                                        {
        //                                            msg = "うわあああっ！";
        //                                            break;
        //                                        }

        //                                    case 4:
        //                                        {
        //                                            msg = "た、助けてええっ！";
        //                                            break;
        //                                        }
        //                                }

        //                                break;
        //                            }
        //                    }

        //                    if (!string.IsNullOrEmpty(msg))
        //                    {
        //                        bool localIsDefined() { object argIndex1 = MainPilot().Name + "(ダメージ)"; var ret = SRC.NPDList.IsDefined(argIndex1); return ret; }

        //                        object argIndex9 = MainPilot().Name + "(泣き)";
        //                        if (SRC.NPDList.IsDefined(argIndex9))
        //                        {
        //                            string argpname2 = MainPilot().Name + "(泣き)";
        //                            string argmsg_mode4 = "";
        //                            GUI.DisplayBattleMessage(argpname2, msg, msg_mode: argmsg_mode4);
        //                        }
        //                        else if (localIsDefined())
        //                        {
        //                            string argpname3 = MainPilot().Name + "(ダメージ)";
        //                            string argmsg_mode6 = "";
        //                            GUI.DisplayBattleMessage(argpname3, msg, msg_mode: argmsg_mode6);
        //                        }
        //                        else
        //                        {
        //                            string argmsg_mode5 = "";
        //                            GUI.DisplayBattleMessage(MainPilot().ID, msg, msg_mode: argmsg_mode5);
        //                        }
        //                    }
        //                }

        //                return;
        //            }
        //        }

        //        object argIndex11 = "混乱";
        //        if (IsConditionSatisfied(argIndex11))
        //        {
        //            string argmain_situation1 = "混乱";
        //            if (IsMessageDefined(argmain_situation1))
        //            {
        //                // 混乱状態用メッセージが定義されていればそちらを使う
        //                situations = new string[2];
        //                situations[1] = "混乱";
        //            }
        //        }
        //    }

        //    // ダイアログデータを使って判定
        //    pnames = new string[4];
        //    pnames[1] = MainPilot().MessageType;
        //    pnames[2] = MainPilot().MessageType;
        //    pnames[3] = MainPilot().MessageType;
        //    string argfname = "追加パイロット";
        //    if (IsFeatureAvailable(argfname))
        //    {
        //        Array.Resize(pnames, 5);
        //        object argIndex12 = 1;
        //        pnames[4] = Pilot(argIndex12).MessageType;
        //    }

        //    var loopTo3 = CountPilot();
        //    for (i = 2; i <= loopTo3; i++)
        //    {
        //        Pilot localPilot() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

        //        pnames[1] = pnames[1] + " " + localPilot().MessageType;
        //        Pilot localPilot1() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

        //        pnames[2] = pnames[2] + " " + localPilot1().MessageType;
        //    }

        //    var loopTo4 = CountSupport();
        //    for (i = 1; i <= loopTo4; i++)
        //    {
        //        Pilot localSupport() { object argIndex1 = i; var ret = Support(argIndex1); return ret; }

        //        pnames[1] = pnames[1] + " " + localSupport().MessageType;
        //    }

        //    string argfname1 = "追加サポート";
        //    if (IsFeatureAvailable(argfname1))
        //    {
        //        pnames[1] = pnames[1] + " " + AdditionalSupport().MessageType;
        //    }

        //    if ((Situation ?? "") == (Commands.SelectedSpecialPower ?? ""))
        //    {
        //        pnames[3] = Commands.SelectedPilot.MessageType;
        //    }

        //    var loopTo5 = Information.UBound(pnames);
        //    for (i = 1; i <= loopTo5; i++)
        //    {
        //        // 追加パイロットにメッセージデータがあればそちらを優先
        //        if (i == 4)
        //        {
        //            bool localIsDefined1() { object argIndex1 = MainPilot().MessageType; var ret = SRC.MDList.IsDefined(argIndex1); MainPilot().MessageType = Conversions.ToString(argIndex1); return ret; }

        //            if (localIsDefined1())
        //            {
        //                break;
        //            }
        //        }

        //        var tmp1 = pnames;
        //        object argIndex14 = tmp1[i];
        //        if (SRC.DDList.IsDefined(argIndex14))
        //        {
        //            var tmp = pnames;
        //            object argIndex13 = tmp[i];
        //            {
        //                var withBlock2 = SRC.DDList.Item(argIndex13);
        //                if (!string.IsNullOrEmpty(selected_msg))
        //                {
        //                    // SelectMessageで選択されたメッセージを検索
        //                    k = 0;
        //                    var loopTo6 = withBlock2.CountDialog();
        //                    for (j = 1; j <= loopTo6; j++)
        //                    {
        //                        if ((withBlock2.Situation(j) ?? "") == (selected_situation ?? ""))
        //                        {
        //                            k = (k + 1);
        //                            if ((SrcFormatter.Format(k) ?? "") == (selected_msg ?? ""))
        //                            {
        //                                var argdd = withBlock2.Dialog(j);
        //                                PlayDialog(argdd, wname);
        //                                return;
        //                            }
        //                        }
        //                        else if ((withBlock2.Situation(j) ?? "") == (selected_msg ?? ""))
        //                        {
        //                            var argdd1 = withBlock2.Dialog(j);
        //                            PlayDialog(argdd1, wname);
        //                            return;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    var loopTo7 = Information.UBound(situations);
        //                    for (j = 1; j <= loopTo7; j++)
        //                    {
        //                        var argu = this;
        //                        dd = withBlock2.SelectDialog(situations[j], argu);
        //                        if (dd is object)
        //                        {
        //                            PlayDialog(dd, wname);
        //                            return;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    // ゲッターのようなユニットは必ずメインパイロットを使ってメッセージを表示
        //    object argIndex15 = 1;
        //    if (Data.PilotNum > 0 & ReferenceEquals(MainPilot(), Pilot(argIndex15)) & (Situation ?? "") != (Commands.SelectedSpecialPower ?? ""))
        //    {
        //        i = GeneralLib.Dice(CountPilot() + CountSupport());
        //    }
        //    else
        //    {
        //        i = 1;
        //    }

        //TryAgain:
        //    ;

        //    // 選んだパイロットによるメッセージデータで判定
        //    if ((Situation ?? "") == (Commands.SelectedSpecialPower ?? ""))
        //    {
        //        bool localIsDefined2() { object argIndex1 = Commands.SelectedPilot.MessageType; var ret = SRC.MDList.IsDefined(argIndex1); Commands.SelectedPilot.MessageType = Conversions.ToString(argIndex1); return ret; }

        //        if (!localIsDefined2())
        //        {
        //            goto TrySelectedMessage;
        //        }
        //    }
        //    else if (i == 1)
        //    {
        //        bool localIsDefined4() { object argIndex1 = MainPilot().MessageType; var ret = SRC.MDList.IsDefined(argIndex1); MainPilot().MessageType = Conversions.ToString(argIndex1); return ret; }

        //        bool localIsDefined5() { object argIndex1 = 1; object argIndex2 = Pilot(argIndex1).MessageType; var ret = SRC.MDList.IsDefined(argIndex2); Pilot(argIndex1).MessageType = Conversions.ToString(argIndex2); return ret; }

        //        if (!localIsDefined4() & !localIsDefined5())
        //        {
        //            goto TrySelectedMessage;
        //        }
        //    }
        //    else if (i <= CountPilot())
        //    {
        //        Pilot localPilot2() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

        //        bool localIsDefined6() { object argIndex1 = (object)hsfce6a149060e4f46a8f718c3a6bd3f01().MessageType; var ret = SRC.MDList.IsDefined(argIndex1); hsfce6a149060e4f46a8f718c3a6bd3f01().MessageType = Conversions.ToString(argIndex1); return ret; }

        //        if (!localIsDefined6())
        //        {
        //            i = 1;
        //            goto TryAgain;
        //        }
        //    }
        //    else
        //    {
        //        Pilot localSupport1() { object argIndex1 = i - CountPilot(); var ret = Support(argIndex1); return ret; }

        //        bool localIsDefined3() { object argIndex1 = (object)hsaf4c50d4b23f4cb1b890b9fa34bacc97().MessageType; var ret = SRC.MDList.IsDefined(argIndex1); hsaf4c50d4b23f4cb1b890b9fa34bacc97().MessageType = Conversions.ToString(argIndex1); return ret; }

        //        if (!localIsDefined3())
        //        {
        //            if (i > 1)
        //            {
        //                i = 1;
        //                goto TryAgain;
        //            }
        //            else
        //            {
        //                goto TrySelectedMessage;
        //            }
        //        }
        //    }

        //    // メッセージを表示
        //    if ((Situation ?? "") == (Commands.SelectedSpecialPower ?? ""))
        //    {
        //        object argIndex16 = Commands.SelectedPilot.MessageType;
        //        md = SRC.MDList.Item(argIndex16);
        //        Commands.SelectedPilot.MessageType = Conversions.ToString(argIndex16);
        //        p = Commands.SelectedPilot;
        //    }
        //    else if (i == 1)
        //    {
        //        object argIndex19 = MainPilot().MessageType;
        //        md = SRC.MDList.Item(argIndex19);
        //        MainPilot().MessageType = Conversions.ToString(argIndex19);
        //        p = MainPilot();
        //        if (md is object)
        //        {
        //            var loopTo8 = Information.UBound(situations);
        //            for (j = 1; j <= loopTo8; j++)
        //            {
        //                var argu1 = this;
        //                if (Strings.Len(md.SelectMessage(situations[j], argu1)) > 0)
        //                {
        //                    break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            object argIndex20 = 1;
        //            object argIndex21 = Pilot(argIndex20).MessageType;
        //            md = SRC.MDList.Item(argIndex21);
        //            Pilot(argIndex20).MessageType = Conversions.ToString(argIndex21);
        //            object argIndex22 = 1;
        //            p = Pilot(argIndex22);
        //        }
        //    }
        //    else if (i <= CountPilot())
        //    {
        //        Pilot localPilot3() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

        //        object argIndex23 = localPilot3().MessageType;
        //        md = SRC.MDList.Item(argIndex23);
        //        localPilot3().MessageType = Conversions.ToString(argIndex23);
        //        object argIndex24 = i;
        //        p = Pilot(argIndex24);
        //    }
        //    else
        //    {
        //        Pilot localSupport2() { object argIndex1 = i - CountPilot(); var ret = Support(argIndex1); return ret; }

        //        object argIndex17 = localSupport2().MessageType;
        //        md = SRC.MDList.Item(argIndex17);
        //        localSupport2().MessageType = Conversions.ToString(argIndex17);
        //        object argIndex18 = i - CountPilot();
        //        p = Support(argIndex18);
        //    }

        //    // メッセージデータが見つからない場合は他のパイロットで探しなおす
        //    if (md is null)
        //    {
        //        if (i != 1)
        //        {
        //            i = 1;
        //            goto TryAgain;
        //        }
        //        else
        //        {
        //            goto TrySelectedMessage;
        //        }
        //    }

        //    if (!string.IsNullOrEmpty(selected_msg))
        //    {
        //        // SelectMessageで選択されたメッセージを検索
        //        k = 0;
        //        var loopTo9 = md.CountMessage();
        //        for (j = 1; j <= loopTo9; j++)
        //        {
        //            if ((md.Situation(j) ?? "") == (selected_situation ?? ""))
        //            {
        //                k = (k + 1);
        //                if ((SrcFormatter.Format(k) ?? "") == (selected_msg ?? ""))
        //                {
        //                    string argmsg = md.Message(j);
        //                    PlayMessage(p, argmsg, wname);
        //                    return;
        //                }
        //            }
        //            else if ((md.Situation(j) ?? "") == (selected_msg ?? ""))
        //            {
        //                string argmsg1 = md.Message(j);
        //                PlayMessage(p, argmsg1, wname);
        //                return;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // メッセージデータからメッセージを検索
        //        var loopTo10 = Information.UBound(situations);
        //        for (j = 1; j <= loopTo10; j++)
        //        {
        //            var argu2 = this;
        //            msg = md.SelectMessage(situations[j], argu2);
        //            if (!string.IsNullOrEmpty(msg))
        //            {
        //                PlayMessage(p, msg, wname);
        //                return;
        //            }
        //        }
        //    }

        //    if (i != 1)
        //    {
        //        i = 1;
        //        goto TryAgain;
        //    }

        //TrySelectedMessage:
        //    ;

        //    // メッセージを表示
        //    if (!string.IsNullOrEmpty(selected_msg) & selected_msg != "-")
        //    {
        //        if (!string.IsNullOrEmpty(selected_pilot))
        //        {
        //            string argmsg_mode7 = "";
        //            GUI.DisplayBattleMessage(selected_pilot, selected_msg, msg_mode: argmsg_mode7);
        //        }
        //        else
        //        {
        //            string argmsg_mode8 = "";
        //            GUI.DisplayBattleMessage(MainPilot().ID, selected_msg, msg_mode: argmsg_mode8);
        //        }
        //    }
        }

        // ダイアログの再生
        public void PlayDialog(Dialog dd, string wname)
        {
            //string msg, buf;
            //int i, idx;
            //Unit t;
            //int tw;

            //// 画像描画が行われたかどうかの判定のためにフラグを初期化
            //GUI.IsPictureDrawn = false;
            //// ダイアログの個々のメッセージを表示
            //var loopTo = dd.Count;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    msg = dd.Message(i);

            //    // メッセージ表示のキャンセル
            //    if (msg == "-")
            //    {
            //        return;
            //    }

            //    // ユニット名
            //    while (Strings.InStr(msg, "$(ユニット)") > 0)
            //    {
            //        idx = Strings.InStr(msg, "$(ユニット)");
            //        buf = Nickname;
            //        if (Strings.InStr(buf, "(") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //        }

            //        if (Strings.InStr(buf, "専用") > 0)
            //        {
            //            buf = Strings.Mid(buf, Strings.InStr(buf, "専用") + 2);
            //        }

            //        msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 7);
            //    }

            //    while (Strings.InStr(msg, "$(機体)") > 0)
            //    {
            //        idx = Strings.InStr(msg, "$(機体)");
            //        buf = Nickname;
            //        if (Strings.InStr(buf, "(") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //        }

            //        if (Strings.InStr(buf, "専用") > 0)
            //        {
            //            buf = Strings.Mid(buf, Strings.InStr(buf, "専用") + 2);
            //        }

            //        msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 5);
            //    }

            //    // パイロット名
            //    while (Strings.InStr(msg, "$(パイロット)") > 0)
            //    {
            //        buf = MainPilot().get_Nickname(false);
            //        if (Strings.InStr(buf, "(") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //        }

            //        idx = Strings.InStr(msg, "$(パイロット)");
            //        msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 8);
            //    }

            //    // 武器名
            //    while (Strings.InStr(msg, "$(武器)") > 0)
            //    {
            //        idx = Strings.InStr(msg, "$(武器)");
            //        buf = wname;
            //        Expression.ReplaceSubExpression(buf);
            //        if (Strings.InStr(buf, "(") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //        }

            //        if (Strings.InStr(buf, "<") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "<") - 1);
            //        }

            //        if (Strings.InStr(buf, "＜") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "＜") - 1);
            //        }

            //        msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 5);
            //    }

            //    // 損傷率
            //    while (Strings.InStr(msg, "$(損傷率)") > 0)
            //    {
            //        idx = Strings.InStr(msg, "$(損傷率)");
            //        msg = Strings.Left(msg, idx - 1) + SrcFormatter.Format(100 * (MaxHP - HP) / MaxHP) + Strings.Mid(msg, idx + 6);
            //    }

            //    // 相手ユニットを設定
            //    if (ReferenceEquals(Commands.SelectedUnit, this))
            //    {
            //        t = Commands.SelectedTarget;
            //        tw = Commands.SelectedTWeapon;
            //    }
            //    else
            //    {
            //        t = Commands.SelectedUnit;
            //        tw = Commands.SelectedWeapon;
            //    }

            //    if (t is object)
            //    {
            //        // 相手ユニット名
            //        while (Strings.InStr(msg, "$(相手ユニット)") > 0)
            //        {
            //            buf = t.Nickname;
            //            if (Strings.InStr(buf, "(") > 0)
            //            {
            //                buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //            }

            //            if (Strings.InStr(buf, "専用") > 0)
            //            {
            //                buf = Strings.Mid(buf, Strings.InStr(buf, "専用") + 2);
            //            }

            //            idx = Strings.InStr(msg, "$(相手ユニット)");
            //            msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 9);
            //        }

            //        while (Strings.InStr(msg, "$(相手機体)") > 0)
            //        {
            //            buf = t.Nickname;
            //            if (Strings.InStr(buf, "(") > 0)
            //            {
            //                buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //            }

            //            if (Strings.InStr(buf, "専用") > 0)
            //            {
            //                buf = Strings.Mid(buf, Strings.InStr(buf, "専用") + 2);
            //            }

            //            idx = Strings.InStr(msg, "$(相手機体)");
            //            msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 7);
            //        }

            //        // 相手パイロット名
            //        while (Strings.InStr(msg, "$(相手パイロット)") > 0)
            //        {
            //            buf = t.MainPilot().get_Nickname(false);
            //            if (Strings.InStr(buf, "(") > 0)
            //            {
            //                buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //            }

            //            idx = Strings.InStr(msg, "$(相手パイロット)");
            //            msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 10);
            //        }

            //        // 相手武器名
            //        while (Strings.InStr(msg, "$(相手武器)") > 0)
            //        {
            //            if (1 <= tw & tw <= t.CountWeapon())
            //            {
            //                buf = t.WeaponNickname(tw);
            //            }
            //            else
            //            {
            //                buf = "";
            //            }

            //            if (Strings.InStr(buf, "<") > 0)
            //            {
            //                buf = Strings.Left(buf, Strings.InStr(buf, "<") - 1);
            //            }

            //            if (Strings.InStr(buf, "＜") > 0)
            //            {
            //                buf = Strings.Left(buf, Strings.InStr(buf, "＜") - 1);
            //            }

            //            idx = Strings.InStr(msg, "$(相手武器)");
            //            msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 7);
            //        }

            //        // 相手損傷率
            //        while (Strings.InStr(msg, "$(相手損傷率)") > 0)
            //        {
            //            buf = SrcFormatter.Format(100 * (t.MaxHP - t.HP) / t.MaxHP);
            //            idx = Strings.InStr(msg, "$(相手損傷率)");
            //            msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 8);
            //        }
            //    }

            //    // メッセージを表示
            //    if ((dd.Name(i) ?? "") == (MainPilot().Name ?? ""))
            //    {
            //        string argmsg_mode = "";
            //        GUI.DisplayBattleMessage(MainPilot().ID, msg, msg_mode: argmsg_mode);
            //    }
            //    else if (Strings.Left(dd.Name(i), 1) == "@")
            //    {
            //        string argpname1 = Strings.Mid(dd.Name(i), 2);
            //        string argmsg_mode2 = "";
            //        GUI.DisplayBattleMessage(argpname1, msg, msg_mode: argmsg_mode2);
            //    }
            //    else
            //    {
            //        string argpname = dd.Name(i);
            //        string argmsg_mode1 = "";
            //        GUI.DisplayBattleMessage(argpname, msg, msg_mode: argmsg_mode1);
            //    }
            //}

            //// カットインは消去しておく
            //string argoname = "戦闘中画面初期化無効";
            //if (!Expression.IsOptionDefined(argoname))
            //{
            //    if (GUI.IsPictureDrawn)
            //    {
            //        GUI.ClearPicture();
            //        // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //        GUI.MainForm.picMain(0).Refresh();
            //    }
            //}
        }

        // メッセージを再生
        public void PlayMessage(Pilot p, string msg, string wname)
        {
            //string buf;
            //int idx;
            //Unit t;
            //int tw;

            //// メッセージ表示をキャンセル
            //if (msg == "-")
            //{
            //    return;
            //}

            //// 画像描画が行われたかどうかの判定のためにフラグを初期化
            //GUI.IsPictureDrawn = false;

            //// ユニット名
            //while (Strings.InStr(msg, "$(ユニット)") > 0)
            //{
            //    idx = Strings.InStr(msg, "$(ユニット)");
            //    buf = Nickname;
            //    if (Strings.InStr(buf, "(") > 0)
            //    {
            //        buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //    }

            //    if (Strings.InStr(buf, "専用") > 0)
            //    {
            //        buf = Strings.Mid(buf, Strings.InStr(buf, "専用") + 2);
            //    }

            //    msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 7);
            //}

            //while (Strings.InStr(msg, "$(機体)") > 0)
            //{
            //    idx = Strings.InStr(msg, "$(機体)");
            //    buf = Nickname;
            //    if (Strings.InStr(buf, "(") > 0)
            //    {
            //        buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //    }

            //    if (Strings.InStr(buf, "専用") > 0)
            //    {
            //        buf = Strings.Mid(buf, Strings.InStr(buf, "専用") + 2);
            //    }

            //    msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 5);
            //}

            //// パイロット名
            //while (Strings.InStr(msg, "$(パイロット)") > 0)
            //{
            //    buf = MainPilot().get_Nickname(false);
            //    if (Strings.InStr(buf, "(") > 0)
            //    {
            //        buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //    }

            //    idx = Strings.InStr(msg, "$(パイロット)");
            //    msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 8);
            //}

            //// 武器名
            //while (Strings.InStr(msg, "$(武器)") > 0)
            //{
            //    idx = Strings.InStr(msg, "$(武器)");
            //    buf = wname;
            //    Expression.ReplaceSubExpression(ref buf);
            //    if (Strings.InStr(buf, "(") > 0)
            //    {
            //        buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //    }

            //    if (Strings.InStr(buf, "<") > 0)
            //    {
            //        buf = Strings.Left(buf, Strings.InStr(buf, "<") - 1);
            //    }

            //    if (Strings.InStr(buf, "＜") > 0)
            //    {
            //        buf = Strings.Left(buf, Strings.InStr(buf, "＜") - 1);
            //    }

            //    msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 5);
            //}

            //// 損傷率
            //while (Strings.InStr(msg, "$(損傷率)") > 0)
            //{
            //    idx = Strings.InStr(msg, "$(損傷率)");
            //    msg = Strings.Left(msg, idx - 1) + SrcFormatter.Format(100 * (MaxHP - HP) / MaxHP) + Strings.Mid(msg, idx + 6);
            //}

            //// 相手ユニットを設定
            //if (ReferenceEquals(Commands.SelectedUnit, this))
            //{
            //    t = Commands.SelectedTarget;
            //    tw = Commands.SelectedTWeapon;
            //}
            //else
            //{
            //    t = Commands.SelectedUnit;
            //    tw = Commands.SelectedWeapon;
            //}

            //if (t is object)
            //{
            //    // 相手ユニット名
            //    while (Strings.InStr(msg, "$(相手ユニット)") > 0)
            //    {
            //        buf = t.Nickname;
            //        if (Strings.InStr(buf, "(") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //        }

            //        if (Strings.InStr(buf, "専用") > 0)
            //        {
            //            buf = Strings.Mid(buf, Strings.InStr(buf, "専用") + 2);
            //        }

            //        idx = Strings.InStr(msg, "$(相手ユニット)");
            //        msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 9);
            //    }

            //    while (Strings.InStr(msg, "$(相手機体)") > 0)
            //    {
            //        buf = t.Nickname;
            //        if (Strings.InStr(buf, "(") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //        }

            //        if (Strings.InStr(buf, "専用") > 0)
            //        {
            //            buf = Strings.Mid(buf, Strings.InStr(buf, "専用") + 2);
            //        }

            //        idx = Strings.InStr(msg, "$(相手機体)");
            //        msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 7);
            //    }

            //    // 相手パイロット名
            //    while (Strings.InStr(msg, "$(相手パイロット)") > 0)
            //    {
            //        buf = t.MainPilot().get_Nickname(false);
            //        if (Strings.InStr(buf, "(") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
            //        }

            //        idx = Strings.InStr(msg, "$(相手パイロット)");
            //        msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 10);
            //    }

            //    // 相手武器名
            //    while (Strings.InStr(msg, "$(相手武器)") > 0)
            //    {
            //        if (1 <= tw & tw <= t.CountWeapon())
            //        {
            //            buf = t.WeaponNickname(tw);
            //        }
            //        else
            //        {
            //            buf = "";
            //        }

            //        if (Strings.InStr(buf, "<") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "<") - 1);
            //        }

            //        if (Strings.InStr(buf, "＜") > 0)
            //        {
            //            buf = Strings.Left(buf, Strings.InStr(buf, "＜") - 1);
            //        }

            //        idx = Strings.InStr(msg, "$(相手武器)");
            //        msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 7);
            //    }

            //    // 相手損傷率
            //    while (Strings.InStr(msg, "$(相手損傷率)") > 0)
            //    {
            //        buf = SrcFormatter.Format(100 * (t.MaxHP - t.HP) / t.MaxHP);
            //        idx = Strings.InStr(msg, "$(相手損傷率)");
            //        msg = Strings.Left(msg, idx - 1) + buf + Strings.Mid(msg, idx + 8);
            //    }
            //}

            //// メッセージを表示
            //string argmsg_mode = "";
            //GUI.DisplayBattleMessage(p.ID, msg, msg_mode: argmsg_mode);

            //// カットインは消去しておく
            //string argoname = "戦闘中画面初期化無効";
            //if (!Expression.IsOptionDefined(argoname))
            //{
            //    if (GUI.IsPictureDrawn)
            //    {
            //        GUI.ClearPicture();
            //        // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //        GUI.MainForm.picMain(0).Refresh();
            //    }
            //}
        }

        // シチュエーション main_situation に対応するメッセージが定義されているか
        public bool IsMessageDefined(string main_situation, bool ignore_condition = false)
        {
            return false;
            //bool IsMessageDefinedRet = default;
            //var pnames = new string[5];
            //var msg = default(string);
            //int i;

            //// しゃべれない場合
            //if (!ignore_condition)
            //{
            //    object argIndex1 = "沈黙";
            //    object argIndex2 = "憑依";
            //    object argIndex3 = "石化";
            //    object argIndex4 = "凍結";
            //    object argIndex5 = "麻痺";
            //    object argIndex6 = "睡眠";
            //    if (IsConditionSatisfied(argIndex1) | IsConditionSatisfied(argIndex2) | IsConditionSatisfied(argIndex3) | IsConditionSatisfied(argIndex4) | IsConditionSatisfied(argIndex5) | IsConditionSatisfied(argIndex6))
            //    {
            //        IsMessageDefinedRet = false;
            //        return IsMessageDefinedRet;
            //    }

            //    // 特殊状態用メッセージが定義されているか確認する場合を考慮
            //    object argIndex7 = "恐怖";
            //    if (IsConditionSatisfied(argIndex7) & main_situation != "恐怖")
            //    {
            //        IsMessageDefinedRet = false;
            //        return IsMessageDefinedRet;
            //    }

            //    object argIndex8 = "混乱";
            //    if (IsConditionSatisfied(argIndex8) & main_situation != "混乱")
            //    {
            //        IsMessageDefinedRet = false;
            //        return IsMessageDefinedRet;
            //    }
            //}

            //// SetMessageコマンドでメッセージが設定されているか判定
            //string argvname = "Message(" + MainPilot().ID + "," + main_situation + ")";
            //if (Expression.IsLocalVariableDefined(argvname))
            //{
            //    IsMessageDefinedRet = true;
            //    return IsMessageDefinedRet;
            //}

            //// ダイアログデータを使って判定
            //{
            //    var withBlock = MainPilot();
            //    pnames[1] = withBlock.MessageType;
            //    pnames[2] = withBlock.MessageType;
            //    pnames[3] = withBlock.MessageType;
            //}

            //object argIndex9 = 1;
            //pnames[4] = Pilot(argIndex9).MessageType;
            //var loopTo = CountPilot();
            //for (i = 2; i <= loopTo; i++)
            //{
            //    object argIndex10 = i;
            //    {
            //        var withBlock1 = Pilot(argIndex10);
            //        pnames[1] = pnames[1] + " " + withBlock1.MessageType;
            //        pnames[2] = pnames[2] + " " + withBlock1.MessageType;
            //    }
            //}

            //var loopTo1 = CountSupport();
            //for (i = 1; i <= loopTo1; i++)
            //{
            //    Pilot localSupport() { object argIndex1 = i; var ret = Support(argIndex1); return ret; }

            //    pnames[1] = pnames[1] + " " + localSupport().MessageType;
            //}

            //string argfname = "追加サポート";
            //if (IsFeatureAvailable(argfname))
            //{
            //    pnames[1] = pnames[1] + " " + AdditionalSupport().MessageType;
            //}

            //if (!string.IsNullOrEmpty(main_situation))
            //{
            //    if ((main_situation ?? "") == (Commands.SelectedSpecialPower ?? ""))
            //    {
            //        pnames[3] = Commands.SelectedPilot.MessageType;
            //    }
            //}

            //for (i = 1; i <= 4; i++)
            //{
            //    var tmp1 = pnames;
            //    object argIndex12 = tmp1[i];
            //    if (SRC.DDList.IsDefined(argIndex12))
            //    {
            //        var tmp = pnames;
            //        object argIndex11 = tmp[i];
            //        {
            //            var withBlock2 = SRC.DDList.Item(argIndex11);
            //            var argu = this;
            //            var argu1 = this;
            //            if (withBlock2.SelectDialog(main_situation, argu1, ignore_condition) is object)
            //            {
            //                IsMessageDefinedRet = true;
            //                return IsMessageDefinedRet;
            //            }
            //        }
            //    }
            //}

            //// メッセージデータを使って判定
            //if ((main_situation ?? "") == (Commands.SelectedSpecialPower ?? ""))
            //{
            //    {
            //        var withBlock3 = Commands.SelectedPilot;
            //        bool localIsDefined() { object argIndex1 = withBlock3.MessageType; var ret = SRC.MDList.IsDefined(argIndex1); withBlock3.MessageType = Conversions.ToString(argIndex1); return ret; }

            //        if (localIsDefined())
            //        {
            //            MessageData localItem() { object argIndex1 = withBlock3.MessageType; var ret = SRC.MDList.Item(argIndex1); withBlock3.MessageType = Conversions.ToString(argIndex1); return ret; }

            //            var argu2 = this;
            //            msg = localItem().SelectMessage(main_situation, argu2);
            //        }
            //    }
            //}
            //else
            //{
            //    {
            //        var withBlock4 = MainPilot();
            //        bool localIsDefined1() { object argIndex1 = withBlock4.MessageType; var ret = SRC.MDList.IsDefined(argIndex1); withBlock4.MessageType = Conversions.ToString(argIndex1); return ret; }

            //        if (localIsDefined1())
            //        {
            //            MessageData localItem1() { object argIndex1 = withBlock4.MessageType; var ret = SRC.MDList.Item(argIndex1); withBlock4.MessageType = Conversions.ToString(argIndex1); return ret; }

            //            var argu3 = this;
            //            msg = localItem1().SelectMessage(main_situation, argu3);
            //        }
            //    }

            //    if (Strings.Len(msg) == 0)
            //    {
            //        object argIndex13 = 1;
            //        {
            //            var withBlock5 = Pilot(argIndex13);
            //            bool localIsDefined2() { object argIndex1 = withBlock5.MessageType; var ret = SRC.MDList.IsDefined(argIndex1); withBlock5.MessageType = Conversions.ToString(argIndex1); return ret; }

            //            if (localIsDefined2())
            //            {
            //                MessageData localItem2() { object argIndex1 = withBlock5.MessageType; var ret = SRC.MDList.Item(argIndex1); withBlock5.MessageType = Conversions.ToString(argIndex1); return ret; }

            //                var argu4 = this;
            //                msg = localItem2().SelectMessage(main_situation, argu4);
            //            }
            //        }
            //    }
            //}

            //if (Strings.Len(msg) > 0)
            //{
            //    IsMessageDefinedRet = true;
            //}

            //return IsMessageDefinedRet;
        }

        // 解説メッセージを表示
        public void SysMessage(string main_situation, string sub_situation = "", string add_msg = "")
        {
        //    string uname, msg, uclass;
        //    string[] situations;
        //    string idx, buf;
        //    int i, ret;
        //    string wname;
        //    if (string.IsNullOrEmpty(sub_situation) | (main_situation ?? "") == (sub_situation ?? ""))
        //    {
        //        situations = new string[2];
        //        situations[1] = main_situation + "(解説)";
        //    }
        //    else
        //    {
        //        situations = new string[3];
        //        situations[1] = main_situation + "(" + sub_situation + ")(解説)";
        //        situations[2] = main_situation + "(解説)";
        //    }

        //    // ADD START MARGE
        //    // 拡張戦闘アニメデータで検索
        //    if (SRC.ExtendedAnimation)
        //    {
        //        {
        //            var withBlock = SRC.EADList;
        //            var loopTo = Information.UBound(situations);
        //            for (i = 1; i <= loopTo; i++)
        //            {
        //                // 戦闘アニメ能力で指定された名称で検索
        //                string argfname = "戦闘アニメ";
        //                if (IsFeatureAvailable(argfname))
        //                {
        //                    object argIndex1 = "戦闘アニメ";
        //                    uname = FeatureData(argIndex1);
        //                    object argIndex2 = uname;
        //                    if (withBlock.IsDefined(argIndex2))
        //                    {
        //                        MessageData localItem() { object argIndex1 = uname; var ret = withBlock.Item(argIndex1); return ret; }

        //                        var argu = this;
        //                        msg = localItem().SelectMessage(situations[i], argu);
        //                        if (Strings.Len(msg) > 0)
        //                        {
        //                            goto FoundMessage;
        //                        }
        //                    }
        //                }

        //                // ユニット名称で検索
        //                bool localIsDefined() { object argIndex1 = Name; var ret = withBlock.IsDefined(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

        //                if (localIsDefined())
        //                {
        //                    MessageData localItem1() { object argIndex1 = Name; var ret = withBlock.Item(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

        //                    var argu1 = this;
        //                    msg = localItem1().SelectMessage(situations[i], argu1);
        //                    if (Strings.Len(msg) > 0)
        //                    {
        //                        goto FoundMessage;
        //                    }
        //                }

        //                // ユニット愛称を修正したもので検索
        //                uname = Nickname0;
        //                ret = Strings.InStr(uname, "(");
        //                if (ret > 1)
        //                {
        //                    uname = Strings.Left(uname, ret - 1);
        //                }

        //                ret = Strings.InStr(uname, "用");
        //                if (ret > 0)
        //                {
        //                    if (ret < Strings.Len(uname))
        //                    {
        //                        uname = Strings.Mid(uname, ret + 1);
        //                    }
        //                }

        //                ret = Strings.InStr(uname, "型");
        //                if (ret > 0)
        //                {
        //                    if (ret < Strings.Len(uname))
        //                    {
        //                        uname = Strings.Mid(uname, ret + 1);
        //                    }
        //                }

        //                if (Strings.Right(uname, 4) == "カスタム")
        //                {
        //                    uname = Strings.Left(uname, Strings.Len(uname) - 4);
        //                }

        //                if (Strings.Right(uname, 1) == "改")
        //                {
        //                    uname = Strings.Left(uname, Strings.Len(uname) - 1);
        //                }

        //                object argIndex3 = uname;
        //                if (withBlock.IsDefined(argIndex3))
        //                {
        //                    MessageData localItem2() { object argIndex1 = uname; var ret = withBlock.Item(argIndex1); return ret; }

        //                    var argu2 = this;
        //                    msg = localItem2().SelectMessage(situations[i], argu2);
        //                    if (Strings.Len(msg) > 0)
        //                    {
        //                        goto FoundMessage;
        //                    }
        //                }

        //                // ユニットクラスで検索
        //                uclass = Class0;
        //                object argIndex4 = uclass;
        //                if (withBlock.IsDefined(argIndex4))
        //                {
        //                    MessageData localItem3() { object argIndex1 = uclass; var ret = withBlock.Item(argIndex1); return ret; }

        //                    var argu3 = this;
        //                    msg = localItem3().SelectMessage(situations[i], argu3);
        //                    if (Strings.Len(msg) > 0)
        //                    {
        //                        goto FoundMessage;
        //                    }
        //                }

        //                // 汎用
        //                object argIndex6 = "汎用";
        //                if (withBlock.IsDefined(argIndex6))
        //                {
        //                    object argIndex5 = "汎用";
        //                    var argu4 = this;
        //                    msg = withBlock.Item(argIndex5).SelectMessage(situations[i], argu4);
        //                    if (Strings.Len(msg) > 0)
        //                    {
        //                        goto FoundMessage;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    // ADD END MARGE

        //    // 戦闘アニメデータで検索
        //    {
        //        var withBlock1 = SRC.ADList;
        //        var loopTo1 = Information.UBound(situations);
        //        for (i = 1; i <= loopTo1; i++)
        //        {
        //            // 戦闘アニメ能力で指定された名称で検索
        //            string argfname1 = "戦闘アニメ";
        //            if (IsFeatureAvailable(argfname1))
        //            {
        //                object argIndex7 = "戦闘アニメ";
        //                uname = FeatureData(argIndex7);
        //                object argIndex8 = uname;
        //                if (withBlock1.IsDefined(argIndex8))
        //                {
        //                    MessageData localItem4() { object argIndex1 = uname; var ret = withBlock1.Item(argIndex1); return ret; }

        //                    var argu5 = this;
        //                    msg = localItem4().SelectMessage(situations[i], argu5);
        //                    if (Strings.Len(msg) > 0)
        //                    {
        //                        goto FoundMessage;
        //                    }
        //                }
        //            }

        //            // ユニット名称で検索
        //            bool localIsDefined1() { object argIndex1 = Name; var ret = withBlock1.IsDefined(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

        //            if (localIsDefined1())
        //            {
        //                MessageData localItem5() { object argIndex1 = Name; var ret = withBlock1.Item(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

        //                var argu6 = this;
        //                msg = localItem5().SelectMessage(situations[i], argu6);
        //                if (Strings.Len(msg) > 0)
        //                {
        //                    goto FoundMessage;
        //                }
        //            }

        //            // ユニット愛称を修正したもので検索
        //            uname = Nickname0;
        //            ret = Strings.InStr(uname, "(");
        //            if (ret > 1)
        //            {
        //                uname = Strings.Left(uname, ret - 1);
        //            }

        //            ret = Strings.InStr(uname, "用");
        //            if (ret > 0)
        //            {
        //                if (ret < Strings.Len(uname))
        //                {
        //                    uname = Strings.Mid(uname, ret + 1);
        //                }
        //            }

        //            ret = Strings.InStr(uname, "型");
        //            if (ret > 0)
        //            {
        //                if (ret < Strings.Len(uname))
        //                {
        //                    uname = Strings.Mid(uname, ret + 1);
        //                }
        //            }

        //            if (Strings.Right(uname, 4) == "カスタム")
        //            {
        //                uname = Strings.Left(uname, Strings.Len(uname) - 4);
        //            }

        //            if (Strings.Right(uname, 1) == "改")
        //            {
        //                uname = Strings.Left(uname, Strings.Len(uname) - 1);
        //            }

        //            object argIndex9 = uname;
        //            if (withBlock1.IsDefined(argIndex9))
        //            {
        //                MessageData localItem6() { object argIndex1 = uname; var ret = withBlock1.Item(argIndex1); return ret; }

        //                var argu7 = this;
        //                msg = localItem6().SelectMessage(situations[i], argu7);
        //                if (Strings.Len(msg) > 0)
        //                {
        //                    goto FoundMessage;
        //                }
        //            }

        //            // ユニットクラスで検索
        //            uclass = Class0;
        //            object argIndex10 = uclass;
        //            if (withBlock1.IsDefined(argIndex10))
        //            {
        //                MessageData localItem7() { object argIndex1 = uclass; var ret = withBlock1.Item(argIndex1); return ret; }

        //                var argu8 = this;
        //                msg = localItem7().SelectMessage(situations[i], argu8);
        //                if (Strings.Len(msg) > 0)
        //                {
        //                    goto FoundMessage;
        //                }
        //            }

        //            // 汎用
        //            object argIndex12 = "汎用";
        //            if (withBlock1.IsDefined(argIndex12))
        //            {
        //                object argIndex11 = "汎用";
        //                var argu9 = this;
        //                msg = withBlock1.Item(argIndex11).SelectMessage(situations[i], argu9);
        //                if (Strings.Len(msg) > 0)
        //                {
        //                    goto FoundMessage;
        //                }
        //            }
        //        }
        //    }

        //    // 特殊効果データで検索
        //    {
        //        var withBlock2 = SRC.EDList;
        //        var loopTo2 = Information.UBound(situations);
        //        for (i = 1; i <= loopTo2; i++)
        //        {
        //            // 特殊効果能力で指定された名称で検索
        //            string argfname2 = "特殊効果";
        //            if (IsFeatureAvailable(argfname2))
        //            {
        //                object argIndex13 = "特殊効果";
        //                uname = FeatureData(argIndex13);
        //                object argIndex14 = uname;
        //                if (withBlock2.IsDefined(argIndex14))
        //                {
        //                    MessageData localItem8() { object argIndex1 = uname; var ret = withBlock2.Item(argIndex1); return ret; }

        //                    var argu10 = this;
        //                    msg = localItem8().SelectMessage(situations[i], argu10);
        //                    if (Strings.Len(msg) > 0)
        //                    {
        //                        goto FoundMessage;
        //                    }
        //                }
        //            }

        //            // ユニット名称で検索
        //            bool localIsDefined2() { object argIndex1 = Name; var ret = withBlock2.IsDefined(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

        //            if (localIsDefined2())
        //            {
        //                MessageData localItem9() { object argIndex1 = Name; var ret = withBlock2.Item(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

        //                var argu11 = this;
        //                msg = localItem9().SelectMessage(situations[i], argu11);
        //                if (Strings.Len(msg) > 0)
        //                {
        //                    goto FoundMessage;
        //                }
        //            }

        //            // ユニット愛称を修正したもので検索
        //            uname = Nickname0;
        //            ret = Strings.InStr(uname, "(");
        //            if (ret > 1)
        //            {
        //                uname = Strings.Left(uname, ret - 1);
        //            }

        //            ret = Strings.InStr(uname, "用");
        //            if (ret > 0)
        //            {
        //                if (ret < Strings.Len(uname))
        //                {
        //                    uname = Strings.Mid(uname, ret + 1);
        //                }
        //            }

        //            ret = Strings.InStr(uname, "型");
        //            if (ret > 0)
        //            {
        //                if (ret < Strings.Len(uname))
        //                {
        //                    uname = Strings.Mid(uname, ret + 1);
        //                }
        //            }

        //            if (Strings.Right(uname, 4) == "カスタム")
        //            {
        //                uname = Strings.Left(uname, Strings.Len(uname) - 4);
        //            }

        //            if (Strings.Right(uname, 1) == "改")
        //            {
        //                uname = Strings.Left(uname, Strings.Len(uname) - 1);
        //            }

        //            object argIndex15 = uname;
        //            if (withBlock2.IsDefined(argIndex15))
        //            {
        //                MessageData localItem10() { object argIndex1 = uname; var ret = withBlock2.Item(argIndex1); return ret; }

        //                var argu12 = this;
        //                msg = localItem10().SelectMessage(situations[i], argu12);
        //                if (Strings.Len(msg) > 0)
        //                {
        //                    goto FoundMessage;
        //                }
        //            }

        //            // ユニットクラスで検索
        //            uclass = Class0;
        //            object argIndex16 = uclass;
        //            if (withBlock2.IsDefined(argIndex16))
        //            {
        //                MessageData localItem11() { object argIndex1 = uclass; var ret = withBlock2.Item(argIndex1); return ret; }

        //                var argu13 = this;
        //                msg = localItem11().SelectMessage(situations[i], argu13);
        //                if (Strings.Len(msg) > 0)
        //                {
        //                    goto FoundMessage;
        //                }
        //            }

        //            // 汎用
        //            object argIndex18 = "汎用";
        //            if (withBlock2.IsDefined(argIndex18))
        //            {
        //                object argIndex17 = "汎用";
        //                var argu14 = this;
        //                msg = withBlock2.Item(argIndex17).SelectMessage(situations[i], argu14);
        //                if (Strings.Len(msg) > 0)
        //                {
        //                    goto FoundMessage;
        //                }
        //            }
        //        }
        //    }

        //    // 対応するメッセージが見つからなかった
        //    return;
        //FoundMessage:
        //    ;


        //    // メッセージ表示のキャンセル
        //    if (msg == "-")
        //    {
        //        return;
        //    }

        //    // ユニット名
        //    while (Strings.InStr(msg, "$(ユニット)") > 0)
        //    {
        //        idx = Strings.InStr(msg, "$(ユニット)").ToString();
        //        buf = Nickname;
        //        if (Strings.InStr(buf, "(") > 0)
        //        {
        //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
        //        }

        //        if (Strings.InStr(buf, "専用") > 0)
        //        {
        //            buf = Strings.Left(buf, Strings.InStr(buf, "専用") + 2);
        //        }

        //        msg = Strings.Left(msg, (Conversions.ToDouble(idx) - 1d)) + buf + Strings.Mid(msg, (Conversions.ToDouble(idx) + 7d));
        //    }

        //    while (Strings.InStr(msg, "$(機体)") > 0)
        //    {
        //        idx = Strings.InStr(msg, "$(機体)").ToString();
        //        buf = Nickname;
        //        if (Strings.InStr(buf, "(") > 0)
        //        {
        //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
        //        }

        //        if (Strings.InStr(buf, "専用") > 0)
        //        {
        //            buf = Strings.Left(buf, Strings.InStr(buf, "専用") + 2);
        //        }

        //        msg = Strings.Left(msg, (Conversions.ToDouble(idx) - 1d)) + buf + Strings.Mid(msg, (Conversions.ToDouble(idx) + 5d));
        //    }

        //    // パイロット名
        //    while (Strings.InStr(msg, "$(パイロット)") > 0)
        //    {
        //        idx = Strings.InStr(msg, "$(パイロット)").ToString();
        //        buf = MainPilot().get_Nickname(false);
        //        if (Strings.InStr(buf, "(") > 0)
        //        {
        //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
        //        }

        //        msg = Strings.Left(msg, (Conversions.ToDouble(idx) - 1d)) + buf + Strings.Mid(msg, (Conversions.ToDouble(idx) + 8d));
        //    }

        //    // 武器名
        //    if (Strings.InStr(msg, "$(武器)") > 0)
        //    {
        //        if (ReferenceEquals(Commands.SelectedUnit, this))
        //        {
        //            wname = Weapon(Commands.SelectedWeapon).Name;
        //        }
        //        else
        //        {
        //            wname = Weapon(Commands.SelectedTWeapon).Name;
        //        }

        //        if (Strings.InStr(wname, "(") > 0)
        //        {
        //            wname = Strings.Left(wname, Strings.InStr(wname, "(") - 1);
        //        }

        //        if (Strings.InStr(wname, "<") > 0)
        //        {
        //            wname = Strings.Left(wname, Strings.InStr(wname, "<") - 1);
        //        }

        //        while (Strings.InStr(msg, "$(武器)") > 0)
        //        {
        //            idx = Strings.InStr(msg, "$(武器)").ToString();
        //            msg = Strings.Left(msg, (Conversions.ToDouble(idx) - 1d)) + wname + Strings.Mid(msg, (Conversions.ToDouble(idx) + 5d));
        //        }
        //    }

        //    // 損傷率
        //    while (Strings.InStr(msg, "$(損傷率)") > 0)
        //    {
        //        idx = Strings.InStr(msg, "$(損傷率)").ToString();
        //        msg = Strings.Left(msg, (Conversions.ToDouble(idx) - 1d)) + SrcFormatter.Format(100 * (MaxHP - HP) / MaxHP) + Strings.Mid(msg, (Conversions.ToDouble(idx) + 6d));
        //    }

        //    // 相手ユニット名
        //    while (Strings.InStr(msg, "$(相手ユニット)") > 0)
        //    {
        //        if (ReferenceEquals(Commands.SelectedUnit, this))
        //        {
        //            if (Commands.SelectedTarget is object)
        //            {
        //                buf = Commands.SelectedTarget.Nickname;
        //            }
        //            else
        //            {
        //                buf = "";
        //            }
        //        }
        //        else
        //        {
        //            buf = Commands.SelectedUnit.Nickname;
        //        }

        //        if (Strings.InStr(buf, "(") > 0)
        //        {
        //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
        //        }

        //        if (Strings.InStr(buf, "専用") > 0)
        //        {
        //            buf = Strings.Left(buf, Strings.InStr(buf, "専用") + 2);
        //        }

        //        idx = Strings.InStr(msg, "$(相手ユニット)").ToString();
        //        msg = Strings.Left(msg, (Conversions.ToDouble(idx) - 1d)) + buf + Strings.Mid(msg, (Conversions.ToDouble(idx) + 9d));
        //    }

        //    while (Strings.InStr(msg, "$(相手機体)") > 0)
        //    {
        //        if (ReferenceEquals(Commands.SelectedUnit, this))
        //        {
        //            if (Commands.SelectedTarget is object)
        //            {
        //                buf = Commands.SelectedTarget.Nickname;
        //            }
        //            else
        //            {
        //                buf = "";
        //            }
        //        }
        //        else
        //        {
        //            buf = Commands.SelectedUnit.Nickname;
        //        }

        //        if (Strings.InStr(buf, "(") > 0)
        //        {
        //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
        //        }

        //        if (Strings.InStr(buf, "専用") > 0)
        //        {
        //            buf = Strings.Left(buf, Strings.InStr(buf, "専用") + 2);
        //        }

        //        idx = Strings.InStr(msg, "$(相手機体)").ToString();
        //        msg = Strings.Left(msg, (Conversions.ToDouble(idx) - 1d)) + buf + Strings.Mid(msg, (Conversions.ToDouble(idx) + 7d));
        //    }

        //    // 相手パイロット名
        //    while (Strings.InStr(msg, "$(相手パイロット)") > 0)
        //    {
        //        if (ReferenceEquals(Commands.SelectedUnit, this))
        //        {
        //            if (Commands.SelectedTarget is object)
        //            {
        //                buf = Commands.SelectedTarget.MainPilot().get_Nickname(false);
        //            }
        //            else
        //            {
        //                buf = "";
        //            }
        //        }
        //        else
        //        {
        //            buf = Commands.SelectedUnit.MainPilot().get_Nickname(false);
        //        }

        //        if (Strings.InStr(buf, "(") > 0)
        //        {
        //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
        //        }

        //        idx = Strings.InStr(msg, "$(相手パイロット)").ToString();
        //        msg = Strings.Left(msg, (Conversions.ToDouble(idx) - 1d)) + buf + Strings.Mid(msg, (Conversions.ToDouble(idx) + 10d));
        //    }

        //    // 相手武器名
        //    while (Strings.InStr(msg, "$(相手武器)") > 0)
        //    {
        //        if (ReferenceEquals(Commands.SelectedUnit, this))
        //        {
        //            if (Commands.SelectedTarget is object)
        //            {
        //                buf = Commands.SelectedTarget.Weapon(Commands.SelectedTWeapon).Name;
        //            }
        //            else
        //            {
        //                buf = "";
        //            }
        //        }
        //        else
        //        {
        //            buf = Commands.SelectedUnit.Weapon(Commands.SelectedWeapon).Name;
        //        }

        //        if (Strings.InStr(buf, "(") > 0)
        //        {
        //            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
        //        }

        //        if (Strings.InStr(buf, "<") > 0)
        //        {
        //            buf = Strings.Left(buf, Strings.InStr(buf, "<") - 1);
        //        }

        //        if (Strings.InStr(buf, "＜") > 0)
        //        {
        //            buf = Strings.Left(buf, Strings.InStr(buf, "＜") - 1);
        //        }

        //        idx = Strings.InStr(msg, "$(相手武器)").ToString();
        //        msg = Strings.Left(msg, (Conversions.ToDouble(idx) - 1d)) + buf + Strings.Mid(msg, (Conversions.ToDouble(idx) + 7d));
        //    }

        //    // 相手損傷率
        //    while (Strings.InStr(msg, "$(相手損傷率)") > 0)
        //    {
        //        idx = Strings.InStr(msg, "$(相手損傷率)").ToString();
        //        if (ReferenceEquals(Commands.SelectedUnit, this))
        //        {
        //            if (Commands.SelectedTarget is object)
        //            {
        //                {
        //                    var withBlock3 = Commands.SelectedTarget;
        //                    buf = SrcFormatter.Format(100 * (withBlock3.MaxHP - withBlock3.HP) / withBlock3.MaxHP);
        //                }
        //            }
        //            else
        //            {
        //                buf = "";
        //            }
        //        }
        //        else
        //        {
        //            {
        //                var withBlock4 = Commands.SelectedUnit;
        //                buf = SrcFormatter.Format(100 * (withBlock4.MaxHP - withBlock4.HP) / withBlock4.MaxHP);
        //            }
        //        }

        //        msg = Strings.Left(msg, (Conversions.ToDouble(idx) - 1d)) + buf + Strings.Mid(msg, (Conversions.ToDouble(idx) + 8d));
        //    }

        //    if (!My.MyProject.Forms.frmMessage.Visible)
        //    {
        //        Unit argu15 = null;
        //        Unit argu21 = null;
        //        GUI.OpenMessageForm(u1: argu15, u2: argu21);
        //    }

        //    if (!string.IsNullOrEmpty(add_msg))
        //    {
        //        string argpname = "-";
        //        string argmsg_mode = "";
        //        GUI.DisplayBattleMessage(argpname, msg + "." + add_msg, msg_mode: argmsg_mode);
        //    }
        //    else
        //    {
        //        string argpname1 = "-";
        //        string argmsg_mode1 = "";
        //        GUI.DisplayBattleMessage(argpname1, msg, msg_mode: argmsg_mode1);
        //    }
        }

        // 解説メッセージが定義されているか？
        public bool IsSysMessageDefined(string main_situation, string sub_situation = "")
        {
            return false;
            //bool IsSysMessageDefinedRet = default;
            //string uclass, uname, msg;
            //string[] situations;
            //int i, ret;
            //if (string.IsNullOrEmpty(sub_situation) | (main_situation ?? "") == (sub_situation ?? ""))
            //{
            //    situations = new string[2];
            //    situations[1] = main_situation + "(解説)";
            //}
            //else
            //{
            //    situations = new string[3];
            //    situations[1] = main_situation + "(" + sub_situation + ")(解説)";
            //    situations[2] = main_situation + "(解説)";
            //}

            //// ADD START MARGE
            //// 拡張戦闘アニメデータで検索
            //if (SRC.ExtendedAnimation)
            //{
            //    {
            //        var withBlock = SRC.EADList;
            //        var loopTo = Information.UBound(situations);
            //        for (i = 1; i <= loopTo; i++)
            //        {
            //            // 戦闘アニメ能力で指定された名称で検索
            //            string argfname = "戦闘アニメ";
            //            if (IsFeatureAvailable(argfname))
            //            {
            //                object argIndex1 = "戦闘アニメ";
            //                uname = FeatureData(argIndex1);
            //                object argIndex2 = uname;
            //                if (withBlock.IsDefined(argIndex2))
            //                {
            //                    MessageData localItem() { object argIndex1 = uname; var ret = withBlock.Item(argIndex1); return ret; }

            //                    var argu = this;
            //                    msg = localItem().SelectMessage(situations[i], argu);
            //                    if (Strings.Len(msg) > 0)
            //                    {
            //                        IsSysMessageDefinedRet = true;
            //                        return IsSysMessageDefinedRet;
            //                    }
            //                }
            //            }

            //            // ユニット名称で検索
            //            bool localIsDefined() { object argIndex1 = Name; var ret = withBlock.IsDefined(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

            //            if (localIsDefined())
            //            {
            //                MessageData localItem1() { object argIndex1 = Name; var ret = withBlock.Item(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

            //                var argu1 = this;
            //                msg = localItem1().SelectMessage(situations[i], argu1);
            //                if (Strings.Len(msg) > 0)
            //                {
            //                    IsSysMessageDefinedRet = true;
            //                    return IsSysMessageDefinedRet;
            //                }
            //            }

            //            // ユニット愛称を修正したもので検索
            //            uname = Nickname0;
            //            ret = Strings.InStr(uname, "(");
            //            if (ret > 1)
            //            {
            //                uname = Strings.Left(uname, ret - 1);
            //            }

            //            ret = Strings.InStr(uname, "用");
            //            if (ret > 0)
            //            {
            //                if (ret < Strings.Len(uname))
            //                {
            //                    uname = Strings.Mid(uname, ret + 1);
            //                }
            //            }

            //            ret = Strings.InStr(uname, "型");
            //            if (ret > 0)
            //            {
            //                if (ret < Strings.Len(uname))
            //                {
            //                    uname = Strings.Mid(uname, ret + 1);
            //                }
            //            }

            //            if (Strings.Right(uname, 4) == "カスタム")
            //            {
            //                uname = Strings.Left(uname, Strings.Len(uname) - 4);
            //            }

            //            if (Strings.Right(uname, 1) == "改")
            //            {
            //                uname = Strings.Left(uname, Strings.Len(uname) - 1);
            //            }

            //            object argIndex3 = uname;
            //            if (withBlock.IsDefined(argIndex3))
            //            {
            //                MessageData localItem2() { object argIndex1 = uname; var ret = withBlock.Item(argIndex1); return ret; }

            //                var argu2 = this;
            //                msg = localItem2().SelectMessage(situations[i], argu2);
            //                if (Strings.Len(msg) > 0)
            //                {
            //                    IsSysMessageDefinedRet = true;
            //                    return IsSysMessageDefinedRet;
            //                }
            //            }

            //            // ユニットクラスで検索
            //            uclass = Class0;
            //            object argIndex4 = uclass;
            //            if (withBlock.IsDefined(argIndex4))
            //            {
            //                MessageData localItem3() { object argIndex1 = uclass; var ret = withBlock.Item(argIndex1); return ret; }

            //                var argu3 = this;
            //                msg = localItem3().SelectMessage(situations[i], argu3);
            //                if (Strings.Len(msg) > 0)
            //                {
            //                    IsSysMessageDefinedRet = true;
            //                    return IsSysMessageDefinedRet;
            //                }
            //            }

            //            // 汎用
            //            object argIndex6 = "汎用";
            //            if (withBlock.IsDefined(argIndex6))
            //            {
            //                object argIndex5 = "汎用";
            //                var argu4 = this;
            //                msg = withBlock.Item(argIndex5).SelectMessage(situations[i], argu4);
            //                if (Strings.Len(msg) > 0)
            //                {
            //                    IsSysMessageDefinedRet = true;
            //                    return IsSysMessageDefinedRet;
            //                }
            //            }
            //        }
            //    }
            //}
            //// ADD END MARGE

            //// 戦闘アニメデータで検索
            //{
            //    var withBlock1 = SRC.ADList;
            //    var loopTo1 = Information.UBound(situations);
            //    for (i = 1; i <= loopTo1; i++)
            //    {
            //        // 戦闘アニメ能力で指定された名称で検索
            //        string argfname1 = "戦闘アニメ";
            //        if (IsFeatureAvailable(argfname1))
            //        {
            //            object argIndex7 = "戦闘アニメ";
            //            uname = FeatureData(argIndex7);
            //            object argIndex8 = uname;
            //            if (withBlock1.IsDefined(argIndex8))
            //            {
            //                MessageData localItem4() { object argIndex1 = uname; var ret = withBlock1.Item(argIndex1); return ret; }

            //                var argu5 = this;
            //                msg = localItem4().SelectMessage(situations[i], argu5);
            //                if (Strings.Len(msg) > 0)
            //                {
            //                    IsSysMessageDefinedRet = true;
            //                    return IsSysMessageDefinedRet;
            //                }
            //            }
            //        }

            //        // ユニット名称で検索
            //        bool localIsDefined1() { object argIndex1 = Name; var ret = withBlock1.IsDefined(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

            //        if (localIsDefined1())
            //        {
            //            MessageData localItem5() { object argIndex1 = Name; var ret = withBlock1.Item(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

            //            var argu6 = this;
            //            msg = localItem5().SelectMessage(situations[i], argu6);
            //            if (Strings.Len(msg) > 0)
            //            {
            //                IsSysMessageDefinedRet = true;
            //                return IsSysMessageDefinedRet;
            //            }
            //        }

            //        // ユニット愛称を修正したもので検索
            //        uname = Nickname0;
            //        ret = Strings.InStr(uname, "(");
            //        if (ret > 1)
            //        {
            //            uname = Strings.Left(uname, ret - 1);
            //        }

            //        ret = Strings.InStr(uname, "用");
            //        if (ret > 0)
            //        {
            //            if (ret < Strings.Len(uname))
            //            {
            //                uname = Strings.Mid(uname, ret + 1);
            //            }
            //        }

            //        ret = Strings.InStr(uname, "型");
            //        if (ret > 0)
            //        {
            //            if (ret < Strings.Len(uname))
            //            {
            //                uname = Strings.Mid(uname, ret + 1);
            //            }
            //        }

            //        if (Strings.Right(uname, 4) == "カスタム")
            //        {
            //            uname = Strings.Left(uname, Strings.Len(uname) - 4);
            //        }

            //        if (Strings.Right(uname, 1) == "改")
            //        {
            //            uname = Strings.Left(uname, Strings.Len(uname) - 1);
            //        }

            //        object argIndex9 = uname;
            //        if (withBlock1.IsDefined(argIndex9))
            //        {
            //            MessageData localItem6() { object argIndex1 = uname; var ret = withBlock1.Item(argIndex1); return ret; }

            //            var argu7 = this;
            //            msg = localItem6().SelectMessage(situations[i], argu7);
            //            if (Strings.Len(msg) > 0)
            //            {
            //                IsSysMessageDefinedRet = true;
            //                return IsSysMessageDefinedRet;
            //            }
            //        }

            //        // ユニットクラスで検索
            //        uclass = Class0;
            //        object argIndex10 = uclass;
            //        if (withBlock1.IsDefined(argIndex10))
            //        {
            //            MessageData localItem7() { object argIndex1 = uclass; var ret = withBlock1.Item(argIndex1); return ret; }

            //            var argu8 = this;
            //            msg = localItem7().SelectMessage(situations[i], argu8);
            //            if (Strings.Len(msg) > 0)
            //            {
            //                IsSysMessageDefinedRet = true;
            //                return IsSysMessageDefinedRet;
            //            }
            //        }

            //        // 汎用
            //        object argIndex12 = "汎用";
            //        if (withBlock1.IsDefined(argIndex12))
            //        {
            //            object argIndex11 = "汎用";
            //            var argu9 = this;
            //            msg = withBlock1.Item(argIndex11).SelectMessage(situations[i], argu9);
            //            if (Strings.Len(msg) > 0)
            //            {
            //                IsSysMessageDefinedRet = true;
            //                return IsSysMessageDefinedRet;
            //            }
            //        }
            //    }
            //}

            //// 特殊効果データで検索
            //{
            //    var withBlock2 = SRC.EDList;
            //    var loopTo2 = Information.UBound(situations);
            //    for (i = 1; i <= loopTo2; i++)
            //    {
            //        // 特殊効果能力で指定された名称で検索
            //        string argfname2 = "特殊効果";
            //        if (IsFeatureAvailable(argfname2))
            //        {
            //            object argIndex13 = "特殊効果";
            //            uname = FeatureData(argIndex13);
            //            object argIndex14 = uname;
            //            if (withBlock2.IsDefined(argIndex14))
            //            {
            //                MessageData localItem8() { object argIndex1 = uname; var ret = withBlock2.Item(argIndex1); return ret; }

            //                var argu10 = this;
            //                msg = localItem8().SelectMessage(situations[i], argu10);
            //                if (Strings.Len(msg) > 0)
            //                {
            //                    IsSysMessageDefinedRet = true;
            //                    return IsSysMessageDefinedRet;
            //                }
            //            }
            //        }

            //        // ユニット名称で検索
            //        bool localIsDefined2() { object argIndex1 = Name; var ret = withBlock2.IsDefined(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

            //        if (localIsDefined2())
            //        {
            //            MessageData localItem9() { object argIndex1 = Name; var ret = withBlock2.Item(argIndex1); Name = Conversions.ToString(argIndex1); return ret; }

            //            var argu11 = this;
            //            msg = localItem9().SelectMessage(situations[i], argu11);
            //            if (Strings.Len(msg) > 0)
            //            {
            //                IsSysMessageDefinedRet = true;
            //                return IsSysMessageDefinedRet;
            //            }
            //        }

            //        // ユニット愛称を修正したもので検索
            //        uname = Nickname0;
            //        ret = Strings.InStr(uname, "(");
            //        if (ret > 1)
            //        {
            //            uname = Strings.Left(uname, ret - 1);
            //        }

            //        ret = Strings.InStr(uname, "用");
            //        if (ret > 0)
            //        {
            //            if (ret < Strings.Len(uname))
            //            {
            //                uname = Strings.Mid(uname, ret + 1);
            //            }
            //        }

            //        ret = Strings.InStr(uname, "型");
            //        if (ret > 0)
            //        {
            //            if (ret < Strings.Len(uname))
            //            {
            //                uname = Strings.Mid(uname, ret + 1);
            //            }
            //        }

            //        if (Strings.Right(uname, 4) == "カスタム")
            //        {
            //            uname = Strings.Left(uname, Strings.Len(uname) - 4);
            //        }

            //        if (Strings.Right(uname, 1) == "改")
            //        {
            //            uname = Strings.Left(uname, Strings.Len(uname) - 1);
            //        }

            //        object argIndex15 = uname;
            //        if (withBlock2.IsDefined(argIndex15))
            //        {
            //            MessageData localItem10() { object argIndex1 = uname; var ret = withBlock2.Item(argIndex1); return ret; }

            //            var argu12 = this;
            //            msg = localItem10().SelectMessage(situations[i], argu12);
            //            if (Strings.Len(msg) > 0)
            //            {
            //                IsSysMessageDefinedRet = true;
            //                return IsSysMessageDefinedRet;
            //            }
            //        }

            //        // ユニットクラスで検索
            //        uclass = Class0;
            //        object argIndex16 = uclass;
            //        if (withBlock2.IsDefined(argIndex16))
            //        {
            //            MessageData localItem11() { object argIndex1 = uclass; var ret = withBlock2.Item(argIndex1); return ret; }

            //            var argu13 = this;
            //            msg = localItem11().SelectMessage(situations[i], argu13);
            //            if (Strings.Len(msg) > 0)
            //            {
            //                IsSysMessageDefinedRet = true;
            //                return IsSysMessageDefinedRet;
            //            }
            //        }

            //        // 汎用
            //        object argIndex18 = "汎用";
            //        if (withBlock2.IsDefined(argIndex18))
            //        {
            //            object argIndex17 = "汎用";
            //            var argu14 = this;
            //            msg = withBlock2.Item(argIndex17).SelectMessage(situations[i], argu14);
            //            if (Strings.Len(msg) > 0)
            //            {
            //                IsSysMessageDefinedRet = true;
            //                return IsSysMessageDefinedRet;
            //            }
            //        }
            //    }
            //}

            //IsSysMessageDefinedRet = false;
            //return IsSysMessageDefinedRet;
        }
    }
}
