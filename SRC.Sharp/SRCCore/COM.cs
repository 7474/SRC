// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using Newtonsoft.Json;
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SRCCore
{
    // コンピューターの思考ルーチン関連モジュール
    public class COM
    {
        protected SRC SRC { get; }
        private IGUI GUI => SRC.GUI;
        private Map Map => SRC.Map;
        private Commands.Command Commands => SRC.Commands;
        private Events.Event Event => SRC.Event;
        private Expressions.Expression Expression => SRC.Expression;
        private Sound Sound => SRC.Sound;
        private Effect Effect => SRC.Effect;
        private ISystemConfig SystemConfig => SRC.SystemConfig;

        public COM(SRC src)
        {
            SRC = src;
        }

        // コンピューターによるユニット操作(１行動)
        public void OperateUnit()
        {
            int j, i;
            int w = 0;
            int tw = 0;
            string wname, twname = default;
            Unit u;
            int tmp_w;
            int prob = default, dmg = default;
            int tprob = default, tdmg = default;
            int max_prob, max_dmg;
            int max_range, min_range;
            int dst_x = default, dst_y = default;
            int new_x = default, new_y = default;
            int new_x0 = default, new_y0 = default;
            int tx = default, ty = default;
            int new_locations_value;
            int distance;
            string def_mode;
            var is_suiside = default(bool);
            bool moved = default, transfered = default;
            string mmode;
            bool searched_enemy = default, searched_nearest_enemy = default;
            var guard_unit_mode = default(bool);
            string buf;
            int prev_money, earnings = default;
            string BGM;
            Unit attack_target;
            double attack_target_hp_ratio;
            Unit defense_target;
            double defense_target_hp_ratio;
            Unit defense_target2;
            var defense_target2_hp_ratio = default(double);
            var support_attack_done = default(bool);
            int w2;
            bool indirect_attack;
            bool is_p_weapon;
            var took_action = default(bool);

            SRC.LogDebug("Start", Commands.SelectedUnit.ID);

            Event.SelectedUnitForEvent = Commands.SelectedUnit;
            Commands.SelectedTarget = null;
            Event.SelectedTargetForEvent = null;
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
            if (Commands.SelectedUnit.IsConditionSatisfied("踊り"))
            {
                // 踊りに忙しい……
                return;
            }

            // スペシャルパワーを使う？
            if (Expression.IsOptionDefined("敵ユニットスペシャルパワー使用") || Expression.IsOptionDefined("敵ユニット精神コマンド使用"))
            {
                TrySpecialPower(Commands.SelectedUnit.MainPilot());
                if (SRC.IsScenarioFinished || SRC.IsCanceled)
                {
                    return;
                }

                foreach (var p in Commands.SelectedUnit.SubPilots.Concat(Commands.SelectedUnit.Supports))
                {
                    TrySpecialPower(p);
                    if (SRC.IsScenarioFinished || SRC.IsCanceled)
                    {
                        return;
                    }
                }

                if (Commands.SelectedUnit.IsFeatureAvailable("追加サポート"))
                {
                    TrySpecialPower(Commands.SelectedUnit.AdditionalSupport());
                    if (SRC.IsScenarioFinished || SRC.IsCanceled)
                    {
                        return;
                    }
                }
            }

            // ハイパーモードが可能であればハイパーモード発動
            TryHyperMode();

            // 特殊な思考モードの場合の処理
            {
                var selectedUnit = Commands.SelectedUnit;
                // 指定された地点を目指す場合
                int localLLength() { string arglist = selectedUnit.Mode; var ret = GeneralLib.LLength(arglist); selectedUnit.Mode = arglist; return ret; }

                if (localLLength() == 2)
                {
                    dst_x = Conversions.ToInteger(GeneralLib.LIndex(selectedUnit.Mode, 1));
                    dst_y = Conversions.ToInteger(GeneralLib.LIndex(selectedUnit.Mode, 2));
                    if (1 <= dst_x && dst_x <= Map.MapWidth && 1 <= dst_y && dst_y <= Map.MapHeight)
                    {
                        goto Move;
                    }
                }

                // 逃亡し続ける場合
                if (selectedUnit.Mode == "逃亡")
                {
                    goto Move;
                }

                // 思考モードが「パイロット名」の場合の処理
                if (!SRC.PList.IsDefined(selectedUnit.Mode))
                {
                    goto TryBattleTransform;
                }

                if (SRC.PList.Item(selectedUnit.Mode).Unit is null)
                {
                    goto TryBattleTransform;
                }

                if (SRC.PList.Item(selectedUnit.Mode).Unit.Status != "出撃")
                {
                    goto TryBattleTransform;
                }

                Commands.SelectedTarget = SRC.PList.Item(selectedUnit.Mode).Unit;
                Map.AreaInSpeed(Commands.SelectedUnit);
                if (!selectedUnit.IsAlly(Commands.SelectedTarget))
                {
                    // ユニットが敵の場合はそのユニットを狙う
                    w = SelectWeapon(Commands.SelectedUnit, Commands.SelectedTarget, "移動可能", out _, out _);
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
                    if (TryFix(moved, Commands.SelectedTarget))
                    {
                        goto EndOfOperation;
                    }

                    // 護衛対象が損傷している場合は回復アビリティを使う
                    if (TryHealing(moved, Commands.SelectedTarget))
                    {
                        goto EndOfOperation;
                    }

                    // 合体技や援護防御を持っている場合はとにかく護衛対象に
                    // 隣接することを優先する
                    if (selectedUnit.MainPilot().IsSkillAvailable("援護") || selectedUnit.MainPilot().IsSkillAvailable("援護防御"))
                    {
                        if (Math.Abs((selectedUnit.x - dst_x)) + Math.Abs((selectedUnit.y - dst_y)) > 1)
                        {
                            goto Move;
                        }

                        guard_unit_mode = true;
                    }

                    if (selectedUnit.IsFeatureAvailable("合体技"))
                    {
                        if (Math.Abs((selectedUnit.x - dst_x)) > 1 || Math.Abs((selectedUnit.y - dst_y)) > 1)
                        {
                            goto Move;
                        }

                        guard_unit_mode = true;
                    }

                    if (guard_unit_mode)
                    {
                        // ちゃんと隣接しているので周りの敵を排除
                        Commands.SelectedTarget = null;
                        goto TryBattleTransform;
                    }

                    // 護衛するユニットを脅かすユニットが存在するかチェック
                    foreach (Unit currentU in SRC.UList.Items)
                    {
                        selectedUnit = currentU;
                        {
                            var withBlock1 = selectedUnit;
                            if (withBlock1.Status == "出撃" && Commands.SelectedUnit.IsEnemy(selectedUnit) && Math.Abs((dst_x - withBlock1.x)) + Math.Abs((dst_y - withBlock1.y)) <= 5)
                            {
                                tmp_w = SelectWeapon(Commands.SelectedUnit, selectedUnit, "移動可能", out prob, out dmg);
                            }
                            else
                            {
                                tmp_w = 0;
                            }

                            if (tmp_w > 0)
                            {
                                // 脅威となり得るユニットと認定
                                if (distance > (Math.Abs((dst_x - withBlock1.x)) + Math.Abs((dst_y - withBlock1.y))))
                                {
                                    // 近い位置にいるユニットを優先
                                    Commands.SelectedTarget = selectedUnit;
                                    w = tmp_w;
                                    distance = (Math.Abs((dst_x - withBlock1.x)) + Math.Abs((dst_y - withBlock1.y)));
                                    max_prob = prob;
                                    max_dmg = dmg;
                                }
                                else if (distance == (Math.Abs((dst_x - withBlock1.x)) + Math.Abs((dst_y - withBlock1.y))))
                                {
                                    // 今までに見つかったユニットと位置が変わらなければ
                                    // より危険度が高いユニットを優先
                                    if (prob > max_prob && prob > 50)
                                    {
                                        Commands.SelectedTarget = selectedUnit;
                                        w = tmp_w;
                                        max_prob = prob;
                                    }
                                    else if (max_prob == 0 && dmg > max_dmg)
                                    {
                                        Commands.SelectedTarget = selectedUnit;
                                        w = tmp_w;
                                        max_dmg = dmg;
                                    }
                                }
                            }
                        }
                    }

                    if (w <= 0)
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
                    w = SelectWeapon(Commands.SelectedUnit, Commands.SelectedTarget, "移動可能", out _, out _);
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
            if (SRC.IsScenarioFinished || SRC.IsCanceled)
            {
                return;
            }

            {
                var withBlock2 = Commands.SelectedUnit;
                if (withBlock2.HP == 0 || withBlock2.MaxAction() == 0)
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
                if (SRC.IsScenarioFinished || SRC.IsCanceled)
                {
                    return;
                }

                goto EndOfOperation;
            }

            // 修理が可能であれば修理装置を使う
            if (TryFix(moved, t: null))
            {
                goto EndOfOperation;
            }

            // マップ型回復アビリティを使う？
            if (TryMapHealing(moved))
            {
                if (SRC.IsScenarioFinished || SRC.IsCanceled)
                {
                    return;
                }

                goto EndOfOperation;
            }

            // 回復アビリティを使う？
            if (TryHealing(moved, t: null))
            {
                if (SRC.IsScenarioFinished || SRC.IsCanceled)
                {
                    return;
                }

                goto EndOfOperation;
            }

        TryMapAttack:
            ;

            // マップ攻撃を使う？
            if (TryMapAttack(moved))
            {
                goto EndOfOperation;
            }

        SearchNearestEnemyWithinRange:
            ;

            // ターゲットにするユニットを探す
            {
                var selectedUnit = Commands.SelectedUnit;
                Map.AreaInSpeed(Commands.SelectedUnit);

                // 護衛すべきユニットがいる場合は移動範囲を限定
                if (guard_unit_mode)
                {
                    {
                        var guardTarget = SRC.PList.Item(selectedUnit.Mode).Unit;
                        for (i = 1; i <= Map.MapWidth; i++)
                        {
                            for (j = 1; j <= Map.MapHeight; j++)
                            {
                                if (!Map.MaskData[i, j])
                                {
                                    if (Math.Abs((guardTarget.x - i)) + Math.Abs((guardTarget.y - j)) > 1)
                                    {
                                        Map.MaskData[i, j] = true;
                                    }
                                }
                            }
                        }
                    }
                }

                // 個々のユニットに対してターゲットとなり得るか判定
                Commands.SelectedTarget = null;
                w = 0;
                max_prob = 0;
                max_dmg = 0;
                foreach (Unit currentU1 in SRC.UList.Items)
                {
                    u = currentU1;
                    if (u.Status != "出撃")
                    {
                        goto NextLoop;
                    }

                    // 敵かどうかを判定
                    if (selectedUnit.IsAlly(u))
                    {
                        goto NextLoop;
                    }

                    // 特定の陣営のみを狙う思考モードの場合
                    switch (selectedUnit.Mode ?? "")
                    {
                        case "味方":
                            {
                                if (u.Party != "味方" && u.Party != "ＮＰＣ")
                                {
                                    goto NextLoop;
                                }

                                break;
                            }

                        case "ＮＰＣ":
                        case "敵":
                        case "中立":
                            {
                                if ((u.Party ?? "") != (selectedUnit.Mode ?? ""))
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
                    if (u.IsUnderSpecialPowerEffect("隠れ身"))
                    {
                        goto NextLoop;
                    }

                    // ステルスの敵は遠距離からは攻撃を受けない
                    if (u.IsFeatureAvailable("ステルス") && !u.IsConditionSatisfied("ステルス無効") && !selectedUnit.IsFeatureAvailable("ステルス無効化"))
                    {
                        if (u.IsFeatureLevelSpecified("ステルス"))
                        {
                            if (Math.Abs((selectedUnit.x - u.x)) + Math.Abs((selectedUnit.y - u.y)) > u.FeatureLevel("ステルス"))
                            {
                                goto NextLoop;
                            }
                        }
                        else if (Math.Abs((selectedUnit.x - u.x)) + Math.Abs((selectedUnit.y - u.y)) > 3)
                        {
                            goto NextLoop;
                        }
                    }

                    // 攻撃に使う武器を選択
                    if (moved)
                    {
                        tmp_w = SelectWeapon(Commands.SelectedUnit, u, "移動後", out prob, out dmg);
                    }
                    else
                    {
                        tmp_w = SelectWeapon(Commands.SelectedUnit, u, "移動可能", out prob, out dmg);
                    }

                    if (tmp_w <= 0)
                    {
                        goto NextLoop;
                    }

                    // サポートガードされる？
                    if (selectedUnit.MainPilot().TacticalTechnique() >= 150)
                    {
                        if (u.LookForSupportGuard(Commands.SelectedUnit, Commands.SelectedUnit.Weapon(tmp_w)) != null)
                        {
                            // 相手を破壊することは出来ない
                            prob = 0;
                            // 仮想的にダメージを半減して判定
                            dmg = dmg / 2;
                        }
                    }

                    // 間接攻撃？
                    indirect_attack = selectedUnit.Weapon(tmp_w).IsWeaponClassifiedAs("間");
                    // w は常に0？
                    //indirect_attack = selectedUnit.IsWeaponClassifiedAs(w, "間");

                    // 召喚ユニットは自分がやられてしまうような攻撃はかけない
                    if (selectedUnit.Party == "ＮＰＣ"
                        && selectedUnit.IsFeatureAvailable("召喚ユニット")
                        && !selectedUnit.IsConditionSatisfied("暴走")
                        && !selectedUnit.IsConditionSatisfied("混乱")
                        && !selectedUnit.IsConditionSatisfied("狂戦士")
                        && !indirect_attack)
                    {
                        tw = SelectWeapon(u, Commands.SelectedUnit, "反撃", out tprob, out tdmg);
                        if (prob < 80 && tprob > prob)
                        {
                            goto NextLoop;
                        }
                    }

                    // 破壊確率が50%以上であれば破壊確率が高いユニットを優先
                    // そうでなければダメージの期待値が高いユニットを優先
                    if (prob > 50)
                    {
                        // 重要なユニットは優先してターゲットにする
                        if (selectedUnit.MainPilot().TacticalTechnique() >= 150)
                        {
                            if (u.MainPilot().IsSkillAvailable("指揮")
                                || u.MainPilot().IsSkillAvailable("広域サポート")
                                || u.IsFeatureAvailable("修理装置"))
                            {
                                prob = (int)(1.5d * prob);
                            }
                            else
                            {
                                // 回復アビリティを持っている？
                                foreach (var adata in u.Abilities)
                                {
                                    if (adata.Data.MaxRange > 0)
                                    {
                                        if (adata.Data.Effects.Any(x => x.EffectType == "回復"))
                                        {
                                            prob = (int)(1.5d * prob);
                                            break;
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
                            var uw = u.Weapon(i);
                            if (uw.IsWeaponAvailable("移動前") && !uw.IsWeaponClassifiedAs("Ｍ"))
                            {
                                if (!moved
                                    && selectedUnit.Mode != "固定"
                                    && selectedUnit.Weapon(tmp_w).IsWeaponClassifiedAs("移動後攻撃可"))
                                {
                                    if (uw.WeaponMaxRange() >= selectedUnit.Weapon(tmp_w).WeaponMaxRange())
                                    {
                                        tw = i;
                                        break;
                                    }
                                }
                                else if (uw.WeaponMaxRange() >= (Math.Abs((selectedUnit.x - u.x)) + Math.Abs((selectedUnit.y - u.y))))
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
                        if (u.MaxAction() == 0 || u.IsConditionSatisfied("攻撃不能"))
                        {
                            tw = 0;
                        }

                        // 反撃してこない？
                        if (tw == 0)
                        {
                            dmg = (int)(1.5d * dmg);
                        }

                        // 重要なユニットは優先してターゲットにする
                        if (selectedUnit.MainPilot().TacticalTechnique() >= 150)
                        {
                            // メインパイロットが指揮や広域サポートを有していたり
                            // 修理装置を持っていれば重要ユニットと認定
                            if (u.MainPilot().IsSkillAvailable("指揮")
                                || u.MainPilot().IsSkillAvailable("広域サポート")
                                || u.IsFeatureAvailable("修理装置"))
                            {
                                dmg = (int)(1.5d * dmg);
                            }
                            else
                            {
                                // 回復アビリティを持っている場合も重要ユニットと認定
                                foreach (var adata in u.Abilities)
                                {
                                    if (adata.Data.MaxRange > 0)
                                    {
                                        if (adata.Data.Effects.Any(x => x.EffectType == "回復"))
                                        {
                                            dmg = (int)(1.5d * dmg);
                                            break;
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
                    if (selectedUnit.Mode == "待機" || selectedUnit.Mode == "固定" || GeneralLib.LLength(selectedUnit.Mode) == 2)
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
            // TODO 武器選択に失敗してるケースがある
            if (w <= 0)
            {
                SRC.LogWarn("Weapon not found. " + JsonConvert.SerializeObject(Commands.SelectedUnit), null);
                goto EndOfOperation;
            }

            // 敵をUpdate
            Commands.SelectedTarget.Update();

            // 敵の位置を記録しておく
            tx = Commands.SelectedTarget.x;
            ty = Commands.SelectedTarget.y;
            //string[] list;
            string caption_msg;
            int hit_prob, crit_prob;
            IList<Unit> partners;
            {
                var selectedUnit = Commands.SelectedUnit;
                var selectedWeapon = selectedUnit.Weapon(w);
                //  
                // ただし合体技は移動後の位置によって攻撃できない場合があるので例外
                if (selectedWeapon.IsWeaponClassifiedAs("移動後攻撃可")
                    && !selectedWeapon.IsWeaponClassifiedAs("合")
                    && !moved
                    && selectedUnit.Mode != "固定")
                {
                    // 移動しなくても攻撃出来る場合は現在位置をデフォルトの攻撃位置に設定
                    if (selectedWeapon.IsTargetWithinRange(Commands.SelectedTarget))
                    {
                        var td = Map.Terrain(selectedUnit.x, selectedUnit.y);
                        new_locations_value = (td.EffectForHPRecover() + td.EffectForENRecover() + 100 * selectedUnit.LookForSupport(selectedUnit.x, selectedUnit.y, true));
                        if (selectedUnit.Area != "空中")
                        {
                            // 地形による防御効果は空中にいる場合は受けられない
                            new_locations_value = new_locations_value + td.HitMod + td.DamageMod;
                        }

                        new_x = selectedUnit.x;
                        new_y = selectedUnit.y;
                    }
                    else
                    {
                        new_locations_value = -1000;
                        new_x = 0;
                        new_y = 0;
                    }

                    // 攻撃をかけられる位置のうち、もっとも地形効果の高い場所を探す
                    // 地形効果が同等ならもっとも近い場所を優先
                    max_range = selectedWeapon.WeaponMaxRange();
                    min_range = selectedWeapon.WeaponData.MinRange;
                    var loopTo7 = GeneralLib.MinLng(tx + max_range, Map.MapWidth);
                    for (i = GeneralLib.MaxLng(1, tx - max_range); i <= loopTo7; i++)
                    {
                        var loopTo8 = GeneralLib.MinLng(ty + (max_range - Math.Abs((tx - i))), Map.MapHeight);
                        for (j = GeneralLib.MaxLng(1, ty - (max_range - Math.Abs((tx - i)))); j <= loopTo8; j++)
                        {
                            if (!Map.MaskData[i, j] && Map.MapDataForUnit[i, j] is null && (Math.Abs((tx - i)) + Math.Abs((ty - j))) >= min_range)
                            {
                                var td = Map.Terrain(i, j);
                                var tmp = (td.EffectForHPRecover() + td.EffectForENRecover() + 100 * selectedUnit.LookForSupport(i, j, true));
                                if (selectedUnit.Area != "空中")
                                {
                                    // 地形による防御効果は空中にいる場合は受けられない
                                    tmp = tmp + td.HitMod + td.DamageMod;
                                    // 水中は水中用ユニットでない限り選択しない
                                    if (Map.Terrain(i, j).Class == "水")
                                    {
                                        if (selectedUnit.IsTransAvailable("水"))
                                        {
                                            tmp = (tmp + 100);
                                        }
                                        else
                                        {
                                            tmp = -1000;
                                        }
                                    }
                                }

                                // 条件が同じであれば直線距離で近い場所を選択する
                                tmp = ((int)(tmp - Math.Sqrt(Math.Pow(Math.Abs((selectedUnit.x - i)), 2d) + Math.Pow(Math.Abs((selectedUnit.y - j)), 2d))));
                                if (new_locations_value < tmp)
                                {
                                    new_locations_value = tmp;
                                    new_x = i;
                                    new_y = j;
                                }
                            }
                        }
                    }

                    if (new_x == 0 && new_y == 0)
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
                    if (new_x != selectedUnit.x || new_y != selectedUnit.y)
                    {
                        selectedUnit.Move(new_x, new_y);
                        Commands.SelectedUnitMoveCost = Map.TotalMoveCost[new_x, new_y];
                        moved = true;

                        // 移動のためＥＮ切れ？
                        if (selectedUnit.EN == 0)
                        {
                            if (selectedUnit.MaxAction() == 0)
                            {
                                goto EndOfOperation;
                            }
                        }

                        // 実はマップ攻撃が使える？
                        if (TryMapAttack(true))
                        {
                            goto EndOfOperation;
                        }

                        // 移動のために選択していた武器が使えなくなったり、合体技が使える
                        // ようになったりすることがあるので、武器を再度選択
                        w = SelectWeapon(Commands.SelectedUnit, Commands.SelectedTarget, "移動後", out _, out _);
                        selectedWeapon = selectedUnit.Weapon(w);
                        if (w == 0)
                        {
                            // 攻撃出来ないので行動終了
                            goto EndOfOperation;
                        }
                    }
                }

                // ユニットを中央表示
                GUI.Center(selectedUnit.x, selectedUnit.y);

                // ハイライト表示を行う
                if (!SRC.BattleAnimation)
                {
                    // 射程範囲をハイライト
                    Map.AreaInRange(selectedUnit.x, selectedUnit.y, selectedWeapon.WeaponMaxRange(), selectedWeapon.WeaponData.MinRange, "空間");
                }
                // 合体技の場合はパートナーもハイライト表示
                if (selectedWeapon.IsWeaponClassifiedAs("合"))
                {
                    if (selectedWeapon.WeaponMaxRange() == 1)
                    {
                        partners = selectedWeapon.CombinationPartner(Commands.SelectedTarget.x, Commands.SelectedTarget.y);
                    }
                    else
                    {
                        partners = selectedWeapon.CombinationPartner();
                    }

                    if (!SRC.BattleAnimation)
                    {
                        foreach (var pu in partners)
                        {
                            Map.MaskData[pu.x, pu.y] = false;
                        }
                    }
                }
                else
                {
                    Commands.SelectedPartners.Clear();
                    partners = new List<Unit>();
                }

                if (!SRC.BattleAnimation)
                {
                    // 自分自身とターゲットもハイライト
                    Map.MaskData[selectedUnit.x, selectedUnit.y] = false;
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
                    if (selectedUnit.IsFeatureAvailable("ＢＧＭ") && Strings.InStr(selectedUnit.MainPilot().Name, "(ザコ)") == 0)
                    {
                        BGM = Sound.SearchMidiFile(selectedUnit.FeatureData("ＢＧＭ"));
                    }

                    Sound.BossBGM = false;
                    if (Strings.Len(BGM) > 0)
                    {
                        // ボス用ＢＧＭを演奏する場合
                        Sound.ChangeBGM(BGM);
                        Sound.BossBGM = true;
                    }
                    else
                    {
                        // 通常の戦闘ＢＧＭ

                        // ターゲットは味方？
                        if (Commands.SelectedTarget.Party == "味方" || Commands.SelectedTarget.Party == "ＮＰＣ" && selectedUnit.Party != "ＮＰＣ")
                        {
                            // ターゲットが味方なのでターゲット側を優先
                            if (Commands.SelectedTarget.IsFeatureAvailable("ＢＧＭ"))
                            {
                                BGM = Sound.SearchMidiFile(Commands.SelectedTarget.FeatureData("ＢＧＭ"));
                            }

                            if (Strings.Len(BGM) == 0)
                            {
                                BGM = Sound.SearchMidiFile(Commands.SelectedTarget.MainPilot().BGM);
                            }
                        }
                        else
                        {
                            // ターゲットが味方でなければ攻撃側を優先
                            if (selectedUnit.IsFeatureAvailable("ＢＧＭ"))
                            {
                                BGM = Sound.SearchMidiFile(selectedUnit.FeatureData("ＢＧＭ"));
                            }

                            if (Strings.Len(BGM) == 0)
                            {
                                BGM = Sound.SearchMidiFile(selectedUnit.MainPilot().BGM);
                            }
                        }

                        if (Strings.Len(BGM) == 0)
                        {
                            BGM = Sound.BGMName("default");
                        }

                        // ＢＧＭを変更
                        Sound.ChangeBGM(BGM);
                    }
                }

                // 移動後攻撃可能？
                is_p_weapon = selectedWeapon.IsWeaponClassifiedAs("移動後攻撃可");

                // 間接攻撃？
                indirect_attack = selectedWeapon.IsWeaponClassifiedAs("間");

                // 相手の反撃手段を設定
                def_mode = "";
                Commands.UseSupportGuard = true;
                if (Commands.SelectedTarget.MaxAction() == 0)
                {
                    // 行動不能の場合

                    tw = -1;
                    // チャージ中または消耗している場合は自動的に防御
                    if (Commands.SelectedTarget.Party == "味方"
                        && (Commands.SelectedTarget.IsFeatureAvailable("チャージ") || Commands.SelectedTarget.IsFeatureAvailable("消耗")))
                    {
                        def_mode = "防御";
                    }
                }
                else if (Commands.SelectedTarget.Party == "味方" && !SystemConfig.AutoDefense)
                {
                    // 味方ユニットによる手動反撃を行う場合

                    // 戦闘アニメを表示する場合でも手動反撃時にはハイライト表示を行う
                    if (SRC.BattleAnimation)
                    {
                        // 射程範囲をハイライト
                        Map.AreaInRange(selectedUnit.x, selectedUnit.y, selectedWeapon.WeaponMaxRange(), selectedWeapon.WeaponData.MinRange, "空間");

                        // 合体技の場合はパートナーもハイライト表示
                        if (selectedWeapon.IsWeaponClassifiedAs("合"))
                        {
                            foreach (var pu in partners)
                            {
                                Map.MaskData[pu.x, pu.y] = false;
                            }
                        }

                        // 自分自身とターゲットもハイライト
                        Map.MaskData[selectedUnit.x, selectedUnit.y] = false;
                        Map.MaskData[Commands.SelectedTarget.x, Commands.SelectedTarget.y] = false;

                        // ハイライト表示を実施
                        GUI.MaskScreen();
                    }

                    hit_prob = selectedWeapon.HitProbability(Commands.SelectedTarget, true);
                    crit_prob = selectedWeapon.CriticalProbability(Commands.SelectedTarget);
                    caption_msg = "反撃：" + selectedWeapon.WeaponNickname() + " 攻撃力=" + SrcFormatter.Format(selectedWeapon.WeaponPower(""));
                    if (!Expression.IsOptionDefined("予測命中率非表示"))
                    {
                        caption_msg = caption_msg + " 命中率=" + SrcFormatter.Format(GeneralLib.MinLng(hit_prob, 100)) + "％（" + crit_prob + "％）";
                    }

                    // 援護防御が受けられる？
                    Commands.SupportGuardUnit = Commands.SelectedTarget.LookForSupportGuard(Commands.SelectedUnit, Commands.SelectedUnit.Weapon(w));
                    var canUseSupportGuard = false;
                    if (Commands.SupportGuardUnit != null)
                    {
                        canUseSupportGuard = true;
                        Commands.UseSupportGuard = true;
                    }

                    GUI.AddPartsToListBox();
                    do
                    {
                        // 攻撃への対応手段を選択
                        var tu = Commands.SelectedTarget;

                        var confirmList = new List<ListBoxItem>();
                        if (IsAbleToCounterAttack(Commands.SelectedTarget, Commands.SelectedUnit) && !indirect_attack)
                        {
                            confirmList.Add(new ListBoxItem("反撃", "反撃"));
                            tw = -1;
                        }
                        else
                        {
                            confirmList.Add(new ListBoxItem("反撃不能", "反撃不能")
                            {
                                ListItemFlag = true,
                            });
                            tw = 0;
                        }

                        if (!Expression.IsOptionDefined("予測命中率非表示"))
                        {
                            confirmList.Add(new ListBoxItem(
                                "防御：命中率＝" + SrcFormatter.Format(GeneralLib.MinLng(hit_prob, 100)) + "％（" + selectedWeapon.CriticalProbability(Commands.SelectedTarget, "防御") + "％）",
                                "防御")
                            {
                                ListItemFlag = tu.IsFeatureAvailable("防御不可"),
                            });
                            confirmList.Add(new ListBoxItem(
                                "回避：命中率＝" + SrcFormatter.Format(GeneralLib.MinLng(hit_prob / 2, 100)) + "％（" + selectedWeapon.CriticalProbability(Commands.SelectedTarget, "回避") + "％）",
                                "回避")
                            {
                                ListItemFlag = tu.IsFeatureAvailable("回避不可") || tu.IsConditionSatisfied("移動不能"),
                            });
                        }
                        else
                        {
                            confirmList.Add(new ListBoxItem("防御", "防御")
                            {
                                ListItemFlag = tu.IsFeatureAvailable("防御不可"),
                            });
                            confirmList.Add(new ListBoxItem("回避", "回避")
                            {
                                ListItemFlag = tu.IsFeatureAvailable("回避不可") || tu.IsConditionSatisfied("移動不能"),
                            });
                        }

                        if (canUseSupportGuard)
                        {
                            if (Expression.IsOptionDefined("等身大基準"))
                            {
                                confirmList.Add(new ListBoxItem(
                                    $"援護防御：使用{(Commands.UseSupportGuard ? "する" : "しない")}("
                                        + Commands.SupportGuardUnit.Nickname + ")",
                                    "援護防御"));
                            }
                            else
                            {
                                confirmList.Add(new ListBoxItem(
                                    $"援護防御：使用{(Commands.UseSupportGuard ? "する" : "しない")} ("
                                        + Commands.SupportGuardUnit.Nickname + "/" + Commands.SupportGuardUnit.MainPilot().get_Nickname(false) + ")",
                                    "援護防御"));
                            }
                        }

                        // 対応手段を選択
                        GUI.TopItem = 1;
                        i = GUI.ListBox(new ListBoxArgs
                        {
                            HasFlag = true,
                            Items = confirmList,
                            lb_caption = caption_msg,
                            lb_info = tu.Nickname0 + " " + tu.MainPilot().get_Nickname(false),
                            lb_mode = "連続表示,カーソル移動",
                        });
                        var selectedItem = confirmList.SafeRefZeroOffset(i - 1);
                        if (selectedItem != null)
                        {
                            switch (selectedItem.ListItemID)
                            {
                                case "反撃":
                                    {
                                        // 反撃を選択した場合は反撃に使う武器を選択
                                        buf = "反撃：" + selectedUnit.Weapon(w).WeaponNickname() + " 攻撃力=" + SrcFormatter.Format(selectedUnit.Weapon(w).WeaponPower(""));
                                        if (!Expression.IsOptionDefined("予測命中率非表示"))
                                        {
                                            buf = buf + " 命中率=" + SrcFormatter.Format(GeneralLib.MinLng(hit_prob, 100)) + "％（" + crit_prob + "％）" + " ： ";
                                        }

                                        {
                                            var withBlock11 = Commands.SelectedTarget.MainPilot();
                                            buf = buf + withBlock11.get_Nickname(false) + " " + Expression.Term("格闘", Commands.SelectedTarget) + SrcFormatter.Format(withBlock11.Infight) + " ";
                                            if (withBlock11.HasMana())
                                            {
                                                buf = buf + Expression.Term("魔力", Commands.SelectedTarget) + SrcFormatter.Format(withBlock11.Shooting);
                                            }
                                            else
                                            {
                                                buf = buf + Expression.Term("射撃", Commands.SelectedTarget) + SrcFormatter.Format(withBlock11.Shooting);
                                            }
                                        }

                                        tw = GUI.WeaponListBox(Commands.SelectedTarget,
                                            new Units.UnitWeaponList(Units.WeaponListMode.Counter, Commands.SelectedTarget, Commands.SelectedUnit),
                                            buf, "反撃", BGM: "")?.WeaponNo() ?? 0;
                                        if (tw == 0)
                                        {
                                            i = 0;
                                        }

                                        break;
                                    }

                                case "防御":
                                    {
                                        // 防御を選択した
                                        def_mode = "防御";
                                        break;
                                    }

                                case "回避":
                                    {
                                        // 回避を選択した
                                        def_mode = "回避";
                                        break;
                                    }

                                case "援護防御":
                                    {
                                        // 援護防御を使用するかどうかを切り替えた
                                        Commands.UseSupportGuard = !Commands.UseSupportGuard;
                                        i = 0;
                                        break;
                                    }

                                default:
                                    {
                                        // 反撃・防御・回避の全てが選択出来ない？
                                        if (confirmList.Take(3).All(x => x.ListItemFlag))
                                        {
                                            // 選択ループを抜ける
                                            i = -1;
                                        }

                                        break;
                                    }
                            }
                        }
                    }
                    while (i == 0);

                    // 反撃手段選択終了
                    GUI.CloseListBox();
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
                    tw = SelectWeapon(Commands.SelectedTarget, Commands.SelectedUnit, "反撃", out _, out _);
                    if (indirect_attack)
                    {
                        tw = 0;
                    }

                    // 防御を選択する？
                    def_mode = Conversions.ToString(SelectDefense(Commands.SelectedUnit, w, Commands.SelectedTarget, tw));
                    if (!string.IsNullOrEmpty(def_mode))
                    {
                        tw = -1;
                    }
                }
            }

            // 味方ユニットの場合は武器用ＢＧＭを演奏する
            {
                if (!SRC.KeepEnemyBGM)
                {
                    var selectedTarget = Commands.SelectedTarget;
                    if (selectedTarget.Party == "味方" && tw > 0 && selectedTarget.IsFeatureAvailable("武器ＢＧＭ"))
                    {
                        var loopTo11 = selectedTarget.CountFeature();
                        for (i = 1; i <= loopTo11; i++)
                        {
                            var fdata = selectedTarget.Feature(i).Data;
                            if (selectedTarget.Feature(i).Name == "武器ＢＧＭ" && (GeneralLib.LIndex(fdata, 1) ?? "") == (selectedTarget.Weapon(tw).Name ?? ""))
                            {
                                // 武器用ＢＧＭが指定されていた
                                BGM = Sound.SearchMidiFile(Strings.Mid(fdata, Strings.InStr(fdata, " ") + 1));
                                if (Strings.Len(BGM) > 0)
                                {
                                    // 武器用ＢＧＭのMIDIが見つかったのでＢＧＭを変更
                                    Sound.BossBGM = false;
                                    Sound.ChangeBGM(BGM);
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

            // 戦闘前に一旦クリア
            Commands.SupportAttackUnit = null;
            Commands.SupportGuardUnit = null;
            Commands.SupportGuardUnit2 = null;

            // 武器の使用イベント
            Event.HandleEvent("使用", Commands.SelectedUnit.MainPilot().ID, wname);
            if (SRC.IsScenarioFinished || SRC.IsCanceled)
            {
                return;
            }

            if (tw > 0)
            {
                twname = Commands.SelectedTarget.Weapon(tw).Name;
                Commands.SaveSelections();
                Commands.SwapSelections();
                Event.HandleEvent("使用", Commands.SelectedUnit.MainPilot().ID, twname);
                Commands.RestoreSelections();
                if (SRC.IsScenarioFinished || SRC.IsCanceled)
                {
                    return;
                }
            }

            // 攻撃イベント
            Event.HandleEvent("攻撃", Commands.SelectedUnit.MainPilot().ID, Commands.SelectedTarget.MainPilot().ID);
            if (SRC.IsScenarioFinished || SRC.IsCanceled)
            {
                return;
            }

            // メッセージウィンドウを開く
            if (SRC.Stage == "ＮＰＣ")
            {
                GUI.OpenMessageForm(Commands.SelectedTarget, Commands.SelectedUnit);
            }
            else
            {
                GUI.OpenMessageForm(Commands.SelectedUnit, Commands.SelectedTarget);
            }

            // イベント用に戦闘に参加するユニットの情報を記録しておく
            Commands.AttackUnit = Commands.SelectedUnit;
            attack_target = Commands.SelectedUnit;
            attack_target_hp_ratio = Commands.SelectedUnit.HP / (double)Commands.SelectedUnit.MaxHP;
            defense_target = Commands.SelectedTarget;
            defense_target_hp_ratio = Commands.SelectedTarget.HP / (double)Commands.SelectedTarget.MaxHP;
            defense_target2 = null;

            // 相手の先制攻撃？
            {
                var targetUnit = Commands.SelectedTarget;
                var targetWeapon = targetUnit.Weapon(tw);
                if (targetUnit.MaxAction() > 0
                    && targetWeapon != null
                    && targetWeapon.IsWeaponAvailable("移動前"))
                {
                    if (!targetWeapon.IsWeaponClassifiedAs("後"))
                    {
                        var selectedWeapon = Commands.SelectedUnit.Weapon(w);
                        if (selectedWeapon.IsWeaponClassifiedAs("後"))
                        {
                            def_mode = "先制攻撃";
                            targetUnit.Attack(targetWeapon, Commands.SelectedUnit, "先制攻撃", "");
                            Commands.SelectedTarget = targetUnit.CurrentForm();
                        }
                        else if (targetWeapon.IsWeaponClassifiedAs("先")
                            || targetUnit.MainPilot().SkillLevel("先読み", ref_mode: "") >= GeneralLib.Dice(16)
                            || targetUnit.IsUnderSpecialPowerEffect("カウンター"))
                        {
                            def_mode = "先制攻撃";
                            targetUnit.Attack(targetWeapon, Commands.SelectedUnit, "カウンター", "");
                            Commands.SelectedTarget = targetUnit.CurrentForm();
                        }
                        else if (targetUnit.MaxCounterAttack() > targetUnit.UsedCounterAttack)
                        {
                            def_mode = "先制攻撃";
                            targetUnit.UsedCounterAttack = (targetUnit.UsedCounterAttack + 1);
                            targetUnit.Attack(targetWeapon, Commands.SelectedUnit, "カウンター", "");
                            Commands.SelectedTarget = targetUnit.CurrentForm();
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
                var currentUnit = Commands.SelectedUnit;
                if (currentUnit.Status == "出撃" && Commands.SelectedTarget.Status == "出撃")
                {
                    Commands.SupportAttackUnit = currentUnit.LookForSupportAttack(Commands.SelectedTarget);

                    // 合体技ではサポートアタック不能
                    if (0 < Commands.SelectedWeapon && Commands.SelectedWeapon <= currentUnit.CountWeapon())
                    {
                        if (currentUnit.Weapon(Commands.SelectedWeapon).IsWeaponClassifiedAs("合"))
                        {
                            Commands.SupportAttackUnit = null;
                        }
                    }

                    // 魅了された場合
                    if (currentUnit.IsConditionSatisfied("魅了") && ReferenceEquals(currentUnit.Master, Commands.SelectedTarget))
                    {
                        Commands.SupportAttackUnit = null;
                    }

                    // 憑依された場合
                    if (currentUnit.IsConditionSatisfied("憑依"))
                    {
                        if ((currentUnit.Master.Party ?? "") == (Commands.SelectedTarget.Party ?? ""))
                        {
                            Commands.SupportAttackUnit = null;
                        }
                    }

                    // 踊らされた場合
                    if (currentUnit.IsConditionSatisfied("踊り"))
                    {
                        Commands.SupportAttackUnit = null;
                    }
                }
            }

            // 攻撃の実施
            {
                var selectedUnit = Commands.SelectedUnit;
                var selectedWeapon = selectedUnit.Weapon(w);
                if (selectedUnit.Status == "出撃"
                    && selectedUnit.MaxAction(true) > 0
                    && !selectedUnit.IsConditionSatisfied("攻撃不能")
                    && Commands.SelectedTarget.Status == "出撃")
                {
                    // まだ武器は使用可能か？
                    // XXX これインデックスずれてたらどうすんの？
                    // TODO 名前で解決？
                    if (w > selectedUnit.CountWeapon())
                    {
                        w = -1;
                    }
                    else if ((wname ?? "") != (selectedUnit.Weapon(w).Name ?? ""))
                    {
                        w = -1;
                    }
                    else if (moved)
                    {
                        if (!selectedUnit.Weapon(w).IsWeaponAvailable("移動後"))
                        {
                            w = -1;
                        }
                    }
                    else
                    {
                        if (!selectedUnit.Weapon(w).IsWeaponAvailable("移動前"))
                        {
                            w = -1;
                        }
                    }

                    if (w > 0)
                    {
                        if (!selectedUnit.Weapon(w).IsTargetWithinRange(Commands.SelectedTarget))
                        {
                            w = 0;
                        }
                    }

                    // 行動不能な場合
                    if (selectedUnit.MaxAction(true) == 0)
                    {
                        w = -1;
                    }

                    // 魅了された場合
                    if (selectedUnit.IsConditionSatisfied("魅了") && ReferenceEquals(selectedUnit.Master, Commands.SelectedTarget))
                    {
                        w = -1;
                    }

                    // 憑依された場合
                    if (selectedUnit.IsConditionSatisfied("憑依"))
                    {
                        if ((selectedUnit.Master.Party ?? "") == (Commands.SelectedTarget.Party ?? ""))
                        {
                            w = -1;
                        }
                    }

                    // 踊らされた場合
                    if (selectedUnit.IsConditionSatisfied("踊り"))
                    {
                        w = -1;
                    }

                    if (w > 0)
                    {
                        // 自爆攻撃？
                        if (selectedWeapon.IsWeaponClassifiedAs("自"))
                        {
                            is_suiside = true;
                        }

                        if (Commands.SupportAttackUnit is object && selectedUnit.MaxSyncAttack() > selectedUnit.UsedSyncAttack)
                        {
                            // 同時援護攻撃
                            selectedUnit.Attack(selectedWeapon, Commands.SelectedTarget, "統率", def_mode);
                        }
                        else
                        {
                            // 通常攻撃
                            selectedUnit.Attack(selectedWeapon, Commands.SelectedTarget, "", def_mode);
                        }
                    }
                    else if (w == 0)
                    {
                        // 射程外
                        if (selectedUnit.IsAnimationDefined("射程外", sub_situation: ""))
                        {
                            selectedUnit.PlayAnimation("射程外", sub_situation: "");
                        }
                        else
                        {
                            selectedUnit.SpecialEffect("射程外", sub_situation: "");
                        }
                        selectedUnit.PilotMessage("射程外", msg_mode: "");
                    }
                }
                else
                {
                    w = -1;
                }

                Commands.SelectedUnit = selectedUnit.CurrentForm();

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
                if (Commands.SupportAttackUnit.Status != "出撃" || Commands.SelectedUnit.Status != "出撃" || Commands.SelectedTarget.Status != "出撃")
                {
                    Commands.SupportAttackUnit = null;
                }
            }

            if (Commands.SupportAttackUnit is object)
            {
                if (Commands.SelectedUnit.MaxSyncAttack() > Commands.SelectedUnit.UsedSyncAttack)
                {
                    {
                        var sau = Commands.SupportAttackUnit;
                        // サポートアタックに使う武器を決定
                        w2 = SelectWeapon(Commands.SupportAttackUnit, Commands.SelectedTarget, "サポートアタック", out _, out _);
                        if (w2 > 0)
                        {
                            // サポートアタックを実施
                            Map.MaskData[sau.x, sau.y] = false;
                            if (!SRC.BattleAnimation)
                            {
                                GUI.MaskScreen();
                            }

                            if (sau.IsAnimationDefined("サポートアタック開始", sub_situation: ""))
                            {
                                sau.PlayAnimation("サポートアタック開始", sub_situation: "");
                            }

                            GUI.UpdateMessageForm(Commands.SelectedTarget, Commands.SupportAttackUnit);
                            sau.Attack(sau.Weapon(w2), Commands.SelectedTarget, "同時援護攻撃", def_mode);
                        }
                    }

                    // 後始末
                    {
                        var safcf = Commands.SupportAttackUnit.CurrentForm();
                        if (w2 > 0)
                        {
                            if (safcf.IsAnimationDefined("サポートアタック終了", sub_situation: ""))
                            {
                                safcf.PlayAnimation("サポートアタック終了", sub_situation: "");
                            }

                            // サポートアタックの残り回数を減らす
                            safcf.UsedSupportAttack = (safcf.UsedSupportAttack + 1);

                            // 同時援護攻撃の残り回数を減らす
                            Commands.SelectedUnit.UsedSyncAttack = (Commands.SelectedUnit.UsedSyncAttack + 1);
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
                var targetUnit = Commands.SelectedTarget;
                var targetWeapon = targetUnit.Weapon(tw);
                // 反撃の実行
                if (def_mode != "先制攻撃")
                {
                    if (targetUnit.Status == "出撃" && Commands.SelectedUnit.Status == "出撃")
                    {
                        // まだ武器は使用可能か？
                        if (tw > 0)
                        {
                            if (tw > targetUnit.CountWeapon())
                            {
                                tw = -1;
                            }
                            else if ((twname ?? "") != (targetUnit.Weapon(tw).Name ?? "")
                                || !targetUnit.Weapon(tw).IsWeaponAvailable("移動前"))
                            {
                                tw = -1;
                            }
                        }

                        if (tw > 0)
                        {
                            if (!targetUnit.Weapon(tw).IsTargetWithinRange(Commands.SelectedUnit))
                            {
                                // 敵が射程外に逃げていたら武器を再選択
                                tw = 0;
                            }
                        }

                        // 行動不能な場合
                        if (targetUnit.MaxAction() == 0)
                        {
                            tw = -1;
                        }

                        // 魅了された場合
                        if (targetUnit.IsConditionSatisfied("魅了") && ReferenceEquals(targetUnit.Master, Commands.SelectedUnit))
                        {
                            tw = -1;
                        }

                        // 憑依された場合
                        if (targetUnit.IsConditionSatisfied("憑依"))
                        {
                            if ((targetUnit.Master.Party ?? "") == (Commands.SelectedUnit.Party ?? ""))
                            {
                                tw = -1;
                            }
                        }

                        // 踊らされた場合
                        if (targetUnit.IsConditionSatisfied("踊り"))
                        {
                            tw = -1;
                        }

                        if (tw > 0 && string.IsNullOrEmpty(def_mode))
                        {
                            // 反撃を実施
                            targetUnit.Attack(targetWeapon, Commands.SelectedUnit, "", "");
                            if (targetUnit.Status == "他形態")
                            {
                                Commands.SelectedTarget = targetUnit.CurrentForm();
                            }

                            if (Commands.SelectedUnit.Status == "他形態")
                            {
                                Commands.SelectedUnit = Commands.SelectedUnit.CurrentForm();
                            }

                            // 攻撃側のユニットがかばわれた場合は攻撃側のターゲットを再設定
                            if (Commands.SupportGuardUnit2 is object)
                            {
                                attack_target = Commands.SupportGuardUnit2;
                                attack_target_hp_ratio = Commands.SupportGuardUnitHPRatio2;
                            }
                        }
                        else if (tw == 0 && targetUnit.x == tx && targetUnit.y == ty)
                        {
                            // 反撃出来る武器がなかった場合は射程外メッセージを表示
                            if (targetUnit.IsAnimationDefined("射程外", sub_situation: ""))
                            {
                                targetUnit.PlayAnimation("射程外", sub_situation: "");
                            }
                            else
                            {
                                targetUnit.SpecialEffect("射程外", sub_situation: "");
                            }

                            targetUnit.PilotMessage("射程外", msg_mode: "");
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
                if (Commands.SupportAttackUnit.Status != "出撃" || Commands.SelectedUnit.Status != "出撃" || Commands.SelectedTarget.Status != "出撃" || support_attack_done)
                {
                    Commands.SupportAttackUnit = null;
                }
            }

            if (Commands.SupportAttackUnit is object)
            {
                {
                    var sau = Commands.SupportAttackUnit;
                    // サポートアタックに使う武器を決定
                    w2 = SelectWeapon(Commands.SupportAttackUnit, Commands.SelectedTarget, "サポートアタック", out _, out _);
                    if (w2 > 0)
                    {
                        // サポートアタックを実施
                        Map.MaskData[sau.x, sau.y] = false;
                        if (!SRC.BattleAnimation)
                        {
                            GUI.MaskScreen();
                        }

                        if (sau.IsAnimationDefined("サポートアタック開始", sub_situation: ""))
                        {
                            sau.PlayAnimation("サポートアタック開始", sub_situation: "");
                        }

                        GUI.UpdateMessageForm(Commands.SelectedTarget, Commands.SupportAttackUnit);
                        sau.Attack(sau.Weapon(w2), Commands.SelectedTarget, "援護攻撃", def_mode);
                    }
                }

                // 後始末
                {
                    var saucf = Commands.SupportAttackUnit.CurrentForm();
                    if (saucf.IsAnimationDefined("サポートアタック終了", sub_situation: ""))
                    {
                        saucf.PlayAnimation("サポートアタック終了", sub_situation: "");
                    }

                    // サポートアタックの残り回数を減らす
                    if (w2 > 0)
                    {
                        saucf.UsedSupportAttack = (saucf.UsedSupportAttack + 1);
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
                var selectedUnit = Commands.SelectedTarget;

                // 経験値＆資金が獲得できるか判定
                if (selectedUnit.Party == "味方" && selectedUnit.Status == "出撃")
                {
                    get_reward = true;
                }
                else if (selectedUnit.Summoner is object)
                {
                    if (selectedUnit.Summoner.Party == "味方"
                        && selectedUnit.Party0 == "ＮＰＣ"
                        && selectedUnit.Status == "出撃"
                        && selectedUnit.IsFeatureAvailable("召喚ユニット")
                        && !selectedUnit.IsConditionSatisfied("混乱")
                        && !selectedUnit.IsConditionSatisfied("暴走"))
                    {
                        get_reward = true;
                    }
                }

                if (get_reward)
                {
                    if (Commands.SelectedUnit.Status == "破壊" && !is_suiside)
                    {
                        // 経験値を獲得
                        selectedUnit.GetExp(Commands.SelectedUnit, "破壊", exp_mode: "");

                        // 現在の資金を記録
                        prev_money = SRC.Money;

                        // 獲得する資金を算出
                        earnings = Commands.SelectedUnit.Value / 2;

                        // スペシャルパワーによる獲得資金増加
                        if (selectedUnit.IsUnderSpecialPowerEffect("獲得資金増加"))
                        {
                            earnings = (int)(earnings * (1d + 0.1d * selectedUnit.SpecialPowerEffectLevel("獲得資金増加")));
                        }

                        // パイロット能力による獲得資金増加
                        if (selectedUnit.IsSkillAvailable("資金獲得"))
                        {
                            if (!selectedUnit.IsUnderSpecialPowerEffect("獲得資金増加") || Expression.IsOptionDefined("収得効果重複"))
                            {
                                earnings = (int)GeneralLib.MinDbl(earnings * ((10d + selectedUnit.SkillLevel("資金獲得", 5d)) / 10d), 999999999d);
                            }
                        }

                        // 資金を獲得
                        SRC.IncrMoney(earnings);
                        if (SRC.Money > prev_money)
                        {
                            GUI.DisplaySysMessage(SrcFormatter.Format(SRC.Money - prev_money) + "の" + Expression.Term("資金", Commands.SelectedUnit) + "を得た。");
                        }
                    }
                    else
                    {
                        selectedUnit.GetExp(Commands.SelectedUnit, "攻撃", exp_mode: "");
                    }
                }

                // スペシャルパワー「獲得資金増加」「獲得経験値増加」の効果はここで削除する
                selectedUnit.RemoveSpecialPowerInEffect("戦闘終了");
                if (earnings > 0)
                {
                    selectedUnit.RemoveSpecialPowerInEffect("敵破壊");
                }
            }

            // 味方が呼び出した召喚ユニットの場合はＮＰＣでも経験値と資金を獲得
            Commands.SelectedUnit = Commands.SelectedUnit.CurrentForm();
            {
                var selectedUnit = Commands.SelectedUnit;
                if (selectedUnit.Summoner is object)
                {
                    if (selectedUnit.Summoner.Party == "味方"
                        && selectedUnit.Party0 == "ＮＰＣ"
                        && selectedUnit.Status == "出撃"
                        && selectedUnit.IsFeatureAvailable("召喚ユニット")
                        && !selectedUnit.IsConditionSatisfied("混乱")
                        && !selectedUnit.IsConditionSatisfied("暴走"))
                    {
                        if (Commands.SelectedTarget.Status == "破壊")
                        {
                            // ターゲットを破壊した場合

                            // 経験値を獲得
                            selectedUnit.GetExp(Commands.SelectedTarget, "破壊", exp_mode: "");

                            // 獲得する資金を算出
                            earnings = Commands.SelectedTarget.Value / 2;

                            // スペシャルパワーによる獲得資金増加
                            if (selectedUnit.IsUnderSpecialPowerEffect("獲得資金増加"))
                            {
                                earnings = (int)(earnings * (1d + 0.1d * selectedUnit.SpecialPowerEffectLevel("獲得資金増加")));
                            }

                            // パイロット能力による獲得資金増加
                            if (selectedUnit.IsSkillAvailable("資金獲得"))
                            {
                                if (!selectedUnit.IsUnderSpecialPowerEffect("獲得資金増加") || Expression.IsOptionDefined("収得効果重複"))
                                {
                                    earnings = (int)((earnings * (10d + selectedUnit.SkillLevel("資金獲得", 5d))) / 10L);
                                }
                            }

                            // 資金を獲得
                            SRC.IncrMoney(earnings);
                            if (earnings > 0)
                            {
                                GUI.DisplaySysMessage(SrcFormatter.Format(earnings) + "の" + Expression.Term("資金", Commands.SelectedTarget) + "を得た。");
                            }
                        }
                        else
                        {
                            // ターゲットを破壊出来なかった場合

                            // 経験値を獲得
                            selectedUnit.GetExp(Commands.SelectedTarget, "攻撃", exp_mode: "");
                        }
                    }
                }

                if (selectedUnit.Status == "出撃")
                {
                    // スペシャルパワー効果「敵破壊時再行動」
                    if (selectedUnit.IsUnderSpecialPowerEffect("敵破壊時再行動"))
                    {
                        if (Commands.SelectedTarget.Status == "破壊")
                        {
                            selectedUnit.UsedAction = (selectedUnit.UsedAction - 1);
                        }
                    }

                    // 持続期間が「戦闘終了」のスペシャルパワー効果を削除
                    selectedUnit.RemoveSpecialPowerInEffect("戦闘終了");
                    if (earnings > 0)
                    {
                        selectedUnit.RemoveSpecialPowerInEffect("敵破壊");
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
                    if (withBlock27.Status == "破壊")
                    {
                        Event.HandleEvent("破壊", withBlock27.MainPilot().ID);
                    }
                    else if (withBlock27.Status == "出撃" && withBlock27.HP / (double)withBlock27.MaxHP < attack_target_hp_ratio)
                    {
                        Event.HandleEvent("損傷率", withBlock27.MainPilot().ID, "" + (100 * (withBlock27.MaxHP - withBlock27.HP) / withBlock27.MaxHP));
                    }

                    if (SRC.IsScenarioFinished || SRC.IsCanceled)
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
                    if (withBlock28.Status == "破壊")
                    {
                        Event.HandleEvent("破壊", withBlock28.MainPilot().ID);
                    }
                    else if (withBlock28.Status == "出撃" && withBlock28.HP / (double)withBlock28.MaxHP < defense_target_hp_ratio)
                    {
                        Event.HandleEvent("損傷率", withBlock28.MainPilot().ID, "" + (100 * (withBlock28.MaxHP - withBlock28.HP) / withBlock28.MaxHP));
                    }
                }
            }

            if (SRC.IsScenarioFinished)
            {
                Commands.RestoreSelections();
                Commands.SelectedPartners.Clear();
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
                            if (withBlock29.Status == "破壊")
                            {
                                Event.HandleEvent("破壊", withBlock29.MainPilot().ID);
                            }
                            else if (withBlock29.Status == "出撃" && withBlock29.HP / (double)withBlock29.MaxHP < defense_target2_hp_ratio)
                            {
                                Event.HandleEvent("損傷率", withBlock29.MainPilot().ID, "" + (100 * (withBlock29.MaxHP - withBlock29.HP) / withBlock29.MaxHP));
                            }
                        }
                    }
                }
            }

            // 元に戻す
            Commands.RestoreSelections();
            if (SRC.IsScenarioFinished || SRC.IsCanceled)
            {
                Commands.SelectedPartners.Clear();
                return;
            }

            // 武器の使用後イベント
            if (Commands.SelectedUnit.Status == "出撃" && w > 0)
            {
                Event.HandleEvent("使用後", Commands.SelectedUnit.MainPilot().ID, wname);
                if (SRC.IsScenarioFinished || SRC.IsCanceled)
                {
                    Commands.SelectedPartners.Clear();
                    return;
                }
            }

            if (Commands.SelectedTarget.Status == "出撃" && tw > 0)
            {
                Commands.SaveSelections();
                Commands.SwapSelections();
                Event.HandleEvent("使用後", Commands.SelectedUnit.MainPilot().ID, twname);
                Commands.RestoreSelections();
                if (SRC.IsScenarioFinished || SRC.IsCanceled)
                {
                    Commands.SelectedPartners.Clear();
                    return;
                }
            }

            // 攻撃後イベント
            if (Commands.SelectedUnit.Status == "出撃" && Commands.SelectedTarget.Status == "出撃")
            {
                Event.HandleEvent("攻撃後", Commands.SelectedUnit.MainPilot().ID, Commands.SelectedTarget.MainPilot().ID);
                if (SRC.IsScenarioFinished || SRC.IsCanceled)
                {
                    Commands.SelectedPartners.Clear();
                    return;
                }
            }

            // もし敵が移動していれば進入イベント
            {
                var withBlock30 = Commands.SelectedTarget;
                if (withBlock30.Status == "出撃")
                {
                    if (withBlock30.x != tx || withBlock30.y != ty)
                    {
                        Event.HandleEvent("進入", withBlock30.MainPilot().ID, "" + withBlock30.x, "" + withBlock30.y);
                        if (SRC.IsScenarioFinished || SRC.IsCanceled)
                        {
                            Commands.SelectedPartners.Clear();
                            return;
                        }
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

            // 再移動
            if (is_p_weapon && Commands.SelectedUnit.Status == "出撃")
            {
                if (Commands.SelectedUnit.MainPilot().IsSkillAvailable("遊撃") && Commands.SelectedUnit.Speed * 2 > Commands.SelectedUnitMoveCost)
                {
                    // 進入イベント
                    if (Commands.SelectedUnitMoveCost > 0)
                    {
                        Event.HandleEvent("進入", Commands.SelectedUnit.MainPilot().ID, "" + Commands.SelectedUnit.x, "" + Commands.SelectedUnit.y);
                        if (SRC.IsScenarioFinished)
                        {
                            return;
                        }
                    }

                    // ユニットが既に出撃していない？
                    if (Commands.SelectedUnit.Status != "出撃")
                    {
                        return;
                    }

                    took_action = true;
                    Map.AreaInSpeed(Commands.SelectedUnit);

                    // 目標地点が設定されている？
                    int localLLength2() { string arglist = Commands.SelectedUnit.Mode; var ret = GeneralLib.LLength(arglist); Commands.SelectedUnit.Mode = arglist; return ret; }

                    if (localLLength2() == 2)
                    {

                        string localLIndex6() { string arglist = Commands.SelectedUnit.Mode; var ret = GeneralLib.LIndex(arglist, 1); Commands.SelectedUnit.Mode = arglist; return ret; }

                        dst_x = Conversions.ToInteger(localLIndex6());

                        string localLIndex8() { string arglist = Commands.SelectedUnit.Mode; var ret = GeneralLib.LIndex(arglist, 2); Commands.SelectedUnit.Mode = arglist; return ret; }

                        dst_y = Conversions.ToInteger(localLIndex8());
                        if (1 <= dst_x && dst_x <= Map.MapWidth && 1 <= dst_y && dst_y <= Map.MapHeight)
                        {
                            goto Move;
                        }
                    }

                    // そうでなければ安全な場所へ
                    Map.SafetyPoint(Commands.SelectedUnit, out dst_x, out dst_y);
                    goto Move;
                }
            }

            // 行動終了
            goto EndOfOperation;
        SearchNearestEnemy:
            ;

            // もっとも近くにいる敵を探す
            searched_nearest_enemy = true;
            Commands.SelectedTarget = SearchNearestEnemy(Commands.SelectedUnit);

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
                int tmp;
                var selectedUnit = Commands.SelectedUnit;
                // 移動可能範囲を設定

                // テレポート能力を使える場合は優先的に使用
                if (GeneralLib.LLength(selectedUnit.FeatureData("テレポート")) == 2)
                {
                    tmp = Conversions.ToInteger(GeneralLib.LIndex("テレポート", 2));
                }
                else
                {
                    tmp = 40;
                }

                if (selectedUnit.IsFeatureAvailable("テレポート") && (selectedUnit.EN > 10 * tmp || selectedUnit.EN - tmp > selectedUnit.MaxEN / 2) && Commands.SelectedUnitMoveCost == 0)
                {
                    mmode = "テレポート";
                    selectedUnit.EN = selectedUnit.EN - tmp;
                    Map.AreaInTeleport(Commands.SelectedUnit);
                    goto MoveAreaSelected;
                }

                // ジャンプ能力を使う？
                if (GeneralLib.LLength(selectedUnit.FeatureData("ジャンプ")) == 2)
                {
                    tmp = Conversions.ToInteger(GeneralLib.LIndex("ジャンプ", 2));
                }
                else
                {
                    tmp = 0;
                }

                if (selectedUnit.IsFeatureAvailable("ジャンプ") && selectedUnit.Area != "空中" && selectedUnit.Area != "宇宙" && (selectedUnit.EN > 10 * tmp || selectedUnit.EN - tmp > selectedUnit.MaxEN / 2) && Commands.SelectedUnitMoveCost == 0)
                {
                    mmode = "ジャンプ";
                    selectedUnit.EN = selectedUnit.EN - tmp;
                    Map.AreaInSpeed(Commands.SelectedUnit, true);
                    goto MoveAreaSelected;
                }

                // 通常移動
                mmode = "";
                Map.AreaInSpeed(Commands.SelectedUnit);
            MoveAreaSelected:
                ;

                // 護衛すべきユニットがいる場合は動ける範囲を限定
                if (guard_unit_mode)
                {
                    {
                        var withBlock32 = SRC.PList.Item(selectedUnit.Mode).Unit;
                        var loopTo13 = Map.MapWidth;
                        for (i = 1; i <= loopTo13; i++)
                        {
                            var loopTo14 = Map.MapHeight;
                            for (j = 1; j <= loopTo14; j++)
                            {
                                if (!Map.MaskData[i, j])
                                {
                                    if (Math.Abs((withBlock32.x - i)) + Math.Abs((withBlock32.y - j)) > 1)
                                    {
                                        Map.MaskData[i, j] = true;
                                    }
                                }
                            }
                        }
                    }
                }

                if (selectedUnit.Mode == "逃亡")
                {
                    // 移動可能範囲内で敵から最も遠い場所を検索
                    Map.SafetyPoint(Commands.SelectedUnit, out dst_x, out dst_y);
                    new_x = dst_x;
                    new_y = dst_y;
                }
                else if (selectedUnit.IsConditionSatisfied("混乱"))
                {
                    // 移動可能範囲内からランダムに行き先を選択
                    dst_x = (selectedUnit.x + GeneralLib.Dice(selectedUnit.Speed + 1) - GeneralLib.Dice(selectedUnit.Speed + 1));
                    dst_y = (selectedUnit.y + GeneralLib.Dice(selectedUnit.Speed + 1) - GeneralLib.Dice(selectedUnit.Speed + 1));
                    Map.NearestPoint(Commands.SelectedUnit, dst_x, dst_y, out new_x, out new_y);
                }
                else
                {
                    // 移動可能範囲内で移動目的地に最も近い場所を検索
                    Map.NearestPoint(Commands.SelectedUnit, dst_x, dst_y, out new_x0, out new_y0);

                    // 移動先が危険地域かどうか判定する
                    tmp = (Math.Abs((dst_x - new_x0)) + Math.Abs((dst_y - new_y0)));
                    if (tmp <= 5)
                    {
                        if (Map.MapDataForUnit[dst_x, dst_y] is object)
                        {
                            if (!selectedUnit.IsEnemy(Map.MapDataForUnit[dst_x, dst_y]))
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
                                        tx = (new_x0 + 1);
                                        ty = new_y0;
                                        break;
                                    }

                                case 2:
                                    {
                                        tx = (new_x0 - 1);
                                        ty = new_y0;
                                        break;
                                    }

                                case 3:
                                    {
                                        tx = new_x0;
                                        ty = (new_y0 + 1);
                                        break;
                                    }

                                case 4:
                                    {
                                        tx = new_x0;
                                        ty = (new_y0 - 1);
                                        break;
                                    }

                                case 5:
                                    {
                                        tx = (new_x0 + 1);
                                        ty = (new_y0 + 1);
                                        break;
                                    }

                                case 6:
                                    {
                                        tx = (new_x0 - 1);
                                        ty = (new_y0 + 1);
                                        break;
                                    }

                                case 7:
                                    {
                                        tx = (new_x0 + 1);
                                        ty = (new_y0 - 1);
                                        break;
                                    }

                                case 8:
                                    {
                                        tx = (new_x0 - 1);
                                        ty = (new_y0 - 1);
                                        break;
                                    }

                                case 9:
                                    {
                                        tx = (new_x0 + 2);
                                        ty = new_y0;
                                        break;
                                    }

                                case 10:
                                    {
                                        tx = (new_x0 - 2);
                                        ty = new_y0;
                                        break;
                                    }

                                case 11:
                                    {
                                        tx = new_x0;
                                        ty = (new_y0 + 2);
                                        break;
                                    }

                                case 12:
                                    {
                                        tx = new_x0;
                                        ty = (new_y0 - 2);
                                        break;
                                    }
                            }

                            if (1 <= tx && tx <= Map.MapWidth && 1 <= ty && ty <= Map.MapHeight)
                            {
                                if (!Map.MaskData[tx, ty] && (Math.Abs((dst_x - tx)) + Math.Abs((dst_y - ty))) < (Math.Abs((dst_x - selectedUnit.x)) + Math.Abs((dst_y - selectedUnit.y))))
                                {
                                    tmp = Map.Terrain(tx, ty).EffectForHPRecover() + Map.Terrain(tx, ty).EffectForENRecover() + 100 * selectedUnit.LookForSupport(tx, ty);

                                    // 地形による防御効果は空中にいる場合にのみ適用
                                    if (selectedUnit.Area != "空中")
                                    {
                                        tmp = ((tmp + Map.Terrain(tx, ty).HitMod) + Map.Terrain(tx, ty).DamageMod);
                                        // 水中用ユニットの場合は水中を優先
                                        if (Map.Terrain(tx, ty).Class == "水")
                                        {
                                            if (selectedUnit.IsTransAvailable("水"))
                                            {
                                                tmp = (tmp + 100);
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
                                        tx = (new_x0 + 1);
                                        ty = new_y0;
                                        break;
                                    }

                                case 2:
                                    {
                                        tx = (new_x0 - 1);
                                        ty = new_y0;
                                        break;
                                    }

                                case 3:
                                    {
                                        tx = new_x0;
                                        ty = (new_y0 + 1);
                                        break;
                                    }

                                case 4:
                                    {
                                        tx = new_x0;
                                        ty = (new_y0 - 1);
                                        break;
                                    }
                            }

                            if (1 <= tx && tx <= Map.MapWidth && 1 <= ty && ty <= Map.MapHeight)
                            {
                                if (!Map.MaskData[tx, ty] && (Math.Abs((dst_x - tx)) + Math.Abs((dst_y - ty))) < (Math.Abs((dst_x - selectedUnit.x)) + Math.Abs((dst_y - selectedUnit.y))))
                                {
                                    tmp = selectedUnit.LookForSupport(tx, ty);
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

                if (new_x < 1 || Map.MapWidth < new_x || new_y < 1 || Map.MapHeight < new_y)
                {
                    // 移動できる場所がない……
                    goto EndOfOperation;
                }

                // 見つかった場所がいまいる場所でなければそこへ移動
                if (selectedUnit.x != new_x || selectedUnit.y != new_y)
                {
                    switch (mmode ?? "")
                    {
                        case "テレポート":
                            {
                                if (selectedUnit.IsMessageDefined("テレポート"))
                                {
                                    GUI.OpenMessageForm(u1: null, u2: null);
                                    selectedUnit.PilotMessage("テレポート", msg_mode: "");
                                    GUI.CloseMessageForm();
                                }

                                if (selectedUnit.IsAnimationDefined("テレポート", selectedUnit.FeatureName("テレポート")))
                                {
                                    selectedUnit.PlayAnimation("テレポート", selectedUnit.FeatureName("テレポート"));
                                }
                                else if (selectedUnit.IsSpecialEffectDefined("テレポート", selectedUnit.FeatureName("テレポート")))
                                {
                                    selectedUnit.SpecialEffect("テレポート", selectedUnit.FeatureName("テレポート"));
                                }
                                else if (SRC.BattleAnimation)
                                {
                                    Effect.ShowAnimation("テレポート発動 Whiz.wav " + selectedUnit.FeatureName0("テレポート"));
                                }
                                else
                                {
                                    Sound.PlayWave("Whiz.wav");
                                }

                                selectedUnit.Move(new_x, new_y, true, false, true);
                                Commands.SelectedUnitMoveCost = 1000;
                                GUI.RedrawScreen();
                                break;
                            }

                        case "ジャンプ":
                            {
                                if (selectedUnit.IsMessageDefined("ジャンプ"))
                                {
                                    GUI.OpenMessageForm(u1: null, u2: null);
                                    selectedUnit.PilotMessage("ジャンプ", msg_mode: "");
                                    GUI.CloseMessageForm();
                                }

                                if (selectedUnit.IsAnimationDefined("ジャンプ", selectedUnit.FeatureName("ジャンプ")))
                                {
                                    selectedUnit.PlayAnimation("ジャンプ", selectedUnit.FeatureName("ジャンプ"));
                                }
                                else if (selectedUnit.IsSpecialEffectDefined("ジャンプ", selectedUnit.FeatureName("ジャンプ")))
                                {
                                    selectedUnit.SpecialEffect("ジャンプ", selectedUnit.FeatureName("ジャンプ"));
                                }
                                else
                                {
                                    Sound.PlayWave("Swing.wav");
                                }

                                selectedUnit.Move(new_x, new_y, true, false, true);
                                Commands.SelectedUnitMoveCost = 1000;
                                GUI.RedrawScreen();
                                break;
                            }

                        default:
                            {
                                // 通常移動
                                selectedUnit.Move(new_x, new_y);
                                Commands.SelectedUnitMoveCost = Map.TotalMoveCost[new_x, new_y];
                                break;
                            }
                    }

                    moved = true;

                    // 思考モードが「(X,Y)に移動」で目的地についた場合
                    if (GeneralLib.LLength(selectedUnit.Mode) == 2)
                    {
                        if (selectedUnit.x == dst_x && selectedUnit.y == dst_y)
                        {
                            selectedUnit.Mode = "待機";
                        }
                    }
                }

                // ここでＥＮ切れ？
                if (selectedUnit.EN == 0)
                {
                    if (selectedUnit.MaxAction() == 0)
                    {
                        goto EndOfOperation;
                    }
                }

                // 魅了されている場合
                if (selectedUnit.IsConditionSatisfied("魅了"))
                {
                    goto EndOfOperation;
                }

                // 逃げている場合
                if (selectedUnit.Mode == "逃亡")
                {
                    goto EndOfOperation;
                }

                // 思考モードが特定のターゲットを狙うように設定されている場合
                if (SRC.PList.IsDefined(selectedUnit.Mode))
                {
                    if (ReferenceEquals(SRC.PList.Item(selectedUnit.Mode).Unit, Commands.SelectedTarget))
                    {
                        if (selectedUnit.IsEnemy(Commands.SelectedTarget))
                        {
                            if (moved)
                            {
                                w = SelectWeapon(Commands.SelectedUnit, Commands.SelectedTarget, "移動後", out _, out _);
                            }
                            else
                            {
                                w = SelectWeapon(Commands.SelectedUnit, Commands.SelectedTarget, "移動可能", out _, out _);
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
                int localLLength4() { string arglist = selectedUnit.Mode; var ret = GeneralLib.LLength(arglist); selectedUnit.Mode = arglist; return ret; }

                if (localLLength4() == 2)
                {
                    if (1 <= dst_x && dst_x <= Map.MapWidth && 1 <= dst_y && dst_y <= Map.MapHeight)
                    {
                        if (Map.MapDataForUnit[dst_x, dst_y] is object)
                        {
                            Commands.SelectedTarget = Map.MapDataForUnit[dst_x, dst_y];
                            if (selectedUnit.IsEnemy(Commands.SelectedTarget))
                            {
                                // 移動先の場所にいる敵を優先して排除
                                if (moved)
                                {
                                    w = SelectWeapon(Commands.SelectedUnit, Commands.SelectedTarget, "移動後", out _, out _);
                                }
                                else
                                {
                                    w = SelectWeapon(Commands.SelectedUnit, Commands.SelectedTarget, "移動可能", out _, out _);
                                }

                                if (w > 0)
                                {
                                    goto AttackEnemy;
                                }
                            }
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
            Commands.SelectedPartners.Clear();
            if (moved)
            {
                // 持続期間が「移動」のスペシャルパワー効果を削除
                Commands.SelectedUnit.RemoveSpecialPowerInEffect("移動");
            }
        }

        // ハイパーモードが可能であればハイパーモード発動
        private void TryHyperMode()
        {
            var bu = Commands.SelectedUnit;
            // ハイパーモードを持っている？
            if (!bu.IsFeatureAvailable("ハイパーモード"))
            {
                return;
            }

            var fname = bu.FeatureName("ハイパーモード");
            var flevel = bu.FeatureLevel("ハイパーモード");
            var fdata = bu.FeatureData("ハイパーモード");

            // 発動条件を満たす？
            if (bu.MainPilot().Morale < 100 + (10d * flevel) && (Strings.InStr(fdata, "気力発動") > 0 || bu.HP > bu.MaxHP / 4))
            {
                return;
            }

            // ハイパーモードが禁止されている？
            if (bu.IsConditionSatisfied("形態固定"))
            {
                return;
            }

            if (bu.IsConditionSatisfied("機体固定"))
            {
                return;
            }

            // 変身中・能力コピー中はハイパーモードを使用できない
            if (bu.IsConditionSatisfied("ノーマルモード付加"))
            {
                return;
            }

            // ハイパーモード先の形態を調べる
            var uname = GeneralLib.LIndex(fdata, 2);
            var u = bu.OtherForm(uname);

            // ハイパーモード先の形態は使用可能？
            if (u.IsConditionSatisfied("行動不能") || !u.IsAbleToEnter(bu.x, bu.y))
            {
                return;
            }

            // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
            if (u.IsFeatureAvailable("追加パイロット"))
            {
                if (!SRC.PList.IsDefined("追加パイロット"))
                {
                    SRC.PList.Add(u.FeatureData("追加パイロット"), bu.MainPilot().Level, bu.Party0, gid: "");
                }
            }

            // ハイパーモードメッセージ
            bu.PilotMassageIfDefined(new string[]
            {
                "ハイパーモード(" + bu.Name + "=>" + uname + ")",
                "ハイパーモード(" + uname + ")",
                "ハイパーモード(" + fname + ")",
                "ハイパーモード",
            });

            // アニメ表示
            bu.PlayAnimationIfDefined(new string[]
            {
                "ハイパーモード(" + bu.Name + "=>" + uname + ")",
                "ハイパーモード(" + uname + ")",
                "ハイパーモード(" + fname + ")",
                "ハイパーモード",
            });

            // ハイパーモード発動
            bu.Transform(uname);

            // ハイパーモードイベント
            var cf = u.CurrentForm();
            Event.HandleEvent("ハイパーモード", cf.MainPilot().ID, cf.Name);

            // ハイパーモード＆ノーマルモードの自動発動
            u.CurrentForm().CheckAutoHyperMode();
            u.CurrentForm().CheckAutoNormalMode();
            Commands.SelectedUnit = u.CurrentForm();
            SRC.GUIStatus.DisplayUnitStatus(Commands.SelectedUnit);
        }

        // 戦闘形態への変形が可能であれば変形する
        public bool TryBattleTransform()
        {
            var bu = Commands.SelectedUnit;
            // 変形が可能？
            if (!bu.IsFeatureAvailable("変形") || bu.IsConditionSatisfied("形態固定") || bu.IsConditionSatisfied("機体固定"))
            {
                return false;
            }

            // ５マス以内に敵がいるかチェック
            if (DistanceFromNearestEnemy(Commands.SelectedUnit) > 5)
            {
                // 周りに敵はいない
                return false;
            }

            // 最も運動性が高い形態に変形
            var u = Commands.SelectedUnit;
            var xx = bu.x;
            var yy = bu.y;
            foreach (var tfuname in bu.Feature("変形").DataL.Skip(1))
            {
                var tfu = bu.OtherForm(tfuname);
                // その形態に変形可能？
                if (tfu.IsConditionSatisfied("行動不能") || !tfu.IsAbleToEnter(xx, yy))
                {
                    continue;
                }

                // 通常形態は弱い形態であるという仮定に基づき、その形態が
                // ノーマルモードで指定されている場合ば無視する
                if ((tfuname ?? "") == (GeneralLib.LIndex("ノーマルモード", 1) ?? ""))
                {
                    continue;
                }

                // 海では水中もしくは空中適応を持つユニットを優先
                switch (Map.Terrain(xx, yy).Class)
                {
                    case "水":
                    case "深海":
                        {
                            // 水中適応を持つユニットを最優先
                            if (Strings.InStr(tfu.Data.Transportation, "水") > 0)
                            {
                                if (Strings.InStr(u.Data.Transportation, "水") == 0)
                                {
                                    u = tfu.OtherForm(tfuname);
                                    continue;
                                }
                            }

                            if (Strings.InStr(u.Data.Transportation, "水") > 0)
                            {
                                if (Strings.InStr(tfu.Data.Transportation, "水") == 0)
                                {
                                    continue;
                                }
                            }

                            // 次点で空中適応ユニット
                            if (Strings.InStr(tfu.Data.Transportation, "空") > 0)
                            {
                                if (Strings.InStr(u.Data.Transportation, "空") == 0)
                                {
                                    u = tfu.OtherForm(tfuname);
                                    continue;
                                }
                            }

                            if (Strings.InStr(u.Data.Transportation, "空") > 0)
                            {
                                if (Strings.InStr(tfu.Data.Transportation, "空") == 0)
                                {
                                    continue;
                                }
                            }

                            break;
                        }
                }

                // 運動性が高いものを優先
                if (tfu.Data.Mobility < u.Data.Mobility)
                {
                    continue;
                }
                else if (tfu.Data.Mobility == u.Data.Mobility)
                {
                    // 運動性が同じなら攻撃力が高いものを優先
                    if (tfu.Data.CountWeapon() == 0)
                    {
                        // この形態は武器を持っていない
                        continue;
                    }
                    else if (u.Data.CountWeapon() > 0)
                    {
                        if (tfu.Weapons.Max(x => x.WeaponData.Power) < u.Weapons.Max(x => x.WeaponData.Power))
                        {
                            continue;
                        }
                        else if (tfu.Weapons.Max(x => x.WeaponData.Power) == u.Weapons.Max(x => x.WeaponData.Power))
                        {
                            // 攻撃力も同じなら装甲が高いものを優先
                            if (tfu.Data.Armor <= u.Data.Armor)
                            {
                                continue;
                            }
                        }
                    }
                }

                u = bu.OtherForm(tfuname);
            }

            // 現在の形態が最も戦闘に適している？
            if (u == bu)
            {
                return false;
            }

            // 形態uに変形決定
            var uname = u.Name;

            // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
            if (u.IsFeatureAvailable("追加パイロット"))
            {
                if (!SRC.PList.IsDefined("追加パイロット"))
                {
                    SRC.PList.Add(u.FeatureData("追加パイロット"), bu.MainPilot().Level, bu.Party0, gid: "");
                }
            }

            // 変形メッセージ
            bu.PilotMassageIfDefined(new string[]
            {
                "変形(" + bu.Name + "=>" + uname + ")",
                "変形(" + uname + ")",
                "変形(" + bu.FeatureName("変形") + ")",
            });

            // アニメ表示
            bu.PlayAnimationIfDefined(new string[]
            {
                "変形(" + bu.Name + "=>" + uname + ")",
                "変形(" + uname + ")",
                "変形(" + bu.FeatureName("変形") + ")",
            });

            // 変形
            bu.Transform(uname);

            // 変形イベント
            var cf = u.CurrentForm();
            SRC.Event.HandleEvent("変形", cf.MainPilot().ID, cf.Name);

            // ハイパーモード＆ノーマルモードの自動発動
            u.CurrentForm().CheckAutoHyperMode();
            u.CurrentForm().CheckAutoNormalMode();
            Commands.SelectedUnit = u.CurrentForm();
            SRC.GUIStatus.DisplayUnitStatus(Commands.SelectedUnit);
            return true;
        }

        // 移動形態への変形が可能であれば変形する
        private bool TryMoveTransform()
        {
            var bu = Commands.SelectedUnit;
            // 変形が可能？
            if (!bu.IsFeatureAvailable("変形") || bu.IsConditionSatisfied("形態固定") || bu.IsConditionSatisfied("機体固定"))
            {
                return false;
            }

            var xx = bu.x;
            var yy = bu.y;
            int tx, ty;

            // 地形に邪魔されて移動できなくならないか調べるため、目的地の方向にある
            // 隣接するマスの座標を調べる
            if (Math.Abs((Commands.SelectedX - xx)) > Math.Abs((Commands.SelectedY - yy)))
            {
                if (Commands.SelectedX > xx)
                {
                    tx = (xx + 1);
                }
                else
                {
                    tx = (xx - 1);
                }

                ty = yy;
            }
            else
            {
                tx = xx;
                if (Commands.SelectedY > yy)
                {
                    ty = (yy + 1);
                }
                else
                {
                    ty = (yy - 1);
                }
            }

            // 最も移動力が高い形態に変形
            var u = bu;
            foreach (var tfuname in bu.Feature("変形").DataL.Skip(1))
            {
                var tfu = bu.OtherForm(tfuname);
                // その形態に変形可能？
                if (tfu.IsConditionSatisfied("行動不能") || !tfu.IsAbleToEnter(xx, yy))
                {
                    continue;
                }

                // 目的地方面に移動可能？
                if (u.IsAbleToEnter(tx, ty) && !tfu.IsAbleToEnter(tx, ty))
                {
                    continue;
                }

                // 移動力が高い方を優先
                var speed1 = tfu.Data.Speed;
                if (tfu.Data.IsFeatureAvailable("テレポート"))
                {
                    speed1 = (int)(speed1 + tfu.Data.FeatureLevel("テレポート") + 1d);
                }

                if (tfu.Data.IsFeatureAvailable("ジャンプ"))
                {
                    speed1 = (int)(speed1 + tfu.Data.FeatureLevel("ジャンプ") + 1d);
                }
                // 移動可能な地形タイプも考慮
                switch (Map.Terrain(xx, yy).Class)
                {
                    case "水":
                    case "深海":
                        {
                            if (Strings.InStr(tfu.Data.Transportation, "水") > 0 || Strings.InStr(tfu.Data.Transportation, "空") > 0)
                            {
                                speed1 = (speed1 + 1);
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
                            if (Strings.InStr(tfu.Data.Transportation, "空") > 0)
                            {
                                speed1 = (speed1 + 1);
                            }

                            break;
                        }
                }

                var speed2 = u.Data.Speed;
                if (u.Data.IsFeatureAvailable("テレポート"))
                {
                    speed2 = (int)(speed2 + u.Data.FeatureLevel("テレポート") + 1d);
                }

                if (u.Data.IsFeatureAvailable("ジャンプ"))
                {
                    speed2 = (int)(speed2 + u.Data.FeatureLevel("ジャンプ") + 1d);
                }
                // 移動可能な地形タイプも考慮
                switch (Map.Terrain(xx, yy).Class)
                {
                    case "水":
                    case "深海":
                        {
                            if (Strings.InStr(u.Data.Transportation, "水") > 0 || Strings.InStr(u.Data.Transportation, "空") > 0)
                            {
                                speed2 = (speed2 + 1);
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
                                speed2 = (speed2 + 1);
                            }

                            break;
                        }
                }

                if (speed2 > speed1)
                {
                    continue;
                }
                else if (speed2 == speed1)
                {
                    // 移動力が同じなら装甲が高い方を優先
                    if (u.Data.Armor >= tfu.Data.Armor)
                    {
                        continue;
                    }
                }

                u = bu.OtherForm(tfuname);
            }

            // 現在の形態が最も移動に適している？
            if (u == bu)
            {
                return false;
            }

            // 形態uに変形決定
            var uname = u.Name;

            // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
            if (u.IsFeatureAvailable("追加パイロット"))
            {
                if (!SRC.PList.IsDefined("追加パイロット"))
                {
                    SRC.PList.Add(u.FeatureData("追加パイロット"), bu.MainPilot().Level, bu.Party0, gid: "");
                }
            }

            // 変形メッセージ
            bu.PilotMassageIfDefined(new string[]
            {
                "変形(" + bu.Name + "=>" + uname + ")",
                "変形(" + uname + ")",
                "変形(" + bu.FeatureName("変形") + ")",
            });

            // アニメ表示
            bu.PlayAnimationIfDefined(new string[]
            {
                "変形(" + bu.Name + "=>" + uname + ")",
                "変形(" + uname + ")",
                "変形(" + bu.FeatureName("変形") + ")",
            });

            // 変形
            bu.Transform(uname);

            // 変形イベント
            var cf = u.CurrentForm();
            Event.HandleEvent("変形", cf.MainPilot().ID, cf.Name);

            // ハイパーモード＆ノーマルモードの自動発動
            u.CurrentForm().CheckAutoHyperMode();
            u.CurrentForm().CheckAutoNormalMode();
            Commands.SelectedUnit = u.CurrentForm();
            SRC.GUIStatus.DisplayUnitStatus(Commands.SelectedUnit);
            return true;
        }

        // 実行時間を必要としないアビリティがあれば使っておく
        public void TryInstantAbility()
        {
            // ５マス以内に敵がいるかチェック
            if (DistanceFromNearestEnemy(Commands.SelectedUnit) > 5)
            {
                // 周りに敵はいないのでアビリティは使わない
                return;
            }

            var u = Commands.SelectedUnit;
            UnitAbility selUa = null;
            // 実行時間を必要としないアビリティを探す
            // TODO EN以外の使用条件見ておく
            foreach (var ua in u.Abilities)
            {
                // 使用可能＆効果あり？
                if (!ua.IsAbilityUseful("移動前"))
                {
                    continue;
                }

                // ＥＮ消費が多すぎない？
                if (ua.AbilityENConsumption() > 0)
                {
                    if (ua.AbilityENConsumption() >= u.EN / 2)
                    {
                        continue;
                    }
                }

                {
                    // 自己強化のアビリティのみが対象
                    if (ua.Data.MaxRange != 0)
                    {
                        continue;
                    }

                    // 実行時間を必要としない？
                    if (!ua.Data.Effects.Any(x => x.EffectType == "再行動"))
                    {
                        continue;
                    }

                    // 強化用アビリティ？
                    if (ua.Data.Effects.Any(x => x.EffectType == "状態" || x.EffectType == "付加" || x.EffectType == "強化"))
                    {
                        // 強化用アビリティが見つかった
                        selUa = ua;
                        break;
                    }
                }
            }

            // ここに来る時は使用できるアビリティがなかった場合
            if (selUa == null)
            {
                return;
            }
            Commands.SelectedAbility = selUa.AbilityNo();
            var aname = selUa.Data.Name;
            Commands.SelectedAbilityName = aname;

            // 合体技パートナーの設定
            IList<Unit> partners;
            if (selUa.IsAbilityClassifiedAs("合"))
            {
                partners = selUa.CombinationPartner();
            }
            else
            {
                Commands.SelectedPartners.Clear();
                partners = new List<Unit>();
            }

            // アビリティの使用イベント
            Event.HandleEvent("使用", u.MainPilot().ID, aname);
            if (SRC.IsScenarioFinished || SRC.IsCanceled)
            {
                return;
            }

            // アビリティを使用
            GUI.OpenMessageForm(Commands.SelectedUnit, u2: null);
            u.ExecuteAbility(selUa, Commands.SelectedUnit);
            GUI.CloseMessageForm();
            Commands.SelectedUnit = u.CurrentForm();

            // アビリティの使用後イベント
            SRC.Event.HandleEvent("使用後", Commands.SelectedUnit.MainPilot().ID, aname);
            if (SRC.IsScenarioFinished || SRC.IsCanceled)
            {
                Commands.SelectedPartners.Clear();
                return;
            }

            // 自爆アビリティの破壊イベント
            if (Commands.SelectedUnit.Status == "破壊")
            {
                Event.HandleEvent("破壊", Commands.SelectedUnit.MainPilot().ID);
                if (SRC.IsScenarioFinished || SRC.IsCanceled)
                {
                    Commands.SelectedPartners.Clear();
                    return;
                }
            }

            // 行動数を消費しておく
            Commands.SelectedUnit.UseAction();

            // 合体技のパートナーの行動数を減らす
            if (!Expression.IsOptionDefined("合体技パートナー行動数無消費"))
            {
                foreach (var pu in partners)
                {
                    pu.CurrentForm().UseAction();
                }
            }
            Commands.SelectedPartners.Clear();
        }

        // 召喚が可能であれば召喚する
        public bool TrySummonning()
        {
            var u = Commands.SelectedUnit;
            // 召喚アビリティを検索
            UnitAbility selUa = u.Abilities
                .Where(x => x.IsAbilityAvailable("移動前"))
                .Where(x => x.Data.Effects.Any(y => y.EffectType == "召喚"))
                .FirstOrDefault();

            // 使用可能な召喚アビリティを持っていなかった
            if (selUa == null)
            {
                return false;

            }

            Commands.SelectedAbility = selUa.AbilityNo();
            var aname = selUa.Data.Name;
            Commands.SelectedAbilityName = aname;


            // 合体技パートナーの設定
            IList<Unit> partners;
            if (selUa.IsAbilityClassifiedAs("合"))
            {
                partners = selUa.CombinationPartner();
            }
            else
            {
                Commands.SelectedPartners.Clear();
                partners = new List<Unit>();
            }

            // 召喚アビリティの使用イベント
            Event.HandleEvent("使用", u.MainPilot().ID, aname);
            if (SRC.IsScenarioFinished || SRC.IsCanceled)
            {
                Commands.SelectedPartners.Clear();
                // XXX true でいいのか？
                return true;
            }

            // 召喚アビリティを使用
            GUI.OpenMessageForm(Commands.SelectedUnit, u2: null);
            u.ExecuteAbility(selUa, Commands.SelectedUnit);
            GUI.CloseMessageForm();
            Commands.SelectedUnit = u.CurrentForm();

            // 召喚アビリティの使用後イベント
            Event.HandleEvent("使用後", Commands.SelectedUnit.MainPilot().ID, aname);
            if (SRC.IsScenarioFinished || SRC.IsCanceled)
            {
                Commands.SelectedPartners.Clear();
                return true;
            }

            // 自爆アビリティの破壊イベント
            if (Commands.SelectedUnit.Status == "破壊")
            {
                Event.HandleEvent("破壊", Commands.SelectedUnit.MainPilot().ID);
                if (SRC.IsScenarioFinished || SRC.IsCanceled)
                {
                    Commands.SelectedPartners.Clear();
                    return true;
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
            Commands.SelectedPartners.Clear();
            return true;
        }

        // マップ型回復アビリティ使用に関する処理
        public bool TryMapHealing(bool moved)
        {
            var u = Commands.SelectedUnit;
            Commands.SelectedAbility = 0;

            // 狂戦士状態の際は回復アビリティを使わない
            if (u.IsConditionSatisfied("狂戦士"))
            {
                return false;
            }

            int tx = 0;
            int ty = 0;
            var p = u.MainPilot();
            var max_score = 0;
            UnitAbility selUa = null;
            foreach (var ua in u.Abilities)
            {
                // マップアビリティかどうか
                if (!ua.IsAbilityClassifiedAs("Ｍ"))
                {
                    continue;
                }

                // アビリティの使用可否を判定
                if (moved)
                {
                    if (!ua.IsAbilityAvailable("移動後"))
                    {
                        continue;
                    }
                }
                else
                {
                    if (!ua.IsAbilityAvailable("移動前"))
                    {
                        continue;
                    }
                }

                // 回復アビリティかどうか
                var healEffect = ua.Data.Effects.FirstOrDefault(x => x.EffectType == "回復");
                if (healEffect == null)
                {
                    // 回復アビリティではなかった
                    continue;
                }
                // 回復量を算出しておく
                int apower;
                if (ua.IsSpellAbility())
                {
                    apower = (int)(5d * healEffect.Level * p.Shooting);
                }
                else
                {
                    apower = (int)(500d * healEffect.Level);
                }

                var max_range = ua.AbilityMaxRange();
                var min_range = ua.AbilityMinRange();
                var x1 = GeneralLib.MaxLng(u.x - max_range, 1);
                var x2 = GeneralLib.MinLng(u.x + max_range, Map.MapWidth);
                var y1 = GeneralLib.MaxLng(u.y - max_range, 1);
                var y2 = GeneralLib.MinLng(u.y + max_range, Map.MapHeight);

                // アビリティの効果範囲に応じてアビリティが有効かどうか判断する
                var num = 0;
                var score = 0;
                if (ua.IsAbilityClassifiedAs("Ｍ全"))
                {
                    Map.AreaInRange(u.x, u.y, max_range, min_range, u.Party);

                    // 支援専用アビリティの場合は自分には効果がない
                    if (ua.IsAbilityClassifiedAs("援"))
                    {
                        Map.MaskData[u.x, u.y] = true;
                    }

                    // 効果範囲内にいるターゲットをカウント
                    for (var i = x1; i <= x2; i++)
                    {
                        for (var j = y1; j <= y2; j++)
                        {
                            if (Map.MaskData[i, j])
                            {
                                continue;
                            }

                            var t = Map.MapDataForUnit[i, j];
                            if (t is null)
                            {
                                continue;
                            }

                            // アビリティが適用可能？
                            if (!ua.IsAbilityApplicable(t))
                            {
                                continue;
                            }

                            {
                                var withBlock1 = t;
                                // ゾンビ？
                                // XXX Unit.CanFix
                                if (withBlock1.IsConditionSatisfied("ゾンビ"))
                                {
                                    continue;
                                }

                                if (100 * withBlock1.HP / withBlock1.MaxHP < 90)
                                {
                                    num = (num + 1);
                                }

                                score = score + 100 * GeneralLib.MinLng(withBlock1.MaxHP - withBlock1.HP, apower) / withBlock1.MaxHP;
                            }
                        }
                    }

                    // 不要？
                    tx = u.x;
                    ty = u.y;
                }
                else if (ua.IsAbilityClassifiedAs("Ｍ投"))
                {
                    var mlv = (int)ua.AbilityLevel("Ｍ投");

                    // 投下位置を変えながら試してみる
                    for (var xx = x1; xx <= x2; xx++)
                    {
                        for (var yy = y1; yy <= y2; yy++)
                        {
                            if ((Math.Abs((u.x - xx)) + Math.Abs((u.y - yy))) > max_range || (Math.Abs((u.x - xx)) + Math.Abs((u.y - yy))) < min_range)
                            {
                                continue;
                            }

                            Map.AreaInRange(xx, yy, 1, mlv, u.Party);
                            Map.AreaInRange(xx, yy, mlv, 1, u.Party);

                            // 支援専用アビリティの場合は自分には効果がない
                            if (ua.IsAbilityClassifiedAs("援"))
                            {
                                Map.MaskData[u.x, u.y] = true;
                            }

                            // 効果範囲内にいるターゲットをカウント
                            var tmp_num = 0;
                            var tmp_score = 0;
                            var loopTo5 = GeneralLib.MinLng(xx + mlv, Map.MapWidth);
                            for (var i = GeneralLib.MaxLng(xx - mlv, 1); i <= loopTo5; i++)
                            {
                                var loopTo6 = GeneralLib.MinLng(yy + mlv, Map.MapHeight);
                                for (var j = GeneralLib.MaxLng(yy - mlv, 1); j <= loopTo6; j++)
                                {
                                    if (Map.MaskData[i, j])
                                    {
                                        continue;
                                    }

                                    var t = Map.MapDataForUnit[i, j];
                                    if (t is null)
                                    {
                                        continue;
                                    }

                                    // アビリティが適用可能？
                                    if (!ua.IsAbilityApplicable(t))
                                    {
                                        continue;
                                    }
                                    // ゾンビ？
                                    // XXX Unit.CanFix
                                    if (t.IsConditionSatisfied("ゾンビ"))
                                    {
                                        continue;
                                    }

                                    if (100 * t.HP / t.MaxHP < 90)
                                    {
                                        tmp_num = (tmp_num + 1);
                                    }

                                    tmp_score = (tmp_score + 100 * GeneralLib.MinLng(t.MaxHP - t.HP, apower) / t.MaxHP);
                                }
                            }

                            if (tmp_num > 2 && tmp_score > score)
                            {
                                num = tmp_num;
                                score = tmp_score;
                                tx = xx;
                                ty = yy;
                            }
                        }
                    }
                }

                if (num > 1 && score > max_score)
                {
                    selUa = ua;
                    max_score = score;
                }
            }

            // 有効なマップアビリティがなかった
            if (selUa == null)
            {
                return false;
            }

            Commands.SelectedAbility = selUa.AbilityNo();
            Commands.SelectedAbilityName = selUa.Data.Name;

            // 合体技パートナーの設定
            IList<Unit> partners;
            if (selUa.IsAbilityClassifiedAs("合"))
            {
                partners = selUa.CombinationPartner();
            }
            else
            {
                Commands.SelectedPartners.Clear();
                partners = new List<Unit>();
            }

            // アビリティを使用
            u.ExecuteMapAbility(selUa, tx, ty);
            if (SRC.IsScenarioFinished || SRC.IsCanceled)
            {
                Commands.SelectedPartners.Clear();
                // XXX false でいいのか？
                return false;
            }

            // 合体技のパートナーの行動数を減らす
            if (!Expression.IsOptionDefined("合体技パートナー行動数無消費"))
            {
                foreach (var pu in partners)
                {
                    pu.CurrentForm().UseAction();
                }
            }
            Commands.SelectedPartners.Clear();
            return true;
        }

        // 可能であれば回復アビリティを使う
        public bool TryHealing(bool moved, [Optional, DefaultParameterValue(null)] Unit t)
        {
            var u = Commands.SelectedUnit;
            // 狂戦士状態の際は回復アビリティを使わない
            if (u.IsConditionSatisfied("狂戦士"))
            {
                return false;
            }

            // 初期化
            Commands.SelectedTarget = null;
            var max_dmg = 80;
            Commands.SelectedAbility = 0;
            var max_power = 0;

            // 移動可能？
            var dont_move = moved || u.Mode == "固定";

            // 移動可能である場合は移動範囲を設定しておく
            if (!dont_move)
            {
                Map.AreaInSpeed(Commands.SelectedUnit);
            }

            var p = u.MainPilot();
            UnitAbility selUa = null;
            var sa_is_able_to_move = false;
            foreach (var ua in u.Abilities)
            {
                // マップアビリティは別関数で調べる
                if (ua.IsAbilityClassifiedAs("Ｍ"))
                {
                    continue;
                }

                // アビリティの使用可否を判定
                if (moved)
                {
                    if (!ua.IsAbilityAvailable("移動後"))
                    {
                        continue;
                    }
                }
                else
                {
                    if (!ua.IsAbilityAvailable("移動前"))
                    {
                        continue;
                    }
                }

                // 回復アビリティかどうか
                var healEffect = ua.Data.Effects.FirstOrDefault(x => x.EffectType == "回復");
                if (healEffect == null)
                {
                    // 回復アビリティではなかった
                    continue;
                }

                // 回復量を算出しておく
                int apower;
                if (ua.IsSpellAbility())
                {
                    apower = (int)(5d * healEffect.Level * p.Shooting);
                }
                else
                {
                    apower = (int)(500d * healEffect.Level);
                }

                // 役立たず？
                if (apower <= 0)
                {
                    continue;
                }

                // 現在の回復アビリティを使って回復させられるターゲットがいるか検索
                foreach (Unit tu in SRC.UList.Items)
                {
                    if (tu.Status != "出撃")
                    {
                        continue;
                    }

                    // 味方かどうかを判定
                    if (!tu.IsAlly(tu))
                    {
                        continue;
                    }

                    // デフォルトのターゲットが指定されている場合はそのユニット以外を
                    // ターゲットにはしない
                    if (t is object)
                    {
                        if (!ReferenceEquals(tu, t))
                        {
                            continue;
                        }
                    }

                    // 損傷度は？
                    var dmg = 100 * tu.HP / tu.MaxHP;

                    // 重要なユニットを優先
                    if (!ReferenceEquals(tu, Commands.SelectedUnit))
                    {
                        if (tu.BossRank >= 0)
                        {
                            dmg = 100 - 2 * (100 - dmg);
                        }
                    }

                    // 現在のターゲットより損傷度がひどくないなら無視
                    if (dmg > max_dmg)
                    {
                        continue;
                    }

                    // 移動可能か？
                    var is_able_to_move = ua.CanUseAfterMove();
                    if (dont_move)
                    {
                        is_able_to_move = false;
                    }

                    switch (tu.Area ?? "")
                    {
                        case "空中":
                        case "宇宙":
                            {
                                if (tu.EN - ua.AbilityENConsumption() < 5)
                                {
                                    is_able_to_move = false;
                                }

                                break;
                            }
                    }

                    // 射程内にいるか？
                    if (is_able_to_move)
                    {
                        if (!ua.IsTargetReachableForAbility(tu))
                        {
                            continue;
                        }
                    }
                    else if (!ua.IsTargetWithinAbilityRange(tu))
                    {
                        continue;
                    }

                    // アビリティが適用可能？
                    if (!ua.IsAbilityApplicable(tu))
                    {
                        continue;
                    }

                    // ゾンビ？
                    if (tu.IsConditionSatisfied("ゾンビ"))
                    {
                        continue;
                    }

                    // 新規ターゲット？
                    if (!ReferenceEquals(tu, Commands.SelectedTarget))
                    {
                        // ターゲット設定
                        Commands.SelectedTarget = tu;
                        max_dmg = dmg;

                        // 新規ターゲットを優先するため、現在選択されているアビリティは破棄
                        selUa = null;
                        max_power = 0;
                    }

                    // 現在選択されている回復アビリティとチェック中のアビリティのどちらが
                    // 優れているかを判定
                    if (max_power < tu.MaxHP - tu.HP)
                    {
                        // 現在選択している回復アビリティでは全ダメージを回復しきれない場合
                        if (apower < max_power)
                        {
                            // 回復量が多いほうを優先
                            continue;
                        }
                        else if (apower == max_power)
                        {
                            // 回復量が同じならコストが低い方を優先
                            if (ua.Data.ENConsumption > selUa.Data.ENConsumption)
                            {
                                continue;
                            }
                            if (ua.Stock() < selUa.Stock())
                            {
                                continue;
                            }
                        }
                    }
                    else if (selUa != null)
                    {
                        // 現在選択している回復アビリティで全快する場合
                        // 全快することが必要条件
                        if (apower >= tu.MaxHP - tu.HP)
                        {
                            continue;
                        }
                        // コストが低い方を優先
                        if (ua.Data.ENConsumption > tu.Ability(Commands.SelectedAbility).Data.ENConsumption)
                        {
                            continue;
                        }
                        if (ua.Stock() < tu.Ability(Commands.SelectedAbility).Stock())
                        {
                            continue;
                        }
                    }

                    selUa = ua;
                    max_power = apower;
                    sa_is_able_to_move = is_able_to_move;
                }

            NextHealingSkill:
                ;
            }

            // 有用なアビリティ＆ターゲットが見つかった？
            if (selUa == null)
            {
                return false;
            }

            if (Commands.SelectedTarget is null)
            {
                return false;
            }

            // 回復アビリティを使用することが確定

            // 適切な位置に移動
            if (!ReferenceEquals(Commands.SelectedTarget, Commands.SelectedUnit) && sa_is_able_to_move)
            {
                var new_x = u.x;
                var new_y = u.y;
                var max_range = selUa.AbilityMaxRange();
                {
                    var tu = Commands.SelectedTarget;
                    // 現在位置から回復が可能であれば現在位置を優先
                    int distance;
                    if ((Math.Abs((tu.x - new_x)) + Math.Abs((tu.y - new_y))) <= max_range)
                    {
                        distance = (int)(Math.Pow(Math.Abs((tu.x - new_x)), 2d) + Math.Pow(Math.Abs((tu.y - new_y)), 2d));
                    }
                    else
                    {
                        distance = 10000;
                    }

                    // 適切な位置を探す
                    var loopTo2 = GeneralLib.MinLng(tu.x + max_range, Map.MapWidth);
                    for (var i = GeneralLib.MaxLng(tu.x - max_range, 1); i <= loopTo2; i++)
                    {
                        var loopTo3 = GeneralLib.MinLng(tu.y + max_range, Map.MapHeight);
                        for (var j = GeneralLib.MaxLng(tu.y - max_range, 1); j <= loopTo3; j++)
                        {
                            if (!Map.MaskData[i, j] && Map.MapDataForUnit[i, j] is null && (Math.Abs((tu.x - i)) + Math.Abs((tu.y - j))) <= max_range)
                            {
                                {
                                    var withBlock2 = Commands.SelectedUnit;
                                    if (Math.Pow(Math.Abs((withBlock2.x - i)), 2d) + Math.Pow(Math.Abs((withBlock2.y - j)), 2d) < distance)
                                    {
                                        new_x = i;
                                        new_y = j;
                                        distance = (int)(Math.Pow(Math.Abs((withBlock2.x - new_x)), 2d) + Math.Pow(Math.Abs((withBlock2.y - new_y)), 2d));
                                    }
                                }
                            }
                        }
                    }
                }

                if (new_x != u.x || new_y != u.y)
                {
                    // 適切な場所が見つかったので移動
                    u.Move(new_x, new_y);
                    moved = true;
                }
            }

            Commands.SelectedAbility = selUa.AbilityNo();
            var aname = selUa.Data.Name;
            Commands.SelectedAbilityName = aname;

            // 合体技パートナーの設定
            IList<Unit> partners;
            if (selUa.IsAbilityClassifiedAs("合"))
            {
                partners = selUa.CombinationPartner();
            }
            else
            {
                Commands.SelectedPartners.Clear();
                partners = new List<Unit>();
            }

            // 使用イベント
            Event.HandleEvent("使用", u.MainPilot().ID, aname);
            if (SRC.IsScenarioFinished || SRC.IsCanceled)
            {
                Commands.SelectedPartners.Clear();
                return true;
            }

            if (ReferenceEquals(Commands.SelectedTarget, Commands.SelectedUnit))
            {
                GUI.OpenMessageForm(Commands.SelectedUnit, u2: null);
            }
            else
            {
                GUI.OpenMessageForm(Commands.SelectedTarget, Commands.SelectedUnit);
            }

            // 回復アビリティを実行
            u.ExecuteAbility(selUa, Commands.SelectedTarget);
            Commands.SelectedUnit = u.CurrentForm();

            GUI.CloseMessageForm();

            // 自爆した場合の破壊イベント
            if (Commands.SelectedUnit.Status == "破壊")
            {
                if (Commands.SelectedUnit.CountPilot() > 0)
                {
                    Event.HandleEvent("破壊", Commands.SelectedUnit.MainPilot().ID);
                }

                Commands.SelectedPartners.Clear();
                return true;
            }

            // 使用後イベント
            if (Commands.SelectedUnit.CountPilot() > 0)
            {
                Event.HandleEvent("使用後", Commands.SelectedUnit.MainPilot().ID, aname);
                if (SRC.IsScenarioFinished || SRC.IsCanceled)
                {
                    Commands.SelectedPartners.Clear();
                    return true;
                }
            }

            // 自爆アビリティの破壊イベント
            if (Commands.SelectedUnit.Status == "破壊")
            {
                Event.HandleEvent("破壊", Commands.SelectedUnit.MainPilot().ID);
                if (SRC.IsScenarioFinished || SRC.IsCanceled)
                {
                    Commands.SelectedPartners.Clear();
                    return true;
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
            Commands.SelectedPartners.Clear();
            return true;
        }

        // 修理が可能であれば修理装置を使う
        public bool TryFix(bool moved, [Optional, DefaultParameterValue(null)] Unit t)
        {
            var u = Commands.SelectedUnit;
            // 修理装置を使用可能？
            if (!u.IsFeatureAvailable("修理装置") || u.Area == "地中")
            {
                return false;
            }

            // 狂戦士状態の際は修理装置を使わない
            if (u.IsConditionSatisfied("狂戦士"))
            {
                return false;
            }
            bool[,] TmpMaskData = null;
            // 修理装置を使用可能な領域を設定
            if (moved || u.Mode == "固定")
            {
                // 移動でない場合
                var loopTo = Map.MapWidth;
                for (var i = 1; i <= loopTo; i++)
                {
                    var loopTo1 = Map.MapHeight;
                    for (var j = 1; j <= loopTo1; j++)
                        Map.MaskData[i, j] = true;
                }

                if (u.x > 1)
                {
                    Map.MaskData[u.x - 1, u.y] = false;
                }

                if (u.x < Map.MapWidth)
                {
                    Map.MaskData[u.x + 1, u.y] = false;
                }

                if (u.y > 1)
                {
                    Map.MaskData[u.x, u.y - 1] = false;
                }

                if (u.y < Map.MapHeight)
                {
                    Map.MaskData[u.x, u.y + 1] = false;
                }
            }
            else
            {
                // 移動可能な場合
                TmpMaskData = new bool[Map.MapWidth + 1 + 1, Map.MapHeight + 1 + 1];
                Map.AreaInSpeed(Commands.SelectedUnit);
                var loopTo2 = Map.MapWidth;
                for (var i = 1; i <= loopTo2; i++)
                {
                    var loopTo3 = Map.MapHeight;
                    for (var j = 1; j <= loopTo3; j++)
                        TmpMaskData[i, j] = Map.MaskData[i, j];
                }

                var loopTo4 = Map.MapWidth;
                for (var i = 0; i <= loopTo4; i++)
                {
                    TmpMaskData[i, 0] = true;
                    TmpMaskData[i, Map.MapHeight + 1] = true;
                }

                var loopTo5 = Map.MapHeight;
                for (var i = 0; i <= loopTo5; i++)
                {
                    TmpMaskData[0, i] = true;
                    TmpMaskData[Map.MapWidth + 1, i] = true;
                }

                var loopTo6 = GeneralLib.MinLng(u.x + (u.Speed + 1), Map.MapWidth);
                for (var i = GeneralLib.MaxLng(u.x - (u.Speed + 1), 1); i <= loopTo6; i++)
                {
                    var loopTo7 = GeneralLib.MinLng(u.y + (u.Speed + 1), Map.MapHeight);
                    for (var j = GeneralLib.MaxLng(u.y - (u.Speed + 1), 1); j <= loopTo7; j++)
                        Map.MaskData[i, j] = TmpMaskData[i, j] && TmpMaskData[i - 1, j] && TmpMaskData[i + 1, j] && TmpMaskData[i, j - 1] && TmpMaskData[i, j + 1];
                }

                Map.MaskData[u.x, u.y] = true;
            }

            // ターゲットを探す
            Commands.SelectedTarget = null;
            var max_dmg = 90;
            var loopTo8 = GeneralLib.MinLng(u.x + (u.Speed + 1), Map.MapWidth);
            for (var i = GeneralLib.MaxLng(u.x - (u.Speed + 1), 1); i <= loopTo8; i++)
            {
                var loopTo9 = GeneralLib.MinLng(u.y + (u.Speed + 1), Map.MapHeight);
                for (var j = GeneralLib.MaxLng(u.y - (u.Speed + 1), 1); j <= loopTo9; j++)
                {
                    if (Map.MaskData[i, j])
                    {
                        goto NextFixTarget;
                    }

                    var tu = Map.MapDataForUnit[i, j];
                    if (tu is null)
                    {
                        goto NextFixTarget;
                    }

                    // デフォルトのターゲットが指定されている場合はそのユニット以外を
                    // ターゲットにはしない
                    if (t is object)
                    {
                        if (!ReferenceEquals(tu, t))
                        {
                            goto NextFixTarget;
                        }
                    }

                    // 現在の選択しているターゲットよりダメージが少なければ選択しない
                    if (100 * tu.HP / tu.MaxHP > max_dmg)
                    {
                        goto NextFixTarget;
                    }

                    // 味方かどうか判定
                    if (!u.IsAlly(tu))
                    {
                        goto NextFixTarget;
                    }

                    // ゾンビ？
                    if (tu.IsConditionSatisfied("ゾンビ"))
                    {
                        goto NextFixTarget;
                    }

                    // 修理不可？
                    if (tu.IsFeatureAvailable("修理不可"))
                    {
                        var fd = tu.Feature("修理不可");
                        foreach (var fname0 in fd.DataL.Skip(1))
                        {
                            var fname = fname0;
                            if (Strings.Left(fname, 1) == "!")
                            {
                                fname = Strings.Mid(fname, 2);
                                if ((fname ?? "") != (u.FeatureName0("修理装置") ?? ""))
                                {
                                    goto NextFixTarget;
                                }
                            }
                            else
                            {
                                if ((fname ?? "") == (u.FeatureName0("修理装置") ?? ""))
                                {
                                    goto NextFixTarget;
                                }
                            }
                        }
                    }

                    Commands.SelectedTarget = tu;
                    max_dmg = 100 * tu.HP / tu.MaxHP;
                NextFixTarget:
                    ;
                }
            }

            // ターゲットが見つからない
            if (Commands.SelectedTarget is null)
            {
                return false;
            }

            // ターゲットに隣接するように移動
            if (!moved && u.Mode != "固定")
            {
                var new_x = u.x;
                var new_y = u.y;
                {
                    var withBlock1 = Commands.SelectedTarget;
                    // 現在位置から修理が可能であれば現在位置を優先
                    int tmp;
                    if (Math.Abs((withBlock1.x - new_x)) + Math.Abs((withBlock1.y - new_y)) == 1)
                    {
                        tmp = 1;
                    }
                    else
                    {
                        tmp = 10000;
                    }

                    var loopTo11 = Map.MapWidth;
                    for (var i = 1; i <= loopTo11; i++)
                    {
                        var loopTo12 = Map.MapHeight;
                        for (var j = 1; j <= loopTo12; j++)
                            Map.MaskData[i, j] = TmpMaskData[i, j];
                    }

                    // 適切な場所を探す
                    var loopTo13 = GeneralLib.MinLng(withBlock1.x + 1, Map.MapWidth);
                    for (var i = GeneralLib.MaxLng(withBlock1.x - 1, 1); i <= loopTo13; i++)
                    {
                        var loopTo14 = GeneralLib.MinLng(withBlock1.y + 1, Map.MapHeight);
                        for (var j = GeneralLib.MaxLng(withBlock1.y - 1, 1); j <= loopTo14; j++)
                        {
                            if (!Map.MaskData[i, j] && Map.MapDataForUnit[i, j] is null && Math.Abs((withBlock1.x - i)) + Math.Abs((withBlock1.y - j)) == 1)
                            {
                                {
                                    var withBlock2 = Commands.SelectedUnit;
                                    if (Math.Pow(Math.Abs((withBlock2.x - i)), 2d) + Math.Pow(Math.Abs((withBlock2.y - j)), 2d) < tmp)
                                    {
                                        new_x = i;
                                        new_y = j;
                                        tmp = (int)(Math.Pow(Math.Abs((withBlock2.x - new_x)), 2d) + Math.Pow(Math.Abs((withBlock2.y - new_y)), 2d));
                                    }
                                }
                            }
                        }
                    }
                }

                if (new_x != u.x || new_y != u.y)
                {
                    // 適切な場所が見つかったので移動
                    u.Move(new_x, new_y);
                    moved = true;
                }
            }

            // 選択内容を変更
            Event.SelectedUnitForEvent = Commands.SelectedUnit;
            Event.SelectedTargetForEvent = Commands.SelectedTarget;

            // メッセージ表示
            GUI.OpenMessageForm(Commands.SelectedTarget, Commands.SelectedUnit);
            if (u.IsMessageDefined("修理"))
            {
                u.PilotMessage("修理", msg_mode: "");
            }

            // アニメ表示
            if (u.IsAnimationDefined("修理", u.FeatureName("修理")))
            {
                u.PlayAnimation("修理", u.FeatureName("修理"));
            }
            else
            {
                u.SpecialEffect("修理", u.FeatureName("修理"));
            }

            GUI.DisplaySysMessage(u.Nickname + "は[" + Commands.SelectedTarget.Nickname + "]に[" + u.FeatureName0("修理装置") + "]を使った。");

            // 修理実行
            {
                var tmp = Commands.SelectedTarget.HP;
                switch (u.FeatureLevel("修理装置"))
                {
                    case 1d:
                    case -1:
                        {
                            Commands.SelectedTarget.RecoverHP(30d + 3d * u.MainPilot().SkillLevel("修理技能", ref_mode: ""));
                            break;
                        }

                    case 2d:
                        {
                            Commands.SelectedTarget.RecoverHP(50d + 5d * u.MainPilot().SkillLevel("修理技能", ref_mode: ""));
                            break;
                        }

                    case 3d:
                        {
                            Commands.SelectedTarget.RecoverHP(100d);
                            break;
                        }
                }

                GUI.DrawSysString(Commands.SelectedTarget.x, Commands.SelectedTarget.y, "+" + SrcFormatter.Format(Commands.SelectedTarget.HP - tmp));
                GUI.UpdateMessageForm(Commands.SelectedTarget, Commands.SelectedUnit);
                GUI.DisplaySysMessage(Commands.SelectedTarget.Nickname + "のＨＰが[" + SrcFormatter.Format(Commands.SelectedTarget.HP - tmp) + "]回復した。");
            }
            // 経験値獲得
            Commands.SelectedUnit.GetExp(Commands.SelectedTarget, "修理", exp_mode: "");
            if (GUI.MessageWait < 10000)
            {
                GUI.Sleep(GUI.MessageWait);
            }

            GUI.CloseMessageForm();

            // 形態変化のチェック
            Commands.SelectedTarget.Update();
            Commands.SelectedTarget.CurrentForm().CheckAutoHyperMode();
            Commands.SelectedTarget.CurrentForm().CheckAutoNormalMode();
            return true;
        }

        // マップ攻撃使用に関する処理
        public bool TryMapAttack(bool moved)
        {
            bool TryMapAttackRet = default;
            var currentUnit = Commands.SelectedUnit;
            Commands.SaveSelections();

            // マップ攻撃を使用するターゲット数の下限を設定する
            var score_limit = 1;
            var loopTo = currentUnit.CountWeapon();
            foreach (var uw in currentUnit.Weapons)
            {
                // 通常攻撃を持っている場合は単独の敵への攻撃の際に通常攻撃を優先する
                if (!uw.IsWeaponClassifiedAs("Ｍ"))
                {
                    // XXX なんか移動前固定条件だったけれど、使えれば、でしょ。
                    if ((!moved && uw.IsWeaponAvailable("移動前")) || (moved && uw.IsWeaponAvailable("移動後")))
                    {
                        score_limit = 2;
                        break;
                    }
                }
            }

            // 威力の高い武器を優先して選択
            // XXX を意図して逆順なのだろう
            UnitWeapon useUw = null;
            int tx = -1;
            int ty = -1;
            foreach (var uw in currentUnit.Weapons.Reverse())
            {
                // XXX ヒットはこの段階ではしてない
                useUw = uw;
                Commands.SelectedWeapon = uw.WeaponNo();
                Commands.SelectedTWeapon = 0;

                // マップ攻撃かどうか
                if (!uw.IsWeaponClassifiedAs("Ｍ"))
                {
                    goto NextWeapon;
                }

                // 武器の使用可否を判定
                if (moved)
                {
                    if (!uw.IsWeaponAvailable("移動後"))
                    {
                        goto NextWeapon;
                    }
                }
                else
                {
                    if (!uw.IsWeaponAvailable("移動前"))
                    {
                        goto NextWeapon;
                    }
                }

                // ボスユニットが自爆＆全ＥＮ消費攻撃等を使うのは非常時のみ
                if (currentUnit.BossRank >= 0)
                {
                    if (uw.IsWeaponClassifiedAs("自") || uw.IsWeaponClassifiedAs("尽") || uw.IsWeaponClassifiedAs("消"))
                    {
                        if (currentUnit.HP > currentUnit.MaxHP / 4)
                        {
                            goto NextWeapon;
                        }
                    }
                }

                var max_range = uw.WeaponMaxRange();
                var min_range = uw.WeaponMinRange();
                var x1 = GeneralLib.MaxLng(currentUnit.x - max_range, 1);
                var y1 = GeneralLib.MaxLng(currentUnit.y - max_range, 1);
                var x2 = GeneralLib.MinLng(currentUnit.x + max_range, Map.MapWidth);
                var y2 = GeneralLib.MinLng(currentUnit.y + max_range, Map.MapHeight);

                // マップ攻撃の種類にしたがって効果範囲内にいる敵をカウント
                if (uw.IsWeaponClassifiedAs("Ｍ直"))
                {
                    foreach (var direction in Constants.DIRECTIONS)
                    {
                        // 効果範囲を設定
                        Map.AreaInLine(currentUnit.x, currentUnit.y, min_range, max_range, direction);
                        Map.MaskData[currentUnit.x, currentUnit.y] = true;

                        // 効果範囲内にいるユニットをカウント
                        var enemy_num = CountTargetInRange(uw, x1, y1, x2, y2);

                        // マップ攻撃が最強武器であればターゲットが１体であっても使用
                        if (enemy_num >= score_limit || enemy_num == 1 && uw.IsLastWeapon())
                        {
                            switch (direction ?? "")
                            {
                                case "N":
                                    {
                                        tx = currentUnit.x;
                                        ty = GeneralLib.MaxLng(currentUnit.y - max_range, 1);
                                        break;
                                    }

                                case "S":
                                    {
                                        tx = currentUnit.x;
                                        ty = GeneralLib.MinLng(currentUnit.y + max_range, Map.MapHeight);
                                        break;
                                    }

                                case "W":
                                    {
                                        tx = GeneralLib.MaxLng(currentUnit.x - max_range, 1);
                                        ty = currentUnit.y;
                                        break;
                                    }

                                case "E":
                                    {
                                        tx = GeneralLib.MinLng(currentUnit.x + max_range, Map.MapWidth);
                                        ty = currentUnit.y;
                                        break;
                                    }
                            }

                            goto FoundWeapon;
                        }
                    }
                }
                else if (uw.IsWeaponClassifiedAs("Ｍ拡"))
                {
                    foreach (var direction in Constants.DIRECTIONS)
                    {
                        // 効果範囲を設定
                        Map.AreaInCone(currentUnit.x, currentUnit.y, min_range, max_range, direction);
                        Map.MaskData[currentUnit.x, currentUnit.y] = true;

                        // 効果範囲内にいるユニットをカウント
                        var enemy_num = CountTargetInRange(uw, x1, y1, x2, y2);

                        // マップ攻撃が最強武器であればターゲットが１体であっても使用
                        if (enemy_num >= score_limit || enemy_num == 1 && uw.IsLastWeapon())
                        {
                            switch (direction ?? "")
                            {
                                case "N":
                                    {
                                        tx = currentUnit.x;
                                        ty = (currentUnit.y - 1);
                                        break;
                                    }

                                case "S":
                                    {
                                        tx = currentUnit.x;
                                        ty = (currentUnit.y + 1);
                                        break;
                                    }

                                case "W":
                                    {
                                        tx = (currentUnit.x - 1);
                                        ty = currentUnit.y;
                                        break;
                                    }

                                case "E":
                                    {
                                        tx = (currentUnit.x + 1);
                                        ty = currentUnit.y;
                                        break;
                                    }
                            }

                            goto FoundWeapon;
                        }
                    }
                }
                else if (uw.IsWeaponClassifiedAs("Ｍ扇"))
                {
                    foreach (var direction in Constants.DIRECTIONS)
                    {
                        // 効果範囲を設定
                        Map.AreaInSector(currentUnit.x, currentUnit.y, min_range, max_range, direction, (int)uw.WeaponLevel("Ｍ扇"));
                        Map.MaskData[currentUnit.x, currentUnit.y] = true;

                        // 効果範囲内にいるユニットをカウント
                        var enemy_num = CountTargetInRange(uw, x1, y1, x2, y2);

                        // マップ攻撃が最強武器であればターゲットが１体であっても使用
                        if (enemy_num >= score_limit || enemy_num == 1 && uw.IsLastWeapon())
                        {
                            switch (direction ?? "")
                            {
                                case "N":
                                    {
                                        tx = currentUnit.x;
                                        ty = (currentUnit.y - 1);
                                        break;
                                    }

                                case "S":
                                    {
                                        tx = currentUnit.x;
                                        ty = (currentUnit.y + 1);
                                        break;
                                    }

                                case "W":
                                    {
                                        tx = (currentUnit.x - 1);
                                        ty = currentUnit.y;
                                        break;
                                    }

                                case "E":
                                    {
                                        tx = (currentUnit.x + 1);
                                        ty = currentUnit.y;
                                        break;
                                    }
                            }

                            goto FoundWeapon;
                        }
                    }
                }
                else if (uw.IsWeaponClassifiedAs("Ｍ全"))
                {
                    // 効果範囲を設定
                    Map.AreaInRange(currentUnit.x, currentUnit.y, max_range, min_range, "すべて");
                    Map.MaskData[currentUnit.x, currentUnit.y] = true;

                    // 効果範囲内にいるユニットをカウント
                    var enemy_num = CountTargetInRange(uw, x1, y1, x2, y2);

                    // マップ攻撃が最強武器であればターゲットが１体であっても使用
                    if (enemy_num >= score_limit || enemy_num == 1 && uw.IsLastWeapon())
                    {
                        tx = currentUnit.x;
                        ty = currentUnit.y;
                        goto FoundWeapon;
                    }
                }
                else if (uw.IsWeaponClassifiedAs("Ｍ投"))
                {
                    var lv = (int)uw.WeaponLevel("Ｍ投");
                    var score = 0;
                    for (var xx = x1; xx <= x2; xx++)
                    {
                        for (var yy = y1; yy <= y2; yy++)
                        {
                            if ((Math.Abs((currentUnit.x - xx)) + Math.Abs((currentUnit.y - yy))) <= max_range && (Math.Abs((currentUnit.x - xx)) + Math.Abs((currentUnit.y - yy))) >= min_range)
                            {
                                // 効果範囲を設定
                                if (lv > 0)
                                {
                                    Map.AreaInRange(xx, yy, lv, 1, "すべて");
                                }
                                else
                                {
                                    Map.ClearMask();
                                    Map.MaskData[xx, yy] = false;
                                }

                                Map.MaskData[currentUnit.x, currentUnit.y] = true;

                                // 効果範囲内にいるユニットをカウント
                                var enemy_num = CountTargetInRange(uw, (xx - lv), (yy - lv), (xx + lv), (yy + lv));
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
                    if (score >= score_limit || score == 1 && uw.IsLastWeapon() || score == 1 && lv == 0)
                    {
                        goto FoundWeapon;
                    }
                }
                else if (uw.IsWeaponClassifiedAs("Ｍ線"))
                {
                    var score = 0;
                    for (var xx = x1; xx <= x2; xx++)
                    {
                        for (var yy = y1; yy <= y2; yy++)
                        {
                            if ((Math.Abs((currentUnit.x - xx)) + Math.Abs((currentUnit.y - yy))) <= max_range && (Math.Abs((currentUnit.x - xx)) + Math.Abs((currentUnit.y - yy))) >= min_range)
                            {
                                // 効果範囲を設定
                                Map.AreaInPointToPoint(currentUnit.x, currentUnit.y, xx, yy);
                                Map.MaskData[currentUnit.x, currentUnit.y] = true;

                                // 効果範囲内にいるユニットをカウント
                                var enemy_num = CountTargetInRange(uw, GeneralLib.MinLng(currentUnit.x, xx), GeneralLib.MinLng(currentUnit.y, yy), GeneralLib.MaxLng(currentUnit.x, xx), GeneralLib.MaxLng(currentUnit.y, yy));
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
                    if (score >= score_limit || score == 1 && uw.IsLastWeapon())
                    {
                        goto FoundWeapon;
                    }
                }
                else if (uw.IsWeaponClassifiedAs("Ｍ移"))
                {
                    // その場を動かない場合は移動型マップ攻撃は選考外
                    if (currentUnit.Mode == "固定")
                    {
                        goto NextWeapon;
                    }

                    var score = 0;
                    for (var xx = x1; xx <= x2; xx++)
                    {
                        for (var yy = y1; yy <= y2; yy++)
                        {
                            if ((Math.Abs((currentUnit.x - xx)) + Math.Abs((currentUnit.y - yy))) <= max_range && (Math.Abs((currentUnit.x - xx)) + Math.Abs((currentUnit.y - yy))) >= min_range && Map.MapDataForUnit[xx, yy] is null && currentUnit.IsAbleToEnter(xx, yy))
                            {
                                // 効果範囲を設定
                                Map.AreaInPointToPoint(currentUnit.x, currentUnit.y, xx, yy);
                                Map.MaskData[currentUnit.x, currentUnit.y] = true;

                                // 効果範囲内にいるユニットをカウント
                                var enemy_num = CountTargetInRange(uw, GeneralLib.MinLng(currentUnit.x, xx), GeneralLib.MinLng(currentUnit.y, yy), GeneralLib.MaxLng(currentUnit.x, xx), GeneralLib.MaxLng(currentUnit.y, yy));
                                if (enemy_num > score)
                                {
                                    // 最終チェック 目標地点にたどり着けるか？
                                    Map.AreaInMoveAction(Commands.SelectedUnit, max_range);
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
                    if (score >= score_limit || score == 1 && uw.IsLastWeapon() || score == 1 && max_range == 2)
                    {
                        goto FoundWeapon;
                    }
                }

            NextWeapon:
                ;
            }

            // 有効なマップ攻撃が見つからなかった

            Commands.RestoreSelections();
            TryMapAttackRet = false;
            return TryMapAttackRet;
        FoundWeapon:
            ;


            // 有効なマップ攻撃が見つかった場合

            // 合体技パートナーの設定
            IList<Unit> partners;
            if (useUw.IsWeaponClassifiedAs("合"))
            {
                partners = useUw.CombinationPartner();
            }
            else
            {
                Commands.SelectedPartners.Clear();
                partners = new List<Unit>();
            }

            // マップ攻撃による攻撃を実行
            currentUnit.MapAttack(useUw, tx, ty);

            // 合体技のパートナーの行動数を減らす
            if (!Expression.IsOptionDefined("合体技パートナー行動数無消費"))
            {
                foreach (var pu in partners)
                {
                    pu.CurrentForm().UseAction();
                }
            }

            Commands.SelectedPartners.Clear();
            Commands.RestoreSelections();
            TryMapAttackRet = true;

            return TryMapAttackRet;
        }

        // 効果範囲内にいるターゲットをカウント
        private int CountTargetInRange(UnitWeapon uw, int x1, int y1, int x2, int y2)
        {
            int CountTargetInRangeRet = default;
            var is_ally_involved = default(bool);
            var currentUnit = Commands.SelectedUnit;
            // 効果範囲内のターゲットを検索
            var loopTo = GeneralLib.MinLng(x2, Map.MapWidth);
            for (var i = GeneralLib.MaxLng(x1, 1); i <= loopTo; i++)
            {
                var loopTo1 = GeneralLib.MinLng(y2, Map.MapHeight);
                for (var j = GeneralLib.MaxLng(y1, 1); j <= loopTo1; j++)
                {
                    // 効果範囲内？
                    if (Map.MaskData[i, j])
                    {
                        goto NextPoint;
                    }

                    var t = Map.MapDataForUnit[i, j];

                    // ユニットが存在する？
                    if (t is null)
                    {
                        goto NextPoint;
                    }

                    // ダメージを与えられる？
                    if (uw.HitProbability(t, false) == 0)
                    {
                        goto NextPoint;
                    }
                    else if (uw.ExpDamage(t, false) <= 10)
                    {
                        if (uw.IsNormalWeapon())
                        {
                            goto NextPoint;
                        }
                        else if (uw.CriticalProbability(t) <= 1 && uw.WeaponLevel("Ｋ") == 0d && uw.WeaponLevel("吹") == 0d)
                        {
                            goto NextPoint;
                        }
                    }

                    // ターゲットは敵？
                    if (currentUnit.IsAlly(t))
                    {
                        // 味方の場合は同士討ちの可能性があるのでチェックしておく
                        is_ally_involved = true;
                        goto NextPoint;
                    }

                    // 特定の陣営のみを攻撃する場合
                    switch (currentUnit.Mode ?? "")
                    {
                        case "味方":
                        case "ＮＰＣ":
                            {
                                if (t.Party != "味方" && t.Party != "ＮＰＣ")
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
                    if (t.IsUnderSpecialPowerEffect("隠れ身"))
                    {
                        goto NextPoint;
                    }

                    if (t.IsFeatureAvailable("ステルス"))
                    {
                        if (!t.IsConditionSatisfied("ステルス無効") && !currentUnit.IsFeatureAvailable("ステルス無効化"))
                        {
                            if (t.IsFeatureLevelSpecified("ステルス"))
                            {
                                if (Math.Abs((currentUnit.x - t.x)) + Math.Abs((currentUnit.y - t.y)) > t.FeatureLevel("ステルス"))
                                {
                                    goto NextPoint;
                                }
                            }
                            else if (Math.Abs((currentUnit.x - t.x)) + Math.Abs((currentUnit.y - t.y)) > 3)
                            {
                                goto NextPoint;
                            }
                        }
                    }

                    // ターゲットに含める
                    CountTargetInRangeRet = (CountTargetInRangeRet + 1);
                NextPoint:
                    ;
                }
            }

            // 味方を巻き込んでしまう場合は攻撃を止める
            if (is_ally_involved && !uw.IsWeaponClassifiedAs("識") && !currentUnit.IsUnderSpecialPowerEffect("識別攻撃"))
            {
                CountTargetInRangeRet = 0;
            }

            return CountTargetInRangeRet;
        }

        // スペシャルパワーを使用する
        public void TrySpecialPower(Pilot p)
        {
            // TODO Impl TrySpecialPower
            //string slist;
            //SpecialPowerData sd;
            //int i, tnum;
            //Commands.SelectedPilot = p;

            //// ザコパイロットはスペシャルパワーを使わない
            //if (Strings.InStr(p.Name, "(ザコ)") > 0)
            //{
            //    return;
            //}

            //// 技量が高いほどスペシャルパワーの発動確率が高い
            //if (GeneralLib.Dice(100) > p.TacticalTechnique0() - 100)
            //{
            //    return;
            //}

            //{
            //    var withBlock = Commands.SelectedUnit;
            //    // 正常な判断力がある？
            //    if (withBlock.IsConditionSatisfied("混乱") || withBlock.IsConditionSatisfied("魅了") || withBlock.IsConditionSatisfied("憑依") || withBlock.IsConditionSatisfied("恐怖") || withBlock.IsConditionSatisfied("狂戦士"))
            //    {
            //        return;
            //    }

            //    // スペシャルパワー使用不能
            //    if (withBlock.IsConditionSatisfied("スペシャルパワー使用不能"))
            //    {
            //        return;
            //    }
            //}

            //// 使用する可能性のあるスペシャルパワーの一覧を作成
            //slist = "";
            //var loopTo = p.CountSpecialPower;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    Commands.SelectedSpecialPower = p.get_SpecialPower(i);
            //    sd = SRC.SPDList.Item(Commands.SelectedSpecialPower);

            //    // ＳＰが足りている？
            //    if (p.SP < p.SpecialPowerCost(Commands.SelectedSpecialPower))
            //    {
            //        goto NextSpecialPower;
            //    }

            //    // 既に実行済み？
            //    if (Commands.SelectedUnit.IsSpecialPowerInEffect(Commands.SelectedSpecialPower))
            //    {
            //        goto NextSpecialPower;
            //    }

            //    sd = SRC.SPDList.Item(Commands.SelectedSpecialPower);
            //    {
            //        var withBlock1 = sd;
            //        // ターゲットを選択する必要のあるスペシャルパワーは判断が難しいので
            //        // 使用しない
            //        switch (withBlock1.TargetType ?? "")
            //        {
            //            case "味方":
            //            case "敵":
            //            case "任意":
            //                {
            //                    goto NextSpecialPower;
            //                    break;
            //                }
            //        }

            //        // ターゲットがいなければ使用しない
            //        tnum = withBlock1.CountTarget(p);
            //        if (tnum == 0)
            //        {
            //            goto NextSpecialPower;
            //        }

            //        // 複数のユニットをターゲットにするスペシャルパワーはターゲットが
            //        // 少ない場合は使用しない
            //        switch (withBlock1.TargetType ?? "")
            //        {
            //            case "全味方":
            //            case "全敵":
            //                {
            //                    if (tnum < 3)
            //                    {
            //                        goto NextSpecialPower;
            //                    }

            //                    break;
            //                }
            //        }

            //        // 使用に適した状況下にある？

            //        // UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(ＨＰ回復) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //        if (Conversions.ToBoolean(withBlock1.IsEffectAvailable("ＨＰ回復")))
            //        {
            //            if (withBlock1.TargetType == "自分")
            //            {
            //                if (Commands.SelectedUnit.HP < 0.7d * Commands.SelectedUnit.MaxHP)
            //                {
            //                    goto AddSpecialPower;
            //                }
            //            }
            //            else if (withBlock1.TargetType == "全味方")
            //            {
            //                if (SRC.Turn >= 3)
            //                {
            //                    goto AddSpecialPower;
            //                }
            //            }
            //        }

            //        // UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(ＥＮ回復) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //        if (Conversions.ToBoolean(withBlock1.IsEffectAvailable("ＥＮ回復")))
            //        {
            //            if (withBlock1.TargetType == "自分")
            //            {
            //                if (Commands.SelectedUnit.EN < 0.3d * Commands.SelectedUnit.MaxEN)
            //                {
            //                    goto AddSpecialPower;
            //                }
            //            }
            //            else if (withBlock1.TargetType == "全味方")
            //            {
            //                if (SRC.Turn >= 4)
            //                {
            //                    goto AddSpecialPower;
            //                }
            //            }
            //        }

            //        // UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(気力増加) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //        if (Conversions.ToBoolean(withBlock1.IsEffectAvailable("気力増加")))
            //        {
            //            if (withBlock1.TargetType == "自分")
            //            {
            //                if (p.Morale < p.MaxMorale)
            //                {
            //                    if (p.CountSpecialPower == 1 || p.SP > p.MaxSP / 2)
            //                    {
            //                        goto AddSpecialPower;
            //                    }
            //                }
            //            }
            //            else if (withBlock1.TargetType == "全味方")
            //            {
            //                goto AddSpecialPower;
            //            }
            //        }

            //        // UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(行動数増加) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //        if (Conversions.ToBoolean(withBlock1.IsEffectAvailable("行動数増加")))
            //        {
            //            if (withBlock1.TargetType == "自分")
            //            {
            //                if (DistanceFromNearestEnemy(Commands.SelectedUnit) <= 5)
            //                {
            //                    goto AddSpecialPower;
            //                }
            //            }
            //        }

            //        // UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(復活) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //        if (Conversions.ToBoolean(withBlock1.IsEffectAvailable("復活")))
            //        {
            //            if (withBlock1.TargetType == "自分")
            //            {
            //                goto AddSpecialPower;
            //            }
            //        }

            //        if (IsSPEffectUseful(sd, "絶対命中") || IsSPEffectUseful(sd, "ダメージ増加") || IsSPEffectUseful(sd, "クリティカル率増加") || IsSPEffectUseful(sd, "命中強化") || IsSPEffectUseful(sd, "貫通攻撃") || IsSPEffectUseful(sd, "再攻撃") || IsSPEffectUseful(sd, "隠れ身"))
            //        {
            //            if (withBlock1.TargetType == "自分")
            //            {
            //                if (DistanceFromNearestEnemy(Commands.SelectedUnit) <= 5 || withBlock1.Duration == "攻撃")
            //                {
            //                    goto AddSpecialPower;
            //                }
            //            }
            //            else if (withBlock1.TargetType == "全味方")
            //            {
            //                goto AddSpecialPower;
            //            }
            //        }

            //        if (IsSPEffectUseful(sd, "絶対回避") || IsSPEffectUseful(sd, "被ダメージ低下") || IsSPEffectUseful(sd, "装甲強化") || IsSPEffectUseful(sd, "回避強化"))
            //        {
            //            if (withBlock1.TargetType == "自分")
            //            {
            //                if (DistanceFromNearestEnemy(Commands.SelectedUnit) <= 5 || withBlock1.Duration == "防御")
            //                {
            //                    goto AddSpecialPower;
            //                }
            //            }
            //            else if (withBlock1.TargetType == "全味方")
            //            {
            //                goto AddSpecialPower;
            //            }
            //        }

            //        if (IsSPEffectUseful(sd, "移動力強化"))
            //        {
            //            if (withBlock1.TargetType == "自分")
            //            {
            //                if (DistanceFromNearestEnemy(Commands.SelectedUnit) > 5)
            //                {
            //                    goto AddSpecialPower;
            //                }
            //            }
            //            else if (withBlock1.TargetType == "全味方")
            //            {
            //                goto AddSpecialPower;
            //            }
            //        }

            //        if (IsSPEffectUseful(sd, "射程延長"))
            //        {
            //            if (withBlock1.TargetType == "自分")
            //            {
            //                switch (DistanceFromNearestEnemy(Commands.SelectedUnit))
            //                {
            //                    case 5:
            //                    case 6:
            //                        {
            //                            goto AddSpecialPower;
            //                            break;
            //                        }
            //                }
            //            }
            //            else if (withBlock1.TargetType == "全味方")
            //            {
            //                goto AddSpecialPower;
            //            }
            //        }

            //        if (Conversions.ToBoolean(Operators.OrObject(Operators.OrObject(Operators.OrObject(Operators.OrObject(withBlock1.IsEffectAvailable("気力低下"), withBlock1.IsEffectAvailable("ランダムダメージ")), withBlock1.IsEffectAvailable("ＨＰ減少")), withBlock1.IsEffectAvailable("ＥＮ減少")), withBlock1.IsEffectAvailable("挑発"))))
            //        {
            //            if (withBlock1.TargetType == "全敵")
            //            {
            //                goto AddSpecialPower;
            //            }
            //        }

            //        if (Conversions.ToBoolean(Operators.OrObject(Operators.OrObject(Operators.OrObject(Operators.OrObject(Operators.OrObject(Operators.OrObject(withBlock1.IsEffectAvailable("ダメージ低下"), withBlock1.IsEffectAvailable("被ダメージ増加")), withBlock1.IsEffectAvailable("命中低下")), withBlock1.IsEffectAvailable("回避低下")), withBlock1.IsEffectAvailable("命中率低下")), withBlock1.IsEffectAvailable("移動力低下")), withBlock1.IsEffectAvailable("サポートガード不能"))))
            //        {
            //            if (withBlock1.TargetType == "全敵")
            //            {
            //                if (SRC.Turn >= 3)
            //                {
            //                    goto AddSpecialPower;
            //                }
            //            }
            //        }
            //    }

            //    // 有用な効果が見つからなかった
            //    goto NextSpecialPower;
            //AddSpecialPower:
            //    ;


            //    // スペシャルパワーを候補リストに追加
            //    slist = slist + " " + Commands.SelectedSpecialPower;
            //NextSpecialPower:
            //    ;
            //}

            //// 使用可能なスペシャルパワーを所有していない
            //if (string.IsNullOrEmpty(slist))
            //{
            //    Commands.SelectedSpecialPower = "";
            //    return;
            //}

            //// 使用するスペシャルパワーをランダムに選択
            //Commands.SelectedSpecialPower = GeneralLib.LIndex(slist, GeneralLib.Dice(GeneralLib.LLength(slist)));

            //// 使用イベント
            //Event.HandleEvent("使用", Commands.SelectedUnit.MainPilot().ID, Commands.SelectedSpecialPower);
            //if (SRC.IsScenarioFinished || SRC.IsCanceled)
            //{
            //    return;
            //}

            //// 選択したスペシャルパワーを実行する
            //p.UseSpecialPower(Commands.SelectedSpecialPower);
            //Commands.SelectedUnit = Commands.SelectedUnit.CurrentForm();

            //// ステータスウィンドウ更新
            //if (!GUI.IsRButtonPressed())
            //{
            //    Status.DisplayUnitStatus(Commands.SelectedUnit);
            //}

            //// 使用後イベント
            //Event.HandleEvent("使用後", Commands.SelectedUnit.MainPilot().ID, Commands.SelectedSpecialPower);
            //Commands.SelectedSpecialPower = "";
        }

        private bool IsSPEffectUseful(SpecialPowerData sd, string ename)
        {
            // TODO Impl IsSPEffectUseful
            bool IsSPEffectUsefulRet = default;
            //// UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(ename) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //if (Conversions.ToBoolean(sd.IsEffectAvailable(ename)))
            //{
            //    if (sd.TargetType == "自分")
            //    {
            //        // 自分自身がターゲットである場合、既に同じ効果を持つスペシャル
            //        // パワーを使用している場合は使用しない。
            //        if (!Commands.SelectedUnit.IsSpecialPowerInEffect(ename))
            //        {
            //            IsSPEffectUsefulRet = true;
            //        }
            //    }
            //    else
            //    {
            //        IsSPEffectUsefulRet = true;
            //    }
            //}

            return IsSPEffectUsefulRet;
        }

        // ユニット u がターゲット t を攻撃するための武器を選択
        // amode:攻撃の種類
        // max_prob:敵を破壊できる確率
        // max_dmg:ダメージ期待値
        public int SelectWeapon(Unit u, Unit t, string amode, out int max_prob, out int max_dmg)
        {
            max_prob = 0;
            max_dmg = 0;

            int SelectWeaponRet = default;
            string smode;
            bool use_true_value = default, is_move_attack;
            int prob, destroy_prob;
            int ct_prob;
            double sp_prob;
            int dmg, exp_dmg;
            double dmg_mod;
            Unit su;
            int support_dmg = default, support_prob = default, support_exp_dmg = default;
            int w;
            string wclass, wattr;
            int max_destroy_prob, max_exp_dmg;
            int i, j;
            Unit checku;
            string checkwc;
            bool flag;
            int parry_prob;
            string fdata;

            // 御主人さまにはさからえません
            if (u.IsConditionSatisfied("魅了"))
            {
                if (ReferenceEquals(u.Master, t))
                {
                    SelectWeaponRet = -1;
                    return SelectWeaponRet;
                }
            }

            // 踊りに忙しい……
            if (u.IsConditionSatisfied("踊り"))
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

            // TODO Impl サポートアタックをしてくれるユニットがいるかどうか
            //// サポートアタックをしてくれるユニットがいるかどうか
            //if (Strings.InStr(amode, "反撃") == 0 && Strings.InStr(amode, "サポート") == 0)
            //{
            //    su = u.LookForSupportAttack(t);
            //    if (su is object)
            //    {
            //        w = SelectWeapon(su, t, "サポートアタック", support_prob, support_exp_dmg);
            //        if (w > 0)
            //        {
            //            support_prob = GeneralLib.MinLng(su.HitProbability(w, t, use_true_value), 100);
            //            dmg_mod = 1d;

            //            // サポートアタックダメージ低下
            //            if (Expression.IsOptionDefined("サポートアタックダメージ低下"))
            //            {
            //                dmg_mod = 0.7d;
            //            }

            //            // 同時援護攻撃？
            //            if (su.MainPilot().IsSkillAvailable("統率") && su.IsNormalWeapon(w))
            //            {
            //                if (Expression.IsOptionDefined("ダメージ倍率低下"))
            //                {
            //                    dmg_mod = 1.2d * dmg_mod;
            //                }
            //                else
            //                {
            //                    dmg_mod = 1.5d * dmg_mod;
            //                }
            //            }

            //            support_dmg = su.ExpDamage(w, t, use_true_value, dmg_mod);
            //        }
            //    }
            //}

            SelectWeaponRet = 0;
            max_destroy_prob = 0;
            max_exp_dmg = -1;

            // 各武器を使って試行
            var loopTo = u.CountWeapon();
            for (w = 1; w <= loopTo; w++)
            {
                var selectedWeapon = u.Weapon(w);
                // 武器が使用可能？
                if (!selectedWeapon.IsWeaponAvailable(smode))
                {
                    goto NextWeapon;
                }

                // マップ攻撃は武器選定外
                if (selectedWeapon.IsWeaponClassifiedAs("Ｍ"))
                {
                    goto NextWeapon;
                }

                // 合体技は自分から攻撃をかける場合にのみ使用
                if (selectedWeapon.IsWeaponClassifiedAs("合"))
                {
                    if (Strings.InStr(amode, "反撃") > 0 || Strings.InStr(amode, "サポート") > 0)
                    {
                        goto NextWeapon;
                    }
                }

                // 射程範囲内？
                if (selectedWeapon.IsWeaponClassifiedAs("移動後攻撃可") && amode == "移動可能" && u.Mode != "固定")
                {
                    // 合体技は移動後攻撃可能でも移動を前提にしない
                    // (移動後の位置では使えない危険性があるため)
                    if (selectedWeapon.IsWeaponClassifiedAs("合") && selectedWeapon.IsWeaponClassifiedAs("Ｐ"))
                    {
                        // 移動して攻撃は出来ない
                        if (!selectedWeapon.IsTargetWithinRange(t))
                        {
                            goto NextWeapon;
                        }

                        is_move_attack = false;
                    }
                    else
                    {
                        // 移動して攻撃可能
                        if (!selectedWeapon.IsTargetReachable(t))
                        {
                            goto NextWeapon;
                        }

                        is_move_attack = true;
                    }
                }
                else
                {
                    // 移動して攻撃は出来ない
                    if (!selectedWeapon.IsTargetWithinRange(t))
                    {
                        goto NextWeapon;
                    }

                    is_move_attack = false;
                }

                // 味方ユニットの場合、最後の一発は使用しない
                if (u.Party == "味方" && u.Party0 == "味方" && Strings.InStr(amode, "イベント") == 0)
                {
                    // 自爆攻撃は武器を手動選択する場合にのみ使用
                    if (selectedWeapon.IsWeaponClassifiedAs("自"))
                    {
                        goto NextWeapon;
                    }

                    // 手動反撃時のサポートアタック以外は残弾数が少ない武器を使用しない
                    if (amode != "サポートアタック" || SystemConfig.AutoDefense)
                    {
                        if (!selectedWeapon.IsWeaponClassifiedAs("永"))
                        {
                            if (selectedWeapon.Bullet() == 1 || selectedWeapon.MaxBullet() == 2 || selectedWeapon.MaxBullet() == 3)
                            {
                                goto NextWeapon;
                            }
                        }

                        if (selectedWeapon.WeaponENConsumption() > 0)
                        {
                            if (selectedWeapon.WeaponENConsumption() >= u.EN / 2 || selectedWeapon.WeaponENConsumption() >= u.MaxEN / 4)
                            {
                                goto NextWeapon;
                            }
                        }

                        if (selectedWeapon.IsWeaponClassifiedAs("尽"))
                        {
                            goto NextWeapon;
                        }
                    }
                }

                // ボスユニットが自爆＆全ＥＮ消費攻撃使うのは非常時のみ
                if (u.BossRank >= 0 && Strings.InStr(amode, "イベント") == 0)
                {
                    if (selectedWeapon.IsWeaponClassifiedAs("自") || selectedWeapon.IsWeaponClassifiedAs("尽"))
                    {
                        if (u.HP > u.MaxHP / 4)
                        {
                            goto NextWeapon;
                        }
                    }
                }

                // 特定のユニットをターゲットにしている場合、自爆攻撃はそのターゲットにしか使わない
                if (selectedWeapon.IsWeaponClassifiedAs("自"))
                {
                    if (SRC.PList.IsDefined(u.Mode))
                    {
                        if (SRC.PList.Item(u.Mode).Unit is object)
                        {
                            if (u.IsEnemy(SRC.PList.Item(u.Mode).Unit))
                            {
                                if (!ReferenceEquals(SRC.PList.Item(u.Mode).Unit, t))
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
                    if (Expression.IsOptionDefined("サポートアタックダメージ低下"))
                    {
                        dmg_mod = 0.7d;
                    }
                }

                // ダメージ算出
                dmg = selectedWeapon.ExpDamage(t, use_true_value, dmg_mod);

                // 攻撃の可否判定を行う場合はダメージを与えられる武器があればよい
                if (Strings.InStr(amode, "可否判定") > 0)
                {
                    if (dmg > 0)
                    {
                        SelectWeaponRet = w;
                        return SelectWeaponRet;
                    }
                    else if (!selectedWeapon.IsNormalWeapon())
                    {
                        if (selectedWeapon.CriticalProbability(t) > 0)
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
                    if (selectedWeapon.IsWeaponClassifiedAs("殺"))
                    {
                        goto NextWeapon;
                    }

                    // ダメージ増加のスペシャルパワーを使用している場合はダメージを与えられない
                    // 武器を選択しない
                    if (u.IsUnderSpecialPowerEffect("ダメージ増加"))
                    {
                        goto NextWeapon;
                    }
                }

                // 相手のＨＰが10以下の場合はダメージをかさ上げ
                if (t.HP <= 10)
                {
                    if (0 < dmg && dmg < 20)
                    {
                        if (selectedWeapon.WeaponData.Power > 0)
                        {
                            dmg = 20;
                        }
                    }
                }

                // 再攻撃が可能な場合
                if (Strings.InStr(amode, "サポート") == 0)
                {
                    if (u.IsUnderSpecialPowerEffect("再攻撃"))
                    {
                        // 再攻撃する残弾＆ＥＮがある？
                        if (selectedWeapon.WeaponData.Bullet > 0)
                        {
                            if (selectedWeapon.Bullet() < 2)
                            {
                                goto NextWeapon;
                            }
                        }

                        if (selectedWeapon.WeaponData.ENConsumption > 0)
                        {
                            if (u.EN < 2 * selectedWeapon.WeaponENConsumption())
                            {
                                goto NextWeapon;
                            }
                        }

                        dmg = 2 * dmg;
                    }
                    else if (selectedWeapon.IsWeaponClassifiedAs("再"))
                    {
                        dmg = (int)(dmg + (dmg * selectedWeapon.WeaponLevel("再")) / 16);
                    }
                }

                // 命中率算出
                prob = selectedWeapon.HitProbability(t, use_true_value);

                // 特殊能力による回避を認識する？
                // TODO Impl 特殊能力による回避を認識する？
                if ((u.MainPilot().TacticalTechnique() >= 150 || u.Party == "味方") && !u.IsUnderSpecialPowerEffect("絶対命中"))
                {
                    //// 切り払い可能な場合は命中率を低下
                    //if (selectedWeapon.IsWeaponClassifiedAs("武") || selectedWeapon.IsWeaponClassifiedAs("突") || selectedWeapon.IsWeaponClassifiedAs("実"))
                    //{

                    //    // 切り払い可能？
                    //    flag = false;
                    //    if (t.IsFeatureAvailable("格闘武器"))
                    //    {
                    //        flag = true;
                    //    }
                    //    else
                    //    {
                    //        var loopTo1 = t.CountWeapon();
                    //        for (i = 1; i <= loopTo1; i++)
                    //        {
                    //            if (t.IsWeaponClassifiedAs(i, "武") && t.IsWeaponMastered(i) && t.MainPilot().Morale >= t.Weapon(i).NecessaryMorale && !t.IsDisabled(t.Weapon(i).Name))
                    //            {
                    //                flag = true;
                    //                break;
                    //            }
                    //        }
                    //    }

                    //    if (!t.MainPilot().IsSkillAvailable("切り払い"))
                    //    {
                    //        flag = false;
                    //    }

                    //    // 切り払い出来る場合は命中率を低下
                    //    if (flag)
                    //    {
                    //        parry_prob = (2d * t.MainPilot().SkillLevel("切り払い", ref_mode: ""));
                    //        if (selectedWeapon.IsWeaponClassifiedAs("実"))
                    //        {
                    //            if (selectedWeapon.IsWeaponClassifiedAs("サ"))
                    //            {
                    //                parry_prob = (parry_prob - u.MainPilot().SkillLevel("超感覚", ref_mode: "") - u.MainPilot().SkillLevel("知覚強化", ref_mode: ""));
                    //                {
                    //                    var withBlock = t.MainPilot();
                    //                    parry_prob = (parry_prob + withBlock.SkillLevel("超感覚", ref_mode: "") + withBlock.SkillLevel("知覚強化", ref_mode: ""));
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            parry_prob = (parry_prob - u.MainPilot().SkillLevel("切り払い", ref_mode: ""));
                    //        }

                    //        if (parry_prob > 0)
                    //        {
                    //            prob = prob * (32 - parry_prob) / 32;
                    //        }
                    //    }
                    //}

                    //// 分身可能な場合は命中率を低下
                    //if (t.IsFeatureAvailable("分身"))
                    //{
                    //    if (t.MainPilot().Morale >= 130)
                    //    {
                    //        prob = prob / 2;
                    //    }
                    //}

                    //if (t.MainPilot().SkillLevel("分身", ref_mode: "") > 0d)
                    //{
                    //    prob = ((prob * t.MainPilot().SkillLevel("分身", ref_mode: "")) / 16L);
                    //}

                    //// 超回避可能な場合は命中率を低下
                    //if (t.IsFeatureAvailable("超回避"))
                    //{
                    //    fdata = t.FeatureData("超回避");
                    //    int localStrToLng() { string argexpr = GeneralLib.LIndex(fdata, 2); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                    //    int localStrToLng1() { string argexpr = GeneralLib.LIndex(fdata, 3); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                    //    if (localStrToLng() > t.EN && localStrToLng1() > t.MainPilot().Morale)
                    //    {
                    //        prob = ((prob * t.FeatureLevel("超回避")) / 10L);
                    //    }
                    //}
                }

                // ＣＴ率算出
                ct_prob = selectedWeapon.CriticalProbability(t);

                // 特殊効果を与える確率を計算
                // TODO Impl 特殊効果を与える確率を計算
                sp_prob = 0d;
                wclass = selectedWeapon.WeaponClass();
                {
                    //    var withBlock1 = t;
                    //    var loopTo2 = Strings.Len(wclass);
                    //    for (i = 1; i <= loopTo2; i++)
                    //    {
                    //        wattr = GeneralLib.GetClassBundle(wclass, i);

                    //        // 特殊効果無効化によって無効化される？
                    //        if (withBlock1.SpecialEffectImmune(wattr))
                    //        {
                    //            goto NextAttribute;
                    //        }

                    //        switch (wattr ?? "")
                    //        {
                    //            case "縛":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("行動不能"))
                    //                    {
                    //                        sp_prob = sp_prob + 0.5d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "Ｓ":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("行動不能"))
                    //                    {
                    //                        sp_prob = sp_prob + 0.3d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "眠":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("睡眠"))
                    //                    {
                    //                        sp_prob = sp_prob + 0.3d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "痺":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("麻痺"))
                    //                    {
                    //                        sp_prob = sp_prob + 0.7d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "不":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("攻撃不能") && withBlock1.CountWeapon() > 0)
                    //                    {
                    //                        sp_prob = sp_prob + 0.2d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "止":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("移動不能") && withBlock1.Speed > 0)
                    //                    {
                    //                        sp_prob = sp_prob + 0.2d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "石":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("石化") && withBlock1.BossRank < 0)
                    //                    {
                    //                        sp_prob = sp_prob + 1d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "凍":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("凍結"))
                    //                    {
                    //                        sp_prob = sp_prob + 0.5d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "乱":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("混乱"))
                    //                    {
                    //                        sp_prob = sp_prob + 0.5d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "撹":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("撹乱") && withBlock1.CountWeapon() > 0)
                    //                    {
                    //                        sp_prob = sp_prob + 0.2d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "恐":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("恐怖"))
                    //                    {
                    //                        sp_prob = sp_prob + 0.4d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "魅":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("魅了"))
                    //                    {
                    //                        sp_prob = sp_prob + 0.6d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "憑":
                    //                {
                    //                    if (withBlock1.BossRank < 0)
                    //                    {
                    //                        sp_prob = sp_prob + 1d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "黙":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("沈黙"))
                    //                    {
                    //                        var loopTo3 = withBlock1.CountWeapon();
                    //                        for (j = 1; j <= loopTo3; j++)
                    //                        {
                    //                            if (withBlock1.IsSpellWeapon(j) || withBlock1.IsWeaponClassifiedAs(j, "音"))
                    //                            {
                    //                                sp_prob = sp_prob + 0.3d;
                    //                                break;
                    //                            }
                    //                        }

                    //                        if (j > withBlock1.CountWeapon())
                    //                        {
                    //                            var loopTo4 = withBlock1.CountAbility();
                    //                            for (j = 1; j <= loopTo4; j++)
                    //                            {
                    //                                if (withBlock1.IsSpellAbility(j) || withBlock1.IsAbilityClassifiedAs(j, "音"))
                    //                                {
                    //                                    sp_prob = sp_prob + 0.3d;
                    //                                    break;
                    //                                }
                    //                            }
                    //                        }
                    //                    }

                    //                    break;
                    //                }

                    //            case "盲":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("盲目"))
                    //                    {
                    //                        sp_prob = sp_prob + 0.3d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "毒":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("毒"))
                    //                    {
                    //                        sp_prob = sp_prob + 0.3d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "踊":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("踊り"))
                    //                    {
                    //                        sp_prob = sp_prob + 0.3d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "狂":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("狂戦士"))
                    //                    {
                    //                        sp_prob = sp_prob + 0.3d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "ゾ":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("ゾンビ"))
                    //                    {
                    //                        sp_prob = sp_prob + 0.3d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "害":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("回復不能"))
                    //                    {
                    //                        if (withBlock1.IsFeatureAvailable("ＨＰ回復") || withBlock1.IsFeatureAvailable("ＥＮ回復"))
                    //                        {
                    //                            sp_prob = sp_prob + 0.4d;
                    //                        }
                    //                    }

                    //                    break;
                    //                }

                    //            case "劣":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("装甲劣化"))
                    //                    {
                    //                        sp_prob = sp_prob + 0.3d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "中":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("バリア無効化"))
                    //                    {
                    //                        if (withBlock1.IsFeatureAvailable("バリア") && Strings.InStr(t.FeatureData("バリア"), "バリア無効化無効") == 0)
                    //                        {
                    //                            sp_prob = sp_prob + 0.3d;
                    //                        }
                    //                        else if (withBlock1.IsFeatureAvailable("広域バリア"))
                    //                        {
                    //                            sp_prob = sp_prob + 0.3d;
                    //                        }
                    //                        else if (withBlock1.IsFeatureAvailable("バリアシールド") && Strings.InStr(t.FeatureData("バリアシールド"), "バリア無効化無効") == 0)
                    //                        {
                    //                            sp_prob = sp_prob + 0.3d;
                    //                        }
                    //                        else if (withBlock1.IsFeatureAvailable("フィールド") && Strings.InStr(t.FeatureData("フィールド"), "バリア無効化無効") == 0)
                    //                        {
                    //                            sp_prob = sp_prob + 0.3d;
                    //                        }
                    //                        else if (withBlock1.IsFeatureAvailable("広域フィールド"))
                    //                        {
                    //                            sp_prob = sp_prob + 0.3d;
                    //                        }
                    //                        else if (withBlock1.IsFeatureAvailable("アクティブフィールド") && Strings.InStr(t.FeatureData("アクティブフィールド"), "バリア無効化無効") == 0)
                    //                        {
                    //                            sp_prob = sp_prob + 0.3d;
                    //                        }
                    //                    }

                    //                    break;
                    //                }

                    //            case "除":
                    //                {
                    //                    var loopTo5 = withBlock1.CountCondition();
                    //                    for (j = 1; j <= loopTo5; j++)
                    //                    {
                    //                        string localCondition() { object argIndex1 = j; var ret = withBlock1.Condition(argIndex1); return ret; }

                    //                        string localCondition1() { object argIndex1 = j; var ret = withBlock1.Condition(argIndex1); return ret; }

                    //                        string localCondition2() { object argIndex1 = j; var ret = withBlock1.Condition(argIndex1); return ret; }

                    //                        int localConditionLifetime() { object argIndex1 = j; var ret = withBlock1.ConditionLifetime(argIndex1); return ret; }

                    //                        if ((Strings.InStr(localCondition(), "付加") > 0 || Strings.InStr(localCondition1(), "強化") > 0 || Strings.InStr(localCondition2(), "ＵＰ") > 0) && localConditionLifetime() > 0)
                    //                        {
                    //                            sp_prob = sp_prob + 0.3d;
                    //                            break;
                    //                        }
                    //                    }

                    //                    break;
                    //                }

                    //            case "即":
                    //                {
                    //                    if (withBlock1.BossRank < 0)
                    //                    {
                    //                        sp_prob = sp_prob + 1d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "告":
                    //                {
                    //                    if (withBlock1.BossRank < 0)
                    //                    {
                    //                        sp_prob = sp_prob + 0.4d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "脱":
                    //                {
                    //                    if (withBlock1.MainPilot().Personality != "機械")
                    //                    {
                    //                        sp_prob = sp_prob + 0.2d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "Ｄ":
                    //                {
                    //                    if (withBlock1.MainPilot().Personality != "機械")
                    //                    {
                    //                        sp_prob = sp_prob + 0.25d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "低攻":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("攻撃力ＤＯＷＮ") && withBlock1.CountWeapon() > 0)
                    //                    {
                    //                        sp_prob = sp_prob + 0.2d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "低防":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("防御力ＤＯＷＮ"))
                    //                    {
                    //                        sp_prob = sp_prob + 0.2d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "低運":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("運動性ＤＯＷＮ"))
                    //                    {
                    //                        sp_prob = sp_prob + 0.1d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "低移":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("移動力ＤＯＷＮ") && withBlock1.Speed > 0)
                    //                    {
                    //                        sp_prob = sp_prob + 0.1d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "盗":
                    //                {
                    //                    if (!withBlock1.IsConditionSatisfied("すかんぴん"))
                    //                    {
                    //                        sp_prob = sp_prob + 0.5d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "写":
                    //                {
                    //                    if (withBlock1.BossRank >= 0 || u.IsFeatureAvailable("ノーマルモード"))
                    //                    {
                    //                        goto NextAttribute;
                    //                    }

                    //                    switch (u.Size ?? "")
                    //                    {
                    //                        case "SS":
                    //                            {
                    //                                switch (withBlock1.Size ?? "")
                    //                                {
                    //                                    case "M":
                    //                                    case "L":
                    //                                    case "LL":
                    //                                    case "XL":
                    //                                        {
                    //                                            goto NextAttribute;
                    //                                            break;
                    //                                        }
                    //                                }

                    //                                break;
                    //                            }

                    //                        case "S":
                    //                            {
                    //                                switch (withBlock1.Size ?? "")
                    //                                {
                    //                                    case "L":
                    //                                    case "LL":
                    //                                    case "XL":
                    //                                        {
                    //                                            goto NextAttribute;
                    //                                            break;
                    //                                        }
                    //                                }

                    //                                break;
                    //                            }

                    //                        case "M":
                    //                            {
                    //                                switch (withBlock1.Size ?? "")
                    //                                {
                    //                                    case "SS":
                    //                                    case "LL":
                    //                                    case "XL":
                    //                                        {
                    //                                            goto NextAttribute;
                    //                                            break;
                    //                                        }
                    //                                }

                    //                                break;
                    //                            }

                    //                        case "L":
                    //                            {
                    //                                switch (withBlock1.Size ?? "")
                    //                                {
                    //                                    case "SS":
                    //                                    case "S":
                    //                                    case "XL":
                    //                                        {
                    //                                            goto NextAttribute;
                    //                                            break;
                    //                                        }
                    //                                }

                    //                                break;
                    //                            }

                    //                        case "LL":
                    //                            {
                    //                                switch (withBlock1.Size ?? "")
                    //                                {
                    //                                    case "SS":
                    //                                    case "S":
                    //                                    case "M":
                    //                                        {
                    //                                            goto NextAttribute;
                    //                                            break;
                    //                                        }
                    //                                }

                    //                                break;
                    //                            }

                    //                        case "XL":
                    //                            {
                    //                                switch (withBlock1.Size ?? "")
                    //                                {
                    //                                    case "SS":
                    //                                    case "S":
                    //                                    case "M":
                    //                                    case "L":
                    //                                        {
                    //                                            goto NextAttribute;
                    //                                            break;
                    //                                        }
                    //                                }

                    //                                break;
                    //                            }
                    //                    }

                    //                    sp_prob = sp_prob + 1d;
                    //                    break;
                    //                }

                    //            case "化":
                    //                {
                    //                    if (withBlock1.BossRank < 0 && !u.IsFeatureAvailable("ノーマルモード"))
                    //                    {
                    //                        sp_prob = sp_prob + 1d;
                    //                    }

                    //                    break;
                    //                }

                    //            case "衰":
                    //                {
                    //                    if (withBlock1.BossRank >= 0)
                    //                    {
                    //                        switch (selectedWeapon.WeaponLevel("衰"))
                    //                        {
                    //                            case 1:
                    //                                {
                    //                                    sp_prob = sp_prob + 1d / 8d;
                    //                                    break;
                    //                                }

                    //                            case 2:
                    //                                {
                    //                                    sp_prob = sp_prob + 1d / 4d;
                    //                                    break;
                    //                                }

                    //                            case 3:
                    //                                {
                    //                                    sp_prob = sp_prob + 1d / 2d;
                    //                                    break;
                    //                                }
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        switch (selectedWeapon.WeaponLevel("衰"))
                    //                        {
                    //                            case 1:
                    //                                {
                    //                                    sp_prob = sp_prob + 1d / 4d;
                    //                                    break;
                    //                                }

                    //                            case 2:
                    //                                {
                    //                                    sp_prob = sp_prob + 1d / 2d;
                    //                                    break;
                    //                                }

                    //                            case 3:
                    //                                {
                    //                                    sp_prob = sp_prob + 3d / 4d;
                    //                                    break;
                    //                                }
                    //                        }
                    //                    }

                    //                    break;
                    //                }

                    //            case "滅":
                    //                {
                    //                    if (withBlock1.BossRank >= 0)
                    //                    {
                    //                        switch (selectedWeapon.WeaponLevel("滅"))
                    //                        {
                    //                            case 1:
                    //                                {
                    //                                    sp_prob = sp_prob + 1d / 16d;
                    //                                    break;
                    //                                }

                    //                            case 2:
                    //                                {
                    //                                    sp_prob = sp_prob + 1d / 8d;
                    //                                    break;
                    //                                }

                    //                            case 3:
                    //                                {
                    //                                    sp_prob = sp_prob + 1d / 4d;
                    //                                    break;
                    //                                }
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        switch (selectedWeapon.WeaponLevel("滅"))
                    //                        {
                    //                            case 1:
                    //                                {
                    //                                    sp_prob = sp_prob + 1d / 8d;
                    //                                    break;
                    //                                }

                    //                            case 2:
                    //                                {
                    //                                    sp_prob = sp_prob + 1d / 4d;
                    //                                    break;
                    //                                }

                    //                            case 3:
                    //                                {
                    //                                    sp_prob = sp_prob + 1d / 2d;
                    //                                    break;
                    //                                }
                    //                        }
                    //                    }

                    //                    break;
                    //                }

                    //            default:
                    //                {
                    //                    // 弱属性
                    //                    if (Strings.Left(wattr, 1) == "弱")
                    //                    {
                    //                        // 味方全員を検索して、現在対象に攻撃可能なユニットが
                    //                        // 付加した弱点に対する属性攻撃を持つ場合。
                    //                        // 特殊効果発動率はとりあえず低防(0.2)とそろえてみた
                    //                        checkwc = Strings.Mid(wattr, 2);
                    //                        if (!withBlock1.Weakness(checkwc))
                    //                        {
                    //                            foreach (Unit currentChecku in SRC.UList)
                    //                            {
                    //                                checku = currentChecku;
                    //                                if ((checku.Party ?? "") == (checku.Party ?? "") && checku.Status == "出撃")
                    //                                {
                    //                                    var loopTo6 = checku.CountWeapon();
                    //                                    for (j = 1; j <= loopTo6; j++)
                    //                                    {
                    //                                        if (checku.IsWeaponClassifiedAs(j, checkwc) && checku.IsWeaponAvailable(j, "移動前"))
                    //                                        {
                    //                                            // 射程範囲内？
                    //                                            if (checku.IsWeaponClassifiedAs(j, "移動後攻撃可") && checku.Mode != "固定")
                    //                                            {
                    //                                                // 合体技は移動後攻撃可能でも移動を前提にしない
                    //                                                // (移動後の位置では使えない危険性があるため)
                    //                                                if (checku.IsWeaponClassifiedAs(j, "合") && checku.IsWeaponClassifiedAs(j, "Ｐ"))
                    //                                                {
                    //                                                    // 移動して攻撃は出来ない
                    //                                                    if (checku.IsTargetWithinRange(j, t))
                    //                                                    {
                    //                                                        sp_prob = sp_prob + 0.2d;
                    //                                                        goto NextAttribute;
                    //                                                    }
                    //                                                }
                    //                                                // 移動して攻撃可能
                    //                                                else if (checku.IsTargetReachable(j, t))
                    //                                                {
                    //                                                    sp_prob = sp_prob + 0.2d;
                    //                                                    goto NextAttribute;
                    //                                                }
                    //                                            }
                    //                                            // 移動して攻撃は出来ない
                    //                                            else if (checku.IsTargetWithinRange(j, t))
                    //                                            {
                    //                                                sp_prob = sp_prob + 0.2d;
                    //                                                goto NextAttribute;
                    //                                            }
                    //                                        }
                    //                                    }
                    //                                }
                    //                            }
                    //                        }
                    //                    }
                    //                    // 効属性
                    //                    else if (Strings.Left(wattr, 1) == "効")
                    //                    {
                    //                        // 味方全員を検索して、現在対象に攻撃可能なユニットが
                    //                        // 付加した有効に対する封印、限定攻撃を持つ場合。
                    //                        // 特殊効果発動率は0.1としてみた
                    //                        checkwc = Strings.Mid(wattr, 2);
                    //                        if (!withBlock1.Weakness(checkwc) && !withBlock1.Effective(checkwc))
                    //                        {
                    //                            foreach (Unit currentChecku1 in SRC.UList)
                    //                            {
                    //                                checku = currentChecku1;
                    //                                if ((checku.Party ?? "") == (withBlock1.Party ?? "") && checku.Status == "出撃")
                    //                                {
                    //                                    var loopTo7 = checku.CountWeapon();
                    //                                    for (j = 1; j <= loopTo7; j++)
                    //                                    {
                    //                                        if (checku.IsWeaponClassifiedAs(j, checkwc) && checku.IsWeaponAvailable(j, "移動前"))
                    //                                        {
                    //                                            // 付加する有効に対応する封印、限定武器がある
                    //                                            int localInStrNotNest1() { string argstring1 = checku.WeaponClass(j); string argstring2 = "封"; var ret = GeneralLib.InStrNotNest(argstring1, argstring2); return ret; }

                    //                                            int localInStrNotNest2() { string argstring1 = checku.WeaponClass(j); string argstring2 = "限"; var ret = GeneralLib.InStrNotNest(argstring1, argstring2); return ret; }

                    //                                            if (localInStrNotNest1() > 0 || localInStrNotNest2() > 0)
                    //                                            {
                    //                                                int localInStrNotNest() { string argstring1 = checku.WeaponClass(j); string argstring2 = "限"; var ret = GeneralLib.InStrNotNest(argstring1, argstring2); return ret; }

                    //                                                if (GeneralLib.InStrNotNest(checku.WeaponClass(j), checkwc) > localInStrNotNest())
                    //                                                {
                    //                                                    // 射程範囲内？
                    //                                                    if (checku.IsWeaponClassifiedAs(j, "移動後攻撃可") && checku.Mode != "固定")
                    //                                                    {
                    //                                                        // 合体技は移動後攻撃可能でも移動を前提にしない
                    //                                                        // (移動後の位置では使えない危険性があるため)
                    //                                                        if (checku.IsWeaponClassifiedAs(j, "合") && checku.IsWeaponClassifiedAs(j, "Ｐ"))
                    //                                                        {
                    //                                                            // 移動して攻撃は出来ない
                    //                                                            if (checku.IsTargetWithinRange(j, t))
                    //                                                            {
                    //                                                                sp_prob = sp_prob + 0.1d;
                    //                                                                goto NextAttribute;
                    //                                                            }
                    //                                                        }
                    //                                                        // 移動して攻撃可能
                    //                                                        else if (checku.IsTargetReachable(j, t))
                    //                                                        {
                    //                                                            sp_prob = sp_prob + 0.1d;
                    //                                                            goto NextAttribute;
                    //                                                        }
                    //                                                    }
                    //                                                    // 移動して攻撃は出来ない
                    //                                                    else if (checku.IsTargetWithinRange(j, t))
                    //                                                    {
                    //                                                        sp_prob = sp_prob + 0.1d;
                    //                                                        goto NextAttribute;
                    //                                                    }
                    //                                                }
                    //                                            }
                    //                                        }
                    //                                    }
                    //                                }
                    //                            }
                    //                        }
                    //                    }
                    //                    // 剋属性
                    //                    else if (Strings.Left(wattr, 1) == "剋")
                    //                    {
                    //                        // 特殊効果発動率は黙属性揃えで0.3
                    //                        checkwc = Strings.Mid(wattr, 2);
                    //                        switch (checkwc ?? "")
                    //                        {
                    //                            case "オ":
                    //                                {
                    //                                    if (!withBlock1.IsConditionSatisfied("オーラ使用不能"))
                    //                                    {
                    //                                        if (withBlock1.IsSkillAvailable("オーラ"))
                    //                                        {
                    //                                            sp_prob = sp_prob + 0.3d;
                    //                                        }
                    //                                    }
                    //                                    else
                    //                                    {
                    //                                        goto NextAttribute;
                    //                                    }

                    //                                    break;
                    //                                }

                    //                            case "超":
                    //                                {
                    //                                    if (!withBlock1.IsConditionSatisfied("超能力使用不能"))
                    //                                    {
                    //                                        if (withBlock1.IsSkillAvailable("超能力"))
                    //                                        {
                    //                                            sp_prob = sp_prob + 0.3d;
                    //                                        }
                    //                                    }
                    //                                    else
                    //                                    {
                    //                                        goto NextAttribute;
                    //                                    }

                    //                                    break;
                    //                                }

                    //                            case "シ":
                    //                                {
                    //                                    if (!withBlock1.IsConditionSatisfied("同調率使用不能"))
                    //                                    {
                    //                                        if (withBlock1.IsSkillAvailable("同調率"))
                    //                                        {
                    //                                            sp_prob = sp_prob + 0.3d;
                    //                                        }
                    //                                    }
                    //                                    else
                    //                                    {
                    //                                        goto NextAttribute;
                    //                                    }

                    //                                    break;
                    //                                }

                    //                            case "サ":
                    //                                {
                    //                                    if (!withBlock1.IsConditionSatisfied("超感覚使用不能") && !withBlock1.IsConditionSatisfied("知覚強化使用不能"))
                    //                                    {
                    //                                        if (withBlock1.IsSkillAvailable("超感覚") || withBlock1.IsSkillAvailable("知覚強化"))
                    //                                        {
                    //                                            sp_prob = sp_prob + 0.3d;
                    //                                        }
                    //                                    }
                    //                                    else
                    //                                    {
                    //                                        goto NextAttribute;
                    //                                    }

                    //                                    break;
                    //                                }

                    //                            case "霊":
                    //                                {
                    //                                    if (!withBlock1.IsConditionSatisfied("霊力使用不能"))
                    //                                    {
                    //                                        if (withBlock1.IsSkillAvailable("霊力"))
                    //                                        {
                    //                                            sp_prob = sp_prob + 0.3d;
                    //                                        }
                    //                                    }
                    //                                    else
                    //                                    {
                    //                                        goto NextAttribute;
                    //                                    }

                    //                                    break;
                    //                                }

                    //                            case "術":
                    //                                {
                    //                                    // 術は射撃を魔力と表示するためだけに付いている場合があるため
                    //                                    // 1レベル以下の場合は武器、アビリティを確認
                    //                                    if (!withBlock1.IsConditionSatisfied("術使用不能"))
                    //                                    {
                    //                                        if (withBlock1.SkillLevel("術") > 1d)
                    //                                        {
                    //                                            sp_prob = sp_prob + 0.3d;
                    //                                        }
                    //                                    }
                    //                                    else
                    //                                    {
                    //                                        goto NextAttribute;
                    //                                    }

                    //                                    break;
                    //                                }

                    //                            case "技":
                    //                                {
                    //                                    if (!withBlock1.IsConditionSatisfied("技使用不能"))
                    //                                    {
                    //                                        if (withBlock1.IsSkillAvailable("技"))
                    //                                        {
                    //                                            sp_prob = sp_prob + 0.3d;
                    //                                        }
                    //                                    }
                    //                                    else
                    //                                    {
                    //                                        goto NextAttribute;
                    //                                    }

                    //                                    break;
                    //                                }
                    //                        }

                    //                        bool localIsConditionSatisfied() { object argIndex1 = checkwc + "属性使用不能"; var ret = withBlock1.IsConditionSatisfied(argIndex1); return ret; }

                    //                        if (!localIsConditionSatisfied())
                    //                        {
                    //                            var loopTo8 = withBlock1.CountWeapon();
                    //                            for (j = 1; j <= loopTo8; j++)
                    //                            {
                    //                                if (withBlock1.IsWeaponClassifiedAs(j, checkwc))
                    //                                {
                    //                                    sp_prob = sp_prob + 0.3d;
                    //                                    break;
                    //                                }
                    //                            }

                    //                            if (j > withBlock1.CountWeapon())
                    //                            {
                    //                                var loopTo9 = withBlock1.CountAbility();
                    //                                for (j = 1; j <= loopTo9; j++)
                    //                                {
                    //                                    if (withBlock1.IsAbilityClassifiedAs(j, checkwc))
                    //                                    {
                    //                                        sp_prob = sp_prob + 0.3d;
                    //                                        break;
                    //                                    }
                    //                                }
                    //                            }
                    //                        }
                    //                    }

                    //                    break;
                    //                }
                    //        }

                    //    NextAttribute:
                    //        ;
                    //    }
                }

                if (sp_prob > 1d)
                {
                    sp_prob = Math.Sqrt(sp_prob);
                }

                sp_prob = sp_prob * ct_prob;

                // バリア等で攻撃が防がれてしまう場合は特殊効果は発動しない
                if (selectedWeapon.WeaponPower("") > 0 && dmg == 0 && !selectedWeapon.IsWeaponClassifiedAs("無"))
                {
                    sp_prob = 0d;
                }

                // 必ず発動する特殊効果を考慮
                if (selectedWeapon.IsWeaponClassifiedAs("吸"))
                {
                    if (u.HP < u.MaxHP)
                    {
                        sp_prob = sp_prob + 25 * dmg / t.MaxHP;
                    }
                }

                if (selectedWeapon.IsWeaponClassifiedAs("減"))
                {
                    sp_prob = sp_prob + 50 * dmg / t.MaxHP;
                }

                if (selectedWeapon.IsWeaponClassifiedAs("奪"))
                {
                    sp_prob = sp_prob + 50 * dmg / t.MaxHP;
                }

                // 先制攻撃の場合は特殊効果を有利に判定
                if (Strings.InStr(amode, "反撃") > 0)
                {
                    if (selectedWeapon.IsWeaponClassifiedAs("先") || u.UsedCounterAttack < u.MaxCounterAttack())
                    {
                        sp_prob = 1.5d * sp_prob;
                    }
                }

                if (sp_prob > 100d)
                {
                    sp_prob = 100d;
                }

                // ＣＴ率が低い場合は特殊効果のみの攻撃を重視しない
                if (dmg == 0 && ct_prob < 30)
                {
                    sp_prob = sp_prob / 5d;
                }

                // ダメージが与えられない武器は使用しない
                if (dmg == 0 && sp_prob == 0d)
                {
                    goto NextWeapon;
                }

                if (prob > 0)
                {
                    if (sp_prob > 0d)
                    {
                        // 特殊効果の影響を加味してダメージの期待値を計算
                        exp_dmg = (int)(dmg + (GeneralLib.MaxLng(t.HP - dmg, 0) * sp_prob) / 100);
                    }
                    else
                    {
                        // クリティカルの影響を加味してダメージの期待値を計算
                        if (Expression.IsOptionDefined("ダメージ倍率低下"))
                        {
                            if (selectedWeapon.IsWeaponClassifiedAs("痛"))
                            {
                                exp_dmg = (int)(dmg + (0.1d * selectedWeapon.WeaponLevel("痛") * dmg * ct_prob) / 100);
                            }
                            else
                            {
                                exp_dmg = (int)(dmg + (0.2d * dmg * ct_prob) / 100);
                            }
                        }
                        else
                        {
                            if (selectedWeapon.IsWeaponClassifiedAs("痛"))
                            {
                                exp_dmg = (int)(dmg + (0.25d * selectedWeapon.WeaponLevel("痛") * dmg * ct_prob) / 100);
                            }
                            else
                            {
                                exp_dmg = (int)(dmg + (0.5d * dmg * ct_prob) / 100);
                            }
                        }
                    }

                    exp_dmg = (int)(exp_dmg * 0.01d * GeneralLib.MinLng(prob, 100));
                }
                else
                {
                    // 命中が当たらない場合は期待値を思い切り下げる
                    prob = 1;
                    exp_dmg = (int)((dmg / 10 + (GeneralLib.MaxLng(t.HP - dmg / 10, 0) * sp_prob) / 100) / 10L);
                }

                // サポートによるダメージを期待値に追加
                if (!is_move_attack)
                {
                    exp_dmg = exp_dmg + support_exp_dmg;
                }

                // 敵の破壊確率を計算
                destroy_prob = 0;
                if (t.Party == "味方" && !t.IsFeatureAvailable("防御不可"))
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
                    if (selectedWeapon.IsWeaponClassifiedAs("先") || u.UsedCounterAttack < u.MaxCounterAttack())
                    {
                        destroy_prob = (int)(1.5d * destroy_prob);
                    }
                }

                // ＥＮ消耗攻撃の使用は慎重に
                if (selectedWeapon.IsWeaponClassifiedAs("消"))
                {
                    if (u.Party == "味方")
                    {
                        // 自動反撃モードかどうか
                        if (SystemConfig.AutoDefense)
                        {
                            goto NextWeapon;
                        }
                    }
                    // 敵ユニットは相手を倒せるときにしかＥＮ消耗攻撃を使わない
                    else if (destroy_prob == 0 && u.BossRank < 0)
                    {
                        goto NextWeapon;
                    }
                }

                if (destroy_prob >= 100)
                {
                    // 破壊確率が100%の場合はコストの低さを優先
                    // (確率が同じ場合は番号が低い武器を使用)
                    if (u.Party == "味方" || u.Party == "ＮＰＣ")
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
                if (u.Weapon(SelectWeaponRet).WeaponAdaption(t.Area) == 0d)
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
        public string SelectDefense(Unit u, int w, Unit t, int tw)
        {
            var uw = u.Weapon(w);
            if (uw == null)
            {
                return "";
            }

            string SelectDefenseRet = "";

            // マップ攻撃に対しては防御行動が取れない
            if (uw.IsWeaponClassifiedAs("Ｍ"))
            {
                return SelectDefenseRet;
            }

            // 踊っている場合は回避扱い
            if (t.IsConditionSatisfied("踊り"))
            {
                SelectDefenseRet = "回避";
                return SelectDefenseRet;
            }

            // 狂戦士状態の際は防御行動を取らない
            if (t.IsConditionSatisfied("狂戦士"))
            {
                return SelectDefenseRet;
            }

            // 無防備状態のユニットは防御行動が取れない
            if (t.IsUnderSpecialPowerEffect("無防備"))
            {
                return SelectDefenseRet;
            }

            if (t.Party != "味方")
            {
                // 「敵ユニット防御使用」オプションを選択している場合にのみ敵ユニットは
                // 防御行動を行う
                if (!Expression.IsOptionDefined("敵ユニット防御使用"))
                {
                    return SelectDefenseRet;
                }

                // 防御行動を使ってくるのは技量が160以上のザコでないパイロットのみ
                {
                    var withBlock1 = t.MainPilot();
                    if (Strings.InStr(withBlock1.Name, "(ザコ)") > 0 || withBlock1.TacticalTechnique() < 160)
                    {
                        return SelectDefenseRet;
                    }
                }
            }

            // 行動不能？
            if (t.MaxAction() == 0)
            {
                // チャージ中、消耗中は常に防御、それ以外の場合は防御行動が取れない
                if (t.IsConditionSatisfied("チャージ") || t.IsConditionSatisfied("消耗"))
                {
                    SelectDefenseRet = "防御";
                }

                return SelectDefenseRet;
            }

            // 相手の攻撃のダメージ・命中率を算出
            var dmg = uw.ExpDamage(t, true);
            var prob = GeneralLib.MinLng(uw.HitProbability(t, true), 100);

            // ダミーを持っている場合、相手の攻撃は無効
            if (t.IsFeatureAvailable("ダミー") && t.ConditionLevel("ダミー破壊") < t.FeatureLevel("ダミー"))
            {
                prob = 0;
            }

            // サポートガードされる場合も相手の攻撃は無効
            if (t.LookForSupportGuard(u, uw) is object)
            {
                prob = 0;
            }

            var tuw = t.Weapon(tw);
            int tdmg = 0;
            int tprob = 0;
            // 反撃のダメージ・命中率を算出
            if (tw > 0)
            {
                tdmg = tuw.ExpDamage(u, true);
                tprob = GeneralLib.MinLng(tuw.HitProbability(u, true), 100);

                // ダミーを持っている場合は反撃は無効
                if (u.IsFeatureAvailable("ダミー") && u.ConditionLevel("ダミー破壊") < u.FeatureLevel("ダミー"))
                {
                    prob = 0;
                }
            }

            // 相手の攻撃の効果とこちらの反撃の効果を比較
            var is_target_inferior = false;
            if (t.Party == "味方")
            {
                // 味方ユニットの場合、相手の攻撃によるダメージの方が多い場合は防御
                if (dmg * prob > tdmg * tprob && tdmg < u.HP)
                {
                    is_target_inferior = true;
                }

                // 気合の一撃は防御を優先し、やり過ごす
                if (u.IsUnderSpecialPowerEffect("ダメージ増加"))
                {
                    if (2 * dmg * prob > tdmg * tprob && tdmg < u.HP)
                    {
                        is_target_inferior = true;
                    }
                }
            }
            else
            {
                // 敵ユニットの場合でも相手の攻撃によるダメージの方が２倍以上多い場合は防御
                if (dmg * prob / 2 > tdmg * tprob && tdmg < u.HP)
                {
                    is_target_inferior = true;
                }

                // 気合の一撃は防御を優先し、やり過ごす
                if (u.IsUnderSpecialPowerEffect("ダメージ増加"))
                {
                    if (dmg * prob > tdmg * tprob && tdmg < u.HP)
                    {
                        is_target_inferior = true;
                    }
                }
            }

            // あと一撃で破壊されてしまう場合は必ず防御
            // (命中率が低い場合を除く)
            if (dmg >= t.HP && prob > 10)
            {
                is_target_inferior = true;
            }

            if (tw > 0)
            {
                // 先制攻撃可能？
                if (!tuw.IsWeaponClassifiedAs("後"))
                {
                    if (tuw.IsWeaponClassifiedAs("先") || uw.IsWeaponClassifiedAs("後") || t.MaxCounterAttack() > t.UsedCounterAttack)
                    {
                        if (tdmg >= u.HP && tprob > 70)
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
            if (dmg > t.HP && dmg / 2 < t.HP && !t.IsFeatureAvailable("防御不可") && !uw.IsWeaponClassifiedAs("殺"))
            {
                SelectDefenseRet = "防御";
                return SelectDefenseRet;
            }

            // 相手の命中率が低い場合は回避
            if (prob < 50 && !t.IsFeatureAvailable("回避不可") && !t.IsConditionSatisfied("移動不能"))
            {
                SelectDefenseRet = "回避";
                return SelectDefenseRet;
            }

            // 防御すれば一撃死をまぬがれる場合は防御
            if (dmg / 2 < t.HP && !t.IsFeatureAvailable("防御不可") && !uw.IsWeaponClassifiedAs("殺"))
            {
                SelectDefenseRet = "防御";
                return SelectDefenseRet;
            }

            // どうしようもないのでとりあえず回避
            if (!t.IsFeatureAvailable("回避不可") && !t.IsConditionSatisfied("移動不能"))
            {
                SelectDefenseRet = "回避";
                return SelectDefenseRet;
            }

            // 回避も出来ないので防御……
            if (!t.IsFeatureAvailable("防御不可"))
            {
                SelectDefenseRet = "防御";
            }

            return SelectDefenseRet;
        }

        // ユニット u がターゲット t に反撃可能か？
        public bool IsAbleToCounterAttack(Unit u, Unit t)
        {
            return u.Weapons.Any(uw => uw.CanUseFor(WeaponListMode.Counter, t));
        }

        // 最も近い敵ユニットを探す
        public Unit SearchNearestEnemy(Unit u)
        {
            Unit SearchNearestEnemyRet = default;
            int distance;
            Unit t;
            distance = 1000;
            for (var x = 1; x <= Map.MapWidth; x++)
            {
                for (var y = 1; y <= Map.MapHeight; y++)
                {
                    t = Map.MapDataForUnit[x, y];
                    if (t is null)
                    {
                        goto NexLoop;
                    }

                    // もっと近くにいる敵を発見済み？
                    if (distance <= (Math.Abs((u.x - t.x)) + Math.Abs((u.y - t.y))))
                    {
                        goto NexLoop;
                    }

                    // 敵？
                    if (u.IsAlly(t))
                    {
                        goto NexLoop;
                    }

                    // 特定の陣営のみを狙う思考モードの場合
                    if (u.Mode == "味方" || u.Mode == "ＮＰＣ" || u.Mode == "敵" || u.Mode == "中立")
                    {
                        if ((t.Party ?? "") != (u.Mode ?? ""))
                        {
                            goto NexLoop;
                        }
                    }

                    // 目視不能？
                    if (t.IsUnderSpecialPowerEffect("隠れ身") || t.Area == "地中")
                    {
                        goto NexLoop;
                    }

                    // ステルス状態にあれば遠くからは発見できない
                    if (t.IsFeatureAvailable("ステルス")
                        && !t.IsConditionSatisfied("ステルス無効")
                        && !u.IsFeatureAvailable("ステルス無効化"))
                    {
                        if (t.IsFeatureLevelSpecified("ステルス"))
                        {
                            if (Math.Abs((u.x - t.x)) + Math.Abs((u.y - t.y)) > t.FeatureLevel("ステルス"))
                            {
                                goto NexLoop;
                            }
                        }
                        else if (Math.Abs((u.x - t.x)) + Math.Abs((u.y - t.y)) > 3)
                        {
                            goto NexLoop;
                        }
                    }

                    // ターゲットを発見
                    SearchNearestEnemyRet = t;
                    distance = (Math.Abs((u.x - t.x)) + Math.Abs((u.y - t.y)));
                NexLoop:
                    ;
                }
            }

            return SearchNearestEnemyRet;
        }

        // 最も近い敵ユニットへの距離を返す
        private int DistanceFromNearestEnemy(Unit u)
        {
            Unit t;
            t = SearchNearestEnemy(u);
            if (t != null)
            {
                return (Math.Abs((u.x - t.x)) + Math.Abs((u.y - t.y)));
            }
            else
            {
                return 1000;
            }
        }
    }
}
