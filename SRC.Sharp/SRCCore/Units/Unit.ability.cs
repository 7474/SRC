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
//        // アビリティ
//        public AbilityData Ability(short a)
//        {
//            AbilityData AbilityRet = default;
//            AbilityRet = adata[a];
//            return AbilityRet;
//        }

//        // アビリティ総数
//        public short CountAbility()
//        {
//            short CountAbilityRet = default;
//            CountAbilityRet = (short)Information.UBound(adata);
//            return CountAbilityRet;
//        }

//        // アビリティの愛称
//        public string AbilityNickname(short a)
//        {
//            string AbilityNicknameRet = default;
//            Unit u;

//            // 愛称内の式置換のため、デフォルトユニットを一時的に変更する
//            u = Event_Renamed.SelectedUnitForEvent;
//            Event_Renamed.SelectedUnitForEvent = this;
//            AbilityNicknameRet = adata[a].Nickname();
//            Event_Renamed.SelectedUnitForEvent = u;
//            return AbilityNicknameRet;
//        }

//        // アビリティ a の最小射程
//        public short AbilityMinRange(short a)
//        {
//            short AbilityMinRangeRet = default;
//            AbilityMinRangeRet = Ability(a).MinRange;
//            string argattr1 = "小";
//            if (IsAbilityClassifiedAs(a, ref argattr1))
//            {
//                string argattr = "小";
//                AbilityMinRangeRet = (short)GeneralLib.MinLng((int)(AbilityMinRangeRet + AbilityLevel(a, ref argattr)), Ability(a).MaxRange);
//            }

//            return AbilityMinRangeRet;
//        }

//        // アビリティ a の最大射程
//        public short AbilityMaxRange(short a)
//        {
//            short AbilityMaxRangeRet = default;
//            AbilityMaxRangeRet = Ability(a).MaxRange;
//            return AbilityMaxRangeRet;
//        }

//        // アビリティ a の消費ＥＮ
//        public short AbilityENConsumption(short a)
//        {
//            short AbilityENConsumptionRet = default;
//            // UPGRADE_NOTE: rate は rate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
//            double rate_Renamed;
//            Pilot p;
//            short i;
//            {
//                var withBlock = Ability(a);
//                AbilityENConsumptionRet = withBlock.ENConsumption;

//                // パイロットの能力によって術及び技の消費ＥＮは減少する
//                if (CountPilot() > 0)
//                {
//                    p = MainPilot();

//                    // 術に該当するか？
//                    if (IsSpellAbility(a))
//                    {
//                        // 術に該当する場合は術技能によってＥＮ消費量を変える
//                        object argIndex1 = "術";
//                        string argref_mode = "";
//                        switch (p.SkillLevel(ref argIndex1, ref_mode: ref argref_mode))
//                        {
//                            case 1d:
//                                {
//                                    break;
//                                }

//                            case 2d:
//                                {
//                                    AbilityENConsumptionRet = (short)(0.9d * AbilityENConsumptionRet);
//                                    break;
//                                }

//                            case 3d:
//                                {
//                                    AbilityENConsumptionRet = (short)(0.8d * AbilityENConsumptionRet);
//                                    break;
//                                }

//                            case 4d:
//                                {
//                                    AbilityENConsumptionRet = (short)(0.7d * AbilityENConsumptionRet);
//                                    break;
//                                }

//                            case 5d:
//                                {
//                                    AbilityENConsumptionRet = (short)(0.6d * AbilityENConsumptionRet);
//                                    break;
//                                }

//                            case 6d:
//                                {
//                                    AbilityENConsumptionRet = (short)(0.5d * AbilityENConsumptionRet);
//                                    break;
//                                }

//                            case 7d:
//                                {
//                                    AbilityENConsumptionRet = (short)(0.45d * AbilityENConsumptionRet);
//                                    break;
//                                }

//                            case 8d:
//                                {
//                                    AbilityENConsumptionRet = (short)(0.4d * AbilityENConsumptionRet);
//                                    break;
//                                }

//                            case 9d:
//                                {
//                                    AbilityENConsumptionRet = (short)(0.35d * AbilityENConsumptionRet);
//                                    break;
//                                }

//                            case var @case when @case >= 10d:
//                                {
//                                    AbilityENConsumptionRet = (short)(0.3d * AbilityENConsumptionRet);
//                                    break;
//                                }
//                        }

//                        AbilityENConsumptionRet = (short)GeneralLib.MinLng(GeneralLib.MaxLng(AbilityENConsumptionRet, 5), withBlock.ENConsumption);
//                    }

//                    // 技に該当するか？
//                    if (IsFeatAbility(a))
//                    {
//                        // 技に該当する場合は技技能によってＥＮ消費量を変える
//                        object argIndex2 = "技";
//                        string argref_mode1 = "";
//                        switch (p.SkillLevel(ref argIndex2, ref_mode: ref argref_mode1))
//                        {
//                            case 1d:
//                                {
//                                    break;
//                                }

//                            case 2d:
//                                {
//                                    AbilityENConsumptionRet = (short)(0.9d * AbilityENConsumptionRet);
//                                    break;
//                                }

//                            case 3d:
//                                {
//                                    AbilityENConsumptionRet = (short)(0.8d * AbilityENConsumptionRet);
//                                    break;
//                                }

//                            case 4d:
//                                {
//                                    AbilityENConsumptionRet = (short)(0.7d * AbilityENConsumptionRet);
//                                    break;
//                                }

//                            case 5d:
//                                {
//                                    AbilityENConsumptionRet = (short)(0.6d * AbilityENConsumptionRet);
//                                    break;
//                                }

//                            case 6d:
//                                {
//                                    AbilityENConsumptionRet = (short)(0.5d * AbilityENConsumptionRet);
//                                    break;
//                                }

//                            case 7d:
//                                {
//                                    AbilityENConsumptionRet = (short)(0.45d * AbilityENConsumptionRet);
//                                    break;
//                                }

//                            case 8d:
//                                {
//                                    AbilityENConsumptionRet = (short)(0.4d * AbilityENConsumptionRet);
//                                    break;
//                                }

//                            case 9d:
//                                {
//                                    AbilityENConsumptionRet = (short)(0.35d * AbilityENConsumptionRet);
//                                    break;
//                                }

//                            case var case1 when case1 >= 10d:
//                                {
//                                    AbilityENConsumptionRet = (short)(0.3d * AbilityENConsumptionRet);
//                                    break;
//                                }
//                        }

//                        AbilityENConsumptionRet = (short)GeneralLib.MinLng(GeneralLib.MaxLng(AbilityENConsumptionRet, 5), withBlock.ENConsumption);
//                    }
//                }

//                // ＥＮ消費減少能力による修正
//                rate_Renamed = 1d;
//                string argfname = "ＥＮ消費減少";
//                if (IsFeatureAvailable(ref argfname))
//                {
//                    var loopTo = CountFeature();
//                    for (i = 1; i <= loopTo; i++)
//                    {
//                        object argIndex3 = i;
//                        if (Feature(ref argIndex3) == "ＥＮ消費減少")
//                        {
//                            double localFeatureLevel() { object argIndex1 = i; var ret = FeatureLevel(ref argIndex1); return ret; }

//                            rate_Renamed = rate_Renamed - 0.1d * localFeatureLevel();
//                        }
//                    }
//                }

//                if (rate_Renamed < 0.1d)
//                {
//                    rate_Renamed = 0.1d;
//                }

//                AbilityENConsumptionRet = (short)(rate_Renamed * AbilityENConsumptionRet);
//            }

//            return AbilityENConsumptionRet;
//        }

//        // アビリティ a が属性 attr を持つかどうか
//        public bool IsAbilityClassifiedAs(short a, ref string attr)
//        {
//            bool IsAbilityClassifiedAsRet = default;
//            if (GeneralLib.InStrNotNest(ref Ability(a).Class_Renamed, ref attr) > 0)
//            {
//                IsAbilityClassifiedAsRet = true;
//            }
//            else
//            {
//                IsAbilityClassifiedAsRet = false;
//            }

