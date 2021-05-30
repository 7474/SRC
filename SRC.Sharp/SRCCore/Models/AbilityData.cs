// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Lib;
using SRCCore.VB;
using System.Collections.Generic;

namespace SRCCore.Models
{
    // アビリティデータのクラス
    public class AbilityData
    {
        // 名称
        public string Name;
        // 使用可能回数
        public int Stock;
        // ＥＮ消費量
        public int ENConsumption;
        // 必要気力
        public int NecessaryMorale;
        // 最小射程
        public int MinRange;
        // 最大射程
        public int MaxRange;
        // 属性
        public string Class = "";
        // 必要技能
        public string NecessarySkill = "";
        // 必要条件
        public string NecessaryCondition = "";

        // 効果
        private SrcCollection<AbilityEffect> colEffects = new SrcCollection<AbilityEffect>();
        public IList<AbilityEffect> Effects => colEffects.List;

        // アビリティ愛称
        public string Nickname()
        {
            string NicknameRet = default;
            NicknameRet = Name;
            // TODO Impl
            //Expression.ReplaceSubExpression(NicknameRet);
            //if (Strings.InStr(NicknameRet, "(") > 0)
            //{
            //    NicknameRet = Strings.Left(NicknameRet, Strings.InStr(NicknameRet, "(") - 1);
            //}

            return NicknameRet;
        }

        // アビリティに効果を追加
        public void SetEffect(string elist)
        {
            elist = (elist ?? "").Trim();
            foreach (var eelm in GeneralLib.ToList(elist))
            {
                var dat = new AbilityEffect();
                var buf = eelm;
                var j = Strings.InStr(buf, "Lv");
                var k = Strings.InStr(buf, "=");
                string etype;
                string elevel;
                string edata;
                if (j > 0 && (k == 0 || j < k))
                {
                    // レベル指定のある効果(データ指定があるものを含む)
                    dat.Name = Strings.Left(buf, j - 1);
                    if (k > 0)
                    {
                        // データ指定があるもの
                        dat.Level = Conversions.ToDouble(Strings.Mid(buf, j + 2, k - (j + 2)));
                        buf = Strings.Mid(buf, k + 1);
                        if (Strings.Left(buf, 1) == "\"")
                        {
                            buf = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                        }

                        j = Strings.InStr(buf, "Lv");
                        k = Strings.InStr(buf, "=");
                        if (j > 0 && (k == 0 || j < k))
                        {
                            // データ指定内にレベル指定がある
                            etype = Strings.Left(buf, j - 1);
                            if (k > 0)
                            {
                                elevel = Strings.Mid(buf, j + 2, k - (j + 2));
                                edata = Strings.Mid(buf, k + 1);
                            }
                            else
                            {
                                elevel = Strings.Mid(buf, j + 2);
                                edata = "";
                            }
                        }
                        else if (k > 0)
                        {
                            // データ指定内にデータ指定がある
                            etype = Strings.Left(buf, k - 1);
                            elevel = "";
                            edata = Strings.Mid(buf, k + 1);
                        }
                        else
                        {
                            // 単純なデータ指定
                            etype = buf;
                            elevel = "";
                            edata = "";
                        }

                        if (dat.Name == "付加" && string.IsNullOrEmpty(elevel))
                        {
                            elevel = SrcFormatter.Format(Constants.DEFAULT_LEVEL);
                        }

                        dat.Data = Strings.Trim(etype + " " + elevel + " " + edata);
                    }
                    else
                    {
                        // データ指定がないもの
                        dat.Level = Conversions.ToDouble(Strings.Mid(buf, j + 2));
                    }
                }
                else if (k > 0)
                {
                    // データ指定を含む効果
                    dat.Name = Strings.Left(buf, k - 1);
                    buf = Strings.Mid(buf, k + 1);
                    if (Strings.Asc(buf) == 34) // "
                    {
                        buf = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                    }

                    j = Strings.InStr(buf, "Lv");
                    k = Strings.InStr(buf, "=");
                    if (dat.Name == "解説")
                    {
                        // 解説の指定
                        etype = buf;
                        elevel = "";
                        edata = "";
                    }
                    else if (j > 0)
                    {
                        // データ指定内にレベル指定がある
                        etype = Strings.Left(buf, j - 1);
                        if (k > 0)
                        {
                            elevel = Strings.Mid(buf, j + 2, k - (j + 2));
                            edata = Strings.Mid(buf, k + 1);
                        }
                        else
                        {
                            elevel = Strings.Mid(buf, j + 2);
                            edata = "";
                        }
                    }
                    else if (k > 0)
                    {
                        // データ指定内にデータ指定がある
                        etype = Strings.Left(buf, k - 1);
                        elevel = "";
                        edata = Strings.Mid(buf, k + 1);
                    }
                    else
                    {
                        // 単純なデータ指定
                        etype = buf;
                        elevel = "";
                        edata = "";
                    }

                    if (dat.Name == "付加" && string.IsNullOrEmpty(elevel))
                    {
                        elevel = SrcFormatter.Format(Constants.DEFAULT_LEVEL);
                    }

                    dat.Data = Strings.Trim(etype + " " + elevel + " " + edata);
                }
                else
                {
                    // 効果名のみ
                    dat.Name = buf;
                }

                j = 1;
                foreach (AbilityEffect dat2 in colEffects)
                {
                    if ((dat.Name ?? "") == (dat2.Name ?? ""))
                    {
                        j = (j + 1);
                    }
                }

                if (j == 1)
                {
                    colEffects.Add(dat, dat.Name);
                }
                else
                {
                    colEffects.Add(dat, dat.Name + SrcFormatter.Format(j));
                }
            }
        }

