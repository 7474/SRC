// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.VB;
using System;

namespace SRCCore.Commands
{
    public partial class Command
    {
        // スペシャルパワーコマンドを開始
        private void StartSpecialPowerCommand()
        {
            throw new NotImplementedException();
            //int n, i, j, ret;
            //string[] pname_list;
            //string[] pid_list;
            //string[] sp_list;
            //string[] list;
            //string[] id_list;
            //string sname;
            //SpecialPowerData sd;
            //Unit u;
            //Pilot p;
            //var strkey_list = default(string[]);
            //int max_item;
            //string max_str;
            //string buf;
            //bool found;
            //GUI.LockGUI();
            //SelectedCommand = "スペシャルパワー";
            //{
            //    var withBlock = SelectedUnit;
            //    pname_list = new string[1];
            //    pid_list = new string[1];
            //    GUI.ListItemFlag = new bool[1];

            //    // スペシャルパワーを使用可能なパイロットの一覧を作成
            //    n = (withBlock.CountPilot() + withBlock.CountSupport());
            //    string argfname = "追加サポート";
            //    if (withBlock.IsFeatureAvailable(argfname))
            //    {
            //        n = (n + 1);
            //    }

            //    var loopTo = n;
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        if (i <= withBlock.CountPilot())
            //        {
            //            // メインパイロット＆サブパイロット
            //            object argIndex1 = i;
            //            p = withBlock.Pilot(argIndex1);
            //            if (i == 1)
            //            {
            //                // １番目のパイロットの場合はメインパイロットを使用
            //                p = withBlock.MainPilot();

            //                // ただし２人乗り以上のユニットで、メインパイロットが
            //                // スペシャルパワーを持たない場合はそのまま１番目のパイロットを使用
            //                if (withBlock.CountPilot() > 1)
            //                {
            //                    object argIndex3 = 1;
            //                    if (p.Data.SP <= 0 & withBlock.Pilot(argIndex3).Data.SP > 0)
            //                    {
            //                        object argIndex2 = 1;
            //                        p = withBlock.Pilot(argIndex2);
            //                    }
            //                }

            //                // サブパイロットがメインパイロットを勤めている場合も
            //                // １番目のパイロットを使用
            //                var loopTo1 = withBlock.CountPilot();
            //                for (j = 2; j <= loopTo1; j++)
            //                {
            //                    Pilot localPilot() { object argIndex1 = j; var ret = withBlock.Pilot(argIndex1); return ret; }

            //                    if (ReferenceEquals(p, localPilot()))
            //                    {
            //                        object argIndex4 = 1;
            //                        p = withBlock.Pilot(argIndex4);
            //                        break;
            //                    }
            //                }
            //            }
            //            else
            //            {
            //            }

            //            if (p.CountSpecialPower == 0)
            //            {
            //                goto NextPilot;
            //            }

            //            Array.Resize(pname_list, Information.UBound(pname_list) + 1 + 1);
            //            Array.Resize(pid_list, Information.UBound(pname_list) + 1 + 1);
            //            Array.Resize(GUI.ListItemFlag, Information.UBound(pname_list) + 1);
            //            GUI.ListItemFlag[Information.UBound(pname_list)] = false;
            //            pid_list[Information.UBound(pname_list)] = p.ID;
            //            string localRightPaddedString() { string argbuf = p.get_Nickname(false); var ret = GeneralLib.RightPaddedString(argbuf, 17); p.get_Nickname(false) = argbuf; return ret; }

            //            string localRightPaddedString1() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.SP) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.MaxSP); var ret = GeneralLib.RightPaddedString(argbuf, 8); return ret; }

            //            pname_list[Information.UBound(pname_list)] = localRightPaddedString() + localRightPaddedString1();
            //            var loopTo2 = p.CountSpecialPower;
            //            for (j = 1; j <= loopTo2; j++)
            //            {
            //                sname = p.get_SpecialPower(j);
            //                if (p.SP >= p.SpecialPowerCost(sname))
            //                {
            //                    SpecialPowerData localItem() { object argIndex1 = sname; var ret = SRC.SPDList.Item(argIndex1); return ret; }

            //                    pname_list[Information.UBound(pname_list)] = pname_list[Information.UBound(pname_list)] + localItem().intName;
            //                }
            //            }
            //        }
            //        else if (i <= (withBlock.CountPilot() + withBlock.CountSupport()))
            //        {
            //            // サポートパイロット
            //            object argIndex5 = i - withBlock.CountPilot();
            //            {
            //                var withBlock2 = withBlock.Support(argIndex5);
            //                if (withBlock2.CountSpecialPower == 0)
            //                {
            //                    goto NextPilot;
            //                }

            //                Array.Resize(pname_list, Information.UBound(pname_list) + 1 + 1);
            //                Array.Resize(pid_list, Information.UBound(pname_list) + 1 + 1);
            //                Array.Resize(GUI.ListItemFlag, Information.UBound(pname_list) + 1);
            //                GUI.ListItemFlag[Information.UBound(pname_list)] = false;
            //                pid_list[Information.UBound(pname_list)] = withBlock2.ID;
            //                string localRightPaddedString4() { string argbuf = withBlock2.get_Nickname(false); var ret = GeneralLib.RightPaddedString(argbuf, 17); withBlock2.get_Nickname(false) = argbuf; return ret; }

            //                string localRightPaddedString5() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock2.SP) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock2.MaxSP); var ret = GeneralLib.RightPaddedString(argbuf, 8); return ret; }

            //                pname_list[Information.UBound(pname_list)] = localRightPaddedString4() + localRightPaddedString5();
            //                var loopTo4 = withBlock2.CountSpecialPower;
            //                for (j = 1; j <= loopTo4; j++)
            //                {
            //                    sname = withBlock2.get_SpecialPower(j);
            //                    if (withBlock2.SP >= withBlock2.SpecialPowerCost(sname))
            //                    {
            //                        SpecialPowerData localItem2() { object argIndex1 = sname; var ret = SRC.SPDList.Item(argIndex1); return ret; }

            //                        pname_list[Information.UBound(pname_list)] = pname_list[Information.UBound(pname_list)] + localItem2().intName;
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            // 追加サポートパイロット
            //            {
            //                var withBlock1 = withBlock.AdditionalSupport();
            //                if (withBlock1.CountSpecialPower == 0)
            //                {
            //                    goto NextPilot;
            //                }

            //                Array.Resize(pname_list, Information.UBound(pname_list) + 1 + 1);
            //                Array.Resize(pid_list, Information.UBound(pname_list) + 1 + 1);
            //                Array.Resize(GUI.ListItemFlag, Information.UBound(pname_list) + 1);
            //                GUI.ListItemFlag[Information.UBound(pname_list)] = false;
            //                pid_list[Information.UBound(pname_list)] = withBlock1.ID;
            //                string localRightPaddedString2() { string argbuf = withBlock1.get_Nickname(false); var ret = GeneralLib.RightPaddedString(argbuf, 17); withBlock1.get_Nickname(false) = argbuf; return ret; }

            //                string localRightPaddedString3() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.SP) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MaxSP); var ret = GeneralLib.RightPaddedString(argbuf, 8); return ret; }

            //                pname_list[Information.UBound(pname_list)] = localRightPaddedString2() + localRightPaddedString3();
            //                var loopTo3 = withBlock1.CountSpecialPower;
            //                for (j = 1; j <= loopTo3; j++)
            //                {
            //                    sname = withBlock1.get_SpecialPower(j);
            //                    if (withBlock1.SP >= withBlock1.SpecialPowerCost(sname))
            //                    {
            //                        SpecialPowerData localItem1() { object argIndex1 = sname; var ret = SRC.SPDList.Item(argIndex1); return ret; }

            //                        pname_list[Information.UBound(pname_list)] = pname_list[Information.UBound(pname_list)] + localItem1().intName;
            //                    }
            //                }
            //            }
            //        }

            //    NextPilot:
            //        ;
            //    }

            //    GUI.TopItem = 1;
            //    if (Information.UBound(pname_list) > 1)
            //    {
            //        // どのパイロットを使うか選択
            //        string argoname = "等身大基準";
            //        if (Expression.IsOptionDefined(argoname))
            //        {
            //            string arglb_caption = "キャラクター選択";
            //            string argtname = "SP";
            //            string argtname1 = "SP";
            //            string arglb_info = "キャラクター     " + Expression.Term(argtname, SelectedUnit, 2) + "/Max" + Expression.Term(argtname1, SelectedUnit, 2);
            //            string arglb_mode = "連続表示,カーソル移動";
            //            i = GUI.ListBox(arglb_caption, pname_list, arglb_info, arglb_mode);
            //        }
            //        else
            //        {
            //            string arglb_caption1 = "パイロット選択";
            //            string argtname2 = "SP";
            //            string argtname3 = "SP";
            //            string arglb_info1 = "パイロット       " + Expression.Term(argtname2, SelectedUnit, 2) + "/Max" + Expression.Term(argtname3, SelectedUnit, 2);
            //            string arglb_mode1 = "連続表示,カーソル移動";
            //            i = GUI.ListBox(arglb_caption1, pname_list, arglb_info1, arglb_mode1);
            //        }
            //    }
            //    else
            //    {
            //        // 一人しかいないので選択の必要なし
            //        i = 1;
            //    }

            //    // 誰もスペシャルパワーを使えなければキャンセル
            //    if (i == 0)
            //    {
            //        My.MyProject.Forms.frmListBox.Hide();
            //        if (SRC.AutoMoveCursor)
            //        {
            //            GUI.RestoreCursorPos();
            //        }

            //        GUI.UnlockGUI();
            //        CancelCommand();
            //        return;
            //    }

            //    // スペシャルパワーを使うパイロットを設定
            //    var tmp = pid_list;
            //    object argIndex6 = tmp[i];
            //    SelectedPilot = SRC.PList.Item(argIndex6);
            //    // そのパイロットのステータスを表示
            //    if (Information.UBound(pname_list) > 1)
            //    {
            //        Status.DisplayPilotStatus(SelectedPilot);
            //    }
            //}

            //{
            //    var withBlock3 = SelectedPilot;
            //    // 使用可能なスペシャルパワーの一覧を作成
            //    sp_list = new string[(withBlock3.CountSpecialPower + 1)];
            //    GUI.ListItemFlag = new bool[(withBlock3.CountSpecialPower + 1)];
            //    var loopTo5 = withBlock3.CountSpecialPower;
            //    for (i = 1; i <= loopTo5; i++)
            //    {
            //        sname = withBlock3.get_SpecialPower(i);
            //        string localLeftPaddedString() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock3.SpecialPowerCost(sname)); var ret = GeneralLib.LeftPaddedString(argbuf, 3); return ret; }

            //        SpecialPowerData localItem3() { object argIndex1 = sname; var ret = SRC.SPDList.Item(argIndex1); return ret; }

            //        sp_list[i] = GeneralLib.RightPaddedString(sname, 13) + localLeftPaddedString() + "  " + localItem3().Comment;
            //        if (withBlock3.SP >= withBlock3.SpecialPowerCost(sname))
            //        {
            //            if (!withBlock3.IsSpecialPowerUseful(sname))
            //            {
            //                GUI.ListItemFlag[i] = true;
            //            }
            //        }
            //        else
            //        {
            //            GUI.ListItemFlag[i] = true;
            //        }
            //    }
            //}

            //// どのコマンドを使用するかを選択
            //{
            //    var withBlock4 = SelectedPilot;
            //    GUI.TopItem = 1;
            //    string argtname4 = "スペシャルパワー";
            //    string arglb_caption2 = Expression.Term(argtname4, SelectedUnit) + "選択";
            //    string argtname5 = "SP";
            //    string argtname6 = "SP";
            //    string arglb_info2 = "名称         消費" + Expression.Term(argtname5, SelectedUnit) + "（" + withBlock4.get_Nickname(false) + " " + Expression.Term(argtname6, SelectedUnit) + "=" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.SP) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.MaxSP) + "）";
            //    string arglb_mode2 = "カーソル移動(行きのみ)";
            //    i = GUI.ListBox(arglb_caption2, sp_list, arglb_info2, arglb_mode2);
            //}

            //// キャンセル
            //if (i == 0)
            //{
            //    Status.DisplayUnitStatus(SelectedUnit);
            //    // カーソル自動移動
            //    if (SRC.AutoMoveCursor)
            //    {
            //        string argcursor_mode = "ユニット選択";
            //        GUI.MoveCursorPos(argcursor_mode, SelectedUnit);
            //    }

            //    GUI.UnlockGUI();
            //    CancelCommand();
            //    return;
            //}

            //// 使用するスペシャルパワーを設定
            //SelectedSpecialPower = SelectedPilot.get_SpecialPower(i);

            //// 味方スペシャルパワー実行の効果により他のパイロットが持っているスペシャルパワーを
            //// 使う場合は記録しておき、後で消費ＳＰを倍にする必要がある
            //SpecialPowerData localItem5() { object argIndex1 = SelectedSpecialPower; var ret = SRC.SPDList.Item(argIndex1); return ret; }

            //if (localItem5().EffectType(1) == "味方スペシャルパワー実行")
            //{
            //    // スペシャルパワー一覧
            //    list = new string[1];
            //    var loopTo6 = SRC.SPDList.Count();
            //    for (i = 1; i <= loopTo6; i++)
            //    {
            //        object argIndex7 = i;
            //        {
            //            var withBlock5 = SRC.SPDList.Item(argIndex7);
            //            if (withBlock5.EffectType(1) != "味方スペシャルパワー実行" & withBlock5.intName != "非表示")
            //            {
            //                Array.Resize(list, Information.UBound(list) + 1 + 1);
            //                Array.Resize(strkey_list, Information.UBound(list) + 1);
            //                list[Information.UBound(list)] = withBlock5.Name;
            //                strkey_list[Information.UBound(list)] = withBlock5.KanaName;
            //            }
            //        }
            //    }

            //    GUI.ListItemFlag = new bool[Information.UBound(list) + 1];

            //    // ソート
            //    var loopTo7 = (Information.UBound(strkey_list) - 1);
            //    for (i = 1; i <= loopTo7; i++)
            //    {
            //        max_item = i;
            //        max_str = strkey_list[i];
            //        var loopTo8 = Information.UBound(strkey_list);
            //        for (j = (i + 1); j <= loopTo8; j++)
            //        {
            //            if (Strings.StrComp(strkey_list[j], max_str, (CompareMethod)1) == -1)
            //            {
            //                max_item = j;
            //                max_str = strkey_list[j];
            //            }
            //        }

            //        if (max_item != i)
            //        {
            //            buf = list[i];
            //            list[i] = list[max_item];
            //            list[max_item] = buf;
            //            buf = strkey_list[i];
            //            strkey_list[i] = max_str;
            //            strkey_list[max_item] = buf;
            //        }
            //    }

            //    // スペシャルパワーを使用可能なパイロットがいるかどうかを判定
            //    var loopTo9 = Information.UBound(list);
            //    for (i = 1; i <= loopTo9; i++)
            //    {
            //        GUI.ListItemFlag[i] = true;
            //        foreach (Pilot currentP in SRC.PList)
            //        {
            //            p = currentP;
            //            if (p.Party == "味方")
            //            {
            //                if (p.Unit_Renamed is object)
            //                {
            //                    object argIndex8 = "憑依";
            //                    if (p.Unit_Renamed.Status_Renamed == "出撃" & !p.Unit_Renamed.IsConditionSatisfied(argIndex8))
            //                    {
            //                        // 本当に乗っている？
            //                        found = false;
            //                        {
            //                            var withBlock6 = p.Unit_Renamed;
            //                            if (ReferenceEquals(p, withBlock6.MainPilot()))
            //                            {
            //                                found = true;
            //                            }
            //                            else
            //                            {
            //                                var loopTo10 = withBlock6.CountPilot();
            //                                for (j = 2; j <= loopTo10; j++)
            //                                {
            //                                    Pilot localPilot1() { object argIndex1 = j; var ret = withBlock6.Pilot(argIndex1); return ret; }

            //                                    if (ReferenceEquals(p, localPilot1()))
            //                                    {
            //                                        found = true;
            //                                        break;
            //                                    }
            //                                }

            //                                var loopTo11 = withBlock6.CountSupport();
            //                                for (j = 1; j <= loopTo11; j++)
            //                                {
            //                                    Pilot localSupport() { object argIndex1 = j; var ret = withBlock6.Support(argIndex1); return ret; }

            //                                    if (ReferenceEquals(p, localSupport()))
            //                                    {
            //                                        found = true;
            //                                        break;
            //                                    }
            //                                }

            //                                if (ReferenceEquals(p, withBlock6.AdditionalSupport()))
            //                                {
            //                                    found = true;
            //                                }
            //                            }
            //                        }

            //                        if (found)
            //                        {
            //                            if (p.IsSpecialPowerAvailable(list[i]))
            //                            {
            //                                GUI.ListItemFlag[i] = false;
            //                                break;
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // 各スペシャルパワーが使用可能か判定
            //    {
            //        var withBlock7 = SelectedPilot;
            //        var loopTo12 = Information.UBound(list);
            //        for (i = 1; i <= loopTo12; i++)
            //        {
            //            if (!GUI.ListItemFlag[i] & withBlock7.SP >= 2 * withBlock7.SpecialPowerCost(list[i]))
            //            {
            //                if (!withBlock7.IsSpecialPowerUseful(list[i]))
            //                {
            //                    GUI.ListItemFlag[i] = true;
            //                }
            //            }
            //            else
            //            {
            //                GUI.ListItemFlag[i] = true;
            //            }
            //        }
            //    }

            //    // スペシャルパワーの解説を設定
            //    GUI.ListItemComment = new string[Information.UBound(list) + 1];
            //    var loopTo13 = Information.UBound(list);
            //    for (i = 1; i <= loopTo13; i++)
            //    {
            //        SpecialPowerData localItem4() { var tmp = list; object argIndex1 = tmp[i]; var ret = SRC.SPDList.Item(argIndex1); return ret; }

            //        GUI.ListItemComment[i] = localItem4().Comment;
            //    }

            //    // 検索するスペシャルパワーを選択
            //    GUI.TopItem = 1;
            //    string argtname7 = "スペシャルパワー";
            //    Unit argu = null;
            //    string arglb_caption3 = Expression.Term(argtname7, u: argu) + "検索";
            //    ret = GUI.MultiColumnListBox(arglb_caption3, list, true);
            //    if (ret == 0)
            //    {
            //        SelectedSpecialPower = "";
            //        CancelCommand();
            //        GUI.UnlockGUI();
            //        return;
            //    }

            //    // スペシャルパワー使用メッセージ
            //    if (SelectedUnit.IsMessageDefined(SelectedSpecialPower))
            //    {
            //        Unit argu1 = null;
            //        Unit argu2 = null;
            //        GUI.OpenMessageForm(u1: argu1, u2: argu2);
            //        string argmsg_mode = "";
            //        SelectedUnit.PilotMessage(SelectedSpecialPower, msg_mode: argmsg_mode);
            //        GUI.CloseMessageForm();
            //    }

            //    SelectedSpecialPower = list[ret];
            //    WithDoubleSPConsumption = true;
            //}
            //else
            //{
            //    WithDoubleSPConsumption = false;
            //}

            //object argIndex9 = SelectedSpecialPower;
            //sd = SRC.SPDList.Item(argIndex9);

            //// ターゲットを選択する必要があるスペシャルパワーの場合
            //switch (sd.TargetType ?? "")
            //{
            //    case "味方":
            //    case "敵":
            //    case "任意":
            //        {
            //            // マップ上のユニットからターゲットを選択する

            //            Unit argu11 = null;
            //            Unit argu21 = null;
            //            GUI.OpenMessageForm(u1: argu11, u2: argu21);
            //            GUI.DisplaySysMessage(SelectedPilot.get_Nickname(false) + "は" + SelectedSpecialPower + "を使った。;" + "ターゲットを選んでください。");
            //            GUI.CloseMessageForm();

            //            // ターゲットのエリアを設定
            //            var loopTo14 = Map.MapWidth;
            //            for (i = 1; i <= loopTo14; i++)
            //            {
            //                var loopTo15 = Map.MapHeight;
            //                for (j = 1; j <= loopTo15; j++)
            //                {
            //                    Map.MaskData[i, j] = true;
            //                    u = Map.MapDataForUnit[i, j];
            //                    if (u is null)
            //                    {
            //                        goto NextLoop;
            //                    }

            //                    // 陣営が合っている？
            //                    switch (sd.TargetType ?? "")
            //                    {
            //                        case "味方":
            //                            {
            //                                {
            //                                    var withBlock8 = u;
            //                                    if (withBlock8.Party != "味方" & withBlock8.Party0 != "味方" & withBlock8.Party != "ＮＰＣ" & withBlock8.Party0 != "ＮＰＣ")
            //                                    {
            //                                        goto NextLoop;
            //                                    }
            //                                }

            //                                break;
            //                            }

            //                        case "敵":
            //                            {
            //                                {
            //                                    var withBlock9 = u;
            //                                    if (withBlock9.Party == "味方" & withBlock9.Party0 == "味方" | withBlock9.Party == "ＮＰＣ" & withBlock9.Party0 == "ＮＰＣ")
            //                                    {
            //                                        goto NextLoop;
            //                                    }
            //                                }

            //                                break;
            //                            }
            //                    }

            //                    // スペシャルパワーを適用可能？
            //                    if (!sd.Effective(SelectedPilot, u))
            //                    {
            //                        goto NextLoop;
            //                    }

            //                    Map.MaskData[i, j] = false;
            //                NextLoop:
            //                    ;
            //                }
            //            }

            //            GUI.MaskScreen();
            //            CommandState = "ターゲット選択";
            //            GUI.UnlockGUI();
            //            return;
            //        }

            //    case "破壊味方":
            //        {
            //            // 破壊された味方ユニットの中からターゲットを選択する

            //            Unit argu12 = null;
            //            Unit argu22 = null;
            //            GUI.OpenMessageForm(u1: argu12, u2: argu22);
            //            GUI.DisplaySysMessage(SelectedPilot.get_Nickname(false) + "は" + SelectedSpecialPower + "を使った。;" + "復活させるユニットを選んでください。");
            //            GUI.CloseMessageForm();

            //            // 破壊された味方ユニットのリストを作成
            //            list = new string[1];
            //            id_list = new string[1];
            //            GUI.ListItemFlag = new bool[1];
            //            foreach (Unit currentU in SRC.UList)
            //            {
            //                u = currentU;
            //                if (u.Party0 == "味方" & u.Status_Renamed == "破壊" & (u.CountPilot() > 0 | u.Data.PilotNum == 0))
            //                {
            //                    Array.Resize(list, Information.UBound(list) + 1 + 1);
            //                    Array.Resize(id_list, Information.UBound(list) + 1);
            //                    Array.Resize(GUI.ListItemFlag, Information.UBound(list) + 1);
            //                    string localRightPaddedString6() { string argbuf = u.Nickname; var ret = GeneralLib.RightPaddedString(argbuf, 28); u.Nickname = argbuf; return ret; }

            //                    string localRightPaddedString7() { string argbuf = u.MainPilot().get_Nickname(false); var ret = GeneralLib.RightPaddedString(argbuf, 18); u.MainPilot().get_Nickname(false) = argbuf; return ret; }

            //                    string localLeftPaddedString1() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(u.MainPilot().Level); var ret = GeneralLib.LeftPaddedString(argbuf, 3); return ret; }

            //                    list[Information.UBound(list)] = localRightPaddedString6() + localRightPaddedString7() + localLeftPaddedString1();
            //                    id_list[Information.UBound(list)] = u.ID;
            //                    GUI.ListItemFlag[Information.UBound(list)] = false;
            //                }
            //            }

            //            GUI.TopItem = 1;
            //            string arglb_caption4 = "ユニット選択";
            //            string arglb_info3 = "ユニット名                  パイロット     レベル";
            //            string arglb_mode3 = "";
            //            i = GUI.ListBox(arglb_caption4, list, arglb_info3, lb_mode: arglb_mode3);
            //            if (i == 0)
            //            {
            //                GUI.UnlockGUI();
            //                CancelCommand();
            //                return;
            //            }

            //            var tmp1 = id_list;
            //            object argIndex10 = tmp1[i];
            //            SelectedTarget = SRC.UList.Item(argIndex10);
            //            break;
            //        }
            //}

            //// 自爆を選択した場合は確認を取る
            //// UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(自爆) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //string argename = "自爆";
            //if (Conversions.ToBoolean(sd.IsEffectAvailable(argename)))
            //{
            //    ret = Interaction.MsgBox("自爆させますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "自爆");
            //    if (ret == MsgBoxResult.Cancel)
            //    {
            //        GUI.UnlockGUI();
            //        return;
            //    }
            //}

            //// 使用イベント
            //Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, SelectedSpecialPower);
            //if (SRC.IsScenarioFinished)
            //{
            //    SRC.IsScenarioFinished = false;
            //    GUI.UnlockGUI();
            //    return;
            //}

            //if (SRC.IsCanceled)
            //{
            //    SRC.IsCanceled = false;
            //    GUI.UnlockGUI();
            //    return;
            //}

            //// スペシャルパワーを使用
            //if (WithDoubleSPConsumption)
            //{
            //    SelectedPilot.UseSpecialPower(SelectedSpecialPower, 2d);
            //}
            //else
            //{
            //    SelectedPilot.UseSpecialPower(SelectedSpecialPower);
            //}

            //SelectedUnit = SelectedUnit.CurrentForm();

            //// カーソル自動移動
            //if (SRC.AutoMoveCursor)
            //{
            //    string argcursor_mode1 = "ユニット選択";
            //    GUI.MoveCursorPos(argcursor_mode1, SelectedUnit);
            //}

            //// ステータスウィンドウを更新
            //Status.DisplayUnitStatus(SelectedUnit);

            //// 使用後イベント
            //Event_Renamed.HandleEvent("使用後", SelectedUnit.MainPilot().ID, SelectedSpecialPower);
            //if (SRC.IsScenarioFinished)
            //{
            //    SRC.IsScenarioFinished = false;
            //}

            //if (SRC.IsCanceled)
            //{
            //    SRC.IsCanceled = false;
            //}

            //SelectedSpecialPower = "";
            //GUI.UnlockGUI();
            //CommandState = "ユニット選択";
        }

