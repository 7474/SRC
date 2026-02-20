// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
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
            bool IsSpellAbilityRet = default;
            int i;
            string nskill;
            if (IsAbilityClassifiedAs("術"))
            {
                IsSpellAbilityRet = true;
                return IsSpellAbilityRet;
            }

            {
                var p = Unit.MainPilot();
                var loopTo = GeneralLib.LLength(Data.NecessarySkill);
                for (i = 1; i <= loopTo; i++)
                {
                    nskill = GeneralLib.LIndex(Data.NecessarySkill, i);
                    if (Strings.InStr(nskill, "Lv") > 0)
                    {
                        nskill = Strings.Left(nskill, Strings.InStr(nskill, "Lv") - 1);
                    }

                    if (p.SkillType(nskill) == "術")
                    {
                        IsSpellAbilityRet = true;
                        return IsSpellAbilityRet;
                    }
                }
            }

            return IsSpellAbilityRet;
        }

        // アビリティ a が技かどうか
        public bool IsFeatAbility()
        {
            bool IsFeatAbilityRet = default;
            int i;
            string nskill;
            if (IsAbilityClassifiedAs("技"))
            {
                IsFeatAbilityRet = true;
                return IsFeatAbilityRet;
            }

            {
                var p = Unit.MainPilot();
                var loopTo = GeneralLib.LLength(Data.NecessarySkill);
                for (i = 1; i <= loopTo; i++)
                {
                    nskill = GeneralLib.LIndex(Data.NecessarySkill, i);
                    if (Strings.InStr(nskill, "Lv") > 0)
                    {
                        nskill = Strings.Left(nskill, Strings.InStr(nskill, "Lv") - 1);
                    }

                    if (p.SkillType(nskill) == "技")
                    {
                        IsFeatAbilityRet = true;
                        return IsFeatAbilityRet;
                    }
                }
            }

            return IsFeatAbilityRet;
        }

        // アビリティ a が使用可能かどうか
        // ref_mode はユニットの状態（移動前、移動後）を示す
        public bool IsAbilityAvailable(string ref_mode)
        {
            bool IsAbilityAvailableRet = default;
            int j, i, k;
            AbilityData ad;
            string uname, pname;
            Unit u;
            IsAbilityAvailableRet = false;
            ad = Data;

            // イベントコマンド「Disable」
            if (Unit.IsDisabled(ad.Name))
            {
                return IsAbilityAvailableRet;
            }

            // パイロットが乗っていなければ常に使用可能と判定
            if (Unit.CountPilot() == 0)
            {
                IsAbilityAvailableRet = true;
                return IsAbilityAvailableRet;
            }

            // 必要技能
            if (!IsAbilityMastered())
            {
                return IsAbilityAvailableRet;
            }

            // 必要条件
            if (!IsAbilityEnabled())
            {
                return IsAbilityAvailableRet;
            }

            // ステータス表示では必要技能だけ満たしていればＯＫ
            if (ref_mode == "インターミッション" || string.IsNullOrEmpty(ref_mode))
            {
                IsAbilityAvailableRet = true;
                return IsAbilityAvailableRet;
            }

            {
                var p = Unit.MainPilot();
                // 必要気力
                if (ad.NecessaryMorale > 0)
                {
                    if (p.Morale < ad.NecessaryMorale)
                    {
                        return IsAbilityAvailableRet;
                    }
                }

                // 霊力消費アビリティ
                if (IsAbilityClassifiedAs("霊"))
                {
                    if (p.Plana < AbilityLevel("霊") * 5d)
                    {
                        return IsAbilityAvailableRet;
                    }
                }
                else if (IsAbilityClassifiedAs("プ"))
                {
                    if (p.Plana < AbilityLevel("プ") * 5d)
                    {
                        return IsAbilityAvailableRet;
                    }
                }
            }

            // 属性使用不能状態
            if (Unit.ConditionLifetime("オーラ使用不能") > 0)
            {
                if (IsAbilityClassifiedAs("オ"))
                {
                    return IsAbilityAvailableRet;
                }
            }

            if (Unit.ConditionLifetime("超能力使用不能") > 0)
            {
                if (IsAbilityClassifiedAs("超"))
                {
                    return IsAbilityAvailableRet;
                }
            }

            if (Unit.ConditionLifetime("同調率使用不能") > 0)
            {
                if (IsAbilityClassifiedAs("シ"))
                {
                    return IsAbilityAvailableRet;
                }
            }

            if (Unit.ConditionLifetime("超感覚使用不能") > 0)
            {
                if (IsAbilityClassifiedAs("サ"))
                {
                    return IsAbilityAvailableRet;
                }
            }

            if (Unit.ConditionLifetime("知覚強化使用不能") > 0)
            {
                if (IsAbilityClassifiedAs("サ"))
                {
                    return IsAbilityAvailableRet;
                }
            }

            if (Unit.ConditionLifetime("霊力使用不能") > 0)
            {
                if (IsAbilityClassifiedAs("霊"))
                {
                    return IsAbilityAvailableRet;
                }
            }

            if (Unit.ConditionLifetime("術使用不能") > 0)
            {
                if (IsAbilityClassifiedAs("術"))
                {
                    return IsAbilityAvailableRet;
                }
            }

            if (Unit.ConditionLifetime("技使用不能") > 0)
            {
                if (IsAbilityClassifiedAs("技"))
                {
                    return IsAbilityAvailableRet;
                }
            }
            foreach (var cond in Unit.Conditions)
            {
                if (Strings.Len(cond.Name) > 6)
                {
                    if (Strings.Right(cond.Name, 6) == "属性使用不能")
                    {
                        if (GeneralLib.InStrNotNest(Data.Class, Strings.Left(cond.Name, Strings.Len(cond.Name) - 6)) > 0)
                        {
                            return IsAbilityAvailableRet;
                        }
                    }
                }
            }

            // 弾数が足りるか
            if (MaxStock() > 0)
            {
                if (Stock() < 1)
                {
                    return IsAbilityAvailableRet;
                }
            }

            // ＥＮが足りるか
            if (ad.ENConsumption > 0)
            {
                if (Unit.EN < AbilityENConsumption())
                {
                    return IsAbilityAvailableRet;
                }
            }

            // お金が足りるか……
            if (Unit.Party == "味方")
            {
                if (IsAbilityClassifiedAs("銭"))
                {
                    if (SRC.Money < GeneralLib.MaxLng((int)AbilityLevel("銭"), 1) * Unit.Value / 10)
                    {
                        return IsAbilityAvailableRet;
                    }
                }
            }

            // 移動不能時には移動型マップアビリティは使用不能
            if (Unit.IsConditionSatisfied("移動不能"))
            {
                if (IsAbilityClassifiedAs("Ｍ移"))
                {
                    return IsAbilityAvailableRet;
                }
            }

            // 術及び音声技は沈黙状態では使用不能
            if (Unit.IsConditionSatisfied("沈黙"))
            {
                {
                    var withBlock1 = Unit.MainPilot();
                    if (IsSpellAbility() || IsAbilityClassifiedAs("音"))
                    {
                        return IsAbilityAvailableRet;
                    }
                }
            }

            // 術は狂戦士状態では使用不能
            if (Unit.IsConditionSatisfied("狂戦士"))
            {
                {
                    var withBlock2 = Unit.MainPilot();
                    if (IsSpellAbility())
                    {
                        return IsAbilityAvailableRet;
                    }
                }
            }

            // 合体技の処理
            if (IsAbilityClassifiedAs("合"))
            {
                if (!IsCombinationAbilityAvailable())
                {
                    return IsAbilityAvailableRet;
                }
            }

            // この地形で変形できるか？
            if (IsAbilityClassifiedAs("変"))
            {
                if (Unit.IsConditionSatisfied("形態固定"))
                {
                    return false;
                }

                if (Unit.IsConditionSatisfied("機体固定"))
                {
                    return false;
                }

                // XXX 変形技が妥当に定義されていなかった場合
                if (Unit.IsFeatureAvailable("変形技"))
                {
                    foreach (var feature in Unit.Features
                        .Where(x => x.Name == "変形技")
                        .Where(x => x.DataL.FirstOrDefault() == Data.Name))
                    {
                        if (!Unit.OtherForm(feature.DataL.Skip(1).FirstOrDefault()).IsAbleToEnter(Unit.x, Unit.y))
                        {
                            return false;
                        }
                    }
                }
                else if (Unit.IsFeatureAvailable("ノーマルモード"))
                {
                    if (!Unit.OtherForm(GeneralLib.LIndex(Unit.FeatureData("ノーマルモード"), 1)).IsAbleToEnter(Unit.x, Unit.y))
                    {
                        return false;
                    }
                }

                if (Unit.IsConditionSatisfied("形態固定"))
                {
                    return IsAbilityAvailableRet;
                }

                if (Unit.IsConditionSatisfied("機体固定"))
                {
                    return IsAbilityAvailableRet;
                }
            }

            // 瀕死時限定
            if (IsAbilityClassifiedAs("瀕"))
            {
                if (Unit.HP > Unit.MaxHP / 4)
                {
                    return IsAbilityAvailableRet;
                }
            }

            // 自動チャージアビリティを充填中
            if (Unit.IsConditionSatisfied(AbilityNickname() + "充填中"))
            {
                return IsAbilityAvailableRet;
            }
            // 共有武器＆アビリティが充填中の場合も使用不可
            if (IsAbilityClassifiedAs("共"))
            {
                int lv = (int)AbilityLevel("共");
                if (Unit.Weapons
                    .Where(x => x.IsWeaponClassifiedAs("共"))
                    .Where(x => x.WeaponLevel("共") == lv)
                    .Where(x => Unit.IsConditionSatisfied(x.WeaponNickname() + "充填中"))
                    .Any())
                {
                    return false;
                }

                if (Unit.Abilities
                    .Where(x => x.IsAbilityClassifiedAs("共"))
                    .Where(x => x.AbilityLevel("共") == lv)
                    .Where(x => Unit.IsConditionSatisfied(x.AbilityNickname() + "充填中"))
                    .Any())
                {
                    return false;
                }
            }

            // 使用禁止
            if (IsAbilityClassifiedAs("禁"))
            {
                return IsAbilityAvailableRet;
            }

            // チャージ判定であればここまででＯＫ
            if (ref_mode == "チャージ")
            {
                IsAbilityAvailableRet = true;
                return IsAbilityAvailableRet;
            }

            // チャージ式アビリティ
            if (IsAbilityClassifiedAs("Ｃ"))
            {
                if (!Unit.IsConditionSatisfied("チャージ完了"))
                {
                    return IsAbilityAvailableRet;
                }
            }

            foreach (var effect in ad.Effects.Where(x => x.EffectType == "召喚"))
            {
                // 召喚は既に召喚を行っている場合には不可能
                foreach (var su in Unit.Servants)
                {
                    var cf = su.CurrentForm();
                    switch (cf.Status ?? "")
                    {
                        case "出撃":
                        case "格納":
                            {
                                // 使用不可
                                return IsAbilityAvailableRet;
                            }

                        case "旧主形態":
                        case "旧形態":
                            {
                                // 合体後の形態が出撃中なら使用不可
                                foreach (var cffd in cf.Features.Where(x => x.Name == "合体"))
                                {
                                    uname = GeneralLib.LIndex(cffd.Data, 2);
                                    if (SRC.UList.IsDefined(uname))
                                    {
                                        var cu = SRC.UList.Item(uname).CurrentForm();
                                        if (cu.Status == "出撃" || cu.Status == "格納")
                                        {
                                            return IsAbilityAvailableRet;
                                        }
                                    }
                                }

                                break;
                            }
                    }

                    // 召喚ユニットのデータがちゃんと定義されているかチェック
                    if (!SRC.UDList.IsDefined(effect.Data))
                    {
                        return IsAbilityAvailableRet;
                    }
                    pname = SRC.UDList.Item(effect.Data).FeatureData("追加パイロット");
                    if (!SRC.PDList.IsDefined(pname))
                    {
                        return IsAbilityAvailableRet;
                    }

                    // 召喚するユニットに乗るパイロットが汎用パイロットでもザコパイロットでも
                    // ない場合、そのユニットが既に出撃中であれば使用不可
                    if (Strings.InStr(pname, "(汎用)") == 0 && Strings.InStr(pname, "(ザコ)") == 0)
                    {
                        if (SRC.PList.IsDefined(pname))
                        {
                            u = SRC.PList.Item(pname).Unit;
                            if (u is object)
                            {
                                if (u.Status == "出撃" || u.Status == "格納")
                                {
                                    return IsAbilityAvailableRet;
                                }
                            }
                        }
                    }
                }
            }

            if (ref_mode == "ステータス")
            {
                IsAbilityAvailableRet = true;
                return IsAbilityAvailableRet;
            }

            foreach (var effect in ad.Effects.Where(x => x.EffectType == "変身"))
            {
                // 自分を変身させる場合
                if (Data.MaxRange == 0)
                {
                    // ノーマルモードを持つユニットは変身できない
                    // (変身からの復帰が出来ないため)
                    if (Unit.IsFeatureAvailable("ノーマルモード"))
                    {
                        return IsAbilityAvailableRet;
                    }
                    var of = Unit.OtherForm(GeneralLib.LIndex(effect.Data, 1));
                    if (!of.IsAbleToEnter(Unit.x, Unit.y))
                    {
                        return IsAbilityAvailableRet;
                    }
                }
            }

            if (ref_mode == "移動前")
            {
                IsAbilityAvailableRet = true;
                return IsAbilityAvailableRet;
            }
            IsAbilityAvailableRet = CanUseAfterMove();

            return IsAbilityAvailableRet;
        }

        public bool CanUseAfterMove()
        {
            bool IsAbilityAvailableRet;
            if (AbilityMaxRange() > 1 || AbilityMaxRange() == 0)
            {
                if (IsAbilityClassifiedAs("Ｐ"))
                {
                    IsAbilityAvailableRet = true;
                }
                else
                {
                    IsAbilityAvailableRet = false;
                }
            }
            else
            {
                if (IsAbilityClassifiedAs("Ｑ"))
                {
                    IsAbilityAvailableRet = false;
                }
                else
                {
                    IsAbilityAvailableRet = true;
                }
            }

            return IsAbilityAvailableRet;
        }

        // アビリティ a の必要技能を満たしているか。
        public bool IsAbilityMastered()
        {
            return Unit.IsNecessarySkillSatisfied(Data.NecessarySkill, p: null);
        }

        // アビリティ a の必要条件を満たしているか。
        public bool IsAbilityEnabled()
        {
            return Unit.IsNecessarySkillSatisfied(Data.NecessaryCondition, p: null);
        }

        // アビリティが使用可能であり、かつ射程内に有効なターゲットがいるかどうか
        public bool IsAbilityUseful(string ref_mode)
        {
            // アビリティが使用可能か？
            if (!IsAbilityAvailable(ref_mode))
            {
                return false;
            }

            // 投下型マップアビリティと扇型マップアビリティは特殊なので判定ができない
            // 移動型マップアビリティは移動手段として使うことを考慮
            if (IsAbilityClassifiedAs("Ｍ投") || IsAbilityClassifiedAs("Ｍ扇") || IsAbilityClassifiedAs("Ｍ移"))
            {
                return true;
            }

            // 召喚は常に有用
            if (Data.Effects.Any(x => x.EffectType == "召喚"))
            {
                return true;
            }

            var min_range = AbilityMinRange();
            var max_range = AbilityMaxRange();

            // 使用する相手がいるか検索
            var loopTo1 = GeneralLib.MinLng(Unit.x + max_range, Map.MapWidth);
            for (var i = GeneralLib.MaxLng(Unit.x - max_range, 1); i <= loopTo1; i++)
            {
                var loopTo2 = GeneralLib.MinLng(Unit.y + max_range, Map.MapHeight);
                for (var j = GeneralLib.MaxLng(Unit.y - max_range, 1); j <= loopTo2; j++)
                {
                    if ((Math.Abs((Unit.x - i)) + Math.Abs((Unit.y - j))) > max_range)
                    {
                        goto NextLoop;
                    }

                    if (Map.MapDataForUnit[i, j] is null)
                    {
                        goto NextLoop;
                    }

                    if (IsAbilityEffective(Map.MapDataForUnit[i, j]))
                    {
                        return true;
                    }

                NextLoop:
                    ;
                }
            }

            return false;
        }

        // アビリティがターゲットtに対して有効(役に立つ)かどうか
        public bool IsAbilityEffective(Unit t)
        {
            // 敵には使用できない。
            // IsEnemyでは魅了等がかかった味方ユニットを敵と認識してしまうので
            // ここでは独自の判定基準を使う
            switch (Unit.Party ?? "")
            {
                case "味方":
                case "ＮＰＣ":
                    if (t.Party != "味方" && t.Party0 != "味方" && t.Party != "ＮＰＣ" && t.Party0 != "ＮＰＣ")
                    {
                        return false;
                    }
                    break;
                default:
                    if ((t.Party ?? "") != (Unit.Party ?? "") && (t.Party0 ?? "") != (Unit.Party ?? ""))
                    {
                        return false;
                    }
                    break;
            }

            // アビリティがそのユニットに対して適用可能か？
            if (!IsAbilityApplicable(t))
            {
                return false;
            }

            bool result = true;
            foreach (var effect in Data.Effects)
            {
                string edata = effect.Data;
                double elevel = effect.Level;
                switch (effect.EffectType ?? "")
                {
                    case "回復":
                        if (elevel > 0d)
                        {
                            if (t.HP < t.MaxHP)
                            {
                                if (!t.IsConditionSatisfied("ゾンビ"))
                                {
                                    return true;
                                }
                            }
                            result = false;
                        }
                        else
                        {
                            // ＨＰを減少させるためのアビリティというのは有り得るので
                            return true;
                        }
                        break;

                    case "治癒":
                        if (string.IsNullOrEmpty(edata))
                        {
                            if (t.ConditionLifetime("攻撃不能") > 0 || t.ConditionLifetime("移動不能") > 0 || t.ConditionLifetime("装甲劣化") > 0 || t.ConditionLifetime("混乱") > 0 || t.ConditionLifetime("恐怖") > 0 || t.ConditionLifetime("踊り") > 0 || t.ConditionLifetime("狂戦士") > 0 || t.ConditionLifetime("ゾンビ") > 0 || t.ConditionLifetime("回復不能") > 0 || t.ConditionLifetime("石化") > 0 || t.ConditionLifetime("凍結") > 0 || t.ConditionLifetime("麻痺") > 0 || t.ConditionLifetime("睡眠") > 0 || t.ConditionLifetime("毒") > 0 || t.ConditionLifetime("盲目") > 0 || t.ConditionLifetime("沈黙") > 0 || t.ConditionLifetime("魅了") > 0 || t.ConditionLifetime("憑依") > 0 || t.ConditionLifetime("オーラ使用不能") > 0 || t.ConditionLifetime("超能力使用不能") > 0 || t.ConditionLifetime("同調率使用不能") > 0 || t.ConditionLifetime("超感覚使用不能") > 0 || t.ConditionLifetime("知覚強化使用不能") > 0 || t.ConditionLifetime("霊力使用不能") > 0 || t.ConditionLifetime("術使用不能") > 0 || t.ConditionLifetime("技使用不能") > 0)
                            {
                                return true;
                            }

                            foreach (var cnd in t.Conditions)
                            {
                                if (cnd.Name.Length > 6 && Strings.Right(cnd.Name, 6) == "属性使用不能")
                                {
                                    if (t.ConditionLifetime(cnd.Name) > 0)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int j = 1; j <= GeneralLib.LLength(edata); j++)
                            {
                                if (t.ConditionLifetime(GeneralLib.LIndex(edata, j)) > 0)
                                {
                                    return true;
                                }
                            }
                        }
                        result = false;
                        break;

                    case "補給":
                        if (elevel > 0d)
                        {
                            if (t.EN < t.MaxEN)
                            {
                                if (!t.IsConditionSatisfied("ゾンビ"))
                                {
                                    return true;
                                }
                            }
                            result = false;
                        }
                        break;

                    case "霊力回復":
                    case "プラーナ回復":
                        if (elevel > 0d)
                        {
                            if (t.MainPilot().Plana < t.MainPilot().MaxPlana())
                            {
                                return true;
                            }
                            result = false;
                        }
                        break;

                    case "ＳＰ回復":
                        if (elevel > 0d)
                        {
                            if (t.MainPilot().SP < t.MainPilot().MaxSP)
                            {
                                return true;
                            }

                            foreach (var p in t.SubPilots)
                            {
                                if (p.SP < p.MaxSP)
                                {
                                    return true;
                                }
                            }

                            foreach (var p in t.Supports)
                            {
                                if (p.SP < p.MaxSP)
                                {
                                    return true;
                                }
                            }

                            if (t.IsFeatureAvailable("追加サポート"))
                            {
                                if (t.AdditionalSupport().SP < t.AdditionalSupport().MaxSP)
                                {
                                    return true;
                                }
                            }

                            result = false;
                        }
                        break;

                    case "気力増加":
                        if (elevel > 0d)
                        {
                            var mainPilot = t.MainPilot();
                            if (mainPilot.Morale < mainPilot.MaxMorale && mainPilot.Personality != "機械")
                            {
                                return true;
                            }

                            foreach (var p in t.SubPilots)
                            {
                                if (p.Morale < p.MaxMorale && p.Personality != "機械")
                                {
                                    return true;
                                }
                            }

                            foreach (var p in t.Supports)
                            {
                                if (p.Morale < p.MaxMorale && p.Personality != "機械")
                                {
                                    return true;
                                }
                            }

                            if (t.IsFeatureAvailable("追加サポート"))
                            {
                                var ap = t.AdditionalSupport();
                                if (ap.Morale < ap.MaxMorale && ap.Personality != "機械")
                                {
                                    return true;
                                }
                            }

                            result = false;
                        }
                        break;

                    case "装填":
                        if (string.IsNullOrEmpty(edata))
                        {
                            for (int j = 1; j <= t.CountWeapon(); j++)
                            {
                                var w = t.Weapon(j);
                                if (w.Bullet() < w.MaxBullet())
                                {
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            for (int j = 1; j <= t.CountWeapon(); j++)
                            {
                                var w = t.Weapon(j);
                                if (w.Bullet() < w.MaxBullet())
                                {
                                    if ((w.WeaponNickname() ?? "") == (edata ?? "") || GeneralLib.InStrNotNest(w.WeaponData.Class, edata) > 0)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        result = false;
                        break;

                    case "付加":
                        if (!t.IsConditionSatisfied(GeneralLib.LIndex(edata, 1) + "付加") || IsAbilityClassifiedAs("除"))
                        {
                            return true;
                        }
                        result = false;
                        break;

                    case "強化":
                        if (!t.IsConditionSatisfied(GeneralLib.LIndex(edata, 1) + "強化") || IsAbilityClassifiedAs("除"))
                        {
                            return true;
                        }
                        result = false;
                        break;

                    case "状態":
                        if (!t.IsConditionSatisfied(edata))
                        {
                            return true;
                        }
                        result = false;
                        break;

                    case "再行動":
                        if (Data.MaxRange == 0)
                        {
                            continue;
                        }
                        if (t.Action == 0 && t.MaxAction() > 0)
                        {
                            return true;
                        }
                        result = false;
                        break;

                    case "変身":
                        if (!t.IsFeatureAvailable("ノーマルモード"))
                        {
                            return true;
                        }
                        result = false;
                        break;

                    case "能力コピー":
                        if (ReferenceEquals(t, Unit) || Unit.IsFeatureAvailable("ノーマルモード") || t.IsConditionSatisfied("混乱") || t.IsEnemy(Unit) || Unit.IsEnemy(t))
                        {
                            result = false;
                            continue;
                        }

                        if (Strings.InStr(edata, "サイズ制限強") > 0)
                        {
                            if ((Unit.Size ?? "") != (t.Size ?? ""))
                            {
                                result = false;
                                continue;
                            }
                        }
                        else if (Strings.InStr(edata, "サイズ制限無し") == 0)
                        {
                            bool sizeIncompat = false;
                            switch (Unit.Size ?? "")
                            {
                                case "SS":
                                    sizeIncompat = t.Size == "M" || t.Size == "L" || t.Size == "LL" || t.Size == "XL";
                                    break;
                                case "S":
                                    sizeIncompat = t.Size == "L" || t.Size == "LL" || t.Size == "XL";
                                    break;
                                case "M":
                                    sizeIncompat = t.Size == "SS" || t.Size == "LL" || t.Size == "XL";
                                    break;
                                case "L":
                                    sizeIncompat = t.Size == "SS" || t.Size == "S" || t.Size == "XL";
                                    break;
                                case "LL":
                                    sizeIncompat = t.Size == "SS" || t.Size == "S" || t.Size == "M";
                                    break;
                                case "XL":
                                    sizeIncompat = t.Size == "SS" || t.Size == "S" || t.Size == "M" || t.Size == "L";
                                    break;
                            }
                            if (sizeIncompat)
                            {
                                result = false;
                                continue;
                            }
                        }

                        return true;
                }
            }

            // そもそも効果がないものは常に使用可能とみなす
            // (include等で特殊効果を定義していると仮定)
            return result;
        }

        // アビリティがターゲットtに対して適用可能かどうか
        public bool IsAbilityApplicable(Unit t)
        {
            if (IsAbilityClassifiedAs("封"))
            {
                if (!t.Weakness(Data.Class) && !t.Effective(Data.Class))
                {
                    return false;
                }
            }

            if (IsAbilityClassifiedAs("限"))
            {
                bool localWeakness()
                {
                    string argstring2 = "限";
                    string arganame = Strings.Mid(Data.Class, GeneralLib.InStrNotNest(Data.Class, argstring2) + 1);
                    var ret = t.Weakness(arganame); return ret;
                }

                bool localEffective()
                {
                    string argstring2 = "限";
                    string arganame = Strings.Mid(Data.Class, GeneralLib.InStrNotNest(Data.Class, argstring2) + 1);
                    var ret = t.Effective(arganame); return ret;
                }

                if (!localWeakness() && !localEffective())
                {
                    return false;
                }
            }

            if (Unit == t)
            {
                // 支援専用アビリティは自分には使用できない
                if (!IsAbilityClassifiedAs("援"))
                {
                    return true;
                }

                return false;
            }

            // 無効化の対象になる場合は使用出来ない
            if (t.Immune(Data.Class))
            {
                if (!t.Weakness(Data.Class) && !t.Effective(Data.Class))
                {
                    return false;
                }
            }

            if (IsAbilityClassifiedAs("視"))
            {
                if (t.IsConditionSatisfied("盲目"))
                {
                    return false;
                }
            }

            if (t.CountPilot() > 0 || t.IsFeatureAvailable("追加パイロット"))
            {
                var p = t.MainPilot();
                if (IsAbilityClassifiedAs("対"))
                {
                    if (p.Level % AbilityLevel("対") != 0d)
                    {
                        return false;
                    }
                }

                if (IsAbilityClassifiedAs("精"))
                {
                    if (p.Personality == "機械")
                    {
                        return false;
                    }
                }

                if (IsAbilityClassifiedAs("♂"))
                {
                    if (p.Sex != "男性")
                    {
                        return false;
                    }
                }

                if (IsAbilityClassifiedAs("♀"))
                {
                    if (p.Sex != "女性")
                    {
                        return false;
                    }
                }
            }

            // 修理不可
            if (t.IsFeatureAvailable("修理不可"))
            {
                if (Data.Effects.Any(x => x.Name == "回復"))
                {
                    foreach (var _fname in t.Feature("修理不可").DataL.Skip(1))
                    {
                        var fname = _fname;
                        if (Strings.Left(fname, 1) == "!")
                        {
                            fname = Strings.Mid(fname, 2);
                            if ((fname ?? "") != (AbilityNickname() ?? ""))
                            {
                                return false;
                            }
                        }
                        else if ((fname ?? "") == (AbilityNickname() ?? ""))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        // ユニット t がアビリティ a の射程範囲内にいるかをチェック
        public bool IsTargetWithinAbilityRange(Unit t)
        {
            int distance = (Math.Abs((Unit.x - t.x)) + Math.Abs((Unit.y - t.y)));

            // 最小射程チェック
            if (distance < AbilityMinRange())
            {
                return false;
            }

            // 最大射程チェック
            if (distance > AbilityMaxRange())
            {
                return false;
            }

            // 合体技で射程が１の場合は相手を囲んでいる必要がある
            if (IsAbilityClassifiedAs("合") && !IsAbilityClassifiedAs("Ｍ") && AbilityMaxRange() == 1)
            {
                var partners = CombinationPartner(t.x, t.y);
                if (partners.Count > 0)
                {
                    return false;
                }
            }

            return true;
        }

        // 移動を併用した場合にユニット t がアビリティ a の射程範囲内にいるかをチェック
        public bool IsTargetReachableForAbility(Unit t)
        {
            // 移動範囲から敵に攻撃が届くかをチェック
            var max_range = AbilityMaxRange();
            var loopTo = GeneralLib.MinLng(t.x + max_range, Map.MapWidth);
            for (var i = GeneralLib.MaxLng(t.x - max_range, 1); i <= loopTo; i++)
            {
                var loopTo1 = GeneralLib.MinLng(t.y + (max_range - Math.Abs((t.x - i))), Map.MapHeight);
                for (var j = GeneralLib.MaxLng(t.y - (max_range - Math.Abs((t.x - i))), 1); j <= loopTo1; j++)
                {
                    if (!Map.MaskData[i, j])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // アビリティの使用によるＥＮ、使用回数の消費等を行う
        public void UseAbility()
        {
            int i, lv;
            double hp_ratio, en_ratio;

            // ＥＮ消費
            if (Data.ENConsumption > 0)
            {
                Unit.EN = Unit.EN - AbilityENConsumption();
            }

            // 弾数消費
            if (Data.Stock > 0 && !IsAbilityClassifiedAs("永"))
            {
                SetStock((Stock() - 1));

                // 一斉使用
                if (IsAbilityClassifiedAs("斉"))
                {
                    foreach (var ua in Unit.Abilities)
                    {
                        ua.SetStockRate(dblStock);
                    }
                }
                else
                {
                    foreach (var ua in Unit.Abilities)
                    {
                        if (ua.IsAbilityClassifiedAs("斉"))
                        {
                            // XXX これどういう式なのかいまいち分からん
                            ua.SetStock(GeneralLib.MinLng((int)(ua.MaxStock() * dblStock + 0.49999d), ua.Stock()));
                        }
                    }
                }

                // 弾数・使用回数共有の処理
                Unit.SyncBullet();
            }

            if (IsAbilityClassifiedAs("消"))
            {
                Unit.AddCondition("消耗", 1, cdata: "");
            }

            if (IsAbilityClassifiedAs("尽"))
            {
                Unit.EN = 0;
            }

            if (IsAbilityClassifiedAs("Ｃ") && Unit.IsConditionSatisfied("チャージ完了"))
            {
                Unit.DeleteCondition("チャージ完了");
            }

            if (AbilityLevel("Ａ") > 0d)
            {
                Unit.AddCondition(AbilityNickname() + "充填中", (int)AbilityLevel("Ａ"), cdata: "");
            }

            if (IsAbilityClassifiedAs("気"))
            {
                Unit.IncreaseMorale((int)(-5 * AbilityLevel("気")));
            }

            if (IsAbilityClassifiedAs("霊"))
            {
                hp_ratio = 100 * Unit.HP / (double)Unit.MaxHP;
                en_ratio = 100 * Unit.EN / (double)Unit.MaxEN;
                Unit.MainPilot().Plana = (int)(Unit.MainPilot().Plana - 5d * AbilityLevel("霊"));
                Unit.HP = (int)(Unit.MaxHP * hp_ratio / 100d);
                Unit.EN = (int)(Unit.MaxEN * en_ratio / 100d);
            }
            else if (IsAbilityClassifiedAs("プ"))
            {
                hp_ratio = 100 * Unit.HP / (double)Unit.MaxHP;
                en_ratio = 100 * Unit.EN / (double)Unit.MaxEN;
                Unit.MainPilot().Plana = (int)(Unit.MainPilot().Plana - 5d * AbilityLevel("プ"));
                Unit.HP = (int)(Unit.MaxHP * hp_ratio / 100d);
                Unit.EN = (int)(Unit.MaxEN * en_ratio / 100d);
            }

            if (Unit.Party == "味方")
            {
                if (IsAbilityClassifiedAs("銭"))
                {
                    SRC.IncrMoney(-GeneralLib.MaxLng((int)AbilityLevel("銭"), 1) * Unit.Value / 10);
                }
            }

            if (IsAbilityClassifiedAs("失"))
            {
                Unit.HP = GeneralLib.MaxLng((int)(Unit.HP - (long)(Unit.MaxHP * AbilityLevel("失")) / 10L), 0);
            }
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

        private void SetStockRate(double new_stock)
        {
            dblStock = new_stock;
        }

        public void SetStockFull()
        {
            dblStock = 1d;
        }

        // 合体技アビリティに必要なパートナーが見つかるか？
        public bool IsCombinationAbilityAvailable(bool check_formation = false)
        {
            IList<Unit> partners;
            if (Unit.Status == "待機" || string.IsNullOrEmpty(Map.MapFileName))
            {
                // 出撃時以外は相手が仲間にいるだけでＯＫ
                partners = CombinationPartner(Unit.x, Unit.y);
            }
            else if (AbilityMaxRange() == 1 && !IsAbilityClassifiedAs("Ｍ"))
            {
                // 射程１の場合は自分の周りのいずれかの敵ユニットに対して合体技が使えればＯＫ
                partners = Map.AdjacentUnit(Unit)
                    .Where(t => Unit.IsEnemy(t))
                    .Select(t => CombinationPartner(t.x, t.y))
                    .FirstOrDefault(x => x.Count > 0) ?? new List<Unit>();
            }
            else
            {
                // 射程２以上の場合は自分の周りにパートナーがいればＯＫ
                partners = CombinationPartner(Unit.x, Unit.y, check_formation);
            }

            // 条件を満たすパートナーの組が見つかったか判定
            return partners.Count > 0;
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

        // 合体技のパートナーを探す
        public IList<Unit> CombinationPartner(int tx = 0, int ty = 0, bool check_formation = false)
        {
            // 合体技のデータを調べておく
            var cname = Data.Name;
            var cen = AbilityENConsumption();
            var cmorale = Data.NecessaryMorale;
            int cplana = 0;
            if (IsAbilityClassifiedAs("霊"))
            {
                cplana = (int)(5d * AbilityLevel("霊"));
            }
            else if (IsAbilityClassifiedAs("プ"))
            {
                cplana = (int)(5d * AbilityLevel("プ"));
            }
            var crange = AbilityMaxRange();
            return Unit.CombinationPartner(
                cname,
                cen,
                cmorale,
                cplana,
                crange,
                tx, ty, check_formation
                );
        }
    }

    public enum AbilityListMode
    {
        List,
        BeforeMove,
        AfterMove,
    }
}