        //// 効果の総数
        //public int CountEffect()
        //{
        //    int CountEffectRet = default;
        //    CountEffectRet = colEffects.Count;
        //    return CountEffectRet;
        //}

        //// 効果の種類
        //public string EffectType(object Index)
        //{
        //    string EffectTypeRet = default;
        //    // UPGRADE_WARNING: オブジェクト colEffects.Item().Name の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //    EffectTypeRet = Conversions.ToString(colEffects[Index].Name);
        //    return EffectTypeRet;
        //}

        //// 効果のレベル
        //public double EffectLevel(object Index)
        //{
        //    double EffectLevelRet = default;
        //    // UPGRADE_WARNING: オブジェクト colEffects.Item().Level の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //    EffectLevelRet = Conversions.ToDouble(colEffects[Index].Level);
        //    return EffectLevelRet;
        //}

        //// 効果のデータ
        //public string EffectData(object Index)
        //{
        //    string EffectDataRet = default;
        //    // UPGRADE_WARNING: オブジェクト colEffects.Item().Data の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //    EffectDataRet = Conversions.ToString(colEffects[Index].Data);
        //    return EffectDataRet;
        //}

        //// 効果内容の解説
        //public string EffectName(object Index)
        //{
        //    string EffectNameRet = default;
        //    AbilityEffect ae;
        //    double elevel, elevel2;
        //    string uname, wclass = default;
        //    double flevel;
        //    double heal_lv = default, supply_lv = default;
        //    int i;
        //    string buf;
        //    string cname, aname;
        //    ae = (AbilityEffect)colEffects[Index];
        //    // 効果レベルが回復・増加量を意味するアビリティ用
        //    elevel = ae.Level;
        //    // 効果レベルがターン数を意味するアビリティ及び召喚アビリティ用
        //    elevel2 = elevel;
        //    if (Commands.SelectedUnit.CountPilot() > 0)
        //    {
        //        {
        //            var withBlock = Commands.SelectedUnit.MainPilot();
        //            // 得意技
        //            if (withBlock.IsSkillAvailable("得意技"))
        //            {
        //                buf = withBlock.SkillData("得意技");
        //                var loopTo = Strings.Len(buf);
        //                for (i = 1; i <= loopTo; i++)
        //                {
        //                    if (GeneralLib.InStrNotNest(Class, GeneralLib.GetClassBundle(buf, i)) > 0)
        //                    {
        //                        elevel = 1.2d * elevel;
        //                        elevel2 = 1.4d * elevel2;
        //                        break;
        //                    }
        //                }
        //            }

        //            // 不得手
        //            if (withBlock.IsSkillAvailable("不得手"))
        //            {
        //                buf = withBlock.SkillData("不得手");
        //                var loopTo1 = Strings.Len(buf);
        //                for (i = 1; i <= loopTo1; i++)
        //                {
        //                    if (GeneralLib.InStrNotNest(Class, GeneralLib.GetClassBundle(buf, i)) > 0)
        //                    {
        //                        elevel = 0.8d * elevel;
        //                        elevel2 = 0.6d * elevel2;
        //                        break;
        //                    }
        //                }
        //            }

