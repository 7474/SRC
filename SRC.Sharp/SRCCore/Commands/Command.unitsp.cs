// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Lib;
using SRCCore.Pilots;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Commands
{
    public partial class Command
    {
        // スペシャルパワーコマンドを開始
        private void StartSpecialPowerCommand()
        {
            GUI.LockGUI();
            SelectedCommand = "スペシャルパワー";
            {
                var u = SelectedUnit;

                // スペシャルパワーを使用可能なパイロットの一覧を作成
                var pilots = u.PilotsHaveSpecialPower();
                var listItems = pilots.Select(p =>
                {
                    return new ListBoxItem
                    {
                        Text = GeneralLib.RightPaddedString(p.get_Nickname(false), 17)
                            + GeneralLib.RightPaddedString($"{p.SP}/{p.MaxSP}", 8)
                            + string.Join("", Enumerable.Range(1, p.CountSpecialPower).Select(i =>
                            {
                                var sname = p.get_SpecialPower(i);
                                if (p.SP >= p.SpecialPowerCost(sname))
                                {
                                    return SRC.SPDList.Item(sname).ShortName;
                                }
                                else
                                {
                                    return "";
                                }
                            })),
                    };
                }).ToList();

                GUI.TopItem = 1;
                int i;
                if (pilots.Count > 1)
                {
                    // どのパイロットを使うか選択
                    string argoname = "等身大基準";
                    if (Expression.IsOptionDefined(argoname))
                    {
                        i = GUI.ListBox(new ListBoxArgs
                        {
                            Items = listItems,
                            HasFlag = false,
                            lb_caption = "キャラクター選択",
                            lb_info = "キャラクター     " + Expression.Term("SP", SelectedUnit, 2) + "/Max" + Expression.Term("SP", SelectedUnit, 2),
                            lb_mode = "連続表示,カーソル移動"
                        });
                    }
                    else
                    {
                        i = GUI.ListBox(new ListBoxArgs
                        {
                            Items = listItems,
                            HasFlag = false,
                            lb_caption = "パイロット選択",
                            lb_info = "パイロット       " + Expression.Term("SP", SelectedUnit, 2) + "/Max" + Expression.Term("SP", SelectedUnit, 2),
                            lb_mode = "連続表示,カーソル移動"
                        });
                    }
                }
                else
                {
                    // 一人しかいないので選択の必要なし
                    i = 1;
                }

                // 誰もスペシャルパワーを使えなければキャンセル
                if (i == 0)
                {
                    GUI.CloseListBox();
                    if (SRC.AutoMoveCursor)
                    {
                        GUI.RestoreCursorPos();
                    }

                    GUI.UnlockGUI();
                    CancelCommand();
                    return;
                }

                // スペシャルパワーを使うパイロットを設定
                SelectedPilot = pilots[i - 1];
                // そのパイロットのステータスを表示
                if (pilots.Count > 1)
                {
                    Status.DisplayPilotStatus(SelectedPilot);
                }
            }

            {
                var p = SelectedPilot;
                // 使用可能なスペシャルパワーの一覧を作成
                var spList = p.SpecialPowerNames.Select(sname =>
                {
                    var cost = p.SpecialPowerCost(sname);
                    var spd = SRC.SPDList.Item(sname);

                    return new ListBoxItem
                    {
                        Text = GeneralLib.RightPaddedString(sname, 13) + GeneralLib.LeftPaddedString("" + cost, 3) + " " + spd.Comment,
                        ListItemFlag = p.SP < cost || !p.IsSpecialPowerUseful(sname),
                    };
                }).ToList();

                // どのコマンドを使用するかを選択
                GUI.TopItem = 1;
                var i = GUI.ListBox(new ListBoxArgs
                {
                    Items = spList,
                    HasFlag = true,
                    lb_caption = Expression.Term("スペシャルパワー", SelectedUnit) + "選択",
                    lb_info = "名称         消費" + Expression.Term("SP", SelectedUnit) + "（" + p.get_Nickname(false) + " " + Expression.Term("SP", SelectedUnit) + "=" + SrcFormatter.Format(p.SP) + "/" + SrcFormatter.Format(p.MaxSP) + "）",
                    lb_mode = "カーソル移動(行きのみ)"
                });
                // キャンセル
                if (i == 0)
                {
                    Status.DisplayUnitStatus(SelectedUnit);
                    // カーソル自動移動
                    if (SRC.AutoMoveCursor)
                    {
                        string argcursor_mode = "ユニット選択";
                        GUI.MoveCursorPos(argcursor_mode, SelectedUnit);
                    }

                    GUI.UnlockGUI();
                    CancelCommand();
                    return;
                }

                // 使用するスペシャルパワーを設定
                SelectedSpecialPower = SelectedPilot.get_SpecialPower(i);
            }

            // 味方スペシャルパワー実行の効果により他のパイロットが持っているスペシャルパワーを
            // 使う場合は記録しておき、後で消費ＳＰを倍にする必要がある
            // TODO Impl 夢
            //if (SRC.SPDList.Item(SelectedSpecialPower).EffectType(1) == "味方スペシャルパワー実行")
            if (false)
            {
                //// スペシャルパワー一覧
                //list = new string[1];
                //var loopTo6 = SRC.SPDList.Count();
                //for (i = 1; i <= loopTo6; i++)
                //{
                //    object argIndex7 = i;
                //    {
                //        var withBlock5 = SRC.SPDList.Item(argIndex7);
                //        if (withBlock5.EffectType(1) != "味方スペシャルパワー実行" & withBlock5.intName != "非表示")
                //        {
                //            Array.Resize(list, Information.UBound(list) + 1 + 1);
                //            Array.Resize(strkey_list, Information.UBound(list) + 1);
                //            list[Information.UBound(list)] = withBlock5.Name;
                //            strkey_list[Information.UBound(list)] = withBlock5.KanaName;
                //        }
                //    }
                //}

                //GUI.ListItemFlag = new bool[Information.UBound(list) + 1];

                //// ソート
                //var loopTo7 = (Information.UBound(strkey_list) - 1);
                //for (i = 1; i <= loopTo7; i++)
                //{
                //    max_item = i;
                //    max_str = strkey_list[i];
                //    var loopTo8 = Information.UBound(strkey_list);
                //    for (j = (i + 1); j <= loopTo8; j++)
                //    {
                //        if (Strings.StrComp(strkey_list[j], max_str, (CompareMethod)1) == -1)
                //        {
                //            max_item = j;
                //            max_str = strkey_list[j];
                //        }
                //    }

                //    if (max_item != i)
                //    {
                //        buf = list[i];
                //        list[i] = list[max_item];
                //        list[max_item] = buf;
                //        buf = strkey_list[i];
                //        strkey_list[i] = max_str;
                //        strkey_list[max_item] = buf;
                //    }
                //}

                //// スペシャルパワーを使用可能なパイロットがいるかどうかを判定
                //var loopTo9 = Information.UBound(list);
                //for (i = 1; i <= loopTo9; i++)
                //{
                //    GUI.ListItemFlag[i] = true;
                //    foreach (Pilot currentP in SRC.PList)
                //    {
                //        p = currentP;
                //        if (p.Party == "味方")
                //        {
                //            if (p.Unit is object)
                //            {
                //                object argIndex8 = "憑依";
                //                if (p.Unit.Status == "出撃" & !p.Unit.IsConditionSatisfied(argIndex8))
                //                {
                //                    // 本当に乗っている？
                //                    found = false;
                //                    {
                //                        var withBlock6 = p.Unit;
                //                        if (ReferenceEquals(p, withBlock6.MainPilot()))
                //                        {
                //                            found = true;
                //                        }
                //                        else
                //                        {
                //                            var loopTo10 = withBlock6.CountPilot();
                //                            for (j = 2; j <= loopTo10; j++)
                //                            {
                //                                Pilot localPilot1() { object argIndex1 = j; var ret = withBlock6.Pilot(argIndex1); return ret; }

                //                                if (ReferenceEquals(p, localPilot1()))
                //                                {
                //                                    found = true;
                //                                    break;
                //                                }
                //                            }

                //                            var loopTo11 = withBlock6.CountSupport();
                //                            for (j = 1; j <= loopTo11; j++)
                //                            {
                //                                Pilot localSupport() { object argIndex1 = j; var ret = withBlock6.Support(argIndex1); return ret; }

                //                                if (ReferenceEquals(p, localSupport()))
                //                                {
                //                                    found = true;
                //                                    break;
                //                                }
                //                            }

                //                            if (ReferenceEquals(p, withBlock6.AdditionalSupport()))
                //                            {
                //                                found = true;
                //                            }
                //                        }
                //                    }

                //                    if (found)
                //                    {
                //                        if (p.IsSpecialPowerAvailable(list[i]))
                //                        {
                //                            GUI.ListItemFlag[i] = false;
                //                            break;
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

                //// 各スペシャルパワーが使用可能か判定
                //{
                //    var withBlock7 = SelectedPilot;
                //    var loopTo12 = Information.UBound(list);
                //    for (i = 1; i <= loopTo12; i++)
                //    {
                //        if (!GUI.ListItemFlag[i] & withBlock7.SP >= 2 * withBlock7.SpecialPowerCost(list[i]))
                //        {
                //            if (!withBlock7.IsSpecialPowerUseful(list[i]))
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

                //// スペシャルパワーの解説を設定
                //GUI.ListItemComment = new string[Information.UBound(list) + 1];
                //var loopTo13 = Information.UBound(list);
                //for (i = 1; i <= loopTo13; i++)
                //{
                //    SpecialPowerData localItem4() { var tmp = list; object argIndex1 = tmp[i]; var ret = SRC.SPDList.Item(argIndex1); return ret; }

                //    GUI.ListItemComment[i] = localItem4().Comment;
                //}

                //// 検索するスペシャルパワーを選択
                //GUI.TopItem = 1;
                //string argtname7 = "スペシャルパワー";
                //Unit argu = null;
                //string arglb_caption3 = Expression.Term(argtname7, u: argu) + "検索";
                //ret = GUI.MultiColumnListBox(arglb_caption3, list, true);
                //if (ret == 0)
                //{
                //    SelectedSpecialPower = "";
                //    CancelCommand();
                //    GUI.UnlockGUI();
                //    return;
                //}

                //// スペシャルパワー使用メッセージ
                //if (SelectedUnit.IsMessageDefined(SelectedSpecialPower))
                //{
                //    Unit argu1 = null;
                //    Unit argu2 = null;
                //    GUI.OpenMessageForm(u1: argu1, u2: argu2);
                //    string argmsg_mode = "";
                //    SelectedUnit.PilotMessage(SelectedSpecialPower, msg_mode: argmsg_mode);
                //    GUI.CloseMessageForm();
                //}

                //SelectedSpecialPower = list[ret];
                //WithDoubleSPConsumption = true;
            }
            else
            {
                WithDoubleSPConsumption = false;
            }

            var sd = SRC.SPDList.Item(SelectedSpecialPower);

            // ターゲットを選択する必要があるスペシャルパワーの場合
            switch (sd.TargetType ?? "")
            {
                case "味方":
                case "敵":
                case "任意":
                    {
                        // マップ上のユニットからターゲットを選択する
                        GUI.OpenMessageForm(null, null);
                        GUI.DisplaySysMessage(SelectedPilot.get_Nickname(false) + "は" + SelectedSpecialPower + "を使った。;" + "ターゲットを選んでください。");
                        GUI.CloseMessageForm();

                        // ターゲットのエリアを設定
                        for (var i = 1; i <= Map.MapWidth; i++)
                        {
                            for (var j = 1; j <= Map.MapHeight; j++)
                            {
                                Map.MaskData[i, j] = true;
                                var u = Map.MapDataForUnit[i, j];
                                if (u is null)
                                {
                                    goto NextLoop;
                                }

                                // 陣営が合っている？
                                switch (sd.TargetType ?? "")
                                {
                                    case "味方":
                                        {
                                            {
                                                var withBlock8 = u;
                                                if (withBlock8.Party != "味方" & withBlock8.Party0 != "味方" & withBlock8.Party != "ＮＰＣ" & withBlock8.Party0 != "ＮＰＣ")
                                                {
                                                    goto NextLoop;
                                                }
                                            }

                                            break;
                                        }

                                    case "敵":
                                        {
                                            {
                                                var withBlock9 = u;
                                                if (withBlock9.Party == "味方" & withBlock9.Party0 == "味方" | withBlock9.Party == "ＮＰＣ" & withBlock9.Party0 == "ＮＰＣ")
                                                {
                                                    goto NextLoop;
                                                }
                                            }

                                            break;
                                        }
                                }

                                // スペシャルパワーを適用可能？
                                if (!sd.Effective(SelectedPilot, u))
                                {
                                    goto NextLoop;
                                }

                                Map.MaskData[i, j] = false;
                            NextLoop:
                                ;
                            }
                        }

                        GUI.MaskScreen();
                        CommandState = "ターゲット選択";
                        GUI.UnlockGUI();
                        return;
                    }

                case "破壊味方":
                    throw new NotImplementedException();
                    {
                        //    // 破壊された味方ユニットの中からターゲットを選択する
                        //    GUI.OpenMessageForm(null, null);
                        //    GUI.DisplaySysMessage(SelectedPilot.get_Nickname(false) + "は" + SelectedSpecialPower + "を使った。;" + "復活させるユニットを選んでください。");
                        //    GUI.CloseMessageForm();

                        //    // 破壊された味方ユニットのリストを作成
                        //    list = new string[1];
                        //    id_list = new string[1];
                        //    GUI.ListItemFlag = new bool[1];
                        //    foreach (Unit currentU in SRC.UList)
                        //    {
                        //        u = currentU;
                        //        if (u.Party0 == "味方" & u.Status == "破壊" & (u.CountPilot() > 0 | u.Data.PilotNum == 0))
                        //        {
                        //            Array.Resize(list, Information.UBound(list) + 1 + 1);
                        //            Array.Resize(id_list, Information.UBound(list) + 1);
                        //            Array.Resize(GUI.ListItemFlag, Information.UBound(list) + 1);
                        //            string localRightPaddedString6() { string argbuf = u.Nickname; var ret = GeneralLib.RightPaddedString(argbuf, 28); u.Nickname = argbuf; return ret; }

                        //            string localRightPaddedString7() { string argbuf = u.MainPilot().get_Nickname(false); var ret = GeneralLib.RightPaddedString(argbuf, 18); u.MainPilot().get_Nickname(false) = argbuf; return ret; }

                        //            string localLeftPaddedString1() { string argbuf = SrcFormatter.Format(u.MainPilot().Level); var ret = GeneralLib.LeftPaddedString(argbuf, 3); return ret; }

                        //            list[Information.UBound(list)] = localRightPaddedString6() + localRightPaddedString7() + localLeftPaddedString1();
                        //            id_list[Information.UBound(list)] = u.ID;
                        //            GUI.ListItemFlag[Information.UBound(list)] = false;
                        //        }
                        //    }

                        //    GUI.TopItem = 1;
                        //    string arglb_caption4 = "ユニット選択";
                        //    string arglb_info3 = "ユニット名                  パイロット     レベル";
                        //    string arglb_mode3 = "";
                        //    i = GUI.ListBox(arglb_caption4, list, arglb_info3, lb_mode: arglb_mode3);
                        //    if (i == 0)
                        //    {
                        //        GUI.UnlockGUI();
                        //        CancelCommand();
                        //        return;
                        //    }

                        //    var tmp1 = id_list;
                        //    object argIndex10 = tmp1[i];
                        //    SelectedTarget = SRC.UList.Item(argIndex10);
                        //    break;
                    }
            }

            // 自爆を選択した場合は確認を取る
            if (sd.IsEffectAvailable("自爆"))
            {
                var ret = GUI.Confirm("自爆させますか？", "自爆", GuiConfirmOption.OkCancel | GuiConfirmOption.Question);
                if (ret == GuiDialogResult.Ok)
                {
                    GUI.UnlockGUI();
                    return;
                }
            }

            // 使用イベント
            Event.HandleEvent("使用", SelectedUnit.MainPilot().ID, SelectedSpecialPower);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                GUI.UnlockGUI();
                return;
            }

            // スペシャルパワーを使用
            if (WithDoubleSPConsumption)
            {
                SelectedPilot.UseSpecialPower(SelectedSpecialPower, 2d);
            }
            else
            {
                SelectedPilot.UseSpecialPower(SelectedSpecialPower);
            }

            SelectedUnit = SelectedUnit.CurrentForm();

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                GUI.MoveCursorPos("ユニット選択", SelectedUnit);
            }

            // ステータスウィンドウを更新
            Status.DisplayUnitStatus(SelectedUnit);

            // 使用後イベント
            Event.HandleEvent("使用後", SelectedUnit.MainPilot().ID, SelectedSpecialPower);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
            }

            SelectedSpecialPower = "";
            GUI.UnlockGUI();
            CommandState = "ユニット選択";
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
            //Event.HandleEvent("使用", SelectedUnit.MainPilot().ID, SelectedSpecialPower);
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
            //    if (SelectedTarget.CurrentForm().Status == "出撃")
            //    {
            //        Status.DisplayUnitStatus(SelectedTarget);
            //    }
            //}

            //// 使用後イベント
            //Event.HandleEvent("使用後", SelectedUnit.MainPilot().ID, SelectedSpecialPower);
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