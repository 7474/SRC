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
        // 「アビリティ」コマンドを開始
        // is_item=True の場合は「アイテム」コマンドによる使い捨てアイテムのアビリティ
        private void StartAbilityCommand(bool is_item = false)
        {
            throw new NotImplementedException();
            //int i, j;
            //Unit t;
            //int min_range, max_range;
            //string cap;
            //GUI.LockGUI();

            //// 使用するアビリティを選択
            //if (is_item)
            //{
            //    cap = "アイテム選択";
            //}
            //else
            //{
            //    string argtname = "アビリティ";
            //    cap = Expression.Term(argtname, SelectedUnit) + "選択";
            //}

            //if (CommandState == "コマンド選択")
            //{
            //    string arglb_mode = "移動前";
            //    SelectedAbility = GUI.AbilityListBox(SelectedUnit, cap, arglb_mode, is_item);
            //}
            //else
            //{
            //    string arglb_mode1 = "移動後";
            //    SelectedAbility = GUI.AbilityListBox(SelectedUnit, cap, arglb_mode1, is_item);
            //}

            //// キャンセル
            //if (SelectedAbility == 0)
            //{
            //    if (SRC.AutoMoveCursor)
            //    {
            //        GUI.RestoreCursorPos();
            //    }

            //    CancelCommand();
            //    GUI.UnlockGUI();
            //    return;
            //}

            //// アビリティ専用ＢＧＭがあればそれを演奏
            //string BGM;
            //{
            //    var withBlock = SelectedUnit;
            //    string argfname = "アビリティＢＧＭ";
            //    if (withBlock.IsFeatureAvailable(argfname))
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

            //                string argmidi_name = Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1);
            //                BGM = Sound.SearchMidiFile(argmidi_name);
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
            //        object argIndex1 = i;
            //        if (SelectedUnit.Ability(SelectedAbility).EffectType(argIndex1) == "変身")
            //        {
            //            is_transformation = true;
            //            break;
            //        }
            //    }

            //    SelectedAbilityName = SelectedUnit.Ability(SelectedAbility).Name;

            //    // 使用イベント
            //    Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, SelectedAbilityName);
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
            //        if (withBlock1.Status_Renamed == "破壊")
            //        {
            //            if (withBlock1.CountPilot() > 0)
            //            {
            //                Event_Renamed.HandleEvent("破壊", withBlock1.MainPilot().ID);
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
            //            Event_Renamed.HandleEvent("使用後", withBlock2.MainPilot().ID, SelectedAbilityName);
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
            //        if (SelectedUnit.Status_Renamed == "出撃")
            //        {
            //            // カーソル自動移動
            //            if (SRC.AutoMoveCursor)
            //            {
            //                string argcursor_mode = "ユニット選択";
            //                GUI.MoveCursorPos(argcursor_mode, SelectedUnit);
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
            //        string argattr = "Ｍ";
            //        if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, argattr))
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
            //        string argattr1 = "Ｍ";
            //        if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, argattr1))
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
            //    string argattr3 = "Ｍ直";
            //    string argattr4 = "Ｍ拡";
            //    string argattr5 = "Ｍ扇";
            //    string argattr6 = "Ｍ移";
            //    if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, argattr3))
            //    {
            //        Map.AreaInCross(withBlock3.x, withBlock3.y, min_range, max_range);
            //    }
            //    else if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, argattr4))
            //    {
            //        Map.AreaInWideCross(withBlock3.x, withBlock3.y, min_range, max_range);
            //    }
            //    else if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, argattr5))
            //    {
            //        string argattr2 = "Ｍ扇";
            //        Map.AreaInSectorCross(withBlock3.x, withBlock3.y, min_range, max_range, withBlock3.AbilityLevel(SelectedAbility, argattr2));
            //    }
            //    else if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, argattr6))
            //    {
            //        Map.AreaInMoveAction(SelectedUnit, max_range);
            //    }
            //    else
            //    {
            //        string arguparty = "すべて";
            //        Map.AreaInRange(withBlock3.x, withBlock3.y, max_range, min_range, arguparty);
            //    }

            //    // 射程１の合体技はパートナーで相手を取り囲んでいないと使用できない
            //    string argattr7 = "合";
            //    string argattr8 = "Ｍ";
            //    if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, argattr7) & !withBlock3.IsAbilityClassifiedAs(SelectedAbility, argattr8) & withBlock3.Ability(SelectedAbility).MaxRange == 1)
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
            //                    string argctype_Renamed = "アビリティ";
            //                    withBlock3.CombinationPartner(argctype_Renamed, SelectedAbility, partners, t.x, t.y);
            //                    if (Information.UBound(partners) == 0)
            //                    {
            //                        Map.MaskData[t.x, t.y] = true;
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // ユニットがいるマスの処理
            //    string argattr9 = "Ｍ投";
            //    string argattr10 = "Ｍ線";
            //    string argattr11 = "Ｍ移";
            //    if (!withBlock3.IsAbilityClassifiedAs(SelectedAbility, argattr9) & !withBlock3.IsAbilityClassifiedAs(SelectedAbility, argattr10) & !withBlock3.IsAbilityClassifiedAs(SelectedAbility, argattr11))
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
            //        string argattr12 = "援";
            //        if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, argattr12))
            //        {
            //            Map.MaskData[withBlock3.x, withBlock3.y] = true;
            //        }
            //    }

            //    string argoname = "大型マップ";
            //    if (!Expression.IsOptionDefined(argoname))
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
            //    if (u.Status_Renamed == "出撃" & u.Party == "味方")
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
            //string argcursor_mode1 = "ユニット選択";
            //GUI.MoveCursorPos(argcursor_mode1, t);

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
            //    string argattr = "合";
            //    if (withBlock.IsAbilityClassifiedAs(SelectedAbility, argattr))
            //    {
            //        if (withBlock.AbilityMaxRange(SelectedAbility) == 1)
            //        {
            //            string argctype_Renamed = "アビリティ";
            //            withBlock.CombinationPartner(argctype_Renamed, SelectedAbility, partners, SelectedTarget.x, SelectedTarget.y);
            //        }
            //        else
            //        {
            //            string argctype_Renamed1 = "アビリティ";
            //            withBlock.CombinationPartner(argctype_Renamed1, SelectedAbility, partners);
            //        }
            //    }
            //    else
            //    {
            //        SelectedPartners = new Unit[1];
            //        partners = new Unit[1];
            //    }
            //}

            //aname = SelectedUnit.Ability(SelectedAbility).Name;
            //SelectedAbilityName = aname;

            //// ADD START MARGE
            //// 移動後使用後可能なアビリティか記録しておく
            //string argattr1 = "Ｐ";
            //string argattr2 = "Ｑ";
            //is_p_ability = SelectedUnit.IsAbilityClassifiedAs(SelectedAbility, argattr1) | SelectedUnit.AbilityMaxRange(SelectedAbility) == 1 & !SelectedUnit.IsAbilityClassifiedAs(SelectedAbility, argattr2);
            //// ADD END MARGE

            //// 使用イベント
            //Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, aname);
            //if (SRC.IsScenarioFinished)
            //{
            //    SRC.IsScenarioFinished = false;
            //    SelectedPartners = new Unit[1];
            //    GUI.UnlockGUI();
            //    return;
            //}

            //if (SRC.IsCanceled)
            //{
            //    SRC.IsCanceled = false;
            //    SelectedPartners = new Unit[1];
            //    WaitCommand();
            //    return;
            //}

            //{
            //    var withBlock1 = SelectedUnit;
            //    foreach (Unit u in SRC.UList)
            //    {
            //        if (u.Status_Renamed == "出撃")
            //        {
            //            Map.MaskData[u.x, u.y] = true;
            //        }
            //    }

            //    // 合体技パートナーのハイライト表示
            //    string argattr3 = "合";
            //    if (withBlock1.IsAbilityClassifiedAs(SelectedAbility, argattr3))
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
            //    if (withBlock3.Status_Renamed == "破壊")
            //    {
            //        if (withBlock3.CountPilot() > 0)
            //        {
            //            Event_Renamed.HandleEvent("破壊", withBlock3.MainPilot().ID);
            //            if (SRC.IsScenarioFinished)
            //            {
            //                SRC.IsScenarioFinished = false;
            //                SelectedPartners = new Unit[1];
            //                GUI.UnlockGUI();
            //                return;
            //            }

            //            if (SRC.IsCanceled)
            //            {
            //                SRC.IsCanceled = false;
            //                SelectedPartners = new Unit[1];
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
            //        Event_Renamed.HandleEvent("使用後", withBlock4.MainPilot().ID, aname);
            //        if (SRC.IsScenarioFinished)
            //        {
            //            SRC.IsScenarioFinished = false;
            //            SelectedPartners = new Unit[1];
            //            GUI.UnlockGUI();
            //            return;
            //        }

            //        if (SRC.IsCanceled)
            //        {
            //            SRC.IsCanceled = false;
            //            SelectedPartners = new Unit[1];
            //            GUI.UnlockGUI();
            //            return;
            //        }
            //    }
            //}

            //// 合体技のパートナーの行動数を減らす
            //string argoname = "合体技パートナー行動数無消費";
            //if (!Expression.IsOptionDefined(argoname))
            //{
            //    var loopTo1 = Information.UBound(partners);
            //    for (i = 1; i <= loopTo1; i++)
            //        partners[i].CurrentForm().UseAction();
            //}

            //SelectedPartners = new Unit[1];

            //// ADD START MARGE
            //// 再移動
            //if (is_p_ability & SelectedUnit.Status_Renamed == "出撃")
            //{
            //    string argsname = "遊撃";
            //    if (SelectedUnit.MainPilot().IsSkillAvailable(argsname) & SelectedUnit.Speed * 2 > SelectedUnitMoveCost)
            //    {
            //        // 進入イベント
            //        if (SelectedUnitMoveCost > 0)
            //        {
            //            Event_Renamed.HandleEvent("進入", SelectedUnit.MainPilot().ID, SelectedUnit.x, SelectedUnit.y);
            //            if (SRC.IsScenarioFinished)
            //            {
            //                SRC.IsScenarioFinished = false;
            //                return;
            //            }
            //        }

            //        // ユニットが既に出撃していない？
            //        if (SelectedUnit.Status_Renamed != "出撃")
            //        {
            //            GUI.RedrawScreen();
            //            Status.ClearUnitStatus();
            //            return;
            //        }

            //        SelectedCommand = "再移動";
            //        Map.AreaInSpeed(SelectedUnit);
            //        string argoname1 = "大型マップ";
            //        if (!Expression.IsOptionDefined(argoname1))
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
            //    string argattr = "Ｐ";
            //    string argattr1 = "Ｑ";
            //    is_p_ability = withBlock.IsAbilityClassifiedAs(SelectedAbility, argattr) | withBlock.AbilityMaxRange(SelectedAbility) == 1 & !withBlock.IsAbilityClassifiedAs(SelectedAbility, argattr1);
            //    // ADD END MARGE

            //    // 目標地点を選択して初めて効果範囲が分かるタイプのマップアビリティ
            //    // の場合は再度プレイヤーの選択を促す必要がある
            //    if (CommandState == "ターゲット選択" | CommandState == "移動後ターゲット選択")
            //    {
            //        string argattr3 = "Ｍ投";
            //        string argattr4 = "Ｍ移";
            //        string argattr5 = "Ｍ線";
            //        if (withBlock.IsAbilityClassifiedAs(SelectedAbility, argattr3))
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
            //            string argattr2 = "Ｍ投";
            //            string arguparty = "味方";
            //            Map.AreaInRange(SelectedX, SelectedY, withBlock.AbilityLevel(SelectedAbility, argattr2), 1, arguparty);
            //            GUI.MaskScreen();
            //            return;
            //        }
            //        else if (withBlock.IsAbilityClassifiedAs(SelectedAbility, argattr4))
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
            //        else if (withBlock.IsAbilityClassifiedAs(SelectedAbility, argattr5))
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
            //    string argattr6 = "合";
            //    if (withBlock.IsAbilityClassifiedAs(SelectedAbility, argattr6))
            //    {
            //        string argctype_Renamed = "アビリティ";
            //        withBlock.CombinationPartner(argctype_Renamed, SelectedAbility, partners);
            //    }
            //    else
            //    {
            //        SelectedPartners = new Unit[1];
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
            //    SelectedPartners = new Unit[1];
            //    GUI.UnlockGUI();
            //    return;
            //}

            //if (SRC.IsCanceled)
            //{
            //    SRC.IsCanceled = false;
            //    SelectedPartners = new Unit[1];
            //    WaitCommand();
            //    return;
            //}

            //// 合体技のパートナーの行動数を減らす
            //string argoname = "合体技パートナー行動数無消費";
            //if (!Expression.IsOptionDefined(argoname))
            //{
            //    var loopTo = Information.UBound(partners);
            //    for (i = 1; i <= loopTo; i++)
            //        partners[i].CurrentForm().UseAction();
            //}

            //SelectedPartners = new Unit[1];

            //// ADD START MARGE
            //// 再移動
            //if (is_p_ability & SelectedUnit.Status_Renamed == "出撃")
            //{
            //    string argsname = "遊撃";
            //    if (SelectedUnit.MainPilot().IsSkillAvailable(argsname) & SelectedUnit.Speed * 2 > SelectedUnitMoveCost)
            //    {
            //        // 進入イベント
            //        if (SelectedUnitMoveCost > 0)
            //        {
            //            Event_Renamed.HandleEvent("進入", SelectedUnit.MainPilot().ID, SelectedUnit.x, SelectedUnit.y);
            //            if (SRC.IsScenarioFinished)
            //            {
            //                SRC.IsScenarioFinished = false;
            //                return;
            //            }
            //        }

            //        // ユニットが既に出撃していない？
            //        if (SelectedUnit.Status_Renamed != "出撃")
            //        {
            //            GUI.RedrawScreen();
            //            Status.ClearUnitStatus();
            //            return;
            //        }

            //        SelectedCommand = "再移動";
            //        Map.AreaInSpeed(SelectedUnit);
            //        string argoname1 = "大型マップ";
            //        if (!Expression.IsOptionDefined(argoname1))
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