        // スペシャルパワーコマンドを終了
        private void FinishSpecialPowerCommand()
        {
            throw new NotImplementedException();
            //int i, ret;
            //GUI.LockGUI();

            //// 自爆を選択した場合は確認を取る
            //object argIndex1 = SelectedSpecialPower;
            //{
            //    var withBlock = SRC.SPDList.Item(argIndex1);
            //    var loopTo = withBlock.CountEffect();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        if (withBlock.EffectType(i) == "自爆")
            //        {
            //            ret = Interaction.MsgBox("自爆させますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "自爆");
            //            if (ret == MsgBoxResult.Cancel)
            //            {
            //                CommandState = "ユニット選択";
            //                GUI.UnlockGUI();
            //                return;
            //            }

            //            break;
            //        }
            //    }
            //}

            //// 使用イベント
            //Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, SelectedSpecialPower);
            //if (SRC.IsScenarioFinished)
            //{
            //    SRC.IsScenarioFinished = false;
            //    CommandState = "ユニット選択";
            //    GUI.UnlockGUI();
            //    return;
            //}

            //if (SRC.IsCanceled)
            //{
            //    SRC.IsCanceled = false;
            //    CommandState = "ユニット選択";
            //    GUI.UnlockGUI();
            //    return;
            //}

            //// スペシャルパワーを使用
            //if (WithDoubleSPConsumption)
            //{
            //    SelectedPilot.UseSpecialPower(SelectedSpecialPower, 2d);
            //}
            //else
            //{
            //    SelectedPilot.UseSpecialPower(SelectedSpecialPower);
            //}

            //SelectedUnit = SelectedUnit.CurrentForm();

            //// ステータスウィンドウを更新
            //if (SelectedTarget is object)
            //{
            //    if (SelectedTarget.CurrentForm().Status_Renamed == "出撃")
            //    {
            //        Status.DisplayUnitStatus(SelectedTarget);
            //    }
            //}

            //// 使用後イベント
            //Event_Renamed.HandleEvent("使用後", SelectedUnit.MainPilot().ID, SelectedSpecialPower);
            //if (SRC.IsScenarioFinished)
            //{
            //    SRC.IsScenarioFinished = false;
            //}

            //if (SRC.IsCanceled)
            //{
            //    SRC.IsCanceled = false;
            //}

            //SelectedSpecialPower = "";
            //GUI.UnlockGUI();
            //CommandState = "ユニット選択";
        }
    }
}