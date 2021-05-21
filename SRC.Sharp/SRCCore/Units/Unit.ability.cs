// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using System;

namespace SRCCore.Units
{
    // === アビリティ関連処理 ===
    public partial class Unit
    {
        // アビリティ
        public UnitAbility Ability(int a)
        {
            return Abilities[a];
        }

        // アビリティ総数
        public int CountAbility()
        {
            return Abilities.Count;
        }


        // === アビリティ発動関連処理 ===

        // XXX 攻撃同様にしたけれど、UnitXxxを用意するならそっちが持ってるべきだったかもしれない。

        // アビリティを使用
        public bool ExecuteAbility(UnitAbility a, Unit t, bool is_map_ability = false, bool is_event = false)
        {
            return false;
            //bool ExecuteAbilityRet = false;
            //var partners = default(Unit[]);
            //int num, j, i, k, w = default;
            //string aclass, aname, anickname, atype = default;
            //string edata;
            //double elevel, elevel2;
            //double elv_mod, elv_mod2;
            //int epower;
            //int prev_value;
            //bool is_useful = default, flag;
            //Unit u;
            //Pilot p;
            //string buf, msg;
            //string uname = default, pname, fname;
            //string ftype, fdata;
            //double flevel;
            //string ftype2, fdata2;
            //double flevel2;
            //var is_anime_played = default(bool);
            //double hp_ratio, en_ratio;
            //int tx, ty;
            //int tx2, ty2;
            //string cname;
            //    var aname = a.Data.Name;
            //    var anickname = a.AbilityNickname();
            //    var aclass = a.Data.Class;

            //    // 現在の選択状況をセーブ
            //    Commands.SaveSelections();

            //    // 選択内容を切り替え
            //    Commands.SelectedUnit = this;
            //    Event.SelectedUnitForEvent = this;
            //    Commands.SelectedTarget = t;
            //    Event.SelectedTargetForEvent = t;
            //    Commands.SelectedAbility = a.AbilityNo();
            //    Commands.SelectedAbilityName = aname;
            //    if (!is_map_ability)
            //    {
            //        // 通常アビリティの場合
            //        if (SRC.BattleAnimation)
            //        {
            //            GUI.RedrawScreen();
            //        }

            //        if (a.IsAbilityClassifiedAs("合"))
            //        {
            //            // 射程が0の場合はマスクをクリアしておく
            //            if (a.AbilityMaxRange() == 0)
            //            {
            //                Map.ClearMask();
            //                Map.MaskData[x, y] = false;
            //            }

            //            // 合体技の場合にパートナーをハイライト表示
            //            if (a.AbilityMaxRange() == 1)
            //            {
            //                CombinationPartner("アビリティ", a, partners, t.x, t.y);
            //            }
            //            else
            //            {
            //                CombinationPartner("アビリティ", a, partners);
            //            }

            //            var loopTo2 = Information.UBound(partners);
            //            for (i = 1; i <= loopTo2; i++)
            //            {
            //                {
            //                    var withBlock = partners[i];
            //                    Map.MaskData[withBlock.x, withBlock.y] = false;
            //                }
            //            }

            //            if (!SRC.BattleAnimation)
            //            {
            //                GUI.MaskScreen();
            //            }
            //        }
            //        else
            //        {
            //            partners = new Unit[1];
            //            Commands.SelectedPartners.Clear();
            //        }

            //        // ダイアログ用にあらかじめ追加パイロットを作成しておく
            //        var loopTo3 = a.Ability().CountEffect();
            //        for (i = 1; i <= loopTo3; i++)
            //        {
            //            edata = a.Ability().EffectData(i);
            //            switch (a.Ability().EffectType(i) ?? "")
            //            {
            //                case "変身":
            //                    {
            //                        bool localIsDefined() { object argIndex1 = GeneralLib.LIndex(edata, 1); var ret = SRC.UDList.IsDefined(argIndex1); return ret; }

            //                        if (!localIsDefined())
            //                        {
            //                            GUI.ErrorMessage(GeneralLib.LIndex(edata, 1) + "のデータが定義されていません");
            //                            return ExecuteAbilityRet;
            //                        }

            //                        {
            //                            var withBlock1 = SRC.UDList.Item(GeneralLib.LIndex(edata, 1));
            //                            if (withBlock1.IsFeatureAvailable("追加パイロット"))
            //                            {
            //                                bool localIsDefined1() { object argIndex1 = "追加パイロット"; object argIndex2 = withBlock1.FeatureData(argIndex1); var ret = SRC.PList.IsDefined(argIndex2); return ret; }

            //                                if (!localIsDefined1())
            //                                {
            //                                    SRC.PList.Add(withBlock1.FeatureData(argIndex2), MainPilot().Level, Party0, gid: "");
            //                                    this.Party0 = argpparty;
            //                                }
            //                            }
            //                        }

            //                        break;
            //                    }
            //            }
            //        }

            //        // アビリティ使用時のメッセージ＆特殊効果
            //        if (IsAnimationDefined(aname + "(準備)", sub_situation: ""))
            //        {
            //            PlayAnimation(aname + "(準備)", sub_situation: "");
            //        }

            //        if (IsMessageDefined("かけ声(" + aname + ")"))
            //        {
            //            if (!My.MyProject.Forms.frmMessage.Visible)
            //            {
            //                if (ReferenceEquals(Commands.SelectedTarget, this))
            //                {
            //                    GUI.OpenMessageForm(this, u2: null);
            //                }
            //                else
            //                {
            //                    GUI.OpenMessageForm(Commands.SelectedTarget, this);
            //                }
            //            }

            //            PilotMessage("かけ声(" + aname + ")", msg_mode: "");
            //        }

            //        if (IsMessageDefined(aname) | IsMessageDefined("アビリティ"))
            //        {
            //            if (!My.MyProject.Forms.frmMessage.Visible)
            //            {
            //                if (ReferenceEquals(Commands.SelectedTarget, this))
            //                {
            //                    GUI.OpenMessageForm(this, u2: null);
            //                }
            //                else
            //                {
            //                    GUI.OpenMessageForm(Commands.SelectedTarget, this);
            //                }
            //            }

            //            PilotMessage(aname, "アビリティ");
            //        }

            //        if (IsAnimationDefined(aname + "(使用)", sub_situation: ""))
            //        {
            //            PlayAnimation(aname + "(使用)", "", true);
            //        }

            //        if (IsAnimationDefined(aname + "(発動)", sub_situation: "") | IsAnimationDefined(aname, sub_situation: ""))
            //        {
            //            PlayAnimation(aname + "(発動)", "", true);
            //            is_anime_played = true;
            //        }
            //        else
            //        {
            //            SpecialEffect(aname, "", true);
            //        }

            //        // アビリティの種類は？
            //        var loopTo4 = a.Ability().CountEffect();
            //        for (i = 1; i <= loopTo4; i++)
            //        {
            //            switch (a.Ability().EffectType(i) ?? "")
            //            {
            //                case "召喚":
            //                    {
            //                        aname = "";
            //                        break;
            //                    }

            //                case "再行動":
            //                    {
            //                        if (this.a.Ability().MaxRange > 0)
            //                        {
            //                            atype = a.Ability().EffectType(i);
            //                        }

            //                        break;
            //                    }

            //                case "解説":
            //                    {
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        atype = a.Ability().EffectType(i);
            //                        break;
            //                    }
            //            }
            //        }

            //        switch (Information.UBound(partners))
            //        {
            //            case 0:
            //                {
            //                    // 通常
            //                    msg = Nickname + "は";
            //                    break;
            //                }

            //            case 1:
            //                {
            //                    // ２体合体
            //                    if ((Nickname ?? "") != (partners[0].Nickname ?? ""))
            //                    {
            //                        msg = Nickname + "は[" + partners[0].Nickname + "]と共に";
            //                    }
            //                    else if ((MainPilot().get_Nickname(false) ?? "") != (partners[0].MainPilot().get_Nickname(false) ?? ""))
            //                    {
            //                        msg = MainPilot().get_Nickname(false) + "と[" + partners[0].MainPilot().get_Nickname(false) + "]の[" + Nickname + "]は";
            //                    }
            //                    else
            //                    {
            //                        msg = Nickname + "達は";
            //                    }

            //                    break;
            //                }

            //            case 2:
            //                {
            //                    // ３体合体
            //                    if ((Nickname ?? "") != (partners[0].Nickname ?? ""))
            //                    {
            //                        msg = Nickname + "は[" + partners[0].Nickname + "]、[" + partners[1].Nickname + "]と共に";
            //                    }
            //                    else if ((MainPilot().get_Nickname(false) ?? "") != (partners[0].MainPilot().get_Nickname(false) ?? ""))
            //                    {
            //                        msg = MainPilot().get_Nickname(false) + "、[" + partners[0].MainPilot().get_Nickname(false) + "]、[" + partners[1].MainPilot().get_Nickname(false) + "]の[" + Nickname + "]は";
            //                    }
            //                    else
            //                    {
            //                        msg = Nickname + "達は";
            //                    }

            //                    break;
            //                }

            //            default:
            //                {
            //                    // ３体以上
            //                    msg = Nickname + "達は";
            //                    break;
            //                }
            //        }

            //        if (a.IsSpellAbility())
            //        {
            //            if (t is object & this.a.Ability().MaxRange != 0)
            //            {
            //                if (ReferenceEquals(this, t))
            //                {
            //                    msg = msg + "自分に";
            //                }
            //                else
            //                {
            //                    msg = msg + "[" + t.Nickname + "]に";
            //                }
            //            }

            //            if (Strings.Right(anickname, 2) == "呪文")
            //            {
            //                msg = msg + "[" + anickname + "]を唱えた。";
            //            }
            //            else if (Strings.Right(anickname, 2) == "の杖")
            //            {
            //                msg = msg + "[" + Strings.Left(anickname, Strings.Len(anickname) - 2) + "]の呪文を唱えた。";
            //            }
            //            else
            //            {
            //                msg = msg + "[" + anickname + "]の呪文を唱えた。";
            //            }
            //        }
            //        else if (Strings.Right(anickname, 1) == "歌")
            //        {
            //            msg = msg + "[" + anickname + "]を歌った。";
            //        }
            //        else if (Strings.Right(anickname, 2) == "踊り")
            //        {
            //            msg = msg + "[" + anickname + "]を踊った。";
            //        }
            //        else
            //        {
            //            if (t is object & this.a.Ability().MaxRange != 0)
            //            {
            //                if (ReferenceEquals(this, t))
            //                {
            //                    msg = msg + "自分に";
            //                }
            //                else
            //                {
            //                    msg = msg + "[" + t.Nickname + "]に";
            //                }
            //            }

            //            msg = msg + "[" + anickname + "]を使った。";
            //        }

            //        if (IsSysMessageDefined(aname, sub_situation: ""))
            //        {
            //            // 「アビリティ名(解説)」のメッセージを使用
            //            if (!My.MyProject.Forms.frmMessage.Visible)
            //            {
            //                if (ReferenceEquals(Commands.SelectedTarget, this))
            //                {
            //                    GUI.OpenMessageForm(this, u2: null);
            //                }
            //                else
            //                {
            //                    GUI.OpenMessageForm(Commands.SelectedTarget, this);
            //                }
            //            }

            //            SysMessage(aname, sub_situation: "", add_msg: "");
            //        }
            //        else if (IsSysMessageDefined("アビリティ", sub_situation: ""))
            //        {
            //            // 「アビリティ(解説)」のメッセージを使用
            //            if (!My.MyProject.Forms.frmMessage.Visible)
            //            {
            //                if (ReferenceEquals(Commands.SelectedTarget, this))
            //                {
            //                    GUI.OpenMessageForm(this, u2: null);
            //                }
            //                else
            //                {
            //                    GUI.OpenMessageForm(Commands.SelectedTarget, this);
            //                }
            //            }

            //            SysMessage("アビリティ", sub_situation: "", add_msg: "");
            //        }
            //        else if (atype == "変身" & this.a.Ability().MaxRange == 0)
            //        {
            //        }
            //        // 変身の場合はメッセージなし
            //        else if (!string.IsNullOrEmpty(atype))
            //        {
            //            if (!My.MyProject.Forms.frmMessage.Visible)
            //            {
            //                if (ReferenceEquals(Commands.SelectedTarget, this))
            //                {
            //                    GUI.OpenMessageForm(this, u2: null);
            //                }
            //                else
            //                {
            //                    GUI.OpenMessageForm(Commands.SelectedTarget, this);
            //                }
            //            }

            //            GUI.DisplaySysMessage(msg);
            //        }

            //        // ＥＮ消費＆使用回数減少
            //        UseAbility(a);

            //        // アビリティの使用に失敗？
            //        if (GeneralLib.Dice(10) <= a.AbilityLevel("難"))
            //        {
            //            GUI.DisplaySysMessage("しかし何もおきなかった…");
            //            goto Finish;
            //        }
            //    }
            //    else
            //    {
            //        // マップアビリティの場合
            //        if (IsAnimationDefined(aname + "(発動)", sub_situation: "") | IsAnimationDefined(aname, sub_situation: ""))
            //        {
            //            PlayAnimation(aname + "(発動)", sub_situation: "");
            //            is_anime_played = true;
            //        }
            //    }

            //    // 相手がアビリティの属性に対して無効化属性を持っているならアビリティは
            //    // 効果なし
            //    if (!ReferenceEquals(this, t))
            //    {
            //        if (t.Immune(aclass))
            //        {
            //            goto Finish;
            //        }
            //    }

            //    // 気力低下アビリティ
            //    if (a.IsAbilityClassifiedAs("脱"))
            //    {
            //        t.IncreaseMorale(-10);
            //    }

            //    // 特殊効果除去アビリティ
            //    if (a.IsAbilityClassifiedAs("除"))
            //    {
            //        i = 1;
            //        while (i <= t.CountCondition())
            //        {
            //            string localCondition() { object argIndex1 = i; var ret = t.Condition(argIndex1); return ret; }

            //            string localCondition1() { object argIndex1 = i; var ret = t.Condition(argIndex1); return ret; }

            //            string localCondition2() { object argIndex1 = i; var ret = t.Condition(argIndex1); return ret; }

            //            string localCondition3() { object argIndex1 = i; var ret = t.Condition(argIndex1); return ret; }

            //            int localConditionLifetime() { object argIndex1 = i; var ret = t.ConditionLifetime(argIndex1); return ret; }

            //            if ((Strings.InStr(localCondition(), "付加") > 0 | Strings.InStr(localCondition1(), "強化") > 0 | Strings.InStr(localCondition2(), "ＵＰ") > 0) & localCondition3() != "ノーマルモード付加" & localConditionLifetime() != 0)
            //            {
            //                t.DeleteCondition(i);
            //            }
            //            else
            //            {
            //                i = (i + 1);
            //            }
            //        }
            //    }

            //    // 得意技・不得手によるアビリティ効果への修正値を計算
            //    elv_mod = 1d;
            //    elv_mod2 = 1d;
            //    {
            //        var withBlock2 = MainPilot();
            //        // 得意技
            //        if (withBlock2.IsSkillAvailable("得意技"))
            //        {
            //            buf = withBlock2.SkillData("得意技");
            //            var loopTo5 = Strings.Len(buf);
            //            for (i = 1; i <= loopTo5; i++)
            //            {
            //                if (Strings.InStr(aclass, GeneralLib.GetClassBundle(buf, i)) > 0)
            //                {
            //                    elv_mod = 1.2d * elv_mod;
            //                    elv_mod2 = 1.4d * elv_mod2;
            //                    break;
            //                }
            //            }
            //        }

            //        // 不得手
            //        if (withBlock2.IsSkillAvailable("不得手"))
            //        {
            //            buf = withBlock2.SkillData("不得手");
            //            var loopTo6 = Strings.Len(buf);
            //            for (i = 1; i <= loopTo6; i++)
            //            {
            //                if (Strings.InStr(aclass, GeneralLib.GetClassBundle(buf, i)) > 0)
            //                {
            //                    elv_mod = 0.8d * elv_mod;
            //                    elv_mod2 = 0.6d * elv_mod2;
            //                    break;
            //                }
            //            }
            //        }
            //    }

            //    // アビリティの効果を適用
            //    var loopTo7 = a.Ability().CountEffect();
            //    for (i = 1; i <= loopTo7; i++)
            //    {
            //        {
            //            var withBlock3 = a.Ability();
            //            edata = withBlock3.EffectData(i);
            //            elevel = withBlock3.EffectLevel(i) * elv_mod;
            //            elevel2 = withBlock3.EffectLevel(i) * elv_mod2;
            //        }

            //        switch (a.Ability().EffectType(i) ?? "")
            //        {
            //            case "回復":
            //                {
            //                    {
            //                        var withBlock4 = t;
            //                        if (elevel > 0d)
            //                        {
            //                            // ＨＰは既に最大値？
            //                            if (withBlock4.HP == withBlock4.MaxHP)
            //                            {
            //                                goto NextLoop;
            //                            }

            //                            // ゾンビ？
            //                            if (withBlock4.IsConditionSatisfied("ゾンビ"))
            //                            {
            //                                goto NextLoop;
            //                            }

            //                            if (!is_anime_played)
            //                            {
            //                                if (a.IsSpellAbility() | a.IsAbilityClassifiedAs("魔"))
            //                                {
            //                                    Effect.ShowAnimation("回復魔法発動");
            //                                }
            //                                else
            //                                {
            //                                    Effect.ShowAnimation("修理装置発動");
            //                                }
            //                            }

            //                            prev_value = withBlock4.HP;
            //                            {
            //                                var withBlock5 = MainPilot();
            //                                if (a.IsSpellAbility())
            //                                {
            //                                    epower = (5d * elevel * withBlock5.Shooting);
            //                                }
            //                                else
            //                                {
            //                                    epower = (500d * elevel);
            //                                }

            //                                epower = ((long)(epower * (10d + withBlock5.SkillLevel("修理", ref_mode: ""))) / 10L);
            //                            }

            //                            t.HP = t.HP + epower;
            //                            GUI.DrawSysString(withBlock4.x, withBlock4.y, "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.HP - prev_value));
            //                            if (ReferenceEquals(t, this))
            //                            {
            //                                GUI.UpdateMessageForm(this, u2: null);
            //                            }
            //                            else
            //                            {
            //                                GUI.UpdateMessageForm(t, this);
            //                            }

            //                            GUI.DisplaySysMessage(withBlock4.Nickname + "の" + Expression.Term("ＨＰ", t) + "が[" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.HP - prev_value) + "]回復した;" + "残り" + Expression.Term("ＨＰ", t) + "は" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.HP) + "（損傷率 = " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(100 * (withBlock4.MaxHP - withBlock4.HP) / withBlock4.MaxHP) + "％）");
            //                            is_useful = true;
            //                        }
            //                        else if (elevel < 0d)
            //                        {
            //                            prev_value = withBlock4.HP;
            //                            {
            //                                var withBlock6 = MainPilot();
            //                                if (a.IsSpellAbility())
            //                                {
            //                                    epower = (5d * elevel * withBlock6.Shooting);
            //                                }
            //                                else
            //                                {
            //                                    epower = (500d * elevel);
            //                                }
            //                            }

            //                            t.HP = t.HP + epower;
            //                            GUI.DrawSysString(withBlock4.x, withBlock4.y, "-" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock4.HP));
            //                            if (ReferenceEquals(t, this))
            //                            {
            //                                GUI.UpdateMessageForm(this, u2: null);
            //                            }
            //                            else
            //                            {
            //                                GUI.UpdateMessageForm(t, this);
            //                            }

            //                            GUI.DisplaySysMessage(withBlock4.Nickname + "の" + Expression.Term("ＨＰ", t) + "が[" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock4.HP) + "]減少した;" + "残り" + Expression.Term("ＨＰ", t) + "は" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.HP) + "（損傷率 = " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(100 * (withBlock4.MaxHP - withBlock4.HP) / withBlock4.MaxHP) + "％）");
            //                        }
            //                    }

            //                    break;
            //                }

            //            case "補給":
            //                {
            //                    {
            //                        var withBlock7 = t;
            //                        if (elevel > 0d)
            //                        {
            //                            // ＥＮは既に最大値？
            //                            if (withBlock7.EN == withBlock7.MaxEN)
            //                            {
            //                                goto NextLoop;
            //                            }

            //                            // ゾンビ？
            //                            if (withBlock7.IsConditionSatisfied("ゾンビ"))
            //                            {
            //                                goto NextLoop;
            //                            }

            //                            if (!is_anime_played)
            //                            {
            //                                if (a.IsSpellAbility() | a.IsAbilityClassifiedAs("魔"))
            //                                {
            //                                    Effect.ShowAnimation("回復魔法発動");
            //                                }
            //                                else
            //                                {
            //                                    Effect.ShowAnimation("補給装置発動");
            //                                }
            //                            }

            //                            prev_value = withBlock7.EN;
            //                            {
            //                                var withBlock8 = MainPilot();
            //                                if (a.IsSpellAbility())
            //                                {
            //                                    epower = ((long)(elevel * withBlock8.Shooting) / 2L);
            //                                }
            //                                else
            //                                {
            //                                    epower = (50d * elevel);
            //                                }

            //                                epower = ((long)(epower * (10d + withBlock8.SkillLevel("補給", ref_mode: ""))) / 10L);
            //                            }

            //                            t.EN = t.EN + epower;
            //                            GUI.DrawSysString(withBlock7.x, withBlock7.y, "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock7.EN - prev_value));
            //                            if (ReferenceEquals(t, this))
            //                            {
            //                                GUI.UpdateMessageForm(this, u2: null);
            //                            }
            //                            else
            //                            {
            //                                GUI.UpdateMessageForm(t, this);
            //                            }

            //                            GUI.DisplaySysMessage(withBlock7.Nickname + "の" + Expression.Term("ＥＮ", t) + "が[" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock7.EN - prev_value) + "]回復した;" + "残り" + Expression.Term("ＥＮ", t) + "は" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock7.EN));
            //                            is_useful = true;
            //                        }
            //                        else if (elevel < 0d)
            //                        {
            //                            // ＥＮは既に0？
            //                            if (withBlock7.EN == 0)
            //                            {
            //                                goto NextLoop;
            //                            }

            //                            prev_value = withBlock7.EN;
            //                            {
            //                                var withBlock9 = MainPilot();
            //                                if (a.IsSpellAbility())
            //                                {
            //                                    epower = ((long)(elevel * withBlock9.Shooting) / 2L);
            //                                }
            //                                else
            //                                {
            //                                    epower = (50d * elevel);
            //                                }
            //                            }

            //                            t.EN = t.EN + epower;
            //                            GUI.DrawSysString(withBlock7.x, withBlock7.y, "-" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock7.EN));
            //                            if (ReferenceEquals(t, this))
            //                            {
            //                                GUI.UpdateMessageForm(this, u2: null);
            //                            }
            //                            else
            //                            {
            //                                GUI.UpdateMessageForm(t, this);
            //                            }

            //                            GUI.DisplaySysMessage(withBlock7.Nickname + "の" + Expression.Term("ＥＮ", t) + "が[" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock7.EN) + "]減少した;" + "残り" + Expression.Term("ＥＮ", t) + "は" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock7.EN));
            //                        }
            //                    }

            //                    break;
            //                }

            //            case "霊力回復":
            //            case "プラーナ回復":
            //                {
            //                    {
            //                        var withBlock10 = t.MainPilot();
            //                        if (elevel > 0d)
            //                        {
            //                            // 霊力は既に最大値？
            //                            if (withBlock10.Plana == withBlock10.MaxPlana())
            //                            {
            //                                goto NextLoop;
            //                            }

            //                            prev_value = withBlock10.Plana;
            //                            if (a.IsSpellAbility())
            //                            {
            //                                withBlock10.Plana = withBlock10.Plana + ((long)(elevel * this.MainPilot().Shooting) / 10L);
            //                            }
            //                            else
            //                            {
            //                                withBlock10.Plana = (withBlock10.Plana + 10d * elevel);
            //                            }

            //                            GUI.DrawSysString(t.x, t.y, "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock10.Plana - prev_value));
            //                            if (ReferenceEquals(t, this))
            //                            {
            //                                GUI.UpdateMessageForm(this, u2: null);
            //                            }
            //                            else
            //                            {
            //                                GUI.UpdateMessageForm(t, this);
            //                            }

            //                            GUI.DisplaySysMessage(withBlock10.get_Nickname(false) + "の[" + withBlock10.SkillName0("霊力") + "]が[" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock10.Plana - prev_value) + "]回復した。");
            //                            is_useful = true;
            //                        }
            //                        else if (elevel < 0d)
            //                        {
            //                            // 霊力は既に0？
            //                            if (withBlock10.Plana == 0)
            //                            {
            //                                goto NextLoop;
            //                            }

            //                            prev_value = withBlock10.Plana;
            //                            if (a.IsSpellAbility())
            //                            {
            //                                withBlock10.Plana = withBlock10.Plana + ((long)(elevel * this.MainPilot().Shooting) / 10L);
            //                            }
            //                            else
            //                            {
            //                                withBlock10.Plana = (withBlock10.Plana + 10d * elevel);
            //                            }

            //                            GUI.DrawSysString(t.x, t.y, "-" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock10.Plana));
            //                            if (ReferenceEquals(t, this))
            //                            {
            //                                GUI.UpdateMessageForm(this, u2: null);
            //                            }
            //                            else
            //                            {
            //                                GUI.UpdateMessageForm(t, this);
            //                            }

            //                            GUI.DisplaySysMessage(withBlock10.get_Nickname(false) + "の[" + withBlock10.SkillName0("霊力") + "]が[" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock10.Plana) + "]減少した。");
            //                        }
            //                    }

            //                    break;
            //                }

            //            case "ＳＰ回復":
            //                {
            //                    if (a.IsSpellAbility())
            //                    {
            //                        epower = ((long)(elevel * this.MainPilot().Shooting) / 10L);
            //                    }
            //                    else
            //                    {
            //                        epower = (10d * elevel);
            //                    }

            //                    {
            //                        var withBlock11 = t;
            //                        // パイロット数を計算
            //                        num = (withBlock11.CountPilot() + withBlock11.CountSupport());
            //                        if (withBlock11.IsFeatureAvailable("追加サポート"))
            //                        {
            //                            num = (num + 1);
            //                        }

            //                        if (elevel > 0d)
            //                        {
            //                            if (num == 1)
            //                            {
            //                                // パイロットが１名のみ
            //                                {
            //                                    var withBlock12 = withBlock11.MainPilot();
            //                                    prev_value = withBlock12.SP;
            //                                    withBlock12.SP = withBlock12.SP + epower;
            //                                    GUI.DrawSysString(t.x, t.y, "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock12.SP - prev_value));
            //                                    GUI.DisplaySysMessage(withBlock12.get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock12.SP - prev_value) + "回復した。");
            //                                    if (withBlock12.SP > prev_value)
            //                                    {
            //                                        is_useful = true;
            //                                    }
            //                                }
            //                            }
            //                            else
            //                            {
            //                                // 複数のパイロットが対象
            //                                {
            //                                    var withBlock13 = withBlock11.MainPilot();
            //                                    prev_value = withBlock13.SP;
            //                                    withBlock13.SP = withBlock13.SP + epower / 5 + epower / num;
            //                                    GUI.DrawSysString(t.x, t.y, "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock13.SP - prev_value));
            //                                    GUI.DisplaySysMessage(withBlock13.get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock13.SP - prev_value) + "回復した。");
            //                                    if (withBlock13.SP > prev_value)
            //                                    {
            //                                        is_useful = true;
            //                                    }
            //                                }

            //                                var loopTo8 = withBlock11.CountPilot();
            //                                for (j = 2; j <= loopTo8; j++)
            //                                {
            //                                    {
            //                                        var withBlock14 = withBlock11.Pilot("追加パイロット"0);
            //                                        prev_value = withBlock14.SP;
            //                                        withBlock14.SP = withBlock14.SP + epower / 5 + epower / num;
            //                                        GUI.DisplaySysMessage(withBlock14.get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock14.SP - prev_value) + "回復した。");
            //                                        if (withBlock14.SP > prev_value)
            //                                        {
            //                                            is_useful = true;
            //                                        }
            //                                    }
            //                                }

            //                                var loopTo9 = withBlock11.CountSupport();
            //                                for (j = 1; j <= loopTo9; j++)
            //                                {
            //                                    {
            //                                        var withBlock15 = withBlock11.Support(j);
            //                                        prev_value = withBlock15.SP;
            //                                        withBlock15.SP = withBlock15.SP + epower / 5 + epower / num;
            //                                        GUI.DisplaySysMessage(withBlock15.get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock15.SP - prev_value) + "回復した。");
            //                                        if (withBlock15.SP > prev_value)
            //                                        {
            //                                            is_useful = true;
            //                                        }
            //                                    }
            //                                }

            //                                if (withBlock11.IsFeatureAvailable("追加サポート"))
            //                                {
            //                                    {
            //                                        var withBlock16 = withBlock11.AdditionalSupport();
            //                                        prev_value = withBlock16.SP;
            //                                        withBlock16.SP = withBlock16.SP + epower / 5 + epower / num;
            //                                        GUI.DisplaySysMessage(withBlock16.get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock16.SP - prev_value) + "回復した。");
            //                                        if (withBlock16.SP > prev_value)
            //                                        {
            //                                            is_useful = true;
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        }
            //                        else if (elevel < 0d)
            //                        {
            //                            if (num == 1)
            //                            {
            //                                // パイロットが１名のみ
            //                                {
            //                                    var withBlock17 = withBlock11.MainPilot();
            //                                    prev_value = withBlock17.SP;
            //                                    withBlock17.SP = withBlock17.SP + epower;
            //                                    GUI.DrawSysString(t.x, t.y, "-" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock17.SP));
            //                                    GUI.DisplaySysMessage(withBlock17.get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock17.SP) + "減少した。");
            //                                }
            //                            }
            //                            else
            //                            {
            //                                // 複数のパイロットが対象
            //                                {
            //                                    var withBlock18 = withBlock11.MainPilot();
            //                                    prev_value = withBlock18.SP;
            //                                    withBlock18.SP = withBlock18.SP + epower / 5 + epower / num;
            //                                    GUI.DrawSysString(t.x, t.y, "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock18.SP));
            //                                    GUI.DisplaySysMessage(withBlock18.get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock18.SP) + "減少した。");
            //                                }

