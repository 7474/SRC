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
            //if (IsAbilityClassifiedAs("術"))
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
            //if (IsAbilityClassifiedAs("技"))
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
            //    if (IsAbilityClassifiedAs("霊"))
            //    {
            //        if (withBlock.Plana < AbilityLevel("霊") * 5d)
            //        {
            //            return IsAbilityAvailableRet;
            //        }
            //    }
            //    else if (IsAbilityClassifiedAs("プ"))
            //    {
            //        if (withBlock.Plana < AbilityLevel("プ") * 5d)
            //        {
            //            return IsAbilityAvailableRet;
            //        }
            //    }
            //}

            //// 属性使用不能状態
            //if (ConditionLifetime("オーラ使用不能") > 0)
            //{
            //    if (IsAbilityClassifiedAs("オ"))
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //if (ConditionLifetime("超能力使用不能") > 0)
            //{
            //    if (IsAbilityClassifiedAs("超"))
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //if (ConditionLifetime("同調率使用不能") > 0)
            //{
            //    if (IsAbilityClassifiedAs("シ"))
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //if (ConditionLifetime("超感覚使用不能") > 0)
            //{
            //    if (IsAbilityClassifiedAs("サ"))
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //if (ConditionLifetime("知覚強化使用不能") > 0)
            //{
            //    if (IsAbilityClassifiedAs("サ"))
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //if (ConditionLifetime("霊力使用不能") > 0)
            //{
            //    if (IsAbilityClassifiedAs("霊"))
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //if (ConditionLifetime("術使用不能") > 0)
            //{
            //    if (IsAbilityClassifiedAs("術"))
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //if (ConditionLifetime("技使用不能") > 0)
            //{
            //    if (IsAbilityClassifiedAs("技"))
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

            //            if (GeneralLib.InStrNotNest(Data.Class, Strings.Left(localCondition(), Strings.Len(localCondition1()) - 6)) > 0)
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
            //    if (IsAbilityClassifiedAs("銭"))
            //    {
            //        if (SRC.Money < GeneralLib.MaxLng(AbilityLevel("銭"), 1) * Value / 10)
            //        {
            //            return IsAbilityAvailableRet;
            //        }
            //    }
            //}

            //// 移動不能時には移動型マップアビリティは使用不能
            //if (IsConditionSatisfied("移動不能"))
            //{
            //    if (IsAbilityClassifiedAs("Ｍ移"))
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //// 術及び音声技は沈黙状態では使用不能
            //if (IsConditionSatisfied("沈黙"))
            //{
            //    {
            //        var withBlock1 = MainPilot();
            //        if (IsSpellAbility() | IsAbilityClassifiedAs("音"))
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
            //if (IsAbilityClassifiedAs("合"))
            //{
            //    if (!IsCombinationAbilityAvailable())
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //// この地形で変形できるか？
            //if (IsAbilityClassifiedAs("変"))
            //{
            //    if (IsFeatureAvailable("変形技"))
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
            //    else if (IsFeatureAvailable("ノーマルモード"))
            //    {
            //        string localLIndex2() { object argIndex1 = "ノーマルモード"; string arglist = FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //        Unit localOtherForm1() { object argIndex1 = (object)hs89e8cc23250142a2b15ab7b087ddcbd2(); var ret = OtherForm(argIndex1); return ret; }

            //        if (!localOtherForm1().IsAbleToEnter(x, y))
            //        {
            //            return IsAbilityAvailableRet;
            //        }
            //    }

            //    if (IsConditionSatisfied("形態固定"))
            //    {
            //        return IsAbilityAvailableRet;
            //    }

            //    if (IsConditionSatisfied("機体固定"))
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //// 瀕死時限定
            //if (IsAbilityClassifiedAs("瀕"))
            //{
            //    if (HP > MaxHP / 4)
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //// 自動チャージアビリティを充填中
            //if (IsConditionSatisfied(AbilityNickname() + "充填中"))
            //{
            //    return IsAbilityAvailableRet;
            //}
            //// 共有武器＆アビリティが充填中の場合も使用不可
            //int lv;
            //if (IsAbilityClassifiedAs("共"))
            //{
            //    lv = AbilityLevel("共");
            //    var loopTo2 = CountAbility();
            //    for (i = 1; i <= loopTo2; i++)
            //    {
            //        if (IsAbilityClassifiedAs(i, "共"))
            //        {
            //            if (lv == AbilityLevel(i, "共"))
            //            {
            //                if (IsConditionSatisfied(AbilityNickname(i) + "充填中"))
            //                {
            //                    return IsAbilityAvailableRet;
            //                }
            //            }
            //        }
            //    }

            //    var loopTo3 = CountAbility();
            //    for (i = 1; i <= loopTo3; i++)
            //    {
            //        if (IsAbilityClassifiedAs(i, "共"))
            //        {
            //            if (lv == AbilityLevel(i, "共"))
            //            {
            //                if (IsConditionSatisfied(AbilityNickname(i) + "充填中"))
            //                {
            //                    return IsAbilityAvailableRet;
            //                }
            //            }
            //        }
            //    }
            //}

            //// 使用禁止
            //if (Conversions.ToInteger(IsAbilityClassifiedAs("禁")) > 0)
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
            //if (IsAbilityClassifiedAs("Ｃ"))
            //{
            //    if (!IsConditionSatisfied("チャージ完了"))
            //    {
            //        return IsAbilityAvailableRet;
            //    }
            //}

            //var loopTo4 = ad.CountEffect();
            //for (i = 1; i <= loopTo4; i++)
            //{
            //    if (ad.EffectType(i) == "召喚")
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
            //                                if (withBlock3.Feature(k) == "合体")
            //                                {
            //                                    string localFeatureData2() { object argIndex1 = k; var ret = withBlock3.FeatureData(argIndex1); return ret; }

            //                                    uname = GeneralLib.LIndex(localFeatureData2(), 2);
            //                                    if (SRC.UList.IsDefined(uname))
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

            //        pname = localItem1().FeatureData("追加パイロット");
            //        bool localIsDefined1() { object argIndex1 = pname; var ret = SRC.PDList.IsDefined(argIndex1); return ret; }

            //        if (!localIsDefined1())
            //        {
            //            return IsAbilityAvailableRet;
            //        }

            //        // 召喚するユニットに乗るパイロットが汎用パイロットでもザコパイロットでも
            //        // ない場合、そのユニットが既に出撃中であれば使用不可
            //        if (Strings.InStr(pname, "(汎用)") == 0 & Strings.InStr(pname, "(ザコ)") == 0)
            //        {
            //            if (SRC.PList.IsDefined(pname))
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
            //    if (ad.EffectType(i) == "変身")
            //    {
            //        // 自分を変身させる場合
            //        if (this.Data.MaxRange == 0)
            //        {
            //            // ノーマルモードを持つユニットは変身できない
            //            // (変身からの復帰が出来ないため)
            //            if (IsFeatureAvailable("ノーマルモード"))
            //            {
            //                return IsAbilityAvailableRet;
            //            }

            //            // その場所で変身可能か？
            //            string localEffectData2() { object argIndex1 = i; var ret = Data.EffectData(argIndex1); return ret; }

            //            string localLIndex3() { string arglist = hsc2f37474313640f6843767ac6d51a5dd(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //            {
            //                var withBlock5 = OtherForm(localLIndex3());
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
            //    if (IsAbilityClassifiedAs("Ｐ"))
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
            //    if (IsAbilityClassifiedAs("Ｑ"))
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
            //IsAbilityMasteredRet = IsNecessarySkillSatisfied(Data.NecessarySkill, p: null);
            //return IsAbilityMasteredRet;
        }

        // アビリティ a の必要条件を満たしているか。
        public bool IsAbilityEnabled()
        {
            return true;
            // TODO Impl
            //bool IsAbilityEnabledRet = default;
            //IsAbilityEnabledRet = IsNecessarySkillSatisfied(Data.NecessaryCondition, p: null);
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
            //if (IsAbilityClassifiedAs("Ｍ投") | IsAbilityClassifiedAs("Ｍ投"1) | IsAbilityClassifiedAs("Ｍ投"2))
            //{
            //    IsAbilityUsefulRet = true;
            //    return IsAbilityUsefulRet;
            //}

            //// 召喚は常に有用
            //var loopTo = Data.CountEffect();
            //for (i = 1; i <= loopTo; i++)
            //{
            //    if (Data.EffectType(i) == "召喚")
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
            //        edata = Data.EffectData(i);
            //        elevel = Data.EffectLevel(i);
            //        switch (Data.EffectType(i) ?? "")
            //        {
            //            case "回復":
            //                {
            //                    if (elevel > 0d)
            //                    {
            //                        if (withBlock.HP < withBlock.MaxHP)
            //                        {
            //                            if (!withBlock.IsConditionSatisfied("ゾンビ"))
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
            //                        if (withBlock.ConditionLifetime("攻撃不能") > 0 | withBlock.ConditionLifetime("移動不能") > 0 | withBlock.ConditionLifetime("装甲劣化") > 0 | withBlock.ConditionLifetime("混乱") > 0 | withBlock.ConditionLifetime("恐怖") > 0 | withBlock.ConditionLifetime("踊り") > 0 | withBlock.ConditionLifetime("狂戦士") > 0 | withBlock.ConditionLifetime("ゾンビ") > 0 | withBlock.ConditionLifetime("回復不能") > 0 | withBlock.ConditionLifetime("石化") > 0 | withBlock.ConditionLifetime("凍結") > 0 | withBlock.ConditionLifetime("麻痺") > 0 | withBlock.ConditionLifetime("睡眠") > 0 | withBlock.ConditionLifetime("毒") > 0 | withBlock.ConditionLifetime("盲目") > 0 | withBlock.ConditionLifetime("沈黙") > 0 | withBlock.ConditionLifetime("魅了") > 0 | withBlock.ConditionLifetime("憑依") > 0 | withBlock.ConditionLifetime("オーラ使用不能") > 0 | withBlock.ConditionLifetime("超能力使用不能") > 0 | withBlock.ConditionLifetime("同調率使用不能") > 0 | withBlock.ConditionLifetime("超感覚使用不能") > 0 | withBlock.ConditionLifetime("知覚強化使用不能") > 0 | withBlock.ConditionLifetime("霊力使用不能") > 0 | withBlock.ConditionLifetime("術使用不能") > 0 | withBlock.ConditionLifetime("技使用不能") > 0)
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

            //                                    if (withBlock.ConditionLifetime(localCondition()) > 0)
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
            //                            if (withBlock.ConditionLifetime(GeneralLib.LIndex(edata, j)) > 0)
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
            //                            if (!withBlock.IsConditionSatisfied("ゾンビ"))
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

            //                        if (withBlock.IsFeatureAvailable("追加サポート"))
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
            //                            {
            //                                var withBlock2 = withBlock.Pilot(j);
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
            //                            {
            //                                var withBlock3 = withBlock.Support(j);
            //                                if (withBlock3.Morale < withBlock3.MaxMorale & withBlock3.Personality != "機械")
            //                                {
            //                                    IsAbilityEffectiveRet = true;
            //                                    return IsAbilityEffectiveRet;
            //                                }
            //                            }
            //                        }

            //                        if (withBlock.IsFeatureAvailable("追加サポート"))
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

            //                    if (!localIsConditionSatisfied() | IsAbilityClassifiedAs("除"))
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

            //                    if (!localIsConditionSatisfied1() | IsAbilityClassifiedAs("除"))
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
            //                    if (!withBlock.IsFeatureAvailable("ノーマルモード"))
            //                    {
            //                        IsAbilityEffectiveRet = true;
            //                        return IsAbilityEffectiveRet;
            //                    }

            //                    IsAbilityEffectiveRet = false;
            //                    break;
            //                }

            //            case "能力コピー":
            //                {
            //                    if (ReferenceEquals(t, this) | IsFeatureAvailable("ノーマルモード") | Conversions.ToInteger(withBlock.IsConditionSatisfied("混乱")) > 0 | withBlock.IsEnemy(this) | IsEnemy(t))
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
            //if (IsAbilityClassifiedAs("封"))
            //{
            //    if (!t.Weakness(Data.Class) & !t.Effective(Data.Class))
            //    {
            //        return IsAbilityApplicableRet;
            //    }
            //}

            //if (IsAbilityClassifiedAs("限"))
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
            //    if (!IsAbilityClassifiedAs("援"))
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

            //if (IsAbilityClassifiedAs("視"))
            //{
            //    if (t.IsConditionSatisfied("盲目"))
            //    {
            //        return IsAbilityApplicableRet;
            //    }
            //}

            //{
            //    var withBlock = t.MainPilot();
            //    if (IsAbilityClassifiedAs("対"))
            //    {
            //        // UPGRADE_WARNING: Mod に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //        if (withBlock.Level % AbilityLevel("対") != 0d)
            //        {
            //            return IsAbilityApplicableRet;
            //        }
            //    }

            //    if (IsAbilityClassifiedAs("精"))
            //    {
            //        if (withBlock.Personality == "機械")
            //        {
            //            return IsAbilityApplicableRet;
            //        }
            //    }

            //    if (IsAbilityClassifiedAs("♂"))
            //    {
            //        if (withBlock.Sex != "男性")
            //        {
            //            return IsAbilityApplicableRet;
            //        }
            //    }

            //    if (IsAbilityClassifiedAs("♀"))
            //    {
            //        if (withBlock.Sex != "女性")
            //        {
            //            return IsAbilityApplicableRet;
            //        }
            //    }
            //}

            //// 修理不可
            //if (t.IsFeatureAvailable("修理不可"))
            //{
            //    var loopTo = Data.CountEffect();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        if (Data.EffectType(i) == "回復")
            //        {
            //            break;
            //        }
            //    }

            //    if (i <= Data.CountEffect())
            //    {
            //        var loopTo1 = Conversions.ToInteger(t.FeatureData("修理不可"));
            //        for (i = 2; i <= loopTo1; i++)
            //        {
            //            fname = GeneralLib.LIndex(t.FeatureData("修理不可"), i);
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
            //if (IsAbilityClassifiedAs("合") & !IsAbilityClassifiedAs("Ｍ") & AbilityMaxRange() == 1)
            //{
            //    CombinationPartner("アビリティ", a, partners, t.x, t.y);
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
            //if (Status == "待機" | string.IsNullOrEmpty(Map.MapFileName))
            //{
            //    // 出撃時以外は相手が仲間にいるだけでＯＫ
            //    CombinationPartner("アビリティ", a, partners, x, y);
            //}
            //else if (AbilityMaxRange(a) == 1 & !IsAbilityClassifiedAs(a, "Ｍ"))
            //{
            //    // 射程１の場合は自分の周りのいずれかの味方ユニットに対して合体技が使えればＯＫ
            //    if (x > 1)
            //    {
            //        if (Map.MapDataForUnit[x - 1, y] is object)
            //        {
            //            if (IsAlly(Map.MapDataForUnit[x - 1, y]))
            //            {
            //                CombinationPartner("アビリティ", a, partners, (x - 1), y, check_formation);
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
            //                    CombinationPartner("アビリティ", a, partners, (x + 1), y, check_formation);
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
            //                    CombinationPartner("アビリティ", a, partners, x, (y - 1), check_formation);
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
            //                    CombinationPartner("アビリティ", a, partners, x, (y + 1), check_formation);
            //                }
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    // 射程２以上の場合は自分の周りにパートナーがいればＯＫ
            //    CombinationPartner("アビリティ", a, partners, x, y, check_formation);
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