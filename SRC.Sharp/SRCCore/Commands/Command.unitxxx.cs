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
        // 「チャージ」コマンド
        private void ChargeCommand()
        {
            throw new NotImplementedException();
            //int ret;
            //Unit[] partners;
            //int i;
            //GUI.LockGUI();
            //ret = Interaction.MsgBox("チャージを開始しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "チャージ開始");
            //if (ret == MsgBoxResult.Cancel)
            //{
            //    CancelCommand();
            //    GUI.UnlockGUI();
            //    return;
            //}

            //// 使用イベント
            //Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, "チャージ");
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

            //{
            //    var withBlock = SelectedUnit;
            //    // チャージのメッセージを表示
            //    string argmain_situation = "チャージ";
            //    if (withBlock.IsMessageDefined(argmain_situation))
            //    {
            //        Unit argu1 = null;
            //        Unit argu2 = null;
            //        GUI.OpenMessageForm(u1: argu1, u2: argu2);
            //        string argSituation = "チャージ";
            //        string argmsg_mode = "";
            //        withBlock.PilotMessage(argSituation, msg_mode: argmsg_mode);
            //        GUI.CloseMessageForm();
            //    }

            //    // アニメ表示を行う
            //    string argmain_situation3 = "チャージ";
            //    string argsub_situation2 = "";
            //    string argmain_situation4 = "チャージ";
            //    string argsub_situation3 = "";
            //    if (withBlock.IsAnimationDefined(argmain_situation3, sub_situation: argsub_situation2))
            //    {
            //        string argmain_situation1 = "チャージ";
            //        string argsub_situation = "";
            //        withBlock.PlayAnimation(argmain_situation1, sub_situation: argsub_situation);
            //    }
            //    else if (withBlock.IsSpecialEffectDefined(argmain_situation4, sub_situation: argsub_situation3))
            //    {
            //        string argmain_situation2 = "チャージ";
            //        string argsub_situation1 = "";
            //        withBlock.SpecialEffect(argmain_situation2, sub_situation: argsub_situation1);
            //    }
            //    else
            //    {
            //        string argwave_name = "Charge.wav";
            //        Sound.PlayWave(argwave_name);
            //    }

            //    // チャージ攻撃のパートナーを探す
            //    partners = new Unit[1];
            //    var loopTo = withBlock.CountWeapon();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        string argattr = "Ｃ";
            //        string argattr1 = "合";
            //        if (withBlock.IsWeaponClassifiedAs(i, argattr) & withBlock.IsWeaponClassifiedAs(i, argattr1))
            //        {
            //            string argref_mode = "チャージ";
            //            if (withBlock.IsWeaponAvailable(i, argref_mode))
            //            {
            //                string argctype_Renamed = "武装";
            //                withBlock.CombinationPartner(argctype_Renamed, i, partners);
            //                break;
            //            }
            //        }
            //    }

            //    if (Information.UBound(partners) == 0)
            //    {
            //        var loopTo1 = withBlock.CountAbility();
            //        for (i = 1; i <= loopTo1; i++)
            //        {
            //            string argattr2 = "Ｃ";
            //            string argattr3 = "合";
            //            if (withBlock.IsAbilityClassifiedAs(i, argattr2) & withBlock.IsAbilityClassifiedAs(i, argattr3))
            //            {
            //                string argref_mode1 = "チャージ";
            //                if (withBlock.IsAbilityAvailable(i, argref_mode1))
            //                {
            //                    string argctype_Renamed1 = "アビリティ";
            //                    withBlock.CombinationPartner(argctype_Renamed1, i, partners);
            //                    break;
            //                }
            //            }
            //        }
            //    }

            //    // ユニットの状態をチャージ中に
            //    string argcname = "チャージ";
            //    string argcdata = "";
            //    withBlock.AddCondition(argcname, 1, cdata: argcdata);

            //    // チャージ攻撃のパートナーもチャージ中にする
            //    var loopTo2 = Information.UBound(partners);
            //    for (i = 1; i <= loopTo2; i++)
            //    {
            //        {
            //            var withBlock1 = partners[i];
            //            string argcname1 = "チャージ";
            //            string argcdata1 = "";
            //            withBlock1.AddCondition(argcname1, 1, cdata: argcdata1);
            //        }
            //    }
            //}

            //// 使用後イベント
            //Event_Renamed.HandleEvent("使用後", SelectedUnit.MainPilot().ID, "チャージ");
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

            //GUI.UnlockGUI();

            //// 行動終了
            //WaitCommand();
        }

        // 「会話」コマンドを開始
        private void StartTalkCommand()
        {
            throw new NotImplementedException();
            //int i, j;
            //Unit t;
            //SelectedCommand = "会話";

            //// UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //t = null;

            //// 会話可能なユニットを表示
            //{
            //    var withBlock = SelectedUnit;
            //    string arguparty = "";
            //    Map.AreaInRange(withBlock.x, withBlock.y, 1, 1, arguparty);
            //    var loopTo = Map.MapWidth;
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        var loopTo1 = Map.MapHeight;
            //        for (j = 1; j <= loopTo1; j++)
            //        {
            //            if (!Map.MaskData[i, j])
            //            {
            //                if (Map.MapDataForUnit[i, j] is object)
            //                {
            //                    bool localIsEventDefined() { var arglname = "会話 " + withBlock.MainPilot().ID + " " + Map.MapDataForUnit[i, j].MainPilot.ID; var ret = Event_Renamed.IsEventDefined(arglname); return ret; }

            //                    if (!localIsEventDefined())
            //                    {
            //                        Map.MaskData[i, j] = true;
            //                        t = Map.MapDataForUnit[i, j];
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    Map.MaskData[withBlock.x, withBlock.y] = false;
            //}

            //GUI.MaskScreen();

            //// カーソル自動移動
            //if (SRC.AutoMoveCursor)
            //{
            //    if (t is object)
            //    {
            //        string argcursor_mode = "ユニット選択";
            //        GUI.MoveCursorPos(argcursor_mode, t);
            //        Status.DisplayUnitStatus(t);
            //    }
            //}

            //if (CommandState == "コマンド選択")
            //{
            //    CommandState = "ターゲット選択";
            //}
            //else
            //{
            //    CommandState = "移動後ターゲット選択";
            //}
        }

        // 「会話」コマンドを終了
        private void FinishTalkCommand()
        {
            throw new NotImplementedException();
            //Pilot p;
            //GUI.LockGUI();
            //if (SelectedUnit.CountPilot() > 0)
            //{
            //    object argIndex1 = 1;
            //    p = SelectedUnit.Pilot(argIndex1);
            //}
            //else
            //{
            //    // UPGRADE_NOTE: オブジェクト p をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    p = null;
            //}

            //// 会話イベントを実施
            //Event_Renamed.HandleEvent("会話", SelectedUnit.MainPilot().ID, SelectedTarget.MainPilot().ID);
            //if (SRC.IsScenarioFinished)
            //{
            //    SRC.IsScenarioFinished = false;
            //    return;
            //}

            //if (p is object)
            //{
            //    if (p.Unit_Renamed is object)
            //    {
            //        SelectedUnit = p.Unit_Renamed;
            //    }
            //}

            //GUI.UnlockGUI();

            //// 行動終了
            //WaitCommand();
        }

        // 「発進」コマンドを開始
        private void StartLaunchCommand()
        {
            throw new NotImplementedException();
            //// MOD END MARGE
            //int i, ret;
            //string[] list;
            //{
            //    var withBlock = SelectedUnit;
            //    list = new string[(withBlock.CountUnitOnBoard() + 1)];
            //    GUI.ListItemID = new string[(withBlock.CountUnitOnBoard() + 1)];
            //    GUI.ListItemFlag = new bool[(withBlock.CountUnitOnBoard() + 1)];
            //}

            //// 母艦に搭載しているユニットの一覧を作成
            //var loopTo = SelectedUnit.CountUnitOnBoard();
            //for (i = 1; i <= loopTo; i++)
            //{
            //    object argIndex1 = i;
            //    {
            //        var withBlock1 = SelectedUnit.UnitOnBoard(argIndex1);
            //        string localRightPaddedString() { string argbuf = withBlock1.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, 25); withBlock1.Nickname0 = argbuf; return ret; }

            //        string localRightPaddedString1() { string argbuf = withBlock1.MainPilot().get_Nickname(false); var ret = GeneralLib.RightPaddedString(argbuf, 17); withBlock1.MainPilot().get_Nickname(false) = argbuf; return ret; }

            //        string localLeftPaddedString() { string argbuf = SrcFormatter.Format(withBlock1.MainPilot().Level); var ret = GeneralLib.LeftPaddedString(argbuf, 2); return ret; }

            //        string localRightPaddedString2() { string argbuf = SrcFormatter.Format(withBlock1.HP) + "/" + SrcFormatter.Format(withBlock1.MaxHP); var ret = GeneralLib.RightPaddedString(argbuf, 12); return ret; }

            //        string localRightPaddedString3() { string argbuf = SrcFormatter.Format(withBlock1.EN) + "/" + SrcFormatter.Format(withBlock1.MaxEN); var ret = GeneralLib.RightPaddedString(argbuf, 8); return ret; }

            //        list[i] = localRightPaddedString() + localRightPaddedString1() + localLeftPaddedString() + " " + localRightPaddedString2() + localRightPaddedString3();
            //        GUI.ListItemID[i] = withBlock1.ID;
            //        if (withBlock1.Action > 0)
            //        {
            //            GUI.ListItemFlag[i] = false;
            //        }
            //        else
            //        {
            //            GUI.ListItemFlag[i] = true;
            //        }
            //    }
            //}

            //// どのユニットを発進させるか選択
            //GUI.TopItem = 1;
            //string arglb_caption = "ユニット選択";
            //string argtname = "ＨＰ";
            //Unit argu = null;
            //string argtname1 = "ＥＮ";
            //Unit argu1 = null;
            //string arglb_info = "ユニット名               パイロット       Lv " + Expression.Term(argtname, argu, 8) + Expression.Term(argtname1, u: argu1);
            //string arglb_mode = "カーソル移動";
            //ret = GUI.ListBox(arglb_caption, list, arglb_info, arglb_mode);

            //// キャンセルされた？
            //if (ret == 0)
            //{
            //    GUI.ListItemID = new string[1];
            //    CancelCommand();
            //    return;
            //}

            //SelectedCommand = "発進";

            //// ユニットの発進処理
            //var tmp = GUI.ListItemID;
            //object argIndex2 = tmp[ret];
            //SelectedTarget = SRC.UList.Item(argIndex2);
            //{
            //    var withBlock2 = SelectedTarget;
            //    withBlock2.x = SelectedUnit.x;
            //    withBlock2.y = SelectedUnit.y;
            //    string localLIndex() { object argIndex1 = "テレポート"; string arglist = withBlock2.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //    int localLLength() { object argIndex1 = "ジャンプ"; string arglist = withBlock2.FeatureData(argIndex1); var ret = GeneralLib.LLength(arglist); return ret; }

            //    string localLIndex1() { object argIndex1 = "ジャンプ"; string arglist = withBlock2.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //    string argfname = "テレポート";
            //    string argfname1 = "ジャンプ";
            //    if (withBlock2.IsFeatureAvailable(argfname) & (withBlock2.Data.Speed == 0 | localLIndex() == "0"))
            //    {
            //        // テレポートによる発進
            //        Map.AreaInTeleport(SelectedTarget);
            //    }
            //    else if (withBlock2.IsFeatureAvailable(argfname1) & (withBlock2.Data.Speed == 0 | localLLength() < 2 | localLIndex1() == "0"))
            //    {
            //        // ジャンプによる発進
            //        Map.AreaInSpeed(SelectedTarget, true);
            //    }
            //    else
            //    {
            //        // 通常移動による発進
            //        Map.AreaInSpeed(SelectedTarget);
            //    }

            //    // 母艦を中央表示
            //    GUI.Center(withBlock2.x, withBlock2.y);

            //    // 発進させるユニットを母艦の代わりに表示
            //    if (withBlock2.BitmapID == 0)
            //    {
            //        object argIndex3 = withBlock2.Name;
            //        {
            //            var withBlock3 = SRC.UList.Item(argIndex3);
            //            if ((SelectedTarget.Party0 ?? "") == (withBlock3.Party0 ?? "") & withBlock3.BitmapID != 0 & (SelectedTarget.get_Bitmap(false) ?? "") == (withBlock3.get_Bitmap(false) ?? ""))
            //            {
            //                SelectedTarget.BitmapID = withBlock3.BitmapID;
            //            }
            //            else
            //            {
            //                SelectedTarget.BitmapID = GUI.MakeUnitBitmap(SelectedTarget);
            //            }
            //        }

            //        withBlock2.Name = Conversions.ToString(argIndex3);
            //    }

            //    GUI.MaskScreen();
            //}

            //GUI.ListItemID = new string[1];
            //if (CommandState == "コマンド選択")
            //{
            //    CommandState = "ターゲット選択";
            //}
        }

        // 「発進」コマンドを終了
        private void FinishLaunchCommand()
        {
            throw new NotImplementedException();
            //// MOD END MARGE
            //int ret;
            //GUI.LockGUI();
            //{
            //    var withBlock = SelectedTarget;
            //    // 発進コマンドの目的地にユニットがいた場合
            //    if (Map.MapDataForUnit[SelectedX, SelectedY] is object)
            //    {
            //        string argfname = "母艦";
            //        if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable(argfname))
            //        {
            //            ret = Interaction.MsgBox("着艦しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "着艦");
            //        }
            //        else
            //        {
            //            ret = Interaction.MsgBox("合体しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "合体");
            //        }

            //        if (ret == MsgBoxResult.Cancel)
            //        {
            //            CancelCommand();
            //            GUI.UnlockGUI();
            //            return;
            //        }
            //    }

            //    // メッセージの表示
            //    string argmain_situation = "発進(" + withBlock.Name + ")";
            //    string argmain_situation1 = "発進";
            //    if (withBlock.IsMessageDefined(argmain_situation))
            //    {
            //        Unit argu1 = null;
            //        Unit argu2 = null;
            //        GUI.OpenMessageForm(u1: argu1, u2: argu2);
            //        string argSituation = "発進(" + withBlock.Name + ")";
            //        string argmsg_mode = "";
            //        withBlock.PilotMessage(argSituation, msg_mode: argmsg_mode);
            //        GUI.CloseMessageForm();
            //    }
            //    else if (withBlock.IsMessageDefined(argmain_situation1))
            //    {
            //        Unit argu11 = null;
            //        Unit argu21 = null;
            //        GUI.OpenMessageForm(u1: argu11, u2: argu21);
            //        string argSituation1 = "発進";
            //        string argmsg_mode1 = "";
            //        withBlock.PilotMessage(argSituation1, msg_mode: argmsg_mode1);
            //        GUI.CloseMessageForm();
            //    }

            //    string argmain_situation2 = "発進";
            //    string argsub_situation = withBlock.Name;
            //    withBlock.SpecialEffect(argmain_situation2, argsub_situation);
            //    withBlock.Name = argsub_situation;
            //    PrevUnitArea = withBlock.Area;
            //    PrevUnitEN = withBlock.EN;
            //    withBlock.Status_Renamed = "出撃";

            //    // 指定した位置に発進したユニットを移動
            //    withBlock.Move(SelectedX, SelectedY);
            //}

            //// 発進したユニットを母艦から降ろす
            //{
            //    var withBlock1 = SelectedUnit;
            //    PrevUnitX = withBlock1.x;
            //    PrevUnitY = withBlock1.y;
            //    withBlock1.UnloadUnit((object)SelectedTarget.ID);

            //    // 母艦の位置には発進したユニットが表示されているので元に戻しておく
            //    Map.MapDataForUnit[withBlock1.x, withBlock1.y] = SelectedUnit;
            //    GUI.PaintUnitBitmap(SelectedUnit);
            //}

            //SelectedUnit = SelectedTarget;
            //{
            //    var withBlock2 = SelectedUnit;
            //    if ((Map.MapDataForUnit[withBlock2.x, withBlock2.y].ID ?? "") != (withBlock2.ID ?? ""))
            //    {
            //        GUI.RedrawScreen();
            //        CommandState = "ユニット選択";
            //        GUI.UnlockGUI();
            //        return;
            //    }
            //}

            //CommandState = "移動後コマンド選択";
            //GUI.UnlockGUI();
            //ProceedCommand();
        }

        // 「命令」コマンドを開始
        private void StartOrderCommand()
        {
            throw new NotImplementedException();
            //// MOD END MARGE
            //string[] list;
            //int i, ret, j;
            //GUI.LockGUI();
            //list = new string[5];
            //GUI.ListItemFlag = new bool[5];

            //// 可能な命令内容一覧を作成
            //list[1] = "自由：自由に行動させる";
            //list[2] = "移動：指定した位置に移動";
            //list[3] = "攻撃：指定した敵を攻撃";
            //list[4] = "護衛：指定したユニットを護衛";
            //if (SelectedUnit.Summoner is object | SelectedUnit.Master is object)
            //{
            //    Array.Resize(list, 6);
            //    Array.Resize(GUI.ListItemFlag, 6);
            //    if (SelectedUnit.Master is object)
            //    {
            //        list[5] = "帰還：主人の所に戻る";
            //    }
            //    else
            //    {
            //        list[5] = "帰還：召喚主の所に戻る";
            //    }
            //}

            //// 命令する行動パターンを選択
            //string arglb_caption = "命令";
            //string arglb_info = "行動パターン";
            //string arglb_mode = "";
            //ret = GUI.ListBox(arglb_caption, list, arglb_info, lb_mode: arglb_mode);

            //// 選択された行動パターンに応じてターゲット領域を表示
            //switch (ret)
            //{
            //    case 0:
            //        {
            //            CancelCommand();
            //            break;
            //        }

            //    case 1: // 自由
            //        {
            //            SelectedUnit.Mode = "通常";
            //            CommandState = "ユニット選択";
            //            Status.DisplayUnitStatus(SelectedUnit);
            //            break;
            //        }

            //    case 2: // 移動
            //        {
            //            SelectedCommand = "移動命令";
            //            var loopTo = Map.MapWidth;
            //            for (i = 1; i <= loopTo; i++)
            //            {
            //                var loopTo1 = Map.MapHeight;
            //                for (j = 1; j <= loopTo1; j++)
            //                    Map.MaskData[i, j] = false;
            //            }

            //            GUI.MaskScreen();
            //            CommandState = "ターゲット選択";
            //            break;
            //        }

            //    case 3: // 攻撃
            //        {
            //            SelectedCommand = "攻撃命令";
            //            string arguparty = "味方の敵";
            //            Map.AreaWithUnit(arguparty);
            //            Map.MaskData[SelectedUnit.x, SelectedUnit.y] = true;
            //            GUI.MaskScreen();
            //            CommandState = "ターゲット選択";
            //            break;
            //        }

            //    case 4: // 護衛
            //        {
            //            SelectedCommand = "護衛命令";
            //            string arguparty1 = "味方";
            //            Map.AreaWithUnit(arguparty1);
            //            Map.MaskData[SelectedUnit.x, SelectedUnit.y] = true;
            //            GUI.MaskScreen();
            //            CommandState = "ターゲット選択";
            //            break;
            //        }

            //    case 5: // 帰還
            //        {
            //            if (SelectedUnit.Master is object)
            //            {
            //                SelectedUnit.Mode = SelectedUnit.Master.MainPilot().ID;
            //            }
            //            else
            //            {
            //                SelectedUnit.Mode = SelectedUnit.Summoner.MainPilot().ID;
            //            }

            //            CommandState = "ユニット選択";
            //            Status.DisplayUnitStatus(SelectedUnit);
            //            break;
            //        }
            //}

            //GUI.UnlockGUI();
        }

        // 「命令」コマンドを終了
        private void FinishOrderCommand()
        {
            throw new NotImplementedException();
            //// MOD END MARGE
            //switch (SelectedCommand ?? "")
            //{
            //    case "移動命令":
            //        {
            //            SelectedUnit.Mode = SrcFormatter.Format(SelectedX) + " " + SrcFormatter.Format(SelectedY);
            //            break;
            //        }

            //    case "攻撃命令":
            //    case "護衛命令":
            //        {
            //            SelectedUnit.Mode = SelectedTarget.MainPilot().ID;
            //            break;
            //        }
            //}

            //if (ReferenceEquals(Status.DisplayedUnit, SelectedUnit))
            //{
            //    Status.DisplayUnitStatus(SelectedUnit);
            //}

            //GUI.RedrawScreen();
            //CommandState = "ユニット選択";
        }
    }
}