// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Pilots;
using SRCCore.VB;
using System;
using System.Linq;

namespace SRCCore.Units
{
    public partial class Unit
    {
        // 経験値を入手
        // t:ターゲット
        // exp_situation:経験値入手の理由
        // exp_mode:マップ攻撃による入手？
        public int GetExp(Unit t, string exp_situation, string exp_mode = null)
        {
            // TODO Impl GetExp
            return 0;
            //int GetExpRet = default;
            //var xp = default(int);
            //int j, i, n;
            //int prev_level;
            //string[] prev_stype;
            //string[] prev_sname;
            //double[] prev_slevel;
            //string[] prev_special_power;
            //string stype, sname;
            //Pilot p;
            //string msg;

            //// 経験値を入手するのは味方ユニット及びＮＰＣの召喚ユニットのみ
            //if ((Party != "味方" || Party0 != "味方") && (Party != "ＮＰＣ" || Party0 != "ＮＰＣ" || !IsFeatureAvailable("召喚ユニット")))
            //{
            //    return GetExpRet;
            //}

            //// メインパイロットの現在の能力を記録
            //{
            //    var withBlock = MainPilot();
            //    prev_level = withBlock.Level;
            //    prev_special_power = new string[(withBlock.CountSpecialPower + 1)];
            //    var loopTo = withBlock.CountSpecialPower;
            //    for (i = 1; i <= loopTo; i++)
            //        prev_special_power[i] = withBlock.get_SpecialPower(i);
            //    prev_stype = new string[(withBlock.CountSkill() + 1)];
            //    prev_sname = new string[(withBlock.CountSkill() + 1)];
            //    prev_slevel = new double[(withBlock.CountSkill() + 1)];
            //    var loopTo1 = withBlock.CountSkill();
            //    for (i = 1; i <= loopTo1; i++)
            //    {
            //        prev_stype[i] = withBlock.Skill(i);
            //        prev_sname[i] = withBlock.SkillName(i);
            //        prev_slevel[i] = withBlock.SkillLevel(i, "基本値");
            //    }
            //}

            //// ターゲットが指定されていない場合は自分がターゲット
            //if (t is null)
            //{
            //    t = this;
            //}

            //// ターゲットにパイロットが乗っていない場合は経験値なし
            //if (t.CountPilot() == 0)
            //{
            //    return GetExpRet;
            //}

            //// ユニットに乗っているパイロット総数を計算
            //n = (int)(CountPilot() + CountSupport());
            //if (IsFeatureAvailable("追加サポート"))
            //{
            //    n = (int)(n + 1);
            //}

            //// 各パイロットが経験値を入手
            //var loopTo2 = n;
            //for (i = 1; i <= loopTo2; i++)
            //{
            //    if (i <= CountPilot())
            //    {
            //        p = Pilot(i);
            //    }
            //    else if (i <= (int)(CountPilot() + CountSupport()))
            //    {
            //        p = Support(i - CountPilot());
            //    }
            //    else
            //    {
            //        p = AdditionalSupport();
            //    }

            //    switch (exp_situation ?? "")
            //    {
            //        case "破壊":
            //            {
            //                xp = t.ExpValue + t.MainPilot().ExpValue;
            //                if (IsUnderSpecialPowerEffect("獲得経験値増加") && exp_mode != "パートナー")
            //                {
            //                    xp = (int)(xp * (1d + 0.1d * SpecialPowerEffectLevel("獲得経験値増加")));
            //                }

            //                break;
            //            }

            //        case "攻撃":
            //            {
            //                xp = (t.ExpValue + t.MainPilot().ExpValue) / 10;
            //                if (IsUnderSpecialPowerEffect("獲得経験値増加") && exp_mode != "パートナー")
            //                {
            //                    xp = (int)(xp * (1d + 0.1d * SpecialPowerEffectLevel("獲得経験値増加")));
            //                }

            //                break;
            //            }

            //        case "アビリティ":
            //            {
            //                if (ReferenceEquals(t, this))
            //                {
            //                    xp = 50;
            //                }
            //                else
            //                {
            //                    xp = 100;
            //                }

            //                break;
            //            }

            //        case "修理":
            //            {
            //                xp = 100;
            //                break;
            //            }

            //        case "補給":
            //            {
            //                xp = 150;
            //                break;
            //            }
            //    }

            //    if (!IsUnderSpecialPowerEffect("獲得経験値増加") || Expression.IsOptionDefined("収得効果重複"))
            //    {
            //        if (p.IsSkillAvailable("素質"))
            //        {
            //            if (p.IsSkillLevelSpecified("素質"))
            //            {
            //                xp = (int)((long)(xp * (10d + p.SkillLevel("素質", ref_mode: ""))) / 10L);
            //            }
            //            else
            //            {
            //                xp = (int)(1.5d * xp);
            //            }
            //        }
            //    }

            //    if (p.IsSkillAvailable("遅成長"))
            //    {
            //        xp = xp / 2;
            //    }

            //    // 対象のパイロットのレベル差による修正
            //    switch ((int)(t.MainPilot().Level - p.Level))
            //    {
            //        case var @case when @case > 7:
            //            {
            //                xp = 5 * xp;
            //                break;
            //            }

            //        case 7:
            //            {
            //                xp = (int)(4.5d * xp);
            //                break;
            //            }

            //        case 6:
            //            {
            //                xp = 4 * xp;
            //                break;
            //            }

            //        case 5:
            //            {
            //                xp = (int)(3.5d * xp);
            //                break;
            //            }

            //        case 4:
            //            {
            //                xp = 3 * xp;
            //                break;
            //            }

            //        case 3:
            //            {
            //                xp = (int)(2.5d * xp);
            //                break;
            //            }

            //        case 2:
            //            {
            //                xp = 2 * xp;
            //                break;
            //            }

            //        case 1:
            //            {
            //                xp = (int)(1.5d * xp);
            //                break;
            //            }

            //        case 0:
            //            {
            //                break;
            //            }

            //        case -1:
            //            {
            //                xp = xp / 2;
            //                break;
            //            }

            //        case -2:
            //            {
            //                xp = xp / 4;
            //                break;
            //            }

            //        case -3:
            //            {
            //                xp = xp / 6;
            //                break;
            //            }

            //        case -4:
            //            {
            //                xp = xp / 8;
            //                break;
            //            }

            //        case -5:
            //            {
            //                xp = xp / 10;
            //                break;
            //            }

            //        case var case1 when case1 < -5:
            //            {
            //                xp = xp / 12;
            //                break;
            //            }
            //    }

            //    p.Exp = p.Exp + xp;

            //    // 一番目のパイロットが獲得した経験値を返す
            //    if (i == 1)
            //    {
            //        GetExpRet = xp;
            //    }
            //}

            //// 追加パイロットの場合、一番目のパイロットにレベル、経験値を合わせる
            //if (!ReferenceEquals(MainPilot(), Pilot(1)))
            //{
            //    MainPilot().Level = Pilot(1).Level;
            //    MainPilot().Exp = Pilot(1).Exp;
            //}

            //// 召喚主も経験値を入手
            //if (Summoner is object)
            //{
            //    Summoner.CurrentForm().GetExp(t, exp_situation, "パートナー");
            //}

            //// マップ攻撃による経験値収得の場合はメッセージ表示を省略
            //if (exp_mode == "マップ")
            //{
            //    return GetExpRet;
            //}

            //// 経験値入手時のメッセージ
            //{
            //    var withBlock1 = MainPilot();
            //    if (withBlock1.Level > prev_level)
            //    {
            //        // レベルアップ

            //        if (IsAnimationDefined("レベルアップ", sub_situation: ""))
            //        {
            //            PlayAnimation("レベルアップ", sub_situation: "");
            //        }
            //        else if (IsSpecialEffectDefined("レベルアップ", sub_situation: ""))
            //        {
            //            SpecialEffect("レベルアップ", sub_situation: "");
            //        }

            //        if (IsMessageDefined("レベルアップ"))
            //        {
            //            PilotMessage("レベルアップ", msg_mode: "");
            //        }

            //        msg = withBlock1.get_Nickname(false) + "は経験値[" + SrcFormatter.Format(GetExpRet) + "]を獲得、" + "レベル[" + SrcFormatter.Format(withBlock1.Level) + "]にレベルアップ。";

            //        // 特殊能力の習得
            //        var loopTo3 = withBlock1.CountSkill();
            //        for (i = 1; i <= loopTo3; i++)
            //        {
            //            stype = withBlock1.Skill(i);
            //            sname = withBlock1.SkillName(i);
            //            if (Strings.InStr(sname, "非表示") == 0)
            //            {
            //                switch (stype ?? "")
            //                {
            //                    case "同調率":
            //                    case "霊力":
            //                    case "追加レベル":
            //                    case "魔力所有":
            //                        {
            //                            break;
            //                        }

            //                    case "ＳＰ消費減少":
            //                    case "スペシャルパワー自動発動":
            //                    case "ハンター":
            //                        {
            //                            var loopTo4 = (int)Information.UBound(prev_stype);
            //                            for (j = 1; j <= loopTo4; j++)
            //                            {
            //                                if ((stype ?? "") == (prev_stype[j] ?? ""))
            //                                {
            //                                    if ((sname ?? "") == (prev_sname[j] ?? ""))
            //                                    {
            //                                        break;
            //                                    }
            //                                }
            //                            }

            //                            if (j > Information.UBound(prev_stype))
            //                            {
            //                                msg = msg + ";" + sname + "を習得した。";
            //                            }

            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            var loopTo5 = (int)Information.UBound(prev_stype);
            //                            for (j = 1; j <= loopTo5; j++)
            //                            {
            //                                if ((stype ?? "") == (prev_stype[j] ?? ""))
            //                                {
            //                                    break;
            //                                }
            //                            }

            //                            double localSkillLevel() { object argIndex1 = i; string argref_mode = "基本値"; var ret = withBlock1.SkillLevel(argIndex1, argref_mode); return ret; }

            //                            if (j > Information.UBound(prev_stype))
            //                            {
            //                                msg = msg + ";" + sname + "を習得した。";
            //                            }
            //                            else if (localSkillLevel() > prev_slevel[j])
            //                            {
            //                                msg = msg + ";" + prev_sname[j] + " => " + sname + "。";
            //                            }

            //                            break;
            //                        }
            //                }
            //            }
            //        }

            //        // スペシャルパワーの習得
            //        if (withBlock1.CountSpecialPower > Information.UBound(prev_special_power))
            //        {
            //            msg = msg + ";" + Expression.Term("スペシャルパワー", this);
            //            var loopTo6 = withBlock1.CountSpecialPower;
            //            for (i = 1; i <= loopTo6; i++)
            //            {
            //                sname = withBlock1.get_SpecialPower(i);
            //                var loopTo7 = (int)Information.UBound(prev_special_power);
            //                for (j = 1; j <= loopTo7; j++)
            //                {
            //                    if ((sname ?? "") == (prev_special_power[j] ?? ""))
            //                    {
            //                        break;
            //                    }
            //                }

            //                if (j > Information.UBound(prev_special_power))
            //                {
            //                    msg = msg + "「" + sname + "」";
            //                }
            //            }

            //            msg = msg + "を習得した。";
            //        }

            //        GUI.DisplaySysMessage(msg);
            //        if (GUI.MessageWait < 10000)
            //        {
            //            GUI.Sleep(GUI.MessageWait);
            //        }

            //        Event.HandleEvent("レベルアップ", withBlock1.ID);
            //        SRC.PList.UpdateSupportMod(this);
            //    }
            //    else if (GetExpRet > 0)
            //    {
            //        GUI.DisplaySysMessage(withBlock1.get_Nickname(false) + "は" + SrcFormatter.Format(GetExpRet) + "の経験値を得た。");
            //    }
            //}

            //return GetExpRet;
        }

