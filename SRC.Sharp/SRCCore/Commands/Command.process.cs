using SRCCore.Events;
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

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
                            ProceedCommand(false, button, cell, unit);
                        }
                        break;
                    case "ターゲット選択":
                    case "移動後ターゲット選択":
                        if (cell != null && !Map.MaskData[cell.X, cell.Y])
                        {
                            ProceedCommand(false, button, cell, unit);
                        }
                        break;
                    case "コマンド選択":
                        CancelCommand();
                        // もし新しいクリック地点がユニットなら、ユニット選択の処理を進める
                        if (unit != null)
                        {
                            ProceedCommand(false, button, cell, unit);
                        }
                        break;

                    case "移動後コマンド選択":
                        CancelCommand();
                        break;

                    default:
                        ProceedCommand(false, button, cell, unit);
                        break;
                }
            }

            if (button == GuiButton.Right)
            {
                // 右クリック
                switch (CommandState ?? "")
                {
                    case "マップコマンド":
                        CommandState = "ユニット選択";
                        break;

                    case "ユニット選択":
                        ProceedCommand(true, button, cell, unit);
                        break;

                    default:
                        CancelCommand();
                        break;
                }
            }
        }

        // コマンドの処理を進める
        // by_cancel = True の場合はコマンドをキャンセルした場合の処理
        public void ProceedCommand(
            bool by_cancel = false,
            GuiButton button = GuiButton.None,
            MapCell cell = null,
            Unit unit = null)
        {
            LogDebug();

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
            switch (CommandState ?? "")
            {
                case "ユニット選択":
                case "マップコマンド":
                    ProceedUnitSelect(by_cancel, button, cell, unit);
                    break;

                case "コマンド選択":
                    ProceedCommandSelect(by_cancel, button, cell, unit);
                    break;

                case "移動後コマンド選択":
                    ProceedAfterMoveCommandSelect(by_cancel, button, cell, unit);
                    break;

                case "ターゲット選択":
                case "移動後ターゲット選択":
                    ProceedTargetSelect(by_cancel, button, cell, unit);
                    break;

                case "マップ攻撃使用":
                case "移動後マップ攻撃使用":
                    ProceedMapAttack(by_cancel, button, cell, unit);
                    break;

            }

            // XXX ロックしたままのパターンをフォローする
            GUI.IsGUILocked = false;
        }

        private void ProceedUnitSelect(
            bool by_cancel = false,
            GuiButton button = GuiButton.None,
            MapCell cell = null,
            Unit unit = null)
        {
            LogDebug();

            SelectedUnit = unit;
            SelectedUnitMoveCost = 0;

            var mapCommands = new List<UiCommand>();

            if (SelectedUnit is null)
            {
                SelectedX = GUI.PixelToMapX((int)GUI.MouseX);
                SelectedY = GUI.PixelToMapY((int)GUI.MouseY);
                if (!Map.IsStatusView)
                {
                    // 通常のステージ
                    Status.DisplayGlobalStatus();

                    // ターン終了
                    if (ViewMode)
                    {
                        mapCommands.Add(new UiCommand(EndTurnCmdID, "部隊編成に戻る"));
                    }
                    else
                    {
                        mapCommands.Add(new UiCommand(EndTurnCmdID, "ターン終了"));
                    }

                    // 中断
                    if (Expression.IsOptionDefined("デバッグ"))
                    {
                        mapCommands.Add(new UiCommand(DumpCmdID, "中断"));
                    }
                    else
                    {
                        if (!Expression.IsOptionDefined("クイックセーブ不可"))
                        {
                            mapCommands.Add(new UiCommand(DumpCmdID, "中断"));
                        }
                    }

                    // 部隊表
                    mapCommands.Add(new UiCommand(UnitListCmdID, "部隊表"));

                    // 全体マップ
                    mapCommands.Add(new UiCommand(GlobalMapCmdID, "全体マップ"));

                    // 作戦目的
                    if (Event.IsEventDefined("勝利条件"))
                    {
                        mapCommands.Add(new UiCommand(OperationObjectCmdID, "勝利条件"));
                    }

                    // 自動反撃モード
                    mapCommands.Add(new UiCommand(AutoDefenseCmdID, "自動反撃モード", SystemConfig.AutoDefense));

                    // 設定変更
                    mapCommands.Add(new UiCommand(ConfigurationCmdID, "設定変更"));

                    // リスタート
                    if (SRC.IsRestartSaveDataAvailable && !ViewMode)
                    {
                        mapCommands.Add(new UiCommand(RestartCmdID, "リスタート"));
                    }

                    // クイックロード
                    if (SRC.IsQuickSaveDataAvailable && !ViewMode)
                    {
                        mapCommands.Add(new UiCommand(QuickLoadCmdID, "クイックロード"));
                    }

                    // クイックセーブ
                    if (!ViewMode)
                    {
                        if (Expression.IsOptionDefined("デバッグ") || !Expression.IsOptionDefined("クイックセーブ不可"))
                        {
                            mapCommands.Add(new UiCommand(QuickSaveCmdID, "クイックセーブ"));
                        }
                    }
                }
                else
                {
                    // パイロットステータス・ユニットステータスのステージ
                }
                // スペシャルパワー検索
                if (SRC.PList.Items.Any(p => p.Party == "味方" && p.CountSpecialPower > 0))
                {
                    mapCommands.Add(new UiCommand(SearchSpecialPowerCmdID, Expression.Term("スペシャルパワー", u: null) + "検索"));
                }

                // イベントで定義されたマップコマンド
                if (!ViewMode)
                {
                    foreach (LabelData lab in Event.colEventLabelList.Values
                        .Where(x => x.Name == LabelType.MapCommandEventLabel && x.Enable))
                    {
                        if (lab.CountPara() == 2)
                        {
                            // 無条件で実行できるコマンド
                            mapCommands.Add(new UiCommand(MapCommandCmdID, lab.Para(2), lab));
                        }
                        else if (GeneralLib.StrToLng(lab.Para(3)) != 0)
                        {
                            // 条件を満たした場合のみ実行できるコマンド
                            mapCommands.Add(new UiCommand(MapCommandCmdID, lab.Para(2), lab));
                        }
                    }
                }

                CommandState = "マップコマンド";
                GUI.IsGUILocked = false;
                // XXX
                //// ADD START 240a
                //// ここに来た時点でcancel=Trueはユニットのいないセルを右クリックした場合のみ
                //if (by_cancel)
                //{
                //    if (GUI.NewGUIMode && !string.IsNullOrEmpty(Map.MapFileName))
                //    {
                //        if (GUI.MouseX < GUI.MainPWidth / 2)
                //        {
                //            GUI.MainForm.picUnitStatus.Move(GUI.MainPWidth - 240, 10);
                //        }
                //        else
                //        {
                //            GUI.MainForm.picUnitStatus.Move(5, 10);
                //        }
                //        GUI.MainForm.picUnitStatus.Visible = true;
                //    }
                //}
                //// ADD  END  240a
                ///
                GUI.ShowMapCommandMenu(mapCommands);
                return;
            }

            Event.SelectedUnitForEvent = SelectedUnit;
            SelectedWeapon = 0;
            SelectedTWeapon = 0;
            SelectedAbility = 0;
            if (by_cancel)
            {
                // ユニット上でキャンセルボタンを押した場合は武器一覧
                // もしくはアビリティ一覧を表示する
                // 情報が隠蔽されている場合は表示しない
                if (Expression.IsOptionDefined("ユニット情報隠蔽")
                    && !SelectedUnit.IsConditionSatisfied("識別済み")
                    && (SelectedUnit.Party0 == "敵"
                        || SelectedUnit.Party0 == "中立")
                        || SelectedUnit.IsConditionSatisfied("ユニット情報隠蔽")
                        || SelectedUnit.IsFeatureAvailable("ダミーユニット"))
                {
                    GUI.IsGUILocked = false;
                    return;
                }

                if (SelectedUnit.CountWeapon() == 0 && SelectedUnit.CountAbility() > 0)
                {
                    AbilityListCommand();
                }
                else
                {
                    WeaponListCommand();
                }

                GUI.IsGUILocked = false;
                return;
            }

            CommandState = "コマンド選択";
            ProceedCommand(by_cancel);
        }

        private void ProceedCommandSelect(
            bool by_cancel = false,
            GuiButton button = GuiButton.None,
            MapCell cell = null,
            Unit unit = null)
        {
            LogDebug();

            var unitCommands = new List<UiCommand>();

            //// MOD START 240aClearUnitStatus
            //// If MainWidth <> 15 Then
            //// DisplayUnitStatus SelectedUnit
            //// End If
            //if (!GUI.NewGUIMode)
            //{
            //    Status.DisplayUnitStatus(SelectedUnit);
            //}
            //else
            //{
            //    Status.ClearUnitStatus();
            //}
            //// MOD  END  240a

            // 武装一覧以外は一旦消しておく
            unitCommands.Add(new UiCommand(WeaponListCmdID, "武装一覧"));

            Event.SelectedUnitForEvent = SelectedUnit;
            SelectedTarget = null;
            Event.SelectedTargetForEvent = null;
            var currentUnit = SelectedUnit;
            {
                // 特殊能力＆アビリティ一覧はどのユニットでも見れる可能性があるので
                // 先に判定しておく

                // 特殊能力一覧コマンド
                {
                    var isFeatureListVisible = false;

                    foreach (var fd in currentUnit.AllFeatures)
                    {
                        if (!string.IsNullOrEmpty(fd.FeatureName(currentUnit)))
                        {
                            switch (fd.Name)
                            {
                                case "合体":
                                    if (SRC.UList.IsDefined(fd.DataL.Skip(1).First()))
                                    {
                                        isFeatureListVisible = true;
                                    }
                                    break;

                                default:
                                    isFeatureListVisible = true;
                                    break;
                            }
                        }
                        else if (fd.Name == "パイロット能力付加" || fd.Name == "パイロット能力強化")
                        {
                            if (Strings.InStr(fd.Data, "非表示") == 0)
                            {
                                isFeatureListVisible = true;
                            }
                        }
                        else if (fd.Name == "武器クラス" || fd.Name == "防具クラス")
                        {
                            if (Expression.IsOptionDefined("アイテム交換"))
                            {
                                isFeatureListVisible = true;
                            }
                        }
                    }
                    var p = currentUnit.MainPilot();
                    foreach (var skill in p.Skills.Where(s => p.SkillName0(s.Name) != "非表示" && !string.IsNullOrEmpty(p.SkillName0(s.Name))))
                    {
                        switch (skill.Name)
                        {
                            case "耐久":
                                if (!Expression.IsOptionDefined("防御力成長") && !Expression.IsOptionDefined("防御力レベルアップ"))
                                {
                                    isFeatureListVisible = true;
                                }
                                break;

                            case "追加レベル":
                            case "格闘ＵＰ":
                            case "射撃ＵＰ":
                            case "命中ＵＰ":
                            case "回避ＵＰ":
                            case "技量ＵＰ":
                            case "反応ＵＰ":
                            case "ＳＰＵＰ":
                            case "格闘ＤＯＷＮ":
                            case "射撃ＤＯＷＮ":
                            case "命中ＤＯＷＮ":
                            case "回避ＤＯＷＮ":
                            case "技量ＤＯＷＮ":
                            case "反応ＤＯＷＮ":
                            case "ＳＰＤＯＷＮ":
                            case "メッセージ":
                            case "魔力所有":
                                break;

                            default:
                                isFeatureListVisible = true;
                                break;
                        }
                        if (isFeatureListVisible) { break; }
                    }
                    if (isFeatureListVisible)
                    {
                        unitCommands.Add(new UiCommand(FeatureListCmdID, "特殊能力一覧"));
                    }
                }

                // アビリティ一覧コマンド
                if (currentUnit.Abilities.Any(x => x.IsAbilityMastered()
                     && !currentUnit.IsDisabled(x.Data.Name)
                     && (!x.IsAbilityClassifiedAs("合") || x.IsCombinationAbilityAvailable(true))
                     && !x.Data.IsItem()
                     ))
                {
                    var caption = Expression.Term("アビリティ", SelectedUnit) + "一覧";
                    var unitAbilities = currentUnit.Abilities.Where(x => !x.Data.IsItem()).ToList();
                    unitCommands.Add(new UiCommand(AbilityListCmdID, caption));
                }

                // 味方じゃない、ステータス表示、ViewMode
                if (currentUnit.Party != "味方" || currentUnit.IsConditionSatisfied("非操作") || ViewMode)
                {
                    // 召喚ユニットは命令コマンドを使用可能
                    if (currentUnit.Party == "ＮＰＣ"
                        && currentUnit.IsFeatureAvailable("召喚ユニット")
                        && !currentUnit.IsConditionSatisfied("魅了")
                        && !currentUnit.IsConditionSatisfied("混乱")
                        && !currentUnit.IsConditionSatisfied("恐怖")
                        && !currentUnit.IsConditionSatisfied("暴走")
                        && !currentUnit.IsConditionSatisfied("狂戦士")
                        && !ViewMode)
                    {
                        if (currentUnit.Summoner is object)
                        {
                            if (currentUnit.Summoner.Party == "味方")
                            {
                                unitCommands.Add(new UiCommand(OrderCmdID, "命令"));
                            }
                        }
                    }

                    // 魅了したユニットに対しても命令コマンドを使用可能
                    if (currentUnit.Party == "ＮＰＣ"
                        && currentUnit.IsConditionSatisfied("魅了")
                        && !currentUnit.IsConditionSatisfied("混乱")
                        && !currentUnit.IsConditionSatisfied("恐怖")
                        && !currentUnit.IsConditionSatisfied("暴走")
                        && !currentUnit.IsConditionSatisfied("狂戦士")
                        && !ViewMode)
                    {
                        if (currentUnit.Master is object)
                        {
                            if (currentUnit.Master.Party == "味方")
                            {
                                if (!unitCommands.Any(x => x.Id == OrderCmdID))
                                {
                                    unitCommands.Add(new UiCommand(OrderCmdID, "命令"));
                                }
                            }
                        }
                    }

                    // ダミーユニットの場合はコマンド一覧を表示しない
                    if (currentUnit.IsFeatureAvailable("ダミーユニット"))
                    {
                        // 特殊能力一覧
                        if (!unitCommands.Any(x => x.Id == FeatureListCmdID))
                        {
                            UnitCommand(unitCommands.First(x => x.Id == FeatureListCmdID));
                        }
                        else
                        {
                            CommandState = "ユニット選択";
                        }

                        GUI.IsGUILocked = false;
                        return;
                    }

                    if (!string.IsNullOrEmpty(Map.MapFileName))
                    {
                        unitCommands.Add(new UiCommand(MoveCmdID, "移動範囲"));
                        if (currentUnit.Weapons.Any(uw => uw.IsWeaponAvailable("") && !uw.IsWeaponClassifiedAs("Ｍ")))
                        {
                            unitCommands.Add(new UiCommand(AttackCmdID, "射程範囲"));
                        }
                    }

                    // ユニットステータスコマンド用
                    if (Map.IsStatusView)
                    {
                        // 変形コマンド
                        if (currentUnit.IsFeatureAvailable("変形"))
                        {
                            var caption = currentUnit.FeatureName("変形");
                            var fd = currentUnit.Feature("変形");
                            if (fd.DataL.Skip(1).Any(uname => currentUnit.OtherForm(uname).IsAvailable()))
                            {
                                unitCommands.Add(new UiCommand(TransformCmdID, caption));
                            }
                        }

                        // 分離コマンド
                        if (currentUnit.IsFeatureAvailable("分離"))
                        {
                            var caption = currentUnit.FeatureName("分離");
                            var fd = currentUnit.Feature("分離");
                            // 分離形態が利用出来ない場合は分離を行わない
                            var isSplitUnitAvailable = fd.DataL.Skip(1).All(uname => SRC.UList.IsDefined(uname));
                            // パイロットが足らない場合も分離を行わない
                            var isSplitPilotAvailable = isSplitUnitAvailable && fd.DataL.Skip(1).Select(uname => SRC.UList.Item(uname))
                                .Where(u => !u.Data.IsFeatureAvailable("召喚ユニット"))
                                .Sum(u => Math.Abs(u.Data.PilotNum)) >= currentUnit.CountPilot();
                            if (isSplitUnitAvailable && isSplitPilotAvailable)
                            {
                                unitCommands.Add(new UiCommand(SplitCmdID, caption));
                            }
                        }

                        if (currentUnit.IsFeatureAvailable("パーツ分離"))
                        {
                            var caption = currentUnit.FeatureName("パーツ分離");
                            unitCommands.Add(new UiCommand(SplitCmdID, caption));
                        }

                        // 合体コマンド
                        if (currentUnit.IsFeatureAvailable("合体"))
                        {
                            foreach (var fd in currentUnit.Features.Where(fd => fd.Name == "合体"))
                            {
                                var caption = fd.DataL.First();
                                // パートナーが存在しているか？
                                var isPartnerUnitAvailable = fd.DataL.Skip(2).Select(uname => SRC.UList.Item(uname))
                                        .All(u => !(u.Status != "出撃" && u.CurrentForm().IsFeatureAvailable("合体制限")));
                                // 合体先のユニットが作成されているか？
                                var isCombineUnitExist = SRC.UList.IsDefined(fd.DataL.Skip(1).First());
                                if (isPartnerUnitAvailable && isCombineUnitExist)
                                {
                                    unitCommands.Add(new UiCommand(CombineCmdID, caption == "非表示" ? "合体" : caption));
                                }
                            }
                        }
                        else if (currentUnit.IsFeatureAvailable("パーツ合体"))
                        {
                            unitCommands.Add(new UiCommand(CombineCmdID, "パーツ合体"));
                        }

                        if (!currentUnit.IsConditionSatisfied("ノーマルモード付加"))
                        {
                            // ハイパーモードコマンド
                            if (currentUnit.IsFeatureAvailable("ハイパーモード"))
                            {
                                var fd = currentUnit.Feature("ハイパーモード");

                                if (currentUnit.OtherForm(fd.DataL.Skip(1).First()).IsAvailable())
                                {
                                    var caption = fd.DataL.First();
                                    unitCommands.Add(new UiCommand(HyperModeCmdID, caption == "非表示" ? "ハイパーモード" : caption));
                                }
                            }
                            else if (currentUnit.IsFeatureAvailable("ノーマルモード"))
                            {
                                var fd = currentUnit.Feature("ノーマルモード");
                                var uname = fd.DataL.First();

                                if (currentUnit.OtherForm(uname).IsAvailable()
                                    && GeneralLib.LIndex(currentUnit.FeatureData("変形"), 2) != uname)
                                {
                                    var caption = fd.DataL.First();
                                    unitCommands.Add(new UiCommand(HyperModeCmdID, "ノーマルモード"));
                                }
                            }
                        }
                        else
                        {
                            // 変身解除
                            if (Strings.InStr(currentUnit.FeatureData("ノーマルモード"), "手動解除") > 0)
                            {
                                string caption;
                                if (currentUnit.IsFeatureAvailable("変身解除コマンド名"))
                                {
                                    caption = currentUnit.FeatureData("変身解除コマンド名");
                                }
                                else if (currentUnit.IsHero())
                                {
                                    caption = "変身解除";
                                }
                                else
                                {
                                    caption = "特殊モード解除";
                                }
                                unitCommands.Add(new UiCommand(HyperModeCmdID, caption));
                            }
                        }

                        // 換装コマンド
                        if (currentUnit.IsFeatureAvailable("換装"))
                        {
                            // エリアスで換装の名称が変更されている？
                            var caption = SRC.ALDList.RefName("換装");
                            if (currentUnit.Feature("換装").DataL.Any(uname => currentUnit.OtherForm(uname).IsAvailable()))
                            {
                                unitCommands.Add(new UiCommand(ExchangeFormCmdID, caption));
                            }
                        }
                    }

                    // ユニットコマンド
                    if (!ViewMode)
                    {
                        AddUserUnitCommand(unitCommands);
                    }

                    // 未確認ユニットの場合は情報を隠蔽
                    if (Expression.IsOptionDefined("ユニット情報隠蔽") && !currentUnit.IsConditionSatisfied("識別済み") && (currentUnit.Party0 == "敵" || currentUnit.Party0 == "中立") || currentUnit.IsConditionSatisfied("ユニット情報隠蔽"))
                    {
                        unitCommands.RemoveItem(uc => !new int[] {
                            MoveCmdID,
                            AttackCmdID,
                            FeatureListCmdID,
                            WeaponListCmdID,
                            AbilityListCmdID,
                        }.Contains(uc.Id));

                        if (!unitCommands.Any())
                        {
                            // 表示可能なコマンドがなかった
                            CommandState = "ユニット選択";
                            GUI.IsGUILocked = false;
                            return;
                        }
                    }

                    GUI.IsGUILocked = false;
                    GUI.ShowUnitCommandMenu(unitCommands.OrderBy(x => x.Id).ToList());
                    // XXX ユニットコマンドの初期選択？
                    //if (by_cancel)
                    //{
                    //    GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY + 5f);
                    //}
                    //else
                    //{
                    //    GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY - 6f);
                    //}
                    return;
                }

                // 行動終了している場合
                // TODO 行動終了している場合でも行動できたほうが便利なので
                //if (currentUnit.Action == 0)
                //{
                //    // 発進コマンドは使用可能
                //    if (currentUnit.IsFeatureAvailable("母艦"))
                //    {
                //        if (currentUnit.Area != "地中")
                //        {
                //            if (currentUnit.CountUnitOnBoard() > 0)
                //            {
                //                unitCommands.Add(new UiCommand(LaunchCmdID, "発進"));
                //            }
                //        }
                //    }

                //    // ユニットコマンド
                //    AddUserUnitCommand(unitCommands);
                //    GUI.IsGUILocked = false;
                //    GUI.ShowUnitCommandMenu(unitCommands.OrderBy(x => x.Id).ToList());
                //    return;
                //}
            }
            {
                // 移動コマンド
                if (currentUnit.Speed <= 0)
                {
                    unitCommands.Add(new UiCommand(WaitCmdID, "待機"));
                }
                else
                {
                    unitCommands.Add(new UiCommand(MoveCmdID, "移動"));
                }
                // 移動
                {
                    // テレポートコマンド
                    if (currentUnit.IsFeatureAvailable("テレポート"))
                    {
                        int enCost = 40;
                        if (GeneralLib.LLength(currentUnit.FeatureData("テレポート")) == 2)
                        {
                            enCost = Conversions.ToInteger(GeneralLib.LIndex(currentUnit.FeatureData("テレポート"), 2));
                        }
                        if (currentUnit.EN >= enCost)
                        {
                            if (Strings.Len(currentUnit.FeatureData("テレポート")) > 0)
                            {
                                unitCommands.Add(new UiCommand(TeleportCmdID, GeneralLib.LIndex(currentUnit.FeatureData("テレポート"), 1)));
                            }
                            else
                            {
                                unitCommands.Add(new UiCommand(TeleportCmdID, "テレポート"));
                            }
                        }
                        // 通常移動がテレポートの場合
                        if (currentUnit.Speed0 == 0 || currentUnit.FeatureLevel("テレポート") >= 0d && enCost == 0)
                        {
                            unitCommands.RemoveItem(x => x.Id == MoveCmdID);
                        }
                    }

                    // ジャンプコマンド
                    if (currentUnit.IsFeatureAvailable("ジャンプ") && currentUnit.Area != "空中" && currentUnit.Area != "宇宙")
                    {
                        int enCost = 0;
                        if (GeneralLib.LLength(currentUnit.FeatureData("ジャンプ")) == 2)
                        {
                            enCost = Conversions.ToInteger(GeneralLib.LIndex(currentUnit.FeatureData("ジャンプ"), 2));
                        }
                        if (currentUnit.EN >= enCost)
                        {
                            if (Strings.Len(currentUnit.FeatureData("ジャンプ")) > 0)
                            {
                                unitCommands.Add(new UiCommand(JumpCmdID, GeneralLib.LIndex(currentUnit.FeatureData("ジャンプ"), 1)));
                            }
                            else
                            {
                                unitCommands.Add(new UiCommand(JumpCmdID, "ジャンプ"));
                            }
                        }
                        // 通常移動がジャンプの場合
                        if (currentUnit.Speed0 == 0 || currentUnit.FeatureLevel("ジャンプ") >= 0d && enCost == 0)
                        {
                            unitCommands.RemoveItem(x => x.Id == MoveCmdID);
                        }
                    }
                    if (currentUnit.IsConditionSatisfied("移動不能"))
                    {
                        unitCommands.RemoveItem(x => x.Id == MoveCmdID || x.Id == TeleportCmdID || x.Id == JumpCmdID);
                    }

                    // 会話コマンド
                    if (Map.AdjacentUnit(currentUnit)
                        .Any(u => Event.IsEventDefined("会話 " + currentUnit.MainPilot().ID + " " + u.MainPilot().ID)))
                    {
                        unitCommands.Add(new UiCommand(TalkCmdID, "会話"));
                    }

                    // 攻撃コマンド
                    if (currentUnit.Weapons.Any(x => x.IsWeaponUseful("移動前")))
                    {
                        unitCommands.Add(new UiCommand(AttackCmdID, "攻撃"));
                    }

                    if (currentUnit.Area == "地中")
                    {
                        unitCommands.RemoveItem(x => x.Id == AttackCmdID);
                    }

                    if (currentUnit.IsConditionSatisfied("攻撃不能"))
                    {
                        unitCommands.RemoveItem(x => x.Id == AttackCmdID);
                    }

                    AddFixSupplyCommand(unitCommands, currentUnit, false);

                    // アビリティコマンド
                    if (currentUnit.Area != "地中")
                    {
                        var displayAbilities = currentUnit.Abilities.Where(x => x.IsAbilityMastered()
                          && !x.Data.IsItem()
                          && x.IsAbilityUseful("移動前")
                          ).ToList();
                        if (displayAbilities.Count > 0)
                        {
                            var caption = Expression.Term("アビリティ", SelectedUnit);
                            var unitAbilities = currentUnit.Abilities.Where(x => !x.Data.IsItem()).ToList();
                            unitCommands.Add(new UiCommand(
                                AbilityCmdID,
                                unitAbilities.Count == 1 ? unitAbilities.First().AbilityNickname() : caption));
                        }
                    }

                    // チャージコマンド
                    if (!currentUnit.IsConditionSatisfied("チャージ完了"))
                    {
                        if (currentUnit.Weapons.Any(uw => uw.IsWeaponClassifiedAs("Ｃ") && uw.IsWeaponAvailable("チャージ"))
                             || currentUnit.Abilities.Any(ua => ua.IsAbilityClassifiedAs("Ｃ") && ua.IsAbilityAvailable("チャージ")))
                        {
                            unitCommands.Add(new UiCommand(ChargeCmdID, Expression.Term("チャージ", SelectedUnit)));
                        }
                    }

                    // スペシャルパワーコマンド
                    {
                        if (!currentUnit.IsConditionSatisfied("憑依") && !currentUnit.IsConditionSatisfied("スペシャルパワー使用不能"))
                        {
                            if (currentUnit.PilotsHaveSpecialPower().Count > 0)
                            {
                                unitCommands.Add(new UiCommand(SpecialPowerCmdID, Expression.Term("スペシャルパワー", SelectedUnit)));
                            }
                        }
                    }

                    // 変形コマンド
                    if (currentUnit.IsFeatureAvailable("変形")
                        && !string.IsNullOrEmpty(currentUnit.FeatureName("変形"))
                        && !currentUnit.IsConditionSatisfied("形態固定")
                        && !currentUnit.IsConditionSatisfied("機体固定"))
                    {
                        var cmdName = currentUnit.FeatureName("変形");
                        foreach (var uname in GeneralLib.ToL(currentUnit.FeatureData("変形")).Skip(1))
                        {
                            if (currentUnit.OtherForm(uname)?.IsAvailable() ?? false)
                            {
                                unitCommands.Add(new UiCommand(TransformCmdID, cmdName));
                                break;
                            }
                        }
                    }

                    // 分離コマンド
                    if (currentUnit.IsFeatureAvailable("分離")
                        && !string.IsNullOrEmpty(currentUnit.FeatureName("分離"))
                        && !currentUnit.IsConditionSatisfied("形態固定")
                        && !currentUnit.IsConditionSatisfied("機体固定"))
                    {
                        var splitForms = GeneralLib.ToL(currentUnit.FeatureData("分離")).Skip(1).ToList();

                        // 分離形態が利用出来ない場合は分離を行わない
                        // パイロットが足らない場合も分離を行わない
                        if (splitForms.All(x => SRC.UList.IsDefined(x))
                            && currentUnit.CountPilot() >= splitForms.Select(x => SRC.UList.Item(x))
                                .Where(x => !x.IsFeatureAvailable("召喚ユニット"))
                                .Count())
                        {
                            unitCommands.Add(new UiCommand(SplitCmdID, currentUnit.FeatureName("分離")));
                        }
                    }

                    if (currentUnit.IsFeatureAvailable("パーツ分離")
                        && !string.IsNullOrEmpty(currentUnit.FeatureName("パーツ分離")))
                    {
                        unitCommands.Add(new UiCommand(SplitCmdID, currentUnit.FeatureName("パーツ分離")));
                    }

                    // 合体コマンド
                    if (currentUnit.IsFeatureAvailable("合体")
                        && !currentUnit.IsConditionSatisfied("形態固定")
                        && !currentUnit.IsConditionSatisfied("機体固定"))
                    {
                        var combines = currentUnit.CombineFeatures(SRC);
                        foreach (var fd in combines)
                        {
                            unitCommands.Add(new UiCommand(CombineCmdID, fd.CombineName));
                        }
                    }

                    if (!currentUnit.IsConditionSatisfied("ノーマルモード付加"))
                    {
                        // ハイパーモードコマンド
                        if (currentUnit.IsFeatureAvailable("ハイパーモード")
                            && (currentUnit.MainPilot().Morale >= (10d * currentUnit.FeatureLevel("ハイパーモード")) + 100
                                || currentUnit.HP <= currentUnit.MaxHP / 4 && Strings.InStr(currentUnit.FeatureData("ハイパーモード"), "気力発動") == 0)
                            && Strings.InStr(currentUnit.FeatureData("ハイパーモード"), "自動発動") == 0
                            && !string.IsNullOrEmpty(currentUnit.FeatureName("ハイパーモード"))
                            && !currentUnit.IsConditionSatisfied("形態固定")
                            && !currentUnit.IsConditionSatisfied("機体固定"))
                        {
                            var fd = currentUnit.Feature("ハイパーモード");

                            if (!currentUnit.IsConditionSatisfied("行動不能")
                                && currentUnit.OtherForm(fd.DataL.Skip(1).First()).IsAbleToEnter(currentUnit.x, currentUnit.y))
                            {
                                var caption = fd.DataL.First();
                                unitCommands.Add(new UiCommand(HyperModeCmdID, caption == "非表示" ? "ハイパーモード" : caption));
                            }
                        }
                    }
                    else
                    {
                        // 変身解除
                        if (Strings.InStr(currentUnit.FeatureData("ノーマルモード"), "手動解除") > 0)
                        {
                            string caption;
                            if (currentUnit.IsFeatureAvailable("変身解除コマンド名"))
                            {
                                caption = currentUnit.FeatureData("変身解除コマンド名");
                            }
                            else if (currentUnit.IsHero())
                            {
                                caption = "変身解除";
                            }
                            else
                            {
                                caption = "特殊モード解除";
                            }
                            unitCommands.Add(new UiCommand(HyperModeCmdID, caption));
                        }
                    }

                    // 地上コマンド
                    if (Map.Terrain(currentUnit.x, currentUnit.y).Class == "陸"
                        || Map.Terrain(currentUnit.x, currentUnit.y).Class == "屋内"
                        || Map.Terrain(currentUnit.x, currentUnit.y).Class == "月面")
                    {
                        if (currentUnit.Area != "地上" && currentUnit.IsTransAvailable("陸"))
                        {
                            unitCommands.Add(new UiCommand(GroundCmdID, "地上"));
                        }
                    }
                    else if (Map.Terrain(currentUnit.x, currentUnit.y).Class == "水"
                        || Map.Terrain(currentUnit.x, currentUnit.y).Class == "深水")
                    {
                        if (currentUnit.Area != "水上" && currentUnit.IsTransAvailable("水上"))
                        {
                            unitCommands.Add(new UiCommand(GroundCmdID, "水上"));
                        }
                    }

                    // 空中コマンド
                    switch (Map.Terrain(currentUnit.x, currentUnit.y).Class ?? "")
                    {
                        case "宇宙":
                            break;

                        case "月面":
                            if ((currentUnit.IsTransAvailable("空") || currentUnit.IsTransAvailable("宇宙")) && !(currentUnit.Area == "宇宙"))
                            {
                                unitCommands.Add(new UiCommand(SkyCmdID, "宇宙"));
                            }
                            break;

                        default:
                            if (currentUnit.IsTransAvailable("空") && !(currentUnit.Area == "空中"))
                            {
                                unitCommands.Add(new UiCommand(SkyCmdID, "空中"));
                            }
                            break;
                    }

                    // 地中コマンド
                    if (currentUnit.IsTransAvailable("地中")
                        && !(currentUnit.Area == "地中")
                        && (Map.Terrain(currentUnit.x, currentUnit.y).Class == "陸" || Map.Terrain(currentUnit.x, currentUnit.y).Class == "月面"))
                    {
                        unitCommands.Add(new UiCommand(UndergroundCmdID, "地中"));
                    }

                    // 水中コマンド
                    if (currentUnit.Area != "水中")
                    {
                        if (Map.Terrain(currentUnit.x, currentUnit.y).Class == "深水"
                            && (currentUnit.IsTransAvailable("水") || currentUnit.IsFeatureAvailable("水泳"))
                            && Strings.Mid(currentUnit.Data.Adaption, 3, 1) != "-")
                        {
                            unitCommands.Add(new UiCommand(WaterCmdID, "水中"));
                        }
                        else if (Map.Terrain(currentUnit.x, currentUnit.y).Class == "水" && Strings.Mid(currentUnit.Data.Adaption, 3, 1) != "-")
                        {
                            unitCommands.Add(new UiCommand(WaterCmdID, "水中"));
                        }
                    }

                    // 発進コマンド
                    if (currentUnit.IsFeatureAvailable("母艦") && currentUnit.Area != "地中")
                    {
                        if (currentUnit.CountUnitOnBoard() > 0)
                        {
                            unitCommands.Add(new UiCommand(LaunchCmdID, "発進"));
                        }
                    }

                    // アイテムコマンド
                    if (currentUnit.Abilities.Any(ua => ua.IsAbilityUseful("移動前") && ua.Data.IsItem()))
                    {
                        unitCommands.Add(new UiCommand(ItemCmdID, "アイテム"));
                    }
                    if (currentUnit.Area == "地中")
                    {
                        unitCommands.RemoveItem(x => x.Id == ItemCmdID);
                    }

                    // 召喚解除コマンド
                    {
                        var isDismissAvailable = false;
                        foreach (var u in currentUnit.Servants)
                        {

                            var cf = u.CurrentForm();
                            switch (cf.Status ?? "")
                            {
                                case "出撃":
                                case "格納":
                                    isDismissAvailable = true;
                                    break;

                                case "旧主形態":
                                case "旧形態":
                                    // 合体後の形態が出撃中なら使用不可
                                    if (!cf.CombineFeatures(SRC)
                                        .Select(x => SRC.UList.Item(x.ConbineUnitName))
                                        .Where(u => u != null)
                                        .Any(u => u.Status == "出撃" || u.Status == "格納"))
                                    {
                                        isDismissAvailable = true;
                                    }
                                    break;
                            }
                        }
                        if (isDismissAvailable)
                        {
                            var caption = "召喚解除";
                            if (currentUnit.IsFeatureAvailable("召喚解除コマンド名"))
                            {
                                caption = currentUnit.FeatureData("召喚解除コマンド名");
                            }
                            unitCommands.Add(new UiCommand(DismissCmdID, caption));
                        }
                    }

                    // ユニットコマンド
                    AddUserUnitCommand(unitCommands);
                }
            }

            //if (!ReferenceEquals(SelectedUnit, Status.DisplayedUnit))
            //{
            //    // MOD START 240a
            //    // DisplayUnitStatus SelectedUnit
            //    // 新ＧＵＩ使用時はクリック時にユニットステータスを表示しない
            //    if (!GUI.NewGUIMode)
            //    {
            //        Status.DisplayUnitStatus(SelectedUnit);
            //    }
            //    // MOD  END  240a
            //}

            GUI.IsGUILocked = false;
            GUI.ShowUnitCommandMenu(unitCommands.OrderBy(x => x.Id).ToList());
            //if (by_cancel)
            //{
            //    GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY + 5f);
            //}
            //else
            //{
            //    GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY - 6f);
            //}
        }

        private void AddFixSupplyCommand(List<UiCommand> unitCommands, Unit currentUnit, bool isMoved)
        {
            // 修理コマンド
            if (currentUnit.IsFeatureAvailable("修理装置") && currentUnit.Area != "地中")
            {
                if (Map.AdjacentUnit(currentUnit)
                    .Any(u => (u.Party == "味方" || u.Party == "ＮＰＣ")
                        && u.CanFix(currentUnit)))
                {
                    var fd = currentUnit.Feature("修理装置");
                    if (!string.IsNullOrEmpty(fd.Data))
                    {
                        var caption = fd.DataL.First();
                        var en = fd.DataL.SafeRefOneOffset(2);

                        if (!(Information.IsNumeric(en) && currentUnit.EN < Conversions.ToInteger(en)))
                        {
                            unitCommands.Add(new UiCommand(FixCmdID, caption));
                        }
                    }
                    else
                    {
                        unitCommands.Add(new UiCommand(FixCmdID, "修理装置"));
                    }
                }
            }

            // 補給コマンド
            if (!isMoved || !Expression.IsOptionDefined("移動後補給不可") || currentUnit.MainPilot().IsSkillAvailable("補給"))
            {
                if (currentUnit.IsFeatureAvailable("補給装置") && currentUnit.Area != "地中")
                {
                    if (Map.AdjacentUnit(currentUnit)
                        .Any(u => (u.Party == "味方" || u.Party == "ＮＰＣ")
                            && u.CanSupply))
                    {
                        var fd = currentUnit.Feature("補給装置");
                        if (!string.IsNullOrEmpty(fd.Data))
                        {
                            var caption = fd.DataL.First();
                            var en = fd.DataL.SafeRefOneOffset(2);

                            if (!(Information.IsNumeric(en) && (currentUnit.EN < Conversions.ToInteger(en) || currentUnit.MainPilot().Morale < 100)))
                            {
                                unitCommands.Add(new UiCommand(SupplyCmdID, caption));
                            }
                        }
                        else
                        {
                            unitCommands.Add(new UiCommand(SupplyCmdID, "補給装置"));
                        }
                    }
                }
            }
        }

        private void AddUserUnitCommand(List<UiCommand> unitCommands)
        {
            foreach (LabelData lab in Event.colEventLabelList.Values
                .Where(x => x.Name == LabelType.UnitCommandEventLabel && x.Enable))
            {
                var label = lab.Para(2);
                var target = lab.Para(3);
                if (SelectedUnit.Party == "味方" && (
                        (target ?? "") == (SelectedUnit.MainPilot().Name ?? "")
                        || (target ?? "") == (SelectedUnit.MainPilot().get_Nickname(false) ?? "")
                        || (target ?? "") == (SelectedUnit.Name ?? "")
                    )
                    || (target ?? "") == (SelectedUnit.Party ?? "")
                    || target == "全")
                {
                    if (lab.CountPara() <= 3)
                    {
                        // 無条件で実行できるコマンド
                        unitCommands.Add(new UiCommand(UnitCommandCmdID, lab.Para(2), lab));
                    }
                    else if (GeneralLib.StrToLng(lab.Para(4)) != 0)
                    {
                        // 条件を満たした場合のみ実行できるコマンド
                        unitCommands.Add(new UiCommand(UnitCommandCmdID, lab.Para(2), lab));
                    }
                }
            }
        }

        private void ProceedAfterMoveCommandSelect(
            bool by_cancel = false,
            GuiButton button = GuiButton.None,
            MapCell cell = null,
            Unit unit = null)
        {
            LogDebug();

            Event.SelectedUnitForEvent = SelectedUnit;
            var unitCommands = new List<UiCommand>();

            {
                var currentUnit = SelectedUnit;
                //// 移動時にＥＮを消費している場合はステータスウィンドウを更新
                //// MOD START MARGE
                //// If MainWidth = 15 Then
                //if (!GUI.NewGUIMode)
                //{
                //    // MOD END MARGE
                //    if (PrevUnitEN != currentUnit.EN)
                //    {
                //        Status.DisplayUnitStatus(SelectedUnit);
                //    }
                //}

                unitCommands.Add(new UiCommand(WaitCmdID, "待機"));

                // 会話コマンド
                if (Map.AdjacentUnit(currentUnit)
                    .Any(u => Event.IsEventDefined("会話 " + currentUnit.MainPilot().ID + " " + u.MainPilot().ID)))
                {
                    unitCommands.Add(new UiCommand(TalkCmdID, "会話"));
                }

                // 攻撃コマンド
                if (currentUnit.Weapons.Any(x => x.IsWeaponUseful("移動後")))
                {
                    unitCommands.Add(new UiCommand(AttackCmdID, "攻撃"));
                }

                if (currentUnit.Area == "地中")
                {
                    unitCommands.RemoveItem(x => x.Id == AttackCmdID);
                }

                if (currentUnit.IsConditionSatisfied("攻撃不能"))
                {
                    unitCommands.RemoveItem(x => x.Id == AttackCmdID);
                }

                AddFixSupplyCommand(unitCommands, currentUnit, true);

                // アビリティコマンド
                if (currentUnit.Area != "地中")
                {
                    var displayAbilities = currentUnit.Abilities.Where(x => x.IsAbilityMastered()
                      && !x.Data.IsItem()
                      && x.IsAbilityUseful("移動後")
                      ).ToList();
                    if (displayAbilities.Count > 0)
                    {
                        var caption = Expression.Term("アビリティ", SelectedUnit);
                        var unitAbilities = currentUnit.Abilities.Where(x => !x.Data.IsItem()).ToList();
                        unitCommands.Add(new UiCommand(
                            AbilityCmdID,
                            unitAbilities.Count == 1 ? unitAbilities.First().AbilityNickname() : caption));
                    }
                }

                // 合体コマンド
                if (currentUnit.IsFeatureAvailable("合体")
                    && !currentUnit.IsConditionSatisfied("形態固定")
                    && !currentUnit.IsConditionSatisfied("機体固定"))
                {
                    var combines = currentUnit.CombineFeatures(SRC);
                    foreach (var fd in combines)
                    {
                        unitCommands.Add(new UiCommand(CombineCmdID, fd.CombineName));
                    }
                }

                // アイテムコマンド
                if (currentUnit.Abilities.Any(ua => ua.IsAbilityUseful("移動後") && ua.Data.IsItem()))
                {
                    unitCommands.Add(new UiCommand(ItemCmdID, "アイテム"));
                }
                if (currentUnit.Area == "地中")
                {
                    unitCommands.RemoveItem(x => x.Id == ItemCmdID);
                }

                AddUserUnitCommand(unitCommands);
            }

            GUI.IsGUILocked = false;
            GUI.ShowUnitCommandMenu(unitCommands.OrderBy(x => x.Id).ToList());
            //if (by_cancel)
            //{
            //    GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY + 5f);
            //}
            //else
            //{
            //    GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY - 6f);
            //    Application.DoEvents();
            //    // ＰＣに負荷がかかったような状態だとポップアップメニューの選択が
            //    // うまく行えない場合があるのでやり直す
            //    while (CommandState == "移動後コマンド選択" && SelectedCommand == "移動")
            //    {
            //        GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY - 6f);
            //        Application.DoEvents();
            //    }
            //}
        }

        private void ProceedTargetSelect(
            bool by_cancel = false,
            GuiButton button = GuiButton.None,
            MapCell cell = null,
            Unit unit = null)
        {
            LogDebug();

            if (cell == null)
            {
                return;
            }
            if (!Map.MaskData[cell.X, cell.Y])
            {
                SelectedX = cell.X;
                SelectedY = cell.Y;

                // 自分自身を選択された場合
                if (SelectedUnit.x == SelectedX && SelectedUnit.y == SelectedY)
                {
                    if (SelectedCommand == "スペシャルパワー")
                    {
                    }
                    // 下に抜ける
                    else if (SelectedCommand == "アビリティ"
                        || SelectedCommand == "マップアビリティ"
                        || SelectedCommand == "アイテム"
                        || SelectedCommand == "マップアイテム")
                    {
                        if (SelectedUnit.Ability(SelectedAbility).AbilityMinRange() > 0)
                        {
                            // 自分自身は選択不可
                            GUI.IsGUILocked = false;
                            return;
                        }
                    }
                    else if (SelectedCommand == "移動命令")
                    {
                    }
                    // 下に抜ける
                    else
                    {
                        // 自分自身は選択不可
                        GUI.IsGUILocked = false;
                        return;
                    }
                }

                // 場所を選択するコマンド
                switch (SelectedCommand ?? "")
                {
                    case "移動":
                    case "再移動":
                        FinishMoveCommand();
                        GUI.IsGUILocked = false;
                        return;

                    case "テレポート":
                        FinishTeleportCommand();
                        GUI.IsGUILocked = false;
                        return;

                    case "ジャンプ":
                        FinishJumpCommand();
                        GUI.IsGUILocked = false;
                        return;

                    case "マップ攻撃":
                        MapAttackCommand();
                        GUI.IsGUILocked = false;
                        return;

                    case "マップアビリティ":
                    case "マップアイテム":
                        MapAbilityCommand();
                        GUI.IsGUILocked = false;
                        return;

                    case "発進":
                        FinishLaunchCommand();
                        GUI.IsGUILocked = false;
                        return;

                    case "移動命令":
                        FinishOrderCommand();
                        GUI.IsGUILocked = false;
                        return;
                }

                // これ以降はユニットを選択するコマンド

                // 指定した地点にユニットがいる？
                if (Map.MapDataForUnit[SelectedX, SelectedY] is null)
                {
                    GUI.IsGUILocked = false;
                    return;
                }

                // ターゲットを選択
                SelectedTarget = Map.MapDataForUnit[SelectedX, SelectedY];
                switch (SelectedCommand ?? "")
                {
                    case "攻撃":
                        FinishAttackCommand();
                        break;

                    case "アビリティ":
                    case "アイテム":
                        FinishAbilityCommand();
                        break;

                    case "会話":
                        FinishTalkCommand();
                        break;

                    case "修理":
                        FinishFixCommand();
                        break;

                    case "補給":
                        FinishSupplyCommand();
                        break;

                    case "スペシャルパワー":
                        FinishSpecialPowerCommand();
                        break;

                    case "攻撃命令":
                    case "護衛命令":
                        FinishOrderCommand();
                        break;
                }
            }
        }

        private void ProceedMapAttack(
            bool by_cancel = false,
            GuiButton button = GuiButton.None,
            MapCell cell = null,
            Unit unit = null)
        {
            LogDebug();

            if (1 <= GUI.PixelToMapX((int)GUI.MouseX) && GUI.PixelToMapX((int)GUI.MouseX) <= Map.MapWidth)
            {
                if (1 <= GUI.PixelToMapY((int)GUI.MouseY) && GUI.PixelToMapY((int)GUI.MouseY) <= Map.MapHeight)
                {
                    if (!Map.MaskData[GUI.PixelToMapX((int)GUI.MouseX), GUI.PixelToMapY((int)GUI.MouseY)])
                    {
                        // 効果範囲内でクリックされればマップ攻撃発動
                        if (SelectedCommand == "マップ攻撃")
                        {
                            MapAttackCommand();
                        }
                        else
                        {
                            MapAbilityCommand();
                        }
                    }
                }
            }
        }

        // ＧＵＩの処理をキャンセル
        public void CancelCommand()
        {
            LogDebug();

            var currentUnit = SelectedUnit;
            switch (CommandState ?? "")
            {
                case "ユニット選択":
                    break;

                case "コマンド選択":
                    CommandState = "ユニット選択";
                    // 選択したコマンドを初期化
                    SelectedCommand = "";
                    //// MOD START MARGE
                    //// If MainWidth <> 15 Then
                    //if (GUI.NewGUIMode)
                    //{
                    //    // MOD  END  MARGE
                    //    Status.ClearUnitStatus();
                    //}

                    break;

                case "ターゲット選択":
                    if (SelectedCommand == "再移動")
                    {
                        WaitCommand();
                        return;
                    }
                    CommandState = "コマンド選択";
                    Status.DisplayUnitStatus(SelectedUnit);
                    GUI.RedrawScreen();
                    ProceedCommand(true);
                    break;

                case "移動後コマンド選択":
                    CommandState = "ターゲット選択";
                    currentUnit.Area = PrevUnitArea;
                    currentUnit.Move(PrevUnitX, PrevUnitY, true, true);
                    currentUnit.EN = PrevUnitEN;
                    if (!ReferenceEquals(SelectedUnit, Map.MapDataForUnit[PrevUnitX, PrevUnitY]))
                    {
                        // 発進をキャンセルした場合
                        SelectedTarget = SelectedUnit;
                        GUI.PaintUnitBitmap(SelectedTarget);
                        SelectedUnit = Map.MapDataForUnit[PrevUnitX, PrevUnitY];
                    }
                    //// MOD START MARGE
                    //// ElseIf MainWidth = 15 Then
                    //else if (!GUI.NewGUIMode)
                    //{
                    //    // MOD END MARGE
                    //    Status.DisplayUnitStatus(SelectedUnit);
                    //}
                    //// MOD START MARGE
                    // 移動後コマンドをキャンセルした場合、MoveCostを0にリセットする
                    SelectedUnitMoveCost = 0;
                    switch (SelectedCommand ?? "")
                    {
                        case "移動":
                            StartMoveCommand();
                            break;

                        case "テレポート":
                            StartTeleportCommand();
                            break;

                        case "ジャンプ":
                            StartJumpCommand();
                            break;

                        case "発進":
                            GUI.PaintUnitBitmap(SelectedTarget);
                            break;
                    }

                    break;

                case "移動後ターゲット選択":
                    CommandState = "移動後コマンド選択";
                    Status.DisplayUnitStatus(SelectedUnit);
                    var tmp_x = currentUnit.x;
                    var tmp_y = currentUnit.y;
                    currentUnit.x = PrevUnitX;
                    currentUnit.y = PrevUnitY;
                    switch (PrevCommand ?? "")
                    {
                        case "移動":
                            Map.AreaInSpeed(SelectedUnit);
                            break;

                        case "テレポート":
                            Map.AreaInTeleport(SelectedUnit);
                            break;

                        case "ジャンプ":
                            Map.AreaInSpeed(SelectedUnit, true);
                            break;

                        case "発進":
                            var targetUnit = SelectedTarget;

                            if (targetUnit.IsFeatureAvailable("テレポート")
                                && (targetUnit.Data.Speed == 0
                                || GeneralLib.LIndex(targetUnit.FeatureData("テレポート"), 2) == "0"))
                            {
                                Map.AreaInTeleport(SelectedTarget);
                            }
                            else if (targetUnit.IsFeatureAvailable("ジャンプ")
                                && (targetUnit.Data.Speed == 0
                                || GeneralLib.LLength(targetUnit.FeatureData("ジャンプ")) < 2
                                || GeneralLib.LIndex(targetUnit.FeatureData("ジャンプ"), 2) == "0"))
                            {
                                Map.AreaInSpeed(SelectedTarget, true);
                            }
                            else
                            {
                                Map.AreaInSpeed(SelectedTarget);
                            }
                            break;
                    }

                    currentUnit.x = tmp_x;
                    currentUnit.y = tmp_y;
                    SelectedCommand = PrevCommand;
                    Map.MaskData[tmp_x, tmp_y] = false;
                    GUI.MaskScreen();
                    ProceedCommand(true);
                    break;

                case "マップ攻撃使用":
                case "移動後マップ攻撃使用":
                    if (CommandState == "マップ攻撃使用")
                    {
                        CommandState = "ターゲット選択";
                    }
                    else
                    {
                        CommandState = "移動後ターゲット選択";
                    }

                    if (SelectedCommand == "マップ攻撃")
                    {
                        var uw = currentUnit.Weapon(SelectedWeapon);
                        if (uw.IsWeaponClassifiedAs("Ｍ直"))
                        {
                            Map.AreaInCross(currentUnit.x, currentUnit.y, uw.WeaponMaxRange(), uw.WeaponMinRange());
                        }
                        else if (uw.IsWeaponClassifiedAs("Ｍ移"))
                        {
                            Map.AreaInMoveAction(SelectedUnit, uw.WeaponMaxRange());
                        }
                        else
                        {
                            Map.AreaInRange(currentUnit.x, currentUnit.y, uw.WeaponMaxRange(), uw.WeaponMinRange(), "すべて");
                        }
                    }
                    else
                    {
                        var ua = currentUnit.Ability(SelectedAbility);
                        if (ua.IsAbilityClassifiedAs("Ｍ直"))
                        {
                            Map.AreaInCross(currentUnit.x, currentUnit.y, ua.AbilityMaxRange(), ua.AbilityMinRange());
                        }
                        else if (ua.IsAbilityClassifiedAs("Ｍ移"))
                        {
                            Map.AreaInMoveAction(SelectedUnit, ua.AbilityMaxRange());
                        }
                        else
                        {
                            Map.AreaInRange(currentUnit.x, currentUnit.y, ua.AbilityMaxRange(), ua.AbilityMinRange(), "すべて");
                        }
                    }

                    GUI.MaskScreen();
                    break;
            }
        }
    }
}
