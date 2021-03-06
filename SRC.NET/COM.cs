﻿using System;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    static class COM
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // コンピューターの思考ルーチン関連モジュール


        // コンピューターによるユニット操作(１行動)
        public static void OperateUnit()
        {
            short j, i, tmp;
            short w = default, tw;
            string wname, twname = default;
            Unit u;
            short tmp_w;
            int prob = default, dmg = default;
            int tprob = default, tdmg = default;
            int max_prob, max_dmg;
            short max_range, min_range;
            short dst_x = default, dst_y = default;
            short new_x = default, new_y = default;
            short new_x0 = default, new_y0 = default;
            short tx = default, ty = default;
            short new_locations_value;
            short distance;
            string def_mode;
            var is_suiside = default(bool);
            bool moved = default, transfered = default;
            string mmode;
            bool searched_enemy = default, searched_nearest_enemy = default;
            var guard_unit_mode = default(bool);
            string buf;
            var partners = default(Unit[]);
            int prev_money, earnings = default;
            string BGM;
            Unit attack_target;
            double attack_target_hp_ratio;
            Unit defense_target;
            double defense_target_hp_ratio;
            Unit defense_target2;
            var defense_target2_hp_ratio = default(double);
            var support_attack_done = default(bool);
            short w2;
            bool indirect_attack;
            bool is_p_weapon;
            var took_action = default(bool);
            Event_Renamed.SelectedUnitForEvent = Commands.SelectedUnit;
            // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Commands.SelectedTarget = null;
            // UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Event_Renamed.SelectedTargetForEvent = null;
            Commands.SelectedWeapon = 0;
            Commands.SelectedTWeapon = 0;
            Commands.SelectedAbility = 0;
            Commands.SelectedUnitMoveCost = 0;

            // まずはUpdate
            Commands.SelectedUnit.Update();

            // 行動出来なければそのまま終了
            if (Commands.SelectedUnit.MaxAction() == 0)
            {
                return;
            }

            // 踊っている？
            object argIndex1 = "踊り";
            if (Commands.SelectedUnit.IsConditionSatisfied(ref argIndex1))
            {
                // 踊りに忙しい……
                return;
            }

            // スペシャルパワーを使う？
            string argoname = "敵ユニットスペシャルパワー使用";
            string argoname1 = "敵ユニット精神コマンド使用";
            if (Expression.IsOptionDefined(ref argoname) | Expression.IsOptionDefined(ref argoname1))
            {
                TrySpecialPower(ref Commands.SelectedUnit.MainPilot());
                if (SRC.IsScenarioFinished | SRC.IsCanceled)
                {
                    return;
                }

                var loopTo = Commands.SelectedUnit.CountPilot();
                for (i = 2; i <= loopTo; i++)
                {
                    Pilot localPilot() { object argIndex1 = i; var ret = Commands.SelectedUnit.Pilot(ref argIndex1); return ret; }

                    var argp = localPilot();
                    TrySpecialPower(ref argp);
                    if (SRC.IsScenarioFinished | SRC.IsCanceled)
                    {
                        return;
                    }
                }

                var loopTo1 = Commands.SelectedUnit.CountSupport();
                for (i = 1; i <= loopTo1; i++)
                {
                    Pilot localSupport() { object argIndex1 = i; var ret = Commands.SelectedUnit.Support(ref argIndex1); return ret; }

                    var argp1 = localSupport();
                    TrySpecialPower(ref argp1);
                    if (SRC.IsScenarioFinished | SRC.IsCanceled)
                    {
                        return;
                    }
                }

                string argfname = "追加サポート";
                if (Commands.SelectedUnit.IsFeatureAvailable(ref argfname))
                {
                    TrySpecialPower(ref Commands.SelectedUnit.AdditionalSupport());
                    if (SRC.IsScenarioFinished | SRC.IsCanceled)
                    {
                        return;
                    }
                }
            }

            // ハイパーモードが可能であればハイパーモード発動
            TryHyperMode();

            // 特殊な思考モードの場合の処理
            {
                var withBlock = Commands.SelectedUnit;
                // 指定された地点を目指す場合
                short localLLength() { string arglist = withBlock.Mode; var ret = GeneralLib.LLength(ref arglist); withBlock.Mode = arglist; return ret; }

                if (localLLength() == 2)
                {
                    string localLIndex() { string arglist = withBlock.Mode; var ret = GeneralLib.LIndex(ref arglist, 1); withBlock.Mode = arglist; return ret; }

                    string localLIndex1() { string arglist = withBlock.Mode; var ret = GeneralLib.LIndex(ref arglist, 1); withBlock.Mode = arglist; return ret; }

                    dst_x = Conversions.ToShort(localLIndex1());
                    string localLIndex2() { string arglist = withBlock.Mode; var ret = GeneralLib.LIndex(ref arglist, 2); withBlock.Mode = arglist; return ret; }

                    string localLIndex3() { string arglist = withBlock.Mode; var ret = GeneralLib.LIndex(ref arglist, 2); withBlock.Mode = arglist; return ret; }

                    dst_y = Conversions.ToShort(localLIndex3());
                    if (1 <= dst_x & dst_x <= Map.MapWidth & 1 <= dst_y & dst_y <= Map.MapHeight)
                    {
                        goto Move;
                    }
                }

                // 逃亡し続ける場合
                if (withBlock.Mode == "逃亡")
                {
                    goto Move;
                }

                // 思考モードが「パイロット名」の場合の処理
                bool localIsDefined() { object argIndex1 = withBlock.Mode; var ret = SRC.PList.IsDefined(ref argIndex1); withBlock.Mode = Conversions.ToString(argIndex1); return ret; }

                if (!localIsDefined())
                {
                    goto TryBattleTransform;
                }

                Pilot localItem() { object argIndex1 = withBlock.Mode; var ret = SRC.PList.Item(ref argIndex1); withBlock.Mode = Conversions.ToString(argIndex1); return ret; }

                if (localItem().Unit_Renamed is null)
                {
                    goto TryBattleTransform;
                }

                Pilot localItem1() { object argIndex1 = withBlock.Mode; var ret = SRC.PList.Item(ref argIndex1); withBlock.Mode = Conversions.ToString(argIndex1); return ret; }

                if (localItem1().Unit_Renamed.Status_Renamed != "出撃")
                {
                    goto TryBattleTransform;
                }

                Pilot localItem2() { object argIndex1 = withBlock.Mode; var ret = SRC.PList.Item(ref argIndex1); withBlock.Mode = Conversions.ToString(argIndex1); return ret; }

                Commands.SelectedTarget = localItem2().Unit_Renamed;
                Map.AreaInSpeed(ref Commands.SelectedUnit);
                if (!withBlock.IsAlly(ref Commands.SelectedTarget))
                {
                    // ユニットが敵の場合はそのユニットを狙う
                    string argamode = "移動可能";
                    int argmax_prob = 0;
                    int argmax_dmg = 0;
                    w = SelectWeapon(ref Commands.SelectedUnit, ref Commands.SelectedTarget, ref argamode, max_prob: ref argmax_prob, max_dmg: ref argmax_dmg);
                    if (w == 0)
                    {
                        dst_x = Commands.SelectedTarget.x;
                        dst_y = Commands.SelectedTarget.y;
                        goto Move;
                    }
                }
                else
                {
                    // ユニットが味方の場合はそのユニットを護衛
                    w = 0;
                    distance = 1000;
                    dst_x = Commands.SelectedTarget.x;
                    dst_y = Commands.SelectedTarget.y;
                    max_prob = 0;
                    max_dmg = 0;

                    // 護衛対象が損傷している場合は修理装置を使う
                    if (TryFix(ref moved, ref Commands.SelectedTarget))
                    {
                        goto EndOfOperation;
                    }

                    // 護衛対象が損傷している場合は回復アビリティを使う
                    if (TryHealing(ref moved, ref Commands.SelectedTarget))
                    {
                        goto EndOfOperation;
                    }

                    // 合体技や援護防御を持っている場合はとにかく護衛対象に
                    // 隣接することを優先する
                    string argsname = "援護";
                    string argsname1 = "援護防御";
                    if (withBlock.MainPilot().IsSkillAvailable(ref argsname) | withBlock.MainPilot().IsSkillAvailable(ref argsname1))
                    {
                        if (Math.Abs((short)(withBlock.x - dst_x)) + Math.Abs((short)(withBlock.y - dst_y)) > 1)
                        {
                            goto Move;
                        }

                        guard_unit_mode = true;
                    }

                    string argfname1 = "合体技";
                    if (withBlock.IsFeatureAvailable(ref argfname1))
                    {
                        if (Math.Abs((short)(withBlock.x - dst_x)) > 1 | Math.Abs((short)(withBlock.y - dst_y)) > 1)
                        {
                            goto Move;
                        }

                        guard_unit_mode = true;
                    }

                    if (guard_unit_mode)
                    {
                        // ちゃんと隣接しているので周りの敵を排除
                        // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        Commands.SelectedTarget = null;
                        goto TryBattleTransform;
                    }

                    // 護衛するユニットを脅かすユニットが存在するかチェック
                    foreach (Unit currentU in SRC.UList)
                    {
                        u = currentU;
                        {
                            var withBlock1 = u;
                            if (withBlock1.Status_Renamed == "出撃" & Commands.SelectedUnit.IsEnemy(ref u) & Math.Abs((short)(dst_x - withBlock1.x)) + Math.Abs((short)(dst_y - withBlock1.y)) <= 5)
                            {
                                string argamode1 = "移動可能";
                                tmp_w = SelectWeapon(ref Commands.SelectedUnit, ref u, ref argamode1, ref prob, ref dmg);
                            }
                            else
                            {
                                tmp_w = 0;
                            }

                            if (tmp_w > 0)
                            {
                                // 脅威となり得るユニットと認定
                                if (distance > (short)(Math.Abs((short)(dst_x - withBlock1.x)) + Math.Abs((short)(dst_y - withBlock1.y))))
                                {
                                    // 近い位置にいるユニットを優先
                                    Commands.SelectedTarget = u;
                                    w = tmp_w;
                                    distance = (short)(Math.Abs((short)(dst_x - withBlock1.x)) + Math.Abs((short)(dst_y - withBlock1.y)));
                                    max_prob = prob;
                                    max_dmg = dmg;
                                }
                                else if (distance == (short)(Math.Abs((short)(dst_x - withBlock1.x)) + Math.Abs((short)(dst_y - withBlock1.y))))
                                {
                                    // 今までに見つかったユニットと位置が変わらなければ
                                    // より危険度が高いユニットを優先
                                    if (prob > max_prob & prob > 50)
                                    {
                                        Commands.SelectedTarget = u;
                                        w = tmp_w;
                                        max_prob = prob;
                                    }
                                    else if (max_prob == 0 & dmg > max_dmg)
                                    {
                                        Commands.SelectedTarget = u;
                                        w = tmp_w;
                                        max_dmg = dmg;
                                    }
                                }
                            }
                        }
                    }

                    if (w == 0)
                    {
                        // 護衛するユニットは安全。護衛するユニットの近くへ移動する
                        goto Move;
                    }
                    else
                    {
                        // 護衛するユニットを脅かすユニットに攻撃
                        goto AttackEnemy;
                    }
                }
            }

            TryBattleTransform:
            ;

            // 戦闘形態への変形が可能であれば変形
            if (TryBattleTransform())
            {
                transfered = true;
                // 既にターゲットを選択している場合は攻撃方法を再選択
                if (w > 0)
                {
                    string argamode2 = "移動可能";
                    int argmax_prob1 = 0;
                    int argmax_dmg1 = 0;
                    w = SelectWeapon(ref Commands.SelectedUnit, ref Commands.SelectedTarget, ref argamode2, max_prob: ref argmax_prob1, max_dmg: ref argmax_dmg1);
                    if (w == 0)
                    {
                        // 変形の結果、攻撃できなくなってしまった……
                        dst_x = Commands.SelectedTarget.x;
                        dst_y = Commands.SelectedTarget.y;
                        goto Move;
                    }
                }
            }

            // 実行時間を消費しないアビリティがあれば使っておく
            TryInstantAbility();
            if (SRC.IsScenarioFinished | SRC.IsCanceled)
            {
                return;
            }

            {
                var withBlock2 = Commands.SelectedUnit;
                if (withBlock2.HP == 0 | withBlock2.MaxAction() == 0)
                {
                    goto EndOfOperation;
                }
            }

            // 既に目標が決まっていればその目標を攻撃
            if (Commands.SelectedTarget is object)
            {
                goto AttackEnemy;
            }

            // 召喚が可能であれば召喚
            if (TrySummonning())
            {
                if (SRC.IsScenarioFinished | SRC.IsCanceled)
                {
                    return;
                }

                goto EndOfOperation;
            }

            // 修理が可能であれば修理装置を使う
            Unit argt = null;
            if (TryFix(ref moved, t: ref argt))
            {
                goto EndOfOperation;
            }

            // マップ型回復アビリティを使う？
            if (TryMapHealing(ref moved))
            {
                if (SRC.IsScenarioFinished | SRC.IsCanceled)
                {
                    return;
                }

                goto EndOfOperation;
            }

            // 回復アビリティを使う？
            Unit argt1 = null;
            if (TryHealing(ref moved, t: ref argt1))
            {
                if (SRC.IsScenarioFinished | SRC.IsCanceled)
                {
                    return;
                }

                goto EndOfOperation;
            }

            TryMapAttack:
            ;

            // マップ攻撃を使う？
            if (TryMapAttack(ref moved))
            {
                goto EndOfOperation;
            }

            SearchNearestEnemyWithinRange:
            ;

            // ターゲットにするユニットを探す
            {
                var withBlock3 = Commands.SelectedUnit;
                Map.AreaInSpeed(ref Commands.SelectedUnit);

                // 護衛すべきユニットがいる場合は移動範囲を限定
                if (guard_unit_mode)
                {
                    Pilot localItem3() { object argIndex1 = withBlock3.Mode; var ret = SRC.PList.Item(ref argIndex1); withBlock3.Mode = Conversions.ToString(argIndex1); return ret; }

                    {
                        var withBlock4 = localItem3().Unit_Renamed;
                        var loopTo2 = Map.MapWidth;
                        for (i = 1; i <= loopTo2; i++)
                        {
                            var loopTo3 = Map.MapHeight;
                            for (j = 1; j <= loopTo3; j++)
                            {
                                if (!Map.MaskData[i, j])
                                {
                                    if (Math.Abs((short)(withBlock4.x - i)) + Math.Abs((short)(withBlock4.y - j)) > 1)
                                    {
                                        Map.MaskData[i, j] = true;
                                    }
                                }
                            }
                        }
                    }
                }

                // 個々のユニットに対してターゲットとなり得るか判定
                // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                Commands.SelectedTarget = null;
                w = 0;
                max_prob = 0;
                max_dmg = 0;
                foreach (Unit currentU1 in SRC.UList)
                {
                    u = currentU1;
                    if (u.Status_Renamed != "出撃")
                    {
                        goto NextLoop;
                    }

                    // 敵かどうかを判定
                    if (withBlock3.IsAlly(ref u))
                    {
                        goto NextLoop;
                    }

                    // 特定の陣営のみを狙う思考モードの場合
                    switch (withBlock3.Mode ?? "")
                    {
                        case "味方":
                            {
                                if (u.Party != "味方" & u.Party != "ＮＰＣ")
                                {
                                    goto NextLoop;
                                }

                                break;
                            }

                        case "ＮＰＣ":
                        case "敵":
                        case "中立":
                            {
                                if ((u.Party ?? "") != (withBlock3.Mode ?? ""))
                                {
                                    goto NextLoop;
                                }

                                break;
                            }
                    }

                    // 自分自身には攻撃しない
                    if (ReferenceEquals(Commands.SelectedUnit.CurrentForm(), u.CurrentForm()))
                    {
                        goto NextLoop;
                    }

                    // 隠れ身中
                    string argsptype = "隠れ身";
                    if (u.IsUnderSpecialPowerEffect(ref argsptype))
                    {
                        goto NextLoop;
                    }

                    // ステルスの敵は遠距離からは攻撃を受けない
                    string argfname2 = "ステルス";
                    object argIndex4 = "ステルス無効";
                    string argfname3 = "ステルス無効化";
                    if (u.IsFeatureAvailable(ref argfname2) & !u.IsConditionSatisfied(ref argIndex4) & !withBlock3.IsFeatureAvailable(ref argfname3))
                    {
                        object argIndex3 = "ステルス";
                        if (u.IsFeatureLevelSpecified(ref argIndex3))
                        {
                            object argIndex2 = "ステルス";
                            if (Math.Abs((short)(withBlock3.x - u.x)) + Math.Abs((short)(withBlock3.y - u.y)) > u.FeatureLevel(ref argIndex2))
                            {
                                goto NextLoop;
                            }
                        }
                        else if (Math.Abs((short)(withBlock3.x - u.x)) + Math.Abs((short)(withBlock3.y - u.y)) > 3)
                        {
                            goto NextLoop;
                        }
                    }

                    // 攻撃に使う武器を選択
                    if (moved)
                    {
                        string argamode3 = "移動後";
                        tmp_w = SelectWeapon(ref Commands.SelectedUnit, ref u, ref argamode3, ref prob, ref dmg);
                    }
                    else
                    {
                        string argamode4 = "移動可能";
                        tmp_w = SelectWeapon(ref Commands.SelectedUnit, ref u, ref argamode4, ref prob, ref dmg);
                    }

                    if (tmp_w <= 0)
                    {
                        goto NextLoop;
                    }

                    // サポートガードされる？
                    if (withBlock3.MainPilot().TacticalTechnique() >= 150)
                    {
                        if (u.LookForSupportGuard(ref Commands.SelectedUnit, tmp_w) is object)
                        {
                            // 相手を破壊することは出来ない
                            prob = 0;
                            // 仮想的にダメージを半減して判定
                            dmg = dmg / 2;
                        }
                    }

                    // 間接攻撃？
                    string argattr = "間";
                    indirect_attack = withBlock3.IsWeaponClassifiedAs(w, ref argattr);

                    // 召喚ユニットは自分がやられてしまうような攻撃はかけない
                    string argfname4 = "召喚ユニット";
                    object argIndex5 = "暴走";
                    object argIndex6 = "混乱";
                    object argIndex7 = "狂戦士";
                    if (withBlock3.Party == "ＮＰＣ" & withBlock3.IsFeatureAvailable(ref argfname4) & !withBlock3.IsConditionSatisfied(ref argIndex5) & !withBlock3.IsConditionSatisfied(ref argIndex6) & !withBlock3.IsConditionSatisfied(ref argIndex7) & !indirect_attack)
                    {
                        string argamode5 = "反撃";
                        tw = SelectWeapon(ref u, ref Commands.SelectedUnit, ref argamode5, ref tprob, ref tdmg);
                        if (prob < 80 & tprob > prob)
                        {
                            goto NextLoop;
                        }
                    }

                    // 破壊確率が50%以上であれば破壊確率が高いユニットを優先
                    // そうでなければダメージの期待値が高いユニットを優先
                    if (prob > 50)
                    {
                        // 重要なユニットは優先してターゲットにする
                        if (withBlock3.MainPilot().TacticalTechnique() >= 150)
                        {
                            string argsname2 = "指揮";
                            string argsname3 = "広域サポート";
                            string argfname5 = "修理装置";
                            if (u.MainPilot().IsSkillAvailable(ref argsname2) | u.MainPilot().IsSkillAvailable(ref argsname3) | u.IsFeatureAvailable(ref argfname5))
                            {
                                prob = (int)(1.5d * prob);
                            }
                            else
                            {
                                // 回復アビリティを持っている？
                                var loopTo4 = u.CountAbility();
                                for (i = 1; i <= loopTo4; i++)
                                {
                                    {
                                        var withBlock5 = u.Ability(i);
                                        if (withBlock5.MaxRange > 0)
                                        {
                                            if (withBlock5.CountEffect() > 0)
                                            {
                                                object argIndex8 = 1;
                                                if (withBlock5.EffectType(ref argIndex8) == "回復")
                                                {
                                                    prob = (int)(1.5d * prob);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (prob > max_prob)
                        {
                            Commands.SelectedTarget = u;
                            w = tmp_w;
                            max_prob = prob;
                        }
                    }
                    else if (max_prob == 0)
                    {
                        // 相手の反撃手段もチェック
                        tw = 0;
                        var loopTo5 = u.CountWeapon();
                        for (i = 1; i <= loopTo5; i++)
                        {
                            string argref_mode = "移動前";
                            string argattr2 = "Ｍ";
                            if (u.IsWeaponAvailable(i, ref argref_mode) & !u.IsWeaponClassifiedAs(i, ref argattr2))
                            {
                                string argattr1 = "移動後攻撃可";
                                if (!moved & withBlock3.Mode != "固定" & withBlock3.IsWeaponClassifiedAs(tmp_w, ref argattr1))
                                {
                                    if (u.WeaponMaxRange(i) >= withBlock3.WeaponMaxRange(tmp_w))
                                    {
                                        tw = i;
                                        break;
                                    }
                                }
                                else if (u.WeaponMaxRange(i) >= (short)(Math.Abs((short)(withBlock3.x - u.x)) + Math.Abs((short)(withBlock3.y - u.y))))
                                {
                                    tw = i;
                                    break;
                                }
                            }
                        }

                        // 間接攻撃には反撃不能
                        if (indirect_attack)
                        {
                            tw = 0;
                        }

                        // ステータス異常により反撃不能？
                        object argIndex9 = "攻撃不能";
                        if (u.MaxAction() == 0 | u.IsConditionSatisfied(ref argIndex9))
                        {
                            tw = 0;
                        }

                        // 反撃してこない？
                        if (tw == 0)
                        {
                            dmg = (int)(1.5d * dmg);
                        }

                        // 重要なユニットは優先してターゲットにする
                        if (withBlock3.MainPilot().TacticalTechnique() >= 150)
                        {
                            string argsname4 = "指揮";
                            string argsname5 = "広域サポート";
                            string argfname6 = "修理装置";
                            if (u.MainPilot().IsSkillAvailable(ref argsname4) | u.MainPilot().IsSkillAvailable(ref argsname5) | u.IsFeatureAvailable(ref argfname6))
                            {
                                // メインパイロットが指揮や広域サポートを有していたり
                                // 修理装置を持っていれば重要ユニットと認定
                                dmg = (int)(1.5d * dmg);
                            }
                            else
                            {
                                // 回復アビリティを持っている場合も重要ユニットと認定
                                var loopTo6 = u.CountAbility();
                                for (i = 1; i <= loopTo6; i++)
                                {
                                    {
                                        var withBlock6 = u.Ability(i);
                                        if (withBlock6.MaxRange > 0)
                                        {
                                            if (withBlock6.CountEffect() > 0)
                                            {
                                                object argIndex10 = 1;
                                                if (withBlock6.EffectType(ref argIndex10) == "回復")
                                                {
                                                    dmg = (int)(1.5d * dmg);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (dmg >= max_dmg)
                        {
                            // 現在のユニットをターゲットに設定
                            Commands.SelectedTarget = u;
                            w = tmp_w;
                            max_dmg = dmg;
                        }
                    }

                    NextLoop:
                    ;
                }

                // 射程内に敵がいなければ移動、もしくは待機
                if (Commands.SelectedTarget is null)
                {
                    short localLLength1() { string arglist = withBlock3.Mode; var ret = GeneralLib.LLength(ref arglist); withBlock3.Mode = arglist; return ret; }

                    if (withBlock3.Mode == "待機" | withBlock3.Mode == "固定" | localLLength1() == 2)
                    {
                        goto EndOfOperation;
                    }

                    if (moved)
                    {
                        // 既に移動済みであればここで終了
                        goto EndOfOperation;
                    }

                    if (searched_enemy)
                    {
                        // 既に索敵済みであればここで終了
                        goto EndOfOperation;
                    }

                    // 一度索敵をしたことを記録
                    searched_enemy = true;

                    // 一番近い敵の方へ移動する
                    goto SearchNearestEnemy;
                }

                searched_enemy = true;
            }

            AttackEnemy:
            ;

            // 敵を攻撃

            // 敵をUpdate
            Commands.SelectedTarget.Update();

            // 敵の位置を記録しておく
            tx = Commands.SelectedTarget.x;
            ty = Commands.SelectedTarget.y;
            string[] list;
            string caption_msg;
            short hit_prob, crit_prob;
            {
                var withBlock7 = Commands.SelectedUnit;
                // 移動後攻撃可能な武器の場合は攻撃前に移動を行う
                // ただし合体技は移動後の位置によって攻撃できない場合があるので例外
                string argattr3 = "移動後攻撃可";
                string argattr4 = "合";
                if (withBlock7.IsWeaponClassifiedAs(w, ref argattr3) & !withBlock7.IsWeaponClassifiedAs(w, ref argattr4) & !moved & withBlock7.Mode != "固定")
                {
                    // 移動しなくても攻撃出来る場合は現在位置をデフォルトの攻撃位置に設定
                    if (withBlock7.IsTargetWithinRange(w, ref Commands.SelectedTarget))
                    {
                        new_locations_value = (short)(Map.TerrainEffectForHPRecover(withBlock7.x, withBlock7.y) + Map.TerrainEffectForENRecover(withBlock7.x, withBlock7.y) + 100 * withBlock7.LookForSupport(withBlock7.x, withBlock7.y, true));
                        if (withBlock7.Area != "空中")
                        {
                            // 地形による防御効果は空中にいる場合は受けられない
                            new_locations_value = (short)((short)(new_locations_value + Map.TerrainEffectForHit(withBlock7.x, withBlock7.y)) + Map.TerrainEffectForDamage(withBlock7.x, withBlock7.y));
                        }

                        new_x = withBlock7.x;
                        new_y = withBlock7.y;
                    }
                    else
                    {
                        new_locations_value = -1000;
                        new_x = 0;
                        new_y = 0;
                    }

                    // 攻撃をかけられる位置のうち、もっとも地形効果の高い場所を探す
                    // 地形効果が同等ならもっとも近い場所を優先
                    max_range = withBlock7.WeaponMaxRange(w);
                    min_range = withBlock7.Weapon(w).MinRange;
                    var loopTo7 = (short)GeneralLib.MinLng(tx + max_range, Map.MapWidth);
                    for (i = (short)GeneralLib.MaxLng(1, tx - max_range); i <= loopTo7; i++)
                    {
                        var loopTo8 = (short)GeneralLib.MinLng(ty + (short)(max_range - Math.Abs((short)(tx - i))), Map.MapHeight);
                        for (j = (short)GeneralLib.MaxLng(1, ty - (short)(max_range - Math.Abs((short)(tx - i)))); j <= loopTo8; j++)
                        {
                            if (!Map.MaskData[i, j] & Map.MapDataForUnit[i, j] is null & (short)(Math.Abs((short)(tx - i)) + Math.Abs((short)(ty - j))) >= min_range)
                            {
                                tmp = (short)(Map.TerrainEffectForHPRecover(i, j) + Map.TerrainEffectForENRecover(i, j) + 100 * withBlock7.LookForSupport(i, j, true));
                                if (withBlock7.Area != "空中")
                                {
                                    // 地形による防御効果は空中にいる場合は受けられない
                                    tmp = (short)((short)(tmp + Map.TerrainEffectForHit(i, j)) + Map.TerrainEffectForDamage(i, j));

                                    // 水中は水中用ユニットでない限り選択しない
                                    if (Map.TerrainClass(i, j) == "水")
                                    {
                                        string argarea_name = "水";
                                        if (withBlock7.IsTransAvailable(ref argarea_name))
                                        {
                                            tmp = (short)(tmp + 100);
                                        }
                                        else
                                        {
                                            tmp = -1000;
                                        }
                                    }
                                }

                                // 条件が同じであれば直線距離で近い場所を選択する
                                tmp = (short)(tmp - Math.Sqrt(Math.Pow(Math.Abs((short)(withBlock7.x - i)), 2d) + Math.Pow(Math.Abs((short)(withBlock7.y - j)), 2d)));
                                if (new_locations_value < tmp)
                                {
                                    new_locations_value = tmp;
                                    new_x = i;
                                    new_y = j;
                                }
                            }
                        }
                    }

                    if (new_x == 0 & new_y == 0)
                    {
                        // 攻撃をかけられる位置がない
                        if (searched_nearest_enemy)
                        {
                            // 既に索敵済みであればここで終了
                            goto EndOfOperation;
                        }

                        goto SearchNearestEnemy;
                    }

                    // 見つけた位置に移動
                    if (new_x != withBlock7.x | new_y != withBlock7.y)
                    {
                        withBlock7.Move(new_x, new_y);
                        Commands.SelectedUnitMoveCost = (short)Map.TotalMoveCost[new_x, new_y];
                        moved = true;

                        // 移動のためＥＮ切れ？
                        if (withBlock7.EN == 0)
                        {
                            if (withBlock7.MaxAction() == 0)
                            {
                                goto EndOfOperation;
                            }
                        }

                        // 実はマップ攻撃が使える？
                        bool argmoved = true;
                        if (TryMapAttack(ref argmoved))
                        {
                            goto EndOfOperation;
                        }

                        // 移動のために選択していた武器が使えなくなったり、合体技が使える
                        // ようになったりすることがあるので、武器を再度選択
                        string argamode6 = "移動後";
                        int argmax_prob2 = 0;
                        int argmax_dmg2 = 0;
                        w = SelectWeapon(ref Commands.SelectedUnit, ref Commands.SelectedTarget, ref argamode6, max_prob: ref argmax_prob2, max_dmg: ref argmax_dmg2);
                        if (w == 0)
                        {
                            // 攻撃出来ないので行動終了
                            goto EndOfOperation;
                        }
                    }
                }

                // ユニットを中央表示
                GUI.Center(withBlock7.x, withBlock7.y);

                // ハイライト表示を行う
                if (!SRC.BattleAnimation)
                {
                    // 射程範囲をハイライト
                    // MOD START マージ
                    // AreaInRange .X, .Y, _
                    // '                .Weapon(w).MinRange, _
                    // '                .WeaponMaxRange(w), _
                    // '                "空間"
                    string arguparty = "空間";
                    Map.AreaInRange(withBlock7.x, withBlock7.y, withBlock7.WeaponMaxRange(w), withBlock7.Weapon(w).MinRange, ref arguparty);
                    // MOD END マージ
                }
                // 合体技の場合はパートナーもハイライト表示
                string argattr5 = "合";
                if (withBlock7.IsWeaponClassifiedAs(w, ref argattr5))
                {
                    if (withBlock7.WeaponMaxRange(w) == 1)
                    {
                        string argctype_Renamed = "武装";
                        withBlock7.CombinationPartner(ref argctype_Renamed, w, ref partners, Commands.SelectedTarget.x, Commands.SelectedTarget.y);
                    }
                    else
                    {
                        string argctype_Renamed1 = "武装";
                        withBlock7.CombinationPartner(ref argctype_Renamed1, w, ref partners);
                    }

                    if (!SRC.BattleAnimation)
                    {
                        var loopTo9 = (short)Information.UBound(partners);
                        for (i = 1; i <= loopTo9; i++)
                        {
                            {
                                var withBlock8 = partners[i];
                                Map.MaskData[withBlock8.x, withBlock8.y] = false;
                            }
                        }
                    }
                }
                else
                {
                    Commands.SelectedPartners = new Unit[1];
                    partners = new Unit[1];
                }

                if (!SRC.BattleAnimation)
                {
                    // 自分自身とターゲットもハイライト
                    Map.MaskData[withBlock7.x, withBlock7.y] = false;
                    Map.MaskData[Commands.SelectedTarget.x, Commands.SelectedTarget.y] = false;

                    // ハイライト表示を実施
                    GUI.MaskScreen();
                }
                else
                {
                    // 戦闘アニメを表示する場合はハイライト表示を行わない
                    GUI.RefreshScreen();
                }

                // ＢＧＭを変更
                if (!SRC.KeepEnemyBGM)
                {
                    BGM = "";

                    // ボス用ＢＧＭ？
                    string argfname7 = "ＢＧＭ";
                    if (withBlock7.IsFeatureAvailable(ref argfname7) & Strings.InStr(withBlock7.MainPilot().Name, "(ザコ)") == 0)
                    {
                        object argIndex11 = "ＢＧＭ";
                        string argmidi_name = withBlock7.FeatureData(ref argIndex11);
                        BGM = Sound.SearchMidiFile(ref argmidi_name);
                    }

                    Sound.BossBGM = false;
                    if (Strings.Len(BGM) > 0)
                    {
                        // ボス用ＢＧＭを演奏する場合
                        Sound.ChangeBGM(ref BGM);
                        Sound.BossBGM = true;
                    }
                    else
                    {
                        // 通常の戦闘ＢＧＭ

                        // ターゲットは味方？
                        if (Commands.SelectedTarget.Party == "味方" | Commands.SelectedTarget.Party == "ＮＰＣ" & withBlock7.Party != "ＮＰＣ")
                        {
                            // ターゲットが味方なのでターゲット側を優先
                            string argfname8 = "ＢＧＭ";
                            if (Commands.SelectedTarget.IsFeatureAvailable(ref argfname8))
                            {
                                object argIndex12 = "ＢＧＭ";
                                string argmidi_name1 = Commands.SelectedTarget.FeatureData(ref argIndex12);
                                BGM = Sound.SearchMidiFile(ref argmidi_name1);
                            }

                            if (Strings.Len(BGM) == 0)
                            {
                                string argmidi_name2 = Commands.SelectedTarget.MainPilot().BGM;
                                BGM = Sound.SearchMidiFile(ref argmidi_name2);
                                Commands.SelectedTarget.MainPilot().BGM = argmidi_name2;
                            }
                        }
                        else
                        {
                            // ターゲットが味方でなければ攻撃側を優先
                            string argfname9 = "ＢＧＭ";
                            if (withBlock7.IsFeatureAvailable(ref argfname9))
                            {
                                object argIndex13 = "ＢＧＭ";
                                string argmidi_name3 = withBlock7.FeatureData(ref argIndex13);
                                BGM = Sound.SearchMidiFile(ref argmidi_name3);
                            }

                            if (Strings.Len(BGM) == 0)
                            {
                                string argmidi_name4 = withBlock7.MainPilot().BGM;
                                BGM = Sound.SearchMidiFile(ref argmidi_name4);
                                withBlock7.MainPilot().BGM = argmidi_name4;
                            }
                        }

                        if (Strings.Len(BGM) == 0)
                        {
                            string argbgm_name = "default";
                            BGM = Sound.BGMName(ref argbgm_name);
                        }

                        // ＢＧＭを変更
                        Sound.ChangeBGM(ref BGM);
                    }
                }

                // 移動後攻撃可能？
                string argattr6 = "移動後攻撃可";
                is_p_weapon = withBlock7.IsWeaponClassifiedAs(w, ref argattr6);

                // 間接攻撃？
                string argattr7 = "間";
                indirect_attack = withBlock7.IsWeaponClassifiedAs(w, ref argattr7);

                // 相手の反撃手段を設定
                def_mode = "";
                Commands.UseSupportGuard = true;
                if (Commands.SelectedTarget.MaxAction() == 0)
                {
                    // 行動不能の場合

                    tw = -1;
                    // チャージ中または消耗している場合は自動的に防御
                    string argfname10 = "チャージ";
                    string argfname11 = "消耗";
                    if (Commands.SelectedTarget.Party == "味方" & (Commands.SelectedTarget.IsFeatureAvailable(ref argfname10) | Commands.SelectedTarget.IsFeatureAvailable(ref argfname11)))
                    {
                        def_mode = "防御";
                    }
                }

                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                else if (Commands.SelectedTarget.Party == "味方" & !GUI.MainForm.mnuMapCommandItem(Commands.AutoDefenseCmdID).Checked)
                {
                    // 味方ユニットによる手動反撃を行う場合

                    // 戦闘アニメを表示する場合でも手動反撃時にはハイライト表示を行う
                    if (SRC.BattleAnimation)
                    {
                        // 射程範囲をハイライト
                        // MOD START マージ
                        // AreaInRange .X, .Y, _
                        // '                    .Weapon(w).MinRange, _
                        // '                    .WeaponMaxRange(w), _
                        // '                    "空間"
                        string arguparty1 = "空間";
                        Map.AreaInRange(withBlock7.x, withBlock7.y, withBlock7.WeaponMaxRange(w), withBlock7.Weapon(w).MinRange, ref arguparty1);
                        // MOD END マージ

                        // 合体技の場合はパートナーもハイライト表示
                        string argattr8 = "合";
                        if (withBlock7.IsWeaponClassifiedAs(w, ref argattr8))
                        {
                            var loopTo10 = (short)Information.UBound(partners);
                            for (i = 1; i <= loopTo10; i++)
                            {
                                {
                                    var withBlock9 = partners[i];
                                    Map.MaskData[withBlock9.x, withBlock9.y] = false;
                                }
                            }
                        }

                        // 自分自身とターゲットもハイライト
                        Map.MaskData[withBlock7.x, withBlock7.y] = false;
                        Map.MaskData[Commands.SelectedTarget.x, Commands.SelectedTarget.y] = false;

                        // ハイライト表示を実施
                        GUI.MaskScreen();
                    }

                    hit_prob = withBlock7.HitProbability(w, ref Commands.SelectedTarget, true);
                    crit_prob = withBlock7.CriticalProbability(w, ref Commands.SelectedTarget);
                    string argtarea = "";
                    caption_msg = "反撃：" + withBlock7.WeaponNickname(w) + " 攻撃力=" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock7.WeaponPower(w, ref argtarea));
                    string argoname2 = "予測命中率非表示";
                    if (!Expression.IsOptionDefined(ref argoname2))
                    {
                        caption_msg = caption_msg + " 命中率=" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(GeneralLib.MinLng(hit_prob, 100)) + "％（" + crit_prob + "％）";
                    }

                    list = new string[4];
                    if (IsAbleToCounterAttack(ref Commands.SelectedTarget, ref Commands.SelectedUnit) & !indirect_attack)
                    {
                        list[1] = "反撃";
                    }
                    else
                    {
                        list[1] = "反撃不能";
                    }

                    string argoname3 = "予測命中率非表示";
                    if (!Expression.IsOptionDefined(ref argoname3))
                    {
                        list[2] = "防御：命中率＝" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(GeneralLib.MinLng(hit_prob, 100)) + "％（" + withBlock7.CriticalProbability(w, ref Commands.SelectedTarget, "防御") + "％）";
                        list[3] = "回避：命中率＝" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(GeneralLib.MinLng(hit_prob / 2, 100)) + "％（" + withBlock7.CriticalProbability(w, ref Commands.SelectedTarget, "回避") + "％）";
                    }
                    else
                    {
                        list[2] = "防御";
                        list[3] = "回避";
                    }

                    // 援護防御が受けられる？
                    Commands.SupportGuardUnit = Commands.SelectedTarget.LookForSupportGuard(ref Commands.SelectedUnit, w);
                    if (Commands.SupportGuardUnit is object)
                    {
                        Array.Resize(ref list, 5);
                        string argoname4 = "等身大基準";
                        if (Expression.IsOptionDefined(ref argoname4))
                        {
                            list[4] = "援護防御：使用する (" + Commands.SupportGuardUnit.Nickname + ")";
                        }
                        else
                        {
                            list[4] = "援護防御：使用する (" + Commands.SupportGuardUnit.Nickname + "/" + Commands.SupportGuardUnit.MainPilot().get_Nickname(false) + ")";
                        }

                        Commands.UseSupportGuard = true;
                    }

                    GUI.AddPartsToListBox();
                    do
                    {
                        // 攻撃への対応手段を選択
                        {
                            var withBlock10 = Commands.SelectedTarget;
                            GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
                            // 各対抗手段が選択可能か判定

                            // 反撃が選択可能？
                            if (list[1] == "反撃")
                            {
                                GUI.ListItemFlag[1] = false;
                                tw = -1;
                            }
                            else
                            {
                                GUI.ListItemFlag[1] = true;
                                tw = 0;
                            }

                            // 防御が選択可能？
                            string argfname12 = "防御不可";
                            if (withBlock10.IsFeatureAvailable(ref argfname12))
                            {
                                GUI.ListItemFlag[2] = true;
                            }
                            else
                            {
                                GUI.ListItemFlag[2] = false;
                            }

                            // 回避が選択可能？
                            string argfname13 = "回避不可";
                            object argIndex14 = "移動不能";
                            if (withBlock10.IsFeatureAvailable(ref argfname13) | withBlock10.IsConditionSatisfied(ref argIndex14))
                            {
                                GUI.ListItemFlag[3] = true;
                            }
                            else
                            {
                                GUI.ListItemFlag[3] = false;
                            }

                            // 対応手段を選択
                            GUI.TopItem = 1;
                            string arglb_info = withBlock10.Nickname0 + " " + withBlock10.MainPilot().get_Nickname(false);
                            string arglb_mode = "連続表示,カーソル移動";
                            i = GUI.ListBox(ref caption_msg, ref list, ref arglb_info, ref arglb_mode);
                        }

                        switch (i)
                        {
                            case 1:
                                {
                                    // 反撃を選択した場合は反撃に使う武器を選択
                                    string argtarea1 = "";
                                    buf = "反撃：" + withBlock7.WeaponNickname(w) + " 攻撃力=" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock7.WeaponPower(w, ref argtarea1));
                                    string argoname5 = "予測命中率非表示";
                                    if (!Expression.IsOptionDefined(ref argoname5))
                                    {
                                        buf = buf + " 命中率=" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(GeneralLib.MinLng(hit_prob, 100)) + "％（" + crit_prob + "％）" + " ： ";
                                    }

                                    {
                                        var withBlock11 = Commands.SelectedTarget.MainPilot();
                                        string argtname = "格闘";
                                        buf = buf + withBlock11.get_Nickname(false) + " " + Expression.Term(ref argtname, ref Commands.SelectedTarget) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock11.Infight) + " ";
                                        if (withBlock11.HasMana())
                                        {
                                            string argtname1 = "魔力";
                                            buf = buf + Expression.Term(ref argtname1, ref Commands.SelectedTarget) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock11.Shooting);
                                        }
                                        else
                                        {
                                            string argtname2 = "射撃";
                                            buf = buf + Expression.Term(ref argtname2, ref Commands.SelectedTarget) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock11.Shooting);
                                        }
                                    }

                                    string arglb_mode1 = "反撃";
                                    string argBGM = "";
                                    tw = GUI.WeaponListBox(ref Commands.SelectedTarget, ref buf, ref arglb_mode1, BGM: ref argBGM);
                                    if (tw == 0)
                                    {
                                        i = 0;
                                    }

                                    break;
                                }

                            case 2:
                                {
                                    // 防御を選択した
                                    def_mode = "防御";
                                    break;
                                }

                            case 3:
                                {
                                    // 回避を選択した
                                    def_mode = "回避";
                                    break;
                                }

                            case 4:
                                {
                                    // 援護防御を使用するかどうかを切り替えた
                                    Commands.UseSupportGuard = !Commands.UseSupportGuard;
                                    if (Commands.UseSupportGuard)
                                    {
                                        list[4] = "援護防御：使用する (";
                                    }
                                    else
                                    {
                                        list[4] = "援護防御：使用しない (";
                                    }

                                    string argoname6 = "等身大基準";
                                    if (Expression.IsOptionDefined(ref argoname6))
                                    {
                                        list[4] = list[4] + Commands.SupportGuardUnit.Nickname + ")";
                                    }
                                    else
                                    {
                                        list[4] = list[4] + Commands.SupportGuardUnit.Nickname + "/" + Commands.SupportGuardUnit.MainPilot().get_Nickname(false) + ")";
                                    }

                                    i = 0;
                                    break;
                                }

                            default:
                                {
                                    // 反撃・防御・回避の全てが選択出来ない？
                                    if (GUI.ListItemFlag[1] & GUI.ListItemFlag[2] & GUI.ListItemFlag[3])
                                    {
                                        break;
                                    }

                                    break;
                                }
                        }
                    }
                    while (i == 0);

                    // 反撃手段選択終了
                    My.MyProject.Forms.frmListBox.Hide();
                    GUI.RemovePartsOnListBox();

                    // ハイライト表示を消去
                    if (SRC.BattleAnimation)
                    {
                        GUI.RefreshScreen();
                    }
                }
                else
                {
                    // コンピューターが操作するユニット及び自動反撃モードの場合

                    // 反撃に使う武器を選択
                    string argamode7 = "反撃";
                    int argmax_prob3 = 0;
                    int argmax_dmg3 = 0;
                    tw = SelectWeapon(ref Commands.SelectedTarget, ref Commands.SelectedUnit, ref argamode7, max_prob: ref argmax_prob3, max_dmg: ref argmax_dmg3);
                    if (indirect_attack)
                    {
                        tw = 0;
                    }

                    // 防御を選択する？
                    // UPGRADE_WARNING: オブジェクト SelectDefense() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    def_mode = Conversions.ToString(SelectDefense(ref Commands.SelectedUnit, ref w, ref Commands.SelectedTarget, ref tw));
                    if (!string.IsNullOrEmpty(def_mode))
                    {
                        tw = -1;
                    }
                }
            }

            // 味方ユニットの場合は武器用ＢＧＭを演奏する
            if (!SRC.KeepEnemyBGM)
            {
                {
                    var withBlock12 = Commands.SelectedTarget;
                    string argfname14 = "武器ＢＧＭ";
                    if (withBlock12.Party == "味方" & tw > 0 & withBlock12.IsFeatureAvailable(ref argfname14))
                    {
                        var loopTo11 = withBlock12.CountFeature();
                        for (i = 1; i <= loopTo11; i++)
                        {
                            string localFeature() { object argIndex1 = i; var ret = withBlock12.Feature(ref argIndex1); return ret; }

                            string localFeatureData2() { object argIndex1 = i; var ret = withBlock12.FeatureData(ref argIndex1); return ret; }

                            string localLIndex4() { string arglist = hs8a3a57a6660e4d73983a0e2cc2500912(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

                            if (localFeature() == "武器ＢＧＭ" & (localLIndex4() ?? "") == (withBlock12.Weapon(tw).Name ?? ""))
                            {
                                // 武器用ＢＧＭが指定されていた
                                string localFeatureData() { object argIndex1 = i; var ret = withBlock12.FeatureData(ref argIndex1); return ret; }

                                string localFeatureData1() { object argIndex1 = i; var ret = withBlock12.FeatureData(ref argIndex1); return ret; }

                                string argmidi_name5 = Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1);
                                BGM = Sound.SearchMidiFile(ref argmidi_name5);
                                if (Strings.Len(BGM) > 0)
                                {
                                    // 武器用ＢＧＭのMIDIが見つかったのでＢＧＭを変更
                                    Sound.BossBGM = false;
                                    Sound.ChangeBGM(ref BGM);
                                }

                                break;
                            }
                        }
                    }
                }
            }

            Commands.SelectedWeapon = w;
            Commands.SelectedTWeapon = tw;
            Commands.SelectedDefenseOption = def_mode;
            wname = Commands.SelectedUnit.Weapon(w).Name;
            Commands.SelectedWeaponName = wname;
            if (tw > 0)
            {
                twname = Commands.SelectedTarget.Weapon(tw).Name;
                Commands.SelectedTWeaponName = twname;
            }
            else
            {
                Commands.SelectedTWeaponName = "";
            }

            // ADD START マージ
            // 戦闘前に一旦クリア
            // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Commands.SupportAttackUnit = null;
            // UPGRADE_NOTE: オブジェクト SupportGuardUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Commands.SupportGuardUnit = null;
            // UPGRADE_NOTE: オブジェクト SupportGuardUnit2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Commands.SupportGuardUnit2 = null;
            // ADD END マージ

            // 武器の使用イベント
            Event_Renamed.HandleEvent("使用", Commands.SelectedUnit.MainPilot().ID, wname);
            if (SRC.IsScenarioFinished | SRC.IsCanceled)
            {
                return;
            }

            if (tw > 0)
            {
                twname = Commands.SelectedTarget.Weapon(tw).Name;
                Commands.SaveSelections();
                Commands.SwapSelections();
                Event_Renamed.HandleEvent("使用", Commands.SelectedUnit.MainPilot().ID, twname);
                Commands.RestoreSelections();
                if (SRC.IsScenarioFinished | SRC.IsCanceled)
                {
                    return;
                }
            }

            // 攻撃イベント
            Event_Renamed.HandleEvent("攻撃", Commands.SelectedUnit.MainPilot().ID, Commands.SelectedTarget.MainPilot().ID);
            if (SRC.IsScenarioFinished | SRC.IsCanceled)
            {
                return;
            }

            // メッセージウィンドウを開く
            if (SRC.Stage == "ＮＰＣ")
            {
                GUI.OpenMessageForm(ref Commands.SelectedTarget, ref Commands.SelectedUnit);
            }
            else
            {
                GUI.OpenMessageForm(ref Commands.SelectedUnit, ref Commands.SelectedTarget);
            }

            // イベント用に戦闘に参加するユニットの情報を記録しておく
            Commands.AttackUnit = Commands.SelectedUnit;
            attack_target = Commands.SelectedUnit;
            attack_target_hp_ratio = Commands.SelectedUnit.HP / (double)Commands.SelectedUnit.MaxHP;
            defense_target = Commands.SelectedTarget;
            defense_target_hp_ratio = Commands.SelectedTarget.HP / (double)Commands.SelectedTarget.MaxHP;
            // UPGRADE_NOTE: オブジェクト defense_target2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            defense_target2 = null;
            // DEL START マージ
            // Set SupportAttackUnit = Nothing
            // Set SupportGuardUnit = Nothing
            // DEL END マージ

            // 相手の先制攻撃？
            {
                var withBlock13 = Commands.SelectedTarget;
                // MOD START マージ
                // If tw > 0 And .MaxAction > 0 Then
                // tw > 0の判定はIsWeaponAvailable内に
                string argref_mode2 = "移動前";
                if (withBlock13.MaxAction() > 0 & withBlock13.IsWeaponAvailable(tw, ref argref_mode2))
                {
                    // MOD END マージ
                    string argattr11 = "後";
                    if (!withBlock13.IsWeaponClassifiedAs(tw, ref argattr11))
                    {
                        string argattr9 = "後";
                        string argattr10 = "先";
                        object argIndex15 = "先読み";
                        string argref_mode1 = "";
                        string argsptype1 = "カウンター";
                        if (Commands.SelectedUnit.IsWeaponClassifiedAs(w, ref argattr9))
                        {
                            def_mode = "先制攻撃";
                            withBlock13.Attack(tw, Commands.SelectedUnit, "先制攻撃", "");
                            Commands.SelectedTarget = withBlock13.CurrentForm();
                        }
                        else if (withBlock13.IsWeaponClassifiedAs(tw, ref argattr10) | withBlock13.MainPilot().SkillLevel(ref argIndex15, ref_mode: ref argref_mode1) >= GeneralLib.Dice(16) | withBlock13.IsUnderSpecialPowerEffect(ref argsptype1))
                        {
                            def_mode = "先制攻撃";
                            withBlock13.Attack(tw, Commands.SelectedUnit, "カウンター", "");
                            Commands.SelectedTarget = withBlock13.CurrentForm();
                        }
                        else if (withBlock13.MaxCounterAttack() > withBlock13.UsedCounterAttack)
                        {
                            def_mode = "先制攻撃";
                            withBlock13.UsedCounterAttack = (short)(withBlock13.UsedCounterAttack + 1);
                            withBlock13.Attack(tw, Commands.SelectedUnit, "カウンター", "");
                            Commands.SelectedTarget = withBlock13.CurrentForm();
                        }

                        // 攻撃側のユニットがかばわれた場合は攻撃側のターゲットを再設定
                        if (Commands.SupportGuardUnit is object)
                        {
                            attack_target = Commands.SupportGuardUnit;
                            attack_target_hp_ratio = Commands.SupportGuardUnitHPRatio;
                        }
                    }
                }
            }

            // サポートアタックのパートナーを探す
            {
                var withBlock14 = Commands.SelectedUnit;
                if (withBlock14.Status_Renamed == "出撃" & Commands.SelectedTarget.Status_Renamed == "出撃")
                {
                    Commands.SupportAttackUnit = withBlock14.LookForSupportAttack(ref Commands.SelectedTarget);

                    // 合体技ではサポートアタック不能
                    if (0 < Commands.SelectedWeapon & Commands.SelectedWeapon <= withBlock14.CountWeapon())
                    {
                        string argattr12 = "合";
                        if (withBlock14.IsWeaponClassifiedAs(Commands.SelectedWeapon, ref argattr12))
                        {
                            // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                            Commands.SupportAttackUnit = null;
                        }
                    }

                    // 魅了された場合
                    object argIndex16 = "魅了";
                    if (withBlock14.IsConditionSatisfied(ref argIndex16) & ReferenceEquals(withBlock14.Master, Commands.SelectedTarget))
                    {
                        // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        Commands.SupportAttackUnit = null;
                    }

                    // 憑依された場合
                    object argIndex17 = "憑依";
                    if (withBlock14.IsConditionSatisfied(ref argIndex17))
                    {
                        if ((withBlock14.Master.Party ?? "") == (Commands.SelectedTarget.Party ?? ""))
                        {
                            // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                            Commands.SupportAttackUnit = null;
                        }
                    }

                    // 踊らされた場合
                    object argIndex18 = "踊り";
                    if (withBlock14.IsConditionSatisfied(ref argIndex18))
                    {
                        // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        Commands.SupportAttackUnit = null;
                    }
                }
            }

            // 攻撃の実施
            {
                var withBlock15 = Commands.SelectedUnit;
                // MOD START マージ
                // If .Status = "出撃" _
                // '            And .MaxAction(True) > 0 _
                // '            And SelectedTarget.Status = "出撃" _
                // '        Then
                object argIndex22 = "攻撃不能";
                if (withBlock15.Status_Renamed == "出撃" & withBlock15.MaxAction(true) > 0 & !withBlock15.IsConditionSatisfied(ref argIndex22) & Commands.SelectedTarget.Status_Renamed == "出撃")
                {
                    // MOD END マージ
                    // まだ武器は使用可能か？
                    if (w > withBlock15.CountWeapon())
                    {
                        w = -1;
                    }
                    else if ((wname ?? "") != (withBlock15.Weapon(w).Name ?? ""))
                    {
                        w = -1;
                    }
                    else if (moved)
                    {
                        string argref_mode4 = "移動後";
                        if (!withBlock15.IsWeaponAvailable(w, ref argref_mode4))
                        {
                            w = -1;
                        }
                    }
                    else
                    {
                        string argref_mode3 = "移動前";
                        if (!withBlock15.IsWeaponAvailable(w, ref argref_mode3))
                        {
                            w = -1;
                        }
                    }

                    if (w > 0)
                    {
                        if (!withBlock15.IsTargetWithinRange(w, ref Commands.SelectedTarget))
                        {
                            w = 0;
                        }
                    }

                    // 行動不能な場合
                    if (withBlock15.MaxAction(true) == 0)
                    {
                        w = -1;
                    }

                    // 魅了された場合
                    object argIndex19 = "魅了";
                    if (withBlock15.IsConditionSatisfied(ref argIndex19) & ReferenceEquals(withBlock15.Master, Commands.SelectedTarget))
                    {
                        w = -1;
                    }

                    // 憑依された場合
                    object argIndex20 = "憑依";
                    if (withBlock15.IsConditionSatisfied(ref argIndex20))
                    {
                        if ((withBlock15.Master.Party ?? "") == (Commands.SelectedTarget.Party ?? ""))
                        {
                            w = -1;
                        }
                    }

                    // 踊らされた場合
                    object argIndex21 = "踊り";
                    if (withBlock15.IsConditionSatisfied(ref argIndex21))
                    {
                        w = -1;
                    }

                    if (w > 0)
                    {
                        // 自爆攻撃？
                        string argattr13 = "自";
                        if (withBlock15.IsWeaponClassifiedAs(w, ref argattr13))
                        {
                            is_suiside = true;
                        }

                        if (Commands.SupportAttackUnit is object & withBlock15.MaxSyncAttack() > withBlock15.UsedSyncAttack)
                        {
                            // 同時援護攻撃
                            withBlock15.Attack(w, Commands.SelectedTarget, "統率", def_mode);
                        }
                        else
                        {
                            // 通常攻撃
                            withBlock15.Attack(w, Commands.SelectedTarget, "", def_mode);
                        }
                    }
                    else if (w == 0)
                    {
                        // 射程外
                        string argmain_situation2 = "射程外";
                        string argsub_situation2 = "";
                        if (withBlock15.IsAnimationDefined(ref argmain_situation2, sub_situation: ref argsub_situation2))
                        {
                            string argmain_situation = "射程外";
                            string argsub_situation = "";
                            withBlock15.PlayAnimation(ref argmain_situation, sub_situation: ref argsub_situation);
                        }
                        else
                        {
                            string argmain_situation1 = "射程外";
                            string argsub_situation1 = "";
                            withBlock15.SpecialEffect(ref argmain_situation1, sub_situation: ref argsub_situation1);
                        }

                        string argSituation = "射程外";
                        string argmsg_mode = "";
                        withBlock15.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                    }
                }
                else
                {
                    w = -1;
                }

                Commands.SelectedUnit = withBlock15.CurrentForm();

                // 防御側のユニットがかばわれた場合は2番目の防御側ユニットとして記録
                if (Commands.SupportGuardUnit is object)
                {
                    defense_target2 = Commands.SupportGuardUnit;
                    defense_target2_hp_ratio = Commands.SupportGuardUnitHPRatio;
                }
            }

            // 同時攻撃
            if (Commands.SupportAttackUnit is object)
            {
                if (Commands.SupportAttackUnit.Status_Renamed != "出撃" | Commands.SelectedUnit.Status_Renamed != "出撃" | Commands.SelectedTarget.Status_Renamed != "出撃")
                {
                    // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                    Commands.SupportAttackUnit = null;
                }
            }

            if (Commands.SupportAttackUnit is object)
            {
                if (Commands.SelectedUnit.MaxSyncAttack() > Commands.SelectedUnit.UsedSyncAttack)
                {
                    {
                        var withBlock16 = Commands.SupportAttackUnit;
                        // サポートアタックに使う武器を決定
                        string argamode8 = "サポートアタック";
                        int argmax_prob4 = 0;
                        int argmax_dmg4 = 0;
                        w2 = SelectWeapon(ref Commands.SupportAttackUnit, ref Commands.SelectedTarget, ref argamode8, max_prob: ref argmax_prob4, max_dmg: ref argmax_dmg4);
                        if (w2 > 0)
                        {
                            // サポートアタックを実施
                            Map.MaskData[withBlock16.x, withBlock16.y] = false;
                            if (!SRC.BattleAnimation)
                            {
                                GUI.MaskScreen();
                            }

                            string argmain_situation4 = "サポートアタック開始";
                            string argsub_situation4 = "";
                            if (withBlock16.IsAnimationDefined(ref argmain_situation4, sub_situation: ref argsub_situation4))
                            {
                                string argmain_situation3 = "サポートアタック開始";
                                string argsub_situation3 = "";
                                withBlock16.PlayAnimation(ref argmain_situation3, sub_situation: ref argsub_situation3);
                            }

                            object argu2 = Commands.SupportAttackUnit;
                            GUI.UpdateMessageForm(ref Commands.SelectedTarget, ref argu2);
                            withBlock16.Attack(w2, Commands.SelectedTarget, "同時援護攻撃", def_mode);
                        }
                    }

                    // 後始末
                    {
                        var withBlock17 = Commands.SupportAttackUnit.CurrentForm();
                        if (w2 > 0)
                        {
                            string argmain_situation6 = "サポートアタック終了";
                            string argsub_situation6 = "";
                            if (withBlock17.IsAnimationDefined(ref argmain_situation6, sub_situation: ref argsub_situation6))
                            {
                                string argmain_situation5 = "サポートアタック終了";
                                string argsub_situation5 = "";
                                withBlock17.PlayAnimation(ref argmain_situation5, sub_situation: ref argsub_situation5);
                            }

                            // サポートアタックの残り回数を減らす
                            withBlock17.UsedSupportAttack = (short)(withBlock17.UsedSupportAttack + 1);

                            // 同時援護攻撃の残り回数を減らす
                            Commands.SelectedUnit.UsedSyncAttack = (short)(Commands.SelectedUnit.UsedSyncAttack + 1);
                        }
                    }

                    support_attack_done = true;

                    // 防御側のユニットがかばわれた場合は本来の防御ユニットデータと
                    // 入れ替えて記録
                    if (Commands.SupportGuardUnit is object)
                    {
                        defense_target = Commands.SupportGuardUnit;
                        defense_target_hp_ratio = Commands.SupportGuardUnitHPRatio;
                    }
                }
            }

            {
                var withBlock18 = Commands.SelectedTarget;
                // 反撃の実行
                if (def_mode != "先制攻撃")
                {
                    if (withBlock18.Status_Renamed == "出撃" & Commands.SelectedUnit.Status_Renamed == "出撃")
                    {
                        // まだ武器は使用可能か？
                        if (tw > 0)
                        {
                            string argref_mode5 = "移動前";
                            if (tw > withBlock18.CountWeapon())
                            {
                                tw = -1;
                            }
                            else if ((twname ?? "") != (withBlock18.Weapon(tw).Name ?? "") | !withBlock18.IsWeaponAvailable(tw, ref argref_mode5))
                            {
                                tw = -1;
                            }
                        }

                        if (tw > 0)
                        {
                            if (!withBlock18.IsTargetWithinRange(tw, ref Commands.SelectedUnit))
                            {
                                // 敵が射程外に逃げていたら武器を再選択
                                tw = 0;
                            }
                        }

                        // 行動不能な場合
                        if (withBlock18.MaxAction() == 0)
                        {
                            tw = -1;
                        }

                        // 魅了された場合
                        object argIndex23 = "魅了";
                        if (withBlock18.IsConditionSatisfied(ref argIndex23) & ReferenceEquals(withBlock18.Master, Commands.SelectedUnit))
                        {
                            tw = -1;
                        }

                        // 憑依された場合
                        object argIndex24 = "憑依";
                        if (withBlock18.IsConditionSatisfied(ref argIndex24))
                        {
                            if ((withBlock18.Master.Party ?? "") == (Commands.SelectedUnit.Party ?? ""))
                            {
                                tw = -1;
                            }
                        }

                        // 踊らされた場合
                        object argIndex25 = "踊り";
                        if (withBlock18.IsConditionSatisfied(ref argIndex25))
                        {
                            tw = -1;
                        }

                        if (tw > 0 & string.IsNullOrEmpty(def_mode))
                        {
                            // 反撃を実施
                            withBlock18.Attack(tw, Commands.SelectedUnit, "", "");
                            if (withBlock18.Status_Renamed == "他形態")
                            {
                                Commands.SelectedTarget = withBlock18.CurrentForm();
                            }

                            if (Commands.SelectedUnit.Status_Renamed == "他形態")
                            {
                                Commands.SelectedUnit = Commands.SelectedUnit.CurrentForm();
                            }

                            // 攻撃側のユニットがかばわれた場合は攻撃側のターゲットを再設定
                            // MOD START マージ
                            // If Not SupportGuardUnit Is Nothing Then
                            // Set attack_target = SupportGuardUnit
                            // attack_target_hp_ratio = SupportGuardUnitHPRatio
                            // End If
                            if (Commands.SupportGuardUnit2 is object)
                            {
                                attack_target = Commands.SupportGuardUnit2;
                                attack_target_hp_ratio = Commands.SupportGuardUnitHPRatio2;
                            }
                        }
                        // MOD END マージ
                        else if (tw == 0 & withBlock18.x == tx & withBlock18.y == ty)
                        {
                            // 反撃出来る武器がなかった場合は射程外メッセージを表示
                            string argmain_situation9 = "射程外";
                            string argsub_situation9 = "";
                            if (withBlock18.IsAnimationDefined(ref argmain_situation9, sub_situation: ref argsub_situation9))
                            {
                                string argmain_situation7 = "射程外";
                                string argsub_situation7 = "";
                                withBlock18.PlayAnimation(ref argmain_situation7, sub_situation: ref argsub_situation7);
                            }
                            else
                            {
                                string argmain_situation8 = "射程外";
                                string argsub_situation8 = "";
                                withBlock18.SpecialEffect(ref argmain_situation8, sub_situation: ref argsub_situation8);
                            }

                            string argSituation1 = "射程外";
                            string argmsg_mode1 = "";
                            withBlock18.PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
                        }
                        else
                        {
                            tw = -1;
                        }
                    }
                    else
                    {
                        tw = -1;
                    }
                }
            }

            // サポートアタック
            if (Commands.SupportAttackUnit is object)
            {
                if (Commands.SupportAttackUnit.Status_Renamed != "出撃" | Commands.SelectedUnit.Status_Renamed != "出撃" | Commands.SelectedTarget.Status_Renamed != "出撃" | support_attack_done)
                {
                    // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                    Commands.SupportAttackUnit = null;
                }
            }

            if (Commands.SupportAttackUnit is object)
            {
                {
                    var withBlock19 = Commands.SupportAttackUnit;
                    // サポートアタックに使う武器を決定
                    string argamode9 = "サポートアタック";
                    int argmax_prob5 = 0;
                    int argmax_dmg5 = 0;
                    w2 = SelectWeapon(ref Commands.SupportAttackUnit, ref Commands.SelectedTarget, ref argamode9, max_prob: ref argmax_prob5, max_dmg: ref argmax_dmg5);
                    if (w2 > 0)
                    {
                        // サポートアタックを実施
                        Map.MaskData[withBlock19.x, withBlock19.y] = false;
                        if (!SRC.BattleAnimation)
                        {
                            GUI.MaskScreen();
                        }

                        string argmain_situation11 = "サポートアタック開始";
                        string argsub_situation11 = "";
                        if (withBlock19.IsAnimationDefined(ref argmain_situation11, sub_situation: ref argsub_situation11))
                        {
                            string argmain_situation10 = "サポートアタック開始";
                            string argsub_situation10 = "";
                            withBlock19.PlayAnimation(ref argmain_situation10, sub_situation: ref argsub_situation10);
                        }

                        object argu21 = Commands.SupportAttackUnit;
                        GUI.UpdateMessageForm(ref Commands.SelectedTarget, ref argu21);
                        withBlock19.Attack(w2, Commands.SelectedTarget, "援護攻撃", def_mode);
                    }
                }

                // 後始末
                {
                    var withBlock20 = Commands.SupportAttackUnit.CurrentForm();
                    string argmain_situation13 = "サポートアタック終了";
                    string argsub_situation13 = "";
                    if (withBlock20.IsAnimationDefined(ref argmain_situation13, sub_situation: ref argsub_situation13))
                    {
                        string argmain_situation12 = "サポートアタック終了";
                        string argsub_situation12 = "";
                        withBlock20.PlayAnimation(ref argmain_situation12, sub_situation: ref argsub_situation12);
                    }

                    // サポートアタックの残り回数を減らす
                    if (w2 > 0)
                    {
                        withBlock20.UsedSupportAttack = (short)(withBlock20.UsedSupportAttack + 1);
                    }
                }

                // 防御側のユニットがかばわれた場合は本来の防御ユニットデータと
                // 入れ替えて記録
                if (Commands.SupportGuardUnit is object)
                {
                    defense_target = Commands.SupportGuardUnit;
                    defense_target_hp_ratio = Commands.SupportGuardUnitHPRatio;
                }
            }

            // 標的が味方の場合の経験値と資金獲得処理
            // (標的が味方が呼び出した召喚ユニットの場合も)
            Commands.SelectedUnit = Commands.SelectedUnit.CurrentForm();
            var get_reward = default(bool);
            {
                var withBlock21 = Commands.SelectedTarget;

                // 経験値＆資金が獲得できるか判定
                if (withBlock21.Party == "味方" & withBlock21.Status_Renamed == "出撃")
                {
                    get_reward = true;
                }
                else if (withBlock21.Summoner is object)
                {
                    string argfname15 = "召喚ユニット";
                    object argIndex26 = "混乱";
                    object argIndex27 = "暴走";
                    if (withBlock21.Summoner.Party == "味方" & withBlock21.Party0 == "ＮＰＣ" & withBlock21.Status_Renamed == "出撃" & withBlock21.IsFeatureAvailable(ref argfname15) & !withBlock21.IsConditionSatisfied(ref argIndex26) & !withBlock21.IsConditionSatisfied(ref argIndex27))
                    {
                        get_reward = true;
                    }
                }

                if (get_reward)
                {
                    if (Commands.SelectedUnit.Status_Renamed == "破壊" & !is_suiside)
                    {
                        // 経験値を獲得
                        string argexp_situation = "破壊";
                        string argexp_mode = "";
                        withBlock21.GetExp(ref Commands.SelectedUnit, ref argexp_situation, exp_mode: ref argexp_mode);

                        // 現在の資金を記録
                        prev_money = SRC.Money;

                        // 獲得する資金を算出
                        earnings = Commands.SelectedUnit.Value / 2;

                        // スペシャルパワーによる獲得資金増加
                        string argsptype2 = "獲得資金増加";
                        if (withBlock21.IsUnderSpecialPowerEffect(ref argsptype2))
                        {
                            string argsname6 = "獲得資金増加";
                            earnings = (int)(earnings * (1d + 0.1d * withBlock21.SpecialPowerEffectLevel(ref argsname6)));
                        }

                        // パイロット能力による獲得資金増加
                        string argsname7 = "資金獲得";
                        if (withBlock21.IsSkillAvailable(ref argsname7))
                        {
                            string argsptype3 = "獲得資金増加";
                            string argoname7 = "収得効果重複";
                            if (!withBlock21.IsUnderSpecialPowerEffect(ref argsptype3) | Expression.IsOptionDefined(ref argoname7))
                            {
                                earnings = (int)GeneralLib.MinDbl(earnings * ((10d + withBlock21.SkillLevel("資金獲得", 5d)) / 10d), 999999999d);
                            }
                        }

                        // 資金を獲得
                        SRC.IncrMoney(earnings);
                        if (SRC.Money > prev_money)
                        {
                            string argtname3 = "資金";
                            GUI.DisplaySysMessage(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SRC.Money - prev_money) + "の" + Expression.Term(ref argtname3, ref Commands.SelectedUnit) + "を得た。");
                        }
                    }
                    else
                    {
                        string argexp_situation1 = "攻撃";
                        string argexp_mode1 = "";
                        withBlock21.GetExp(ref Commands.SelectedUnit, ref argexp_situation1, exp_mode: ref argexp_mode1);
                    }
                }

                // スペシャルパワー「獲得資金増加」「獲得経験値増加」の効果はここで削除する
                string argstype = "戦闘終了";
                withBlock21.RemoveSpecialPowerInEffect(ref argstype);
                if (earnings > 0)
                {
                    string argstype1 = "敵破壊";
                    withBlock21.RemoveSpecialPowerInEffect(ref argstype1);
                }
            }

            // 味方が呼び出した召喚ユニットの場合はＮＰＣでも経験値と資金を獲得
            Commands.SelectedUnit = Commands.SelectedUnit.CurrentForm();
            {
                var withBlock22 = Commands.SelectedUnit;
                if (withBlock22.Summoner is object)
                {
                    string argfname16 = "召喚ユニット";
                    object argIndex28 = "混乱";
                    object argIndex29 = "暴走";
                    if (withBlock22.Summoner.Party == "味方" & withBlock22.Party0 == "ＮＰＣ" & withBlock22.Status_Renamed == "出撃" & withBlock22.IsFeatureAvailable(ref argfname16) & !withBlock22.IsConditionSatisfied(ref argIndex28) & !withBlock22.IsConditionSatisfied(ref argIndex29))
                    {
                        if (Commands.SelectedTarget.Status_Renamed == "破壊")
                        {
                            // ターゲットを破壊した場合

                            // 経験値を獲得
                            string argexp_situation2 = "破壊";
                            string argexp_mode2 = "";
                            withBlock22.GetExp(ref Commands.SelectedTarget, ref argexp_situation2, exp_mode: ref argexp_mode2);

                            // 獲得する資金を算出
                            earnings = Commands.SelectedTarget.Value / 2;

                            // スペシャルパワーによる獲得資金増加
                            string argsptype4 = "獲得資金増加";
                            if (withBlock22.IsUnderSpecialPowerEffect(ref argsptype4))
                            {
                                string argsname8 = "獲得資金増加";
                                earnings = (int)(earnings * (1d + 0.1d * withBlock22.SpecialPowerEffectLevel(ref argsname8)));
                            }

                            // パイロット能力による獲得資金増加
                            string argsname9 = "資金獲得";
                            if (withBlock22.IsSkillAvailable(ref argsname9))
                            {
                                string argsptype5 = "獲得資金増加";
                                string argoname8 = "収得効果重複";
                                if (!withBlock22.IsUnderSpecialPowerEffect(ref argsptype5) | Expression.IsOptionDefined(ref argoname8))
                                {
                                    earnings = (int)((long)(earnings * (10d + withBlock22.SkillLevel("資金獲得", 5d))) / 10L);
                                }
                            }

                            // 資金を獲得
                            SRC.IncrMoney(earnings);
                            if (earnings > 0)
                            {
                                string argtname4 = "資金";
                                GUI.DisplaySysMessage(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(earnings) + "の" + Expression.Term(ref argtname4, ref Commands.SelectedTarget) + "を得た。");
                            }
                        }
                        else
                        {
                            // ターゲットを破壊出来なかった場合

                            // 経験値を獲得
                            string argexp_situation3 = "攻撃";
                            string argexp_mode3 = "";
                            withBlock22.GetExp(ref Commands.SelectedTarget, ref argexp_situation3, exp_mode: ref argexp_mode3);
                        }
                    }
                }

                if (withBlock22.Status_Renamed == "出撃")
                {
                    // スペシャルパワー効果「敵破壊時再行動」
                    string argsptype6 = "敵破壊時再行動";
                    if (withBlock22.IsUnderSpecialPowerEffect(ref argsptype6))
                    {
                        if (Commands.SelectedTarget.Status_Renamed == "破壊")
                        {
                            withBlock22.UsedAction = (short)(withBlock22.UsedAction - 1);
                        }
                    }

                    // 持続期間が「戦闘終了」のスペシャルパワー効果を削除
                    string argstype2 = "戦闘終了";
                    withBlock22.RemoveSpecialPowerInEffect(ref argstype2);
                    if (earnings > 0)
                    {
                        string argstype3 = "敵破壊";
                        withBlock22.RemoveSpecialPowerInEffect(ref argstype3);
                    }
                }
            }

            GUI.CloseMessageForm();
            GUI.RedrawScreen();

            // 状態＆データ更新
            {
                var withBlock23 = attack_target.CurrentForm();
                withBlock23.UpdateCondition();
                withBlock23.Update();
            }

            if (Commands.SupportAttackUnit is object)
            {
                {
                    var withBlock24 = Commands.SupportAttackUnit.CurrentForm();
                    withBlock24.UpdateCondition();
                    withBlock24.Update();
                }
            }

            {
                var withBlock25 = defense_target.CurrentForm();
                withBlock25.UpdateCondition();
                withBlock25.Update();
            }

            if (defense_target2 is object)
            {
                {
                    var withBlock26 = defense_target2.CurrentForm();
                    withBlock26.UpdateCondition();
                    withBlock26.Update();
                }
            }

            if (Commands.SelectedWeapon <= 0)
            {
                Commands.SelectedWeaponName = "";
            }

            if (Commands.SelectedTWeapon <= 0)
            {
                Commands.SelectedTWeaponName = "";
            }

            // 破壊＆損傷率イベント発生

            // 攻撃を受けた攻撃側ユニット
            {
                var withBlock27 = attack_target.CurrentForm();
                if (withBlock27.CountPilot() > 0)
                {
                    if (withBlock27.Status_Renamed == "破壊")
                    {
                        Event_Renamed.HandleEvent("破壊", withBlock27.MainPilot().ID);
                    }
                    else if (withBlock27.Status_Renamed == "出撃" & withBlock27.HP / (double)withBlock27.MaxHP < attack_target_hp_ratio)
                    {
                        Event_Renamed.HandleEvent("損傷率", withBlock27.MainPilot().ID, 100 * (withBlock27.MaxHP - withBlock27.HP) / withBlock27.MaxHP);
                    }

                    if (SRC.IsScenarioFinished | SRC.IsCanceled)
                    {
                        return;
                    }
                }
            }

            // ターゲット側のイベント処理を行うためにユニットの入れ替えを行う
            Commands.SaveSelections();
            Commands.SwapSelections();

            // 攻撃を受けた防御側ユニット
            {
                var withBlock28 = defense_target.CurrentForm();
                if (withBlock28.CountPilot() > 0)
                {
                    if (withBlock28.Status_Renamed == "破壊")
                    {
                        Event_Renamed.HandleEvent("破壊", withBlock28.MainPilot().ID);
                    }
                    else if (withBlock28.Status_Renamed == "出撃" & withBlock28.HP / (double)withBlock28.MaxHP < defense_target_hp_ratio)
                    {
                        Event_Renamed.HandleEvent("損傷率", withBlock28.MainPilot().ID, 100 * (withBlock28.MaxHP - withBlock28.HP) / withBlock28.MaxHP);
                    }
                }
            }

            if (SRC.IsScenarioFinished)
            {
                Commands.RestoreSelections();
                Commands.SelectedPartners = new Unit[1];
                return;
            }

            // 攻撃を受けた防御側ユニットその2
            if (defense_target2 is object)
            {
                if (!ReferenceEquals(defense_target2.CurrentForm(), defense_target.CurrentForm()))
                {
                    {
                        var withBlock29 = defense_target2.CurrentForm();
                        if (withBlock29.CountPilot() > 0)
                        {
                            if (withBlock29.Status_Renamed == "破壊")
                            {
                                Event_Renamed.HandleEvent("破壊", withBlock29.MainPilot().ID);
                            }
                            else if (withBlock29.Status_Renamed == "出撃" & withBlock29.HP / (double)withBlock29.MaxHP < defense_target2_hp_ratio)
                            {
                                Event_Renamed.HandleEvent("損傷率", withBlock29.MainPilot().ID, 100 * (withBlock29.MaxHP - withBlock29.HP) / withBlock29.MaxHP);
                            }
                        }
                    }
                }
            }

            // 元に戻す
            Commands.RestoreSelections();
            if (SRC.IsScenarioFinished | SRC.IsCanceled)
            {
                Commands.SelectedPartners = new Unit[1];
                return;
            }

            // 武器の使用後イベント
            if (Commands.SelectedUnit.Status_Renamed == "出撃" & w > 0)
            {
                Event_Renamed.HandleEvent("使用後", Commands.SelectedUnit.MainPilot().ID, wname);
                if (SRC.IsScenarioFinished | SRC.IsCanceled)
                {
                    Commands.SelectedPartners = new Unit[1];
                    return;
                }
            }

            if (Commands.SelectedTarget.Status_Renamed == "出撃" & tw > 0)
            {
                Commands.SaveSelections();
                Commands.SwapSelections();
                Event_Renamed.HandleEvent("使用後", Commands.SelectedUnit.MainPilot().ID, twname);
                Commands.RestoreSelections();
                if (SRC.IsScenarioFinished | SRC.IsCanceled)
                {
                    Commands.SelectedPartners = new Unit[1];
                    return;
                }
            }

            // 攻撃後イベント
            if (Commands.SelectedUnit.Status_Renamed == "出撃" & Commands.SelectedTarget.Status_Renamed == "出撃")
            {
                Event_Renamed.HandleEvent("攻撃後", Commands.SelectedUnit.MainPilot().ID, Commands.SelectedTarget.MainPilot().ID);
                if (SRC.IsScenarioFinished | SRC.IsCanceled)
                {
                    Commands.SelectedPartners = new Unit[1];
                    return;
                }
            }

            // もし敵が移動していれば進入イベント
            {
                var withBlock30 = Commands.SelectedTarget;
                if (withBlock30.Status_Renamed == "出撃")
                {
                    if (withBlock30.x != tx | withBlock30.y != ty)
                    {
                        Event_Renamed.HandleEvent("進入", withBlock30.MainPilot().ID, withBlock30.x, withBlock30.y);
                        if (SRC.IsScenarioFinished | SRC.IsCanceled)
                        {
                            Commands.SelectedPartners = new Unit[1];
                            return;
                        }
                    }
                }
            }

            // 合体技のパートナーの行動数を減らす
            string argoname9 = "合体技パートナー行動数無消費";
            if (!Expression.IsOptionDefined(ref argoname9))
            {
                var loopTo12 = (short)Information.UBound(partners);
                for (i = 1; i <= loopTo12; i++)
                    partners[i].CurrentForm().UseAction();
            }

            // 再移動
            if (is_p_weapon & Commands.SelectedUnit.Status_Renamed == "出撃")
            {
                string argsname10 = "遊撃";
                if (Commands.SelectedUnit.MainPilot().IsSkillAvailable(ref argsname10) & Commands.SelectedUnit.Speed * 2 > Commands.SelectedUnitMoveCost)
                {
                    // 進入イベント
                    if (Commands.SelectedUnitMoveCost > 0)
                    {
                        Event_Renamed.HandleEvent("進入", Commands.SelectedUnit.MainPilot().ID, Commands.SelectedUnit.x, Commands.SelectedUnit.y);
                        if (SRC.IsScenarioFinished)
                        {
                            return;
                        }
                    }

                    // ユニットが既に出撃していない？
                    if (Commands.SelectedUnit.Status_Renamed != "出撃")
                    {
                        return;
                    }

                    took_action = true;
                    Map.AreaInSpeed(ref Commands.SelectedUnit);

                    // 目標地点が設定されている？
                    short localLLength2() { string arglist = Commands.SelectedUnit.Mode; var ret = GeneralLib.LLength(ref arglist); Commands.SelectedUnit.Mode = arglist; return ret; }

                    if (localLLength2() == 2)
                    {
                        string localLIndex5() { string arglist = Commands.SelectedUnit.Mode; var ret = GeneralLib.LIndex(ref arglist, 1); Commands.SelectedUnit.Mode = arglist; return ret; }

                        string localLIndex6() { string arglist = Commands.SelectedUnit.Mode; var ret = GeneralLib.LIndex(ref arglist, 1); Commands.SelectedUnit.Mode = arglist; return ret; }

                        dst_x = Conversions.ToShort(localLIndex6());
                        string localLIndex7() { string arglist = Commands.SelectedUnit.Mode; var ret = GeneralLib.LIndex(ref arglist, 2); Commands.SelectedUnit.Mode = arglist; return ret; }

                        string localLIndex8() { string arglist = Commands.SelectedUnit.Mode; var ret = GeneralLib.LIndex(ref arglist, 2); Commands.SelectedUnit.Mode = arglist; return ret; }

                        dst_y = Conversions.ToShort(localLIndex8());
                        if (1 <= dst_x & dst_x <= Map.MapWidth & 1 <= dst_y & dst_y <= Map.MapHeight)
                        {
                            goto Move;
                        }
                    }

                    // そうでなければ安全な場所へ
                    Map.SafetyPoint(ref Commands.SelectedUnit, ref dst_x, ref dst_y);
                    goto Move;
                }
            }

            // 行動終了
            goto EndOfOperation;
            SearchNearestEnemy:
            ;


            // もっとも近くにいる敵を探す
            searched_nearest_enemy = true;
            Commands.SelectedTarget = SearchNearestEnemy(ref Commands.SelectedUnit);

            // ターゲットが見つからなければあきらめて終了
            if (Commands.SelectedTarget is null)
            {
                goto EndOfOperation;
            }

            // 見つかったらターゲットの位置を目標地点にして移動
            dst_x = Commands.SelectedTarget.x;
            dst_y = Commands.SelectedTarget.y;
            Move:
            ;


            // 目標地点
            Commands.SelectedX = dst_x;
            Commands.SelectedY = dst_y;

            // 移動形態への変形が可能であれば変形
            if (!transfered)
            {
                if (TryMoveTransform())
                {
                    transfered = true;
                }
            }

            {
                var withBlock31 = Commands.SelectedUnit;
                // 移動可能範囲を設定

                // テレポート能力を使える場合は優先的に使用
                object argIndex30 = "テレポート";
                string arglist = withBlock31.FeatureData(ref argIndex30);
                if (GeneralLib.LLength(ref arglist) == 2)
                {
                    string localLIndex9() { object argIndex1 = "テレポート"; string arglist = withBlock31.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    string localLIndex10() { object argIndex1 = "テレポート"; string arglist = withBlock31.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    tmp = Conversions.ToShort(localLIndex10());
                }
                else
                {
                    tmp = 40;
                }

                string argfname17 = "テレポート";
                if (withBlock31.IsFeatureAvailable(ref argfname17) & (withBlock31.EN > 10 * tmp | withBlock31.EN - tmp > withBlock31.MaxEN / 2) & Commands.SelectedUnitMoveCost == 0)
                {
                    mmode = "テレポート";
                    withBlock31.EN = withBlock31.EN - tmp;
                    Map.AreaInTeleport(ref Commands.SelectedUnit);
                    goto MoveAreaSelected;
                }

                // ジャンプ能力を使う？
                object argIndex31 = "ジャンプ";
                string arglist1 = withBlock31.FeatureData(ref argIndex31);
                if (GeneralLib.LLength(ref arglist1) == 2)
                {
                    string localLIndex11() { object argIndex1 = "ジャンプ"; string arglist = withBlock31.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    string localLIndex12() { object argIndex1 = "ジャンプ"; string arglist = withBlock31.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    tmp = Conversions.ToShort(localLIndex12());
                }
                else
                {
                    tmp = 0;
                }

                string argfname18 = "ジャンプ";
                if (withBlock31.IsFeatureAvailable(ref argfname18) & withBlock31.Area != "空中" & withBlock31.Area != "宇宙" & (withBlock31.EN > 10 * tmp | withBlock31.EN - tmp > withBlock31.MaxEN / 2) & Commands.SelectedUnitMoveCost == 0)
                {
                    mmode = "ジャンプ";
                    withBlock31.EN = withBlock31.EN - tmp;
                    Map.AreaInSpeed(ref Commands.SelectedUnit, true);
                    goto MoveAreaSelected;
                }

                // 通常移動
                mmode = "";
                Map.AreaInSpeed(ref Commands.SelectedUnit);
                MoveAreaSelected:
                ;


                // 護衛すべきユニットがいる場合は動ける範囲を限定
                if (guard_unit_mode)
                {
                    Pilot localItem4() { object argIndex1 = withBlock31.Mode; var ret = SRC.PList.Item(ref argIndex1); withBlock31.Mode = Conversions.ToString(argIndex1); return ret; }

                    {
                        var withBlock32 = localItem4().Unit_Renamed;
                        var loopTo13 = Map.MapWidth;
                        for (i = 1; i <= loopTo13; i++)
                        {
                            var loopTo14 = Map.MapHeight;
                            for (j = 1; j <= loopTo14; j++)
                            {
                                if (!Map.MaskData[i, j])
                                {
                                    if (Math.Abs((short)(withBlock32.x - i)) + Math.Abs((short)(withBlock32.y - j)) > 1)
                                    {
                                        Map.MaskData[i, j] = true;
                                    }
                                }
                            }
                        }
                    }
                }

                object argIndex32 = "混乱";
                if (withBlock31.Mode == "逃亡")
                {
                    // 移動可能範囲内で敵から最も遠い場所を検索
                    Map.SafetyPoint(ref Commands.SelectedUnit, ref dst_x, ref dst_y);
                    new_x = dst_x;
                    new_y = dst_y;
                }
                else if (withBlock31.IsConditionSatisfied(ref argIndex32))
                {
                    // 移動可能範囲内からランダムに行き先を選択
                    dst_x = (short)(withBlock31.x + GeneralLib.Dice(withBlock31.Speed + 1) - GeneralLib.Dice(withBlock31.Speed + 1));
                    dst_y = (short)(withBlock31.y + GeneralLib.Dice(withBlock31.Speed + 1) - GeneralLib.Dice(withBlock31.Speed + 1));
                    Map.NearestPoint(ref Commands.SelectedUnit, dst_x, dst_y, ref new_x, ref new_y);
                }
                else
                {
                    // 移動可能範囲内で移動目的地に最も近い場所を検索
                    Map.NearestPoint(ref Commands.SelectedUnit, dst_x, dst_y, ref new_x0, ref new_y0);

                    // 移動先が危険地域かどうか判定する
                    tmp = (short)(Math.Abs((short)(dst_x - new_x0)) + Math.Abs((short)(dst_y - new_y0)));
                    if (tmp <= 5)
                    {
                        if (Map.MapDataForUnit[dst_x, dst_y] is object)
                        {
                            if (!withBlock31.IsEnemy(ref Map.MapDataForUnit[dst_x, dst_y]))
                            {
                                tmp = 1000;
                            }
                        }
                        else
                        {
                            tmp = 1000;
                        }
                    }

                    new_x = new_x0;
                    new_y = new_y0;
                    new_locations_value = -1;
                    if (tmp <= 5)
                    {
                        // 移動先は危険地域。援護し合えるユニットと隣接するか、
                        // 有効な地形効果が得られる場所を探す。
                        for (i = 0; i <= 12; i++)
                        {
                            switch (i)
                            {
                                case 0:
                                    {
                                        tx = new_x0;
                                        ty = new_y0;
                                        break;
                                    }

                                case 1:
                                    {
                                        tx = (short)(new_x0 + 1);
                                        ty = new_y0;
                                        break;
                                    }

                                case 2:
                                    {
                                        tx = (short)(new_x0 - 1);
                                        ty = new_y0;
                                        break;
                                    }

                                case 3:
                                    {
                                        tx = new_x0;
                                        ty = (short)(new_y0 + 1);
                                        break;
                                    }

                                case 4:
                                    {
                                        tx = new_x0;
                                        ty = (short)(new_y0 - 1);
                                        break;
                                    }

                                case 5:
                                    {
                                        tx = (short)(new_x0 + 1);
                                        ty = (short)(new_y0 + 1);
                                        break;
                                    }

                                case 6:
                                    {
                                        tx = (short)(new_x0 - 1);
                                        ty = (short)(new_y0 + 1);
                                        break;
                                    }

                                case 7:
                                    {
                                        tx = (short)(new_x0 + 1);
                                        ty = (short)(new_y0 - 1);
                                        break;
                                    }

                                case 8:
                                    {
                                        tx = (short)(new_x0 - 1);
                                        ty = (short)(new_y0 - 1);
                                        break;
                                    }

                                case 9:
                                    {
                                        tx = (short)(new_x0 + 2);
                                        ty = new_y0;
                                        break;
                                    }

                                case 10:
                                    {
                                        tx = (short)(new_x0 - 2);
                                        ty = new_y0;
                                        break;
                                    }

                                case 11:
                                    {
                                        tx = new_x0;
                                        ty = (short)(new_y0 + 2);
                                        break;
                                    }

                                case 12:
                                    {
                                        tx = new_x0;
                                        ty = (short)(new_y0 - 2);
                                        break;
                                    }
                            }

                            if (1 <= tx & tx <= Map.MapWidth & 1 <= ty & ty <= Map.MapHeight)
                            {
                                if (!Map.MaskData[tx, ty] & (short)(Math.Abs((short)(dst_x - tx)) + Math.Abs((short)(dst_y - ty))) < (short)(Math.Abs((short)(dst_x - withBlock31.x)) + Math.Abs((short)(dst_y - withBlock31.y))))
                                {
                                    tmp = (short)(Map.TerrainEffectForHPRecover(tx, ty) + Map.TerrainEffectForENRecover(tx, ty) + 100 * withBlock31.LookForSupport(tx, ty));

                                    // 地形による防御効果は空中にいる場合にのみ適用
                                    if (withBlock31.Area != "空中")
                                    {
                                        tmp = (short)((short)(tmp + Map.TerrainEffectForHit(tx, ty)) + Map.TerrainEffectForDamage(tx, ty));
                                        // 水中用ユニットの場合は水中を優先
                                        if (Map.TerrainClass(tx, ty) == "水")
                                        {
                                            string argarea_name1 = "水";
                                            if (withBlock31.IsTransAvailable(ref argarea_name1))
                                            {
                                                tmp = (short)(tmp + 100);
                                            }
                                        }
                                    }

                                    if (tmp > new_locations_value)
                                    {
                                        new_x = tx;
                                        new_y = ty;
                                        new_locations_value = tmp;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // 移動先は危険地域ではない。
                        // 援護し合えるユニットがいれば隣接する。
                        for (i = 0; i <= 4; i++)
                        {
                            switch (i)
                            {
                                case 0:
                                    {
                                        tx = new_x0;
                                        ty = new_y0;
                                        break;
                                    }

                                case 1:
                                    {
                                        tx = (short)(new_x0 + 1);
                                        ty = new_y0;
                                        break;
                                    }

                                case 2:
                                    {
                                        tx = (short)(new_x0 - 1);
                                        ty = new_y0;
                                        break;
                                    }

                                case 3:
                                    {
                                        tx = new_x0;
                                        ty = (short)(new_y0 + 1);
                                        break;
                                    }

                                case 4:
                                    {
                                        tx = new_x0;
                                        ty = (short)(new_y0 - 1);
                                        break;
                                    }
                            }

                            if (1 <= tx & tx <= Map.MapWidth & 1 <= ty & ty <= Map.MapHeight)
                            {
                                if (!Map.MaskData[tx, ty] & (short)(Math.Abs((short)(dst_x - tx)) + Math.Abs((short)(dst_y - ty))) < (short)(Math.Abs((short)(dst_x - withBlock31.x)) + Math.Abs((short)(dst_y - withBlock31.y))))
                                {
                                    tmp = withBlock31.LookForSupport(tx, ty);
                                    if (tmp > new_locations_value)
                                    {
                                        new_x = tx;
                                        new_y = ty;
                                        new_locations_value = tmp;
                                    }
                                }
                            }
                        }
                    }
                }

                if (new_x < 1 | Map.MapWidth < new_x | new_y < 1 | Map.MapHeight < new_y)
                {
                    // 移動できる場所がない……
                    goto EndOfOperation;
                }

                // 見つかった場所がいまいる場所でなければそこへ移動
                if (withBlock31.x != new_x | withBlock31.y != new_y)
                {
                    switch (mmode ?? "")
                    {
                        case "テレポート":
                            {
                                string argmain_situation14 = "テレポート";
                                if (withBlock31.IsMessageDefined(ref argmain_situation14))
                                {
                                    Unit argu1 = null;
                                    Unit argu22 = null;
                                    GUI.OpenMessageForm(u1: ref argu1, u2: ref argu22);
                                    string argSituation2 = "テレポート";
                                    string argmsg_mode2 = "";
                                    withBlock31.PilotMessage(ref argSituation2, msg_mode: ref argmsg_mode2);
                                    GUI.CloseMessageForm();
                                }

                                bool localIsSpecialEffectDefined() { string argmain_situation = "テレポート"; object argIndex1 = "テレポート"; string argsub_situation = withBlock31.FeatureName(ref argIndex1); var ret = withBlock31.IsSpecialEffectDefined(ref argmain_situation, ref argsub_situation); return ret; }

                                string argmain_situation17 = "テレポート";
                                object argIndex36 = "テレポート";
                                string argsub_situation16 = withBlock31.FeatureName(ref argIndex36);
                                if (withBlock31.IsAnimationDefined(ref argmain_situation17, ref argsub_situation16))
                                {
                                    string argmain_situation15 = "テレポート";
                                    object argIndex33 = "テレポート";
                                    string argsub_situation14 = withBlock31.FeatureName(ref argIndex33);
                                    withBlock31.PlayAnimation(ref argmain_situation15, ref argsub_situation14);
                                }
                                else if (localIsSpecialEffectDefined())
                                {
                                    string argmain_situation16 = "テレポート";
                                    object argIndex34 = "テレポート";
                                    string argsub_situation15 = withBlock31.FeatureName(ref argIndex34);
                                    withBlock31.SpecialEffect(ref argmain_situation16, ref argsub_situation15);
                                }
                                else if (SRC.BattleAnimation)
                                {
                                    object argIndex35 = "テレポート";
                                    string arganame = "テレポート発動 Whiz.wav " + withBlock31.FeatureName0(ref argIndex35);
                                    Effect.ShowAnimation(ref arganame);
                                }
                                else
                                {
                                    string argwave_name = "Whiz.wav";
                                    Sound.PlayWave(ref argwave_name);
                                }

                                withBlock31.Move(new_x, new_y, true, false, true);
                                Commands.SelectedUnitMoveCost = 1000;
                                GUI.RedrawScreen();
                                break;
                            }

                        case "ジャンプ":
                            {
                                string argmain_situation18 = "ジャンプ";
                                if (withBlock31.IsMessageDefined(ref argmain_situation18))
                                {
                                    Unit argu11 = null;
                                    Unit argu23 = null;
                                    GUI.OpenMessageForm(u1: ref argu11, u2: ref argu23);
                                    string argSituation3 = "ジャンプ";
                                    string argmsg_mode3 = "";
                                    withBlock31.PilotMessage(ref argSituation3, msg_mode: ref argmsg_mode3);
                                    GUI.CloseMessageForm();
                                }

                                bool localIsSpecialEffectDefined1() { string argmain_situation = "ジャンプ"; object argIndex1 = "ジャンプ"; string argsub_situation = withBlock31.FeatureName(ref argIndex1); var ret = withBlock31.IsSpecialEffectDefined(ref argmain_situation, ref argsub_situation); return ret; }

                                string argmain_situation21 = "ジャンプ";
                                object argIndex39 = "ジャンプ";
                                string argsub_situation19 = withBlock31.FeatureName(ref argIndex39);
                                if (withBlock31.IsAnimationDefined(ref argmain_situation21, ref argsub_situation19))
                                {
                                    string argmain_situation19 = "ジャンプ";
                                    object argIndex37 = "ジャンプ";
                                    string argsub_situation17 = withBlock31.FeatureName(ref argIndex37);
                                    withBlock31.PlayAnimation(ref argmain_situation19, ref argsub_situation17);
                                }
                                else if (localIsSpecialEffectDefined1())
                                {
                                    string argmain_situation20 = "ジャンプ";
                                    object argIndex38 = "ジャンプ";
                                    string argsub_situation18 = withBlock31.FeatureName(ref argIndex38);
                                    withBlock31.SpecialEffect(ref argmain_situation20, ref argsub_situation18);
                                }
                                else
                                {
                                    string argwave_name1 = "Swing.wav";
                                    Sound.PlayWave(ref argwave_name1);
                                }

                                withBlock31.Move(new_x, new_y, true, false, true);
                                Commands.SelectedUnitMoveCost = 1000;
                                GUI.RedrawScreen();
                                break;
                            }

                        default:
                            {
                                // 通常移動
                                withBlock31.Move(new_x, new_y);
                                Commands.SelectedUnitMoveCost = (short)Map.TotalMoveCost[new_x, new_y];
                                break;
                            }
                    }

                    moved = true;

                    // 思考モードが「(X,Y)に移動」で目的地についた場合
                    short localLLength3() { string arglist = withBlock31.Mode; var ret = GeneralLib.LLength(ref arglist); withBlock31.Mode = arglist; return ret; }

                    if (localLLength3() == 2)
                    {
                        if (withBlock31.x == dst_x & withBlock31.y == dst_y)
                        {
                            withBlock31.Mode = "待機";
                        }
                    }
                }

                // ここでＥＮ切れ？
                if (withBlock31.EN == 0)
                {
                    if (withBlock31.MaxAction() == 0)
                    {
                        goto EndOfOperation;
                    }
                }

                // 魅了されている場合
                object argIndex40 = "魅了";
                if (withBlock31.IsConditionSatisfied(ref argIndex40))
                {
                    goto EndOfOperation;
                }

                // 逃げている場合
                if (withBlock31.Mode == "逃亡")
                {
                    goto EndOfOperation;
                }

                // 思考モードが特定のターゲットを狙うように設定されている場合
                bool localIsDefined1() { object argIndex1 = withBlock31.Mode; var ret = SRC.PList.IsDefined(ref argIndex1); withBlock31.Mode = Conversions.ToString(argIndex1); return ret; }

                if (localIsDefined1())
                {
                    Pilot localItem5() { object argIndex1 = withBlock31.Mode; var ret = SRC.PList.Item(ref argIndex1); withBlock31.Mode = Conversions.ToString(argIndex1); return ret; }

                    if (ReferenceEquals(localItem5().Unit_Renamed, Commands.SelectedTarget))
                    {
                        if (withBlock31.IsEnemy(ref Commands.SelectedTarget))
                        {
                            if (moved)
                            {
                                string argamode10 = "移動後";
                                int argmax_prob6 = 0;
                                int argmax_dmg6 = 0;
                                w = SelectWeapon(ref Commands.SelectedUnit, ref Commands.SelectedTarget, ref argamode10, max_prob: ref argmax_prob6, max_dmg: ref argmax_dmg6);
                            }
                            else
                            {
                                string argamode11 = "移動可能";
                                int argmax_prob7 = 0;
                                int argmax_dmg7 = 0;
                                w = SelectWeapon(ref Commands.SelectedUnit, ref Commands.SelectedTarget, ref argamode11, max_prob: ref argmax_prob7, max_dmg: ref argmax_dmg7);
                            }

                            if (w > 0)
                            {
                                // 移動の結果、ターゲットが射程内に入った
                                goto AttackEnemy;
                            }
                        }
                        else
                        {
                            // 護衛するユニットのもとを離れるべからず
                            moved = true;
                        }
                    }
                }

                // 特定の地点に移動中
                short localLLength4() { string arglist = withBlock31.Mode; var ret = GeneralLib.LLength(ref arglist); withBlock31.Mode = arglist; return ret; }

                if (localLLength4() == 2)
                {
                    if (1 <= dst_x & dst_x <= Map.MapWidth & 1 <= dst_y & dst_y <= Map.MapHeight)
                    {
                        if (Map.MapDataForUnit[dst_x, dst_y] is object)
                        {
                            Commands.SelectedTarget = Map.MapDataForUnit[dst_x, dst_y];
                            if (withBlock31.IsEnemy(ref Commands.SelectedTarget))
                            {
                                // 移動先の場所にいる敵を優先して排除
                                if (moved)
                                {
                                    string argamode12 = "移動後";
                                    int argmax_prob8 = 0;
                                    int argmax_dmg8 = 0;
                                    w = SelectWeapon(ref Commands.SelectedUnit, ref Commands.SelectedTarget, ref argamode12, max_prob: ref argmax_prob8, max_dmg: ref argmax_dmg8);
                                }
                                else
                                {
                                    string argamode13 = "移動可能";
                                    int argmax_prob9 = 0;
                                    int argmax_dmg9 = 0;
                                    w = SelectWeapon(ref Commands.SelectedUnit, ref Commands.SelectedTarget, ref argamode13, max_prob: ref argmax_prob9, max_dmg: ref argmax_dmg9);
                                }

                                if (w > 0)
                                {
                                    goto AttackEnemy;
                                }
                            }
                            // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                            Commands.SelectedTarget = null;
                        }
                    }
                }

                // 改めて攻撃のシーケンスに移行
                if (!took_action)
                {
                    goto TryMapAttack;
                }
            }

            EndOfOperation:
            ;


            // 行動終了

            Commands.SelectedPartners = new Unit[1];
            if (moved)
            {
                // 持続期間が「移動」のスペシャルパワー効果を削除
                string argstype4 = "移動";
                Commands.SelectedUnit.RemoveSpecialPowerInEffect(ref argstype4);
            }
        }

        // ハイパーモードが可能であればハイパーモード発動
        private static void TryHyperMode()
        {
            string uname;
            Unit u;
            string fname, fdata;
            double flevel;
            {
                var withBlock = Commands.SelectedUnit;
                // ハイパーモードを持っている？
                string argfname = "ハイパーモード";
                if (!withBlock.IsFeatureAvailable(ref argfname))
                {
                    return;
                }

                object argIndex1 = "ハイパーモード";
                fname = withBlock.FeatureName(ref argIndex1);
                object argIndex2 = "ハイパーモード";
                flevel = withBlock.FeatureLevel(ref argIndex2);
                object argIndex3 = "ハイパーモード";
                fdata = withBlock.FeatureData(ref argIndex3);

                // 発動条件を満たす？
                if (withBlock.MainPilot().Morale < 100 + (short)(10d * flevel) & (Strings.InStr(fdata, "気力発動") > 0 | withBlock.HP > withBlock.MaxHP / 4))
                {
                    return;
                }

                // ハイパーモードが禁止されている？
                object argIndex4 = "形態固定";
                if (withBlock.IsConditionSatisfied(ref argIndex4))
                {
                    return;
                }

                object argIndex5 = "機体固定";
                if (withBlock.IsConditionSatisfied(ref argIndex5))
                {
                    return;
                }

                // 変身中・能力コピー中はハイパーモードを使用できない
                object argIndex6 = "ノーマルモード付加";
                if (withBlock.IsConditionSatisfied(ref argIndex6))
                {
                    return;
                }

                // ハイパーモード先の形態を調べる
                uname = GeneralLib.LIndex(ref fdata, 2);
                object argIndex7 = uname;
                u = withBlock.OtherForm(ref argIndex7);

                // ハイパーモード先の形態は使用可能？
                object argIndex8 = "行動不能";
                if (u.IsConditionSatisfied(ref argIndex8) | !u.IsAbleToEnter(withBlock.x, withBlock.y))
                {
                    return;
                }

                // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
                string argfname1 = "追加パイロット";
                if (u.IsFeatureAvailable(ref argfname1))
                {
                    bool localIsDefined() { object argIndex1 = "追加パイロット"; object argIndex2 = u.FeatureData(ref argIndex1); var ret = SRC.PList.IsDefined(ref argIndex2); return ret; }

                    if (!localIsDefined())
                    {
                        object argIndex9 = "追加パイロット";
                        string argpname = u.FeatureData(ref argIndex9);
                        string argpparty = withBlock.Party0;
                        string arggid = "";
                        SRC.PList.Add(ref argpname, withBlock.MainPilot().Level, ref argpparty, gid: ref arggid);
                        withBlock.Party0 = argpparty;
                    }
                }

                // ハイパーモードメッセージ
                bool localIsMessageDefined() { string argmain_situation = "ハイパーモード(" + uname + ")"; var ret = withBlock.IsMessageDefined(ref argmain_situation); return ret; }

                bool localIsMessageDefined1() { string argmain_situation = "ハイパーモード(" + fname + ")"; var ret = withBlock.IsMessageDefined(ref argmain_situation); return ret; }

                string argmain_situation = "ハイパーモード(" + withBlock.Name + "=>" + uname + ")";
                string argmain_situation1 = "ハイパーモード";
                if (withBlock.IsMessageDefined(ref argmain_situation))
                {
                    Unit argu1 = null;
                    Unit argu2 = null;
                    GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                    string argSituation = "ハイパーモード(" + withBlock.Name + "=>" + uname + ")";
                    string argmsg_mode = "";
                    withBlock.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                    GUI.CloseMessageForm();
                }
                else if (localIsMessageDefined())
                {
                    Unit argu11 = null;
                    Unit argu21 = null;
                    GUI.OpenMessageForm(u1: ref argu11, u2: ref argu21);
                    string argSituation1 = "ハイパーモード(" + uname + ")";
                    string argmsg_mode1 = "";
                    withBlock.PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
                    GUI.CloseMessageForm();
                }
                else if (localIsMessageDefined1())
                {
                    Unit argu12 = null;
                    Unit argu22 = null;
                    GUI.OpenMessageForm(u1: ref argu12, u2: ref argu22);
                    string argSituation2 = "ハイパーモード(" + fname + ")";
                    string argmsg_mode2 = "";
                    withBlock.PilotMessage(ref argSituation2, msg_mode: ref argmsg_mode2);
                    GUI.CloseMessageForm();
                }
                else if (withBlock.IsMessageDefined(ref argmain_situation1))
                {
                    Unit argu13 = null;
                    Unit argu23 = null;
                    GUI.OpenMessageForm(u1: ref argu13, u2: ref argu23);
                    string argSituation3 = "ハイパーモード";
                    string argmsg_mode3 = "";
                    withBlock.PilotMessage(ref argSituation3, msg_mode: ref argmsg_mode3);
                    GUI.CloseMessageForm();
                }

                // アニメ表示
                bool localIsAnimationDefined() { string argmain_situation = "ハイパーモード(" + uname + ")"; string argsub_situation = ""; var ret = withBlock.IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsAnimationDefined1() { string argmain_situation = "ハイパーモード(" + fname + ")"; string argsub_situation = ""; var ret = withBlock.IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsSpecialEffectDefined() { string argmain_situation = "ハイパーモード(" + withBlock.Name + "=>" + uname + ")"; string argsub_situation = ""; var ret = withBlock.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsSpecialEffectDefined1() { string argmain_situation = "ハイパーモード(" + uname + ")"; string argsub_situation = ""; var ret = withBlock.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsSpecialEffectDefined2() { string argmain_situation = "ハイパーモード(" + fname + ")"; string argsub_situation = ""; var ret = withBlock.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                string argmain_situation10 = "ハイパーモード(" + withBlock.Name + "=>" + uname + ")";
                string argsub_situation8 = "";
                string argmain_situation11 = "ハイパーモード";
                string argsub_situation9 = "";
                string argmain_situation12 = "ハイパーモード";
                string argsub_situation10 = "";
                if (withBlock.IsAnimationDefined(ref argmain_situation10, sub_situation: ref argsub_situation8))
                {
                    string argmain_situation2 = "ハイパーモード(" + withBlock.Name + "=>" + uname + ")";
                    string argsub_situation = "";
                    withBlock.PlayAnimation(ref argmain_situation2, sub_situation: ref argsub_situation);
                }
                else if (localIsAnimationDefined())
                {
                    string argmain_situation3 = "ハイパーモード(" + uname + ")";
                    string argsub_situation1 = "";
                    withBlock.PlayAnimation(ref argmain_situation3, sub_situation: ref argsub_situation1);
                }
                else if (localIsAnimationDefined1())
                {
                    string argmain_situation4 = "ハイパーモード(" + fname + ")";
                    string argsub_situation2 = "";
                    withBlock.PlayAnimation(ref argmain_situation4, sub_situation: ref argsub_situation2);
                }
                else if (withBlock.IsAnimationDefined(ref argmain_situation11, sub_situation: ref argsub_situation9))
                {
                    string argmain_situation5 = "ハイパーモード";
                    string argsub_situation3 = "";
                    withBlock.PlayAnimation(ref argmain_situation5, sub_situation: ref argsub_situation3);
                }
                else if (localIsSpecialEffectDefined())
                {
                    string argmain_situation6 = "ハイパーモード(" + withBlock.Name + "=>" + uname + ")";
                    string argsub_situation4 = "";
                    withBlock.SpecialEffect(ref argmain_situation6, sub_situation: ref argsub_situation4);
                }
                else if (localIsSpecialEffectDefined1())
                {
                    string argmain_situation7 = "ハイパーモード(" + uname + ")";
                    string argsub_situation5 = "";
                    withBlock.SpecialEffect(ref argmain_situation7, sub_situation: ref argsub_situation5);
                }
                else if (localIsSpecialEffectDefined2())
                {
                    string argmain_situation8 = "ハイパーモード(" + fname + ")";
                    string argsub_situation6 = "";
                    withBlock.SpecialEffect(ref argmain_situation8, sub_situation: ref argsub_situation6);
                }
                else if (withBlock.IsSpecialEffectDefined(ref argmain_situation12, sub_situation: ref argsub_situation10))
                {
                    string argmain_situation9 = "ハイパーモード";
                    string argsub_situation7 = "";
                    withBlock.SpecialEffect(ref argmain_situation9, sub_situation: ref argsub_situation7);
                }

                // ハイパーモード発動
                withBlock.Transform(ref uname);
            }

            // ハイパーモードイベント
            {
                var withBlock1 = u.CurrentForm();
                Event_Renamed.HandleEvent("ハイパーモード", withBlock1.MainPilot().ID, withBlock1.Name);
            }

            // ハイパーモード＆ノーマルモードの自動発動
            u.CurrentForm().CheckAutoHyperMode();
            u.CurrentForm().CheckAutoNormalMode();
            Commands.SelectedUnit = u.CurrentForm();
            Status.DisplayUnitStatus(ref Commands.SelectedUnit);
        }

        // 戦闘形態への変形が可能であれば変形する
        public static bool TryBattleTransform()
        {
            bool TryBattleTransformRet = default;
            string uname;
            Unit u;
            bool flag;
            short xx, yy;
            short i, j;
            {
                var withBlock = Commands.SelectedUnit;
                // 変形が可能？
                string argfname = "変形";
                object argIndex1 = "形態固定";
                object argIndex2 = "機体固定";
                if (!withBlock.IsFeatureAvailable(ref argfname) | withBlock.IsConditionSatisfied(ref argIndex1) | withBlock.IsConditionSatisfied(ref argIndex2))
                {
                    return TryBattleTransformRet;
                }

                // ５マス以内に敵がいるかチェック
                if (DistanceFromNearestEnemy(ref Commands.SelectedUnit) > 5)
                {
                    // 周りに敵はいない
                    return TryBattleTransformRet;
                }

                // 最も運動性が高い形態に変形
                u = Commands.SelectedUnit;
                xx = withBlock.x;
                yy = withBlock.y;
                object argIndex9 = "変形";
                string arglist1 = withBlock.FeatureData(ref argIndex9);
                var loopTo = GeneralLib.LLength(ref arglist1);
                for (i = 2; i <= loopTo; i++)
                {
                    object argIndex3 = "変形";
                    string arglist = withBlock.FeatureData(ref argIndex3);
                    uname = GeneralLib.LIndex(ref arglist, i);
                    object argIndex7 = uname;
                    {
                        var withBlock1 = withBlock.OtherForm(ref argIndex7);
                        // その形態に変形可能？
                        object argIndex4 = "行動不能";
                        if (withBlock1.IsConditionSatisfied(ref argIndex4) | !withBlock1.IsAbleToEnter(xx, yy))
                        {
                            goto NextForm;
                        }

                        // 通常形態は弱い形態であるという仮定に基づき、その形態が
                        // ノーマルモードで指定されている場合ば無視する
                        string localLIndex() { object argIndex1 = "ノーマルモード"; string arglist = withBlock1.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

                        if ((uname ?? "") == (localLIndex() ?? ""))
                        {
                            goto NextForm;
                        }

                        // 海では水中もしくは空中適応を持つユニットを優先
                        switch (Map.TerrainClass(xx, yy) ?? "")
                        {
                            case "水":
                            case "深海":
                                {
                                    // 水中適応を持つユニットを最優先
                                    if (Strings.InStr(withBlock1.Data.Transportation, "水") > 0)
                                    {
                                        if (Strings.InStr(u.Data.Transportation, "水") == 0)
                                        {
                                            object argIndex5 = uname;
                                            u = withBlock1.OtherForm(ref argIndex5);
                                            goto NextForm;
                                        }
                                    }

                                    if (Strings.InStr(u.Data.Transportation, "水") > 0)
                                    {
                                        if (Strings.InStr(withBlock1.Data.Transportation, "水") == 0)
                                        {
                                            goto NextForm;
                                        }
                                    }

                                    // 次点で空中適応ユニット
                                    if (Strings.InStr(withBlock1.Data.Transportation, "空") > 0)
                                    {
                                        if (Strings.InStr(u.Data.Transportation, "空") == 0)
                                        {
                                            object argIndex6 = uname;
                                            u = withBlock1.OtherForm(ref argIndex6);
                                            goto NextForm;
                                        }
                                    }

                                    if (Strings.InStr(u.Data.Transportation, "空") > 0)
                                    {
                                        if (Strings.InStr(withBlock1.Data.Transportation, "空") == 0)
                                        {
                                            goto NextForm;
                                        }
                                    }

                                    break;
                                }
                        }

                        // 運動性が高いものを優先
                        if (withBlock1.Data.Mobility < u.Data.Mobility)
                        {
                            goto NextForm;
                        }
                        else if (withBlock1.Data.Mobility == u.Data.Mobility)
                        {
                            // 運動性が同じなら攻撃力が高いものを優先
                            if (withBlock1.Data.CountWeapon() == 0)
                            {
                                // この形態は武器を持っていない
                                goto NextForm;
                            }
                            else if (u.Data.CountWeapon() > 0)
                            {
                                WeaponData localWeapon() { object argIndex1 = withBlock1.Data.CountWeapon(); var ret = withBlock1.Data.Weapon(ref argIndex1); return ret; }

                                WeaponData localWeapon1() { object argIndex1 = u.Data.CountWeapon(); var ret = u.Data.Weapon(ref argIndex1); return ret; }

                                WeaponData localWeapon2() { object argIndex1 = withBlock1.Data.CountWeapon(); var ret = withBlock1.Data.Weapon(ref argIndex1); return ret; }

                                WeaponData localWeapon3() { object argIndex1 = u.Data.CountWeapon(); var ret = u.Data.Weapon(ref argIndex1); return ret; }

                                if (localWeapon().Power < localWeapon1().Power)
                                {
                                    goto NextForm;
                                }
                                else if (localWeapon2().Power == localWeapon3().Power)
                                {
                                    // 攻撃力も同じなら装甲が高いものを優先
                                    if (withBlock1.Data.Armor <= u.Data.Armor)
                                    {
                                        goto NextForm;
                                    }
                                }
                            }
                        }
                    }

                    object argIndex8 = uname;
                    u = withBlock.OtherForm(ref argIndex8);
                    NextForm:
                    ;
                }

                // 現在の形態が最も戦闘に適している？
                if (ReferenceEquals(u, Commands.SelectedUnit))
                {
                    return TryBattleTransformRet;
                }

                // 形態uに変形決定
                uname = u.Name;

                // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
                string argfname1 = "追加パイロット";
                if (u.IsFeatureAvailable(ref argfname1))
                {
                    bool localIsDefined1() { object argIndex1 = "追加パイロット"; object argIndex2 = u.FeatureData(ref argIndex1); var ret = SRC.PList.IsDefined(ref argIndex2); return ret; }

                    if (!localIsDefined1())
                    {
                        bool localIsDefined() { object argIndex1 = "追加パイロット"; object argIndex2 = u.FeatureData(ref argIndex1); var ret = SRC.PDList.IsDefined(ref argIndex2); return ret; }

                        if (!localIsDefined())
                        {
                            object argIndex10 = "追加パイロット";
                            string argmsg = uname + "の追加パイロット「" + u.FeatureData(ref argIndex10) + "」のデータが見つかりません";
                            GUI.ErrorMessage(ref argmsg);
                            SRC.TerminateSRC();
                        }

                        object argIndex11 = "追加パイロット";
                        string argpname = u.FeatureData(ref argIndex11);
                        string argpparty = withBlock.Party0;
                        string arggid = "";
                        SRC.PList.Add(ref argpname, withBlock.MainPilot().Level, ref argpparty, gid: ref arggid);
                        withBlock.Party0 = argpparty;
                    }
                }

                // 変形メッセージ
                bool localIsMessageDefined() { string argmain_situation = "変形(" + uname + ")"; var ret = withBlock.IsMessageDefined(ref argmain_situation); return ret; }

                bool localIsMessageDefined1() { object argIndex1 = "変形"; string argmain_situation = "変形(" + withBlock.FeatureName(ref argIndex1) + ")"; var ret = withBlock.IsMessageDefined(ref argmain_situation); return ret; }

                string argmain_situation = "変形(" + withBlock.Name + "=>" + uname + ")";
                if (withBlock.IsMessageDefined(ref argmain_situation))
                {
                    Unit argu1 = null;
                    Unit argu2 = null;
                    GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                    string argSituation = "変形(" + withBlock.Name + "=>" + uname + ")";
                    string argmsg_mode = "";
                    withBlock.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                    GUI.CloseMessageForm();
                }
                else if (localIsMessageDefined())
                {
                    Unit argu11 = null;
                    Unit argu21 = null;
                    GUI.OpenMessageForm(u1: ref argu11, u2: ref argu21);
                    string argSituation1 = "変形(" + uname + ")";
                    string argmsg_mode1 = "";
                    withBlock.PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
                    GUI.CloseMessageForm();
                }
                else if (localIsMessageDefined1())
                {
                    Unit argu12 = null;
                    Unit argu22 = null;
                    GUI.OpenMessageForm(u1: ref argu12, u2: ref argu22);
                    object argIndex12 = "変形";
                    string argSituation2 = "変形(" + withBlock.FeatureName(ref argIndex12) + ")";
                    string argmsg_mode2 = "";
                    withBlock.PilotMessage(ref argSituation2, msg_mode: ref argmsg_mode2);
                    GUI.CloseMessageForm();
                }

                // アニメ表示
                bool localIsAnimationDefined() { string argmain_situation = "変形(" + uname + ")"; string argsub_situation = ""; var ret = withBlock.IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsAnimationDefined1() { object argIndex1 = "変形"; string argmain_situation = "変形(" + withBlock.FeatureName(ref argIndex1) + ")"; string argsub_situation = ""; var ret = withBlock.IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsSpecialEffectDefined() { string argmain_situation = "変形(" + withBlock.Name + "=>" + uname + ")"; string argsub_situation = ""; var ret = withBlock.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsSpecialEffectDefined1() { string argmain_situation = "変形(" + uname + ")"; string argsub_situation = ""; var ret = withBlock.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsSpecialEffectDefined2() { object argIndex1 = "変形"; string argmain_situation = "変形(" + withBlock.FeatureName(ref argIndex1) + ")"; string argsub_situation = ""; var ret = withBlock.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                string argmain_situation7 = "変形(" + withBlock.Name + "=>" + uname + ")";
                string argsub_situation6 = "";
                if (withBlock.IsAnimationDefined(ref argmain_situation7, sub_situation: ref argsub_situation6))
                {
                    string argmain_situation1 = "変形(" + withBlock.Name + "=>" + uname + ")";
                    string argsub_situation = "";
                    withBlock.PlayAnimation(ref argmain_situation1, sub_situation: ref argsub_situation);
                }
                else if (localIsAnimationDefined())
                {
                    string argmain_situation2 = "変形(" + uname + ")";
                    string argsub_situation1 = "";
                    withBlock.PlayAnimation(ref argmain_situation2, sub_situation: ref argsub_situation1);
                }
                else if (localIsAnimationDefined1())
                {
                    object argIndex13 = "変形";
                    string argmain_situation3 = "変形(" + withBlock.FeatureName(ref argIndex13) + ")";
                    string argsub_situation2 = "";
                    withBlock.PlayAnimation(ref argmain_situation3, sub_situation: ref argsub_situation2);
                }
                else if (localIsSpecialEffectDefined())
                {
                    string argmain_situation4 = "変形(" + withBlock.Name + "=>" + uname + ")";
                    string argsub_situation3 = "";
                    withBlock.SpecialEffect(ref argmain_situation4, sub_situation: ref argsub_situation3);
                }
                else if (localIsSpecialEffectDefined1())
                {
                    string argmain_situation5 = "変形(" + uname + ")";
                    string argsub_situation4 = "";
                    withBlock.SpecialEffect(ref argmain_situation5, sub_situation: ref argsub_situation4);
                }
                else if (localIsSpecialEffectDefined2())
                {
                    object argIndex14 = "変形";
                    string argmain_situation6 = "変形(" + withBlock.FeatureName(ref argIndex14) + ")";
                    string argsub_situation5 = "";
                    withBlock.SpecialEffect(ref argmain_situation6, sub_situation: ref argsub_situation5);
                }

                // 変形
                withBlock.Transform(ref uname);
            }

            // 変形イベント
            {
                var withBlock2 = u.CurrentForm();
                Event_Renamed.HandleEvent("変形", withBlock2.MainPilot().ID, withBlock2.Name);
            }

            // ハイパーモード＆ノーマルモードの自動発動
            u.CurrentForm().CheckAutoHyperMode();
            u.CurrentForm().CheckAutoNormalMode();
            Commands.SelectedUnit = u.CurrentForm();
            Status.DisplayUnitStatus(ref Commands.SelectedUnit);
            TryBattleTransformRet = true;
            return TryBattleTransformRet;
        }

        // 移動形態への変形が可能であれば変形する
        private static bool TryMoveTransform()
        {
            bool TryMoveTransformRet = default;
            string uname;
            Unit u;
            short xx, yy;
            short tx, ty = default;
            short speed1, speed2;
            short i;
            {
                var withBlock = Commands.SelectedUnit;
                // 変形が可能？
                string argfname = "変形";
                object argIndex1 = "形態固定";
                object argIndex2 = "機体固定";
                if (!withBlock.IsFeatureAvailable(ref argfname) | withBlock.IsConditionSatisfied(ref argIndex1) | withBlock.IsConditionSatisfied(ref argIndex2))
                {
                    return TryMoveTransformRet;
                }

                xx = withBlock.x;
                yy = withBlock.y;

                // 地形に邪魔されて移動できなくならないか調べるため、目的地の方向にある
                // 隣接するマスの座標を調べる
                if (Math.Abs((short)(Commands.SelectedX - xx)) > Math.Abs((short)(Commands.SelectedY - yy)))
                {
                    if (Commands.SelectedX > xx)
                    {
                        tx = (short)(xx + 1);
                    }
                    else
                    {
                        tx = (short)(xx - 1);
                    }

                    ty = yy;
                }
                else
                {
                    tx = xx;
                    if (Commands.SelectedY > ty)
                    {
                        ty = (short)(yy + 1);
                    }
                    else
                    {
                        ty = (short)(yy - 1);
                    }
                }

                // 最も移動力が高い形態に変形
                u = Commands.SelectedUnit;
                object argIndex11 = "変形";
                string arglist1 = withBlock.FeatureData(ref argIndex11);
                var loopTo = GeneralLib.LLength(ref arglist1);
                for (i = 2; i <= loopTo; i++)
                {
                    object argIndex3 = "変形";
                    string arglist = withBlock.FeatureData(ref argIndex3);
                    uname = GeneralLib.LIndex(ref arglist, i);
                    object argIndex9 = uname;
                    {
                        var withBlock1 = withBlock.OtherForm(ref argIndex9);
                        // その形態に変形可能？
                        object argIndex4 = "行動不能";
                        if (withBlock1.IsConditionSatisfied(ref argIndex4) | !withBlock1.IsAbleToEnter(xx, yy))
                        {
                            goto NextForm;
                        }

                        // 目的地方面に移動可能？
                        if (u.IsAbleToEnter(tx, ty) & !withBlock1.IsAbleToEnter(tx, ty))
                        {
                            goto NextForm;
                        }

                        // 移動力が高い方を優先
                        speed1 = withBlock1.Data.Speed;
                        string argfname1 = "テレポート";
                        if (withBlock1.Data.IsFeatureAvailable(ref argfname1))
                        {
                            object argIndex5 = "テレポート";
                            speed1 = (short)(speed1 + withBlock1.Data.FeatureLevel(ref argIndex5) + 1d);
                        }

                        string argfname2 = "ジャンプ";
                        if (withBlock1.Data.IsFeatureAvailable(ref argfname2))
                        {
                            object argIndex6 = "ジャンプ";
                            speed1 = (short)(speed1 + withBlock1.Data.FeatureLevel(ref argIndex6) + 1d);
                        }
                        // 移動可能な地形タイプも考慮
                        switch (Map.TerrainClass(xx, yy) ?? "")
                        {
                            case "水":
                            case "深海":
                                {
                                    if (Strings.InStr(withBlock1.Data.Transportation, "水") > 0 | Strings.InStr(withBlock1.Data.Transportation, "空") > 0)
                                    {
                                        speed1 = (short)(speed1 + 1);
                                    }

                                    break;
                                }
                            // 宇宙や屋内では差が出ない
                            case "宇宙":
                            case "屋内":
                                {
                                    break;
                                }

                            default:
                                {
                                    if (Strings.InStr(withBlock1.Data.Transportation, "空") > 0)
                                    {
                                        speed1 = (short)(speed1 + 1);
                                    }

                                    break;
                                }
                        }

                        speed2 = u.Data.Speed;
                        string argfname3 = "テレポート";
                        if (u.Data.IsFeatureAvailable(ref argfname3))
                        {
                            object argIndex7 = "テレポート";
                            speed2 = (short)(speed2 + u.Data.FeatureLevel(ref argIndex7) + 1d);
                        }

                        string argfname4 = "ジャンプ";
                        if (u.Data.IsFeatureAvailable(ref argfname4))
                        {
                            object argIndex8 = "ジャンプ";
                            speed2 = (short)(speed2 + u.Data.FeatureLevel(ref argIndex8) + 1d);
                        }
                        // 移動可能な地形タイプも考慮
                        switch (Map.TerrainClass(xx, yy) ?? "")
                        {
                            case "水":
                            case "深海":
                                {
                                    if (Strings.InStr(u.Data.Transportation, "水") > 0 | Strings.InStr(u.Data.Transportation, "空") > 0)
                                    {
                                        speed2 = (short)(speed2 + 1);
                                    }

                                    break;
                                }
                            // 宇宙や屋内では差が出ない
                            case "宇宙":
                            case "屋内":
                                {
                                    break;
                                }

                            default:
                                {
                                    if (Strings.InStr(u.Data.Transportation, "空") > 0)
                                    {
                                        speed2 = (short)(speed2 + 1);
                                    }

                                    break;
                                }
                        }

                        if (speed2 > speed1)
                        {
                            goto NextForm;
                        }
                        else if (speed2 == speed1)
                        {
                            // 移動力が同じなら装甲が高い方を優先
                            if (u.Data.Armor >= withBlock1.Data.Armor)
                            {
                                goto NextForm;
                            }
                        }
                    }

                    object argIndex10 = uname;
                    u = withBlock.OtherForm(ref argIndex10);
                    NextForm:
                    ;
                }

                // 現在の形態が最も移動に適している？
                if (ReferenceEquals(Commands.SelectedUnit, u))
                {
                    return TryMoveTransformRet;
                }

                // 形態uに変形決定
                uname = u.Name;

                // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
                string argfname5 = "追加パイロット";
                if (u.IsFeatureAvailable(ref argfname5))
                {
                    bool localIsDefined1() { object argIndex1 = "追加パイロット"; object argIndex2 = u.FeatureData(ref argIndex1); var ret = SRC.PList.IsDefined(ref argIndex2); return ret; }

                    if (!localIsDefined1())
                    {
                        bool localIsDefined() { object argIndex1 = "追加パイロット"; object argIndex2 = u.FeatureData(ref argIndex1); var ret = SRC.PDList.IsDefined(ref argIndex2); return ret; }

                        if (!localIsDefined())
                        {
                            object argIndex12 = "追加パイロット";
                            string argmsg = uname + "の追加パイロット「" + u.FeatureData(ref argIndex12) + "」のデータが見つかりません";
                            GUI.ErrorMessage(ref argmsg);
                            SRC.TerminateSRC();
                        }

                        object argIndex13 = "追加パイロット";
                        string argpname = u.FeatureData(ref argIndex13);
                        string argpparty = withBlock.Party0;
                        string arggid = "";
                        SRC.PList.Add(ref argpname, withBlock.MainPilot().Level, ref argpparty, gid: ref arggid);
                        withBlock.Party0 = argpparty;
                    }
                }

                // 変形メッセージ
                bool localIsMessageDefined() { string argmain_situation = "変形(" + uname + ")"; var ret = withBlock.IsMessageDefined(ref argmain_situation); return ret; }

                bool localIsMessageDefined1() { object argIndex1 = "変形"; string argmain_situation = "変形(" + withBlock.FeatureName(ref argIndex1) + ")"; var ret = withBlock.IsMessageDefined(ref argmain_situation); return ret; }

                string argmain_situation = "変形(" + withBlock.Name + "=>" + uname + ")";
                if (withBlock.IsMessageDefined(ref argmain_situation))
                {
                    Unit argu1 = null;
                    Unit argu2 = null;
                    GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                    string argSituation = "変形(" + withBlock.Name + "=>" + uname + ")";
                    string argmsg_mode = "";
                    withBlock.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                    GUI.CloseMessageForm();
                }
                else if (localIsMessageDefined())
                {
                    Unit argu11 = null;
                    Unit argu21 = null;
                    GUI.OpenMessageForm(u1: ref argu11, u2: ref argu21);
                    string argSituation1 = "変形(" + uname + ")";
                    string argmsg_mode1 = "";
                    withBlock.PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
                    GUI.CloseMessageForm();
                }
                else if (localIsMessageDefined1())
                {
                    Unit argu12 = null;
                    Unit argu22 = null;
                    GUI.OpenMessageForm(u1: ref argu12, u2: ref argu22);
                    object argIndex14 = "変形";
                    string argSituation2 = "変形(" + withBlock.FeatureName(ref argIndex14) + ")";
                    string argmsg_mode2 = "";
                    withBlock.PilotMessage(ref argSituation2, msg_mode: ref argmsg_mode2);
                    GUI.CloseMessageForm();
                }

                // アニメ表示
                bool localIsAnimationDefined() { string argmain_situation = "変形(" + uname + ")"; string argsub_situation = ""; var ret = withBlock.IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsAnimationDefined1() { object argIndex1 = "変形"; string argmain_situation = "変形(" + withBlock.FeatureName(ref argIndex1) + ")"; string argsub_situation = ""; var ret = withBlock.IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsSpecialEffectDefined() { string argmain_situation = "変形(" + withBlock.Name + "=>" + uname + ")"; string argsub_situation = ""; var ret = withBlock.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsSpecialEffectDefined1() { string argmain_situation = "変形(" + uname + ")"; string argsub_situation = ""; var ret = withBlock.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsSpecialEffectDefined2() { object argIndex1 = "変形"; string argmain_situation = "変形(" + withBlock.FeatureName(ref argIndex1) + ")"; string argsub_situation = ""; var ret = withBlock.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                string argmain_situation7 = "変形(" + withBlock.Name + "=>" + uname + ")";
                string argsub_situation6 = "";
                if (withBlock.IsAnimationDefined(ref argmain_situation7, sub_situation: ref argsub_situation6))
                {
                    string argmain_situation1 = "変形(" + withBlock.Name + "=>" + uname + ")";
                    string argsub_situation = "";
                    withBlock.PlayAnimation(ref argmain_situation1, sub_situation: ref argsub_situation);
                }
                else if (localIsAnimationDefined())
                {
                    string argmain_situation2 = "変形(" + uname + ")";
                    string argsub_situation1 = "";
                    withBlock.PlayAnimation(ref argmain_situation2, sub_situation: ref argsub_situation1);
                }
                else if (localIsAnimationDefined1())
                {
                    object argIndex15 = "変形";
                    string argmain_situation3 = "変形(" + withBlock.FeatureName(ref argIndex15) + ")";
                    string argsub_situation2 = "";
                    withBlock.PlayAnimation(ref argmain_situation3, sub_situation: ref argsub_situation2);
                }
                else if (localIsSpecialEffectDefined())
                {
                    string argmain_situation4 = "変形(" + withBlock.Name + "=>" + uname + ")";
                    string argsub_situation3 = "";
                    withBlock.SpecialEffect(ref argmain_situation4, sub_situation: ref argsub_situation3);
                }
                else if (localIsSpecialEffectDefined1())
                {
                    string argmain_situation5 = "変形(" + uname + ")";
                    string argsub_situation4 = "";
                    withBlock.SpecialEffect(ref argmain_situation5, sub_situation: ref argsub_situation4);
                }
                else if (localIsSpecialEffectDefined2())
                {
                    object argIndex16 = "変形";
                    string argmain_situation6 = "変形(" + withBlock.FeatureName(ref argIndex16) + ")";
                    string argsub_situation5 = "";
                    withBlock.SpecialEffect(ref argmain_situation6, sub_situation: ref argsub_situation5);
                }

                // 変形
                withBlock.Transform(ref uname);
            }

            // 変形イベント
            {
                var withBlock2 = u.CurrentForm();
                Event_Renamed.HandleEvent("変形", withBlock2.MainPilot().ID, withBlock2.Name);
            }

            // ハイパーモード＆ノーマルモードの自動発動
            u.CurrentForm().CheckAutoHyperMode();
            u.CurrentForm().CheckAutoNormalMode();
            Commands.SelectedUnit = u.CurrentForm();
            Status.DisplayUnitStatus(ref Commands.SelectedUnit);
            TryMoveTransformRet = true;
            return TryMoveTransformRet;
        }

        // 実行時間を必要としないアビリティがあれば使っておく
        public static void TryInstantAbility()
        {
            short i, j;
            string aname;
            var partners = default(Unit[]);

            // ５マス以内に敵がいるかチェック
            if (DistanceFromNearestEnemy(ref Commands.SelectedUnit) > 5)
            {
                // 周りに敵はいないのでアビリティは使わない
                return;
            }

            {
                var withBlock = Commands.SelectedUnit;
                // 実行時間を必要としないアビリティを探す
                var loopTo = withBlock.CountAbility();
                for (i = 1; i <= loopTo; i++)
                {
                    // 使用可能＆効果あり？
                    string argref_mode = "移動前";
                    if (!withBlock.IsAbilityUseful(i, ref argref_mode))
                    {
                        goto NextAbility;
                    }

                    // ＥＮ消費が多すぎない？
                    if (withBlock.AbilityENConsumption(i) > 0)
                    {
                        if (withBlock.AbilityENConsumption(i) >= withBlock.EN / 2)
                        {
                            goto NextAbility;
                        }
                    }

                    {
                        var withBlock1 = withBlock.Ability(i);
                        // 自己強化のアビリティのみが対象
                        if (withBlock1.MaxRange != 0)
                        {
                            goto NextAbility;
                        }

                        // 実行時間を必要としない？
                        var loopTo1 = withBlock1.CountEffect();
                        for (j = 1; j <= loopTo1; j++)
                        {
                            object argIndex1 = j;
                            if (withBlock1.EffectType(ref argIndex1) == "再行動")
                            {
                                break;
                            }
                        }

                        if (j > withBlock1.CountEffect())
                        {
                            goto NextAbility;
                        }

                        // 強化用アビリティ？
                        var loopTo2 = withBlock1.CountEffect();
                        for (j = 1; j <= loopTo2; j++)
                        {
                            string localEffectType() { object argIndex1 = j; var ret = withBlock1.EffectType(ref argIndex1); return ret; }

                            string localEffectType1() { object argIndex1 = j; var ret = withBlock1.EffectType(ref argIndex1); return ret; }

                            string localEffectType2() { object argIndex1 = j; var ret = withBlock1.EffectType(ref argIndex1); return ret; }

                            if (localEffectType() == "状態" | localEffectType1() == "付加" | localEffectType2() == "強化")
                            {
                                // 強化用アビリティが見つかった
                                Commands.SelectedAbility = i;
                                goto UseInstantAbility;
                            }
                        }
                    }

                    NextAbility:
                    ;
                }

                // ここに来る時は使用できるアビリティがなかった場合
                return;
                UseInstantAbility:
                ;


                // 合体技パートナーの設定
                string argattr = "合";
                if (withBlock.IsAbilityClassifiedAs(Commands.SelectedAbility, ref argattr))
                {
                    string argctype_Renamed = "アビリティ";
                    withBlock.CombinationPartner(ref argctype_Renamed, Commands.SelectedAbility, ref partners);
                }
                else
                {
                    Commands.SelectedPartners = new Unit[1];
                    partners = new Unit[1];
                }

                aname = withBlock.Ability(Commands.SelectedAbility).Name;
                Commands.SelectedAbilityName = aname;

                // アビリティの使用イベント
                Event_Renamed.HandleEvent("使用", withBlock.MainPilot().ID, aname);
                if (SRC.IsScenarioFinished | SRC.IsCanceled)
                {
                    return;
                }

                // アビリティを使用
                Unit argu2 = null;
                GUI.OpenMessageForm(ref Commands.SelectedUnit, u2: ref argu2);
                withBlock.ExecuteAbility(Commands.SelectedAbility, ref Commands.SelectedUnit);
                GUI.CloseMessageForm();
                Commands.SelectedUnit = withBlock.CurrentForm();
            }

            // アビリティの使用後イベント
            Event_Renamed.HandleEvent("使用後", Commands.SelectedUnit.MainPilot().ID, aname);
            if (SRC.IsScenarioFinished | SRC.IsCanceled)
            {
                Commands.SelectedPartners = new Unit[1];
                return;
            }

            // 自爆アビリティの破壊イベント
            if (Commands.SelectedUnit.Status_Renamed == "破壊")
            {
                Event_Renamed.HandleEvent("破壊", Commands.SelectedUnit.MainPilot().ID);
                if (SRC.IsScenarioFinished | SRC.IsCanceled)
                {
                    Commands.SelectedPartners = new Unit[1];
                    return;
                }
            }

            // 行動数を消費しておく
            Commands.SelectedUnit.UseAction();

            // 合体技のパートナーの行動数を減らす
            string argoname = "合体技パートナー行動数無消費";
            if (!Expression.IsOptionDefined(ref argoname))
            {
                var loopTo3 = (short)Information.UBound(partners);
                for (i = 1; i <= loopTo3; i++)
                    partners[i].CurrentForm().UseAction();
            }

            Commands.SelectedPartners = new Unit[1];
        }

        // 召喚が可能であれば召喚する
        public static bool TrySummonning()
        {
            bool TrySummonningRet = default;
            short i, j;
            string aname;
            var partners = default(Unit[]);
            {
                var withBlock = Commands.SelectedUnit;
                // 召喚アビリティを検索
                var loopTo = withBlock.CountAbility();
                for (i = 1; i <= loopTo; i++)
                {
                    string argref_mode = "移動前";
                    if (withBlock.IsAbilityAvailable(i, ref argref_mode))
                    {
                        var loopTo1 = withBlock.Ability(i).CountEffect();
                        for (j = 1; j <= loopTo1; j++)
                        {
                            object argIndex1 = j;
                            if (withBlock.Ability(i).EffectType(ref argIndex1) == "召喚")
                            {
                                Commands.SelectedAbility = i;
                                goto UseSummonning;
                            }
                        }
                    }
                }

                // 使用可能な召喚アビリティを持っていなかった
                return TrySummonningRet;
                UseSummonning:
                ;
                TrySummonningRet = true;
                aname = withBlock.Ability(Commands.SelectedAbility).Name;
                Commands.SelectedAbilityName = aname;

                // 召喚アビリティの使用イベント
                Event_Renamed.HandleEvent("使用", withBlock.MainPilot().ID, aname);
                if (SRC.IsScenarioFinished | SRC.IsCanceled)
                {
                    return TrySummonningRet;
                }

                // 合体技パートナーの設定
                string argattr = "合";
                if (withBlock.IsAbilityClassifiedAs(Commands.SelectedAbility, ref argattr))
                {
                    string argctype_Renamed = "アビリティ";
                    withBlock.CombinationPartner(ref argctype_Renamed, Commands.SelectedAbility, ref partners);
                }
                else
                {
                    Commands.SelectedPartners = new Unit[1];
                    partners = new Unit[1];
                }

                // 召喚アビリティを使用
                Unit argu2 = null;
                GUI.OpenMessageForm(ref Commands.SelectedUnit, u2: ref argu2);
                withBlock.ExecuteAbility(Commands.SelectedAbility, ref Commands.SelectedUnit);
                GUI.CloseMessageForm();
                Commands.SelectedUnit = withBlock.CurrentForm();
            }

            // 召喚アビリティの使用後イベント
            Event_Renamed.HandleEvent("使用後", Commands.SelectedUnit.MainPilot().ID, aname);
            if (SRC.IsScenarioFinished | SRC.IsCanceled)
            {
                Commands.SelectedPartners = new Unit[1];
                return TrySummonningRet;
            }

            // 自爆アビリティの破壊イベント
            if (Commands.SelectedUnit.Status_Renamed == "破壊")
            {
                Event_Renamed.HandleEvent("破壊", Commands.SelectedUnit.MainPilot().ID);
                if (SRC.IsScenarioFinished | SRC.IsCanceled)
                {
                    Commands.SelectedPartners = new Unit[1];
                    return TrySummonningRet;
                }
            }

            // 合体技のパートナーの行動数を減らす
            string argoname = "合体技パートナー行動数無消費";
            if (!Expression.IsOptionDefined(ref argoname))
            {
                var loopTo2 = (short)Information.UBound(partners);
                for (i = 1; i <= loopTo2; i++)
                    partners[i].CurrentForm().UseAction();
            }

            Commands.SelectedPartners = new Unit[1];
            return TrySummonningRet;
        }

        // マップ型回復アビリティ使用に関する処理
        public static bool TryMapHealing(ref bool moved)
        {
            bool TryMapHealingRet = default;
            short a;
            var apower = default(int);
            short max_range, min_range;
            short xx, tx = default, ty = default, yy;
            short y1, x1, x2, y2;
            short i, j;
            short num;
            int score, max_score = default;
            Pilot p;
            Unit t;
            var partners = default(Unit[]);
            short tmp_num, tmp_score;
            short mlv;
            {
                var withBlock = Commands.SelectedUnit;
                Commands.SelectedAbility = 0;

                // 狂戦士状態の際は回復アビリティを使わない
                object argIndex1 = "狂戦士";
                if (withBlock.IsConditionSatisfied(ref argIndex1))
                {
                    return TryMapHealingRet;
                }

                p = withBlock.MainPilot();
                a = withBlock.CountAbility();
                while (a > 0)
                {
                    // マップアビリティかどうか
                    string argattr = "Ｍ";
                    if (!withBlock.IsAbilityClassifiedAs(a, ref argattr))
                    {
                        goto NextAbility;
                    }

                    // アビリティの使用可否を判定
                    if (moved)
                    {
                        string argref_mode = "移動後";
                        if (!withBlock.IsAbilityAvailable(a, ref argref_mode))
                        {
                            goto NextAbility;
                        }
                    }
                    else
                    {
                        string argref_mode1 = "移動前";
                        if (!withBlock.IsAbilityAvailable(a, ref argref_mode1))
                        {
                            goto NextAbility;
                        }
                    }

                    // 回復アビリティかどうか
                    var loopTo = withBlock.Ability(a).CountEffect();
                    for (i = 1; i <= loopTo; i++)
                    {
                        object argIndex2 = i;
                        if (withBlock.Ability(a).EffectType(ref argIndex2) == "回復")
                        {
                            // 回復量を算出しておく
                            if (withBlock.IsSpellAbility(a))
                            {
                                double localEffectLevel() { object argIndex1 = i; var ret = withBlock.Ability(a).EffectLevel(ref argIndex1); return ret; }

                                apower = (int)(5d * localEffectLevel() * p.Shooting);
                            }
                            else
                            {
                                double localEffectLevel1() { object argIndex1 = i; var ret = withBlock.Ability(a).EffectLevel(ref argIndex1); return ret; }

                                apower = (int)(500d * localEffectLevel1());
                            }

                            break;
                        }
                    }

                    if (i > withBlock.Ability(a).CountEffect())
                    {
                        // 回復アビリティではなかった
                        goto NextAbility;
                    }

                    max_range = withBlock.AbilityMaxRange(a);
                    min_range = withBlock.AbilityMinRange(a);
                    x1 = (short)GeneralLib.MaxLng(withBlock.x - max_range, 1);
                    x2 = (short)GeneralLib.MinLng(withBlock.x + max_range, Map.MapWidth);
                    y1 = (short)GeneralLib.MaxLng(withBlock.y - max_range, 1);
                    y2 = (short)GeneralLib.MinLng(withBlock.y + max_range, Map.MapHeight);

                    // アビリティの効果範囲に応じてアビリティが有効かどうか判断する
                    num = 0;
                    score = 0;
                    string argattr4 = "Ｍ全";
                    string argattr5 = "Ｍ投";
                    if (withBlock.IsAbilityClassifiedAs(a, ref argattr4))
                    {
                        // MOD START マージ
                        // AreaInRange .X, .Y, min_range, max_range, .Party
                        string arguparty = withBlock.Party;
                        Map.AreaInRange(withBlock.x, withBlock.y, max_range, min_range, ref arguparty);
                        withBlock.Party = arguparty;
                        // MOD END マージ

                        // 支援専用アビリティの場合は自分には効果がない
                        string argattr1 = "援";
                        if (withBlock.IsAbilityClassifiedAs(a, ref argattr1))
                        {
                            Map.MaskData[withBlock.x, withBlock.y] = true;
                        }

                        // 効果範囲内にいるターゲットをカウント
                        var loopTo1 = x2;
                        for (i = x1; i <= loopTo1; i++)
                        {
                            var loopTo2 = y2;
                            for (j = y1; j <= loopTo2; j++)
                            {
                                if (Map.MaskData[i, j])
                                {
                                    goto NextUnit1;
                                }

                                t = Map.MapDataForUnit[i, j];
                                if (t is null)
                                {
                                    goto NextUnit1;
                                }

                                // アビリティが適用可能？
                                if (!withBlock.IsAbilityApplicable(a, ref t))
                                {
                                    goto NextUnit1;
                                }

                                {
                                    var withBlock1 = t;
                                    // ゾンビ？
                                    object argIndex3 = "ゾンビ";
                                    if (withBlock1.IsConditionSatisfied(ref argIndex3))
                                    {
                                        goto NextUnit1;
                                    }

                                    if (100 * withBlock1.HP / withBlock1.MaxHP < 90)
                                    {
                                        num = (short)(num + 1);
                                    }

                                    score = score + 100 * GeneralLib.MinLng(withBlock1.MaxHP - withBlock1.HP, apower) / withBlock1.MaxHP;
                                }

                                NextUnit1:
                                ;
                            }
                        }

                        // 不要？
                        tx = withBlock.x;
                        ty = withBlock.y;
                    }
                    else if (withBlock.IsAbilityClassifiedAs(a, ref argattr5))
                    {
                        string argattr2 = "Ｍ投";
                        mlv = (short)withBlock.AbilityLevel(a, ref argattr2);

                        // 投下位置を変えながら試してみる
                        var loopTo3 = x2;
                        for (xx = x1; xx <= loopTo3; xx++)
                        {
                            var loopTo4 = y2;
                            for (yy = y1; yy <= loopTo4; yy++)
                            {
                                if ((short)(Math.Abs((short)(withBlock.x - xx)) + Math.Abs((short)(withBlock.y - yy))) > max_range | (short)(Math.Abs((short)(withBlock.x - xx)) + Math.Abs((short)(withBlock.y - yy))) < min_range)
                                {
                                    goto NextPoint;
                                }

                                // MOD START マージ
                                string arguparty1 = withBlock.Party;
                                Map.AreaInRange(xx, yy, 1, mlv, ref arguparty1);
                                withBlock.Party = arguparty1;
                                string arguparty2 = withBlock.Party;
                                Map.AreaInRange(xx, yy, mlv, 1, ref arguparty2);
                                withBlock.Party = arguparty2;
                                // MOD END マージ

                                // 支援専用アビリティの場合は自分には効果がない
                                string argattr3 = "援";
                                if (withBlock.IsAbilityClassifiedAs(a, ref argattr3))
                                {
                                    Map.MaskData[withBlock.x, withBlock.y] = true;
                                }

                                // 効果範囲内にいるターゲットをカウント
                                tmp_num = 0;
                                tmp_score = 0;
                                var loopTo5 = (short)GeneralLib.MinLng(xx + mlv, Map.MapWidth);
                                for (i = (short)GeneralLib.MaxLng(xx - mlv, 1); i <= loopTo5; i++)
                                {
                                    var loopTo6 = (short)GeneralLib.MinLng(yy + mlv, Map.MapHeight);
                                    for (j = (short)GeneralLib.MaxLng(yy - mlv, 1); j <= loopTo6; j++)
                                    {
                                        if (Map.MaskData[i, j])
                                        {
                                            goto NextUnit2;
                                        }

                                        t = Map.MapDataForUnit[i, j];
                                        if (t is null)
                                        {
                                            goto NextUnit2;
                                        }

                                        // アビリティが適用可能？
                                        if (!withBlock.IsAbilityApplicable(a, ref t))
                                        {
                                            goto NextUnit2;
                                        }
                                        // ゾンビ？
                                        object argIndex4 = "ゾンビ";
                                        if (t.IsConditionSatisfied(ref argIndex4))
                                        {
                                            goto NextUnit2;
                                        }

                                        if (100 * t.HP / t.MaxHP < 90)
                                        {
                                            tmp_num = (short)(tmp_num + 1);
                                        }

                                        tmp_score = (short)(tmp_score + 100 * GeneralLib.MinLng(t.MaxHP - t.HP, apower) / t.MaxHP);
                                        NextUnit2:
                                        ;
                                    }
                                }

                                if (tmp_num > 2 & tmp_score > score)
                                {
                                    num = tmp_num;
                                    score = tmp_score;
                                    tx = xx;
                                    ty = yy;
                                }

                                NextPoint:
                                ;
                            }
                        }
                    }

                    if (num > 1 & score > max_score)
                    {
                        Commands.SelectedAbility = a;
                        max_score = score;
                    }

                    NextAbility:
                    ;
                    a = (short)(a - 1);
                }

                if (Commands.SelectedAbility == 0)
                {
                    // 有効なマップアビリティがなかった
                    return TryMapHealingRet;
                }

                // 合体技パートナーの設定
                string argattr6 = "合";
                if (withBlock.IsAbilityClassifiedAs(Commands.SelectedAbility, ref argattr6))
                {
                    string argctype_Renamed = "アビリティ";
                    withBlock.CombinationPartner(ref argctype_Renamed, Commands.SelectedAbility, ref partners);
                }
                else
                {
                    Commands.SelectedPartners = new Unit[1];
                    partners = new Unit[1];
                }

                Commands.SelectedAbilityName = withBlock.Ability(Commands.SelectedAbility).Name;

                // アビリティを使用
                withBlock.ExecuteMapAbility(Commands.SelectedAbility, tx, ty);
                if (SRC.IsScenarioFinished | SRC.IsCanceled)
                {
                    Commands.SelectedPartners = new Unit[1];
                    return TryMapHealingRet;
                }

                // 合体技のパートナーの行動数を減らす
                string argoname = "合体技パートナー行動数無消費";
                if (!Expression.IsOptionDefined(ref argoname))
                {
                    var loopTo7 = (short)Information.UBound(partners);
                    for (i = 1; i <= loopTo7; i++)
                        partners[i].CurrentForm().UseAction();
                }

                Commands.SelectedPartners = new Unit[1];
            }

            TryMapHealingRet = true;
            return TryMapHealingRet;
        }

        // 可能であれば回復アビリティを使う
        public static bool TryHealing(ref bool moved, [Optional, DefaultParameterValue(null)] ref Unit t)
        {
            bool TryHealingRet = default;
            short i, a, j;
            string aname;
            int apower;
            int max_power;
            short max_range;
            int dmg, max_dmg;
            short new_x, new_y;
            int distance;
            bool is_able_to_move, dont_move, sa_is_able_to_move = default;
            var partners = default(Unit[]);
            {
                var withBlock = Commands.SelectedUnit;
                // 狂戦士状態の際は回復アビリティを使わない
                object argIndex1 = "狂戦士";
                if (withBlock.IsConditionSatisfied(ref argIndex1))
                {
                    return TryHealingRet;
                }

                // 初期化
                // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                Commands.SelectedTarget = null;
                max_dmg = 80;
                Commands.SelectedAbility = 0;
                max_power = 0;

                // 移動可能？
                dont_move = moved | withBlock.Mode == "固定";

                // 移動可能である場合は移動範囲を設定しておく
                if (!dont_move)
                {
                    Map.AreaInSpeed(ref Commands.SelectedUnit);
                }

                var loopTo = withBlock.CountAbility();
                for (a = 1; a <= loopTo; a++)
                {
                    // アビリティが使用可能？
                    if (moved)
                    {
                        string argref_mode = "移動後";
                        if (!withBlock.IsAbilityAvailable(a, ref argref_mode))
                        {
                            goto NextHealingSkill;
                        }
                    }
                    else
                    {
                        string argref_mode1 = "移動前";
                        if (!withBlock.IsAbilityAvailable(a, ref argref_mode1))
                        {
                            goto NextHealingSkill;
                        }
                    }

                    // マップアビリティは別関数で調べる
                    string argattr = "Ｍ";
                    if (withBlock.IsAbilityClassifiedAs(a, ref argattr))
                    {
                        goto NextHealingSkill;
                    }

                    // これは回復アビリティ？
                    var loopTo1 = withBlock.Ability(a).CountEffect();
                    for (i = 1; i <= loopTo1; i++)
                    {
                        object argIndex2 = i;
                        if (withBlock.Ability(a).EffectType(ref argIndex2) == "回復")
                        {
                            break;
                        }
                    }

                    if (i > withBlock.Ability(a).CountEffect())
                    {
                        goto NextHealingSkill;
                    }

                    // 回復量を算出
                    if (withBlock.IsSpellAbility(a))
                    {
                        double localEffectLevel() { object argIndex1 = i; var ret = withBlock.Ability(a).EffectLevel(ref argIndex1); return ret; }

                        double localEffectLevel1() { object argIndex1 = i; var ret = withBlock.Ability(a).EffectLevel(ref argIndex1); return ret; }

                        apower = (int)(5d * localEffectLevel1() * withBlock.MainPilot().Shooting);
                    }
                    else
                    {
                        double localEffectLevel2() { object argIndex1 = i; var ret = withBlock.Ability(a).EffectLevel(ref argIndex1); return ret; }

                        apower = (int)(500d * localEffectLevel2());
                    }

                    // 役立たず？
                    if (apower <= 0)
                    {
                        goto NextHealingSkill;
                    }

                    // 現在の回復アビリティを使って回復させられるターゲットがいるか検索
                    foreach (Unit u in SRC.UList)
                    {
                        if (u.Status_Renamed != "出撃")
                        {
                            goto NextHealingTarget;
                        }

                        // 味方かどうかを判定
                        if (!withBlock.IsAlly(ref u))
                        {
                            goto NextHealingTarget;
                        }

                        // デフォルトのターゲットが指定されている場合はそのユニット以外を
                        // ターゲットにはしない
                        if (t is object)
                        {
                            if (!ReferenceEquals(u, t))
                            {
                                goto NextHealingTarget;
                            }
                        }

                        // 損傷度は？
                        dmg = 100 * u.HP / u.MaxHP;

                        // 重要なユニットを優先
                        if (!ReferenceEquals(u, Commands.SelectedUnit))
                        {
                            if (u.BossRank >= 0)
                            {
                                dmg = 100 - 2 * (100 - dmg);
                            }
                        }

                        // 現在のターゲットより損傷度がひどくないなら無視
                        if (dmg > max_dmg)
                        {
                            goto NextHealingTarget;
                        }

                        // 移動可能か？
                        string argattr1 = "Ｐ";
                        if (withBlock.AbilityMaxRange(a) == 1 | withBlock.IsAbilityClassifiedAs(a, ref argattr1))
                        {
                            is_able_to_move = true;
                        }
                        else
                        {
                            is_able_to_move = false;
                        }

                        string argattr2 = "Ｑ";
                        if (withBlock.IsAbilityClassifiedAs(a, ref argattr2))
                        {
                            is_able_to_move = false;
                        }

                        if (dont_move)
                        {
                            is_able_to_move = false;
                        }

                        switch (withBlock.Area ?? "")
                        {
                            case "空中":
                            case "宇宙":
                                {
                                    if (withBlock.EN - withBlock.AbilityENConsumption(a) < 5)
                                    {
                                        is_able_to_move = false;
                                    }

                                    break;
                                }
                        }

                        // 射程内にいるか？
                        if (is_able_to_move)
                        {
                            if (!withBlock.IsTargetReachableForAbility(a, ref u))
                            {
                                goto NextHealingTarget;
                            }
                        }
                        else if (!withBlock.IsTargetWithinAbilityRange(a, ref u))
                        {
                            goto NextHealingTarget;
                        }

                        // アビリティが適用可能？
                        if (!withBlock.IsAbilityApplicable(a, ref u))
                        {
                            goto NextHealingTarget;
                        }

                        // ゾンビ？
                        object argIndex3 = "ゾンビ";
                        if (u.IsConditionSatisfied(ref argIndex3))
                        {
                            goto NextHealingTarget;
                        }

                        // 新規ターゲット？
                        if (!ReferenceEquals(u, Commands.SelectedTarget))
                        {
                            // ターゲット設定
                            Commands.SelectedTarget = u;
                            max_dmg = dmg;

                            // 新規ターゲットを優先するため、現在選択されているアビリティは破棄
                            Commands.SelectedAbility = 0;
                            max_power = 0;
                        }

                        // 現在選択されている回復アビリティとチェック中のアビリティのどちらが
                        // 優れているかを判定
                        if (max_power < u.MaxHP - u.HP)
                        {
                            // 現在選択している回復アビリティでは全ダメージを回復しきれない場合
                            if (apower < max_power)
                            {
                                // 回復量が多いほうを優先
                                goto NextHealingTarget;
                            }
                            else if (apower == max_power)
                            {
                                // 回復量が同じならコストが低い方を優先
                                if (withBlock.Ability(a).ENConsumption > withBlock.Ability(Commands.SelectedAbility).ENConsumption)
                                {
                                    goto NextHealingTarget;
                                }

                                if (withBlock.Ability(a).Stock < withBlock.Ability(Commands.SelectedAbility).Stock)
                                {
                                    goto NextHealingTarget;
                                }
                            }
                        }
                        else if (Commands.SelectedAbility > 0)
                        {
                            // 現在選択している回復アビリティで全快する場合
                            // 全快することが必要条件
                            if (apower >= u.MaxHP - u.HP)
                            {
                                goto NextHealingTarget;
                            }
                            // コストが低い方を優先
                            if (withBlock.Ability(a).ENConsumption > withBlock.Ability(Commands.SelectedAbility).ENConsumption)
                            {
                                goto NextHealingTarget;
                            }

                            if (withBlock.Ability(a).Stock < withBlock.Ability(Commands.SelectedAbility).Stock)
                            {
                                goto NextHealingTarget;
                            }
                        }

                        Commands.SelectedAbility = a;
                        max_power = apower;
                        sa_is_able_to_move = is_able_to_move;
                        NextHealingTarget:
                        ;
                    }

                    NextHealingSkill:
                    ;
                }

                // 有用なアビリティ＆ターゲットが見つかった？
                if (Commands.SelectedAbility == 0)
                {
                    return TryHealingRet;
                }

                if (Commands.SelectedTarget is null)
                {
                    return TryHealingRet;
                }

                // 回復アビリティを使用することが確定
                TryHealingRet = true;

                // 適切な位置に移動
                if (!ReferenceEquals(Commands.SelectedTarget, Commands.SelectedUnit) & sa_is_able_to_move)
                {
                    new_x = withBlock.x;
                    new_y = withBlock.y;
                    max_range = withBlock.AbilityMaxRange(Commands.SelectedAbility);
                    {
                        var withBlock1 = Commands.SelectedTarget;
                        // 現在位置から回復が可能であれば現在位置を優先
                        if ((short)(Math.Abs((short)(withBlock1.x - new_x)) + Math.Abs((short)(withBlock1.y - new_y))) <= max_range)
                        {
                            distance = (int)(Math.Pow(Math.Abs((short)(withBlock1.x - new_x)), 2d) + Math.Pow(Math.Abs((short)(withBlock1.y - new_y)), 2d));
                        }
                        else
                        {
                            distance = 10000;
                        }

                        // 適切な位置を探す
                        var loopTo2 = (short)GeneralLib.MinLng(withBlock1.x + max_range, Map.MapWidth);
                        for (i = (short)GeneralLib.MaxLng(withBlock1.x - max_range, 1); i <= loopTo2; i++)
                        {
                            var loopTo3 = (short)GeneralLib.MinLng(withBlock1.y + max_range, Map.MapHeight);
                            for (j = (short)GeneralLib.MaxLng(withBlock1.y - max_range, 1); j <= loopTo3; j++)
                            {
                                if (!Map.MaskData[i, j] & Map.MapDataForUnit[i, j] is null & (short)(Math.Abs((short)(withBlock1.x - i)) + Math.Abs((short)(withBlock1.y - j))) <= max_range)
                                {
                                    {
                                        var withBlock2 = Commands.SelectedUnit;
                                        if (Math.Pow(Math.Abs((short)(withBlock2.x - i)), 2d) + Math.Pow(Math.Abs((short)(withBlock2.y - j)), 2d) < distance)
                                        {
                                            new_x = i;
                                            new_y = j;
                                            distance = (int)(Math.Pow(Math.Abs((short)(withBlock2.x - new_x)), 2d) + Math.Pow(Math.Abs((short)(withBlock2.y - new_y)), 2d));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (new_x != withBlock.x | new_y != withBlock.y)
                    {
                        // 適切な場所が見つかったので移動
                        withBlock.Move(new_x, new_y);
                        moved = true;
                    }
                }

                aname = withBlock.Ability(Commands.SelectedAbility).Name;
                Commands.SelectedAbilityName = aname;

                // 合体技パートナーの設定
                string argattr3 = "合";
                if (withBlock.IsAbilityClassifiedAs(Commands.SelectedAbility, ref argattr3))
                {
                    string argctype_Renamed = "アビリティ";
                    withBlock.CombinationPartner(ref argctype_Renamed, Commands.SelectedAbility, ref partners);
                }
                else
                {
                    Commands.SelectedPartners = new Unit[1];
                    partners = new Unit[1];
                }

                // 使用イベント
                Event_Renamed.HandleEvent("使用", withBlock.MainPilot().ID, aname);
                if (SRC.IsScenarioFinished | SRC.IsCanceled)
                {
                    Commands.SelectedPartners = new Unit[1];
                    return TryHealingRet;
                }

                if (ReferenceEquals(Commands.SelectedTarget, Commands.SelectedUnit))
                {
                    Unit argu2 = null;
                    GUI.OpenMessageForm(ref Commands.SelectedUnit, u2: ref argu2);
                }
                else
                {
                    GUI.OpenMessageForm(ref Commands.SelectedTarget, ref Commands.SelectedUnit);
                }

                // 回復アビリティを実行
                withBlock.ExecuteAbility(Commands.SelectedAbility, ref Commands.SelectedTarget);
                Commands.SelectedUnit = withBlock.CurrentForm();
            }

            GUI.CloseMessageForm();

            // 自爆した場合の破壊イベント
            if (Commands.SelectedUnit.Status_Renamed == "破壊")
            {
                if (Commands.SelectedUnit.CountPilot() > 0)
                {
                    Event_Renamed.HandleEvent("破壊", Commands.SelectedUnit.MainPilot().ID);
                }

                Commands.SelectedPartners = new Unit[1];
                return TryHealingRet;
            }

            // 使用後イベント
            if (Commands.SelectedUnit.CountPilot() > 0)
            {
                Event_Renamed.HandleEvent("使用後", Commands.SelectedUnit.MainPilot().ID, aname);
                if (SRC.IsScenarioFinished | SRC.IsCanceled)
                {
                    Commands.SelectedPartners = new Unit[1];
                    return TryHealingRet;
                }
            }

            // 自爆アビリティの破壊イベント
            if (Commands.SelectedUnit.Status_Renamed == "破壊")
            {
                Event_Renamed.HandleEvent("破壊", Commands.SelectedUnit.MainPilot().ID);
                if (SRC.IsScenarioFinished | SRC.IsCanceled)
                {
                    Commands.SelectedPartners = new Unit[1];
                    return TryHealingRet;
                }
            }

            // 合体技のパートナーの行動数を減らす
            string argoname = "合体技パートナー行動数無消費";
            if (!Expression.IsOptionDefined(ref argoname))
            {
                var loopTo4 = (short)Information.UBound(partners);
                for (i = 1; i <= loopTo4; i++)
                    partners[i].CurrentForm().UseAction();
            }

            Commands.SelectedPartners = new Unit[1];
            return TryHealingRet;
        }

        // 修理が可能であれば修理装置を使う
        public static bool TryFix(ref bool moved, [Optional, DefaultParameterValue(null)] ref Unit t)
        {
            bool TryFixRet = default;
            var TmpMaskData = default(bool[]);
            short j, i, k;
            short new_x, new_y;
            int max_dmg;
            int tmp;
            Unit u;
            string fname;
            {
                var withBlock = Commands.SelectedUnit;
                // 修理装置を使用可能？
                string argfname = "修理装置";
                if (!withBlock.IsFeatureAvailable(ref argfname) | withBlock.Area == "地中")
                {
                    return TryFixRet;
                }

                // 狂戦士状態の際は修理装置を使わない
                object argIndex1 = "狂戦士";
                if (withBlock.IsConditionSatisfied(ref argIndex1))
                {
                    return TryFixRet;
                }

                // 修理装置を使用可能な領域を設定
                if (moved | withBlock.Mode == "固定")
                {
                    // 移動でない場合
                    var loopTo = Map.MapWidth;
                    for (i = 1; i <= loopTo; i++)
                    {
                        var loopTo1 = Map.MapHeight;
                        for (j = 1; j <= loopTo1; j++)
                            Map.MaskData[i, j] = true;
                    }

                    if (withBlock.x > 1)
                    {
                        Map.MaskData[withBlock.x - 1, withBlock.y] = false;
                    }

                    if (withBlock.x < Map.MapWidth)
                    {
                        Map.MaskData[withBlock.x + 1, withBlock.y] = false;
                    }

                    if (withBlock.y > 1)
                    {
                        Map.MaskData[withBlock.x, withBlock.y - 1] = false;
                    }

                    if (withBlock.y < Map.MapHeight)
                    {
                        Map.MaskData[withBlock.x, withBlock.y + 1] = false;
                    }
                }
                else
                {
                    // 移動可能な場合
                    TmpMaskData = new bool[Map.MapWidth + 1 + 1, Map.MapHeight + 1 + 1];
                    Map.AreaInSpeed(ref Commands.SelectedUnit);
                    var loopTo2 = Map.MapWidth;
                    for (i = 1; i <= loopTo2; i++)
                    {
                        var loopTo3 = Map.MapHeight;
                        for (j = 1; j <= loopTo3; j++)
                            TmpMaskData[i, j] = Map.MaskData[i, j];
                    }

                    var loopTo4 = Map.MapWidth;
                    for (i = 0; i <= loopTo4; i++)
                    {
                        TmpMaskData[i, 0] = true;
                        TmpMaskData[i, Map.MapHeight + 1] = true;
                    }

                    var loopTo5 = Map.MapHeight;
                    for (i = 0; i <= loopTo5; i++)
                    {
                        TmpMaskData[0, i] = true;
                        TmpMaskData[Map.MapWidth + 1, i] = true;
                    }

                    var loopTo6 = (short)GeneralLib.MinLng(withBlock.x + (withBlock.Speed + 1), Map.MapWidth);
                    for (i = (short)GeneralLib.MaxLng(withBlock.x - (withBlock.Speed + 1), 1); i <= loopTo6; i++)
                    {
                        var loopTo7 = (short)GeneralLib.MinLng(withBlock.y + (withBlock.Speed + 1), Map.MapHeight);
                        for (j = (short)GeneralLib.MaxLng(withBlock.y - (withBlock.Speed + 1), 1); j <= loopTo7; j++)
                            Map.MaskData[i, j] = TmpMaskData[i, j] & TmpMaskData[i - 1, j] & TmpMaskData[i + 1, j] & TmpMaskData[i, j - 1] & TmpMaskData[i, j + 1];
                    }

                    Map.MaskData[withBlock.x, withBlock.y] = true;
                }

                // ターゲットを探す
                // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                Commands.SelectedTarget = null;
                max_dmg = 90;
                var loopTo8 = (short)GeneralLib.MinLng(withBlock.x + (withBlock.Speed + 1), Map.MapWidth);
                for (i = (short)GeneralLib.MaxLng(withBlock.x - (withBlock.Speed + 1), 1); i <= loopTo8; i++)
                {
                    var loopTo9 = (short)GeneralLib.MinLng(withBlock.y + (withBlock.Speed + 1), Map.MapHeight);
                    for (j = (short)GeneralLib.MaxLng(withBlock.y - (withBlock.Speed + 1), 1); j <= loopTo9; j++)
                    {
                        if (Map.MaskData[i, j])
                        {
                            goto NextFixTarget;
                        }

                        u = Map.MapDataForUnit[i, j];
                        if (u is null)
                        {
                            goto NextFixTarget;
                        }

                        // デフォルトのターゲットが指定されている場合はそのユニット以外を
                        // ターゲットにはしない
                        if (t is object)
                        {
                            if (!ReferenceEquals(u, t))
                            {
                                goto NextFixTarget;
                            }
                        }

                        // 現在の選択しているターゲットよりダメージが少なければ選択しない
                        if (100 * u.HP / u.MaxHP > max_dmg)
                        {
                            goto NextFixTarget;
                        }

                        // 味方かどうか判定
                        if (!withBlock.IsAlly(ref u))
                        {
                            goto NextFixTarget;
                        }

                        // ゾンビ？
                        object argIndex2 = "ゾンビ";
                        if (u.IsConditionSatisfied(ref argIndex2))
                        {
                            goto NextFixTarget;
                        }

                        // 修理不可？
                        string argfname1 = "修理不可";
                        if (u.IsFeatureAvailable(ref argfname1))
                        {
                            object argIndex6 = "修理不可";
                            object argIndex7 = "修理不可";
                            var loopTo10 = (short)Conversions.ToInteger(u.FeatureData(ref argIndex7));
                            for (k = 2; k <= loopTo10; k++)
                            {
                                object argIndex3 = "修理不可";
                                string arglist = u.FeatureData(ref argIndex3);
                                fname = GeneralLib.LIndex(ref arglist, k);
                                if (Strings.Left(fname, 1) == "!")
                                {
                                    fname = Strings.Mid(fname, 2);
                                    object argIndex4 = "修理装置";
                                    if ((fname ?? "") != (withBlock.FeatureName0(ref argIndex4) ?? ""))
                                    {
                                        goto NextFixTarget;
                                    }
                                }
                                else
                                {
                                    object argIndex5 = "修理装置";
                                    if ((fname ?? "") == (withBlock.FeatureName0(ref argIndex5) ?? ""))
                                    {
                                        goto NextFixTarget;
                                    }
                                }
                            }
                        }

                        Commands.SelectedTarget = u;
                        max_dmg = 100 * u.HP / u.MaxHP;
                        NextFixTarget:
                        ;
                    }
                }

                // ターゲットが見つからない
                if (Commands.SelectedTarget is null)
                {
                    return TryFixRet;
                }

                // ターゲットに隣接するように移動
                if (!moved & withBlock.Mode != "固定")
                {
                    new_x = withBlock.x;
                    new_y = withBlock.y;
                    {
                        var withBlock1 = Commands.SelectedTarget;
                        // 現在位置から修理が可能であれば現在位置を優先
                        if (Math.Abs((short)(withBlock1.x - new_x)) + Math.Abs((short)(withBlock1.y - new_y)) == 1)
                        {
                            tmp = 1;
                        }
                        else
                        {
                            tmp = 10000;
                        }

                        var loopTo11 = Map.MapWidth;
                        for (i = 1; i <= loopTo11; i++)
                        {
                            var loopTo12 = Map.MapHeight;
                            for (j = 1; j <= loopTo12; j++)
                                Map.MaskData[i, j] = TmpMaskData[i, j];
                        }

                        // 適切な場所を探す
                        var loopTo13 = (short)GeneralLib.MinLng(withBlock1.x + 1, Map.MapWidth);
                        for (i = (short)GeneralLib.MaxLng(withBlock1.x - 1, 1); i <= loopTo13; i++)
                        {
                            var loopTo14 = (short)GeneralLib.MinLng(withBlock1.y + 1, Map.MapHeight);
                            for (j = (short)GeneralLib.MaxLng(withBlock1.y - 1, 1); j <= loopTo14; j++)
                            {
                                if (!Map.MaskData[i, j] & Map.MapDataForUnit[i, j] is null & Math.Abs((short)(withBlock1.x - i)) + Math.Abs((short)(withBlock1.y - j)) == 1)
                                {
                                    {
                                        var withBlock2 = Commands.SelectedUnit;
                                        if (Math.Pow(Math.Abs((short)(withBlock2.x - i)), 2d) + Math.Pow(Math.Abs((short)(withBlock2.y - j)), 2d) < tmp)
                                        {
                                            new_x = i;
                                            new_y = j;
                                            tmp = (int)(Math.Pow(Math.Abs((short)(withBlock2.x - new_x)), 2d) + Math.Pow(Math.Abs((short)(withBlock2.y - new_y)), 2d));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (new_x != withBlock.x | new_y != withBlock.y)
                    {
                        // 適切な場所が見つかったので移動
                        withBlock.Move(new_x, new_y);
                        moved = true;
                    }
                }

                // 選択内容を変更
                Event_Renamed.SelectedUnitForEvent = Commands.SelectedUnit;
                Event_Renamed.SelectedTargetForEvent = Commands.SelectedTarget;

                // メッセージ表示
                GUI.OpenMessageForm(ref Commands.SelectedTarget, ref Commands.SelectedUnit);
                string argmain_situation = "修理";
                if (withBlock.IsMessageDefined(ref argmain_situation))
                {
                    string argSituation = "修理";
                    string argmsg_mode = "";
                    withBlock.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                }

                // アニメ表示
                string argmain_situation3 = "修理";
                object argIndex10 = "修理";
                string argsub_situation2 = withBlock.FeatureName(ref argIndex10);
                if (withBlock.IsAnimationDefined(ref argmain_situation3, ref argsub_situation2))
                {
                    string argmain_situation1 = "修理";
                    object argIndex8 = "修理";
                    string argsub_situation = withBlock.FeatureName(ref argIndex8);
                    withBlock.PlayAnimation(ref argmain_situation1, ref argsub_situation);
                }
                else
                {
                    string argmain_situation2 = "修理";
                    object argIndex9 = "修理";
                    string argsub_situation1 = withBlock.FeatureName(ref argIndex9);
                    withBlock.SpecialEffect(ref argmain_situation2, ref argsub_situation1);
                }

                object argIndex11 = "修理装置";
                GUI.DisplaySysMessage(withBlock.Nickname + "は[" + Commands.SelectedTarget.Nickname + "]に[" + withBlock.FeatureName0(ref argIndex11) + "]を使った。");

                // 修理実行
                tmp = Commands.SelectedTarget.HP;
                object argIndex14 = "修理装置";
                switch (withBlock.FeatureLevel(ref argIndex14))
                {
                    case 1d:
                    case -1:
                        {
                            object argIndex12 = "修理技能";
                            string argref_mode = "";
                            Commands.SelectedTarget.RecoverHP(30d + 3d * withBlock.MainPilot().SkillLevel(ref argIndex12, ref_mode: ref argref_mode));
                            break;
                        }

                    case 2d:
                        {
                            object argIndex13 = "修理技能";
                            string argref_mode1 = "";
                            Commands.SelectedTarget.RecoverHP(50d + 5d * withBlock.MainPilot().SkillLevel(ref argIndex13, ref_mode: ref argref_mode1));
                            break;
                        }

                    case 3d:
                        {
                            Commands.SelectedTarget.RecoverHP(100d);
                            break;
                        }
                }

                string argmsg = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Commands.SelectedTarget.HP - tmp);
                GUI.DrawSysString(Commands.SelectedTarget.x, Commands.SelectedTarget.y, ref argmsg);
                object argu2 = Commands.SelectedUnit;
                GUI.UpdateMessageForm(ref Commands.SelectedTarget, ref argu2);
                GUI.DisplaySysMessage(Commands.SelectedTarget.Nickname + "のＨＰが[" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Commands.SelectedTarget.HP - tmp) + "]回復した。");
            }

            // 経験値獲得
            string argexp_situation = "修理";
            string argexp_mode = "";
            Commands.SelectedUnit.GetExp(ref Commands.SelectedTarget, ref argexp_situation, exp_mode: ref argexp_mode);
            if (GUI.MessageWait < 10000)
            {
                GUI.Sleep(GUI.MessageWait);
            }

            GUI.CloseMessageForm();

            // 形態変化のチェック
            Commands.SelectedTarget.Update();
            Commands.SelectedTarget.CurrentForm().CheckAutoHyperMode();
            Commands.SelectedTarget.CurrentForm().CheckAutoNormalMode();
            TryFixRet = true;
            return TryFixRet;
        }

        // マップ攻撃使用に関する処理
        public static bool TryMapAttack(ref bool moved)
        {
            bool TryMapAttackRet = default;
            short w;
            short xx, tx = default, ty = default, yy;
            short y1, x1, x2, y2;
            short i, j;
            short enemy_num;
            short score, score_limit;
            Unit t;
            var direction = default(string);
            short min_range, max_range, lv;
            var partners = default(Unit[]);
            {
                var withBlock = Commands.SelectedUnit;
                Commands.SaveSelections();

                // マップ攻撃を使用するターゲット数の下限を設定する
                score_limit = 1;
                var loopTo = withBlock.CountWeapon();
                for (i = 1; i <= loopTo; i++)
                {
                    // 通常攻撃を持っている場合は単独の敵への攻撃の際に通常攻撃を優先する
                    string argattr = "Ｍ";
                    if (!withBlock.IsWeaponClassifiedAs(i, ref argattr))
                    {
                        // MOD START マージ
                        // score_limit = 2
                        // Exit For
                        string argref_mode = "移動前";
                        if (withBlock.IsWeaponAvailable(i, ref argref_mode))
                        {
                            score_limit = 2;
                            break;
                        }
                        // MOD END マージ
                    }
                }

                // 威力の高い武器を優先して選択
                w = withBlock.CountWeapon();
                while (w > 0)
                {
                    Commands.SelectedWeapon = w;
                    Commands.SelectedTWeapon = 0;

                    // マップ攻撃かどうか
                    string argattr1 = "Ｍ";
                    if (!withBlock.IsWeaponClassifiedAs(w, ref argattr1))
                    {
                        goto NextWeapon;
                    }

                    // 武器の使用可否を判定
                    if (moved)
                    {
                        string argref_mode1 = "移動後";
                        if (!withBlock.IsWeaponAvailable(w, ref argref_mode1))
                        {
                            goto NextWeapon;
                        }
                    }
                    else
                    {
                        string argref_mode2 = "移動前";
                        if (!withBlock.IsWeaponAvailable(w, ref argref_mode2))
                        {
                            goto NextWeapon;
                        }
                    }

                    // ボスユニットが自爆＆全ＥＮ消費攻撃等を使うのは非常時のみ
                    if (withBlock.BossRank >= 0)
                    {
                        string argattr2 = "自";
                        string argattr3 = "尽";
                        string argattr4 = "消";
                        if (withBlock.IsWeaponClassifiedAs(w, ref argattr2) | withBlock.IsWeaponClassifiedAs(w, ref argattr3) | withBlock.IsWeaponClassifiedAs(w, ref argattr4))
                        {
                            if (withBlock.HP > withBlock.MaxHP / 4)
                            {
                                goto NextWeapon;
                            }
                        }
                    }

                    max_range = withBlock.WeaponMaxRange(w);
                    min_range = withBlock.Weapon(w).MinRange;
                    x1 = (short)GeneralLib.MaxLng(withBlock.x - max_range, 1);
                    y1 = (short)GeneralLib.MaxLng(withBlock.y - max_range, 1);
                    x2 = (short)GeneralLib.MinLng(withBlock.x + max_range, Map.MapWidth);
                    y2 = (short)GeneralLib.MinLng(withBlock.y + max_range, Map.MapHeight);

                    // マップ攻撃の種類にしたがって効果範囲内にいる敵をカウント
                    string argattr7 = "Ｍ直";
                    string argattr8 = "Ｍ拡";
                    string argattr9 = "Ｍ扇";
                    string argattr10 = "Ｍ全";
                    string argattr11 = "Ｍ投";
                    string argattr12 = "Ｍ線";
                    string argattr13 = "Ｍ移";
                    if (withBlock.IsWeaponClassifiedAs(w, ref argattr7))
                    {
                        for (i = 1; i <= 4; i++)
                        {
                            switch (i)
                            {
                                case 1:
                                    {
                                        direction = "N";
                                        break;
                                    }

                                case 2:
                                    {
                                        direction = "S";
                                        break;
                                    }

                                case 3:
                                    {
                                        direction = "W";
                                        break;
                                    }

                                case 4:
                                    {
                                        direction = "E";
                                        break;
                                    }
                            }

                            // 効果範囲を設定
                            Map.AreaInLine(withBlock.x, withBlock.y, min_range, ref max_range, ref direction);
                            Map.MaskData[withBlock.x, withBlock.y] = true;

                            // 効果範囲内にいるユニットをカウント
                            enemy_num = CountTargetInRange(w, x1, y1, x2, y2);

                            // マップ攻撃が最強武器であればターゲットが１体であっても使用
                            if (enemy_num >= score_limit | enemy_num == 1 & w == withBlock.CountWeapon())
                            {
                                switch (direction ?? "")
                                {
                                    case "N":
                                        {
                                            tx = withBlock.x;
                                            ty = (short)GeneralLib.MaxLng(withBlock.y - max_range, 1);
                                            break;
                                        }

                                    case "S":
                                        {
                                            tx = withBlock.x;
                                            ty = (short)GeneralLib.MinLng(withBlock.y + max_range, Map.MapHeight);
                                            break;
                                        }

                                    case "W":
                                        {
                                            tx = (short)GeneralLib.MaxLng(withBlock.x - max_range, 1);
                                            ty = withBlock.y;
                                            break;
                                        }

                                    case "E":
                                        {
                                            tx = (short)GeneralLib.MinLng(withBlock.x + max_range, Map.MapWidth);
                                            ty = withBlock.y;
                                            break;
                                        }
                                }

                                goto FoundWeapon;
                            }
                        }
                    }
                    else if (withBlock.IsWeaponClassifiedAs(w, ref argattr8))
                    {
                        for (i = 1; i <= 4; i++)
                        {
                            switch (i)
                            {
                                case 1:
                                    {
                                        direction = "N";
                                        break;
                                    }

                                case 2:
                                    {
                                        direction = "S";
                                        break;
                                    }

                                case 3:
                                    {
                                        direction = "W";
                                        break;
                                    }

                                case 4:
                                    {
                                        direction = "E";
                                        break;
                                    }
                            }

                            // 効果範囲を設定
                            Map.AreaInCone(withBlock.x, withBlock.y, min_range, ref max_range, ref direction);
                            Map.MaskData[withBlock.x, withBlock.y] = true;

                            // 効果範囲内にいるユニットをカウント
                            enemy_num = CountTargetInRange(w, x1, y1, x2, y2);

                            // マップ攻撃が最強武器であればターゲットが１体であっても使用
                            if (enemy_num >= score_limit | enemy_num == 1 & w == withBlock.CountWeapon())
                            {
                                switch (direction ?? "")
                                {
                                    case "N":
                                        {
                                            tx = withBlock.x;
                                            ty = (short)(withBlock.y - 1);
                                            break;
                                        }

                                    case "S":
                                        {
                                            tx = withBlock.x;
                                            ty = (short)(withBlock.y + 1);
                                            break;
                                        }

                                    case "W":
                                        {
                                            tx = (short)(withBlock.x - 1);
                                            ty = withBlock.y;
                                            break;
                                        }

                                    case "E":
                                        {
                                            tx = (short)(withBlock.x + 1);
                                            ty = withBlock.y;
                                            break;
                                        }
                                }

                                goto FoundWeapon;
                            }
                        }
                    }
                    else if (withBlock.IsWeaponClassifiedAs(w, ref argattr9))
                    {
                        for (i = 1; i <= 4; i++)
                        {
                            switch (i)
                            {
                                case 1:
                                    {
                                        direction = "N";
                                        break;
                                    }

                                case 2:
                                    {
                                        direction = "S";
                                        break;
                                    }

                                case 3:
                                    {
                                        direction = "W";
                                        break;
                                    }

                                case 4:
                                    {
                                        direction = "E";
                                        break;
                                    }
                            }

                            // 効果範囲を設定
                            string argattr5 = "Ｍ扇";
                            Map.AreaInSector(withBlock.x, withBlock.y, min_range, ref max_range, ref direction, (short)withBlock.WeaponLevel(w, ref argattr5));
                            Map.MaskData[withBlock.x, withBlock.y] = true;

                            // 効果範囲内にいるユニットをカウント
                            enemy_num = CountTargetInRange(w, x1, y1, x2, y2);

                            // マップ攻撃が最強武器であればターゲットが１体であっても使用
                            if (enemy_num >= score_limit | enemy_num == 1 & w == withBlock.CountWeapon())
                            {
                                switch (direction ?? "")
                                {
                                    case "N":
                                        {
                                            tx = withBlock.x;
                                            ty = (short)(withBlock.y - 1);
                                            break;
                                        }

                                    case "S":
                                        {
                                            tx = withBlock.x;
                                            ty = (short)(withBlock.y + 1);
                                            break;
                                        }

                                    case "W":
                                        {
                                            tx = (short)(withBlock.x - 1);
                                            ty = withBlock.y;
                                            break;
                                        }

                                    case "E":
                                        {
                                            tx = (short)(withBlock.x + 1);
                                            ty = withBlock.y;
                                            break;
                                        }
                                }

                                goto FoundWeapon;
                            }
                        }
                    }
                    else if (withBlock.IsWeaponClassifiedAs(w, ref argattr10))
                    {
                        // 効果範囲を設定
                        // MOD START マージ
                        // AreaInRange .X, .Y, min_range, max_range, "すべて"
                        string arguparty = "すべて";
                        Map.AreaInRange(withBlock.x, withBlock.y, max_range, min_range, ref arguparty);
                        // MOD END マージ
                        Map.MaskData[withBlock.x, withBlock.y] = true;

                        // 効果範囲内にいるユニットをカウント
                        enemy_num = CountTargetInRange(w, x1, y1, x2, y2);

                        // マップ攻撃が最強武器であればターゲットが１体であっても使用
                        if (enemy_num >= score_limit | enemy_num == 1 & w == withBlock.CountWeapon())
                        {
                            tx = withBlock.x;
                            ty = withBlock.y;
                            goto FoundWeapon;
                        }
                    }
                    else if (withBlock.IsWeaponClassifiedAs(w, ref argattr11))
                    {
                        string argattr6 = "Ｍ投";
                        lv = (short)withBlock.WeaponLevel(w, ref argattr6);
                        score = 0;
                        var loopTo1 = x2;
                        for (xx = x1; xx <= loopTo1; xx++)
                        {
                            var loopTo2 = y2;
                            for (yy = y1; yy <= loopTo2; yy++)
                            {
                                if ((short)(Math.Abs((short)(withBlock.x - xx)) + Math.Abs((short)(withBlock.y - yy))) <= max_range & (short)(Math.Abs((short)(withBlock.x - xx)) + Math.Abs((short)(withBlock.y - yy))) >= min_range)
                                {
                                    // 効果範囲を設定
                                    if (lv > 0)
                                    {
                                        // MOD START マージ
                                        // AreaInRange xx, yy, 1, lv, "すべて"
                                        string arguparty1 = "すべて";
                                        Map.AreaInRange(xx, yy, lv, 1, ref arguparty1);
                                    }
                                    // MOD END マージ
                                    else
                                    {
                                        var loopTo3 = Map.MapWidth;
                                        for (i = 1; i <= loopTo3; i++)
                                        {
                                            var loopTo4 = Map.MapHeight;
                                            for (j = 1; j <= loopTo4; j++)
                                                Map.MaskData[i, j] = true;
                                        }

                                        Map.MaskData[xx, yy] = false;
                                    }

                                    Map.MaskData[withBlock.x, withBlock.y] = true;

                                    // 効果範囲内にいるユニットをカウント
                                    enemy_num = CountTargetInRange(w, (short)(xx - lv), (short)(yy - lv), (short)(xx + lv), (short)(yy + lv));
                                    if (enemy_num > score)
                                    {
                                        score = enemy_num;
                                        tx = xx;
                                        ty = yy;
                                    }
                                }
                            }
                        }

                        // マップ攻撃が最強武器であればターゲットが１体であっても使用
                        // また、Ｍ投L0の場合は最大でも１体の敵しか狙えない
                        if (score >= score_limit | score == 1 & w == withBlock.CountWeapon() | score == 1 & lv == 0)
                        {
                            goto FoundWeapon;
                        }
                    }
                    else if (withBlock.IsWeaponClassifiedAs(w, ref argattr12))
                    {
                        score = 0;
                        var loopTo5 = x2;
                        for (xx = x1; xx <= loopTo5; xx++)
                        {
                            var loopTo6 = y2;
                            for (yy = y1; yy <= loopTo6; yy++)
                            {
                                if ((short)(Math.Abs((short)(withBlock.x - xx)) + Math.Abs((short)(withBlock.y - yy))) <= max_range & (short)(Math.Abs((short)(withBlock.x - xx)) + Math.Abs((short)(withBlock.y - yy))) >= min_range)
                                {
                                    // 効果範囲を設定
                                    Map.AreaInPointToPoint(withBlock.x, withBlock.y, xx, yy);
                                    Map.MaskData[withBlock.x, withBlock.y] = true;

                                    // 効果範囲内にいるユニットをカウント
                                    enemy_num = CountTargetInRange(w, (short)GeneralLib.MinLng(withBlock.x, xx), (short)GeneralLib.MinLng(withBlock.y, yy), (short)GeneralLib.MaxLng(withBlock.x, xx), (short)GeneralLib.MaxLng(withBlock.y, yy));
                                    if (enemy_num > score)
                                    {
                                        score = enemy_num;
                                        tx = xx;
                                        ty = yy;
                                    }
                                }
                            }
                        }

                        // マップ攻撃が最強武器であればターゲットが１体であっても使用
                        if (score >= score_limit | score == 1 & w == withBlock.CountWeapon())
                        {
                            goto FoundWeapon;
                        }
                    }
                    else if (withBlock.IsWeaponClassifiedAs(w, ref argattr13))
                    {
                        // その場を動かない場合は移動型マップ攻撃は選考外
                        if (withBlock.Mode == "固定")
                        {
                            goto NextWeapon;
                        }

                        score = 0;
                        var loopTo7 = x2;
                        for (xx = x1; xx <= loopTo7; xx++)
                        {
                            var loopTo8 = y2;
                            for (yy = y1; yy <= loopTo8; yy++)
                            {
                                if ((short)(Math.Abs((short)(withBlock.x - xx)) + Math.Abs((short)(withBlock.y - yy))) <= max_range & (short)(Math.Abs((short)(withBlock.x - xx)) + Math.Abs((short)(withBlock.y - yy))) >= min_range & Map.MapDataForUnit[xx, yy] is null & withBlock.IsAbleToEnter(xx, yy))
                                {
                                    // 効果範囲を設定
                                    Map.AreaInPointToPoint(withBlock.x, withBlock.y, xx, yy);
                                    Map.MaskData[withBlock.x, withBlock.y] = true;

                                    // 効果範囲内にいるユニットをカウント
                                    enemy_num = CountTargetInRange(w, (short)GeneralLib.MinLng(withBlock.x, xx), (short)GeneralLib.MinLng(withBlock.y, yy), (short)GeneralLib.MaxLng(withBlock.x, xx), (short)GeneralLib.MaxLng(withBlock.y, yy));
                                    if (enemy_num > score)
                                    {
                                        // 最終チェック 目標地点にたどり着けるか？
                                        Map.AreaInMoveAction(ref Commands.SelectedUnit, max_range);
                                        if (!Map.MaskData[xx, yy])
                                        {
                                            score = enemy_num;
                                            tx = xx;
                                            ty = yy;
                                        }
                                    }
                                }
                            }
                        }

                        // マップ攻撃が最強武器であればターゲットが１体であっても使用
                        // また、射程が２の場合は最大でも１体の敵しか狙えない
                        if (score >= score_limit | score == 1 & w == withBlock.CountWeapon() | score == 1 & max_range == 2)
                        {
                            goto FoundWeapon;
                        }
                    }

                    NextWeapon:
                    ;
                    w = (short)(w - 1);
                }

                // 有効なマップ攻撃が見つからなかった

                Commands.RestoreSelections();
                TryMapAttackRet = false;
                return TryMapAttackRet;
                FoundWeapon:
                ;


                // 有効なマップ攻撃が見つかった場合

                // 合体技パートナーの設定
                string argattr14 = "合";
                if (withBlock.IsWeaponClassifiedAs(w, ref argattr14))
                {
                    string argctype_Renamed = "武装";
                    withBlock.CombinationPartner(ref argctype_Renamed, w, ref partners);
                }
                else
                {
                    Commands.SelectedPartners = new Unit[1];
                    partners = new Unit[1];
                }

                // マップ攻撃による攻撃を実行
                withBlock.MapAttack(w, tx, ty);

                // 合体技のパートナーの行動数を減らす
                string argoname = "合体技パートナー行動数無消費";
                if (!Expression.IsOptionDefined(ref argoname))
                {
                    var loopTo9 = (short)Information.UBound(partners);
                    for (i = 1; i <= loopTo9; i++)
                        partners[i].CurrentForm().UseAction();
                }

                Commands.SelectedPartners = new Unit[1];
                Commands.RestoreSelections();
                TryMapAttackRet = true;
            }

            return TryMapAttackRet;
        }

        // 効果範囲内にいるターゲットをカウント
        private static short CountTargetInRange(short w, short x1, short y1, short x2, short y2)
        {
            short CountTargetInRangeRet = default;
            short i, j;
            Unit t;
            var is_ally_involved = default(bool);
            {
                var withBlock = Commands.SelectedUnit;
                // 効果範囲内のターゲットを検索
                var loopTo = (short)GeneralLib.MinLng(x2, Map.MapWidth);
                for (i = (short)GeneralLib.MaxLng(x1, 1); i <= loopTo; i++)
                {
                    var loopTo1 = (short)GeneralLib.MinLng(y2, Map.MapHeight);
                    for (j = (short)GeneralLib.MaxLng(y1, 1); j <= loopTo1; j++)
                    {
                        // 効果範囲内？
                        if (Map.MaskData[i, j])
                        {
                            goto NextPoint;
                        }

                        t = Map.MapDataForUnit[i, j];

                        // ユニットが存在する？
                        if (t is null)
                        {
                            goto NextPoint;
                        }

                        // ダメージを与えられる？
                        if (withBlock.HitProbability(w, ref t, false) == 0)
                        {
                            goto NextPoint;
                        }
                        else if (withBlock.ExpDamage(w, ref t, false) <= 10)
                        {
                            string argattr = "Ｋ";
                            string argattr1 = "吹";
                            if (withBlock.IsNormalWeapon(w))
                            {
                                goto NextPoint;
                            }
                            else if (withBlock.CriticalProbability(w, ref t) <= 1 & withBlock.WeaponLevel(w, ref argattr) == 0d & withBlock.WeaponLevel(w, ref argattr1) == 0d)
                            {
                                goto NextPoint;
                            }
                        }

                        // ターゲットは敵？
                        if (withBlock.IsAlly(ref t))
                        {
                            // 味方の場合は同士討ちの可能性があるのでチェックしておく
                            is_ally_involved = true;
                            goto NextPoint;
                        }

                        // 特定の陣営のみを攻撃する場合
                        switch (withBlock.Mode ?? "")
                        {
                            case "味方":
                            case "ＮＰＣ":
                                {
                                    if (t.Party != "味方" & t.Party != "ＮＰＣ")
                                    {
                                        goto NextPoint;
                                    }

                                    break;
                                }

                            case "敵":
                                {
                                    if (t.Party != "敵")
                                    {
                                        goto NextPoint;
                                    }

                                    break;
                                }

                            case "中立":
                                {
                                    if (t.Party != "中立")
                                    {
                                        goto NextPoint;
                                    }

                                    break;
                                }
                        }

                        // ターゲットが見える？
                        string argsptype = "隠れ身";
                        if (t.IsUnderSpecialPowerEffect(ref argsptype))
                        {
                            goto NextPoint;
                        }

                        string argfname1 = "ステルス";
                        if (t.IsFeatureAvailable(ref argfname1))
                        {
                            object argIndex3 = "ステルス無効";
                            string argfname = "ステルス無効化";
                            if (!t.IsConditionSatisfied(ref argIndex3) & !withBlock.IsFeatureAvailable(ref argfname))
                            {
                                object argIndex2 = "ステルス";
                                if (t.IsFeatureLevelSpecified(ref argIndex2))
                                {
                                    object argIndex1 = "ステルス";
                                    if (Math.Abs((short)(withBlock.x - t.x)) + Math.Abs((short)(withBlock.y - t.y)) > t.FeatureLevel(ref argIndex1))
                                    {
                                        goto NextPoint;
                                    }
                                }
                                else if (Math.Abs((short)(withBlock.x - t.x)) + Math.Abs((short)(withBlock.y - t.y)) > 3)
                                {
                                    goto NextPoint;
                                }
                            }
                        }

                        // ターゲットに含める
                        CountTargetInRangeRet = (short)(CountTargetInRangeRet + 1);
                        NextPoint:
                        ;
                    }
                }

                // 味方を巻き込んでしまう場合は攻撃を止める
                string argattr2 = "識";
                string argsptype1 = "識別攻撃";
                if (is_ally_involved & !withBlock.IsWeaponClassifiedAs(w, ref argattr2) & !withBlock.IsUnderSpecialPowerEffect(ref argsptype1))
                {
                    CountTargetInRangeRet = 0;
                }
            }

            return CountTargetInRangeRet;
        }

        // スペシャルパワーを使用する
        public static void TrySpecialPower(ref Pilot p)
        {
            string slist;
            SpecialPowerData sd;
            short i, tnum;
            Commands.SelectedPilot = p;

            // ザコパイロットはスペシャルパワーを使わない
            if (Strings.InStr(p.Name, "(ザコ)") > 0)
            {
                return;
            }

            // 技量が高いほどスペシャルパワーの発動確率が高い
            if (GeneralLib.Dice(100) > p.TacticalTechnique0() - 100)
            {
                return;
            }

            {
                var withBlock = Commands.SelectedUnit;
                // 正常な判断力がある？
                object argIndex1 = "混乱";
                object argIndex2 = "魅了";
                object argIndex3 = "憑依";
                object argIndex4 = "恐怖";
                object argIndex5 = "狂戦士";
                if (withBlock.IsConditionSatisfied(ref argIndex1) | withBlock.IsConditionSatisfied(ref argIndex2) | withBlock.IsConditionSatisfied(ref argIndex3) | withBlock.IsConditionSatisfied(ref argIndex4) | withBlock.IsConditionSatisfied(ref argIndex5))
                {
                    return;
                }

                // スペシャルパワー使用不能
                object argIndex6 = "スペシャルパワー使用不能";
                if (withBlock.IsConditionSatisfied(ref argIndex6))
                {
                    return;
                }
            }

            // 使用する可能性のあるスペシャルパワーの一覧を作成
            slist = "";
            var loopTo = p.CountSpecialPower;
            for (i = 1; i <= loopTo; i++)
            {
                Commands.SelectedSpecialPower = p.get_SpecialPower(i);
                object argIndex7 = Commands.SelectedSpecialPower;
                sd = SRC.SPDList.Item(ref argIndex7);

                // ＳＰが足りている？
                if (p.SP < p.SpecialPowerCost(ref Commands.SelectedSpecialPower))
                {
                    goto NextSpecialPower;
                }

                // 既に実行済み？
                if (Commands.SelectedUnit.IsSpecialPowerInEffect(ref Commands.SelectedSpecialPower))
                {
                    goto NextSpecialPower;
                }

                object argIndex8 = Commands.SelectedSpecialPower;
                sd = SRC.SPDList.Item(ref argIndex8);
                {
                    var withBlock1 = sd;
                    // ターゲットを選択する必要のあるスペシャルパワーは判断が難しいので
                    // 使用しない
                    switch (withBlock1.TargetType ?? "")
                    {
                        case "味方":
                        case "敵":
                        case "任意":
                            {
                                goto NextSpecialPower;
                                break;
                            }
                    }

                    // ターゲットがいなければ使用しない
                    tnum = withBlock1.CountTarget(ref p);
                    if (tnum == 0)
                    {
                        goto NextSpecialPower;
                    }

                    // 複数のユニットをターゲットにするスペシャルパワーはターゲットが
                    // 少ない場合は使用しない
                    switch (withBlock1.TargetType ?? "")
                    {
                        case "全味方":
                        case "全敵":
                            {
                                if (tnum < 3)
                                {
                                    goto NextSpecialPower;
                                }

                                break;
                            }
                    }

                    // 使用に適した状況下にある？

                    // UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(ＨＰ回復) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    string argename = "ＨＰ回復";
                    if (Conversions.ToBoolean(withBlock1.IsEffectAvailable(ref argename)))
                    {
                        if (withBlock1.TargetType == "自分")
                        {
                            if (Commands.SelectedUnit.HP < 0.7d * Commands.SelectedUnit.MaxHP)
                            {
                                goto AddSpecialPower;
                            }
                        }
                        else if (withBlock1.TargetType == "全味方")
                        {
                            if (SRC.Turn >= 3)
                            {
                                goto AddSpecialPower;
                            }
                        }
                    }

                    // UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(ＥＮ回復) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    string argename1 = "ＥＮ回復";
                    if (Conversions.ToBoolean(withBlock1.IsEffectAvailable(ref argename1)))
                    {
                        if (withBlock1.TargetType == "自分")
                        {
                            if (Commands.SelectedUnit.EN < 0.3d * Commands.SelectedUnit.MaxEN)
                            {
                                goto AddSpecialPower;
                            }
                        }
                        else if (withBlock1.TargetType == "全味方")
                        {
                            if (SRC.Turn >= 4)
                            {
                                goto AddSpecialPower;
                            }
                        }
                    }

                    // UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(気力増加) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    string argename2 = "気力増加";
                    if (Conversions.ToBoolean(withBlock1.IsEffectAvailable(ref argename2)))
                    {
                        if (withBlock1.TargetType == "自分")
                        {
                            if (p.Morale < p.MaxMorale)
                            {
                                if (p.CountSpecialPower == 1 | p.SP > p.MaxSP / 2)
                                {
                                    goto AddSpecialPower;
                                }
                            }
                        }
                        else if (withBlock1.TargetType == "全味方")
                        {
                            goto AddSpecialPower;
                        }
                    }

                    // UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(行動数増加) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    string argename3 = "行動数増加";
                    if (Conversions.ToBoolean(withBlock1.IsEffectAvailable(ref argename3)))
                    {
                        if (withBlock1.TargetType == "自分")
                        {
                            if (DistanceFromNearestEnemy(ref Commands.SelectedUnit) <= 5)
                            {
                                goto AddSpecialPower;
                            }
                        }
                    }

                    // UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(復活) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    string argename4 = "復活";
                    if (Conversions.ToBoolean(withBlock1.IsEffectAvailable(ref argename4)))
                    {
                        if (withBlock1.TargetType == "自分")
                        {
                            goto AddSpecialPower;
                        }
                    }

                    string argename5 = "絶対命中";
                    string argename6 = "ダメージ増加";
                    string argename7 = "クリティカル率増加";
                    string argename8 = "命中強化";
                    string argename9 = "貫通攻撃";
                    string argename10 = "再攻撃";
                    string argename11 = "隠れ身";
                    if (IsSPEffectUseful(ref sd, ref argename5) | IsSPEffectUseful(ref sd, ref argename6) | IsSPEffectUseful(ref sd, ref argename7) | IsSPEffectUseful(ref sd, ref argename8) | IsSPEffectUseful(ref sd, ref argename9) | IsSPEffectUseful(ref sd, ref argename10) | IsSPEffectUseful(ref sd, ref argename11))
                    {
                        if (withBlock1.TargetType == "自分")
                        {
                            if (DistanceFromNearestEnemy(ref Commands.SelectedUnit) <= 5 | withBlock1.Duration == "攻撃")
                            {
                                goto AddSpecialPower;
                            }
                        }
                        else if (withBlock1.TargetType == "全味方")
                        {
                            goto AddSpecialPower;
                        }
                    }

                    string argename12 = "絶対回避";
                    string argename13 = "被ダメージ低下";
                    string argename14 = "装甲強化";
                    string argename15 = "回避強化";
                    if (IsSPEffectUseful(ref sd, ref argename12) | IsSPEffectUseful(ref sd, ref argename13) | IsSPEffectUseful(ref sd, ref argename14) | IsSPEffectUseful(ref sd, ref argename15))
                    {
                        if (withBlock1.TargetType == "自分")
                        {
                            if (DistanceFromNearestEnemy(ref Commands.SelectedUnit) <= 5 | withBlock1.Duration == "防御")
                            {
                                goto AddSpecialPower;
                            }
                        }
                        else if (withBlock1.TargetType == "全味方")
                        {
                            goto AddSpecialPower;
                        }
                    }

                    string argename16 = "移動力強化";
                    if (IsSPEffectUseful(ref sd, ref argename16))
                    {
                        if (withBlock1.TargetType == "自分")
                        {
                            if (DistanceFromNearestEnemy(ref Commands.SelectedUnit) > 5)
                            {
                                goto AddSpecialPower;
                            }
                        }
                        else if (withBlock1.TargetType == "全味方")
                        {
                            goto AddSpecialPower;
                        }
                    }

                    string argename17 = "射程延長";
                    if (IsSPEffectUseful(ref sd, ref argename17))
                    {
                        if (withBlock1.TargetType == "自分")
                        {
                            switch (DistanceFromNearestEnemy(ref Commands.SelectedUnit))
                            {
                                case 5:
                                case 6:
                                    {
                                        goto AddSpecialPower;
                                        break;
                                    }
                            }
                        }
                        else if (withBlock1.TargetType == "全味方")
                        {
                            goto AddSpecialPower;
                        }
                    }

                    string argename18 = "気力低下";
                    string argename19 = "ランダムダメージ";
                    string argename20 = "ＨＰ減少";
                    string argename21 = "ＥＮ減少";
                    string argename22 = "挑発";
                    if (Conversions.ToBoolean(Operators.OrObject(Operators.OrObject(Operators.OrObject(Operators.OrObject(withBlock1.IsEffectAvailable(ref argename18), withBlock1.IsEffectAvailable(ref argename19)), withBlock1.IsEffectAvailable(ref argename20)), withBlock1.IsEffectAvailable(ref argename21)), withBlock1.IsEffectAvailable(ref argename22))))
                    {
                        if (withBlock1.TargetType == "全敵")
                        {
                            goto AddSpecialPower;
                        }
                    }

                    string argename23 = "ダメージ低下";
                    string argename24 = "被ダメージ増加";
                    string argename25 = "命中低下";
                    string argename26 = "回避低下";
                    string argename27 = "命中率低下";
                    string argename28 = "移動力低下";
                    string argename29 = "サポートガード不能";
                    if (Conversions.ToBoolean(Operators.OrObject(Operators.OrObject(Operators.OrObject(Operators.OrObject(Operators.OrObject(Operators.OrObject(withBlock1.IsEffectAvailable(ref argename23), withBlock1.IsEffectAvailable(ref argename24)), withBlock1.IsEffectAvailable(ref argename25)), withBlock1.IsEffectAvailable(ref argename26)), withBlock1.IsEffectAvailable(ref argename27)), withBlock1.IsEffectAvailable(ref argename28)), withBlock1.IsEffectAvailable(ref argename29))))
                    {
                        if (withBlock1.TargetType == "全敵")
                        {
                            if (SRC.Turn >= 3)
                            {
                                goto AddSpecialPower;
                            }
                        }
                    }
                }

                // 有用な効果が見つからなかった
                goto NextSpecialPower;
                AddSpecialPower:
                ;


                // スペシャルパワーを候補リストに追加
                slist = slist + " " + Commands.SelectedSpecialPower;
                NextSpecialPower:
                ;
            }

            // 使用可能なスペシャルパワーを所有していない
            if (string.IsNullOrEmpty(slist))
            {
                Commands.SelectedSpecialPower = "";
                return;
            }

            // 使用するスペシャルパワーをランダムに選択
            Commands.SelectedSpecialPower = GeneralLib.LIndex(ref slist, (short)GeneralLib.Dice(GeneralLib.LLength(ref slist)));

            // 使用イベント
            Event_Renamed.HandleEvent("使用", Commands.SelectedUnit.MainPilot().ID, Commands.SelectedSpecialPower);
            if (SRC.IsScenarioFinished | SRC.IsCanceled)
            {
                return;
            }

            // 選択したスペシャルパワーを実行する
            p.UseSpecialPower(ref Commands.SelectedSpecialPower);
            Commands.SelectedUnit = Commands.SelectedUnit.CurrentForm();

            // ステータスウィンドウ更新
            if (!GUI.IsRButtonPressed())
            {
                Status.DisplayUnitStatus(ref Commands.SelectedUnit);
            }

            // 使用後イベント
            Event_Renamed.HandleEvent("使用後", Commands.SelectedUnit.MainPilot().ID, Commands.SelectedSpecialPower);
            Commands.SelectedSpecialPower = "";
        }

        private static bool IsSPEffectUseful(ref SpecialPowerData sd, ref string ename)
        {
            bool IsSPEffectUsefulRet = default;
            // UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(ename) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            if (Conversions.ToBoolean(sd.IsEffectAvailable(ref ename)))
            {
                if (sd.TargetType == "自分")
                {
                    // 自分自身がターゲットである場合、既に同じ効果を持つスペシャル
                    // パワーを使用している場合は使用しない。
                    if (!Commands.SelectedUnit.IsSpecialPowerInEffect(ref ename))
                    {
                        IsSPEffectUsefulRet = true;
                    }
                }
                else
                {
                    IsSPEffectUsefulRet = true;
                }
            }

            return IsSPEffectUsefulRet;
        }

        // ユニット u がターゲット t を攻撃するための武器を選択
        // amode:攻撃の種類
        // max_prob:敵を破壊できる確率
        // max_dmg:ダメージ期待値
        public static short SelectWeapon(ref Unit u, ref Unit t, [Optional, DefaultParameterValue("")] ref string amode, [Optional, DefaultParameterValue(0)] ref int max_prob, [Optional, DefaultParameterValue(0)] ref int max_dmg)
        {
            short SelectWeaponRet = default;
            string smode;
            bool use_true_value = default, is_move_attack;
            int prob, destroy_prob;
            short ct_prob;
            double sp_prob;
            int dmg, exp_dmg;
            double dmg_mod;
            Unit su;
            int support_dmg = default, support_prob = default, support_exp_dmg = default;
            short w;
            string wclass, wattr;
            int max_destroy_prob, max_exp_dmg;
            short i, j;
            Unit checku;
            string checkwc;
            bool flag;
            short parry_prob;
            string fdata;
            // 御主人さまにはさからえません
            object argIndex1 = "魅了";
            if (u.IsConditionSatisfied(ref argIndex1))
            {
                if (ReferenceEquals(u.Master, t))
                {
                    SelectWeaponRet = -1;
                    return SelectWeaponRet;
                }
            }

            // 踊りに忙しい……
            object argIndex2 = "踊り";
            if (u.IsConditionSatisfied(ref argIndex2))
            {
                SelectWeaponRet = -1;
                return SelectWeaponRet;
            }

            // スペシャルパワー等の影響を考えて武器を選択するかを判定
            if (u.Party == "味方")
            {
                use_true_value = true;
            }

            // ユニットが移動前かどうかを判定
            if (amode == "移動後")
            {
                smode = "移動後";
            }
            else
            {
                smode = "移動前";
            }

            // サポートアタックをしてくれるユニットがいるかどうか
            if (Strings.InStr(amode, "反撃") == 0 & Strings.InStr(amode, "サポート") == 0)
            {
                su = u.LookForSupportAttack(ref t);
                if (su is object)
                {
                    string argamode = "サポートアタック";
                    w = SelectWeapon(ref su, ref t, ref argamode, ref support_prob, ref support_exp_dmg);
                    if (w > 0)
                    {
                        support_prob = GeneralLib.MinLng(su.HitProbability(w, ref t, use_true_value), 100);
                        dmg_mod = 1d;

                        // サポートアタックダメージ低下
                        string argoname = "サポートアタックダメージ低下";
                        if (Expression.IsOptionDefined(ref argoname))
                        {
                            dmg_mod = 0.7d;
                        }

                        // 同時援護攻撃？
                        string argsname = "統率";
                        if (su.MainPilot().IsSkillAvailable(ref argsname) & su.IsNormalWeapon(w))
                        {
                            string argoname1 = "ダメージ倍率低下";
                            if (Expression.IsOptionDefined(ref argoname1))
                            {
                                dmg_mod = 1.2d * dmg_mod;
                            }
                            else
                            {
                                dmg_mod = 1.5d * dmg_mod;
                            }
                        }

                        support_dmg = su.ExpDamage(w, ref t, use_true_value, dmg_mod);
                    }
                }
            }

            SelectWeaponRet = 0;
            max_destroy_prob = 0;
            max_exp_dmg = -1;

            // 各武器を使って試行
            var loopTo = u.CountWeapon();
            for (w = 1; w <= loopTo; w++)
            {
                // 武器が使用可能？
                if (!u.IsWeaponAvailable(w, ref smode))
                {
                    goto NextWeapon;
                }

                // マップ攻撃は武器選定外
                string argattr = "Ｍ";
                if (u.IsWeaponClassifiedAs(w, ref argattr))
                {
                    goto NextWeapon;
                }

                // 合体技は自分から攻撃をかける場合にのみ使用
                string argattr1 = "合";
                if (u.IsWeaponClassifiedAs(w, ref argattr1))
                {
                    if (Strings.InStr(amode, "反撃") > 0 | Strings.InStr(amode, "サポート") > 0)
                    {
                        goto NextWeapon;
                    }
                }

                // 射程範囲内？
                string argattr4 = "移動後攻撃可";
                if (u.IsWeaponClassifiedAs(w, ref argattr4) & amode == "移動可能" & u.Mode != "固定")
                {
                    // 合体技は移動後攻撃可能でも移動を前提にしない
                    // (移動後の位置では使えない危険性があるため)
                    string argattr2 = "合";
                    string argattr3 = "Ｐ";
                    if (u.IsWeaponClassifiedAs(w, ref argattr2) & u.IsWeaponClassifiedAs(w, ref argattr3))
                    {
                        // 移動して攻撃は出来ない
                        if (!u.IsTargetWithinRange(w, ref t))
                        {
                            goto NextWeapon;
                        }

                        is_move_attack = false;
                    }
                    else
                    {
                        // 移動して攻撃可能
                        if (!u.IsTargetReachable(w, ref t))
                        {
                            goto NextWeapon;
                        }

                        is_move_attack = true;
                    }
                }
                else
                {
                    // 移動して攻撃は出来ない
                    if (!u.IsTargetWithinRange(w, ref t))
                    {
                        goto NextWeapon;
                    }

                    is_move_attack = false;
                }

                // 味方ユニットの場合、最後の一発は使用しない
                if (u.Party == "味方" & u.Party0 == "味方" & Strings.InStr(amode, "イベント") == 0)
                {
                    // 自爆攻撃は武器を手動選択する場合にのみ使用
                    string argattr5 = "自";
                    if (u.IsWeaponClassifiedAs(w, ref argattr5))
                    {
                        goto NextWeapon;
                    }

                    // 手動反撃時のサポートアタック以外は残弾数が少ない武器を使用しない
                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    if (amode != "サポートアタック" | GUI.MainForm.mnuMapCommandItem(Commands.AutoDefenseCmdID).Checked)
                    {
                        string argattr6 = "永";
                        if (!u.IsWeaponClassifiedAs(w, ref argattr6))
                        {
                            if (u.Bullet(w) == 1 | u.MaxBullet(w) == 2 | u.MaxBullet(w) == 3)
                            {
                                goto NextWeapon;
                            }
                        }

                        if (u.WeaponENConsumption(w) > 0)
                        {
                            if (u.WeaponENConsumption(w) >= u.EN / 2 | u.WeaponENConsumption(w) >= u.MaxEN / 4)
                            {
                                goto NextWeapon;
                            }
                        }

                        string argattr7 = "尽";
                        if (u.IsWeaponClassifiedAs(w, ref argattr7))
                        {
                            goto NextWeapon;
                        }
                    }
                }

                // ボスユニットが自爆＆全ＥＮ消費攻撃使うのは非常時のみ
                if (u.BossRank >= 0 & Strings.InStr(amode, "イベント") == 0)
                {
                    string argattr8 = "自";
                    string argattr9 = "尽";
                    if (u.IsWeaponClassifiedAs(w, ref argattr8) | u.IsWeaponClassifiedAs(w, ref argattr9))
                    {
                        if (u.HP > u.MaxHP / 4)
                        {
                            goto NextWeapon;
                        }
                    }
                }

                // 特定のユニットをターゲットにしている場合、自爆攻撃はそのターゲットにしか使わない
                string argattr10 = "自";
                if (u.IsWeaponClassifiedAs(w, ref argattr10))
                {
                    bool localIsDefined() { object argIndex1 = u.Mode; var ret = SRC.PList.IsDefined(ref argIndex1); u.Mode = Conversions.ToString(argIndex1); return ret; }

                    if (localIsDefined())
                    {
                        Pilot localItem3() { object argIndex1 = u.Mode; var ret = SRC.PList.Item(ref argIndex1); u.Mode = Conversions.ToString(argIndex1); return ret; }

                        Pilot localItem4() { object argIndex1 = u.Mode; var ret = SRC.PList.Item(ref argIndex1); u.Mode = Conversions.ToString(argIndex1); return ret; }

                        if (localItem4().Unit_Renamed is object)
                        {
                            Pilot localItem2() { object argIndex1 = u.Mode; var ret = SRC.PList.Item(ref argIndex1); u.Mode = Conversions.ToString(argIndex1); return ret; }

                            if (u.IsEnemy(ref localItem2().Unit_Renamed))
                            {
                                Pilot localItem() { object argIndex1 = u.Mode; var ret = SRC.PList.Item(ref argIndex1); u.Mode = Conversions.ToString(argIndex1); return ret; }

                                Pilot localItem1() { object argIndex1 = u.Mode; var ret = SRC.PList.Item(ref argIndex1); u.Mode = Conversions.ToString(argIndex1); return ret; }

                                if (!ReferenceEquals(localItem1().Unit_Renamed, t))
                                {
                                    goto NextWeapon;
                                }
                            }
                        }
                    }
                }

                // ダメージ修正率
                dmg_mod = 1d;

                // サポートアタックダメージ低下
                if (Strings.InStr(amode, "サポート") > 0)
                {
                    string argoname2 = "サポートアタックダメージ低下";
                    if (Expression.IsOptionDefined(ref argoname2))
                    {
                        dmg_mod = 0.7d;
                    }
                }

                // ダメージ算出
                dmg = u.ExpDamage(w, ref t, use_true_value, dmg_mod);

                // 攻撃の可否判定を行う場合はダメージを与えられる武器があればよい
                if (Strings.InStr(amode, "可否判定") > 0)
                {
                    if (dmg > 0)
                    {
                        SelectWeaponRet = w;
                        return SelectWeaponRet;
                    }
                    else if (!u.IsNormalWeapon(w))
                    {
                        if (u.CriticalProbability(w, ref t) > 0)
                        {
                            SelectWeaponRet = w;
                            return SelectWeaponRet;
                        }
                    }

                    goto NextWeapon;
                }

                if (dmg == 0)
                {
                    // 抹殺攻撃は一撃で倒せる場合でないと効果が無い
                    string argattr11 = "殺";
                    if (u.IsWeaponClassifiedAs(w, ref argattr11))
                    {
                        goto NextWeapon;
                    }

                    // ダメージ増加のスペシャルパワーを使用している場合はダメージを与えられない
                    // 武器を選択しない
                    string argsptype = "ダメージ増加";
                    if (u.IsUnderSpecialPowerEffect(ref argsptype))
                    {
                        goto NextWeapon;
                    }
                }

                // 相手のＨＰが10以下の場合はダメージをかさ上げ
                if (t.HP <= 10)
                {
                    if (0 < dmg & dmg < 20)
                    {
                        if (u.Weapon(w).Power > 0)
                        {
                            dmg = 20;
                        }
                    }
                }

                // 再攻撃が可能な場合
                if (Strings.InStr(amode, "サポート") == 0)
                {
                    string argsptype1 = "再攻撃";
                    string argattr13 = "再";
                    if (u.IsUnderSpecialPowerEffect(ref argsptype1))
                    {
                        // 再攻撃する残弾＆ＥＮがある？
                        if (u.Weapon(w).Bullet > 0)
                        {
                            if (u.Bullet(w) < 2)
                            {
                                goto NextWeapon;
                            }
                        }

                        if (u.Weapon(w).ENConsumption > 0)
                        {
                            if (u.EN < 2 * u.WeaponENConsumption(w))
                            {
                                goto NextWeapon;
                            }
                        }

                        dmg = 2 * dmg;
                    }
                    else if (u.IsWeaponClassifiedAs(w, ref argattr13))
                    {
                        string argattr12 = "再";
                        dmg = (int)(dmg + (long)(dmg * u.WeaponLevel(w, ref argattr12)) / 16L);
                    }
                }

                // 命中率算出
                prob = u.HitProbability(w, ref t, use_true_value);

                // 特殊能力による回避を認識する？
                string argsptype2 = "絶対命中";
                if ((u.MainPilot().TacticalTechnique() >= 150 | u.Party == "味方") & !u.IsUnderSpecialPowerEffect(ref argsptype2))
                {
                    // 切り払い可能な場合は命中率を低下
                    string argattr17 = "武";
                    string argattr18 = "突";
                    string argattr19 = "実";
                    if (u.IsWeaponClassifiedAs(w, ref argattr17) | u.IsWeaponClassifiedAs(w, ref argattr18) | u.IsWeaponClassifiedAs(w, ref argattr19))
                    {

                        // 切り払い可能？
                        flag = false;
                        string argfname = "格闘武器";
                        if (t.IsFeatureAvailable(ref argfname))
                        {
                            flag = true;
                        }
                        else
                        {
                            var loopTo1 = t.CountWeapon();
                            for (i = 1; i <= loopTo1; i++)
                            {
                                string argattr14 = "武";
                                if (t.IsWeaponClassifiedAs(i, ref argattr14) & t.IsWeaponMastered(i) & t.MainPilot().Morale >= t.Weapon(i).NecessaryMorale & !t.IsDisabled(ref t.Weapon(i).Name))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }

                        string argsname1 = "切り払い";
                        if (!t.MainPilot().IsSkillAvailable(ref argsname1))
                        {
                            flag = false;
                        }

                        // 切り払い出来る場合は命中率を低下
                        if (flag)
                        {
                            object argIndex3 = "切り払い";
                            string argref_mode = "";
                            parry_prob = (short)(2d * t.MainPilot().SkillLevel(ref argIndex3, ref_mode: ref argref_mode));
                            string argattr16 = "実";
                            if (u.IsWeaponClassifiedAs(w, ref argattr16))
                            {
                                string argattr15 = "サ";
                                if (u.IsWeaponClassifiedAs(w, ref argattr15))
                                {
                                    object argIndex4 = "超感覚";
                                    string argref_mode1 = "";
                                    object argIndex5 = "知覚強化";
                                    string argref_mode2 = "";
                                    parry_prob = (short)(parry_prob - u.MainPilot().SkillLevel(ref argIndex4, ref_mode: ref argref_mode1) - u.MainPilot().SkillLevel(ref argIndex5, ref_mode: ref argref_mode2));
                                    {
                                        var withBlock = t.MainPilot();
                                        object argIndex6 = "超感覚";
                                        string argref_mode3 = "";
                                        object argIndex7 = "知覚強化";
                                        string argref_mode4 = "";
                                        parry_prob = (short)(parry_prob + withBlock.SkillLevel(ref argIndex6, ref_mode: ref argref_mode3) + withBlock.SkillLevel(ref argIndex7, ref_mode: ref argref_mode4));
                                    }
                                }
                            }
                            else
                            {
                                object argIndex8 = "切り払い";
                                string argref_mode5 = "";
                                parry_prob = (short)(parry_prob - u.MainPilot().SkillLevel(ref argIndex8, ref_mode: ref argref_mode5));
                            }

                            if (parry_prob > 0)
                            {
                                prob = prob * (32 - parry_prob) / 32;
                            }
                        }
                    }

                    // 分身可能な場合は命中率を低下
                    string argfname1 = "分身";
                    if (t.IsFeatureAvailable(ref argfname1))
                    {
                        if (t.MainPilot().Morale >= 130)
                        {
                            prob = prob / 2;
                        }
                    }

                    object argIndex10 = "分身";
                    string argref_mode7 = "";
                    if (t.MainPilot().SkillLevel(ref argIndex10, ref_mode: ref argref_mode7) > 0d)
                    {
                        object argIndex9 = "分身";
                        string argref_mode6 = "";
                        prob = (int)((long)(prob * t.MainPilot().SkillLevel(ref argIndex9, ref_mode: ref argref_mode6)) / 16L);
                    }

                    // 超回避可能な場合は命中率を低下
                    string argfname2 = "超回避";
                    if (t.IsFeatureAvailable(ref argfname2))
                    {
                        object argIndex11 = "超回避";
                        fdata = t.FeatureData(ref argIndex11);
                        int localStrToLng() { string argexpr = GeneralLib.LIndex(ref fdata, 2); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

                        int localStrToLng1() { string argexpr = GeneralLib.LIndex(ref fdata, 3); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

                        if (localStrToLng() > t.EN & localStrToLng1() > t.MainPilot().Morale)
                        {
                            object argIndex12 = "超回避";
                            prob = (int)((long)(prob * t.FeatureLevel(ref argIndex12)) / 10L);
                        }
                    }
                }

                // ＣＴ率算出
                ct_prob = u.CriticalProbability(w, ref t);

                // 特殊効果を与える確率を計算
                sp_prob = 0d;
                wclass = u.WeaponClass(w);
                {
                    var withBlock1 = t;
                    var loopTo2 = (short)Strings.Len(wclass);
                    for (i = 1; i <= loopTo2; i++)
                    {
                        wattr = GeneralLib.GetClassBundle(ref wclass, ref i);

                        // 特殊効果無効化によって無効化される？
                        if (withBlock1.SpecialEffectImmune(ref wattr))
                        {
                            goto NextAttribute;
                        }

                        switch (wattr ?? "")
                        {
                            case "縛":
                                {
                                    object argIndex13 = "行動不能";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex13))
                                    {
                                        sp_prob = sp_prob + 0.5d;
                                    }

                                    break;
                                }

                            case "Ｓ":
                                {
                                    object argIndex14 = "行動不能";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex14))
                                    {
                                        sp_prob = sp_prob + 0.3d;
                                    }

                                    break;
                                }

                            case "眠":
                                {
                                    object argIndex15 = "睡眠";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex15))
                                    {
                                        sp_prob = sp_prob + 0.3d;
                                    }

                                    break;
                                }

                            case "痺":
                                {
                                    object argIndex16 = "麻痺";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex16))
                                    {
                                        sp_prob = sp_prob + 0.7d;
                                    }

                                    break;
                                }

                            case "不":
                                {
                                    object argIndex17 = "攻撃不能";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex17) & withBlock1.CountWeapon() > 0)
                                    {
                                        sp_prob = sp_prob + 0.2d;
                                    }

                                    break;
                                }

                            case "止":
                                {
                                    object argIndex18 = "移動不能";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex18) & withBlock1.Speed > 0)
                                    {
                                        sp_prob = sp_prob + 0.2d;
                                    }

                                    break;
                                }

                            case "石":
                                {
                                    object argIndex19 = "石化";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex19) & withBlock1.BossRank < 0)
                                    {
                                        sp_prob = sp_prob + 1d;
                                    }

                                    break;
                                }

                            case "凍":
                                {
                                    object argIndex20 = "凍結";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex20))
                                    {
                                        sp_prob = sp_prob + 0.5d;
                                    }

                                    break;
                                }

                            case "乱":
                                {
                                    object argIndex21 = "混乱";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex21))
                                    {
                                        sp_prob = sp_prob + 0.5d;
                                    }

                                    break;
                                }

                            case "撹":
                                {
                                    object argIndex22 = "撹乱";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex22) & withBlock1.CountWeapon() > 0)
                                    {
                                        sp_prob = sp_prob + 0.2d;
                                    }

                                    break;
                                }

                            case "恐":
                                {
                                    object argIndex23 = "恐怖";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex23))
                                    {
                                        sp_prob = sp_prob + 0.4d;
                                    }

                                    break;
                                }

                            case "魅":
                                {
                                    object argIndex24 = "魅了";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex24))
                                    {
                                        sp_prob = sp_prob + 0.6d;
                                    }

                                    break;
                                }

                            case "憑":
                                {
                                    if (withBlock1.BossRank < 0)
                                    {
                                        sp_prob = sp_prob + 1d;
                                    }

                                    break;
                                }

                            case "黙":
                                {
                                    object argIndex25 = "沈黙";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex25))
                                    {
                                        var loopTo3 = withBlock1.CountWeapon();
                                        for (j = 1; j <= loopTo3; j++)
                                        {
                                            string argattr20 = "音";
                                            if (withBlock1.IsSpellWeapon(j) | withBlock1.IsWeaponClassifiedAs(j, ref argattr20))
                                            {
                                                sp_prob = sp_prob + 0.3d;
                                                break;
                                            }
                                        }

                                        if (j > withBlock1.CountWeapon())
                                        {
                                            var loopTo4 = withBlock1.CountAbility();
                                            for (j = 1; j <= loopTo4; j++)
                                            {
                                                string argattr21 = "音";
                                                if (withBlock1.IsSpellAbility(j) | withBlock1.IsAbilityClassifiedAs(j, ref argattr21))
                                                {
                                                    sp_prob = sp_prob + 0.3d;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }

                            case "盲":
                                {
                                    object argIndex26 = "盲目";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex26))
                                    {
                                        sp_prob = sp_prob + 0.3d;
                                    }

                                    break;
                                }

                            case "毒":
                                {
                                    object argIndex27 = "毒";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex27))
                                    {
                                        sp_prob = sp_prob + 0.3d;
                                    }

                                    break;
                                }

                            case "踊":
                                {
                                    object argIndex28 = "踊り";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex28))
                                    {
                                        sp_prob = sp_prob + 0.3d;
                                    }

                                    break;
                                }

                            case "狂":
                                {
                                    object argIndex29 = "狂戦士";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex29))
                                    {
                                        sp_prob = sp_prob + 0.3d;
                                    }

                                    break;
                                }

                            case "ゾ":
                                {
                                    object argIndex30 = "ゾンビ";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex30))
                                    {
                                        sp_prob = sp_prob + 0.3d;
                                    }

                                    break;
                                }

                            case "害":
                                {
                                    object argIndex31 = "回復不能";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex31))
                                    {
                                        string argfname3 = "ＨＰ回復";
                                        string argfname4 = "ＥＮ回復";
                                        if (withBlock1.IsFeatureAvailable(ref argfname3) | withBlock1.IsFeatureAvailable(ref argfname4))
                                        {
                                            sp_prob = sp_prob + 0.4d;
                                        }
                                    }

                                    break;
                                }

                            case "劣":
                                {
                                    object argIndex32 = "装甲劣化";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex32))
                                    {
                                        sp_prob = sp_prob + 0.3d;
                                    }

                                    break;
                                }

                            case "中":
                                {
                                    object argIndex37 = "バリア無効化";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex37))
                                    {
                                        string argfname5 = "バリア";
                                        object argIndex33 = "バリア";
                                        string argfname6 = "広域バリア";
                                        string argfname7 = "バリアシールド";
                                        object argIndex34 = "バリアシールド";
                                        string argfname8 = "フィールド";
                                        object argIndex35 = "フィールド";
                                        string argfname9 = "広域フィールド";
                                        string argfname10 = "アクティブフィールド";
                                        object argIndex36 = "アクティブフィールド";
                                        if (withBlock1.IsFeatureAvailable(ref argfname5) & Strings.InStr(t.FeatureData(ref argIndex33), "バリア無効化無効") == 0)
                                        {
                                            sp_prob = sp_prob + 0.3d;
                                        }
                                        else if (withBlock1.IsFeatureAvailable(ref argfname6))
                                        {
                                            sp_prob = sp_prob + 0.3d;
                                        }
                                        else if (withBlock1.IsFeatureAvailable(ref argfname7) & Strings.InStr(t.FeatureData(ref argIndex34), "バリア無効化無効") == 0)
                                        {
                                            sp_prob = sp_prob + 0.3d;
                                        }
                                        else if (withBlock1.IsFeatureAvailable(ref argfname8) & Strings.InStr(t.FeatureData(ref argIndex35), "バリア無効化無効") == 0)
                                        {
                                            sp_prob = sp_prob + 0.3d;
                                        }
                                        else if (withBlock1.IsFeatureAvailable(ref argfname9))
                                        {
                                            sp_prob = sp_prob + 0.3d;
                                        }
                                        else if (withBlock1.IsFeatureAvailable(ref argfname10) & Strings.InStr(t.FeatureData(ref argIndex36), "バリア無効化無効") == 0)
                                        {
                                            sp_prob = sp_prob + 0.3d;
                                        }
                                    }

                                    break;
                                }

                            case "除":
                                {
                                    var loopTo5 = withBlock1.CountCondition();
                                    for (j = 1; j <= loopTo5; j++)
                                    {
                                        string localCondition() { object argIndex1 = j; var ret = withBlock1.Condition(ref argIndex1); return ret; }

                                        string localCondition1() { object argIndex1 = j; var ret = withBlock1.Condition(ref argIndex1); return ret; }

                                        string localCondition2() { object argIndex1 = j; var ret = withBlock1.Condition(ref argIndex1); return ret; }

                                        short localConditionLifetime() { object argIndex1 = j; var ret = withBlock1.ConditionLifetime(ref argIndex1); return ret; }

                                        if ((Strings.InStr(localCondition(), "付加") > 0 | Strings.InStr(localCondition1(), "強化") > 0 | Strings.InStr(localCondition2(), "ＵＰ") > 0) & localConditionLifetime() > 0)
                                        {
                                            sp_prob = sp_prob + 0.3d;
                                            break;
                                        }
                                    }

                                    break;
                                }

                            case "即":
                                {
                                    if (withBlock1.BossRank < 0)
                                    {
                                        sp_prob = sp_prob + 1d;
                                    }

                                    break;
                                }

                            case "告":
                                {
                                    if (withBlock1.BossRank < 0)
                                    {
                                        sp_prob = sp_prob + 0.4d;
                                    }

                                    break;
                                }

                            case "脱":
                                {
                                    if (withBlock1.MainPilot().Personality != "機械")
                                    {
                                        sp_prob = sp_prob + 0.2d;
                                    }

                                    break;
                                }

                            case "Ｄ":
                                {
                                    if (withBlock1.MainPilot().Personality != "機械")
                                    {
                                        sp_prob = sp_prob + 0.25d;
                                    }

                                    break;
                                }

                            case "低攻":
                                {
                                    object argIndex38 = "攻撃力ＤＯＷＮ";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex38) & withBlock1.CountWeapon() > 0)
                                    {
                                        sp_prob = sp_prob + 0.2d;
                                    }

                                    break;
                                }

                            case "低防":
                                {
                                    object argIndex39 = "防御力ＤＯＷＮ";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex39))
                                    {
                                        sp_prob = sp_prob + 0.2d;
                                    }

                                    break;
                                }

                            case "低運":
                                {
                                    object argIndex40 = "運動性ＤＯＷＮ";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex40))
                                    {
                                        sp_prob = sp_prob + 0.1d;
                                    }

                                    break;
                                }

                            case "低移":
                                {
                                    object argIndex41 = "移動力ＤＯＷＮ";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex41) & withBlock1.Speed > 0)
                                    {
                                        sp_prob = sp_prob + 0.1d;
                                    }

                                    break;
                                }

                            case "盗":
                                {
                                    object argIndex42 = "すかんぴん";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex42))
                                    {
                                        sp_prob = sp_prob + 0.5d;
                                    }

                                    break;
                                }

                            case "写":
                                {
                                    string argfname11 = "ノーマルモード";
                                    if (withBlock1.BossRank >= 0 | u.IsFeatureAvailable(ref argfname11))
                                    {
                                        goto NextAttribute;
                                    }

                                    switch (u.Size ?? "")
                                    {
                                        case "SS":
                                            {
                                                switch (withBlock1.Size ?? "")
                                                {
                                                    case "M":
                                                    case "L":
                                                    case "LL":
                                                    case "XL":
                                                        {
                                                            goto NextAttribute;
                                                            break;
                                                        }
                                                }

                                                break;
                                            }

                                        case "S":
                                            {
                                                switch (withBlock1.Size ?? "")
                                                {
                                                    case "L":
                                                    case "LL":
                                                    case "XL":
                                                        {
                                                            goto NextAttribute;
                                                            break;
                                                        }
                                                }

                                                break;
                                            }

                                        case "M":
                                            {
                                                switch (withBlock1.Size ?? "")
                                                {
                                                    case "SS":
                                                    case "LL":
                                                    case "XL":
                                                        {
                                                            goto NextAttribute;
                                                            break;
                                                        }
                                                }

                                                break;
                                            }

                                        case "L":
                                            {
                                                switch (withBlock1.Size ?? "")
                                                {
                                                    case "SS":
                                                    case "S":
                                                    case "XL":
                                                        {
                                                            goto NextAttribute;
                                                            break;
                                                        }
                                                }

                                                break;
                                            }

                                        case "LL":
                                            {
                                                switch (withBlock1.Size ?? "")
                                                {
                                                    case "SS":
                                                    case "S":
                                                    case "M":
                                                        {
                                                            goto NextAttribute;
                                                            break;
                                                        }
                                                }

                                                break;
                                            }

                                        case "XL":
                                            {
                                                switch (withBlock1.Size ?? "")
                                                {
                                                    case "SS":
                                                    case "S":
                                                    case "M":
                                                    case "L":
                                                        {
                                                            goto NextAttribute;
                                                            break;
                                                        }
                                                }

                                                break;
                                            }
                                    }

                                    sp_prob = sp_prob + 1d;
                                    break;
                                }

                            case "化":
                                {
                                    string argfname12 = "ノーマルモード";
                                    if (withBlock1.BossRank < 0 & !u.IsFeatureAvailable(ref argfname12))
                                    {
                                        sp_prob = sp_prob + 1d;
                                    }

                                    break;
                                }

                            case "衰":
                                {
                                    if (withBlock1.BossRank >= 0)
                                    {
                                        string argattr22 = "衰";
                                        string argattr23 = "衰";
                                        switch ((short)u.WeaponLevel(w, ref argattr23))
                                        {
                                            case 1:
                                                {
                                                    sp_prob = sp_prob + 1d / 8d;
                                                    break;
                                                }

                                            case 2:
                                                {
                                                    sp_prob = sp_prob + 1d / 4d;
                                                    break;
                                                }

                                            case 3:
                                                {
                                                    sp_prob = sp_prob + 1d / 2d;
                                                    break;
                                                }
                                        }
                                    }
                                    else
                                    {
                                        string argattr24 = "衰";
                                        string argattr25 = "衰";
                                        switch ((short)u.WeaponLevel(w, ref argattr25))
                                        {
                                            case 1:
                                                {
                                                    sp_prob = sp_prob + 1d / 4d;
                                                    break;
                                                }

                                            case 2:
                                                {
                                                    sp_prob = sp_prob + 1d / 2d;
                                                    break;
                                                }

                                            case 3:
                                                {
                                                    sp_prob = sp_prob + 3d / 4d;
                                                    break;
                                                }
                                        }
                                    }

                                    break;
                                }

                            case "滅":
                                {
                                    if (withBlock1.BossRank >= 0)
                                    {
                                        string argattr26 = "滅";
                                        string argattr27 = "滅";
                                        switch ((short)u.WeaponLevel(w, ref argattr27))
                                        {
                                            case 1:
                                                {
                                                    sp_prob = sp_prob + 1d / 16d;
                                                    break;
                                                }

                                            case 2:
                                                {
                                                    sp_prob = sp_prob + 1d / 8d;
                                                    break;
                                                }

                                            case 3:
                                                {
                                                    sp_prob = sp_prob + 1d / 4d;
                                                    break;
                                                }
                                        }
                                    }
                                    else
                                    {
                                        string argattr28 = "滅";
                                        string argattr29 = "滅";
                                        switch ((short)u.WeaponLevel(w, ref argattr29))
                                        {
                                            case 1:
                                                {
                                                    sp_prob = sp_prob + 1d / 8d;
                                                    break;
                                                }

                                            case 2:
                                                {
                                                    sp_prob = sp_prob + 1d / 4d;
                                                    break;
                                                }

                                            case 3:
                                                {
                                                    sp_prob = sp_prob + 1d / 2d;
                                                    break;
                                                }
                                        }
                                    }

                                    break;
                                }

                            default:
                                {
                                    // 弱属性
                                    if (Strings.Left(wattr, 1) == "弱")
                                    {
                                        // 味方全員を検索して、現在対象に攻撃可能なユニットが
                                        // 付加した弱点に対する属性攻撃を持つ場合。
                                        // 特殊効果発動率はとりあえず低防(0.2)とそろえてみた
                                        checkwc = Strings.Mid(wattr, 2);
                                        if (!withBlock1.Weakness(ref checkwc))
                                        {
                                            foreach (Unit currentChecku in SRC.UList)
                                            {
                                                checku = currentChecku;
                                                if ((checku.Party ?? "") == (checku.Party ?? "") & checku.Status_Renamed == "出撃")
                                                {
                                                    var loopTo6 = checku.CountWeapon();
                                                    for (j = 1; j <= loopTo6; j++)
                                                    {
                                                        string argref_mode8 = "移動前";
                                                        if (checku.IsWeaponClassifiedAs(j, ref checkwc) & checku.IsWeaponAvailable(j, ref argref_mode8))
                                                        {
                                                            // 射程範囲内？
                                                            string argattr32 = "移動後攻撃可";
                                                            if (checku.IsWeaponClassifiedAs(j, ref argattr32) & checku.Mode != "固定")
                                                            {
                                                                // 合体技は移動後攻撃可能でも移動を前提にしない
                                                                // (移動後の位置では使えない危険性があるため)
                                                                string argattr30 = "合";
                                                                string argattr31 = "Ｐ";
                                                                if (checku.IsWeaponClassifiedAs(j, ref argattr30) & checku.IsWeaponClassifiedAs(j, ref argattr31))
                                                                {
                                                                    // 移動して攻撃は出来ない
                                                                    if (checku.IsTargetWithinRange(j, ref t))
                                                                    {
                                                                        sp_prob = sp_prob + 0.2d;
                                                                        goto NextAttribute;
                                                                    }
                                                                }
                                                                // 移動して攻撃可能
                                                                else if (checku.IsTargetReachable(j, ref t))
                                                                {
                                                                    sp_prob = sp_prob + 0.2d;
                                                                    goto NextAttribute;
                                                                }
                                                            }
                                                            // 移動して攻撃は出来ない
                                                            else if (checku.IsTargetWithinRange(j, ref t))
                                                            {
                                                                sp_prob = sp_prob + 0.2d;
                                                                goto NextAttribute;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    // 効属性
                                    else if (Strings.Left(wattr, 1) == "効")
                                    {
                                        // 味方全員を検索して、現在対象に攻撃可能なユニットが
                                        // 付加した有効に対する封印、限定攻撃を持つ場合。
                                        // 特殊効果発動率は0.1としてみた
                                        checkwc = Strings.Mid(wattr, 2);
                                        if (!withBlock1.Weakness(ref checkwc) & !withBlock1.Effective(ref checkwc))
                                        {
                                            foreach (Unit currentChecku1 in SRC.UList)
                                            {
                                                checku = currentChecku1;
                                                if ((checku.Party ?? "") == (withBlock1.Party ?? "") & checku.Status_Renamed == "出撃")
                                                {
                                                    var loopTo7 = checku.CountWeapon();
                                                    for (j = 1; j <= loopTo7; j++)
                                                    {
                                                        string argref_mode9 = "移動前";
                                                        if (checku.IsWeaponClassifiedAs(j, ref checkwc) & checku.IsWeaponAvailable(j, ref argref_mode9))
                                                        {
                                                            // 付加する有効に対応する封印、限定武器がある
                                                            short localInStrNotNest1() { string argstring1 = checku.WeaponClass(j); string argstring2 = "封"; var ret = GeneralLib.InStrNotNest(ref argstring1, ref argstring2); return ret; }

                                                            short localInStrNotNest2() { string argstring1 = checku.WeaponClass(j); string argstring2 = "限"; var ret = GeneralLib.InStrNotNest(ref argstring1, ref argstring2); return ret; }

                                                            if (localInStrNotNest1() > 0 | localInStrNotNest2() > 0)
                                                            {
                                                                short localInStrNotNest() { string argstring1 = checku.WeaponClass(j); string argstring2 = "限"; var ret = GeneralLib.InStrNotNest(ref argstring1, ref argstring2); return ret; }

                                                                string argstring1 = checku.WeaponClass(j);
                                                                if (GeneralLib.InStrNotNest(ref argstring1, ref checkwc) > localInStrNotNest())
                                                                {
                                                                    // 射程範囲内？
                                                                    string argattr35 = "移動後攻撃可";
                                                                    if (checku.IsWeaponClassifiedAs(j, ref argattr35) & checku.Mode != "固定")
                                                                    {
                                                                        // 合体技は移動後攻撃可能でも移動を前提にしない
                                                                        // (移動後の位置では使えない危険性があるため)
                                                                        string argattr33 = "合";
                                                                        string argattr34 = "Ｐ";
                                                                        if (checku.IsWeaponClassifiedAs(j, ref argattr33) & checku.IsWeaponClassifiedAs(j, ref argattr34))
                                                                        {
                                                                            // 移動して攻撃は出来ない
                                                                            if (checku.IsTargetWithinRange(j, ref t))
                                                                            {
                                                                                sp_prob = sp_prob + 0.1d;
                                                                                goto NextAttribute;
                                                                            }
                                                                        }
                                                                        // 移動して攻撃可能
                                                                        else if (checku.IsTargetReachable(j, ref t))
                                                                        {
                                                                            sp_prob = sp_prob + 0.1d;
                                                                            goto NextAttribute;
                                                                        }
                                                                    }
                                                                    // 移動して攻撃は出来ない
                                                                    else if (checku.IsTargetWithinRange(j, ref t))
                                                                    {
                                                                        sp_prob = sp_prob + 0.1d;
                                                                        goto NextAttribute;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    // 剋属性
                                    else if (Strings.Left(wattr, 1) == "剋")
                                    {
                                        // 特殊効果発動率は黙属性揃えで0.3
                                        checkwc = Strings.Mid(wattr, 2);
                                        switch (checkwc ?? "")
                                        {
                                            case "オ":
                                                {
                                                    object argIndex43 = "オーラ使用不能";
                                                    if (!withBlock1.IsConditionSatisfied(ref argIndex43))
                                                    {
                                                        string argsname2 = "オーラ";
                                                        if (withBlock1.IsSkillAvailable(ref argsname2))
                                                        {
                                                            sp_prob = sp_prob + 0.3d;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        goto NextAttribute;
                                                    }

                                                    break;
                                                }

                                            case "超":
                                                {
                                                    object argIndex44 = "超能力使用不能";
                                                    if (!withBlock1.IsConditionSatisfied(ref argIndex44))
                                                    {
                                                        string argsname3 = "超能力";
                                                        if (withBlock1.IsSkillAvailable(ref argsname3))
                                                        {
                                                            sp_prob = sp_prob + 0.3d;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        goto NextAttribute;
                                                    }

                                                    break;
                                                }

                                            case "シ":
                                                {
                                                    object argIndex45 = "同調率使用不能";
                                                    if (!withBlock1.IsConditionSatisfied(ref argIndex45))
                                                    {
                                                        string argsname4 = "同調率";
                                                        if (withBlock1.IsSkillAvailable(ref argsname4))
                                                        {
                                                            sp_prob = sp_prob + 0.3d;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        goto NextAttribute;
                                                    }

                                                    break;
                                                }

                                            case "サ":
                                                {
                                                    object argIndex46 = "超感覚使用不能";
                                                    object argIndex47 = "知覚強化使用不能";
                                                    if (!withBlock1.IsConditionSatisfied(ref argIndex46) & !withBlock1.IsConditionSatisfied(ref argIndex47))
                                                    {
                                                        string argsname5 = "超感覚";
                                                        string argsname6 = "知覚強化";
                                                        if (withBlock1.IsSkillAvailable(ref argsname5) | withBlock1.IsSkillAvailable(ref argsname6))
                                                        {
                                                            sp_prob = sp_prob + 0.3d;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        goto NextAttribute;
                                                    }

                                                    break;
                                                }

                                            case "霊":
                                                {
                                                    object argIndex48 = "霊力使用不能";
                                                    if (!withBlock1.IsConditionSatisfied(ref argIndex48))
                                                    {
                                                        string argsname7 = "霊力";
                                                        if (withBlock1.IsSkillAvailable(ref argsname7))
                                                        {
                                                            sp_prob = sp_prob + 0.3d;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        goto NextAttribute;
                                                    }

                                                    break;
                                                }

                                            case "術":
                                                {
                                                    // 術は射撃を魔力と表示するためだけに付いている場合があるため
                                                    // 1レベル以下の場合は武器、アビリティを確認
                                                    object argIndex49 = "術使用不能";
                                                    if (!withBlock1.IsConditionSatisfied(ref argIndex49))
                                                    {
                                                        if (withBlock1.SkillLevel("術") > 1d)
                                                        {
                                                            sp_prob = sp_prob + 0.3d;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        goto NextAttribute;
                                                    }

                                                    break;
                                                }

                                            case "技":
                                                {
                                                    object argIndex50 = "技使用不能";
                                                    if (!withBlock1.IsConditionSatisfied(ref argIndex50))
                                                    {
                                                        string argsname8 = "技";
                                                        if (withBlock1.IsSkillAvailable(ref argsname8))
                                                        {
                                                            sp_prob = sp_prob + 0.3d;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        goto NextAttribute;
                                                    }

                                                    break;
                                                }
                                        }

                                        bool localIsConditionSatisfied() { object argIndex1 = checkwc + "属性使用不能"; var ret = withBlock1.IsConditionSatisfied(ref argIndex1); return ret; }

                                        if (!localIsConditionSatisfied())
                                        {
                                            var loopTo8 = withBlock1.CountWeapon();
                                            for (j = 1; j <= loopTo8; j++)
                                            {
                                                if (withBlock1.IsWeaponClassifiedAs(j, ref checkwc))
                                                {
                                                    sp_prob = sp_prob + 0.3d;
                                                    break;
                                                }
                                            }

                                            if (j > withBlock1.CountWeapon())
                                            {
                                                var loopTo9 = withBlock1.CountAbility();
                                                for (j = 1; j <= loopTo9; j++)
                                                {
                                                    if (withBlock1.IsAbilityClassifiedAs(j, ref checkwc))
                                                    {
                                                        sp_prob = sp_prob + 0.3d;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }
                        }

                        NextAttribute:
                        ;
                    }
                }

                if (sp_prob > 1d)
                {
                    sp_prob = Math.Sqrt(sp_prob);
                }

                sp_prob = sp_prob * ct_prob;

                // バリア等で攻撃が防がれてしまう場合は特殊効果は発動しない
                string argtarea = "";
                string argattr36 = "無";
                if (u.WeaponPower(w, ref argtarea) > 0 & dmg == 0 & !u.IsWeaponClassifiedAs(w, ref argattr36))
                {
                    sp_prob = 0d;
                }

                // 必ず発動する特殊効果を考慮
                string argattr37 = "吸";
                if (u.IsWeaponClassifiedAs(w, ref argattr37))
                {
                    if (u.HP < u.MaxHP)
                    {
                        sp_prob = sp_prob + 25 * dmg / t.MaxHP;
                    }
                }

                string argattr38 = "減";
                if (u.IsWeaponClassifiedAs(w, ref argattr38))
                {
                    sp_prob = sp_prob + 50 * dmg / t.MaxHP;
                }

                string argattr39 = "奪";
                if (u.IsWeaponClassifiedAs(w, ref argattr39))
                {
                    sp_prob = sp_prob + 50 * dmg / t.MaxHP;
                }

                // 先制攻撃の場合は特殊効果を有利に判定
                if (Strings.InStr(amode, "反撃") > 0)
                {
                    string argattr40 = "先";
                    if (u.IsWeaponClassifiedAs(w, ref argattr40) | u.UsedCounterAttack < u.MaxCounterAttack())
                    {
                        sp_prob = 1.5d * sp_prob;
                    }
                }

                if (sp_prob > 100d)
                {
                    sp_prob = 100d;
                }

                // ＣＴ率が低い場合は特殊効果のみの攻撃を重視しない
                if (dmg == 0 & ct_prob < 30)
                {
                    sp_prob = sp_prob / 5d;
                }

                // ダメージが与えられない武器は使用しない
                if (dmg == 0 & sp_prob == 0d)
                {
                    goto NextWeapon;
                }

                if (prob > 0)
                {
                    if (sp_prob > 0d)
                    {
                        // 特殊効果の影響を加味してダメージの期待値を計算
                        exp_dmg = (int)(dmg + (long)(GeneralLib.MaxLng(t.HP - dmg, 0) * sp_prob) / 100L);
                    }
                    else
                    {
                        // クリティカルの影響を加味してダメージの期待値を計算
                        string argoname3 = "ダメージ倍率低下";
                        if (Expression.IsOptionDefined(ref argoname3))
                        {
                            string argattr42 = "痛";
                            if (u.IsWeaponClassifiedAs(w, ref argattr42))
                            {
                                string argattr41 = "痛";
                                exp_dmg = (int)(dmg + (long)(0.1d * u.WeaponLevel(w, ref argattr41) * dmg * ct_prob) / 100L);
                            }
                            else
                            {
                                exp_dmg = (int)(dmg + (long)(0.2d * dmg * ct_prob) / 100L);
                            }
                        }
                        else
                        {
                            string argattr44 = "痛";
                            if (u.IsWeaponClassifiedAs(w, ref argattr44))
                            {
                                string argattr43 = "痛";
                                exp_dmg = (int)(dmg + (long)(0.25d * u.WeaponLevel(w, ref argattr43) * dmg * ct_prob) / 100L);
                            }
                            else
                            {
                                exp_dmg = (int)(dmg + (long)(0.5d * dmg * ct_prob) / 100L);
                            }
                        }
                    }

                    exp_dmg = (int)(exp_dmg * 0.01d * GeneralLib.MinLng(prob, 100));
                }
                else
                {
                    // 命中が当たらない場合は期待値を思い切り下げる
                    prob = 1;
                    exp_dmg = (int)((dmg / 10 + (long)(GeneralLib.MaxLng(t.HP - dmg / 10, 0) * sp_prob) / 100L) / 10L);
                }

                // サポートによるダメージを期待値に追加
                if (!is_move_attack)
                {
                    exp_dmg = exp_dmg + support_exp_dmg;
                }

                // 敵の破壊確率を計算
                destroy_prob = 0;
                string argfname13 = "防御不可";
                if (t.Party == "味方" & !t.IsFeatureAvailable(ref argfname13))
                {
                    if (dmg >= 2 * t.HP)
                    {
                        destroy_prob = GeneralLib.MinLng(prob, 100);
                    }
                    // サポートによる破壊確率
                    if (!is_move_attack)
                    {
                        if (support_dmg >= 2 * t.HP)
                        {
                            destroy_prob = destroy_prob + (100 - destroy_prob) * support_prob / 100;
                        }
                        else if (dmg + support_dmg >= 2 * t.HP)
                        {
                            destroy_prob = destroy_prob + (100 - destroy_prob) * prob * support_prob / 10000;
                        }
                    }
                }
                else
                {
                    if (dmg >= t.HP)
                    {
                        destroy_prob = GeneralLib.MinLng(prob, 100);
                    }
                    // サポートによる破壊確率
                    if (!is_move_attack)
                    {
                        if (support_dmg >= t.HP)
                        {
                            destroy_prob = destroy_prob + (100 - destroy_prob) * support_prob / 100;
                        }
                        else if (dmg + support_dmg >= t.HP)
                        {
                            destroy_prob = destroy_prob + (100 - destroy_prob) * prob * support_prob / 10000;
                        }
                    }
                }

                // 先制攻撃の場合は敵を破壊出来る攻撃を有利に判定
                if (Strings.InStr(amode, "反撃") > 0)
                {
                    string argattr45 = "先";
                    if (u.IsWeaponClassifiedAs(w, ref argattr45) | u.UsedCounterAttack < u.MaxCounterAttack())
                    {
                        destroy_prob = (int)(1.5d * destroy_prob);
                    }
                }

                // ＥＮ消耗攻撃の使用は慎重に
                string argattr46 = "消";
                if (u.IsWeaponClassifiedAs(w, ref argattr46))
                {
                    if (u.Party == "味方")
                    {
                        // 自動反撃モードかどうか
                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                        if (GUI.MainForm.mnuMapCommandItem(Commands.AutoDefenseCmdID).Checked)
                        {
                            goto NextWeapon;
                        }
                    }
                    // 敵ユニットは相手を倒せるときにしかＥＮ消耗攻撃を使わない
                    else if (destroy_prob == 0 & u.BossRank < 0)
                    {
                        goto NextWeapon;
                    }
                }

                if (destroy_prob >= 100)
                {
                    // 破壊確率が100%の場合はコストの低さを優先
                    // (確率が同じ場合は番号が低い武器を使用)
                    if (u.Party == "味方" | u.Party == "ＮＰＣ")
                    {
                        if (destroy_prob > max_destroy_prob)
                        {
                            SelectWeaponRet = w;
                            max_destroy_prob = destroy_prob;
                            max_exp_dmg = exp_dmg;
                        }
                    }
                    // 敵の場合はコスト無視
                    else if (destroy_prob > max_destroy_prob)
                    {
                        SelectWeaponRet = w;
                        max_destroy_prob = destroy_prob;
                        max_exp_dmg = exp_dmg;
                    }
                    else if (exp_dmg > max_exp_dmg)
                    {
                        SelectWeaponRet = w;
                        max_destroy_prob = destroy_prob;
                        max_exp_dmg = exp_dmg;
                    }
                }
                else if (destroy_prob > 50)
                {
                    // 破壊確率が50%より高い場合は破壊確率の高さを優先
                    if (destroy_prob > max_destroy_prob)
                    {
                        SelectWeaponRet = w;
                        max_destroy_prob = destroy_prob;
                        max_exp_dmg = exp_dmg;
                    }
                    else if (destroy_prob == max_destroy_prob)
                    {
                        if (exp_dmg > max_exp_dmg)
                        {
                            SelectWeaponRet = w;
                            max_destroy_prob = destroy_prob;
                            max_exp_dmg = exp_dmg;
                        }
                    }
                }
                // 破壊確率が50%以下の場合はダメージの期待値の高さを優先
                else if (max_destroy_prob <= 50)
                {
                    if (exp_dmg > max_exp_dmg)
                    {
                        SelectWeaponRet = w;
                        max_destroy_prob = destroy_prob;
                        max_exp_dmg = exp_dmg;
                    }
                }

                NextWeapon:
                ;
            }

            // ダメージを与えられない武器が選択された場合はキャンセル
            if (SelectWeaponRet > 0)
            {
                if (u.WeaponAdaption(SelectWeaponRet, ref t.Area) == 0d)
                {
                    SelectWeaponRet = 0;
                }
            }

            // 攻撃結果の期待値の書き込み
            if (max_destroy_prob > 50)
            {
                max_prob = max_destroy_prob;
            }
            else
            {
                max_prob = 0;
            }

            max_dmg = (int)(100d * (max_exp_dmg / (double)u.HP));
            return SelectWeaponRet;
        }

        // ユニット u が武器 w で攻撃をかけた際にターゲット t が選択する防御行動を返す
        public static object SelectDefense(ref Unit u, ref short w, ref Unit t, ref short tw)
        {
            object SelectDefenseRet = default;
            int prob, dmg;
            int tprob = default, tdmg = default;
            var is_target_inferior = default(bool);

            // マップ攻撃に対しては防御行動が取れない
            string argattr = "Ｍ";
            if (u.IsWeaponClassifiedAs(w, ref argattr))
            {
                return SelectDefenseRet;
            }

            {
                var withBlock = t;
                // 踊っている場合は回避扱い
                object argIndex1 = "踊り";
                if (withBlock.IsConditionSatisfied(ref argIndex1))
                {
                    // UPGRADE_WARNING: オブジェクト SelectDefense の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    SelectDefenseRet = "回避";
                    return SelectDefenseRet;
                }

                // 狂戦士状態の際は防御行動を取らない
                object argIndex2 = "狂戦士";
                if (withBlock.IsConditionSatisfied(ref argIndex2))
                {
                    return SelectDefenseRet;
                }

                // 無防備状態のユニットは防御行動が取れない
                string argsptype = "無防備";
                if (withBlock.IsUnderSpecialPowerEffect(ref argsptype))
                {
                    return SelectDefenseRet;
                }

                if (withBlock.Party != "味方")
                {
                    // 「敵ユニット防御使用」オプションを選択している場合にのみ敵ユニットは
                    // 防御行動を行う
                    string argoname = "敵ユニット防御使用";
                    if (!Expression.IsOptionDefined(ref argoname))
                    {
                        return SelectDefenseRet;
                    }

                    // 防御行動を使ってくるのは技量が160以上のザコでないパイロットのみ
                    {
                        var withBlock1 = withBlock.MainPilot();
                        if (Strings.InStr(withBlock1.Name, "(ザコ)") > 0 | withBlock1.TacticalTechnique() < 160)
                        {
                            return SelectDefenseRet;
                        }
                    }
                }

                // 行動不能？
                if (withBlock.MaxAction() == 0)
                {
                    // チャージ中、消耗中は常に防御、それ以外の場合は防御行動が取れない
                    object argIndex3 = "チャージ";
                    object argIndex4 = "消耗";
                    if (withBlock.IsConditionSatisfied(ref argIndex3) | withBlock.IsConditionSatisfied(ref argIndex4))
                    {
                        // UPGRADE_WARNING: オブジェクト SelectDefense の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        SelectDefenseRet = "防御";
                    }

                    return SelectDefenseRet;
                }

                // 相手の攻撃のダメージ・命中率を算出
                dmg = u.ExpDamage(w, ref t, true);
                prob = GeneralLib.MinLng(u.HitProbability(w, ref t, true), 100);

                // ダミーを持っている場合、相手の攻撃は無効
                string argfname = "ダミー";
                object argIndex5 = "ダミー破壊";
                object argIndex6 = "ダミー";
                if (withBlock.IsFeatureAvailable(ref argfname) & withBlock.ConditionLevel(ref argIndex5) < withBlock.FeatureLevel(ref argIndex6))
                {
                    prob = 0;
                }

                // サポートガードされる場合も相手の攻撃は無効
                if (withBlock.LookForSupportGuard(ref u, w) is object)
                {
                    prob = 0;
                }

                // 反撃のダメージ・命中率を算出
                if (tw > 0)
                {
                    tdmg = withBlock.ExpDamage(tw, ref u, true);
                    tprob = GeneralLib.MinLng(withBlock.HitProbability(tw, ref u, true), 100);

                    // ダミーを持っている場合は反撃は無効
                    string argfname1 = "ダミー";
                    object argIndex7 = "ダミー破壊";
                    object argIndex8 = "ダミー";
                    if (u.IsFeatureAvailable(ref argfname1) & u.ConditionLevel(ref argIndex7) < u.FeatureLevel(ref argIndex8))
                    {
                        prob = 0;
                    }
                }

                // 相手の攻撃の効果とこちらの反撃の効果を比較
                if (withBlock.Party == "味方")
                {
                    // 味方ユニットの場合、相手の攻撃によるダメージの方が多い場合は防御
                    if (dmg * prob > tdmg * tprob & tdmg < u.HP)
                    {
                        is_target_inferior = true;
                    }

                    // 気合の一撃は防御を優先し、やり過ごす
                    string argsptype1 = "ダメージ増加";
                    if (u.IsUnderSpecialPowerEffect(ref argsptype1))
                    {
                        if (2 * dmg * prob > tdmg * tprob & tdmg < u.HP)
                        {
                            is_target_inferior = true;
                        }
                    }
                }
                else
                {
                    // 敵ユニットの場合でも相手の攻撃によるダメージの方が２倍以上多い場合は防御
                    if (dmg * prob / 2 > tdmg * tprob & tdmg < u.HP)
                    {
                        is_target_inferior = true;
                    }

                    // 気合の一撃は防御を優先し、やり過ごす
                    string argsptype2 = "ダメージ増加";
                    if (u.IsUnderSpecialPowerEffect(ref argsptype2))
                    {
                        if (dmg * prob > tdmg * tprob & tdmg < u.HP)
                        {
                            is_target_inferior = true;
                        }
                    }
                }

                // あと一撃で破壊されてしまう場合は必ず防御
                // (命中率が低い場合を除く)
                if (dmg >= withBlock.HP & prob > 10)
                {
                    is_target_inferior = true;
                }

                if (tw > 0)
                {
                    // 先制攻撃可能？
                    string argattr3 = "後";
                    if (!withBlock.IsWeaponClassifiedAs(tw, ref argattr3))
                    {
                        string argattr1 = "先";
                        string argattr2 = "後";
                        if (withBlock.IsWeaponClassifiedAs(tw, ref argattr1) | u.IsWeaponClassifiedAs(w, ref argattr2) | withBlock.MaxCounterAttack() > withBlock.UsedCounterAttack)
                        {
                            if (tdmg >= u.HP & tprob > 70)
                            {
                                // 先制攻撃で倒せる場合は迷わず反撃
                                is_target_inferior = false;
                            }
                        }
                    }
                }
                else
                {
                    // 反撃できない場合は防御
                    is_target_inferior = true;
                }

                if (!is_target_inferior)
                {
                    // 反撃を選択した
                    return SelectDefenseRet;
                }

                // 防御側が劣勢なので反撃は行わず、防御行動を選択

                // 命中すれば一撃死で、防御すれば破壊をまぬがれる攻撃は必ず防御
                string argfname2 = "防御不可";
                string argattr4 = "殺";
                if (dmg > withBlock.HP & dmg / 2 < withBlock.HP & !withBlock.IsFeatureAvailable(ref argfname2) & !u.IsWeaponClassifiedAs(w, ref argattr4))
                {
                    // UPGRADE_WARNING: オブジェクト SelectDefense の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    SelectDefenseRet = "防御";
                    return SelectDefenseRet;
                }

                // 相手の命中率が低い場合は回避
                string argfname3 = "回避不可";
                object argIndex9 = "移動不能";
                if (prob < 50 & !withBlock.IsFeatureAvailable(ref argfname3) & !withBlock.IsConditionSatisfied(ref argIndex9))
                {
                    // UPGRADE_WARNING: オブジェクト SelectDefense の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    SelectDefenseRet = "回避";
                    return SelectDefenseRet;
                }

                // 防御すれば一撃死をまぬがれる場合は防御
                string argfname4 = "防御不可";
                string argattr5 = "殺";
                if (dmg / 2 < withBlock.HP & !withBlock.IsFeatureAvailable(ref argfname4) & !u.IsWeaponClassifiedAs(w, ref argattr5))
                {
                    // UPGRADE_WARNING: オブジェクト SelectDefense の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    SelectDefenseRet = "防御";
                    return SelectDefenseRet;
                }

                // どうしようもないのでとりあえず回避
                string argfname5 = "回避不可";
                object argIndex10 = "移動不能";
                if (!withBlock.IsFeatureAvailable(ref argfname5) & !withBlock.IsConditionSatisfied(ref argIndex10))
                {
                    // UPGRADE_WARNING: オブジェクト SelectDefense の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    SelectDefenseRet = "回避";
                    return SelectDefenseRet;
                }

                // 回避も出来ないので防御……
                string argfname6 = "防御不可";
                if (!withBlock.IsFeatureAvailable(ref argfname6))
                {
                    // UPGRADE_WARNING: オブジェクト SelectDefense の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    SelectDefenseRet = "防御";
                }
            }

            return SelectDefenseRet;
        }

        // ユニット u がターゲット t に反撃可能か？
        public static bool IsAbleToCounterAttack(ref Unit u, ref Unit t)
        {
            bool IsAbleToCounterAttackRet = default;
            short i, w, idx;
            string buf, wclass, ch;
            var loopTo = u.CountWeapon();
            for (w = 1; w <= loopTo; w++)
            {
                // 武器が使用可能？
                string argref_mode = "移動前";
                if (!u.IsWeaponAvailable(w, ref argref_mode))
                {
                    goto NextWeapon;
                }

                // マップ攻撃は武器選定外
                string argattr = "Ｍ";
                if (u.IsWeaponClassifiedAs(w, ref argattr))
                {
                    goto NextWeapon;
                }

                // 合体技は自分から攻撃をかける場合にのみ使用
                string argattr1 = "合";
                if (u.IsWeaponClassifiedAs(w, ref argattr1))
                {
                    goto NextWeapon;
                }

                // 射程範囲内？
                if (!u.IsTargetWithinRange(w, ref t))
                {
                    goto NextWeapon;
                }

                // ADD START マージ
                // ダメージを与えられる？
                if (u.Damage(w, ref t, true) > 0)
                {
                    IsAbleToCounterAttackRet = true;
                    return IsAbleToCounterAttackRet;
                }

                // 特殊効果を与えられる？
                if (!u.IsNormalWeapon(w))
                {
                    if (u.CriticalProbability(w, ref t) > 0)
                    {
                        IsAbleToCounterAttackRet = true;
                        return IsAbleToCounterAttackRet;
                    }
                }
                // ADD END マージ

                // DEL START マージ
                // '地形適応は？
                // If .WeaponAdaption(w, t.Area) = 0 Then
                // GoTo NextWeapon
                // End If
                // 
                // '封印攻撃は弱点、有効を持つユニット以外には効かない
                // If .IsWeaponClassifiedAs(w, "封") Then
                // wclass = .WeaponClass(w)
                // buf = t.strWeakness & t.strEffective
                // For i = 1 To Len(buf)
                // ch = GetClassBundle(buf, i)
                // If ch <> "物" And ch <> "魔" Then
                // If InStrNotNest(wclass, ch) > 0 Then
                // Exit For
                // End If
                // End If
                // Next
                // If i > Len(buf) Then
                // GoTo NextWeapon
                // End If
                // End If
                // 
                // '限定攻撃は指定属性を弱点、有効を持つユニット以外には効かない
                // idx = InStrNotNest(.WeaponClass(w), "限")
                // If idx > 0 Then
                // wclass = .WeaponClass(w)
                // buf = t.strWeakness & t.strEffective
                // For i = 1 To Len(buf)
                // ch = GetClassBundle(buf, i)
                // If ch <> "物" And ch <> "魔" Then
                // If InStrNotNest(wclass, ch) > idx Then
                // Exit For
                // End If
                // End If
                // Next
                // If i > Len(buf) Then
                // GoTo NextWeapon
                // End If
                // End If
                // 
                // '特定レベル限定攻撃
                // If .IsWeaponClassifiedAs(w, "対") Then
                // If t.MainPilot.Level Mod .WeaponLevel(w, "対") <> 0 Then
                // GoTo NextWeapon
                // End If
                // End If
                // 
                // '反撃に使用できる武器が見つかった
                // IsAbleToCounterAttack = True
                // Exit Function
                // DEL END マージ
                NextWeapon:
                ;
            }

            // 反撃に使用できる武器がなかった
            IsAbleToCounterAttackRet = false;
            return IsAbleToCounterAttackRet;
        }

        // 最も近い敵ユニットを探す
        public static Unit SearchNearestEnemy(ref Unit u)
        {
            Unit SearchNearestEnemyRet = default;
            short distance;
            short i, j;
            Unit t;
            distance = 1000;
            var loopTo = Map.MapWidth;
            for (i = 1; i <= loopTo; i++)
            {
                var loopTo1 = Map.MapHeight;
                for (j = 1; j <= loopTo1; j++)
                {
                    t = Map.MapDataForUnit[i, j];
                    if (t is null)
                    {
                        goto NexLoop;
                    }

                    // もっと近くにいる敵を発見済み？
                    if (distance <= (short)(Math.Abs((short)(u.x - t.x)) + Math.Abs((short)(u.y - t.y))))
                    {
                        goto NexLoop;
                    }

                    // 敵？
                    if (u.IsAlly(ref t))
                    {
                        goto NexLoop;
                    }

                    // 特定の陣営のみを狙う思考モードの場合
                    if (u.Mode == "味方" | u.Mode == "ＮＰＣ" | u.Mode == "敵" | u.Mode == "中立")
                    {
                        if ((t.Party ?? "") != (u.Mode ?? ""))
                        {
                            goto NexLoop;
                        }
                    }

                    // 目視不能？
                    string argsptype = "隠れ身";
                    if (t.IsUnderSpecialPowerEffect(ref argsptype) | t.Area == "地中")
                    {
                        goto NexLoop;
                    }

                    // ステルス状態にあれば遠くからは発見できない
                    string argfname = "ステルス";
                    object argIndex3 = "ステルス無効";
                    string argfname1 = "ステルス無効化";
                    if (t.IsFeatureAvailable(ref argfname) & !t.IsConditionSatisfied(ref argIndex3) & !u.IsFeatureAvailable(ref argfname1))
                    {
                        object argIndex2 = "ステルス";
                        if (t.IsFeatureLevelSpecified(ref argIndex2))
                        {
                            object argIndex1 = "ステルス";
                            if (Math.Abs((short)(u.x - t.x)) + Math.Abs((short)(u.y - t.y)) > t.FeatureLevel(ref argIndex1))
                            {
                                goto NexLoop;
                            }
                        }
                        else if (Math.Abs((short)(u.x - t.x)) + Math.Abs((short)(u.y - t.y)) > 3)
                        {
                            goto NexLoop;
                        }
                    }

                    // ターゲットを発見
                    SearchNearestEnemyRet = t;
                    distance = (short)(Math.Abs((short)(u.x - t.x)) + Math.Abs((short)(u.y - t.y)));
                    NexLoop:
                    ;
                }
            }

            return SearchNearestEnemyRet;
        }

        // 最も近い敵ユニットへの距離を返す
        private static short DistanceFromNearestEnemy(ref Unit u)
        {
            short DistanceFromNearestEnemyRet = default;
            Unit t;
            t = SearchNearestEnemy(ref u);
            if (t is object)
            {
                DistanceFromNearestEnemyRet = (short)(Math.Abs((short)(u.x - t.x)) + Math.Abs((short)(u.y - t.y)));
            }
            else
            {
                DistanceFromNearestEnemyRet = 1000;
            }

            return DistanceFromNearestEnemyRet;
        }
    }
}