        //            // 術アビリティの場合は魔力によって効果レベルが修正を受ける
        //            if (GeneralLib.InStrNotNest(Class, "術") > 0)
        //            {
        //                elevel = elevel * withBlock.Shooting / 100d;
        //            }
        //            else
        //            {
        //                var loopTo2 = GeneralLib.LLength(NecessarySkill);
        //                for (i = 1; i <= loopTo2; i++)
        //                {
        //                    if (withBlock.SkillType(GeneralLib.LIndex(NecessarySkill, i)) == "術")
        //                    {
        //                        elevel = elevel * withBlock.Shooting / 100d;
        //                        break;
        //                    }
        //                }
        //            }

        //            // 修理＆補給技能
        //            heal_lv = withBlock.SkillLevel("修理", ref_mode: "");
        //            supply_lv = withBlock.SkillLevel("補給", ref_mode: "");
        //        }
        //    }

        //    // アビリティの効果は最低でも１ターン持続
        //    if (elevel2 != 0d)
        //    {
        //        elevel2 = GeneralLib.MaxLng(elevel2, 1);
        //    }

        //    switch (ae.Name ?? "")
        //    {
        //        case "回復":
        //            {
        //                EffectNameRet = Expression.Term("ＨＰ", u: null) + "を";
        //                if (elevel > 0d)
        //                {
        //                    EffectNameRet = EffectNameRet + SrcFormatter.Format(((long)(500d * elevel * (10d + heal_lv)) / 10L)) + "回復";
        //                }
        //                else if (elevel < 0d)
        //                {
        //                    EffectNameRet = EffectNameRet + SrcFormatter.Format((-500 * elevel)) + "減少";
        //                }

        //                break;
        //            }

        //        case "補給":
        //            {
        //                EffectNameRet = Expression.Term("ＥＮ", u: null) + "を";
        //                if (elevel > 0d)
        //                {
        //                    EffectNameRet = EffectNameRet + SrcFormatter.Format((50d * elevel * (10d + supply_lv)) / 10) + "回復";
        //                }
        //                else if (elevel < 0d)
        //                {
        //                    EffectNameRet = EffectNameRet + SrcFormatter.Format((-50 * elevel)) + "減少";
        //                }

        //                break;
        //            }

        //        case "霊力回復":
        //        case "プラーナ回復":
        //            {
        //                if (Commands.SelectedUnit.CountPilot() > 0)
        //                {
        //                    EffectNameRet = Commands.SelectedUnit.MainPilot().SkillName0("霊力");
        //                }
        //                else
        //                {
        //                    EffectNameRet = "霊力";
        //                }

        //                if (elevel > 0d)
        //                {
        //                    EffectNameRet = EffectNameRet + "を" + SrcFormatter.Format((10d * elevel)) + "回復";
        //                }
        //                else if (elevel < 0d)
        //                {
        //                    EffectNameRet = EffectNameRet + "を" + SrcFormatter.Format((-10 * elevel)) + "減少";
        //                }

        //                break;
        //            }

        //        case "ＳＰ回復":
        //            {
        //                EffectNameRet = Expression.Term("ＳＰ", u: null) + "を";
        //                if (elevel > 0d)
        //                {
        //                    EffectNameRet = EffectNameRet + SrcFormatter.Format((10d * elevel)) + "回復";
        //                }
        //                else if (elevel < 0d)
        //                {
        //                    EffectNameRet = EffectNameRet + SrcFormatter.Format((-10 * elevel)) + "減少";
        //                }

        //                break;
        //            }

        //        case "気力増加":
        //            {
        //                EffectNameRet = Expression.Term("気力", u: null) + "を";
        //                if (elevel > 0d)
        //                {
        //                    EffectNameRet = EffectNameRet + SrcFormatter.Format((10d * elevel)) + "増加";
        //                }
        //                else if (elevel < 0d)
        //                {
        //                    EffectNameRet = EffectNameRet + SrcFormatter.Format((-10 * elevel)) + "減少";
        //                }

        //                break;
        //            }

        //        case "装填":
        //            {
        //                if (Strings.Len(ae.Data) == 0)
        //                {
        //                    EffectNameRet = "武器の弾数を回復";
        //                }
        //                else
        //                {
        //                    var loopTo3 = Commands.SelectedUnit.CountWeapon();
        //                    for (i = 1; i <= loopTo3; i++)
        //                    {
        //                        if ((Commands.SelectedUnit.WeaponNickname(i) ?? "") == (ae.Data ?? ""))
        //                        {
        //                            EffectNameRet = ae.Data + "の弾数を回復";
        //                            return EffectNameRet;
        //                        }
        //                    }

        //                    EffectNameRet = ae.Data + "属性を持つ武器の弾数を回復";
        //                }

        //                break;
        //            }

