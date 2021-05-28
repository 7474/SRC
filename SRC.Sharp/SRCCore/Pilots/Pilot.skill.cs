// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.VB;
using System.Linq;
using System.Text.RegularExpressions;

namespace SRCCore.Pilots
{
    // === 特殊能力関連処理 ===
    public partial class Pilot
    {
        // 特殊能力の総数
        public int CountSkill()
        {
            return colSkill.Count;
        }

        // 特殊能力
        public string Skill(string Index)
        {
            return colSkill[Index]?.Name ?? "";
        }
        public string Skill(int Index)
        {
            return colSkill[Index]?.Name ?? "";
        }

        public bool HasSupportSkill()
        {
            return IsSkillAvailable("援護")
                || IsSkillAvailable("援護攻撃")
                || IsSkillAvailable("援護防御")
                || IsSkillAvailable("統率")
                || IsSkillAvailable("指揮")
                || IsSkillAvailable("広域サポート");
        }

        // 現在のレベルにおいて特殊能力 sname が使用可能か
        public bool IsSkillAvailable(string sname)
        {
            if (!string.IsNullOrEmpty(Skill(sname)))
            {
                return true;
            }

            // 特殊能力付加＆強化による修正
            if (Unit != null)
            {
                // TODO Impl
                //var u = Unit;
                //if (u.CountCondition() == 0)
                //{
                //    return false;
                //}

                //if (u.CountPilot() == 0)
                //{
                //    return false;
                //}

                //if (!ReferenceEquals(this, u.MainPilot()) & !ReferenceEquals(this, u.Pilot(1)))
                //{
                //    return false;
                //}

                //if (u.IsConditionSatisfied(sname + "付加"))
                //{
                //    return true;
                //}
                //else if (u.IsConditionSatisfied(sname + "付加２"))
                //{
                //    return true;
                //}

                //if (u.IsConditionSatisfied(sname + "強化"))
                //{
                //    if (u.ConditionLevel(sname + "強化") > 0d)
                //    {
                //        return true;
                //    }
                //}

                //if (u.IsConditionSatisfied("強化２"))
                //{
                //    if (u.ConditionLevel("強化２") > 0d)
                //    {
                //        return true;
                //    }
                //}
            }

            return false;
        }

        //        // 現在のレベルにおいて特殊能力 sname が使用可能か
        //        // (付加による影響を無視した場合)
        //        public bool IsSkillAvailable2(string sname)
        //        {
        //            bool IsSkillAvailable2Ret = default;
        //            SkillData sd;
        //            ;
        //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
        //            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 48641


        //            Input:

        //                    On Error GoTo ErrorHandler

        //             */
        //            sd = (SkillData)colSkill[sname];
        //            IsSkillAvailable2Ret = true;
        //            return IsSkillAvailable2Ret;
        //        ErrorHandler:
        //            ;
        //            IsSkillAvailable2Ret = false;
        //        }

        // 現在のレベルにおける特殊能力 Index のレベル
        // データでレベル指定がない場合はレベル 1
        // 特殊能力が使用不能の場合はレベル 0
        public double SkillLevel(int Index, string ref_mode = "")
        {
            SkillData sd = colSkill[Index];
            return SkillLevel(null, ref_mode, sd);
        }
        public double SkillLevel(string Index, string ref_mode = "")
        {
            SkillData sd = colSkill[Index];
            return SkillLevel(Index, ref_mode, sd);
        }
        private double SkillLevel(string Index, string ref_mode, SkillData sd)
        {
            string sname = sd?.Name;
            double SkillLevelRet = sd?.Level ?? 0;
            if (SkillLevelRet == Constants.DEFAULT_LEVEL)
            {
                SkillLevelRet = 1d;
            }
            if (string.IsNullOrEmpty(sname))
            {
                if (string.IsNullOrEmpty(Index))
                {
                    return SkillLevelRet;
                }
                else
                {
                    sname = Index;
                }
            }
            if (ref_mode == "修正値")
            {
                SkillLevelRet = 0d;
            }
            else if (ref_mode == "基本値")
            {
                return SkillLevelRet;
            }

            // 重複可能な能力は特殊能力付加で置き換えられことはない
            switch (sname ?? "")
            {
                case "ハンター":
                case "ＳＰ消費減少":
                case "スペシャルパワー自動発動":
                    {
                        if (Information.IsNumeric(Index))
                        {
                            return SkillLevelRet;
                        }

                        break;
                    }
            }

            // 特殊能力付加＆強化による修正
            if (Unit is null)
            {
                return SkillLevelRet;
            }

            {
                // TODO Impl
                //if (Unit.CountCondition() == 0)
                //{
                //    return SkillLevelRet;
                //}

                //if (Unit.CountPilot() == 0)
                //{
                //    return SkillLevelRet;
                //}

                //if (!ReferenceEquals(this, Unit.MainPilot()) & !ReferenceEquals(this, Unit.Pilot(1)))
                //{
                //    return SkillLevelRet;
                //}

                //bool localIsConditionSatisfied() { object 1 = sname + "付加２"; var ret = Unit.IsConditionSatisfied(1); return ret; }

                //if (Unit.IsConditionSatisfied(sname + "付加"))
                //{
                //    SkillLevelRet = Unit.ConditionLevel(sname + "付加");
                //    if (SkillLevelRet == Constants.DEFAULT_LEVEL)
                //    {
                //        SkillLevelRet = 1d;
                //    }
                //}
                //else if (localIsConditionSatisfied())
                //{
                //    SkillLevelRet = Unit.ConditionLevel(sname + "付加２");
                //    if (SkillLevelRet == Constants.DEFAULT_LEVEL)
                //    {
                //        SkillLevelRet = 1d;
                //    }
                //}

                //if (Unit.IsConditionSatisfied(sname + "強化"))
                //{
                //    double localConditionLevel() { object argIndex1 = sname + "強化"; var ret = Unit.ConditionLevel(argIndex1); return ret; }

                //    SkillLevelRet = SkillLevelRet + localConditionLevel();
                //}

                //if (Unit.IsConditionSatisfied(sname + "強化２"))
                //{
                //    double localConditionLevel1() { object argIndex1 = sname + "強化２"; var ret = Unit.ConditionLevel(argIndex1); return ret; }

                //    SkillLevelRet = SkillLevelRet + localConditionLevel1();
                //}
            }

            return SkillLevelRet;
        }