//            return IsAbilityClassifiedAsRet;
//        }

//        // アビリティ a の属性 atrr のレベル
//        public double AbilityLevel(short a, ref string attr)
//        {
//            double AbilityLevelRet = default;
//            string attrlv, aclass;
//            short start_idx, i;
//            string c;
//            ;
//#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
//            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 535884


//            Input:

//                    On Error GoTo ErrorHandler

//             */
//            attrlv = attr + "L";

//            // アビリティ属性を調べてみる
//            aclass = Ability(a).Class_Renamed;

//            // レベル指定があるか？
//            start_idx = (short)Strings.InStr(aclass, attrlv);
//            if (start_idx == 0)
//            {
//                return AbilityLevelRet;
//            }

//            // レベル指定部分の切り出し
//            start_idx = (short)(start_idx + Strings.Len(attrlv));
//            i = start_idx;
//            while (true)
//            {
//                c = Strings.Mid(aclass, i, 1);
//                if (string.IsNullOrEmpty(c))
//                {
//                    break;
//                }

//                switch (Strings.Asc(c))
//                {
//                    case var @case when 45 <= @case && @case <= 46:
//                    case var case1 when 48 <= case1 && case1 <= 57: // "-", ".", 0-9
//                        {
//                            break;
//                        }

//                    default:
//                        {
//                            break;
//                        }
//                }

//                i = (short)(i + 1);
//            }

//            AbilityLevelRet = Conversions.ToDouble(Strings.Mid(aclass, start_idx, i - start_idx));
//            return AbilityLevelRet;
//        ErrorHandler:
//            ;
//            string argmsg = Name + "の" + "アビリティ「" + Ability(a).Name + "」の" + "属性「" + attr + "」のレベル指定が不正です";
//            GUI.ErrorMessage(ref argmsg);
//        }

//        // アビリティ a が術かどうか
//        public bool IsSpellAbility(short a)
//        {
//            bool IsSpellAbilityRet = default;
//            short i;
//            string nskill;
//            string argattr = "術";
//            if (IsAbilityClassifiedAs(a, ref argattr))
//            {
//                IsSpellAbilityRet = true;
//                return IsSpellAbilityRet;
//            }

//            {
//                var withBlock = MainPilot();
//                var loopTo = GeneralLib.LLength(ref Ability(a).NecessarySkill);
//                for (i = 1; i <= loopTo; i++)
//                {
//                    nskill = GeneralLib.LIndex(ref Ability(a).NecessarySkill, i);
//                    if (Strings.InStr(nskill, "Lv") > 0)
//                    {
//                        nskill = Strings.Left(nskill, Strings.InStr(nskill, "Lv") - 1);
//                    }

//                    if (withBlock.SkillType(ref nskill) == "術")
//                    {
//                        IsSpellAbilityRet = true;
//                        return IsSpellAbilityRet;
//                    }
//                }
//            }

//            return IsSpellAbilityRet;
//        }

//        // アビリティ a が技かどうか
//        public bool IsFeatAbility(short a)
//        {
//            bool IsFeatAbilityRet = default;
//            short i;
//            string nskill;
//            string argattr = "技";
//            if (IsAbilityClassifiedAs(a, ref argattr))
//            {
//                IsFeatAbilityRet = true;
//                return IsFeatAbilityRet;
//            }

//            {
//                var withBlock = MainPilot();
//                var loopTo = GeneralLib.LLength(ref Ability(a).NecessarySkill);
//                for (i = 1; i <= loopTo; i++)
//                {
//                    nskill = GeneralLib.LIndex(ref Ability(a).NecessarySkill, i);
//                    if (Strings.InStr(nskill, "Lv") > 0)
//                    {
//                        nskill = Strings.Left(nskill, Strings.InStr(nskill, "Lv") - 1);
//                    }

//                    if (withBlock.SkillType(ref nskill) == "技")
//                    {
//                        IsFeatAbilityRet = true;
//                        return IsFeatAbilityRet;
//                    }
//                }
//            }

//            return IsFeatAbilityRet;
//        }

//        // アビリティ a が使用可能かどうか
//        // ref_mode はユニットの状態（移動前、移動後）を示す
//        public bool IsAbilityAvailable(short a, ref string ref_mode)
//        {
//            bool IsAbilityAvailableRet = default;
//            short j, i, k;
//            AbilityData ad;
//            string uname, pname;
//            Unit u;
//            IsAbilityAvailableRet = false;
//            ad = Ability(a);

//            // イベントコマンド「Disable」
//            if (IsDisabled(ref ad.Name))
//            {
//                return IsAbilityAvailableRet;
//            }

//            // パイロットが乗っていなければ常に使用可能と判定
//            if (CountPilot() == 0)
//            {
//                IsAbilityAvailableRet = true;
//                return IsAbilityAvailableRet;
//            }

//            // 必要技能
//            if (!IsAbilityMastered(a))
//            {
//                return IsAbilityAvailableRet;
//            }

//            // 必要条件
//            if (!IsAbilityEnabled(a))
//            {
//                return IsAbilityAvailableRet;
//            }

//            // ステータス表示では必要技能だけ満たしていればＯＫ
//            if (ref_mode == "インターミッション" | string.IsNullOrEmpty(ref_mode))
//            {
//                IsAbilityAvailableRet = true;
//                return IsAbilityAvailableRet;
//            }

//            {
//                var withBlock = MainPilot();
//                // 必要気力
//                if (ad.NecessaryMorale > 0)
//                {
//                    if (withBlock.Morale < ad.NecessaryMorale)
//                    {
//                        return IsAbilityAvailableRet;
//                    }
//                }

//                // 霊力消費アビリティ
//                string argattr2 = "霊";
//                string argattr3 = "プ";
//                if (IsAbilityClassifiedAs(a, ref argattr2))
//                {
//                    string argattr = "霊";
//                    if (withBlock.Plana < AbilityLevel(a, ref argattr) * 5d)
//                    {
//                        return IsAbilityAvailableRet;
//                    }
//                }
//                else if (IsAbilityClassifiedAs(a, ref argattr3))
//                {
//                    string argattr1 = "プ";
//                    if (withBlock.Plana < AbilityLevel(a, ref argattr1) * 5d)
//                    {
//                        return IsAbilityAvailableRet;
//                    }
//                }
//            }

//            // 属性使用不能状態
//            object argIndex1 = "オーラ使用不能";
//            if (ConditionLifetime(ref argIndex1) > 0)
//            {
//                string argattr4 = "オ";
//                if (IsAbilityClassifiedAs(a, ref argattr4))
//                {
//                    return IsAbilityAvailableRet;
//                }
//            }

//            object argIndex2 = "超能力使用不能";
//            if (ConditionLifetime(ref argIndex2) > 0)
//            {
//                string argattr5 = "超";
//                if (IsAbilityClassifiedAs(a, ref argattr5))
//                {
//                    return IsAbilityAvailableRet;
//                }
//            }

//            object argIndex3 = "同調率使用不能";
//            if (ConditionLifetime(ref argIndex3) > 0)
//            {
//                string argattr6 = "シ";
//                if (IsAbilityClassifiedAs(a, ref argattr6))
//                {
//                    return IsAbilityAvailableRet;
//                }
//            }

//            object argIndex4 = "超感覚使用不能";
//            if (ConditionLifetime(ref argIndex4) > 0)
//            {
//                string argattr7 = "サ";
//                if (IsAbilityClassifiedAs(a, ref argattr7))
//                {
//                    return IsAbilityAvailableRet;
//                }
//            }

//            object argIndex5 = "知覚強化使用不能";
//            if (ConditionLifetime(ref argIndex5) > 0)
//            {
//                string argattr8 = "サ";
//                if (IsAbilityClassifiedAs(a, ref argattr8))
//                {
//                    return IsAbilityAvailableRet;
//                }
//            }