        //        case "治癒":
        //            {
        //                var loopTo4 = GeneralLib.LLength(ae.Data);
        //                for (i = 1; i <= loopTo4; i++)
        //                {
        //                    cname = GeneralLib.LIndex(ae.Data, i);
        //                    switch (cname ?? "")
        //                    {
        //                        case "装甲劣化":
        //                            {
        //                                cname = Expression.Term("装甲", u: null) + "劣化";
        //                                break;
        //                            }

        //                        case "運動性ＵＰ":
        //                            {
        //                                cname = Expression.Term("運動性", u: null) + "ＵＰ";
        //                                break;
        //                            }

        //                        case "運動性ＤＯＷＮ":
        //                            {
        //                                cname = Expression.Term("運動性", u: null) + "ＤＯＷＮ";
        //                                break;
        //                            }

        //                        case "移動力ＵＰ":
        //                            {
        //                                cname = Expression.Term("移動力", u: null) + "ＵＰ";
        //                                break;
        //                            }

        //                        case "移動力ＤＯＷＮ":
        //                            {
        //                                cname = Expression.Term("移動力", u: null) + "ＤＯＷＮ";
        //                                break;
        //                            }
        //                    }

        //                    EffectNameRet = EffectNameRet + " " + cname;
        //                }

        //                EffectNameRet = Strings.Trim(EffectNameRet);
        //                if (Strings.Len(EffectNameRet) > 0)
        //                {
        //                    EffectNameRet = EffectNameRet + "を回復";
        //                }
        //                else
        //                {
        //                    EffectNameRet = "状態回復";
        //                }

        //                break;
        //            }

        //        case "状態":
        //            {
        //                cname = GeneralLib.LIndex(ae.Data, 1);
        //                switch (cname ?? "")
        //                {
        //                    case "装甲劣化":
        //                        {
        //                            cname = Expression.Term("装甲", u: null) + "劣化";
        //                            break;
        //                        }

        //                    case "運動性ＵＰ":
        //                        {
        //                            cname = Expression.Term("運動性", u: null) + "ＵＰ";
        //                            break;
        //                        }

        //                    case "運動性ＤＯＷＮ":
        //                        {
        //                            cname = Expression.Term("運動性", u: null) + "ＤＯＷＮ";
        //                            break;
        //                        }

        //                    case "移動力ＵＰ":
        //                        {
        //                            cname = Expression.Term("移動力", u: null) + "ＵＰ";
        //                            break;
        //                        }

        //                    case "移動力ＤＯＷＮ":
        //                        {
        //                            cname = Expression.Term("移動力", u: null) + "ＤＯＷＮ";
        //                            break;
        //                        }
        //                }

        //                if (0d < elevel2 && elevel2 <= 10d)
        //                {
        //                    EffectNameRet = cname + "(" + SrcFormatter.Format(elevel2) + "ターン)";
        //                }
        //                else
        //                {
        //                    EffectNameRet = cname;
        //                }

        //                break;
        //            }

        //        case "付加":
        //            {
        //                switch (GeneralLib.LIndex(ae.Data, 1) ?? "")
        //                {
        //                    case "耐性":
        //                        {
        //                            aname = Help.AttributeName(Commands.SelectedUnit, GeneralLib.LIndex(ae.Data, 3));
        //                            if (string.IsNullOrEmpty(aname))
        //                            {
        //                                aname = GeneralLib.LIndex(ae.Data, 3) + "攻撃";
        //                            }

        //                            EffectNameRet = aname + "のダメージを半減";
        //                            break;
        //                        }

        //                    case "無効化":
        //                        {
        //                            aname = Help.AttributeName(Commands.SelectedUnit, GeneralLib.LIndex(ae.Data, 3));
        //                            if (string.IsNullOrEmpty(aname))
        //                            {
        //                                aname = GeneralLib.LIndex(ae.Data, 3) + "攻撃";
        //                            }

        //                            EffectNameRet = aname + "を無効化";
        //                            break;
        //                        }

        //                    case "特殊効果無効化":
        //                        {
        //                            aname = Help.AttributeName(Commands.SelectedUnit, GeneralLib.LIndex(ae.Data, 3));
        //                            if (string.IsNullOrEmpty(aname))
        //                            {
        //                                aname = GeneralLib.LIndex(ae.Data, 3) + "攻撃";
        //                            }

        //                            EffectNameRet = aname + "の特殊効果を無効化";
        //                            break;
        //                        }

