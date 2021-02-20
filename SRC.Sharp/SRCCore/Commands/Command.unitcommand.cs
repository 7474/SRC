// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

namespace SRCCore.Commands
{
    public partial class Command
    {
        // ユニットコマンドを実行
        public void UnitCommand(int idx)
        {
            int prev_used_action;
            PrevCommand = SelectedCommand;
            {
                var withBlock = SelectedUnit;
                prev_used_action = withBlock.UsedAction;
                switch (idx)
                {
                    case MoveCmdID: // 移動
                        {
                            // なんらかの原因により、ユニットコマンドの選択がうまくいかなかった場合は
                            // 移動後のコマンド選択をやり直す
                            if (CommandState == "移動後コマンド選択")
                            {
                                Application.DoEvents();
                                return;
                            }

                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            if (GUI.MainForm.mnuUnitCommandItem.Item(MoveCmdID).Caption == "移動")
                            {
                                StartMoveCommand();
                            }
                            else
                            {
                                ShowAreaInSpeedCommand();
                            }

                            break;
                        }

                    case TeleportCmdID: // テレポート
                        {
                            StartTeleportCommand();
                            break;
                        }

                    case JumpCmdID: // ジャンプ
                        {
                            StartJumpCommand();
                            break;
                        }

                    case TalkCmdID: // 会話
                        {
                            StartTalkCommand();
                            break;
                        }

                    case AttackCmdID: // 攻撃
                        {
                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            if (GUI.MainForm.mnuUnitCommandItem.Item(AttackCmdID).Caption == "攻撃")
                            {
                                StartAttackCommand();
                            }
                            else
                            {
                                ShowAreaInRangeCommand();
                            }

                            break;
                        }

                    case FixCmdID: // 修理
                        {
                            StartFixCommand();
                            break;
                        }

                    case SupplyCmdID: // 補給
                        {
                            StartSupplyCommand();
                            break;
                        }

                    case AbilityCmdID: // アビリティ
                        {
                            StartAbilityCommand();
                            break;
                        }

                    case ChargeCmdID: // チャージ
                        {
                            ChargeCommand();
                            break;
                        }

                    case SpecialPowerCmdID: // 精神
                        {
                            StartSpecialPowerCommand();
                            break;
                        }

                    case TransformCmdID: // 変形
                        {
                            TransformCommand();
                            break;
                        }

                    case SplitCmdID: // 分離
                        {
                            SplitCommand();
                            break;
                        }

                    case CombineCmdID: // 合体
                        {
                            CombineCommand();
                            break;
                        }

                    case HyperModeCmdID: // ハイパーモード・変身解除
                        {
                            object argIndex1 = "ノーマルモード";
                            if (Strings.InStr(withBlock.FeatureData(ref argIndex1), "手動解除") > 0)
                            {
                                CancelTransformationCommand();
                            }
                            else
                            {
                                HyperModeCommand();
                            }

                            break;
                        }

                    case GroundCmdID: // 地上
                        {
                            GUI.LockGUI();
                            if (Map.TerrainClass(withBlock.x, withBlock.y) == "水" | Map.TerrainClass(withBlock.x, withBlock.y) == "深水")
                            {
                                withBlock.Area = "水上";
                            }
                            else
                            {
                                withBlock.Area = "地上";
                            }

                            withBlock.Update();
                            if (withBlock.IsMessageDefined(ref withBlock.Area))
                            {
                                Unit argu1 = null;
                                Unit argu2 = null;
                                GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                                string argmsg_mode = "";
                                withBlock.PilotMessage(ref withBlock.Area, msg_mode: ref argmsg_mode);
                                GUI.CloseMessageForm();
                            }

                            GUI.PaintUnitBitmap(ref SelectedUnit);
                            CommandState = "ユニット選択";
                            GUI.UnlockGUI();
                            break;
                        }

                    case SkyCmdID: // 空中
                        {
                            GUI.LockGUI();
                            if (Map.TerrainClass(withBlock.x, withBlock.y) == "月面")
                            {
                                withBlock.Area = "宇宙";
                            }
                            else
                            {
                                withBlock.Area = "空中";
                            }

                            withBlock.Update();
                            if (withBlock.IsMessageDefined(ref withBlock.Area))
                            {
                                Unit argu11 = null;
                                Unit argu21 = null;
                                GUI.OpenMessageForm(u1: ref argu11, u2: ref argu21);
                                string argmsg_mode1 = "";
                                withBlock.PilotMessage(ref withBlock.Area, msg_mode: ref argmsg_mode1);
                                GUI.CloseMessageForm();
                            }

                            GUI.PaintUnitBitmap(ref SelectedUnit);
                            CommandState = "ユニット選択";
                            GUI.UnlockGUI();
                            break;
                        }

                    case UndergroundCmdID: // 地中
                        {
                            GUI.LockGUI();
                            withBlock.Area = "地中";
                            withBlock.Update();
                            if (withBlock.IsMessageDefined(ref withBlock.Area))
                            {
                                Unit argu12 = null;
                                Unit argu22 = null;
                                GUI.OpenMessageForm(u1: ref argu12, u2: ref argu22);
                                string argmsg_mode2 = "";
                                withBlock.PilotMessage(ref withBlock.Area, msg_mode: ref argmsg_mode2);
                                GUI.CloseMessageForm();
                            }

                            GUI.PaintUnitBitmap(ref SelectedUnit);
                            CommandState = "ユニット選択";
                            GUI.UnlockGUI();
                            break;
                        }

                    case WaterCmdID: // 水中
                        {
                            GUI.LockGUI();
                            withBlock.Area = "水中";
                            withBlock.Update();
                            if (withBlock.IsMessageDefined(ref withBlock.Area))
                            {
                                Unit argu13 = null;
                                Unit argu23 = null;
                                GUI.OpenMessageForm(u1: ref argu13, u2: ref argu23);
                                string argmsg_mode3 = "";
                                withBlock.PilotMessage(ref withBlock.Area, msg_mode: ref argmsg_mode3);
                                GUI.CloseMessageForm();
                            }

                            GUI.PaintUnitBitmap(ref SelectedUnit);
                            CommandState = "ユニット選択";
                            GUI.UnlockGUI();
                            break;
                        }

                    case LaunchCmdID: // 発進
                        {
                            StartLaunchCommand();
                            break;
                        }

                    case ItemCmdID: // アイテム
                        {
                            StartAbilityCommand(true);
                            break;
                        }

                    case DismissCmdID: // 召喚解除
                        {
                            GUI.LockGUI();

                            // 召喚解除の使用イベント
                            Event_Renamed.HandleEvent("使用", withBlock.MainPilot().ID, "召喚解除");
                            if (SRC.IsScenarioFinished)
                            {
                                SRC.IsScenarioFinished = false;
                                GUI.UnlockGUI();
                                return;
                            }

                            if (SRC.IsCanceled)
                            {
                                SRC.IsCanceled = false;
                                return;
                            }

                            // 召喚ユニットを解放
                            withBlock.DismissServant();

                            // 召喚解除の使用後イベント
                            Event_Renamed.HandleEvent("使用後", withBlock.CurrentForm().MainPilot().ID, "召喚解除");
                            if (SRC.IsScenarioFinished)
                            {
                                SRC.IsScenarioFinished = false;
                            }

                            if (SRC.IsCanceled)
                            {
                                SRC.IsCanceled = false;
                            }

                            GUI.UnlockGUI();
                            break;
                        }

                    case OrderCmdID: // 命令/換装
                        {
                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            if (GUI.MainForm.mnuUnitCommandItem.Item(OrderCmdID).Caption == "命令")
                            {
                                StartOrderCommand();
                            }
                            else
                            {
                                ExchangeFormCommand();
                            }

                            break;
                        }

                    case FeatureListCmdID: // 特殊能力一覧
                        {
                            FeatureListCommand();
                            break;
                        }

                    case WeaponListCmdID: // 武器一覧
                        {
                            WeaponListCommand();
                            break;
                        }

                    case AbilityListCmdID: // アビリティ一覧
                        {
                            AbilityListCommand();
                            break;
                        }

                    case var @case when UnitCommand1CmdID <= @case && @case <= UnitCommand10CmdID: // ユニットコマンド
                        {
                            GUI.LockGUI();

                            // ユニットコマンドの使用イベント
                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            Event_Renamed.HandleEvent("使用", withBlock.MainPilot().ID, GUI.MainForm.mnuUnitCommandItem.Item(idx).Caption);
                            if (SRC.IsScenarioFinished)
                            {
                                SRC.IsScenarioFinished = false;
                                GUI.UnlockGUI();
                                return;
                            }

                            if (SRC.IsCanceled)
                            {
                                SRC.IsCanceled = false;
                                WaitCommand();
                                return;
                            }

                            // ユニットコマンドを実行
                            Event_Renamed.HandleEvent(UnitCommandLabelList[idx - UnitCommand1CmdID + 1]);
                            if (SRC.IsCanceled)
                            {
                                SRC.IsCanceled = false;
                                CancelCommand();
                                GUI.UnlockGUI();
                                return;
                            }

                            // ユニットコマンドの使用後イベント
                            if (withBlock.CurrentForm().CountPilot() > 0)
                            {
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                Event_Renamed.HandleEvent("使用後", withBlock.CurrentForm().MainPilot().ID, GUI.MainForm.mnuUnitCommandItem.Item(idx).Caption);
                                if (SRC.IsScenarioFinished)
                                {
                                    SRC.IsScenarioFinished = false;
                                    GUI.UnlockGUI();
                                    return;
                                }
                            }

                            // ステータスウィンドウを更新
                            if (withBlock.CurrentForm().CountPilot() > 0)
                            {
                                Status.DisplayUnitStatus(ref withBlock.CurrentForm());
                            }

                            // 行動終了
                            if (withBlock.CurrentForm().UsedAction <= prev_used_action)
                            {
                                if (CommandState == "移動後コマンド選択")
                                {
                                    WaitCommand();
                                }
                                else
                                {
                                    CommandState = "ユニット選択";
                                    GUI.UnlockGUI();
                                }
                            }
                            else if (SRC.IsCanceled)
                            {
                                SRC.IsCanceled = false;
                            }
                            else
                            {
                                WaitCommand(true);
                            }

                            break;
                        }

                    case WaitCmdID: // 待機
                        {
                            WaitCommand();
                            break;
                        }

                    default:
                        {
                            // なんらかの原因により、ユニットコマンドの選択がうまくいかなかった場合は
                            // 移動後のコマンド選択をやり直す
                            if (CommandState == "移動後コマンド選択")
                            {
                                Application.DoEvents();
                                return;
                            }

                            break;
                        }
                }
            }
        }

        // 「移動」コマンドを開始
        // MOD START MARGE
        // Public Sub StartMoveCommand()
        private void StartMoveCommand()
        {
            // MOD END MARGE
            SelectedCommand = "移動";
            Map.AreaInSpeed(ref SelectedUnit);
            string argoname = "大型マップ";
            if (!Expression.IsOptionDefined(ref argoname))
            {
                GUI.Center(SelectedUnit.x, SelectedUnit.y);
            }

            GUI.MaskScreen();
            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Application.DoEvents();
                Status.ClearUnitStatus();
            }

            CommandState = "ターゲット選択";
        }