        // 特殊能力 Index にレベル指定がなされているか判定
        public bool IsSkillLevelSpecified(int Index)
        {
            SkillData sd = colSkill[Index];
            if (sd != null)
            {
                return sd.Level != Constants.DEFAULT_LEVEL;
            }
            return false;
        }

        public bool IsSkillLevelSpecified(string Index)
        {
            SkillData sd = colSkill[Index];
            if (sd != null)
            {
                return sd.Level != Constants.DEFAULT_LEVEL;
            }

            // 特殊能力付加＆強化による修正
            if (Unit is null)
            {
                return false;
            }

            {
                string sname = Index;
                var u = Unit;
                if (u.CountCondition() == 0)
                {
                    return false;
                }

                if (u.CountPilot() == 0)
                {
                    return false;
                }

                if (!ReferenceEquals(this, u.MainPilot()) & !ReferenceEquals(this, u.Pilots.First()))
                {
                    return false;
                }

                if (u.IsConditionSatisfied(sname + "付加"))
                {
                    if (u.ConditionLevel(sname + "付加") != Constants.DEFAULT_LEVEL)
                    {
                        return true;
                    }
                }
                else if (u.IsConditionSatisfied(sname + "付加２"))
                {
                    if (u.ConditionLevel(sname + "付加２") != Constants.DEFAULT_LEVEL)
                    {
                        return true;
                    }
                }

                if (u.IsConditionSatisfied(sname + "強化"))
                {
                    return true;
                }
                else if (u.IsConditionSatisfied(sname + "強化２"))
                {
                    return true;
                }
            }
            return false;
        }

        // 特殊能力のデータ
        public string SkillData(int Index)
        {
            var sd = colSkill[Index];
            return SkillData(null, sd);
        }
        public string SkillData(string Index)
        {
            var sd = colSkill[Index];
            return SkillData(Index, sd);
        }
        private static string SkillData(string Index, SkillData sd)
        {
            string sname = sd.Name;
            string SkillDataRet = sd?.StrData;
            return SkillDataRet;
            // TODO Impl
            //ErrorHandler:
            //    ;
            //    if (string.IsNullOrEmpty(sname))
            //    {
            //        if (Information.IsNumeric(Index))
            //        {
            //            return SkillDataRet;
            //        }
            //        else
            //        {
            //            // UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //            sname = Conversions.ToString(Index);
            //        }
            //    }

            //    // 重複可能な能力は特殊能力付加で置き換えられことはない
            //    switch (sname ?? "")
            //    {
            //        case "ハンター":
            //        case "ＳＰ消費減少":
            //        case "スペシャルパワー自動発動":
            //            {
            //                if (Information.IsNumeric(Index))
            //                {
            //                    return SkillDataRet;
            //                }

            //                break;
            //            }
            //    }

            //    // 特殊能力付加＆強化による修正
            //    if (Unit is null)
            //    {
            //        return SkillDataRet;
            //    }

            //    {
            //        var withBlock = Unit;
            //        if (withBlock.CountCondition() == 0)
            //        {
            //            return SkillDataRet;
            //        }

            //        if (withBlock.CountPilot() == 0)
            //        {
            //            return SkillDataRet;
            //        }

            //        if (!ReferenceEquals(this, withBlock.MainPilot()) & !ReferenceEquals(this, withBlock.Pilot(1)))
            //        {
            //            return SkillDataRet;
            //        }

            //        bool localIsConditionSatisfied() { object 1 = sname + "付加２"; var ret = withBlock.IsConditionSatisfied(1); return ret; }

            //        if (withBlock.IsConditionSatisfied(sname + "付加"))
            //        {
            //            SkillDataRet = withBlock.ConditionData(sname + "付加");
            //        }
            //        else if (localIsConditionSatisfied())
            //        {
            //            SkillDataRet = withBlock.ConditionData(sname + "付加２");
            //        }

            //        if (withBlock.IsConditionSatisfied(sname + "強化"))
            //        {
            //            string localConditionData() { object argIndex1 = sname + "強化"; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //            if (Strings.Len(localConditionData()) > 0)
            //            {
            //                SkillDataRet = withBlock.ConditionData(sname + "強化");
            //            }
            //        }

            //        if (withBlock.IsConditionSatisfied(sname + "強化２"))
            //        {
            //            string localConditionData1() { object argIndex1 = sname + "強化２"; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //            if (Strings.Len(localConditionData1()) > 0)
            //            {
            //                SkillDataRet = withBlock.ConditionData(sname + "強化２");
            //            }
            //        }
            //    }

            //    return SkillDataRet;
        }

