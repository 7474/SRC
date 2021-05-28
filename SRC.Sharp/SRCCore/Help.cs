// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Lib;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Linq;

namespace SRCCore
{
    // 特殊能力＆武器属性の解説表示を行うモジュール
    public class Help
    {
        private SRC SRC { get; }
        private IGUI GUI => SRC.GUI;
        private Expressions.Expression Expression => SRC.Expression;
        private Events.Event Event => SRC.Event;

        public Help(SRC src)
        {
            SRC = src;
        }

        // パイロット p の特殊能力の解説を表示
        public void SkillHelp(Pilot p, string sindex)
        {
            string stype, sname;
            string msg;
            bool prev_mode;

            // 特殊能力の名称を調べる
            if (Information.IsNumeric(sindex))
            {
                sname = p.SkillName(Conversions.ToInteger(sindex));
            }
            else
            {
                // 付加されたパイロット用特殊能力
                if (Strings.InStr(sindex, "Lv") > 0)
                {
                    stype = Strings.Left(sindex, Strings.InStr(sindex, "Lv") - 1);
                }
                else
                {
                    stype = sindex;
                }

                sname = p.SkillName(stype);
            }

            msg = SkillHelpMessage(p, sindex);

            // 解説の表示
            if (Strings.Len(msg) > 0)
            {
                prev_mode = GUI.AutoMessageMode;
                GUI.AutoMessageMode = false;
                GUI.OpenMessageForm(u1: null, u2: null);
                if (SRC.AutoMoveCursor)
                {
                    GUI.MoveCursorPos("メッセージウィンドウ");
                }

                GUI.DisplayMessage("システム", "<b>" + sname + "</b>;" + msg);
                GUI.CloseMessageForm();
                GUI.AutoMessageMode = prev_mode;
            }
        }

