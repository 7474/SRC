using SRCCore.Events;
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Units;
using SRCCore.VB;
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

                    // 全体マップ
                    mapCommands.Add(new UiCommand(GlobalMapCmdID, "全体マップ"));

                    // 作戦目的
                    if (Event.IsEventDefined("勝利条件"))
                    {
                        mapCommands.Add(new UiCommand(OperationObjectCmdID, "勝利条件"));
                    }

                    // 自動反撃モード
                    mapCommands.Add(new UiCommand(AutoDefenseCmdID, "自動反撃モード"));

                    // 設定変更
                    mapCommands.Add(new UiCommand(ConfigurationCmdID, "設定変更"));

                    // リスタート
                    if (SRC.IsRestartSaveDataAvailable & !ViewMode)
                    {
                        mapCommands.Add(new UiCommand(RestartCmdID, "リスタート"));
                    }

                    // クイックロード
                    if (SRC.IsQuickSaveDataAvailable & !ViewMode)
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
                mapCommands.Add(new UiCommand(SearchSpecialPowerCmdID, "スペシャルパワー検索"));
                // TODO 表示名と表示有無の解決
                //Unit argu = null;
                //GUI.MainForm.mnuMapCommandItem(SearchSpecialPowerCmdID).Caption = Expression.Term("スペシャルパワー", u: argu) + "検索";
                //foreach (Pilot p in SRC.PList)
                //{
                //    if (p.Party == "味方")
                //    {
                //        if (p.CountSpecialPower > 0)
                //        {
                //            GUI.MainForm.mnuMapCommandItem(SearchSpecialPowerCmdID).Visible = true;
                //            break;
                //        }
                //    }
                //}

                // イベントで定義されたマップコマンド
                if (!ViewMode)
                {
                    var i = MapCommand1CmdID;
                    var mapCommandLabels = Event.colEventLabelList.Values
                        .Where(x => x.Name == LabelType.MapCommandEventLabel && x.Enable);
                    foreach (LabelData lab in mapCommandLabels)
                    {
                        int localStrToLng()
                        {
                            string argexpr = lab.Para(3);
                            var ret = GeneralLib.StrToLng(argexpr);
                            return ret;
                        }

                        // TODO ラベルの表示名にする
                        if (lab.CountPara() == 2)
                        {
                            // XXX  lab.Para(2)
                            mapCommands.Add(new UiCommand(i, lab.Data));
                        }
                        else if (localStrToLng() != 0)
                        {
                            mapCommands.Add(new UiCommand(i, lab.Data));
                        }
                        // 実行用にインデックス作っとく
                        MapCommandLabelList[i] = lab;
                        // TODO 上限儲けるなら適当に打ち切る
                        //GUI.MainForm.mnuMapCommandItem(i).Caption = lab.Para(2);
                        //MapCommandLabelList[i - MapCommand1CmdID + 1] = lab.LineNum.ToString();
                        //i = (i + 1);
                        //if (i > MapCommand10CmdID)
                        //{
                        //    break;
                        //}
                    }
                }

                CommandState = "マップコマンド";
                GUI.IsGUILocked = false;
                // XXX
                //// ADD START 240a
                //// ここに来た時点でcancel=Trueはユニットのいないセルを右クリックした場合のみ
                //if (by_cancel)
                //{
                //    if (GUI.NewGUIMode & !string.IsNullOrEmpty(Map.MapFileName))
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
                // TODO
                //// ユニット上でキャンセルボタンを押した場合は武器一覧
                //// もしくはアビリティ一覧を表示する
                //{
                //    var withBlock2 = SelectedUnit;
                //    // 情報が隠蔽されている場合は表示しない
                //    string argoname4 = "ユニット情報隠蔽";
                //    object argIndex1 = "識別済み";
                //    object argIndex2 = "ユニット情報隠蔽";
                //    string argfname = "ダミーユニット";
                //    if (Expression.IsOptionDefined(argoname4) & !withBlock2.IsConditionSatisfied(argIndex1) & (withBlock2.Party0 == "敵" | withBlock2.Party0 == "中立") | withBlock2.IsConditionSatisfied(argIndex2) | withBlock2.IsFeatureAvailable(argfname))
                //    {
                //        GUI.IsGUILocked = false;
                //        return;
                //    }

                //    if (withBlock2.CountWeapon() == 0 & withBlock2.CountAbility() > 0)
                //    {
                //        AbilityListCommand();
                //    }
                //    else
                //    {
                //        WeaponListCommand();
                //    }
                //}

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
                //// 特殊能力＆アビリティ一覧はどのユニットでも見れる可能性があるので
                //// 先に判定しておく

                //// 特殊能力一覧コマンド
                //var loopTo = currentUnit.CountAllFeature();
                //for (var i = 1; i <= loopTo; i++)
                //{
                //    string localAllFeature() { object argIndex1 = i; var ret = currentUnit.AllFeature(argIndex1); return ret; }

                //    string localAllFeature1() { object argIndex1 = i; var ret = currentUnit.AllFeature(argIndex1); return ret; }

                //    string localAllFeature2() { object argIndex1 = i; var ret = currentUnit.AllFeature(argIndex1); return ret; }

                //    string localAllFeature3() { object argIndex1 = i; var ret = currentUnit.AllFeature(argIndex1); return ret; }

                //    object argIndex5 = i;
                //    if (!string.IsNullOrEmpty(currentUnit.AllFeatureName(argIndex5)))
                //    {
                //        object argIndex4 = i;
                //        switch (currentUnit.AllFeature(argIndex4) ?? "")
                //        {
                //            case "合体":
                //                {
                //                    string localAllFeatureData() { object argIndex1 = i; var ret = currentUnit.AllFeatureData(argIndex1); return ret; }

                //                    string localLIndex() { string arglist = hsde8149624c274ab08211d9ffa37bf9bf(); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                //                    object argIndex3 = localLIndex();
                //                    if (SRC.UList.IsDefined(argIndex3))
                //                    {
                //                        GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = true;
                //                        break;
                //                    }

                //                    break;
                //                }

                //            default:
                //                {
                //                    GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = true;
                //                    break;
                //                }
                //        }
                //    }
                //    else if (localAllFeature() == "パイロット能力付加" | localAllFeature1() == "パイロット能力強化")
                //    {
                //        string localAllFeatureData1() { object argIndex1 = i; var ret = currentUnit.AllFeatureData(argIndex1); return ret; }

                //        if (Strings.InStr(localAllFeatureData1(), "非表示") == 0)
                //        {
                //            GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = true;
                //            break;
                //        }
                //    }
                //    else if (localAllFeature2() == "武器クラス" | localAllFeature3() == "防具クラス")
                //    {
                //        string argoname5 = "アイテム交換";
                //        if (Expression.IsOptionDefined(argoname5))
                //        {
                //            GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = true;
                //            break;
                //        }
                //    }
                //}

                //{
                //    var withBlock5 = currentUnit.MainPilot();
                //    var loopTo1 = withBlock5.CountSkill();
                //    for (i = 1; i <= loopTo1; i++)
                //    {
                //        string localSkillName0() { object argIndex1 = i; var ret = withBlock5.SkillName0(argIndex1); return ret; }

                //        string localSkillName01() { object argIndex1 = i; var ret = withBlock5.SkillName0(argIndex1); return ret; }

                //        if (localSkillName0() != "非表示" & !string.IsNullOrEmpty(localSkillName01()))
                //        {
                //            object argIndex6 = i;
                //            switch (withBlock5.Skill(argIndex6) ?? "")
                //            {
                //                case "耐久":
                //                    {
                //                        string argoname6 = "防御力成長";
                //                        string argoname7 = "防御力レベルアップ";
                //                        if (!Expression.IsOptionDefined(argoname6) & !Expression.IsOptionDefined(argoname7))
                //                        {
                //                            GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = true;
                //                            break;
                //                        }

                //                        break;
                //                    }

                //                case "追加レベル":
                //                case "格闘ＵＰ":
                //                case "射撃ＵＰ":
                //                case "命中ＵＰ":
                //                case "回避ＵＰ":
                //                case "技量ＵＰ":
                //                case "反応ＵＰ":
                //                case "ＳＰＵＰ":
                //                case "格闘ＤＯＷＮ":
                //                case "射撃ＤＯＷＮ":
                //                case "命中ＤＯＷＮ":
                //                case "回避ＤＯＷＮ":
                //                case "技量ＤＯＷＮ":
                //                case "反応ＤＯＷＮ":
                //                case "ＳＰＤＯＷＮ":
                //                case "メッセージ":
                //                case "魔力所有":
                //                    {
                //                        break;
                //                    }

                //                default:
                //                    {
                //                        GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = true;
                //                        break;
                //                    }
                //            }
                //        }
                //    }
                //}

                //// アビリティ一覧コマンド
                //var loopTo2 = currentUnit.CountAbility();
                //for (i = 1; i <= loopTo2; i++)
                //{
                //    string argattr = "合";
                //    if (currentUnit.IsAbilityMastered(i) & !currentUnit.IsDisabled(currentUnit.Ability(i).Name) & (!currentUnit.IsAbilityClassifiedAs(i, argattr) | currentUnit.IsCombinationAbilityAvailable(i, true)) & !currentUnit.Ability(i).IsItem())
                //    {
                //        string argtname1 = "アビリティ";
                //        GUI.MainForm.mnuUnitCommandItem(AbilityListCmdID).Caption = Expression.Term(argtname1, SelectedUnit) + "一覧";
                //        GUI.MainForm.mnuUnitCommandItem(AbilityListCmdID).Visible = true;
                //        break;
                //    }
                //}

                //// 味方じゃない場合
                //object argIndex36 = "非操作";
                //if (currentUnit.Party != "味方" | currentUnit.IsConditionSatisfied(argIndex36) | ViewMode)
                //{
                //    // 召喚ユニットは命令コマンドを使用可能
                //    string argfname1 = "召喚ユニット";
                //    object argIndex7 = "魅了";
                //    object argIndex8 = "混乱";
                //    object argIndex9 = "恐怖";
                //    object argIndex10 = "暴走";
                //    object argIndex11 = "狂戦士";
                //    if (currentUnit.Party == "ＮＰＣ" & currentUnit.IsFeatureAvailable(argfname1) & !currentUnit.IsConditionSatisfied(argIndex7) & !currentUnit.IsConditionSatisfied(argIndex8) & !currentUnit.IsConditionSatisfied(argIndex9) & !currentUnit.IsConditionSatisfied(argIndex10) & !currentUnit.IsConditionSatisfied(argIndex11) & !ViewMode)
                //    {
                //        if (currentUnit.Summoner is object)
                //        {
                //            if (currentUnit.Summoner.Party == "味方")
                //            {
                //                GUI.MainForm.mnuUnitCommandItem(OrderCmdID).Caption = "命令";
                //                GUI.MainForm.mnuUnitCommandItem(OrderCmdID).Visible = true;
                //            }
                //        }
                //    }

                //    // 魅了したユニットに対しても命令コマンドを使用可能
                //    object argIndex12 = "魅了";
                //    object argIndex13 = "混乱";
                //    object argIndex14 = "恐怖";
                //    object argIndex15 = "暴走";
                //    object argIndex16 = "狂戦士";
                //    if (currentUnit.Party == "ＮＰＣ" & currentUnit.IsConditionSatisfied(argIndex12) & !currentUnit.IsConditionSatisfied(argIndex13) & !currentUnit.IsConditionSatisfied(argIndex14) & !currentUnit.IsConditionSatisfied(argIndex15) & !currentUnit.IsConditionSatisfied(argIndex16) & !ViewMode)
                //    {
                //        if (currentUnit.Master is object)
                //        {
                //            if (currentUnit.Master.Party == "味方")
                //            {
                //                GUI.MainForm.mnuUnitCommandItem(OrderCmdID).Caption = "命令";
                //                GUI.MainForm.mnuUnitCommandItem(OrderCmdID).Visible = true;
                //            }
                //        }
                //    }

                //    // ダミーユニットの場合はコマンド一覧を表示しない
                //    string argfname2 = "ダミーユニット";
                //    if (currentUnit.IsFeatureAvailable(argfname2))
                //    {
                //        // 特殊能力一覧
                //        if (GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible)
                //        {
                //            UnitCommand(FeatureListCmdID);
                //        }
                //        else
                //        {
                //            CommandState = "ユニット選択";
                //        }

                //        GUI.IsGUILocked = false;
                //        return;
                //    }

                //    if (!string.IsNullOrEmpty(Map.MapFileName))
                //    {
                //        GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Caption = "移動範囲";
                //        GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Visible = true;
                //        var loopTo3 = currentUnit.CountWeapon();
                //        for (i = 1; i <= loopTo3; i++)
                //        {
                //            string argref_mode = "";
                //            string argattr1 = "Ｍ";
                //            if (currentUnit.IsWeaponAvailable(i, argref_mode) & !currentUnit.IsWeaponClassifiedAs(i, argattr1))
                //            {
                //                GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Caption = "射程範囲";
                //                GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = true;
                //            }
                //        }
                //    }

                //    // ユニットステータスコマンド用
                //    if (string.IsNullOrEmpty(Map.MapFileName))
                //    {
                //        // 変形コマンド
                //        string argfname3 = "変形";
                //        if (currentUnit.IsFeatureAvailable(argfname3))
                //        {
                //            object argIndex17 = "変形";
                //            GUI.MainForm.mnuUnitCommandItem(TransformCmdID).Caption = currentUnit.FeatureName(argIndex17);
                //            if (GUI.MainForm.mnuUnitCommandItem(TransformCmdID).Caption == "")
                //            {
                //                GUI.MainForm.mnuUnitCommandItem(TransformCmdID).Caption = "変形";
                //            }

                //            object argIndex19 = "変形";
                //            string arglist1 = currentUnit.FeatureData(argIndex19);
                //            var loopTo4 = GeneralLib.LLength(arglist1);
                //            for (i = 2; i <= loopTo4; i++)
                //            {
                //                object argIndex18 = "変形";
                //                string arglist = currentUnit.FeatureData(argIndex18);
                //                uname = GeneralLib.LIndex(arglist, i);
                //                Unit localOtherForm() { object argIndex1 = uname; var ret = currentUnit.OtherForm(argIndex1); return ret; }

                //                if (localOtherForm().IsAvailable())
                //                {
                //                    GUI.MainForm.mnuUnitCommandItem(TransformCmdID).Visible = true;
                //                    break;
                //                }
                //            }
                //        }

                //        // 分離コマンド
                //        string argfname5 = "分離";
                //        if (currentUnit.IsFeatureAvailable(argfname5))
                //        {
                //            GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible = true;
                //            object argIndex20 = "分離";
                //            GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption = currentUnit.FeatureName(argIndex20);
                //            if (GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption == "")
                //            {
                //                GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption = "分離";
                //            }

                //            object argIndex21 = "分離";
                //            buf = currentUnit.FeatureData(argIndex21);

                //            // 分離形態が利用出来ない場合は分離を行わない
                //            var loopTo5 = GeneralLib.LLength(buf);
                //            for (i = 2; i <= loopTo5; i++)
                //            {
                //                bool localIsDefined() { object argIndex1 = GeneralLib.LIndex(buf, i); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

                //                if (!localIsDefined())
                //                {
                //                    GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible = false;
                //                    break;
                //                }
                //            }

                //            // パイロットが足らない場合も分離を行わない
                //            if (GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible)
                //            {
                //                n = 0;
                //                var loopTo6 = GeneralLib.LLength(buf);
                //                for (i = 2; i <= loopTo6; i++)
                //                {
                //                    Unit localItem() { object argIndex1 = GeneralLib.LIndex(buf, i); var ret = SRC.UList.Item(argIndex1); return ret; }

                //                    {
                //                        var withBlock6 = localItem().Data;
                //                        string argfname4 = "召喚ユニット";
                //                        if (!withBlock6.IsFeatureAvailable(argfname4))
                //                        {
                //                            n = (n + Math.Abs(withBlock6.PilotNum));
                //                        }
                //                    }
                //                }

                //                if (currentUnit.CountPilot() < n)
                //                {
                //                    GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible = false;
                //                }
                //            }
                //        }

                //        string argfname6 = "パーツ分離";
                //        if (currentUnit.IsFeatureAvailable(argfname6))
                //        {
                //            object argIndex22 = "パーツ分離";
                //            GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption = currentUnit.FeatureName(argIndex22);
                //            if (GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption == "")
                //            {
                //                GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption = "パーツ分離";
                //            }
                //            GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible = true;
                //        }

                //        // 合体コマンド
                //        string argfname8 = "合体";
                //        string argfname9 = "パーツ合体";
                //        if (currentUnit.IsFeatureAvailable(argfname8))
                //        {
                //            var loopTo7 = currentUnit.CountFeature();
                //            for (i = 1; i <= loopTo7; i++)
                //            {
                //                object argIndex24 = i;
                //                if (currentUnit.Feature(argIndex24) == "合体")
                //                {
                //                    n = 0;
                //                    // パートナーが存在しているか？
                //                    string localFeatureData1() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

                //                    string arglist2 = localFeatureData1();
                //                    var loopTo8 = GeneralLib.LLength(arglist2);
                //                    for (j = 3; j <= loopTo8; j++)
                //                    {
                //                        string localFeatureData() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

                //                        string localLIndex1() { string arglist = hse3098790f77a4c3c8e351b0c8f045435(); var ret = GeneralLib.LIndex(arglist, j); return ret; }

                //                        object argIndex23 = localLIndex1();
                //                        u = SRC.UList.Item(argIndex23);
                //                        if (u is null)
                //                        {
                //                            break;
                //                        }

                //                        string argfname7 = "合体制限";
                //                        if (u.Status_Renamed != "出撃" & u.CurrentForm().IsFeatureAvailable(argfname7))
                //                        {
                //                            break;
                //                        }

                //                        n = (n + 1);
                //                    }

                //                    // 合体先のユニットが作成されているか？
                //                    string localFeatureData2() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

                //                    string localLIndex2() { string arglist = hse489dc5578704b21a62d1221f27f2c9c(); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                //                    bool localIsDefined1() { object argIndex1 = (object)hs2edf74710015446592f60b6fcb7267d6(); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

                //                    if (!localIsDefined1())
                //                    {
                //                        n = 0;
                //                    }

                //                    // すべての条件を満たしている場合
                //                    string localFeatureData4() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

                //                    int localLLength() { string arglist = hs57a7b11782d04866bf1e5d24ed51c504(); var ret = GeneralLib.LLength(arglist); return ret; }

                //                    if (n == localLLength() - 2)
                //                    {
                //                        GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Visible = true;
                //                        string localFeatureData3() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

                //                        string arglist3 = localFeatureData3();
                //                        GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Caption = GeneralLib.LIndex(arglist3, 1);
                //                        if (GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Caption == "非表示")
                //                        {
                //                            GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Caption = "合体";
                //                        }

                //                        break;
                //                    }
                //                }
                //            }
                //        }
                //        else if (currentUnit.IsFeatureAvailable(argfname9))
                //        {
                //            GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Caption = "パーツ合体";
                //            GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Visible = true;
                //        }

                //        object argIndex30 = "ノーマルモード付加";
                //        if (!currentUnit.IsConditionSatisfied(argIndex30))
                //        {
                //            // ハイパーモードコマンド
                //            string argfname10 = "ハイパーモード";
                //            string argfname11 = "ノーマルモード";
                //            if (currentUnit.IsFeatureAvailable(argfname10))
                //            {
                //                object argIndex25 = "ハイパーモード";
                //                string arglist4 = currentUnit.FeatureData(argIndex25);
                //                uname = GeneralLib.LIndex(arglist4, 2);
                //                Unit localOtherForm1() { object argIndex1 = uname; var ret = currentUnit.OtherForm(argIndex1); return ret; }

                //                if (localOtherForm1().IsAvailable())
                //                {
                //                    GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = true;
                //                    object argIndex26 = "ハイパーモード";
                //                    string arglist5 = currentUnit.FeatureData(argIndex26);
                //                    GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = GeneralLib.LIndex(arglist5, 1);
                //                    if (GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption == "非表示")
                //                    {
                //                        GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "ハイパーモード";
                //                    }
                //                }
                //            }
                //            else if (currentUnit.IsFeatureAvailable(argfname11))
                //            {
                //                object argIndex27 = "ノーマルモード";
                //                string arglist6 = currentUnit.FeatureData(argIndex27);
                //                uname = GeneralLib.LIndex(arglist6, 1);
                //                Unit localOtherForm2() { object argIndex1 = uname; var ret = currentUnit.OtherForm(argIndex1); return ret; }

                //                if (localOtherForm2().IsAvailable())
                //                {
                //                    GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = true;
                //                    GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "ノーマルモード";
                //                    string localLIndex3() { object argIndex1 = "変形"; string arglist = currentUnit.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                //                    if ((uname ?? "") == (localLIndex3() ?? ""))
                //                    {
                //                        GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = false;
                //                    }
                //                }
                //            }
                //        }
                //        else
                //        {
                //            // 変身解除
                //            object argIndex29 = "ノーマルモード";
                //            if (Strings.InStr(currentUnit.FeatureData(argIndex29), "手動解除") > 0)
                //            {
                //                GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = true;
                //                string argfname12 = "変身解除コマンド名";
                //                if (currentUnit.IsFeatureAvailable(argfname12))
                //                {
                //                    object argIndex28 = "変身解除コマンド名";
                //                    GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = currentUnit.FeatureData(argIndex28);
                //                }
                //                else if (currentUnit.IsHero())
                //                {
                //                    GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "変身解除";
                //                }
                //                else
                //                {
                //                    GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "特殊モード解除";
                //                }
                //            }
                //        }

                //        // 換装コマンド
                //        string argfname13 = "換装";
                //        if (currentUnit.IsFeatureAvailable(argfname13))
                //        {
                //            GUI.MainForm.mnuUnitCommandItem(OrderCmdID).Caption = "換装";
                //            object argIndex32 = "換装";
                //            string arglist8 = currentUnit.FeatureData(argIndex32);
                //            var loopTo9 = GeneralLib.LLength(arglist8);
                //            for (i = 1; i <= loopTo9; i++)
                //            {
                //                object argIndex31 = "換装";
                //                string arglist7 = currentUnit.FeatureData(argIndex31);
                //                uname = GeneralLib.LIndex(arglist7, i);
                //                Unit localOtherForm3() { object argIndex1 = uname; var ret = currentUnit.OtherForm(argIndex1); return ret; }

                //                if (localOtherForm3().IsAvailable())
                //                {
                //                    GUI.MainForm.mnuUnitCommandItem(OrderCmdID).Visible = true;
                //                    break;
                //                }
                //            }

                //            // エリアスで換装の名称が変更されている？
                //            {
                //                var withBlock7 = SRC.ALDList;
                //                var loopTo10 = withBlock7.Count();
                //                for (i = 1; i <= loopTo10; i++)
                //                {
                //                    object argIndex33 = i;
                //                    {
                //                        var withBlock8 = withBlock7.Item(argIndex33);
                //                        if (withBlock8.get_AliasType(1) == "換装")
                //                        {
                //                            GUI.MainForm.mnuUnitCommandItem(OrderCmdID).Caption = withBlock8.Name;
                //                            break;
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }

                //    // ユニットコマンド
                //    if (!ViewMode)
                //    {
                //        i = UnitCommand1CmdID;
                //        foreach (LabelData currentLab1 in Event.colEventLabelList)
                //        {
                //            lab = currentLab1;
                //            if (lab.Name == Event.LabelType.UnitCommandEventLabel & lab.Enable)
                //            {
                //                string argexpr = lab.Para(3);
                //                buf = Expression.GetValueAsString(argexpr);
                //                if (SelectedUnit.Party == "味方" & ((buf ?? "") == (SelectedUnit.MainPilot().Name ?? "") | (buf ?? "") == (SelectedUnit.MainPilot().get_Nickname(false) ?? "") | (buf ?? "") == (SelectedUnit.Name ?? "")) | (buf ?? "") == (SelectedUnit.Party ?? "") | buf == "全")
                //                {
                //                    int localGetValueAsLong() { string argexpr = lab.Para(4); var ret = Expression.GetValueAsLong(argexpr); return ret; }

                //                    if (lab.CountPara() <= 3)
                //                    {
                //                        GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
                //                    }
                //                    else if (localGetValueAsLong() != 0)
                //                    {
                //                        GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
                //                    }
                //                }

                //                if (GUI.MainForm.mnuUnitCommandItem(i).Visible)
                //                {
                //                    GUI.MainForm.mnuUnitCommandItem(i).Caption = lab.Para(2);
                //                    UnitCommandLabelList[i - UnitCommand1CmdID + 1] = lab.LineNum.ToString();
                //                    i = (i + 1);
                //                    if (i > UnitCommand10CmdID)
                //                    {
                //                        break;
                //                    }
                //                }
                //            }
                //        }
                //    }

                //    // 未確認ユニットの場合は情報を隠蔽
                //    string argoname8 = "ユニット情報隠蔽";
                //    object argIndex34 = "識別済み";
                //    object argIndex35 = "ユニット情報隠蔽";
                //    if (Expression.IsOptionDefined(argoname8) & !currentUnit.IsConditionSatisfied(argIndex34) & (currentUnit.Party0 == "敵" | currentUnit.Party0 == "中立") | currentUnit.IsConditionSatisfied(argIndex35))
                //    {
                //        GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Visible = true;
                //        GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = false;
                //        GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = false;
                //        GUI.MainForm.mnuUnitCommandItem(WeaponListCmdID).Visible = false;
                //        GUI.MainForm.mnuUnitCommandItem(AbilityListCmdID).Visible = false;
                //        for (i = 1; i <= WaitCmdID; i++)
                //        {
                //            if (GUI.MainForm.mnuUnitCommandItem(i).Visible)
                //            {
                //                break;
                //            }
                //        }

                //        if (i > WaitCmdID)
                //        {
                //            // 表示可能なコマンドがなかった
                //            CommandState = "ユニット選択";
                //            GUI.IsGUILocked = false;
                //            return;
                //        }
                //        // メニューコマンドを全て殺してしまうとエラーになるのでここで非表示
                //        GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Visible = false;
                //    }

                //    GUI.IsGUILocked = false;
                //    if (by_cancel)
                //    {
                //        GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY + 5f);
                //    }
                //    else
                //    {
                //        GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY - 6f);
                //    }

                //    return;
                //}

                //// 行動終了している場合
                //else if (currentUnit.Action == 0)
                //{
                //    // 発進コマンドは使用可能
                //    string argfname14 = "母艦";
                //    if (currentUnit.IsFeatureAvailable(argfname14))
                //    {
                //        if (currentUnit.Area != "地中")
                //        {
                //            if (currentUnit.CountUnitOnBoard() > 0)
                //            {
                //                GUI.MainForm.mnuUnitCommandItem(LaunchCmdID).Visible = true;
                //            }
                //        }
                //    }

                //    // ユニットコマンド
                //    i = UnitCommand1CmdID;
                //    foreach (LabelData currentLab2 in Event.colEventLabelList)
                //    {
                //        lab = currentLab2;
                //        if (lab.Name == Event.LabelType.UnitCommandEventLabel & (lab.AsterNum == 1 | lab.AsterNum == 3))
                //        {
                //            if (lab.Enable)
                //            {
                //                buf = lab.Para(3);
                //                if (SelectedUnit.Party == "味方" & ((buf ?? "") == (SelectedUnit.MainPilot().Name ?? "") | (buf ?? "") == (SelectedUnit.MainPilot().get_Nickname(false) ?? "") | (buf ?? "") == (SelectedUnit.Name ?? "")) | (buf ?? "") == (SelectedUnit.Party ?? "") | buf == "全")
                //                {
                //                    int localStrToLng1() { string argexpr = lab.Para(4); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                //                    if (lab.CountPara() <= 3)
                //                    {
                //                        GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
                //                    }
                //                    else if (localStrToLng1() != 0)
                //                    {
                //                        GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
                //                    }
                //                }
                //            }

                //            if (GUI.MainForm.mnuUnitCommandItem(i).Visible)
                //            {
                //                GUI.MainForm.mnuUnitCommandItem(i).Caption = lab.Para(2);
                //                UnitCommandLabelList[i - UnitCommand1CmdID + 1] = lab.LineNum.ToString();
                //                i = (i + 1);
                //                if (i > UnitCommand10CmdID)
                //                {
                //                    break;
                //                }
                //            }
                //        }
                //    }

                //    GUI.IsGUILocked = false;
                //    GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY - 5f);
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
                } // 移動

                {
                    //// テレポートコマンド
                    //string argfname15 =;
                    //if (currentUnit.IsFeatureAvailable("テレポート"))
                    //{
                    //    if (Strings.Len(currentUnit.FeatureData("テレポート")) > 0)
                    //    {
                    //        string arglist9 = currentUnit.FeatureData("テレポート");
                    //        GUI.MainForm.mnuUnitCommandItem(TeleportCmdID).Caption = GeneralLib.LIndex(arglist9, 1);
                    //    }
                    //    else
                    //    {
                    //        GUI.MainForm.mnuUnitCommandItem(TeleportCmdID).Caption = "テレポート";
                    //    }

                    //    object argIndex40 = "テレポート";
                    //    string arglist10 = currentUnit.FeatureData(argIndex40);
                    //    if (GeneralLib.LLength(arglist10) == 2)
                    //    {
                    //        string localLIndex4() { object argIndex1 = "テレポート"; string arglist = currentUnit.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                    //        string localLIndex5() { object argIndex1 = "テレポート"; string arglist = currentUnit.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                    //        if (currentUnit.EN >= Conversions.Toint(localLIndex5()))
                    //        {
                    //            GUI.MainForm.mnuUnitCommandItem(TeleportCmdID).Visible = true;
                    //        }
                    //        // 通常移動がテレポートの場合
                    //        string localLIndex6() { object argIndex1 = "テレポート"; string arglist = currentUnit.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                    //        object argIndex39 = "テレポート";
                    //        if (currentUnit.Speed0 == 0 | currentUnit.FeatureLevel(argIndex39) >= 0d & localLIndex6() == "0")
                    //        {
                    //            GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Visible = false;
                    //        }
                    //    }
                    //    else if (currentUnit.EN >= 40)
                    //    {
                    //        GUI.MainForm.mnuUnitCommandItem(TeleportCmdID).Visible = true;
                    //    }

                    //    object argIndex41 = "移動不能";
                    //    if (currentUnit.IsConditionSatisfied(argIndex41))
                    //    {
                    //        GUI.MainForm.mnuUnitCommandItem(TeleportCmdID).Visible = false;
                    //    }
                    //}

                    //// ジャンプコマンド
                    //string argfname16 = "ジャンプ";
                    //if (currentUnit.IsFeatureAvailable(argfname16) & currentUnit.Area != "空中" & currentUnit.Area != "宇宙")
                    //{
                    //    object argIndex43 = "ジャンプ";
                    //    if (Strings.Len(currentUnit.FeatureData(argIndex43)) > 0)
                    //    {
                    //        object argIndex42 = "ジャンプ";
                    //        string arglist11 = currentUnit.FeatureData(argIndex42);
                    //        GUI.MainForm.mnuUnitCommandItem(JumpCmdID).Caption = GeneralLib.LIndex(arglist11, 1);
                    //    }
                    //    else
                    //    {
                    //        GUI.MainForm.mnuUnitCommandItem(JumpCmdID).Caption = "ジャンプ";
                    //    }

                    //    object argIndex46 = "ジャンプ";
                    //    string arglist12 = currentUnit.FeatureData(argIndex46);
                    //    if (GeneralLib.LLength(arglist12) == 2)
                    //    {
                    //        string localLIndex7() { object argIndex1 = "ジャンプ"; string arglist = currentUnit.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                    //        string localLIndex8() { object argIndex1 = "ジャンプ"; string arglist = currentUnit.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                    //        if (currentUnit.EN >= Conversions.Toint(localLIndex8()))
                    //        {
                    //            GUI.MainForm.mnuUnitCommandItem(JumpCmdID).Visible = true;
                    //        }
                    //        // 通常移動がジャンプの場合
                    //        string localLIndex9() { object argIndex1 = "ジャンプ"; string arglist = currentUnit.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                    //        object argIndex44 = "ジャンプ";
                    //        if (currentUnit.Speed0 == 0 | currentUnit.FeatureLevel(argIndex44) >= 0d & localLIndex9() == "0")
                    //        {
                    //            GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Visible = false;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        GUI.MainForm.mnuUnitCommandItem(JumpCmdID).Visible = true;
                    //        object argIndex45 = "ジャンプ";
                    //        if (currentUnit.FeatureLevel(argIndex45) >= 0d)
                    //        {
                    //            GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Visible = false;
                    //        }
                    //    }

                    //    object argIndex47 = "移動不能";
                    //    if (currentUnit.IsConditionSatisfied(argIndex47))
                    //    {
                    //        GUI.MainForm.mnuUnitCommandItem(JumpCmdID).Visible = false;
                    //    }
                    //}

                    //// 会話コマンド
                    //for (i = 1; i <= 4; i++)
                    //{
                    //    // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                    //    u = null;
                    //    switch (i)
                    //    {
                    //        case 1:
                    //            {
                    //                if (currentUnit.x > 1)
                    //                {
                    //                    u = Map.MapDataForUnit[currentUnit.x - 1, currentUnit.y];
                    //                }

                    //                break;
                    //            }

                    //        case 2:
                    //            {
                    //                if (currentUnit.x < Map.MapWidth)
                    //                {
                    //                    u = Map.MapDataForUnit[currentUnit.x + 1, currentUnit.y];
                    //                }

                    //                break;
                    //            }

                    //        case 3:
                    //            {
                    //                if (currentUnit.y > 1)
                    //                {
                    //                    u = Map.MapDataForUnit[currentUnit.x, currentUnit.y - 1];
                    //                }

                    //                break;
                    //            }

                    //        case 4:
                    //            {
                    //                if (currentUnit.y < Map.MapHeight)
                    //                {
                    //                    u = Map.MapDataForUnit[currentUnit.x, currentUnit.y + 1];
                    //                }

                    //                break;
                    //            }
                    //    }

                    //    if (u is object)
                    //    {
                    //        string arglname1 = "会話 " + currentUnit.MainPilot().ID + " " + u.MainPilot().ID;
                    //        if (Event.IsEventDefined(arglname1))
                    //        {
                    //            GUI.MainForm.mnuUnitCommandItem(TalkCmdID).Visible = true;
                    //            break;
                    //        }
                    //    }
                    //}

                    // 攻撃コマンド
                    if (currentUnit.Weapons.Any(x => x.IsWeaponUseful("移動前")))
                    {
                        unitCommands.Add(new UiCommand(AttackCmdID, "攻撃"));
                    }

                    if (currentUnit.Area == "地中")
                    {
                        unitCommands.RemoveItem(x => x.Id == AttackCmdID);
                    }

                    //object argIndex48 = "攻撃不能";
                    //if (currentUnit.IsConditionSatisfied(argIndex48))
                    //{
                    //    GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = false;
                    //}

                    //// 修理コマンド
                    //string argfname17 = "修理装置";
                    //if (currentUnit.IsFeatureAvailable(argfname17) & currentUnit.Area != "地中")
                    //{
                    //    for (i = 1; i <= 4; i++)
                    //    {
                    //        // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                    //        u = null;
                    //        switch (i)
                    //        {
                    //            case 1:
                    //                {
                    //                    if (currentUnit.x > 1)
                    //                    {
                    //                        u = Map.MapDataForUnit[currentUnit.x - 1, currentUnit.y];
                    //                    }

                    //                    break;
                    //                }

                    //            case 2:
                    //                {
                    //                    if (currentUnit.x < Map.MapWidth)
                    //                    {
                    //                        u = Map.MapDataForUnit[currentUnit.x + 1, currentUnit.y];
                    //                    }

                    //                    break;
                    //                }

                    //            case 3:
                    //                {
                    //                    if (currentUnit.y > 1)
                    //                    {
                    //                        u = Map.MapDataForUnit[currentUnit.x, currentUnit.y - 1];
                    //                    }

                    //                    break;
                    //                }

                    //            case 4:
                    //                {
                    //                    if (currentUnit.y < Map.MapHeight)
                    //                    {
                    //                        u = Map.MapDataForUnit[currentUnit.x, currentUnit.y + 1];
                    //                    }

                    //                    break;
                    //                }
                    //        }

                    //        if (u is object)
                    //        {
                    //            {
                    //                var withBlock9 = u;
                    //                object argIndex49 = "ゾンビ";
                    //                if ((withBlock9.Party == "味方" | withBlock9.Party == "ＮＰＣ") & withBlock9.HP < withBlock9.MaxHP & !withBlock9.IsConditionSatisfied(argIndex49))
                    //                {
                    //                    GUI.MainForm.mnuUnitCommandItem(FixCmdID).Visible = true;
                    //                    break;
                    //                }
                    //            }
                    //        }
                    //    }

                    //    object argIndex51 = "修理装置";
                    //    if (Strings.Len(currentUnit.FeatureData(argIndex51)) > 0)
                    //    {
                    //        object argIndex50 = "修理装置";
                    //        string arglist13 = currentUnit.FeatureData(argIndex50);
                    //        GUI.MainForm.mnuUnitCommandItem(FixCmdID).Caption = GeneralLib.LIndex(arglist13, 1);
                    //        string localLIndex12() { object argIndex1 = "修理装置"; string arglist = currentUnit.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                    //        if (Information.IsNumeric(localLIndex12()))
                    //        {
                    //            string localLIndex10() { object argIndex1 = "修理装置"; string arglist = currentUnit.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                    //            string localLIndex11() { object argIndex1 = "修理装置"; string arglist = currentUnit.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                    //            if (currentUnit.EN < Conversions.Toint(localLIndex11()))
                    //            {
                    //                GUI.MainForm.mnuUnitCommandItem(FixCmdID).Visible = false;
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        GUI.MainForm.mnuUnitCommandItem(FixCmdID).Caption = "修理装置";
                    //    }
                    //}

                    //// 補給コマンド
                    //string argfname18 = "補給装置";
                    //if (currentUnit.IsFeatureAvailable(argfname18) & currentUnit.Area != "地中")
                    //{
                    //    for (i = 1; i <= 4; i++)
                    //    {
                    //        // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                    //        u = null;
                    //        switch (i)
                    //        {
                    //            case 1:
                    //                {
                    //                    if (currentUnit.x > 1)
                    //                    {
                    //                        u = Map.MapDataForUnit[currentUnit.x - 1, currentUnit.y];
                    //                    }

                    //                    break;
                    //                }

                    //            case 2:
                    //                {
                    //                    if (currentUnit.x < Map.MapWidth)
                    //                    {
                    //                        u = Map.MapDataForUnit[currentUnit.x + 1, currentUnit.y];
                    //                    }

                    //                    break;
                    //                }

                    //            case 3:
                    //                {
                    //                    if (currentUnit.y > 1)
                    //                    {
                    //                        u = Map.MapDataForUnit[currentUnit.x, currentUnit.y - 1];
                    //                    }

                    //                    break;
                    //                }

                    //            case 4:
                    //                {
                    //                    if (currentUnit.y < Map.MapHeight)
                    //                    {
                    //                        u = Map.MapDataForUnit[currentUnit.x, currentUnit.y + 1];
                    //                    }

                    //                    break;
                    //                }
                    //        }

                    //        if (u is object)
                    //        {
                    //            {
                    //                var withBlock10 = u;
                    //                if (withBlock10.Party == "味方" | withBlock10.Party == "ＮＰＣ")
                    //                {
                    //                    object argIndex52 = "ゾンビ";
                    //                    if (withBlock10.EN < withBlock10.MaxEN & !withBlock10.IsConditionSatisfied(argIndex52))
                    //                    {
                    //                        GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = true;
                    //                    }
                    //                    else
                    //                    {
                    //                        var loopTo12 = withBlock10.CountWeapon();
                    //                        for (j = 1; j <= loopTo12; j++)
                    //                        {
                    //                            if (withBlock10.Bullet(j) < withBlock10.MaxBullet(j))
                    //                            {
                    //                                GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = true;
                    //                                break;
                    //                            }
                    //                        }

                    //                        var loopTo13 = withBlock10.CountAbility();
                    //                        for (j = 1; j <= loopTo13; j++)
                    //                        {
                    //                            if (withBlock10.Stock(j) < withBlock10.MaxStock(j))
                    //                            {
                    //                                GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = true;
                    //                                break;
                    //                            }
                    //                        }
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }

                    //    object argIndex54 = "補給装置";
                    //    if (Strings.Len(currentUnit.FeatureData(argIndex54)) > 0)
                    //    {
                    //        object argIndex53 = "補給装置";
                    //        string arglist14 = currentUnit.FeatureData(argIndex53);
                    //        GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = GeneralLib.LIndex(arglist14, 1);
                    //        string localLIndex15() { object argIndex1 = "補給装置"; string arglist = currentUnit.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                    //        if (Information.IsNumeric(localLIndex15()))
                    //        {
                    //            string localLIndex13() { object argIndex1 = "補給装置"; string arglist = currentUnit.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                    //            string localLIndex14() { object argIndex1 = "補給装置"; string arglist = currentUnit.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                    //            if (currentUnit.EN < Conversions.Toint(localLIndex14()) | currentUnit.MainPilot().Morale < 100)
                    //            {
                    //                GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = false;
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = "補給装置";
                    //    }
                    //}

                    //// アビリティコマンド
                    //n = 0;
                    //var loopTo14 = currentUnit.CountAbility();
                    //for (i = 1; i <= loopTo14; i++)
                    //{
                    //    if (!currentUnit.Ability(i).IsItem() & currentUnit.IsAbilityMastered(i))
                    //    {
                    //        n = (n + 1);
                    //        string argref_mode2 = "移動前";
                    //        if (currentUnit.IsAbilityUseful(i, argref_mode2))
                    //        {
                    //            GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = true;
                    //        }
                    //    }
                    //}

                    if (currentUnit.Area == "地中")
                    {
                        unitCommands.RemoveItem(x => x.Id == AbilityCmdID);
                    }
                    //string argtname2 = "アビリティ";
                    //GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = Expression.Term(argtname2, SelectedUnit);
                    //if (n == 1)
                    //{
                    //    var loopTo15 = currentUnit.CountAbility();
                    //    for (i = 1; i <= loopTo15; i++)
                    //    {
                    //        if (!currentUnit.Ability(i).IsItem() & currentUnit.IsAbilityMastered(i))
                    //        {
                    //            GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = currentUnit.AbilityNickname(i);
                    //            break;
                    //        }
                    //    }
                    //}

                    //// チャージコマンド
                    //object argIndex55 = "チャージ完了";
                    //if (!currentUnit.IsConditionSatisfied(argIndex55))
                    //{
                    //    var loopTo16 = currentUnit.CountWeapon();
                    //    for (i = 1; i <= loopTo16; i++)
                    //    {
                    //        string argattr2 = "Ｃ";
                    //        if (currentUnit.IsWeaponClassifiedAs(i, argattr2))
                    //        {
                    //            string argref_mode3 = "チャージ";
                    //            if (currentUnit.IsWeaponAvailable(i, argref_mode3))
                    //            {
                    //                GUI.MainForm.mnuUnitCommandItem(ChargeCmdID).Visible = true;
                    //                break;
                    //            }
                    //        }
                    //    }

                    //    var loopTo17 = currentUnit.CountAbility();
                    //    for (i = 1; i <= loopTo17; i++)
                    //    {
                    //        string argattr3 = "Ｃ";
                    //        if (currentUnit.IsAbilityClassifiedAs(i, argattr3))
                    //        {
                    //            string argref_mode4 = "チャージ";
                    //            if (currentUnit.IsAbilityAvailable(i, argref_mode4))
                    //            {
                    //                GUI.MainForm.mnuUnitCommandItem(ChargeCmdID).Visible = true;
                    //                break;
                    //            }
                    //        }
                    //    }
                    //}

                    //// スペシャルパワーコマンド
                    //string argtname3 = "スペシャルパワー";
                    //GUI.MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Caption = Expression.Term(argtname3, SelectedUnit);
                    //if (currentUnit.MainPilot().CountSpecialPower > 0)
                    //{
                    //    GUI.MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = true;
                    //}
                    //else
                    //{
                    //    var loopTo18 = currentUnit.CountPilot();
                    //    for (i = 1; i <= loopTo18; i++)
                    //    {
                    //        Pilot localPilot() { object argIndex1 = i; var ret = currentUnit.Pilot(argIndex1); return ret; }

                    //        if (localPilot().CountSpecialPower > 0)
                    //        {
                    //            GUI.MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = true;
                    //            break;
                    //        }
                    //    }

                    //    var loopTo19 = currentUnit.CountSupport();
                    //    for (i = 1; i <= loopTo19; i++)
                    //    {
                    //        Pilot localSupport() { object argIndex1 = i; var ret = currentUnit.Support(argIndex1); return ret; }

                    //        if (localSupport().CountSpecialPower > 0)
                    //        {
                    //            GUI.MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = true;
                    //            break;
                    //        }
                    //    }

                    //    string argfname19 = "追加サポート";
                    //    if (currentUnit.IsFeatureAvailable(argfname19))
                    //    {
                    //        if (currentUnit.AdditionalSupport().CountSpecialPower > 0)
                    //        {
                    //            GUI.MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = true;
                    //        }
                    //    }
                    //}

                    //object argIndex56 = "憑依";
                    //object argIndex57 = "スペシャルパワー使用不能";
                    //if (currentUnit.IsConditionSatisfied(argIndex56) | currentUnit.IsConditionSatisfied(argIndex57))
                    //{
                    //    GUI.MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = false;
                    //}

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

                    //string argfname23 = "パーツ分離";
                    //object argIndex70 = "パーツ分離";
                    //if (currentUnit.IsFeatureAvailable(argfname23) & !string.IsNullOrEmpty(currentUnit.FeatureName(argIndex70)))
                    //{
                    //    object argIndex69 = "パーツ分離";
                    //    GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption = currentUnit.FeatureName(argIndex69);
                    //    GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible = true;
                    //}

                    //// 合体コマンド
                    //string argfname25 = "合体";
                    //object argIndex74 = "形態固定";
                    //object argIndex75 = "機体固定";
                    //if (currentUnit.IsFeatureAvailable(argfname25) & !currentUnit.IsConditionSatisfied(argIndex74) & !currentUnit.IsConditionSatisfied(argIndex75))
                    //{
                    //    var loopTo23 = currentUnit.CountFeature();
                    //    for (i = 1; i <= loopTo23; i++)
                    //    {
                    //        // 3体以上からなる合体能力を持っているか？
                    //        string localFeature() { object argIndex1 = i; var ret = currentUnit.Feature(argIndex1); return ret; }

                    //        string localFeatureName() { object argIndex1 = i; var ret = currentUnit.FeatureName(argIndex1); return ret; }

                    //        string localFeatureData10() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

                    //        int localLLength2() { string arglist = hsc81ad842db7849b2b1585eed09bb5348(); var ret = GeneralLib.LLength(arglist); return ret; }

                    //        if (localFeature() == "合体" & !string.IsNullOrEmpty(localFeatureName()) & localLLength2() > 3)
                    //        {
                    //            n = 0;
                    //            // パートナーは隣接しているか？
                    //            string localFeatureData6() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

                    //            string arglist17 = localFeatureData6();
                    //            var loopTo24 = GeneralLib.LLength(arglist17);
                    //            for (j = 3; j <= loopTo24; j++)
                    //            {
                    //                string localFeatureData5() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

                    //                string localLIndex16() { string arglist = hsd2010d4d78e3489683c8d110139f0dc7(); var ret = GeneralLib.LIndex(arglist, j); return ret; }

                    //                object argIndex71 = localLIndex16();
                    //                u = SRC.UList.Item(argIndex71);
                    //                if (u is null)
                    //                {
                    //                    break;
                    //                }

                    //                if (!u.IsOperational())
                    //                {
                    //                    break;
                    //                }

                    //                string argfname24 = "合体制限";
                    //                if (u.Status_Renamed != "出撃" & u.CurrentForm().IsFeatureAvailable(argfname24))
                    //                {
                    //                    break;
                    //                }

                    //                if (Math.Abs((currentUnit.x - u.CurrentForm().x)) + Math.Abs((currentUnit.y - u.CurrentForm().y)) > 2)
                    //                {
                    //                    break;
                    //                }

                    //                n = (n + 1);
                    //            }

                    //            // 合体先のユニットが作成され、かつ合体可能な状態にあるか？
                    //            string localFeatureData7() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

                    //            string arglist18 = localFeatureData7();
                    //            uname = GeneralLib.LIndex(arglist18, 2);
                    //            object argIndex72 = uname;
                    //            u = SRC.UList.Item(argIndex72);
                    //            object argIndex73 = "行動不能";
                    //            if (u is null)
                    //            {
                    //                n = 0;
                    //            }
                    //            else if (u.IsConditionSatisfied(argIndex73))
                    //            {
                    //                n = 0;
                    //            }

                    //            // すべての条件を満たしている場合
                    //            string localFeatureData9() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

                    //            int localLLength1() { string arglist = hs7cff35930ba14e62b7ee40e2d0172e97(); var ret = GeneralLib.LLength(arglist); return ret; }

                    //            if (n == localLLength1() - 2)
                    //            {
                    //                GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Visible = true;
                    //                string localFeatureData8() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

                    //                string arglist19 = localFeatureData8();
                    //                GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Caption = GeneralLib.LIndex(arglist19, 1);
                    //                break;
                    //            }
                    //        }
                    //    }
                    //}

                    //object argIndex88 = "ノーマルモード付加";
                    //if (!currentUnit.IsConditionSatisfied(argIndex88))
                    //{
                    //    // ハイパーモードコマンド
                    //    string argfname26 = "ハイパーモード";
                    //    object argIndex79 = "ハイパーモード";
                    //    object argIndex80 = "ハイパーモード";
                    //    object argIndex81 = "ハイパーモード";
                    //    object argIndex82 = "ハイパーモード";
                    //    object argIndex83 = "ハイパーモード";
                    //    object argIndex84 = "形態固定";
                    //    object argIndex85 = "機体固定";
                    //    if (currentUnit.IsFeatureAvailable(argfname26) & (currentUnit.MainPilot().Morale >= (10d * currentUnit.FeatureLevel(argIndex80)) + 100 | currentUnit.HP <= currentUnit.MaxHP / 4 & Strings.InStr(currentUnit.FeatureData(argIndex81), "気力発動") == 0) & Strings.InStr(currentUnit.FeatureData(argIndex82), "自動発動") == 0 & !string.IsNullOrEmpty(currentUnit.FeatureName(argIndex83)) & !currentUnit.IsConditionSatisfied(argIndex84) & !currentUnit.IsConditionSatisfied(argIndex85))
                    //    {
                    //        object argIndex76 = "ハイパーモード";
                    //        string arglist20 = currentUnit.FeatureData(argIndex76);
                    //        uname = GeneralLib.LIndex(arglist20, 2);
                    //        Unit localOtherForm5() { object argIndex1 = uname; var ret = currentUnit.OtherForm(argIndex1); return ret; }

                    //        Unit localOtherForm6() { object argIndex1 = uname; var ret = currentUnit.OtherForm(argIndex1); return ret; }

                    //        object argIndex78 = "行動不能";
                    //        if (!localOtherForm5().IsConditionSatisfied(argIndex78) & localOtherForm6().IsAbleToEnter(currentUnit.x, currentUnit.y))
                    //        {
                    //            GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = true;
                    //            object argIndex77 = "ハイパーモード";
                    //            string arglist21 = currentUnit.FeatureData(argIndex77);
                    //            GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = GeneralLib.LIndex(arglist21, 1);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    // 変身解除
                    //    object argIndex87 = "ノーマルモード";
                    //    if (Strings.InStr(currentUnit.FeatureData(argIndex87), "手動解除") > 0)
                    //    {
                    //        GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = true;
                    //        string argfname27 = "変身解除コマンド名";
                    //        if (currentUnit.IsFeatureAvailable(argfname27))
                    //        {
                    //            object argIndex86 = "変身解除コマンド名";
                    //            GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = currentUnit.FeatureData(argIndex86);
                    //        }
                    //        else if (currentUnit.IsHero())
                    //        {
                    //            GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "変身解除";
                    //        }
                    //        else
                    //        {
                    //            GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "特殊モード解除";
                    //        }
                    //    }
                    //}

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
                            if ((currentUnit.IsTransAvailable("空") || currentUnit.IsTransAvailable("宇宙")) & !(currentUnit.Area == "宇宙"))
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
                    if (currentUnit.IsFeatureAvailable("母艦") & currentUnit.Area != "地中")
                    {
                        if (currentUnit.CountUnitOnBoard() > 0)
                        {
                            unitCommands.Add(new UiCommand(LaunchCmdID, "発進"));
                        }
                    }

                    //// アイテムコマンド
                    //var loopTo25 = currentUnit.CountAbility();
                    //for (i = 1; i <= loopTo25; i++)
                    //{
                    //    string argref_mode5 = "移動前";
                    //    if (currentUnit.IsAbilityUseful(i, argref_mode5) & currentUnit.Ability(i).IsItem())
                    //    {
                    //        GUI.MainForm.mnuUnitCommandItem(ItemCmdID).Visible = true;
                    //        break;
                    //    }
                    //}

                    //if (currentUnit.Area == "地中")
                    //{
                    //    GUI.MainForm.mnuUnitCommandItem(ItemCmdID).Visible = false;
                    //}

                    //// 召喚解除コマンド
                    //var loopTo26 = currentUnit.CountServant();
                    //for (i = 1; i <= loopTo26; i++)
                    //{
                    //    Unit localServant() { object argIndex1 = i; var ret = currentUnit.Servant(argIndex1); return ret; }

                    //    {
                    //        var withBlock12 = localServant().CurrentForm();
                    //        switch (withBlock12.Status_Renamed ?? "")
                    //        {
                    //            case "出撃":
                    //            case "格納":
                    //                {
                    //                    GUI.MainForm.mnuUnitCommandItem(DismissCmdID).Visible = true;
                    //                    break;
                    //                }

                    //            case "旧主形態":
                    //            case "旧形態":
                    //                {
                    //                    // 合体後の形態が出撃中なら使用不可
                    //                    GUI.MainForm.mnuUnitCommandItem(DismissCmdID).Visible = true;
                    //                    var loopTo27 = withBlock12.CountFeature();
                    //                    for (j = 1; j <= loopTo27; j++)
                    //                    {
                    //                        object argIndex90 = j;
                    //                        if (withBlock12.Feature(argIndex90) == "合体")
                    //                        {
                    //                            string localFeatureData11() { object argIndex1 = j; var ret = withBlock12.FeatureData(argIndex1); return ret; }

                    //                            string arglist22 = localFeatureData11();
                    //                            uname = GeneralLib.LIndex(arglist22, 2);
                    //                            object argIndex89 = uname;
                    //                            if (SRC.UList.IsDefined(argIndex89))
                    //                            {
                    //                                Unit localItem2() { object argIndex1 = uname; var ret = SRC.UList.Item(argIndex1); return ret; }

                    //                                {
                    //                                    var withBlock13 = localItem2().CurrentForm();
                    //                                    if (withBlock13.Status_Renamed == "出撃" | withBlock13.Status_Renamed == "格納")
                    //                                    {
                    //                                        GUI.MainForm.mnuUnitCommandItem(DismissCmdID).Visible = false;
                    //                                    }
                    //                                }
                    //                            }
                    //                        }
                    //                    }

                    //                    break;
                    //                }
                    //        }
                    //    }
                    //}

                    //string argfname30 = "召喚解除コマンド名";
                    //if (currentUnit.IsFeatureAvailable(argfname30))
                    //{
                    //    object argIndex91 = "召喚解除コマンド名";
                    //    GUI.MainForm.mnuUnitCommandItem(DismissCmdID).Caption = currentUnit.FeatureData(argIndex91);
                    //}
                    //else
                    //{
                    //    GUI.MainForm.mnuUnitCommandItem(DismissCmdID).Caption = "召喚解除";
                    //}

                    //// ユニットコマンド
                    //i = UnitCommand1CmdID;
                    //foreach (LabelData currentLab3 in Event.colEventLabelList)
                    //{
                    //    lab = currentLab3;
                    //    if (lab.Name == Event.LabelType.UnitCommandEventLabel)
                    //    {
                    //        if (lab.Enable)
                    //        {
                    //            buf = lab.Para(3);
                    //            if (SelectedUnit.Party == "味方" & ((buf ?? "") == (SelectedUnit.MainPilot().Name ?? "") | (buf ?? "") == (SelectedUnit.MainPilot().get_Nickname(false) ?? "") | (buf ?? "") == (SelectedUnit.Name ?? "")) | (buf ?? "") == (SelectedUnit.Party ?? "") | buf == "全")
                    //            {
                    //                int localStrToLng2() { string argexpr = lab.Para(4); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                    //                if (lab.CountPara() <= 3)
                    //                {
                    //                    GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
                    //                }
                    //                else if (localStrToLng2() != 0)
                    //                {
                    //                    GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
                    //                }
                    //            }
                    //        }

                    //        if (GUI.MainForm.mnuUnitCommandItem(i).Visible)
                    //        {
                    //            GUI.MainForm.mnuUnitCommandItem(i).Caption = lab.Para(2);
                    //            UnitCommandLabelList[i - UnitCommand1CmdID + 1] = lab.LineNum.ToString();
                    //            i = (i + 1);
                    //            if (i > UnitCommand10CmdID)
                    //            {
                    //                break;
                    //            }
                    //        }
                    //    }
                    //}
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

                //// 会話コマンド
                //GUI.MainForm.mnuUnitCommandItem(TalkCmdID).Visible = false;
                //for (i = 1; i <= 4; i++)
                //{
                //    // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                //    u = null;
                //    switch (i)
                //    {
                //        case 1:
                //            {
                //                if (currentUnit.x > 1)
                //                {
                //                    u = Map.MapDataForUnit[currentUnit.x - 1, currentUnit.y];
                //                }

                //                break;
                //            }

                //        case 2:
                //            {
                //                if (currentUnit.x < Map.MapWidth)
                //                {
                //                    u = Map.MapDataForUnit[currentUnit.x + 1, currentUnit.y];
                //                }

                //                break;
                //            }

                //        case 3:
                //            {
                //                if (currentUnit.y > 1)
                //                {
                //                    u = Map.MapDataForUnit[currentUnit.x, currentUnit.y - 1];
                //                }

                //                break;
                //            }

                //        case 4:
                //            {
                //                if (currentUnit.y < Map.MapHeight)
                //                {
                //                    u = Map.MapDataForUnit[currentUnit.x, currentUnit.y + 1];
                //                }

                //                break;
                //            }
                //    }

                //    if (u is object)
                //    {
                //        string arglname2 = "会話 " + currentUnit.MainPilot().ID + " " + u.MainPilot().ID;
                //        if (Event.IsEventDefined(arglname2))
                //        {
                //            GUI.MainForm.mnuUnitCommandItem(TalkCmdID).Visible = true;
                //            break;
                //        }
                //    }
                //}

                // 攻撃コマンド
                if (currentUnit.Weapons.Any(x => x.IsWeaponUseful("移動後")))
                {
                    unitCommands.Add(new UiCommand(AttackCmdID, "攻撃"));
                }

                //if (currentUnit.Area == "地中")
                //{
                //    GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = false;
                //}

                //object argIndex92 = "攻撃不能";
                //if (currentUnit.IsConditionSatisfied(argIndex92))
                //{
                //    GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = false;
                //}

                //// 修理コマンド
                //GUI.MainForm.mnuUnitCommandItem(FixCmdID).Visible = false;
                //string argfname31 = "修理装置";
                //if (currentUnit.IsFeatureAvailable(argfname31) & currentUnit.Area != "地中")
                //{
                //    for (i = 1; i <= 4; i++)
                //    {
                //        // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                //        u = null;
                //        switch (i)
                //        {
                //            case 1:
                //                {
                //                    if (currentUnit.x > 1)
                //                    {
                //                        u = Map.MapDataForUnit[currentUnit.x - 1, currentUnit.y];
                //                    }

                //                    break;
                //                }

                //            case 2:
                //                {
                //                    if (currentUnit.x < Map.MapWidth)
                //                    {
                //                        u = Map.MapDataForUnit[currentUnit.x + 1, currentUnit.y];
                //                    }

                //                    break;
                //                }

                //            case 3:
                //                {
                //                    if (currentUnit.y > 1)
                //                    {
                //                        u = Map.MapDataForUnit[currentUnit.x, currentUnit.y - 1];
                //                    }

                //                    break;
                //                }

                //            case 4:
                //                {
                //                    if (currentUnit.y < Map.MapHeight)
                //                    {
                //                        u = Map.MapDataForUnit[currentUnit.x, currentUnit.y + 1];
                //                    }

                //                    break;
                //                }
                //        }

                //        if (u is object)
                //        {
                //            {
                //                var withBlock16 = u;
                //                if ((withBlock16.Party == "味方" | withBlock16.Party == "ＮＰＣ") & withBlock16.HP < withBlock16.MaxHP)
                //                {
                //                    GUI.MainForm.mnuUnitCommandItem(FixCmdID).Visible = true;
                //                    break;
                //                }
                //            }
                //        }
                //    }

                //    object argIndex94 = "修理装置";
                //    if (Strings.Len(currentUnit.FeatureData(argIndex94)) > 0)
                //    {
                //        object argIndex93 = "修理装置";
                //        string arglist23 = currentUnit.FeatureData(argIndex93);
                //        GUI.MainForm.mnuUnitCommandItem(FixCmdID).Caption = GeneralLib.LIndex(arglist23, 1);
                //        string localLIndex19() { object argIndex1 = "修理装置"; string arglist = currentUnit.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                //        if (Information.IsNumeric(localLIndex19()))
                //        {
                //            string localLIndex17() { object argIndex1 = "修理装置"; string arglist = currentUnit.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                //            string localLIndex18() { object argIndex1 = "修理装置"; string arglist = currentUnit.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                //            if (currentUnit.EN < Conversions.Toint(localLIndex18()))
                //            {
                //                GUI.MainForm.mnuUnitCommandItem(FixCmdID).Visible = false;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        GUI.MainForm.mnuUnitCommandItem(FixCmdID).Caption = "修理装置";
                //    }
                //}

                //// 補給コマンド
                //GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = false;
                //string argfname32 = "補給装置";
                //if (currentUnit.IsFeatureAvailable(argfname32) & currentUnit.Area != "地中")
                //{
                //    for (i = 1; i <= 4; i++)
                //    {
                //        // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                //        u = null;
                //        switch (i)
                //        {
                //            case 1:
                //                {
                //                    if (currentUnit.x > 1)
                //                    {
                //                        u = Map.MapDataForUnit[currentUnit.x - 1, currentUnit.y];
                //                    }

                //                    break;
                //                }

                //            case 2:
                //                {
                //                    if (currentUnit.x < Map.MapWidth)
                //                    {
                //                        u = Map.MapDataForUnit[currentUnit.x + 1, currentUnit.y];
                //                    }

                //                    break;
                //                }

                //            case 3:
                //                {
                //                    if (currentUnit.y > 1)
                //                    {
                //                        u = Map.MapDataForUnit[currentUnit.x, currentUnit.y - 1];
                //                    }

                //                    break;
                //                }

                //            case 4:
                //                {
                //                    if (currentUnit.y < Map.MapHeight)
                //                    {
                //                        u = Map.MapDataForUnit[currentUnit.x, currentUnit.y + 1];
                //                    }

                //                    break;
                //                }
                //        }

                //        if (u is object)
                //        {
                //            {
                //                var withBlock17 = u;
                //                if (withBlock17.Party == "味方" | withBlock17.Party == "ＮＰＣ")
                //                {
                //                    if (withBlock17.EN < withBlock17.MaxEN)
                //                    {
                //                        GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = true;
                //                    }
                //                    else
                //                    {
                //                        var loopTo29 = withBlock17.CountWeapon();
                //                        for (j = 1; j <= loopTo29; j++)
                //                        {
                //                            if (withBlock17.Bullet(j) < withBlock17.MaxBullet(j))
                //                            {
                //                                GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = true;
                //                                break;
                //                            }
                //                        }

                //                        var loopTo30 = withBlock17.CountAbility();
                //                        for (j = 1; j <= loopTo30; j++)
                //                        {
                //                            if (withBlock17.Stock(j) < withBlock17.MaxStock(j))
                //                            {
                //                                GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = true;
                //                                break;
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }

                //    object argIndex96 = "補給装置";
                //    if (Strings.Len(currentUnit.FeatureData(argIndex96)) > 0)
                //    {
                //        object argIndex95 = "補給装置";
                //        string arglist24 = currentUnit.FeatureData(argIndex95);
                //        GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = GeneralLib.LIndex(arglist24, 1);
                //        string localLIndex22() { object argIndex1 = "補給装置"; string arglist = currentUnit.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                //        if (Information.IsNumeric(localLIndex22()))
                //        {
                //            string localLIndex20() { object argIndex1 = "補給装置"; string arglist = currentUnit.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                //            string localLIndex21() { object argIndex1 = "補給装置"; string arglist = currentUnit.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                //            if (currentUnit.EN < Conversions.Toint(localLIndex21()) | currentUnit.MainPilot().Morale < 100)
                //            {
                //                GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = false;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = "補給装置";
                //    }

                //    string argoname9 = "移動後補給不可";
                //    string argsname = "補給";
                //    if (Expression.IsOptionDefined(argoname9) & !SelectedUnit.MainPilot().IsSkillAvailable(argsname))
                //    {
                //        GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = false;
                //    }
                //}

                //// アビリティコマンド
                //GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = false;
                //n = 0;
                //var loopTo31 = currentUnit.CountAbility();
                //for (i = 1; i <= loopTo31; i++)
                //{
                //    if (!currentUnit.Ability(i).IsItem())
                //    {
                //        n = (n + 1);
                //        string argref_mode7 = "移動後";
                //        if (currentUnit.IsAbilityUseful(i, argref_mode7))
                //        {
                //            GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = true;
                //        }
                //    }
                //}

                //if (currentUnit.Area == "地中")
                //{
                //    GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = false;
                //}
                //string argtname4 = "アビリティ";
                //GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = Expression.Term(argtname4, SelectedUnit);
                //if (n == 1)
                //{
                //    var loopTo32 = currentUnit.CountAbility();
                //    for (i = 1; i <= loopTo32; i++)
                //    {
                //        if (!currentUnit.Ability(i).IsItem())
                //        {
                //            GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = currentUnit.AbilityNickname(i);
                //            break;
                //        }
                //    }
                //}

                //{
                //    var withBlock18 = GUI.MainForm;
                //    withBlock18.mnuUnitCommandItem(ChargeCmdID).Visible = false;
                //    withBlock18.mnuUnitCommandItem(SpecialPowerCmdID).Visible = false;
                //    withBlock18.mnuUnitCommandItem(TransformCmdID).Visible = false;
                //    withBlock18.mnuUnitCommandItem(SplitCmdID).Visible = false;
                //}

                //// 合体コマンド
                //GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Visible = false;
                //string argfname34 = "合体";
                //object argIndex100 = "形態固定";
                //object argIndex101 = "機体固定";
                //if (currentUnit.IsFeatureAvailable(argfname34) & !currentUnit.IsConditionSatisfied(argIndex100) & !currentUnit.IsConditionSatisfied(argIndex101))
                //{
                //    var loopTo33 = currentUnit.CountFeature();
                //    for (i = 1; i <= loopTo33; i++)
                //    {
                //        // 3体以上からなる合体能力を持っているか？
                //        string localFeature1() { object argIndex1 = i; var ret = currentUnit.Feature(argIndex1); return ret; }

                //        string localFeatureName1() { object argIndex1 = i; var ret = currentUnit.FeatureName(argIndex1); return ret; }

                //        string localFeatureData17() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

                //        int localLLength4() { string arglist = hs9469e7ffcaeb496ca82716c7891638cc(); var ret = GeneralLib.LLength(arglist); return ret; }

                //        if (localFeature1() == "合体" & !string.IsNullOrEmpty(localFeatureName1()) & localLLength4() > 3)
                //        {
                //            n = 0;
                //            string localFeatureData13() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

                //            string arglist25 = localFeatureData13();
                //            var loopTo34 = GeneralLib.LLength(arglist25);
                //            for (j = 3; j <= loopTo34; j++)
                //            {
                //                string localFeatureData12() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

                //                string localLIndex23() { string arglist = hs175f067af849438ea2ce369fbd24d08f(); var ret = GeneralLib.LIndex(arglist, j); return ret; }

                //                object argIndex97 = localLIndex23();
                //                u = SRC.UList.Item(argIndex97);
                //                if (u is null)
                //                {
                //                    break;
                //                }

                //                if (!u.IsOperational())
                //                {
                //                    break;
                //                }

                //                string argfname33 = "合体制限";
                //                if (u.Status_Renamed != "出撃" & u.CurrentForm().IsFeatureAvailable(argfname33))
                //                {
                //                    break;
                //                }

                //                if (Math.Abs((currentUnit.x - u.CurrentForm().x)) + Math.Abs((currentUnit.y - u.CurrentForm().y)) > 2)
                //                {
                //                    break;
                //                }

                //                n = (n + 1);
                //            }

                //            string localFeatureData14() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

                //            string arglist26 = localFeatureData14();
                //            uname = GeneralLib.LIndex(arglist26, 2);
                //            object argIndex98 = uname;
                //            u = SRC.UList.Item(argIndex98);
                //            object argIndex99 = "行動不能";
                //            if (u is null)
                //            {
                //                n = 0;
                //            }
                //            else if (u.IsConditionSatisfied(argIndex99))
                //            {
                //                n = 0;
                //            }

                //            string localFeatureData16() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

                //            int localLLength3() { string arglist = hse55df985d6054cfe94b90350a9c471f6(); var ret = GeneralLib.LLength(arglist); return ret; }

                //            if (n == localLLength3() - 2)
                //            {
                //                GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Visible = true;
                //                string localFeatureData15() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

                //                string arglist27 = localFeatureData15();
                //                GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Caption = GeneralLib.LIndex(arglist27, 1);
                //                break;
                //            }
                //        }
                //    }
                //}

                //{
                //    var withBlock19 = GUI.MainForm;
                //    withBlock19.mnuUnitCommandItem(HyperModeCmdID).Visible = false;
                //    withBlock19.mnuUnitCommandItem(GroundCmdID).Visible = false;
                //    withBlock19.mnuUnitCommandItem(SkyCmdID).Visible = false;
                //    withBlock19.mnuUnitCommandItem(UndergroundCmdID).Visible = false;
                //    withBlock19.mnuUnitCommandItem(WaterCmdID).Visible = false;
                //    withBlock19.mnuUnitCommandItem(LaunchCmdID).Visible = false;
                //}

                //// アイテムコマンド
                //GUI.MainForm.mnuUnitCommandItem(ItemCmdID).Visible = false;
                //var loopTo35 = currentUnit.CountAbility();
                //for (i = 1; i <= loopTo35; i++)
                //{
                //    string argref_mode8 = "移動後";
                //    if (currentUnit.IsAbilityUseful(i, argref_mode8) & currentUnit.Ability(i).IsItem())
                //    {
                //        GUI.MainForm.mnuUnitCommandItem(ItemCmdID).Visible = true;
                //        break;
                //    }
                //}

                //if (currentUnit.Area == "地中")
                //{
                //    GUI.MainForm.mnuUnitCommandItem(ItemCmdID).Visible = false;
                //}

                //{
                //    var withBlock20 = GUI.MainForm;
                //    withBlock20.mnuUnitCommandItem(DismissCmdID).Visible = false;
                //    withBlock20.mnuUnitCommandItem(OrderCmdID).Visible = false;
                //    withBlock20.mnuUnitCommandItem(FeatureListCmdID).Visible = false;
                //    withBlock20.mnuUnitCommandItem(WeaponListCmdID).Visible = false;
                //    withBlock20.mnuUnitCommandItem(AbilityListCmdID).Visible = false;
                //    withBlock20.mnuUnitCommandItem(UnitCommand1CmdID).Visible = false;
                //    withBlock20.mnuUnitCommandItem(UnitCommand2CmdID).Visible = false;
                //    withBlock20.mnuUnitCommandItem(UnitCommand3CmdID).Visible = false;
                //    withBlock20.mnuUnitCommandItem(UnitCommand4CmdID).Visible = false;
                //    withBlock20.mnuUnitCommandItem(UnitCommand5CmdID).Visible = false;
                //    withBlock20.mnuUnitCommandItem(UnitCommand6CmdID).Visible = false;
                //    withBlock20.mnuUnitCommandItem(UnitCommand7CmdID).Visible = false;
                //    withBlock20.mnuUnitCommandItem(UnitCommand8CmdID).Visible = false;
                //    withBlock20.mnuUnitCommandItem(UnitCommand9CmdID).Visible = false;
                //    withBlock20.mnuUnitCommandItem(UnitCommand10CmdID).Visible = false;
                //}

                //// ユニットコマンド
                //i = UnitCommand1CmdID;
                //foreach (LabelData currentLab4 in Event.colEventLabelList)
                //{
                //    lab = currentLab4;
                //    if (lab.Name == Event.LabelType.UnitCommandEventLabel & lab.AsterNum >= 2)
                //    {
                //        if (lab.Enable)
                //        {
                //            buf = lab.Para(3);
                //            if (SelectedUnit.Party == "味方" & ((buf ?? "") == (SelectedUnit.MainPilot().Name ?? "") | (buf ?? "") == (SelectedUnit.MainPilot().get_Nickname(false) ?? "") | (buf ?? "") == (SelectedUnit.Name ?? "")) | (buf ?? "") == (SelectedUnit.Party ?? "") | buf == "全")
                //            {
                //                int localStrToLng3() { string argexpr = lab.Para(4); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                //                if (lab.CountPara() <= 3)
                //                {
                //                    GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
                //                }
                //                else if (localStrToLng3() != 0)
                //                {
                //                    GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
                //                }
                //            }
                //        }

                //        if (GUI.MainForm.mnuUnitCommandItem(i).Visible)
                //        {
                //            GUI.MainForm.mnuUnitCommandItem(i).Caption = lab.Para(2);
                //            UnitCommandLabelList[i - UnitCommand1CmdID + 1] = lab.LineNum.ToString();
                //            i = (i + 1);
                //            if (i > UnitCommand10CmdID)
                //            {
                //                break;
                //            }
                //        }
                //    }
                //}
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
            //    while (CommandState == "移動後コマンド選択" & SelectedCommand == "移動")
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

            // TODO
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
                    //if (SelectedCommand == "スペシャルパワー")
                    //{
                    //}
                    //// 下に抜ける
                    //else if (SelectedCommand == "アビリティ" | SelectedCommand == "マップアビリティ" | SelectedCommand == "アイテム" | SelectedCommand == "マップアイテム")
                    //{
                    //    if (SelectedUnit.AbilityMinRange(SelectedAbility) > 0)
                    //    {
                    //        // 自分自身は選択不可
                    //        GUI.IsGUILocked = false;
                    //        return;
                    //    }
                    //}
                    //else if (SelectedCommand == "移動命令")
                    //{
                    //}
                    //// 下に抜ける
                    //else
                    //{
                    //    // 自分自身は選択不可
                    //    GUI.IsGUILocked = false;
                    //    return;
                    //}
                }

                // 場所を選択するコマンド
                switch (SelectedCommand ?? "")
                {
                    case "移動":
                    case "再移動":
                        FinishMoveCommand();
                        GUI.IsGUILocked = false;
                        return;

                    //case "テレポート":
                    //        FinishTeleportCommand();
                    //        GUI.IsGUILocked = false;
                    //        return;

                    //case "ジャンプ":
                    //        FinishJumpCommand();
                    //        GUI.IsGUILocked = false;
                    //        return;

                    //case "マップ攻撃":
                    //        MapAttackCommand();
                    //        GUI.IsGUILocked = false;
                    //        return;

                    //case "マップアビリティ":
                    //case "マップアイテム":
                    //        MapAbilityCommand();
                    //        GUI.IsGUILocked = false;
                    //        return;

                    case "発進":
                        FinishLaunchCommand();
                        GUI.IsGUILocked = false;
                        return;

                        //case "移動命令":
                        //        FinishOrderCommand();
                        //        GUI.IsGUILocked = false;
                        //        return;
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

                        //    case "アビリティ":
                        //    case "アイテム":
                        //        FinishAbilityCommand();
                        //        break;

                        //    case "会話":
                        //        FinishTalkCommand();
                        //        break;

                        //    case "修理":
                        //        FinishFixCommand();
                        //        break;

                        //    case "補給":
                        //        FinishSupplyCommand();
                        //        break;

                        //    case "スペシャルパワー":
                        //        FinishSpecialPowerCommand();
                        //        break;

                        //    case "攻撃命令":
                        //    case "護衛命令":
                        //        FinishOrderCommand();
                        //        break;
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

            //if (1 <= GUI.PixelToMapX(GUI.MouseX) & GUI.PixelToMapX(GUI.MouseX) <= Map.MapWidth)
            //{
            //    if (1 <= GUI.PixelToMapY(GUI.MouseY) & GUI.PixelToMapY(GUI.MouseY) <= Map.MapHeight)
            //    {
            //        if (!Map.MaskData[GUI.PixelToMapX(GUI.MouseX), GUI.PixelToMapY(GUI.MouseY)])
            //        {
            //            // 効果範囲内でクリックされればマップ攻撃発動
            //            if (SelectedCommand == "マップ攻撃")
            //            {
            //                MapAttackCommand();
            //            }
            //            else
            //            {
            //                MapAbilityCommand();
            //            }
            //        }
            //    }
            //}
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

                        //case "テレポート":
                        //    StartTeleportCommand();
                        //    break;

                        //case "ジャンプ":
                        //    StartJumpCommand();
                        //    break;

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

                    //if (SelectedCommand == "マップ攻撃")
                    //{
                    //    string argattr = "Ｍ直";
                    //    string argattr1 = "Ｍ移";
                    //    if (currentUnit.IsWeaponClassifiedAs(SelectedWeapon, argattr))
                    //    {
                    //        Map.AreaInCross(currentUnit.x, currentUnit.y, currentUnit.WeaponMaxRange(SelectedWeapon), currentUnit.Weapon(SelectedWeapon).MinRange);
                    //    }
                    //    else if (currentUnit.IsWeaponClassifiedAs(SelectedWeapon, argattr1))
                    //    {
                    //        Map.AreaInMoveAction(SelectedUnit, currentUnit.WeaponMaxRange(SelectedWeapon));
                    //    }
                    //    else
                    //    {
                    //        string arguparty = "すべて";
                    //        Map.AreaInRange(currentUnit.x, currentUnit.y, currentUnit.WeaponMaxRange(SelectedWeapon), currentUnit.Weapon(SelectedWeapon).MinRange, arguparty);
                    //    }
                    //}
                    //else
                    //{
                    //    string argattr2 = "Ｍ直";
                    //    string argattr3 = "Ｍ移";
                    //    if (currentUnit.IsAbilityClassifiedAs(SelectedAbility, argattr2))
                    //    {
                    //        int argmax_range = currentUnit.AbilityMinRange(SelectedAbility);
                    //        Map.AreaInCross(currentUnit.x, currentUnit.y, currentUnit.AbilityMaxRange(SelectedAbility), argmax_range);
                    //    }
                    //    else if (currentUnit.IsAbilityClassifiedAs(SelectedAbility, argattr3))
                    //    {
                    //        Map.AreaInMoveAction(SelectedUnit, currentUnit.AbilityMaxRange(SelectedAbility));
                    //    }
                    //    else
                    //    {
                    //        string arguparty1 = "すべて";
                    //        Map.AreaInRange(currentUnit.x, currentUnit.y, currentUnit.AbilityMaxRange(SelectedAbility), currentUnit.AbilityMinRange(SelectedAbility), arguparty1);
                    //    }
                    //}

                    GUI.MaskScreen();
                    break;
            }
        }
    }
}
