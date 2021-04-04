// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;
using System;

namespace SRCCore.Commands
{
    public partial class Command
    {
        // 「攻撃」コマンドを開始
        private void StartAttackCommand()
        {
            LogDebug();

            int i, j;
            Unit t;
            int min_range, max_range;
            var BGM = "";
            GUI.LockGUI();
            var partners = default(Unit[]);
            var currentUnit = SelectedUnit;

            // ＢＧＭの設定
            if (currentUnit.IsFeatureAvailable("ＢＧＭ"))
            {
                BGM = Sound.SearchMidiFile(currentUnit.FeatureData("ＢＧＭ"));
            }

            if (Strings.Len(BGM) == 0)
            {
                BGM = Sound.SearchMidiFile(currentUnit.MainPilot().BGM);
            }

            if (Strings.Len(BGM) == 0)
            {
                BGM = Sound.BGMName("default");
            }

            // 武器の選択
            UnitWeapon currentWeapon;
            UseSupportAttack = true;
            if (CommandState == "コマンド選択")
            {
                currentWeapon = GUI.WeaponListBox(
                    SelectedUnit,
                    new UnitWeaponList(WeaponListMode.BeforeMove, SelectedUnit),
                     "武器選択", "移動前", BGM);
            }
            else
            {
                currentWeapon = GUI.WeaponListBox(
                    SelectedUnit,
                    new UnitWeaponList(WeaponListMode.AfterMove, SelectedUnit),
                     "武器選択", "移動後", BGM);
            }

            // キャンセル
            if (currentWeapon == null)
            {
                SelectedWeapon = 0;
                if (SRC.AutoMoveCursor)
                {
                    GUI.RestoreCursorPos();
                }

                CancelCommand();
                GUI.UnlockGUI();
                return;
            }

            SelectedWeapon = currentWeapon.WeaponNo();

            //// 武器ＢＧＭの演奏
            //if (currentUnit.IsFeatureAvailable("武器ＢＧＭ"))
            //{
            //    var loopTo = currentUnit.CountFeature();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        string localFeature() { object argIndex1 = i; var ret = currentUnit.Feature(argIndex1); return ret; }

            //        string localFeatureData2() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

            //        string localLIndex() { string arglist = hs644c0746afde449eb03788895da90548(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //        if (localFeature() == "武器ＢＧＭ" & (localLIndex() ?? "") == (currentUnit.Weapon(SelectedWeapon).Name ?? ""))
            //        {
            //            string localFeatureData() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

            //            string localFeatureData1() { object argIndex1 = i; var ret = currentUnit.FeatureData(argIndex1); return ret; }

            //            BGM = Sound.SearchMidiFile(Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1));
            //            if (Strings.Len(BGM) > 0)
            //            {
            //                Sound.ChangeBGM(BGM);
            //            }

            //            break;
            //        }
            //    }
            //}

            // 選択した武器の種類により、この後のコマンドの進行の仕方が異なる
            if (currentWeapon.IsWeaponClassifiedAs("Ｍ"))
            {
                SelectedCommand = "マップ攻撃";
            }
            else
            {
                SelectedCommand = "攻撃";
            }

            // 武器の射程を求めておく
            min_range = currentWeapon.WeaponMinRange();
            max_range = currentWeapon.WeaponMaxRange();

            // 攻撃範囲の表示
            //if (currentUnit.IsWeaponClassifiedAs(SelectedWeapon, "Ｍ直"))
            //{
            //    Map.AreaInCross(currentUnit.x, currentUnit.y, min_range, max_range);
            //}
            //else if (currentUnit.IsWeaponClassifiedAs(SelectedWeapon, "Ｍ拡"))
            //{
            //    Map.AreaInWideCross(currentUnit.x, currentUnit.y, min_range, max_range);
            //}
            //else if (currentUnit.IsWeaponClassifiedAs(SelectedWeapon, "Ｍ扇"))
            //{
            //    Map.AreaInSectorCross(currentUnit.x, currentUnit.y, min_range, max_range, currentUnit.WeaponLevel(SelectedWeapon, "Ｍ扇"));
            //}
            //else if (currentUnit.IsWeaponClassifiedAs(SelectedWeapon, "Ｍ全") | currentUnit.IsWeaponClassifiedAs(SelectedWeapon, "Ｍ投") | currentUnit.IsWeaponClassifiedAs(SelectedWeapon, "Ｍ線"))
            //{
            //    Map.AreaInRange(currentUnit.x, currentUnit.y, max_range, min_range, "すべて");
            //}
            //else if (currentUnit.IsWeaponClassifiedAs(SelectedWeapon, "Ｍ移"))
            //{
            //    Map.AreaInMoveAction(SelectedUnit, max_range);
            //}
            //else
            {
                Map.AreaInRange(currentUnit.x, currentUnit.y, max_range, min_range, "味方の敵");
            }

            //// 射程１の合体技はパートナーで相手を取り囲んでいないと使用できない
            //if (max_range == 1 & currentUnit.IsWeaponClassifiedAs(SelectedWeapon, "合") & !currentUnit.IsWeaponClassifiedAs(SelectedWeapon, "Ｍ"))
            //{
            //    for (i = 1; i <= 4; i++)
            //    {
            //        t = null;
            //        switch (i)
            //        {
            //            case 1:
            //                {
            //                    if (currentUnit.x > 1)
            //                    {
            //                        t = Map.MapDataForUnit[currentUnit.x - 1, currentUnit.y];
            //                    }

            //                    break;
            //                }

            //            case 2:
            //                {
            //                    if (currentUnit.x < Map.MapWidth)
            //                    {
            //                        t = Map.MapDataForUnit[currentUnit.x + 1, currentUnit.y];
            //                    }

            //                    break;
            //                }

            //            case 3:
            //                {
            //                    if (currentUnit.y > 1)
            //                    {
            //                        t = Map.MapDataForUnit[currentUnit.x, currentUnit.y - 1];
            //                    }

            //                    break;
            //                }

            //            case 4:
            //                {
            //                    if (currentUnit.y < Map.MapHeight)
            //                    {
            //                        t = Map.MapDataForUnit[currentUnit.x, currentUnit.y + 1];
            //                    }

            //                    break;
            //                }
            //        }

            //        if (t is object)
            //        {
            //            if (currentUnit.IsEnemy(t))
            //            {
            //                currentUnit.CombinationPartner("武装", SelectedWeapon, partners, t.x, t.y);
            //                if (Information.UBound(partners) == 0)
            //                {
            //                    Map.MaskData[t.x, t.y] = true;
            //                }
            //            }
            //        }
            //    }
            //}

            //// ユニットに対するマスクの設定
            //if (!currentUnit.IsWeaponClassifiedAs(SelectedWeapon, "Ｍ投") & !currentUnit.IsWeaponClassifiedAs(SelectedWeapon, "Ｍ線"))
            //{
            //    var loopTo1 = GeneralLib.MinLng(currentUnit.x + max_range, Map.MapWidth);
            //    for (i = GeneralLib.MaxLng(currentUnit.x - max_range, 1); i <= loopTo1; i++)
            //    {
            //        var loopTo2 = GeneralLib.MinLng(currentUnit.y + max_range, Map.MapHeight);
            //        for (j = GeneralLib.MaxLng(currentUnit.y - max_range, 1); j <= loopTo2; j++)
            //        {
            //            if (Map.MaskData[i, j])
            //            {
            //                goto NextLoop;
            //            }

            //            t = Map.MapDataForUnit[i, j];
            //            if (t is null)
            //            {
            //                goto NextLoop;
            //            }

            //            // 武器の地形適応が有効？
            //            if (currentUnit.WeaponAdaption(SelectedWeapon, t.Area) == 0d)
            //            {
            //                Map.MaskData[i, j] = true;
            //                goto NextLoop;
            //            }

            //            // 封印武器の対象属性外でない？
            //            if (currentUnit.IsWeaponClassifiedAs(SelectedWeapon, "封"))
            //            {
            //                if (currentUnit.Weapon(SelectedWeapon).Power > 0 & currentUnit.Damage(SelectedWeapon, t, true) == 0 | currentUnit.CriticalProbability(SelectedWeapon, t) == 0)
            //                {
            //                    Map.MaskData[i, j] = true;
            //                    goto NextLoop;
            //                }
            //            }

            //            // 限定武器の対象属性外でない？
            //            if (currentUnit.IsWeaponClassifiedAs(SelectedWeapon, "限"))
            //            {
            //                if (currentUnit.Weapon(SelectedWeapon).Power > 0 & currentUnit.Damage(SelectedWeapon, t, true) == 0 | currentUnit.Weapon(SelectedWeapon).Power == 0 & currentUnit.CriticalProbability(SelectedWeapon, t) == 0)
            //                {
            //                    Map.MaskData[i, j] = true;
            //                    goto NextLoop;
            //                }
            //            }

            //            // 識別攻撃の場合の処理
            //            if (currentUnit.IsWeaponClassifiedAs(SelectedWeapon, "識") | currentUnit.IsUnderSpecialPowerEffect("識別攻撃"))
            //            {
            //                if (currentUnit.IsAlly(t))
            //                {
            //                    Map.MaskData[i, j] = true;
            //                    goto NextLoop;
            //                }
            //            }

            //            // ステルス＆隠れ身チェック
            //            if (!currentUnit.IsWeaponClassifiedAs(SelectedWeapon, "Ｍ"))
            //            {
            //                if (!currentUnit.IsTargetWithinRange(SelectedWeapon, t))
            //                {
            //                    Map.MaskData[i, j] = true;
            //                    goto NextLoop;
            //                }
            //            }

            //        NextLoop:
            //            ;
            //        }
            //    }
            //}

            //Map.MaskData[currentUnit.x, currentUnit.y] = false;
            //if (!Expression.IsOptionDefined("大型マップ"))
            //{
            //    GUI.Center(currentUnit.x, currentUnit.y);
            //}

            GUI.MaskScreen();

            // ターゲット選択へ
            if (CommandState == "コマンド選択")
            {
                CommandState = "ターゲット選択";
            }
            else
            {
                CommandState = "移動後ターゲット選択";
            }

            //// カーソル自動移動を行う？
            //if (!SRC.AutoMoveCursor)
            //{
            //    GUI.UnlockGUI();
            //    return;
            //}

            //// ＨＰがもっとも低いターゲットを探す
            //t = null;
            //foreach (Unit u in SRC.UList)
            //{
            //    if (u.Status == "出撃" & (u.Party == "敵" | u.Party == "中立"))
            //    {
            //        if (Map.MaskData[u.x, u.y] == false)
            //        {
            //            if (t is null)
            //            {
            //                t = u;
            //            }
            //            else if (u.HP < t.HP)
            //            {
            //                t = u;
            //            }
            //            else if (u.HP == t.HP)
            //            {
            //                if (Math.Pow(Math.Abs((SelectedUnit.x - u.x)), 2d) + Math.Pow(Math.Abs((SelectedUnit.y - u.y)), 2d) < Math.Pow(Math.Abs((SelectedUnit.x - t.x)), 2d) + Math.Pow(Math.Abs((SelectedUnit.y - t.y)), 2d))
            //                {
            //                    t = u;
            //                }
            //            }
            //        }
            //    }
            //}

            //// 適当なターゲットが見つからなければ自分自身を選択
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

            GUI.UnlockGUI();
        }