        // 特殊能力の名称
        public string SkillName(int Index)
        {
            SkillData sd = colSkill[Index];
            return SkillName(sd);
        }
        public string SkillName(string Index)
        {
            // パイロットが所有している特殊能力の中から検索
            SkillData sd = colSkill[Index];
            if (sd != null)
            {
                return SkillName(sd);
            }
            // TODO Impl SkillName 仕様が重い
            return "";
            //        ErrorHandler:
            //            ;
            //            if (string.IsNullOrEmpty(sname))
            //            {
            //                if (Information.IsNumeric(Index))
            //                {
            //                    return SkillNameRet;
            //                }
            //                else
            //                {
            //                    // UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    sname = Conversions.ToString(Index);
            //                }
            //            }

            //            if (sname == "耐久")
            //            {
            //                if (Expression.IsOptionDefined("防御力成長") | Expression.IsOptionDefined("防御力成長"1))
            //                {
            //                    // 防御力成長オプション使用時には耐久能力を非表示
            //                    SkillNameRet = "非表示";
            //                    return SkillNameRet;
            //                }
            //            }

            //            // 得意技＆不得手は名称変更されない
            //            switch (sname ?? "")
            //            {
            //                case "得意技":
            //                case "不得手":
            //                    {
            //                        SkillNameRet = sname;
            //                        return SkillNameRet;
            //                    }
            //            }

            //            // SetSkillコマンドで封印されている場合
            //            if (string.IsNullOrEmpty(SkillNameRet))
            //            {
            //                if (Expression.IsGlobalVariableDefined("Ability(" + ID + "," + sname + ")"))
            //                {
            //                    // オリジナルの名称を使用
            //                    SkillNameRet = Data.SkillName(Level, sname);
            //                    if (Strings.InStr(SkillNameRet, "非表示") > 0)
            //                    {
            //                        SkillNameRet = "非表示";
            //                        return SkillNameRet;
            //                    }
            //                }
            //            }

            //            // 重複可能な能力は特殊能力付加で名称が置き換えられことはない
            //            switch (sname ?? "")
            //            {
            //                case "ハンター":
            //                case "スペシャルパワー自動発動":
            //                    {
            //                        if (Information.IsNumeric(Index))
            //                        {
            //                            if (Strings.Left(SkillNameRet, 1) == "(")
            //                            {
            //                                SkillNameRet = Strings.Mid(SkillNameRet, 2);
            //                                SkillNameRet = Strings.Left(SkillNameRet, GeneralLib.InStr2(SkillNameRet, ")") - 1);
            //                            }

            //                            return SkillNameRet;
            //                        }

            //                        break;
            //                    }

            //                case "ＳＰ消費減少":
            //                    {
            //                        if (Information.IsNumeric(Index))
            //                        {
            //                            if (Strings.Left(SkillNameRet, 1) == "(")
            //                            {
            //                                SkillNameRet = Strings.Mid(SkillNameRet, 2);
            //                                SkillNameRet = Strings.Left(SkillNameRet, GeneralLib.InStr2(SkillNameRet, ")") - 1);
            //                            }

            //                            i = Strings.InStr(SkillNameRet, "Lv");
            //                            if (i > 0)
            //                            {
            //                                SkillNameRet = Strings.Left(SkillNameRet, i - 1);
            //                            }

            //                            return SkillNameRet;
            //                        }

            //                        break;
            //                    }
            //            }

            //            // 特殊能力付加＆強化による修正
            //            if (Unit is object)
            //            {
            //                {
            //                    var withBlock = Unit;
            //                    if (withBlock.CountCondition() > 0 & withBlock.CountPilot() > 0)
            //                    {
            //                        if (ReferenceEquals(withBlock.MainPilot(), this) | ReferenceEquals(withBlock.Pilot(1), this))
            //                        {
            //                            // ユニット用特殊能力による付加
            //                            if (withBlock.IsConditionSatisfied(sname + "付加２"))
            //                            {
            //                                string localConditionData() { object argIndex1 = sname + "付加２"; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //                                buf = GeneralLib.LIndex(localConditionData(), 1);
            //                                if (!string.IsNullOrEmpty(buf))
            //                                {
            //                                    SkillNameRet = buf;
            //                                }
            //                                else if (string.IsNullOrEmpty(SkillNameRet))
            //                                {
            //                                    SkillNameRet = sname;
            //                                }

            //                                if (Strings.InStr(SkillNameRet, "非表示") > 0)
            //                                {
            //                                    SkillNameRet = "非表示";
            //                                    return SkillNameRet;
            //                                }

            //                                // レベル指定
            //                                if (withBlock.ConditionLevel(sname + "付加２") != Constants.DEFAULT_LEVEL)
            //                                {
            //                                    if (Strings.InStr(SkillNameRet, "Lv") > 0)
            //                                    {
            //                                        SkillNameRet = Strings.Left(SkillNameRet, Strings.InStr(SkillNameRet, "Lv") - 1);
            //                                    }

            //                                    double localConditionLevel() { object argIndex1 = sname + "付加２"; var ret = withBlock.ConditionLevel(argIndex1); return ret; }

            //                                    SkillNameRet = SkillNameRet + "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLevel());
            //                                }
            //                            }

            //                            // アビリティによる付加
            //                            if (withBlock.IsConditionSatisfied(sname + "付加"))
            //                            {
            //                                string localConditionData1() { object argIndex1 = sname + "付加"; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //                                buf = GeneralLib.LIndex(localConditionData1(), 1);
            //                                if (!string.IsNullOrEmpty(buf))
            //                                {
            //                                    SkillNameRet = buf;
            //                                }
            //                                else if (string.IsNullOrEmpty(SkillNameRet))
            //                                {
            //                                    SkillNameRet = sname;
            //                                }

            //                                if (Strings.InStr(SkillNameRet, "非表示") > 0)
            //                                {
            //                                    SkillNameRet = "非表示";
            //                                    return SkillNameRet;
            //                                }

            //                                // レベル指定
            //                                if (withBlock.ConditionLevel(sname + "付加") != Constants.DEFAULT_LEVEL)
            //                                {
            //                                    if (Strings.InStr(SkillNameRet, "Lv") > 0)
            //                                    {
            //                                        SkillNameRet = Strings.Left(SkillNameRet, Strings.InStr(SkillNameRet, "Lv") - 1);
            //                                    }

            //                                    double localConditionLevel1() { object argIndex1 = sname + "付加"; var ret = withBlock.ConditionLevel(argIndex1); return ret; }

            //                                    SkillNameRet = SkillNameRet + "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLevel1());
            //                                }
            //                            }

            //                            // ユニット用特殊能力による強化
            //                            if (withBlock.IsConditionSatisfied(sname + "強化２"))
            //                            {
            //                                if (string.IsNullOrEmpty(SkillNameRet))
            //                                {
            //                                    // 強化される能力をパイロットが持っていなかった場合
            //                                    string localConditionData2() { object argIndex1 = sname + "強化２"; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //                                    SkillNameRet = GeneralLib.LIndex(localConditionData2(), 1);
            //                                    if (string.IsNullOrEmpty(SkillNameRet))
            //                                    {
            //                                        SkillNameRet = sname;
            //                                    }

            //                                    if (Strings.InStr(SkillNameRet, "非表示") > 0)
            //                                    {
            //                                        SkillNameRet = "非表示";
            //                                        return SkillNameRet;
            //                                    }

            //                                    SkillNameRet = SkillNameRet + "Lv0";
            //                                }

            //                                if (sname != "同調率" & sname != "霊力")
            //                                {
            //                                    if (withBlock.ConditionLevel(sname + "強化２") >= 0d)
            //                                    {
            //                                        double localConditionLevel2() { object argIndex1 = sname + "強化２"; var ret = withBlock.ConditionLevel(argIndex1); return ret; }

            //                                        SkillNameRet = SkillNameRet + "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLevel2());
            //                                    }
            //                                    else
            //                                    {
            //                                        double localConditionLevel3() { object argIndex1 = sname + "強化２"; var ret = withBlock.ConditionLevel(argIndex1); return ret; }

            //                                        SkillNameRet = SkillNameRet + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLevel3());
            //                                    }
            //                                }
            //                            }

            //                            // アビリティによる強化
            //                            if (withBlock.IsConditionSatisfied(sname + "強化"))
            //                            {
            //                                if (string.IsNullOrEmpty(SkillNameRet))
            //                                {
            //                                    // 強化される能力をパイロットが持っていなかった場合
            //                                    string localConditionData3() { object argIndex1 = sname + "強化"; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //                                    SkillNameRet = GeneralLib.LIndex(localConditionData3(), 1);
            //                                    if (string.IsNullOrEmpty(SkillNameRet))
            //                                    {
            //                                        SkillNameRet = sname;
            //                                    }

            //                                    if (Strings.InStr(SkillNameRet, "非表示") > 0)
            //                                    {
            //                                        SkillNameRet = "非表示";
            //                                        return SkillNameRet;
            //                                    }

            //                                    SkillNameRet = SkillNameRet + "Lv0";
            //                                }

            //                                if (sname != "同調率" & sname != "霊力")
            //                                {
            //                                    if (withBlock.ConditionLevel(sname + "強化") >= 0d)
            //                                    {
            //                                        double localConditionLevel4() { object argIndex1 = sname + "強化"; var ret = withBlock.ConditionLevel(argIndex1); return ret; }

            //                                        SkillNameRet = SkillNameRet + "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLevel4());
            //                                    }
            //                                    else
            //                                    {
            //                                        double localConditionLevel5() { object argIndex1 = sname + "強化"; var ret = withBlock.ConditionLevel(argIndex1); return ret; }

            //                                        SkillNameRet = SkillNameRet + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLevel5());
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            // 能力強化系は非表示
            //            if (Strings.Right(sname, 2) == "ＵＰ" | Strings.Right(sname, 4) == "ＤＯＷＮ")
            //            {
            //                SkillNameRet = "非表示";
            //                return SkillNameRet;
            //            }

            //            switch (sname ?? "")
            //            {
            //                case "追加レベル":
            //                case "メッセージ":
            //                case "魔力所有":
            //                    {
            //                        // 非表示の能力
            //                        SkillNameRet = "非表示";
            //                        return SkillNameRet;
            //                    }

            //                case "耐久":
            //                    {
            //                        if (Expression.IsOptionDefined("防御力成長") | Expression.IsOptionDefined("防御力レベルアップ"))
            //                        {
            //                            // 防御力成長オプション使用時には耐久能力を非表示
            //                            SkillNameRet = "非表示";
            //                            return SkillNameRet;
            //                        }

            //                        break;
            //                    }
            //            }

            //            // これらの能力からはレベル指定を除く
            //            switch (sname ?? "")
            //            {
            //                case "階級":
            //                case "同調率":
            //                case "霊力":
            //                case "ＳＰ消費減少":
            //                    {
            //                        i = Strings.InStr(SkillNameRet, "Lv");
            //                        if (i > 0)
            //                        {
            //                            SkillNameRet = Strings.Left(SkillNameRet, i - 1);
            //                        }

            //                        break;
            //                    }
            //            }

            //            // レベル非表示用の括弧を削除
            //            if (Strings.Left(SkillNameRet, 1) == "(")
            //            {
            //                SkillNameRet = Strings.Mid(SkillNameRet, 2);
            //                SkillNameRet = Strings.Left(SkillNameRet, GeneralLib.InStr2(SkillNameRet, ")") - 1);
            //            }

            //            if (string.IsNullOrEmpty(SkillNameRet))
            //            {
            //                SkillNameRet = sname;
            //            }

            //            return SkillNameRet;
        }

