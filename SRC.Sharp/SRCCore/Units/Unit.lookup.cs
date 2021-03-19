// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.VB;
using System;
using System.Collections.Generic;

namespace SRCCore.Units
{
    public partial class Unit
    {
        // (tx,ty)の地点の周囲に「連携攻撃」を行ってくれるユニットがいるかどうかを判定
        public Unit LookForAttackHelp(int tx, int ty)
        {
            throw new NotImplementedException();
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
            //    string argIndex1 = "混乱";
            //    string argIndex2 = "暴走";
            //    string argIndex3 = "魅了";
            //    string argIndex4 = "憑依";
            //    string argIndex5 = "恐怖";
            //    string argIndex6 = "狂戦士";
            //    if (u.IsConditionSatisfied(argIndex1) | u.IsConditionSatisfied(argIndex2) | u.IsConditionSatisfied(argIndex3) | u.IsConditionSatisfied(argIndex4) | u.IsConditionSatisfied(argIndex5) | u.IsConditionSatisfied(argIndex6))
            //    {
            //        goto NextLoop;
            //    }

            //    // メッセージが登録されている？
            //    bool localIsMessageDefined() { string argmain_situation = "連携攻撃(" + u.MainPilot().Name + ")"; var ret = IsMessageDefined(argmain_situation, true); return ret; }

            //    bool localIsMessageDefined1() { string argmain_situation = "連携攻撃(" + u.MainPilot().get_Nickname(false) + ")"; var ret = IsMessageDefined(argmain_situation, true); return ret; }

            //    if (!localIsMessageDefined() & !localIsMessageDefined1())
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
        public Unit LookForGuardHelp(Unit t, int tw, bool is_critical)
        {
            throw new NotImplementedException();
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
            //        string argIndex1 = "混乱";
            //        string argIndex2 = "暴走";
            //        string argIndex3 = "魅了";
            //        string argIndex4 = "憑依";
            //        string argIndex5 = "恐怖";
            //        string argIndex6 = "狂戦士";
            //        if (withBlock.IsConditionSatisfied(argIndex1) | withBlock.IsConditionSatisfied(argIndex2) | withBlock.IsConditionSatisfied(argIndex3) | withBlock.IsConditionSatisfied(argIndex4) | withBlock.IsConditionSatisfied(argIndex5) | withBlock.IsConditionSatisfied(argIndex6))
            //        {
            //            goto NextLoop;
            //        }

            //        // メッセージが登録されている？
            //        bool localIsMessageDefined() { string argmain_situation = "かばう(" + MainPilot().Name + ")"; var ret = withBlock.IsMessageDefined(argmain_situation, true); return ret; }

            //        bool localIsMessageDefined1() { string argmain_situation = "かばう(" + MainPilot().get_Nickname(false) + ")"; var ret = withBlock.IsMessageDefined(argmain_situation, true); return ret; }

            //        if (!localIsMessageDefined() & !localIsMessageDefined1())
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
            //                            string argarea_name = "空";
            //                            string argarea_name1 = "宇宙";
            //                            if (!withBlock.IsTransAvailable(argarea_name) & !withBlock.IsTransAvailable(argarea_name1))
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
            //        string argfname = "防御不可";
            //        string argattr = "殺";
            //        if (withBlock.IsFeatureAvailable(argfname) | t.IsWeaponClassifiedAs(tw, argattr))
            //        {
            //            ratio = 1d;
            //        }
            //        else
            //        {
            //            ratio = 0.5d;
            //        }

            //        if (is_critical)
            //        {
            //            string argoname = "ダメージ倍率低下";
            //            if (Expression.IsOptionDefined(argoname))
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
                    string arglist = p.SkillData("先手必勝");
                    if (GeneralLib.LLength(arglist) == 2)
                    {
                        if (p.Morale >= GeneralLib.StrToLng(GeneralLib.LIndex(arglist, 2)))
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
            throw new NotImplementedException();
            //Unit LookForSupportAttackRet = default;
            //Unit u;
            //int i, w;
            //int max_wpower;
            //string team, uteam;

            //// 正常な判断が可能？
            //string argIndex1 = "混乱";
            //if (IsConditionSatisfied(argIndex1))
            //{
            //    return LookForSupportAttackRet;
            //}

            //// 同士討ちの場合はどちらに荷担すべきか分からないので……
            //if (t is object)
            //{
            //    if ((Party ?? "") == (t.Party ?? ""))
            //    {
            //        return LookForSupportAttackRet;
            //    }
            //}

            //string argIndex2 = "チーム";
            //team = MainPilot().SkillData(argIndex2);
            //max_wpower = -1;
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
            //    // サポートアタック数が残っている？
            //    if (u.MaxSupportAttack() <= u.UsedSupportAttack)
            //    {
            //        goto NextUnit;
            //    }

            //    // 行動数が残っている？
            //    if (u.Action == 0)
            //    {
            //        goto NextUnit;
            //    }

            //    // 正常な判断が可能？
            //    string argIndex3 = "混乱";
            //    string argIndex4 = "暴走";
            //    string argIndex5 = "恐怖";
            //    string argIndex6 = "狂戦士";
            //    string argIndex7 = "踊り";
            //    if (u.IsConditionSatisfied(argIndex3) | u.IsConditionSatisfied(argIndex4) | u.IsConditionSatisfied(argIndex5) | u.IsConditionSatisfied(argIndex6) | u.IsConditionSatisfied(argIndex7))
            //    {
            //        goto NextUnit;
            //    }

            //    // 味方？
            //    switch (u.Party ?? "")
            //    {
            //        case "ＮＰＣ":
            //            {
            //                switch (Party ?? "")
            //                {
            //                    case "敵":
            //                    case "中立":
            //                        {
            //                            goto NextUnit;
            //                            break;
            //                        }
            //                }

            //                break;
            //            }

            //        default:
            //            {
            //                if ((u.Party ?? "") != (Party ?? ""))
            //                {
            //                    goto NextUnit;
            //                }

            //                break;
            //            }
            //    }

            //    // 同じチームに属している？
            //    string argIndex8 = "チーム";
            //    uteam = u.MainPilot().SkillData(argIndex8);
            //    if ((team ?? "") != (uteam ?? "") & !string.IsNullOrEmpty(uteam))
            //    {
            //        goto NextUnit;
            //    }

            //    // まだターゲットが特定されていない？
            //    if (t is null)
            //    {
            //        LookForSupportAttackRet = u;
            //        return LookForSupportAttackRet;
            //    }

            //    // 攻撃可能？
            //    // 高い威力の武器はリストの最後の方にあることが多いので後ろから判定
            //    w = u.CountWeapon();
            //    while (w > 0)
            //    {
            //        // 攻撃力が今まで見つかった武器以下の場合は選考外
            //        if (u.WeaponPower(w, t.Area) <= max_wpower)
            //        {
            //            goto NextWeapon;
            //        }

            //        // サポートアタックに利用可能？
            //        string argattr = "Ｍ";
            //        if (u.IsWeaponClassifiedAs(w, argattr))
            //        {
            //            goto NextWeapon;
            //        }

            //        string argattr1 = "合";
            //        if (u.IsWeaponClassifiedAs(w, argattr1))
            //        {
            //            goto NextWeapon;
            //        }

            //        string argref_mode = "移動前";
            //        if (!u.IsWeaponAvailable(w, argref_mode))
            //        {
            //            goto NextWeapon;
            //        }

            //        if (!u.IsTargetWithinRange(w, t))
            //        {
            //            goto NextWeapon;
            //        }

            //        if (u.Party == "味方" & u.Party0 == "味方")
            //        {
            //            // 味方ユニットは自爆攻撃をサポートアタックには使用しない
            //            string argattr2 = "自";
            //            if (u.IsWeaponClassifiedAs(w, argattr2))
            //            {
            //                goto NextWeapon;
            //            }

            //            // 自動反撃の場合、味方ユニットは残弾数が少ない武器を使用しない
            //            // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //            if (GUI.MainForm.mnuMapCommandItem(Commands.AutoDefenseCmdID).Checked)
            //            {
            //                string argattr3 = "永";
            //                if (!u.IsWeaponClassifiedAs(w, argattr3))
            //                {
            //                    if (u.Bullet(w) == 1 | u.MaxBullet(w) == 2 | u.MaxBullet(w) == 3)
            //                    {
            //                        goto NextWeapon;
            //                    }
            //                }

            //                if (u.WeaponENConsumption(w) > 0)
            //                {
            //                    if (u.WeaponENConsumption(w) >= u.EN / 2 | u.WeaponENConsumption(w) >= u.MaxEN / 4)
            //                    {
            //                        goto NextWeapon;
            //                    }
            //                }

            //                string argattr4 = "尽";
            //                if (u.IsWeaponClassifiedAs(w, argattr4))
            //                {
            //                    goto NextWeapon;
            //                }
            //            }
            //        }

            //        // 援護攻撃用の武器が見つかった
            //        max_wpower = u.WeaponPower(w, t.Area);
            //        LookForSupportAttackRet = u;
            //    NextWeapon:
            //        ;
            //        w = (w - 1);
            //    }

            //NextUnit:
            //    ;
            //}

            //return LookForSupportAttackRet;
        }

        // ユニット t からの攻撃に対して周囲にサポートガードを行ってくれるユニットが
        // いるかどうかを判定
        public Unit LookForSupportGuard(Unit t, int tw)
        {
            return null;
            // TODO Impl
            //Unit LookForSupportGuardRet = default;
            //Unit u;
            //int i;
            //int my_dmg, dmg;
            //double ratio;
            //int ux, uy;
            //string uarea;
            //string team, uteam;

            //// マップ攻撃はサポートガード不能
            //string argattr = "Ｍ";
            //if (t.IsWeaponClassifiedAs(tw, argattr))
            //{
            //    return LookForSupportGuardRet;
            //}

            //// スペシャルパワーでサポートガードが無効化されている？
            //string argsptype = "サポートガード無効化";
            //if (t.IsUnderSpecialPowerEffect(argsptype))
            //{
            //    return LookForSupportGuardRet;
            //}

            //// 同士討ちの場合は本来の陣営に属するユニットのみを守る
            //if ((Party ?? "") == (t.Party ?? "") | Party == "ＮＰＣ" & t.Party == "味方")
            //{
            //    if ((Party ?? "") != (Party0 ?? ""))
            //    {
            //        return LookForSupportGuardRet;
            //    }

            //    string argIndex1 = "暴走";
            //    if (IsConditionSatisfied(argIndex1))
            //    {
            //        return LookForSupportGuardRet;
            //    }
            //}

            //// 自分が受けるダメージを求めておく
            //var argt = this;
            //my_dmg = t.ExpDamage(tw, argt, true);

            //// かばう必要がない？
            //// 手動反撃で味方の場合はダメージにかかわらず常にかばう
            //// UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //if (Party != "味方" | GUI.MainForm.mnuMapCommandItem(Commands.AutoDefenseCmdID).Checked)
            //{
            //    if (t.IsNormalWeapon(tw))
            //    {
            //        if (my_dmg < MaxHP / 20 & my_dmg < HP / 5)
            //        {
            //            return LookForSupportGuardRet;
            //        }
            //    }
            //    else
            //    {
            //        var argt1 = this;
            //        if (t.CriticalProbability(tw, argt1) > 0)
            //        {
            //            return LookForSupportGuardRet;
            //        }
            //    }
            //}

            //string argIndex2 = "チーム";
            //team = MainPilot().SkillData(argIndex2);
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
            //        string argsptype1 = "サポートガード不能";
            //        if (withBlock.IsUnderSpecialPowerEffect(argsptype1))
            //        {
            //            goto NextUnit;
            //        }

            //        // 正常な判断が可能？
            //        string argIndex3 = "混乱";
            //        string argIndex4 = "暴走";
            //        string argIndex5 = "恐怖";
            //        string argIndex6 = "狂戦士";
            //        if (withBlock.IsConditionSatisfied(argIndex3) | withBlock.IsConditionSatisfied(argIndex4) | withBlock.IsConditionSatisfied(argIndex5) | withBlock.IsConditionSatisfied(argIndex6))
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
            //                    string argoname = "対ＮＰＣサポートガード無効";
            //                    if (Expression.IsOptionDefined(argoname))
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
            //        string argIndex7 = "チーム";
            //        uteam = withBlock.MainPilot().SkillData(argIndex7);
            //        if ((team ?? "") != (uteam ?? "") & !string.IsNullOrEmpty(uteam))
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
            //                        string argarea_name = "空";
            //                        if (!withBlock.IsTransAvailable(argarea_name))
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
            //        string argfname = "防御不可";
            //        string argattr1 = "殺";
            //        if (withBlock.IsFeatureAvailable(argfname) | t.IsWeaponClassifiedAs(tw, argattr1))
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
            //            string argoname1 = "ダメージ倍率低下";
            //            if (Expression.IsOptionDefined(argoname1))
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
            //            if (dmg >= withBlock.MaxHP / 20 | dmg >= withBlock.HP / 5)
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
            // TODO Impl
            //int LookForSupportRet = default;
            //Unit u;
            //int i;
            //var do_support = default(bool);
            //string team, uteam;
            //{
            //    var withBlock = MainPilot();
            //    // 自分自身がサポートを行うことが出来るか？
            //    string argsname = "援護";
            //    string argsname1 = "援護攻撃";
            //    string argsname2 = "援護防御";
            //    string argsname3 = "指揮";
            //    string argsname4 = "広域サポート";
            //    if (withBlock.IsSkillAvailable(argsname) | withBlock.IsSkillAvailable(argsname1) | withBlock.IsSkillAvailable(argsname2) | withBlock.IsSkillAvailable(argsname3) | withBlock.IsSkillAvailable(argsname4))
            //    {
            //        do_support = true;
            //    }

            //    string argIndex1 = "チーム";
            //    team = withBlock.SkillData(argIndex1);
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
            //        string argIndex2 = "混乱";
            //        string argIndex3 = "暴走";
            //        string argIndex4 = "恐怖";
            //        string argIndex5 = "狂戦士";
            //        if (withBlock1.IsConditionSatisfied(argIndex2) | withBlock1.IsConditionSatisfied(argIndex3) | withBlock1.IsConditionSatisfied(argIndex4) | withBlock1.IsConditionSatisfied(argIndex5))
            //        {
            //            goto NextUnit;
            //        }

            //        // 味方？
            //        var argt = this;
            //        if (IsEnemy(u) | withBlock1.IsEnemy(argt))
            //        {
            //            goto NextUnit;
            //        }

            //        // 同じチームに属している？
            //        string argIndex6 = "チーム";
            //        uteam = withBlock1.MainPilot().SkillData(argIndex6);
            //        if ((team ?? "") != (uteam ?? "") & !string.IsNullOrEmpty(team) & !string.IsNullOrEmpty(uteam))
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
            //            string argsname6 = "援護";
            //            string argsname7 = "援護攻撃";
            //            string argsname8 = "援護防御";
            //            string argsname9 = "指揮";
            //            string argsname10 = "広域サポート";
            //            if (withBlock2.IsSkillAvailable(argsname6) | withBlock2.IsSkillAvailable(argsname7))
            //            {
            //                LookForSupportRet = (LookForSupportRet + 1);
            //                // これから攻撃する場合、相手が行動出来ればサポートアタックが可能
            //                if (for_attack)
            //                {
            //                    if (u.Action > 0)
            //                    {
            //                        LookForSupportRet = (LookForSupportRet + 1);
            //                        // 同時援護攻撃が可能であればさらにボーナス
            //                        string argsname5 = "統率";
            //                        if (MainPilot().IsSkillAvailable(argsname5))
            //                        {
            //                            LookForSupportRet = (LookForSupportRet + 1);
            //                        }
            //                    }
            //                }
            //            }
            //            else if (withBlock2.IsSkillAvailable(argsname8) | withBlock2.IsSkillAvailable(argsname9) | withBlock2.IsSkillAvailable(argsname10))
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

        // 合体技のパートナーを探す
        // UPGRADE_NOTE: ctype は ctype_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public void CombinationPartner(string ctype_Renamed, int w, Unit[] partners, int tx = 0, int ty = 0, bool check_formation = false)
        {
            throw new NotImplementedException();
            //Unit u;
            //string uname;
            //int j, i, k;
            //int clevel = default, cnum = default;
            //var clist = default(string);
            //string cname;
            //int cmorale, cen, cplana = default;
            //int crange, loop_limit;

            //// 正常な判断が可能？
            //string argIndex1 = "混乱";
            //if (IsConditionSatisfied(argIndex1))
            //{
            //    partners = new Unit[1];
            //    return;
            //}

            //// 合体技のデータを調べておく
            //if (ctype_Renamed == "武装")
            //{
            //    cname = Weapon(w).Name;
            //    cen = WeaponENConsumption(w);
            //    cmorale = Weapon(w).NecessaryMorale;
            //    string argattr2 = "霊";
            //    string argattr3 = "プ";
            //    if (IsWeaponClassifiedAs(w, argattr2))
            //    {
            //        string argattr = "霊";
            //        cplana = (5d * WeaponLevel(w, argattr));
            //    }
            //    else if (IsWeaponClassifiedAs(w, argattr3))
            //    {
            //        string argattr1 = "プ";
            //        cplana = (5d * WeaponLevel(w, argattr1));
            //    }

            //    crange = WeaponMaxRange(w);
            //}
            //else
            //{
            //    cname = Ability(w).Name;
            //    cen = AbilityENConsumption(w);
            //    cmorale = Ability(w).NecessaryMorale;
            //    string argattr6 = "霊";
            //    string argattr7 = "プ";
            //    if (IsAbilityClassifiedAs(w, argattr6))
            //    {
            //        string argattr4 = "霊";
            //        cplana = (5d * AbilityLevel(w, argattr4));
            //    }
            //    else if (IsAbilityClassifiedAs(w, argattr7))
            //    {
            //        string argattr5 = "プ";
            //        cplana = (5d * AbilityLevel(w, argattr5));
            //    }

            //    crange = AbilityMaxRange(w);
            //}

            //// ユニットの特殊能力「合体技」の検索
            //var loopTo = CountFeature();
            //for (i = 1; i <= loopTo; i++)
            //{
            //    string argIndex4 = i;
            //    if (Feature(argIndex4) == "合体技")
            //    {
            //        string localFeatureData1() { string argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //        string arglist1 = localFeatureData1();
            //        if ((GeneralLib.LIndex(arglist1, 1) ?? "") == (cname ?? ""))
            //        {
            //            string argIndex3 = i;
            //            if (IsFeatureLevelSpecified(argIndex3))
            //            {
            //                string argIndex2 = i;
            //                clevel = FeatureLevel(argIndex2);
            //            }
            //            else
            //            {
            //                clevel = 0;
            //            }

            //            string localFeatureData() { string argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //            string arglist = localFeatureData();
            //            clist = GeneralLib.ListTail(arglist, 2);
            //            cnum = GeneralLib.LLength(clist);
            //            break;
            //        }
            //    }
            //}

            //if (i > CountFeature())
            //{
            //    partners = new Unit[1];
            //    return;
            //}

            //// 出撃していない場合
            //if (Status_Renamed != "出撃" | string.IsNullOrEmpty(Map.MapFileName))
            //{
            //    // パートナーが仲間にいるだけでよい
            //    var loopTo1 = cnum;
            //    for (i = 1; i <= loopTo1; i++)
            //    {
            //        uname = GeneralLib.LIndex(clist, i);

            //        // パートナーがユニット名で指定されている場合
            //        string argIndex6 = uname;
            //        if (SRC.UList.IsDefined(argIndex6))
            //        {
            //            string argIndex5 = uname;
            //            {
            //                var withBlock = SRC.UList.Item(argIndex5);
            //                if (withBlock.Status_Renamed == "出撃" | withBlock.Status_Renamed == "待機")
            //                {
            //                    goto NextPartner;
            //                }
            //            }
            //        }

            //        // パートナーがパイロット名で指定されている場合
            //        string argIndex7 = uname;
            //        if (SRC.PList.IsDefined(argIndex7))
            //        {
            //            Pilot localItem1() { string argIndex1 = uname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //            Pilot localItem2() { string argIndex1 = uname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //            if (localItem2().Unit_Renamed is object)
            //            {
            //                Pilot localItem() { string argIndex1 = uname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                {
            //                    var withBlock1 = localItem().Unit_Renamed;
            //                    if (withBlock1.Status_Renamed == "出撃" | withBlock1.Status_Renamed == "待機")
            //                    {
            //                        goto NextPartner;
            //                    }
            //                }
            //            }
            //        }

            //        // パートナーが見つからなかった
            //        partners = new Unit[1];
            //        return;
            //    NextPartner:
            //        ;
            //    }
            //    // パートナーが全員仲間にいる
            //    partners = new Unit[(cnum + 1)];
            //    return;
            //}

            //// 合体技の基点の設定
            //if (tx == 0)
            //{
            //    tx = x;
            //}

            //if (ty == 0)
            //{
            //    ty = y;
            //}

            //// パートナーの検索範囲を設定

            //if (crange == 1)
            //{
            //    if (cnum >= 8)
            //    {
            //        // 射程１で８体合体以上の場合は２マス以内
            //        loop_limit = 12;
            //    }
            //    else if (cnum >= 4)
            //    {
            //        // 射程１で４体合体以上の場合は斜め隣接可
            //        loop_limit = 8;
            //    }
            //    else
            //    {
            //        // どれにも該当していなければ隣接のみ
            //        loop_limit = 4;
            //    }
            //}
            //else if (cnum >= 9)
            //{
            //    // 射程２以上で９体合体以上の場合は２マス以内
            //    loop_limit = 12;
            //}
            //else if (cnum >= 5)
            //{
            //    // 射程２以上で５体合体以上の場合は斜め隣接可
            //    loop_limit = 8;
            //}
            //else
            //{
            //    // どれにも該当していなければ隣接のみ
            //    loop_limit = 4;
            //}

            //// 合体技斜め隣接可オプション
            //string argoname = "合体技斜め隣接可";
            //if (Expression.IsOptionDefined(argoname))
            //{
            //    if (loop_limit == 4)
            //    {
            //        loop_limit = 8;
            //    }
            //}

            //partners = new Unit[1];
            //var loopTo2 = cnum;
            //for (i = 1; i <= loopTo2; i++)
            //{
            //    // パートナーの名称
            //    uname = GeneralLib.LIndex(clist, i);
            //    var loopTo3 = loop_limit;
            //    for (j = 1; j <= loopTo3; j++)
            //    {
            //        // パートナーの検索位置設定
            //        // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //        u = null;
            //        switch (j)
            //        {
            //            case 1:
            //                {
            //                    if (tx > 1)
            //                    {
            //                        u = Map.MapDataForUnit[tx - 1, ty];
            //                    }

            //                    break;
            //                }

            //            case 2:
            //                {
            //                    if (tx < Map.MapWidth)
            //                    {
            //                        u = Map.MapDataForUnit[tx + 1, ty];
            //                    }

            //                    break;
            //                }

            //            case 3:
            //                {
            //                    if (ty > 1)
            //                    {
            //                        u = Map.MapDataForUnit[tx, ty - 1];
            //                    }

            //                    break;
            //                }

            //            case 4:
            //                {
            //                    if (ty < Map.MapHeight)
            //                    {
            //                        u = Map.MapDataForUnit[tx, ty + 1];
            //                    }

            //                    break;
            //                }

            //            case 5:
            //                {
            //                    if (tx > 1 & ty > 1)
            //                    {
            //                        u = Map.MapDataForUnit[tx - 1, ty - 1];
            //                    }

            //                    break;
            //                }

            //            case 6:
            //                {
            //                    if (tx < Map.MapWidth & ty < Map.MapHeight)
            //                    {
            //                        u = Map.MapDataForUnit[tx + 1, ty + 1];
            //                    }

            //                    break;
            //                }

            //            case 7:
            //                {
            //                    if (tx > 1 & ty < Map.MapHeight)
            //                    {
            //                        u = Map.MapDataForUnit[tx - 1, ty + 1];
            //                    }

            //                    break;
            //                }

            //            case 8:
            //                {
            //                    if (tx < Map.MapWidth & ty > 1)
            //                    {
            //                        u = Map.MapDataForUnit[tx + 1, ty - 1];
            //                    }

            //                    break;
            //                }

            //            case 9:
            //                {
            //                    if (tx > 2)
            //                    {
            //                        u = Map.MapDataForUnit[tx - 2, ty];
            //                    }

            //                    break;
            //                }

            //            case 10:
            //                {
            //                    if (tx < Map.MapWidth - 1)
            //                    {
            //                        u = Map.MapDataForUnit[tx + 2, ty];
            //                    }

            //                    break;
            //                }

            //            case 11:
            //                {
            //                    if (ty > 2)
            //                    {
            //                        u = Map.MapDataForUnit[tx, ty - 2];
            //                    }

            //                    break;
            //                }

            //            case 12:
            //                {
            //                    if (ty < Map.MapHeight - 1)
            //                    {
            //                        u = Map.MapDataForUnit[tx, ty + 2];
            //                    }

            //                    break;
            //                }
            //        }

            //        // ユニットが存在する？
            //        if (u is null)
            //        {
            //            goto NextNeighbor;
            //        }

            //        {
            //            var withBlock2 = u;
            //            // 合体技のパートナーに該当する？
            //            if ((withBlock2.Name ?? "") != (uname ?? ""))
            //            {
            //                // パイロット名でも確認
            //                if ((withBlock2.MainPilot().Name ?? "") != (uname ?? ""))
            //                {
            //                    goto NextNeighbor;
            //                }
            //            }

            //            // ユニットが自分？
            //            if (ReferenceEquals(u, this))
            //            {
            //                goto NextNeighbor;
            //            }

            //            // 既に選択済み？
            //            var loopTo4 = Information.UBound(partners);
            //            for (k = 1; k <= loopTo4; k++)
            //            {
            //                if (ReferenceEquals(u, partners[k]))
            //                {
            //                    goto NextNeighbor;
            //                }
            //            }

            //            // ユニットが敵？
            //            if (IsEnemy(u))
            //            {
            //                goto NextNeighbor;
            //            }

            //            // 行動出来なければだめ
            //            string argIndex8 = "混乱";
            //            string argIndex9 = "恐怖";
            //            string argIndex10 = "憑依";
            //            if (withBlock2.MaxAction() == 0 | withBlock2.IsConditionSatisfied(argIndex8) | withBlock2.IsConditionSatisfied(argIndex9) | withBlock2.IsConditionSatisfied(argIndex10))
            //            {
            //                goto NextNeighbor;
            //            }

            //            // 合体技にレベルが設定されていればパイロット間の信頼度をチェック
            //            if (clevel > 0)
            //            {
            //                if (MainPilot().Relation(withBlock2.MainPilot()) < clevel | withBlock2.MainPilot().Relation(MainPilot()) < clevel)
            //                {
            //                    goto NextNeighbor;
            //                }
            //            }

            //            // パートナーが武器を使うための条件を満たしているかを判定
            //            if (!check_formation)
            //            {
            //                if (ctype_Renamed == "武装")
            //                {
            //                    // 合体技と同名の武器を検索
            //                    var loopTo5 = withBlock2.CountWeapon();
            //                    for (k = 1; k <= loopTo5; k++)
            //                    {
            //                        if ((withBlock2.Weapon(k).Name ?? "") == (cname ?? ""))
            //                        {
            //                            break;
            //                        }
            //                    }

            //                    if (k <= withBlock2.CountWeapon())
            //                    {
            //                        // 武器が使える？
            //                        if (!withBlock2.IsWeaponMastered(k))
            //                        {
            //                            goto NextNeighbor;
            //                        }

            //                        if (withBlock2.Weapon(k).NecessaryMorale > 0)
            //                        {
            //                            if (withBlock2.MainPilot().Morale < withBlock2.Weapon(k).NecessaryMorale)
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }

            //                        if (withBlock2.WeaponENConsumption(k) > 0)
            //                        {
            //                            if (withBlock2.EN < withBlock2.WeaponENConsumption(k))
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }

            //                        if (withBlock2.Weapon(k).Bullet > 0)
            //                        {
            //                            if (withBlock2.Bullet(k) == 0)
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }

            //                        string argattr10 = "霊";
            //                        string argattr11 = "プ";
            //                        if (withBlock2.WeaponLevel(k, argattr10) > 0d)
            //                        {
            //                            string argattr8 = "霊";
            //                            if (withBlock2.MainPilot().Plana < 5d * withBlock2.WeaponLevel(k, argattr8))
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }
            //                        else if (withBlock2.WeaponLevel(k, argattr11) > 0d)
            //                        {
            //                            string argattr9 = "プ";
            //                            if (withBlock2.MainPilot().Plana < 5d * withBlock2.WeaponLevel(k, argattr9))
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        // 同名の武器を持っていなかった場合はチェック項目を限定
            //                        if (cmorale > 0)
            //                        {
            //                            if (withBlock2.MainPilot().Morale < cmorale)
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }

            //                        if (cen > 0)
            //                        {
            //                            if (withBlock2.EN < cen)
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }

            //                        if (cplana > 0)
            //                        {
            //                            if (withBlock2.MainPilot().Plana < cplana)
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }
            //                    }
            //                }
            //                else
            //                {
            //                    // 合体技と同名のアビリティを検索
            //                    var loopTo6 = withBlock2.CountAbility();
            //                    for (k = 1; k <= loopTo6; k++)
            //                    {
            //                        if ((withBlock2.Ability(k).Name ?? "") == (cname ?? ""))
            //                        {
            //                            break;
            //                        }
            //                    }

            //                    if (k <= withBlock2.CountAbility())
            //                    {
            //                        // アビリティが使える？
            //                        if (!withBlock2.IsAbilityMastered(k))
            //                        {
            //                            goto NextNeighbor;
            //                        }

            //                        if (withBlock2.Ability(k).NecessaryMorale > 0)
            //                        {
            //                            if (withBlock2.MainPilot().Morale < withBlock2.Ability(k).NecessaryMorale)
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }

            //                        if (withBlock2.AbilityENConsumption(k) > 0)
            //                        {
            //                            if (withBlock2.EN < withBlock2.AbilityENConsumption(k))
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }

            //                        if (withBlock2.Ability(k).Stock > 0)
            //                        {
            //                            if (withBlock2.Stock(k) == 0)
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }

            //                        string argattr14 = "霊";
            //                        string argattr15 = "プ";
            //                        if (withBlock2.AbilityLevel(k, argattr14) > 0d)
            //                        {
            //                            string argattr12 = "霊";
            //                            if (withBlock2.MainPilot().Plana < 5d * withBlock2.AbilityLevel(k, argattr12))
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }
            //                        else if (withBlock2.AbilityLevel(k, argattr15) > 0d)
            //                        {
            //                            string argattr13 = "プ";
            //                            if (withBlock2.MainPilot().Plana < 5d * withBlock2.AbilityLevel(k, argattr13))
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        // 同名のアビリティを持っていなかった場合はチェック項目を限定
            //                        if (cmorale > 0)
            //                        {
            //                            if (withBlock2.MainPilot().Morale < cmorale)
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }

            //                        if (cen > 0)
            //                        {
            //                            if (withBlock2.EN < cen)
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }

            //                        if (cplana > 0)
            //                        {
            //                            if (withBlock2.MainPilot().Plana < cplana)
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //            // フォーメーションのチェックだけの時も必要技能は調べておく
            //            else if (ctype_Renamed == "武装")
            //            {
            //                var loopTo7 = withBlock2.CountWeapon();
            //                for (k = 1; k <= loopTo7; k++)
            //                {
            //                    if ((withBlock2.Weapon(k).Name ?? "") == (cname ?? ""))
            //                    {
            //                        break;
            //                    }
            //                }

            //                if (k <= withBlock2.CountWeapon())
            //                {
            //                    if (!withBlock2.IsWeaponMastered(k))
            //                    {
            //                        goto NextNeighbor;
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                var loopTo8 = withBlock2.CountAbility();
            //                for (k = 1; k <= loopTo8; k++)
            //                {
            //                    if ((withBlock2.Ability(k).Name ?? "") == (cname ?? ""))
            //                    {
            //                        break;
            //                    }
            //                }

            //                if (k <= withBlock2.CountAbility())
            //                {
            //                    if (!withBlock2.IsAbilityMastered(k))
            //                    {
            //                        goto NextNeighbor;
            //                    }
            //                }
            //            }
            //        }

            //        // 見つかったパートナーを記録
            //        Array.Resize(partners, i + 1);
            //        partners[i] = u;
            //        break;
            //    NextNeighbor:
            //        ;
            //    }

            //    // パートナーが見つからなかった？
            //    if (j > loop_limit)
            //    {
            //        partners = new Unit[1];
            //        return;
            //    }
            //}

            //// 合体技メッセージ判定用にパートナー一覧を記録
            //Commands.SelectedPartners = new Unit[Information.UBound(partners) + 1];
            //var loopTo9 = Information.UBound(partners);
            //for (i = 1; i <= loopTo9; i++)
            //    Commands.SelectedPartners[i] = partners[i];
        }

        // 合体技攻撃に必要なパートナーが見つかるか？
        public bool IsCombinationAttackAvailable(int w, bool check_formation = false)
        {
            throw new NotImplementedException();
            //bool IsCombinationAttackAvailableRet = default;
            //Unit[] partners;
            //partners = new Unit[1];
            //string argattr = "Ｍ";
            //if (Status_Renamed == "待機" | string.IsNullOrEmpty(Map.MapFileName))
            //{
            //    // 出撃時以外は相手が仲間にいるだけでＯＫ
            //    string argctype_Renamed = "武装";
            //    CombinationPartner(argctype_Renamed, w, partners, x, y);
            //}
            //else if (WeaponMaxRange(w) == 1 & !IsWeaponClassifiedAs(w, argattr))
            //{
            //    // 射程１の場合は自分の周りのいずれかの敵ユニットに対して合体技が使えればＯＫ
            //    if (x > 1)
            //    {
            //        if (Map.MapDataForUnit[x - 1, y] is object)
            //        {
            //            if (IsEnemy(Map.MapDataForUnit[x - 1, y]))
            //            {
            //                string argctype_Renamed2 = "武装";
            //                CombinationPartner(argctype_Renamed2, w, partners, (x - 1), y, check_formation);
            //            }
            //        }
            //    }

            //    if (Information.UBound(partners) == 0)
            //    {
            //        if (x < Map.MapWidth)
            //        {
            //            if (Map.MapDataForUnit[x + 1, y] is object)
            //            {
            //                if (IsEnemy(Map.MapDataForUnit[x + 1, y]))
            //                {
            //                    string argctype_Renamed3 = "武装";
            //                    CombinationPartner(argctype_Renamed3, w, partners, (x + 1), y, check_formation);
            //                }
            //            }
            //        }
            //    }

            //    if (Information.UBound(partners) == 0)
            //    {
            //        if (y > 1)
            //        {
            //            if (Map.MapDataForUnit[x, y - 1] is object)
            //            {
            //                if (IsEnemy(Map.MapDataForUnit[x, y - 1]))
            //                {
            //                    string argctype_Renamed4 = "武装";
            //                    CombinationPartner(argctype_Renamed4, w, partners, x, (y - 1), check_formation);
            //                }
            //            }
            //        }
            //    }

            //    if (Information.UBound(partners) == 0)
            //    {
            //        if (y < Map.MapHeight)
            //        {
            //            if (Map.MapDataForUnit[x, y + 1] is object)
            //            {
            //                if (IsEnemy(Map.MapDataForUnit[x, y + 1]))
            //                {
            //                    string argctype_Renamed5 = "武装";
            //                    CombinationPartner(argctype_Renamed5, w, partners, x, (y + 1), check_formation);
            //                }
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    // 射程２以上の場合は自分の周りにパートナーがいればＯＫ
            //    string argctype_Renamed1 = "武装";
            //    CombinationPartner(argctype_Renamed1, w, partners, x, y, check_formation);
            //}

            //// 条件を満たすパートナーの組が見つかったか判定
            //if (Information.UBound(partners) > 0)
            //{
            //    IsCombinationAttackAvailableRet = true;
            //}
            //else
            //{
            //    IsCombinationAttackAvailableRet = false;
            //}

            //return IsCombinationAttackAvailableRet;
        }

        // 合体技アビリティに必要なパートナーが見つかるか？
        public bool IsCombinationAbilityAvailable(int a, bool check_formation = false)
        {
            throw new NotImplementedException();
            //bool IsCombinationAbilityAvailableRet = default;
            //Unit[] partners;
            //partners = new Unit[1];
            //string argattr = "Ｍ";
            //if (Status_Renamed == "待機" | string.IsNullOrEmpty(Map.MapFileName))
            //{
            //    // 出撃時以外は相手が仲間にいるだけでＯＫ
            //    string argctype_Renamed = "アビリティ";
            //    CombinationPartner(argctype_Renamed, a, partners, x, y);
            //}
            //else if (AbilityMaxRange(a) == 1 & !IsAbilityClassifiedAs(a, argattr))
            //{
            //    // 射程１の場合は自分の周りのいずれかの味方ユニットに対して合体技が使えればＯＫ
            //    if (x > 1)
            //    {
            //        if (Map.MapDataForUnit[x - 1, y] is object)
            //        {
            //            if (IsAlly(Map.MapDataForUnit[x - 1, y]))
            //            {
            //                string argctype_Renamed2 = "アビリティ";
            //                CombinationPartner(argctype_Renamed2, a, partners, (x - 1), y, check_formation);
            //            }
            //        }
            //    }

            //    if (Information.UBound(partners) == 0)
            //    {
            //        if (x < Map.MapWidth)
            //        {
            //            if (Map.MapDataForUnit[x + 1, y] is object)
            //            {
            //                if (IsAlly(Map.MapDataForUnit[x + 1, y]))
            //                {
            //                    string argctype_Renamed3 = "アビリティ";
            //                    CombinationPartner(argctype_Renamed3, a, partners, (x + 1), y, check_formation);
            //                }
            //            }
            //        }
            //    }

            //    if (Information.UBound(partners) == 0)
            //    {
            //        if (y > 1)
            //        {
            //            if (Map.MapDataForUnit[x, y - 1] is object)
            //            {
            //                if (IsAlly(Map.MapDataForUnit[x, y - 1]))
            //                {
            //                    string argctype_Renamed4 = "アビリティ";
            //                    CombinationPartner(argctype_Renamed4, a, partners, x, (y - 1), check_formation);
            //                }
            //            }
            //        }
            //    }

            //    if (Information.UBound(partners) == 0)
            //    {
            //        if (y > Map.MapHeight)
            //        {
            //            if (Map.MapDataForUnit[x, y + 1] is object)
            //            {
            //                if (IsAlly(Map.MapDataForUnit[x, y + 1]))
            //                {
            //                    string argctype_Renamed5 = "アビリティ";
            //                    CombinationPartner(argctype_Renamed5, a, partners, x, (y + 1), check_formation);
            //                }
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    // 射程２以上の場合は自分の周りにパートナーがいればＯＫ
            //    string argctype_Renamed1 = "アビリティ";
            //    CombinationPartner(argctype_Renamed1, a, partners, x, y, check_formation);
            //}

            //// 条件を満たすパートナーの組が見つかったか判定
            //if (Information.UBound(partners) > 0)
            //{
            //    IsCombinationAbilityAvailableRet = true;
            //}
            //else
            //{
            //    IsCombinationAbilityAvailableRet = false;
            //}

            //return IsCombinationAbilityAvailableRet;
        }
    }
}