//            object argIndex6 = "霊力使用不能";
//            if (ConditionLifetime(ref argIndex6) > 0)
//            {
//                string argattr9 = "霊";
//                if (IsAbilityClassifiedAs(a, ref argattr9))
//                {
//                    return IsAbilityAvailableRet;
//                }
//            }

//            object argIndex7 = "術使用不能";
//            if (ConditionLifetime(ref argIndex7) > 0)
//            {
//                string argattr10 = "術";
//                if (IsAbilityClassifiedAs(a, ref argattr10))
//                {
//                    return IsAbilityAvailableRet;
//                }
//            }

//            object argIndex8 = "技使用不能";
//            if (ConditionLifetime(ref argIndex8) > 0)
//            {
//                string argattr11 = "技";
//                if (IsAbilityClassifiedAs(a, ref argattr11))
//                {
//                    return IsAbilityAvailableRet;
//                }
//            }

//            var loopTo = CountCondition();
//            for (i = 1; i <= loopTo; i++)
//            {
//                string localCondition3() { object argIndex1 = i; var ret = Condition(ref argIndex1); return ret; }

//                if (Strings.Len(localCondition3()) > 6)
//                {
//                    string localCondition2() { object argIndex1 = i; var ret = Condition(ref argIndex1); return ret; }

//                    if (Strings.Right(localCondition2(), 6) == "属性使用不能")
//                    {
//                        string localCondition() { object argIndex1 = i; var ret = Condition(ref argIndex1); return ret; }

//                        string localCondition1() { object argIndex1 = i; var ret = Condition(ref argIndex1); return ret; }

//                        string argstring2 = Strings.Left(localCondition(), Strings.Len(localCondition1()) - 6);
//                        if (GeneralLib.InStrNotNest(ref Ability(a).Class_Renamed, ref argstring2) > 0)
//                        {
//                            return IsAbilityAvailableRet;
//                        }
//                    }
//                }
//            }

//            // 弾数が足りるか
//            if (MaxStock(a) > 0)
//            {
//                if (Stock(a) < 1)
//                {
//                    return IsAbilityAvailableRet;
//                }
//            }

//            // ＥＮが足りるか
//            if (ad.ENConsumption > 0)
//            {
//                if (EN < AbilityENConsumption(a))
//                {
//                    return IsAbilityAvailableRet;
//                }
//            }

//            // お金が足りるか……
//            if (Party == "味方")
//            {
//                string argattr13 = "銭";
//                if (IsAbilityClassifiedAs(a, ref argattr13))
//                {
//                    string argattr12 = "銭";
//                    if (SRC.Money < GeneralLib.MaxLng((int)AbilityLevel(a, ref argattr12), 1) * Value / 10)
//                    {
//                        return IsAbilityAvailableRet;
//                    }
//                }
//            }

//            // 移動不能時には移動型マップアビリティは使用不能
//            object argIndex9 = "移動不能";
//            if (IsConditionSatisfied(ref argIndex9))
//            {
//                string argattr14 = "Ｍ移";
//                if (IsAbilityClassifiedAs(a, ref argattr14))
//                {
//                    return IsAbilityAvailableRet;
//                }
//            }

//            // 術及び音声技は沈黙状態では使用不能
//            object argIndex10 = "沈黙";
//            if (IsConditionSatisfied(ref argIndex10))
//            {
//                {
//                    var withBlock1 = MainPilot();
//                    string argattr15 = "音";
//                    if (IsSpellAbility(a) | IsAbilityClassifiedAs(a, ref argattr15))
//                    {
//                        return IsAbilityAvailableRet;
//                    }
//                }
//            }

//            // 術は狂戦士状態では使用不能
//            object argIndex11 = "狂戦士";
//            if (IsConditionSatisfied(ref argIndex11))
//            {
//                {
//                    var withBlock2 = MainPilot();
//                    if (IsSpellAbility(a))
//                    {
//                        return IsAbilityAvailableRet;
//                    }
//                }
//            }

//            // 合体技の処理
//            string argattr16 = "合";
//            if (IsAbilityClassifiedAs(a, ref argattr16))
//            {
//                if (!IsCombinationAbilityAvailable(a))
//                {
//                    return IsAbilityAvailableRet;
//                }
//            }

//            // この地形で変形できるか？
//            string argattr17 = "変";
//            if (IsAbilityClassifiedAs(a, ref argattr17))
//            {
//                string argfname = "変形技";
//                string argfname1 = "ノーマルモード";
//                if (IsFeatureAvailable(ref argfname))
//                {
//                    var loopTo1 = CountFeature();
//                    for (i = 1; i <= loopTo1; i++)
//                    {
//                        string localFeature() { object argIndex1 = i; var ret = Feature(ref argIndex1); return ret; }

//                        string localFeatureData1() { object argIndex1 = i; var ret = FeatureData(ref argIndex1); return ret; }

//                        string localLIndex1() { string arglist = hsc19c10c9cae54732ac7c9c2e90257bd2(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

//                        if (localFeature() == "変形技" & (localLIndex1() ?? "") == (ad.Name ?? ""))
//                        {
//                            string localFeatureData() { object argIndex1 = i; var ret = FeatureData(ref argIndex1); return ret; }

//                            string localLIndex() { string arglist = hs999a427db36f427e9868be6a72c4f4c0(); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

//                            Unit localOtherForm() { object argIndex1 = (object)hsb208b29f50af4a41be714b7083a85c98(); var ret = OtherForm(ref argIndex1); return ret; }

//                            if (!localOtherForm().IsAbleToEnter(x, y))
//                            {
//                                return IsAbilityAvailableRet;
//                            }
//                        }
//                    }
//                }
//                else if (IsFeatureAvailable(ref argfname1))
//                {
//                    string localLIndex2() { object argIndex1 = "ノーマルモード"; string arglist = FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

//                    Unit localOtherForm1() { object argIndex1 = (object)hs89e8cc23250142a2b15ab7b087ddcbd2(); var ret = OtherForm(ref argIndex1); return ret; }

//                    if (!localOtherForm1().IsAbleToEnter(x, y))
//                    {
//                        return IsAbilityAvailableRet;
//                    }
//                }

//                object argIndex12 = "形態固定";
//                if (IsConditionSatisfied(ref argIndex12))
//                {
//                    return IsAbilityAvailableRet;
//                }

//                object argIndex13 = "機体固定";
//                if (IsConditionSatisfied(ref argIndex13))
//                {
//                    return IsAbilityAvailableRet;
//                }
//            }

//            // 瀕死時限定
//            string argattr18 = "瀕";
//            if (IsAbilityClassifiedAs(a, ref argattr18))
//            {
//                if (HP > MaxHP / 4)
//                {
//                    return IsAbilityAvailableRet;
//                }
//            }

//            // 自動チャージアビリティを充填中
//            object argIndex14 = AbilityNickname(a) + "充填中";
//            if (IsConditionSatisfied(ref argIndex14))
//            {
//                return IsAbilityAvailableRet;
//            }
//            // 共有武器＆アビリティが充填中の場合も使用不可
//            short lv;
//            string argattr24 = "共";
//            if (IsAbilityClassifiedAs(a, ref argattr24))
//            {
//                string argattr19 = "共";
//                lv = (short)AbilityLevel(a, ref argattr19);
//                var loopTo2 = CountAbility();
//                for (i = 1; i <= loopTo2; i++)
//                {
//                    string argattr21 = "共";
//                    if (IsAbilityClassifiedAs(i, ref argattr21))
//                    {
//                        string argattr20 = "共";
//                        if (lv == AbilityLevel(i, ref argattr20))
//                        {
//                            object argIndex15 = AbilityNickname(i) + "充填中";
//                            if (IsConditionSatisfied(ref argIndex15))
//                            {
//                                return IsAbilityAvailableRet;
//                            }
//                        }
//                    }
//                }