        // 「移動」コマンドを終了
        // MOD START MARGE
        // Public Sub FinishMoveCommand()
        private void FinishMoveCommand()
        {
            // MOD END MARGE
            int ret;
            GUI.LockGUI();
            {
                var withBlock = SelectedUnit;
                PrevUnitX = withBlock.x;
                PrevUnitY = withBlock.y;
                PrevUnitArea = withBlock.Area;
                PrevUnitEN = withBlock.EN;

                // 移動後に着艦or合体する場合はプレイヤーに確認を取る
                if (Map.MapDataForUnit[SelectedX, SelectedY] is object)
                {
                    string argfname = "母艦";
                    string argfname1 = "母艦";
                    if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable(ref argfname) & !withBlock.IsFeatureAvailable(ref argfname1))
                    {
                        ret = Interaction.MsgBox("着艦しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "着艦");
                    }
                    else
                    {
                        ret = Interaction.MsgBox("合体しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "合体");
                    }

                    if (ret == MsgBoxResult.Cancel)
                    {
                        CancelCommand();
                        GUI.UnlockGUI();
                        return;
                    }
                }

                // ユニットを移動
                withBlock.Move(SelectedX, SelectedY);

                // 移動後に着艦または合体した？
                if (!ReferenceEquals(Map.MapDataForUnit[withBlock.x, withBlock.y], SelectedUnit))
                {
                    string argfname2 = "母艦";
                    string argfname3 = "母艦";
                    if (Map.MapDataForUnit[withBlock.x, withBlock.y].IsFeatureAvailable(ref argfname2) & !withBlock.IsFeatureAvailable(ref argfname3) & withBlock.CountPilot() > 0)
                    {
                        // 着艦メッセージ表示
                        string argmain_situation = "着艦(" + withBlock.Name + ")";
                        string argmain_situation1 = "着艦";
                        if (withBlock.IsMessageDefined(ref argmain_situation))
                        {
                            Unit argu1 = null;
                            Unit argu2 = null;
                            GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                            string argSituation = "着艦(" + withBlock.Name + ")";
                            string argmsg_mode = "";
                            withBlock.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                            GUI.CloseMessageForm();
                        }
                        else if (withBlock.IsMessageDefined(ref argmain_situation1))
                        {
                            Unit argu11 = null;
                            Unit argu21 = null;
                            GUI.OpenMessageForm(u1: ref argu11, u2: ref argu21);
                            string argSituation1 = "着艦";
                            string argmsg_mode1 = "";
                            withBlock.PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
                            GUI.CloseMessageForm();
                        }

                        string argmain_situation2 = "着艦";
                        string argsub_situation = withBlock.Name;
                        withBlock.SpecialEffect(ref argmain_situation2, ref argsub_situation);
                        withBlock.Name = argsub_situation;

                        // 収納イベント
                        SelectedTarget = Map.MapDataForUnit[withBlock.x, withBlock.y];
                        Event_Renamed.HandleEvent("収納", withBlock.MainPilot().ID);
                    }
                    else
                    {
                        // 合体後のユニットを選択
                        SelectedUnit = Map.MapDataForUnit[withBlock.x, withBlock.y];

                        // 合体イベント
                        Event_Renamed.HandleEvent("合体", SelectedUnit.MainPilot().ID, SelectedUnit.Name);
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
                    SelectedUnit.RemoveSpecialPowerInEffect(ref argstype);
                    Status.DisplayUnitStatus(ref SelectedUnit);
                    GUI.RedrawScreen();
                    CommandState = "ユニット選択";
                    GUI.UnlockGUI();
                    return;
                }
                // ADD START MARGE
                if (SelectedUnitMoveCost > 0)
                {
                    // 行動数を減らす
                    WaitCommand();
                    return;
                }

                SelectedUnitMoveCost = Map.TotalMoveCost[withBlock.x, withBlock.y];
                // ADD END MARGE
            }

            CommandState = "移動後コマンド選択";
            GUI.UnlockGUI();
            ProceedCommand();
        }


        // 「テレポート」コマンドを開始
        // MOD START MARGE
        // Public Sub StartTeleportCommand()
        private void StartTeleportCommand()
        {
            // MOD END MARGE
            SelectedCommand = "テレポート";
            Map.AreaInTeleport(ref SelectedUnit);
            string argoname = "大型マップ";
            if (!Expression.IsOptionDefined(ref argoname))
            {
                GUI.Center(SelectedUnit.x, SelectedUnit.y);
            }

            GUI.MaskScreen();
            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Application.DoEvents();
                Status.ClearUnitStatus();
            }

            CommandState = "ターゲット選択";
        }

        // 「テレポート」コマンドを終了
        // MOD START MARGE
        // Public Sub FinishTeleportCommand()
        private void FinishTeleportCommand()
        {
            // MOD END MARGE
            int ret;
            GUI.LockGUI();
            {
                var withBlock = SelectedUnit;
                PrevUnitX = withBlock.x;
                PrevUnitY = withBlock.y;
                PrevUnitArea = withBlock.Area;
                PrevUnitEN = withBlock.EN;

                // テレポート後に着艦or合体する場合はプレイヤーに確認を取る
                if (Map.MapDataForUnit[SelectedX, SelectedY] is object)
                {
                    string argfname = "母艦";
                    if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable(ref argfname))
                    {
                        ret = Interaction.MsgBox("着艦しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "着艦");
                    }
                    else
                    {
                        ret = Interaction.MsgBox("合体しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "合体");
                    }

                    if (ret == MsgBoxResult.Cancel)
                    {
                        CancelCommand();
                        GUI.UnlockGUI();
                        return;
                    }
                }

                // メッセージを表示
                object argIndex2 = "テレポート";
                string argmain_situation = "テレポート(" + withBlock.FeatureName(ref argIndex2) + ")";
                string argmain_situation1 = "テレポート";
                if (withBlock.IsMessageDefined(ref argmain_situation))
                {
                    Unit argu1 = null;
                    Unit argu2 = null;
                    GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                    object argIndex1 = "テレポート";
                    string argSituation = "テレポート(" + withBlock.FeatureName(ref argIndex1) + ")";
                    string argmsg_mode = "";
                    withBlock.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                    GUI.CloseMessageForm();
                }
                else if (withBlock.IsMessageDefined(ref argmain_situation1))
                {
                    Unit argu11 = null;
                    Unit argu21 = null;
                    GUI.OpenMessageForm(u1: ref argu11, u2: ref argu21);
                    string argSituation1 = "テレポート";
                    string argmsg_mode1 = "";
                    withBlock.PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
                    GUI.CloseMessageForm();
                }

                // アニメ表示
                bool localIsSpecialEffectDefined() { string argmain_situation = "テレポート"; object argIndex1 = "テレポート"; string argsub_situation = withBlock.FeatureName(ref argIndex1); var ret = withBlock.IsSpecialEffectDefined(ref argmain_situation, ref argsub_situation); return ret; }

                string argmain_situation4 = "テレポート";
                object argIndex6 = "テレポート";
                string argsub_situation2 = withBlock.FeatureName(ref argIndex6);
                if (withBlock.IsAnimationDefined(ref argmain_situation4, ref argsub_situation2))
                {
                    string argmain_situation2 = "テレポート";
                    object argIndex3 = "テレポート";
                    string argsub_situation = withBlock.FeatureName(ref argIndex3);
                    withBlock.PlayAnimation(ref argmain_situation2, ref argsub_situation);
                }
                else if (localIsSpecialEffectDefined())
                {
                    string argmain_situation3 = "テレポート";
                    object argIndex4 = "テレポート";
                    string argsub_situation1 = withBlock.FeatureName(ref argIndex4);
                    withBlock.SpecialEffect(ref argmain_situation3, ref argsub_situation1);
                }
                else if (SRC.BattleAnimation)
                {
                    object argIndex5 = "テレポート";
                    string arganame = "テレポート発動 Whiz.wav " + withBlock.FeatureName0(ref argIndex5);
                    Effect.ShowAnimation(ref arganame);
                }
                else
                {
                    string argwave_name = "Whiz.wav";
                    Sound.PlayWave(ref argwave_name);
                }

                // ＥＮを消費
                object argIndex7 = "テレポート";
                string arglist = withBlock.FeatureData(ref argIndex7);
                if (GeneralLib.LLength(ref arglist) == 2)
                {
                    string localLIndex() { object argIndex1 = "テレポート"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    string localLIndex1() { object argIndex1 = "テレポート"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    withBlock.EN = PrevUnitEN - Conversions.Toint(localLIndex1());
                }
                else
                {
                    withBlock.EN = PrevUnitEN - 40;
                }

                // ユニットを移動
                withBlock.Move(SelectedX, SelectedY, true, false, true);
                GUI.RedrawScreen();

                // 移動後に着艦または合体した？
                if (!ReferenceEquals(Map.MapDataForUnit[SelectedX, SelectedY], SelectedUnit))
                {
                    string argfname1 = "母艦";
                    string argfname2 = "母艦";
                    if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable(ref argfname1) & !withBlock.IsFeatureAvailable(ref argfname2) & withBlock.CountPilot() > 0)
                    {
                        // 着艦メッセージ表示
                        string argmain_situation5 = "着艦(" + withBlock.Name + ")";
                        string argmain_situation6 = "着艦";
                        if (withBlock.IsMessageDefined(ref argmain_situation5))
                        {
                            Unit argu12 = null;
                            Unit argu22 = null;
                            GUI.OpenMessageForm(u1: ref argu12, u2: ref argu22);
                            string argSituation2 = "着艦(" + withBlock.Name + ")";
                            string argmsg_mode2 = "";
                            withBlock.PilotMessage(ref argSituation2, msg_mode: ref argmsg_mode2);
                            GUI.CloseMessageForm();
                        }
                        else if (withBlock.IsMessageDefined(ref argmain_situation6))
                        {
                            Unit argu13 = null;
                            Unit argu23 = null;
                            GUI.OpenMessageForm(u1: ref argu13, u2: ref argu23);
                            string argSituation3 = "着艦";
                            string argmsg_mode3 = "";
                            withBlock.PilotMessage(ref argSituation3, msg_mode: ref argmsg_mode3);
                            GUI.CloseMessageForm();
                        }

                        string argmain_situation7 = "着艦";
                        string argsub_situation3 = withBlock.Name;
                        withBlock.SpecialEffect(ref argmain_situation7, ref argsub_situation3);
                        withBlock.Name = argsub_situation3;

                        // 収納イベント
                        SelectedTarget = Map.MapDataForUnit[SelectedX, SelectedY];
                        Event_Renamed.HandleEvent("収納", withBlock.MainPilot().ID);
                    }
                    else
                    {
                        // 合体後のユニットを選択
                        SelectedUnit = Map.MapDataForUnit[SelectedX, SelectedY];

                        // 合体イベント
                        Event_Renamed.HandleEvent("合体", SelectedUnit.MainPilot().ID, SelectedUnit.Name);
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
                    SelectedUnit.RemoveSpecialPowerInEffect(ref argstype);
                    Status.DisplayUnitStatus(ref Map.MapDataForUnit[SelectedX, SelectedY]);
                    GUI.RedrawScreen();
                    CommandState = "ユニット選択";
                    GUI.UnlockGUI();
                    return;
                }
                // ADD START MARGE
                SelectedUnitMoveCost = 100;
                // ADD END MARGE
            }

            CommandState = "移動後コマンド選択";
            GUI.UnlockGUI();
            ProceedCommand();
        }


        // 「ジャンプ」コマンドを開始
        // MOD START MARGE
        // Public Sub StartJumpCommand()
        private void StartJumpCommand()
        {
            // MOD END MARGE
            SelectedCommand = "ジャンプ";
            Map.AreaInSpeed(ref SelectedUnit, true);
            string argoname = "大型マップ";
            if (!Expression.IsOptionDefined(ref argoname))
            {
                GUI.Center(SelectedUnit.x, SelectedUnit.y);
            }

            GUI.MaskScreen();
            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Application.DoEvents();
                Status.ClearUnitStatus();
            }

            CommandState = "ターゲット選択";
        }

        // 「ジャンプ」コマンドを終了
        // MOD START MARGE
        // Public Sub FinishJumpCommand()
        private void FinishJumpCommand()
        {
            // MOD END MARGE
            int ret;
            GUI.LockGUI();
            {
                var withBlock = SelectedUnit;
                PrevUnitX = withBlock.x;
                PrevUnitY = withBlock.y;
                PrevUnitArea = withBlock.Area;
                PrevUnitEN = withBlock.EN;

                // ジャンプ後に着艦or合体する場合はプレイヤーに確認を取る
                if (Map.MapDataForUnit[SelectedX, SelectedY] is object)
                {
                    string argfname = "母艦";
                    if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable(ref argfname))
                    {
                        ret = Interaction.MsgBox("着艦しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "着艦");
                    }
                    else
                    {
                        ret = Interaction.MsgBox("合体しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "合体");
                    }

                    if (ret == MsgBoxResult.Cancel)
                    {
                        CancelCommand();
                        GUI.UnlockGUI();
                        return;
                    }
                }

                // メッセージを表示
                object argIndex2 = "ジャンプ";
                string argmain_situation = "ジャンプ(" + withBlock.FeatureName(ref argIndex2) + ")";
                string argmain_situation1 = "ジャンプ";
                if (withBlock.IsMessageDefined(ref argmain_situation))
                {
                    Unit argu1 = null;
                    Unit argu2 = null;
                    GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                    object argIndex1 = "ジャンプ";
                    string argSituation = "ジャンプ(" + withBlock.FeatureName(ref argIndex1) + ")";
                    string argmsg_mode = "";
                    withBlock.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                    GUI.CloseMessageForm();
                }
                else if (withBlock.IsMessageDefined(ref argmain_situation1))
                {
                    Unit argu11 = null;
                    Unit argu21 = null;
                    GUI.OpenMessageForm(u1: ref argu11, u2: ref argu21);
                    string argSituation1 = "ジャンプ";
                    string argmsg_mode1 = "";
                    withBlock.PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
                    GUI.CloseMessageForm();
                }

                // アニメ表示
                bool localIsSpecialEffectDefined() { string argmain_situation = "ジャンプ"; object argIndex1 = "ジャンプ"; string argsub_situation = withBlock.FeatureName(ref argIndex1); var ret = withBlock.IsSpecialEffectDefined(ref argmain_situation, ref argsub_situation); return ret; }

                string argmain_situation4 = "ジャンプ";
                object argIndex5 = "ジャンプ";
                string argsub_situation2 = withBlock.FeatureName(ref argIndex5);
                if (withBlock.IsAnimationDefined(ref argmain_situation4, ref argsub_situation2))
                {
                    string argmain_situation2 = "ジャンプ";
                    object argIndex3 = "ジャンプ";
                    string argsub_situation = withBlock.FeatureName(ref argIndex3);
                    withBlock.PlayAnimation(ref argmain_situation2, ref argsub_situation);
                }
                else if (localIsSpecialEffectDefined())
                {
                    string argmain_situation3 = "ジャンプ";
                    object argIndex4 = "ジャンプ";
                    string argsub_situation1 = withBlock.FeatureName(ref argIndex4);
                    withBlock.SpecialEffect(ref argmain_situation3, ref argsub_situation1);
                }
                else
                {
                    string argwave_name = "Swing.wav";
                    Sound.PlayWave(ref argwave_name);
                }

                // ＥＮを消費
                object argIndex6 = "ジャンプ";
                string arglist = withBlock.FeatureData(ref argIndex6);
                if (GeneralLib.LLength(ref arglist) == 2)
                {
                    string localLIndex() { object argIndex1 = "ジャンプ"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    string localLIndex1() { object argIndex1 = "ジャンプ"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    withBlock.EN = PrevUnitEN - Conversions.Toint(localLIndex1());
                }

                // ユニットを移動
                withBlock.Move(SelectedX, SelectedY, true, false, true);
                GUI.RedrawScreen();

                // 移動後に着艦または合体した？
                if (!ReferenceEquals(Map.MapDataForUnit[SelectedX, SelectedY], SelectedUnit))
                {
                    string argfname1 = "母艦";
                    string argfname2 = "母艦";
                    if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable(ref argfname1) & !withBlock.IsFeatureAvailable(ref argfname2) & withBlock.CountPilot() > 0)
                    {
                        // 着艦メッセージ表示
                        string argmain_situation5 = "着艦(" + withBlock.Name + ")";
                        string argmain_situation6 = "着艦";
                        if (withBlock.IsMessageDefined(ref argmain_situation5))
                        {
                            Unit argu12 = null;
                            Unit argu22 = null;
                            GUI.OpenMessageForm(u1: ref argu12, u2: ref argu22);
                            string argSituation2 = "着艦(" + withBlock.Name + ")";
                            string argmsg_mode2 = "";
                            withBlock.PilotMessage(ref argSituation2, msg_mode: ref argmsg_mode2);
                            GUI.CloseMessageForm();
                        }
                        else if (withBlock.IsMessageDefined(ref argmain_situation6))
                        {
                            Unit argu13 = null;
                            Unit argu23 = null;
                            GUI.OpenMessageForm(u1: ref argu13, u2: ref argu23);
                            string argSituation3 = "着艦";
                            string argmsg_mode3 = "";
                            withBlock.PilotMessage(ref argSituation3, msg_mode: ref argmsg_mode3);
                            GUI.CloseMessageForm();
                        }

                        string argmain_situation7 = "着艦";
                        string argsub_situation3 = withBlock.Name;
                        withBlock.SpecialEffect(ref argmain_situation7, ref argsub_situation3);
                        withBlock.Name = argsub_situation3;

                        // 収納イベント
                        SelectedTarget = Map.MapDataForUnit[SelectedX, SelectedY];
                        Event_Renamed.HandleEvent("収納", withBlock.MainPilot().ID);
                    }
                    else
                    {
                        // 合体後のユニットを選択
                        SelectedUnit = Map.MapDataForUnit[SelectedX, SelectedY];

                        // 合体イベント
                        Event_Renamed.HandleEvent("合体", SelectedUnit.MainPilot().ID, SelectedUnit.Name);
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
                    SelectedUnit.RemoveSpecialPowerInEffect(ref argstype);
                    Status.DisplayUnitStatus(ref Map.MapDataForUnit[SelectedX, SelectedY]);
                    GUI.RedrawScreen();
                    CommandState = "ユニット選択";
                    GUI.UnlockGUI();
                    return;
                }
                // ADD START MARGE
                SelectedUnitMoveCost = 100;
                // ADD END MARGE
            }

            CommandState = "移動後コマンド選択";
            GUI.UnlockGUI();
            ProceedCommand();
        }


        // 「攻撃」コマンドを開始
        // MOD START MARGE
        // Public Sub StartAttackCommand()
        private void StartAttackCommand()
        {
            // MOD END MARGE
            int i, j;
            Unit t;
            int min_range, max_range;
            var BGM = default(string);
            GUI.LockGUI();
            var partners = default(Unit[]);
            {
                var withBlock = SelectedUnit;
                // ＢＧＭの設定
                string argfname = "ＢＧＭ";
                if (withBlock.IsFeatureAvailable(ref argfname))
                {
                    object argIndex1 = "ＢＧＭ";
                    string argmidi_name = withBlock.FeatureData(ref argIndex1);
                    BGM = Sound.SearchMidiFile(ref argmidi_name);
                }

                if (Strings.Len(BGM) == 0)
                {
                    string argmidi_name1 = withBlock.MainPilot().BGM;
                    BGM = Sound.SearchMidiFile(ref argmidi_name1);
                    withBlock.MainPilot().BGM = argmidi_name1;
                }

                if (Strings.Len(BGM) == 0)
                {
                    string argbgm_name = "default";
                    BGM = Sound.BGMName(ref argbgm_name);
                }

                // 武器の選択
                UseSupportAttack = true;
                if (CommandState == "コマンド選択")
                {
                    string argcaption_msg = "武器選択";
                    string arglb_mode = "移動前";
                    SelectedWeapon = GUI.WeaponListBox(ref SelectedUnit, ref argcaption_msg, ref arglb_mode, ref BGM);
                }
                else
                {
                    string argcaption_msg1 = "武器選択";
                    string arglb_mode1 = "移動後";
                    SelectedWeapon = GUI.WeaponListBox(ref SelectedUnit, ref argcaption_msg1, ref arglb_mode1, ref BGM);
                }

                // キャンセル
                if (SelectedWeapon == 0)
                {
                    if (SRC.AutoMoveCursor)
                    {
                        GUI.RestoreCursorPos();
                    }

                    CancelCommand();
                    GUI.UnlockGUI();
                    return;
                }

                // 武器ＢＧＭの演奏
                string argfname1 = "武器ＢＧＭ";
                if (withBlock.IsFeatureAvailable(ref argfname1))
                {
                    var loopTo = withBlock.CountFeature();
                    for (i = 1; i <= loopTo; i++)
                    {
                        string localFeature() { object argIndex1 = i; var ret = withBlock.Feature(ref argIndex1); return ret; }

                        string localFeatureData2() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                        string localLIndex() { string arglist = hs644c0746afde449eb03788895da90548(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

                        if (localFeature() == "武器ＢＧＭ" & (localLIndex() ?? "") == (withBlock.Weapon(SelectedWeapon).Name ?? ""))
                        {
                            string localFeatureData() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                            string localFeatureData1() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                            string argmidi_name2 = Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1);
                            BGM = Sound.SearchMidiFile(ref argmidi_name2);
                            if (Strings.Len(BGM) > 0)
                            {
                                Sound.ChangeBGM(ref BGM);
                            }

                            break;
                        }
                    }
                }

                // 選択した武器の種類により、この後のコマンドの進行の仕方が異なる
                string argattr = "Ｍ";
                if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr))
                {
                    SelectedCommand = "マップ攻撃";
                }
                else
                {
                    SelectedCommand = "攻撃";
                }

                // 武器の射程を求めておく
                min_range = withBlock.Weapon(SelectedWeapon).MinRange;
                max_range = withBlock.WeaponMaxRange(SelectedWeapon);

                // 攻撃範囲の表示
                string argattr2 = "Ｍ直";
                string argattr3 = "Ｍ拡";
                string argattr4 = "Ｍ扇";
                string argattr5 = "Ｍ全";
                string argattr6 = "Ｍ投";
                string argattr7 = "Ｍ線";
                string argattr8 = "Ｍ移";
                if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr2))
                {
                    Map.AreaInCross(withBlock.x, withBlock.y, min_range, ref max_range);
                }
                else if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr3))
                {
                    Map.AreaInWideCross(withBlock.x, withBlock.y, min_range, ref max_range);
                }
                else if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr4))
                {
                    string argattr1 = "Ｍ扇";
                    Map.AreaInSectorCross(withBlock.x, withBlock.y, min_range, ref max_range, withBlock.WeaponLevel(SelectedWeapon, ref argattr1));
                }
                else if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr5) | withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr6) | withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr7))
                {
                    string arguparty1 = "すべて";
                    Map.AreaInRange(withBlock.x, withBlock.y, max_range, min_range, ref arguparty1);
                }
                else if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr8))
                {
                    Map.AreaInMoveAction(ref SelectedUnit, max_range);
                }
                else
                {
                    string arguparty = "味方の敵";
                    Map.AreaInRange(withBlock.x, withBlock.y, max_range, min_range, ref arguparty);
                }

                // 射程１の合体技はパートナーで相手を取り囲んでいないと使用できない
                string argattr9 = "合";
                string argattr10 = "Ｍ";
                if (max_range == 1 & withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr9) & !withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr10))
                {
                    for (i = 1; i <= 4; i++)
                    {
                        // UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        t = null;
                        switch (i)
                        {
                            case 1:
                                {
                                    if (withBlock.x > 1)
                                    {
                                        t = Map.MapDataForUnit[withBlock.x - 1, withBlock.y];
                                    }

                                    break;
                                }

                            case 2:
                                {
                                    if (withBlock.x < Map.MapWidth)
                                    {
                                        t = Map.MapDataForUnit[withBlock.x + 1, withBlock.y];
                                    }

                                    break;
                                }

                            case 3:
                                {
                                    if (withBlock.y > 1)
                                    {
                                        t = Map.MapDataForUnit[withBlock.x, withBlock.y - 1];
                                    }

                                    break;
                                }

                            case 4:
                                {
                                    if (withBlock.y < Map.MapHeight)
                                    {
                                        t = Map.MapDataForUnit[withBlock.x, withBlock.y + 1];
                                    }

                                    break;
                                }
                        }

                        if (t is object)
                        {
                            if (withBlock.IsEnemy(ref t))
                            {
                                string argctype_Renamed = "武装";
                                withBlock.CombinationPartner(ref argctype_Renamed, SelectedWeapon, ref partners, t.x, t.y);
                                if (Information.UBound(partners) == 0)
                                {
                                    Map.MaskData[t.x, t.y] = true;
                                }
                            }
                        }
                    }
                }

                // ユニットに対するマスクの設定
                string argattr15 = "Ｍ投";
                string argattr16 = "Ｍ線";
                if (!withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr15) & !withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr16))
                {
                    var loopTo1 = GeneralLib.MinLng(withBlock.x + max_range, Map.MapWidth);
                    for (i = GeneralLib.MaxLng(withBlock.x - max_range, 1); i <= loopTo1; i++)
                    {
                        var loopTo2 = GeneralLib.MinLng(withBlock.y + max_range, Map.MapHeight);
                        for (j = GeneralLib.MaxLng(withBlock.y - max_range, 1); j <= loopTo2; j++)
                        {
                            if (Map.MaskData[i, j])
                            {
                                goto NextLoop;
                            }

                            t = Map.MapDataForUnit[i, j];
                            if (t is null)
                            {
                                goto NextLoop;
                            }

                            // 武器の地形適応が有効？
                            if (withBlock.WeaponAdaption(SelectedWeapon, ref t.Area) == 0d)
                            {
                                Map.MaskData[i, j] = true;
                                goto NextLoop;
                            }

                            // 封印武器の対象属性外でない？
                            string argattr11 = "封";
                            if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr11))
                            {
                                if (withBlock.Weapon(SelectedWeapon).Power > 0 & withBlock.Damage(SelectedWeapon, ref t, true) == 0 | withBlock.CriticalProbability(SelectedWeapon, ref t) == 0)
                                {
                                    Map.MaskData[i, j] = true;
                                    goto NextLoop;
                                }
                            }

                            // 限定武器の対象属性外でない？
                            string argattr12 = "限";
                            if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr12))
                            {
                                if (withBlock.Weapon(SelectedWeapon).Power > 0 & withBlock.Damage(SelectedWeapon, ref t, true) == 0 | withBlock.Weapon(SelectedWeapon).Power == 0 & withBlock.CriticalProbability(SelectedWeapon, ref t) == 0)
                                {
                                    Map.MaskData[i, j] = true;
                                    goto NextLoop;
                                }
                            }

                            // 識別攻撃の場合の処理
                            string argattr13 = "識";
                            string argsptype = "識別攻撃";
                            if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr13) | withBlock.IsUnderSpecialPowerEffect(ref argsptype))
                            {
                                if (withBlock.IsAlly(ref t))
                                {
                                    Map.MaskData[i, j] = true;
                                    goto NextLoop;
                                }
                            }

                            // ステルス＆隠れ身チェック
                            string argattr14 = "Ｍ";
                            if (!withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr14))
                            {
                                if (!withBlock.IsTargetWithinRange(SelectedWeapon, ref t))
                                {
                                    Map.MaskData[i, j] = true;
                                    goto NextLoop;
                                }
                            }

                        NextLoop:
                            ;
                        }
                    }
                }

                Map.MaskData[withBlock.x, withBlock.y] = false;
                string argoname = "大型マップ";
                if (!Expression.IsOptionDefined(ref argoname))
                {
                    GUI.Center(withBlock.x, withBlock.y);
                }

                GUI.MaskScreen();
            }

            // ターゲット選択へ
            if (CommandState == "コマンド選択")
            {
                CommandState = "ターゲット選択";
            }
            else
            {
                CommandState = "移動後ターゲット選択";
            }

            // カーソル自動移動を行う？
            if (!SRC.AutoMoveCursor)
            {
                GUI.UnlockGUI();
                return;
            }

            // ＨＰがもっとも低いターゲットを探す
            // UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            t = null;
            foreach (Unit u in SRC.UList)
            {
                if (u.Status_Renamed == "出撃" & (u.Party == "敵" | u.Party == "中立"))
                {
                    if (Map.MaskData[u.x, u.y] == false)
                    {
                        if (t is null)
                        {
                            t = u;
                        }
                        else if (u.HP < t.HP)
                        {
                            t = u;
                        }
                        else if (u.HP == t.HP)
                        {
                            if (Math.Pow(Math.Abs((SelectedUnit.x - u.x)), 2d) + Math.Pow(Math.Abs((SelectedUnit.y - u.y)), 2d) < Math.Pow(Math.Abs((SelectedUnit.x - t.x)), 2d) + Math.Pow(Math.Abs((SelectedUnit.y - t.y)), 2d))
                            {
                                t = u;
                            }
                        }
                    }
                }
            }

            // 適当なターゲットが見つからなければ自分自身を選択
            if (t is null)
            {
                t = SelectedUnit;
            }

            // カーソルを移動
            string argcursor_mode = "ユニット選択";
            GUI.MoveCursorPos(ref argcursor_mode, t);

            // ターゲットのステータスを表示
            if (!ReferenceEquals(SelectedUnit, t))
            {
                Status.DisplayUnitStatus(ref t);
            }

            GUI.UnlockGUI();
        }

        // 「攻撃」コマンドを終了
        // MOD START MARGE
        // Public Sub FinishAttackCommand()
        private void FinishAttackCommand()
        {
            // MOD END MARGE
            int i;
            var earnings = default;
            string def_mode;
            var partners = default(Unit[]);
            var BGM = default(string);
            var is_suiside = default(bool);
            string wname, twname = default;
            int tx, ty;
            Unit attack_target;
            double attack_target_hp_ratio;
            Unit defense_target;
            double defense_target_hp_ratio;
            Unit defense_target2;
            var defense_target2_hp_ratio = default(double);
            var support_attack_done = default(bool);
            int w2;
            // ADD START MARGE
            bool is_p_weapon;
            // ADD END MARGE
            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Status.ClearUnitStatus();
            }

            GUI.LockGUI();
            wname = SelectedUnit.Weapon(SelectedWeapon).Name;
            SelectedWeaponName = wname;

            // ADD START MARGE
            // 移動後使用後可能な武器か記録しておく
            string argattr = "移動後攻撃可";
            is_p_weapon = SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, ref argattr);
            // ADD END MARGE

            // 合体技のパートナーを設定
            {
                var withBlock = SelectedUnit;
                string argattr1 = "合";
                if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr1))
                {
                    if (withBlock.WeaponMaxRange(SelectedWeapon) == 1)
                    {
                        string argctype_Renamed = "武装";
                        withBlock.CombinationPartner(ref argctype_Renamed, SelectedWeapon, ref partners, SelectedTarget.x, SelectedTarget.y);
                    }
                    else
                    {
                        string argctype_Renamed1 = "武装";
                        withBlock.CombinationPartner(ref argctype_Renamed1, SelectedWeapon, ref partners);
                    }
                }
                else
                {
                    SelectedPartners = new Unit[1];
                    partners = new Unit[1];
                }
            }

            // 敵の反撃手段を設定
            UseSupportGuard = true;
            string argamode = "反撃";
            int argmax_prob = 0;
            int argmax_dmg = 0;
            SelectedTWeapon = COM.SelectWeapon(ref SelectedTarget, ref SelectedUnit, ref argamode, max_prob: ref argmax_prob, max_dmg: ref argmax_dmg);
            string argattr2 = "間";
            if (SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, ref argattr2))
            {
                SelectedTWeapon = 0;
            }

            if (SelectedTWeapon > 0)
            {
                twname = SelectedTarget.Weapon(SelectedTWeapon).Name;
                SelectedTWeaponName = twname;
            }
            else
            {
                SelectedTWeaponName = "";
            }

            // 敵の防御行動を設定
            // UPGRADE_WARNING: オブジェクト SelectDefense() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            def_mode = Conversions.ToString(COM.SelectDefense(ref SelectedUnit, ref SelectedWeapon, ref SelectedTarget, ref SelectedTWeapon));
            if (!string.IsNullOrEmpty(def_mode))
            {
                if (SelectedTWeapon > 0)
                {
                    SelectedTWeapon = -1;
                }
            }

            SelectedDefenseOption = def_mode;

            // 戦闘前に一旦クリア
            // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            SupportAttackUnit = null;
            // UPGRADE_NOTE: オブジェクト SupportGuardUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            SupportGuardUnit = null;
            // UPGRADE_NOTE: オブジェクト SupportGuardUnit2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            SupportGuardUnit2 = null;

            // 攻撃側の武器使用イベント
            Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, wname);
            if (SRC.IsScenarioFinished)
            {
                GUI.UnlockGUI();
                SRC.IsScenarioFinished = false;
                SelectedPartners = new Unit[1];
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                WaitCommand();
                return;
            }

            // 敵の武器使用イベント
            if (SelectedTWeapon > 0)
            {
                SaveSelections();
                SwapSelections();
                Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, twname);
                RestoreSelections();
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    SelectedPartners = new Unit[1];
                    GUI.UnlockGUI();
                    return;
                }

                if (SRC.IsCanceled)
                {
                    SRC.IsCanceled = false;
                    SelectedPartners = new Unit[1];
                    WaitCommand();
                    return;
                }
            }

            // 攻撃イベント
            Event_Renamed.HandleEvent("攻撃", SelectedUnit.MainPilot().ID, SelectedTarget.MainPilot().ID);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                SelectedPartners = new Unit[1];
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                SelectedPartners = new Unit[1];
                WaitCommand();
                return;
            }

            // 敵がＢＧＭ能力を持つ場合はＢＧＭを変更
            {
                var withBlock1 = SelectedTarget;
                string argfname = "ＢＧＭ";
                if (withBlock1.IsFeatureAvailable(ref argfname) & Strings.InStr(withBlock1.MainPilot().Name, "(ザコ)") == 0)
                {
                    object argIndex1 = "ＢＧＭ";
                    string argmidi_name = withBlock1.FeatureData(ref argIndex1);
                    BGM = Sound.SearchMidiFile(ref argmidi_name);
                    if (Strings.Len(BGM) > 0)
                    {
                        Sound.BossBGM = false;
                        Sound.ChangeBGM(ref BGM);
                        Sound.BossBGM = true;
                    }
                }
            }

            // そうではなく、ボス用ＢＧＭが流れていれば味方のＢＧＭに切り替え
            if (Strings.Len(BGM) == 0 & Sound.BossBGM)
            {
                Sound.BossBGM = false;
                BGM = "";
                {
                    var withBlock2 = SelectedUnit;
                    string argfname1 = "武器ＢＧＭ";
                    if (withBlock2.IsFeatureAvailable(ref argfname1))
                    {
                        var loopTo = withBlock2.CountFeature();
                        for (i = 1; i <= loopTo; i++)
                        {
                            string localFeature() { object argIndex1 = i; var ret = withBlock2.Feature(ref argIndex1); return ret; }

                            string localFeatureData2() { object argIndex1 = i; var ret = withBlock2.FeatureData(ref argIndex1); return ret; }

                            string localLIndex() { string arglist = hs51a192742c114976b44a246d492333eb(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

                            if (localFeature() == "武器ＢＧＭ" & (localLIndex() ?? "") == (withBlock2.Weapon(SelectedWeapon).Name ?? ""))
                            {
                                string localFeatureData() { object argIndex1 = i; var ret = withBlock2.FeatureData(ref argIndex1); return ret; }

                                string localFeatureData1() { object argIndex1 = i; var ret = withBlock2.FeatureData(ref argIndex1); return ret; }

                                string argmidi_name1 = Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1);
                                BGM = Sound.SearchMidiFile(ref argmidi_name1);
                                break;
                            }
                        }
                    }

                    if (Strings.Len(BGM) == 0)
                    {
                        string argfname2 = "ＢＧＭ";
                        if (withBlock2.IsFeatureAvailable(ref argfname2))
                        {
                            object argIndex2 = "ＢＧＭ";
                            string argmidi_name2 = withBlock2.FeatureData(ref argIndex2);
                            BGM = Sound.SearchMidiFile(ref argmidi_name2);
                        }
                    }

                    if (Strings.Len(BGM) == 0)
                    {
                        string argmidi_name3 = withBlock2.MainPilot().BGM;
                        BGM = Sound.SearchMidiFile(ref argmidi_name3);
                        withBlock2.MainPilot().BGM = argmidi_name3;
                    }

                    if (Strings.Len(BGM) == 0)
                    {
                        string argbgm_name = "default";
                        BGM = Sound.BGMName(ref argbgm_name);
                    }

                    Sound.ChangeBGM(ref BGM);
                }
            }

            {
                var withBlock3 = SelectedUnit;
                // 攻撃参加ユニット以外はマスク
                foreach (Unit u in SRC.UList)
                {
                    if (u.Status_Renamed == "出撃")
                    {
                        Map.MaskData[u.x, u.y] = true;
                    }
                }

                // 合体技パートナーのハイライト表示
                var loopTo1 = Information.UBound(partners);
                for (i = 1; i <= loopTo1; i++)
                {
                    {
                        var withBlock4 = partners[i];
                        Map.MaskData[withBlock4.x, withBlock4.y] = false;
                    }
                }

                Map.MaskData[withBlock3.x, withBlock3.y] = false;
                Map.MaskData[SelectedTarget.x, SelectedTarget.y] = false;
                if (!SRC.BattleAnimation)
                {
                    GUI.MaskScreen();
                }
            }

            // イベント用に戦闘に参加するユニットの情報を記録しておく
            AttackUnit = SelectedUnit;
            attack_target = SelectedUnit;
            attack_target_hp_ratio = SelectedUnit.HP / (double)SelectedUnit.MaxHP;
            defense_target = SelectedTarget;
            defense_target_hp_ratio = SelectedTarget.HP / (double)SelectedTarget.MaxHP;
            // UPGRADE_NOTE: オブジェクト defense_target2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            defense_target2 = null;
            // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            SupportAttackUnit = null;
            // UPGRADE_NOTE: オブジェクト SupportGuardUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            SupportGuardUnit = null;
            // UPGRADE_NOTE: オブジェクト SupportGuardUnit2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            SupportGuardUnit2 = null;

            // ターゲットの位置を記録
            tx = SelectedTarget.x;
            ty = SelectedTarget.y;
            GUI.OpenMessageForm(ref SelectedTarget, ref SelectedUnit);

            // 相手の先制攻撃？
            {
                var withBlock5 = SelectedTarget;
                // MOD START MARGE
                // If SelectedTWeapon > 0 And .MaxAction > 0 And .IsWeaponAvailable(SelectedTWeapon, "移動前") Then
                // SelectedTWeapon > 0の判定は、IsWeaponAvailableで行うようにした
                string argref_mode1 = "移動前";
                if (withBlock5.MaxAction() > 0 & withBlock5.IsWeaponAvailable(SelectedTWeapon, ref argref_mode1))
                {
                    // MOD END MARGE
                    string argattr8 = "後";
                    if (!withBlock5.IsWeaponClassifiedAs(SelectedTWeapon, ref argattr8))
                    {
                        string argattr6 = "後";
                        string argattr7 = "先";
                        object argIndex3 = "先読み";
                        string argref_mode = "";
                        string argsptype = "カウンター";
                        if (SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, ref argattr6))
                        {
                            def_mode = "先制攻撃";
                            string argattr3 = "自";
                            if (withBlock5.IsWeaponClassifiedAs(SelectedTWeapon, ref argattr3))
                            {
                                is_suiside = true;
                            }

                            // 先制攻撃攻撃を実施
                            withBlock5.Attack(SelectedTWeapon, SelectedUnit, "先制攻撃", "");
                            SelectedTarget = withBlock5.CurrentForm();
                        }
                        else if (withBlock5.IsWeaponClassifiedAs(SelectedTWeapon, ref argattr7) | withBlock5.MainPilot().SkillLevel(ref argIndex3, ref_mode: ref argref_mode) >= GeneralLib.Dice(16) | withBlock5.IsUnderSpecialPowerEffect(ref argsptype))
                        {
                            def_mode = "先制攻撃";
                            string argattr4 = "自";
                            if (withBlock5.IsWeaponClassifiedAs(SelectedTWeapon, ref argattr4))
                            {
                                is_suiside = true;
                            }

                            // カウンター攻撃を実施
                            withBlock5.Attack(SelectedTWeapon, SelectedUnit, "カウンター", "");
                            SelectedTarget = withBlock5.CurrentForm();
                        }
                        else if (withBlock5.MaxCounterAttack() > withBlock5.UsedCounterAttack)
                        {
                            def_mode = "先制攻撃";
                            string argattr5 = "自";
                            if (withBlock5.IsWeaponClassifiedAs(SelectedTWeapon, ref argattr5))
                            {
                                is_suiside = true;
                            }

                            // カウンター攻撃の残り回数を減少
                            withBlock5.UsedCounterAttack = (withBlock5.UsedCounterAttack + 1);

                            // カウンター攻撃を実施
                            withBlock5.Attack(SelectedTWeapon, SelectedUnit, "カウンター", "");
                            SelectedTarget = withBlock5.CurrentForm();
                        }
                    }

                    // 攻撃側のユニットがかばわれた場合は攻撃側のターゲットを再設定
                    if (SupportGuardUnit2 is object)
                    {
                        attack_target = SupportGuardUnit2;
                        attack_target_hp_ratio = SupportGuardUnitHPRatio2;
                    }
                }
            }

            // サポートアタックのパートナーを探す
            {
                var withBlock6 = SelectedUnit;
                if (withBlock6.Status_Renamed == "出撃" & SelectedTarget.Status_Renamed == "出撃" & UseSupportAttack)
                {
                    SupportAttackUnit = withBlock6.LookForSupportAttack(ref SelectedTarget);

                    // 合体技ではサポートアタック不能
                    if (0 < SelectedWeapon & SelectedWeapon <= withBlock6.CountWeapon())
                    {
                        string argattr9 = "合";
                        if (withBlock6.IsWeaponClassifiedAs(SelectedWeapon, ref argattr9))
                        {
                            // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                            SupportAttackUnit = null;
                        }
                    }

                    // 魅了された場合
                    object argIndex4 = "魅了";
                    if (withBlock6.IsConditionSatisfied(ref argIndex4) & ReferenceEquals(withBlock6.Master, SelectedTarget))
                    {
                        // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        SupportAttackUnit = null;
                    }

                    // 憑依された場合
                    object argIndex5 = "憑依";
                    if (withBlock6.IsConditionSatisfied(ref argIndex5))
                    {
                        if ((withBlock6.Master.Party ?? "") == (SelectedTarget.Party ?? ""))
                        {
                            // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                            SupportAttackUnit = null;
                        }
                    }

                    // 踊らされた場合
                    object argIndex6 = "踊り";
                    if (withBlock6.IsConditionSatisfied(ref argIndex6))
                    {
                        // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        SupportAttackUnit = null;
                    }
                }
            }

            // 攻撃の実施
            {
                var withBlock7 = SelectedUnit;
                object argIndex10 = "攻撃不能";
                if (withBlock7.Status_Renamed == "出撃" & withBlock7.MaxAction(true) > 0 & !withBlock7.IsConditionSatisfied(ref argIndex10) & SelectedTarget.Status_Renamed == "出撃")
                {
                    // まだ武器は使用可能か？
                    if (SelectedWeapon > withBlock7.CountWeapon())
                    {
                        SelectedWeapon = -1;
                    }
                    else if ((wname ?? "") != (withBlock7.Weapon(SelectedWeapon).Name ?? ""))
                    {
                        SelectedWeapon = -1;
                    }
                    else if (CommandState == "移動後ターゲット選択")
                    {
                        string argref_mode3 = "移動後";
                        if (!withBlock7.IsWeaponAvailable(SelectedWeapon, ref argref_mode3))
                        {
                            SelectedWeapon = -1;
                        }
                    }
                    else
                    {
                        string argref_mode2 = "移動前";
                        if (!withBlock7.IsWeaponAvailable(SelectedWeapon, ref argref_mode2))
                        {
                            SelectedWeapon = -1;
                        }
                    }

                    if (SelectedWeapon > 0)
                    {
                        if (!withBlock7.IsTargetWithinRange(SelectedWeapon, ref SelectedTarget))
                        {
                            SelectedWeapon = 0;
                        }
                    }

                    // 魅了された場合
                    object argIndex7 = "魅了";
                    if (withBlock7.IsConditionSatisfied(ref argIndex7) & ReferenceEquals(withBlock7.Master, SelectedTarget))
                    {
                        SelectedWeapon = -1;
                    }

                    // 憑依された場合
                    object argIndex8 = "憑依";
                    if (withBlock7.IsConditionSatisfied(ref argIndex8))
                    {
                        if ((withBlock7.Master.Party ?? "") == (SelectedTarget.Party0 ?? ""))
                        {
                            SelectedWeapon = -1;
                        }
                    }

                    // 踊らされた場合
                    object argIndex9 = "踊り";
                    if (withBlock7.IsConditionSatisfied(ref argIndex9))
                    {
                        SelectedWeapon = -1;
                    }

                    if (SelectedWeapon > 0)
                    {
                        if (SupportAttackUnit is object & withBlock7.MaxSyncAttack() > withBlock7.UsedSyncAttack)
                        {
                            // 同時援護攻撃
                            withBlock7.Attack(SelectedWeapon, SelectedTarget, "統率", def_mode);
                        }
                        else
                        {
                            // 通常攻撃
                            withBlock7.Attack(SelectedWeapon, SelectedTarget, "", def_mode);
                        }
                    }
                    else if (SelectedWeapon == 0)
                    {
                        string argmain_situation2 = "射程外";
                        string argsub_situation2 = "";
                        if (withBlock7.IsAnimationDefined(ref argmain_situation2, sub_situation: ref argsub_situation2))
                        {
                            string argmain_situation = "射程外";
                            string argsub_situation = "";
                            withBlock7.PlayAnimation(ref argmain_situation, sub_situation: ref argsub_situation);
                        }
                        else
                        {
                            string argmain_situation1 = "射程外";
                            string argsub_situation1 = "";
                            withBlock7.SpecialEffect(ref argmain_situation1, sub_situation: ref argsub_situation1);
                        }

                        string argSituation = "射程外";
                        string argmsg_mode = "";
                        withBlock7.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                    }
                }
                else
                {
                    SelectedWeapon = -1;
                }

                SelectedUnit = withBlock7.CurrentForm();

                // 防御側のユニットがかばわれた場合は2番目の防御側ユニットとして記録
                if (SupportGuardUnit is object)
                {
                    defense_target2 = SupportGuardUnit;
                    defense_target2_hp_ratio = SupportGuardUnitHPRatio;
                }
            }

            // 同時攻撃
            if (SupportAttackUnit is object)
            {
                if (SupportAttackUnit.Status_Renamed != "出撃" | SelectedUnit.Status_Renamed != "出撃" | SelectedTarget.Status_Renamed != "出撃")
                {
                    // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                    SupportAttackUnit = null;
                }
            }

            if (SupportAttackUnit is object)
            {
                if (SelectedUnit.MaxSyncAttack() > SelectedUnit.UsedSyncAttack)
                {
                    {
                        var withBlock8 = SupportAttackUnit;
                        // サポートアタックに使う武器を決定
                        string argamode1 = "サポートアタック";
                        int argmax_prob1 = 0;
                        int argmax_dmg1 = 0;
                        w2 = COM.SelectWeapon(ref SupportAttackUnit, ref SelectedTarget, ref argamode1, max_prob: ref argmax_prob1, max_dmg: ref argmax_dmg1);
                        if (w2 > 0)
                        {
                            // サポートアタックを実施
                            Map.MaskData[withBlock8.x, withBlock8.y] = false;
                            if (!SRC.BattleAnimation)
                            {
                                GUI.MaskScreen();
                            }

                            string argmain_situation4 = "サポートアタック開始";
                            string argsub_situation4 = "";
                            if (withBlock8.IsAnimationDefined(ref argmain_situation4, sub_situation: ref argsub_situation4))
                            {
                                string argmain_situation3 = "サポートアタック開始";
                                string argsub_situation3 = "";
                                withBlock8.PlayAnimation(ref argmain_situation3, sub_situation: ref argsub_situation3);
                            }

                            object argu2 = SupportAttackUnit;
                            GUI.UpdateMessageForm(ref SelectedTarget, ref argu2);
                            withBlock8.Attack(w2, SelectedTarget, "同時援護攻撃", def_mode);
                        }
                    }

                    // 後始末
                    {
                        var withBlock9 = SupportAttackUnit.CurrentForm();
                        if (w2 > 0)
                        {
                            string argmain_situation6 = "サポートアタック終了";
                            string argsub_situation6 = "";
                            if (withBlock9.IsAnimationDefined(ref argmain_situation6, sub_situation: ref argsub_situation6))
                            {
                                string argmain_situation5 = "サポートアタック終了";
                                string argsub_situation5 = "";
                                withBlock9.PlayAnimation(ref argmain_situation5, sub_situation: ref argsub_situation5);
                            }

                            // サポートアタックの残り回数を減らす
                            withBlock9.UsedSupportAttack = (withBlock9.UsedSupportAttack + 1);

                            // 同時援護攻撃の残り回数を減らす
                            SelectedUnit.UsedSyncAttack = (SelectedUnit.UsedSyncAttack + 1);
                        }
                    }

                    support_attack_done = true;

                    // 防御側のユニットがかばわれた場合は本来の防御ユニットデータと
                    // 入れ替えて記録
                    if (SupportGuardUnit is object)
                    {
                        defense_target = SupportGuardUnit;
                        defense_target_hp_ratio = SupportGuardUnitHPRatio;
                    }
                }
            }

            // 反撃の実施
            {
                var withBlock10 = SelectedTarget;
                if (def_mode != "先制攻撃")
                {
                    if (withBlock10.Status_Renamed == "出撃" & withBlock10.Party != "味方" & SelectedUnit.Status_Renamed == "出撃")
                    {
                        // まだ武器は使用可能か？
                        if (SelectedTWeapon > 0)
                        {
                            string argref_mode4 = "移動前";
                            if (SelectedTWeapon > withBlock10.CountWeapon())
                            {
                                SelectedTWeapon = -1;
                            }
                            else if ((twname ?? "") != (withBlock10.Weapon(SelectedTWeapon).Name ?? "") | !withBlock10.IsWeaponAvailable(SelectedTWeapon, ref argref_mode4))
                            {
                                SelectedTWeapon = -1;
                            }
                        }

                        if (SelectedTWeapon > 0)
                        {
                            if (!withBlock10.IsTargetWithinRange(SelectedTWeapon, ref SelectedUnit))
                            {
                                // 敵が射程外に逃げていたら武器を再選択
                                string argamode2 = "反撃";
                                int argmax_prob2 = 0;
                                int argmax_dmg2 = 0;
                                SelectedTWeapon = COM.SelectWeapon(ref SelectedTarget, ref SelectedUnit, ref argamode2, max_prob: ref argmax_prob2, max_dmg: ref argmax_dmg2);
                            }
                        }

                        // 行動不能な場合
                        if (withBlock10.MaxAction() == 0)
                        {
                            SelectedTWeapon = -1;
                        }

                        // 魅了された場合
                        object argIndex11 = "魅了";
                        if (withBlock10.IsConditionSatisfied(ref argIndex11) & ReferenceEquals(withBlock10.Master, SelectedUnit))
                        {
                            SelectedTWeapon = -1;
                        }

                        // 憑依された場合
                        object argIndex12 = "憑依";
                        if (withBlock10.IsConditionSatisfied(ref argIndex12))
                        {
                            if ((withBlock10.Master.Party ?? "") == (SelectedUnit.Party ?? ""))
                            {
                                SelectedWeapon = -1;
                            }
                        }

                        // 踊らされた場合
                        object argIndex13 = "踊り";
                        if (withBlock10.IsConditionSatisfied(ref argIndex13))
                        {
                            SelectedTWeapon = -1;
                        }

                        if (SelectedTWeapon > 0 & string.IsNullOrEmpty(def_mode))
                        {
                            // 反撃を実施
                            string argattr10 = "自";
                            if (withBlock10.IsWeaponClassifiedAs(SelectedTWeapon, ref argattr10))
                            {
                                is_suiside = true;
                            }

                            withBlock10.Attack(SelectedTWeapon, SelectedUnit, "", "");

                            // 攻撃側のユニットがかばわれた場合は攻撃側のターゲットを再設定
                            if (SupportGuardUnit2 is object)
                            {
                                attack_target = SupportGuardUnit2;
                                attack_target_hp_ratio = SupportGuardUnitHPRatio2;
                            }
                        }
                        else if (SelectedTWeapon == 0 & withBlock10.x == tx & withBlock10.y == ty)
                        {
                            // 反撃出来る武器がなかった場合は射程外メッセージを表示
                            string argmain_situation9 = "射程外";
                            string argsub_situation9 = "";
                            if (withBlock10.IsAnimationDefined(ref argmain_situation9, sub_situation: ref argsub_situation9))
                            {
                                string argmain_situation7 = "射程外";
                                string argsub_situation7 = "";
                                withBlock10.PlayAnimation(ref argmain_situation7, sub_situation: ref argsub_situation7);
                            }
                            else
                            {
                                string argmain_situation8 = "射程外";
                                string argsub_situation8 = "";
                                withBlock10.SpecialEffect(ref argmain_situation8, sub_situation: ref argsub_situation8);
                            }

                            string argSituation1 = "射程外";
                            string argmsg_mode1 = "";
                            withBlock10.PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
                        }
                        else
                        {
                            SelectedTWeapon = -1;
                        }
                    }
                    else
                    {
                        SelectedTWeapon = -1;
                    }
                }
            }

            // サポートアタック
            if (SupportAttackUnit is object)
            {
                if (SupportAttackUnit.Status_Renamed != "出撃" | SelectedUnit.Status_Renamed != "出撃" | SelectedTarget.Status_Renamed != "出撃" | support_attack_done)
                {
                    // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                    SupportAttackUnit = null;
                }
            }

            if (SupportAttackUnit is object)
            {
                {
                    var withBlock11 = SupportAttackUnit;
                    // サポートアタックに使う武器を決定
                    string argamode3 = "サポートアタック";
                    int argmax_prob3 = 0;
                    int argmax_dmg3 = 0;
                    w2 = COM.SelectWeapon(ref SupportAttackUnit, ref SelectedTarget, ref argamode3, max_prob: ref argmax_prob3, max_dmg: ref argmax_dmg3);
                    if (w2 > 0)
                    {
                        // サポートアタックを実施
                        Map.MaskData[withBlock11.x, withBlock11.y] = false;
                        if (!SRC.BattleAnimation)
                        {
                            GUI.MaskScreen();
                        }

                        string argmain_situation11 = "サポートアタック開始";
                        string argsub_situation11 = "";
                        if (withBlock11.IsAnimationDefined(ref argmain_situation11, sub_situation: ref argsub_situation11))
                        {
                            string argmain_situation10 = "サポートアタック開始";
                            string argsub_situation10 = "";
                            withBlock11.PlayAnimation(ref argmain_situation10, sub_situation: ref argsub_situation10);
                        }

                        object argu21 = SupportAttackUnit;
                        GUI.UpdateMessageForm(ref SelectedTarget, ref argu21);
                        withBlock11.Attack(w2, SelectedTarget, "援護攻撃", def_mode);
                    }
                }

                // 後始末
                {
                    var withBlock12 = SupportAttackUnit.CurrentForm();
                    string argmain_situation13 = "サポートアタック終了";
                    string argsub_situation13 = "";
                    if (withBlock12.IsAnimationDefined(ref argmain_situation13, sub_situation: ref argsub_situation13))
                    {
                        string argmain_situation12 = "サポートアタック終了";
                        string argsub_situation12 = "";
                        withBlock12.PlayAnimation(ref argmain_situation12, sub_situation: ref argsub_situation12);
                    }

                    // サポートアタックの残り回数を減らす
                    if (w2 > 0)
                    {
                        withBlock12.UsedSupportAttack = (withBlock12.UsedSupportAttack + 1);
                    }
                }

                // 防御側のユニットがかばわれた場合は本来の防御ユニットデータと
                // 入れ替えて記録
                if (SupportGuardUnit is object)
                {
                    defense_target = SupportGuardUnit;
                    defense_target_hp_ratio = SupportGuardUnitHPRatio;
                }
            }

            SelectedTarget = SelectedTarget.CurrentForm();
            {
                var withBlock13 = SelectedUnit;
                if (withBlock13.Status_Renamed == "出撃")
                {
                    // 攻撃をかけたユニットがまだ生き残っていれば経験値＆資金を獲得

                    if (SelectedTarget.Status_Renamed == "破壊" & !is_suiside)
                    {
                        // 敵を破壊した場合

                        // 経験値を獲得
                        string argexp_situation = "破壊";
                        string argexp_mode = "";
                        withBlock13.GetExp(ref SelectedTarget, ref argexp_situation, exp_mode: ref argexp_mode);
                        string argoname = "合体技パートナー経験値無効";
                        if (!Expression.IsOptionDefined(ref argoname))
                        {
                            var loopTo2 = Information.UBound(partners);
                            for (i = 1; i <= loopTo2; i++)
                            {
                                string argexp_situation1 = "破壊";
                                string argexp_mode1 = "パートナー";
                                partners[i].CurrentForm().GetExp(ref SelectedTarget, ref argexp_situation1, ref argexp_mode1);
                            }
                        }

                        // 獲得する資金を算出
                        earnings = SelectedTarget.Value / 2;

                        // スペシャルパワーによる獲得資金増加
                        string argsptype1 = "獲得資金増加";
                        if (withBlock13.IsUnderSpecialPowerEffect(ref argsptype1))
                        {
                            string argsname = "獲得資金増加";
                            earnings = (earnings * (1d + 0.1d * withBlock13.SpecialPowerEffectLevel(ref argsname)));
                        }

                        // パイロット能力による獲得資金増加
                        string argsname1 = "資金獲得";
                        if (withBlock13.IsSkillAvailable(ref argsname1))
                        {
                            string argsptype2 = "獲得資金増加";
                            string argoname1 = "収得効果重複";
                            if (!withBlock13.IsUnderSpecialPowerEffect(ref argsptype2) | Expression.IsOptionDefined(ref argoname1))
                            {
                                earnings = GeneralLib.MinDbl(earnings * ((10d + withBlock13.SkillLevel("資金獲得", 5d)) / 10d), 999999999d);
                            }
                        }

                        // 資金を獲得
                        SRC.IncrMoney(earnings);
                        if (earnings > 0)
                        {
                            string argtname = "資金";
                            GUI.DisplaySysMessage(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(earnings) + "の" + Expression.Term(ref argtname, ref SelectedUnit) + "を得た。");
                        }

                        // スペシャルパワー効果「敵破壊時再行動」
                        string argsptype3 = "敵破壊時再行動";
                        if (withBlock13.IsUnderSpecialPowerEffect(ref argsptype3))
                        {
                            withBlock13.UsedAction = (withBlock13.UsedAction - 1);
                        }
                    }
                    else
                    {
                        // 相手を破壊できなかった場合

                        // 経験値を獲得
                        string argexp_situation2 = "攻撃";
                        string argexp_mode2 = "";
                        withBlock13.GetExp(ref SelectedTarget, ref argexp_situation2, exp_mode: ref argexp_mode2);
                        string argoname2 = "合体技パートナー経験値無効";
                        if (!Expression.IsOptionDefined(ref argoname2))
                        {
                            var loopTo3 = Information.UBound(partners);
                            for (i = 1; i <= loopTo3; i++)
                            {
                                string argexp_situation3 = "攻撃";
                                string argexp_mode3 = "パートナー";
                                partners[i].CurrentForm().GetExp(ref SelectedTarget, ref argexp_situation3, ref argexp_mode3);
                            }
                        }
                    }

                    // スペシャルパワー「獲得資金増加」「獲得経験値増加」の効果はここで削除する
                    string argstype = "戦闘終了";
                    withBlock13.RemoveSpecialPowerInEffect(ref argstype);
                    if (earnings > 0)
                    {
                        string argstype1 = "敵破壊";
                        withBlock13.RemoveSpecialPowerInEffect(ref argstype1);
                    }
                }
            }

            {
                var withBlock14 = SelectedTarget;
                if (withBlock14.Status_Renamed == "出撃")
                {
                    // 持続期間が「戦闘終了」のスペシャルパワー効果を削除
                    string argstype2 = "戦闘終了";
                    withBlock14.RemoveSpecialPowerInEffect(ref argstype2);
                }
            }

            GUI.CloseMessageForm();
            Status.ClearUnitStatus();

            // 状態＆データ更新
            {
                var withBlock15 = attack_target.CurrentForm();
                withBlock15.UpdateCondition();
                withBlock15.Update();
            }

            if (SupportAttackUnit is object)
            {
                {
                    var withBlock16 = SupportAttackUnit.CurrentForm();
                    withBlock16.UpdateCondition();
                    withBlock16.Update();
                }
            }

            {
                var withBlock17 = defense_target.CurrentForm();
                withBlock17.UpdateCondition();
                withBlock17.Update();
            }

            if (defense_target2 is object)
            {
                {
                    var withBlock18 = defense_target2.CurrentForm();
                    withBlock18.UpdateCondition();
                    withBlock18.Update();
                }
            }

            // 破壊＆損傷率イベント発生

            if (SelectedWeapon <= 0)
            {
                SelectedWeaponName = "";
            }

            if (SelectedTWeapon <= 0)
            {
                SelectedTWeaponName = "";
            }

            // 攻撃を受けた攻撃側ユニット
            {
                var withBlock19 = attack_target.CurrentForm();
                if (withBlock19.Status_Renamed == "破壊")
                {
                    Event_Renamed.HandleEvent("破壊", withBlock19.MainPilot().ID);
                }
                else if (withBlock19.Status_Renamed == "出撃" & withBlock19.HP / (double)withBlock19.MaxHP < attack_target_hp_ratio)
                {
                    Event_Renamed.HandleEvent("損傷率", withBlock19.MainPilot().ID, 100 * (withBlock19.MaxHP - withBlock19.HP) / withBlock19.MaxHP);
                }
            }

            if (SRC.IsScenarioFinished)
            {
                GUI.UnlockGUI();
                SRC.IsScenarioFinished = false;
                SelectedPartners = new Unit[1];
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                goto EndAttack;
            }

            SelectedUnit = SelectedUnit.CurrentForm();

            // ターゲット側のイベント処理を行うためにユニットの入れ替えを行う
            SaveSelections();
            SwapSelections();

            // 攻撃を受けた防御側ユニット
            {
                var withBlock20 = defense_target.CurrentForm();
                if (withBlock20.CountPilot() > 0)
                {
                    if (withBlock20.Status_Renamed == "破壊")
                    {
                        Event_Renamed.HandleEvent("破壊", withBlock20.MainPilot().ID);
                    }
                    else if (withBlock20.Status_Renamed == "出撃" & withBlock20.HP / (double)withBlock20.MaxHP < defense_target_hp_ratio)
                    {
                        Event_Renamed.HandleEvent("損傷率", withBlock20.MainPilot().ID, 100 * (withBlock20.MaxHP - withBlock20.HP) / withBlock20.MaxHP);
                    }
                }
            }

            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                SelectedPartners = new Unit[1];
                return;
            }

            // 攻撃を受けた防御側ユニットその2
            if (defense_target2 is object)
            {
                if (!ReferenceEquals(defense_target2.CurrentForm(), defense_target.CurrentForm()))
                {
                    {
                        var withBlock21 = defense_target2.CurrentForm();
                        if (withBlock21.CountPilot() > 0)
                        {
                            if (withBlock21.Status_Renamed == "破壊")
                            {
                                Event_Renamed.HandleEvent("破壊", withBlock21.MainPilot().ID);
                            }
                            else if (withBlock21.Status_Renamed == "出撃" & withBlock21.HP / (double)withBlock21.MaxHP < defense_target2_hp_ratio)
                            {
                                Event_Renamed.HandleEvent("損傷率", withBlock21.MainPilot().ID, 100 * (withBlock21.MaxHP - withBlock21.HP) / withBlock21.MaxHP);
                            }
                        }
                    }
                }
            }

            RestoreSelections();
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                SelectedPartners = new Unit[1];
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                SelectedPartners = new Unit[1];
                GUI.UnlockGUI();
                return;
            }

            // 武器の使用後イベント
            if (SelectedUnit.Status_Renamed == "出撃" & SelectedWeapon > 0)
            {
                Event_Renamed.HandleEvent("使用後", SelectedUnit.MainPilot().ID, wname);
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    SelectedPartners = new Unit[1];
                    GUI.UnlockGUI();
                    return;
                }

                if (SRC.IsCanceled)
                {
                    SRC.IsCanceled = false;
                    SelectedPartners = new Unit[1];
                    GUI.UnlockGUI();
                    return;
                }
            }

            if (SelectedTarget.Status_Renamed == "出撃" & SelectedTWeapon > 0)
            {
                SaveSelections();
                SwapSelections();
                Event_Renamed.HandleEvent("使用後", SelectedUnit.MainPilot().ID, twname);
                RestoreSelections();
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    SelectedPartners = new Unit[1];
                    GUI.UnlockGUI();
                    return;
                }

                if (SRC.IsCanceled)
                {
                    SRC.IsCanceled = false;
                    SelectedPartners = new Unit[1];
                    GUI.UnlockGUI();
                    return;
                }
            }

            // 攻撃後イベント
            if (SelectedUnit.Status_Renamed == "出撃" & SelectedTarget.Status_Renamed == "出撃")
            {
                Event_Renamed.HandleEvent("攻撃後", SelectedUnit.MainPilot().ID, SelectedTarget.MainPilot().ID);
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    SelectedPartners = new Unit[1];
                    GUI.UnlockGUI();
                    return;
                }

                if (SRC.IsCanceled)
                {
                    SRC.IsCanceled = false;
                    SelectedPartners = new Unit[1];
                    GUI.UnlockGUI();
                    return;
                }
            }

            // もし敵が移動していれば進入イベント
            {
                var withBlock22 = SelectedTarget;
                // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                SelectedTarget = null;
                if (withBlock22.Status_Renamed == "出撃")
                {
                    if (withBlock22.x != tx | withBlock22.y != ty)
                    {
                        Event_Renamed.HandleEvent("進入", withBlock22.MainPilot().ID, withBlock22.x, withBlock22.y);
                        if (SRC.IsScenarioFinished)
                        {
                            SRC.IsScenarioFinished = false;
                            SelectedPartners = new Unit[1];
                            GUI.UnlockGUI();
                            return;
                        }

                        if (SRC.IsCanceled)
                        {
                            SRC.IsCanceled = false;
                            SelectedPartners = new Unit[1];
                            GUI.UnlockGUI();
                            return;
                        }
                    }
                }
            }

        EndAttack:
            ;


            // 合体技のパートナーの行動数を減らす
            string argoname3 = "合体技パートナー行動数無消費";
            if (!Expression.IsOptionDefined(ref argoname3))
            {
                var loopTo4 = Information.UBound(partners);
                for (i = 1; i <= loopTo4; i++)
                    partners[i].CurrentForm().UseAction();
            }

            SelectedPartners = new Unit[1];

            // ハイパーモード＆ノーマルモードの自動発動をチェック
            SRC.UList.CheckAutoHyperMode();
            SRC.UList.CheckAutoNormalMode();

            // ADD START MARGE
            // 再移動
            if (is_p_weapon & SelectedUnit.Status_Renamed == "出撃")
            {
                string argsname2 = "遊撃";
                if (SelectedUnit.MainPilot().IsSkillAvailable(ref argsname2) & SelectedUnit.Speed * 2 > SelectedUnitMoveCost)
                {
                    // 進入イベント
                    if (SelectedUnitMoveCost > 0)
                    {
                        Event_Renamed.HandleEvent("進入", SelectedUnit.MainPilot().ID, SelectedUnit.x, SelectedUnit.y);
                        if (SRC.IsScenarioFinished)
                        {
                            SRC.IsScenarioFinished = false;
                            return;
                        }
                    }

                    // ユニットが既に出撃していない？
                    if (SelectedUnit.Status_Renamed != "出撃")
                    {
                        GUI.RedrawScreen();
                        Status.ClearUnitStatus();
                        return;
                    }

                    SelectedCommand = "再移動";
                    Map.AreaInSpeed(ref SelectedUnit);
                    string argoname4 = "大型マップ";
                    if (!Expression.IsOptionDefined(ref argoname4))
                    {
                        GUI.Center(SelectedUnit.x, SelectedUnit.y);
                    }

                    GUI.MaskScreen();
                    if (GUI.NewGUIMode)
                    {
                        Application.DoEvents();
                        Status.ClearUnitStatus();
                    }
                    else
                    {
                        Status.DisplayUnitStatus(ref SelectedUnit);
                    }

                    CommandState = "ターゲット選択";
                    return;
                }
            }
            // ADD END MARGE

            // 行動数を減らす
            WaitCommand();
        }

        // マップ攻撃による「攻撃」コマンドを終了
        // MOD START MARGE
        // Public Sub MapAttackCommand()
        private void MapAttackCommand()
        {
            // MOD END MARGE
            int i;
            var partners = default(Unit[]);
            // ADD START MARGE
            bool is_p_weapon;
            // ADD END MARGE

            {
                var withBlock = SelectedUnit;
                // ADD START MARGE
                // 移動後使用後可能な武器か記録しておく
                string argattr = "移動後攻撃可";
                is_p_weapon = withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr);
                // ADD END MARGE
                // 攻撃目標地点を選択して初めて攻撃範囲が分かるタイプのマップ攻撃
                // の場合は再度プレイヤーの選択を促す必要がある
                if (CommandState == "ターゲット選択" | CommandState == "移動後ターゲット選択")
                {
                    string argattr4 = "Ｍ投";
                    string argattr5 = "Ｍ移";
                    string argattr6 = "Ｍ線";
                    if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr4))
                    {
                        if (CommandState == "ターゲット選択")
                        {
                            CommandState = "マップ攻撃使用";
                        }
                        else
                        {
                            CommandState = "移動後マップ攻撃使用";
                        }

                        // 攻撃目標地点
                        SelectedX = GUI.PixelToMapX(GUI.MouseX);
                        SelectedY = GUI.PixelToMapY(GUI.MouseY);

                        // 攻撃範囲を設定
                        string argattr3 = "識";
                        string argsptype = "識別攻撃";
                        if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr3) | withBlock.IsUnderSpecialPowerEffect(ref argsptype))
                        {
                            string argattr1 = "Ｍ投";
                            string arguparty = "味方の敵";
                            Map.AreaInRange(SelectedX, SelectedY, withBlock.WeaponLevel(SelectedWeapon, ref argattr1), 1, ref arguparty);
                        }
                        else
                        {
                            string argattr2 = "Ｍ投";
                            string arguparty1 = "すべて";
                            Map.AreaInRange(SelectedX, SelectedY, withBlock.WeaponLevel(SelectedWeapon, ref argattr2), 1, ref arguparty1);
                        }

                        GUI.MaskScreen();
                        return;
                    }
                    else if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr5))
                    {
                        // 攻撃目標地点
                        SelectedX = GUI.PixelToMapX(GUI.MouseX);
                        SelectedY = GUI.PixelToMapY(GUI.MouseY);

                        // 攻撃目標地点に他のユニットがいては駄目
                        if (Map.MapDataForUnit[SelectedX, SelectedY] is object)
                        {
                            GUI.MaskScreen();
                            return;
                        }

                        if (CommandState == "ターゲット選択")
                        {
                            CommandState = "マップ攻撃使用";
                        }
                        else
                        {
                            CommandState = "移動後マップ攻撃使用";
                        }

                        // 攻撃範囲を設定
                        Map.AreaInPointToPoint(withBlock.x, withBlock.y, SelectedX, SelectedY);
                        GUI.MaskScreen();
                        return;
                    }
                    else if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr6))
                    {
                        if (CommandState == "ターゲット選択")
                        {
                            CommandState = "マップ攻撃使用";
                        }
                        else
                        {
                            CommandState = "移動後マップ攻撃使用";
                        }

                        // 攻撃目標地点
                        SelectedX = GUI.PixelToMapX(GUI.MouseX);
                        SelectedY = GUI.PixelToMapY(GUI.MouseY);

                        // 攻撃範囲を設定
                        Map.AreaInPointToPoint(withBlock.x, withBlock.y, SelectedX, SelectedY);
                        GUI.MaskScreen();
                        return;
                    }
                }

                // 合体技パートナーの設定
                string argattr7 = "合";
                if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr7))
                {
                    string argctype_Renamed = "武装";
                    withBlock.CombinationPartner(ref argctype_Renamed, SelectedWeapon, ref partners);
                }
                else
                {
                    SelectedPartners = new Unit[1];
                    partners = new Unit[1];
                }

                if (GUI.MainWidth != 15)
                {
                    Status.ClearUnitStatus();
                }

                GUI.LockGUI();
                SelectedTWeapon = 0;

                // マップ攻撃による攻撃を行う
                withBlock.MapAttack(SelectedWeapon, SelectedX, SelectedY);
                SelectedUnit = withBlock.CurrentForm();
                // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                SelectedTarget = null;
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    SelectedPartners = new Unit[1];
                    GUI.UnlockGUI();
                    return;
                }

                if (SRC.IsCanceled)
                {
                    SRC.IsCanceled = false;
                    SelectedPartners = new Unit[1];
                    WaitCommand();
                    return;
                }
            }

            // 合体技のパートナーの行動数を減らす
            string argoname = "合体技パートナー行動数無消費";
            if (!Expression.IsOptionDefined(ref argoname))
            {
                var loopTo = Information.UBound(partners);
                for (i = 1; i <= loopTo; i++)
                    partners[i].CurrentForm().UseAction();
            }

            SelectedPartners = new Unit[1];

            // ADD START MARGE
            // 再移動
            if (is_p_weapon & SelectedUnit.Status_Renamed == "出撃")
            {
                string argsname = "遊撃";
                if (SelectedUnit.MainPilot().IsSkillAvailable(ref argsname) & SelectedUnit.Speed * 2 > SelectedUnitMoveCost)
                {
                    // 進入イベント
                    if (SelectedUnitMoveCost > 0)
                    {
                        Event_Renamed.HandleEvent("進入", SelectedUnit.MainPilot().ID, SelectedUnit.x, SelectedUnit.y);
                        if (SRC.IsScenarioFinished)
                        {
                            SRC.IsScenarioFinished = false;
                            return;
                        }
                    }

                    // ユニットが既に出撃していない？
                    if (SelectedUnit.Status_Renamed != "出撃")
                    {
                        GUI.RedrawScreen();
                        Status.ClearUnitStatus();
                        return;
                    }

                    SelectedCommand = "再移動";
                    Map.AreaInSpeed(ref SelectedUnit);
                    string argoname1 = "大型マップ";
                    if (!Expression.IsOptionDefined(ref argoname1))
                    {
                        GUI.Center(SelectedUnit.x, SelectedUnit.y);
                    }

                    GUI.MaskScreen();
                    if (GUI.NewGUIMode)
                    {
                        Application.DoEvents();
                        Status.ClearUnitStatus();
                    }
                    else
                    {
                        Status.DisplayUnitStatus(ref SelectedUnit);
                    }

                    CommandState = "ターゲット選択";
                    return;
                }
            }
            // ADD END MARGE

            // 行動終了
            WaitCommand();
        }


        // 「アビリティ」コマンドを開始
        // is_item=True の場合は「アイテム」コマンドによる使い捨てアイテムのアビリティ
        // MOD STAR MARGE
        // Public Sub StartAbilityCommand(Optional ByVal is_item As Boolean)
        private void StartAbilityCommand(bool is_item = false)
        {
            // MOD END MARGE
            int i, j;
            Unit t;
            int min_range, max_range;
            string cap;
            GUI.LockGUI();

            // 使用するアビリティを選択
            if (is_item)
            {
                cap = "アイテム選択";
            }
            else
            {
                string argtname = "アビリティ";
                cap = Expression.Term(ref argtname, ref SelectedUnit) + "選択";
            }

            if (CommandState == "コマンド選択")
            {
                string arglb_mode = "移動前";
                SelectedAbility = GUI.AbilityListBox(ref SelectedUnit, ref cap, ref arglb_mode, is_item);
            }
            else
            {
                string arglb_mode1 = "移動後";
                SelectedAbility = GUI.AbilityListBox(ref SelectedUnit, ref cap, ref arglb_mode1, is_item);
            }

            // キャンセル
            if (SelectedAbility == 0)
            {
                if (SRC.AutoMoveCursor)
                {
                    GUI.RestoreCursorPos();
                }

                CancelCommand();
                GUI.UnlockGUI();
                return;
            }

            // アビリティ専用ＢＧＭがあればそれを演奏
            string BGM;
            {
                var withBlock = SelectedUnit;
                string argfname = "アビリティＢＧＭ";
                if (withBlock.IsFeatureAvailable(ref argfname))
                {
                    var loopTo = withBlock.CountFeature();
                    for (i = 1; i <= loopTo; i++)
                    {
                        string localFeature() { object argIndex1 = i; var ret = withBlock.Feature(ref argIndex1); return ret; }

                        string localFeatureData2() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                        string localLIndex() { string arglist = hs8866f19275ca4c5cbfc3bb7415f2da30(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

                        if (localFeature() == "アビリティＢＧＭ" & (localLIndex() ?? "") == (withBlock.Ability(SelectedAbility).Name ?? ""))
                        {
                            string localFeatureData() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                            string localFeatureData1() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                            string argmidi_name = Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1);
                            BGM = Sound.SearchMidiFile(ref argmidi_name);
                            if (Strings.Len(BGM) > 0)
                            {
                                Sound.ChangeBGM(ref BGM);
                            }

                            break;
                        }
                    }
                }
            }

            // 射程0のアビリティはその場で実行
            var is_transformation = default(bool);
            if (SelectedUnit.Ability(SelectedAbility).MaxRange == 0)
            {
                SelectedTarget = SelectedUnit;

                // 変身アビリティであるか判定
                var loopTo1 = SelectedUnit.Ability(SelectedAbility).CountEffect();
                for (i = 1; i <= loopTo1; i++)
                {
                    object argIndex1 = i;
                    if (SelectedUnit.Ability(SelectedAbility).EffectType(ref argIndex1) == "変身")
                    {
                        is_transformation = true;
                        break;
                    }
                }

                SelectedAbilityName = SelectedUnit.Ability(SelectedAbility).Name;

                // 使用イベント
                Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, SelectedAbilityName);
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    GUI.UnlockGUI();
                    return;
                }

                if (SRC.IsCanceled)
                {
                    SRC.IsCanceled = false;
                    WaitCommand();
                    return;
                }

                // アビリティを実行
                SelectedUnit.ExecuteAbility(SelectedAbility, ref SelectedUnit);
                SelectedUnit = SelectedUnit.CurrentForm();
                GUI.CloseMessageForm();

                // 破壊イベント
                {
                    var withBlock1 = SelectedUnit;
                    if (withBlock1.Status_Renamed == "破壊")
                    {
                        if (withBlock1.CountPilot() > 0)
                        {
                            Event_Renamed.HandleEvent("破壊", withBlock1.MainPilot().ID);
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
                        }

                        WaitCommand();
                        return;
                    }
                }

                // 使用後イベント
                {
                    var withBlock2 = SelectedUnit;
                    if (withBlock2.CountPilot() > 0)
                    {
                        Event_Renamed.HandleEvent("使用後", withBlock2.MainPilot().ID, SelectedAbilityName);
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
                    }
                }

                // 変身アビリティの場合は行動終了しない
                if (!is_transformation | CommandState == "移動後コマンド選択")
                {
                    WaitCommand();
                }
                else
                {
                    if (SelectedUnit.Status_Renamed == "出撃")
                    {
                        // カーソル自動移動
                        if (SRC.AutoMoveCursor)
                        {
                            string argcursor_mode = "ユニット選択";
                            GUI.MoveCursorPos(ref argcursor_mode, SelectedUnit);
                        }

                        Status.DisplayUnitStatus(ref SelectedUnit);
                    }
                    else
                    {
                        Status.ClearUnitStatus();
                    }

                    CommandState = "ユニット選択";
                    GUI.UnlockGUI();
                }

                return;
            }

            var partners = default(Unit[]);
            {
                var withBlock3 = SelectedUnit;
                // マップ型アビリティかどうかで今後のコマンド処理の進行の仕方が異なる
                if (is_item)
                {
                    string argattr = "Ｍ";
                    if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr))
                    {
                        SelectedCommand = "マップアイテム";
                    }
                    else
                    {
                        SelectedCommand = "アイテム";
                    }
                }
                else
                {
                    string argattr1 = "Ｍ";
                    if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr1))
                    {
                        SelectedCommand = "マップアビリティ";
                    }
                    else
                    {
                        SelectedCommand = "アビリティ";
                    }
                }

                // アビリティの射程を求めておく
                min_range = withBlock3.AbilityMinRange(SelectedAbility);
                max_range = withBlock3.AbilityMaxRange(SelectedAbility);

                // アビリティの効果範囲を設定
                string argattr3 = "Ｍ直";
                string argattr4 = "Ｍ拡";
                string argattr5 = "Ｍ扇";
                string argattr6 = "Ｍ移";
                if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr3))
                {
                    Map.AreaInCross(withBlock3.x, withBlock3.y, min_range, ref max_range);
                }
                else if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr4))
                {
                    Map.AreaInWideCross(withBlock3.x, withBlock3.y, min_range, ref max_range);
                }
                else if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr5))
                {
                    string argattr2 = "Ｍ扇";
                    Map.AreaInSectorCross(withBlock3.x, withBlock3.y, min_range, ref max_range, withBlock3.AbilityLevel(SelectedAbility, ref argattr2));
                }
                else if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr6))
                {
                    Map.AreaInMoveAction(ref SelectedUnit, max_range);
                }
                else
                {
                    string arguparty = "すべて";
                    Map.AreaInRange(withBlock3.x, withBlock3.y, max_range, min_range, ref arguparty);
                }

                // 射程１の合体技はパートナーで相手を取り囲んでいないと使用できない
                string argattr7 = "合";
                string argattr8 = "Ｍ";
                if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr7) & !withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr8) & withBlock3.Ability(SelectedAbility).MaxRange == 1)
                {
                    for (i = 1; i <= 4; i++)
                    {
                        // UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        t = null;
                        switch (i)
                        {
                            case 1:
                                {
                                    if (withBlock3.x > 1)
                                    {
                                        t = Map.MapDataForUnit[withBlock3.x - 1, withBlock3.y];
                                    }

                                    break;
                                }

                            case 2:
                                {
                                    if (withBlock3.x < Map.MapWidth)
                                    {
                                        t = Map.MapDataForUnit[withBlock3.x + 1, withBlock3.y];
                                    }

                                    break;
                                }

                            case 3:
                                {
                                    if (withBlock3.y > 1)
                                    {
                                        t = Map.MapDataForUnit[withBlock3.x, withBlock3.y - 1];
                                    }

                                    break;
                                }

                            case 4:
                                {
                                    if (withBlock3.y < Map.MapHeight)
                                    {
                                        t = Map.MapDataForUnit[withBlock3.x, withBlock3.y + 1];
                                    }

                                    break;
                                }
                        }

                        if (t is object)
                        {
                            if (withBlock3.IsAlly(ref t))
                            {
                                string argctype_Renamed = "アビリティ";
                                withBlock3.CombinationPartner(ref argctype_Renamed, SelectedAbility, ref partners, t.x, t.y);
                                if (Information.UBound(partners) == 0)
                                {
                                    Map.MaskData[t.x, t.y] = true;
                                }
                            }
                        }
                    }
                }

                // ユニットがいるマスの処理
                string argattr9 = "Ｍ投";
                string argattr10 = "Ｍ線";
                string argattr11 = "Ｍ移";
                if (!withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr9) & !withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr10) & !withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr11))
                {
                    var loopTo2 = GeneralLib.MinLng(withBlock3.x + max_range, Map.MapWidth);
                    for (i = GeneralLib.MaxLng(withBlock3.x - max_range, 1); i <= loopTo2; i++)
                    {
                        var loopTo3 = GeneralLib.MinLng(withBlock3.y + max_range, Map.MapHeight);
                        for (j = GeneralLib.MaxLng(withBlock3.y - max_range, 1); j <= loopTo3; j++)
                        {
                            if (!Map.MaskData[i, j])
                            {
                                t = Map.MapDataForUnit[i, j];
                                if (t is object)
                                {
                                    // 有効？
                                    if (withBlock3.IsAbilityEffective(SelectedAbility, ref t))
                                    {
                                        Map.MaskData[i, j] = false;
                                    }
                                    else
                                    {
                                        Map.MaskData[i, j] = true;
                                    }
                                }
                            }
                        }
                    }
                }

                // 支援専用アビリティは自分には使用できない
                if (!Map.MaskData[withBlock3.x, withBlock3.y])
                {
                    string argattr12 = "援";
                    if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr12))
                    {
                        Map.MaskData[withBlock3.x, withBlock3.y] = true;
                    }
                }

                string argoname = "大型マップ";
                if (!Expression.IsOptionDefined(ref argoname))
                {
                    GUI.Center(withBlock3.x, withBlock3.y);
                }

                GUI.MaskScreen();
            }

            if (CommandState == "コマンド選択")
            {
                CommandState = "ターゲット選択";
            }
            else
            {
                CommandState = "移動後ターゲット選択";
            }

            // カーソル自動移動を行う？
            if (!SRC.AutoMoveCursor)
            {
                GUI.UnlockGUI();
                return;
            }

            // 自分から最も近い味方ユニットを探す
            // UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            t = null;
            foreach (Unit u in SRC.UList)
            {
                if (u.Status_Renamed == "出撃" & u.Party == "味方")
                {
                    if (Map.MaskData[u.x, u.y] == false & !ReferenceEquals(u, SelectedUnit))
                    {
                        if (t is null)
                        {
                            t = u;
                        }
                        else if (Math.Pow(Math.Abs((SelectedUnit.x - u.x)), 2d) + Math.Pow(Math.Abs((SelectedUnit.y - u.y)), 2d) < Math.Pow(Math.Abs((SelectedUnit.x - t.x)), 2d) + Math.Pow(Math.Abs((SelectedUnit.y - t.y)), 2d))
                        {
                            t = u;
                        }
                    }
                }
            }

            // 適当がユニットがなければ自分自身を選択
            if (t is null)
            {
                t = SelectedUnit;
            }

            // カーソルを移動
            string argcursor_mode1 = "ユニット選択";
            GUI.MoveCursorPos(ref argcursor_mode1, t);

            // ターゲットのステータスを表示
            if (!ReferenceEquals(SelectedUnit, t))
            {
                Status.DisplayUnitStatus(ref t);
            }

            GUI.UnlockGUI();
        }

        // 「アビリティ」コマンドを終了
        // MOD START MARGE
        // Public Sub FinishAbilityCommand()
        private void FinishAbilityCommand()
        {
            // MOD END MARGE
            int i;
            var partners = default(Unit[]);
            string aname;
            // ADD START MARGE
            bool is_p_ability;
            // ADD END MARGE

            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Status.ClearUnitStatus();
            }

            GUI.LockGUI();

            // 合体技のパートナーを設定
            {
                var withBlock = SelectedUnit;
                string argattr = "合";
                if (withBlock.IsAbilityClassifiedAs(SelectedAbility, ref argattr))
                {
                    if (withBlock.AbilityMaxRange(SelectedAbility) == 1)
                    {
                        string argctype_Renamed = "アビリティ";
                        withBlock.CombinationPartner(ref argctype_Renamed, SelectedAbility, ref partners, SelectedTarget.x, SelectedTarget.y);
                    }
                    else
                    {
                        string argctype_Renamed1 = "アビリティ";
                        withBlock.CombinationPartner(ref argctype_Renamed1, SelectedAbility, ref partners);
                    }
                }
                else
                {
                    SelectedPartners = new Unit[1];
                    partners = new Unit[1];
                }
            }

            aname = SelectedUnit.Ability(SelectedAbility).Name;
            SelectedAbilityName = aname;

            // ADD START MARGE
            // 移動後使用後可能なアビリティか記録しておく
            string argattr1 = "Ｐ";
            string argattr2 = "Ｑ";
            is_p_ability = SelectedUnit.IsAbilityClassifiedAs(SelectedAbility, ref argattr1) | SelectedUnit.AbilityMaxRange(SelectedAbility) == 1 & !SelectedUnit.IsAbilityClassifiedAs(SelectedAbility, ref argattr2);
            // ADD END MARGE

            // 使用イベント
            Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, aname);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                SelectedPartners = new Unit[1];
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                SelectedPartners = new Unit[1];
                WaitCommand();
                return;
            }

            {
                var withBlock1 = SelectedUnit;
                foreach (Unit u in SRC.UList)
                {
                    if (u.Status_Renamed == "出撃")
                    {
                        Map.MaskData[u.x, u.y] = true;
                    }
                }

                // 合体技パートナーのハイライト表示
                string argattr3 = "合";
                if (withBlock1.IsAbilityClassifiedAs(SelectedAbility, ref argattr3))
                {
                    var loopTo = Information.UBound(partners);
                    for (i = 1; i <= loopTo; i++)
                    {
                        {
                            var withBlock2 = partners[i];
                            Map.MaskData[withBlock2.x, withBlock2.y] = false;
                        }
                    }
                }

                Map.MaskData[withBlock1.x, withBlock1.y] = false;
                Map.MaskData[SelectedTarget.x, SelectedTarget.y] = false;
                if (!SRC.BattleAnimation)
                {
                    GUI.MaskScreen();
                }

                // アビリティを実行
                withBlock1.ExecuteAbility(SelectedAbility, ref SelectedTarget);
                SelectedUnit = withBlock1.CurrentForm();
                GUI.CloseMessageForm();
                Status.ClearUnitStatus();
            }

            // 破壊イベント
            {
                var withBlock3 = SelectedUnit;
                if (withBlock3.Status_Renamed == "破壊")
                {
                    if (withBlock3.CountPilot() > 0)
                    {
                        Event_Renamed.HandleEvent("破壊", withBlock3.MainPilot().ID);
                        if (SRC.IsScenarioFinished)
                        {
                            SRC.IsScenarioFinished = false;
                            SelectedPartners = new Unit[1];
                            GUI.UnlockGUI();
                            return;
                        }

                        if (SRC.IsCanceled)
                        {
                            SRC.IsCanceled = false;
                            SelectedPartners = new Unit[1];
                            GUI.UnlockGUI();
                            return;
                        }
                    }

                    WaitCommand();
                    return;
                }
            }

            // 使用後イベント
            {
                var withBlock4 = SelectedUnit;
                if (withBlock4.CountPilot() > 0)
                {
                    Event_Renamed.HandleEvent("使用後", withBlock4.MainPilot().ID, aname);
                    if (SRC.IsScenarioFinished)
                    {
                        SRC.IsScenarioFinished = false;
                        SelectedPartners = new Unit[1];
                        GUI.UnlockGUI();
                        return;
                    }

                    if (SRC.IsCanceled)
                    {
                        SRC.IsCanceled = false;
                        SelectedPartners = new Unit[1];
                        GUI.UnlockGUI();
                        return;
                    }
                }
            }

            // 合体技のパートナーの行動数を減らす
            string argoname = "合体技パートナー行動数無消費";
            if (!Expression.IsOptionDefined(ref argoname))
            {
                var loopTo1 = Information.UBound(partners);
                for (i = 1; i <= loopTo1; i++)
                    partners[i].CurrentForm().UseAction();
            }

            SelectedPartners = new Unit[1];

            // ADD START MARGE
            // 再移動
            if (is_p_ability & SelectedUnit.Status_Renamed == "出撃")
            {
                string argsname = "遊撃";
                if (SelectedUnit.MainPilot().IsSkillAvailable(ref argsname) & SelectedUnit.Speed * 2 > SelectedUnitMoveCost)
                {
                    // 進入イベント
                    if (SelectedUnitMoveCost > 0)
                    {
                        Event_Renamed.HandleEvent("進入", SelectedUnit.MainPilot().ID, SelectedUnit.x, SelectedUnit.y);
                        if (SRC.IsScenarioFinished)
                        {
                            SRC.IsScenarioFinished = false;
                            return;
                        }
                    }

                    // ユニットが既に出撃していない？
                    if (SelectedUnit.Status_Renamed != "出撃")
                    {
                        GUI.RedrawScreen();
                        Status.ClearUnitStatus();
                        return;
                    }

                    SelectedCommand = "再移動";
                    Map.AreaInSpeed(ref SelectedUnit);
                    string argoname1 = "大型マップ";
                    if (!Expression.IsOptionDefined(ref argoname1))
                    {
                        GUI.Center(SelectedUnit.x, SelectedUnit.y);
                    }

                    GUI.MaskScreen();
                    if (GUI.NewGUIMode)
                    {
                        Application.DoEvents();
                        Status.ClearUnitStatus();
                    }
                    else
                    {
                        Status.DisplayUnitStatus(ref SelectedUnit);
                    }

                    CommandState = "ターゲット選択";
                    return;
                }
            }
            // ADD END MARGE

            // 行動終了
            WaitCommand();
        }

        // マップ型「アビリティ」コマンドを終了
        // MOD START MARGE
        // Public Sub MapAbilityCommand()
        private void MapAbilityCommand()
        {
            // MOD END MARGE
            int i;
            var partners = default(Unit[]);
            // ADD START MARGE
            bool is_p_ability;
            // ADD END MARGE

            {
                var withBlock = SelectedUnit;
                // ADD START MARGE
                // 移動後使用後可能なアビリティか記録しておく
                string argattr = "Ｐ";
                string argattr1 = "Ｑ";
                is_p_ability = withBlock.IsAbilityClassifiedAs(SelectedAbility, ref argattr) | withBlock.AbilityMaxRange(SelectedAbility) == 1 & !withBlock.IsAbilityClassifiedAs(SelectedAbility, ref argattr1);
                // ADD END MARGE

                // 目標地点を選択して初めて効果範囲が分かるタイプのマップアビリティ
                // の場合は再度プレイヤーの選択を促す必要がある
                if (CommandState == "ターゲット選択" | CommandState == "移動後ターゲット選択")
                {
                    string argattr3 = "Ｍ投";
                    string argattr4 = "Ｍ移";
                    string argattr5 = "Ｍ線";
                    if (withBlock.IsAbilityClassifiedAs(SelectedAbility, ref argattr3))
                    {
                        if (CommandState == "ターゲット選択")
                        {
                            CommandState = "マップ攻撃使用";
                        }
                        else
                        {
                            CommandState = "移動後マップ攻撃使用";
                        }

                        // 目標地点
                        SelectedX = GUI.PixelToMapX(GUI.MouseX);
                        SelectedY = GUI.PixelToMapY(GUI.MouseY);

                        // 効果範囲を設定
                        string argattr2 = "Ｍ投";
                        string arguparty = "味方";
                        Map.AreaInRange(SelectedX, SelectedY, withBlock.AbilityLevel(SelectedAbility, ref argattr2), 1, ref arguparty);
                        GUI.MaskScreen();
                        return;
                    }
                    else if (withBlock.IsAbilityClassifiedAs(SelectedAbility, ref argattr4))
                    {
                        SelectedX = GUI.PixelToMapX(GUI.MouseX);
                        SelectedY = GUI.PixelToMapY(GUI.MouseY);
                        if (Map.MapDataForUnit[SelectedX, SelectedY] is object)
                        {
                            GUI.MaskScreen();
                            return;
                        }

                        // 目標地点
                        if (CommandState == "ターゲット選択")
                        {
                            CommandState = "マップ攻撃使用";
                        }
                        else
                        {
                            CommandState = "移動後マップ攻撃使用";
                        }

                        // 効果範囲を設定
                        Map.AreaInPointToPoint(withBlock.x, withBlock.y, SelectedX, SelectedY);
                        GUI.MaskScreen();
                        return;
                    }
                    else if (withBlock.IsAbilityClassifiedAs(SelectedAbility, ref argattr5))
                    {
                        if (CommandState == "ターゲット選択")
                        {
                            CommandState = "マップ攻撃使用";
                        }
                        else
                        {
                            CommandState = "移動後マップ攻撃使用";
                        }

                        // 目標地点
                        SelectedX = GUI.PixelToMapX(GUI.MouseX);
                        SelectedY = GUI.PixelToMapY(GUI.MouseY);

                        // 効果範囲を設定
                        Map.AreaInPointToPoint(withBlock.x, withBlock.y, SelectedX, SelectedY);
                        GUI.MaskScreen();
                        return;
                    }
                }

                // 合体技パートナーの設定
                string argattr6 = "合";
                if (withBlock.IsAbilityClassifiedAs(SelectedAbility, ref argattr6))
                {
                    string argctype_Renamed = "アビリティ";
                    withBlock.CombinationPartner(ref argctype_Renamed, SelectedAbility, ref partners);
                }
                else
                {
                    SelectedPartners = new Unit[1];
                    partners = new Unit[1];
                }
            }

            if (GUI.MainWidth != 15)
            {
                Status.ClearUnitStatus();
            }

            GUI.LockGUI();

            // アビリティを実行
            SelectedUnit.ExecuteMapAbility(SelectedAbility, SelectedX, SelectedY);
            SelectedUnit = SelectedUnit.CurrentForm();
            // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            SelectedTarget = null;
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                SelectedPartners = new Unit[1];
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                SelectedPartners = new Unit[1];
                WaitCommand();
                return;
            }

            // 合体技のパートナーの行動数を減らす
            string argoname = "合体技パートナー行動数無消費";
            if (!Expression.IsOptionDefined(ref argoname))
            {
                var loopTo = Information.UBound(partners);
                for (i = 1; i <= loopTo; i++)
                    partners[i].CurrentForm().UseAction();
            }

            SelectedPartners = new Unit[1];

            // ADD START MARGE
            // 再移動
            if (is_p_ability & SelectedUnit.Status_Renamed == "出撃")
            {
                string argsname = "遊撃";
                if (SelectedUnit.MainPilot().IsSkillAvailable(ref argsname) & SelectedUnit.Speed * 2 > SelectedUnitMoveCost)
                {
                    // 進入イベント
                    if (SelectedUnitMoveCost > 0)
                    {
                        Event_Renamed.HandleEvent("進入", SelectedUnit.MainPilot().ID, SelectedUnit.x, SelectedUnit.y);
                        if (SRC.IsScenarioFinished)
                        {
                            SRC.IsScenarioFinished = false;
                            return;
                        }
                    }

                    // ユニットが既に出撃していない？
                    if (SelectedUnit.Status_Renamed != "出撃")
                    {
                        GUI.RedrawScreen();
                        Status.ClearUnitStatus();
                        return;
                    }

                    SelectedCommand = "再移動";
                    Map.AreaInSpeed(ref SelectedUnit);
                    string argoname1 = "大型マップ";
                    if (!Expression.IsOptionDefined(ref argoname1))
                    {
                        GUI.Center(SelectedUnit.x, SelectedUnit.y);
                    }

                    GUI.MaskScreen();
                    if (GUI.NewGUIMode)
                    {
                        Application.DoEvents();
                        Status.ClearUnitStatus();
                    }
                    else
                    {
                        Status.DisplayUnitStatus(ref SelectedUnit);
                    }

                    CommandState = "ターゲット選択";
                    return;
                }
            }
            // ADD END MARGE

            // 行動終了
            WaitCommand();
        }


        // スペシャルパワーコマンドを開始
        // MOD START MARGE
        // Public Sub StartSpecialPowerCommand()
        private void StartSpecialPowerCommand()
        {
            // MOD END MARGE
            int n, i, j, ret;
            string[] pname_list;
            string[] pid_list;
            string[] sp_list;
            string[] list;
            string[] id_list;
            string sname;
            SpecialPowerData sd;
            Unit u;
            Pilot p;
            var strkey_list = default(string[]);
            int max_item;
            string max_str;
            string buf;
            bool found;
            GUI.LockGUI();
            SelectedCommand = "スペシャルパワー";
            {
                var withBlock = SelectedUnit;
                pname_list = new string[1];
                pid_list = new string[1];
                GUI.ListItemFlag = new bool[1];

                // スペシャルパワーを使用可能なパイロットの一覧を作成
                n = (withBlock.CountPilot() + withBlock.CountSupport());
                string argfname = "追加サポート";
                if (withBlock.IsFeatureAvailable(ref argfname))
                {
                    n = (n + 1);
                }

                var loopTo = n;
                for (i = 1; i <= loopTo; i++)
                {
                    if (i <= withBlock.CountPilot())
                    {
                        // メインパイロット＆サブパイロット
                        object argIndex1 = i;
                        p = withBlock.Pilot(ref argIndex1);
                        if (i == 1)
                        {
                            // １番目のパイロットの場合はメインパイロットを使用
                            p = withBlock.MainPilot();

                            // ただし２人乗り以上のユニットで、メインパイロットが
                            // スペシャルパワーを持たない場合はそのまま１番目のパイロットを使用
                            if (withBlock.CountPilot() > 1)
                            {
                                object argIndex3 = 1;
                                if (p.Data.SP <= 0 & withBlock.Pilot(ref argIndex3).Data.SP > 0)
                                {
                                    object argIndex2 = 1;
                                    p = withBlock.Pilot(ref argIndex2);
                                }
                            }

                            // サブパイロットがメインパイロットを勤めている場合も
                            // １番目のパイロットを使用
                            var loopTo1 = withBlock.CountPilot();
                            for (j = 2; j <= loopTo1; j++)
                            {
                                Pilot localPilot() { object argIndex1 = j; var ret = withBlock.Pilot(ref argIndex1); return ret; }

                                if (ReferenceEquals(p, localPilot()))
                                {
                                    object argIndex4 = 1;
                                    p = withBlock.Pilot(ref argIndex4);
                                    break;
                                }
                            }
                        }
                        else
                        {
                        }

                        if (p.CountSpecialPower == 0)
                        {
                            goto NextPilot;
                        }

                        Array.Resize(ref pname_list, Information.UBound(pname_list) + 1 + 1);
                        Array.Resize(ref pid_list, Information.UBound(pname_list) + 1 + 1);
                        Array.Resize(ref GUI.ListItemFlag, Information.UBound(pname_list) + 1);
                        GUI.ListItemFlag[Information.UBound(pname_list)] = false;
                        pid_list[Information.UBound(pname_list)] = p.ID;
                        string localRightPaddedString() { string argbuf = p.get_Nickname(false); var ret = GeneralLib.RightPaddedString(ref argbuf, 17); p.get_Nickname(false) = argbuf; return ret; }

                        string localRightPaddedString1() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.SP) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.MaxSP); var ret = GeneralLib.RightPaddedString(ref argbuf, 8); return ret; }

                        pname_list[Information.UBound(pname_list)] = localRightPaddedString() + localRightPaddedString1();
                        var loopTo2 = p.CountSpecialPower;
                        for (j = 1; j <= loopTo2; j++)
                        {
                            sname = p.get_SpecialPower(j);
                            if (p.SP >= p.SpecialPowerCost(ref sname))
                            {
                                SpecialPowerData localItem() { object argIndex1 = sname; var ret = SRC.SPDList.Item(ref argIndex1); return ret; }

                                pname_list[Information.UBound(pname_list)] = pname_list[Information.UBound(pname_list)] + localItem().intName;
                            }
                        }
                    }
                    else if (i <= (withBlock.CountPilot() + withBlock.CountSupport()))
                    {
                        // サポートパイロット
                        object argIndex5 = i - withBlock.CountPilot();
                        {
                            var withBlock2 = withBlock.Support(ref argIndex5);
                            if (withBlock2.CountSpecialPower == 0)
                            {
                                goto NextPilot;
                            }

                            Array.Resize(ref pname_list, Information.UBound(pname_list) + 1 + 1);
                            Array.Resize(ref pid_list, Information.UBound(pname_list) + 1 + 1);
                            Array.Resize(ref GUI.ListItemFlag, Information.UBound(pname_list) + 1);
                            GUI.ListItemFlag[Information.UBound(pname_list)] = false;
                            pid_list[Information.UBound(pname_list)] = withBlock2.ID;
                            string localRightPaddedString4() { string argbuf = withBlock2.get_Nickname(false); var ret = GeneralLib.RightPaddedString(ref argbuf, 17); withBlock2.get_Nickname(false) = argbuf; return ret; }

                            string localRightPaddedString5() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock2.SP) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock2.MaxSP); var ret = GeneralLib.RightPaddedString(ref argbuf, 8); return ret; }

                            pname_list[Information.UBound(pname_list)] = localRightPaddedString4() + localRightPaddedString5();
                            var loopTo4 = withBlock2.CountSpecialPower;
                            for (j = 1; j <= loopTo4; j++)
                            {
                                sname = withBlock2.get_SpecialPower(j);
                                if (withBlock2.SP >= withBlock2.SpecialPowerCost(ref sname))
                                {
                                    SpecialPowerData localItem2() { object argIndex1 = sname; var ret = SRC.SPDList.Item(ref argIndex1); return ret; }

                                    pname_list[Information.UBound(pname_list)] = pname_list[Information.UBound(pname_list)] + localItem2().intName;
                                }
                            }
                        }
                    }
                    else
                    {
                        // 追加サポートパイロット
                        {
                            var withBlock1 = withBlock.AdditionalSupport();
                            if (withBlock1.CountSpecialPower == 0)
                            {
                                goto NextPilot;
                            }

                            Array.Resize(ref pname_list, Information.UBound(pname_list) + 1 + 1);
                            Array.Resize(ref pid_list, Information.UBound(pname_list) + 1 + 1);
                            Array.Resize(ref GUI.ListItemFlag, Information.UBound(pname_list) + 1);
                            GUI.ListItemFlag[Information.UBound(pname_list)] = false;
                            pid_list[Information.UBound(pname_list)] = withBlock1.ID;
                            string localRightPaddedString2() { string argbuf = withBlock1.get_Nickname(false); var ret = GeneralLib.RightPaddedString(ref argbuf, 17); withBlock1.get_Nickname(false) = argbuf; return ret; }

                            string localRightPaddedString3() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.SP) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MaxSP); var ret = GeneralLib.RightPaddedString(ref argbuf, 8); return ret; }

                            pname_list[Information.UBound(pname_list)] = localRightPaddedString2() + localRightPaddedString3();
                            var loopTo3 = withBlock1.CountSpecialPower;
                            for (j = 1; j <= loopTo3; j++)
                            {
                                sname = withBlock1.get_SpecialPower(j);
                                if (withBlock1.SP >= withBlock1.SpecialPowerCost(ref sname))
                                {
                                    SpecialPowerData localItem1() { object argIndex1 = sname; var ret = SRC.SPDList.Item(ref argIndex1); return ret; }

                                    pname_list[Information.UBound(pname_list)] = pname_list[Information.UBound(pname_list)] + localItem1().intName;
                                }
                            }
                        }
                    }

                NextPilot:
                    ;
                }

                GUI.TopItem = 1;
                if (Information.UBound(pname_list) > 1)
                {
                    // どのパイロットを使うか選択
                    string argoname = "等身大基準";
                    if (Expression.IsOptionDefined(ref argoname))
                    {
                        string arglb_caption = "キャラクター選択";
                        string argtname = "SP";
                        string argtname1 = "SP";
                        string arglb_info = "キャラクター     " + Expression.Term(ref argtname, ref SelectedUnit, 2) + "/Max" + Expression.Term(ref argtname1, ref SelectedUnit, 2);
                        string arglb_mode = "連続表示,カーソル移動";
                        i = GUI.ListBox(ref arglb_caption, ref pname_list, ref arglb_info, ref arglb_mode);
                    }
                    else
                    {
                        string arglb_caption1 = "パイロット選択";
                        string argtname2 = "SP";
                        string argtname3 = "SP";
                        string arglb_info1 = "パイロット       " + Expression.Term(ref argtname2, ref SelectedUnit, 2) + "/Max" + Expression.Term(ref argtname3, ref SelectedUnit, 2);
                        string arglb_mode1 = "連続表示,カーソル移動";
                        i = GUI.ListBox(ref arglb_caption1, ref pname_list, ref arglb_info1, ref arglb_mode1);
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
                    My.MyProject.Forms.frmListBox.Hide();
                    if (SRC.AutoMoveCursor)
                    {
                        GUI.RestoreCursorPos();
                    }

                    GUI.UnlockGUI();
                    CancelCommand();
                    return;
                }

                // スペシャルパワーを使うパイロットを設定
                var tmp = pid_list;
                object argIndex6 = tmp[i];
                SelectedPilot = SRC.PList.Item(ref argIndex6);
                // そのパイロットのステータスを表示
                if (Information.UBound(pname_list) > 1)
                {
                    Status.DisplayPilotStatus(SelectedPilot);
                }
            }

            {
                var withBlock3 = SelectedPilot;
                // 使用可能なスペシャルパワーの一覧を作成
                sp_list = new string[(withBlock3.CountSpecialPower + 1)];
                GUI.ListItemFlag = new bool[(withBlock3.CountSpecialPower + 1)];
                var loopTo5 = withBlock3.CountSpecialPower;
                for (i = 1; i <= loopTo5; i++)
                {
                    sname = withBlock3.get_SpecialPower(i);
                    string localLeftPaddedString() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock3.SpecialPowerCost(ref sname)); var ret = GeneralLib.LeftPaddedString(ref argbuf, 3); return ret; }

                    SpecialPowerData localItem3() { object argIndex1 = sname; var ret = SRC.SPDList.Item(ref argIndex1); return ret; }

                    sp_list[i] = GeneralLib.RightPaddedString(ref sname, 13) + localLeftPaddedString() + "  " + localItem3().Comment;
                    if (withBlock3.SP >= withBlock3.SpecialPowerCost(ref sname))
                    {
                        if (!withBlock3.IsSpecialPowerUseful(ref sname))
                        {
                            GUI.ListItemFlag[i] = true;
                        }
                    }
                    else
                    {
                        GUI.ListItemFlag[i] = true;
                    }
                }
            }

            // どのコマンドを使用するかを選択
            {
                var withBlock4 = SelectedPilot;
                GUI.TopItem = 1;
                string argtname4 = "スペシャルパワー";
                string arglb_caption2 = Expression.Term(ref argtname4, ref SelectedUnit) + "選択";
                string argtname5 = "SP";
                string argtname6 = "SP";
                string arglb_info2 = "名称         消費" + Expression.Term(ref argtname5, ref SelectedUnit) + "（" + withBlock4.get_Nickname(false) + " " + Expression.Term(ref argtname6, ref SelectedUnit) + "=" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.SP) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.MaxSP) + "）";
                string arglb_mode2 = "カーソル移動(行きのみ)";
                i = GUI.ListBox(ref arglb_caption2, ref sp_list, ref arglb_info2, ref arglb_mode2);
            }

            // キャンセル
            if (i == 0)
            {
                Status.DisplayUnitStatus(ref SelectedUnit);
                // カーソル自動移動
                if (SRC.AutoMoveCursor)
                {
                    string argcursor_mode = "ユニット選択";
                    GUI.MoveCursorPos(ref argcursor_mode, SelectedUnit);
                }

                GUI.UnlockGUI();
                CancelCommand();
                return;
            }

            // 使用するスペシャルパワーを設定
            SelectedSpecialPower = SelectedPilot.get_SpecialPower(i);

            // 味方スペシャルパワー実行の効果により他のパイロットが持っているスペシャルパワーを
            // 使う場合は記録しておき、後で消費ＳＰを倍にする必要がある
            SpecialPowerData localItem5() { object argIndex1 = SelectedSpecialPower; var ret = SRC.SPDList.Item(ref argIndex1); return ret; }

            if (localItem5().EffectType(1) == "味方スペシャルパワー実行")
            {
                // スペシャルパワー一覧
                list = new string[1];
                var loopTo6 = SRC.SPDList.Count();
                for (i = 1; i <= loopTo6; i++)
                {
                    object argIndex7 = i;
                    {
                        var withBlock5 = SRC.SPDList.Item(ref argIndex7);
                        if (withBlock5.EffectType(1) != "味方スペシャルパワー実行" & withBlock5.intName != "非表示")
                        {
                            Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                            Array.Resize(ref strkey_list, Information.UBound(list) + 1);
                            list[Information.UBound(list)] = withBlock5.Name;
                            strkey_list[Information.UBound(list)] = withBlock5.KanaName;
                        }
                    }
                }

                GUI.ListItemFlag = new bool[Information.UBound(list) + 1];

                // ソート
                var loopTo7 = (Information.UBound(strkey_list) - 1);
                for (i = 1; i <= loopTo7; i++)
                {
                    max_item = i;
                    max_str = strkey_list[i];
                    var loopTo8 = Information.UBound(strkey_list);
                    for (j = (i + 1); j <= loopTo8; j++)
                    {
                        if (Strings.StrComp(strkey_list[j], max_str, (CompareMethod)1) == -1)
                        {
                            max_item = j;
                            max_str = strkey_list[j];
                        }
                    }

                    if (max_item != i)
                    {
                        buf = list[i];
                        list[i] = list[max_item];
                        list[max_item] = buf;
                        buf = strkey_list[i];
                        strkey_list[i] = max_str;
                        strkey_list[max_item] = buf;
                    }
                }

                // スペシャルパワーを使用可能なパイロットがいるかどうかを判定
                var loopTo9 = Information.UBound(list);
                for (i = 1; i <= loopTo9; i++)
                {
                    GUI.ListItemFlag[i] = true;
                    foreach (Pilot currentP in SRC.PList)
                    {
                        p = currentP;
                        if (p.Party == "味方")
                        {
                            if (p.Unit_Renamed is object)
                            {
                                object argIndex8 = "憑依";
                                if (p.Unit_Renamed.Status_Renamed == "出撃" & !p.Unit_Renamed.IsConditionSatisfied(ref argIndex8))
                                {
                                    // 本当に乗っている？
                                    found = false;
                                    {
                                        var withBlock6 = p.Unit_Renamed;
                                        if (ReferenceEquals(p, withBlock6.MainPilot()))
                                        {
                                            found = true;
                                        }
                                        else
                                        {
                                            var loopTo10 = withBlock6.CountPilot();
                                            for (j = 2; j <= loopTo10; j++)
                                            {
                                                Pilot localPilot1() { object argIndex1 = j; var ret = withBlock6.Pilot(ref argIndex1); return ret; }

                                                if (ReferenceEquals(p, localPilot1()))
                                                {
                                                    found = true;
                                                    break;
                                                }
                                            }

                                            var loopTo11 = withBlock6.CountSupport();
                                            for (j = 1; j <= loopTo11; j++)
                                            {
                                                Pilot localSupport() { object argIndex1 = j; var ret = withBlock6.Support(ref argIndex1); return ret; }

                                                if (ReferenceEquals(p, localSupport()))
                                                {
                                                    found = true;
                                                    break;
                                                }
                                            }

                                            if (ReferenceEquals(p, withBlock6.AdditionalSupport()))
                                            {
                                                found = true;
                                            }
                                        }
                                    }

                                    if (found)
                                    {
                                        if (p.IsSpecialPowerAvailable(ref list[i]))
                                        {
                                            GUI.ListItemFlag[i] = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // 各スペシャルパワーが使用可能か判定
                {
                    var withBlock7 = SelectedPilot;
                    var loopTo12 = Information.UBound(list);
                    for (i = 1; i <= loopTo12; i++)
                    {
                        if (!GUI.ListItemFlag[i] & withBlock7.SP >= 2 * withBlock7.SpecialPowerCost(ref list[i]))
                        {
                            if (!withBlock7.IsSpecialPowerUseful(ref list[i]))
                            {
                                GUI.ListItemFlag[i] = true;
                            }
                        }
                        else
                        {
                            GUI.ListItemFlag[i] = true;
                        }
                    }
                }

                // スペシャルパワーの解説を設定
                GUI.ListItemComment = new string[Information.UBound(list) + 1];
                var loopTo13 = Information.UBound(list);
                for (i = 1; i <= loopTo13; i++)
                {
                    SpecialPowerData localItem4() { var tmp = list; object argIndex1 = tmp[i]; var ret = SRC.SPDList.Item(ref argIndex1); return ret; }

                    GUI.ListItemComment[i] = localItem4().Comment;
                }

                // 検索するスペシャルパワーを選択
                GUI.TopItem = 1;
                string argtname7 = "スペシャルパワー";
                Unit argu = null;
                string arglb_caption3 = Expression.Term(ref argtname7, u: ref argu) + "検索";
                ret = GUI.MultiColumnListBox(ref arglb_caption3, ref list, true);
                if (ret == 0)
                {
                    SelectedSpecialPower = "";
                    CancelCommand();
                    GUI.UnlockGUI();
                    return;
                }

                // スペシャルパワー使用メッセージ
                if (SelectedUnit.IsMessageDefined(ref SelectedSpecialPower))
                {
                    Unit argu1 = null;
                    Unit argu2 = null;
                    GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                    string argmsg_mode = "";
                    SelectedUnit.PilotMessage(ref SelectedSpecialPower, msg_mode: ref argmsg_mode);
                    GUI.CloseMessageForm();
                }

                SelectedSpecialPower = list[ret];
                WithDoubleSPConsumption = true;
            }
            else
            {
                WithDoubleSPConsumption = false;
            }

            object argIndex9 = SelectedSpecialPower;
            sd = SRC.SPDList.Item(ref argIndex9);

            // ターゲットを選択する必要があるスペシャルパワーの場合
            switch (sd.TargetType ?? "")
            {
                case "味方":
                case "敵":
                case "任意":
                    {
                        // マップ上のユニットからターゲットを選択する

                        Unit argu11 = null;
                        Unit argu21 = null;
                        GUI.OpenMessageForm(u1: ref argu11, u2: ref argu21);
                        GUI.DisplaySysMessage(SelectedPilot.get_Nickname(false) + "は" + SelectedSpecialPower + "を使った。;" + "ターゲットを選んでください。");
                        GUI.CloseMessageForm();

                        // ターゲットのエリアを設定
                        var loopTo14 = Map.MapWidth;
                        for (i = 1; i <= loopTo14; i++)
                        {
                            var loopTo15 = Map.MapHeight;
                            for (j = 1; j <= loopTo15; j++)
                            {
                                Map.MaskData[i, j] = true;
                                u = Map.MapDataForUnit[i, j];
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
                                if (!sd.Effective(ref SelectedPilot, ref u))
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
                    {
                        // 破壊された味方ユニットの中からターゲットを選択する

                        Unit argu12 = null;
                        Unit argu22 = null;
                        GUI.OpenMessageForm(u1: ref argu12, u2: ref argu22);
                        GUI.DisplaySysMessage(SelectedPilot.get_Nickname(false) + "は" + SelectedSpecialPower + "を使った。;" + "復活させるユニットを選んでください。");
                        GUI.CloseMessageForm();

                        // 破壊された味方ユニットのリストを作成
                        list = new string[1];
                        id_list = new string[1];
                        GUI.ListItemFlag = new bool[1];
                        foreach (Unit currentU in SRC.UList)
                        {
                            u = currentU;
                            if (u.Party0 == "味方" & u.Status_Renamed == "破壊" & (u.CountPilot() > 0 | u.Data.PilotNum == 0))
                            {
                                Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                                Array.Resize(ref id_list, Information.UBound(list) + 1);
                                Array.Resize(ref GUI.ListItemFlag, Information.UBound(list) + 1);
                                string localRightPaddedString6() { string argbuf = u.Nickname; var ret = GeneralLib.RightPaddedString(ref argbuf, 28); u.Nickname = argbuf; return ret; }

                                string localRightPaddedString7() { string argbuf = u.MainPilot().get_Nickname(false); var ret = GeneralLib.RightPaddedString(ref argbuf, 18); u.MainPilot().get_Nickname(false) = argbuf; return ret; }

                                string localLeftPaddedString1() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(u.MainPilot().Level); var ret = GeneralLib.LeftPaddedString(ref argbuf, 3); return ret; }

                                list[Information.UBound(list)] = localRightPaddedString6() + localRightPaddedString7() + localLeftPaddedString1();
                                id_list[Information.UBound(list)] = u.ID;
                                GUI.ListItemFlag[Information.UBound(list)] = false;
                            }
                        }

                        GUI.TopItem = 1;
                        string arglb_caption4 = "ユニット選択";
                        string arglb_info3 = "ユニット名                  パイロット     レベル";
                        string arglb_mode3 = "";
                        i = GUI.ListBox(ref arglb_caption4, ref list, ref arglb_info3, lb_mode: ref arglb_mode3);
                        if (i == 0)
                        {
                            GUI.UnlockGUI();
                            CancelCommand();
                            return;
                        }

                        var tmp1 = id_list;
                        object argIndex10 = tmp1[i];
                        SelectedTarget = SRC.UList.Item(ref argIndex10);
                        break;
                    }
            }

            // 自爆を選択した場合は確認を取る
            // UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(自爆) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            string argename = "自爆";
            if (Conversions.ToBoolean(sd.IsEffectAvailable(ref argename)))
            {
                ret = Interaction.MsgBox("自爆させますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "自爆");
                if (ret == MsgBoxResult.Cancel)
                {
                    GUI.UnlockGUI();
                    return;
                }
            }

            // 使用イベント
            Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, SelectedSpecialPower);
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
                SelectedPilot.UseSpecialPower(ref SelectedSpecialPower, 2d);
            }
            else
            {
                SelectedPilot.UseSpecialPower(ref SelectedSpecialPower);
            }

            SelectedUnit = SelectedUnit.CurrentForm();

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                string argcursor_mode1 = "ユニット選択";
                GUI.MoveCursorPos(ref argcursor_mode1, SelectedUnit);
            }

            // ステータスウィンドウを更新
            Status.DisplayUnitStatus(ref SelectedUnit);

            // 使用後イベント
            Event_Renamed.HandleEvent("使用後", SelectedUnit.MainPilot().ID, SelectedSpecialPower);
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
        // MOD START MARGE
        // Public Sub FinishSpecialPowerCommand()
        private void FinishSpecialPowerCommand()
        {
            // MOD END MARGE
            int i, ret;
            GUI.LockGUI();

            // 自爆を選択した場合は確認を取る
            object argIndex1 = SelectedSpecialPower;
            {
                var withBlock = SRC.SPDList.Item(ref argIndex1);
                var loopTo = withBlock.CountEffect();
                for (i = 1; i <= loopTo; i++)
                {
                    if (withBlock.EffectType(i) == "自爆")
                    {
                        ret = Interaction.MsgBox("自爆させますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "自爆");
                        if (ret == MsgBoxResult.Cancel)
                        {
                            CommandState = "ユニット選択";
                            GUI.UnlockGUI();
                            return;
                        }

                        break;
                    }
                }
            }

            // 使用イベント
            Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, SelectedSpecialPower);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            // スペシャルパワーを使用
            if (WithDoubleSPConsumption)
            {
                SelectedPilot.UseSpecialPower(ref SelectedSpecialPower, 2d);
            }
            else
            {
                SelectedPilot.UseSpecialPower(ref SelectedSpecialPower);
            }

            SelectedUnit = SelectedUnit.CurrentForm();

            // ステータスウィンドウを更新
            if (SelectedTarget is object)
            {
                if (SelectedTarget.CurrentForm().Status_Renamed == "出撃")
                {
                    Status.DisplayUnitStatus(ref SelectedTarget);
                }
            }

            // 使用後イベント
            Event_Renamed.HandleEvent("使用後", SelectedUnit.MainPilot().ID, SelectedSpecialPower);
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


        // 「修理」コマンドを開始
        // MOD START MARGE
        // Public Sub StartFixCommand()
        private void StartFixCommand()
        {
            // MOD END MARGE
            int j, i, k;
            Unit t;
            string fname;
            SelectedCommand = "修理";

            // 射程範囲？を表示
            {
                var withBlock = SelectedUnit;
                string arguparty = "味方";
                Map.AreaInRange(withBlock.x, withBlock.y, 1, 1, ref arguparty);
                var loopTo = Map.MapWidth;
                for (i = 1; i <= loopTo; i++)
                {
                    var loopTo1 = Map.MapHeight;
                    for (j = 1; j <= loopTo1; j++)
                    {
                        if (!Map.MaskData[i, j] & Map.MapDataForUnit[i, j] is object)
                        {
                            {
                                var withBlock1 = Map.MapDataForUnit[i, j];
                                object argIndex1 = "ゾンビ";
                                if (withBlock1.HP == withBlock1.MaxHP | withBlock1.IsConditionSatisfied(ref argIndex1))
                                {
                                    Map.MaskData[i, j] = true;
                                }

                                string argfname = "修理不可";
                                if (withBlock1.IsFeatureAvailable(ref argfname))
                                {
                                    object argIndex5 = "修理不可";
                                    object argIndex6 = "修理不可";
                                    var loopTo2 = Conversions.ToInteger(withBlock1.FeatureData(ref argIndex6));
                                    for (k = 2; k <= loopTo2; k++)
                                    {
                                        object argIndex2 = "修理不可";
                                        string arglist = withBlock1.FeatureData(ref argIndex2);
                                        fname = GeneralLib.LIndex(ref arglist, k);
                                        if (Strings.Left(fname, 1) == "!")
                                        {
                                            fname = Strings.Mid(fname, 2);
                                            object argIndex3 = "修理装置";
                                            if ((fname ?? "") != (SelectedUnit.FeatureName0(ref argIndex3) ?? ""))
                                            {
                                                Map.MaskData[i, j] = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            object argIndex4 = "修理装置";
                                            if ((fname ?? "") == (SelectedUnit.FeatureName0(ref argIndex4) ?? ""))
                                            {
                                                Map.MaskData[i, j] = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Map.MaskData[withBlock.x, withBlock.y] = false;
            }

            GUI.MaskScreen();

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                // UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                t = null;
                foreach (Unit u in SRC.UList)
                {
                    if (u.Status_Renamed == "出撃" & u.Party == "味方")
                    {
                        if (Map.MaskData[u.x, u.y] == false & !ReferenceEquals(u, SelectedUnit))
                        {
                            if (t is null)
                            {
                                t = u;
                            }
                            else if (u.MaxHP - u.HP > t.MaxHP - t.HP)
                            {
                                t = u;
                            }
                        }
                    }
                }

                if (t is null)
                {
                    t = SelectedUnit;
                }

                string argcursor_mode = "ユニット選択";
                GUI.MoveCursorPos(ref argcursor_mode, t);
                if (!ReferenceEquals(SelectedUnit, t))
                {
                    Status.DisplayUnitStatus(ref t);
                }
            }

            if (CommandState == "コマンド選択")
            {
                CommandState = "ターゲット選択";
            }
            else
            {
                CommandState = "移動後ターゲット選択";
            }
        }

        // 「修理」コマンドを終了
        // MOD START MARGE
        // Public Sub FinishFixCommand()
        private void FinishFixCommand()
        {
            // MOD END MARGE
            int tmp;
            GUI.LockGUI();
            GUI.OpenMessageForm(ref SelectedTarget, ref SelectedUnit);
            {
                var withBlock = SelectedUnit;
                // 選択内容を変更
                Event_Renamed.SelectedUnitForEvent = SelectedUnit;
                Event_Renamed.SelectedTargetForEvent = SelectedTarget;

                // 修理メッセージ＆特殊効果
                string argmain_situation = "修理";
                if (withBlock.IsMessageDefined(ref argmain_situation))
                {
                    string argSituation = "修理";
                    string argmsg_mode = "";
                    withBlock.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                }

                string argmain_situation3 = "修理";
                object argIndex3 = "修理装置";
                string argsub_situation2 = withBlock.FeatureName(ref argIndex3);
                if (withBlock.IsAnimationDefined(ref argmain_situation3, ref argsub_situation2))
                {
                    string argmain_situation1 = "修理";
                    object argIndex1 = "修理装置";
                    string argsub_situation = withBlock.FeatureName(ref argIndex1);
                    withBlock.PlayAnimation(ref argmain_situation1, ref argsub_situation);
                }
                else
                {
                    string argmain_situation2 = "修理";
                    object argIndex2 = "修理装置";
                    string argsub_situation1 = withBlock.FeatureName(ref argIndex2);
                    withBlock.SpecialEffect(ref argmain_situation2, ref argsub_situation1);
                }

                object argIndex4 = "修理装置";
                GUI.DisplaySysMessage(withBlock.Nickname + "は" + SelectedTarget.Nickname + "に" + withBlock.FeatureName(ref argIndex4) + "を使った。");

                // 修理を実行
                tmp = SelectedTarget.HP;
                object argIndex7 = "修理装置";
                switch (withBlock.FeatureLevel(ref argIndex7))
                {
                    case 1d:
                    case -1:
                        {
                            object argIndex5 = "修理";
                            string argref_mode = "";
                            SelectedTarget.RecoverHP(30d + 3d * SelectedUnit.MainPilot().SkillLevel(ref argIndex5, ref_mode: ref argref_mode));
                            break;
                        }

                    case 2d:
                        {
                            object argIndex6 = "修理";
                            string argref_mode1 = "";
                            SelectedTarget.RecoverHP(50d + 5d * SelectedUnit.MainPilot().SkillLevel(ref argIndex6, ref_mode: ref argref_mode1));
                            break;
                        }

                    case 3d:
                        {
                            SelectedTarget.RecoverHP(100d);
                            break;
                        }
                }

                string localLIndex2() { object argIndex1 = "修理装置"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                if (Information.IsNumeric(localLIndex2()))
                {
                    string localLIndex() { object argIndex1 = "修理装置"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    string localLIndex1() { object argIndex1 = "修理装置"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    withBlock.EN = withBlock.EN - Conversions.Toint(localLIndex1());
                }

                string argmsg = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SelectedTarget.HP - tmp);
                GUI.DrawSysString(SelectedTarget.x, SelectedTarget.y, ref argmsg);
                object argu2 = SelectedUnit;
                GUI.UpdateMessageForm(ref SelectedTarget, ref argu2);
                string argtname = "ＨＰ";
                GUI.DisplaySysMessage(SelectedTarget.Nickname + "の" + Expression.Term(ref argtname, ref SelectedTarget) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SelectedTarget.HP - tmp) + "回復した。");

                // 経験値獲得
                string argexp_situation = "修理";
                string argexp_mode = "";
                withBlock.GetExp(ref SelectedTarget, ref argexp_situation, exp_mode: ref argexp_mode);
                if (GUI.MessageWait < 10000)
                {
                    GUI.Sleep(GUI.MessageWait);
                }
            }

            GUI.CloseMessageForm();

            // 形態変化のチェック
            SelectedTarget.Update();
            SelectedTarget.CurrentForm().CheckAutoHyperMode();
            SelectedTarget.CurrentForm().CheckAutoNormalMode();

            // 行動終了
            WaitCommand();
        }


        // 「補給」コマンドを開始
        // MOD START MARGE
        // Public Sub StartSupplyCommand()
        private void StartSupplyCommand()
        {
            // MOD END MARGE
            int j, i, k;
            Unit t;
            SelectedCommand = "補給";

            // 射程範囲？を表示
            {
                var withBlock = SelectedUnit;
                string arguparty = "味方";
                Map.AreaInRange(withBlock.x, withBlock.y, 1, 1, ref arguparty);
                var loopTo = Map.MapWidth;
                for (i = 1; i <= loopTo; i++)
                {
                    var loopTo1 = Map.MapHeight;
                    for (j = 1; j <= loopTo1; j++)
                    {
                        if (!Map.MaskData[i, j] & Map.MapDataForUnit[i, j] is object)
                        {
                            Map.MaskData[i, j] = true;
                            {
                                var withBlock1 = Map.MapDataForUnit[i, j];
                                object argIndex1 = "ゾンビ";
                                if (withBlock1.EN < withBlock1.MaxEN & !withBlock1.IsConditionSatisfied(ref argIndex1))
                                {
                                    Map.MaskData[i, j] = false;
                                }
                                else
                                {
                                    var loopTo2 = withBlock1.CountWeapon();
                                    for (k = 1; k <= loopTo2; k++)
                                    {
                                        if (withBlock1.Bullet(k) < withBlock1.MaxBullet(k))
                                        {
                                            Map.MaskData[i, j] = false;
                                            break;
                                        }
                                    }

                                    var loopTo3 = withBlock1.CountAbility();
                                    for (k = 1; k <= loopTo3; k++)
                                    {
                                        if (withBlock1.Stock(k) < withBlock1.MaxStock(k))
                                        {
                                            Map.MaskData[i, j] = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Map.MaskData[withBlock.x, withBlock.y] = false;
            }

            GUI.MaskScreen();

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                // UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                t = null;
                foreach (Unit u in SRC.UList)
                {
                    if (u.Status_Renamed == "出撃" & u.Party == "味方")
                    {
                        if (Map.MaskData[u.x, u.y] == false & !ReferenceEquals(u, SelectedUnit))
                        {
                            t = u;
                            break;
                        }
                    }
                }

                if (t is null)
                {
                    t = SelectedUnit;
                }

                string argcursor_mode = "ユニット選択";
                GUI.MoveCursorPos(ref argcursor_mode, t);
                if (!ReferenceEquals(SelectedUnit, t))
                {
                    Status.DisplayUnitStatus(ref t);
                }
            }

            if (CommandState == "コマンド選択")
            {
                CommandState = "ターゲット選択";
            }
            else
            {
                CommandState = "移動後ターゲット選択";
            }
        }

        // 「補給」コマンドを終了
        // MOD START MARGE
        // Public Sub FinishSupplyCommand()
        private void FinishSupplyCommand()
        {
            // MOD END MARGE

            GUI.LockGUI();
            GUI.OpenMessageForm(ref SelectedTarget, ref SelectedUnit);
            {
                var withBlock = SelectedUnit;
                // 選択内容を変更
                Event_Renamed.SelectedUnitForEvent = SelectedUnit;
                Event_Renamed.SelectedTargetForEvent = SelectedTarget;

                // 補給メッセージ＆特殊効果
                string argmain_situation = "補給";
                if (withBlock.IsMessageDefined(ref argmain_situation))
                {
                    string argSituation = "補給";
                    string argmsg_mode = "";
                    withBlock.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                }

                string argmain_situation3 = "補給";
                object argIndex3 = "補給装置";
                string argsub_situation2 = withBlock.FeatureName(ref argIndex3);
                if (withBlock.IsAnimationDefined(ref argmain_situation3, ref argsub_situation2))
                {
                    string argmain_situation1 = "補給";
                    object argIndex1 = "補給装置";
                    string argsub_situation = withBlock.FeatureName(ref argIndex1);
                    withBlock.PlayAnimation(ref argmain_situation1, ref argsub_situation);
                }
                else
                {
                    string argmain_situation2 = "補給";
                    object argIndex2 = "補給装置";
                    string argsub_situation1 = withBlock.FeatureName(ref argIndex2);
                    withBlock.SpecialEffect(ref argmain_situation2, ref argsub_situation1);
                }

                object argIndex4 = "補給装置";
                GUI.DisplaySysMessage(withBlock.Nickname + "は" + SelectedTarget.Nickname + "に" + withBlock.FeatureName(ref argIndex4) + "を使った。");

                // 補給を実施
                SelectedTarget.FullSupply();
                SelectedTarget.IncreaseMorale(-10);
                string localLIndex2() { object argIndex1 = "補給装置"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                if (Information.IsNumeric(localLIndex2()))
                {
                    string localLIndex() { object argIndex1 = "補給装置"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    string localLIndex1() { object argIndex1 = "補給装置"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    withBlock.EN = withBlock.EN - Conversions.Toint(localLIndex1());
                }

                object argu2 = SelectedUnit;
                GUI.UpdateMessageForm(ref SelectedTarget, ref argu2);
                string argtname = "ＥＮ";
                GUI.DisplaySysMessage(SelectedTarget.Nickname + "の弾数と" + Expression.Term(ref argtname, ref SelectedTarget) + "が全快した。");

                // 経験値を獲得
                string argexp_situation = "補給";
                string argexp_mode = "";
                withBlock.GetExp(ref SelectedTarget, ref argexp_situation, exp_mode: ref argexp_mode);
                if (GUI.MessageWait < 10000)
                {
                    GUI.Sleep(GUI.MessageWait);
                }
            }

            // 形態変化のチェック
            SelectedTarget.Update();
            SelectedTarget.CurrentForm().CheckAutoHyperMode();
            SelectedTarget.CurrentForm().CheckAutoNormalMode();
            GUI.CloseMessageForm();

            // 行動終了
            WaitCommand();
        }


        // 「チャージ」コマンド
        // MOD START MARGE
        // Public Sub ChargeCommand()
        private void ChargeCommand()
        {
            // MOD END MARGE
            int ret;
            Unit[] partners;
            int i;
            GUI.LockGUI();
            ret = Interaction.MsgBox("チャージを開始しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "チャージ開始");
            if (ret == MsgBoxResult.Cancel)
            {
                CancelCommand();
                GUI.UnlockGUI();
                return;
            }

            // 使用イベント
            Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, "チャージ");
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            {
                var withBlock = SelectedUnit;
                // チャージのメッセージを表示
                string argmain_situation = "チャージ";
                if (withBlock.IsMessageDefined(ref argmain_situation))
                {
                    Unit argu1 = null;
                    Unit argu2 = null;
                    GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                    string argSituation = "チャージ";
                    string argmsg_mode = "";
                    withBlock.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                    GUI.CloseMessageForm();
                }

                // アニメ表示を行う
                string argmain_situation3 = "チャージ";
                string argsub_situation2 = "";
                string argmain_situation4 = "チャージ";
                string argsub_situation3 = "";
                if (withBlock.IsAnimationDefined(ref argmain_situation3, sub_situation: ref argsub_situation2))
                {
                    string argmain_situation1 = "チャージ";
                    string argsub_situation = "";
                    withBlock.PlayAnimation(ref argmain_situation1, sub_situation: ref argsub_situation);
                }
                else if (withBlock.IsSpecialEffectDefined(ref argmain_situation4, sub_situation: ref argsub_situation3))
                {
                    string argmain_situation2 = "チャージ";
                    string argsub_situation1 = "";
                    withBlock.SpecialEffect(ref argmain_situation2, sub_situation: ref argsub_situation1);
                }
                else
                {
                    string argwave_name = "Charge.wav";
                    Sound.PlayWave(ref argwave_name);
                }

                // チャージ攻撃のパートナーを探す
                partners = new Unit[1];
                var loopTo = withBlock.CountWeapon();
                for (i = 1; i <= loopTo; i++)
                {
                    string argattr = "Ｃ";
                    string argattr1 = "合";
                    if (withBlock.IsWeaponClassifiedAs(i, ref argattr) & withBlock.IsWeaponClassifiedAs(i, ref argattr1))
                    {
                        string argref_mode = "チャージ";
                        if (withBlock.IsWeaponAvailable(i, ref argref_mode))
                        {
                            string argctype_Renamed = "武装";
                            withBlock.CombinationPartner(ref argctype_Renamed, i, ref partners);
                            break;
                        }
                    }
                }

                if (Information.UBound(partners) == 0)
                {
                    var loopTo1 = withBlock.CountAbility();
                    for (i = 1; i <= loopTo1; i++)
                    {
                        string argattr2 = "Ｃ";
                        string argattr3 = "合";
                        if (withBlock.IsAbilityClassifiedAs(i, ref argattr2) & withBlock.IsAbilityClassifiedAs(i, ref argattr3))
                        {
                            string argref_mode1 = "チャージ";
                            if (withBlock.IsAbilityAvailable(i, ref argref_mode1))
                            {
                                string argctype_Renamed1 = "アビリティ";
                                withBlock.CombinationPartner(ref argctype_Renamed1, i, ref partners);
                                break;
                            }
                        }
                    }
                }

                // ユニットの状態をチャージ中に
                string argcname = "チャージ";
                string argcdata = "";
                withBlock.AddCondition(ref argcname, 1, cdata: ref argcdata);

                // チャージ攻撃のパートナーもチャージ中にする
                var loopTo2 = Information.UBound(partners);
                for (i = 1; i <= loopTo2; i++)
                {
                    {
                        var withBlock1 = partners[i];
                        string argcname1 = "チャージ";
                        string argcdata1 = "";
                        withBlock1.AddCondition(ref argcname1, 1, cdata: ref argcdata1);
                    }
                }
            }

            // 使用後イベント
            Event_Renamed.HandleEvent("使用後", SelectedUnit.MainPilot().ID, "チャージ");
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            GUI.UnlockGUI();

            // 行動終了
            WaitCommand();
        }

        // 「会話」コマンドを開始
        // MOD START MARGE
        // Public Sub StartTalkCommand()
        private void StartTalkCommand()
        {
            // MOD END MARGE
            int i, j;
            Unit t;
            SelectedCommand = "会話";

            // UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            t = null;

            // 会話可能なユニットを表示
            {
                var withBlock = SelectedUnit;
                string arguparty = "";
                Map.AreaInRange(withBlock.x, withBlock.y, 1, 1, ref arguparty);
                var loopTo = Map.MapWidth;
                for (i = 1; i <= loopTo; i++)
                {
                    var loopTo1 = Map.MapHeight;
                    for (j = 1; j <= loopTo1; j++)
                    {
                        if (!Map.MaskData[i, j])
                        {
                            if (Map.MapDataForUnit[i, j] is object)
                            {
                                bool localIsEventDefined() { var arglname = "会話 " + withBlock.MainPilot().ID + " " + Map.MapDataForUnit[i, j].MainPilot.ID; var ret = Event_Renamed.IsEventDefined(ref arglname); return ret; }

                                if (!localIsEventDefined())
                                {
                                    Map.MaskData[i, j] = true;
                                    t = Map.MapDataForUnit[i, j];
                                }
                            }
                        }
                    }
                }

                Map.MaskData[withBlock.x, withBlock.y] = false;
            }

            GUI.MaskScreen();

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                if (t is object)
                {
                    string argcursor_mode = "ユニット選択";
                    GUI.MoveCursorPos(ref argcursor_mode, t);
                    Status.DisplayUnitStatus(ref t);
                }
            }

            if (CommandState == "コマンド選択")
            {
                CommandState = "ターゲット選択";
            }
            else
            {
                CommandState = "移動後ターゲット選択";
            }
        }

        // 「会話」コマンドを終了
        // MOD START MARGE
        // Public Sub FinishTalkCommand()
        private void FinishTalkCommand()
        {
            // MOD END MARGE
            Pilot p;
            GUI.LockGUI();
            if (SelectedUnit.CountPilot() > 0)
            {
                object argIndex1 = 1;
                p = SelectedUnit.Pilot(ref argIndex1);
            }
            else
            {
                // UPGRADE_NOTE: オブジェクト p をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                p = null;
            }

            // 会話イベントを実施
            Event_Renamed.HandleEvent("会話", SelectedUnit.MainPilot().ID, SelectedTarget.MainPilot().ID);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                return;
            }

            if (p is object)
            {
                if (p.Unit_Renamed is object)
                {
                    SelectedUnit = p.Unit_Renamed;
                }
            }

            GUI.UnlockGUI();

            // 行動終了
            WaitCommand();
        }

        // 「変形」コマンド
        // MOD START MARGE
        // Public Sub TransformCommand()
        private void TransformCommand()
        {
            // MOD END MARGE
            string[] list;
            string[] list_id;
            int i;
            int ret;
            string uname, fdata;
            string prev_uname;

            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Status.ClearUnitStatus();
            }

            GUI.LockGUI();
            object argIndex1 = "変形";
            fdata = SelectedUnit.FeatureData(ref argIndex1);
            if (string.IsNullOrEmpty(Map.MapFileName))
            {
                // ユニットステータスコマンドの場合
                {
                    var withBlock = SelectedUnit;
                    list = new string[1];
                    list_id = new string[1];
                    // 変形可能な形態一覧を作成
                    var loopTo = GeneralLib.LLength(ref fdata);
                    for (i = 2; i <= loopTo; i++)
                    {
                        object argIndex2 = GeneralLib.LIndex(ref fdata, i);
                        {
                            var withBlock1 = withBlock.OtherForm(ref argIndex2);
                            if (withBlock1.IsAvailable())
                            {
                                Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                                Array.Resize(ref list_id, Information.UBound(list) + 1);
                                list[Information.UBound(list)] = withBlock1.Nickname;
                                list_id[Information.UBound(list)] = withBlock1.Name;
                            }
                        }
                    }

                    GUI.ListItemFlag = new bool[Information.UBound(list) + 1];

                    // 変形する形態を選択
                    if (Information.UBound(list) > 1)
                    {
                        GUI.TopItem = 1;
                        string arglb_caption = "変形";
                        string arglb_info = "名前";
                        string arglb_mode = "カーソル移動";
                        ret = GUI.ListBox(ref arglb_caption, ref list, ref arglb_info, ref arglb_mode);
                        if (ret == 0)
                        {
                            CancelCommand();
                            GUI.UnlockGUI();
                            return;
                        }
                    }
                    else
                    {
                        ret = 1;
                    }

                    // 変形を実施
                    Unit localOtherForm() { var tmp = list_id; object argIndex1 = tmp[ret]; var ret = withBlock.OtherForm(ref argIndex1); return ret; }

                    string argnew_form = localOtherForm().Name;
                    withBlock.Transform(ref argnew_form);
                    localOtherForm().Name = argnew_form;

                    // ユニットリストの表示を更新
                    string argsmode = "";
                    Event_Renamed.MakeUnitList(smode: ref argsmode);

                    // ステータスウィンドウの表示を更新
                    Status.DisplayUnitStatus(ref withBlock.CurrentForm());

                    // コマンドを終了
                    GUI.UnlockGUI();
                    CommandState = "ユニット選択";
                    return;
                }
            }

            // 変形可能な形態の一覧を作成
            list = new string[1];
            list_id = new string[1];
            GUI.ListItemFlag = new bool[1];
            var loopTo1 = GeneralLib.LLength(ref fdata);
            for (i = 2; i <= loopTo1; i++)
            {
                object argIndex3 = GeneralLib.LIndex(ref fdata, i);
                {
                    var withBlock2 = SelectedUnit.OtherForm(ref argIndex3);
                    if (withBlock2.IsAvailable())
                    {
                        Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                        Array.Resize(ref list_id, Information.UBound(list) + 1);
                        Array.Resize(ref GUI.ListItemFlag, Information.UBound(list) + 1);
                        list[Information.UBound(list)] = withBlock2.Nickname0;
                        list_id[Information.UBound(list)] = withBlock2.Name;
                        if (withBlock2.IsAbleToEnter(SelectedUnit.x, SelectedUnit.y) | string.IsNullOrEmpty(Map.MapFileName))
                        {
                            GUI.ListItemFlag[Information.UBound(list)] = false;
                        }
                        else
                        {
                            GUI.ListItemFlag[Information.UBound(list)] = true;
                        }
                    }
                }
            }

            // 変形先の形態を選択
            if (Information.UBound(list) == 1)
            {
                if (GUI.ListItemFlag[1])
                {
                    Interaction.MsgBox("この地形では" + GeneralLib.LIndex(ref fdata, 1) + "できません");
                    CancelCommand();
                    GUI.UnlockGUI();
                    return;
                }

                ret = 1;
            }
            else
            {
                GUI.TopItem = 1;
                if (!SelectedUnit.IsHero())
                {
                    string arglb_caption1 = "変形先";
                    string arglb_info1 = "名前";
                    string arglb_mode1 = "カーソル移動";
                    ret = GUI.ListBox(ref arglb_caption1, ref list, ref arglb_info1, ref arglb_mode1);
                }
                else
                {
                    string arglb_caption2 = "変身先";
                    string arglb_info2 = "名前";
                    string arglb_mode2 = "カーソル移動";
                    ret = GUI.ListBox(ref arglb_caption2, ref list, ref arglb_info2, ref arglb_mode2);
                }

                if (ret == 0)
                {
                    CancelCommand();
                    GUI.UnlockGUI();
                    return;
                }
            }

            uname = list_id[ret];
            string BGM;
            {
                var withBlock3 = SelectedUnit;
                // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
                object argIndex6 = uname;
                {
                    var withBlock4 = SRC.UDList.Item(ref argIndex6);
                    string argfname = "追加パイロット";
                    if (withBlock4.IsFeatureAvailable(ref argfname))
                    {
                        bool localIsDefined1() { object argIndex1 = "追加パイロット"; object argIndex2 = withBlock4.FeatureData(ref argIndex1); var ret = SRC.PList.IsDefined(ref argIndex2); return ret; }

                        if (!localIsDefined1())
                        {
                            bool localIsDefined() { object argIndex1 = "追加パイロット"; object argIndex2 = withBlock4.FeatureData(ref argIndex1); var ret = SRC.PDList.IsDefined(ref argIndex2); return ret; }

                            if (!localIsDefined())
                            {
                                object argIndex4 = "追加パイロット";
                                string argmsg = uname + "の追加パイロット「" + withBlock4.FeatureData(ref argIndex4) + "」のデータが見つかりません";
                                GUI.ErrorMessage(ref argmsg);
                                SRC.TerminateSRC();
                            }

                            object argIndex5 = "追加パイロット";
                            string argpname = withBlock4.FeatureData(ref argIndex5);
                            string argpparty = SelectedUnit.Party0;
                            string arggid = "";
                            SRC.PList.Add(ref argpname, SelectedUnit.MainPilot().Level, ref argpparty, gid: ref arggid);
                            SelectedUnit.Party0 = argpparty;
                        }
                    }
                }

                // ＢＧＭの変更
                string argfname1 = "変形ＢＧＭ";
                if (withBlock3.IsFeatureAvailable(ref argfname1))
                {
                    var loopTo2 = withBlock3.CountFeature();
                    for (i = 1; i <= loopTo2; i++)
                    {
                        string localFeature() { object argIndex1 = i; var ret = withBlock3.Feature(ref argIndex1); return ret; }

                        string localFeatureData2() { object argIndex1 = i; var ret = withBlock3.FeatureData(ref argIndex1); return ret; }

                        string localLIndex() { string arglist = hs6c18ebb7075745309751cd168b7bf5f0(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

                        if (localFeature() == "変形ＢＧＭ" & (localLIndex() ?? "") == (uname ?? ""))
                        {
                            string localFeatureData() { object argIndex1 = i; var ret = withBlock3.FeatureData(ref argIndex1); return ret; }

                            string localFeatureData1() { object argIndex1 = i; var ret = withBlock3.FeatureData(ref argIndex1); return ret; }

                            string argmidi_name = Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1);
                            BGM = Sound.SearchMidiFile(ref argmidi_name);
                            if (Strings.Len(BGM) > 0)
                            {
                                Sound.ChangeBGM(ref BGM);
                                GUI.Sleep(500);
                            }

                            break;
                        }
                    }
                }

                // メッセージを表示
                bool localIsMessageDefined2() { string argmain_situation = "変形(" + withBlock3.Name + "=>" + uname + ")"; var ret = withBlock3.IsMessageDefined(ref argmain_situation); return ret; }

                bool localIsMessageDefined3() { string argmain_situation = "変形(" + uname + ")"; var ret = withBlock3.IsMessageDefined(ref argmain_situation); return ret; }

                bool localIsMessageDefined4() { object argIndex1 = "変形"; string argmain_situation = "変形(" + withBlock3.FeatureName(ref argIndex1) + ")"; var ret = withBlock3.IsMessageDefined(ref argmain_situation); return ret; }

                if (localIsMessageDefined2() | localIsMessageDefined3() | localIsMessageDefined4())
                {
                    GUI.Center(withBlock3.x, withBlock3.y);
                    GUI.RefreshScreen();
                    Unit argu1 = null;
                    Unit argu2 = null;
                    GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                    bool localIsMessageDefined() { string argmain_situation = "変形(" + uname + ")"; var ret = withBlock3.IsMessageDefined(ref argmain_situation); return ret; }

                    bool localIsMessageDefined1() { object argIndex1 = "変形"; string argmain_situation = "変形(" + withBlock3.FeatureName(ref argIndex1) + ")"; var ret = withBlock3.IsMessageDefined(ref argmain_situation); return ret; }

                    string argmain_situation = "変形(" + withBlock3.Name + "=>" + uname + ")";
                    if (withBlock3.IsMessageDefined(ref argmain_situation))
                    {
                        string argSituation = "変形(" + withBlock3.Name + "=>" + uname + ")";
                        string argmsg_mode = "";
                        withBlock3.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                    }
                    else if (localIsMessageDefined())
                    {
                        string argSituation1 = "変形(" + uname + ")";
                        string argmsg_mode1 = "";
                        withBlock3.PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
                    }
                    else if (localIsMessageDefined1())
                    {
                        object argIndex7 = "変形";
                        string argSituation2 = "変形(" + withBlock3.FeatureName(ref argIndex7) + ")";
                        string argmsg_mode2 = "";
                        withBlock3.PilotMessage(ref argSituation2, msg_mode: ref argmsg_mode2);
                    }

                    GUI.CloseMessageForm();
                }

                // アニメ表示
                bool localIsAnimationDefined() { string argmain_situation = "変形(" + uname + ")"; string argsub_situation = ""; var ret = withBlock3.IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsAnimationDefined1() { object argIndex1 = "変形"; string argmain_situation = "変形(" + withBlock3.FeatureName(ref argIndex1) + ")"; string argsub_situation = ""; var ret = withBlock3.IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsSpecialEffectDefined() { string argmain_situation = "変形(" + withBlock3.Name + "=>" + uname + ")"; string argsub_situation = ""; var ret = withBlock3.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsSpecialEffectDefined1() { string argmain_situation = "変形(" + uname + ")"; string argsub_situation = ""; var ret = withBlock3.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsSpecialEffectDefined2() { object argIndex1 = "変形"; string argmain_situation = "変形(" + withBlock3.FeatureName(ref argIndex1) + ")"; string argsub_situation = ""; var ret = withBlock3.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                string argmain_situation7 = "変形(" + withBlock3.Name + "=>" + uname + ")";
                string argsub_situation6 = "";
                if (withBlock3.IsAnimationDefined(ref argmain_situation7, sub_situation: ref argsub_situation6))
                {
                    string argmain_situation1 = "変形(" + withBlock3.Name + "=>" + uname + ")";
                    string argsub_situation = "";
                    withBlock3.PlayAnimation(ref argmain_situation1, sub_situation: ref argsub_situation);
                }
                else if (localIsAnimationDefined())
                {
                    string argmain_situation2 = "変形(" + uname + ")";
                    string argsub_situation1 = "";
                    withBlock3.PlayAnimation(ref argmain_situation2, sub_situation: ref argsub_situation1);
                }
                else if (localIsAnimationDefined1())
                {
                    object argIndex8 = "変形";
                    string argmain_situation3 = "変形(" + withBlock3.FeatureName(ref argIndex8) + ")";
                    string argsub_situation2 = "";
                    withBlock3.PlayAnimation(ref argmain_situation3, sub_situation: ref argsub_situation2);
                }
                else if (localIsSpecialEffectDefined())
                {
                    string argmain_situation4 = "変形(" + withBlock3.Name + "=>" + uname + ")";
                    string argsub_situation3 = "";
                    withBlock3.SpecialEffect(ref argmain_situation4, sub_situation: ref argsub_situation3);
                }
                else if (localIsSpecialEffectDefined1())
                {
                    string argmain_situation5 = "変形(" + uname + ")";
                    string argsub_situation4 = "";
                    withBlock3.SpecialEffect(ref argmain_situation5, sub_situation: ref argsub_situation4);
                }
                else if (localIsSpecialEffectDefined2())
                {
                    object argIndex9 = "変形";
                    string argmain_situation6 = "変形(" + withBlock3.FeatureName(ref argIndex9) + ")";
                    string argsub_situation5 = "";
                    withBlock3.SpecialEffect(ref argmain_situation6, sub_situation: ref argsub_situation5);
                }
            }

            // 変形
            prev_uname = SelectedUnit.Name;
            SelectedUnit.Transform(ref uname);
            SelectedUnit = Map.MapDataForUnit[SelectedUnit.x, SelectedUnit.y];

            // 変形をキャンセルする？
            if (SelectedUnit.Action == 0)
            {
                ret = Interaction.MsgBox("この形態ではこれ以上の行動が出来ません。" + Constants.vbCr + Constants.vbLf + "それでも変形しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "変形");
                if (ret == MsgBoxResult.Cancel)
                {
                    SelectedUnit.Transform(ref prev_uname);
                    SelectedUnit = Map.MapDataForUnit[SelectedUnit.x, SelectedUnit.y];
                    object argIndex11 = "消耗";
                    if (SelectedUnit.IsConditionSatisfied(ref argIndex11))
                    {
                        object argIndex10 = "消耗";
                        SelectedUnit.DeleteCondition(ref argIndex10);
                    }
                }

                GUI.RedrawScreen();
            }

            // 変形イベント
            {
                var withBlock5 = SelectedUnit.CurrentForm();
                Event_Renamed.HandleEvent("変形", withBlock5.MainPilot().ID, withBlock5.Name);
            }

            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                Status.ClearUnitStatus();
                GUI.RedrawScreen();
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            SRC.IsCanceled = false;

            // ハイパーモード・ノーマルモードの自動発動をチェック
            SelectedUnit.CurrentForm().CheckAutoHyperMode();
            SelectedUnit.CurrentForm().CheckAutoNormalMode();

            // カーソル自動移動
            if (SelectedUnit.Status_Renamed == "出撃")
            {
                if (SRC.AutoMoveCursor)
                {
                    string argcursor_mode = "ユニット選択";
                    GUI.MoveCursorPos(ref argcursor_mode, SelectedUnit);
                }

                Status.DisplayUnitStatus(ref SelectedUnit);
            }

            CommandState = "ユニット選択";
            GUI.UnlockGUI();
        }

        // 「ハイパーモード」コマンド
        // MOD START MARGE
        // Public Sub HyperModeCommand()
        private void HyperModeCommand()
        {
            // MOD END MARGE
            string uname, fname;
            int i;

            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Status.ClearUnitStatus();
            }

            GUI.LockGUI();
            object argIndex1 = "ハイパーモード";
            string arglist = SelectedUnit.FeatureData(ref argIndex1);
            uname = GeneralLib.LIndex(ref arglist, 2);
            object argIndex2 = "ハイパーモード";
            fname = SelectedUnit.FeatureName(ref argIndex2);
            if (string.IsNullOrEmpty(Map.MapFileName))
            {
                // ユニットステータスコマンドの場合
                {
                    var withBlock = SelectedUnit;
                    string argfname = "ハイパーモード";
                    if (!withBlock.IsFeatureAvailable(ref argfname))
                    {
                        object argIndex3 = "ノーマルモード";
                        string arglist1 = SelectedUnit.FeatureData(ref argIndex3);
                        uname = GeneralLib.LIndex(ref arglist1, 1);
                    }

                    // ハイパーモードを発動
                    withBlock.Transform(ref uname);

                    // ユニットリストの表示を更新
                    string argsmode = "";
                    Event_Renamed.MakeUnitList(smode: ref argsmode);

                    // ステータスウィンドウの表示を更新
                    Status.DisplayUnitStatus(ref withBlock.CurrentForm());

                    // コマンドを終了
                    GUI.UnlockGUI();
                    CommandState = "ユニット選択";
                    return;
                }
            }

            // ハイパーモードを発動可能かどうかチェック
            object argIndex4 = uname;
            {
                var withBlock1 = SelectedUnit.OtherForm(ref argIndex4);
                if (!withBlock1.IsAbleToEnter(SelectedUnit.x, SelectedUnit.y) & !string.IsNullOrEmpty(Map.MapFileName))
                {
                    Interaction.MsgBox("この地形では変形できません");
                    GUI.UnlockGUI();
                    CancelCommand();
                    return;
                }
            }

            // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
            object argIndex7 = uname;
            {
                var withBlock2 = SRC.UDList.Item(ref argIndex7);
                string argfname1 = "追加パイロット";
                if (withBlock2.IsFeatureAvailable(ref argfname1))
                {
                    bool localIsDefined1() { object argIndex1 = "追加パイロット"; object argIndex2 = withBlock2.FeatureData(ref argIndex1); var ret = SRC.PList.IsDefined(ref argIndex2); return ret; }

                    if (!localIsDefined1())
                    {
                        bool localIsDefined() { object argIndex1 = "追加パイロット"; object argIndex2 = withBlock2.FeatureData(ref argIndex1); var ret = SRC.PDList.IsDefined(ref argIndex2); return ret; }

                        if (!localIsDefined())
                        {
                            object argIndex5 = "追加パイロット";
                            string argmsg = uname + "の追加パイロット「" + withBlock2.FeatureData(ref argIndex5) + "」のデータが見つかりません";
                            GUI.ErrorMessage(ref argmsg);
                            SRC.TerminateSRC();
                        }

                        object argIndex6 = "追加パイロット";
                        string argpname = withBlock2.FeatureData(ref argIndex6);
                        string argpparty = SelectedUnit.Party0;
                        string arggid = "";
                        SRC.PList.Add(ref argpname, SelectedUnit.MainPilot().Level, ref argpparty, gid: ref arggid);
                        SelectedUnit.Party0 = argpparty;
                    }
                }
            }

            string BGM;
            {
                var withBlock3 = SelectedUnit;
                // ＢＧＭを変更
                string argfname2 = "ハイパーモードＢＧＭ";
                if (withBlock3.IsFeatureAvailable(ref argfname2))
                {
                    var loopTo = withBlock3.CountFeature();
                    for (i = 1; i <= loopTo; i++)
                    {
                        string localFeature() { object argIndex1 = i; var ret = withBlock3.Feature(ref argIndex1); return ret; }

                        string localFeatureData2() { object argIndex1 = i; var ret = withBlock3.FeatureData(ref argIndex1); return ret; }

                        string localLIndex() { string arglist = hs79a81f167161473a965a38e7883f62a2(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

                        if (localFeature() == "ハイパーモードＢＧＭ" & (localLIndex() ?? "") == (uname ?? ""))
                        {
                            string localFeatureData() { object argIndex1 = i; var ret = withBlock3.FeatureData(ref argIndex1); return ret; }

                            string localFeatureData1() { object argIndex1 = i; var ret = withBlock3.FeatureData(ref argIndex1); return ret; }

                            string argmidi_name = Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1);
                            BGM = Sound.SearchMidiFile(ref argmidi_name);
                            if (Strings.Len(BGM) > 0)
                            {
                                Sound.ChangeBGM(ref BGM);
                                GUI.Sleep(500);
                            }

                            break;
                        }
                    }
                }

                // メッセージを表示
                bool localIsMessageDefined2() { string argmain_situation = "ハイパーモード(" + withBlock3.Name + "=>" + uname + ")"; var ret = withBlock3.IsMessageDefined(ref argmain_situation); return ret; }

                bool localIsMessageDefined3() { string argmain_situation = "ハイパーモード(" + uname + ")"; var ret = withBlock3.IsMessageDefined(ref argmain_situation); return ret; }

                bool localIsMessageDefined4() { string argmain_situation = "ハイパーモード(" + fname + ")"; var ret = withBlock3.IsMessageDefined(ref argmain_situation); return ret; }

                string argmain_situation1 = "ハイパーモード";
                if (localIsMessageDefined2() | localIsMessageDefined3() | localIsMessageDefined4() | withBlock3.IsMessageDefined(ref argmain_situation1))
                {
                    GUI.Center(withBlock3.x, withBlock3.y);
                    GUI.RefreshScreen();
                    Unit argu1 = null;
                    Unit argu2 = null;
                    GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                    bool localIsMessageDefined() { string argmain_situation = "ハイパーモード(" + uname + ")"; var ret = withBlock3.IsMessageDefined(ref argmain_situation); return ret; }

                    bool localIsMessageDefined1() { string argmain_situation = "ハイパーモード(" + fname + ")"; var ret = withBlock3.IsMessageDefined(ref argmain_situation); return ret; }

                    string argmain_situation = "ハイパーモード(" + withBlock3.Name + "=>" + uname + ")";
                    if (withBlock3.IsMessageDefined(ref argmain_situation))
                    {
                        string argSituation = "ハイパーモード(" + withBlock3.Name + "=>" + uname + ")";
                        string argmsg_mode = "";
                        withBlock3.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                    }
                    else if (localIsMessageDefined())
                    {
                        string argSituation2 = "ハイパーモード(" + uname + ")";
                        string argmsg_mode2 = "";
                        withBlock3.PilotMessage(ref argSituation2, msg_mode: ref argmsg_mode2);
                    }
                    else if (localIsMessageDefined1())
                    {
                        string argSituation3 = "ハイパーモード(" + fname + ")";
                        string argmsg_mode3 = "";
                        withBlock3.PilotMessage(ref argSituation3, msg_mode: ref argmsg_mode3);
                    }
                    else
                    {
                        string argSituation1 = "ハイパーモード";
                        string argmsg_mode1 = "";
                        withBlock3.PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
                    }

                    GUI.CloseMessageForm();
                }

                // アニメ表示
                bool localIsAnimationDefined() { string argmain_situation = "ハイパーモード(" + uname + ")"; string argsub_situation = ""; var ret = withBlock3.IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsAnimationDefined1() { string argmain_situation = "ハイパーモード(" + fname + ")"; string argsub_situation = ""; var ret = withBlock3.IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsSpecialEffectDefined() { string argmain_situation = "ハイパーモード(" + withBlock3.Name + "=>" + uname + ")"; string argsub_situation = ""; var ret = withBlock3.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsSpecialEffectDefined1() { string argmain_situation = "ハイパーモード(" + uname + ")"; string argsub_situation = ""; var ret = withBlock3.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsSpecialEffectDefined2() { string argmain_situation = "ハイパーモード(" + fname + ")"; string argsub_situation = ""; var ret = withBlock3.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                string argmain_situation10 = "ハイパーモード(" + withBlock3.Name + "=>" + uname + ")";
                string argsub_situation8 = "";
                string argmain_situation11 = "ハイパーモード";
                string argsub_situation9 = "";
                if (withBlock3.IsAnimationDefined(ref argmain_situation10, sub_situation: ref argsub_situation8))
                {
                    string argmain_situation2 = "ハイパーモード(" + withBlock3.Name + "=>" + uname + ")";
                    string argsub_situation = "";
                    withBlock3.PlayAnimation(ref argmain_situation2, sub_situation: ref argsub_situation);
                }
                else if (localIsAnimationDefined())
                {
                    string argmain_situation4 = "ハイパーモード(" + uname + ")";
                    string argsub_situation2 = "";
                    withBlock3.PlayAnimation(ref argmain_situation4, sub_situation: ref argsub_situation2);
                }
                else if (localIsAnimationDefined1())
                {
                    string argmain_situation5 = "ハイパーモード(" + fname + ")";
                    string argsub_situation3 = "";
                    withBlock3.PlayAnimation(ref argmain_situation5, sub_situation: ref argsub_situation3);
                }
                else if (withBlock3.IsAnimationDefined(ref argmain_situation11, sub_situation: ref argsub_situation9))
                {
                    string argmain_situation6 = "ハイパーモード";
                    string argsub_situation4 = "";
                    withBlock3.PlayAnimation(ref argmain_situation6, sub_situation: ref argsub_situation4);
                }
                else if (localIsSpecialEffectDefined())
                {
                    string argmain_situation7 = "ハイパーモード(" + withBlock3.Name + "=>" + uname + ")";
                    string argsub_situation5 = "";
                    withBlock3.SpecialEffect(ref argmain_situation7, sub_situation: ref argsub_situation5);
                }
                else if (localIsSpecialEffectDefined1())
                {
                    string argmain_situation8 = "ハイパーモード(" + uname + ")";
                    string argsub_situation6 = "";
                    withBlock3.SpecialEffect(ref argmain_situation8, sub_situation: ref argsub_situation6);
                }
                else if (localIsSpecialEffectDefined2())
                {
                    string argmain_situation9 = "ハイパーモード(" + fname + ")";
                    string argsub_situation7 = "";
                    withBlock3.SpecialEffect(ref argmain_situation9, sub_situation: ref argsub_situation7);
                }
                else
                {
                    string argmain_situation3 = "ハイパーモード";
                    string argsub_situation1 = "";
                    withBlock3.SpecialEffect(ref argmain_situation3, sub_situation: ref argsub_situation1);
                }
            }

            // ハイパーモード発動
            SelectedUnit.Transform(ref uname);

            // ハイパーモード・ノーマルモードの自動発動をチェック
            SelectedUnit.CurrentForm().CheckAutoHyperMode();
            SelectedUnit.CurrentForm().CheckAutoNormalMode();
            SelectedUnit = Map.MapDataForUnit[SelectedUnit.x, SelectedUnit.y];

            // 変形イベント
            {
                var withBlock4 = SelectedUnit.CurrentForm();
                Event_Renamed.HandleEvent("変形", withBlock4.MainPilot().ID, withBlock4.Name);
            }

            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                Status.ClearUnitStatus();
                GUI.RedrawScreen();
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            SRC.IsCanceled = false;

            // カーソル自動移動
            if (SelectedUnit.Status_Renamed == "出撃")
            {
                if (SRC.AutoMoveCursor)
                {
                    string argcursor_mode = "ユニット選択";
                    GUI.MoveCursorPos(ref argcursor_mode, SelectedUnit);
                }

                Status.DisplayUnitStatus(ref SelectedUnit);
            }

            CommandState = "ユニット選択";
            GUI.UnlockGUI();
        }

        // 「変身解除」コマンド
        // MOD START MARGE
        // Public Sub CancelTransformationCommand()
        private void CancelTransformationCommand()
        {
            // MOD END MARGE
            int ret;

            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Status.ClearUnitStatus();
            }

            GUI.LockGUI();
            {
                var withBlock = SelectedUnit;
                if (string.IsNullOrEmpty(Map.MapFileName))
                {
                    // ユニットステータスコマンドの場合
                    string localLIndex() { object argIndex1 = "ノーマルモード"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

                    string argnew_form = localLIndex();
                    withBlock.Transform(ref argnew_form);
                    string argsmode = "";
                    Event_Renamed.MakeUnitList(smode: ref argsmode);
                    Status.DisplayUnitStatus(ref withBlock.CurrentForm());
                    GUI.UnlockGUI();
                    CommandState = "ユニット選択";
                    return;
                }

                if (withBlock.IsHero())
                {
                    ret = Interaction.MsgBox("変身を解除しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "変身解除");
                }
                else
                {
                    ret = Interaction.MsgBox("特殊モードを解除しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "特殊モード解除");
                }

                if (ret == MsgBoxResult.Cancel)
                {
                    GUI.UnlockGUI();
                    CancelCommand();
                    return;
                }

                string localLIndex1() { object argIndex1 = "ノーマルモード"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

                string argnew_form1 = localLIndex1();
                withBlock.Transform(ref argnew_form1);
                SelectedUnit = Map.MapDataForUnit[withBlock.x, withBlock.y];
            }

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                string argcursor_mode = "ユニット選択";
                GUI.MoveCursorPos(ref argcursor_mode, SelectedUnit);
            }

            Status.DisplayUnitStatus(ref SelectedUnit);
            GUI.RedrawScreen();

            // 変形イベント
            Event_Renamed.HandleEvent("変形", SelectedUnit.MainPilot().ID, SelectedUnit.Name);
            SRC.IsScenarioFinished = false;
            SRC.IsCanceled = false;
            CommandState = "ユニット選択";
            GUI.UnlockGUI();
        }

        // 「分離」コマンド
        // MOD START MARGE
        // Public Sub SplitCommand()
        private void SplitCommand()
        {
            // MOD END MARGE
            string uname, tname, fname;
            int ret;
            string BGM;

            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Status.ClearUnitStatus();
            }

            GUI.LockGUI();
            if (string.IsNullOrEmpty(Map.MapFileName))
            {
                // ユニットステータスコマンドの場合

                // 分離を実施
                {
                    var withBlock = SelectedUnit;
                    string argfname = "パーツ分離";
                    if (withBlock.IsFeatureAvailable(ref argfname))
                    {
                        object argIndex1 = "パーツ分離";
                        string arglist = withBlock.FeatureData(ref argIndex1);
                        tname = GeneralLib.LIndex(ref arglist, 2);
                        withBlock.Transform(ref tname);
                    }
                    else
                    {
                        withBlock.Split_Renamed();
                    }

                    SRC.UList.CheckAutoHyperMode();
                    SRC.UList.CheckAutoNormalMode();
                    Status.DisplayUnitStatus(ref Map.MapDataForUnit[withBlock.x, withBlock.y]);
                }

                // ユニットリストの表示を更新
                string argsmode = "";
                Event_Renamed.MakeUnitList(smode: ref argsmode);

                // コマンドを終了
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            {
                var withBlock1 = SelectedUnit;
                string argfname3 = "パーツ分離";
                if (withBlock1.IsFeatureAvailable(ref argfname3))
                {
                    // パーツ分離を行う場合

                    ret = Interaction.MsgBox("パーツを分離しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "パーツ分離");
                    if (ret == MsgBoxResult.Cancel)
                    {
                        GUI.UnlockGUI();
                        CancelCommand();
                        return;
                    }

                    object argIndex2 = "パーツ分離";
                    string arglist1 = withBlock1.FeatureData(ref argIndex2);
                    tname = GeneralLib.LIndex(ref arglist1, 2);
                    Unit localOtherForm() { object argIndex1 = tname; var ret = withBlock1.OtherForm(ref argIndex1); return ret; }

                    if (!localOtherForm().IsAbleToEnter(withBlock1.x, withBlock1.y))
                    {
                        Interaction.MsgBox("この地形では分離できません");
                        GUI.UnlockGUI();
                        CancelCommand();
                        return;
                    }

                    // ＢＧＭ変更
                    string argfname1 = "分離ＢＧＭ";
                    if (withBlock1.IsFeatureAvailable(ref argfname1))
                    {
                        object argIndex3 = "分離ＢＧＭ";
                        string argmidi_name = withBlock1.FeatureData(ref argIndex3);
                        BGM = Sound.SearchMidiFile(ref argmidi_name);
                        if (Strings.Len(BGM) > 0)
                        {
                            object argIndex4 = "分離ＢＧＭ";
                            string argbgm_name = withBlock1.FeatureData(ref argIndex4);
                            Sound.StartBGM(ref argbgm_name);
                            GUI.Sleep(500);
                        }
                    }

                    object argIndex5 = "パーツ分離";
                    fname = withBlock1.FeatureName(ref argIndex5);

                    // メッセージを表示
                    bool localIsMessageDefined1() { string argmain_situation = "分離(" + withBlock1.Name + ")"; var ret = withBlock1.IsMessageDefined(ref argmain_situation); return ret; }

                    bool localIsMessageDefined2() { string argmain_situation = "分離(" + fname + ")"; var ret = withBlock1.IsMessageDefined(ref argmain_situation); return ret; }

                    string argmain_situation1 = "分離";
                    if (localIsMessageDefined1() | localIsMessageDefined2() | withBlock1.IsMessageDefined(ref argmain_situation1))
                    {
                        GUI.Center(withBlock1.x, withBlock1.y);
                        GUI.RefreshScreen();
                        Unit argu1 = null;
                        Unit argu2 = null;
                        GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                        bool localIsMessageDefined() { string argmain_situation = "分離(" + fname + ")"; var ret = withBlock1.IsMessageDefined(ref argmain_situation); return ret; }

                        string argmain_situation = "分離(" + withBlock1.Name + ")";
                        if (withBlock1.IsMessageDefined(ref argmain_situation))
                        {
                            string argSituation = "分離(" + withBlock1.Name + ")";
                            string argmsg_mode = "";
                            withBlock1.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                        }
                        else if (localIsMessageDefined())
                        {
                            string argSituation2 = "分離(" + fname + ")";
                            string argmsg_mode2 = "";
                            withBlock1.PilotMessage(ref argSituation2, msg_mode: ref argmsg_mode2);
                        }
                        else
                        {
                            string argSituation1 = "分離";
                            string argmsg_mode1 = "";
                            withBlock1.PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
                        }

                        GUI.CloseMessageForm();
                    }

                    // アニメ表示
                    bool localIsAnimationDefined() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = withBlock1.IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    bool localIsSpecialEffectDefined() { string argmain_situation = "分離(" + withBlock1.Name + ")"; string argsub_situation = ""; var ret = withBlock1.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    bool localIsSpecialEffectDefined1() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = withBlock1.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    string argmain_situation8 = "分離(" + withBlock1.Name + ")";
                    string argsub_situation6 = "";
                    string argmain_situation9 = "分離";
                    string argsub_situation7 = "";
                    if (withBlock1.IsAnimationDefined(ref argmain_situation8, sub_situation: ref argsub_situation6))
                    {
                        string argmain_situation2 = "分離(" + withBlock1.Name + ")";
                        string argsub_situation = "";
                        withBlock1.PlayAnimation(ref argmain_situation2, sub_situation: ref argsub_situation);
                    }
                    else if (localIsAnimationDefined())
                    {
                        string argmain_situation4 = "分離(" + fname + ")";
                        string argsub_situation2 = "";
                        withBlock1.PlayAnimation(ref argmain_situation4, sub_situation: ref argsub_situation2);
                    }
                    else if (withBlock1.IsAnimationDefined(ref argmain_situation9, sub_situation: ref argsub_situation7))
                    {
                        string argmain_situation5 = "分離";
                        string argsub_situation3 = "";
                        withBlock1.PlayAnimation(ref argmain_situation5, sub_situation: ref argsub_situation3);
                    }
                    else if (localIsSpecialEffectDefined())
                    {
                        string argmain_situation6 = "分離(" + withBlock1.Name + ")";
                        string argsub_situation4 = "";
                        withBlock1.SpecialEffect(ref argmain_situation6, sub_situation: ref argsub_situation4);
                    }
                    else if (localIsSpecialEffectDefined1())
                    {
                        string argmain_situation7 = "分離(" + fname + ")";
                        string argsub_situation5 = "";
                        withBlock1.SpecialEffect(ref argmain_situation7, sub_situation: ref argsub_situation5);
                    }
                    else
                    {
                        string argmain_situation3 = "分離";
                        string argsub_situation1 = "";
                        withBlock1.SpecialEffect(ref argmain_situation3, sub_situation: ref argsub_situation1);
                    }

                    // パーツ分離
                    uname = withBlock1.Name;
                    withBlock1.Transform(ref tname);
                    SelectedUnit = Map.MapDataForUnit[withBlock1.x, withBlock1.y];
                    Status.DisplayUnitStatus(ref SelectedUnit);
                }
                else
                {
                    // 通常の分離を行う場合

                    ret = Interaction.MsgBox("分離しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "分離");
                    if (ret == MsgBoxResult.Cancel)
                    {
                        GUI.UnlockGUI();
                        CancelCommand();
                        return;
                    }

                    // ＢＧＭを変更
                    string argfname2 = "分離ＢＧＭ";
                    if (withBlock1.IsFeatureAvailable(ref argfname2))
                    {
                        object argIndex6 = "分離ＢＧＭ";
                        string argmidi_name1 = withBlock1.FeatureData(ref argIndex6);
                        BGM = Sound.SearchMidiFile(ref argmidi_name1);
                        if (Strings.Len(BGM) > 0)
                        {
                            object argIndex7 = "分離ＢＧＭ";
                            string argbgm_name1 = withBlock1.FeatureData(ref argIndex7);
                            Sound.StartBGM(ref argbgm_name1);
                            GUI.Sleep(500);
                        }
                    }

                    object argIndex8 = "分離";
                    fname = withBlock1.FeatureName(ref argIndex8);

                    // メッセージを表示
                    bool localIsMessageDefined4() { string argmain_situation = "分離(" + withBlock1.Name + ")"; var ret = withBlock1.IsMessageDefined(ref argmain_situation); return ret; }

                    bool localIsMessageDefined5() { string argmain_situation = "分離(" + fname + ")"; var ret = withBlock1.IsMessageDefined(ref argmain_situation); return ret; }

                    string argmain_situation11 = "分離";
                    if (localIsMessageDefined4() | localIsMessageDefined5() | withBlock1.IsMessageDefined(ref argmain_situation11))
                    {
                        GUI.Center(withBlock1.x, withBlock1.y);
                        GUI.RefreshScreen();
                        Unit argu11 = null;
                        Unit argu21 = null;
                        GUI.OpenMessageForm(u1: ref argu11, u2: ref argu21);
                        bool localIsMessageDefined3() { string argmain_situation = "分離(" + fname + ")"; var ret = withBlock1.IsMessageDefined(ref argmain_situation); return ret; }

                        string argmain_situation10 = "分離(" + withBlock1.Name + ")";
                        if (withBlock1.IsMessageDefined(ref argmain_situation10))
                        {
                            string argSituation3 = "分離(" + withBlock1.Name + ")";
                            string argmsg_mode3 = "";
                            withBlock1.PilotMessage(ref argSituation3, msg_mode: ref argmsg_mode3);
                        }
                        else if (localIsMessageDefined3())
                        {
                            string argSituation5 = "分離(" + fname + ")";
                            string argmsg_mode5 = "";
                            withBlock1.PilotMessage(ref argSituation5, msg_mode: ref argmsg_mode5);
                        }
                        else
                        {
                            string argSituation4 = "分離";
                            string argmsg_mode4 = "";
                            withBlock1.PilotMessage(ref argSituation4, msg_mode: ref argmsg_mode4);
                        }

                        GUI.CloseMessageForm();
                    }

                    // アニメ表示
                    bool localIsAnimationDefined1() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = withBlock1.IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    bool localIsSpecialEffectDefined2() { string argmain_situation = "分離(" + withBlock1.Name + ")"; string argsub_situation = ""; var ret = withBlock1.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    bool localIsSpecialEffectDefined3() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = withBlock1.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    string argmain_situation18 = "分離(" + withBlock1.Name + ")";
                    string argsub_situation14 = "";
                    string argmain_situation19 = "分離";
                    string argsub_situation15 = "";
                    if (withBlock1.IsAnimationDefined(ref argmain_situation18, sub_situation: ref argsub_situation14))
                    {
                        string argmain_situation12 = "分離(" + withBlock1.Name + ")";
                        string argsub_situation8 = "";
                        withBlock1.PlayAnimation(ref argmain_situation12, sub_situation: ref argsub_situation8);
                    }
                    else if (localIsAnimationDefined1())
                    {
                        string argmain_situation14 = "分離(" + fname + ")";
                        string argsub_situation10 = "";
                        withBlock1.PlayAnimation(ref argmain_situation14, sub_situation: ref argsub_situation10);
                    }
                    else if (withBlock1.IsAnimationDefined(ref argmain_situation19, sub_situation: ref argsub_situation15))
                    {
                        string argmain_situation15 = "分離";
                        string argsub_situation11 = "";
                        withBlock1.PlayAnimation(ref argmain_situation15, sub_situation: ref argsub_situation11);
                    }
                    else if (localIsSpecialEffectDefined2())
                    {
                        string argmain_situation16 = "分離(" + withBlock1.Name + ")";
                        string argsub_situation12 = "";
                        withBlock1.SpecialEffect(ref argmain_situation16, sub_situation: ref argsub_situation12);
                    }
                    else if (localIsSpecialEffectDefined3())
                    {
                        string argmain_situation17 = "分離(" + fname + ")";
                        string argsub_situation13 = "";
                        withBlock1.SpecialEffect(ref argmain_situation17, sub_situation: ref argsub_situation13);
                    }
                    else
                    {
                        string argmain_situation13 = "分離";
                        string argsub_situation9 = "";
                        withBlock1.SpecialEffect(ref argmain_situation13, sub_situation: ref argsub_situation9);
                    }

                    // 分離
                    uname = withBlock1.Name;
                    withBlock1.Split_Renamed();

                    // 選択ユニットを再設定
                    string localLIndex() { object argIndex1 = "分離"; string arglist = withBlock1.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    object argIndex9 = localLIndex();
                    SelectedUnit = SRC.UList.Item(ref argIndex9);
                    Status.DisplayUnitStatus(ref SelectedUnit);
                }
            }

            // 分離イベント
            Event_Renamed.HandleEvent("分離", SelectedUnit.MainPilot().ID, uname);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                Status.ClearUnitStatus();
                GUI.RedrawScreen();
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            SRC.IsCanceled = false;

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                string argcursor_mode = "ユニット選択";
                GUI.MoveCursorPos(ref argcursor_mode, SelectedUnit);
            }

            // ハイパーモード＆ノーマルモードの自動発動チェック
            SRC.UList.CheckAutoHyperMode();
            SRC.UList.CheckAutoNormalMode();
            CommandState = "ユニット選択";
            GUI.UnlockGUI();
        }

        // 「合体」コマンド
        // MOD START MARGE
        // Public Sub CombineCommand()
        private void CombineCommand()
        {
            // MOD END MARGE
            int i, j;
            string[] list;
            Unit u;

            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Status.ClearUnitStatus();
            }

            GUI.LockGUI();
            list = new string[1];
            GUI.ListItemFlag = new bool[1];
            {
                var withBlock = SelectedUnit;
                if (string.IsNullOrEmpty(Map.MapFileName))
                {
                    // ユニットステータスコマンドの時
                    // パーツ合体ならば……
                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    string argfname = "パーツ合体";
                    if (GUI.MainForm.mnuUnitCommandItem.Item(CombineCmdID).Caption == "パーツ合体" & withBlock.IsFeatureAvailable(ref argfname))
                    {
                        // パーツ合体を実施
                        object argIndex1 = "パーツ合体";
                        string argnew_form = withBlock.FeatureData(ref argIndex1);
                        withBlock.Transform(ref argnew_form);
                        Status.DisplayUnitStatus(ref Map.MapDataForUnit[withBlock.x, withBlock.y]);
                        Map.MapDataForUnit[withBlock.x, withBlock.y].CheckAutoHyperMode();
                        Map.MapDataForUnit[withBlock.x, withBlock.y].CheckAutoNormalMode();

                        // ユニットリストの表示を更新
                        string argsmode = "";
                        Event_Renamed.MakeUnitList(smode: ref argsmode);

                        // コマンドを終了
                        CommandState = "ユニット選択";
                        GUI.UnlockGUI();
                        return;
                    }
                }

                // 選択可能な合体パターンのリストを作成
                var loopTo = withBlock.CountFeature();
                for (i = 1; i <= loopTo; i++)
                {
                    string localFeature() { object argIndex1 = i; var ret = withBlock.Feature(ref argIndex1); return ret; }

                    string localFeatureData4() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                    int localLLength() { string arglist = hsdebc20cc3cfc47d3be3b16b45d9b88b4(); var ret = GeneralLib.LLength(ref arglist); return ret; }

                    if (localFeature() == "合体" & (localLLength() > 3 | string.IsNullOrEmpty(Map.MapFileName)))
                    {
                        string localFeatureData() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                        string localLIndex() { string arglist = hs8ad4cb28843f4cd79e5881037333d438(); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                        bool localIsDefined() { object argIndex1 = (object)hs340135fe58cf47dbaf5b402aedf48d4a(); var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

                        if (!localIsDefined())
                        {
                            goto NextLoop;
                        }

                        string localFeatureData2() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                        string arglist = localFeatureData2();
                        var loopTo1 = GeneralLib.LLength(ref arglist);
                        for (j = 3; j <= loopTo1; j++)
                        {
                            string localFeatureData1() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                            string localLIndex1() { string arglist = hs751afdc814f14f178b9bc1e2c197a85c(); var ret = GeneralLib.LIndex(ref arglist, j); return ret; }

                            object argIndex2 = localLIndex1();
                            u = SRC.UList.Item(ref argIndex2);
                            if (u is null)
                            {
                                goto NextLoop;
                            }

                            if (!u.IsOperational())
                            {
                                goto NextLoop;
                            }

                            string argfname1 = "合体制限";
                            if (u.Status_Renamed != "出撃" & u.CurrentForm().IsFeatureAvailable(ref argfname1))
                            {
                                goto NextLoop;
                            }

                            if (!string.IsNullOrEmpty(Map.MapFileName))
                            {
                                if (Math.Abs((withBlock.x - u.CurrentForm().x)) > 2 | Math.Abs((withBlock.y - u.CurrentForm().y)) > 2)
                                {
                                    goto NextLoop;
                                }
                            }
                        }

                        Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                        GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
                        string localFeatureData3() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                        string arglist1 = localFeatureData3();
                        list[Information.UBound(list)] = GeneralLib.LIndex(ref arglist1, 2);
                        GUI.ListItemFlag[Information.UBound(list)] = false;
                    }

                NextLoop:
                    ;
                }

                // どの合体を行うかを選択
                if (Information.UBound(list) == 1)
                {
                    i = 1;
                }
                else
                {
                    GUI.TopItem = 1;
                    string arglb_caption = "合体後の形態";
                    string arglb_info = "名前";
                    string arglb_mode = "";
                    i = GUI.ListBox(ref arglb_caption, ref list, ref arglb_info, lb_mode: ref arglb_mode);
                    if (i == 0)
                    {
                        CancelCommand();
                        GUI.UnlockGUI();
                        return;
                    }
                }
            }

            if (string.IsNullOrEmpty(Map.MapFileName))
            {
                // ユニットステータスコマンドの時
                SelectedUnit.Combine(ref list[i], true);

                // ハイパーモード・ノーマルモードの自動発動をチェック
                SRC.UList.CheckAutoHyperMode();
                SRC.UList.CheckAutoNormalMode();

                // ユニットリストの表示を更新
                string argsmode1 = "";
                Event_Renamed.MakeUnitList(smode: ref argsmode1);

                // コマンドを終了
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            // 合体！
            SelectedUnit.Combine(ref list[i]);

            // ハイパーモード＆ノーマルモードの自動発動
            SRC.UList.CheckAutoHyperMode();
            SRC.UList.CheckAutoNormalMode();

            // 合体後のユニットを選択しておく
            SelectedUnit = Map.MapDataForUnit[SelectedUnit.x, SelectedUnit.y];

            // 行動数消費
            SelectedUnit.UseAction();

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                string argcursor_mode = "ユニット選択";
                GUI.MoveCursorPos(ref argcursor_mode, SelectedUnit);
            }

            Status.DisplayUnitStatus(ref SelectedUnit);

            // 合体イベント
            Event_Renamed.HandleEvent("合体", SelectedUnit.MainPilot().ID, SelectedUnit.Name);
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

            // 行動終了
            WaitCommand(true);
        }

        // 「換装」コマンド
        // MOD START MARGE
        // Public Sub ExchangeFormCommand()
        public void ExchangeFormCommand()
        {
            // MOD END MARGE
            string[] list;
            string[] id_list;
            int j, i, k;
            int max_value;
            int ret;
            string fdata;
            string[] farray;
            GUI.LockGUI();
            {
                var withBlock = SelectedUnit;
                object argIndex1 = "換装";
                fdata = withBlock.FeatureData(ref argIndex1);

                // 選択可能な換装先のリストを作成
                list = new string[1];
                id_list = new string[1];
                GUI.ListItemComment = new string[1];
                var loopTo = GeneralLib.LLength(ref fdata);
                for (i = 1; i <= loopTo; i++)
                {
                    object argIndex5 = GeneralLib.LIndex(ref fdata, i);
                    {
                        var withBlock1 = withBlock.OtherForm(ref argIndex5);
                        if (withBlock1.IsAvailable())
                        {
                            Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                            Array.Resize(ref id_list, Information.UBound(list) + 1);
                            Array.Resize(ref GUI.ListItemComment, Information.UBound(list) + 1);
                            id_list[Information.UBound(list)] = withBlock1.Name;

                            // 各形態の表示内容を作成
                            if ((SelectedUnit.Nickname0 ?? "") == (withBlock1.Nickname ?? ""))
                            {
                                string argbuf = withBlock1.Name;
                                list[Information.UBound(list)] = GeneralLib.RightPaddedString(ref argbuf, 27);
                                withBlock1.Name = argbuf;
                            }
                            else
                            {
                                string argbuf1 = withBlock1.Nickname0;
                                list[Information.UBound(list)] = GeneralLib.RightPaddedString(ref argbuf1, 27);
                                withBlock1.Nickname0 = argbuf1;
                            }

                            string localLeftPaddedString() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MaxHP); var ret = GeneralLib.LeftPaddedString(ref argbuf, 6); return ret; }

                            string localLeftPaddedString1() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MaxEN); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

                            string localLeftPaddedString2() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.get_Armor("")); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

                            string localLeftPaddedString3() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.get_Mobility("")); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

                            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString() + localLeftPaddedString1() + localLeftPaddedString2() + localLeftPaddedString3() + " " + withBlock1.Data.Adaption;

                            // 最大攻撃力
                            max_value = 0;
                            var loopTo1 = withBlock1.CountWeapon();
                            for (j = 1; j <= loopTo1; j++)
                            {
                                string argattr = "合";
                                if (withBlock1.IsWeaponMastered(j) & !withBlock1.IsDisabled(ref withBlock1.Weapon(j).Name) & !withBlock1.IsWeaponClassifiedAs(j, ref argattr))
                                {
                                    string argtarea1 = "";
                                    if (withBlock1.WeaponPower(j, ref argtarea1) > max_value)
                                    {
                                        string argtarea = "";
                                        max_value = withBlock1.WeaponPower(j, ref argtarea);
                                    }
                                }
                            }

                            string localLeftPaddedString4() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(max_value); var ret = GeneralLib.LeftPaddedString(ref argbuf, 7); return ret; }

                            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString4();

                            // 最大射程
                            max_value = 0;
                            var loopTo2 = withBlock1.CountWeapon();
                            for (j = 1; j <= loopTo2; j++)
                            {
                                string argattr1 = "合";
                                if (withBlock1.IsWeaponMastered(j) & !withBlock1.IsDisabled(ref withBlock1.Weapon(j).Name) & !withBlock1.IsWeaponClassifiedAs(j, ref argattr1))
                                {
                                    if (withBlock1.WeaponMaxRange(j) > max_value)
                                    {
                                        max_value = withBlock1.WeaponMaxRange(j);
                                    }
                                }
                            }

                            string localLeftPaddedString5() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(max_value); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

                            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString5();

                            // 換装先が持つ特殊能力一覧
                            farray = new string[1];
                            var loopTo3 = withBlock1.CountFeature();
                            for (j = 1; j <= loopTo3; j++)
                            {
                                object argIndex4 = j;
                                if (!string.IsNullOrEmpty(withBlock1.FeatureName(ref argIndex4)))
                                {
                                    // 重複する特殊能力は表示しないようチェック
                                    var loopTo4 = Information.UBound(farray);
                                    for (k = 1; k <= loopTo4; k++)
                                    {
                                        object argIndex2 = j;
                                        if ((withBlock1.FeatureName(ref argIndex2) ?? "") == (farray[k] ?? ""))
                                        {
                                            break;
                                        }
                                    }

                                    if (k > Information.UBound(farray))
                                    {
                                        string localFeatureName() { object argIndex1 = j; var ret = withBlock1.FeatureName(ref argIndex1); return ret; }

                                        GUI.ListItemComment[Information.UBound(list)] = GUI.ListItemComment[Information.UBound(list)] + localFeatureName() + " ";
                                        Array.Resize(ref farray, Information.UBound(farray) + 1 + 1);
                                        object argIndex3 = j;
                                        farray[Information.UBound(farray)] = withBlock1.FeatureName(ref argIndex3);
                                    }
                                }
                            }
                        }
                    }
                }

                GUI.ListItemFlag = new bool[Information.UBound(list) + 1];

                // どの形態に換装するかを選択
                GUI.TopItem = 1;
                string arglb_caption = "変更先選択";
                string argtname = "ＨＰ";
                Unit argu = null;
                string argtname1 = "ＥＮ";
                Unit argu1 = null;
                string argtname2 = "装甲";
                Unit argu2 = null;
                string argtname3 = "運動";
                Unit argu3 = null;
                string arglb_info = "ユニット                     " + Expression.Term(ref argtname, ref argu, 4) + " " + Expression.Term(ref argtname1, ref argu1, 4) + " " + Expression.Term(ref argtname2, ref argu2, 4) + " " + Expression.Term(ref argtname3, ref argu3, 4) + " " + "適応 攻撃力 射程";
                string arglb_mode = "カーソル移動,コメント";
                ret = GUI.ListBox(ref arglb_caption, ref list, ref arglb_info, ref arglb_mode);
                if (ret == 0)
                {
                    CancelCommand();
                    GUI.UnlockGUI();
                    return;
                }

                // 換装を実施
                Unit localOtherForm() { var tmp = id_list; object argIndex1 = tmp[ret]; var ret = withBlock.OtherForm(ref argIndex1); return ret; }

                string argnew_form = localOtherForm().Name;
                withBlock.Transform(ref argnew_form);
                localOtherForm().Name = argnew_form;

                // ユニットリストの再構築
                string argsmode = "";
                Event_Renamed.MakeUnitList(smode: ref argsmode);

                // カーソル自動移動
                if (SRC.AutoMoveCursor)
                {
                    string argcursor_mode = "ユニット選択";
                    GUI.MoveCursorPos(ref argcursor_mode, withBlock.CurrentForm());
                }

                Status.DisplayUnitStatus(ref withBlock.CurrentForm());
            }

            CommandState = "ユニット選択";
            GUI.UnlockGUI();
        }


        // 「発進」コマンドを開始
        // MOD START MARGE
        // Public Sub StartLaunchCommand()
        private void StartLaunchCommand()
        {
            // MOD END MARGE
            int i, ret;
            string[] list;
            {
                var withBlock = SelectedUnit;
                list = new string[(withBlock.CountUnitOnBoard() + 1)];
                GUI.ListItemID = new string[(withBlock.CountUnitOnBoard() + 1)];
                GUI.ListItemFlag = new bool[(withBlock.CountUnitOnBoard() + 1)];
            }

            // 母艦に搭載しているユニットの一覧を作成
            var loopTo = SelectedUnit.CountUnitOnBoard();
            for (i = 1; i <= loopTo; i++)
            {
                object argIndex1 = i;
                {
                    var withBlock1 = SelectedUnit.UnitOnBoard(ref argIndex1);
                    string localRightPaddedString() { string argbuf = withBlock1.Nickname0; var ret = GeneralLib.RightPaddedString(ref argbuf, 25); withBlock1.Nickname0 = argbuf; return ret; }

                    string localRightPaddedString1() { string argbuf = withBlock1.MainPilot().get_Nickname(false); var ret = GeneralLib.RightPaddedString(ref argbuf, 17); withBlock1.MainPilot().get_Nickname(false) = argbuf; return ret; }

                    string localLeftPaddedString() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MainPilot().Level); var ret = GeneralLib.LeftPaddedString(ref argbuf, 2); return ret; }

                    string localRightPaddedString2() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.HP) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MaxHP); var ret = GeneralLib.RightPaddedString(ref argbuf, 12); return ret; }

                    string localRightPaddedString3() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.EN) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MaxEN); var ret = GeneralLib.RightPaddedString(ref argbuf, 8); return ret; }

                    list[i] = localRightPaddedString() + localRightPaddedString1() + localLeftPaddedString() + " " + localRightPaddedString2() + localRightPaddedString3();
                    GUI.ListItemID[i] = withBlock1.ID;
                    if (withBlock1.Action > 0)
                    {
                        GUI.ListItemFlag[i] = false;
                    }
                    else
                    {
                        GUI.ListItemFlag[i] = true;
                    }
                }
            }

            // どのユニットを発進させるか選択
            GUI.TopItem = 1;
            string arglb_caption = "ユニット選択";
            string argtname = "ＨＰ";
            Unit argu = null;
            string argtname1 = "ＥＮ";
            Unit argu1 = null;
            string arglb_info = "ユニット名               パイロット       Lv " + Expression.Term(ref argtname, ref argu, 8) + Expression.Term(ref argtname1, u: ref argu1);
            string arglb_mode = "カーソル移動";
            ret = GUI.ListBox(ref arglb_caption, ref list, ref arglb_info, ref arglb_mode);

            // キャンセルされた？
            if (ret == 0)
            {
                GUI.ListItemID = new string[1];
                CancelCommand();
                return;
            }

            SelectedCommand = "発進";

            // ユニットの発進処理
            var tmp = GUI.ListItemID;
            object argIndex2 = tmp[ret];
            SelectedTarget = SRC.UList.Item(ref argIndex2);
            {
                var withBlock2 = SelectedTarget;
                withBlock2.x = SelectedUnit.x;
                withBlock2.y = SelectedUnit.y;
                string localLIndex() { object argIndex1 = "テレポート"; string arglist = withBlock2.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                int localLLength() { object argIndex1 = "ジャンプ"; string arglist = withBlock2.FeatureData(ref argIndex1); var ret = GeneralLib.LLength(ref arglist); return ret; }

                string localLIndex1() { object argIndex1 = "ジャンプ"; string arglist = withBlock2.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                string argfname = "テレポート";
                string argfname1 = "ジャンプ";
                if (withBlock2.IsFeatureAvailable(ref argfname) & (withBlock2.Data.Speed == 0 | localLIndex() == "0"))
                {
                    // テレポートによる発進
                    Map.AreaInTeleport(ref SelectedTarget);
                }
                else if (withBlock2.IsFeatureAvailable(ref argfname1) & (withBlock2.Data.Speed == 0 | localLLength() < 2 | localLIndex1() == "0"))
                {
                    // ジャンプによる発進
                    Map.AreaInSpeed(ref SelectedTarget, true);
                }
                else
                {
                    // 通常移動による発進
                    Map.AreaInSpeed(ref SelectedTarget);
                }

                // 母艦を中央表示
                GUI.Center(withBlock2.x, withBlock2.y);

                // 発進させるユニットを母艦の代わりに表示
                if (withBlock2.BitmapID == 0)
                {
                    object argIndex3 = withBlock2.Name;
                    {
                        var withBlock3 = SRC.UList.Item(ref argIndex3);
                        if ((SelectedTarget.Party0 ?? "") == (withBlock3.Party0 ?? "") & withBlock3.BitmapID != 0 & (SelectedTarget.get_Bitmap(false) ?? "") == (withBlock3.get_Bitmap(false) ?? ""))
                        {
                            SelectedTarget.BitmapID = withBlock3.BitmapID;
                        }
                        else
                        {
                            SelectedTarget.BitmapID = GUI.MakeUnitBitmap(ref SelectedTarget);
                        }
                    }

                    withBlock2.Name = Conversions.ToString(argIndex3);
                }

                GUI.MaskScreen();
            }

            GUI.ListItemID = new string[1];
            if (CommandState == "コマンド選択")
            {
                CommandState = "ターゲット選択";
            }
        }

        // 「発進」コマンドを終了
        // MOD START MARGE
        // Public Sub FinishLaunchCommand()
        private void FinishLaunchCommand()
        {
            // MOD END MARGE
            int ret;
            GUI.LockGUI();
            {
                var withBlock = SelectedTarget;
                // 発進コマンドの目的地にユニットがいた場合
                if (Map.MapDataForUnit[SelectedX, SelectedY] is object)
                {
                    string argfname = "母艦";
                    if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable(ref argfname))
                    {
                        ret = Interaction.MsgBox("着艦しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "着艦");
                    }
                    else
                    {
                        ret = Interaction.MsgBox("合体しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "合体");
                    }

                    if (ret == MsgBoxResult.Cancel)
                    {
                        CancelCommand();
                        GUI.UnlockGUI();
                        return;
                    }
                }

                // メッセージの表示
                string argmain_situation = "発進(" + withBlock.Name + ")";
                string argmain_situation1 = "発進";
                if (withBlock.IsMessageDefined(ref argmain_situation))
                {
                    Unit argu1 = null;
                    Unit argu2 = null;
                    GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                    string argSituation = "発進(" + withBlock.Name + ")";
                    string argmsg_mode = "";
                    withBlock.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                    GUI.CloseMessageForm();
                }
                else if (withBlock.IsMessageDefined(ref argmain_situation1))
                {
                    Unit argu11 = null;
                    Unit argu21 = null;
                    GUI.OpenMessageForm(u1: ref argu11, u2: ref argu21);
                    string argSituation1 = "発進";
                    string argmsg_mode1 = "";
                    withBlock.PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
                    GUI.CloseMessageForm();
                }

                string argmain_situation2 = "発進";
                string argsub_situation = withBlock.Name;
                withBlock.SpecialEffect(ref argmain_situation2, ref argsub_situation);
                withBlock.Name = argsub_situation;
                PrevUnitArea = withBlock.Area;
                PrevUnitEN = withBlock.EN;
                withBlock.Status_Renamed = "出撃";

                // 指定した位置に発進したユニットを移動
                withBlock.Move(SelectedX, SelectedY);
            }

            // 発進したユニットを母艦から降ろす
            {
                var withBlock1 = SelectedUnit;
                PrevUnitX = withBlock1.x;
                PrevUnitY = withBlock1.y;
                withBlock1.UnloadUnit(ref (object)SelectedTarget.ID);

                // 母艦の位置には発進したユニットが表示されているので元に戻しておく
                Map.MapDataForUnit[withBlock1.x, withBlock1.y] = SelectedUnit;
                GUI.PaintUnitBitmap(ref SelectedUnit);
            }

            SelectedUnit = SelectedTarget;
            {
                var withBlock2 = SelectedUnit;
                if ((Map.MapDataForUnit[withBlock2.x, withBlock2.y].ID ?? "") != (withBlock2.ID ?? ""))
                {
                    GUI.RedrawScreen();
                    CommandState = "ユニット選択";
                    GUI.UnlockGUI();
                    return;
                }
            }

            CommandState = "移動後コマンド選択";
            GUI.UnlockGUI();
            ProceedCommand();
        }


        // 「命令」コマンドを開始
        // MOD START MARGE
        // Public Sub StartOrderCommand()
        private void StartOrderCommand()
        {
            // MOD END MARGE
            string[] list;
            int i, ret, j;
            GUI.LockGUI();
            list = new string[5];
            GUI.ListItemFlag = new bool[5];

            // 可能な命令内容一覧を作成
            list[1] = "自由：自由に行動させる";
            list[2] = "移動：指定した位置に移動";
            list[3] = "攻撃：指定した敵を攻撃";
            list[4] = "護衛：指定したユニットを護衛";
            if (SelectedUnit.Summoner is object | SelectedUnit.Master is object)
            {
                Array.Resize(ref list, 6);
                Array.Resize(ref GUI.ListItemFlag, 6);
                if (SelectedUnit.Master is object)
                {
                    list[5] = "帰還：主人の所に戻る";
                }
                else
                {
                    list[5] = "帰還：召喚主の所に戻る";
                }
            }

            // 命令する行動パターンを選択
            string arglb_caption = "命令";
            string arglb_info = "行動パターン";
            string arglb_mode = "";
            ret = GUI.ListBox(ref arglb_caption, ref list, ref arglb_info, lb_mode: ref arglb_mode);

            // 選択された行動パターンに応じてターゲット領域を表示
            switch (ret)
            {
                case 0:
                    {
                        CancelCommand();
                        break;
                    }

                case 1: // 自由
                    {
                        SelectedUnit.Mode = "通常";
                        CommandState = "ユニット選択";
                        Status.DisplayUnitStatus(ref SelectedUnit);
                        break;
                    }

                case 2: // 移動
                    {
                        SelectedCommand = "移動命令";
                        var loopTo = Map.MapWidth;
                        for (i = 1; i <= loopTo; i++)
                        {
                            var loopTo1 = Map.MapHeight;
                            for (j = 1; j <= loopTo1; j++)
                                Map.MaskData[i, j] = false;
                        }

                        GUI.MaskScreen();
                        CommandState = "ターゲット選択";
                        break;
                    }

                case 3: // 攻撃
                    {
                        SelectedCommand = "攻撃命令";
                        string arguparty = "味方の敵";
                        Map.AreaWithUnit(ref arguparty);
                        Map.MaskData[SelectedUnit.x, SelectedUnit.y] = true;
                        GUI.MaskScreen();
                        CommandState = "ターゲット選択";
                        break;
                    }

                case 4: // 護衛
                    {
                        SelectedCommand = "護衛命令";
                        string arguparty1 = "味方";
                        Map.AreaWithUnit(ref arguparty1);
                        Map.MaskData[SelectedUnit.x, SelectedUnit.y] = true;
                        GUI.MaskScreen();
                        CommandState = "ターゲット選択";
                        break;
                    }

                case 5: // 帰還
                    {
                        if (SelectedUnit.Master is object)
                        {
                            SelectedUnit.Mode = SelectedUnit.Master.MainPilot().ID;
                        }
                        else
                        {
                            SelectedUnit.Mode = SelectedUnit.Summoner.MainPilot().ID;
                        }

                        CommandState = "ユニット選択";
                        Status.DisplayUnitStatus(ref SelectedUnit);
                        break;
                    }
            }

            GUI.UnlockGUI();
        }

        // 「命令」コマンドを終了
        // MOD START MARGE
        // Public Sub FinishOrderCommand()
        private void FinishOrderCommand()
        {
            // MOD END MARGE
            switch (SelectedCommand ?? "")
            {
                case "移動命令":
                    {
                        SelectedUnit.Mode = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SelectedX) + " " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SelectedY);
                        break;
                    }

                case "攻撃命令":
                case "護衛命令":
                    {
                        SelectedUnit.Mode = SelectedTarget.MainPilot().ID;
                        break;
                    }
            }

            if (ReferenceEquals(Status.DisplayedUnit, SelectedUnit))
            {
                Status.DisplayUnitStatus(ref SelectedUnit);
            }

            GUI.RedrawScreen();
            CommandState = "ユニット選択";
        }


        // 「特殊能力一覧」コマンド
        // MOD START MARGE
        // Public Sub FeatureListCommand()
        private void FeatureListCommand()
        {
            // MOD END MARGE
            string[] list;
            var id_list = default(string[]);
            bool[] is_unit_feature;
            int i, j;
            var ret = default;
            string fname0, fname, ftype;
            GUI.LockGUI();

            // 表示する特殊能力名一覧の作成
            list = new string[1];
            var id_ist = new object[1];
            is_unit_feature = new bool[1];

            // 武器・防具クラス
            string argoname = "アイテム交換";
            if (Expression.IsOptionDefined(ref argoname))
            {
                {
                    var withBlock = SelectedUnit;
                    string argfname = "武器クラス";
                    string argfname1 = "防具クラス";
                    if (withBlock.IsFeatureAvailable(ref argfname) | withBlock.IsFeatureAvailable(ref argfname1))
                    {
                        Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                        Array.Resize(ref id_list, Information.UBound(list) + 1);
                        Array.Resize(ref is_unit_feature, Information.UBound(list) + 1);
                        list[Information.UBound(list)] = "武器・防具クラス";
                        id_list[Information.UBound(list)] = "武器・防具クラス";
                        is_unit_feature[Information.UBound(list)] = true;
                    }
                }
            }

            {
                var withBlock1 = SelectedUnit.MainPilot();
                // パイロット特殊能力
                var loopTo = withBlock1.CountSkill();
                for (i = 1; i <= loopTo; i++)
                {
                    object argIndex3 = i;
                    switch (withBlock1.Skill(ref argIndex3) ?? "")
                    {
                        case "得意技":
                        case "不得手":
                            {
                                object argIndex1 = i;
                                fname = withBlock1.Skill(ref argIndex1);
                                break;
                            }

                        default:
                            {
                                object argIndex2 = i;
                                fname = withBlock1.SkillName(ref argIndex2);
                                break;
                            }
                    }

                    // 非表示の能力は除く
                    if (Strings.InStr(fname, "非表示") > 0)
                    {
                        goto NextSkill;
                    }

                    // 既に表示されていればスキップ
                    var loopTo1 = Information.UBound(list);
                    for (j = 1; j <= loopTo1; j++)
                    {
                        if ((list[j] ?? "") == (fname ?? ""))
                        {
                            goto NextSkill;
                        }
                    }

                    // リストに追加
                    Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                    Array.Resize(ref id_list, Information.UBound(list) + 1);
                    list[Information.UBound(list)] = fname;
                    id_list[Information.UBound(list)] = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(i);
                NextSkill:
                    ;
                }
            }

            {
                var withBlock2 = SelectedUnit;
                // 付加・強化されたパイロット用特殊能力
                var loopTo2 = withBlock2.CountCondition();
                for (i = 1; i <= loopTo2; i++)
                {
                    // パイロット能力付加または強化？
                    string localCondition() { object argIndex1 = i; var ret = withBlock2.Condition(ref argIndex1); return ret; }

                    string localCondition1() { object argIndex1 = i; var ret = withBlock2.Condition(ref argIndex1); return ret; }

                    if (Strings.Right(localCondition(), 3) != "付加２" & Strings.Right(localCondition1(), 3) != "強化２")
                    {
                        goto NextSkill2;
                    }

                    string localCondition2() { object argIndex1 = i; var ret = withBlock2.Condition(ref argIndex1); return ret; }

                    string localCondition3() { object argIndex1 = i; var ret = withBlock2.Condition(ref argIndex1); return ret; }

                    ftype = Strings.Left(localCondition2(), Strings.Len(localCondition3()) - 3);

                    // 非表示の能力？
                    string localConditionData() { object argIndex1 = i; var ret = withBlock2.ConditionData(ref argIndex1); return ret; }

                    string arglist = localConditionData();
                    switch (GeneralLib.LIndex(ref arglist, 1) ?? "")
                    {
                        case "非表示":
                        case "解説":
                            {
                                goto NextSkill2;
                                break;
                            }
                    }

                    // 有効時間が残っている？
                    object argIndex4 = i;
                    if (withBlock2.ConditionLifetime(ref argIndex4) == 0)
                    {
                        goto NextSkill2;
                    }

                    // 表示名称
                    object argIndex5 = ftype;
                    fname = withBlock2.MainPilot().SkillName(ref argIndex5);
                    if (Strings.InStr(fname, "非表示") > 0)
                    {
                        goto NextSkill2;
                    }

                    // 既に表示していればスキップ
                    var loopTo3 = Information.UBound(list);
                    for (j = 1; j <= loopTo3; j++)
                    {
                        if ((list[j] ?? "") == (fname ?? ""))
                        {
                            goto NextSkill2;
                        }
                    }

                    // リストに追加
                    Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                    Array.Resize(ref id_list, Information.UBound(list) + 1);
                    list[Information.UBound(list)] = fname;
                    id_list[Information.UBound(list)] = ftype;
                NextSkill2:
                    ;
                }

                Array.Resize(ref is_unit_feature, Information.UBound(list) + 1);

                // ユニット用特殊能力
                // 付加された特殊能力より先に固有の特殊能力を表示
                if (withBlock2.CountAllFeature() > withBlock2.AdditionalFeaturesNum)
                {
                    i = (withBlock2.AdditionalFeaturesNum + 1);
                }
                else
                {
                    i = 1;
                }

                while (i <= withBlock2.CountAllFeature())
                {
                    // 非表示の特殊能力を排除
                    object argIndex6 = i;
                    if (string.IsNullOrEmpty(withBlock2.AllFeatureName(ref argIndex6)))
                    {
                        goto NextFeature;
                    }

                    // 合体の場合は合体後の形態が作成されていなければならない
                    string localAllFeature() { object argIndex1 = i; var ret = withBlock2.AllFeature(ref argIndex1); return ret; }

                    string localAllFeatureData() { object argIndex1 = i; var ret = withBlock2.AllFeatureData(ref argIndex1); return ret; }

                    string localLIndex() { string arglist = hs7afb75fef08b43c283a05523ef7388cb(); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    bool localIsDefined() { object argIndex1 = (object)hse6256782c58b487b8147a3f247066e6f(); var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

                    if (localAllFeature() == "合体" & !localIsDefined())
                    {
                        goto NextFeature;
                    }

                    // 既に表示していればスキップ
                    var loopTo4 = Information.UBound(list);
                    for (j = 1; j <= loopTo4; j++)
                    {
                        string localAllFeatureName() { object argIndex1 = i; var ret = withBlock2.AllFeatureName(ref argIndex1); return ret; }

                        if ((list[j] ?? "") == (localAllFeatureName() ?? ""))
                        {
                            goto NextFeature;
                        }
                    }

                    // リストに追加
                    Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                    Array.Resize(ref id_list, Information.UBound(list) + 1);
                    Array.Resize(ref is_unit_feature, Information.UBound(list) + 1);
                    object argIndex7 = i;
                    list[Information.UBound(list)] = withBlock2.AllFeatureName(ref argIndex7);
                    id_list[Information.UBound(list)] = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(i);
                    is_unit_feature[Information.UBound(list)] = true;
                NextFeature:
                    ;
                    if (i == withBlock2.AdditionalFeaturesNum)
                    {
                        break;
                    }
                    else if (i == withBlock2.CountFeature())
                    {
                        // 付加された特殊能力は後から表示
                        if (withBlock2.AdditionalFeaturesNum > 0)
                        {
                            i = 0;
                        }
                    }

                    i = (i + 1);
                }

                // アビリティで付加・強化されたパイロット用特殊能力
                var loopTo5 = withBlock2.CountCondition();
                for (i = 1; i <= loopTo5; i++)
                {
                    // パイロット能力付加または強化？
                    string localCondition4() { object argIndex1 = i; var ret = withBlock2.Condition(ref argIndex1); return ret; }

                    string localCondition5() { object argIndex1 = i; var ret = withBlock2.Condition(ref argIndex1); return ret; }

                    if (Strings.Right(localCondition4(), 2) != "付加" & Strings.Right(localCondition5(), 2) != "強化")
                    {
                        goto NextSkill3;
                    }

                    string localCondition6() { object argIndex1 = i; var ret = withBlock2.Condition(ref argIndex1); return ret; }

                    string localCondition7() { object argIndex1 = i; var ret = withBlock2.Condition(ref argIndex1); return ret; }

                    ftype = Strings.Left(localCondition6(), Strings.Len(localCondition7()) - 2);

                    // 非表示の能力？
                    if (ftype == "メッセージ")
                    {
                        goto NextSkill3;
                    }

                    string localConditionData1() { object argIndex1 = i; var ret = withBlock2.ConditionData(ref argIndex1); return ret; }

                    string arglist1 = localConditionData1();
                    switch (GeneralLib.LIndex(ref arglist1, 1) ?? "")
                    {
                        case "非表示":
                        case "解説":
                            {
                                goto NextSkill3;
                                break;
                            }
                    }

                    // 有効時間が残っている？
                    object argIndex8 = i;
                    if (withBlock2.ConditionLifetime(ref argIndex8) == 0)
                    {
                        goto NextSkill3;
                    }

                    // 表示名称
                    object argIndex9 = ftype;
                    if (string.IsNullOrEmpty(withBlock2.FeatureName0(ref argIndex9)))
                    {
                        goto NextSkill3;
                    }

                    object argIndex10 = ftype;
                    fname = withBlock2.MainPilot().SkillName0(ref argIndex10);
                    if (Strings.InStr(fname, "非表示") > 0)
                    {
                        goto NextSkill3;
                    }

                    // 付加されたユニット用特殊能力として既に表示していればスキップ
                    var loopTo6 = Information.UBound(list);
                    for (j = 1; j <= loopTo6; j++)
                    {
                        if ((list[j] ?? "") == (fname ?? ""))
                        {
                            goto NextSkill3;
                        }
                    }

                    object argIndex11 = ftype;
                    fname = withBlock2.MainPilot().SkillName(ref argIndex11);
                    if (Strings.InStr(fname, "Lv") > 0)
                    {
                        fname0 = Strings.Left(fname, Strings.InStr(fname, "Lv") - 1);
                    }
                    else
                    {
                        fname0 = fname;
                    }

                    // パイロット用特殊能力として既に表示していればスキップ
                    var loopTo7 = Information.UBound(list);
                    for (j = 1; j <= loopTo7; j++)
                    {
                        if ((list[j] ?? "") == (fname ?? "") | (list[j] ?? "") == (fname0 ?? ""))
                        {
                            goto NextSkill3;
                        }
                    }

                    // リストに追加
                    Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                    Array.Resize(ref id_list, Information.UBound(list) + 1);
                    Array.Resize(ref is_unit_feature, Information.UBound(list) + 1);
                    list[Information.UBound(list)] = fname;
                    id_list[Information.UBound(list)] = ftype;
                    is_unit_feature[Information.UBound(list)] = false;
                NextSkill3:
                    ;
                }
            }

            GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
            switch (Information.UBound(list))
            {
                case 0:
                    {
                        break;
                    }

                case 1:
                    {
                        if (SRC.AutoMoveCursor)
                        {
                            GUI.SaveCursorPos();
                        }

                        if (id_list[ret] == "武器・防具クラス")
                        {
                            Help.FeatureHelp(ref SelectedUnit, id_list[1], false);
                        }
                        else if (is_unit_feature[1])
                        {
                            Help.FeatureHelp(ref SelectedUnit, id_list[1], GeneralLib.StrToLng(ref id_list[1]) <= SelectedUnit.AdditionalFeaturesNum);
                        }
                        else
                        {
                            Help.SkillHelp(ref SelectedUnit.MainPilot(), ref id_list[1]);
                        }

                        if (SRC.AutoMoveCursor)
                        {
                            GUI.RestoreCursorPos();
                        }

                        break;
                    }

                default:
                    {
                        GUI.TopItem = 1;
                        string arglb_caption = "特殊能力一覧";
                        string arglb_info = "能力名";
                        string arglb_mode = "表示のみ";
                        ret = GUI.ListBox(ref arglb_caption, ref list, ref arglb_info, ref arglb_mode);
                        if (SRC.AutoMoveCursor)
                        {
                            string argcursor_mode = "ダイアログ";
                            GUI.MoveCursorPos(ref argcursor_mode);
                        }

                        while (true)
                        {
                            string arglb_caption1 = "特殊能力一覧";
                            string arglb_info1 = "能力名";
                            string arglb_mode1 = "連続表示";
                            ret = GUI.ListBox(ref arglb_caption1, ref list, ref arglb_info1, ref arglb_mode1);
                            // listが一定なので連続表示を流用
                            My.MyProject.Forms.frmListBox.Hide();
                            if (ret == 0)
                            {
                                break;
                            }

                            if (id_list[ret] == "武器・防具クラス")
                            {
                                Help.FeatureHelp(ref SelectedUnit, id_list[ret], false);
                            }
                            else if (is_unit_feature[ret])
                            {
                                Help.FeatureHelp(ref SelectedUnit, id_list[ret], Conversions.ToDouble(id_list[ret]) <= SelectedUnit.AdditionalFeaturesNum);
                            }
                            else
                            {
                                Help.SkillHelp(ref SelectedUnit.MainPilot(), ref id_list[ret]);
                            }
                        }

                        if (SRC.AutoMoveCursor)
                        {
                            GUI.RestoreCursorPos();
                        }

                        break;
                    }
            }

            CommandState = "ユニット選択";
            GUI.UnlockGUI();
        }

        // 「武器一覧」コマンド
        // MOD START MARGE
        // Public Sub WeaponListCommand()
        private void WeaponListCommand()
        {
            // MOD END MARGE
            string[] list;
            int i;
            string buf;
            int w;
            string wclass;
            string atype, alevel;
            string c;
            GUI.LockGUI();
            int min_range, max_range;
            while (true)
            {
                string argcaption_msg = "武装一覧";
                string arglb_mode = "一覧";
                string argBGM = "";
                w = GUI.WeaponListBox(ref SelectedUnit, ref argcaption_msg, ref arglb_mode, BGM: ref argBGM);
                SelectedWeapon = w;
                if (SelectedWeapon <= 0)
                {
                    // キャンセル
                    if (SRC.AutoMoveCursor)
                    {
                        GUI.RestoreCursorPos();
                    }

                    My.MyProject.Forms.frmListBox.Hide();
                    GUI.UnlockGUI();
                    CommandState = "ユニット選択";
                    return;
                }

                // 指定された武器の属性一覧を作成
                list = new string[1];
                i = 0;
                {
                    var withBlock = SelectedUnit;
                    wclass = withBlock.WeaponClass(w);
                    while (i <= Strings.Len(wclass))
                    {
                        i = (i + 1);
                        buf = GeneralLib.GetClassBundle(ref wclass, ref i);
                        atype = "";
                        alevel = "";

                        // 非表示？
                        if (buf == "|")
                        {
                            break;
                        }

                        // Ｍ属性
                        if (Strings.Mid(wclass, i, 1) == "Ｍ")
                        {
                            i = (i + 1);
                            buf = buf + Strings.Mid(wclass, i, 1);
                        }

                        // レベル指定
                        if (Strings.Mid(wclass, i + 1, 1) == "L")
                        {
                            i = (i + 2);
                            c = Strings.Mid(wclass, i, 1);
                            while (Information.IsNumeric(c) | c == "." | c == "-")
                            {
                                alevel = alevel + c;
                                i = (i + 1);
                                c = Strings.Mid(wclass, i, 1);
                            }

                            i = (i - 1);
                        }

                        // 属性の名称
                        atype = Help.AttributeName(ref SelectedUnit, ref buf);
                        if (Strings.Len(atype) > 0)
                        {
                            Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                            if (Strings.Len(alevel) > 0)
                            {
                                string localRightPaddedString() { string argbuf = buf + "L" + alevel; var ret = GeneralLib.RightPaddedString(ref argbuf, 8); return ret; }

                                list[Information.UBound(list)] = localRightPaddedString() + atype + "レベル" + Strings.StrConv(alevel, VbStrConv.Wide);
                            }
                            else
                            {
                                list[Information.UBound(list)] = GeneralLib.RightPaddedString(ref buf, 8) + atype;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(Map.MapFileName))
                    {
                        Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                        list[Information.UBound(list)] = "射程範囲";
                    }

                    if (Information.UBound(list) > 0)
                    {
                        GUI.TopItem = 1;
                        while (true)
                        {
                            if (Information.UBound(list) == 1 & list[1] == "射程範囲")
                            {
                                i = 1;
                            }
                            else
                            {
                                GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
                                string arglb_caption = "武器属性一覧";
                                string arglb_info = "属性    効果";
                                string arglb_mode1 = "連続表示";
                                i = GUI.ListBox(ref arglb_caption, ref list, ref arglb_info, ref arglb_mode1);
                            }

                            if (i == 0)
                            {
                                // キャンセル
                                break;
                            }
                            else if (list[i] == "射程範囲")
                            {
                                My.MyProject.Forms.frmListBox.Hide();

                                // 武器の射程を求めておく
                                min_range = withBlock.Weapon(w).MinRange;
                                max_range = withBlock.WeaponMaxRange(w);

                                // 射程範囲表示
                                string argattr3 = "Ｐ";
                                string argattr4 = "Ｑ";
                                string argattr5 = "Ｍ直";
                                string argattr6 = "Ｍ拡";
                                string argattr7 = "Ｍ扇";
                                string argattr8 = "Ｍ全";
                                string argattr9 = "Ｍ線";
                                string argattr10 = "Ｍ投";
                                string argattr11 = "Ｍ移";
                                if ((max_range == 1 | withBlock.IsWeaponClassifiedAs(w, ref argattr3)) & !withBlock.IsWeaponClassifiedAs(w, ref argattr4))
                                {
                                    string arguparty = withBlock.Party + "の敵";
                                    Map.AreaInReachable(ref SelectedUnit, max_range, ref arguparty);
                                }
                                else if (withBlock.IsWeaponClassifiedAs(w, ref argattr5))
                                {
                                    Map.AreaInCross(withBlock.x, withBlock.y, min_range, ref max_range);
                                }
                                else if (withBlock.IsWeaponClassifiedAs(w, ref argattr6))
                                {
                                    Map.AreaInWideCross(withBlock.x, withBlock.y, min_range, ref max_range);
                                }
                                else if (withBlock.IsWeaponClassifiedAs(w, ref argattr7))
                                {
                                    string argattr = "Ｍ扇";
                                    Map.AreaInSectorCross(withBlock.x, withBlock.y, min_range, ref max_range, withBlock.WeaponLevel(w, ref argattr));
                                }
                                else if (withBlock.IsWeaponClassifiedAs(w, ref argattr8) | withBlock.IsWeaponClassifiedAs(w, ref argattr9))
                                {
                                    string arguparty2 = "すべて";
                                    Map.AreaInRange(withBlock.x, withBlock.y, max_range, min_range, ref arguparty2);
                                }
                                else if (withBlock.IsWeaponClassifiedAs(w, ref argattr10))
                                {
                                    string argattr1 = "Ｍ投";
                                    max_range = (max_range + withBlock.WeaponLevel(w, ref argattr1));
                                    string argattr2 = "Ｍ投";
                                    min_range = (min_range - withBlock.WeaponLevel(w, ref argattr2));
                                    min_range = GeneralLib.MaxLng(min_range, 1);
                                    string arguparty3 = "すべて";
                                    Map.AreaInRange(withBlock.x, withBlock.y, max_range, min_range, ref arguparty3);
                                }
                                else if (withBlock.IsWeaponClassifiedAs(w, ref argattr11))
                                {
                                    Map.AreaInMoveAction(ref SelectedUnit, max_range);
                                }
                                else
                                {
                                    string arguparty1 = withBlock.Party + "の敵";
                                    Map.AreaInRange(withBlock.x, withBlock.y, max_range, min_range, ref arguparty1);
                                }

                                GUI.Center(withBlock.x, withBlock.y);
                                GUI.MaskScreen();

                                // 先行入力されていたクリックイベントを解消
                                Application.DoEvents();
                                WaitClickMode = true;
                                GUI.IsFormClicked = false;

                                // クリックされるまで待つ
                                while (!GUI.IsFormClicked)
                                {
                                    GUI.Sleep(25);
                                    Application.DoEvents();
                                    if (GUI.IsRButtonPressed(true))
                                    {
                                        break;
                                    }
                                }

                                GUI.RedrawScreen();
                                if (Information.UBound(list) == 1 & list[i] == "射程範囲")
                                {
                                    break;
                                }
                            }
                            else
                            {
                                // 指定された属性の解説を表示
                                My.MyProject.Forms.frmListBox.Hide();
                                string argatr = GeneralLib.LIndex(ref list[i], 1);
                                Help.AttributeHelp(ref SelectedUnit, ref argatr, w);
                            }
                        }
                    }
                }
            }
        }

        // 「アビリティ一覧」コマンド
        // MOD START MARGE
        // Public Sub AbilityListCommand()
        private void AbilityListCommand()
        {
            // MOD END MARGE
            string[] list;
            int i;
            string buf;
            int a;
            string alevel, atype, aclass;
            string c;
            GUI.LockGUI();
            int min_range, max_range;
            while (true)
            {
                string argtname = "アビリティ";
                string argcaption_msg = Expression.Term(ref argtname, ref SelectedUnit) + "一覧";
                string arglb_mode = "一覧";
                a = GUI.AbilityListBox(ref SelectedUnit, ref argcaption_msg, ref arglb_mode);
                SelectedAbility = a;
                if (SelectedAbility <= 0)
                {
                    // キャンセル
                    if (SRC.AutoMoveCursor)
                    {
                        GUI.RestoreCursorPos();
                    }

                    My.MyProject.Forms.frmListBox.Hide();
                    GUI.UnlockGUI();
                    CommandState = "ユニット選択";
                    return;
                }

                // 指定されたアビリティの属性一覧を作成
                list = new string[1];
                i = 0;
                {
                    var withBlock = SelectedUnit;
                    aclass = withBlock.Ability(a).Class_Renamed;
                    while (i <= Strings.Len(aclass))
                    {
                        i = (i + 1);
                        buf = GeneralLib.GetClassBundle(ref aclass, ref i);
                        atype = "";
                        alevel = "";

                        // 非表示？
                        if (buf == "|")
                        {
                            break;
                        }

                        // Ｍ属性
                        if (Strings.Mid(aclass, i, 1) == "Ｍ")
                        {
                            i = (i + 1);
                            buf = buf + Strings.Mid(aclass, i, 1);
                        }

                        // レベル指定
                        if (Strings.Mid(aclass, i + 1, 1) == "L")
                        {
                            i = (i + 2);
                            c = Strings.Mid(aclass, i, 1);
                            while (Information.IsNumeric(c) | c == "." | c == "-")
                            {
                                alevel = alevel + c;
                                i = (i + 1);
                                c = Strings.Mid(aclass, i, 1);
                            }

                            i = (i - 1);
                        }

                        // 属性の名称
                        atype = Help.AttributeName(ref SelectedUnit, ref buf, true);
                        if (Strings.Len(atype) > 0)
                        {
                            Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                            if (Strings.Len(alevel) > 0)
                            {
                                string localRightPaddedString() { string argbuf = buf + "L" + alevel; var ret = GeneralLib.RightPaddedString(ref argbuf, 8); return ret; }

                                list[Information.UBound(list)] = localRightPaddedString() + atype + "レベル" + Strings.StrConv(alevel, VbStrConv.Wide);
                            }
                            else
                            {
                                list[Information.UBound(list)] = GeneralLib.RightPaddedString(ref buf, 8) + atype;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(Map.MapFileName))
                    {
                        Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                        list[Information.UBound(list)] = "射程範囲";
                    }

                    if (Information.UBound(list) > 0)
                    {
                        GUI.TopItem = 1;
                        while (true)
                        {
                            if (Information.UBound(list) == 1 & list[1] == "射程範囲")
                            {
                                i = 1;
                            }
                            else
                            {
                                GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
                                string arglb_caption = "アビリティ属性一覧";
                                string arglb_info = "属性    効果";
                                string arglb_mode1 = "連続表示";
                                i = GUI.ListBox(ref arglb_caption, ref list, ref arglb_info, ref arglb_mode1);
                            }

                            if (i == 0)
                            {
                                // キャンセル
                                break;
                            }
                            else if (list[i] == "射程範囲")
                            {
                                My.MyProject.Forms.frmListBox.Hide();

                                // アビリティの射程を求めておく
                                min_range = withBlock.AbilityMinRange(a);
                                max_range = withBlock.AbilityMaxRange(a);

                                // 射程範囲表示
                                string argattr3 = "Ｐ";
                                string argattr4 = "Ｑ";
                                string argattr5 = "Ｍ直";
                                string argattr6 = "Ｍ拡";
                                string argattr7 = "Ｍ扇";
                                string argattr8 = "Ｍ投";
                                string argattr9 = "Ｍ移";
                                if ((max_range == 1 | withBlock.IsAbilityClassifiedAs(a, ref argattr3)) & !withBlock.IsAbilityClassifiedAs(a, ref argattr4))
                                {
                                    string arguparty = "すべて";
                                    Map.AreaInReachable(ref SelectedUnit, max_range, ref arguparty);
                                }
                                else if (withBlock.IsAbilityClassifiedAs(a, ref argattr5))
                                {
                                    Map.AreaInCross(withBlock.x, withBlock.y, min_range, ref max_range);
                                }
                                else if (withBlock.IsAbilityClassifiedAs(a, ref argattr6))
                                {
                                    Map.AreaInWideCross(withBlock.x, withBlock.y, min_range, ref max_range);
                                }
                                else if (withBlock.IsAbilityClassifiedAs(a, ref argattr7))
                                {
                                    string argattr = "Ｍ扇";
                                    Map.AreaInSectorCross(withBlock.x, withBlock.y, min_range, ref max_range, withBlock.AbilityLevel(a, ref argattr));
                                }
                                else if (withBlock.IsAbilityClassifiedAs(a, ref argattr8))
                                {
                                    string argattr1 = "Ｍ投";
                                    max_range = (max_range + withBlock.AbilityLevel(a, ref argattr1));
                                    string argattr2 = "Ｍ投";
                                    min_range = (min_range - withBlock.AbilityLevel(a, ref argattr2));
                                    min_range = GeneralLib.MaxLng(min_range, 1);
                                    string arguparty2 = "すべて";
                                    Map.AreaInRange(withBlock.x, withBlock.y, max_range, min_range, ref arguparty2);
                                }
                                else if (withBlock.IsAbilityClassifiedAs(a, ref argattr9))
                                {
                                    Map.AreaInMoveAction(ref SelectedUnit, max_range);
                                }
                                else
                                {
                                    string arguparty1 = "すべて";
                                    Map.AreaInRange(withBlock.x, withBlock.y, max_range, min_range, ref arguparty1);
                                }

                                GUI.Center(withBlock.x, withBlock.y);
                                GUI.MaskScreen();

                                // 先行入力されていたクリックイベントを解消
                                Application.DoEvents();
                                WaitClickMode = true;
                                GUI.IsFormClicked = false;

                                // クリックされるまで待つ
                                while (!GUI.IsFormClicked)
                                {
                                    GUI.Sleep(25);
                                    Application.DoEvents();
                                    if (GUI.IsRButtonPressed(true))
                                    {
                                        break;
                                    }
                                }

                                GUI.RedrawScreen();
                                if (Information.UBound(list) == 1 & list[i] == "射程範囲")
                                {
                                    break;
                                }
                            }
                            else
                            {
                                // 指定された属性の解説を表示
                                My.MyProject.Forms.frmListBox.Hide();
                                string argatr = GeneralLib.LIndex(ref list[i], 1);
                                Help.AttributeHelp(ref SelectedUnit, ref argatr, a, true);
                            }
                        }
                    }
                }
            }
        }

        // 「移動範囲」コマンド
        // MOD START MARGE
        // Public Sub ShowAreaInSpeedCommand()
        private void ShowAreaInSpeedCommand()
        {
            // MOD END MARGE
            SelectedCommand = "移動範囲";
            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Status.ClearUnitStatus();
            }

            Map.AreaInSpeed(ref SelectedUnit);
            GUI.Center(SelectedUnit.x, SelectedUnit.y);
            GUI.MaskScreen();
            CommandState = "ターゲット選択";
        }

        // 「射程範囲」コマンド
        // MOD START MARGE
        // Public Sub ShowAreaInRangeCommand()
        private void ShowAreaInRangeCommand()
        {
            // MOD END MARGE
            int w, i, max_range;
            SelectedCommand = "射程範囲";

            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Status.ClearUnitStatus();
            }

            {
                var withBlock = SelectedUnit;
                // 最大の射程を持つ武器を探す
                w = 0;
                max_range = 0;
                var loopTo = withBlock.CountWeapon();
                for (i = 1; i <= loopTo; i++)
                {
                    string argref_mode = "ステータス";
                    string argattr = "Ｍ";
                    if (withBlock.IsWeaponAvailable(i, ref argref_mode) & !withBlock.IsWeaponClassifiedAs(i, ref argattr))
                    {
                        if (withBlock.WeaponMaxRange(i) > max_range)
                        {
                            w = i;
                            max_range = withBlock.WeaponMaxRange(i);
                        }
                    }
                }

                // 見つかった最大の射程を持つ武器の射程範囲を選択
                string arguparty = withBlock.Party + "の敵";
                Map.AreaInRange(withBlock.x, withBlock.y, max_range, 1, ref arguparty);

                // 射程範囲を表示
                GUI.Center(withBlock.x, withBlock.y);
                GUI.MaskScreen();
            }

            CommandState = "ターゲット選択";
        }

        // 「待機」コマンド
        // 他のコマンドの終了処理にも使われる
        // MOD START MARGE
        // Public Sub WaitCommand(Optional ByVal WithoutAction As Boolean)
        // 今後どうしてもPrivateじゃダメな処理が出たら戻してください
        private void WaitCommand(bool WithoutAction = false)
        {
            // MOD END MARGE
            Pilot p;
            int i;

            // コマンド終了時はターゲットを解除
            // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            SelectedTarget = null;

            // ユニットにパイロットが乗っていない？
            if (SelectedUnit.CountPilot() == 0)
            {
                CommandState = "ユニット選択";
                GUI.RedrawScreen();
                Status.ClearUnitStatus();
                return;
            }

            if (!WithoutAction)
            {
                // 残り行動数を減少させる
                SelectedUnit.UseAction();

                // 持続期間が「移動」のスペシャルパワー効果を削除
                if (Strings.InStr(CommandState, "移動後") > 0)
                {
                    string argstype = "移動";
                    SelectedUnit.RemoveSpecialPowerInEffect(ref argstype);
                }
            }

            CommandState = "ユニット選択";

            // アップデート
            SelectedUnit.Update();
            SRC.PList.UpdateSupportMod(SelectedUnit);

            // ユニットが既に出撃していない？
            if (SelectedUnit.Status_Renamed != "出撃")
            {
                GUI.RedrawScreen();
                Status.ClearUnitStatus();
                return;
            }

            GUI.LockGUI();
            GUI.RedrawScreen();
            object argIndex1 = 1;
            p = SelectedUnit.Pilot(ref argIndex1);

            // 接触イベント
            for (i = 1; i <= 4; i++)
            {
                // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                SelectedTarget = null;
                {
                    var withBlock = SelectedUnit;
                    switch (i)
                    {
                        case 1:
                            {
                                if (withBlock.x > 1)
                                {
                                    SelectedTarget = Map.MapDataForUnit[withBlock.x - 1, withBlock.y];
                                }

                                break;
                            }

                        case 2:
                            {
                                if (withBlock.x < Map.MapWidth)
                                {
                                    SelectedTarget = Map.MapDataForUnit[withBlock.x + 1, withBlock.y];
                                }

                                break;
                            }

                        case 3:
                            {
                                if (withBlock.y > 1)
                                {
                                    SelectedTarget = Map.MapDataForUnit[withBlock.x, withBlock.y - 1];
                                }

                                break;
                            }

                        case 4:
                            {
                                if (withBlock.y < Map.MapHeight)
                                {
                                    SelectedTarget = Map.MapDataForUnit[withBlock.x, withBlock.y + 1];
                                }

                                break;
                            }
                    }
                }

                if (SelectedTarget is object)
                {
                    Event_Renamed.HandleEvent("接触", SelectedUnit.MainPilot().ID, SelectedTarget.MainPilot().ID);
                    // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                    SelectedTarget = null;
                    if (SRC.IsScenarioFinished)
                    {
                        SRC.IsScenarioFinished = false;
                        return;
                    }

                    if (SelectedUnit.Status_Renamed != "出撃")
                    {
                        GUI.RedrawScreen();
                        Status.ClearUnitStatus();
                        GUI.UnlockGUI();
                        return;
                    }
                }
            }

            // 進入イベント
            Event_Renamed.HandleEvent("進入", SelectedUnit.MainPilot().ID, SelectedUnit.x, SelectedUnit.y);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                return;
            }

            if (SelectedUnit.CountPilot() == 0)
            {
                GUI.RedrawScreen();
                Status.ClearUnitStatus();
                GUI.UnlockGUI();
                return;
            }

            // 行動終了イベント
            Event_Renamed.HandleEvent("行動終了", SelectedUnit.MainPilot().ID);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                return;
            }

            if (SelectedUnit.CountPilot() == 0)
            {
                GUI.RedrawScreen();
                Status.ClearUnitStatus();
                GUI.UnlockGUI();
                return;
            }

            if (p.Unit_Renamed is object)
            {
                SelectedUnit = p.Unit_Renamed;
            }

            if (SelectedUnit.Action > 0 & SelectedUnit.CountPilot() > 0)
            {
                // カーソル自動移動
                if (SRC.AutoMoveCursor)
                {
                    string argcursor_mode = "ユニット選択";
                    GUI.MoveCursorPos(ref argcursor_mode, SelectedUnit);
                }
            }

            // ハイパーモード・ノーマルモードの自動発動をチェック
            SelectedUnit.CurrentForm().CheckAutoHyperMode();
            SelectedUnit.CurrentForm().CheckAutoNormalMode();
            if (GUI.IsPictureVisible | GUI.IsCursorVisible)
            {
                GUI.RedrawScreen();
            }

            GUI.UnlockGUI();

            // ステータスウィンドウの表示内容を更新
            if (SelectedUnit.Status_Renamed == "出撃" & GUI.MainWidth == 15)
            {
                Status.DisplayUnitStatus(ref SelectedUnit);
            }
            else
            {
                Status.ClearUnitStatus();
            }
        }
    }
}