            //                                var loopTo10 = withBlock11.CountPilot();
            //                                for (j = 2; j <= loopTo10; j++)
            //                                {
            //                                    {
            //                                        var withBlock19 = withBlock11.Pilot(j);
            //                                        prev_value = withBlock19.SP;
            //                                        withBlock19.SP = withBlock19.SP + epower / 5 + epower / num;
            //                                        GUI.DisplaySysMessage(withBlock19.get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock19.SP) + "減少した。");
            //                                    }
            //                                }

            //                                var loopTo11 = withBlock11.CountSupport();
            //                                for (j = 1; j <= loopTo11; j++)
            //                                {
            //                                    {
            //                                        var withBlock20 = withBlock11.Support(j);
            //                                        prev_value = withBlock20.SP;
            //                                        withBlock20.SP = withBlock20.SP + epower / 5 + epower / num;
            //                                        GUI.DisplaySysMessage(withBlock20.get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock20.SP) + "減少した。");
            //                                    }
            //                                }

            //                                if (withBlock11.IsFeatureAvailable("追加サポート"))
            //                                {
            //                                    {
            //                                        var withBlock21 = withBlock11.AdditionalSupport();
            //                                        prev_value = withBlock21.SP;
            //                                        withBlock21.SP = withBlock21.SP + epower / 5 + epower / num;
            //                                        GUI.DisplaySysMessage(withBlock21.get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock21.SP) + "減少した。");
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }

            //                    break;
            //                }

