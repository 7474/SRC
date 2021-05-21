// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Units;
using System;

namespace SRCCore.Commands
{
    public partial class Command
    {
        // 「アビリティ」コマンドを開始
        // is_item=True の場合は「アイテム」コマンドによる使い捨てアイテムのアビリティ
        private void StartAbilityCommand(bool is_item = false)
        {
            string cap;
            GUI.LockGUI();

            // 使用するアビリティを選択
            if (is_item)
            {
                cap = "アイテム選択";
            }
            else
            {
                cap = Expression.Term("アビリティ", SelectedUnit) + "選択";
            }

            UnitAbility unitAbility;
            if (CommandState == "コマンド選択")
            {
                unitAbility = GUI.AbilityListBox(SelectedUnit, new UnitAbilityList(AbilityListMode.BeforeMove, SelectedUnit), cap, "移動前", is_item);
            }
            else
            {
                unitAbility = GUI.AbilityListBox(SelectedUnit, new UnitAbilityList(AbilityListMode.AfterMove, SelectedUnit), cap, "移動後", is_item);
            }

            // キャンセル
            if (unitAbility == null)
            {
                SelectedAbility = 0;
                if (SRC.AutoMoveCursor)
                {
                    GUI.RestoreCursorPos();
                }

                CancelCommand();
                GUI.UnlockGUI();
                return;
            }
            SelectedAbility = unitAbility.AbilityNo();

            throw new NotImplementedException();
            //int i, j;
            //Unit t;
            //int min_range, max_range;

            //// アビリティ専用ＢＧＭがあればそれを演奏
            //string BGM;
            //{
            //    var withBlock = SelectedUnit;
            //    if (withBlock.IsFeatureAvailable("アビリティＢＧＭ"))
            //    {
            //        var loopTo = withBlock.CountFeature();
            //        for (i = 1; i <= loopTo; i++)
            //        {
            //            string localFeature() { object argIndex1 = i; var ret = withBlock.Feature(argIndex1); return ret; }

            //            string localFeatureData2() { object argIndex1 = i; var ret = withBlock.FeatureData(argIndex1); return ret; }

            //            string localLIndex() { string arglist = hs8866f19275ca4c5cbfc3bb7415f2da30(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //            if (localFeature() == "アビリティＢＧＭ" & (localLIndex() ?? "") == (withBlock.Ability(SelectedAbility).Name ?? ""))
            //            {
            //                string localFeatureData() { object argIndex1 = i; var ret = withBlock.FeatureData(argIndex1); return ret; }

            //                string localFeatureData1() { object argIndex1 = i; var ret = withBlock.FeatureData(argIndex1); return ret; }

            //                BGM = Sound.SearchMidiFile(Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1));
            //                if (Strings.Len(BGM) > 0)
            //                {
            //                    Sound.ChangeBGM(BGM);
            //                }

            //                break;
            //            }
            //        }
            //    }
            //}

            //// 射程0のアビリティはその場で実行
            //var is_transformation = default(bool);
            //if (SelectedUnit.Ability(SelectedAbility).MaxRange == 0)
            //{
            //    SelectedTarget = SelectedUnit;

            //    // 変身アビリティであるか判定
            //    var loopTo1 = SelectedUnit.Ability(SelectedAbility).CountEffect();
            //    for (i = 1; i <= loopTo1; i++)
            //    {
            //        if (SelectedUnit.Ability(SelectedAbility).EffectType(i) == "変身")
            //        {
            //            is_transformation = true;
            //            break;
            //        }
            //    }

            //    SelectedAbilityName = SelectedUnit.Ability(SelectedAbility).Name;

            //    // 使用イベント
            //    Event.HandleEvent("使用", SelectedUnit.MainPilot().ID, SelectedAbilityName);
            //    if (SRC.IsScenarioFinished)
            //    {
            //        SRC.IsScenarioFinished = false;
            //        GUI.UnlockGUI();
            //        return;
            //    }

            //    if (SRC.IsCanceled)
            //    {
            //        SRC.IsCanceled = false;
            //        WaitCommand();
            //        return;
            //    }

            //    // アビリティを実行
            //    SelectedUnit.ExecuteAbility(SelectedAbility, SelectedUnit);
            //    SelectedUnit = SelectedUnit.CurrentForm();
            //    GUI.CloseMessageForm();

            //    // 破壊イベント
            //    {
            //        var withBlock1 = SelectedUnit;
            //        if (withBlock1.Status == "破壊")
            //        {
            //            if (withBlock1.CountPilot() > 0)
            //            {
            //                Event.HandleEvent("破壊", withBlock1.MainPilot().ID);
            //                if (SRC.IsScenarioFinished)
            //                {
            //                    SRC.IsScenarioFinished = false;
            //                    GUI.UnlockGUI();
            //                    return;
            //                }

            //                if (SRC.IsCanceled)
            //                {
            //                    SRC.IsCanceled = false;
            //                    GUI.UnlockGUI();
            //                    return;
            //                }
            //            }

            //            WaitCommand();
            //            return;
            //        }
            //    }

            //    // 使用後イベント
            //    {
            //        var withBlock2 = SelectedUnit;
            //        if (withBlock2.CountPilot() > 0)
            //        {
            //            Event.HandleEvent("使用後", withBlock2.MainPilot().ID, SelectedAbilityName);
            //            if (SRC.IsScenarioFinished)
            //            {
            //                SRC.IsScenarioFinished = false;
            //                GUI.UnlockGUI();
            //                return;
            //            }

            //            if (SRC.IsCanceled)
            //            {
            //                SRC.IsCanceled = false;
            //                GUI.UnlockGUI();
            //                return;
            //            }
            //        }
            //    }

            //    // 変身アビリティの場合は行動終了しない
            //    if (!is_transformation | CommandState == "移動後コマンド選択")
            //    {
            //        WaitCommand();
            //    }
            //    else
            //    {
            //        if (SelectedUnit.Status == "出撃")
            //        {
            //            // カーソル自動移動
            //            if (SRC.AutoMoveCursor)
            //            {
            //                GUI.MoveCursorPos("ユニット選択", SelectedUnit);
            //            }

            //            Status.DisplayUnitStatus(SelectedUnit);
            //        }
            //        else
            //        {
            //            Status.ClearUnitStatus();
            //        }

            //        CommandState = "ユニット選択";
            //        GUI.UnlockGUI();
            //    }

            //    return;
            //}

            //var partners = default(Unit[]);
            //{
            //    var withBlock3 = SelectedUnit;
            //    // マップ型アビリティかどうかで今後のコマンド処理の進行の仕方が異なる
            //    if (is_item)
            //    {
            //        if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, "Ｍ"))
            //        {
            //            SelectedCommand = "マップアイテム";
            //        }
            //        else
            //        {
            //            SelectedCommand = "アイテム";
            //        }
            //    }
            //    else
            //    {
            //        if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, "Ｍ"))
            //        {
            //            SelectedCommand = "マップアビリティ";
            //        }
            //        else
            //        {
            //            SelectedCommand = "アビリティ";
            //        }
            //    }

            //    // アビリティの射程を求めておく
            //    min_range = withBlock3.AbilityMinRange(SelectedAbility);
            //    max_range = withBlock3.AbilityMaxRange(SelectedAbility);

            //    // アビリティの効果範囲を設定
            //    if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, "Ｍ直"))
            //    {
            //        Map.AreaInCross(withBlock3.x, withBlock3.y, min_range, max_range);
            //    }
            //    else if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, "Ｍ拡"))
            //    {
            //        Map.AreaInWideCross(withBlock3.x, withBlock3.y, min_range, max_range);
            //    }
            //    else if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, "Ｍ扇"))
            //    {
            //        Map.AreaInSectorCross(withBlock3.x, withBlock3.y, min_range, max_range, withBlock3.AbilityLevel(SelectedAbility, "Ｍ扇"));
            //    }
            //    else if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, "Ｍ移"))
            //    {
            //        Map.AreaInMoveAction(SelectedUnit, max_range);
            //    }
            //    else
            //    {
            //        Map.AreaInRange(withBlock3.x, withBlock3.y, max_range, min_range, "すべて");
            //    }

            //    // 射程１の合体技はパートナーで相手を取り囲んでいないと使用できない
            //    if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, "合") & !withBlock3.IsAbilityClassifiedAs(SelectedAbility, "Ｍ") & withBlock3.Ability(SelectedAbility).MaxRange == 1)
            //    {
            //        for (i = 1; i <= 4; i++)
            //        {
            //            // UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //            t = null;
            //            switch (i)
            //            {
            //                case 1:
            //                    {
            //                        if (withBlock3.x > 1)
            //                        {
            //                            t = Map.MapDataForUnit[withBlock3.x - 1, withBlock3.y];
            //                        }

            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        if (withBlock3.x < Map.MapWidth)
            //                        {
            //                            t = Map.MapDataForUnit[withBlock3.x + 1, withBlock3.y];
            //                        }

            //                        break;
            //                    }

            //                case 3:
            //                    {
            //                        if (withBlock3.y > 1)
            //                        {
            //                            t = Map.MapDataForUnit[withBlock3.x, withBlock3.y - 1];
            //                        }

            //                        break;
            //                    }

            //                case 4:
            //                    {
            //                        if (withBlock3.y < Map.MapHeight)
            //                        {
            //                            t = Map.MapDataForUnit[withBlock3.x, withBlock3.y + 1];
            //                        }

            //                        break;
            //                    }
            //            }

            //            if (t is object)
            //            {
            //                if (withBlock3.IsAlly(t))
            //                {
            //                    withBlock3.CombinationPartner("アビリティ", SelectedAbility, partners, t.x, t.y);
            //                    if (Information.UBound(partners) == 0)
            //                    {
            //                        Map.MaskData[t.x, t.y] = true;
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // ユニットがいるマスの処理
            //    if (!withBlock3.IsAbilityClassifiedAs(SelectedAbility, "Ｍ投") & !withBlock3.IsAbilityClassifiedAs(SelectedAbility, "Ｍ線") & !withBlock3.IsAbilityClassifiedAs(SelectedAbility, "Ｍ移"))
            //    {
            //        var loopTo2 = GeneralLib.MinLng(withBlock3.x + max_range, Map.MapWidth);
            //        for (i = GeneralLib.MaxLng(withBlock3.x - max_range, 1); i <= loopTo2; i++)
            //        {
            //            var loopTo3 = GeneralLib.MinLng(withBlock3.y + max_range, Map.MapHeight);
            //            for (j = GeneralLib.MaxLng(withBlock3.y - max_range, 1); j <= loopTo3; j++)
            //            {
            //                if (!Map.MaskData[i, j])
            //                {
            //                    t = Map.MapDataForUnit[i, j];
            //                    if (t is object)
            //                    {
            //                        // 有効？
            //                        if (withBlock3.IsAbilityEffective(SelectedAbility, t))
            //                        {
            //                            Map.MaskData[i, j] = false;
            //                        }
            //                        else
            //                        {
            //                            Map.MaskData[i, j] = true;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // 支援専用アビリティは自分には使用できない
            //    if (!Map.MaskData[withBlock3.x, withBlock3.y])
            //    {
            //        if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, "援"))
            //        {
            //            Map.MaskData[withBlock3.x, withBlock3.y] = true;
            //        }
            //    }

            //    if (!Expression.IsOptionDefined("大型マップ"))
            //    {
            //        GUI.Center(withBlock3.x, withBlock3.y);
            //    }

            //    GUI.MaskScreen();
            //}

            //if (CommandState == "コマンド選択")
            //{
            //    CommandState = "ターゲット選択";
            //}
            //else
            //{
            //    CommandState = "移動後ターゲット選択";
            //}

            //// カーソル自動移動を行う？
            //if (!SRC.AutoMoveCursor)
            //{
            //    GUI.UnlockGUI();
            //    return;
            //}

            //// 自分から最も近い味方ユニットを探す
            //// UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //t = null;
            //foreach (Unit u in SRC.UList)
            //{
            //    if (u.Status == "出撃" & u.Party == "味方")
            //    {
            //        if (Map.MaskData[u.x, u.y] == false & !ReferenceEquals(u, SelectedUnit))
            //        {
            //            if (t is null)
            //            {
            //                t = u;
            //            }
            //            else if (Math.Pow(Math.Abs((SelectedUnit.x - u.x)), 2d) + Math.Pow(Math.Abs((SelectedUnit.y - u.y)), 2d) < Math.Pow(Math.Abs((SelectedUnit.x - t.x)), 2d) + Math.Pow(Math.Abs((SelectedUnit.y - t.y)), 2d))
            //            {
            //                t = u;
            //            }
            //        }
            //    }
            //}

            //// 適当がユニットがなければ自分自身を選択
            //if (t is null)
            //{
            //    t = SelectedUnit;
            //}

            //// カーソルを移動
            //GUI.MoveCursorPos("ユニット選択", t);

            //// ターゲットのステータスを表示
            //if (!ReferenceEquals(SelectedUnit, t))
            //{
            //    Status.DisplayUnitStatus(t);
            //}

            //GUI.UnlockGUI();
        }

