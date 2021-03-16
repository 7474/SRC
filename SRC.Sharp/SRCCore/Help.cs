//using System;
//using Microsoft.VisualBasic;
//using Microsoft.VisualBasic.CompilerServices;
//using SRCCore.Pilots;
//using SRCCore.VB;

//namespace Project1
//{
//    static class Help
//    {

//        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
//        // 本プログラムはフリーソフトであり、無保証です。
//        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
//        // 再頒布または改変することができます。

//        // 特殊能力＆武器属性の解説表示を行うモジュール


//        // パイロット p の特殊能力の解説を表示
//        public static void SkillHelp(ref Pilot p, ref string sindex)
//        {
//            string stype, sname;
//            string msg;
//            bool prev_mode;

//            // 特殊能力の名称を調べる
//            if (Information.IsNumeric(sindex))
//            {
//                object argIndex1 = Conversions.ToShort(sindex);
//                sname = p.SkillName(ref argIndex1);
//            }
//            else
//            {
//                // 付加されたパイロット用特殊能力
//                if (Strings.InStr(sindex, "Lv") > 0)
//                {
//                    stype = Strings.Left(sindex, Strings.InStr(sindex, "Lv") - 1);
//                }
//                else
//                {
//                    stype = sindex;
//                }

//                object argIndex2 = stype;
//                sname = p.SkillName(ref argIndex2);
//            }

//            msg = SkillHelpMessage(ref p, ref sindex);

//            // 解説の表示
//            if (Strings.Len(msg) > 0)
//            {
//                prev_mode = GUI.AutoMessageMode;
//                GUI.AutoMessageMode = false;
//                Unit argu1 = null;
//                Unit argu2 = null;
//                GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
//                if (SRC.AutoMoveCursor)
//                {
//                    string argcursor_mode = "メッセージウィンドウ";
//                    GUI.MoveCursorPos(ref argcursor_mode);
//                }

//                string argpname = "システム";
//                GUI.DisplayMessage(ref argpname, "<b>" + sname + "</b>;" + msg);
//                GUI.CloseMessageForm();
//                GUI.AutoMessageMode = prev_mode;
//            }
//        }

//        // パイロット p の特殊能力の解説
//        public static string SkillHelpMessage(ref Pilot p, ref string sindex)
//        {
//            string SkillHelpMessageRet = default;
//            string sname, stype, sname0;
//            double slevel;
//            string sdata;
//            bool is_level_specified;
//            var msg = default(string);
//            Unit u, u2 = default;
//            string uname, fdata;
//            short i;

//            // 特殊能力の名称、レベル、データを調べる
//            if (Information.IsNumeric(sindex))
//            {
//                object argIndex1 = Conversions.ToShort(sindex);
//                stype = p.Skill(ref argIndex1);
//                object argIndex2 = Conversions.ToShort(sindex);
//                string argref_mode = "";
//                slevel = p.SkillLevel(ref argIndex2, ref_mode: ref argref_mode);
//                object argIndex3 = Conversions.ToShort(sindex);
//                sdata = p.SkillData(ref argIndex3);
//                object argIndex4 = Conversions.ToShort(sindex);
//                sname = p.SkillName(ref argIndex4);
//                object argIndex5 = Conversions.ToShort(sindex);
//                sname0 = p.SkillName0(ref argIndex5);
//                object argIndex6 = Conversions.ToShort(sindex);
//                is_level_specified = p.IsSkillLevelSpecified(ref argIndex6);
//            }
//            else
//            {
//                // 付加されたパイロット用特殊能力
//                if (Strings.InStr(sindex, "Lv") > 0)
//                {
//                    stype = Strings.Left(sindex, Strings.InStr(sindex, "Lv") - 1);
//                }
//                else
//                {
//                    stype = sindex;
//                }

//                stype = p.SkillType(ref stype);
//                object argIndex7 = stype;
//                string argref_mode1 = "";
//                slevel = p.SkillLevel(ref argIndex7, ref_mode: ref argref_mode1);
//                object argIndex8 = stype;
//                sdata = p.SkillData(ref argIndex8);
//                object argIndex9 = stype;
//                sname = p.SkillName(ref argIndex9);
//                object argIndex10 = stype;
//                sname0 = p.SkillName0(ref argIndex10);
//                object argIndex11 = stype;
//                is_level_specified = p.IsSkillLevelSpecified(ref argIndex11);
//            }

//            // パイロットが乗っているユニット
//            u = p.Unit_Renamed;
//            if (u.Name == "ステータス表示用ダミーユニット")
//            {
//                string argvname = "搭乗ユニット[" + p.ID + "]";
//                if (Expression.IsLocalVariableDefined(ref argvname))
//                {
//                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
//                    uname = Conversions.ToString(Event_Renamed.LocalVariableList["搭乗ユニット[" + p.ID + "]"].StringValue);
//                    if (!string.IsNullOrEmpty(uname))
//                    {
//                        u2 = u;
//                        object argIndex12 = uname;
//                        u = SRC.UList.Item(ref argIndex12);
//                    }
//                }
//            }

//            switch (stype ?? "")
//            {
//                case "オーラ":
//                    {
//                        object argIndex14 = "バリア";
//                        if (u.FeatureName0(ref argIndex14) == "オーラバリア")
//                        {
//                            object argIndex13 = "オーラバリア";
//                            msg = "オーラ技「オ」の攻撃力と" + u.FeatureName0(ref argIndex13) + "の強度に" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(100d * slevel)) + "の修正を与える。";
//                        }
//                        else
//                        {
//                            msg = "オーラ技「オ」の攻撃力の強度に" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(100d * slevel)) + "の修正を与える。";
//                        }

//                        string argfname = "オーラ変換器";
//                        if (u.IsFeatureAvailable(ref argfname))
//                        {
//                            string argtname = "ＨＰ";
//                            string argtname1 = "ＥＮ";
//                            string argtname2 = "装甲";
//                            string argtname3 = "運動性";
//                            Unit argu = null;
//                            msg = msg + "また、" + Expression.Term(ref argtname, ref u) + "、" + Expression.Term(ref argtname1, ref u) + "、" + Expression.Term(ref argtname2, ref u) + "、" + Expression.Term(ref argtname3, u: ref argu) + "がレベルに合わせてそれぞれ増加する。";
//                        }

//                        break;
//                    }

//                case "分身":
//                    {
//                        msg = Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)((long)(100d * slevel) / 16L)) + "% の確率で分身し、攻撃を回避する。";
//                        break;
//                    }

//                case "超感覚":
//                    {
//                        string argtname4 = "命中";
//                        string argtname5 = "回避";
//                        msg = Expression.Term(ref argtname4, ref u) + "・" + Expression.Term(ref argtname5, ref u);
//                        if (slevel > 0d)
//                        {
//                            msg = msg + "に +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(2d * slevel + 3d)) + " の修正を与える。";
//                        }
//                        else
//                        {
//                            msg = msg + "に +0 の修正を与える。";
//                        }

//                        if (slevel > 3d)
//                        {
//                            msg = msg + ";思念誘導攻撃(サ)の射程を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)((long)slevel / 4L)) + "だけ延長する。";
//                        }

//                        break;
//                    }

//                case "知覚強化":
//                    {
//                        string argtname6 = "命中";
//                        string argtname7 = "回避";
//                        msg = Expression.Term(ref argtname6, ref u) + "・" + Expression.Term(ref argtname7, ref u);
//                        if (slevel > 0d)
//                        {
//                            msg = msg + "に +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(2d * slevel + 3d)) + " の修正を与える。;";
//                        }
//                        else
//                        {
//                            msg = msg + "に +0 の修正を与える。;";
//                        }

//                        if (slevel > 3d)
//                        {
//                            msg = msg + "思念誘導攻撃(サ)の射程を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)((long)slevel / 4L)) + "だけ延長する。";
//                        }

//                        string argtname8 = "ＳＰ";
//                        msg = msg + "精神不安定により" + Expression.Term(ref argtname8, ref u) + "消費量が20%増加する";
//                        break;
//                    }

//                case "念力":
//                    {
//                        string argtname9 = "命中";
//                        string argtname10 = "回避";
//                        msg = Expression.Term(ref argtname9, ref u) + "・" + Expression.Term(ref argtname10, ref u);
//                        if (slevel > 0d)
//                        {
//                            msg = msg + "に +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(2d * slevel + 3d)) + " の修正を与える。";
//                        }
//                        else
//                        {
//                            msg = msg + "に +0 の修正を与える。";
//                        }

//                        break;
//                    }

//                case "切り払い":
//                    {
//                        msg = "格闘武器(武)、突進技(突)、実弾攻撃(実)による攻撃を " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)((long)(100d * slevel) / 16L)) + "% の確率で切り払って回避する。";
//                        break;
//                    }

//                case "迎撃":
//                    {
//                        msg = "実弾攻撃(実)による攻撃を " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)((long)(100d * slevel) / 16L)) + "% の確率で迎撃する。";
//                        break;
//                    }

//                case "サイボーグ":
//                    {
//                        string argtname11 = "命中";
//                        string argtname12 = "回避";
//                        msg = Expression.Term(ref argtname11, ref u) + "・" + Expression.Term(ref argtname12, ref u);
//                        msg = msg + "に +5 の修正を与える。";
//                        break;
//                    }

//                case "Ｓ防御":
//                    {
//                        string argfname1 = "盾";
//                        if (u.IsFeatureAvailable(ref argfname1))
//                        {
//                            msg = "シールド防御を行い、ダメージを" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(100d * slevel + 400d)) + "減少させる。";
//                        }
//                        else
//                        {
//                            msg = Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)((long)(100d * slevel) / 16L)) + "% の確率でシールド防御を行う。";
//                        }

//                        break;
//                    }

//                case "資金獲得":
//                    {
//                        string argtname13 = "資金";
//                        Unit argu1 = null;
//                        msg = "敵を倒した時に得られる" + Expression.Term(ref argtname13, u: ref argu1);
//                        if (!is_level_specified)
//                        {
//                            msg = msg + "が 50% 増加する。";
//                        }
//                        else if (slevel >= 0d)
//                        {
//                            msg = msg + "が " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * slevel) + "% 増加する。";
//                        }
//                        else
//                        {
//                            msg = msg + "が " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(-10 * slevel) + "% 減少する。";
//                        }

//                        break;
//                    }

//                case "浄化":
//                    {
//                        object argIndex15 = "再生";
//                        msg = "浄化技(浄)を使うことで敵の" + p.SkillName0(ref argIndex15) + "能力を無効化。";
//                        break;
//                    }

//                case "同調率":
//                    {
//                        if (u.IsHero())
//                        {
//                            msg = "同調により";
//                        }
//                        else
//                        {
//                            msg = "機体に同調し";
//                        }

//                        string argtname14 = "運動性";
//                        msg = msg + Expression.Term(ref argtname14, ref u) + "・攻撃力を強化する。";
//                        break;
//                    }

//                case "同調率成長":
//                    {
//                        if (slevel >= 0d)
//                        {
//                            object argIndex16 = "同調率";
//                            msg = p.SkillName0(ref argIndex16) + "の成長率が " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * slevel) + "% 増加する。";
//                        }
//                        else
//                        {
//                            object argIndex17 = "同調率";
//                            msg = p.SkillName0(ref argIndex17) + "の成長率が " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(-10 * slevel) + "% 減少する。";
//                        }

//                        break;
//                    }

//                case "霊力":
//                    {
//                        string argtname15 = "ＨＰ";
//                        string argtname16 = "ＥＮ";
//                        string argtname17 = "装甲";
//                        string argtname18 = "移動力";
//                        msg = "現在の" + sname0 + "値にあわせて" + Expression.Term(ref argtname15, ref u) + "・" + Expression.Term(ref argtname16, ref u) + "・" + Expression.Term(ref argtname17, ref u) + "・" + Expression.Term(ref argtname18, ref u) + "を強化する。";
//                        break;
//                    }

//                case "霊力成長":
//                    {
//                        if (slevel >= 0d)
//                        {
//                            object argIndex18 = "霊力";
//                            msg = p.SkillName0(ref argIndex18) + "の成長率が " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * slevel) + "% 増加する。";
//                        }
//                        else
//                        {
//                            object argIndex19 = "霊力";
//                            msg = p.SkillName0(ref argIndex19) + "の成長率が " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(-10 * slevel) + "% 減少する。";
//                        }

//                        break;
//                    }

//                case "底力":
//                    {
//                        string argtname19 = "ＨＰ";
//                        string argtname20 = "ＨＰ";
//                        msg = Expression.Term(ref argtname19, ref u) + "が最大" + Expression.Term(ref argtname20, ref u) + "の1/4以下の時に発動。;" + "命中＆回避 +30%、クリティカル発生率 +50%。";
//                        break;
//                    }

//                case "超底力":
//                    {
//                        string argtname21 = "ＨＰ";
//                        string argtname22 = "ＨＰ";
//                        msg = Expression.Term(ref argtname21, ref u) + "が最大" + Expression.Term(ref argtname22, ref u) + "の1/4以下の時に発動。;" + "命中＆回避 +50%、クリティカル発生率 +50%。";
//                        break;
//                    }

//                case "覚悟":
//                    {
//                        string argtname23 = "ＨＰ";
//                        string argtname24 = "ＨＰ";
//                        msg = Expression.Term(ref argtname23, ref u) + "が最大" + Expression.Term(ref argtname24, ref u) + "の1/4以下の時に発動。;";
//                        string argoname = "ダメージ倍率低下";
//                        if (Expression.IsOptionDefined(ref argoname))
//                        {
//                            msg = msg + "攻撃力10%アップ、クリティカル発生率 +50%。";
//                        }
//                        else
//                        {
//                            msg = msg + "攻撃力1.2倍、クリティカル発生率 +50%。";
//                        }

//                        break;
//                    }

//                case "不屈":
//                    {
//                        string argtname25 = "ＨＰ";
//                        string argtname26 = "ＨＰ";
//                        msg = Expression.Term(ref argtname25, ref u) + "が最大" + Expression.Term(ref argtname26, ref u) + "の1/2以下の時に発動。;" + "損傷率に応じて防御力が増加する。";
//                        break;
//                    }

//                case "素質":
//                    {
//                        if (!is_level_specified)
//                        {
//                            msg = "入手する経験値が50%増加する。";
//                        }
//                        else if (slevel >= 0d)
//                        {
//                            msg = "入手する経験値が " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * slevel) + "% 増加する。";
//                        }
//                        else
//                        {
//                            msg = "入手する経験値が " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(-10 * slevel) + "% 減少する。";
//                        }

//                        break;
//                    }

//                case "遅成長":
//                    {
//                        msg = "入手する経験値が半減する。";
//                        break;
//                    }

//                case "再生":
//                case "英雄":
//                    {
//                        string argtname27 = "ＨＰ";
//                        msg = Expression.Term(ref argtname27, ref u) + "が０になった時に" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)((long)(100d * slevel) / 16L)) + "%の確率で復活する。";
//                        break;
//                    }

//                case "超能力":
//                    {
//                        string argtname28 = "命中";
//                        string argtname29 = "回避";
//                        string argtname30 = "ＣＴ率";
//                        string argtname31 = "ＳＰ";
//                        msg = Expression.Term(ref argtname28, ref u) + "・" + Expression.Term(ref argtname29, ref u) + "・" + Expression.Term(ref argtname30, ref u) + "にそれぞれ +5。;" + "サイキック攻撃(超)の攻撃力に +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(100d * slevel)) + "。;" + Expression.Term(ref argtname31, ref u) + "消費量を20%削減する。";
//                        break;
//                    }

//                case "悟り":
//                    {
//                        string argtname32 = "命中";
//                        string argtname33 = "回避";
//                        msg = Expression.Term(ref argtname32, ref u) + "・" + Expression.Term(ref argtname33, ref u) + "に +10 の修正を与える。";
//                        break;
//                    }

//                case "超反応":
//                    {
//                        string argtname34 = "命中";
//                        string argtname35 = "回避";
//                        string argtname36 = "ＣＴ率";
//                        msg = Expression.Term(ref argtname34, ref u) + "・" + Expression.Term(ref argtname35, ref u) + "・" + Expression.Term(ref argtname36, ref u);
//                        if (slevel >= 0d)
//                        {
//                            msg = msg + "にそれぞれ +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(2d * slevel)) + " の修正を与える。";
//                        }
//                        else
//                        {
//                            msg = msg + "にそれぞれ " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(2d * slevel)) + " の修正を与える。";
//                        }

//                        break;
//                    }

//                case "術":
//                    {
//                        switch (slevel)
//                        {
//                            case 1d:
//                                {
//                                    i = 0;
//                                    break;
//                                }

//                            case 2d:
//                                {
//                                    i = 10;
//                                    break;
//                                }

//                            case 3d:
//                                {
//                                    i = 20;
//                                    break;
//                                }

//                            case 4d:
//                                {
//                                    i = 30;
//                                    break;
//                                }

//                            case 5d:
//                                {
//                                    i = 40;
//                                    break;
//                                }

//                            case 6d:
//                                {
//                                    i = 50;
//                                    break;
//                                }

//                            case 7d:
//                                {
//                                    i = 55;
//                                    break;
//                                }

//                            case 8d:
//                                {
//                                    i = 60;
//                                    break;
//                                }

//                            case 9d:
//                                {
//                                    i = 65;
//                                    break;
//                                }

//                            case var @case when @case >= 10d:
//                                {
//                                    i = 70;
//                                    break;
//                                }

//                            default:
//                                {
//                                    i = 0;
//                                    break;
//                                }
//                        }

//                        string argtname37 = "アビリティ";
//                        string argtname38 = "アビリティ";
//                        string argtname39 = "ＥＮ";
//                        msg = "術属性を持つ武装・" + Expression.Term(ref argtname37, ref u) + "及び必要技能が" + sname0 + "の武装・" + Expression.Term(ref argtname38, ref u) + "の消費" + Expression.Term(ref argtname39, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(i) + "%減少させる。";
//                        break;
//                    }

//                case "技":
//                    {
//                        switch (slevel)
//                        {
//                            case 1d:
//                                {
//                                    i = 0;
//                                    break;
//                                }

//                            case 2d:
//                                {
//                                    i = 10;
//                                    break;
//                                }

//                            case 3d:
//                                {
//                                    i = 20;
//                                    break;
//                                }

//                            case 4d:
//                                {
//                                    i = 30;
//                                    break;
//                                }

//                            case 5d:
//                                {
//                                    i = 40;
//                                    break;
//                                }

//                            case 6d:
//                                {
//                                    i = 50;
//                                    break;
//                                }

//                            case 7d:
//                                {
//                                    i = 55;
//                                    break;
//                                }

//                            case 8d:
//                                {
//                                    i = 60;
//                                    break;
//                                }

//                            case 9d:
//                                {
//                                    i = 65;
//                                    break;
//                                }

//                            case var case1 when case1 >= 10d:
//                                {
//                                    i = 70;
//                                    break;
//                                }

//                            default:
//                                {
//                                    i = 0;
//                                    break;
//                                }
//                        }

//                        string argtname40 = "アビリティ";
//                        string argtname41 = "アビリティ";
//                        string argtname42 = "ＥＮ";
//                        msg = "技属性を持つ武装・" + Expression.Term(ref argtname40, ref u) + "及び必要技能が" + sname0 + "の武装・" + Expression.Term(ref argtname41, ref u) + "の消費" + Expression.Term(ref argtname42, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(i) + "%減少させる。";
//                        break;
//                    }

//                case "集中力":
//                    {
//                        string argtname43 = "スペシャルパワー";
//                        string argtname44 = "ＳＰ";
//                        msg = Expression.Term(ref argtname43, ref u) + "の" + Expression.Term(ref argtname44, ref u) + "消費量が元の80%に減少する。";
//                        break;
//                    }

//                case "闘争本能":
//                    {
//                        if (p.MinMorale > 100)
//                        {
//                            object argIndex20 = "闘争本能";
//                            if (!p.IsSkillLevelSpecified(ref argIndex20))
//                            {
//                                string argtname45 = "気力";
//                                msg = "出撃時の" + Expression.Term(ref argtname45, ref u) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.MinMorale + 5d * slevel) + "に増加する。";
//                            }
//                            else if (slevel >= 0d)
//                            {
//                                string argtname47 = "気力";
//                                msg = "出撃時の" + Expression.Term(ref argtname47, ref u) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.MinMorale + 5d * slevel) + "に増加する。";
//                            }
//                            else
//                            {
//                                string argtname46 = "気力";
//                                msg = "出撃時の" + Expression.Term(ref argtname46, ref u) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.MinMorale + 5d * slevel) + "に減少する。";
//                            }
//                        }
//                        else
//                        {
//                            object argIndex21 = "闘争本能";
//                            if (!p.IsSkillLevelSpecified(ref argIndex21))
//                            {
//                                string argtname48 = "気力";
//                                msg = "出撃時の" + Expression.Term(ref argtname48, ref u) + "が105に増加する。";
//                            }
//                            else if (slevel >= 0d)
//                            {
//                                string argtname50 = "気力";
//                                msg = "出撃時の" + Expression.Term(ref argtname50, ref u) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(100d + 5d * slevel) + "に増加する。";
//                            }
//                            else
//                            {
//                                string argtname49 = "気力";
//                                msg = "出撃時の" + Expression.Term(ref argtname49, ref u) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(100d + 5d * slevel) + "に減少する。";
//                            }
//                        }

//                        break;
//                    }

//                case "潜在力開放":
//                    {
//                        string argoname1 = "ダメージ倍率低下";
//                        if (Expression.IsOptionDefined(ref argoname1))
//                        {
//                            string argtname51 = "気力";
//                            msg = Expression.Term(ref argtname51, ref u) + "130以上で発動し、ダメージを 20% 増加させる。";
//                        }
//                        else
//                        {
//                            string argtname52 = "気力";
//                            msg = Expression.Term(ref argtname52, ref u) + "130以上で発動し、ダメージを 25% 増加させる。";
//                        }

//                        break;
//                    }

//                case "指揮":
//                    {
//                        string argtname53 = "命中";
//                        string argtname54 = "回避";
//                        msg = "半径" + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.CommandRange()), VbStrConv.Wide) + "マス以内にいる味方ザコ・汎用及び階級所有パイロットの" + Expression.Term(ref argtname53, ref u) + "・" + Expression.Term(ref argtname54, ref u);
//                        if (slevel >= 0d)
//                        {
//                            msg = msg + "に +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(5d * slevel)) + "。";
//                        }
//                        else
//                        {
//                            msg = msg + "に " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(5d * slevel)) + "。";
//                        }

//                        break;
//                    }

//                case "階級":
//                    {
//                        if (Strings.InStr(sname, "階級Lv") == 0)
//                        {
//                            msg = "階級レベル" + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)slevel), VbStrConv.Wide) + "に相当する。;";
//                        }

//                        msg = msg + "半径" + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.CommandRange()), VbStrConv.Wide) + "マス以内にいるザコ及び階級所有パイロットに指揮効果を与える。";
//                        break;
//                    }

//                case "格闘サポート":
//                    {
//                        string argtname55 = "格闘";
//                        msg = "自分がサポートパイロットの時にメインパイロットの" + Expression.Term(ref argtname55, ref u);
//                        if (slevel >= 0d)
//                        {
//                            msg = msg + "に +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(2d * slevel)) + "。";
//                        }
//                        else
//                        {
//                            msg = msg + "に " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(2d * slevel)) + "。";
//                        }

//                        break;
//                    }

//                case "射撃サポート":
//                    {
//                        string argtname56 = "射撃";
//                        msg = "自分がサポートパイロットの時にメインパイロットの" + Expression.Term(ref argtname56, ref u);
//                        if (slevel >= 0d)
//                        {
//                            msg = msg + "に +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(2d * slevel)) + "。";
//                        }
//                        else
//                        {
//                            msg = msg + "に " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(2d * slevel)) + "。";
//                        }

//                        break;
//                    }

//                case "魔力サポート":
//                    {
//                        string argtname57 = "魔力";
//                        msg = "自分がサポートパイロットの時にメインパイロットの" + Expression.Term(ref argtname57, ref u);
//                        if (slevel >= 0d)
//                        {
//                            msg = msg + "に +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(2d * slevel)) + "。";
//                        }
//                        else
//                        {
//                            msg = msg + "に " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(2d * slevel)) + "。";
//                        }

//                        break;
//                    }

//                case "命中サポート":
//                    {
//                        string argtname58 = "命中";
//                        msg = "自分がサポートパイロットの時にメインパイロットの" + Expression.Term(ref argtname58, ref u);
//                        if (slevel >= 0d)
//                        {
//                            msg = msg + "に +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(2d * slevel)) + "。";
//                        }
//                        else
//                        {
//                            msg = msg + "に " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(2d * slevel)) + "。";
//                        }

//                        break;
//                    }

//                case "回避サポート":
//                    {
//                        string argtname59 = "回避";
//                        msg = "自分がサポートパイロットの時にメインパイロットの" + Expression.Term(ref argtname59, ref u);
//                        if (slevel >= 0d)
//                        {
//                            msg = msg + "に +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(2d * slevel)) + "。";
//                        }
//                        else
//                        {
//                            msg = msg + "に " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(2d * slevel)) + "。";
//                        }

//                        break;
//                    }

//                case "技量サポート":
//                    {
//                        string argtname60 = "技量";
//                        msg = "自分がサポートパイロットの時にメインパイロットの" + Expression.Term(ref argtname60, ref u);
//                        if (slevel >= 0d)
//                        {
//                            msg = msg + "に +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(2d * slevel)) + "。";
//                        }
//                        else
//                        {
//                            msg = msg + "に " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(2d * slevel)) + "。";
//                        }

//                        break;
//                    }

//                case "反応サポート":
//                    {
//                        string argtname61 = "反応";
//                        msg = "自分がサポートパイロットの時にメインパイロットの" + Expression.Term(ref argtname61, ref u);
//                        if (slevel >= 0d)
//                        {
//                            msg = msg + "に +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(2d * slevel)) + "。";
//                        }
//                        else
//                        {
//                            msg = msg + "に " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(2d * slevel)) + "。";
//                        }

//                        break;
//                    }

//                case "サポート":
//                    {
//                        string argtname62 = "命中";
//                        string argtname63 = "回避";
//                        msg = "自分がサポートパイロットの時にメインパイロットの" + Expression.Term(ref argtname62, ref u) + "・" + Expression.Term(ref argtname63, ref u);
//                        if (slevel >= 0d)
//                        {
//                            msg = msg + "に +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(3d * slevel)) + "。";
//                        }
//                        else
//                        {
//                            msg = msg + "に " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(3d * slevel)) + "。";
//                        }

//                        break;
//                    }

//                case "広域サポート":
//                    {
//                        string argtname64 = "命中";
//                        string argtname65 = "回避";
//                        msg = "半径２マス以内にいる味方パイロットの" + Expression.Term(ref argtname64, ref u) + "・" + Expression.Term(ref argtname65, ref u);
//                        if (slevel >= 0d)
//                        {
//                            msg = msg + "に +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(5d * slevel)) + "。";
//                        }
//                        else
//                        {
//                            msg = msg + "に " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(5d * slevel)) + "。";
//                        }

//                        break;
//                    }

//                case "援護":
//                    {
//                        msg = "隣接するユニットにサポートアタックとサポートガードを" + "１ターンにそれぞれ" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)slevel) + "回行う。";
//                        break;
//                    }

//                case "援護攻撃":
//                    {
//                        msg = "隣接するユニットにサポートアタックを１ターンに" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)slevel) + "回行う。";
//                        break;
//                    }

//                case "援護防御":
//                    {
//                        msg = "隣接するユニットにサポートガードを１ターンに" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)slevel) + "回行う。";
//                        break;
//                    }

//                case "統率":
//                    {
//                        msg = "自分から攻撃をかけた場合、" + "サポートアタックが同時援護攻撃に変更される。;" + "（１ターンに " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)slevel) + "回）";
//                        break;
//                    }

//                case "チーム":
//                    {
//                        msg = sdata + "に所属する。" + "同じ" + sdata + "のユニットに対してのみ援護や指揮を行う。";
//                        break;
//                    }

//                case "カウンター":
//                    {
//                        msg = "１ターンに " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)slevel) + "回" + "反撃がカウンター攻撃になり、相手の攻撃に先制して反撃を行う。";
//                        break;
//                    }

//                case "先手必勝":
//                    {
//                        if (GeneralLib.LLength(ref sdata) == 2)
//                        {
//                            string argtname66 = "気力";
//                            msg = "パイロットの" + Expression.Term(ref argtname66, ref u) + "が" + GeneralLib.LIndex(ref sdata, 2) + "以上で発動。";
//                        }
//                        else
//                        {
//                            string argtname67 = "気力";
//                            msg = "パイロットの" + Expression.Term(ref argtname67, ref u) + "が120以上で発動。";
//                        }

//                        msg = msg + "反撃が必ずカウンター攻撃になり、相手の攻撃に先制して反撃を行う。";
//                        break;
//                    }

//                case "先読み":
//                    {
//                        msg = Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)((long)(100d * slevel) / 16L)) + "%の確率で" + "反撃がカウンター攻撃になり、相手の攻撃に先制して反撃を行う。";
//                        break;
//                    }

//                case "再攻撃":
//                    {
//                        string argtname68 = "反応";
//                        msg = "自分の攻撃の直後に " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)((long)(100d * slevel) / 16L)) + "% の確率で再攻撃を行う。" + "ただしパイロットの" + Expression.Term(ref argtname68, ref u) + "が相手を下回る場合、確率は半減。";
//                        break;
//                    }

//                case "２回行動":
//                    {
//                        msg = "１ターンに２回、行動が可能になる。";
//                        break;
//                    }

//                case "耐久":
//                    {
//                        if (slevel >= 0d)
//                        {
//                            string argtname69 = "装甲";
//                            msg = "ダメージ計算の際に" + Expression.Term(ref argtname69, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(5d * slevel)) + "%増加させる。";
//                        }
//                        else
//                        {
//                            string argtname70 = "装甲";
//                            msg = "ダメージ計算の際に" + Expression.Term(ref argtname70, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(5d * Math.Abs(slevel))) + "%減少させる。";
//                        }

//                        break;
//                    }

//                case "ＳＰ低成長":
//                    {
//                        string argtname71 = "ＳＰ";
//                        msg = "レベルアップ時の最大" + Expression.Term(ref argtname71, ref u) + "の増加量が通常の半分に減少する。";
//                        break;
//                    }

//                case "ＳＰ高成長":
//                    {
//                        string argtname72 = "ＳＰ";
//                        msg = "レベルアップ時の最大" + Expression.Term(ref argtname72, ref u) + "の増加量が通常の1.5倍に増加する。";
//                        break;
//                    }

//                case "ＳＰ回復":
//                    {
//                        string argtname73 = "ＳＰ";
//                        msg = "毎ターン" + Expression.Term(ref argtname73, ref u) + "がパイロットレベル/8+5回復する(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.Level / 8 + 5) + ")。";
//                        break;
//                    }

//                case "格闘成長":
//                    {
//                        // 攻撃力低成長オプションが指定されているかどうかで解説を変更する。
//                        string argtname74 = "格闘";
//                        msg = "レベルアップ時の" + Expression.Term(ref argtname74, ref u) + "の増加量が";
//                        string argoname2 = "攻撃力低成長";
//                        if (Expression.IsOptionDefined(ref argoname2))
//                        {
//                            msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(slevel + 0.5d) + "になる。";
//                        }
//                        else
//                        {
//                            msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(slevel + 1d) + "になる。";
//                        }