            //            case "気力増加":
            //                {
            //                    if (a.IsSpellAbility())
            //                    {
            //                        epower = ((long)(elevel * this.MainPilot().Shooting) / 10L);
            //                    }
            //                    else
            //                    {
            //                        epower = (10d * elevel);
            //                    }

            //                    {
            //                        var withBlock22 = t;
            //                        prev_value = withBlock22.MainPilot().Morale;
            //                        withBlock22.IncreaseMorale(epower);
            //                        if (elevel > 0d)
            //                        {
            //                            {
            //                                var withBlock23 = withBlock22.MainPilot();
            //                                GUI.DrawSysString(t.x, t.y, "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock23.Morale - prev_value));
            //                                GUI.DisplaySysMessage(withBlock23.get_Nickname(false) + "の" + Expression.Term("気力", t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock23.Morale - prev_value) + "増加した。");
            //                            }
            //                        }
            //                        else if (elevel < 0d)
            //                        {
            //                            {
            //                                var withBlock24 = withBlock22.MainPilot();
            //                                GUI.DrawSysString(t.x, t.y, "-" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock24.Morale));
            //                                GUI.DisplaySysMessage(withBlock24.get_Nickname(false) + "の" + Expression.Term("気力", t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock24.Morale) + "減少した。");
            //                            }
            //                        }

            //                        if (withBlock22.MainPilot().Morale > prev_value)
            //                        {
            //                            is_useful = true;
            //                        }
            //                    }

            //                    break;
            //                }

            //            case "装填":
            //                {
            //                    {
            //                        var withBlock25 = t;
            //                        flag = false;
            //                        if (string.IsNullOrEmpty(edata))
            //                        {
            //                            // 全ての武器の弾数を回復
            //                            var loopTo12 = withBlock25.CountWeapon();
            //                            for (j = 1; j <= loopTo12; j++)
            //                            {
            //                                if (withBlock25.Bullet(j) < withBlock25.MaxBullet(j))
            //                                {
            //                                    withBlock25.BulletSupply();
            //                                    flag = true;
            //                                    break;
            //                                }
            //                            }

            //                            // 弾数とアビリティ使用回数の同期を取る
            //                            if (flag)
            //                            {
            //                                var loopTo13 = withBlock25.CountAbility();
            //                                for (j = 1; j <= loopTo13; j++)
            //                                {
            //                                    if (withBlock25.IsAbilityClassifiedAs(j, "共"))
            //                                    {
            //                                        var loopTo14 = withBlock25.CountWeapon();
            //                                        for (k = 1; k <= loopTo14; k++)
            //                                        {
            //                                            if (withBlock25.IsWeaponClassifiedAs(k, "共") & withBlock25.AbilityLevel(j, "共") == withBlock25.WeaponLevel(k, "共"))
            //                                            {
            //                                                withBlock25.SetStock(j, withBlock25.MaxStock(j));
            //                                            }
            //                                        }
            //                                    }
            //                                }

            //                                // 弾数・使用回数の共有化処理
            //                                withBlock25.SyncBullet();
            //                            }
            //                        }
            //                        else
            //                        {
            //                            // 特定の武器の弾数のみを回復
            //                            var loopTo15 = withBlock25.CountWeapon();
            //                            for (j = 1; j <= loopTo15; j++)
            //                            {
            //                                if (withBlock25.Bullet(j) < withBlock25.MaxBullet(j))
            //                                {
            //                                    if ((withBlock25.WeaponNickname(j) ?? "") == (edata ?? "") | GeneralLib.InStrNotNest(withBlock25.Weapon(j).Class, edata) > 0)
            //                                    {
            //                                        withBlock25.SetBullet(j, withBlock25.MaxBullet(j));
            //                                        flag = true;
            //                                        w = j;
            //                                    }
            //                                }
            //                            }

            //                            var loopTo16 = withBlock25.CountOtherForm();
            //                            for (j = 1; j <= loopTo16; j++)
            //                            {
            //                                {
            //                                    var withBlock26 = withBlock25.OtherForm(j);
            //                                    var loopTo17 = withBlock26.CountWeapon();
            //                                    for (k = 1; k <= loopTo17; k++)
            //                                    {
            //                                        if (withBlock26.Bullet(k) < withBlock26.MaxBullet(k))
            //                                        {
            //                                            if ((withBlock26.WeaponNickname(k) ?? "") == (edata ?? "") | GeneralLib.InStrNotNest(withBlock26.Weapon(k).Class, edata) > 0)
            //                                            {
            //                                                withBlock26.SetBullet(k, withBlock26.MaxBullet(k));
            //                                            }
            //                                        }
            //                                    }
            //                                }
            //                            }

            //                            // 弾数の同期を取る
            //                            if (flag)
            //                            {
            //                                if (withBlock25.IsWeaponClassifiedAs(w, "共"))
            //                                {
            //                                    var loopTo18 = withBlock25.CountWeapon();
            //                                    for (j = 1; j <= loopTo18; j++)
            //                                    {
            //                                        if (withBlock25.IsWeaponClassifiedAs(j, "共") & withBlock25.WeaponLevel(j, "共") == withBlock25.WeaponLevel(w, "共"))
            //                                        {
            //                                            withBlock25.SetBullet(j, withBlock25.MaxBullet(j));
            //                                        }
            //                                    }

            //                                    var loopTo19 = withBlock25.CountAbility();
            //                                    for (j = 1; j <= loopTo19; j++)
            //                                    {
            //                                        if (withBlock25.IsAbilityClassifiedAs(j, "共") & withBlock25.AbilityLevel(j, "共") == withBlock25.WeaponLevel(w, "共"))
            //                                        {
            //                                            withBlock25.SetStock(j, withBlock25.MaxStock(j));
            //                                        }
            //                                    }
            //                                }

            //                                // 弾数・使用回数の共有化処理
            //                                withBlock25.SyncBullet();
            //                            }
            //                        }

            //                        if (flag)
            //                        {
            //                            GUI.DisplaySysMessage(withBlock25.Nickname + "の武装の使用回数が回復した。");
            //                            if (a.AbilityMaxRange() > 0)
            //                            {
            //                                is_useful = true;
            //                            }
            //                        }
            //                    }

            //                    break;
            //                }

            //            case "治癒":
            //                {
            //                    {
            //                        var withBlock27 = t;
            //                        if (!is_anime_played)
            //                        {
            //                            if (a.IsSpellAbility() | a.IsAbilityClassifiedAs("魔"))
            //                            {
            //                                Effect.ShowAnimation("回復魔法発動");
            //                            }
            //                        }

            //                        if (string.IsNullOrEmpty(edata))
            //                        {
            //                            // 全てのステータス異常を回復
            //                            if (withBlock27.ConditionLifetime("攻撃不能") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("攻撃不能");
            //                                is_useful = true;
            //                            }

            //                            if (withBlock27.ConditionLifetime("移動不能") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("移動不能");
            //                                is_useful = true;
            //                            }

            //                            if (withBlock27.ConditionLifetime("装甲劣化") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("装甲劣化");
            //                                is_useful = true;
            //                            }

            //                            if (withBlock27.ConditionLifetime("混乱") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("混乱");
            //                                is_useful = true;
            //                            }

            //                            if (withBlock27.ConditionLifetime("恐怖") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("恐怖");
            //                                is_useful = true;
            //                            }

            //                            if (withBlock27.ConditionLifetime("踊り") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("踊り");
            //                                is_useful = true;
            //                            }

            //                            if (withBlock27.ConditionLifetime("狂戦士") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("狂戦士");
            //                                is_useful = true;
            //                            }

            //                            if (withBlock27.ConditionLifetime("ゾンビ") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("ゾンビ");
            //                                is_useful = true;
            //                            }

            //                            if (withBlock27.ConditionLifetime("回復不能") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("回復不能");
            //                                is_useful = true;
            //                            }

            //                            if (withBlock27.ConditionLifetime("石化") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("石化");
            //                                is_useful = true;
            //                            }

            //                            if (withBlock27.ConditionLifetime("凍結") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("凍結");
            //                                is_useful = true;
            //                            }

            //                            if (withBlock27.ConditionLifetime("麻痺") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("麻痺");
            //                                is_useful = true;
            //                            }

            //                            if (withBlock27.ConditionLifetime("睡眠") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("睡眠");
            //                                is_useful = true;
            //                            }

            //                            if (withBlock27.ConditionLifetime("毒") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("毒");
            //                                is_useful = true;
            //                            }

            //                            if (withBlock27.ConditionLifetime("盲目") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("盲目");
            //                                is_useful = true;
            //                            }

            //                            if (withBlock27.ConditionLifetime("沈黙") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("沈黙");
            //                                is_useful = true;
            //                            }

            //                            if (withBlock27.ConditionLifetime("魅了") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("魅了");
            //                                is_useful = true;
            //                            }

            //                            if (withBlock27.ConditionLifetime("憑依") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("憑依");
            //                                is_useful = true;
            //                            }
            //                            // 剋属性
            //                            if (withBlock27.ConditionLifetime("オーラ使用不能") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("オーラ使用不能");
            //                            }

            //                            if (withBlock27.ConditionLifetime("超能力使用不能") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("超能力使用不能");
            //                            }

            //                            if (withBlock27.ConditionLifetime("同調率使用不能") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("同調率使用不能");
            //                            }

            //                            if (withBlock27.ConditionLifetime("超感覚使用不能") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("超感覚使用不能");
            //                            }

            //                            if (withBlock27.ConditionLifetime("知覚強化使用不能") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("知覚強化使用不能");
            //                            }

            //                            if (withBlock27.ConditionLifetime("霊力使用不能") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("霊力使用不能");
            //                            }

            //                            if (withBlock27.ConditionLifetime("術使用不能") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("術使用不能");
            //                            }

            //                            if (withBlock27.ConditionLifetime("技使用不能") > 0)
            //                            {
            //                                withBlock27.DeleteCondition("技使用不能");
            //                            }

            //                            j = 1;
            //                            while (j <= withBlock27.CountCondition())
            //                            {
            //                                // 弱点、有効付加はあえて外してあります。
            //                                string localCondition5() { object argIndex1 = j; var ret = withBlock27.Condition(argIndex1); return ret; }

            //                                string localCondition6() { object argIndex1 = j; var ret = withBlock27.Condition(argIndex1); return ret; }

            //                                string localCondition7() { object argIndex1 = j; var ret = withBlock27.Condition(argIndex1); return ret; }

            //                                int localConditionLifetime1() { object argIndex1 = (object)hs35eef8b33aab4975b1c788eecf306c48(); var ret = withBlock27.ConditionLifetime(argIndex1); return ret; }

            //                                if (Strings.Len(localCondition5()) > 6 & Strings.Right(localCondition6(), 6) == "属性使用不能" & localConditionLifetime1() > 0)
            //                                {
            //                                    string localCondition4() { object argIndex1 = j; var ret = withBlock27.Condition(argIndex1); return ret; }

            //                                    withBlock27.DeleteCondition(localCondition4());
            //                                    is_useful = true;
            //                                }
            //                                else
            //                                {
            //                                    j = (j + 1);
            //                                }
            //                            }

            //                            if (is_useful)
            //                            {
            //                                if (ReferenceEquals(t, CurrentForm()))
            //                                {
            //                                    GUI.UpdateMessageForm(t, u2: null);
            //                                }
            //                                else
            //                                {
            //                                    GUI.UpdateMessageForm(t, CurrentForm());
            //                                }

            //                                GUI.DisplaySysMessage(withBlock27.Nickname + "の状態が回復した。");
            //                            }
            //                        }
            //                        else
            //                        {
            //                            // 指定されたステータス異常のみを回復
            //                            j = 1;
            //                            while (j <= GeneralLib.LLength(edata))
            //                            {
            //                                cname = GeneralLib.LIndex(edata, j);
            //                                if (withBlock27.ConditionLifetime(cname) > 0)
            //                                {
            //                                    withBlock27.DeleteCondition(cname);
            //                                    if (ReferenceEquals(t, CurrentForm()))
            //                                    {
            //                                        GUI.UpdateMessageForm(t, u2: null);
            //                                    }
            //                                    else
            //                                    {
            //                                        GUI.UpdateMessageForm(t, CurrentForm());
            //                                    }

            //                                    if (cname == "装甲劣化")
            //                                    {
            //                                        cname = Expression.Term("装甲", t) + "劣化";
            //                                    }

            //                                    GUI.DisplaySysMessage(withBlock27.Nickname + "の[" + cname + "]が回復した。");
            //                                    is_useful = true;
            //                                }
            //                                else
            //                                {
            //                                    j = (j + 1);
            //                                }
            //                            }
            //                        }
            //                    }

            //                    break;
            //                }

            //            case "付加":
            //                {
            //                    {
            //                        var withBlock28 = t;
            //                        if (elevel2 == 0d)
            //                        {
            //                            // レベル指定がない場合は付加が半永久的に持続
            //                            elevel2 = 10000d;
            //                        }
            //                        else
            //                        {
            //                            // そうでなければ最低１ターンは効果が持続
            //                            elevel2 = GeneralLib.MaxLng(elevel2, 1);
            //                        }

            //                        // 効果時間が継続中？
            //                        if (withBlock28.IsConditionSatisfied(GeneralLib.LIndex(edata, 1) + "付加"))
            //                        {
            //                            goto NextLoop;
            //                        }

            //                        ftype = GeneralLib.LIndex(edata, 1);
            //                        flevel = Conversions.ToDouble(GeneralLib.LIndex(edata, 2));
            //                        fdata = "";
            //                        var loopTo20 = GeneralLib.LLength(edata);
            //                        for (j = 3; j <= loopTo20; j++)
            //                            fdata = fdata + GeneralLib.LIndex(edata, j) + " ";
            //                        fdata = Strings.Trim(fdata);
            //                        if (Strings.Left(fdata, 1) == "\"" & Strings.Right(fdata, 1) == "\"")
            //                        {
            //                            fdata = Strings.Trim(Strings.Mid(fdata, 2, Strings.Len(fdata) - 2));
            //                        }

            //                        // エリアスが定義されている？
            //                        if (SRC.ALDList.IsDefined(ftype))
            //                        {
            //                            {
            //                                var withBlock29 = SRC.ALDList.Item(ftype);
            //                                var loopTo21 = withBlock29.Count;
            //                                for (j = 1; j <= loopTo21; j++)
            //                                {
            //                                    // エリアスの定義に従って特殊能力定義を置き換える
            //                                    ftype2 = withBlock29.get_AliasType(j);
            //                                    string localLIndex() { string arglist = withBlock29.get_AliasData(j); var ret = GeneralLib.LIndex(arglist, 1); withBlock29.get_AliasData(j) = arglist; return ret; }

            //                                    if (localLIndex() == "解説")
            //                                    {
            //                                        // 特殊能力の解説
            //                                        if (!string.IsNullOrEmpty(fdata))
            //                                        {
            //                                            ftype2 = GeneralLib.LIndex(fdata, 1);
            //                                        }

            //                                        flevel2 = SRC.DEFAULT_LEVEL;
            //                                        fdata2 = withBlock29.get_AliasData(j);
            //                                    }
            //                                    else
            //                                    {
            //                                        // 通常の特殊能力
            //                                        if (withBlock29.get_AliasLevelIsPlusMod(j))
            //                                        {
            //                                            if (flevel == SRC.DEFAULT_LEVEL)
            //                                            {
            //                                                flevel = 1d;
            //                                            }

            //                                            flevel2 = flevel + withBlock29.get_AliasLevel(j);
            //                                        }
            //                                        else if (withBlock29.get_AliasLevelIsMultMod(j))
            //                                        {
            //                                            if (flevel == SRC.DEFAULT_LEVEL)
            //                                            {
            //                                                flevel = 1d;
            //                                            }

            //                                            flevel2 = flevel * withBlock29.get_AliasLevel(j);
            //                                        }
            //                                        else if (flevel != SRC.DEFAULT_LEVEL)
            //                                        {
            //                                            flevel2 = flevel;
            //                                        }
            //                                        else
            //                                        {
            //                                            flevel2 = withBlock29.get_AliasLevel(j);
            //                                        }