        // ユニットの陣営を変更
        public void ChangeParty(string new_party)
        {
            // 陣営を変更
            Party = new_party;

            // パイロットの陣営を変更
            foreach (var p in Pilots.Concat(SupportPilots))
            {
                p.Party = new_party;
            }

            if (IsFeatureAvailable("追加サポート"))
            {
                AdditionalSupport().Party = new_party;
            }

            // 他形態の陣営を変更
            foreach (var of in OtherForms)
            {
                of.ChangeParty(new_party);
            }

            // 出撃中？
            if (Status == "出撃")
            {
                // 自分の陣営のステージなら行動可能に
                if ((Party ?? "") == (SRC.Stage ?? ""))
                {
                    Rest();
                }
                // マップ上のユニット画像を更新
                GUI.PaintUnitBitmap(this);
            }

            SRC.PList.UpdateSupportMod(this);

            // 思考モードを通常に
            Mode = "通常";
        }

        // ユニットに乗っているパイロットの気力をnumだけ増減
        // is_event:イベントによる気力増減(性格を無視して気力操作)
        public void IncreaseMorale(int num, bool is_event = false)
        {
            Pilot p;
            if (CountPilot() == 0)
            {
                return;
            }

            // メインパイロット
            {
                var withBlock = MainPilot();
                if (withBlock.Personality != "機械" || is_event)
                {
                    withBlock.Morale = withBlock.Morale + num;
                }
            }

            // サブパイロット
            foreach (Pilot currentP in colPilot)
            {
                p = currentP;
                if ((MainPilot().ID ?? "") != (p.ID ?? "") && (p.Personality != "機械" || is_event))
                {
                    p.Morale = p.Morale + num;
                }
            }

            // サポート
            foreach (Pilot currentP1 in colSupport)
            {
                p = currentP1;
                if (p.Personality != "機械" || is_event)
                {
                    p.Morale = p.Morale + num;
                }
            }

            // 追加サポート
            if (IsFeatureAvailable("追加サポート"))
            {
                {
                    var withBlock1 = AdditionalSupport();
                    if (withBlock1.Personality != "機械" || is_event)
                    {
                        withBlock1.Morale = withBlock1.Morale + num;
                    }
                }
            }
        }

