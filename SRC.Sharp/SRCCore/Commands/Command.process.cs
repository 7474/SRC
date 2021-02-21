using SRCCore.Events;
using SRCCore.Maps;
using SRCCore.Units;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.Commands
{
    // ユニット＆マップコマンドの実行を行うモジュール
    public partial class Command
    {
        public void ProceedInput(GuiButton button, MapCell cell, Unit unit)
        {
            if (button == GuiButton.Left)
            {
                // 左クリック
                switch (CommandState ?? "")
                {
                    case "マップコマンド":
                        {
                            CommandState = "ユニット選択";
                            break;
                        }

                    case "ユニット選択":
                        if (unit != null)
                        {
                            ProceedCommand();
                        }
                        break;
                    case "ターゲット選択":
                    case "移動後ターゲット選択":
                        if (unit != null)
                        {
                            ProceedCommand();
                        }
                        break;
                    case "コマンド選択":
                        CancelCommand();
                        // もし新しいクリック地点がユニットなら、ユニット選択の処理を進める
                        if (unit != null)
                        {
                            ProceedCommand();
                        }
                        break;

                    case "移動後コマンド選択":
                        CancelCommand();
                        break;

                    default:
                        ProceedCommand();
                        break;
                }
            }

            if (button == GuiButton.Right)
            {
                GUI.ShowMapCommandMenu(new List<UiCommand>
                {
                    new UiCommand(1, "test1"),
                    new UiCommand(2, "test2"),
                });
                // 右クリック
                switch (CommandState ?? "")
                {
                    case "マップコマンド":
                        CommandState = "ユニット選択";
                        break;

                    case "ユニット選択":
                        ProceedCommand(true);
                        break;

                    default:
                        CancelCommand();
                        break;
                }
            }
        }

        // コマンドの処理を進める
        // by_cancel = True の場合はコマンドをキャンセルした場合の処理
        public void ProceedCommand(bool by_cancel = false)
        {
            int j, i, n;
            Unit u;
            string uname;
            string buf;
            LabelData lab;

            // 閲覧モードはキャンセルで終了。それ以外の入力は無視
            if (ViewMode)
            {
                if (by_cancel)
                {
                    ViewMode = false;
                }

                return;
            }

            // 処理が行われるまでこれ以降のコマンド受付を禁止
            // (スクロール禁止にしなければならないほどの時間はないため、LockGUIは使わない)
            GUI.IsGUILocked = true;

            // コマンド実行を行うということはシナリオプレイ中ということなので毎回初期化する。
            SRC.IsScenarioFinished = false;
            SRC.IsCanceled = false;

            //// ポップアップメニュー上で押したマウスボタンが左右どちらかを判定するため、
            //// あらかじめGetAsyncKeyState()を実行しておく必要がある
            //GUI.GetAsyncKeyState(GUI.RButtonID);
            //switch (CommandState ?? "")
            //{
            //    case "ユニット選択":
            //    case "マップコマンド":
            //        {
            //            // UPGRADE_NOTE: オブジェクト SelectedUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //            SelectedUnit = null;
            //            // ADD START MARGE
            //            SelectedUnitMoveCost = 0;
            //            // ADD END MARGE
            //            if (1 <= GUI.PixelToMapX(GUI.MouseX) & GUI.PixelToMapX(GUI.MouseX) <= Map.MapWidth & 1 <= GUI.PixelToMapY(GUI.MouseY) & GUI.PixelToMapY(GUI.MouseY) <= Map.MapHeight)
            //            {
            //                SelectedUnit = Map.MapDataForUnit[GUI.PixelToMapX(GUI.MouseX), GUI.PixelToMapY(GUI.MouseY)];
            //            }

            //            if (SelectedUnit is null)
            //            {
            //                SelectedX = GUI.PixelToMapX(GUI.MouseX);
            //                SelectedY = GUI.PixelToMapY(GUI.MouseY);
            //                if (!string.IsNullOrEmpty(Map.MapFileName))
            //                {
            //                    // 通常のステージ

            //                    Status.DisplayGlobalStatus();

            //                    // ターン終了
            //                    if (ViewMode)
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuMapCommandItem(EndTurnCmdID).Caption = "部隊編成に戻る";
            //                    }
            //                    else
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuMapCommandItem(EndTurnCmdID).Caption = "ターン終了";
            //                    }
            //                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    GUI.MainForm.mnuMapCommandItem(EndTurnCmdID).Visible = true;

            //                    // 中断
            //                    string argoname1 = "デバッグ";
            //                    if (Expression.IsOptionDefined(argoname1))
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuMapCommandItem(DumpCmdID).Visible = true;
            //                    }
            //                    else
            //                    {
            //                        string argoname = "クイックセーブ不可";
            //                        if (!Expression.IsOptionDefined(argoname))
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuMapCommandItem(DumpCmdID).Visible = true;
            //                        }
            //                        else
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuMapCommandItem(DumpCmdID).Visible = false;
            //                        }
            //                    }

            //                    // 全体マップ
            //                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    GUI.MainForm.mnuMapCommandItem(GlobalMapCmdID).Visible = true;

            //                    // 作戦目的
            //                    string arglname = "勝利条件";
            //                    if (Event_Renamed.IsEventDefined(arglname))
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuMapCommandItem(OperationObjectCmdID).Visible = true;
            //                    }
            //                    else
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuMapCommandItem(OperationObjectCmdID).Visible = false;
            //                    }

            //                    // 自動反撃モード
            //                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    GUI.MainForm.mnuMapCommandItem(AutoDefenseCmdID).Visible = true;

            //                    // 設定変更
            //                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    GUI.MainForm.mnuMapCommandItem(ConfigurationCmdID).Visible = true;

            //                    // リスタート
            //                    if (SRC.IsRestartSaveDataAvailable & !ViewMode)
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuMapCommandItem(RestartCmdID).Visible = true;
            //                    }
            //                    else
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuMapCommandItem(RestartCmdID).Visible = false;
            //                    }

            //                    // クイックロード
            //                    if (SRC.IsQuickSaveDataAvailable & !ViewMode)
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuMapCommandItem(QuickLoadCmdID).Visible = true;
            //                    }
            //                    else
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuMapCommandItem(QuickLoadCmdID).Visible = false;
            //                    }

            //                    // クイックセーブ
            //                    string argoname3 = "デバッグ";
            //                    if (ViewMode)
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuMapCommandItem(QuickSaveCmdID).Visible = false;
            //                    }
            //                    else if (Expression.IsOptionDefined(argoname3))
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuMapCommandItem(QuickSaveCmdID).Visible = true;
            //                    }
            //                    else
            //                    {
            //                        string argoname2 = "クイックセーブ不可";
            //                        if (!Expression.IsOptionDefined(argoname2))
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuMapCommandItem(QuickSaveCmdID).Visible = true;
            //                        }
            //                        else
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuMapCommandItem(QuickSaveCmdID).Visible = false;
            //                        }
            //                    }
            //                }
            //                else
            //                {
            //                    // パイロットステータス・ユニットステータスのステージ
            //                    {
            //                        var withBlock = GUI.MainForm;
            //                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        withBlock.mnuMapCommandItem(EndTurnCmdID).Visible = false;
            //                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        withBlock.mnuMapCommandItem(DumpCmdID).Visible = false;
            //                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        withBlock.mnuMapCommandItem(GlobalMapCmdID).Visible = false;
            //                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        withBlock.mnuMapCommandItem(OperationObjectCmdID).Visible = false;
            //                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        withBlock.mnuMapCommandItem(AutoDefenseCmdID).Visible = false;
            //                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        withBlock.mnuMapCommandItem(ConfigurationCmdID).Visible = false;
            //                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        withBlock.mnuMapCommandItem(RestartCmdID).Visible = false;
            //                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        withBlock.mnuMapCommandItem(QuickLoadCmdID).Visible = false;
            //                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        withBlock.mnuMapCommandItem(QuickSaveCmdID).Visible = false;
            //                    }
            //                }

            //                // スペシャルパワー検索
            //                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                GUI.MainForm.mnuMapCommandItem(SearchSpecialPowerCmdID).Visible = false;
            //                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                string argtname = "スペシャルパワー";
            //                Unit argu = null;
            //                GUI.MainForm.mnuMapCommandItem(SearchSpecialPowerCmdID).Caption = Expression.Term(argtname, u: argu) + "検索";
            //                foreach (Pilot p in SRC.PList)
            //                {
            //                    if (p.Party == "味方")
            //                    {
            //                        if (p.CountSpecialPower > 0)
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuMapCommandItem(SearchSpecialPowerCmdID).Visible = true;
            //                            break;
            //                        }
            //                    }
            //                }

            //                // イベントで定義されたマップコマンド
            //                {
            //                    var withBlock1 = GUI.MainForm;
            //                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock1.mnuMapCommandItem(MapCommand1CmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock1.mnuMapCommandItem(MapCommand2CmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock1.mnuMapCommandItem(MapCommand3CmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock1.mnuMapCommandItem(MapCommand4CmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock1.mnuMapCommandItem(MapCommand5CmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock1.mnuMapCommandItem(MapCommand6CmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock1.mnuMapCommandItem(MapCommand7CmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock1.mnuMapCommandItem(MapCommand8CmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock1.mnuMapCommandItem(MapCommand9CmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock1.mnuMapCommandItem(MapCommand10CmdID).Visible = false;
            //                }

            //                if (!ViewMode)
            //                {
            //                    i = MapCommand1CmdID;
            //                    foreach (LabelData currentLab in Event_Renamed.colEventLabelList)
            //                    {
            //                        lab = currentLab;
            //                        if (lab.Name == Event_Renamed.LabelType.MapCommandEventLabel)
            //                        {
            //                            if (lab.Enable)
            //                            {
            //                                int localStrToLng() { string argexpr = lab.Para(3); var ret = GeneralLib.StrToLng(argexpr); return ret; }

            //                                if (lab.CountPara() == 2)
            //                                {
            //                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.mnuMapCommandItem(i).Visible = true;
            //                                }
            //                                else if (localStrToLng() != 0)
            //                                {
            //                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.mnuMapCommandItem(i).Visible = true;
            //                                }
            //                            }

            //                            // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            if (GUI.MainForm.mnuMapCommandItem(i).Visible)
            //                            {
            //                                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuMapCommandItem(i).Caption = lab.Para(2);
            //                                MapCommandLabelList[i - MapCommand1CmdID + 1] = lab.LineNum.ToString();
            //                                i = (i + 1);
            //                                if (i > MapCommand10CmdID)
            //                                {
            //                                    break;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }

            //                CommandState = "マップコマンド";
            //                GUI.IsGUILocked = false;
            //                // ADD START 240a
            //                // ここに来た時点でcancel=Trueはユニットのいないセルを右クリックした場合のみ
            //                if (by_cancel)
            //                {
            //                    if (GUI.NewGUIMode & !string.IsNullOrEmpty(Map.MapFileName))
            //                    {
            //                        if (GUI.MouseX < GUI.MainPWidth / 2)
            //                        {
            //                            // UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.picUnitStatus.Move(GUI.MainPWidth - 240, 10);
            //                        }
            //                        else
            //                        {
            //                            // UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.picUnitStatus.Move(5, 10);
            //                        }
            //                        // UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.picUnitStatus.Visible = true;
            //                    }
            //                }
            //                // ADD  END  240a
            //                // UPGRADE_ISSUE: Control mnuMapCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                // UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
            //                GUI.MainForm.PopupMenu(GUI.MainForm.mnuMapCommand, 6, GUI.MouseX, GUI.MouseY + 6f);
            //                return;
            //            }

            //            Event_Renamed.SelectedUnitForEvent = SelectedUnit;
            //            SelectedWeapon = 0;
            //            SelectedTWeapon = 0;
            //            SelectedAbility = 0;
            //            if (by_cancel)
            //            {
            //                // ユニット上でキャンセルボタンを押した場合は武器一覧
            //                // もしくはアビリティ一覧を表示する
            //                {
            //                    var withBlock2 = SelectedUnit;
            //                    // 情報が隠蔽されている場合は表示しない
            //                    string argoname4 = "ユニット情報隠蔽";
            //                    object argIndex1 = "識別済み";
            //                    object argIndex2 = "ユニット情報隠蔽";
            //                    string argfname = "ダミーユニット";
            //                    if (Expression.IsOptionDefined(argoname4) & !withBlock2.IsConditionSatisfied(argIndex1) & (withBlock2.Party0 == "敵" | withBlock2.Party0 == "中立") | withBlock2.IsConditionSatisfied(argIndex2) | withBlock2.IsFeatureAvailable(argfname))
            //                    {
            //                        GUI.IsGUILocked = false;
            //                        return;
            //                    }

            //                    if (withBlock2.CountWeapon() == 0 & withBlock2.CountAbility() > 0)
            //                    {
            //                        AbilityListCommand();
            //                    }
            //                    else
            //                    {
            //                        WeaponListCommand();
            //                    }
            //                }

            //                GUI.IsGUILocked = false;
            //                return;
            //            }

            //            CommandState = "コマンド選択";
            //            ProceedCommand(by_cancel);
            //            break;
            //        }

            //    case "コマンド選択":
            //        {
            //            // MOD START 240aClearUnitStatus
            //            // If MainWidth <> 15 Then
            //            // DisplayUnitStatus SelectedUnit
            //            // End If
            //            if (!GUI.NewGUIMode)
            //            {
            //                Status.DisplayUnitStatus(SelectedUnit);
            //            }
            //            else
            //            {
            //                Status.ClearUnitStatus();
            //            }
            //            // MOD  END  240a

            //            // 武装一覧以外は一旦消しておく
            //            {
            //                var withBlock3 = GUI.MainForm;
            //                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                withBlock3.mnuUnitCommandItem(WeaponListCmdID).Visible = true;
            //                for (i = 0; i <= WeaponListCmdID - 1; i++)
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock3.mnuUnitCommandItem(i).Visible = false;
            //                for (i = WeaponListCmdID + 1; i <= WaitCmdID; i++)
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock3.mnuUnitCommandItem(i).Visible = false;
            //            }

            //            Event_Renamed.SelectedUnitForEvent = SelectedUnit;
            //            // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //            SelectedTarget = null;
            //            // UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //            Event_Renamed.SelectedTargetForEvent = null;
            //            {
            //                var withBlock4 = SelectedUnit;
            //                // 特殊能力＆アビリティ一覧はどのユニットでも見れる可能性があるので
            //                // 先に判定しておく

            //                // 特殊能力一覧コマンド
            //                var loopTo = withBlock4.CountAllFeature();
            //                for (i = 1; i <= loopTo; i++)
            //                {
            //                    string localAllFeature() { object argIndex1 = i; var ret = withBlock4.AllFeature(argIndex1); return ret; }

            //                    string localAllFeature1() { object argIndex1 = i; var ret = withBlock4.AllFeature(argIndex1); return ret; }

            //                    string localAllFeature2() { object argIndex1 = i; var ret = withBlock4.AllFeature(argIndex1); return ret; }

            //                    string localAllFeature3() { object argIndex1 = i; var ret = withBlock4.AllFeature(argIndex1); return ret; }

            //                    object argIndex5 = i;
            //                    if (!string.IsNullOrEmpty(withBlock4.AllFeatureName(argIndex5)))
            //                    {
            //                        object argIndex4 = i;
            //                        switch (withBlock4.AllFeature(argIndex4) ?? "")
            //                        {
            //                            case "合体":
            //                                {
            //                                    string localAllFeatureData() { object argIndex1 = i; var ret = withBlock4.AllFeatureData(argIndex1); return ret; }

            //                                    string localLIndex() { string arglist = hsde8149624c274ab08211d9ffa37bf9bf(); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                                    object argIndex3 = localLIndex();
            //                                    if (SRC.UList.IsDefined(argIndex3))
            //                                    {
            //                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                        GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = true;
            //                                        break;
            //                                    }

            //                                    break;
            //                                }

            //                            default:
            //                                {
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = true;
            //                                    break;
            //                                }
            //                        }
            //                    }
            //                    else if (localAllFeature() == "パイロット能力付加" | localAllFeature1() == "パイロット能力強化")
            //                    {
            //                        string localAllFeatureData1() { object argIndex1 = i; var ret = withBlock4.AllFeatureData(argIndex1); return ret; }

            //                        if (Strings.InStr(localAllFeatureData1(), "非表示") == 0)
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = true;
            //                            break;
            //                        }
            //                    }
            //                    else if (localAllFeature2() == "武器クラス" | localAllFeature3() == "防具クラス")
            //                    {
            //                        string argoname5 = "アイテム交換";
            //                        if (Expression.IsOptionDefined(argoname5))
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = true;
            //                            break;
            //                        }
            //                    }
            //                }

            //                {
            //                    var withBlock5 = withBlock4.MainPilot();
            //                    var loopTo1 = withBlock5.CountSkill();
            //                    for (i = 1; i <= loopTo1; i++)
            //                    {
            //                        string localSkillName0() { object argIndex1 = i; var ret = withBlock5.SkillName0(argIndex1); return ret; }

            //                        string localSkillName01() { object argIndex1 = i; var ret = withBlock5.SkillName0(argIndex1); return ret; }

            //                        if (localSkillName0() != "非表示" & !string.IsNullOrEmpty(localSkillName01()))
            //                        {
            //                            object argIndex6 = i;
            //                            switch (withBlock5.Skill(argIndex6) ?? "")
            //                            {
            //                                case "耐久":
            //                                    {
            //                                        string argoname6 = "防御力成長";
            //                                        string argoname7 = "防御力レベルアップ";
            //                                        if (!Expression.IsOptionDefined(argoname6) & !Expression.IsOptionDefined(argoname7))
            //                                        {
            //                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                            GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = true;
            //                                            break;
            //                                        }

            //                                        break;
            //                                    }

            //                                case "追加レベル":
            //                                case "格闘ＵＰ":
            //                                case "射撃ＵＰ":
            //                                case "命中ＵＰ":
            //                                case "回避ＵＰ":
            //                                case "技量ＵＰ":
            //                                case "反応ＵＰ":
            //                                case "ＳＰＵＰ":
            //                                case "格闘ＤＯＷＮ":
            //                                case "射撃ＤＯＷＮ":
            //                                case "命中ＤＯＷＮ":
            //                                case "回避ＤＯＷＮ":
            //                                case "技量ＤＯＷＮ":
            //                                case "反応ＤＯＷＮ":
            //                                case "ＳＰＤＯＷＮ":
            //                                case "メッセージ":
            //                                case "魔力所有":
            //                                    {
            //                                        break;
            //                                    }

            //                                default:
            //                                    {
            //                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                        GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = true;
            //                                        break;
            //                                    }
            //                            }
            //                        }
            //                    }
            //                }

            //                // アビリティ一覧コマンド
            //                var loopTo2 = withBlock4.CountAbility();
            //                for (i = 1; i <= loopTo2; i++)
            //                {
            //                    string argattr = "合";
            //                    if (withBlock4.IsAbilityMastered(i) & !withBlock4.IsDisabled(withBlock4.Ability(i).Name) & (!withBlock4.IsAbilityClassifiedAs(i, argattr) | withBlock4.IsCombinationAbilityAvailable(i, true)) & !withBlock4.Ability(i).IsItem())
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        string argtname1 = "アビリティ";
            //                        GUI.MainForm.mnuUnitCommandItem(AbilityListCmdID).Caption = Expression.Term(argtname1, SelectedUnit) + "一覧";
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(AbilityListCmdID).Visible = true;
            //                        break;
            //                    }
            //                }

            //                // 味方じゃない場合
            //                object argIndex36 = "非操作";
            //                if (withBlock4.Party != "味方" | withBlock4.IsConditionSatisfied(argIndex36) | ViewMode)
            //                {
            //                    // 召喚ユニットは命令コマンドを使用可能
            //                    string argfname1 = "召喚ユニット";
            //                    object argIndex7 = "魅了";
            //                    object argIndex8 = "混乱";
            //                    object argIndex9 = "恐怖";
            //                    object argIndex10 = "暴走";
            //                    object argIndex11 = "狂戦士";
            //                    if (withBlock4.Party == "ＮＰＣ" & withBlock4.IsFeatureAvailable(argfname1) & !withBlock4.IsConditionSatisfied(argIndex7) & !withBlock4.IsConditionSatisfied(argIndex8) & !withBlock4.IsConditionSatisfied(argIndex9) & !withBlock4.IsConditionSatisfied(argIndex10) & !withBlock4.IsConditionSatisfied(argIndex11) & !ViewMode)
            //                    {
            //                        if (withBlock4.Summoner is object)
            //                        {
            //                            if (withBlock4.Summoner.Party == "味方")
            //                            {
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(OrderCmdID).Caption = "命令";
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(OrderCmdID).Visible = true;
            //                            }
            //                        }
            //                    }

            //                    // 魅了したユニットに対しても命令コマンドを使用可能
            //                    object argIndex12 = "魅了";
            //                    object argIndex13 = "混乱";
            //                    object argIndex14 = "恐怖";
            //                    object argIndex15 = "暴走";
            //                    object argIndex16 = "狂戦士";
            //                    if (withBlock4.Party == "ＮＰＣ" & withBlock4.IsConditionSatisfied(argIndex12) & !withBlock4.IsConditionSatisfied(argIndex13) & !withBlock4.IsConditionSatisfied(argIndex14) & !withBlock4.IsConditionSatisfied(argIndex15) & !withBlock4.IsConditionSatisfied(argIndex16) & !ViewMode)
            //                    {
            //                        if (withBlock4.Master is object)
            //                        {
            //                            if (withBlock4.Master.Party == "味方")
            //                            {
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(OrderCmdID).Caption = "命令";
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(OrderCmdID).Visible = true;
            //                            }
            //                        }
            //                    }

            //                    // ダミーユニットの場合はコマンド一覧を表示しない
            //                    string argfname2 = "ダミーユニット";
            //                    if (withBlock4.IsFeatureAvailable(argfname2))
            //                    {
            //                        // 特殊能力一覧
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        if (GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible)
            //                        {
            //                            UnitCommand(FeatureListCmdID);
            //                        }
            //                        else
            //                        {
            //                            CommandState = "ユニット選択";
            //                        }

            //                        GUI.IsGUILocked = false;
            //                        return;
            //                    }

            //                    if (!string.IsNullOrEmpty(Map.MapFileName))
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Caption = "移動範囲";
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Visible = true;
            //                        var loopTo3 = withBlock4.CountWeapon();
            //                        for (i = 1; i <= loopTo3; i++)
            //                        {
            //                            string argref_mode = "";
            //                            string argattr1 = "Ｍ";
            //                            if (withBlock4.IsWeaponAvailable(i, argref_mode) & !withBlock4.IsWeaponClassifiedAs(i, argattr1))
            //                            {
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Caption = "射程範囲";
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = true;
            //                            }
            //                        }
            //                    }

            //                    // ユニットステータスコマンド用
            //                    if (string.IsNullOrEmpty(Map.MapFileName))
            //                    {
            //                        // 変形コマンド
            //                        string argfname3 = "変形";
            //                        if (withBlock4.IsFeatureAvailable(argfname3))
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            object argIndex17 = "変形";
            //                            GUI.MainForm.mnuUnitCommandItem(TransformCmdID).Caption = withBlock4.FeatureName(argIndex17);
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            if (GUI.MainForm.mnuUnitCommandItem(TransformCmdID).Caption == "")
            //                            {
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(TransformCmdID).Caption = "変形";
            //                            }

            //                            object argIndex19 = "変形";
            //                            string arglist1 = withBlock4.FeatureData(argIndex19);
            //                            var loopTo4 = GeneralLib.LLength(arglist1);
            //                            for (i = 2; i <= loopTo4; i++)
            //                            {
            //                                object argIndex18 = "変形";
            //                                string arglist = withBlock4.FeatureData(argIndex18);
            //                                uname = GeneralLib.LIndex(arglist, i);
            //                                Unit localOtherForm() { object argIndex1 = uname; var ret = withBlock4.OtherForm(argIndex1); return ret; }

            //                                if (localOtherForm().IsAvailable())
            //                                {
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.mnuUnitCommandItem(TransformCmdID).Visible = true;
            //                                    break;
            //                                }
            //                            }
            //                        }

            //                        // 分離コマンド
            //                        string argfname5 = "分離";
            //                        if (withBlock4.IsFeatureAvailable(argfname5))
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible = true;
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            object argIndex20 = "分離";
            //                            GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption = withBlock4.FeatureName(argIndex20);
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            if (GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption == "")
            //                            {
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption = "分離";
            //                            }

            //                            object argIndex21 = "分離";
            //                            buf = withBlock4.FeatureData(argIndex21);

            //                            // 分離形態が利用出来ない場合は分離を行わない
            //                            var loopTo5 = GeneralLib.LLength(buf);
            //                            for (i = 2; i <= loopTo5; i++)
            //                            {
            //                                bool localIsDefined() { object argIndex1 = GeneralLib.LIndex(buf, i); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //                                if (!localIsDefined())
            //                                {
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible = false;
            //                                    break;
            //                                }
            //                            }

            //                            // パイロットが足らない場合も分離を行わない
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            if (GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible)
            //                            {
            //                                n = 0;
            //                                var loopTo6 = GeneralLib.LLength(buf);
            //                                for (i = 2; i <= loopTo6; i++)
            //                                {
            //                                    Unit localItem() { object argIndex1 = GeneralLib.LIndex(buf, i); var ret = SRC.UList.Item(argIndex1); return ret; }

            //                                    {
            //                                        var withBlock6 = localItem().Data;
            //                                        string argfname4 = "召喚ユニット";
            //                                        if (!withBlock6.IsFeatureAvailable(argfname4))
            //                                        {
            //                                            n = (n + Math.Abs(withBlock6.PilotNum));
            //                                        }
            //                                    }
            //                                }

            //                                if (withBlock4.CountPilot() < n)
            //                                {
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible = false;
            //                                }
            //                            }
            //                        }

            //                        string argfname6 = "パーツ分離";
            //                        if (withBlock4.IsFeatureAvailable(argfname6))
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            object argIndex22 = "パーツ分離";
            //                            GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption = withBlock4.FeatureName(argIndex22);
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            if (GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption == "")
            //                            {
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption = "パーツ分離";
            //                            }
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible = true;
            //                        }

            //                        // 合体コマンド
            //                        string argfname8 = "合体";
            //                        string argfname9 = "パーツ合体";
            //                        if (withBlock4.IsFeatureAvailable(argfname8))
            //                        {
            //                            var loopTo7 = withBlock4.CountFeature();
            //                            for (i = 1; i <= loopTo7; i++)
            //                            {
            //                                object argIndex24 = i;
            //                                if (withBlock4.Feature(argIndex24) == "合体")
            //                                {
            //                                    n = 0;
            //                                    // パートナーが存在しているか？
            //                                    string localFeatureData1() { object argIndex1 = i; var ret = withBlock4.FeatureData(argIndex1); return ret; }

            //                                    string arglist2 = localFeatureData1();
            //                                    var loopTo8 = GeneralLib.LLength(arglist2);
            //                                    for (j = 3; j <= loopTo8; j++)
            //                                    {
            //                                        string localFeatureData() { object argIndex1 = i; var ret = withBlock4.FeatureData(argIndex1); return ret; }

            //                                        string localLIndex1() { string arglist = hse3098790f77a4c3c8e351b0c8f045435(); var ret = GeneralLib.LIndex(arglist, j); return ret; }

            //                                        object argIndex23 = localLIndex1();
            //                                        u = SRC.UList.Item(argIndex23);
            //                                        if (u is null)
            //                                        {
            //                                            break;
            //                                        }

            //                                        string argfname7 = "合体制限";
            //                                        if (u.Status_Renamed != "出撃" & u.CurrentForm().IsFeatureAvailable(argfname7))
            //                                        {
            //                                            break;
            //                                        }

            //                                        n = (n + 1);
            //                                    }

            //                                    // 合体先のユニットが作成されているか？
            //                                    string localFeatureData2() { object argIndex1 = i; var ret = withBlock4.FeatureData(argIndex1); return ret; }

            //                                    string localLIndex2() { string arglist = hse489dc5578704b21a62d1221f27f2c9c(); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                                    bool localIsDefined1() { object argIndex1 = (object)hs2edf74710015446592f60b6fcb7267d6(); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //                                    if (!localIsDefined1())
            //                                    {
            //                                        n = 0;
            //                                    }

            //                                    // すべての条件を満たしている場合
            //                                    string localFeatureData4() { object argIndex1 = i; var ret = withBlock4.FeatureData(argIndex1); return ret; }

            //                                    int localLLength() { string arglist = hs57a7b11782d04866bf1e5d24ed51c504(); var ret = GeneralLib.LLength(arglist); return ret; }

            //                                    if (n == localLLength() - 2)
            //                                    {
            //                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                        GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Visible = true;
            //                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                        string localFeatureData3() { object argIndex1 = i; var ret = withBlock4.FeatureData(argIndex1); return ret; }

            //                                        string arglist3 = localFeatureData3();
            //                                        GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Caption = GeneralLib.LIndex(arglist3, 1);
            //                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                        if (GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Caption == "非表示")
            //                                        {
            //                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                            GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Caption = "合体";
            //                                        }

            //                                        break;
            //                                    }
            //                                }
            //                            }
            //                        }
            //                        else if (withBlock4.IsFeatureAvailable(argfname9))
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Caption = "パーツ合体";
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Visible = true;
            //                        }

            //                        object argIndex30 = "ノーマルモード付加";
            //                        if (!withBlock4.IsConditionSatisfied(argIndex30))
            //                        {
            //                            // ハイパーモードコマンド
            //                            string argfname10 = "ハイパーモード";
            //                            string argfname11 = "ノーマルモード";
            //                            if (withBlock4.IsFeatureAvailable(argfname10))
            //                            {
            //                                object argIndex25 = "ハイパーモード";
            //                                string arglist4 = withBlock4.FeatureData(argIndex25);
            //                                uname = GeneralLib.LIndex(arglist4, 2);
            //                                Unit localOtherForm1() { object argIndex1 = uname; var ret = withBlock4.OtherForm(argIndex1); return ret; }

            //                                if (localOtherForm1().IsAvailable())
            //                                {
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = true;
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    object argIndex26 = "ハイパーモード";
            //                                    string arglist5 = withBlock4.FeatureData(argIndex26);
            //                                    GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = GeneralLib.LIndex(arglist5, 1);
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    if (GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption == "非表示")
            //                                    {
            //                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                        GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "ハイパーモード";
            //                                    }
            //                                }
            //                            }
            //                            else if (withBlock4.IsFeatureAvailable(argfname11))
            //                            {
            //                                object argIndex27 = "ノーマルモード";
            //                                string arglist6 = withBlock4.FeatureData(argIndex27);
            //                                uname = GeneralLib.LIndex(arglist6, 1);
            //                                Unit localOtherForm2() { object argIndex1 = uname; var ret = withBlock4.OtherForm(argIndex1); return ret; }

            //                                if (localOtherForm2().IsAvailable())
            //                                {
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = true;
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "ノーマルモード";
            //                                    string localLIndex3() { object argIndex1 = "変形"; string arglist = withBlock4.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                                    if ((uname ?? "") == (localLIndex3() ?? ""))
            //                                    {
            //                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                        GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = false;
            //                                    }
            //                                }
            //                            }
            //                        }
            //                        else
            //                        {
            //                            // 変身解除
            //                            object argIndex29 = "ノーマルモード";
            //                            if (Strings.InStr(withBlock4.FeatureData(argIndex29), "手動解除") > 0)
            //                            {
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = true;
            //                                string argfname12 = "変身解除コマンド名";
            //                                if (withBlock4.IsFeatureAvailable(argfname12))
            //                                {
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    object argIndex28 = "変身解除コマンド名";
            //                                    GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = withBlock4.FeatureData(argIndex28);
            //                                }
            //                                else if (withBlock4.IsHero())
            //                                {
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "変身解除";
            //                                }
            //                                else
            //                                {
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "特殊モード解除";
            //                                }
            //                            }
            //                        }

            //                        // 換装コマンド
            //                        string argfname13 = "換装";
            //                        if (withBlock4.IsFeatureAvailable(argfname13))
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(OrderCmdID).Caption = "換装";
            //                            object argIndex32 = "換装";
            //                            string arglist8 = withBlock4.FeatureData(argIndex32);
            //                            var loopTo9 = GeneralLib.LLength(arglist8);
            //                            for (i = 1; i <= loopTo9; i++)
            //                            {
            //                                object argIndex31 = "換装";
            //                                string arglist7 = withBlock4.FeatureData(argIndex31);
            //                                uname = GeneralLib.LIndex(arglist7, i);
            //                                Unit localOtherForm3() { object argIndex1 = uname; var ret = withBlock4.OtherForm(argIndex1); return ret; }

            //                                if (localOtherForm3().IsAvailable())
            //                                {
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.mnuUnitCommandItem(OrderCmdID).Visible = true;
            //                                    break;
            //                                }
            //                            }

            //                            // エリアスで換装の名称が変更されている？
            //                            {
            //                                var withBlock7 = SRC.ALDList;
            //                                var loopTo10 = withBlock7.Count();
            //                                for (i = 1; i <= loopTo10; i++)
            //                                {
            //                                    object argIndex33 = i;
            //                                    {
            //                                        var withBlock8 = withBlock7.Item(argIndex33);
            //                                        if (withBlock8.get_AliasType(1) == "換装")
            //                                        {
            //                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                            GUI.MainForm.mnuUnitCommandItem(OrderCmdID).Caption = withBlock8.Name;
            //                                            break;
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }

            //                    // ユニットコマンド
            //                    if (!ViewMode)
            //                    {
            //                        i = UnitCommand1CmdID;
            //                        foreach (LabelData currentLab1 in Event_Renamed.colEventLabelList)
            //                        {
            //                            lab = currentLab1;
            //                            if (lab.Name == Event_Renamed.LabelType.UnitCommandEventLabel & lab.Enable)
            //                            {
            //                                string argexpr = lab.Para(3);
            //                                buf = Expression.GetValueAsString(argexpr);
            //                                if (SelectedUnit.Party == "味方" & ((buf ?? "") == (SelectedUnit.MainPilot().Name ?? "") | (buf ?? "") == (SelectedUnit.MainPilot().get_Nickname(false) ?? "") | (buf ?? "") == (SelectedUnit.Name ?? "")) | (buf ?? "") == (SelectedUnit.Party ?? "") | buf == "全")
            //                                {
            //                                    int localGetValueAsLong() { string argexpr = lab.Para(4); var ret = Expression.GetValueAsLong(argexpr); return ret; }

            //                                    if (lab.CountPara() <= 3)
            //                                    {
            //                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                        GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
            //                                    }
            //                                    else if (localGetValueAsLong() != 0)
            //                                    {
            //                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                        GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
            //                                    }
            //                                }

            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                if (GUI.MainForm.mnuUnitCommandItem(i).Visible)
            //                                {
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.mnuUnitCommandItem(i).Caption = lab.Para(2);
            //                                    UnitCommandLabelList[i - UnitCommand1CmdID + 1] = lab.LineNum.ToString();
            //                                    i = (i + 1);
            //                                    if (i > UnitCommand10CmdID)
            //                                    {
            //                                        break;
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }

            //                    // 未確認ユニットの場合は情報を隠蔽
            //                    string argoname8 = "ユニット情報隠蔽";
            //                    object argIndex34 = "識別済み";
            //                    object argIndex35 = "ユニット情報隠蔽";
            //                    if (Expression.IsOptionDefined(argoname8) & !withBlock4.IsConditionSatisfied(argIndex34) & (withBlock4.Party0 == "敵" | withBlock4.Party0 == "中立") | withBlock4.IsConditionSatisfied(argIndex35))
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Visible = true;
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = false;
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = false;
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(WeaponListCmdID).Visible = false;
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(AbilityListCmdID).Visible = false;
            //                        for (i = 1; i <= WaitCmdID; i++)
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            if (GUI.MainForm.mnuUnitCommandItem(i).Visible)
            //                            {
            //                                break;
            //                            }
            //                        }

            //                        if (i > WaitCmdID)
            //                        {
            //                            // 表示可能なコマンドがなかった
            //                            CommandState = "ユニット選択";
            //                            GUI.IsGUILocked = false;
            //                            return;
            //                        }
            //                        // メニューコマンドを全て殺してしまうとエラーになるのでここで非表示
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Visible = false;
            //                    }

            //                    GUI.IsGUILocked = false;
            //                    if (by_cancel)
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        // UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
            //                        GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY + 5f);
            //                    }
            //                    else
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        // UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
            //                        GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY - 6f);
            //                    }

            //                    return;
            //                }

            //                // 行動終了している場合
            //                else if (withBlock4.Action == 0)
            //                {
            //                    // 発進コマンドは使用可能
            //                    string argfname14 = "母艦";
            //                    if (withBlock4.IsFeatureAvailable(argfname14))
            //                    {
            //                        if (withBlock4.Area != "地中")
            //                        {
            //                            if (withBlock4.CountUnitOnBoard() > 0)
            //                            {
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(LaunchCmdID).Visible = true;
            //                            }
            //                        }
            //                    }

            //                    // ユニットコマンド
            //                    i = UnitCommand1CmdID;
            //                    foreach (LabelData currentLab2 in Event_Renamed.colEventLabelList)
            //                    {
            //                        lab = currentLab2;
            //                        if (lab.Name == Event_Renamed.LabelType.UnitCommandEventLabel & (lab.AsterNum == 1 | lab.AsterNum == 3))
            //                        {
            //                            if (lab.Enable)
            //                            {
            //                                buf = lab.Para(3);
            //                                if (SelectedUnit.Party == "味方" & ((buf ?? "") == (SelectedUnit.MainPilot().Name ?? "") | (buf ?? "") == (SelectedUnit.MainPilot().get_Nickname(false) ?? "") | (buf ?? "") == (SelectedUnit.Name ?? "")) | (buf ?? "") == (SelectedUnit.Party ?? "") | buf == "全")
            //                                {
            //                                    int localStrToLng1() { string argexpr = lab.Para(4); var ret = GeneralLib.StrToLng(argexpr); return ret; }

            //                                    if (lab.CountPara() <= 3)
            //                                    {
            //                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                        GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
            //                                    }
            //                                    else if (localStrToLng1() != 0)
            //                                    {
            //                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                        GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
            //                                    }
            //                                }
            //                            }

            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            if (GUI.MainForm.mnuUnitCommandItem(i).Visible)
            //                            {
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(i).Caption = lab.Para(2);
            //                                UnitCommandLabelList[i - UnitCommand1CmdID + 1] = lab.LineNum.ToString();
            //                                i = (i + 1);
            //                                if (i > UnitCommand10CmdID)
            //                                {
            //                                    break;
            //                                }
            //                            }
            //                        }
            //                    }

            //                    GUI.IsGUILocked = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    // UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
            //                    GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY - 5f);
            //                    return;
            //                }

            //                // 移動コマンド
            //                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Caption = "移動";
            //                if (withBlock4.Speed <= 0)
            //                {
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    GUI.MainForm.mnuUnitCommandItem(WaitCmdID).Visible = true; // 待機
            //                }
            //                else
            //                {
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Visible = true;
            //                } // 移動

            //                // テレポートコマンド
            //                string argfname15 = "テレポート";
            //                if (withBlock4.IsFeatureAvailable(argfname15))
            //                {
            //                    object argIndex38 = "テレポート";
            //                    if (Strings.Len(withBlock4.FeatureData(argIndex38)) > 0)
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        object argIndex37 = "テレポート";
            //                        string arglist9 = withBlock4.FeatureData(argIndex37);
            //                        GUI.MainForm.mnuUnitCommandItem(TeleportCmdID).Caption = GeneralLib.LIndex(arglist9, 1);
            //                    }
            //                    else
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(TeleportCmdID).Caption = "テレポート";
            //                    }

            //                    object argIndex40 = "テレポート";
            //                    string arglist10 = withBlock4.FeatureData(argIndex40);
            //                    if (GeneralLib.LLength(arglist10) == 2)
            //                    {
            //                        string localLIndex4() { object argIndex1 = "テレポート"; string arglist = withBlock4.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                        string localLIndex5() { object argIndex1 = "テレポート"; string arglist = withBlock4.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                        if (withBlock4.EN >= Conversions.Toint(localLIndex5()))
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(TeleportCmdID).Visible = true;
            //                        }
            //                        // 通常移動がテレポートの場合
            //                        string localLIndex6() { object argIndex1 = "テレポート"; string arglist = withBlock4.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                        object argIndex39 = "テレポート";
            //                        if (withBlock4.Speed0 == 0 | withBlock4.FeatureLevel(argIndex39) >= 0d & localLIndex6() == "0")
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Visible = false;
            //                        }
            //                    }
            //                    else if (withBlock4.EN >= 40)
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(TeleportCmdID).Visible = true;
            //                    }

            //                    object argIndex41 = "移動不能";
            //                    if (withBlock4.IsConditionSatisfied(argIndex41))
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(TeleportCmdID).Visible = false;
            //                    }
            //                }

            //                // ジャンプコマンド
            //                string argfname16 = "ジャンプ";
            //                if (withBlock4.IsFeatureAvailable(argfname16) & withBlock4.Area != "空中" & withBlock4.Area != "宇宙")
            //                {
            //                    object argIndex43 = "ジャンプ";
            //                    if (Strings.Len(withBlock4.FeatureData(argIndex43)) > 0)
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        object argIndex42 = "ジャンプ";
            //                        string arglist11 = withBlock4.FeatureData(argIndex42);
            //                        GUI.MainForm.mnuUnitCommandItem(JumpCmdID).Caption = GeneralLib.LIndex(arglist11, 1);
            //                    }
            //                    else
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(JumpCmdID).Caption = "ジャンプ";
            //                    }

            //                    object argIndex46 = "ジャンプ";
            //                    string arglist12 = withBlock4.FeatureData(argIndex46);
            //                    if (GeneralLib.LLength(arglist12) == 2)
            //                    {
            //                        string localLIndex7() { object argIndex1 = "ジャンプ"; string arglist = withBlock4.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                        string localLIndex8() { object argIndex1 = "ジャンプ"; string arglist = withBlock4.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                        if (withBlock4.EN >= Conversions.Toint(localLIndex8()))
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(JumpCmdID).Visible = true;
            //                        }
            //                        // 通常移動がジャンプの場合
            //                        string localLIndex9() { object argIndex1 = "ジャンプ"; string arglist = withBlock4.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                        object argIndex44 = "ジャンプ";
            //                        if (withBlock4.Speed0 == 0 | withBlock4.FeatureLevel(argIndex44) >= 0d & localLIndex9() == "0")
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Visible = false;
            //                        }
            //                    }
            //                    else
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(JumpCmdID).Visible = true;
            //                        object argIndex45 = "ジャンプ";
            //                        if (withBlock4.FeatureLevel(argIndex45) >= 0d)
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Visible = false;
            //                        }
            //                    }

            //                    object argIndex47 = "移動不能";
            //                    if (withBlock4.IsConditionSatisfied(argIndex47))
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(JumpCmdID).Visible = false;
            //                    }
            //                }

            //                // 会話コマンド
            //                for (i = 1; i <= 4; i++)
            //                {
            //                    // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                    u = null;
            //                    switch (i)
            //                    {
            //                        case 1:
            //                            {
            //                                if (withBlock4.x > 1)
            //                                {
            //                                    u = Map.MapDataForUnit[withBlock4.x - 1, withBlock4.y];
            //                                }

            //                                break;
            //                            }

            //                        case 2:
            //                            {
            //                                if (withBlock4.x < Map.MapWidth)
            //                                {
            //                                    u = Map.MapDataForUnit[withBlock4.x + 1, withBlock4.y];
            //                                }

            //                                break;
            //                            }

            //                        case 3:
            //                            {
            //                                if (withBlock4.y > 1)
            //                                {
            //                                    u = Map.MapDataForUnit[withBlock4.x, withBlock4.y - 1];
            //                                }

            //                                break;
            //                            }

            //                        case 4:
            //                            {
            //                                if (withBlock4.y < Map.MapHeight)
            //                                {
            //                                    u = Map.MapDataForUnit[withBlock4.x, withBlock4.y + 1];
            //                                }

            //                                break;
            //                            }
            //                    }

            //                    if (u is object)
            //                    {
            //                        string arglname1 = "会話 " + withBlock4.MainPilot().ID + " " + u.MainPilot().ID;
            //                        if (Event_Renamed.IsEventDefined(arglname1))
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(TalkCmdID).Visible = true;
            //                            break;
            //                        }
            //                    }
            //                }

            //                // 攻撃コマンド
            //                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Caption = "攻撃";
            //                var loopTo11 = withBlock4.CountWeapon();
            //                for (i = 1; i <= loopTo11; i++)
            //                {
            //                    string argref_mode1 = "移動前";
            //                    if (withBlock4.IsWeaponUseful(i, argref_mode1))
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = true;
            //                        break;
            //                    }
            //                }

            //                if (withBlock4.Area == "地中")
            //                {
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = false;
            //                }

            //                object argIndex48 = "攻撃不能";
            //                if (withBlock4.IsConditionSatisfied(argIndex48))
            //                {
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = false;
            //                }

            //                // 修理コマンド
            //                string argfname17 = "修理装置";
            //                if (withBlock4.IsFeatureAvailable(argfname17) & withBlock4.Area != "地中")
            //                {
            //                    for (i = 1; i <= 4; i++)
            //                    {
            //                        // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                        u = null;
            //                        switch (i)
            //                        {
            //                            case 1:
            //                                {
            //                                    if (withBlock4.x > 1)
            //                                    {
            //                                        u = Map.MapDataForUnit[withBlock4.x - 1, withBlock4.y];
            //                                    }

            //                                    break;
            //                                }

            //                            case 2:
            //                                {
            //                                    if (withBlock4.x < Map.MapWidth)
            //                                    {
            //                                        u = Map.MapDataForUnit[withBlock4.x + 1, withBlock4.y];
            //                                    }

            //                                    break;
            //                                }

            //                            case 3:
            //                                {
            //                                    if (withBlock4.y > 1)
            //                                    {
            //                                        u = Map.MapDataForUnit[withBlock4.x, withBlock4.y - 1];
            //                                    }

            //                                    break;
            //                                }

            //                            case 4:
            //                                {
            //                                    if (withBlock4.y < Map.MapHeight)
            //                                    {
            //                                        u = Map.MapDataForUnit[withBlock4.x, withBlock4.y + 1];
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (u is object)
            //                        {
            //                            {
            //                                var withBlock9 = u;
            //                                object argIndex49 = "ゾンビ";
            //                                if ((withBlock9.Party == "味方" | withBlock9.Party == "ＮＰＣ") & withBlock9.HP < withBlock9.MaxHP & !withBlock9.IsConditionSatisfied(argIndex49))
            //                                {
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.mnuUnitCommandItem(FixCmdID).Visible = true;
            //                                    break;
            //                                }
            //                            }
            //                        }
            //                    }

            //                    object argIndex51 = "修理装置";
            //                    if (Strings.Len(withBlock4.FeatureData(argIndex51)) > 0)
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        object argIndex50 = "修理装置";
            //                        string arglist13 = withBlock4.FeatureData(argIndex50);
            //                        GUI.MainForm.mnuUnitCommandItem(FixCmdID).Caption = GeneralLib.LIndex(arglist13, 1);
            //                        string localLIndex12() { object argIndex1 = "修理装置"; string arglist = withBlock4.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                        if (Information.IsNumeric(localLIndex12()))
            //                        {
            //                            string localLIndex10() { object argIndex1 = "修理装置"; string arglist = withBlock4.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                            string localLIndex11() { object argIndex1 = "修理装置"; string arglist = withBlock4.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                            if (withBlock4.EN < Conversions.Toint(localLIndex11()))
            //                            {
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(FixCmdID).Visible = false;
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(FixCmdID).Caption = "修理装置";
            //                    }
            //                }

            //                // 補給コマンド
            //                string argfname18 = "補給装置";
            //                if (withBlock4.IsFeatureAvailable(argfname18) & withBlock4.Area != "地中")
            //                {
            //                    for (i = 1; i <= 4; i++)
            //                    {
            //                        // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                        u = null;
            //                        switch (i)
            //                        {
            //                            case 1:
            //                                {
            //                                    if (withBlock4.x > 1)
            //                                    {
            //                                        u = Map.MapDataForUnit[withBlock4.x - 1, withBlock4.y];
            //                                    }

            //                                    break;
            //                                }

            //                            case 2:
            //                                {
            //                                    if (withBlock4.x < Map.MapWidth)
            //                                    {
            //                                        u = Map.MapDataForUnit[withBlock4.x + 1, withBlock4.y];
            //                                    }

            //                                    break;
            //                                }

            //                            case 3:
            //                                {
            //                                    if (withBlock4.y > 1)
            //                                    {
            //                                        u = Map.MapDataForUnit[withBlock4.x, withBlock4.y - 1];
            //                                    }

            //                                    break;
            //                                }

            //                            case 4:
            //                                {
            //                                    if (withBlock4.y < Map.MapHeight)
            //                                    {
            //                                        u = Map.MapDataForUnit[withBlock4.x, withBlock4.y + 1];
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (u is object)
            //                        {
            //                            {
            //                                var withBlock10 = u;
            //                                if (withBlock10.Party == "味方" | withBlock10.Party == "ＮＰＣ")
            //                                {
            //                                    object argIndex52 = "ゾンビ";
            //                                    if (withBlock10.EN < withBlock10.MaxEN & !withBlock10.IsConditionSatisfied(argIndex52))
            //                                    {
            //                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                        GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = true;
            //                                    }
            //                                    else
            //                                    {
            //                                        var loopTo12 = withBlock10.CountWeapon();
            //                                        for (j = 1; j <= loopTo12; j++)
            //                                        {
            //                                            if (withBlock10.Bullet(j) < withBlock10.MaxBullet(j))
            //                                            {
            //                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                                GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = true;
            //                                                break;
            //                                            }
            //                                        }

            //                                        var loopTo13 = withBlock10.CountAbility();
            //                                        for (j = 1; j <= loopTo13; j++)
            //                                        {
            //                                            if (withBlock10.Stock(j) < withBlock10.MaxStock(j))
            //                                            {
            //                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                                GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = true;
            //                                                break;
            //                                            }
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }

            //                    object argIndex54 = "補給装置";
            //                    if (Strings.Len(withBlock4.FeatureData(argIndex54)) > 0)
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        object argIndex53 = "補給装置";
            //                        string arglist14 = withBlock4.FeatureData(argIndex53);
            //                        GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = GeneralLib.LIndex(arglist14, 1);
            //                        string localLIndex15() { object argIndex1 = "補給装置"; string arglist = withBlock4.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                        if (Information.IsNumeric(localLIndex15()))
            //                        {
            //                            string localLIndex13() { object argIndex1 = "補給装置"; string arglist = withBlock4.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                            string localLIndex14() { object argIndex1 = "補給装置"; string arglist = withBlock4.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                            if (withBlock4.EN < Conversions.Toint(localLIndex14()) | withBlock4.MainPilot().Morale < 100)
            //                            {
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = false;
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = "補給装置";
            //                    }
            //                }

            //                // アビリティコマンド
            //                n = 0;
            //                var loopTo14 = withBlock4.CountAbility();
            //                for (i = 1; i <= loopTo14; i++)
            //                {
            //                    if (!withBlock4.Ability(i).IsItem() & withBlock4.IsAbilityMastered(i))
            //                    {
            //                        n = (n + 1);
            //                        string argref_mode2 = "移動前";
            //                        if (withBlock4.IsAbilityUseful(i, argref_mode2))
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = true;
            //                        }
            //                    }
            //                }

            //                if (withBlock4.Area == "地中")
            //                {
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = false;
            //                }
            //                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                string argtname2 = "アビリティ";
            //                GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = Expression.Term(argtname2, SelectedUnit);
            //                if (n == 1)
            //                {
            //                    var loopTo15 = withBlock4.CountAbility();
            //                    for (i = 1; i <= loopTo15; i++)
            //                    {
            //                        if (!withBlock4.Ability(i).IsItem() & withBlock4.IsAbilityMastered(i))
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = withBlock4.AbilityNickname(i);
            //                            break;
            //                        }
            //                    }
            //                }

            //                // チャージコマンド
            //                object argIndex55 = "チャージ完了";
            //                if (!withBlock4.IsConditionSatisfied(argIndex55))
            //                {
            //                    var loopTo16 = withBlock4.CountWeapon();
            //                    for (i = 1; i <= loopTo16; i++)
            //                    {
            //                        string argattr2 = "Ｃ";
            //                        if (withBlock4.IsWeaponClassifiedAs(i, argattr2))
            //                        {
            //                            string argref_mode3 = "チャージ";
            //                            if (withBlock4.IsWeaponAvailable(i, argref_mode3))
            //                            {
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(ChargeCmdID).Visible = true;
            //                                break;
            //                            }
            //                        }
            //                    }

            //                    var loopTo17 = withBlock4.CountAbility();
            //                    for (i = 1; i <= loopTo17; i++)
            //                    {
            //                        string argattr3 = "Ｃ";
            //                        if (withBlock4.IsAbilityClassifiedAs(i, argattr3))
            //                        {
            //                            string argref_mode4 = "チャージ";
            //                            if (withBlock4.IsAbilityAvailable(i, argref_mode4))
            //                            {
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(ChargeCmdID).Visible = true;
            //                                break;
            //                            }
            //                        }
            //                    }
            //                }

            //                // スペシャルパワーコマンド
            //                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                string argtname3 = "スペシャルパワー";
            //                GUI.MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Caption = Expression.Term(argtname3, SelectedUnit);
            //                if (withBlock4.MainPilot().CountSpecialPower > 0)
            //                {
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    GUI.MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = true;
            //                }
            //                else
            //                {
            //                    var loopTo18 = withBlock4.CountPilot();
            //                    for (i = 1; i <= loopTo18; i++)
            //                    {
            //                        Pilot localPilot() { object argIndex1 = i; var ret = withBlock4.Pilot(argIndex1); return ret; }

            //                        if (localPilot().CountSpecialPower > 0)
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = true;
            //                            break;
            //                        }
            //                    }

            //                    var loopTo19 = withBlock4.CountSupport();
            //                    for (i = 1; i <= loopTo19; i++)
            //                    {
            //                        Pilot localSupport() { object argIndex1 = i; var ret = withBlock4.Support(argIndex1); return ret; }

            //                        if (localSupport().CountSpecialPower > 0)
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = true;
            //                            break;
            //                        }
            //                    }

            //                    string argfname19 = "追加サポート";
            //                    if (withBlock4.IsFeatureAvailable(argfname19))
            //                    {
            //                        if (withBlock4.AdditionalSupport().CountSpecialPower > 0)
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = true;
            //                        }
            //                    }
            //                }

            //                object argIndex56 = "憑依";
            //                object argIndex57 = "スペシャルパワー使用不能";
            //                if (withBlock4.IsConditionSatisfied(argIndex56) | withBlock4.IsConditionSatisfied(argIndex57))
            //                {
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    GUI.MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = false;
            //                }

            //                // 変形コマンド
            //                string argfname20 = "変形";
            //                object argIndex61 = "変形";
            //                object argIndex62 = "形態固定";
            //                object argIndex63 = "機体固定";
            //                if (withBlock4.IsFeatureAvailable(argfname20) & !string.IsNullOrEmpty(withBlock4.FeatureName(argIndex61)) & !withBlock4.IsConditionSatisfied(argIndex62) & !withBlock4.IsConditionSatisfied(argIndex63))
            //                {
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    object argIndex58 = "変形";
            //                    GUI.MainForm.mnuUnitCommandItem(TransformCmdID).Caption = withBlock4.FeatureName(argIndex58);
            //                    object argIndex60 = "変形";
            //                    string arglist16 = withBlock4.FeatureData(argIndex60);
            //                    var loopTo20 = GeneralLib.LLength(arglist16);
            //                    for (i = 2; i <= loopTo20; i++)
            //                    {
            //                        object argIndex59 = "変形";
            //                        string arglist15 = withBlock4.FeatureData(argIndex59);
            //                        uname = GeneralLib.LIndex(arglist15, i);
            //                        Unit localOtherForm4() { object argIndex1 = uname; var ret = withBlock4.OtherForm(argIndex1); return ret; }

            //                        if (localOtherForm4().IsAvailable())
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(TransformCmdID).Visible = true;
            //                            break;
            //                        }
            //                    }
            //                }

            //                // 分離コマンド
            //                string argfname22 = "分離";
            //                object argIndex66 = "分離";
            //                object argIndex67 = "形態固定";
            //                object argIndex68 = "機体固定";
            //                if (withBlock4.IsFeatureAvailable(argfname22) & !string.IsNullOrEmpty(withBlock4.FeatureName(argIndex66)) & !withBlock4.IsConditionSatisfied(argIndex67) & !withBlock4.IsConditionSatisfied(argIndex68))
            //                {
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible = true;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    object argIndex64 = "分離";
            //                    GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption = withBlock4.FeatureName(argIndex64);
            //                    object argIndex65 = "分離";
            //                    buf = withBlock4.FeatureData(argIndex65);

            //                    // 分離形態が利用出来ない場合は分離を行わない
            //                    var loopTo21 = GeneralLib.LLength(buf);
            //                    for (i = 2; i <= loopTo21; i++)
            //                    {
            //                        bool localIsDefined2() { object argIndex1 = GeneralLib.LIndex(buf, i); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //                        if (!localIsDefined2())
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible = false;
            //                            break;
            //                        }
            //                    }

            //                    // パイロットが足らない場合も分離を行わない
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    if (GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible)
            //                    {
            //                        n = 0;
            //                        var loopTo22 = GeneralLib.LLength(buf);
            //                        for (i = 2; i <= loopTo22; i++)
            //                        {
            //                            Unit localItem1() { object argIndex1 = GeneralLib.LIndex(buf, i); var ret = SRC.UList.Item(argIndex1); return ret; }

            //                            {
            //                                var withBlock11 = localItem1().Data;
            //                                string argfname21 = "召喚ユニット";
            //                                if (!withBlock11.IsFeatureAvailable(argfname21))
            //                                {
            //                                    n = (n + Math.Abs(withBlock11.PilotNum));
            //                                }
            //                            }
            //                        }

            //                        if (withBlock4.CountPilot() < n)
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible = false;
            //                        }
            //                    }
            //                }

            //                string argfname23 = "パーツ分離";
            //                object argIndex70 = "パーツ分離";
            //                if (withBlock4.IsFeatureAvailable(argfname23) & !string.IsNullOrEmpty(withBlock4.FeatureName(argIndex70)))
            //                {
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    object argIndex69 = "パーツ分離";
            //                    GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption = withBlock4.FeatureName(argIndex69);
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible = true;
            //                }

            //                // 合体コマンド
            //                string argfname25 = "合体";
            //                object argIndex74 = "形態固定";
            //                object argIndex75 = "機体固定";
            //                if (withBlock4.IsFeatureAvailable(argfname25) & !withBlock4.IsConditionSatisfied(argIndex74) & !withBlock4.IsConditionSatisfied(argIndex75))
            //                {
            //                    var loopTo23 = withBlock4.CountFeature();
            //                    for (i = 1; i <= loopTo23; i++)
            //                    {
            //                        // 3体以上からなる合体能力を持っているか？
            //                        string localFeature() { object argIndex1 = i; var ret = withBlock4.Feature(argIndex1); return ret; }

            //                        string localFeatureName() { object argIndex1 = i; var ret = withBlock4.FeatureName(argIndex1); return ret; }

            //                        string localFeatureData10() { object argIndex1 = i; var ret = withBlock4.FeatureData(argIndex1); return ret; }

            //                        int localLLength2() { string arglist = hsc81ad842db7849b2b1585eed09bb5348(); var ret = GeneralLib.LLength(arglist); return ret; }

            //                        if (localFeature() == "合体" & !string.IsNullOrEmpty(localFeatureName()) & localLLength2() > 3)
            //                        {
            //                            n = 0;
            //                            // パートナーは隣接しているか？
            //                            string localFeatureData6() { object argIndex1 = i; var ret = withBlock4.FeatureData(argIndex1); return ret; }

            //                            string arglist17 = localFeatureData6();
            //                            var loopTo24 = GeneralLib.LLength(arglist17);
            //                            for (j = 3; j <= loopTo24; j++)
            //                            {
            //                                string localFeatureData5() { object argIndex1 = i; var ret = withBlock4.FeatureData(argIndex1); return ret; }

            //                                string localLIndex16() { string arglist = hsd2010d4d78e3489683c8d110139f0dc7(); var ret = GeneralLib.LIndex(arglist, j); return ret; }

            //                                object argIndex71 = localLIndex16();
            //                                u = SRC.UList.Item(argIndex71);
            //                                if (u is null)
            //                                {
            //                                    break;
            //                                }

            //                                if (!u.IsOperational())
            //                                {
            //                                    break;
            //                                }

            //                                string argfname24 = "合体制限";
            //                                if (u.Status_Renamed != "出撃" & u.CurrentForm().IsFeatureAvailable(argfname24))
            //                                {
            //                                    break;
            //                                }

            //                                if (Math.Abs((withBlock4.x - u.CurrentForm().x)) + Math.Abs((withBlock4.y - u.CurrentForm().y)) > 2)
            //                                {
            //                                    break;
            //                                }

            //                                n = (n + 1);
            //                            }

            //                            // 合体先のユニットが作成され、かつ合体可能な状態にあるか？
            //                            string localFeatureData7() { object argIndex1 = i; var ret = withBlock4.FeatureData(argIndex1); return ret; }

            //                            string arglist18 = localFeatureData7();
            //                            uname = GeneralLib.LIndex(arglist18, 2);
            //                            object argIndex72 = uname;
            //                            u = SRC.UList.Item(argIndex72);
            //                            object argIndex73 = "行動不能";
            //                            if (u is null)
            //                            {
            //                                n = 0;
            //                            }
            //                            else if (u.IsConditionSatisfied(argIndex73))
            //                            {
            //                                n = 0;
            //                            }

            //                            // すべての条件を満たしている場合
            //                            string localFeatureData9() { object argIndex1 = i; var ret = withBlock4.FeatureData(argIndex1); return ret; }

            //                            int localLLength1() { string arglist = hs7cff35930ba14e62b7ee40e2d0172e97(); var ret = GeneralLib.LLength(arglist); return ret; }

            //                            if (n == localLLength1() - 2)
            //                            {
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Visible = true;
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                string localFeatureData8() { object argIndex1 = i; var ret = withBlock4.FeatureData(argIndex1); return ret; }

            //                                string arglist19 = localFeatureData8();
            //                                GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Caption = GeneralLib.LIndex(arglist19, 1);
            //                                break;
            //                            }
            //                        }
            //                    }
            //                }

            //                object argIndex88 = "ノーマルモード付加";
            //                if (!withBlock4.IsConditionSatisfied(argIndex88))
            //                {
            //                    // ハイパーモードコマンド
            //                    string argfname26 = "ハイパーモード";
            //                    object argIndex79 = "ハイパーモード";
            //                    object argIndex80 = "ハイパーモード";
            //                    object argIndex81 = "ハイパーモード";
            //                    object argIndex82 = "ハイパーモード";
            //                    object argIndex83 = "ハイパーモード";
            //                    object argIndex84 = "形態固定";
            //                    object argIndex85 = "機体固定";
            //                    if (withBlock4.IsFeatureAvailable(argfname26) & (withBlock4.MainPilot().Morale >= (10d * withBlock4.FeatureLevel(argIndex80)) + 100 | withBlock4.HP <= withBlock4.MaxHP / 4 & Strings.InStr(withBlock4.FeatureData(argIndex81), "気力発動") == 0) & Strings.InStr(withBlock4.FeatureData(argIndex82), "自動発動") == 0 & !string.IsNullOrEmpty(withBlock4.FeatureName(argIndex83)) & !withBlock4.IsConditionSatisfied(argIndex84) & !withBlock4.IsConditionSatisfied(argIndex85))
            //                    {
            //                        object argIndex76 = "ハイパーモード";
            //                        string arglist20 = withBlock4.FeatureData(argIndex76);
            //                        uname = GeneralLib.LIndex(arglist20, 2);
            //                        Unit localOtherForm5() { object argIndex1 = uname; var ret = withBlock4.OtherForm(argIndex1); return ret; }

            //                        Unit localOtherForm6() { object argIndex1 = uname; var ret = withBlock4.OtherForm(argIndex1); return ret; }

            //                        object argIndex78 = "行動不能";
            //                        if (!localOtherForm5().IsConditionSatisfied(argIndex78) & localOtherForm6().IsAbleToEnter(withBlock4.x, withBlock4.y))
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = true;
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            object argIndex77 = "ハイパーモード";
            //                            string arglist21 = withBlock4.FeatureData(argIndex77);
            //                            GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = GeneralLib.LIndex(arglist21, 1);
            //                        }
            //                    }
            //                }
            //                else
            //                {
            //                    // 変身解除
            //                    object argIndex87 = "ノーマルモード";
            //                    if (Strings.InStr(withBlock4.FeatureData(argIndex87), "手動解除") > 0)
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = true;
            //                        string argfname27 = "変身解除コマンド名";
            //                        if (withBlock4.IsFeatureAvailable(argfname27))
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            object argIndex86 = "変身解除コマンド名";
            //                            GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = withBlock4.FeatureData(argIndex86);
            //                        }
            //                        else if (withBlock4.IsHero())
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "変身解除";
            //                        }
            //                        else
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "特殊モード解除";
            //                        }
            //                    }
            //                }

            //                // 地上コマンド
            //                if (Map.TerrainClass(withBlock4.x, withBlock4.y) == "陸" | Map.TerrainClass(withBlock4.x, withBlock4.y) == "屋内" | Map.TerrainClass(withBlock4.x, withBlock4.y) == "月面")
            //                {
            //                    string argarea_name = "陸";
            //                    if (withBlock4.Area != "地上" & withBlock4.IsTransAvailable(argarea_name))
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(GroundCmdID).Caption = "地上";
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(GroundCmdID).Visible = true;
            //                    }
            //                }
            //                else if (Map.TerrainClass(withBlock4.x, withBlock4.y) == "水" | Map.TerrainClass(withBlock4.x, withBlock4.y) == "深水")
            //                {
            //                    string argarea_name1 = "水上";
            //                    if (withBlock4.Area != "水上" & withBlock4.IsTransAvailable(argarea_name1))
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(GroundCmdID).Caption = "水上";
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(GroundCmdID).Visible = true;
            //                    }
            //                }

            //                // 空中コマンド
            //                switch (Map.TerrainClass(withBlock4.x, withBlock4.y) ?? "")
            //                {
            //                    case "宇宙":
            //                        {
            //                            break;
            //                        }

            //                    case "月面":
            //                        {
            //                            string argarea_name2 = "空";
            //                            string argarea_name3 = "宇宙";
            //                            if ((withBlock4.IsTransAvailable(argarea_name2) | withBlock4.IsTransAvailable(argarea_name3)) & !(withBlock4.Area == "宇宙"))
            //                            {
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(SkyCmdID).Caption = "宇宙";
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(SkyCmdID).Visible = true;
            //                            }

            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            string argarea_name4 = "空";
            //                            if (withBlock4.IsTransAvailable(argarea_name4) & !(withBlock4.Area == "空中"))
            //                            {
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(SkyCmdID).Caption = "空中";
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(SkyCmdID).Visible = true;
            //                            }

            //                            break;
            //                        }
            //                }

            //                // 地中コマンド
            //                string argarea_name5 = "地中";
            //                if (withBlock4.IsTransAvailable(argarea_name5) & !(withBlock4.Area == "地中") & (Map.TerrainClass(withBlock4.x, withBlock4.y) == "陸" | Map.TerrainClass(withBlock4.x, withBlock4.y) == "月面"))
            //                {
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    GUI.MainForm.mnuUnitCommandItem(UndergroundCmdID).Visible = true;
            //                }

            //                // 水中コマンド
            //                if (withBlock4.Area != "水中")
            //                {
            //                    string argarea_name6 = "水";
            //                    string argfname28 = "水泳";
            //                    if (Map.TerrainClass(withBlock4.x, withBlock4.y) == "深水" & (withBlock4.IsTransAvailable(argarea_name6) | withBlock4.IsFeatureAvailable(argfname28)) & Strings.Mid(withBlock4.Data.Adaption, 3, 1) != "-")
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(WaterCmdID).Visible = true;
            //                    }
            //                    else if (Map.TerrainClass(withBlock4.x, withBlock4.y) == "水" & Strings.Mid(withBlock4.Data.Adaption, 3, 1) != "-")
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(WaterCmdID).Visible = true;
            //                    }
            //                }

            //                // 発進コマンド
            //                string argfname29 = "母艦";
            //                if (withBlock4.IsFeatureAvailable(argfname29) & withBlock4.Area != "地中")
            //                {
            //                    if (withBlock4.CountUnitOnBoard() > 0)
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(LaunchCmdID).Visible = true;
            //                    }
            //                }

            //                // アイテムコマンド
            //                var loopTo25 = withBlock4.CountAbility();
            //                for (i = 1; i <= loopTo25; i++)
            //                {
            //                    string argref_mode5 = "移動前";
            //                    if (withBlock4.IsAbilityUseful(i, argref_mode5) & withBlock4.Ability(i).IsItem())
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(ItemCmdID).Visible = true;
            //                        break;
            //                    }
            //                }

            //                if (withBlock4.Area == "地中")
            //                {
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    GUI.MainForm.mnuUnitCommandItem(ItemCmdID).Visible = false;
            //                }

            //                // 召喚解除コマンド
            //                var loopTo26 = withBlock4.CountServant();
            //                for (i = 1; i <= loopTo26; i++)
            //                {
            //                    Unit localServant() { object argIndex1 = i; var ret = withBlock4.Servant(argIndex1); return ret; }

            //                    {
            //                        var withBlock12 = localServant().CurrentForm();
            //                        switch (withBlock12.Status_Renamed ?? "")
            //                        {
            //                            case "出撃":
            //                            case "格納":
            //                                {
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.mnuUnitCommandItem(DismissCmdID).Visible = true;
            //                                    break;
            //                                }

            //                            case "旧主形態":
            //                            case "旧形態":
            //                                {
            //                                    // 合体後の形態が出撃中なら使用不可
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.mnuUnitCommandItem(DismissCmdID).Visible = true;
            //                                    var loopTo27 = withBlock12.CountFeature();
            //                                    for (j = 1; j <= loopTo27; j++)
            //                                    {
            //                                        object argIndex90 = j;
            //                                        if (withBlock12.Feature(argIndex90) == "合体")
            //                                        {
            //                                            string localFeatureData11() { object argIndex1 = j; var ret = withBlock12.FeatureData(argIndex1); return ret; }

            //                                            string arglist22 = localFeatureData11();
            //                                            uname = GeneralLib.LIndex(arglist22, 2);
            //                                            object argIndex89 = uname;
            //                                            if (SRC.UList.IsDefined(argIndex89))
            //                                            {
            //                                                Unit localItem2() { object argIndex1 = uname; var ret = SRC.UList.Item(argIndex1); return ret; }

            //                                                {
            //                                                    var withBlock13 = localItem2().CurrentForm();
            //                                                    if (withBlock13.Status_Renamed == "出撃" | withBlock13.Status_Renamed == "格納")
            //                                                    {
            //                                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                                        GUI.MainForm.mnuUnitCommandItem(DismissCmdID).Visible = false;
            //                                                    }
            //                                                }
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }
            //                        }
            //                    }
            //                }

            //                string argfname30 = "召喚解除コマンド名";
            //                if (withBlock4.IsFeatureAvailable(argfname30))
            //                {
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    object argIndex91 = "召喚解除コマンド名";
            //                    GUI.MainForm.mnuUnitCommandItem(DismissCmdID).Caption = withBlock4.FeatureData(argIndex91);
            //                }
            //                else
            //                {
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    GUI.MainForm.mnuUnitCommandItem(DismissCmdID).Caption = "召喚解除";
            //                }

            //                // ユニットコマンド
            //                i = UnitCommand1CmdID;
            //                foreach (LabelData currentLab3 in Event_Renamed.colEventLabelList)
            //                {
            //                    lab = currentLab3;
            //                    if (lab.Name == Event_Renamed.LabelType.UnitCommandEventLabel)
            //                    {
            //                        if (lab.Enable)
            //                        {
            //                            buf = lab.Para(3);
            //                            if (SelectedUnit.Party == "味方" & ((buf ?? "") == (SelectedUnit.MainPilot().Name ?? "") | (buf ?? "") == (SelectedUnit.MainPilot().get_Nickname(false) ?? "") | (buf ?? "") == (SelectedUnit.Name ?? "")) | (buf ?? "") == (SelectedUnit.Party ?? "") | buf == "全")
            //                            {
            //                                int localStrToLng2() { string argexpr = lab.Para(4); var ret = GeneralLib.StrToLng(argexpr); return ret; }

            //                                if (lab.CountPara() <= 3)
            //                                {
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
            //                                }
            //                                else if (localStrToLng2() != 0)
            //                                {
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
            //                                }
            //                            }
            //                        }

            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        if (GUI.MainForm.mnuUnitCommandItem(i).Visible)
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(i).Caption = lab.Para(2);
            //                            UnitCommandLabelList[i - UnitCommand1CmdID + 1] = lab.LineNum.ToString();
            //                            i = (i + 1);
            //                            if (i > UnitCommand10CmdID)
            //                            {
            //                                break;
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            if (!ReferenceEquals(SelectedUnit, Status.DisplayedUnit))
            //            {
            //                // MOD START 240a
            //                // DisplayUnitStatus SelectedUnit
            //                // 新ＧＵＩ使用時はクリック時にユニットステータスを表示しない
            //                if (!GUI.NewGUIMode)
            //                {
            //                    Status.DisplayUnitStatus(SelectedUnit);
            //                }
            //                // MOD  END  240a
            //            }

            //            GUI.IsGUILocked = false;
            //            if (by_cancel)
            //            {
            //                // UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                // UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
            //                GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY + 5f);
            //            }
            //            else
            //            {
            //                // UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                // UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
            //                GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY - 6f);
            //            }

            //            break;
            //        }

            //    case "移動後コマンド選択":
            //        {
            //            Event_Renamed.SelectedUnitForEvent = SelectedUnit;
            //            {
            //                var withBlock14 = SelectedUnit;
            //                // 移動時にＥＮを消費している場合はステータスウィンドウを更新
            //                // MOD START MARGE
            //                // If MainWidth = 15 Then
            //                if (!GUI.NewGUIMode)
            //                {
            //                    // MOD END MARGE
            //                    if (PrevUnitEN != withBlock14.EN)
            //                    {
            //                        Status.DisplayUnitStatus(SelectedUnit);
            //                    }
            //                }

            //                {
            //                    var withBlock15 = GUI.MainForm;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock15.mnuUnitCommandItem(WaitCmdID).Visible = true;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock15.mnuUnitCommandItem(MoveCmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock15.mnuUnitCommandItem(TeleportCmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock15.mnuUnitCommandItem(JumpCmdID).Visible = false;
            //                }

            //                // 会話コマンド
            //                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                GUI.MainForm.mnuUnitCommandItem(TalkCmdID).Visible = false;
            //                for (i = 1; i <= 4; i++)
            //                {
            //                    // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                    u = null;
            //                    switch (i)
            //                    {
            //                        case 1:
            //                            {
            //                                if (withBlock14.x > 1)
            //                                {
            //                                    u = Map.MapDataForUnit[withBlock14.x - 1, withBlock14.y];
            //                                }

            //                                break;
            //                            }

            //                        case 2:
            //                            {
            //                                if (withBlock14.x < Map.MapWidth)
            //                                {
            //                                    u = Map.MapDataForUnit[withBlock14.x + 1, withBlock14.y];
            //                                }

            //                                break;
            //                            }

            //                        case 3:
            //                            {
            //                                if (withBlock14.y > 1)
            //                                {
            //                                    u = Map.MapDataForUnit[withBlock14.x, withBlock14.y - 1];
            //                                }

            //                                break;
            //                            }

            //                        case 4:
            //                            {
            //                                if (withBlock14.y < Map.MapHeight)
            //                                {
            //                                    u = Map.MapDataForUnit[withBlock14.x, withBlock14.y + 1];
            //                                }

            //                                break;
            //                            }
            //                    }

            //                    if (u is object)
            //                    {
            //                        string arglname2 = "会話 " + withBlock14.MainPilot().ID + " " + u.MainPilot().ID;
            //                        if (Event_Renamed.IsEventDefined(arglname2))
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(TalkCmdID).Visible = true;
            //                            break;
            //                        }
            //                    }
            //                }

            //                // 攻撃コマンド
            //                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Caption = "攻撃";
            //                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = false;
            //                var loopTo28 = withBlock14.CountWeapon();
            //                for (i = 1; i <= loopTo28; i++)
            //                {
            //                    string argref_mode6 = "移動後";
            //                    if (withBlock14.IsWeaponUseful(i, argref_mode6))
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = true;
            //                        break;
            //                    }
            //                }

            //                if (withBlock14.Area == "地中")
            //                {
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = false;
            //                }

            //                object argIndex92 = "攻撃不能";
            //                if (withBlock14.IsConditionSatisfied(argIndex92))
            //                {
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = false;
            //                }

            //                // 修理コマンド
            //                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                GUI.MainForm.mnuUnitCommandItem(FixCmdID).Visible = false;
            //                string argfname31 = "修理装置";
            //                if (withBlock14.IsFeatureAvailable(argfname31) & withBlock14.Area != "地中")
            //                {
            //                    for (i = 1; i <= 4; i++)
            //                    {
            //                        // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                        u = null;
            //                        switch (i)
            //                        {
            //                            case 1:
            //                                {
            //                                    if (withBlock14.x > 1)
            //                                    {
            //                                        u = Map.MapDataForUnit[withBlock14.x - 1, withBlock14.y];
            //                                    }

            //                                    break;
            //                                }

            //                            case 2:
            //                                {
            //                                    if (withBlock14.x < Map.MapWidth)
            //                                    {
            //                                        u = Map.MapDataForUnit[withBlock14.x + 1, withBlock14.y];
            //                                    }

            //                                    break;
            //                                }

            //                            case 3:
            //                                {
            //                                    if (withBlock14.y > 1)
            //                                    {
            //                                        u = Map.MapDataForUnit[withBlock14.x, withBlock14.y - 1];
            //                                    }

            //                                    break;
            //                                }

            //                            case 4:
            //                                {
            //                                    if (withBlock14.y < Map.MapHeight)
            //                                    {
            //                                        u = Map.MapDataForUnit[withBlock14.x, withBlock14.y + 1];
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (u is object)
            //                        {
            //                            {
            //                                var withBlock16 = u;
            //                                if ((withBlock16.Party == "味方" | withBlock16.Party == "ＮＰＣ") & withBlock16.HP < withBlock16.MaxHP)
            //                                {
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.mnuUnitCommandItem(FixCmdID).Visible = true;
            //                                    break;
            //                                }
            //                            }
            //                        }
            //                    }

            //                    object argIndex94 = "修理装置";
            //                    if (Strings.Len(withBlock14.FeatureData(argIndex94)) > 0)
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        object argIndex93 = "修理装置";
            //                        string arglist23 = withBlock14.FeatureData(argIndex93);
            //                        GUI.MainForm.mnuUnitCommandItem(FixCmdID).Caption = GeneralLib.LIndex(arglist23, 1);
            //                        string localLIndex19() { object argIndex1 = "修理装置"; string arglist = withBlock14.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                        if (Information.IsNumeric(localLIndex19()))
            //                        {
            //                            string localLIndex17() { object argIndex1 = "修理装置"; string arglist = withBlock14.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                            string localLIndex18() { object argIndex1 = "修理装置"; string arglist = withBlock14.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                            if (withBlock14.EN < Conversions.Toint(localLIndex18()))
            //                            {
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(FixCmdID).Visible = false;
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(FixCmdID).Caption = "修理装置";
            //                    }
            //                }

            //                // 補給コマンド
            //                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = false;
            //                string argfname32 = "補給装置";
            //                if (withBlock14.IsFeatureAvailable(argfname32) & withBlock14.Area != "地中")
            //                {
            //                    for (i = 1; i <= 4; i++)
            //                    {
            //                        // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                        u = null;
            //                        switch (i)
            //                        {
            //                            case 1:
            //                                {
            //                                    if (withBlock14.x > 1)
            //                                    {
            //                                        u = Map.MapDataForUnit[withBlock14.x - 1, withBlock14.y];
            //                                    }

            //                                    break;
            //                                }

            //                            case 2:
            //                                {
            //                                    if (withBlock14.x < Map.MapWidth)
            //                                    {
            //                                        u = Map.MapDataForUnit[withBlock14.x + 1, withBlock14.y];
            //                                    }

            //                                    break;
            //                                }

            //                            case 3:
            //                                {
            //                                    if (withBlock14.y > 1)
            //                                    {
            //                                        u = Map.MapDataForUnit[withBlock14.x, withBlock14.y - 1];
            //                                    }

            //                                    break;
            //                                }

            //                            case 4:
            //                                {
            //                                    if (withBlock14.y < Map.MapHeight)
            //                                    {
            //                                        u = Map.MapDataForUnit[withBlock14.x, withBlock14.y + 1];
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (u is object)
            //                        {
            //                            {
            //                                var withBlock17 = u;
            //                                if (withBlock17.Party == "味方" | withBlock17.Party == "ＮＰＣ")
            //                                {
            //                                    if (withBlock17.EN < withBlock17.MaxEN)
            //                                    {
            //                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                        GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = true;
            //                                    }
            //                                    else
            //                                    {
            //                                        var loopTo29 = withBlock17.CountWeapon();
            //                                        for (j = 1; j <= loopTo29; j++)
            //                                        {
            //                                            if (withBlock17.Bullet(j) < withBlock17.MaxBullet(j))
            //                                            {
            //                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                                GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = true;
            //                                                break;
            //                                            }
            //                                        }

            //                                        var loopTo30 = withBlock17.CountAbility();
            //                                        for (j = 1; j <= loopTo30; j++)
            //                                        {
            //                                            if (withBlock17.Stock(j) < withBlock17.MaxStock(j))
            //                                            {
            //                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                                GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = true;
            //                                                break;
            //                                            }
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }

            //                    object argIndex96 = "補給装置";
            //                    if (Strings.Len(withBlock14.FeatureData(argIndex96)) > 0)
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        object argIndex95 = "補給装置";
            //                        string arglist24 = withBlock14.FeatureData(argIndex95);
            //                        GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = GeneralLib.LIndex(arglist24, 1);
            //                        string localLIndex22() { object argIndex1 = "補給装置"; string arglist = withBlock14.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                        if (Information.IsNumeric(localLIndex22()))
            //                        {
            //                            string localLIndex20() { object argIndex1 = "補給装置"; string arglist = withBlock14.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                            string localLIndex21() { object argIndex1 = "補給装置"; string arglist = withBlock14.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                            if (withBlock14.EN < Conversions.Toint(localLIndex21()) | withBlock14.MainPilot().Morale < 100)
            //                            {
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = false;
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = "補給装置";
            //                    }

            //                    string argoname9 = "移動後補給不可";
            //                    string argsname = "補給";
            //                    if (Expression.IsOptionDefined(argoname9) & !SelectedUnit.MainPilot().IsSkillAvailable(argsname))
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = false;
            //                    }
            //                }

            //                // アビリティコマンド
            //                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = false;
            //                n = 0;
            //                var loopTo31 = withBlock14.CountAbility();
            //                for (i = 1; i <= loopTo31; i++)
            //                {
            //                    if (!withBlock14.Ability(i).IsItem())
            //                    {
            //                        n = (n + 1);
            //                        string argref_mode7 = "移動後";
            //                        if (withBlock14.IsAbilityUseful(i, argref_mode7))
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = true;
            //                        }
            //                    }
            //                }

            //                if (withBlock14.Area == "地中")
            //                {
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = false;
            //                }
            //                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                string argtname4 = "アビリティ";
            //                GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = Expression.Term(argtname4, SelectedUnit);
            //                if (n == 1)
            //                {
            //                    var loopTo32 = withBlock14.CountAbility();
            //                    for (i = 1; i <= loopTo32; i++)
            //                    {
            //                        if (!withBlock14.Ability(i).IsItem())
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = withBlock14.AbilityNickname(i);
            //                            break;
            //                        }
            //                    }
            //                }

            //                {
            //                    var withBlock18 = GUI.MainForm;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock18.mnuUnitCommandItem(ChargeCmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock18.mnuUnitCommandItem(SpecialPowerCmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock18.mnuUnitCommandItem(TransformCmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock18.mnuUnitCommandItem(SplitCmdID).Visible = false;
            //                }

            //                // 合体コマンド
            //                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Visible = false;
            //                string argfname34 = "合体";
            //                object argIndex100 = "形態固定";
            //                object argIndex101 = "機体固定";
            //                if (withBlock14.IsFeatureAvailable(argfname34) & !withBlock14.IsConditionSatisfied(argIndex100) & !withBlock14.IsConditionSatisfied(argIndex101))
            //                {
            //                    var loopTo33 = withBlock14.CountFeature();
            //                    for (i = 1; i <= loopTo33; i++)
            //                    {
            //                        // 3体以上からなる合体能力を持っているか？
            //                        string localFeature1() { object argIndex1 = i; var ret = withBlock14.Feature(argIndex1); return ret; }

            //                        string localFeatureName1() { object argIndex1 = i; var ret = withBlock14.FeatureName(argIndex1); return ret; }

            //                        string localFeatureData17() { object argIndex1 = i; var ret = withBlock14.FeatureData(argIndex1); return ret; }

            //                        int localLLength4() { string arglist = hs9469e7ffcaeb496ca82716c7891638cc(); var ret = GeneralLib.LLength(arglist); return ret; }

            //                        if (localFeature1() == "合体" & !string.IsNullOrEmpty(localFeatureName1()) & localLLength4() > 3)
            //                        {
            //                            n = 0;
            //                            string localFeatureData13() { object argIndex1 = i; var ret = withBlock14.FeatureData(argIndex1); return ret; }

            //                            string arglist25 = localFeatureData13();
            //                            var loopTo34 = GeneralLib.LLength(arglist25);
            //                            for (j = 3; j <= loopTo34; j++)
            //                            {
            //                                string localFeatureData12() { object argIndex1 = i; var ret = withBlock14.FeatureData(argIndex1); return ret; }

            //                                string localLIndex23() { string arglist = hs175f067af849438ea2ce369fbd24d08f(); var ret = GeneralLib.LIndex(arglist, j); return ret; }

            //                                object argIndex97 = localLIndex23();
            //                                u = SRC.UList.Item(argIndex97);
            //                                if (u is null)
            //                                {
            //                                    break;
            //                                }

            //                                if (!u.IsOperational())
            //                                {
            //                                    break;
            //                                }

            //                                string argfname33 = "合体制限";
            //                                if (u.Status_Renamed != "出撃" & u.CurrentForm().IsFeatureAvailable(argfname33))
            //                                {
            //                                    break;
            //                                }

            //                                if (Math.Abs((withBlock14.x - u.CurrentForm().x)) + Math.Abs((withBlock14.y - u.CurrentForm().y)) > 2)
            //                                {
            //                                    break;
            //                                }

            //                                n = (n + 1);
            //                            }

            //                            string localFeatureData14() { object argIndex1 = i; var ret = withBlock14.FeatureData(argIndex1); return ret; }

            //                            string arglist26 = localFeatureData14();
            //                            uname = GeneralLib.LIndex(arglist26, 2);
            //                            object argIndex98 = uname;
            //                            u = SRC.UList.Item(argIndex98);
            //                            object argIndex99 = "行動不能";
            //                            if (u is null)
            //                            {
            //                                n = 0;
            //                            }
            //                            else if (u.IsConditionSatisfied(argIndex99))
            //                            {
            //                                n = 0;
            //                            }

            //                            string localFeatureData16() { object argIndex1 = i; var ret = withBlock14.FeatureData(argIndex1); return ret; }

            //                            int localLLength3() { string arglist = hse55df985d6054cfe94b90350a9c471f6(); var ret = GeneralLib.LLength(arglist); return ret; }

            //                            if (n == localLLength3() - 2)
            //                            {
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Visible = true;
            //                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                string localFeatureData15() { object argIndex1 = i; var ret = withBlock14.FeatureData(argIndex1); return ret; }

            //                                string arglist27 = localFeatureData15();
            //                                GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Caption = GeneralLib.LIndex(arglist27, 1);
            //                                break;
            //                            }
            //                        }
            //                    }
            //                }

            //                {
            //                    var withBlock19 = GUI.MainForm;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock19.mnuUnitCommandItem(HyperModeCmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock19.mnuUnitCommandItem(GroundCmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock19.mnuUnitCommandItem(SkyCmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock19.mnuUnitCommandItem(UndergroundCmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock19.mnuUnitCommandItem(WaterCmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock19.mnuUnitCommandItem(LaunchCmdID).Visible = false;
            //                }

            //                // アイテムコマンド
            //                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                GUI.MainForm.mnuUnitCommandItem(ItemCmdID).Visible = false;
            //                var loopTo35 = withBlock14.CountAbility();
            //                for (i = 1; i <= loopTo35; i++)
            //                {
            //                    string argref_mode8 = "移動後";
            //                    if (withBlock14.IsAbilityUseful(i, argref_mode8) & withBlock14.Ability(i).IsItem())
            //                    {
            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        GUI.MainForm.mnuUnitCommandItem(ItemCmdID).Visible = true;
            //                        break;
            //                    }
            //                }

            //                if (withBlock14.Area == "地中")
            //                {
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    GUI.MainForm.mnuUnitCommandItem(ItemCmdID).Visible = false;
            //                }

            //                {
            //                    var withBlock20 = GUI.MainForm;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock20.mnuUnitCommandItem(DismissCmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock20.mnuUnitCommandItem(OrderCmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock20.mnuUnitCommandItem(FeatureListCmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock20.mnuUnitCommandItem(WeaponListCmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock20.mnuUnitCommandItem(AbilityListCmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock20.mnuUnitCommandItem(UnitCommand1CmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock20.mnuUnitCommandItem(UnitCommand2CmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock20.mnuUnitCommandItem(UnitCommand3CmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock20.mnuUnitCommandItem(UnitCommand4CmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock20.mnuUnitCommandItem(UnitCommand5CmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock20.mnuUnitCommandItem(UnitCommand6CmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock20.mnuUnitCommandItem(UnitCommand7CmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock20.mnuUnitCommandItem(UnitCommand8CmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock20.mnuUnitCommandItem(UnitCommand9CmdID).Visible = false;
            //                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    withBlock20.mnuUnitCommandItem(UnitCommand10CmdID).Visible = false;
            //                }

            //                // ユニットコマンド
            //                i = UnitCommand1CmdID;
            //                foreach (LabelData currentLab4 in Event_Renamed.colEventLabelList)
            //                {
            //                    lab = currentLab4;
            //                    if (lab.Name == Event_Renamed.LabelType.UnitCommandEventLabel & lab.AsterNum >= 2)
            //                    {
            //                        if (lab.Enable)
            //                        {
            //                            buf = lab.Para(3);
            //                            if (SelectedUnit.Party == "味方" & ((buf ?? "") == (SelectedUnit.MainPilot().Name ?? "") | (buf ?? "") == (SelectedUnit.MainPilot().get_Nickname(false) ?? "") | (buf ?? "") == (SelectedUnit.Name ?? "")) | (buf ?? "") == (SelectedUnit.Party ?? "") | buf == "全")
            //                            {
            //                                int localStrToLng3() { string argexpr = lab.Para(4); var ret = GeneralLib.StrToLng(argexpr); return ret; }

            //                                if (lab.CountPara() <= 3)
            //                                {
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
            //                                }
            //                                else if (localStrToLng3() != 0)
            //                                {
            //                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                                    GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
            //                                }
            //                            }
            //                        }

            //                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                        if (GUI.MainForm.mnuUnitCommandItem(i).Visible)
            //                        {
            //                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                            GUI.MainForm.mnuUnitCommandItem(i).Caption = lab.Para(2);
            //                            UnitCommandLabelList[i - UnitCommand1CmdID + 1] = lab.LineNum.ToString();
            //                            i = (i + 1);
            //                            if (i > UnitCommand10CmdID)
            //                            {
            //                                break;
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            GUI.IsGUILocked = false;
            //            if (by_cancel)
            //            {
            //                // UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                // UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
            //                GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY + 5f);
            //            }
            //            else
            //            {
            //                // UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                // UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
            //                GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY - 6f);
            //                Application.DoEvents();
            //                // ＰＣに負荷がかかったような状態だとポップアップメニューの選択が
            //                // うまく行えない場合があるのでやり直す
            //                while (CommandState == "移動後コマンド選択" & SelectedCommand == "移動")
            //                {
            //                    // UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //                    // UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
            //                    GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY - 6f);
            //                    Application.DoEvents();
            //                }
            //            }

            //            break;
            //        }

            //    case "ターゲット選択":
            //    case "移動後ターゲット選択":
            //        {
            //            if (!Map.MaskData[GUI.PixelToMapX(GUI.MouseX), GUI.PixelToMapY(GUI.MouseY)])
            //            {
            //                SelectedX = GUI.PixelToMapX(GUI.MouseX);
            //                SelectedY = GUI.PixelToMapY(GUI.MouseY);

            //                // 自分自身を選択された場合
            //                if (SelectedUnit.x == SelectedX & SelectedUnit.y == SelectedY)
            //                {
            //                    if (SelectedCommand == "スペシャルパワー")
            //                    {
            //                    }
            //                    // 下に抜ける
            //                    else if (SelectedCommand == "アビリティ" | SelectedCommand == "マップアビリティ" | SelectedCommand == "アイテム" | SelectedCommand == "マップアイテム")
            //                    {
            //                        if (SelectedUnit.AbilityMinRange(SelectedAbility) > 0)
            //                        {
            //                            // 自分自身は選択不可
            //                            GUI.IsGUILocked = false;
            //                            return;
            //                        }
            //                    }
            //                    else if (SelectedCommand == "移動命令")
            //                    {
            //                    }
            //                    // 下に抜ける
            //                    else
            //                    {
            //                        // 自分自身は選択不可
            //                        GUI.IsGUILocked = false;
            //                        return;
            //                    }
            //                }

            //                // 場所を選択するコマンド
            //                switch (SelectedCommand ?? "")
            //                {
            //                    // MOD START MARGE
            //                    // Case "移動"
            //                    case "移動":
            //                    case "再移動":
            //                        {
            //                            // MOD END MARGE
            //                            FinishMoveCommand();
            //                            GUI.IsGUILocked = false;
            //                            return;
            //                        }

            //                    case "テレポート":
            //                        {
            //                            FinishTeleportCommand();
            //                            GUI.IsGUILocked = false;
            //                            return;
            //                        }

            //                    case "ジャンプ":
            //                        {
            //                            FinishJumpCommand();
            //                            GUI.IsGUILocked = false;
            //                            return;
            //                        }

            //                    case "マップ攻撃":
            //                        {
            //                            MapAttackCommand();
            //                            GUI.IsGUILocked = false;
            //                            return;
            //                        }

            //                    case "マップアビリティ":
            //                    case "マップアイテム":
            //                        {
            //                            MapAbilityCommand();
            //                            GUI.IsGUILocked = false;
            //                            return;
            //                        }

            //                    case "発進":
            //                        {
            //                            FinishLaunchCommand();
            //                            GUI.IsGUILocked = false;
            //                            return;
            //                        }

            //                    case "移動命令":
            //                        {
            //                            FinishOrderCommand();
            //                            GUI.IsGUILocked = false;
            //                            return;
            //                        }
            //                }

            //                // これ以降はユニットを選択するコマンド

            //                // 指定した地点にユニットがいる？
            //                if (Map.MapDataForUnit[SelectedX, SelectedY] is null)
            //                {
            //                    GUI.IsGUILocked = false;
            //                    return;
            //                }

            //                // ターゲットを選択
            //                SelectedTarget = Map.MapDataForUnit[SelectedX, SelectedY];
            //                switch (SelectedCommand ?? "")
            //                {
            //                    case "攻撃":
            //                        {
            //                            FinishAttackCommand();
            //                            break;
            //                        }

            //                    case "アビリティ":
            //                    case "アイテム":
            //                        {
            //                            FinishAbilityCommand();
            //                            break;
            //                        }

            //                    case "会話":
            //                        {
            //                            FinishTalkCommand();
            //                            break;
            //                        }

            //                    case "修理":
            //                        {
            //                            FinishFixCommand();
            //                            break;
            //                        }

            //                    case "補給":
            //                        {
            //                            FinishSupplyCommand();
            //                            break;
            //                        }

            //                    case "スペシャルパワー":
            //                        {
            //                            FinishSpecialPowerCommand();
            //                            break;
            //                        }

            //                    case "攻撃命令":
            //                    case "護衛命令":
            //                        {
            //                            FinishOrderCommand();
            //                            break;
            //                        }
            //                }
            //            }

            //            break;
            //        }

            //    case "マップ攻撃使用":
            //    case "移動後マップ攻撃使用":
            //        {
            //            if (1 <= GUI.PixelToMapX(GUI.MouseX) & GUI.PixelToMapX(GUI.MouseX) <= Map.MapWidth)
            //            {
            //                if (1 <= GUI.PixelToMapY(GUI.MouseY) & GUI.PixelToMapY(GUI.MouseY) <= Map.MapHeight)
            //                {
            //                    if (!Map.MaskData[GUI.PixelToMapX(GUI.MouseX), GUI.PixelToMapY(GUI.MouseY)])
            //                    {
            //                        // 効果範囲内でクリックされればマップ攻撃発動
            //                        if (SelectedCommand == "マップ攻撃")
            //                        {
            //                            MapAttackCommand();
            //                        }
            //                        else
            //                        {
            //                            MapAbilityCommand();
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }
            //}

            GUI.IsGUILocked = false;
        }

        // ＧＵＩの処理をキャンセル
        public void CancelCommand()
        {
            int tmp_x, tmp_y;
            {
                var withBlock = SelectedUnit;
                //switch (CommandState ?? "")
                //{
                //    case "ユニット選択":
                //        {
                //            break;
                //        }

                //    case "コマンド選択":
                //        {
                //            CommandState = "ユニット選択";
                //            // ADD START
                //            // 選択したコマンドを初期化
                //            SelectedCommand = "";
                //            // MOD START MARGE
                //            // If MainWidth <> 15 Then
                //            if (GUI.NewGUIMode)
                //            {
                //                // MOD  END  MARGE
                //                Status.ClearUnitStatus();
                //            }

                //            break;
                //        }

                //    case "ターゲット選択":
                //        {
                //            // ADD START MARGE
                //            if (SelectedCommand == "再移動")
                //            {
                //                WaitCommand();
                //                return;
                //            }
                //            // ADD END MARGE
                //            CommandState = "コマンド選択";
                //            Status.DisplayUnitStatus(SelectedUnit);
                //            GUI.RedrawScreen();
                //            ProceedCommand(true);
                //            break;
                //        }

                //    case "移動後コマンド選択":
                //        {
                //            CommandState = "ターゲット選択";
                //            withBlock.Area = PrevUnitArea;
                //            withBlock.Move(PrevUnitX, PrevUnitY, true, true);
                //            withBlock.EN = PrevUnitEN;
                //            if (!ReferenceEquals(SelectedUnit, Map.MapDataForUnit[PrevUnitX, PrevUnitY]))
                //            {
                //                // 発進をキャンセルした場合
                //                SelectedTarget = SelectedUnit;
                //                GUI.PaintUnitBitmap(SelectedTarget);
                //                SelectedUnit = Map.MapDataForUnit[PrevUnitX, PrevUnitY];
                //            }
                //            // MOD START MARGE
                //            // ElseIf MainWidth = 15 Then
                //            else if (!GUI.NewGUIMode)
                //            {
                //                // MOD END MARGE
                //                Status.DisplayUnitStatus(SelectedUnit);
                //            }
                //            // MOD START MARGE
                //            // 移動後コマンドをキャンセルした場合、MoveCostを0にリセットする
                //            SelectedUnitMoveCost = 0;
                //            // MOD END MARGE
                //            switch (SelectedCommand ?? "")
                //            {
                //                case "移動":
                //                    {
                //                        StartMoveCommand();
                //                        break;
                //                    }

                //                case "テレポート":
                //                    {
                //                        StartTeleportCommand();
                //                        break;
                //                    }

                //                case "ジャンプ":
                //                    {
                //                        StartJumpCommand();
                //                        break;
                //                    }

                //                case "発進":
                //                    {
                //                        GUI.PaintUnitBitmap(SelectedTarget);
                //                        break;
                //                    }
                //            }

                //            break;
                //        }

                //    case "移動後ターゲット選択":
                //        {
                //            CommandState = "移動後コマンド選択";
                //            Status.DisplayUnitStatus(SelectedUnit);
                //            tmp_x = withBlock.x;
                //            tmp_y = withBlock.y;
                //            withBlock.x = PrevUnitX;
                //            withBlock.y = PrevUnitY;
                //            switch (PrevCommand ?? "")
                //            {
                //                case "移動":
                //                    {
                //                        Map.AreaInSpeed(SelectedUnit);
                //                        break;
                //                    }

                //                case "テレポート":
                //                    {
                //                        Map.AreaInTeleport(SelectedUnit);
                //                        break;
                //                    }

                //                case "ジャンプ":
                //                    {
                //                        Map.AreaInSpeed(SelectedUnit, true);
                //                        break;
                //                    }

                //                case "発進":
                //                    {
                //                        {
                //                            var withBlock1 = SelectedTarget;
                //                            string localLIndex() { object argIndex1 = "テレポート"; string arglist = withBlock1.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                //                            int localLLength() { object argIndex1 = "ジャンプ"; string arglist = withBlock1.FeatureData(argIndex1); var ret = GeneralLib.LLength(arglist); return ret; }

                //                            string localLIndex1() { object argIndex1 = "ジャンプ"; string arglist = withBlock1.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                //                            string argfname = "テレポート";
                //                            string argfname1 = "ジャンプ";
                //                            if (withBlock1.IsFeatureAvailable(argfname) & (withBlock1.Data.Speed == 0 | localLIndex() == "0"))
                //                            {
                //                                Map.AreaInTeleport(SelectedTarget);
                //                            }
                //                            else if (withBlock1.IsFeatureAvailable(argfname1) & (withBlock1.Data.Speed == 0 | localLLength() < 2 | localLIndex1() == "0"))
                //                            {
                //                                Map.AreaInSpeed(SelectedTarget, true);
                //                            }
                //                            else
                //                            {
                //                                Map.AreaInSpeed(SelectedTarget);
                //                            }
                //                        }

                //                        break;
                //                    }
                //            }

                //            withBlock.x = tmp_x;
                //            withBlock.y = tmp_y;
                //            SelectedCommand = PrevCommand;
                //            Map.MaskData[tmp_x, tmp_y] = false;
                //            GUI.MaskScreen();
                //            ProceedCommand(true);
                //            break;
                //        }

                //    case "マップ攻撃使用":
                //    case "移動後マップ攻撃使用":
                //        {
                //            if (CommandState == "マップ攻撃使用")
                //            {
                //                CommandState = "ターゲット選択";
                //            }
                //            else
                //            {
                //                CommandState = "移動後ターゲット選択";
                //            }

                //            if (SelectedCommand == "マップ攻撃")
                //            {
                //                string argattr = "Ｍ直";
                //                string argattr1 = "Ｍ移";
                //                if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, argattr))
                //                {
                //                    Map.AreaInCross(withBlock.x, withBlock.y, withBlock.WeaponMaxRange(SelectedWeapon), withBlock.Weapon(SelectedWeapon).MinRange);
                //                }
                //                else if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, argattr1))
                //                {
                //                    Map.AreaInMoveAction(SelectedUnit, withBlock.WeaponMaxRange(SelectedWeapon));
                //                }
                //                else
                //                {
                //                    string arguparty = "すべて";
                //                    Map.AreaInRange(withBlock.x, withBlock.y, withBlock.WeaponMaxRange(SelectedWeapon), withBlock.Weapon(SelectedWeapon).MinRange, arguparty);
                //                }
                //            }
                //            else
                //            {
                //                string argattr2 = "Ｍ直";
                //                string argattr3 = "Ｍ移";
                //                if (withBlock.IsAbilityClassifiedAs(SelectedAbility, argattr2))
                //                {
                //                    int argmax_range = withBlock.AbilityMinRange(SelectedAbility);
                //                    Map.AreaInCross(withBlock.x, withBlock.y, withBlock.AbilityMaxRange(SelectedAbility), argmax_range);
                //                }
                //                else if (withBlock.IsAbilityClassifiedAs(SelectedAbility, argattr3))
                //                {
                //                    Map.AreaInMoveAction(SelectedUnit, withBlock.AbilityMaxRange(SelectedAbility));
                //                }
                //                else
                //                {
                //                    string arguparty1 = "すべて";
                //                    Map.AreaInRange(withBlock.x, withBlock.y, withBlock.AbilityMaxRange(SelectedAbility), withBlock.AbilityMinRange(SelectedAbility), arguparty1);
                //                }
                //            }

                //            GUI.MaskScreen();
                //            break;
                //        }
                //}
            }
        }
    }
}