//                        break;
//                    }

//                case "射撃成長":
//                    {
//                        // 攻撃力低成長オプション、術技能の有無によってデフォルト解説を変更する。
//                        if (p.HasMana())
//                        {
//                            string argtname75 = "魔力";
//                            msg = "レベルアップ時の" + Expression.Term(ref argtname75, ref u) + "の増加量が";
//                        }
//                        else
//                        {
//                            string argtname76 = "射撃";
//                            msg = "レベルアップ時の" + Expression.Term(ref argtname76, ref u) + "の増加量が";
//                        }

//                        string argoname3 = "攻撃力低成長";
//                        if (Expression.IsOptionDefined(ref argoname3))
//                        {
//                            msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(slevel + 0.5d) + "になる。";
//                        }
//                        else
//                        {
//                            msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(slevel + 1d) + "になる。";
//                        }

//                        break;
//                    }

//                case "命中成長":
//                    {
//                        string argtname77 = "命中";
//                        msg = "レベルアップ時の" + Expression.Term(ref argtname77, ref u) + "の増加量が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(slevel + 2d) + "になる。";
//                        break;
//                    }

//                case "回避成長":
//                    {
//                        string argtname78 = "回避";
//                        msg = "レベルアップ時の" + Expression.Term(ref argtname78, ref u) + "の増加量が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(slevel + 2d) + "になる。";
//                        break;
//                    }

//                case "技量成長":
//                    {
//                        string argtname79 = "技量";
//                        msg = "レベルアップ時の" + Expression.Term(ref argtname79, ref u) + "の増加量が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(slevel + 1d) + "になる。";
//                        break;
//                    }

//                case "反応成長":
//                    {
//                        string argtname80 = "反応";
//                        msg = "レベルアップ時の" + Expression.Term(ref argtname80, ref u) + "の増加量が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(slevel + 1d) + "になる。";
//                        break;
//                    }

//                case "防御成長":
//                    {
//                        // 防御力低成長オプションが指定されているかどうかで解説を変更する。
//                        string argtname81 = "防御";
//                        msg = "レベルアップ時の" + Expression.Term(ref argtname81, ref u) + "の増加量が";
//                        string argoname4 = "防御力低成長";
//                        if (Expression.IsOptionDefined(ref argoname4))
//                        {
//                            msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(slevel + 0.5d) + "になる。";
//                        }
//                        else
//                        {
//                            msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(slevel + 1d) + "になる。";
//                        }

//                        break;
//                    }

//                case "精神統一":
//                    {
//                        string argtname82 = "ＳＰ";
//                        string argtname83 = "ＳＰ";
//                        string argtname84 = "ＳＰ";
//                        string argtname85 = "ＳＰ";
//                        msg = Expression.Term(ref argtname82, ref u) + "が最大" + Expression.Term(ref argtname83, ref u) + "の20%未満(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.MaxSP / 5) + "未満)の場合、" + "ターン開始時に" + Expression.Term(ref argtname84, ref u) + "が最大" + Expression.Term(ref argtname85, ref u) + "の10%分回復する(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.MaxSP / 10) + ")。";
//                        break;
//                    }

//                case "損傷時気力増加":
//                    {
//                        if (slevel >= -1)
//                        {
//                            string argtname86 = "気力";
//                            msg = "ダメージを受けた際に" + Expression.Term(ref argtname86, ref u) + "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(slevel + 1d)) + "。";
//                        }
//                        else
//                        {
//                            string argtname87 = "気力";
//                            msg = "ダメージを受けた際に" + Expression.Term(ref argtname87, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(slevel + 1d)) + "。";
//                        }

//                        break;
//                    }

//                case "命中時気力増加":
//                    {
//                        if (slevel >= 0d)
//                        {
//                            string argtname88 = "気力";
//                            msg = "攻撃を命中させた際に" + Expression.Term(ref argtname88, ref u) + "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)slevel) + "。(マップ攻撃は例外)";
//                        }
//                        else
//                        {
//                            string argtname89 = "気力";
//                            msg = "攻撃を命中させた際に" + Expression.Term(ref argtname89, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)slevel) + "。(マップ攻撃は例外)";
//                        }

//                        break;
//                    }

//                case "失敗時気力増加":
//                    {
//                        if (slevel >= 0d)
//                        {
//                            string argtname90 = "気力";
//                            msg = "攻撃を外してしまった際に" + Expression.Term(ref argtname90, ref u) + "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)slevel) + "。(マップ攻撃は例外)";
//                        }
//                        else
//                        {
//                            string argtname91 = "気力";
//                            msg = "攻撃を外してしまった際に" + Expression.Term(ref argtname91, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)slevel) + "。(マップ攻撃は例外)";
//                        }

//                        break;
//                    }

//                case "回避時気力増加":
//                    {
//                        if (slevel >= 0d)
//                        {
//                            string argtname92 = "気力";
//                            msg = "攻撃を回避した際に" + Expression.Term(ref argtname92, ref u) + "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)slevel) + "。";
//                        }
//                        else
//                        {
//                            string argtname93 = "気力";
//                            msg = "攻撃を回避した際に" + Expression.Term(ref argtname93, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)slevel) + "。";
//                        }

//                        break;
//                    }

//                case "起死回生":
//                    {
//                        string argtname94 = "ＳＰ";
//                        string argtname95 = "ＨＰ";
//                        string argtname96 = "ＥＮ";
//                        string argtname97 = "ＳＰ";
//                        string argtname98 = "ＨＰ";
//                        string argtname99 = "ＥＮ";
//                        msg = Expression.Term(ref argtname94, ref u) + "、" + Expression.Term(ref argtname95, ref u) + "、" + Expression.Term(ref argtname96, ref u) + "の全てが最大値の20%以下になると毎ターン最初に発動。" + Expression.Term(ref argtname97, ref u) + "、" + Expression.Term(ref argtname98, ref u) + "、" + Expression.Term(ref argtname99, ref u) + "が全快する。";
//                        break;
//                    }

//                case "戦術":
//                    {
//                        string argtname100 = "技量";
//                        msg = "思考パターン決定の際に用いられる" + Expression.Term(ref argtname100, ref u);
//                        if (slevel >= 0d)
//                        {
//                            msg = msg + "初期値がレベル×10増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(10d * slevel)) + ")。";
//                        }
//                        else
//                        {
//                            msg = msg + "初期値がレベル×10減少(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(10d * slevel)) + ")。";
//                        }

//                        break;
//                    }

//                case "得意技":
//                    {
//                        string localSkillData() { object argIndex1 = stype; var ret = p.SkillData(ref argIndex1); return ret; }

//                        string argtname101 = "アビリティ";
//                        string argtname102 = "アビリティ";
//                        msg = "「" + localSkillData() + "」属性を持つ武器・" + Expression.Term(ref argtname101, ref u) + "によるダメージ・効果量が 20% 増加。" + "また、" + Expression.Term(ref argtname102, ref u) + "の継続時間が 40% 増加。";
//                        break;
//                    }

//                case "不得手":
//                    {
//                        string localSkillData1() { object argIndex1 = stype; var ret = p.SkillData(ref argIndex1); return ret; }

//                        string argtname103 = "アビリティ";
//                        string argtname104 = "アビリティ";
//                        msg = "「" + localSkillData1() + "」属性を持つ武器・" + Expression.Term(ref argtname103, ref u) + "によるダメージ・効果量が 20% 減少。" + "また、" + Expression.Term(ref argtname104, ref u) + "の継続時間が 40% 減少。";
//                        break;
//                    }

//                case "ハンター":
//                    {
//                        msg = "ターゲットが";
//                        var loopTo = GeneralLib.LLength(ref sdata);
//                        for (i = 2; i <= loopTo; i++)
//                        {
//                            if (i == 3)
//                            {
//                                msg = msg + "や";
//                            }
//                            else if (3 > 2)
//                            {
//                                msg = msg + "、";
//                            }

//                            msg = msg + GeneralLib.LIndex(ref sdata, i);
//                        }

//                        msg = msg + "である場合、ターゲットに与えるダメージが";
//                        if (slevel >= 0d)
//                        {
//                            msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * slevel) + "%増加する。";
//                        }
//                        else
//                        {
//                            msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(-10 * slevel) + "%減少する。";
//                        }

//                        break;
//                    }

//                case "ＳＰ消費減少":
//                    {
//                        string argtname105 = "スペシャルパワー";
//                        msg = Expression.Term(ref argtname105, ref u);
//                        var loopTo1 = GeneralLib.LLength(ref sdata);
//                        for (i = 2; i <= loopTo1; i++)
//                            msg = msg + "「" + GeneralLib.LIndex(ref sdata, i) + "」";
//                        string argtname106 = "ＳＰ";
//                        msg = msg + "の" + Expression.Term(ref argtname106, ref u) + "消費量が";
//                        if (slevel >= 0d)
//                        {
//                            msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * slevel) + "%減少する。";
//                        }
//                        else
//                        {
//                            msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(-10 * slevel) + "%増加する。";
//                        }

//                        break;
//                    }

//                case "スペシャルパワー自動発動":
//                    {
//                        string argtname107 = "気力";
//                        string argtname108 = "ＳＰ";
//                        msg = Expression.Term(ref argtname107, ref u) + "が" + GeneralLib.LIndex(ref sdata, 3) + "以上で発動し、" + "毎ターン最初に「" + GeneralLib.LIndex(ref sdata, 2) + "」が自動でかかる。" + "（" + Expression.Term(ref argtname108, ref u) + "は消費しない）";
//                        break;
//                    }

//                case "修理":
//                    {
//                        string argtname109 = "アビリティ";
//                        string argtname110 = "ＨＰ";
//                        msg = "修理装置や回復" + Expression.Term(ref argtname109, ref u) + "を使った際の" + Expression.Term(ref argtname110, ref u) + "回復量が ";
//                        if (slevel >= 0d)
//                        {
//                            msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * slevel) + "% 増加する。";
//                        }
//                        else
//                        {
//                            msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(-10 * slevel) + "% 減少する。";
//                        }

//                        break;
//                    }

//                case "補給":
//                    {
//                        string argoname5 = "移動後補給不可";
//                        if (Expression.IsOptionDefined(ref argoname5))
//                        {
//                            msg = "移動後に補給装置を使用できるようになる。また、";
//                        }

//                        string argtname111 = "アビリティ";
//                        string argtname112 = "ＥＮ";
//                        msg = msg + "補給" + Expression.Term(ref argtname111, ref u) + "を使った際の" + Expression.Term(ref argtname112, ref u) + "回復量が ";
//                        if (slevel >= 0d)
//                        {
//                            msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * slevel) + "% 増加する。";
//                        }
//                        else
//                        {
//                            msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(-10 * slevel) + "% 減少する。";
//                        }

//                        break;
//                    }

//                case "気力上限":
//                    {
//                        i = 150;
//                        if (slevel != 0d)
//                        {
//                            i = (short)GeneralLib.MaxLng((int)slevel, 0);
//                        }

//                        string argtname113 = "気力";
//                        msg = Expression.Term(ref argtname113, ref u) + "の上限が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(i) + "になる。";
//                        break;
//                    }

//                case "気力下限":
//                    {
//                        i = 50;
//                        if (slevel != 0d)
//                        {
//                            i = (short)GeneralLib.MaxLng((int)slevel, 0);
//                        }

//                        string argtname114 = "気力";
//                        msg = Expression.Term(ref argtname114, ref u) + "の下限が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(i) + "になる。";
//                        break;
//                    }

//                // ADD START MARGE
//                case "遊撃":
//                    {
//                        // ADD END MARGE

//                        string argtname115 = "アビリティ";
//                        msg = "移動後使用可能な武器・" + Expression.Term(ref argtname115, ref u) + "を使った後に、残った移動力を使って移動できる。";
//                        break;
//                    }

//                default:
//                    {
//                        // ダミー能力

//                        // パイロット側で解説を定義している？
//                        object argIndex22 = sname0;
//                        sdata = p.SkillData(ref argIndex22);
//                        if (GeneralLib.ListIndex(ref sdata, 1) == "解説")
//                        {
//                            msg = GeneralLib.ListIndex(ref sdata, GeneralLib.ListLength(ref sdata));
//                            if (Strings.Left(msg, 1) == "\"")
//                            {
//                                msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
//                            }

//                            SkillHelpMessageRet = msg;
//                            return SkillHelpMessageRet;
//                        }

//                        // ユニット側で解説を定義している？
//                        var loopTo2 = u.CountFeature();
//                        for (i = 1; i <= loopTo2; i++)
//                        {
//                            object argIndex24 = i;
//                            if ((u.Feature(ref argIndex24) ?? "") == (stype ?? ""))
//                            {
//                                object argIndex23 = i;
//                                fdata = u.FeatureData(ref argIndex23);
//                                if (GeneralLib.ListIndex(ref fdata, 1) == "解説")
//                                {
//                                    msg = GeneralLib.ListIndex(ref fdata, GeneralLib.ListLength(ref fdata));
//                                }
//                            }
//                        }

//                        if (u2 is object)
//                        {
//                            var loopTo3 = u2.CountFeature();
//                            for (i = 1; i <= loopTo3; i++)
//                            {
//                                object argIndex26 = i;
//                                if ((u2.Feature(ref argIndex26) ?? "") == (stype ?? ""))
//                                {
//                                    object argIndex25 = i;
//                                    fdata = u2.FeatureData(ref argIndex25);
//                                    if (GeneralLib.ListIndex(ref fdata, 1) == "解説")
//                                    {
//                                        msg = GeneralLib.ListIndex(ref fdata, GeneralLib.ListLength(ref fdata));
//                                    }
//                                }
//                            }
//                        }

//                        if (string.IsNullOrEmpty(msg))
//                        {
//                            return SkillHelpMessageRet;
//                        }

//                        // ユニット側で解説を定義している場合
//                        if (Strings.Left(msg, 1) == "\"")
//                        {
//                            msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
//                        }

//                        break;
//                    }
//            }

//            // パイロット側で解説を定義している？
//            object argIndex27 = sname0;
//            sdata = p.SkillData(ref argIndex27);
//            if (GeneralLib.ListIndex(ref sdata, 1) == "解説")
//            {
//                msg = GeneralLib.ListIndex(ref sdata, GeneralLib.ListLength(ref sdata));
//                if (Strings.Left(msg, 1) == "\"")
//                {
//                    msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
//                }
//            }

//            // ユニット側で解説を定義している？
//            var loopTo4 = u.CountFeature();
//            for (i = 1; i <= loopTo4; i++)
//            {
//                object argIndex29 = i;
//                if ((u.Feature(ref argIndex29) ?? "") == (sname0 ?? ""))
//                {
//                    object argIndex28 = i;
//                    fdata = u.FeatureData(ref argIndex28);
//                    if (GeneralLib.ListIndex(ref fdata, 1) == "解説")
//                    {
//                        msg = GeneralLib.ListIndex(ref fdata, GeneralLib.ListLength(ref fdata));
//                        if (Strings.Left(msg, 1) == "\"")
//                        {
//                            msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
//                        }
//                    }
//                }
//            }

//            if (u2 is object)
//            {
//                var loopTo5 = u2.CountFeature();
//                for (i = 1; i <= loopTo5; i++)
//                {
//                    object argIndex31 = i;
//                    if ((u2.Feature(ref argIndex31) ?? "") == (sname0 ?? ""))
//                    {
//                        object argIndex30 = i;
//                        fdata = u2.FeatureData(ref argIndex30);
//                        if (GeneralLib.ListIndex(ref fdata, 1) == "解説")
//                        {
//                            msg = GeneralLib.ListIndex(ref fdata, GeneralLib.ListLength(ref fdata));
//                            if (Strings.Left(msg, 1) == "\"")
//                            {
//                                msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
//                            }
//                        }
//                    }
//                }
//            }

//            // 等身大基準の際は「パイロット」という語を使わないようにする
//            string argoname6 = "等身大基準";
//            if (Expression.IsOptionDefined(ref argoname6))
//            {
//                string args2 = "メインパイロット";
//                string args3 = "ユニット";
//                GeneralLib.ReplaceString(ref msg, ref args2, ref args3);
//                string args21 = "サポートパイロット";
//                string args31 = "サポート";
//                GeneralLib.ReplaceString(ref msg, ref args21, ref args31);
//                string args22 = "パイロットレベル";
//                string args32 = "レベル";
//                GeneralLib.ReplaceString(ref msg, ref args22, ref args32);
//                string args23 = "パイロット";
//                string args33 = "ユニット";
//                GeneralLib.ReplaceString(ref msg, ref args23, ref args33);
//            }

//            SkillHelpMessageRet = msg;
//            return SkillHelpMessageRet;
//        }


//        // ユニット u の findex 番目の特殊能力の解説を表示
//        public static void FeatureHelp(ref Unit u, object findex, bool is_additional)
//        {
//            string fname;
//            string msg;
//            bool prev_mode;
//            {
//                var withBlock = u;
//                // 特殊能力の名称を調べる
//                // UPGRADE_WARNING: オブジェクト findex の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
//                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(findex, "武器・防具クラス", false)))
//                {
//                    // UPGRADE_WARNING: オブジェクト findex の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
//                    fname = Conversions.ToString(findex);
//                }
//                else if (Information.IsNumeric(findex))
//                {
//                    // UPGRADE_WARNING: オブジェクト findex の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
//                    object argIndex1 = Conversions.ToShort(findex);
//                    fname = withBlock.AllFeatureName(ref argIndex1);
//                }
//                else
//                {
//                    fname = withBlock.AllFeatureName(ref findex);
//                }
//            }

//            msg = FeatureHelpMessage(ref u, findex, is_additional);

//            // 解説の表示
//            if (Strings.Len(msg) > 0)
//            {
//                prev_mode = GUI.AutoMessageMode;
//                GUI.AutoMessageMode = false;
//                Unit argu1 = null;
//                Unit argu2 = null;
//                GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
//                if (SRC.AutoMoveCursor)
//                {
//                    string argcursor_mode = "メッセージウィンドウ";
//                    GUI.MoveCursorPos(ref argcursor_mode);
//                }

//                string argpname = "システム";
//                GUI.DisplayMessage(ref argpname, "<b>" + fname + "</b>;" + msg);
//                GUI.CloseMessageForm();
//                GUI.AutoMessageMode = prev_mode;
//            }
//        }

//        // ユニット u の findex 番目の特殊能力の解説
//        public static string FeatureHelpMessage(ref Unit u, object findex, bool is_additional)
//        {
//            string FeatureHelpMessageRet = default;
//            var fid = default(short);
//            string fname, ftype, fname0;
//            string fdata = default, opt;
//            double flevel = default, lv_mod;
//            var flevel_specified = default(bool);
//            var msg = default(string);
//            short i, idx;
//            var buf = default(string);
//            var prob = default(short);
//            Pilot p;
//            string sname;
//            double slevel;
//            string uname;
//            {
//                var withBlock = u;
//                // メインパイロット
//                p = withBlock.MainPilot();

//                // 特殊能力の名称、レベル、データを調べる
//                // UPGRADE_WARNING: オブジェクト findex の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
//                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(findex, "武器・防具クラス", false)))
//                {
//                    // UPGRADE_WARNING: オブジェクト findex の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
//                    ftype = Conversions.ToString(findex);
//                    // UPGRADE_WARNING: オブジェクト findex の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
//                    fname = Conversions.ToString(findex);
//                }
//                else if (Information.IsNumeric(findex))
//                {
//                    // UPGRADE_WARNING: オブジェクト findex の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
//                    fid = Conversions.ToShort(findex);
//                    object argIndex2 = fid;
//                    ftype = withBlock.AllFeature(ref argIndex2);
//                    object argIndex3 = fid;
//                    fname = withBlock.AllFeatureName(ref argIndex3);
//                    object argIndex4 = fid;
//                    fdata = withBlock.AllFeatureData(ref argIndex4);
//                    object argIndex5 = fid;
//                    flevel = withBlock.AllFeatureLevel(ref argIndex5);
//                    object argIndex6 = fid;
//                    flevel_specified = withBlock.AllFeatureLevelSpecified(ref argIndex6);
//                }
//                else
//                {
//                    ftype = withBlock.AllFeature(ref findex);
//                    fname = withBlock.AllFeatureName(ref findex);
//                    fdata = withBlock.AllFeatureData(ref findex);
//                    flevel = withBlock.AllFeatureLevel(ref findex);
//                    flevel_specified = withBlock.AllFeatureLevelSpecified(ref findex);
//                    var loopTo = withBlock.CountFeature();
//                    for (fid = 1; fid <= loopTo; fid++)
//                    {
//                        // UPGRADE_WARNING: オブジェクト findex の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
//                        object argIndex1 = fid;
//                        if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(withBlock.AllFeature(ref argIndex1), findex, false)))
//                        {
//                            break;
//                        }
//                    }
//                }

//                if (Strings.InStr(fname, "Lv") > 0)
//                {
//                    fname0 = Strings.Left(fname, Strings.InStr(fname, "Lv") - 1);
//                }
//                else
//                {
//                    fname0 = fname;
//                }

//                // 重複可能な特殊能力の場合、レベルのみが異なる能力のレベルは累積する
//                switch (ftype ?? "")
//                {
//                    case "フィールド":
//                    case "アーマー":
//                    case "レジスト":
//                    case "攻撃回避":
//                        {
//                            var loopTo1 = u.CountAllFeature();
//                            for (i = 1; i <= loopTo1; i++)
//                            {
//                                string localAllFeature() { object argIndex1 = i; var ret = withBlock.AllFeature(ref argIndex1); return ret; }

//                                string localAllFeatureData() { object argIndex1 = i; var ret = withBlock.AllFeatureData(ref argIndex1); return ret; }

//                                if (i != fid & (localAllFeature() ?? "") == (ftype ?? "") & (localAllFeatureData() ?? "") == (fdata ?? ""))
//                                {
//                                    double localAllFeatureLevel() { object argIndex1 = i; var ret = withBlock.AllFeatureLevel(ref argIndex1); return ret; }

//                                    flevel = flevel + localAllFeatureLevel();
//                                }
//                            }

//                            break;
//                        }
//                }
//            }

//            switch (ftype ?? "")
//            {
//                case "シールド":
//                    {
//                        object argIndex7 = "Ｓ防御";
//                        sname = p.SkillName0(ref argIndex7);
//                        object argIndex8 = "Ｓ防御";
//                        string argref_mode = "";
//                        prob = (short)((long)(p.SkillLevel(ref argIndex8, ref_mode: ref argref_mode) * 100d) / 16L);
//                        msg = sname + "Lv/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で防御を行い、" + "ダメージを半減。";
//                        break;
//                    }

//                case "大型シールド":
//                    {
//                        object argIndex9 = "Ｓ防御";
//                        sname = p.SkillName0(ref argIndex9);
//                        string argsname = "Ｓ防御";
//                        if (p.IsSkillAvailable(ref argsname))
//                        {
//                            object argIndex10 = "Ｓ防御";
//                            string argref_mode1 = "";
//                            prob = (short)((long)((p.SkillLevel(ref argIndex10, ref_mode: ref argref_mode1) + 1d) * 100d) / 16L);
//                        }

//                        msg = "(" + sname + "Lv+1)/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で防御を行い、" + "ダメージを半減。";
//                        break;
//                    }

//                case "小型シールド":
//                    {
//                        object argIndex11 = "Ｓ防御";
//                        sname = p.SkillName0(ref argIndex11);
//                        object argIndex12 = "Ｓ防御";
//                        string argref_mode2 = "";
//                        prob = (short)((long)(p.SkillLevel(ref argIndex12, ref_mode: ref argref_mode2) * 100d) / 16L);
//                        msg = sname + "Lv/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で防御を行い、" + "ダメージを2/3に減少。";
//                        break;
//                    }

//                case "エネルギーシールド":
//                    {
//                        object argIndex13 = "Ｓ防御";
//                        sname = p.SkillName0(ref argIndex13);
//                        object argIndex14 = "Ｓ防御";
//                        string argref_mode3 = "";
//                        prob = (short)((long)(p.SkillLevel(ref argIndex14, ref_mode: ref argref_mode3) * 100d) / 16L);
//                        if (flevel > 0d)
//                        {
//                            msg = sname + "Lv/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で防御を行い、" + "ダメージを半減した上で更に" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(100d * flevel) + "減少。";
//                        }
//                        else
//                        {
//                            msg = sname + "Lv/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で防御を行い、" + "ダメージを半減。";
//                        }

//                        msg = msg + "発動時に5ＥＮ消費。「無」属性を持つ武器には無効。";
//                        break;
//                    }

//                case "アクティブシールド":
//                    {
//                        object argIndex15 = "Ｓ防御";
//                        sname = p.SkillName0(ref argIndex15);
//                        object argIndex16 = "Ｓ防御";
//                        string argref_mode4 = "";
//                        prob = (short)((long)(p.SkillLevel(ref argIndex16, ref_mode: ref argref_mode4) * 100d) / 16L);
//                        string argsname1 = "Ｓ防御";
//                        if (p.IsSkillAvailable(ref argsname1))
//                        {
//                            object argIndex17 = "Ｓ防御";
//                            string argref_mode5 = "";
//                            prob = (short)((long)((p.SkillLevel(ref argIndex17, ref_mode: ref argref_mode5) + 2d) * 100d) / 16L);
//                        }

//                        msg = "(" + sname + "Lv+2)/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で防御を行い、" + "ダメージを半減。";
//                        break;
//                    }

//                case "盾":
//                    {
//                        object argIndex18 = "Ｓ防御";
//                        sname = p.SkillName0(ref argIndex18);
//                        object argIndex19 = "Ｓ防御";
//                        string argref_mode6 = "";
//                        slevel = p.SkillLevel(ref argIndex19, ref_mode: ref argref_mode6);
//                        if (slevel > 0d)
//                        {
//                            slevel = 100d * slevel + 400d;
//                        }

//                        msg = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(flevel) + "回、攻撃によって貫通されるまでシールド防御を行い、" + "ダメージを減少させる(-" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)slevel) + ")。;" + "ただし攻撃側が「破」属性を持っていた場合、一度に２回分破壊される。;" + "ダメージの減少量はパイロットの" + sname + "レベルによって決まる。";
//                        break;
//                    }

//                case "バリア":
//                    {
//                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(ref fdata, 2)) & GeneralLib.LIndex(ref fdata, 2) != "全")
//                        {
//                            if (Strings.Left(GeneralLib.LIndex(ref fdata, 2), 1) == "!")
//                            {
//                                msg = "「" + Strings.Mid(GeneralLib.LIndex(ref fdata, 2), 2) + "」属性を持たない";
//                            }
//                            else
//                            {
//                                msg = "「" + GeneralLib.LIndex(ref fdata, 2) + "」属性を持つ";
//                            }
//                        }

//                        msg = msg + "ダメージ" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(1000d * flevel)) + "以下の攻撃を無効化。";
//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 3)))
//                        {
//                            int localStrToLng() { string argexpr = GeneralLib.LIndex(ref fdata, 3); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                            string argexpr = GeneralLib.LIndex(ref fdata, 3);
//                            if (GeneralLib.StrToLng(ref argexpr) > 0)
//                            {
//                                string argtname = "ＥＮ";
//                                msg = msg + ";発動時に" + GeneralLib.LIndex(ref fdata, 3) + Expression.Term(ref argtname, ref u) + "消費。";
//                            }
//                            else if (localStrToLng() < 0)
//                            {
//                                string argtname1 = "ＥＮ";
//                                msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(ref fdata, 3), 2) + Expression.Term(ref argtname1, ref u) + "増加。";
//                            }
//                        }
//                        else
//                        {
//                            msg = msg + ";発動時に10ＥＮ消費。";
//                        }

//                        string argexpr1 = GeneralLib.LIndex(ref fdata, 4);
//                        if (GeneralLib.StrToLng(ref argexpr1) > 50)
//                        {
//                            string argtname2 = "気力";
//                            msg = msg + Expression.Term(ref argtname2, ref u) + GeneralLib.LIndex(ref fdata, 4) + "以上で使用可能。";
//                        }

//                        var loopTo2 = GeneralLib.LLength(ref fdata);
//                        for (i = 5; i <= loopTo2; i++)
//                        {
//                            opt = GeneralLib.LIndex(ref fdata, i);
//                            idx = (short)Strings.InStr(opt, "*");
//                            if (idx > 0)
//                            {
//                                string argexpr2 = Strings.Mid(opt, idx + 1);
//                                lv_mod = GeneralLib.StrToDbl(ref argexpr2);
//                                opt = Strings.Left(opt, idx - 1);
//                            }
//                            else
//                            {
//                                lv_mod = -1;
//                            }

//                            switch (p.SkillType(ref opt) ?? "")
//                            {
//                                case "相殺":
//                                    {
//                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、隣接時に効果は相殺。";
//                                        break;
//                                    }

//                                case "中和":
//                                    {
//                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、" + "隣接時にレベル分だけ効果を中和。";
//                                        break;
//                                    }

//                                case "近接無効":
//                                    {
//                                        msg = msg + ";「武」「突」「接」による攻撃には無効。";
//                                        break;
//                                    }

//                                case "バリア無効化無効":
//                                    {
//                                        msg = msg + ";バリア無効化によって無効化されない。";
//                                        break;
//                                    }

//                                case "手動":
//                                    {
//                                        msg = msg + ";防御選択時にのみ発動。";
//                                        break;
//                                    }

//                                case "能力必要":
//                                    {
//                                        break;
//                                    }
//                                // スキップ
//                                case "同調率":
//                                    {
//                                        object argIndex20 = opt;
//                                        sname = p.SkillName0(ref argIndex20);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 20d;
//                                        }

//                                        if (u.SyncLevel() >= 30d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
//                                        }
//                                        else if (u.SyncLevel() > 0d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
//                                        }

//                                        break;
//                                    }

//                                case "霊力":
//                                    {
//                                        object argIndex21 = opt;
//                                        sname = p.SkillName0(ref argIndex21);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 10d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PlanaLevel()) + ")。";
//                                        break;
//                                    }

//                                case "オーラ":
//                                    {
//                                        object argIndex22 = opt;
//                                        sname = p.SkillName0(ref argIndex22);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.AuraLevel()) + ")。";
//                                        break;
//                                    }

//                                case "超能力":
//                                    {
//                                        object argIndex23 = opt;
//                                        sname = p.SkillName0(ref argIndex23);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PsychicLevel()) + ")。";
//                                        break;
//                                    }

//                                default:
//                                    {
//                                        sname = u.SkillName0(opt);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.SkillLevel(opt)) + ")。";
//                                        break;
//                                    }
//                            }
//                        }

//                        break;
//                    }

//                case "バリアシールド":
//                    {
//                        object argIndex24 = "Ｓ防御";
//                        sname = p.SkillName0(ref argIndex24);
//                        object argIndex25 = "Ｓ防御";
//                        string argref_mode7 = "";
//                        prob = (short)((long)(p.SkillLevel(ref argIndex25, ref_mode: ref argref_mode7) * 100d) / 16L);
//                        msg = sname + "Lv/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で発動し、";
//                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(ref fdata, 2)) & GeneralLib.LIndex(ref fdata, 2) != "全")
//                        {
//                            if (Strings.Left(GeneralLib.LIndex(ref fdata, 2), 1) == "!")
//                            {
//                                msg = msg + "「" + Strings.Mid(GeneralLib.LIndex(ref fdata, 2), 2) + "」属性を持たない";
//                            }
//                            else
//                            {
//                                msg = msg + "「" + GeneralLib.LIndex(ref fdata, 2) + "」属性を持つ";
//                            }
//                        }