        // ユニットが破壊された時の処理
        public void Die(bool without_update = false)
        {
            HP = 0;
            Status = "破壊";

            // 破壊をキャンセルし、破壊イベント内で処理をしたい場合
            if (IsConditionSatisfied("破壊キャンセル"))
            {
                DeleteCondition("破壊キャンセル");
                goto SkipExplode;
            }

            Map.MapDataForUnit[x, y] = null;

            // 爆発表示
            GUI.ClearPicture();
            if (IsAnimationDefined("脱出", sub_situation: ""))
            {
                GUI.EraseUnitBitmap(x, y, false);
                PlayAnimation("脱出", sub_situation: "");
            }
            else if (IsSpecialEffectDefined("脱出", sub_situation: ""))
            {
                GUI.EraseUnitBitmap(x, y, false);
                SpecialEffect("脱出", sub_situation: "");
            }
            else
            {
                Effect.DieAnimation(this);
            }

        SkipExplode:
            ;

            // 召喚したユニットを解放
            DismissServant();

            // 魅了・憑依したユニットを解放
            DismissSlave();
            if (Master is object)
            {
                Master.CurrentForm().DeleteSlave(ID);
                Master = null;
            }

            if (Summoner is object)
            {
                Summoner.CurrentForm().DeleteServant(ID);
                Summoner = null;
            }

            // 支配しているユニットを強制退却
            if (IsFeatureAvailable("支配"))
            {
                var fd = Feature("支配");
                foreach (var pname in fd.DataL.Skip(1))
                {
                    foreach (Pilot p in SRC.PList.Items)
                    {
                        if ((p.Name ?? "") == (pname ?? "") || (p.get_Nickname(false) ?? "") == (pname ?? ""))
                        {
                            if (p.Unit is object)
                            {
                                if (p.Unit.Status == "出撃" || p.Unit.Status == "格納")
                                {
                                    p.Unit.Die(true);
                                }
                            }
                        }
                    }
                }
            }

            // 情報更新
            if (!without_update)
            {
                SRC.PList.UpdateSupportMod(this);
            }
        }

