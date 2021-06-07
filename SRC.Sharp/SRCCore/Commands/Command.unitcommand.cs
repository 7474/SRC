// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.VB;
using System.Linq;

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

                case TeleportCmdID: // テレポート
                    StartTeleportCommand();
                    break;

                case JumpCmdID: // ジャンプ
                    StartJumpCommand();
                    break;

                case TalkCmdID: // 会話
                    StartTalkCommand();
                    break;

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


                case FixCmdID: // 修理
                    StartFixCommand();
                    break;

                case SupplyCmdID: // 補給
                    StartSupplyCommand();
                    break;

                case AbilityCmdID: // アビリティ
                    StartAbilityCommand();
                    break;

                case ChargeCmdID: // チャージ
                    ChargeCommand();
                    break;

                case SpecialPowerCmdID: // 精神
                    StartSpecialPowerCommand();
                    break;

                case TransformCmdID: // 変形
                    TransformCommand();
                    break;

                case SplitCmdID: // 分離
                    SplitCommand();
                    break;

                case CombineCmdID: // 合体
                    CombineCommand(command);
                    break;

                case HyperModeCmdID: // ハイパーモード・変身解除
                    if (Strings.InStr(unit.FeatureData("ノーマルモード"), "手動解除") > 0)
                    {
                        CancelTransformationCommand();
                    }
                    else
                    {
                        HyperModeCommand();
                    }
                    break;

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
                        if (unit.IsMessageDefined(unit.Area))
                        {
                            GUI.OpenMessageForm(u1: null, u2: null);
                            unit.PilotMessage(unit.Area, msg_mode: "");
                            GUI.CloseMessageForm();
                        }

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
                        if (unit.IsMessageDefined(unit.Area))
                        {
                            GUI.OpenMessageForm(u1: null, u2: null);
                            unit.PilotMessage(unit.Area, msg_mode: "");
                            GUI.CloseMessageForm();
                        }

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
                        if (unit.IsMessageDefined(unit.Area))
                        {
                            GUI.OpenMessageForm(u1: null, u2: null);
                            unit.PilotMessage(unit.Area, msg_mode: "");
                            GUI.CloseMessageForm();
                        }

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
                        if (unit.IsMessageDefined(unit.Area))
                        {
                            GUI.OpenMessageForm(u1: null, u2: null);
                            unit.PilotMessage(unit.Area, msg_mode: "");
                            GUI.CloseMessageForm();
                        }

                        GUI.PaintUnitBitmap(SelectedUnit);
                        CommandState = "ユニット選択";
                        GUI.UnlockGUI();
                        break;
                    }

                case LaunchCmdID: // 発進
                    StartLaunchCommand();
                    break;

                case ItemCmdID: // アイテム
                    StartAbilityCommand(true);
                    break;

                case DismissCmdID: // 召喚解除
                    DismissCommand();
                    break;

                case OrderCmdID: // 命令
                    StartOrderCommand();
                    break;

                case ExchangeFormCmdID: // 換装
                    ExchangeFormCommand();
                    break;

                case FeatureListCmdID: // 特殊能力一覧
                    FeatureListCommand();
                    break;

                case WeaponListCmdID: // 武器一覧
                    WeaponListCommand();
                    break;

                case AbilityListCmdID: // アビリティ一覧
                    AbilityListCommand();
                    break;

                case UnitCommandCmdID: // ユニットコマンド
                    UserDefineUnitCommand(command);
                    break;

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

        private void UserDefineUnitCommand(UiCommand command)
        {
            var unit = SelectedUnit;
            var prev_used_action = unit.UsedAction;

            GUI.LockGUI();

            // ユニットコマンドの使用イベント
            Event.HandleEvent("使用", unit.MainPilot().ID, command.Label);
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
            Event.HandleEvent("" + command.LabelData.EventDataId);
            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                CancelCommand();
                GUI.UnlockGUI();
                return;
            }

            // ユニットコマンドの使用後イベント
            if (unit.CurrentForm().CountPilot() > 0)
            {
                Event.HandleEvent("使用後", unit.CurrentForm().MainPilot().ID, command.Label);
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    GUI.UnlockGUI();
                    return;
                }
            }

            // ステータスウィンドウを更新
            if (unit.CurrentForm().CountPilot() > 0)
            {
                Status.DisplayUnitStatus(unit.CurrentForm());
            }

            // 行動終了
            if (unit.CurrentForm().UsedAction <= prev_used_action)
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
        }

        private void DismissCommand()
        {
            var unit = SelectedUnit;
            GUI.LockGUI();

            // 召喚解除の使用イベント
            Event.HandleEvent("使用", unit.MainPilot().ID, "召喚解除");
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
            unit.DismissServant();

            // 召喚解除の使用後イベント
            Event.HandleEvent("使用後", unit.CurrentForm().MainPilot().ID, "召喚解除");
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
            }

            GUI.UnlockGUI();
        }

        // 「待機」コマンド
        // 他のコマンドの終了処理にも使われる
        private void WaitCommand(bool WithoutAction = false)
        {
            LogDebug();

            // コマンド終了時はターゲットを解除
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
                    SelectedUnit.RemoveSpecialPowerInEffect("移動");
                }
            }

            CommandState = "ユニット選択";

            // アップデート
            SelectedUnit.Update();
            SRC.PList.UpdateSupportMod(SelectedUnit);

            // ユニットが既に出撃していない？
            if (SelectedUnit.Status != "出撃")
            {
                GUI.RedrawScreen();
                Status.ClearUnitStatus();
                return;
            }

            GUI.LockGUI();
            GUI.RedrawScreen();
            var p = SelectedUnit.Pilots.First();

            // 接触イベント
            foreach (var unit in Map.AdjacentUnit(SelectedUnit))
            {
                SelectedTarget = unit;
                Event.HandleEvent("接触", SelectedUnit.MainPilot().ID, SelectedTarget.MainPilot().ID);
                SelectedTarget = null;
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    return;
                }

                if (SelectedUnit.Status != "出撃")
                {
                    GUI.RedrawScreen();
                    Status.ClearUnitStatus();
                    GUI.UnlockGUI();
                    return;
                }
            }

            // 進入イベント
            Event.HandleEvent("進入", SelectedUnit.MainPilot().ID, "" + SelectedUnit.x, "" + SelectedUnit.y);
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
            Event.HandleEvent("行動終了", SelectedUnit.MainPilot().ID);
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

            if (p.Unit is object)
            {
                SelectedUnit = p.Unit;
            }

            if (SelectedUnit.Action > 0 && SelectedUnit.CountPilot() > 0)
            {
                // カーソル自動移動
                if (SRC.AutoMoveCursor)
                {
                    GUI.MoveCursorPos("ユニット選択", SelectedUnit);
                }
            }

            // ハイパーモード・ノーマルモードの自動発動をチェック
            SelectedUnit.CurrentForm().CheckAutoHyperMode();
            SelectedUnit.CurrentForm().CheckAutoNormalMode();
            if (GUI.IsPictureVisible || GUI.IsCursorVisible)
            {
                GUI.RedrawScreen();
            }

            GUI.UnlockGUI();

            //// ステータスウィンドウの表示内容を更新
            //if (SelectedUnit.Status == "出撃" && GUI.MainWidth == 15)
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