        // 「アビリティ」コマンドを終了
        private void FinishAbilityCommand()
        {
            throw new NotImplementedException();
            //int i;
            //var partners = default(Unit[]);
            //string aname;
            //// ADD START MARGE
            //bool is_p_ability;
            //// ADD END MARGE

            //// MOD START MARGE
            //// If MainWidth <> 15 Then
            //if (GUI.NewGUIMode)
            //{
            //    // MOD END MARGE
            //    Status.ClearUnitStatus();
            //}

            //GUI.LockGUI();

            //// 合体技のパートナーを設定
            //{
            //    var withBlock = SelectedUnit;
            //    if (withBlock.IsAbilityClassifiedAs(SelectedAbility, "合"))
            //    {
            //        if (withBlock.AbilityMaxRange(SelectedAbility) == 1)
            //        {
            //            withBlock.CombinationPartner("アビリティ", SelectedAbility, partners, SelectedTarget.x, SelectedTarget.y);
            //        }
            //        else
            //        {
            //            withBlock.CombinationPartner("アビリティ", SelectedAbility, partners);
            //        }
            //    }
            //    else
            //    {
            //        SelectedPartners.Clear();
            //        partners = new Unit[1];
            //    }
            //}

            //aname = SelectedUnit.Ability(SelectedAbility).Name;
            //SelectedAbilityName = aname;

            //// ADD START MARGE
            //// 移動後使用後可能なアビリティか記録しておく
            //is_p_ability = SelectedUnit.IsAbilityClassifiedAs(SelectedAbility, "Ｐ") | SelectedUnit.AbilityMaxRange(SelectedAbility) == 1 & !SelectedUnit.IsAbilityClassifiedAs(SelectedAbility, "Ｑ");
            //// ADD END MARGE

            //// 使用イベント
            //Event.HandleEvent("使用", SelectedUnit.MainPilot().ID, aname);
            //if (SRC.IsScenarioFinished)
            //{
            //    SRC.IsScenarioFinished = false;
            //    SelectedPartners.Clear();
            //    GUI.UnlockGUI();
            //    return;
            //}

            //if (SRC.IsCanceled)
            //{
            //    SRC.IsCanceled = false;
            //    SelectedPartners.Clear();
            //    WaitCommand();
            //    return;
            //}

            //{
            //    var withBlock1 = SelectedUnit;
            //    foreach (Unit u in SRC.UList)
            //    {
            //        if (u.Status == "出撃")
            //        {
            //            Map.MaskData[u.x, u.y] = true;
            //        }
            //    }

            //    // 合体技パートナーのハイライト表示
            //    if (withBlock1.IsAbilityClassifiedAs(SelectedAbility, "合"))
            //    {
            //        var loopTo = Information.UBound(partners);
            //        for (i = 1; i <= loopTo; i++)
            //        {
            //            {
            //                var withBlock2 = partners[i];
            //                Map.MaskData[withBlock2.x, withBlock2.y] = false;
            //            }
            //        }
            //    }

            //    Map.MaskData[withBlock1.x, withBlock1.y] = false;
            //    Map.MaskData[SelectedTarget.x, SelectedTarget.y] = false;
            //    if (!SRC.BattleAnimation)
            //    {
            //        GUI.MaskScreen();
            //    }

            //    // アビリティを実行
            //    withBlock1.ExecuteAbility(SelectedAbility, SelectedTarget);
            //    SelectedUnit = withBlock1.CurrentForm();
            //    GUI.CloseMessageForm();
            //    Status.ClearUnitStatus();
            //}

            //// 破壊イベント
            //{
            //    var withBlock3 = SelectedUnit;
            //    if (withBlock3.Status == "破壊")
            //    {
            //        if (withBlock3.CountPilot() > 0)
            //        {
            //            Event.HandleEvent("破壊", withBlock3.MainPilot().ID);
            //            if (SRC.IsScenarioFinished)
            //            {
            //                SRC.IsScenarioFinished = false;
            //                SelectedPartners.Clear();
            //                GUI.UnlockGUI();
            //                return;
            //            }

            //            if (SRC.IsCanceled)
            //            {
            //                SRC.IsCanceled = false;
            //                SelectedPartners.Clear();
            //                GUI.UnlockGUI();
            //                return;
            //            }
            //        }

            //        WaitCommand();
            //        return;
            //    }
            //}

            //// 使用後イベント
            //{
            //    var withBlock4 = SelectedUnit;
            //    if (withBlock4.CountPilot() > 0)
            //    {
            //        Event.HandleEvent("使用後", withBlock4.MainPilot().ID, aname);
            //        if (SRC.IsScenarioFinished)
            //        {
            //            SRC.IsScenarioFinished = false;
            //            SelectedPartners.Clear();
            //            GUI.UnlockGUI();
            //            return;
            //        }

            //        if (SRC.IsCanceled)
            //        {
            //            SRC.IsCanceled = false;
            //            SelectedPartners.Clear();
            //            GUI.UnlockGUI();
            //            return;
            //        }
            //    }
            //}

            //// 合体技のパートナーの行動数を減らす
            //if (!Expression.IsOptionDefined("合体技パートナー行動数無消費"))
            //{
            //    var loopTo1 = Information.UBound(partners);
            //    for (i = 1; i <= loopTo1; i++)
            //        partners[i].CurrentForm().UseAction();
            //}

            //SelectedPartners.Clear();

            //// ADD START MARGE
            //// 再移動
            //if (is_p_ability & SelectedUnit.Status == "出撃")
            //{
            //    if (SelectedUnit.MainPilot().IsSkillAvailable("遊撃") & SelectedUnit.Speed * 2 > SelectedUnitMoveCost)
            //    {
            //        // 進入イベント
            //        if (SelectedUnitMoveCost > 0)
            //        {
            //            Event.HandleEvent("進入", SelectedUnit.MainPilot().ID, SelectedUnit.x, SelectedUnit.y);
            //            if (SRC.IsScenarioFinished)
            //            {
            //                SRC.IsScenarioFinished = false;
            //                return;
            //            }
            //        }

            //        // ユニットが既に出撃していない？
            //        if (SelectedUnit.Status != "出撃")
            //        {
            //            GUI.RedrawScreen();
            //            Status.ClearUnitStatus();
            //            return;
            //        }

            //        SelectedCommand = "再移動";
            //        Map.AreaInSpeed(SelectedUnit);
            //        if (!Expression.IsOptionDefined("大型マップ"))
            //        {
            //            GUI.Center(SelectedUnit.x, SelectedUnit.y);
            //        }

            //        GUI.MaskScreen();
            //        if (GUI.NewGUIMode)
            //        {
            //            Application.DoEvents();
            //            Status.ClearUnitStatus();
            //        }
            //        else
            //        {
            //            Status.DisplayUnitStatus(SelectedUnit);
            //        }

            //        CommandState = "ターゲット選択";
            //        return;
            //    }
            //}
            //// ADD END MARGE

            //// 行動終了
            //WaitCommand();
        }