//                        msg = msg + "ダメージ" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(1000d * flevel)) + "以下の攻撃を無効化。";
//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 3)))
//                        {
//                            int localStrToLng1() { string argexpr = GeneralLib.LIndex(ref fdata, 3); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                            string argexpr3 = GeneralLib.LIndex(ref fdata, 3);
//                            if (GeneralLib.StrToLng(ref argexpr3) > 0)
//                            {
//                                string argtname3 = "ＥＮ";
//                                msg = msg + "発動時に" + GeneralLib.LIndex(ref fdata, 3) + Expression.Term(ref argtname3, ref u) + "消費。";
//                            }
//                            else if (localStrToLng1() < 0)
//                            {
//                                string argtname4 = "ＥＮ";
//                                msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(ref fdata, 3), 2) + Expression.Term(ref argtname4, ref u) + "増加。";
//                            }
//                        }
//                        else
//                        {
//                            string argtname5 = "ＥＮ";
//                            msg = msg + "発動時に10" + Expression.Term(ref argtname5, ref u) + "消費。";
//                        }

//                        string argexpr4 = GeneralLib.LIndex(ref fdata, 4);
//                        if (GeneralLib.StrToLng(ref argexpr4) > 50)
//                        {
//                            string argtname6 = "気力";
//                            msg = msg + Expression.Term(ref argtname6, ref u) + GeneralLib.LIndex(ref fdata, 4) + "以上で使用可能。";
//                        }

//                        var loopTo3 = GeneralLib.LLength(ref fdata);
//                        for (i = 5; i <= loopTo3; i++)
//                        {
//                            opt = GeneralLib.LIndex(ref fdata, i);
//                            idx = (short)Strings.InStr(opt, "*");
//                            if (idx > 0)
//                            {
//                                string argexpr5 = Strings.Mid(opt, idx + 1);
//                                lv_mod = GeneralLib.StrToDbl(ref argexpr5);
//                                opt = Strings.Left(opt, idx - 1);
//                            }
//                            else
//                            {
//                                lv_mod = -1;
//                            }

//                            switch (p.SkillType(ref opt) ?? "")
//                            {
//                                case "相殺":
//                                    {
//                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、隣接時に効果は相殺。";
//                                        break;
//                                    }

//                                case "中和":
//                                    {
//                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、" + "隣接時にレベル分だけ効果を中和。";
//                                        break;
//                                    }

//                                case "近接無効":
//                                    {
//                                        msg = msg + ";「武」「突」「接」による攻撃には無効。";
//                                        break;
//                                    }

//                                case "バリア無効化無効":
//                                    {
//                                        msg = msg + ";バリア無効化によって無効化されない。";
//                                        break;
//                                    }

//                                case "手動":
//                                    {
//                                        msg = msg + ";防御選択時にのみ発動。";
//                                        break;
//                                    }

//                                case "能力必要":
//                                    {
//                                        break;
//                                    }
//                                // スキップ
//                                case "同調率":
//                                    {
//                                        object argIndex26 = opt;
//                                        sname = p.SkillName0(ref argIndex26);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 20d;
//                                        }

//                                        if (u.SyncLevel() >= 30d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
//                                        }
//                                        else if (u.SyncLevel() > 0d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
//                                        }

//                                        break;
//                                    }

//                                case "霊力":
//                                    {
//                                        object argIndex27 = opt;
//                                        sname = p.SkillName0(ref argIndex27);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 10d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PlanaLevel()) + ")。";
//                                        break;
//                                    }

//                                case "オーラ":
//                                    {
//                                        object argIndex28 = opt;
//                                        sname = p.SkillName0(ref argIndex28);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.AuraLevel()) + ")。";
//                                        break;
//                                    }

//                                case "超能力":
//                                    {
//                                        object argIndex29 = opt;
//                                        sname = p.SkillName0(ref argIndex29);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PsychicLevel()) + ")。";
//                                        break;
//                                    }

//                                default:
//                                    {
//                                        sname = u.SkillName0(opt);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.SkillLevel(opt)) + ")。";
//                                        break;
//                                    }
//                            }
//                        }

//                        break;
//                    }

//                case "広域バリア":
//                    {
//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)) & GeneralLib.LIndex(ref fdata, 2) != "1")
//                        {
//                            msg = "半径" + Strings.StrConv(GeneralLib.LIndex(ref fdata, 2), VbStrConv.Wide) + "マス以内の味方ユニットに対する";
//                            i = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 2));
//                        }
//                        else
//                        {
//                            msg = "隣接する味方ユニットに対する";
//                            i = 1;
//                        }

//                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(ref fdata, 3)) & GeneralLib.LIndex(ref fdata, 3) != "全")
//                        {
//                            if (Strings.Left(GeneralLib.LIndex(ref fdata, 3), 1) == "!")
//                            {
//                                msg = msg + "「" + Strings.Mid(GeneralLib.LIndex(ref fdata, 3), 2) + "」属性を持たない";
//                            }
//                            else
//                            {
//                                msg = msg + "「" + GeneralLib.LIndex(ref fdata, 3) + "」属性を持つ";
//                            }
//                        }

//                        msg = msg + "ダメージ" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(1000d * flevel)) + "以下の攻撃を無効化。";
//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 4)))
//                        {
//                            int localStrToLng2() { string argexpr = GeneralLib.LIndex(ref fdata, 4); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                            string argexpr6 = GeneralLib.LIndex(ref fdata, 4);
//                            if (GeneralLib.StrToLng(ref argexpr6) > 0)
//                            {
//                                string argtname7 = "ＥＮ";
//                                msg = msg + ";発動時に" + GeneralLib.LIndex(ref fdata, 4) + Expression.Term(ref argtname7, ref u) + "消費。";
//                            }
//                            else if (localStrToLng2() < 0)
//                            {
//                                string argtname8 = "ＥＮ";
//                                msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(ref fdata, 4), 2) + Expression.Term(ref argtname8, ref u) + "増加。";
//                            }
//                        }
//                        else
//                        {
//                            string argtname9 = "ＥＮ";
//                            msg = msg + ";発動時に" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(20 * i) + Expression.Term(ref argtname9, ref u) + "消費。";
//                        }

//                        string argexpr7 = GeneralLib.LIndex(ref fdata, 5);
//                        if (GeneralLib.StrToLng(ref argexpr7) > 50)
//                        {
//                            string argtname10 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname10, ref u) + GeneralLib.LIndex(ref fdata, 5) + "以上で使用可能。";
//                        }

//                        msg = msg + ";ただし攻撃側も有効範囲内にいる場合は無効化。";
//                        break;
//                    }

//                case "フィールド":
//                    {
//                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(ref fdata, 2)) & GeneralLib.LIndex(ref fdata, 2) != "全")
//                        {
//                            if (Strings.Left(GeneralLib.LIndex(ref fdata, 2), 1) == "!")
//                            {
//                                msg = "「" + Strings.Mid(GeneralLib.LIndex(ref fdata, 2), 2) + "」属性を持たない";
//                            }
//                            else
//                            {
//                                msg = "「" + GeneralLib.LIndex(ref fdata, 2) + "」属性を持つ";
//                            }
//                        }

//                        if (flevel >= 0d)
//                        {
//                            msg = msg + "攻撃のダメージを" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(500d * flevel)) + "減少させる。";
//                        }
//                        else
//                        {
//                            msg = msg + "攻撃のダメージを" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(-500 * flevel)) + "増加させる。";
//                        }

//                        int localStrToLng3() { string argexpr = GeneralLib.LIndex(ref fdata, 3); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                        string argexpr8 = GeneralLib.LIndex(ref fdata, 3);
//                        if (GeneralLib.StrToLng(ref argexpr8) > 0)
//                        {
//                            string argtname11 = "ＥＮ";
//                            msg = msg + ";発動時に" + GeneralLib.LIndex(ref fdata, 3) + Expression.Term(ref argtname11, ref u) + "消費。";
//                        }
//                        else if (localStrToLng3() < 0)
//                        {
//                            string argtname12 = "ＥＮ";
//                            msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(ref fdata, 3), 2) + Expression.Term(ref argtname12, ref u) + "増加。";
//                        }

//                        string argexpr9 = GeneralLib.LIndex(ref fdata, 4);
//                        if (GeneralLib.StrToLng(ref argexpr9) > 50)
//                        {
//                            string argtname13 = "気力";
//                            msg = msg + Expression.Term(ref argtname13, ref u) + GeneralLib.LIndex(ref fdata, 4) + "以上で使用可能。";
//                        }

//                        var loopTo4 = GeneralLib.LLength(ref fdata);
//                        for (i = 5; i <= loopTo4; i++)
//                        {
//                            opt = GeneralLib.LIndex(ref fdata, i);
//                            idx = (short)Strings.InStr(opt, "*");
//                            if (idx > 0)
//                            {
//                                string argexpr10 = Strings.Mid(opt, idx + 1);
//                                lv_mod = GeneralLib.StrToDbl(ref argexpr10);
//                                opt = Strings.Left(opt, idx - 1);
//                            }
//                            else
//                            {
//                                lv_mod = -1;
//                            }

//                            switch (p.SkillType(ref opt) ?? "")
//                            {
//                                case "相殺":
//                                    {
//                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、隣接時に効果は相殺。";
//                                        break;
//                                    }

//                                case "中和":
//                                    {
//                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、" + "隣接時にレベル分だけ効果を中和。";
//                                        break;
//                                    }

//                                case "近接無効":
//                                    {
//                                        msg = msg + ";「武」「突」「接」による攻撃には無効。";
//                                        break;
//                                    }

//                                case "バリア無効化無効":
//                                    {
//                                        msg = msg + ";バリア無効化によって無効化されない。";
//                                        break;
//                                    }

//                                case "手動":
//                                    {
//                                        msg = msg + ";防御選択時にのみ発動。";
//                                        break;
//                                    }

//                                case "能力必要":
//                                    {
//                                        break;
//                                    }
//                                // スキップ
//                                case "同調率":
//                                    {
//                                        object argIndex30 = opt;
//                                        sname = p.SkillName0(ref argIndex30);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 20d;
//                                        }

//                                        if (u.SyncLevel() >= 30d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
//                                        }
//                                        else if (u.SyncLevel() > 0d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
//                                        }

//                                        break;
//                                    }

//                                case "霊力":
//                                    {
//                                        object argIndex31 = opt;
//                                        sname = p.SkillName0(ref argIndex31);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 10d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PlanaLevel()) + ")。";
//                                        break;
//                                    }

//                                case "オーラ":
//                                    {
//                                        object argIndex32 = opt;
//                                        sname = p.SkillName0(ref argIndex32);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.AuraLevel()) + ")。";
//                                        break;
//                                    }

//                                case "超能力":
//                                    {
//                                        object argIndex33 = opt;
//                                        sname = p.SkillName0(ref argIndex33);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PsychicLevel()) + ")。";
//                                        break;
//                                    }

//                                default:
//                                    {
//                                        sname = u.SkillName0(opt);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.SkillLevel(opt)) + ")。";
//                                        break;
//                                    }
//                            }
//                        }

//                        break;
//                    }

//                case "アクティブフィールド":
//                    {
//                        object argIndex34 = "Ｓ防御";
//                        sname = p.SkillName0(ref argIndex34);
//                        object argIndex35 = "Ｓ防御";
//                        string argref_mode8 = "";
//                        prob = (short)((long)(p.SkillLevel(ref argIndex35, ref_mode: ref argref_mode8) * 100d) / 16L);
//                        msg = sname + "Lv/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で発動し、";
//                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(ref fdata, 2)) & GeneralLib.LIndex(ref fdata, 2) != "全")
//                        {
//                            if (Strings.Left(GeneralLib.LIndex(ref fdata, 2), 1) == "!")
//                            {
//                                msg = msg + "「" + Strings.Mid(GeneralLib.LIndex(ref fdata, 2), 2) + "」属性を持たない";
//                            }
//                            else
//                            {
//                                msg = msg + "「" + GeneralLib.LIndex(ref fdata, 2) + "」属性を持つ";
//                            }
//                        }

//                        if (flevel >= 0d)
//                        {
//                            msg = msg + "攻撃のダメージを" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(500d * flevel)) + "減少させる。";
//                        }
//                        else
//                        {
//                            msg = msg + "攻撃のダメージを" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(-500 * flevel)) + "増加させる。";
//                        }

//                        int localStrToLng4() { string argexpr = GeneralLib.LIndex(ref fdata, 3); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                        string argexpr11 = GeneralLib.LIndex(ref fdata, 3);
//                        if (GeneralLib.StrToLng(ref argexpr11) > 0)
//                        {
//                            string argtname14 = "ＥＮ";
//                            msg = msg + ";発動時に" + GeneralLib.LIndex(ref fdata, 3) + Expression.Term(ref argtname14, ref u) + "消費。";
//                        }
//                        else if (localStrToLng4() < 0)
//                        {
//                            string argtname15 = "ＥＮ";
//                            msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(ref fdata, 3), 2) + Expression.Term(ref argtname15, ref u) + "増加。";
//                        }

//                        string argexpr12 = GeneralLib.LIndex(ref fdata, 4);
//                        if (GeneralLib.StrToLng(ref argexpr12) > 50)
//                        {
//                            string argtname16 = "気力";
//                            msg = msg + Expression.Term(ref argtname16, ref u) + GeneralLib.LIndex(ref fdata, 4) + "以上で使用可能。";
//                        }

//                        var loopTo5 = GeneralLib.LLength(ref fdata);
//                        for (i = 5; i <= loopTo5; i++)
//                        {
//                            opt = GeneralLib.LIndex(ref fdata, i);
//                            idx = (short)Strings.InStr(opt, "*");
//                            if (idx > 0)
//                            {
//                                string argexpr13 = Strings.Mid(opt, idx + 1);
//                                lv_mod = GeneralLib.StrToDbl(ref argexpr13);
//                                opt = Strings.Left(opt, idx - 1);
//                            }
//                            else
//                            {
//                                lv_mod = -1;
//                            }

//                            switch (p.SkillType(ref opt) ?? "")
//                            {
//                                case "相殺":
//                                    {
//                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、隣接時に効果は相殺。";
//                                        break;
//                                    }

//                                case "中和":
//                                    {
//                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、" + "隣接時にレベル分だけ効果を中和。";
//                                        break;
//                                    }

//                                case "近接無効":
//                                    {
//                                        msg = msg + ";「武」「突」「接」による攻撃には無効。";
//                                        break;
//                                    }

//                                case "バリア無効化無効":
//                                    {
//                                        msg = msg + ";バリア無効化によって無効化されない。";
//                                        break;
//                                    }

//                                case "手動":
//                                    {
//                                        msg = msg + ";防御選択時にのみ発動。";
//                                        break;
//                                    }

//                                case "能力必要":
//                                    {
//                                        break;
//                                    }
//                                // スキップ
//                                case "同調率":
//                                    {
//                                        object argIndex36 = opt;
//                                        sname = p.SkillName0(ref argIndex36);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 20d;
//                                        }

//                                        if (u.SyncLevel() >= 30d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
//                                        }
//                                        else if (u.SyncLevel() > 0d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
//                                        }

//                                        break;
//                                    }

//                                case "霊力":
//                                    {
//                                        object argIndex37 = opt;
//                                        sname = p.SkillName0(ref argIndex37);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 10d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PlanaLevel()) + ")。";
//                                        break;
//                                    }

//                                case "オーラ":
//                                    {
//                                        object argIndex38 = opt;
//                                        sname = p.SkillName0(ref argIndex38);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.AuraLevel()) + ")。";
//                                        break;
//                                    }

//                                case "超能力":
//                                    {
//                                        object argIndex39 = opt;
//                                        sname = p.SkillName0(ref argIndex39);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PsychicLevel()) + ")。";
//                                        break;
//                                    }

//                                default:
//                                    {
//                                        sname = u.SkillName0(opt);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.SkillLevel(opt)) + ")。";
//                                        break;
//                                    }
//                            }
//                        }

//                        break;
//                    }

//                case "広域フィールド":
//                    {
//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)) & GeneralLib.LIndex(ref fdata, 2) != "1")
//                        {
//                            msg = "半径" + Strings.StrConv(GeneralLib.LIndex(ref fdata, 2), VbStrConv.Wide) + "マス以内の味方ユニットに対する";
//                            i = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 2));
//                        }
//                        else
//                        {
//                            msg = "隣接する味方ユニットに対する";
//                            i = 1;
//                        }

//                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(ref fdata, 3)) & GeneralLib.LIndex(ref fdata, 3) != "全")
//                        {
//                            if (Strings.Left(GeneralLib.LIndex(ref fdata, 3), 1) == "!")
//                            {
//                                msg = msg + "「" + Strings.Mid(GeneralLib.LIndex(ref fdata, 3), 2) + "」属性を持たない";
//                            }
//                            else
//                            {
//                                msg = msg + "「" + GeneralLib.LIndex(ref fdata, 3) + "」属性を持つ";
//                            }
//                        }

//                        if (flevel >= 0d)
//                        {
//                            msg = msg + "攻撃のダメージを" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(500d * flevel)) + "減少させる。";
//                        }
//                        else
//                        {
//                            msg = msg + "攻撃のダメージを" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(-500 * flevel)) + "増加させる。";
//                        }

//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 4)))
//                        {
//                            int localStrToLng5() { string argexpr = GeneralLib.LIndex(ref fdata, 4); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                            string argexpr14 = GeneralLib.LIndex(ref fdata, 4);
//                            if (GeneralLib.StrToLng(ref argexpr14) > 0)
//                            {
//                                string argtname17 = "ＥＮ";
//                                msg = msg + ";発動時に" + GeneralLib.LIndex(ref fdata, 4) + Expression.Term(ref argtname17, ref u) + "消費。";
//                            }
//                            else if (localStrToLng5() < 0)
//                            {
//                                string argtname18 = "ＥＮ";
//                                msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(ref fdata, 4), 2) + Expression.Term(ref argtname18, ref u) + "増加。";
//                            }
//                        }
//                        else
//                        {
//                            string argtname19 = "ＥＮ";
//                            msg = msg + ";発動時に" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(20 * i) + Expression.Term(ref argtname19, ref u) + "消費。";
//                        }

//                        string argexpr15 = GeneralLib.LIndex(ref fdata, 5);
//                        if (GeneralLib.StrToLng(ref argexpr15) > 50)
//                        {
//                            string argtname20 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname20, ref u) + GeneralLib.LIndex(ref fdata, 5) + "以上で使用可能。";
//                        }

//                        msg = msg + ";ただし攻撃側も有効範囲内にいる場合は無効化。";
//                        break;
//                    }

//                case "プロテクション":
//                    {
//                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(ref fdata, 2)) & GeneralLib.LIndex(ref fdata, 2) != "全")
//                        {
//                            if (Strings.Left(GeneralLib.LIndex(ref fdata, 2), 1) == "!")
//                            {
//                                msg = "「" + Strings.Mid(GeneralLib.LIndex(ref fdata, 2), 2) + "」属性を持たない";
//                            }
//                            else
//                            {
//                                msg = "「" + GeneralLib.LIndex(ref fdata, 2) + "」属性を持つ";
//                            }
//                        }

//                        if (flevel > 10d)
//                        {
//                            msg = msg + "攻撃のダメージを" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(10d * flevel - 100d)) + "%吸収する。";
//                        }
//                        else if (flevel >= 0d)
//                        {
//                            msg = msg + "攻撃のダメージを" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(10d * flevel)) + "%減少させる。";
//                        }
//                        else
//                        {
//                            msg = msg + "攻撃のダメージを" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(-10 * flevel)) + "%増加させる。";
//                        }

//                        int localStrToLng6() { string argexpr = GeneralLib.LIndex(ref fdata, 3); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                        int localStrToLng7() { string argexpr = GeneralLib.LIndex(ref fdata, 3); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                        if (!Information.IsNumeric(GeneralLib.LIndex(ref fdata, 3)))
//                        {
//                            string argtname21 = "ＥＮ";
//                            msg = msg + ";発動時に10" + Expression.Term(ref argtname21, ref u) + "増加。";
//                        }
//                        else if (localStrToLng6() > 0)
//                        {
//                            string argtname22 = "ＥＮ";
//                            msg = msg + ";発動時に" + GeneralLib.LIndex(ref fdata, 3) + Expression.Term(ref argtname22, ref u) + "消費。";
//                        }
//                        else if (localStrToLng7() < 0)
//                        {
//                            string argtname23 = "ＥＮ";
//                            msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(ref fdata, 3), 2) + Expression.Term(ref argtname23, ref u) + "増加。";
//                        }

//                        string argexpr16 = GeneralLib.LIndex(ref fdata, 4);
//                        if (GeneralLib.StrToLng(ref argexpr16) > 50)
//                        {
//                            string argtname24 = "気力";
//                            msg = msg + Expression.Term(ref argtname24, ref u) + GeneralLib.LIndex(ref fdata, 4) + "以上で使用可能。";
//                        }

//                        var loopTo6 = GeneralLib.LLength(ref fdata);
//                        for (i = 5; i <= loopTo6; i++)
//                        {
//                            opt = GeneralLib.LIndex(ref fdata, i);
//                            idx = (short)Strings.InStr(opt, "*");
//                            if (idx > 0)
//                            {
//                                string argexpr17 = Strings.Mid(opt, idx + 1);
//                                lv_mod = GeneralLib.StrToDbl(ref argexpr17);
//                                opt = Strings.Left(opt, idx - 1);
//                            }
//                            else
//                            {
//                                lv_mod = -1;
//                            }

//                            switch (p.SkillType(ref opt) ?? "")
//                            {
//                                case "相殺":
//                                    {
//                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、隣接時に効果は相殺。";
//                                        break;
//                                    }

//                                case "中和":
//                                    {
//                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、" + "隣接時にレベル分だけ効果を中和。";
//                                        break;
//                                    }

//                                case "近接無効":
//                                    {
//                                        msg = msg + ";「武」「突」「接」による攻撃には無効。";
//                                        break;
//                                    }

//                                case "バリア無効化無効":
//                                    {
//                                        msg = msg + ";バリア無効化によって無効化されない。";
//                                        break;
//                                    }

//                                case "手動":
//                                    {
//                                        msg = msg + ";防御選択時にのみ発動。";
//                                        break;
//                                    }

//                                case "能力必要":
//                                    {
//                                        break;
//                                    }
//                                // スキップ
//                                case "同調率":
//                                    {
//                                        object argIndex40 = opt;
//                                        sname = p.SkillName0(ref argIndex40);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 0.5d;
//                                        }

//                                        if (u.SyncLevel() >= 30d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
//                                        }
//                                        else if (u.SyncLevel() > 0d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
//                                        }

//                                        break;
//                                    }

//                                case "霊力":
//                                    {
//                                        object argIndex41 = opt;
//                                        sname = p.SkillName0(ref argIndex41);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 0.2d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PlanaLevel()) + ")。";
//                                        break;
//                                    }

//                                case "オーラ":
//                                    {
//                                        object argIndex42 = opt;
//                                        sname = p.SkillName0(ref argIndex42);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 5d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.AuraLevel()) + ")。";
//                                        break;
//                                    }

//                                case "超能力":
//                                    {
//                                        object argIndex43 = opt;
//                                        sname = p.SkillName0(ref argIndex43);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 5d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PsychicLevel()) + ")。";
//                                        break;
//                                    }

//                                default:
//                                    {
//                                        sname = u.SkillName0(opt);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 5d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.SkillLevel(opt)) + ")。";
//                                        break;
//                                    }
//                            }
//                        }

//                        break;
//                    }

//                case "アクティブプロテクション":
//                    {
//                        object argIndex44 = "Ｓ防御";
//                        sname = p.SkillName0(ref argIndex44);
//                        object argIndex45 = "Ｓ防御";
//                        string argref_mode9 = "";
//                        prob = (short)((long)(p.SkillLevel(ref argIndex45, ref_mode: ref argref_mode9) * 100d) / 16L);
//                        msg = sname + "Lv/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で発動し、";
//                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(ref fdata, 2)) & GeneralLib.LIndex(ref fdata, 2) != "全")
//                        {
//                            if (Strings.Left(GeneralLib.LIndex(ref fdata, 2), 1) == "!")
//                            {
//                                msg = msg + "「" + Strings.Mid(GeneralLib.LIndex(ref fdata, 2), 2) + "」属性を持たない";
//                            }
//                            else
//                            {
//                                msg = msg + "「" + GeneralLib.LIndex(ref fdata, 2) + "」属性を持つ";
//                            }
//                        }

//                        if (flevel > 10d)
//                        {
//                            msg = msg + "攻撃のダメージを" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(10d * flevel - 100d)) + "%吸収する。";
//                        }
//                        else if (flevel >= 0d)
//                        {
//                            msg = msg + "攻撃のダメージを" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(10d * flevel)) + "%減少させる。";
//                        }
//                        else
//                        {
//                            msg = msg + "攻撃のダメージを" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(-10 * flevel)) + "%増加させる。";
//                        }

//                        int localStrToLng8() { string argexpr = GeneralLib.LIndex(ref fdata, 3); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                        int localStrToLng9() { string argexpr = GeneralLib.LIndex(ref fdata, 3); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                        if (!Information.IsNumeric(GeneralLib.LIndex(ref fdata, 3)))
//                        {
//                            string argtname25 = "ＥＮ";
//                            msg = msg + ";発動時に10" + Expression.Term(ref argtname25, ref u) + "増加。";
//                        }
//                        else if (localStrToLng8() > 0)
//                        {
//                            string argtname26 = "ＥＮ";
//                            msg = msg + ";発動時に" + GeneralLib.LIndex(ref fdata, 3) + Expression.Term(ref argtname26, ref u) + "消費。";
//                        }
//                        else if (localStrToLng9() < 0)
//                        {
//                            string argtname27 = "ＥＮ";
//                            msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(ref fdata, 3), 2) + Expression.Term(ref argtname27, ref u) + "増加。";
//                        }

//                        string argexpr18 = GeneralLib.LIndex(ref fdata, 4);
//                        if (GeneralLib.StrToLng(ref argexpr18) > 50)
//                        {
//                            string argtname28 = "気力";
//                            msg = msg + Expression.Term(ref argtname28, ref u) + GeneralLib.LIndex(ref fdata, 4) + "以上で使用可能。";
//                        }

//                        var loopTo7 = GeneralLib.LLength(ref fdata);
//                        for (i = 5; i <= loopTo7; i++)
//                        {
//                            opt = GeneralLib.LIndex(ref fdata, i);
//                            idx = (short)Strings.InStr(opt, "*");
//                            if (idx > 0)
//                            {
//                                string argexpr19 = Strings.Mid(opt, idx + 1);
//                                lv_mod = GeneralLib.StrToDbl(ref argexpr19);
//                                opt = Strings.Left(opt, idx - 1);
//                            }
//                            else
//                            {
//                                lv_mod = -1;
//                            }

//                            switch (p.SkillType(ref opt) ?? "")
//                            {
//                                case "相殺":
//                                    {
//                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、隣接時に効果は相殺。";
//                                        break;
//                                    }

//                                case "中和":
//                                    {
//                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、" + "隣接時にレベル分だけ効果を中和。";
//                                        break;
//                                    }

//                                case "近接無効":
//                                    {
//                                        msg = msg + ";「武」「突」「接」による攻撃には無効。";
//                                        break;
//                                    }

//                                case "バリア無効化無効":
//                                    {
//                                        msg = msg + ";バリア無効化によって無効化されない。";
//                                        break;
//                                    }

//                                case "手動":
//                                    {
//                                        msg = msg + ";防御選択時にのみ発動。";
//                                        break;
//                                    }

//                                case "能力必要":
//                                    {
//                                        break;
//                                    }
//                                // スキップ
//                                case "同調率":
//                                    {
//                                        object argIndex46 = opt;
//                                        sname = p.SkillName0(ref argIndex46);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 0.5d;
//                                        }

//                                        if (u.SyncLevel() >= 30d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + "%)。";
//                                        }
//                                        else if (u.SyncLevel() > 0d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + "%)。";
//                                        }

//                                        break;
//                                    }

//                                case "霊力":
//                                    {
//                                        object argIndex47 = opt;
//                                        sname = p.SkillName0(ref argIndex47);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 0.2d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PlanaLevel()) + "%)。";
//                                        break;
//                                    }

//                                case "オーラ":
//                                    {
//                                        object argIndex48 = opt;
//                                        sname = p.SkillName0(ref argIndex48);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 5d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.AuraLevel()) + "%)。";
//                                        break;
//                                    }

//                                case "超能力":
//                                    {
//                                        object argIndex49 = opt;
//                                        sname = p.SkillName0(ref argIndex49);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 5d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PsychicLevel()) + "%)。";
//                                        break;
//                                    }

//                                default:
//                                    {
//                                        sname = u.SkillName0(opt);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 5d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.SkillLevel(opt)) + "%)。";
//                                        break;
//                                    }
//                            }
//                        }

//                        break;
//                    }

//                case "広域プロテクション":
//                    {
//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)) & GeneralLib.LIndex(ref fdata, 2) != "1")
//                        {
//                            msg = "半径" + Strings.StrConv(GeneralLib.LIndex(ref fdata, 2), VbStrConv.Wide) + "マス以内の味方ユニットに対する";
//                            i = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 2));
//                        }
//                        else
//                        {
//                            msg = "隣接する味方ユニットに対する";
//                            i = 1;
//                        }

//                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(ref fdata, 3)) & GeneralLib.LIndex(ref fdata, 3) != "全")
//                        {
//                            if (Strings.Left(GeneralLib.LIndex(ref fdata, 3), 1) == "!")
//                            {
//                                msg = msg + "「" + Strings.Mid(GeneralLib.LIndex(ref fdata, 3), 2) + "」属性を持たない";
//                            }
//                            else
//                            {
//                                msg = msg + "「" + GeneralLib.LIndex(ref fdata, 3) + "」属性を持つ";
//                            }
//                        }

//                        if (flevel > 10d)
//                        {
//                            msg = msg + "攻撃のダメージを" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(10d * flevel - 100d)) + "%吸収する。";
//                        }
//                        else if (flevel >= 0d)
//                        {
//                            msg = msg + "攻撃のダメージを" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(10d * flevel)) + "%減少させる。";
//                        }
//                        else
//                        {
//                            msg = msg + "攻撃のダメージを" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(-10 * flevel)) + "%増加させる。";
//                        }

//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 4)))
//                        {
//                            int localStrToLng10() { string argexpr = GeneralLib.LIndex(ref fdata, 4); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                            string argexpr20 = GeneralLib.LIndex(ref fdata, 4);
//                            if (GeneralLib.StrToLng(ref argexpr20) > 0)
//                            {
//                                string argtname29 = "ＥＮ";
//                                msg = msg + ";発動時に" + GeneralLib.LIndex(ref fdata, 4) + Expression.Term(ref argtname29, ref u) + "消費。";
//                            }
//                            else if (localStrToLng10() < 0)
//                            {
//                                string argtname30 = "ＥＮ";
//                                msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(ref fdata, 4), 2) + Expression.Term(ref argtname30, ref u) + "増加。";
//                            }
//                        }
//                        else
//                        {
//                            string argtname31 = "ＥＮ";
//                            msg = msg + ";発動時に" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(20 * i) + Expression.Term(ref argtname31, ref u) + "消費。";
//                        }