            //                                        fdata2 = withBlock29.get_AliasData(j);
            //                                        if (!string.IsNullOrEmpty(fdata))
            //                                        {
            //                                            if (Strings.InStr(fdata2, "非表示") != 1)
            //                                            {
            //                                                fdata2 = fdata + " " + GeneralLib.ListTail(fdata2, (GeneralLib.LLength(fdata) + 1));
            //                                            }
            //                                        }
            //                                    }

            //                                    t.AddCondition(ftype2 + "付加", elevel2, flevel2, fdata2);
            //                                }
            //                            }
            //                        }
            //                        else
            //                        {
            //                            withBlock28.AddCondition(ftype + "付加", elevel2, flevel, fdata);
            //                        }

            //                        withBlock28.Update();
            //                        if (ReferenceEquals(t, CurrentForm()))
            //                        {
            //                            GUI.UpdateMessageForm(t, u2: null);
            //                        }
            //                        else
            //                        {
            //                            GUI.UpdateMessageForm(t, CurrentForm());
            //                        }

            //                        switch (GeneralLib.LIndex(edata, 1) ?? "")
            //                        {
            //                            case "耐性":
            //                            case "無効化":
            //                            case "吸収":
            //                                {
            //                                    GUI.DisplaySysMessage(withBlock28.Nickname + "は[" + GeneralLib.LIndex(edata, 3) + "]属性に対する[" + GeneralLib.LIndex(edata, 1) + "]能力を得た。");
            //                                    break;
            //                                }

            //                            case "特殊効果無効化":
            //                                {
            //                                    GUI.DisplaySysMessage(withBlock28.Nickname + "は[" + GeneralLib.LIndex(edata, 3) + "]属性に対する無効化能力を得た。");
            //                                    break;
            //                                }

            //                            case "攻撃属性":
            //                                {
            //                                    GUI.DisplaySysMessage(withBlock28.Nickname + "は[" + GeneralLib.LIndex(edata, 3) + "]の攻撃属性を得た。");
            //                                    break;
            //                                }

            //                            case "武器強化":
            //                                {
            //                                    GUI.DisplaySysMessage(withBlock28.Nickname + "の" + "武器の攻撃力が上がった。");
            //                                    break;
            //                                }

            //                            case "命中率強化":
            //                                {
            //                                    GUI.DisplaySysMessage(withBlock28.Nickname + "の" + "武器の命中率が上がった。");
            //                                    break;
            //                                }

            //                            case "ＣＴ率強化":
            //                                {
            //                                    GUI.DisplaySysMessage(withBlock28.Nickname + "の" + "武器のＣＴ率が上がった。");
            //                                    break;
            //                                }

            //                            case "特殊効果発動率強化":
            //                                {
            //                                    GUI.DisplaySysMessage(withBlock28.Nickname + "の" + "武器の特殊効果発動率が上がった。");
            //                                    break;
            //                                }

            //                            case "射程延長":
            //                                {
            //                                    GUI.DisplaySysMessage(withBlock28.Nickname + "の" + "武器の射程が伸びた。");
            //                                    break;
            //                                }

            //                            case "サイズ変更":
            //                                {
            //                                    GUI.DisplaySysMessage(withBlock28.Nickname + "の" + "サイズが" + Strings.StrConv(GeneralLib.LIndex(edata, 3), VbStrConv.Wide) + "サイズに変化した。");
            //                                    break;
            //                                }
            //                            // メッセージを表示しない。
            //                            case "パイロット愛称":
            //                            case "パイロット画像":
            //                            case "愛称変更":
            //                            case "ユニット画像":
            //                            case "ＢＧＭ":
            //                                {
            //                                    break;
            //                                }

            //                            default:
            //                                {
            //                                    // 付加する能力名
            //                                    fname = GeneralLib.ListIndex(fdata, 1);
            //                                    if (string.IsNullOrEmpty(fname) | fname == "非表示")
            //                                    {
            //                                        if ((GeneralLib.LIndex(edata, 2) ?? "") != (Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SRC.DEFAULT_LEVEL) ?? ""))
            //                                        {
            //                                            fname = GeneralLib.LIndex(edata, 1) + "Lv" + GeneralLib.LIndex(edata, 2);
            //                                        }
            //                                        else
            //                                        {
            //                                            fname = GeneralLib.LIndex(edata, 1);
            //                                        }
            //                                    }

            //                                    GUI.DisplaySysMessage(withBlock28.Nickname + "は[" + fname + "]の能力を得た。");
            //                                    break;
            //                                }
            //                        }

            //                        if (a.AbilityMaxRange() > 0)
            //                        {
            //                            is_useful = true;
            //                        }
            //                    }

            //                    break;
            //                }

            //            case "強化":
            //                {
            //                    {
            //                        var withBlock30 = t;
            //                        if (elevel2 == 0d)
            //                        {
            //                            // レベル指定がない場合は付加が半永久的に持続
            //                            elevel2 = 10000d;
            //                        }
            //                        else
            //                        {
            //                            // そうでなければ最低１ターンは効果が持続
            //                            elevel2 = GeneralLib.MaxLng(elevel2, 1);
            //                        }

            //                        // 効果時間が継続中？
            //                        if (withBlock30.IsConditionSatisfied(GeneralLib.LIndex(edata, 1) + "強化"))
            //                        {
            //                            goto NextLoop;
            //                        }

            //                        ftype = GeneralLib.LIndex(edata, 1);
            //                        flevel = Conversions.ToDouble(GeneralLib.LIndex(edata, 2));
            //                        fdata = "";
            //                        var loopTo22 = GeneralLib.LLength(edata);
            //                        for (j = 3; j <= loopTo22; j++)
            //                            fdata = fdata + GeneralLib.LIndex(edata, j) + " ";
            //                        fdata = Strings.Trim(fdata);

            //                        // エリアスが定義されている？
            //                        if (SRC.ALDList.IsDefined(ftype))
            //                        {
            //                            {
            //                                var withBlock31 = SRC.ALDList.Item(ftype);
            //                                var loopTo23 = withBlock31.Count;
            //                                for (j = 1; j <= loopTo23; j++)
            //                                {
            //                                    // エリアスの定義に従って特殊能力定義を置き換える
            //                                    ftype2 = withBlock31.get_AliasType(i);
            //                                    string localLIndex1() { string arglist = withBlock31.get_AliasData(j); var ret = GeneralLib.LIndex(arglist, 1); withBlock31.get_AliasData(j) = arglist; return ret; }

            //                                    if (localLIndex1() == "解説")
            //                                    {
            //                                        // 特殊能力の解説
            //                                        if (!string.IsNullOrEmpty(fdata))
            //                                        {
            //                                            ftype2 = GeneralLib.LIndex(fdata, 1);
            //                                        }

            //                                        flevel2 = SRC.DEFAULT_LEVEL;
            //                                        fdata2 = withBlock31.get_AliasData(j);
            //                                        t.AddCondition(ftype2 + "付加", elevel2, flevel2, fdata2);
            //                                    }
            //                                    else
            //                                    {
            //                                        // 通常の特殊能力
            //                                        if (withBlock31.get_AliasLevelIsMultMod(j))
            //                                        {
            //                                            if (flevel == SRC.DEFAULT_LEVEL)
            //                                            {
            //                                                flevel = 1d;
            //                                            }

            //                                            flevel2 = flevel * withBlock31.get_AliasLevel(j);
            //                                        }
            //                                        else if (flevel != SRC.DEFAULT_LEVEL)
            //                                        {
            //                                            flevel2 = flevel;
            //                                        }
            //                                        else
            //                                        {
            //                                            flevel2 = withBlock31.get_AliasLevel(j);
            //                                        }

            //                                        fdata2 = withBlock31.get_AliasData(j);
            //                                        if (!string.IsNullOrEmpty(fdata))
            //                                        {
            //                                            if (Strings.InStr(fdata2, "非表示") != 1)
            //                                            {
            //                                                fdata2 = fdata + " " + GeneralLib.ListTail(fdata2, (GeneralLib.LLength(fdata) + 1));
            //                                            }
            //                                        }

            //                                        t.AddCondition(ftype2 + "強化", elevel2, flevel2, fdata2);
            //                                    }
            //                                }
            //                            }
            //                        }
            //                        else
            //                        {
            //                            withBlock30.AddCondition(ftype + "強化", elevel2, flevel, fdata);
            //                        }

            //                        withBlock30.Update();
            //                        if (ReferenceEquals(t, CurrentForm()))
            //                        {
            //                            GUI.UpdateMessageForm(t, u2: null);
            //                        }
            //                        else
            //                        {
            //                            GUI.UpdateMessageForm(t, CurrentForm());
            //                        }

            //                        // 強化する能力名
            //                        fname = GeneralLib.LIndex(edata, 3);
            //                        if (string.IsNullOrEmpty(fname) | fname == "非表示")
            //                        {
            //                            fname = GeneralLib.LIndex(edata, 1);
            //                        }

            //                        if (t.SkillName0(fname) != "非表示")
            //                        {
            //                            fname = t.SkillName0(fname);
            //                        }

            //                        GUI.DisplaySysMessage(withBlock30.Nickname + "の[" + fname + "]レベルが" + GeneralLib.LIndex(edata, 2) + "上がった。");
            //                        if (a.AbilityMaxRange() > 0)
            //                        {
            //                            is_useful = true;
            //                        }
            //                    }

            //                    break;
            //                }

            //            case "状態":
            //                {
            //                    {
            //                        var withBlock32 = t;
            //                        if (elevel2 == 0d)
            //                        {
            //                            // レベル指定がない場合は付加が半永久的に持続
            //                            elevel2 = 10000d;
            //                        }
            //                        else
            //                        {
            //                            // そうでなければ最低１ターンは状態が持続
            //                            elevel = GeneralLib.MaxLng(elevel2, 1);
            //                        }

            //                        // 効果時間が継続中？
            //                        if (withBlock32.IsConditionSatisfied(edata))
            //                        {
            //                            goto NextLoop;
            //                        }

            //                        withBlock32.AddCondition(edata, elevel2, cdata: "");

            //                        // 状態発動アニメーション表示
            //                        bool localIsAnimationDefined() { string argmain_situation = aname + "(発動)"; string argsub_situation = ""; var ret = IsAnimationDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //                        if (!localIsAnimationDefined() & !IsAnimationDefined(aname, sub_situation: ""))
            //                        {
            //                            switch (edata ?? "")
            //                            {
            //                                case "攻撃力ＵＰ":
            //                                case "防御力ＵＰ":
            //                                case "運動性ＵＰ":
            //                                case "移動力ＵＰ":
            //                                case "狂戦士":
            //                                    {
            //                                        Effect.ShowAnimation(edata + "発動");
            //                                        break;
            //                                    }
            //                            }
            //                        }

            //                        switch (edata ?? "")
            //                        {
            //                            case "装甲劣化":
            //                                {
            //                                    cname = Expression.Term("装甲", t) + "劣化";
            //                                    break;
            //                                }

            //                            case "運動性ＵＰ":
            //                                {
            //                                    cname = Expression.Term("運動性", t) + "ＵＰ";
            //                                    break;
            //                                }

            //                            case "運動性ＤＯＷＮ":
            //                                {
            //                                    cname = Expression.Term("運動性", t) + "ＤＯＷＮ";
            //                                    break;
            //                                }

            //                            case "移動力ＵＰ":
            //                                {
            //                                    cname = Expression.Term("移動力", t) + "ＵＰ";
            //                                    break;
            //                                }

            //                            case "移動力ＤＯＷＮ":
            //                                {
            //                                    cname = Expression.Term("移動力", t) + "ＤＯＷＮ";
            //                                    break;
            //                                }

            //                            default:
            //                                {
            //                                    cname = edata;
            //                                    break;
            //                                }
            //                        }

            //                        GUI.DisplaySysMessage(withBlock32.Nickname + "は" + cname + "の状態になった。");
            //                        if (a.AbilityMaxRange() > 0)
            //                        {
            //                            is_useful = true;
            //                        }
            //                    }

            //                    break;
            //                }

            //            case "召喚":
            //                {
            //                    GUI.UpdateMessageForm(CurrentForm(), u2: null);
            //                    bool localIsDefined2() { object argIndex1 = edata; var ret = SRC.UDList.IsDefined(argIndex1); return ret; }

            //                    if (!localIsDefined2())
            //                    {
            //                        GUI.ErrorMessage(edata + "のデータが定義されていません");
            //                        return ExecuteAbilityRet;
            //                    }

            //                    UnitData localItem() { object argIndex1 = edata; var ret = SRC.UDList.Item(argIndex1); return ret; }

            //                    pname = localItem().FeatureData("追加パイロット");
            //                    bool localIsDefined3() { object argIndex1 = pname; var ret = SRC.PDList.IsDefined(argIndex1); return ret; }

            //                    if (!localIsDefined3())
            //                    {
            //                        GUI.ErrorMessage("追加パイロット「" + pname + "」のデータがありません");
            //                        return ExecuteAbilityRet;
            //                    }

            //                    // 召喚したユニットを配置する座標を決定する。
            //                    // 最も近い敵ユニットの方向にユニットを配置する。
            //                    u = COM.SearchNearestEnemy(this);
            //                    if (u is object)
            //                    {
            //                        if (Math.Abs((x - u.x)) > Math.Abs((y - u.y)))
            //                        {
            //                            if (x < u.x)
            //                            {
            //                                tx = (x + 1);
            //                            }
            //                            else if (x > u.x)
            //                            {
            //                                tx = (x - 1);
            //                            }
            //                            else
            //                            {
            //                                tx = x;
            //                            }

            //                            ty = y;
            //                            tx2 = x;
            //                            if (y < u.y)
            //                            {
            //                                ty2 = (y + 1);
            //                            }
            //                            else if (y > u.y)
            //                            {
            //                                ty2 = (y - 1);
            //                            }
            //                            else if (y == 1)
            //                            {
            //                                if (Map.MapDataForUnit[x, 2] is null)
            //                                {
            //                                    ty2 = 2;
            //                                }
            //                                else
            //                                {
            //                                    ty2 = 1;
            //                                }
            //                            }
            //                            else if (y == Map.MapHeight)
            //                            {
            //                                if (Map.MapDataForUnit[x, Map.MapHeight - 1] is null)
            //                                {
            //                                    ty2 = (Map.MapHeight - 1);
            //                                }
            //                                else
            //                                {
            //                                    ty2 = Map.MapHeight;
            //                                }
            //                            }
            //                            else if (Map.MapDataForUnit[x, y - 1] is null)
            //                            {
            //                                ty2 = (y - 1);
            //                            }
            //                            else if (Map.MapDataForUnit[x, y + 1] is null)
            //                            {
            //                                ty2 = (y - 1);
            //                            }
            //                            else
            //                            {
            //                                ty2 = y;
            //                            }
            //                        }
            //                        else
            //                        {
            //                            tx = x;
            //                            if (y < u.y)
            //                            {
            //                                ty = (y + 1);
            //                            }
            //                            else if (y > u.y)
            //                            {
            //                                ty = (y - 1);
            //                            }
            //                            else
            //                            {
            //                                ty = y;
            //                            }

            //                            if (x < u.x)
            //                            {
            //                                tx2 = (x + 1);
            //                            }
            //                            else if (x > u.x)
            //                            {
            //                                tx2 = (x - 1);
            //                            }
            //                            else if (x == 1)
            //                            {
            //                                if (Map.MapDataForUnit[2, y] is null)
            //                                {
            //                                    tx2 = 2;
            //                                }
            //                                else
            //                                {
            //                                    tx2 = 1;
            //                                }
            //                            }
            //                            else if (x == Map.MapWidth)
            //                            {
            //                                if (Map.MapDataForUnit[Map.MapWidth - 1, y] is null)
            //                                {
            //                                    tx2 = (Map.MapWidth - 1);
            //                                }
            //                                else
            //                                {
            //                                    tx2 = Map.MapWidth;
            //                                }
            //                            }
            //                            else if (Map.MapDataForUnit[x - 1, y] is null)
            //                            {
            //                                tx2 = (x - 1);
            //                            }
            //                            else if (Map.MapDataForUnit[x + 1, y] is null)
            //                            {
            //                                tx2 = (x + 1);
            //                            }
            //                            else
            //                            {
            //                                tx2 = x;
            //                            }