        // マップ型「アビリティ」コマンドを終了
        private void MapAbilityCommand()
        {
            throw new NotImplementedException();
            //int i;
            //var partners = default(Unit[]);
            //// ADD START MARGE
            //bool is_p_ability;
            //// ADD END MARGE

            //{
            //    var withBlock = SelectedUnit;
            //    // ADD START MARGE
            //    // 移動後使用後可能なアビリティか記録しておく
            //    is_p_ability = withBlock.IsAbilityClassifiedAs(SelectedAbility, "Ｐ") | withBlock.AbilityMaxRange(SelectedAbility) == 1 & !withBlock.IsAbilityClassifiedAs(SelectedAbility, "Ｑ");
            //    // ADD END MARGE

            //    // 目標地点を選択して初めて効果範囲が分かるタイプのマップアビリティ
            //    // の場合は再度プレイヤーの選択を促す必要がある
            //    if (CommandState == "ターゲット選択" | CommandState == "移動後ターゲット選択")
            //    {
            //        if (withBlock.IsAbilityClassifiedAs(SelectedAbility, "Ｍ投"))
            //        {
            //            if (CommandState == "ターゲット選択")
            //            {
            //                CommandState = "マップ攻撃使用";
            //            }
            //            else
            //            {
            //                CommandState = "移動後マップ攻撃使用";
            //            }

            //            // 目標地点
            //            SelectedX = GUI.PixelToMapX(GUI.MouseX);
            //            SelectedY = GUI.PixelToMapY(GUI.MouseY);

            //            // 効果範囲を設定
            //            Map.AreaInRange(SelectedX, SelectedY, withBlock.AbilityLevel(SelectedAbility, "Ｍ投"), 1, "味方");
            //            GUI.MaskScreen();
            //            return;
            //        }
            //        else if (withBlock.IsAbilityClassifiedAs(SelectedAbility, "Ｍ移"))
            //        {
            //            SelectedX = GUI.PixelToMapX(GUI.MouseX);
            //            SelectedY = GUI.PixelToMapY(GUI.MouseY);
            //            if (Map.MapDataForUnit[SelectedX, SelectedY] is object)
            //            {
            //                GUI.MaskScreen();
            //                return;
            //            }

            //            // 目標地点
            //            if (CommandState == "ターゲット選択")
            //            {
            //                CommandState = "マップ攻撃使用";
            //            }
            //            else
            //            {
            //                CommandState = "移動後マップ攻撃使用";
            //            }

            //            // 効果範囲を設定
            //            Map.AreaInPointToPoint(withBlock.x, withBlock.y, SelectedX, SelectedY);
            //            GUI.MaskScreen();
            //            return;
            //        }
            //        else if (withBlock.IsAbilityClassifiedAs(SelectedAbility, "Ｍ線"))
            //        {
            //            if (CommandState == "ターゲット選択")
            //            {
            //                CommandState = "マップ攻撃使用";
            //            }
            //            else
            //            {
            //                CommandState = "移動後マップ攻撃使用";
            //            }

            //            // 目標地点
            //            SelectedX = GUI.PixelToMapX(GUI.MouseX);
            //            SelectedY = GUI.PixelToMapY(GUI.MouseY);

            //            // 効果範囲を設定
            //            Map.AreaInPointToPoint(withBlock.x, withBlock.y, SelectedX, SelectedY);
            //            GUI.MaskScreen();
            //            return;
            //        }
            //    }

            //    // 合体技パートナーの設定
            //    if (withBlock.IsAbilityClassifiedAs(SelectedAbility, "合"))
            //    {
            //        withBlock.CombinationPartner("アビリティ", SelectedAbility, partners);
            //    }
            //    else
            //    {
            //        SelectedPartners.Clear();
            //        partners = new Unit[1];
            //    }
            //}

            //if (GUI.MainWidth != 15)
            //{
            //    Status.ClearUnitStatus();
            //}

            //GUI.LockGUI();

            //// アビリティを実行
            //SelectedUnit.ExecuteMapAbility(SelectedAbility, SelectedX, SelectedY);
            //SelectedUnit = SelectedUnit.CurrentForm();
            //// UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //SelectedTarget = null;
            //if (SRC.IsScenarioFinished)
            //{
            //    SRC.IsScenarioFinished = false;
            //    SelectedPartners.Clear();
            //    GUI.UnlockGUI();
            //    return;
            //}

            //if (SRC.IsCanceled)
            //{
            //    SRC.IsCanceled = false;
            //    SelectedPartners.Clear();
            //    WaitCommand();
            //    return;
            //}

            //// 合体技のパートナーの行動数を減らす
            //if (!Expression.IsOptionDefined("合体技パートナー行動数無消費"))
            //{
            //    var loopTo = Information.UBound(partners);
            //    for (i = 1; i <= loopTo; i++)
            //        partners[i].CurrentForm().UseAction();
            //}

            //SelectedPartners.Clear();

            //// ADD START MARGE
            //// 再移動
            //if (is_p_ability & SelectedUnit.Status == "出撃")
            //{
            //    if (SelectedUnit.MainPilot().IsSkillAvailable("遊撃") & SelectedUnit.Speed * 2 > SelectedUnitMoveCost)
            //    {
            //        // 進入イベント
            //        if (SelectedUnitMoveCost > 0)
            //        {
            //            Event.HandleEvent("進入", SelectedUnit.MainPilot().ID, SelectedUnit.x, SelectedUnit.y);
            //            if (SRC.IsScenarioFinished)
            //            {
            //                SRC.IsScenarioFinished = false;
            //                return;
            //            }
            //        }

            //        // ユニットが既に出撃していない？
            //        if (SelectedUnit.Status != "出撃")
            //        {
            //            GUI.RedrawScreen();
            //            Status.ClearUnitStatus();
            //            return;
            //        }

            //        SelectedCommand = "再移動";
            //        Map.AreaInSpeed(SelectedUnit);
            //        if (!Expression.IsOptionDefined("大型マップ"))
            //        {
            //            GUI.Center(SelectedUnit.x, SelectedUnit.y);
            //        }

            //        GUI.MaskScreen();
            //        if (GUI.NewGUIMode)
            //        {
            //            Application.DoEvents();
            //            Status.ClearUnitStatus();
            //        }
            //        else
            //        {
            //            Status.DisplayUnitStatus(SelectedUnit);
            //        }

            //        CommandState = "ターゲット選択";
            //        return;
            //    }
            //}
            //// ADD END MARGE

            //// 行動終了
            //WaitCommand();
        }
    }
}
