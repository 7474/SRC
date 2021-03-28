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

        // 合体技アビリティに必要なパートナーが見つかるか？
        public bool IsCombinationAbilityAvailable(int a, bool check_formation = false)
        {
            throw new NotImplementedException();
            //bool IsCombinationAbilityAvailableRet = default;
            //Unit[] partners;
            //partners = new Unit[1];
            //string argattr = "Ｍ";
            //if (Status == "待機" | string.IsNullOrEmpty(Map.MapFileName))
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

        // (tx,ty)にユニットが進入可能か？
        public bool IsAbleToEnter(int tx, int ty)
        {
            return true;
            // TODO Impl
            //bool IsAbleToEnterRet = default;
            //bool ignore_move_cost;

            //// 使用不能の形態はどの地形に対しても進入不可能とみなす
            //if (!IsAvailable())
            //{
            //    IsAbleToEnterRet = false;
            //    return IsAbleToEnterRet;
            //}

            //// 単に必要技能をチェックしている場合？
            //if (string.IsNullOrEmpty(Map.MapFileName))
            //{
            //    IsAbleToEnterRet = true;
            //    return IsAbleToEnterRet;
            //}

            //// マップ外？
            //if (tx < 1 | Map.MapWidth < tx | ty < 1 | Map.MapHeight < ty)
            //{
            //    IsAbleToEnterRet = false;
            //    return IsAbleToEnterRet;
            //}

            //// 地形適応チェック
            //switch (Map.TerrainClass(tx, ty) ?? "")
            //{
            //    case "空":
            //        {
            //            string argarea_name = "空";
            //            string argfname = "空中移動";
            //            if (!IsTransAvailable(argarea_name) & !CurrentForm().IsFeatureAvailable(argfname))
            //            {
            //                IsAbleToEnterRet = false;
            //                return IsAbleToEnterRet;
            //            }

            //            break;
            //        }

            //    case "水":
            //        {
            //            string argarea_name1 = "空";
            //            string argfname1 = "空中移動";
            //            string argarea_name2 = "水上";
            //            if (IsTransAvailable(argarea_name1) | CurrentForm().IsFeatureAvailable(argfname1) | IsTransAvailable(argarea_name2))
            //            {
            //                IsAbleToEnterRet = true;
            //                return IsAbleToEnterRet;
            //            }

            //            string argfname2 = "水中移動";
            //            if (get_Adaption(3) == 0 & !CurrentForm().IsFeatureAvailable(argfname2))
            //            {
            //                IsAbleToEnterRet = false;
            //                return IsAbleToEnterRet;
            //            }

            //            break;
            //        }

            //    case "深水":
            //        {
            //            string argarea_name3 = "空";
            //            string argfname3 = "空中移動";
            //            string argarea_name4 = "水上";
            //            if (IsTransAvailable(argarea_name3) | CurrentForm().IsFeatureAvailable(argfname3) | IsTransAvailable(argarea_name4))
            //            {
            //                IsAbleToEnterRet = true;
            //                return IsAbleToEnterRet;
            //            }

            //            string argarea_name5 = "水";
            //            string argfname4 = "水中移動";
            //            if (!IsTransAvailable(argarea_name5) & !CurrentForm().IsFeatureAvailable(argfname4))
            //            {
            //                IsAbleToEnterRet = false;
            //                return IsAbleToEnterRet;
            //            }

            //            break;
            //        }

            //    case "宇宙":
            //        {
            //            string argfname5 = "宇宙移動";
            //            if (get_Adaption(4) == 0 & !CurrentForm().IsFeatureAvailable(argfname5))
            //            {
            //                IsAbleToEnterRet = false;
            //                return IsAbleToEnterRet;
            //            }

            //            break;
            //        }

            //    case "月面":
            //        {
            //            string argarea_name6 = "空";
            //            string argfname6 = "空中移動";
            //            string argarea_name7 = "宇";
            //            string argfname7 = "宇宙移動";
            //            if (IsTransAvailable(argarea_name6) | CurrentForm().IsFeatureAvailable(argfname6) | IsTransAvailable(argarea_name7) | CurrentForm().IsFeatureAvailable(argfname7))
            //            {
            //                IsAbleToEnterRet = true;
            //                return IsAbleToEnterRet;
            //            }

            //            break;
            //        }

            //    default:
            //        {
            //            string argarea_name8 = "空";
            //            string argfname8 = "空中移動";
            //            if (IsTransAvailable(argarea_name8) | CurrentForm().IsFeatureAvailable(argfname8))
            //            {
            //                IsAbleToEnterRet = true;
            //                return IsAbleToEnterRet;
            //            }

            //            string argarea_name9 = "陸";
            //            string argfname9 = "陸上移動";
            //            if (!IsTransAvailable(argarea_name9) & !CurrentForm().IsFeatureAvailable(argfname9))
            //            {
            //                IsAbleToEnterRet = false;
            //                return IsAbleToEnterRet;
            //            }

            //            break;
            //        }
            //}

            //// 進入不能？
            //if (Map.TerrainMoveCost(tx, ty) >= 1000)
            //{
            //    IsAbleToEnterRet = false;
            //    return IsAbleToEnterRet;
            //}

            //IsAbleToEnterRet = true;
            //return IsAbleToEnterRet;
        }

        // この形態が使用可能か？ (Disable＆必要技能のチェック)
        public bool IsAvailable()
        {
            return true;
            // TODO Impl
            //bool IsAvailableRet = default;
            //int i;
            //IsAvailableRet = true;

            //// イベントコマンド「Disable」
            //bool localIsDisabled() { string argfname = Name; var ret = IsDisabled(argfname); Name = argfname; return ret; }

            //if (localIsDisabled())
            //{
            //    IsAvailableRet = false;
            //    return IsAvailableRet;
            //}

            //// 制限時間の切れた形態？
            //if (Status == "他形態")
            //{
            //    object argIndex1 = "行動不能";
            //    if (IsConditionSatisfied(argIndex1))
            //    {
            //        IsAvailableRet = false;
            //        return IsAvailableRet;
            //    }
            //}

            //{
            //    var withBlock = CurrentForm();
            //    // 技能チェックが必要？
            //    string argfname = "必要技能";
            //    string argfname1 = "不必要技能";
            //    if (withBlock.CountPilot() == 0 | !IsFeatureAvailable(argfname) & !IsFeatureAvailable(argfname1))
            //    {
            //        return IsAvailableRet;
            //    }

            //    // 必要技能をチェック
            //    var loopTo = CountFeature();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        object argIndex2 = i;
            //        switch (Feature(argIndex2) ?? "")
            //        {
            //            case "必要技能":
            //                {
            //                    string localFeatureData() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //                    bool localIsNecessarySkillSatisfied() { string argnabilities = hsc9e6151c7b7e42d6b233bb86f17bea66(); Pilot argp = null; var ret = withBlock.IsNecessarySkillSatisfied(argnabilities, p: argp); return ret; }

            //                    if (!localIsNecessarySkillSatisfied())
            //                    {
            //                        IsAvailableRet = false;
            //                        return IsAvailableRet;
            //                    }

            //                    break;
            //                }

            //            case "不必要技能":
            //                {
            //                    string localFeatureData1() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //                    string argnabilities = localFeatureData1();
            //                    Pilot argp = null;
            //                    if (withBlock.IsNecessarySkillSatisfied(argnabilities, p: argp))
            //                    {
            //                        IsAvailableRet = false;
            //                        return IsAvailableRet;
            //                    }

            //                    break;
            //                }
            //        }
            //    }
            //}

            //return IsAvailableRet;
        }

        // 必要技能を満たしているか？
        public bool IsNecessarySkillSatisfied(string nabilities, [Optional, DefaultParameterValue(null)] Pilot p)
        {
            return true;
            // TODO Impl
            //bool IsNecessarySkillSatisfiedRet = default;
            //int i, num;
            //var nskill_list = new string[101];
            //if (Strings.Len(nabilities) == 0)
            //{
            //    IsNecessarySkillSatisfiedRet = true;
            //    return IsNecessarySkillSatisfiedRet;
            //}

            //num = GeneralLib.LLength(nabilities);
            //var loopTo = (int)GeneralLib.MinLng(num, 100);
            //for (i = 1; i <= loopTo; i++)
            //    nskill_list[i] = GeneralLib.LIndex(nabilities, i);

            //// 個々の必要条件をチェック
            //i = 1;
            //while (i <= GeneralLib.MinLng(num, 100))
            //{
            //    if (IsNecessarySkillSatisfied2(nskill_list[i], p))
            //    {
            //        // 必要条件が満たされた場合、その後の「or」をスキップ
            //        if (i <= num - 2)
            //        {
            //            while (Strings.LCase(nskill_list[i + 1]) == "or")
            //            {
            //                i = (int)(i + 2);
            //                // 検査する必要条件が無くなったので必要技能が満たされたと判定
            //                if (i == num)
            //                {
            //                    IsNecessarySkillSatisfiedRet = true;
            //                    return IsNecessarySkillSatisfiedRet;
            //                }
            //                else if (i > num)
            //                {
            //                    // orの後ろに必要条件がない
            //                    string argmsg = Name + "に対する必要技能「" + nabilities + "」が不正です";
            //                    GUI.ErrorMessage(argmsg);
            //                    SRC.TerminateSRC();
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        // 必要条件が満たされなかった場合、その後に「or」がなければ
            //        // 必要技能が満たされなかったと判定
            //        if (i > num - 2)
            //        {
            //            return IsNecessarySkillSatisfiedRet;
            //        }

            //        i = (int)(i + 1);
            //        if (Strings.LCase(nskill_list[i]) != "or")
            //        {
            //            return IsNecessarySkillSatisfiedRet;
            //        }
            //    }

            //    i = (int)(i + 1);
            //}

            //IsNecessarySkillSatisfiedRet = true;
            //return IsNecessarySkillSatisfiedRet;
        }

        public bool IsNecessarySkillSatisfied2(string ndata, Pilot p)
        {
            return true;
            // TODO Impl
            //bool IsNecessarySkillSatisfied2Ret = default;
            //string stype2, stype, sname;
            //double slevel;
            //double nlevel;
            //var mp = default(Pilot);
            //int i, j;

            //// ステータスコマンド実行時は条件が満たされていると見なす？
            //if (Strings.Left(ndata, 1) == "+")
            //{
            //    if (Status == "出撃" & InterMission.InStatusCommand())
            //    {
            //        IsNecessarySkillSatisfied2Ret = true;
            //        return IsNecessarySkillSatisfied2Ret;
            //    }

            //    ndata = Strings.Mid(ndata, 2);
            //}

            //// 召喚者技能を参照？
            //if (Strings.Left(ndata, 1) == "*")
            //{
            //    if (Summoner is null)
            //    {
            //        return IsNecessarySkillSatisfied2Ret;
            //    }

            //    string argndata = Strings.Mid(ndata, 2);
            //    Pilot argp = null;
            //    IsNecessarySkillSatisfied2Ret = Summoner.IsNecessarySkillSatisfied2(argndata, argp);
            //    return IsNecessarySkillSatisfied2Ret;
            //}

            //i = (int)Strings.InStr(ndata, "Lv");
            //if (i > 0)
            //{
            //    sname = Strings.Left(ndata, i - 1);
            //    string argexpr = Strings.Mid(ndata, i + 2);
            //    nlevel = GeneralLib.StrToDbl(argexpr);
            //}
            //else
            //{
            //    sname = ndata;
            //    nlevel = 1d;
            //}

            //// 不必要技能？
            //if (Strings.Left(sname, 1) == "!")
            //{
            //    bool localIsNecessarySkillSatisfied2() { string argndata = Strings.Mid(ndata, 2); var ret = IsNecessarySkillSatisfied2(argndata, p); return ret; }

            //    IsNecessarySkillSatisfied2Ret = !localIsNecessarySkillSatisfied2();
            //    return IsNecessarySkillSatisfied2Ret;
            //}

            //// 必要技能の判定に使用するパイロットを設定
            //if (p is null)
            //{
            //    if (CountPilot() > 0)
            //    {
            //        mp = MainPilot();
            //    }
            //    else
            //    {
            //        {
            //            var withBlock = CurrentForm();
            //            if (withBlock.CountPilot() > 0)
            //            {
            //                mp = withBlock.MainPilot();
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    mp = p;
            //}

            //// ダミーパイロットの場合は無視
            //if (mp is object)
            //{
            //    if (mp.Nickname0 == "パイロット不在")
            //    {
            //        // UPGRADE_NOTE: オブジェクト mp をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //        mp = null;
            //    }
            //}

            //slevel = -10000;

            //// まず名称が変わらない必要技能を判定
            //switch (sname ?? "")
            //{
            //    case "レベル":
            //        {
            //            if (mp is object)
            //            {
            //                slevel = mp.Level;
            //            }
            //            else
            //            {
            //                slevel = 0d;
            //            }

            //            break;
            //        }

            //    case "格闘":
            //        {
            //            if (mp is object)
            //            {
            //                slevel = mp.InfightBase;
            //            }
            //            else
            //            {
            //                slevel = 0d;
            //            }

            //            break;
            //        }

            //    case "射撃":
            //        {
            //            slevel = 0d;
            //            if (mp is object)
            //            {
            //                if (!mp.HasMana())
            //                {
            //                    slevel = mp.ShootingBase;
            //                }
            //            }

            //            break;
            //        }

            //    case "魔力":
            //        {
            //            slevel = 0d;
            //            if (mp is object)
            //            {
            //                if (mp.HasMana())
            //                {
            //                    slevel = mp.ShootingBase;
            //                }
            //            }

            //            break;
            //        }

            //    case "命中":
            //        {
            //            if (mp is object)
            //            {
            //                slevel = mp.HitBase;
            //            }
            //            else
            //            {
            //                slevel = 0d;
            //            }

            //            break;
            //        }

            //    case "回避":
            //        {
            //            if (mp is object)
            //            {
            //                slevel = mp.DodgeBase;
            //            }
            //            else
            //            {
            //                slevel = 0d;
            //            }

            //            break;
            //        }

            //    case "技量":
            //        {
            //            if (mp is object)
            //            {
            //                slevel = mp.TechniqueBase;
            //            }
            //            else
            //            {
            //                slevel = 0d;
            //            }

            //            break;
            //        }

            //    case "反応":
            //        {
            //            if (mp is object)
            //            {
            //                slevel = mp.IntuitionBase;
            //            }
            //            else
            //            {
            //                slevel = 0d;
            //            }

            //            break;
            //        }

            //    case "格闘初期値":
            //        {
            //            slevel = 0d;
            //            if (mp is object)
            //            {
            //                string argoname = "攻撃力低成長";
            //                if (Expression.IsOptionDefined(argoname))
            //                {
            //                    slevel = mp.InfightBase - mp.Level / 2;
            //                }
            //                else
            //                {
            //                    slevel = mp.InfightBase - mp.Level;
            //                }
            //            }

            //            break;
            //        }

            //    case "射撃初期値":
            //        {
            //            slevel = 0d;
            //            if (mp is object)
            //            {
            //                if (!mp.HasMana())
            //                {
            //                    string argoname1 = "攻撃力低成長";
            //                    if (Expression.IsOptionDefined(argoname1))
            //                    {
            //                        slevel = mp.ShootingBase - mp.Level / 2;
            //                    }
            //                    else
            //                    {
            //                        slevel = mp.ShootingBase - mp.Level;
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "魔力初期値":
            //        {
            //            slevel = 0d;
            //            if (mp is object)
            //            {
            //                if (mp.HasMana())
            //                {
            //                    string argoname2 = "攻撃力低成長";
            //                    if (Expression.IsOptionDefined(argoname2))
            //                    {
            //                        slevel = mp.ShootingBase - mp.Level / 2;
            //                    }
            //                    else
            //                    {
            //                        slevel = mp.ShootingBase - mp.Level;
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "命中初期値":
            //        {
            //            slevel = 0d;
            //            if (mp is object)
            //            {
            //                slevel = mp.HitBase - mp.Level;
            //            }

            //            break;
            //        }

            //    case "回避初期値":
            //        {
            //            slevel = 0d;
            //            if (mp is object)
            //            {
            //                slevel = mp.DodgeBase - mp.Level;
            //            }

            //            break;
            //        }

            //    case "技量初期値":
            //        {
            //            slevel = 0d;
            //            if (mp is object)
            //            {
            //                slevel = mp.TechniqueBase - mp.Level;
            //            }

            //            break;
            //        }

            //    case "反応初期値":
            //        {
            //            slevel = 0d;
            //            if (mp is object)
            //            {
            //                slevel = mp.IntuitionBase - mp.Level;
            //            }

            //            break;
            //        }

            //    case "男性":
            //        {
            //            slevel = 0d;
            //            if (mp is object)
            //            {
            //                if (mp.Sex == "男性")
            //                {
            //                    slevel = 1d;
            //                }

            //                if (Data.PilotNum > 1)
            //                {
            //                    var loopTo = CountPilot();
            //                    for (i = 1; i <= loopTo; i++)
            //                    {
            //                        Pilot localPilot() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

            //                        if (localPilot().Sex == "男性")
            //                        {
            //                            slevel = 1d;
            //                        }
            //                    }
            //                }

            //                var loopTo1 = CountSupport();
            //                for (i = 1; i <= loopTo1; i++)
            //                {
            //                    Pilot localSupport() { object argIndex1 = i; var ret = Support(argIndex1); return ret; }

            //                    if (localSupport().Sex == "男性")
            //                    {
            //                        slevel = 1d;
            //                    }
            //                }

            //                string argfname = "追加サポート";
            //                if (IsFeatureAvailable(argfname))
            //                {
            //                    if (AdditionalSupport().Sex == "男性")
            //                    {
            //                        slevel = 1d;
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "女性":
            //        {
            //            slevel = 0d;
            //            if (mp is object)
            //            {
            //                if (mp.Sex == "女性")
            //                {
            //                    slevel = 1d;
            //                }

            //                if (Data.PilotNum > 1)
            //                {
            //                    var loopTo2 = CountPilot();
            //                    for (i = 1; i <= loopTo2; i++)
            //                    {
            //                        Pilot localPilot1() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

            //                        if (localPilot1().Sex == "女性")
            //                        {
            //                            slevel = 1d;
            //                        }
            //                    }
            //                }

            //                var loopTo3 = CountSupport();
            //                for (i = 1; i <= loopTo3; i++)
            //                {
            //                    Pilot localSupport1() { object argIndex1 = i; var ret = Support(argIndex1); return ret; }

            //                    if (localSupport1().Sex == "女性")
            //                    {
            //                        slevel = 1d;
            //                    }
            //                }

            //                string argfname1 = "追加サポート";
            //                if (IsFeatureAvailable(argfname1))
            //                {
            //                    if (AdditionalSupport().Sex == "女性")
            //                    {
            //                        slevel = 1d;
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "生身":
            //        {
            //            if (IsHero())
            //            {
            //                slevel = 1d;
            //            }
            //            else
            //            {
            //                slevel = 0d;
            //            }

            //            break;
            //        }

            //    case "瀕死":
            //        {
            //            if (HP <= MaxHP / 4)
            //            {
            //                slevel = 1d;
            //            }
            //            else
            //            {
            //                slevel = 0d;
            //            }

            //            break;
            //        }

            //    case "ＨＰ":
            //        {
            //            slevel = 10d * HP / MaxHP;
            //            break;
            //        }

            //    case "ＥＮ":
            //        {
            //            slevel = 10d * EN / MaxEN;
            //            break;
            //        }

            //    case "気力":
            //        {
            //            if (mp is object)
            //            {
            //                slevel = mp.Morale - 100d;
            //                slevel = slevel / 10d;
            //            }
            //            else
            //            {
            //                slevel = 0d;
            //            }

            //            break;
            //        }

            //    case "ランク":
            //        {
            //            slevel = Rank;
            //            break;
            //        }

            //    case "地上":
            //    case "空中":
            //    case "水中":
            //    case "水上":
            //    case "宇宙":
            //    case "地中":
            //        {
            //            slevel = 0d;
            //            if (Status == "出撃")
            //            {
            //                if ((sname ?? "") == (Area ?? ""))
            //                {
            //                    slevel = 1d;
            //                }
            //            }

            //            break;
            //        }

            //    case "アイテム":
            //        {
            //            // 使い捨てアイテム表記用
            //            slevel = 1d;
            //            break;
            //        }

            //    case "当て身技":
            //    case "自動反撃":
            //        {
            //            // アビリティで付加された当て身技及び自動反撃専用の武器が表示されるのを
            //            // 防ぐため、これらの必要技能は常に満たされないとみなす
            //            return IsNecessarySkillSatisfied2Ret;
            //        }
            //}

            //// 上の条件のいずれかに該当？
            //if (slevel != -10000)
            //{
            //    // 指定された技能のレベルが必要なレベル以上の場合に必要技能が満たされたと判定
            //    if (slevel >= nlevel)
            //    {
            //        IsNecessarySkillSatisfied2Ret = true;
            //    }

            //    return IsNecessarySkillSatisfied2Ret;
            //}

            //// 必要技能の種類を判別
            //if (mp is object)
            //{
            //    stype = mp.SkillType(sname);
            //}
            //else
            //{
            //    stype = sname;
            //}

            //// 名称が変わる可能性がある必要技能を判定
            //string iname;
            //string uname;
            //Unit u;
            //int max_range;
            //switch (stype ?? "")
            //{
            //    case "超感覚":
            //        {
            //            if (p is object)
            //            {
            //                object argIndex1 = "超感覚";
            //                string argref_mode = "";
            //                slevel = p.SkillLevel(argIndex1, ref_mode: argref_mode);
            //                if ((stype ?? "") != (sname ?? ""))
            //                {
            //                    if ((p.SkillNameForNS(stype) ?? "") != (sname ?? ""))
            //                    {
            //                        slevel = 0d;
            //                    }
            //                }

            //                object argIndex2 = "知覚強化";
            //                string argref_mode1 = "";
            //                slevel = slevel + p.SkillLevel(argIndex2, ref_mode: argref_mode1);
            //            }
            //            else if (mp is object)
            //            {
            //                object argIndex3 = "超感覚";
            //                string argref_mode2 = "";
            //                slevel = mp.SkillLevel(argIndex3, ref_mode: argref_mode2);
            //                if (Data.PilotNum > 1)
            //                {
            //                    var loopTo4 = CountPilot();
            //                    for (i = 2; i <= loopTo4; i++)
            //                    {
            //                        object argIndex5 = i;
            //                        {
            //                            var withBlock1 = Pilot(argIndex5);
            //                            object argIndex4 = "超感覚";
            //                            string argref_mode3 = "";
            //                            slevel = GeneralLib.MaxDbl(slevel, withBlock1.SkillLevel(argIndex4, ref_mode: argref_mode3));
            //                            double localSkillLevel() { object argIndex1 = sname; string argref_mode = ""; var ret = withBlock1.SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

            //                            slevel = GeneralLib.MaxDbl(slevel, localSkillLevel());
            //                        }
            //                    }
            //                }

            //                var loopTo5 = CountSupport();
            //                for (i = 1; i <= loopTo5; i++)
            //                {
            //                    object argIndex7 = i;
            //                    {
            //                        var withBlock2 = Support(argIndex7);
            //                        object argIndex6 = "超感覚";
            //                        string argref_mode4 = "";
            //                        slevel = GeneralLib.MaxDbl(slevel, withBlock2.SkillLevel(argIndex6, ref_mode: argref_mode4));
            //                        double localSkillLevel1() { object argIndex1 = sname; string argref_mode = ""; var ret = withBlock2.SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

            //                        slevel = GeneralLib.MaxDbl(slevel, localSkillLevel1());
            //                    }
            //                }

            //                string argfname2 = "追加サポート";
            //                if (IsFeatureAvailable(argfname2))
            //                {
            //                    {
            //                        var withBlock3 = AdditionalSupport();
            //                        object argIndex8 = "超感覚";
            //                        string argref_mode5 = "";
            //                        slevel = GeneralLib.MaxDbl(slevel, withBlock3.SkillLevel(argIndex8, ref_mode: argref_mode5));
            //                        double localSkillLevel2() { object argIndex1 = sname; string argref_mode = ""; var ret = withBlock3.SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

            //                        slevel = GeneralLib.MaxDbl(slevel, localSkillLevel2());
            //                    }
            //                }

            //                if ((stype ?? "") != (sname ?? ""))
            //                {
            //                    if ((mp.SkillNameForNS(stype) ?? "") != (sname ?? ""))
            //                    {
            //                        slevel = 0d;
            //                    }
            //                }

            //                object argIndex9 = "知覚強化";
            //                string argref_mode6 = "";
            //                slevel = slevel + mp.SkillLevel(argIndex9, ref_mode: argref_mode6);
            //                if (Data.PilotNum > 1)
            //                {
            //                    var loopTo6 = CountPilot();
            //                    for (i = 2; i <= loopTo6; i++)
            //                    {
            //                        Pilot localPilot2() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

            //                        object argIndex10 = "知覚強化";
            //                        string argref_mode7 = "";
            //                        slevel = GeneralLib.MaxDbl(slevel, localPilot2().SkillLevel(argIndex10, ref_mode: argref_mode7));
            //                    }
            //                }

            //                var loopTo7 = CountSupport();
            //                for (i = 1; i <= loopTo7; i++)
            //                {
            //                    Pilot localSupport2() { object argIndex1 = i; var ret = Support(argIndex1); return ret; }

            //                    object argIndex11 = "知覚強化";
            //                    string argref_mode8 = "";
            //                    slevel = GeneralLib.MaxDbl(slevel, localSupport2().SkillLevel(argIndex11, ref_mode: argref_mode8));
            //                }

            //                string argfname3 = "追加サポート";
            //                if (IsFeatureAvailable(argfname3))
            //                {
            //                    object argIndex12 = "知覚強化";
            //                    string argref_mode9 = "";
            //                    slevel = GeneralLib.MaxDbl(slevel, AdditionalSupport().SkillLevel(argIndex12, ref_mode: argref_mode9));
            //                }
            //            }

            //            break;
            //        }

            //    case "同調率":
            //        {
            //            if (p is object)
            //            {
            //                slevel = p.SynchroRate();
            //            }
            //            else if (mp is object)
            //            {
            //                slevel = mp.SynchroRate();
            //                if (Data.PilotNum > 1)
            //                {
            //                    var loopTo8 = CountPilot();
            //                    for (i = 2; i <= loopTo8; i++)
            //                    {
            //                        Pilot localPilot3() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

            //                        slevel = GeneralLib.MaxDbl(slevel, localPilot3().SynchroRate());
            //                    }
            //                }

            //                var loopTo9 = CountSupport();
            //                for (i = 1; i <= loopTo9; i++)
            //                {
            //                    Pilot localSupport3() { object argIndex1 = i; var ret = Support(argIndex1); return ret; }

            //                    slevel = GeneralLib.MaxDbl(slevel, localSupport3().SynchroRate());
            //                }

            //                string argfname4 = "追加サポート";
            //                if (IsFeatureAvailable(argfname4))
            //                {
            //                    slevel = GeneralLib.MaxDbl(slevel, AdditionalSupport().SynchroRate());
            //                }
            //            }

            //            if ((stype ?? "") != (sname ?? ""))
            //            {
            //                if ((mp.SkillNameForNS(stype) ?? "") != (sname ?? ""))
            //                {
            //                    slevel = 0d;
            //                }
            //            }

            //            break;
            //        }

            //    case "オーラ":
            //        {
            //            if (p is object)
            //            {
            //                object argIndex13 = "オーラ";
            //                string argref_mode10 = "";
            //                slevel = p.SkillLevel(argIndex13, ref_mode: argref_mode10);
            //            }
            //            else if (mp is object)
            //            {
            //                slevel = AuraLevel();
            //            }

            //            if ((stype ?? "") != (sname ?? ""))
            //            {
            //                if ((mp.SkillNameForNS(stype) ?? "") != (sname ?? ""))
            //                {
            //                    slevel = 0d;
            //                }
            //            }

            //            break;
            //        }

            //    case "霊力":
            //        {
            //            if (p is object)
            //            {
            //                slevel = p.Plana;
            //            }
            //            else if (mp is object)
            //            {
            //                slevel = mp.Plana;
            //            }

            //            if ((stype ?? "") != (sname ?? ""))
            //            {
            //                if ((mp.SkillNameForNS(stype) ?? "") != (sname ?? ""))
            //                {
            //                    slevel = 0d;
            //                }
            //            }

            //            break;
            //        }

            //    default:
            //        {
            //            // 上記以外のパイロット用特殊能力

            //            if (mp is object)
            //            {
            //                // 特定パイロット専用？
            //                if ((sname ?? "") == (mp.Name ?? "") | (sname ?? "") == (mp.get_Nickname(false) ?? ""))
            //                {
            //                    slevel = 1d;
            //                }
            //                else if ((stype ?? "") == (sname ?? ""))
            //                {
            //                    object argIndex14 = stype;
            //                    string argref_mode11 = "";
            //                    slevel = mp.SkillLevel(argIndex14, ref_mode: argref_mode11);
            //                }
            //                else if ((mp.SkillNameForNS(stype) ?? "") == (sname ?? ""))
            //                {
            //                    object argIndex15 = stype;
            //                    string argref_mode12 = "";
            //                    slevel = mp.SkillLevel(argIndex15, ref_mode: argref_mode12);
            //                }

            //                // パイロット数が括弧つきでない場合のみ
            //                if (Data.PilotNum > 1)
            //                {
            //                    // サブパイロットの技能を検索
            //                    var loopTo10 = CountPilot();
            //                    for (i = 2; i <= loopTo10; i++)
            //                    {
            //                        object argIndex16 = i;
            //                        {
            //                            var withBlock4 = Pilot(argIndex16);
            //                            if ((sname ?? "") == (withBlock4.Name ?? "") | (sname ?? "") == (withBlock4.get_Nickname(false) ?? ""))
            //                            {
            //                                slevel = 1d;
            //                                break;
            //                            }

            //                            stype2 = withBlock4.SkillType(sname);
            //                            if ((stype2 ?? "") == (sname ?? ""))
            //                            {
            //                                double localSkillLevel3() { object argIndex1 = stype2; string argref_mode = ""; var ret = withBlock4.SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

            //                                slevel = GeneralLib.MaxDbl(slevel, localSkillLevel3());
            //                            }
            //                            else if ((withBlock4.SkillNameForNS(stype2) ?? "") == (sname ?? ""))
            //                            {
            //                                double localSkillLevel4() { object argIndex1 = stype2; string argref_mode = ""; var ret = withBlock4.SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

            //                                slevel = GeneralLib.MaxDbl(slevel, localSkillLevel4());
            //                            }
            //                        }
            //                    }
            //                }

            //                // サポートパイロットの技能を検索
            //                var loopTo11 = CountSupport();
            //                for (i = 1; i <= loopTo11; i++)
            //                {
            //                    object argIndex17 = i;
            //                    {
            //                        var withBlock5 = Support(argIndex17);
            //                        if ((sname ?? "") == (withBlock5.Name ?? "") | (sname ?? "") == (withBlock5.get_Nickname(false) ?? ""))
            //                        {
            //                            slevel = 1d;
            //                            break;
            //                        }

            //                        stype2 = withBlock5.SkillType(sname);
            //                        if ((stype2 ?? "") == (sname ?? ""))
            //                        {
            //                            double localSkillLevel5() { object argIndex1 = stype2; string argref_mode = ""; var ret = withBlock5.SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

            //                            slevel = GeneralLib.MaxDbl(slevel, localSkillLevel5());
            //                        }
            //                        else if ((withBlock5.SkillNameForNS(stype2) ?? "") == (sname ?? ""))
            //                        {
            //                            double localSkillLevel6() { object argIndex1 = stype2; string argref_mode = ""; var ret = withBlock5.SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

            //                            slevel = GeneralLib.MaxDbl(slevel, localSkillLevel6());
            //                        }
            //                    }
            //                }

            //                // 追加サポートの技能を検索
            //                string argfname5 = "追加サポート";
            //                if (IsFeatureAvailable(argfname5) & CountPilot() > 0)
            //                {
            //                    {
            //                        var withBlock6 = AdditionalSupport();
            //                        if ((sname ?? "") == (withBlock6.Name ?? "") | (sname ?? "") == (withBlock6.get_Nickname(false) ?? ""))
            //                        {
            //                            slevel = 1d;
            //                        }
            //                        else
            //                        {
            //                            stype2 = withBlock6.SkillType(sname);
            //                            if ((stype2 ?? "") == (sname ?? ""))
            //                            {
            //                                double localSkillLevel7() { object argIndex1 = stype2; string argref_mode = ""; var ret = withBlock6.SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

            //                                slevel = GeneralLib.MaxDbl(slevel, localSkillLevel7());
            //                            }
            //                            else if ((withBlock6.SkillNameForNS(stype2) ?? "") == (sname ?? ""))
            //                            {
            //                                double localSkillLevel8() { object argIndex1 = stype2; string argref_mode = ""; var ret = withBlock6.SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

            //                                slevel = GeneralLib.MaxDbl(slevel, localSkillLevel8());
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            if (slevel == 0d)
            //            {
            //                // ユニット名またはクラスに該当？
            //                if ((sname ?? "") == (Name ?? "") | (sname ?? "") == (Nickname0 ?? "") | (sname ?? "") == (Class0 ?? ""))
            //                {
            //                    slevel = 1d;
            //                }
            //            }

            //            if (slevel == 0d)
            //            {
            //                if (Strings.Left(sname, 1) == "@")
            //                {
            //                    // 地形を指定した必要技能
            //                    if (Status == "出撃" & 1 <= x & x <= Map.MapWidth & 1 <= y & y <= Map.MapHeight)
            //                    {
            //                        if ((Strings.Mid(sname, 2) ?? "") == (Map.TerrainName(x, y) ?? ""))
            //                        {
            //                            slevel = 1d;
            //                        }
            //                    }
            //                }
            //                else if (Strings.Right(sname, 2) == "装備")
            //                {
            //                    // アイテムを指定した必要技能
            //                    iname = Strings.Left(sname, Strings.Len(sname) - 2);
            //                    var loopTo12 = CountItem();
            //                    for (i = 1; i <= loopTo12; i++)
            //                    {
            //                        object argIndex18 = i;
            //                        {
            //                            var withBlock7 = Item(argIndex18);
            //                            if (withBlock7.Activated)
            //                            {
            //                                if ((iname ?? "") == (withBlock7.Name ?? "") | (iname ?? "") == (withBlock7.Class0() ?? ""))
            //                                {
            //                                    slevel = 1d;
            //                                    break;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //                else if (Strings.Right(sname, 2) == "隣接" | Strings.Right(sname, 4) == "マス以内")
            //                {
            //                    // 特定のユニットが近くにいることを指定した必要技能
            //                    if (Status == "出撃")
            //                    {
            //                        if (Strings.Right(sname, 2) == "隣接")
            //                        {
            //                            uname = Strings.Left(sname, Strings.Len(sname) - 2);
            //                            max_range = 1;
            //                        }
            //                        else
            //                        {
            //                            uname = Strings.Left(sname, Strings.Len(sname) - 5);
            //                            string argexpr1 = Strings.Mid(sname, Strings.Len(sname) - 4, 1);
            //                            max_range = (int)GeneralLib.StrToLng(argexpr1);
            //                        }

            //                        var loopTo13 = (int)GeneralLib.MinLng(x + max_range, Map.MapWidth);
            //                        for (i = (int)GeneralLib.MaxLng(x - max_range, 1); i <= loopTo13; i++)
            //                        {
            //                            var loopTo14 = (int)GeneralLib.MinLng(y + max_range, Map.MapHeight);
            //                            for (j = (int)GeneralLib.MaxLng(y - max_range, 1); j <= loopTo14; j++)
            //                            {
            //                                u = Map.MapDataForUnit[i, j];

            //                                // 距離が範囲外？
            //                                if ((int)(Math.Abs((int)(x - i)) + Math.Abs((int)(y - j))) > max_range)
            //                                {
            //                                    goto NextNeighbor;
            //                                }

            //                                // ユニットがいない？
            //                                if (u is null)
            //                                {
            //                                    goto NextNeighbor;
            //                                }

            //                                // ユニットが自分？
            //                                if (ReferenceEquals(u, this) | x == i & y == j)
            //                                {
            //                                    goto NextNeighbor;
            //                                }

            //                                // ユニットが敵？
            //                                if (IsEnemy(u))
            //                                {
            //                                    goto NextNeighbor;
            //                                }
            //                                // 合体技のパートナーに該当するか
            //                                if (uname == "母艦")
            //                                {
            //                                    string argfname6 = "母艦";
            //                                    if (!u.IsFeatureAvailable(argfname6))
            //                                    {
            //                                        goto NextNeighbor;
            //                                    }
            //                                }
            //                                else if ((u.Name ?? "") != (uname ?? "") & (u.MainPilot().Name ?? "") != (uname ?? ""))
            //                                {
            //                                    goto NextNeighbor;
            //                                }

            //                                // 行動出来なければだめ
            //                                object argIndex19 = "混乱";
            //                                object argIndex20 = "恐怖";
            //                                object argIndex21 = "憑依";
            //                                if (u.MaxAction() == 0 | u.IsConditionSatisfied(argIndex19) | u.IsConditionSatisfied(argIndex20) | u.IsConditionSatisfied(argIndex21))
            //                                {
            //                                    goto NextNeighbor;
            //                                }

            //                                // パートナーが見つかった
            //                                IsNecessarySkillSatisfied2Ret = true;
            //                                return IsNecessarySkillSatisfied2Ret;
            //                            NextNeighbor:
            //                                ;
            //                            }
            //                        }
            //                    }
            //                }
            //                else if (Strings.Right(sname, 2) == "状態")
            //                {
            //                    // 特殊状態を指定した必要技能
            //                    object argIndex22 = Strings.Left(sname, Strings.Len(sname) - 2);
            //                    if (IsConditionSatisfied(argIndex22))
            //                    {
            //                        slevel = 1d;
            //                    }
            //                }
            //            }

            //            break;
            //        }
            //}

            //// 指定された技能のレベルが必要なレベル以上の場合に必要技能が満たされたと判定
            //if (slevel >= nlevel)
            //{
            //    IsNecessarySkillSatisfied2Ret = true;
            //}

            //return IsNecessarySkillSatisfied2Ret;
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
    }
}