        // 「攻撃」コマンドを終了
        private void FinishAttackCommand()
        {
            LogDebug();

            // XXX COMの攻撃と同じところは切り出しておきたい。。。

            int i;
            var earnings = false;
            string def_mode = "";
            var partners = default(Unit[]);
            var BGM = "";
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
            bool is_p_weapon;
            //// If MainWidth <> 15 Then
            //if (GUI.NewGUIMode)
            //{
            //    Status.ClearUnitStatus();
            //}

            GUI.LockGUI();
            var currentUnit = SelectedUnit;
            var currentWeapon = currentUnit.Weapon(SelectedWeapon);
            wname = currentWeapon.Name;
            SelectedWeaponName = wname;

            // 移動後使用後可能な武器か記録しておく
            is_p_weapon = currentWeapon.IsWeaponClassifiedAs("移動後攻撃可");

            //// 合体技のパートナーを設定
            //{
            //    var withBlock = SelectedUnit;
            //    if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, "合"))
            //    {
            //        if (withBlock.WeaponMaxRange(SelectedWeapon) == 1)
            //        {
            //            withBlock.CombinationPartner("武装", SelectedWeapon, partners, SelectedTarget.x, SelectedTarget.y);
            //        }
            //        else
            //        {
            //            withBlock.CombinationPartner("武装", SelectedWeapon, partners);
            //        }
            //    }
            //    else
            //    {
            //        SelectedPartners = new Unit[0];
            //        partners = new Unit[1];
            //    }
            //}

            // 敵の反撃手段を設定
            UnitWeapon currentTWeapon = null;
            UseSupportGuard = true;
            SelectedTWeapon = COM.SelectWeapon(SelectedTarget, SelectedUnit, "反撃", out _, out _);
            if (currentWeapon.IsWeaponClassifiedAs("間"))
            {
                SelectedTWeapon = 0;
            }

            if (SelectedTWeapon > 0)
            {
                currentTWeapon = SelectedTarget.Weapon(SelectedTWeapon);
                twname = currentTWeapon.Name;
                SelectedTWeaponName = twname;
            }
            else
            {
                SelectedTWeaponName = "";
            }

            // 敵の防御行動を設定
            def_mode = Conversions.ToString(COM.SelectDefense(SelectedUnit, SelectedWeapon, SelectedTarget, SelectedTWeapon));
            if (!string.IsNullOrEmpty(def_mode))
            {
                if (SelectedTWeapon > 0)
                {
                    SelectedTWeapon = -1;
                }
            }

            SelectedDefenseOption = def_mode;

            // 戦闘前に一旦クリア
            SupportAttackUnit = null;
            SupportGuardUnit = null;
            SupportGuardUnit2 = null;

            // 攻撃側の武器使用イベント
            Event.HandleEvent("使用", SelectedUnit.MainPilot().ID, wname);
            if (SRC.IsScenarioFinished)
            {
                GUI.UnlockGUI();
                SRC.IsScenarioFinished = false;
                SelectedPartners = new Unit[0];
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
                Event.HandleEvent("使用", SelectedUnit.MainPilot().ID, twname);
                RestoreSelections();
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    SelectedPartners = new Unit[0];
                    GUI.UnlockGUI();
                    return;
                }

                if (SRC.IsCanceled)
                {
                    SRC.IsCanceled = false;
                    SelectedPartners = new Unit[0];
                    WaitCommand();
                    return;
                }
            }