//                        string argexpr21 = GeneralLib.LIndex(ref fdata, 5);
//                        if (GeneralLib.StrToLng(ref argexpr21) > 50)
//                        {
//                            string argtname32 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname32, ref u) + GeneralLib.LIndex(ref fdata, 5) + "以上で使用可能。";
//                        }

//                        msg = msg + ";ただし攻撃側も有効範囲内にいる場合は無効化。";
//                        break;
//                    }

//                case "アーマー":
//                    {
//                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(ref fdata, 2)) & GeneralLib.LIndex(ref fdata, 2) != "全")
//                        {
//                            if (Strings.Left(GeneralLib.LIndex(ref fdata, 2), 1) == "!")
//                            {
//                                msg = "「" + Strings.Mid(GeneralLib.LIndex(ref fdata, 2), 2) + "」属性を持たない";
//                            }
//                            else
//                            {
//                                msg = "「" + GeneralLib.LIndex(ref fdata, 2) + "」属性を持つ";
//                            }
//                        }

//                        if (flevel >= 0d)
//                        {
//                            msg = msg + "攻撃に対して装甲を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(100d * flevel)) + "増加させる。";
//                        }
//                        else
//                        {
//                            msg = msg + "攻撃に対して装甲を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(-100 * flevel)) + "減少させる。";
//                        }

//                        string argexpr22 = GeneralLib.LIndex(ref fdata, 3);
//                        if (GeneralLib.StrToLng(ref argexpr22) > 50)
//                        {
//                            string argtname33 = "気力";
//                            msg = msg + Expression.Term(ref argtname33, ref u) + GeneralLib.LIndex(ref fdata, 3) + "以上で使用可能。";
//                        }

//                        var loopTo8 = GeneralLib.LLength(ref fdata);
//                        for (i = 4; i <= loopTo8; i++)
//                        {
//                            opt = GeneralLib.LIndex(ref fdata, i);
//                            idx = (short)Strings.InStr(opt, "*");
//                            if (idx > 0)
//                            {
//                                string argexpr23 = Strings.Mid(opt, idx + 1);
//                                lv_mod = GeneralLib.StrToDbl(ref argexpr23);
//                                opt = Strings.Left(opt, idx - 1);
//                            }
//                            else
//                            {
//                                lv_mod = -1;
//                            }

//                            switch (p.SkillType(ref opt) ?? "")
//                            {
//                                case "能力必要":
//                                    {
//                                        break;
//                                    }
//                                // スキップ
//                                case "同調率":
//                                    {
//                                        object argIndex50 = opt;
//                                        sname = p.SkillName0(ref argIndex50);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 5d;
//                                        }

//                                        if (u.SyncLevel() >= 30d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
//                                        }
//                                        else if (u.SyncLevel() > 0d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
//                                        }

//                                        break;
//                                    }

//                                case "霊力":
//                                    {
//                                        object argIndex51 = opt;
//                                        sname = p.SkillName0(ref argIndex51);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 2d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PlanaLevel()) + ")。";
//                                        break;
//                                    }

//                                case "オーラ":
//                                    {
//                                        object argIndex52 = opt;
//                                        sname = p.SkillName0(ref argIndex52);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 50d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.AuraLevel()) + ")。";
//                                        break;
//                                    }

//                                case "超能力":
//                                    {
//                                        object argIndex53 = opt;
//                                        sname = p.SkillName0(ref argIndex53);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 50d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PsychicLevel()) + ")。";
//                                        break;
//                                    }

//                                default:
//                                    {
//                                        sname = u.SkillName0(opt);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 50d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.SkillLevel(opt)) + ")。";
//                                        break;
//                                    }
//                            }
//                        }

//                        break;
//                    }

//                case "レジスト":
//                    {
//                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(ref fdata, 2)) & GeneralLib.LIndex(ref fdata, 2) != "全")
//                        {
//                            if (Strings.Left(GeneralLib.LIndex(ref fdata, 2), 1) == "!")
//                            {
//                                msg = "「" + Strings.Mid(GeneralLib.LIndex(ref fdata, 2), 2) + "」属性を持たない";
//                            }
//                            else
//                            {
//                                msg = "「" + GeneralLib.LIndex(ref fdata, 2) + "」属性を持つ";
//                            }
//                        }

//                        if (flevel > 10d)
//                        {
//                            msg = msg + "攻撃に対してダメージを" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(100 - (int)(10d * flevel)) + "%吸収する。";
//                        }
//                        else if (flevel >= 0d)
//                        {
//                            msg = msg + "攻撃に対してダメージを" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(10d * flevel)) + "%軽減させる。";
//                        }
//                        else
//                        {
//                            msg = msg + "攻撃に対してダメージを" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(-10 * flevel)) + "%増加させる。";
//                        }

//                        string argexpr24 = GeneralLib.LIndex(ref fdata, 3);
//                        if (GeneralLib.StrToLng(ref argexpr24) > 50)
//                        {
//                            string argtname34 = "気力";
//                            msg = msg + Expression.Term(ref argtname34, ref u) + GeneralLib.LIndex(ref fdata, 3) + "以上で使用可能。";
//                        }

//                        var loopTo9 = GeneralLib.LLength(ref fdata);
//                        for (i = 4; i <= loopTo9; i++)
//                        {
//                            opt = GeneralLib.LIndex(ref fdata, i);
//                            idx = (short)Strings.InStr(opt, "*");
//                            if (idx > 0)
//                            {
//                                string argexpr25 = Strings.Mid(opt, idx + 1);
//                                lv_mod = GeneralLib.StrToDbl(ref argexpr25);
//                                opt = Strings.Left(opt, idx - 1);
//                            }
//                            else
//                            {
//                                lv_mod = -1;
//                            }

//                            switch (p.SkillType(ref opt) ?? "")
//                            {
//                                case "能力必要":
//                                    {
//                                        break;
//                                    }
//                                // スキップ
//                                case "同調率":
//                                    {
//                                        object argIndex54 = opt;
//                                        sname = p.SkillName0(ref argIndex54);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 5d;
//                                        }

//                                        if (u.SyncLevel() >= 30d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + "%)。";
//                                        }
//                                        else if (u.SyncLevel() > 0d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + "%)。";
//                                        }

//                                        break;
//                                    }

//                                case "霊力":
//                                    {
//                                        object argIndex55 = opt;
//                                        sname = p.SkillName0(ref argIndex55);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 2d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PlanaLevel()) + "%)。";
//                                        break;
//                                    }

//                                case "オーラ":
//                                    {
//                                        object argIndex56 = opt;
//                                        sname = p.SkillName0(ref argIndex56);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 50d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.AuraLevel()) + "%)。";
//                                        break;
//                                    }

//                                case "超能力":
//                                    {
//                                        object argIndex57 = opt;
//                                        sname = p.SkillName0(ref argIndex57);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 50d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PsychicLevel()) + "%)。";
//                                        break;
//                                    }

//                                default:
//                                    {
//                                        sname = u.SkillName0(opt);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 50d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.SkillLevel(opt)) + "%)。";
//                                        break;
//                                    }
//                            }
//                        }

//                        break;
//                    }

//                case "当て身技":
//                    {
//                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(ref fdata, 3)) & GeneralLib.LIndex(ref fdata, 3) != "全")
//                        {
//                            if (Strings.Left(GeneralLib.LIndex(ref fdata, 3), 1) == "!")
//                            {
//                                msg = "「" + Strings.Mid(GeneralLib.LIndex(ref fdata, 3), 2) + "」属性を持たない";
//                            }
//                            else
//                            {
//                                msg = "「" + GeneralLib.LIndex(ref fdata, 3) + "」属性を持つ";
//                            }
//                        }

//                        if (flevel != 1d)
//                        {
//                            msg = msg + "ダメージ" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(500d * flevel)) + "までの";
//                        }

//                        msg = msg + "攻撃を";
//                        buf = GeneralLib.LIndex(ref fdata, 4);
//                        if (Information.IsNumeric(buf))
//                        {
//                            if (buf != "100")
//                            {
//                                msg = msg + buf + "%の確率で受け止め、";
//                            }
//                            else
//                            {
//                                msg = msg + "受け止め、";
//                            }
//                        }
//                        else if (Strings.InStr(buf, "+") > 0 | Strings.InStr(buf, "-") > 0)
//                        {
//                            i = (short)GeneralLib.MaxLng(Strings.InStr(buf, "+"), Strings.InStr(buf, "-"));
//                            sname = u.SkillName0(Strings.Left(buf, i - 1));
//                            prob = (short)((long)((u.SkillLevel(Strings.Left(buf, i - 1)) + Conversions.ToShort(Strings.Mid(buf, i))) * 100d) / 16L);
//                            msg = msg + "(" + sname + "Lv" + Strings.Mid(buf, i) + ")/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で受け止め、";
//                        }
//                        else
//                        {
//                            sname = u.SkillName0(buf);
//                            prob = (short)((long)(u.SkillLevel(buf) * 100d) / 16L);
//                            msg = msg + sname + "Lv/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で受け止め、";
//                        }

//                        buf = GeneralLib.LIndex(ref fdata, 2);
//                        if (Strings.InStr(buf, "(") > 0)
//                        {
//                            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
//                        }

//                        msg = msg + buf + "で反撃。";
//                        int localStrToLng11() { string argexpr = GeneralLib.LIndex(ref fdata, 5); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                        string argexpr26 = GeneralLib.LIndex(ref fdata, 5);
//                        if (GeneralLib.StrToLng(ref argexpr26) > 0)
//                        {
//                            string argtname35 = "ＥＮ";
//                            msg = msg + ";発動時に" + GeneralLib.LIndex(ref fdata, 5) + Expression.Term(ref argtname35, ref u) + "消費。";
//                        }
//                        else if (localStrToLng11() < 0)
//                        {
//                            string argtname36 = "ＥＮ";
//                            msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(ref fdata, 5), 2) + Expression.Term(ref argtname36, ref u) + "増加。";
//                        }

//                        string argexpr27 = GeneralLib.LIndex(ref fdata, 6);
//                        if (GeneralLib.StrToLng(ref argexpr27) > 50)
//                        {
//                            string argtname37 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname37, ref u) + GeneralLib.LIndex(ref fdata, 6) + "以上で使用可能。";
//                        }

//                        var loopTo10 = GeneralLib.LLength(ref fdata);
//                        for (i = 7; i <= loopTo10; i++)
//                        {
//                            opt = GeneralLib.LIndex(ref fdata, i);
//                            idx = (short)Strings.InStr(opt, "*");
//                            if (idx > 0)
//                            {
//                                string argexpr28 = Strings.Mid(opt, idx + 1);
//                                lv_mod = GeneralLib.StrToDbl(ref argexpr28);
//                                opt = Strings.Left(opt, idx - 1);
//                            }
//                            else
//                            {
//                                lv_mod = -1;
//                            }

//                            switch (p.SkillType(ref opt) ?? "")
//                            {
//                                case "相殺":
//                                    {
//                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、隣接時に効果は相殺。";
//                                        break;
//                                    }

//                                case "中和":
//                                    {
//                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、" + "隣接時にレベル分だけ効果を相殺。";
//                                        break;
//                                    }

//                                case "近接無効":
//                                    {
//                                        msg = msg + ";「武」「突」「接」による攻撃には無効。";
//                                        break;
//                                    }

//                                case "手動":
//                                    {
//                                        msg = msg + ";防御選択時にのみ発動。";
//                                        break;
//                                    }

//                                case "能力必要":
//                                    {
//                                        break;
//                                    }
//                                // スキップ
//                                case "同調率":
//                                    {
//                                        object argIndex58 = opt;
//                                        sname = p.SkillName0(ref argIndex58);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 20d;
//                                        }

//                                        if (u.SyncLevel() >= 30d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
//                                        }
//                                        else if (u.SyncLevel() > 0d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
//                                        }

//                                        break;
//                                    }

//                                case "霊力":
//                                    {
//                                        object argIndex59 = opt;
//                                        sname = p.SkillName0(ref argIndex59);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 10d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PlanaLevel()) + ")。";
//                                        break;
//                                    }

//                                case "オーラ":
//                                    {
//                                        object argIndex60 = opt;
//                                        sname = p.SkillName0(ref argIndex60);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.AuraLevel()) + ")。";
//                                        break;
//                                    }

//                                case "超能力":
//                                    {
//                                        object argIndex61 = opt;
//                                        sname = p.SkillName0(ref argIndex61);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PsychicLevel()) + ")。";
//                                        break;
//                                    }

//                                default:
//                                    {
//                                        sname = u.SkillName0(opt);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.SkillLevel(opt)) + ")。";
//                                        break;
//                                    }
//                            }
//                        }

//                        break;
//                    }

//                case "反射":
//                    {
//                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(ref fdata, 2)) & GeneralLib.LIndex(ref fdata, 2) != "全")
//                        {
//                            if (Strings.Left(GeneralLib.LIndex(ref fdata, 2), 1) == "!")
//                            {
//                                msg = "「" + Strings.Mid(GeneralLib.LIndex(ref fdata, 2), 2) + "」属性を持たない";
//                            }
//                            else
//                            {
//                                msg = "「" + GeneralLib.LIndex(ref fdata, 2) + "」属性を持つ";
//                            }
//                        }

//                        if (flevel != 1d)
//                        {
//                            msg = msg + "ダメージ" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(500d * flevel)) + "までの";
//                        }

//                        msg = msg + "攻撃を";
//                        buf = GeneralLib.LIndex(ref fdata, 3);
//                        if (Information.IsNumeric(buf))
//                        {
//                            if (buf != "100")
//                            {
//                                msg = msg + buf + "%の確率で反射。";
//                            }
//                            else
//                            {
//                                msg = msg + "反射。";
//                            }
//                        }
//                        else if (Strings.InStr(buf, "+") > 0 | Strings.InStr(buf, "-") > 0)
//                        {
//                            i = (short)GeneralLib.MaxLng(Strings.InStr(buf, "+"), Strings.InStr(buf, "-"));
//                            sname = u.SkillName0(Strings.Left(buf, i - 1));
//                            prob = (short)((long)((u.SkillLevel(Strings.Left(buf, i - 1)) + Conversions.ToShort(Strings.Mid(buf, i))) * 100d) / 16L);
//                            msg = msg + "(" + sname + "Lv" + Strings.Mid(buf, i) + ")/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で反射。";
//                        }
//                        else
//                        {
//                            sname = u.SkillName0(buf);
//                            prob = (short)((long)(u.SkillLevel(buf) * 100d) / 16L);
//                            msg = msg + sname + "Lv/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で反射。";
//                        }

//                        int localStrToLng12() { string argexpr = GeneralLib.LIndex(ref fdata, 4); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                        string argexpr29 = GeneralLib.LIndex(ref fdata, 4);
//                        if (GeneralLib.StrToLng(ref argexpr29) > 0)
//                        {
//                            string argtname38 = "ＥＮ";
//                            msg = msg + ";発動時に" + GeneralLib.LIndex(ref fdata, 4) + Expression.Term(ref argtname38, ref u) + "消費。";
//                        }
//                        else if (localStrToLng12() < 0)
//                        {
//                            string argtname39 = "ＥＮ";
//                            msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(ref fdata, 4), 2) + Expression.Term(ref argtname39, ref u) + "増加。";
//                        }

//                        string argexpr30 = GeneralLib.LIndex(ref fdata, 5);
//                        if (GeneralLib.StrToLng(ref argexpr30) > 50)
//                        {
//                            string argtname40 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname40, ref u) + GeneralLib.LIndex(ref fdata, 5) + "以上で使用可能。";
//                        }

//                        var loopTo11 = GeneralLib.LLength(ref fdata);
//                        for (i = 6; i <= loopTo11; i++)
//                        {
//                            opt = GeneralLib.LIndex(ref fdata, i);
//                            idx = (short)Strings.InStr(opt, "*");
//                            if (idx > 0)
//                            {
//                                string argexpr31 = Strings.Mid(opt, idx + 1);
//                                lv_mod = GeneralLib.StrToDbl(ref argexpr31);
//                                opt = Strings.Left(opt, idx - 1);
//                            }
//                            else
//                            {
//                                lv_mod = -1;
//                            }

//                            switch (p.SkillType(ref opt) ?? "")
//                            {
//                                case "相殺":
//                                    {
//                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、隣接時に効果は相殺。";
//                                        break;
//                                    }

//                                case "中和":
//                                    {
//                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、" + "隣接時にレベル分だけ効果を中和。";
//                                        break;
//                                    }

//                                case "近接無効":
//                                    {
//                                        msg = msg + ";「武」「突」「接」による攻撃には無効。";
//                                        break;
//                                    }

//                                case "手動":
//                                    {
//                                        msg = msg + ";防御選択時にのみ発動。";
//                                        break;
//                                    }

//                                case "能力必要":
//                                    {
//                                        break;
//                                    }
//                                // スキップ
//                                case "同調率":
//                                    {
//                                        object argIndex62 = opt;
//                                        sname = p.SkillName0(ref argIndex62);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 20d;
//                                        }

//                                        if (u.SyncLevel() >= 30d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
//                                        }
//                                        else if (u.SyncLevel() > 0d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
//                                        }

//                                        break;
//                                    }

//                                case "霊力":
//                                    {
//                                        object argIndex63 = opt;
//                                        sname = p.SkillName0(ref argIndex63);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 10d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PlanaLevel()) + ")。";
//                                        break;
//                                    }

//                                case "オーラ":
//                                    {
//                                        object argIndex64 = opt;
//                                        sname = p.SkillName0(ref argIndex64);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.AuraLevel()) + ")。";
//                                        break;
//                                    }

//                                case "超能力":
//                                    {
//                                        object argIndex65 = opt;
//                                        sname = p.SkillName0(ref argIndex65);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PsychicLevel()) + ")。";
//                                        break;
//                                    }

//                                default:
//                                    {
//                                        sname = u.SkillName0(opt);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.SkillLevel(opt)) + ")。";
//                                        break;
//                                    }
//                            }
//                        }

//                        break;
//                    }

//                case "阻止":
//                    {
//                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(ref fdata, 2)) & GeneralLib.LIndex(ref fdata, 2) != "全")
//                        {
//                            if (Strings.Left(GeneralLib.LIndex(ref fdata, 2), 1) == "!")
//                            {
//                                msg = "「" + Strings.Mid(GeneralLib.LIndex(ref fdata, 2), 2) + "」属性を持たない";
//                            }
//                            else
//                            {
//                                msg = "「" + GeneralLib.LIndex(ref fdata, 2) + "」属性を持つ";
//                            }
//                        }

//                        if (flevel != 1d)
//                        {
//                            msg = msg + "ダメージ" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(500d * flevel)) + "以下の";
//                        }

//                        msg = msg + "攻撃を";
//                        buf = GeneralLib.LIndex(ref fdata, 3);
//                        if (Information.IsNumeric(buf))
//                        {
//                            if (buf != "100")
//                            {
//                                msg = msg + buf + "%の確率で阻止。";
//                            }
//                            else
//                            {
//                                // MOD START MARGE
//                                // msg = msg & buf & "阻止。"
//                                msg = msg + "阻止。";
//                                // MOD END MARGE
//                            }
//                        }
//                        else if (Strings.InStr(buf, "+") > 0 | Strings.InStr(buf, "-") > 0)
//                        {
//                            i = (short)GeneralLib.MaxLng(Strings.InStr(buf, "+"), Strings.InStr(buf, "-"));
//                            sname = u.SkillName0(Strings.Left(buf, i - 1));
//                            prob = (short)((long)((u.SkillLevel(Strings.Left(buf, i - 1)) + Conversions.ToShort(Strings.Mid(buf, i))) * 100d) / 16L);
//                            msg = msg + "(" + sname + "Lv" + Strings.Mid(buf, i) + ")/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で阻止。";
//                        }
//                        else
//                        {
//                            sname = u.SkillName0(buf);
//                            prob = (short)((long)(u.SkillLevel(buf) * 100d) / 16L);
//                            msg = msg + sname + "Lv/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で阻止。";
//                        }

//                        int localStrToLng13() { string argexpr = GeneralLib.LIndex(ref fdata, 4); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                        string argexpr32 = GeneralLib.LIndex(ref fdata, 4);
//                        if (GeneralLib.StrToLng(ref argexpr32) > 0)
//                        {
//                            string argtname41 = "ＥＮ";
//                            msg = msg + ";発動時に" + GeneralLib.LIndex(ref fdata, 4) + Expression.Term(ref argtname41, ref u) + "消費。";
//                        }
//                        else if (localStrToLng13() < 0)
//                        {
//                            string argtname42 = "ＥＮ";
//                            msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(ref fdata, 4), 2) + Expression.Term(ref argtname42, ref u) + "増加。";
//                        }

//                        string argexpr33 = GeneralLib.LIndex(ref fdata, 5);
//                        if (GeneralLib.StrToLng(ref argexpr33) > 50)
//                        {
//                            string argtname43 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname43, ref u) + GeneralLib.LIndex(ref fdata, 5) + "以上で使用可能。";
//                        }

//                        var loopTo12 = GeneralLib.LLength(ref fdata);
//                        for (i = 6; i <= loopTo12; i++)
//                        {
//                            opt = GeneralLib.LIndex(ref fdata, i);
//                            idx = (short)Strings.InStr(opt, "*");
//                            if (idx > 0)
//                            {
//                                string argexpr34 = Strings.Mid(opt, idx + 1);
//                                lv_mod = GeneralLib.StrToDbl(ref argexpr34);
//                                opt = Strings.Left(opt, idx - 1);
//                            }
//                            else
//                            {
//                                lv_mod = -1;
//                            }

//                            switch (p.SkillType(ref opt) ?? "")
//                            {
//                                case "相殺":
//                                    {
//                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、隣接時に効果は相殺。";
//                                        break;
//                                    }

//                                case "中和":
//                                    {
//                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、" + "隣接時にレベル分だけ効果を中和。";
//                                        break;
//                                    }

//                                case "近接無効":
//                                    {
//                                        msg = msg + ";「武」「突」「接」による攻撃には無効。";
//                                        break;
//                                    }

//                                case "手動":
//                                    {
//                                        msg = msg + ";防御選択時にのみ発動。";
//                                        break;
//                                    }

//                                case "能力必要":
//                                    {
//                                        break;
//                                    }
//                                // スキップ
//                                case "同調率":
//                                    {
//                                        object argIndex66 = opt;
//                                        sname = p.SkillName0(ref argIndex66);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 20d;
//                                        }

//                                        if (u.SyncLevel() >= 30d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
//                                        }
//                                        else if (u.SyncLevel() > 0d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
//                                        }

//                                        break;
//                                    }

//                                case "霊力":
//                                    {
//                                        object argIndex67 = opt;
//                                        sname = p.SkillName0(ref argIndex67);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 10d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PlanaLevel()) + ")。";
//                                        break;
//                                    }

//                                case "オーラ":
//                                    {
//                                        object argIndex68 = opt;
//                                        sname = p.SkillName0(ref argIndex68);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.AuraLevel()) + ")。";
//                                        break;
//                                    }

//                                case "超能力":
//                                    {
//                                        object argIndex69 = opt;
//                                        sname = p.SkillName0(ref argIndex69);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PsychicLevel()) + ")。";
//                                        break;
//                                    }

//                                default:
//                                    {
//                                        sname = u.SkillName0(opt);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.SkillLevel(opt)) + ")。";
//                                        break;
//                                    }
//                            }
//                        }

//                        break;
//                    }

//                case "広域阻止":
//                    {
//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)) & GeneralLib.LIndex(ref fdata, 2) != "1")
//                        {
//                            msg = "半径" + Strings.StrConv(GeneralLib.LIndex(ref fdata, 2), VbStrConv.Wide) + "マス以内の味方ユニットに対する";
//                            i = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 2));
//                        }
//                        else
//                        {
//                            msg = "隣接する味方ユニットに対する";
//                            i = 1;
//                        }

//                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(ref fdata, 3)) & GeneralLib.LIndex(ref fdata, 3) != "全")
//                        {
//                            if (Strings.Left(GeneralLib.LIndex(ref fdata, 3), 1) == "!")
//                            {
//                                msg = msg + "「" + Strings.Mid(GeneralLib.LIndex(ref fdata, 3), 2) + "」属性を持たない";
//                            }
//                            else
//                            {
//                                msg = msg + "「" + GeneralLib.LIndex(ref fdata, 3) + "」属性を持つ";
//                            }
//                        }

//                        if (flevel != 1d)
//                        {
//                            msg = msg + "ダメージ" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(500d * flevel)) + "以下の";
//                        }

//                        msg = msg + "攻撃を";
//                        buf = GeneralLib.LIndex(ref fdata, 4);
//                        if (Information.IsNumeric(buf))
//                        {
//                            if (buf != "100")
//                            {
//                                // MOD START MARGE
//                                // msg = msg & "%の確率で阻止。"
//                                msg = msg + buf + "%の確率で阻止。";
//                            }
//                            // MOD END MARGE
//                            else
//                            {
//                                msg = msg + "阻止。";
//                            }
//                        }
//                        else if (Strings.InStr(buf, "+") > 0 | Strings.InStr(buf, "-") > 0)
//                        {
//                            i = (short)GeneralLib.MaxLng(Strings.InStr(buf, "+"), Strings.InStr(buf, "-"));
//                            sname = u.SkillName0(Strings.Left(buf, i - 1));
//                            prob = (short)((long)((u.SkillLevel(Strings.Left(buf, i - 1)) + Conversions.ToShort(Strings.Mid(buf, i))) * 100d) / 16L);
//                            msg = msg + "(" + sname + "Lv" + Strings.Mid(buf, i) + ")/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で阻止。";
//                        }
//                        else
//                        {
//                            sname = u.SkillName0(buf);
//                            prob = (short)((long)(u.SkillLevel(buf) * 100d) / 16L);
//                            msg = msg + sname + "Lv/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で阻止。";
//                        }

//                        int localStrToLng14() { string argexpr = GeneralLib.LIndex(ref fdata, 5); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                        string argexpr35 = GeneralLib.LIndex(ref fdata, 5);
//                        if (GeneralLib.StrToLng(ref argexpr35) > 0)
//                        {
//                            string argtname44 = "ＥＮ";
//                            msg = msg + ";発動時に" + GeneralLib.LIndex(ref fdata, 5) + Expression.Term(ref argtname44, ref u) + "消費。";
//                        }
//                        else if (localStrToLng14() < 0)
//                        {
//                            string argtname45 = "ＥＮ";
//                            msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(ref fdata, 5), 2) + Expression.Term(ref argtname45, ref u) + "増加。";
//                        }

//                        string argexpr36 = GeneralLib.LIndex(ref fdata, 6);
//                        if (GeneralLib.StrToLng(ref argexpr36) > 50)
//                        {
//                            string argtname46 = "気力";
//                            msg = msg + Expression.Term(ref argtname46, ref u) + GeneralLib.LIndex(ref fdata, 6) + "以上で使用可能。";
//                        }

//                        msg = msg + ";ただし攻撃側も有効範囲内にいる場合は無効化。";
//                        break;
//                    }

//                case "融合":
//                    {
//                        prob = (short)((long)(flevel * 100d) / 16L);
//                        string argtname47 = "ＨＰ";
//                        msg = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(flevel) + "/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で発動し、" + "ダメージを" + Expression.Term(ref argtname47, ref u) + "に変換。;" + "ただし、「武」「突」「接」による攻撃には無効。";
//                        break;
//                    }

//                case "変換":
//                    {
//                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(ref fdata, 2)) & GeneralLib.LIndex(ref fdata, 2) != "全")
//                        {
//                            if (Strings.Left(GeneralLib.LIndex(ref fdata, 2), 1) == "!")
//                            {
//                                msg = "「" + Strings.Mid(GeneralLib.LIndex(ref fdata, 2), 2) + "」属性を持たない";
//                            }
//                            else
//                            {
//                                msg = "「" + GeneralLib.LIndex(ref fdata, 2) + "」属性を持つ";
//                            }
//                        }

//                        string argtname48 = "ＥＮ";
//                        string argtname49 = "ＥＮ";
//                        msg = msg + "攻撃を受けた際にダメージを" + Expression.Term(ref argtname48, ref u) + "に変換。;" + "変換効率は " + Expression.Term(ref argtname49, ref u) + "増加 ＝ ";
//                        msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(0.01d * flevel);
//                        msg = msg + " × ダメージ";
//                        break;
//                    }

//                case "ビーム吸収":
//                    {
//                        msg = "ビームによる攻撃のダメージをＨＰに変換";
//                        break;
//                    }

//                case "自動反撃":
//                    {
//                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(ref fdata, 3)) & GeneralLib.LIndex(ref fdata, 3) != "全")
//                        {
//                            if (Strings.Left(GeneralLib.LIndex(ref fdata, 3), 1) == "!")
//                            {
//                                msg = "「" + Strings.Mid(GeneralLib.LIndex(ref fdata, 3), 2) + "」属性を持たない";
//                            }
//                            else
//                            {
//                                msg = "「" + GeneralLib.LIndex(ref fdata, 3) + "」属性を持つ";
//                            }
//                        }

//                        if (flevel != 1d)
//                        {
//                            msg = msg + "ダメージ" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(500d * flevel)) + "までの";
//                        }

//                        msg = msg + "攻撃を受けた際に";
//                        buf = GeneralLib.LIndex(ref fdata, 4);
//                        if (Information.IsNumeric(buf))
//                        {
//                            if (buf != "100")
//                            {
//                                msg = msg + buf + "%の確率で、";
//                            }
//                        }
//                        else if (Strings.InStr(buf, "+") > 0 | Strings.InStr(buf, "-") > 0)
//                        {
//                            i = (short)GeneralLib.MaxLng(Strings.InStr(buf, "+"), Strings.InStr(buf, "-"));
//                            sname = u.SkillName0(Strings.Left(buf, i - 1));
//                            prob = (short)((long)((u.SkillLevel(Strings.Left(buf, i - 1)) + Conversions.ToShort(Strings.Mid(buf, i))) * 100d) / 16L);
//                            msg = msg + "(" + sname + "Lv" + Strings.Mid(buf, i) + ")/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で、";
//                        }
//                        else
//                        {
//                            sname = u.SkillName0(buf);
//                            prob = (short)((long)(u.SkillLevel(buf) * 100d) / 16L);
//                            msg = msg + sname + "Lv/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で、";
//                        }

//                        buf = GeneralLib.LIndex(ref fdata, 2);
//                        if (Strings.InStr(buf, "(") > 0)
//                        {
//                            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
//                        }

//                        msg = msg + buf + "による自動反撃が発動する。";
//                        int localStrToLng15() { string argexpr = GeneralLib.LIndex(ref fdata, 5); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                        string argexpr37 = GeneralLib.LIndex(ref fdata, 5);
//                        if (GeneralLib.StrToLng(ref argexpr37) > 0)
//                        {
//                            string argtname50 = "ＥＮ";
//                            msg = msg + ";発動時に" + GeneralLib.LIndex(ref fdata, 5) + Expression.Term(ref argtname50, ref u) + "消費。";
//                        }
//                        else if (localStrToLng15() < 0)
//                        {
//                            string argtname51 = "ＥＮ";
//                            msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(ref fdata, 5), 2) + Expression.Term(ref argtname51, ref u) + "増加。";
//                        }

