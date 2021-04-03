// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Models;
using SRCCore.Units;
using SRCCore.VB;
using System;

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

                //object argIndex1 = 1;
                //object argIndex2 = 1;
                //if (!ReferenceEquals(this, u.MainPilot()) & !ReferenceEquals(this, u.Pilot(argIndex2)))
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
        public double SkillLevel(string Index, string ref_mode = "")
        {
            SkillData sd = colSkill[Index];
            string sname = sd?.Name;
            double SkillLevelRet = sd?.Level ?? 0;
            if (SkillLevelRet == Constants.DEFAULT_LEVEL)
            {
                SkillLevelRet = 1d;
            }

            if (string.IsNullOrEmpty(sname))
            {
                // XXX これ引数に数字で参照指定してた場合向けか？
                if (Information.IsNumeric(Index))
                {
                    return SkillLevelRet;
                }
                else
                {
                    sname = Conversions.ToString(Index);
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

                //object argIndex1 = 1;
                //object argIndex2 = 1;
                //if (!ReferenceEquals(this, Unit.MainPilot()) & !ReferenceEquals(this, Unit.Pilot(argIndex2)))
                //{
                //    return SkillLevelRet;
                //}

                //bool localIsConditionSatisfied() { object argIndex1 = sname + "付加２"; var ret = Unit.IsConditionSatisfied(argIndex1); return ret; }

                //object argIndex5 = sname + "付加";
                //if (Unit.IsConditionSatisfied(argIndex5))
                //{
                //    object argIndex3 = sname + "付加";
                //    SkillLevelRet = Unit.ConditionLevel(argIndex3);
                //    if (SkillLevelRet == Constants.DEFAULT_LEVEL)
                //    {
                //        SkillLevelRet = 1d;
                //    }
                //}
                //else if (localIsConditionSatisfied())
                //{
                //    object argIndex4 = sname + "付加２";
                //    SkillLevelRet = Unit.ConditionLevel(argIndex4);
                //    if (SkillLevelRet == Constants.DEFAULT_LEVEL)
                //    {
                //        SkillLevelRet = 1d;
                //    }
                //}

                //object argIndex6 = sname + "強化";
                //if (Unit.IsConditionSatisfied(argIndex6))
                //{
                //    double localConditionLevel() { object argIndex1 = sname + "強化"; var ret = Unit.ConditionLevel(argIndex1); return ret; }

                //    SkillLevelRet = SkillLevelRet + localConditionLevel();
                //}

                //object argIndex7 = sname + "強化２";
                //if (Unit.IsConditionSatisfied(argIndex7))
                //{
                //    double localConditionLevel1() { object argIndex1 = sname + "強化２"; var ret = Unit.ConditionLevel(argIndex1); return ret; }

                //    SkillLevelRet = SkillLevelRet + localConditionLevel1();
                //}
            }

            return SkillLevelRet;
        }

        // 特殊能力 Index にレベル指定がなされているか判定
        public bool IsSkillLevelSpecified(string Index)
        {
            string sname;
            SkillData sd = colSkill[Index];
            if (sd != null)
            {
                return sd.Level != Constants.DEFAULT_LEVEL;
            }
            // TODO Impl
            return false;
            //ErrorHandler:
            //    ;
            //    if (string.IsNullOrEmpty(sname))
            //    {
            //        if (Information.IsNumeric(Index))
            //        {
            //            return IsSkillLevelSpecifiedRet;
            //        }
            //        else
            //        {
            //            // UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //            sname = Conversions.ToString(Index);
            //        }
            //    }

            //    // 特殊能力付加＆強化による修正
            //    if (Unit is null)
            //    {
            //        return IsSkillLevelSpecifiedRet;
            //    }

            //    {
            //        var withBlock = Unit;
            //        if (withBlock.CountCondition() == 0)
            //        {
            //            return IsSkillLevelSpecifiedRet;
            //        }

            //        if (withBlock.CountPilot() == 0)
            //        {
            //            return IsSkillLevelSpecifiedRet;
            //        }

            //        object argIndex1 = 1;
            //        object argIndex2 = 1;
            //        if (!ReferenceEquals(this, withBlock.MainPilot()) & !ReferenceEquals(this, withBlock.Pilot(argIndex2)))
            //        {
            //            return IsSkillLevelSpecifiedRet;
            //        }

            //        bool localIsConditionSatisfied() { object argIndex1 = sname + "付加２"; var ret = withBlock.IsConditionSatisfied(argIndex1); return ret; }

            //        object argIndex5 = sname + "付加";
            //        if (withBlock.IsConditionSatisfied(argIndex5))
            //        {
            //            object argIndex3 = sname + "付加";
            //            if (withBlock.ConditionLevel(argIndex3) != Constants.DEFAULT_LEVEL)
            //            {
            //                IsSkillLevelSpecifiedRet = true;
            //            }
            //        }
            //        else if (localIsConditionSatisfied())
            //        {
            //            object argIndex4 = sname + "付加２";
            //            if (withBlock.ConditionLevel(argIndex4) != Constants.DEFAULT_LEVEL)
            //            {
            //                IsSkillLevelSpecifiedRet = true;
            //            }
            //        }

            //        bool localIsConditionSatisfied1() { object argIndex1 = sname + "強化２"; var ret = withBlock.IsConditionSatisfied(argIndex1); return ret; }

            //        object argIndex6 = sname + "強化";
            //        if (withBlock.IsConditionSatisfied(argIndex6))
            //        {
            //            IsSkillLevelSpecifiedRet = true;
            //        }
            //        else if (localIsConditionSatisfied1())
            //        {
            //            IsSkillLevelSpecifiedRet = true;
            //        }
            //    }
        }

        // 特殊能力のデータ
        public string SkillData(string Index)
        {
            string SkillDataRet = default;
            string sname;
            var sd = colSkill[Index];
            sname = sd.Name;
            SkillDataRet = sd?.StrData;
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

            //        object argIndex1 = 1;
            //        object argIndex2 = 1;
            //        if (!ReferenceEquals(this, withBlock.MainPilot()) & !ReferenceEquals(this, withBlock.Pilot(argIndex2)))
            //        {
            //            return SkillDataRet;
            //        }

            //        bool localIsConditionSatisfied() { object argIndex1 = sname + "付加２"; var ret = withBlock.IsConditionSatisfied(argIndex1); return ret; }

            //        object argIndex5 = sname + "付加";
            //        if (withBlock.IsConditionSatisfied(argIndex5))
            //        {
            //            object argIndex3 = sname + "付加";
            //            SkillDataRet = withBlock.ConditionData(argIndex3);
            //        }
            //        else if (localIsConditionSatisfied())
            //        {
            //            object argIndex4 = sname + "付加２";
            //            SkillDataRet = withBlock.ConditionData(argIndex4);
            //        }

            //        object argIndex7 = sname + "強化";
            //        if (withBlock.IsConditionSatisfied(argIndex7))
            //        {
            //            string localConditionData() { object argIndex1 = sname + "強化"; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //            if (Strings.Len(localConditionData()) > 0)
            //            {
            //                object argIndex6 = sname + "強化";
            //                SkillDataRet = withBlock.ConditionData(argIndex6);
            //            }
            //        }

            //        object argIndex9 = sname + "強化２";
            //        if (withBlock.IsConditionSatisfied(argIndex9))
            //        {
            //            string localConditionData1() { object argIndex1 = sname + "強化２"; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //            if (Strings.Len(localConditionData1()) > 0)
            //            {
            //                object argIndex8 = sname + "強化２";
            //                SkillDataRet = withBlock.ConditionData(argIndex8);
            //            }
            //        }
            //    }

            //    return SkillDataRet;
        }

        //        // 特殊能力の名称
        //        public string SkillName(string Index)
        //        {
        //            string SkillNameRet = default;
        //            SkillData sd;
        //            string sname;
        //            string buf;

        //            // パイロットが所有している特殊能力の中から検索
        //            int i;
        //            ;
        //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
        //            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 54690


        //            Input:

        //                    'パイロットが所有している特殊能力の中から検索
        //                    On Error GoTo ErrorHandler

        //             */
        //            sd = (SkillData)colSkill[Index];
        //            sname = sd.Name;

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
        //                string argoname = "防御力成長";
        //                string argoname1 = "防御力レベルアップ";
        //                if (Expression.IsOptionDefined(argoname) | Expression.IsOptionDefined(argoname1))
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
        //                string argvname = "Ability(" + ID + "," + sname + ")";
        //                if (Expression.IsGlobalVariableDefined(argvname))
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
        //                                string argstr2 = ")";
        //                                SkillNameRet = Strings.Left(SkillNameRet, GeneralLib.InStr2(SkillNameRet, argstr2) - 1);
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
        //                                string argstr21 = ")";
        //                                SkillNameRet = Strings.Left(SkillNameRet, GeneralLib.InStr2(SkillNameRet, argstr21) - 1);
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
        //                        object argIndex9 = 1;
        //                        if (ReferenceEquals(withBlock.MainPilot(), this) | ReferenceEquals(withBlock.Pilot(argIndex9), this))
        //                        {
        //                            // ユニット用特殊能力による付加
        //                            object argIndex2 = sname + "付加２";
        //                            if (withBlock.IsConditionSatisfied(argIndex2))
        //                            {
        //                                string localConditionData() { object argIndex1 = sname + "付加２"; var ret = withBlock.ConditionData(argIndex1); return ret; }

        //                                string arglist = localConditionData();
        //                                buf = GeneralLib.LIndex(arglist, 1);
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
        //                                object argIndex1 = sname + "付加２";
        //                                if (withBlock.ConditionLevel(argIndex1) != Constants.DEFAULT_LEVEL)
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
        //                            object argIndex4 = sname + "付加";
        //                            if (withBlock.IsConditionSatisfied(argIndex4))
        //                            {
        //                                string localConditionData1() { object argIndex1 = sname + "付加"; var ret = withBlock.ConditionData(argIndex1); return ret; }

        //                                string arglist1 = localConditionData1();
        //                                buf = GeneralLib.LIndex(arglist1, 1);
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
        //                                object argIndex3 = sname + "付加";
        //                                if (withBlock.ConditionLevel(argIndex3) != Constants.DEFAULT_LEVEL)
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
        //                            object argIndex6 = sname + "強化２";
        //                            if (withBlock.IsConditionSatisfied(argIndex6))
        //                            {
        //                                if (string.IsNullOrEmpty(SkillNameRet))
        //                                {
        //                                    // 強化される能力をパイロットが持っていなかった場合
        //                                    string localConditionData2() { object argIndex1 = sname + "強化２"; var ret = withBlock.ConditionData(argIndex1); return ret; }

        //                                    string arglist2 = localConditionData2();
        //                                    SkillNameRet = GeneralLib.LIndex(arglist2, 1);
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
        //                                    object argIndex5 = sname + "強化２";
        //                                    if (withBlock.ConditionLevel(argIndex5) >= 0d)
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
        //                            object argIndex8 = sname + "強化";
        //                            if (withBlock.IsConditionSatisfied(argIndex8))
        //                            {
        //                                if (string.IsNullOrEmpty(SkillNameRet))
        //                                {
        //                                    // 強化される能力をパイロットが持っていなかった場合
        //                                    string localConditionData3() { object argIndex1 = sname + "強化"; var ret = withBlock.ConditionData(argIndex1); return ret; }

        //                                    string arglist3 = localConditionData3();
        //                                    SkillNameRet = GeneralLib.LIndex(arglist3, 1);
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
        //                                    object argIndex7 = sname + "強化";
        //                                    if (withBlock.ConditionLevel(argIndex7) >= 0d)
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
        //                        string argoname2 = "防御力成長";
        //                        string argoname3 = "防御力レベルアップ";
        //                        if (Expression.IsOptionDefined(argoname2) | Expression.IsOptionDefined(argoname3))
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
        //                string argstr22 = ")";
        //                SkillNameRet = Strings.Left(SkillNameRet, GeneralLib.InStr2(SkillNameRet, argstr22) - 1);
        //            }

        //            if (string.IsNullOrEmpty(SkillNameRet))
        //            {
        //                SkillNameRet = sname;
        //            }

        //            return SkillNameRet;
        //        }

        //        // 特殊能力名称（レベル表示抜き）
        //        public string SkillName0(string Index)
        //        {
        //            string SkillName0Ret = default;
        //            SkillData sd;
        //            string sname;
        //            string buf;

        //            // パイロットが所有している特殊能力の中から検索
        //            int i;
        //            ;
        //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
        //            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 63575


        //            Input:

        //                    'パイロットが所有している特殊能力の中から検索
        //                    On Error GoTo ErrorHandler

        //             */
        //            sd = (SkillData)colSkill[Index];
        //            sname = sd.Name;

        //            // 能力強化系は非表示
        //            if (Strings.Right(sname, 2) == "ＵＰ" | Strings.Right(sname, 4) == "ＤＯＷＮ")
        //            {
        //                SkillName0Ret = "非表示";
        //                return SkillName0Ret;
        //            }

        //            switch (sname ?? "")
        //            {
        //                case "追加レベル":
        //                case "メッセージ":
        //                case "魔力所有":
        //                    {
        //                        // 非表示の能力
        //                        SkillName0Ret = "非表示";
        //                        return SkillName0Ret;
        //                    }

        //                case "得意技":
        //                case "不得手":
        //                    {
        //                        // 別名指定が存在しない能力
        //                        SkillName0Ret = sname;
        //                        return SkillName0Ret;
        //                    }
        //            }

        //            if (Strings.Len(sd.StrData) > 0)
        //            {
        //                SkillName0Ret = GeneralLib.LIndex(sd.StrData, 1);
        //                if (SkillName0Ret == "非表示")
        //                {
        //                    return SkillName0Ret;
        //                }
        //            }
        //            else
        //            {
        //                SkillName0Ret = sname;
        //            }

        //        ErrorHandler:
        //            ;
        //            if (string.IsNullOrEmpty(sname))
        //            {
        //                if (Information.IsNumeric(Index))
        //                {
        //                    return SkillName0Ret;
        //                }
        //                else
        //                {
        //                    // UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //                    sname = Conversions.ToString(Index);
        //                }
        //            }

        //            if (sname == "耐久")
        //            {
        //                string argoname = "防御力成長";
        //                string argoname1 = "防御力レベルアップ";
        //                if (Expression.IsOptionDefined(argoname) | Expression.IsOptionDefined(argoname1))
        //                {
        //                    // 防御力成長オプション使用時には耐久能力を非表示
        //                    SkillName0Ret = "非表示";
        //                    return SkillName0Ret;
        //                }
        //            }

        //            // 得意技＆不得手は名称変更されない
        //            switch (sname ?? "")
        //            {
        //                case "得意技":
        //                case "不得手":
        //                    {
        //                        SkillName0Ret = sname;
        //                        return SkillName0Ret;
        //                    }
        //            }

        //            // SetSkillコマンドで封印されている場合
        //            if (string.IsNullOrEmpty(SkillName0Ret))
        //            {
        //                string argvname = "Ability(" + ID + "," + sname + ")";
        //                if (Expression.IsGlobalVariableDefined(argvname))
        //                {
        //                    // オリジナルの名称を使用
        //                    SkillName0Ret = Data.SkillName(Level, sname);
        //                    if (Strings.InStr(SkillName0Ret, "非表示") > 0)
        //                    {
        //                        SkillName0Ret = "非表示";
        //                        return SkillName0Ret;
        //                    }
        //                }
        //            }

        //            // 重複可能な能力は特殊能力付加で名称が置き換えられことはない
        //            switch (sname ?? "")
        //            {
        //                case "ハンター":
        //                case "ＳＰ消費減少":
        //                case "スペシャルパワー自動発動":
        //                    {
        //                        if (Information.IsNumeric(Index))
        //                        {
        //                            return SkillName0Ret;
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
        //                        object argIndex5 = 1;
        //                        if (ReferenceEquals(withBlock.MainPilot(), this) | ReferenceEquals(withBlock.Pilot(argIndex5), this))
        //                        {
        //                            // ユニット用特殊能力による付加
        //                            object argIndex1 = sname + "付加２";
        //                            if (withBlock.IsConditionSatisfied(argIndex1))
        //                            {
        //                                string localConditionData() { object argIndex1 = sname + "付加２"; var ret = withBlock.ConditionData(argIndex1); return ret; }

        //                                string arglist = localConditionData();
        //                                buf = GeneralLib.LIndex(arglist, 1);
        //                                if (!string.IsNullOrEmpty(buf))
        //                                {
        //                                    SkillName0Ret = buf;
        //                                }
        //                                else if (string.IsNullOrEmpty(SkillName0Ret))
        //                                {
        //                                    SkillName0Ret = sname;
        //                                }

        //                                if (Strings.InStr(SkillName0Ret, "非表示") > 0)
        //                                {
        //                                    SkillName0Ret = "非表示";
        //                                    return SkillName0Ret;
        //                                }
        //                            }

        //                            // アビリティによる付加
        //                            object argIndex2 = sname + "付加";
        //                            if (withBlock.IsConditionSatisfied(argIndex2))
        //                            {
        //                                string localConditionData1() { object argIndex1 = sname + "付加"; var ret = withBlock.ConditionData(argIndex1); return ret; }

        //                                string arglist1 = localConditionData1();
        //                                buf = GeneralLib.LIndex(arglist1, 1);
        //                                if (!string.IsNullOrEmpty(buf))
        //                                {
        //                                    SkillName0Ret = buf;
        //                                }
        //                                else if (string.IsNullOrEmpty(SkillName0Ret))
        //                                {
        //                                    SkillName0Ret = sname;
        //                                }

        //                                if (Strings.InStr(SkillName0Ret, "非表示") > 0)
        //                                {
        //                                    SkillName0Ret = "非表示";
        //                                    return SkillName0Ret;
        //                                }
        //                            }

        //                            // ユニット用特殊能力による強化
        //                            if (string.IsNullOrEmpty(SkillName0Ret))
        //                            {
        //                                object argIndex3 = sname + "強化２";
        //                                if (withBlock.IsConditionSatisfied(argIndex3))
        //                                {
        //                                    string localConditionData2() { object argIndex1 = sname + "強化２"; var ret = withBlock.ConditionData(argIndex1); return ret; }

        //                                    string arglist2 = localConditionData2();
        //                                    SkillName0Ret = GeneralLib.LIndex(arglist2, 1);
        //                                    if (string.IsNullOrEmpty(SkillName0Ret))
        //                                    {
        //                                        SkillName0Ret = sname;
        //                                    }

        //                                    if (Strings.InStr(SkillName0Ret, "非表示") > 0)
        //                                    {
        //                                        SkillName0Ret = "非表示";
        //                                        return SkillName0Ret;
        //                                    }
        //                                }
        //                            }

        //                            // アビリティによる強化
        //                            if (string.IsNullOrEmpty(SkillName0Ret))
        //                            {
        //                                object argIndex4 = sname + "強化";
        //                                if (withBlock.IsConditionSatisfied(argIndex4))
        //                                {
        //                                    string localConditionData3() { object argIndex1 = sname + "強化"; var ret = withBlock.ConditionData(argIndex1); return ret; }

        //                                    string arglist3 = localConditionData3();
        //                                    SkillName0Ret = GeneralLib.LIndex(arglist3, 1);
        //                                    if (string.IsNullOrEmpty(SkillName0Ret))
        //                                    {
        //                                        SkillName0Ret = sname;
        //                                    }

        //                                    if (Strings.InStr(SkillName0Ret, "非表示") > 0)
        //                                    {
        //                                        SkillName0Ret = "非表示";
        //                                        return SkillName0Ret;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            // 該当するものが無ければエリアスから検索
        //            if (string.IsNullOrEmpty(SkillName0Ret))
        //            {
        //                {
        //                    var withBlock1 = SRC.ALDList;
        //                    var loopTo = withBlock1.Count();
        //                    for (i = 1; i <= loopTo; i++)
        //                    {
        //                        object argIndex6 = i;
        //                        {
        //                            var withBlock2 = withBlock1.Item(argIndex6);
        //                            if ((withBlock2.get_AliasType(1) ?? "") == (sname ?? ""))
        //                            {
        //                                SkillName0Ret = withBlock2.Name;
        //                                return SkillName0Ret;
        //                            }
        //                        }
        //                    }
        //                }

        //                SkillName0Ret = sname;
        //            }

        //            // 能力強化系は非表示
        //            if (Strings.Right(sname, 2) == "ＵＰ" | Strings.Right(sname, 4) == "ＤＯＷＮ")
        //            {
        //                SkillName0Ret = "非表示";
        //                return SkillName0Ret;
        //            }

        //            switch (sname ?? "")
        //            {
        //                case "追加レベル":
        //                case "メッセージ":
        //                case "魔力所有":
        //                    {
        //                        // 非表示の能力
        //                        SkillName0Ret = "非表示";
        //                        return SkillName0Ret;
        //                    }

        //                case "耐久":
        //                    {
        //                        string argoname2 = "防御力成長";
        //                        string argoname3 = "防御力レベルアップ";
        //                        if (Expression.IsOptionDefined(argoname2) | Expression.IsOptionDefined(argoname3))
        //                        {
        //                            // 防御力成長オプション使用時には耐久能力を非表示
        //                            SkillName0Ret = "非表示";
        //                            return SkillName0Ret;
        //                        }

        //                        break;
        //                    }
        //            }

        //            // レベル非表示用の括弧を削除
        //            if (Strings.Left(SkillName0Ret, 1) == "(")
        //            {
        //                SkillName0Ret = Strings.Mid(SkillName0Ret, 2);
        //                string argstr2 = ")";
        //                SkillName0Ret = Strings.Left(SkillName0Ret, GeneralLib.InStr2(SkillName0Ret, argstr2) - 1);
        //            }

        //            // レベル指定を削除
        //            i = Strings.InStr(SkillName0Ret, "Lv");
        //            if (i > 0)
        //            {
        //                SkillName0Ret = Strings.Left(SkillName0Ret, i - 1);
        //            }

        //            return SkillName0Ret;
        //        }

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
        //                string argvname = "Ability(" + ID + "," + stype + ")";
        //                if (Expression.IsGlobalVariableDefined(argvname))
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
        //                        object argIndex5 = 1;
        //                        if (ReferenceEquals(this, withBlock.MainPilot()) | ReferenceEquals(this, withBlock.Pilot(argIndex5)))
        //                        {
        //                            // ユニット用特殊能力による付加
        //                            object argIndex1 = stype + "付加２";
        //                            if (withBlock.IsConditionSatisfied(argIndex1))
        //                            {
        //                                string localConditionData() { object argIndex1 = (object)(stype + "付加２"); var ret = withBlock.ConditionData(argIndex1); return ret; }

        //                                string arglist = localConditionData();
        //                                buf = GeneralLib.LIndex(arglist, 1);
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
        //                            object argIndex2 = stype + "付加";
        //                            if (withBlock.IsConditionSatisfied(argIndex2))
        //                            {
        //                                string localConditionData1() { object argIndex1 = (object)(stype + "付加"); var ret = withBlock.ConditionData(argIndex1); return ret; }

        //                                string arglist1 = localConditionData1();
        //                                buf = GeneralLib.LIndex(arglist1, 1);
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
        //                                object argIndex3 = stype + "強化２";
        //                                if (withBlock.IsConditionSatisfied(argIndex3))
        //                                {
        //                                    string localConditionData2() { object argIndex1 = (object)(stype + "強化２"); var ret = withBlock.ConditionData(argIndex1); return ret; }

        //                                    string arglist2 = localConditionData2();
        //                                    SkillNameForNSRet = GeneralLib.LIndex(arglist2, 1);
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
        //                                object argIndex4 = stype + "強化";
        //                                if (withBlock.IsConditionSatisfied(argIndex4))
        //                                {
        //                                    string localConditionData3() { object argIndex1 = (object)(stype + "強化"); var ret = withBlock.ConditionData(argIndex1); return ret; }

        //                                    string arglist3 = localConditionData3();
        //                                    SkillNameForNSRet = GeneralLib.LIndex(arglist3, 1);
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
        //                        object argIndex6 = i;
        //                        {
        //                            var withBlock2 = withBlock1.Item(argIndex6);
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
        //                string argstr2 = ")";
        //                SkillNameForNSRet = Strings.Left(SkillNameForNSRet, GeneralLib.InStr2(SkillNameForNSRet, argstr2) - 1);
        //            }

        //            // レベル表示を削除
        //            i = Strings.InStr(SkillNameForNSRet, "Lv");
        //            if (i > 0)
        //            {
        //                SkillNameForNSRet = Strings.Left(SkillNameForNSRet, i - 1);
        //            }

        //            return SkillNameForNSRet;
        //        }

        //        // 特殊能力の種類
        //        public string SkillType(string sname)
        //        {
        //            string SkillTypeRet = default;
        //            int i;
        //            string sname0, sname2;
        //            if (string.IsNullOrEmpty(sname))
        //            {
        //                return SkillTypeRet;
        //            }

        //            i = Strings.InStr(sname, "Lv");
        //            if (i > 0)
        //            {
        //                sname0 = Strings.Left(sname, i - 1);
        //            }
        //            else
        //            {
        //                sname0 = sname;
        //            }

        //            // エリアスデータが定義されている？
        //            object argIndex2 = sname0;
        //            if (SRC.ALDList.IsDefined(argIndex2))
        //            {
        //                object argIndex1 = sname0;
        //                {
        //                    var withBlock = SRC.ALDList.Item(argIndex1);
        //                    SkillTypeRet = withBlock.get_AliasType(1);
        //                    return SkillTypeRet;
        //                }
        //            }

        //            // 特殊能力一覧から検索
        //            foreach (SkillData sd in colSkill)
        //            {
        //                if ((sname0 ?? "") == (sd.Name ?? ""))
        //                {
        //                    SkillTypeRet = sd.Name;
        //                    return SkillTypeRet;
        //                }

        //                sname2 = GeneralLib.LIndex(sd.StrData, 1);
        //                if ((sname0 ?? "") == (sname2 ?? ""))
        //                {
        //                    SkillTypeRet = sd.Name;
        //                    return SkillTypeRet;
        //                }

        //                if (Strings.Left(sname2, 1) == "(")
        //                {
        //                    if (Strings.Right(sname2, 1) == ")")
        //                    {
        //                        sname2 = Strings.Mid(sname2, 2, Strings.Len(sname2) - 2);
        //                        if ((sname ?? "") == (sname2 ?? ""))
        //                        {
        //                            SkillTypeRet = sd.Name;
        //                            return SkillTypeRet;
        //                        }
        //                    }
        //                }
        //            }

        //            // その能力を修得していない
        //            SkillTypeRet = sname0;

        //            // 特殊能力付加による修正
        //            if (Unit is object)
        //            {
        //                {
        //                    var withBlock1 = Unit;
        //                    if (Conversions.ToBoolean(withBlock1.CountCondition() & Conversions.Toint(withBlock1.CountPilot() > 0)))
        //                    {
        //                        object argIndex5 = 1;
        //                        if (ReferenceEquals(this, withBlock1.MainPilot()) | ReferenceEquals(this, withBlock1.Pilot(argIndex5)))
        //                        {
        //                            var loopTo = withBlock1.CountCondition();
        //                            for (i = 1; i <= loopTo; i++)
        //                            {
        //                                string localCondition() { object argIndex1 = i; var ret = withBlock1.Condition(argIndex1); return ret; }

        //                                string localCondition1() { object argIndex1 = i; var ret = withBlock1.Condition(argIndex1); return ret; }

        //                                if (Strings.Right(localCondition(), 2) == "付加")
        //                                {
        //                                    string localConditionData() { object argIndex1 = i; var ret = withBlock1.ConditionData(argIndex1); return ret; }

        //                                    string arglist = localConditionData();
        //                                    if ((GeneralLib.LIndex(arglist, 1) ?? "") == (sname0 ?? ""))
        //                                    {
        //                                        object argIndex3 = i;
        //                                        SkillTypeRet = withBlock1.Condition(argIndex3);
        //                                        SkillTypeRet = Strings.Left(SkillTypeRet, Strings.Len(SkillTypeRet) - 2);
        //                                        break;
        //                                    }
        //                                }
        //                                else if (Strings.Right(localCondition1(), 3) == "付加２")
        //                                {
        //                                    string localConditionData1() { object argIndex1 = i; var ret = withBlock1.ConditionData(argIndex1); return ret; }

        //                                    string arglist1 = localConditionData1();
        //                                    if ((GeneralLib.LIndex(arglist1, 1) ?? "") == (sname0 ?? ""))
        //                                    {
        //                                        object argIndex4 = i;
        //                                        SkillTypeRet = withBlock1.Condition(argIndex4);
        //                                        SkillTypeRet = Strings.Left(SkillTypeRet, Strings.Len(SkillTypeRet) - 3);
        //                                        break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            return SkillTypeRet;
        //        }
    }
}
