// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Units
{
    public class UnitAbility
    {
        public Unit Unit { get; }
        public AbilityData Data { get; private set; }

        private SRC SRC;
        private Events.Event Event => SRC.Event;
        private Maps.Map Map => SRC.Map;
        private IGUI GUI => SRC.GUI;

        private double dblStock;

        public UnitAbility(SRC src, Unit unit, AbilityData data)
        {
            SRC = src;
            Unit = unit;
            Data = data;
        }

        // ユニットの何番目のアビリティか
        public int AbilityNo()
        {
            return Unit.Abilities.IndexOf(this) + 1;
        }

        // アビリティの愛称
        public string AbilityNickname()
        {
            // 愛称内の式置換のため、デフォルトユニットを一時的に変更する
            var u = Event.SelectedUnitForEvent;
            Event.SelectedUnitForEvent = Unit;
            var AbilityNicknameRet = Data.Nickname();
            Event.SelectedUnitForEvent = u;
            return AbilityNicknameRet;
        }

        // アビリティ a の最小射程
        public int AbilityMinRange()
        {
            int AbilityMinRangeRet = Data.MinRange;
            if (IsAbilityClassifiedAs("小"))
            {
                AbilityMinRangeRet = GeneralLib.MinLng((int)(AbilityMinRangeRet + AbilityLevel("小")), Data.MaxRange);
            }

            return AbilityMinRangeRet;
        }

        // アビリティ a の最大射程
        public int AbilityMaxRange()
        {
            int AbilityMaxRangeRet = default;
            AbilityMaxRangeRet = Data.MaxRange;
            return AbilityMaxRangeRet;
        }

        // アビリティ a の消費ＥＮ
        public int AbilityENConsumption()
        {
            int AbilityENConsumptionRet = default;
            double rate;
            Pilot p;
            int i;
            {
                AbilityENConsumptionRet = Data.ENConsumption;

                // パイロットの能力によって術及び技の消費ＥＮは減少する
                if (Unit.CountPilot() > 0)
                {
                    p = Unit.MainPilot();

                    // 術に該当するか？
                    if (IsSpellAbility())
                    {
                        // 術に該当する場合は術技能によってＥＮ消費量を変える
                        switch (p.SkillLevel("術", ""))
                        {
                            case 1d:
                                {
                                    break;
                                }

                            case 2d:
                                {
                                    AbilityENConsumptionRet = (int)(0.9d * AbilityENConsumptionRet);
                                    break;
                                }

                            case 3d:
                                {
                                    AbilityENConsumptionRet = (int)(0.8d * AbilityENConsumptionRet);
                                    break;
                                }

                            case 4d:
                                {
                                    AbilityENConsumptionRet = (int)(0.7d * AbilityENConsumptionRet);
                                    break;
                                }

                            case 5d:
                                {
                                    AbilityENConsumptionRet = (int)(0.6d * AbilityENConsumptionRet);
                                    break;
                                }

                            case 6d:
                                {
                                    AbilityENConsumptionRet = (int)(0.5d * AbilityENConsumptionRet);
                                    break;
                                }

                            case 7d:
                                {
                                    AbilityENConsumptionRet = (int)(0.45d * AbilityENConsumptionRet);
                                    break;
                                }

                            case 8d:
                                {
                                    AbilityENConsumptionRet = (int)(0.4d * AbilityENConsumptionRet);
                                    break;
                                }

                            case 9d:
                                {
                                    AbilityENConsumptionRet = (int)(0.35d * AbilityENConsumptionRet);
                                    break;
                                }

                            case var @case when @case >= 10d:
                                {
                                    AbilityENConsumptionRet = (int)(0.3d * AbilityENConsumptionRet);
                                    break;
                                }
                        }

                        AbilityENConsumptionRet = GeneralLib.MinLng(GeneralLib.MaxLng(AbilityENConsumptionRet, 5), Data.ENConsumption);
                    }

                    // 技に該当するか？
                    if (IsFeatAbility())
                    {
                        // 技に該当する場合は技技能によってＥＮ消費量を変える
                        switch (p.SkillLevel("技", ""))
                        {
                            case 1d:
                                {
                                    break;
                                }

                            case 2d:
                                {
                                    AbilityENConsumptionRet = (int)(0.9d * AbilityENConsumptionRet);
                                    break;
                                }

                            case 3d:
                                {
                                    AbilityENConsumptionRet = (int)(0.8d * AbilityENConsumptionRet);
                                    break;
                                }

                            case 4d:
                                {
                                    AbilityENConsumptionRet = (int)(0.7d * AbilityENConsumptionRet);
                                    break;
                                }

                            case 5d:
                                {
                                    AbilityENConsumptionRet = (int)(0.6d * AbilityENConsumptionRet);
                                    break;
                                }

                            case 6d:
                                {
                                    AbilityENConsumptionRet = (int)(0.5d * AbilityENConsumptionRet);
                                    break;
                                }

                            case 7d:
                                {
                                    AbilityENConsumptionRet = (int)(0.45d * AbilityENConsumptionRet);
                                    break;
                                }

                            case 8d:
                                {
                                    AbilityENConsumptionRet = (int)(0.4d * AbilityENConsumptionRet);
                                    break;
                                }

                            case 9d:
                                {
                                    AbilityENConsumptionRet = (int)(0.35d * AbilityENConsumptionRet);
                                    break;
                                }

                            case var case1 when case1 >= 10d:
                                {
                                    AbilityENConsumptionRet = (int)(0.3d * AbilityENConsumptionRet);
                                    break;
                                }
                        }

                        AbilityENConsumptionRet = GeneralLib.MinLng(GeneralLib.MaxLng(AbilityENConsumptionRet, 5), Data.ENConsumption);
                    }
                }

                // ＥＮ消費減少能力による修正
                rate = 1d;
                if (Unit.IsFeatureAvailable("ＥＮ消費減少"))
                {
                    foreach (var f in Unit.Features.Where(x => x.Name == "ＥＮ消費減少"))
                    {
                        rate = rate - 0.1d * f.Level;
                    }
                }

                if (rate < 0.1d)
                {
                    rate = 0.1d;
                }

                AbilityENConsumptionRet = (int)(rate * AbilityENConsumptionRet);
            }

            return AbilityENConsumptionRet;
        }

        // アビリティ a が属性 attr を持つかどうか
        public bool IsAbilityClassifiedAs(string attr)
        {
            bool IsAbilityClassifiedAsRet = default;
            if (GeneralLib.InStrNotNest(Data.Class, attr) > 0)
            {
                IsAbilityClassifiedAsRet = true;
            }
            else
            {
                IsAbilityClassifiedAsRet = false;
            }

            return IsAbilityClassifiedAsRet;
        }

        // アビリティ a の属性 atrr のレベル
        public double AbilityLevel(string attr)
        {
            try
            {
                var attrlv = attr + "L";

                // アビリティ属性を調べてみる
                var aclass = Data.Class;

                // レベル指定があるか？
                var start_idx = Strings.InStr(aclass, attrlv);
                if (start_idx == 0)
                {
                    return 0d;
                }

                // レベル指定部分の切り出し
                start_idx = (start_idx + Strings.Len(attrlv));
                var i = start_idx;
                while (true)
                {
                    var c = Strings.Mid(aclass, i, 1);
                    if (string.IsNullOrEmpty(c))
                    {
                        break;
                    }

                    switch (Strings.Asc(c))
                    {
                        case var @case when 45 <= @case && @case <= 46:
                        case var case1 when 48 <= case1 && case1 <= 57: // "-", ".", 0-9
                            {
                                break;
                            }

                        default:
                            {
                                break;
                            }
                    }

                    i = (i + 1);
                }

                return Conversions.ToDouble(Strings.Mid(aclass, start_idx, i - start_idx));
            }
            catch
            {
                GUI.ErrorMessage(Data.Name + "の" + "アビリティ「" + Data.Name + "」の" + "属性「" + attr + "」のレベル指定が不正です");
                return 0d;
            }
        }

        // アビリティ a が術かどうか
        public bool IsSpellAbility()
        {
            return false;
            // TODO Impl
            //bool IsSpellAbilityRet = default;
            //int i;
            //string nskill;
            //string argattr = "術";
            //if (IsAbilityClassifiedAs(argattr))
            //{
            //    IsSpellAbilityRet = true;
            //    return IsSpellAbilityRet;
            //}

            //{
            //    var p = Unit.MainPilot();
            //    var loopTo = GeneralLib.LLength(Data.NecessarySkill);
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        nskill = GeneralLib.LIndex(Data.NecessarySkill, i);
            //        if (Strings.InStr(nskill, "Lv") > 0)
            //        {
            //            nskill = Strings.Left(nskill, Strings.InStr(nskill, "Lv") - 1);
            //        }

            //        if (p.SkillType(nskill) == "術")
            //        {
            //            IsSpellAbilityRet = true;
            //            return IsSpellAbilityRet;
            //        }
            //    }
            //}

            //return IsSpellAbilityRet;
        }

        // アビリティ a が技かどうか
        public bool IsFeatAbility()
        {
            return false;
            // TODO Impl
            //bool IsFeatAbilityRet = default;
            //int i;
            //string nskill;
            //string argattr = "技";
            //if (IsAbilityClassifiedAs(argattr))
            //{
            //    IsFeatAbilityRet = true;
            //    return IsFeatAbilityRet;
            //}

            //{
            //    var withBlock = MainPilot();
            //    var loopTo = GeneralLib.LLength(Data.NecessarySkill);
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        nskill = GeneralLib.LIndex(Data.NecessarySkill, i);
            //        if (Strings.InStr(nskill, "Lv") > 0)
            //        {
            //            nskill = Strings.Left(nskill, Strings.InStr(nskill, "Lv") - 1);
            //        }

            //        if (withBlock.SkillType(nskill) == "技")
            //        {
            //            IsFeatAbilityRet = true;
            //            return IsFeatAbilityRet;
            //        }
            //    }
            //}

            //return IsFeatAbilityRet;
        }

        // アビリティ a が使用可能かどうか
        //_mode はユニットの状態（移動前、移動後）を示す
        public bool IsAbilityAvailable(string mode)
        {
            return true;
            // TODO Impl
            //bool IsAbilityAvailableRet = default;
            //int j, i, k;
            //AbilityData ad;
            //string uname, pname;
            //Unit u;
            //IsAbilityAvailableRet = false;
            //ad = Data;

            //// イベントコマンド「Disable」
            //if (IsDisabled(ad.Name))
            //{
            //    return IsAbilityAvailableRet;
            //}

            //// パイロットが乗っていなければ常に使用可能と判定
            //if (CountPilot() == 0)
            //{
            //    IsAbilityAvailableRet = true;
            //    return IsAbilityAvailableRet;
            //}

            //// 必要技能
            //if (!IsAbilityMastered())
            //{
            //    return IsAbilityAvailableRet;
            //}

            //// 必要条件
            //if (!IsAbilityEnabled())
            //{
            //    return IsAbilityAvailableRet;
            //}

            //// ステータス表示では必要技能だけ満たしていればＯＫ
            //if (ref_mode == "インターミッション" | string.IsNullOrEmpty(ref_mode))
            //{
            //    IsAbilityAvailableRet = true;
            //    return IsAbilityAvailableRet;
            //}

            //{
            //    var withBlock = MainPilot();
            //    // 必要気力
            //    if (ad.NecessaryMorale > 0)
            //    {
            //        if (withBlock.Morale < ad.NecessaryMorale)
            //        {
            //            return IsAbilityAvailableRet;
            //        }
            //    }

            //    // 霊力消費アビリティ
            //    string argattr2 = "霊";
            //    string argattr3 = "プ";
            //    if (IsAbilityClassifiedAs(argattr2))
            //    {
            //        string argattr = "霊";
            //        if (withBlock.Plana < AbilityLevel(argattr) * 5d)
            //        {
            //            return IsAbilityAvailableRet;
            //        }
            //    }
            //    else if (IsAbilityClassifiedAs(argattr3))
            //    {
            //        string argattr1 = "プ";
            //        if (withBlock.Plana < AbilityLevel(argattr1) * 5d)
            //        {
            //            return IsAbilityAvailableRet;
            //        }
            //    }
            //}

            //// 属性使用不能状態
            //object argIndex1 = "オーラ使用不能";
            //if (ConditionLifetime(argIndex1) > 0)
            //{
            //    string argattr4 = "オ";
            //    if (IsAbilityClassifiedAs(argattr4))
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //object argIndex2 = "超能力使用不能";
            //if (ConditionLifetime(argIndex2) > 0)
            //{
            //    string argattr5 = "超";
            //    if (IsAbilityClassifiedAs(argattr5))
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //object argIndex3 = "同調率使用不能";
            //if (ConditionLifetime(argIndex3) > 0)
            //{
            //    string argattr6 = "シ";
            //    if (IsAbilityClassifiedAs(argattr6))
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //object argIndex4 = "超感覚使用不能";
            //if (ConditionLifetime(argIndex4) > 0)
            //{
            //    string argattr7 = "サ";
            //    if (IsAbilityClassifiedAs(argattr7))
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //object argIndex5 = "知覚強化使用不能";
            //if (ConditionLifetime(argIndex5) > 0)
            //{
            //    string argattr8 = "サ";
            //    if (IsAbilityClassifiedAs(argattr8))
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //object argIndex6 = "霊力使用不能";
            //if (ConditionLifetime(argIndex6) > 0)
            //{
            //    string argattr9 = "霊";
            //    if (IsAbilityClassifiedAs(argattr9))
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //object argIndex7 = "術使用不能";
            //if (ConditionLifetime(argIndex7) > 0)
            //{
            //    string argattr10 = "術";
            //    if (IsAbilityClassifiedAs(argattr10))
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //object argIndex8 = "技使用不能";
            //if (ConditionLifetime(argIndex8) > 0)
            //{
            //    string argattr11 = "技";
            //    if (IsAbilityClassifiedAs(argattr11))
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //var loopTo = CountCondition();
            //for (i = 1; i <= loopTo; i++)
            //{
            //    string localCondition3() { object argIndex1 = i; var ret = Condition(argIndex1); return ret; }

            //    if (Strings.Len(localCondition3()) > 6)
            //    {
            //        string localCondition2() { object argIndex1 = i; var ret = Condition(argIndex1); return ret; }

            //        if (Strings.Right(localCondition2(), 6) == "属性使用不能")
            //        {
            //            string localCondition() { object argIndex1 = i; var ret = Condition(argIndex1); return ret; }

            //            string localCondition1() { object argIndex1 = i; var ret = Condition(argIndex1); return ret; }

            //            string argstring2 = Strings.Left(localCondition(), Strings.Len(localCondition1()) - 6);
            //            if (GeneralLib.InStrNotNest(Data.Class, argstring2) > 0)
            //            {
            //                return IsAbilityAvailableRet;
            //            }
            //        }
            //    }
            //}

            //// 弾数が足りるか
            //if (MaxStock() > 0)
            //{
            //    if (Stock() < 1)
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //// ＥＮが足りるか
            //if (ad.ENConsumption > 0)
            //{
            //    if (EN < AbilityENConsumption())
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //// お金が足りるか……
            //if (Party == "味方")
            //{
            //    string argattr13 = "銭";
            //    if (IsAbilityClassifiedAs(argattr13))
            //    {
            //        string argattr12 = "銭";
            //        if (SRC.Money < GeneralLib.MaxLng(AbilityLevel(argattr12), 1) * Value / 10)
            //        {
            //            return IsAbilityAvailableRet;
            //        }
            //    }
            //}

            //// 移動不能時には移動型マップアビリティは使用不能
            //object argIndex9 = "移動不能";
            //if (IsConditionSatisfied(argIndex9))
            //{
            //    string argattr14 = "Ｍ移";
            //    if (IsAbilityClassifiedAs(argattr14))
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //// 術及び音声技は沈黙状態では使用不能
            //object argIndex10 = "沈黙";
            //if (IsConditionSatisfied(argIndex10))
            //{
            //    {
            //        var withBlock1 = MainPilot();
            //        string argattr15 = "音";
            //        if (IsSpellAbility() | IsAbilityClassifiedAs(argattr15))
            //        {
            //            return IsAbilityAvailableRet;
            //        }
            //    }
            //}

            //// 術は狂戦士状態では使用不能
            //if (Unit.IsConditionSatisfied("狂戦士"))
            //{
            //    {
            //        var withBlock2 = MainPilot();
            //        if (IsSpellAbility())
            //        {
            //            return IsAbilityAvailableRet;
            //        }
            //    }
            //}

            //// 合体技の処理
            //string argattr16 = "合";
            //if (IsAbilityClassifiedAs(argattr16))
            //{
            //    if (!IsCombinationAbilityAvailable())
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //// この地形で変形できるか？
            //string argattr17 = "変";
            //if (IsAbilityClassifiedAs(argattr17))
            //{
            //    string argfname = "変形技";
            //    string argfname1 = "ノーマルモード";
            //    if (IsFeatureAvailable(argfname))
            //    {
            //        var loopTo1 = CountFeature();
            //        for (i = 1; i <= loopTo1; i++)
            //        {
            //            string localFeature() { object argIndex1 = i; var ret = Feature(argIndex1); return ret; }

            //            string localFeatureData1() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //            string localLIndex1() { string arglist = hsc19c10c9cae54732ac7c9c2e90257bd2(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //            if (localFeature() == "変形技" & (localLIndex1() ?? "") == (ad.Name ?? ""))
            //            {
            //                string localFeatureData() { object argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //                string localLIndex() { string arglist = hs999a427db36f427e9868be6a72c4f4c0(); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                Unit localOtherForm() { object argIndex1 = (object)hsb208b29f50af4a41be714b7083a85c98(); var ret = OtherForm(argIndex1); return ret; }

            //                if (!localOtherForm().IsAbleToEnter(x, y))
            //                {
            //                    return IsAbilityAvailableRet;
            //                }
            //            }
            //        }
            //    }
            //    else if (IsFeatureAvailable(argfname1))
            //    {
            //        string localLIndex2() { object argIndex1 = "ノーマルモード"; string arglist = FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //        Unit localOtherForm1() { object argIndex1 = (object)hs89e8cc23250142a2b15ab7b087ddcbd2(); var ret = OtherForm(argIndex1); return ret; }

            //        if (!localOtherForm1().IsAbleToEnter(x, y))
            //        {
            //            return IsAbilityAvailableRet;
            //        }
            //    }

            //    object argIndex12 = "形態固定";
            //    if (IsConditionSatisfied(argIndex12))
            //    {
            //        return IsAbilityAvailableRet;
            //    }

            //    object argIndex13 = "機体固定";
            //    if (IsConditionSatisfied(argIndex13))
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //// 瀕死時限定
            //string argattr18 = "瀕";
            //if (IsAbilityClassifiedAs(argattr18))
            //{
            //    if (HP > MaxHP / 4)
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //// 自動チャージアビリティを充填中
            //object argIndex14 = AbilityNickname() + "充填中";
            //if (IsConditionSatisfied(argIndex14))
            //{
            //    return IsAbilityAvailableRet;
            //}
            //// 共有武器＆アビリティが充填中の場合も使用不可
            //int lv;
            //string argattr24 = "共";
            //if (IsAbilityClassifiedAs(argattr24))
            //{
            //    string argattr19 = "共";
            //    lv = AbilityLevel(argattr19);
            //    var loopTo2 = CountAbility();
            //    for (i = 1; i <= loopTo2; i++)
            //    {
            //        string argattr21 = "共";
            //        if (IsAbilityClassifiedAs(i, argattr21))
            //        {
            //            string argattr20 = "共";
            //            if (lv == AbilityLevel(i, argattr20))
            //            {
            //                object argIndex15 = AbilityNickname(i) + "充填中";
            //                if (IsConditionSatisfied(argIndex15))
            //                {
            //                    return IsAbilityAvailableRet;
            //                }
            //            }
            //        }
            //    }

            //    var loopTo3 = CountAbility();
            //    for (i = 1; i <= loopTo3; i++)
            //    {
            //        string argattr23 = "共";
            //        if (IsAbilityClassifiedAs(i, argattr23))
            //        {
            //            string argattr22 = "共";
            //            if (lv == AbilityLevel(i, argattr22))
            //            {
            //                object argIndex16 = AbilityNickname(i) + "充填中";
            //                if (IsConditionSatisfied(argIndex16))
            //                {
            //                    return IsAbilityAvailableRet;
            //                }
            //            }
            //        }
            //    }
            //}

            //// 使用禁止
            //string argattr25 = "禁";
            //if (Conversions.ToInteger(IsAbilityClassifiedAs(argattr25)) > 0)
            //{
            //    return IsAbilityAvailableRet;
            //}

            //// チャージ判定であればここまででＯＫ
            //if (ref_mode == "チャージ")
            //{
            //    IsAbilityAvailableRet = true;
            //    return IsAbilityAvailableRet;
            //}

            //// チャージ式アビリティ
            //string argattr26 = "Ｃ";
            //if (IsAbilityClassifiedAs(argattr26))
            //{
            //    object argIndex17 = "チャージ完了";
            //    if (!IsConditionSatisfied(argIndex17))
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //var loopTo4 = ad.CountEffect();
            //for (i = 1; i <= loopTo4; i++)
            //{
            //    object argIndex22 = i;
            //    if (ad.EffectType(argIndex22) == "召喚")
            //    {
            //        // 召喚は既に召喚を行っている場合には不可能
            //        var loopTo5 = CountServant();
            //        for (j = 1; j <= loopTo5; j++)
            //        {
            //            Unit localServant() { object argIndex1 = j; var ret = Servant(argIndex1); return ret; }

            //            {
            //                var withBlock3 = localServant().CurrentForm();
            //                switch (withBlock3.Status_Renamed ?? "")
            //                {
            //                    case "出撃":
            //                    case "格納":
            //                        {
            //                            // 使用不可
            //                            return IsAbilityAvailableRet;
            //                        }

            //                    case "旧主形態":
            //                    case "旧形態":
            //                        {
            //                            // 合体後の形態が出撃中なら使用不可
            //                            var loopTo6 = withBlock3.CountFeature();
            //                            for (k = 1; k <= loopTo6; k++)
            //                            {
            //                                object argIndex19 = k;
            //                                if (withBlock3.Feature(argIndex19) == "合体")
            //                                {
            //                                    string localFeatureData2() { object argIndex1 = k; var ret = withBlock3.FeatureData(argIndex1); return ret; }

            //                                    string arglist = localFeatureData2();
            //                                    uname = GeneralLib.LIndex(arglist, 2);
            //                                    object argIndex18 = uname;
            //                                    if (SRC.UList.IsDefined(argIndex18))
            //                                    {
            //                                        Unit localItem() { object argIndex1 = uname; var ret = SRC.UList.Item(argIndex1); return ret; }

            //                                        {
            //                                            var withBlock4 = localItem().CurrentForm();
            //                                            if (withBlock4.Status_Renamed == "出撃" | withBlock4.Status_Renamed == "格納")
            //                                            {
            //                                                return IsAbilityAvailableRet;
            //                                            }
            //                                        }
            //                                    }
            //                                }
            //                            }

            //                            break;
            //                        }
            //                }
            //            }
            //        }

            //        // 召喚ユニットのデータがちゃんと定義されているかチェック
            //        string localEffectData() { object argIndex1 = i; var ret = ad.EffectData(argIndex1); return ret; }

            //        bool localIsDefined() { object argIndex1 = (object)hsdeb28db1320b43f3b566123058fdd2af(); var ret = SRC.UDList.IsDefined(argIndex1); return ret; }

            //        if (!localIsDefined())
            //        {
            //            return IsAbilityAvailableRet;
            //        }

            //        string localEffectData1() { object argIndex1 = i; var ret = ad.EffectData(argIndex1); return ret; }

            //        UnitData localItem1() { object argIndex1 = (object)hsdcada415e8eb41c68f44c81ef2bb94c6(); var ret = SRC.UDList.Item(argIndex1); return ret; }

            //        object argIndex20 = "追加パイロット";
            //        pname = localItem1().FeatureData(argIndex20);
            //        bool localIsDefined1() { object argIndex1 = pname; var ret = SRC.PDList.IsDefined(argIndex1); return ret; }

            //        if (!localIsDefined1())
            //        {
            //            return IsAbilityAvailableRet;
            //        }

            //        // 召喚するユニットに乗るパイロットが汎用パイロットでもザコパイロットでも
            //        // ない場合、そのユニットが既に出撃中であれば使用不可
            //        if (Strings.InStr(pname, "(汎用)") == 0 & Strings.InStr(pname, "(ザコ)") == 0)
            //        {
            //            object argIndex21 = pname;
            //            if (SRC.PList.IsDefined(argIndex21))
            //            {
            //                Pilot localItem2() { object argIndex1 = pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                u = localItem2().Unit_Renamed;
            //                if (u is object)
            //                {
            //                    if (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納")
            //                    {
            //                        return IsAbilityAvailableRet;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            //if (ref_mode == "ステータス")
            //{
            //    IsAbilityAvailableRet = true;
            //    return IsAbilityAvailableRet;
            //}

            //var loopTo7 = ad.CountEffect();
            //for (i = 1; i <= loopTo7; i++)
            //{
            //    object argIndex24 = i;
            //    if (ad.EffectType(argIndex24) == "変身")
            //    {
            //        // 自分を変身させる場合
            //        if (this.Data.MaxRange == 0)
            //        {
            //            // ノーマルモードを持つユニットは変身できない
            //            // (変身からの復帰が出来ないため)
            //            string argfname2 = "ノーマルモード";
            //            if (IsFeatureAvailable(argfname2))
            //            {
            //                return IsAbilityAvailableRet;
            //            }

            //            // その場所で変身可能か？
            //            string localEffectData2() { object argIndex1 = i; var ret = Data.EffectData(argIndex1); return ret; }

            //            string localLIndex3() { string arglist = hsc2f37474313640f6843767ac6d51a5dd(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //            object argIndex23 = localLIndex3();
            //            {
            //                var withBlock5 = OtherForm(argIndex23);
            //                if (!withBlock5.IsAbleToEnter(x, y))
            //                {
            //                    return IsAbilityAvailableRet;
            //                }
            //            }
            //        }
            //    }
            //}

            //if (ref_mode == "移動前")
            //{
            //    IsAbilityAvailableRet = true;
            //    return IsAbilityAvailableRet;
            //}

            //if (AbilityMaxRange() > 1 | AbilityMaxRange() == 0)
            //{
            //    string argattr27 = "Ｐ";
            //    if (IsAbilityClassifiedAs(argattr27))
            //    {
            //        IsAbilityAvailableRet = true;
            //    }
            //    else
            //    {
            //        IsAbilityAvailableRet = false;
            //    }
            //}
            //else
            //{
            //    string argattr28 = "Ｑ";
            //    if (IsAbilityClassifiedAs(argattr28))
            //    {
            //        IsAbilityAvailableRet = false;
            //    }
            //    else
            //    {
            //        IsAbilityAvailableRet = true;
            //    }
            //}

            //return IsAbilityAvailableRet;
        }

        // アビリティ a の必要技能を満たしているか。
        public bool IsAbilityMastered()
        {
            return true;
            // TODO Impl
            //bool IsAbilityMasteredRet = default;
            //Pilot argp = null;
            //IsAbilityMasteredRet = IsNecessarySkillSatisfied(Data.NecessarySkill, p: argp);
            //return IsAbilityMasteredRet;
        }

        // アビリティ a の必要条件を満たしているか。
        public bool IsAbilityEnabled()
        {
            return true;
            // TODO Impl
            //bool IsAbilityEnabledRet = default;
            //Pilot argp = null;
            //IsAbilityEnabledRet = IsNecessarySkillSatisfied(Data.NecessaryCondition, p: argp);
            //return IsAbilityEnabledRet;
        }

        // アビリティが使用可能であり、かつ射程内に有効なターゲットがいるかどうか
        public bool IsAbilityUseful(string mode)
        {
            return true;
            // TODO Impl
            //bool IsAbilityUsefulRet = default;
            //int i, j;
            //int max_range, min_range;

            //// アビリティが使用可能か？
            //if (!IsAbilityAvailable(_mode))
            //{
            //    IsAbilityUsefulRet = false;
            //    return IsAbilityUsefulRet;
            //}

            //// 投下型マップアビリティと扇型マップアビリティは特殊なので判定ができない
            //// 移動型マップアビリティは移動手段として使うことを考慮
            //string argattr = "Ｍ投";
            //string argattr1 = "Ｍ扇";
            //string argattr2 = "Ｍ移";
            //if (IsAbilityClassifiedAs(argattr) | IsAbilityClassifiedAs(argattr1) | IsAbilityClassifiedAs(argattr2))
            //{
            //    IsAbilityUsefulRet = true;
            //    return IsAbilityUsefulRet;
            //}

            //// 召喚は常に有用
            //var loopTo = Data.CountEffect();
            //for (i = 1; i <= loopTo; i++)
            //{
            //    object argIndex1 = i;
            //    if (Data.EffectType(argIndex1) == "召喚")
            //    {
            //        IsAbilityUsefulRet = true;
            //        return IsAbilityUsefulRet;
            //    }
            //}

            //min_range = AbilityMinRange();
            //max_range = AbilityMaxRange();

            //// 使用する相手がいるか検索
            //var loopTo1 = GeneralLib.MinLng(x + max_range, Map.MapWidth);
            //for (i = GeneralLib.MaxLng(x - max_range, 1); i <= loopTo1; i++)
            //{
            //    var loopTo2 = GeneralLib.MinLng(y + max_range, Map.MapHeight);
            //    for (j = GeneralLib.MaxLng(y - max_range, 1); j <= loopTo2; j++)
            //    {
            //        if ((Math.Abs((x - i)) + Math.Abs((y - j))) > max_range)
            //        {
            //            goto NextLoop;
            //        }

            //        if (Map.MapDataForUnit[i, j] is null)
            //        {
            //            goto NextLoop;
            //        }

            //        if (IsAbilityEffective(Map.MapDataForUnit[i, j]))
            //        {
            //            IsAbilityUsefulRet = true;
            //            return IsAbilityUsefulRet;
            //        }

            //    NextLoop:
            //        ;
            //    }
            //}

            //IsAbilityUsefulRet = false;
            //return IsAbilityUsefulRet;
        }

        // アビリティがターゲットtに対して有効(役に立つ)かどうか
        public bool IsAbilityEffective(Unit t)
        {
            return true;
            // TODO Impl
            //bool IsAbilityEffectiveRet = default;
            //int i, j;
            //string edata;
            //double elevel;
            //bool flag;
            //{
            //    var withBlock = t;
            //    // 敵には使用できない。
            //    // IsEnemyでは魅了等がかかった味方ユニットを敵と認識してしまうので
            //    // ここでは独自の判定基準を使う
            //    switch (Party ?? "")
            //    {
            //        case "味方":
            //        case "ＮＰＣ":
            //            {
            //                if (withBlock.Party != "味方" & withBlock.Party0 != "味方" & withBlock.Party != "ＮＰＣ" & withBlock.Party0 != "ＮＰＣ")
            //                {
            //                    return IsAbilityEffectiveRet;
            //                }

            //                break;
            //            }

            //        default:
            //            {
            //                if ((withBlock.Party ?? "") != (Party ?? "") & (withBlock.Party0 ?? "") != (Party ?? ""))
            //                {
            //                    return IsAbilityEffectiveRet;
            //                }

            //                break;
            //            }
            //    }

            //    // アビリティがそのユニットに対して適用可能か？
            //    if (!IsAbilityApplicable(t))
            //    {
            //        return IsAbilityEffectiveRet;
            //    }

            //    IsAbilityEffectiveRet = true;
            //    var loopTo = Data.CountEffect();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        object argIndex1 = i;
            //        edata = Data.EffectData(argIndex1);
            //        object argIndex2 = i;
            //        elevel = Data.EffectLevel(argIndex2);
            //        object argIndex36 = i;
            //        switch (Data.EffectType(argIndex36) ?? "")
            //        {
            //            case "回復":
            //                {
            //                    if (elevel > 0d)
            //                    {
            //                        if (withBlock.HP < withBlock.MaxHP)
            //                        {
            //                            object argIndex3 = "ゾンビ";
            //                            if (!withBlock.IsConditionSatisfied(argIndex3))
            //                            {
            //                                IsAbilityEffectiveRet = true;
            //                                return IsAbilityEffectiveRet;
            //                            }
            //                        }

            //                        IsAbilityEffectiveRet = false;
            //                    }
            //                    else
            //                    {
            //                        // ＨＰを減少させるためのアビリティというのは有り得るので
            //                        IsAbilityEffectiveRet = true;
            //                        return IsAbilityEffectiveRet;
            //                    }

            //                    break;
            //                }

            //            case "治癒":
            //                {
            //                    if (string.IsNullOrEmpty(edata))
            //                    {
            //                        object argIndex4 = "攻撃不能";
            //                        object argIndex5 = "移動不能";
            //                        object argIndex6 = "装甲劣化";
            //                        object argIndex7 = "混乱";
            //                        object argIndex8 = "恐怖";
            //                        object argIndex9 = "踊り";
            //                        object argIndex10 = "狂戦士";
            //                        object argIndex11 = "ゾンビ";
            //                        object argIndex12 = "回復不能";
            //                        object argIndex13 = "石化";
            //                        object argIndex14 = "凍結";
            //                        object argIndex15 = "麻痺";
            //                        object argIndex16 = "睡眠";
            //                        object argIndex17 = "毒";
            //                        object argIndex18 = "盲目";
            //                        object argIndex19 = "沈黙";
            //                        object argIndex20 = "魅了";
            //                        object argIndex21 = "憑依";
            //                        object argIndex22 = "オーラ使用不能";
            //                        object argIndex23 = "超能力使用不能";
            //                        object argIndex24 = "同調率使用不能";
            //                        object argIndex25 = "超感覚使用不能";
            //                        object argIndex26 = "知覚強化使用不能";
            //                        object argIndex27 = "霊力使用不能";
            //                        object argIndex28 = "術使用不能";
            //                        object argIndex29 = "技使用不能";
            //                        if (withBlock.ConditionLifetime(argIndex4) > 0 | withBlock.ConditionLifetime(argIndex5) > 0 | withBlock.ConditionLifetime(argIndex6) > 0 | withBlock.ConditionLifetime(argIndex7) > 0 | withBlock.ConditionLifetime(argIndex8) > 0 | withBlock.ConditionLifetime(argIndex9) > 0 | withBlock.ConditionLifetime(argIndex10) > 0 | withBlock.ConditionLifetime(argIndex11) > 0 | withBlock.ConditionLifetime(argIndex12) > 0 | withBlock.ConditionLifetime(argIndex13) > 0 | withBlock.ConditionLifetime(argIndex14) > 0 | withBlock.ConditionLifetime(argIndex15) > 0 | withBlock.ConditionLifetime(argIndex16) > 0 | withBlock.ConditionLifetime(argIndex17) > 0 | withBlock.ConditionLifetime(argIndex18) > 0 | withBlock.ConditionLifetime(argIndex19) > 0 | withBlock.ConditionLifetime(argIndex20) > 0 | withBlock.ConditionLifetime(argIndex21) > 0 | withBlock.ConditionLifetime(argIndex22) > 0 | withBlock.ConditionLifetime(argIndex23) > 0 | withBlock.ConditionLifetime(argIndex24) > 0 | withBlock.ConditionLifetime(argIndex25) > 0 | withBlock.ConditionLifetime(argIndex26) > 0 | withBlock.ConditionLifetime(argIndex27) > 0 | withBlock.ConditionLifetime(argIndex28) > 0 | withBlock.ConditionLifetime(argIndex29) > 0)
            //                        {
            //                            IsAbilityEffectiveRet = true;
            //                            return IsAbilityEffectiveRet;
            //                        }

            //                        var loopTo1 = withBlock.CountCondition();
            //                        for (j = 1; j <= loopTo1; j++)
            //                        {
            //                            string localCondition2() { object argIndex1 = j; var ret = withBlock.Condition(argIndex1); return ret; }

            //                            if (Strings.Len(localCondition2()) > 6)
            //                            {
            //                                // 前回書き忘れたのですが、
            //                                // 弱点はともかく有効は一概にデメリットのみでもないので
            //                                // 状態回復から除外してみました。
            //                                string localCondition1() { object argIndex1 = j; var ret = withBlock.Condition(argIndex1); return ret; }

            //                                if (Strings.Right(localCondition1(), 6) == "属性使用不能")
            //                                {
            //                                    string localCondition() { object argIndex1 = j; var ret = withBlock.Condition(argIndex1); return ret; }

            //                                    object argIndex30 = localCondition();
            //                                    if (withBlock.ConditionLifetime(argIndex30) > 0)
            //                                    {
            //                                        IsAbilityEffectiveRet = true;
            //                                        return IsAbilityEffectiveRet;
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        var loopTo2 = GeneralLib.LLength(edata);
            //                        for (j = 1; j <= loopTo2; j++)
            //                        {
            //                            object argIndex31 = GeneralLib.LIndex(edata, j);
            //                            if (withBlock.ConditionLifetime(argIndex31) > 0)
            //                            {
            //                                IsAbilityEffectiveRet = true;
            //                                return IsAbilityEffectiveRet;
            //                            }
            //                        }
            //                    }

            //                    IsAbilityEffectiveRet = false;
            //                    break;
            //                }

            //            case "補給":
            //                {
            //                    if (elevel > 0d)
            //                    {
            //                        if (withBlock.EN < withBlock.MaxEN)
            //                        {
            //                            object argIndex32 = "ゾンビ";
            //                            if (!withBlock.IsConditionSatisfied(argIndex32))
            //                            {
            //                                IsAbilityEffectiveRet = true;
            //                                return IsAbilityEffectiveRet;
            //                            }
            //                        }

            //                        IsAbilityEffectiveRet = false;
            //                    }

            //                    break;
            //                }

            //            case "霊力回復":
            //            case "プラーナ回復":
            //                {
            //                    if (elevel > 0d)
            //                    {
            //                        if (withBlock.MainPilot().Plana < withBlock.MainPilot().MaxPlana())
            //                        {
            //                            IsAbilityEffectiveRet = true;
            //                            return IsAbilityEffectiveRet;
            //                        }

            //                        IsAbilityEffectiveRet = false;
            //                    }

            //                    break;
            //                }

            //            case "ＳＰ回復":
            //                {
            //                    if (elevel > 0d)
            //                    {
            //                        if (withBlock.MainPilot().SP < withBlock.MainPilot().MaxSP)
            //                        {
            //                            IsAbilityEffectiveRet = true;
            //                            return IsAbilityEffectiveRet;
            //                        }

            //                        var loopTo3 = withBlock.CountPilot();
            //                        for (j = 2; j <= loopTo3; j++)
            //                        {
            //                            Pilot localPilot() { object argIndex1 = j; var ret = withBlock.Pilot(argIndex1); return ret; }

            //                            Pilot localPilot1() { object argIndex1 = j; var ret = withBlock.Pilot(argIndex1); return ret; }

            //                            if (localPilot().SP < localPilot1().MaxSP)
            //                            {
            //                                IsAbilityEffectiveRet = true;
            //                                return IsAbilityEffectiveRet;
            //                            }
            //                        }

            //                        var loopTo4 = withBlock.CountSupport();
            //                        for (j = 1; j <= loopTo4; j++)
            //                        {
            //                            Pilot localSupport() { object argIndex1 = j; var ret = withBlock.Support(argIndex1); return ret; }

            //                            Pilot localSupport1() { object argIndex1 = j; var ret = withBlock.Support(argIndex1); return ret; }

            //                            if (localSupport().SP < localSupport1().MaxSP)
            //                            {
            //                                IsAbilityEffectiveRet = true;
            //                                return IsAbilityEffectiveRet;
            //                            }
            //                        }

            //                        string argfname = "追加サポート";
            //                        if (withBlock.IsFeatureAvailable(argfname))
            //                        {
            //                            if (withBlock.AdditionalSupport().SP < withBlock.AdditionalSupport().MaxSP)
            //                            {
            //                                IsAbilityEffectiveRet = true;
            //                                return IsAbilityEffectiveRet;
            //                            }
            //                        }

            //                        IsAbilityEffectiveRet = false;
            //                    }

            //                    break;
            //                }

            //            case "気力増加":
            //                {
            //                    if (elevel > 0d)
            //                    {
            //                        {
            //                            var withBlock1 = withBlock.MainPilot();
            //                            if (withBlock1.Morale < withBlock1.MaxMorale & withBlock1.Personality != "機械")
            //                            {
            //                                IsAbilityEffectiveRet = true;
            //                                return IsAbilityEffectiveRet;
            //                            }
            //                        }

            //                        var loopTo5 = withBlock.CountPilot();
            //                        for (j = 2; j <= loopTo5; j++)
            //                        {
            //                            object argIndex33 = j;
            //                            {
            //                                var withBlock2 = withBlock.Pilot(argIndex33);
            //                                if (withBlock2.Morale < withBlock2.MaxMorale & withBlock2.Personality != "機械")
            //                                {
            //                                    IsAbilityEffectiveRet = true;
            //                                    return IsAbilityEffectiveRet;
            //                                }
            //                            }
            //                        }

            //                        var loopTo6 = withBlock.CountSupport();
            //                        for (j = 1; j <= loopTo6; j++)
            //                        {
            //                            object argIndex34 = j;
            //                            {
            //                                var withBlock3 = withBlock.Support(argIndex34);
            //                                if (withBlock3.Morale < withBlock3.MaxMorale & withBlock3.Personality != "機械")
            //                                {
            //                                    IsAbilityEffectiveRet = true;
            //                                    return IsAbilityEffectiveRet;
            //                                }
            //                            }
            //                        }

            //                        string argfname1 = "追加サポート";
            //                        if (withBlock.IsFeatureAvailable(argfname1))
            //                        {
            //                            {
            //                                var withBlock4 = withBlock.AdditionalSupport();
            //                                if (withBlock4.Morale < withBlock4.MaxMorale & withBlock4.Personality != "機械")
            //                                {
            //                                    IsAbilityEffectiveRet = true;
            //                                    return IsAbilityEffectiveRet;
            //                                }
            //                            }
            //                        }

            //                        IsAbilityEffectiveRet = false;
            //                    }

            //                    break;
            //                }

            //            case "装填":
            //                {
            //                    if (string.IsNullOrEmpty(edata))
            //                    {
            //                        var loopTo7 = withBlock.CountWeapon();
            //                        for (j = 1; j <= loopTo7; j++)
            //                        {
            //                            if (withBlock.Bullet(j) < withBlock.MaxBullet(j))
            //                            {
            //                                IsAbilityEffectiveRet = true;
            //                                return IsAbilityEffectiveRet;
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        var loopTo8 = withBlock.CountWeapon();
            //                        for (j = 1; j <= loopTo8; j++)
            //                        {
            //                            if (withBlock.Bullet(j) < withBlock.MaxBullet(j))
            //                            {
            //                                if ((withBlock.WeaponNickname(j) ?? "") == (edata ?? "") | GeneralLib.InStrNotNest(withBlock.Weapon(j).Class, edata) > 0)
            //                                {
            //                                    IsAbilityEffectiveRet = true;
            //                                    return IsAbilityEffectiveRet;
            //                                }
            //                            }
            //                        }
            //                    }

            //                    IsAbilityEffectiveRet = false;
            //                    break;
            //                }

            //            case "付加":
            //                {
            //                    bool localIsConditionSatisfied() { object argIndex1 = GeneralLib.LIndex(edata, 1) + "付加"; var ret = withBlock.IsConditionSatisfied(argIndex1); return ret; }

            //                    string argattr = "除";
            //                    if (!localIsConditionSatisfied() | IsAbilityClassifiedAs(argattr))
            //                    {
            //                        IsAbilityEffectiveRet = true;
            //                        return IsAbilityEffectiveRet;
            //                    }

            //                    IsAbilityEffectiveRet = false;
            //                    break;
            //                }

            //            case "強化":
            //                {
            //                    bool localIsConditionSatisfied1() { object argIndex1 = GeneralLib.LIndex(edata, 1) + "強化"; var ret = withBlock.IsConditionSatisfied(argIndex1); return ret; }

            //                    string argattr1 = "除";
            //                    if (!localIsConditionSatisfied1() | IsAbilityClassifiedAs(argattr1))
            //                    {
            //                        IsAbilityEffectiveRet = true;
            //                        return IsAbilityEffectiveRet;
            //                    }

            //                    IsAbilityEffectiveRet = false;
            //                    break;
            //                }

            //            case "状態":
            //                {
            //                    bool localIsConditionSatisfied2() { object argIndex1 = edata; var ret = withBlock.IsConditionSatisfied(argIndex1); return ret; }

            //                    if (!localIsConditionSatisfied2())
            //                    {
            //                        IsAbilityEffectiveRet = true;
            //                        return IsAbilityEffectiveRet;
            //                    }

            //                    IsAbilityEffectiveRet = false;
            //                    break;
            //                }

            //            case "再行動":
            //                {
            //                    if (this.Data.MaxRange == 0)
            //                    {
            //                        goto NextEffect;
            //                    }

            //                    if (withBlock.Action == 0 & withBlock.MaxAction() > 0)
            //                    {
            //                        IsAbilityEffectiveRet = true;
            //                        return IsAbilityEffectiveRet;
            //                    }

            //                    IsAbilityEffectiveRet = false;
            //                    break;
            //                }

            //            case "変身":
            //                {
            //                    string argfname2 = "ノーマルモード";
            //                    if (!withBlock.IsFeatureAvailable(argfname2))
            //                    {
            //                        IsAbilityEffectiveRet = true;
            //                        return IsAbilityEffectiveRet;
            //                    }

            //                    IsAbilityEffectiveRet = false;
            //                    break;
            //                }

            //            case "能力コピー":
            //                {
            //                    string argfname3 = "ノーマルモード";
            //                    object argIndex35 = "混乱";
            //                    var argt = this;
            //                    if (ReferenceEquals(t, this) | IsFeatureAvailable(argfname3) | Conversions.ToInteger(withBlock.IsConditionSatisfied(argIndex35)) > 0 | withBlock.IsEnemy(argt) | IsEnemy(t))
            //                    {
            //                        IsAbilityEffectiveRet = false;
            //                        goto NextEffect;
            //                    }

            //                    if (Strings.InStr(edata, "サイズ制限強") > 0)
            //                    {
            //                        if ((Size ?? "") != (withBlock.Size ?? ""))
            //                        {
            //                            IsAbilityEffectiveRet = false;
            //                            goto NextEffect;
            //                        }
            //                    }
            //                    else if (Strings.InStr(edata, "サイズ制限無し") == 0)
            //                    {
            //                        switch (Size ?? "")
            //                        {
            //                            case "SS":
            //                                {
            //                                    switch (withBlock.Size ?? "")
            //                                    {
            //                                        case "M":
            //                                        case "L":
            //                                        case "LL":
            //                                        case "XL":
            //                                            {
            //                                                IsAbilityEffectiveRet = false;
            //                                                goto NextEffect;
            //                                                break;
            //                                            }
            //                                    }

            //                                    break;
            //                                }

            //                            case "S":
            //                                {
            //                                    switch (withBlock.Size ?? "")
            //                                    {
            //                                        case "L":
            //                                        case "LL":
            //                                        case "XL":
            //                                            {
            //                                                IsAbilityEffectiveRet = false;
            //                                                goto NextEffect;
            //                                                break;
            //                                            }
            //                                    }

            //                                    break;
            //                                }

            //                            case "M":
            //                                {
            //                                    switch (withBlock.Size ?? "")
            //                                    {
            //                                        case "SS":
            //                                        case "LL":
            //                                        case "XL":
            //                                            {
            //                                                IsAbilityEffectiveRet = false;
            //                                                goto NextEffect;
            //                                                break;
            //                                            }
            //                                    }

            //                                    break;
            //                                }

            //                            case "L":
            //                                {
            //                                    switch (withBlock.Size ?? "")
            //                                    {
            //                                        case "SS":
            //                                        case "S":
            //                                        case "XL":
            //                                            {
            //                                                IsAbilityEffectiveRet = false;
            //                                                goto NextEffect;
            //                                                break;
            //                                            }
            //                                    }

            //                                    break;
            //                                }

            //                            case "LL":
            //                                {
            //                                    switch (withBlock.Size ?? "")
            //                                    {
            //                                        case "SS":
            //                                        case "S":
            //                                        case "M":
            //                                            {
            //                                                IsAbilityEffectiveRet = false;
            //                                                goto NextEffect;
            //                                                break;
            //                                            }
            //                                    }

            //                                    break;
            //                                }

            //                            case "XL":
            //                                {
            //                                    switch (withBlock.Size ?? "")
            //                                    {
            //                                        case "SS":
            //                                        case "S":
            //                                        case "M":
            //                                        case "L":
            //                                            {
            //                                                IsAbilityEffectiveRet = false;
            //                                                goto NextEffect;
            //                                                break;
            //                                            }
            //                                    }

            //                                    break;
            //                                }
            //                        }
            //                    }

            //                    IsAbilityEffectiveRet = true;
            //                    return IsAbilityEffectiveRet;
            //                }
            //        }

            //    NextEffect:
            //        ;
            //    }

            //    // そもそも効果がないものは常に使用可能とみなす
            //    // (include等で特殊効果を定義していると仮定)
            //    if (IsAbilityEffectiveRet)
            //    {
            //        return IsAbilityEffectiveRet;
            //    }
            //}

            //return IsAbilityEffectiveRet;
        }

        // アビリティがターゲットtに対して適用可能かどうか
        public bool IsAbilityApplicable(Unit t)
        {
            return true;
            // TODO Impl
            //bool IsAbilityApplicableRet = default;
            //int i;
            //string fname;
            //string argattr = "封";
            //if (IsAbilityClassifiedAs(argattr))
            //{
            //    if (!t.Weakness(Data.Class) & !t.Effective(Data.Class))
            //    {
            //        return IsAbilityApplicableRet;
            //    }
            //}

            //string argattr1 = "限";
            //if (IsAbilityClassifiedAs(argattr1))
            //{
            //    bool localWeakness() { string argstring2 = "限"; string arganame = Strings.Mid(Data.Class, GeneralLib.InStrNotNest(Data.Class, argstring2) + 1); var ret = t.Weakness(arganame); return ret; }

            //    bool localEffective() { string argstring2 = "限"; string arganame = Strings.Mid(Data.Class, GeneralLib.InStrNotNest(Data.Class, argstring2) + 1); var ret = t.Effective(arganame); return ret; }

            //    if (!localWeakness() & !localEffective())
            //    {
            //        return IsAbilityApplicableRet;
            //    }
            //}

            //if (ReferenceEquals(this, t))
            //{
            //    // 支援専用アビリティは自分には使用できない
            //    string argattr2 = "援";
            //    if (!IsAbilityClassifiedAs(argattr2))
            //    {
            //        IsAbilityApplicableRet = true;
            //    }

            //    return IsAbilityApplicableRet;
            //}

            //// 無効化の対象になる場合は使用出来ない
            //if (t.Immune(Data.Class))
            //{
            //    if (!t.Weakness(Data.Class) & !t.Effective(Data.Class))
            //    {
            //        return IsAbilityApplicableRet;
            //    }
            //}

            //string argattr3 = "視";
            //if (IsAbilityClassifiedAs(argattr3))
            //{
            //    object argIndex1 = "盲目";
            //    if (t.IsConditionSatisfied(argIndex1))
            //    {
            //        return IsAbilityApplicableRet;
            //    }
            //}

            //{
            //    var withBlock = t.MainPilot();
            //    string argattr5 = "対";
            //    if (IsAbilityClassifiedAs(argattr5))
            //    {
            //        // UPGRADE_WARNING: Mod に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //        string argattr4 = "対";
            //        if (withBlock.Level % AbilityLevel(argattr4) != 0d)
            //        {
            //            return IsAbilityApplicableRet;
            //        }
            //    }

            //    string argattr6 = "精";
            //    if (IsAbilityClassifiedAs(argattr6))
            //    {
            //        if (withBlock.Personality == "機械")
            //        {
            //            return IsAbilityApplicableRet;
            //        }
            //    }

            //    string argattr7 = "♂";
            //    if (IsAbilityClassifiedAs(argattr7))
            //    {
            //        if (withBlock.Sex != "男性")
            //        {
            //            return IsAbilityApplicableRet;
            //        }
            //    }

            //    string argattr8 = "♀";
            //    if (IsAbilityClassifiedAs(argattr8))
            //    {
            //        if (withBlock.Sex != "女性")
            //        {
            //            return IsAbilityApplicableRet;
            //        }
            //    }
            //}

            //// 修理不可
            //string argfname = "修理不可";
            //if (t.IsFeatureAvailable(argfname))
            //{
            //    var loopTo = Data.CountEffect();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        object argIndex2 = i;
            //        if (Data.EffectType(argIndex2) == "回復")
            //        {
            //            break;
            //        }
            //    }

            //    if (i <= Data.CountEffect())
            //    {
            //        object argIndex4 = "修理不可";
            //        object argIndex5 = "修理不可";
            //        var loopTo1 = Conversions.ToInteger(t.FeatureData(argIndex5));
            //        for (i = 2; i <= loopTo1; i++)
            //        {
            //            object argIndex3 = "修理不可";
            //            string arglist = t.FeatureData(argIndex3);
            //            fname = GeneralLib.LIndex(arglist, i);
            //            if (Strings.Left(fname, 1) == "!")
            //            {
            //                fname = Strings.Mid(fname, 2);
            //                if ((fname ?? "") != (AbilityNickname() ?? ""))
            //                {
            //                    return IsAbilityApplicableRet;
            //                }
            //            }
            //            else if ((fname ?? "") == (AbilityNickname() ?? ""))
            //            {
            //                return IsAbilityApplicableRet;
            //            }
            //        }
            //    }
            //}

            //IsAbilityApplicableRet = true;
            //return IsAbilityApplicableRet;
        }

        // ユニット t がアビリティ a の射程範囲内にいるかをチェック
        public bool IsTargetWithinAbilityRange(Unit t)
        {
            return true;
            // TODO Impl
            //bool IsTargetWithinAbilityRangeRet = default;
            //int distance;
            //IsTargetWithinAbilityRangeRet = true;
            //distance = (Math.Abs((x - t.x)) + Math.Abs((y - t.y)));

            //// 最小射程チェック
            //if (distance < AbilityMinRange())
            //{
            //    IsTargetWithinAbilityRangeRet = false;
            //    return IsTargetWithinAbilityRangeRet;
            //}

            //// 最大射程チェック
            //if (distance > AbilityMaxRange())
            //{
            //    IsTargetWithinAbilityRangeRet = false;
            //    return IsTargetWithinAbilityRangeRet;
            //}

            //// 合体技で射程が１の場合は相手を囲んでいる必要がある
            //var partners = default(Unit[]);
            //string argattr = "合";
            //string argattr1 = "Ｍ";
            //if (IsAbilityClassifiedAs(argattr) & !IsAbilityClassifiedAs(argattr1) & AbilityMaxRange() == 1)
            //{
            //    string argctype_Renamed = "アビリティ";
            //    CombinationPartner(argctype_Renamed, a, partners, t.x, t.y);
            //    if (Information.UBound(partners) == 0)
            //    {
            //        IsTargetWithinAbilityRangeRet = false;
            //        return IsTargetWithinAbilityRangeRet;
            //    }
            //}

            //return IsTargetWithinAbilityRangeRet;
        }

        // 移動を併用した場合にユニット t がアビリティ a の射程範囲内にいるかをチェック
        public bool IsTargetReachableForAbility(Unit t)
        {
            return true;
            // TODO Impl
            //bool IsTargetReachableForAbilityRet = default;
            //int i, j;
            //int max_range;
            //IsTargetReachableForAbilityRet = true;
            //// 移動範囲から敵に攻撃が届くかをチェック
            //max_range = AbilityMaxRange();
            //var loopTo = GeneralLib.MinLng(t.x + max_range, Map.MapWidth);
            //for (i = GeneralLib.MaxLng(t.x - max_range, 1); i <= loopTo; i++)
            //{
            //    var loopTo1 = GeneralLib.MinLng(t.y + (max_range - Math.Abs((t.x - i))), Map.MapHeight);
            //    for (j = GeneralLib.MaxLng(t.y - (max_range - Math.Abs((t.x - i))), 1); j <= loopTo1; j++)
            //    {
            //        if (!Map.MaskData[i, j])
            //        {
            //            return IsTargetReachableForAbilityRet;
            //        }
            //    }
            //}

            //IsTargetReachableForAbilityRet = false;
            //return IsTargetReachableForAbilityRet;
        }

        // アビリティの残り使用回数
        public int Stock()
        {
            int StockRet = default;
            StockRet = (int)(dblStock * MaxStock());
            return StockRet;
        }

        // アビリティの最大使用回数
        public int MaxStock()
        {
            int MaxStockRet = default;
            if (Unit.BossRank > 0)
            {
                MaxStockRet = (int)(Data.Stock * (5 + Unit.BossRank) / 5d);
            }
            else
            {
                MaxStockRet = Data.Stock;
            }

            return MaxStockRet;
        }

        // アビリティの残り使用回数を設定
        public void SetStock(int new_stock)
        {
            if (new_stock < 0)
            {
                dblStock = 0d;
            }
            else if (MaxStock() > 0)
            {
                dblStock = new_stock / (double)MaxStock();
            }
            else
            {
                dblStock = 1d;
            }
        }

        // 合体技アビリティに必要なパートナーが見つかるか？
        public bool IsCombinationAbilityAvailable(bool check_formation = false)
        {
            throw new NotImplementedException();
            //bool IsCombinationAbilityAvailableRet = default;
            //Unit[] partners;
            //partners = new Unit[1];
            //string argattr = "Ｍ";
            //if (Status == "待機" | string.IsNullOrEmpty(Map.MapFileName))
            //{
            //    // 出撃時以外は相手が仲間にいるだけでＯＫ
            //    string argctype_Renamed = "アビリティ";
            //    CombinationPartner(argctype_Renamed, a, partners, x, y);
            //}
            //else if (AbilityMaxRange(a) == 1 & !IsAbilityClassifiedAs(a, argattr))
            //{
            //    // 射程１の場合は自分の周りのいずれかの味方ユニットに対して合体技が使えればＯＫ
            //    if (x > 1)
            //    {
            //        if (Map.MapDataForUnit[x - 1, y] is object)
            //        {
            //            if (IsAlly(Map.MapDataForUnit[x - 1, y]))
            //            {
            //                string argctype_Renamed2 = "アビリティ";
            //                CombinationPartner(argctype_Renamed2, a, partners, (x - 1), y, check_formation);
            //            }
            //        }
            //    }

            //    if (Information.UBound(partners) == 0)
            //    {
            //        if (x < Map.MapWidth)
            //        {
            //            if (Map.MapDataForUnit[x + 1, y] is object)
            //            {
            //                if (IsAlly(Map.MapDataForUnit[x + 1, y]))
            //                {
            //                    string argctype_Renamed3 = "アビリティ";
            //                    CombinationPartner(argctype_Renamed3, a, partners, (x + 1), y, check_formation);
            //                }
            //            }
            //        }
            //    }

            //    if (Information.UBound(partners) == 0)
            //    {
            //        if (y > 1)
            //        {
            //            if (Map.MapDataForUnit[x, y - 1] is object)
            //            {
            //                if (IsAlly(Map.MapDataForUnit[x, y - 1]))
            //                {
            //                    string argctype_Renamed4 = "アビリティ";
            //                    CombinationPartner(argctype_Renamed4, a, partners, x, (y - 1), check_formation);
            //                }
            //            }
            //        }
            //    }

            //    if (Information.UBound(partners) == 0)
            //    {
            //        if (y > Map.MapHeight)
            //        {
            //            if (Map.MapDataForUnit[x, y + 1] is object)
            //            {
            //                if (IsAlly(Map.MapDataForUnit[x, y + 1]))
            //                {
            //                    string argctype_Renamed5 = "アビリティ";
            //                    CombinationPartner(argctype_Renamed5, a, partners, x, (y + 1), check_formation);
            //                }
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    // 射程２以上の場合は自分の周りにパートナーがいればＯＫ
            //    string argctype_Renamed1 = "アビリティ";
            //    CombinationPartner(argctype_Renamed1, a, partners, x, y, check_formation);
            //}

            //// 条件を満たすパートナーの組が見つかったか判定
            //if (Information.UBound(partners) > 0)
            //{
            //    IsCombinationAbilityAvailableRet = true;
            //}
            //else
            //{
            //    IsCombinationAbilityAvailableRet = false;
            //}

            //return IsCombinationAbilityAvailableRet;
        }

        public bool IdDisplayFor(AbilityListMode mode)
        {
            // Disableコマンドで使用不可にされた武器と使用できない合体技は表示しない
            // 必要技能を満たさない武器は表示しない
            var baseCondition = !Unit.IsDisabled(Data.Name)
                && IsAbilityMastered()
                // XXX 条件によって check_formation 見直す
                && !(IsAbilityClassifiedAs("合") && IsCombinationAbilityAvailable(true))
                ;
            return baseCondition;
        }

        public bool CanUseFor(AbilityListMode mode, Unit targetUnit)
        {
            var display = IdDisplayFor(mode);
            switch (mode)
            {
                case AbilityListMode.List:
                    return display;

                case AbilityListMode.BeforeMove:
                    return display && IsAbilityUseful("移動前");

                case AbilityListMode.AfterMove:
                    return display && IsAbilityUseful("移動後");

                default:
                    throw new NotSupportedException();
            }
        }
    }

    public enum AbilityListMode
    {
        List,
        BeforeMove,
        AfterMove,
    }
}