        // スペシャルパワー自爆による自爆
        public void SuicidalExplosion(bool is_event = false)
        {
            PilotMessage("自爆", msg_mode: "");
            GUI.DisplaySysMessage(Nickname + "は自爆した。");

            // ダメージ量設定
            var dmg = HP;

            // 効果範囲の設定
            Map.AreaInRange(x, y, 1, 1, "");
            Map.MaskData[x, y] = true;

            // 爆発
            GUI.EraseUnitBitmap(x, y);
            Effect.ExplodeAnimation(Size, x, y);

            // パーツ分離できれば自爆後にパーツ分離
            if (IsFeatureAvailable("パーツ分離"))
            {
                var uname = GeneralLib.LIndex(FeatureData("パーツ分離"), 2);
                if (OtherForm(uname).IsAbleToEnter(x, y))
                {
                    Transform(uname);
                    {
                        var cf = CurrentForm();
                        cf.HP = cf.MaxHP;
                        cf.UsedAction = cf.MaxAction();
                    }

                    var fname = FeatureName("パーツ分離");
                    if (!SysMessageIfDefined(new string[]
                    {
                            "破壊時分離(" + Name + ")",
                            "破壊時分離(" + fname + ")",
                            "破壊時分離",
                            "分離(" + Name + ")",
                            "分離(" + fname + ")",
                            "分離",
                    }))
                    {
                        GUI.DisplaySysMessage(Nickname + "は破壊されたパーツを分離させた。");
                    }
                    goto SkipSuicide;
                }
            }

            // 自分を破壊
            HP = 0;
            GUI.UpdateMessageForm(this, u2: null);
            // 既に爆発アニメーションを表示しているので
            AddCondition("破壊キャンセル", 1, cdata: "");
            Map.MapDataForUnit[x, y] = null;
            Die();
            if (!is_event)
            {
                var u = Commands.SelectedUnit;
                Commands.SelectedUnit = this;
                Event.HandleEvent("破壊", MainPilot().ID);
                Commands.SelectedUnit = u;
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    return;
                }
            }

