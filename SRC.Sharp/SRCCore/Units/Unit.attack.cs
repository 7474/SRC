// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Items;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Pilots;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Units
{
    // === 攻撃関連処理 ===
    public partial class Unit
    {
        // 武器 w でユニット t に攻撃
        // attack_mode は攻撃の種類
        // def_mode はユニット t の防御態勢
        // is_event はイベント(Attackコマンド)による攻撃かどうかを現す
        public void Attack(UnitWeapon w, Unit t, string attack_mode, string def_mode, bool is_event = false)
        {
            int prob;
            int dmg, prev_hp;
            bool is_hit = default, is_critical;
            string critical_type;
            bool use_shield, use_shield_msg;
            bool is_penetrated;
            bool use_protect_msg;
            bool use_support_guard;
            string wname, wnickname;
            string msg, buf;
            int k, i, j;
            Unit su = default, orig_t;
            //Unit[] partners;
            int tx, ty;
            string tarea;
            int prev_x = default, prev_y = default;
            var prev_area = default(string);
            var second_attack = default(bool);
            bool be_quiet;
            int attack_num = default, hit_count = default;
            int slevel;
            Unit saved_selected_unit;
            double hp_ratio, en_ratio;
            bool separate_parts;
            int orig_w;
            bool is_ext_anime_defined;

            wname = w.Name;
            wnickname = w.WeaponNickname();

            // メッセージ表示用に選択状況を切り替え
            Commands.SaveSelections();
            saved_selected_unit = Commands.SelectedUnit;
            if (attack_mode == "反射")
            {
                Commands.SelectedUnit = Commands.SelectedTarget;
                Commands.SelectedTarget = this;
                Event.SelectedUnitForEvent = Event.SelectedTargetForEvent;
                Event.SelectedTargetForEvent = this;
                Commands.SelectedWeapon = w.WeaponNo();
                Commands.SelectedWeaponName = wname;
            }
            else
            {
                if (ReferenceEquals(Commands.SelectedUnit, t))
                {
                    Commands.SelectedTWeapon = Commands.SelectedWeapon;
                    Commands.SelectedTWeaponName = Commands.SelectedWeaponName;
                }

                Commands.SelectedWeapon = w.WeaponNo();
                Commands.SelectedWeaponName = wname;
                Commands.SelectedUnit = this;
                Commands.SelectedTarget = t;
                Event.SelectedUnitForEvent = this;
                Event.SelectedTargetForEvent = t;
            }

            // サポートガードを行ったユニットに関する情報をクリア
            if (!IsDefense())
            {
                Commands.SupportGuardUnit = null;
                Commands.SupportGuardUnit2 = null;
            }

            // パイロットのセリフを表示するかどうかを判定
            if (attack_mode == "マップ攻撃" || attack_mode == "反射" || attack_mode == "当て身技" || attack_mode == "自動反撃")
            {
                be_quiet = true;
            }
            else
            {
                be_quiet = false;
            }

            // 戦闘アニメを表示する場合はマップウィンドウをクリアする
            if (SRC.BattleAnimation)
            {
                //if (GUI.MainWidth != 15)
                //{
                //    Status.ClearUnitStatus();
                //}

                if (!Expression.IsOptionDefined("戦闘中画面初期化無効"))
                {
                    GUI.RedrawScreen();
                }
            }

            orig_t = t;

            // かばった時にターゲットの位置を元のターゲットの位置と一致させるため記録
            tx = t.x;
            ty = t.y;
            tarea = t.Area;
        begin:
            ;

            // 情報を更新
            Update();
            MainPilot().UpdateSupportMod();
            t.Update();
            t.MainPilot().UpdateSupportMod();

            // ダメージ表示のため、ターゲットのＨＰを記録しておく
            prev_hp = t.HP;

            // 各種設定をリセット
            msg = "";
            is_critical = false;
            critical_type = "";
            use_shield = false;
            use_shield_msg = false;
            use_protect_msg = false;
            use_support_guard = false;
            is_penetrated = false;

            // 命中率を算出
            prob = w.HitProbability(t, true);

            // ダメージを算出
            dmg = w.Damage(t, true, Strings.InStr(attack_mode, "援護攻撃") > 0);

            // 特殊効果を持たない武器ならクリティカルの可能性あり
            if (w.IsNormalWeapon() && dmg > 0)
            {
                if (w.CriticalProbability(t, def_mode) >= GeneralLib.Dice(100)
                    || attack_mode == "統率"
                    || attack_mode == "同時援護攻撃")
                {
                    is_critical = true;
                }
            }

            IList<Unit> partners = new List<Unit>();
            Commands.SelectedPartners = new Unit[0];
            if (attack_mode != "マップ攻撃" && attack_mode != "反射" && !second_attack)
            {
                if (w.IsWeaponClassifiedAs("合"))
                {
                    // 合体技の場合にパートナーをハイライト表示
                    if (w.WeaponMaxRange() == 1)
                    {
                        partners = w.CombinationPartner("武装", tx, ty);
                    }
                    else
                    {
                        partners = w.CombinationPartner("武装");
                    }

                    foreach (var pu in partners)
                    {
                        Map.MaskData[pu.x, pu.y] = false;
                    }

                    if (!SRC.BattleAnimation)
                    {
                        GUI.MaskScreen();
                    }
                }
                else if (!is_critical && dmg > 0 && Strings.InStr(attack_mode, "援護攻撃") == 0)
                {
                    // 連携攻撃が発動するかを判定
                    // （連携攻撃は合体技では発動しない）
                    if (w.WeaponData.MaxRange > 1)
                    {
                        su = LookForAttackHelp(x, y);
                    }
                    else
                    {
                        su = LookForAttackHelp(tx, ty);
                    }

                    if (su != null)
                    {
                        // 連携攻撃発動
                        Map.MaskData[su.x, su.y] = false;
                        if (!SRC.BattleAnimation)
                        {
                            GUI.MaskScreen();
                        }

                        if (IsMessageDefined("連携攻撃(" + su.MainPilot().Name + ")", true))
                        {
                            PilotMessage("連携攻撃(" + su.MainPilot().Name + ")", msg_mode: "");
                        }
                        else
                        {
                            PilotMessage("連携攻撃(" + su.MainPilot().get_Nickname(false) + ")", msg_mode: "");
                        }

                        is_critical = true;
                        su = null;
                    }
                }
            }

            // クリティカルならダメージ1.5倍
            if (is_critical)
            {
                if (Expression.IsOptionDefined("ダメージ倍率低下"))
                {
                    if (w.IsWeaponClassifiedAs("痛"))
                    {
                        dmg = (int)((1d + 0.1d * (w.WeaponLevel("痛") + 2d)) * dmg);
                    }
                    else
                    {
                        dmg = (int)(1.2d * dmg);
                    }
                }
                else
                {
                    if (w.IsWeaponClassifiedAs("痛"))
                    {
                        dmg = (int)((1d + 0.25d * (w.WeaponLevel("痛") + 2d)) * dmg);
                    }
                    else
                    {
                        dmg = (int)(1.5d * dmg);
                    }
                }
            }

            // 攻撃種類のアニメ表示
            if (SRC.BattleAnimation)
            {
                switch (attack_mode ?? "")
                {
                    case "援護攻撃":
                    case "同時援護攻撃":
                        {
                            Effect.ShowAnimation("援護攻撃発動");
                            break;
                        }

                    case "カウンター":
                        {
                            Effect.ShowAnimation("カウンター発動");
                            break;
                        }
                }
            }

            // 攻撃側のメッセージ表示
            if (!be_quiet)
            {
                // 攻撃準備の効果音
                if (IsAnimationDefined(wname + "(準備)", sub_situation: ""))
                {
                    PlayAnimation(wname + "(準備)", sub_situation: "");
                }
                else if (IsAnimationDefined(wname, sub_situation: "") && !Expression.IsOptionDefined("武器準備アニメ非表示") && SRC.WeaponAnimation)
                {
                    PlayAnimation(wname + "(準備)", sub_situation: "");
                }
                else if (IsSpecialEffectDefined(wname + "(準備)", sub_situation: ""))
                {
                    SpecialEffect(wname + "(準備)", sub_situation: "");
                }
                else
                {
                    Effect.PrepareWeaponEffect(this, w);
                }

                // 攻撃メッセージの前に出力されるメッセージ
                if (second_attack)
                {
                    PilotMessage("再攻撃", msg_mode: "");
                }
                else if (Strings.InStr(attack_mode, "援護攻撃") > 0)
                {
                    {
                        var withBlock1 = Commands.AttackUnit.CurrentForm().MainPilot();
                        bool localIsMessageDefined() { string argmain_situation = "サポートアタック(" + withBlock1.get_Nickname(false) + ")"; var ret = IsMessageDefined(argmain_situation); return ret; }

                        if (IsMessageDefined("サポートアタック(" + withBlock1.Name + ")"))
                        {
                            PilotMessage("サポートアタック(" + withBlock1.Name + ")", msg_mode: "");
                        }
                        else if (localIsMessageDefined())
                        {
                            PilotMessage("サポートアタック(" + withBlock1.get_Nickname(false) + ")", msg_mode: "");
                        }
                        else if (IsMessageDefined("サポートアタック"))
                        {
                            PilotMessage("サポートアタック", msg_mode: "");
                        }
                    }
                }
                else if (attack_mode == "カウンター")
                {
                    PilotMessage("カウンター", msg_mode: "");
                }
                else if (IsMessageDefined(wname) && wname != "格闘" && wname != "射撃" && wname != "攻撃" && !w.IsWeaponClassifiedAs("合"))
                {
                    if (IsMessageDefined("かけ声(" + wname + ")"))
                    {
                        PilotMessage("かけ声(" + wname + ")", msg_mode: "");
                    }
                    else if (IsDefense())
                    {
                        PilotMessage("かけ声(反撃)", msg_mode: "");
                    }
                    else
                    {
                        PilotMessage("かけ声", msg_mode: "");
                    }
                }

                // 攻撃メッセージ
                Sound.IsWavePlayed = false;
                if (!second_attack)
                {
                    if (attack_mode == "カウンター")
                    {
                        PilotMessage(wname, "カウンター");
                    }
                    else
                    {
                        PilotMessage(wname, "攻撃");
                    }
                }

                // 攻撃アニメ
                if (IsDefense() && IsAnimationDefined(wname + "(反撃)", ""))
                {
                    PlayAnimation(wname + "(反撃)", sub_situation: "");
                }
                else if (IsAnimationDefined(wname + "(攻撃)")
                    || IsAnimationDefined(wname, sub_situation: ""))
                {
                    PlayAnimation(wname + "(攻撃)", sub_situation: "");
                }
                else if (IsSpecialEffectDefined(wname, sub_situation: ""))
                {
                    SpecialEffect(wname, sub_situation: "");
                }
                else if (!Sound.IsWavePlayed)
                {
                    Effect.AttackEffect(this, w);
                }
            }
            else if (attack_mode == "自動反撃")
            {
                // 攻撃アニメ
                if (IsDefense() && IsAnimationDefined(wname + "(反撃)", ""))
                {
                    PlayAnimation(wname + "(反撃)", sub_situation: "");
                }
                else if (IsAnimationDefined(wname + "(攻撃)", "") || IsAnimationDefined(wname, sub_situation: ""))
                {
                    PlayAnimation(wname + "(攻撃)", sub_situation: "");
                }
                else if (IsSpecialEffectDefined(wname, sub_situation: ""))
                {
                    SpecialEffect(wname, sub_situation: "");
                }
                else if (!Sound.IsWavePlayed)
                {
                    Effect.AttackEffect(this, w);
                }
            }

            if (attack_mode != "マップ攻撃" && attack_mode != "反射")
            {
                // 武器使用による弾数＆ＥＮの消費
                w.UseWeapon();
                // 武器使用によるＥＮ消費の表示
                GUI.UpdateMessageForm(this, t);
            }

            // 防御手段による命中率低下
            if (def_mode == "回避")
            {
                if (!IsUnderSpecialPowerEffect("絶対命中")
                    && !t.IsUnderSpecialPowerEffect("無防備")
                    && !t.IsFeatureAvailable("回避不可")
                    && !t.IsConditionSatisfied("移動不能"))
                {
                    prob = (prob / 2);
                }
            }

            // 反射攻撃の場合は命中率が低下
            if (attack_mode == "反射")
            {
                prob = (prob / 2);
            }

            // 攻撃を行ったことについてのシステムメッセージ
            if (!be_quiet)
            {
                switch (partners.Count)
                {
                    case 0:
                        {
                            // 通常攻撃
                            msg = Nickname + "は";
                            break;
                        }

                    case 1:
                        {
                            // ２体合体攻撃
                            if ((Nickname ?? "") != (partners[1].Nickname ?? ""))
                            {
                                msg = Nickname + "は[" + partners[1].Nickname + "]と共に";
                            }
                            else if ((MainPilot().get_Nickname(false) ?? "") != (partners[1].MainPilot().get_Nickname(false) ?? ""))
                            {
                                msg = MainPilot().get_Nickname(false) + "と[" + partners[1].MainPilot().get_Nickname(false) + "]は";
                            }
                            else
                            {
                                msg = Nickname + "達は";
                            }

                            break;
                        }

                    case 2:
                        {
                            // ３体合体攻撃
                            if ((Nickname ?? "") != (partners[1].Nickname ?? ""))
                            {
                                msg = Nickname + "は[" + partners[1].Nickname + "]、[" + partners[2].Nickname + "]と共に";
                            }
                            else if ((MainPilot().get_Nickname(false) ?? "") != (partners[1].MainPilot().get_Nickname(false) ?? ""))
                            {
                                msg = MainPilot().get_Nickname(false) + "は[" + partners[1].MainPilot().get_Nickname(false) + "]、[" + partners[2].MainPilot().get_Nickname(false) + "]と共に";
                            }
                            else
                            {
                                msg = Nickname + "達は";
                            }

                            break;
                        }

                    default:
                        {
                            // ３体以上による合体攻撃
                            msg = Nickname + "達は";
                            break;
                        }
                }

                // ジャンプ攻撃
                if (t.Area == "空中" && (w.IsWeaponClassifiedAs("武") || w.IsWeaponClassifiedAs("突") || w.IsWeaponClassifiedAs("接")) && !IsTransAvailable("空"))
                {
                    msg = msg + "ジャンプし、";
                }

                if (second_attack)
                {
                    msg = msg + "再度";
                }
                else if (attack_mode == "カウンター" || attack_mode == "先制攻撃")
                {
                    msg = "先制攻撃！;" + msg + "先手を取り";
                }

                // 攻撃の種類によってメッセージを切り替え
                if (Strings.Right(wnickname, 2) == "攻撃" || Strings.Right(wnickname, 4) == "アタック" || wnickname == "突撃")
                {
                    msg = msg + "[" + wnickname + "]をかけた。;";
                }
                else if (w.IsSpellWeapon())
                {
                    if (Strings.Right(wnickname, 2) == "呪文")
                    {
                        msg = msg + "[" + wnickname + "]を唱えた。;";
                    }
                    else if (Strings.Right(wnickname, 2) == "の杖")
                    {
                        msg = msg + "[" + Strings.Left(wnickname, Strings.Len(wnickname) - 2) + "]の呪文を唱えた。;";
                    }
                    else
                    {
                        msg = msg + "[" + wnickname + "]の呪文を唱えた。;";
                    }
                }
                else if (w.IsWeaponClassifiedAs("盗"))
                {
                    msg = msg + "[" + t.Nickname + "]の持ち物を盗もうとした。;";
                }
                else if (w.IsWeaponClassifiedAs("習"))
                {
                    msg = msg + "[" + t.Nickname + "]の技を習得しようと試みた。;";
                }
                else if (w.IsWeaponClassifiedAs("実") && (Strings.InStr(wnickname, "ミサイル") > 0 || Strings.InStr(wnickname, "ロケット") > 0))
                {
                    msg = msg + "[" + wnickname + "]を発射した。;";
                }
                else if (Strings.Right(wnickname, 1) == "息" || Strings.Right(wnickname, 3) == "ブレス" || Strings.Right(wnickname, 2) == "光線" || Strings.Right(wnickname, 1) == "光" || Strings.Right(wnickname, 3) == "ビーム" || Strings.Right(wnickname, 4) == "レーザー")
                {
                    msg = msg + "[" + wnickname + "]を放った。;";
                }
                else if (Strings.Right(wnickname, 1) == "歌")
                {
                    msg = msg + "[" + wnickname + "]を歌った。;";
                }
                else if (Strings.Right(wnickname, 2) == "踊り")
                {
                    msg = msg + "[" + wnickname + "]を踊った。;";
                }
                else
                {
                    msg = msg + "[" + wnickname + "]で攻撃をかけた。;";
                }

                // 命中率＆ＣＴ率表示
                if (is_event)
                {
                    // イベントによる攻撃の場合は命中率をスペシャルパワーの影響を含めずに表示
                    if (def_mode == "回避")
                    {
                        buf = "命中率 = " + GeneralLib.MinLng(w.HitProbability(t, false) / 2, 100) + "％" + "（" + SrcFormatter.Format(w.CriticalProbability(t, def_mode)) + "％）";
                    }
                    else
                    {
                        buf = "命中率 = " + GeneralLib.MinLng(w.HitProbability(t, false), 100) + "％" + "（" + SrcFormatter.Format(w.CriticalProbability(t, def_mode)) + "％）";
                    }
                }
                else
                {
                    buf = "命中率 = " + GeneralLib.MinLng(prob, 100) + "％" + "（" + SrcFormatter.Format(w.CriticalProbability(t, def_mode)) + "％）";
                }

                // 攻撃解説表示
                if (IsSysMessageDefined(wname, sub_situation: ""))
                {
                    // 「武器名(解説)」のメッセージを使用
                    SysMessage(wname, "", buf);
                }
                else if (IsSysMessageDefined("攻撃", sub_situation: ""))
                {
                    // 「攻撃(解説)」のメッセージを使用
                    SysMessage("攻撃", "", buf);
                }
                else
                {
                    GUI.DisplaySysMessage(msg + buf, SRC.BattleAnimation);
                }
            }

            msg = "";

            // 防御方法を表示
            switch (def_mode ?? "")
            {
                case "回避":
                    {
                        if (t.IsConditionSatisfied("踊り"))
                        {
                            msg = t.Nickname + "は踊っている。;";
                        }
                        else
                        {
                            msg = t.Nickname + "は回避運動をとった。;";
                        }

                        break;
                    }

                case "防御":
                    {
                        msg = t.Nickname + "は防御行動をとった。;";
                        break;
                    }
            }

            // スペシャルパワー「必殺」「瀕死」
            if (IsUnderSpecialPowerEffect("絶対破壊") || IsUnderSpecialPowerEffect("絶対瀕死"))
            {
                if (!be_quiet)
                {
                    PilotMessage(wname + "(命中)", msg_mode: "");
                }

                if (IsAnimationDefined(wname + "(命中)", sub_situation: "") || IsAnimationDefined(wname, sub_situation: ""))
                {
                    PlayAnimation(wname + "(命中)", sub_situation: "");
                }
                else if (IsSpecialEffectDefined(wname + "(命中)", ""))
                {
                    SpecialEffect(wname + "(命中)", sub_situation: "");
                }
                else if (!Sound.IsWavePlayed)
                {
                    Effect.HitEffect(this, w, t);
                }

                if (IsUnderSpecialPowerEffect("絶対瀕死"))
                {
                    if (Expression.IsOptionDefined("ダメージ下限解除") || Expression.IsOptionDefined("ダメージ下限１"))
                    {
                        if (t.HP > 1)
                        {
                            dmg = t.HP - 1;
                        }
                        else
                        {
                            dmg = 0;
                        }
                    }
                    else if (t.HP > 10)
                    {
                        dmg = t.HP - 10;
                    }
                    else
                    {
                        dmg = 0;
                    }
                }

                else
                {
                    dmg = t.HP;
                }

                goto ApplyDamage;
            }

            // 回避能力の処理
            if (prob > 0)
            {
                if (CheckDodgeFeature(w, t, tx, ty, attack_mode, def_mode, dmg, be_quiet))
                {
                    dmg = 0;
                    goto EndAttack;
                }
            }

            // 攻撃回数を求める
            if (w.IsWeaponClassifiedAs("連"))
            {
                attack_num = (int)w.WeaponLevel("連");
                // TODO レベル取得してないから0除算しちゃうの回避しておく
                attack_num = Math.Max(attack_num, 1);
            }
            else
            {
                attack_num = 1;
            }

            // 命中回数を求める
            hit_count = 0;
            for (i = 1; i <= attack_num; i++)
            {
                if (GeneralLib.Dice(100) <= prob)
                {
                    hit_count = hit_count + 1;
                }
            }
            // 命中回数に基いてダメージを修正
            dmg = dmg * hit_count / attack_num;

            // 攻撃回避時の処理
            if (hit_count == 0)
            {
                if (IsAnimationDefined(wname + "(回避)", sub_situation: ""))
                {
                    PlayAnimation(wname + "(回避)", sub_situation: "");
                }
                else if (IsSpecialEffectDefined(wname + "(回避)", ""))
                {
                    SpecialEffect(wname + "(回避)", sub_situation: "");
                }
                else if (t.IsAnimationDefined("回避", sub_situation: ""))
                {
                    t.PlayAnimation("回避", sub_situation: "");
                }
                else if (t.IsSpecialEffectDefined("回避", sub_situation: ""))
                {
                    t.SpecialEffect("回避", sub_situation: "");
                }
                else
                {
                    Effect.DodgeEffect(this, w);
                }

                if (!be_quiet)
                {
                    t.PilotMessage("回避", msg_mode: "");
                    PilotMessage(wname + "(回避)", msg_mode: "");
                }

                if (t.IsSysMessageDefined("回避", sub_situation: ""))
                {
                    t.SysMessage("回避", sub_situation: "", add_msg: "");
                }
                else
                {
                    switch (def_mode ?? "")
                    {
                        case "回避":
                            {
                                if (t.IsConditionSatisfied("踊り"))
                                {
                                    GUI.DisplaySysMessage(t.Nickname + "は激しく踊りながら攻撃をかわした。");
                                }
                                else
                                {
                                    GUI.DisplaySysMessage(t.Nickname + "は回避運動をとり、攻撃をかわした。");
                                }

                                break;
                            }

                        case "防御":
                            {
                                GUI.DisplaySysMessage(t.Nickname + "は防御行動をとったが、攻撃は外れた。");
                                break;
                            }

                        default:
                            {
                                GUI.DisplaySysMessage(t.Nickname + "は攻撃をかわした。");
                                break;
                            }
                    }
                }

                goto EndAttack;
            }

            // 敵ユニットがかばわれた場合の処理
            if (su == null)
            {
                use_support_guard = false;
                if (t.IsUnderSpecialPowerEffect("みがわり"))
                {
                    // スペシャルパワー「みがわり」
                    i = 1;
                    while (i <= t.CountSpecialPower())
                    {
                        if (t.SpecialPower(i).IsEffectAvailable("みがわり"))
                        {
                            if (SRC.PList.IsDefined(t.SpecialPowerData(i)))
                            {
                                su = SRC.PList.Item(t.SpecialPowerData(i)).Unit;
                                t.RemoveSpecialPowerInEffect("みがわり");
                                i = (i - 1);
                                if (su != null)
                                {
                                    su = su.CurrentForm();
                                    if (su.Status != "出撃")
                                    {
                                        su = null;
                                    }
                                }
                            }
                        }
                        i = (i + 1);
                    }
                }
                else if (!is_event && def_mode != "マップ攻撃" && def_mode != "援護防御")
                {
                    if (t.IsDefense())
                    {
                        // サポートガード
                        if (Commands.UseSupportGuard)
                        {
                            su = t.LookForSupportGuard(this, w);
                            if (su != null)
                            {
                                use_support_guard = true;
                                // サポートガードの残り回数を減らす
                                su.UsedSupportGuard = (su.UsedSupportGuard + 1);
                            }
                        }
                    }

                    if (su == null)
                    {
                        // かばう
                        su = t.LookForGuardHelp(this, w, is_critical);
                    }
                }

                if (su != null)
                {
                    su.Update();

                    // メッセージウィンドウの表示を入れ替え
                    if (Party == "味方" || Party == "ＮＰＣ")
                    {
                        GUI.UpdateMessageForm(su, this);
                    }
                    else
                    {
                        GUI.UpdateMessageForm(this, su);
                    }

                    if (!SRC.BattleAnimation)
                    {
                        // 身代わりになるユニットをハイライト表示
                        if (Map.MaskData[su.x, su.y])
                        {
                            Map.MaskData[su.x, su.y] = false;
                            GUI.MaskScreen();
                            Map.MaskData[su.x, su.y] = true;
                        }
                    }

                    // かばう際のメッセージ
                    if (use_support_guard)
                    {
                        if (su.IsMessageDefined("サポートガード(" + t.MainPilot().Name + ")"))
                        {
                            su.PilotMessage("サポートガード(" + t.MainPilot().Name + ")", msg_mode: "");
                        }
                        else if (su.IsMessageDefined("サポートガード(" + t.MainPilot().get_Nickname(false) + ")"))
                        {
                            su.PilotMessage("サポートガード(" + t.MainPilot().get_Nickname(false) + ")", msg_mode: "");
                        }
                        else if (su.IsMessageDefined("サポートガード"))
                        {
                            su.PilotMessage("サポートガード", msg_mode: "");
                        }
                    }
                    else if (su.IsMessageDefined("かばう(" + t.MainPilot().Name + ")"))
                    {
                        su.PilotMessage("かばう(" + t.MainPilot().Name + ")", msg_mode: "");
                        use_protect_msg = true;
                    }
                    else if (su.IsMessageDefined("かばう(" + t.MainPilot().get_Nickname(false) + ")"))
                    {
                        su.PilotMessage("かばう(" + t.MainPilot().get_Nickname(false) + ")", msg_mode: "");
                        use_protect_msg = true;
                    }

                    msg = su.MainPilot().get_Nickname(false) + "は[" + t.MainPilot().get_Nickname(false) + "]をかばった。;";

                    // 身代わりになるユニットをターゲットの位置まで移動
                    {
                        var withBlock2 = su;
                        // アニメ表示
                        if (SRC.BattleAnimation)
                        {
                            if (su.IsAnimationDefined("サポートガード開始", sub_situation: ""))
                            {
                                su.PlayAnimation("サポートガード開始", sub_situation: "");
                            }
                            else if (!GUI.IsRButtonPressed())
                            {
                                if (use_support_guard)
                                {
                                    GUI.MoveUnitBitmap(su, withBlock2.x, withBlock2.y, tx, ty, 80, 4);
                                }
                                else
                                {
                                    GUI.MoveUnitBitmap(su, withBlock2.x, withBlock2.y, tx, ty, 50);
                                }
                            }
                        }

                        Map.MapDataForUnit[withBlock2.x, withBlock2.y] = null;
                        prev_x = withBlock2.x;
                        prev_y = withBlock2.y;
                        prev_area = withBlock2.Area;
                        withBlock2.x = tx;
                        withBlock2.y = ty;
                        withBlock2.Area = tarea;
                        Map.MapDataForUnit[withBlock2.x, withBlock2.y] = su;
                    }

                    // ターゲットを再設定
                    t = su;
                    Commands.SelectedTarget = t;
                    Event.SelectedTargetForEvent = t;
                }
            }

            if (su != null)
            {
                // ダメージを再計算
                {
                    prev_hp = t.HP;
                    dmg = w.Damage(t, true);
                    if (is_critical)
                    {
                        if (Expression.IsOptionDefined("ダメージ倍率低下"))
                        {
                            if (w.IsWeaponClassifiedAs("痛"))
                            {
                                dmg = (int)((1d + 0.1d * (w.WeaponLevel("痛") + 2d)) * dmg);
                            }
                            else
                            {
                                dmg = (int)(1.2d * dmg);
                            }
                        }
                        else
                        {
                            if (w.IsWeaponClassifiedAs("痛"))
                            {
                                dmg = (int)((1d + 0.25d * (w.WeaponLevel("痛") + 2d)) * dmg);
                            }
                            else
                            {
                                dmg = (int)(1.5d * dmg);
                            }
                        }
                    }
                }

                // かばう場合は常に全弾命中
                hit_count = attack_num;

                // 常に防御モードに設定
                def_mode = "防御";

                // サポートガードを行うユニットに関する情報を記録
                if (IsDefense())
                {
                    Commands.SupportGuardUnit2 = su;
                    Commands.SupportGuardUnitHPRatio2 = su.HP / (double)su.MaxHP;
                }
                else
                {
                    Commands.SupportGuardUnit = su;
                    Commands.SupportGuardUnitHPRatio = su.HP / (double)su.MaxHP;
                }
            }

            // 受けの処理
            if (CheckParryFeature(w, t, tx, ty, attack_mode, def_mode, dmg, msg, be_quiet || use_protect_msg))
            {
                dmg = 0;
                goto EndAttack;
            }

            // 防御＆かばう時はダメージを半減
            if (!w.IsWeaponClassifiedAs("殺"))
            {
                if (def_mode == "防御" && !t.IsUnderSpecialPowerEffect("無防備") && !t.IsFeatureAvailable("防御不可"))
                {
                    dmg = dmg / 2;
                }
            }

            // ダミー
            if (CheckDummyFeature(w, t, be_quiet))
            {
                dmg = 0;
                goto EndAttack;
            }

            // これ以降は命中時の処理
            is_hit = true;

            // シールド防御判定
            // XXX dmg 書き換えてそう
            CheckShieldFeature(w, t, dmg, be_quiet, use_shield, use_shield_msg);

            // 防御能力の処理
            if (CheckDefenseFeature(w, t, tx, ty, attack_mode, def_mode, dmg, msg, be_quiet || use_protect_msg, is_penetrated))
            {
                if (!be_quiet)
                {
                    PilotMessage(wname + "(攻撃無効化)", msg_mode: "");
                }

                dmg = 0;
                goto EndAttack;
            }

            // 命中時の特殊効果を表示。
            // 防御能力の処理を先に行うのは攻撃無効化の特殊効果を優先させるため。
            Sound.IsWavePlayed = false;
            if (!be_quiet)
            {
                PilotMessage(wname + "(命中)", msg_mode: "");
            }

            if (IsAnimationDefined(wname + "(命中)", sub_situation: "") || IsAnimationDefined(wname, sub_situation: ""))
            {
                PlayAnimation(wname + "(命中)", sub_situation: "");
            }
            else if (IsSpecialEffectDefined(wname + "(命中)"))
            {
                SpecialEffect(wname + "(命中)", sub_situation: "");
            }
            else if (!Sound.IsWavePlayed)
            {
                Effect.HitEffect(this, w, t, hit_count);
            }

            SysMessage(wname + "(命中)", sub_situation: "", add_msg: "");

            // 無敵の場合
            if (t.IsConditionSatisfied("無敵"))
            {
                if (!be_quiet)
                {
                    t.PilotMessage("攻撃無効化", msg_mode: "");
                    PilotMessage(wname + "(攻撃無効化)", msg_mode: "");
                }

                GUI.DisplaySysMessage(msg + t.Nickname + "は[" + wnickname + "]を無効化した！");
                dmg = 0;
                goto EndAttack;
            }

            // 抹殺攻撃は一撃で倒せる場合にしか効かない
            if (w.IsWeaponClassifiedAs("殺"))
            {
                if (t.HP > dmg)
                {
                    GUI.DisplaySysMessage(msg + t.Nickname + "は攻撃による影響を受けなかった。");
                    goto EndAttack;
                }
            }

            // ターゲット位置を変更する攻撃はサポートガードの場合は無効
            if (su == null && def_mode != "援護防御")
            {
                // 吹き飛ばし
                if (w.IsWeaponClassifiedAs("吹") || w.IsWeaponClassifiedAs("Ｋ"))
                {
                    CheckBlowAttack(w, t, dmg, msg, attack_mode, def_mode, critical_type);
                }

                // 引き寄せ
                if (w.IsWeaponClassifiedAs("引"))
                {
                    CheckDrawAttack(w, t, msg, def_mode, critical_type);
                }

                // 強制転移
                if (w.IsWeaponClassifiedAs("転"))
                {
                    CheckTeleportAwayAttack(w, t, msg, def_mode, critical_type);
                }
            }

            // クリティカルメッセージはこの時点で追加
            if (is_critical)
            {
                msg = msg + "クリティカル！;";
            }

            // シールド防御の効果適用
            int spower;
            if (use_shield)
            {
                if (w.IsWeaponClassifiedAs("破"))
                {
                    if (t.IsFeatureAvailable("小型シールド"))
                    {
                        dmg = 5 * dmg / 6;
                    }
                    else
                    {
                        dmg = 3 * dmg / 4;
                    }
                }
                else
                {
                    if (t.IsFeatureAvailable("小型シールド"))
                    {
                        dmg = 2 * dmg / 3;
                    }
                    else
                    {
                        dmg = dmg / 2;
                    }
                }

                if (t.IsFeatureAvailable("エネルギーシールド") && t.EN > 5 && !w.IsWeaponClassifiedAs("無") && !IsUnderSpecialPowerEffect("防御能力無効化"))
                {
                    t.EN = t.EN - 5;
                    if (w.IsWeaponClassifiedAs("破"))
                    {
                        spower = (int)(50d * t.FeatureLevel("エネルギーシールド"));
                    }
                    else
                    {
                        spower = (int)(100d * t.FeatureLevel("エネルギーシールド"));
                    }

                    if (dmg <= spower)
                    {
                        if (attack_mode != "反射")
                        {
                            GUI.UpdateMessageForm(this, t);
                        }
                        else
                        {
                            GUI.UpdateMessageForm(this, null);
                        }

                        var fname = t.FeatureName0("エネルギーシールド");
                        if (!be_quiet)
                        {
                            if (t.IsMessageDefined("攻撃無効化(" + fname + ")"))
                            {
                                t.PilotMessage("攻撃無効化(" + fname + ")", msg_mode: "");
                            }
                            else
                            {
                                t.PilotMessage("攻撃無効化", msg_mode: "");
                            }
                        }

                        if (t.IsAnimationDefined("攻撃無効化", fname))
                        {
                            t.PlayAnimation("攻撃無効化", fname);
                        }
                        else
                        {
                            t.SpecialEffect("攻撃無効化", fname);
                        }

                        GUI.DisplaySysMessage(msg + fname + "が攻撃を防いだ。");
                        goto EndAttack;
                    }

                    dmg = dmg - spower;
                }
            }

            // 最低ダメージは10
            if (dmg > 0 && dmg < 10)
            {
                dmg = 10;
            }

            // 都合により破壊させない場合
            if (IsUnderSpecialPowerEffect("てかげん") && this.MainPilot().Technique > t.MainPilot().Technique && Strings.InStr(attack_mode, "援護攻撃") == 0 || t.IsConditionSatisfied("不死身"))
            {
                if (t.HP <= 10)
                {
                    dmg = 0;
                }
                else if (t.HP - dmg < 10)
                {
                    dmg = t.HP - 10;
                }
            }

            // 特殊効果
            CauseEffect(w, t, msg, critical_type, def_mode, dmg >= t.HP);
            if (Strings.InStr(critical_type, "即死") > 0 && !use_support_guard && !use_protect_msg)
            {
                if (t.IsHero())
                {
                    msg = msg + w.WeaponNickname() + "が" + t.Nickname + "の命を奪った。;";
                }
                else
                {
                    msg = msg + w.WeaponNickname() + "が" + t.Nickname + "を一撃で破壊した。;";
                }

                dmg = t.HP;
            }
            else if (t.HP - dmg < 0)
            {
                dmg = t.HP;
            }


            // 確実に発生する特殊効果
            int prev_en;
            if (w.IsWeaponClassifiedAs("減") && !t.SpecialEffectImmune("減"))
            {
                msg = msg + wnickname + "が[" + t.Nickname + "]の" + Expression.Term("ＥＮ", t) + "を低下させた。;";
                t.EN = GeneralLib.MaxLng((int)(t.EN - t.MaxEN * (dmg / (double)t.MaxHP)), 0);
            }
            else if (w.IsWeaponClassifiedAs("奪") && !t.SpecialEffectImmune("奪"))
            {
                msg = msg + Nickname + "は[" + t.Nickname + "]の" + Expression.Term("ＥＮ", t) + "を吸収した。;";
                prev_en = t.EN;
                t.EN = GeneralLib.MaxLng((int)(t.EN - t.MaxEN * (dmg / (double)t.MaxHP)), 0);
                EN = EN + (prev_en - t.EN) / 2;
            }

            // クリティカル時メッセージ
            if (is_critical || Strings.Len(critical_type) > 0)
            {
                if (!be_quiet)
                {
                    PilotMessage(wname + "(クリティカル)", msg_mode: "");
                }

                if (IsAnimationDefined(wname + "(クリティカル)", sub_situation: ""))
                {
                    PlayAnimation(wname + "(クリティカル)", sub_situation: "");
                }
                else if (IsSpecialEffectDefined(wname + "(クリティカル)"))
                {
                    SpecialEffect(wname + "(クリティカル)", sub_situation: "");
                }
                else
                {
                    Effect.CriticalEffect(critical_type, w, use_support_guard || use_protect_msg);
                }
            }

        ApplyDamage:
            ;

            //// XXX 仮メッセージ
            //GUI.DisplaySysMessage(
            //    $"{Name}({w.Name}) -> {t.Name}" +
            //    Environment.NewLine +
            //    $"{prob}%...{(is_hit ? "Hit" : "Miss")} {dmg}",
            //    SRC.BattleAnimation);

            // ダメージの適用
            t.HP = t.HP - dmg;
            {
                // ＨＰ吸収
                if (w.IsWeaponClassifiedAs("吸") && !t.SpecialEffectImmune("吸"))
                {
                    if (HP < MaxHP)
                    {
                        msg = msg + Nickname + "は[" + t.Nickname + "]の" + Expression.Term("ＨＰ", t) + "を吸収した。;";
                        HP = HP + (prev_hp - t.HP) / 4;
                    }
                }

                // マップ攻撃の場合はメッセージが表示されないので
                // その代わりに少しディレイを入れる
                if (def_mode == "マップ攻撃")
                {
                    GUI.Sleep(150);
                }

                // ダメージによるＨＰゲージ減少を表示
                if (attack_mode != "反射")
                {
                    GUI.UpdateMessageForm(this, t);
                }
                else
                {
                    // XXX これ多分機能しない
                    GUI.UpdateMessageForm(this, null);
                }

                // ダメージ量表示前にカットインは一旦消去しておく
                if (!Expression.IsOptionDefined("戦闘中画面初期化無効") || attack_mode == "マップ攻撃")
                {
                    if (GUI.IsPictureVisible)
                    {
                        GUI.ClearPicture();
                        GUI.RedrawScreen();
                    }
                }

                // ダメージ量をマップウィンドウに表示
                if (!Expression.IsOptionDefined("ダメージ表示無効") || attack_mode == "マップ攻撃")
                {
                    if (IsAnimationDefined(wname + "(ダメージ表示)", sub_situation: ""))
                    {
                        PlayAnimation(wname + "(ダメージ表示)", sub_situation: "");
                    }
                    else if (IsAnimationDefined("ダメージ表示", sub_situation: ""))
                    {
                        PlayAnimation("ダメージ表示", sub_situation: "");
                    }
                    else
                    {
                        if (!SRC.BattleAnimation || w.WeaponPower("") > 0 || dmg > 0)
                        {
                            if (!SRC.BattleAnimation && su != null)
                            {
                                GUI.DrawSysString(prev_x, prev_y, SrcFormatter.Format(dmg));
                            }
                            else
                            {
                                GUI.DrawSysString(t.x, t.y, SrcFormatter.Format(dmg));
                            }
                        }
                    }
                }

                // 自動反撃発動
                if (t.HP > 0)
                {
                    CheckAutoAttack(w, t, attack_mode, def_mode, dmg, be_quiet || use_protect_msg);
                    if (Status != "出撃")
                    {
                        goto EndAttack;
                    }
                }

                // 破壊アニメ
                if (t.HP == 0)
                {
                    if (t.IsAnimationDefined("破壊", sub_situation: ""))
                    {
                        t.PlayAnimation("破壊", sub_situation: "");
                    }
                    else
                    {
                        t.SpecialEffect("破壊", sub_situation: "");
                    }
                }

                // パーツ分離が発動可能かチェック
                separate_parts = false;
                if (t.HP == 0)
                {
                    if (t.IsFeatureAvailable("パーツ分離"))
                    {
                        if (t.OtherForm(GeneralLib.LIndex(t.FeatureData("パーツ分離"), 2)).IsAbleToEnter(t.x, t.y))
                        {
                            if (t.IsFeatureLevelSpecified("パーツ分離"))
                            {
                                if (GeneralLib.Dice(100) <= 10d * t.FeatureLevel("パーツ分離"))
                                {
                                    separate_parts = true;
                                }
                            }
                            else
                            {
                                separate_parts = true;
                            }
                        }
                    }
                }

                // 破壊メッセージ
                if (attack_mode != "マップ攻撃" && !use_protect_msg && !use_shield_msg)
                {
                    if (t.HP == 0)
                    {
                        if (separate_parts)
                        {
                            var fname = t.FeatureName("パーツ分離");
                            if (t.IsMessageDefined("破壊時分離(" + t.Name + ")"))
                            {
                                t.PilotMessage("破壊時分離(" + t.Name + ")", msg_mode: "");
                            }
                            else if (t.IsMessageDefined("破壊時分離(" + fname + ")"))
                            {
                                t.PilotMessage("破壊時分離(" + fname + ")", msg_mode: "");
                            }
                            else if (t.IsMessageDefined("破壊時"))
                            {
                                t.PilotMessage("破壊時分離", msg_mode: "");
                            }
                            else if (t.IsMessageDefined("分離(" + t.Name + ")"))
                            {
                                t.PilotMessage("分離(" + t.Name + ")", msg_mode: "");
                            }
                            else if (t.IsMessageDefined("分離(" + fname + ")"))
                            {
                                t.PilotMessage("分離(" + fname + ")", msg_mode: "");
                            }
                            else if (t.IsMessageDefined("分離"))
                            {
                                t.PilotMessage("分離", msg_mode: "");
                            }
                            else
                            {
                                t.PilotMessage("ダメージ大", msg_mode: "");
                            }
                        }
                        else
                        {
                            t.PilotMessage("破壊", msg_mode: "");
                        }
                    }
                }

                if (!be_quiet)
                {
                    if (t.HP == 0)
                    {
                        // とどめメッセージ
                        PilotMessage(wname + "(とどめ)", msg_mode: "");
                    }
                    else
                    {
                        // ダメージメッセージ
                        PilotMessage(wname + "(ダメージ)", msg_mode: "");
                    }
                }

                // ダメージアニメ
                if (t.HP == 0)
                {
                    // どどめアニメ
                    if (attack_mode != "マップ攻撃" && attack_mode != "反射")
                    {
                        if (IsAnimationDefined(wname + "(とどめ)", sub_situation: ""))
                        {
                            PlayAnimation(wname + "(とどめ)", sub_situation: "");
                        }
                        else
                        {
                            SpecialEffect(wname + "(とどめ)", sub_situation: "");
                        }
                    }
                }
                else if ((dmg <= 0.05d * t.MaxHP && t.HP >= 0.25d * t.MaxHP || dmg <= 10) && Strings.Len(critical_type) == 0)
                {
                    // ダメージが非常に小さい
                    if (t.IsAnimationDefined("ダメージ小", sub_situation: ""))
                    {
                        t.PlayAnimation("ダメージ小", sub_situation: "");
                    }
                    else
                    {
                        t.SpecialEffect("ダメージ小", sub_situation: "");
                    }
                }
                else if (t.HP < 0.25d * t.MaxHP)
                {
                    // ダメージ大
                    if (t.IsAnimationDefined("ダメージ大", sub_situation: ""))
                    {
                        t.PlayAnimation("ダメージ大", sub_situation: "");
                    }
                    else
                    {
                        t.SpecialEffect("ダメージ大", sub_situation: "");
                    }
                }
                else if (t.HP > 0.8d * t.MaxHP && Strings.Len(critical_type) == 0)
                {
                    // ダメージ小
                    if (t.IsAnimationDefined("ダメージ小", sub_situation: ""))
                    {
                        t.PlayAnimation("ダメージ小", sub_situation: "");
                    }
                    else
                    {
                        t.SpecialEffect("ダメージ小", sub_situation: "");
                    }
                }
                else
                {
                    // ダメージ中
                    if (t.IsAnimationDefined("ダメージ中", sub_situation: ""))
                    {
                        t.PlayAnimation("ダメージ中", sub_situation: "");
                    }
                    else
                    {
                        t.SpecialEffect("ダメージ中", sub_situation: "");
                    }
                }

                // ダメージメッセージ
                if (attack_mode != "マップ攻撃" && !use_protect_msg && !use_shield_msg)
                {
                    if (t.HP == 0)
                    {
                        // 破壊時メッセージは既に表示している
                    }
                    else if ((dmg <= 0.05d * t.MaxHP && t.HP >= 0.25d * t.MaxHP || dmg <= 10) && Strings.Len(critical_type) == 0)
                    {
                        // ダメージが非常に小さい
                        t.PilotMessage("ダメージ小", msg_mode: "");
                    }
                    else if (t.HP < 0.25d * t.MaxHP)
                    {
                        // ダメージ大
                        t.PilotMessage("ダメージ大", msg_mode: "");
                    }
                    else if (is_penetrated && t.IsMessageDefined("バリア貫通"))
                    {
                        t.PilotMessage("バリア貫通", msg_mode: "");
                    }
                    else if (t.HP >= 0.8d * t.MaxHP && Strings.Len(critical_type) == 0)
                    {
                        // ステータス異常が起こった場合は最低でもダメージ中のメッセージ
                        t.PilotMessage("ダメージ小", msg_mode: "");
                    }
                    else
                    {
                        t.PilotMessage("ダメージ中", msg_mode: "");
                    }
                }

                // シールド防御
                if (use_shield && t.HP > 0)
                {
                    if (t.IsFeatureAvailable("シールド"))
                    {
                        var fname = t.FeatureName("シールド");
                        if (t.IsSysMessageDefined("シールド防御", fname))
                        {
                            t.SysMessage("シールド防御", fname, add_msg: "");
                        }
                        else
                        {
                            msg = msg + t.Nickname + "は[" + fname + "]で防御した。;";
                        }
                    }
                    else if (t.IsFeatureAvailable("エネルギーシールド") && t.EN > 5 && !w.IsWeaponClassifiedAs("無") && !IsUnderSpecialPowerEffect("防御能力無効化"))
                    {
                        t.EN = t.EN - 5;
                        var fname = t.FeatureName("エネルギーシールド");
                        if (t.IsSysMessageDefined("シールド防御", fname))
                        {
                            t.SysMessage("シールド防御", fname, add_msg: "");
                        }
                        else
                        {
                            msg = msg + t.Nickname + "は[" + fname + "]を展開した。;";
                        }
                    }
                    else if (t.IsFeatureAvailable("小型シールド"))
                    {
                        var fname = t.FeatureName("小型シールド");
                        if (t.IsSysMessageDefined("シールド防御", fname))
                        {
                            t.SysMessage("シールド防御", fname, add_msg: "");
                        }
                        else
                        {
                            msg = msg + t.Nickname + "は[" + fname + "]で防御した。;";
                        }
                    }
                    else if (t.IsFeatureAvailable("大型シールド"))
                    {
                        var fname = t.FeatureName("大型シールド");
                        if (t.IsSysMessageDefined("シールド防御", fname))
                        {
                            t.SysMessage("シールド防御", fname, add_msg: "");
                        }
                        else
                        {
                            msg = msg + t.Nickname + "は[" + fname + "]で防御した。;";
                        }
                    }
                    else if (t.IsFeatureAvailable("アクティブシールド"))
                    {
                        var fname = t.FeatureName("アクティブシールド");
                        if (t.IsSysMessageDefined("シールド防御", fname))
                        {
                            t.SysMessage("シールド防御", fname, add_msg: "");
                        }
                        else if (!t.IsHero())
                        {
                            msg = msg + t.Nickname + "の[" + fname + "]が機体を守った。;";
                        }
                        else
                        {
                            msg = msg + fname + "が[" + t.Nickname + "]を守った。;";
                        }
                    }
                }

                // ターゲットが生き残った場合の処理
                if (t.HP > 0)
                {
                    if (dmg == 0)
                    {
                        if (Strings.Len(critical_type) > 0)
                        {
                            GUI.DisplaySysMessage(msg);
                        }
                        else if (w.IsWeaponClassifiedAs("盗"))
                        {
                            // 盗み失敗
                            if (t.IsConditionSatisfied("すかんぴん"))
                            {
                                GUI.DisplaySysMessage(msg + t.Nickname + "は盗める物を持っていなかった。");
                            }
                            else
                            {
                                GUI.DisplaySysMessage(msg + t.Nickname + "は素早く持ち物を守った。");
                            }
                        }
                        else if (w.IsWeaponClassifiedAs("習"))
                        {
                            // ラーニング失敗
                            if (t.IsFeatureAvailable("ラーニング可能技"))
                            {
                                buf = t.FeatureData("ラーニング可能技");
                                string fname;
                                switch (GeneralLib.LIndex(buf, 2) ?? "")
                                {
                                    case "表示":
                                    case var @case when @case == "":
                                        {
                                            fname = GeneralLib.LIndex(buf, 1);
                                            break;
                                        }

                                    default:
                                        {
                                            fname = GeneralLib.LIndex(buf, 2);
                                            break;
                                        }
                                }

                                if (MainPilot().IsSkillAvailable(GeneralLib.LIndex(buf, 1)))
                                {
                                    GUI.DisplaySysMessage(msg + MainPilot().get_Nickname(false) + "は「" + fname + "」を既に習得していた。");
                                }
                                else
                                {
                                    GUI.DisplaySysMessage(msg + MainPilot().get_Nickname(false) + "は「" + fname + "」を習得出来なかった。");
                                }
                            }
                            else
                            {
                                GUI.DisplaySysMessage(msg + t.Nickname + "は習得可能な技を持っていなかった。");
                            }
                        }
                        else if (w.IsWeaponClassifiedAs("写") || w.IsWeaponClassifiedAs("化"))
                        {
                        }
                        // 能力コピーの判定はこれから
                        else
                        {
                            GUI.DisplaySysMessage(msg + t.Nickname + "は攻撃による影響を受けなかった。");
                        }
                    }
                    else if (t.IsConditionSatisfied("データ不明"))
                    {
                        if (attack_num > 1)
                        {
                            msg = msg + SrcFormatter.Format(hit_count) + "回命中し、";
                        }

                        GUI.DisplaySysMessage(msg + t.Nickname + "は[" + SrcFormatter.Format(dmg) + "]のダメージを受けた。");
                    }
                    else
                    {
                        if (attack_num > 1)
                        {
                            msg = msg + SrcFormatter.Format(hit_count) + "回命中し、";
                        }

                        GUI.DisplaySysMessage(msg + t.Nickname + "は[" + SrcFormatter.Format(dmg) + "]のダメージを受けた。;" + "残りＨＰは" + SrcFormatter.Format(t.HP) + "（損傷率 = " + SrcFormatter.Format(100 * (t.MaxHP - t.HP) / t.MaxHP) + "％）");
                    }

                    // 特殊能力「不安定」による暴走チェック
                    if (t.IsFeatureAvailable("不安定"))
                    {
                        if (t.HP <= t.MaxHP / 4 && !t.IsConditionSatisfied("暴走"))
                        {
                            t.AddCondition("暴走", -1, cdata: "");
                            t.Update();
                            if (t.IsHero())
                            {
                                GUI.DisplaySysMessage(t.Nickname + "は暴走した。");
                            }
                            else
                            {
                                if (Strings.Len(t.FeatureName("不安定")) > 0)
                                {
                                    GUI.DisplaySysMessage(t.Nickname + "は[" + t.FeatureName("不安定") + "]の暴走のために制御不能に陥った。");
                                }
                                else
                                {
                                    GUI.DisplaySysMessage(t.Nickname + "は制御不能に陥った。");
                                }
                            }
                        }
                    }

                    // ダメージを受ければ眠りからさめる
                    if (t.IsConditionSatisfied("睡眠") && !w.IsWeaponClassifiedAs("眠"))
                    {
                        t.DeleteCondition("睡眠");
                        GUI.DisplaySysMessage(t.Nickname + "は眠りから覚めた。");
                    }

                    // ダメージを受けると凍結解除
                    if (t.IsConditionSatisfied("凍結") && !w.IsWeaponClassifiedAs("凍"))
                    {
                        t.DeleteCondition("凍結");
                        GUI.DisplaySysMessage(t.Nickname + "は凍結状態から開放された。");
                    }
                }

                //// 破壊された場合の処理
                int morale_mod;
                if (t.HP == 0)
                {
                    if (attack_num > 1)
                    {
                        msg = msg + SrcFormatter.Format(hit_count) + "回命中し、";
                    }

                    if (t.IsSysMessageDefined("破壊", sub_situation: ""))
                    {
                        t.SysMessage("破壊", sub_situation: "", add_msg: "");
                    }
                    else if (t.IsHero())
                    {
                        GUI.DisplaySysMessage(msg + t.Nickname + "は[" + SrcFormatter.Format(dmg) + "]のダメージを受け倒された。");
                    }
                    else
                    {
                        GUI.DisplaySysMessage(msg + t.Nickname + "は[" + SrcFormatter.Format(dmg) + "]のダメージを受け破壊された。");
                    }

                    // 復活するかどうかのチェックを行う

                    // スペシャルパワー「復活」
                    if (t.IsUnderSpecialPowerEffect("復活"))
                    {
                        t.RemoveSpecialPowerInEffect("破壊");
                        goto Resurrect;
                    }

                    // パイロット用特殊能力「英雄」＆「再生」
                    if (!is_event && !IsUnderSpecialPowerEffect("絶対破壊"))
                    {
                        if (GeneralLib.Dice(16) <= t.MainPilot().SkillLevel("英雄", ref_mode: ""))
                        {
                            t.HP = t.MaxHP / 2;
                            t.IncreaseMorale(10);
                            if (t.IsMessageDefined("復活"))
                            {
                                t.PilotMessage("復活", msg_mode: "");
                            }

                            if (t.IsAnimationDefined("復活", sub_situation: ""))
                            {
                                t.PlayAnimation("復活", sub_situation: "");
                            }
                            else
                            {
                                t.SpecialEffect("復活", sub_situation: "");
                            }

                            buf = t.MainPilot().SkillName0("英雄");
                            if (buf == "非表示")
                            {
                                buf = "英雄";
                            }

                            if (t.IsSysMessageDefined("復活", buf))
                            {
                                t.SysMessage("復活", buf, add_msg: "");
                            }
                            else
                            {
                                GUI.DisplaySysMessage(t.MainPilot().get_Nickname(false) + "の熱き" + buf + "の心が[" + t.Nickname + "]を復活させた！");
                            }

                            goto Resurrect;
                        }

                        // 浄化の適用
                        if (t.MainPilot().IsSkillAvailable("再生"))
                        {
                            if (w.IsWeaponClassifiedAs("浄"))
                            {
                                foreach (var p in AllPilots)
                                {
                                    if (p.IsSkillAvailable("浄化"))
                                    {
                                        if (IsMessageDefined("浄化(" + wname + ")"))
                                        {
                                            PilotMessage("浄化(" + wname + ")", msg_mode: "");
                                            if (IsAnimationDefined("浄化", wname))
                                            {
                                                PlayAnimation("浄化", wname);
                                            }
                                            else
                                            {
                                                SpecialEffect("浄化", wname);
                                            }
                                        }
                                        else if (IsMessageDefined("浄化"))
                                        {
                                            PilotMessage("浄化", msg_mode: "");
                                            if (IsAnimationDefined("浄化", wname))
                                            {
                                                PlayAnimation("浄化", wname);
                                            }
                                            else
                                            {
                                                SpecialEffect("浄化", wname);
                                            }
                                        }
                                        else if (IsMessageDefined("浄解(" + wname + ")"))
                                        {
                                            PilotMessage("浄解(" + wname + ")", msg_mode: "");
                                            if (IsAnimationDefined("浄解", wname))
                                            {
                                                PlayAnimation("浄解", wname);
                                            }
                                            else
                                            {
                                                SpecialEffect("浄解", wname);
                                            }
                                        }
                                        else if (IsMessageDefined("浄解"))
                                        {
                                            PilotMessage("浄解", msg_mode: "");
                                            if (IsAnimationDefined("浄解", wname))
                                            {
                                                PlayAnimation("浄解", wname);
                                            }
                                            else
                                            {
                                                SpecialEffect("浄解", wname);
                                            }
                                        }

                                        if (IsSysMessageDefined("浄化", sub_situation: ""))
                                        {
                                            SysMessage("浄化", sub_situation: "", add_msg: "");
                                        }
                                        else
                                        {
                                            GUI.DisplaySysMessage(p.get_Nickname(false) + "は浄化を行って[" + t.Nickname + "]の復活を防いだ。");
                                        }

                                        goto Cure;
                                    }
                                }

                                if (IsHero())
                                {
                                    if (IsMessageDefined("浄化(" + wname + ")"))
                                    {
                                        PilotMessage("浄化(" + wname + ")", msg_mode: "");
                                        if (IsAnimationDefined("浄化", wname))
                                        {
                                            PlayAnimation("浄化", wname);
                                        }
                                        else
                                        {
                                            SpecialEffect("浄化", wname);
                                        }

                                        GUI.DisplaySysMessage(MainPilot().get_Nickname(false) + "は浄化を行って[" + t.Nickname + "]の復活を防いだ。");
                                    }
                                    else if (IsMessageDefined("浄化"))
                                    {
                                        PilotMessage("浄化", msg_mode: "");
                                        if (IsAnimationDefined("浄化", wname))
                                        {
                                            PlayAnimation("浄化", wname);
                                        }
                                        else
                                        {
                                            SpecialEffect("浄化", wname);
                                        }

                                        GUI.DisplaySysMessage(MainPilot().get_Nickname(false) + "は浄化を行って[" + t.Nickname + "]の復活を防いだ。");
                                    }
                                    else if (IsMessageDefined("浄解(" + wname + ")"))
                                    {
                                        PilotMessage("浄解(" + wname + ")", msg_mode: "");
                                        if (IsAnimationDefined("浄解", wname))
                                        {
                                            PlayAnimation("浄解", wname);
                                        }
                                        else
                                        {
                                            SpecialEffect("浄解", wname);
                                        }

                                        GUI.DisplaySysMessage(MainPilot().get_Nickname(false) + "は浄化を行って[" + t.Nickname + "]の復活を防いだ。");
                                    }
                                    else if (IsMessageDefined("浄解"))
                                    {
                                        PilotMessage("浄解", msg_mode: "");
                                        if (IsAnimationDefined("浄解", wname))
                                        {
                                            PlayAnimation("浄解", wname);
                                        }
                                        else
                                        {
                                            SpecialEffect("浄解", wname);
                                        }

                                        if (IsSysMessageDefined("浄化", sub_situation: ""))
                                        {
                                            SysMessage("浄化", sub_situation: "", add_msg: "");
                                        }
                                        else
                                        {
                                            GUI.DisplaySysMessage(MainPilot().get_Nickname(false) + "は浄化を行って[" + t.Nickname + "]の復活を防いだ。");
                                        }
                                    }

                                    goto Cure;
                                }
                            }

                            if (GeneralLib.Dice(16) <= t.MainPilot().SkillLevel("再生", ref_mode: ""))
                            {
                                t.HP = t.MaxHP / 2;
                                if (t.IsMessageDefined("復活"))
                                {
                                    t.PilotMessage("復活", msg_mode: "");
                                }

                                if (t.IsAnimationDefined("復活", sub_situation: ""))
                                {
                                    t.PlayAnimation("復活", sub_situation: "");
                                }
                                else
                                {
                                    t.SpecialEffect("復活", sub_situation: "");
                                }

                                buf = t.MainPilot().SkillName0("再生");
                                if (buf == "非表示")
                                {
                                    buf = "再生";
                                }

                                if (t.IsSysMessageDefined("再生", buf))
                                {
                                    t.SysMessage("再生", buf, add_msg: "");
                                }
                                else
                                {
                                    GUI.DisplaySysMessage(t.Nickname + "は" + buf + "の力で一瞬にして復活した！");
                                }

                                goto Resurrect;
                            }
                        }
                    }

                Cure:
                    ;


                    // ユニット破壊によるパーツ分離
                    if (separate_parts)
                    {
                        var uname = GeneralLib.LIndex(t.FeatureData("パーツ分離"), 2);
                        if (!t.IsHero())
                        {
                            if (SRC.BattleAnimation)
                            {
                                Effect.ExplodeAnimation(t.Size, t.x, t.y);
                            }
                            else
                            {
                                Sound.PlayWave("Explode.wav");
                            }
                        }

                        var fname = t.FeatureName("パーツ分離");
                        if (t.IsAnimationDefined("破壊時分離(" + t.Name + ")", sub_situation: ""))
                        {
                            t.PlayAnimation("破壊時分離(" + t.Name + ")", sub_situation: "");
                        }
                        else if (t.IsAnimationDefined("破壊時分離(" + fname + ")"))
                        {
                            t.PlayAnimation("破壊時分離(" + fname + ")", sub_situation: "");
                        }
                        else if (t.IsAnimationDefined("破壊時分離", sub_situation: ""))
                        {
                            t.PlayAnimation("破壊時分離", sub_situation: "");
                        }
                        else if (t.IsSpecialEffectDefined("破壊時分離(" + t.Name + ")"))
                        {
                            t.SpecialEffect("破壊時分離(" + t.Name + ")", sub_situation: "");
                        }
                        else if (t.IsSpecialEffectDefined("破壊時分離(" + fname + ")"))
                        {
                            t.SpecialEffect("破壊時分離(" + fname + ")", sub_situation: "");
                        }
                        else if (t.IsSpecialEffectDefined("破壊時分離", sub_situation: ""))
                        {
                            t.SpecialEffect("破壊時分離", sub_situation: "");
                        }
                        else if (t.IsAnimationDefined("分離(" + t.Name + ")"))
                        {
                            t.PlayAnimation("分離(" + t.Name + ")", sub_situation: "");
                        }
                        else if (t.IsAnimationDefined("分離(" + fname + ")"))
                        {
                            t.PlayAnimation("分離(" + fname + ")", sub_situation: "");
                        }
                        else if (t.IsAnimationDefined("分離", sub_situation: ""))
                        {
                            t.PlayAnimation("分離", sub_situation: "");
                        }
                        else if (t.IsSpecialEffectDefined("分離(" + t.Name + ")"))
                        {
                            t.SpecialEffect("分離(" + t.Name + ")", sub_situation: "");
                        }
                        else if (t.IsSpecialEffectDefined("分離(" + fname + ")"))
                        {
                            t.SpecialEffect("分離(" + fname + ")", sub_situation: "");
                        }
                        else
                        {
                            t.SpecialEffect("分離", sub_situation: "");
                        }

                        t.Transform(uname);
                        {
                            var withBlock4 = t.CurrentForm();
                            withBlock4.HP = withBlock4.MaxHP;
                            // 自分から攻撃して破壊された時には行動数を0に
                            if ((withBlock4.Party ?? "") == (SRC.Stage ?? ""))
                            {
                                withBlock4.UsedAction = withBlock4.MaxAction();
                            }
                        }

                        if (t.IsSysMessageDefined("破壊時分離(" + t.Name + ")", sub_situation: ""))
                        {
                            t.SysMessage("破壊時分離(" + t.Name + ")", sub_situation: "", add_msg: "");
                        }
                        else if (t.IsSysMessageDefined("破壊時分離(" + fname + ")"))
                        {
                            t.SysMessage("破壊時分離(" + fname + ")", sub_situation: "", add_msg: "");
                        }
                        else if (t.IsSysMessageDefined("破壊時分離", sub_situation: ""))
                        {
                            t.SysMessage("破壊時分離", sub_situation: "", add_msg: "");
                        }
                        else if (t.IsSysMessageDefined("分離(" + t.Name + ")"))
                        {
                            t.SysMessage("分離(" + t.Name + ")", sub_situation: "", add_msg: "");
                        }
                        else if (t.IsSysMessageDefined("分離(" + fname + ")"))
                        {
                            t.SysMessage("分離(" + fname + ")", sub_situation: "", add_msg: "");
                        }
                        else if (t.IsSysMessageDefined("分離", sub_situation: ""))
                        {
                            t.SysMessage("分離", sub_situation: "", add_msg: "");
                        }
                        else if (t.IsHero())
                        {
                            if ((t.Nickname ?? "") != (t.OtherForm(uname).Nickname ?? ""))
                            {
                                GUI.DisplaySysMessage(t.Nickname + "は" + t.OtherForm(uname).Nickname + "に変化した。");
                            }
                            else
                            {
                                GUI.DisplaySysMessage(t.Nickname + "は変化し、蘇った。");
                            }
                        }
                        else
                        {
                            GUI.DisplaySysMessage(t.Nickname + "は破壊されたパーツを分離させた。");
                        }

                        t = t.CurrentForm();
                        Commands.SelectedTarget = t;
                        Event.SelectedTargetForEvent = t;
                        goto Resurrect;
                    }

                    // ユニット破壊による気力の変動
                    if (attack_mode != "マップ攻撃")
                    {
                        // 敵を破壊したユニットのパイロットはトータルで気力+4
                        if (Strings.InStr(attack_mode, "援護攻撃") > 0)
                        {
                            Commands.AttackUnit.CurrentForm().IncreaseMorale(3);
                        }
                        else
                        {
                            IncreaseMorale(3);
                        }

                        // それ以外のパイロット
                        foreach (Pilot p in SRC.PList.Items)
                        {
                            // 出撃中のパイロットのみが対象
                            if (p.Unit == null)
                            {
                                goto NextPilot;
                            }

                            if (p.Unit.Status != "出撃")
                            {
                                goto NextPilot;
                            }

                            if ((p.Party ?? "") == (Party ?? ""))
                            {
                                // 敵を破壊したユニットの陣営のパイロットは気力+1
                                if (p.Personality != "機械")
                                {
                                    p.Morale = (p.Morale + 1);
                                }
                            }
                            else if ((p.Party ?? "") == (t.Party ?? ""))
                            {
                                // 破壊されたユニットの陣営のパイロットは性格に応じて気力を変化
                                switch (p.Personality ?? "")
                                {
                                    case "超強気":
                                        {
                                            morale_mod = 2;
                                            break;
                                        }

                                    case "強気":
                                        {
                                            morale_mod = 1;
                                            break;
                                        }

                                    case "弱気":
                                        {
                                            morale_mod = -1;
                                            break;
                                        }

                                    default:
                                        {
                                            morale_mod = 0;
                                            break;
                                        }
                                }

                                // 味方の場合の気力変化量はオプションで変化する
                                if (p.Party == "味方" && Expression.IsOptionDefined("破壊時味方気力変化５倍"))
                                {
                                    p.Morale = (p.Morale + 5 * morale_mod);
                                }
                                else
                                {
                                    p.Morale = (p.Morale + morale_mod);
                                }
                            }

                        NextPilot:
                            ;
                        }
                    }

                    // 脱出メッセージの表示
                    if (t.IsMessageDefined("脱出") && !is_event && !Event.IsEventDefined("破壊 " + t.MainPilot().ID, true))
                    {
                        t.PilotMessage("脱出", msg_mode: "");
                    }

                    // 戦闘アニメ表示を使わない場合はかばったユニットを元の位置に戻しておく
                    if (!SRC.BattleAnimation)
                    {
                        if (su != null)
                        {
                            {
                                var withBlock5 = su;
                                withBlock5.x = prev_x;
                                withBlock5.y = prev_y;
                                withBlock5.Area = prev_area;
                            }
                        }
                    }

                    // ユニットを破壊
                    t.Die();
                }
            }

        Resurrect:
            ;
        // 復活した場合は破壊関連の処理を行わない

        EndAttack:
            ;
            {
                if (Status == "出撃"
                    && t.Status == "出撃"
                    && Strings.InStr(attack_mode, "援護攻撃") == 0
                    && attack_mode != "マップ攻撃"
                    && attack_mode != "反射"
                    && !w.IsWeaponClassifiedAs("合")
                    && HP > 0
                    && t.HP > 0)
                {
                    // 再攻撃
                    if (!second_attack && w.IsWeaponAvailable("ステータス") && w.IsTargetWithinRange(t))
                    {
                        // スペシャルパワー効果「再攻撃」
                        if (IsUnderSpecialPowerEffect("再攻撃"))
                        {
                            second_attack = true;
                            RemoveSpecialPowerInEffect("攻撃");
                            goto begin;
                        }

                        // 再攻撃能力
                        if (MainPilot().IsSkillAvailable("再攻撃"))
                        {
                            if (this.MainPilot().Intuition >= t.MainPilot().Intuition)
                            {
                                slevel = (int)(2d * MainPilot().SkillLevel("再攻撃", ref_mode: ""));
                            }
                            else
                            {
                                slevel = (int)MainPilot().SkillLevel("再攻撃", ref_mode: "");
                            }

                            if (slevel >= GeneralLib.Dice(32))
                            {
                                second_attack = true;
                                RemoveSpecialPowerInEffect("攻撃");
                                goto begin;
                            }
                        }

                        // 再属性
                        if (w.WeaponLevel("再") >= GeneralLib.Dice(16))
                        {
                            second_attack = true;
                            RemoveSpecialPowerInEffect("攻撃");
                            goto begin;
                        }
                    }

                    // 追加攻撃
                    if (ReferenceEquals(su, t))
                    {
                        CheckAdditionalAttack(w, t, be_quiet, attack_mode, "援護防御", dmg);
                    }
                    else
                    {
                        CheckAdditionalAttack(w, t, be_quiet, attack_mode, "", dmg);
                    }
                }

                // サポートガードを行ったユニットは破壊処理の前に以前の位置に復帰させる
                int sx, sy;
                if (su != null)
                {
                    su = su.CurrentForm();
                    {
                        var withBlock6 = su;
                        sx = withBlock6.x;
                        sy = withBlock6.y;

                        // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        Map.MapDataForUnit[sx, sy] = null;
                        withBlock6.x = prev_x;
                        withBlock6.y = prev_y;
                        withBlock6.Area = prev_area;
                        if (withBlock6.Status == "出撃")
                        {
                            Map.MapDataForUnit[withBlock6.x, withBlock6.y] = su;
                            Map.MapDataForUnit[tx, ty] = orig_t;
                            if (SRC.BattleAnimation)
                            {
                                if (su.IsAnimationDefined("サポートガード終了", sub_situation: ""))
                                {
                                    if (!GUI.IsRButtonPressed())
                                    {
                                        su.PlayAnimation("サポートガード終了", sub_situation: "");
                                    }
                                }
                                else if (!GUI.IsRButtonPressed())
                                {
                                    GUI.PaintUnitBitmap(orig_t, "リフレッシュ無し");
                                    if (use_support_guard)
                                    {
                                        GUI.MoveUnitBitmap(su, sx, sy, withBlock6.x, withBlock6.y, 80, 4);
                                    }
                                    else
                                    {
                                        GUI.MoveUnitBitmap(su, sx, sy, withBlock6.x, withBlock6.y, 50);
                                    }
                                }
                                else
                                {
                                    GUI.PaintUnitBitmap(su, "リフレッシュ無し");
                                    GUI.PaintUnitBitmap(orig_t, "リフレッシュ無し");
                                }
                            }
                        }
                        else
                        {
                            Map.MapDataForUnit[withBlock6.x, withBlock6.y] = null;
                            Map.MapDataForUnit[tx, ty] = orig_t;
                            GUI.PaintUnitBitmap(orig_t, "リフレッシュ無し");
                        }
                    }
                }

                if (is_hit)
                {
                    // 攻撃を命中させたことによる気力増加
                    if (attack_mode != "マップ攻撃" && attack_mode != "反射")
                    {
                        {
                            var cf = CurrentForm();
                            if (cf.MainPilot().IsSkillAvailable("命中時気力増加"))
                            {
                                cf.Pilots.First().Morale = (int)(cf.Pilots.First().Morale + cf.MainPilot().SkillLevel("命中時気力増加", ref_mode: ""));
                            }
                        }
                    }

                    // 攻撃を受けたことによる気力増加
                    t.IncreaseMorale(1);
                    if (t.MainPilot().IsSkillAvailable("損傷時気力増加"))
                    {
                        t.Pilots.First().Morale = (int)(t.Pilots.First().Morale + t.MainPilot().SkillLevel("損傷時気力増加", ref_mode: ""));
                    }
                }
                else
                {
                    // 攻撃を外したことによる気力増加
                    if (attack_mode != "マップ攻撃" && attack_mode != "反射")
                    {
                        {
                            var cf = CurrentForm();
                            if (cf.MainPilot().IsSkillAvailable("失敗時気力増加"))
                            {
                                cf.Pilots.First().Morale = (int)(cf.Pilots.First().Morale + cf.MainPilot().SkillLevel("失敗時気力増加", ref_mode: ""));
                            }
                        }
                    }

                    // 攻撃を回避したことによる気力増加
                    if (t.MainPilot().IsSkillAvailable("回避時気力増加"))
                    {
                        t.Pilots.First().Morale = (int)(t.Pilots.First().Morale + t.MainPilot().SkillLevel("回避時気力増加", ref_mode: ""));
                    }
                }

                // スペシャルパワー効果の解除
                if (Strings.InStr(msg, "かばった") == 0)
                {
                    t.RemoveSpecialPowerInEffect("防御");
                }

                if (is_hit)
                {
                    t.RemoveSpecialPowerInEffect("被弾");
                }

                // 戦闘アニメで変更されたユニット画像を元に戻す
                if (t.IsConditionSatisfied("ユニット画像"))
                {
                    t.DeleteCondition("ユニット画像");
                    t.BitmapID = GUI.MakeUnitBitmap(t);
                    if (t.Status == "出撃")
                    {
                        GUI.PaintUnitBitmap(t, "リフレッシュ無し");
                    }
                }

                if (t.IsConditionSatisfied("非表示付加"))
                {
                    t.DeleteCondition("非表示付加");
                    t.BitmapID = GUI.MakeUnitBitmap(t);
                    if (t.Status == "出撃")
                    {
                        GUI.PaintUnitBitmap(t, "リフレッシュ無し");
                    }
                }

                // 戦闘に参加したユニットを識別
                {
                    var cf = CurrentForm();
                    if (Expression.IsOptionDefined("ユニット情報隠蔽"))
                    {
                        if (cf.Party0 == "敵" || cf.Party0 == "中立")
                        {
                            cf.AddCondition("識別済み", -1, 0d, "非表示");
                        }

                        if (t.Party0 == "敵" || t.Party0 == "中立")
                        {
                            t.AddCondition("識別済み", -1, 0d, "非表示");
                        }
                    }
                    else
                    {
                        if (cf.IsConditionSatisfied("ユニット情報隠蔽"))
                        {
                            cf.DeleteCondition("ユニット情報隠蔽");
                        }

                        if (t.IsConditionSatisfied("ユニット情報隠蔽"))
                        {
                            t.DeleteCondition("ユニット情報隠蔽");
                        }
                    }
                }
            }
            // 情報を更新
            CurrentForm().Update();
            t.Update();
            {
                // マップ攻撃や反射による攻撃の場合はここまで
                switch (attack_mode ?? "")
                {
                    case "マップ攻撃":
                    case "反射":
                        {
                            Commands.RestoreSelections();
                            return;
                        }
                }

                // ステルスが解ける？
                if (IsFeatureAvailable("ステルス"))
                {
                    if (w.IsWeaponClassifiedAs("忍"))
                    {
                        // 暗殺武器の場合、相手を倒すか行動不能にすればステルス継続
                        if (t.CurrentForm().Status == "出撃" && t.CurrentForm().MaxAction() > 0)
                        {
                            AddCondition("ステルス無効", 1, cdata: "");
                        }
                    }
                    else
                    {
                        AddCondition("ステルス無効", 1, cdata: "");
                    }
                }

                // 合体技のパートナーの弾数＆ＥＮの消費
                foreach (var partner in partners)
                {
                    {
                        var cf = partner.CurrentForm();
                        var cfw = cf.Weapons.FirstOrDefault(x => x.Name == wname);
                        if (cfw != null)
                        {
                            cfw.UseWeapon();

                            if (cfw.IsWeaponClassifiedAs("自"))
                            {
                                if (cf.IsFeatureAvailable("パーツ分離"))
                                {
                                    var uname = GeneralLib.LIndex(cf.FeatureData("パーツ分離"), 2);

                                    if (cf.OtherForm(uname).IsAbleToEnter(cf.x, cf.y))
                                    {
                                        cf.Transform(uname);
                                        {
                                            var withBlock11 = cf.CurrentForm();
                                            withBlock11.HP = withBlock11.MaxHP;
                                            withBlock11.UsedAction = withBlock11.MaxAction();
                                        }
                                    }
                                    else
                                    {
                                        cf.Die();
                                    }
                                }
                                else
                                {
                                    cf.Die();
                                }
                            }
                            else if (cfw.IsWeaponClassifiedAs("失") && cf.HP == 0)
                            {
                                cf.Die();
                            }
                            else if (cfw.IsWeaponClassifiedAs("変"))
                            {
                                // XXX 変形技
                                if (cf.IsFeatureAvailable("変形技"))
                                {
                                    var uname = "";
                                    var fd = cf.Features.FirstOrDefault(x => x.Name == "変形技" && x.DataL[0] == wname);
                                    if (fd != null)
                                    {
                                        uname = fd.DataL[1];
                                        if (cf.OtherForm(uname).IsAbleToEnter(cf.x, cf.y))
                                        {
                                            cf.Transform(uname);
                                        }
                                    }

                                    if ((uname ?? "") != (cf.CurrentForm().Name ?? ""))
                                    {
                                        if (cf.IsFeatureAvailable("ノーマルモード"))
                                        {
                                            uname = GeneralLib.LIndex(cf.FeatureData("ノーマルモード"), 1);
                                            if (cf.OtherForm(uname).IsAbleToEnter(cf.x, cf.y))
                                            {
                                                cf.Transform(uname);
                                            }
                                        }
                                    }
                                }
                                else if (cf.IsFeatureAvailable("ノーマルモード"))
                                {
                                    var uname = GeneralLib.LIndex(cf.FeatureData("ノーマルモード"), 1);
                                    if (cf.OtherForm(uname).IsAbleToEnter(cf.x, cf.y))
                                    {
                                        cf.Transform(uname);
                                    }
                                }
                            }

                            break;
                        }
                        else
                        {
                            // 同名の武器がなかった場合は自分のデータを使って処理
                            if (w.WeaponData.ENConsumption > 0)
                            {
                                cf.EN = cf.EN - w.WeaponENConsumption();
                            }

                            if (w.IsWeaponClassifiedAs("消"))
                            {
                                cf.AddCondition("消耗", 1, cdata: "");
                            }

                            if (w.IsWeaponClassifiedAs("Ｃ") && cf.IsConditionSatisfied("チャージ完了"))
                            {
                                cf.DeleteCondition("チャージ完了");
                            }

                            if (w.IsWeaponClassifiedAs("気"))
                            {
                                cf.IncreaseMorale((int)(-5 * w.WeaponLevel("気")));
                            }

                            if (w.IsWeaponClassifiedAs("霊"))
                            {
                                hp_ratio = 100 * cf.HP / (double)cf.MaxHP;
                                en_ratio = 100 * cf.EN / (double)cf.MaxEN;
                                cf.MainPilot().Plana = (int)(cf.MainPilot().Plana - 5d * w.WeaponLevel("霊"));
                                cf.HP = (int)(cf.MaxHP * hp_ratio / 100d);
                                cf.EN = (int)(cf.MaxEN * en_ratio / 100d);
                            }
                            else if (w.IsWeaponClassifiedAs("プ"))
                            {
                                hp_ratio = 100 * cf.HP / (double)cf.MaxHP;
                                en_ratio = 100 * cf.EN / (double)cf.MaxEN;
                                cf.MainPilot().Plana = (int)(cf.MainPilot().Plana - 5d * w.WeaponLevel("プ"));
                                cf.HP = (int)(cf.MaxHP * hp_ratio / 100d);
                                cf.EN = (int)(cf.MaxEN * en_ratio / 100d);
                            }

                            if (w.IsWeaponClassifiedAs("失"))
                            {
                                cf.HP = GeneralLib.MaxLng((int)(cf.HP - (long)(cf.MaxHP * w.WeaponLevel("失")) / 10L), 0);
                            }

                            if (w.IsWeaponClassifiedAs("自"))
                            {
                                if (cf.IsFeatureAvailable("パーツ分離"))
                                {
                                    var uname = GeneralLib.LIndex(cf.FeatureData("パーツ分離"), 1);
                                    if (cf.OtherForm(uname).IsAbleToEnter(cf.x, cf.y))
                                    {
                                        cf.Transform(uname);
                                        {
                                            var withBlock12 = cf.CurrentForm();
                                            withBlock12.HP = withBlock12.MaxHP;
                                            withBlock12.UsedAction = withBlock12.MaxAction();
                                        }
                                    }
                                    else
                                    {
                                        cf.Die();
                                    }
                                }
                                else
                                {
                                    cf.Die();
                                }
                            }
                            else if (w.IsWeaponClassifiedAs("失") && cf.HP == 0)
                            {
                                cf.Die();
                            }
                            else if (w.IsWeaponClassifiedAs("変"))
                            {
                                if (cf.IsFeatureAvailable("ノーマルモード"))
                                {
                                    var uname = GeneralLib.LIndex(cf.FeatureData("ノーマルモード"), 1);

                                    if (cf.OtherForm(uname).IsAbleToEnter(cf.x, cf.y))
                                    {
                                        cf.Transform(uname);
                                    }
                                }
                            }
                        }
                    }
                }

                // 以下の特殊効果はは武器データの変化があるため、同時には適応されない
                // 反射等により破壊された場合はなにも出来ない
                // 自爆攻撃
                // ＨＰ消費攻撃による自殺
                // 変形技

                // 能力コピー
                if (CurrentForm().Status == "破壊")
                {
                }
                else if (w.IsWeaponClassifiedAs("自"))
                {
                    if (IsFeatureAvailable("パーツ分離"))
                    {
                        var uname = GeneralLib.LIndex(FeatureData("パーツ分離"), 2);

                        if (OtherForm(uname).IsAbleToEnter(x, y))
                        {
                            Transform(uname);
                            {
                                var withBlock13 = CurrentForm();
                                withBlock13.HP = withBlock13.MaxHP;
                                withBlock13.UsedAction = withBlock13.MaxAction();
                            }

                            var fname = FeatureName("パーツ分離");
                            if (IsSysMessageDefined("破壊時分離(" + Name + ")", sub_situation: ""))
                            {
                                SysMessage("破壊時分離(" + Name + ")", sub_situation: "", add_msg: "");
                            }
                            else if (IsSysMessageDefined("破壊時分離(" + fname + ")"))
                            {
                                SysMessage("破壊時分離(" + fname + ")", sub_situation: "", add_msg: "");
                            }
                            else if (IsSysMessageDefined("破壊時分離", sub_situation: ""))
                            {
                                SysMessage("破壊時分離", sub_situation: "", add_msg: "");
                            }
                            else if (IsSysMessageDefined("分離(" + Name + ")"))
                            {
                                SysMessage("分離(" + Name + ")", sub_situation: "", add_msg: "");
                            }
                            else if (IsSysMessageDefined("分離(" + fname + ")"))
                            {
                                SysMessage("分離(" + fname + ")", sub_situation: "", add_msg: "");
                            }
                            else if (IsSysMessageDefined("分離", sub_situation: ""))
                            {
                                SysMessage("分離", sub_situation: "", add_msg: "");
                            }
                            else
                            {
                                GUI.DisplaySysMessage(Nickname + "は破壊されたパーツを分離させた。");
                            }
                        }
                        else
                        {
                            Die();
                        }
                    }
                    else
                    {
                        Die();
                    }
                }
                else if (w.IsWeaponClassifiedAs("失") && HP == 0)
                {
                    Die();
                }
                else if (w.IsWeaponClassifiedAs("変"))
                {
                    if (IsFeatureAvailable("変形技"))
                    {
                        var uname = "";
                        var fd = Features.FirstOrDefault(x => x.Name == "変形技" && x.DataL[0] == wname);
                        if (fd != null)
                        {
                            uname = fd.DataL[1];
                            if (OtherForm(uname).IsAbleToEnter(x, y))
                            {
                                Transform(uname);
                            }
                        }

                        if ((uname ?? "") != (CurrentForm().Name ?? ""))
                        {
                            if (IsFeatureAvailable("ノーマルモード"))
                            {
                                uname = GeneralLib.LIndex(FeatureData("ノーマルモード"), 1);
                                if (OtherForm(uname).IsAbleToEnter(x, y))
                                {
                                    Transform(uname);
                                }
                            }
                        }
                    }
                    else if (IsFeatureAvailable("ノーマルモード"))
                    {
                        var uname = GeneralLib.LIndex(FeatureData("ノーマルモード"), 1);

                        if (OtherForm(uname).IsAbleToEnter(x, y))
                        {
                            Transform(uname);
                        }
                    }
                }

                // アイテムを消費
                else if (w.WeaponData.IsItem() && w.Bullet() == 0 && w.MaxBullet() > 0)
                {
                    // アイテムを削除
                    var num = Data.CountWeapon();
                    num += AllPilots.Sum(x => x.Data.CountWeapon());

                    foreach (Item itm in colItem.List)
                    {
                        // XXX もうちょいいいアイテムの特定ができそう、武器が何由来かを持っておけばよさそう
                        num = (num + itm.CountWeapon());
                        if (w.WeaponNo() <= num)
                        {
                            itm.Exist = false;
                            DeleteItem(itm);
                            break;
                        }
                    }
                }
                else if (is_hit && (w.IsWeaponClassifiedAs("写")
                    || w.IsWeaponClassifiedAs("化")) && (dmg > 0
                    || !w.IsWeaponClassifiedAs("殺")))
                {
                    CheckMetamorphAttack(w, t, def_mode);
                }

                {
                    var cf = CurrentForm();
                    // スペシャルパワーの効果を削除
                    if (Strings.InStr(attack_mode, "援護攻撃") == 0)
                    {
                        if (cf.IsUnderSpecialPowerEffect("攻撃後消耗"))
                        {
                            cf.AddCondition("消耗", 1, cdata: "");
                        }

                        cf.RemoveSpecialPowerInEffect("攻撃");
                        if (is_hit)
                        {
                            cf.RemoveSpecialPowerInEffect("命中");
                        }
                    }

                    // 戦闘アニメで変更されたユニット画像を元に戻す
                    if (cf.IsConditionSatisfied("ユニット画像"))
                    {
                        cf.DeleteCondition("ユニット画像");
                        cf.BitmapID = GUI.MakeUnitBitmap(CurrentForm());
                        GUI.PaintUnitBitmap(CurrentForm());
                    }

                    if (cf.IsConditionSatisfied("非表示付加"))
                    {
                        cf.DeleteCondition("非表示付加");
                        cf.BitmapID = GUI.MakeUnitBitmap(CurrentForm());
                        GUI.PaintUnitBitmap(CurrentForm());
                    }

                    foreach (var partner in partners)
                    {
                        {
                            var pcf = partner.CurrentForm();
                            if (pcf.IsConditionSatisfied("ユニット画像"))
                            {
                                pcf.DeleteCondition("ユニット画像");
                                pcf.BitmapID = GUI.MakeUnitBitmap(partner.CurrentForm());
                                GUI.PaintUnitBitmap(partner.CurrentForm());
                            }

                            if (pcf.IsConditionSatisfied("非表示付加"))
                            {
                                pcf.DeleteCondition("非表示付加");
                                pcf.BitmapID = GUI.MakeUnitBitmap(partner.CurrentForm());
                                GUI.PaintUnitBitmap(partner.CurrentForm());
                            }
                        }
                    }
                }

                // カットインは消去しておく
                if (!Expression.IsOptionDefined("戦闘中画面初期化無効") || attack_mode == "マップ攻撃")
                {
                    if (GUI.IsPictureVisible)
                    {
                        GUI.ClearPicture();
                        GUI.RefreshScreen();
                    }
                }

                // 戦闘アニメ終了処理
                if (IsAnimationDefined(wname + "(終了)", sub_situation: ""))
                {
                    PlayAnimation(wname + "(終了)", sub_situation: "");
                }
                else if (IsAnimationDefined("終了", sub_situation: ""))
                {
                    PlayAnimation("終了", sub_situation: "");
                }
            }
            // ユニット選択を解除
            Commands.RestoreSelections();
        }
    }
}