        //                    case "吸収":
        //                        {
        //                            aname = Help.AttributeName(Commands.SelectedUnit, GeneralLib.LIndex(ae.Data, 3));
        //                            if (string.IsNullOrEmpty(aname))
        //                            {
        //                                aname = GeneralLib.LIndex(ae.Data, 3) + "攻撃";
        //                            }

        //                            EffectNameRet = aname + "を吸収";
        //                            break;
        //                        }

        //                    case "追加パイロット":
        //                        {
        //                            EffectNameRet = "パイロット変化";
        //                            break;
        //                        }

        //                    case "追加サポート":
        //                        {
        //                            EffectNameRet = "サポート追加";
        //                            break;
        //                        }

        //                    case "性格変更":
        //                        {
        //                            EffectNameRet = "パイロットの性格を" + GeneralLib.LIndex(ae.Data, 3) + "に変更";
        //                            break;
        //                        }

        //                    case "ＢＧＭ":
        //                        {
        //                            EffectNameRet = "ＢＧＭ変更";
        //                            break;
        //                        }

        //                    case "攻撃属性":
        //                        {
        //                            var loopTo5 = GeneralLib.LLength(ae.Data);
        //                            for (i = 4; i <= loopTo5; i++)
        //                            {
        //                                if (Strings.InStr(GeneralLib.LIndex(ae.Data, i), "!") == 0)
        //                                {
        //                                    wclass = wclass + GeneralLib.LIndex(ae.Data, i);
        //                                }
        //                            }

        //                            EffectNameRet = WeaponType(wclass) + "の属性に" + GeneralLib.LIndex(ae.Data, 3) + "を追加";
        //                            break;
        //                        }

        //                    case "武器強化":
        //                        {
        //                            var loopTo6 = GeneralLib.LLength(ae.Data);
        //                            for (i = 3; i <= loopTo6; i++)
        //                            {
        //                                if (Strings.InStr(GeneralLib.LIndex(ae.Data, i), "!") == 0)
        //                                {
        //                                    wclass = wclass + GeneralLib.LIndex(ae.Data, i);
        //                                }
        //                            }

        //                            flevel = GeneralLib.StrToDbl(GeneralLib.LIndex(ae.Data, 2));
        //                            if (flevel >= 0d)
        //                            {
        //                                EffectNameRet = WeaponType(wclass) + "の攻撃力を+" + SrcFormatter.Format(100d * flevel);
        //                            }
        //                            else
        //                            {
        //                                EffectNameRet = WeaponType(wclass) + "の攻撃力を" + SrcFormatter.Format(100d * flevel);
        //                            }

        //                            break;
        //                        }

        //                    case "命中率強化":
        //                        {
        //                            var loopTo7 = GeneralLib.LLength(ae.Data);
        //                            for (i = 3; i <= loopTo7; i++)
        //                            {
        //                                if (Strings.InStr(GeneralLib.LIndex(ae.Data, i), "!") == 0)
        //                                {
        //                                    wclass = wclass + GeneralLib.LIndex(ae.Data, i);
        //                                }
        //                            }

        //                            flevel = GeneralLib.StrToDbl(GeneralLib.LIndex(ae.Data, 2));
        //                            if (flevel >= 0d)
        //                            {
        //                                EffectNameRet = WeaponType(wclass) + "の命中率を+" + SrcFormatter.Format(5d * flevel);
        //                            }
        //                            else
        //                            {
        //                                EffectNameRet = WeaponType(wclass) + "の命中率を" + SrcFormatter.Format(5d * flevel);
        //                            }

        //                            break;
        //                        }

        //                    case "ＣＴ率強化":
        //                    case "特殊効果発動率強化":
        //                        {
        //                            var loopTo8 = GeneralLib.LLength(ae.Data);
        //                            for (i = 3; i <= loopTo8; i++)
        //                            {
        //                                if (Strings.InStr(GeneralLib.LIndex(ae.Data, i), "!") == 0)
        //                                {
        //                                    wclass = wclass + GeneralLib.LIndex(ae.Data, i);
        //                                }
        //                            }

        //                            flevel = GeneralLib.StrToDbl(GeneralLib.LIndex(ae.Data, 2));
        //                            if (flevel >= 0d)
        //                            {
        //                                EffectNameRet = WeaponType(wclass) + "のＣＴ率を+" + SrcFormatter.Format(5d * flevel);
        //                            }
        //                            else
        //                            {
        //                                EffectNameRet = WeaponType(wclass) + "のＣＴ率を" + SrcFormatter.Format(5d * flevel);
        //                            }

        //                            break;
        //                        }