//                        string argexpr38 = GeneralLib.LIndex(ref fdata, 6);
//                        if (GeneralLib.StrToLng(ref argexpr38) > 50)
//                        {
//                            string argtname52 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname52, ref u) + GeneralLib.LIndex(ref fdata, 6) + "以上で使用可能。";
//                        }

//                        var loopTo13 = GeneralLib.LLength(ref fdata);
//                        for (i = 7; i <= loopTo13; i++)
//                        {
//                            opt = GeneralLib.LIndex(ref fdata, i);
//                            idx = (short)Strings.InStr(opt, "*");
//                            if (idx > 0)
//                            {
//                                string argexpr39 = Strings.Mid(opt, idx + 1);
//                                lv_mod = GeneralLib.StrToDbl(ref argexpr39);
//                                opt = Strings.Left(opt, idx - 1);
//                            }
//                            else
//                            {
//                                lv_mod = -1;
//                            }

//                            switch (p.SkillType(ref opt) ?? "")
//                            {
//                                case "相殺":
//                                    {
//                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、隣接時に効果は相殺。";
//                                        break;
//                                    }

//                                case "中和":
//                                    {
//                                        msg = msg + ";" + fname0 + "を持つユニット同士の場合、" + "隣接時にレベル分だけ効果を相殺。";
//                                        break;
//                                    }

//                                case "近接無効":
//                                    {
//                                        msg = msg + ";「武」「突」「接」による攻撃には無効。";
//                                        break;
//                                    }

//                                case "手動":
//                                    {
//                                        msg = msg + ";防御選択時にのみ発動。";
//                                        break;
//                                    }

//                                case "能力必要":
//                                    {
//                                        break;
//                                    }
//                                // スキップ
//                                case "同調率":
//                                    {
//                                        object argIndex70 = opt;
//                                        sname = p.SkillName0(ref argIndex70);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 20d;
//                                        }

//                                        if (u.SyncLevel() >= 30d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
//                                        }
//                                        else if (u.SyncLevel() > 0d)
//                                        {
//                                            msg = msg + ";パイロットの" + sname + "により強度が変化(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * (u.SyncLevel() - 30d)) + ")。";
//                                        }

//                                        break;
//                                    }

//                                case "霊力":
//                                    {
//                                        object argIndex71 = opt;
//                                        sname = p.SkillName0(ref argIndex71);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 10d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PlanaLevel()) + ")。";
//                                        break;
//                                    }

//                                case "オーラ":
//                                    {
//                                        object argIndex72 = opt;
//                                        sname = p.SkillName0(ref argIndex72);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.AuraLevel()) + ")。";
//                                        break;
//                                    }

//                                case "超能力":
//                                    {
//                                        object argIndex73 = opt;
//                                        sname = p.SkillName0(ref argIndex73);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "により強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.PsychicLevel()) + ")。";
//                                        break;
//                                    }

//                                default:
//                                    {
//                                        sname = u.SkillName0(opt);
//                                        if (lv_mod == -1)
//                                        {
//                                            lv_mod = 200d;
//                                        }

//                                        msg = msg + ";パイロットの" + sname + "レベルにより強度が増加(+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lv_mod * u.SkillLevel(opt)) + ")。";
//                                        break;
//                                    }
//                            }
//                        }

//                        break;
//                    }

//                case "ＨＰ回復":
//                    {
//                        string argtname53 = "ＨＰ";
//                        string argtname54 = "ＨＰ";
//                        msg = "毎ターン最大" + Expression.Term(ref argtname53, ref u) + "の" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * flevel) + "%分の" + Expression.Term(ref argtname54, ref u) + "を回復。";
//                        break;
//                    }

//                case "ＥＮ回復":
//                    {
//                        string argtname55 = "ＥＮ";
//                        string argtname56 = "ＥＮ";
//                        msg = "毎ターン最大" + Expression.Term(ref argtname55, ref u) + "の" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * flevel) + "%分の" + Expression.Term(ref argtname56, ref u) + "を回復。";
//                        break;
//                    }

//                case "霊力回復":
//                    {
//                        object argIndex74 = "霊力";
//                        sname = p.SkillName0(ref argIndex74);
//                        msg = "毎ターン最大" + sname + "の" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * flevel) + "%分の" + sname + "を回復。";
//                        break;
//                    }

//                case "ＨＰ消費":
//                    {
//                        string argtname57 = "ＨＰ";
//                        string argtname58 = "ＨＰ";
//                        msg = "毎ターン最大" + Expression.Term(ref argtname57, ref u) + "の" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * flevel) + "%分の" + Expression.Term(ref argtname58, ref u) + "を消費。";
//                        break;
//                    }

//                case "ＥＮ消費":
//                    {
//                        string argtname59 = "ＥＮ";
//                        string argtname60 = "ＥＮ";
//                        msg = "毎ターン最大" + Expression.Term(ref argtname59, ref u) + "の" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * flevel) + "%分の" + Expression.Term(ref argtname60, ref u) + "を消費。";
//                        break;
//                    }

//                case "霊力消費":
//                    {
//                        object argIndex75 = "霊力";
//                        sname = p.SkillName0(ref argIndex75);
//                        msg = "毎ターン最大" + sname + "の" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * flevel) + "%分の" + sname + "を消費。";
//                        break;
//                    }

//                case "分身":
//                    {
//                        string argtname61 = "気力";
//                        msg = "50%の確率で攻撃を完全に回避。;" + "発動条件：" + Expression.Term(ref argtname61, ref u) + "130以上";
//                        break;
//                    }

//                case "超回避":
//                    {
//                        msg = "あらゆる攻撃を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * flevel) + "%の確率で回避。";
//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)))
//                        {
//                            int localStrToLng16() { string argexpr = GeneralLib.LIndex(ref fdata, 2); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                            string argexpr40 = GeneralLib.LIndex(ref fdata, 2);
//                            if (GeneralLib.StrToLng(ref argexpr40) > 0)
//                            {
//                                string argtname62 = "ＥＮ";
//                                msg = msg + ";発動時に" + GeneralLib.LIndex(ref fdata, 2) + Expression.Term(ref argtname62, ref u) + "消費。";
//                            }
//                            else if (localStrToLng16() < 0)
//                            {
//                                string argtname63 = "ＥＮ";
//                                msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(ref fdata, 2), 2) + Expression.Term(ref argtname63, ref u) + "増加。";
//                            }
//                        }

//                        string argexpr41 = GeneralLib.LIndex(ref fdata, 3);
//                        if (GeneralLib.StrToLng(ref argexpr41) > 50)
//                        {
//                            string argtname64 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname64, ref u) + GeneralLib.LIndex(ref fdata, 3) + "以上で使用可能。";
//                        }

//                        if (GeneralLib.LIndex(ref fdata, 4) == "手動")
//                        {
//                            msg = msg + ";回避選択時にのみ発動。";
//                        }

//                        break;
//                    }

//                case "緊急テレポート":
//                    {
//                        msg = "攻撃を受けた際に" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * flevel) + "%の確率で" + "テレポートし、攻撃を回避。;" + "テレポート先は" + GeneralLib.LIndex(ref fdata, 2) + "マス以内の範囲の内、" + "最も敵から遠い地点から選ばれる。";
//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 3)))
//                        {
//                            int localStrToLng17() { string argexpr = GeneralLib.LIndex(ref fdata, 3); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                            string argexpr42 = GeneralLib.LIndex(ref fdata, 3);
//                            if (GeneralLib.StrToLng(ref argexpr42) > 0)
//                            {
//                                string argtname65 = "ＥＮ";
//                                msg = msg + ";発動時に" + GeneralLib.LIndex(ref fdata, 3) + Expression.Term(ref argtname65, ref u) + "消費。";
//                            }
//                            else if (localStrToLng17() < 0)
//                            {
//                                string argtname66 = "ＥＮ";
//                                msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(ref fdata, 3), 2) + Expression.Term(ref argtname66, ref u) + "増加。";
//                            }
//                        }

//                        string argexpr43 = GeneralLib.LIndex(ref fdata, 4);
//                        if (GeneralLib.StrToLng(ref argexpr43) > 50)
//                        {
//                            string argtname67 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname67, ref u) + GeneralLib.LIndex(ref fdata, 4) + "以上で使用可能。";
//                        }

//                        if (GeneralLib.LIndex(ref fdata, 5) == "手動")
//                        {
//                            msg = msg + ";回避選択時にのみ発動。";
//                        }

//                        break;
//                    }

//                case "ダミー":
//                    {
//                        buf = fname;
//                        if (Conversions.ToBoolean(Strings.InStr(buf, "Lv")))
//                        {
//                            buf = Strings.Left(buf, Strings.InStr(buf, "Lv") - 1);
//                        }

//                        msg = buf + "を身代わりにして攻撃を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(flevel) + "回まで回避。";
//                        break;
//                    }

//                case "攻撃回避":
//                    {
//                        if (!string.IsNullOrEmpty(GeneralLib.LIndex(ref fdata, 2)) & GeneralLib.LIndex(ref fdata, 2) != "全")
//                        {
//                            if (Strings.Left(GeneralLib.LIndex(ref fdata, 2), 1) == "!")
//                            {
//                                msg = "「" + Strings.Mid(GeneralLib.LIndex(ref fdata, 2), 2) + "」属性を持たない";
//                            }
//                            else
//                            {
//                                msg = "「" + GeneralLib.LIndex(ref fdata, 2) + "」属性を持つ";
//                            }
//                        }

//                        if (flevel >= 0d)
//                        {
//                            msg = msg + "攻撃の命中率を本来の" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(100d - 10d * flevel)) + "%に減少させる。";
//                        }
//                        else
//                        {
//                            msg = msg + "攻撃の命中率を本来の" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((int)(100d - 10d * flevel)) + "%に増加させる。";
//                        }

//                        string argexpr44 = GeneralLib.LIndex(ref fdata, 3);
//                        if (GeneralLib.StrToLng(ref argexpr44) > 50)
//                        {
//                            string argtname68 = "気力";
//                            msg = msg + Expression.Term(ref argtname68, ref u) + GeneralLib.LIndex(ref fdata, 3) + "以上で使用可能。";
//                        }

//                        break;
//                    }

//                case "抵抗力":
//                    {
//                        if (flevel >= 0d)
//                        {
//                            msg = "武器の特殊効果を受ける確率を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * flevel) + "%減少させる。";
//                        }
//                        else
//                        {
//                            msg = "武器の特殊効果を受ける確率を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(-10 * flevel) + "%増加させる。";
//                        }

//                        break;
//                    }

//                case "修理装置":
//                    {
//                        string argtname69 = "ＨＰ";
//                        msg = "他のユニットの" + Expression.Term(ref argtname69, ref u);
//                        switch (flevel)
//                        {
//                            case 1d:
//                                {
//                                    string argtname70 = "ＨＰ";
//                                    msg = msg + "を最大" + Expression.Term(ref argtname70, ref u) + "の30%だけ回復。";
//                                    break;
//                                }

//                            case 2d:
//                                {
//                                    string argtname71 = "ＨＰ";
//                                    msg = msg + "を最大" + Expression.Term(ref argtname71, ref u) + "の50%だけ回復。";
//                                    break;
//                                }

//                            case 3d:
//                                {
//                                    msg = msg + "を全快。";
//                                    break;
//                                }
//                        }

//                        break;
//                    }

//                case "補給装置":
//                    {
//                        string argtname72 = "ＥＮ";
//                        string argtname73 = "気力";
//                        msg = "他のユニットの" + Expression.Term(ref argtname72, ref u) + "と弾薬を全快。;" + "ただしユニットのパイロットの" + Expression.Term(ref argtname73, ref u) + "は-10。";
//                        string argoname = "移動後補給不可";
//                        if (Expression.IsOptionDefined(ref argoname))
//                        {
//                            msg = msg + "移動後は使用不可。";
//                        }

//                        break;
//                    }

//                case "修理不可":
//                    {
//                        var loopTo14 = (short)Conversions.ToInteger(fdata);
//                        for (i = 2; i <= loopTo14; i++)
//                        {
//                            buf = GeneralLib.LIndex(ref fdata, i);
//                            if (Strings.Left(buf, 1) == "!")
//                            {
//                                buf = Strings.Mid(buf, 2);
//                                string argtname74 = "ＨＰ";
//                                msg = msg + buf + "以外では" + Expression.Term(ref argtname74, ref u) + "を回復出来ない。";
//                            }
//                            else
//                            {
//                                string argtname75 = "ＨＰ";
//                                msg = msg + buf + "では" + Expression.Term(ref argtname75, ref u) + "を回復出来ない。";
//                            }
//                        }

//                        string argtname76 = "スペシャルパワー";
//                        msg = msg + buf + ";ただし、" + Expression.Term(ref argtname76, ref u) + "や地形、母艦による回復は可能。";
//                        break;
//                    }

//                case "霊力変換器":
//                    {
//                        object argIndex76 = "霊力";
//                        sname = p.SkillName0(ref argIndex76);
//                        msg = sname + "に合わせて各種能力が上昇する。";
//                        if (flevel_specified)
//                        {
//                            msg = msg + ";（" + sname + "上限 = " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(flevel) + "）";
//                        }

//                        break;
//                    }

//                case "オーラ変換器":
//                    {
//                        object argIndex77 = "オーラ";
//                        sname = p.SkillName0(ref argIndex77);
//                        msg = sname + "レベルに合わせて各種能力が上昇する。";
//                        if (flevel_specified)
//                        {
//                            msg = msg + ";（" + sname + "上限レベル = " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(flevel) + "）";
//                        }

//                        break;
//                    }

//                case "サイキックドライブ":
//                    {
//                        object argIndex78 = "超能力";
//                        sname = p.SkillName0(ref argIndex78);
//                        string argtname77 = "装甲";
//                        string argtname78 = "運動性";
//                        msg = sname + "レベルごとに" + Expression.Term(ref argtname77, ref u) + "+100、" + Expression.Term(ref argtname78, ref u) + "+5";
//                        if (flevel_specified)
//                        {
//                            msg = msg + ";（" + sname + "上限レベル = " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(flevel) + "）";
//                        }

//                        break;
//                    }

//                case "シンクロドライブ":
//                    {
//                        object argIndex79 = "同調率";
//                        sname = p.SkillName0(ref argIndex79);
//                        msg = sname + "に合わせて各種能力が上昇する。";
//                        if (flevel_specified)
//                        {
//                            msg = msg + ";（" + sname + "上限 = " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(flevel) + "%）";
//                        }

//                        break;
//                    }

//                case "ステルス":
//                    {
//                        if (flevel_specified)
//                        {
//                            msg = "敵から" + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(flevel), VbStrConv.Wide) + "マス以内にいない限り発見されない。" + "ただし自分から攻撃すると１ターン無効。";
//                        }
//                        else
//                        {
//                            msg = "敵から３マス以内にいない限り発見されない。" + "ただし自分から攻撃すると１ターン無効。";
//                        }

//                        break;
//                    }

//                case "ステルス無効化":
//                    {
//                        msg = "敵のステルス能力を無効化する。";
//                        break;
//                    }

//                case "テレポート":
//                    {
//                        string argtname79 = "移動力";
//                        msg = "テレポートを行い、" + Expression.Term(ref argtname79, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(u.Speed + flevel) + "で地形を無視して移動。;";
//                        if (GeneralLib.LLength(ref fdata) > 1)
//                        {
//                            if (Conversions.ToShort(GeneralLib.LIndex(ref fdata, 2)) > 0)
//                            {
//                                string argtname80 = "ＥＮ";
//                                msg = msg + GeneralLib.LIndex(ref fdata, 2) + Expression.Term(ref argtname80, ref u) + "消費。";
//                            }
//                        }
//                        else
//                        {
//                            string argtname81 = "ＥＮ";
//                            msg = msg + "40" + Expression.Term(ref argtname81, ref u) + "消費。";
//                        }

//                        break;
//                    }

//                case "ジャンプ":
//                    {
//                        string argtname82 = "移動力";
//                        msg = Expression.Term(ref argtname82, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(u.Speed + flevel) + "で地上地形を無視しながらジャンプ移動。";
//                        if (GeneralLib.LLength(ref fdata) > 1)
//                        {
//                            string argexpr45 = GeneralLib.LIndex(ref fdata, 2);
//                            if (GeneralLib.StrToLng(ref argexpr45) > 0)
//                            {
//                                string argtname83 = "ＥＮ";
//                                msg = msg + ";" + GeneralLib.LIndex(ref fdata, 2) + Expression.Term(ref argtname83, ref u) + "消費。";
//                            }
//                        }

//                        break;
//                    }

//                case "水泳":
//                    {
//                        msg = "水中を泳いで移動可能。深海等の深い水の地形に進入することが出来る。" + "ただし水中での移動コストが１になる訳ではない。";
//                        break;
//                    }

//                case "水上移動":
//                    {
//                        msg = "水上に浮かんで移動可能。";
//                        break;
//                    }

//                case "ホバー移動":
//                    {
//                        string argtname84 = "ＥＮ";
//                        msg = "空中に浮きながら移動することで砂漠と雪原の移動コストが１になる。" + "また、水上移動も可能。ただし移動時に5" + Expression.Term(ref argtname84, ref u) + "消費。";
//                        break;
//                    }

//                case "透過移動":
//                    {
//                        msg = "障害物を無視して移動。";
//                        break;
//                    }

//                case "すり抜け移動":
//                    {
//                        msg = "敵ユニットがいるマスを通過可能。";
//                        break;
//                    }

//                case "線路移動":
//                    {
//                        msg = "線路上のみを移動可能。";
//                        break;
//                    }

//                case "移動制限":
//                    {
//                        msg = msg + GeneralLib.LIndex(ref fdata, 2);
//                        var loopTo15 = GeneralLib.LLength(ref fdata);
//                        for (i = 3; i <= loopTo15; i++)
//                            msg = msg + "、" + GeneralLib.LIndex(ref fdata, i);
//                        msg = msg + "上のみを移動可能。";
//                        break;
//                    }

//                case "進入不可":
//                    {
//                        msg = msg + GeneralLib.LIndex(ref fdata, 2);
//                        var loopTo16 = GeneralLib.LLength(ref fdata);
//                        for (i = 3; i <= loopTo16; i++)
//                            msg = msg + "、" + GeneralLib.LIndex(ref fdata, i);
//                        msg = msg + "には進入不可。";
//                        break;
//                    }

//                case "地形適応":
//                    {
//                        msg = msg + GeneralLib.LIndex(ref fdata, 2);
//                        var loopTo17 = GeneralLib.LLength(ref fdata);
//                        for (i = 3; i <= loopTo17; i++)
//                            msg = msg + "、" + GeneralLib.LIndex(ref fdata, i);
//                        msg = msg + "における移動コストが１になる。";
//                        break;
//                    }

//                case "追加移動力":
//                    {
//                        string argtname85 = "移動力";
//                        msg = GeneralLib.LIndex(ref fdata, 2) + "にいると、" + Expression.Term(ref argtname85, ref u) + "が";
//                        if (flevel >= 0d)
//                        {
//                            msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(flevel) + "増加。";
//                        }
//                        else
//                        {
//                            msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(-flevel) + "減少。";
//                        }

//                        break;
//                    }

//                case "母艦":
//                    {
//                        msg = "他のユニットを格納し、修理・運搬可能。";
//                        break;
//                    }

//                case "格納不可":
//                    {
//                        msg = "母艦に格納することが出来ない。";
//                        break;
//                    }

//                case "両手利き":
//                    {
//                        msg = "両手に武器を装備可能。";
//                        break;
//                    }

//                case "部隊ユニット":
//                    {
//                        msg = "複数のユニットによって構成された部隊ユニット。";
//                        break;
//                    }

//                case "召喚ユニット":
//                    {
//                        msg = "召喚されたユニット。";
//                        break;
//                    }

//                case "変形":
//                    {
//                        if (u.IsHero())
//                        {
//                            buf = "変化";
//                        }
//                        else
//                        {
//                            buf = "変形";
//                        }

//                        if (GeneralLib.LLength(ref fdata) > 2)
//                        {
//                            msg = "以下の形態に" + buf + "; ";
//                            var loopTo18 = GeneralLib.LLength(ref fdata);
//                            for (i = 2; i <= loopTo18; i++)
//                            {
//                                Unit localOtherForm() { object argIndex1 = GeneralLib.LIndex(ref fdata, i); var ret = u.OtherForm(ref argIndex1); return ret; }

//                                if (localOtherForm().IsAvailable())
//                                {
//                                    UnitData localItem2() { object argIndex1 = GeneralLib.LIndex(ref fdata, i); var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                                    if ((u.Nickname ?? "") == (localItem2().Nickname ?? ""))
//                                    {
//                                        UnitData localItem() { object argIndex1 = GeneralLib.LIndex(ref fdata, i); var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                                        uname = localItem().Name;
//                                        if (Strings.Right(uname, 5) == "(前期型)")
//                                        {
//                                            uname = Strings.Left(uname, Strings.Len(uname) - 5);
//                                        }
//                                        else if (Strings.Right(uname, 5) == "・前期型)")
//                                        {
//                                            uname = Strings.Left(uname, Strings.Len(uname) - 5) + ")";
//                                        }
//                                        else if (Strings.Right(uname, 5) == "(後期型)")
//                                        {
//                                            uname = Strings.Left(uname, Strings.Len(uname) - 5);
//                                        }
//                                    }
//                                    else
//                                    {
//                                        UnitData localItem1() { object argIndex1 = GeneralLib.LIndex(ref fdata, i); var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                                        uname = localItem1().Nickname;
//                                    }

//                                    msg = msg + uname + "  ";
//                                }
//                            }
//                        }
//                        else
//                        {
//                            UnitData localItem5() { object argIndex1 = GeneralLib.LIndex(ref fdata, 2); var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                            if ((u.Nickname ?? "") == (localItem5().Nickname ?? ""))
//                            {
//                                UnitData localItem3() { object argIndex1 = GeneralLib.LIndex(ref fdata, 2); var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                                uname = localItem3().Name;
//                            }
//                            else
//                            {
//                                UnitData localItem4() { object argIndex1 = GeneralLib.LIndex(ref fdata, 2); var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                                uname = localItem4().Nickname;
//                            }

//                            if (Strings.Right(uname, 5) == "(前期型)")
//                            {
//                                uname = Strings.Left(uname, Strings.Len(uname) - 5);
//                            }
//                            else if (Strings.Right(uname, 5) == "・前期型)")
//                            {
//                                uname = Strings.Left(uname, Strings.Len(uname) - 5) + ")";
//                            }
//                            else if (Strings.Right(uname, 5) == "(後期型)")
//                            {
//                                uname = Strings.Left(uname, Strings.Len(uname) - 5);
//                            }

//                            msg = "<B>" + uname + "</B>に" + buf + "。";
//                        }

//                        break;
//                    }

//                case "パーツ分離":
//                    {
//                        UnitData localItem8() { object argIndex1 = GeneralLib.LIndex(ref fdata, 2); var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                        if ((u.Nickname ?? "") == (localItem8().Nickname ?? ""))
//                        {
//                            UnitData localItem6() { object argIndex1 = GeneralLib.LIndex(ref fdata, 2); var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                            uname = localItem6().Name;
//                        }
//                        else
//                        {
//                            UnitData localItem7() { object argIndex1 = GeneralLib.LIndex(ref fdata, 2); var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                            uname = localItem7().Nickname;
//                        }

//                        if (Strings.Right(uname, 5) == "(前期型)")
//                        {
//                            uname = Strings.Left(uname, Strings.Len(uname) - 5);
//                        }
//                        else if (Strings.Right(uname, 5) == "・前期型)")
//                        {
//                            uname = Strings.Left(uname, Strings.Len(uname) - 5) + ")";
//                        }
//                        else if (Strings.Right(uname, 5) == "(後期型)")
//                        {
//                            uname = Strings.Left(uname, Strings.Len(uname) - 5);
//                        }

//                        msg = "パーツを分離し" + uname + "に変形。";
//                        if (flevel_specified)
//                        {
//                            msg = msg + ";ユニット破壊時に" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * flevel) + "%の確率で発動。";
//                        }

//                        break;
//                    }

//                case "パーツ合体":
//                    {
//                        UnitData localItem11() { object argIndex1 = fdata; var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                        if ((u.Nickname ?? "") == (localItem11().Nickname ?? ""))
//                        {
//                            UnitData localItem9() { object argIndex1 = fdata; var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                            uname = localItem9().Name;
//                        }
//                        else
//                        {
//                            UnitData localItem10() { object argIndex1 = fdata; var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                            uname = localItem10().Nickname;
//                        }

//                        if (Strings.Right(uname, 5) == "(前期型)")
//                        {
//                            uname = Strings.Left(uname, Strings.Len(uname) - 5);
//                        }
//                        else if (Strings.Right(uname, 5) == "・前期型)")
//                        {
//                            uname = Strings.Left(uname, Strings.Len(uname) - 5) + ")";
//                        }
//                        else if (Strings.Right(uname, 5) == "(後期型)")
//                        {
//                            uname = Strings.Left(uname, Strings.Len(uname) - 5);
//                        }

//                        msg = "パーツと合体し" + uname + "に変形。";
//                        break;
//                    }

//                case "ハイパーモード":
//                    {
//                        UnitData localItem14() { object argIndex1 = GeneralLib.LIndex(ref fdata, 2); var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                        if ((u.Nickname ?? "") == (localItem14().Nickname ?? ""))
//                        {
//                            UnitData localItem12() { object argIndex1 = GeneralLib.LIndex(ref fdata, 2); var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                            uname = localItem12().Name;
//                        }
//                        else
//                        {
//                            UnitData localItem13() { object argIndex1 = GeneralLib.LIndex(ref fdata, 2); var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                            uname = localItem13().Nickname;
//                        }

//                        if (Strings.Right(uname, 5) == "(前期型)")
//                        {
//                            uname = Strings.Left(uname, Strings.Len(uname) - 5);
//                        }
//                        else if (Strings.Right(uname, 5) == "・前期型)")
//                        {
//                            uname = Strings.Left(uname, Strings.Len(uname) - 5) + ")";
//                        }
//                        else if (Strings.Right(uname, 5) == "(後期型)")
//                        {
//                            uname = Strings.Left(uname, Strings.Len(uname) - 5);
//                        }

//                        if ((u.Nickname ?? "") != (uname ?? ""))
//                        {
//                            uname = "<B>" + uname + "</B>";
//                        }
//                        else
//                        {
//                            uname = "";
//                        }

//                        if (Strings.InStr(fdata, "気力発動") > 0)
//                        {
//                            string argtname86 = "気力";
//                            msg = Expression.Term(ref argtname86, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(100d + 10d * flevel) + "で特殊形態" + uname + "に";
//                        }
//                        else if (flevel <= 5d)
//                        {
//                            string argtname89 = "気力";
//                            string argtname90 = "ＨＰ";
//                            string argtname91 = "ＨＰ";
//                            msg = Expression.Term(ref argtname89, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(100d + 10d * flevel) + "、" + "もしくは" + Expression.Term(ref argtname90, ref u) + "が最大" + Expression.Term(ref argtname91, ref u) + "の1/4以下で特殊形態" + uname + "に";
//                        }
//                        else
//                        {
//                            string argtname87 = "ＨＰ";
//                            string argtname88 = "ＨＰ";
//                            msg = Expression.Term(ref argtname87, ref u) + "が最大" + Expression.Term(ref argtname88, ref u) + "の1/4以下で特殊形態" + uname + "に";
//                        }

//                        if (Strings.InStr(fdata, "自動発動") > 0)
//                        {
//                            msg = msg + "自動";
//                        }

//                        if (u.IsHero())
//                        {
//                            msg = msg + "変身。";
//                        }
//                        else
//                        {
//                            msg = msg + "変形。";
//                        }

//                        break;
//                    }

//                case "合体":
//                    {
//                        if (u.IsHero())
//                        {
//                            buf = "変化。";
//                        }
//                        else
//                        {
//                            buf = "変形。";
//                        }

//                        if (GeneralLib.LLength(ref fdata) > 3)
//                        {
//                            object argIndex80 = GeneralLib.LIndex(ref fdata, 2);
//                            if (SRC.UDList.IsDefined(ref argIndex80))
//                            {
//                                UnitData localItem15() { object argIndex1 = GeneralLib.LIndex(ref fdata, 2); var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                                msg = "以下のユニットと合体し<B>" + localItem15().Nickname + "</B>に" + buf + "; ";
//                            }
//                            else
//                            {
//                                msg = "以下のユニットと合体し<B>" + GeneralLib.LIndex(ref fdata, 2) + "</B>に" + buf + "; ";
//                            }

//                            var loopTo19 = GeneralLib.LLength(ref fdata);
//                            for (i = 3; i <= loopTo19; i++)
//                            {
//                                object argIndex81 = GeneralLib.LIndex(ref fdata, i);
//                                if (SRC.UDList.IsDefined(ref argIndex81))
//                                {
//                                    UnitData localItem16() { object argIndex1 = GeneralLib.LIndex(ref fdata, i); var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                                    msg = msg + localItem16().Nickname + "  ";
//                                }
//                                else
//                                {
//                                    msg = msg + GeneralLib.LIndex(ref fdata, i) + "  ";
//                                }
//                            }
//                        }
//                        else
//                        {
//                            object argIndex82 = GeneralLib.LIndex(ref fdata, 3);
//                            if (SRC.UDList.IsDefined(ref argIndex82))
//                            {
//                                UnitData localItem17() { object argIndex1 = GeneralLib.LIndex(ref fdata, 3); var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                                msg = localItem17().Nickname + "と合体し";
//                            }
//                            else
//                            {
//                                msg = GeneralLib.LIndex(ref fdata, 3) + "と合体し";
//                            }

//                            object argIndex83 = GeneralLib.LIndex(ref fdata, 2);
//                            if (SRC.UDList.IsDefined(ref argIndex83))
//                            {
//                                UnitData localItem18() { object argIndex1 = GeneralLib.LIndex(ref fdata, 2); var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                                msg = msg + localItem18().Nickname + "に" + buf;
//                            }
//                            else
//                            {
//                                msg = msg + GeneralLib.LIndex(ref fdata, 2) + "に" + buf;
//                            }
//                        }

//                        break;
//                    }

//                case "分離":
//                    {
//                        msg = "以下のユニットに分離。; ";
//                        var loopTo20 = GeneralLib.LLength(ref fdata);
//                        for (i = 2; i <= loopTo20; i++)
//                        {
//                            object argIndex84 = GeneralLib.LIndex(ref fdata, i);
//                            if (SRC.UDList.IsDefined(ref argIndex84))
//                            {
//                                UnitData localItem19() { object argIndex1 = GeneralLib.LIndex(ref fdata, i); var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                                msg = msg + localItem19().Nickname + "  ";
//                            }
//                            else
//                            {
//                                msg = msg + GeneralLib.LIndex(ref fdata, i) + "  ";
//                            }
//                        }

//                        break;
//                    }

//                case "不安定":
//                    {
//                        string argtname92 = "ＨＰ";
//                        msg = Expression.Term(ref argtname92, ref u) + "が最大値の1/4以下になると暴走する。";
//                        break;
//                    }

//                case "支配":
//                    {
//                        if (GeneralLib.LLength(ref fdata) == 2)
//                        {
//                            bool localIsDefined() { object argIndex1 = GeneralLib.LIndex(ref fdata, 2); var ret = SRC.PDList.IsDefined(ref argIndex1); return ret; }