//                var loopTo3 = CountAbility();
//                for (i = 1; i <= loopTo3; i++)
//                {
//                    string argattr23 = "共";
//                    if (IsAbilityClassifiedAs(i, ref argattr23))
//                    {
//                        string argattr22 = "共";
//                        if (lv == AbilityLevel(i, ref argattr22))
//                        {
//                            object argIndex16 = AbilityNickname(i) + "充填中";
//                            if (IsConditionSatisfied(ref argIndex16))
//                            {
//                                return IsAbilityAvailableRet;
//                            }
//                        }
//                    }
//                }
//            }

//            // 使用禁止
//            string argattr25 = "禁";
//            if (Conversions.ToInteger(IsAbilityClassifiedAs(a, ref argattr25)) > 0)
//            {
//                return IsAbilityAvailableRet;
//            }

//            // チャージ判定であればここまででＯＫ
//            if (ref_mode == "チャージ")
//            {
//                IsAbilityAvailableRet = true;
//                return IsAbilityAvailableRet;
//            }

//            // チャージ式アビリティ
//            string argattr26 = "Ｃ";
//            if (IsAbilityClassifiedAs(a, ref argattr26))
//            {
//                object argIndex17 = "チャージ完了";
//                if (!IsConditionSatisfied(ref argIndex17))
//                {
//                    return IsAbilityAvailableRet;
//                }
//            }

//            var loopTo4 = ad.CountEffect();
//            for (i = 1; i <= loopTo4; i++)
//            {
//                object argIndex22 = i;
//                if (ad.EffectType(ref argIndex22) == "召喚")
//                {
//                    // 召喚は既に召喚を行っている場合には不可能
//                    var loopTo5 = CountServant();
//                    for (j = 1; j <= loopTo5; j++)
//                    {
//                        Unit localServant() { object argIndex1 = j; var ret = Servant(ref argIndex1); return ret; }

//                        {
//                            var withBlock3 = localServant().CurrentForm();
//                            switch (withBlock3.Status_Renamed ?? "")
//                            {
//                                case "出撃":
//                                case "格納":
//                                    {
//                                        // 使用不可
//                                        return IsAbilityAvailableRet;
//                                    }

//                                case "旧主形態":
//                                case "旧形態":
//                                    {
//                                        // 合体後の形態が出撃中なら使用不可
//                                        var loopTo6 = withBlock3.CountFeature();
//                                        for (k = 1; k <= loopTo6; k++)
//                                        {
//                                            object argIndex19 = k;
//                                            if (withBlock3.Feature(ref argIndex19) == "合体")
//                                            {
//                                                string localFeatureData2() { object argIndex1 = k; var ret = withBlock3.FeatureData(ref argIndex1); return ret; }

//                                                string arglist = localFeatureData2();
//                                                uname = GeneralLib.LIndex(ref arglist, 2);
//                                                object argIndex18 = uname;
//                                                if (SRC.UList.IsDefined(ref argIndex18))
//                                                {
//                                                    Unit localItem() { object argIndex1 = uname; var ret = SRC.UList.Item(ref argIndex1); return ret; }

//                                                    {
//                                                        var withBlock4 = localItem().CurrentForm();
//                                                        if (withBlock4.Status_Renamed == "出撃" | withBlock4.Status_Renamed == "格納")
//                                                        {
//                                                            return IsAbilityAvailableRet;
//                                                        }
//                                                    }
//                                                }
//                                            }
//                                        }

//                                        break;
//                                    }
//                            }
//                        }
//                    }

//                    // 召喚ユニットのデータがちゃんと定義されているかチェック
//                    string localEffectData() { object argIndex1 = i; var ret = ad.EffectData(ref argIndex1); return ret; }

//                    bool localIsDefined() { object argIndex1 = (object)hsdeb28db1320b43f3b566123058fdd2af(); var ret = SRC.UDList.IsDefined(ref argIndex1); return ret; }

//                    if (!localIsDefined())
//                    {
//                        return IsAbilityAvailableRet;
//                    }

//                    string localEffectData1() { object argIndex1 = i; var ret = ad.EffectData(ref argIndex1); return ret; }

//                    UnitData localItem1() { object argIndex1 = (object)hsdcada415e8eb41c68f44c81ef2bb94c6(); var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                    object argIndex20 = "追加パイロット";
//                    pname = localItem1().FeatureData(ref argIndex20);
//                    bool localIsDefined1() { object argIndex1 = pname; var ret = SRC.PDList.IsDefined(ref argIndex1); return ret; }

//                    if (!localIsDefined1())
//                    {
//                        return IsAbilityAvailableRet;
//                    }

//                    // 召喚するユニットに乗るパイロットが汎用パイロットでもザコパイロットでも
//                    // ない場合、そのユニットが既に出撃中であれば使用不可
//                    if (Strings.InStr(pname, "(汎用)") == 0 & Strings.InStr(pname, "(ザコ)") == 0)
//                    {
//                        object argIndex21 = pname;
//                        if (SRC.PList.IsDefined(ref argIndex21))
//                        {
//                            Pilot localItem2() { object argIndex1 = pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

//                            u = localItem2().Unit_Renamed;
//                            if (u is object)
//                            {
//                                if (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納")
//                                {
//                                    return IsAbilityAvailableRet;
//                                }
//                            }
//                        }
//                    }
//                }
//            }

//            if (ref_mode == "ステータス")
//            {
//                IsAbilityAvailableRet = true;
//                return IsAbilityAvailableRet;
//            }

//            var loopTo7 = ad.CountEffect();
//            for (i = 1; i <= loopTo7; i++)
//            {
//                object argIndex24 = i;
//                if (ad.EffectType(ref argIndex24) == "変身")
//                {
//                    // 自分を変身させる場合
//                    if (this.Ability(a).MaxRange == 0)
//                    {
//                        // ノーマルモードを持つユニットは変身できない
//                        // (変身からの復帰が出来ないため)
//                        string argfname2 = "ノーマルモード";
//                        if (IsFeatureAvailable(ref argfname2))
//                        {
//                            return IsAbilityAvailableRet;
//                        }

//                        // その場所で変身可能か？
//                        string localEffectData2() { object argIndex1 = i; var ret = Ability(a).EffectData(ref argIndex1); return ret; }

//                        string localLIndex3() { string arglist = hsc2f37474313640f6843767ac6d51a5dd(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

//                        object argIndex23 = localLIndex3();
//                        {
//                            var withBlock5 = OtherForm(ref argIndex23);
//                            if (!withBlock5.IsAbleToEnter(x, y))
//                            {
//                                return IsAbilityAvailableRet;
//                            }
//                        }
//                    }
//                }
//            }

//            if (ref_mode == "移動前")
//            {
//                IsAbilityAvailableRet = true;
//                return IsAbilityAvailableRet;
//            }

//            if (AbilityMaxRange(a) > 1 | AbilityMaxRange(a) == 0)
//            {
//                string argattr27 = "Ｐ";
//                if (IsAbilityClassifiedAs(a, ref argattr27))
//                {
//                    IsAbilityAvailableRet = true;
//                }
//                else
//                {
//                    IsAbilityAvailableRet = false;
//                }
//            }
//            else
//            {
//                string argattr28 = "Ｑ";
//                if (IsAbilityClassifiedAs(a, ref argattr28))
//                {
//                    IsAbilityAvailableRet = false;
//                }
//                else
//                {
//                    IsAbilityAvailableRet = true;
//                }
//            }

//            return IsAbilityAvailableRet;
//        }

//        // アビリティ a の必要技能を満たしているか。
//        public bool IsAbilityMastered(short a)
//        {
//            bool IsAbilityMasteredRet = default;
//            Pilot argp = null;
//            IsAbilityMasteredRet = IsNecessarySkillSatisfied(ref Ability(a).NecessarySkill, p: ref argp);
//            return IsAbilityMasteredRet;
//        }