            //                            ty2 = y;
            //                        }
            //                    }
            //                    else
            //                    {
            //                        tx = x;
            //                        ty = y;
            //                        tx2 = x;
            //                        ty2 = y;
            //                    }

            //                    var loopTo24 = GeneralLib.MaxLng(elevel, 1);
            //                    for (j = 1; j <= loopTo24; j++)
            //                    {
            //                        PilotData localItem1() { object argIndex1 = pname; var ret = SRC.PDList.Item(argIndex1); return ret; }

            //                        PilotData localItem2() { object argIndex1 = pname; var ret = SRC.PDList.Item(argIndex1); return ret; }

            //                        if (Strings.InStr(localItem1().Name, "(ザコ)") > 0 | Strings.InStr(localItem2().Name, "(汎用)") > 0)
            //                        {
            //                            p = SRC.PList.Add(pname, MainPilot().Level, Party, gid: "");
            //                            Party = argpparty1;
            //                            p.FullRecover();
            //                            u = SRC.UList.Add(edata, Rank, Party);
            //                            Party = arguparty;
            //                        }
            //                        else
            //                        {
            //                            bool localIsDefined4() { object argIndex1 = pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                            if (!localIsDefined4())
            //                            {
            //                                p = SRC.PList.Add(pname, MainPilot().Level, Party, gid: "");
            //                                Party = argpparty2;
            //                                p.FullRecover();
            //                                u = SRC.UList.Add(edata, Rank, Party);
            //                                Party = arguparty1;
            //                            }
            //                            else
            //                            {
            //                                p = SRC.PList.Item(pname);
            //                                u = p.Unit;
            //                                if (u is null)
            //                                {
            //                                    if (SRC.UList.IsDefined(edata))
            //                                    {
            //                                        u = SRC.UList.Item(edata);
            //                                    }
            //                                    else
            //                                    {
            //                                        u = SRC.UList.Add(edata, Rank, Party);
            //                                        Party = arguparty2;
            //                                    }
            //                                }
            //                            }
            //                        }

            //                        p.Ride(u);
            //                        AddServant(u);
            //                        if (Party == "味方")
            //                        {
            //                            if (GeneralLib.LIndex(u.FeatureData("召喚ユニット"), 2) == "ＮＰＣ")
            //                            {
            //                                u.ChangeParty("ＮＰＣ");
            //                            }
            //                        }

            //                        u.Summoner = CurrentForm();
            //                        u.FullRecover();
            //                        u.Mode = MainPilot().ID;
            //                        u.UsedAction = 0;
            //                        if (u.IsFeatureAvailable("制限時間"))
            //                        {
            //                            u.AddCondition("残り時間", Conversions.Toint(u.FeatureData("制限時間")), cdata: "");
            //                        }

            //                        if (u.IsMessageDefined("発進"))
            //                        {
            //                            if (!My.MyProject.Forms.frmMessage.Visible)
            //                            {
            //                                GUI.OpenMessageForm(this, u2: null);
            //                            }

            //                            u.PilotMessage("発進", msg_mode: "");
            //                        }

            //                        // ユニットを配置
            //                        if (Map.MapDataForUnit[tx, ty] is null & u.IsAbleToEnter(tx, ty))
            //                        {
            //                            u.StandBy(tx, ty, "出撃");
            //                        }
            //                        else if (Map.MapDataForUnit[tx2, ty2] is null & u.IsAbleToEnter(tx2, ty2))
            //                        {
            //                            u.StandBy(tx2, ty2, "出撃");
            //                        }
            //                        else
            //                        {
            //                            u.StandBy(x, y, "出撃");
            //                        }

            //                        // ちゃんと配置できた？
            //                        if (u.Status == "待機")
            //                        {
            //                            // 空いた場所がなく出撃出来なかった場合
            //                            GUI.DisplaySysMessage(Nickname + "は" + u.Nickname + "の召喚に失敗した。");
            //                            DeleteServant(u.ID);
            //                            u.Status = "破棄";
            //                        }
            //                    }

            //                    break;
            //                }

            //            case "変身":
            //                {
            //                    // 既に変身している場合は変身出来ない
            //                    if (t.IsFeatureAvailable("ノーマルモード"))
            //                    {
            //                        goto NextLoop;
            //                    }

            //                    buf = t.Name;
            //                    t.Transform(GeneralLib.LIndex(edata, 1));
            //                    t = t.CurrentForm();
            //                    if (elevel2 > 0d)
            //                    {
            //                        t.AddCondition("残り時間", GeneralLib.MaxLng(elevel2, 1), cdata: "");
            //                    }

            //                    var loopTo25 = GeneralLib.LLength(edata);
            //                    for (j = 2; j <= loopTo25; j++)
            //                        buf = buf + " " + GeneralLib.LIndex(edata, j);
            //                    t.AddCondition("ノーマルモード付加", -1, 1d, buf);

            //                    // 変身した場合はそこで終わり
            //                    break;
            //                }

            //            case "能力コピー":
            //                {
            //                    // 既に変身している場合は能力コピー出来ない
            //                    if (IsFeatureAvailable("ノーマルモード"))
            //                    {
            //                        goto NextLoop;
            //                    }

            //                    Transform(t.Name);
            //                    t.Name = argnew_form1;
            //                    {
            //                        var withBlock33 = CurrentForm();
            //                        if (elevel2 > 0d)
            //                        {
            //                            withBlock33.AddCondition("残り時間", GeneralLib.MaxLng(elevel2, 1), cdata: "");
            //                        }

            //                        // 元の形態に戻れるように設定
            //                        buf = Name;
            //                        var loopTo26 = GeneralLib.LLength(edata);
            //                        for (j = 1; j <= loopTo26; j++)
            //                            buf = buf + " " + GeneralLib.LIndex(edata, j);
            //                        withBlock33.AddCondition("ノーマルモード付加", -1, 1d, buf);
            //                        withBlock33.AddCondition("能力コピー", -1, cdata: "");

            //                        // コピー元のパイロット画像とメッセージを使うように設定
            //                        withBlock33.AddCondition("パイロット画像", -1, 0d, "非表示 " + t.MainPilot().get_Bitmap(false));
            //                        withBlock33.AddCondition("メッセージ", -1, 0d, "非表示 " + t.MainPilot().MessageType);
            //                    }

            //                    // 能力コピーした場合はそこで終わり
            //                    ExecuteAbilityRet = true;
            //                    Commands.RestoreSelections();
            //                    return ExecuteAbilityRet;
            //                }

            //            case "再行動":
            //                {
            //                    if (!ReferenceEquals(t, CurrentForm()))
            //                    {
            //                        if (t.Action == 0 & t.MaxAction() > 0)
            //                        {
            //                            if (t.UsedAction > t.MaxAction())
            //                            {
            //                                t.UsedAction = t.MaxAction();
            //                            }

            //                            t.UsedAction = (t.UsedAction - 1);
            //                            GUI.DisplaySysMessage(t.Nickname + "を行動可能にした。");
            //                            is_useful = true;
            //                        }
            //                    }
            //                    else
            //                    {
            //                        t.UsedAction = (t.UsedAction - 1);
            //                    }

            //                    break;
            //                }
            //        }

            //    NextLoop:
            //        ;
            //    }

            //    t.CurrentForm().Update();
            //    t.CurrentForm().CheckAutoHyperMode();
            //    t.CurrentForm().CheckAutoNormalMode();
            //    ExecuteAbilityRet = is_useful;
            //Finish:
            //    ;


            //    // 選択状況を復元
            //    Commands.RestoreSelections();

            //    // マップアビリティの場合、これ以降の処理は必要なし
            //    if (is_map_ability)
            //    {
            //        return ExecuteAbilityRet;
            //    }

            //    // 合体技のパートナーの弾数＆ＥＮの消費
            //    var loopTo27 = Information.UBound(partners);
            //    for (i = 1; i <= loopTo27; i++)
            //    {
            //        {
            //            var withBlock34 = partners[i].CurrentForm();
            //            var loopTo28 = withBlock34.CountAbility();
            //            for (j = 1; j <= loopTo28; j++)
            //            {
            //                // パートナーが同名のアビリティを持っていればそのアビリティのデータを使う
            //                if ((withBlock34.Ability(j).Name ?? "") == (aname ?? ""))
            //                {
            //                    withBlock34.UseAbility(j);
            //                    if (withBlock34.IsAbilityClassifiedAs(j, "自"))
            //                    {
            //                        if (withBlock34.IsFeatureAvailable("パーツ分離"))
            //                        {
            //                            uname = GeneralLib.LIndex(withBlock34.FeatureData(argIndex96), 2);
            //                            Unit localOtherForm() { object argIndex1 = uname; var ret = withBlock34.OtherForm(argIndex1); return ret; }

            //                            if (localOtherForm().IsAbleToEnter(withBlock34.x, withBlock34.y))
            //                            {
            //                                withBlock34.Transform(uname);
            //                                {
            //                                    var withBlock35 = withBlock34.CurrentForm();
            //                                    withBlock35.HP = withBlock35.MaxHP;
            //                                    withBlock35.UsedAction = withBlock35.MaxAction();
            //                                }
            //                            }
            //                            else
            //                            {
            //                                withBlock34.Die();
            //                            }
            //                        }
            //                        else
            //                        {
            //                            withBlock34.Die();
            //                        }
            //                    }
            //                    else if (withBlock34.IsAbilityClassifiedAs(j, "失") & withBlock34.HP == 0)
            //                    {
            //                        withBlock34.Die();
            //                    }
            //                    else if (withBlock34.IsAbilityClassifiedAs(j, "変"))
            //                    {
            //                        if (withBlock34.IsFeatureAvailable("変形技"))
            //                        {
            //                            var loopTo29 = withBlock34.CountFeature();
            //                            for (k = 1; k <= loopTo29; k++)
            //                            {
            //                                string localFeature() { object argIndex1 = k; var ret = withBlock34.Feature(argIndex1); return ret; }

            //                                string localFeatureData1() { object argIndex1 = k; var ret = withBlock34.FeatureData(argIndex1); return ret; }

            //                                string localLIndex2() { string arglist = hsd94f2b67de0b4586a4a3a3d57d84bb20(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //                                if (localFeature() == "変形技" & (localLIndex2() ?? "") == (aname ?? ""))
            //                                {
            //                                    string localFeatureData() { object argIndex1 = k; var ret = withBlock34.FeatureData(argIndex1); return ret; }

            //                                    uname = GeneralLib.LIndex(localFeatureData(), 2);
            //                                    Unit localOtherForm1() { object argIndex1 = uname; var ret = withBlock34.OtherForm(argIndex1); return ret; }

            //                                    if (localOtherForm1().IsAbleToEnter(withBlock34.x, withBlock34.y))
            //                                    {
            //                                        withBlock34.Transform(uname);
            //                                    }

            //                                    break;
            //                                }
            //                            }

            //                            if ((uname ?? "") != (withBlock34.CurrentForm().Name ?? ""))
            //                            {
            //                                if (withBlock34.IsFeatureAvailable("ノーマルモード"))
            //                                {
            //                                    uname = GeneralLib.LIndex(withBlock34.FeatureData(argIndex97), 1);
            //                                    Unit localOtherForm2() { object argIndex1 = uname; var ret = withBlock34.OtherForm(argIndex1); return ret; }

            //                                    if (localOtherForm2().IsAbleToEnter(withBlock34.x, withBlock34.y))
            //                                    {
            //                                        withBlock34.Transform(uname);
            //                                    }
            //                                }
            //                            }
            //                        }
            //                        else if (withBlock34.IsFeatureAvailable("ノーマルモード"))
            //                        {
            //                            uname = GeneralLib.LIndex(withBlock34.FeatureData(argIndex98), 1);
            //                            Unit localOtherForm3() { object argIndex1 = uname; var ret = withBlock34.OtherForm(argIndex1); return ret; }

            //                            if (localOtherForm3().IsAbleToEnter(withBlock34.x, withBlock34.y))
            //                            {
            //                                withBlock34.Transform(uname);
            //                            }
            //                        }
            //                    }

            //                    break;
            //                }
            //            }

            //            // 同名のアビリティがなかった場合は自分のデータを使って処理
            //            if (j > withBlock34.CountAbility())
            //            {
            //                if (this.a.Ability().ENConsumption > 0)
            //                {
            //                    withBlock34.EN = withBlock34.EN - a.AbilityENConsumption();
            //                }

            //                if (a.IsAbilityClassifiedAs("消"))
            //                {
            //                    withBlock34.AddCondition("消耗", 1, cdata: "");
            //                }

            //                if (a.IsAbilityClassifiedAs("Ｃ") & withBlock34.IsConditionSatisfied("チャージ完了"))
            //                {
            //                    withBlock34.DeleteCondition("チャージ完了");
            //                }

            //                if (a.IsAbilityClassifiedAs("気"))
            //                {
            //                    withBlock34.IncreaseMorale((-5 * a.AbilityLevel("気")));
            //                }

            //                if (a.IsAbilityClassifiedAs("霊"))
            //                {
            //                    hp_ratio = 100 * withBlock34.HP / (double)withBlock34.MaxHP;
            //                    en_ratio = 100 * withBlock34.EN / (double)withBlock34.MaxEN;
            //                    withBlock34.MainPilot().Plana = (withBlock34.MainPilot().Plana - 5d * a.AbilityLevel("霊"));
            //                    withBlock34.HP = (withBlock34.MaxHP * hp_ratio / 100d);
            //                    withBlock34.EN = (withBlock34.MaxEN * en_ratio / 100d);
            //                }
            //                else if (a.IsAbilityClassifiedAs("プ"))
            //                {
            //                    hp_ratio = 100 * withBlock34.HP / (double)withBlock34.MaxHP;
            //                    en_ratio = 100 * withBlock34.EN / (double)withBlock34.MaxEN;
            //                    withBlock34.MainPilot().Plana = (withBlock34.MainPilot().Plana - 5d * a.AbilityLevel("プ"));
            //                    withBlock34.HP = (withBlock34.MaxHP * hp_ratio / 100d);
            //                    withBlock34.EN = (withBlock34.MaxEN * en_ratio / 100d);
            //                }

            //                if (a.IsAbilityClassifiedAs("失"))
            //                {
            //                    withBlock34.HP = GeneralLib.MaxLng((withBlock34.HP - (long)(withBlock34.MaxHP * a.AbilityLevel("失")) / 10L), 0);
            //                }

            //                if (a.IsAbilityClassifiedAs("自"))
            //                {
            //                    if (withBlock34.IsFeatureAvailable("パーツ分離"))
            //                    {
            //                        uname = GeneralLib.LIndex(withBlock34.FeatureData(argIndex101), 2);
            //                        Unit localOtherForm4() { object argIndex1 = uname; var ret = withBlock34.OtherForm(argIndex1); return ret; }

            //                        if (localOtherForm4().IsAbleToEnter(withBlock34.x, withBlock34.y))
            //                        {
            //                            withBlock34.Transform(uname);
            //                            {
            //                                var withBlock36 = withBlock34.CurrentForm();
            //                                withBlock36.HP = withBlock36.MaxHP;
            //                                withBlock36.UsedAction = withBlock36.MaxAction();
            //                            }
            //                        }
            //                        else
            //                        {
            //                            withBlock34.Die();
            //                        }
            //                    }
            //                    else
            //                    {
            //                        withBlock34.Die();
            //                    }
            //                }
            //                else if (a.IsAbilityClassifiedAs("失") & withBlock34.HP == 0)
            //                {
            //                    withBlock34.Die();
            //                }
            //                else if (a.IsAbilityClassifiedAs("変"))
            //                {
            //                    if (withBlock34.IsFeatureAvailable("ノーマルモード"))
            //                    {
            //                        uname = GeneralLib.LIndex(withBlock34.FeatureData("ノーマルモード"), 1);
            //                        Unit localOtherForm5() { object argIndex1 = uname; var ret = withBlock34.OtherForm(argIndex1); return ret; }