        //                    case "射程延長":
        //                        {
        //                            var loopTo9 = GeneralLib.LLength(ae.Data);
        //                            for (i = 3; i <= loopTo9; i++)
        //                            {
        //                                if (Strings.InStr(GeneralLib.LIndex(ae.Data, i), "!") == 0)
        //                                {
        //                                    wclass = wclass + GeneralLib.LIndex(ae.Data, i);
        //                                }
        //                            }

        //                            flevel = GeneralLib.StrToLng(GeneralLib.LIndex(ae.Data, 2));
        //                            if (flevel >= 0d)
        //                            {
        //                                EffectNameRet = WeaponType(wclass) + "の射程を+" + SrcFormatter.Format(flevel);
        //                            }
        //                            else
        //                            {
        //                                EffectNameRet = WeaponType(wclass) + "の射程を" + SrcFormatter.Format(flevel);
        //                            }

        //                            break;
        //                        }

        //                    case "サイズ変更":
        //                        {
        //                            EffectNameRet = "サイズが" + GeneralLib.LIndex(ae.Data, 3) + "に変化";
        //                            break;
        //                        }

        //                    case "地形適応変更":
        //                        {
        //                            int localStrToLng() { string argexpr = GeneralLib.LIndex(ae.Data, 4); var ret = GeneralLib.StrToLng(argexpr); return ret; }

        //                            int localStrToLng1() { string argexpr = GeneralLib.LIndex(ae.Data, 5); var ret = GeneralLib.StrToLng(argexpr); return ret; }

        //                            int localStrToLng2() { string argexpr = GeneralLib.LIndex(ae.Data, 6); var ret = GeneralLib.StrToLng(argexpr); return ret; }

        //                            if (GeneralLib.StrToLng(GeneralLib.LIndex(ae.Data, 3)) > 0)
        //                            {
        //                                EffectNameRet = "空への適応を強化";
        //                            }
        //                            else if (localStrToLng() > 0)
        //                            {
        //                                EffectNameRet = "陸への適応を強化";
        //                            }
        //                            else if (localStrToLng1() > 0)
        //                            {
        //                                EffectNameRet = "水中への適応を強化";
        //                            }
        //                            else if (localStrToLng2() > 0)
        //                            {
        //                                EffectNameRet = "宇宙への適応を強化";
        //                            }

        //                            break;
        //                        }

        //                    case "地形適応固定変更":
        //                        {
        //                            int localStrToLng3() { string argexpr = GeneralLib.LIndex(ae.Data, 3); var ret = GeneralLib.StrToLng(argexpr); return ret; }

        //                            int localStrToLng4() { string argexpr = GeneralLib.LIndex(ae.Data, 3); var ret = GeneralLib.StrToLng(argexpr); return ret; }

        //                            int localStrToLng5() { string argexpr = GeneralLib.LIndex(ae.Data, 4); var ret = GeneralLib.StrToLng(argexpr); return ret; }

        //                            int localStrToLng6() { string argexpr = GeneralLib.LIndex(ae.Data, 4); var ret = GeneralLib.StrToLng(argexpr); return ret; }

        //                            int localStrToLng7() { string argexpr = GeneralLib.LIndex(ae.Data, 5); var ret = GeneralLib.StrToLng(argexpr); return ret; }

        //                            int localStrToLng8() { string argexpr = GeneralLib.LIndex(ae.Data, 5); var ret = GeneralLib.StrToLng(argexpr); return ret; }

        //                            int localStrToLng9() { string argexpr = GeneralLib.LIndex(ae.Data, 6); var ret = GeneralLib.StrToLng(argexpr); return ret; }

        //                            int localStrToLng10() { string argexpr = GeneralLib.LIndex(ae.Data, 6); var ret = GeneralLib.StrToLng(argexpr); return ret; }

