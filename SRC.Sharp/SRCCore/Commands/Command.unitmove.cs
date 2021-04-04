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
                SelectedUnit.RemoveSpecialPowerInEffect("移動");
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

        // 「テレポート」コマンドを開始
        private void StartTeleportCommand()
        {
            SelectedCommand = "テレポート";
            Map.AreaInTeleport(SelectedUnit);
            if (!Expression.IsOptionDefined("大型マップ"))
            {
                GUI.Center(SelectedUnit.x, SelectedUnit.y);
            }

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
        //            if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable("母艦"))
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
        //        if (withBlock.IsMessageDefined("テレポート(" + withBlock.FeatureName(argIndex2) + ")"))
        //        {
        //            GUI.OpenMessageForm(u1: null, u2: null);
        //            withBlock.PilotMessage("テレポート(" + withBlock.FeatureName("テレポート") + ")", msg_mode: "");
        //            GUI.CloseMessageForm();
        //        }
        //        else if (withBlock.IsMessageDefined("テレポート"))
        //        {
        //            GUI.OpenMessageForm(u1: null, u2: null);
        //            withBlock.PilotMessage("テレポート", msg_mode: "");
        //            GUI.CloseMessageForm();
        //        }

        //        // アニメ表示
        //        bool localIsSpecialEffectDefined() { string argmain_situation = "テレポート"; object argIndex1 = "テレポート"; string argsub_situation = withBlock.FeatureName(argIndex1); var ret = withBlock.IsSpecialEffectDefined(argmain_situation, argsub_situation); return ret; }

        //        if (withBlock.IsAnimationDefined("テレポート", withBlock.FeatureName(argIndex6)))
        //        {
        //            withBlock.PlayAnimation("テレポート", withBlock.FeatureName("テレポート"));
        //        }
        //        else if (localIsSpecialEffectDefined())
        //        {
        //            withBlock.SpecialEffect("テレポート", withBlock.FeatureName("テレポート"));
        //        }
        //        else if (SRC.BattleAnimation)
        //        {
        //            Effect.ShowAnimation("テレポート発動 Whiz.wav " + withBlock.FeatureName0("テレポート"));
        //        }
        //        else
        //        {
        //            Sound.PlayWave("Whiz.wav");
        //        }

        //        // ＥＮを消費
        //        if (GeneralLib.LLength(withBlock.FeatureData("テレポート")) == 2)
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
        //            if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable("母艦") & !withBlock.IsFeatureAvailable("母艦") & withBlock.CountPilot() > 0)
        //            {
        //                // 着艦メッセージ表示
        //                if (withBlock.IsMessageDefined("着艦(" + withBlock.Name + ")"))
        //                {
        //                    GUI.OpenMessageForm(u1: null, u2: null);
        //                    withBlock.PilotMessage("着艦(" + withBlock.Name + ")", msg_mode: "");
        //                    GUI.CloseMessageForm();
        //                }
        //                else if (withBlock.IsMessageDefined("着艦"))
        //                {
        //                    GUI.OpenMessageForm(u1: null, u2: null);
        //                    withBlock.PilotMessage("着艦", msg_mode: "");
        //                    GUI.CloseMessageForm();
        //                }

        //                withBlock.SpecialEffect("着艦", withBlock.Name);
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
        //            SelectedUnit.RemoveSpecialPowerInEffect("移動");
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


        // 「ジャンプ」コマンドを開始
        private void StartJumpCommand()
        {
            SelectedCommand = "ジャンプ";
            Map.AreaInSpeed(SelectedUnit, true);
            if (!Expression.IsOptionDefined("大型マップ"))
            {
                GUI.Center(SelectedUnit.x, SelectedUnit.y);
            }

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
        //            if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable("母艦"))
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
        //        if (withBlock.IsMessageDefined("ジャンプ(" + withBlock.FeatureName(argIndex2) + ")"))
        //        {
        //            GUI.OpenMessageForm(u1: null, u2: null);
        //            withBlock.PilotMessage("ジャンプ(" + withBlock.FeatureName("ジャンプ") + ")", msg_mode: "");
        //            GUI.CloseMessageForm();
        //        }
        //        else if (withBlock.IsMessageDefined("ジャンプ"))
        //        {
        //            GUI.OpenMessageForm(u1: null, u2: null);
        //            withBlock.PilotMessage("ジャンプ", msg_mode: "");
        //            GUI.CloseMessageForm();
        //        }

        //        // アニメ表示
        //        bool localIsSpecialEffectDefined() { string argmain_situation = "ジャンプ"; object argIndex1 = "ジャンプ"; string argsub_situation = withBlock.FeatureName(argIndex1); var ret = withBlock.IsSpecialEffectDefined(argmain_situation, argsub_situation); return ret; }

        //        if (withBlock.IsAnimationDefined("ジャンプ", withBlock.FeatureName(argIndex5)))
        //        {
        //            withBlock.PlayAnimation("ジャンプ", withBlock.FeatureName("ジャンプ"));
        //        }
        //        else if (localIsSpecialEffectDefined())
        //        {
        //            withBlock.SpecialEffect("ジャンプ", withBlock.FeatureName("ジャンプ"));
        //        }
        //        else
        //        {
        //            Sound.PlayWave("Swing.wav");
        //        }

        //        // ＥＮを消費
        //        if (GeneralLib.LLength(withBlock.FeatureData(argIndex6)) == 2)
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
        //            if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable("母艦") & !withBlock.IsFeatureAvailable("母艦") & withBlock.CountPilot() > 0)
        //            {
        //                // 着艦メッセージ表示
        //                if (withBlock.IsMessageDefined("着艦(" + withBlock.Name + ")"))
        //                {
        //                    GUI.OpenMessageForm(u1: null, u2: null);
        //                    withBlock.PilotMessage("着艦(" + withBlock.Name + ")", msg_mode: "");
        //                    GUI.CloseMessageForm();
        //                }
        //                else if (withBlock.IsMessageDefined("着艦"))
        //                {
        //                    GUI.OpenMessageForm(u1: null, u2: null);
        //                    withBlock.PilotMessage("着艦", msg_mode: "");
        //                    GUI.CloseMessageForm();
        //                }

        //                withBlock.SpecialEffect("着艦", withBlock.Name);
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
        //            SelectedUnit.RemoveSpecialPowerInEffect("移動");
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