            // 攻撃イベント
            Event.HandleEvent("攻撃", SelectedUnit.MainPilot().ID, SelectedTarget.MainPilot().ID);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                SelectedPartners = new Unit[0];
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                SelectedPartners = new Unit[0];
                WaitCommand();
                return;
            }

            // 敵がＢＧＭ能力を持つ場合はＢＧＭを変更
            {
                var tu = SelectedTarget;
                if (tu.IsFeatureAvailable("ＢＧＭ") && Strings.InStr(tu.MainPilot().Name, "(ザコ)") == 0)
                {
                    BGM = Sound.SearchMidiFile(tu.FeatureData("ＢＧＭ"));
                    if (Strings.Len(BGM) > 0)
                    {
                        Sound.BossBGM = false;
                        Sound.ChangeBGM(BGM);
                        Sound.BossBGM = true;
                    }
                }
            }

            // そうではなく、ボス用ＢＧＭが流れていれば味方のＢＧＭに切り替え
            if (Strings.Len(BGM) == 0 && Sound.BossBGM)
            {
                Sound.BossBGM = false;
                BGM = "";
                {
                    var su = SelectedUnit;
                    //if (su.IsFeatureAvailable("武器ＢＧＭ"))
                    //{
                    //    var loopTo = su.CountFeature();
                    //    for (i = 1; i <= loopTo; i++)
                    //    {
                    //        string localFeature() { object argIndex1 = i; var ret = su.Feature(argIndex1); return ret; }

                    //        string localFeatureData2() { object argIndex1 = i; var ret = su.FeatureData(argIndex1); return ret; }

                    //        string localLIndex() { string arglist = hs51a192742c114976b44a246d492333eb(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

                    //        if (localFeature() == "武器ＢＧＭ" & (localLIndex() ?? "") == (su.Weapon(SelectedWeapon).Name ?? ""))
                    //        {
                    //            string localFeatureData() { object argIndex1 = i; var ret = su.FeatureData(argIndex1); return ret; }

                    //            string localFeatureData1() { object argIndex1 = i; var ret = su.FeatureData(argIndex1); return ret; }

                    //            BGM = Sound.SearchMidiFile(Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1));
                    //            break;
                    //        }
                    //    }
                    //}

                    if (Strings.Len(BGM) == 0)
                    {
                        if (su.IsFeatureAvailable("ＢＧＭ"))
                        {
                            BGM = Sound.SearchMidiFile(su.FeatureData("ＢＧＭ"));
                        }
                    }

                    if (Strings.Len(BGM) == 0)
                    {
                        BGM = Sound.SearchMidiFile(su.MainPilot().BGM);
                    }

                    if (Strings.Len(BGM) == 0)
                    {
                        BGM = Sound.BGMName("default");
                    }

                    Sound.ChangeBGM(BGM);
                }
            }

            // 攻撃参加ユニット以外はマスク
            foreach (Unit u in SRC.UList.Items)
            {
                if (u.Status == "出撃")
                {
                    Map.MaskData[u.x, u.y] = true;
                }
            }

            //    // 合体技パートナーのハイライト表示
            //    var loopTo1 = Information.UBound(partners);
            //    for (i = 1; i <= loopTo1; i++)
            //    {
            //        {
            //            var withBlock4 = partners[i];
            //            Map.MaskData[withBlock4.x, withBlock4.y] = false;
            //        }
            //    }

            Map.MaskData[SelectedUnit.x, SelectedUnit.y] = false;
            Map.MaskData[SelectedTarget.x, SelectedTarget.y] = false;
            if (!SRC.BattleAnimation)
            {
                GUI.MaskScreen();
            }