//        // アビリティ a の必要条件を満たしているか。
//        public bool IsAbilityEnabled(short a)
//        {
//            bool IsAbilityEnabledRet = default;
//            Pilot argp = null;
//            IsAbilityEnabledRet = IsNecessarySkillSatisfied(ref Ability(a).NecessaryCondition, p: ref argp);
//            return IsAbilityEnabledRet;
//        }

//        // アビリティが使用可能であり、かつ射程内に有効なターゲットがいるかどうか
//        public bool IsAbilityUseful(short a, ref string ref_mode)
//        {
//            bool IsAbilityUsefulRet = default;
//            short i, j;
//            short max_range, min_range;

//            // アビリティが使用可能か？
//            if (!IsAbilityAvailable(a, ref ref_mode))
//            {
//                IsAbilityUsefulRet = false;
//                return IsAbilityUsefulRet;
//            }

//            // 投下型マップアビリティと扇型マップアビリティは特殊なので判定ができない
//            // 移動型マップアビリティは移動手段として使うことを考慮
//            string argattr = "Ｍ投";
//            string argattr1 = "Ｍ扇";
//            string argattr2 = "Ｍ移";
//            if (IsAbilityClassifiedAs(a, ref argattr) | IsAbilityClassifiedAs(a, ref argattr1) | IsAbilityClassifiedAs(a, ref argattr2))
//            {
//                IsAbilityUsefulRet = true;
//                return IsAbilityUsefulRet;
//            }

//            // 召喚は常に有用
//            var loopTo = Ability(a).CountEffect();
//            for (i = 1; i <= loopTo; i++)
//            {
//                object argIndex1 = i;
//                if (Ability(a).EffectType(ref argIndex1) == "召喚")
//                {
//                    IsAbilityUsefulRet = true;
//                    return IsAbilityUsefulRet;
//                }
//            }

//            min_range = AbilityMinRange(a);
//            max_range = AbilityMaxRange(a);

//            // 使用する相手がいるか検索
//            var loopTo1 = (short)GeneralLib.MinLng(x + max_range, Map.MapWidth);
//            for (i = (short)GeneralLib.MaxLng(x - max_range, 1); i <= loopTo1; i++)
//            {
//                var loopTo2 = (short)GeneralLib.MinLng(y + max_range, Map.MapHeight);
//                for (j = (short)GeneralLib.MaxLng(y - max_range, 1); j <= loopTo2; j++)
//                {
//                    if ((short)(Math.Abs((short)(x - i)) + Math.Abs((short)(y - j))) > max_range)
//                    {
//                        goto NextLoop;
//                    }

//                    if (Map.MapDataForUnit[i, j] is null)
//                    {
//                        goto NextLoop;
//                    }

//                    if (IsAbilityEffective(a, ref Map.MapDataForUnit[i, j]))
//                    {
//                        IsAbilityUsefulRet = true;
//                        return IsAbilityUsefulRet;
//                    }

//                NextLoop:
//                    ;
//                }
//            }

//            IsAbilityUsefulRet = false;
//            return IsAbilityUsefulRet;
//        }

//        // アビリティがターゲットtに対して有効(役に立つ)かどうか
//        public bool IsAbilityEffective(short a, ref Unit t)
//        {
//            bool IsAbilityEffectiveRet = default;
//            short i, j;
//            string edata;
//            double elevel;
//            bool flag;
//            {
//                var withBlock = t;
//                // 敵には使用できない。
//                // IsEnemyでは魅了等がかかった味方ユニットを敵と認識してしまうので
//                // ここでは独自の判定基準を使う
//                switch (Party ?? "")
//                {
//                    case "味方":
//                    case "ＮＰＣ":
//                        {
//                            if (withBlock.Party != "味方" & withBlock.Party0 != "味方" & withBlock.Party != "ＮＰＣ" & withBlock.Party0 != "ＮＰＣ")
//                            {
//                                return IsAbilityEffectiveRet;
//                            }

//                            break;
//                        }

//                    default:
//                        {
//                            if ((withBlock.Party ?? "") != (Party ?? "") & (withBlock.Party0 ?? "") != (Party ?? ""))
//                            {
//                                return IsAbilityEffectiveRet;
//                            }

//                            break;
//                        }
//                }

//                // アビリティがそのユニットに対して適用可能か？
//                if (!IsAbilityApplicable(a, ref t))
//                {
//                    return IsAbilityEffectiveRet;
//                }

//                IsAbilityEffectiveRet = true;
//                var loopTo = Ability(a).CountEffect();
//                for (i = 1; i <= loopTo; i++)
//                {
//                    object argIndex1 = i;
//                    edata = Ability(a).EffectData(ref argIndex1);
//                    object argIndex2 = i;
//                    elevel = Ability(a).EffectLevel(ref argIndex2);
//                    object argIndex36 = i;
//                    switch (Ability(a).EffectType(ref argIndex36) ?? "")
//                    {
//                        case "回復":
//                            {
//                                if (elevel > 0d)
//                                {
//                                    if (withBlock.HP < withBlock.MaxHP)
//                                    {
//                                        object argIndex3 = "ゾンビ";
//                                        if (!withBlock.IsConditionSatisfied(ref argIndex3))
//                                        {
//                                            IsAbilityEffectiveRet = true;
//                                            return IsAbilityEffectiveRet;
//                                        }
//                                    }

//                                    IsAbilityEffectiveRet = false;
//                                }
//                                else
//                                {
//                                    // ＨＰを減少させるためのアビリティというのは有り得るので
//                                    IsAbilityEffectiveRet = true;
//                                    return IsAbilityEffectiveRet;
//                                }

//                                break;
//                            }

//                        case "治癒":
//                            {
//                                if (string.IsNullOrEmpty(edata))
//                                {
//                                    object argIndex4 = "攻撃不能";
//                                    object argIndex5 = "移動不能";
//                                    object argIndex6 = "装甲劣化";
//                                    object argIndex7 = "混乱";
//                                    object argIndex8 = "恐怖";
//                                    object argIndex9 = "踊り";
//                                    object argIndex10 = "狂戦士";
//                                    object argIndex11 = "ゾンビ";
//                                    object argIndex12 = "回復不能";
//                                    object argIndex13 = "石化";
//                                    object argIndex14 = "凍結";
//                                    object argIndex15 = "麻痺";
//                                    object argIndex16 = "睡眠";
//                                    object argIndex17 = "毒";
//                                    object argIndex18 = "盲目";
//                                    object argIndex19 = "沈黙";
//                                    object argIndex20 = "魅了";
//                                    object argIndex21 = "憑依";
//                                    object argIndex22 = "オーラ使用不能";
//                                    object argIndex23 = "超能力使用不能";
//                                    object argIndex24 = "同調率使用不能";
//                                    object argIndex25 = "超感覚使用不能";
//                                    object argIndex26 = "知覚強化使用不能";
//                                    object argIndex27 = "霊力使用不能";
//                                    object argIndex28 = "術使用不能";
//                                    object argIndex29 = "技使用不能";
//                                    if (withBlock.ConditionLifetime(ref argIndex4) > 0 | withBlock.ConditionLifetime(ref argIndex5) > 0 | withBlock.ConditionLifetime(ref argIndex6) > 0 | withBlock.ConditionLifetime(ref argIndex7) > 0 | withBlock.ConditionLifetime(ref argIndex8) > 0 | withBlock.ConditionLifetime(ref argIndex9) > 0 | withBlock.ConditionLifetime(ref argIndex10) > 0 | withBlock.ConditionLifetime(ref argIndex11) > 0 | withBlock.ConditionLifetime(ref argIndex12) > 0 | withBlock.ConditionLifetime(ref argIndex13) > 0 | withBlock.ConditionLifetime(ref argIndex14) > 0 | withBlock.ConditionLifetime(ref argIndex15) > 0 | withBlock.ConditionLifetime(ref argIndex16) > 0 | withBlock.ConditionLifetime(ref argIndex17) > 0 | withBlock.ConditionLifetime(ref argIndex18) > 0 | withBlock.ConditionLifetime(ref argIndex19) > 0 | withBlock.ConditionLifetime(ref argIndex20) > 0 | withBlock.ConditionLifetime(ref argIndex21) > 0 | withBlock.ConditionLifetime(ref argIndex22) > 0 | withBlock.ConditionLifetime(ref argIndex23) > 0 | withBlock.ConditionLifetime(ref argIndex24) > 0 | withBlock.ConditionLifetime(ref argIndex25) > 0 | withBlock.ConditionLifetime(ref argIndex26) > 0 | withBlock.ConditionLifetime(ref argIndex27) > 0 | withBlock.ConditionLifetime(ref argIndex28) > 0 | withBlock.ConditionLifetime(ref argIndex29) > 0)
//                                    {
//                                        IsAbilityEffectiveRet = true;
//                                        return IsAbilityEffectiveRet;
//                                    }