        //                            if (localStrToLng3() <= 5 && localStrToLng4() >= 0)
        //                            {
        //                                if (GeneralLib.LIndex(ae.Data, 6) == "強制")
        //                                {
        //                                    EffectNameRet = "空への適応を強制的に変化";
        //                                }
        //                                else
        //                                {
        //                                    EffectNameRet = "空への適応を変化";
        //                                }
        //                            }
        //                            else if (localStrToLng5() <= 5 && localStrToLng6() >= 0)
        //                            {
        //                                if (GeneralLib.LIndex(ae.Data, 6) == "強制")
        //                                {
        //                                    EffectNameRet = "陸への適応を強制的に変化";
        //                                }
        //                                else
        //                                {
        //                                    EffectNameRet = "陸への適応を変化";
        //                                }
        //                            }
        //                            else if (localStrToLng7() <= 5 && localStrToLng8() >= 0)
        //                            {
        //                                if (GeneralLib.LIndex(ae.Data, 6) == "強制")
        //                                {
        //                                    EffectNameRet = "水中への適応を強制的に変化";
        //                                }
        //                                else
        //                                {
        //                                    EffectNameRet = "水中への適応を変化";
        //                                }
        //                            }
        //                            else if (localStrToLng9() <= 5 && localStrToLng10() >= 0)
        //                            {
        //                                if (GeneralLib.LIndex(ae.Data, 6) == "強制")
        //                                {
        //                                    EffectNameRet = "宇宙への適応を強制的に変化";
        //                                }
        //                                else
        //                                {
        //                                    EffectNameRet = "宇宙への適応を変化";
        //                                }
        //                            }

        //                            break;
        //                        }

        //                    case "Ｖ－ＵＰ":
        //                        {
        //                            switch (GeneralLib.LIndex(ae.Data, 3) ?? "")
        //                            {
        //                                case "武器":
        //                                    {
        //                                        EffectNameRet = "武器攻撃力を強化";
        //                                        break;
        //                                    }

        //                                case "ユニット":
        //                                    {
        //                                        EffectNameRet = "各パラメータを強化";
        //                                        break;
        //                                    }

        //                                default:
        //                                    {
        //                                        EffectNameRet = "ユニットを強化";
        //                                        break;
        //                                    }
        //                            }

        //                            break;
        //                        }

        //                    case "格闘武器":
        //                    case "迎撃武器":
        //                    case "制限時間":
        //                        {
        //                            EffectNameRet = EffectNameRet + "付加";
        //                            break;
        //                        }

        //                    case "パイロット愛称":
        //                    case "パイロット画像":
        //                    case "愛称変更":
        //                    case "ユニット画像":
        //                        {
        //                            EffectNameRet = "";
        //                            break;
        //                        }

        //                    default:
        //                        {
        //                            EffectNameRet = GeneralLib.ListIndex(ae.Data, 3);
        //                            if (Strings.Left(EffectNameRet, 1) == "\"")
        //                            {
        //                                EffectNameRet = GeneralLib.ListIndex(Strings.Mid(EffectNameRet, 2, Strings.Len(EffectNameRet) - 2), 1);
        //                            }

        //                            if (string.IsNullOrEmpty(EffectNameRet) || EffectNameRet == "非表示")
        //                            {
        //                                if ((GeneralLib.LIndex(ae.Data, 2) ?? "") != (SrcFormatter.Format(Constants.DEFAULT_LEVEL) ?? "") && GeneralLib.LLength(ae.Data) <= 3)
        //                                {
        //                                    EffectNameRet = GeneralLib.LIndex(ae.Data, 1) + "Lv" + GeneralLib.LIndex(ae.Data, 2) + "付加";
        //                                }
        //                                else
        //                                {
        //                                    EffectNameRet = GeneralLib.LIndex(ae.Data, 1) + "付加";
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if ((GeneralLib.LIndex(ae.Data, 2) ?? "") != (SrcFormatter.Format(Constants.DEFAULT_LEVEL) ?? "") && GeneralLib.LLength(ae.Data) <= 3)
        //                                {
        //                                    EffectNameRet = EffectNameRet + "Lv" + GeneralLib.LIndex(ae.Data, 2);
        //                                }

        //                                EffectNameRet = EffectNameRet + "付加";
        //                            }

        //                            break;
        //                        }
        //                }

        //                if (!string.IsNullOrEmpty(EffectNameRet))
        //                {
        //                    if (0d < elevel2 && elevel2 <= 10d)
        //                    {
        //                        EffectNameRet = EffectNameRet + "(" + SrcFormatter.Format(elevel2) + "ターン)";
        //                    }
        //                }

        //                break;
        //            }

        //        case "強化":
        //            {
        //                EffectNameRet = GeneralLib.ListIndex(ae.Data, 3);
        //                if (string.IsNullOrEmpty(EffectNameRet) || EffectNameRet == "非表示")
        //                {
        //                    if (GeneralLib.StrToLng(GeneralLib.LIndex(ae.Data, 2)) > 0)
        //                    {
        //                        EffectNameRet = GeneralLib.LIndex(ae.Data, 1) + "Lv" + GeneralLib.LIndex(ae.Data, 2);
        //                    }
        //                    else
        //                    {
        //                        EffectNameRet = GeneralLib.LIndex(ae.Data, 1);
        //                    }
        //                }