//                            if (!localIsDefined())
//                            {
//                                string argmsg = "支配対象のパイロット「" + GeneralLib.LIndex(ref fdata, 2) + "」のデータが定義されていません";
//                                GUI.ErrorMessage(ref argmsg);
//                                return FeatureHelpMessageRet;
//                            }

//                            PilotData localItem20() { object argIndex1 = GeneralLib.LIndex(ref fdata, 2); var ret = SRC.PDList.Item(ref argIndex1); return ret; }

//                            msg = localItem20().Nickname + "の存在を維持し、仕えさせている。";
//                        }
//                        else
//                        {
//                            msg = "以下のユニットの存在を維持し、仕えさせている。;";
//                            var loopTo21 = GeneralLib.LLength(ref fdata);
//                            for (i = 2; i <= loopTo21; i++)
//                            {
//                                bool localIsDefined1() { object argIndex1 = GeneralLib.LIndex(ref fdata, 2); var ret = SRC.PDList.IsDefined(ref argIndex1); return ret; }

//                                if (!localIsDefined1())
//                                {
//                                    string argmsg1 = "支配対象のパイロット「" + GeneralLib.LIndex(ref fdata, i) + "」のデータが定義されていません";
//                                    GUI.ErrorMessage(ref argmsg1);
//                                    return FeatureHelpMessageRet;
//                                }

//                                PilotData localItem21() { object argIndex1 = GeneralLib.LIndex(ref fdata, i); var ret = SRC.PDList.Item(ref argIndex1); return ret; }

//                                msg = msg + localItem21().Nickname + "  ";
//                            }
//                        }

//                        break;
//                    }

//                case "ＥＣＭ":
//                    {
//                        msg = "半径３マス以内の味方ユニットに対する攻撃の命中率を元の";
//                        if (flevel >= 0d)
//                        {
//                            msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(100d - 5d * flevel) + "%に減少させる。";
//                        }
//                        else
//                        {
//                            msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(100d - 5d * flevel) + "%に増加させる。";
//                        }

//                        buf = fname;
//                        if (Conversions.ToBoolean(Strings.InStr(buf, "Lv")))
//                        {
//                            buf = Strings.Left(buf, Strings.InStr(buf, "Lv") - 1);
//                        }

//                        msg = msg + "同時に相手の" + buf + "能力の効果を無効化。";
//                        msg = msg + ";思念誘導攻撃や近接攻撃には無効。";
//                        break;
//                    }

//                case "ブースト":
//                    {
//                        string argoname1 = "ダメージ倍率低下";
//                        if (Expression.IsOptionDefined(ref argoname1))
//                        {
//                            string argtname93 = "気力";
//                            msg = Expression.Term(ref argtname93, ref u) + "130以上で発動し、ダメージを 20% アップ。";
//                        }
//                        else
//                        {
//                            string argtname94 = "気力";
//                            msg = Expression.Term(ref argtname94, ref u) + "130以上で発動し、ダメージを 25% アップ。";
//                        }

//                        break;
//                    }

//                case "防御不可":
//                    {
//                        msg = "攻撃を受けた際に防御運動を取ることが出来ない。";
//                        break;
//                    }

//                case "回避不可":
//                    {
//                        msg = "攻撃を受けた際に回避運動を取ることが出来ない。";
//                        break;
//                    }

//                case "格闘強化":
//                    {
//                        if (flevel >= 0d)
//                        {
//                            string argtname95 = "格闘";
//                            msg = "パイロットの" + Expression.Term(ref argtname95, ref u) + "を+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(5d * flevel)) + "。";
//                        }
//                        else
//                        {
//                            string argtname96 = "格闘";
//                            msg = "パイロットの" + Expression.Term(ref argtname96, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(5d * flevel)) + "。";
//                        }

//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)))
//                        {
//                            string argtname97 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname97, ref u) + GeneralLib.LIndex(ref fdata, 2) + "以上で発動。";
//                        }

//                        break;
//                    }

//                case "射撃強化":
//                    {
//                        if (p.HasMana())
//                        {
//                            if (flevel >= 0d)
//                            {
//                                string argtname98 = "魔力";
//                                msg = "パイロットの" + Expression.Term(ref argtname98, ref u) + "を+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(5d * flevel)) + "。";
//                            }
//                            else
//                            {
//                                string argtname99 = "魔力";
//                                msg = "パイロットの" + Expression.Term(ref argtname99, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(5d * flevel)) + "。";
//                            }
//                        }
//                        else if (flevel >= 0d)
//                        {
//                            string argtname100 = "射撃";
//                            msg = "パイロットの" + Expression.Term(ref argtname100, ref u) + "を+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(5d * flevel)) + "。";
//                        }
//                        else
//                        {
//                            string argtname101 = "射撃";
//                            msg = "パイロットの" + Expression.Term(ref argtname101, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(5d * flevel)) + "。";
//                        }

//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)))
//                        {
//                            string argtname102 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname102, ref u) + GeneralLib.LIndex(ref fdata, 2) + "以上で発動。";
//                        }

//                        break;
//                    }

//                case "命中強化":
//                    {
//                        if (flevel >= 0d)
//                        {
//                            string argtname103 = "命中";
//                            msg = "パイロットの" + Expression.Term(ref argtname103, ref u) + "を+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(5d * flevel)) + "。";
//                        }
//                        else
//                        {
//                            string argtname104 = "命中";
//                            msg = "パイロットの" + Expression.Term(ref argtname104, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(5d * flevel)) + "。";
//                        }

//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)))
//                        {
//                            msg = msg + "気力" + GeneralLib.LIndex(ref fdata, 2) + "以上で発動。";
//                        }

//                        break;
//                    }

//                case "回避強化":
//                    {
//                        if (flevel >= 0d)
//                        {
//                            string argtname105 = "回避";
//                            msg = "パイロットの" + Expression.Term(ref argtname105, ref u) + "を+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(5d * flevel)) + "。";
//                        }
//                        else
//                        {
//                            string argtname106 = "回避";
//                            msg = "パイロットの" + Expression.Term(ref argtname106, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(5d * flevel)) + "。";
//                        }

//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)))
//                        {
//                            string argtname107 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname107, ref u) + GeneralLib.LIndex(ref fdata, 2) + "以上で発動。";
//                        }

//                        break;
//                    }

//                case "技量強化":
//                    {
//                        if (flevel >= 0d)
//                        {
//                            string argtname108 = "技量";
//                            msg = "パイロットの" + Expression.Term(ref argtname108, ref u) + "を+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(5d * flevel)) + "。";
//                        }
//                        else
//                        {
//                            string argtname109 = "技量";
//                            msg = "パイロットの" + Expression.Term(ref argtname109, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(5d * flevel)) + "。";
//                        }

//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)))
//                        {
//                            string argtname110 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname110, ref u) + GeneralLib.LIndex(ref fdata, 2) + "以上で発動。";
//                        }

//                        break;
//                    }

//                case "反応強化":
//                    {
//                        if (flevel >= 0d)
//                        {
//                            string argtname111 = "反応";
//                            msg = "パイロットの" + Expression.Term(ref argtname111, ref u) + "を+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(5d * flevel)) + "。";
//                        }
//                        else
//                        {
//                            string argtname112 = "反応";
//                            msg = "パイロットの" + Expression.Term(ref argtname112, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(5d * flevel)) + "。";
//                        }

//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)))
//                        {
//                            string argtname113 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname113, ref u) + GeneralLib.LIndex(ref fdata, 2) + "以上で発動。";
//                        }

//                        break;
//                    }

//                case "ＨＰ強化":
//                    {
//                        if (flevel >= 0d)
//                        {
//                            string argtname114 = "ＨＰ";
//                            msg = "最大" + Expression.Term(ref argtname114, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(200d * flevel)) + "増加。";
//                        }
//                        else
//                        {
//                            string argtname115 = "ＨＰ";
//                            msg = "最大" + Expression.Term(ref argtname115, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(-200 * flevel)) + "減少。";
//                        }

//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)))
//                        {
//                            string argtname116 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname116, ref u) + GeneralLib.LIndex(ref fdata, 2) + "以上で発動。";
//                        }

//                        break;
//                    }

//                case "ＥＮ強化":
//                    {
//                        if (flevel >= 0d)
//                        {
//                            string argtname117 = "ＥＮ";
//                            msg = "最大" + Expression.Term(ref argtname117, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(10d * flevel)) + "増加。";
//                        }
//                        else
//                        {
//                            string argtname118 = "ＥＮ";
//                            msg = "最大" + Expression.Term(ref argtname118, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(-10 * flevel)) + "減少。";
//                        }

//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)))
//                        {
//                            string argtname119 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname119, ref u) + GeneralLib.LIndex(ref fdata, 2) + "以上で発動。";
//                        }

//                        break;
//                    }

//                case "装甲強化":
//                    {
//                        if (flevel >= 0d)
//                        {
//                            string argtname120 = "装甲";
//                            msg = Expression.Term(ref argtname120, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(100d * flevel)) + "増加。";
//                        }
//                        else
//                        {
//                            string argtname121 = "装甲";
//                            msg = Expression.Term(ref argtname121, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(-100 * flevel)) + "減少。";
//                        }

//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)))
//                        {
//                            string argtname122 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname122, ref u) + GeneralLib.LIndex(ref fdata, 2) + "以上で発動。";
//                        }

//                        break;
//                    }

//                case "運動性強化":
//                    {
//                        if (flevel >= 0d)
//                        {
//                            string argtname123 = "運動性";
//                            msg = Expression.Term(ref argtname123, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(5d * flevel)) + "増加。";
//                        }
//                        else
//                        {
//                            string argtname124 = "運動性";
//                            msg = Expression.Term(ref argtname124, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(-5 * flevel)) + "減少。";
//                        }

//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)))
//                        {
//                            string argtname125 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname125, ref u) + GeneralLib.LIndex(ref fdata, 2) + "以上で発動。";
//                        }

//                        break;
//                    }

//                case "移動力強化":
//                    {
//                        if (flevel >= 0d)
//                        {
//                            string argtname126 = "移動力";
//                            msg = Expression.Term(ref argtname126, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)flevel) + "増加。";
//                        }
//                        else
//                        {
//                            string argtname127 = "移動力";
//                            msg = Expression.Term(ref argtname127, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)flevel) + "減少。";
//                        }

//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)))
//                        {
//                            string argtname128 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname128, ref u) + GeneralLib.LIndex(ref fdata, 2) + "以上で発動。";
//                        }

//                        break;
//                    }

//                case "ＨＰ割合強化":
//                    {
//                        if (flevel >= 0d)
//                        {
//                            string argtname129 = "ＨＰ";
//                            msg = "最大" + Expression.Term(ref argtname129, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(5d * flevel)) + "%分増加。";
//                        }
//                        else
//                        {
//                            string argtname130 = "ＨＰ";
//                            msg = "最大" + Expression.Term(ref argtname130, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(-5 * flevel)) + "%分減少。";
//                        }

//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)))
//                        {
//                            string argtname131 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname131, ref u) + GeneralLib.LIndex(ref fdata, 2) + "以上で発動。";
//                        }

//                        break;
//                    }

//                case "ＥＮ割合強化":
//                    {
//                        if (flevel >= 0d)
//                        {
//                            string argtname132 = "ＥＮ";
//                            msg = "最大" + Expression.Term(ref argtname132, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(5d * flevel)) + "%分増加。";
//                        }
//                        else
//                        {
//                            string argtname133 = "ＥＮ";
//                            msg = "最大" + Expression.Term(ref argtname133, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(-5 * flevel)) + "%分減少。";
//                        }

//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)))
//                        {
//                            string argtname134 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname134, ref u) + GeneralLib.LIndex(ref fdata, 2) + "以上で発動。";
//                        }

//                        break;
//                    }

//                case "装甲割合強化":
//                    {
//                        if (flevel >= 0d)
//                        {
//                            string argtname135 = "装甲";
//                            msg = Expression.Term(ref argtname135, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(5d * flevel)) + "%分増加。";
//                        }
//                        else
//                        {
//                            string argtname136 = "装甲";
//                            msg = Expression.Term(ref argtname136, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(-5 * flevel)) + "%分減少。";
//                        }

//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)))
//                        {
//                            string argtname137 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname137, ref u) + GeneralLib.LIndex(ref fdata, 2) + "以上で発動。";
//                        }

//                        break;
//                    }

//                case "運動性割合強化":
//                    {
//                        if (flevel >= 0d)
//                        {
//                            string argtname138 = "運動性";
//                            msg = Expression.Term(ref argtname138, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(5d * flevel)) + "%分増加。";
//                        }
//                        else
//                        {
//                            string argtname139 = "運動性";
//                            msg = Expression.Term(ref argtname139, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(-5 * flevel)) + "%分減少。";
//                        }

//                        if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)))
//                        {
//                            string argtname140 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname140, ref u) + GeneralLib.LIndex(ref fdata, 2) + "以上で発動。";
//                        }

//                        break;
//                    }

//                case "武器・防具クラス":
//                    {
//                        fdata = Strings.Trim(u.WeaponProficiency());
//                        if (!string.IsNullOrEmpty(fdata))
//                        {
//                            msg = "武器【" + fdata + "】;";
//                        }
//                        else
//                        {
//                            msg = "武器【-】;";
//                        }

//                        fdata = Strings.Trim(u.ArmorProficiency());
//                        if (!string.IsNullOrEmpty(fdata))
//                        {
//                            msg = msg + "防具【" + fdata + "】";
//                        }
//                        else
//                        {
//                            msg = msg + "防具【-】";
//                        }

//                        break;
//                    }

//                case "追加攻撃":
//                    {
//                        if (GeneralLib.LIndex(ref fdata, 3) != "全")
//                        {
//                            buf = GeneralLib.LIndex(ref fdata, 3);
//                            if (Strings.Left(buf, 1) == "@")
//                            {
//                                msg = Strings.Mid(buf, 2) + "による";
//                            }
//                            else
//                            {
//                                msg = "「" + buf + "」属性を持つ武器による";
//                            }
//                        }

//                        msg = msg + "攻撃の後に、";
//                        buf = GeneralLib.LIndex(ref fdata, 4);
//                        if (Information.IsNumeric(buf))
//                        {
//                            if (buf != "100")
//                            {
//                                msg = msg + buf + "%の確率で";
//                            }
//                        }
//                        else if (Strings.InStr(buf, "+") > 0 | Strings.InStr(buf, "-") > 0)
//                        {
//                            i = (short)GeneralLib.MaxLng(Strings.InStr(buf, "+"), Strings.InStr(buf, "-"));
//                            sname = u.SkillName0(Strings.Left(buf, i - 1));
//                            prob = (short)((long)((u.SkillLevel(Strings.Left(buf, i - 1)) + Conversions.ToShort(Strings.Mid(buf, i))) * 100d) / 16L);
//                            msg = msg + "(" + sname + "Lv" + Strings.Mid(buf, i) + ")/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で";
//                        }
//                        else
//                        {
//                            sname = u.SkillName0(buf);
//                            prob = (short)((long)(u.SkillLevel(buf) * 100d) / 16L);
//                            msg = msg + sname + "Lv/16の確率(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prob) + "%)で";
//                        }

//                        buf = GeneralLib.LIndex(ref fdata, 2);
//                        if (Strings.InStr(buf, "(") > 0)
//                        {
//                            buf = Strings.Left(buf, Strings.InStr(buf, "(") - 1);
//                        }

//                        msg = msg + buf + "による追撃を行う。";
//                        int localStrToLng18() { string argexpr = GeneralLib.LIndex(ref fdata, 5); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                        string argexpr46 = GeneralLib.LIndex(ref fdata, 5);
//                        if (GeneralLib.StrToLng(ref argexpr46) > 0)
//                        {
//                            msg = msg + ";発動時に" + GeneralLib.LIndex(ref fdata, 5) + "ＥＮ消費。";
//                        }
//                        else if (localStrToLng18() < 0)
//                        {
//                            msg = msg + ";発動時に" + Strings.Mid(GeneralLib.LIndex(ref fdata, 5), 2) + "ＥＮ増加。";
//                        }

//                        string argexpr47 = GeneralLib.LIndex(ref fdata, 6);
//                        if (GeneralLib.StrToLng(ref argexpr47) > 50)
//                        {
//                            string argtname141 = "気力";
//                            msg = msg + ";" + Expression.Term(ref argtname141, ref u) + GeneralLib.LIndex(ref fdata, 6) + "以上で使用可能。";
//                        }

//                        if (Strings.InStr(fdata, "連鎖不可") > 0)
//                        {
//                            msg = msg + "連鎖不可。";
//                        }

//                        break;
//                    }

//                case "ＺＯＣ":
//                    {
//                        object argIndex85 = "ＺＯＣ";
//                        if (u.FeatureLevel(ref argIndex85) < 0d)
//                        {
//                            msg = "このユニットはＺＯＣによる影響を与えることが出来ない。";
//                        }
//                        else
//                        {
//                            msg = "このユニットから";
//                            if (GeneralLib.LLength(ref fdata) < 2)
//                            {
//                                buf = "1";
//                            }
//                            else
//                            {
//                                buf = GeneralLib.LIndex(ref fdata, 2);
//                            }

//                            opt = GeneralLib.LIndex(ref fdata, 3);
//                            if (Strings.InStr(opt, "直線") > 0)
//                            {
//                                msg = msg + buf + "マス以内の直線上";
//                            }
//                            else if (Strings.InStr(opt, " 水平") > 0)
//                            {
//                                msg = msg + "左右" + buf + "マス以内の直線上";
//                            }
//                            else if (Strings.InStr(opt, " 垂直") > 0)
//                            {
//                                msg = msg + "上下" + buf + "マス以内の直線上";
//                            }
//                            else
//                            {
//                                msg = msg + buf + "マス以内";
//                            }

//                            msg = msg + "を通過する敵ユニットに、ＺＯＣによる影響を与える。";
//                        }

//                        break;
//                    }

//                case "ＺＯＣ無効化":
//                    {
//                        if (flevel == 1d)
//                        {
//                            msg = "このユニットは敵ユニットによるＺＯＣの影響を受けない。";
//                        }
//                        else
//                        {
//                            msg = "このユニットは敵ユニットによる" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(flevel) + "レベル以下のＺＯＣの影響を受けない。";
//                        }

//                        break;
//                    }

//                case "隣接ユニットＺＯＣ無効化":
//                    {
//                        if (flevel == 1d)
//                        {
//                            msg = "このユニットが隣接する敵ユニットによるＺＯＣを無効化する。";
//                        }
//                        else
//                        {
//                            msg = "このユニットが隣接する敵ユニットによる" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(flevel) + "レベル以下のＺＯＣを無効化する。";
//                        }

//                        break;
//                    }

//                case "広域ＺＯＣ無効化":
//                    {
//                        msg = "このユニットから";
//                        if (GeneralLib.LLength(ref fdata) < 2)
//                        {
//                            buf = "1";
//                        }
//                        else
//                        {
//                            buf = GeneralLib.LIndex(ref fdata, 2);
//                        }

//                        if (flevel == 1d)
//                        {
//                            msg = msg + buf + "マス以内に設定されたＺＯＣの影響を無効化する。";
//                        }
//                        else
//                        {
//                            msg = msg + buf + "マス以内に設定された" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(flevel) + "レベル以下のＺＯＣの影響を無効化する。";
//                        }

//                        break;
//                    }

//                // ADD START MARGE
//                case "地形効果無効化":
//                    {
//                        if (GeneralLib.LLength(ref fdata) > 1)
//                        {
//                            var loopTo22 = GeneralLib.LLength(ref fdata);
//                            for (i = 2; i <= loopTo22; i++)
//                            {
//                                if (i > 2)
//                                {
//                                    msg = msg + "、";
//                                }

//                                msg = msg + GeneralLib.LIndex(ref fdata, i);
//                            }

//                            msg = msg + "の";
//                        }
//                        else
//                        {
//                            msg = msg + "全地形の";
//                        }
//                        // ADD END MARGE

//                        msg = msg + "ＨＰ・ＥＮ減少や状態付加等の特殊効果を無効化する。";
//                        break;
//                    }

//                default:
//                    {
//                        string localAllFeatureData1() { object argIndex1 = fname; var ret = u.AllFeatureData(ref argIndex1); return ret; }

//                        string localListIndex() { string arglist = hs055a0679e2434182a961742a50d72158(); var ret = GeneralLib.ListIndex(ref arglist, 1); return ret; }

//                        if (is_additional)
//                        {
//                            // 付加された能力の場合、ユニット用特殊能力に該当しなければ
//                            // パイロット用特殊能力とみなす
//                            msg = SkillHelpMessage(ref u.MainPilot(), ref ftype);
//                            if (Strings.Len(msg) > 0)
//                            {
//                                return FeatureHelpMessageRet;
//                            }

//                            // 実はダミー能力？
//                            if (Strings.Len(fdata) > 0)
//                            {
//                                msg = GeneralLib.ListIndex(ref fdata, GeneralLib.ListLength(ref fdata));
//                                if (Strings.Left(msg, 1) == "\"")
//                                {
//                                    msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
//                                }
//                            }

//                            // 解説が存在しない？
//                            if (Strings.Len(msg) == 0)
//                            {
//                                return FeatureHelpMessageRet;
//                            }
//                        }
//                        else if (Strings.Len(fdata) > 0)
//                        {
//                            // ダミー能力の場合
//                            msg = GeneralLib.ListIndex(ref fdata, GeneralLib.ListLength(ref fdata));
//                            if (Strings.Left(msg, 1) == "\"")
//                            {
//                                msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
//                            }
//                        }
//                        else if (localListIndex() != "解説")
//                        {
//                            // 解説がない場合
//                            return FeatureHelpMessageRet;
//                        }

//                        break;
//                    }
//            }

//            object argIndex86 = fname0;
//            fdata = u.AllFeatureData(ref argIndex86);
//            if (GeneralLib.ListIndex(ref fdata, 1) == "解説")
//            {
//                // 解説を定義している場合
//                msg = GeneralLib.ListTail(ref fdata, 2);
//                if (Strings.Left(msg, 1) == "\"")
//                {
//                    msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
//                }
//            }

//            // 等身大基準の際は「パイロット」という語を使わないようにする
//            string argoname2 = "等身大基準";
//            if (Expression.IsOptionDefined(ref argoname2))
//            {
//                string args2 = "パイロット";
//                string args3 = "ユニット";
//                GeneralLib.ReplaceString(ref msg, ref args2, ref args3);
//            }

//            FeatureHelpMessageRet = msg;
//            return FeatureHelpMessageRet;
//        }

//        // ユニット u の武器＆アビリティ属性 atr の名称
//        public static string AttributeName(ref Unit u, ref string atr, bool is_ability = false)
//        {
//            string AttributeNameRet = default;
//            string fdata;
//            switch (atr ?? "")
//            {
//                case "全":
//                    {
//                        AttributeNameRet = "全ての攻撃";
//                        break;
//                    }

//                case "格":
//                    {
//                        AttributeNameRet = "格闘系攻撃";
//                        break;
//                    }

//                case "射":
//                    {
//                        AttributeNameRet = "射撃系攻撃";
//                        break;
//                    }

//                case "複":
//                    {
//                        AttributeNameRet = "複合技";
//                        break;
//                    }

//                case "Ｐ":
//                    {
//                        AttributeNameRet = "移動後使用可能攻撃";
//                        break;
//                    }

//                case "Ｑ":
//                    {
//                        AttributeNameRet = "移動後使用不能攻撃";
//                        break;
//                    }

//                case "Ｒ":
//                    {
//                        AttributeNameRet = "低改造武器";
//                        break;
//                    }

//                case "改":
//                    {
//                        AttributeNameRet = "低改造武器";
//                        break;
//                    }

//                case "攻":
//                    {
//                        AttributeNameRet = "攻撃専用";
//                        break;
//                    }

//                case "反":
//                    {
//                        AttributeNameRet = "反撃専用";
//                        break;
//                    }

//                case "武":
//                    {
//                        AttributeNameRet = "格闘武器";
//                        break;
//                    }

//                case "突":
//                    {
//                        AttributeNameRet = "突進技";
//                        break;
//                    }

//                case "接":
//                    {
//                        AttributeNameRet = "接近戦攻撃";
//                        break;
//                    }

//                case "Ｊ":
//                    {
//                        AttributeNameRet = "ジャンプ攻撃";
//                        break;
//                    }

//                case "Ｂ":
//                    {
//                        AttributeNameRet = "ビーム攻撃";
//                        break;
//                    }

//                case "実":
//                    {
//                        AttributeNameRet = "実弾攻撃";
//                        break;
//                    }

//                case "オ":
//                    {
//                        AttributeNameRet = "オーラ技";
//                        break;
//                    }

//                case "超":
//                    {
//                        AttributeNameRet = "サイキック攻撃";
//                        break;
//                    }

//                case "シ":
//                    {
//                        AttributeNameRet = "同調率対象攻撃";
//                        break;
//                    }

//                case "サ":
//                    {
//                        AttributeNameRet = "思念誘導攻撃";
//                        break;
//                    }

//                case "体":
//                    {
//                        AttributeNameRet = "生命力換算攻撃";
//                        break;
//                    }

//                case "吸":
//                    {
//                        string argtname = "ＨＰ";
//                        AttributeNameRet = Expression.Term(ref argtname, ref u) + "吸収攻撃";
//                        break;
//                    }

//                case "減":
//                    {
//                        string argtname1 = "ＥＮ";
//                        AttributeNameRet = Expression.Term(ref argtname1, ref u) + "破壊攻撃";
//                        break;
//                    }

//                case "奪":
//                    {
//                        string argtname2 = "ＥＮ";
//                        AttributeNameRet = Expression.Term(ref argtname2, ref u) + "吸収攻撃";
//                        break;
//                    }

//                case "貫":
//                    {
//                        AttributeNameRet = "貫通攻撃";
//                        break;
//                    }

//                case "無":
//                    {
//                        AttributeNameRet = "バリア無効化攻撃";
//                        break;
//                    }

//                case "浄":
//                    {
//                        AttributeNameRet = "浄化技";
//                        break;
//                    }

//                case "封":
//                    {
//                        AttributeNameRet = "封印技";
//                        break;
//                    }

//                case "限":
//                    {
//                        AttributeNameRet = "限定技";
//                        break;
//                    }

//                case "殺":
//                    {
//                        AttributeNameRet = "抹殺攻撃";
//                        break;
//                    }

//                case "浸":
//                    {
//                        AttributeNameRet = "浸蝕攻撃";
//                        break;
//                    }

//                case "破":
//                    {
//                        AttributeNameRet = "シールド貫通攻撃";
//                        break;
//                    }

//                case "♂":
//                    {
//                        AttributeNameRet = "対男性用攻撃";
//                        break;
//                    }

//                case "♀":
//                    {
//                        AttributeNameRet = "対女性用攻撃";
//                        break;
//                    }

//                case "Ａ":
//                    {
//                        AttributeNameRet = "自動充填式攻撃";
//                        break;
//                    }

//                case "Ｃ":
//                    {
//                        AttributeNameRet = "チャージ式攻撃";
//                        break;
//                    }

//                case "合":
//                    {
//                        AttributeNameRet = "合体技";
//                        break;
//                    }

//                case "共":
//                    {
//                        if (!is_ability)
//                        {
//                            AttributeNameRet = "弾薬共有武器";
//                        }
//                        else
//                        {
//                            string argtname3 = "アビリティ";
//                            AttributeNameRet = "使用回数共有" + Expression.Term(ref argtname3, ref u);
//                        }

//                        break;
//                    }

//                case "斉":
//                    {
//                        AttributeNameRet = "一斉発射";
//                        break;
//                    }

//                case "永":
//                    {
//                        AttributeNameRet = "永続武器";
//                        break;
//                    }

//                case "術":
//                    {
//                        AttributeNameRet = "術";
//                        break;
//                    }

//                case "技":
//                    {
//                        AttributeNameRet = "技";
//                        break;
//                    }

//                case "視":
//                    {
//                        AttributeNameRet = "視覚攻撃";
//                        break;
//                    }

//                case "音":
//                    {
//                        if (!is_ability)
//                        {
//                            AttributeNameRet = "音波攻撃";
//                        }
//                        else
//                        {
//                            string argtname4 = "アビリティ";
//                            AttributeNameRet = "音波" + Expression.Term(ref argtname4, ref u);
//                        }

//                        break;
//                    }

//                case "気":
//                    {
//                        string argtname5 = "気力";
//                        AttributeNameRet = Expression.Term(ref argtname5, ref u) + "消費攻撃";
//                        break;
//                    }

//                case "霊":
//                case "プ":
//                    {
//                        AttributeNameRet = "霊力消費攻撃";
//                        break;
//                    }

//                case "失":
//                    {
//                        string argtname6 = "ＨＰ";
//                        AttributeNameRet = Expression.Term(ref argtname6, ref u) + "消費攻撃";
//                        break;
//                    }

//                case "銭":
//                    {
//                        string argtname7 = "資金";
//                        AttributeNameRet = Expression.Term(ref argtname7, ref u) + "消費攻撃";
//                        break;
//                    }

//                case "消":
//                    {
//                        AttributeNameRet = "消耗技";
//                        break;
//                    }

//                case "自":
//                    {
//                        AttributeNameRet = "自爆攻撃";
//                        break;
//                    }

//                case "変":
//                    {
//                        AttributeNameRet = "変形技";
//                        break;
//                    }

//                case "間":
//                    {
//                        AttributeNameRet = "間接攻撃";
//                        break;
//                    }

//                case "Ｍ直":
//                    {
//                        AttributeNameRet = "直線型マップ攻撃";
//                        break;
//                    }

//                case "Ｍ拡":
//                    {
//                        AttributeNameRet = "拡散型マップ攻撃";
//                        break;
//                    }

//                case "Ｍ扇":
//                    {
//                        AttributeNameRet = "扇型マップ攻撃";
//                        break;
//                    }

//                case "Ｍ全":
//                    {
//                        AttributeNameRet = "全方位型マップ攻撃";
//                        break;
//                    }

//                case "Ｍ投":
//                    {
//                        AttributeNameRet = "投下型マップ攻撃";
//                        break;
//                    }

//                case "Ｍ移":
//                    {
//                        AttributeNameRet = "移動型マップ攻撃";
//                        break;
//                    }

//                case "Ｍ線":
//                    {
//                        AttributeNameRet = "線状マップ攻撃";
//                        break;
//                    }

//                case "識":
//                    {
//                        AttributeNameRet = "識別型マップ攻撃";
//                        break;
//                    }

//                case "縛":
//                    {
//                        AttributeNameRet = "捕縛攻撃";
//                        break;
//                    }

//                case "Ｓ":
//                    {
//                        AttributeNameRet = "ショック攻撃";
//                        break;
//                    }

//                case "劣":
//                    {
//                        AttributeNameRet = "装甲劣化攻撃";
//                        break;
//                    }

//                case "中":
//                    {
//                        AttributeNameRet = "バリア中和攻撃";
//                        break;
//                    }

//                case "石":
//                    {
//                        AttributeNameRet = "石化攻撃";
//                        break;
//                    }

//                case "凍":
//                    {
//                        AttributeNameRet = "凍結攻撃";
//                        break;
//                    }

//                case "痺":
//                    {
//                        AttributeNameRet = "麻痺攻撃";
//                        break;
//                    }

//                case "眠":
//                    {
//                        AttributeNameRet = "催眠攻撃";
//                        break;
//                    }

