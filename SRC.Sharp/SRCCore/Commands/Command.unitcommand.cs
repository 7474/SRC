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
        // ユニットコマンドを実行
        public void UnitCommand(UiCommand command)
        {
            LogDebug();

            PrevCommand = SelectedCommand;
            var unit = SelectedUnit;
            var prev_used_action = unit.UsedAction;
            switch (command.Id)
            {
                case MoveCmdID: // 移動
                    // なんらかの原因により、ユニットコマンドの選択がうまくいかなかった場合は
                    // 移動後のコマンド選択をやり直す
                    if (CommandState == "移動後コマンド選択")
                    {
                        //Application.DoEvents();
                        return;
                    }

                    if (command.Label == "移動")
                    {
                        StartMoveCommand();
                    }
                    else
                    {
                        ShowAreaInSpeedCommand();
                    }
                    break;

                //        case TeleportCmdID: // テレポート
                //            {
                //                StartTeleportCommand();
                //                break;
                //            }

                //        case JumpCmdID: // ジャンプ
                //            {
                //                StartJumpCommand();
                //                break;
                //            }

                //        case TalkCmdID: // 会話
                //            {
                //                StartTalkCommand();
                //                break;
                //            }

                case AttackCmdID: // 攻撃
                    if (command.Label == "攻撃")
                    {
                        StartAttackCommand();
                    }
                    else
                    {
                        ShowAreaInRangeCommand();
                    }
                    break;


                //        case FixCmdID: // 修理
                //            {
                //                StartFixCommand();
                //                break;
                //            }

                //        case SupplyCmdID: // 補給
                //            {
                //                StartSupplyCommand();
                //                break;
                //            }

                //        case AbilityCmdID: // アビリティ
                //            {
                //                StartAbilityCommand();
                //                break;
                //            }

                //        case ChargeCmdID: // チャージ
                //            {
                //                ChargeCommand();
                //                break;
                //            }

                //        case SpecialPowerCmdID: // 精神
                //            {
                //                StartSpecialPowerCommand();
                //                break;
                //            }

                //        case TransformCmdID: // 変形
                //            {
                //                TransformCommand();
                //                break;
                //            }

                //        case SplitCmdID: // 分離
                //            {
                //                SplitCommand();
                //                break;
                //            }

                //        case CombineCmdID: // 合体
                //            {
                //                CombineCommand();
                //                break;
                //            }

                //        case HyperModeCmdID: // ハイパーモード・変身解除
                //            {
                //                object argIndex1 = "ノーマルモード";
                //                if (Strings.InStr(unit.FeatureData(argIndex1), "手動解除") > 0)
                //                {
                //                    CancelTransformationCommand();
                //                }
                //                else
                //                {
                //                    HyperModeCommand();
                //                }

                //                break;
                //            }

                case GroundCmdID: // 地上
                    {
                        GUI.LockGUI();
                        if (Map.Terrain(unit.x, unit.y).Class == "水" || Map.Terrain(unit.x, unit.y).Class == "深水")
                        {
                            unit.Area = "水上";
                        }
                        else
                        {
                            unit.Area = "地上";
                        }

                        unit.Update();
                        // XXX
                        //if (unit.IsMessageDefined(unit.Area))
                        //{
                        //    Unit argu1 = null;
                        //    Unit argu2 = null;
                        //    GUI.OpenMessageForm(u1: argu1, u2: argu2);
                        //    string argmsg_mode = "";
                        //    unit.PilotMessage(unit.Area, msg_mode: argmsg_mode);
                        //    GUI.CloseMessageForm();
                        //}

                        GUI.PaintUnitBitmap(SelectedUnit);
                        CommandState = "ユニット選択";
                        GUI.UnlockGUI();
                        break;
                    }

                case SkyCmdID: // 空中
                    {
                        GUI.LockGUI();
                        if (Map.Terrain(unit.x, unit.y).Class == "月面")
                        {
                            unit.Area = "宇宙";
                        }
                        else
                        {
                            unit.Area = "空中";
                        }

                        unit.Update();
                        // XXX
                        //if (unit.IsMessageDefined(unit.Area))
                        //{
                        //    Unit argu11 = null;
                        //    Unit argu21 = null;
                        //    GUI.OpenMessageForm(u1: argu11, u2: argu21);
                        //    string argmsg_mode1 = "";
                        //    unit.PilotMessage(unit.Area, msg_mode: argmsg_mode1);
                        //    GUI.CloseMessageForm();
                        //}

                        GUI.PaintUnitBitmap(SelectedUnit);
                        CommandState = "ユニット選択";
                        GUI.UnlockGUI();
                        break;
                    }

                case UndergroundCmdID: // 地中
                    {
                        GUI.LockGUI();
                        unit.Area = "地中";
                        unit.Update();
                        // XXX
                        //if (unit.IsMessageDefined(unit.Area))
                        //{
                        //    Unit argu12 = null;
                        //    Unit argu22 = null;
                        //    GUI.OpenMessageForm(u1: argu12, u2: argu22);
                        //    string argmsg_mode2 = "";
                        //    unit.PilotMessage(unit.Area, msg_mode: argmsg_mode2);
                        //    GUI.CloseMessageForm();
                        //}

                        GUI.PaintUnitBitmap(SelectedUnit);
                        CommandState = "ユニット選択";
                        GUI.UnlockGUI();
                        break;
                    }

                case WaterCmdID: // 水中
                    {
                        GUI.LockGUI();
                        unit.Area = "水中";
                        unit.Update();
                        // XXX
                        //if (unit.IsMessageDefined(unit.Area))
                        //{
                        //    Unit argu13 = null;
                        //    Unit argu23 = null;
                        //    GUI.OpenMessageForm(u1: argu13, u2: argu23);
                        //    string argmsg_mode3 = "";
                        //    unit.PilotMessage(unit.Area, msg_mode: argmsg_mode3);
                        //    GUI.CloseMessageForm();
                        //}

                        GUI.PaintUnitBitmap(SelectedUnit);
                        CommandState = "ユニット選択";
                        GUI.UnlockGUI();
                        break;
                    }

                //        case LaunchCmdID: // 発進
                //            {
                //                StartLaunchCommand();
                //                break;
                //            }

                //        case ItemCmdID: // アイテム
                //            {
                //                StartAbilityCommand(true);
                //                break;
                //            }

                //        case DismissCmdID: // 召喚解除
                //            {
                //                GUI.LockGUI();

                //                // 召喚解除の使用イベント
                //                Event_Renamed.HandleEvent("使用", unit.MainPilot().ID, "召喚解除");
                //                if (SRC.IsScenarioFinished)
                //                {
                //                    SRC.IsScenarioFinished = false;
                //                    GUI.UnlockGUI();
                //                    return;
                //                }

                //                if (SRC.IsCanceled)
                //                {
                //                    SRC.IsCanceled = false;
                //                    return;
                //                }

                //                // 召喚ユニットを解放
                //                unit.DismissServant();

                //                // 召喚解除の使用後イベント
                //                Event_Renamed.HandleEvent("使用後", unit.CurrentForm().MainPilot().ID, "召喚解除");
                //                if (SRC.IsScenarioFinished)
                //                {
                //                    SRC.IsScenarioFinished = false;
                //                }

                //                if (SRC.IsCanceled)
                //                {
                //                    SRC.IsCanceled = false;
                //                }

                //                GUI.UnlockGUI();
                //                break;
                //            }

                //        case OrderCmdID: // 命令/換装
                //            {
                //                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //                if (GUI.MainForm.mnuUnitCommandItem.Item(OrderCmdID).Caption == "命令")
                //                {
                //                    StartOrderCommand();
                //                }
                //                else
                //                {
                //                    ExchangeFormCommand();
                //                }

                //                break;
                //            }

                //        case FeatureListCmdID: // 特殊能力一覧
                //            {
                //                FeatureListCommand();
                //                break;
                //            }

                //        case WeaponListCmdID: // 武器一覧
                //            {
                //                WeaponListCommand();
                //                break;
                //            }

                //        case AbilityListCmdID: // アビリティ一覧
                //            {
                //                AbilityListCommand();
                //                break;
                //            }

                //        case var @case when UnitCommand1CmdID <= @case && @case <= UnitCommand10CmdID: // ユニットコマンド
                //            {
                //                GUI.LockGUI();

                //                // ユニットコマンドの使用イベント
                //                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //                Event_Renamed.HandleEvent("使用", unit.MainPilot().ID, GUI.MainForm.mnuUnitCommandItem.Item(idx).Caption);
                //                if (SRC.IsScenarioFinished)
                //                {
                //                    SRC.IsScenarioFinished = false;
                //                    GUI.UnlockGUI();
                //                    return;
                //                }

                //                if (SRC.IsCanceled)
                //                {
                //                    SRC.IsCanceled = false;
                //                    WaitCommand();
                //                    return;
                //                }

                //                // ユニットコマンドを実行
                //                Event_Renamed.HandleEvent(UnitCommandLabelList[idx - UnitCommand1CmdID + 1]);
                //                if (SRC.IsCanceled)
                //                {
                //                    SRC.IsCanceled = false;
                //                    CancelCommand();
                //                    GUI.UnlockGUI();
                //                    return;
                //                }

                //                // ユニットコマンドの使用後イベント
                //                if (unit.CurrentForm().CountPilot() > 0)
                //                {
                //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //                    Event_Renamed.HandleEvent("使用後", unit.CurrentForm().MainPilot().ID, GUI.MainForm.mnuUnitCommandItem.Item(idx).Caption);
                //                    if (SRC.IsScenarioFinished)
                //                    {
                //                        SRC.IsScenarioFinished = false;
                //                        GUI.UnlockGUI();
                //                        return;
                //                    }
                //                }

                //                // ステータスウィンドウを更新
                //                if (unit.CurrentForm().CountPilot() > 0)
                //                {
                //                    Status.DisplayUnitStatus(unit.CurrentForm());
                //                }

                //                // 行動終了
                //                if (unit.CurrentForm().UsedAction <= prev_used_action)
                //                {
                //                    if (CommandState == "移動後コマンド選択")
                //                    {
                //                        WaitCommand();
                //                    }
                //                    else
                //                    {
                //                        CommandState = "ユニット選択";
                //                        GUI.UnlockGUI();
                //                    }
                //                }
                //                else if (SRC.IsCanceled)
                //                {
                //                    SRC.IsCanceled = false;
                //                }
                //                else
                //                {
                //                    WaitCommand(true);
                //                }

                //                break;
                //            }

                case WaitCmdID: // 待機
                    WaitCommand();
                    break;

                default:
                    // なんらかの原因により、ユニットコマンドの選択がうまくいかなかった場合は
                    // 移動後のコマンド選択をやり直す
                    if (CommandState == "移動後コマンド選択")
                    {
                        // XXX
                        //Application.DoEvents();
                        return;
                    }
                    break;
            }
        }

        // 「待機」コマンド
        // 他のコマンドの終了処理にも使われる
        private void WaitCommand(bool WithoutAction = false)
        {
            LogDebug();

            // コマンド終了時はターゲットを解除
            SelectedTarget = null;

            //// ユニットにパイロットが乗っていない？
            //if (SelectedUnit.CountPilot() == 0)
            //{
            //    CommandState = "ユニット選択";
            //    GUI.RedrawScreen();
            //    Status.ClearUnitStatus();
            //    return;
            //}

            if (!WithoutAction)
            {
                // 残り行動数を減少させる
                SelectedUnit.UseAction();

                // 持続期間が「移動」のスペシャルパワー効果を削除
                if (Strings.InStr(CommandState, "移動後") > 0)
                {
                    SelectedUnit.RemoveSpecialPowerInEffect("移動");
                }
            }

            CommandState = "ユニット選択";

            //// アップデート
            //SelectedUnit.Update();
            //SRC.PList.UpdateSupportMod(SelectedUnit);

            //// ユニットが既に出撃していない？
            //if (SelectedUnit.Status_Renamed != "出撃")
            //{
            //    GUI.RedrawScreen();
            //    Status.ClearUnitStatus();
            //    return;
            //}

            GUI.LockGUI();
            GUI.RedrawScreen();
            //object argIndex1 = 1;
            //p = SelectedUnit.Pilot(argIndex1);

            //// 接触イベント
            //for (i = 1; i <= 4; i++)
            //{
            //    // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    SelectedTarget = null;
            //    {
            //        var unit = SelectedUnit;
            //        switch (i)
            //        {
            //            case 1:
            //                {
            //                    if (unit.x > 1)
            //                    {
            //                        SelectedTarget = Map.MapDataForUnit[unit.x - 1, unit.y];
            //                    }

            //                    break;
            //                }

            //            case 2:
            //                {
            //                    if (unit.x < Map.MapWidth)
            //                    {
            //                        SelectedTarget = Map.MapDataForUnit[unit.x + 1, unit.y];
            //                    }

            //                    break;
            //                }

            //            case 3:
            //                {
            //                    if (unit.y > 1)
            //                    {
            //                        SelectedTarget = Map.MapDataForUnit[unit.x, unit.y - 1];
            //                    }

            //                    break;
            //                }

            //            case 4:
            //                {
            //                    if (unit.y < Map.MapHeight)
            //                    {
            //                        SelectedTarget = Map.MapDataForUnit[unit.x, unit.y + 1];
            //                    }

            //                    break;
            //                }
            //        }
            //    }

            //    if (SelectedTarget is object)
            //    {
            //        Event_Renamed.HandleEvent("接触", SelectedUnit.MainPilot().ID, SelectedTarget.MainPilot().ID);
            //        // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //        SelectedTarget = null;
            //        if (SRC.IsScenarioFinished)
            //        {
            //            SRC.IsScenarioFinished = false;
            //            return;
            //        }

            //        if (SelectedUnit.Status_Renamed != "出撃")
            //        {
            //            GUI.RedrawScreen();
            //            Status.ClearUnitStatus();
            //            GUI.UnlockGUI();
            //            return;
            //        }
            //    }
            //}

            //// 進入イベント
            //Event_Renamed.HandleEvent("進入", SelectedUnit.MainPilot().ID, SelectedUnit.x, SelectedUnit.y);
            //if (SRC.IsScenarioFinished)
            //{
            //    SRC.IsScenarioFinished = false;
            //    return;
            //}

            //if (SelectedUnit.CountPilot() == 0)
            //{
            //    GUI.RedrawScreen();
            //    Status.ClearUnitStatus();
            //    GUI.UnlockGUI();
            //    return;
            //}

            // 行動終了イベント
            Event.HandleEvent("行動終了", SelectedUnit.MainPilot().ID);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                return;
            }

            //if (SelectedUnit.CountPilot() == 0)
            //{
            //    GUI.RedrawScreen();
            //    Status.ClearUnitStatus();
            //    GUI.UnlockGUI();
            //    return;
            //}

            //if (p.Unit_Renamed is object)
            //{
            //    SelectedUnit = p.Unit_Renamed;
            //}

            //if (SelectedUnit.Action > 0 & SelectedUnit.CountPilot() > 0)
            //{
            //    // カーソル自動移動
            //    if (SRC.AutoMoveCursor)
            //    {
            //        string argcursor_mode = "ユニット選択";
            //        GUI.MoveCursorPos(argcursor_mode, SelectedUnit);
            //    }
            //}

            //// ハイパーモード・ノーマルモードの自動発動をチェック
            //SelectedUnit.CurrentForm().CheckAutoHyperMode();
            //SelectedUnit.CurrentForm().CheckAutoNormalMode();
            if (GUI.IsPictureVisible || GUI.IsCursorVisible)
            {
                GUI.RedrawScreen();
            }

            GUI.UnlockGUI();

            //// ステータスウィンドウの表示内容を更新
            //if (SelectedUnit.Status_Renamed == "出撃" & GUI.MainWidth == 15)
            //{
            //    Status.DisplayUnitStatus(SelectedUnit);
            //}
            //else
            //{
            //    Status.ClearUnitStatus();
            //}
        }
    }
}