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

namespace SRCCore.Units
{
    // === アビリティ関連処理 ===
    public partial class Unit
    {
        // アビリティ
        public UnitAbility Ability(int a)
        {
            return Abilities.SafeRefOneOffset(a);
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
            bool ExecuteAbilityRet = false;
            bool is_useful = false;
            var is_anime_played = false;

            var aname = a.Data.Name;
            var anickname = a.AbilityNickname();
            var aclass = a.Data.Class;
            IList<Unit> partners = new List<Unit>();

            // 現在の選択状況をセーブ
            Commands.SaveSelections();

            // 選択内容を切り替え
            Commands.SelectedUnit = this;
            Event.SelectedUnitForEvent = this;
            Commands.SelectedTarget = t;
            Event.SelectedTargetForEvent = t;
            Commands.SelectedAbility = a.AbilityNo();
            Commands.SelectedAbilityName = aname;
            if (!is_map_ability)
            {
                // 通常アビリティの場合
                if (SRC.BattleAnimation)
                {
                    GUI.RedrawScreen();
                }

                if (a.IsAbilityClassifiedAs("合"))
                {
                    // 射程が0の場合はマスクをクリアしておく
                    if (a.AbilityMaxRange() == 0)
                    {
                        Map.ClearMask();
                        Map.MaskData[x, y] = false;
                    }

                    // 合体技の場合にパートナーをハイライト表示
                    if (a.AbilityMaxRange() == 1)
                    {
                        partners = a.CombinationPartner(t.x, t.y);
                    }
                    else
                    {
                        partners = a.CombinationPartner();
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
                else
                {
                    Commands.SelectedPartners.Clear();
                }

                // ダイアログ用にあらかじめ追加パイロットを作成しておく
                foreach (var effect in a.Data.Effects)
                {
                    var edata = effect.Data;
                    switch (effect.EffectType)
                    {
                        case "変身":
                            {
                                if (!SRC.UDList.IsDefined(GeneralLib.LIndex(edata, 1)))
                                {
                                    GUI.ErrorMessage(GeneralLib.LIndex(edata, 1) + "のデータが定義されていません");
                                    return ExecuteAbilityRet;
                                }

                                var tfu = SRC.UDList.Item(GeneralLib.LIndex(edata, 1));
                                if (tfu.IsFeatureAvailable("追加パイロット"))
                                {
                                    if (!SRC.PList.IsDefined(tfu.FeatureData("追加パイロット")))
                                    {
                                        SRC.PList.Add(t.FeatureData("追加パイロット"), MainPilot().Level, Party0, gid: "");
                                    }
                                }

                                break;
                            }
                    }
                }

                // アビリティ使用時のメッセージ＆特殊効果
                if (IsAnimationDefined(aname + "(準備)", sub_situation: ""))
                {
                    PlayAnimation(aname + "(準備)", sub_situation: "");
                }

                if (IsMessageDefined("かけ声(" + aname + ")"))
                {
                    if (!GUI.MessageFormVisible)
                    {
                        if (ReferenceEquals(Commands.SelectedTarget, this))
                        {
                            GUI.OpenMessageForm(this, u2: null);
                        }
                        else
                        {
                            GUI.OpenMessageForm(Commands.SelectedTarget, this);
                        }
                    }

                    PilotMessage("かけ声(" + aname + ")", msg_mode: "");
                }

                if (IsMessageDefined(aname) || IsMessageDefined("アビリティ"))
                {
                    if (!GUI.MessageFormVisible)
                    {
                        if (ReferenceEquals(Commands.SelectedTarget, this))
                        {
                            GUI.OpenMessageForm(this, u2: null);
                        }
                        else
                        {
                            GUI.OpenMessageForm(Commands.SelectedTarget, this);
                        }
                    }

                    PilotMessage(aname, "アビリティ");
                }

                if (IsAnimationDefined(aname + "(使用)", sub_situation: ""))
                {
                    PlayAnimation(aname + "(使用)", "", true);
                }

                if (IsAnimationDefined(aname + "(発動)", sub_situation: "") || IsAnimationDefined(aname, sub_situation: ""))
                {
                    PlayAnimation(aname + "(発動)", "", true);
                    is_anime_played = true;
                }
                else
                {
                    SpecialEffect(aname, "", true);
                }

                // アビリティの種類は？
                var atype = "";
                foreach (var effect in a.Data.Effects)
                {
                    switch (effect.EffectType)
                    {
                        case "召喚":
                            {
                                aname = "";
                                break;
                            }

                        case "再行動":
                            {
                                if (a.AbilityMaxRange() > 0)
                                {
                                    atype = effect.EffectType;
                                }

                                break;
                            }

                        case "解説":
                            {
                                break;
                            }

                        default:
                            {
                                atype = effect.EffectType;
                                break;
                            }
                    }
                }

                var msg = "";
                switch (partners.Count)
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
                            if ((Nickname ?? "") != (partners[0].Nickname ?? ""))
                            {
                                msg = Nickname + "は[" + partners[0].Nickname + "]と共に";
                            }
                            else if ((MainPilot().get_Nickname(false) ?? "") != (partners[0].MainPilot().get_Nickname(false) ?? ""))
                            {
                                msg = MainPilot().get_Nickname(false) + "と[" + partners[0].MainPilot().get_Nickname(false) + "]の[" + Nickname + "]は";
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
                            if ((Nickname ?? "") != (partners[0].Nickname ?? ""))
                            {
                                msg = Nickname + "は[" + partners[0].Nickname + "]、[" + partners[1].Nickname + "]と共に";
                            }
                            else if ((MainPilot().get_Nickname(false) ?? "") != (partners[0].MainPilot().get_Nickname(false) ?? ""))
                            {
                                msg = MainPilot().get_Nickname(false) + "、[" + partners[0].MainPilot().get_Nickname(false) + "]、[" + partners[1].MainPilot().get_Nickname(false) + "]の[" + Nickname + "]は";
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

                if (a.IsSpellAbility())
                {
                    if (t is object && a.AbilityMaxRange() != 0)
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
                    if (t is object && a.AbilityMaxRange() != 0)
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

                if (IsSysMessageDefined(aname, sub_situation: ""))
                {
                    // 「アビリティ名(解説)」のメッセージを使用
                    if (!GUI.MainFormVisible)
                    {
                        if (ReferenceEquals(Commands.SelectedTarget, this))
                        {
                            GUI.OpenMessageForm(this, u2: null);
                        }
                        else
                        {
                            GUI.OpenMessageForm(Commands.SelectedTarget, this);
                        }
                    }

                    SysMessage(aname, sub_situation: "", add_msg: "");
                }
                else if (IsSysMessageDefined("アビリティ", sub_situation: ""))
                {
                    // 「アビリティ(解説)」のメッセージを使用
                    if (!GUI.MainFormVisible)
                    {
                        if (ReferenceEquals(Commands.SelectedTarget, this))
                        {
                            GUI.OpenMessageForm(this, u2: null);
                        }
                        else
                        {
                            GUI.OpenMessageForm(Commands.SelectedTarget, this);
                        }
                    }

                    SysMessage("アビリティ", sub_situation: "", add_msg: "");
                }
                else if (atype == "変身" && a.AbilityMaxRange() == 0)
                {
                }
                // 変身の場合はメッセージなし
                else if (!string.IsNullOrEmpty(atype))
                {
                    if (!GUI.MainFormVisible)
                    {
                        if (ReferenceEquals(Commands.SelectedTarget, this))
                        {
                            GUI.OpenMessageForm(this, u2: null);
                        }
                        else
                        {
                            GUI.OpenMessageForm(Commands.SelectedTarget, this);
                        }
                    }

                    GUI.DisplaySysMessage(msg);
                }

                // ＥＮ消費＆使用回数減少
                a.UseAbility();

                // アビリティの使用に失敗？
                if (GeneralLib.Dice(10) <= a.AbilityLevel("難"))
                {
                    GUI.DisplaySysMessage("しかし何もおきなかった…");
                    goto Finish;
                }
            }
            else
            {
                // マップアビリティの場合
                if (IsAnimationDefined(aname + "(発動)", sub_situation: "") || IsAnimationDefined(aname, sub_situation: ""))
                {
                    PlayAnimation(aname + "(発動)", sub_situation: "");
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
            if (a.IsAbilityClassifiedAs("脱"))
            {
                t.IncreaseMorale(-10);
            }

            // 特殊効果除去アビリティ
            if (a.IsAbilityClassifiedAs("除"))
            {
                foreach (var cond in t.Conditions.Where(x => (
                         Strings.InStr(x.Name, "付加") > 0
                             || Strings.InStr(x.Name, "強化") > 0
                             || Strings.InStr(x.Name, "ＵＰ") > 0)
                         && x.Name != "ノーマルモード付加"
                         && x.IsEnable))
                {
                    t.DeleteCondition(cond.Name);
                }
            }

            // 得意技・不得手によるアビリティ効果への修正値を計算
            var elv_mod = 1d;
            var elv_mod2 = 1d;
            {
                var withBlock2 = MainPilot();
                // 得意技
                if (withBlock2.IsSkillAvailable("得意技"))
                {
                    var buf = withBlock2.SkillData("得意技");
                    var loopTo5 = Strings.Len(buf);
                    for (var i = 1; i <= loopTo5; i++)
                    {
                        if (Strings.InStr(aclass, GeneralLib.GetClassBundle(buf, ref i)) > 0)
                        {
                            elv_mod = 1.2d * elv_mod;
                            elv_mod2 = 1.4d * elv_mod2;
                            break;
                        }
                    }
                }

                // 不得手
                if (withBlock2.IsSkillAvailable("不得手"))
                {
                    var buf = withBlock2.SkillData("不得手");
                    var loopTo6 = Strings.Len(buf);
                    for (var i = 1; i <= loopTo6; i++)
                    {
                        if (Strings.InStr(aclass, GeneralLib.GetClassBundle(buf, ref i)) > 0)
                        {
                            elv_mod = 0.8d * elv_mod;
                            elv_mod2 = 0.6d * elv_mod2;
                            break;
                        }
                    }
                }
            }

            // アビリティの効果を適用
            foreach (var effect in a.Data.Effects)
            {
                var edata = effect.Data;
                var elevel = effect.Level * elv_mod;
                var elevel2 = effect.Level * elv_mod2;

                switch (effect.EffectType)
                {
                    case "回復":
                        {
                            if (elevel > 0d)
                            {
                                if (!t.CanHPRecovery)
                                {
                                    goto NextLoop;
                                }

                                if (!is_anime_played)
                                {
                                    if (a.IsSpellAbility() || a.IsAbilityClassifiedAs("魔"))
                                    {
                                        Effect.ShowAnimation("回復魔法発動");
                                    }
                                    else
                                    {
                                        Effect.ShowAnimation("修理装置発動");
                                    }
                                }

                                var prev_value = t.HP;
                                var epower = 0;
                                {
                                    var p = MainPilot();
                                    if (a.IsSpellAbility())
                                    {
                                        epower = ((int)(5d * elevel * p.Shooting));
                                    }
                                    else
                                    {
                                        epower = ((int)(500d * elevel));
                                    }

                                    epower = (int)(epower * (10d + p.SkillLevel("修理", ref_mode: ""))) / 10;
                                }

                                t.HP = t.HP + epower;
                                GUI.DrawSysString(t.x, t.y, "+" + SrcFormatter.Format(t.HP - prev_value));
                                if (ReferenceEquals(t, this))
                                {
                                    GUI.UpdateMessageForm(this, u2: null);
                                }
                                else
                                {
                                    GUI.UpdateMessageForm(t, this);
                                }

                                GUI.DisplaySysMessage(t.Nickname + "の" + Expression.Term("ＨＰ", t) + "が[" + SrcFormatter.Format(t.HP - prev_value) + "]回復した;" + "残り" + Expression.Term("ＨＰ", t) + "は" + SrcFormatter.Format(t.HP) + "（損傷率 = " + SrcFormatter.Format(100 * (t.MaxHP - t.HP) / t.MaxHP) + "％）");
                                is_useful = true;
                            }
                            else if (elevel < 0d)
                            {
                                var prev_value = t.HP;
                                var epower = 0;
                                {
                                    var p = MainPilot();
                                    if (a.IsSpellAbility())
                                    {
                                        epower = (int)(5d * elevel * p.Shooting);
                                    }
                                    else
                                    {
                                        epower = (int)(500d * elevel);
                                    }
                                }

                                t.HP = t.HP + epower;
                                GUI.DrawSysString(t.x, t.y, "-" + SrcFormatter.Format(prev_value - t.HP));
                                if (ReferenceEquals(t, this))
                                {
                                    GUI.UpdateMessageForm(this, u2: null);
                                }
                                else
                                {
                                    GUI.UpdateMessageForm(t, this);
                                }

                                GUI.DisplaySysMessage(t.Nickname + "の" + Expression.Term("ＨＰ", t) + "が[" + SrcFormatter.Format(prev_value - t.HP) + "]減少した;" + "残り" + Expression.Term("ＨＰ", t) + "は" + SrcFormatter.Format(t.HP) + "（損傷率 = " + SrcFormatter.Format(100 * (t.MaxHP - t.HP) / t.MaxHP) + "％）");
                            }

                            break;
                        }

                    case "補給":
                        {
                            {
                                if (elevel > 0d)
                                {
                                    if (!t.CanENRecovery)
                                    {
                                        goto NextLoop;
                                    }

                                    if (!is_anime_played)
                                    {
                                        if (a.IsSpellAbility() || a.IsAbilityClassifiedAs("魔"))
                                        {
                                            Effect.ShowAnimation("回復魔法発動");
                                        }
                                        else
                                        {
                                            Effect.ShowAnimation("補給装置発動");
                                        }
                                    }

                                    var prev_value = t.EN;
                                    var epower = 0;
                                    {
                                        var withBlock8 = MainPilot();
                                        if (a.IsSpellAbility())
                                        {
                                            epower = (int)((long)(elevel * withBlock8.Shooting) / 2L);
                                        }
                                        else
                                        {
                                            epower = (int)(50d * elevel);
                                        }

                                        epower = (int)((long)(epower * (10d + withBlock8.SkillLevel("補給", ref_mode: ""))) / 10L);
                                    }

                                    t.EN = t.EN + epower;
                                    GUI.DrawSysString(t.x, t.y, "+" + SrcFormatter.Format(t.EN - prev_value));
                                    if (ReferenceEquals(t, this))
                                    {
                                        GUI.UpdateMessageForm(this, u2: null);
                                    }
                                    else
                                    {
                                        GUI.UpdateMessageForm(t, this);
                                    }

                                    GUI.DisplaySysMessage(t.Nickname + "の" + Expression.Term("ＥＮ", t) + "が[" + SrcFormatter.Format(t.EN - prev_value) + "]回復した;" + "残り" + Expression.Term("ＥＮ", t) + "は" + SrcFormatter.Format(t.EN));
                                    is_useful = true;
                                }
                                else if (elevel < 0d)
                                {
                                    // ＥＮは既に0？
                                    if (t.EN == 0)
                                    {
                                        goto NextLoop;
                                    }

                                    var prev_value = t.EN;
                                    var epower = 0;
                                    {
                                        var withBlock9 = MainPilot();
                                        if (a.IsSpellAbility())
                                        {
                                            epower = (int)((long)(elevel * withBlock9.Shooting) / 2L);
                                        }
                                        else
                                        {
                                            epower = (int)(50d * elevel);
                                        }
                                    }

                                    t.EN = t.EN + epower;
                                    GUI.DrawSysString(t.x, t.y, "-" + SrcFormatter.Format(prev_value - t.EN));
                                    if (ReferenceEquals(t, this))
                                    {
                                        GUI.UpdateMessageForm(this, u2: null);
                                    }
                                    else
                                    {
                                        GUI.UpdateMessageForm(t, this);
                                    }

                                    GUI.DisplaySysMessage(t.Nickname + "の" + Expression.Term("ＥＮ", t) + "が[" + SrcFormatter.Format(prev_value - t.EN) + "]減少した;" + "残り" + Expression.Term("ＥＮ", t) + "は" + SrcFormatter.Format(t.EN));
                                }
                            }

                            break;
                        }

                    case "霊力回復":
                    case "プラーナ回復":
                        {
                            {
                                var p = t.MainPilot();
                                if (elevel > 0d)
                                {
                                    // 霊力は既に最大値？
                                    if (p.Plana == p.MaxPlana())
                                    {
                                        goto NextLoop;
                                    }

                                    var prev_value = p.Plana;
                                    var epower = 0;
                                    if (a.IsSpellAbility())
                                    {
                                        p.Plana = (int)(p.Plana + ((long)(elevel * this.MainPilot().Shooting) / 10L));
                                    }
                                    else
                                    {
                                        p.Plana = (int)(p.Plana + 10d * elevel);
                                    }

                                    GUI.DrawSysString(t.x, t.y, "+" + SrcFormatter.Format(p.Plana - prev_value));
                                    if (ReferenceEquals(t, this))
                                    {
                                        GUI.UpdateMessageForm(this, u2: null);
                                    }
                                    else
                                    {
                                        GUI.UpdateMessageForm(t, this);
                                    }

                                    GUI.DisplaySysMessage(p.get_Nickname(false) + "の[" + p.SkillName0("霊力") + "]が[" + SrcFormatter.Format(p.Plana - prev_value) + "]回復した。");
                                    is_useful = true;
                                }
                                else if (elevel < 0d)
                                {
                                    // 霊力は既に0？
                                    if (p.Plana == 0)
                                    {
                                        goto NextLoop;
                                    }

                                    var prev_value = p.Plana;
                                    var epower = 0;
                                    if (a.IsSpellAbility())
                                    {
                                        p.Plana = (int)(p.Plana + ((long)(elevel * this.MainPilot().Shooting) / 10L));
                                    }
                                    else
                                    {
                                        p.Plana = (int)(p.Plana + 10d * elevel);
                                    }

                                    GUI.DrawSysString(t.x, t.y, "-" + SrcFormatter.Format(prev_value - p.Plana));
                                    if (ReferenceEquals(t, this))
                                    {
                                        GUI.UpdateMessageForm(this, u2: null);
                                    }
                                    else
                                    {
                                        GUI.UpdateMessageForm(t, this);
                                    }

                                    GUI.DisplaySysMessage(p.get_Nickname(false) + "の[" + p.SkillName0("霊力") + "]が[" + SrcFormatter.Format(prev_value - p.Plana) + "]減少した。");
                                }
                            }

                            break;
                        }

                    case "ＳＰ回復":
                        {
                            var epower = 0;
                            if (a.IsSpellAbility())
                            {
                                epower = (int)((long)(elevel * this.MainPilot().Shooting) / 10L);
                            }
                            else
                            {
                                epower = (int)(10d * elevel);
                            }

                            {
                                // パイロット数を計算
                                var num = t.AllPilots.Count();

                                if (elevel > 0d)
                                {
                                    if (num == 1)
                                    {
                                        // パイロットが１名のみ
                                        // パイロットが１名のみ
                                        {
                                            var p = t.MainPilot();
                                            var prev_value = p.SP;
                                            p.SP = p.SP + epower;
                                            GUI.DrawSysString(t.x, t.y, "+" + SrcFormatter.Format(p.SP - prev_value));
                                            GUI.DisplaySysMessage(p.get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + SrcFormatter.Format(p.SP - prev_value) + "回復した。");
                                            if (p.SP > prev_value)
                                            {
                                                is_useful = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // 複数のパイロットが対象
                                        foreach (var p in t.AllPilots)
                                        {
                                            var prev_value = p.SP;
                                            p.SP = p.SP + epower / 5 + epower / num;
                                            GUI.DrawSysString(t.x, t.y, "+" + SrcFormatter.Format(p.SP - prev_value));
                                            GUI.DisplaySysMessage(p.get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + SrcFormatter.Format(p.SP - prev_value) + "回復した。");
                                            if (p.SP > prev_value)
                                            {
                                                is_useful = true;
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
                                            var p = t.MainPilot();
                                            var prev_value = p.SP;
                                            p.SP = p.SP + epower;
                                            GUI.DrawSysString(t.x, t.y, "-" + SrcFormatter.Format(prev_value - p.SP));
                                            GUI.DisplaySysMessage(p.get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + SrcFormatter.Format(prev_value - p.SP) + "減少した。");
                                        }
                                    }
                                    else
                                    {
                                        // 複数のパイロットが対象
                                        foreach (var p in t.AllPilots)
                                        {
                                            var prev_value = p.SP;
                                            p.SP = p.SP + epower / 5 + epower / num;
                                            GUI.DrawSysString(t.x, t.y, "+" + SrcFormatter.Format(prev_value - p.SP));
                                            GUI.DisplaySysMessage(p.get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + SrcFormatter.Format(prev_value - p.SP) + "減少した。");
                                        }
                                    }
                                }
                            }

                            break;
                        }

                    case "気力増加":
                        {
                            var epower = 0;
                            if (a.IsSpellAbility())
                            {
                                epower = (int)((long)(elevel * this.MainPilot().Shooting) / 10L);
                            }
                            else
                            {
                                epower = (int)(10d * elevel);
                            }

                            {
                                var prev_value = t.MainPilot().Morale;
                                t.IncreaseMorale(epower);
                                if (elevel > 0d)
                                {
                                    {
                                        var withBlock23 = t.MainPilot();
                                        GUI.DrawSysString(t.x, t.y, "+" + SrcFormatter.Format(withBlock23.Morale - prev_value));
                                        GUI.DisplaySysMessage(withBlock23.get_Nickname(false) + "の" + Expression.Term("気力", t) + "が" + SrcFormatter.Format(withBlock23.Morale - prev_value) + "増加した。");
                                    }
                                }
                                else if (elevel < 0d)
                                {
                                    {
                                        var withBlock24 = t.MainPilot();
                                        GUI.DrawSysString(t.x, t.y, "-" + SrcFormatter.Format(prev_value - withBlock24.Morale));
                                        GUI.DisplaySysMessage(withBlock24.get_Nickname(false) + "の" + Expression.Term("気力", t) + "が" + SrcFormatter.Format(prev_value - withBlock24.Morale) + "減少した。");
                                    }
                                }

                                if (t.MainPilot().Morale > prev_value)
                                {
                                    is_useful = true;
                                }
                            }

                            break;
                        }

                    case "装填":
                        {
                            {
                                var flag = false;
                                if (string.IsNullOrEmpty(edata))
                                {
                                    // 全ての武器の弾数を回復
                                    if (t.CanBulletRecovery)
                                    {

                                        t.BulletSupply();
                                        flag = true;
                                    }

                                    // 弾数とアビリティ使用回数の同期を取る
                                    if (flag)
                                    {
                                        foreach (var ua in t.Abilities
                                            .Where(x => x.IsAbilityClassifiedAs("共"))
                                            .Where(x => t.Weapons.Select(w => w.WeaponLevel("共")).Any(lv => x.AbilityLevel("共") == lv)))
                                        {
                                            ua.SetStockFull();
                                        }

                                        // 弾数・使用回数の共有化処理
                                        t.SyncBullet();
                                    }
                                }
                                else
                                {
                                    // 特定の武器の弾数のみを回復
                                    var wlist = new List<UnitWeapon>();
                                    foreach (var uw in t.OtherForms.Append(t).SelectMany(t => Weapons.Where(uw => uw.WeaponNickname() == edata
                                        || GeneralLib.InStrNotNest(uw.WeaponClass(), edata) > 0)))
                                    {
                                        uw.SetBulletFull();
                                        flag = true;
                                        wlist.Add(uw);
                                    }

                                    // 弾数の同期を取る
                                    if (flag)
                                    {
                                        foreach (var ua in t.Abilities
                                            .Where(x => x.IsAbilityClassifiedAs("共"))
                                            .Where(x => wlist.Select(w => w.WeaponLevel("共")).Any(lv => x.AbilityLevel("共") == lv)))
                                        {
                                            ua.SetStockFull();
                                        }

                                        // 弾数・使用回数の共有化処理
                                        t.SyncBullet();
                                    }
                                }

                                if (flag)
                                {
                                    GUI.DisplaySysMessage((string)(t.Nickname + "の武装の使用回数が回復した。"));
                                    if (a.AbilityMaxRange() > 0)
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
                                if (!is_anime_played)
                                {
                                    if (a.IsSpellAbility() || a.IsAbilityClassifiedAs("魔"))
                                    {
                                        Effect.ShowAnimation("回復魔法発動");
                                    }
                                }

                                if (string.IsNullOrEmpty(edata))
                                {
                                    // 全てのステータス異常を回復
                                    if (t.ConditionLifetime("攻撃不能") > 0)
                                    {
                                        t.DeleteCondition("攻撃不能");
                                        is_useful = true;
                                    }

                                    if (t.ConditionLifetime("移動不能") > 0)
                                    {
                                        t.DeleteCondition("移動不能");
                                        is_useful = true;
                                    }

                                    if (t.ConditionLifetime("装甲劣化") > 0)
                                    {
                                        t.DeleteCondition("装甲劣化");
                                        is_useful = true;
                                    }

                                    if (t.ConditionLifetime("混乱") > 0)
                                    {
                                        t.DeleteCondition("混乱");
                                        is_useful = true;
                                    }

                                    if (t.ConditionLifetime("恐怖") > 0)
                                    {
                                        t.DeleteCondition("恐怖");
                                        is_useful = true;
                                    }

                                    if (t.ConditionLifetime("踊り") > 0)
                                    {
                                        t.DeleteCondition("踊り");
                                        is_useful = true;
                                    }

                                    if (t.ConditionLifetime("狂戦士") > 0)
                                    {
                                        t.DeleteCondition("狂戦士");
                                        is_useful = true;
                                    }

                                    if (t.ConditionLifetime("ゾンビ") > 0)
                                    {
                                        t.DeleteCondition("ゾンビ");
                                        is_useful = true;
                                    }

                                    if (t.ConditionLifetime("回復不能") > 0)
                                    {
                                        t.DeleteCondition("回復不能");
                                        is_useful = true;
                                    }

                                    if (t.ConditionLifetime("石化") > 0)
                                    {
                                        t.DeleteCondition("石化");
                                        is_useful = true;
                                    }

                                    if (t.ConditionLifetime("凍結") > 0)
                                    {
                                        t.DeleteCondition("凍結");
                                        is_useful = true;
                                    }

                                    if (t.ConditionLifetime("麻痺") > 0)
                                    {
                                        t.DeleteCondition("麻痺");
                                        is_useful = true;
                                    }

                                    if (t.ConditionLifetime("睡眠") > 0)
                                    {
                                        t.DeleteCondition("睡眠");
                                        is_useful = true;
                                    }

                                    if (t.ConditionLifetime("毒") > 0)
                                    {
                                        t.DeleteCondition("毒");
                                        is_useful = true;
                                    }

                                    if (t.ConditionLifetime("盲目") > 0)
                                    {
                                        t.DeleteCondition("盲目");
                                        is_useful = true;
                                    }

                                    if (t.ConditionLifetime("沈黙") > 0)
                                    {
                                        t.DeleteCondition("沈黙");
                                        is_useful = true;
                                    }

                                    if (t.ConditionLifetime("魅了") > 0)
                                    {
                                        t.DeleteCondition("魅了");
                                        is_useful = true;
                                    }

                                    if (t.ConditionLifetime("憑依") > 0)
                                    {
                                        t.DeleteCondition("憑依");
                                        is_useful = true;
                                    }
                                    // 剋属性
                                    if (t.ConditionLifetime("オーラ使用不能") > 0)
                                    {
                                        t.DeleteCondition("オーラ使用不能");
                                    }

                                    if (t.ConditionLifetime("超能力使用不能") > 0)
                                    {
                                        t.DeleteCondition("超能力使用不能");
                                    }

                                    if (t.ConditionLifetime("同調率使用不能") > 0)
                                    {
                                        t.DeleteCondition("同調率使用不能");
                                    }

                                    if (t.ConditionLifetime("超感覚使用不能") > 0)
                                    {
                                        t.DeleteCondition("超感覚使用不能");
                                    }

                                    if (t.ConditionLifetime("知覚強化使用不能") > 0)
                                    {
                                        t.DeleteCondition("知覚強化使用不能");
                                    }

                                    if (t.ConditionLifetime("霊力使用不能") > 0)
                                    {
                                        t.DeleteCondition("霊力使用不能");
                                    }

                                    if (t.ConditionLifetime("術使用不能") > 0)
                                    {
                                        t.DeleteCondition("術使用不能");
                                    }

                                    if (t.ConditionLifetime("技使用不能") > 0)
                                    {
                                        t.DeleteCondition("技使用不能");
                                    }

                                    foreach (var cond in t.Conditions.CloneList())
                                    {
                                        // 弱点、有効付加はあえて外してあります。
                                        if (Strings.Len(cond.Name) > 6 && Strings.Right(cond.Name, 6) == "属性使用不能" && cond.Lifetime > 0)
                                        {
                                            t.DeleteCondition(cond.Name);
                                            is_useful = true;
                                        }
                                    }

                                    if (is_useful)
                                    {
                                        if (ReferenceEquals(t, CurrentForm()))
                                        {
                                            GUI.UpdateMessageForm(t, u2: null);
                                        }
                                        else
                                        {
                                            GUI.UpdateMessageForm(t, CurrentForm());
                                        }

                                        GUI.DisplaySysMessage((string)(t.Nickname + "の状態が回復した。"));
                                    }
                                }
                                else
                                {
                                    // 指定されたステータス異常のみを回復
                                    foreach (var cname0 in GeneralLib.ToL(edata))
                                    {
                                        var cname = cname0;
                                        if (t.ConditionLifetime(cname) > 0)
                                        {
                                            t.DeleteCondition(cname);
                                            if (ReferenceEquals(t, CurrentForm()))
                                            {
                                                GUI.UpdateMessageForm(t, u2: null);
                                            }
                                            else
                                            {
                                                GUI.UpdateMessageForm(t, CurrentForm());
                                            }

                                            if (cname == "装甲劣化")
                                            {
                                                cname = Expression.Term("装甲", t) + "劣化";
                                            }

                                            GUI.DisplaySysMessage(t.Nickname + "の[" + cname + "]が回復した。");
                                            is_useful = true;
                                        }
                                    }
                                }
                            }

                            break;
                        }

                    case "付加":
                        {
                            {
                                if (elevel2 == 0d)
                                {
                                    // レベル指定がない場合は付加が半永久的に持続
                                    elevel2 = 10000d;
                                }
                                else
                                {
                                    // そうでなければ最低１ターンは効果が持続
                                    elevel2 = GeneralLib.MaxLng((int)elevel2, 1);
                                }

                                // 効果時間が継続中？
                                if (t.IsConditionSatisfied(GeneralLib.LIndex(edata, 1) + "付加"))
                                {
                                    goto NextLoop;
                                }

                                var ftype = GeneralLib.LIndex(edata, 1);
                                var flevel = Conversions.ToDouble(GeneralLib.LIndex(edata, 2));
                                var fdata = "";
                                var loopTo20 = GeneralLib.LLength(edata);
                                for (var j = 3; j <= loopTo20; j++)
                                    fdata = fdata + GeneralLib.LIndex(edata, j) + " ";
                                fdata = Strings.Trim(fdata);
                                if (Strings.Left(fdata, 1) == "\"" && Strings.Right(fdata, 1) == "\"")
                                {
                                    fdata = Strings.Trim(Strings.Mid(fdata, 2, Strings.Len(fdata) - 2));
                                }

                                // エリアスが定義されている？
                                if (SRC.ALDList.IsDefined(ftype))
                                {
                                    var adata = SRC.ALDList.Item(ftype);
                                    foreach (var elm in adata.Elements)
                                    {
                                        // エリアスの定義に従って特殊能力定義を置き換える
                                        var ftype2 = elm.strAliasType;
                                        double flevel2;
                                        string fdata2;

                                        if (GeneralLib.LIndex(elm.strAliasData, 1) == "解説")
                                        {
                                            // 特殊能力の解説
                                            if (!string.IsNullOrEmpty(fdata))
                                            {
                                                ftype2 = GeneralLib.LIndex(fdata, 1);
                                            }

                                            flevel2 = Constants.DEFAULT_LEVEL;
                                            fdata2 = elm.strAliasData;
                                        }
                                        else
                                        {
                                            // 通常の特殊能力
                                            if (elm.blnAliasLevelIsPlusMod)
                                            {
                                                if (flevel == Constants.DEFAULT_LEVEL)
                                                {
                                                    flevel = 1d;
                                                }

                                                flevel2 = flevel + elm.dblAliasLevel;
                                            }
                                            else if (elm.blnAliasLevelIsMultMod)
                                            {
                                                if (flevel == Constants.DEFAULT_LEVEL)
                                                {
                                                    flevel = 1d;
                                                }

                                                flevel2 = flevel * elm.dblAliasLevel;
                                            }
                                            else if (flevel != Constants.DEFAULT_LEVEL)
                                            {
                                                flevel2 = flevel;
                                            }
                                            else
                                            {
                                                flevel2 = elm.dblAliasLevel;
                                            }

                                            fdata2 = elm.strAliasData;
                                            if (!string.IsNullOrEmpty(fdata))
                                            {
                                                if (Strings.InStr(fdata2, "非表示") != 1)
                                                {
                                                    fdata2 = fdata + " " + GeneralLib.ListTail(fdata2, GeneralLib.LLength(fdata) + 1);
                                                }
                                            }
                                        }

                                        t.AddCondition(ftype2 + "付加", (int)elevel2, flevel2, fdata2);
                                    }
                                }
                                else
                                {
                                    t.AddCondition(ftype + "付加", (int)elevel2, flevel, fdata);
                                }

                                t.Update();
                                if (ReferenceEquals(t, CurrentForm()))
                                {
                                    GUI.UpdateMessageForm(t, u2: null);
                                }
                                else
                                {
                                    GUI.UpdateMessageForm(t, CurrentForm());
                                }

                                switch (GeneralLib.LIndex(edata, 1) ?? "")
                                {
                                    case "耐性":
                                    case "無効化":
                                    case "吸収":
                                        {
                                            GUI.DisplaySysMessage((string)(t.Nickname + "は[" + GeneralLib.LIndex(edata, 3) + "]属性に対する[" + GeneralLib.LIndex(edata, 1) + "]能力を得た。"));
                                            break;
                                        }

                                    case "特殊効果無効化":
                                        {
                                            GUI.DisplaySysMessage((string)(t.Nickname + "は[" + GeneralLib.LIndex(edata, 3) + "]属性に対する無効化能力を得た。"));
                                            break;
                                        }

                                    case "攻撃属性":
                                        {
                                            GUI.DisplaySysMessage((string)(t.Nickname + "は[" + GeneralLib.LIndex(edata, 3) + "]の攻撃属性を得た。"));
                                            break;
                                        }

                                    case "武器強化":
                                        {
                                            GUI.DisplaySysMessage((string)(t.Nickname + "の" + "武器の攻撃力が上がった。"));
                                            break;
                                        }

                                    case "命中率強化":
                                        {
                                            GUI.DisplaySysMessage((string)(t.Nickname + "の" + "武器の命中率が上がった。"));
                                            break;
                                        }

                                    case "ＣＴ率強化":
                                        {
                                            GUI.DisplaySysMessage((string)(t.Nickname + "の" + "武器のＣＴ率が上がった。"));
                                            break;
                                        }

                                    case "特殊効果発動率強化":
                                        {
                                            GUI.DisplaySysMessage((string)(t.Nickname + "の" + "武器の特殊効果発動率が上がった。"));
                                            break;
                                        }

                                    case "射程延長":
                                        {
                                            GUI.DisplaySysMessage((string)(t.Nickname + "の" + "武器の射程が伸びた。"));
                                            break;
                                        }

                                    case "サイズ変更":
                                        {
                                            GUI.DisplaySysMessage((string)(t.Nickname + "の" + "サイズが" + Strings.StrConv(GeneralLib.LIndex(edata, 3), VbStrConv.Wide) + "サイズに変化した。"));
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
                                            var fname = GeneralLib.ListIndex(fdata, 1);
                                            if (string.IsNullOrEmpty(fname) || fname == "非表示")
                                            {
                                                if ((GeneralLib.LIndex(edata, 2) ?? "") != (SrcFormatter.Format(Constants.DEFAULT_LEVEL) ?? ""))
                                                {
                                                    fname = GeneralLib.LIndex(edata, 1) + "Lv" + GeneralLib.LIndex(edata, 2);
                                                }
                                                else
                                                {
                                                    fname = GeneralLib.LIndex(edata, 1);
                                                }
                                            }

                                            GUI.DisplaySysMessage(t.Nickname + "は[" + fname + "]の能力を得た。");
                                            break;
                                        }
                                }

                                if (a.AbilityMaxRange() > 0)
                                {
                                    is_useful = true;
                                }
                            }

                            break;
                        }

                    case "強化":
                        {
                            {
                                if (elevel2 == 0d)
                                {
                                    // レベル指定がない場合は付加が半永久的に持続
                                    elevel2 = 10000d;
                                }
                                else
                                {
                                    // そうでなければ最低１ターンは効果が持続
                                    elevel2 = GeneralLib.MaxLng((int)elevel2, 1);
                                }

                                // 効果時間が継続中？
                                if (t.IsConditionSatisfied(GeneralLib.LIndex(edata, 1) + "強化"))
                                {
                                    goto NextLoop;
                                }

                                var ftype = GeneralLib.LIndex(edata, 1);
                                var flevel = Conversions.ToDouble(GeneralLib.LIndex(edata, 2));
                                var fdata = "";
                                var loopTo22 = GeneralLib.LLength(edata);
                                for (var j = 3; j <= loopTo22; j++)
                                    fdata = fdata + GeneralLib.LIndex(edata, j) + " ";
                                fdata = Strings.Trim(fdata);

                                // エリアスが定義されている？
                                if (SRC.ALDList.IsDefined(ftype))
                                {
                                    var adata = SRC.ALDList.Item(ftype);
                                    foreach (var elm in adata.Elements)
                                    {
                                        // エリアスの定義に従って特殊能力定義を置き換える
                                        var ftype2 = elm.strAliasType;
                                        double flevel2;
                                        string fdata2;

                                        if (GeneralLib.LIndex(elm.strAliasData, 1) == "解説")
                                        {
                                            // 特殊能力の解説
                                            if (!string.IsNullOrEmpty(fdata))
                                            {
                                                ftype2 = GeneralLib.LIndex(fdata, 1);
                                            }

                                            flevel2 = Constants.DEFAULT_LEVEL;
                                            fdata2 = elm.strAliasData;
                                            t.AddCondition(ftype2 + "付加", (int)elevel2, flevel2, fdata2);
                                        }
                                        else
                                        {
                                            // 通常の特殊能力
                                            if (elm.blnAliasLevelIsMultMod)
                                            {
                                                if (flevel == Constants.DEFAULT_LEVEL)
                                                {
                                                    flevel = 1d;
                                                }

                                                flevel2 = flevel * elm.dblAliasLevel;
                                            }
                                            else if (flevel != Constants.DEFAULT_LEVEL)
                                            {
                                                flevel2 = flevel;
                                            }
                                            else
                                            {
                                                flevel2 = elm.dblAliasLevel;
                                            }

                                            fdata2 = elm.strAliasData;
                                            if (!string.IsNullOrEmpty(fdata))
                                            {
                                                if (Strings.InStr(fdata2, "非表示") != 1)
                                                {
                                                    fdata2 = fdata + " " + GeneralLib.ListTail(fdata2, GeneralLib.LLength(fdata) + 1);
                                                }
                                            }

                                            t.AddCondition(ftype2 + "強化", (int)elevel2, flevel2, fdata2);
                                        }
                                    }
                                }
                                else
                                {
                                    t.AddCondition(ftype + "強化", (int)elevel2, flevel, fdata);
                                }

                                t.Update();
                                if (ReferenceEquals(t, CurrentForm()))
                                {
                                    GUI.UpdateMessageForm(t, u2: null);
                                }
                                else
                                {
                                    GUI.UpdateMessageForm(t, CurrentForm());
                                }

                                // 強化する能力名
                                var fname = GeneralLib.LIndex(edata, 3);
                                if (string.IsNullOrEmpty(fname) || fname == "非表示")
                                {
                                    fname = GeneralLib.LIndex(edata, 1);
                                }

                                if (t.SkillName0(fname) != "非表示")
                                {
                                    fname = t.SkillName0(fname);
                                }

                                GUI.DisplaySysMessage(t.Nickname + "の[" + fname + "]レベルが" + GeneralLib.LIndex(edata, 2) + "上がった。");
                                if (a.AbilityMaxRange() > 0)
                                {
                                    is_useful = true;
                                }
                            }

                            break;
                        }

                    case "状態":
                        {
                            {
                                if (elevel2 == 0d)
                                {
                                    // レベル指定がない場合は付加が半永久的に持続
                                    elevel2 = 10000d;
                                }
                                else
                                {
                                    // そうでなければ最低１ターンは状態が持続
                                    elevel = GeneralLib.MaxLng((int)elevel2, 1);
                                }

                                // 効果時間が継続中？
                                if (t.IsConditionSatisfied(edata))
                                {
                                    goto NextLoop;
                                }

                                t.AddCondition(edata, (int)elevel2, cdata: "");

                                // 状態発動アニメーション表示
                                bool localIsAnimationDefined() { string argmain_situation = aname + "(発動)"; string argsub_situation = ""; var ret = IsAnimationDefined(argmain_situation, sub_situation: argsub_situation); return ret; }

                                if (!localIsAnimationDefined() && !IsAnimationDefined(aname, sub_situation: ""))
                                {
                                    switch (edata ?? "")
                                    {
                                        case "攻撃力ＵＰ":
                                        case "防御力ＵＰ":
                                        case "運動性ＵＰ":
                                        case "移動力ＵＰ":
                                        case "狂戦士":
                                            {
                                                Effect.ShowAnimation(edata + "発動");
                                                break;
                                            }
                                    }
                                }

                                var cname = "";
                                switch (edata ?? "")
                                {
                                    case "装甲劣化":
                                        {
                                            cname = Expression.Term("装甲", t) + "劣化";
                                            break;
                                        }

                                    case "運動性ＵＰ":
                                        {
                                            cname = Expression.Term("運動性", t) + "ＵＰ";
                                            break;
                                        }

                                    case "運動性ＤＯＷＮ":
                                        {
                                            cname = Expression.Term("運動性", t) + "ＤＯＷＮ";
                                            break;
                                        }

                                    case "移動力ＵＰ":
                                        {
                                            cname = Expression.Term("移動力", t) + "ＵＰ";
                                            break;
                                        }

                                    case "移動力ＤＯＷＮ":
                                        {
                                            cname = Expression.Term("移動力", t) + "ＤＯＷＮ";
                                            break;
                                        }

                                    default:
                                        {
                                            cname = edata;
                                            break;
                                        }
                                }

                                GUI.DisplaySysMessage(t.Nickname + "は" + cname + "の状態になった。");
                                if (a.AbilityMaxRange() > 0)
                                {
                                    is_useful = true;
                                }
                            }

                            break;
                        }

                    case "召喚":
                        {
                            GUI.UpdateMessageForm(CurrentForm(), u2: null);

                            if (!SRC.UDList.IsDefined(edata))
                            {
                                GUI.ErrorMessage(edata + "のデータが定義されていません");
                                return ExecuteAbilityRet;
                            }

                            var pname = SRC.UDList.Item(edata).FeatureData("追加パイロット");
                            if (!SRC.PDList.IsDefined(pname))
                            {
                                GUI.ErrorMessage("追加パイロット「" + pname + "」のデータがありません");
                                return ExecuteAbilityRet;
                            }

                            // 召喚したユニットを配置する座標を決定する。
                            // 最も近い敵ユニットの方向にユニットを配置する。
                            var u = COM.SearchNearestEnemy(this);
                            int tx, ty, tx2, ty2;
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

                            var loopTo24 = GeneralLib.MaxLng((int)elevel, 1);
                            for (var j = 1; j <= loopTo24; j++)
                            {
                                Pilot p;
                                var pd = SRC.PDList.Item(pname);
                                if (Strings.InStr(pd.Name, "(ザコ)") > 0 || Strings.InStr(pd.Name, "(汎用)") > 0)
                                {
                                    p = SRC.PList.Add(pname, MainPilot().Level, Party, gid: "");
                                    p.FullRecover();
                                    u = SRC.UList.Add(edata, Rank, Party);
                                }
                                else
                                {
                                    if (!SRC.PList.IsDefined(pname))
                                    {
                                        p = SRC.PList.Add(pname, MainPilot().Level, Party, gid: "");
                                        p.FullRecover();
                                        u = SRC.UList.Add(edata, Rank, Party);
                                    }
                                    else
                                    {
                                        p = SRC.PList.Item(pname);
                                        u = p.Unit;
                                        if (u is null)
                                        {
                                            if (SRC.UList.IsDefined(edata))
                                            {
                                                u = SRC.UList.Item(edata);
                                            }
                                            else
                                            {
                                                u = SRC.UList.Add(edata, Rank, Party);
                                            }
                                        }
                                    }
                                }

                                p.Ride(u);
                                AddServant(u);
                                if (Party == "味方")
                                {
                                    if (GeneralLib.LIndex(u.FeatureData("召喚ユニット"), 2) == "ＮＰＣ")
                                    {
                                        u.ChangeParty("ＮＰＣ");
                                    }
                                }

                                u.Summoner = CurrentForm();
                                u.FullRecover();
                                u.Mode = MainPilot().ID;
                                u.UsedAction = 0;
                                if (u.IsFeatureAvailable("制限時間"))
                                {
                                    u.AddCondition("残り時間", Conversions.ToInteger(u.FeatureData("制限時間")), cdata: "");
                                }

                                if (u.IsMessageDefined("発進"))
                                {
                                    if (!GUI.MainFormVisible)
                                    {
                                        GUI.OpenMessageForm(this, u2: null);
                                    }

                                    u.PilotMessage("発進", msg_mode: "");
                                }

                                // ユニットを配置
                                if (Map.MapDataForUnit[tx, ty] is null && u.IsAbleToEnter(tx, ty))
                                {
                                    u.StandBy(tx, ty, "出撃");
                                }
                                else if (Map.MapDataForUnit[tx2, ty2] is null && u.IsAbleToEnter(tx2, ty2))
                                {
                                    u.StandBy(tx2, ty2, "出撃");
                                }
                                else
                                {
                                    u.StandBy(x, y, "出撃");
                                }

                                // ちゃんと配置できた？
                                if (u.Status == "待機")
                                {
                                    // 空いた場所がなく出撃出来なかった場合
                                    GUI.DisplaySysMessage(Nickname + "は" + u.Nickname + "の召喚に失敗した。");
                                    DeleteServant(u.ID);
                                    u.Status = "破棄";
                                }
                            }

                            break;
                        }

                    case "変身":
                        {
                            // 既に変身している場合は変身出来ない
                            if (t.IsFeatureAvailable("ノーマルモード"))
                            {
                                goto NextLoop;
                            }

                            var buf = t.Name;
                            t.Transform(GeneralLib.LIndex(edata, 1));
                            t = t.CurrentForm();
                            if (elevel2 > 0d)
                            {
                                t.AddCondition("残り時間", GeneralLib.MaxLng((int)elevel2, 1), cdata: "");
                            }

                            var loopTo25 = GeneralLib.LLength(edata);
                            for (var j = 2; j <= loopTo25; j++)
                                buf = buf + " " + GeneralLib.LIndex(edata, j);
                            t.AddCondition("ノーマルモード付加", -1, 1d, buf);

                            // 変身した場合はそこで終わり
                            break;
                        }

                    case "能力コピー":
                        {
                            // 既に変身している場合は能力コピー出来ない
                            if (IsFeatureAvailable("ノーマルモード"))
                            {
                                goto NextLoop;
                            }

                            Transform(t.Name);
                            {
                                var cf = CurrentForm();
                                if (elevel2 > 0d)
                                {
                                    cf.AddCondition("残り時間", GeneralLib.MaxLng((int)elevel2, 1), cdata: "");
                                }

                                // 元の形態に戻れるように設定
                                var buf = Name;
                                var loopTo26 = GeneralLib.LLength(edata);
                                for (var j = 1; j <= loopTo26; j++)
                                    buf = buf + " " + GeneralLib.LIndex(edata, j);
                                cf.AddCondition("ノーマルモード付加", -1, 1d, buf);
                                cf.AddCondition("能力コピー", -1, cdata: "");

                                // コピー元のパイロット画像とメッセージを使うように設定
                                cf.AddCondition("パイロット画像", -1, 0d, "非表示 " + t.MainPilot().get_Bitmap(false));
                                cf.AddCondition("メッセージ", -1, 0d, "非表示 " + t.MainPilot().MessageType);
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
                                if (t.Action == 0 && t.MaxAction() > 0)
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
            foreach (var pu in partners)
            {
                var cf = pu.CurrentForm();
                bool abilityFound = false;
                for (var aj = 1; aj <= cf.CountAbility(); aj++)
                {
                    // パートナーが同名のアビリティを持っていればそのアビリティのデータを使う
                    if (cf.Ability(aj).Data.Name == aname)
                    {
                        cf.Ability(aj).UseAbility();
                        abilityFound = true;
                        break;
                    }
                }

                // 同名のアビリティがなかった場合は自分のデータを使って処理
                if (!abilityFound)
                {
                    if (a.AbilityENConsumption() > 0)
                    {
                        cf.EN = cf.EN - a.AbilityENConsumption();
                    }

                    if (a.IsAbilityClassifiedAs("消"))
                    {
                        cf.AddCondition("消耗", 1, cdata: "");
                    }

                    if (a.IsAbilityClassifiedAs("Ｃ") && cf.IsConditionSatisfied("チャージ完了"))
                    {
                        cf.DeleteCondition("チャージ完了");
                    }

                    if (a.IsAbilityClassifiedAs("気"))
                    {
                        cf.IncreaseMorale((int)(-5 * a.AbilityLevel("気")));
                    }
                }
            }

            // 変身した場合
            if (Status == "他形態")
            {
                var cf = CurrentForm();
                // 使い捨てアイテムによる変身の処理
                for (var i = 1; i <= cf.CountAbility(); i++)
                {
                    if ((cf.Ability(i).Data.Name ?? "") == (aname ?? ""))
                    {
                        if (cf.Ability(i).Data.IsItem() && cf.Ability(i).Stock() == 0 && cf.Ability(i).MaxStock() > 0)
                        {
                            var itm = cf.ItemList.FirstOrDefault(item => item.Abilities.Any(x => x.Name == aname));
                            if (itm != null)
                            {
                                itm.Exist = false;
                                cf.DeleteItem(itm);
                                cf.Update();
                            }
                            break;
                        }
                    }
                }

                // 自殺？
                if (cf.HP == 0)
                {
                    cf.Die();
                }

                // WaitCommandによる画面クリアが行われないので
                GUI.RedrawScreen();
                return ExecuteAbilityRet;
            }

            // 経験値の獲得
            if (is_useful && !is_event && !Expression.IsOptionDefined("アビリティ経験値無効"))
            {
                GetExp(t, "アビリティ", exp_mode: "");
                if (!Expression.IsOptionDefined("合体技パートナー経験値無効"))
                {
                    foreach (var pu in partners)
                    {
                        pu.CurrentForm().GetExp(t, "アビリティ", "パートナー");
                    }
                }
            }

            // 以下の効果はアビリティデータが変化する場合があるため同時には適応されない
            // 自爆技
            // ＨＰ消費アビリティで自殺
            // 変形技
            if (a.IsAbilityClassifiedAs("自"))
            {
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
            else if (a.IsAbilityClassifiedAs("失") && HP == 0)
            {
                Die();
            }
            else if (a.IsAbilityClassifiedAs("変"))
            {
                if (IsFeatureAvailable("変形技"))
                {
                    var uname = "";
                    var fd = Features.FirstOrDefault(x => x.Name == "変形技" && x.DataL[0] == aname);
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
            else if (a.Data.IsItem() && a.Stock() == 0 && a.MaxStock() > 0)
            {
                // アイテムを削除
                var itm = ItemList.FirstOrDefault(itm => itm.Abilities.Any(x => x.Name == a.Data.Name));
                if (itm != null)
                {
                    itm.Exist = false;
                    DeleteItem(itm);
                }
            }

            // 戦闘アニメ終了処理
            if (IsAnimationDefined(aname + "(終了)", sub_situation: ""))
            {
                PlayAnimation(aname + "(終了)", sub_situation: "");
            }
            else if (IsAnimationDefined("終了", sub_situation: ""))
            {
                PlayAnimation("終了", sub_situation: "");
            }

            {
                var withBlock39 = CurrentForm();
                // 戦闘アニメで変更されたユニット画像を元に戻す
                if (withBlock39.IsConditionSatisfied("ユニット画像"))
                {
                    withBlock39.DeleteCondition("ユニット画像");
                    GUI.PaintUnitBitmap(CurrentForm());
                }

                if (withBlock39.IsConditionSatisfied("非表示付加"))
                {
                    withBlock39.DeleteCondition("非表示付加");
                    GUI.PaintUnitBitmap(CurrentForm());
                }
            }

            foreach (var pu in partners)
            {
                {
                    var cf = pu.CurrentForm();
                    if (cf.IsConditionSatisfied("ユニット画像"))
                    {
                        cf.DeleteCondition("ユニット画像");
                        GUI.PaintUnitBitmap(cf);
                    }

                    if (cf.IsConditionSatisfied("非表示付加"))
                    {
                        cf.DeleteCondition("非表示付加");
                        GUI.PaintUnitBitmap(cf);
                    }
                }
            }

            return ExecuteAbilityRet;
        }

        // マップアビリティ a を (tx,ty) に使用
        public void ExecuteMapAbility(UnitAbility a, int tx, int ty, bool is_event = false)
        {
            var aname = a.Data.Name;
            var anickname = a.AbilityNickname();
            if (!is_event)
            {
                // マップ攻撃の使用イベント
                Event.HandleEvent("使用", MainPilot().ID, aname);
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
            var min_range = a.AbilityMinRange();
            var max_range = a.AbilityMaxRange();
            if (a.IsAbilityClassifiedAs("Ｍ直"))
            {
                if (ty < y)
                {
                    Map.AreaInLine(x, y, min_range, max_range, "N");
                }
                else if (ty > y)
                {
                    Map.AreaInLine(x, y, min_range, max_range, "S");
                }
                else if (tx < x)
                {
                    Map.AreaInLine(x, y, min_range, max_range, "W");
                }
                else
                {
                    Map.AreaInLine(x, y, min_range, max_range, "E");
                }
            }
            else if (a.IsAbilityClassifiedAs("Ｍ拡"))
            {
                if (ty < y && Math.Abs((y - ty)) > Math.Abs((x - tx)))
                {
                    Map.AreaInCone(x, y, min_range, max_range, "N");
                }
                else if (ty > y && Math.Abs((y - ty)) > Math.Abs((x - tx)))
                {
                    Map.AreaInCone(x, y, min_range, max_range, "S");
                }
                else if (tx < x && Math.Abs((x - tx)) > Math.Abs((y - ty)))
                {
                    Map.AreaInCone(x, y, min_range, max_range, "W");
                }
                else
                {
                    Map.AreaInCone(x, y, min_range, max_range, "E");
                }
            }
            else if (a.IsAbilityClassifiedAs("Ｍ扇"))
            {
                if (ty < y && Math.Abs((y - ty)) >= Math.Abs((x - tx)))
                {
                    Map.AreaInSector(x, y, min_range, max_range, "N", (int)a.AbilityLevel("Ｍ扇"));
                }
                else if (ty > y && Math.Abs((y - ty)) >= Math.Abs((x - tx)))
                {
                    Map.AreaInSector(x, y, min_range, max_range, "S", (int)a.AbilityLevel("Ｍ扇"));
                }
                else if (tx < x && Math.Abs((x - tx)) >= Math.Abs((y - ty)))
                {
                    Map.AreaInSector(x, y, min_range, max_range, "W", (int)a.AbilityLevel("Ｍ扇"));
                }
                else
                {
                    Map.AreaInSector(x, y, min_range, max_range, "E", (int)a.AbilityLevel("Ｍ扇"));
                }
            }
            else if (a.IsAbilityClassifiedAs("Ｍ投"))
            {
                Map.AreaInRange(tx, ty, (int)a.AbilityLevel("Ｍ投"), 1, "すべて");
            }
            else if (a.IsAbilityClassifiedAs("Ｍ全"))
            {
                Map.AreaInRange(x, y, max_range, min_range, "すべて");
            }
            else if (a.IsAbilityClassifiedAs("Ｍ移") || a.IsAbilityClassifiedAs("Ｍ線"))
            {
                Map.AreaInPointToPoint(x, y, tx, ty);
            }

            // ユニットがいるマスの処理
            for (var i = 1; i <= Map.MapWidth; i++)
            {
                for (var j = 1; j <= Map.MapHeight; j++)
                {
                    if (!Map.MaskData[i, j])
                    {
                        var t = Map.MapDataForUnit[i, j];
                        if (t is object)
                        {
                            // 有効？
                            if (a.IsAbilityEffective(t))
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
            if (a.IsAbilityClassifiedAs("援"))
            {
                Map.MaskData[x, y] = true;
            }

            // マップアビリティの影響を受けるユニットのリストを作成
            var targets = new List<Unit>();
            for (var i = 1; i <= Map.MapWidth; i++)
            {
                for (var j = 1; j <= Map.MapHeight; j++)
                {
                    // マップアビリティの影響をうけるかチェック
                    if (Map.MaskData[i, j])
                    {
                        continue;
                    }

                    var t = Map.MapDataForUnit[i, j];
                    if (t is null)
                    {
                        continue;
                    }

                    if (!a.IsAbilityApplicable(t))
                    {
                        Map.MaskData[i, j] = true;
                        continue;
                    }

                    targets.Add(t);
                }
            }

            int rx, ry;
            // アビリティ実行の起点を設定
            if (a.IsAbilityClassifiedAs("Ｍ投"))
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
            targets = targets.OrderBy(t => Math.Abs(t.x - rx) + Math.Abs(t.y - ry)).ToList();

            // 合体技
            var partners = a.CombinationPartner();
            if (a.IsAbilityClassifiedAs("合"))
            {
                // 合体技のパートナーのハイライト表示
                // MaskDataを保存して使用している
                var tmpMaskData = new bool[Map.MapWidth + 1, Map.MapHeight + 1];
                for (var i = 1; i <= Map.MapWidth; i++)
                {
                    for (var j = 1; j <= Map.MapHeight; j++)
                    {
                        tmpMaskData[i, j] = Map.MaskData[i, j];
                    }
                }

                // パートナーユニットはマスクを解除
                foreach (var pu in partners)
                {
                    Map.MaskData[pu.x, pu.y] = false;
                    tmpMaskData[pu.x, pu.y] = true;
                }

                GUI.MaskScreen();

                // マスクを復元
                for (var i = 1; i <= Map.MapWidth; i++)
                {
                    for (var j = 1; j <= Map.MapHeight; j++)
                    {
                        Map.MaskData[i, j] = tmpMaskData[i, j];
                    }
                }
            }
            else
            {
                Commands.SelectedPartners.Clear();
                GUI.MaskScreen();
            }

            GUI.OpenMessageForm(this, u2: null);

            // 現在の選択状況をセーブ
            Commands.SaveSelections();

            // 選択内容を切り替え
            Commands.SelectedUnit = this;
            Event.SelectedUnitForEvent = this;
            Commands.SelectedAbility = a.AbilityNo();
            Commands.SelectedAbilityName = a.Data.Name;
            Commands.SelectedX = tx;
            Commands.SelectedY = ty;

            // 変な「対～」メッセージが表示されないようにターゲットをオフ
            Commands.SelectedTarget = null;
            Event.SelectedTargetForEvent = null;

            // マップアビリティ開始のメッセージ＆特殊効果
            if (IsAnimationDefined(aname + "(準備)", sub_situation: ""))
            {
                PlayAnimation(aname + "(準備)", sub_situation: "");
            }

            if (IsMessageDefined("かけ声(" + aname + ")"))
            {
                PilotMessage("かけ声(" + aname + ")", msg_mode: "");
            }

            PilotMessage(aname, "アビリティ");
            if (IsAnimationDefined(aname + "(使用)", sub_situation: ""))
            {
                PlayAnimation(aname + "(使用)", "", true);
            }
            else
            {
                SpecialEffect(aname, "", true);
            }

            // ＥＮ消費＆使用回数減少
            a.UseAbility();
            GUI.UpdateMessageForm(this, u2: null);
            var msg = "";
            switch (partners.Count)
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
                        if ((Nickname ?? "") != (partners[0].Nickname ?? ""))
                        {
                            msg = Nickname + "は[" + partners[0].Nickname + "]と共に";
                        }
                        else if ((MainPilot().get_Nickname(false) ?? "") != (partners[0].MainPilot().get_Nickname(false) ?? ""))
                        {
                            msg = MainPilot().get_Nickname(false) + "と[" + partners[0].MainPilot().get_Nickname(false) + "]の[" + Nickname + "]は";
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
                        if ((Nickname ?? "") != (partners[0].Nickname ?? ""))
                        {
                            msg = Nickname + "は[" + partners[0].Nickname + "]、[" + partners[1].Nickname + "]と共に";
                        }
                        else if ((MainPilot().get_Nickname(false) ?? "") != (partners[0].MainPilot().get_Nickname(false) ?? ""))
                        {
                            msg = MainPilot().get_Nickname(false) + "、[" + partners[0].MainPilot().get_Nickname(false) + "]、[" + partners[1].MainPilot().get_Nickname(false) + "]の[" + Nickname + "]は";
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

            if (a.IsSpellAbility())
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

            if (IsSysMessageDefined(aname, sub_situation: ""))
            {
                // 「アビリティ名(解説)」のメッセージを使用
                SysMessage(aname, sub_situation: "", add_msg: "");
            }
            else if (IsSysMessageDefined("アビリティ", sub_situation: ""))
            {
                // 「アビリティ(解説)」のメッセージを使用
                SysMessage("アビリティ", sub_situation: "", add_msg: "");
            }
            else
            {
                GUI.DisplaySysMessage(msg);
            }

            // 選択状況を復元
            Commands.RestoreSelections();

            // アビリティの使用に失敗？
            if (GeneralLib.Dice(10) <= a.AbilityLevel("難"))
            {
                GUI.DisplaySysMessage("しかし何もおきなかった…");
                goto Finish;
            }

            // 使用元ユニットは SelectedTarget に設定していないといけない
            Commands.SelectedTarget = this;

            // 各ユニットにアビリティを使用
            var is_useful = false;
            Unit max_lv_t = null;
            foreach (var tu in targets)
            {
                var t = tu.CurrentForm();
                if (t.Status == "出撃")
                {
                    if (ReferenceEquals(t, this))
                    {
                        GUI.UpdateMessageForm(this, u2: null);
                    }
                    else
                    {
                        GUI.UpdateMessageForm(t, this);
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

            // 戦闘アニメ終了処理
            if (IsAnimationDefined(aname + "(終了)", sub_situation: ""))
            {
                PlayAnimation(aname + "(終了)", sub_situation: "");
            }
            else if (IsAnimationDefined("終了", sub_situation: ""))
            {
                PlayAnimation("終了", sub_situation: "");
            }

            {
                var withBlock3 = CurrentForm();
                // 戦闘アニメで変更されたユニット画像を元に戻す
                if (withBlock3.IsConditionSatisfied("ユニット画像"))
                {
                    withBlock3.DeleteCondition("ユニット画像");
                    GUI.PaintUnitBitmap(CurrentForm());
                }

                if (withBlock3.IsConditionSatisfied("非表示付加"))
                {
                    withBlock3.DeleteCondition("非表示付加");
                    GUI.PaintUnitBitmap(CurrentForm());
                }
            }

            foreach (var pu in partners)
            {
                {
                    var cf = pu.CurrentForm();
                    if (cf.IsConditionSatisfied("ユニット画像"))
                    {
                        cf.DeleteCondition("ユニット画像");
                        GUI.PaintUnitBitmap(cf);
                    }

                    if (cf.IsConditionSatisfied("非表示付加"))
                    {
                        cf.DeleteCondition("非表示付加");
                        GUI.PaintUnitBitmap(cf);
                    }
                }
            }

            // 獲得した経験値の表示
            if (is_useful && !is_event && !Expression.IsOptionDefined("アビリティ経験値無効"))
            {
                GetExp(max_lv_t, "アビリティ", exp_mode: "");
                if (!Expression.IsOptionDefined("合体技パートナー経験値無効"))
                {
                    foreach (var pu in partners)
                    {
                        pu.CurrentForm().GetExp(null, "アビリティ", "パートナー");
                    }
                }
            }

            // 合体技のパートナーの弾数＆ＥＮの消費
            foreach (var pu in partners)
            {
                var cf = pu.CurrentForm();
                var cfa = cf.Abilities.FirstOrDefault(x => x.Data.Name == aname);
                if (cfa != null)
                {
                    cfa.UseAbility();

                    if (cfa.IsAbilityClassifiedAs("自"))
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
                    else if (cfa.IsAbilityClassifiedAs("失") && cf.HP == 0)
                    {
                        cf.Die();
                    }
                    else if (cfa.IsAbilityClassifiedAs("変"))
                    {
                        // XXX 変形技
                        if (cf.IsFeatureAvailable("変形技"))
                        {
                            var uname = "";
                            var fd = cf.Features.FirstOrDefault(x => x.Name == "変形技" && x.DataL[0] == aname);
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
                    if (a.Data.ENConsumption > 0)
                    {
                        cf.EN = cf.EN - a.AbilityENConsumption();
                    }

                    if (a.IsAbilityClassifiedAs("消"))
                    {
                        cf.AddCondition("消耗", 1, cdata: "");
                    }

                    if (a.IsAbilityClassifiedAs("Ｃ") && cf.IsConditionSatisfied("チャージ完了"))
                    {
                        cf.DeleteCondition("チャージ完了");
                    }

                    if (a.IsAbilityClassifiedAs("気"))
                    {
                        cf.IncreaseMorale((int)(-5 * a.AbilityLevel("気")));
                    }

                    if (a.IsAbilityClassifiedAs("霊"))
                    {
                        var hp_ratio = 100 * cf.HP / (double)cf.MaxHP;
                        var en_ratio = 100 * cf.EN / (double)cf.MaxEN;
                        cf.MainPilot().Plana = (int)(cf.MainPilot().Plana - 5d * a.AbilityLevel("霊"));
                        cf.HP = (int)(cf.MaxHP * hp_ratio / 100d);
                        cf.EN = (int)(cf.MaxEN * en_ratio / 100d);
                    }
                    else if (a.IsAbilityClassifiedAs("プ"))
                    {
                        var hp_ratio = 100 * cf.HP / (double)cf.MaxHP;
                        var en_ratio = 100 * cf.EN / (double)cf.MaxEN;
                        cf.MainPilot().Plana = (int)(cf.MainPilot().Plana - 5d * a.AbilityLevel("プ"));
                        cf.HP = (int)(cf.MaxHP * hp_ratio / 100d);
                        cf.EN = (int)(cf.MaxEN * en_ratio / 100d);
                    }

                    if (a.IsAbilityClassifiedAs("失"))
                    {
                        cf.HP = GeneralLib.MaxLng((int)(cf.HP - (long)(cf.MaxHP * a.AbilityLevel("失")) / 10L), 0);
                    }

                    if (a.IsAbilityClassifiedAs("自"))
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
                    else if (a.IsAbilityClassifiedAs("失") && cf.HP == 0)
                    {
                        cf.Die();
                    }
                    else if (a.IsAbilityClassifiedAs("変"))
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

            // 移動型マップアビリティによる移動
            if (a.IsAbilityClassifiedAs("Ｍ移"))
            {
                Jump(tx, ty);
            }

        Finish:
            ;

            // 以下の効果はアビリティデータが変化する可能性があるため、同時には適用されない
            // 自爆の処理
            // ＨＰ消費アビリティで自殺した場合
            // 変形技
            if (a.IsAbilityClassifiedAs("自"))
            {
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
            else if (a.IsAbilityClassifiedAs("失") && HP == 0)
            {
                Die();
            }
            else if (a.IsAbilityClassifiedAs("変"))
            {
                if (IsFeatureAvailable("変形技"))
                {
                    var uname = "";
                    var fd = Features.FirstOrDefault(x => x.Name == "変形技" && x.DataL[0] == aname);
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
            else if (a.Data.IsItem() && a.Stock() == 0 && a.MaxStock() > 0)
            {
                // アイテムを削除
                var itm = ItemList.FirstOrDefault(itm => itm.Abilities.Any(x => x.Name == a.Data.Name));
                if (itm != null)
                {
                    itm.Exist = false;
                    DeleteItem(itm);
                }
            }

            // 戦闘アニメ終了処理
            if (IsAnimationDefined(aname + "(終了)", sub_situation: ""))
            {
                PlayAnimation(aname + "(終了)", sub_situation: "");
            }
            else if (IsAnimationDefined("終了", sub_situation: ""))
            {
                PlayAnimation("終了", sub_situation: "");
            }

            {
                var withBlock39 = CurrentForm();
                // 戦闘アニメで変更されたユニット画像を元に戻す
                if (withBlock39.IsConditionSatisfied("ユニット画像"))
                {
                    withBlock39.DeleteCondition("ユニット画像");
                    GUI.PaintUnitBitmap(CurrentForm());
                }

                if (withBlock39.IsConditionSatisfied("非表示付加"))
                {
                    withBlock39.DeleteCondition("非表示付加");
                    GUI.PaintUnitBitmap(CurrentForm());
                }
            }

            // 使用後イベント
            if (!is_event)
            {
                Event.HandleEvent("使用後", CurrentForm().MainPilot().ID, aname);
                if (SRC.IsScenarioFinished || SRC.IsCanceled)
                {
                    return;
                }
            }

            GUI.CloseMessageForm();

            // ハイパーモード＆ノーマルモードの自動発動をチェック
            SRC.UList.CheckAutoHyperMode();
            SRC.UList.CheckAutoNormalMode();
        }

    }
}