//                case "乱":
//                    {
//                        AttributeNameRet = "混乱攻撃";
//                        break;
//                    }

//                case "魅":
//                    {
//                        AttributeNameRet = "魅了攻撃";
//                        break;
//                    }

//                case "憑":
//                    {
//                        AttributeNameRet = "憑依攻撃";
//                        break;
//                    }

//                case "盲":
//                    {
//                        AttributeNameRet = "目潰し攻撃";
//                        break;
//                    }

//                case "毒":
//                    {
//                        AttributeNameRet = "毒攻撃";
//                        break;
//                    }

//                case "撹":
//                    {
//                        AttributeNameRet = "撹乱攻撃";
//                        break;
//                    }

//                case "恐":
//                    {
//                        AttributeNameRet = "恐怖攻撃";
//                        break;
//                    }

//                case "不":
//                    {
//                        AttributeNameRet = "攻撃封印攻撃";
//                        break;
//                    }

//                case "止":
//                    {
//                        AttributeNameRet = "足止め攻撃";
//                        break;
//                    }

//                case "黙":
//                    {
//                        AttributeNameRet = "沈黙攻撃";
//                        break;
//                    }

//                case "除":
//                    {
//                        AttributeNameRet = "特殊効果除去攻撃";
//                        break;
//                    }

//                case "即":
//                    {
//                        AttributeNameRet = "即死攻撃";
//                        break;
//                    }

//                case "告":
//                    {
//                        AttributeNameRet = "死の宣告";
//                        break;
//                    }

//                case "脱":
//                    {
//                        string argtname8 = "気力";
//                        AttributeNameRet = Expression.Term(ref argtname8, ref u) + "減少攻撃";
//                        break;
//                    }

//                case "Ｄ":
//                    {
//                        string argtname9 = "気力";
//                        AttributeNameRet = Expression.Term(ref argtname9, ref u) + "吸収攻撃";
//                        break;
//                    }

//                case "低攻":
//                    {
//                        AttributeNameRet = "攻撃力低下攻撃";
//                        break;
//                    }

//                case "低防":
//                    {
//                        AttributeNameRet = "防御力低下攻撃";
//                        break;
//                    }

//                case "低運":
//                    {
//                        string argtname10 = "運動性";
//                        AttributeNameRet = Expression.Term(ref argtname10, ref u) + "低下攻撃";
//                        break;
//                    }

//                case "低移":
//                    {
//                        string argtname11 = "移動力";
//                        AttributeNameRet = Expression.Term(ref argtname11, ref u) + "低下攻撃";
//                        break;
//                    }

//                case "精":
//                    {
//                        AttributeNameRet = "精神攻撃";
//                        break;
//                    }

//                case "先":
//                    {
//                        AttributeNameRet = "先制攻撃";
//                        break;
//                    }

//                case "後":
//                    {
//                        AttributeNameRet = "後攻攻撃";
//                        break;
//                    }

//                case "連":
//                    {
//                        AttributeNameRet = "連続攻撃";
//                        break;
//                    }

//                case "再":
//                    {
//                        AttributeNameRet = "再攻撃";
//                        break;
//                    }

//                case "吹":
//                    {
//                        AttributeNameRet = "吹き飛ばし攻撃";
//                        break;
//                    }

//                case "Ｋ":
//                    {
//                        AttributeNameRet = "ノックバック攻撃";
//                        break;
//                    }

//                case "引":
//                    {
//                        AttributeNameRet = "引き寄せ攻撃";
//                        break;
//                    }

//                case "転":
//                    {
//                        AttributeNameRet = "強制転移攻撃";
//                        break;
//                    }

//                case "忍":
//                    {
//                        AttributeNameRet = "暗殺技";
//                        break;
//                    }

//                case "尽":
//                    {
//                        string argtname12 = "ＥＮ";
//                        AttributeNameRet = "全" + Expression.Term(ref argtname12, ref u) + "消費攻撃";
//                        break;
//                    }

//                case "盗":
//                    {
//                        AttributeNameRet = "盗み";
//                        break;
//                    }

//                case "Ｈ":
//                    {
//                        AttributeNameRet = "ホーミング攻撃";
//                        break;
//                    }

//                case "追":
//                    {
//                        AttributeNameRet = "自己追尾攻撃";
//                        break;
//                    }

//                case "有":
//                    {
//                        AttributeNameRet = "有線式誘導攻撃";
//                        break;
//                    }

//                case "誘":
//                    {
//                        AttributeNameRet = "特殊誘導攻撃";
//                        break;
//                    }

//                case "爆":
//                    {
//                        AttributeNameRet = "爆発攻撃";
//                        break;
//                    }

//                case "空":
//                    {
//                        AttributeNameRet = "対空攻撃";
//                        break;
//                    }

//                case "固":
//                    {
//                        AttributeNameRet = "ダメージ固定攻撃";
//                        break;
//                    }

//                case "衰":
//                    {
//                        string argtname13 = "ＨＰ";
//                        AttributeNameRet = Expression.Term(ref argtname13, ref u) + "減衰攻撃";
//                        break;
//                    }

//                case "滅":
//                    {
//                        string argtname14 = "ＥＮ";
//                        AttributeNameRet = Expression.Term(ref argtname14, ref u) + "減衰攻撃";
//                        break;
//                    }

//                case "踊":
//                    {
//                        AttributeNameRet = "踊らせ攻撃";
//                        break;
//                    }

//                case "狂":
//                    {
//                        AttributeNameRet = "狂戦士化攻撃";
//                        break;
//                    }

//                case "ゾ":
//                    {
//                        AttributeNameRet = "ゾンビ化攻撃";
//                        break;
//                    }

//                case "害":
//                    {
//                        AttributeNameRet = "回復能力阻害攻撃";
//                        break;
//                    }

//                case "習":
//                    {
//                        AttributeNameRet = "ラーニング";
//                        break;
//                    }

//                case "写":
//                    {
//                        AttributeNameRet = "能力コピー";
//                        break;
//                    }

//                case "化":
//                    {
//                        AttributeNameRet = "変化";
//                        break;
//                    }

//                case "痛":
//                    {
//                        AttributeNameRet = "クリティカル";
//                        break;
//                    }

//                case "援":
//                    {
//                        string argtname15 = "アビリティ";
//                        AttributeNameRet = "支援専用" + Expression.Term(ref argtname15, ref u);
//                        break;
//                    }

//                case "難":
//                    {
//                        string argtname16 = "アビリティ";
//                        AttributeNameRet = "高難度" + Expression.Term(ref argtname16, ref u);
//                        break;
//                    }

//                case "地":
//                case "水":
//                case "火":
//                case "風":
//                case "冷":
//                case "雷":
//                case "光":
//                case "闇":
//                case "聖":
//                case "死":
//                case "木":
//                    {
//                        AttributeNameRet = atr + "属性";
//                        break;
//                    }

//                case "魔":
//                    {
//                        AttributeNameRet = "魔法攻撃";
//                        break;
//                    }

//                case "時":
//                    {
//                        AttributeNameRet = "時間操作攻撃";
//                        break;
//                    }

//                case "重":
//                    {
//                        AttributeNameRet = "重力攻撃";
//                        break;
//                    }

//                case "銃":
//                case "剣":
//                case "刀":
//                case "槍":
//                case "斧":
//                case "弓":
//                    {
//                        AttributeNameRet = atr + "攻撃";
//                        break;
//                    }

//                case var @case when @case == "銃":
//                    {
//                        break;
//                    }

//                case "機":
//                    {
//                        AttributeNameRet = "対機械用攻撃";
//                        break;
//                    }

//                case "感":
//                    {
//                        AttributeNameRet = "対エスパー用攻撃";
//                        break;
//                    }

//                case "竜":
//                    {
//                        AttributeNameRet = "竜殺しの武器";
//                        break;
//                    }

//                case "瀕":
//                    {
//                        AttributeNameRet = "瀕死時限定攻撃";
//                        break;
//                    }

//                case "対":
//                    {
//                        AttributeNameRet = "特定レベル限定攻撃";
//                        break;
//                    }

//                case "ラ":
//                    {
//                        AttributeNameRet = "ラーニング可能技";
//                        break;
//                    }

//                case "禁":
//                    {
//                        AttributeNameRet = "使用禁止";
//                        break;
//                    }

//                case "小":
//                    {
//                        AttributeNameRet = "最小射程";
//                        break;
//                    }

//                case "散":
//                    {
//                        AttributeNameRet = "拡散攻撃";
//                        break;
//                    }

//                default:
//                    {
//                        if (Strings.Left(atr, 1) == "弱")
//                        {
//                            AttributeNameRet = Strings.Mid(atr, 2) + "属性弱点付加攻撃";
//                        }
//                        else if (Strings.Left(atr, 1) == "効")
//                        {
//                            AttributeNameRet = Strings.Mid(atr, 2) + "属性有効付加攻撃";
//                        }
//                        else if (Strings.Left(atr, 1) == "剋")
//                        {
//                            AttributeNameRet = Strings.Mid(atr, 2) + "属性使用妨害攻撃";
//                        }

//                        break;
//                    }
//            }

//            if (u is object)
//            {
//                object argIndex1 = atr;
//                fdata = u.FeatureData(ref argIndex1);
//                if (GeneralLib.ListIndex(ref fdata, 1) == "解説")
//                {
//                    // 解説を定義している場合
//                    AttributeNameRet = GeneralLib.ListIndex(ref fdata, 2);
//                    return AttributeNameRet;
//                }
//            }

//            if (is_ability)
//            {
//                if (Strings.Right(AttributeNameRet, 2) == "攻撃" | Strings.Right(AttributeNameRet, 2) == "武器")
//                {
//                    string argtname17 = "アビリティ";
//                    AttributeNameRet = Strings.Left(AttributeNameRet, Strings.Len(AttributeNameRet) - 2) + Expression.Term(ref argtname17, ref u);
//                }
//            }

//            return AttributeNameRet;
//        }

//        // ユニット u の idx 番目の武器＆アビリティの属性 atr の解説を表示
//        public static void AttributeHelp(ref Unit u, ref string atr, short idx, bool is_ability = false)
//        {
//            string msg, aname;
//            bool prev_mode;
//            msg = AttributeHelpMessage(ref u, ref atr, idx, is_ability);

//            // 解説の表示
//            if (Strings.Len(msg) > 0)
//            {
//                prev_mode = GUI.AutoMessageMode;
//                GUI.AutoMessageMode = false;
//                Unit argu1 = null;
//                Unit argu2 = null;
//                GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
//                if (SRC.AutoMoveCursor)
//                {
//                    string argcursor_mode = "メッセージウィンドウ";
//                    GUI.MoveCursorPos(ref argcursor_mode);
//                }

//                if (Strings.InStr(atr, "L") > 0)
//                {
//                    string localAttributeName() { string argatr = Strings.Left(atr, Strings.InStr(atr, "L") - 1); var ret = AttributeName(ref u, ref argatr, is_ability); return ret; }

//                    aname = localAttributeName() + "レベル" + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Strings.Mid(atr, Strings.InStr(atr, "L") + 1)), VbStrConv.Wide);
//                }
//                else
//                {
//                    aname = AttributeName(ref u, ref atr, is_ability);
//                }

//                string argpname = "システム";
//                GUI.DisplayMessage(ref argpname, "<b>" + aname + "</b>;" + msg);
//                GUI.CloseMessageForm();
//                GUI.AutoMessageMode = prev_mode;
//            }
//        }

//        // ユニット u の idx 番目の武器＆アビリティの属性 atr の解説を表示
//        public static string AttributeHelpMessage(ref Unit u, ref string atr, short idx, bool is_ability)
//        {
//            string AttributeHelpMessageRet = default;
//            string atype;
//            double alevel;
//            string msg = default, whatsthis;
//            string wanickname, waname, uname = default;
//            Pilot p;
//            short i, j;
//            string buf;
//            string fdata;

//            // 属性レベルの収得
//            if (Strings.InStr(atr, "L") > 0)
//            {
//                atype = Strings.Left(atr, Strings.InStr(atr, "L") - 1);
//                alevel = Conversions.ToDouble(Strings.Mid(atr, Strings.InStr(atr, "L") + 1));
//            }
//            else
//            {
//                atype = atr;
//                alevel = SRC.DEFAULT_LEVEL;
//            }

//            {
//                var withBlock = u;
//                // 武器(アビリティ)名
//                if (!is_ability)
//                {
//                    waname = withBlock.Weapon(idx).Name;
//                    wanickname = withBlock.WeaponNickname(idx);
//                    whatsthis = "攻撃";
//                }
//                else
//                {
//                    waname = withBlock.Ability(idx).Name;
//                    wanickname = withBlock.AbilityNickname(idx);
//                    string argtname = "アビリティ";
//                    whatsthis = Expression.Term(ref argtname, ref u);
//                }

//                // メインパイロット
//                p = withBlock.MainPilot();
//            }

//            switch (atype ?? "")
//            {
//                case "格":
//                    {
//                        string argtname1 = "格闘";
//                        msg = "パイロットの" + Expression.Term(ref argtname1, ref u) + "を使って攻撃力を算出。";
//                        break;
//                    }

//                case "射":
//                    {
//                        if (p.HasMana())
//                        {
//                            string argtname2 = "魔力";
//                            msg = "パイロットの" + Expression.Term(ref argtname2, ref u) + "を使って攻撃力を算出。";
//                        }
//                        else
//                        {
//                            string argtname3 = "射撃";
//                            msg = "パイロットの" + Expression.Term(ref argtname3, ref u) + "を使って攻撃力を算出。";
//                        }

//                        break;
//                    }

//                case "複":
//                    {
//                        if (p.HasMana())
//                        {
//                            string argtname4 = "格闘";
//                            string argtname5 = "魔力";
//                            msg = "格闘と魔法の両方を使った攻撃。" + "パイロットの" + Expression.Term(ref argtname4, ref u) + "と" + Expression.Term(ref argtname5, ref u) + "の" + "平均値を使って攻撃力を算出する。";
//                        }
//                        else
//                        {
//                            string argtname6 = "格闘";
//                            string argtname7 = "射撃";
//                            msg = "格闘と射撃の両方を使った攻撃。" + "パイロットの" + Expression.Term(ref argtname6, ref u) + "と" + Expression.Term(ref argtname7, ref u) + "の" + "平均値を使って攻撃力を算出する。";
//                        }

//                        break;
//                    }

//                case "Ｐ":
//                    {
//                        msg = "射程にかかわらず移動後に使用可能。";
//                        break;
//                    }

//                case "Ｑ":
//                    {
//                        msg = "射程にかかわらず移動後は使用不能。";
//                        break;
//                    }

//                case "攻":
//                    {
//                        msg = "攻撃時にのみ使用可能。";
//                        break;
//                    }

//                case "反":
//                    {
//                        msg = "反撃時にのみ使用可能。";
//                        break;
//                    }

//                case "Ｒ":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            msg = "ユニットランクや特殊能力による攻撃力上昇が通常の半分。";
//                        }
//                        else
//                        {
//                            msg = "ユニットランクや特殊能力による攻撃力上昇が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * alevel) + "％になる。";
//                        }

//                        msg = "ユニットランクや特殊能力による攻撃力上昇が通常の半分。";
//                        break;
//                    }

//                case "改":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            msg = "ユニットランクによる攻撃力上昇が通常の半分。";
//                        }
//                        else
//                        {
//                            msg = "ユニットランクによる攻撃力上昇が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * alevel) + "％になる。";
//                        }

//                        break;
//                    }

//                case "武":
//                    {
//                        msg = "この武器を使って実弾攻撃などを切り払うことが可能。" + "切り払いの対象になる。";
//                        break;
//                    }

//                case "突":
//                    {
//                        msg = "切り払いの対象になる。";
//                        break;
//                    }

//                case "接":
//                    {
//                        msg = "投げ技等、相手に密着して繰り出す格闘戦攻撃。;" + "切り払い無効。";
//                        break;
//                    }

//                case "Ｊ":
//                    {
//                        msg = "ジャンプ攻撃時の地形適応を指定したレベルだけ上げる。";
//                        break;
//                    }

//                case "Ｂ":
//                    {
//                        msg = "対ビーム用防御能力の対象になる。";
//                        break;
//                    }

//                case "実":
//                    {
//                        msg = "切り払いと迎撃の対象になる。";
//                        string argoname = "距離修正";
//                        if (Expression.IsOptionDefined(ref argoname))
//                        {
//                            msg = msg + "長距離の敵を攻撃する際もダメージが低下しない。";
//                        }

//                        break;
//                    }

//                case "オ":
//                    {
//                        object argIndex1 = "オーラ";
//                        msg = "パイロットの" + p.SkillName0(ref argIndex1) + "レベルによって攻撃力が変化。";
//                        break;
//                    }

//                case "超":
//                    {
//                        object argIndex2 = "超能力";
//                        msg = "パイロットの" + p.SkillName0(ref argIndex2) + "レベルによって攻撃力が変化。";
//                        break;
//                    }

//                case "シ":
//                    {
//                        object argIndex3 = "同調率";
//                        msg = "パイロットの" + p.SkillName0(ref argIndex3) + "によって攻撃力が変化。";
//                        break;
//                    }

//                case "サ":
//                    {
//                        object argIndex4 = "超感覚";
//                        msg = "パイロットの" + p.SkillName0(ref argIndex4) + "レベルによって射程が変化。";
//                        string argoname1 = "距離修正";
//                        if (Expression.IsOptionDefined(ref argoname1))
//                        {
//                            msg = msg + "距離による命中率低下がない。また、";
//                        }

//                        msg = msg + "ＥＣＭによる影響を受けない。";
//                        break;
//                    }

//                case "体":
//                    {
//                        string argtname8 = "ＨＰ";
//                        msg = "生命力を攻撃力に換える攻撃。ユニットの" + Expression.Term(ref argtname8, ref u) + "によって攻撃力が変化する。";
//                        break;
//                    }

//                case "吸":
//                    {
//                        string argtname9 = "ＨＰ";
//                        msg = "与えたダメージの１／４を吸収し、自分の" + Expression.Term(ref argtname9, ref u) + "に変換。";
//                        break;
//                    }

//                case "減":
//                    {
//                        string argtname10 = "ＨＰ";
//                        string argtname11 = "ＥＮ";
//                        msg = Expression.Term(ref argtname10, ref u) + "にダメージを与えると同時に相手の" + Expression.Term(ref argtname11, ref u) + "を減少させる。";
//                        break;
//                    }

//                case "奪":
//                    {
//                        string argtname12 = "ＨＰ";
//                        string argtname13 = "ＥＮ";
//                        string argtname14 = "ＥＮ";
//                        msg = Expression.Term(ref argtname12, ref u) + "にダメージを与えると同時に相手の" + Expression.Term(ref argtname13, ref u) + "を減少させ、" + "減少させた" + Expression.Term(ref argtname14, ref u) + "の半分を自分のものにする。";
//                        break;
//                    }

//                case "貫":
//                    {
//                        if (alevel > 0d)
//                        {
//                            string argtname15 = "装甲";
//                            msg = "相手の" + Expression.Term(ref argtname15, ref u) + "を本来の" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(100d - 10d * alevel) + "％の値とみなしてダメージ計算を行う。";
//                        }
//                        else
//                        {
//                            string argtname16 = "装甲";
//                            msg = "相手の" + Expression.Term(ref argtname16, ref u) + "を半分とみなしてダメージ計算を行う。";
//                        }

//                        break;
//                    }

//                case "無":
//                    {
//                        msg = "バリアやフィールドなどの防御能力の効果を無視してダメージを与える。";
//                        break;
//                    }

//                case "浸":
//                    {
//                        msg = "シールド防御を無視してダメージを与える。";
//                        break;
//                    }

//                case "破":
//                    {
//                        msg = "シールド防御の効果を半減させる。";
//                        break;
//                    }

//                case "浄":
//                    {
//                        object argIndex5 = "再生";
//                        msg = "敵の" + p.SkillName0(ref argIndex5) + "能力を無効化。";
//                        break;
//                    }

//                case "封":
//                    {
//                        msg = "特定の弱点を持つ敵にのみ有効な武装。" + "弱点をついたときにのみダメージを与えることが出来る。";
//                        break;
//                    }

//                case "限":
//                    {
//                        msg = "特定の弱点を持つ敵にのみ有効な武装。" + "限定属性以降に指定した属性で;" + "弱点をついたときにのみダメージを与えることが出来る。";
//                        break;
//                    }

//                case "殺":
//                    {
//                        msg = "相手を一撃で倒せる場合にのみ有効な攻撃。;" + "相手は防御＆シールド防御出来ない。";
//                        break;
//                    }

//                case "♂":
//                    {
//                        msg = "男性にのみ有効。";
//                        break;
//                    }

//                case "♀":
//                    {
//                        msg = "女性にのみ有効。";
//                        break;
//                    }

//                case "Ｃ":
//                    {
//                        msg = "チャージコマンドを使用してチャージ完了の状態にならないと使用不能。";
//                        break;
//                    }

//                case "Ａ":
//                    {
//                        msg = "使用すると" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(alevel) + "ターン後に再チャージが完了するまで使用不能。";
//                        if (!is_ability)
//                        {
//                            var loopTo = u.CountWeapon();
//                            for (i = 1; i <= loopTo; i++)
//                            {
//                                if (i != idx & (wanickname ?? "") == (u.WeaponNickname(i) ?? ""))
//                                {
//                                    msg = msg + "同名の武器も連動して使用不能になる。";
//                                    break;
//                                }
//                            }

//                            string argattr = "共";
//                            if (u.IsWeaponClassifiedAs(idx, ref argattr) & u.Weapon(idx).Bullet == 0)
//                            {
//                                msg = msg + "同レベルの弾薬共有武器も連動して使用不能になる。";
//                            }
//                        }
//                        else
//                        {
//                            var loopTo1 = u.CountAbility();
//                            for (i = 1; i <= loopTo1; i++)
//                            {
//                                if (i != idx & (wanickname ?? "") == (u.AbilityNickname(i) ?? ""))
//                                {
//                                    string argtname17 = "アビリティ";
//                                    msg = msg + "同名の" + Expression.Term(ref argtname17, ref u) + "も連動して使用不能になる。";
//                                    break;
//                                }
//                            }

//                            string argattr1 = "共";
//                            if (u.IsAbilityClassifiedAs(idx, ref argattr1) & u.Ability(idx).Stock == 0)
//                            {
//                                string argtname18 = "アビリティ";
//                                msg = msg + "同レベルの使用回数共有" + Expression.Term(ref argtname18, ref u) + "も連動して使用不能になる。";
//                            }
//                        }

//                        break;
//                    }

//                case "合":
//                    {
//                        var loopTo2 = u.CountFeature();
//                        for (i = 1; i <= loopTo2; i++)
//                        {
//                            string localFeature() { object argIndex1 = i; var ret = u.Feature(ref argIndex1); return ret; }

//                            string localFeatureData() { object argIndex1 = i; var ret = u.FeatureData(ref argIndex1); return ret; }

//                            string localLIndex() { string arglist = hsa15788173c3b4c89b40f3ad0a57a4d4e(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

//                            if (localFeature() == "合体技" & (localLIndex() ?? "") == (waname ?? ""))
//                            {
//                                break;
//                            }
//                        }

//                        if (i > u.CountFeature())
//                        {
//                            string argmsg = u.Name + "の合体技「" + waname + "」に対応した合体技能力がありません";
//                            GUI.ErrorMessage(ref argmsg);
//                            return AttributeHelpMessageRet;
//                        }

//                        string localFeatureData4() { object argIndex1 = i; var ret = u.FeatureData(ref argIndex1); return ret; }

//                        string arglist3 = localFeatureData4();
//                        if (GeneralLib.LLength(ref arglist3) == 2)
//                        {
//                            string localFeatureData1() { object argIndex1 = i; var ret = u.FeatureData(ref argIndex1); return ret; }

//                            string arglist = localFeatureData1();
//                            uname = GeneralLib.LIndex(ref arglist, 2);
//                            object argIndex6 = uname;
//                            if (SRC.UDList.IsDefined(ref argIndex6))
//                            {
//                                UnitData localItem() { object argIndex1 = uname; var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                                uname = localItem().Nickname;
//                            }

//                            if ((uname ?? "") == (u.Nickname ?? ""))
//                            {
//                                msg = "他の" + uname + "と協力して行う技。";
//                            }
//                            else
//                            {
//                                msg = uname + "と協力して行う技。";
//                            }
//                        }
//                        else
//                        {
//                            msg = "以下のユニットと協力して行う技。;";
//                            string localFeatureData3() { object argIndex1 = i; var ret = u.FeatureData(ref argIndex1); return ret; }

//                            string arglist2 = localFeatureData3();
//                            var loopTo3 = GeneralLib.LLength(ref arglist2);
//                            for (j = 2; j <= loopTo3; j++)
//                            {
//                                string localFeatureData2() { object argIndex1 = i; var ret = u.FeatureData(ref argIndex1); return ret; }

//                                string arglist1 = localFeatureData2();
//                                uname = GeneralLib.LIndex(ref arglist1, j);
//                                object argIndex7 = uname;
//                                if (SRC.UDList.IsDefined(ref argIndex7))
//                                {
//                                    UnitData localItem1() { object argIndex1 = uname; var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                                    uname = localItem1().Nickname;
//                                }

//                                msg = msg + uname + "  ";
//                            }
//                        }

//                        break;
//                    }

//                case "共":
//                    {
//                        if (!is_ability)
//                        {
//                            msg = "複数の武器で弾薬を共有していることを示す。";
//                            if (alevel > 0d)
//                            {
//                                msg = msg + ";同レベルの弾薬共有武器間で弾薬を共有している。";
//                            }
//                        }
//                        else
//                        {
//                            string argtname19 = "アビリティ";
//                            msg = "複数の" + Expression.Term(ref argtname19, ref u) + "で使用回数を共有していることを示す。";
//                            if (alevel > 0d)
//                            {
//                                string argtname20 = "アビリティ";
//                                msg = msg + ";同レベルの使用回数共有" + Expression.Term(ref argtname20, ref u) + "間で使用回数を共有している。";
//                            }
//                        }

//                        break;
//                    }

//                case "斉":
//                    {
//                        if (!is_ability)
//                        {
//                            msg = "弾数制の武器全ての弾数を消費して攻撃を行う。";
//                        }
//                        else
//                        {
//                            string argtname21 = "アビリティ";
//                            msg = "回数制の" + Expression.Term(ref argtname21, ref u) + "全ての使用回数を消費する。";
//                        }

//                        break;
//                    }

//                case "永":
//                    {
//                        msg = "切り払いや迎撃されない限り弾数が減少しない。";
//                        break;
//                    }

//                case "術":
//                    {
//                        object argIndex8 = "術";
//                        buf = p.SkillName0(ref argIndex8);
//                        if (buf == "非表示")
//                        {
//                            buf = "術";
//                        }

//                        string argtname22 = "ＥＮ";
//                        msg = buf + "技能によって" + Expression.Term(ref argtname22, ref u) + "消費量が減少。";
//                        if (is_ability)
//                        {
//                            string argtname23 = "魔力";
//                            msg = msg + ";パイロットの" + Expression.Term(ref argtname23, ref u) + "によって威力が増減する。";
//                        }

//                        msg = msg + ";沈黙状態の時には使用不能｡";
//                        break;
//                    }

//                case "技":
//                    {
//                        object argIndex9 = "技";
//                        buf = p.SkillName0(ref argIndex9);
//                        if (buf == "非表示")
//                        {
//                            buf = "技";
//                        }

//                        string argtname24 = "ＥＮ";
//                        msg = buf + "技能によって" + Expression.Term(ref argtname24, ref u) + "消費量が減少。";
//                        break;
//                    }

//                case "音":
//                    {
//                        if (!is_ability)
//                        {
//                            msg = "声などの音を使った攻撃であることを示す｡";
//                        }
//                        else
//                        {
//                            string argtname25 = "アビリティ";
//                            msg = "声などの音を使った" + Expression.Term(ref argtname25, ref u) + "であることを示す｡";
//                        }

//                        msg = msg + "沈黙状態の時には使用不能｡ ";
//                        break;
//                    }

//                case "視":
//                    {
//                        msg = "視覚に働きかける攻撃。盲目状態のユニットには効かない。";
//                        break;
//                    }

//                case "気":
//                    {
//                        msg = "使用時に気力" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(5d * alevel) + "を消費。";
//                        break;
//                    }

//                case "霊":
//                case "プ":
//                    {
//                        object argIndex10 = "霊力";
//                        msg = "使用時に" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(5d * alevel) + p.SkillName0(ref argIndex10) + "を消費。";
//                        break;
//                    }

//                case "失":
//                    {
//                        string argtname26 = "ＨＰ";
//                        msg = "使用時に" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((long)(alevel * u.MaxHP) / 10L) + "の" + Expression.Term(ref argtname26, ref u) + "を失う。";
//                        break;
//                    }

//                case "銭":
//                    {
//                        string argtname27 = "資金";
//                        string argtname28 = "資金";
//                        msg = "使用時に" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(GeneralLib.MaxLng((int)alevel, 1) * u.Value / 10) + "の" + Expression.Term(ref argtname27, ref u) + "が必要。;" + Expression.Term(ref argtname28, ref u) + "が足りない場合は使用不可。";
//                        break;
//                    }

//                case "消":
//                    {
//                        msg = "使用後に1ターン消耗状態に陥り、回避・反撃不能。";
//                        break;
//                    }

//                case "尽":
//                    {
//                        if (!is_ability)
//                        {
//                            if (alevel > 0d)
//                            {
//                                string argtname29 = "ＥＮ";
//                                string argtname30 = "ＥＮ";
//                                string argtname31 = "ＥＮ";
//                                string argtname32 = "ＥＮ";
//                                msg = "全" + Expression.Term(ref argtname29, ref u) + "を使って攻撃し、使用後に" + Expression.Term(ref argtname30, ref u) + "が0になる。;" + "(残り" + Expression.Term(ref argtname31, ref u) + "－必要" + Expression.Term(ref argtname32, ref u) + ")×" + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(alevel), VbStrConv.Wide) + "だけ攻撃力が上昇。";
//                            }
//                            else
//                            {
//                                string argtname33 = "ＥＮ";
//                                msg = "全" + Expression.Term(ref argtname33, ref u) + "を使って攻撃し、使用後にＥＮが0になる。";
//                            }
//                        }
//                        else
//                        {
//                            string argtname34 = "ＥＮ";
//                            msg = "使用後に" + Expression.Term(ref argtname34, ref u) + "が0になる。";
//                        }

//                        break;
//                    }

//                case "自":
//                    {
//                        msg = "使用後に自爆。";
//                        break;
//                    }

//                case "変":
//                    {
//                        string argfname = "変形技";
//                        if (u.IsFeatureAvailable(ref argfname))
//                        {
//                            var loopTo4 = u.CountFeature();
//                            for (i = 1; i <= loopTo4; i++)
//                            {
//                                string localFeature1() { object argIndex1 = i; var ret = u.Feature(ref argIndex1); return ret; }

//                                string localFeatureData6() { object argIndex1 = i; var ret = u.FeatureData(ref argIndex1); return ret; }

//                                string localLIndex1() { string arglist = hsebc97fcb17064e2d82d9baad376850c8(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

//                                if (localFeature1() == "変形技" & (localLIndex1() ?? "") == (waname ?? ""))
//                                {
//                                    string localFeatureData5() { object argIndex1 = i; var ret = u.FeatureData(ref argIndex1); return ret; }

