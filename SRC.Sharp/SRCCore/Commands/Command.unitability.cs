// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

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

            var currentUnit = SelectedUnit;

            // アビリティ専用ＢＧＭがあればそれを演奏
            if (currentUnit.IsFeatureAvailable("アビリティＢＧＭ"))
            {
                var BGM = currentUnit.Features.Where(x => x.Name == "アビリティＢＧＭ")
                    .Where(x => GeneralLib.LIndex(x.Data, 1) == unitAbility.Data.Name)
                    .Select(x => Sound.SearchMidiFile(Strings.Mid(x.Data, Strings.InStr(x.Data, " ") + 1)))
                    .FirstOrDefault();
                if (!string.IsNullOrEmpty(BGM))
                {
                    Sound.ChangeBGM(BGM);
                }
            }

            // 射程0のアビリティはその場で実行
            if (unitAbility.Data.MaxRange == 0)
            {
                SelectedTarget = SelectedUnit;

                // 変身アビリティであるか判定
                var is_transformation = unitAbility.Data.Effects.Any(x => x.EffectType == "変身");

                SelectedAbilityName = unitAbility.Data.Name;

                // 使用イベント
                Event.HandleEvent("使用", SelectedUnit.MainPilot().ID, SelectedAbilityName);
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
                SelectedUnit.ExecuteAbility(unitAbility, SelectedUnit);
                SelectedUnit = SelectedUnit.CurrentForm();
                GUI.CloseMessageForm();

                // 破壊イベント
                {
                    var withBlock1 = SelectedUnit;
                    if (withBlock1.Status == "破壊")
                    {
                        if (withBlock1.CountPilot() > 0)
                        {
                            Event.HandleEvent("破壊", withBlock1.MainPilot().ID);
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
                        Event.HandleEvent("使用後", withBlock2.MainPilot().ID, SelectedAbilityName);
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
                    if (SelectedUnit.Status == "出撃")
                    {
                        // カーソル自動移動
                        if (SRC.AutoMoveCursor)
                        {
                            GUI.MoveCursorPos("ユニット選択", SelectedUnit);
                        }

                        Status.DisplayUnitStatus(SelectedUnit);
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

            // アビリティの射程を求めておく
            var min_range = unitAbility.AbilityMinRange();
            var max_range = unitAbility.AbilityMaxRange();
            {
                // マップ型アビリティかどうかで今後のコマンド処理の進行の仕方が異なる
                if (is_item)
                {
                    if (unitAbility.IsAbilityClassifiedAs("Ｍ"))
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
                    if (unitAbility.IsAbilityClassifiedAs("Ｍ"))
                    {
                        SelectedCommand = "マップアビリティ";
                    }
                    else
                    {
                        SelectedCommand = "アビリティ";
                    }
                }


                // アビリティの効果範囲を設定
                if (unitAbility.IsAbilityClassifiedAs("Ｍ直"))
                {
                    Map.AreaInCross(currentUnit.x, currentUnit.y, min_range, max_range);
                }
                else if (unitAbility.IsAbilityClassifiedAs("Ｍ拡"))
                {
                    Map.AreaInWideCross(currentUnit.x, currentUnit.y, min_range, max_range);
                }
                else if (unitAbility.IsAbilityClassifiedAs("Ｍ扇"))
                {
                    Map.AreaInSectorCross(currentUnit.x, currentUnit.y, min_range, max_range, (int)unitAbility.AbilityLevel("Ｍ扇"));
                }
                else if (unitAbility.IsAbilityClassifiedAs("Ｍ移"))
                {
                    Map.AreaInMoveAction(SelectedUnit, max_range);
                }
                else
                {
                    Map.AreaInRange(currentUnit.x, currentUnit.y, max_range, min_range, "すべて");
                }

                // 射程１の合体技はパートナーで相手を取り囲んでいないと使用できない
                if (unitAbility.IsAbilityClassifiedAs("合") && !unitAbility.IsAbilityClassifiedAs("Ｍ") && unitAbility.Data.MaxRange == 1)
                {
                    foreach (var t in Map.AdjacentUnit(currentUnit))
                    {
                        if (currentUnit.IsAlly(t))
                        {
                            var partners = unitAbility.CombinationPartner(t.x, t.y);
                            if (partners.Count == 0)
                            {
                                Map.MaskData[t.x, t.y] = true;
                            }
                        }
                    }
                }
            }

            // ユニットがいるマスの処理
            if (!unitAbility.IsAbilityClassifiedAs("Ｍ投")
                && !unitAbility.IsAbilityClassifiedAs("Ｍ線")
                && !unitAbility.IsAbilityClassifiedAs("Ｍ移"))
            {
                var loopTo2 = GeneralLib.MinLng(currentUnit.x + max_range, Map.MapWidth);
                for (var i = GeneralLib.MaxLng(currentUnit.x - max_range, 1); i <= loopTo2; i++)
                {
                    var loopTo3 = GeneralLib.MinLng(currentUnit.y + max_range, Map.MapHeight);
                    for (var j = GeneralLib.MaxLng(currentUnit.y - max_range, 1); j <= loopTo3; j++)
                    {
                        if (!Map.MaskData[i, j])
                        {
                            var t = Map.MapDataForUnit[i, j];
                            if (t is object)
                            {
                                // 有効？
                                if (unitAbility.IsAbilityEffective(t))
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
            if (!Map.MaskData[currentUnit.x, currentUnit.y])
            {
                if (unitAbility.IsAbilityClassifiedAs("援"))
                {
                    Map.MaskData[currentUnit.x, currentUnit.y] = true;
                }
            }

            if (!Expression.IsOptionDefined("大型マップ"))
            {
                GUI.Center(currentUnit.x, currentUnit.y);
            }

            GUI.MaskScreen();

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
            {
                Unit t = null;
                foreach (Unit u in SRC.UList.Items)
                {
                    if (u.Status == "出撃" && u.Party == "味方")
                    {
                        if (Map.MaskData[u.x, u.y] == false && !ReferenceEquals(u, SelectedUnit))
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
                GUI.MoveCursorPos("ユニット選択", t);

                // ターゲットのステータスを表示
                if (!ReferenceEquals(SelectedUnit, t))
                {
                    Status.DisplayUnitStatus(t);
                }
            }
            GUI.UnlockGUI();
        }

        // 「アビリティ」コマンドを終了
        private void FinishAbilityCommand()
        {
            //// MOD START MARGE
            //// If MainWidth <> 15 Then
            //if (GUI.NewGUIMode)
            //{
            //    // MOD END MARGE
            //    Status.ClearUnitStatus();
            //}

            GUI.LockGUI();
            var currentUnit = SelectedUnit;
            var unitAbility = currentUnit.Ability(SelectedAbility);

            // 合体技のパートナーを設定
            IList<Unit> partners = new List<Unit>();
            {
                if (unitAbility.IsAbilityClassifiedAs("合"))
                {
                    if (unitAbility.AbilityMaxRange() == 1)
                    {
                        partners = unitAbility.CombinationPartner(SelectedTarget.x, SelectedTarget.y);
                    }
                    else
                    {
                        partners = unitAbility.CombinationPartner();
                    }
                }
                else
                {
                    SelectedPartners.Clear();
                }
            }

            var aname = unitAbility.Data.Name;
            SelectedAbilityName = aname;

            // 移動後使用後可能なアビリティか記録しておく
            var is_p_ability = unitAbility.IsAbilityClassifiedAs("Ｐ")
                        || unitAbility.AbilityMaxRange() == 1 && !unitAbility.IsAbilityClassifiedAs("Ｑ");

            // 使用イベント
            Event.HandleEvent("使用", SelectedUnit.MainPilot().ID, aname);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                SelectedPartners.Clear();
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                SelectedPartners.Clear();
                WaitCommand();
                return;
            }

            {
                foreach (Unit u in SRC.UList.Items)
                {
                    if (u.Status == "出撃")
                    {
                        Map.MaskData[u.x, u.y] = true;
                    }
                }

                // 合体技パートナーのハイライト表示
                foreach (var pu in partners)
                {
                    Map.MaskData[pu.x, pu.y] = false;
                }

                Map.MaskData[currentUnit.x, currentUnit.y] = false;
                Map.MaskData[SelectedTarget.x, SelectedTarget.y] = false;
                if (!SRC.BattleAnimation)
                {
                    GUI.MaskScreen();
                }

                // アビリティを実行
                currentUnit.ExecuteAbility(unitAbility, SelectedTarget);
                SelectedUnit = currentUnit.CurrentForm();
                GUI.CloseMessageForm();
                Status.ClearUnitStatus();
            }

            // 破壊イベント
            {
                if (currentUnit.Status == "破壊")
                {
                    if (currentUnit.CountPilot() > 0)
                    {
                        Event.HandleEvent("破壊", currentUnit.MainPilot().ID);
                        if (SRC.IsScenarioFinished)
                        {
                            SRC.IsScenarioFinished = false;
                            SelectedPartners.Clear();
                            GUI.UnlockGUI();
                            return;
                        }

                        if (SRC.IsCanceled)
                        {
                            SRC.IsCanceled = false;
                            SelectedPartners.Clear();
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
                    Event.HandleEvent("使用後", withBlock4.MainPilot().ID, aname);
                    if (SRC.IsScenarioFinished)
                    {
                        SRC.IsScenarioFinished = false;
                        SelectedPartners.Clear();
                        GUI.UnlockGUI();
                        return;
                    }

                    if (SRC.IsCanceled)
                    {
                        SRC.IsCanceled = false;
                        SelectedPartners.Clear();
                        GUI.UnlockGUI();
                        return;
                    }
                }
            }

            // 合体技のパートナーの行動数を減らす
            if (!Expression.IsOptionDefined("合体技パートナー行動数無消費"))
            {
                foreach (var pu in partners)
                {
                    pu.CurrentForm().UseAction();
                }
            }

            SelectedPartners.Clear();

            // 再移動
            if (is_p_ability && SelectedUnit.Status == "出撃")
            {
                if (SelectedUnit.MainPilot().IsSkillAvailable("遊撃") && SelectedUnit.Speed * 2 > SelectedUnitMoveCost)
                {
                    // 進入イベント
                    if (SelectedUnitMoveCost > 0)
                    {
                        Event.HandleEvent("進入", SelectedUnit.MainPilot().ID, "" + SelectedUnit.x, "" + SelectedUnit.y);
                        if (SRC.IsScenarioFinished)
                        {
                            SRC.IsScenarioFinished = false;
                            return;
                        }
                    }

                    // ユニットが既に出撃していない？
                    if (SelectedUnit.Status != "出撃")
                    {
                        GUI.RedrawScreen();
                        Status.ClearUnitStatus();
                        return;
                    }

                    SelectedCommand = "再移動";
                    Map.AreaInSpeed(SelectedUnit);
                    if (!Expression.IsOptionDefined("大型マップ"))
                    {
                        GUI.Center(SelectedUnit.x, SelectedUnit.y);
                    }

                    GUI.MaskScreen();
                    //if (GUI.NewGUIMode)
                    //{
                    //    Application.DoEvents();
                    //    Status.ClearUnitStatus();
                    //}
                    //else
                    {
                        Status.DisplayUnitStatus(SelectedUnit);
                    }

                    CommandState = "ターゲット選択";
                    return;
                }
            }

            // 行動終了
            WaitCommand();
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
            //    is_p_ability = withBlock.IsAbilityClassifiedAs(SelectedAbility, "Ｐ") | withBlock.AbilityMaxRange(SelectedAbility) == 1 && !withBlock.IsAbilityClassifiedAs(SelectedAbility, "Ｑ");
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
            //if (is_p_ability && SelectedUnit.Status == "出撃")
            //{
            //    if (SelectedUnit.MainPilot().IsSkillAvailable("遊撃") && SelectedUnit.Speed * 2 > SelectedUnitMoveCost)
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
