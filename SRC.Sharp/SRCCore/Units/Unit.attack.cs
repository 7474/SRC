// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.VB;
using System.Collections.Generic;

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
            short prob;
            int dmg, prev_hp;
            bool is_hit = default, is_critical;
            string critical_type;
            bool use_shield, use_shield_msg;
            bool is_penetrated;
            bool use_protect_msg;
            bool use_support_guard;
            string wname, wnickname;
            string fname, uname = default;
            string msg, buf;
            short k, i, j, num;
            Unit su = default, orig_t;
            Unit[] partners;
            short tx, ty;
            string tarea;
            short prev_x = default, prev_y = default;
            var prev_area = default(string);
            var second_attack = default(bool);
            bool be_quiet;
            int attack_num = default, hit_count = default;
            short slevel;
            Unit saved_selected_unit;
            double hp_ratio, en_ratio;
            bool separate_parts;
            short orig_w;
            // ADD START MARGE
            bool is_ext_anime_defined;
            // ADD END MARGE

            wname = Weapon(w).Name;
            wnickname = WeaponNickname(w);

            // メッセージ表示用に選択状況を切り替え
            Commands.SaveSelections();
            saved_selected_unit = Commands.SelectedUnit;
            if (attack_mode == "反射")
            {
                Commands.SelectedUnit = Commands.SelectedTarget;
                Commands.SelectedTarget = this;
                Event_Renamed.SelectedUnitForEvent = Event_Renamed.SelectedTargetForEvent;
                Event_Renamed.SelectedTargetForEvent = this;
                Commands.SelectedWeapon = w;
                Commands.SelectedWeaponName = wname;
            }
            else
            {
                if (ReferenceEquals(Commands.SelectedUnit, t))
                {
                    Commands.SelectedTWeapon = Commands.SelectedWeapon;
                    Commands.SelectedTWeaponName = Commands.SelectedWeaponName;
                }

                Commands.SelectedWeapon = w;
                Commands.SelectedWeaponName = wname;
                Commands.SelectedUnit = this;
                Commands.SelectedTarget = t;
                Event_Renamed.SelectedUnitForEvent = this;
                Event_Renamed.SelectedTargetForEvent = t;
            }

            // サポートガードを行ったユニットに関する情報をクリア
            if (!IsDefense())
            {
                // UPGRADE_NOTE: オブジェクト SupportGuardUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                Commands.SupportGuardUnit = null;
                // UPGRADE_NOTE: オブジェクト SupportGuardUnit2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                Commands.SupportGuardUnit2 = null;
            }

            // パイロットのセリフを表示するかどうかを判定
            if (attack_mode == "マップ攻撃" | attack_mode == "反射" | attack_mode == "当て身技" | attack_mode == "自動反撃")
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
                if (GUI.MainWidth != 15)
                {
                    Status.ClearUnitStatus();
                }

                string argoname = "戦闘中画面初期化無効";
                if (!Expression.IsOptionDefined(ref argoname))
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
            prob = HitProbability(w, ref t, true);

            // ダメージを算出
            dmg = Damage(w, ref t, true, Strings.InStr(attack_mode, "援護攻撃") > 0);

            // 特殊効果を持たない武器ならクリティカルの可能性あり
            if (IsNormalWeapon(w) & dmg > 0)
            {
                if (CriticalProbability(w, ref t, def_mode) >= GeneralLib.Dice(100) | attack_mode == "統率" | attack_mode == "同時援護攻撃")
                {
                    is_critical = true;
                }
            }

            partners = new Unit[1];
            Commands.SelectedPartners = new Unit[1];
            if (attack_mode != "マップ攻撃" & attack_mode != "反射" & !second_attack)
            {
                string argattr = "合";
                if (IsWeaponClassifiedAs(w, ref argattr))
                {
                    // 合体技の場合にパートナーをハイライト表示
                    if (WeaponMaxRange(w) == 1)
                    {
                        string argctype_Renamed = "武装";
                        CombinationPartner(ref argctype_Renamed, w, ref partners, tx, ty);
                    }
                    else
                    {
                        string argctype_Renamed1 = "武装";
                        CombinationPartner(ref argctype_Renamed1, w, ref partners);
                    }

                    var loopTo = (short)Information.UBound(partners);
                    for (i = 1; i <= loopTo; i++)
                    {
                        {
                            var withBlock = partners[i];
                            Map.MaskData[withBlock.x, withBlock.y] = false;
                        }
                    }

                    if (!SRC.BattleAnimation)
                    {
                        GUI.MaskScreen();
                    }
                }
                else if (!is_critical & dmg > 0 & Strings.InStr(attack_mode, "援護攻撃") == 0)
                {
                    // 連携攻撃が発動するかを判定
                    // （連携攻撃は合体技では発動しない）
                    if (this.Weapon(w).MaxRange > 1)
                    {
                        su = LookForAttackHelp(x, y);
                    }
                    else
                    {
                        su = LookForAttackHelp(tx, ty);
                    }

                    if (su is object)
                    {
                        // 連携攻撃発動
                        Map.MaskData[su.x, su.y] = false;
                        if (!SRC.BattleAnimation)
                        {
                            GUI.MaskScreen();
                        }

                        string argmain_situation = "連携攻撃(" + su.MainPilot().Name + ")";
                        if (IsMessageDefined(ref argmain_situation, true))
                        {
                            string argSituation = "連携攻撃(" + su.MainPilot().Name + ")";
                            string argmsg_mode = "";
                            PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                        }
                        else
                        {
                            string argSituation1 = "連携攻撃(" + su.MainPilot().get_Nickname(false) + ")";
                            string argmsg_mode1 = "";
                            PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
                        }

                        is_critical = true;
                        // UPGRADE_NOTE: オブジェクト su をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        su = null;
                    }
                }
            }

            // クリティカルならダメージ1.5倍
            if (is_critical)
            {
                string argoname1 = "ダメージ倍率低下";
                if (Expression.IsOptionDefined(ref argoname1))
                {
                    string argattr2 = "痛";
                    if (IsWeaponClassifiedAs(w, ref argattr2))
                    {
                        string argattr1 = "痛";
                        dmg = (int)((1d + 0.1d * (WeaponLevel(w, ref argattr1) + 2d)) * dmg);
                    }
                    else
                    {
                        dmg = (int)(1.2d * dmg);
                    }
                }
                else
                {
                    string argattr4 = "痛";
                    if (IsWeaponClassifiedAs(w, ref argattr4))
                    {
                        string argattr3 = "痛";
                        dmg = (int)((1d + 0.25d * (WeaponLevel(w, ref argattr3) + 2d)) * dmg);
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
                            string arganame = "援護攻撃発動";
                            Effect.ShowAnimation(ref arganame);
                            break;
                        }

                    case "カウンター":
                        {
                            string arganame1 = "カウンター発動";
                            Effect.ShowAnimation(ref arganame1);
                            break;
                        }
                }
            }

            // 攻撃側のメッセージ表示
            if (!be_quiet)
            {
                // 攻撃準備の効果音
                bool localIsSpecialEffectDefined() { string argmain_situation = wname + "(準備)"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                string argmain_situation4 = wname + "(準備)";
                string argsub_situation3 = "";
                string argsub_situation4 = "";
                string argoname2 = "武器準備アニメ非表示";
                if (IsAnimationDefined(ref argmain_situation4, sub_situation: ref argsub_situation3))
                {
                    string argmain_situation1 = wname + "(準備)";
                    string argsub_situation = "";
                    PlayAnimation(ref argmain_situation1, sub_situation: ref argsub_situation);
                }
                else if (IsAnimationDefined(ref wname, sub_situation: ref argsub_situation4) & !Expression.IsOptionDefined(ref argoname2) & SRC.WeaponAnimation)
                {
                    string argmain_situation2 = wname + "(準備)";
                    string argsub_situation1 = "";
                    PlayAnimation(ref argmain_situation2, sub_situation: ref argsub_situation1);
                }
                else if (localIsSpecialEffectDefined())
                {
                    string argmain_situation3 = wname + "(準備)";
                    string argsub_situation2 = "";
                    SpecialEffect(ref argmain_situation3, sub_situation: ref argsub_situation2);
                }
                else
                {
                    var argu = this;
                    Effect.PrepareWeaponEffect(ref argu, w);
                }

                // 攻撃メッセージの前に出力されるメッセージ
                string argattr5 = "合";
                if (second_attack)
                {
                    string argSituation2 = "再攻撃";
                    string argmsg_mode2 = "";
                    PilotMessage(ref argSituation2, msg_mode: ref argmsg_mode2);
                }
                else if (Strings.InStr(attack_mode, "援護攻撃") > 0)
                {
                    {
                        var withBlock1 = Commands.AttackUnit.CurrentForm().MainPilot();
                        bool localIsMessageDefined() { string argmain_situation = "サポートアタック(" + withBlock1.get_Nickname(false) + ")"; var ret = IsMessageDefined(ref argmain_situation); return ret; }

                        string argmain_situation5 = "サポートアタック(" + withBlock1.Name + ")";
                        string argmain_situation6 = "サポートアタック";
                        if (IsMessageDefined(ref argmain_situation5))
                        {
                            string argSituation3 = "サポートアタック(" + withBlock1.Name + ")";
                            string argmsg_mode3 = "";
                            PilotMessage(ref argSituation3, msg_mode: ref argmsg_mode3);
                        }
                        else if (localIsMessageDefined())
                        {
                            string argSituation4 = "サポートアタック(" + withBlock1.get_Nickname(false) + ")";
                            string argmsg_mode4 = "";
                            PilotMessage(ref argSituation4, msg_mode: ref argmsg_mode4);
                        }
                        else if (IsMessageDefined(ref argmain_situation6))
                        {
                            string argSituation5 = "サポートアタック";
                            string argmsg_mode5 = "";
                            PilotMessage(ref argSituation5, msg_mode: ref argmsg_mode5);
                        }
                    }
                }
                else if (attack_mode == "カウンター")
                {
                    string argSituation6 = "カウンター";
                    string argmsg_mode6 = "";
                    PilotMessage(ref argSituation6, msg_mode: ref argmsg_mode6);
                }
                else if (IsMessageDefined(ref wname) & wname != "格闘" & wname != "射撃" & wname != "攻撃" & !IsWeaponClassifiedAs(w, ref argattr5))
                {
                    string argmain_situation7 = "かけ声(" + wname + ")";
                    if (IsMessageDefined(ref argmain_situation7))
                    {
                        string argSituation7 = "かけ声(" + wname + ")";
                        string argmsg_mode7 = "";
                        PilotMessage(ref argSituation7, msg_mode: ref argmsg_mode7);
                    }
                    else if (IsDefense())
                    {
                        string argSituation9 = "かけ声(反撃)";
                        string argmsg_mode9 = "";
                        PilotMessage(ref argSituation9, msg_mode: ref argmsg_mode9);
                    }
                    else
                    {
                        string argSituation8 = "かけ声";
                        string argmsg_mode8 = "";
                        PilotMessage(ref argSituation8, msg_mode: ref argmsg_mode8);
                    }
                }

                // 攻撃メッセージ
                Sound.IsWavePlayed = false;
                if (!second_attack)
                {
                    if (attack_mode == "カウンター")
                    {
                        string argmsg_mode10 = "カウンター";
                        PilotMessage(ref wname, ref argmsg_mode10);
                    }
                    else
                    {
                        string argmsg_mode11 = "攻撃";
                        PilotMessage(ref wname, ref argmsg_mode11);
                    }
                }

                // 攻撃アニメ
                bool localIsAnimationDefined() { string argmain_situation = wname + "(反撃)"; string argsub_situation = ""; var ret = IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsAnimationDefined1() { string argmain_situation = wname + "(攻撃)"; string argsub_situation = ""; var ret = IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                string argsub_situation8 = "";
                string argsub_situation9 = "";
                if (IsDefense() & localIsAnimationDefined())
                {
                    string argmain_situation8 = wname + "(反撃)";
                    string argsub_situation5 = "";
                    PlayAnimation(ref argmain_situation8, sub_situation: ref argsub_situation5);
                }
                else if (localIsAnimationDefined1() | IsAnimationDefined(ref wname, sub_situation: ref argsub_situation8))
                {
                    string argmain_situation9 = wname + "(攻撃)";
                    string argsub_situation6 = "";
                    PlayAnimation(ref argmain_situation9, sub_situation: ref argsub_situation6);
                }
                else if (IsSpecialEffectDefined(ref wname, sub_situation: ref argsub_situation9))
                {
                    string argsub_situation7 = "";
                    SpecialEffect(ref wname, sub_situation: ref argsub_situation7);
                }
                else if (!Sound.IsWavePlayed)
                {
                    var argu1 = this;
                    Effect.AttackEffect(ref argu1, w);
                }
            }
            else if (attack_mode == "自動反撃")
            {
                // 攻撃アニメ
                bool localIsAnimationDefined2() { string argmain_situation = wname + "(反撃)"; string argsub_situation = ""; var ret = IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsAnimationDefined3() { string argmain_situation = wname + "(攻撃)"; string argsub_situation = ""; var ret = IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                string argsub_situation13 = "";
                string argsub_situation14 = "";
                if (IsDefense() & localIsAnimationDefined2())
                {
                    string argmain_situation10 = wname + "(反撃)";
                    string argsub_situation10 = "";
                    PlayAnimation(ref argmain_situation10, sub_situation: ref argsub_situation10);
                }
                else if (localIsAnimationDefined3() | IsAnimationDefined(ref wname, sub_situation: ref argsub_situation13))
                {
                    string argmain_situation11 = wname + "(攻撃)";
                    string argsub_situation11 = "";
                    PlayAnimation(ref argmain_situation11, sub_situation: ref argsub_situation11);
                }
                else if (IsSpecialEffectDefined(ref wname, sub_situation: ref argsub_situation14))
                {
                    string argsub_situation12 = "";
                    SpecialEffect(ref wname, sub_situation: ref argsub_situation12);
                }
                else if (!Sound.IsWavePlayed)
                {
                    var argu2 = this;
                    Effect.AttackEffect(ref argu2, w);
                }
            }

            if (attack_mode != "マップ攻撃" & attack_mode != "反射")
            {
                // 武器使用による弾数＆ＥＮの消費
                UseWeapon(w);
                // 武器使用によるＥＮ消費の表示
                var argu11 = this;
                object argu21 = t;
                GUI.UpdateMessageForm(ref argu11, ref argu21);
            }

            // 防御手段による命中率低下
            if (def_mode == "回避")
            {
                string argsptype = "絶対命中";
                string argsptype1 = "無防備";
                string argfname = "回避不可";
                object argIndex1 = "移動不能";
                if (!IsUnderSpecialPowerEffect(ref argsptype) & !t.IsUnderSpecialPowerEffect(ref argsptype1) & !t.IsFeatureAvailable(ref argfname) & !t.IsConditionSatisfied(ref argIndex1))
                {
                    prob = (short)(prob / 2);
                }
            }

            // 反射攻撃の場合は命中率が低下
            if (attack_mode == "反射")
            {
                prob = (short)(prob / 2);
            }

            // 攻撃を行ったことについてのシステムメッセージ
            if (!be_quiet)
            {
                switch (Information.UBound(partners))
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
                string argattr6 = "武";
                string argattr7 = "突";
                string argattr8 = "接";
                string argarea_name = "空";
                if (t.Area == "空中" & (IsWeaponClassifiedAs(w, ref argattr6) | IsWeaponClassifiedAs(w, ref argattr7) | IsWeaponClassifiedAs(w, ref argattr8)) & !IsTransAvailable(ref argarea_name))
                {
                    msg = msg + "ジャンプし、";
                }

                if (second_attack)
                {
                    msg = msg + "再度";
                }
                else if (attack_mode == "カウンター" | attack_mode == "先制攻撃")
                {
                    msg = "先制攻撃！;" + msg + "先手を取り";
                }

                // 攻撃の種類によってメッセージを切り替え
                string argattr9 = "盗";
                string argattr10 = "習";
                string argattr11 = "実";
                if (Strings.Right(wnickname, 2) == "攻撃" | Strings.Right(wnickname, 4) == "アタック" | wnickname == "突撃")
                {
                    msg = msg + "[" + wnickname + "]をかけた。;";
                }
                else if (IsSpellWeapon(w))
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
                else if (IsWeaponClassifiedAs(w, ref argattr9))
                {
                    msg = msg + "[" + t.Nickname + "]の持ち物を盗もうとした。;";
                }
                else if (IsWeaponClassifiedAs(w, ref argattr10))
                {
                    msg = msg + "[" + t.Nickname + "]の技を習得しようと試みた。;";
                }
                else if (IsWeaponClassifiedAs(w, ref argattr11) & (Strings.InStr(wnickname, "ミサイル") > 0 | Strings.InStr(wnickname, "ロケット") > 0))
                {
                    msg = msg + "[" + wnickname + "]を発射した。;";
                }
                else if (Strings.Right(wnickname, 1) == "息" | Strings.Right(wnickname, 3) == "ブレス" | Strings.Right(wnickname, 2) == "光線" | Strings.Right(wnickname, 1) == "光" | Strings.Right(wnickname, 3) == "ビーム" | Strings.Right(wnickname, 4) == "レーザー")
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
                        buf = "命中率 = " + GeneralLib.MinLng(HitProbability(w, ref t, false) / 2, 100) + "％" + "（" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(CriticalProbability(w, ref t, def_mode)) + "％）";
                    }
                    else
                    {
                        buf = "命中率 = " + GeneralLib.MinLng(HitProbability(w, ref t, false), 100) + "％" + "（" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(CriticalProbability(w, ref t, def_mode)) + "％）";
                    }
                }
                else
                {
                    buf = "命中率 = " + GeneralLib.MinLng(prob, 100) + "％" + "（" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(CriticalProbability(w, ref t, def_mode)) + "％）";
                }

                // 攻撃解説表示
                string argsub_situation17 = "";
                string argmain_situation13 = "攻撃";
                string argsub_situation18 = "";
                if (IsSysMessageDefined(ref wname, sub_situation: ref argsub_situation17))
                {
                    // 「武器名(解説)」のメッセージを使用
                    string argsub_situation15 = "";
                    SysMessage(ref wname, ref argsub_situation15, ref buf);
                }
                else if (IsSysMessageDefined(ref argmain_situation13, sub_situation: ref argsub_situation18))
                {
                    // 「攻撃(解説)」のメッセージを使用
                    string argmain_situation12 = "攻撃";
                    string argsub_situation16 = "";
                    SysMessage(ref argmain_situation12, ref argsub_situation16, ref buf);
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
                        object argIndex2 = "踊り";
                        if (t.IsConditionSatisfied(ref argIndex2))
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
            string argsptype3 = "絶対破壊";
            string argsptype4 = "絶対瀕死";
            if (IsUnderSpecialPowerEffect(ref argsptype3) | IsUnderSpecialPowerEffect(ref argsptype4))
            {
                if (!be_quiet)
                {
                    string argSituation10 = wname + "(命中)";
                    string argmsg_mode12 = "";
                    PilotMessage(ref argSituation10, msg_mode: ref argmsg_mode12);
                }

                bool localIsSpecialEffectDefined1() { string argmain_situation = wname + "(命中)"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                string argmain_situation16 = wname + "(命中)";
                string argsub_situation21 = "";
                string argsub_situation22 = "";
                if (IsAnimationDefined(ref argmain_situation16, sub_situation: ref argsub_situation21) | IsAnimationDefined(ref wname, sub_situation: ref argsub_situation22))
                {
                    string argmain_situation14 = wname + "(命中)";
                    string argsub_situation19 = "";
                    PlayAnimation(ref argmain_situation14, sub_situation: ref argsub_situation19);
                }
                else if (localIsSpecialEffectDefined1())
                {
                    string argmain_situation15 = wname + "(命中)";
                    string argsub_situation20 = "";
                    SpecialEffect(ref argmain_situation15, sub_situation: ref argsub_situation20);
                }
                else if (!Sound.IsWavePlayed)
                {
                    var argu3 = this;
                    Effect.HitEffect(ref argu3, ref w, ref t);
                }

                string argsptype2 = "絶対瀕死";
                if (IsUnderSpecialPowerEffect(ref argsptype2))
                {
                    // MOD START MARGE
                    // If t.HP > 10 Then
                    // dmg = t.HP - 10
                    // Else
                    // dmg = 0
                    // End If
                    string argoname3 = "ダメージ下限解除";
                    string argoname4 = "ダメージ下限１";
                    if (Expression.IsOptionDefined(ref argoname3) | Expression.IsOptionDefined(ref argoname4))
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
                // MOD END MARGE

                else
                {
                    dmg = t.HP;
                }

                goto ApplyDamage;
            }

            // 回避能力の処理
            if (prob > 0)
            {
                if (CheckDodgeFeature(w, ref t, ref tx, ref ty, ref attack_mode, ref def_mode, ref dmg, ref be_quiet))
                {
                    dmg = 0;
                    goto EndAttack;
                }
            }

            // 攻撃回数を求める
            string argattr13 = "連";
            if (IsWeaponClassifiedAs(w, ref argattr13))
            {
                string argattr12 = "連";
                attack_num = (int)WeaponLevel(w, ref argattr12);
            }
            else
            {
                attack_num = 1;
            }

            // 命中回数を求める
            hit_count = 0;
            var loopTo1 = (short)attack_num;
            for (i = 1; i <= loopTo1; i++)
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
                bool localIsSpecialEffectDefined2() { string argmain_situation = wname + "(回避)"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                string argmain_situation21 = wname + "(回避)";
                string argsub_situation27 = "";
                string argmain_situation22 = "回避";
                string argsub_situation28 = "";
                string argmain_situation23 = "回避";
                string argsub_situation29 = "";
                if (IsAnimationDefined(ref argmain_situation21, sub_situation: ref argsub_situation27))
                {
                    string argmain_situation17 = wname + "(回避)";
                    string argsub_situation23 = "";
                    PlayAnimation(ref argmain_situation17, sub_situation: ref argsub_situation23);
                }
                else if (localIsSpecialEffectDefined2())
                {
                    string argmain_situation18 = wname + "(回避)";
                    string argsub_situation24 = "";
                    SpecialEffect(ref argmain_situation18, sub_situation: ref argsub_situation24);
                }
                else if (t.IsAnimationDefined(ref argmain_situation22, sub_situation: ref argsub_situation28))
                {
                    string argmain_situation19 = "回避";
                    string argsub_situation25 = "";
                    t.PlayAnimation(ref argmain_situation19, sub_situation: ref argsub_situation25);
                }
                else if (t.IsSpecialEffectDefined(ref argmain_situation23, sub_situation: ref argsub_situation29))
                {
                    string argmain_situation20 = "回避";
                    string argsub_situation26 = "";
                    t.SpecialEffect(ref argmain_situation20, sub_situation: ref argsub_situation26);
                }
                else
                {
                    var argu4 = this;
                    Effect.DodgeEffect(ref argu4, ref w);
                }

                if (!be_quiet)
                {
                    string argSituation11 = "回避";
                    string argmsg_mode13 = "";
                    t.PilotMessage(ref argSituation11, msg_mode: ref argmsg_mode13);
                    string argSituation12 = wname + "(回避)";
                    string argmsg_mode14 = "";
                    PilotMessage(ref argSituation12, msg_mode: ref argmsg_mode14);
                }

                string argmain_situation25 = "回避";
                string argsub_situation31 = "";
                if (t.IsSysMessageDefined(ref argmain_situation25, sub_situation: ref argsub_situation31))
                {
                    string argmain_situation24 = "回避";
                    string argsub_situation30 = "";
                    string argadd_msg = "";
                    t.SysMessage(ref argmain_situation24, sub_situation: ref argsub_situation30, add_msg: ref argadd_msg);
                }
                else
                {
                    switch (def_mode ?? "")
                    {
                        case "回避":
                            {
                                object argIndex3 = "踊り";
                                if (t.IsConditionSatisfied(ref argIndex3))
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
            if (su is null)
            {
                use_support_guard = false;
                string argsptype5 = "みがわり";
                if (t.IsUnderSpecialPowerEffect(ref argsptype5))
                {
                    // スペシャルパワー「みがわり」
                    i = 1;
                    while (i <= t.CountSpecialPower())
                    {
                        // UPGRADE_WARNING: オブジェクト t.SpecialPower(i).IsEffectAvailable(みがわり) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        SpecialPowerData localSpecialPower() { object argIndex1 = i; var ret = t.SpecialPower(ref argIndex1); return ret; }

                        string argename = "みがわり";
                        if (Conversions.ToBoolean(localSpecialPower().IsEffectAvailable(ref argename)))
                        {
                            string localSpecialPowerData1() { object argIndex1 = i; var ret = t.SpecialPowerData(ref argIndex1); return ret; }

                            object argIndex4 = localSpecialPowerData1();
                            if (SRC.PList.IsDefined(ref argIndex4))
                            {
                                string localSpecialPowerData() { object argIndex1 = i; var ret = t.SpecialPowerData(ref argIndex1); return ret; }

                                Pilot localItem() { object argIndex1 = (object)hs40b56b80c15841019d507de2a6e31457(); var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                su = localItem().Unit_Renamed;
                                string argstype = "みがわり";
                                t.RemoveSpecialPowerInEffect(ref argstype);
                                i = (short)(i - 1);
                                if (su is object)
                                {
                                    su = su.CurrentForm();
                                    if (su.Status_Renamed != "出撃")
                                    {
                                        // UPGRADE_NOTE: オブジェクト su をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                                        su = null;
                                    }
                                }
                            }
                        }

                        i = (short)(i + 1);
                    }
                }
                else if (!is_event & def_mode != "マップ攻撃" & def_mode != "援護防御")
                {
                    if (t.IsDefense())
                    {
                        // サポートガード
                        if (Commands.UseSupportGuard)
                        {
                            var argt = this;
                            su = t.LookForSupportGuard(ref argt, w);
                            if (su is object)
                            {
                                use_support_guard = true;
                                // サポートガードの残り回数を減らす
                                su.UsedSupportGuard = (short)(su.UsedSupportGuard + 1);
                            }
                        }
                    }

                    if (su is null)
                    {
                        // かばう
                        var argt1 = this;
                        su = t.LookForGuardHelp(ref argt1, w, is_critical);
                    }
                }

                if (su is object)
                {
                    su.Update();

                    // メッセージウィンドウの表示を入れ替え
                    if (Party == "味方" | Party == "ＮＰＣ")
                    {
                        object argu22 = this;
                        GUI.UpdateMessageForm(ref su, ref argu22);
                    }
                    else
                    {
                        var argu12 = this;
                        object argu23 = su;
                        GUI.UpdateMessageForm(ref argu12, ref argu23);
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
                    bool localIsMessageDefined2() { string argmain_situation = "かばう(" + t.MainPilot().Name + ")"; var ret = su.IsMessageDefined(ref argmain_situation); return ret; }

                    bool localIsMessageDefined3() { string argmain_situation = "かばう(" + t.MainPilot().get_Nickname(false) + ")"; var ret = su.IsMessageDefined(ref argmain_situation); return ret; }

                    if (use_support_guard)
                    {
                        bool localIsMessageDefined1() { string argmain_situation = "サポートガード(" + t.MainPilot().get_Nickname(false) + ")"; var ret = su.IsMessageDefined(ref argmain_situation); return ret; }

                        string argmain_situation26 = "サポートガード(" + t.MainPilot().Name + ")";
                        string argmain_situation27 = "サポートガード";
                        if (su.IsMessageDefined(ref argmain_situation26))
                        {
                            string argSituation13 = "サポートガード(" + t.MainPilot().Name + ")";
                            string argmsg_mode15 = "";
                            su.PilotMessage(ref argSituation13, msg_mode: ref argmsg_mode15);
                        }
                        else if (localIsMessageDefined1())
                        {
                            string argSituation14 = "サポートガード(" + t.MainPilot().get_Nickname(false) + ")";
                            string argmsg_mode16 = "";
                            su.PilotMessage(ref argSituation14, msg_mode: ref argmsg_mode16);
                        }
                        else if (su.IsMessageDefined(ref argmain_situation27))
                        {
                            string argSituation15 = "サポートガード";
                            string argmsg_mode17 = "";
                            su.PilotMessage(ref argSituation15, msg_mode: ref argmsg_mode17);
                        }
                    }
                    else if (localIsMessageDefined2())
                    {
                        string argSituation16 = "かばう(" + t.MainPilot().Name + ")";
                        string argmsg_mode18 = "";
                        su.PilotMessage(ref argSituation16, msg_mode: ref argmsg_mode18);
                        use_protect_msg = true;
                    }
                    else if (localIsMessageDefined3())
                    {
                        string argSituation17 = "かばう(" + t.MainPilot().get_Nickname(false) + ")";
                        string argmsg_mode19 = "";
                        su.PilotMessage(ref argSituation17, msg_mode: ref argmsg_mode19);
                        use_protect_msg = true;
                    }

                    msg = su.MainPilot().get_Nickname(false) + "は[" + t.MainPilot().get_Nickname(false) + "]をかばった。;";

                    // 身代わりになるユニットをターゲットの位置まで移動
                    {
                        var withBlock2 = su;
                        // アニメ表示
                        if (SRC.BattleAnimation)
                        {
                            string argmain_situation29 = "サポートガード開始";
                            string argsub_situation33 = "";
                            if (su.IsAnimationDefined(ref argmain_situation29, sub_situation: ref argsub_situation33))
                            {
                                string argmain_situation28 = "サポートガード開始";
                                string argsub_situation32 = "";
                                su.PlayAnimation(ref argmain_situation28, sub_situation: ref argsub_situation32);
                            }
                            else if (!GUI.IsRButtonPressed())
                            {
                                if (use_support_guard)
                                {
                                    GUI.MoveUnitBitmap(ref su, withBlock2.x, withBlock2.y, tx, ty, 80, 4);
                                }
                                else
                                {
                                    GUI.MoveUnitBitmap(ref su, withBlock2.x, withBlock2.y, tx, ty, 50);
                                }
                            }
                        }

                        // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
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
                    Event_Renamed.SelectedTargetForEvent = t;
                }
            }

            if (su is object)
            {
                // ダメージを再計算
                {
                    var withBlock3 = t;
                    prev_hp = withBlock3.HP;
                    dmg = Damage(w, ref t, true);
                    if (is_critical)
                    {
                        string argoname5 = "ダメージ倍率低下";
                        if (Expression.IsOptionDefined(ref argoname5))
                        {
                            string argattr15 = "痛";
                            if (IsWeaponClassifiedAs(w, ref argattr15))
                            {
                                string argattr14 = "痛";
                                dmg = (int)((1d + 0.1d * (WeaponLevel(w, ref argattr14) + 2d)) * dmg);
                            }
                            else
                            {
                                dmg = (int)(1.2d * dmg);
                            }
                        }
                        else
                        {
                            string argattr17 = "痛";
                            if (IsWeaponClassifiedAs(w, ref argattr17))
                            {
                                string argattr16 = "痛";
                                dmg = (int)((1d + 0.25d * (WeaponLevel(w, ref argattr16) + 2d)) * dmg);
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
            bool argbe_quiet = be_quiet | use_protect_msg;
            if (CheckParryFeature(w, ref t, ref tx, ref ty, ref attack_mode, ref def_mode, ref dmg, ref msg, ref argbe_quiet))
            {
                dmg = 0;
                goto EndAttack;
            }

            // 防御＆かばう時はダメージを半減
            string argattr18 = "殺";
            if (!IsWeaponClassifiedAs(w, ref argattr18))
            {
                string argsptype6 = "無防備";
                string argfname1 = "防御不可";
                if (def_mode == "防御" & !t.IsUnderSpecialPowerEffect(ref argsptype6) & !t.IsFeatureAvailable(ref argfname1))
                {
                    dmg = dmg / 2;
                }
            }

            // ダミー
            if (CheckDummyFeature(w, ref t, be_quiet))
            {
                dmg = 0;
                goto EndAttack;
            }

            // これ以降は命中時の処理

            is_hit = true;

            // シールド防御判定
            CheckShieldFeature(w, ref t, dmg, be_quiet, ref use_shield, ref use_shield_msg);

            // 防御能力の処理
            bool argbe_quiet1 = be_quiet | use_protect_msg;
            if (CheckDefenseFeature(w, ref t, ref tx, ref ty, ref attack_mode, ref def_mode, ref dmg, ref msg, ref argbe_quiet1, ref is_penetrated))
            {
                if (!be_quiet)
                {
                    string argSituation18 = wname + "(攻撃無効化)";
                    string argmsg_mode20 = "";
                    PilotMessage(ref argSituation18, msg_mode: ref argmsg_mode20);
                }

                dmg = 0;
                goto EndAttack;
            }

            // 命中時の特殊効果を表示。
            // 防御能力の処理を先に行うのは攻撃無効化の特殊効果を優先させるため。
            Sound.IsWavePlayed = false;
            if (!be_quiet)
            {
                string argSituation19 = wname + "(命中)";
                string argmsg_mode21 = "";
                PilotMessage(ref argSituation19, msg_mode: ref argmsg_mode21);
            }

            bool localIsSpecialEffectDefined3() { string argmain_situation = wname + "(命中)"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

            string argmain_situation32 = wname + "(命中)";
            string argsub_situation36 = "";
            string argsub_situation37 = "";
            if (IsAnimationDefined(ref argmain_situation32, sub_situation: ref argsub_situation36) | IsAnimationDefined(ref wname, sub_situation: ref argsub_situation37))
            {
                string argmain_situation30 = wname + "(命中)";
                string argsub_situation34 = "";
                PlayAnimation(ref argmain_situation30, sub_situation: ref argsub_situation34);
            }
            else if (localIsSpecialEffectDefined3())
            {
                string argmain_situation31 = wname + "(命中)";
                string argsub_situation35 = "";
                SpecialEffect(ref argmain_situation31, sub_situation: ref argsub_situation35);
            }
            else if (!Sound.IsWavePlayed)
            {
                var argu5 = this;
                Effect.HitEffect(ref argu5, ref w, ref t, (short)hit_count);
            }

            string argmain_situation33 = wname + "(命中)";
            string argsub_situation38 = "";
            string argadd_msg1 = "";
            SysMessage(ref argmain_situation33, sub_situation: ref argsub_situation38, add_msg: ref argadd_msg1);

            // 無敵の場合
            object argIndex5 = "無敵";
            if (t.IsConditionSatisfied(ref argIndex5))
            {
                if (!be_quiet)
                {
                    string argSituation20 = "攻撃無効化";
                    string argmsg_mode22 = "";
                    t.PilotMessage(ref argSituation20, msg_mode: ref argmsg_mode22);
                    string argSituation21 = wname + "(攻撃無効化)";
                    string argmsg_mode23 = "";
                    PilotMessage(ref argSituation21, msg_mode: ref argmsg_mode23);
                }

                GUI.DisplaySysMessage(msg + t.Nickname + "は[" + wnickname + "]を無効化した！");
                dmg = 0;
                goto EndAttack;
            }

            // 抹殺攻撃は一撃で倒せる場合にしか効かない
            string argattr19 = "殺";
            if (IsWeaponClassifiedAs(w, ref argattr19))
            {
                if (t.HP > dmg)
                {
                    GUI.DisplaySysMessage(msg + t.Nickname + "は攻撃による影響を受けなかった。");
                    goto EndAttack;
                }
            }

            // ターゲット位置を変更する攻撃はサポートガードの場合は無効
            if (su is null & def_mode != "援護防御")
            {
                // 吹き飛ばし
                string argattr20 = "吹";
                string argattr21 = "Ｋ";
                if (IsWeaponClassifiedAs(w, ref argattr20) | IsWeaponClassifiedAs(w, ref argattr21))
                {
                    CheckBlowAttack(w, ref t, ref dmg, ref msg, ref attack_mode, ref def_mode, ref critical_type);
                }

                // 引き寄せ
                string argattr22 = "引";
                if (IsWeaponClassifiedAs(w, ref argattr22))
                {
                    CheckDrawAttack(w, ref t, ref msg, ref def_mode, ref critical_type);
                }

                // 強制転移
                string argattr23 = "転";
                if (IsWeaponClassifiedAs(w, ref argattr23))
                {
                    CheckTeleportAwayAttack(w, ref t, ref msg, ref def_mode, ref critical_type);
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
                string argattr24 = "破";
                if (IsWeaponClassifiedAs(w, ref argattr24))
                {
                    string argfname2 = "小型シールド";
                    if (t.IsFeatureAvailable(ref argfname2))
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
                    string argfname3 = "小型シールド";
                    if (t.IsFeatureAvailable(ref argfname3))
                    {
                        dmg = 2 * dmg / 3;
                    }
                    else
                    {
                        dmg = dmg / 2;
                    }
                }

                string argfname4 = "エネルギーシールド";
                string argattr26 = "無";
                string argsptype7 = "防御能力無効化";
                if (t.IsFeatureAvailable(ref argfname4) & t.EN > 5 & !IsWeaponClassifiedAs(w, ref argattr26) & !IsUnderSpecialPowerEffect(ref argsptype7))
                {
                    t.EN = t.EN - 5;
                    string argattr25 = "破";
                    if (IsWeaponClassifiedAs(w, ref argattr25))
                    {
                        object argIndex6 = "エネルギーシールド";
                        spower = (int)(50d * t.FeatureLevel(ref argIndex6));
                    }
                    else
                    {
                        object argIndex7 = "エネルギーシールド";
                        spower = (int)(100d * t.FeatureLevel(ref argIndex7));
                    }

                    if (dmg <= spower)
                    {
                        if (attack_mode != "反射")
                        {
                            var argu13 = this;
                            object argu24 = t;
                            GUI.UpdateMessageForm(ref argu13, ref argu24);
                        }
                        else
                        {
                            var argu14 = this;
                            object argu25 = null;
                            GUI.UpdateMessageForm(ref argu14, ref argu25);
                        }

                        object argIndex8 = "エネルギーシールド";
                        fname = t.FeatureName0(ref argIndex8);
                        if (!be_quiet)
                        {
                            string argmain_situation34 = "攻撃無効化(" + fname + ")";
                            if (t.IsMessageDefined(ref argmain_situation34))
                            {
                                string argSituation22 = "攻撃無効化(" + fname + ")";
                                string argmsg_mode24 = "";
                                t.PilotMessage(ref argSituation22, msg_mode: ref argmsg_mode24);
                            }
                            else
                            {
                                string argSituation23 = "攻撃無効化";
                                string argmsg_mode25 = "";
                                t.PilotMessage(ref argSituation23, msg_mode: ref argmsg_mode25);
                            }
                        }

                        string argmain_situation37 = "攻撃無効化";
                        if (t.IsAnimationDefined(ref argmain_situation37, ref fname))
                        {
                            string argmain_situation35 = "攻撃無効化";
                            t.PlayAnimation(ref argmain_situation35, ref fname);
                        }
                        else
                        {
                            string argmain_situation36 = "攻撃無効化";
                            t.SpecialEffect(ref argmain_situation36, ref fname);
                        }

                        GUI.DisplaySysMessage(msg + fname + "が攻撃を防いだ。");
                        goto EndAttack;
                    }

                    dmg = dmg - spower;
                }
            }

            // 最低ダメージは10
            if (dmg > 0 & dmg < 10)
            {
                dmg = 10;
            }

            // 都合により破壊させない場合
            string argsptype8 = "てかげん";
            object argIndex9 = "不死身";
            if (IsUnderSpecialPowerEffect(ref argsptype8) & this.MainPilot().Technique > t.MainPilot().Technique & Strings.InStr(attack_mode, "援護攻撃") == 0 | t.IsConditionSatisfied(ref argIndex9))
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
            CauseEffect(w, ref t, ref msg, ref critical_type, ref def_mode, dmg >= t.HP);
            if (Strings.InStr(critical_type, "即死") > 0 & !use_support_guard & !use_protect_msg)
            {
                if (t.IsHero())
                {
                    msg = msg + WeaponNickname(w) + "が" + t.Nickname + "の命を奪った。;";
                }
                else
                {
                    msg = msg + WeaponNickname(w) + "が" + t.Nickname + "を一撃で破壊した。;";
                }

                dmg = t.HP;
            }
            else if (t.HP - dmg < 0)
            {
                dmg = t.HP;
            }


            // 確実に発生する特殊効果
            short prev_en;
            string argattr27 = "減";
            string arganame2 = "減";
            string argattr28 = "奪";
            string arganame3 = "奪";
            if (IsWeaponClassifiedAs(w, ref argattr27) & !t.SpecialEffectImmune(ref arganame2))
            {
                string argtname = "ＥＮ";
                msg = msg + wnickname + "が[" + t.Nickname + "]の" + Expression.Term(ref argtname, ref t) + "を低下させた。;";
                t.EN = GeneralLib.MaxLng((int)(t.EN - t.MaxEN * (dmg / (double)t.MaxHP)), 0);
            }
            else if (IsWeaponClassifiedAs(w, ref argattr28) & !t.SpecialEffectImmune(ref arganame3))
            {
                string argtname1 = "ＥＮ";
                msg = msg + Nickname + "は[" + t.Nickname + "]の" + Expression.Term(ref argtname1, ref t) + "を吸収した。;";
                prev_en = (short)t.EN;
                t.EN = GeneralLib.MaxLng((int)(t.EN - t.MaxEN * (dmg / (double)t.MaxHP)), 0);
                EN = EN + (prev_en - t.EN) / 2;
            }

            // クリティカル時メッセージ
            if (is_critical | Strings.Len(critical_type) > 0)
            {
                if (!be_quiet)
                {
                    string argSituation24 = wname + "(クリティカル)";
                    string argmsg_mode26 = "";
                    PilotMessage(ref argSituation24, msg_mode: ref argmsg_mode26);
                }

                bool localIsSpecialEffectDefined4() { string argmain_situation = wname + "(クリティカル)"; string argsub_situation = ""; var ret = IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                string argmain_situation40 = wname + "(クリティカル)";
                string argsub_situation41 = "";
                if (IsAnimationDefined(ref argmain_situation40, sub_situation: ref argsub_situation41))
                {
                    string argmain_situation38 = wname + "(クリティカル)";
                    string argsub_situation39 = "";
                    PlayAnimation(ref argmain_situation38, sub_situation: ref argsub_situation39);
                }
                else if (localIsSpecialEffectDefined4())
                {
                    string argmain_situation39 = wname + "(クリティカル)";
                    string argsub_situation40 = "";
                    SpecialEffect(ref argmain_situation39, sub_situation: ref argsub_situation40);
                }
                else
                {
                    Effect.CriticalEffect(ref critical_type, w, use_support_guard | use_protect_msg);
                }
            }

        ApplyDamage:
            ;

            // ダメージの適用
            t.HP = t.HP - dmg;

            // ＨＰ吸収
            string argattr29 = "吸";
            string arganame4 = "吸";
            if (IsWeaponClassifiedAs(w, ref argattr29) & !t.SpecialEffectImmune(ref arganame4))
            {
                if (HP < MaxHP)
                {
                    string argtname2 = "ＨＰ";
                    msg = msg + Nickname + "は[" + t.Nickname + "]の" + Expression.Term(ref argtname2, ref t) + "を吸収した。;";
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
                var argu15 = this;
                object argu26 = t;
                GUI.UpdateMessageForm(ref argu15, ref argu26);
            }
            else
            {
                var argu16 = this;
                object argu27 = null;
                GUI.UpdateMessageForm(ref argu16, ref argu27);
            }

            // ダメージ量表示前にカットインは一旦消去しておく
            string argoname6 = "戦闘中画面初期化無効";
            if (!Expression.IsOptionDefined(ref argoname6) | attack_mode == "マップ攻撃")
            {
                if (GUI.IsPictureVisible)
                {
                    GUI.ClearPicture();
                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    GUI.MainForm.picMain(0).Refresh();
                }
            }

            // ダメージ量をマップウィンドウに表示
            string argoname7 = "ダメージ表示無効";
            if (!Expression.IsOptionDefined(ref argoname7) | attack_mode == "マップ攻撃")
            {
                string argmain_situation43 = wname + "(ダメージ表示)";
                string argsub_situation44 = "";
                string argmain_situation44 = "ダメージ表示";
                string argsub_situation45 = "";
                if (IsAnimationDefined(ref argmain_situation43, sub_situation: ref argsub_situation44))
                {
                    string argmain_situation41 = wname + "(ダメージ表示)";
                    string argsub_situation42 = "";
                    PlayAnimation(ref argmain_situation41, sub_situation: ref argsub_situation42);
                }
                else if (IsAnimationDefined(ref argmain_situation44, sub_situation: ref argsub_situation45))
                {
                    string argmain_situation42 = "ダメージ表示";
                    string argsub_situation43 = "";
                    PlayAnimation(ref argmain_situation42, sub_situation: ref argsub_situation43);
                }
                else
                {
                    string argtarea = "";
                    if (!SRC.BattleAnimation | WeaponPower(w, ref argtarea) > 0 | dmg > 0)
                    {
                        if (!SRC.BattleAnimation & su is object)
                        {
                            string argmsg = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(dmg);
                            GUI.DrawSysString(prev_x, prev_y, ref argmsg);
                        }
                        else
                        {
                            string argmsg1 = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(dmg);
                            GUI.DrawSysString(t.x, t.y, ref argmsg1);
                        }
                    }
                }
            }

            // 自動反撃発動
            if (t.HP > 0)
            {
                bool argbe_quiet2 = be_quiet | use_protect_msg;
                CheckAutoAttack(w, ref t, ref attack_mode, ref def_mode, ref dmg, ref argbe_quiet2);
                if (Status_Renamed != "出撃")
                {
                    goto EndAttack;
                }
            }

            // 破壊アニメ
            if (t.HP == 0)
            {
                string argmain_situation47 = "破壊";
                string argsub_situation48 = "";
                if (t.IsAnimationDefined(ref argmain_situation47, sub_situation: ref argsub_situation48))
                {
                    string argmain_situation45 = "破壊";
                    string argsub_situation46 = "";
                    t.PlayAnimation(ref argmain_situation45, sub_situation: ref argsub_situation46);
                }
                else
                {
                    string argmain_situation46 = "破壊";
                    string argsub_situation47 = "";
                    t.SpecialEffect(ref argmain_situation46, sub_situation: ref argsub_situation47);
                }
            }

            // パーツ分離が発動可能かチェック
            separate_parts = false;
            if (t.HP == 0)
            {
                string argfname5 = "パーツ分離";
                if (t.IsFeatureAvailable(ref argfname5))
                {
                    string localLIndex() { object argIndex1 = "パーツ分離"; string arglist = t.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    Unit localOtherForm() { object argIndex1 = (object)hs03c1bfcb86cd46eaab84349de7f47706(); var ret = t.OtherForm(ref argIndex1); return ret; }

                    if (localOtherForm().IsAbleToEnter(t.x, t.y))
                    {
                        object argIndex11 = "パーツ分離";
                        if (t.IsFeatureLevelSpecified(ref argIndex11))
                        {
                            object argIndex10 = "パーツ分離";
                            if (GeneralLib.Dice(100) <= 10d * t.FeatureLevel(ref argIndex10))
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
            if (attack_mode != "マップ攻撃" & !use_protect_msg & !use_shield_msg)
            {
                if (t.HP == 0)
                {
                    if (separate_parts)
                    {
                        object argIndex12 = "パーツ分離";
                        fname = t.FeatureName(ref argIndex12);
                        bool localIsMessageDefined4() { string argmain_situation = "破壊時分離(" + fname + ")"; var ret = t.IsMessageDefined(ref argmain_situation); return ret; }

                        bool localIsMessageDefined5() { string argmain_situation = "分離(" + t.Name + ")"; var ret = t.IsMessageDefined(ref argmain_situation); return ret; }

                        bool localIsMessageDefined6() { string argmain_situation = "分離(" + fname + ")"; var ret = t.IsMessageDefined(ref argmain_situation); return ret; }

                        string argmain_situation48 = "破壊時分離(" + t.Name + ")";
                        string argmain_situation49 = "破壊時";
                        string argmain_situation50 = "分離";
                        if (t.IsMessageDefined(ref argmain_situation48))
                        {
                            string argSituation25 = "破壊時分離(" + t.Name + ")";
                            string argmsg_mode27 = "";
                            t.PilotMessage(ref argSituation25, msg_mode: ref argmsg_mode27);
                        }
                        else if (localIsMessageDefined4())
                        {
                            string argSituation27 = "破壊時分離(" + fname + ")";
                            string argmsg_mode29 = "";
                            t.PilotMessage(ref argSituation27, msg_mode: ref argmsg_mode29);
                        }
                        else if (t.IsMessageDefined(ref argmain_situation49))
                        {
                            string argSituation28 = "破壊時分離";
                            string argmsg_mode30 = "";
                            t.PilotMessage(ref argSituation28, msg_mode: ref argmsg_mode30);
                        }
                        else if (localIsMessageDefined5())
                        {
                            string argSituation29 = "分離(" + t.Name + ")";
                            string argmsg_mode31 = "";
                            t.PilotMessage(ref argSituation29, msg_mode: ref argmsg_mode31);
                        }
                        else if (localIsMessageDefined6())
                        {
                            string argSituation30 = "分離(" + fname + ")";
                            string argmsg_mode32 = "";
                            t.PilotMessage(ref argSituation30, msg_mode: ref argmsg_mode32);
                        }
                        else if (t.IsMessageDefined(ref argmain_situation50))
                        {
                            string argSituation31 = "分離";
                            string argmsg_mode33 = "";
                            t.PilotMessage(ref argSituation31, msg_mode: ref argmsg_mode33);
                        }
                        else
                        {
                            string argSituation26 = "ダメージ大";
                            string argmsg_mode28 = "";
                            t.PilotMessage(ref argSituation26, msg_mode: ref argmsg_mode28);
                        }
                    }
                    else
                    {
                        string argSituation32 = "破壊";
                        string argmsg_mode34 = "";
                        t.PilotMessage(ref argSituation32, msg_mode: ref argmsg_mode34);
                    }
                }
            }

            if (!be_quiet)
            {
                if (t.HP == 0)
                {
                    // とどめメッセージ
                    string argSituation33 = wname + "(とどめ)";
                    string argmsg_mode35 = "";
                    PilotMessage(ref argSituation33, msg_mode: ref argmsg_mode35);
                }
                else
                {
                    // ダメージメッセージ
                    string argSituation34 = wname + "(ダメージ)";
                    string argmsg_mode36 = "";
                    PilotMessage(ref argSituation34, msg_mode: ref argmsg_mode36);
                }
            }

            // ダメージアニメ
            if (t.HP == 0)
            {
                // どどめアニメ
                if (attack_mode != "マップ攻撃" & attack_mode != "反射")
                {
                    string argmain_situation53 = wname + "(とどめ)";
                    string argsub_situation51 = "";
                    if (IsAnimationDefined(ref argmain_situation53, sub_situation: ref argsub_situation51))
                    {
                        string argmain_situation51 = wname + "(とどめ)";
                        string argsub_situation49 = "";
                        PlayAnimation(ref argmain_situation51, sub_situation: ref argsub_situation49);
                    }
                    else
                    {
                        string argmain_situation52 = wname + "(とどめ)";
                        string argsub_situation50 = "";
                        SpecialEffect(ref argmain_situation52, sub_situation: ref argsub_situation50);
                    }
                }
            }
            else if ((dmg <= 0.05d * t.MaxHP & t.HP >= 0.25d * t.MaxHP | dmg <= 10) & Strings.Len(critical_type) == 0)
            {
                // ダメージが非常に小さい
                string argmain_situation56 = "ダメージ小";
                string argsub_situation54 = "";
                if (t.IsAnimationDefined(ref argmain_situation56, sub_situation: ref argsub_situation54))
                {
                    string argmain_situation54 = "ダメージ小";
                    string argsub_situation52 = "";
                    t.PlayAnimation(ref argmain_situation54, sub_situation: ref argsub_situation52);
                }
                else
                {
                    string argmain_situation55 = "ダメージ小";
                    string argsub_situation53 = "";
                    t.SpecialEffect(ref argmain_situation55, sub_situation: ref argsub_situation53);
                }
            }
            else if (t.HP < 0.25d * t.MaxHP)
            {
                // ダメージ大
                string argmain_situation62 = "ダメージ大";
                string argsub_situation60 = "";
                if (t.IsAnimationDefined(ref argmain_situation62, sub_situation: ref argsub_situation60))
                {
                    string argmain_situation60 = "ダメージ大";
                    string argsub_situation58 = "";
                    t.PlayAnimation(ref argmain_situation60, sub_situation: ref argsub_situation58);
                }
                else
                {
                    string argmain_situation61 = "ダメージ大";
                    string argsub_situation59 = "";
                    t.SpecialEffect(ref argmain_situation61, sub_situation: ref argsub_situation59);
                }
            }
            else if (t.HP > 0.8d * t.MaxHP & Strings.Len(critical_type) == 0)
            {
                // ダメージ小
                string argmain_situation65 = "ダメージ小";
                string argsub_situation63 = "";
                if (t.IsAnimationDefined(ref argmain_situation65, sub_situation: ref argsub_situation63))
                {
                    string argmain_situation63 = "ダメージ小";
                    string argsub_situation61 = "";
                    t.PlayAnimation(ref argmain_situation63, sub_situation: ref argsub_situation61);
                }
                else
                {
                    string argmain_situation64 = "ダメージ小";
                    string argsub_situation62 = "";
                    t.SpecialEffect(ref argmain_situation64, sub_situation: ref argsub_situation62);
                }
            }
            else
            {
                // ダメージ中
                string argmain_situation59 = "ダメージ中";
                string argsub_situation57 = "";
                if (t.IsAnimationDefined(ref argmain_situation59, sub_situation: ref argsub_situation57))
                {
                    string argmain_situation57 = "ダメージ中";
                    string argsub_situation55 = "";
                    t.PlayAnimation(ref argmain_situation57, sub_situation: ref argsub_situation55);
                }
                else
                {
                    string argmain_situation58 = "ダメージ中";
                    string argsub_situation56 = "";
                    t.SpecialEffect(ref argmain_situation58, sub_situation: ref argsub_situation56);
                }
            }

            // ダメージメッセージ
            if (attack_mode != "マップ攻撃" & !use_protect_msg & !use_shield_msg)
            {
                string argmain_situation66 = "バリア貫通";
                if (t.HP == 0)
                {
                }
                // 破壊時メッセージは既に表示している
                else if ((dmg <= 0.05d * t.MaxHP & t.HP >= 0.25d * t.MaxHP | dmg <= 10) & Strings.Len(critical_type) == 0)
                {
                    // ダメージが非常に小さい
                    string argSituation36 = "ダメージ小";
                    string argmsg_mode38 = "";
                    t.PilotMessage(ref argSituation36, msg_mode: ref argmsg_mode38);
                }
                else if (t.HP < 0.25d * t.MaxHP)
                {
                    // ダメージ大
                    string argSituation37 = "ダメージ大";
                    string argmsg_mode39 = "";
                    t.PilotMessage(ref argSituation37, msg_mode: ref argmsg_mode39);
                }
                else if (is_penetrated & t.IsMessageDefined(ref argmain_situation66))
                {
                    string argSituation38 = "バリア貫通";
                    string argmsg_mode40 = "";
                    t.PilotMessage(ref argSituation38, msg_mode: ref argmsg_mode40);
                }
                else if (t.HP >= 0.8d * t.MaxHP & Strings.Len(critical_type) == 0)
                {
                    // ステータス異常が起こった場合は最低でもダメージ中のメッセージ
                    string argSituation39 = "ダメージ小";
                    string argmsg_mode41 = "";
                    t.PilotMessage(ref argSituation39, msg_mode: ref argmsg_mode41);
                }
                else
                {
                    string argSituation35 = "ダメージ中";
                    string argmsg_mode37 = "";
                    t.PilotMessage(ref argSituation35, msg_mode: ref argmsg_mode37);
                }
            }

            // シールド防御
            if (use_shield & t.HP > 0)
            {
                string argfname6 = "シールド";
                string argfname7 = "エネルギーシールド";
                string argattr30 = "無";
                string argsptype9 = "防御能力無効化";
                string argfname8 = "小型シールド";
                string argfname9 = "大型シールド";
                string argfname10 = "アクティブシールド";
                if (t.IsFeatureAvailable(ref argfname6))
                {
                    object argIndex13 = "シールド";
                    fname = t.FeatureName(ref argIndex13);
                    string argmain_situation68 = "シールド防御";
                    if (t.IsSysMessageDefined(ref argmain_situation68, ref fname))
                    {
                        string argmain_situation67 = "シールド防御";
                        string argadd_msg2 = "";
                        t.SysMessage(ref argmain_situation67, ref fname, add_msg: ref argadd_msg2);
                    }
                    else
                    {
                        msg = msg + t.Nickname + "は[" + fname + "]で防御した。;";
                    }
                }
                else if (t.IsFeatureAvailable(ref argfname7) & t.EN > 5 & !IsWeaponClassifiedAs(w, ref argattr30) & !IsUnderSpecialPowerEffect(ref argsptype9))
                {
                    t.EN = t.EN - 5;
                    object argIndex14 = "エネルギーシールド";
                    fname = t.FeatureName(ref argIndex14);
                    string argmain_situation70 = "シールド防御";
                    if (t.IsSysMessageDefined(ref argmain_situation70, ref fname))
                    {
                        string argmain_situation69 = "シールド防御";
                        string argadd_msg3 = "";
                        t.SysMessage(ref argmain_situation69, ref fname, add_msg: ref argadd_msg3);
                    }
                    else
                    {
                        msg = msg + t.Nickname + "は[" + fname + "]を展開した。;";
                    }
                }
                else if (t.IsFeatureAvailable(ref argfname8))
                {
                    object argIndex15 = "小型シールド";
                    fname = t.FeatureName(ref argIndex15);
                    string argmain_situation72 = "シールド防御";
                    if (t.IsSysMessageDefined(ref argmain_situation72, ref fname))
                    {
                        string argmain_situation71 = "シールド防御";
                        string argadd_msg4 = "";
                        t.SysMessage(ref argmain_situation71, ref fname, add_msg: ref argadd_msg4);
                    }
                    else
                    {
                        msg = msg + t.Nickname + "は[" + fname + "]で防御した。;";
                    }
                }
                else if (t.IsFeatureAvailable(ref argfname9))
                {
                    object argIndex16 = "大型シールド";
                    fname = t.FeatureName(ref argIndex16);
                    string argmain_situation74 = "シールド防御";
                    if (t.IsSysMessageDefined(ref argmain_situation74, ref fname))
                    {
                        string argmain_situation73 = "シールド防御";
                        string argadd_msg5 = "";
                        t.SysMessage(ref argmain_situation73, ref fname, add_msg: ref argadd_msg5);
                    }
                    else
                    {
                        msg = msg + t.Nickname + "は[" + fname + "]で防御した。;";
                    }
                }
                else if (t.IsFeatureAvailable(ref argfname10))
                {
                    object argIndex17 = "アクティブシールド";
                    fname = t.FeatureName(ref argIndex17);
                    string argmain_situation76 = "シールド防御";
                    if (t.IsSysMessageDefined(ref argmain_situation76, ref fname))
                    {
                        string argmain_situation75 = "シールド防御";
                        string argadd_msg6 = "";
                        t.SysMessage(ref argmain_situation75, ref fname, add_msg: ref argadd_msg6);
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
                object argIndex20 = "データ不明";
                if (dmg == 0)
                {
                    string argattr31 = "盗";
                    string argattr32 = "習";
                    string argattr33 = "写";
                    string argattr34 = "化";
                    if (Strings.Len(critical_type) > 0)
                    {
                        GUI.DisplaySysMessage(msg);
                    }
                    else if (IsWeaponClassifiedAs(w, ref argattr31))
                    {
                        // 盗み失敗
                        object argIndex18 = "すかんぴん";
                        if (t.IsConditionSatisfied(ref argIndex18))
                        {
                            GUI.DisplaySysMessage(msg + t.Nickname + "は盗める物を持っていなかった。");
                        }
                        else
                        {
                            GUI.DisplaySysMessage(msg + t.Nickname + "は素早く持ち物を守った。");
                        }
                    }
                    else if (IsWeaponClassifiedAs(w, ref argattr32))
                    {
                        // ラーニング失敗
                        string argfname11 = "ラーニング可能技";
                        if (t.IsFeatureAvailable(ref argfname11))
                        {
                            object argIndex19 = "ラーニング可能技";
                            buf = t.FeatureData(ref argIndex19);
                            switch (GeneralLib.LIndex(ref buf, 2) ?? "")
                            {
                                case "表示":
                                case var @case when @case == "":
                                    {
                                        fname = GeneralLib.LIndex(ref buf, 1);
                                        break;
                                    }

                                default:
                                    {
                                        fname = GeneralLib.LIndex(ref buf, 2);
                                        break;
                                    }
                            }

                            string argsname = GeneralLib.LIndex(ref buf, 1);
                            if (MainPilot().IsSkillAvailable(ref argsname))
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
                    else if (IsWeaponClassifiedAs(w, ref argattr33) | IsWeaponClassifiedAs(w, ref argattr34))
                    {
                    }
                    // 能力コピーの判定はこれから
                    else
                    {
                        GUI.DisplaySysMessage(msg + t.Nickname + "は攻撃による影響を受けなかった。");
                    }
                }
                else if (t.IsConditionSatisfied(ref argIndex20))
                {
                    if (attack_num > 1)
                    {
                        msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(hit_count) + "回命中し、";
                    }

                    GUI.DisplaySysMessage(msg + t.Nickname + "は[" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(dmg) + "]のダメージを受けた。");
                }
                else
                {
                    if (attack_num > 1)
                    {
                        msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(hit_count) + "回命中し、";
                    }

                    GUI.DisplaySysMessage(msg + t.Nickname + "は[" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(dmg) + "]のダメージを受けた。;" + "残りＨＰは" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(t.HP) + "（損傷率 = " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(100 * (t.MaxHP - t.HP) / t.MaxHP) + "％）");
                }

                // 特殊能力「不安定」による暴走チェック
                string argfname12 = "不安定";
                if (t.IsFeatureAvailable(ref argfname12))
                {
                    object argIndex23 = "暴走";
                    if (t.HP <= t.MaxHP / 4 & !t.IsConditionSatisfied(ref argIndex23))
                    {
                        string argcname = "暴走";
                        string argcdata = "";
                        t.AddCondition(ref argcname, -1, cdata: ref argcdata);
                        t.Update();
                        if (t.IsHero())
                        {
                            GUI.DisplaySysMessage(t.Nickname + "は暴走した。");
                        }
                        else
                        {
                            object argIndex22 = "不安定";
                            if (Strings.Len(t.FeatureName(ref argIndex22)) > 0)
                            {
                                object argIndex21 = "不安定";
                                GUI.DisplaySysMessage(t.Nickname + "は[" + t.FeatureName(ref argIndex21) + "]の暴走のために制御不能に陥った。");
                            }
                            else
                            {
                                GUI.DisplaySysMessage(t.Nickname + "は制御不能に陥った。");
                            }
                        }
                    }
                }

                // ダメージを受ければ眠りからさめる
                object argIndex25 = "睡眠";
                string argattr35 = "眠";
                if (t.IsConditionSatisfied(ref argIndex25) & !IsWeaponClassifiedAs(w, ref argattr35))
                {
                    object argIndex24 = "睡眠";
                    t.DeleteCondition(ref argIndex24);
                    GUI.DisplaySysMessage(t.Nickname + "は眠りから覚めた。");
                }

                // ダメージを受けると凍結解除
                object argIndex27 = "凍結";
                string argattr36 = "凍";
                if (t.IsConditionSatisfied(ref argIndex27) & !IsWeaponClassifiedAs(w, ref argattr36))
                {
                    object argIndex26 = "凍結";
                    t.DeleteCondition(ref argIndex26);
                    GUI.DisplaySysMessage(t.Nickname + "は凍結状態から開放された。");
                }
            }

            // 破壊された場合の処理
            short morale_mod;
            if (t.HP == 0)
            {
                if (attack_num > 1)
                {
                    msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(hit_count) + "回命中し、";
                }

                string argmain_situation78 = "破壊";
                string argsub_situation65 = "";
                if (t.IsSysMessageDefined(ref argmain_situation78, sub_situation: ref argsub_situation65))
                {
                    string argmain_situation77 = "破壊";
                    string argsub_situation64 = "";
                    string argadd_msg7 = "";
                    t.SysMessage(ref argmain_situation77, sub_situation: ref argsub_situation64, add_msg: ref argadd_msg7);
                }
                else if (t.IsHero())
                {
                    GUI.DisplaySysMessage(msg + t.Nickname + "は[" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(dmg) + "]のダメージを受け倒された。");
                }
                else
                {
                    GUI.DisplaySysMessage(msg + t.Nickname + "は[" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(dmg) + "]のダメージを受け破壊された。");
                }

                // 復活するかどうかのチェックを行う

                // スペシャルパワー「復活」
                string argsptype10 = "復活";
                if (t.IsUnderSpecialPowerEffect(ref argsptype10))
                {
                    string argstype1 = "破壊";
                    t.RemoveSpecialPowerInEffect(ref argstype1);
                    goto Resurrect;
                }

                // パイロット用特殊能力「英雄」＆「再生」
                string argsptype11 = "絶対破壊";
                if (!is_event & !IsUnderSpecialPowerEffect(ref argsptype11))
                {
                    object argIndex29 = "英雄";
                    string argref_mode = "";
                    if (GeneralLib.Dice(16) <= t.MainPilot().SkillLevel(ref argIndex29, ref_mode: ref argref_mode))
                    {
                        t.HP = t.MaxHP / 2;
                        t.IncreaseMorale(10);
                        string argmain_situation79 = "復活";
                        if (t.IsMessageDefined(ref argmain_situation79))
                        {
                            string argSituation40 = "復活";
                            string argmsg_mode42 = "";
                            t.PilotMessage(ref argSituation40, msg_mode: ref argmsg_mode42);
                        }

                        string argmain_situation82 = "復活";
                        string argsub_situation68 = "";
                        if (t.IsAnimationDefined(ref argmain_situation82, sub_situation: ref argsub_situation68))
                        {
                            string argmain_situation80 = "復活";
                            string argsub_situation66 = "";
                            t.PlayAnimation(ref argmain_situation80, sub_situation: ref argsub_situation66);
                        }
                        else
                        {
                            string argmain_situation81 = "復活";
                            string argsub_situation67 = "";
                            t.SpecialEffect(ref argmain_situation81, sub_situation: ref argsub_situation67);
                        }

                        object argIndex28 = "英雄";
                        buf = t.MainPilot().SkillName0(ref argIndex28);
                        if (buf == "非表示")
                        {
                            buf = "英雄";
                        }

                        string argmain_situation84 = "復活";
                        if (t.IsSysMessageDefined(ref argmain_situation84, ref buf))
                        {
                            string argmain_situation83 = "復活";
                            string argadd_msg8 = "";
                            t.SysMessage(ref argmain_situation83, ref buf, add_msg: ref argadd_msg8);
                        }
                        else
                        {
                            GUI.DisplaySysMessage(t.MainPilot().get_Nickname(false) + "の熱き" + buf + "の心が[" + t.Nickname + "]を復活させた！");
                        }

                        goto Resurrect;
                    }

                    // 浄化の適用
                    string argsname4 = "再生";
                    if (t.MainPilot().IsSkillAvailable(ref argsname4))
                    {
                        string argattr37 = "浄";
                        if (IsWeaponClassifiedAs(w, ref argattr37))
                        {
                            string argsname1 = "浄化";
                            if (MainPilot().IsSkillAvailable(ref argsname1))
                            {
                                bool localIsMessageDefined7() { string argmain_situation = "浄解(" + wname + ")"; var ret = IsMessageDefined(ref argmain_situation); return ret; }

                                string argmain_situation97 = "浄化(" + wname + ")";
                                string argmain_situation98 = "浄化";
                                string argmain_situation99 = "浄解";
                                if (IsMessageDefined(ref argmain_situation97))
                                {
                                    string argSituation41 = "浄化(" + wname + ")";
                                    string argmsg_mode43 = "";
                                    PilotMessage(ref argSituation41, msg_mode: ref argmsg_mode43);
                                    string argmain_situation87 = "浄化";
                                    if (IsAnimationDefined(ref argmain_situation87, ref wname))
                                    {
                                        string argmain_situation85 = "浄化";
                                        PlayAnimation(ref argmain_situation85, ref wname);
                                    }
                                    else
                                    {
                                        string argmain_situation86 = "浄化";
                                        SpecialEffect(ref argmain_situation86, ref wname);
                                    }
                                }
                                else if (IsMessageDefined(ref argmain_situation98))
                                {
                                    string argSituation42 = "浄化";
                                    string argmsg_mode44 = "";
                                    PilotMessage(ref argSituation42, msg_mode: ref argmsg_mode44);
                                    string argmain_situation90 = "浄化";
                                    if (IsAnimationDefined(ref argmain_situation90, ref wname))
                                    {
                                        string argmain_situation88 = "浄化";
                                        PlayAnimation(ref argmain_situation88, ref wname);
                                    }
                                    else
                                    {
                                        string argmain_situation89 = "浄化";
                                        SpecialEffect(ref argmain_situation89, ref wname);
                                    }
                                }
                                else if (localIsMessageDefined7())
                                {
                                    string argSituation43 = "浄解(" + wname + ")";
                                    string argmsg_mode45 = "";
                                    PilotMessage(ref argSituation43, msg_mode: ref argmsg_mode45);
                                    string argmain_situation93 = "浄解";
                                    if (IsAnimationDefined(ref argmain_situation93, ref wname))
                                    {
                                        string argmain_situation91 = "浄解";
                                        PlayAnimation(ref argmain_situation91, ref wname);
                                    }
                                    else
                                    {
                                        string argmain_situation92 = "浄解";
                                        SpecialEffect(ref argmain_situation92, ref wname);
                                    }
                                }
                                else if (IsMessageDefined(ref argmain_situation99))
                                {
                                    string argSituation44 = "浄解";
                                    string argmsg_mode46 = "";
                                    PilotMessage(ref argSituation44, msg_mode: ref argmsg_mode46);
                                    string argmain_situation96 = "浄解";
                                    if (IsAnimationDefined(ref argmain_situation96, ref wname))
                                    {
                                        string argmain_situation94 = "浄解";
                                        PlayAnimation(ref argmain_situation94, ref wname);
                                    }
                                    else
                                    {
                                        string argmain_situation95 = "浄解";
                                        SpecialEffect(ref argmain_situation95, ref wname);
                                    }
                                }

                                string argmain_situation101 = "浄化";
                                string argsub_situation70 = "";
                                if (IsSysMessageDefined(ref argmain_situation101, sub_situation: ref argsub_situation70))
                                {
                                    string argmain_situation100 = "浄化";
                                    string argsub_situation69 = "";
                                    string argadd_msg9 = "";
                                    SysMessage(ref argmain_situation100, sub_situation: ref argsub_situation69, add_msg: ref argadd_msg9);
                                }
                                else
                                {
                                    GUI.DisplaySysMessage(MainPilot().get_Nickname(false) + "は浄化を行って[" + t.Nickname + "]の復活を防いだ。");
                                }

                                goto Cure;
                            }

                            var loopTo2 = CountPilot();
                            for (i = 2; i <= loopTo2; i++)
                            {
                                Pilot localPilot1() { object argIndex1 = i; var ret = Pilot(ref argIndex1); return ret; }

                                string argsname2 = "浄化";
                                if (localPilot1().IsSkillAvailable(ref argsname2))
                                {
                                    bool localIsMessageDefined8() { string argmain_situation = "浄解(" + wname + ")"; var ret = IsMessageDefined(ref argmain_situation); return ret; }

                                    string argmain_situation114 = "浄化(" + wname + ")";
                                    string argmain_situation115 = "浄化";
                                    string argmain_situation116 = "浄解";
                                    if (IsMessageDefined(ref argmain_situation114))
                                    {
                                        string argSituation45 = "浄化(" + wname + ")";
                                        string argmsg_mode47 = "";
                                        PilotMessage(ref argSituation45, msg_mode: ref argmsg_mode47);
                                        string argmain_situation104 = "浄化";
                                        if (IsAnimationDefined(ref argmain_situation104, ref wname))
                                        {
                                            string argmain_situation102 = "浄化";
                                            PlayAnimation(ref argmain_situation102, ref wname);
                                        }
                                        else
                                        {
                                            string argmain_situation103 = "浄化";
                                            SpecialEffect(ref argmain_situation103, ref wname);
                                        }
                                    }
                                    else if (IsMessageDefined(ref argmain_situation115))
                                    {
                                        string argSituation46 = "浄化";
                                        string argmsg_mode48 = "";
                                        PilotMessage(ref argSituation46, msg_mode: ref argmsg_mode48);
                                        string argmain_situation107 = "浄化";
                                        if (IsAnimationDefined(ref argmain_situation107, ref wname))
                                        {
                                            string argmain_situation105 = "浄化";
                                            PlayAnimation(ref argmain_situation105, ref wname);
                                        }
                                        else
                                        {
                                            string argmain_situation106 = "浄化";
                                            SpecialEffect(ref argmain_situation106, ref wname);
                                        }
                                    }
                                    else if (localIsMessageDefined8())
                                    {
                                        string argSituation47 = "浄解(" + wname + ")";
                                        string argmsg_mode49 = "";
                                        PilotMessage(ref argSituation47, msg_mode: ref argmsg_mode49);
                                        string argmain_situation110 = "浄解";
                                        if (IsAnimationDefined(ref argmain_situation110, ref wname))
                                        {
                                            string argmain_situation108 = "浄解";
                                            PlayAnimation(ref argmain_situation108, ref wname);
                                        }
                                        else
                                        {
                                            string argmain_situation109 = "浄解";
                                            SpecialEffect(ref argmain_situation109, ref wname);
                                        }
                                    }
                                    else if (IsMessageDefined(ref argmain_situation116))
                                    {
                                        string argSituation48 = "浄解";
                                        string argmsg_mode50 = "";
                                        PilotMessage(ref argSituation48, msg_mode: ref argmsg_mode50);
                                        string argmain_situation113 = "浄解";
                                        if (IsAnimationDefined(ref argmain_situation113, ref wname))
                                        {
                                            string argmain_situation111 = "浄解";
                                            PlayAnimation(ref argmain_situation111, ref wname);
                                        }
                                        else
                                        {
                                            string argmain_situation112 = "浄解";
                                            SpecialEffect(ref argmain_situation112, ref wname);
                                        }
                                    }

                                    string argmain_situation118 = "浄化";
                                    string argsub_situation72 = "";
                                    if (IsSysMessageDefined(ref argmain_situation118, sub_situation: ref argsub_situation72))
                                    {
                                        string argmain_situation117 = "浄化";
                                        string argsub_situation71 = "";
                                        string argadd_msg10 = "";
                                        SysMessage(ref argmain_situation117, sub_situation: ref argsub_situation71, add_msg: ref argadd_msg10);
                                    }
                                    else
                                    {
                                        Pilot localPilot() { object argIndex1 = i; var ret = Pilot(ref argIndex1); return ret; }

                                        GUI.DisplaySysMessage(localPilot().get_Nickname(false) + "は浄化を行って[" + t.Nickname + "]の復活を防いだ。");
                                    }

                                    goto Cure;
                                }
                            }

                            var loopTo3 = CountSupport();
                            for (i = 1; i <= loopTo3; i++)
                            {
                                Pilot localSupport1() { object argIndex1 = i; var ret = Support(ref argIndex1); return ret; }

                                string argsname3 = "浄化";
                                if (localSupport1().IsSkillAvailable(ref argsname3))
                                {
                                    bool localIsMessageDefined9() { string argmain_situation = "浄解(" + wname + ")"; var ret = IsMessageDefined(ref argmain_situation); return ret; }

                                    string argmain_situation131 = "浄化(" + wname + ")";
                                    string argmain_situation132 = "浄化";
                                    string argmain_situation133 = "浄解";
                                    if (IsMessageDefined(ref argmain_situation131))
                                    {
                                        string argSituation49 = "浄化(" + wname + ")";
                                        string argmsg_mode51 = "";
                                        PilotMessage(ref argSituation49, msg_mode: ref argmsg_mode51);
                                        string argmain_situation121 = "浄化";
                                        if (IsAnimationDefined(ref argmain_situation121, ref wname))
                                        {
                                            string argmain_situation119 = "浄化";
                                            PlayAnimation(ref argmain_situation119, ref wname);
                                        }
                                        else
                                        {
                                            string argmain_situation120 = "浄化";
                                            SpecialEffect(ref argmain_situation120, ref wname);
                                        }
                                    }
                                    else if (IsMessageDefined(ref argmain_situation132))
                                    {
                                        string argSituation50 = "浄化";
                                        string argmsg_mode52 = "";
                                        PilotMessage(ref argSituation50, msg_mode: ref argmsg_mode52);
                                        string argmain_situation124 = "浄化";
                                        if (IsAnimationDefined(ref argmain_situation124, ref wname))
                                        {
                                            string argmain_situation122 = "浄化";
                                            PlayAnimation(ref argmain_situation122, ref wname);
                                        }
                                        else
                                        {
                                            string argmain_situation123 = "浄化";
                                            SpecialEffect(ref argmain_situation123, ref wname);
                                        }
                                    }
                                    else if (localIsMessageDefined9())
                                    {
                                        string argSituation51 = "浄解(" + wname + ")";
                                        string argmsg_mode53 = "";
                                        PilotMessage(ref argSituation51, msg_mode: ref argmsg_mode53);
                                        string argmain_situation127 = "浄解";
                                        if (IsAnimationDefined(ref argmain_situation127, ref wname))
                                        {
                                            string argmain_situation125 = "浄解";
                                            PlayAnimation(ref argmain_situation125, ref wname);
                                        }
                                        else
                                        {
                                            string argmain_situation126 = "浄解";
                                            SpecialEffect(ref argmain_situation126, ref wname);
                                        }
                                    }
                                    else if (IsMessageDefined(ref argmain_situation133))
                                    {
                                        string argSituation52 = "浄解";
                                        string argmsg_mode54 = "";
                                        PilotMessage(ref argSituation52, msg_mode: ref argmsg_mode54);
                                        string argmain_situation130 = "浄解";
                                        if (IsAnimationDefined(ref argmain_situation130, ref wname))
                                        {
                                            string argmain_situation128 = "浄解";
                                            PlayAnimation(ref argmain_situation128, ref wname);
                                        }
                                        else
                                        {
                                            string argmain_situation129 = "浄解";
                                            SpecialEffect(ref argmain_situation129, ref wname);
                                        }
                                    }

                                    string argmain_situation135 = "浄化";
                                    string argsub_situation74 = "";
                                    if (IsSysMessageDefined(ref argmain_situation135, sub_situation: ref argsub_situation74))
                                    {
                                        string argmain_situation134 = "浄化";
                                        string argsub_situation73 = "";
                                        string argadd_msg11 = "";
                                        SysMessage(ref argmain_situation134, sub_situation: ref argsub_situation73, add_msg: ref argadd_msg11);
                                    }
                                    else
                                    {
                                        Pilot localSupport() { object argIndex1 = i; var ret = Support(ref argIndex1); return ret; }

                                        GUI.DisplaySysMessage(localSupport().get_Nickname(false) + "は浄化を行って[" + t.Nickname + "]の復活を防いだ。");
                                    }

                                    goto Cure;
                                }
                            }

                            if (IsHero())
                            {
                                bool localIsMessageDefined10() { string argmain_situation = "浄解(" + wname + ")"; var ret = IsMessageDefined(ref argmain_situation); return ret; }

                                string argmain_situation150 = "浄化(" + wname + ")";
                                string argmain_situation151 = "浄化";
                                string argmain_situation152 = "浄解";
                                if (IsMessageDefined(ref argmain_situation150))
                                {
                                    string argSituation53 = "浄化(" + wname + ")";
                                    string argmsg_mode55 = "";
                                    PilotMessage(ref argSituation53, msg_mode: ref argmsg_mode55);
                                    string argmain_situation138 = "浄化";
                                    if (IsAnimationDefined(ref argmain_situation138, ref wname))
                                    {
                                        string argmain_situation136 = "浄化";
                                        PlayAnimation(ref argmain_situation136, ref wname);
                                    }
                                    else
                                    {
                                        string argmain_situation137 = "浄化";
                                        SpecialEffect(ref argmain_situation137, ref wname);
                                    }

                                    GUI.DisplaySysMessage(MainPilot().get_Nickname(false) + "は浄化を行って[" + t.Nickname + "]の復活を防いだ。");
                                }
                                else if (IsMessageDefined(ref argmain_situation151))
                                {
                                    string argSituation54 = "浄化";
                                    string argmsg_mode56 = "";
                                    PilotMessage(ref argSituation54, msg_mode: ref argmsg_mode56);
                                    string argmain_situation141 = "浄化";
                                    if (IsAnimationDefined(ref argmain_situation141, ref wname))
                                    {
                                        string argmain_situation139 = "浄化";
                                        PlayAnimation(ref argmain_situation139, ref wname);
                                    }
                                    else
                                    {
                                        string argmain_situation140 = "浄化";
                                        SpecialEffect(ref argmain_situation140, ref wname);
                                    }

                                    GUI.DisplaySysMessage(MainPilot().get_Nickname(false) + "は浄化を行って[" + t.Nickname + "]の復活を防いだ。");
                                }
                                else if (localIsMessageDefined10())
                                {
                                    string argSituation55 = "浄解(" + wname + ")";
                                    string argmsg_mode57 = "";
                                    PilotMessage(ref argSituation55, msg_mode: ref argmsg_mode57);
                                    string argmain_situation144 = "浄解";
                                    if (IsAnimationDefined(ref argmain_situation144, ref wname))
                                    {
                                        string argmain_situation142 = "浄解";
                                        PlayAnimation(ref argmain_situation142, ref wname);
                                    }
                                    else
                                    {
                                        string argmain_situation143 = "浄解";
                                        SpecialEffect(ref argmain_situation143, ref wname);
                                    }

                                    GUI.DisplaySysMessage(MainPilot().get_Nickname(false) + "は浄化を行って[" + t.Nickname + "]の復活を防いだ。");
                                }
                                else if (IsMessageDefined(ref argmain_situation152))
                                {
                                    string argSituation56 = "浄解";
                                    string argmsg_mode58 = "";
                                    PilotMessage(ref argSituation56, msg_mode: ref argmsg_mode58);
                                    string argmain_situation147 = "浄解";
                                    if (IsAnimationDefined(ref argmain_situation147, ref wname))
                                    {
                                        string argmain_situation145 = "浄解";
                                        PlayAnimation(ref argmain_situation145, ref wname);
                                    }
                                    else
                                    {
                                        string argmain_situation146 = "浄解";
                                        SpecialEffect(ref argmain_situation146, ref wname);
                                    }

                                    string argmain_situation149 = "浄化";
                                    string argsub_situation76 = "";
                                    if (IsSysMessageDefined(ref argmain_situation149, sub_situation: ref argsub_situation76))
                                    {
                                        string argmain_situation148 = "浄化";
                                        string argsub_situation75 = "";
                                        string argadd_msg12 = "";
                                        SysMessage(ref argmain_situation148, sub_situation: ref argsub_situation75, add_msg: ref argadd_msg12);
                                    }
                                    else
                                    {
                                        GUI.DisplaySysMessage(MainPilot().get_Nickname(false) + "は浄化を行って[" + t.Nickname + "]の復活を防いだ。");
                                    }
                                }

                                goto Cure;
                            }
                        }

                        object argIndex31 = "再生";
                        string argref_mode1 = "";
                        if (GeneralLib.Dice(16) <= t.MainPilot().SkillLevel(ref argIndex31, ref_mode: ref argref_mode1))
                        {
                            t.HP = t.MaxHP / 2;
                            string argmain_situation153 = "復活";
                            if (t.IsMessageDefined(ref argmain_situation153))
                            {
                                string argSituation57 = "復活";
                                string argmsg_mode59 = "";
                                t.PilotMessage(ref argSituation57, msg_mode: ref argmsg_mode59);
                            }

                            string argmain_situation156 = "復活";
                            string argsub_situation79 = "";
                            if (t.IsAnimationDefined(ref argmain_situation156, sub_situation: ref argsub_situation79))
                            {
                                string argmain_situation154 = "復活";
                                string argsub_situation77 = "";
                                t.PlayAnimation(ref argmain_situation154, sub_situation: ref argsub_situation77);
                            }
                            else
                            {
                                string argmain_situation155 = "復活";
                                string argsub_situation78 = "";
                                t.SpecialEffect(ref argmain_situation155, sub_situation: ref argsub_situation78);
                            }

                            object argIndex30 = "再生";
                            buf = t.MainPilot().SkillName0(ref argIndex30);
                            if (buf == "非表示")
                            {
                                buf = "再生";
                            }

                            string argmain_situation158 = "再生";
                            if (t.IsSysMessageDefined(ref argmain_situation158, ref buf))
                            {
                                string argmain_situation157 = "再生";
                                string argadd_msg13 = "";
                                t.SysMessage(ref argmain_situation157, ref buf, add_msg: ref argadd_msg13);
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
                    object argIndex32 = "パーツ分離";
                    string arglist = t.FeatureData(ref argIndex32);
                    uname = GeneralLib.LIndex(ref arglist, 2);
                    if (!t.IsHero())
                    {
                        if (SRC.BattleAnimation)
                        {
                            string argtsize = t.Size;
                            Effect.ExplodeAnimation(ref argtsize, t.x, t.y);
                            t.Size = argtsize;
                        }
                        else
                        {
                            string argwave_name = "Explode.wav";
                            Sound.PlayWave(ref argwave_name);
                        }
                    }

                    object argIndex33 = "パーツ分離";
                    fname = t.FeatureName(ref argIndex33);
                    bool localIsAnimationDefined4() { string argmain_situation = "破壊時分離(" + fname + ")"; string argsub_situation = ""; var ret = t.IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    bool localIsSpecialEffectDefined5() { string argmain_situation = "破壊時分離(" + t.Name + ")"; string argsub_situation = ""; var ret = t.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    bool localIsSpecialEffectDefined6() { string argmain_situation = "破壊時分離(" + fname + ")"; string argsub_situation = ""; var ret = t.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    bool localIsAnimationDefined5() { string argmain_situation = "分離(" + t.Name + ")"; string argsub_situation = ""; var ret = t.IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    bool localIsAnimationDefined6() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = t.IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    bool localIsSpecialEffectDefined7() { string argmain_situation = "分離(" + t.Name + ")"; string argsub_situation = ""; var ret = t.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    bool localIsSpecialEffectDefined8() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = t.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    string argmain_situation171 = "破壊時分離(" + t.Name + ")";
                    string argsub_situation92 = "";
                    string argmain_situation172 = "破壊時分離";
                    string argsub_situation93 = "";
                    string argmain_situation173 = "破壊時分離";
                    string argsub_situation94 = "";
                    string argmain_situation174 = "分離";
                    string argsub_situation95 = "";
                    if (t.IsAnimationDefined(ref argmain_situation171, sub_situation: ref argsub_situation92))
                    {
                        string argmain_situation159 = "破壊時分離(" + t.Name + ")";
                        string argsub_situation80 = "";
                        t.PlayAnimation(ref argmain_situation159, sub_situation: ref argsub_situation80);
                    }
                    else if (localIsAnimationDefined4())
                    {
                        string argmain_situation161 = "破壊時分離(" + fname + ")";
                        string argsub_situation82 = "";
                        t.PlayAnimation(ref argmain_situation161, sub_situation: ref argsub_situation82);
                    }
                    else if (t.IsAnimationDefined(ref argmain_situation172, sub_situation: ref argsub_situation93))
                    {
                        string argmain_situation162 = "破壊時分離";
                        string argsub_situation83 = "";
                        t.PlayAnimation(ref argmain_situation162, sub_situation: ref argsub_situation83);
                    }
                    else if (localIsSpecialEffectDefined5())
                    {
                        string argmain_situation163 = "破壊時分離(" + t.Name + ")";
                        string argsub_situation84 = "";
                        t.SpecialEffect(ref argmain_situation163, sub_situation: ref argsub_situation84);
                    }
                    else if (localIsSpecialEffectDefined6())
                    {
                        string argmain_situation164 = "破壊時分離(" + fname + ")";
                        string argsub_situation85 = "";
                        t.SpecialEffect(ref argmain_situation164, sub_situation: ref argsub_situation85);
                    }
                    else if (t.IsSpecialEffectDefined(ref argmain_situation173, sub_situation: ref argsub_situation94))
                    {
                        string argmain_situation165 = "破壊時分離";
                        string argsub_situation86 = "";
                        t.SpecialEffect(ref argmain_situation165, sub_situation: ref argsub_situation86);
                    }
                    else if (localIsAnimationDefined5())
                    {
                        string argmain_situation166 = "分離(" + t.Name + ")";
                        string argsub_situation87 = "";
                        t.PlayAnimation(ref argmain_situation166, sub_situation: ref argsub_situation87);
                    }
                    else if (localIsAnimationDefined6())
                    {
                        string argmain_situation167 = "分離(" + fname + ")";
                        string argsub_situation88 = "";
                        t.PlayAnimation(ref argmain_situation167, sub_situation: ref argsub_situation88);
                    }
                    else if (t.IsAnimationDefined(ref argmain_situation174, sub_situation: ref argsub_situation95))
                    {
                        string argmain_situation168 = "分離";
                        string argsub_situation89 = "";
                        t.PlayAnimation(ref argmain_situation168, sub_situation: ref argsub_situation89);
                    }
                    else if (localIsSpecialEffectDefined7())
                    {
                        string argmain_situation169 = "分離(" + t.Name + ")";
                        string argsub_situation90 = "";
                        t.SpecialEffect(ref argmain_situation169, sub_situation: ref argsub_situation90);
                    }
                    else if (localIsSpecialEffectDefined8())
                    {
                        string argmain_situation170 = "分離(" + fname + ")";
                        string argsub_situation91 = "";
                        t.SpecialEffect(ref argmain_situation170, sub_situation: ref argsub_situation91);
                    }
                    else
                    {
                        string argmain_situation160 = "分離";
                        string argsub_situation81 = "";
                        t.SpecialEffect(ref argmain_situation160, sub_situation: ref argsub_situation81);
                    }

                    t.Transform(ref uname);
                    {
                        var withBlock4 = t.CurrentForm();
                        withBlock4.HP = withBlock4.MaxHP;
                        // 自分から攻撃して破壊された時には行動数を0に
                        if ((withBlock4.Party ?? "") == (SRC.Stage ?? ""))
                        {
                            withBlock4.UsedAction = withBlock4.MaxAction();
                        }
                    }

                    bool localIsSysMessageDefined() { string argmain_situation = "破壊時分離(" + fname + ")"; string argsub_situation = ""; var ret = t.IsSysMessageDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    bool localIsSysMessageDefined1() { string argmain_situation = "分離(" + t.Name + ")"; string argsub_situation = ""; var ret = t.IsSysMessageDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    bool localIsSysMessageDefined2() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = t.IsSysMessageDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    string argmain_situation181 = "破壊時分離(" + t.Name + ")";
                    string argsub_situation102 = "";
                    string argmain_situation182 = "破壊時分離";
                    string argsub_situation103 = "";
                    string argmain_situation183 = "分離";
                    string argsub_situation104 = "";
                    if (t.IsSysMessageDefined(ref argmain_situation181, sub_situation: ref argsub_situation102))
                    {
                        string argmain_situation175 = "破壊時分離(" + t.Name + ")";
                        string argsub_situation96 = "";
                        string argadd_msg14 = "";
                        t.SysMessage(ref argmain_situation175, sub_situation: ref argsub_situation96, add_msg: ref argadd_msg14);
                    }
                    else if (localIsSysMessageDefined())
                    {
                        string argmain_situation176 = "破壊時分離(" + fname + ")";
                        string argsub_situation97 = "";
                        string argadd_msg15 = "";
                        t.SysMessage(ref argmain_situation176, sub_situation: ref argsub_situation97, add_msg: ref argadd_msg15);
                    }
                    else if (t.IsSysMessageDefined(ref argmain_situation182, sub_situation: ref argsub_situation103))
                    {
                        string argmain_situation177 = "破壊時分離";
                        string argsub_situation98 = "";
                        string argadd_msg16 = "";
                        t.SysMessage(ref argmain_situation177, sub_situation: ref argsub_situation98, add_msg: ref argadd_msg16);
                    }
                    else if (localIsSysMessageDefined1())
                    {
                        string argmain_situation178 = "分離(" + t.Name + ")";
                        string argsub_situation99 = "";
                        string argadd_msg17 = "";
                        t.SysMessage(ref argmain_situation178, sub_situation: ref argsub_situation99, add_msg: ref argadd_msg17);
                    }
                    else if (localIsSysMessageDefined2())
                    {
                        string argmain_situation179 = "分離(" + fname + ")";
                        string argsub_situation100 = "";
                        string argadd_msg18 = "";
                        t.SysMessage(ref argmain_situation179, sub_situation: ref argsub_situation100, add_msg: ref argadd_msg18);
                    }
                    else if (t.IsSysMessageDefined(ref argmain_situation183, sub_situation: ref argsub_situation104))
                    {
                        string argmain_situation180 = "分離";
                        string argsub_situation101 = "";
                        string argadd_msg19 = "";
                        t.SysMessage(ref argmain_situation180, sub_situation: ref argsub_situation101, add_msg: ref argadd_msg19);
                    }
                    else if (t.IsHero())
                    {
                        Unit localOtherForm2() { object argIndex1 = uname; var ret = t.OtherForm(ref argIndex1); return ret; }

                        if ((t.Nickname ?? "") != (localOtherForm2().Nickname ?? ""))
                        {
                            Unit localOtherForm1() { object argIndex1 = uname; var ret = t.OtherForm(ref argIndex1); return ret; }

                            GUI.DisplaySysMessage(t.Nickname + "は" + localOtherForm1().Nickname + "に変化した。");
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
                    Event_Renamed.SelectedTargetForEvent = t;
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
                    foreach (Pilot p in SRC.PList)
                    {
                        // 出撃中のパイロットのみが対象
                        if (p.Unit_Renamed is null)
                        {
                            goto NextPilot;
                        }

                        if (p.Unit_Renamed.Status_Renamed != "出撃")
                        {
                            goto NextPilot;
                        }

                        if ((p.Party ?? "") == (Party ?? ""))
                        {
                            // 敵を破壊したユニットの陣営のパイロットは気力+1
                            if (p.Personality != "機械")
                            {
                                p.Morale = (short)(p.Morale + 1);
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
                            string argoname8 = "破壊時味方気力変化５倍";
                            if (p.Party == "味方" & Expression.IsOptionDefined(ref argoname8))
                            {
                                p.Morale = (short)(p.Morale + 5 * morale_mod);
                            }
                            else
                            {
                                p.Morale = (short)(p.Morale + morale_mod);
                            }
                        }

                    NextPilot:
                        ;
                    }
                }

                // 脱出メッセージの表示
                bool localIsEventDefined() { string arglname = "破壊 " + t.MainPilot().ID; var ret = Event_Renamed.IsEventDefined(ref arglname, true); return ret; }

                string argmain_situation184 = "脱出";
                if (t.IsMessageDefined(ref argmain_situation184) & !is_event & !localIsEventDefined())
                {
                    string argSituation58 = "脱出";
                    string argmsg_mode60 = "";
                    t.PilotMessage(ref argSituation58, msg_mode: ref argmsg_mode60);
                }

                // 戦闘アニメ表示を使わない場合はかばったユニットを元の位置に戻しておく
                if (!SRC.BattleAnimation)
                {
                    if (su is object)
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

        Resurrect:
            ;
        // 復活した場合は破壊関連の処理を行わない

        EndAttack:
            ;
            string argattr39 = "合";
            if (Status_Renamed == "出撃" & t.Status_Renamed == "出撃" & Strings.InStr(attack_mode, "援護攻撃") == 0 & attack_mode != "マップ攻撃" & attack_mode != "反射" & !IsWeaponClassifiedAs(w, ref argattr39) & HP > 0 & t.HP > 0)
            {
                // 再攻撃
                string argref_mode4 = "ステータス";
                if (!second_attack & IsWeaponAvailable(w, ref argref_mode4) & IsTargetWithinRange(w, ref t))
                {
                    // スペシャルパワー効果「再攻撃」
                    string argsptype12 = "再攻撃";
                    if (IsUnderSpecialPowerEffect(ref argsptype12))
                    {
                        second_attack = true;
                        string argstype2 = "攻撃";
                        RemoveSpecialPowerInEffect(ref argstype2);
                        goto begin;
                    }

                    // 再攻撃能力
                    string argsname5 = "再攻撃";
                    if (MainPilot().IsSkillAvailable(ref argsname5))
                    {
                        if (this.MainPilot().Intuition >= t.MainPilot().Intuition)
                        {
                            object argIndex34 = "再攻撃";
                            string argref_mode2 = "";
                            slevel = (short)(2d * MainPilot().SkillLevel(ref argIndex34, ref_mode: ref argref_mode2));
                        }
                        else
                        {
                            object argIndex35 = "再攻撃";
                            string argref_mode3 = "";
                            slevel = (short)MainPilot().SkillLevel(ref argIndex35, ref_mode: ref argref_mode3);
                        }

                        if (slevel >= GeneralLib.Dice(32))
                        {
                            second_attack = true;
                            string argstype3 = "攻撃";
                            RemoveSpecialPowerInEffect(ref argstype3);
                            goto begin;
                        }
                    }

                    // 再属性
                    string argattr38 = "再";
                    if (WeaponLevel(w, ref argattr38) >= GeneralLib.Dice(16))
                    {
                        second_attack = true;
                        string argstype4 = "攻撃";
                        RemoveSpecialPowerInEffect(ref argstype4);
                        goto begin;
                    }
                }

                // 追加攻撃
                if (ReferenceEquals(su, t))
                {
                    string argdef_mode = "援護防御";
                    CheckAdditionalAttack(w, ref t, be_quiet, ref attack_mode, ref argdef_mode, dmg);
                }
                else
                {
                    string argdef_mode1 = "";
                    CheckAdditionalAttack(w, ref t, be_quiet, ref attack_mode, ref argdef_mode1, dmg);
                }
            }

            // サポートガードを行ったユニットは破壊処理の前に以前の位置に復帰させる
            short sx, sy;
            if (su is object)
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
                    if (withBlock6.Status_Renamed == "出撃")
                    {
                        Map.MapDataForUnit[withBlock6.x, withBlock6.y] = su;
                        Map.MapDataForUnit[tx, ty] = orig_t;
                        if (SRC.BattleAnimation)
                        {
                            string argmain_situation186 = "サポートガード終了";
                            string argsub_situation106 = "";
                            if (su.IsAnimationDefined(ref argmain_situation186, sub_situation: ref argsub_situation106))
                            {
                                if (!GUI.IsRButtonPressed())
                                {
                                    string argmain_situation185 = "サポートガード終了";
                                    string argsub_situation105 = "";
                                    su.PlayAnimation(ref argmain_situation185, sub_situation: ref argsub_situation105);
                                }
                            }
                            else if (!GUI.IsRButtonPressed())
                            {
                                GUI.PaintUnitBitmap(ref orig_t, "リフレッシュ無し");
                                if (use_support_guard)
                                {
                                    GUI.MoveUnitBitmap(ref su, sx, sy, withBlock6.x, withBlock6.y, 80, 4);
                                }
                                else
                                {
                                    GUI.MoveUnitBitmap(ref su, sx, sy, withBlock6.x, withBlock6.y, 50);
                                }
                            }
                            else
                            {
                                GUI.PaintUnitBitmap(ref su, "リフレッシュ無し");
                                GUI.PaintUnitBitmap(ref orig_t, "リフレッシュ無し");
                            }
                        }
                    }
                    else
                    {
                        // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        Map.MapDataForUnit[withBlock6.x, withBlock6.y] = null;
                        Map.MapDataForUnit[tx, ty] = orig_t;
                        GUI.PaintUnitBitmap(ref orig_t, "リフレッシュ無し");
                    }
                }
            }

            if (is_hit)
            {
                // 攻撃を命中させたことによる気力増加
                if (attack_mode != "マップ攻撃" & attack_mode != "反射")
                {
                    {
                        var withBlock7 = CurrentForm();
                        string argsname6 = "命中時気力増加";
                        if (withBlock7.MainPilot().IsSkillAvailable(ref argsname6))
                        {
                            object argIndex36 = 1;
                            object argIndex37 = 1;
                            object argIndex38 = "命中時気力増加";
                            string argref_mode5 = "";
                            withBlock7.Pilot(ref argIndex36).Morale = (short)(withBlock7.Pilot(ref argIndex37).Morale + withBlock7.MainPilot().SkillLevel(ref argIndex38, ref_mode: ref argref_mode5));
                        }
                    }
                }

                // 攻撃を受けたことによる気力増加
                t.IncreaseMorale(1);
                string argsname7 = "損傷時気力増加";
                if (t.MainPilot().IsSkillAvailable(ref argsname7))
                {
                    object argIndex39 = 1;
                    object argIndex40 = 1;
                    object argIndex41 = "損傷時気力増加";
                    string argref_mode6 = "";
                    t.Pilot(ref argIndex39).Morale = (short)(t.Pilot(ref argIndex40).Morale + t.MainPilot().SkillLevel(ref argIndex41, ref_mode: ref argref_mode6));
                }
            }
            else
            {
                // 攻撃を外したことによる気力増加
                if (attack_mode != "マップ攻撃" & attack_mode != "反射")
                {
                    {
                        var withBlock8 = CurrentForm();
                        string argsname8 = "失敗時気力増加";
                        if (withBlock8.MainPilot().IsSkillAvailable(ref argsname8))
                        {
                            object argIndex42 = 1;
                            object argIndex43 = 1;
                            object argIndex44 = "失敗時気力増加";
                            string argref_mode7 = "";
                            withBlock8.Pilot(ref argIndex42).Morale = (short)(withBlock8.Pilot(ref argIndex43).Morale + withBlock8.MainPilot().SkillLevel(ref argIndex44, ref_mode: ref argref_mode7));
                        }
                    }
                }

                // 攻撃を回避したことによる気力増加
                string argsname9 = "回避時気力増加";
                if (t.MainPilot().IsSkillAvailable(ref argsname9))
                {
                    object argIndex45 = 1;
                    object argIndex46 = 1;
                    object argIndex47 = "回避時気力増加";
                    string argref_mode8 = "";
                    t.Pilot(ref argIndex45).Morale = (short)(t.Pilot(ref argIndex46).Morale + t.MainPilot().SkillLevel(ref argIndex47, ref_mode: ref argref_mode8));
                }
            }

            // スペシャルパワー効果の解除
            if (Strings.InStr(msg, "かばった") == 0)
            {
                string argstype5 = "防御";
                t.RemoveSpecialPowerInEffect(ref argstype5);
            }

            if (is_hit)
            {
                string argstype6 = "被弾";
                t.RemoveSpecialPowerInEffect(ref argstype6);
            }

            // 戦闘アニメで変更されたユニット画像を元に戻す
            object argIndex49 = "ユニット画像";
            if (t.IsConditionSatisfied(ref argIndex49))
            {
                object argIndex48 = "ユニット画像";
                t.DeleteCondition(ref argIndex48);
                t.BitmapID = GUI.MakeUnitBitmap(ref t);
                if (t.Status_Renamed == "出撃")
                {
                    GUI.PaintUnitBitmap(ref t, "リフレッシュ無し");
                }
            }

            object argIndex51 = "非表示付加";
            if (t.IsConditionSatisfied(ref argIndex51))
            {
                object argIndex50 = "非表示付加";
                t.DeleteCondition(ref argIndex50);
                t.BitmapID = GUI.MakeUnitBitmap(ref t);
                if (t.Status_Renamed == "出撃")
                {
                    GUI.PaintUnitBitmap(ref t, "リフレッシュ無し");
                }
            }

            // 戦闘に参加したユニットを識別
            {
                var withBlock9 = CurrentForm();
                string argoname9 = "ユニット情報隠蔽";
                if (Expression.IsOptionDefined(ref argoname9))
                {
                    if (withBlock9.Party0 == "敵" | withBlock9.Party0 == "中立")
                    {
                        string argcname1 = "識別済み";
                        string argcdata1 = "非表示";
                        withBlock9.AddCondition(ref argcname1, -1, 0d, ref argcdata1);
                    }

                    if (t.Party0 == "敵" | t.Party0 == "中立")
                    {
                        string argcname2 = "識別済み";
                        string argcdata2 = "非表示";
                        t.AddCondition(ref argcname2, -1, 0d, ref argcdata2);
                    }
                }
                else
                {
                    object argIndex53 = "ユニット情報隠蔽";
                    if (withBlock9.IsConditionSatisfied(ref argIndex53))
                    {
                        object argIndex52 = "ユニット情報隠蔽";
                        withBlock9.DeleteCondition(ref argIndex52);
                    }

                    object argIndex55 = "ユニット情報隠蔽";
                    if (t.IsConditionSatisfied(ref argIndex55))
                    {
                        object argIndex54 = "ユニット情報隠蔽";
                        t.DeleteCondition(ref argIndex54);
                    }
                }
            }

            // 情報を更新
            CurrentForm().Update();
            t.Update();

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
            string argfname13 = "ステルス";
            if (IsFeatureAvailable(ref argfname13))
            {
                string argattr40 = "忍";
                if (IsWeaponClassifiedAs(w, ref argattr40))
                {
                    // 暗殺武器の場合、相手を倒すか行動不能にすればステルス継続
                    if (t.CurrentForm().Status_Renamed == "出撃" & t.CurrentForm().MaxAction() > 0)
                    {
                        string argcname3 = "ステルス無効";
                        string argcdata3 = "";
                        AddCondition(ref argcname3, 1, cdata: ref argcdata3);
                    }
                }
                else
                {
                    string argcname4 = "ステルス無効";
                    string argcdata4 = "";
                    AddCondition(ref argcname4, 1, cdata: ref argcdata4);
                }
            }

            // 合体技のパートナーの弾数＆ＥＮの消費
            var loopTo4 = (short)Information.UBound(partners);
            for (i = 1; i <= loopTo4; i++)
            {
                {
                    var withBlock10 = partners[i].CurrentForm();
                    var loopTo5 = withBlock10.CountWeapon();
                    for (j = 1; j <= loopTo5; j++)
                    {
                        if ((withBlock10.Weapon(j).Name ?? "") == (wname ?? ""))
                        {
                            withBlock10.UseWeapon(j);
                            string argattr41 = "自";
                            string argattr42 = "失";
                            string argattr43 = "変";
                            if (withBlock10.IsWeaponClassifiedAs(j, ref argattr41))
                            {
                                string argfname14 = "パーツ分離";
                                if (withBlock10.IsFeatureAvailable(ref argfname14))
                                {
                                    object argIndex56 = "パーツ分離";
                                    string arglist1 = withBlock10.FeatureData(ref argIndex56);
                                    uname = GeneralLib.LIndex(ref arglist1, 2);
                                    Unit localOtherForm3() { object argIndex1 = uname; var ret = withBlock10.OtherForm(ref argIndex1); return ret; }

                                    if (localOtherForm3().IsAbleToEnter(withBlock10.x, withBlock10.y))
                                    {
                                        withBlock10.Transform(ref uname);
                                        {
                                            var withBlock11 = withBlock10.CurrentForm();
                                            withBlock11.HP = withBlock11.MaxHP;
                                            withBlock11.UsedAction = withBlock11.MaxAction();
                                        }
                                    }
                                    else
                                    {
                                        withBlock10.Die();
                                    }
                                }
                                else
                                {
                                    withBlock10.Die();
                                }
                            }
                            else if (withBlock10.IsWeaponClassifiedAs(j, ref argattr42) & withBlock10.HP == 0)
                            {
                                withBlock10.Die();
                            }
                            else if (withBlock10.IsWeaponClassifiedAs(j, ref argattr43))
                            {
                                string argfname16 = "変形技";
                                string argfname17 = "ノーマルモード";
                                if (withBlock10.IsFeatureAvailable(ref argfname16))
                                {
                                    uname = "";
                                    var loopTo6 = withBlock10.CountFeature();
                                    for (k = 1; k <= loopTo6; k++)
                                    {
                                        string localFeature() { object argIndex1 = k; var ret = withBlock10.Feature(ref argIndex1); return ret; }

                                        string localFeatureData1() { object argIndex1 = k; var ret = withBlock10.FeatureData(ref argIndex1); return ret; }

                                        string localLIndex1() { string arglist = hs343c2cd9fcb6419f8281c931401bbbc0(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

                                        if (localFeature() == "変形技" & (localLIndex1() ?? "") == (wname ?? ""))
                                        {
                                            string localFeatureData() { object argIndex1 = k; var ret = withBlock10.FeatureData(ref argIndex1); return ret; }

                                            string arglist2 = localFeatureData();
                                            uname = GeneralLib.LIndex(ref arglist2, 2);
                                            Unit localOtherForm4() { object argIndex1 = uname; var ret = withBlock10.OtherForm(ref argIndex1); return ret; }

                                            if (localOtherForm4().IsAbleToEnter(withBlock10.x, withBlock10.y))
                                            {
                                                withBlock10.Transform(ref uname);
                                            }

                                            break;
                                        }
                                    }

                                    if ((uname ?? "") != (withBlock10.CurrentForm().Name ?? ""))
                                    {
                                        string argfname15 = "ノーマルモード";
                                        if (withBlock10.IsFeatureAvailable(ref argfname15))
                                        {
                                            object argIndex57 = "ノーマルモード";
                                            string arglist3 = withBlock10.FeatureData(ref argIndex57);
                                            uname = GeneralLib.LIndex(ref arglist3, 1);
                                            Unit localOtherForm5() { object argIndex1 = uname; var ret = withBlock10.OtherForm(ref argIndex1); return ret; }

                                            if (localOtherForm5().IsAbleToEnter(withBlock10.x, withBlock10.y))
                                            {
                                                withBlock10.Transform(ref uname);
                                            }
                                        }
                                    }
                                }
                                else if (withBlock10.IsFeatureAvailable(ref argfname17))
                                {
                                    object argIndex58 = "ノーマルモード";
                                    string arglist4 = withBlock10.FeatureData(ref argIndex58);
                                    uname = GeneralLib.LIndex(ref arglist4, 1);
                                    Unit localOtherForm6() { object argIndex1 = uname; var ret = withBlock10.OtherForm(ref argIndex1); return ret; }

                                    if (localOtherForm6().IsAbleToEnter(withBlock10.x, withBlock10.y))
                                    {
                                        withBlock10.Transform(ref uname);
                                    }
                                }
                            }

                            break;
                        }
                    }

                    // 同名の武器がなかった場合は自分のデータを使って処理
                    if (j > withBlock10.CountWeapon())
                    {
                        if (this.Weapon(w).ENConsumption > 0)
                        {
                            withBlock10.EN = withBlock10.EN - WeaponENConsumption(w);
                        }

                        string argattr44 = "消";
                        if (IsWeaponClassifiedAs(w, ref argattr44))
                        {
                            string argcname5 = "消耗";
                            string argcdata5 = "";
                            withBlock10.AddCondition(ref argcname5, 1, cdata: ref argcdata5);
                        }

                        string argattr45 = "Ｃ";
                        object argIndex60 = "チャージ完了";
                        if (IsWeaponClassifiedAs(w, ref argattr45) & withBlock10.IsConditionSatisfied(ref argIndex60))
                        {
                            object argIndex59 = "チャージ完了";
                            withBlock10.DeleteCondition(ref argIndex59);
                        }

                        string argattr47 = "気";
                        if (IsWeaponClassifiedAs(w, ref argattr47))
                        {
                            string argattr46 = "気";
                            withBlock10.IncreaseMorale((short)(-5 * WeaponLevel(w, ref argattr46)));
                        }

                        string argattr50 = "霊";
                        string argattr51 = "プ";
                        if (IsWeaponClassifiedAs(w, ref argattr50))
                        {
                            hp_ratio = 100 * withBlock10.HP / (double)withBlock10.MaxHP;
                            en_ratio = 100 * withBlock10.EN / (double)withBlock10.MaxEN;
                            string argattr48 = "霊";
                            withBlock10.MainPilot().Plana = (int)(withBlock10.MainPilot().Plana - 5d * WeaponLevel(w, ref argattr48));
                            withBlock10.HP = (int)(withBlock10.MaxHP * hp_ratio / 100d);
                            withBlock10.EN = (int)(withBlock10.MaxEN * en_ratio / 100d);
                        }
                        else if (IsWeaponClassifiedAs(w, ref argattr51))
                        {
                            hp_ratio = 100 * withBlock10.HP / (double)withBlock10.MaxHP;
                            en_ratio = 100 * withBlock10.EN / (double)withBlock10.MaxEN;
                            string argattr49 = "プ";
                            withBlock10.MainPilot().Plana = (int)(withBlock10.MainPilot().Plana - 5d * WeaponLevel(w, ref argattr49));
                            withBlock10.HP = (int)(withBlock10.MaxHP * hp_ratio / 100d);
                            withBlock10.EN = (int)(withBlock10.MaxEN * en_ratio / 100d);
                        }

                        string argattr53 = "失";
                        if (IsWeaponClassifiedAs(w, ref argattr53))
                        {
                            string argattr52 = "失";
                            withBlock10.HP = GeneralLib.MaxLng((int)(withBlock10.HP - (long)(withBlock10.MaxHP * WeaponLevel(w, ref argattr52)) / 10L), 0);
                        }

                        string argattr54 = "自";
                        string argattr55 = "失";
                        string argattr56 = "変";
                        if (IsWeaponClassifiedAs(w, ref argattr54))
                        {
                            string argfname18 = "パーツ分離";
                            if (withBlock10.IsFeatureAvailable(ref argfname18))
                            {
                                object argIndex61 = "パーツ分離";
                                string arglist5 = withBlock10.FeatureData(ref argIndex61);
                                uname = GeneralLib.LIndex(ref arglist5, 2);
                                Unit localOtherForm7() { object argIndex1 = uname; var ret = withBlock10.OtherForm(ref argIndex1); return ret; }

                                if (localOtherForm7().IsAbleToEnter(withBlock10.x, withBlock10.y))
                                {
                                    withBlock10.Transform(ref uname);
                                    {
                                        var withBlock12 = withBlock10.CurrentForm();
                                        withBlock12.HP = withBlock12.MaxHP;
                                        withBlock12.UsedAction = withBlock12.MaxAction();
                                    }
                                }
                                else
                                {
                                    withBlock10.Die();
                                }
                            }
                            else
                            {
                                withBlock10.Die();
                            }
                        }
                        else if (IsWeaponClassifiedAs(w, ref argattr55) & withBlock10.HP == 0)
                        {
                            withBlock10.Die();
                        }
                        else if (IsWeaponClassifiedAs(w, ref argattr56))
                        {
                            string argfname19 = "ノーマルモード";
                            if (withBlock10.IsFeatureAvailable(ref argfname19))
                            {
                                object argIndex62 = "ノーマルモード";
                                string arglist6 = withBlock10.FeatureData(ref argIndex62);
                                uname = GeneralLib.LIndex(ref arglist6, 1);
                                Unit localOtherForm8() { object argIndex1 = uname; var ret = withBlock10.OtherForm(ref argIndex1); return ret; }

                                if (localOtherForm8().IsAbleToEnter(withBlock10.x, withBlock10.y))
                                {
                                    withBlock10.Transform(ref uname);
                                }
                            }
                        }
                    }
                }
            }

            // 以下の特殊効果はは武器データの変化があるため、同時には適応されない

            // 反射等により破壊された場合はなにも出来ない

            // 自爆攻撃
            string argattr57 = "自";

            // ＨＰ消費攻撃による自殺
            string argattr58 = "失";

            // 変形技
            string argattr59 = "変";

            // 能力コピー
            string argattr60 = "写";
            string argattr61 = "化";
            string argattr62 = "殺";
            if (CurrentForm().Status_Renamed == "破壊")
            {
            }
            else if (IsWeaponClassifiedAs(w, ref argattr57))
            {
                string argfname20 = "パーツ分離";
                if (IsFeatureAvailable(ref argfname20))
                {
                    object argIndex63 = "パーツ分離";
                    string arglist7 = FeatureData(ref argIndex63);
                    uname = GeneralLib.LIndex(ref arglist7, 2);
                    Unit localOtherForm9() { object argIndex1 = uname; var ret = OtherForm(ref argIndex1); return ret; }

                    if (localOtherForm9().IsAbleToEnter(x, y))
                    {
                        Transform(ref uname);
                        {
                            var withBlock13 = CurrentForm();
                            withBlock13.HP = withBlock13.MaxHP;
                            withBlock13.UsedAction = withBlock13.MaxAction();
                        }

                        object argIndex64 = "パーツ分離";
                        fname = FeatureName(ref argIndex64);
                        bool localIsSysMessageDefined3() { string argmain_situation = "破壊時分離(" + fname + ")"; string argsub_situation = ""; var ret = IsSysMessageDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                        bool localIsSysMessageDefined4() { string argmain_situation = "分離(" + Name + ")"; string argsub_situation = ""; var ret = IsSysMessageDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                        bool localIsSysMessageDefined5() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = IsSysMessageDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                        string argmain_situation193 = "破壊時分離(" + Name + ")";
                        string argsub_situation113 = "";
                        string argmain_situation194 = "破壊時分離";
                        string argsub_situation114 = "";
                        string argmain_situation195 = "分離";
                        string argsub_situation115 = "";
                        if (IsSysMessageDefined(ref argmain_situation193, sub_situation: ref argsub_situation113))
                        {
                            string argmain_situation187 = "破壊時分離(" + Name + ")";
                            string argsub_situation107 = "";
                            string argadd_msg20 = "";
                            SysMessage(ref argmain_situation187, sub_situation: ref argsub_situation107, add_msg: ref argadd_msg20);
                        }
                        else if (localIsSysMessageDefined3())
                        {
                            string argmain_situation188 = "破壊時分離(" + fname + ")";
                            string argsub_situation108 = "";
                            string argadd_msg21 = "";
                            SysMessage(ref argmain_situation188, sub_situation: ref argsub_situation108, add_msg: ref argadd_msg21);
                        }
                        else if (IsSysMessageDefined(ref argmain_situation194, sub_situation: ref argsub_situation114))
                        {
                            string argmain_situation189 = "破壊時分離";
                            string argsub_situation109 = "";
                            string argadd_msg22 = "";
                            SysMessage(ref argmain_situation189, sub_situation: ref argsub_situation109, add_msg: ref argadd_msg22);
                        }
                        else if (localIsSysMessageDefined4())
                        {
                            string argmain_situation190 = "分離(" + Name + ")";
                            string argsub_situation110 = "";
                            string argadd_msg23 = "";
                            SysMessage(ref argmain_situation190, sub_situation: ref argsub_situation110, add_msg: ref argadd_msg23);
                        }
                        else if (localIsSysMessageDefined5())
                        {
                            string argmain_situation191 = "分離(" + fname + ")";
                            string argsub_situation111 = "";
                            string argadd_msg24 = "";
                            SysMessage(ref argmain_situation191, sub_situation: ref argsub_situation111, add_msg: ref argadd_msg24);
                        }
                        else if (IsSysMessageDefined(ref argmain_situation195, sub_situation: ref argsub_situation115))
                        {
                            string argmain_situation192 = "分離";
                            string argsub_situation112 = "";
                            string argadd_msg25 = "";
                            SysMessage(ref argmain_situation192, sub_situation: ref argsub_situation112, add_msg: ref argadd_msg25);
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
            else if (IsWeaponClassifiedAs(w, ref argattr58) & HP == 0)
            {
                Die();
            }
            else if (IsWeaponClassifiedAs(w, ref argattr59))
            {
                string argfname22 = "変形技";
                string argfname23 = "ノーマルモード";
                if (IsFeatureAvailable(ref argfname22))
                {
                    var loopTo7 = CountFeature();
                    for (i = 1; i <= loopTo7; i++)
                    {
                        string localFeature1() { object argIndex1 = i; var ret = Feature(ref argIndex1); return ret; }

                        string localFeatureData3() { object argIndex1 = i; var ret = FeatureData(ref argIndex1); return ret; }

                        string localLIndex2() { string arglist = hsd78be329e21a495e9d12e8227830c1b1(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

                        if (localFeature1() == "変形技" & (localLIndex2() ?? "") == (wname ?? ""))
                        {
                            string localFeatureData2() { object argIndex1 = i; var ret = FeatureData(ref argIndex1); return ret; }

                            string arglist8 = localFeatureData2();
                            uname = GeneralLib.LIndex(ref arglist8, 2);
                            Unit localOtherForm10() { object argIndex1 = uname; var ret = OtherForm(ref argIndex1); return ret; }

                            if (localOtherForm10().IsAbleToEnter(x, y))
                            {
                                Transform(ref uname);
                            }

                            break;
                        }
                    }

                    if ((uname ?? "") != (CurrentForm().Name ?? ""))
                    {
                        string argfname21 = "ノーマルモード";
                        if (IsFeatureAvailable(ref argfname21))
                        {
                            object argIndex65 = "ノーマルモード";
                            string arglist9 = FeatureData(ref argIndex65);
                            uname = GeneralLib.LIndex(ref arglist9, 1);
                            Unit localOtherForm11() { object argIndex1 = uname; var ret = OtherForm(ref argIndex1); return ret; }

                            if (localOtherForm11().IsAbleToEnter(x, y))
                            {
                                Transform(ref uname);
                            }
                        }
                    }
                }
                else if (IsFeatureAvailable(ref argfname23))
                {
                    object argIndex66 = "ノーマルモード";
                    string arglist10 = FeatureData(ref argIndex66);
                    uname = GeneralLib.LIndex(ref arglist10, 1);
                    Unit localOtherForm12() { object argIndex1 = uname; var ret = OtherForm(ref argIndex1); return ret; }

                    if (localOtherForm12().IsAbleToEnter(x, y))
                    {
                        Transform(ref uname);
                    }
                }
            }

            // アイテムを消費
            else if (Weapon(w).IsItem() & Bullet(w) == 0 & MaxBullet(w) > 0)
            {
                // アイテムを削除
                num = Data.CountWeapon();
                num = (short)(num + MainPilot().Data.CountWeapon());
                var loopTo8 = CountPilot();
                for (i = 2; i <= loopTo8; i++)
                {
                    Pilot localPilot2() { object argIndex1 = i; var ret = Pilot(ref argIndex1); return ret; }

                    num = (short)(num + localPilot2().Data.CountWeapon());
                }

                var loopTo9 = CountSupport();
                for (i = 2; i <= loopTo9; i++)
                {
                    Pilot localSupport2() { object argIndex1 = i; var ret = Support(ref argIndex1); return ret; }

                    num = (short)(num + localSupport2().Data.CountWeapon());
                }

                string argfname24 = "追加サポート";
                if (IsFeatureAvailable(ref argfname24))
                {
                    num = (short)(num + AdditionalSupport().Data.CountWeapon());
                }

                foreach (Item itm in colItem)
                {
                    num = (short)(num + itm.CountWeapon());
                    if (w <= num)
                    {
                        itm.Exist = false;
                        DeleteItem(ref (object)itm.ID);
                        break;
                    }
                }
            }
            else if (is_hit & (IsWeaponClassifiedAs(w, ref argattr60) | IsWeaponClassifiedAs(w, ref argattr61)) & (dmg > 0 | !IsWeaponClassifiedAs(w, ref argattr62)))
            {
                CheckMetamorphAttack(w, ref t, ref def_mode);
            }

            {
                var withBlock14 = CurrentForm();
                // スペシャルパワーの効果を削除
                if (Strings.InStr(attack_mode, "援護攻撃") == 0)
                {
                    string argsptype13 = "攻撃後消耗";
                    if (withBlock14.IsUnderSpecialPowerEffect(ref argsptype13))
                    {
                        string argcname6 = "消耗";
                        string argcdata6 = "";
                        withBlock14.AddCondition(ref argcname6, 1, cdata: ref argcdata6);
                    }

                    string argstype7 = "攻撃";
                    withBlock14.RemoveSpecialPowerInEffect(ref argstype7);
                    if (is_hit)
                    {
                        string argstype8 = "命中";
                        withBlock14.RemoveSpecialPowerInEffect(ref argstype8);
                    }
                }

                // 戦闘アニメで変更されたユニット画像を元に戻す
                object argIndex68 = "ユニット画像";
                if (withBlock14.IsConditionSatisfied(ref argIndex68))
                {
                    object argIndex67 = "ユニット画像";
                    withBlock14.DeleteCondition(ref argIndex67);
                    withBlock14.BitmapID = GUI.MakeUnitBitmap(ref CurrentForm());
                    GUI.PaintUnitBitmap(ref CurrentForm());
                }

                object argIndex70 = "非表示付加";
                if (withBlock14.IsConditionSatisfied(ref argIndex70))
                {
                    object argIndex69 = "非表示付加";
                    withBlock14.DeleteCondition(ref argIndex69);
                    withBlock14.BitmapID = GUI.MakeUnitBitmap(ref CurrentForm());
                    GUI.PaintUnitBitmap(ref CurrentForm());
                }

                var loopTo10 = (short)Information.UBound(partners);
                for (i = 1; i <= loopTo10; i++)
                {
                    {
                        var withBlock15 = partners[i].CurrentForm();
                        object argIndex72 = "ユニット画像";
                        if (withBlock15.IsConditionSatisfied(ref argIndex72))
                        {
                            object argIndex71 = "ユニット画像";
                            withBlock15.DeleteCondition(ref argIndex71);
                            withBlock15.BitmapID = GUI.MakeUnitBitmap(ref partners[i].CurrentForm());
                            GUI.PaintUnitBitmap(ref partners[i].CurrentForm());
                        }

                        object argIndex74 = "非表示付加";
                        if (withBlock15.IsConditionSatisfied(ref argIndex74))
                        {
                            object argIndex73 = "非表示付加";
                            withBlock15.DeleteCondition(ref argIndex73);
                            withBlock15.BitmapID = GUI.MakeUnitBitmap(ref partners[i].CurrentForm());
                            GUI.PaintUnitBitmap(ref partners[i].CurrentForm());
                        }
                    }
                }
            }

            // カットインは消去しておく
            string argoname10 = "戦闘中画面初期化無効";
            if (!Expression.IsOptionDefined(ref argoname10) | attack_mode == "マップ攻撃")
            {
                if (GUI.IsPictureVisible)
                {
                    GUI.ClearPicture();
                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    GUI.MainForm.picMain(0).Refresh();
                }
            }

            // ADD START MARGE
            // 戦闘アニメ終了処理
            string argmain_situation198 = wname + "(終了)";
            string argsub_situation118 = "";
            string argmain_situation199 = "終了";
            string argsub_situation119 = "";
            if (IsAnimationDefined(ref argmain_situation198, sub_situation: ref argsub_situation118))
            {
                string argmain_situation196 = wname + "(終了)";
                string argsub_situation116 = "";
                PlayAnimation(ref argmain_situation196, sub_situation: ref argsub_situation116);
            }
            else if (IsAnimationDefined(ref argmain_situation199, sub_situation: ref argsub_situation119))
            {
                string argmain_situation197 = "終了";
                string argsub_situation117 = "";
                PlayAnimation(ref argmain_situation197, sub_situation: ref argsub_situation117);
            }
            // ADD END MARGE

            // ユニット選択を解除
            Commands.RestoreSelections();
        }
    }
}
