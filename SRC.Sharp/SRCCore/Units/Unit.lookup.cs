// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.Pilots;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SRCCore.Units
{
    // === 各種処理を行うための関数＆サブルーチン ===
    public partial class Unit
    {
        // 出撃中？
        public bool IsOperational()
        {
            if (Status == "出撃")
            {
                return true;
            }

            return OtherForms.Any(x => x.Status == "出撃");
        }

        // ユニットがユニット nm と同一？
        public bool IsEqual(string nm)
        {
            if ((Name ?? "") == (nm ?? ""))
            {
                return true;
            }

            return OtherForms.Any(x => (x.Name ?? "") == (nm ?? ""));
        }

        // 人間ユニットかどうか判定
        public bool IsHero()
        {
            return Strings.Left(Data.Class, 1) == "(";
        }

        // (tx,ty)の地点の周囲に「連携攻撃」を行ってくれるユニットがいるかどうかを判定
        public Unit LookForAttackHelp(int tx, int ty)
        {
            return null;
            // TODO Impl LookForAttackHelp
            //Unit LookForAttackHelpRet = default;
            //Unit u;
            //int i;
            //for (i = 1; i <= 4; i++)
            //{
            //    // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    u = null;
            //    switch (i)
            //    {
            //        case 1:
            //            {
            //                if (tx > 1)
            //                {
            //                    u = Map.MapDataForUnit[tx - 1, ty];
            //                }

            //                break;
            //            }

            //        case 2:
            //            {
            //                if (tx < Map.MapWidth)
            //                {
            //                    u = Map.MapDataForUnit[tx + 1, ty];
            //                }

            //                break;
            //            }

            //        case 3:
            //            {
            //                if (ty > 1)
            //                {
            //                    u = Map.MapDataForUnit[tx, ty - 1];
            //                }

            //                break;
            //            }

            //        case 4:
            //            {
            //                if (ty < Map.MapHeight)
            //                {
            //                    u = Map.MapDataForUnit[tx, ty + 1];
            //                }

            //                break;
            //            }
            //    }

            //    // ユニットがいる？
            //    if (u is null)
            //    {
            //        goto NextLoop;
            //    }

            //    // ユニットが敵でない？
            //    if (IsEnemy(u))
            //    {
            //        goto NextLoop;
            //    }
            //    // 信頼度を満たしている？
            //    if (GeneralLib.Dice(10) > u.MainPilot().Relation(MainPilot()))
            //    {
            //        goto NextLoop;
            //    }

            //    // 行動可能？
            //    if (u.MaxAction() == 0)
            //    {
            //        goto NextLoop;
            //    }

            //    // 正常な判断力がある？
            //    // XXX 場面で参照状態違わん？
            //    if (u.IsConditionSatisfied("混乱") || u.IsConditionSatisfied("暴走") || u.IsConditionSatisfied("魅了") || u.IsConditionSatisfied("憑依") || u.IsConditionSatisfied("恐怖") || u.IsConditionSatisfied("狂戦士"))
            //    {
            //        goto NextLoop;
            //    }

            //    // メッセージが登録されている？
            //    bool localIsMessageDefined() { string argmain_situation = "連携攻撃(" + u.MainPilot().Name + ")"; var ret = IsMessageDefined(argmain_situation, true); return ret; }

            //    bool localIsMessageDefined1() { string argmain_situation = "連携攻撃(" + u.MainPilot().get_Nickname(false) + ")"; var ret = IsMessageDefined(argmain_situation, true); return ret; }

            //    if (!localIsMessageDefined() && !localIsMessageDefined1())
            //    {
            //        goto NextLoop;
            //    }

            //    // 見つかった
            //    LookForAttackHelpRet = u;
            //    return LookForAttackHelpRet;
            //NextLoop:
            //    ;
            //}

            //// 見つからなかった
            //// UPGRADE_NOTE: オブジェクト LookForAttackHelp をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //LookForAttackHelpRet = null;
            //return LookForAttackHelpRet;
        }

        // tからの攻撃に対して「かばう」を行ってくれるユニットがいるかどうか判定
        public Unit LookForGuardHelp(Unit t, UnitWeapon tw, bool is_critical)
        {
            return null;
            // TODO Impl LookForGuardHelp
            //Unit LookForGuardHelpRet = default;
            //Unit u;
            //int i;
            //int dmg;
            //double ratio;
            //int ux, uy;
            //string uarea;
            //for (i = 1; i <= 4; i++)
            //{
            //    // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    u = null;
            //    switch (i)
            //    {
            //        case 1:
            //            {
            //                if (x > 1)
            //                {
            //                    u = Map.MapDataForUnit[x - 1, y];
            //                }

            //                break;
            //            }

            //        case 2:
            //            {
            //                if (x < Map.MapWidth)
            //                {
            //                    u = Map.MapDataForUnit[x + 1, y];
            //                }

            //                break;
            //            }

            //        case 3:
            //            {
            //                if (y > 1)
            //                {
            //                    u = Map.MapDataForUnit[x, y - 1];
            //                }

            //                break;
            //            }

            //        case 4:
            //            {
            //                if (y < Map.MapHeight)
            //                {
            //                    u = Map.MapDataForUnit[x, y + 1];
            //                }

            //                break;
            //            }
            //    }

            //    // ユニットがいる？
            //    if (u is null)
            //    {
            //        goto NextLoop;
            //    }

            //    // ユニットが敵でない？
            //    if (IsEnemy(u))
            //    {
            //        goto NextLoop;
            //    }

            //    {
            //        var withBlock = u;
            //        // 信頼度を満たしている？
            //        if (GeneralLib.Dice(10) > withBlock.MainPilot().Relation(MainPilot()))
            //        {
            //            goto NextLoop;
            //        }

            //        // 行動可能？
            //        if (withBlock.MaxAction() == 0)
            //        {
            //            goto NextLoop;
            //        }

            //        // 正常な判断力がある？
            //        if (withBlock.IsConditionSatisfied("混乱") || withBlock.IsConditionSatisfied("暴走") || withBlock.IsConditionSatisfied("魅了") || withBlock.IsConditionSatisfied("憑依") || withBlock.IsConditionSatisfied("恐怖") || withBlock.IsConditionSatisfied("狂戦士"))
            //        {
            //            goto NextLoop;
            //        }

            //        // メッセージが登録されている？
            //        bool localIsMessageDefined() { string argmain_situation = "かばう(" + MainPilot().Name + ")"; var ret = withBlock.IsMessageDefined(argmain_situation, true); return ret; }

            //        bool localIsMessageDefined1() { string argmain_situation = "かばう(" + MainPilot().get_Nickname(false) + ")"; var ret = withBlock.IsMessageDefined(argmain_situation, true); return ret; }

            //        if (!localIsMessageDefined() && !localIsMessageDefined1())
            //        {
            //            goto NextLoop;
            //        }

            //        // 援護相手のユニットのいる地形に進入可能？
            //        if ((Area ?? "") != (withBlock.Area ?? ""))
            //        {
            //            switch (Area ?? "")
            //            {
            //                case "空中":
            //                    {
            //                        if (withBlock.get_Adaption(1) == 0)
            //                        {
            //                            goto NextLoop;
            //                        }

            //                        break;
            //                    }

            //                case "地上":
            //                    {
            //                        if (withBlock.get_Adaption(2) == 0)
            //                        {
            //                            goto NextLoop;
            //                        }

            //                        break;
            //                    }

            //                case "水中":
            //                case "水上":
            //                    {
            //                        if (withBlock.get_Adaption(3) == 0)
            //                        {
            //                            goto NextLoop;
            //                        }

            //                        break;
            //                    }

            //                case "宇宙":
            //                    {
            //                        if (Map.TerrainClass(x, y) == "月面")
            //                        {
            //                            if (!withBlock.IsTransAvailable("空") && !withBlock.IsTransAvailable("宇宙"))
            //                            {
            //                                goto NextLoop;
            //                            }
            //                        }
            //                        else if (withBlock.get_Adaption(4) == 0)
            //                        {
            //                            goto NextLoop;
            //                        }

            //                        break;
            //                    }
            //            }
            //        }

            //        // ダメージを算出
            //        if (withBlock.IsFeatureAvailable("防御不可") || t.IsWeaponClassifiedAs(tw, "殺"))
            //        {
            //            ratio = 1d;
            //        }
            //        else
            //        {
            //            ratio = 0.5d;
            //        }

            //        if (is_critical)
            //        {
            //            if (Expression.IsOptionDefined("ダメージ倍率低下"))
            //            {
            //                ratio = 1.2d * ratio;
            //            }
            //            else
            //            {
            //                ratio = 1.5d * ratio;
            //            }
            //        }

            //        ux = withBlock.x;
            //        uy = withBlock.y;
            //        uarea = withBlock.Area;
            //        withBlock.x = x;
            //        withBlock.y = y;
            //        withBlock.Area = Area;
            //        dmg = t.ExpDamage(tw, u, true, ratio);
            //        withBlock.x = ux;
            //        withBlock.y = uy;
            //        withBlock.Area = uarea;

            //        // 自分が倒されてしまうような場合はかばわない
            //        if (dmg >= withBlock.HP)
            //        {
            //            goto NextLoop;
            //        }
            //    }

            //    // 見つかった
            //    LookForGuardHelpRet = u;
            //    return LookForGuardHelpRet;
            //NextLoop:
            //    ;
            //}

            //// 見つからなかった
            //// UPGRADE_NOTE: オブジェクト LookForGuardHelp をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //LookForGuardHelpRet = null;
            //return LookForGuardHelpRet;
        }

        // 最大サポートアタック回数
        public int MaxSupportAttack()
        {
            var p = MainPilot();
            return GeneralLib.MaxLng(
                    (int)p.SkillLevel("援護攻撃", ref_mode: ""), (int)p.SkillLevel("援護", ref_mode: ""));
        }

        // 最大サポートガード回数
        public int MaxSupportGuard()
        {
            var p = MainPilot();
            return GeneralLib.MaxLng(
                    (int)p.SkillLevel("援護防御", ref_mode: ""), (int)p.SkillLevel("援護", ref_mode: ""));
        }

        // 最大同時援護攻撃回数
        public int MaxSyncAttack()
        {
            return (int)MainPilot().SkillLevel("統率", ref_mode: "");
        }

        // 最大カウンター攻撃回数
        public int MaxCounterAttack()
        {
            int MaxCounterAttackRet = default;
            {
                var p = MainPilot();
                MaxCounterAttackRet = (int)p.SkillLevel("カウンター", ref_mode: "");
                if (p.IsSkillAvailable("先手必勝"))
                {
                    if (GeneralLib.LLength(p.SkillData("先手必勝")) == 2)
                    {
                        if (p.Morale >= GeneralLib.StrToLng(GeneralLib.LIndex("先手必勝", 2)))
                        {
                            MaxCounterAttackRet = 1000;
                        }
                    }
                    else if (p.Morale >= 120)
                    {
                        MaxCounterAttackRet = 1000;
                    }
                }
            }

            return MaxCounterAttackRet;
        }

        // ユニット t に対して周囲にサポートアタックを行ってくれるユニットがいるかどうかを判定
        public Unit LookForSupportAttack(Unit t)
        {
            Unit LookForSupportAttackRet = default;
            // 正常な判断が可能？
            if (IsConditionSatisfied("混乱"))
            {
                return LookForSupportAttackRet;
            }

            // 同士討ちの場合はどちらに荷担すべきか分からないので……
            if (t is object)
            {
                if ((Party ?? "") == (t.Party ?? ""))
                {
                    return LookForSupportAttackRet;
                }
            }

            var team = MainPilot().SkillData("チーム");
            var max_wpower = -1;
            foreach (var u in Map.AdjacentUnit(this))
            {
                // サポートアタック数が残っている？
                if (u.MaxSupportAttack() <= u.UsedSupportAttack)
                {
                    goto NextUnit;
                }

                // 行動数が残っている？
                if (u.Action == 0)
                {
                    goto NextUnit;
                }

                // 正常な判断が可能？
                if (u.IsConditionSatisfied("混乱") || u.IsConditionSatisfied("暴走") || u.IsConditionSatisfied("恐怖") || u.IsConditionSatisfied("狂戦士") || u.IsConditionSatisfied("踊り"))
                {
                    goto NextUnit;
                }

                // 味方？
                switch (u.Party ?? "")
                {
                    case "ＮＰＣ":
                        {
                            switch (Party ?? "")
                            {
                                case "敵":
                                case "中立":
                                    {
                                        goto NextUnit;
                                    }
                            }

                            break;
                        }

                    default:
                        {
                            if ((u.Party ?? "") != (Party ?? ""))
                            {
                                goto NextUnit;
                            }

                            break;
                        }
                }

                // 同じチームに属している？
                var uteam = u.MainPilot().SkillData("チーム");
                if ((team ?? "") != (uteam ?? "") && !string.IsNullOrEmpty(uteam))
                {
                    goto NextUnit;
                }

                // まだターゲットが特定されていない？
                if (t is null)
                {
                    LookForSupportAttackRet = u;
                    return LookForSupportAttackRet;
                }

                // 攻撃可能？
                // 高い威力の武器はリストの最後の方にあることが多いので後ろから判定
                foreach (var w in u.Weapons.Reverse())
                {
                    // 攻撃力が今まで見つかった武器以下の場合は選考外
                    if (w.WeaponPower(t.Area) <= max_wpower)
                    {
                        goto NextWeapon;
                    }

                    // サポートアタックに利用可能？
                    if (w.IsWeaponClassifiedAs("Ｍ"))
                    {
                        goto NextWeapon;
                    }

                    if (w.IsWeaponClassifiedAs("合"))
                    {
                        goto NextWeapon;
                    }

                    if (!w.IsWeaponAvailable("移動前"))
                    {
                        goto NextWeapon;
                    }

                    if (!w.IsTargetWithinRange(t))
                    {
                        goto NextWeapon;
                    }

                    if (u.Party == "味方" && u.Party0 == "味方")
                    {
                        // 味方ユニットは自爆攻撃をサポートアタックには使用しない
                        if (w.IsWeaponClassifiedAs("自"))
                        {
                            goto NextWeapon;
                        }

                        // 自動反撃の場合、味方ユニットは残弾数が少ない武器を使用しない
                        if (SRC.SystemConfig.AutoDefense)
                        {
                            if (!w.IsWeaponClassifiedAs("永"))
                            {
                                if (w.Bullet() == 1 || w.MaxBullet() == 2 || w.MaxBullet() == 3)
                                {
                                    goto NextWeapon;
                                }
                            }

                            if (w.WeaponENConsumption() > 0)
                            {
                                if (w.WeaponENConsumption() >= u.EN / 2 || w.WeaponENConsumption() >= u.MaxEN / 4)
                                {
                                    goto NextWeapon;
                                }
                            }

                            if (w.IsWeaponClassifiedAs("尽"))
                            {
                                goto NextWeapon;
                            }
                        }
                    }

                    // 援護攻撃用の武器が見つかった
                    max_wpower = w.WeaponPower(t.Area);
                    LookForSupportAttackRet = u;
                NextWeapon:
                    ;
                }

            NextUnit:
                ;
            }

            return LookForSupportAttackRet;
        }

        // ユニット t からの攻撃に対して周囲にサポートガードを行ってくれるユニットが
        // いるかどうかを判定
        public Unit LookForSupportGuard(Unit t, UnitWeapon tw)
        {
            return null;
            // TODO Impl LookForSupportGuard
            //Unit LookForSupportGuardRet = default;
            //Unit u;
            //int i;
            //int my_dmg, dmg;
            //double ratio;
            //int ux, uy;
            //string uarea;
            //string team, uteam;

            //// マップ攻撃はサポートガード不能
            //if (t.IsWeaponClassifiedAs(tw, "Ｍ"))
            //{
            //    return LookForSupportGuardRet;
            //}

            //// スペシャルパワーでサポートガードが無効化されている？
            //if (t.IsUnderSpecialPowerEffect("サポートガード無効化"))
            //{
            //    return LookForSupportGuardRet;
            //}

            //// 同士討ちの場合は本来の陣営に属するユニットのみを守る
            //if ((Party ?? "") == (t.Party ?? "") || Party == "ＮＰＣ" && t.Party == "味方")
            //{
            //    if ((Party ?? "") != (Party0 ?? ""))
            //    {
            //        return LookForSupportGuardRet;
            //    }

            //    if (IsConditionSatisfied("暴走"))
            //    {
            //        return LookForSupportGuardRet;
            //    }
            //}

            //// 自分が受けるダメージを求めておく
            //my_dmg = t.ExpDamage(tw, this, true);

            //// かばう必要がない？
            //// 手動反撃で味方の場合はダメージにかかわらず常にかばう
            //// UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //if (Party != "味方" || SystemConfig.AutoDefense)
            //{
            //    if (t.IsNormalWeapon(tw))
            //    {
            //        if (my_dmg < MaxHP / 20 && my_dmg < HP / 5)
            //        {
            //            return LookForSupportGuardRet;
            //        }
            //    }
            //    else
            //    {
            //        if (t.CriticalProbability(tw, this) > 0)
            //        {
            //            return LookForSupportGuardRet;
            //        }
            //    }
            //}

            //team = MainPilot().SkillData("チーム");
            //for (i = 1; i <= 4; i++)
            //{
            //    // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    u = null;
            //    switch (i)
            //    {
            //        case 1:
            //            {
            //                if (x > 1)
            //                {
            //                    u = Map.MapDataForUnit[x - 1, y];
            //                }

            //                break;
            //            }

            //        case 2:
            //            {
            //                if (x < Map.MapWidth)
            //                {
            //                    u = Map.MapDataForUnit[x + 1, y];
            //                }

            //                break;
            //            }

            //        case 3:
            //            {
            //                if (y > 1)
            //                {
            //                    u = Map.MapDataForUnit[x, y - 1];
            //                }

            //                break;
            //            }

            //        case 4:
            //            {
            //                if (y < Map.MapHeight)
            //                {
            //                    u = Map.MapDataForUnit[x, y + 1];
            //                }

            //                break;
            //            }
            //    }

            //    if (u is null)
            //    {
            //        goto NextUnit;
            //    }

            //    if (ReferenceEquals(u, t))
            //    {
            //        goto NextUnit;
            //    }

            //    {
            //        var withBlock = u;
            //        // サポートガード数が残っている？
            //        if (withBlock.MaxSupportGuard() <= withBlock.UsedSupportGuard)
            //        {
            //            goto NextUnit;
            //        }

            //        // 行動可能？
            //        if (withBlock.MaxAction() == 0)
            //        {
            //            goto NextUnit;
            //        }

            //        // スペシャルパワーでサポートガードが封印されている？
            //        if (withBlock.IsUnderSpecialPowerEffect("サポートガード不能"))
            //        {
            //            goto NextUnit;
            //        }

            //        // 正常な判断が可能？
            //        if (withBlock.IsConditionSatisfied("混乱") || withBlock.IsConditionSatisfied("暴走") || withBlock.IsConditionSatisfied("恐怖") || withBlock.IsConditionSatisfied("狂戦士"))
            //        {
            //            goto NextUnit;
            //        }

            //        // ＨＰが高いほうを優先
            //        if (LookForSupportGuardRet is object)
            //        {
            //            if (LookForSupportGuardRet.HP >= withBlock.HP)
            //            {
            //                goto NextUnit;
            //            }
            //        }

            //        // 味方？
            //        switch (withBlock.Party ?? "")
            //        {
            //            case "味方":
            //                {
            //                    if (Expression.IsOptionDefined("対ＮＰＣサポートガード無効"))
            //                    {
            //                        if ((withBlock.Party ?? "") != (Party ?? ""))
            //                        {
            //                            goto NextUnit;
            //                        }
            //                    }
            //                    else
            //                    {
            //                        switch (Party ?? "")
            //                        {
            //                            case "敵":
            //                            case "中立":
            //                                {
            //                                    goto NextUnit;
            //                                    break;
            //                                }
            //                        }
            //                    }

            //                    break;
            //                }

            //            case "ＮＰＣ":
            //                {
            //                    switch (Party ?? "")
            //                    {
            //                        case "敵":
            //                        case "中立":
            //                            {
            //                                goto NextUnit;
            //                                break;
            //                            }
            //                    }

            //                    break;
            //                }

            //            default:
            //                {
            //                    if ((withBlock.Party ?? "") != (Party ?? ""))
            //                    {
            //                        goto NextUnit;
            //                    }

            //                    break;
            //                }
            //        }

            //        // 同じチームに属している？
            //        uteam = withBlock.MainPilot().SkillData("チーム");
            //        if ((team ?? "") != (uteam ?? "") && !string.IsNullOrEmpty(uteam))
            //        {
            //            goto NextUnit;
            //        }

            //        // 援護相手のユニットのいる地形に進入可能？
            //        if ((Area ?? "") != (withBlock.Area ?? ""))
            //        {
            //            switch (Area ?? "")
            //            {
            //                case "空中":
            //                    {
            //                        if (!withBlock.IsTransAvailable("空"))
            //                        {
            //                            goto NextUnit;
            //                        }

            //                        break;
            //                    }

            //                case "地上":
            //                    {
            //                        if (withBlock.get_Adaption(2) == 0)
            //                        {
            //                            goto NextUnit;
            //                        }

            //                        break;
            //                    }

            //                case "水中":
            //                case "水上":
            //                    {
            //                        if (withBlock.get_Adaption(3) == 0)
            //                        {
            //                            goto NextUnit;
            //                        }

            //                        break;
            //                    }

            //                case "宇宙":
            //                    {
            //                        if (withBlock.get_Adaption(4) == 0)
            //                        {
            //                            goto NextUnit;
            //                        }

            //                        break;
            //                    }
            //            }
            //        }

            //        // 機械をかばうのは機械のみ
            //        if (MainPilot().Personality == "機械")
            //        {
            //            if (withBlock.MainPilot().Personality != "機械")
            //            {
            //                goto NextUnit;
            //            }
            //        }

            //        // ダメージを算出
            //        if (withBlock.IsFeatureAvailable("防御不可") || t.IsWeaponClassifiedAs(tw, "殺"))
            //        {
            //            ratio = 1d;
            //        }
            //        else
            //        {
            //            ratio = 0.5d;
            //        }

            //        if (t.IsNormalWeapon(tw))
            //        {
            //            // ダメージは常に最悪の状況を考えてクリティカル時の値に
            //            if (Expression.IsOptionDefined("ダメージ倍率低下"))
            //            {
            //                ratio = 1.2d * ratio;
            //            }
            //            else
            //            {
            //                ratio = 1.5d * ratio;
            //            }
            //        }

            //        ux = withBlock.x;
            //        uy = withBlock.y;
            //        uarea = withBlock.Area;
            //        withBlock.x = x;
            //        withBlock.y = y;
            //        withBlock.Area = Area;
            //        dmg = t.ExpDamage(tw, u, true, ratio);
            //        withBlock.x = ux;
            //        withBlock.y = uy;
            //        withBlock.Area = uarea;

            //        // ボスはザコを見殺しにする！
            //        if (withBlock.BossRank > BossRank)
            //        {
            //            // 被るダメージが少ない場合は別だけど……
            //            if (dmg >= withBlock.MaxHP / 20 || dmg >= withBlock.HP / 5)
            //            {
            //                goto NextUnit;
            //            }
            //        }

            //        // 自分が倒されてしまうような場合はかばわない(クリティカルを含む)
            //        if (dmg >= withBlock.HP)
            //        {
            //            // ボスは例外……
            //            if (withBlock.BossRank >= BossRank)
            //            {
            //                goto NextUnit;
            //            }
            //        }
            //    }

            //    LookForSupportGuardRet = u;
            //NextUnit:
            //    ;
            //}

            //return LookForSupportGuardRet;
        }

        // (tx,ty)の地点の周囲にサポートを行ってくれるユニットがいるかどうかを判定。
        public int LookForSupport(int tx, int ty, bool for_attack = false)
        {
            return 0;
            // TODO Impl LookForSupport
            //int LookForSupportRet = default;
            //Unit u;
            //int i;
            //var do_support = default(bool);
            //string team, uteam;
            //{
            //    var withBlock = MainPilot();
            //    // 自分自身がサポートを行うことが出来るか？
            //    if (withBlock.IsSkillAvailable("援護") || withBlock.IsSkillAvailable("援護攻撃") || withBlock.IsSkillAvailable("援護防御") || withBlock.IsSkillAvailable("指揮") || withBlock.IsSkillAvailable("広域サポート"))
            //    {
            //        do_support = true;
            //    }

            //    team = withBlock.SkillData("チーム");
            //}

            //for (i = 1; i <= 4; i++)
            //{
            //    // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    u = null;
            //    switch (i)
            //    {
            //        case 1:
            //            {
            //                if (tx > 1)
            //                {
            //                    u = Map.MapDataForUnit[tx - 1, ty];
            //                }

            //                break;
            //            }

            //        case 2:
            //            {
            //                if (tx < Map.MapWidth)
            //                {
            //                    u = Map.MapDataForUnit[tx + 1, ty];
            //                }

            //                break;
            //            }

            //        case 3:
            //            {
            //                if (ty > 1)
            //                {
            //                    u = Map.MapDataForUnit[tx, ty - 1];
            //                }

            //                break;
            //            }

            //        case 4:
            //            {
            //                if (ty < Map.MapHeight)
            //                {
            //                    u = Map.MapDataForUnit[tx, ty + 1];
            //                }

            //                break;
            //            }
            //    }

            //    if (u is null)
            //    {
            //        goto NextUnit;
            //    }

            //    if (ReferenceEquals(u, this))
            //    {
            //        goto NextUnit;
            //    }

            //    {
            //        var withBlock1 = u;
            //        // 正常な判断が可能？
            //        if (withBlock1.IsConditionSatisfied("混乱") || withBlock1.IsConditionSatisfied("暴走") || withBlock1.IsConditionSatisfied("恐怖") || withBlock1.IsConditionSatisfied("狂戦士"))
            //        {
            //            goto NextUnit;
            //        }

            //        // 味方？
            //        if (IsEnemy(u) || withBlock1.IsEnemy(this))
            //        {
            //            goto NextUnit;
            //        }

            //        // 同じチームに属している？
            //        uteam = withBlock1.MainPilot().SkillData("チーム");
            //        if ((team ?? "") != (uteam ?? "") && !string.IsNullOrEmpty(team) && !string.IsNullOrEmpty(uteam))
            //        {
            //            goto NextUnit;
            //        }

            //        // 移動のみの場合、相手がこれから移動してしまっては意味がない
            //        if (!for_attack)
            //        {
            //            if (withBlock1.Action > 0)
            //            {
            //                goto NextUnit;
            //            }
            //        }

            //        // 自分自身がサポート可能であれば、相手が誰でも役に立つ
            //        if (do_support)
            //        {
            //            LookForSupportRet = (LookForSupportRet + 1);
            //        }

            //        // サポート能力を持っている？
            //        {
            //            var withBlock2 = withBlock1.MainPilot();
            //            if (withBlock2.IsSkillAvailable("援護") || withBlock2.IsSkillAvailable("援護攻撃"))
            //            {
            //                LookForSupportRet = (LookForSupportRet + 1);
            //                // これから攻撃する場合、相手が行動出来ればサポートアタックが可能
            //                if (for_attack)
            //                {
            //                    if (u.Action > 0)
            //                    {
            //                        LookForSupportRet = (LookForSupportRet + 1);
            //                        // 同時援護攻撃が可能であればさらにボーナス
            //                        if (MainPilot().IsSkillAvailable("統率"))
            //                        {
            //                            LookForSupportRet = (LookForSupportRet + 1);
            //                        }
            //                    }
            //                }
            //            }
            //            else if (withBlock2.IsSkillAvailable("援護防御") || withBlock2.IsSkillAvailable("指揮") || withBlock2.IsSkillAvailable("広域サポート"))
            //            {
            //                LookForSupportRet = (LookForSupportRet + 1);
            //            }
            //        }
            //    }

            //NextUnit:
            //    ;
            //}

            //return LookForSupportRet;
        }

        // (tx,ty)にユニットが進入可能か？
        public bool IsAbleToEnter(int tx, int ty)
        {
            // 使用不能の形態はどの地形に対しても進入不可能とみなす
            if (!IsAvailable())
            {
                return false;
            }

            // 単に必要技能をチェックしている場合？
            if (string.IsNullOrEmpty(Map.MapFileName))
            {
                return true;
            }

            // マップ外？
            if (tx < 1 || Map.MapWidth < tx || ty < 1 || Map.MapHeight < ty)
            {
                return false;
            }

            // 地形適応チェック
            switch (Map.Terrain(tx, ty)?.Class ?? "")
            {
                case "空":
                    {
                        if (!IsTransAvailable("空") && !CurrentForm().IsFeatureAvailable("空中移動"))
                        {
                            return false;
                        }

                        break;
                    }

                case "水":
                    {
                        if (IsTransAvailable("空") || CurrentForm().IsFeatureAvailable("空中移動") || IsTransAvailable("水上"))
                        {
                            return true;
                        }

                        if (get_Adaption(3) == 0 && !CurrentForm().IsFeatureAvailable("水中移動"))
                        {
                            return false;
                        }

                        break;
                    }

                case "深水":
                    {
                        if (IsTransAvailable("空") || CurrentForm().IsFeatureAvailable("空中移動") || IsTransAvailable("水上"))
                        {
                            return true;
                        }

                        if (!IsTransAvailable("水") && !CurrentForm().IsFeatureAvailable("水中移動"))
                        {
                            return false;
                        }

                        break;
                    }

                case "宇宙":
                    {
                        if (get_Adaption(4) == 0 && !CurrentForm().IsFeatureAvailable("宇宙移動"))
                        {
                            return false;
                        }

                        break;
                    }

                case "月面":
                    {
                        if (IsTransAvailable("空") || CurrentForm().IsFeatureAvailable("空中移動") || IsTransAvailable("宇") || CurrentForm().IsFeatureAvailable("宇宙移動"))
                        {
                            return true;
                        }

                        break;
                    }

                default:
                    {
                        if (IsTransAvailable("空") || CurrentForm().IsFeatureAvailable("空中移動"))
                        {
                            return true;
                        }

                        if (!IsTransAvailable("陸") && !CurrentForm().IsFeatureAvailable("陸上移動"))
                        {
                            return false;
                        }

                        break;
                    }
            }

            // 進入不能？
            if (Map.Terrain(tx, ty)?.MoveCost >= 1000)
            {
                return false;
            }

            return true;
        }

        // この形態が使用可能か？ (Disable＆必要技能のチェック)
        public bool IsAvailable()
        {
            // イベントコマンド「Disable」
            if (IsDisabled(Name))
            {
                return false;
            }

            // 制限時間の切れた形態？
            if (Status == "他形態")
            {
                if (IsConditionSatisfied("行動不能"))
                {
                    return false;
                }
            }

            var withBlock = CurrentForm();
            // 技能チェックが必要？
            if (withBlock.CountPilot() == 0 || !IsFeatureAvailable("必要技能") && !IsFeatureAvailable("不必要技能"))
            {
                return true;
            }

            // 必要技能をチェック
            var loopTo = CountFeature();
            for (int i = 1; i <= loopTo; i++)
            {
            switch (Feature(i)?.Name ?? "")
                {
                    case "必要技能":
                        {
                            if (!withBlock.IsNecessarySkillSatisfied(FeatureData(i)))
                            {
                                return false;
                            }

                            break;
                        }

                    case "不必要技能":
                        {
                            if (withBlock.IsNecessarySkillSatisfied(FeatureData(i)))
                            {
                                return false;
                            }

                            break;
                        }
                }
            }

            return true;
        }

        // 必要技能を満たしているか？
        public bool IsNecessarySkillSatisfied(string nabilities, [Optional, DefaultParameterValue(null)] Pilot p)
        {
            if (string.IsNullOrEmpty(nabilities))
            {
                return true;
            }

            int num = GeneralLib.LLength(nabilities);
            var nskill_list = new string[101];
            var loopTo = (int)GeneralLib.MinLng(num, 100);
            for (int i = 1; i <= loopTo; i++)
                nskill_list[i] = GeneralLib.LIndex(nabilities, i);

            // 個々の必要条件をチェック
            int idx = 1;
            while (idx <= GeneralLib.MinLng(num, 100))
            {
                if (IsNecessarySkillSatisfied2(nskill_list[idx], p))
                {
                    // 必要条件が満たされた場合、その後の「or」をスキップ
                    if (idx <= num - 2)
                    {
                        while (Strings.LCase(nskill_list[idx + 1]) == "or")
                        {
                            idx = idx + 2;
                            // 検査する必要条件が無くなったので必要技能が満たされたと判定
                            if (idx == num)
                            {
                                return true;
                            }
                            else if (idx > num)
                            {
                                // orの後ろに必要条件がない
                                GUI.ErrorMessage(Name + "に対する必要技能「" + nabilities + "」が不正です");
                                SRC.TerminateSRC();
                            }
                        }
                    }
                }
                else
                {
                    // 必要条件が満たされなかった場合、その後に「or」がなければ
                    // 必要技能が満たされなかったと判定
                    if (idx > num - 2)
                    {
                        return false;
                    }

                    idx = idx + 1;
                    if (Strings.LCase(nskill_list[idx]) != "or")
                    {
                        return false;
                    }
                }

                idx = idx + 1;
            }

            return true;
        }

        public bool IsNecessarySkillSatisfied2(string ndata, Pilot p)
        {
            string stype2, stype, sname;
            double slevel;
            double nlevel;
            var mp = default(Pilot);
            int i, j;

            // ステータスコマンド実行時は条件が満たされていると見なす？
            if (Strings.Left(ndata, 1) == "+")
            {
                if (Status == "出撃" && SRC.InterMission.InStatusCommand())
                {
                    return true;
                }

                ndata = Strings.Mid(ndata, 2);
            }

            // 召喚者技能を参照？
            if (Strings.Left(ndata, 1) == "*")
            {
                if (Summoner is null)
                {
                    return false;
                }

                return Summoner.IsNecessarySkillSatisfied2(Strings.Mid(ndata, 2), null);
            }

            i = Strings.InStr(ndata, "Lv");
            if (i > 0)
            {
                sname = Strings.Left(ndata, i - 1);
                nlevel = GeneralLib.StrToDbl(Strings.Mid(ndata, i + 2));
            }
            else
            {
                sname = ndata;
                nlevel = 1d;
            }

            // 不必要技能？
            if (Strings.Left(sname, 1) == "!")
            {
                return !IsNecessarySkillSatisfied2(Strings.Mid(ndata, 2), p);
            }

            // 必要技能の判定に使用するパイロットを設定
            if (p is null)
            {
                if (CountPilot() > 0)
                {
                    mp = MainPilot();
                }
                else
                {
                    var withBlock = CurrentForm();
                    if (withBlock.CountPilot() > 0)
                    {
                        mp = withBlock.MainPilot();
                    }
                }
            }
            else
            {
                mp = p;
            }

            // ダミーパイロットの場合は無視
            if (mp is object)
            {
                if (mp.Nickname0 == "パイロット不在")
                {
                    mp = null;
                }
            }

            slevel = -10000;

            // まず名称が変わらない必要技能を判定
            switch (sname ?? "")
            {
                case "レベル":
                    {
                        slevel = mp is object ? mp.Level : 0d;
                        break;
                    }

                case "格闘":
                    {
                        slevel = mp is object ? mp.InfightBase : 0d;
                        break;
                    }

                case "射撃":
                    {
                        slevel = 0d;
                        if (mp is object && !mp.HasMana())
                        {
                            slevel = mp.ShootingBase;
                        }

                        break;
                    }

                case "魔力":
                    {
                        slevel = 0d;
                        if (mp is object && mp.HasMana())
                        {
                            slevel = mp.ShootingBase;
                        }

                        break;
                    }

                case "命中":
                    {
                        slevel = mp is object ? mp.HitBase : 0d;
                        break;
                    }

                case "回避":
                    {
                        slevel = mp is object ? mp.DodgeBase : 0d;
                        break;
                    }

                case "技量":
                    {
                        slevel = mp is object ? mp.TechniqueBase : 0d;
                        break;
                    }

                case "反応":
                    {
                        slevel = mp is object ? mp.IntuitionBase : 0d;
                        break;
                    }

                case "格闘初期値":
                    {
                        slevel = 0d;
                        if (mp is object)
                        {
                            slevel = mp.InfightBase - (Expression.IsOptionDefined("攻撃力低成長") ? mp.Level / 2 : mp.Level);
                        }

                        break;
                    }

                case "射撃初期値":
                    {
                        slevel = 0d;
                        if (mp is object && !mp.HasMana())
                        {
                            slevel = mp.ShootingBase - (Expression.IsOptionDefined("攻撃力低成長") ? mp.Level / 2 : mp.Level);
                        }

                        break;
                    }

                case "魔力初期値":
                    {
                        slevel = 0d;
                        if (mp is object && mp.HasMana())
                        {
                            slevel = mp.ShootingBase - (Expression.IsOptionDefined("攻撃力低成長") ? mp.Level / 2 : mp.Level);
                        }

                        break;
                    }

                case "命中初期値":
                    {
                        slevel = 0d;
                        if (mp is object)
                        {
                            slevel = mp.HitBase - mp.Level;
                        }

                        break;
                    }

                case "回避初期値":
                    {
                        slevel = 0d;
                        if (mp is object)
                        {
                            slevel = mp.DodgeBase - mp.Level;
                        }

                        break;
                    }

                case "技量初期値":
                    {
                        slevel = 0d;
                        if (mp is object)
                        {
                            slevel = mp.TechniqueBase - mp.Level;
                        }

                        break;
                    }

                case "反応初期値":
                    {
                        slevel = 0d;
                        if (mp is object)
                        {
                            slevel = mp.IntuitionBase - mp.Level;
                        }

                        break;
                    }

                case "男性":
                    {
                        slevel = 0d;
                        if (mp is object)
                        {
                            if (mp.Sex == "男性")
                            {
                                slevel = 1d;
                            }

                            if (Data.PilotNum > 1)
                            {
                                foreach (var pi in SubPilots)
                                {
                                    if (pi.Sex == "男性")
                                    {
                                        slevel = 1d;
                                    }
                                }
                            }

                            foreach (var si in Supports)
                            {
                                if (si.Sex == "男性")
                                {
                                    slevel = 1d;
                                }
                            }

                            if (IsFeatureAvailable("追加サポート"))
                            {
                                if (AdditionalSupport().Sex == "男性")
                                {
                                    slevel = 1d;
                                }
                            }
                        }

                        break;
                    }

                case "女性":
                    {
                        slevel = 0d;
                        if (mp is object)
                        {
                            if (mp.Sex == "女性")
                            {
                                slevel = 1d;
                            }

                            if (Data.PilotNum > 1)
                            {
                                foreach (var pi in SubPilots)
                                {
                                    if (pi.Sex == "女性")
                                    {
                                        slevel = 1d;
                                    }
                                }
                            }

                            foreach (var si in Supports)
                            {
                                if (si.Sex == "女性")
                                {
                                    slevel = 1d;
                                }
                            }

                            if (IsFeatureAvailable("追加サポート"))
                            {
                                if (AdditionalSupport().Sex == "女性")
                                {
                                    slevel = 1d;
                                }
                            }
                        }

                        break;
                    }

                case "生身":
                    {
                        slevel = IsHero() ? 1d : 0d;
                        break;
                    }

                case "瀕死":
                    {
                        slevel = HP <= MaxHP / 4 ? 1d : 0d;
                        break;
                    }

                case "ＨＰ":
                    {
                        slevel = 10d * HP / MaxHP;
                        break;
                    }

                case "ＥＮ":
                    {
                        slevel = 10d * EN / MaxEN;
                        break;
                    }

                case "気力":
                    {
                        if (mp is object)
                        {
                            slevel = (mp.Morale - 100d) / 10d;
                        }
                        else
                        {
                            slevel = 0d;
                        }

                        break;
                    }

                case "ランク":
                    {
                        slevel = Rank;
                        break;
                    }

                case "地上":
                case "空中":
                case "水中":
                case "水上":
                case "宇宙":
                case "地中":
                    {
                        slevel = 0d;
                        if (Status == "出撃" && (sname ?? "") == (Area ?? ""))
                        {
                            slevel = 1d;
                        }

                        break;
                    }

                case "アイテム":
                    {
                        // 使い捨てアイテム表記用
                        slevel = 1d;
                        break;
                    }

                case "当て身技":
                case "自動反撃":
                    {
                        // アビリティで付加された当て身技及び自動反撃専用の武器が表示されるのを
                        // 防ぐため、これらの必要技能は常に満たされないとみなす
                        return false;
                    }
            }

            // 上の条件のいずれかに該当？
            if (slevel != -10000)
            {
                return slevel >= nlevel;
            }

            // 必要技能の種類を判別
            stype = mp is object ? mp.SkillType(sname) : sname;

            // 名称が変わる可能性がある必要技能を判定
            string iname;
            string uname;
            Unit u;
            int max_range;
            switch (stype ?? "")
            {
                case "超感覚":
                    {
                        if (p is object)
                        {
                            slevel = p.SkillLevel("超感覚", ref_mode: "");
                            if ((stype ?? "") != (sname ?? "") && (p.SkillNameForNS(stype) ?? "") != (sname ?? ""))
                            {
                                slevel = 0d;
                            }

                            slevel += p.SkillLevel("知覚強化", ref_mode: "");
                        }
                        else if (mp is object)
                        {
                            slevel = mp.SkillLevel("超感覚", ref_mode: "");
                            if (Data.PilotNum > 1)
                            {
                                foreach (var pi in SubPilots)
                                {
                                    slevel = GeneralLib.MaxDbl(slevel, pi.SkillLevel("超感覚", ref_mode: ""));
                                    slevel = GeneralLib.MaxDbl(slevel, pi.SkillLevel(sname, ref_mode: ""));
                                }
                            }

                            foreach (var si in Supports)
                            {
                                slevel = GeneralLib.MaxDbl(slevel, si.SkillLevel("超感覚", ref_mode: ""));
                                slevel = GeneralLib.MaxDbl(slevel, si.SkillLevel(sname, ref_mode: ""));
                            }

                            if (IsFeatureAvailable("追加サポート"))
                            {
                                var asi = AdditionalSupport();
                                slevel = GeneralLib.MaxDbl(slevel, asi.SkillLevel("超感覚", ref_mode: ""));
                                slevel = GeneralLib.MaxDbl(slevel, asi.SkillLevel(sname, ref_mode: ""));
                            }

                            if ((stype ?? "") != (sname ?? "") && (mp.SkillNameForNS(stype) ?? "") != (sname ?? ""))
                            {
                                slevel = 0d;
                            }

                            slevel += mp.SkillLevel("知覚強化", ref_mode: "");
                            if (Data.PilotNum > 1)
                            {
                                foreach (var pi in SubPilots)
                                {
                                    slevel = GeneralLib.MaxDbl(slevel, pi.SkillLevel("知覚強化", ref_mode: ""));
                                }
                            }

                            foreach (var si in Supports)
                            {
                                slevel = GeneralLib.MaxDbl(slevel, si.SkillLevel("知覚強化", ref_mode: ""));
                            }

                            if (IsFeatureAvailable("追加サポート"))
                            {
                                slevel = GeneralLib.MaxDbl(slevel, AdditionalSupport().SkillLevel("知覚強化", ref_mode: ""));
                            }
                        }

                        break;
                    }

                case "同調率":
                    {
                        if (p is object)
                        {
                            slevel = p.SynchroRate();
                        }
                        else if (mp is object)
                        {
                            slevel = mp.SynchroRate();
                            if (Data.PilotNum > 1)
                            {
                                foreach (var pi in SubPilots)
                                {
                                    slevel = GeneralLib.MaxDbl(slevel, pi.SynchroRate());
                                }
                            }

                            foreach (var si in Supports)
                            {
                                slevel = GeneralLib.MaxDbl(slevel, si.SynchroRate());
                            }

                            if (IsFeatureAvailable("追加サポート"))
                            {
                                slevel = GeneralLib.MaxDbl(slevel, AdditionalSupport().SynchroRate());
                            }
                        }

                        if ((stype ?? "") != (sname ?? "") && mp is object && (mp.SkillNameForNS(stype) ?? "") != (sname ?? ""))
                        {
                            slevel = 0d;
                        }

                        break;
                    }

                case "オーラ":
                    {
                        if (p is object)
                        {
                            slevel = p.SkillLevel("オーラ", ref_mode: "");
                        }
                        else if (mp is object)
                        {
                            slevel = AuraLevel();
                        }

                        if ((stype ?? "") != (sname ?? "") && mp is object && (mp.SkillNameForNS(stype) ?? "") != (sname ?? ""))
                        {
                            slevel = 0d;
                        }

                        break;
                    }

                case "霊力":
                    {
                        if (p is object)
                        {
                            slevel = p.Plana;
                        }
                        else if (mp is object)
                        {
                            slevel = mp.Plana;
                        }

                        if ((stype ?? "") != (sname ?? "") && mp is object && (mp.SkillNameForNS(stype) ?? "") != (sname ?? ""))
                        {
                            slevel = 0d;
                        }

                        break;
                    }

                default:
                    {
                        // 上記以外のパイロット用特殊能力
                        if (mp is object)
                        {
                            // 特定パイロット専用？
                            if ((sname ?? "") == (mp.Name ?? "") || (sname ?? "") == (mp.get_Nickname(false) ?? ""))
                            {
                                slevel = 1d;
                            }
                            else if ((stype ?? "") == (sname ?? ""))
                            {
                                slevel = mp.SkillLevel(stype, ref_mode: "");
                            }
                            else if ((mp.SkillNameForNS(stype) ?? "") == (sname ?? ""))
                            {
                                slevel = mp.SkillLevel(stype, ref_mode: "");
                            }

                            // パイロット数が括弧つきでない場合のみ
                            if (Data.PilotNum > 1)
                            {
                                // サブパイロットの技能を検索
                                foreach (var pi in SubPilots)
                                {
                                    if ((sname ?? "") == (pi.Name ?? "") || (sname ?? "") == (pi.get_Nickname(false) ?? ""))
                                    {
                                        slevel = 1d;
                                        break;
                                    }

                                    stype2 = pi.SkillType(sname);
                                    if ((stype2 ?? "") == (sname ?? ""))
                                    {
                                        slevel = GeneralLib.MaxDbl(slevel, pi.SkillLevel(stype2, ref_mode: ""));
                                    }
                                    else if ((pi.SkillNameForNS(stype2) ?? "") == (sname ?? ""))
                                    {
                                        slevel = GeneralLib.MaxDbl(slevel, pi.SkillLevel(stype2, ref_mode: ""));
                                    }
                                }
                            }

                            // サポートパイロットの技能を検索
                            foreach (var si in Supports)
                            {
                                if ((sname ?? "") == (si.Name ?? "") || (sname ?? "") == (si.get_Nickname(false) ?? ""))
                                {
                                    slevel = 1d;
                                    break;
                                }

                                stype2 = si.SkillType(sname);
                                if ((stype2 ?? "") == (sname ?? ""))
                                {
                                    slevel = GeneralLib.MaxDbl(slevel, si.SkillLevel(stype2, ref_mode: ""));
                                }
                                else if ((si.SkillNameForNS(stype2) ?? "") == (sname ?? ""))
                                {
                                    slevel = GeneralLib.MaxDbl(slevel, si.SkillLevel(stype2, ref_mode: ""));
                                }
                            }

                            // 追加サポートの技能を検索
                            if (IsFeatureAvailable("追加サポート") && CountPilot() > 0)
                            {
                                var asi = AdditionalSupport();
                                if ((sname ?? "") == (asi.Name ?? "") || (sname ?? "") == (asi.get_Nickname(false) ?? ""))
                                {
                                    slevel = 1d;
                                }
                                else
                                {
                                    stype2 = asi.SkillType(sname);
                                    if ((stype2 ?? "") == (sname ?? ""))
                                    {
                                        slevel = GeneralLib.MaxDbl(slevel, asi.SkillLevel(stype2, ref_mode: ""));
                                    }
                                    else if ((asi.SkillNameForNS(stype2) ?? "") == (sname ?? ""))
                                    {
                                        slevel = GeneralLib.MaxDbl(slevel, asi.SkillLevel(stype2, ref_mode: ""));
                                    }
                                }
                            }
                        }

                        if (slevel == 0d)
                        {
                            // ユニット名またはクラスに該当？
                            if ((sname ?? "") == (Name ?? "") || (sname ?? "") == (Nickname0 ?? "") || (sname ?? "") == (Class0 ?? ""))
                            {
                                slevel = 1d;
                            }
                        }

                        if (slevel == 0d)
                        {
                            if (Strings.Left(sname, 1) == "@")
                            {
                                // 地形を指定した必要技能
                                if (Status == "出撃" && 1 <= x && x <= Map.MapWidth && 1 <= y && y <= Map.MapHeight)
                                {
                                    if ((Strings.Mid(sname, 2) ?? "") == (Map.Terrain(x, y)?.Name ?? ""))
                                    {
                                        slevel = 1d;
                                    }
                                }
                            }
                            else if (Strings.Right(sname, 2) == "装備")
                            {
                                // アイテムを指定した必要技能
                                iname = Strings.Left(sname, Strings.Len(sname) - 2);
                                var loopTo12 = CountItem();
                                for (i = 1; i <= loopTo12; i++)
                                {
                                    var item = Item(i);
                                    if (item.Activated && ((iname ?? "") == (item.Name ?? "") || (iname ?? "") == (item.Class0() ?? "")))
                                    {
                                        slevel = 1d;
                                        break;
                                    }
                                }
                            }
                            else if (Strings.Right(sname, 2) == "隣接" || Strings.Right(sname, 4) == "マス以内")
                            {
                                // 特定のユニットが近くにいることを指定した必要技能
                                if (Status == "出撃")
                                {
                                    if (Strings.Right(sname, 2) == "隣接")
                                    {
                                        uname = Strings.Left(sname, Strings.Len(sname) - 2);
                                        max_range = 1;
                                    }
                                    else
                                    {
                                        uname = Strings.Left(sname, Strings.Len(sname) - 5);
                                        // e.g. "3マス以内" → extract "3" (the digit before "マス以内" which is 4 chars, and the char before it is at offset Len-4)
                                        max_range = GeneralLib.StrToLng(Strings.Mid(sname, Strings.Len(sname) - 4, 1));
                                    }

                                    var loopTo13 = GeneralLib.MinLng(x + max_range, Map.MapWidth);
                                    // ix, jy: map coordinates for neighbor search
                                    for (int ix = GeneralLib.MaxLng(x - max_range, 1); ix <= loopTo13; ix++)
                                    {
                                        var loopTo14 = GeneralLib.MinLng(y + max_range, Map.MapHeight);
                                        for (int jy = GeneralLib.MaxLng(y - max_range, 1); jy <= loopTo14; jy++)
                                        {
                                            u = Map.MapDataForUnit[ix, jy];

                                            // 距離が範囲外？
                                            if ((Math.Abs(x - ix) + Math.Abs(y - jy)) > max_range)
                                            {
                                                goto NextNeighbor;
                                            }

                                            // ユニットがいない？
                                            if (u is null)
                                            {
                                                goto NextNeighbor;
                                            }

                                            // ユニットが自分？
                                            if (ReferenceEquals(u, this) || x == ix && y == jy)
                                            {
                                                goto NextNeighbor;
                                            }

                                            // ユニットが敵？
                                            if (IsEnemy(u))
                                            {
                                                goto NextNeighbor;
                                            }

                                            // 合体技のパートナーに該当するか
                                            if (uname == "母艦")
                                            {
                                                if (!u.IsFeatureAvailable("母艦"))
                                                {
                                                    goto NextNeighbor;
                                                }
                                            }
                                            else if ((u.Name ?? "") != (uname ?? "") && (u.MainPilot().Name ?? "") != (uname ?? ""))
                                            {
                                                goto NextNeighbor;
                                            }

                                            // 行動出来なければだめ
                                            if (u.MaxAction() == 0 || u.IsConditionSatisfied("混乱") || u.IsConditionSatisfied("恐怖") || u.IsConditionSatisfied("憑依"))
                                            {
                                                goto NextNeighbor;
                                            }

                                            // パートナーが見つかった
                                            return true;
                                            NextNeighbor:
                                            ;
                                        }
                                    }
                                }
                            }
                            else if (Strings.Right(sname, 2) == "状態")
                            {
                                // 特殊状態を指定した必要技能
                                if (IsConditionSatisfied(Strings.Left(sname, Strings.Len(sname) - 2)))
                                {
                                    slevel = 1d;
                                }
                            }
                        }

                        break;
                    }
            }

            // 指定された技能のレベルが必要なレベル以上の場合に必要技能が満たされたと判定
            return slevel >= nlevel;
        }

        // 能力 fname を封印されているか？
        public bool IsDisabled(string fname)
        {
            bool IsDisabledRet = default;
            if (Strings.Len(fname) == 0)
            {
                IsDisabledRet = false;
                return IsDisabledRet;
            }

            if (Expression.IsGlobalVariableDefined("Disable(" + fname + ")"))
            {
                IsDisabledRet = true;
                return IsDisabledRet;
            }

            if (Expression.IsGlobalVariableDefined("Disable(" + Name + "," + fname + ")"))
            {
                IsDisabledRet = true;
                return IsDisabledRet;
            }

            IsDisabledRet = false;
            return IsDisabledRet;
        }

        // 現在、自分が攻撃を受けている側かどうか判定
        public bool IsDefense()
        {
            bool IsDefenseRet = default;
            if ((Party ?? "") == (SRC.Stage ?? ""))
            {
                IsDefenseRet = false;
            }
            else
            {
                IsDefenseRet = true;
            }

            return IsDefenseRet;
        }
        public IList<Unit> CombinationPartner(string cname, int cen, int cmorale, int cplana, int crange, int tx = 0, int ty = 0, bool check_formation = false)
        {
            var partners = new List<Unit>();

            // 正常な判断が可能？
            if (IsConditionSatisfied("混乱"))
            {
                return partners;
            }

            // ユニットの特殊能力「合体技」の検索
            int clevel = 0;
            IList<String> clist = new List<string>();
            int cnum = 0;
            foreach (var fd in Features.Where(x => x.Name == "合体技"))
            {
                if ((GeneralLib.LIndex(fd.Data, 1) ?? "") == (cname ?? ""))
                {
                    if (fd.HasLevel)
                    {
                        clevel = (int)fd.FeatureLevel;
                    }
                    else
                    {
                        clevel = 0;
                    }

                    clist = GeneralLib.ToList(fd.Data).Skip(1).ToList();
                    cnum = clist.Count;
                    break;
                }
            }

            if (cnum == 0)
            {
                return partners;
            }

            // 出撃していない場合
            if (Status != "出撃" || string.IsNullOrEmpty(Map.MapFileName))
            {
                // パートナーが仲間にいるだけでよい
                foreach (var uname in clist)
                {
                    // パートナーがユニット名で指定されている場合
                    if (SRC.UList.IsDefined(uname))
                    {
                        var u = SRC.UList.Item(uname);
                        if (u.Status == "出撃" || u.Status == "待機")
                        {
                            partners.Add(u);
                            continue;
                        }
                    }

                    // パートナーがパイロット名で指定されている場合
                    if (SRC.PList.IsDefined(uname))
                    {
                        var p = SRC.PList.Item(uname);
                        if (p.Unit is object)
                        {
                            var u = p.Unit;
                            if (u.Status == "出撃" || u.Status == "待機")
                            {
                                partners.Add(u);
                                continue;
                            }
                        }
                    }

                    // パートナーが見つからなかった
                    return new List<Unit>();
                }
                // パートナーが全員仲間にいる
                return partners.Count == cnum ? partners : new List<Unit>();
            }

            // 合体技の基点の設定
            if (tx == 0)
            {
                tx = x;
            }

            if (ty == 0)
            {
                ty = y;
            }

            // パートナーの検索範囲を設定
            int loop_limit;
            if (crange == 1)
            {
                if (cnum >= 8)
                {
                    // 射程１で８体合体以上の場合は２マス以内
                    loop_limit = 12;
                }
                else if (cnum >= 4)
                {
                    // 射程１で４体合体以上の場合は斜め隣接可
                    loop_limit = 8;
                }
                else
                {
                    // どれにも該当していなければ隣接のみ
                    loop_limit = 4;
                }
            }
            else if (cnum >= 9)
            {
                // 射程２以上で９体合体以上の場合は２マス以内
                loop_limit = 12;
            }
            else if (cnum >= 5)
            {
                // 射程２以上で５体合体以上の場合は斜め隣接可
                loop_limit = 8;
            }
            else
            {
                // どれにも該当していなければ隣接のみ
                loop_limit = 4;
            }

            // 合体技斜め隣接可オプション
            if (Expression.IsOptionDefined("合体技斜め隣接可"))
            {
                if (loop_limit == 4)
                {
                    loop_limit = 8;
                }
            }

            partners.Clear();
            // パートナーの名称
            foreach (var uname in clist)
            {
                for (var j = 1; j <= loop_limit; j++)
                {
                    // パートナーの検索位置設定
                    // XXX もうちょいいい感じにしたいもんだ
                    Unit u = null;
                    switch (j)
                    {
                        case 1:
                            {
                                if (tx > 1)
                                {
                                    u = Map.MapDataForUnit[tx - 1, ty];
                                }

                                break;
                            }

                        case 2:
                            {
                                if (tx < Map.MapWidth)
                                {
                                    u = Map.MapDataForUnit[tx + 1, ty];
                                }

                                break;
                            }

                        case 3:
                            {
                                if (ty > 1)
                                {
                                    u = Map.MapDataForUnit[tx, ty - 1];
                                }

                                break;
                            }

                        case 4:
                            {
                                if (ty < Map.MapHeight)
                                {
                                    u = Map.MapDataForUnit[tx, ty + 1];
                                }

                                break;
                            }

                        case 5:
                            {
                                if (tx > 1 && ty > 1)
                                {
                                    u = Map.MapDataForUnit[tx - 1, ty - 1];
                                }

                                break;
                            }

                        case 6:
                            {
                                if (tx < Map.MapWidth && ty < Map.MapHeight)
                                {
                                    u = Map.MapDataForUnit[tx + 1, ty + 1];
                                }

                                break;
                            }

                        case 7:
                            {
                                if (tx > 1 && ty < Map.MapHeight)
                                {
                                    u = Map.MapDataForUnit[tx - 1, ty + 1];
                                }

                                break;
                            }

                        case 8:
                            {
                                if (tx < Map.MapWidth && ty > 1)
                                {
                                    u = Map.MapDataForUnit[tx + 1, ty - 1];
                                }

                                break;
                            }

                        case 9:
                            {
                                if (tx > 2)
                                {
                                    u = Map.MapDataForUnit[tx - 2, ty];
                                }

                                break;
                            }

                        case 10:
                            {
                                if (tx < Map.MapWidth - 1)
                                {
                                    u = Map.MapDataForUnit[tx + 2, ty];
                                }

                                break;
                            }

                        case 11:
                            {
                                if (ty > 2)
                                {
                                    u = Map.MapDataForUnit[tx, ty - 2];
                                }

                                break;
                            }

                        case 12:
                            {
                                if (ty < Map.MapHeight - 1)
                                {
                                    u = Map.MapDataForUnit[tx, ty + 2];
                                }

                                break;
                            }
                    }

                    // ユニットが存在する？
                    if (u is null)
                    {
                        goto NextNeighbor;
                    }
                    // 合体技のパートナーに該当する？
                    if ((u.Name ?? "") != (uname ?? ""))
                    {
                        // パイロット名でも確認
                        if ((u.MainPilot().Name ?? "") != (uname ?? ""))
                        {
                            goto NextNeighbor;
                        }
                    }

                    // ユニットが自分？
                    if (ReferenceEquals(u, this))
                    {
                        goto NextNeighbor;
                    }

                    // 既に選択済み？
                    if (partners.Contains(u))
                    {
                        goto NextNeighbor;
                    }

                    // ユニットが敵？
                    if (IsEnemy(u))
                    {
                        goto NextNeighbor;
                    }

                    // 行動出来なければだめ
                    if (u.MaxAction() == 0 || u.IsConditionSatisfied("混乱")
                        || u.IsConditionSatisfied("恐怖")
                        || u.IsConditionSatisfied("憑依"))
                    {
                        goto NextNeighbor;
                    }

                    // 合体技にレベルが設定されていればパイロット間の信頼度をチェック
                    if (clevel > 0)
                    {
                        if (MainPilot().Relation(u.MainPilot()) < clevel
                            || u.MainPilot().Relation(MainPilot()) < clevel)
                        {
                            goto NextNeighbor;
                        }
                    }

                    // TODO Impl 合体技条件
                    //// パートナーが武器を使うための条件を満たしているかを判定
                    //if (!check_formation)
                    //{
                    //    if (ctype == "武装")
                    //    {
                    //        // 合体技と同名の武器を検索
                    //        var loopTo5 = u.CountWeapon();
                    //        for (k = 1; k <= loopTo5; k++)
                    //        {
                    //            if ((u.Weapon(k).Name ?? "") == (cname ?? ""))
                    //            {
                    //                break;
                    //            }
                    //        }

                    //        if (k <= u.CountWeapon())
                    //        {
                    //            // 武器が使える？
                    //            if (!u.IsWeaponMastered(k))
                    //            {
                    //                goto NextNeighbor;
                    //            }

                    //            if (u.Weapon(k).NecessaryMorale > 0)
                    //            {
                    //                if (u.MainPilot().Morale < u.Weapon(k).NecessaryMorale)
                    //                {
                    //                    goto NextNeighbor;
                    //                }
                    //            }

                    //            if (u.WeaponENConsumption(k) > 0)
                    //            {
                    //                if (u.EN < u.WeaponENConsumption(k))
                    //                {
                    //                    goto NextNeighbor;
                    //                }
                    //            }

                    //            if (u.Weapon(k).Bullet > 0)
                    //            {
                    //                if (u.Bullet(k) == 0)
                    //                {
                    //                    goto NextNeighbor;
                    //                }
                    //            }

                    //            if (u.WeaponLevel(k, "霊") > 0d)
                    //            {
                    //                if (u.MainPilot().Plana < 5d * u.WeaponLevel(k, "霊"))
                    //                {
                    //                    goto NextNeighbor;
                    //                }
                    //            }
                    //            else if (u.WeaponLevel(k, "プ") > 0d)
                    //            {
                    //                if (u.MainPilot().Plana < 5d * u.WeaponLevel(k, "プ"))
                    //                {
                    //                    goto NextNeighbor;
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            // 同名の武器を持っていなかった場合はチェック項目を限定
                    //            if (cmorale > 0)
                    //            {
                    //                if (u.MainPilot().Morale < cmorale)
                    //                {
                    //                    goto NextNeighbor;
                    //                }
                    //            }

                    //            if (cen > 0)
                    //            {
                    //                if (u.EN < cen)
                    //                {
                    //                    goto NextNeighbor;
                    //                }
                    //            }

                    //            if (cplana > 0)
                    //            {
                    //                if (u.MainPilot().Plana < cplana)
                    //                {
                    //                    goto NextNeighbor;
                    //                }
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        // 合体技と同名のアビリティを検索
                    //        var loopTo6 = u.CountAbility();
                    //        for (k = 1; k <= loopTo6; k++)
                    //        {
                    //            if ((u.Ability(k).Name ?? "") == (cname ?? ""))
                    //            {
                    //                break;
                    //            }
                    //        }

                    //        if (k <= u.CountAbility())
                    //        {
                    //            // アビリティが使える？
                    //            if (!u.IsAbilityMastered(k))
                    //            {
                    //                goto NextNeighbor;
                    //            }

                    //            if (u.Ability(k).NecessaryMorale > 0)
                    //            {
                    //                if (u.MainPilot().Morale < u.Ability(k).NecessaryMorale)
                    //                {
                    //                    goto NextNeighbor;
                    //                }
                    //            }

                    //            if (u.AbilityENConsumption(k) > 0)
                    //            {
                    //                if (u.EN < u.AbilityENConsumption(k))
                    //                {
                    //                    goto NextNeighbor;
                    //                }
                    //            }

                    //            if (u.Ability(k).Stock > 0)
                    //            {
                    //                if (u.Stock(k) == 0)
                    //                {
                    //                    goto NextNeighbor;
                    //                }
                    //            }

                    //            if (u.AbilityLevel(k, "霊") > 0d)
                    //            {
                    //                if (u.MainPilot().Plana < 5d * u.AbilityLevel(k, "霊"))
                    //                {
                    //                    goto NextNeighbor;
                    //                }
                    //            }
                    //            else if (u.AbilityLevel(k, "プ") > 0d)
                    //            {
                    //                if (u.MainPilot().Plana < 5d * u.AbilityLevel(k, "プ"))
                    //                {
                    //                    goto NextNeighbor;
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            // 同名のアビリティを持っていなかった場合はチェック項目を限定
                    //            if (cmorale > 0)
                    //            {
                    //                if (u.MainPilot().Morale < cmorale)
                    //                {
                    //                    goto NextNeighbor;
                    //                }
                    //            }

                    //            if (cen > 0)
                    //            {
                    //                if (u.EN < cen)
                    //                {
                    //                    goto NextNeighbor;
                    //                }
                    //            }

                    //            if (cplana > 0)
                    //            {
                    //                if (u.MainPilot().Plana < cplana)
                    //                {
                    //                    goto NextNeighbor;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    //// フォーメーションのチェックだけの時も必要技能は調べておく
                    //else if (ctype == "武装")
                    //{
                    //    var loopTo7 = u.CountWeapon();
                    //    for (k = 1; k <= loopTo7; k++)
                    //    {
                    //        if ((u.Weapon(k).Name ?? "") == (cname ?? ""))
                    //        {
                    //            break;
                    //        }
                    //    }

                    //    if (k <= u.CountWeapon())
                    //    {
                    //        if (!u.IsWeaponMastered(k))
                    //        {
                    //            goto NextNeighbor;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    var loopTo8 = u.CountAbility();
                    //    for (k = 1; k <= loopTo8; k++)
                    //    {
                    //        if ((u.Ability(k).Name ?? "") == (cname ?? ""))
                    //        {
                    //            break;
                    //        }
                    //    }

                    //    if (k <= u.CountAbility())
                    //    {
                    //        if (!u.IsAbilityMastered(k))
                    //        {
                    //            goto NextNeighbor;
                    //        }
                    //    }
                    //}

                    // 見つかったパートナーを記録
                    partners.Add(u);
                    break;
                NextNeighbor:
                    ;
                }

            }

            // パートナーが見つからなかった？
            if (partners.Count != cnum)
            {
                return new List<Unit>();
            }

            // 合体技メッセージ判定用にパートナー一覧を記録
            Commands.SelectedPartners = partners.CloneList();
            return partners;
        }
    }
}
