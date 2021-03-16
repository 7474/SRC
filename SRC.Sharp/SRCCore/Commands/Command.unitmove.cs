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
        // 「移動」コマンドを開始
        private void StartMoveCommand()
        {
            LogDebug();

            SelectedCommand = "移動";
            Map.AreaInSpeed(SelectedUnit);
            //if (!Expression.IsOptionDefined("大型マップ"))
            //{
            GUI.Center(SelectedUnit.x, SelectedUnit.y);
            //}

            GUI.MaskScreen();
            //// MOD START MARGE
            //// If MainWidth <> 15 Then
            //if (GUI.NewGUIMode)
            //{
            //    // MOD END MARGE
            //    Application.DoEvents();
            //    Status.ClearUnitStatus();
            //}

            CommandState = "ターゲット選択";
        }

        // 「移動」コマンドを終了
        private void FinishMoveCommand()
        {
            LogDebug();

            GUI.LockGUI();

            var u = SelectedUnit;
            PrevUnitX = u.x;
            PrevUnitY = u.y;
            PrevUnitArea = u.Area;
            PrevUnitEN = u.EN;

            // 移動後に着艦or合体する場合はプレイヤーに確認を取る
            if (Map.MapDataForUnit[SelectedX, SelectedY] != null)
            {
                GuiDialogResult res;
                if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable("母艦") && !u.IsFeatureAvailable("母艦"))
                {

                    res = GUI.Confirm("着艦しますか？", "着艦", GuiConfirmOption.OkCancel | GuiConfirmOption.Question);
                }
                else
                {
                    res = GUI.Confirm("合体しますか？", "合体", GuiConfirmOption.OkCancel | GuiConfirmOption.Question);
                }

                if (res == GuiDialogResult.Cancel)
                {
                    CancelCommand();
                    GUI.UnlockGUI();
                    return;
                }
            }

            // ユニットを移動
            u.Move(SelectedX, SelectedY);

            // 移動後に着艦または合体した？
            if (!ReferenceEquals(Map.MapDataForUnit[u.x, u.y], SelectedUnit))
            {
                if (Map.MapDataForUnit[u.x, u.y].IsFeatureAvailable("母艦") && !u.IsFeatureAvailable("母艦") && u.CountPilot() > 0)
                {
                    // 着艦メッセージ表示
                    if (u.IsMessageDefined("着艦(" + u.Name + ")"))
                    {
                        GUI.OpenMessageForm(u1: null, u2: null);
                        u.PilotMessage("着艦(" + u.Name + ")", msg_mode: "");
                        GUI.CloseMessageForm();
                    }
                    else if (u.IsMessageDefined("着艦"))
                    {
                        GUI.OpenMessageForm(u1: null, u2: null);
                        u.PilotMessage("着艦", msg_mode: "");
                        GUI.CloseMessageForm();
                    }
                    u.SpecialEffect("着艦", u.Name);

                    // 収納イベント
                    SelectedTarget = Map.MapDataForUnit[u.x, u.y];
                    Event.HandleEvent("収納", u.MainPilot().ID);
                }
                else
                {
                    // 合体後のユニットを選択
                    SelectedUnit = Map.MapDataForUnit[u.x, u.y];

                    // 合体イベント
                    Event.HandleEvent("合体", SelectedUnit.MainPilot().ID, SelectedUnit.Name);
                }

                // 移動後の収納・合体イベントでステージが終了することがあるので
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    GUI.UnlockGUI();
                    return;
                }

                if (SRC.IsCanceled)
                {
                    SRC.IsCanceled = false;
                    Status.ClearUnitStatus();
                    GUI.RedrawScreen();
                    CommandState = "ユニット選択";
                    GUI.UnlockGUI();
                    return;
                }

                // 残り行動数を減少させる
                SelectedUnit.UseAction();

                // 持続期間が「移動」のスペシャルパワー効果を削除
                string argstype = "移動";
                SelectedUnit.RemoveSpecialPowerInEffect(argstype);
                Status.DisplayUnitStatus(SelectedUnit);
                GUI.RedrawScreen();
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            if (SelectedUnitMoveCost > 0)
            {
                // 行動数を減らす
                WaitCommand();
                return;
            }

            SelectedUnitMoveCost = Map.TotalMoveCost[u.x, u.y];

            CommandState = "移動後コマンド選択";
            GUI.UnlockGUI();
            ProceedCommand();
        }

        //// 「テレポート」コマンドを開始
        //private void StartTeleportCommand()
        //{
        //    SelectedCommand = "テレポート";
        //    Map.AreaInTeleport(SelectedUnit);
        //    string argoname = "大型マップ";
        //    if (!Expression.IsOptionDefined(argoname))
        //    {
        //        GUI.Center(SelectedUnit.x, SelectedUnit.y);
        //    }

        //    GUI.MaskScreen();
        //    // MOD START MARGE
        //    // If MainWidth <> 15 Then
        //    if (GUI.NewGUIMode)
        //    {
        //        // MOD END MARGE
        //        Application.DoEvents();
        //        Status.ClearUnitStatus();
        //    }

        //    CommandState = "ターゲット選択";
        //}

        //// 「テレポート」コマンドを終了
        //private void FinishTeleportCommand()
        //{
        //    int ret;
        //    GUI.LockGUI();
        //    {
        //        var withBlock = SelectedUnit;
        //        PrevUnitX = withBlock.x;
        //        PrevUnitY = withBlock.y;
        //        PrevUnitArea = withBlock.Area;
        //        PrevUnitEN = withBlock.EN;

        //        // テレポート後に着艦or合体する場合はプレイヤーに確認を取る
        //        if (Map.MapDataForUnit[SelectedX, SelectedY] is object)
        //        {
        //            string argfname = "母艦";
        //            if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable(argfname))
        //            {
        //                ret = Interaction.MsgBox("着艦しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "着艦");
        //            }
        //            else
        //            {
        //                ret = Interaction.MsgBox("合体しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "合体");
        //            }

        //            if (ret == MsgBoxResult.Cancel)
        //            {
        //                CancelCommand();
        //                GUI.UnlockGUI();
        //                return;
        //            }
        //        }

        //        // メッセージを表示
        //        object argIndex2 = "テレポート";
        //        string argmain_situation = "テレポート(" + withBlock.FeatureName(argIndex2) + ")";
        //        string argmain_situation1 = "テレポート";
        //        if (withBlock.IsMessageDefined(argmain_situation))
        //        {
        //            Unit argu1 = null;
        //            Unit argu2 = null;
        //            GUI.OpenMessageForm(u1: argu1, u2: argu2);
        //            object argIndex1 = "テレポート";
        //            string argSituation = "テレポート(" + withBlock.FeatureName(argIndex1) + ")";
        //            string argmsg_mode = "";
        //            withBlock.PilotMessage(argSituation, msg_mode: argmsg_mode);
        //            GUI.CloseMessageForm();
        //        }
        //        else if (withBlock.IsMessageDefined(argmain_situation1))
        //        {
        //            Unit argu11 = null;
        //            Unit argu21 = null;
        //            GUI.OpenMessageForm(u1: argu11, u2: argu21);
        //            string argSituation1 = "テレポート";
        //            string argmsg_mode1 = "";
        //            withBlock.PilotMessage(argSituation1, msg_mode: argmsg_mode1);
        //            GUI.CloseMessageForm();
        //        }

        //        // アニメ表示
        //        bool localIsSpecialEffectDefined() { string argmain_situation = "テレポート"; object argIndex1 = "テレポート"; string argsub_situation = withBlock.FeatureName(argIndex1); var ret = withBlock.IsSpecialEffectDefined(argmain_situation, argsub_situation); return ret; }

        //        string argmain_situation4 = "テレポート";
        //        object argIndex6 = "テレポート";
        //        string argsub_situation2 = withBlock.FeatureName(argIndex6);
        //        if (withBlock.IsAnimationDefined(argmain_situation4, argsub_situation2))
        //        {
        //            string argmain_situation2 = "テレポート";
        //            object argIndex3 = "テレポート";
        //            string argsub_situation = withBlock.FeatureName(argIndex3);
        //            withBlock.PlayAnimation(argmain_situation2, argsub_situation);
        //        }
        //        else if (localIsSpecialEffectDefined())
        //        {
        //            string argmain_situation3 = "テレポート";
        //            object argIndex4 = "テレポート";
        //            string argsub_situation1 = withBlock.FeatureName(argIndex4);
        //            withBlock.SpecialEffect(argmain_situation3, argsub_situation1);
        //        }
        //        else if (SRC.BattleAnimation)
        //        {
        //            object argIndex5 = "テレポート";
        //            string arganame = "テレポート発動 Whiz.wav " + withBlock.FeatureName0(argIndex5);
        //            Effect.ShowAnimation(arganame);
        //        }
        //        else
        //        {
        //            string argwave_name = "Whiz.wav";
        //            Sound.PlayWave(argwave_name);
        //        }

        //        // ＥＮを消費
        //        object argIndex7 = "テレポート";
        //        string arglist = withBlock.FeatureData(argIndex7);
        //        if (GeneralLib.LLength(arglist) == 2)
        //        {
        //            string localLIndex() { object argIndex1 = "テレポート"; string arglist = withBlock.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

        //            string localLIndex1() { object argIndex1 = "テレポート"; string arglist = withBlock.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

        //            withBlock.EN = PrevUnitEN - Conversions.Toint(localLIndex1());
        //        }
        //        else
        //        {
        //            withBlock.EN = PrevUnitEN - 40;
        //        }

        //        // ユニットを移動
        //        withBlock.Move(SelectedX, SelectedY, true, false, true);
        //        GUI.RedrawScreen();

        //        // 移動後に着艦または合体した？
        //        if (!ReferenceEquals(Map.MapDataForUnit[SelectedX, SelectedY], SelectedUnit))
        //        {
        //            string argfname1 = "母艦";
        //            string argfname2 = "母艦";
        //            if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable(argfname1) & !withBlock.IsFeatureAvailable(argfname2) & withBlock.CountPilot() > 0)
        //            {
        //                // 着艦メッセージ表示
        //                string argmain_situation5 = "着艦(" + withBlock.Name + ")";
        //                string argmain_situation6 = "着艦";
        //                if (withBlock.IsMessageDefined(argmain_situation5))
        //                {
        //                    Unit argu12 = null;
        //                    Unit argu22 = null;
        //                    GUI.OpenMessageForm(u1: argu12, u2: argu22);
        //                    string argSituation2 = "着艦(" + withBlock.Name + ")";
        //                    string argmsg_mode2 = "";
        //                    withBlock.PilotMessage(argSituation2, msg_mode: argmsg_mode2);
        //                    GUI.CloseMessageForm();
        //                }
        //                else if (withBlock.IsMessageDefined(argmain_situation6))
        //                {
        //                    Unit argu13 = null;
        //                    Unit argu23 = null;
        //                    GUI.OpenMessageForm(u1: argu13, u2: argu23);
        //                    string argSituation3 = "着艦";
        //                    string argmsg_mode3 = "";
        //                    withBlock.PilotMessage(argSituation3, msg_mode: argmsg_mode3);
        //                    GUI.CloseMessageForm();
        //                }

        //                string argmain_situation7 = "着艦";
        //                string argsub_situation3 = withBlock.Name;
        //                withBlock.SpecialEffect(argmain_situation7, argsub_situation3);
        //                withBlock.Name = argsub_situation3;

        //                // 収納イベント
        //                SelectedTarget = Map.MapDataForUnit[SelectedX, SelectedY];
        //                Event.HandleEvent("収納", withBlock.MainPilot().ID);
        //            }
        //            else
        //            {
        //                // 合体後のユニットを選択
        //                SelectedUnit = Map.MapDataForUnit[SelectedX, SelectedY];

        //                // 合体イベント
        //                Event.HandleEvent("合体", SelectedUnit.MainPilot().ID, SelectedUnit.Name);
        //            }

        //            // 移動後の収納・合体イベントでステージが終了することがあるので
        //            if (SRC.IsScenarioFinished)
        //            {
        //                SRC.IsScenarioFinished = false;
        //                GUI.UnlockGUI();
        //                return;
        //            }

        //            if (SRC.IsCanceled)
        //            {
        //                SRC.IsCanceled = false;
        //                Status.ClearUnitStatus();
        //                GUI.RedrawScreen();
        //                CommandState = "ユニット選択";
        //                GUI.UnlockGUI();
        //                return;
        //            }

        //            // 残り行動数を減少させる
        //            SelectedUnit.UseAction();

        //            // 持続期間が「移動」のスペシャルパワー効果を削除
        //            string argstype = "移動";
        //            SelectedUnit.RemoveSpecialPowerInEffect(argstype);
        //            Status.DisplayUnitStatus(Map.MapDataForUnit[SelectedX, SelectedY]);
        //            GUI.RedrawScreen();
        //            CommandState = "ユニット選択";
        //            GUI.UnlockGUI();
        //            return;
        //        }
        //        // ADD START MARGE
        //        SelectedUnitMoveCost = 100;
        //        // ADD END MARGE
        //    }

        //    CommandState = "移動後コマンド選択";
        //    GUI.UnlockGUI();
        //    ProceedCommand();
        //}


        //// 「ジャンプ」コマンドを開始
        //private void StartJumpCommand()
        //{
        //    SelectedCommand = "ジャンプ";
        //    Map.AreaInSpeed(SelectedUnit, true);
        //    string argoname = "大型マップ";
        //    if (!Expression.IsOptionDefined(argoname))
        //    {
        //        GUI.Center(SelectedUnit.x, SelectedUnit.y);
        //    }

        //    GUI.MaskScreen();
        //    // MOD START MARGE
        //    // If MainWidth <> 15 Then
        //    if (GUI.NewGUIMode)
        //    {
        //        // MOD END MARGE
        //        Application.DoEvents();
        //        Status.ClearUnitStatus();
        //    }

        //    CommandState = "ターゲット選択";
        //}

        //// 「ジャンプ」コマンドを終了
        //// MOD START MARGE
        //// Public Sub FinishJumpCommand()
        //private void FinishJumpCommand()
        //{
        //    // MOD END MARGE
        //    int ret;
        //    GUI.LockGUI();
        //    {
        //        var withBlock = SelectedUnit;
        //        PrevUnitX = withBlock.x;
        //        PrevUnitY = withBlock.y;
        //        PrevUnitArea = withBlock.Area;
        //        PrevUnitEN = withBlock.EN;

        //        // ジャンプ後に着艦or合体する場合はプレイヤーに確認を取る
        //        if (Map.MapDataForUnit[SelectedX, SelectedY] is object)
        //        {
        //            string argfname = "母艦";
        //            if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable(argfname))
        //            {
        //                ret = Interaction.MsgBox("着艦しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "着艦");
        //            }
        //            else
        //            {
        //                ret = Interaction.MsgBox("合体しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "合体");
        //            }

        //            if (ret == MsgBoxResult.Cancel)
        //            {
        //                CancelCommand();
        //                GUI.UnlockGUI();
        //                return;
        //            }
        //        }

        //        // メッセージを表示
        //        object argIndex2 = "ジャンプ";
        //        string argmain_situation = "ジャンプ(" + withBlock.FeatureName(argIndex2) + ")";
        //        string argmain_situation1 = "ジャンプ";
        //        if (withBlock.IsMessageDefined(argmain_situation))
        //        {
        //            Unit argu1 = null;
        //            Unit argu2 = null;
        //            GUI.OpenMessageForm(u1: argu1, u2: argu2);
        //            object argIndex1 = "ジャンプ";
        //            string argSituation = "ジャンプ(" + withBlock.FeatureName(argIndex1) + ")";
        //            string argmsg_mode = "";
        //            withBlock.PilotMessage(argSituation, msg_mode: argmsg_mode);
        //            GUI.CloseMessageForm();
        //        }
        //        else if (withBlock.IsMessageDefined(argmain_situation1))
        //        {
        //            Unit argu11 = null;
        //            Unit argu21 = null;
        //            GUI.OpenMessageForm(u1: argu11, u2: argu21);
        //            string argSituation1 = "ジャンプ";
        //            string argmsg_mode1 = "";
        //            withBlock.PilotMessage(argSituation1, msg_mode: argmsg_mode1);
        //            GUI.CloseMessageForm();
        //        }

        //        // アニメ表示
        //        bool localIsSpecialEffectDefined() { string argmain_situation = "ジャンプ"; object argIndex1 = "ジャンプ"; string argsub_situation = withBlock.FeatureName(argIndex1); var ret = withBlock.IsSpecialEffectDefined(argmain_situation, argsub_situation); return ret; }

        //        string argmain_situation4 = "ジャンプ";
        //        object argIndex5 = "ジャンプ";
        //        string argsub_situation2 = withBlock.FeatureName(argIndex5);
        //        if (withBlock.IsAnimationDefined(argmain_situation4, argsub_situation2))
        //        {
        //            string argmain_situation2 = "ジャンプ";
        //            object argIndex3 = "ジャンプ";
        //            string argsub_situation = withBlock.FeatureName(argIndex3);
        //            withBlock.PlayAnimation(argmain_situation2, argsub_situation);
        //        }
        //        else if (localIsSpecialEffectDefined())
        //        {
        //            string argmain_situation3 = "ジャンプ";
        //            object argIndex4 = "ジャンプ";
        //            string argsub_situation1 = withBlock.FeatureName(argIndex4);
        //            withBlock.SpecialEffect(argmain_situation3, argsub_situation1);
        //        }
        //        else
        //        {
        //            string argwave_name = "Swing.wav";
        //            Sound.PlayWave(argwave_name);
        //        }

        //        // ＥＮを消費
        //        object argIndex6 = "ジャンプ";
        //        string arglist = withBlock.FeatureData(argIndex6);
        //        if (GeneralLib.LLength(arglist) == 2)
        //        {
        //            string localLIndex() { object argIndex1 = "ジャンプ"; string arglist = withBlock.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

        //            string localLIndex1() { object argIndex1 = "ジャンプ"; string arglist = withBlock.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

        //            withBlock.EN = PrevUnitEN - Conversions.Toint(localLIndex1());
        //        }

        //        // ユニットを移動
        //        withBlock.Move(SelectedX, SelectedY, true, false, true);
        //        GUI.RedrawScreen();

        //        // 移動後に着艦または合体した？
        //        if (!ReferenceEquals(Map.MapDataForUnit[SelectedX, SelectedY], SelectedUnit))
        //        {
        //            string argfname1 = "母艦";
        //            string argfname2 = "母艦";
        //            if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable(argfname1) & !withBlock.IsFeatureAvailable(argfname2) & withBlock.CountPilot() > 0)
        //            {
        //                // 着艦メッセージ表示
        //                string argmain_situation5 = "着艦(" + withBlock.Name + ")";
        //                string argmain_situation6 = "着艦";
        //                if (withBlock.IsMessageDefined(argmain_situation5))
        //                {
        //                    Unit argu12 = null;
        //                    Unit argu22 = null;
        //                    GUI.OpenMessageForm(u1: argu12, u2: argu22);
        //                    string argSituation2 = "着艦(" + withBlock.Name + ")";
        //                    string argmsg_mode2 = "";
        //                    withBlock.PilotMessage(argSituation2, msg_mode: argmsg_mode2);
        //                    GUI.CloseMessageForm();
        //                }
        //                else if (withBlock.IsMessageDefined(argmain_situation6))
        //                {
        //                    Unit argu13 = null;
        //                    Unit argu23 = null;
        //                    GUI.OpenMessageForm(u1: argu13, u2: argu23);
        //                    string argSituation3 = "着艦";
        //                    string argmsg_mode3 = "";
        //                    withBlock.PilotMessage(argSituation3, msg_mode: argmsg_mode3);
        //                    GUI.CloseMessageForm();
        //                }

        //                string argmain_situation7 = "着艦";
        //                string argsub_situation3 = withBlock.Name;
        //                withBlock.SpecialEffect(argmain_situation7, argsub_situation3);
        //                withBlock.Name = argsub_situation3;

        //                // 収納イベント
        //                SelectedTarget = Map.MapDataForUnit[SelectedX, SelectedY];
        //                Event.HandleEvent("収納", withBlock.MainPilot().ID);
        //            }
        //            else
        //            {
        //                // 合体後のユニットを選択
        //                SelectedUnit = Map.MapDataForUnit[SelectedX, SelectedY];

        //                // 合体イベント
        //                Event.HandleEvent("合体", SelectedUnit.MainPilot().ID, SelectedUnit.Name);
        //            }

        //            // 移動後の収納・合体イベントでステージが終了することがあるので
        //            if (SRC.IsScenarioFinished)
        //            {
        //                SRC.IsScenarioFinished = false;
        //                GUI.UnlockGUI();
        //                return;
        //            }

        //            if (SRC.IsCanceled)
        //            {
        //                SRC.IsCanceled = false;
        //                Status.ClearUnitStatus();
        //                GUI.RedrawScreen();
        //                CommandState = "ユニット選択";
        //                GUI.UnlockGUI();
        //                return;
        //            }

        //            // 残り行動数を減少させる
        //            SelectedUnit.UseAction();

        //            // 持続期間が「移動」のスペシャルパワー効果を削除
        //            string argstype = "移動";
        //            SelectedUnit.RemoveSpecialPowerInEffect(argstype);
        //            Status.DisplayUnitStatus(Map.MapDataForUnit[SelectedX, SelectedY]);
        //            GUI.RedrawScreen();
        //            CommandState = "ユニット選択";
        //            GUI.UnlockGUI();
        //            return;
        //        }
        //        // ADD START MARGE
        //        SelectedUnitMoveCost = 100;
        //        // ADD END MARGE
        //    }

        //    CommandState = "移動後コマンド選択";
        //    GUI.UnlockGUI();
        //    ProceedCommand();
        //}
    }
}