        //                if (0d < elevel2 && elevel2 <= 10d)
        //                {
        //                    EffectNameRet = EffectNameRet + "強化(" + SrcFormatter.Format(elevel2) + "ターン)";
        //                }

        //                break;
        //            }

        //        case "召喚":
        //            {
        //                bool localIsDefined() { object argIndex1 = ae.Data; var ret = SRC.UDList.IsDefined(argIndex1); return ret; }

        //                if (!localIsDefined())
        //                {
        //                    GUI.ErrorMessage("召喚ユニット「" + ae.Data + "」が定義されていません");
        //                    return EffectNameRet;
        //                }

        //                if (elevel2 > 1d)
        //                {
        //                    UnitData localItem() { object argIndex1 = ae.Data; var ret = SRC.UDList.Item(argIndex1); return ret; }

        //                    EffectNameRet = localItem().Nickname + "を" + Strings.StrConv(SrcFormatter.Format(elevel2), VbStrConv.Wide) + "体召喚";
        //                }
        //                else
        //                {
        //                    UnitData localItem1() { object argIndex1 = ae.Data; var ret = SRC.UDList.Item(argIndex1); return ret; }

        //                    EffectNameRet = localItem1().Nickname + "を召喚";
        //                }

        //                break;
        //            }

        //        case "変身":
        //            {
        //                uname = GeneralLib.LIndex(ae.Data, 1);
        //                bool localIsDefined1() { object argIndex1 = uname; var ret = SRC.UDList.IsDefined(argIndex1); return ret; }

        //                if (!localIsDefined1())
        //                {
        //                    GUI.ErrorMessage("変身先のデータ「" + uname + "」が定義されていません");
        //                    return EffectNameRet;
        //                }

        //                if (0d < elevel2 && elevel2 <= 10d)
        //                {
        //                    UnitData localItem2() { object argIndex1 = uname; var ret = SRC.UDList.Item(argIndex1); return ret; }

        //                    EffectNameRet = localItem2().Nickname + "に変身" + "(" + SrcFormatter.Format(elevel2) + "ターン)";
        //                }
        //                else
        //                {
        //                    UnitData localItem3() { object argIndex1 = uname; var ret = SRC.UDList.Item(argIndex1); return ret; }

        //                    EffectNameRet = localItem3().Nickname + "に変身";
        //                }

        //                break;
        //            }

        //        case "能力コピー":
        //            {
        //                if (0d < elevel2 && elevel2 <= 10d)
        //                {
        //                    EffectNameRet = "任意の味方ユニットに変身" + "(" + SrcFormatter.Format(elevel2) + "ターン)";
        //                }
        //                else
        //                {
        //                    EffectNameRet = "任意の味方ユニットに変身";
        //                }

        //                break;
        //            }

        //        case "再行動":
        //            {
        //                if (MaxRange != 0)
        //                {
        //                    EffectNameRet = "行動済みユニットを再行動";
        //                }
        //                else
        //                {
        //                    EffectNameRet = "行動非消費";
        //                }

        //                break;
        //            }

        //        case "解説":
        //            {
        //                EffectNameRet = ae.Data;
        //                break;
        //            }
        //    }

        //    return EffectNameRet;
        //}

        //// 付加する武器強化系能力の対象表示用に武器の種類を判定
        //private string WeaponType(string wclass)
        //{
        //    string WeaponTypeRet = default;
        //    if (wclass == "全" || string.IsNullOrEmpty(wclass))
        //    {
        //        WeaponTypeRet = "武器";
        //    }
        //    else if (Strings.Len(wclass) == 1)
        //    {
        //        WeaponTypeRet = Help.AttributeName(null, wclass);
        //    }
        //    else if (Strings.Right(wclass, 2) == "装備")
        //    {
        //        WeaponTypeRet = Strings.Left(wclass, Strings.Len(wclass) - 2);
        //    }
        //    else
        //    {
        //        WeaponTypeRet = wclass + "属性攻撃";
        //    }

        //    return WeaponTypeRet;
        //}

        // 使い捨てアイテムによるアビリティかどうかを返す
        public bool IsItem()
        {
            bool IsItemRet = default;
            int i;
            var loopTo = GeneralLib.LLength(NecessarySkill);
            for (i = 1; i <= loopTo; i++)
            {
                if (GeneralLib.LIndex(NecessarySkill, i) == "アイテム")
                {
                    IsItemRet = true;
                    return IsItemRet;
                }
            }

            return IsItemRet;
        }
    }
}