            //                        if (localOtherForm5().IsAbleToEnter(withBlock34.x, withBlock34.y))
            //                        {
            //                            withBlock34.Transform(uname);
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // 変身した場合
            //    if (Status == "他形態")
            //    {
            //        {
            //            var withBlock37 = CurrentForm();
            //            // 使い捨てアイテムによる変身の処理
            //            var loopTo30 = withBlock37.CountAbility();
            //            for (i = 1; i <= loopTo30; i++)
            //            {
            //                if ((withBlock37.Ability(i).Name ?? "") == (aname ?? ""))
            //                {
            //                    // アイテムを消費
            //                    if (withBlock37.Ability(i).IsItem() & withBlock37.Stock(i) == 0 & withBlock37.MaxStock(i) > 0)
            //                    {
            //                        var loopTo31 = withBlock37.CountItem();
            //                        for (j = 1; j <= loopTo31; j++)
            //                        {
            //                            Item localItem5() { object argIndex1 = j; var ret = withBlock37.Item(argIndex1); return ret; }

            //                            var loopTo32 = localItem5().CountAbility();
            //                            for (k = 1; k <= loopTo32; k++)
            //                            {
            //                                Item localItem4() { object argIndex1 = j; var ret = withBlock37.Item(argIndex1); return ret; }

            //                                AbilityData localAbility() { object argIndex1 = k; var ret = hs8bdb16b7368640769bb5144024b221c0().Ability(argIndex1); return ret; }

            //                                if ((localAbility().Name ?? "") == (aname ?? ""))
            //                                {
            //                                    Item localItem3() { object argIndex1 = j; var ret = withBlock37.Item(argIndex1); return ret; }

            //                                    localItem3().Exist = false;
            //                                    withBlock37.DeleteItem(j);
            //                                    withBlock37.Update();
            //                                    goto ExitLoop;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //        ExitLoop:
            //            ;


            //            // 自殺？
            //            if (withBlock37.HP == 0)
            //            {
            //                withBlock37.Die();
            //            }
            //        }

            //        // WaitCommandによる画面クリアが行われないので
            //        GUI.RedrawScreen();
            //        return ExecuteAbilityRet;
            //    }

            //    // 経験値の獲得
            //    if (is_useful & !is_event & !Expression.IsOptionDefined("アビリティ経験値無効"))
            //    {
            //        GetExp(t, "アビリティ", exp_mode: "");
            //        if (!Expression.IsOptionDefined("合体技パートナー経験値無効"))
            //        {
            //            var loopTo33 = Information.UBound(partners);
            //            for (i = 1; i <= loopTo33; i++)
            //            {
            //                partners[i].CurrentForm().GetExp(t, "アビリティ", "パートナー");
            //            }
            //        }
            //    }

            //    // 以下の効果はアビリティデータが変化する場合があるため同時には適応されない

            //    // 自爆技

            //    // ＨＰ消費アビリティで自殺

            //    // 変形技
            //    if (a.IsAbilityClassifiedAs("自"))
            //    {
            //        if (IsFeatureAvailable("パーツ分離"))
            //        {
            //            uname = GeneralLib.LIndex(FeatureData(argIndex104), 2);
            //            Unit localOtherForm6() { object argIndex1 = uname; var ret = OtherForm(argIndex1); return ret; }

            //            if (localOtherForm6().IsAbleToEnter(x, y))
            //            {
            //                Transform(uname);
            //                {
            //                    var withBlock38 = CurrentForm();
            //                    withBlock38.HP = withBlock38.MaxHP;
            //                    withBlock38.UsedAction = withBlock38.MaxAction();
            //                }

            //                fname = FeatureName("パーツ分離");
            //                bool localIsSysMessageDefined() { string argmain_situation = "破壊時分離(" + fname + ")"; string argsub_situation = ""; var ret = IsSysMessageDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //                bool localIsSysMessageDefined1() { string argmain_situation = "分離(" + Name + ")"; string argsub_situation = ""; var ret = IsSysMessageDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //                bool localIsSysMessageDefined2() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = IsSysMessageDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //                if (IsSysMessageDefined("破壊時分離(" + Name + ")", sub_situation: ""))
            //                {
            //                    SysMessage("破壊時分離(" + Name + ")", sub_situation: "", add_msg: "");
            //                }
            //                else if (localIsSysMessageDefined())
            //                {
            //                    SysMessage("破壊時分離(" + fname + ")", sub_situation: "", add_msg: "");
            //                }
            //                else if (IsSysMessageDefined("破壊時分離", sub_situation: ""))
            //                {
            //                    SysMessage("破壊時分離", sub_situation: "", add_msg: "");
            //                }
            //                else if (localIsSysMessageDefined1())
            //                {
            //                    SysMessage("分離(" + Name + ")", sub_situation: "", add_msg: "");
            //                }
            //                else if (localIsSysMessageDefined2())
            //                {
            //                    SysMessage("分離(" + fname + ")", sub_situation: "", add_msg: "");
            //                }
            //                else if (IsSysMessageDefined("分離", sub_situation: ""))
            //                {
            //                    SysMessage("分離", sub_situation: "", add_msg: "");
            //                }
            //                else
            //                {
            //                    GUI.DisplaySysMessage(Nickname + "は破壊されたパーツを分離させた。");
            //                }
            //            }
            //            else
            //            {
            //                Die();
            //            }
            //        }
            //        else
            //        {
            //            Die();
            //        }
            //    }
            //    else if (a.IsAbilityClassifiedAs("失") & HP == 0)
            //    {
            //        Die();
            //    }
            //    else if (a.IsAbilityClassifiedAs("変"))
            //    {
            //        if (IsFeatureAvailable("変形技"))
            //        {
            //            var loopTo34 = CountFeature();
            //            for (i = 1; i <= loopTo34; i++)
            //            {
            //                string localFeature1() { object argIndex1 = i; var ret = Feature(argIndex1); return ret; }

            //                string localFeatureData3() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //                string localLIndex3() { string arglist = hs943d006232364b899ee9a8aea8dcca5a(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //                if (localFeature1() == "変形技" & (localLIndex3() ?? "") == (a.Ability().Name ?? ""))
            //                {
            //                    string localFeatureData2() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //                    uname = GeneralLib.LIndex(localFeatureData2(), 2);
            //                    Unit localOtherForm7() { object argIndex1 = uname; var ret = OtherForm(argIndex1); return ret; }

            //                    if (localOtherForm7().IsAbleToEnter(x, y))
            //                    {
            //                        Transform(uname);
            //                    }

            //                    break;
            //                }
            //            }

            //            if ((uname ?? "") != (CurrentForm().Name ?? ""))
            //            {
            //                if (IsFeatureAvailable("ノーマルモード"))
            //                {
            //                    uname = GeneralLib.LIndex(FeatureData(argIndex106), 1);
            //                    Unit localOtherForm8() { object argIndex1 = uname; var ret = OtherForm(argIndex1); return ret; }

            //                    if (localOtherForm8().IsAbleToEnter(x, y))
            //                    {
            //                        Transform(uname);
            //                    }
            //                }
            //            }
            //        }
            //        else if (IsFeatureAvailable("ノーマルモード"))
            //        {
            //            uname = GeneralLib.LIndex(FeatureData(argIndex107), 1);
            //            Unit localOtherForm9() { object argIndex1 = uname; var ret = OtherForm(argIndex1); return ret; }

            //            if (localOtherForm9().IsAbleToEnter(x, y))
            //            {
            //                Transform(uname);
            //            }
            //        }
            //    }

            //    // アイテムを消費
            //    else if (a.Ability().IsItem() & a.Stock() == 0 & a.MaxStock() > 0)
            //    {
            //        // アイテムを削除
            //        num = Data.CountAbility();
            //        num = (num + MainPilot().Data.CountAbility());
            //        var loopTo35 = CountPilot();
            //        for (i = 2; i <= loopTo35; i++)
            //        {
            //            Pilot localPilot() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

            //            num = (num + localPilot().Data.CountAbility());
            //        }

            //        var loopTo36 = CountSupport();
            //        for (i = 2; i <= loopTo36; i++)
            //        {
            //            Pilot localSupport() { object argIndex1 = i; var ret = Support(argIndex1); return ret; }

            //            num = (num + localSupport().Data.CountAbility());
            //        }

            //        if (IsFeatureAvailable("追加サポート"))
            //        {
            //            num = (num + AdditionalSupport().Data.CountAbility());
            //        }

            //        foreach (Item itm in colItem)
            //        {
            //            num = (num + itm.CountAbility());
            //            if (a <= num)
            //            {
            //                itm.Exist = false;
            //                DeleteItem((object)itm.ID);
            //                break;
            //            }
            //        }
            //    }

            //    // ADD START MARGE
            //    // 戦闘アニメ終了処理
            //    if (IsAnimationDefined(aname + "(終了)", sub_situation: ""))
            //    {
            //        PlayAnimation(aname + "(終了)", sub_situation: "");
            //    }
            //    else if (IsAnimationDefined("終了", sub_situation: ""))
            //    {
            //        PlayAnimation("終了", sub_situation: "");
            //    }
            //    // ADD END MARGE

            //    {
            //        var withBlock39 = CurrentForm();
            //        // 戦闘アニメで変更されたユニット画像を元に戻す
            //        if (withBlock39.IsConditionSatisfied("ユニット画像"))
            //        {
            //            withBlock39.DeleteCondition("ユニット画像");
            //            withBlock39.BitmapID = GUI.MakeUnitBitmap(CurrentForm());
            //            GUI.PaintUnitBitmap(CurrentForm());
            //        }

            //        if (withBlock39.IsConditionSatisfied("非表示付加"))
            //        {
            //            withBlock39.DeleteCondition("非表示付加");
            //            withBlock39.BitmapID = GUI.MakeUnitBitmap(CurrentForm());
            //            GUI.PaintUnitBitmap(CurrentForm());
            //        }
            //    }

            //    var loopTo37 = Information.UBound(partners);
            //    for (i = 1; i <= loopTo37; i++)
            //    {
            //        {
            //            var withBlock40 = partners[i].CurrentForm();
            //            if (withBlock40.IsConditionSatisfied("ユニット画像"))
            //            {
            //                withBlock40.DeleteCondition("ユニット画像");
            //                withBlock40.BitmapID = GUI.MakeUnitBitmap(partners[i].CurrentForm());
            //                GUI.PaintUnitBitmap(partners[i].CurrentForm());
            //            }

            //            if (withBlock40.IsConditionSatisfied("非表示付加"))
            //            {
            //                withBlock40.DeleteCondition("非表示付加");
            //                withBlock40.BitmapID = GUI.MakeUnitBitmap(partners[i].CurrentForm());
            //                GUI.PaintUnitBitmap(partners[i].CurrentForm());
            //            }
            //        }
            //    }

