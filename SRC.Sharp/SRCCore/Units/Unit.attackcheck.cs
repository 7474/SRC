// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Lib;
using SRCCore.VB;

namespace SRCCore.Units
{
    // === 攻撃関連処理 ===
    public partial class Unit
    {
        // 回避用特殊能力の判定
        public bool CheckDodgeFeature(UnitWeapon w, Unit t, int tx, int ty, string attack_mode, string def_mode, int dmg, bool be_quiet)
        {
            // TODO Impl
            return false;
            //    bool CheckDodgeFeatureRet = default;
            //    string wname;
            //    int ecost, nmorale;
            //    string fname, fdata;
            //    double flevel;
            //    int fid, frange;
            //    Unit u;
            //    int j, i, k;
            //    int prob;
            //    string buf;
            //    string team, uteam;

            //    // スペシャルパワーで回避能力が無効化されている？
            //    if ((IsUnderSpecialPowerEffect("絶対命中") || IsUnderSpecialPowerEffect("回避能力無効化")) && !t.IsUnderSpecialPowerEffect("特殊防御発動"))
            //    {
            //        return CheckDodgeFeatureRet;
            //    }

            //    // 能動防御は行動できなければ発動しない
            //    if (t.MaxAction() == 0 || t.IsUnderSpecialPowerEffect("無防備"))
            //    {
            //        return CheckDodgeFeatureRet;
            //    }

            //    wname = WeaponNickname(w);
            //    team = MainPilot().SkillData("チーム");

            //    // 阻止無効化
            //    if (w.IsWeaponClassifiedAs("無") || IsUnderSpecialPowerEffect("防御能力無効化"))
            //    {
            //        goto SkipBlock;
            //    }

            //    // 広域阻止
            //    // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    u = null;
            //    flevel = 0d;
            //    fid = 0;
            //    // 阻止してくれるユニットを探す
            //    var loopTo = GeneralLib.MinLng(tx + 3, Map.MapWidth);
            //    for (i = GeneralLib.MaxLng(tx - 3, 1); i <= loopTo; i++)
            //    {
            //        var loopTo1 = GeneralLib.MinLng(ty + 3, Map.MapHeight);
            //        for (j = GeneralLib.MaxLng(ty - 3, 1); j <= loopTo1; j++)
            //        {
            //            if (Map.MapDataForUnit[i, j] is null || Math.Abs((tx - i)) + Math.Abs((ty - j)) > 3)
            //            {
            //                goto NextPoint;
            //            }

            //            {
            //                var withBlock = Map.MapDataForUnit[i, j];
            //                if (withBlock.IsEnemy(t))
            //                {
            //                    goto NextPoint;
            //                }

            //                if (withBlock.Area == "地中")
            //                {
            //                    goto NextPoint;
            //                }

            //                if (!withBlock.IsFeatureAvailable("広域阻止"))
            //                {
            //                    goto NextPoint;
            //                }

            //                // 同じチームに属している？
            //                uteam = withBlock.MainPilot().SkillData("チーム");
            //                if ((team ?? "") != (uteam ?? "") && !string.IsNullOrEmpty(uteam))
            //                {
            //                    goto NextPoint;
            //                }

            //                var loopTo2 = withBlock.CountFeature();
            //                for (k = 1; k <= loopTo2; k++)
            //                {
            //                    if (withBlock.Feature(k) == "広域阻止")
            //                    {
            //                        fdata = withBlock.FeatureData(k);

            //                        // 有効範囲
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
            //                        {
            //                            frange = Conversions.Toint(GeneralLib.LIndex(fdata, 2));
            //                        }
            //                        else
            //                        {
            //                            frange = 1;
            //                        }

            //                        // 使用条件
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
            //                        {
            //                            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 5));
            //                        }
            //                        else
            //                        {
            //                            ecost = 0;
            //                        }

            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 6)))
            //                        {
            //                            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 6));
            //                        }
            //                        else
            //                        {
            //                            nmorale = 0;
            //                        }

            //                        // 発動条件を満たしている？
            //                        bool localIsAttributeClassified() { string argaclass1 = GeneralLib.LIndex(fdata, 3); string argaclass2 = w.WeaponClass(); var ret = withBlock.IsAttributeClassified(argaclass1, argaclass2); return ret; }

            //                        if (withBlock.EN >= ecost && withBlock.MainPilot().Morale >= nmorale && localIsAttributeClassified() && (Math.Abs((tx - i)) + Math.Abs((ty - j))) <= frange && (Math.Abs((x - i)) + Math.Abs((y - j))) > frange && (!ReferenceEquals(Map.MapDataForUnit[i, j], t) || !t.IsFeatureAvailable("阻止")))
            //                        {
            //                            if (withBlock.FeatureLevel(k) > flevel)
            //                            {
            //                                u = Map.MapDataForUnit[i, j];
            //                                flevel = withBlock.FeatureLevel(k);
            //                                fid = k;
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //        NextPoint:
            //            ;
            //        }
            //    }

            //    if (u is object)
            //    {
            //        // 阻止してくれるユニットがいる場合
            //        if (fid == 0)
            //        {
            //            fname = u.FeatureName("広域阻止");
            //            fdata = u.FeatureData("広域阻止");
            //            flevel = u.FeatureLevel("広域阻止");
            //        }
            //        else
            //        {
            //            fname = u.FeatureName(fid);
            //            fdata = u.FeatureData(fid);
            //            flevel = u.FeatureLevel(fid);
            //        }

            //        if (flevel == 1d)
            //        {
            //            flevel = 10000d;
            //        }

            //        // 阻止確率の設定
            //        buf = GeneralLib.LIndex(fdata, 4);
            //        if (Information.IsNumeric(buf))
            //        {
            //            prob = Conversions.Toint(buf);
            //        }
            //        else if (Strings.InStr(buf, "+") > 0 || Strings.InStr(buf, "-") > 0)
            //        {
            //            i = GeneralLib.MaxLng(Strings.InStr(buf, "+"), Strings.InStr(buf, "-"));
            //            prob = (100d * (u.SkillLevel(Strings.Left(buf, i - 1)) + Conversions.Toint(Strings.Mid(buf, i))) / 16d);
            //        }
            //        else
            //        {
            //            prob = (u.SkillLevel(buf) * 100d / 16d);
            //        }

            //        // 見切り
            //        if (u.IsUnderSpecialPowerEffect("特殊防御発動"))
            //        {
            //            prob = 100;
            //        }

            //        // 必中がかかっていれば阻止は無効
            //        if (IsUnderSpecialPowerEffect("絶対命中") && !u.IsUnderSpecialPowerEffect("特殊防御発動"))
            //        {
            //            prob = 0;
            //        }

            //        // ダメージが許容範囲外であれば阻止できない
            //        if (dmg > 500d * flevel)
            //        {
            //            prob = 0;
            //        }

            //        // ＥＮ消費量
            //        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
            //        {
            //            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 5));
            //        }
            //        else
            //        {
            //            ecost = 0;
            //        }

            //        // 攻撃を阻止
            //        if (prob >= GeneralLib.Dice(100))
            //        {
            //            u.EN = u.EN - ecost;
            //            if (!be_quiet)
            //            {
            //                if (u.IsMessageDefined("阻止(" + fname + ")"))
            //                {
            //                    u.PilotMessage("阻止(" + fname + ")", msg_mode: "");
            //                }
            //                else
            //                {
            //                    u.PilotMessage("阻止", msg_mode: "");
            //                }
            //            }

            //            if (u.IsAnimationDefined("阻止", fname))
            //            {
            //                u.PlayAnimation("阻止", fname);
            //            }
            //            else
            //            {
            //                u.SpecialEffect("阻止", fname);
            //            }

            //            if (u.IsSysMessageDefined("阻止", fname))
            //            {
            //                u.SysMessage("阻止", fname, add_msg: "");
            //            }
            //            else
            //            {
            //                GUI.DisplaySysMessage(u.Nickname + "は[" + fname + "]で[" + wname + "]を防いだ。");
            //            }

            //            CheckDodgeFeatureRet = true;
            //            return CheckDodgeFeatureRet;
            //        }
            //    }

            //SkipBlock:
            //    ;


            //    // 分身(ユニット用特殊能力)
            //    if (t.IsFeatureAvailable("分身") && t.MainPilot().Morale >= 130 && !t.IsFeatureLevelSpecified("分身") && (GeneralLib.Dice(2) == 1 || t.IsUnderSpecialPowerEffect("特殊防御発動")))
            //    {
            //        fname = t.FeatureName("分身");

            //        // 特殊効果
            //        if (t.IsAnimationDefined("分身", fname))
            //        {
            //            t.PlayAnimation("分身", fname);
            //        }
            //        else if (t.IsSpecialEffectDefined("分身", fname))
            //        {
            //            t.SpecialEffect("分身", fname);
            //        }
            //        else if (SRC.BattleAnimation)
            //        {
            //            if (fname == "分身")
            //            {
            //                Effect.ShowAnimation("分身発動");
            //            }
            //            else
            //            {
            //                Effect.ShowAnimation("分身発動 - " + fname);
            //            }
            //        }

            //        // 回避音
            //        Effect.DodgeEffect(this, w);

            //        // メッセージ
            //        if (!be_quiet)
            //        {
            //            if (t.IsMessageDefined("分身(" + fname + ")"))
            //            {
            //                t.PilotMessage("分身(" + fname + ")", msg_mode: "");
            //            }
            //            else
            //            {
            //                t.PilotMessage("分身", msg_mode: "");
            //            }
            //        }

            //        if (t.IsSysMessageDefined("分身", fname))
            //        {
            //            t.SysMessage("分身", fname, add_msg: "");
            //        }
            //        else if (fname != "分身")
            //        {
            //            GUI.DisplaySysMessage(t.Nickname + "は[" + fname + "]を使って攻撃をかわした");
            //        }
            //        else
            //        {
            //            GUI.DisplaySysMessage(t.Nickname + "は分身して攻撃をかわした。");
            //        }

            //        CheckDodgeFeatureRet = true;
            //        return CheckDodgeFeatureRet;
            //    }

            //    // 超回避
            //    if (t.IsFeatureAvailable("超回避"))
            //    {
            //        fname = t.FeatureName("超回避");
            //        fdata = t.FeatureData("超回避");
            //        flevel = t.FeatureLevel("超回避");

            //        // 発動率
            //        prob = flevel;
            //        if (t.IsUnderSpecialPowerEffect("特殊防御発動"))
            //        {
            //            prob = 10;
            //        }

            //        // 必要条件
            //        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
            //        {
            //            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 2));
            //        }
            //        else
            //        {
            //            ecost = 0;
            //        }

            //        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
            //        {
            //            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
            //        }
            //        else
            //        {
            //            nmorale = 0;
            //        }

            //        if (GeneralLib.LIndex(fdata, 4) == "手動")
            //        {
            //            if (def_mode != "回避")
            //            {
            //                prob = 0;
            //            }
            //        }

            //        // 発動条件を満たしている？
            //        if (t.EN >= ecost && t.MainPilot().Morale >= nmorale && prob >= GeneralLib.Dice(10))
            //        {
            //            // ＥＮ消費
            //            if (ecost != 0)
            //            {
            //                t.EN = t.EN - ecost;
            //                if (attack_mode != "反射")
            //                {
            //                    GUI.UpdateMessageForm(this, t);
            //                }
            //                else
            //                {
            //                    GUI.UpdateMessageForm(this, null);
            //                }
            //            }

            //            // 特殊効果
            //            if (t.IsAnimationDefined("分身", fname))
            //            {
            //                t.PlayAnimation("分身", fname);
            //            }
            //            else if (t.IsSpecialEffectDefined("分身", fname))
            //            {
            //                t.SpecialEffect("分身", fname);
            //            }
            //            else if (SRC.BattleAnimation)
            //            {
            //                Effect.ShowAnimation("回避発動");
            //            }
            //            else
            //            {
            //                // 回避音
            //                Effect.DodgeEffect(this, w);
            //            }

            //            // メッセージ
            //            if (!be_quiet)
            //            {
            //                if (t.IsMessageDefined("分身(" + fname + ")"))
            //                {
            //                    t.PilotMessage("分身(" + fname + ")", msg_mode: "");
            //                }
            //                else
            //                {
            //                    t.PilotMessage("分身", msg_mode: "");
            //                }
            //            }

            //            if (t.IsSysMessageDefined("分身", fname))
            //            {
            //                t.SysMessage("分身", fname, add_msg: "");
            //            }
            //            else
            //            {
            //                GUI.DisplaySysMessage(t.Nickname + "は[" + fname + "]を使って攻撃をかわした。");
            //            }

            //            CheckDodgeFeatureRet = true;
            //            return CheckDodgeFeatureRet;
            //        }
            //    }

            //    // 緊急テレポート
            //    int new_x = default, new_y = default;
            //    if (t.IsFeatureAvailable("緊急テレポート"))
            //    {
            //        fname = t.FeatureName("緊急テレポート");
            //        fdata = t.FeatureData("緊急テレポート");
            //        flevel = t.FeatureLevel("緊急テレポート");

            //        // 発動率
            //        prob = flevel;
            //        if (t.IsUnderSpecialPowerEffect("特殊防御発動"))
            //        {
            //            prob = 10;
            //        }

            //        // 必要条件
            //        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
            //        {
            //            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
            //        }
            //        else
            //        {
            //            ecost = 0;
            //        }

            //        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //        {
            //            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //        }
            //        else
            //        {
            //            nmorale = 0;
            //        }

            //        if (GeneralLib.LIndex(fdata, 5) == "手動")
            //        {
            //            if (def_mode != "回避")
            //            {
            //                prob = 0;
            //            }
            //        }

            //        // 発動条件を満たしている？
            //        if (t.EN >= ecost && t.MainPilot().Morale >= nmorale && prob >= GeneralLib.Dice(10))
            //        {

            //            // 逃げ場所がある？
            //            int localStrToLng() { string argexpr = GeneralLib.LIndex(fdata, 2); var ret = GeneralLib.StrToLng(argexpr); return ret; }

            //            Map.AreaInTeleport(t, localStrToLng());
            //            Map.SafetyPoint(t, new_x, new_y);
            //            if ((t.x != new_x || t.y != new_y) && new_x != 0 && new_y != 0)
            //            {
            //                // ＥＮ消費
            //                if (ecost != 0)
            //                {
            //                    t.EN = t.EN - ecost;
            //                    if (attack_mode != "反射")
            //                    {
            //                        GUI.UpdateMessageForm(this, t);
            //                    }
            //                    else
            //                    {
            //                        GUI.UpdateMessageForm(this, null);
            //                    }
            //                }

            //                // 特殊効果
            //                if (t.IsAnimationDefined("緊急テレポート", fname))
            //                {
            //                    t.PlayAnimation("緊急テレポート", fname);
            //                }
            //                else if (t.IsSpecialEffectDefined("緊急テレポート", fname))
            //                {
            //                    t.SpecialEffect("緊急テレポート", fname);
            //                }
            //                else if (SRC.BattleAnimation)
            //                {
            //                    if (fname == "緊急テレポート")
            //                    {
            //                        Effect.ShowAnimation("緊急テレポート発動");
            //                    }
            //                    else
            //                    {
            //                        Effect.ShowAnimation("緊急テレポート発動 - " + fname);
            //                    }
            //                }

            //                // 回避音
            //                Effect.DodgeEffect(this, w);

            //                // 緊急テレポート発動！
            //                t.Jump(new_x, new_y);

            //                // メッセージ
            //                if (!be_quiet)
            //                {
            //                    if (t.IsMessageDefined("緊急テレポート(" + fname + ")"))
            //                    {
            //                        t.PilotMessage("緊急テレポート(" + fname + ")", msg_mode: "");
            //                    }
            //                    else
            //                    {
            //                        t.PilotMessage("緊急テレポート", msg_mode: "");
            //                    }
            //                }

            //                if (t.IsSysMessageDefined("緊急テレポート", fname))
            //                {
            //                    t.SysMessage("緊急テレポート", fname, add_msg: "");
            //                }
            //                else
            //                {
            //                    GUI.DisplaySysMessage(t.Nickname + "は[" + fname + "]を使って攻撃をかわした。");
            //                }

            //                CheckDodgeFeatureRet = true;
            //                return CheckDodgeFeatureRet;
            //            }
            //        }
            //    }

            //    // 分身(パイロット用特殊能力)
            //    if (t.MainPilot().IsSkillAvailable("分身"))
            //    {
            //        prob = (2d * t.MainPilot().SkillLevel("分身", ref_mode: "") - MainPilot().SkillLevel("分身", ref_mode: ""));
            //        if (t.IsUnderSpecialPowerEffect("特殊防御発動"))
            //        {
            //            prob = 32;
            //        }

            //        if (prob >= GeneralLib.Dice(32))
            //        {
            //            fname = t.MainPilot().SkillName0("分身");

            //            // 特殊効果
            //            if (t.IsAnimationDefined("分身", fname))
            //            {
            //                t.PlayAnimation("分身", fname);
            //            }
            //            else if (t.IsSpecialEffectDefined("分身", fname))
            //            {
            //                t.SpecialEffect("分身", fname);
            //            }
            //            else if (SRC.BattleAnimation)
            //            {
            //                Effect.ShowAnimation("分身発動");
            //            }
            //            else
            //            {
            //                // 回避音
            //                Effect.DodgeEffect(this, w);
            //            }

            //            // メッセージ
            //            if (!be_quiet)
            //            {
            //                if (t.IsMessageDefined("分身(" + fname + ")"))
            //                {
            //                    t.PilotMessage("分身(" + fname + ")", msg_mode: "");
            //                }
            //                else
            //                {
            //                    t.PilotMessage("分身", msg_mode: "");
            //                }
            //            }

            //            if (t.IsSysMessageDefined("分身", fname))
            //            {
            //                t.SysMessage("分身", fname, add_msg: "");
            //            }
            //            else
            //            {
            //                GUI.DisplaySysMessage(t.Nickname + "は分身して攻撃をかわした。");
            //            }

            //            CheckDodgeFeatureRet = true;
            //            return CheckDodgeFeatureRet;
            //        }
            //    }

            //    return CheckDodgeFeatureRet;
        }

        // 切り払い＆反射のチェック
        // (命中時に発動し、発動すれば必ずダメージが0になる能力)
        public bool CheckParryFeature(UnitWeapon w, Unit t, int tx, int ty, string attack_mode, string def_mode, int dmg, string msg, bool be_quiet)
        {
            // TODO Impl
            return false;
            //    bool CheckParryFeatureRet = default;
            //    string wname, wname2;
            //    int w2;
            //    int ecost, nmorale;
            //    string fname = default, fdata;
            //    double flevel;
            //    double slevel, lv_mod;
            //    string opt;
            //    int j, i, idx;
            //    int prob;
            //    string buf;

            //    // スペシャルパワーで回避能力が無効化されている？
            //    if ((IsUnderSpecialPowerEffect("絶対命中") || IsUnderSpecialPowerEffect("回避能力無効化")) && !t.IsUnderSpecialPowerEffect("特殊防御発動"))
            //    {
            //        return CheckParryFeatureRet;
            //    }

            //    // 能動防御は行動できなければ発動しない
            //    if (t.MaxAction() == 0 || t.IsUnderSpecialPowerEffect("無防備"))
            //    {
            //        return CheckParryFeatureRet;
            //    }

            //    wname = WeaponNickname(w);

            //    // ターゲットの迎撃レベルをチェック
            //    slevel = t.SkillLevel("迎撃");

            //    // 切り払いに使用する武器を持っている？
            //    // (持っていれば切り払いの方を優先)
            //    wname2 = "";
            //    if (t.IsFeatureAvailable("格闘武器"))
            //    {
            //        wname2 = t.FeatureData("格闘武器");
            //    }
            //    else
            //    {
            //        var loopTo = t.CountWeapon();
            //        for (i = 1; i <= loopTo; i++)
            //        {
            //            if (t.IsWeaponClassifiedAs(i, "武") && t.IsWeaponAvailable(i, "移動前"))
            //            {
            //                wname2 = t.WeaponNickname(i);
            //                break;
            //            }
            //        }
            //    }
            //    // 発動条件を満たしている？
            //    if (w.IsWeaponClassifiedAs("実") && (slevel > t.MainPilot().SkillLevel("切り払い", ref_mode: "") || slevel > 0d && Strings.Len(wname2) == 0))
            //    {
            //        // 迎撃武器を検索
            //        i = 0;
            //        if (t.IsFeatureAvailable("迎撃武器"))
            //        {
            //            var loopTo1 = t.CountWeapon();
            //            for (i = 1; i <= loopTo1; i++)
            //            {
            //                if ((t.Weapon(i).Name ?? "") == (t.FeatureData("迎撃武器") ?? ""))
            //                {
            //                    if (!t.IsWeaponAvailable(i, "移動前"))
            //                    {
            //                        i = 0;
            //                    }

            //                    break;
            //                }
            //            }
            //        }

            //        if (i == 0)
            //        {
            //            // 迎撃武器がない場合は迎撃用の武器としての条件を満たす武器を検索
            //            var loopTo2 = t.CountWeapon();
            //            for (i = 1; i <= loopTo2; i++)
            //            {
            //                if (t.IsWeaponAvailable(i, "移動後") && t.IsWeaponClassifiedAs(i, "移動後攻撃可") && t.IsWeaponClassifiedAs(i, "射撃系") && (t.Weapon(i).Bullet >= 10 || t.Weapon(i).Bullet == 0 && t.Weapon(i).ENConsumption <= 5) && t.MainPilot().Morale >= t.Weapon(i).NecessaryMorale)
            //                {
            //                    break;
            //                }
            //            }
            //        }

            //        // 迎撃用武器が弾切れ、ＥＮ不足の場合は迎撃不可
            //        if (0 < i && i <= t.CountWeapon())
            //        {
            //            if (!t.IsWeaponAvailable(i, "ステータス"))
            //            {
            //                i = 0;
            //            }
            //        }

            //        // 迎撃を実行
            //        if (0 < i && i <= t.CountWeapon() && (slevel >= GeneralLib.Dice(16) || t.IsUnderSpecialPowerEffect("特殊防御発動")))
            //        {
            //            // メッセージ
            //            if (!be_quiet)
            //            {
            //                if (t.IsMessageDefined("迎撃(" + t.Weapon(i).Name + ")"))
            //                {
            //                    t.PilotMessage("迎撃(" + t.Weapon(i).Name + ")", msg_mode: "");
            //                }
            //                else
            //                {
            //                    t.PilotMessage("迎撃", msg_mode: "");
            //                }
            //            }
            //            else
            //            {
            //                Sound.IsWavePlayed = false;
            //            }

            //            // 効果音
            //            if (!Sound.IsWavePlayed)
            //            {
            //                bool localIsSpecialEffectDefined() { string argmain_situation = wname + "(迎撃)"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //                if (IsAnimationDefined(wname + "(迎撃)", sub_situation: ""))
            //                {
            //                    PlayAnimation(wname + "(迎撃)", sub_situation: "");
            //                }
            //                else if (localIsSpecialEffectDefined())
            //                {
            //                    SpecialEffect(wname + "(迎撃)", sub_situation: "");
            //                }
            //                else if (t.IsAnimationDefined("迎撃", fname))
            //                {
            //                    t.PlayAnimation("迎撃", fname);
            //                }
            //                else if (t.IsSpecialEffectDefined("迎撃", fname))
            //                {
            //                    t.SpecialEffect("迎撃", fname);
            //                }
            //                else if (t.IsSpecialEffectDefined(t.Weapon(i).Name, sub_situation: ""))
            //                {
            //                    t.SpecialEffect(t.Weapon(i).Name, sub_situation: "");
            //                }
            //                else
            //                {
            //                    Effect.AttackEffect(t, i);
            //                }
            //            }

            //            GUI.DisplaySysMessage(t.Nickname + "は[" + t.WeaponNickname(i) + "]で[" + wname + "]を阻止した。");

            //            // 迎撃された永続武器は使用回数を減らす
            //            if (w.IsWeaponClassifiedAs("永") && this.Weapon(w).Bullet > 0)
            //            {
            //                SetBullet(w, (Bullet(w) - 1));
            //                SyncBullet();
            //                IsMapAttackCanceled = true;
            //            }

            //            // 迎撃武器の弾数を消費
            //            t.UseWeapon(i);
            //            CheckParryFeatureRet = true;
            //            return CheckParryFeatureRet;
            //        }
            //    }

            //    // 無属性武器には阻止が効かない
            //    if (w.IsWeaponClassifiedAs("無") || IsUnderSpecialPowerEffect("防御能力無効化"))
            //    {
            //        goto SkipBlock;
            //    }

            //    // 阻止
            //    var loopTo3 = t.CountFeature();
            //    for (i = 1; i <= loopTo3; i++)
            //    {
            //        if (t.Feature(i) == "阻止")
            //        {
            //            fname = t.FeatureName0(i);
            //            fdata = t.FeatureData(i);
            //            flevel = t.FeatureLevel(i);
            //            if (flevel == 1d)
            //            {
            //                flevel = 10000d;
            //            }

            //            // 阻止確率の設定
            //            buf = GeneralLib.LIndex(fdata, 3);
            //            if (Information.IsNumeric(buf))
            //            {
            //                prob = Conversions.Toint(buf);
            //            }
            //            else if (Strings.InStr(buf, "+") > 0 || Strings.InStr(buf, "-") > 0)
            //            {
            //                j = GeneralLib.MaxLng(Strings.InStr(buf, "+"), Strings.InStr(buf, "-"));
            //                prob = (100d * (t.SkillLevel(Strings.Left(buf, j - 1)) + Conversions.Toint(Strings.Mid(buf, j))) / 16d);
            //            }
            //            else
            //            {
            //                prob = (100d * t.SkillLevel(buf) / 16d);
            //            }

            //            // 見切り
            //            if (t.IsUnderSpecialPowerEffect("特殊防御発動"))
            //            {
            //                prob = 100;
            //            }

            //            // 必中がかかっていれば阻止は無効
            //            if (IsUnderSpecialPowerEffect("絶対命中") && !t.IsUnderSpecialPowerEffect("特殊防御発動"))
            //            {
            //                prob = 0;
            //            }

            //            // 対象属性の判定
            //            bool localIsAttributeClassified() { string argaclass1 = GeneralLib.LIndex(fdata, 2); string argaclass2 = w.WeaponClass(); var ret = t.IsAttributeClassified(argaclass1, argaclass2); return ret; }

            //            if (!localIsAttributeClassified())
            //            {
            //                prob = 0;
            //            }

            //            // 使用条件
            //            if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //            {
            //                ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //            }
            //            else
            //            {
            //                ecost = 0;
            //            }

            //            if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
            //            {
            //                nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 5));
            //            }
            //            else
            //            {
            //                nmorale = 0;
            //            }

            //            if (t.EN < ecost || t.MainPilot().Morale < nmorale)
            //            {
            //                prob = 0;
            //            }

            //            // オプション
            //            slevel = 0d;
            //            var loopTo4 = GeneralLib.LLength(fdata);
            //            for (j = 6; j <= loopTo4; j++)
            //            {
            //                if (prob == 0)
            //                {
            //                    break;
            //                }

            //                opt = GeneralLib.LIndex(fdata, j);
            //                idx = Strings.InStr(opt, "*");
            //                if (idx > 0)
            //                {
            //                    lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
            //                    opt = Strings.Left(opt, idx - 1);
            //                }
            //                else
            //                {
            //                    lv_mod = -1;
            //                }

            //                switch (t.MainPilot().SkillType(opt) ?? "")
            //                {
            //                    case "相殺":
            //                        {
            //                            string localLIndex() { object argIndex1 = "阻止"; string arglist = FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //                            if ((GeneralLib.LIndex(fdata, 1) ?? "") == (localLIndex() ?? "") && Math.Abs((x - tx)) + Math.Abs((y - ty)) == 1)
            //                            {
            //                                msg = msg + Nickname + "は[" + fname + "]を中和した。;";
            //                                prob = 0;
            //                            }

            //                            break;
            //                        }

            //                    case "中和":
            //                        {
            //                            string localLIndex1() { object argIndex1 = "阻止"; string arglist = FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //                            if ((GeneralLib.LIndex(fdata, 1) ?? "") == (localLIndex1() ?? "") && Math.Abs((x - tx)) + Math.Abs((y - ty)) == 1)
            //                            {
            //                                flevel = flevel - FeatureLevel("阻止");
            //                                if (flevel <= 0d)
            //                                {
            //                                    msg = msg + Nickname + "は[" + fname + "]を中和した。;";
            //                                    prob = 0;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "近接無効":
            //                        {
            //                            if (w.IsWeaponClassifiedAs("武") || w.IsWeaponClassifiedAs("突") || w.IsWeaponClassifiedAs("接"))
            //                            {
            //                                prob = 0;
            //                            }

            //                            break;
            //                        }

            //                    case "手動":
            //                        {
            //                            if (def_mode != "防御")
            //                            {
            //                                prob = 0;
            //                            }

            //                            break;
            //                        }

            //                    case "能力必要":
            //                        {
            //                            break;
            //                        }
            //                    // スキップ
            //                    case "同調率":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 20d;
            //                            }

            //                            slevel = lv_mod * (t.SyncLevel() - 30d);
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == -30 * lv_mod)
            //                                {
            //                                    prob = 0;
            //                                }
            //                            }
            //                            else if (slevel == -30 * lv_mod)
            //                            {
            //                                slevel = 0d;
            //                            }

            //                            break;
            //                        }

            //                    case "霊力":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 10d;
            //                            }

            //                            slevel = lv_mod * t.PlanaLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    prob = 0;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "オーラ":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 200d;
            //                            }

            //                            slevel = lv_mod * t.AuraLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    prob = 0;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "超能力":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 200d;
            //                            }

            //                            slevel = lv_mod * t.PsychicLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    prob = 0;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 200d;
            //                            }

            //                            slevel = lv_mod * t.SkillLevel(opt);
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    prob = 0;
            //                                }
            //                            }

            //                            break;
            //                        }
            //                }
            //            }

            //            // ダメージが許容範囲外であれば阻止できない
            //            if (dmg > 500d * flevel + slevel)
            //            {
            //                prob = 0;
            //            }

            //            // 攻撃を阻止
            //            if (prob >= GeneralLib.Dice(100))
            //            {
            //                if (ecost != 0)
            //                {
            //                    t.EN = t.EN - ecost;
            //                    if (attack_mode != "反射")
            //                    {
            //                        GUI.UpdateMessageForm(this, t);
            //                    }
            //                    else
            //                    {
            //                        GUI.UpdateMessageForm(this, null);
            //                    }
            //                }

            //                if (!be_quiet)
            //                {
            //                    if (t.IsMessageDefined("阻止(" + fname + ")"))
            //                    {
            //                        t.PilotMessage("阻止(" + fname + ")", msg_mode: "");
            //                    }
            //                    else
            //                    {
            //                        t.PilotMessage("阻止", msg_mode: "");
            //                    }
            //                }

            //                bool localIsSpecialEffectDefined1() { string argmain_situation = wname + "(阻止)"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //                if (IsAnimationDefined(wname + "(阻止)", sub_situation: ""))
            //                {
            //                    PlayAnimation(wname + "(阻止)", sub_situation: "");
            //                }
            //                else if (localIsSpecialEffectDefined1())
            //                {
            //                    SpecialEffect(wname + "(阻止)", sub_situation: "");
            //                }
            //                else if (t.IsAnimationDefined("阻止", fname))
            //                {
            //                    t.PlayAnimation("阻止", fname);
            //                }
            //                else
            //                {
            //                    t.SpecialEffect("阻止", fname);
            //                }

            //                if (t.IsSysMessageDefined("阻止", fname))
            //                {
            //                    t.SysMessage("阻止", fname, add_msg: "");
            //                }
            //                else
            //                {
            //                    GUI.DisplaySysMessage(t.Nickname + "は[" + fname + "]で[" + wname + "]を防いだ。");
            //                }

            //                CheckParryFeatureRet = true;
            //                return CheckParryFeatureRet;
            //            }
            //        }
            //    }

            //SkipBlock:
            //    ;


            //    // マップ攻撃や無属性武器には当て身技は効かない
            //    if (w.IsWeaponClassifiedAs("Ｍ") || w.IsWeaponClassifiedAs("無") || IsUnderSpecialPowerEffect("防御能力無効化"))
            //    {
            //        goto SkipParryAttack;
            //    }

            //    // 当て身技
            //    var loopTo5 = t.CountFeature();
            //    for (i = 1; i <= loopTo5; i++)
            //    {
            //        // 封印されている？
            //        if (t.Feature(i) == "当て身技")
            //        {
            //            fname = t.FeatureName0(i);
            //            if (string.IsNullOrEmpty(fname))
            //            {
            //                fname = "当て身技";
            //            }

            //            fdata = t.FeatureData(i);
            //            flevel = t.FeatureLevel(i);
            //            if (flevel == 1d)
            //            {
            //                flevel = 10000d;
            //            }

            //            // 当て身確率の設定
            //            buf = GeneralLib.LIndex(fdata, 4);
            //            if (Information.IsNumeric(buf))
            //            {
            //                prob = Conversions.Toint(buf);
            //            }
            //            else if (Strings.InStr(buf, "+") > 0 || Strings.InStr(buf, "-") > 0)
            //            {
            //                j = GeneralLib.MaxLng(Strings.InStr(buf, "+"), Strings.InStr(buf, "-"));
            //                prob = (100d * (t.SkillLevel(Strings.Left(buf, j - 1)) + Conversions.Toint(Strings.Mid(buf, j))) / 16d);
            //            }
            //            else
            //            {
            //                prob = (100d * t.SkillLevel(buf) / 16d);
            //            }

            //            // 見切り
            //            if (t.IsUnderSpecialPowerEffect("特殊防御発動"))
            //            {
            //                prob = 100;
            //            }

            //            // 必中がかかっていれば当て身技は無効
            //            if (IsUnderSpecialPowerEffect("絶対命中") && !t.IsUnderSpecialPowerEffect("特殊防御発動"))
            //            {
            //                break;
            //            }

            //            // 自分の反射や当て身技に対して当て身技は出来ない
            //            if (attack_mode == "反射" || attack_mode == "当て身技")
            //            {
            //                break;
            //            }

            //            // 対象属性の判定
            //            bool localIsAttributeClassified1() { string argaclass1 = GeneralLib.LIndex(fdata, 3); string argaclass2 = w.WeaponClass(); var ret = t.IsAttributeClassified(argaclass1, argaclass2); return ret; }

            //            if (!localIsAttributeClassified1())
            //            {
            //                prob = 0;
            //            }

            //            // 使用条件
            //            if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
            //            {
            //                ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 5));
            //            }
            //            else
            //            {
            //                ecost = 0;
            //            }

            //            if (Information.IsNumeric(GeneralLib.LIndex(fdata, 6)))
            //            {
            //                nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 6));
            //            }
            //            else
            //            {
            //                nmorale = 0;
            //            }

            //            if (t.EN < ecost || t.MainPilot().Morale < nmorale)
            //            {
            //                prob = 0;
            //            }

            //            // オプション
            //            slevel = 0d;
            //            var loopTo6 = GeneralLib.LLength(fdata);
            //            for (j = 7; j <= loopTo6; j++)
            //            {
            //                if (prob == 0)
            //                {
            //                    break;
            //                }

            //                opt = GeneralLib.LIndex(fdata, j);
            //                idx = Strings.InStr(opt, "*");
            //                if (idx > 0)
            //                {
            //                    lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
            //                    opt = Strings.Left(opt, idx - 1);
            //                }
            //                else
            //                {
            //                    lv_mod = -1;
            //                }

            //                switch (t.MainPilot().SkillType(opt) ?? "")
            //                {
            //                    case "相殺":
            //                        {
            //                            if (IsSameCategory(fdata, FeatureData("当て身技")) && Math.Abs((x - tx)) + Math.Abs((y - ty)) == 1)
            //                            {
            //                                prob = 0;
            //                            }

            //                            break;
            //                        }

            //                    case "中和":
            //                        {
            //                            if (IsSameCategory(fdata, FeatureData("当て身技")) && Math.Abs((x - tx)) + Math.Abs((y - ty)) == 1)
            //                            {
            //                                flevel = flevel - FeatureLevel("当て身技");
            //                                if (flevel <= 0d)
            //                                {
            //                                    prob = 0;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "近接無効":
            //                        {
            //                            if (w.IsWeaponClassifiedAs("武") || w.IsWeaponClassifiedAs("突") || w.IsWeaponClassifiedAs("接"))
            //                            {
            //                                prob = 0;
            //                            }

            //                            break;
            //                        }

            //                    case "手動":
            //                        {
            //                            if (def_mode != "防御")
            //                            {
            //                                prob = 0;
            //                            }

            //                            break;
            //                        }

            //                    case "能力必要":
            //                        {
            //                            break;
            //                        }
            //                    // スキップ
            //                    case "同調率":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 20d;
            //                            }

            //                            slevel = lv_mod * (t.SyncLevel() - 30d);
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == -30 * lv_mod)
            //                                {
            //                                    prob = 0;
            //                                }
            //                            }
            //                            else if (slevel == -30 * lv_mod)
            //                            {
            //                                slevel = 0d;
            //                            }

            //                            break;
            //                        }

            //                    case "霊力":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 10d;
            //                            }

            //                            slevel = lv_mod * t.PlanaLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    prob = 0;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "オーラ":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 200d;
            //                            }

            //                            slevel = lv_mod * t.AuraLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    prob = 0;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "超能力":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 200d;
            //                            }

            //                            slevel = lv_mod * t.PsychicLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    prob = 0;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 200d;
            //                            }

            //                            slevel = lv_mod * t.SkillLevel(opt);
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    prob = 0;
            //                                }
            //                            }

            //                            break;
            //                        }
            //                }
            //            }

            //            // ダメージが許容範囲外であれば当て身技を使えない
            //            if (dmg > 500d * flevel + slevel)
            //            {
            //                prob = 0;
            //            }

            //            // 使用する当て身技を検索
            //            wname2 = GeneralLib.LIndex(fdata, 2);
            //            w2 = 0;
            //            var loopTo7 = t.CountWeapon();
            //            for (j = 1; j <= loopTo7; j++)
            //            {
            //                if ((t.Weapon(j).Name ?? "") == (wname2 ?? ""))
            //                {
            //                    if (t.IsWeaponAvailable(j, "必要技能無視"))
            //                    {
            //                        w2 = j;
            //                    }

            //                    break;
            //                }
            //            }

            //            // 当て身技発動
            //            if (prob >= GeneralLib.Dice(100) && w2 > 0)
            //            {
            //                if (ecost != 0)
            //                {
            //                    t.EN = t.EN - ecost;
            //                    GUI.UpdateMessageForm(this, t);
            //                }

            //                // メッセージ
            //                if (!be_quiet)
            //                {
            //                    if (t.IsMessageDefined("当て身技(" + fname + ")"))
            //                    {
            //                        t.PilotMessage("当て身技(" + fname + ")", msg_mode: "");
            //                    }
            //                    else
            //                    {
            //                        t.PilotMessage("当て身技", msg_mode: "");
            //                    }
            //                }
            //                else
            //                {
            //                    Sound.IsWavePlayed = false;
            //                }

            //                // 効果音
            //                if (!Sound.IsWavePlayed)
            //                {
            //                    bool localIsSpecialEffectDefined2() { string argmain_situation = wname + "(当て身技)"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //                    if (IsAnimationDefined(wname + "(当て身技)", sub_situation: ""))
            //                    {
            //                        PlayAnimation(wname + "(当て身技)", sub_situation: "");
            //                    }
            //                    else if (localIsSpecialEffectDefined2())
            //                    {
            //                        SpecialEffect(wname + "(当て身技)", sub_situation: "");
            //                    }
            //                    else if (t.IsAnimationDefined("当て身技", fname))
            //                    {
            //                        t.PlayAnimation("当て身技", fname);
            //                    }
            //                    else if (t.IsSpecialEffectDefined("当て身技", fname))
            //                    {
            //                        t.SpecialEffect("当て身技", fname);
            //                    }
            //                    else if (SRC.BattleAnimation)
            //                    {
            //                        Effect.ShowAnimation("打突命中");
            //                    }
            //                    else if (w.IsWeaponClassifiedAs("武") || w.IsWeaponClassifiedAs("突") || w.IsWeaponClassifiedAs("接") || w.IsWeaponClassifiedAs("実"))
            //                    {
            //                        Sound.PlayWave("Sword.wav");
            //                    }
            //                    else
            //                    {
            //                        Sound.PlayWave("BeamCoat.wav");
            //                    }
            //                }

            //                if (t.IsSysMessageDefined("当て身技", fname))
            //                {
            //                    t.SysMessage("当て身技", fname, add_msg: "");
            //                }
            //                else
            //                {
            //                    GUI.DisplaySysMessage(t.Nickname + "は[" + fname + "]で[" + wname + "]を受け止めた。");
            //                }

            //                // 当て身技で攻撃をかける
            //                t.Attack(w2, this, "当て身技", "");
            //                t = t.CurrentForm();
            //                CheckParryFeatureRet = true;
            //                return CheckParryFeatureRet;
            //            }
            //        }
            //    }

            //SkipParryAttack:
            //    ;


            //    // 切り払いに使用する武器を調べる
            //    wname2 = "";
            //    if (t.IsFeatureAvailable("格闘武器"))
            //    {
            //        wname2 = t.FeatureData("格闘武器");
            //    }
            //    else
            //    {
            //        var loopTo8 = t.CountWeapon();
            //        for (i = 1; i <= loopTo8; i++)
            //        {
            //            if (t.IsWeaponClassifiedAs(i, "武") && !t.IsWeaponClassifiedAs(i, "合") && t.IsWeaponMastered(i) && t.MainPilot().Morale >= t.Weapon(i).NecessaryMorale && !t.IsDisabled(t.Weapon(i).Name))
            //            {
            //                wname2 = t.WeaponNickname(i);
            //                break;
            //            }
            //        }
            //    }

            //    // 切り払い出来る？
            //    if (t.MainPilot().SkillLevel("切り払い", ref_mode: "") > 0d && Strings.Len(wname2) > 0)
            //    {
            //        // 武属性や突属性を持っていても切り払いの対象外になります
            //        if (w.IsWeaponClassifiedAs("実"))
            //        {
            //            prob = 0;

            //            // 思念誘導はＮＴレベルに応じて切り払いにくくなる
            //            if (w.IsWeaponClassifiedAs("サ"))
            //            {
            //                prob = (t.MainPilot().SkillLevel("超感覚", ref_mode: "") + t.MainPilot().SkillLevel("知覚強化", ref_mode: ""));
            //                prob = (prob - MainPilot().SkillLevel("超感覚", ref_mode: "") - MainPilot().SkillLevel("知覚強化", ref_mode: ""));
            //                if (prob > 0)
            //                {
            //                    prob = 0;
            //                }
            //            }
            //            else
            //            {
            //                prob = 0;
            //            }

            //            prob = (prob + 2d * t.MainPilot().SkillLevel("切り払い", ref_mode: ""));

            //            // 見切りがあれば必ず発動
            //            if (t.IsUnderSpecialPowerEffect("特殊防御発動"))
            //            {
            //                prob = 32;
            //            }

            //            if (prob >= GeneralLib.Dice(32))
            //            {
            //                // メッセージ
            //                if (!be_quiet)
            //                {
            //                    if (t.IsMessageDefined("切り払い(" + wname2 + ")"))
            //                    {
            //                        t.PilotMessage("切り払い(" + wname2 + ")", msg_mode: "");
            //                    }
            //                    else
            //                    {
            //                        t.PilotMessage("切り払い", msg_mode: "");
            //                    }
            //                }
            //                else
            //                {
            //                    Sound.IsWavePlayed = false;
            //                }

            //                // 効果音
            //                if (!Sound.IsWavePlayed)
            //                {
            //                    bool localIsSpecialEffectDefined3() { string argmain_situation = wname + "(切り払い)"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //                    if (IsAnimationDefined(wname + "(切り払い)", sub_situation: ""))
            //                    {
            //                        PlayAnimation(wname + "(切り払い)", sub_situation: "");
            //                    }
            //                    else if (localIsSpecialEffectDefined3())
            //                    {
            //                        SpecialEffect(wname + "(切り払い)", sub_situation: "");
            //                    }
            //                    else if (t.IsAnimationDefined("切り払い", wname2))
            //                    {
            //                        t.PlayAnimation("切り払い", wname2);
            //                    }
            //                    else if (t.IsSpecialEffectDefined("切り払い", wname2))
            //                    {
            //                        t.SpecialEffect("切り払い", wname2);
            //                    }
            //                    else
            //                    {
            //                        Effect.ParryEffect(this, w, t);
            //                    }
            //                }

            //                GUI.DisplaySysMessage(t.Nickname + "は[" + wname2 + "]で[" + wname + "]を叩き落とした。");

            //                // 切り払われた永続武器は使用回数を減らす
            //                if (w.IsWeaponClassifiedAs("永") && this.Weapon(w).Bullet > 0)
            //                {
            //                    SetBullet(w, (Bullet(w) - 1));
            //                    SyncBullet();
            //                    IsMapAttackCanceled = true;
            //                }

            //                CheckParryFeatureRet = true;
            //                return CheckParryFeatureRet;
            //            }
            //        }
            //        else if (w.IsWeaponClassifiedAs("接"))
            //        {
            //        }
            //        else if (w.IsWeaponClassifiedAs("突"))
            //        {
            //            // 相手も切り払い出来れば切り払い確率は下がる
            //            prob = (2d * t.MainPilot().SkillLevel("切り払い", ref_mode: "") - MainPilot().SkillLevel("切り払い", ref_mode: ""));

            //            // 見切りがあれば必ず発動
            //            if (t.IsUnderSpecialPowerEffect("特殊防御発動"))
            //            {
            //                prob = 32;
            //            }

            //            if (prob >= GeneralLib.Dice(32))
            //            {
            //                // メッセージ
            //                if (!be_quiet)
            //                {
            //                    if (t.IsMessageDefined("切り払い(" + wname2 + ")"))
            //                    {
            //                        t.PilotMessage("切り払い(" + wname2 + ")", msg_mode: "");
            //                    }
            //                    else
            //                    {
            //                        t.PilotMessage("切り払い", msg_mode: "");
            //                    }
            //                }
            //                else
            //                {
            //                    Sound.IsWavePlayed = false;
            //                }

            //                // 効果音
            //                if (!Sound.IsWavePlayed)
            //                {
            //                    bool localIsSpecialEffectDefined4() { string argmain_situation = wname + "(切り払い)"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //                    if (IsAnimationDefined(wname + "(切り払い)", sub_situation: ""))
            //                    {
            //                        PlayAnimation(wname + "(切り払い)", sub_situation: "");
            //                    }
            //                    else if (localIsSpecialEffectDefined4())
            //                    {
            //                        SpecialEffect(wname + "(切り払い)", sub_situation: "");
            //                    }
            //                    else if (t.IsAnimationDefined("切り払い", wname2))
            //                    {
            //                        t.PlayAnimation("切り払い", wname2);
            //                    }
            //                    else if (t.IsSpecialEffectDefined("切り払い", wname2))
            //                    {
            //                        t.SpecialEffect("切り払い", wname2);
            //                    }
            //                    else
            //                    {
            //                        Effect.DodgeEffect(this, w);
            //                        GUI.Sleep(190);
            //                        Sound.PlayWave("Sword.wav");
            //                    }
            //                }

            //                GUI.DisplaySysMessage(t.Nickname + "は[" + wname2 + "]で[" + wname + "]を受け流した。");

            //                // 切り払われた永続武器は使用回数を減らす
            //                if (w.IsWeaponClassifiedAs("永") && this.Weapon(w).Bullet > 0)
            //                {
            //                    SetBullet(w, (Bullet(w) - 1));
            //                    SyncBullet();
            //                    IsMapAttackCanceled = true;
            //                }

            //                CheckParryFeatureRet = true;
            //                return CheckParryFeatureRet;
            //            }
            //        }
            //        else if (w.IsWeaponClassifiedAs("武"))
            //        {
            //            // 相手も切り払い出来れば切り払い確率は下がる
            //            prob = (2d * t.MainPilot().SkillLevel("切り払い", ref_mode: "") - MainPilot().SkillLevel("切り払い", ref_mode: ""));

            //            // 見切りがあれば必ず発動
            //            if (t.IsUnderSpecialPowerEffect("特殊防御発動"))
            //            {
            //                prob = 32;
            //            }

            //            if (prob >= GeneralLib.Dice(32))
            //            {
            //                // メッセージ
            //                if (!be_quiet)
            //                {
            //                    if (t.IsMessageDefined("切り払い(" + wname2 + ")"))
            //                    {
            //                        t.PilotMessage("切り払い(" + wname2 + ")", msg_mode: "");
            //                    }
            //                    else
            //                    {
            //                        t.PilotMessage("切り払い", msg_mode: "");
            //                    }
            //                }
            //                else
            //                {
            //                    Sound.IsWavePlayed = false;
            //                }

            //                // 効果音
            //                if (!Sound.IsWavePlayed)
            //                {
            //                    bool localIsSpecialEffectDefined5() { string argmain_situation = wname + "(切り払い)"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //                    if (IsAnimationDefined(wname + "(切り払い)", sub_situation: ""))
            //                    {
            //                        PlayAnimation(wname + "(切り払い)", sub_situation: "");
            //                    }
            //                    else if (localIsSpecialEffectDefined5())
            //                    {
            //                        SpecialEffect(wname + "(切り払い)", sub_situation: "");
            //                    }
            //                    else if (t.IsAnimationDefined("切り払い", wname2))
            //                    {
            //                        t.PlayAnimation("切り払い", wname2);
            //                    }
            //                    else if (t.IsSpecialEffectDefined("切り払い", wname2))
            //                    {
            //                        t.SpecialEffect("切り払い", wname2);
            //                    }
            //                    else
            //                    {
            //                        Effect.DodgeEffect(this, w);
            //                        GUI.Sleep(190);
            //                        Sound.PlayWave("Sword.wav");
            //                    }
            //                }

            //                GUI.DisplaySysMessage(t.Nickname + "は[" + wname2 + "]で[" + wname + "]を受けとめた。");

            //                // 切り払われた永続武器は使用回数を減らす
            //                if (w.IsWeaponClassifiedAs("永") && this.Weapon(w).Bullet > 0)
            //                {
            //                    SetBullet(w, (Bullet(w) - 1));
            //                    SyncBullet();
            //                    IsMapAttackCanceled = true;
            //                }

            //                CheckParryFeatureRet = true;
            //                return CheckParryFeatureRet;
            //            }
            //        }
            //    }

            //    // 反射無効化
            //    if (w.IsWeaponClassifiedAs("無") || IsUnderSpecialPowerEffect("防御能力無効化"))
            //    {
            //        return CheckParryFeatureRet;
            //    }

            //    // 攻撃反射の処理
            //    var loopTo9 = t.CountFeature();
            //    for (i = 1; i <= loopTo9; i++)
            //    {
            //        if (t.Feature(i) == "反射")
            //        {
            //            fname = t.FeatureName0(i);
            //            if (string.IsNullOrEmpty(fname))
            //            {
            //                if (t.IsFeatureAvailable("バリアシールド"))
            //                {
            //                    fname = t.FeatureName0("バリアシールド");
            //                }
            //                else
            //                {
            //                    fname = "反射";
            //                }
            //            }

            //            fdata = t.FeatureData(i);
            //            flevel = t.FeatureLevel(i);
            //            if (flevel == 1d)
            //            {
            //                flevel = 10000d;
            //            }

            //            // 反射確率の設定
            //            buf = GeneralLib.LIndex(fdata, 3);
            //            if (Information.IsNumeric(buf))
            //            {
            //                prob = Conversions.Toint(buf);
            //            }
            //            else if (Strings.InStr(buf, "+") > 0 || Strings.InStr(buf, "-") > 0)
            //            {
            //                j = GeneralLib.MaxLng(Strings.InStr(buf, "+"), Strings.InStr(buf, "-"));
            //                prob = (100d * (t.SkillLevel(Strings.Left(buf, j - 1)) + Conversions.Toint(Strings.Mid(buf, j))) / 16d);
            //            }
            //            else
            //            {
            //                prob = (100d * t.SkillLevel(buf) / 16d);
            //            }

            //            // 反射された攻撃を反射する場合は確率を下げる
            //            if (attack_mode == "反射")
            //            {
            //                prob = prob / 2;
            //            }

            //            // 見切り
            //            if (t.IsUnderSpecialPowerEffect("特殊防御発動"))
            //            {
            //                prob = 100;
            //            }

            //            // 当て身技は反射出来ない
            //            if (attack_mode == "当て身技")
            //            {
            //                break;
            //            }

            //            // 必中がかかっていれば反射は無効
            //            if (IsUnderSpecialPowerEffect("絶対命中") && !t.IsUnderSpecialPowerEffect("特殊防御発動"))
            //            {
            //                break;
            //            }

            //            // 対象属性の判定
            //            bool localIsAttributeClassified2() { string argaclass1 = GeneralLib.LIndex(fdata, 2); string argaclass2 = w.WeaponClass(); var ret = t.IsAttributeClassified(argaclass1, argaclass2); return ret; }

            //            if (!localIsAttributeClassified2())
            //            {
            //                prob = 0;
            //            }

            //            // 使用条件
            //            if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //            {
            //                ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //            }
            //            else
            //            {
            //                ecost = 0;
            //            }

            //            if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
            //            {
            //                nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 5));
            //            }
            //            else
            //            {
            //                nmorale = 0;
            //            }

            //            if (t.EN < ecost || t.MainPilot().Morale < nmorale)
            //            {
            //                prob = 0;
            //            }

            //            // オプション
            //            slevel = 0d;
            //            var loopTo10 = GeneralLib.LLength(fdata);
            //            for (j = 6; j <= loopTo10; j++)
            //            {
            //                if (prob == 0)
            //                {
            //                    break;
            //                }

            //                opt = GeneralLib.LIndex(fdata, j);
            //                idx = Strings.InStr(opt, "*");
            //                if (idx > 0)
            //                {
            //                    lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
            //                    opt = Strings.Left(opt, idx - 1);
            //                }
            //                else
            //                {
            //                    lv_mod = -1;
            //                }

            //                switch (t.MainPilot().SkillType(opt) ?? "")
            //                {
            //                    case "相殺":
            //                        {
            //                            string localLIndex2() { object argIndex1 = "阻止"; string arglist = FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //                            if ((GeneralLib.LIndex(fdata, 1) ?? "") == (localLIndex2() ?? "") && Math.Abs((x - tx)) + Math.Abs((y - ty)) == 1)
            //                            {
            //                                msg = msg + Nickname + "は[" + fname + "]を中和した。;";
            //                                prob = 0;
            //                            }

            //                            break;
            //                        }

            //                    case "中和":
            //                        {
            //                            string localLIndex3() { object argIndex1 = "阻止"; string arglist = FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //                            if ((GeneralLib.LIndex(fdata, 1) ?? "") == (localLIndex3() ?? "") && Math.Abs((x - tx)) + Math.Abs((y - ty)) == 1)
            //                            {
            //                                flevel = flevel - FeatureLevel("阻止");
            //                                if (flevel <= 0d)
            //                                {
            //                                    msg = msg + Nickname + "は[" + fname + "]を中和した。;";
            //                                    prob = 0;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "近接無効":
            //                        {
            //                            if (w.IsWeaponClassifiedAs("武") || w.IsWeaponClassifiedAs("突") || w.IsWeaponClassifiedAs("接"))
            //                            {
            //                                prob = 0;
            //                            }

            //                            break;
            //                        }

            //                    case "手動":
            //                        {
            //                            if (def_mode != "防御")
            //                            {
            //                                prob = 0;
            //                            }

            //                            break;
            //                        }

            //                    case "能力必要":
            //                        {
            //                            break;
            //                        }
            //                    // スキップ
            //                    case "同調率":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 20d;
            //                            }

            //                            slevel = lv_mod * (t.SyncLevel() - 30d);
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == -30 * lv_mod)
            //                                {
            //                                    prob = 0;
            //                                }
            //                            }
            //                            else if (slevel == -30 * lv_mod)
            //                            {
            //                                slevel = 0d;
            //                            }

            //                            break;
            //                        }

            //                    case "霊力":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 10d;
            //                            }

            //                            slevel = lv_mod * t.PlanaLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    prob = 0;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "オーラ":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 200d;
            //                            }

            //                            slevel = lv_mod * t.AuraLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    prob = 0;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "超能力":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 200d;
            //                            }

            //                            slevel = lv_mod * t.PsychicLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    prob = 0;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 200d;
            //                            }

            //                            slevel = lv_mod * t.SkillLevel(opt);
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    prob = 0;
            //                                }
            //                            }

            //                            break;
            //                        }
            //                }
            //            }

            //            // ダメージが許容範囲外であれば反射できない
            //            if (dmg > 500d * flevel + slevel)
            //            {
            //                prob = 0;
            //            }

            //            // 攻撃を反射
            //            if (prob >= GeneralLib.Dice(100))
            //            {
            //                if (ecost != 0)
            //                {
            //                    t.EN = t.EN - ecost;
            //                    GUI.UpdateMessageForm(this, t);
            //                }

            //                // メッセージ
            //                if (!be_quiet)
            //                {
            //                    if (t.IsMessageDefined("反射(" + fname + ")"))
            //                    {
            //                        t.PilotMessage("反射(" + fname + ")", msg_mode: "");
            //                    }
            //                    else
            //                    {
            //                        t.PilotMessage("反射", msg_mode: "");
            //                    }
            //                }
            //                else
            //                {
            //                    Sound.IsWavePlayed = false;
            //                }

            //                // 効果音
            //                if (!Sound.IsWavePlayed)
            //                {
            //                    bool localIsSpecialEffectDefined6() { string argmain_situation = wname + "(反射)"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //                    if (IsAnimationDefined(wname + "(反射)", sub_situation: ""))
            //                    {
            //                        PlayAnimation(wname + "(反射)", sub_situation: "");
            //                    }
            //                    else if (localIsSpecialEffectDefined6())
            //                    {
            //                        SpecialEffect(wname + "(反射)", sub_situation: "");
            //                    }
            //                    else if (t.IsAnimationDefined("反射", fname))
            //                    {
            //                        t.PlayAnimation("反射", fname);
            //                    }
            //                    else if (t.IsSpecialEffectDefined("反射", fname))
            //                    {
            //                        t.SpecialEffect("反射", fname);
            //                    }
            //                    else if (SRC.BattleAnimation)
            //                    {
            //                        if (fname == "反射")
            //                        {
            //                            Effect.ShowAnimation("反射発動");
            //                        }
            //                        else
            //                        {
            //                            Effect.ShowAnimation("反射発動 - " + fname);
            //                        }
            //                    }
            //                    else if (w.IsWeaponClassifiedAs("武") || w.IsWeaponClassifiedAs("突") || w.IsWeaponClassifiedAs("接") || w.IsWeaponClassifiedAs("実"))
            //                    {
            //                        Sound.PlayWave("Sword.wav");
            //                    }
            //                    else
            //                    {
            //                        Sound.PlayWave("BeamCoat.wav");
            //                    }
            //                }

            //                if (t.IsSysMessageDefined("反射", fname))
            //                {
            //                    t.SysMessage("反射", fname, add_msg: "");
            //                }
            //                else if (fname != "反射")
            //                {
            //                    GUI.DisplaySysMessage(t.Nickname + "は[" + fname + "]で[" + wname + "]を弾き返した。");
            //                }
            //                else
            //                {
            //                    GUI.DisplaySysMessage(t.Nickname + "は[" + wname + "]を弾き返した。");
            //                }

            //                // 攻撃を反射
            //                if (!w.IsWeaponClassifiedAs("Ｍ") && attack_mode != "反射")
            //                {
            //                    Attack(w, this, "反射", "");
            //                }

            //                CheckParryFeatureRet = true;
            //                return CheckParryFeatureRet;
            //            }
            //        }
            //    }

            //    return CheckParryFeatureRet;
        }

        // ダミー能力のチェック
        private bool CheckDummyFeature(UnitWeapon w, Unit t, bool be_quiet)
        {
            // TODO Impl
            return false;
            //bool CheckDummyFeatureRet = default;
            //string wname;
            //string fname;
            //wname = WeaponNickname(w);
            //if (t.IsConditionSatisfied("ダミー付加"))
            //{
            //    // 命中時の特殊効果
            //    Sound.IsWavePlayed = false;
            //    if (!be_quiet)
            //    {
            //        PilotMessage(wname + "(命中)", msg_mode: "");
            //    }

            //    bool localIsSpecialEffectDefined() { string argmain_situation = wname + "(命中)"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //    if (IsAnimationDefined(wname + "(命中)", sub_situation: "") || IsAnimationDefined(wname, sub_situation: ""))
            //    {
            //        PlayAnimation(wname + "(命中)", sub_situation: "");
            //    }
            //    else if (localIsSpecialEffectDefined())
            //    {
            //        SpecialEffect(wname + "(命中)", sub_situation: "");
            //    }
            //    else if (!Sound.IsWavePlayed)
            //    {
            //        Effect.HitEffect(this, w, t);
            //    }

            //    fname = t.FeatureName("ダミー");
            //    if (Strings.Len(fname) > 0)
            //    {
            //        if (Strings.InStr(fname, "Lv") > 0)
            //        {
            //            fname = Strings.Left(fname, Strings.InStr(fname, "Lv") - 1);
            //        }
            //    }
            //    else
            //    {
            //        fname = "ダミー";
            //    }

            //    if (!be_quiet)
            //    {
            //        if (t.IsMessageDefined("ダミー(" + fname + ")"))
            //        {
            //            t.PilotMessage("ダミー(" + fname + ")", msg_mode: "");
            //        }
            //        else
            //        {
            //            t.PilotMessage("ダミー", msg_mode: "");
            //        }
            //    }

            //    if (t.IsAnimationDefined("ダミー", fname))
            //    {
            //        t.PlayAnimation("ダミー", fname);
            //    }
            //    else
            //    {
            //        t.SpecialEffect("ダミー", fname);
            //    }

            //    if (t.IsSysMessageDefined("ダミー", fname))
            //    {
            //        t.SysMessage("ダミー", fname, add_msg: "");
            //    }
            //    else
            //    {
            //        GUI.DisplaySysMessage(t.Nickname + "は[" + fname + "]を身代わりにして攻撃をかわした。");
            //    }

            //    t.SetConditionLevel("ダミー付加", t.ConditionLevel("ダミー付加") - 1d);
            //    if (t.ConditionLevel("ダミー付加") == 0d)
            //    {
            //        t.DeleteCondition("ダミー付加");
            //    }

            //    CheckDummyFeatureRet = true;
            //}
            //else if (t.IsFeatureAvailable("ダミー"))
            //{
            //    if (t.ConditionLevel("ダミー破壊") < t.FeatureLevel("ダミー"))
            //    {
            //        // 命中時の特殊効果
            //        Sound.IsWavePlayed = false;
            //        if (!be_quiet)
            //        {
            //            PilotMessage(wname + "(命中)", msg_mode: "");
            //        }

            //        bool localIsSpecialEffectDefined1() { string argmain_situation = wname + "(命中)"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //        if (IsAnimationDefined(wname + "(命中)", sub_situation: "") || IsAnimationDefined(wname, sub_situation: ""))
            //        {
            //            PlayAnimation(wname + "(命中)", sub_situation: "");
            //        }
            //        else if (localIsSpecialEffectDefined1())
            //        {
            //            SpecialEffect(wname + "(命中)", sub_situation: "");
            //        }
            //        else if (!Sound.IsWavePlayed)
            //        {
            //            Effect.HitEffect(this, w, t);
            //        }

            //        fname = t.FeatureName("ダミー");
            //        if (Strings.Len(fname) > 0)
            //        {
            //            if (Strings.InStr(fname, "Lv") > 0)
            //            {
            //                fname = Strings.Left(fname, Strings.InStr(fname, "Lv") - 1);
            //            }
            //        }
            //        else
            //        {
            //            fname = "ダミー";
            //        }

            //        if (!be_quiet)
            //        {
            //            if (t.IsMessageDefined("ダミー(" + fname + ")"))
            //            {
            //                t.PilotMessage("ダミー(" + fname + ")", msg_mode: "");
            //            }
            //            else
            //            {
            //                t.PilotMessage("ダミー", msg_mode: "");
            //            }
            //        }

            //        if (t.IsAnimationDefined("ダミー", fname))
            //        {
            //            t.PlayAnimation("ダミー", fname);
            //        }
            //        else
            //        {
            //            t.SpecialEffect("ダミー", fname);
            //        }

            //        if (IsSysMessageDefined("ダミー", fname))
            //        {
            //            SysMessage("ダミー", fname, add_msg: "");
            //        }
            //        else
            //        {
            //            GUI.DisplaySysMessage(t.Nickname + "は[" + fname + "]を身代わりにして攻撃をかわした。");
            //        }

            //        if (t.IsConditionSatisfied("ダミー破壊"))
            //        {
            //            t.SetConditionLevel("ダミー破壊", t.ConditionLevel("ダミー破壊") + 1d);
            //        }
            //        else
            //        {
            //            t.AddCondition("ダミー破壊", -1, 1d, cdata: "");
            //        }

            //        CheckDummyFeatureRet = true;
            //    }
            //}

            //return CheckDummyFeatureRet;
        }

        // シールド防御能力のチェック
        private bool CheckShieldFeature(UnitWeapon w, Unit t, int dmg, bool be_quiet, bool use_shield, bool use_shield_msg)
        {
            // TODO Impl
            return false;
            //int prob;
            //string fname;

            //// ダメージが0以下ならシールド防御しても意味がない
            //if (dmg <= 0)
            //{
            //    return default;
            //}

            //// Ｓ防御技能を持っている？
            //if (t.MainPilot().SkillLevel("Ｓ防御", ref_mode: "") <= 0d)
            //{
            //    return default;
            //}

            //// 行動可能？
            //if (t.IsConditionSatisfied("行動不能") || t.IsConditionSatisfied("麻痺") || t.IsConditionSatisfied("石化") || t.IsConditionSatisfied("凍結") || t.IsConditionSatisfied("睡眠") || t.IsUnderSpecialPowerEffect("行動不能") || t.IsUnderSpecialPowerEffect("行動不能"1))
            //{
            //    return default;
            //}

            //// シールド防御出来ない武器？
            //if (w.IsWeaponClassifiedAs("精") || w.IsWeaponClassifiedAs("殺") || w.IsWeaponClassifiedAs("浸"))
            //{
            //    return default;
            //}

            //// スペシャルパワーで無効化される？
            //if (IsUnderSpecialPowerEffect("シールド防御無効化"))
            //{
            //    return default;
            //}

            //// シールド系防御能力を検索
            //if (t.IsFeatureAvailable("シールド"))
            //{
            //    prob = t.MainPilot().SkillLevel("Ｓ防御", ref_mode: "");
            //    fname = t.FeatureName("シールド");
            //}
            //else if (t.IsFeatureAvailable("小型シールド"))
            //{
            //    prob = t.MainPilot().SkillLevel("Ｓ防御", ref_mode: "");
            //    fname = t.FeatureName("小型シールド");
            //}
            //else if (t.IsFeatureAvailable("エネルギーシールド") && t.EN > 5 && !w.IsWeaponClassifiedAs("無") && !IsUnderSpecialPowerEffect("防御能力無効化"))
            //{
            //    prob = t.MainPilot().SkillLevel("Ｓ防御", ref_mode: "");
            //    fname = t.FeatureName("エネルギーシールド");
            //}
            //else if (t.IsFeatureAvailable("大型シールド"))
            //{
            //    prob = (t.MainPilot().SkillLevel("Ｓ防御", ref_mode: "") + 1d);
            //    fname = t.FeatureName("大型シールド");
            //}
            //else if (t.IsFeatureAvailable("アクティブシールド"))
            //{
            //    prob = (t.MainPilot().SkillLevel("Ｓ防御", ref_mode: "") + 2d);
            //    fname = t.FeatureName("アクティブシールド");
            //}
            //else
            //{
            //    // 使用可能なシールド系防御能力が無かった
            //    return default;
            //}

            //// シールド発動確率を満たしている？
            //if (prob >= GeneralLib.Dice(16) || t.IsUnderSpecialPowerEffect("特殊防御発動"))
            //{
            //    use_shield = true;
            //    if (w.IsWeaponClassifiedAs("破"))
            //    {
            //        if (t.IsFeatureAvailable("小型シールド"))
            //        {
            //            dmg = 5 * dmg / 6;
            //        }
            //        else
            //        {
            //            dmg = 3 * dmg / 4;
            //        }
            //    }
            //    else
            //    {
            //        if (t.IsFeatureAvailable("小型シールド"))
            //        {
            //            dmg = 2 * dmg / 3;
            //        }
            //        else
            //        {
            //            dmg = dmg / 2;
            //        }
            //    }

            //    if (dmg > 0 && dmg < 10)
            //    {
            //        dmg = 10;
            //    }

            //    if (dmg < t.HP && !be_quiet)
            //    {
            //        if (t.IsMessageDefined("シールド防御(" + fname + ")"))
            //        {
            //            t.PilotMessage("シールド防御(" + fname + ")", msg_mode: "");
            //            use_shield_msg = true;
            //        }
            //        else if (t.IsMessageDefined("シールド防御"))
            //        {
            //            t.PilotMessage("シールド防御", msg_mode: "");
            //            use_shield_msg = true;
            //        }
            //    }

            //    if (t.IsAnimationDefined("シールド防御", fname))
            //    {
            //        t.PlayAnimation("シールド防御", fname);
            //    }
            //    else if (t.IsSpecialEffectDefined("シールド防御", fname))
            //    {
            //        t.SpecialEffect("シールド防御", fname);
            //    }
            //    else
            //    {
            //        Effect.ShieldEffect(t);
            //    }
            //}

            //return default;
        }

        // バリアなどの防御能力のチェック
        private bool CheckDefenseFeature(UnitWeapon w, Unit t, int tx, int ty, string attack_mode, string def_mode, int dmg, string msg, bool be_quiet, bool is_penetrated)
        {
            // TODO Impl
            return false;
            //    bool CheckDefenseFeatureRet = default;
            //    string wname;
            //    int ecost, nmorale;
            //    string fname, fdata;
            //    double flevel;
            //    int fid, frange;
            //    string opt;
            //    double lv_mod;
            //    Unit u;
            //    double slevel;
            //    int k, i, j, idx;
            //    bool neautralize;
            //    string team, uteam;
            //    double dmg_mod;
            //    bool defined;
            //    wname = WeaponNickname(w);
            //    team = MainPilot().SkillData("チーム");

            //    // 攻撃吸収
            //    if (dmg < 0)
            //    {
            //        t.HP = t.HP - dmg;
            //        if (attack_mode != "反射")
            //        {
            //            GUI.UpdateMessageForm(this, t);
            //        }
            //        else
            //        {
            //            GUI.UpdateMessageForm(this, null);
            //        }

            //        Effect.NegateEffect(this, t, w, wname, dmg, "吸収", "", 0, msg, be_quiet);
            //        CheckDefenseFeatureRet = true;
            //        return CheckDefenseFeatureRet;
            //    }

            //    // 攻撃無効化
            //    if (dmg == 0 && this.Weapon(w).Power > 0)
            //    {
            //        if (w.IsWeaponClassifiedAs("封") || w.IsWeaponClassifiedAs("限"))
            //        {
            //            GUI.DisplaySysMessage(msg + t.Nickname + "には[" + wname + "]は通用しない。");
            //        }
            //        else
            //        {
            //            Effect.NegateEffect(this, t, w, wname, dmg, "", "", 0, msg, be_quiet);
            //        }

            //        CheckDefenseFeatureRet = true;
            //        return CheckDefenseFeatureRet;
            //    }

            //    // 特殊効果がない場合にはクリティカル発生の可能性がある
            //    if (!IsNormalWeapon(w))
            //    {
            //        // 特殊効果を伴う武器
            //        if (CriticalProbability(w, t, def_mode) == 0 && this.Weapon(w).Power == 0)
            //        {
            //            // 攻撃力が0の攻撃は、クリティカル発生率が0の場合も無効化されていると見なす
            //            Effect.NegateEffect(this, t, w, wname, dmg, "", "", 0, msg, be_quiet);
            //            CheckDefenseFeatureRet = true;
            //            return CheckDefenseFeatureRet;
            //        }
            //    }

            //    // バリア無効化
            //    if (w.IsWeaponClassifiedAs("無") || IsUnderSpecialPowerEffect("防御能力無効化"))
            //    {
            //        goto SkipBarrier;
            //    }

            //    // 広域バリア
            //    // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    u = null;
            //    flevel = 0d;
            //    fid = 0;
            //    // バリアをはってくれるユニットを探す
            //    var loopTo = GeneralLib.MinLng(tx + 3, Map.MapWidth);
            //    for (i = GeneralLib.MaxLng(tx - 3, 1); i <= loopTo; i++)
            //    {
            //        var loopTo1 = GeneralLib.MinLng(ty + 3, Map.MapHeight);
            //        for (j = GeneralLib.MaxLng(ty - 3, 1); j <= loopTo1; j++)
            //        {
            //            if (Map.MapDataForUnit[i, j] is null || Math.Abs((tx - i)) + Math.Abs((ty - j)) > 3)
            //            {
            //                goto NextPoint;
            //            }

            //            {
            //                var withBlock = Map.MapDataForUnit[i, j];
            //                // 敵？
            //                if (withBlock.IsEnemy(t))
            //                {
            //                    goto NextPoint;
            //                }

            //                // 行動不能？
            //                if (withBlock.MaxAction() == 0)
            //                {
            //                    goto NextPoint;
            //                }

            //                // 地中にいる？
            //                if (withBlock.Area == "地中")
            //                {
            //                    goto NextPoint;
            //                }

            //                // 広域バリアを持っている？
            //                if (!withBlock.IsFeatureAvailable("広域バリア"))
            //                {
            //                    goto NextPoint;
            //                }

            //                // 同じチームに属している？
            //                uteam = withBlock.MainPilot().SkillData("チーム");
            //                if ((team ?? "") != (uteam ?? "") && !string.IsNullOrEmpty(uteam))
            //                {
            //                    goto NextPoint;
            //                }

            //                var loopTo2 = withBlock.CountFeature();
            //                for (k = 1; k <= loopTo2; k++)
            //                {
            //                    if (withBlock.Feature(k) == "広域バリア")
            //                    {
            //                        fdata = withBlock.FeatureData(k);

            //                        // 効果範囲
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
            //                        {
            //                            frange = Conversions.Toint(GeneralLib.LIndex(fdata, 2));
            //                        }
            //                        else
            //                        {
            //                            frange = 1;
            //                        }

            //                        // 使用条件
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //                        {
            //                            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //                        }
            //                        else
            //                        {
            //                            ecost = (20 * frange);
            //                        }

            //                        if (withBlock.IsConditionSatisfied("バリア発動"))
            //                        {
            //                            // すでに発動済み
            //                            ecost = 0;
            //                        }

            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
            //                        {
            //                            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 5));
            //                        }
            //                        else
            //                        {
            //                            nmorale = 0;
            //                        }

            //                        // 発動可能かチェック
            //                        bool localIsAttributeClassified() { string argaclass1 = GeneralLib.LIndex(fdata, 3); string argaclass2 = w.WeaponClass(); var ret = withBlock.IsAttributeClassified(argaclass1, argaclass2); return ret; }

            //                        if (withBlock.EN >= ecost && withBlock.MainPilot().Morale >= nmorale && localIsAttributeClassified() && (Math.Abs((tx - i)) + Math.Abs((ty - j))) <= frange && (Math.Abs((x - i)) + Math.Abs((y - j))) > frange && (!ReferenceEquals(Map.MapDataForUnit[i, j], t) || !t.IsFeatureAvailable("バリア")) && !withBlock.IsConditionSatisfied("バリア無効化"))
            //                        {
            //                            if (withBlock.FeatureLevel(k) > flevel)
            //                            {
            //                                u = Map.MapDataForUnit[i, j];
            //                                flevel = withBlock.FeatureLevel(k);
            //                                fid = k;
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //        NextPoint:
            //            ;
            //        }
            //    }

            //    if (u is object)
            //    {
            //        // バリアをはってくれるユニットがいる場合
            //        if (fid == 0)
            //        {
            //            fname = u.FeatureName0("広域バリア");
            //            fdata = u.FeatureData("広域バリア");
            //        }
            //        else
            //        {
            //            fname = u.FeatureName0(fid);
            //            fdata = u.FeatureData(fid);
            //        }

            //        if (string.IsNullOrEmpty(fname))
            //        {
            //            if (u.IsFeatureAvailable("バリア"))
            //            {
            //                fname = u.FeatureName0("バリア");
            //            }
            //            else
            //            {
            //                fname = "広域バリア";
            //            }
            //        }

            //        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //        {
            //            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //        }
            //        else
            //        {
            //            ecost = 20;
            //        }

            //        if (!u.IsConditionSatisfied("バリア発動"))
            //        {
            //            // バリア発動はターン中に一度のみ
            //            u.EN = u.EN - ecost;
            //            if (u.IsMessageDefined("バリア発動(" + fname + ")"))
            //            {
            //                u.PilotMessage("バリア発動(" + fname + ")", msg_mode: "");
            //            }
            //            else
            //            {
            //                u.PilotMessage("バリア発動", msg_mode: "");
            //            }

            //            if (u.IsAnimationDefined("バリア発動", fname))
            //            {
            //                u.PlayAnimation("バリア発動", fname);
            //            }
            //            else
            //            {
            //                u.SpecialEffect("バリア発動", fname);
            //            }

            //            if (u.IsSysMessageDefined("バリア発動", fname))
            //            {
            //                u.SysMessage("バリア発動", fname, add_msg: "");
            //            }
            //            else
            //            {
            //                GUI.DisplaySysMessage(u.Nickname + "は[" + fname + "]を発動させた。");
            //            }

            //            if (fname == "広域バリア" || fname == "バリア")
            //            {
            //                u.AddCondition("バリア発動", 1, cdata: "");
            //            }
            //            else
            //            {
            //                u.AddCondition("バリア発動", 1, 0d, fname + "発動");
            //            }
            //        }

            //        if (1000d * flevel >= dmg)
            //        {
            //            Effect.NegateEffect(this, t, w, wname, dmg, fname, fdata, 10, msg, be_quiet);
            //            CheckDefenseFeatureRet = true;
            //            return CheckDefenseFeatureRet;
            //        }
            //        else if (flevel > 0d)
            //        {
            //            msg = msg + wname + "が[" + fname + "]を貫いた。;";
            //        }
            //    }

            //    // 広域フィールド
            //    // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    u = null;
            //    flevel = 0d;
            //    fid = 0;
            //    // フィールドをはってくれるユニットを探す
            //    var loopTo3 = GeneralLib.MinLng(tx + 3, Map.MapWidth);
            //    for (i = GeneralLib.MaxLng(tx - 3, 1); i <= loopTo3; i++)
            //    {
            //        var loopTo4 = GeneralLib.MinLng(ty + 3, Map.MapHeight);
            //        for (j = GeneralLib.MaxLng(ty - 3, 1); j <= loopTo4; j++)
            //        {
            //            if (Map.MapDataForUnit[i, j] is null || Math.Abs((tx - i)) + Math.Abs((ty - j)) > 3)
            //            {
            //                goto NextPoint2;
            //            }

            //            {
            //                var withBlock1 = Map.MapDataForUnit[i, j];
            //                // 敵？
            //                if (withBlock1.IsEnemy(t))
            //                {
            //                    goto NextPoint2;
            //                }

            //                // 行動不能？
            //                if (withBlock1.MaxAction() == 0)
            //                {
            //                    goto NextPoint2;
            //                }

            //                // 地中にいる？
            //                if (withBlock1.Area == "地中")
            //                {
            //                    goto NextPoint2;
            //                }

            //                // 広域フィールドを持っている？
            //                if (!withBlock1.IsFeatureAvailable("広域フィールド"))
            //                {
            //                    goto NextPoint2;
            //                }

            //                // 同じチームに属している？
            //                uteam = withBlock1.MainPilot().SkillData("チーム");
            //                if ((team ?? "") != (uteam ?? "") && !string.IsNullOrEmpty(uteam))
            //                {
            //                    goto NextPoint2;
            //                }

            //                var loopTo5 = withBlock1.CountFeature();
            //                for (k = 1; k <= loopTo5; k++)
            //                {
            //                    if (withBlock1.Feature(k) == "広域フィールド")
            //                    {
            //                        fdata = withBlock1.FeatureData(k);

            //                        // 効果範囲
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
            //                        {
            //                            frange = Conversions.Toint(GeneralLib.LIndex(fdata, 2));
            //                        }
            //                        else
            //                        {
            //                            frange = 1;
            //                        }

            //                        // 使用条件
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //                        {
            //                            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //                        }
            //                        else
            //                        {
            //                            ecost = (20 * frange);
            //                        }

            //                        if (withBlock1.IsConditionSatisfied("フィールド発動"))
            //                        {
            //                            // すでに発動済み
            //                            ecost = 0;
            //                        }

            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
            //                        {
            //                            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 5));
            //                        }
            //                        else
            //                        {
            //                            nmorale = 0;
            //                        }

            //                        // 発動可能かチェック
            //                        bool localIsAttributeClassified1() { string argaclass1 = GeneralLib.LIndex(fdata, 3); string argaclass2 = w.WeaponClass(); var ret = withBlock1.IsAttributeClassified(argaclass1, argaclass2); return ret; }

            //                        if (withBlock1.EN >= ecost && withBlock1.MainPilot().Morale >= nmorale && localIsAttributeClassified1() && (Math.Abs((tx - i)) + Math.Abs((ty - j))) <= frange && (Math.Abs((x - i)) + Math.Abs((y - j))) > frange && (!ReferenceEquals(Map.MapDataForUnit[i, j], t) || !t.IsFeatureAvailable("フィールド")) && !withBlock1.IsConditionSatisfied("バリア無効化"))
            //                        {
            //                            if (withBlock1.FeatureLevel(k) > flevel)
            //                            {
            //                                u = Map.MapDataForUnit[i, j];
            //                                flevel = withBlock1.FeatureLevel(k);
            //                                fid = k;
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //        NextPoint2:
            //            ;
            //        }
            //    }

            //    if (u is object)
            //    {
            //        // フィールドをはってくれるユニットがいる場合
            //        if (fid == 0)
            //        {
            //            fname = u.FeatureName0("広域フィールド");
            //            fdata = u.FeatureData("広域フィールド");
            //        }
            //        else
            //        {
            //            fname = u.FeatureName0(fid);
            //            fdata = u.FeatureData(fid);
            //        }

            //        if (string.IsNullOrEmpty(fname))
            //        {
            //            if (u.IsFeatureAvailable("フィールド"))
            //            {
            //                fname = u.FeatureName0("フィールド");
            //            }
            //            else
            //            {
            //                fname = "広域フィールド";
            //            }
            //        }

            //        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //        {
            //            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //        }
            //        else
            //        {
            //            ecost = 20;
            //        }

            //        if (!u.IsConditionSatisfied("フィールド発動"))
            //        {
            //            // フィールド発動はターン中に一度のみ
            //            u.EN = u.EN - ecost;
            //            if (u.IsMessageDefined("フィールド発動(" + fname + ")"))
            //            {
            //                u.PilotMessage("フィールド発動(" + fname + ")", msg_mode: "");
            //            }
            //            else
            //            {
            //                u.PilotMessage("フィールド発動", msg_mode: "");
            //            }

            //            if (u.IsAnimationDefined("フィールド発動", fname))
            //            {
            //                u.PlayAnimation("フィールド発動", fname);
            //            }
            //            else
            //            {
            //                u.SpecialEffect("フィールド発動", fname);
            //            }

            //            if (u.IsSysMessageDefined("フィールド発動", fname))
            //            {
            //                u.SysMessage("フィールド発動", fname, add_msg: "");
            //            }
            //            else
            //            {
            //                GUI.DisplaySysMessage(u.Nickname + "は[" + fname + "]を発動させた。");
            //            }

            //            if (fname == "広域フィールド" || fname == "フィールド")
            //            {
            //                u.AddCondition("フィールド発動", 1, cdata: "");
            //            }
            //            else
            //            {
            //                u.AddCondition("フィールド発動", 1, 0d, fname + "発動");
            //            }
            //        }

            //        if (500d * flevel >= dmg)
            //        {
            //            Effect.NegateEffect(this, t, w, wname, dmg, fname, fdata, 10, msg, be_quiet);
            //            CheckDefenseFeatureRet = true;
            //            return CheckDefenseFeatureRet;
            //        }
            //        else if (flevel > 0d)
            //        {
            //            dmg = (dmg - 500d * flevel);
            //            msg = msg + wname + "が[" + fname + "]を貫いた。;";
            //        }
            //    }

            //    // 広域プロテクション
            //    // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    u = null;
            //    flevel = 0d;
            //    fid = 0;
            //    // プロテクションをはってくれるユニットを探す
            //    var loopTo6 = GeneralLib.MinLng(tx + 3, Map.MapWidth);
            //    for (i = GeneralLib.MaxLng(tx - 3, 1); i <= loopTo6; i++)
            //    {
            //        var loopTo7 = GeneralLib.MinLng(ty + 3, Map.MapHeight);
            //        for (j = GeneralLib.MaxLng(ty - 3, 1); j <= loopTo7; j++)
            //        {
            //            if (Map.MapDataForUnit[i, j] is null || Math.Abs((tx - i)) + Math.Abs((ty - j)) > 3)
            //            {
            //                goto NextPoint3;
            //            }

            //            {
            //                var withBlock2 = Map.MapDataForUnit[i, j];
            //                // 敵？
            //                if (withBlock2.IsEnemy(t))
            //                {
            //                    goto NextPoint3;
            //                }

            //                // 行動不能？
            //                if (withBlock2.MaxAction() == 0)
            //                {
            //                    goto NextPoint3;
            //                }

            //                // 地中にいる？
            //                if (withBlock2.Area == "地中")
            //                {
            //                    goto NextPoint3;
            //                }

            //                // 広域プロテクションを持っている？
            //                if (!withBlock2.IsFeatureAvailable("広域プロテクション"))
            //                {
            //                    goto NextPoint3;
            //                }

            //                // 同じチームに属している？
            //                uteam = withBlock2.MainPilot().SkillData("チーム");
            //                if ((team ?? "") != (uteam ?? "") && !string.IsNullOrEmpty(uteam))
            //                {
            //                    goto NextPoint3;
            //                }

            //                var loopTo8 = withBlock2.CountFeature();
            //                for (k = 1; k <= loopTo8; k++)
            //                {
            //                    if (withBlock2.Feature(k) == "広域プロテクション")
            //                    {
            //                        fdata = withBlock2.FeatureData(k);

            //                        // 効果範囲
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
            //                        {
            //                            frange = Conversions.Toint(GeneralLib.LIndex(fdata, 2));
            //                        }
            //                        else
            //                        {
            //                            frange = 1;
            //                        }

            //                        // 使用条件
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //                        {
            //                            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //                        }
            //                        else
            //                        {
            //                            ecost = (20 * frange);
            //                        }

            //                        if (withBlock2.IsConditionSatisfied("プロテクション発動"))
            //                        {
            //                            // すでに発動済み
            //                            ecost = 0;
            //                        }

            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
            //                        {
            //                            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 5));
            //                        }
            //                        else
            //                        {
            //                            nmorale = 0;
            //                        }

            //                        // 発動可能かチェック
            //                        bool localIsAttributeClassified2() { string argaclass1 = GeneralLib.LIndex(fdata, 3); string argaclass2 = w.WeaponClass(); var ret = withBlock2.IsAttributeClassified(argaclass1, argaclass2); return ret; }

            //                        if (withBlock2.EN >= ecost && withBlock2.MainPilot().Morale >= nmorale && localIsAttributeClassified2() && (Math.Abs((tx - i)) + Math.Abs((ty - j))) <= frange && (Math.Abs((x - i)) + Math.Abs((y - j))) > frange && (!ReferenceEquals(Map.MapDataForUnit[i, j], t) || !t.IsFeatureAvailable("プロテクション")) && !withBlock2.IsConditionSatisfied("バリア無効化"))
            //                        {
            //                            if (withBlock2.FeatureLevel(k) > flevel)
            //                            {
            //                                u = Map.MapDataForUnit[i, j];
            //                                flevel = withBlock2.FeatureLevel(k);
            //                                fid = k;
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //        NextPoint3:
            //            ;
            //        }
            //    }

            //    if (u is object)
            //    {
            //        // プロテクションをはってくれるユニットがいる場合
            //        if (fid == 0)
            //        {
            //            fname = u.FeatureName0("広域プロテクション");
            //            fdata = u.FeatureData("広域プロテクション");
            //        }
            //        else
            //        {
            //            fname = u.FeatureName0(fid);
            //            fdata = u.FeatureData(fid);
            //        }

            //        if (string.IsNullOrEmpty(fname))
            //        {
            //            if (u.IsFeatureAvailable("プロテクション"))
            //            {
            //                fname = u.FeatureName0("プロテクション");
            //            }
            //            else
            //            {
            //                fname = "広域プロテクション";
            //            }
            //        }

            //        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //        {
            //            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //        }
            //        else
            //        {
            //            ecost = 20;
            //        }

            //        if (!u.IsConditionSatisfied("プロテクション発動"))
            //        {
            //            // プロテクション発動はターン中に一度のみ
            //            u.EN = u.EN - ecost;
            //            if (u.IsMessageDefined("プロテクション発動(" + fname + ")"))
            //            {
            //                u.PilotMessage("プロテクション発動(" + fname + ")", msg_mode: "");
            //            }
            //            else
            //            {
            //                u.PilotMessage("プロテクション発動", msg_mode: "");
            //            }

            //            if (u.IsAnimationDefined("プロテクション発動", fname))
            //            {
            //                u.PlayAnimation("プロテクション発動", fname);
            //            }
            //            else
            //            {
            //                u.SpecialEffect("プロテクション発動", fname);
            //            }

            //            if (u.IsSysMessageDefined("プロテクション発動", fname))
            //            {
            //                u.SysMessage("プロテクション発動", fname, add_msg: "");
            //            }
            //            else
            //            {
            //                GUI.DisplaySysMessage(u.Nickname + "は[" + fname + "]を発動させた。");
            //            }

            //            if (fname == "広域プロテクション" || fname == "プロテクション")
            //            {
            //                u.AddCondition("プロテクション発動", 1, cdata: "");
            //            }
            //            else
            //            {
            //                u.AddCondition("プロテクション発動", 1, 0d, fname + "発動");
            //            }
            //        }

            //        dmg = ((long)(dmg * (10d - flevel)) / 10L);
            //        if (dmg < 0)
            //        {
            //            msg = msg + u.Nickname + "がダメージを吸収した。;";
            //            u.HP = u.HP - dmg;
            //            CheckDefenseFeatureRet = true;
            //            return CheckDefenseFeatureRet;
            //        }
            //        else if (flevel > 0d)
            //        {
            //            msg = msg + u.Nickname + "の[" + fname + "]がダメージを減少させた。;";
            //        }
            //    }

            //    // バリア能力
            //    var loopTo9 = t.CountFeature();
            //    for (i = 1; i <= loopTo9; i++)
            //    {
            //        if (t.Feature(i) == "バリア")
            //        {
            //            fname = t.FeatureName0(i);
            //            if (string.IsNullOrEmpty(fname))
            //            {
            //                if (t.IsFeatureAvailable("広域バリア"))
            //                {
            //                    fname = t.FeatureName0("広域バリア");
            //                }
            //                else
            //                {
            //                    fname = "バリア";
            //                }
            //            }

            //            fdata = t.FeatureData(i);
            //            flevel = t.FeatureLevel(i);

            //            // 必要条件
            //            if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
            //            {
            //                ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
            //            }
            //            else
            //            {
            //                ecost = 10;
            //            }

            //            if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //            {
            //                nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //            }
            //            else
            //            {
            //                nmorale = 0;
            //            }

            //            // オプション
            //            neautralize = false;
            //            slevel = 0d;
            //            var loopTo10 = GeneralLib.LLength(fdata);
            //            for (j = 5; j <= loopTo10; j++)
            //            {
            //                opt = GeneralLib.LIndex(fdata, j);
            //                idx = Strings.InStr(opt, "*");
            //                if (idx > 0)
            //                {
            //                    lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
            //                    opt = Strings.Left(opt, idx - 1);
            //                }
            //                else
            //                {
            //                    lv_mod = -1;
            //                }

            //                switch (t.MainPilot().SkillType(opt) ?? "")
            //                {
            //                    case "相殺":
            //                        {
            //                            if (IsSameCategory(fdata, FeatureData("バリア")) && Math.Abs((x - tx)) + Math.Abs((y - ty)) == 1)
            //                            {
            //                                string localFeatureName() { object argIndex1 = i; var ret = t.FeatureName(argIndex1); return ret; }

            //                                msg = msg + Nickname + "は[" + localFeatureName() + "]を中和した。;";
            //                                neautralize = true;
            //                            }

            //                            break;
            //                        }

            //                    case "中和":
            //                        {
            //                            if (IsSameCategory(fdata, FeatureData("バリア")) && Math.Abs((x - tx)) + Math.Abs((y - ty)) == 1)
            //                            {
            //                                flevel = flevel - FeatureLevel("バリア");
            //                                if (flevel <= 0d)
            //                                {
            //                                    msg = msg + Nickname + "は[" + fname + "]を中和した。;";
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "近接無効":
            //                        {
            //                            if (w.IsWeaponClassifiedAs("武") || w.IsWeaponClassifiedAs("突") || w.IsWeaponClassifiedAs("接"))
            //                            {
            //                                neautralize = true;
            //                            }

            //                            break;
            //                        }

            //                    case "手動":
            //                        {
            //                            if (def_mode != "防御")
            //                            {
            //                                neautralize = true;
            //                            }

            //                            break;
            //                        }

            //                    case "能力必要":
            //                    case "バリア無効化無効":
            //                        {
            //                            break;
            //                        }
            //                    // スキップ
            //                    case "同調率":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 20d;
            //                            }

            //                            slevel = lv_mod * (t.SyncLevel() - 30d);
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == -30 * lv_mod)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }
            //                            else if (slevel == -30 * lv_mod)
            //                            {
            //                                slevel = 0d;
            //                            }

            //                            break;
            //                        }

            //                    case "霊力":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 10d;
            //                            }

            //                            slevel = lv_mod * t.PlanaLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "オーラ":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 200d;
            //                            }

            //                            slevel = lv_mod * t.AuraLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "超能力":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 200d;
            //                            }

            //                            slevel = lv_mod * t.PsychicLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 200d;
            //                            }

            //                            slevel = lv_mod * t.SkillLevel(opt);
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }
            //                }
            //            }

            //            // バリア無効化で無効化されている？
            //            if (t.IsConditionSatisfied("バリア無効化"))
            //            {
            //                if (Strings.InStr(fdata, "バリア無効化無効") == 0)
            //                {
            //                    neautralize = true;
            //                }
            //            }

            //            // 発動可能？
            //            bool localIsAttributeClassified3() { string argaclass1 = GeneralLib.LIndex(fdata, 2); string argaclass2 = w.WeaponClass(); var ret = t.IsAttributeClassified(argaclass1, argaclass2); return ret; }

            //            if (t.EN >= ecost && t.MainPilot().Morale >= nmorale && localIsAttributeClassified3() && !neautralize)
            //            {
            //                // バリア発動
            //                t.EN = t.EN - ecost;
            //                if (dmg <= 1000d * flevel + slevel)
            //                {
            //                    if (ecost != 0)
            //                    {
            //                        if (attack_mode != "反射")
            //                        {
            //                            GUI.UpdateMessageForm(this, t);
            //                        }
            //                        else
            //                        {
            //                            GUI.UpdateMessageForm(this, null);
            //                        }
            //                    }

            //                    Effect.NegateEffect(this, t, w, wname, dmg, fname, fdata, ecost, msg, be_quiet);
            //                    CheckDefenseFeatureRet = true;
            //                    return CheckDefenseFeatureRet;
            //                }
            //                else if (flevel > 0d || slevel > 0d)
            //                {
            //                    if (Strings.InStr(msg, "[" + fname + "]を貫いた") == 0)
            //                    {
            //                        is_penetrated = true;
            //                        msg = msg + wname + "が[" + fname + "]を貫いた。;";
            //                        if (t.IsAnimationDefined("バリア貫通", fname))
            //                        {
            //                            t.PlayAnimation("バリア貫通", fname);
            //                        }
            //                        else
            //                        {
            //                            t.SpecialEffect("バリア貫通", fname);
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // フィールド能力
            //    var loopTo11 = t.CountFeature();
            //    for (i = 1; i <= loopTo11; i++)
            //    {
            //        if (t.Feature(i) == "フィールド")
            //        {
            //            fname = t.FeatureName0(i);
            //            if (string.IsNullOrEmpty(fname))
            //            {
            //                if (t.IsFeatureAvailable("バリア"))
            //                {
            //                    fname = t.FeatureName("バリア");
            //                }
            //                else
            //                {
            //                    fname = "フィールド";
            //                }
            //            }

            //            fdata = t.FeatureData(i);
            //            flevel = t.FeatureLevel(i);

            //            // 必要条件
            //            if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
            //            {
            //                ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
            //            }
            //            else
            //            {
            //                ecost = 0;
            //            }

            //            if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //            {
            //                nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //            }
            //            else
            //            {
            //                nmorale = 0;
            //            }

            //            // オプション
            //            neautralize = false;
            //            slevel = 0d;
            //            var loopTo12 = GeneralLib.LLength(fdata);
            //            for (j = 5; j <= loopTo12; j++)
            //            {
            //                opt = GeneralLib.LIndex(fdata, j);
            //                idx = Strings.InStr(opt, "*");
            //                if (idx > 0)
            //                {
            //                    lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
            //                    opt = Strings.Left(opt, idx - 1);
            //                }
            //                else
            //                {
            //                    lv_mod = -1;
            //                }

            //                switch (t.MainPilot().SkillType(opt) ?? "")
            //                {
            //                    case "相殺":
            //                        {
            //                            if (IsSameCategory(fdata, FeatureData(argIndex54)) && Math.Abs((x - tx)) + Math.Abs((y - ty)) == 1)
            //                            {
            //                                msg = msg + Nickname + "は[" + fname + "]を中和した。;";
            //                                neautralize = true;
            //                            }

            //                            break;
            //                        }

            //                    case "中和":
            //                        {
            //                            if (IsSameCategory(fdata, FeatureData("フィールド")) && Math.Abs((x - tx)) + Math.Abs((y - ty)) == 1)
            //                            {
            //                                flevel = flevel - FeatureLevel("フィールド");
            //                                if (flevel <= 0d)
            //                                {
            //                                    msg = msg + Nickname + "は[" + fname + "]を中和した。;";
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "近接無効":
            //                        {
            //                            if (w.IsWeaponClassifiedAs("武") || w.IsWeaponClassifiedAs("突") || w.IsWeaponClassifiedAs("接"))
            //                            {
            //                                neautralize = true;
            //                            }

            //                            break;
            //                        }

            //                    case "手動":
            //                        {
            //                            if (def_mode != "防御")
            //                            {
            //                                neautralize = true;
            //                            }

            //                            break;
            //                        }

            //                    case "能力必要":
            //                    case "バリア無効化無効":
            //                        {
            //                            break;
            //                        }
            //                    // スキップ
            //                    case "同調率":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 20d;
            //                            }

            //                            slevel = lv_mod * (t.SyncLevel() - 30d);
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == -30 * lv_mod)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }
            //                            else if (slevel == -30 * lv_mod)
            //                            {
            //                                slevel = 0d;
            //                            }

            //                            break;
            //                        }

            //                    case "霊力":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 10d;
            //                            }

            //                            slevel = lv_mod * t.PlanaLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "オーラ":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 200d;
            //                            }

            //                            slevel = lv_mod * t.AuraLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "超能力":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 200d;
            //                            }

            //                            slevel = lv_mod * t.PsychicLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 200d;
            //                            }

            //                            slevel = lv_mod * t.SkillLevel(opt);
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }
            //                }
            //            }

            //            // バリア無効化で無効化されている？
            //            if (t.IsConditionSatisfied("バリア無効化"))
            //            {
            //                if (Strings.InStr(fdata, "バリア無効化無効") == 0)
            //                {
            //                    neautralize = true;
            //                }
            //            }

            //            // 発動可能？
            //            bool localIsAttributeClassified4() { string argaclass1 = GeneralLib.LIndex(fdata, 2); string argaclass2 = w.WeaponClass(); var ret = t.IsAttributeClassified(argaclass1, argaclass2); return ret; }

            //            if (t.EN >= ecost && t.MainPilot().Morale >= nmorale && localIsAttributeClassified4() && !neautralize)
            //            {
            //                // フィールド発動
            //                t.EN = t.EN - ecost;
            //                if (dmg <= 500d * flevel + slevel)
            //                {
            //                    if (ecost != 0)
            //                    {
            //                        if (attack_mode != "反射")
            //                        {
            //                            GUI.UpdateMessageForm(this, t);
            //                        }
            //                        else
            //                        {
            //                            GUI.UpdateMessageForm(this, null);
            //                        }
            //                    }

            //                    Effect.NegateEffect(this, t, w, wname, dmg, fname, fdata, ecost, msg, be_quiet);
            //                    CheckDefenseFeatureRet = true;
            //                    return CheckDefenseFeatureRet;
            //                }
            //                else if (flevel > 0d || slevel > 0d)
            //                {
            //                    dmg = (dmg - 500d * flevel - slevel);
            //                    if (Strings.InStr(msg, "[" + fname + "]を貫いた") == 0)
            //                    {
            //                        msg = msg + wname + "が[" + fname + "]を貫いた。;";
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // プロテクション能力
            //    var loopTo13 = t.CountFeature();
            //    for (i = 1; i <= loopTo13; i++)
            //    {
            //        if (t.Feature(i) == "プロテクション")
            //        {
            //            fname = t.FeatureName0(i);
            //            if (string.IsNullOrEmpty(fname))
            //            {
            //                if (t.IsFeatureAvailable("バリア"))
            //                {
            //                    fname = t.FeatureName("バリア");
            //                }
            //                else
            //                {
            //                    fname = "プロテクション";
            //                }
            //            }

            //            fdata = t.FeatureData(i);
            //            flevel = t.FeatureLevel(i);

            //            // 必要条件
            //            if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
            //            {
            //                ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
            //            }
            //            else
            //            {
            //                ecost = 10;
            //            }

            //            if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //            {
            //                nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //            }
            //            else
            //            {
            //                nmorale = 0;
            //            }

            //            // オプション
            //            neautralize = false;
            //            slevel = 0d;
            //            var loopTo14 = GeneralLib.LLength(fdata);
            //            for (j = 5; j <= loopTo14; j++)
            //            {
            //                opt = GeneralLib.LIndex(fdata, j);
            //                idx = Strings.InStr(opt, "*");
            //                if (idx > 0)
            //                {
            //                    lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
            //                    opt = Strings.Left(opt, idx - 1);
            //                }
            //                else
            //                {
            //                    lv_mod = -1;
            //                }

            //                switch (t.MainPilot().SkillType(opt) ?? "")
            //                {
            //                    case "相殺":
            //                        {
            //                            if (IsSameCategory(fdata, FeatureData("プロテクション")) && Math.Abs((x - tx)) + Math.Abs((y - ty)) == 1)
            //                            {
            //                                msg = msg + Nickname + "は[" + fname + "]を中和した。;";
            //                                neautralize = true;
            //                            }

            //                            break;
            //                        }

            //                    case "中和":
            //                        {
            //                            if (IsSameCategory(fdata, FeatureData(argIndex65)) && Math.Abs((x - tx)) + Math.Abs((y - ty)) == 1)
            //                            {
            //                                flevel = flevel - FeatureLevel("プロテクション");
            //                                if (flevel <= 0d)
            //                                {
            //                                    msg = msg + Nickname + "は[" + fname + "]を中和した。;";
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "近接無効":
            //                        {
            //                            if (w.IsWeaponClassifiedAs("武") || w.IsWeaponClassifiedAs("突") || w.IsWeaponClassifiedAs("接"))
            //                            {
            //                                neautralize = true;
            //                            }

            //                            break;
            //                        }

            //                    case "手動":
            //                        {
            //                            if (def_mode != "防御")
            //                            {
            //                                neautralize = true;
            //                            }

            //                            break;
            //                        }

            //                    case "能力必要":
            //                    case "バリア無効化無効":
            //                        {
            //                            break;
            //                        }
            //                    // スキップ
            //                    case "同調率":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 0.5d;
            //                            }

            //                            slevel = lv_mod * (t.SyncLevel() - 30d);
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == -30 * lv_mod)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }
            //                            else if (slevel == -30 * lv_mod)
            //                            {
            //                                slevel = 0d;
            //                            }

            //                            break;
            //                        }

            //                    case "霊力":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 0.2d;
            //                            }

            //                            slevel = lv_mod * t.PlanaLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "オーラ":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 5d;
            //                            }

            //                            slevel = lv_mod * t.AuraLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "超能力":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 5d;
            //                            }

            //                            slevel = lv_mod * t.PsychicLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 5d;
            //                            }

            //                            slevel = lv_mod * t.SkillLevel(opt);
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }
            //                }
            //            }

            //            // バリア無効化で無効化されている？
            //            if (t.IsConditionSatisfied("バリア無効化"))
            //            {
            //                if (Strings.InStr(fdata, "バリア無効化無効") == 0)
            //                {
            //                    neautralize = true;
            //                }
            //            }

            //            // 発動可能？
            //            bool localIsAttributeClassified5() { string argaclass1 = GeneralLib.LIndex(fdata, 2); string argaclass2 = w.WeaponClass(); var ret = t.IsAttributeClassified(argaclass1, argaclass2); return ret; }

            //            if (t.EN >= ecost && t.MainPilot().Morale >= nmorale && localIsAttributeClassified5() && !neautralize && dmg > 0)
            //            {
            //                // プロテクション発動
            //                dmg = ((long)(dmg * (100d - 10d * flevel - slevel)) / 100L);
            //                if (ecost != 0)
            //                {
            //                    t.EN = t.EN - ecost;
            //                    if (attack_mode != "反射")
            //                    {
            //                        GUI.UpdateMessageForm(this, t);
            //                    }
            //                    else
            //                    {
            //                        GUI.UpdateMessageForm(this, null);
            //                    }
            //                }

            //                if (dmg <= 0)
            //                {
            //                    Effect.NegateEffect(this, t, w, wname, dmg, fname, fdata, ecost, msg, be_quiet);
            //                    t.HP = t.HP - dmg;
            //                    GUI.UpdateMessageForm(this, t);
            //                    CheckDefenseFeatureRet = true;
            //                    return CheckDefenseFeatureRet;
            //                }
            //                else if (flevel > 0d || slevel > 0d)
            //                {
            //                    if (Strings.InStr(msg, "[" + fname + "]") == 0)
            //                    {
            //                        msg = msg + "[" + fname + "]がダメージを減少させた。;";
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // バリアシールド、アクティブフィールド、アクティブプロテクションは能動防御
            //    if (t.MaxAction() == 0 || t.IsUnderSpecialPowerEffect("無防備"))
            //    {
            //        goto SkipActiveBarrier;
            //    }

            //    // バリアシールド能力
            //    var loopTo15 = t.CountFeature();
            //    for (i = 1; i <= loopTo15; i++)
            //    {
            //        if (t.Feature(i) == "バリアシールド")
            //        {
            //            fname = t.FeatureName0(i);
            //            if (string.IsNullOrEmpty(fname))
            //            {
            //                if (t.IsFeatureAvailable("反射"))
            //                {
            //                    fname = t.FeatureName0("反射");
            //                }
            //                else
            //                {
            //                    fname = "バリアシールド";
            //                }
            //            }

            //            fdata = t.FeatureData(i);
            //            flevel = t.FeatureLevel(i);

            //            // 使用条件
            //            if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
            //            {
            //                ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
            //            }
            //            else
            //            {
            //                ecost = 10;
            //            }

            //            if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //            {
            //                nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //            }
            //            else
            //            {
            //                nmorale = 0;
            //            }

            //            // オプション
            //            neautralize = false;
            //            slevel = 0d;
            //            var loopTo16 = GeneralLib.LLength(fdata);
            //            for (j = 5; j <= loopTo16; j++)
            //            {
            //                opt = GeneralLib.LIndex(fdata, j);
            //                idx = Strings.InStr(opt, "*");
            //                if (idx > 0)
            //                {
            //                    lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
            //                    opt = Strings.Left(opt, idx - 1);
            //                }
            //                else
            //                {
            //                    lv_mod = -1;
            //                }

            //                switch (t.MainPilot().SkillType(opt) ?? "")
            //                {
            //                    case "相殺":
            //                        {
            //                            if (IsSameCategory(fdata, FeatureData("バリアシールド")) && Math.Abs((x - t.x)) + Math.Abs((y - t.y)) == 1)
            //                            {
            //                                string localFeatureName1() { object argIndex1 = i; var ret = t.FeatureName(argIndex1); return ret; }

            //                                msg = msg + Nickname + "は[" + localFeatureName1() + "]を中和した。;";
            //                                neautralize = true;
            //                            }

            //                            break;
            //                        }

            //                    case "中和":
            //                        {
            //                            if (IsSameCategory(fdata, FeatureData("バリアシールド")) && Math.Abs((x - tx)) + Math.Abs((y - ty)) == 1)
            //                            {
            //                                flevel = flevel - FeatureLevel("バリアシールド");
            //                                if (flevel <= 0d)
            //                                {
            //                                    msg = msg + Nickname + "は[" + fname + "]を中和した。;";
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "近接無効":
            //                        {
            //                            if (w.IsWeaponClassifiedAs("武") || w.IsWeaponClassifiedAs("突") || w.IsWeaponClassifiedAs("接"))
            //                            {
            //                                neautralize = true;
            //                            }

            //                            break;
            //                        }

            //                    case "手動":
            //                        {
            //                            if (def_mode != "防御")
            //                            {
            //                                neautralize = true;
            //                            }

            //                            break;
            //                        }

            //                    case "能力必要":
            //                    case "バリア無効化無効":
            //                        {
            //                            break;
            //                        }
            //                    // スキップ
            //                    case "同調率":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 20d;
            //                            }

            //                            slevel = lv_mod * (t.SyncLevel() - 30d);
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == -30 * lv_mod)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }
            //                            else if (slevel == -30 * lv_mod)
            //                            {
            //                                slevel = 0d;
            //                            }

            //                            break;
            //                        }

            //                    case "霊力":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 10d;
            //                            }

            //                            slevel = lv_mod * t.PlanaLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "オーラ":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 200d;
            //                            }

            //                            slevel = lv_mod * t.AuraLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "超能力":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 200d;
            //                            }

            //                            slevel = lv_mod * t.PsychicLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 200d;
            //                            }

            //                            slevel = lv_mod * t.SkillLevel(opt);
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }
            //                }
            //            }

            //            // バリア無効化で無効化されている？
            //            if (t.IsConditionSatisfied("バリア無効化"))
            //            {
            //                if (Strings.InStr(fdata, "バリア無効化無効") == 0)
            //                {
            //                    neautralize = true;
            //                }
            //            }

            //            // 発動可能？
            //            bool localIsAttributeClassified6() { string argaclass1 = GeneralLib.LIndex(fdata, 2); string argaclass2 = w.WeaponClass(); var ret = t.IsAttributeClassified(argaclass1, argaclass2); return ret; }

            //            if (t.EN >= ecost && t.MainPilot().Morale >= nmorale && localIsAttributeClassified6() && t.MainPilot().SkillLevel("Ｓ防御", ref_mode: "") > 0d && !neautralize)
            //            {
            //                // バリアシールド発動
            //                if (t.MainPilot().SkillLevel("Ｓ防御", ref_mode: "") >= GeneralLib.Dice(16) || t.IsUnderSpecialPowerEffect("特殊防御発動"))
            //                {
            //                    t.EN = t.EN - ecost;
            //                    if (dmg <= 1000d * flevel + slevel)
            //                    {
            //                        if (ecost != 0)
            //                        {
            //                            if (attack_mode != "反射")
            //                            {
            //                                GUI.UpdateMessageForm(this, t);
            //                            }
            //                            else
            //                            {
            //                                GUI.UpdateMessageForm(this, null);
            //                            }
            //                        }

            //                        Effect.NegateEffect(this, t, w, wname, dmg, fname, fdata, ecost, msg, be_quiet);
            //                        CheckDefenseFeatureRet = true;
            //                        return CheckDefenseFeatureRet;
            //                    }
            //                    else if (flevel > 0d || slevel > 0d)
            //                    {
            //                        if (Strings.InStr(msg, "[" + fname + "]を貫いた") == 0)
            //                        {
            //                            is_penetrated = true;
            //                            msg = msg + wname + "が[" + fname + "]を貫いた。;";
            //                            if (t.IsAnimationDefined("バリア貫通", fname))
            //                            {
            //                                t.PlayAnimation("バリア貫通", fname);
            //                            }
            //                            else
            //                            {
            //                                t.SpecialEffect("バリア貫通", fname);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // アクティブフィールド能力
            //    var loopTo17 = t.CountFeature();
            //    for (i = 1; i <= loopTo17; i++)
            //    {
            //        if (t.Feature(i) == "アクティブフィールド")
            //        {
            //            fname = t.FeatureName0(i);
            //            if (string.IsNullOrEmpty(fname))
            //            {
            //                if (t.IsFeatureAvailable("反射"))
            //                {
            //                    fname = t.FeatureName0("反射");
            //                }
            //                else
            //                {
            //                    fname = "アクティブフィールド";
            //                }
            //            }

            //            fdata = t.FeatureData(i);
            //            flevel = t.FeatureLevel(i);

            //            // 使用条件
            //            if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
            //            {
            //                ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
            //            }
            //            else
            //            {
            //                ecost = 0;
            //            }

            //            if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //            {
            //                nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //            }
            //            else
            //            {
            //                nmorale = 0;
            //            }

            //            // オプション
            //            neautralize = false;
            //            slevel = 0d;
            //            var loopTo18 = GeneralLib.LLength(fdata);
            //            for (j = 5; j <= loopTo18; j++)
            //            {
            //                opt = GeneralLib.LIndex(fdata, j);
            //                idx = Strings.InStr(opt, "*");
            //                if (idx > 0)
            //                {
            //                    lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
            //                    opt = Strings.Left(opt, idx - 1);
            //                }
            //                else
            //                {
            //                    lv_mod = -1;
            //                }

            //                switch (t.MainPilot().SkillType(opt) ?? "")
            //                {
            //                    case "相殺":
            //                        {
            //                            if (IsSameCategory(fdata, FeatureData("アクティブフィールド")) && Math.Abs((x - t.x)) + Math.Abs((y - t.y)) == 1)
            //                            {
            //                                string localFeatureName2() { object argIndex1 = i; var ret = t.FeatureName(argIndex1); return ret; }

            //                                msg = msg + Nickname + "は[" + localFeatureName2() + "]を中和した。;";
            //                                neautralize = true;
            //                            }

            //                            break;
            //                        }

            //                    case "中和":
            //                        {
            //                            if (IsSameCategory(fdata, FeatureData("アクティブフィールド")) && Math.Abs((x - tx)) + Math.Abs((y - ty)) == 1)
            //                            {
            //                                flevel = flevel - FeatureLevel("アクティブフィールド");
            //                                if (flevel <= 0d)
            //                                {
            //                                    msg = msg + Nickname + "は[" + fname + "]を中和した。;";
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "近接無効":
            //                        {
            //                            if (w.IsWeaponClassifiedAs("武") || w.IsWeaponClassifiedAs("突") || w.IsWeaponClassifiedAs("接"))
            //                            {
            //                                neautralize = true;
            //                            }

            //                            break;
            //                        }

            //                    case "手動":
            //                        {
            //                            if (def_mode != "防御")
            //                            {
            //                                neautralize = true;
            //                            }

            //                            break;
            //                        }

            //                    case "能力必要":
            //                    case "バリア無効化無効":
            //                        {
            //                            break;
            //                        }
            //                    // スキップ
            //                    case "同調率":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 20d;
            //                            }

            //                            slevel = lv_mod * (t.SyncLevel() - 30d);
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == -30 * lv_mod)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }
            //                            else if (slevel == -30 * lv_mod)
            //                            {
            //                                slevel = 0d;
            //                            }

            //                            break;
            //                        }

            //                    case "霊力":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 10d;
            //                            }

            //                            slevel = lv_mod * t.PlanaLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "オーラ":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 200d;
            //                            }

            //                            slevel = lv_mod * t.AuraLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "超能力":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 200d;
            //                            }

            //                            slevel = lv_mod * t.PsychicLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 200d;
            //                            }

            //                            slevel = lv_mod * t.SkillLevel(opt);
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }
            //                }
            //            }

            //            // バリア無効化で無効化されている？
            //            if (t.IsConditionSatisfied("バリア無効化"))
            //            {
            //                if (Strings.InStr(fdata, "バリア無効化無効") == 0)
            //                {
            //                    neautralize = true;
            //                }
            //            }

            //            // 発動可能？
            //            bool localIsAttributeClassified7() { string argaclass1 = GeneralLib.LIndex(fdata, 2); string argaclass2 = w.WeaponClass(); var ret = t.IsAttributeClassified(argaclass1, argaclass2); return ret; }

            //            if (t.EN >= ecost && t.MainPilot().Morale >= nmorale && localIsAttributeClassified7() && t.MainPilot().SkillLevel("Ｓ防御", ref_mode: "") > 0d && !neautralize)
            //            {
            //                // アクティブフィールド発動
            //                if (t.MainPilot().SkillLevel("Ｓ防御", ref_mode: "") >= GeneralLib.Dice(16) || t.IsUnderSpecialPowerEffect("特殊防御発動"))
            //                {
            //                    t.EN = t.EN - ecost;
            //                    if (dmg <= 500d * flevel + slevel)
            //                    {
            //                        if (ecost != 0)
            //                        {
            //                            if (attack_mode != "反射")
            //                            {
            //                                GUI.UpdateMessageForm(this, t);
            //                            }
            //                            else
            //                            {
            //                                GUI.UpdateMessageForm(this, null);
            //                            }
            //                        }

            //                        Effect.NegateEffect(this, t, w, wname, dmg, fname, fdata, ecost, msg, be_quiet);
            //                        CheckDefenseFeatureRet = true;
            //                        return CheckDefenseFeatureRet;
            //                    }
            //                    else if (flevel > 0d || slevel > 0d)
            //                    {
            //                        dmg = (dmg - 500d * flevel - slevel);
            //                        if (Strings.InStr(msg, "[" + fname + "]を貫いた") == 0)
            //                        {
            //                            msg = msg + wname + "が[" + fname + "]を貫いた。;";
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // アクティブプロテクション能力
            //    var loopTo19 = t.CountFeature();
            //    for (i = 1; i <= loopTo19; i++)
            //    {
            //        if (t.Feature(i) == "アクティブプロテクション")
            //        {
            //            fname = t.FeatureName0(i);
            //            if (string.IsNullOrEmpty(fname))
            //            {
            //                if (t.IsFeatureAvailable("反射"))
            //                {
            //                    fname = t.FeatureName0("反射");
            //                }
            //                else
            //                {
            //                    fname = "アクティブプロテクション";
            //                }
            //            }

            //            fdata = t.FeatureData(i);
            //            flevel = t.FeatureLevel(i);

            //            // 使用条件
            //            if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
            //            {
            //                ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
            //            }
            //            else
            //            {
            //                ecost = 10;
            //            }

            //            if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //            {
            //                nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //            }
            //            else
            //            {
            //                nmorale = 0;
            //            }

            //            // オプション
            //            neautralize = false;
            //            slevel = 0d;
            //            var loopTo20 = GeneralLib.LLength(fdata);
            //            for (j = 5; j <= loopTo20; j++)
            //            {
            //                opt = GeneralLib.LIndex(fdata, j);
            //                idx = Strings.InStr(opt, "*");
            //                if (idx > 0)
            //                {
            //                    lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
            //                    opt = Strings.Left(opt, idx - 1);
            //                }
            //                else
            //                {
            //                    lv_mod = -1;
            //                }

            //                switch (t.MainPilot().SkillType(opt) ?? "")
            //                {
            //                    case "相殺":
            //                        {
            //                            if (IsSameCategory(fdata, FeatureData("アクティブプロテクション")) && Math.Abs((x - t.x)) + Math.Abs((y - t.y)) == 1)
            //                            {
            //                                string localFeatureName3() { object argIndex1 = i; var ret = t.FeatureName(argIndex1); return ret; }

            //                                msg = msg + Nickname + "は[" + localFeatureName3() + "]を中和した。;";
            //                                neautralize = true;
            //                            }

            //                            break;
            //                        }

            //                    case "中和":
            //                        {
            //                            if (IsSameCategory(fdata, FeatureData("アクティブプロテクション")) && Math.Abs((x - tx)) + Math.Abs((y - ty)) == 1)
            //                            {
            //                                flevel = flevel - FeatureLevel("アクティブプロテクション");
            //                                if (flevel <= 0d)
            //                                {
            //                                    msg = msg + Nickname + "は[" + fname + "]を中和した。;";
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "近接無効":
            //                        {
            //                            if (w.IsWeaponClassifiedAs("武") || w.IsWeaponClassifiedAs("突") || w.IsWeaponClassifiedAs("接"))
            //                            {
            //                                neautralize = true;
            //                            }

            //                            break;
            //                        }

            //                    case "手動":
            //                        {
            //                            if (def_mode != "防御")
            //                            {
            //                                neautralize = true;
            //                            }

            //                            break;
            //                        }

            //                    case "能力必要":
            //                    case "バリア無効化無効":
            //                        {
            //                            break;
            //                        }
            //                    // スキップ
            //                    case "同調率":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 0.5d;
            //                            }

            //                            slevel = lv_mod * (t.SyncLevel() - 30d);
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == -30 * lv_mod)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }
            //                            else if (slevel == -30 * lv_mod)
            //                            {
            //                                slevel = 0d;
            //                            }

            //                            break;
            //                        }

            //                    case "霊力":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 0.2d;
            //                            }

            //                            slevel = lv_mod * t.PlanaLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "オーラ":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 5d;
            //                            }

            //                            slevel = lv_mod * t.AuraLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "超能力":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 5d;
            //                            }

            //                            slevel = lv_mod * t.PsychicLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 5d;
            //                            }

            //                            slevel = lv_mod * t.SkillLevel(opt);
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }
            //                }
            //            }

            //            // バリア無効化で無効化されている？
            //            if (t.IsConditionSatisfied("バリア無効化"))
            //            {
            //                if (Strings.InStr(fdata, "バリア無効化無効") == 0)
            //                {
            //                    neautralize = true;
            //                }
            //            }

            //            // 発動可能？
            //            bool localIsAttributeClassified8() { string argaclass1 = GeneralLib.LIndex(fdata, 2); string argaclass2 = w.WeaponClass(); var ret = t.IsAttributeClassified(argaclass1, argaclass2); return ret; }

            //            if (t.EN >= ecost && t.MainPilot().Morale >= nmorale && localIsAttributeClassified8() && t.MainPilot().SkillLevel("Ｓ防御", ref_mode: "") > 0d && !neautralize && dmg > 0)
            //            {
            //                // アクティブプロテクション発動
            //                if (t.MainPilot().SkillLevel("Ｓ防御", ref_mode: "") >= GeneralLib.Dice(16) || t.IsUnderSpecialPowerEffect("特殊防御発動"))
            //                {
            //                    dmg = ((long)(dmg * (100d - 10d * flevel - slevel)) / 100L);
            //                    if (ecost != 0)
            //                    {
            //                        t.EN = t.EN - ecost;
            //                        if (attack_mode != "反射")
            //                        {
            //                            GUI.UpdateMessageForm(this, t);
            //                        }
            //                        else
            //                        {
            //                            GUI.UpdateMessageForm(this, null);
            //                        }
            //                    }

            //                    if (dmg <= 0)
            //                    {
            //                        Effect.NegateEffect(this, t, w, wname, dmg, fname, fdata, ecost, msg, be_quiet);
            //                        t.HP = t.HP - dmg;
            //                        GUI.UpdateMessageForm(this, t);
            //                        CheckDefenseFeatureRet = true;
            //                        return CheckDefenseFeatureRet;
            //                    }
            //                    else if (flevel > 0d || slevel > 0d)
            //                    {
            //                        if (Strings.InStr(msg, "[" + fname + "]") == 0)
            //                        {
            //                            msg = msg + "[" + fname + "]がダメージを減少させた。;";
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //SkipActiveBarrier:
            //    ;


            //    // 相手の攻撃をＥＮに変換
            //    var loopTo21 = t.CountFeature();
            //    for (i = 1; i <= loopTo21; i++)
            //    {
            //        if (t.Feature(i) == "変換")
            //        {
            //            fdata = t.FeatureData(i);
            //            flevel = t.FeatureLevel(i);

            //            // 必要気力
            //            if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
            //            {
            //                nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
            //            }
            //            else
            //            {
            //                nmorale = 0;
            //            }

            //            // 発動可能？
            //            bool localIsAttributeClassified9() { string argaclass1 = GeneralLib.LIndex(fdata, 2); string argaclass2 = w.WeaponClass(); var ret = t.IsAttributeClassified(argaclass1, argaclass2); return ret; }

            //            if (t.MainPilot().Morale >= nmorale && localIsAttributeClassified9())
            //            {
            //                t.EN = (t.EN + 0.01d * flevel * dmg);
            //            }
            //        }
            //    }

            //    // 対ビーム用防御能力
            //    if (w.IsWeaponClassifiedAs("Ｂ"))
            //    {
            //        // ビーム吸収
            //        if (t.IsFeatureAvailable("ビーム吸収"))
            //        {
            //            fname = t.FeatureName("ビーム吸収");
            //            t.HP = t.HP + dmg;
            //            Effect.NegateEffect(this, t, w, wname, dmg, fname, "Ｂ", 0, msg, be_quiet);
            //            CheckDefenseFeatureRet = true;
            //            return CheckDefenseFeatureRet;
            //        }
            //    }

            //SkipBarrier:
            //    ;


            //    // 攻撃力が0の場合は盾や融合を無視
            //    if (this.Weapon(w).Power == 0)
            //    {
            //        return CheckDefenseFeatureRet;
            //    }

            //    // 盾防御
            //    if (t.IsFeatureAvailable("盾") && t.MainPilot().IsSkillAvailable("Ｓ防御") && t.MaxAction() > 0 && !w.IsWeaponClassifiedAs("精") && !w.IsWeaponClassifiedAs("浸") && !w.IsWeaponClassifiedAs("殺") && !IsUnderSpecialPowerEffect("シールド防御無効化") && !t.IsUnderSpecialPowerEffect("無防備") && (t.IsConditionSatisfied("盾付加") || t.FeatureLevel("盾") > t.ConditionLevel("盾ダメージ")))
            //    {
            //        fname = t.FeatureName0("盾");
            //        if (!be_quiet)
            //        {
            //            t.PilotMessage("シールド防御", fname);
            //        }

            //        if (t.IsAnimationDefined("シールド防御", fname))
            //        {
            //            t.PlayAnimation("シールド防御", fname);
            //        }
            //        else if (t.IsSpecialEffectDefined("シールド防御", fname))
            //        {
            //            t.SpecialEffect("シールド防御", fname);
            //        }
            //        else
            //        {
            //            Effect.ShowAnimation("ミドルシールド発動");
            //        }

            //        if (w.IsWeaponClassifiedAs("破"))
            //        {
            //            dmg = GeneralLib.MaxLng((dmg - 50d * (t.MainPilot().SkillLevel("Ｓ防御", ref_mode: "") + 4d)), 0);
            //        }
            //        else
            //        {
            //            dmg = GeneralLib.MaxLng((dmg - 100d * (t.MainPilot().SkillLevel("Ｓ防御", ref_mode: "") + 4d)), 0);
            //        }

            //        if (t.IsSysMessageDefined("シールド防御", fname))
            //        {
            //            t.SysMessage("シールド防御", fname, add_msg: "");
            //        }
            //        else if (dmg == 0)
            //        {
            //            GUI.DisplaySysMessage(t.Nickname + "は[" + fname + "]を使って攻撃を防いだ。");
            //        }
            //        else
            //        {
            //            GUI.DisplaySysMessage(t.Nickname + "は[" + fname + "]を使ってダメージを軽減させた。");
            //        }

            //        if (dmg == 0)
            //        {
            //            // 攻撃を盾で完全に防いだ場合

            //            // 命中時の特殊効果
            //            Sound.IsWavePlayed = false;
            //            if (!be_quiet)
            //            {
            //                PilotMessage(wname + "(命中)", msg_mode: "");
            //            }

            //            bool localIsSpecialEffectDefined() { string argmain_situation = wname + "(命中)"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //            if (IsAnimationDefined(wname + "(命中)", sub_situation: "") || IsAnimationDefined(wname, sub_situation: ""))
            //            {
            //                PlayAnimation(wname + "(命中)", sub_situation: "");
            //            }
            //            else if (localIsSpecialEffectDefined())
            //            {
            //                SpecialEffect(wname + "(命中)", sub_situation: "");
            //            }
            //            else if (!Sound.IsWavePlayed)
            //            {
            //                Effect.HitEffect(this, w, t);
            //            }

            //            CheckDefenseFeatureRet = true;
            //            return CheckDefenseFeatureRet;
            //        }
            //        else
            //        {
            //            // 攻撃が盾を貫通した場合
            //            if (t.IsConditionSatisfied("盾付加"))
            //            {
            //                if (w.IsWeaponClassifiedAs("破"))
            //                {
            //                    t.SetConditionLevel("盾付加", t.ConditionLevel("盾付加") - 2d);
            //                }
            //                else
            //                {
            //                    t.SetConditionLevel("盾付加", t.ConditionLevel("盾付加") - 1d);
            //                }

            //                if (t.ConditionLevel("盾付加") <= 0d)
            //                {
            //                    t.DeleteCondition("盾付加");
            //                }
            //            }
            //            else
            //            {
            //                if (w.IsWeaponClassifiedAs("破"))
            //                {
            //                    if (t.IsConditionSatisfied("盾ダメージ"))
            //                    {
            //                        t.SetConditionLevel("盾ダメージ", t.ConditionLevel("盾ダメージ") + 2d);
            //                    }
            //                    else
            //                    {
            //                        t.AddCondition("盾ダメージ", -1, 2d, cdata: "");
            //                    }
            //                }
            //                else
            //                {
            //                    if (t.IsConditionSatisfied("盾ダメージ"))
            //                    {
            //                        t.SetConditionLevel("盾ダメージ", t.ConditionLevel("盾ダメージ") + 1d);
            //                    }
            //                    else
            //                    {
            //                        t.AddCondition("盾ダメージ", -1, 1d, cdata: "");
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // 融合能力
            //    if (t.IsFeatureAvailable("融合"))
            //    {
            //        // 融合可能？
            //        if (!w.IsWeaponClassifiedAs("武") && !w.IsWeaponClassifiedAs("突") && !w.IsWeaponClassifiedAs("接") && (t.FeatureLevel("融合") >= GeneralLib.Dice(16) || t.IsUnderSpecialPowerEffect("特殊防御発動")))
            //        {
            //            // 融合発動
            //            t.HP = t.HP + dmg;
            //            if (attack_mode != "反射")
            //            {
            //                GUI.UpdateMessageForm(this, t);
            //            }
            //            else
            //            {
            //                GUI.UpdateMessageForm(this, null);
            //            }

            //            fname = t.FeatureName("融合");
            //            if (!be_quiet)
            //            {
            //                if (t.IsMessageDefined("攻撃無効化(" + fname + ")"))
            //                {
            //                    t.PilotMessage("攻撃無効化(" + fname + ")", msg_mode: "");
            //                }
            //                else
            //                {
            //                    t.PilotMessage("攻撃無効化", msg_mode: "");
            //                }
            //            }

            //            if (t.IsAnimationDefined("攻撃無効化", fname))
            //            {
            //                t.PlayAnimation("攻撃無効化", fname);
            //            }
            //            else if (t.IsSpecialEffectDefined("攻撃無効化", fname))
            //            {
            //                t.SpecialEffect("攻撃無効化", fname);
            //            }
            //            else
            //            {
            //                Effect.AbsorbEffect(this, w, t);
            //            }

            //            bool localIsSpecialEffectDefined1() { string argmain_situation = wname + "(攻撃無効化)"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //            if (IsAnimationDefined(wname + "(攻撃無効化)", sub_situation: ""))
            //            {
            //                PlayAnimation(wname + "(攻撃無効化)", sub_situation: "");
            //            }
            //            else if (localIsSpecialEffectDefined1())
            //            {
            //                SpecialEffect(wname + "(攻撃無効化)", sub_situation: "");
            //            }

            //            if (t.IsSysMessageDefined("攻撃無効化", fname))
            //            {
            //                t.SysMessage("攻撃無効化", fname, add_msg: "");
            //            }
            //            else
            //            {
            //                if (w.IsWeaponClassifiedAs("実"))
            //                {
            //                    GUI.DisplaySysMessage(msg + t.Nickname + "は[" + wname + "]を取り込んだ。");
            //                }
            //                else
            //                {
            //                    GUI.DisplaySysMessage(msg + t.Nickname + "は[" + wname + "]の攻撃を吸収した。");
            //                }
            //            }

            //            CheckDefenseFeatureRet = true;
            //            return CheckDefenseFeatureRet;
            //        }
            //    }

            //    return CheckDefenseFeatureRet;
        }

        // 自動反撃のチェック
        public void CheckAutoAttack(UnitWeapon w, Unit t, string attack_mode, string def_mode, int dmg, bool be_quiet)
        {
            // TODO Impl
            return;
            //string wname2;
            //int w2;
            //int ecost, nmorale;
            //string fname, fdata;
            //double flevel;
            //double slevel, lv_mod;
            //string opt;
            //int j, i, idx;
            //int prob;
            //string buf;

            //// 反撃系の攻撃に対しては自動反撃を行わない
            //if (attack_mode == "自動反撃" || attack_mode == "反射" || attack_mode == "当て身技")
            //{
            //    return;
            //}

            //// マップ攻撃、間接攻撃、無属性武器には自動反撃出来ない
            //if (w.IsWeaponClassifiedAs("Ｍ") || w.IsWeaponClassifiedAs("間") || w.IsWeaponClassifiedAs("無") || IsUnderSpecialPowerEffect("防御能力無効化"))
            //{
            //    return;
            //}

            //// 自動反撃の結果形態が変化して特殊能力数が変わることがあるのでFor文は使わない
            //i = 1;
            //while (i <= t.CountFeature())
            //{
            //    if (t.Feature(i) == "自動反撃")
            //    {
            //        fname = t.FeatureName0(i);
            //        if (string.IsNullOrEmpty(fname))
            //        {
            //            fname = "自動反撃";
            //        }

            //        fdata = t.FeatureData(i);
            //        flevel = t.FeatureLevel(i);
            //        if (flevel == 1d)
            //        {
            //            flevel = 10000d;
            //        }

            //        // 自動反撃確率の設定
            //        buf = GeneralLib.LIndex(fdata, 4);
            //        if (Information.IsNumeric(buf))
            //        {
            //            prob = Conversions.Toint(buf);
            //        }
            //        else if (Strings.InStr(buf, "+") > 0 || Strings.InStr(buf, "-") > 0)
            //        {
            //            j = GeneralLib.MaxLng(Strings.InStr(buf, "+"), Strings.InStr(buf, "-"));
            //            prob = (100d * (t.SkillLevel(Strings.Left(buf, j - 1)) + Conversions.Toint(Strings.Mid(buf, j))) / 16d);
            //        }
            //        else
            //        {
            //            prob = (100d * t.SkillLevel(buf) / 16d);
            //        }

            //        // 見切り
            //        if (t.IsUnderSpecialPowerEffect("特殊防御発動"))
            //        {
            //            prob = 100;
            //        }

            //        // 対象属性の判定
            //        bool localIsAttributeClassified() { string argaclass1 = GeneralLib.LIndex(fdata, 3); string argaclass2 = w.WeaponClass(); var ret = t.IsAttributeClassified(argaclass1, argaclass2); return ret; }

            //        if (!localIsAttributeClassified())
            //        {
            //            prob = 0;
            //        }

            //        // 使用条件
            //        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
            //        {
            //            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 5));
            //        }
            //        else
            //        {
            //            ecost = 0;
            //        }

            //        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 6)))
            //        {
            //            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 6));
            //        }
            //        else
            //        {
            //            nmorale = 0;
            //        }

            //        if (t.EN < ecost || t.MainPilot().Morale < nmorale)
            //        {
            //            prob = 0;
            //        }

            //        // 能動防御は行動できなければ発動しない
            //        if (t.MaxAction() == 0)
            //        {
            //            if (Strings.InStr(fdata, "完全自動") == 0)
            //            {
            //                prob = 0;
            //            }
            //        }

            //        // オプション
            //        slevel = 0d;
            //        var loopTo = GeneralLib.LLength(fdata);
            //        for (j = 7; j <= loopTo; j++)
            //        {
            //            if (prob == 0)
            //            {
            //                break;
            //            }

            //            opt = GeneralLib.LIndex(fdata, j);
            //            idx = Strings.InStr(opt, "*");
            //            if (idx > 0)
            //            {
            //                lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
            //                opt = Strings.Left(opt, idx - 1);
            //            }
            //            else
            //            {
            //                lv_mod = -1;
            //            }

            //            switch (t.MainPilot().SkillType(opt) ?? "")
            //            {
            //                case "相殺":
            //                    {
            //                        if (IsSameCategory(fdata, FeatureData("自動反撃")) && Math.Abs((x - t.x)) + Math.Abs((y - t.y)) == 1)
            //                        {
            //                            prob = 0;
            //                        }

            //                        break;
            //                    }

            //                case "中和":
            //                    {
            //                        if (IsSameCategory(fdata, FeatureData("自動反撃")) && Math.Abs((x - t.x)) + Math.Abs((y - t.y)) == 1)
            //                        {
            //                            flevel = flevel - FeatureLevel("自動反撃");
            //                            if (flevel <= 0d)
            //                            {
            //                                prob = 0;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "近接無効":
            //                    {
            //                        if (w.IsWeaponClassifiedAs("武") || w.IsWeaponClassifiedAs("突") || w.IsWeaponClassifiedAs("接"))
            //                        {
            //                            prob = 0;
            //                        }

            //                        break;
            //                    }

            //                case "手動":
            //                    {
            //                        if (def_mode != "防御")
            //                        {
            //                            prob = 0;
            //                        }

            //                        break;
            //                    }

            //                case "能力必要":
            //                    {
            //                        break;
            //                    }
            //                // スキップ
            //                case "同調率":
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 20d;
            //                        }

            //                        slevel = lv_mod * (t.SyncLevel() - 30d);
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == -30 * lv_mod)
            //                            {
            //                                prob = 0;
            //                            }
            //                        }
            //                        else if (slevel == -30 * lv_mod)
            //                        {
            //                            slevel = 0d;
            //                        }

            //                        break;
            //                    }

            //                case "霊力":
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 10d;
            //                        }

            //                        slevel = lv_mod * t.PlanaLevel();
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == 0d)
            //                            {
            //                                prob = 0;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "オーラ":
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 200d;
            //                        }

            //                        slevel = lv_mod * t.AuraLevel();
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == 0d)
            //                            {
            //                                prob = 0;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "超能力":
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 200d;
            //                        }

            //                        slevel = lv_mod * t.PsychicLevel();
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == 0d)
            //                            {
            //                                prob = 0;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                default:
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 200d;
            //                        }

            //                        slevel = lv_mod * t.SkillLevel(opt);
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == 0d)
            //                            {
            //                                prob = 0;
            //                            }
            //                        }

            //                        break;
            //                    }
            //            }
            //        }

            //        // ダメージが許容範囲外であれば自動反撃を使えない
            //        if (dmg > 500d * flevel + slevel)
            //        {
            //            prob = 0;
            //        }

            //        // 使用する武器を検索
            //        wname2 = GeneralLib.LIndex(fdata, 2);
            //        w2 = 0;
            //        var loopTo1 = t.CountWeapon();
            //        for (j = 1; j <= loopTo1; j++)
            //        {
            //            if ((t.Weapon(j).Name ?? "") == (wname2 ?? ""))
            //            {
            //                if (t.IsWeaponAvailable(j, "必要技能無視"))
            //                {
            //                    if (w.IsWeaponClassifiedAs("武") || w.IsWeaponClassifiedAs("突") || w.IsWeaponClassifiedAs("接"))
            //                    {
            //                        w2 = j;
            //                    }
            //                    else
            //                    {
            //                        if (t.IsTargetWithinRange(j, this))
            //                        {
            //                            w2 = j;
            //                        }
            //                    }
            //                }

            //                break;
            //            }
            //        }

            //        // 自動反撃発動
            //        if (prob >= GeneralLib.Dice(100) && w2 > 0)
            //        {
            //            if (ecost != 0)
            //            {
            //                t.EN = t.EN - ecost;
            //                GUI.UpdateMessageForm(this, t);
            //            }

            //            // メッセージ
            //            if (!be_quiet)
            //            {
            //                if (t.IsMessageDefined("自動反撃(" + fname + ")"))
            //                {
            //                    t.PilotMessage("自動反撃(" + fname + ")", msg_mode: "");
            //                }
            //                else
            //                {
            //                    t.PilotMessage("自動反撃", msg_mode: "");
            //                }
            //            }
            //            else
            //            {
            //                Sound.IsWavePlayed = false;
            //            }

            //            // 効果音
            //            if (!Sound.IsWavePlayed)
            //            {
            //                if (t.IsAnimationDefined("自動反撃", fname))
            //                {
            //                    t.PlayAnimation("自動反撃", fname);
            //                }
            //                else if (t.IsSpecialEffectDefined("自動反撃", fname))
            //                {
            //                    t.SpecialEffect("自動反撃", fname);
            //                }
            //            }

            //            if (t.IsSysMessageDefined("自動反撃", fname))
            //            {
            //                t.SysMessage("自動反撃", fname, add_msg: "");
            //            }
            //            else
            //            {
            //                GUI.DisplaySysMessage(t.Nickname + "は" + t.WeaponNickname(w2) + "で反撃した。");
            //            }

            //            // 自動反撃で攻撃をかける
            //            t.Attack(w2, this, "自動反撃", "");
            //            t = t.CurrentForm();
            //            if (Status != "出撃" || t.Status != "出撃")
            //            {
            //                return;
            //            }
            //        }
            //    }

            //    i = (i + 1);
            //}
        }

        // 追加攻撃のチェック
        public void CheckAdditionalAttack(UnitWeapon w, Unit t, bool be_quiet, string attack_mode, string def_mode, int dmg)
        {
            // TODO Impl
            return;
            //string wnskill, wname, wnickname, wclass;
            //string wtype, sname;
            //string wname2;
            //int w2;
            //int ecost, nmorale;
            //string fname, fdata;
            //double flevel;
            //int i, j;
            //int prob;
            //string buf;
            //bool found;
            //var attack_count = default;
            //wname = Weapon(w).Name;
            //wnickname = WeaponNickname(w);
            //wclass = w.WeaponClass();
            //wnskill = Weapon(w).NecessarySkill;

            //// 追加攻撃の結果形態が変化して特殊能力数が変わることがあるのでFor文は使わない
            //i = 1;
            //while (i <= CountFeature())
            //{
            //    if (Feature(i) == "追加攻撃")
            //    {
            //        fname = FeatureName0(i);
            //        if (string.IsNullOrEmpty(fname))
            //        {
            //            fname = "追加攻撃";
            //        }

            //        fdata = FeatureData(i);
            //        flevel = FeatureLevel(i);
            //        if (flevel == 1d)
            //        {
            //            flevel = 10000d;
            //        }

            //        // 追加攻撃確率の設定
            //        buf = GeneralLib.LIndex(fdata, 4);
            //        if (Information.IsNumeric(buf))
            //        {
            //            prob = Conversions.Toint(buf);
            //        }
            //        else if (Strings.InStr(buf, "+") > 0 || Strings.InStr(buf, "-") > 0)
            //        {
            //            j = GeneralLib.MaxLng(Strings.InStr(buf, "+"), Strings.InStr(buf, "-"));
            //            prob = (100d * (SkillLevel(Strings.Left(buf, j - 1)) + Conversions.Toint(Strings.Mid(buf, j))) / 16d);
            //        }
            //        else
            //        {
            //            prob = (SkillLevel(buf) * 100d / 16d);
            //        }

            //        // 対象武器の判定
            //        wtype = GeneralLib.LIndex(fdata, 3);
            //        found = false;
            //        if (Strings.Left(wtype, 1) == "@")
            //        {
            //            // 武器名または必要技能による指定
            //            wtype = Strings.Mid(wtype, 2);
            //            if ((wname ?? "") == (wtype ?? "") || (wnickname ?? "") == (wtype ?? ""))
            //            {
            //                found = true;
            //            }
            //            else
            //            {
            //                var loopTo = GeneralLib.LLength(wnskill);
            //                for (j = 1; j <= loopTo; j++)
            //                {
            //                    sname = GeneralLib.LIndex(wnskill, j);
            //                    if (Strings.InStr(sname, "Lv") > 0)
            //                    {
            //                        sname = Strings.Left(sname, Strings.InStr(sname, "Lv") - 1);
            //                    }

            //                    if ((sname ?? "") == (wtype ?? ""))
            //                    {
            //                        found = true;
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            // 属性による指定
            //            switch (wtype ?? "")
            //            {
            //                case "全":
            //                    {
            //                        found = true;
            //                        break;
            //                    }

            //                case "物":
            //                    {
            //                        if (GeneralLib.InStrNotNest(wclass, "魔") == 0 || GeneralLib.InStrNotNest(wclass, "魔武") > 0 || GeneralLib.InStrNotNest(wclass, "魔突") > 0 || GeneralLib.InStrNotNest(wclass, "魔接") > 0 || GeneralLib.InStrNotNest(wclass, "魔銃") > 0 || GeneralLib.InStrNotNest(wclass, "魔実") > 0)
            //                        {
            //                            found = true;
            //                        }

            //                        break;
            //                    }

            //                default:
            //                    {
            //                        if (IsAttributeClassified(wtype, wclass))
            //                        {
            //                            found = true;
            //                        }

            //                        break;
            //                    }
            //            }
            //        }

            //        if (!found)
            //        {
            //            prob = 0;
            //        }

            //        // 使用条件
            //        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
            //        {
            //            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 5));
            //        }
            //        else
            //        {
            //            ecost = 0;
            //        }

            //        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 6)))
            //        {
            //            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 6));
            //        }
            //        else
            //        {
            //            nmorale = 0;
            //        }

            //        if (EN < ecost || this.MainPilot().Morale < nmorale)
            //        {
            //            prob = 0;
            //        }

            //        // 連鎖不可
            //        if (Strings.InStr(fdata, "連鎖不可") > 0)
            //        {
            //            if (attack_count > 0 || attack_mode == "追加攻撃")
            //            {
            //                prob = 0;
            //            }
            //        }

            //        // 命中時限定
            //        if (Strings.InStr(fdata, "命中時限定") > 0)
            //        {
            //            if (dmg <= 0)
            //            {
            //                prob = 0;
            //            }
            //        }

            //        // 使用する武器を検索
            //        wname2 = GeneralLib.LIndex(fdata, 2);
            //        w2 = 0;
            //        var loopTo1 = CountWeapon();
            //        for (j = 1; j <= loopTo1; j++)
            //        {
            //            if ((Weapon(j).Name ?? "") == (wname2 ?? ""))
            //            {
            //                if (IsWeaponAvailable(j, "必要技能無視"))
            //                {
            //                    if (IsTargetWithinRange(j, t))
            //                    {
            //                        w2 = j;
            //                        break;
            //                    }
            //                }
            //            }
            //        }

            //        // 追加攻撃反撃発動
            //        if (prob >= GeneralLib.Dice(100) && w2 > 0)
            //        {
            //            if (ecost != 0)
            //            {
            //                EN = EN - ecost;
            //                GUI.UpdateMessageForm(this, t);
            //            }

            //            // メッセージ
            //            if (!be_quiet)
            //            {
            //                if (IsMessageDefined("追加攻撃(" + fname + ")"))
            //                {
            //                    PilotMessage("追加攻撃(" + fname + ")", msg_mode: "");
            //                }
            //                else
            //                {
            //                    PilotMessage("追加攻撃", msg_mode: "");
            //                }
            //            }

            //            // 効果音
            //            if (IsAnimationDefined("追加攻撃", fname))
            //            {
            //                PlayAnimation("追加攻撃", fname);
            //            }
            //            else if (IsSpecialEffectDefined("追加攻撃", fname))
            //            {
            //                SpecialEffect("追加攻撃", fname);
            //            }

            //            if (IsSysMessageDefined("追加攻撃", fname))
            //            {
            //                SysMessage("追加攻撃", fname, add_msg: "");
            //            }
            //            else
            //            {
            //                GUI.DisplaySysMessage(Nickname + "はさらに[" + WeaponNickname(w2) + "]で攻撃を加えた。");
            //            }

            //            // 追加攻撃をかける
            //            Attack(w2, t, "追加攻撃", def_mode);
            //            t = t.CurrentForm();
            //            if (Status != "出撃" || t.Status != "出撃")
            //            {
            //                return;
            //            }

            //            // 追加攻撃を実施したことを記録
            //            attack_count = (attack_count + 1);
            //        }
            //    }

            //    i = (i + 1);
            //}
        }

        // クリティカルによる特殊効果
        public bool CauseEffect(UnitWeapon w, Unit t, out string msg, out string critical_type, string def_mode, bool will_die)
        {
            msg = "";
            critical_type = "";
            var wname = w.WeaponNickname();

            // 特殊効果発生確率
            int prob;
            if (IsUnderSpecialPowerEffect("特殊効果発動"))
            {
                prob = 100;
            }
            else
            {
                prob = w.CriticalProbability(t, def_mode);
            }

            bool CauseEffectRet = false;

            // TODO Impl
            if (will_die)
            {
                // メッセージ等がうっとうしいので破壊が確定している場合は
                // 通常の特殊効果をスキップ
                goto SkipNormalEffect;
            }

            // 各種効果の発動チェック

            // 捕縛攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("縛") && !t.SpecialEffectImmune("縛"))
                {
                    msg = msg + "[" + t.Nickname + "]の自由を奪った。;";
                    if (w.IsWeaponLevelSpecified("縛"))
                    {
                        t.AddCondition("行動不能", (int)w.WeaponLevel("縛"), cdata: "");
                    }
                    else
                    {
                        t.AddCondition("行動不能", 2, cdata: "");
                    }

                    critical_type = critical_type + " 捕縛";
                    CauseEffectRet = true;
                }
            }

            // ショック攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("Ｓ") && !t.SpecialEffectImmune("Ｓ"))
                {
                    msg = msg + "[" + t.Nickname + "]を一時的に行動不能にした。;";
                    if (w.IsWeaponLevelSpecified("Ｓ"))
                    {
                        t.AddCondition("行動不能", (int)w.WeaponLevel("Ｓ"), cdata: "");
                    }
                    else
                    {
                        t.AddCondition("行動不能", 1, cdata: "");
                    }

                    critical_type = critical_type + " ショック";
                    CauseEffectRet = true;
                }
            }

            // 装甲劣化攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("劣") && !t.SpecialEffectImmune("劣"))
                {
                    msg = msg + "[" + t.Nickname + "]の" + Expression.Term("装甲", t) + "を劣化させた。;";
                    if (w.IsWeaponLevelSpecified("劣"))
                    {
                        t.AddCondition("装甲劣化", (int)w.WeaponLevel("劣"), Constants.DEFAULT_LEVEL, Expression.Term("装甲", t) + "劣化");
                    }
                    else
                    {
                        t.AddCondition("装甲劣化", 10000, Constants.DEFAULT_LEVEL, Expression.Term("装甲", t) + "劣化");
                    }

                    critical_type = critical_type + " 劣化";
                    CauseEffectRet = true;
                }
            }

            // バリア中和攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                string fname = "";
                if (w.IsWeaponClassifiedAs("中") && !t.SpecialEffectImmune("中") && (t.IsFeatureAvailable("バリア") || t.IsFeatureAvailable("バリアシールド") || t.IsFeatureAvailable("広域バリア") || t.IsFeatureAvailable("フィールド") || t.IsFeatureAvailable("アクティブフィールド") || t.IsFeatureAvailable("広域フィールド") || t.IsFeatureAvailable("プロテクション") || t.IsFeatureAvailable("アクティプロテクション") || t.IsFeatureAvailable("広域プロテクション")))
                {
                    fname = "バリア";
                    if (t.IsFeatureAvailable("バリア") && Strings.InStr(t.FeatureData("バリア"), "バリア無効化無効") == 0)
                    {
                        fname = t.FeatureName0("バリア");
                        if (Strings.Len(fname) == 0)
                        {
                            fname = "バリア";
                        }
                    }
                    else if (t.IsFeatureAvailable("バリアシールド") && Strings.InStr(t.FeatureData("バリアシールド"), "バリア無効化無効") == 0)
                    {
                        fname = t.FeatureName0("バリアシールド");
                        if (Strings.Len(fname) == 0)
                        {
                            fname = "バリアシールド";
                        }
                    }
                    else if (t.IsFeatureAvailable("広域バリア"))
                    {
                        fname = t.FeatureName0("広域バリア");
                        if (Strings.Len(fname) == 0)
                        {
                            fname = "広域バリア";
                        }
                    }
                    else if (t.IsFeatureAvailable("フィールド") && Strings.InStr(t.FeatureData("フィールド"), "バリア無効化無効") == 0)
                    {
                        fname = t.FeatureName0("フィールド");
                        if (Strings.Len(fname) == 0)
                        {
                            fname = "フィールド";
                        }
                    }
                    else if (t.IsFeatureAvailable("アクティブフィールド") && Strings.InStr(t.FeatureData("アクティブフィールド"), "バリア無効化無効") == 0)
                    {
                        fname = t.FeatureName0("アクティブフィールド");
                        if (Strings.Len(fname) == 0)
                        {
                            fname = "アクティブフィールド";
                        }
                    }
                    else if (t.IsFeatureAvailable("広域フィールド"))
                    {
                        fname = t.FeatureName0("広域フィールド");
                        if (Strings.Len(fname) == 0)
                        {
                            fname = "広域フィールド";
                        }
                    }
                    else if (t.IsFeatureAvailable("プロテクション") && Strings.InStr(t.FeatureData("プロテクション"), "バリア無効化無効") == 0)
                    {
                        fname = t.FeatureName0("プロテクション");
                        if (Strings.Len(fname) == 0)
                        {
                            fname = "プロテクション";
                        }
                    }
                    else if (t.IsFeatureAvailable("アクティブプロテクション") && Strings.InStr(t.FeatureData("アクティブプロテクション"), "バリア無効化無効") == 0)
                    {
                        fname = t.FeatureName0("アクティブプロテクション");
                        if (Strings.Len(fname) == 0)
                        {
                            fname = "アクティブプロテクション";
                        }
                    }
                    else if (t.IsFeatureAvailable("広域プロテクション"))
                    {
                        fname = t.FeatureName0("広域プロテクション");
                        if (Strings.Len(fname) == 0)
                        {
                            fname = "広域プロテクション";
                        }
                    }

                    msg = msg + "[" + t.Nickname + "]の" + fname + "を無効化した。;";
                    if (w.IsWeaponLevelSpecified("中"))
                    {
                        t.AddCondition("バリア無効化", (int)w.WeaponLevel("中"), cdata: "");
                    }
                    else
                    {
                        t.AddCondition("バリア無効化", 1, cdata: "");
                    }

                    critical_type = critical_type + " バリア中和";
                    CauseEffectRet = true;
                }
            }

            // 石化攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("石") && !t.SpecialEffectImmune("石") && t.BossRank < 0)
                {
                    msg = msg + "[" + t.Nickname + "]を石化させた。;";
                    if (w.IsWeaponLevelSpecified("石"))
                    {
                        t.AddCondition("石化", (int)w.WeaponLevel("石"), cdata: "");
                    }
                    else
                    {
                        t.AddCondition("石化", 10000, cdata: "");
                    }

                    critical_type = critical_type + " 石化";
                    CauseEffectRet = true;
                }
            }

            // 凍結攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("凍") && !t.SpecialEffectImmune("凍"))
                {
                    msg = msg + "[" + t.Nickname + "]を凍らせた。;";
                    if (w.IsWeaponLevelSpecified("凍"))
                    {
                        t.AddCondition("凍結", (int)w.WeaponLevel("凍"), cdata: "");
                    }
                    else
                    {
                        t.AddCondition("凍結", 3, cdata: "");
                    }

                    critical_type = critical_type + " 凍結";
                    CauseEffectRet = true;
                }
            }

            // 麻痺攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("痺") && !t.SpecialEffectImmune("痺"))
                {
                    msg = msg + "[" + t.Nickname + "]を麻痺させた。;";
                    if (w.IsWeaponLevelSpecified("痺"))
                    {
                        t.AddCondition("麻痺", (int)w.WeaponLevel("痺"), cdata: "");
                    }
                    else
                    {
                        t.AddCondition("麻痺", 3, cdata: "");
                    }

                    critical_type = critical_type + " 麻痺";
                    CauseEffectRet = true;
                }
            }

            // 催眠攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("眠") && !t.SpecialEffectImmune("眠") && !(t.MainPilot().Personality == "機械"))
                {
                    msg = msg + "[" + t.MainPilot().get_Nickname(false) + "]を眠らせた。;";
                    if (w.IsWeaponLevelSpecified("眠"))
                    {
                        t.AddCondition("睡眠", (int)w.WeaponLevel("眠"), cdata: "");
                    }
                    else
                    {
                        t.AddCondition("睡眠", 3, cdata: "");
                    }

                    critical_type = critical_type + " 睡眠";
                    CauseEffectRet = true;
                }
            }

            // 混乱攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("乱") && !t.SpecialEffectImmune("乱"))
                {
                    msg = msg + "[" + t.MainPilot().get_Nickname(false) + "]を混乱させた。;";
                    if (w.IsWeaponLevelSpecified("乱"))
                    {
                        t.AddCondition("混乱", (int)w.WeaponLevel("乱"), cdata: "");
                    }
                    else
                    {
                        t.AddCondition("混乱", 3, cdata: "");
                    }

                    critical_type = critical_type + " 混乱";
                    CauseEffectRet = true;
                }
            }

            if (!ReferenceEquals(t, this))
            {
                // 魅了攻撃
                if (prob >= GeneralLib.Dice(100))
                {
                    if (w.IsWeaponClassifiedAs("魅") 
                        && !t.SpecialEffectImmune("魅")
                        && !t.IsConditionSatisfied("魅了") 
                        && !t.IsConditionSatisfied("憑依"))
                    {
                        msg = msg + MainPilot().get_Nickname(false) + "が[" + t.MainPilot().get_Nickname(false) + "]を魅了した。;";
                        if (w.IsWeaponLevelSpecified("魅"))
                        {
                            t.AddCondition("魅了", (int)w.WeaponLevel("魅"), cdata: "");
                        }
                        else
                        {
                            t.AddCondition("魅了", 3, cdata: "");
                        }

                        if (t.Master is object)
                        {
                            t.Master.DeleteSlave(t.ID);
                        }

                        AddSlave(t);
                        t.Master = this;
                        t.Mode = MainPilot().ID;
                        SRC.PList.UpdateSupportMod(t);
                        critical_type = critical_type + " 魅了";
                        CauseEffectRet = true;
                    }
                }

                // 憑依攻撃
                if (prob >= GeneralLib.Dice(100))
                {
                    if (w.IsWeaponClassifiedAs("憑") && !t.SpecialEffectImmune("憑") && !t.IsConditionSatisfied("憑依") && t.BossRank < 0)
                    {
                        msg = msg + MainPilot().get_Nickname(false) + "が[" + t.Nickname + "]を乗っ取った。;";
                        if (t.IsConditionSatisfied("魅了"))
                        {
                            // 憑依の方の効果を優先する
                            t.DeleteCondition("魅了");
                        }

                        if (w.IsWeaponLevelSpecified("憑"))
                        {
                            t.AddCondition("憑依", (int)w.WeaponLevel("憑"), cdata: "");
                        }
                        else
                        {
                            t.AddCondition("憑依", 10000, cdata: "");
                        }

                        if (t.Master is object)
                        {
                            t.Master.DeleteSlave(t.ID);
                        }

                        AddSlave(t);
                        t.Master = this;
                        SRC.PList.UpdateSupportMod(t);
                        critical_type = critical_type + " 憑依";
                        CauseEffectRet = true;
                    }
                }
            }

            // 撹乱攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("撹") && !t.SpecialEffectImmune("撹"))
                {
                    msg = msg + "[" + t.Nickname + "]を撹乱した。;";
                    if (w.IsWeaponLevelSpecified("撹"))
                    {
                        t.AddCondition("撹乱", (int)w.WeaponLevel("撹"), cdata: "");
                    }
                    else
                    {
                        t.AddCondition("撹乱", 2, cdata: "");
                    }

                    critical_type = critical_type + " 撹乱";
                    CauseEffectRet = true;
                }
            }

            // 恐怖攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("恐") && !t.SpecialEffectImmune("恐"))
                {
                    msg = msg + t.MainPilot().get_Nickname(false) + "は恐怖に陥った。;";
                    if (w.IsWeaponLevelSpecified("恐"))
                    {
                        t.AddCondition("恐怖", (int)w.WeaponLevel("恐"), cdata: "");
                    }
                    else
                    {
                        t.AddCondition("恐怖", 3, cdata: "");
                    }

                    critical_type = critical_type + " 恐怖";
                    CauseEffectRet = true;
                }
            }

            // 目潰し攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("盲") && !t.SpecialEffectImmune("盲"))
                {
                    msg = msg + "[" + t.MainPilot().get_Nickname(false) + "]の視力を奪った。;";
                    if (w.IsWeaponLevelSpecified("盲"))
                    {
                        t.AddCondition("盲目", (int)w.WeaponLevel("盲"), cdata: "");
                    }
                    else
                    {
                        t.AddCondition("盲目", 3, cdata: "");
                    }

                    critical_type = critical_type + " 盲目";
                    CauseEffectRet = true;
                }
            }

            // 毒攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("毒") && !t.SpecialEffectImmune("毒"))
                {
                    msg = msg + t.Nickname + "は毒を受けた。;";
                    if (w.IsWeaponLevelSpecified("毒"))
                    {
                        t.AddCondition("毒", (int)w.WeaponLevel("毒"), cdata: "");
                    }
                    else
                    {
                        t.AddCondition("毒", 3, cdata: "");
                    }

                    critical_type = critical_type + " 毒";
                    CauseEffectRet = true;
                }
            }

            // 攻撃封印攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("不") && !t.SpecialEffectImmune("不"))
                {
                    msg = msg + "[" + t.Nickname + "]の攻撃能力を奪った。;";
                    if (w.IsWeaponLevelSpecified("不"))
                    {
                        t.AddCondition("攻撃不能", (int)w.WeaponLevel("不"), cdata: "");
                    }
                    else
                    {
                        t.AddCondition("攻撃不能", 1, cdata: "");
                    }

                    critical_type = critical_type + " 攻撃不能";
                    CauseEffectRet = true;
                }
            }

            // 足止め攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("止") && !t.SpecialEffectImmune("止"))
                {
                    msg = msg + "[" + t.Nickname + "]の動きを止めた。;";
                    if ((t.Party ?? "") != (SRC.Stage ?? ""))
                    {
                        if (w.IsWeaponLevelSpecified("止"))
                        {
                            t.AddCondition("移動不能", (int)(w.WeaponLevel("止") + 1d), cdata: "");
                        }
                        else
                        {
                            t.AddCondition("移動不能", 2, cdata: "");
                        }
                    }
                    else
                    {
                        if (w.IsWeaponLevelSpecified("止"))
                        {
                            t.AddCondition("移動不能", (int)w.WeaponLevel("止"), cdata: "");
                        }
                        else
                        {
                            t.AddCondition("移動不能", 1, cdata: "");
                        }
                    }

                    critical_type = critical_type + " 移動不能";
                    CauseEffectRet = true;
                }
            }

            // 沈黙攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("黙") && !t.SpecialEffectImmune("黙"))
                {
                    msg = msg + "[" + t.MainPilot().get_Nickname(false) + "]を沈黙させた。;";
                    if (w.IsWeaponLevelSpecified("黙"))
                    {
                        t.AddCondition("沈黙", (int)w.WeaponLevel("黙"), cdata: "");
                    }
                    else
                    {
                        t.AddCondition("沈黙", 3, cdata: "");
                    }

                    critical_type = critical_type + " 沈黙";
                    CauseEffectRet = true;
                }
            }

            // 踊らせ攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("踊") && !t.SpecialEffectImmune("踊"))
                {
                    msg = msg + "[" + t.Nickname + "]は突然踊りだした。;";
                    if (w.IsWeaponLevelSpecified("踊"))
                    {
                        t.AddCondition("踊り", (int)w.WeaponLevel("踊"), cdata: "");
                    }
                    else
                    {
                        t.AddCondition("踊り", 3, cdata: "");
                    }

                    critical_type = critical_type + " 踊り";
                    CauseEffectRet = true;
                }
            }

            // 狂戦士化攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("狂") && !t.SpecialEffectImmune("狂"))
                {
                    msg = msg + "[" + t.MainPilot().get_Nickname(false) + "]は狂戦士と化した。;";
                    if (w.IsWeaponLevelSpecified("狂"))
                    {
                        t.AddCondition("狂戦士", (int)w.WeaponLevel("狂"), cdata: "");
                    }
                    else
                    {
                        t.AddCondition("狂戦士", 3, cdata: "");
                    }

                    critical_type = critical_type + " 狂戦士";
                    CauseEffectRet = true;
                }
            }

            // ゾンビ化攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("ゾ") && !t.SpecialEffectImmune("ゾ"))
                {
                    msg = msg + "[" + t.Nickname + "]はゾンビと化した。;";
                    if (w.IsWeaponLevelSpecified("ゾ"))
                    {
                        t.AddCondition("ゾンビ", (int)w.WeaponLevel("ゾ"), cdata: "");
                    }
                    else
                    {
                        t.AddCondition("ゾンビ", 10000, cdata: "");
                    }

                    critical_type = critical_type + " ゾンビ";
                    CauseEffectRet = true;
                }
            }

            // 回復能力阻害攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("害") && !t.SpecialEffectImmune("害"))
                {
                    msg = msg + "[" + t.Nickname + "]の自己回復能力は封じられた。;";
                    if (w.IsWeaponLevelSpecified("害"))
                    {
                        t.AddCondition("回復不能", (int)w.WeaponLevel("害"), cdata: "");
                    }
                    else
                    {
                        t.AddCondition("回復不能", 10000, cdata: "");
                    }

                    critical_type = critical_type + " 回復不能";
                    CauseEffectRet = true;
                }
            }

            // TODO Impl
            //// 特殊効果除去攻撃
            //if (prob >= GeneralLib.Dice(100))
            //{
            //    if (w.IsWeaponClassifiedAs("除") && !t.SpecialEffectImmune("除"))
            //    {
            //        var loopTo = t.CountCondition();
            //        for (i = 1; i <= loopTo; i++)
            //        {
            //            string localCondition() { object argIndex1 = i; var ret = t.Condition(argIndex1); return ret; }

            //            string localCondition1() { object argIndex1 = i; var ret = t.Condition(argIndex1); return ret; }

            //            string localCondition2() { object argIndex1 = i; var ret = t.Condition(argIndex1); return ret; }

            //            string localCondition3() { object argIndex1 = i; var ret = t.Condition(argIndex1); return ret; }

            //            int localConditionLifetime() { object argIndex1 = i; var ret = t.ConditionLifetime(argIndex1); return ret; }

            //            if ((Strings.InStr(localCondition(), "付加") > 0 || Strings.InStr(localCondition1(), "強化") > 0 || Strings.InStr(localCondition2(), "ＵＰ") > 0) && localCondition3() != "ノーマルモード付加" && localConditionLifetime() > 0)
            //            {
            //                break;
            //            }
            //        }

            //        if (i <= t.CountCondition())
            //        {
            //            msg = msg + "[" + t.Nickname + "]にかけられた特殊効果を打ち消した。;";
            //            do
            //            {
            //                string localCondition4() { object argIndex1 = i; var ret = t.Condition(argIndex1); return ret; }

            //                string localCondition5() { object argIndex1 = i; var ret = t.Condition(argIndex1); return ret; }

            //                string localCondition6() { object argIndex1 = i; var ret = t.Condition(argIndex1); return ret; }

            //                string localCondition7() { object argIndex1 = i; var ret = t.Condition(argIndex1); return ret; }

            //                int localConditionLifetime1() { object argIndex1 = i; var ret = t.ConditionLifetime(argIndex1); return ret; }

            //                if ((Strings.InStr(localCondition4(), "付加") > 0 || Strings.InStr(localCondition5(), "強化") > 0 || Strings.InStr(localCondition6(), "ＵＰ") > 0) && localCondition7() != "ノーマルモード付加" && localConditionLifetime1() > 0)
            //                {
            //                    t.DeleteCondition(i);
            //                }
            //                else
            //                {
            //                    i = (i + 1);
            //                }
            //            }
            //            while (i <= t.CountCondition());
            //            critical_type = critical_type + " 解除";
            //            CauseEffectRet = true;
            //        }
            //    }
            //}

            // 即死攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("即")
                    && (!t.SpecialEffectImmune("即") || t.Weakness(w.WeaponClass()) || t.Effective(w.WeaponClass()))
                    && t.BossRank < 0
                    && (!IsUnderSpecialPowerEffect("てかげん") || this.MainPilot().Technique <= t.MainPilot().Technique)
                    && !t.IsConditionSatisfied("不死身"))
                {
                    critical_type = critical_type + " 即死";
                    CauseEffectRet = true;
                    return CauseEffectRet;
                }
            }

            // 死の宣告
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("告") && !t.SpecialEffectImmune("告") && t.BossRank < 0)
                {
                    msg = msg + "[" + t.MainPilot().get_Nickname(false) + "]に死の宣告が下された。;";
                    if (Strings.InStr(w.WeaponClass(), "告L") > 0)
                    {
                        if (w.WeaponLevel("告") > 0d)
                        {
                            t.AddCondition("死の宣告", (int)w.WeaponLevel("告"), cdata: "");
                        }
                        else
                        {
                            t.HP = 1;
                        }
                    }
                    else
                    {
                        t.AddCondition("死の宣告", 1, cdata: "");
                    }

                    critical_type = critical_type + " 死の宣告";
                    CauseEffectRet = true;
                }
            }

            if (t.MainPilot().Personality != "機械")
            {
                // 気力減少攻撃
                if (prob >= GeneralLib.Dice(100))
                {
                    if (w.IsWeaponClassifiedAs("脱") && !t.SpecialEffectImmune("脱"))
                    {
                        msg = msg + "[" + t.MainPilot().get_Nickname(false) + "]の" + Expression.Term("気力", t) + "を低下させた。;";
                        if (w.IsWeaponLevelSpecified("脱"))
                        {
                            t.IncreaseMorale((-5 * (int)w.WeaponLevel("脱")));
                        }
                        else
                        {
                            t.IncreaseMorale(-10);
                        }

                        critical_type = critical_type + " 脱力";
                        CauseEffectRet = true;
                    }
                }

                // 気力吸収攻撃
                if (prob >= GeneralLib.Dice(100))
                {
                    if (w.IsWeaponClassifiedAs("Ｄ") && !t.SpecialEffectImmune("Ｄ"))
                    {
                        msg = msg + MainPilot().get_Nickname(false) + "は[" + t.MainPilot().get_Nickname(false) + "]の" + Expression.Term("気力", t) + "を吸い取った。;";
                        if (w.IsWeaponLevelSpecified("Ｄ"))
                        {
                            t.IncreaseMorale((-5 * (int)w.WeaponLevel("Ｄ")));
                            IncreaseMorale((int)(2.5d * (int)w.WeaponLevel("Ｄ")));
                        }
                        else
                        {
                            t.IncreaseMorale(-10);
                            IncreaseMorale(5);
                        }

                        critical_type = critical_type + " 気力吸収";
                        CauseEffectRet = true;
                    }
                }
            }

            // 攻撃力低下攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("低攻") && !t.SpecialEffectImmune("低攻"))
                {
                    msg = msg + "[" + t.Nickname + "]の攻撃力を低下させた。;";
                    if (t.IsConditionSatisfied("攻撃力ＵＰ"))
                    {
                        t.DeleteCondition("攻撃力ＵＰ");
                    }
                    else
                    {
                        if (w.IsWeaponLevelSpecified("低攻"))
                        {
                            t.AddCondition("攻撃力ＤＯＷＮ", (int)w.WeaponLevel("低攻"), cdata: "");
                        }
                        else
                        {
                            t.AddCondition("攻撃力ＤＯＷＮ", 3, cdata: "");
                        }
                    }

                    critical_type = critical_type + " 攻撃力ＤＯＷＮ";
                    CauseEffectRet = true;
                }
            }

            // 防御力低下攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("低防") && !t.SpecialEffectImmune("低防"))
                {
                    msg = msg + "[" + t.Nickname + "]の防御力を低下させた。;";
                    if (t.IsConditionSatisfied("防御力ＵＰ"))
                    {
                        t.DeleteCondition("防御力ＵＰ");
                    }
                    else
                    {
                        if (w.IsWeaponLevelSpecified("低防"))
                        {
                            t.AddCondition("防御力ＤＯＷＮ", (int)w.WeaponLevel("低防"), cdata: "");
                        }
                        else
                        {
                            t.AddCondition("防御力ＤＯＷＮ", 3, cdata: "");
                        }
                    }

                    critical_type = critical_type + " 防御力ＤＯＷＮ";
                    CauseEffectRet = true;
                }
            }

            // 運動性低下攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("低運") && !t.SpecialEffectImmune("低運"))
                {
                    msg = msg + "[" + t.Nickname + "]の" + Expression.Term("運動性", t) + "を低下させた。;";
                    if (t.IsConditionSatisfied("運動性ＵＰ"))
                    {
                        t.DeleteCondition("運動性ＵＰ");
                    }
                    else
                    {
                        if (w.IsWeaponLevelSpecified("低運"))
                        {
                            t.AddCondition("運動性ＤＯＷＮ", (int)w.WeaponLevel("低運"), Constants.DEFAULT_LEVEL, Expression.Term("運動性", t) + "ＤＯＷＮ");
                        }
                        else
                        {
                            t.AddCondition("運動性ＤＯＷＮ", 3, Constants.DEFAULT_LEVEL, Expression.Term("運動性", t) + "ＤＯＷＮ");
                        }
                    }

                    critical_type = critical_type + " 運動性ＤＯＷＮ";
                    CauseEffectRet = true;
                }
            }

            // 移動力低下攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("低移") && !t.SpecialEffectImmune("低移"))
                {
                    msg = msg + "[" + t.Nickname + "]の" + Expression.Term("移動力", t) + "を低下させた。;";
                    if (t.IsConditionSatisfied("移動力ＵＰ"))
                    {
                        t.DeleteCondition("移動力ＵＰ");
                    }
                    else
                    {
                        if (w.IsWeaponLevelSpecified("低移"))
                        {
                            t.AddCondition("移動力ＤＯＷＮ", (int)w.WeaponLevel("低移"), Constants.DEFAULT_LEVEL, Expression.Term("移動力", t) + "ＤＯＷＮ");
                        }
                        else
                        {
                            t.AddCondition("移動力ＤＯＷＮ", 3, Constants.DEFAULT_LEVEL, Expression.Term("移動力", t) + "ＤＯＷＮ");
                        }
                    }

                    critical_type = critical_type + " 移動力ＤＯＷＮ";
                    CauseEffectRet = true;
                }
            }

            // ＨＰ減衰攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("衰") && !t.SpecialEffectImmune("衰"))
                {
                    msg = msg + "[" + t.Nickname + "]の" + Expression.Term("ＨＰ", t) + "を";
                    if (t.BossRank >= 0)
                    {
                        switch (w.WeaponLevel("衰"))
                        {
                            case 1:
                                {
                                    t.HP = t.HP * 7 / 8;
                                    msg = msg + "12.5%";
                                    break;
                                }

                            case 2:
                                {
                                    t.HP = t.HP * 3 / 4;
                                    msg = msg + "25%";
                                    break;
                                }

                            case 3:
                                {
                                    t.HP = t.HP / 2;
                                    msg = msg + "50%";
                                    break;
                                }
                        }
                    }
                    else
                    {
                        switch (w.WeaponLevel("衰"))
                        {
                            case 1:
                                {
                                    t.HP = t.HP * 3 / 4;
                                    msg = msg + "25%";
                                    break;
                                }

                            case 2:
                                {
                                    t.HP = t.HP / 2;
                                    msg = msg + "50%";
                                    break;
                                }

                            case 3:
                                {
                                    t.HP = t.HP / 4;
                                    msg = msg + "75%";
                                    break;
                                }
                        }
                    }

                    msg = msg + "減少させた。;";
                    critical_type = critical_type + " 減衰";
                    CauseEffectRet = true;
                }
            }

            // ＥＮ減衰攻撃
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("滅") && !t.SpecialEffectImmune("滅"))
                {
                    msg = msg + "[" + t.Nickname + "]の" + Expression.Term("ＥＮ", t) + "を";
                    if (t.BossRank >= 0)
                    {
                        switch (w.WeaponLevel("滅"))
                        {
                            case 1:
                                {
                                    t.EN = t.EN * 7 / 8;
                                    msg = msg + "12.5%";
                                    break;
                                }

                            case 2:
                                {
                                    t.EN = t.EN * 3 / 4;
                                    msg = msg + "25%";
                                    break;
                                }

                            case 3:
                                {
                                    t.EN = t.EN / 2;
                                    msg = msg + "50%";
                                    break;
                                }
                        }
                    }
                    else
                    {
                        switch (w.WeaponLevel("滅"))
                        {
                            case 1:
                                {
                                    t.EN = t.EN * 3 / 4;
                                    msg = msg + "25%";
                                    break;
                                }

                            case 2:
                                {
                                    t.EN = t.EN / 2;
                                    msg = msg + "50%";
                                    break;
                                }

                            case 3:
                                {
                                    t.EN = t.EN / 4;
                                    msg = msg + "75%";
                                    break;
                                }
                        }
                    }

                    msg = msg + "減少させた。;";
                    critical_type = critical_type + " 減衰";
                    CauseEffectRet = true;
                }
            }

        // TODO Impl
        //// 弱点付加属性（弱が存在するだけループ）
        //i = GeneralLib.InStrNotNest(strWeaponClass[w], "弱");
        //while (i > 0)
        //{
        //    ch = Strings.Mid(GeneralLib.GetClassBundle(strWeaponClass[w], i), 2);
        //    if (prob >= GeneralLib.Dice(100))
        //    {
        //        if (!t.SpecialEffectImmune(ch))
        //        {
        //            msg = msg + "[" + t.Nickname + "]は[" + ch + "]属性に弱くなった。;";
        //            if (w.IsWeaponLevelSpecified("弱" + ch))
        //            {
        //                double localWeaponLevel() { string argattr = "弱" + ch; var ret = (int)w.WeaponLevel(argattr); return ret; }

        //                t.AddCondition(ch + "属性弱点付加", localWeaponLevel(), cdata: "");
        //            }
        //            else
        //            {
        //                t.AddCondition(ch + "属性弱点付加", 3, cdata: "");
        //            }

        //            critical_type = critical_type + " " + ch + "属性弱点付加";
        //            CauseEffectRet = true;
        //        }
        //    }

        //    i = GeneralLib.InStrNotNest(strWeaponClass[w], "弱", (i + 1));
        //}

        //// 有効付加属性
        //i = GeneralLib.InStrNotNest(strWeaponClass[w], "効");
        //while (i > 0)
        //{
        //    ch = Strings.Mid(GeneralLib.GetClassBundle(strWeaponClass[w], i), 2);
        //    if (prob >= GeneralLib.Dice(100))
        //    {
        //        // 既に相手が指定属性を弱点として持っている場合無効
        //        if (!t.Weakness(ch) && !t.SpecialEffectImmune(ch))
        //        {
        //            msg = msg + "[" + t.Nickname + "]に[" + ch + "]属性が有効になった。;";
        //            if (w.IsWeaponLevelSpecified("効" + ch))
        //            {
        //                double localWeaponLevel1() { string argattr = "効" + ch; var ret = (int)w.WeaponLevel(argattr); return ret; }

        //                t.AddCondition(ch + "属性有効付加", localWeaponLevel1(), cdata: "");
        //            }
        //            else
        //            {
        //                t.AddCondition(ch + "属性有効付加", 3, cdata: "");
        //            }

        //            critical_type = critical_type + " " + ch + "属性有効付加";
        //            CauseEffectRet = true;
        //        }
        //    }

        //    i = GeneralLib.InStrNotNest(strWeaponClass[w], "効", (i + 1));
        //}

        //// 属性使用禁止攻撃
        //i = GeneralLib.InStrNotNest(strWeaponClass[w], "剋");
        //while (i > 0)
        //{
        //    ch = Strings.Mid(GeneralLib.GetClassBundle(strWeaponClass[w], i), 2);
        //    if (prob >= GeneralLib.Dice(100))
        //    {
        //        if (!t.SpecialEffectImmune(ch))
        //        {
        //            Skill = new string[1];
        //            switch (ch ?? "")
        //            {
        //                case "オ":
        //                    {
        //                        Skill[0] = "オーラ";
        //                        break;
        //                    }

        //                case "超":
        //                    {
        //                        Skill[0] = "超能力";
        //                        break;
        //                    }

        //                case "シ":
        //                    {
        //                        Skill[0] = "同調率";
        //                        break;
        //                    }

        //                case "サ":
        //                    {
        //                        if (t.MainPilot().IsSkillAvailable("超感覚") && t.MainPilot(Conversions.ToBoolean(0)).IsSkillAvailable("知覚強化"))
        //                        {
        //                            Skill = new string[2];
        //                            Skill[0] = "超感覚";
        //                            Skill[1] = "知覚強化";
        //                        }
        //                        else if (t.MainPilot().IsSkillAvailable("超感覚"))
        //                        {
        //                            Skill[0] = "超感覚";
        //                        }
        //                        else if (t.MainPilot().IsSkillAvailable("知覚強化"))
        //                        {
        //                            Skill[0] = "知覚強化";
        //                        }
        //                        else
        //                        {
        //                            Skill = new string[2];
        //                            Skill[0] = "超感覚";
        //                            Skill[1] = "知覚強化";
        //                        }

        //                        break;
        //                    }

        //                case "霊":
        //                    {
        //                        Skill[0] = "霊力";
        //                        break;
        //                    }

        //                case "術":
        //                    {
        //                        Skill[0] = "術";
        //                        break;
        //                    }

        //                case "技":
        //                    {
        //                        Skill[0] = "技";
        //                        break;
        //                    }

        //                default:
        //                    {
        //                        Skill[0] = "";
        //                        break;
        //                    }
        //            }

        //            var loopTo1 = Information.UBound(Skill);
        //            for (j = 0; j <= loopTo1; j++)
        //            {
        //                if (Strings.Len(Skill[j]) > 0)
        //                {
        //                    var tmp = Skill;
        //                    fname = t.MainPilot().SkillName0(tmp[j]);
        //                    if (fname == "非表示")
        //                    {
        //                        fname = Skill[j];
        //                    }
        //                }
        //                else
        //                {
        //                    Skill[0] = ch + "属性";
        //                    fname = ch + "属性";
        //                }

        //                msg = msg + "[" + t.Nickname + "]は" + fname + "が使用出来なくなった。;";
        //                if (w.IsWeaponLevelSpecified("剋" + ch))
        //                {
        //                    double localWeaponLevel2() { string argattr = "剋" + ch; var ret = (int)w.WeaponLevel(argattr); return ret; }

        //                    t.AddCondition(Skill[j] + "使用不能", localWeaponLevel2(), cdata: "");
        //                }
        //                else
        //                {
        //                    t.AddCondition(Skill[j] + "使用不能", 3, cdata: "");
        //                }

        //                critical_type = critical_type + " " + Skill[j] + "使用不能";
        //            }

        //            CauseEffectRet = true;
        //        }
        //    }

        //    i = GeneralLib.InStrNotNest(strWeaponClass[w], "剋", (i + 1));
        //}

        SkipNormalEffect:
            ;


            // これ以降の効果は敵が破壊される場合も発動する

            // 盗み
            int prev_money;
            string iname;
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("盗") && !t.SpecialEffectImmune("盗") && !t.IsConditionSatisfied("すかんぴん") && (Party == "味方" && t.Party0 != "味方" || Party != "味方" && t.Party0 == "味方"))
                {
                    if (t.Party0 == "味方")
                    {
                        // 味方の場合は必ず資金が減少する
                        prev_money = SRC.Money;
                        SRC.IncrMoney(-t.Value / 2);
                        if (w.WeaponData.Power > 0)
                        {
                            msg = msg + "[" + t.Nickname + "]は" + Expression.Term("資金", t) + SrcFormatter.Format(prev_money - SRC.Money) + "を奪い取られた。;";
                        }
                        else
                        {
                            msg = msg + "[" + t.Nickname + "]は" + Expression.Term("資金", t) + SrcFormatter.Format(prev_money - SRC.Money) + "を盗まれた。;";
                        }

                        critical_type = critical_type + " 盗み";
                        CauseEffectRet = true;
                    }
                    else if (GeneralLib.Dice(8) == 1 && t.IsFeatureAvailable("レアアイテム所有"))
                    {
                        // レアアイテムを盗んだ場合
                        iname = t.FeatureData("レアアイテム所有");
                        if (SRC.IDList.IsDefined(iname))
                        {
                            SRC.IList.Add(iname);
                            if (w.WeaponData.Power > 0)
                            {
                                msg = msg + "[" + t.Nickname + "]から" + SRC.IDList.Item(iname).Nickname + "を奪い取った。;";
                            }
                            else
                            {
                                msg = msg + "[" + t.Nickname + "]から" + SRC.IDList.Item(iname).Nickname + "を盗んだ。;";
                            }
                        }
                        else
                        {
                            GUI.ErrorMessage(t.Name + "の所有アイテム「" + iname + "」のデータが見つかりません");
                        }

                        critical_type = critical_type + " 盗み";
                        CauseEffectRet = true;
                    }
                    else if (t.IsFeatureAvailable("アイテム所有"))
                    {
                        // アイテムを盗んだ場合
                        iname = t.FeatureData("アイテム所有");
                        if (SRC.IDList.IsDefined(iname))
                        {
                            SRC.IList.Add(iname);
                            if (w.WeaponData.Power > 0)
                            {
                                msg = msg + "[" + t.Nickname + "]から" + SRC.IDList.Item(iname).Nickname + "を奪い取った。;";
                            }
                            else
                            {
                                msg = msg + "[" + t.Nickname + "]から" + SRC.IDList.Item(iname).Nickname + "を盗んだ。;";
                            }
                        }
                        else
                        {
                            GUI.ErrorMessage(t.Name + "の所有アイテム「" + iname + "」のデータが見つかりません");
                        }

                        critical_type = critical_type + " 盗み";
                        CauseEffectRet = true;
                    }
                    else if (t.Value > 0)
                    {
                        // 資金を盗んだ場合
                        SRC.IncrMoney(t.Value / 4);
                        if (w.WeaponData.Power > 0)
                        {
                            msg = msg + "[" + t.Nickname + "]から" + Expression.Term("資金", t) + SrcFormatter.Format(t.Value / 4) + "を奪い取った。;";
                        }
                        else
                        {
                            msg = msg + "[" + t.Nickname + "]から" + Expression.Term("資金", t) + SrcFormatter.Format(t.Value / 4) + "を盗んだ。;";
                        }

                        critical_type = critical_type + " 盗み";
                        CauseEffectRet = true;
                    }

                    // 一度盗んだユニットからは再度盗むことは出来ない
                    if (t.Party0 != "味方")
                    {
                        t.AddCondition("すかんぴん", -1, 0d, "非表示");
                    }
                }
            }

            // ラーニング
            string sname, stype, vname;
            if (prob >= GeneralLib.Dice(100))
            {
                if (w.IsWeaponClassifiedAs("習") && t.IsFeatureAvailable("ラーニング可能技") && Party0 == "味方")
                {
                    stype = GeneralLib.LIndex(t.FeatureData("ラーニング可能技"), 1);
                    switch (GeneralLib.LIndex(t.FeatureData("ラーニング可能技"), 2) ?? "")
                    {
                        case "表示":
                        case var @case when @case == "":
                            {
                                sname = stype;
                                break;
                            }

                        default:
                            {
                                sname = GeneralLib.LIndex(t.FeatureData("ラーニング可能技"), 2);
                                break;
                            }
                    }

                    if (!MainPilot().IsSkillAvailable(stype))
                    {
                        msg = msg + "[" + MainPilot().get_Nickname(false) + "]は「" + sname + "」を習得した。;";
                        vname = "Ability(" + MainPilot().ID + ")";
                        // XXX 元は変数オブジェクト取得して直接設定していた。ローカル変数と競合した時向け？
                        if (!Expression.IsGlobalVariableDefined(vname))
                        {
                            Expression.DefineGlobalVariable(vname);
                            Expression.SetVariableAsString(vname, stype);
                        }
                        else
                        {
                            Expression.SetVariableAsString(vname, Expression.GetValueAsString(vname) + " " + stype);
                        }

                        vname = "Ability(" + MainPilot().ID + "," + stype + ")";
                        if (!Expression.IsGlobalVariableDefined(vname))
                        {
                            Expression.DefineGlobalVariable(vname);
                        }

                        if (GeneralLib.LLength(t.FeatureData("ラーニング可能技")) == 1)
                        {
                            Expression.SetVariableAsString(vname, "-1 非表示");
                        }
                        else if ((stype ?? "") != (sname ?? ""))
                        {
                            Expression.SetVariableAsString(vname, "-1 " + sname);
                        }
                        else
                        {
                            Expression.SetVariableAsString(vname, "-1");
                        }

                        critical_type = critical_type + " ラーニング";
                        CauseEffectRet = true;
                    }
                }
            }

            return CauseEffectRet;
        }

        // 吹き飛ばしチェック
        public bool CheckBlowAttack(UnitWeapon w, Unit t, int dmg, string msg, string attack_mode, string def_mode, string critical_type)
        {
            // TODO Impl
            return false;
            //bool CheckBlowAttackRet = default;
            //int tx, ty;
            //int sx, sy;
            //int nx, ny;
            //int dx = default, dy = default;
            //var is_crashed = default(bool);
            //var t2 = default(Unit);
            //int dmg2, orig_dmg;
            //int wlevel;
            //int i, prob;
            //var is_critical = default(bool);
            //TerrainData td;

            //// 特殊効果無効？
            //if (w.IsWeaponClassifiedAs("吹") && t.SpecialEffectImmune("吹"))
            //{
            //    return CheckBlowAttackRet;
            //}

            //if (w.IsWeaponClassifiedAs("Ｋ") && t.SpecialEffectImmune("Ｋ"))
            //{
            //    return CheckBlowAttackRet;
            //}

            //wlevel = GeneralLib.MaxLng(w.WeaponLevel("吹"), (int)w.WeaponLevel("Ｋ"));

            //// 特殊効果発生確率
            //if (IsUnderSpecialPowerEffect("特殊効果発動"))
            //{
            //    prob = 100;
            //}
            //else
            //{
            //    prob = CriticalProbability(w, t, def_mode);
            //}

            //// 吹き飛ばし距離の算出
            //if (prob >= GeneralLib.Dice(100))
            //{
            //    wlevel = (wlevel + 1);
            //    is_critical = true;
            //}

            //// 吹き飛ばし距離が０であればここで終わり
            //if (wlevel == 0)
            //{
            //    return CheckBlowAttackRet;
            //}

            //// サイズによる制限
            //if (t.Size == "XL")
            //{
            //    return CheckBlowAttackRet;
            //}

            //if (w.IsWeaponClassifiedAs("Ｋ"))
            //{
            //    switch (Size ?? "")
            //    {
            //        case "SS":
            //            {
            //                if (t.Size != "SS" && t.Size != "S")
            //                {
            //                    return CheckBlowAttackRet;
            //                }

            //                break;
            //            }

            //        case "S":
            //            {
            //                if (t.Size == "L" || t.Size == "LL")
            //                {
            //                    return CheckBlowAttackRet;
            //                }

            //                break;
            //            }

            //        case "M":
            //            {
            //                if (t.Size == "LL")
            //                {
            //                    return CheckBlowAttackRet;
            //                }

            //                break;
            //            }
            //    }
            //}

            //// 固定物は動かせない
            //if (t.IsFeatureAvailable("地形ユニット"))
            //{
            //    return CheckBlowAttackRet;
            //}

            //if (t.Data.Speed == 0 && t.Speed == 0)
            //{
            //    return CheckBlowAttackRet;
            //}

            //// 自分自身は吹き飛ばせない
            //if (ReferenceEquals(t, this))
            //{
            //    return CheckBlowAttackRet;
            //}

            //// 吹き飛ばしの中心座標を設定
            //if (w.WeaponLevel("Ｍ投") > 0d)
            //{
            //    sx = Commands.SelectedX;
            //    sy = Commands.SelectedY;
            //}
            //else
            //{
            //    sx = x;
            //    sy = y;
            //}

            //// 吹き飛ばされる場所を設定
            //tx = t.x;
            //ty = t.y;
            //if (!w.IsWeaponClassifiedAs("Ｍ移"))
            //{
            //    if (Math.Abs((sx - tx)) > Math.Abs((sy - ty)))
            //    {
            //        if (sx > tx)
            //        {
            //            dx = -1;
            //        }
            //        else
            //        {
            //            dx = 1;
            //        }
            //    }
            //    else if (Math.Abs((sx - tx)) < Math.Abs((sy - ty)))
            //    {
            //        if (sy > ty)
            //        {
            //            dy = -1;
            //        }
            //        else
            //        {
            //            dy = 1;
            //        }
            //    }
            //    else if (GeneralLib.Dice(2) == 1)
            //    {
            //        if (sx > tx)
            //        {
            //            dx = -1;
            //        }
            //        else
            //        {
            //            dx = 1;
            //        }
            //    }
            //    else if (sy > ty)
            //    {
            //        dy = -1;
            //    }
            //    else
            //    {
            //        dy = 1;
            //    }
            //}
            //// Ｍ移の場合は横に弾き飛ばす形になる
            //else if (Math.Abs((sx - tx)) > Math.Abs((sy - ty)))
            //{
            //    if (GeneralLib.Dice(2) == 1)
            //    {
            //        dy = 1;
            //    }
            //    else
            //    {
            //        dy = -1;
            //    }
            //}
            //else if (Math.Abs((sx - tx)) < Math.Abs((sy - ty)))
            //{
            //    if (GeneralLib.Dice(2) == 1)
            //    {
            //        dx = 1;
            //    }
            //    else
            //    {
            //        dx = -1;
            //    }
            //}
            //else if (sx == tx && sx == ty)
            //{
            //    switch (GeneralLib.Dice(4))
            //    {
            //        case 1:
            //            {
            //                dx = -1;
            //                break;
            //            }

            //        case 2:
            //            {
            //                dx = 1;
            //                break;
            //            }

            //        case 3:
            //            {
            //                dy = -1;
            //                break;
            //            }

            //        case 4:
            //            {
            //                dy = 1;
            //                break;
            //            }
            //    }
            //}
            //else if (GeneralLib.Dice(2) == 1)
            //{
            //    if (sx > tx)
            //    {
            //        dx = 1;
            //    }
            //    else
            //    {
            //        dx = -1;
            //    }
            //}
            //else if (sy > ty)
            //{
            //    dy = 1;
            //}
            //else
            //{
            //    dy = -1;
            //}

            //// 吹き飛ばし後の位置の計算と、衝突の判定
            //nx = tx;
            //ny = ty;
            //i = 1;
            //while (i <= wlevel)
            //{
            //    nx = (nx + dx);
            //    ny = (ny + dy);

            //    // 吹き飛ばしコストに地形効果【摩擦】の補正を加える
            //    // MOD START 240a
            //    // Set td = TDList.Item(MapData(X, Y, 0))
            //    switch (Map.MapData[x, y, Map.MapDataIndex.BoxType])
            //    {
            //        case Map.BoxTypes.Under:
            //        case Map.BoxTypes.UpperBmpOnly:
            //            {
            //                td = SRC.TDList.Item(Map.MapData[x, y, Map.MapDataIndex.TerrainType]);
            //                break;
            //            }

            //        default:
            //            {
            //                td = SRC.TDList.Item(Map.MapData[x, y, Map.MapDataIndex.LayerType]);
            //                break;
            //            }
            //    }
            //    // MOD START 240a
            //    if (t.Area == "地上" && (td.Class == "陸" || td.Class == "屋内" || td.Class == "月面") || t.Area == "水中" && (td.Class == "水" || td.Class == "深水") || (t.Area ?? "") == (Class ?? ""))
            //    {
            //        if (td.IsFeatureAvailable("摩擦"))
            //        {
            //            i = (i + td.FeatureLevel("摩擦"));
            //        }
            //    }

            //    // マップ端
            //    if (nx < 1 || Map.MapWidth < nx || ny < 1 || Map.MapHeight < ny)
            //    {
            //        nx = (nx - dx);
            //        ny = (ny - dy);
            //        break;
            //    }

            //    // 進入不能？
            //    if (!t.IsAbleToEnter(nx, ny) || Map.MapDataForUnit[nx, ny] is object)
            //    {
            //        is_crashed = true;
            //        if (Map.MapDataForUnit[nx, ny] is object)
            //        {
            //            t2 = Map.MapDataForUnit[nx, ny];
            //        }

            //        nx = (nx - dx);
            //        ny = (ny - dy);
            //        break;
            //    }

            //    // 障害物あり？
            //    if (t.Area != "空中")
            //    {
            //        if (Map.TerrainHasObstacle(nx, ny))
            //        {
            //            is_crashed = true;
            //        }
            //    }

            //    i = (i + 1);
            //}

            //// ユニットを強制移動
            //if (tx != nx || ty != ny)
            //{
            //    GUI.EraseUnitBitmap(tx, ty);
            //    if (IsAnimationDefined("吹き飛ばし", sub_situation: ""))
            //    {
            //        PlayAnimation("吹き飛ばし", sub_situation: "");
            //    }
            //    else
            //    {
            //        GUI.MoveUnitBitmap(t, tx, ty, nx, ny, 20);
            //    }

            //    t.Jump(nx, ny, false);
            //}

            //// 激突
            //orig_dmg = dmg;
            //if (is_crashed)
            //{
            //    dmg = orig_dmg + GeneralLib.MaxLng((orig_dmg - t.get_Armor("") * t.MainPilot().Morale / 100) / 2, 0);

            //    // 最低ダメージ
            //    if (def_mode == "防御")
            //    {
            //        dmg = GeneralLib.MaxLng(dmg, 5);
            //    }
            //    else
            //    {
            //        dmg = GeneralLib.MaxLng(dmg, 10);
            //    }

            //    Sound.PlayWave("Crash.wav");
            //}

            //// 巻き添え
            //if (t2 is object && !ReferenceEquals(t2, t))
            //{
            //    dmg2 = (orig_dmg - t2.get_Armor("") * t2.MainPilot().Morale / 100) / 2;

            //    // 最低ダメージ
            //    if (dmg2 < 10)
            //    {
            //        dmg2 = 10;
            //    }

            //    // 無敵の場合はダメージを受けない
            //    if (t2.IsConditionSatisfied("無敵"))
            //    {
            //        dmg2 = 0;
            //    }

            //    // 各種処理がややこしくなるので巻き添えではユニットを破壊しない
            //    if (t2.HP - dmg2 < 10)
            //    {
            //        dmg2 = t2.HP - 10;
            //    }

            //    // ダメージ適用
            //    if (dmg2 > 0)
            //    {
            //        t2.HP = t2.HP - dmg2;
            //    }
            //    else
            //    {
            //        dmg2 = 0;
            //    }

            //    // ダメージ量表示
            //    if (!Expression.IsOptionDefined("ダメージ表示無効") || attack_mode == "マップ攻撃")
            //    {
            //        GUI.DrawSysString(t2.x, t2.y, SrcFormatter.Format(dmg2), true);
            //    }

            //    // 特殊能力「不安定」による暴走チェック
            //    if (t2.IsFeatureAvailable("不安定"))
            //    {
            //        if (t2.HP <= t2.MaxHP / 4 && !t2.IsConditionSatisfied("暴走"))
            //        {
            //            t2.AddCondition("暴走", -1, cdata: "");
            //            t2.Update();
            //        }
            //    }

            //    // ダメージを受ければ眠りからさめる
            //    if (t2.IsConditionSatisfied("睡眠") && !w.IsWeaponClassifiedAs("眠"))
            //    {
            //        t2.DeleteCondition("睡眠");
            //    }
            //}

            //msg = t.Nickname + "を吹き飛ばした。;" + msg;
            //if (is_critical)
            //{
            //    msg = "クリティカル！ " + msg;
            //}

            //// 吹き飛ばしが発生したことを伝える
            //critical_type = critical_type + " 吹き飛ばし";
            //CheckBlowAttackRet = true;
            //return CheckBlowAttackRet;
        }

        // 引き寄せチェック
        public bool CheckDrawAttack(UnitWeapon w, Unit t, string msg, string def_mode, string critical_type)
        {
            // TODO Impl
            return false;
            //bool CheckDrawAttackRet = default;
            //int tx = default, ty = default;
            //int sx, sy;
            //int nx, ny;
            //int prob;

            //// 特殊効果無効？
            //if (t.SpecialEffectImmune("引"))
            //{
            //    return CheckDrawAttackRet;
            //}

            //// 既に隣接している？
            //if (Math.Abs((x - tx)) + Math.Abs((y - ty)) == 1)
            //{
            //    return CheckDrawAttackRet;
            //}

            //// サイズによる制限
            //if (t.Size == "XL")
            //{
            //    return CheckDrawAttackRet;
            //}

            //// 固定物は動かせない
            //if (t.IsFeatureAvailable("地形ユニット"))
            //{
            //    return CheckDrawAttackRet;
            //}

            //if (t.Data.Speed == 0 && t.Speed == 0)
            //{
            //    return CheckDrawAttackRet;
            //}

            //// 自分自身は引き寄せない
            //if (ReferenceEquals(t, this))
            //{
            //    return CheckDrawAttackRet;
            //}

            //// 特殊効果発生確率
            //if (IsUnderSpecialPowerEffect("特殊効果発動"))
            //{
            //    prob = 100;
            //}
            //else
            //{
            //    prob = CriticalProbability(w, t, def_mode);
            //}

            //// 引き寄せ発生？
            //if (GeneralLib.Dice(100) > prob)
            //{
            //    return CheckDrawAttackRet;
            //}

            //// 引き寄せの中心座標を設定
            //if (w.WeaponLevel("Ｍ投") > 0d)
            //{
            //    sx = Commands.SelectedX;
            //    sy = Commands.SelectedY;
            //}
            //else
            //{
            //    sx = x;
            //    sy = y;
            //}

            //// ターゲットの座標
            //tx = t.x;
            //ty = t.y;

            //// 既に引き寄せの中心位置にいる？
            //if (sx == tx && sy == ty)
            //{
            //    return CheckDrawAttackRet;
            //}

            //// 引き寄せられる場所を設定
            //if (Map.MapDataForUnit[sx, sy] is null)
            //{
            //    nx = sx;
            //    ny = sy;
            //}
            //else if (Math.Abs((sx - tx)) > Math.Abs((sy - ty)))
            //{
            //    if (sx > tx)
            //    {
            //        nx = (sx - 1);
            //    }
            //    else
            //    {
            //        nx = (sx + 1);
            //    }

            //    ny = y;
            //    if (Map.MapDataForUnit[nx, ny] is object)
            //    {
            //        if (sy != ty)
            //        {
            //            if (sy > ty)
            //            {
            //                if (Map.MapDataForUnit[sx, sy - 1] is null)
            //                {
            //                    nx = sx;
            //                    ny = (sy - 1);
            //                }
            //                else if (Map.MapDataForUnit[nx, sy - 1] is null)
            //                {
            //                    ny = (sy - 1);
            //                }
            //            }
            //            else if (Map.MapDataForUnit[sx, sy + 1] is null)
            //            {
            //                nx = sx;
            //                ny = (sy + 1);
            //            }
            //            else if (Map.MapDataForUnit[nx, sy + 1] is null)
            //            {
            //                ny = (sy + 1);
            //            }
            //        }
            //    }
            //}
            //else if (Math.Abs((sx - tx)) < Math.Abs((sy - ty)))
            //{
            //    nx = sx;
            //    if (sy > ty)
            //    {
            //        ny = (sy - 1);
            //    }
            //    else
            //    {
            //        ny = (sy + 1);
            //    }

            //    if (Map.MapDataForUnit[nx, ny] is object)
            //    {
            //        if (sx != tx)
            //        {
            //            if (sx > tx)
            //            {
            //                if (Map.MapDataForUnit[sx - 1, sy] is null)
            //                {
            //                    nx = (sx - 1);
            //                    ny = sy;
            //                }
            //                else if (Map.MapDataForUnit[sx - 1, ny] is null)
            //                {
            //                    nx = (sx - 1);
            //                }
            //            }
            //            else if (Map.MapDataForUnit[sx + 1, sy] is null)
            //            {
            //                nx = (sx + 1);
            //                ny = sy;
            //            }
            //            else if (Map.MapDataForUnit[sx + 1, ny] is null)
            //            {
            //                nx = (sx + 1);
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    if (GeneralLib.Dice(2) == 1)
            //    {
            //        if (sx > tx)
            //        {
            //            nx = (sx - 1);
            //        }
            //        else
            //        {
            //            nx = (sx + 1);
            //        }

            //        ny = sy;
            //        if (Map.MapDataForUnit[nx, ny] is object)
            //        {
            //            nx = sx;
            //            if (sy > ty)
            //            {
            //                ny = (sy - 1);
            //            }
            //            else
            //            {
            //                ny = (sy + 1);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        nx = sx;
            //        if (sy > ty)
            //        {
            //            ny = (sy - 1);
            //        }
            //        else
            //        {
            //            ny = (sy + 1);
            //        }

            //        if (Map.MapDataForUnit[nx, ny] is object)
            //        {
            //            if (sx > tx)
            //            {
            //                nx = (sx - 1);
            //            }
            //            else
            //            {
            //                nx = (sx + 1);
            //            }

            //            ny = sy;
            //        }
            //    }

            //    if (Map.MapDataForUnit[nx, ny] is object)
            //    {
            //        if (sx > tx)
            //        {
            //            nx = (sx - 1);
            //        }
            //        else
            //        {
            //            nx = (sx + 1);
            //        }

            //        if (sy > ty)
            //        {
            //            ny = (sy - 1);
            //        }
            //        else
            //        {
            //            ny = (sy + 1);
            //        }
            //    }
            //}

            //// 結局動いてない？
            //if (nx == tx && ny == ty)
            //{
            //    return CheckDrawAttackRet;
            //}

            //// ユニットを強制移動
            //t.Jump(nx, ny);

            //// 本当に動いた？
            //if (t.x == tx && t.y == ty)
            //{
            //    return CheckDrawAttackRet;
            //}

            //msg = t.Nickname + "を引き寄せた。;" + msg;

            //// 引き寄せが発生したことを伝える
            //critical_type = critical_type + " 引き寄せ";
            //CheckDrawAttackRet = true;
            //return CheckDrawAttackRet;
        }

        // 強制転移チェック
        public bool CheckTeleportAwayAttack(UnitWeapon w, Unit t, string msg, string def_mode, string critical_type)
        {
            // TODO Impl
            return false;
            //bool CheckTeleportAwayAttackRet = default;
            //int tx, ty;
            //int nx = default, ny = default;
            //int d, prob, i;

            //// 特殊効果無効？
            //if (t.SpecialEffectImmune("転"))
            //{
            //    return CheckTeleportAwayAttackRet;
            //}

            //// サイズによる制限
            //if (t.Size == "XL")
            //{
            //    return CheckTeleportAwayAttackRet;
            //}

            //// 固定物は動かせない
            //if (t.IsFeatureAvailable("地形ユニット"))
            //{
            //    return CheckTeleportAwayAttackRet;
            //}

            //if (t.Data.Speed == 0 && t.Speed == 0)
            //{
            //    return CheckTeleportAwayAttackRet;
            //}

            //// 自分自身は強制転移出来ない
            //if (ReferenceEquals(t, this))
            //{
            //    return CheckTeleportAwayAttackRet;
            //}

            //// 特殊効果発生確率
            //if (IsUnderSpecialPowerEffect("特殊効果発動"))
            //{
            //    prob = 100;
            //}
            //else
            //{
            //    prob = CriticalProbability(w, t, def_mode);
            //}

            //// 強制転移発生？
            //if (GeneralLib.Dice(100) > prob)
            //{
            //    return CheckTeleportAwayAttackRet;
            //}

            //// 強制転移先を設定
            //tx = t.x;
            //ty = t.y;
            //for (i = 1; i <= 10; i++)
            //{
            //    d = GeneralLib.Dice(w.WeaponLevel("転"));
            //    if (GeneralLib.Dice(2) == 1)
            //    {
            //        nx = (tx + d);
            //    }
            //    else
            //    {
            //        nx = (tx - d);
            //    }

            //    d = (w.WeaponLevel("転") - d);
            //    if (GeneralLib.Dice(2) == 1)
            //    {
            //        ny = (ty + d);
            //    }
            //    else
            //    {
            //        ny = (ty - d);
            //    }

            //    if (1 <= nx && nx <= Map.MapWidth && 1 <= ny && ny <= Map.MapHeight)
            //    {
            //        break;
            //    }
            //}

            //// 転院先がない？
            //if (i > 10)
            //{
            //    return CheckTeleportAwayAttackRet;
            //}

            //// ユニットを強制移動
            //t.Jump(nx, ny);

            //// 本当に動いた？
            //if (t.x == tx && t.y == ty)
            //{
            //    return CheckTeleportAwayAttackRet;
            //}

            //msg = t.Nickname + "をテレポートさせた。;" + msg;

            //// 強制転移が発生したことを伝える
            //critical_type = critical_type + " 強制転移";
            //CheckTeleportAwayAttackRet = true;
            //return CheckTeleportAwayAttackRet;
        }

        // 能力コピーチェック
        public bool CheckMetamorphAttack(UnitWeapon w, Unit t, string def_mode)
        {
            // TODO Impl
            return false;
            //bool CheckMetamorphAttackRet = default;
            //int prob, wlv;
            //string uname;

            //// 既にコピー済み？
            //if (IsFeatureAvailable("ノーマルモード"))
            //{
            //    return CheckMetamorphAttackRet;
            //}

            //// 特殊効果無効？
            //if (t.SpecialEffectImmune("写"))
            //{
            //    return CheckMetamorphAttackRet;
            //}

            //// ボスユニットはコピー出来ない
            //if (t.BossRank >= 0)
            //{
            //    return CheckMetamorphAttackRet;
            //}

            //// 自分自身はコピー出来ない
            //if (ReferenceEquals(t, this))
            //{
            //    return CheckMetamorphAttackRet;
            //}

            //// サイズ制限
            //if (w.IsWeaponClassifiedAs("写"))
            //{
            //    switch (Size ?? "")
            //    {
            //        case "SS":
            //            {
            //                switch (t.Size ?? "")
            //                {
            //                    case "M":
            //                    case "L":
            //                    case "LL":
            //                    case "XL":
            //                        {
            //                            return CheckMetamorphAttackRet;
            //                        }
            //                }

            //                break;
            //            }

            //        case "S":
            //            {
            //                switch (t.Size ?? "")
            //                {
            //                    case "L":
            //                    case "LL":
            //                    case "XL":
            //                        {
            //                            return CheckMetamorphAttackRet;
            //                        }
            //                }

            //                break;
            //            }

            //        case "M":
            //            {
            //                switch (t.Size ?? "")
            //                {
            //                    case "SS":
            //                    case "LL":
            //                    case "XL":
            //                        {
            //                            return CheckMetamorphAttackRet;
            //                        }
            //                }

            //                break;
            //            }

            //        case "L":
            //            {
            //                switch (t.Size ?? "")
            //                {
            //                    case "SS":
            //                    case "S":
            //                    case "XL":
            //                        {
            //                            return CheckMetamorphAttackRet;
            //                        }
            //                }

            //                break;
            //            }

            //        case "LL":
            //            {
            //                switch (t.Size ?? "")
            //                {
            //                    case "SS":
            //                    case "S":
            //                    case "M":
            //                        {
            //                            return CheckMetamorphAttackRet;
            //                        }
            //                }

            //                break;
            //            }

            //        case "XL":
            //            {
            //                switch (t.Size ?? "")
            //                {
            //                    case "SS":
            //                    case "S":
            //                    case "M":
            //                    case "L":
            //                        {
            //                            return CheckMetamorphAttackRet;
            //                        }
            //                }

            //                break;
            //            }
            //    }
            //}

            //// 特殊効果発生確率
            //if (IsUnderSpecialPowerEffect("特殊効果発動"))
            //{
            //    prob = 100;
            //}
            //else
            //{
            //    prob = CriticalProbability(w, t, def_mode);
            //}

            //// コピー成功？
            //if (GeneralLib.Dice(100) > prob)
            //{
            //    return CheckMetamorphAttackRet;
            //}

            //// コピーしてしまうとその場にいれなくなってしまう？
            //Unit localOtherForm() { object argIndex1 = t.Name; var ret = OtherForm(argIndex1); t.Name = Conversions.ToString(argIndex1); return ret; }

            //if (!localOtherForm().IsAbleToEnter(x, y))
            //{
            //    return CheckMetamorphAttackRet;
            //}

            //// 変身前に情報を記録しておく
            //uname = Nickname;
            //wlv = GeneralLib.MaxLng(w.WeaponLevel("写"), (int)w.WeaponLevel("化"));

            //// 変身
            //Transform(t.Name);
            //t.Name = argnew_form;
            //{
            //    var withBlock = CurrentForm();
            //    // 元に戻れるように設定
            //    if (wlv > 0)
            //    {
            //        withBlock.AddCondition("残り時間", wlv, cdata: "");
            //    }

            //    withBlock.AddCondition("ノーマルモード付加", -1, 1d, Name + " 手動解除可");
            //    withBlock.AddCondition("能力コピー", -1, cdata: "");

            //    // コピー元のパイロット画像とメッセージを使うように設定
            //    withBlock.AddCondition("パイロット画像", -1, 0d, "非表示 " + t.MainPilot().get_Bitmap(false));
            //    withBlock.AddCondition("メッセージ", -1, 0d, "非表示 " + t.MainPilot().MessageType);
            //}

            //GUI.DisplaySysMessage(uname + "は" + t.Nickname + "に変身した。");

            //// 能力コピーが発生したことを伝える
            //CheckMetamorphAttackRet = true;
            //return CheckMetamorphAttackRet;
        }

        // 特殊能力 fdata1 と fdata2 が同じ名称か判定
        // 「中和」「相殺」用
        public bool IsSameCategory(string fdata1, string fdata2)
        {
            bool IsSameCategoryRet = default;
            string fc1, fc2;
            fc1 = GeneralLib.LIndex(fdata1, 1);
            // レベル指定を除く
            if (Strings.InStr(fc1, "Lv") > 0)
            {
                fc1 = Strings.Left(fc1, Strings.InStr(fc1, "Lv") - 1);
            }

            fc2 = GeneralLib.LIndex(fdata2, 1);
            // レベル指定を除く
            if (Strings.InStr(fc2, "Lv") > 0)
            {
                fc2 = Strings.Left(fc2, Strings.InStr(fc2, "Lv") - 1);
            }

            if ((fc1 ?? "") == (fc2 ?? ""))
            {
                IsSameCategoryRet = true;
            }

            return IsSameCategoryRet;
        }
    }
}
