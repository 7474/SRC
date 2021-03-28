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

        // アビリティを使用
        public bool ExecuteAbility(int a, Unit t, bool is_map_ability = false, bool is_event = false)
        {
            bool ExecuteAbilityRet = default;
            var partners = default(Unit[]);
            int num, j, i, k, w = default;
            string aclass, aname, anickname, atype = default;
            string edata;
            double elevel, elevel2;
            double elv_mod, elv_mod2;
            int epower;
            int prev_value;
            bool is_useful = default, flag;
            Unit u;
            Pilot p;
            string buf, msg;
            string uname = default, pname, fname;
            string ftype, fdata;
            double flevel;
            string ftype2, fdata2;
            double flevel2;
            var is_anime_played = default(bool);
            double hp_ratio, en_ratio;
            int tx, ty;
            int tx2, ty2;
            string cname;
            aname = Ability(a).Name;
            anickname = AbilityNickname(a);
            aclass = Ability(a).Class_Renamed;

            // 現在の選択状況をセーブ
            Commands.SaveSelections();

            // 選択内容を切り替え
            Commands.SelectedUnit = this;
            Event_Renamed.SelectedUnitForEvent = this;
            Commands.SelectedTarget = t;
            Event_Renamed.SelectedTargetForEvent = t;
            Commands.SelectedAbility = a;
            Commands.SelectedAbilityName = aname;
            if (!is_map_ability)
            {
                // 通常アビリティの場合
                if (SRC.BattleAnimation)
                {
                    GUI.RedrawScreen();
                }

                string argattr = "合";
                if (IsAbilityClassifiedAs(a, argattr))
                {
                    // 射程が0の場合はマスクをクリアしておく
                    if (AbilityMaxRange(a) == 0)
                    {
                        var loopTo = Map.MapWidth;
                        for (i = 1; i <= loopTo; i++)
                        {
                            var loopTo1 = Map.MapHeight;
                            for (j = 1; j <= loopTo1; j++)
                                Map.MaskData[i, j] = true;
                        }

                        Map.MaskData[x, y] = false;
                    }

                    // 合体技の場合にパートナーをハイライト表示
                    if (AbilityMaxRange(a) == 1)
                    {
                        string argctype_Renamed = "アビリティ";
                        CombinationPartner(argctype_Renamed, a, partners, t.x, t.y);
                    }
                    else
                    {
                        string argctype_Renamed1 = "アビリティ";
                        CombinationPartner(argctype_Renamed1, a, partners);
                    }

                    var loopTo2 = Information.UBound(partners);
                    for (i = 1; i <= loopTo2; i++)
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
                else
                {
                    partners = new Unit[1];
                    Commands.SelectedPartners = new Unit[1];
                }

                // ダイアログ用にあらかじめ追加パイロットを作成しておく
                var loopTo3 = Ability(a).CountEffect();
                for (i = 1; i <= loopTo3; i++)
                {
                    object argIndex1 = i;
                    edata = Ability(a).EffectData(argIndex1);
                    object argIndex4 = i;
                    switch (Ability(a).EffectType(argIndex4) ?? "")
                    {
                        case "変身":
                            {
                                bool localIsDefined() { object argIndex1 = GeneralLib.LIndex(edata, 1); var ret = SRC.UDList.IsDefined(argIndex1); return ret; }

                                if (!localIsDefined())
                                {
                                    string argmsg = GeneralLib.LIndex(edata, 1) + "のデータが定義されていません";
                                    GUI.ErrorMessage(argmsg);
                                    return ExecuteAbilityRet;
                                }

                                object argIndex3 = GeneralLib.LIndex(edata, 1);
                                {
                                    var withBlock1 = SRC.UDList.Item(argIndex3);
                                    string argfname = "追加パイロット";
                                    if (withBlock1.IsFeatureAvailable(argfname))
                                    {
                                        bool localIsDefined1() { object argIndex1 = "追加パイロット"; object argIndex2 = withBlock1.FeatureData(argIndex1); var ret = SRC.PList.IsDefined(argIndex2); return ret; }

                                        if (!localIsDefined1())
                                        {
                                            object argIndex2 = "追加パイロット";
                                            string argpname = withBlock1.FeatureData(argIndex2);
                                            string argpparty = Party0;
                                            string arggid = "";
                                            SRC.PList.Add(argpname, MainPilot().Level, argpparty, gid: arggid);
                                            this.Party0 = argpparty;
                                        }
                                    }
                                }

                                break;
                            }
                    }
                }

                // アビリティ使用時のメッセージ＆特殊効果
                string argmain_situation1 = aname + "(準備)";
                string argsub_situation1 = "";
                if (IsAnimationDefined(argmain_situation1, sub_situation: argsub_situation1))
                {
                    string argmain_situation = aname + "(準備)";
                    string argsub_situation = "";
                    PlayAnimation(argmain_situation, sub_situation: argsub_situation);
                }

                string argmain_situation2 = "かけ声(" + aname + ")";
                if (IsMessageDefined(argmain_situation2))
                {
                    if (!My.MyProject.Forms.frmMessage.Visible)
                    {
                        if (ReferenceEquals(Commands.SelectedTarget, this))
                        {
                            var argu1 = this;
                            Unit argu2 = null;
                            GUI.OpenMessageForm(argu1, u2: argu2);
                        }
                        else
                        {
                            var argu21 = this;
                            GUI.OpenMessageForm(Commands.SelectedTarget, argu21);
                        }
                    }

                    string argSituation = "かけ声(" + aname + ")";
                    string argmsg_mode = "";
                    PilotMessage(argSituation, msg_mode: argmsg_mode);
                }

                string argmain_situation3 = "アビリティ";
                if (IsMessageDefined(aname) | IsMessageDefined(argmain_situation3))
                {
                    if (!My.MyProject.Forms.frmMessage.Visible)
                    {
                        if (ReferenceEquals(Commands.SelectedTarget, this))
                        {
                            var argu11 = this;
                            Unit argu22 = null;
                            GUI.OpenMessageForm(argu11, u2: argu22);
                        }
                        else
                        {
                            var argu23 = this;
                            GUI.OpenMessageForm(Commands.SelectedTarget, argu23);
                        }
                    }

                    string argmsg_mode1 = "アビリティ";
                    PilotMessage(aname, argmsg_mode1);
                }

                string argmain_situation5 = aname + "(使用)";
                string argsub_situation3 = "";
                if (IsAnimationDefined(argmain_situation5, sub_situation: argsub_situation3))
                {
                    string argmain_situation4 = aname + "(使用)";
                    string argsub_situation2 = "";
                    PlayAnimation(argmain_situation4, argsub_situation2, true);
                }

                string argmain_situation7 = aname + "(発動)";
                string argsub_situation6 = "";
                string argsub_situation7 = "";
                if (IsAnimationDefined(argmain_situation7, sub_situation: argsub_situation6) | IsAnimationDefined(aname, sub_situation: argsub_situation7))
                {
                    string argmain_situation6 = aname + "(発動)";
                    string argsub_situation4 = "";
                    PlayAnimation(argmain_situation6, argsub_situation4, true);
                    is_anime_played = true;
                }
                else
                {
                    string argsub_situation5 = "";
                    SpecialEffect(aname, argsub_situation5, true);
                }

                // アビリティの種類は？
                var loopTo4 = Ability(a).CountEffect();
                for (i = 1; i <= loopTo4; i++)
                {
                    object argIndex7 = i;
                    switch (Ability(a).EffectType(argIndex7) ?? "")
                    {
                        case "召喚":
                            {
                                aname = "";
                                break;
                            }

                        case "再行動":
                            {
                                if (this.Ability(a).MaxRange > 0)
                                {
                                    object argIndex5 = i;
                                    atype = Ability(a).EffectType(argIndex5);
                                }

                                break;
                            }

                        case "解説":
                            {
                                break;
                            }

                        default:
                            {
                                object argIndex6 = i;
                                atype = Ability(a).EffectType(argIndex6);
                                break;
                            }
                    }
                }

                switch (Information.UBound(partners))
                {
                    case 0:
                        {
                            // 通常
                            msg = Nickname + "は";
                            break;
                        }

                    case 1:
                        {
                            // ２体合体
                            if ((Nickname ?? "") != (partners[1].Nickname ?? ""))
                            {
                                msg = Nickname + "は[" + partners[1].Nickname + "]と共に";
                            }
                            else if ((MainPilot().get_Nickname(false) ?? "") != (partners[1].MainPilot().get_Nickname(false) ?? ""))
                            {
                                msg = MainPilot().get_Nickname(false) + "と[" + partners[1].MainPilot().get_Nickname(false) + "]の[" + Nickname + "]は";
                            }
                            else
                            {
                                msg = Nickname + "達は";
                            }

                            break;
                        }

                    case 2:
                        {
                            // ３体合体
                            if ((Nickname ?? "") != (partners[1].Nickname ?? ""))
                            {
                                msg = Nickname + "は[" + partners[1].Nickname + "]、[" + partners[2].Nickname + "]と共に";
                            }
                            else if ((MainPilot().get_Nickname(false) ?? "") != (partners[1].MainPilot().get_Nickname(false) ?? ""))
                            {
                                msg = MainPilot().get_Nickname(false) + "、[" + partners[1].MainPilot().get_Nickname(false) + "]、[" + partners[2].MainPilot().get_Nickname(false) + "]の[" + Nickname + "]は";
                            }
                            else
                            {
                                msg = Nickname + "達は";
                            }

                            break;
                        }

                    default:
                        {
                            // ３体以上
                            msg = Nickname + "達は";
                            break;
                        }
                }

                if (IsSpellAbility(a))
                {
                    if (t is object & this.Ability(a).MaxRange != 0)
                    {
                        if (ReferenceEquals(this, t))
                        {
                            msg = msg + "自分に";
                        }
                        else
                        {
                            msg = msg + "[" + t.Nickname + "]に";
                        }
                    }

                    if (Strings.Right(anickname, 2) == "呪文")
                    {
                        msg = msg + "[" + anickname + "]を唱えた。";
                    }
                    else if (Strings.Right(anickname, 2) == "の杖")
                    {
                        msg = msg + "[" + Strings.Left(anickname, Strings.Len(anickname) - 2) + "]の呪文を唱えた。";
                    }
                    else
                    {
                        msg = msg + "[" + anickname + "]の呪文を唱えた。";
                    }
                }
                else if (Strings.Right(anickname, 1) == "歌")
                {
                    msg = msg + "[" + anickname + "]を歌った。";
                }
                else if (Strings.Right(anickname, 2) == "踊り")
                {
                    msg = msg + "[" + anickname + "]を踊った。";
                }
                else
                {
                    if (t is object & this.Ability(a).MaxRange != 0)
                    {
                        if (ReferenceEquals(this, t))
                        {
                            msg = msg + "自分に";
                        }
                        else
                        {
                            msg = msg + "[" + t.Nickname + "]に";
                        }
                    }

                    msg = msg + "[" + anickname + "]を使った。";
                }

                string argsub_situation10 = "";
                string argmain_situation9 = "アビリティ";
                string argsub_situation11 = "";
                if (IsSysMessageDefined(aname, sub_situation: argsub_situation10))
                {
                    // 「アビリティ名(解説)」のメッセージを使用
                    if (!My.MyProject.Forms.frmMessage.Visible)
                    {
                        if (ReferenceEquals(Commands.SelectedTarget, this))
                        {
                            var argu12 = this;
                            Unit argu24 = null;
                            GUI.OpenMessageForm(argu12, u2: argu24);
                        }
                        else
                        {
                            var argu25 = this;
                            GUI.OpenMessageForm(Commands.SelectedTarget, argu25);
                        }
                    }

                    string argsub_situation8 = "";
                    string argadd_msg = "";
                    SysMessage(aname, sub_situation: argsub_situation8, add_msg: argadd_msg);
                }
                else if (IsSysMessageDefined(argmain_situation9, sub_situation: argsub_situation11))
                {
                    // 「アビリティ(解説)」のメッセージを使用
                    if (!My.MyProject.Forms.frmMessage.Visible)
                    {
                        if (ReferenceEquals(Commands.SelectedTarget, this))
                        {
                            var argu13 = this;
                            Unit argu26 = null;
                            GUI.OpenMessageForm(argu13, u2: argu26);
                        }
                        else
                        {
                            var argu27 = this;
                            GUI.OpenMessageForm(Commands.SelectedTarget, argu27);
                        }
                    }

                    string argmain_situation8 = "アビリティ";
                    string argsub_situation9 = "";
                    string argadd_msg1 = "";
                    SysMessage(argmain_situation8, sub_situation: argsub_situation9, add_msg: argadd_msg1);
                }
                else if (atype == "変身" & this.Ability(a).MaxRange == 0)
                {
                }
                // 変身の場合はメッセージなし
                else if (!string.IsNullOrEmpty(atype))
                {
                    if (!My.MyProject.Forms.frmMessage.Visible)
                    {
                        if (ReferenceEquals(Commands.SelectedTarget, this))
                        {
                            var argu14 = this;
                            Unit argu28 = null;
                            GUI.OpenMessageForm(argu14, u2: argu28);
                        }
                        else
                        {
                            var argu29 = this;
                            GUI.OpenMessageForm(Commands.SelectedTarget, argu29);
                        }
                    }

                    GUI.DisplaySysMessage(msg);
                }

                // ＥＮ消費＆使用回数減少
                UseAbility(a);

                // アビリティの使用に失敗？
                string argattr1 = "難";
                if (GeneralLib.Dice(10) <= AbilityLevel(a, argattr1))
                {
                    GUI.DisplaySysMessage("しかし何もおきなかった…");
                    goto Finish;
                }
            }
            else
            {
                // マップアビリティの場合
                string argmain_situation11 = aname + "(発動)";
                string argsub_situation13 = "";
                string argsub_situation14 = "";
                if (IsAnimationDefined(argmain_situation11, sub_situation: argsub_situation13) | IsAnimationDefined(aname, sub_situation: argsub_situation14))
                {
                    string argmain_situation10 = aname + "(発動)";
                    string argsub_situation12 = "";
                    PlayAnimation(argmain_situation10, sub_situation: argsub_situation12);
                    is_anime_played = true;
                }
            }

            // 相手がアビリティの属性に対して無効化属性を持っているならアビリティは
            // 効果なし
            if (!ReferenceEquals(this, t))
            {
                if (t.Immune(aclass))
                {
                    goto Finish;
                }
            }

            // 気力低下アビリティ
            string argattr2 = "脱";
            if (IsAbilityClassifiedAs(a, argattr2))
            {
                t.IncreaseMorale(-10);
            }

            // 特殊効果除去アビリティ
            string argattr3 = "除";
            if (IsAbilityClassifiedAs(a, argattr3))
            {
                i = 1;
                while (i <= t.CountCondition())
                {
                    string localCondition() { object argIndex1 = i; var ret = t.Condition(argIndex1); return ret; }

                    string localCondition1() { object argIndex1 = i; var ret = t.Condition(argIndex1); return ret; }

                    string localCondition2() { object argIndex1 = i; var ret = t.Condition(argIndex1); return ret; }

                    string localCondition3() { object argIndex1 = i; var ret = t.Condition(argIndex1); return ret; }

                    int localConditionLifetime() { object argIndex1 = i; var ret = t.ConditionLifetime(argIndex1); return ret; }

                    if ((Strings.InStr(localCondition(), "付加") > 0 | Strings.InStr(localCondition1(), "強化") > 0 | Strings.InStr(localCondition2(), "ＵＰ") > 0) & localCondition3() != "ノーマルモード付加" & localConditionLifetime() != 0)
                    {
                        object argIndex8 = i;
                        t.DeleteCondition(argIndex8);
                    }
                    else
                    {
                        i = (i + 1);
                    }
                }
            }

            // 得意技・不得手によるアビリティ効果への修正値を計算
            elv_mod = 1d;
            elv_mod2 = 1d;
            {
                var withBlock2 = MainPilot();
                // 得意技
                string argsname = "得意技";
                if (withBlock2.IsSkillAvailable(argsname))
                {
                    object argIndex9 = "得意技";
                    buf = withBlock2.SkillData(argIndex9);
                    var loopTo5 = Strings.Len(buf);
                    for (i = 1; i <= loopTo5; i++)
                    {
                        if (Strings.InStr(aclass, GeneralLib.GetClassBundle(buf, i)) > 0)
                        {
                            elv_mod = 1.2d * elv_mod;
                            elv_mod2 = 1.4d * elv_mod2;
                            break;
                        }
                    }
                }

                // 不得手
                string argsname1 = "不得手";
                if (withBlock2.IsSkillAvailable(argsname1))
                {
                    object argIndex10 = "不得手";
                    buf = withBlock2.SkillData(argIndex10);
                    var loopTo6 = Strings.Len(buf);
                    for (i = 1; i <= loopTo6; i++)
                    {
                        if (Strings.InStr(aclass, GeneralLib.GetClassBundle(buf, i)) > 0)
                        {
                            elv_mod = 0.8d * elv_mod;
                            elv_mod2 = 0.6d * elv_mod2;
                            break;
                        }
                    }
                }
            }

            // アビリティの効果を適用
            var loopTo7 = Ability(a).CountEffect();
            for (i = 1; i <= loopTo7; i++)
            {
                {
                    var withBlock3 = Ability(a);
                    object argIndex11 = i;
                    edata = withBlock3.EffectData(argIndex11);
                    object argIndex12 = i;
                    elevel = withBlock3.EffectLevel(argIndex12) * elv_mod;
                    object argIndex13 = i;
                    elevel2 = withBlock3.EffectLevel(argIndex13) * elv_mod2;
                }

                object argIndex95 = i;
                switch (Ability(a).EffectType(argIndex95) ?? "")
                {
                    case "回復":
                        {
                            {
                                var withBlock4 = t;
                                if (elevel > 0d)
                                {
                                    // ＨＰは既に最大値？
                                    if (withBlock4.HP == withBlock4.MaxHP)
                                    {
                                        goto NextLoop;
                                    }

                                    // ゾンビ？
                                    object argIndex14 = "ゾンビ";
                                    if (withBlock4.IsConditionSatisfied(argIndex14))
                                    {
                                        goto NextLoop;
                                    }

                                    if (!is_anime_played)
                                    {
                                        string argattr4 = "魔";
                                        if (IsSpellAbility(a) | IsAbilityClassifiedAs(a, argattr4))
                                        {
                                            string arganame = "回復魔法発動";
                                            Effect.ShowAnimation(arganame);
                                        }
                                        else
                                        {
                                            string arganame1 = "修理装置発動";
                                            Effect.ShowAnimation(arganame1);
                                        }
                                    }

                                    prev_value = withBlock4.HP;
                                    {
                                        var withBlock5 = MainPilot();
                                        if (IsSpellAbility(a))
                                        {
                                            epower = (5d * elevel * withBlock5.Shooting);
                                        }
                                        else
                                        {
                                            epower = (500d * elevel);
                                        }

                                        object argIndex15 = "修理";
                                        string argref_mode = "";
                                        epower = ((long)(epower * (10d + withBlock5.SkillLevel(argIndex15, ref_mode: argref_mode))) / 10L);
                                    }

                                    t.HP = t.HP + epower;
                                    string argmsg1 = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.HP - prev_value);
                                    GUI.DrawSysString(withBlock4.x, withBlock4.y, argmsg1);
                                    if (ReferenceEquals(t, this))
                                    {
                                        var argu15 = this;
                                        object argu210 = null;
                                        GUI.UpdateMessageForm(argu15, u2: argu210);
                                    }
                                    else
                                    {
                                        object argu211 = this;
                                        GUI.UpdateMessageForm(t, argu211);
                                    }

                                    string argtname = "ＨＰ";
                                    string argtname1 = "ＨＰ";
                                    GUI.DisplaySysMessage(withBlock4.Nickname + "の" + Expression.Term(argtname, t) + "が[" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.HP - prev_value) + "]回復した;" + "残り" + Expression.Term(argtname1, t) + "は" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.HP) + "（損傷率 = " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(100 * (withBlock4.MaxHP - withBlock4.HP) / withBlock4.MaxHP) + "％）");
                                    is_useful = true;
                                }
                                else if (elevel < 0d)
                                {
                                    prev_value = withBlock4.HP;
                                    {
                                        var withBlock6 = MainPilot();
                                        if (IsSpellAbility(a))
                                        {
                                            epower = (5d * elevel * withBlock6.Shooting);
                                        }
                                        else
                                        {
                                            epower = (500d * elevel);
                                        }
                                    }

                                    t.HP = t.HP + epower;
                                    string argmsg2 = "-" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock4.HP);
                                    GUI.DrawSysString(withBlock4.x, withBlock4.y, argmsg2);
                                    if (ReferenceEquals(t, this))
                                    {
                                        var argu16 = this;
                                        object argu212 = null;
                                        GUI.UpdateMessageForm(argu16, u2: argu212);
                                    }
                                    else
                                    {
                                        object argu213 = this;
                                        GUI.UpdateMessageForm(t, argu213);
                                    }

                                    string argtname2 = "ＨＰ";
                                    string argtname3 = "ＨＰ";
                                    GUI.DisplaySysMessage(withBlock4.Nickname + "の" + Expression.Term(argtname2, t) + "が[" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock4.HP) + "]減少した;" + "残り" + Expression.Term(argtname3, t) + "は" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.HP) + "（損傷率 = " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(100 * (withBlock4.MaxHP - withBlock4.HP) / withBlock4.MaxHP) + "％）");
                                }
                            }

                            break;
                        }

                    case "補給":
                        {
                            {
                                var withBlock7 = t;
                                if (elevel > 0d)
                                {
                                    // ＥＮは既に最大値？
                                    if (withBlock7.EN == withBlock7.MaxEN)
                                    {
                                        goto NextLoop;
                                    }

                                    // ゾンビ？
                                    object argIndex16 = "ゾンビ";
                                    if (withBlock7.IsConditionSatisfied(argIndex16))
                                    {
                                        goto NextLoop;
                                    }

                                    if (!is_anime_played)
                                    {
                                        string argattr5 = "魔";
                                        if (IsSpellAbility(a) | IsAbilityClassifiedAs(a, argattr5))
                                        {
                                            string arganame2 = "回復魔法発動";
                                            Effect.ShowAnimation(arganame2);
                                        }
                                        else
                                        {
                                            string arganame3 = "補給装置発動";
                                            Effect.ShowAnimation(arganame3);
                                        }
                                    }

                                    prev_value = withBlock7.EN;
                                    {
                                        var withBlock8 = MainPilot();
                                        if (IsSpellAbility(a))
                                        {
                                            epower = ((long)(elevel * withBlock8.Shooting) / 2L);
                                        }
                                        else
                                        {
                                            epower = (50d * elevel);
                                        }

                                        object argIndex17 = "補給";
                                        string argref_mode1 = "";
                                        epower = ((long)(epower * (10d + withBlock8.SkillLevel(argIndex17, ref_mode: argref_mode1))) / 10L);
                                    }

                                    t.EN = t.EN + epower;
                                    string argmsg3 = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock7.EN - prev_value);
                                    GUI.DrawSysString(withBlock7.x, withBlock7.y, argmsg3);
                                    if (ReferenceEquals(t, this))
                                    {
                                        var argu17 = this;
                                        object argu214 = null;
                                        GUI.UpdateMessageForm(argu17, u2: argu214);
                                    }
                                    else
                                    {
                                        object argu215 = this;
                                        GUI.UpdateMessageForm(t, argu215);
                                    }

                                    string argtname4 = "ＥＮ";
                                    string argtname5 = "ＥＮ";
                                    GUI.DisplaySysMessage(withBlock7.Nickname + "の" + Expression.Term(argtname4, t) + "が[" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock7.EN - prev_value) + "]回復した;" + "残り" + Expression.Term(argtname5, t) + "は" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock7.EN));
                                    is_useful = true;
                                }
                                else if (elevel < 0d)
                                {
                                    // ＥＮは既に0？
                                    if (withBlock7.EN == 0)
                                    {
                                        goto NextLoop;
                                    }

                                    prev_value = withBlock7.EN;
                                    {
                                        var withBlock9 = MainPilot();
                                        if (IsSpellAbility(a))
                                        {
                                            epower = ((long)(elevel * withBlock9.Shooting) / 2L);
                                        }
                                        else
                                        {
                                            epower = (50d * elevel);
                                        }
                                    }

                                    t.EN = t.EN + epower;
                                    string argmsg4 = "-" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock7.EN);
                                    GUI.DrawSysString(withBlock7.x, withBlock7.y, argmsg4);
                                    if (ReferenceEquals(t, this))
                                    {
                                        var argu18 = this;
                                        object argu216 = null;
                                        GUI.UpdateMessageForm(argu18, u2: argu216);
                                    }
                                    else
                                    {
                                        object argu217 = this;
                                        GUI.UpdateMessageForm(t, argu217);
                                    }

                                    string argtname6 = "ＥＮ";
                                    string argtname7 = "ＥＮ";
                                    GUI.DisplaySysMessage(withBlock7.Nickname + "の" + Expression.Term(argtname6, t) + "が[" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock7.EN) + "]減少した;" + "残り" + Expression.Term(argtname7, t) + "は" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock7.EN));
                                }
                            }

                            break;
                        }

                    case "霊力回復":
                    case "プラーナ回復":
                        {
                            {
                                var withBlock10 = t.MainPilot();
                                if (elevel > 0d)
                                {
                                    // 霊力は既に最大値？
                                    if (withBlock10.Plana == withBlock10.MaxPlana())
                                    {
                                        goto NextLoop;
                                    }

                                    prev_value = withBlock10.Plana;
                                    if (IsSpellAbility(a))
                                    {
                                        withBlock10.Plana = withBlock10.Plana + ((long)(elevel * this.MainPilot().Shooting) / 10L);
                                    }
                                    else
                                    {
                                        withBlock10.Plana = (withBlock10.Plana + 10d * elevel);
                                    }

                                    string argmsg5 = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock10.Plana - prev_value);
                                    GUI.DrawSysString(t.x, t.y, argmsg5);
                                    if (ReferenceEquals(t, this))
                                    {
                                        var argu19 = this;
                                        object argu218 = null;
                                        GUI.UpdateMessageForm(argu19, u2: argu218);
                                    }
                                    else
                                    {
                                        object argu219 = this;
                                        GUI.UpdateMessageForm(t, argu219);
                                    }

                                    object argIndex18 = "霊力";
                                    GUI.DisplaySysMessage(withBlock10.get_Nickname(false) + "の[" + withBlock10.SkillName0(argIndex18) + "]が[" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock10.Plana - prev_value) + "]回復した。");
                                    is_useful = true;
                                }
                                else if (elevel < 0d)
                                {
                                    // 霊力は既に0？
                                    if (withBlock10.Plana == 0)
                                    {
                                        goto NextLoop;
                                    }

                                    prev_value = withBlock10.Plana;
                                    if (IsSpellAbility(a))
                                    {
                                        withBlock10.Plana = withBlock10.Plana + ((long)(elevel * this.MainPilot().Shooting) / 10L);
                                    }
                                    else
                                    {
                                        withBlock10.Plana = (withBlock10.Plana + 10d * elevel);
                                    }

                                    string argmsg6 = "-" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock10.Plana);
                                    GUI.DrawSysString(t.x, t.y, argmsg6);
                                    if (ReferenceEquals(t, this))
                                    {
                                        var argu110 = this;
                                        object argu220 = null;
                                        GUI.UpdateMessageForm(argu110, u2: argu220);
                                    }
                                    else
                                    {
                                        object argu221 = this;
                                        GUI.UpdateMessageForm(t, argu221);
                                    }

                                    object argIndex19 = "霊力";
                                    GUI.DisplaySysMessage(withBlock10.get_Nickname(false) + "の[" + withBlock10.SkillName0(argIndex19) + "]が[" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock10.Plana) + "]減少した。");
                                }
                            }

                            break;
                        }

                    case "ＳＰ回復":
                        {
                            if (IsSpellAbility(a))
                            {
                                epower = ((long)(elevel * this.MainPilot().Shooting) / 10L);
                            }
                            else
                            {
                                epower = (10d * elevel);
                            }

                            {
                                var withBlock11 = t;
                                // パイロット数を計算
                                num = (withBlock11.CountPilot() + withBlock11.CountSupport());
                                string argfname1 = "追加サポート";
                                if (withBlock11.IsFeatureAvailable(argfname1))
                                {
                                    num = (num + 1);
                                }

                                if (elevel > 0d)
                                {
                                    if (num == 1)
                                    {
                                        // パイロットが１名のみ
                                        {
                                            var withBlock12 = withBlock11.MainPilot();
                                            prev_value = withBlock12.SP;
                                            withBlock12.SP = withBlock12.SP + epower;
                                            string argmsg7 = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock12.SP - prev_value);
                                            GUI.DrawSysString(t.x, t.y, argmsg7);
                                            string argtname8 = "ＳＰ";
                                            GUI.DisplaySysMessage(withBlock12.get_Nickname(false) + "の" + Expression.Term(argtname8, t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock12.SP - prev_value) + "回復した。");
                                            if (withBlock12.SP > prev_value)
                                            {
                                                is_useful = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // 複数のパイロットが対象
                                        {
                                            var withBlock13 = withBlock11.MainPilot();
                                            prev_value = withBlock13.SP;
                                            withBlock13.SP = withBlock13.SP + epower / 5 + epower / num;
                                            string argmsg8 = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock13.SP - prev_value);
                                            GUI.DrawSysString(t.x, t.y, argmsg8);
                                            string argtname9 = "ＳＰ";
                                            GUI.DisplaySysMessage(withBlock13.get_Nickname(false) + "の" + Expression.Term(argtname9, t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock13.SP - prev_value) + "回復した。");
                                            if (withBlock13.SP > prev_value)
                                            {
                                                is_useful = true;
                                            }
                                        }

                                        var loopTo8 = withBlock11.CountPilot();
                                        for (j = 2; j <= loopTo8; j++)
                                        {
                                            object argIndex20 = j;
                                            {
                                                var withBlock14 = withBlock11.Pilot(argIndex20);
                                                prev_value = withBlock14.SP;
                                                withBlock14.SP = withBlock14.SP + epower / 5 + epower / num;
                                                string argtname10 = "ＳＰ";
                                                GUI.DisplaySysMessage(withBlock14.get_Nickname(false) + "の" + Expression.Term(argtname10, t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock14.SP - prev_value) + "回復した。");
                                                if (withBlock14.SP > prev_value)
                                                {
                                                    is_useful = true;
                                                }
                                            }
                                        }

                                        var loopTo9 = withBlock11.CountSupport();
                                        for (j = 1; j <= loopTo9; j++)
                                        {
                                            object argIndex21 = j;
                                            {
                                                var withBlock15 = withBlock11.Support(argIndex21);
                                                prev_value = withBlock15.SP;
                                                withBlock15.SP = withBlock15.SP + epower / 5 + epower / num;
                                                string argtname11 = "ＳＰ";
                                                GUI.DisplaySysMessage(withBlock15.get_Nickname(false) + "の" + Expression.Term(argtname11, t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock15.SP - prev_value) + "回復した。");
                                                if (withBlock15.SP > prev_value)
                                                {
                                                    is_useful = true;
                                                }
                                            }
                                        }

                                        string argfname2 = "追加サポート";
                                        if (withBlock11.IsFeatureAvailable(argfname2))
                                        {
                                            {
                                                var withBlock16 = withBlock11.AdditionalSupport();
                                                prev_value = withBlock16.SP;
                                                withBlock16.SP = withBlock16.SP + epower / 5 + epower / num;
                                                string argtname12 = "ＳＰ";
                                                GUI.DisplaySysMessage(withBlock16.get_Nickname(false) + "の" + Expression.Term(argtname12, t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock16.SP - prev_value) + "回復した。");
                                                if (withBlock16.SP > prev_value)
                                                {
                                                    is_useful = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (elevel < 0d)
                                {
                                    if (num == 1)
                                    {
                                        // パイロットが１名のみ
                                        {
                                            var withBlock17 = withBlock11.MainPilot();
                                            prev_value = withBlock17.SP;
                                            withBlock17.SP = withBlock17.SP + epower;
                                            string argmsg9 = "-" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock17.SP);
                                            GUI.DrawSysString(t.x, t.y, argmsg9);
                                            string argtname13 = "ＳＰ";
                                            GUI.DisplaySysMessage(withBlock17.get_Nickname(false) + "の" + Expression.Term(argtname13, t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock17.SP) + "減少した。");
                                        }
                                    }
                                    else
                                    {
                                        // 複数のパイロットが対象
                                        {
                                            var withBlock18 = withBlock11.MainPilot();
                                            prev_value = withBlock18.SP;
                                            withBlock18.SP = withBlock18.SP + epower / 5 + epower / num;
                                            string argmsg10 = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock18.SP);
                                            GUI.DrawSysString(t.x, t.y, argmsg10);
                                            string argtname14 = "ＳＰ";
                                            GUI.DisplaySysMessage(withBlock18.get_Nickname(false) + "の" + Expression.Term(argtname14, t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock18.SP) + "減少した。");
                                        }

                                        var loopTo10 = withBlock11.CountPilot();
                                        for (j = 2; j <= loopTo10; j++)
                                        {
                                            object argIndex22 = j;
                                            {
                                                var withBlock19 = withBlock11.Pilot(argIndex22);
                                                prev_value = withBlock19.SP;
                                                withBlock19.SP = withBlock19.SP + epower / 5 + epower / num;
                                                string argtname15 = "ＳＰ";
                                                GUI.DisplaySysMessage(withBlock19.get_Nickname(false) + "の" + Expression.Term(argtname15, t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock19.SP) + "減少した。");
                                            }
                                        }

                                        var loopTo11 = withBlock11.CountSupport();
                                        for (j = 1; j <= loopTo11; j++)
                                        {
                                            object argIndex23 = j;
                                            {
                                                var withBlock20 = withBlock11.Support(argIndex23);
                                                prev_value = withBlock20.SP;
                                                withBlock20.SP = withBlock20.SP + epower / 5 + epower / num;
                                                string argtname16 = "ＳＰ";
                                                GUI.DisplaySysMessage(withBlock20.get_Nickname(false) + "の" + Expression.Term(argtname16, t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock20.SP) + "減少した。");
                                            }
                                        }

                                        string argfname3 = "追加サポート";
                                        if (withBlock11.IsFeatureAvailable(argfname3))
                                        {
                                            {
                                                var withBlock21 = withBlock11.AdditionalSupport();
                                                prev_value = withBlock21.SP;
                                                withBlock21.SP = withBlock21.SP + epower / 5 + epower / num;
                                                string argtname17 = "ＳＰ";
                                                GUI.DisplaySysMessage(withBlock21.get_Nickname(false) + "の" + Expression.Term(argtname17, t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock21.SP) + "減少した。");
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                        }

                    case "気力増加":
                        {
                            if (IsSpellAbility(a))
                            {
                                epower = ((long)(elevel * this.MainPilot().Shooting) / 10L);
                            }
                            else
                            {
                                epower = (10d * elevel);
                            }

                            {
                                var withBlock22 = t;
                                prev_value = withBlock22.MainPilot().Morale;
                                withBlock22.IncreaseMorale(epower);
                                if (elevel > 0d)
                                {
                                    {
                                        var withBlock23 = withBlock22.MainPilot();
                                        string argmsg11 = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock23.Morale - prev_value);
                                        GUI.DrawSysString(t.x, t.y, argmsg11);
                                        string argtname18 = "気力";
                                        GUI.DisplaySysMessage(withBlock23.get_Nickname(false) + "の" + Expression.Term(argtname18, t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock23.Morale - prev_value) + "増加した。");
                                    }
                                }
                                else if (elevel < 0d)
                                {
                                    {
                                        var withBlock24 = withBlock22.MainPilot();
                                        string argmsg12 = "-" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock24.Morale);
                                        GUI.DrawSysString(t.x, t.y, argmsg12);
                                        string argtname19 = "気力";
                                        GUI.DisplaySysMessage(withBlock24.get_Nickname(false) + "の" + Expression.Term(argtname19, t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_value - withBlock24.Morale) + "減少した。");
                                    }
                                }

                                if (withBlock22.MainPilot().Morale > prev_value)
                                {
                                    is_useful = true;
                                }
                            }

                            break;
                        }

                    case "装填":
                        {
                            {
                                var withBlock25 = t;
                                flag = false;
                                if (string.IsNullOrEmpty(edata))
                                {
                                    // 全ての武器の弾数を回復
                                    var loopTo12 = withBlock25.CountWeapon();
                                    for (j = 1; j <= loopTo12; j++)
                                    {
                                        if (withBlock25.Bullet(j) < withBlock25.MaxBullet(j))
                                        {
                                            withBlock25.BulletSupply();
                                            flag = true;
                                            break;
                                        }
                                    }

                                    // 弾数とアビリティ使用回数の同期を取る
                                    if (flag)
                                    {
                                        var loopTo13 = withBlock25.CountAbility();
                                        for (j = 1; j <= loopTo13; j++)
                                        {
                                            string argattr9 = "共";
                                            if (withBlock25.IsAbilityClassifiedAs(j, argattr9))
                                            {
                                                var loopTo14 = withBlock25.CountWeapon();
                                                for (k = 1; k <= loopTo14; k++)
                                                {
                                                    string argattr6 = "共";
                                                    string argattr7 = "共";
                                                    string argattr8 = "共";
                                                    if (withBlock25.IsWeaponClassifiedAs(k, argattr6) & withBlock25.AbilityLevel(j, argattr7) == withBlock25.WeaponLevel(k, argattr8))
                                                    {
                                                        withBlock25.SetStock(j, withBlock25.MaxStock(j));
                                                    }
                                                }
                                            }
                                        }

                                        // 弾数・使用回数の共有化処理
                                        withBlock25.SyncBullet();
                                    }
                                }
                                else
                                {
                                    // 特定の武器の弾数のみを回復
                                    var loopTo15 = withBlock25.CountWeapon();
                                    for (j = 1; j <= loopTo15; j++)
                                    {
                                        if (withBlock25.Bullet(j) < withBlock25.MaxBullet(j))
                                        {
                                            if ((withBlock25.WeaponNickname(j) ?? "") == (edata ?? "") | GeneralLib.InStrNotNest(withBlock25.Weapon(j).Class_Renamed, edata) > 0)
                                            {
                                                withBlock25.SetBullet(j, withBlock25.MaxBullet(j));
                                                flag = true;
                                                w = j;
                                            }
                                        }
                                    }

                                    var loopTo16 = withBlock25.CountOtherForm();
                                    for (j = 1; j <= loopTo16; j++)
                                    {
                                        object argIndex24 = j;
                                        {
                                            var withBlock26 = withBlock25.OtherForm(argIndex24);
                                            var loopTo17 = withBlock26.CountWeapon();
                                            for (k = 1; k <= loopTo17; k++)
                                            {
                                                if (withBlock26.Bullet(k) < withBlock26.MaxBullet(k))
                                                {
                                                    if ((withBlock26.WeaponNickname(k) ?? "") == (edata ?? "") | GeneralLib.InStrNotNest(withBlock26.Weapon(k).Class_Renamed, edata) > 0)
                                                    {
                                                        withBlock26.SetBullet(k, withBlock26.MaxBullet(k));
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    // 弾数の同期を取る
                                    if (flag)
                                    {
                                        string argattr16 = "共";
                                        if (withBlock25.IsWeaponClassifiedAs(w, argattr16))
                                        {
                                            var loopTo18 = withBlock25.CountWeapon();
                                            for (j = 1; j <= loopTo18; j++)
                                            {
                                                string argattr10 = "共";
                                                string argattr11 = "共";
                                                string argattr12 = "共";
                                                if (withBlock25.IsWeaponClassifiedAs(j, argattr10) & withBlock25.WeaponLevel(j, argattr11) == withBlock25.WeaponLevel(w, argattr12))
                                                {
                                                    withBlock25.SetBullet(j, withBlock25.MaxBullet(j));
                                                }
                                            }

                                            var loopTo19 = withBlock25.CountAbility();
                                            for (j = 1; j <= loopTo19; j++)
                                            {
                                                string argattr13 = "共";
                                                string argattr14 = "共";
                                                string argattr15 = "共";
                                                if (withBlock25.IsAbilityClassifiedAs(j, argattr13) & withBlock25.AbilityLevel(j, argattr14) == withBlock25.WeaponLevel(w, argattr15))
                                                {
                                                    withBlock25.SetStock(j, withBlock25.MaxStock(j));
                                                }
                                            }
                                        }

                                        // 弾数・使用回数の共有化処理
                                        withBlock25.SyncBullet();
                                    }
                                }

                                if (flag)
                                {
                                    GUI.DisplaySysMessage(withBlock25.Nickname + "の武装の使用回数が回復した。");
                                    if (AbilityMaxRange(a) > 0)
                                    {
                                        is_useful = true;
                                    }
                                }
                            }

                            break;
                        }

                    case "治癒":
                        {
                            {
                                var withBlock27 = t;
                                if (!is_anime_played)
                                {
                                    string argattr17 = "魔";
                                    if (IsSpellAbility(a) | IsAbilityClassifiedAs(a, argattr17))
                                    {
                                        string arganame4 = "回復魔法発動";
                                        Effect.ShowAnimation(arganame4);
                                    }
                                }

                                if (string.IsNullOrEmpty(edata))
                                {
                                    // 全てのステータス異常を回復
                                    object argIndex26 = "攻撃不能";
                                    if (withBlock27.ConditionLifetime(argIndex26) > 0)
                                    {
                                        object argIndex25 = "攻撃不能";
                                        withBlock27.DeleteCondition(argIndex25);
                                        is_useful = true;
                                    }

                                    object argIndex28 = "移動不能";
                                    if (withBlock27.ConditionLifetime(argIndex28) > 0)
                                    {
                                        object argIndex27 = "移動不能";
                                        withBlock27.DeleteCondition(argIndex27);
                                        is_useful = true;
                                    }

                                    object argIndex30 = "装甲劣化";
                                    if (withBlock27.ConditionLifetime(argIndex30) > 0)
                                    {
                                        object argIndex29 = "装甲劣化";
                                        withBlock27.DeleteCondition(argIndex29);
                                        is_useful = true;
                                    }

                                    object argIndex32 = "混乱";
                                    if (withBlock27.ConditionLifetime(argIndex32) > 0)
                                    {
                                        object argIndex31 = "混乱";
                                        withBlock27.DeleteCondition(argIndex31);
                                        is_useful = true;
                                    }

                                    object argIndex34 = "恐怖";
                                    if (withBlock27.ConditionLifetime(argIndex34) > 0)
                                    {
                                        object argIndex33 = "恐怖";
                                        withBlock27.DeleteCondition(argIndex33);
                                        is_useful = true;
                                    }

                                    object argIndex36 = "踊り";
                                    if (withBlock27.ConditionLifetime(argIndex36) > 0)
                                    {
                                        object argIndex35 = "踊り";
                                        withBlock27.DeleteCondition(argIndex35);
                                        is_useful = true;
                                    }

                                    object argIndex38 = "狂戦士";
                                    if (withBlock27.ConditionLifetime(argIndex38) > 0)
                                    {
                                        object argIndex37 = "狂戦士";
                                        withBlock27.DeleteCondition(argIndex37);
                                        is_useful = true;
                                    }

                                    object argIndex40 = "ゾンビ";
                                    if (withBlock27.ConditionLifetime(argIndex40) > 0)
                                    {
                                        object argIndex39 = "ゾンビ";
                                        withBlock27.DeleteCondition(argIndex39);
                                        is_useful = true;
                                    }

                                    object argIndex42 = "回復不能";
                                    if (withBlock27.ConditionLifetime(argIndex42) > 0)
                                    {
                                        object argIndex41 = "回復不能";
                                        withBlock27.DeleteCondition(argIndex41);
                                        is_useful = true;
                                    }

                                    object argIndex44 = "石化";
                                    if (withBlock27.ConditionLifetime(argIndex44) > 0)
                                    {
                                        object argIndex43 = "石化";
                                        withBlock27.DeleteCondition(argIndex43);
                                        is_useful = true;
                                    }

                                    object argIndex46 = "凍結";
                                    if (withBlock27.ConditionLifetime(argIndex46) > 0)
                                    {
                                        object argIndex45 = "凍結";
                                        withBlock27.DeleteCondition(argIndex45);
                                        is_useful = true;
                                    }

                                    object argIndex48 = "麻痺";
                                    if (withBlock27.ConditionLifetime(argIndex48) > 0)
                                    {
                                        object argIndex47 = "麻痺";
                                        withBlock27.DeleteCondition(argIndex47);
                                        is_useful = true;
                                    }

                                    object argIndex50 = "睡眠";
                                    if (withBlock27.ConditionLifetime(argIndex50) > 0)
                                    {
                                        object argIndex49 = "睡眠";
                                        withBlock27.DeleteCondition(argIndex49);
                                        is_useful = true;
                                    }

                                    object argIndex52 = "毒";
                                    if (withBlock27.ConditionLifetime(argIndex52) > 0)
                                    {
                                        object argIndex51 = "毒";
                                        withBlock27.DeleteCondition(argIndex51);
                                        is_useful = true;
                                    }

                                    object argIndex54 = "盲目";
                                    if (withBlock27.ConditionLifetime(argIndex54) > 0)
                                    {
                                        object argIndex53 = "盲目";
                                        withBlock27.DeleteCondition(argIndex53);
                                        is_useful = true;
                                    }

                                    object argIndex56 = "沈黙";
                                    if (withBlock27.ConditionLifetime(argIndex56) > 0)
                                    {
                                        object argIndex55 = "沈黙";
                                        withBlock27.DeleteCondition(argIndex55);
                                        is_useful = true;
                                    }

                                    object argIndex58 = "魅了";
                                    if (withBlock27.ConditionLifetime(argIndex58) > 0)
                                    {
                                        object argIndex57 = "魅了";
                                        withBlock27.DeleteCondition(argIndex57);
                                        is_useful = true;
                                    }

                                    object argIndex60 = "憑依";
                                    if (withBlock27.ConditionLifetime(argIndex60) > 0)
                                    {
                                        object argIndex59 = "憑依";
                                        withBlock27.DeleteCondition(argIndex59);
                                        is_useful = true;
                                    }
                                    // 剋属性
                                    object argIndex62 = "オーラ使用不能";
                                    if (withBlock27.ConditionLifetime(argIndex62) > 0)
                                    {
                                        object argIndex61 = "オーラ使用不能";
                                        withBlock27.DeleteCondition(argIndex61);
                                    }

                                    object argIndex64 = "超能力使用不能";
                                    if (withBlock27.ConditionLifetime(argIndex64) > 0)
                                    {
                                        object argIndex63 = "超能力使用不能";
                                        withBlock27.DeleteCondition(argIndex63);
                                    }

                                    object argIndex66 = "同調率使用不能";
                                    if (withBlock27.ConditionLifetime(argIndex66) > 0)
                                    {
                                        object argIndex65 = "同調率使用不能";
                                        withBlock27.DeleteCondition(argIndex65);
                                    }

                                    object argIndex68 = "超感覚使用不能";
                                    if (withBlock27.ConditionLifetime(argIndex68) > 0)
                                    {
                                        object argIndex67 = "超感覚使用不能";
                                        withBlock27.DeleteCondition(argIndex67);
                                    }

                                    object argIndex70 = "知覚強化使用不能";
                                    if (withBlock27.ConditionLifetime(argIndex70) > 0)
                                    {
                                        object argIndex69 = "知覚強化使用不能";
                                        withBlock27.DeleteCondition(argIndex69);
                                    }

                                    object argIndex72 = "霊力使用不能";
                                    if (withBlock27.ConditionLifetime(argIndex72) > 0)
                                    {
                                        object argIndex71 = "霊力使用不能";
                                        withBlock27.DeleteCondition(argIndex71);
                                    }

                                    object argIndex74 = "術使用不能";
                                    if (withBlock27.ConditionLifetime(argIndex74) > 0)
                                    {
                                        object argIndex73 = "術使用不能";
                                        withBlock27.DeleteCondition(argIndex73);
                                    }

                                    object argIndex76 = "技使用不能";
                                    if (withBlock27.ConditionLifetime(argIndex76) > 0)
                                    {
                                        object argIndex75 = "技使用不能";
                                        withBlock27.DeleteCondition(argIndex75);
                                    }

                                    j = 1;
                                    while (j <= withBlock27.CountCondition())
                                    {
                                        // 弱点、有効付加はあえて外してあります。
                                        string localCondition5() { object argIndex1 = j; var ret = withBlock27.Condition(argIndex1); return ret; }

                                        string localCondition6() { object argIndex1 = j; var ret = withBlock27.Condition(argIndex1); return ret; }

                                        string localCondition7() { object argIndex1 = j; var ret = withBlock27.Condition(argIndex1); return ret; }

                                        int localConditionLifetime1() { object argIndex1 = (object)hs35eef8b33aab4975b1c788eecf306c48(); var ret = withBlock27.ConditionLifetime(argIndex1); return ret; }

                                        if (Strings.Len(localCondition5()) > 6 & Strings.Right(localCondition6(), 6) == "属性使用不能" & localConditionLifetime1() > 0)
                                        {
                                            string localCondition4() { object argIndex1 = j; var ret = withBlock27.Condition(argIndex1); return ret; }

                                            object argIndex77 = localCondition4();
                                            withBlock27.DeleteCondition(argIndex77);
                                            is_useful = true;
                                        }
                                        else
                                        {
                                            j = (j + 1);
                                        }
                                    }

                                    if (is_useful)
                                    {
                                        if (ReferenceEquals(t, CurrentForm()))
                                        {
                                            object argu222 = null;
                                            GUI.UpdateMessageForm(t, u2: argu222);
                                        }
                                        else
                                        {
                                            object argu223 = CurrentForm();
                                            GUI.UpdateMessageForm(t, argu223);
                                        }

                                        GUI.DisplaySysMessage(withBlock27.Nickname + "の状態が回復した。");
                                    }
                                }
                                else
                                {
                                    // 指定されたステータス異常のみを回復
                                    j = 1;
                                    while (j <= GeneralLib.LLength(edata))
                                    {
                                        cname = GeneralLib.LIndex(edata, j);
                                        object argIndex79 = cname;
                                        if (withBlock27.ConditionLifetime(argIndex79) > 0)
                                        {
                                            object argIndex78 = cname;
                                            withBlock27.DeleteCondition(argIndex78);
                                            if (ReferenceEquals(t, CurrentForm()))
                                            {
                                                object argu224 = null;
                                                GUI.UpdateMessageForm(t, u2: argu224);
                                            }
                                            else
                                            {
                                                object argu225 = CurrentForm();
                                                GUI.UpdateMessageForm(t, argu225);
                                            }

                                            if (cname == "装甲劣化")
                                            {
                                                string argtname20 = "装甲";
                                                cname = Expression.Term(argtname20, t) + "劣化";
                                            }

                                            GUI.DisplaySysMessage(withBlock27.Nickname + "の[" + cname + "]が回復した。");
                                            is_useful = true;
                                        }
                                        else
                                        {
                                            j = (j + 1);
                                        }
                                    }
                                }
                            }

                            break;
                        }

                    case "付加":
                        {
                            {
                                var withBlock28 = t;
                                if (elevel2 == 0d)
                                {
                                    // レベル指定がない場合は付加が半永久的に持続
                                    elevel2 = 10000d;
                                }
                                else
                                {
                                    // そうでなければ最低１ターンは効果が持続
                                    elevel2 = GeneralLib.MaxLng(elevel2, 1);
                                }

                                // 効果時間が継続中？
                                object argIndex80 = GeneralLib.LIndex(edata, 1) + "付加";
                                if (withBlock28.IsConditionSatisfied(argIndex80))
                                {
                                    goto NextLoop;
                                }

                                ftype = GeneralLib.LIndex(edata, 1);
                                flevel = Conversions.ToDouble(GeneralLib.LIndex(edata, 2));
                                fdata = "";
                                var loopTo20 = GeneralLib.LLength(edata);
                                for (j = 3; j <= loopTo20; j++)
                                    fdata = fdata + GeneralLib.LIndex(edata, j) + " ";
                                fdata = Strings.Trim(fdata);
                                if (Strings.Left(fdata, 1) == "\"" & Strings.Right(fdata, 1) == "\"")
                                {
                                    fdata = Strings.Trim(Strings.Mid(fdata, 2, Strings.Len(fdata) - 2));
                                }

                                // エリアスが定義されている？
                                object argIndex82 = ftype;
                                if (SRC.ALDList.IsDefined(argIndex82))
                                {
                                    object argIndex81 = ftype;
                                    {
                                        var withBlock29 = SRC.ALDList.Item(argIndex81);
                                        var loopTo21 = withBlock29.Count;
                                        for (j = 1; j <= loopTo21; j++)
                                        {
                                            // エリアスの定義に従って特殊能力定義を置き換える
                                            ftype2 = withBlock29.get_AliasType(j);
                                            string localLIndex() { string arglist = withBlock29.get_AliasData(j); var ret = GeneralLib.LIndex(arglist, 1); withBlock29.get_AliasData(j) = arglist; return ret; }

                                            if (localLIndex() == "解説")
                                            {
                                                // 特殊能力の解説
                                                if (!string.IsNullOrEmpty(fdata))
                                                {
                                                    ftype2 = GeneralLib.LIndex(fdata, 1);
                                                }

                                                flevel2 = SRC.DEFAULT_LEVEL;
                                                fdata2 = withBlock29.get_AliasData(j);
                                            }
                                            else
                                            {
                                                // 通常の特殊能力
                                                if (withBlock29.get_AliasLevelIsPlusMod(j))
                                                {
                                                    if (flevel == SRC.DEFAULT_LEVEL)
                                                    {
                                                        flevel = 1d;
                                                    }

                                                    flevel2 = flevel + withBlock29.get_AliasLevel(j);
                                                }
                                                else if (withBlock29.get_AliasLevelIsMultMod(j))
                                                {
                                                    if (flevel == SRC.DEFAULT_LEVEL)
                                                    {
                                                        flevel = 1d;
                                                    }

                                                    flevel2 = flevel * withBlock29.get_AliasLevel(j);
                                                }
                                                else if (flevel != SRC.DEFAULT_LEVEL)
                                                {
                                                    flevel2 = flevel;
                                                }
                                                else
                                                {
                                                    flevel2 = withBlock29.get_AliasLevel(j);
                                                }

                                                fdata2 = withBlock29.get_AliasData(j);
                                                if (!string.IsNullOrEmpty(fdata))
                                                {
                                                    if (Strings.InStr(fdata2, "非表示") != 1)
                                                    {
                                                        fdata2 = fdata + " " + GeneralLib.ListTail(fdata2, (GeneralLib.LLength(fdata) + 1));
                                                    }
                                                }
                                            }

                                            string argcname = ftype2 + "付加";
                                            t.AddCondition(argcname, elevel2, flevel2, fdata2);
                                        }
                                    }
                                }
                                else
                                {
                                    string argcname1 = ftype + "付加";
                                    withBlock28.AddCondition(argcname1, elevel2, flevel, fdata);
                                }

                                withBlock28.Update();
                                if (ReferenceEquals(t, CurrentForm()))
                                {
                                    object argu226 = null;
                                    GUI.UpdateMessageForm(t, u2: argu226);
                                }
                                else
                                {
                                    object argu227 = CurrentForm();
                                    GUI.UpdateMessageForm(t, argu227);
                                }

                                switch (GeneralLib.LIndex(edata, 1) ?? "")
                                {
                                    case "耐性":
                                    case "無効化":
                                    case "吸収":
                                        {
                                            GUI.DisplaySysMessage(withBlock28.Nickname + "は[" + GeneralLib.LIndex(edata, 3) + "]属性に対する[" + GeneralLib.LIndex(edata, 1) + "]能力を得た。");
                                            break;
                                        }

                                    case "特殊効果無効化":
                                        {
                                            GUI.DisplaySysMessage(withBlock28.Nickname + "は[" + GeneralLib.LIndex(edata, 3) + "]属性に対する無効化能力を得た。");
                                            break;
                                        }

                                    case "攻撃属性":
                                        {
                                            GUI.DisplaySysMessage(withBlock28.Nickname + "は[" + GeneralLib.LIndex(edata, 3) + "]の攻撃属性を得た。");
                                            break;
                                        }

                                    case "武器強化":
                                        {
                                            GUI.DisplaySysMessage(withBlock28.Nickname + "の" + "武器の攻撃力が上がった。");
                                            break;
                                        }

                                    case "命中率強化":
                                        {
                                            GUI.DisplaySysMessage(withBlock28.Nickname + "の" + "武器の命中率が上がった。");
                                            break;
                                        }

                                    case "ＣＴ率強化":
                                        {
                                            GUI.DisplaySysMessage(withBlock28.Nickname + "の" + "武器のＣＴ率が上がった。");
                                            break;
                                        }

                                    case "特殊効果発動率強化":
                                        {
                                            GUI.DisplaySysMessage(withBlock28.Nickname + "の" + "武器の特殊効果発動率が上がった。");
                                            break;
                                        }

                                    case "射程延長":
                                        {
                                            GUI.DisplaySysMessage(withBlock28.Nickname + "の" + "武器の射程が伸びた。");
                                            break;
                                        }

                                    case "サイズ変更":
                                        {
                                            GUI.DisplaySysMessage(withBlock28.Nickname + "の" + "サイズが" + Strings.StrConv(GeneralLib.LIndex(edata, 3), VbStrConv.Wide) + "サイズに変化した。");
                                            break;
                                        }
                                    // メッセージを表示しない。
                                    case "パイロット愛称":
                                    case "パイロット画像":
                                    case "愛称変更":
                                    case "ユニット画像":
                                    case "ＢＧＭ":
                                        {
                                            break;
                                        }

                                    default:
                                        {
                                            // 付加する能力名
                                            fname = GeneralLib.ListIndex(fdata, 1);
                                            if (string.IsNullOrEmpty(fname) | fname == "非表示")
                                            {
                                                if ((GeneralLib.LIndex(edata, 2) ?? "") != (Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SRC.DEFAULT_LEVEL) ?? ""))
                                                {
                                                    fname = GeneralLib.LIndex(edata, 1) + "Lv" + GeneralLib.LIndex(edata, 2);
                                                }
                                                else
                                                {
                                                    fname = GeneralLib.LIndex(edata, 1);
                                                }
                                            }

                                            GUI.DisplaySysMessage(withBlock28.Nickname + "は[" + fname + "]の能力を得た。");
                                            break;
                                        }
                                }

                                if (AbilityMaxRange(a) > 0)
                                {
                                    is_useful = true;
                                }
                            }

                            break;
                        }

                    case "強化":
                        {
                            {
                                var withBlock30 = t;
                                if (elevel2 == 0d)
                                {
                                    // レベル指定がない場合は付加が半永久的に持続
                                    elevel2 = 10000d;
                                }
                                else
                                {
                                    // そうでなければ最低１ターンは効果が持続
                                    elevel2 = GeneralLib.MaxLng(elevel2, 1);
                                }

                                // 効果時間が継続中？
                                object argIndex83 = GeneralLib.LIndex(edata, 1) + "強化";
                                if (withBlock30.IsConditionSatisfied(argIndex83))
                                {
                                    goto NextLoop;
                                }

                                ftype = GeneralLib.LIndex(edata, 1);
                                flevel = Conversions.ToDouble(GeneralLib.LIndex(edata, 2));
                                fdata = "";
                                var loopTo22 = GeneralLib.LLength(edata);
                                for (j = 3; j <= loopTo22; j++)
                                    fdata = fdata + GeneralLib.LIndex(edata, j) + " ";
                                fdata = Strings.Trim(fdata);

                                // エリアスが定義されている？
                                object argIndex85 = ftype;
                                if (SRC.ALDList.IsDefined(argIndex85))
                                {
                                    object argIndex84 = ftype;
                                    {
                                        var withBlock31 = SRC.ALDList.Item(argIndex84);
                                        var loopTo23 = withBlock31.Count;
                                        for (j = 1; j <= loopTo23; j++)
                                        {
                                            // エリアスの定義に従って特殊能力定義を置き換える
                                            ftype2 = withBlock31.get_AliasType(i);
                                            string localLIndex1() { string arglist = withBlock31.get_AliasData(j); var ret = GeneralLib.LIndex(arglist, 1); withBlock31.get_AliasData(j) = arglist; return ret; }

                                            if (localLIndex1() == "解説")
                                            {
                                                // 特殊能力の解説
                                                if (!string.IsNullOrEmpty(fdata))
                                                {
                                                    ftype2 = GeneralLib.LIndex(fdata, 1);
                                                }

                                                flevel2 = SRC.DEFAULT_LEVEL;
                                                fdata2 = withBlock31.get_AliasData(j);
                                                string argcname2 = ftype2 + "付加";
                                                t.AddCondition(argcname2, elevel2, flevel2, fdata2);
                                            }
                                            else
                                            {
                                                // 通常の特殊能力
                                                if (withBlock31.get_AliasLevelIsMultMod(j))
                                                {
                                                    if (flevel == SRC.DEFAULT_LEVEL)
                                                    {
                                                        flevel = 1d;
                                                    }

                                                    flevel2 = flevel * withBlock31.get_AliasLevel(j);
                                                }
                                                else if (flevel != SRC.DEFAULT_LEVEL)
                                                {
                                                    flevel2 = flevel;
                                                }
                                                else
                                                {
                                                    flevel2 = withBlock31.get_AliasLevel(j);
                                                }

                                                fdata2 = withBlock31.get_AliasData(j);
                                                if (!string.IsNullOrEmpty(fdata))
                                                {
                                                    if (Strings.InStr(fdata2, "非表示") != 1)
                                                    {
                                                        fdata2 = fdata + " " + GeneralLib.ListTail(fdata2, (GeneralLib.LLength(fdata) + 1));
                                                    }
                                                }

                                                string argcname3 = ftype2 + "強化";
                                                t.AddCondition(argcname3, elevel2, flevel2, fdata2);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    string argcname4 = ftype + "強化";
                                    withBlock30.AddCondition(argcname4, elevel2, flevel, fdata);
                                }

                                withBlock30.Update();
                                if (ReferenceEquals(t, CurrentForm()))
                                {
                                    object argu228 = null;
                                    GUI.UpdateMessageForm(t, u2: argu228);
                                }
                                else
                                {
                                    object argu229 = CurrentForm();
                                    GUI.UpdateMessageForm(t, argu229);
                                }

                                // 強化する能力名
                                fname = GeneralLib.LIndex(edata, 3);
                                if (string.IsNullOrEmpty(fname) | fname == "非表示")
                                {
                                    fname = GeneralLib.LIndex(edata, 1);
                                }

                                if (t.SkillName0(fname) != "非表示")
                                {
                                    fname = t.SkillName0(fname);
                                }

                                GUI.DisplaySysMessage(withBlock30.Nickname + "の[" + fname + "]レベルが" + GeneralLib.LIndex(edata, 2) + "上がった。");
                                if (AbilityMaxRange(a) > 0)
                                {
                                    is_useful = true;
                                }
                            }

                            break;
                        }

                    case "状態":
                        {
                            {
                                var withBlock32 = t;
                                if (elevel2 == 0d)
                                {
                                    // レベル指定がない場合は付加が半永久的に持続
                                    elevel2 = 10000d;
                                }
                                else
                                {
                                    // そうでなければ最低１ターンは状態が持続
                                    elevel = GeneralLib.MaxLng(elevel2, 1);
                                }

                                // 効果時間が継続中？
                                object argIndex86 = edata;
                                if (withBlock32.IsConditionSatisfied(argIndex86))
                                {
                                    goto NextLoop;
                                }

                                string argcdata = "";
                                withBlock32.AddCondition(edata, elevel2, cdata: argcdata);

                                // 状態発動アニメーション表示
                                bool localIsAnimationDefined() { string argmain_situation = aname + "(発動)"; string argsub_situation = ""; var ret = IsAnimationDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

                                string argsub_situation15 = "";
                                if (!localIsAnimationDefined() & !IsAnimationDefined(aname, sub_situation: argsub_situation15))
                                {
                                    switch (edata ?? "")
                                    {
                                        case "攻撃力ＵＰ":
                                        case "防御力ＵＰ":
                                        case "運動性ＵＰ":
                                        case "移動力ＵＰ":
                                        case "狂戦士":
                                            {
                                                string arganame5 = edata + "発動";
                                                Effect.ShowAnimation(arganame5);
                                                break;
                                            }
                                    }
                                }

                                switch (edata ?? "")
                                {
                                    case "装甲劣化":
                                        {
                                            string argtname21 = "装甲";
                                            cname = Expression.Term(argtname21, t) + "劣化";
                                            break;
                                        }

                                    case "運動性ＵＰ":
                                        {
                                            string argtname22 = "運動性";
                                            cname = Expression.Term(argtname22, t) + "ＵＰ";
                                            break;
                                        }

                                    case "運動性ＤＯＷＮ":
                                        {
                                            string argtname23 = "運動性";
                                            cname = Expression.Term(argtname23, t) + "ＤＯＷＮ";
                                            break;
                                        }

                                    case "移動力ＵＰ":
                                        {
                                            string argtname24 = "移動力";
                                            cname = Expression.Term(argtname24, t) + "ＵＰ";
                                            break;
                                        }

                                    case "移動力ＤＯＷＮ":
                                        {
                                            string argtname25 = "移動力";
                                            cname = Expression.Term(argtname25, t) + "ＤＯＷＮ";
                                            break;
                                        }

                                    default:
                                        {
                                            cname = edata;
                                            break;
                                        }
                                }

                                GUI.DisplaySysMessage(withBlock32.Nickname + "は" + cname + "の状態になった。");
                                if (AbilityMaxRange(a) > 0)
                                {
                                    is_useful = true;
                                }
                            }

                            break;
                        }

                    case "召喚":
                        {
                            object argu230 = null;
                            GUI.UpdateMessageForm(CurrentForm(), u2: argu230);
                            bool localIsDefined2() { object argIndex1 = edata; var ret = SRC.UDList.IsDefined(argIndex1); return ret; }

                            if (!localIsDefined2())
                            {
                                string argmsg13 = edata + "のデータが定義されていません";
                                GUI.ErrorMessage(argmsg13);
                                return ExecuteAbilityRet;
                            }

                            UnitData localItem() { object argIndex1 = edata; var ret = SRC.UDList.Item(argIndex1); return ret; }

                            object argIndex87 = "追加パイロット";
                            pname = localItem().FeatureData(argIndex87);
                            bool localIsDefined3() { object argIndex1 = pname; var ret = SRC.PDList.IsDefined(argIndex1); return ret; }

                            if (!localIsDefined3())
                            {
                                string argmsg14 = "追加パイロット「" + pname + "」のデータがありません";
                                GUI.ErrorMessage(argmsg14);
                                return ExecuteAbilityRet;
                            }

                            // 召喚したユニットを配置する座標を決定する。
                            // 最も近い敵ユニットの方向にユニットを配置する。
                            var argu = this;
                            u = COM.SearchNearestEnemy(argu);
                            if (u is object)
                            {
                                if (Math.Abs((x - u.x)) > Math.Abs((y - u.y)))
                                {
                                    if (x < u.x)
                                    {
                                        tx = (x + 1);
                                    }
                                    else if (x > u.x)
                                    {
                                        tx = (x - 1);
                                    }
                                    else
                                    {
                                        tx = x;
                                    }

                                    ty = y;
                                    tx2 = x;
                                    if (y < u.y)
                                    {
                                        ty2 = (y + 1);
                                    }
                                    else if (y > u.y)
                                    {
                                        ty2 = (y - 1);
                                    }
                                    else if (y == 1)
                                    {
                                        if (Map.MapDataForUnit[x, 2] is null)
                                        {
                                            ty2 = 2;
                                        }
                                        else
                                        {
                                            ty2 = 1;
                                        }
                                    }
                                    else if (y == Map.MapHeight)
                                    {
                                        if (Map.MapDataForUnit[x, Map.MapHeight - 1] is null)
                                        {
                                            ty2 = (Map.MapHeight - 1);
                                        }
                                        else
                                        {
                                            ty2 = Map.MapHeight;
                                        }
                                    }
                                    else if (Map.MapDataForUnit[x, y - 1] is null)
                                    {
                                        ty2 = (y - 1);
                                    }
                                    else if (Map.MapDataForUnit[x, y + 1] is null)
                                    {
                                        ty2 = (y - 1);
                                    }
                                    else
                                    {
                                        ty2 = y;
                                    }
                                }
                                else
                                {
                                    tx = x;
                                    if (y < u.y)
                                    {
                                        ty = (y + 1);
                                    }
                                    else if (y > u.y)
                                    {
                                        ty = (y - 1);
                                    }
                                    else
                                    {
                                        ty = y;
                                    }

                                    if (x < u.x)
                                    {
                                        tx2 = (x + 1);
                                    }
                                    else if (x > u.x)
                                    {
                                        tx2 = (x - 1);
                                    }
                                    else if (x == 1)
                                    {
                                        if (Map.MapDataForUnit[2, y] is null)
                                        {
                                            tx2 = 2;
                                        }
                                        else
                                        {
                                            tx2 = 1;
                                        }
                                    }
                                    else if (x == Map.MapWidth)
                                    {
                                        if (Map.MapDataForUnit[Map.MapWidth - 1, y] is null)
                                        {
                                            tx2 = (Map.MapWidth - 1);
                                        }
                                        else
                                        {
                                            tx2 = Map.MapWidth;
                                        }
                                    }
                                    else if (Map.MapDataForUnit[x - 1, y] is null)
                                    {
                                        tx2 = (x - 1);
                                    }
                                    else if (Map.MapDataForUnit[x + 1, y] is null)
                                    {
                                        tx2 = (x + 1);
                                    }
                                    else
                                    {
                                        tx2 = x;
                                    }

                                    ty2 = y;
                                }
                            }
                            else
                            {
                                tx = x;
                                ty = y;
                                tx2 = x;
                                ty2 = y;
                            }

                            var loopTo24 = GeneralLib.MaxLng(elevel, 1);
                            for (j = 1; j <= loopTo24; j++)
                            {
                                PilotData localItem1() { object argIndex1 = pname; var ret = SRC.PDList.Item(argIndex1); return ret; }

                                PilotData localItem2() { object argIndex1 = pname; var ret = SRC.PDList.Item(argIndex1); return ret; }

                                if (Strings.InStr(localItem1().Name, "(ザコ)") > 0 | Strings.InStr(localItem2().Name, "(汎用)") > 0)
                                {
                                    string argpparty1 = Party;
                                    string arggid1 = "";
                                    p = SRC.PList.Add(pname, MainPilot().Level, argpparty1, gid: arggid1);
                                    Party = argpparty1;
                                    p.FullRecover();
                                    string arguparty = Party;
                                    u = SRC.UList.Add(edata, Rank, arguparty);
                                    Party = arguparty;
                                }
                                else
                                {
                                    bool localIsDefined4() { object argIndex1 = pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    if (!localIsDefined4())
                                    {
                                        string argpparty2 = Party;
                                        string arggid2 = "";
                                        p = SRC.PList.Add(pname, MainPilot().Level, argpparty2, gid: arggid2);
                                        Party = argpparty2;
                                        p.FullRecover();
                                        string arguparty1 = Party;
                                        u = SRC.UList.Add(edata, Rank, arguparty1);
                                        Party = arguparty1;
                                    }
                                    else
                                    {
                                        object argIndex88 = pname;
                                        p = SRC.PList.Item(argIndex88);
                                        u = p.Unit_Renamed;
                                        if (u is null)
                                        {
                                            object argIndex90 = edata;
                                            if (SRC.UList.IsDefined(argIndex90))
                                            {
                                                object argIndex89 = edata;
                                                u = SRC.UList.Item(argIndex89);
                                            }
                                            else
                                            {
                                                string arguparty2 = Party;
                                                u = SRC.UList.Add(edata, Rank, arguparty2);
                                                Party = arguparty2;
                                            }
                                        }
                                    }
                                }

                                p.Ride(u);
                                AddServant(u);
                                if (Party == "味方")
                                {
                                    object argIndex91 = "召喚ユニット";
                                    string arglist = u.FeatureData(argIndex91);
                                    if (GeneralLib.LIndex(arglist, 2) == "ＮＰＣ")
                                    {
                                        string argnew_party = "ＮＰＣ";
                                        u.ChangeParty(argnew_party);
                                    }
                                }

                                u.Summoner = CurrentForm();
                                u.FullRecover();
                                u.Mode = MainPilot().ID;
                                u.UsedAction = 0;
                                string argfname4 = "制限時間";
                                if (u.IsFeatureAvailable(argfname4))
                                {
                                    string argcname5 = "残り時間";
                                    object argIndex92 = "制限時間";
                                    object argIndex93 = "制限時間";
                                    string argcdata1 = "";
                                    u.AddCondition(argcname5, Conversions.Toint(u.FeatureData(argIndex93)), cdata: argcdata1);
                                }

                                string argmain_situation12 = "発進";
                                if (u.IsMessageDefined(argmain_situation12))
                                {
                                    if (!My.MyProject.Forms.frmMessage.Visible)
                                    {
                                        var argu111 = this;
                                        Unit argu231 = null;
                                        GUI.OpenMessageForm(argu111, u2: argu231);
                                    }

                                    string argSituation1 = "発進";
                                    string argmsg_mode2 = "";
                                    u.PilotMessage(argSituation1, msg_mode: argmsg_mode2);
                                }

                                // ユニットを配置
                                if (Map.MapDataForUnit[tx, ty] is null & u.IsAbleToEnter(tx, ty))
                                {
                                    u.StandBy(tx, ty, "出撃");
                                }
                                else if (Map.MapDataForUnit[tx2, ty2] is null & u.IsAbleToEnter(tx2, ty2))
                                {
                                    u.StandBy(tx2, ty2, "出撃");
                                }
                                else
                                {
                                    u.StandBy(x, y, "出撃");
                                }

                                // ちゃんと配置できた？
                                if (u.Status_Renamed == "待機")
                                {
                                    // 空いた場所がなく出撃出来なかった場合
                                    GUI.DisplaySysMessage(Nickname + "は" + u.Nickname + "の召喚に失敗した。");
                                    object argIndex94 = u.ID;
                                    DeleteServant(argIndex94);
                                    u.Status_Renamed = "破棄";
                                }
                            }

                            break;
                        }

                    case "変身":
                        {
                            // 既に変身している場合は変身出来ない
                            string argfname5 = "ノーマルモード";
                            if (t.IsFeatureAvailable(argfname5))
                            {
                                goto NextLoop;
                            }

                            buf = t.Name;
                            string argnew_form = GeneralLib.LIndex(edata, 1);
                            t.Transform(argnew_form);
                            t = t.CurrentForm();
                            if (elevel2 > 0d)
                            {
                                string argcname6 = "残り時間";
                                string argcdata2 = "";
                                t.AddCondition(argcname6, GeneralLib.MaxLng(elevel2, 1), cdata: argcdata2);
                            }

                            var loopTo25 = GeneralLib.LLength(edata);
                            for (j = 2; j <= loopTo25; j++)
                                buf = buf + " " + GeneralLib.LIndex(edata, j);
                            string argcname7 = "ノーマルモード付加";
                            t.AddCondition(argcname7, -1, 1d, buf);

                            // 変身した場合はそこで終わり
                            break;
                        }

                    case "能力コピー":
                        {
                            // 既に変身している場合は能力コピー出来ない
                            string argfname6 = "ノーマルモード";
                            if (IsFeatureAvailable(argfname6))
                            {
                                goto NextLoop;
                            }

                            string argnew_form1 = t.Name;
                            Transform(argnew_form1);
                            t.Name = argnew_form1;
                            {
                                var withBlock33 = CurrentForm();
                                if (elevel2 > 0d)
                                {
                                    string argcname8 = "残り時間";
                                    string argcdata3 = "";
                                    withBlock33.AddCondition(argcname8, GeneralLib.MaxLng(elevel2, 1), cdata: argcdata3);
                                }

                                // 元の形態に戻れるように設定
                                buf = Name;
                                var loopTo26 = GeneralLib.LLength(edata);
                                for (j = 1; j <= loopTo26; j++)
                                    buf = buf + " " + GeneralLib.LIndex(edata, j);
                                string argcname9 = "ノーマルモード付加";
                                withBlock33.AddCondition(argcname9, -1, 1d, buf);
                                string argcname10 = "能力コピー";
                                string argcdata4 = "";
                                withBlock33.AddCondition(argcname10, -1, cdata: argcdata4);

                                // コピー元のパイロット画像とメッセージを使うように設定
                                string argcname11 = "パイロット画像";
                                string argcdata5 = "非表示 " + t.MainPilot().get_Bitmap(false);
                                withBlock33.AddCondition(argcname11, -1, 0d, argcdata5);
                                string argcname12 = "メッセージ";
                                string argcdata6 = "非表示 " + t.MainPilot().MessageType;
                                withBlock33.AddCondition(argcname12, -1, 0d, argcdata6);
                            }

                            // 能力コピーした場合はそこで終わり
                            ExecuteAbilityRet = true;
                            Commands.RestoreSelections();
                            return ExecuteAbilityRet;
                        }

                    case "再行動":
                        {
                            if (!ReferenceEquals(t, CurrentForm()))
                            {
                                if (t.Action == 0 & t.MaxAction() > 0)
                                {
                                    if (t.UsedAction > t.MaxAction())
                                    {
                                        t.UsedAction = t.MaxAction();
                                    }

                                    t.UsedAction = (t.UsedAction - 1);
                                    GUI.DisplaySysMessage(t.Nickname + "を行動可能にした。");
                                    is_useful = true;
                                }
                            }
                            else
                            {
                                t.UsedAction = (t.UsedAction - 1);
                            }

                            break;
                        }
                }

            NextLoop:
                ;
            }

            t.CurrentForm().Update();
            t.CurrentForm().CheckAutoHyperMode();
            t.CurrentForm().CheckAutoNormalMode();
            ExecuteAbilityRet = is_useful;
        Finish:
            ;


            // 選択状況を復元
            Commands.RestoreSelections();

            // マップアビリティの場合、これ以降の処理は必要なし
            if (is_map_ability)
            {
                return ExecuteAbilityRet;
            }

            // 合体技のパートナーの弾数＆ＥＮの消費
            var loopTo27 = Information.UBound(partners);
            for (i = 1; i <= loopTo27; i++)
            {
                {
                    var withBlock34 = partners[i].CurrentForm();
                    var loopTo28 = withBlock34.CountAbility();
                    for (j = 1; j <= loopTo28; j++)
                    {
                        // パートナーが同名のアビリティを持っていればそのアビリティのデータを使う
                        if ((withBlock34.Ability(j).Name ?? "") == (aname ?? ""))
                        {
                            withBlock34.UseAbility(j);
                            string argattr18 = "自";
                            string argattr19 = "失";
                            string argattr20 = "変";
                            if (withBlock34.IsAbilityClassifiedAs(j, argattr18))
                            {
                                string argfname7 = "パーツ分離";
                                if (withBlock34.IsFeatureAvailable(argfname7))
                                {
                                    object argIndex96 = "パーツ分離";
                                    string arglist1 = withBlock34.FeatureData(argIndex96);
                                    uname = GeneralLib.LIndex(arglist1, 2);
                                    Unit localOtherForm() { object argIndex1 = uname; var ret = withBlock34.OtherForm(argIndex1); return ret; }

                                    if (localOtherForm().IsAbleToEnter(withBlock34.x, withBlock34.y))
                                    {
                                        withBlock34.Transform(uname);
                                        {
                                            var withBlock35 = withBlock34.CurrentForm();
                                            withBlock35.HP = withBlock35.MaxHP;
                                            withBlock35.UsedAction = withBlock35.MaxAction();
                                        }
                                    }
                                    else
                                    {
                                        withBlock34.Die();
                                    }
                                }
                                else
                                {
                                    withBlock34.Die();
                                }
                            }
                            else if (withBlock34.IsAbilityClassifiedAs(j, argattr19) & withBlock34.HP == 0)
                            {
                                withBlock34.Die();
                            }
                            else if (withBlock34.IsAbilityClassifiedAs(j, argattr20))
                            {
                                string argfname9 = "変形技";
                                string argfname10 = "ノーマルモード";
                                if (withBlock34.IsFeatureAvailable(argfname9))
                                {
                                    var loopTo29 = withBlock34.CountFeature();
                                    for (k = 1; k <= loopTo29; k++)
                                    {
                                        string localFeature() { object argIndex1 = k; var ret = withBlock34.Feature(argIndex1); return ret; }

                                        string localFeatureData1() { object argIndex1 = k; var ret = withBlock34.FeatureData(argIndex1); return ret; }

                                        string localLIndex2() { string arglist = hsd94f2b67de0b4586a4a3a3d57d84bb20(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

                                        if (localFeature() == "変形技" & (localLIndex2() ?? "") == (aname ?? ""))
                                        {
                                            string localFeatureData() { object argIndex1 = k; var ret = withBlock34.FeatureData(argIndex1); return ret; }

                                            string arglist2 = localFeatureData();
                                            uname = GeneralLib.LIndex(arglist2, 2);
                                            Unit localOtherForm1() { object argIndex1 = uname; var ret = withBlock34.OtherForm(argIndex1); return ret; }

                                            if (localOtherForm1().IsAbleToEnter(withBlock34.x, withBlock34.y))
                                            {
                                                withBlock34.Transform(uname);
                                            }

                                            break;
                                        }
                                    }

                                    if ((uname ?? "") != (withBlock34.CurrentForm().Name ?? ""))
                                    {
                                        string argfname8 = "ノーマルモード";
                                        if (withBlock34.IsFeatureAvailable(argfname8))
                                        {
                                            object argIndex97 = "ノーマルモード";
                                            string arglist3 = withBlock34.FeatureData(argIndex97);
                                            uname = GeneralLib.LIndex(arglist3, 1);
                                            Unit localOtherForm2() { object argIndex1 = uname; var ret = withBlock34.OtherForm(argIndex1); return ret; }

                                            if (localOtherForm2().IsAbleToEnter(withBlock34.x, withBlock34.y))
                                            {
                                                withBlock34.Transform(uname);
                                            }
                                        }
                                    }
                                }
                                else if (withBlock34.IsFeatureAvailable(argfname10))
                                {
                                    object argIndex98 = "ノーマルモード";
                                    string arglist4 = withBlock34.FeatureData(argIndex98);
                                    uname = GeneralLib.LIndex(arglist4, 1);
                                    Unit localOtherForm3() { object argIndex1 = uname; var ret = withBlock34.OtherForm(argIndex1); return ret; }

                                    if (localOtherForm3().IsAbleToEnter(withBlock34.x, withBlock34.y))
                                    {
                                        withBlock34.Transform(uname);
                                    }
                                }
                            }

                            break;
                        }
                    }

                    // 同名のアビリティがなかった場合は自分のデータを使って処理
                    if (j > withBlock34.CountAbility())
                    {
                        if (this.Ability(a).ENConsumption > 0)
                        {
                            withBlock34.EN = withBlock34.EN - AbilityENConsumption(a);
                        }

                        string argattr21 = "消";
                        if (IsAbilityClassifiedAs(a, argattr21))
                        {
                            string argcname13 = "消耗";
                            string argcdata7 = "";
                            withBlock34.AddCondition(argcname13, 1, cdata: argcdata7);
                        }

                        string argattr22 = "Ｃ";
                        object argIndex100 = "チャージ完了";
                        if (IsAbilityClassifiedAs(a, argattr22) & withBlock34.IsConditionSatisfied(argIndex100))
                        {
                            object argIndex99 = "チャージ完了";
                            withBlock34.DeleteCondition(argIndex99);
                        }

                        string argattr24 = "気";
                        if (IsAbilityClassifiedAs(a, argattr24))
                        {
                            string argattr23 = "気";
                            withBlock34.IncreaseMorale((-5 * AbilityLevel(a, argattr23)));
                        }

                        string argattr27 = "霊";
                        string argattr28 = "プ";
                        if (IsAbilityClassifiedAs(a, argattr27))
                        {
                            hp_ratio = 100 * withBlock34.HP / (double)withBlock34.MaxHP;
                            en_ratio = 100 * withBlock34.EN / (double)withBlock34.MaxEN;
                            string argattr25 = "霊";
                            withBlock34.MainPilot().Plana = (withBlock34.MainPilot().Plana - 5d * AbilityLevel(a, argattr25));
                            withBlock34.HP = (withBlock34.MaxHP * hp_ratio / 100d);
                            withBlock34.EN = (withBlock34.MaxEN * en_ratio / 100d);
                        }
                        else if (IsAbilityClassifiedAs(a, argattr28))
                        {
                            hp_ratio = 100 * withBlock34.HP / (double)withBlock34.MaxHP;
                            en_ratio = 100 * withBlock34.EN / (double)withBlock34.MaxEN;
                            string argattr26 = "プ";
                            withBlock34.MainPilot().Plana = (withBlock34.MainPilot().Plana - 5d * AbilityLevel(a, argattr26));
                            withBlock34.HP = (withBlock34.MaxHP * hp_ratio / 100d);
                            withBlock34.EN = (withBlock34.MaxEN * en_ratio / 100d);
                        }

                        string argattr30 = "失";
                        if (IsAbilityClassifiedAs(a, argattr30))
                        {
                            string argattr29 = "失";
                            withBlock34.HP = GeneralLib.MaxLng((withBlock34.HP - (long)(withBlock34.MaxHP * AbilityLevel(a, argattr29)) / 10L), 0);
                        }

                        string argattr31 = "自";
                        string argattr32 = "失";
                        string argattr33 = "変";
                        if (IsAbilityClassifiedAs(a, argattr31))
                        {
                            string argfname11 = "パーツ分離";
                            if (withBlock34.IsFeatureAvailable(argfname11))
                            {
                                object argIndex101 = "パーツ分離";
                                string arglist5 = withBlock34.FeatureData(argIndex101);
                                uname = GeneralLib.LIndex(arglist5, 2);
                                Unit localOtherForm4() { object argIndex1 = uname; var ret = withBlock34.OtherForm(argIndex1); return ret; }

                                if (localOtherForm4().IsAbleToEnter(withBlock34.x, withBlock34.y))
                                {
                                    withBlock34.Transform(uname);
                                    {
                                        var withBlock36 = withBlock34.CurrentForm();
                                        withBlock36.HP = withBlock36.MaxHP;
                                        withBlock36.UsedAction = withBlock36.MaxAction();
                                    }
                                }
                                else
                                {
                                    withBlock34.Die();
                                }
                            }
                            else
                            {
                                withBlock34.Die();
                            }
                        }
                        else if (IsAbilityClassifiedAs(a, argattr32) & withBlock34.HP == 0)
                        {
                            withBlock34.Die();
                        }
                        else if (IsAbilityClassifiedAs(a, argattr33))
                        {
                            string argfname12 = "ノーマルモード";
                            if (withBlock34.IsFeatureAvailable(argfname12))
                            {
                                object argIndex102 = "ノーマルモード";
                                string arglist6 = withBlock34.FeatureData(argIndex102);
                                uname = GeneralLib.LIndex(arglist6, 1);
                                Unit localOtherForm5() { object argIndex1 = uname; var ret = withBlock34.OtherForm(argIndex1); return ret; }

                                if (localOtherForm5().IsAbleToEnter(withBlock34.x, withBlock34.y))
                                {
                                    withBlock34.Transform(uname);
                                }
                            }
                        }
                    }
                }
            }

            // 変身した場合
            if (Status_Renamed == "他形態")
            {
                {
                    var withBlock37 = CurrentForm();
                    // 使い捨てアイテムによる変身の処理
                    var loopTo30 = withBlock37.CountAbility();
                    for (i = 1; i <= loopTo30; i++)
                    {
                        if ((withBlock37.Ability(i).Name ?? "") == (aname ?? ""))
                        {
                            // アイテムを消費
                            if (withBlock37.Ability(i).IsItem() & withBlock37.Stock(i) == 0 & withBlock37.MaxStock(i) > 0)
                            {
                                var loopTo31 = withBlock37.CountItem();
                                for (j = 1; j <= loopTo31; j++)
                                {
                                    Item localItem5() { object argIndex1 = j; var ret = withBlock37.Item(argIndex1); return ret; }

                                    var loopTo32 = localItem5().CountAbility();
                                    for (k = 1; k <= loopTo32; k++)
                                    {
                                        Item localItem4() { object argIndex1 = j; var ret = withBlock37.Item(argIndex1); return ret; }

                                        AbilityData localAbility() { object argIndex1 = k; var ret = hs8bdb16b7368640769bb5144024b221c0().Ability(argIndex1); return ret; }

                                        if ((localAbility().Name ?? "") == (aname ?? ""))
                                        {
                                            Item localItem3() { object argIndex1 = j; var ret = withBlock37.Item(argIndex1); return ret; }

                                            localItem3().Exist = false;
                                            object argIndex103 = j;
                                            withBlock37.DeleteItem(argIndex103);
                                            withBlock37.Update();
                                            goto ExitLoop;
                                        }
                                    }
                                }
                            }
                        }
                    }

                ExitLoop:
                    ;


                    // 自殺？
                    if (withBlock37.HP == 0)
                    {
                        withBlock37.Die();
                    }
                }

                // WaitCommandによる画面クリアが行われないので
                GUI.RedrawScreen();
                return ExecuteAbilityRet;
            }

            // 経験値の獲得
            string argoname1 = "アビリティ経験値無効";
            if (is_useful & !is_event & !Expression.IsOptionDefined(argoname1))
            {
                string argexp_situation = "アビリティ";
                string argexp_mode = "";
                GetExp(t, argexp_situation, exp_mode: argexp_mode);
                string argoname = "合体技パートナー経験値無効";
                if (!Expression.IsOptionDefined(argoname))
                {
                    var loopTo33 = Information.UBound(partners);
                    for (i = 1; i <= loopTo33; i++)
                    {
                        string argexp_situation1 = "アビリティ";
                        string argexp_mode1 = "パートナー";
                        partners[i].CurrentForm().GetExp(t, argexp_situation1, argexp_mode1);
                    }
                }
            }

            // 以下の効果はアビリティデータが変化する場合があるため同時には適応されない

            // 自爆技
            string argattr34 = "自";

            // ＨＰ消費アビリティで自殺
            string argattr35 = "失";

            // 変形技
            string argattr36 = "変";
            if (IsAbilityClassifiedAs(a, argattr34))
            {
                string argfname13 = "パーツ分離";
                if (IsFeatureAvailable(argfname13))
                {
                    object argIndex104 = "パーツ分離";
                    string arglist7 = FeatureData(argIndex104);
                    uname = GeneralLib.LIndex(arglist7, 2);
                    Unit localOtherForm6() { object argIndex1 = uname; var ret = OtherForm(argIndex1); return ret; }

                    if (localOtherForm6().IsAbleToEnter(x, y))
                    {
                        Transform(uname);
                        {
                            var withBlock38 = CurrentForm();
                            withBlock38.HP = withBlock38.MaxHP;
                            withBlock38.UsedAction = withBlock38.MaxAction();
                        }

                        object argIndex105 = "パーツ分離";
                        fname = FeatureName(argIndex105);
                        bool localIsSysMessageDefined() { string argmain_situation = "破壊時分離(" + fname + ")"; string argsub_situation = ""; var ret = IsSysMessageDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

                        bool localIsSysMessageDefined1() { string argmain_situation = "分離(" + Name + ")"; string argsub_situation = ""; var ret = IsSysMessageDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

                        bool localIsSysMessageDefined2() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = IsSysMessageDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

                        string argmain_situation19 = "破壊時分離(" + Name + ")";
                        string argsub_situation22 = "";
                        string argmain_situation20 = "破壊時分離";
                        string argsub_situation23 = "";
                        string argmain_situation21 = "分離";
                        string argsub_situation24 = "";
                        if (IsSysMessageDefined(argmain_situation19, sub_situation: argsub_situation22))
                        {
                            string argmain_situation13 = "破壊時分離(" + Name + ")";
                            string argsub_situation16 = "";
                            string argadd_msg2 = "";
                            SysMessage(argmain_situation13, sub_situation: argsub_situation16, add_msg: argadd_msg2);
                        }
                        else if (localIsSysMessageDefined())
                        {
                            string argmain_situation14 = "破壊時分離(" + fname + ")";
                            string argsub_situation17 = "";
                            string argadd_msg3 = "";
                            SysMessage(argmain_situation14, sub_situation: argsub_situation17, add_msg: argadd_msg3);
                        }
                        else if (IsSysMessageDefined(argmain_situation20, sub_situation: argsub_situation23))
                        {
                            string argmain_situation15 = "破壊時分離";
                            string argsub_situation18 = "";
                            string argadd_msg4 = "";
                            SysMessage(argmain_situation15, sub_situation: argsub_situation18, add_msg: argadd_msg4);
                        }
                        else if (localIsSysMessageDefined1())
                        {
                            string argmain_situation16 = "分離(" + Name + ")";
                            string argsub_situation19 = "";
                            string argadd_msg5 = "";
                            SysMessage(argmain_situation16, sub_situation: argsub_situation19, add_msg: argadd_msg5);
                        }
                        else if (localIsSysMessageDefined2())
                        {
                            string argmain_situation17 = "分離(" + fname + ")";
                            string argsub_situation20 = "";
                            string argadd_msg6 = "";
                            SysMessage(argmain_situation17, sub_situation: argsub_situation20, add_msg: argadd_msg6);
                        }
                        else if (IsSysMessageDefined(argmain_situation21, sub_situation: argsub_situation24))
                        {
                            string argmain_situation18 = "分離";
                            string argsub_situation21 = "";
                            string argadd_msg7 = "";
                            SysMessage(argmain_situation18, sub_situation: argsub_situation21, add_msg: argadd_msg7);
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
            else if (IsAbilityClassifiedAs(a, argattr35) & HP == 0)
            {
                Die();
            }
            else if (IsAbilityClassifiedAs(a, argattr36))
            {
                string argfname15 = "変形技";
                string argfname16 = "ノーマルモード";
                if (IsFeatureAvailable(argfname15))
                {
                    var loopTo34 = CountFeature();
                    for (i = 1; i <= loopTo34; i++)
                    {
                        string localFeature1() { object argIndex1 = i; var ret = Feature(argIndex1); return ret; }

                        string localFeatureData3() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

                        string localLIndex3() { string arglist = hs943d006232364b899ee9a8aea8dcca5a(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

                        if (localFeature1() == "変形技" & (localLIndex3() ?? "") == (Ability(a).Name ?? ""))
                        {
                            string localFeatureData2() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

                            string arglist8 = localFeatureData2();
                            uname = GeneralLib.LIndex(arglist8, 2);
                            Unit localOtherForm7() { object argIndex1 = uname; var ret = OtherForm(argIndex1); return ret; }

                            if (localOtherForm7().IsAbleToEnter(x, y))
                            {
                                Transform(uname);
                            }

                            break;
                        }
                    }

                    if ((uname ?? "") != (CurrentForm().Name ?? ""))
                    {
                        string argfname14 = "ノーマルモード";
                        if (IsFeatureAvailable(argfname14))
                        {
                            object argIndex106 = "ノーマルモード";
                            string arglist9 = FeatureData(argIndex106);
                            uname = GeneralLib.LIndex(arglist9, 1);
                            Unit localOtherForm8() { object argIndex1 = uname; var ret = OtherForm(argIndex1); return ret; }

                            if (localOtherForm8().IsAbleToEnter(x, y))
                            {
                                Transform(uname);
                            }
                        }
                    }
                }
                else if (IsFeatureAvailable(argfname16))
                {
                    object argIndex107 = "ノーマルモード";
                    string arglist10 = FeatureData(argIndex107);
                    uname = GeneralLib.LIndex(arglist10, 1);
                    Unit localOtherForm9() { object argIndex1 = uname; var ret = OtherForm(argIndex1); return ret; }

                    if (localOtherForm9().IsAbleToEnter(x, y))
                    {
                        Transform(uname);
                    }
                }
            }

            // アイテムを消費
            else if (Ability(a).IsItem() & Stock(a) == 0 & MaxStock(a) > 0)
            {
                // アイテムを削除
                num = Data.CountAbility();
                num = (num + MainPilot().Data.CountAbility());
                var loopTo35 = CountPilot();
                for (i = 2; i <= loopTo35; i++)
                {
                    Pilot localPilot() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

                    num = (num + localPilot().Data.CountAbility());
                }

                var loopTo36 = CountSupport();
                for (i = 2; i <= loopTo36; i++)
                {
                    Pilot localSupport() { object argIndex1 = i; var ret = Support(argIndex1); return ret; }

                    num = (num + localSupport().Data.CountAbility());
                }

                string argfname17 = "追加サポート";
                if (IsFeatureAvailable(argfname17))
                {
                    num = (num + AdditionalSupport().Data.CountAbility());
                }

                foreach (Item itm in colItem)
                {
                    num = (num + itm.CountAbility());
                    if (a <= num)
                    {
                        itm.Exist = false;
                        DeleteItem((object)itm.ID);
                        break;
                    }
                }
            }

            // ADD START MARGE
            // 戦闘アニメ終了処理
            string argmain_situation24 = aname + "(終了)";
            string argsub_situation27 = "";
            string argmain_situation25 = "終了";
            string argsub_situation28 = "";
            if (IsAnimationDefined(argmain_situation24, sub_situation: argsub_situation27))
            {
                string argmain_situation22 = aname + "(終了)";
                string argsub_situation25 = "";
                PlayAnimation(argmain_situation22, sub_situation: argsub_situation25);
            }
            else if (IsAnimationDefined(argmain_situation25, sub_situation: argsub_situation28))
            {
                string argmain_situation23 = "終了";
                string argsub_situation26 = "";
                PlayAnimation(argmain_situation23, sub_situation: argsub_situation26);
            }
            // ADD END MARGE

            {
                var withBlock39 = CurrentForm();
                // 戦闘アニメで変更されたユニット画像を元に戻す
                object argIndex109 = "ユニット画像";
                if (withBlock39.IsConditionSatisfied(argIndex109))
                {
                    object argIndex108 = "ユニット画像";
                    withBlock39.DeleteCondition(argIndex108);
                    withBlock39.BitmapID = GUI.MakeUnitBitmap(CurrentForm());
                    GUI.PaintUnitBitmap(CurrentForm());
                }

                object argIndex111 = "非表示付加";
                if (withBlock39.IsConditionSatisfied(argIndex111))
                {
                    object argIndex110 = "非表示付加";
                    withBlock39.DeleteCondition(argIndex110);
                    withBlock39.BitmapID = GUI.MakeUnitBitmap(CurrentForm());
                    GUI.PaintUnitBitmap(CurrentForm());
                }
            }

            var loopTo37 = Information.UBound(partners);
            for (i = 1; i <= loopTo37; i++)
            {
                {
                    var withBlock40 = partners[i].CurrentForm();
                    object argIndex113 = "ユニット画像";
                    if (withBlock40.IsConditionSatisfied(argIndex113))
                    {
                        object argIndex112 = "ユニット画像";
                        withBlock40.DeleteCondition(argIndex112);
                        withBlock40.BitmapID = GUI.MakeUnitBitmap(partners[i].CurrentForm());
                        GUI.PaintUnitBitmap(partners[i].CurrentForm());
                    }

                    object argIndex115 = "非表示付加";
                    if (withBlock40.IsConditionSatisfied(argIndex115))
                    {
                        object argIndex114 = "非表示付加";
                        withBlock40.DeleteCondition(argIndex114);
                        withBlock40.BitmapID = GUI.MakeUnitBitmap(partners[i].CurrentForm());
                        GUI.PaintUnitBitmap(partners[i].CurrentForm());
                    }
                }
            }

            return ExecuteAbilityRet;
        }

        // マップアビリティ a を (tx,ty) に使用
        public void ExecuteMapAbility(int a, int tx, int ty, bool is_event = false)
        {
            int k, i, j, num;
            Unit t, max_lv_t;
            Unit[] targets;
            var partners = default(Unit[]);
            var is_useful = default(bool);
            string anickname, aname, msg;
            int min_range, max_range;
            int rx, ry;
            string uname = default, fname;
            double hp_ratio, en_ratio;
            aname = Ability(a).Name;
            anickname = AbilityNickname(a);
            if (!is_event)
            {
                // マップ攻撃の使用イベント
                Event_Renamed.HandleEvent("使用", MainPilot().ID, aname);
                if (SRC.IsScenarioFinished)
                {
                    return;
                }

                if (SRC.IsCanceled)
                {
                    SRC.IsCanceled = false;
                    return;
                }
            }

            // 効果範囲を設定
            min_range = AbilityMinRange(a);
            max_range = AbilityMaxRange(a);
            string argattr5 = "Ｍ直";
            string argattr6 = "Ｍ拡";
            string argattr7 = "Ｍ扇";
            string argattr8 = "Ｍ投";
            string argattr9 = "Ｍ全";
            string argattr10 = "Ｍ移";
            string argattr11 = "Ｍ線";
            if (IsAbilityClassifiedAs(a, argattr5))
            {
                if (ty < y)
                {
                    string argdirection = "N";
                    Map.AreaInLine(x, y, min_range, max_range, argdirection);
                }
                else if (ty > y)
                {
                    string argdirection2 = "S";
                    Map.AreaInLine(x, y, min_range, max_range, argdirection2);
                }
                else if (tx < x)
                {
                    string argdirection3 = "W";
                    Map.AreaInLine(x, y, min_range, max_range, argdirection3);
                }
                else
                {
                    string argdirection1 = "E";
                    Map.AreaInLine(x, y, min_range, max_range, argdirection1);
                }
            }
            else if (IsAbilityClassifiedAs(a, argattr6))
            {
                if (ty < y & Math.Abs((y - ty)) > Math.Abs((x - tx)))
                {
                    string argdirection4 = "N";
                    Map.AreaInCone(x, y, min_range, max_range, argdirection4);
                }
                else if (ty > y & Math.Abs((y - ty)) > Math.Abs((x - tx)))
                {
                    string argdirection6 = "S";
                    Map.AreaInCone(x, y, min_range, max_range, argdirection6);
                }
                else if (tx < x & Math.Abs((x - tx)) > Math.Abs((y - ty)))
                {
                    string argdirection7 = "W";
                    Map.AreaInCone(x, y, min_range, max_range, argdirection7);
                }
                else
                {
                    string argdirection5 = "E";
                    Map.AreaInCone(x, y, min_range, max_range, argdirection5);
                }
            }
            else if (IsAbilityClassifiedAs(a, argattr7))
            {
                if (ty < y & Math.Abs((y - ty)) >= Math.Abs((x - tx)))
                {
                    string argdirection8 = "N";
                    string argattr = "Ｍ扇";
                    Map.AreaInSector(x, y, min_range, max_range, argdirection8, AbilityLevel(a, argattr));
                }
                else if (ty > y & Math.Abs((y - ty)) >= Math.Abs((x - tx)))
                {
                    string argdirection10 = "S";
                    string argattr2 = "Ｍ扇";
                    Map.AreaInSector(x, y, min_range, max_range, argdirection10, AbilityLevel(a, argattr2));
                }
                else if (tx < x & Math.Abs((x - tx)) >= Math.Abs((y - ty)))
                {
                    string argdirection11 = "W";
                    string argattr3 = "Ｍ扇";
                    Map.AreaInSector(x, y, min_range, max_range, argdirection11, AbilityLevel(a, argattr3));
                }
                else
                {
                    string argdirection9 = "E";
                    string argattr1 = "Ｍ扇";
                    Map.AreaInSector(x, y, min_range, max_range, argdirection9, AbilityLevel(a, argattr1));
                }
            }
            else if (IsAbilityClassifiedAs(a, argattr8))
            {
                string argattr4 = "Ｍ投";
                string arguparty = "すべて";
                Map.AreaInRange(tx, ty, AbilityLevel(a, argattr4), 1, arguparty);
            }
            else if (IsAbilityClassifiedAs(a, argattr9))
            {
                string arguparty1 = "すべて";
                Map.AreaInRange(x, y, max_range, min_range, arguparty1);
            }
            else if (IsAbilityClassifiedAs(a, argattr10) | IsAbilityClassifiedAs(a, argattr11))
            {
                Map.AreaInPointToPoint(x, y, tx, ty);
            }

            // ユニットがいるマスの処理
            var loopTo = Map.MapWidth;
            for (i = 1; i <= loopTo; i++)
            {
                var loopTo1 = Map.MapHeight;
                for (j = 1; j <= loopTo1; j++)
                {
                    if (!Map.MaskData[i, j])
                    {
                        t = Map.MapDataForUnit[i, j];
                        if (t is object)
                        {
                            // 有効？
                            if (IsAbilityEffective(a, t))
                            {
                                Map.MaskData[i, j] = false;
                            }
                            else
                            {
                                Map.MaskData[i, j] = true;
                            }
                        }
                    }
                }
            }

            // 支援専用アビリティは自分には使用できない
            string argattr12 = "援";
            if (IsAbilityClassifiedAs(a, argattr12))
            {
                Map.MaskData[x, y] = true;
            }

            // マップアビリティの影響を受けるユニットのリストを作成
            targets = new Unit[1];
            var loopTo2 = Map.MapWidth;
            for (i = 1; i <= loopTo2; i++)
            {
                var loopTo3 = Map.MapHeight;
                for (j = 1; j <= loopTo3; j++)
                {
                    // マップアビリティの影響をうけるかチェック
                    if (Map.MaskData[i, j])
                    {
                        goto NextLoop;
                    }

                    t = Map.MapDataForUnit[i, j];
                    if (t is null)
                    {
                        goto NextLoop;
                    }

                    if (!IsAbilityApplicable(a, t))
                    {
                        Map.MaskData[i, j] = true;
                        goto NextLoop;
                    }

                    Array.Resize(targets, Information.UBound(targets) + 1 + 1);
                    targets[Information.UBound(targets)] = t;
                NextLoop:
                    ;
                }
            }

            // アビリティ実行の起点を設定
            string argattr13 = "Ｍ投";
            if (IsAbilityClassifiedAs(a, argattr13))
            {
                rx = tx;
                ry = ty;
            }
            else
            {
                rx = x;
                ry = y;
            }

            // 起点からの距離に応じて並べ替え
            int min_item, min_value;
            var loopTo4 = (Information.UBound(targets) - 1);
            for (i = 1; i <= loopTo4; i++)
            {
                min_item = i;
                {
                    var withBlock = targets[i];
                    min_value = (Math.Abs((withBlock.x - rx)) + Math.Abs((withBlock.y - ry)));
                }

                var loopTo5 = Information.UBound(targets);
                for (j = (i + 1); j <= loopTo5; j++)
                {
                    {
                        var withBlock1 = targets[j];
                        if ((Math.Abs((withBlock1.x - rx)) + Math.Abs((withBlock1.y - ry))) < min_value)
                        {
                            min_item = j;
                            min_value = (Math.Abs((withBlock1.x - rx)) + Math.Abs((withBlock1.y - ry)));
                        }
                    }
                }

                if (min_item != i)
                {
                    t = targets[i];
                    targets[i] = targets[min_item];
                    targets[min_item] = t;
                }
            }

            // 合体技
            bool[] TmpMaskData;
            string argattr14 = "合";
            if (IsAbilityClassifiedAs(a, argattr14))
            {

                // 合体技のパートナーのハイライト表示
                // MaskDataを保存して使用している
                TmpMaskData = new bool[(Map.MapWidth + 1), (Map.MapHeight + 1)];
                var loopTo6 = Map.MapWidth;
                for (i = 1; i <= loopTo6; i++)
                {
                    var loopTo7 = Map.MapHeight;
                    for (j = 1; j <= loopTo7; j++)
                        TmpMaskData[i, j] = Map.MaskData[i, j];
                }

                string argctype_Renamed = "アビリティ";
                CombinationPartner(argctype_Renamed, a, partners);

                // パートナーユニットはマスクを解除
                var loopTo8 = Information.UBound(partners);
                for (i = 1; i <= loopTo8; i++)
                {
                    {
                        var withBlock2 = partners[i];
                        Map.MaskData[withBlock2.x, withBlock2.y] = false;
                        TmpMaskData[withBlock2.x, withBlock2.y] = true;
                    }
                }

                GUI.MaskScreen();

                // マスクを復元
                var loopTo9 = Map.MapWidth;
                for (i = 1; i <= loopTo9; i++)
                {
                    var loopTo10 = Map.MapHeight;
                    for (j = 1; j <= loopTo10; j++)
                        Map.MaskData[i, j] = TmpMaskData[i, j];
                }
            }
            else
            {
                partners = new Unit[1];
                Commands.SelectedPartners = new Unit[1];
                GUI.MaskScreen();
            }

            var argu1 = this;
            Unit argu2 = null;
            GUI.OpenMessageForm(argu1, u2: argu2);

            // 現在の選択状況をセーブ
            Commands.SaveSelections();

            // 選択内容を切り替え
            Commands.SelectedUnit = this;
            Event_Renamed.SelectedUnitForEvent = this;
            Commands.SelectedAbility = a;
            Commands.SelectedAbilityName = Ability(a).Name;
            Commands.SelectedX = tx;
            Commands.SelectedY = ty;

            // 変な「対～」メッセージが表示されないようにターゲットをオフ
            // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Commands.SelectedTarget = null;
            // UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Event_Renamed.SelectedTargetForEvent = null;

            // マップアビリティ開始のメッセージ＆特殊効果
            string argmain_situation1 = aname + "(準備)";
            string argsub_situation1 = "";
            if (IsAnimationDefined(argmain_situation1, sub_situation: argsub_situation1))
            {
                string argmain_situation = aname + "(準備)";
                string argsub_situation = "";
                PlayAnimation(argmain_situation, sub_situation: argsub_situation);
            }

            string argmain_situation2 = "かけ声(" + aname + ")";
            if (IsMessageDefined(argmain_situation2))
            {
                string argSituation = "かけ声(" + aname + ")";
                string argmsg_mode = "";
                PilotMessage(argSituation, msg_mode: argmsg_mode);
            }

            string argmsg_mode1 = "アビリティ";
            PilotMessage(aname, argmsg_mode1);
            string argmain_situation4 = aname + "(使用)";
            string argsub_situation4 = "";
            if (IsAnimationDefined(argmain_situation4, sub_situation: argsub_situation4))
            {
                string argmain_situation3 = aname + "(使用)";
                string argsub_situation2 = "";
                PlayAnimation(argmain_situation3, argsub_situation2, true);
            }
            else
            {
                string argsub_situation3 = "";
                SpecialEffect(aname, argsub_situation3, true);
            }

            // ＥＮ消費＆使用回数減少
            UseAbility(a);
            var argu11 = this;
            object argu21 = null;
            GUI.UpdateMessageForm(argu11, u2: argu21);
            switch (Information.UBound(partners))
            {
                case 0:
                    {
                        // 通常
                        msg = Nickname + "は";
                        break;
                    }

                case 1:
                    {
                        // ２体合体
                        if ((Nickname ?? "") != (partners[1].Nickname ?? ""))
                        {
                            msg = Nickname + "は[" + partners[1].Nickname + "]と共に";
                        }
                        else if ((MainPilot().get_Nickname(false) ?? "") != (partners[1].MainPilot().get_Nickname(false) ?? ""))
                        {
                            msg = MainPilot().get_Nickname(false) + "と[" + partners[1].MainPilot().get_Nickname(false) + "]の[" + Nickname + "]は";
                        }
                        else
                        {
                            msg = Nickname + "達は";
                        }

                        break;
                    }

                case 2:
                    {
                        // ３体合体
                        if ((Nickname ?? "") != (partners[1].Nickname ?? ""))
                        {
                            msg = Nickname + "は[" + partners[1].Nickname + "]、[" + partners[2].Nickname + "]と共に";
                        }
                        else if ((MainPilot().get_Nickname(false) ?? "") != (partners[1].MainPilot().get_Nickname(false) ?? ""))
                        {
                            msg = MainPilot().get_Nickname(false) + "、[" + partners[1].MainPilot().get_Nickname(false) + "]、[" + partners[2].MainPilot().get_Nickname(false) + "]の[" + Nickname + "]は";
                        }
                        else
                        {
                            msg = Nickname + "達は";
                        }

                        break;
                    }

                default:
                    {
                        // ３体以上
                        msg = Nickname + "達は";
                        break;
                    }
            }

            if (IsSpellAbility(a))
            {
                if (Strings.Right(anickname, 2) == "呪文")
                {
                    msg = msg + "[" + anickname + "]を唱えた。";
                }
                else if (Strings.Right(anickname, 2) == "の杖")
                {
                    msg = msg + "[" + Strings.Left(anickname, Strings.Len(anickname) - 2) + "]の呪文を唱えた。";
                }
                else
                {
                    msg = msg + "[" + anickname + "]の呪文を唱えた。";
                }
            }
            else if (Strings.Right(anickname, 1) == "歌")
            {
                msg = msg + "[" + anickname + "]を歌った。";
            }
            else if (Strings.Right(anickname, 2) == "踊り")
            {
                msg = msg + "[" + anickname + "]を踊った。";
            }
            else
            {
                msg = msg + "[" + anickname + "]を使った。";
            }

            string argsub_situation7 = "";
            string argmain_situation6 = "アビリティ";
            string argsub_situation8 = "";
            if (IsSysMessageDefined(aname, sub_situation: argsub_situation7))
            {
                string argsub_situation5 = "";
                string argadd_msg = "";
                // 「アビリティ名(解説)」のメッセージを使用
                SysMessage(aname, sub_situation: argsub_situation5, add_msg: argadd_msg);
            }
            else if (IsSysMessageDefined(argmain_situation6, sub_situation: argsub_situation8))
            {
                // 「アビリティ(解説)」のメッセージを使用
                string argmain_situation5 = "アビリティ";
                string argsub_situation6 = "";
                string argadd_msg1 = "";
                SysMessage(argmain_situation5, sub_situation: argsub_situation6, add_msg: argadd_msg1);
            }
            else
            {
                GUI.DisplaySysMessage(msg);
            }

            // 選択状況を復元
            Commands.RestoreSelections();

            // アビリティの使用に失敗？
            string argattr15 = "難";
            if (GeneralLib.Dice(10) <= AbilityLevel(a, argattr15))
            {
                GUI.DisplaySysMessage("しかし何もおきなかった…");
                goto Finish;
            }

            // 使用元ユニットは SelectedTarget に設定していないといけない
            Commands.SelectedTarget = this;

            // 各ユニットにアビリティを使用
            // UPGRADE_NOTE: オブジェクト max_lv_t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            max_lv_t = null;
            var loopTo11 = Information.UBound(targets);
            for (i = 1; i <= loopTo11; i++)
            {
                t = targets[i].CurrentForm();
                if (t.Status_Renamed == "出撃")
                {
                    if (ReferenceEquals(t, this))
                    {
                        var argu12 = this;
                        object argu22 = null;
                        GUI.UpdateMessageForm(argu12, u2: argu22);
                    }
                    else
                    {
                        object argu23 = this;
                        GUI.UpdateMessageForm(t, argu23);
                    }

                    if (ExecuteAbility(a, t, true))
                    {
                        t = t.CurrentForm();
                        is_useful = true;

                        // 獲得経験値算出用にメインパイロットのレベルが最も高い
                        // ユニットを求めておく
                        if (max_lv_t is null)
                        {
                            max_lv_t = t;
                        }
                        else if (t.MainPilot().Level > max_lv_t.MainPilot().Level)
                        {
                            max_lv_t = t;
                        }
                    }
                }
            }

            // ADD START MARGE
            // 戦闘アニメ終了処理
            string argmain_situation9 = aname + "(終了)";
            string argsub_situation11 = "";
            string argmain_situation10 = "終了";
            string argsub_situation12 = "";
            if (IsAnimationDefined(argmain_situation9, sub_situation: argsub_situation11))
            {
                string argmain_situation7 = aname + "(終了)";
                string argsub_situation9 = "";
                PlayAnimation(argmain_situation7, sub_situation: argsub_situation9);
            }
            else if (IsAnimationDefined(argmain_situation10, sub_situation: argsub_situation12))
            {
                string argmain_situation8 = "終了";
                string argsub_situation10 = "";
                PlayAnimation(argmain_situation8, sub_situation: argsub_situation10);
            }
            // ADD END MARGE

            {
                var withBlock3 = CurrentForm();
                // 戦闘アニメで変更されたユニット画像を元に戻す
                object argIndex2 = "ユニット画像";
                if (withBlock3.IsConditionSatisfied(argIndex2))
                {
                    object argIndex1 = "ユニット画像";
                    withBlock3.DeleteCondition(argIndex1);
                    withBlock3.BitmapID = GUI.MakeUnitBitmap(CurrentForm());
                    GUI.PaintUnitBitmap(CurrentForm());
                }

                object argIndex4 = "非表示付加";
                if (withBlock3.IsConditionSatisfied(argIndex4))
                {
                    object argIndex3 = "非表示付加";
                    withBlock3.DeleteCondition(argIndex3);
                    withBlock3.BitmapID = GUI.MakeUnitBitmap(CurrentForm());
                    GUI.PaintUnitBitmap(CurrentForm());
                }
            }

            var loopTo12 = Information.UBound(partners);
            for (i = 1; i <= loopTo12; i++)
            {
                {
                    var withBlock4 = partners[i].CurrentForm();
                    object argIndex6 = "ユニット画像";
                    if (withBlock4.IsConditionSatisfied(argIndex6))
                    {
                        object argIndex5 = "ユニット画像";
                        withBlock4.DeleteCondition(argIndex5);
                        withBlock4.BitmapID = GUI.MakeUnitBitmap(partners[i].CurrentForm());
                        GUI.PaintUnitBitmap(partners[i].CurrentForm());
                    }

                    object argIndex8 = "非表示付加";
                    if (withBlock4.IsConditionSatisfied(argIndex8))
                    {
                        object argIndex7 = "非表示付加";
                        withBlock4.DeleteCondition(argIndex7);
                        withBlock4.BitmapID = GUI.MakeUnitBitmap(partners[i].CurrentForm());
                        GUI.PaintUnitBitmap(partners[i].CurrentForm());
                    }
                }
            }

            // 獲得した経験値の表示
            string argoname1 = "アビリティ経験値無効";
            if (is_useful & !is_event & !Expression.IsOptionDefined(argoname1))
            {
                string argexp_situation = "アビリティ";
                string argexp_mode = "";
                GetExp(max_lv_t, argexp_situation, exp_mode: argexp_mode);
                string argoname = "合体技パートナー経験値無効";
                if (!Expression.IsOptionDefined(argoname))
                {
                    var loopTo13 = Information.UBound(partners);
                    for (i = 1; i <= loopTo13; i++)
                    {
                        Unit argt = null;
                        string argexp_situation1 = "アビリティ";
                        string argexp_mode1 = "パートナー";
                        partners[i].CurrentForm().GetExp(argt, argexp_situation1, argexp_mode1);
                    }
                }
            }

            // 合体技のパートナーの弾数＆ＥＮの消費
            var loopTo14 = Information.UBound(partners);
            for (i = 1; i <= loopTo14; i++)
            {
                {
                    var withBlock5 = partners[i].CurrentForm();
                    var loopTo15 = withBlock5.CountAbility();
                    for (j = 1; j <= loopTo15; j++)
                    {
                        // パートナーが同名のアビリティを持っていればそのアビリティのデータを使う
                        if ((withBlock5.Ability(j).Name ?? "") == (aname ?? ""))
                        {
                            withBlock5.UseAbility(j);
                            string argattr16 = "自";
                            string argattr17 = "失";
                            string argattr18 = "変";
                            if (withBlock5.IsAbilityClassifiedAs(j, argattr16))
                            {
                                string argfname = "パーツ分離";
                                if (withBlock5.IsFeatureAvailable(argfname))
                                {
                                    object argIndex9 = "パーツ分離";
                                    string arglist = withBlock5.FeatureData(argIndex9);
                                    uname = GeneralLib.LIndex(arglist, 2);
                                    Unit localOtherForm() { object argIndex1 = uname; var ret = withBlock5.OtherForm(argIndex1); return ret; }

                                    if (localOtherForm().IsAbleToEnter(withBlock5.x, withBlock5.y))
                                    {
                                        withBlock5.Transform(uname);
                                        {
                                            var withBlock6 = withBlock5.CurrentForm();
                                            withBlock6.HP = withBlock6.MaxHP;
                                            withBlock6.UsedAction = withBlock6.MaxAction();
                                        }
                                    }
                                    else
                                    {
                                        withBlock5.Die();
                                    }
                                }
                                else
                                {
                                    withBlock5.Die();
                                }
                            }
                            else if (withBlock5.IsAbilityClassifiedAs(j, argattr17) & withBlock5.HP == 0)
                            {
                                withBlock5.Die();
                            }
                            else if (withBlock5.IsAbilityClassifiedAs(j, argattr18))
                            {
                                string argfname2 = "変形技";
                                string argfname3 = "ノーマルモード";
                                if (withBlock5.IsFeatureAvailable(argfname2))
                                {
                                    var loopTo16 = withBlock5.CountFeature();
                                    for (k = 1; k <= loopTo16; k++)
                                    {
                                        string localFeature() { object argIndex1 = k; var ret = withBlock5.Feature(argIndex1); return ret; }

                                        string localFeatureData1() { object argIndex1 = k; var ret = withBlock5.FeatureData(argIndex1); return ret; }

                                        string localLIndex() { string arglist = hsa17e1f441163458982d95695a4abb266(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

                                        if (localFeature() == "変形技" & (localLIndex() ?? "") == (aname ?? ""))
                                        {
                                            string localFeatureData() { object argIndex1 = k; var ret = withBlock5.FeatureData(argIndex1); return ret; }

                                            string arglist1 = localFeatureData();
                                            uname = GeneralLib.LIndex(arglist1, 2);
                                            Unit localOtherForm1() { object argIndex1 = uname; var ret = withBlock5.OtherForm(argIndex1); return ret; }

                                            if (localOtherForm1().IsAbleToEnter(withBlock5.x, withBlock5.y))
                                            {
                                                withBlock5.Transform(uname);
                                            }

                                            break;
                                        }
                                    }

                                    if ((uname ?? "") != (withBlock5.CurrentForm().Name ?? ""))
                                    {
                                        string argfname1 = "ノーマルモード";
                                        if (withBlock5.IsFeatureAvailable(argfname1))
                                        {
                                            object argIndex10 = "ノーマルモード";
                                            string arglist2 = withBlock5.FeatureData(argIndex10);
                                            uname = GeneralLib.LIndex(arglist2, 1);
                                            Unit localOtherForm2() { object argIndex1 = uname; var ret = withBlock5.OtherForm(argIndex1); return ret; }

                                            if (localOtherForm2().IsAbleToEnter(withBlock5.x, withBlock5.y))
                                            {
                                                withBlock5.Transform(uname);
                                            }
                                        }
                                    }
                                }
                                else if (withBlock5.IsFeatureAvailable(argfname3))
                                {
                                    object argIndex11 = "ノーマルモード";
                                    string arglist3 = withBlock5.FeatureData(argIndex11);
                                    uname = GeneralLib.LIndex(arglist3, 1);
                                    Unit localOtherForm3() { object argIndex1 = uname; var ret = withBlock5.OtherForm(argIndex1); return ret; }

                                    if (localOtherForm3().IsAbleToEnter(withBlock5.x, withBlock5.y))
                                    {
                                        withBlock5.Transform(uname);
                                    }
                                }
                            }

                            break;
                        }
                    }

                    // 同名のアビリティがなかった場合は自分のデータを使って処理
                    if (j > withBlock5.CountAbility())
                    {
                        if (this.Ability(a).ENConsumption > 0)
                        {
                            withBlock5.EN = withBlock5.EN - AbilityENConsumption(a);
                        }

                        string argattr19 = "消";
                        if (IsAbilityClassifiedAs(a, argattr19))
                        {
                            string argcname = "消耗";
                            string argcdata = "";
                            withBlock5.AddCondition(argcname, 1, cdata: argcdata);
                        }

                        string argattr20 = "Ｃ";
                        object argIndex13 = "チャージ完了";
                        if (IsAbilityClassifiedAs(a, argattr20) & withBlock5.IsConditionSatisfied(argIndex13))
                        {
                            object argIndex12 = "チャージ完了";
                            withBlock5.DeleteCondition(argIndex12);
                        }

                        string argattr22 = "気";
                        if (IsAbilityClassifiedAs(a, argattr22))
                        {
                            string argattr21 = "気";
                            withBlock5.IncreaseMorale((-5 * AbilityLevel(a, argattr21)));
                        }

                        string argattr25 = "霊";
                        string argattr26 = "プ";
                        if (IsAbilityClassifiedAs(a, argattr25))
                        {
                            hp_ratio = 100 * withBlock5.HP / (double)withBlock5.MaxHP;
                            en_ratio = 100 * withBlock5.EN / (double)withBlock5.MaxEN;
                            string argattr23 = "霊";
                            withBlock5.MainPilot().Plana = (withBlock5.MainPilot().Plana - 5d * AbilityLevel(a, argattr23));
                            withBlock5.HP = (withBlock5.MaxHP * hp_ratio / 100d);
                            withBlock5.EN = (withBlock5.MaxEN * en_ratio / 100d);
                        }
                        else if (IsAbilityClassifiedAs(a, argattr26))
                        {
                            hp_ratio = 100 * withBlock5.HP / (double)withBlock5.MaxHP;
                            en_ratio = 100 * withBlock5.EN / (double)withBlock5.MaxEN;
                            string argattr24 = "プ";
                            withBlock5.MainPilot().Plana = (withBlock5.MainPilot().Plana - 5d * AbilityLevel(a, argattr24));
                            withBlock5.HP = (withBlock5.MaxHP * hp_ratio / 100d);
                            withBlock5.EN = (withBlock5.MaxEN * en_ratio / 100d);
                        }

                        string argattr28 = "失";
                        if (IsAbilityClassifiedAs(a, argattr28))
                        {
                            string argattr27 = "失";
                            withBlock5.HP = GeneralLib.MaxLng((withBlock5.HP - (long)(withBlock5.MaxHP * AbilityLevel(a, argattr27)) / 10L), 0);
                        }

                        string argattr29 = "自";
                        string argattr30 = "失";
                        string argattr31 = "変";
                        if (IsAbilityClassifiedAs(a, argattr29))
                        {
                            string argfname4 = "パーツ分離";
                            if (withBlock5.IsFeatureAvailable(argfname4))
                            {
                                object argIndex14 = "パーツ分離";
                                string arglist4 = withBlock5.FeatureData(argIndex14);
                                uname = GeneralLib.LIndex(arglist4, 2);
                                Unit localOtherForm4() { object argIndex1 = uname; var ret = withBlock5.OtherForm(argIndex1); return ret; }

                                if (localOtherForm4().IsAbleToEnter(withBlock5.x, withBlock5.y))
                                {
                                    withBlock5.Transform(uname);
                                    {
                                        var withBlock7 = withBlock5.CurrentForm();
                                        withBlock7.HP = withBlock7.MaxHP;
                                        withBlock7.UsedAction = withBlock7.MaxAction();
                                    }
                                }
                                else
                                {
                                    withBlock5.Die();
                                }
                            }
                            else
                            {
                                withBlock5.Die();
                            }
                        }
                        else if (IsAbilityClassifiedAs(a, argattr30) & withBlock5.HP == 0)
                        {
                            withBlock5.Die();
                        }
                        else if (IsAbilityClassifiedAs(a, argattr31))
                        {
                            string argfname5 = "ノーマルモード";
                            if (withBlock5.IsFeatureAvailable(argfname5))
                            {
                                object argIndex15 = "ノーマルモード";
                                string arglist5 = withBlock5.FeatureData(argIndex15);
                                uname = GeneralLib.LIndex(arglist5, 1);
                                Unit localOtherForm5() { object argIndex1 = uname; var ret = withBlock5.OtherForm(argIndex1); return ret; }

                                if (localOtherForm5().IsAbleToEnter(withBlock5.x, withBlock5.y))
                                {
                                    withBlock5.Transform(uname);
                                }
                            }
                        }
                    }
                }
            }

            // 移動型マップアビリティによる移動
            string argattr32 = "Ｍ移";
            if (IsAbilityClassifiedAs(a, argattr32))
            {
                Jump(tx, ty);
            }

        Finish:
            ;


            // 以下の効果はアビリティデータが変化する可能性があるため、同時には適用されない

            // 自爆の処理
            string argattr33 = "自";

            // ＨＰ消費アビリティで自殺した場合
            string argattr34 = "失";

            // 変形技
            string argattr35 = "変";
            if (IsAbilityClassifiedAs(a, argattr33))
            {
                string argfname6 = "パーツ分離";
                if (IsFeatureAvailable(argfname6))
                {
                    // パーツ合体したユニットが自爆する時はパーツを分離するだけ
                    object argIndex16 = "パーツ分離";
                    string arglist6 = FeatureData(argIndex16);
                    uname = GeneralLib.LIndex(arglist6, 2);
                    Unit localOtherForm6() { object argIndex1 = uname; var ret = OtherForm(argIndex1); return ret; }

                    if (localOtherForm6().IsAbleToEnter(x, y))
                    {
                        Transform(uname);
                        {
                            var withBlock8 = CurrentForm();
                            withBlock8.HP = withBlock8.MaxHP;
                            withBlock8.UsedAction = withBlock8.MaxAction();
                        }

                        object argIndex17 = "パーツ分離";
                        fname = FeatureName(argIndex17);
                        bool localIsSysMessageDefined() { string argmain_situation = "破壊時分離(" + fname + ")"; string argsub_situation = ""; var ret = IsSysMessageDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

                        bool localIsSysMessageDefined1() { string argmain_situation = "分離(" + Name + ")"; string argsub_situation = ""; var ret = IsSysMessageDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

                        bool localIsSysMessageDefined2() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = IsSysMessageDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

                        string argmain_situation17 = "破壊時分離(" + Name + ")";
                        string argsub_situation19 = "";
                        string argmain_situation18 = "破壊時分離";
                        string argsub_situation20 = "";
                        string argmain_situation19 = "分離";
                        string argsub_situation21 = "";
                        if (IsSysMessageDefined(argmain_situation17, sub_situation: argsub_situation19))
                        {
                            string argmain_situation11 = "破壊時分離(" + Name + ")";
                            string argsub_situation13 = "";
                            string argadd_msg2 = "";
                            SysMessage(argmain_situation11, sub_situation: argsub_situation13, add_msg: argadd_msg2);
                        }
                        else if (localIsSysMessageDefined())
                        {
                            string argmain_situation12 = "破壊時分離(" + fname + ")";
                            string argsub_situation14 = "";
                            string argadd_msg3 = "";
                            SysMessage(argmain_situation12, sub_situation: argsub_situation14, add_msg: argadd_msg3);
                        }
                        else if (IsSysMessageDefined(argmain_situation18, sub_situation: argsub_situation20))
                        {
                            string argmain_situation13 = "破壊時分離";
                            string argsub_situation15 = "";
                            string argadd_msg4 = "";
                            SysMessage(argmain_situation13, sub_situation: argsub_situation15, add_msg: argadd_msg4);
                        }
                        else if (localIsSysMessageDefined1())
                        {
                            string argmain_situation14 = "分離(" + Name + ")";
                            string argsub_situation16 = "";
                            string argadd_msg5 = "";
                            SysMessage(argmain_situation14, sub_situation: argsub_situation16, add_msg: argadd_msg5);
                        }
                        else if (localIsSysMessageDefined2())
                        {
                            string argmain_situation15 = "分離(" + fname + ")";
                            string argsub_situation17 = "";
                            string argadd_msg6 = "";
                            SysMessage(argmain_situation15, sub_situation: argsub_situation17, add_msg: argadd_msg6);
                        }
                        else if (IsSysMessageDefined(argmain_situation19, sub_situation: argsub_situation21))
                        {
                            string argmain_situation16 = "分離";
                            string argsub_situation18 = "";
                            string argadd_msg7 = "";
                            SysMessage(argmain_situation16, sub_situation: argsub_situation18, add_msg: argadd_msg7);
                        }
                        else
                        {
                            GUI.DisplaySysMessage(Nickname + "は破壊されたパーツを分離させた。");
                        }
                    }
                    else
                    {
                        // しかし、パーツ分離できない地形ではそのまま自爆
                        Die();
                        if (!is_event)
                        {
                            Event_Renamed.HandleEvent("破壊", MainPilot().ID);
                            if (SRC.IsScenarioFinished)
                            {
                                return;
                            }
                        }
                    }
                }
                else
                {
                    Die();
                    if (!is_event)
                    {
                        Event_Renamed.HandleEvent("破壊", MainPilot().ID);
                        if (SRC.IsScenarioFinished)
                        {
                            return;
                        }
                    }
                }
            }
            else if (IsAbilityClassifiedAs(a, argattr34) & HP == 0)
            {
                Die();
                if (!is_event)
                {
                    Event_Renamed.HandleEvent("破壊", MainPilot().ID);
                    if (SRC.IsScenarioFinished)
                    {
                        return;
                    }
                }
            }
            else if (IsAbilityClassifiedAs(a, argattr35))
            {
                string argfname8 = "変形技";
                string argfname9 = "ノーマルモード";
                if (IsFeatureAvailable(argfname8))
                {
                    var loopTo17 = CountFeature();
                    for (i = 1; i <= loopTo17; i++)
                    {
                        string localFeature1() { object argIndex1 = i; var ret = Feature(argIndex1); return ret; }

                        string localFeatureData3() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

                        string localLIndex1() { string arglist = hs60551c61d0954d3e93ffb43a55a73d66(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

                        if (localFeature1() == "変形技" & (localLIndex1() ?? "") == (Ability(a).Name ?? ""))
                        {
                            string localFeatureData2() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

                            string arglist7 = localFeatureData2();
                            uname = GeneralLib.LIndex(arglist7, 2);
                            Unit localOtherForm7() { object argIndex1 = uname; var ret = OtherForm(argIndex1); return ret; }

                            if (localOtherForm7().IsAbleToEnter(x, y))
                            {
                                Transform(uname);
                            }

                            break;
                        }
                    }

                    if ((uname ?? "") != (CurrentForm().Name ?? ""))
                    {
                        string argfname7 = "ノーマルモード";
                        if (IsFeatureAvailable(argfname7))
                        {
                            object argIndex18 = "ノーマルモード";
                            string arglist8 = FeatureData(argIndex18);
                            uname = GeneralLib.LIndex(arglist8, 1);
                            Unit localOtherForm8() { object argIndex1 = uname; var ret = OtherForm(argIndex1); return ret; }

                            if (localOtherForm8().IsAbleToEnter(x, y))
                            {
                                Transform(uname);
                            }
                        }
                    }
                }
                else if (IsFeatureAvailable(argfname9))
                {
                    object argIndex19 = "ノーマルモード";
                    string arglist9 = FeatureData(argIndex19);
                    uname = GeneralLib.LIndex(arglist9, 1);
                    Unit localOtherForm9() { object argIndex1 = uname; var ret = OtherForm(argIndex1); return ret; }

                    if (localOtherForm9().IsAbleToEnter(x, y))
                    {
                        Transform(uname);
                    }
                }
            }

            // アイテムを消費
            else if (Ability(a).IsItem() & Stock(a) == 0 & MaxStock(a) > 0)
            {
                // アイテムを削除
                num = Data.CountAbility();
                num = (num + MainPilot().Data.CountAbility());
                var loopTo18 = CountPilot();
                for (i = 2; i <= loopTo18; i++)
                {
                    Pilot localPilot() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

                    num = (num + localPilot().Data.CountAbility());
                }

                var loopTo19 = CountSupport();
                for (i = 2; i <= loopTo19; i++)
                {
                    Pilot localSupport() { object argIndex1 = i; var ret = Support(argIndex1); return ret; }

                    num = (num + localSupport().Data.CountAbility());
                }

                string argfname10 = "追加サポート";
                if (IsFeatureAvailable(argfname10))
                {
                    num = (num + AdditionalSupport().Data.CountAbility());
                }

                foreach (Item itm in colItem)
                {
                    num = (num + itm.CountAbility());
                    if (a <= num)
                    {
                        itm.Exist = false;
                        DeleteItem((object)itm.ID);
                        break;
                    }
                }
            }

            // 使用後イベント
            if (!is_event)
            {
                Event_Renamed.HandleEvent("使用後", CurrentForm().MainPilot().ID, aname);
                if (SRC.IsScenarioFinished | SRC.IsCanceled)
                {
                    return;
                }
            }

            GUI.CloseMessageForm();

            // ハイパーモード＆ノーマルモードの自動発動をチェック
            SRC.UList.CheckAutoHyperMode();
            SRC.UList.CheckAutoNormalMode();
        }

        // アビリティの使用によるＥＮ、使用回数の消費等を行う
        public void UseAbility(int a)
        {
            int i, lv;
            double hp_ratio, en_ratio;
            if (this.Ability(a).ENConsumption > 0)
            {
                EN = EN - AbilityENConsumption(a);
            }

            if (this.Ability(a).Stock > 0)
            {
                SetStock(a, (Stock(a) - 1));

                // 一斉使用
                string argattr1 = "斉";
                if (IsAbilityClassifiedAs(a, argattr1))
                {
                    var loopTo = Information.UBound(dblStock);
                    for (i = 1; i <= loopTo; i++)
                        SetStock(i, GeneralLib.MinLng((MaxStock(i) * Stock(a)) / MaxStock(a), Stock(i)));
                }
                else
                {
                    var loopTo1 = Information.UBound(dblStock);
                    for (i = 1; i <= loopTo1; i++)
                    {
                        string argattr = "斉";
                        if (IsAbilityClassifiedAs(i, argattr))
                        {
                            SetStock(i, GeneralLib.MinLng(((MaxStock(i) * Stock(a)) / MaxStock(a) + 0.49999d), Stock(i)));
                        }
                    }
                }

                // 弾数・使用回数共有の処理
                SyncBullet();
            }

            // 消耗技
            string argattr2 = "消";
            if (IsAbilityClassifiedAs(a, argattr2))
            {
                string argcname = "消耗";
                string argcdata = "";
                AddCondition(argcname, 1, cdata: argcdata);
            }

            // 全ＥＮ消費アビリティ
            string argattr3 = "尽";
            if (IsAbilityClassifiedAs(a, argattr3))
            {
                EN = 0;
            }

            // チャージ式アビリティ
            string argattr4 = "Ｃ";
            object argIndex2 = "チャージ完了";
            if (IsAbilityClassifiedAs(a, argattr4) & IsConditionSatisfied(argIndex2))
            {
                object argIndex1 = "チャージ完了";
                DeleteCondition(argIndex1);
            }

            // 自動充填式アビリティ
            string argattr6 = "Ａ";
            if (AbilityLevel(a, argattr6) > 0d)
            {
                string argcname1 = AbilityNickname(a) + "充填中";
                string argattr5 = "Ａ";
                string argcdata1 = "";
                AddCondition(argcname1, AbilityLevel(a, argattr5), cdata: argcdata1);
            }

            // 気力を消費
            string argattr8 = "気";
            if (IsAbilityClassifiedAs(a, argattr8))
            {
                string argattr7 = "気";
                IncreaseMorale((-5 * AbilityLevel(a, argattr7)));
            }

            // 霊力の消費
            string argattr11 = "霊";
            string argattr12 = "プ";
            if (IsAbilityClassifiedAs(a, argattr11))
            {
                hp_ratio = 100 * HP / (double)MaxHP;
                en_ratio = 100 * EN / (double)MaxEN;
                string argattr9 = "霊";
                MainPilot().Plana = (this.MainPilot().Plana - 5d * AbilityLevel(a, argattr9));
                HP = (MaxHP * hp_ratio / 100d);
                EN = (MaxEN * en_ratio / 100d);
            }
            else if (IsAbilityClassifiedAs(a, argattr12))
            {
                hp_ratio = 100 * HP / (double)MaxHP;
                en_ratio = 100 * EN / (double)MaxEN;
                string argattr10 = "プ";
                MainPilot().Plana = (this.MainPilot().Plana - 5d * AbilityLevel(a, argattr10));
                HP = (MaxHP * hp_ratio / 100d);
                EN = (MaxEN * en_ratio / 100d);
            }

            // 資金消費アビリティ
            if (Party == "味方")
            {
                string argattr14 = "銭";
                if (IsAbilityClassifiedAs(a, argattr14))
                {
                    string argattr13 = "銭";
                    SRC.IncrMoney(-GeneralLib.MaxLng(AbilityLevel(a, argattr13), 1) * Value / 10);
                }
            }

            // ＨＰ消費アビリティ
            string argattr16 = "失";
            if (IsAbilityClassifiedAs(a, argattr16))
            {
                string argattr15 = "失";
                HP = GeneralLib.MaxLng((HP - (long)(MaxHP * AbilityLevel(a, argattr15)) / 10L), 0);
            }
        }
    }
}