        SkipSuicide:
            ;


            // 周りのエリアに爆発効果を適用
            var loopTo = Map.MapWidth;
            for (var i = 1; i <= loopTo; i++)
            {
                var loopTo1 = Map.MapHeight;
                for (var j = 1; j <= loopTo1; j++)
                {
                    if (Map.MaskData[i, j])
                    {
                        goto NextLoop;
                    }

                    var t = Map.MapDataForUnit[i, j];
                    if (t is null)
                    {
                        goto NextLoop;
                    }

                    {
                        GUI.ClearMessageForm();
                        if (CurrentForm().Party == "味方" || CurrentForm().Party == "ＮＰＣ")
                        {
                            GUI.UpdateMessageForm(t, CurrentForm());
                        }
                        else
                        {
                            GUI.UpdateMessageForm(CurrentForm(), t);
                        }

                        // ダメージの適用
                        var prev_hp = t.HP;
                        int tdmg;
                        if (t.IsConditionSatisfied("無敵"))
                        {
                            tdmg = 0;
                        }
                        else if (t.IsConditionSatisfied("不死身"))
                        {
                            t.HP = GeneralLib.MaxLng(t.HP - dmg, 10);
                            tdmg = prev_hp - t.HP;
                        }
                        else
                        {
                            t.HP = t.HP - dmg;
                            tdmg = prev_hp - t.HP;
                        }

                        // 特殊能力「不安定」による暴走チェック
                        if (t.IsFeatureAvailable("不安定"))
                        {
                            if (t.HP <= t.MaxHP / 4 && !t.IsConditionSatisfied("暴走"))
                            {
                                t.AddCondition("暴走", -1, cdata: "");
                                t.Update();
                            }
                        }

                        // ダメージを受ければ眠りからさめる
                        if (t.IsConditionSatisfied("睡眠"))
                        {
                            t.DeleteCondition("睡眠");
                        }

                        if (CurrentForm().Party == "味方" || CurrentForm().Party == "ＮＰＣ")
                        {
                            GUI.UpdateMessageForm(t, CurrentForm());
                        }
                        else
                        {
                            GUI.UpdateMessageForm(CurrentForm(), t);
                        }

                        if (t.HP > 0)
                        {
                            GUI.DrawSysString(t.x, t.y, SrcFormatter.Format(tdmg));
                            GUI.RefreshScreen();
                        }

                        if (t.HP == 0)
                        {
                            if (t.IsUnderSpecialPowerEffect("復活"))
                            {
                                t.HP = t.MaxHP;
                                t.RemoveSpecialPowerInEffect("破壊");
                                GUI.DisplaySysMessage(t.Nickname + "は復活した！");
                                goto NextLoop;
                            }

                            if (t.IsFeatureAvailable("パーツ分離"))
                            {
                                var uname = GeneralLib.LIndex(t.FeatureData("パーツ分離"), 2);
                                if (t.OtherForm(uname).IsAbleToEnter(t.x, t.y))
                                {
                                    t.Transform(uname);
                                    {
                                        var cf = t.CurrentForm();
                                        cf.HP = cf.MaxHP;
                                        cf.UsedAction = cf.MaxAction();
                                    }

                                    GUI.DisplaySysMessage(t.Nickname + "は破壊されたパーツを分離させた。");
                                    goto NextLoop;
                                }
                            }

                            t.Die();
                        }

                        if (!is_event)
                        {
                            var u = Commands.SelectedUnit;
                            Commands.SelectedUnit = t.CurrentForm();
                            Commands.SelectedTarget = this;
                            if (t.Status == "破壊")
                            {
                                GUI.DisplaySysMessage((string)(t.Nickname + "は破壊された"));
                                Event.HandleEvent("破壊", t.MainPilot().ID);
                            }
                            else
                            {
                                GUI.DisplaySysMessage(t.Nickname + "は" + tdmg + "のダメージを受けた。;" + "残りＨＰは" + SrcFormatter.Format(t.HP) + "（損傷率 = " + 100 * (t.MaxHP - t.HP) / t.MaxHP + "％）");
                                Event.HandleEvent("損傷率", t.MainPilot().ID, "" + (int)(100 - t.HP * 100 / t.MaxHP));
                            }

                            Commands.SelectedUnit = u;
                            if (SRC.IsScenarioFinished)
                            {
                                SRC.IsScenarioFinished = false;
                                return;
                            }
                        }
                    }

                NextLoop:
                    ;
                }
            }
        }
    }
}