            //    return ExecuteAbilityRet;
        }

        // マップアビリティ a を (tx,ty) に使用
        public void ExecuteMapAbility(UnitAbility a, int tx, int ty, bool is_event = false)
        {
            throw new NotImplementedException();
            //    int k, i, j, num;
            //    Unit t, max_lv_t;
            //    Unit[] targets;
            //    var partners = default(Unit[]);
            //    var is_useful = default(bool);
            //    string anickname, aname, msg;
            //    int min_range, max_range;
            //    int rx, ry;
            //    string uname = default, fname;
            //    double hp_ratio, en_ratio;
            //    aname = a.Ability().Name;
            //    anickname = a.AbilityNickname();
            //    if (!is_event)
            //    {
            //        // マップ攻撃の使用イベント
            //        Event.HandleEvent("使用", MainPilot().ID, aname);
            //        if (SRC.IsScenarioFinished)
            //        {
            //            return;
            //        }

            //        if (SRC.IsCanceled)
            //        {
            //            SRC.IsCanceled = false;
            //            return;
            //        }
            //    }

            //    // 効果範囲を設定
            //    min_range = a.AbilityMinRange();
            //    max_range = a.AbilityMaxRange();
            //    if (a.IsAbilityClassifiedAs("Ｍ直"))
            //    {
            //        if (ty < y)
            //        {
            //            Map.AreaInLine(x, y, min_range, max_range, "N");
            //        }
            //        else if (ty > y)
            //        {
            //            Map.AreaInLine(x, y, min_range, max_range, "S");
            //        }
            //        else if (tx < x)
            //        {
            //            Map.AreaInLine(x, y, min_range, max_range, "W");
            //        }
            //        else
            //        {
            //            Map.AreaInLine(x, y, min_range, max_range, "E");
            //        }
            //    }
            //    else if (a.IsAbilityClassifiedAs("Ｍ拡"))
            //    {
            //        if (ty < y & Math.Abs((y - ty)) > Math.Abs((x - tx)))
            //        {
            //            Map.AreaInCone(x, y, min_range, max_range, "N");
            //        }
            //        else if (ty > y & Math.Abs((y - ty)) > Math.Abs((x - tx)))
            //        {
            //            Map.AreaInCone(x, y, min_range, max_range, "S");
            //        }
            //        else if (tx < x & Math.Abs((x - tx)) > Math.Abs((y - ty)))
            //        {
            //            Map.AreaInCone(x, y, min_range, max_range, "W");
            //        }
            //        else
            //        {
            //            Map.AreaInCone(x, y, min_range, max_range, "E");
            //        }
            //    }
            //    else if (a.IsAbilityClassifiedAs("Ｍ扇"))
            //    {
            //        if (ty < y & Math.Abs((y - ty)) >= Math.Abs((x - tx)))
            //        {
            //            Map.AreaInSector(x, y, min_range, max_range, "N", a.AbilityLevel("Ｍ扇"));
            //        }
            //        else if (ty > y & Math.Abs((y - ty)) >= Math.Abs((x - tx)))
            //        {
            //            Map.AreaInSector(x, y, min_range, max_range, "S", a.AbilityLevel("Ｍ扇"));
            //        }
            //        else if (tx < x & Math.Abs((x - tx)) >= Math.Abs((y - ty)))
            //        {
            //            Map.AreaInSector(x, y, min_range, max_range, "W", a.AbilityLevel("Ｍ扇"));
            //        }
            //        else
            //        {
            //            Map.AreaInSector(x, y, min_range, max_range, "E", a.AbilityLevel("Ｍ扇"));
            //        }
            //    }
            //    else if (a.IsAbilityClassifiedAs("Ｍ投"))
            //    {
            //        Map.AreaInRange(tx, ty, a.AbilityLevel("Ｍ投"), 1, "すべて");
            //    }
            //    else if (a.IsAbilityClassifiedAs("Ｍ全"))
            //    {
            //        Map.AreaInRange(x, y, max_range, min_range, "すべて");
            //    }
            //    else if (a.IsAbilityClassifiedAs("Ｍ移") | a.IsAbilityClassifiedAs("Ｍ線"))
            //    {
            //        Map.AreaInPointToPoint(x, y, tx, ty);
            //    }

            //    // ユニットがいるマスの処理
            //    var loopTo = Map.MapWidth;
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        var loopTo1 = Map.MapHeight;
            //        for (j = 1; j <= loopTo1; j++)
            //        {
            //            if (!Map.MaskData[i, j])
            //            {
            //                t = Map.MapDataForUnit[i, j];
            //                if (t is object)
            //                {
            //                    // 有効？
            //                    if (a.IsAbilityEffective(t))
            //                    {
            //                        Map.MaskData[i, j] = false;
            //                    }
            //                    else
            //                    {
            //                        Map.MaskData[i, j] = true;
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // 支援専用アビリティは自分には使用できない
            //    if (a.IsAbilityClassifiedAs("援"))
            //    {
            //        Map.MaskData[x, y] = true;
            //    }

            //    // マップアビリティの影響を受けるユニットのリストを作成
            //    targets = new Unit[1];
            //    var loopTo2 = Map.MapWidth;
            //    for (i = 1; i <= loopTo2; i++)
            //    {
            //        var loopTo3 = Map.MapHeight;
            //        for (j = 1; j <= loopTo3; j++)
            //        {
            //            // マップアビリティの影響をうけるかチェック
            //            if (Map.MaskData[i, j])
            //            {
            //                goto NextLoop;
            //            }

            //            t = Map.MapDataForUnit[i, j];
            //            if (t is null)
            //            {
            //                goto NextLoop;
            //            }

            //            if (!a.IsAbilityApplicable(t))
            //            {
            //                Map.MaskData[i, j] = true;
            //                goto NextLoop;
            //            }

            //            Array.Resize(targets, Information.UBound(targets) + 1 + 1);
            //            targets[Information.UBound(targets)] = t;
            //        NextLoop:
            //            ;
            //        }
            //    }

            //    // アビリティ実行の起点を設定
            //    if (a.IsAbilityClassifiedAs("Ｍ投"))
            //    {
            //        rx = tx;
            //        ry = ty;
            //    }
            //    else
            //    {
            //        rx = x;
            //        ry = y;
            //    }

            //    // 起点からの距離に応じて並べ替え
            //    int min_item, min_value;
            //    var loopTo4 = (Information.UBound(targets) - 1);
            //    for (i = 1; i <= loopTo4; i++)
            //    {
            //        min_item = i;
            //        {
            //            var withBlock = targets[i];
            //            min_value = (Math.Abs((withBlock.x - rx)) + Math.Abs((withBlock.y - ry)));
            //        }

            //        var loopTo5 = Information.UBound(targets);
            //        for (j = (i + 1); j <= loopTo5; j++)
            //        {
            //            {
            //                var withBlock1 = targets[j];
            //                if ((Math.Abs((withBlock1.x - rx)) + Math.Abs((withBlock1.y - ry))) < min_value)
            //                {
            //                    min_item = j;
            //                    min_value = (Math.Abs((withBlock1.x - rx)) + Math.Abs((withBlock1.y - ry)));
            //                }
            //            }
            //        }

            //        if (min_item != i)
            //        {
            //            t = targets[i];
            //            targets[i] = targets[min_item];
            //            targets[min_item] = t;
            //        }
            //    }

            //    // 合体技
            //    bool[] TmpMaskData;
            //    if (a.IsAbilityClassifiedAs("合"))
            //    {

            //        // 合体技のパートナーのハイライト表示
            //        // MaskDataを保存して使用している
            //        TmpMaskData = new bool[(Map.MapWidth + 1), (Map.MapHeight + 1)];
            //        var loopTo6 = Map.MapWidth;
            //        for (i = 1; i <= loopTo6; i++)
            //        {
            //            var loopTo7 = Map.MapHeight;
            //            for (j = 1; j <= loopTo7; j++)
            //                TmpMaskData[i, j] = Map.MaskData[i, j];
            //        }

            //        CombinationPartner("アビリティ", a, partners);

            //        // パートナーユニットはマスクを解除
            //        var loopTo8 = Information.UBound(partners);
            //        for (i = 1; i <= loopTo8; i++)
            //        {
            //            {
            //                var withBlock2 = partners[i];
            //                Map.MaskData[withBlock2.x, withBlock2.y] = false;
            //                TmpMaskData[withBlock2.x, withBlock2.y] = true;
            //            }
            //        }

            //        GUI.MaskScreen();

            //        // マスクを復元
            //        var loopTo9 = Map.MapWidth;
            //        for (i = 1; i <= loopTo9; i++)
            //        {
            //            var loopTo10 = Map.MapHeight;
            //            for (j = 1; j <= loopTo10; j++)
            //                Map.MaskData[i, j] = TmpMaskData[i, j];
            //        }
            //    }
            //    else
            //    {
            //        partners = new Unit[1];
            //        Commands.SelectedPartners.Clear();
            //        GUI.MaskScreen();
            //    }

            //    GUI.OpenMessageForm(this, u2: null);

            //    // 現在の選択状況をセーブ
            //    Commands.SaveSelections();

            //    // 選択内容を切り替え
            //    Commands.SelectedUnit = this;
            //    Event.SelectedUnitForEvent = this;
            //    Commands.SelectedAbility = a;
            //    Commands.SelectedAbilityName = a.Ability().Name;
            //    Commands.SelectedX = tx;
            //    Commands.SelectedY = ty;

            //    // 変な「対～」メッセージが表示されないようにターゲットをオフ
            //    // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    Commands.SelectedTarget = null;
            //    // UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    Event.SelectedTargetForEvent = null;

            //    // マップアビリティ開始のメッセージ＆特殊効果
            //    if (IsAnimationDefined(aname + "(準備)", sub_situation: ""))
            //    {
            //        PlayAnimation(aname + "(準備)", sub_situation: "");
            //    }

            //    if (IsMessageDefined("かけ声(" + aname + ")"))
            //    {
            //        PilotMessage("かけ声(" + aname + ")", msg_mode: "");
            //    }

            //    PilotMessage(aname, "アビリティ");
            //    if (IsAnimationDefined(aname + "(使用)", sub_situation: ""))
            //    {
            //        PlayAnimation(aname + "(使用)", "", true);
            //    }
            //    else
            //    {
            //        SpecialEffect(aname, "", true);
            //    }

            //    // ＥＮ消費＆使用回数減少
            //    a.UseAbility();
            //    GUI.UpdateMessageForm(this, u2: null);
            //    switch (Information.UBound(partners))
            //    {
            //        case 0:
            //            {
            //                // 通常
            //                msg = Nickname + "は";
            //                break;
            //            }

            //        case 1:
            //            {
            //                // ２体合体
            //                if ((Nickname ?? "") != (partners[0].Nickname ?? ""))
            //                {
            //                    msg = Nickname + "は[" + partners[0].Nickname + "]と共に";
            //                }
            //                else if ((MainPilot().get_Nickname(false) ?? "") != (partners[0].MainPilot().get_Nickname(false) ?? ""))
            //                {
            //                    msg = MainPilot().get_Nickname(false) + "と[" + partners[0].MainPilot().get_Nickname(false) + "]の[" + Nickname + "]は";
            //                }
            //                else
            //                {
            //                    msg = Nickname + "達は";
            //                }

            //                break;
            //            }

            //        case 2:
            //            {
            //                // ３体合体
            //                if ((Nickname ?? "") != (partners[0].Nickname ?? ""))
            //                {
            //                    msg = Nickname + "は[" + partners[0].Nickname + "]、[" + partners[1].Nickname + "]と共に";
            //                }
            //                else if ((MainPilot().get_Nickname(false) ?? "") != (partners[0].MainPilot().get_Nickname(false) ?? ""))
            //                {
            //                    msg = MainPilot().get_Nickname(false) + "、[" + partners[0].MainPilot().get_Nickname(false) + "]、[" + partners[1].MainPilot().get_Nickname(false) + "]の[" + Nickname + "]は";
            //                }
            //                else
            //                {
            //                    msg = Nickname + "達は";
            //                }

            //                break;
            //            }

            //        default:
            //            {
            //                // ３体以上
            //                msg = Nickname + "達は";
            //                break;
            //            }
            //    }

            //    if (a.IsSpellAbility())
            //    {
            //        if (Strings.Right(anickname, 2) == "呪文")
            //        {
            //            msg = msg + "[" + anickname + "]を唱えた。";
            //        }
            //        else if (Strings.Right(anickname, 2) == "の杖")
            //        {
            //            msg = msg + "[" + Strings.Left(anickname, Strings.Len(anickname) - 2) + "]の呪文を唱えた。";
            //        }
            //        else
            //        {
            //            msg = msg + "[" + anickname + "]の呪文を唱えた。";
            //        }
            //    }
            //    else if (Strings.Right(anickname, 1) == "歌")
            //    {
            //        msg = msg + "[" + anickname + "]を歌った。";
            //    }
            //    else if (Strings.Right(anickname, 2) == "踊り")
            //    {
            //        msg = msg + "[" + anickname + "]を踊った。";
            //    }
            //    else
            //    {
            //        msg = msg + "[" + anickname + "]を使った。";
            //    }

            //    if (IsSysMessageDefined(aname, sub_situation: ""))
            //    {
            //        // 「アビリティ名(解説)」のメッセージを使用
            //        SysMessage(aname, sub_situation: "", add_msg: "");
            //    }
            //    else if (IsSysMessageDefined("アビリティ", sub_situation: ""))
            //    {
            //        // 「アビリティ(解説)」のメッセージを使用
            //        SysMessage("アビリティ", sub_situation: "", add_msg: "");
            //    }
            //    else
            //    {
            //        GUI.DisplaySysMessage(msg);
            //    }

            //    // 選択状況を復元
            //    Commands.RestoreSelections();

            //    // アビリティの使用に失敗？
            //    if (GeneralLib.Dice(10) <= a.AbilityLevel("難"))
            //    {
            //        GUI.DisplaySysMessage("しかし何もおきなかった…");
            //        goto Finish;
            //    }

            //    // 使用元ユニットは SelectedTarget に設定していないといけない
            //    Commands.SelectedTarget = this;

            //    // 各ユニットにアビリティを使用
            //    // UPGRADE_NOTE: オブジェクト max_lv_t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    max_lv_t = null;
            //    var loopTo11 = Information.UBound(targets);
            //    for (i = 1; i <= loopTo11; i++)
            //    {
            //        t = targets[i].CurrentForm();
            //        if (t.Status == "出撃")
            //        {
            //            if (ReferenceEquals(t, this))
            //            {
            //                GUI.UpdateMessageForm(this, u2: null);
            //            }
            //            else
            //            {
            //                GUI.UpdateMessageForm(t, this);
            //            }

            //            if (a.ExecuteAbility(t, true))
            //            {
            //                t = t.CurrentForm();
            //                is_useful = true;

            //                // 獲得経験値算出用にメインパイロットのレベルが最も高い
            //                // ユニットを求めておく
            //                if (max_lv_t is null)
            //                {
            //                    max_lv_t = t;
            //                }
            //                else if (t.MainPilot().Level > max_lv_t.MainPilot().Level)
            //                {
            //                    max_lv_t = t;
            //                }
            //            }
            //        }
            //    }

            //    // ADD START MARGE
            //    // 戦闘アニメ終了処理
            //    if (IsAnimationDefined(aname + "(終了)", sub_situation: ""))
            //    {
            //        PlayAnimation(aname + "(終了)", sub_situation: "");
            //    }
            //    else if (IsAnimationDefined("終了", sub_situation: ""))
            //    {
            //        PlayAnimation("終了", sub_situation: "");
            //    }
            //    // ADD END MARGE

            //    {
            //        var withBlock3 = CurrentForm();
            //        // 戦闘アニメで変更されたユニット画像を元に戻す
            //        if (withBlock3.IsConditionSatisfied("ユニット画像"))
            //        {
            //            withBlock3.DeleteCondition("ユニット画像");
            //            withBlock3.BitmapID = GUI.MakeUnitBitmap(CurrentForm());
            //            GUI.PaintUnitBitmap(CurrentForm());
            //        }

            //        if (withBlock3.IsConditionSatisfied("非表示付加"))
            //        {
            //            withBlock3.DeleteCondition("非表示付加");
            //            withBlock3.BitmapID = GUI.MakeUnitBitmap(CurrentForm());
            //            GUI.PaintUnitBitmap(CurrentForm());
            //        }
            //    }

            //    var loopTo12 = Information.UBound(partners);
            //    for (i = 1; i <= loopTo12; i++)
            //    {
            //        {
            //            var withBlock4 = partners[i].CurrentForm();
            //            if (withBlock4.IsConditionSatisfied("ユニット画像"))
            //            {
            //                withBlock4.DeleteCondition("ユニット画像");
            //                withBlock4.BitmapID = GUI.MakeUnitBitmap(partners[i].CurrentForm());
            //                GUI.PaintUnitBitmap(partners[i].CurrentForm());
            //            }

            //            if (withBlock4.IsConditionSatisfied("非表示付加"))
            //            {
            //                withBlock4.DeleteCondition("非表示付加");
            //                withBlock4.BitmapID = GUI.MakeUnitBitmap(partners[i].CurrentForm());
            //                GUI.PaintUnitBitmap(partners[i].CurrentForm());
            //            }
            //        }
            //    }

            //    // 獲得した経験値の表示
            //    if (is_useful & !is_event & !Expression.IsOptionDefined("アビリティ経験値無効"))
            //    {
            //        GetExp(max_lv_t, "アビリティ", exp_mode: "");
            //        if (!Expression.IsOptionDefined("合体技パートナー経験値無効"))
            //        {
            //            var loopTo13 = Information.UBound(partners);
            //            for (i = 1; i <= loopTo13; i++)
            //            {
            //                partners[i].CurrentForm().GetExp(null, "アビリティ", "パートナー");
            //            }
            //        }
            //    }

            //    // 合体技のパートナーの弾数＆ＥＮの消費
            //    var loopTo14 = Information.UBound(partners);
            //    for (i = 1; i <= loopTo14; i++)
            //    {
            //        {
            //            var withBlock5 = partners[i].CurrentForm();
            //            var loopTo15 = withBlock5.CountAbility();
            //            for (j = 1; j <= loopTo15; j++)
            //            {
            //                // パートナーが同名のアビリティを持っていればそのアビリティのデータを使う
            //                if ((withBlock5.Ability(j).Name ?? "") == (aname ?? ""))
            //                {
            //                    withBlock5.UseAbility(j);
            //                    if (withBlock5.IsAbilityClassifiedAs(j, "自"))
            //                    {
            //                        if (withBlock5.IsFeatureAvailable("パーツ分離"))
            //                        {
            //                            uname = GeneralLib.LIndex(withBlock5.FeatureData("パーツ分離"), 2);
            //                            Unit localOtherForm() { object argIndex1 = uname; var ret = withBlock5.OtherForm(argIndex1); return ret; }

            //                            if (localOtherForm().IsAbleToEnter(withBlock5.x, withBlock5.y))
            //                            {
            //                                withBlock5.Transform(uname);
            //                                {
            //                                    var withBlock6 = withBlock5.CurrentForm();
            //                                    withBlock6.HP = withBlock6.MaxHP;
            //                                    withBlock6.UsedAction = withBlock6.MaxAction();
            //                                }
            //                            }
            //                            else
            //                            {
            //                                withBlock5.Die();
            //                            }
            //                        }
            //                        else
            //                        {
            //                            withBlock5.Die();
            //                        }
            //                    }
            //                    else if (withBlock5.IsAbilityClassifiedAs(j, "失") & withBlock5.HP == 0)
            //                    {
            //                        withBlock5.Die();
            //                    }
            //                    else if (withBlock5.IsAbilityClassifiedAs(j, "変"))
            //                    {
            //                        if (withBlock5.IsFeatureAvailable("変形技"))
            //                        {
            //                            var loopTo16 = withBlock5.CountFeature();
            //                            for (k = 1; k <= loopTo16; k++)
            //                            {
            //                                string localFeature() { object argIndex1 = k; var ret = withBlock5.Feature(argIndex1); return ret; }

            //                                string localFeatureData1() { object argIndex1 = k; var ret = withBlock5.FeatureData(argIndex1); return ret; }

            //                                string localLIndex() { string arglist = hsa17e1f441163458982d95695a4abb266(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //                                if (localFeature() == "変形技" & (localLIndex() ?? "") == (aname ?? ""))
            //                                {
            //                                    string localFeatureData() { object argIndex1 = k; var ret = withBlock5.FeatureData(argIndex1); return ret; }

            //                                    uname = GeneralLib.LIndex(localFeatureData(), 2);
            //                                    Unit localOtherForm1() { object argIndex1 = uname; var ret = withBlock5.OtherForm(argIndex1); return ret; }

            //                                    if (localOtherForm1().IsAbleToEnter(withBlock5.x, withBlock5.y))
            //                                    {
            //                                        withBlock5.Transform(uname);
            //                                    }

            //                                    break;
            //                                }
            //                            }

            //                            if ((uname ?? "") != (withBlock5.CurrentForm().Name ?? ""))
            //                            {
            //                                if (withBlock5.IsFeatureAvailable("ノーマルモード"))
            //                                {
            //                                    uname = GeneralLib.LIndex(withBlock5.FeatureData(argIndex10), 1);
            //                                    Unit localOtherForm2() { object argIndex1 = uname; var ret = withBlock5.OtherForm(argIndex1); return ret; }

            //                                    if (localOtherForm2().IsAbleToEnter(withBlock5.x, withBlock5.y))
            //                                    {
            //                                        withBlock5.Transform(uname);
            //                                    }
            //                                }
            //                            }
            //                        }
            //                        else if (withBlock5.IsFeatureAvailable("ノーマルモード"))
            //                        {
            //                            uname = GeneralLib.LIndex(withBlock5.FeatureData(argIndex11), 1);
            //                            Unit localOtherForm3() { object argIndex1 = uname; var ret = withBlock5.OtherForm(argIndex1); return ret; }

            //                            if (localOtherForm3().IsAbleToEnter(withBlock5.x, withBlock5.y))
            //                            {
            //                                withBlock5.Transform(uname);
            //                            }
            //                        }
            //                    }

            //                    break;
            //                }
            //            }

            //            // 同名のアビリティがなかった場合は自分のデータを使って処理
            //            if (j > withBlock5.CountAbility())
            //            {
            //                if (this.a.Ability().ENConsumption > 0)
            //                {
            //                    withBlock5.EN = withBlock5.EN - a.AbilityENConsumption();
            //                }

            //                if (a.IsAbilityClassifiedAs("消"))
            //                {
            //                    withBlock5.AddCondition("消耗", 1, cdata: "");
            //                }

            //                if (a.IsAbilityClassifiedAs("Ｃ") & withBlock5.IsConditionSatisfied("チャージ完了"))
            //                {
            //                    withBlock5.DeleteCondition("チャージ完了");
            //                }

            //                if (a.IsAbilityClassifiedAs("気"))
            //                {
            //                    withBlock5.IncreaseMorale((-5 * a.AbilityLevel("気")));
            //                }

            //                if (a.IsAbilityClassifiedAs("霊"))
            //                {
            //                    hp_ratio = 100 * withBlock5.HP / (double)withBlock5.MaxHP;
            //                    en_ratio = 100 * withBlock5.EN / (double)withBlock5.MaxEN;
            //                    withBlock5.MainPilot().Plana = (withBlock5.MainPilot().Plana - 5d * a.AbilityLevel("霊"));
            //                    withBlock5.HP = (withBlock5.MaxHP * hp_ratio / 100d);
            //                    withBlock5.EN = (withBlock5.MaxEN * en_ratio / 100d);
            //                }
            //                else if (a.IsAbilityClassifiedAs("プ"))
            //                {
            //                    hp_ratio = 100 * withBlock5.HP / (double)withBlock5.MaxHP;
            //                    en_ratio = 100 * withBlock5.EN / (double)withBlock5.MaxEN;
            //                    withBlock5.MainPilot().Plana = (withBlock5.MainPilot().Plana - 5d * a.AbilityLevel("プ"));
            //                    withBlock5.HP = (withBlock5.MaxHP * hp_ratio / 100d);
            //                    withBlock5.EN = (withBlock5.MaxEN * en_ratio / 100d);
            //                }

            //                if (a.IsAbilityClassifiedAs("失"))
            //                {
            //                    withBlock5.HP = GeneralLib.MaxLng((withBlock5.HP - (long)(withBlock5.MaxHP * a.AbilityLevel("失")) / 10L), 0);
            //                }

            //                if (a.IsAbilityClassifiedAs("自"))
            //                {
            //                    if (withBlock5.IsFeatureAvailable("パーツ分離"))
            //                    {
            //                        uname = GeneralLib.LIndex(withBlock5.FeatureData(argIndex14), 2);
            //                        Unit localOtherForm4() { object argIndex1 = uname; var ret = withBlock5.OtherForm(argIndex1); return ret; }

            //                        if (localOtherForm4().IsAbleToEnter(withBlock5.x, withBlock5.y))
            //                        {
            //                            withBlock5.Transform(uname);
            //                            {
            //                                var withBlock7 = withBlock5.CurrentForm();
            //                                withBlock7.HP = withBlock7.MaxHP;
            //                                withBlock7.UsedAction = withBlock7.MaxAction();
            //                            }
            //                        }
            //                        else
            //                        {
            //                            withBlock5.Die();
            //                        }
            //                    }
            //                    else
            //                    {
            //                        withBlock5.Die();
            //                    }
            //                }
            //                else if (a.IsAbilityClassifiedAs("失") & withBlock5.HP == 0)
            //                {
            //                    withBlock5.Die();
            //                }
            //                else if (a.IsAbilityClassifiedAs("変"))
            //                {
            //                    if (withBlock5.IsFeatureAvailable("ノーマルモード"))
            //                    {
            //                        uname = GeneralLib.LIndex(withBlock5.FeatureData(argIndex15), 1);
            //                        Unit localOtherForm5() { object argIndex1 = uname; var ret = withBlock5.OtherForm(argIndex1); return ret; }

            //                        if (localOtherForm5().IsAbleToEnter(withBlock5.x, withBlock5.y))
            //                        {
            //                            withBlock5.Transform(uname);
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // 移動型マップアビリティによる移動
            //    if (a.IsAbilityClassifiedAs("Ｍ移"))
            //    {
            //        Jump(tx, ty);
            //    }

            //Finish:
            //    ;


            //    // 以下の効果はアビリティデータが変化する可能性があるため、同時には適用されない

            //    // 自爆の処理

            //    // ＨＰ消費アビリティで自殺した場合

            //    // 変形技
            //    if (a.IsAbilityClassifiedAs("自"))
            //    {
            //        if (IsFeatureAvailable("パーツ分離"))
            //        {
            //            // パーツ合体したユニットが自爆する時はパーツを分離するだけ
            //            uname = GeneralLib.LIndex(FeatureData(argIndex16), 2);
            //            Unit localOtherForm6() { object argIndex1 = uname; var ret = OtherForm(argIndex1); return ret; }

            //            if (localOtherForm6().IsAbleToEnter(x, y))
            //            {
            //                Transform(uname);
            //                {
            //                    var withBlock8 = CurrentForm();
            //                    withBlock8.HP = withBlock8.MaxHP;
            //                    withBlock8.UsedAction = withBlock8.MaxAction();
            //                }

            //                fname = FeatureName("パーツ分離");
            //                bool localIsSysMessageDefined() { string argmain_situation = "破壊時分離(" + fname + ")"; string argsub_situation = ""; var ret = IsSysMessageDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //                bool localIsSysMessageDefined1() { string argmain_situation = "分離(" + Name + ")"; string argsub_situation = ""; var ret = IsSysMessageDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //                bool localIsSysMessageDefined2() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = IsSysMessageDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

            //                if (IsSysMessageDefined("破壊時分離(" + Name + ")", sub_situation: ""))
            //                {
            //                    SysMessage("破壊時分離(" + Name + ")", sub_situation: "", add_msg: "");
            //                }
            //                else if (localIsSysMessageDefined())
            //                {
            //                    SysMessage("破壊時分離(" + fname + ")", sub_situation: "", add_msg: "");
            //                }
            //                else if (IsSysMessageDefined("破壊時分離", sub_situation: ""))
            //                {
            //                    SysMessage("破壊時分離", sub_situation: "", add_msg: "");
            //                }
            //                else if (localIsSysMessageDefined1())
            //                {
            //                    SysMessage("分離(" + Name + ")", sub_situation: "", add_msg: "");
            //                }
            //                else if (localIsSysMessageDefined2())
            //                {
            //                    SysMessage("分離(" + fname + ")", sub_situation: "", add_msg: "");
            //                }
            //                else if (IsSysMessageDefined("分離", sub_situation: ""))
            //                {
            //                    SysMessage("分離", sub_situation: "", add_msg: "");
            //                }
            //                else
            //                {
            //                    GUI.DisplaySysMessage(Nickname + "は破壊されたパーツを分離させた。");
            //                }
            //            }
            //            else
            //            {
            //                // しかし、パーツ分離できない地形ではそのまま自爆
            //                Die();
            //                if (!is_event)
            //                {
            //                    Event.HandleEvent("破壊", MainPilot().ID);
            //                    if (SRC.IsScenarioFinished)
            //                    {
            //                        return;
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            Die();
            //            if (!is_event)
            //            {
            //                Event.HandleEvent("破壊", MainPilot().ID);
            //                if (SRC.IsScenarioFinished)
            //                {
            //                    return;
            //                }
            //            }
            //        }
            //    }
            //    else if (a.IsAbilityClassifiedAs("失") & HP == 0)
            //    {
            //        Die();
            //        if (!is_event)
            //        {
            //            Event.HandleEvent("破壊", MainPilot().ID);
            //            if (SRC.IsScenarioFinished)
            //            {
            //                return;
            //            }
            //        }
            //    }
            //    else if (a.IsAbilityClassifiedAs("変"))
            //    {
            //        if (IsFeatureAvailable("変形技"))
            //        {
            //            var loopTo17 = CountFeature();
            //            for (i = 1; i <= loopTo17; i++)
            //            {
            //                string localFeature1() { object argIndex1 = i; var ret = Feature(argIndex1); return ret; }

            //                string localFeatureData3() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //                string localLIndex1() { string arglist = hs60551c61d0954d3e93ffb43a55a73d66(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //                if (localFeature1() == "変形技" & (localLIndex1() ?? "") == (a.Ability().Name ?? ""))
            //                {
            //                    string localFeatureData2() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //                    uname = GeneralLib.LIndex(localFeatureData2(), 2);
            //                    Unit localOtherForm7() { object argIndex1 = uname; var ret = OtherForm(argIndex1); return ret; }

            //                    if (localOtherForm7().IsAbleToEnter(x, y))
            //                    {
            //                        Transform(uname);
            //                    }

            //                    break;
            //                }
            //            }

            //            if ((uname ?? "") != (CurrentForm().Name ?? ""))
            //            {
            //                if (IsFeatureAvailable("ノーマルモード"))
            //                {
            //                    uname = GeneralLib.LIndex(FeatureData(argIndex18), 1);
            //                    Unit localOtherForm8() { object argIndex1 = uname; var ret = OtherForm(argIndex1); return ret; }

            //                    if (localOtherForm8().IsAbleToEnter(x, y))
            //                    {
            //                        Transform(uname);
            //                    }
            //                }
            //            }
            //        }
            //        else if (IsFeatureAvailable("ノーマルモード"))
            //        {
            //            uname = GeneralLib.LIndex(FeatureData(argIndex19), 1);
            //            Unit localOtherForm9() { object argIndex1 = uname; var ret = OtherForm(argIndex1); return ret; }

            //            if (localOtherForm9().IsAbleToEnter(x, y))
            //            {
            //                Transform(uname);
            //            }
            //        }
            //    }

            //    // アイテムを消費
            //    else if (a.Ability().IsItem() & a.Stock() == 0 & a.MaxStock() > 0)
            //    {
            //        // アイテムを削除
            //        num = Data.CountAbility();
            //        num = (num + MainPilot().Data.CountAbility());
            //        var loopTo18 = CountPilot();
            //        for (i = 2; i <= loopTo18; i++)
            //        {
            //            Pilot localPilot() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

            //            num = (num + localPilot().Data.CountAbility());
            //        }

            //        var loopTo19 = CountSupport();
            //        for (i = 2; i <= loopTo19; i++)
            //        {
            //            Pilot localSupport() { object argIndex1 = i; var ret = Support(argIndex1); return ret; }

            //            num = (num + localSupport().Data.CountAbility());
            //        }

            //        if (IsFeatureAvailable("追加サポート"))
            //        {
            //            num = (num + AdditionalSupport().Data.CountAbility());
            //        }

            //        foreach (Item itm in colItem)
            //        {
            //            num = (num + itm.CountAbility());
            //            if (a <= num)
            //            {
            //                itm.Exist = false;
            //                DeleteItem((object)itm.ID);
            //                break;
            //            }
            //        }
            //    }

            //    // 使用後イベント
            //    if (!is_event)
            //    {
            //        Event.HandleEvent("使用後", CurrentForm().MainPilot().ID, aname);
            //        if (SRC.IsScenarioFinished | SRC.IsCanceled)
            //        {
            //            return;
            //        }
            //    }

            //    GUI.CloseMessageForm();

            //    // ハイパーモード＆ノーマルモードの自動発動をチェック
            //    SRC.UList.CheckAutoHyperMode();
            //    SRC.UList.CheckAutoNormalMode();
        }

        // アビリティの使用によるＥＮ、使用回数の消費等を行う
        public void UseAbility(UnitAbility a)
        {
            // TODO Impl
            //int i, lv;
            //double hp_ratio, en_ratio;
            //if (this.a.Ability().ENConsumption > 0)
            //{
            //    EN = EN - a.AbilityENConsumption();
            //}

            //if (this.a.Ability().Stock > 0)
            //{
            //    a.SetStock((a.Stock() - 1));

            //    // 一斉使用
            //    if (a.IsAbilityClassifiedAs("斉"))
            //    {
            //        var loopTo = Information.UBound(dblStock);
            //        for (i = 1; i <= loopTo; i++)
            //            SetStock(i, GeneralLib.MinLng((MaxStock(i) * a.Stock()) / a.MaxStock(), Stock(i)));
            //    }
            //    else
            //    {
            //        var loopTo1 = Information.UBound(dblStock);
            //        for (i = 1; i <= loopTo1; i++)
            //        {
            //            if (IsAbilityClassifiedAs(i, "斉"))
            //            {
            //                SetStock(i, GeneralLib.MinLng(((MaxStock(i) * a.Stock()) / a.MaxStock() + 0.49999d), Stock(i)));
            //            }
            //        }
            //    }

            //    // 弾数・使用回数共有の処理
            //    SyncBullet();
            //}

            //// 消耗技
            //if (a.IsAbilityClassifiedAs("消"))
            //{
            //    AddCondition("消耗", 1, cdata: "");
            //}

            //// 全ＥＮ消費アビリティ
            //if (a.IsAbilityClassifiedAs("尽"))
            //{
            //    EN = 0;
            //}

            //// チャージ式アビリティ
            //if (a.IsAbilityClassifiedAs("Ｃ") & IsConditionSatisfied("チャージ完了"))
            //{
            //    DeleteCondition("チャージ完了");
            //}

            //// 自動充填式アビリティ
            //if (a.AbilityLevel("Ａ") > 0d)
            //{
            //    AddCondition(a.AbilityNickname() + "充填中", a.AbilityLevel("Ａ"), cdata: "");
            //}

            //// 気力を消費
            //if (a.IsAbilityClassifiedAs("気"))
            //{
            //    IncreaseMorale((-5 * a.AbilityLevel("気")));
            //}

            //// 霊力の消費
            //if (a.IsAbilityClassifiedAs("霊"))
            //{
            //    hp_ratio = 100 * HP / (double)MaxHP;
            //    en_ratio = 100 * EN / (double)MaxEN;
            //    MainPilot().Plana = (this.MainPilot().Plana - 5d * a.AbilityLevel("霊"));
            //    HP = (MaxHP * hp_ratio / 100d);
            //    EN = (MaxEN * en_ratio / 100d);
            //}
            //else if (a.IsAbilityClassifiedAs("プ"))
            //{
            //    hp_ratio = 100 * HP / (double)MaxHP;
            //    en_ratio = 100 * EN / (double)MaxEN;
            //    MainPilot().Plana = (this.MainPilot().Plana - 5d * a.AbilityLevel("プ"));
            //    HP = (MaxHP * hp_ratio / 100d);
            //    EN = (MaxEN * en_ratio / 100d);
            //}

            //// 資金消費アビリティ
            //if (Party == "味方")
            //{
            //    if (a.IsAbilityClassifiedAs("銭"))
            //    {
            //        SRC.IncrMoney(-GeneralLib.MaxLng(a.AbilityLevel("銭"), 1) * Value / 10);
            //    }
            //}

            //// ＨＰ消費アビリティ
            //if (a.IsAbilityClassifiedAs("失"))
            //{
            //    HP = GeneralLib.MaxLng((HP - (long)(MaxHP * a.AbilityLevel("失")) / 10L), 0);
            //}
        }
    }
}