        // パイロット p の特殊能力の解説
        public string SkillHelpMessage(Pilot p, string sindex)
        {
            string SkillHelpMessageRet = default;
            string sname, stype, sname0;
            double slevel;
            string sdata;
            bool is_level_specified;
            var msg = default(string);
            Unit u, u2 = default;
            string uname, fdata;
            int i;

            // 特殊能力の名称、レベル、データを調べる
            if (Information.IsNumeric(sindex))
            {
                stype = p.Skill(Conversions.ToInteger(sindex));
                slevel = p.SkillLevel(Conversions.ToInteger(sindex), ref_mode: "");
                sdata = p.SkillData(Conversions.ToInteger(sindex));
                sname = p.SkillName(Conversions.ToInteger(sindex));
                sname0 = p.SkillName0(Conversions.ToInteger(sindex));
                is_level_specified = p.IsSkillLevelSpecified(Conversions.ToInteger(sindex));
            }
            else
            {
                // 付加されたパイロット用特殊能力
                if (Strings.InStr(sindex, "Lv") > 0)
                {
                    stype = Strings.Left(sindex, Strings.InStr(sindex, "Lv") - 1);
                }
                else
                {
                    stype = sindex;
                }

                stype = p.SkillType(stype);
                slevel = p.SkillLevel(stype, ref_mode: "");
                sdata = p.SkillData(stype);
                sname = p.SkillName(stype);
                sname0 = p.SkillName0(stype);
                is_level_specified = p.IsSkillLevelSpecified(stype);
            }

            // パイロットが乗っているユニット
            u = p.Unit;
            if (u.Name == "ステータス表示用ダミーユニット")
            {
                if (Expression.IsLocalVariableDefined("搭乗ユニット[" + p.ID + "]"))
                {
                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    uname = Conversions.ToString(Event.LocalVariableList["搭乗ユニット[" + p.ID + "]"].StringValue);
                    if (!string.IsNullOrEmpty(uname))
                    {
                        u2 = u;
                        u = SRC.UList.Item(uname);
                    }
                }
            }

            switch (stype ?? "")
            {
                case "オーラ":
                    {
                        if (u.FeatureName0("バリア") == "オーラバリア")
                        {
                            msg = "オーラ技「オ」の攻撃力と" + u.FeatureName0("オーラバリア") + "の強度に" + SrcFormatter.Format((int)(100d * slevel)) + "の修正を与える。";
                        }
                        else
                        {
                            msg = "オーラ技「オ」の攻撃力の強度に" + SrcFormatter.Format((int)(100d * slevel)) + "の修正を与える。";
                        }

                        if (u.IsFeatureAvailable("オーラ変換器"))
                        {
                            msg = msg + "また、" + Expression.Term("ＨＰ", u) + "、" + Expression.Term("ＥＮ", u) + "、" + Expression.Term("装甲", u) + "、" + Expression.Term("運動性", u: null) + "がレベルに合わせてそれぞれ増加する。";
                        }

                        break;
                    }

                case "分身":
                    {
                        msg = SrcFormatter.Format((int)((long)(100d * slevel) / 16L)) + "% の確率で分身し、攻撃を回避する。";
                        break;
                    }

                case "超感覚":
                    {
                        msg = Expression.Term("命中", u) + "・" + Expression.Term("回避", u);
                        if (slevel > 0d)
                        {
                            msg = msg + "に +" + SrcFormatter.Format((int)(2d * slevel + 3d)) + " の修正を与える。";
                        }
                        else
                        {
                            msg = msg + "に +0 の修正を与える。";
                        }

                        if (slevel > 3d)
                        {
                            msg = msg + ";思念誘導攻撃(サ)の射程を" + SrcFormatter.Format((int)((long)slevel / 4L)) + "だけ延長する。";
                        }

                        break;
                    }

                case "知覚強化":
                    {
                        msg = Expression.Term("命中", u) + "・" + Expression.Term("回避", u);
                        if (slevel > 0d)
                        {
                            msg = msg + "に +" + SrcFormatter.Format((int)(2d * slevel + 3d)) + " の修正を与える。;";
                        }
                        else
                        {
                            msg = msg + "に +0 の修正を与える。;";
                        }

                        if (slevel > 3d)
                        {
                            msg = msg + "思念誘導攻撃(サ)の射程を" + SrcFormatter.Format((int)((long)slevel / 4L)) + "だけ延長する。";
                        }

                        msg = msg + "精神不安定により" + Expression.Term("ＳＰ", u) + "消費量が20%増加する";
                        break;
                    }

                case "念力":
                    {
                        msg = Expression.Term("命中", u) + "・" + Expression.Term("回避", u);
                        if (slevel > 0d)
                        {
                            msg = msg + "に +" + SrcFormatter.Format((int)(2d * slevel + 3d)) + " の修正を与える。";
                        }
                        else
                        {
                            msg = msg + "に +0 の修正を与える。";
                        }

                        break;
                    }

                case "切り払い":
                    {
                        msg = "格闘武器(武)、突進技(突)、実弾攻撃(実)による攻撃を " + SrcFormatter.Format((int)((long)(100d * slevel) / 16L)) + "% の確率で切り払って回避する。";
                        break;
                    }

                case "迎撃":
                    {
                        msg = "実弾攻撃(実)による攻撃を " + SrcFormatter.Format((int)((long)(100d * slevel) / 16L)) + "% の確率で迎撃する。";
                        break;
                    }

                case "サイボーグ":
                    {
                        msg = Expression.Term("命中", u) + "・" + Expression.Term("回避", u);
                        msg = msg + "に +5 の修正を与える。";
                        break;
                    }

                case "Ｓ防御":
                    {
                        if (u.IsFeatureAvailable("盾"))
                        {
                            msg = "シールド防御を行い、ダメージを" + SrcFormatter.Format((int)(100d * slevel + 400d)) + "減少させる。";
                        }
                        else
                        {
                            msg = SrcFormatter.Format((int)((long)(100d * slevel) / 16L)) + "% の確率でシールド防御を行う。";
                        }

                        break;
                    }

                case "資金獲得":
                    {
                        msg = "敵を倒した時に得られる" + Expression.Term("資金", u: null);
                        if (!is_level_specified)
                        {
                            msg = msg + "が 50% 増加する。";
                        }
                        else if (slevel >= 0d)
                        {
                            msg = msg + "が " + SrcFormatter.Format(10d * slevel) + "% 増加する。";
                        }
                        else
                        {
                            msg = msg + "が " + SrcFormatter.Format(-10 * slevel) + "% 減少する。";
                        }

                        break;
                    }

                case "浄化":
                    {
                        msg = "浄化技(浄)を使うことで敵の" + p.SkillName0("再生") + "能力を無効化。";
                        break;
                    }

                case "同調率":
                    {
                        if (u.IsHero())
                        {
                            msg = "同調により";
                        }
                        else
                        {
                            msg = "機体に同調し";
                        }

                        msg = msg + Expression.Term("運動性", u) + "・攻撃力を強化する。";
                        break;
                    }

                case "同調率成長":
                    {
                        if (slevel >= 0d)
                        {
                            msg = p.SkillName0("同調率") + "の成長率が " + SrcFormatter.Format(10d * slevel) + "% 増加する。";
                        }
                        else
                        {
                            msg = p.SkillName0("同調率") + "の成長率が " + SrcFormatter.Format(-10 * slevel) + "% 減少する。";
                        }

                        break;
                    }

                case "霊力":
                    {
                        msg = "現在の" + sname0 + "値にあわせて" + Expression.Term("ＨＰ", u) + "・" + Expression.Term("ＥＮ", u) + "・" + Expression.Term("装甲", u) + "・" + Expression.Term("移動力", u) + "を強化する。";
                        break;
                    }

                case "霊力成長":
                    {
                        if (slevel >= 0d)
                        {
                            msg = p.SkillName0("霊力") + "の成長率が " + SrcFormatter.Format(10d * slevel) + "% 増加する。";
                        }
                        else
                        {
                            msg = p.SkillName0("霊力") + "の成長率が " + SrcFormatter.Format(-10 * slevel) + "% 減少する。";
                        }

                        break;
                    }

                case "底力":
                    {
                        msg = Expression.Term("ＨＰ", u) + "が最大" + Expression.Term("ＨＰ", u) + "の1/4以下の時に発動。;" + "命中＆回避 +30%、クリティカル発生率 +50%。";
                        break;
                    }

                case "超底力":
                    {
                        msg = Expression.Term("ＨＰ", u) + "が最大" + Expression.Term("ＨＰ", u) + "の1/4以下の時に発動。;" + "命中＆回避 +50%、クリティカル発生率 +50%。";
                        break;
                    }

                case "覚悟":
                    {
                        msg = Expression.Term("ＨＰ", u) + "が最大" + Expression.Term("ＨＰ", u) + "の1/4以下の時に発動。;";
                        if (Expression.IsOptionDefined("ダメージ倍率低下"))
                        {
                            msg = msg + "攻撃力10%アップ、クリティカル発生率 +50%。";
                        }
                        else
                        {
                            msg = msg + "攻撃力1.2倍、クリティカル発生率 +50%。";
                        }

                        break;
                    }

                case "不屈":
                    {
                        msg = Expression.Term("ＨＰ", u) + "が最大" + Expression.Term("ＨＰ", u) + "の1/2以下の時に発動。;" + "損傷率に応じて防御力が増加する。";
                        break;
                    }

                case "素質":
                    {
                        if (!is_level_specified)
                        {
                            msg = "入手する経験値が50%増加する。";
                        }
                        else if (slevel >= 0d)
                        {
                            msg = "入手する経験値が " + SrcFormatter.Format(10d * slevel) + "% 増加する。";
                        }
                        else
                        {
                            msg = "入手する経験値が " + SrcFormatter.Format(-10 * slevel) + "% 減少する。";
                        }

                        break;
                    }

                case "遅成長":
                    {
                        msg = "入手する経験値が半減する。";
                        break;
                    }

                case "再生":
                case "英雄":
                    {
                        msg = Expression.Term("ＨＰ", u) + "が０になった時に" + SrcFormatter.Format((int)((long)(100d * slevel) / 16L)) + "%の確率で復活する。";
                        break;
                    }

                case "超能力":
                    {
                        msg = Expression.Term("命中", u) + "・" + Expression.Term("回避", u) + "・" + Expression.Term("ＣＴ率", u) + "にそれぞれ +5。;" + "サイキック攻撃(超)の攻撃力に +" + SrcFormatter.Format((int)(100d * slevel)) + "。;" + Expression.Term("ＳＰ", u) + "消費量を20%削減する。";
                        break;
                    }

                case "悟り":
                    {
                        msg = Expression.Term("命中", u) + "・" + Expression.Term("回避", u) + "に +10 の修正を与える。";
                        break;
                    }

                case "超反応":
                    {
                        msg = Expression.Term("命中", u) + "・" + Expression.Term("回避", u) + "・" + Expression.Term("ＣＴ率", u);
                        if (slevel >= 0d)
                        {
                            msg = msg + "にそれぞれ +" + SrcFormatter.Format((int)(2d * slevel)) + " の修正を与える。";
                        }
                        else
                        {
                            msg = msg + "にそれぞれ " + SrcFormatter.Format((int)(2d * slevel)) + " の修正を与える。";
                        }

                        break;
                    }

                case "術":
                    {
                        switch (slevel)
                        {
                            case 1d:
                                {
                                    i = 0;
                                    break;
                                }

                            case 2d:
                                {
                                    i = 10;
                                    break;
                                }

                            case 3d:
                                {
                                    i = 20;
                                    break;
                                }

                            case 4d:
                                {
                                    i = 30;
                                    break;
                                }

                            case 5d:
                                {
                                    i = 40;
                                    break;
                                }

                            case 6d:
                                {
                                    i = 50;
                                    break;
                                }

                            case 7d:
                                {
                                    i = 55;
                                    break;
                                }

                            case 8d:
                                {
                                    i = 60;
                                    break;
                                }

                            case 9d:
                                {
                                    i = 65;
                                    break;
                                }

                            case var @case when @case >= 10d:
                                {
                                    i = 70;
                                    break;
                                }

                            default:
                                {
                                    i = 0;
                                    break;
                                }
                        }

                        msg = "術属性を持つ武装・" + Expression.Term("アビリティ", u) + "及び必要技能が" + sname0 + "の武装・" + Expression.Term("アビリティ", u) + "の消費" + Expression.Term("ＥＮ", u) + "を" + SrcFormatter.Format(i) + "%減少させる。";
                        break;
                    }

                case "技":
                    {
                        switch (slevel)
                        {
                            case 1d:
                                {
                                    i = 0;
                                    break;
                                }

                            case 2d:
                                {
                                    i = 10;
                                    break;
                                }

                            case 3d:
                                {
                                    i = 20;
                                    break;
                                }

                            case 4d:
                                {
                                    i = 30;
                                    break;
                                }

                            case 5d:
                                {
                                    i = 40;
                                    break;
                                }

                            case 6d:
                                {
                                    i = 50;
                                    break;
                                }

                            case 7d:
                                {
                                    i = 55;
                                    break;
                                }

                            case 8d:
                                {
                                    i = 60;
                                    break;
                                }

                            case 9d:
                                {
                                    i = 65;
                                    break;
                                }

                            case var case1 when case1 >= 10d:
                                {
                                    i = 70;
                                    break;
                                }

                            default:
                                {
                                    i = 0;
                                    break;
                                }
                        }

                        msg = "技属性を持つ武装・" + Expression.Term("アビリティ", u) + "及び必要技能が" + sname0 + "の武装・" + Expression.Term("アビリティ", u) + "の消費" + Expression.Term("ＥＮ", u) + "を" + SrcFormatter.Format(i) + "%減少させる。";
                        break;
                    }

                case "集中力":
                    {
                        msg = Expression.Term("スペシャルパワー", u) + "の" + Expression.Term("ＳＰ", u) + "消費量が元の80%に減少する。";
                        break;
                    }

                case "闘争本能":
                    {
                        if (p.MinMorale > 100)
                        {
                            if (!p.IsSkillLevelSpecified("闘争本能"))
                            {
                                msg = "出撃時の" + Expression.Term("気力", u) + "が" + SrcFormatter.Format(p.MinMorale + 5d * slevel) + "に増加する。";
                            }
                            else if (slevel >= 0d)
                            {
                                msg = "出撃時の" + Expression.Term("気力", u) + "が" + SrcFormatter.Format(p.MinMorale + 5d * slevel) + "に増加する。";
                            }
                            else
                            {
                                msg = "出撃時の" + Expression.Term("気力", u) + "が" + SrcFormatter.Format(p.MinMorale + 5d * slevel) + "に減少する。";
                            }
                        }
                        else
                        {
                            if (!p.IsSkillLevelSpecified("闘争本能"))
                            {
                                msg = "出撃時の" + Expression.Term("気力", u) + "が105に増加する。";
                            }
                            else if (slevel >= 0d)
                            {
                                msg = "出撃時の" + Expression.Term("気力", u) + "が" + SrcFormatter.Format(100d + 5d * slevel) + "に増加する。";
                            }
                            else
                            {
                                msg = "出撃時の" + Expression.Term("気力", u) + "が" + SrcFormatter.Format(100d + 5d * slevel) + "に減少する。";
                            }
                        }

                        break;
                    }

                case "潜在力開放":
                    {
                        if (Expression.IsOptionDefined("ダメージ倍率低下"))
                        {
                            msg = Expression.Term("気力", u) + "130以上で発動し、ダメージを 20% 増加させる。";
                        }
                        else
                        {
                            msg = Expression.Term("気力", u) + "130以上で発動し、ダメージを 25% 増加させる。";
                        }

                        break;
                    }

                case "指揮":
                    {
                        msg = "半径" + Strings.StrConv(SrcFormatter.Format(p.CommandRange()), VbStrConv.Wide) + "マス以内にいる味方ザコ・汎用及び階級所有パイロットの" + Expression.Term("命中", u) + "・" + Expression.Term("回避", u);
                        if (slevel >= 0d)
                        {
                            msg = msg + "に +" + SrcFormatter.Format((int)(5d * slevel)) + "。";
                        }
                        else
                        {
                            msg = msg + "に " + SrcFormatter.Format((int)(5d * slevel)) + "。";
                        }

                        break;
                    }

                case "階級":
                    {
                        if (Strings.InStr(sname, "階級Lv") == 0)
                        {
                            msg = "階級レベル" + Strings.StrConv(SrcFormatter.Format((int)slevel), VbStrConv.Wide) + "に相当する。;";
                        }

                        msg = msg + "半径" + Strings.StrConv(SrcFormatter.Format(p.CommandRange()), VbStrConv.Wide) + "マス以内にいるザコ及び階級所有パイロットに指揮効果を与える。";
                        break;
                    }

                case "格闘サポート":
                    {
                        msg = "自分がサポートパイロットの時にメインパイロットの" + Expression.Term("格闘", u);
                        if (slevel >= 0d)
                        {
                            msg = msg + "に +" + SrcFormatter.Format((int)(2d * slevel)) + "。";
                        }
                        else
                        {
                            msg = msg + "に " + SrcFormatter.Format((int)(2d * slevel)) + "。";
                        }

                        break;
                    }

                case "射撃サポート":
                    {
                        msg = "自分がサポートパイロットの時にメインパイロットの" + Expression.Term("射撃", u);
                        if (slevel >= 0d)
                        {
                            msg = msg + "に +" + SrcFormatter.Format((int)(2d * slevel)) + "。";
                        }
                        else
                        {
                            msg = msg + "に " + SrcFormatter.Format((int)(2d * slevel)) + "。";
                        }

                        break;
                    }

                case "魔力サポート":
                    {
                        msg = "自分がサポートパイロットの時にメインパイロットの" + Expression.Term("魔力", u);
                        if (slevel >= 0d)
                        {
                            msg = msg + "に +" + SrcFormatter.Format((int)(2d * slevel)) + "。";
                        }
                        else
                        {
                            msg = msg + "に " + SrcFormatter.Format((int)(2d * slevel)) + "。";
                        }

                        break;
                    }

                case "命中サポート":
                    {
                        msg = "自分がサポートパイロットの時にメインパイロットの" + Expression.Term("命中", u);
                        if (slevel >= 0d)
                        {
                            msg = msg + "に +" + SrcFormatter.Format((int)(2d * slevel)) + "。";
                        }
                        else
                        {
                            msg = msg + "に " + SrcFormatter.Format((int)(2d * slevel)) + "。";
                        }

                        break;
                    }

                case "回避サポート":
                    {
                        msg = "自分がサポートパイロットの時にメインパイロットの" + Expression.Term("回避", u);
                        if (slevel >= 0d)
                        {
                            msg = msg + "に +" + SrcFormatter.Format((int)(2d * slevel)) + "。";
                        }
                        else
                        {
                            msg = msg + "に " + SrcFormatter.Format((int)(2d * slevel)) + "。";
                        }

                        break;
                    }

                case "技量サポート":
                    {
                        msg = "自分がサポートパイロットの時にメインパイロットの" + Expression.Term("技量", u);
                        if (slevel >= 0d)
                        {
                            msg = msg + "に +" + SrcFormatter.Format((int)(2d * slevel)) + "。";
                        }
                        else
                        {
                            msg = msg + "に " + SrcFormatter.Format((int)(2d * slevel)) + "。";
                        }

                        break;
                    }

                case "反応サポート":
                    {
                        msg = "自分がサポートパイロットの時にメインパイロットの" + Expression.Term("反応", u);
                        if (slevel >= 0d)
                        {
                            msg = msg + "に +" + SrcFormatter.Format((int)(2d * slevel)) + "。";
                        }
                        else
                        {
                            msg = msg + "に " + SrcFormatter.Format((int)(2d * slevel)) + "。";
                        }

                        break;
                    }

                case "サポート":
                    {
                        msg = "自分がサポートパイロットの時にメインパイロットの" + Expression.Term("命中", u) + "・" + Expression.Term("回避", u);
                        if (slevel >= 0d)
                        {
                            msg = msg + "に +" + SrcFormatter.Format((int)(3d * slevel)) + "。";
                        }
                        else
                        {
                            msg = msg + "に " + SrcFormatter.Format((int)(3d * slevel)) + "。";
                        }

                        break;
                    }

                case "広域サポート":
                    {
                        msg = "半径２マス以内にいる味方パイロットの" + Expression.Term("命中", u) + "・" + Expression.Term("回避", u);
                        if (slevel >= 0d)
                        {
                            msg = msg + "に +" + SrcFormatter.Format((int)(5d * slevel)) + "。";
                        }
                        else
                        {
                            msg = msg + "に " + SrcFormatter.Format((int)(5d * slevel)) + "。";
                        }

                        break;
                    }

                case "援護":
                    {
                        msg = "隣接するユニットにサポートアタックとサポートガードを" + "１ターンにそれぞれ" + SrcFormatter.Format((int)slevel) + "回行う。";
                        break;
                    }

                case "援護攻撃":
                    {
                        msg = "隣接するユニットにサポートアタックを１ターンに" + SrcFormatter.Format((int)slevel) + "回行う。";
                        break;
                    }

                case "援護防御":
                    {
                        msg = "隣接するユニットにサポートガードを１ターンに" + SrcFormatter.Format((int)slevel) + "回行う。";
                        break;
                    }

                case "統率":
                    {
                        msg = "自分から攻撃をかけた場合、" + "サポートアタックが同時援護攻撃に変更される。;" + "（１ターンに " + SrcFormatter.Format((int)slevel) + "回）";
                        break;
                    }

                case "チーム":
                    {
                        msg = sdata + "に所属する。" + "同じ" + sdata + "のユニットに対してのみ援護や指揮を行う。";
                        break;
                    }

                case "カウンター":
                    {
                        msg = "１ターンに " + SrcFormatter.Format((int)slevel) + "回" + "反撃がカウンター攻撃になり、相手の攻撃に先制して反撃を行う。";
                        break;
                    }

                case "先手必勝":
                    {
                        if (GeneralLib.LLength(sdata) == 2)
                        {
                            msg = "パイロットの" + Expression.Term("気力", u) + "が" + GeneralLib.LIndex(sdata, 2) + "以上で発動。";
                        }
                        else
                        {
                            msg = "パイロットの" + Expression.Term("気力", u) + "が120以上で発動。";
                        }

                        msg = msg + "反撃が必ずカウンター攻撃になり、相手の攻撃に先制して反撃を行う。";
                        break;
                    }

                case "先読み":
                    {
                        msg = SrcFormatter.Format((int)((long)(100d * slevel) / 16L)) + "%の確率で" + "反撃がカウンター攻撃になり、相手の攻撃に先制して反撃を行う。";
                        break;
                    }

                case "再攻撃":
                    {
                        msg = "自分の攻撃の直後に " + SrcFormatter.Format((int)((long)(100d * slevel) / 16L)) + "% の確率で再攻撃を行う。" + "ただしパイロットの" + Expression.Term("反応", u) + "が相手を下回る場合、確率は半減。";
                        break;
                    }

                case "２回行動":
                    {
                        msg = "１ターンに２回、行動が可能になる。";
                        break;
                    }

                case "耐久":
                    {
                        if (slevel >= 0d)
                        {
                            msg = "ダメージ計算の際に" + Expression.Term("装甲", u) + "を" + SrcFormatter.Format((int)(5d * slevel)) + "%増加させる。";
                        }
                        else
                        {
                            msg = "ダメージ計算の際に" + Expression.Term("装甲", u) + "を" + SrcFormatter.Format((int)(5d * Math.Abs(slevel))) + "%減少させる。";
                        }

                        break;
                    }

                case "ＳＰ低成長":
                    {
                        msg = "レベルアップ時の最大" + Expression.Term("ＳＰ", u) + "の増加量が通常の半分に減少する。";
                        break;
                    }

                case "ＳＰ高成長":
                    {
                        msg = "レベルアップ時の最大" + Expression.Term("ＳＰ", u) + "の増加量が通常の1.5倍に増加する。";
                        break;
                    }

                case "ＳＰ回復":
                    {
                        msg = "毎ターン" + Expression.Term("ＳＰ", u) + "がパイロットレベル/8+5回復する(+" + SrcFormatter.Format(p.Level / 8 + 5) + ")。";
                        break;
                    }

                case "格闘成長":
                    {
                        // 攻撃力低成長オプションが指定されているかどうかで解説を変更する。
                        msg = "レベルアップ時の" + Expression.Term("格闘", u) + "の増加量が";
                        if (Expression.IsOptionDefined("攻撃力低成長"))
                        {
                            msg = msg + SrcFormatter.Format(slevel + 0.5d) + "になる。";
                        }
                        else
                        {
                            msg = msg + SrcFormatter.Format(slevel + 1d) + "になる。";
                        }

                        break;
                    }

                case "射撃成長":
                    {
                        // 攻撃力低成長オプション、術技能の有無によってデフォルト解説を変更する。
                        if (p.HasMana())
                        {
                            msg = "レベルアップ時の" + Expression.Term("魔力", u) + "の増加量が";
                        }
                        else
                        {
                            msg = "レベルアップ時の" + Expression.Term("射撃", u) + "の増加量が";
                        }

                        if (Expression.IsOptionDefined("攻撃力低成長"))
                        {
                            msg = msg + SrcFormatter.Format(slevel + 0.5d) + "になる。";
                        }
                        else
                        {
                            msg = msg + SrcFormatter.Format(slevel + 1d) + "になる。";
                        }

                        break;
                    }

                case "命中成長":
                    {
                        msg = "レベルアップ時の" + Expression.Term("命中", u) + "の増加量が" + SrcFormatter.Format(slevel + 2d) + "になる。";
                        break;
                    }

                case "回避成長":
                    {
                        msg = "レベルアップ時の" + Expression.Term("回避", u) + "の増加量が" + SrcFormatter.Format(slevel + 2d) + "になる。";
                        break;
                    }

                case "技量成長":
                    {
                        msg = "レベルアップ時の" + Expression.Term("技量", u) + "の増加量が" + SrcFormatter.Format(slevel + 1d) + "になる。";
                        break;
                    }

                case "反応成長":
                    {
                        msg = "レベルアップ時の" + Expression.Term("反応", u) + "の増加量が" + SrcFormatter.Format(slevel + 1d) + "になる。";
                        break;
                    }

                case "防御成長":
                    {
                        // 防御力低成長オプションが指定されているかどうかで解説を変更する。
                        msg = "レベルアップ時の" + Expression.Term("防御", u) + "の増加量が";
                        if (Expression.IsOptionDefined("防御力低成長"))
                        {
                            msg = msg + SrcFormatter.Format(slevel + 0.5d) + "になる。";
                        }
                        else
                        {
                            msg = msg + SrcFormatter.Format(slevel + 1d) + "になる。";
                        }

                        break;
                    }

                case "精神統一":
                    {
                        msg = Expression.Term("ＳＰ", u) + "が最大" + Expression.Term("ＳＰ", u) + "の20%未満(" + SrcFormatter.Format(p.MaxSP / 5) + "未満)の場合、" + "ターン開始時に" + Expression.Term("ＳＰ", u) + "が最大" + Expression.Term("ＳＰ", u) + "の10%分回復する(+" + SrcFormatter.Format(p.MaxSP / 10) + ")。";
                        break;
                    }

                case "損傷時気力増加":
                    {
                        if (slevel >= -1)
                        {
                            msg = "ダメージを受けた際に" + Expression.Term("気力", u) + "+" + SrcFormatter.Format((int)(slevel + 1d)) + "。";
                        }
                        else
                        {
                            msg = "ダメージを受けた際に" + Expression.Term("気力", u) + SrcFormatter.Format((int)(slevel + 1d)) + "。";
                        }

                        break;
                    }

                case "命中時気力増加":
                    {
                        if (slevel >= 0d)
                        {
                            msg = "攻撃を命中させた際に" + Expression.Term("気力", u) + "+" + SrcFormatter.Format((int)slevel) + "。(マップ攻撃は例外)";
                        }
                        else
                        {
                            msg = "攻撃を命中させた際に" + Expression.Term("気力", u) + SrcFormatter.Format((int)slevel) + "。(マップ攻撃は例外)";
                        }

                        break;
                    }

                case "失敗時気力増加":
                    {
                        if (slevel >= 0d)
                        {
                            msg = "攻撃を外してしまった際に" + Expression.Term("気力", u) + "+" + SrcFormatter.Format((int)slevel) + "。(マップ攻撃は例外)";
                        }
                        else
                        {
                            msg = "攻撃を外してしまった際に" + Expression.Term("気力", u) + SrcFormatter.Format((int)slevel) + "。(マップ攻撃は例外)";
                        }

                        break;
                    }

                case "回避時気力増加":
                    {
                        if (slevel >= 0d)
                        {
                            msg = "攻撃を回避した際に" + Expression.Term("気力", u) + "+" + SrcFormatter.Format((int)slevel) + "。";
                        }
                        else
                        {
                            msg = "攻撃を回避した際に" + Expression.Term("気力", u) + SrcFormatter.Format((int)slevel) + "。";
                        }

                        break;
                    }

                case "起死回生":
                    {
                        msg = Expression.Term("ＳＰ", u) + "、" + Expression.Term("ＨＰ", u) + "、" + Expression.Term("ＥＮ", u) + "の全てが最大値の20%以下になると毎ターン最初に発動。" + Expression.Term("ＳＰ", u) + "、" + Expression.Term("ＨＰ", u) + "、" + Expression.Term("ＥＮ", u) + "が全快する。";
                        break;
                    }

                case "戦術":
                    {
                        msg = "思考パターン決定の際に用いられる" + Expression.Term("技量", u);
                        if (slevel >= 0d)
                        {
                            msg = msg + "初期値がレベル×10増加(+" + SrcFormatter.Format((int)(10d * slevel)) + ")。";
                        }
                        else
                        {
                            msg = msg + "初期値がレベル×10減少(" + SrcFormatter.Format((int)(10d * slevel)) + ")。";
                        }

                        break;
                    }

                case "得意技":
                    {
                        msg = "「" + p.SkillData(stype) + "」属性を持つ武器・" + Expression.Term("アビリティ", u) + "によるダメージ・効果量が 20% 増加。" + "また、" + Expression.Term("アビリティ", u) + "の継続時間が 40% 増加。";
                        break;
                    }

                case "不得手":
                    {
                        msg = "「" + p.SkillData(stype) + "」属性を持つ武器・" + Expression.Term("アビリティ", u) + "によるダメージ・効果量が 20% 減少。" + "また、" + Expression.Term("アビリティ", u) + "の継続時間が 40% 減少。";
                        break;
                    }

                case "ハンター":
                    {
                        msg = "ターゲットが";
                        var loopTo = GeneralLib.LLength(sdata);
                        for (i = 2; i <= loopTo; i++)
                        {
                            if (i == 3)
                            {
                                msg = msg + "や";
                            }
                            else if (3 > 2)
                            {
                                msg = msg + "、";
                            }

                            msg = msg + GeneralLib.LIndex(sdata, i);
                        }

                        msg = msg + "である場合、ターゲットに与えるダメージが";
                        if (slevel >= 0d)
                        {
                            msg = msg + SrcFormatter.Format(10d * slevel) + "%増加する。";
                        }
                        else
                        {
                            msg = msg + SrcFormatter.Format(-10 * slevel) + "%減少する。";
                        }

                        break;
                    }

                case "ＳＰ消費減少":
                    {
                        msg = Expression.Term("スペシャルパワー", u);
                        var loopTo1 = GeneralLib.LLength(sdata);
                        for (i = 2; i <= loopTo1; i++)
                            msg = msg + "「" + GeneralLib.LIndex(sdata, i) + "」";
                        msg = msg + "の" + Expression.Term("ＳＰ", u) + "消費量が";
                        if (slevel >= 0d)
                        {
                            msg = msg + SrcFormatter.Format(10d * slevel) + "%減少する。";
                        }
                        else
                        {
                            msg = msg + SrcFormatter.Format(-10 * slevel) + "%増加する。";
                        }

                        break;
                    }

                case "スペシャルパワー自動発動":
                    {
                        msg = Expression.Term("気力", u) + "が" + GeneralLib.LIndex(sdata, 3) + "以上で発動し、" + "毎ターン最初に「" + GeneralLib.LIndex(sdata, 2) + "」が自動でかかる。" + "（" + Expression.Term("ＳＰ", u) + "は消費しない）";
                        break;
                    }

                case "修理":
                    {
                        msg = "修理装置や回復" + Expression.Term("アビリティ", u) + "を使った際の" + Expression.Term("ＨＰ", u) + "回復量が ";
                        if (slevel >= 0d)
                        {
                            msg = msg + SrcFormatter.Format(10d * slevel) + "% 増加する。";
                        }
                        else
                        {
                            msg = msg + SrcFormatter.Format(-10 * slevel) + "% 減少する。";
                        }

                        break;
                    }

                case "補給":
                    {
                        if (Expression.IsOptionDefined("移動後補給不可"))
                        {
                            msg = "移動後に補給装置を使用できるようになる。また、";
                        }

                        msg = msg + "補給" + Expression.Term("アビリティ", u) + "を使った際の" + Expression.Term("ＥＮ", u) + "回復量が ";
                        if (slevel >= 0d)
                        {
                            msg = msg + SrcFormatter.Format(10d * slevel) + "% 増加する。";
                        }
                        else
                        {
                            msg = msg + SrcFormatter.Format(-10 * slevel) + "% 減少する。";
                        }

                        break;
                    }

                case "気力上限":
                    {
                        i = 150;
                        if (slevel != 0d)
                        {
                            i = (int)GeneralLib.MaxLng((int)slevel, 0);
                        }

                        msg = Expression.Term("気力", u) + "の上限が" + SrcFormatter.Format(i) + "になる。";
                        break;
                    }

                case "気力下限":
                    {
                        i = 50;
                        if (slevel != 0d)
                        {
                            i = (int)GeneralLib.MaxLng((int)slevel, 0);
                        }

                        msg = Expression.Term("気力", u) + "の下限が" + SrcFormatter.Format(i) + "になる。";
                        break;
                    }

                // ADD START MARGE
                case "遊撃":
                    {
                        // ADD END MARGE

                        msg = "移動後使用可能な武器・" + Expression.Term("アビリティ", u) + "を使った後に、残った移動力を使って移動できる。";
                        break;
                    }

                default:
                    {
                        // ダミー能力

                        // パイロット側で解説を定義している？
                        sdata = p.SkillData(sname0);
                        if (GeneralLib.ListIndex(sdata, 1) == "解説")
                        {
                            msg = GeneralLib.ListIndex(sdata, GeneralLib.ListLength(sdata));
                            if (Strings.Left(msg, 1) == "\"")
                            {
                                msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
                            }

                            SkillHelpMessageRet = msg;
                            return SkillHelpMessageRet;
                        }

                        // ユニット側で解説を定義している？
                        var loopTo2 = u.CountFeature();
                        for (i = 1; i <= loopTo2; i++)
                        {
                            if ((u.Feature(i).Name ?? "") == (stype ?? ""))
                            {
                                fdata = u.FeatureData(i);
                                if (GeneralLib.ListIndex(fdata, 1) == "解説")
                                {
                                    msg = GeneralLib.ListIndex(fdata, GeneralLib.ListLength(fdata));
                                }
                            }
                        }

                        if (u2 is object)
                        {
                            var loopTo3 = u2.CountFeature();
                            for (i = 1; i <= loopTo3; i++)
                            {
                                if ((u2.Feature(i).Name ?? "") == (stype ?? ""))
                                {
                                    fdata = u2.FeatureData(i);
                                    if (GeneralLib.ListIndex(fdata, 1) == "解説")
                                    {
                                        msg = GeneralLib.ListIndex(fdata, GeneralLib.ListLength(fdata));
                                    }
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(msg))
                        {
                            return SkillHelpMessageRet;
                        }

                        // ユニット側で解説を定義している場合
                        if (Strings.Left(msg, 1) == "\"")
                        {
                            msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
                        }

                        break;
                    }
            }

            // パイロット側で解説を定義している？
            sdata = p.SkillData(sname0);
            if (GeneralLib.ListIndex(sdata, 1) == "解説")
            {
                msg = GeneralLib.ListIndex(sdata, GeneralLib.ListLength(sdata));
                if (Strings.Left(msg, 1) == "\"")
                {
                    msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
                }
            }

            // ユニット側で解説を定義している？
            var loopTo4 = u.CountFeature();
            for (i = 1; i <= loopTo4; i++)
            {
                if ((u.Feature(i).Name ?? "") == (sname0 ?? ""))
                {
                    fdata = u.FeatureData(i);
                    if (GeneralLib.ListIndex(fdata, 1) == "解説")
                    {
                        msg = GeneralLib.ListIndex(fdata, GeneralLib.ListLength(fdata));
                        if (Strings.Left(msg, 1) == "\"")
                        {
                            msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
                        }
                    }
                }
            }

            if (u2 is object)
            {
                var loopTo5 = u2.CountFeature();
                for (i = 1; i <= loopTo5; i++)
                {
                    if ((u2.Feature(i).Name ?? "") == (sname0 ?? ""))
                    {
                        fdata = u2.FeatureData(i);
                        if (GeneralLib.ListIndex(fdata, 1) == "解説")
                        {
                            msg = GeneralLib.ListIndex(fdata, GeneralLib.ListLength(fdata));
                            if (Strings.Left(msg, 1) == "\"")
                            {
                                msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
                            }
                        }
                    }
                }
            }

            // 等身大基準の際は「パイロット」という語を使わないようにする
            if (Expression.IsOptionDefined("等身大基準"))
            {
                msg = msg.Replace("メインパイロット", "ユニット");
                msg = msg.Replace("サポートパイロット", "サポート");
                msg = msg.Replace("パイロットレベル", "レベル");
                msg = msg.Replace("パイロット", "ユニット");
            }

            SkillHelpMessageRet = msg;
            return SkillHelpMessageRet;
        }


        // ユニット u の findex 番目の特殊能力の解説を表示
        public void FeatureHelp(Unit u, int findex, bool is_additional)
        {
            // 特殊能力の名称を調べる
            var fname = u.AllFeature(findex).FeatureName(u);

            FeatureHelpInternal(u, fname, is_additional);
        }

        public void FeatureHelp(Unit u, string findex, bool is_additional)
        {
            // 特殊能力の名称を調べる
            string fname;
            if (findex == "武器・防具クラス")
            {
                // XXX 意味わからん混ざり方している
                fname = findex;
            }
            else
            {
                fname = u.AllFeature(findex).FeatureName(u);
            }
            FeatureHelpInternal(u, fname, is_additional);
        }

        public void FeatureHelpInternal(Unit u, string fname, bool is_additional)
        {
            string msg;
            bool prev_mode;

            msg = FeatureHelpMessage(u, fname, is_additional);

            // 解説の表示
            if (Strings.Len(msg) > 0)
            {
                prev_mode = GUI.AutoMessageMode;
                GUI.AutoMessageMode = false;
                GUI.OpenMessageForm(u1: null, u2: null);
                if (SRC.AutoMoveCursor)
                {
                    GUI.MoveCursorPos("メッセージウィンドウ");
                }

                GUI.DisplayMessage("システム", "<b>" + fname + "</b>;" + msg);
                GUI.CloseMessageForm();
                GUI.AutoMessageMode = prev_mode;
            }
        }

        // ユニット u の findex 番目の特殊能力の解説
        public string FeatureHelpMessage(Unit u, int findex, bool is_additional)
        {
            return FeatureHelpMessage(u, u.AllFeature(findex).Name, is_additional);
        }

        public string FeatureHelpMessage(Unit u, string findex, bool is_additional)
        {
            string FeatureHelpMessageRet = default;
            var fid = default(int);
            string fname, ftype, fname0;
            string fdata = default, opt;
            double flevel = default, lv_mod;
            var flevel_specified = default(bool);
            var msg = default(string);
            int i, idx;
            var buf = default(string);
            var prob = default(int);
            Pilot p;
            string sname;
            double slevel;
            string uname;
            {
                // メインパイロット
                p = u.MainPilot();

                // 特殊能力の名称、レベル、データを調べる
                var fd = u.AllFeature(findex);
                if (findex == "武器・防具クラス")
                {
                    // XXX 意味わからん混ざり方している
                    ftype = Conversions.ToString(findex);
                    fname = Conversions.ToString(findex);
                }
                else
                {
                    ftype = fd.Name;
                    fname = fd.FeatureName(u);
                    fdata = fd.Data;
                    flevel = fd.Level;
                    flevel_specified = fd.HasLevel;
                    fid = u.AllFeatures.IndexOf(fd) + 1;
                }

                if (Strings.InStr(fname, "Lv") > 0)
                {
                    fname0 = Strings.Left(fname, Strings.InStr(fname, "Lv") - 1);
                }
                else
                {
                    fname0 = fname;
                }

                // 重複可能な特殊能力の場合、レベルのみが異なる能力のレベルは累積する
                switch (ftype ?? "")
                {
                    case "フィールド":
                    case "アーマー":
                    case "レジスト":
                    case "攻撃回避":
                        {
                            flevel = u.AllFeatures.Where(x => x.Name == fd.Name).Where(x => x.Data == fd.Data).Sum(x => x.FeatureLevel);
                            break;
                        }
                }
            }

            switch (ftype ?? "")
            {
                case "シールド":
                    {
                        sname = p.SkillName0("Ｓ防御");
                        prob = (int)((long)(p.SkillLevel("Ｓ防御", ref_mode: "") * 100d) / 16L);
                        msg = sname + "Lv/16の確率(" + SrcFormatter.Format(prob) + "%)で防御を行い、" + "ダメージを半減。";
                        break;
                    }

                case "大型シールド":
                    {
                        sname = p.SkillName0("Ｓ防御");
                        if (p.IsSkillAvailable("Ｓ防御"))
                        {
                            prob = (int)((long)((p.SkillLevel("Ｓ防御", ref_mode: "") + 1d) * 100d) / 16L);
                        }

                        msg = "(" + sname + "Lv+1)/16の確率(" + SrcFormatter.Format(prob) + "%)で防御を行い、" + "ダメージを半減。";
                        break;
                    }

                case "小型シールド":
                    {
                        sname = p.SkillName0("Ｓ防御");
                        prob = (int)((long)(p.SkillLevel("Ｓ防御", ref_mode: "") * 100d) / 16L);
                        msg = sname + "Lv/16の確率(" + SrcFormatter.Format(prob) + "%)で防御を行い、" + "ダメージを2/3に減少。";
                        break;
                    }

                case "エネルギーシールド":
                    {
                        sname = p.SkillName0("Ｓ防御");
                        prob = (int)((long)(p.SkillLevel("Ｓ防御", ref_mode: "") * 100d) / 16L);
                        if (flevel > 0d)
                        {
                            msg = sname + "Lv/16の確率(" + SrcFormatter.Format(prob) + "%)で防御を行い、" + "ダメージを半減した上で更に" + SrcFormatter.Format(100d * flevel) + "減少。";
                        }
                        else
                        {
                            msg = sname + "Lv/16の確率(" + SrcFormatter.Format(prob) + "%)で防御を行い、" + "ダメージを半減。";
                        }

                        msg = msg + "発動時に5ＥＮ消費。「無」属性を持つ武器には無効。";
                        break;
                    }

                case "アクティブシールド":
                    {
                        sname = p.SkillName0("Ｓ防御");
                        prob = (int)((long)(p.SkillLevel("Ｓ防御", ref_mode: "") * 100d) / 16L);
                        if (p.IsSkillAvailable("Ｓ防御"))
                        {
                            prob = (int)((long)((p.SkillLevel("Ｓ防御", ref_mode: "") + 2d) * 100d) / 16L);
                        }

                        msg = "(" + sname + "Lv+2)/16の確率(" + SrcFormatter.Format(prob) + "%)で防御を行い、" + "ダメージを半減。";
                        break;
                    }

                case "盾":
                    {
                        sname = p.SkillName0("Ｓ防御");
                        slevel = p.SkillLevel("Ｓ防御", ref_mode: "");
                        if (slevel > 0d)
                        {
                            slevel = 100d * slevel + 400d;
                        }

                        msg = SrcFormatter.Format(flevel) + "回、攻撃によって貫通されるまでシールド防御を行い、" + "ダメージを減少させる(-" + SrcFormatter.Format((int)slevel) + ")。;" + "ただし攻撃側が「破」属性を持っていた場合、一度に２回分破壊される。;" + "ダメージの減少量はパイロットの" + sname + "レベルによって決まる。";
                        break;
                    }

                case "バリア":
                    {
                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(fdata, 2)) && GeneralLib.LIndex(fdata, 2) != "全")
                        {
                            if (Strings.Left(GeneralLib.LIndex(fdata, 2), 1) == "!")
                            {
                                msg = "「" + Strings.Mid(GeneralLib.LIndex(fdata, 2), 2) + "」属性を持たない";
                            }
                            else
                            {
                                msg = "「" + GeneralLib.LIndex(fdata, 2) + "」属性を持つ";
                            }
                        }

                        msg = msg + "ダメージ" + SrcFormatter.Format((int)(1000d * flevel)) + "以下の攻撃を無効化。";
                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
                        {
                            int localStrToLng() { string argexpr = GeneralLib.LIndex(fdata, 3); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                            if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 3)) > 0)
                            {
                                msg = msg + ";発動時に" + GeneralLib.LIndex(fdata, 3) + Expression.Term("ＥＮ", u) + "消費。";
                            }
                            else if (localStrToLng() < 0)
                            {
                                msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(fdata, 3), 2) + Expression.Term("ＥＮ", u) + "増加。";
                            }
                        }
                        else
                        {
                            msg = msg + ";発動時に10ＥＮ消費。";
                        }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 4)) > 50)
                        {
                            msg = msg + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 4) + "以上で使用可能。";
                        }

                        var loopTo2 = GeneralLib.LLength(fdata);
                        for (i = 5; i <= loopTo2; i++)
                        {
                            opt = GeneralLib.LIndex(fdata, i);
                            idx = (int)Strings.InStr(opt, "*");
                            if (idx > 0)
                            {
                                lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
                                opt = Strings.Left(opt, idx - 1);
                            }
                            else
                            {
                                lv_mod = -1;
                            }

                            switch (p.SkillType(opt) ?? "")
                            {
                                case "相殺":
                                    {
                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、隣接時に効果は相殺。";
                                        break;
                                    }

                                case "中和":
                                    {
                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、" + "隣接時にレベル分だけ効果を中和。";
                                        break;
                                    }

                                case "近接無効":
                                    {
                                        msg = msg + ";「武」「突」「接」による攻撃には無効。";
                                        break;
                                    }

                                case "バリア無効化無効":
                                    {
                                        msg = msg + ";バリア無効化によって無効化されない。";
                                        break;
                                    }

                                case "手動":
                                    {
                                        msg = msg + ";防御選択時にのみ発動。";
                                        break;
                                    }

                                case "能力必要":
                                    {
                                        break;
                                    }
                                // スキップ
                                case "同調率":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 20d;
                                        }

                                        if (u.SyncLevel() >= 30d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
                                        }
                                        else if (u.SyncLevel() > 0d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
                                        }

                                        break;
                                    }

                                case "霊力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 10d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PlanaLevel()) + ")。";
                                        break;
                                    }

                                case "オーラ":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.AuraLevel()) + ")。";
                                        break;
                                    }

                                case "超能力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PsychicLevel()) + ")。";
                                        break;
                                    }

                                default:
                                    {
                                        sname = u.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + SrcFormatter.Format(lv_mod * u.SkillLevel(opt)) + ")。";
                                        break;
                                    }
                            }
                        }

                        break;
                    }

                case "バリアシールド":
                    {
                        sname = p.SkillName0("Ｓ防御");
                        prob = (int)((long)(p.SkillLevel("Ｓ防御", ref_mode: "") * 100d) / 16L);
                        msg = sname + "Lv/16の確率(" + SrcFormatter.Format(prob) + "%)で発動し、";
                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(fdata, 2)) && GeneralLib.LIndex(fdata, 2) != "全")
                        {
                            if (Strings.Left(GeneralLib.LIndex(fdata, 2), 1) == "!")
                            {
                                msg = msg + "「" + Strings.Mid(GeneralLib.LIndex(fdata, 2), 2) + "」属性を持たない";
                            }
                            else
                            {
                                msg = msg + "「" + GeneralLib.LIndex(fdata, 2) + "」属性を持つ";
                            }
                        }

                        msg = msg + "ダメージ" + SrcFormatter.Format((int)(1000d * flevel)) + "以下の攻撃を無効化。";
                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
                        {
                            int localStrToLng1() { string argexpr = GeneralLib.LIndex(fdata, 3); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                            if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 3)) > 0)
                            {
                                msg = msg + "発動時に" + GeneralLib.LIndex(fdata, 3) + Expression.Term("ＥＮ", u) + "消費。";
                            }
                            else if (localStrToLng1() < 0)
                            {
                                msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(fdata, 3), 2) + Expression.Term("ＥＮ", u) + "増加。";
                            }
                        }
                        else
                        {
                            msg = msg + "発動時に10" + Expression.Term("ＥＮ", u) + "消費。";
                        }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 4)) > 50)
                        {
                            msg = msg + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 4) + "以上で使用可能。";
                        }

                        var loopTo3 = GeneralLib.LLength(fdata);
                        for (i = 5; i <= loopTo3; i++)
                        {
                            opt = GeneralLib.LIndex(fdata, i);
                            idx = (int)Strings.InStr(opt, "*");
                            if (idx > 0)
                            {
                                lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
                                opt = Strings.Left(opt, idx - 1);
                            }
                            else
                            {
                                lv_mod = -1;
                            }

                            switch (p.SkillType(opt) ?? "")
                            {
                                case "相殺":
                                    {
                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、隣接時に効果は相殺。";
                                        break;
                                    }

                                case "中和":
                                    {
                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、" + "隣接時にレベル分だけ効果を中和。";
                                        break;
                                    }

                                case "近接無効":
                                    {
                                        msg = msg + ";「武」「突」「接」による攻撃には無効。";
                                        break;
                                    }

                                case "バリア無効化無効":
                                    {
                                        msg = msg + ";バリア無効化によって無効化されない。";
                                        break;
                                    }

                                case "手動":
                                    {
                                        msg = msg + ";防御選択時にのみ発動。";
                                        break;
                                    }

                                case "能力必要":
                                    {
                                        break;
                                    }
                                // スキップ
                                case "同調率":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 20d;
                                        }

                                        if (u.SyncLevel() >= 30d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
                                        }
                                        else if (u.SyncLevel() > 0d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
                                        }

                                        break;
                                    }

                                case "霊力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 10d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PlanaLevel()) + ")。";
                                        break;
                                    }

                                case "オーラ":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.AuraLevel()) + ")。";
                                        break;
                                    }

                                case "超能力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PsychicLevel()) + ")。";
                                        break;
                                    }

                                default:
                                    {
                                        sname = u.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + SrcFormatter.Format(lv_mod * u.SkillLevel(opt)) + ")。";
                                        break;
                                    }
                            }
                        }

                        break;
                    }

                case "広域バリア":
                    {
                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)) && GeneralLib.LIndex(fdata, 2) != "1")
                        {
                            msg = "半径" + Strings.StrConv(GeneralLib.LIndex(fdata, 2), VbStrConv.Wide) + "マス以内の味方ユニットに対する";
                            i = Conversions.ToInteger(GeneralLib.LIndex(fdata, 2));
                        }
                        else
                        {
                            msg = "隣接する味方ユニットに対する";
                            i = 1;
                        }

                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(fdata, 3)) && GeneralLib.LIndex(fdata, 3) != "全")
                        {
                            if (Strings.Left(GeneralLib.LIndex(fdata, 3), 1) == "!")
                            {
                                msg = msg + "「" + Strings.Mid(GeneralLib.LIndex(fdata, 3), 2) + "」属性を持たない";
                            }
                            else
                            {
                                msg = msg + "「" + GeneralLib.LIndex(fdata, 3) + "」属性を持つ";
                            }
                        }

                        msg = msg + "ダメージ" + SrcFormatter.Format((int)(1000d * flevel)) + "以下の攻撃を無効化。";
                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
                        {
                            int localStrToLng2() { string argexpr = GeneralLib.LIndex(fdata, 4); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                            if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 4)) > 0)
                            {
                                msg = msg + ";発動時に" + GeneralLib.LIndex(fdata, 4) + Expression.Term("ＥＮ", u) + "消費。";
                            }
                            else if (localStrToLng2() < 0)
                            {
                                msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(fdata, 4), 2) + Expression.Term("ＥＮ", u) + "増加。";
                            }
                        }
                        else
                        {
                            msg = msg + ";発動時に" + SrcFormatter.Format(20 * i) + Expression.Term("ＥＮ", u) + "消費。";
                        }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 5)) > 50)
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 5) + "以上で使用可能。";
                        }

                        msg = msg + ";ただし攻撃側も有効範囲内にいる場合は無効化。";
                        break;
                    }

                case "フィールド":
                    {
                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(fdata, 2)) && GeneralLib.LIndex(fdata, 2) != "全")
                        {
                            if (Strings.Left(GeneralLib.LIndex(fdata, 2), 1) == "!")
                            {
                                msg = "「" + Strings.Mid(GeneralLib.LIndex(fdata, 2), 2) + "」属性を持たない";
                            }
                            else
                            {
                                msg = "「" + GeneralLib.LIndex(fdata, 2) + "」属性を持つ";
                            }
                        }

                        if (flevel >= 0d)
                        {
                            msg = msg + "攻撃のダメージを" + SrcFormatter.Format((int)(500d * flevel)) + "減少させる。";
                        }
                        else
                        {
                            msg = msg + "攻撃のダメージを" + SrcFormatter.Format((int)(-500 * flevel)) + "増加させる。";
                        }

                        int localStrToLng3() { string argexpr = GeneralLib.LIndex(fdata, 3); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 3)) > 0)
                        {
                            msg = msg + ";発動時に" + GeneralLib.LIndex(fdata, 3) + Expression.Term("ＥＮ", u) + "消費。";
                        }
                        else if (localStrToLng3() < 0)
                        {
                            msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(fdata, 3), 2) + Expression.Term("ＥＮ", u) + "増加。";
                        }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 4)) > 50)
                        {
                            msg = msg + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 4) + "以上で使用可能。";
                        }

                        var loopTo4 = GeneralLib.LLength(fdata);
                        for (i = 5; i <= loopTo4; i++)
                        {
                            opt = GeneralLib.LIndex(fdata, i);
                            idx = (int)Strings.InStr(opt, "*");
                            if (idx > 0)
                            {
                                lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
                                opt = Strings.Left(opt, idx - 1);
                            }
                            else
                            {
                                lv_mod = -1;
                            }

                            switch (p.SkillType(opt) ?? "")
                            {
                                case "相殺":
                                    {
                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、隣接時に効果は相殺。";
                                        break;
                                    }

                                case "中和":
                                    {
                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、" + "隣接時にレベル分だけ効果を中和。";
                                        break;
                                    }

                                case "近接無効":
                                    {
                                        msg = msg + ";「武」「突」「接」による攻撃には無効。";
                                        break;
                                    }

                                case "バリア無効化無効":
                                    {
                                        msg = msg + ";バリア無効化によって無効化されない。";
                                        break;
                                    }

                                case "手動":
                                    {
                                        msg = msg + ";防御選択時にのみ発動。";
                                        break;
                                    }

                                case "能力必要":
                                    {
                                        break;
                                    }
                                // スキップ
                                case "同調率":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 20d;
                                        }

                                        if (u.SyncLevel() >= 30d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
                                        }
                                        else if (u.SyncLevel() > 0d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
                                        }

                                        break;
                                    }

                                case "霊力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 10d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PlanaLevel()) + ")。";
                                        break;
                                    }

                                case "オーラ":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.AuraLevel()) + ")。";
                                        break;
                                    }

                                case "超能力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PsychicLevel()) + ")。";
                                        break;
                                    }

                                default:
                                    {
                                        sname = u.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + SrcFormatter.Format(lv_mod * u.SkillLevel(opt)) + ")。";
                                        break;
                                    }
                            }
                        }

                        break;
                    }

                case "アクティブフィールド":
                    {
                        sname = p.SkillName0("Ｓ防御");
                        prob = (int)((long)(p.SkillLevel("Ｓ防御", ref_mode: "") * 100d) / 16L);
                        msg = sname + "Lv/16の確率(" + SrcFormatter.Format(prob) + "%)で発動し、";
                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(fdata, 2)) && GeneralLib.LIndex(fdata, 2) != "全")
                        {
                            if (Strings.Left(GeneralLib.LIndex(fdata, 2), 1) == "!")
                            {
                                msg = msg + "「" + Strings.Mid(GeneralLib.LIndex(fdata, 2), 2) + "」属性を持たない";
                            }
                            else
                            {
                                msg = msg + "「" + GeneralLib.LIndex(fdata, 2) + "」属性を持つ";
                            }
                        }

                        if (flevel >= 0d)
                        {
                            msg = msg + "攻撃のダメージを" + SrcFormatter.Format((int)(500d * flevel)) + "減少させる。";
                        }
                        else
                        {
                            msg = msg + "攻撃のダメージを" + SrcFormatter.Format((int)(-500 * flevel)) + "増加させる。";
                        }

                        int localStrToLng4() { string argexpr = GeneralLib.LIndex(fdata, 3); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 3)) > 0)
                        {
                            msg = msg + ";発動時に" + GeneralLib.LIndex(fdata, 3) + Expression.Term("ＥＮ", u) + "消費。";
                        }
                        else if (localStrToLng4() < 0)
                        {
                            msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(fdata, 3), 2) + Expression.Term("ＥＮ", u) + "増加。";
                        }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 4)) > 50)
                        {
                            msg = msg + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 4) + "以上で使用可能。";
                        }

                        var loopTo5 = GeneralLib.LLength(fdata);
                        for (i = 5; i <= loopTo5; i++)
                        {
                            opt = GeneralLib.LIndex(fdata, i);
                            idx = (int)Strings.InStr(opt, "*");
                            if (idx > 0)
                            {
                                lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
                                opt = Strings.Left(opt, idx - 1);
                            }
                            else
                            {
                                lv_mod = -1;
                            }

                            switch (p.SkillType(opt) ?? "")
                            {
                                case "相殺":
                                    {
                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、隣接時に効果は相殺。";
                                        break;
                                    }

                                case "中和":
                                    {
                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、" + "隣接時にレベル分だけ効果を中和。";
                                        break;
                                    }

                                case "近接無効":
                                    {
                                        msg = msg + ";「武」「突」「接」による攻撃には無効。";
                                        break;
                                    }

                                case "バリア無効化無効":
                                    {
                                        msg = msg + ";バリア無効化によって無効化されない。";
                                        break;
                                    }

                                case "手動":
                                    {
                                        msg = msg + ";防御選択時にのみ発動。";
                                        break;
                                    }

                                case "能力必要":
                                    {
                                        break;
                                    }
                                // スキップ
                                case "同調率":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 20d;
                                        }

                                        if (u.SyncLevel() >= 30d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
                                        }
                                        else if (u.SyncLevel() > 0d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
                                        }

                                        break;
                                    }

                                case "霊力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 10d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PlanaLevel()) + ")。";
                                        break;
                                    }

                                case "オーラ":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.AuraLevel()) + ")。";
                                        break;
                                    }

                                case "超能力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PsychicLevel()) + ")。";
                                        break;
                                    }

                                default:
                                    {
                                        sname = u.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + SrcFormatter.Format(lv_mod * u.SkillLevel(opt)) + ")。";
                                        break;
                                    }
                            }
                        }

                        break;
                    }

                case "広域フィールド":
                    {
                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)) && GeneralLib.LIndex(fdata, 2) != "1")
                        {
                            msg = "半径" + Strings.StrConv(GeneralLib.LIndex(fdata, 2), VbStrConv.Wide) + "マス以内の味方ユニットに対する";
                            i = Conversions.ToInteger(GeneralLib.LIndex(fdata, 2));
                        }
                        else
                        {
                            msg = "隣接する味方ユニットに対する";
                            i = 1;
                        }

                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(fdata, 3)) && GeneralLib.LIndex(fdata, 3) != "全")
                        {
                            if (Strings.Left(GeneralLib.LIndex(fdata, 3), 1) == "!")
                            {
                                msg = msg + "「" + Strings.Mid(GeneralLib.LIndex(fdata, 3), 2) + "」属性を持たない";
                            }
                            else
                            {
                                msg = msg + "「" + GeneralLib.LIndex(fdata, 3) + "」属性を持つ";
                            }
                        }

                        if (flevel >= 0d)
                        {
                            msg = msg + "攻撃のダメージを" + SrcFormatter.Format((int)(500d * flevel)) + "減少させる。";
                        }
                        else
                        {
                            msg = msg + "攻撃のダメージを" + SrcFormatter.Format((int)(-500 * flevel)) + "増加させる。";
                        }

                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
                        {
                            int localStrToLng5() { string argexpr = GeneralLib.LIndex(fdata, 4); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                            if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 4)) > 0)
                            {
                                msg = msg + ";発動時に" + GeneralLib.LIndex(fdata, 4) + Expression.Term("ＥＮ", u) + "消費。";
                            }
                            else if (localStrToLng5() < 0)
                            {
                                msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(fdata, 4), 2) + Expression.Term("ＥＮ", u) + "増加。";
                            }
                        }
                        else
                        {
                            msg = msg + ";発動時に" + SrcFormatter.Format(20 * i) + Expression.Term("ＥＮ", u) + "消費。";
                        }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 5)) > 50)
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 5) + "以上で使用可能。";
                        }

                        msg = msg + ";ただし攻撃側も有効範囲内にいる場合は無効化。";
                        break;
                    }

                case "プロテクション":
                    {
                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(fdata, 2)) && GeneralLib.LIndex(fdata, 2) != "全")
                        {
                            if (Strings.Left(GeneralLib.LIndex(fdata, 2), 1) == "!")
                            {
                                msg = "「" + Strings.Mid(GeneralLib.LIndex(fdata, 2), 2) + "」属性を持たない";
                            }
                            else
                            {
                                msg = "「" + GeneralLib.LIndex(fdata, 2) + "」属性を持つ";
                            }
                        }

                        if (flevel > 10d)
                        {
                            msg = msg + "攻撃のダメージを" + SrcFormatter.Format((int)(10d * flevel - 100d)) + "%吸収する。";
                        }
                        else if (flevel >= 0d)
                        {
                            msg = msg + "攻撃のダメージを" + SrcFormatter.Format((int)(10d * flevel)) + "%減少させる。";
                        }
                        else
                        {
                            msg = msg + "攻撃のダメージを" + SrcFormatter.Format((int)(-10 * flevel)) + "%増加させる。";
                        }

                        int localStrToLng6() { string argexpr = GeneralLib.LIndex(fdata, 3); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                        int localStrToLng7() { string argexpr = GeneralLib.LIndex(fdata, 3); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                        if (!Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
                        {
                            msg = msg + ";発動時に10" + Expression.Term("ＥＮ", u) + "増加。";
                        }
                        else if (localStrToLng6() > 0)
                        {
                            msg = msg + ";発動時に" + GeneralLib.LIndex(fdata, 3) + Expression.Term("ＥＮ", u) + "消費。";
                        }
                        else if (localStrToLng7() < 0)
                        {
                            msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(fdata, 3), 2) + Expression.Term("ＥＮ", u) + "増加。";
                        }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 4)) > 50)
                        {
                            msg = msg + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 4) + "以上で使用可能。";
                        }

                        var loopTo6 = GeneralLib.LLength(fdata);
                        for (i = 5; i <= loopTo6; i++)
                        {
                            opt = GeneralLib.LIndex(fdata, i);
                            idx = (int)Strings.InStr(opt, "*");
                            if (idx > 0)
                            {
                                lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
                                opt = Strings.Left(opt, idx - 1);
                            }
                            else
                            {
                                lv_mod = -1;
                            }

                            switch (p.SkillType(opt) ?? "")
                            {
                                case "相殺":
                                    {
                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、隣接時に効果は相殺。";
                                        break;
                                    }

                                case "中和":
                                    {
                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、" + "隣接時にレベル分だけ効果を中和。";
                                        break;
                                    }

                                case "近接無効":
                                    {
                                        msg = msg + ";「武」「突」「接」による攻撃には無効。";
                                        break;
                                    }

                                case "バリア無効化無効":
                                    {
                                        msg = msg + ";バリア無効化によって無効化されない。";
                                        break;
                                    }

                                case "手動":
                                    {
                                        msg = msg + ";防御選択時にのみ発動。";
                                        break;
                                    }

                                case "能力必要":
                                    {
                                        break;
                                    }
                                // スキップ
                                case "同調率":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 0.5d;
                                        }

                                        if (u.SyncLevel() >= 30d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
                                        }
                                        else if (u.SyncLevel() > 0d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
                                        }

                                        break;
                                    }

                                case "霊力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 0.2d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PlanaLevel()) + ")。";
                                        break;
                                    }

                                case "オーラ":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 5d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.AuraLevel()) + ")。";
                                        break;
                                    }

                                case "超能力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 5d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PsychicLevel()) + ")。";
                                        break;
                                    }

                                default:
                                    {
                                        sname = u.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 5d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + SrcFormatter.Format(lv_mod * u.SkillLevel(opt)) + ")。";
                                        break;
                                    }
                            }
                        }

                        break;
                    }

                case "アクティブプロテクション":
                    {
                        sname = p.SkillName0("Ｓ防御");
                        prob = (int)((long)(p.SkillLevel("Ｓ防御", ref_mode: "") * 100d) / 16L);
                        msg = sname + "Lv/16の確率(" + SrcFormatter.Format(prob) + "%)で発動し、";
                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(fdata, 2)) && GeneralLib.LIndex(fdata, 2) != "全")
                        {
                            if (Strings.Left(GeneralLib.LIndex(fdata, 2), 1) == "!")
                            {
                                msg = msg + "「" + Strings.Mid(GeneralLib.LIndex(fdata, 2), 2) + "」属性を持たない";
                            }
                            else
                            {
                                msg = msg + "「" + GeneralLib.LIndex(fdata, 2) + "」属性を持つ";
                            }
                        }

                        if (flevel > 10d)
                        {
                            msg = msg + "攻撃のダメージを" + SrcFormatter.Format((int)(10d * flevel - 100d)) + "%吸収する。";
                        }
                        else if (flevel >= 0d)
                        {
                            msg = msg + "攻撃のダメージを" + SrcFormatter.Format((int)(10d * flevel)) + "%減少させる。";
                        }
                        else
                        {
                            msg = msg + "攻撃のダメージを" + SrcFormatter.Format((int)(-10 * flevel)) + "%増加させる。";
                        }

                        int localStrToLng8() { string argexpr = GeneralLib.LIndex(fdata, 3); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                        int localStrToLng9() { string argexpr = GeneralLib.LIndex(fdata, 3); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                        if (!Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
                        {
                            msg = msg + ";発動時に10" + Expression.Term("ＥＮ", u) + "増加。";
                        }
                        else if (localStrToLng8() > 0)
                        {
                            msg = msg + ";発動時に" + GeneralLib.LIndex(fdata, 3) + Expression.Term("ＥＮ", u) + "消費。";
                        }
                        else if (localStrToLng9() < 0)
                        {
                            msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(fdata, 3), 2) + Expression.Term("ＥＮ", u) + "増加。";
                        }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 4)) > 50)
                        {
                            msg = msg + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 4) + "以上で使用可能。";
                        }

                        var loopTo7 = GeneralLib.LLength(fdata);
                        for (i = 5; i <= loopTo7; i++)
                        {
                            opt = GeneralLib.LIndex(fdata, i);
                            idx = (int)Strings.InStr(opt, "*");
                            if (idx > 0)
                            {
                                lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
                                opt = Strings.Left(opt, idx - 1);
                            }
                            else
                            {
                                lv_mod = -1;
                            }

                            switch (p.SkillType(opt) ?? "")
                            {
                                case "相殺":
                                    {
                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、隣接時に効果は相殺。";
                                        break;
                                    }

                                case "中和":
                                    {
                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、" + "隣接時にレベル分だけ効果を中和。";
                                        break;
                                    }

                                case "近接無効":
                                    {
                                        msg = msg + ";「武」「突」「接」による攻撃には無効。";
                                        break;
                                    }

                                case "バリア無効化無効":
                                    {
                                        msg = msg + ";バリア無効化によって無効化されない。";
                                        break;
                                    }

                                case "手動":
                                    {
                                        msg = msg + ";防御選択時にのみ発動。";
                                        break;
                                    }

                                case "能力必要":
                                    {
                                        break;
                                    }
                                // スキップ
                                case "同調率":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 0.5d;
                                        }

                                        if (u.SyncLevel() >= 30d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + "%)。";
                                        }
                                        else if (u.SyncLevel() > 0d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + "%)。";
                                        }

                                        break;
                                    }

                                case "霊力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 0.2d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PlanaLevel()) + "%)。";
                                        break;
                                    }

                                case "オーラ":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 5d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.AuraLevel()) + "%)。";
                                        break;
                                    }

                                case "超能力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 5d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PsychicLevel()) + "%)。";
                                        break;
                                    }

                                default:
                                    {
                                        sname = u.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 5d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + SrcFormatter.Format(lv_mod * u.SkillLevel(opt)) + "%)。";
                                        break;
                                    }
                            }
                        }

                        break;
                    }

                case "広域プロテクション":
                    {
                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)) && GeneralLib.LIndex(fdata, 2) != "1")
                        {
                            msg = "半径" + Strings.StrConv(GeneralLib.LIndex(fdata, 2), VbStrConv.Wide) + "マス以内の味方ユニットに対する";
                            i = Conversions.ToInteger(GeneralLib.LIndex(fdata, 2));
                        }
                        else
                        {
                            msg = "隣接する味方ユニットに対する";
                            i = 1;
                        }

                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(fdata, 3)) && GeneralLib.LIndex(fdata, 3) != "全")
                        {
                            if (Strings.Left(GeneralLib.LIndex(fdata, 3), 1) == "!")
                            {
                                msg = msg + "「" + Strings.Mid(GeneralLib.LIndex(fdata, 3), 2) + "」属性を持たない";
                            }
                            else
                            {
                                msg = msg + "「" + GeneralLib.LIndex(fdata, 3) + "」属性を持つ";
                            }
                        }

                        if (flevel > 10d)
                        {
                            msg = msg + "攻撃のダメージを" + SrcFormatter.Format((int)(10d * flevel - 100d)) + "%吸収する。";
                        }
                        else if (flevel >= 0d)
                        {
                            msg = msg + "攻撃のダメージを" + SrcFormatter.Format((int)(10d * flevel)) + "%減少させる。";
                        }
                        else
                        {
                            msg = msg + "攻撃のダメージを" + SrcFormatter.Format((int)(-10 * flevel)) + "%増加させる。";
                        }

                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
                        {
                            int localStrToLng10() { string argexpr = GeneralLib.LIndex(fdata, 4); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                            if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 4)) > 0)
                            {
                                msg = msg + ";発動時に" + GeneralLib.LIndex(fdata, 4) + Expression.Term("ＥＮ", u) + "消費。";
                            }
                            else if (localStrToLng10() < 0)
                            {
                                msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(fdata, 4), 2) + Expression.Term("ＥＮ", u) + "増加。";
                            }
                        }
                        else
                        {
                            msg = msg + ";発動時に" + SrcFormatter.Format(20 * i) + Expression.Term("ＥＮ", u) + "消費。";
                        }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 5)) > 50)
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 5) + "以上で使用可能。";
                        }

                        msg = msg + ";ただし攻撃側も有効範囲内にいる場合は無効化。";
                        break;
                    }

                case "アーマー":
                    {
                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(fdata, 2)) && GeneralLib.LIndex(fdata, 2) != "全")
                        {
                            if (Strings.Left(GeneralLib.LIndex(fdata, 2), 1) == "!")
                            {
                                msg = "「" + Strings.Mid(GeneralLib.LIndex(fdata, 2), 2) + "」属性を持たない";
                            }
                            else
                            {
                                msg = "「" + GeneralLib.LIndex(fdata, 2) + "」属性を持つ";
                            }
                        }

                        if (flevel >= 0d)
                        {
                            msg = msg + "攻撃に対して装甲を" + SrcFormatter.Format((int)(100d * flevel)) + "増加させる。";
                        }
                        else
                        {
                            msg = msg + "攻撃に対して装甲を" + SrcFormatter.Format((int)(-100 * flevel)) + "減少させる。";
                        }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 3)) > 50)
                        {
                            msg = msg + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 3) + "以上で使用可能。";
                        }

                        var loopTo8 = GeneralLib.LLength(fdata);
                        for (i = 4; i <= loopTo8; i++)
                        {
                            opt = GeneralLib.LIndex(fdata, i);
                            idx = (int)Strings.InStr(opt, "*");
                            if (idx > 0)
                            {
                                lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
                                opt = Strings.Left(opt, idx - 1);
                            }
                            else
                            {
                                lv_mod = -1;
                            }

                            switch (p.SkillType(opt) ?? "")
                            {
                                case "能力必要":
                                    {
                                        break;
                                    }
                                // スキップ
                                case "同調率":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 5d;
                                        }

                                        if (u.SyncLevel() >= 30d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
                                        }
                                        else if (u.SyncLevel() > 0d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
                                        }

                                        break;
                                    }

                                case "霊力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 2d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PlanaLevel()) + ")。";
                                        break;
                                    }

                                case "オーラ":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 50d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.AuraLevel()) + ")。";
                                        break;
                                    }

                                case "超能力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 50d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PsychicLevel()) + ")。";
                                        break;
                                    }

                                default:
                                    {
                                        sname = u.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 50d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + SrcFormatter.Format(lv_mod * u.SkillLevel(opt)) + ")。";
                                        break;
                                    }
                            }
                        }

                        break;
                    }

                case "レジスト":
                    {
                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(fdata, 2)) && GeneralLib.LIndex(fdata, 2) != "全")
                        {
                            if (Strings.Left(GeneralLib.LIndex(fdata, 2), 1) == "!")
                            {
                                msg = "「" + Strings.Mid(GeneralLib.LIndex(fdata, 2), 2) + "」属性を持たない";
                            }
                            else
                            {
                                msg = "「" + GeneralLib.LIndex(fdata, 2) + "」属性を持つ";
                            }
                        }

                        if (flevel > 10d)
                        {
                            msg = msg + "攻撃に対してダメージを" + SrcFormatter.Format(100 - (int)(10d * flevel)) + "%吸収する。";
                        }
                        else if (flevel >= 0d)
                        {
                            msg = msg + "攻撃に対してダメージを" + SrcFormatter.Format((int)(10d * flevel)) + "%軽減させる。";
                        }
                        else
                        {
                            msg = msg + "攻撃に対してダメージを" + SrcFormatter.Format((int)(-10 * flevel)) + "%増加させる。";
                        }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 3)) > 50)
                        {
                            msg = msg + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 3) + "以上で使用可能。";
                        }

                        var loopTo9 = GeneralLib.LLength(fdata);
                        for (i = 4; i <= loopTo9; i++)
                        {
                            opt = GeneralLib.LIndex(fdata, i);
                            idx = (int)Strings.InStr(opt, "*");
                            if (idx > 0)
                            {
                                lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
                                opt = Strings.Left(opt, idx - 1);
                            }
                            else
                            {
                                lv_mod = -1;
                            }

                            switch (p.SkillType(opt) ?? "")
                            {
                                case "能力必要":
                                    {
                                        break;
                                    }
                                // スキップ
                                case "同調率":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 5d;
                                        }

                                        if (u.SyncLevel() >= 30d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + "%)。";
                                        }
                                        else if (u.SyncLevel() > 0d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + "%)。";
                                        }

                                        break;
                                    }

                                case "霊力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 2d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PlanaLevel()) + "%)。";
                                        break;
                                    }

                                case "オーラ":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 50d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.AuraLevel()) + "%)。";
                                        break;
                                    }

                                case "超能力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 50d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PsychicLevel()) + "%)。";
                                        break;
                                    }

                                default:
                                    {
                                        sname = u.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 50d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + SrcFormatter.Format(lv_mod * u.SkillLevel(opt)) + "%)。";
                                        break;
                                    }
                            }
                        }

                        break;
                    }

                case "当て身技":
                    {
                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(fdata, 3)) && GeneralLib.LIndex(fdata, 3) != "全")
                        {
                            if (Strings.Left(GeneralLib.LIndex(fdata, 3), 1) == "!")
                            {
                                msg = "「" + Strings.Mid(GeneralLib.LIndex(fdata, 3), 2) + "」属性を持たない";
                            }
                            else
                            {
                                msg = "「" + GeneralLib.LIndex(fdata, 3) + "」属性を持つ";
                            }
                        }

                        if (flevel != 1d)
                        {
                            msg = msg + "ダメージ" + SrcFormatter.Format((int)(500d * flevel)) + "までの";
                        }

                        msg = msg + "攻撃を";
                        buf = GeneralLib.LIndex(fdata, 4);
                        if (Information.IsNumeric(buf))
                        {
                            if (buf != "100")
                            {
                                msg = msg + buf + "%の確率で受け止め、";
                            }
                            else
                            {
                                msg = msg + "受け止め、";
                            }
                        }
                        else if (Strings.InStr(buf, "+") > 0 | Strings.InStr(buf, "-") > 0)
                        {
                            i = (int)GeneralLib.MaxLng(Strings.InStr(buf, "+"), Strings.InStr(buf, "-"));
                            sname = u.SkillName0(Strings.Left(buf, i - 1));
                            prob = (int)((long)((u.SkillLevel(Strings.Left(buf, i - 1)) + Conversions.ToInteger(Strings.Mid(buf, i))) * 100d) / 16L);
                            msg = msg + "(" + sname + "Lv" + Strings.Mid(buf, i) + ")/16の確率(" + SrcFormatter.Format(prob) + "%)で受け止め、";
                        }
                        else
                        {
                            sname = u.SkillName0(buf);
                            prob = (int)((long)(u.SkillLevel(buf) * 100d) / 16L);
                            msg = msg + sname + "Lv/16の確率(" + SrcFormatter.Format(prob) + "%)で受け止め、";
                        }

                        buf = GeneralLib.LIndex(fdata, 2);
                        if (Strings.InStr(buf, "(") > 0)
                        {
                            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
                        }

                        msg = msg + buf + "で反撃。";
                        int localStrToLng11() { string argexpr = GeneralLib.LIndex(fdata, 5); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 5)) > 0)
                        {
                            msg = msg + ";発動時に" + GeneralLib.LIndex(fdata, 5) + Expression.Term("ＥＮ", u) + "消費。";
                        }
                        else if (localStrToLng11() < 0)
                        {
                            msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(fdata, 5), 2) + Expression.Term("ＥＮ", u) + "増加。";
                        }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 6)) > 50)
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 6) + "以上で使用可能。";
                        }

                        var loopTo10 = GeneralLib.LLength(fdata);
                        for (i = 7; i <= loopTo10; i++)
                        {
                            opt = GeneralLib.LIndex(fdata, i);
                            idx = (int)Strings.InStr(opt, "*");
                            if (idx > 0)
                            {
                                lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
                                opt = Strings.Left(opt, idx - 1);
                            }
                            else
                            {
                                lv_mod = -1;
                            }

                            switch (p.SkillType(opt) ?? "")
                            {
                                case "相殺":
                                    {
                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、隣接時に効果は相殺。";
                                        break;
                                    }

                                case "中和":
                                    {
                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、" + "隣接時にレベル分だけ効果を相殺。";
                                        break;
                                    }

                                case "近接無効":
                                    {
                                        msg = msg + ";「武」「突」「接」による攻撃には無効。";
                                        break;
                                    }

                                case "手動":
                                    {
                                        msg = msg + ";防御選択時にのみ発動。";
                                        break;
                                    }

                                case "能力必要":
                                    {
                                        break;
                                    }
                                // スキップ
                                case "同調率":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 20d;
                                        }

                                        if (u.SyncLevel() >= 30d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
                                        }
                                        else if (u.SyncLevel() > 0d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
                                        }

                                        break;
                                    }

                                case "霊力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 10d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PlanaLevel()) + ")。";
                                        break;
                                    }

                                case "オーラ":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.AuraLevel()) + ")。";
                                        break;
                                    }

                                case "超能力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PsychicLevel()) + ")。";
                                        break;
                                    }

                                default:
                                    {
                                        sname = u.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + SrcFormatter.Format(lv_mod * u.SkillLevel(opt)) + ")。";
                                        break;
                                    }
                            }
                        }

                        break;
                    }

                case "反射":
                    {
                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(fdata, 2)) && GeneralLib.LIndex(fdata, 2) != "全")
                        {
                            if (Strings.Left(GeneralLib.LIndex(fdata, 2), 1) == "!")
                            {
                                msg = "「" + Strings.Mid(GeneralLib.LIndex(fdata, 2), 2) + "」属性を持たない";
                            }
                            else
                            {
                                msg = "「" + GeneralLib.LIndex(fdata, 2) + "」属性を持つ";
                            }
                        }

                        if (flevel != 1d)
                        {
                            msg = msg + "ダメージ" + SrcFormatter.Format((int)(500d * flevel)) + "までの";
                        }

                        msg = msg + "攻撃を";
                        buf = GeneralLib.LIndex(fdata, 3);
                        if (Information.IsNumeric(buf))
                        {
                            if (buf != "100")
                            {
                                msg = msg + buf + "%の確率で反射。";
                            }
                            else
                            {
                                msg = msg + "反射。";
                            }
                        }
                        else if (Strings.InStr(buf, "+") > 0 | Strings.InStr(buf, "-") > 0)
                        {
                            i = (int)GeneralLib.MaxLng(Strings.InStr(buf, "+"), Strings.InStr(buf, "-"));
                            sname = u.SkillName0(Strings.Left(buf, i - 1));
                            prob = (int)((long)((u.SkillLevel(Strings.Left(buf, i - 1)) + Conversions.ToInteger(Strings.Mid(buf, i))) * 100d) / 16L);
                            msg = msg + "(" + sname + "Lv" + Strings.Mid(buf, i) + ")/16の確率(" + SrcFormatter.Format(prob) + "%)で反射。";
                        }
                        else
                        {
                            sname = u.SkillName0(buf);
                            prob = (int)((long)(u.SkillLevel(buf) * 100d) / 16L);
                            msg = msg + sname + "Lv/16の確率(" + SrcFormatter.Format(prob) + "%)で反射。";
                        }

                        int localStrToLng12() { string argexpr = GeneralLib.LIndex(fdata, 4); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 4)) > 0)
                        {
                            msg = msg + ";発動時に" + GeneralLib.LIndex(fdata, 4) + Expression.Term("ＥＮ", u) + "消費。";
                        }
                        else if (localStrToLng12() < 0)
                        {
                            msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(fdata, 4), 2) + Expression.Term("ＥＮ", u) + "増加。";
                        }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 5)) > 50)
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 5) + "以上で使用可能。";
                        }

                        var loopTo11 = GeneralLib.LLength(fdata);
                        for (i = 6; i <= loopTo11; i++)
                        {
                            opt = GeneralLib.LIndex(fdata, i);
                            idx = (int)Strings.InStr(opt, "*");
                            if (idx > 0)
                            {
                                lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
                                opt = Strings.Left(opt, idx - 1);
                            }
                            else
                            {
                                lv_mod = -1;
                            }

                            switch (p.SkillType(opt) ?? "")
                            {
                                case "相殺":
                                    {
                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、隣接時に効果は相殺。";
                                        break;
                                    }

                                case "中和":
                                    {
                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、" + "隣接時にレベル分だけ効果を中和。";
                                        break;
                                    }

                                case "近接無効":
                                    {
                                        msg = msg + ";「武」「突」「接」による攻撃には無効。";
                                        break;
                                    }

                                case "手動":
                                    {
                                        msg = msg + ";防御選択時にのみ発動。";
                                        break;
                                    }

                                case "能力必要":
                                    {
                                        break;
                                    }
                                // スキップ
                                case "同調率":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 20d;
                                        }

                                        if (u.SyncLevel() >= 30d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
                                        }
                                        else if (u.SyncLevel() > 0d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
                                        }

                                        break;
                                    }

                                case "霊力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 10d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PlanaLevel()) + ")。";
                                        break;
                                    }

                                case "オーラ":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.AuraLevel()) + ")。";
                                        break;
                                    }

                                case "超能力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PsychicLevel()) + ")。";
                                        break;
                                    }

                                default:
                                    {
                                        sname = u.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + SrcFormatter.Format(lv_mod * u.SkillLevel(opt)) + ")。";
                                        break;
                                    }
                            }
                        }

                        break;
                    }

                case "阻止":
                    {
                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(fdata, 2)) && GeneralLib.LIndex(fdata, 2) != "全")
                        {
                            if (Strings.Left(GeneralLib.LIndex(fdata, 2), 1) == "!")
                            {
                                msg = "「" + Strings.Mid(GeneralLib.LIndex(fdata, 2), 2) + "」属性を持たない";
                            }
                            else
                            {
                                msg = "「" + GeneralLib.LIndex(fdata, 2) + "」属性を持つ";
                            }
                        }

                        if (flevel != 1d)
                        {
                            msg = msg + "ダメージ" + SrcFormatter.Format((int)(500d * flevel)) + "以下の";
                        }

                        msg = msg + "攻撃を";
                        buf = GeneralLib.LIndex(fdata, 3);
                        if (Information.IsNumeric(buf))
                        {
                            if (buf != "100")
                            {
                                msg = msg + buf + "%の確率で阻止。";
                            }
                            else
                            {
                                // MOD START MARGE
                                // msg = msg && buf && "阻止。"
                                msg = msg + "阻止。";
                                // MOD END MARGE
                            }
                        }
                        else if (Strings.InStr(buf, "+") > 0 | Strings.InStr(buf, "-") > 0)
                        {
                            i = (int)GeneralLib.MaxLng(Strings.InStr(buf, "+"), Strings.InStr(buf, "-"));
                            sname = u.SkillName0(Strings.Left(buf, i - 1));
                            prob = (int)((long)((u.SkillLevel(Strings.Left(buf, i - 1)) + Conversions.ToInteger(Strings.Mid(buf, i))) * 100d) / 16L);
                            msg = msg + "(" + sname + "Lv" + Strings.Mid(buf, i) + ")/16の確率(" + SrcFormatter.Format(prob) + "%)で阻止。";
                        }
                        else
                        {
                            sname = u.SkillName0(buf);
                            prob = (int)((long)(u.SkillLevel(buf) * 100d) / 16L);
                            msg = msg + sname + "Lv/16の確率(" + SrcFormatter.Format(prob) + "%)で阻止。";
                        }

                        int localStrToLng13() { string argexpr = GeneralLib.LIndex(fdata, 4); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 4)) > 0)
                        {
                            msg = msg + ";発動時に" + GeneralLib.LIndex(fdata, 4) + Expression.Term("ＥＮ", u) + "消費。";
                        }
                        else if (localStrToLng13() < 0)
                        {
                            msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(fdata, 4), 2) + Expression.Term("ＥＮ", u) + "増加。";
                        }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 5)) > 50)
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 5) + "以上で使用可能。";
                        }

                        var loopTo12 = GeneralLib.LLength(fdata);
                        for (i = 6; i <= loopTo12; i++)
                        {
                            opt = GeneralLib.LIndex(fdata, i);
                            idx = (int)Strings.InStr(opt, "*");
                            if (idx > 0)
                            {
                                lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
                                opt = Strings.Left(opt, idx - 1);
                            }
                            else
                            {
                                lv_mod = -1;
                            }

                            switch (p.SkillType(opt) ?? "")
                            {
                                case "相殺":
                                    {
                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、隣接時に効果は相殺。";
                                        break;
                                    }

                                case "中和":
                                    {
                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、" + "隣接時にレベル分だけ効果を中和。";
                                        break;
                                    }

                                case "近接無効":
                                    {
                                        msg = msg + ";「武」「突」「接」による攻撃には無効。";
                                        break;
                                    }

                                case "手動":
                                    {
                                        msg = msg + ";防御選択時にのみ発動。";
                                        break;
                                    }

                                case "能力必要":
                                    {
                                        break;
                                    }
                                // スキップ
                                case "同調率":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 20d;
                                        }

                                        if (u.SyncLevel() >= 30d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
                                        }
                                        else if (u.SyncLevel() > 0d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
                                        }

                                        break;
                                    }

                                case "霊力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 10d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PlanaLevel()) + ")。";
                                        break;
                                    }

                                case "オーラ":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.AuraLevel()) + ")。";
                                        break;
                                    }

                                case "超能力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PsychicLevel()) + ")。";
                                        break;
                                    }

                                default:
                                    {
                                        sname = u.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + SrcFormatter.Format(lv_mod * u.SkillLevel(opt)) + ")。";
                                        break;
                                    }
                            }
                        }

                        break;
                    }

                case "広域阻止":
                    {
                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)) && GeneralLib.LIndex(fdata, 2) != "1")
                        {
                            msg = "半径" + Strings.StrConv(GeneralLib.LIndex(fdata, 2), VbStrConv.Wide) + "マス以内の味方ユニットに対する";
                            i = Conversions.ToInteger(GeneralLib.LIndex(fdata, 2));
                        }
                        else
                        {
                            msg = "隣接する味方ユニットに対する";
                            i = 1;
                        }

                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(fdata, 3)) && GeneralLib.LIndex(fdata, 3) != "全")
                        {
                            if (Strings.Left(GeneralLib.LIndex(fdata, 3), 1) == "!")
                            {
                                msg = msg + "「" + Strings.Mid(GeneralLib.LIndex(fdata, 3), 2) + "」属性を持たない";
                            }
                            else
                            {
                                msg = msg + "「" + GeneralLib.LIndex(fdata, 3) + "」属性を持つ";
                            }
                        }

                        if (flevel != 1d)
                        {
                            msg = msg + "ダメージ" + SrcFormatter.Format((int)(500d * flevel)) + "以下の";
                        }

                        msg = msg + "攻撃を";
                        buf = GeneralLib.LIndex(fdata, 4);
                        if (Information.IsNumeric(buf))
                        {
                            if (buf != "100")
                            {
                                // MOD START MARGE
                                // msg = msg && "%の確率で阻止。"
                                msg = msg + buf + "%の確率で阻止。";
                            }
                            // MOD END MARGE
                            else
                            {
                                msg = msg + "阻止。";
                            }
                        }
                        else if (Strings.InStr(buf, "+") > 0 | Strings.InStr(buf, "-") > 0)
                        {
                            i = (int)GeneralLib.MaxLng(Strings.InStr(buf, "+"), Strings.InStr(buf, "-"));
                            sname = u.SkillName0(Strings.Left(buf, i - 1));
                            prob = (int)((long)((u.SkillLevel(Strings.Left(buf, i - 1)) + Conversions.ToInteger(Strings.Mid(buf, i))) * 100d) / 16L);
                            msg = msg + "(" + sname + "Lv" + Strings.Mid(buf, i) + ")/16の確率(" + SrcFormatter.Format(prob) + "%)で阻止。";
                        }
                        else
                        {
                            sname = u.SkillName0(buf);
                            prob = (int)((long)(u.SkillLevel(buf) * 100d) / 16L);
                            msg = msg + sname + "Lv/16の確率(" + SrcFormatter.Format(prob) + "%)で阻止。";
                        }

                        int localStrToLng14() { string argexpr = GeneralLib.LIndex(fdata, 5); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 5)) > 0)
                        {
                            msg = msg + ";発動時に" + GeneralLib.LIndex(fdata, 5) + Expression.Term("ＥＮ", u) + "消費。";
                        }
                        else if (localStrToLng14() < 0)
                        {
                            msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(fdata, 5), 2) + Expression.Term("ＥＮ", u) + "増加。";
                        }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 6)) > 50)
                        {
                            msg = msg + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 6) + "以上で使用可能。";
                        }

                        msg = msg + ";ただし攻撃側も有効範囲内にいる場合は無効化。";
                        break;
                    }

                case "融合":
                    {
                        prob = (int)((long)(flevel * 100d) / 16L);
                        msg = SrcFormatter.Format(flevel) + "/16の確率(" + SrcFormatter.Format(prob) + "%)で発動し、" + "ダメージを" + Expression.Term("ＨＰ", u) + "に変換。;" + "ただし、「武」「突」「接」による攻撃には無効。";
                        break;
                    }

                case "変換":
                    {
                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(fdata, 2)) && GeneralLib.LIndex(fdata, 2) != "全")
                        {
                            if (Strings.Left(GeneralLib.LIndex(fdata, 2), 1) == "!")
                            {
                                msg = "「" + Strings.Mid(GeneralLib.LIndex(fdata, 2), 2) + "」属性を持たない";
                            }
                            else
                            {
                                msg = "「" + GeneralLib.LIndex(fdata, 2) + "」属性を持つ";
                            }
                        }

                        msg = msg + "攻撃を受けた際にダメージを" + Expression.Term("ＥＮ", u) + "に変換。;" + "変換効率は " + Expression.Term("ＥＮ", u) + "増加 ＝ ";
                        msg = msg + SrcFormatter.Format(0.01d * flevel);
                        msg = msg + " × ダメージ";
                        break;
                    }

                case "ビーム吸収":
                    {
                        msg = "ビームによる攻撃のダメージをＨＰに変換";
                        break;
                    }

                case "自動反撃":
                    {
                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(fdata, 3)) && GeneralLib.LIndex(fdata, 3) != "全")
                        {
                            if (Strings.Left(GeneralLib.LIndex(fdata, 3), 1) == "!")
                            {
                                msg = "「" + Strings.Mid(GeneralLib.LIndex(fdata, 3), 2) + "」属性を持たない";
                            }
                            else
                            {
                                msg = "「" + GeneralLib.LIndex(fdata, 3) + "」属性を持つ";
                            }
                        }

                        if (flevel != 1d)
                        {
                            msg = msg + "ダメージ" + SrcFormatter.Format((int)(500d * flevel)) + "までの";
                        }

                        msg = msg + "攻撃を受けた際に";
                        buf = GeneralLib.LIndex(fdata, 4);
                        if (Information.IsNumeric(buf))
                        {
                            if (buf != "100")
                            {
                                msg = msg + buf + "%の確率で、";
                            }
                        }
                        else if (Strings.InStr(buf, "+") > 0 | Strings.InStr(buf, "-") > 0)
                        {
                            i = (int)GeneralLib.MaxLng(Strings.InStr(buf, "+"), Strings.InStr(buf, "-"));
                            sname = u.SkillName0(Strings.Left(buf, i - 1));
                            prob = (int)((long)((u.SkillLevel(Strings.Left(buf, i - 1)) + Conversions.ToInteger(Strings.Mid(buf, i))) * 100d) / 16L);
                            msg = msg + "(" + sname + "Lv" + Strings.Mid(buf, i) + ")/16の確率(" + SrcFormatter.Format(prob) + "%)で、";
                        }
                        else
                        {
                            sname = u.SkillName0(buf);
                            prob = (int)((long)(u.SkillLevel(buf) * 100d) / 16L);
                            msg = msg + sname + "Lv/16の確率(" + SrcFormatter.Format(prob) + "%)で、";
                        }

                        buf = GeneralLib.LIndex(fdata, 2);
                        if (Strings.InStr(buf, "(") > 0)
                        {
                            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
                        }

                        msg = msg + buf + "による自動反撃が発動する。";
                        int localStrToLng15() { string argexpr = GeneralLib.LIndex(fdata, 5); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 5)) > 0)
                        {
                            msg = msg + ";発動時に" + GeneralLib.LIndex(fdata, 5) + Expression.Term("ＥＮ", u) + "消費。";
                        }
                        else if (localStrToLng15() < 0)
                        {
                            msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(fdata, 5), 2) + Expression.Term("ＥＮ", u) + "増加。";
                        }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 6)) > 50)
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 6) + "以上で使用可能。";
                        }

                        var loopTo13 = GeneralLib.LLength(fdata);
                        for (i = 7; i <= loopTo13; i++)
                        {
                            opt = GeneralLib.LIndex(fdata, i);
                            idx = (int)Strings.InStr(opt, "*");
                            if (idx > 0)
                            {
                                lv_mod = GeneralLib.StrToDbl(Strings.Mid(opt, idx + 1));
                                opt = Strings.Left(opt, idx - 1);
                            }
                            else
                            {
                                lv_mod = -1;
                            }

                            switch (p.SkillType(opt) ?? "")
                            {
                                case "相殺":
                                    {
                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、隣接時に効果は相殺。";
                                        break;
                                    }

                                case "中和":
                                    {
                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、" + "隣接時にレベル分だけ効果を相殺。";
                                        break;
                                    }

                                case "近接無効":
                                    {
                                        msg = msg + ";「武」「突」「接」による攻撃には無効。";
                                        break;
                                    }

                                case "手動":
                                    {
                                        msg = msg + ";防御選択時にのみ発動。";
                                        break;
                                    }

                                case "能力必要":
                                    {
                                        break;
                                    }
                                // スキップ
                                case "同調率":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 20d;
                                        }

                                        if (u.SyncLevel() >= 30d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
                                        }
                                        else if (u.SyncLevel() > 0d)
                                        {
                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + SrcFormatter.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
                                        }

                                        break;
                                    }

                                case "霊力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 10d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PlanaLevel()) + ")。";
                                        break;
                                    }

                                case "オーラ":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.AuraLevel()) + ")。";
                                        break;
                                    }

                                case "超能力":
                                    {
                                        sname = p.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + SrcFormatter.Format(lv_mod * u.PsychicLevel()) + ")。";
                                        break;
                                    }

                                default:
                                    {
                                        sname = u.SkillName0(opt);
                                        if (lv_mod == -1)
                                        {
                                            lv_mod = 200d;
                                        }

                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + SrcFormatter.Format(lv_mod * u.SkillLevel(opt)) + ")。";
                                        break;
                                    }
                            }
                        }

                        break;
                    }

                case "ＨＰ回復":
                    {
                        msg = "毎ターン最大" + Expression.Term("ＨＰ", u) + "の" + SrcFormatter.Format(10d * flevel) + "%分の" + Expression.Term("ＨＰ", u) + "を回復。";
                        break;
                    }

                case "ＥＮ回復":
                    {
                        msg = "毎ターン最大" + Expression.Term("ＥＮ", u) + "の" + SrcFormatter.Format(10d * flevel) + "%分の" + Expression.Term("ＥＮ", u) + "を回復。";
                        break;
                    }

                case "霊力回復":
                    {
                        sname = p.SkillName0("霊力");
                        msg = "毎ターン最大" + sname + "の" + SrcFormatter.Format(10d * flevel) + "%分の" + sname + "を回復。";
                        break;
                    }

                case "ＨＰ消費":
                    {
                        msg = "毎ターン最大" + Expression.Term("ＨＰ", u) + "の" + SrcFormatter.Format(10d * flevel) + "%分の" + Expression.Term("ＨＰ", u) + "を消費。";
                        break;
                    }

                case "ＥＮ消費":
                    {
                        msg = "毎ターン最大" + Expression.Term("ＥＮ", u) + "の" + SrcFormatter.Format(10d * flevel) + "%分の" + Expression.Term("ＥＮ", u) + "を消費。";
                        break;
                    }

                case "霊力消費":
                    {
                        sname = p.SkillName0("霊力");
                        msg = "毎ターン最大" + sname + "の" + SrcFormatter.Format(10d * flevel) + "%分の" + sname + "を消費。";
                        break;
                    }

                case "分身":
                    {
                        msg = "50%の確率で攻撃を完全に回避。;" + "発動条件：" + Expression.Term("気力", u) + "130以上";
                        break;
                    }

                case "超回避":
                    {
                        msg = "あらゆる攻撃を" + SrcFormatter.Format(10d * flevel) + "%の確率で回避。";
                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                        {
                            int localStrToLng16() { string argexpr = GeneralLib.LIndex(fdata, 2); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                            if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 2)) > 0)
                            {
                                msg = msg + ";発動時に" + GeneralLib.LIndex(fdata, 2) + Expression.Term("ＥＮ", u) + "消費。";
                            }
                            else if (localStrToLng16() < 0)
                            {
                                msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(fdata, 2), 2) + Expression.Term("ＥＮ", u) + "増加。";
                            }
                        }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 3)) > 50)
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 3) + "以上で使用可能。";
                        }

                        if (GeneralLib.LIndex(fdata, 4) == "手動")
                        {
                            msg = msg + ";回避選択時にのみ発動。";
                        }

                        break;
                    }

                case "緊急テレポート":
                    {
                        msg = "攻撃を受けた際に" + SrcFormatter.Format(10d * flevel) + "%の確率で" + "テレポートし、攻撃を回避。;" + "テレポート先は" + GeneralLib.LIndex(fdata, 2) + "マス以内の範囲の内、" + "最も敵から遠い地点から選ばれる。";
                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
                        {
                            int localStrToLng17() { string argexpr = GeneralLib.LIndex(fdata, 3); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                            if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 3)) > 0)
                            {
                                msg = msg + ";発動時に" + GeneralLib.LIndex(fdata, 3) + Expression.Term("ＥＮ", u) + "消費。";
                            }
                            else if (localStrToLng17() < 0)
                            {
                                msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(fdata, 3), 2) + Expression.Term("ＥＮ", u) + "増加。";
                            }
                        }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 4)) > 50)
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 4) + "以上で使用可能。";
                        }

                        if (GeneralLib.LIndex(fdata, 5) == "手動")
                        {
                            msg = msg + ";回避選択時にのみ発動。";
                        }

                        break;
                    }

                case "ダミー":
                    {
                        buf = fname;
                        if (Conversions.ToBoolean(Strings.InStr(buf, "Lv")))
                        {
                            buf = Strings.Left(buf, Strings.InStr(buf, "Lv") - 1);
                        }

                        msg = buf + "を身代わりにして攻撃を" + SrcFormatter.Format(flevel) + "回まで回避。";
                        break;
                    }

                case "攻撃回避":
                    {
                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(fdata, 2)) && GeneralLib.LIndex(fdata, 2) != "全")
                        {
                            if (Strings.Left(GeneralLib.LIndex(fdata, 2), 1) == "!")
                            {
                                msg = "「" + Strings.Mid(GeneralLib.LIndex(fdata, 2), 2) + "」属性を持たない";
                            }
                            else
                            {
                                msg = "「" + GeneralLib.LIndex(fdata, 2) + "」属性を持つ";
                            }
                        }

                        if (flevel >= 0d)
                        {
                            msg = msg + "攻撃の命中率を本来の" + SrcFormatter.Format((int)(100d - 10d * flevel)) + "%に減少させる。";
                        }
                        else
                        {
                            msg = msg + "攻撃の命中率を本来の" + SrcFormatter.Format((int)(100d - 10d * flevel)) + "%に増加させる。";
                        }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 3)) > 50)
                        {
                            msg = msg + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 3) + "以上で使用可能。";
                        }

                        break;
                    }

                case "抵抗力":
                    {
                        if (flevel >= 0d)
                        {
                            msg = "武器の特殊効果を受ける確率を" + SrcFormatter.Format(10d * flevel) + "%減少させる。";
                        }
                        else
                        {
                            msg = "武器の特殊効果を受ける確率を" + SrcFormatter.Format(-10 * flevel) + "%増加させる。";
                        }

                        break;
                    }

                case "修理装置":
                    {
                        msg = "他のユニットの" + Expression.Term("ＨＰ", u);
                        switch (flevel)
                        {
                            case 1d:
                                {
                                    msg = msg + "を最大" + Expression.Term("ＨＰ", u) + "の30%だけ回復。";
                                    break;
                                }

                            case 2d:
                                {
                                    msg = msg + "を最大" + Expression.Term("ＨＰ", u) + "の50%だけ回復。";
                                    break;
                                }

                            case 3d:
                                {
                                    msg = msg + "を全快。";
                                    break;
                                }
                        }

                        break;
                    }

                case "補給装置":
                    {
                        msg = "他のユニットの" + Expression.Term("ＥＮ", u) + "と弾薬を全快。;" + "ただしユニットのパイロットの" + Expression.Term("気力", u) + "は-10。";
                        if (Expression.IsOptionDefined("移動後補給不可"))
                        {
                            msg = msg + "移動後は使用不可。";
                        }

                        break;
                    }

                case "修理不可":
                    {
                        var loopTo14 = (int)Conversions.ToInteger(fdata);
                        for (i = 2; i <= loopTo14; i++)
                        {
                            buf = GeneralLib.LIndex(fdata, i);
                            if (Strings.Left(buf, 1) == "!")
                            {
                                buf = Strings.Mid(buf, 2);
                                msg = msg + buf + "以外では" + Expression.Term("ＨＰ", u) + "を回復出来ない。";
                            }
                            else
                            {
                                msg = msg + buf + "では" + Expression.Term("ＨＰ", u) + "を回復出来ない。";
                            }
                        }

                        msg = msg + buf + ";ただし、" + Expression.Term("スペシャルパワー", u) + "や地形、母艦による回復は可能。";
                        break;
                    }

                case "霊力変換器":
                    {
                        sname = p.SkillName0("霊力");
                        msg = sname + "に合わせて各種能力が上昇する。";
                        if (flevel_specified)
                        {
                            msg = msg + ";（" + sname + "上限 = " + SrcFormatter.Format(flevel) + "）";
                        }

                        break;
                    }

                case "オーラ変換器":
                    {
                        sname = p.SkillName0("オーラ");
                        msg = sname + "レベルに合わせて各種能力が上昇する。";
                        if (flevel_specified)
                        {
                            msg = msg + ";（" + sname + "上限レベル = " + SrcFormatter.Format(flevel) + "）";
                        }

                        break;
                    }

                case "サイキックドライブ":
                    {
                        sname = p.SkillName0("超能力");
                        msg = sname + "レベルごとに" + Expression.Term("装甲", u) + "+100、" + Expression.Term("運動性", u) + "+5";
                        if (flevel_specified)
                        {
                            msg = msg + ";（" + sname + "上限レベル = " + SrcFormatter.Format(flevel) + "）";
                        }

                        break;
                    }

                case "シンクロドライブ":
                    {
                        sname = p.SkillName0("同調率");
                        msg = sname + "に合わせて各種能力が上昇する。";
                        if (flevel_specified)
                        {
                            msg = msg + ";（" + sname + "上限 = " + SrcFormatter.Format(flevel) + "%）";
                        }

                        break;
                    }

                case "ステルス":
                    {
                        if (flevel_specified)
                        {
                            msg = "敵から" + Strings.StrConv(SrcFormatter.Format(flevel), VbStrConv.Wide) + "マス以内にいない限り発見されない。" + "ただし自分から攻撃すると１ターン無効。";
                        }
                        else
                        {
                            msg = "敵から３マス以内にいない限り発見されない。" + "ただし自分から攻撃すると１ターン無効。";
                        }

                        break;
                    }

                case "ステルス無効化":
                    {
                        msg = "敵のステルス能力を無効化する。";
                        break;
                    }

                case "テレポート":
                    {
                        msg = "テレポートを行い、" + Expression.Term("移動力", u) + SrcFormatter.Format(u.Speed + flevel) + "で地形を無視して移動。;";
                        if (GeneralLib.LLength(fdata) > 1)
                        {
                            if (Conversions.ToInteger(GeneralLib.LIndex(fdata, 2)) > 0)
                            {
                                msg = msg + GeneralLib.LIndex(fdata, 2) + Expression.Term("ＥＮ", u) + "消費。";
                            }
                        }
                        else
                        {
                            msg = msg + "40" + Expression.Term("ＥＮ", u) + "消費。";
                        }

                        break;
                    }

                case "ジャンプ":
                    {
                        msg = Expression.Term("移動力", u) + SrcFormatter.Format(u.Speed + flevel) + "で地上地形を無視しながらジャンプ移動。";
                        if (GeneralLib.LLength(fdata) > 1)
                        {
                            if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 2)) > 0)
                            {
                                msg = msg + ";" + GeneralLib.LIndex(fdata, 2) + Expression.Term("ＥＮ", u) + "消費。";
                            }
                        }

                        break;
                    }

                case "水泳":
                    {
                        msg = "水中を泳いで移動可能。深海等の深い水の地形に進入することが出来る。" + "ただし水中での移動コストが１になる訳ではない。";
                        break;
                    }

                case "水上移動":
                    {
                        msg = "水上に浮かんで移動可能。";
                        break;
                    }

                case "ホバー移動":
                    {
                        msg = "空中に浮きながら移動することで砂漠と雪原の移動コストが１になる。" + "また、水上移動も可能。ただし移動時に5" + Expression.Term("ＥＮ", u) + "消費。";
                        break;
                    }

                case "透過移動":
                    {
                        msg = "障害物を無視して移動。";
                        break;
                    }

                case "すり抜け移動":
                    {
                        msg = "敵ユニットがいるマスを通過可能。";
                        break;
                    }

                case "線路移動":
                    {
                        msg = "線路上のみを移動可能。";
                        break;
                    }

                case "移動制限":
                    {
                        msg = msg + GeneralLib.LIndex(fdata, 2);
                        var loopTo15 = GeneralLib.LLength(fdata);
                        for (i = 3; i <= loopTo15; i++)
                            msg = msg + "、" + GeneralLib.LIndex(fdata, i);
                        msg = msg + "上のみを移動可能。";
                        break;
                    }

                case "進入不可":
                    {
                        msg = msg + GeneralLib.LIndex(fdata, 2);
                        var loopTo16 = GeneralLib.LLength(fdata);
                        for (i = 3; i <= loopTo16; i++)
                            msg = msg + "、" + GeneralLib.LIndex(fdata, i);
                        msg = msg + "には進入不可。";
                        break;
                    }

                case "地形適応":
                    {
                        msg = msg + GeneralLib.LIndex(fdata, 2);
                        var loopTo17 = GeneralLib.LLength(fdata);
                        for (i = 3; i <= loopTo17; i++)
                            msg = msg + "、" + GeneralLib.LIndex(fdata, i);
                        msg = msg + "における移動コストが１になる。";
                        break;
                    }

                case "追加移動力":
                    {
                        msg = GeneralLib.LIndex(fdata, 2) + "にいると、" + Expression.Term("移動力", u) + "が";
                        if (flevel >= 0d)
                        {
                            msg = msg + SrcFormatter.Format(flevel) + "増加。";
                        }
                        else
                        {
                            msg = msg + SrcFormatter.Format(-flevel) + "減少。";
                        }

                        break;
                    }

                case "母艦":
                    {
                        msg = "他のユニットを格納し、修理・運搬可能。";
                        break;
                    }

                case "格納不可":
                    {
                        msg = "母艦に格納することが出来ない。";
                        break;
                    }

                case "両手利き":
                    {
                        msg = "両手に武器を装備可能。";
                        break;
                    }

                case "部隊ユニット":
                    {
                        msg = "複数のユニットによって構成された部隊ユニット。";
                        break;
                    }

                case "召喚ユニット":
                    {
                        msg = "召喚されたユニット。";
                        break;
                    }

                case "変形":
                    {
                        if (u.IsHero())
                        {
                            buf = "変化";
                        }
                        else
                        {
                            buf = "変形";
                        }

                        if (GeneralLib.LLength(fdata) > 2)
                        {
                            msg = "以下の形態に" + buf + "; ";
                            var loopTo18 = GeneralLib.LLength(fdata);
                            for (i = 2; i <= loopTo18; i++)
                            {
                                Unit localOtherForm() { object argIndex1 = GeneralLib.LIndex(fdata, i); var ret = u.OtherForm(argIndex1); return ret; }

                                if (localOtherForm().IsAvailable())
                                {
                                    UnitData localItem2() { object argIndex1 = GeneralLib.LIndex(fdata, i); var ret = SRC.UDList.Item(argIndex1); return ret; }

                                    if ((u.Nickname ?? "") == (localItem2().Nickname ?? ""))
                                    {
                                        UnitData localItem() { object argIndex1 = GeneralLib.LIndex(fdata, i); var ret = SRC.UDList.Item(argIndex1); return ret; }

                                        uname = localItem().Name;
                                        if (Strings.Right(uname, 5) == "(前期型)")
                                        {
                                            uname = Strings.Left(uname, Strings.Len(uname) - 5);
                                        }
                                        else if (Strings.Right(uname, 5) == "・前期型)")
                                        {
                                            uname = Strings.Left(uname, Strings.Len(uname) - 5) + ")";
                                        }
                                        else if (Strings.Right(uname, 5) == "(後期型)")
                                        {
                                            uname = Strings.Left(uname, Strings.Len(uname) - 5);
                                        }
                                    }
                                    else
                                    {
                                        UnitData localItem1() { object argIndex1 = GeneralLib.LIndex(fdata, i); var ret = SRC.UDList.Item(argIndex1); return ret; }

                                        uname = localItem1().Nickname;
                                    }

                                    msg = msg + uname + "  ";
                                }
                            }
                        }
                        else
                        {
                            UnitData localItem5() { object argIndex1 = GeneralLib.LIndex(fdata, 2); var ret = SRC.UDList.Item(argIndex1); return ret; }

                            if ((u.Nickname ?? "") == (localItem5().Nickname ?? ""))
                            {
                                UnitData localItem3() { object argIndex1 = GeneralLib.LIndex(fdata, 2); var ret = SRC.UDList.Item(argIndex1); return ret; }

                                uname = localItem3().Name;
                            }
                            else
                            {
                                UnitData localItem4() { object argIndex1 = GeneralLib.LIndex(fdata, 2); var ret = SRC.UDList.Item(argIndex1); return ret; }

                                uname = localItem4().Nickname;
                            }

                            if (Strings.Right(uname, 5) == "(前期型)")
                            {
                                uname = Strings.Left(uname, Strings.Len(uname) - 5);
                            }
                            else if (Strings.Right(uname, 5) == "・前期型)")
                            {
                                uname = Strings.Left(uname, Strings.Len(uname) - 5) + ")";
                            }
                            else if (Strings.Right(uname, 5) == "(後期型)")
                            {
                                uname = Strings.Left(uname, Strings.Len(uname) - 5);
                            }

                            msg = "<B>" + uname + "</B>に" + buf + "。";
                        }

                        break;
                    }

                case "パーツ分離":
                    {
                        UnitData localItem8() { object argIndex1 = GeneralLib.LIndex(fdata, 2); var ret = SRC.UDList.Item(argIndex1); return ret; }

                        if ((u.Nickname ?? "") == (localItem8().Nickname ?? ""))
                        {
                            UnitData localItem6() { object argIndex1 = GeneralLib.LIndex(fdata, 2); var ret = SRC.UDList.Item(argIndex1); return ret; }

                            uname = localItem6().Name;
                        }
                        else
                        {
                            UnitData localItem7() { object argIndex1 = GeneralLib.LIndex(fdata, 2); var ret = SRC.UDList.Item(argIndex1); return ret; }

                            uname = localItem7().Nickname;
                        }

                        if (Strings.Right(uname, 5) == "(前期型)")
                        {
                            uname = Strings.Left(uname, Strings.Len(uname) - 5);
                        }
                        else if (Strings.Right(uname, 5) == "・前期型)")
                        {
                            uname = Strings.Left(uname, Strings.Len(uname) - 5) + ")";
                        }
                        else if (Strings.Right(uname, 5) == "(後期型)")
                        {
                            uname = Strings.Left(uname, Strings.Len(uname) - 5);
                        }

                        msg = "パーツを分離し" + uname + "に変形。";
                        if (flevel_specified)
                        {
                            msg = msg + ";ユニット破壊時に" + SrcFormatter.Format(10d * flevel) + "%の確率で発動。";
                        }

                        break;
                    }

                case "パーツ合体":
                    {
                        UnitData localItem11() { object argIndex1 = fdata; var ret = SRC.UDList.Item(argIndex1); return ret; }

                        if ((u.Nickname ?? "") == (localItem11().Nickname ?? ""))
                        {
                            UnitData localItem9() { object argIndex1 = fdata; var ret = SRC.UDList.Item(argIndex1); return ret; }

                            uname = localItem9().Name;
                        }
                        else
                        {
                            UnitData localItem10() { object argIndex1 = fdata; var ret = SRC.UDList.Item(argIndex1); return ret; }

                            uname = localItem10().Nickname;
                        }

                        if (Strings.Right(uname, 5) == "(前期型)")
                        {
                            uname = Strings.Left(uname, Strings.Len(uname) - 5);
                        }
                        else if (Strings.Right(uname, 5) == "・前期型)")
                        {
                            uname = Strings.Left(uname, Strings.Len(uname) - 5) + ")";
                        }
                        else if (Strings.Right(uname, 5) == "(後期型)")
                        {
                            uname = Strings.Left(uname, Strings.Len(uname) - 5);
                        }

                        msg = "パーツと合体し" + uname + "に変形。";
                        break;
                    }

                case "ハイパーモード":
                    {
                        UnitData localItem14() { object argIndex1 = GeneralLib.LIndex(fdata, 2); var ret = SRC.UDList.Item(argIndex1); return ret; }

                        if ((u.Nickname ?? "") == (localItem14().Nickname ?? ""))
                        {
                            UnitData localItem12() { object argIndex1 = GeneralLib.LIndex(fdata, 2); var ret = SRC.UDList.Item(argIndex1); return ret; }

                            uname = localItem12().Name;
                        }
                        else
                        {
                            UnitData localItem13() { object argIndex1 = GeneralLib.LIndex(fdata, 2); var ret = SRC.UDList.Item(argIndex1); return ret; }

                            uname = localItem13().Nickname;
                        }

                        if (Strings.Right(uname, 5) == "(前期型)")
                        {
                            uname = Strings.Left(uname, Strings.Len(uname) - 5);
                        }
                        else if (Strings.Right(uname, 5) == "・前期型)")
                        {
                            uname = Strings.Left(uname, Strings.Len(uname) - 5) + ")";
                        }
                        else if (Strings.Right(uname, 5) == "(後期型)")
                        {
                            uname = Strings.Left(uname, Strings.Len(uname) - 5);
                        }

                        if ((u.Nickname ?? "") != (uname ?? ""))
                        {
                            uname = "<B>" + uname + "</B>";
                        }
                        else
                        {
                            uname = "";
                        }

                        if (Strings.InStr(fdata, "気力発動") > 0)
                        {
                            msg = Expression.Term("気力", u) + SrcFormatter.Format(100d + 10d * flevel) + "で特殊形態" + uname + "に";
                        }
                        else if (flevel <= 5d)
                        {
                            msg = Expression.Term("気力", u) + SrcFormatter.Format(100d + 10d * flevel) + "、" + "もしくは" + Expression.Term("ＨＰ", u) + "が最大" + Expression.Term("ＨＰ", u) + "の1/4以下で特殊形態" + uname + "に";
                        }
                        else
                        {
                            msg = Expression.Term("ＨＰ", u) + "が最大" + Expression.Term("ＨＰ", u) + "の1/4以下で特殊形態" + uname + "に";
                        }

                        if (Strings.InStr(fdata, "自動発動") > 0)
                        {
                            msg = msg + "自動";
                        }

                        if (u.IsHero())
                        {
                            msg = msg + "変身。";
                        }
                        else
                        {
                            msg = msg + "変形。";
                        }

                        break;
                    }

                case "合体":
                    {
                        if (u.IsHero())
                        {
                            buf = "変化。";
                        }
                        else
                        {
                            buf = "変形。";
                        }

                        if (GeneralLib.LLength(fdata) > 3)
                        {
                            if (SRC.UDList.IsDefined(GeneralLib.LIndex(fdata, 2)))
                            {
                                UnitData localItem15() { object argIndex1 = GeneralLib.LIndex(fdata, 2); var ret = SRC.UDList.Item(argIndex1); return ret; }

                                msg = "以下のユニットと合体し<B>" + localItem15().Nickname + "</B>に" + buf + "; ";
                            }
                            else
                            {
                                msg = "以下のユニットと合体し<B>" + GeneralLib.LIndex(fdata, 2) + "</B>に" + buf + "; ";
                            }

                            var loopTo19 = GeneralLib.LLength(fdata);
                            for (i = 3; i <= loopTo19; i++)
                            {
                                if (SRC.UDList.IsDefined(GeneralLib.LIndex(fdata, i)))
                                {
                                    UnitData localItem16() { object argIndex1 = GeneralLib.LIndex(fdata, i); var ret = SRC.UDList.Item(argIndex1); return ret; }

                                    msg = msg + localItem16().Nickname + "  ";
                                }
                                else
                                {
                                    msg = msg + GeneralLib.LIndex(fdata, i) + "  ";
                                }
                            }
                        }
                        else
                        {
                            if (SRC.UDList.IsDefined(GeneralLib.LIndex(fdata, 3)))
                            {
                                UnitData localItem17() { object argIndex1 = GeneralLib.LIndex(fdata, 3); var ret = SRC.UDList.Item(argIndex1); return ret; }

                                msg = localItem17().Nickname + "と合体し";
                            }
                            else
                            {
                                msg = GeneralLib.LIndex(fdata, 3) + "と合体し";
                            }

                            if (SRC.UDList.IsDefined(GeneralLib.LIndex(fdata, 2)))
                            {
                                UnitData localItem18() { object argIndex1 = GeneralLib.LIndex(fdata, 2); var ret = SRC.UDList.Item(argIndex1); return ret; }

                                msg = msg + localItem18().Nickname + "に" + buf;
                            }
                            else
                            {
                                msg = msg + GeneralLib.LIndex(fdata, 2) + "に" + buf;
                            }
                        }

                        break;
                    }

                case "分離":
                    {
                        msg = "以下のユニットに分離。; ";
                        var loopTo20 = GeneralLib.LLength(fdata);
                        for (i = 2; i <= loopTo20; i++)
                        {
                            if (SRC.UDList.IsDefined(GeneralLib.LIndex(fdata, i)))
                            {
                                UnitData localItem19() { object argIndex1 = GeneralLib.LIndex(fdata, i); var ret = SRC.UDList.Item(argIndex1); return ret; }

                                msg = msg + localItem19().Nickname + "  ";
                            }
                            else
                            {
                                msg = msg + GeneralLib.LIndex(fdata, i) + "  ";
                            }
                        }

                        break;
                    }

                case "不安定":
                    {
                        msg = Expression.Term("ＨＰ", u) + "が最大値の1/4以下になると暴走する。";
                        break;
                    }

                case "支配":
                    {
                        if (GeneralLib.LLength(fdata) == 2)
                        {
                            bool localIsDefined() { object argIndex1 = GeneralLib.LIndex(fdata, 2); var ret = SRC.PDList.IsDefined(argIndex1); return ret; }

                            if (!localIsDefined())
                            {
                                GUI.ErrorMessage("支配対象のパイロット「" + GeneralLib.LIndex(fdata, 2) + "」のデータが定義されていません");
                                return FeatureHelpMessageRet;
                            }

                            PilotData localItem20() { object argIndex1 = GeneralLib.LIndex(fdata, 2); var ret = SRC.PDList.Item(argIndex1); return ret; }

                            msg = localItem20().Nickname + "の存在を維持し、仕えさせている。";
                        }
                        else
                        {
                            msg = "以下のユニットの存在を維持し、仕えさせている。;";
                            var loopTo21 = GeneralLib.LLength(fdata);
                            for (i = 2; i <= loopTo21; i++)
                            {
                                bool localIsDefined1() { object argIndex1 = GeneralLib.LIndex(fdata, 2); var ret = SRC.PDList.IsDefined(argIndex1); return ret; }

                                if (!localIsDefined1())
                                {
                                    GUI.ErrorMessage("支配対象のパイロット「" + GeneralLib.LIndex(fdata, i) + "」のデータが定義されていません");
                                    return FeatureHelpMessageRet;
                                }

                                PilotData localItem21() { object argIndex1 = GeneralLib.LIndex(fdata, i); var ret = SRC.PDList.Item(argIndex1); return ret; }

                                msg = msg + localItem21().Nickname + "  ";
                            }
                        }

                        break;
                    }

                case "ＥＣＭ":
                    {
                        msg = "半径３マス以内の味方ユニットに対する攻撃の命中率を元の";
                        if (flevel >= 0d)
                        {
                            msg = msg + SrcFormatter.Format(100d - 5d * flevel) + "%に減少させる。";
                        }
                        else
                        {
                            msg = msg + SrcFormatter.Format(100d - 5d * flevel) + "%に増加させる。";
                        }

                        buf = fname;
                        if (Conversions.ToBoolean(Strings.InStr(buf, "Lv")))
                        {
                            buf = Strings.Left(buf, Strings.InStr(buf, "Lv") - 1);
                        }

                        msg = msg + "同時に相手の" + buf + "能力の効果を無効化。";
                        msg = msg + ";思念誘導攻撃や近接攻撃には無効。";
                        break;
                    }

                case "ブースト":
                    {
                        if (Expression.IsOptionDefined("ダメージ倍率低下"))
                        {
                            msg = Expression.Term("気力", u) + "130以上で発動し、ダメージを 20% アップ。";
                        }
                        else
                        {
                            msg = Expression.Term("気力", u) + "130以上で発動し、ダメージを 25% アップ。";
                        }

                        break;
                    }

                case "防御不可":
                    {
                        msg = "攻撃を受けた際に防御運動を取ることが出来ない。";
                        break;
                    }

                case "回避不可":
                    {
                        msg = "攻撃を受けた際に回避運動を取ることが出来ない。";
                        break;
                    }

                case "格闘強化":
                    {
                        if (flevel >= 0d)
                        {
                            msg = "パイロットの" + Expression.Term("格闘", u) + "を+" + SrcFormatter.Format((int)(5d * flevel)) + "。";
                        }
                        else
                        {
                            msg = "パイロットの" + Expression.Term("格闘", u) + "を" + SrcFormatter.Format((int)(5d * flevel)) + "。";
                        }

                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 2) + "以上で発動。";
                        }

                        break;
                    }

                case "射撃強化":
                    {
                        if (p.HasMana())
                        {
                            if (flevel >= 0d)
                            {
                                msg = "パイロットの" + Expression.Term("魔力", u) + "を+" + SrcFormatter.Format((int)(5d * flevel)) + "。";
                            }
                            else
                            {
                                msg = "パイロットの" + Expression.Term("魔力", u) + "を" + SrcFormatter.Format((int)(5d * flevel)) + "。";
                            }
                        }
                        else if (flevel >= 0d)
                        {
                            msg = "パイロットの" + Expression.Term("射撃", u) + "を+" + SrcFormatter.Format((int)(5d * flevel)) + "。";
                        }
                        else
                        {
                            msg = "パイロットの" + Expression.Term("射撃", u) + "を" + SrcFormatter.Format((int)(5d * flevel)) + "。";
                        }

                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 2) + "以上で発動。";
                        }

                        break;
                    }

                case "命中強化":
                    {
                        if (flevel >= 0d)
                        {
                            msg = "パイロットの" + Expression.Term("命中", u) + "を+" + SrcFormatter.Format((int)(5d * flevel)) + "。";
                        }
                        else
                        {
                            msg = "パイロットの" + Expression.Term("命中", u) + "を" + SrcFormatter.Format((int)(5d * flevel)) + "。";
                        }

                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                        {
                            msg = msg + "気力" + GeneralLib.LIndex(fdata, 2) + "以上で発動。";
                        }

                        break;
                    }

                case "回避強化":
                    {
                        if (flevel >= 0d)
                        {
                            msg = "パイロットの" + Expression.Term("回避", u) + "を+" + SrcFormatter.Format((int)(5d * flevel)) + "。";
                        }
                        else
                        {
                            msg = "パイロットの" + Expression.Term("回避", u) + "を" + SrcFormatter.Format((int)(5d * flevel)) + "。";
                        }

                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 2) + "以上で発動。";
                        }

                        break;
                    }

                case "技量強化":
                    {
                        if (flevel >= 0d)
                        {
                            msg = "パイロットの" + Expression.Term("技量", u) + "を+" + SrcFormatter.Format((int)(5d * flevel)) + "。";
                        }
                        else
                        {
                            msg = "パイロットの" + Expression.Term("技量", u) + "を" + SrcFormatter.Format((int)(5d * flevel)) + "。";
                        }

                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 2) + "以上で発動。";
                        }

                        break;
                    }

                case "反応強化":
                    {
                        if (flevel >= 0d)
                        {
                            msg = "パイロットの" + Expression.Term("反応", u) + "を+" + SrcFormatter.Format((int)(5d * flevel)) + "。";
                        }
                        else
                        {
                            msg = "パイロットの" + Expression.Term("反応", u) + "を" + SrcFormatter.Format((int)(5d * flevel)) + "。";
                        }

                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 2) + "以上で発動。";
                        }

                        break;
                    }

                case "ＨＰ強化":
                    {
                        if (flevel >= 0d)
                        {
                            msg = "最大" + Expression.Term("ＨＰ", u) + "を" + SrcFormatter.Format((int)(200d * flevel)) + "増加。";
                        }
                        else
                        {
                            msg = "最大" + Expression.Term("ＨＰ", u) + "を" + SrcFormatter.Format((int)(-200 * flevel)) + "減少。";
                        }

                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 2) + "以上で発動。";
                        }

                        break;
                    }

                case "ＥＮ強化":
                    {
                        if (flevel >= 0d)
                        {
                            msg = "最大" + Expression.Term("ＥＮ", u) + "を" + SrcFormatter.Format((int)(10d * flevel)) + "増加。";
                        }
                        else
                        {
                            msg = "最大" + Expression.Term("ＥＮ", u) + "を" + SrcFormatter.Format((int)(-10 * flevel)) + "減少。";
                        }

                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 2) + "以上で発動。";
                        }

                        break;
                    }

                case "装甲強化":
                    {
                        if (flevel >= 0d)
                        {
                            msg = Expression.Term("装甲", u) + "を" + SrcFormatter.Format((int)(100d * flevel)) + "増加。";
                        }
                        else
                        {
                            msg = Expression.Term("装甲", u) + "を" + SrcFormatter.Format((int)(-100 * flevel)) + "減少。";
                        }

                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 2) + "以上で発動。";
                        }

                        break;
                    }

                case "運動性強化":
                    {
                        if (flevel >= 0d)
                        {
                            msg = Expression.Term("運動性", u) + "を" + SrcFormatter.Format((int)(5d * flevel)) + "増加。";
                        }
                        else
                        {
                            msg = Expression.Term("運動性", u) + "を" + SrcFormatter.Format((int)(-5 * flevel)) + "減少。";
                        }

                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 2) + "以上で発動。";
                        }

                        break;
                    }

                case "移動力強化":
                    {
                        if (flevel >= 0d)
                        {
                            msg = Expression.Term("移動力", u) + "を" + SrcFormatter.Format((int)flevel) + "増加。";
                        }
                        else
                        {
                            msg = Expression.Term("移動力", u) + "を" + SrcFormatter.Format((int)flevel) + "減少。";
                        }

                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 2) + "以上で発動。";
                        }

                        break;
                    }

                case "ＨＰ割合強化":
                    {
                        if (flevel >= 0d)
                        {
                            msg = "最大" + Expression.Term("ＨＰ", u) + "を" + SrcFormatter.Format((int)(5d * flevel)) + "%分増加。";
                        }
                        else
                        {
                            msg = "最大" + Expression.Term("ＨＰ", u) + "を" + SrcFormatter.Format((int)(-5 * flevel)) + "%分減少。";
                        }

                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 2) + "以上で発動。";
                        }

                        break;
                    }

                case "ＥＮ割合強化":
                    {
                        if (flevel >= 0d)
                        {
                            msg = "最大" + Expression.Term("ＥＮ", u) + "を" + SrcFormatter.Format((int)(5d * flevel)) + "%分増加。";
                        }
                        else
                        {
                            msg = "最大" + Expression.Term("ＥＮ", u) + "を" + SrcFormatter.Format((int)(-5 * flevel)) + "%分減少。";
                        }

                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 2) + "以上で発動。";
                        }

                        break;
                    }

                case "装甲割合強化":
                    {
                        if (flevel >= 0d)
                        {
                            msg = Expression.Term("装甲", u) + "を" + SrcFormatter.Format((int)(5d * flevel)) + "%分増加。";
                        }
                        else
                        {
                            msg = Expression.Term("装甲", u) + "を" + SrcFormatter.Format((int)(-5 * flevel)) + "%分減少。";
                        }

                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 2) + "以上で発動。";
                        }

                        break;
                    }

                case "運動性割合強化":
                    {
                        if (flevel >= 0d)
                        {
                            msg = Expression.Term("運動性", u) + "を" + SrcFormatter.Format((int)(5d * flevel)) + "%分増加。";
                        }
                        else
                        {
                            msg = Expression.Term("運動性", u) + "を" + SrcFormatter.Format((int)(-5 * flevel)) + "%分減少。";
                        }

                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 2) + "以上で発動。";
                        }

                        break;
                    }

                case "武器・防具クラス":
                    {
                        fdata = Strings.Trim(u.WeaponProficiency());
                        if (!string.IsNullOrEmpty(fdata))
                        {
                            msg = "武器【" + fdata + "】;";
                        }
                        else
                        {
                            msg = "武器【-】;";
                        }

                        fdata = Strings.Trim(u.ArmorProficiency());
                        if (!string.IsNullOrEmpty(fdata))
                        {
                            msg = msg + "防具【" + fdata + "】";
                        }
                        else
                        {
                            msg = msg + "防具【-】";
                        }

                        break;
                    }

                case "追加攻撃":
                    {
                        if (GeneralLib.LIndex(fdata, 3) != "全")
                        {
                            buf = GeneralLib.LIndex(fdata, 3);
                            if (Strings.Left(buf, 1) == "@")
                            {
                                msg = Strings.Mid(buf, 2) + "による";
                            }
                            else
                            {
                                msg = "「" + buf + "」属性を持つ武器による";
                            }
                        }

                        msg = msg + "攻撃の後に、";
                        buf = GeneralLib.LIndex(fdata, 4);
                        if (Information.IsNumeric(buf))
                        {
                            if (buf != "100")
                            {
                                msg = msg + buf + "%の確率で";
                            }
                        }
                        else if (Strings.InStr(buf, "+") > 0 | Strings.InStr(buf, "-") > 0)
                        {
                            i = (int)GeneralLib.MaxLng(Strings.InStr(buf, "+"), Strings.InStr(buf, "-"));
                            sname = u.SkillName0(Strings.Left(buf, i - 1));
                            prob = (int)((long)((u.SkillLevel(Strings.Left(buf, i - 1)) + Conversions.ToInteger(Strings.Mid(buf, i))) * 100d) / 16L);
                            msg = msg + "(" + sname + "Lv" + Strings.Mid(buf, i) + ")/16の確率(" + SrcFormatter.Format(prob) + "%)で";
                        }
                        else
                        {
                            sname = u.SkillName0(buf);
                            prob = (int)((long)(u.SkillLevel(buf) * 100d) / 16L);
                            msg = msg + sname + "Lv/16の確率(" + SrcFormatter.Format(prob) + "%)で";
                        }

                        buf = GeneralLib.LIndex(fdata, 2);
                        if (Strings.InStr(buf, "(") > 0)
                        {
                            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
                        }

                        msg = msg + buf + "による追撃を行う。";
                        int localStrToLng18() { string argexpr = GeneralLib.LIndex(fdata, 5); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 5)) > 0)
                        {
                            msg = msg + ";発動時に" + GeneralLib.LIndex(fdata, 5) + "ＥＮ消費。";
                        }
                        else if (localStrToLng18() < 0)
                        {
                            msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(fdata, 5), 2) + "ＥＮ増加。";
                        }

                        if (GeneralLib.StrToLng(GeneralLib.LIndex(fdata, 6)) > 50)
                        {
                            msg = msg + ";" + Expression.Term("気力", u) + GeneralLib.LIndex(fdata, 6) + "以上で使用可能。";
                        }

                        if (Strings.InStr(fdata, "連鎖不可") > 0)
                        {
                            msg = msg + "連鎖不可。";
                        }

                        break;
                    }

                case "ＺＯＣ":
                    {
                        if (u.FeatureLevel("ＺＯＣ") < 0d)
                        {
                            msg = "このユニットはＺＯＣによる影響を与えることが出来ない。";
                        }
                        else
                        {
                            msg = "このユニットから";
                            if (GeneralLib.LLength(fdata) < 2)
                            {
                                buf = "1";
                            }
                            else
                            {
                                buf = GeneralLib.LIndex(fdata, 2);
                            }

                            opt = GeneralLib.LIndex(fdata, 3);
                            if (Strings.InStr(opt, "直線") > 0)
                            {
                                msg = msg + buf + "マス以内の直線上";
                            }
                            else if (Strings.InStr(opt, " 水平") > 0)
                            {
                                msg = msg + "左右" + buf + "マス以内の直線上";
                            }
                            else if (Strings.InStr(opt, " 垂直") > 0)
                            {
                                msg = msg + "上下" + buf + "マス以内の直線上";
                            }
                            else
                            {
                                msg = msg + buf + "マス以内";
                            }

                            msg = msg + "を通過する敵ユニットに、ＺＯＣによる影響を与える。";
                        }

                        break;
                    }

                case "ＺＯＣ無効化":
                    {
                        if (flevel == 1d)
                        {
                            msg = "このユニットは敵ユニットによるＺＯＣの影響を受けない。";
                        }
                        else
                        {
                            msg = "このユニットは敵ユニットによる" + SrcFormatter.Format(flevel) + "レベル以下のＺＯＣの影響を受けない。";
                        }

                        break;
                    }

                case "隣接ユニットＺＯＣ無効化":
                    {
                        if (flevel == 1d)
                        {
                            msg = "このユニットが隣接する敵ユニットによるＺＯＣを無効化する。";
                        }
                        else
                        {
                            msg = "このユニットが隣接する敵ユニットによる" + SrcFormatter.Format(flevel) + "レベル以下のＺＯＣを無効化する。";
                        }

                        break;
                    }

                case "広域ＺＯＣ無効化":
                    {
                        msg = "このユニットから";
                        if (GeneralLib.LLength(fdata) < 2)
                        {
                            buf = "1";
                        }
                        else
                        {
                            buf = GeneralLib.LIndex(fdata, 2);
                        }

                        if (flevel == 1d)
                        {
                            msg = msg + buf + "マス以内に設定されたＺＯＣの影響を無効化する。";
                        }
                        else
                        {
                            msg = msg + buf + "マス以内に設定された" + SrcFormatter.Format(flevel) + "レベル以下のＺＯＣの影響を無効化する。";
                        }

                        break;
                    }

                // ADD START MARGE
                case "地形効果無効化":
                    {
                        if (GeneralLib.LLength(fdata) > 1)
                        {
                            var loopTo22 = GeneralLib.LLength(fdata);
                            for (i = 2; i <= loopTo22; i++)
                            {
                                if (i > 2)
                                {
                                    msg = msg + "、";
                                }

                                msg = msg + GeneralLib.LIndex(fdata, i);
                            }

                            msg = msg + "の";
                        }
                        else
                        {
                            msg = msg + "全地形の";
                        }
                        // ADD END MARGE

                        msg = msg + "ＨＰ・ＥＮ減少や状態付加等の特殊効果を無効化する。";
                        break;
                    }

                default:
                    {
                        string localAllFeatureData1() { object argIndex1 = fname; var ret = u.AllFeatureData(argIndex1); return ret; }

                        string localListIndex() { string arglist = hs055a0679e2434182a961742a50d72158(); var ret = GeneralLib.ListIndex(arglist, 1); return ret; }

                        if (is_additional)
                        {
                            // 付加された能力の場合、ユニット用特殊能力に該当しなければ
                            // パイロット用特殊能力とみなす
                            msg = SkillHelpMessage(u.MainPilot(), ftype);
                            if (Strings.Len(msg) > 0)
                            {
                                return FeatureHelpMessageRet;
                            }

                            // 実はダミー能力？
                            if (Strings.Len(fdata) > 0)
                            {
                                msg = GeneralLib.ListIndex(fdata, GeneralLib.ListLength(fdata));
                                if (Strings.Left(msg, 1) == "\"")
                                {
                                    msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
                                }
                            }

                            // 解説が存在しない？
                            if (Strings.Len(msg) == 0)
                            {
                                return FeatureHelpMessageRet;
                            }
                        }
                        else if (Strings.Len(fdata) > 0)
                        {
                            // ダミー能力の場合
                            msg = GeneralLib.ListIndex(fdata, GeneralLib.ListLength(fdata));
                            if (Strings.Left(msg, 1) == "\"")
                            {
                                msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
                            }
                        }
                        else if (localListIndex() != "解説")
                        {
                            // 解説がない場合
                            return FeatureHelpMessageRet;
                        }

                        break;
                    }
            }

            fdata = u.AllFeatureData(fname0);
            if (GeneralLib.ListIndex(fdata, 1) == "解説")
            {
                // 解説を定義している場合
                msg = GeneralLib.ListTail(fdata, 2);
                if (Strings.Left(msg, 1) == "\"")
                {
                    msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
                }
            }

            // 等身大基準の際は「パイロット」という語を使わないようにする
            if (Expression.IsOptionDefined("等身大基準"))
            {
                msg = msg.Replace("パイロット", "ユニット");
            }

            FeatureHelpMessageRet = msg;
            return FeatureHelpMessageRet;
        }

        // ユニット u の武器＆アビリティ属性 atr の名称
        public string AttributeName(Unit u, string atr, bool is_ability = false)
        {
            string AttributeNameRet = default;
            string fdata;
            switch (atr ?? "")
            {
                case "全":
                    {
                        AttributeNameRet = "全ての攻撃";
                        break;
                    }

                case "格":
                    {
                        AttributeNameRet = "格闘系攻撃";
                        break;
                    }

                case "射":
                    {
                        AttributeNameRet = "射撃系攻撃";
                        break;
                    }

                case "複":
                    {
                        AttributeNameRet = "複合技";
                        break;
                    }

                case "Ｐ":
                    {
                        AttributeNameRet = "移動後使用可能攻撃";
                        break;
                    }

                case "Ｑ":
                    {
                        AttributeNameRet = "移動後使用不能攻撃";
                        break;
                    }

                case "Ｒ":
                    {
                        AttributeNameRet = "低改造武器";
                        break;
                    }

                case "改":
                    {
                        AttributeNameRet = "低改造武器";
                        break;
                    }

                case "攻":
                    {
                        AttributeNameRet = "攻撃専用";
                        break;
                    }

                case "反":
                    {
                        AttributeNameRet = "反撃専用";
                        break;
                    }

                case "武":
                    {
                        AttributeNameRet = "格闘武器";
                        break;
                    }

                case "突":
                    {
                        AttributeNameRet = "突進技";
                        break;
                    }

                case "接":
                    {
                        AttributeNameRet = "接近戦攻撃";
                        break;
                    }

                case "Ｊ":
                    {
                        AttributeNameRet = "ジャンプ攻撃";
                        break;
                    }

                case "Ｂ":
                    {
                        AttributeNameRet = "ビーム攻撃";
                        break;
                    }

                case "実":
                    {
                        AttributeNameRet = "実弾攻撃";
                        break;
                    }

                case "オ":
                    {
                        AttributeNameRet = "オーラ技";
                        break;
                    }

                case "超":
                    {
                        AttributeNameRet = "サイキック攻撃";
                        break;
                    }

                case "シ":
                    {
                        AttributeNameRet = "同調率対象攻撃";
                        break;
                    }

                case "サ":
                    {
                        AttributeNameRet = "思念誘導攻撃";
                        break;
                    }

                case "体":
                    {
                        AttributeNameRet = "生命力換算攻撃";
                        break;
                    }

                case "吸":
                    {
                        AttributeNameRet = Expression.Term("ＨＰ", u) + "吸収攻撃";
                        break;
                    }

                case "減":
                    {
                        AttributeNameRet = Expression.Term("ＥＮ", u) + "破壊攻撃";
                        break;
                    }

                case "奪":
                    {
                        AttributeNameRet = Expression.Term("ＥＮ", u) + "吸収攻撃";
                        break;
                    }

                case "貫":
                    {
                        AttributeNameRet = "貫通攻撃";
                        break;
                    }

                case "無":
                    {
                        AttributeNameRet = "バリア無効化攻撃";
                        break;
                    }

                case "浄":
                    {
                        AttributeNameRet = "浄化技";
                        break;
                    }

                case "封":
                    {
                        AttributeNameRet = "封印技";
                        break;
                    }

                case "限":
                    {
                        AttributeNameRet = "限定技";
                        break;
                    }

                case "殺":
                    {
                        AttributeNameRet = "抹殺攻撃";
                        break;
                    }

                case "浸":
                    {
                        AttributeNameRet = "浸蝕攻撃";
                        break;
                    }

                case "破":
                    {
                        AttributeNameRet = "シールド貫通攻撃";
                        break;
                    }

                case "♂":
                    {
                        AttributeNameRet = "対男性用攻撃";
                        break;
                    }

                case "♀":
                    {
                        AttributeNameRet = "対女性用攻撃";
                        break;
                    }

                case "Ａ":
                    {
                        AttributeNameRet = "自動充填式攻撃";
                        break;
                    }

                case "Ｃ":
                    {
                        AttributeNameRet = "チャージ式攻撃";
                        break;
                    }

                case "合":
                    {
                        AttributeNameRet = "合体技";
                        break;
                    }

                case "共":
                    {
                        if (!is_ability)
                        {
                            AttributeNameRet = "弾薬共有武器";
                        }
                        else
                        {
                            AttributeNameRet = "使用回数共有" + Expression.Term("アビリティ", u);
                        }

                        break;
                    }

                case "斉":
                    {
                        AttributeNameRet = "一斉発射";
                        break;
                    }

                case "永":
                    {
                        AttributeNameRet = "永続武器";
                        break;
                    }

                case "術":
                    {
                        AttributeNameRet = "術";
                        break;
                    }

                case "技":
                    {
                        AttributeNameRet = "技";
                        break;
                    }

                case "視":
                    {
                        AttributeNameRet = "視覚攻撃";
                        break;
                    }

                case "音":
                    {
                        if (!is_ability)
                        {
                            AttributeNameRet = "音波攻撃";
                        }
                        else
                        {
                            AttributeNameRet = "音波" + Expression.Term("アビリティ", u);
                        }

                        break;
                    }

                case "気":
                    {
                        AttributeNameRet = Expression.Term("気力", u) + "消費攻撃";
                        break;
                    }

                case "霊":
                case "プ":
                    {
                        AttributeNameRet = "霊力消費攻撃";
                        break;
                    }

                case "失":
                    {
                        AttributeNameRet = Expression.Term("ＨＰ", u) + "消費攻撃";
                        break;
                    }

                case "銭":
                    {
                        AttributeNameRet = Expression.Term("資金", u) + "消費攻撃";
                        break;
                    }

                case "消":
                    {
                        AttributeNameRet = "消耗技";
                        break;
                    }

                case "自":
                    {
                        AttributeNameRet = "自爆攻撃";
                        break;
                    }

                case "変":
                    {
                        AttributeNameRet = "変形技";
                        break;
                    }

                case "間":
                    {
                        AttributeNameRet = "間接攻撃";
                        break;
                    }

                case "Ｍ直":
                    {
                        AttributeNameRet = "直線型マップ攻撃";
                        break;
                    }

                case "Ｍ拡":
                    {
                        AttributeNameRet = "拡散型マップ攻撃";
                        break;
                    }

                case "Ｍ扇":
                    {
                        AttributeNameRet = "扇型マップ攻撃";
                        break;
                    }

                case "Ｍ全":
                    {
                        AttributeNameRet = "全方位型マップ攻撃";
                        break;
                    }

                case "Ｍ投":
                    {
                        AttributeNameRet = "投下型マップ攻撃";
                        break;
                    }

                case "Ｍ移":
                    {
                        AttributeNameRet = "移動型マップ攻撃";
                        break;
                    }

                case "Ｍ線":
                    {
                        AttributeNameRet = "線状マップ攻撃";
                        break;
                    }

                case "識":
                    {
                        AttributeNameRet = "識別型マップ攻撃";
                        break;
                    }

                case "縛":
                    {
                        AttributeNameRet = "捕縛攻撃";
                        break;
                    }

                case "Ｓ":
                    {
                        AttributeNameRet = "ショック攻撃";
                        break;
                    }

                case "劣":
                    {
                        AttributeNameRet = "装甲劣化攻撃";
                        break;
                    }

                case "中":
                    {
                        AttributeNameRet = "バリア中和攻撃";
                        break;
                    }

                case "石":
                    {
                        AttributeNameRet = "石化攻撃";
                        break;
                    }

                case "凍":
                    {
                        AttributeNameRet = "凍結攻撃";
                        break;
                    }

                case "痺":
                    {
                        AttributeNameRet = "麻痺攻撃";
                        break;
                    }

                case "眠":
                    {
                        AttributeNameRet = "催眠攻撃";
                        break;
                    }

                case "乱":
                    {
                        AttributeNameRet = "混乱攻撃";
                        break;
                    }

                case "魅":
                    {
                        AttributeNameRet = "魅了攻撃";
                        break;
                    }

                case "憑":
                    {
                        AttributeNameRet = "憑依攻撃";
                        break;
                    }

                case "盲":
                    {
                        AttributeNameRet = "目潰し攻撃";
                        break;
                    }

                case "毒":
                    {
                        AttributeNameRet = "毒攻撃";
                        break;
                    }

                case "撹":
                    {
                        AttributeNameRet = "撹乱攻撃";
                        break;
                    }

                case "恐":
                    {
                        AttributeNameRet = "恐怖攻撃";
                        break;
                    }

                case "不":
                    {
                        AttributeNameRet = "攻撃封印攻撃";
                        break;
                    }

                case "止":
                    {
                        AttributeNameRet = "足止め攻撃";
                        break;
                    }

                case "黙":
                    {
                        AttributeNameRet = "沈黙攻撃";
                        break;
                    }

                case "除":
                    {
                        AttributeNameRet = "特殊効果除去攻撃";
                        break;
                    }

                case "即":
                    {
                        AttributeNameRet = "即死攻撃";
                        break;
                    }

                case "告":
                    {
                        AttributeNameRet = "死の宣告";
                        break;
                    }

                case "脱":
                    {
                        AttributeNameRet = Expression.Term("気力", u) + "減少攻撃";
                        break;
                    }

                case "Ｄ":
                    {
                        AttributeNameRet = Expression.Term("気力", u) + "吸収攻撃";
                        break;
                    }

                case "低攻":
                    {
                        AttributeNameRet = "攻撃力低下攻撃";
                        break;
                    }

                case "低防":
                    {
                        AttributeNameRet = "防御力低下攻撃";
                        break;
                    }

                case "低運":
                    {
                        AttributeNameRet = Expression.Term("運動性", u) + "低下攻撃";
                        break;
                    }

                case "低移":
                    {
                        AttributeNameRet = Expression.Term("移動力", u) + "低下攻撃";
                        break;
                    }

                case "精":
                    {
                        AttributeNameRet = "精神攻撃";
                        break;
                    }

                case "先":
                    {
                        AttributeNameRet = "先制攻撃";
                        break;
                    }

                case "後":
                    {
                        AttributeNameRet = "後攻攻撃";
                        break;
                    }

                case "連":
                    {
                        AttributeNameRet = "連続攻撃";
                        break;
                    }

                case "再":
                    {
                        AttributeNameRet = "再攻撃";
                        break;
                    }

                case "吹":
                    {
                        AttributeNameRet = "吹き飛ばし攻撃";
                        break;
                    }

                case "Ｋ":
                    {
                        AttributeNameRet = "ノックバック攻撃";
                        break;
                    }

                case "引":
                    {
                        AttributeNameRet = "引き寄せ攻撃";
                        break;
                    }

                case "転":
                    {
                        AttributeNameRet = "強制転移攻撃";
                        break;
                    }

                case "忍":
                    {
                        AttributeNameRet = "暗殺技";
                        break;
                    }

                case "尽":
                    {
                        AttributeNameRet = "全" + Expression.Term("ＥＮ", u) + "消費攻撃";
                        break;
                    }

                case "盗":
                    {
                        AttributeNameRet = "盗み";
                        break;
                    }

                case "Ｈ":
                    {
                        AttributeNameRet = "ホーミング攻撃";
                        break;
                    }

                case "追":
                    {
                        AttributeNameRet = "自己追尾攻撃";
                        break;
                    }

                case "有":
                    {
                        AttributeNameRet = "有線式誘導攻撃";
                        break;
                    }

                case "誘":
                    {
                        AttributeNameRet = "特殊誘導攻撃";
                        break;
                    }

                case "爆":
                    {
                        AttributeNameRet = "爆発攻撃";
                        break;
                    }

                case "空":
                    {
                        AttributeNameRet = "対空攻撃";
                        break;
                    }

                case "固":
                    {
                        AttributeNameRet = "ダメージ固定攻撃";
                        break;
                    }

                case "衰":
                    {
                        AttributeNameRet = Expression.Term("ＨＰ", u) + "減衰攻撃";
                        break;
                    }

                case "滅":
                    {
                        AttributeNameRet = Expression.Term("ＥＮ", u) + "減衰攻撃";
                        break;
                    }

                case "踊":
                    {
                        AttributeNameRet = "踊らせ攻撃";
                        break;
                    }

                case "狂":
                    {
                        AttributeNameRet = "狂戦士化攻撃";
                        break;
                    }

                case "ゾ":
                    {
                        AttributeNameRet = "ゾンビ化攻撃";
                        break;
                    }

                case "害":
                    {
                        AttributeNameRet = "回復能力阻害攻撃";
                        break;
                    }

                case "習":
                    {
                        AttributeNameRet = "ラーニング";
                        break;
                    }

                case "写":
                    {
                        AttributeNameRet = "能力コピー";
                        break;
                    }

                case "化":
                    {
                        AttributeNameRet = "変化";
                        break;
                    }

                case "痛":
                    {
                        AttributeNameRet = "クリティカル";
                        break;
                    }

                case "援":
                    {
                        AttributeNameRet = "支援専用" + Expression.Term("アビリティ", u);
                        break;
                    }

                case "難":
                    {
                        AttributeNameRet = "高難度" + Expression.Term("アビリティ", u);
                        break;
                    }

                case "地":
                case "水":
                case "火":
                case "風":
                case "冷":
                case "雷":
                case "光":
                case "闇":
                case "聖":
                case "死":
                case "木":
                    {
                        AttributeNameRet = atr + "属性";
                        break;
                    }

                case "魔":
                    {
                        AttributeNameRet = "魔法攻撃";
                        break;
                    }

                case "時":
                    {
                        AttributeNameRet = "時間操作攻撃";
                        break;
                    }

                case "重":
                    {
                        AttributeNameRet = "重力攻撃";
                        break;
                    }

                case "銃":
                case "剣":
                case "刀":
                case "槍":
                case "斧":
                case "弓":
                    {
                        AttributeNameRet = atr + "攻撃";
                        break;
                    }

                case var @case when @case == "銃":
                    {
                        break;
                    }

                case "機":
                    {
                        AttributeNameRet = "対機械用攻撃";
                        break;
                    }

                case "感":
                    {
                        AttributeNameRet = "対エスパー用攻撃";
                        break;
                    }

                case "竜":
                    {
                        AttributeNameRet = "竜殺しの武器";
                        break;
                    }

                case "瀕":
                    {
                        AttributeNameRet = "瀕死時限定攻撃";
                        break;
                    }

                case "対":
                    {
                        AttributeNameRet = "特定レベル限定攻撃";
                        break;
                    }

                case "ラ":
                    {
                        AttributeNameRet = "ラーニング可能技";
                        break;
                    }

                case "禁":
                    {
                        AttributeNameRet = "使用禁止";
                        break;
                    }

                case "小":
                    {
                        AttributeNameRet = "最小射程";
                        break;
                    }

                case "散":
                    {
                        AttributeNameRet = "拡散攻撃";
                        break;
                    }

                default:
                    {
                        if (Strings.Left(atr, 1) == "弱")
                        {
                            AttributeNameRet = Strings.Mid(atr, 2) + "属性弱点付加攻撃";
                        }
                        else if (Strings.Left(atr, 1) == "効")
                        {
                            AttributeNameRet = Strings.Mid(atr, 2) + "属性有効付加攻撃";
                        }
                        else if (Strings.Left(atr, 1) == "剋")
                        {
                            AttributeNameRet = Strings.Mid(atr, 2) + "属性使用妨害攻撃";
                        }

                        break;
                    }
            }

            if (u is object)
            {
                fdata = u.FeatureData(atr);
                if (GeneralLib.ListIndex(fdata, 1) == "解説")
                {
                    // 解説を定義している場合
                    AttributeNameRet = GeneralLib.ListIndex(fdata, 2);
                    return AttributeNameRet;
                }
            }

            if (is_ability)
            {
                if (Strings.Right(AttributeNameRet, 2) == "攻撃" | Strings.Right(AttributeNameRet, 2) == "武器")
                {
                    AttributeNameRet = Strings.Left(AttributeNameRet, Strings.Len(AttributeNameRet) - 2) + Expression.Term("アビリティ", u);
                }
            }

            return AttributeNameRet;
        }

        // ユニット u の idx 番目の武器＆アビリティの属性 atr の解説を表示
        public void AttributeHelp(Unit u, string atr, int idx, bool is_ability = false)
        {
            string msg, aname;
            bool prev_mode;
            msg = AttributeHelpMessage(u, atr, idx, is_ability);

            // 解説の表示
            if (Strings.Len(msg) > 0)
            {
                prev_mode = GUI.AutoMessageMode;
                GUI.AutoMessageMode = false;
                GUI.OpenMessageForm(u1: null, u2: null);
                if (SRC.AutoMoveCursor)
                {
                    GUI.MoveCursorPos("メッセージウィンドウ");
                }

                if (Strings.InStr(atr, "L") > 0)
                {
                    string localAttributeName() { string argatr = Strings.Left(atr, Strings.InStr(atr, "L") - 1); var ret = AttributeName(u, argatr, is_ability); return ret; }

                    aname = localAttributeName() + "レベル" + Strings.StrConv(SrcFormatter.Format(Strings.Mid(atr, Strings.InStr(atr, "L") + 1)), VbStrConv.Wide);
                }
                else
                {
                    aname = AttributeName(u, atr, is_ability);
                }

                GUI.DisplayMessage("システム", "<b>" + aname + "</b>;" + msg);
                GUI.CloseMessageForm();
                GUI.AutoMessageMode = prev_mode;
            }
        }

        // ユニット u の idx 番目の武器＆アビリティの属性 atr の解説を表示
        public string AttributeHelpMessage(Unit u, string atr, int idx, bool is_ability)
        {
            string AttributeHelpMessageRet = default;
            string atype;
            double alevel;
            string msg = default, whatsthis;
            string wanickname, waname, uname = default;
            Pilot p;
            int i, j;
            string buf;
            string fdata;

            // 属性レベルの収得
            if (Strings.InStr(atr, "L") > 0)
            {
                atype = Strings.Left(atr, Strings.InStr(atr, "L") - 1);
                alevel = Conversions.ToDouble(Strings.Mid(atr, Strings.InStr(atr, "L") + 1));
            }
            else
            {
                atype = atr;
                alevel = Constants.DEFAULT_LEVEL;
            }

            {
                var withBlock = u;
                // 武器(アビリティ)名
                if (!is_ability)
                {
                    waname = withBlock.Weapon(idx).Name;
                    wanickname = withBlock.WeaponNickname(idx);
                    whatsthis = "攻撃";
                }
                else
                {
                    waname = withBlock.Ability(idx).Name;
                    wanickname = withBlock.AbilityNickname(idx);
                    whatsthis = Expression.Term("アビリティ", u);
                }

                // メインパイロット
                p = withBlock.MainPilot();
            }

            switch (atype ?? "")
            {
                case "格":
                    {
                        msg = "パイロットの" + Expression.Term("格闘", u) + "を使って攻撃力を算出。";
                        break;
                    }

                case "射":
                    {
                        if (p.HasMana())
                        {
                            msg = "パイロットの" + Expression.Term("魔力", u) + "を使って攻撃力を算出。";
                        }
                        else
                        {
                            msg = "パイロットの" + Expression.Term("射撃", u) + "を使って攻撃力を算出。";
                        }

                        break;
                    }

                case "複":
                    {
                        if (p.HasMana())
                        {
                            msg = "格闘と魔法の両方を使った攻撃。" + "パイロットの" + Expression.Term("格闘", u) + "と" + Expression.Term("魔力", u) + "の" + "平均値を使って攻撃力を算出する。";
                        }
                        else
                        {
                            msg = "格闘と射撃の両方を使った攻撃。" + "パイロットの" + Expression.Term("格闘", u) + "と" + Expression.Term("射撃", u) + "の" + "平均値を使って攻撃力を算出する。";
                        }

                        break;
                    }

                case "Ｐ":
                    {
                        msg = "射程にかかわらず移動後に使用可能。";
                        break;
                    }

                case "Ｑ":
                    {
                        msg = "射程にかかわらず移動後は使用不能。";
                        break;
                    }

                case "攻":
                    {
                        msg = "攻撃時にのみ使用可能。";
                        break;
                    }

                case "反":
                    {
                        msg = "反撃時にのみ使用可能。";
                        break;
                    }

                case "Ｒ":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            msg = "ユニットランクや特殊能力による攻撃力上昇が通常の半分。";
                        }
                        else
                        {
                            msg = "ユニットランクや特殊能力による攻撃力上昇が" + SrcFormatter.Format(10d * alevel) + "％になる。";
                        }

                        msg = "ユニットランクや特殊能力による攻撃力上昇が通常の半分。";
                        break;
                    }

                case "改":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            msg = "ユニットランクによる攻撃力上昇が通常の半分。";
                        }
                        else
                        {
                            msg = "ユニットランクによる攻撃力上昇が" + SrcFormatter.Format(10d * alevel) + "％になる。";
                        }

                        break;
                    }

                case "武":
                    {
                        msg = "この武器を使って実弾攻撃などを切り払うことが可能。" + "切り払いの対象になる。";
                        break;
                    }

                case "突":
                    {
                        msg = "切り払いの対象になる。";
                        break;
                    }

                case "接":
                    {
                        msg = "投げ技等、相手に密着して繰り出す格闘戦攻撃。;" + "切り払い無効。";
                        break;
                    }

                case "Ｊ":
                    {
                        msg = "ジャンプ攻撃時の地形適応を指定したレベルだけ上げる。";
                        break;
                    }

                case "Ｂ":
                    {
                        msg = "対ビーム用防御能力の対象になる。";
                        break;
                    }

                case "実":
                    {
                        msg = "切り払いと迎撃の対象になる。";
                        if (Expression.IsOptionDefined("距離修正"))
                        {
                            msg = msg + "長距離の敵を攻撃する際もダメージが低下しない。";
                        }

                        break;
                    }

                case "オ":
                    {
                        msg = "パイロットの" + p.SkillName0("オーラ") + "レベルによって攻撃力が変化。";
                        break;
                    }

                case "超":
                    {
                        msg = "パイロットの" + p.SkillName0("超能力") + "レベルによって攻撃力が変化。";
                        break;
                    }

                case "シ":
                    {
                        msg = "パイロットの" + p.SkillName0("同調率") + "によって攻撃力が変化。";
                        break;
                    }

                case "サ":
                    {
                        msg = "パイロットの" + p.SkillName0("超感覚") + "レベルによって射程が変化。";
                        if (Expression.IsOptionDefined("距離修正"))
                        {
                            msg = msg + "距離による命中率低下がない。また、";
                        }

                        msg = msg + "ＥＣＭによる影響を受けない。";
                        break;
                    }

                case "体":
                    {
                        msg = "生命力を攻撃力に換える攻撃。ユニットの" + Expression.Term("ＨＰ", u) + "によって攻撃力が変化する。";
                        break;
                    }

                case "吸":
                    {
                        msg = "与えたダメージの１／４を吸収し、自分の" + Expression.Term("ＨＰ", u) + "に変換。";
                        break;
                    }

                case "減":
                    {
                        msg = Expression.Term("ＨＰ", u) + "にダメージを与えると同時に相手の" + Expression.Term("ＥＮ", u) + "を減少させる。";
                        break;
                    }

                case "奪":
                    {
                        msg = Expression.Term("ＨＰ", u) + "にダメージを与えると同時に相手の" + Expression.Term("ＥＮ", u) + "を減少させ、" + "減少させた" + Expression.Term("ＥＮ", u) + "の半分を自分のものにする。";
                        break;
                    }

                case "貫":
                    {
                        if (alevel > 0d)
                        {
                            msg = "相手の" + Expression.Term("装甲", u) + "を本来の" + SrcFormatter.Format(100d - 10d * alevel) + "％の値とみなしてダメージ計算を行う。";
                        }
                        else
                        {
                            msg = "相手の" + Expression.Term("装甲", u) + "を半分とみなしてダメージ計算を行う。";
                        }

                        break;
                    }

                case "無":
                    {
                        msg = "バリアやフィールドなどの防御能力の効果を無視してダメージを与える。";
                        break;
                    }

                case "浸":
                    {
                        msg = "シールド防御を無視してダメージを与える。";
                        break;
                    }

                case "破":
                    {
                        msg = "シールド防御の効果を半減させる。";
                        break;
                    }

                case "浄":
                    {
                        msg = "敵の" + p.SkillName0("再生") + "能力を無効化。";
                        break;
                    }

                case "封":
                    {
                        msg = "特定の弱点を持つ敵にのみ有効な武装。" + "弱点をついたときにのみダメージを与えることが出来る。";
                        break;
                    }

                case "限":
                    {
                        msg = "特定の弱点を持つ敵にのみ有効な武装。" + "限定属性以降に指定した属性で;" + "弱点をついたときにのみダメージを与えることが出来る。";
                        break;
                    }

                case "殺":
                    {
                        msg = "相手を一撃で倒せる場合にのみ有効な攻撃。;" + "相手は防御＆シールド防御出来ない。";
                        break;
                    }

                case "♂":
                    {
                        msg = "男性にのみ有効。";
                        break;
                    }

                case "♀":
                    {
                        msg = "女性にのみ有効。";
                        break;
                    }

                case "Ｃ":
                    {
                        msg = "チャージコマンドを使用してチャージ完了の状態にならないと使用不能。";
                        break;
                    }

                case "Ａ":
                    {
                        msg = "使用すると" + SrcFormatter.Format(alevel) + "ターン後に再チャージが完了するまで使用不能。";
                        if (!is_ability)
                        {
                            var loopTo = u.CountWeapon();
                            for (i = 1; i <= loopTo; i++)
                            {
                                if (i != idx && (wanickname ?? "") == (u.WeaponNickname(i) ?? ""))
                                {
                                    msg = msg + "同名の武器も連動して使用不能になる。";
                                    break;
                                }
                            }

                            if (u.IsWeaponClassifiedAs(idx, "共") && u.Weapon(idx).Bullet == 0)
                            {
                                msg = msg + "同レベルの弾薬共有武器も連動して使用不能になる。";
                            }
                        }
                        else
                        {
                            var loopTo1 = u.CountAbility();
                            for (i = 1; i <= loopTo1; i++)
                            {
                                if (i != idx && (wanickname ?? "") == (u.AbilityNickname(i) ?? ""))
                                {
                                    msg = msg + "同名の" + Expression.Term("アビリティ", u) + "も連動して使用不能になる。";
                                    break;
                                }
                            }

                            if (u.IsAbilityClassifiedAs(idx, "共") && u.Ability(idx).Stock == 0)
                            {
                                msg = msg + "同レベルの使用回数共有" + Expression.Term("アビリティ", u) + "も連動して使用不能になる。";
                            }
                        }

                        break;
                    }

                case "合":
                    {
                        var fd = u.Features.FirstOrDefault(fd => fd.Name == "合体技" && fd.DataL.First() == waname));
                        if (fd == null)
                        {
                            GUI.ErrorMessage(u.Name + "の合体技「" + waname + "」に対応した合体技能力がありません");
                            return AttributeHelpMessageRet;
                        }

                        if (fd.DataL.Count == 2)
                        {
                            uname = fd.DataL.Skip(1).First();
                            if (SRC.UDList.IsDefined(uname))
                            {
                                uname = SRC.UDList.Item(uname).Nickname;
                            }

                            if ((uname ?? "") == (u.Nickname ?? ""))
                            {
                                msg = "他の" + uname + "と協力して行う技。";
                            }
                            else
                            {
                                msg = uname + "と協力して行う技。";
                            }
                        }
                        else
                        {
                            msg = "以下のユニットと協力して行う技。;";
                            foreach (var puname in fd.DataL.Skip(1))
                            {
                                uname = puname;
                                if (SRC.UDList.IsDefined(puname))
                                {
                                    uname = SRC.UDList.Item(puname).Nickname;
                                }

                                msg = msg + uname + "  ";
                            }
                        }

                        break;
                    }

                case "共":
                    {
                        if (!is_ability)
                        {
                            msg = "複数の武器で弾薬を共有していることを示す。";
                            if (alevel > 0d)
                            {
                                msg = msg + ";同レベルの弾薬共有武器間で弾薬を共有している。";
                            }
                        }
                        else
                        {
                            msg = "複数の" + Expression.Term("アビリティ", u) + "で使用回数を共有していることを示す。";
                            if (alevel > 0d)
                            {
                                msg = msg + ";同レベルの使用回数共有" + Expression.Term("アビリティ", u) + "間で使用回数を共有している。";
                            }
                        }

                        break;
                    }

                case "斉":
                    {
                        if (!is_ability)
                        {
                            msg = "弾数制の武器全ての弾数を消費して攻撃を行う。";
                        }
                        else
                        {
                            msg = "回数制の" + Expression.Term("アビリティ", u) + "全ての使用回数を消費する。";
                        }

                        break;
                    }

                case "永":
                    {
                        msg = "切り払いや迎撃されない限り弾数が減少しない。";
                        break;
                    }

                case "術":
                    {
                        buf = p.SkillName0("術");
                        if (buf == "非表示")
                        {
                            buf = "術";
                        }

                        msg = buf + "技能によって" + Expression.Term("ＥＮ", u) + "消費量が減少。";
                        if (is_ability)
                        {
                            msg = msg + ";パイロットの" + Expression.Term("魔力", u) + "によって威力が増減する。";
                        }

                        msg = msg + ";沈黙状態の時には使用不能｡";
                        break;
                    }

                case "技":
                    {
                        buf = p.SkillName0("技");
                        if (buf == "非表示")
                        {
                            buf = "技";
                        }

                        msg = buf + "技能によって" + Expression.Term("ＥＮ", u) + "消費量が減少。";
                        break;
                    }

                case "音":
                    {
                        if (!is_ability)
                        {
                            msg = "声などの音を使った攻撃であることを示す｡";
                        }
                        else
                        {
                            msg = "声などの音を使った" + Expression.Term("アビリティ", u) + "であることを示す｡";
                        }

                        msg = msg + "沈黙状態の時には使用不能｡ ";
                        break;
                    }

                case "視":
                    {
                        msg = "視覚に働きかける攻撃。盲目状態のユニットには効かない。";
                        break;
                    }

                case "気":
                    {
                        msg = "使用時に気力" + SrcFormatter.Format(5d * alevel) + "を消費。";
                        break;
                    }

                case "霊":
                case "プ":
                    {
                        msg = "使用時に" + SrcFormatter.Format(5d * alevel) + p.SkillName0("霊力") + "を消費。";
                        break;
                    }

                case "失":
                    {
                        msg = "使用時に" + SrcFormatter.Format((long)(alevel * u.MaxHP) / 10L) + "の" + Expression.Term("ＨＰ", u) + "を失う。";
                        break;
                    }

                case "銭":
                    {
                        msg = "使用時に" + SrcFormatter.Format(GeneralLib.MaxLng((int)alevel, 1) * u.Value / 10) + "の" + Expression.Term("資金", u) + "が必要。;" + Expression.Term("資金", u) + "が足りない場合は使用不可。";
                        break;
                    }

                case "消":
                    {
                        msg = "使用後に1ターン消耗状態に陥り、回避・反撃不能。";
                        break;
                    }

                case "尽":
                    {
                        if (!is_ability)
                        {
                            if (alevel > 0d)
                            {
                                msg = "全" + Expression.Term("ＥＮ", u) + "を使って攻撃し、使用後に" + Expression.Term("ＥＮ", u) + "が0になる。;" + "(残り" + Expression.Term("ＥＮ", u) + "－必要" + Expression.Term("ＥＮ", u) + ")×" + Strings.StrConv(SrcFormatter.Format(alevel), VbStrConv.Wide) + "だけ攻撃力が上昇。";
                            }
                            else
                            {
                                msg = "全" + Expression.Term("ＥＮ", u) + "を使って攻撃し、使用後にＥＮが0になる。";
                            }
                        }
                        else
                        {
                            msg = "使用後に" + Expression.Term("ＥＮ", u) + "が0になる。";
                        }

                        break;
                    }

                case "自":
                    {
                        msg = "使用後に自爆。";
                        break;
                    }

                case "変":
                    {
                        if (u.IsFeatureAvailable("変形技"))
                        {
                            var fd = u.Feature("変形技");
                            if (fd.DataL.First() == (waname ?? ""))
                            {
                                uname = fd.DataL.Skip(1).First();
                            }
                        }

                        if (string.IsNullOrEmpty(uname))
                        {
                            uname = GeneralLib.LIndex(u.FeatureData("ノーマルモード"), 1);
                        }

                        if (SRC.UDList.IsDefined(uname))
                        {
                            {
                                var withBlock1 = SRC.UDList.Item(uname);
                                if ((u.Nickname ?? "") != (withBlock1.Nickname ?? ""))
                                {
                                    uname = withBlock1.Nickname;
                                }
                                else
                                {
                                    uname = withBlock1.Name;
                                }
                            }
                        }

                        msg = "使用後に" + uname + "へ変化する。";
                        break;
                    }

                case "間":
                    {
                        msg = "視界外などから間接的に攻撃を行うことにより" + "相手の反撃を封じる武器。";
                        break;
                    }

                case "Ｍ直":
                    {
                        msg = "上下左右の一方向に対する直線状の効果範囲を持つ。";
                        break;
                    }

                case "Ｍ拡":
                    {
                        msg = "上下左右の一方向に対する幅３マスの直線状の効果範囲を持つ。";
                        break;
                    }

                case "Ｍ扇":
                    {
                        msg = "上下左右の一方向に対する扇状の効果範囲を持つ。;" + "扇の広がり方の度合いはレベルによって異なる。";
                        break;
                    }

                case "Ｍ全":
                    {
                        msg = "ユニットの周り全域に対する効果範囲を持つ。";
                        break;
                    }

                case "Ｍ投":
                    {
                        msg = "指定した地点を中心とした一定範囲の効果範囲を持つ。";
                        break;
                    }

                case "Ｍ移":
                    {
                        msg = "使用後に指定した地点までユニットが移動し、" + "ユニットが通過した場所が効果範囲になる。";
                        break;
                    }

                case "Ｍ線":
                    {
                        msg = "指定した地点とユニットを結ぶ直線が効果範囲になる。";
                        break;
                    }

                case "識":
                    {
                        msg = "効果範囲内にいる味方ユニットを自動的に識別し、敵のみにダメージを与える。";
                        break;
                    }

                case "縛":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            alevel = 2d;
                        }

                        msg = "クリティカル発生時に相手を";
                        if (alevel > 0d)
                        {
                            msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                        }
                        else
                        {
                            msg = msg + "その戦闘中のみ";
                        }

                        msg = msg + "行動不能にする。";
                        break;
                    }

                case "Ｓ":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            alevel = 1d;
                        }

                        msg = "クリティカル発生時に相手を";
                        if (alevel > 0d)
                        {
                            msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                        }
                        else
                        {
                            msg = msg + "その戦闘中のみ";
                        }

                        msg = msg + "行動不能にする。";
                        break;
                    }

                case "劣":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            msg = "クリティカル発生時に相手の装甲を半減させる。";
                        }
                        else
                        {
                            msg = "クリティカル発生時に相手の装甲を";
                            if (alevel > 0d)
                            {
                                msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                            }
                            else
                            {
                                msg = msg + "その戦闘中のみ";
                            }

                            msg = msg + "半減させる。";
                        }

                        break;
                    }

                case "中":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            alevel = 1d;
                        }

                        msg = "クリティカル発生時に相手が持つバリア等の防御能力を";
                        if (alevel > 0d)
                        {
                            msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                        }
                        else
                        {
                            msg = msg + "その戦闘中のみ";
                        }

                        msg = msg + "無効化する。";
                        break;
                    }

                case "石":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            msg = "クリティカル発生時に相手を石化させる。";
                        }
                        else
                        {
                            msg = "クリティカル発生時に相手を";
                            if (alevel > 0d)
                            {
                                msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                            }
                            else
                            {
                                msg = msg + "その戦闘中のみ";
                            }

                            msg = msg + "石化させる。";
                        }

                        break;
                    }

                case "凍":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            alevel = 3d;
                        }

                        msg = "クリティカル発生時に相手を";
                        if (alevel > 0d)
                        {
                            msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                        }
                        else
                        {
                            msg = msg + "その戦闘中のみ";
                        }

                        msg = msg + "凍らせる。";
                        msg = msg + ";凍結した相手は" + Expression.Term("装甲", u) + "が半減するが、";
                        msg = msg + "ダメージを与えると凍結は解除される。";
                        break;
                    }

                case "痺":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            alevel = 3d;
                        }

                        msg = "クリティカル発生時に相手を";
                        if (alevel > 0d)
                        {
                            msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                        }
                        else
                        {
                            msg = msg + "その戦闘中のみ";
                        }

                        msg = msg + "麻痺させる。";
                        break;
                    }

                case "眠":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            alevel = 3d;
                        }

                        msg = "クリティカル発生時に相手を";
                        if (alevel > 0d)
                        {
                            msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                        }
                        else
                        {
                            msg = msg + "その戦闘中のみ";
                        }

                        msg = msg + "眠らせる。";
                        msg = msg + ";眠った相手への攻撃のダメージは１.５倍になるが、睡眠も解除される。";
                        msg = msg + ";性格が機械の敵には無効。";
                        break;
                    }

                case "乱":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            alevel = 3d;
                        }

                        msg = "クリティカル発生時に相手を";
                        if (alevel > 0d)
                        {
                            msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                        }
                        else
                        {
                            msg = msg + "その戦闘中のみ";
                        }

                        msg = msg + "混乱させる。";
                        break;
                    }

                case "魅":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            alevel = 3d;
                        }

                        msg = "クリティカル発生時に相手を";
                        if (alevel > 0d)
                        {
                            msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                        }
                        else
                        {
                            msg = msg + "その戦闘中のみ";
                        }

                        msg = msg + "魅了する。";
                        break;
                    }

                case "憑":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            msg = "クリティカル発生時に相手を乗っ取って支配する。";
                        }
                        else
                        {
                            msg = "クリティカル発生時に相手を";
                            if (alevel > 0d)
                            {
                                msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                            }
                            else
                            {
                                msg = msg + "その戦闘中のみ";
                            }

                            msg = msg + "乗っ取って支配する。";
                        }

                        break;
                    }

                case "盲":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            alevel = 3d;
                        }

                        msg = "クリティカル発生時に相手を";
                        if (alevel > 0d)
                        {
                            msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                        }
                        else
                        {
                            msg = msg + "その戦闘中のみ";
                        }

                        msg = msg + "盲目にする。";
                        break;
                    }

                case "毒":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            alevel = 3d;
                        }

                        msg = "クリティカル発生時に相手を";
                        if (alevel > 0d)
                        {
                            msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                        }
                        else
                        {
                            msg = msg + "その戦闘中のみ";
                        }

                        msg = msg + "毒状態にする。";
                        break;
                    }

                case "撹":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            alevel = 2d;
                        }

                        msg = "クリティカル発生時に相手を";
                        if (alevel > 0d)
                        {
                            msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                        }
                        else
                        {
                            msg = msg + "その戦闘中のみ";
                        }

                        msg = msg + "撹乱する。";
                        break;
                    }

                case "恐":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            alevel = 3d;
                        }

                        msg = "クリティカル発生時に相手を";
                        if (alevel > 0d)
                        {
                            msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                        }
                        else
                        {
                            msg = msg + "その戦闘中のみ";
                        }

                        msg = msg + "恐怖に陥れる。";
                        break;
                    }

                case "不":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            alevel = 1d;
                        }

                        msg = "クリティカル発生時に相手を";
                        if (alevel > 0d)
                        {
                            msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                        }
                        else
                        {
                            msg = msg + "その戦闘中のみ";
                        }

                        msg = msg + "攻撃不能にする。";
                        break;
                    }

                case "止":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            alevel = 1d;
                        }

                        msg = "クリティカル発生時に相手を";
                        if (alevel > 0d)
                        {
                            msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                        }
                        else
                        {
                            msg = msg + "その戦闘中のみ";
                        }

                        msg = msg + "移動不能にする。";
                        break;
                    }

                case "黙":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            alevel = 3d;
                        }

                        msg = "クリティカル発生時に相手を";
                        if (alevel > 0d)
                        {
                            msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                        }
                        else
                        {
                            msg = msg + "その戦闘中のみ";
                        }

                        msg = msg + "沈黙状態にする。";
                        break;
                    }

                case "除":
                    {
                        if (!is_ability)
                        {
                            msg = "クリティカル発生時に相手にかけられた" + Expression.Term("アビリティ", u) + "による特殊効果を打ち消す。";
                        }
                        else
                        {
                            msg = Expression.Term("アビリティ", u) + "実行時に、それまでに相手にかけられていた" + Expression.Term("アビリティ", u) + "による特殊効果が解除される。";
                        }

                        break;
                    }

                case "即":
                    {
                        msg = "クリティカル発生時に相手を即死させる。";
                        break;
                    }

                case "告":
                    {
                        if (alevel > 0d)
                        {
                            msg = "クリティカル発生時に相手を「死の宣告」状態にし、" + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン後に" + Expression.Term("ＨＰ", u) + "を１にする。";
                        }
                        else
                        {
                            msg = "クリティカル発生時に相手の" + Expression.Term("ＨＰ", u) + "を１にする。";
                        }

                        break;
                    }

                case "脱":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            msg = "相手の" + Expression.Term("気力", u) + "を10低下させる。";
                        }
                        else if (alevel >= 0d)
                        {
                            msg = "相手の" + Expression.Term("気力", u) + "を" + SrcFormatter.Format((int)(5d * alevel)) + "低下させる。";
                        }
                        else
                        {
                            msg = "相手の" + Expression.Term("気力", u) + "を" + SrcFormatter.Format((int)(-5 * alevel)) + "増加させる。";
                        }

                        break;
                    }

                case "Ｄ":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            msg = "相手の" + Expression.Term("気力", u) + "を10低下させ、その半分を吸収する。";
                        }
                        else if (alevel >= 0d)
                        {
                            msg = "相手の" + Expression.Term("気力", u) + "を" + SrcFormatter.Format((int)(5d * alevel)) + "低下させ、その半分を吸収する。";
                        }
                        else
                        {
                            msg = "相手の" + Expression.Term("気力", u) + "を" + SrcFormatter.Format((int)(-5 * alevel)) + "増加させ、その半分を与える。";
                        }

                        break;
                    }

                case "低攻":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            alevel = 3d;
                        }

                        msg = "クリティカル発生時に相手の攻撃力を";
                        if (alevel > 0d)
                        {
                            msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                        }
                        else
                        {
                            msg = msg + "その戦闘中のみ";
                        }

                        msg = msg + "低下させる。";
                        break;
                    }

                case "低防":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            alevel = 3d;
                        }

                        msg = "クリティカル発生時に相手の" + Expression.Term("装甲", u) + "を";
                        if (alevel > 0d)
                        {
                            msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                        }
                        else
                        {
                            msg = msg + "その戦闘中のみ";
                        }

                        msg = msg + "低下させる。";
                        break;
                    }

                case "低運":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            alevel = 3d;
                        }

                        msg = "クリティカル発生時に相手の" + Expression.Term("運動性", u) + "を";
                        if (alevel > 0d)
                        {
                            msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                        }
                        else
                        {
                            msg = msg + "その戦闘中のみ";
                        }

                        msg = msg + "低下させる。";
                        break;
                    }

                case "低移":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            alevel = 3d;
                        }

                        msg = "クリティカル発生時に相手の" + Expression.Term("移動力", u) + "を";
                        if (alevel > 0d)
                        {
                            msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                        }
                        else
                        {
                            msg = msg + "その戦闘中のみ";
                        }

                        msg = msg + "低下させる。。";
                        break;
                    }

                case "先":
                    {
                        msg = "反撃時でも相手より先に攻撃する。";
                        break;
                    }

                case "後":
                    {
                        msg = "反撃時ではない場合も相手より後に攻撃する。";
                        break;
                    }

                case "吹":
                    {
                        if (alevel > 0d)
                        {
                            msg = "相手ユニットを" + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "マス吹き飛ばす。;" + "クリティカル発生時は吹き飛ばし距離＋１。";
                        }
                        else
                        {
                            msg = "クリティカル発生時に相手ユニットを１マス吹き飛ばす。";
                        }

                        break;
                    }

                case "Ｋ":
                    {
                        if (alevel > 0d)
                        {
                            msg = "相手ユニットを" + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "マス吹き飛ばす。;" + "クリティカル発生時は吹き飛ばし距離＋１。" + Expression.Term("サイズ", u) + "制限あり。";
                        }
                        else
                        {
                            msg = "クリティカル発生時に相手ユニットを１マス吹き飛ばす。" + Expression.Term("サイズ", u) + "制限あり。";
                        }

                        break;
                    }

                case "引":
                    {
                        msg = "クリティカル発生時に相手ユニットを隣接するマスまで引き寄せる。";
                        break;
                    }

                case "転":
                    {
                        msg = "クリティカル発生時に相手ユニットを" + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "マス強制テレポートさせる。テレポート先はランダムに選ばれる。";
                        break;
                    }

                case "連":
                    {
                        msg = SrcFormatter.Format(alevel) + "回連続して攻撃を行う。;" + "攻撃によって与えるダメージは下記の式で計算される。;" + "  通常のダメージ量 × 命中回数 ／ 攻撃回数";
                        break;
                    }

                case "再":
                    {
                        msg = SrcFormatter.Format((long)(100d * alevel) / 16L) + "%の確率で再攻撃。";
                        break;
                    }

                case "精":
                    {
                        msg = "精神に働きかける攻撃。性格が「機械」のユニットには効かない。" + "シールドを無効化。";
                        break;
                    }

                case "援":
                    {
                        msg = "自分以外のユニットに対してのみ使用可能。";
                        break;
                    }

                case "難":
                    {
                        msg = SrcFormatter.Format(10d * alevel) + "%の確率で失敗する。";
                        break;
                    }

                case "忍":
                    {
                        msg = "物音を立てずに攻撃し、" + "ステルス状態の際に" + Expression.Term("ＣＴ率", u) + "に+10のボーナス。" + "一撃で相手を倒した場合は自分から攻撃をかけてもステルス状態が維持される。";
                        break;
                    }

                case "盗":
                    {
                        msg = "クリティカル発生時に敵から持ち物を盗む。;" + "盗めるものは通常は" + Expression.Term("資金", u) + "(普通に倒した時の半分の額)だが、" + "相手によってはアイテムを入手することもある。";
                        break;
                    }

                case "Ｈ":
                    {
                        msg = "レーダー等でターゲットを追尾する攻撃。;";
                        if (Expression.IsOptionDefined("距離修正"))
                        {
                            msg = msg + "長距離の敵を攻撃する際も命中率が低下しないが、";
                        }

                        msg = msg + "ＥＣＭによる影響を強く受ける。";
                        msg = msg + "攻撃側が撹乱等の状態に陥っても命中率が低下しない。";
                        break;
                    }

                case "追":
                    {
                        msg = "自己判断能力を持ち、ターゲットを追尾する攻撃。;";
                        if (Expression.IsOptionDefined("距離修正"))
                        {
                            msg = msg + "長距離の敵を攻撃する際も命中率が低下しない。また、";
                        }

                        msg = msg + "攻撃側が撹乱等の状態に陥っても命中率が低下しない。";
                        break;
                    }

                case "有":
                    {
                        msg = "有線による誘導でターゲットを追尾する攻撃。;";
                        if (Expression.IsOptionDefined("距離修正"))
                        {
                            msg = msg + "長距離の敵を攻撃する際も命中率が低下しない。また、";
                        }

                        msg = msg + "ＥＣＭによる影響を受けない。";
                        msg = msg + "しかし、スペシャルパワーや" + "アイテムの効果によって射程が増加しない。";
                        break;
                    }

                case "誘":
                    {
                        msg = "電波妨害を受けない特殊な手段による誘導でターゲットを追尾する攻撃。;";
                        if (Expression.IsOptionDefined("距離修正"))
                        {
                            msg = msg + "長距離の敵を攻撃する際も命中率が低下しない。また、";
                        }

                        msg = msg + "ＥＣＭによる影響を受けない。";
                        break;
                    }

                case "爆":
                    {
                        msg = "爆発によりダメージを与える攻撃。;";
                        if (Expression.IsOptionDefined("距離修正"))
                        {
                            msg = msg + "長距離の敵を攻撃する際もダメージが低下しない。";
                        }

                        break;
                    }

                case "空":
                    {
                        msg = "空中にいるターゲットを攻撃することを目的とした攻撃。";
                        if (Expression.IsOptionDefined("高度修正"))
                        {
                            msg = msg + "地上から空中にいる敵を攻撃する際に命中率が低下しない。";
                        }

                        break;
                    }

                case "固":
                    {
                        msg = "パイロットの" + Expression.Term("気力", u) + "や攻撃力、防御側の" + Expression.Term("装甲", u) + "にかかわらず" + "武器の攻撃力と同じダメージを与える攻撃。" + "ただし、ユニットランクが上がっても攻撃力は増えない。" + Expression.Term("スペシャルパワー", u) + "や" + Expression.Term("地形適応", u) + "によるダメージ修正は有効。";
                        break;
                    }

                case "衰":
                    {
                        msg = "クリティカル発生時に敵の" + Expression.Term("ＨＰ", u) + "を現在値の ";
                        switch ((int)alevel)
                        {
                            case 1:
                                {
                                    msg = msg + "3/4";
                                    break;
                                }

                            case 2:
                                {
                                    msg = msg + "1/2";
                                    break;
                                }

                            case 3:
                                {
                                    msg = msg + "1/4";
                                    break;
                                }
                        }

                        msg = msg + " まで減少させる。";
                        break;
                    }

                case "滅":
                    {
                        msg = "クリティカル発生時に敵の" + Expression.Term("ＥＮ", u) + "を現在値の ";
                        switch ((int)alevel)
                        {
                            case 1:
                                {
                                    msg = msg + "3/4";
                                    break;
                                }

                            case 2:
                                {
                                    msg = msg + "1/2";
                                    break;
                                }

                            case 3:
                                {
                                    msg = msg + "1/4";
                                    break;
                                }
                        }

                        msg = msg + " まで減少させる。";
                        break;
                    }

                case "踊":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            alevel = 3d;
                        }

                        msg = "クリティカル発生時に相手を";
                        if (alevel > 0d)
                        {
                            msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                        }
                        else
                        {
                            msg = msg + "その戦闘中のみ";
                        }

                        msg = msg + "踊らせる。";
                        break;
                    }

                case "狂":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            alevel = 3d;
                        }

                        msg = "クリティカル発生時に相手を";
                        if (alevel > 0d)
                        {
                            msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                        }
                        else
                        {
                            msg = msg + "その戦闘中のみ";
                        }

                        msg = msg + "狂戦士状態にする。";
                        break;
                    }

                case "ゾ":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            msg = "クリティカル発生時に相手をゾンビ状態にする。";
                        }
                        else
                        {
                            msg = "クリティカル発生時に相手を";
                            if (alevel > 0d)
                            {
                                msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                            }
                            else
                            {
                                msg = msg + "その戦闘中のみ";
                            }

                            msg = msg + "ゾンビ状態にする。";
                        }

                        break;
                    }

                case "害":
                    {
                        if (alevel == Constants.DEFAULT_LEVEL)
                        {
                            msg = "クリティカル発生時に相手の自己回復能力を破壊する。";
                        }
                        else
                        {
                            msg = "クリティカル発生時に相手を";
                            if (alevel > 0d)
                            {
                                msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                            }
                            else
                            {
                                msg = msg + "その戦闘中のみ";
                            }

                            msg = msg + "自己回復不能状態にする。";
                        }

                        break;
                    }

                case "習":
                    {
                        msg = "クリティカル発生時に相手の持つ技を習得出来る。;" + "ただし、習得可能な技を相手が持っていなければ無効。";
                        break;
                    }

                case "写":
                    {
                        msg = "クリティカル発生時に相手ユニットに変身する。;" + "ただし、既に変身している場合は使用できない。" + "また、相手と２段階以上" + Expression.Term("サイズ", u) + "が異なる場合は無効。";
                        break;
                    }

                case "化":
                    {
                        msg = "クリティカル発生時に相手ユニットに変身する。;" + "ただし、既に変身している場合は使用できない。";
                        break;
                    }

                case "痛":
                    {
                        msg = "クリティカル発生時に通常の ";
                        if (Expression.IsOptionDefined("ダメージ倍率低下"))
                        {
                            msg = msg + SrcFormatter.Format(100d + 10d * (alevel + 2d));
                        }
                        else
                        {
                            msg = msg + SrcFormatter.Format(100d + 25d * (alevel + 2d));
                        }

                        msg = msg + "% のダメージを与える。";
                        break;
                    }

                case "地":
                case "水":
                case "火":
                case "風":
                case "冷":
                case "雷":
                case "光":
                case "闇":
                case "聖":
                case "死":
                case "木":
                    {
                        switch (atype ?? "")
                        {
                            case "水":
                            case "火":
                            case "風":
                                {
                                    msg = atype + "を使った";
                                    break;
                                }

                            case "光":
                            case "闇":
                            case "死":
                                {
                                    msg = atype + "の力を使った";
                                    break;
                                }

                            case "地":
                                {
                                    msg = "大地の力を借りた";
                                    break;
                                }

                            case "冷":
                                {
                                    msg = "冷気による";
                                    break;
                                }

                            case "雷":
                                {
                                    msg = "電撃による";
                                    break;
                                }

                            case "聖":
                                {
                                    msg = "聖なる力を借りた";
                                    break;
                                }

                            case "木":
                                {
                                    msg = "樹木の力を借りた";
                                    break;
                                }
                        }

                        msg = msg + whatsthis + "。";
                        break;
                    }

                case "魔":
                    {
                        if (!is_ability)
                        {
                            msg = "魔力を帯びた攻撃。";
                        }
                        else
                        {
                            msg = "魔法による" + Expression.Term("アビリティ", u) + "。";
                        }

                        break;
                    }

                case "時":
                    {
                        msg = "時の流れを操る" + whatsthis + "。";
                        break;
                    }

                case "重":
                    {
                        msg = "重力を使った攻撃。";
                        break;
                    }

                case "銃":
                case "剣":
                case "刀":
                case "槍":
                case "斧":
                case "弓":
                    {
                        msg = atype + "を使った攻撃。";
                        break;
                    }

                case "機":
                    {
                        msg = "機械(ロボット、アンドロイド)に対し特に有効な攻撃。";
                        break;
                    }

                case "感":
                    {
                        msg = "エスパー(超能力者)に対し特に有効な攻撃。";
                        break;
                    }

                case "竜":
                    {
                        msg = "竜族(ドラゴン)に対し特に有効な武器。";
                        break;
                    }

                case "瀕":
                    {
                        msg = "瀕死時にのみ使用可能な" + whatsthis + "。";
                        break;
                    }

                case "禁":
                    {
                        msg = "現在の状況下では使用することが出来ません。";
                        break;
                    }

                case "対":
                    {
                        if (!is_ability)
                        {
                            whatsthis = "攻撃";
                        }

                        msg = "相手のメインパイロットのレベルが" + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "の倍数の場合にのみ有効な" + whatsthis + "。";
                        break;
                    }

                case "ラ":
                    {
                        if (!is_ability)
                        {
                            whatsthis = "攻撃";
                        }

                        msg = "ラーニングが可能な" + whatsthis + "。";
                        break;
                    }

                case "小":
                    {
                        msg = "最小射程が" + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "になる。";
                        break;
                    }

                case "散":
                    {
                        msg = "相手から２マス以上離れていると命中率が上昇し、与えるダメージが減少する。";
                        break;
                    }

                default:
                    {
                        // 弱、効、剋属性
                        switch (Strings.Left(atype, 1) ?? "")
                        {
                            case "弱":
                                {
                                    if (alevel == Constants.DEFAULT_LEVEL)
                                    {
                                        alevel = 3d;
                                    }

                                    msg = "クリティカル発生時に相手に" + Strings.Mid(atype, 2) + "属性に対する弱点を";
                                    if (alevel > 0d)
                                    {
                                        msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                                    }
                                    else
                                    {
                                        msg = msg + "その戦闘中のみ";
                                    }

                                    msg = msg + "付加する。";
                                    break;
                                }

                            case "効":
                                {
                                    if (alevel == Constants.DEFAULT_LEVEL)
                                    {
                                        alevel = 3d;
                                    }

                                    msg = "クリティカル発生時に相手に" + Strings.Mid(atype, 2) + "属性に対する有効を";
                                    if (alevel > 0d)
                                    {
                                        msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                                    }
                                    else
                                    {
                                        msg = msg + "その戦闘中のみ";
                                    }

                                    msg = msg + "付加する。";
                                    break;
                                }

                            case "剋":
                                {
                                    if (alevel == Constants.DEFAULT_LEVEL)
                                    {
                                        alevel = 3d;
                                    }

                                    msg = "クリティカル発生時に相手の";
                                    switch (Strings.Mid(atype, 2) ?? "")
                                    {
                                        case "オ":
                                            {
                                                msg = msg + "オーラ";
                                                break;
                                            }

                                        case "超":
                                            {
                                                msg = msg + "超能力";
                                                break;
                                            }

                                        case "シ":
                                            {
                                                msg = msg + "同調率";
                                                break;
                                            }

                                        case "サ":
                                            {
                                                msg = msg + "超感覚、知覚強化";
                                                break;
                                            }

                                        case "霊":
                                            {
                                                msg = msg + "霊力";
                                                break;
                                            }

                                        case "術":
                                            {
                                                msg = msg + "術";
                                                break;
                                            }

                                        case "技":
                                            {
                                                msg = msg + "技";
                                                break;
                                            }

                                        default:
                                            {
                                                msg = msg + Strings.Mid(atype, 2) + "属性の武器、アビリティ";
                                                break;
                                            }
                                    }

                                    msg = msg + "を";
                                    if (alevel > 0d)
                                    {
                                        msg = msg + Strings.StrConv(SrcFormatter.Format((int)alevel), VbStrConv.Wide) + "ターン";
                                    }
                                    else
                                    {
                                        msg = msg + "その戦闘中のみ";
                                    }

                                    msg = msg + "使用不能にする。";
                                    break;
                                }
                        }

                        break;
                    }
            }

            fdata = u.FeatureData(atype);
            if (GeneralLib.ListIndex(fdata, 1) == "解説")
            {
                // 解説を定義している場合
                msg = GeneralLib.ListTail(fdata, 3);
                if (Strings.Left(msg, 1) == "\"")
                {
                    msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
                }
            }

            // 等身大基準の際は「パイロット」という語を使わないようにする
            if (Expression.IsOptionDefined("等身大基準"))
            {
                msg = msg.Replace("メインパイロット", "ユニット");
                msg = msg.Replace("パイロット", "ユニット");
                msg = msg.Replace("相手のユニット", "相手ユニット");
            }

            AttributeHelpMessageRet = msg;
            return AttributeHelpMessageRet;
        }
    }
}