//                                    var loopTo1 = withBlock.CountCondition();
//                                    for (j = 1; j <= loopTo1; j++)
//                                    {
//                                        string localCondition2() { object argIndex1 = j; var ret = withBlock.Condition(ref argIndex1); return ret; }

//                                        if (Strings.Len(localCondition2()) > 6)
//                                        {
//                                            // 前回書き忘れたのですが、
//                                            // 弱点はともかく有効は一概にデメリットのみでもないので
//                                            // 状態回復から除外してみました。
//                                            string localCondition1() { object argIndex1 = j; var ret = withBlock.Condition(ref argIndex1); return ret; }

//                                            if (Strings.Right(localCondition1(), 6) == "属性使用不能")
//                                            {
//                                                string localCondition() { object argIndex1 = j; var ret = withBlock.Condition(ref argIndex1); return ret; }

//                                                object argIndex30 = localCondition();
//                                                if (withBlock.ConditionLifetime(ref argIndex30) > 0)
//                                                {
//                                                    IsAbilityEffectiveRet = true;
//                                                    return IsAbilityEffectiveRet;
//                                                }
//                                            }
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    var loopTo2 = GeneralLib.LLength(ref edata);
//                                    for (j = 1; j <= loopTo2; j++)
//                                    {
//                                        object argIndex31 = GeneralLib.LIndex(ref edata, j);
//                                        if (withBlock.ConditionLifetime(ref argIndex31) > 0)
//                                        {
//                                            IsAbilityEffectiveRet = true;
//                                            return IsAbilityEffectiveRet;
//                                        }
//                                    }
//                                }

//                                IsAbilityEffectiveRet = false;
//                                break;
//                            }

//                        case "補給":
//                            {
//                                if (elevel > 0d)
//                                {
//                                    if (withBlock.EN < withBlock.MaxEN)
//                                    {
//                                        object argIndex32 = "ゾンビ";
//                                        if (!withBlock.IsConditionSatisfied(ref argIndex32))
//                                        {
//                                            IsAbilityEffectiveRet = true;
//                                            return IsAbilityEffectiveRet;
//                                        }
//                                    }

//                                    IsAbilityEffectiveRet = false;
//                                }

//                                break;
//                            }

//                        case "霊力回復":
//                        case "プラーナ回復":
//                            {
//                                if (elevel > 0d)
//                                {
//                                    if (withBlock.MainPilot().Plana < withBlock.MainPilot().MaxPlana())
//                                    {
//                                        IsAbilityEffectiveRet = true;
//                                        return IsAbilityEffectiveRet;
//                                    }

//                                    IsAbilityEffectiveRet = false;
//                                }

//                                break;
//                            }

//                        case "ＳＰ回復":
//                            {
//                                if (elevel > 0d)
//                                {
//                                    if (withBlock.MainPilot().SP < withBlock.MainPilot().MaxSP)
//                                    {
//                                        IsAbilityEffectiveRet = true;
//                                        return IsAbilityEffectiveRet;
//                                    }

//                                    var loopTo3 = withBlock.CountPilot();
//                                    for (j = 2; j <= loopTo3; j++)
//                                    {
//                                        Pilot localPilot() { object argIndex1 = j; var ret = withBlock.Pilot(ref argIndex1); return ret; }

//                                        Pilot localPilot1() { object argIndex1 = j; var ret = withBlock.Pilot(ref argIndex1); return ret; }

//                                        if (localPilot().SP < localPilot1().MaxSP)
//                                        {
//                                            IsAbilityEffectiveRet = true;
//                                            return IsAbilityEffectiveRet;
//                                        }
//                                    }

//                                    var loopTo4 = withBlock.CountSupport();
//                                    for (j = 1; j <= loopTo4; j++)
//                                    {
//                                        Pilot localSupport() { object argIndex1 = j; var ret = withBlock.Support(ref argIndex1); return ret; }

//                                        Pilot localSupport1() { object argIndex1 = j; var ret = withBlock.Support(ref argIndex1); return ret; }

//                                        if (localSupport().SP < localSupport1().MaxSP)
//                                        {
//                                            IsAbilityEffectiveRet = true;
//                                            return IsAbilityEffectiveRet;
//                                        }
//                                    }

//                                    string argfname = "追加サポート";
//                                    if (withBlock.IsFeatureAvailable(ref argfname))
//                                    {
//                                        if (withBlock.AdditionalSupport().SP < withBlock.AdditionalSupport().MaxSP)
//                                        {
//                                            IsAbilityEffectiveRet = true;
//                                            return IsAbilityEffectiveRet;
//                                        }
//                                    }

//                                    IsAbilityEffectiveRet = false;
//                                }

//                                break;
//                            }

//                        case "気力増加":
//                            {
//                                if (elevel > 0d)
//                                {
//                                    {
//                                        var withBlock1 = withBlock.MainPilot();
//                                        if (withBlock1.Morale < withBlock1.MaxMorale & withBlock1.Personality != "機械")
//                                        {
//                                            IsAbilityEffectiveRet = true;
//                                            return IsAbilityEffectiveRet;
//                                        }
//                                    }

//                                    var loopTo5 = withBlock.CountPilot();
//                                    for (j = 2; j <= loopTo5; j++)
//                                    {
//                                        object argIndex33 = j;
//                                        {
//                                            var withBlock2 = withBlock.Pilot(ref argIndex33);
//                                            if (withBlock2.Morale < withBlock2.MaxMorale & withBlock2.Personality != "機械")
//                                            {
//                                                IsAbilityEffectiveRet = true;
//                                                return IsAbilityEffectiveRet;
//                                            }
//                                        }
//                                    }

//                                    var loopTo6 = withBlock.CountSupport();
//                                    for (j = 1; j <= loopTo6; j++)
//                                    {
//                                        object argIndex34 = j;
//                                        {
//                                            var withBlock3 = withBlock.Support(ref argIndex34);
//                                            if (withBlock3.Morale < withBlock3.MaxMorale & withBlock3.Personality != "機械")
//                                            {
//                                                IsAbilityEffectiveRet = true;
//                                                return IsAbilityEffectiveRet;
//                                            }
//                                        }
//                                    }

//                                    string argfname1 = "追加サポート";
//                                    if (withBlock.IsFeatureAvailable(ref argfname1))
//                                    {
//                                        {
//                                            var withBlock4 = withBlock.AdditionalSupport();
//                                            if (withBlock4.Morale < withBlock4.MaxMorale & withBlock4.Personality != "機械")
//                                            {
//                                                IsAbilityEffectiveRet = true;
//                                                return IsAbilityEffectiveRet;
//                                            }
//                                        }
//                                    }

//                                    IsAbilityEffectiveRet = false;
//                                }

//                                break;
//                            }