        private static string SkillName(SkillData sd)
        {
            return sd?.Name ?? "";
            // TODO Impl SkillName 仕様が重い
            //sname = sd.Name;
            //            // 能力強化系は非表示
            //            if (Strings.Right(sname, 2) == "ＵＰ" | Strings.Right(sname, 4) == "ＤＯＷＮ")
            //            {
            //                SkillNameRet = "非表示";
            //                return SkillNameRet;
            //            }

            //            switch (sname ?? "")
            //            {
            //                case "追加レベル":
            //                case "メッセージ":
            //                case "魔力所有":
            //                    {
            //                        // 非表示の能力
            //                        SkillNameRet = "非表示";
            //                        return SkillNameRet;
            //                    }

            //                case "得意技":
            //                case "不得手":
            //                    {
            //                        // 別名指定が存在しない能力
            //                        SkillNameRet = sname;
            //                        return SkillNameRet;
            //                    }
            //            }

            //            if (Strings.Len(sd.StrData) > 0)
            //            {
            //                SkillNameRet = GeneralLib.LIndex(sd.StrData, 1);
            //                switch (SkillNameRet ?? "")
            //                {
            //                    case "非表示":
            //                        {
            //                            return SkillNameRet;
            //                        }

            //                    case "解説":
            //                        {
            //                            SkillNameRet = "非表示";
            //                            return SkillNameRet;
            //                        }
            //                }
            //            }
            //            else
            //            {
            //                SkillNameRet = sname;
            //            }

            //            // レベル指定
            //            if (sd.Level != Constants.DEFAULT_LEVEL & Strings.InStr(SkillNameRet, "Lv") == 0 & Strings.Left(SkillNameRet, 1) != "(")
            //            {
            //                SkillNameRet = SkillNameRet + "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(sd.Level);
            //            }
        }