            // イベント用に戦闘に参加するユニットの情報を記録しておく
            AttackUnit = SelectedUnit;
            attack_target = SelectedUnit;
            attack_target_hp_ratio = SelectedUnit.HP / (double)SelectedUnit.MaxHP;
            defense_target = SelectedTarget;
            defense_target_hp_ratio = SelectedTarget.HP / (double)SelectedTarget.MaxHP;
            defense_target2 = null;
            SupportAttackUnit = null;
            SupportGuardUnit = null;
            SupportGuardUnit2 = null;

            //// ターゲットの位置を記録
            tx = SelectedTarget.x;
            ty = SelectedTarget.y;
            GUI.OpenMessageForm(SelectedTarget, SelectedUnit);

            // 相手の先制攻撃？
            {
                var targetUnit = SelectedTarget;
                var targetWeapon = targetUnit.Weapon(SelectedTWeapon);
                if (targetWeapon != null
                    && targetUnit.MaxAction() > 0
                    && targetWeapon.IsWeaponAvailable("移動前"))
                {
                    if (!targetWeapon.IsWeaponClassifiedAs("後"))
                    {
                        var selectedWeapon = SelectedUnit.Weapon(SelectedWeapon);
                        if (selectedWeapon.IsWeaponClassifiedAs("後"))
                        {
                            def_mode = "先制攻撃";
                            if (targetWeapon.IsWeaponClassifiedAs("自"))
                            {
                                is_suiside = true;
                            }

                            // 先制攻撃攻撃を実施
                            targetUnit.Attack(targetWeapon, SelectedUnit, "先制攻撃", "");
                            SelectedTarget = targetUnit.CurrentForm();
                        }
                        else if (targetWeapon.IsWeaponClassifiedAs("先")
                            || targetUnit.MainPilot().SkillLevel("先読み", "") >= GeneralLib.Dice(16)
                            || targetUnit.IsUnderSpecialPowerEffect("カウンター"))
                        {
                            def_mode = "先制攻撃";
                            if (targetWeapon.IsWeaponClassifiedAs("自"))
                            {
                                is_suiside = true;
                            }

                            // カウンター攻撃を実施
                            targetUnit.Attack(targetWeapon, SelectedUnit, "カウンター", "");
                            SelectedTarget = targetUnit.CurrentForm();
                        }
                        else if (targetUnit.MaxCounterAttack() > targetUnit.UsedCounterAttack)
                        {
                            def_mode = "先制攻撃";
                            if (targetWeapon.IsWeaponClassifiedAs("自"))
                            {
                                is_suiside = true;
                            }

                            // カウンター攻撃の残り回数を減少
                            targetUnit.UsedCounterAttack = (targetUnit.UsedCounterAttack + 1);

                            // カウンター攻撃を実施
                            targetUnit.Attack(targetWeapon, SelectedUnit, "カウンター", "");
                            SelectedTarget = targetUnit.CurrentForm();
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

            //// サポートアタックのパートナーを探す
            //{
            //    var withBlock6 = SelectedUnit;
            //    if (withBlock6.Status == "出撃" & SelectedTarget.Status == "出撃" & UseSupportAttack)
            //    {
            //        SupportAttackUnit = withBlock6.LookForSupportAttack(SelectedTarget);

            //        // 合体技ではサポートアタック不能
            //        if (0 < SelectedWeapon & SelectedWeapon <= withBlock6.CountWeapon())
            //        {
            //            if (withBlock6.IsWeaponClassifiedAs(SelectedWeapon, "合"))
            //            {
            //                SupportAttackUnit = null;
            //            }
            //        }

            //        // 魅了された場合
            //        if (withBlock6.IsConditionSatisfied("魅了") & ReferenceEquals(withBlock6.Master, SelectedTarget))
            //        {
            //            SupportAttackUnit = null;
            //        }

            //        // 憑依された場合
            //        if (withBlock6.IsConditionSatisfied("憑依"))
            //        {
            //            if ((withBlock6.Master.Party ?? "") == (SelectedTarget.Party ?? ""))
            //            {
            //                SupportAttackUnit = null;
            //            }
            //        }

            //        // 踊らされた場合
            //        if (withBlock6.IsConditionSatisfied("踊り"))
            //        {
            //            SupportAttackUnit = null;
            //        }
            //    }
            //}

            // 攻撃の実施
            {
                //if (currentUnit.Status == "出撃" & currentUnit.MaxAction(true) > 0 & !currentUnit.IsConditionSatisfied("攻撃不能") & SelectedTarget.Status == "出撃")
                //{
                //    // まだ武器は使用可能か？
                //    if (SelectedWeapon > currentUnit.CountWeapon())
                //    {
                //        SelectedWeapon = -1;
                //    }
                //    else if ((wname ?? "") != (currentUnit.Weapon(SelectedWeapon).Name ?? ""))
                //    {
                //        SelectedWeapon = -1;
                //    }
                //    else if (CommandState == "移動後ターゲット選択")
                //    {
                //        if (!currentUnit.IsWeaponAvailable(SelectedWeapon, "移動後"))
                //        {
                //            SelectedWeapon = -1;
                //        }
                //    }
                //    else
                //    {
                //        if (!currentUnit.IsWeaponAvailable(SelectedWeapon, "移動前"))
                //        {
                //            SelectedWeapon = -1;
                //        }
                //    }

                //    if (SelectedWeapon > 0)
                //    {
                //        if (!currentUnit.IsTargetWithinRange(SelectedWeapon, SelectedTarget))
                //        {
                //            SelectedWeapon = 0;
                //        }
                //    }

                //    // 魅了された場合
                //    if (currentUnit.IsConditionSatisfied("魅了") & ReferenceEquals(currentUnit.Master, SelectedTarget))
                //    {
                //        SelectedWeapon = -1;
                //    }

                //    // 憑依された場合
                //    if (currentUnit.IsConditionSatisfied("憑依"))
                //    {
                //        if ((currentUnit.Master.Party ?? "") == (SelectedTarget.Party0 ?? ""))
                //        {
                //            SelectedWeapon = -1;
                //        }
                //    }

                //    // 踊らされた場合
                //    if (currentUnit.IsConditionSatisfied("踊り"))
                //    {
                //        SelectedWeapon = -1;
                //    }

                if (SelectedWeapon > 0)
                {
                    //if (SupportAttackUnit is object & currentUnit.MaxSyncAttack() > currentUnit.UsedSyncAttack)
                    //{
                    //    // 同時援護攻撃
                    //    currentUnit.Attack(SelectedWeapon, SelectedTarget, "統率", def_mode);
                    //}
                    //else
                    {
                        // 通常攻撃
                        currentUnit.Attack(currentWeapon, SelectedTarget, "", def_mode);
                    }
                }
                //else if (SelectedWeapon == 0)
                //{
                //    if (currentUnit.IsAnimationDefined("射程外", sub_situation: ""))
                //    {
                //        currentUnit.PlayAnimation("射程外", sub_situation: "");
                //    }
                //    else
                //    {
                //        currentUnit.SpecialEffect("射程外", sub_situation: "");
                //    }
                //    currentUnit.PilotMessage("射程外", msg_mode: "");
                //}
                //}
                //    else
                //{
                //    SelectedWeapon = -1;
                //}

                SelectedUnit = currentUnit.CurrentForm();

                // 防御側のユニットがかばわれた場合は2番目の防御側ユニットとして記録
                if (SupportGuardUnit is object)
                {
                    defense_target2 = SupportGuardUnit;
                    defense_target2_hp_ratio = SupportGuardUnitHPRatio;
                }
            }

            //// 同時攻撃
            //if (SupportAttackUnit is object)
            //{
            //    if (SupportAttackUnit.Status != "出撃" | SelectedUnit.Status != "出撃" | SelectedTarget.Status != "出撃")
            //    {
            //        SupportAttackUnit = null;
            //    }
            //}

            //if (SupportAttackUnit is object)
            //{
            //    if (SelectedUnit.MaxSyncAttack() > SelectedUnit.UsedSyncAttack)
            //    {
            //        {
            //            var withBlock8 = SupportAttackUnit;
            //            // サポートアタックに使う武器を決定
            //            w2 = COM.SelectWeapon(SupportAttackUnit, SelectedTarget, "サポートアタック", max_prob: 0, max_dmg: 0);
            //            if (w2 > 0)
            //            {
            //                // サポートアタックを実施
            //                Map.MaskData[withBlock8.x, withBlock8.y] = false;
            //                if (!SRC.BattleAnimation)
            //                {
            //                    GUI.MaskScreen();
            //                }

            //                if (withBlock8.IsAnimationDefined("サポートアタック開始", sub_situation: ""))
            //                {
            //                    withBlock8.PlayAnimation("サポートアタック開始", sub_situation: "");
            //                }

            //                GUI.UpdateMessageForm(SelectedTarget, SupportAttackUnit);
            //                withBlock8.Attack(w2, SelectedTarget, "同時援護攻撃", def_mode);
            //            }
            //        }

            //        // 後始末
            //        {
            //            var withBlock9 = SupportAttackUnit.CurrentForm();
            //            if (w2 > 0)
            //            {
            //                if (withBlock9.IsAnimationDefined("サポートアタック終了", sub_situation: ""))
            //                {
            //                    withBlock9.PlayAnimation("サポートアタック終了", sub_situation: "");
            //                }

            //                // サポートアタックの残り回数を減らす
            //                withBlock9.UsedSupportAttack = (withBlock9.UsedSupportAttack + 1);

            //                // 同時援護攻撃の残り回数を減らす
            //                SelectedUnit.UsedSyncAttack = (SelectedUnit.UsedSyncAttack + 1);
            //            }
            //        }

            //        support_attack_done = true;

            //        // 防御側のユニットがかばわれた場合は本来の防御ユニットデータと
            //        // 入れ替えて記録
            //        if (SupportGuardUnit is object)
            //        {
            //            defense_target = SupportGuardUnit;
            //            defense_target_hp_ratio = SupportGuardUnitHPRatio;
            //        }
            //    }
            //}

            // 反撃の実施
            {
                var targetUnit = SelectedTarget;
                var targetWeapon = targetUnit.Weapon(SelectedTWeapon);
                if (def_mode != "先制攻撃")
                {
                    if (targetUnit.Status == "出撃" & targetUnit.Party != "味方" & SelectedUnit.Status == "出撃")
                    {
                        // まだ武器は使用可能か？
                        if (SelectedTWeapon > 0)
                        {
                            if (SelectedTWeapon > targetUnit.CountWeapon())
                            {
                                SelectedTWeapon = -1;
                            }
                            else if ((twname ?? "") != (targetUnit.Weapon(SelectedTWeapon).Name ?? "")
                                || !targetWeapon.IsWeaponAvailable("移動前"))
                            {
                                SelectedTWeapon = -1;
                            }
                        }

                        if (SelectedTWeapon > 0)
                        {
                            if (!targetWeapon.IsTargetWithinRange(SelectedUnit))
                            {
                                // 敵が射程外に逃げていたら武器を再選択
                                SelectedTWeapon = COM.SelectWeapon(SelectedTarget, SelectedUnit, "反撃", out _, out _);
                                targetWeapon = targetUnit.Weapon(SelectedTWeapon);
                            }
                        }

                        // 行動不能な場合
                        if (targetUnit.MaxAction() == 0)
                        {
                            SelectedTWeapon = -1;
                        }

                        // 魅了された場合
                        if (targetUnit.IsConditionSatisfied("魅了") & ReferenceEquals(targetUnit.Master, SelectedUnit))
                        {
                            SelectedTWeapon = -1;
                        }

                        // 憑依された場合
                        if (targetUnit.IsConditionSatisfied("憑依"))
                        {
                            if ((targetUnit.Master.Party ?? "") == (SelectedUnit.Party ?? ""))
                            {
                                SelectedWeapon = -1;
                            }
                        }

                        // 踊らされた場合
                        if (targetUnit.IsConditionSatisfied("踊り"))
                        {
                            SelectedTWeapon = -1;
                        }

                        if (SelectedTWeapon > 0 & string.IsNullOrEmpty(def_mode))
                        {
                            // 反撃を実施
                            if (targetWeapon.IsWeaponClassifiedAs("自"))
                            {
                                is_suiside = true;
                            }

                            targetUnit.Attack(targetWeapon, SelectedUnit, "", "");

                            // 攻撃側のユニットがかばわれた場合は攻撃側のターゲットを再設定
                            if (SupportGuardUnit2 is object)
                            {
                                attack_target = SupportGuardUnit2;
                                attack_target_hp_ratio = SupportGuardUnitHPRatio2;
                            }
                        }
                        // TODO Impl
                        //else if (SelectedTWeapon == 0 & targetUnit.x == tx & targetUnit.y == ty)
                        //{
                        //    // 反撃出来る武器がなかった場合は射程外メッセージを表示
                        //    if (targetUnit.IsAnimationDefined("射程外", ""))
                        //    {
                        //        targetUnit.PlayAnimation("射程外", "");
                        //    }
                        //    else
                        //    {
                        //        targetUnit.SpecialEffect("射程外", "");
                        //    }

                        //    targetUnit.PilotMessage("射程外", "");
                        //}
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

            //// サポートアタック
            //if (SupportAttackUnit is object)
            //{
            //    if (SupportAttackUnit.Status != "出撃" | SelectedUnit.Status != "出撃" | SelectedTarget.Status != "出撃" | support_attack_done)
            //    {
            //        SupportAttackUnit = null;
            //    }
            //}

            //if (SupportAttackUnit is object)
            //{
            //    {
            //        var withBlock11 = SupportAttackUnit;
            //        // サポートアタックに使う武器を決定
            //        w2 = COM.SelectWeapon(SupportAttackUnit, SelectedTarget, "サポートアタック", max_prob: 0, max_dmg: 0);
            //        if (w2 > 0)
            //        {
            //            // サポートアタックを実施
            //            Map.MaskData[withBlock11.x, withBlock11.y] = false;
            //            if (!SRC.BattleAnimation)
            //            {
            //                GUI.MaskScreen();
            //            }

            //            if (withBlock11.IsAnimationDefined("サポートアタック開始", sub_situation: ""))
            //            {
            //                withBlock11.PlayAnimation("サポートアタック開始", sub_situation: "");
            //            }

            //            GUI.UpdateMessageForm(SelectedTarget, SupportAttackUnit);
            //            withBlock11.Attack(w2, SelectedTarget, "援護攻撃", def_mode);
            //        }
            //    }

            //    // 後始末
            //    {
            //        var withBlock12 = SupportAttackUnit.CurrentForm();
            //        if (withBlock12.IsAnimationDefined("サポートアタック終了", sub_situation: ""))
            //        {
            //            withBlock12.PlayAnimation("サポートアタック終了", sub_situation: "");
            //        }

            //        // サポートアタックの残り回数を減らす
            //        if (w2 > 0)
            //        {
            //            withBlock12.UsedSupportAttack = (withBlock12.UsedSupportAttack + 1);
            //        }
            //    }

            //    // 防御側のユニットがかばわれた場合は本来の防御ユニットデータと
            //    // 入れ替えて記録
            //    if (SupportGuardUnit is object)
            //    {
            //        defense_target = SupportGuardUnit;
            //        defense_target_hp_ratio = SupportGuardUnitHPRatio;
            //    }
            //}

            //SelectedTarget = SelectedTarget.CurrentForm();
            //{
            //    var withBlock13 = SelectedUnit;
            //    if (withBlock13.Status == "出撃")
            //    {
            //        // 攻撃をかけたユニットがまだ生き残っていれば経験値＆資金を獲得

            //        if (SelectedTarget.Status == "破壊" & !is_suiside)
            //        {
            //            // 敵を破壊した場合

            //            // 経験値を獲得
            //            withBlock13.GetExp(SelectedTarget, "破壊", exp_mode: "");
            //            if (!Expression.IsOptionDefined("合体技パートナー経験値無効"))
            //            {
            //                var loopTo2 = Information.UBound(partners);
            //                for (i = 1; i <= loopTo2; i++)
            //                {
            //                    partners[i].CurrentForm().GetExp(SelectedTarget, "破壊", "パートナー");
            //                }
            //            }

            //            // 獲得する資金を算出
            //            earnings = SelectedTarget.Value / 2;

            //            // スペシャルパワーによる獲得資金増加
            //            if (withBlock13.IsUnderSpecialPowerEffect("獲得資金増加"))
            //            {
            //                earnings = (earnings * (1d + 0.1d * withBlock13.SpecialPowerEffectLevel("獲得資金増加")));
            //            }

            //            // パイロット能力による獲得資金増加
            //            if (withBlock13.IsSkillAvailable("資金獲得"))
            //            {
            //                if (!withBlock13.IsUnderSpecialPowerEffect("獲得資金増加") | Expression.IsOptionDefined("収得効果重複"))
            //                {
            //                    earnings = GeneralLib.MinDbl(earnings * ((10d + withBlock13.SkillLevel("資金獲得", 5d)) / 10d), 999999999d);
            //                }
            //            }

            //            // 資金を獲得
            //            SRC.IncrMoney(earnings);
            //            if (earnings > 0)
            //            {
            //                GUI.DisplaySysMessage(SrcFormatter.Format(earnings) + "の" + Expression.Term("資金", SelectedUnit) + "を得た。");
            //            }

            //            // スペシャルパワー効果「敵破壊時再行動」
            //            if (withBlock13.IsUnderSpecialPowerEffect("敵破壊時再行動"))
            //            {
            //                withBlock13.UsedAction = (withBlock13.UsedAction - 1);
            //            }
            //        }
            //        else
            //        {
            //            // 相手を破壊できなかった場合

            //            // 経験値を獲得
            //            withBlock13.GetExp(SelectedTarget, "攻撃", exp_mode: "");
            //            if (!Expression.IsOptionDefined("合体技パートナー経験値無効"))
            //            {
            //                var loopTo3 = Information.UBound(partners);
            //                for (i = 1; i <= loopTo3; i++)
            //                {
            //                    partners[i].CurrentForm().GetExp(SelectedTarget, "攻撃", "パートナー");
            //                }
            //            }
            //        }

            //        // スペシャルパワー「獲得資金増加」「獲得経験値増加」の効果はここで削除する
            //        withBlock13.RemoveSpecialPowerInEffect("戦闘終了");
            //        if (earnings > 0)
            //        {
            //            withBlock13.RemoveSpecialPowerInEffect("敵破壊");
            //        }
            //    }
            //}

            {
                var withBlock14 = SelectedTarget;
                if (withBlock14.Status == "出撃")
                {
                    // 持続期間が「戦闘終了」のスペシャルパワー効果を削除
                    withBlock14.RemoveSpecialPowerInEffect("戦闘終了");
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

            //if (SupportAttackUnit is object)
            //{
            //    {
            //        var withBlock16 = SupportAttackUnit.CurrentForm();
            //        withBlock16.UpdateCondition();
            //        withBlock16.Update();
            //    }
            //}

            //{
            //    var withBlock17 = defense_target.CurrentForm();
            //    withBlock17.UpdateCondition();
            //    withBlock17.Update();
            //}

            //if (defense_target2 is object)
            //{
            //    {
            //        var withBlock18 = defense_target2.CurrentForm();
            //        withBlock18.UpdateCondition();
            //        withBlock18.Update();
            //    }
            //}

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
                if (withBlock19.Status == "破壊")
                {
                    Event.HandleEvent("破壊", withBlock19.MainPilot().ID);
                }
                else if (withBlock19.Status == "出撃" & withBlock19.HP / (double)withBlock19.MaxHP < attack_target_hp_ratio)
                {
                    Event.HandleEvent("損傷率", withBlock19.MainPilot().ID, "" + (100 * (withBlock19.MaxHP - withBlock19.HP) / withBlock19.MaxHP));
                }
            }

            if (SRC.IsScenarioFinished)
            {
                GUI.UnlockGUI();
                SRC.IsScenarioFinished = false;
                SelectedPartners = new Unit[0];
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
                var targetUnit = defense_target.CurrentForm();
                if (targetUnit.CountPilot() > 0)
                {
                    if (targetUnit.Status == "破壊")
                    {
                        Event.HandleEvent("破壊", targetUnit.MainPilot().ID);
                    }
                    else if (targetUnit.Status == "出撃" & targetUnit.HP / (double)targetUnit.MaxHP < defense_target_hp_ratio)
                    {
                        Event.HandleEvent("損傷率", targetUnit.MainPilot().ID, "" + (100 * (targetUnit.MaxHP - targetUnit.HP) / targetUnit.MaxHP));
                    }
                }
            }

            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                SelectedPartners = new Unit[0];
                return;
            }

            //// 攻撃を受けた防御側ユニットその2
            //if (defense_target2 is object)
            //{
            //    if (!ReferenceEquals(defense_target2.CurrentForm(), defense_target.CurrentForm()))
            //    {
            //        {
            //            var withBlock21 = defense_target2.CurrentForm();
            //            if (withBlock21.CountPilot() > 0)
            //            {
            //                if (withBlock21.Status == "破壊")
            //                {
            //                    Event.HandleEvent("破壊", withBlock21.MainPilot().ID);
            //                }
            //                else if (withBlock21.Status == "出撃" & withBlock21.HP / (double)withBlock21.MaxHP < defense_target2_hp_ratio)
            //                {
            //                    Event.HandleEvent("損傷率", withBlock21.MainPilot().ID, 100 * (withBlock21.MaxHP - withBlock21.HP) / withBlock21.MaxHP);
            //                }
            //            }
            //        }
            //    }
            //}

            RestoreSelections();
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                SelectedPartners = new Unit[0];
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                SelectedPartners = new Unit[0];
                GUI.UnlockGUI();
                return;
            }

            // 武器の使用後イベント
            if (SelectedUnit.Status == "出撃" && SelectedWeapon > 0)
            {
                Event.HandleEvent("使用後", SelectedUnit.MainPilot().ID, wname);
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    SelectedPartners = new Unit[0];
                    GUI.UnlockGUI();
                    return;
                }

                if (SRC.IsCanceled)
                {
                    SRC.IsCanceled = false;
                    SelectedPartners = new Unit[0];
                    GUI.UnlockGUI();
                    return;
                }
            }

            if (SelectedTarget.Status == "出撃" & SelectedTWeapon > 0)
            {
                SaveSelections();
                SwapSelections();
                Event.HandleEvent("使用後", SelectedUnit.MainPilot().ID, twname);
                RestoreSelections();
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    SelectedPartners = new Unit[0];
                    GUI.UnlockGUI();
                    return;
                }

                if (SRC.IsCanceled)
                {
                    SRC.IsCanceled = false;
                    SelectedPartners = new Unit[0];
                    GUI.UnlockGUI();
                    return;
                }
            }

            // 攻撃後イベント
            if (SelectedUnit.Status == "出撃" && SelectedTarget.Status == "出撃")
            {
                Event.HandleEvent("攻撃後", SelectedUnit.MainPilot().ID, SelectedTarget.MainPilot().ID);
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    SelectedPartners = new Unit[0];
                    GUI.UnlockGUI();
                    return;
                }

                if (SRC.IsCanceled)
                {
                    SRC.IsCanceled = false;
                    SelectedPartners = new Unit[0];
                    GUI.UnlockGUI();
                    return;
                }
            }

            // もし敵が移動していれば進入イベント
            {
                var targetUnit = SelectedTarget;
                SelectedTarget = null;
                if (targetUnit.Status == "出撃")
                {
                    if (targetUnit.x != tx | targetUnit.y != ty)
                    {
                        Event.HandleEvent("進入", targetUnit.MainPilot().ID, "" + targetUnit.x, "" + targetUnit.y);
                        if (SRC.IsScenarioFinished)
                        {
                            SRC.IsScenarioFinished = false;
                            SelectedPartners = new Unit[0];
                            GUI.UnlockGUI();
                            return;
                        }

                        if (SRC.IsCanceled)
                        {
                            SRC.IsCanceled = false;
                            SelectedPartners = new Unit[0];
                            GUI.UnlockGUI();
                            return;
                        }
                    }
                }
            }

        EndAttack:
            ;


            //// 合体技のパートナーの行動数を減らす
            //if (!Expression.IsOptionDefined("合体技パートナー行動数無消費"))
            //{
            //    var loopTo4 = Information.UBound(partners);
            //    for (i = 1; i <= loopTo4; i++)
            //        partners[i].CurrentForm().UseAction();
            //}

            SelectedPartners = new Unit[0];

            // ハイパーモード＆ノーマルモードの自動発動をチェック
            SRC.UList.CheckAutoHyperMode();
            SRC.UList.CheckAutoNormalMode();

            //// ADD START MARGE
            //// 再移動
            //if (is_p_weapon & SelectedUnit.Status == "出撃")
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

            // 行動数を減らす
            WaitCommand();
        }

        // マップ攻撃による「攻撃」コマンドを終了
        private void MapAttackCommand()
        {
            LogDebug();

            throw new NotImplementedException();
            //// MOD END MARGE
            //int i;
            //var partners = default(Unit[]);
            //// ADD START MARGE
            //bool is_p_weapon;
            //// ADD END MARGE

            //{
            //    var withBlock = SelectedUnit;
            //    // ADD START MARGE
            //    // 移動後使用後可能な武器か記録しておく
            //    is_p_weapon = withBlock.IsWeaponClassifiedAs(SelectedWeapon, "移動後攻撃可");
            //    // ADD END MARGE
            //    // 攻撃目標地点を選択して初めて攻撃範囲が分かるタイプのマップ攻撃
            //    // の場合は再度プレイヤーの選択を促す必要がある
            //    if (CommandState == "ターゲット選択" | CommandState == "移動後ターゲット選択")
            //    {
            //        if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, "Ｍ投"))
            //        {
            //            if (CommandState == "ターゲット選択")
            //            {
            //                CommandState = "マップ攻撃使用";
            //            }
            //            else
            //            {
            //                CommandState = "移動後マップ攻撃使用";
            //            }

            //            // 攻撃目標地点
            //            SelectedX = GUI.PixelToMapX(GUI.MouseX);
            //            SelectedY = GUI.PixelToMapY(GUI.MouseY);

            //            // 攻撃範囲を設定
            //            if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, "識") | withBlock.IsUnderSpecialPowerEffect("識別攻撃"))
            //            {
            //                Map.AreaInRange(SelectedX, SelectedY, withBlock.WeaponLevel(SelectedWeapon, "Ｍ投"), 1, "味方の敵");
            //            }
            //            else
            //            {
            //                Map.AreaInRange(SelectedX, SelectedY, withBlock.WeaponLevel(SelectedWeapon, "Ｍ投"), 1, "すべて");
            //            }

            //            GUI.MaskScreen();
            //            return;
            //        }
            //        else if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, "Ｍ移"))
            //        {
            //            // 攻撃目標地点
            //            SelectedX = GUI.PixelToMapX(GUI.MouseX);
            //            SelectedY = GUI.PixelToMapY(GUI.MouseY);

            //            // 攻撃目標地点に他のユニットがいては駄目
            //            if (Map.MapDataForUnit[SelectedX, SelectedY] is object)
            //            {
            //                GUI.MaskScreen();
            //                return;
            //            }

            //            if (CommandState == "ターゲット選択")
            //            {
            //                CommandState = "マップ攻撃使用";
            //            }
            //            else
            //            {
            //                CommandState = "移動後マップ攻撃使用";
            //            }

            //            // 攻撃範囲を設定
            //            Map.AreaInPointToPoint(withBlock.x, withBlock.y, SelectedX, SelectedY);
            //            GUI.MaskScreen();
            //            return;
            //        }
            //        else if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, "Ｍ線"))
            //        {
            //            if (CommandState == "ターゲット選択")
            //            {
            //                CommandState = "マップ攻撃使用";
            //            }
            //            else
            //            {
            //                CommandState = "移動後マップ攻撃使用";
            //            }

            //            // 攻撃目標地点
            //            SelectedX = GUI.PixelToMapX(GUI.MouseX);
            //            SelectedY = GUI.PixelToMapY(GUI.MouseY);

            //            // 攻撃範囲を設定
            //            Map.AreaInPointToPoint(withBlock.x, withBlock.y, SelectedX, SelectedY);
            //            GUI.MaskScreen();
            //            return;
            //        }
            //    }

            //    // 合体技パートナーの設定
            //    if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, "合"))
            //    {
            //        withBlock.CombinationPartner("武装", SelectedWeapon, partners);
            //    }
            //    else
            //    {
            //        SelectedPartners = new Unit[0];
            //        partners = new Unit[1];
            //    }

            //    if (GUI.MainWidth != 15)
            //    {
            //        Status.ClearUnitStatus();
            //    }

            //    GUI.LockGUI();
            //    SelectedTWeapon = 0;

            //    // マップ攻撃による攻撃を行う
            //    withBlock.MapAttack(SelectedWeapon, SelectedX, SelectedY);
            //    SelectedUnit = withBlock.CurrentForm();
            //    SelectedTarget = null;
            //    if (SRC.IsScenarioFinished)
            //    {
            //        SRC.IsScenarioFinished = false;
            //        SelectedPartners = new Unit[0];
            //        GUI.UnlockGUI();
            //        return;
            //    }

            //    if (SRC.IsCanceled)
            //    {
            //        SRC.IsCanceled = false;
            //        SelectedPartners = new Unit[0];
            //        WaitCommand();
            //        return;
            //    }
            //}

            //// 合体技のパートナーの行動数を減らす
            //if (!Expression.IsOptionDefined("合体技パートナー行動数無消費"))
            //{
            //    var loopTo = Information.UBound(partners);
            //    for (i = 1; i <= loopTo; i++)
            //        partners[i].CurrentForm().UseAction();
            //}

            //SelectedPartners = new Unit[0];

            //// ADD START MARGE
            //// 再移動
            //if (is_p_weapon & SelectedUnit.Status == "出撃")
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