//                        case "装填":
//                            {
//                                if (string.IsNullOrEmpty(edata))
//                                {
//                                    var loopTo7 = withBlock.CountWeapon();
//                                    for (j = 1; j <= loopTo7; j++)
//                                    {
//                                        if (withBlock.Bullet(j) < withBlock.MaxBullet(j))
//                                        {
//                                            IsAbilityEffectiveRet = true;
//                                            return IsAbilityEffectiveRet;
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    var loopTo8 = withBlock.CountWeapon();
//                                    for (j = 1; j <= loopTo8; j++)
//                                    {
//                                        if (withBlock.Bullet(j) < withBlock.MaxBullet(j))
//                                        {
//                                            if ((withBlock.WeaponNickname(j) ?? "") == (edata ?? "") | GeneralLib.InStrNotNest(ref withBlock.Weapon(j).Class_Renamed, ref edata) > 0)
//                                            {
//                                                IsAbilityEffectiveRet = true;
//                                                return IsAbilityEffectiveRet;
//                                            }
//                                        }
//                                    }
//                                }

//                                IsAbilityEffectiveRet = false;
//                                break;
//                            }

//                        case "付加":
//                            {
//                                bool localIsConditionSatisfied() { object argIndex1 = GeneralLib.LIndex(ref edata, 1) + "付加"; var ret = withBlock.IsConditionSatisfied(ref argIndex1); return ret; }

//                                string argattr = "除";
//                                if (!localIsConditionSatisfied() | IsAbilityClassifiedAs(a, ref argattr))
//                                {
//                                    IsAbilityEffectiveRet = true;
//                                    return IsAbilityEffectiveRet;
//                                }

//                                IsAbilityEffectiveRet = false;
//                                break;
//                            }

//                        case "強化":
//                            {
//                                bool localIsConditionSatisfied1() { object argIndex1 = GeneralLib.LIndex(ref edata, 1) + "強化"; var ret = withBlock.IsConditionSatisfied(ref argIndex1); return ret; }

//                                string argattr1 = "除";
//                                if (!localIsConditionSatisfied1() | IsAbilityClassifiedAs(a, ref argattr1))
//                                {
//                                    IsAbilityEffectiveRet = true;
//                                    return IsAbilityEffectiveRet;
//                                }

//                                IsAbilityEffectiveRet = false;
//                                break;
//                            }

//                        case "状態":
//                            {
//                                bool localIsConditionSatisfied2() { object argIndex1 = edata; var ret = withBlock.IsConditionSatisfied(ref argIndex1); return ret; }

//                                if (!localIsConditionSatisfied2())
//                                {
//                                    IsAbilityEffectiveRet = true;
//                                    return IsAbilityEffectiveRet;
//                                }

//                                IsAbilityEffectiveRet = false;
//                                break;
//                            }

//                        case "再行動":
//                            {
//                                if (this.Ability(a).MaxRange == 0)
//                                {
//                                    goto NextEffect;
//                                }

//                                if (withBlock.Action == 0 & withBlock.MaxAction() > 0)
//                                {
//                                    IsAbilityEffectiveRet = true;
//                                    return IsAbilityEffectiveRet;
//                                }

//                                IsAbilityEffectiveRet = false;
//                                break;
//                            }

//                        case "変身":
//                            {
//                                string argfname2 = "ノーマルモード";
//                                if (!withBlock.IsFeatureAvailable(ref argfname2))
//                                {
//                                    IsAbilityEffectiveRet = true;
//                                    return IsAbilityEffectiveRet;
//                                }

//                                IsAbilityEffectiveRet = false;
//                                break;
//                            }

//                        case "能力コピー":
//                            {
//                                string argfname3 = "ノーマルモード";
//                                object argIndex35 = "混乱";
//                                var argt = this;
//                                if (ReferenceEquals(t, this) | IsFeatureAvailable(ref argfname3) | Conversions.ToInteger(withBlock.IsConditionSatisfied(ref argIndex35)) > 0 | withBlock.IsEnemy(ref argt) | IsEnemy(ref t))
//                                {
//                                    IsAbilityEffectiveRet = false;
//                                    goto NextEffect;
//                                }

//                                if (Strings.InStr(edata, "サイズ制限強") > 0)
//                                {
//                                    if ((Size ?? "") != (withBlock.Size ?? ""))
//                                    {
//                                        IsAbilityEffectiveRet = false;
//                                        goto NextEffect;
//                                    }
//                                }
//                                else if (Strings.InStr(edata, "サイズ制限無し") == 0)
//                                {
//                                    switch (Size ?? "")
//                                    {
//                                        case "SS":
//                                            {
//                                                switch (withBlock.Size ?? "")
//                                                {
//                                                    case "M":
//                                                    case "L":
//                                                    case "LL":
//                                                    case "XL":
//                                                        {
//                                                            IsAbilityEffectiveRet = false;
//                                                            goto NextEffect;
//                                                            break;
//                                                        }
//                                                }

//                                                break;
//                                            }

//                                        case "S":
//                                            {
//                                                switch (withBlock.Size ?? "")
//                                                {
//                                                    case "L":
//                                                    case "LL":
//                                                    case "XL":
//                                                        {
//                                                            IsAbilityEffectiveRet = false;
//                                                            goto NextEffect;
//                                                            break;
//                                                        }
//                                                }

//                                                break;
//                                            }

//                                        case "M":
//                                            {
//                                                switch (withBlock.Size ?? "")
//                                                {
//                                                    case "SS":
//                                                    case "LL":
//                                                    case "XL":
//                                                        {
//                                                            IsAbilityEffectiveRet = false;
//                                                            goto NextEffect;
//                                                            break;
//                                                        }
//                                                }

//                                                break;
//                                            }

//                                        case "L":
//                                            {
//                                                switch (withBlock.Size ?? "")
//                                                {
//                                                    case "SS":
//                                                    case "S":
//                                                    case "XL":
//                                                        {
//                                                            IsAbilityEffectiveRet = false;
//                                                            goto NextEffect;
//                                                            break;
//                                                        }
//                                                }

//                                                break;
//                                            }

//                                        case "LL":
//                                            {
//                                                switch (withBlock.Size ?? "")
//                                                {
//                                                    case "SS":
//                                                    case "S":
//                                                    case "M":
//                                                        {
//                                                            IsAbilityEffectiveRet = false;
//                                                            goto NextEffect;
//                                                            break;
//                                                        }
//                                                }

//                                                break;
//                                            }

//                                        case "XL":
//                                            {
//                                                switch (withBlock.Size ?? "")
//                                                {
//                                                    case "SS":
//                                                    case "S":
//                                                    case "M":
//                                                    case "L":
//                                                        {
//                                                            IsAbilityEffectiveRet = false;
//                                                            goto NextEffect;
//                                                            break;
//                                                        }
//                                                }

//                                                break;
//                                            }
//                                    }
//                                }

//                                IsAbilityEffectiveRet = true;
//                                return IsAbilityEffectiveRet;
//                            }
//                    }

//                NextEffect:
//                    ;
//                }

//                // そもそも効果がないものは常に使用可能とみなす
//                // (include等で特殊効果を定義していると仮定)
//                if (IsAbilityEffectiveRet)
//                {
//                    return IsAbilityEffectiveRet;
//                }
//            }

//            return IsAbilityEffectiveRet;
//        }

//        // アビリティがターゲットtに対して適用可能かどうか
//        public bool IsAbilityApplicable(short a, ref Unit t)
//        {
//            bool IsAbilityApplicableRet = default;
//            short i;
//            string fname;
//            string argattr = "封";
//            if (IsAbilityClassifiedAs(a, ref argattr))
//            {
//                if (!t.Weakness(ref Ability(a).Class_Renamed) & !t.Effective(ref Ability(a).Class_Renamed))
//                {
//                    return IsAbilityApplicableRet;
//                }
//            }

//            string argattr1 = "限";
//            if (IsAbilityClassifiedAs(a, ref argattr1))
//            {
//                bool localWeakness() { string argstring2 = "限"; string arganame = Strings.Mid(Ability(a).Class_Renamed, GeneralLib.InStrNotNest(ref Ability(a).Class_Renamed, ref argstring2) + 1); var ret = t.Weakness(ref arganame); return ret; }