        // 特殊能力名称（レベル表示抜き）
        public string SkillName0(int Index)
        {
            SkillData sd = colSkill[Index];
            return SkillName0(sd);
        }
        public string SkillName0(string Index)
        {
            SkillData sd = colSkill[Index];
            if (sd != null)
            {
                var sname0 = SkillName0(sd);
                if (!string.IsNullOrEmpty(sname0))
                {
                    return sname0;
                }
            }

            var SkillName0Ret = "";
            var sname = Index;

            if (sname == "耐久")
            {
                if (Expression.IsOptionDefined("防御力成長") | Expression.IsOptionDefined("防御力レベルアップ"))
                {
                    // 防御力成長オプション使用時には耐久能力を非表示
                    SkillName0Ret = "非表示";
                    return SkillName0Ret;
                }
            }

            // 得意技＆不得手は名称変更されない
            switch (sname ?? "")
            {
                case "得意技":
                case "不得手":
                    {
                        SkillName0Ret = sname;
                        return SkillName0Ret;
                    }
            }

            // SetSkillコマンドで封印されている場合
            if (string.IsNullOrEmpty(SkillName0Ret))
            {
                if (Expression.IsGlobalVariableDefined("Ability(" + ID + "," + sname + ")"))
                {
                    // オリジナルの名称を使用
                    SkillName0Ret = Data.SkillName(Level, sname);
                    if (Strings.InStr(SkillName0Ret, "非表示") > 0)
                    {
                        SkillName0Ret = "非表示";
                        return SkillName0Ret;
                    }
                }
            }

            // 重複可能な能力は特殊能力付加で名称が置き換えられことはない
            switch (sname ?? "")
            {
                case "ハンター":
                case "ＳＰ消費減少":
                case "スペシャルパワー自動発動":
                    {
                        if (Information.IsNumeric(Index))
                        {
                            return SkillName0Ret;
                        }

                        break;
                    }
            }

            // 特殊能力付加＆強化による修正
            if (Unit is object)
            {
                {
                    var u = Unit;
                    if (u.CountCondition() > 0 & u.CountPilot() > 0)
                    {
                        if (ReferenceEquals(u.MainPilot(), this) | ReferenceEquals(u.Pilot(1), this))
                        {
                            // ユニット用特殊能力による付加
                            if (u.IsConditionSatisfied(sname + "付加２"))
                            {
                                var buf = GeneralLib.LIndex(u.Condition(sname + "付加２").StrData, 1);
                                if (!string.IsNullOrEmpty(buf))
                                {
                                    SkillName0Ret = buf;
                                }
                                else if (string.IsNullOrEmpty(SkillName0Ret))
                                {
                                    SkillName0Ret = sname;
                                }

                                if (Strings.InStr(SkillName0Ret, "非表示") > 0)
                                {
                                    SkillName0Ret = "非表示";
                                    return SkillName0Ret;
                                }
                            }

                            // アビリティによる付加
                            if (u.IsConditionSatisfied(sname + "付加"))
                            {
                                var buf = GeneralLib.LIndex(u.Condition(sname + "付加").StrData, 1);
                                if (!string.IsNullOrEmpty(buf))
                                {
                                    SkillName0Ret = buf;
                                }
                                else if (string.IsNullOrEmpty(SkillName0Ret))
                                {
                                    SkillName0Ret = sname;
                                }

                                if (Strings.InStr(SkillName0Ret, "非表示") > 0)
                                {
                                    SkillName0Ret = "非表示";
                                    return SkillName0Ret;
                                }
                            }

                            // ユニット用特殊能力による強化
                            if (string.IsNullOrEmpty(SkillName0Ret))
                            {
                                if (u.IsConditionSatisfied(sname + "強化２"))
                                {
                                    SkillName0Ret = GeneralLib.LIndex(u.Condition(sname + "強化２").StrData, 1);
                                    if (string.IsNullOrEmpty(SkillName0Ret))
                                    {
                                        SkillName0Ret = sname;
                                    }

                                    if (Strings.InStr(SkillName0Ret, "非表示") > 0)
                                    {
                                        SkillName0Ret = "非表示";
                                        return SkillName0Ret;
                                    }
                                }
                            }

                            // アビリティによる強化
                            if (string.IsNullOrEmpty(SkillName0Ret))
                            {
                                if (u.IsConditionSatisfied(sname + "強化"))
                                {
                                    SkillName0Ret = GeneralLib.LIndex(u.Condition(sname + "強化").StrData, 1);
                                    if (string.IsNullOrEmpty(SkillName0Ret))
                                    {
                                        SkillName0Ret = sname;
                                    }

                                    if (Strings.InStr(SkillName0Ret, "非表示") > 0)
                                    {
                                        SkillName0Ret = "非表示";
                                        return SkillName0Ret;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // 該当するものが無ければエリアスから検索
            if (string.IsNullOrEmpty(SkillName0Ret))
            {
                var asname = SRC.ALDList.RefName(sname);
                if (asname != sname)
                {
                    return asname;
                }
                SkillName0Ret = sname;
            }

            // 能力強化系は非表示
            if (Strings.Right(sname, 2) == "ＵＰ" | Strings.Right(sname, 4) == "ＤＯＷＮ")
            {
                SkillName0Ret = "非表示";
                return SkillName0Ret;
            }

            switch (sname ?? "")
            {
                case "追加レベル":
                case "メッセージ":
                case "魔力所有":
                    {
                        // 非表示の能力
                        SkillName0Ret = "非表示";
                        return SkillName0Ret;
                    }

                case "耐久":
                    {
                        if (Expression.IsOptionDefined("防御力成長") | Expression.IsOptionDefined("防御力レベルアップ"))
                        {
                            // 防御力成長オプション使用時には耐久能力を非表示
                            SkillName0Ret = "非表示";
                            return SkillName0Ret;
                        }

                        break;
                    }
            }

            // レベル非表示用の括弧を削除
            if (Strings.Left(SkillName0Ret, 1) == "(")
            {
                SkillName0Ret = Strings.Mid(SkillName0Ret, 2);
                SkillName0Ret = Strings.Left(SkillName0Ret, GeneralLib.InStr2(SkillName0Ret, ")") - 1);
            }

            // レベル指定を削除
            {
                var i = Strings.InStr(SkillName0Ret, "Lv");
                if (i > 0)
                {
                    SkillName0Ret = Strings.Left(SkillName0Ret, i - 1);
                }
            }

            return SkillName0Ret;
        }

        private static string SkillName0(SkillData sd)
        {
            var sname = sd.Name;

            // 能力強化系は非表示
            if (Strings.Right(sname, 2) == "ＵＰ" | Strings.Right(sname, 4) == "ＤＯＷＮ")
            {
                return "非表示";
            }

            switch (sname ?? "")
            {
                case "追加レベル":
                case "メッセージ":
                case "魔力所有":
                    {
                        // 非表示の能力
                        return "非表示";
                    }

                case "得意技":
                case "不得手":
                    {
                        // 別名指定が存在しない能力
                        return sname;
                    }
            }

            if (Strings.Len(sd.StrData) > 0)
            {
                var aname = GeneralLib.LIndex(sd.StrData, 1);
                if (aname == "非表示")
                {
                    return aname;
                }
            }
            else
            {
                return sname;
            }

            return "";
        }

        //        // 特殊能力名称（必要技能判定用）
        //        // 名称からレベル指定を削除し、名称が非表示にされている場合は元の特殊能力名
        //        // もしくはエリアス名を使用する。
        //        public string SkillNameForNS(string stype)
        //        {
        //            string SkillNameForNSRet = default;
        //            SkillData sd;
        //            string buf;
        //            int i;

        //            // 非表示の特殊能力
        //            if (Strings.Right(stype, 2) == "ＵＰ" | Strings.Right(stype, 4) == "ＤＯＷＮ")
        //            {
        //                SkillNameForNSRet = stype;
        //                return SkillNameForNSRet;
        //            }

        //            if (stype == "メッセージ")
        //            {
        //                SkillNameForNSRet = stype;
        //                return SkillNameForNSRet;

        //                // パイロットが所有している特殊能力の中から検索
        //            };
        //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
        //            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 69788


        //            Input:

        //                    'パイロットが所有している特殊能力の中から検索
        //                    On Error GoTo ErrorHandler

        //             */
        //            sd = (SkillData)colSkill[stype];
        //            if (Strings.Len(sd.StrData) > 0)
        //            {
        //                SkillNameForNSRet = GeneralLib.LIndex(sd.StrData, 1);
        //            }
        //            else
        //            {
        //                SkillNameForNSRet = stype;
        //            }

        //        ErrorHandler:
        //            ;


        //            // SetSkillコマンドで封印されている場合
        //            if (string.IsNullOrEmpty(SkillNameForNSRet))
        //            {
        //                if (Expression.IsGlobalVariableDefined("Ability(" + ID + "," + stype + ")"))
        //                {
        //                    // オリジナルの名称を使用
        //                    SkillNameForNSRet = Data.SkillName(Level, stype);
        //                    if (Strings.InStr(SkillNameForNSRet, "非表示") > 0)
        //                    {
        //                        SkillNameForNSRet = "非表示";
        //                    }
        //                }
        //            }

        //            // 特殊能力付加＆強化による修正
        //            if (Unit is object)
        //            {
        //                {
        //                    var withBlock = Unit;
        //                    if (withBlock.CountCondition() > 0 & withBlock.CountPilot() > 0)
        //                    {
        //                        if (ReferenceEquals(this, withBlock.MainPilot()) | ReferenceEquals(this, withBlock.Pilot(1)))
        //                        {
        //                            // ユニット用特殊能力による付加
        //                            if (withBlock.IsConditionSatisfied(stype + "付加２"))
        //                            {
        //                                string localConditionData() { object argIndex1 = (object)(stype + "付加２"); var ret = withBlock.ConditionData(argIndex1); return ret; }

        //                                buf = GeneralLib.LIndex(localConditionData(), 1);
        //                                if (!string.IsNullOrEmpty(buf))
        //                                {
        //                                    SkillNameForNSRet = buf;
        //                                }
        //                                else if (string.IsNullOrEmpty(SkillNameForNSRet))
        //                                {
        //                                    SkillNameForNSRet = stype;
        //                                }

        //                                if (Strings.InStr(SkillNameForNSRet, "非表示") > 0)
        //                                {
        //                                    SkillNameForNSRet = "非表示";
        //                                }
        //                            }

        //                            // アビリティによる付加
        //                            if (withBlock.IsConditionSatisfied(stype + "付加"))
        //                            {
        //                                string localConditionData1() { object argIndex1 = (object)(stype + "付加"); var ret = withBlock.ConditionData(argIndex1); return ret; }

        //                                buf = GeneralLib.LIndex(localConditionData1(), 1);
        //                                if (!string.IsNullOrEmpty(buf))
        //                                {
        //                                    SkillNameForNSRet = buf;
        //                                }
        //                                else if (string.IsNullOrEmpty(SkillNameForNSRet))
        //                                {
        //                                    SkillNameForNSRet = stype;
        //                                }

        //                                if (Strings.InStr(SkillNameForNSRet, "非表示") > 0)
        //                                {
        //                                    SkillNameForNSRet = "非表示";
        //                                }
        //                            }

        //                            // ユニット用特殊能力による強化
        //                            if (string.IsNullOrEmpty(SkillNameForNSRet))
        //                            {
        //                                if (withBlock.IsConditionSatisfied(stype + "強化２"))
        //                                {
        //                                    string localConditionData2() { object argIndex1 = (object)(stype + "強化２"); var ret = withBlock.ConditionData(argIndex1); return ret; }

        //                                    SkillNameForNSRet = GeneralLib.LIndex(localConditionData2(), 1);
        //                                    if (string.IsNullOrEmpty(SkillNameForNSRet))
        //                                    {
        //                                        SkillNameForNSRet = stype;
        //                                    }

        //                                    if (Strings.InStr(SkillNameForNSRet, "非表示") > 0)
        //                                    {
        //                                        SkillNameForNSRet = "非表示";
        //                                    }
        //                                }
        //                            }

        //                            // アビリティによる強化
        //                            if (string.IsNullOrEmpty(SkillNameForNSRet))
        //                            {
        //                                if (withBlock.IsConditionSatisfied(stype + "強化"))
        //                                {
        //                                    string localConditionData3() { object argIndex1 = (object)(stype + "強化"); var ret = withBlock.ConditionData(argIndex1); return ret; }

        //                                    SkillNameForNSRet = GeneralLib.LIndex(localConditionData3(), 1);
        //                                    if (string.IsNullOrEmpty(SkillNameForNSRet))
        //                                    {
        //                                        SkillNameForNSRet = stype;
        //                                    }

        //                                    if (Strings.InStr(SkillNameForNSRet, "非表示") > 0)
        //                                    {
        //                                        SkillNameForNSRet = "非表示";
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            // 該当するものが無ければエリアスから検索
        //            if (string.IsNullOrEmpty(SkillNameForNSRet) | SkillNameForNSRet == "非表示")
        //            {
        //                {
        //                    var withBlock1 = SRC.ALDList;
        //                    var loopTo = withBlock1.Count();
        //                    for (i = 1; i <= loopTo; i++)
        //                    {
        //                        {
        //                            var withBlock2 = withBlock1.Item(i);
        //                            if ((withBlock2.get_AliasType(1) ?? "") == (stype ?? ""))
        //                            {
        //                                SkillNameForNSRet = withBlock2.Name;
        //                                return SkillNameForNSRet;
        //                            }
        //                        }
        //                    }
        //                }

        //                SkillNameForNSRet = stype;
        //            }

        //            // レベル非表示用の括弧を削除
        //            if (Strings.Left(SkillNameForNSRet, 1) == "(")
        //            {
        //                SkillNameForNSRet = Strings.Mid(SkillNameForNSRet, 2);
        //                SkillNameForNSRet = Strings.Left(SkillNameForNSRet, GeneralLib.InStr2(SkillNameForNSRet, ")") - 1);
        //            }

        //            // レベル表示を削除
        //            i = Strings.InStr(SkillNameForNSRet, "Lv");
        //            if (i > 0)
        //            {
        //                SkillNameForNSRet = Strings.Left(SkillNameForNSRet, i - 1);
        //            }

        //            return SkillNameForNSRet;
        //        }

        // 特殊能力の種類
        public string SkillType(string sname)
        {
            string SkillTypeRet = default;
            int i;
            string sname0, sname2;
            if (string.IsNullOrEmpty(sname))
            {
                return SkillTypeRet;
            }

            i = Strings.InStr(sname, "Lv");
            if (i > 0)
            {
                sname0 = Strings.Left(sname, i - 1);
            }
            else
            {
                sname0 = sname;
            }

            // エリアスデータが定義されている？
            if (SRC.ALDList.IsDefined(sname0))
            {
                return SRC.ALDList.Item(sname0).ReplaceTypeName(sname0);
            }

            // 特殊能力一覧から検索
            foreach (SkillData sd in colSkill)
            {
                if ((sname0 ?? "") == (sd.Name ?? ""))
                {
                    SkillTypeRet = sd.Name;
                    return SkillTypeRet;
                }

                sname2 = GeneralLib.LIndex(sd.StrData, 1);
                if ((sname0 ?? "") == (sname2 ?? ""))
                {
                    SkillTypeRet = sd.Name;
                    return SkillTypeRet;
                }

                if (Strings.Left(sname2, 1) == "(")
                {
                    if (Strings.Right(sname2, 1) == ")")
                    {
                        sname2 = Strings.Mid(sname2, 2, Strings.Len(sname2) - 2);
                        if ((sname ?? "") == (sname2 ?? ""))
                        {
                            SkillTypeRet = sd.Name;
                            return SkillTypeRet;
                        }
                    }
                }
            }

            // その能力を修得していない
            SkillTypeRet = sname0;

            // 特殊能力付加による修正
            if (Unit != null)
            {
                var u = Unit;
                if (u.CountCondition() > 0 && u.CountPilot() > 0)
                {
                    if (ReferenceEquals(this, u.MainPilot()) || ReferenceEquals(this, u.Pilots.First()))
                    {
                        // XXX 疎通してないから怪しいかもしれん
                        var conditionType = u.Conditions
                            .Where(c => c.Name.EndsWith("付加") || c.Name.EndsWith("付加２"))
                            .Where(c => GeneralLib.LIndex(c.StrData, 1) == sname0)
                            .Select(c => Regex.Replace(c.Name, "付加２?$", ""))
                            .FirstOrDefault();
                        if (string.IsNullOrEmpty(conditionType))
                        {
                            return conditionType;
                        }
                    }
                }
            }

            return SkillTypeRet;
        }
    }
}