//                                    string arglist4 = localFeatureData5();
//                                    uname = GeneralLib.LIndex(ref arglist4, 2);
//                                    break;
//                                }
//                            }
//                        }

//                        if (string.IsNullOrEmpty(uname))
//                        {
//                            object argIndex11 = "ノーマルモード";
//                            string arglist5 = u.FeatureData(ref argIndex11);
//                            uname = GeneralLib.LIndex(ref arglist5, 1);
//                        }

//                        object argIndex13 = uname;
//                        if (SRC.UDList.IsDefined(ref argIndex13))
//                        {
//                            object argIndex12 = uname;
//                            {
//                                var withBlock1 = SRC.UDList.Item(ref argIndex12);
//                                if ((u.Nickname ?? "") != (withBlock1.Nickname ?? ""))
//                                {
//                                    uname = withBlock1.Nickname;
//                                }
//                                else
//                                {
//                                    uname = withBlock1.Name;
//                                }
//                            }
//                        }

//                        msg = "使用後に" + uname + "へ変化する。";
//                        break;
//                    }

//                case "間":
//                    {
//                        msg = "視界外などから間接的に攻撃を行うことにより" + "相手の反撃を封じる武器。";
//                        break;
//                    }

//                case "Ｍ直":
//                    {
//                        msg = "上下左右の一方向に対する直線状の効果範囲を持つ。";
//                        break;
//                    }

//                case "Ｍ拡":
//                    {
//                        msg = "上下左右の一方向に対する幅３マスの直線状の効果範囲を持つ。";
//                        break;
//                    }

//                case "Ｍ扇":
//                    {
//                        msg = "上下左右の一方向に対する扇状の効果範囲を持つ。;" + "扇の広がり方の度合いはレベルによって異なる。";
//                        break;
//                    }

//                case "Ｍ全":
//                    {
//                        msg = "ユニットの周り全域に対する効果範囲を持つ。";
//                        break;
//                    }

//                case "Ｍ投":
//                    {
//                        msg = "指定した地点を中心とした一定範囲の効果範囲を持つ。";
//                        break;
//                    }

//                case "Ｍ移":
//                    {
//                        msg = "使用後に指定した地点までユニットが移動し、" + "ユニットが通過した場所が効果範囲になる。";
//                        break;
//                    }

//                case "Ｍ線":
//                    {
//                        msg = "指定した地点とユニットを結ぶ直線が効果範囲になる。";
//                        break;
//                    }

//                case "識":
//                    {
//                        msg = "効果範囲内にいる味方ユニットを自動的に識別し、敵のみにダメージを与える。";
//                        break;
//                    }

//                case "縛":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            alevel = 2d;
//                        }

//                        msg = "クリティカル発生時に相手を";
//                        if (alevel > 0d)
//                        {
//                            msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                        }
//                        else
//                        {
//                            msg = msg + "その戦闘中のみ";
//                        }

//                        msg = msg + "行動不能にする。";
//                        break;
//                    }

//                case "Ｓ":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            alevel = 1d;
//                        }

//                        msg = "クリティカル発生時に相手を";
//                        if (alevel > 0d)
//                        {
//                            msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                        }
//                        else
//                        {
//                            msg = msg + "その戦闘中のみ";
//                        }

//                        msg = msg + "行動不能にする。";
//                        break;
//                    }

//                case "劣":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            msg = "クリティカル発生時に相手の装甲を半減させる。";
//                        }
//                        else
//                        {
//                            msg = "クリティカル発生時に相手の装甲を";
//                            if (alevel > 0d)
//                            {
//                                msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                            }
//                            else
//                            {
//                                msg = msg + "その戦闘中のみ";
//                            }

//                            msg = msg + "半減させる。";
//                        }

//                        break;
//                    }

//                case "中":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            alevel = 1d;
//                        }

//                        msg = "クリティカル発生時に相手が持つバリア等の防御能力を";
//                        if (alevel > 0d)
//                        {
//                            msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                        }
//                        else
//                        {
//                            msg = msg + "その戦闘中のみ";
//                        }

//                        msg = msg + "無効化する。";
//                        break;
//                    }

//                case "石":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            msg = "クリティカル発生時に相手を石化させる。";
//                        }
//                        else
//                        {
//                            msg = "クリティカル発生時に相手を";
//                            if (alevel > 0d)
//                            {
//                                msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                            }
//                            else
//                            {
//                                msg = msg + "その戦闘中のみ";
//                            }

//                            msg = msg + "石化させる。";
//                        }

//                        break;
//                    }

//                case "凍":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            alevel = 3d;
//                        }

//                        msg = "クリティカル発生時に相手を";
//                        if (alevel > 0d)
//                        {
//                            msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                        }
//                        else
//                        {
//                            msg = msg + "その戦闘中のみ";
//                        }

//                        msg = msg + "凍らせる。";
//                        string argtname35 = "装甲";
//                        msg = msg + ";凍結した相手は" + Expression.Term(ref argtname35, ref u) + "が半減するが、";
//                        msg = msg + "ダメージを与えると凍結は解除される。";
//                        break;
//                    }

//                case "痺":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            alevel = 3d;
//                        }

//                        msg = "クリティカル発生時に相手を";
//                        if (alevel > 0d)
//                        {
//                            msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                        }
//                        else
//                        {
//                            msg = msg + "その戦闘中のみ";
//                        }

//                        msg = msg + "麻痺させる。";
//                        break;
//                    }

//                case "眠":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            alevel = 3d;
//                        }

//                        msg = "クリティカル発生時に相手を";
//                        if (alevel > 0d)
//                        {
//                            msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                        }
//                        else
//                        {
//                            msg = msg + "その戦闘中のみ";
//                        }

//                        msg = msg + "眠らせる。";
//                        msg = msg + ";眠った相手への攻撃のダメージは１.５倍になるが、睡眠も解除される。";
//                        msg = msg + ";性格が機械の敵には無効。";
//                        break;
//                    }

//                case "乱":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            alevel = 3d;
//                        }

//                        msg = "クリティカル発生時に相手を";
//                        if (alevel > 0d)
//                        {
//                            msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                        }
//                        else
//                        {
//                            msg = msg + "その戦闘中のみ";
//                        }

//                        msg = msg + "混乱させる。";
//                        break;
//                    }

//                case "魅":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            alevel = 3d;
//                        }

//                        msg = "クリティカル発生時に相手を";
//                        if (alevel > 0d)
//                        {
//                            msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                        }
//                        else
//                        {
//                            msg = msg + "その戦闘中のみ";
//                        }

//                        msg = msg + "魅了する。";
//                        break;
//                    }

//                case "憑":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            msg = "クリティカル発生時に相手を乗っ取って支配する。";
//                        }
//                        else
//                        {
//                            msg = "クリティカル発生時に相手を";
//                            if (alevel > 0d)
//                            {
//                                msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                            }
//                            else
//                            {
//                                msg = msg + "その戦闘中のみ";
//                            }

//                            msg = msg + "乗っ取って支配する。";
//                        }

//                        break;
//                    }

//                case "盲":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            alevel = 3d;
//                        }

//                        msg = "クリティカル発生時に相手を";
//                        if (alevel > 0d)
//                        {
//                            msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                        }
//                        else
//                        {
//                            msg = msg + "その戦闘中のみ";
//                        }

//                        msg = msg + "盲目にする。";
//                        break;
//                    }

//                case "毒":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            alevel = 3d;
//                        }

//                        msg = "クリティカル発生時に相手を";
//                        if (alevel > 0d)
//                        {
//                            msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                        }
//                        else
//                        {
//                            msg = msg + "その戦闘中のみ";
//                        }

//                        msg = msg + "毒状態にする。";
//                        break;
//                    }

//                case "撹":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            alevel = 2d;
//                        }

//                        msg = "クリティカル発生時に相手を";
//                        if (alevel > 0d)
//                        {
//                            msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                        }
//                        else
//                        {
//                            msg = msg + "その戦闘中のみ";
//                        }

//                        msg = msg + "撹乱する。";
//                        break;
//                    }

//                case "恐":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            alevel = 3d;
//                        }

//                        msg = "クリティカル発生時に相手を";
//                        if (alevel > 0d)
//                        {
//                            msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                        }
//                        else
//                        {
//                            msg = msg + "その戦闘中のみ";
//                        }

//                        msg = msg + "恐怖に陥れる。";
//                        break;
//                    }

//                case "不":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            alevel = 1d;
//                        }

//                        msg = "クリティカル発生時に相手を";
//                        if (alevel > 0d)
//                        {
//                            msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                        }
//                        else
//                        {
//                            msg = msg + "その戦闘中のみ";
//                        }

//                        msg = msg + "攻撃不能にする。";
//                        break;
//                    }

//                case "止":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            alevel = 1d;
//                        }

//                        msg = "クリティカル発生時に相手を";
//                        if (alevel > 0d)
//                        {
//                            msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                        }
//                        else
//                        {
//                            msg = msg + "その戦闘中のみ";
//                        }

//                        msg = msg + "移動不能にする。";
//                        break;
//                    }

//                case "黙":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            alevel = 3d;
//                        }

//                        msg = "クリティカル発生時に相手を";
//                        if (alevel > 0d)
//                        {
//                            msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                        }
//                        else
//                        {
//                            msg = msg + "その戦闘中のみ";
//                        }

//                        msg = msg + "沈黙状態にする。";
//                        break;
//                    }

//                case "除":
//                    {
//                        if (!is_ability)
//                        {
//                            string argtname36 = "アビリティ";
//                            msg = "クリティカル発生時に相手にかけられた" + Expression.Term(ref argtname36, ref u) + "による特殊効果を打ち消す。";
//                        }
//                        else
//                        {
//                            string argtname37 = "アビリティ";
//                            string argtname38 = "アビリティ";
//                            msg = Expression.Term(ref argtname37, ref u) + "実行時に、それまでに相手にかけられていた" + Expression.Term(ref argtname38, ref u) + "による特殊効果が解除される。";
//                        }

//                        break;
//                    }

//                case "即":
//                    {
//                        msg = "クリティカル発生時に相手を即死させる。";
//                        break;
//                    }

//                case "告":
//                    {
//                        if (alevel > 0d)
//                        {
//                            string argtname39 = "ＨＰ";
//                            msg = "クリティカル発生時に相手を「死の宣告」状態にし、" + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン後に" + Expression.Term(ref argtname39, ref u) + "を１にする。";
//                        }
//                        else
//                        {
//                            string argtname40 = "ＨＰ";
//                            msg = "クリティカル発生時に相手の" + Expression.Term(ref argtname40, ref u) + "を１にする。";
//                        }

//                        break;
//                    }

//                case "脱":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            string argtname41 = "気力";
//                            msg = "相手の" + Expression.Term(ref argtname41, ref u) + "を10低下させる。";
//                        }
//                        else if (alevel >= 0d)
//                        {
//                            string argtname43 = "気力";
//                            msg = "相手の" + Expression.Term(ref argtname43, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(5d * alevel)) + "低下させる。";
//                        }
//                        else
//                        {
//                            string argtname42 = "気力";
//                            msg = "相手の" + Expression.Term(ref argtname42, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(-5 * alevel)) + "増加させる。";
//                        }

//                        break;
//                    }

//                case "Ｄ":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            string argtname44 = "気力";
//                            msg = "相手の" + Expression.Term(ref argtname44, ref u) + "を10低下させ、その半分を吸収する。";
//                        }
//                        else if (alevel >= 0d)
//                        {
//                            string argtname46 = "気力";
//                            msg = "相手の" + Expression.Term(ref argtname46, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(5d * alevel)) + "低下させ、その半分を吸収する。";
//                        }
//                        else
//                        {
//                            string argtname45 = "気力";
//                            msg = "相手の" + Expression.Term(ref argtname45, ref u) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)(-5 * alevel)) + "増加させ、その半分を与える。";
//                        }

//                        break;
//                    }

//                case "低攻":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            alevel = 3d;
//                        }

//                        msg = "クリティカル発生時に相手の攻撃力を";
//                        if (alevel > 0d)
//                        {
//                            msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                        }
//                        else
//                        {
//                            msg = msg + "その戦闘中のみ";
//                        }

//                        msg = msg + "低下させる。";
//                        break;
//                    }

//                case "低防":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            alevel = 3d;
//                        }

//                        string argtname47 = "装甲";
//                        msg = "クリティカル発生時に相手の" + Expression.Term(ref argtname47, ref u) + "を";
//                        if (alevel > 0d)
//                        {
//                            msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                        }
//                        else
//                        {
//                            msg = msg + "その戦闘中のみ";
//                        }

//                        msg = msg + "低下させる。";
//                        break;
//                    }

//                case "低運":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            alevel = 3d;
//                        }

//                        string argtname48 = "運動性";
//                        msg = "クリティカル発生時に相手の" + Expression.Term(ref argtname48, ref u) + "を";
//                        if (alevel > 0d)
//                        {
//                            msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                        }
//                        else
//                        {
//                            msg = msg + "その戦闘中のみ";
//                        }

//                        msg = msg + "低下させる。";
//                        break;
//                    }

//                case "低移":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            alevel = 3d;
//                        }

//                        string argtname49 = "移動力";
//                        msg = "クリティカル発生時に相手の" + Expression.Term(ref argtname49, ref u) + "を";
//                        if (alevel > 0d)
//                        {
//                            msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                        }
//                        else
//                        {
//                            msg = msg + "その戦闘中のみ";
//                        }

//                        msg = msg + "低下させる。。";
//                        break;
//                    }

//                case "先":
//                    {
//                        msg = "反撃時でも相手より先に攻撃する。";
//                        break;
//                    }

//                case "後":
//                    {
//                        msg = "反撃時ではない場合も相手より後に攻撃する。";
//                        break;
//                    }

//                case "吹":
//                    {
//                        if (alevel > 0d)
//                        {
//                            msg = "相手ユニットを" + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "マス吹き飛ばす。;" + "クリティカル発生時は吹き飛ばし距離＋１。";
//                        }
//                        else
//                        {
//                            msg = "クリティカル発生時に相手ユニットを１マス吹き飛ばす。";
//                        }

//                        break;
//                    }

//                case "Ｋ":
//                    {
//                        if (alevel > 0d)
//                        {
//                            string argtname50 = "サイズ";
//                            msg = "相手ユニットを" + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "マス吹き飛ばす。;" + "クリティカル発生時は吹き飛ばし距離＋１。" + Expression.Term(ref argtname50, ref u) + "制限あり。";
//                        }
//                        else
//                        {
//                            string argtname51 = "サイズ";
//                            msg = "クリティカル発生時に相手ユニットを１マス吹き飛ばす。" + Expression.Term(ref argtname51, ref u) + "制限あり。";
//                        }

//                        break;
//                    }

//                case "引":
//                    {
//                        msg = "クリティカル発生時に相手ユニットを隣接するマスまで引き寄せる。";
//                        break;
//                    }

//                case "転":
//                    {
//                        msg = "クリティカル発生時に相手ユニットを" + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "マス強制テレポートさせる。テレポート先はランダムに選ばれる。";
//                        break;
//                    }

//                case "連":
//                    {
//                        msg = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(alevel) + "回連続して攻撃を行う。;" + "攻撃によって与えるダメージは下記の式で計算される。;" + "  通常のダメージ量 × 命中回数 ／ 攻撃回数";
//                        break;
//                    }

//                case "再":
//                    {
//                        msg = Microsoft.VisualBasic.Compatibility.VB6.Support.Format((long)(100d * alevel) / 16L) + "%の確率で再攻撃。";
//                        break;
//                    }

//                case "精":
//                    {
//                        msg = "精神に働きかける攻撃。性格が「機械」のユニットには効かない。" + "シールドを無効化。";
//                        break;
//                    }

//                case "援":
//                    {
//                        msg = "自分以外のユニットに対してのみ使用可能。";
//                        break;
//                    }

//                case "難":
//                    {
//                        msg = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * alevel) + "%の確率で失敗する。";
//                        break;
//                    }

//                case "忍":
//                    {
//                        string argtname52 = "ＣＴ率";
//                        msg = "物音を立てずに攻撃し、" + "ステルス状態の際に" + Expression.Term(ref argtname52, ref u) + "に+10のボーナス。" + "一撃で相手を倒した場合は自分から攻撃をかけてもステルス状態が維持される。";
//                        break;
//                    }

//                case "盗":
//                    {
//                        string argtname53 = "資金";
//                        msg = "クリティカル発生時に敵から持ち物を盗む。;" + "盗めるものは通常は" + Expression.Term(ref argtname53, ref u) + "(普通に倒した時の半分の額)だが、" + "相手によってはアイテムを入手することもある。";
//                        break;
//                    }

//                case "Ｈ":
//                    {
//                        msg = "レーダー等でターゲットを追尾する攻撃。;";
//                        string argoname2 = "距離修正";
//                        if (Expression.IsOptionDefined(ref argoname2))
//                        {
//                            msg = msg + "長距離の敵を攻撃する際も命中率が低下しないが、";
//                        }

//                        msg = msg + "ＥＣＭによる影響を強く受ける。";
//                        msg = msg + "攻撃側が撹乱等の状態に陥っても命中率が低下しない。";
//                        break;
//                    }

//                case "追":
//                    {
//                        msg = "自己判断能力を持ち、ターゲットを追尾する攻撃。;";
//                        string argoname3 = "距離修正";
//                        if (Expression.IsOptionDefined(ref argoname3))
//                        {
//                            msg = msg + "長距離の敵を攻撃する際も命中率が低下しない。また、";
//                        }

//                        msg = msg + "攻撃側が撹乱等の状態に陥っても命中率が低下しない。";
//                        break;
//                    }

//                case "有":
//                    {
//                        msg = "有線による誘導でターゲットを追尾する攻撃。;";
//                        string argoname4 = "距離修正";
//                        if (Expression.IsOptionDefined(ref argoname4))
//                        {
//                            msg = msg + "長距離の敵を攻撃する際も命中率が低下しない。また、";
//                        }

//                        msg = msg + "ＥＣＭによる影響を受けない。";
//                        msg = msg + "しかし、スペシャルパワーや" + "アイテムの効果によって射程が増加しない。";
//                        break;
//                    }

//                case "誘":
//                    {
//                        msg = "電波妨害を受けない特殊な手段による誘導でターゲットを追尾する攻撃。;";
//                        string argoname5 = "距離修正";
//                        if (Expression.IsOptionDefined(ref argoname5))
//                        {
//                            msg = msg + "長距離の敵を攻撃する際も命中率が低下しない。また、";
//                        }

//                        msg = msg + "ＥＣＭによる影響を受けない。";
//                        break;
//                    }

//                case "爆":
//                    {
//                        msg = "爆発によりダメージを与える攻撃。;";
//                        string argoname6 = "距離修正";
//                        if (Expression.IsOptionDefined(ref argoname6))
//                        {
//                            msg = msg + "長距離の敵を攻撃する際もダメージが低下しない。";
//                        }

//                        break;
//                    }

//                case "空":
//                    {
//                        msg = "空中にいるターゲットを攻撃することを目的とした攻撃。";
//                        string argoname7 = "高度修正";
//                        if (Expression.IsOptionDefined(ref argoname7))
//                        {
//                            msg = msg + "地上から空中にいる敵を攻撃する際に命中率が低下しない。";
//                        }

//                        break;
//                    }

//                case "固":
//                    {
//                        string argtname54 = "気力";
//                        string argtname55 = "装甲";
//                        string argtname56 = "スペシャルパワー";
//                        string argtname57 = "地形適応";
//                        msg = "パイロットの" + Expression.Term(ref argtname54, ref u) + "や攻撃力、防御側の" + Expression.Term(ref argtname55, ref u) + "にかかわらず" + "武器の攻撃力と同じダメージを与える攻撃。" + "ただし、ユニットランクが上がっても攻撃力は増えない。" + Expression.Term(ref argtname56, ref u) + "や" + Expression.Term(ref argtname57, ref u) + "によるダメージ修正は有効。";
//                        break;
//                    }

//                case "衰":
//                    {
//                        string argtname58 = "ＨＰ";
//                        msg = "クリティカル発生時に敵の" + Expression.Term(ref argtname58, ref u) + "を現在値の ";
//                        switch ((short)alevel)
//                        {
//                            case 1:
//                                {
//                                    msg = msg + "3/4";
//                                    break;
//                                }

//                            case 2:
//                                {
//                                    msg = msg + "1/2";
//                                    break;
//                                }

//                            case 3:
//                                {
//                                    msg = msg + "1/4";
//                                    break;
//                                }
//                        }

//                        msg = msg + " まで減少させる。";
//                        break;
//                    }

//                case "滅":
//                    {
//                        string argtname59 = "ＥＮ";
//                        msg = "クリティカル発生時に敵の" + Expression.Term(ref argtname59, ref u) + "を現在値の ";
//                        switch ((short)alevel)
//                        {
//                            case 1:
//                                {
//                                    msg = msg + "3/4";
//                                    break;
//                                }

//                            case 2:
//                                {
//                                    msg = msg + "1/2";
//                                    break;
//                                }

//                            case 3:
//                                {
//                                    msg = msg + "1/4";
//                                    break;
//                                }
//                        }

//                        msg = msg + " まで減少させる。";
//                        break;
//                    }

//                case "踊":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            alevel = 3d;
//                        }

//                        msg = "クリティカル発生時に相手を";
//                        if (alevel > 0d)
//                        {
//                            msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                        }
//                        else
//                        {
//                            msg = msg + "その戦闘中のみ";
//                        }

//                        msg = msg + "踊らせる。";
//                        break;
//                    }

//                case "狂":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            alevel = 3d;
//                        }

//                        msg = "クリティカル発生時に相手を";
//                        if (alevel > 0d)
//                        {
//                            msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                        }
//                        else
//                        {
//                            msg = msg + "その戦闘中のみ";
//                        }

//                        msg = msg + "狂戦士状態にする。";
//                        break;
//                    }

//                case "ゾ":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            msg = "クリティカル発生時に相手をゾンビ状態にする。";
//                        }
//                        else
//                        {
//                            msg = "クリティカル発生時に相手を";
//                            if (alevel > 0d)
//                            {
//                                msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                            }
//                            else
//                            {
//                                msg = msg + "その戦闘中のみ";
//                            }

//                            msg = msg + "ゾンビ状態にする。";
//                        }

//                        break;
//                    }

//                case "害":
//                    {
//                        if (alevel == SRC.DEFAULT_LEVEL)
//                        {
//                            msg = "クリティカル発生時に相手の自己回復能力を破壊する。";
//                        }
//                        else
//                        {
//                            msg = "クリティカル発生時に相手を";
//                            if (alevel > 0d)
//                            {
//                                msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                            }
//                            else
//                            {
//                                msg = msg + "その戦闘中のみ";
//                            }

//                            msg = msg + "自己回復不能状態にする。";
//                        }

//                        break;
//                    }

//                case "習":
//                    {
//                        msg = "クリティカル発生時に相手の持つ技を習得出来る。;" + "ただし、習得可能な技を相手が持っていなければ無効。";
//                        break;
//                    }

//                case "写":
//                    {
//                        string argtname60 = "サイズ";
//                        msg = "クリティカル発生時に相手ユニットに変身する。;" + "ただし、既に変身している場合は使用できない。" + "また、相手と２段階以上" + Expression.Term(ref argtname60, ref u) + "が異なる場合は無効。";
//                        break;
//                    }

//                case "化":
//                    {
//                        msg = "クリティカル発生時に相手ユニットに変身する。;" + "ただし、既に変身している場合は使用できない。";
//                        break;
//                    }

//                case "痛":
//                    {
//                        msg = "クリティカル発生時に通常の ";
//                        string argoname8 = "ダメージ倍率低下";
//                        if (Expression.IsOptionDefined(ref argoname8))
//                        {
//                            msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(100d + 10d * (alevel + 2d));
//                        }
//                        else
//                        {
//                            msg = msg + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(100d + 25d * (alevel + 2d));
//                        }

//                        msg = msg + "% のダメージを与える。";
//                        break;
//                    }

//                case "地":
//                case "水":
//                case "火":
//                case "風":
//                case "冷":
//                case "雷":
//                case "光":
//                case "闇":
//                case "聖":
//                case "死":
//                case "木":
//                    {
//                        switch (atype ?? "")
//                        {
//                            case "水":
//                            case "火":
//                            case "風":
//                                {
//                                    msg = atype + "を使った";
//                                    break;
//                                }

//                            case "光":
//                            case "闇":
//                            case "死":
//                                {
//                                    msg = atype + "の力を使った";
//                                    break;
//                                }

//                            case "地":
//                                {
//                                    msg = "大地の力を借りた";
//                                    break;
//                                }

//                            case "冷":
//                                {
//                                    msg = "冷気による";
//                                    break;
//                                }

//                            case "雷":
//                                {
//                                    msg = "電撃による";
//                                    break;
//                                }

//                            case "聖":
//                                {
//                                    msg = "聖なる力を借りた";
//                                    break;
//                                }

//                            case "木":
//                                {
//                                    msg = "樹木の力を借りた";
//                                    break;
//                                }
//                        }

//                        msg = msg + whatsthis + "。";
//                        break;
//                    }

//                case "魔":
//                    {
//                        if (!is_ability)
//                        {
//                            msg = "魔力を帯びた攻撃。";
//                        }
//                        else
//                        {
//                            string argtname61 = "アビリティ";
//                            msg = "魔法による" + Expression.Term(ref argtname61, ref u) + "。";
//                        }

//                        break;
//                    }

//                case "時":
//                    {
//                        msg = "時の流れを操る" + whatsthis + "。";
//                        break;
//                    }

//                case "重":
//                    {
//                        msg = "重力を使った攻撃。";
//                        break;
//                    }

//                case "銃":
//                case "剣":
//                case "刀":
//                case "槍":
//                case "斧":
//                case "弓":
//                    {
//                        msg = atype + "を使った攻撃。";
//                        break;
//                    }

//                case "機":
//                    {
//                        msg = "機械(ロボット、アンドロイド)に対し特に有効な攻撃。";
//                        break;
//                    }

//                case "感":
//                    {
//                        msg = "エスパー(超能力者)に対し特に有効な攻撃。";
//                        break;
//                    }

//                case "竜":
//                    {
//                        msg = "竜族(ドラゴン)に対し特に有効な武器。";
//                        break;
//                    }

//                case "瀕":
//                    {
//                        msg = "瀕死時にのみ使用可能な" + whatsthis + "。";
//                        break;
//                    }

//                case "禁":
//                    {
//                        msg = "現在の状況下では使用することが出来ません。";
//                        break;
//                    }

//                case "対":
//                    {
//                        if (!is_ability)
//                        {
//                            whatsthis = "攻撃";
//                        }

//                        msg = "相手のメインパイロットのレベルが" + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "の倍数の場合にのみ有効な" + whatsthis + "。";
//                        break;
//                    }

//                case "ラ":
//                    {
//                        if (!is_ability)
//                        {
//                            whatsthis = "攻撃";
//                        }

//                        msg = "ラーニングが可能な" + whatsthis + "。";
//                        break;
//                    }

//                case "小":
//                    {
//                        msg = "最小射程が" + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "になる。";
//                        break;
//                    }

//                case "散":
//                    {
//                        msg = "相手から２マス以上離れていると命中率が上昇し、与えるダメージが減少する。";
//                        break;
//                    }

//                default:
//                    {
//                        // 弱、効、剋属性
//                        switch (Strings.Left(atype, 1) ?? "")
//                        {
//                            case "弱":
//                                {
//                                    if (alevel == SRC.DEFAULT_LEVEL)
//                                    {
//                                        alevel = 3d;
//                                    }

//                                    msg = "クリティカル発生時に相手に" + Strings.Mid(atype, 2) + "属性に対する弱点を";
//                                    if (alevel > 0d)
//                                    {
//                                        msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                                    }
//                                    else
//                                    {
//                                        msg = msg + "その戦闘中のみ";
//                                    }

//                                    msg = msg + "付加する。";
//                                    break;
//                                }

//                            case "効":
//                                {
//                                    if (alevel == SRC.DEFAULT_LEVEL)
//                                    {
//                                        alevel = 3d;
//                                    }

//                                    msg = "クリティカル発生時に相手に" + Strings.Mid(atype, 2) + "属性に対する有効を";
//                                    if (alevel > 0d)
//                                    {
//                                        msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                                    }
//                                    else
//                                    {
//                                        msg = msg + "その戦闘中のみ";
//                                    }

//                                    msg = msg + "付加する。";
//                                    break;
//                                }

//                            case "剋":
//                                {
//                                    if (alevel == SRC.DEFAULT_LEVEL)
//                                    {
//                                        alevel = 3d;
//                                    }

//                                    msg = "クリティカル発生時に相手の";
//                                    switch (Strings.Mid(atype, 2) ?? "")
//                                    {
//                                        case "オ":
//                                            {
//                                                msg = msg + "オーラ";
//                                                break;
//                                            }

//                                        case "超":
//                                            {
//                                                msg = msg + "超能力";
//                                                break;
//                                            }

//                                        case "シ":
//                                            {
//                                                msg = msg + "同調率";
//                                                break;
//                                            }

//                                        case "サ":
//                                            {
//                                                msg = msg + "超感覚、知覚強化";
//                                                break;
//                                            }

//                                        case "霊":
//                                            {
//                                                msg = msg + "霊力";
//                                                break;
//                                            }

//                                        case "術":
//                                            {
//                                                msg = msg + "術";
//                                                break;
//                                            }

//                                        case "技":
//                                            {
//                                                msg = msg + "技";
//                                                break;
//                                            }

//                                        default:
//                                            {
//                                                msg = msg + Strings.Mid(atype, 2) + "属性の武器、アビリティ";
//                                                break;
//                                            }
//                                    }

//                                    msg = msg + "を";
//                                    if (alevel > 0d)
//                                    {
//                                        msg = msg + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format((short)alevel), VbStrConv.Wide) + "ターン";
//                                    }
//                                    else
//                                    {
//                                        msg = msg + "その戦闘中のみ";
//                                    }

//                                    msg = msg + "使用不能にする。";
//                                    break;
//                                }
//                        }

//                        break;
//                    }
//            }

//            object argIndex14 = atype;
//            fdata = u.FeatureData(ref argIndex14);
//            if (GeneralLib.ListIndex(ref fdata, 1) == "解説")
//            {
//                // 解説を定義している場合
//                msg = GeneralLib.ListTail(ref fdata, 3);
//                if (Strings.Left(msg, 1) == "\"")
//                {
//                    msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
//                }
//            }

//            // 等身大基準の際は「パイロット」という語を使わないようにする
//            string argoname9 = "等身大基準";
//            if (Expression.IsOptionDefined(ref argoname9))
//            {
//                string args2 = "メインパイロット";
//                string args3 = "ユニット";
//                GeneralLib.ReplaceString(ref msg, ref args2, ref args3);
//                string args21 = "パイロット";
//                string args31 = "ユニット";
//                GeneralLib.ReplaceString(ref msg, ref args21, ref args31);
//                string args22 = "相手のユニット";
//                string args32 = "相手ユニット";
//                GeneralLib.ReplaceString(ref msg, ref args22, ref args32);
//            }

//            AttributeHelpMessageRet = msg;
//            return AttributeHelpMessageRet;
//        }
//    }
//}