//                bool localEffective() { string argstring2 = "限"; string arganame = Strings.Mid(Ability(a).Class_Renamed, GeneralLib.InStrNotNest(ref Ability(a).Class_Renamed, ref argstring2) + 1); var ret = t.Effective(ref arganame); return ret; }

//                if (!localWeakness() & !localEffective())
//                {
//                    return IsAbilityApplicableRet;
//                }
//            }

//            if (ReferenceEquals(this, t))
//            {
//                // 支援専用アビリティは自分には使用できない
//                string argattr2 = "援";
//                if (!IsAbilityClassifiedAs(a, ref argattr2))
//                {
//                    IsAbilityApplicableRet = true;
//                }

//                return IsAbilityApplicableRet;
//            }

//            // 無効化の対象になる場合は使用出来ない
//            if (t.Immune(ref Ability(a).Class_Renamed))
//            {
//                if (!t.Weakness(ref Ability(a).Class_Renamed) & !t.Effective(ref Ability(a).Class_Renamed))
//                {
//                    return IsAbilityApplicableRet;
//                }
//            }

//            string argattr3 = "視";
//            if (IsAbilityClassifiedAs(a, ref argattr3))
//            {
//                object argIndex1 = "盲目";
//                if (t.IsConditionSatisfied(ref argIndex1))
//                {
//                    return IsAbilityApplicableRet;
//                }
//            }

//            {
//                var withBlock = t.MainPilot();
//                string argattr5 = "対";
//                if (IsAbilityClassifiedAs(a, ref argattr5))
//                {
//                    // UPGRADE_WARNING: Mod に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
//                    string argattr4 = "対";
//                    if (withBlock.Level % AbilityLevel(a, ref argattr4) != 0d)
//                    {
//                        return IsAbilityApplicableRet;
//                    }
//                }

//                string argattr6 = "精";
//                if (IsAbilityClassifiedAs(a, ref argattr6))
//                {
//                    if (withBlock.Personality == "機械")
//                    {
//                        return IsAbilityApplicableRet;
//                    }
//                }

//                string argattr7 = "♂";
//                if (IsAbilityClassifiedAs(a, ref argattr7))
//                {
//                    if (withBlock.Sex != "男性")
//                    {
//                        return IsAbilityApplicableRet;
//                    }
//                }

//                string argattr8 = "♀";
//                if (IsAbilityClassifiedAs(a, ref argattr8))
//                {
//                    if (withBlock.Sex != "女性")
//                    {
//                        return IsAbilityApplicableRet;
//                    }
//                }
//            }

//            // 修理不可
//            string argfname = "修理不可";
//            if (t.IsFeatureAvailable(ref argfname))
//            {
//                var loopTo = Ability(a).CountEffect();
//                for (i = 1; i <= loopTo; i++)
//                {
//                    object argIndex2 = i;
//                    if (Ability(a).EffectType(ref argIndex2) == "回復")
//                    {
//                        break;
//                    }
//                }

//                if (i <= Ability(a).CountEffect())
//                {
//                    object argIndex4 = "修理不可";
//                    object argIndex5 = "修理不可";
//                    var loopTo1 = (short)Conversions.ToInteger(t.FeatureData(ref argIndex5));
//                    for (i = 2; i <= loopTo1; i++)
//                    {
//                        object argIndex3 = "修理不可";
//                        string arglist = t.FeatureData(ref argIndex3);
//                        fname = GeneralLib.LIndex(ref arglist, i);
//                        if (Strings.Left(fname, 1) == "!")
//                        {
//                            fname = Strings.Mid(fname, 2);
//                            if ((fname ?? "") != (AbilityNickname(a) ?? ""))
//                            {
//                                return IsAbilityApplicableRet;
//                            }
//                        }
//                        else if ((fname ?? "") == (AbilityNickname(a) ?? ""))
//                        {
//                            return IsAbilityApplicableRet;
//                        }
//                    }
//                }
//            }

//            IsAbilityApplicableRet = true;
//            return IsAbilityApplicableRet;
//        }

//        // ユニット t がアビリティ a の射程範囲内にいるかをチェック
//        public bool IsTargetWithinAbilityRange(short a, ref Unit t)
//        {
//            bool IsTargetWithinAbilityRangeRet = default;
//            short distance;
//            IsTargetWithinAbilityRangeRet = true;
//            distance = (short)(Math.Abs((short)(x - t.x)) + Math.Abs((short)(y - t.y)));

//            // 最小射程チェック
//            if (distance < AbilityMinRange(a))
//            {
//                IsTargetWithinAbilityRangeRet = false;
//                return IsTargetWithinAbilityRangeRet;
//            }

//            // 最大射程チェック
//            if (distance > AbilityMaxRange(a))
//            {
//                IsTargetWithinAbilityRangeRet = false;
//                return IsTargetWithinAbilityRangeRet;
//            }

//            // 合体技で射程が１の場合は相手を囲んでいる必要がある
//            var partners = default(Unit[]);
//            string argattr = "合";
//            string argattr1 = "Ｍ";
//            if (IsAbilityClassifiedAs(a, ref argattr) & !IsAbilityClassifiedAs(a, ref argattr1) & AbilityMaxRange(a) == 1)
//            {
//                string argctype_Renamed = "アビリティ";
//                CombinationPartner(ref argctype_Renamed, a, ref partners, t.x, t.y);
//                if (Information.UBound(partners) == 0)
//                {
//                    IsTargetWithinAbilityRangeRet = false;
//                    return IsTargetWithinAbilityRangeRet;
//                }
//            }

//            return IsTargetWithinAbilityRangeRet;
//        }

//        // 移動を併用した場合にユニット t がアビリティ a の射程範囲内にいるかをチェック
//        public bool IsTargetReachableForAbility(short a, ref Unit t)
//        {
//            bool IsTargetReachableForAbilityRet = default;
//            short i, j;
//            short max_range;
//            IsTargetReachableForAbilityRet = true;
//            // 移動範囲から敵に攻撃が届くかをチェック
//            max_range = AbilityMaxRange(a);
//            var loopTo = (short)GeneralLib.MinLng(t.x + max_range, Map.MapWidth);
//            for (i = (short)GeneralLib.MaxLng(t.x - max_range, 1); i <= loopTo; i++)
//            {
//                var loopTo1 = (short)GeneralLib.MinLng(t.y + (short)(max_range - Math.Abs((short)(t.x - i))), Map.MapHeight);
//                for (j = (short)GeneralLib.MaxLng(t.y - (short)(max_range - Math.Abs((short)(t.x - i))), 1); j <= loopTo1; j++)
//                {
//                    if (!Map.MaskData[i, j])
//                    {
//                        return IsTargetReachableForAbilityRet;
//                    }
//                }
//            }

//            IsTargetReachableForAbilityRet = false;
//            return IsTargetReachableForAbilityRet;
//        }

//        // アビリティの残り使用回数
//        public short Stock(short a)
//        {
//            short StockRet = default;
//            StockRet = (short)(dblStock[a] * MaxStock(a));
//            return StockRet;
//        }

//        // アビリティの最大使用回数
//        public short MaxStock(short a)
//        {
//            short MaxStockRet = default;
//            if (BossRank > 0)
//            {
//                MaxStockRet = (short)(this.Ability(a).Stock * (5 + BossRank) / 5d);
//            }
//            else
//            {
//                MaxStockRet = Ability(a).Stock;
//            }

//            return MaxStockRet;
//        }

//        // アビリティの残り使用回数を設定
//        public void SetStock(short a, short new_stock)
//        {
//            if (new_stock < 0)
//            {
//                dblStock[a] = 0d;
//            }
//            else if (MaxStock(a) > 0)
//            {
//                dblStock[a] = new_stock / (double)MaxStock(a);
//            }
//            else
//            {
//                dblStock[a] = 1d;
//            }
//        }
    }
}
