// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using Newtonsoft.Json;
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.Units;
using SRCCore.VB;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Items
{
    // 作成されたアイテムのクラス
    [JsonObject(MemberSerialization.OptIn)]
    public class Item
    {
        // 識別子
        [JsonProperty]
        public string ID;

        [JsonProperty]
        public string ItemName { get; set; }
        // アイテムデータへのポインタ
        public ItemData Data { get => SRC.IDList.Item(ItemName); set { ItemName = value?.Name; } }

        [JsonProperty]
        public string UnitId { get; set; }
        // アイテムを装備しているユニット
        public Unit Unit { get => SRC.UList.Item(UnitId); set { UnitId = value?.ID; } }
        // アイテムが存在しているか？ (RemoveItemされていないか？)
        [JsonProperty]
        public bool Exist;
        // アイテムが効力を発揮できているか？ (必要技能や武器クラス＆防具クラスを満たしているか？)
        [JsonProperty]
        public bool Activated;
        public IList<FeatureData> Features => Data.Features;

        private SRC SRC;
        private Events.Event Event => SRC.Event;
        private Expressions.Expression Expression => SRC.Expression;
        public Item(SRC src)
        {
            SRC = src;

            Exist = true;
            Activated = true;
            Data = null;
            Unit = null;
        }

        public void Restore(SRC src)
        {
            SRC = src;
        }

        // 名称
        public string Name
        {
            get
            {
                string NameRet = default;
                NameRet = Data.Name;
                return NameRet;
            }

            set
            {
                Data = SRC.IDList.Item(value);
            }
        }

        // 愛称
        public string Nickname()
        {
            string NicknameRet = default;
            Unit u;
            NicknameRet = Data.Nickname;

            // 愛称内の式置換のため、デフォルトユニットを一時的に変更する
            u = Event.SelectedUnitForEvent;
            Event.SelectedUnitForEvent = Unit;
            Expression.ReplaceSubExpression(ref NicknameRet);
            Event.SelectedUnitForEvent = u;
            return NicknameRet;
        }

        // 読み仮名
        public string KanaName()
        {
            string KanaNameRet = default;
            Unit u;
            KanaNameRet = Data.KanaName;

            // 読み仮名内の式置換のため、デフォルトユニットを一時的に変更する
            u = Event.SelectedUnitForEvent;
            Event.SelectedUnitForEvent = Unit;
            Expression.ReplaceSubExpression(ref KanaNameRet);
            Event.SelectedUnitForEvent = u;
            return KanaNameRet;
        }

        // クラス
        public string Class()
        {
            string ClassRet = default;
            ClassRet = Data.Class;
            return ClassRet;
        }

        public string Class0()
        {
            string Class0Ret = default;
            int i, n;
            Class0Ret = Data.Class;

            // 専用指定を削除
            if (Strings.Right(Class0Ret, 3) == "専用)")
            {
                n = 1;
                i = (Strings.Len(Class0Ret) - 2);
                do
                {
                    i = (i - 1);
                    switch (Strings.Mid(Class0Ret, i, 1) ?? "")
                    {
                        case "(":
                            {
                                n = (n - 1);
                                if (n == 0)
                                {
                                    Class0Ret = Strings.Left(Class0Ret, i - 1);
                                    break;
                                }

                                break;
                            }

                        case ")":
                            {
                                n = (n + 1);
                                break;
                            }
                    }
                }
                while (i > 0);
            }

            return Class0Ret;
        }

        // 装備個所
        public string Part()
        {
            string PartRet = default;
            PartRet = Data.Part;
            return PartRet;
        }

        // ＨＰ修正値
        public int HP()
        {
            int HPRet = default;
            HPRet = Data.HP;
            return HPRet;
        }

        // ＥＮ修正値
        public int EN()
        {
            int ENRet = default;
            ENRet = Data.EN;
            return ENRet;
        }

        // 装甲修正値
        public int Armor()
        {
            int ArmorRet = default;
            ArmorRet = Data.Armor;
            return ArmorRet;
        }

        // 運動性修正値
        public int Mobility()
        {
            int MobilityRet = default;
            MobilityRet = Data.Mobility;
            return MobilityRet;
        }

        // 移動力修正値
        public int Speed()
        {
            int SpeedRet = default;
            SpeedRet = Data.Speed;
            return SpeedRet;
        }

        // 特殊能力総数
        public int CountFeature()
        {
            int CountFeatureRet = default;
            CountFeatureRet = Data.CountFeature();
            return CountFeatureRet;
        }

        // 特殊能力
        public FeatureData Feature(string Index)
        {
            return Data.Feature(Index);
        }
        public FeatureData Feature(int Index)
        {
            return Data.Feature(Index);
        }

        // 特殊能力の名称
        public string FeatureName(string Index)
        {
            return Data.FeatureName(Index);
        }
        public string FeatureName(int Index)
        {
            return Data.FeatureName(Index);
        }

        // 特殊能力のレベル
        public double FeatureLevel(string Index)
        {
            double FeatureLevelRet = default;
            FeatureLevelRet = Data.FeatureLevel(Index);
            return FeatureLevelRet;
        }

        // 特殊能力のデータ
        public string FeatureData(string Index)
        {
            string FeatureDataRet = default;
            FeatureDataRet = Data.FeatureData(Index);
            return FeatureDataRet;
        }

        // 特殊能力の必要技能
        public string FeatureNecessarySkill(string Index)
        {
            string FeatureNecessarySkillRet = default;
            FeatureNecessarySkillRet = Data.Feature(Index).NecessarySkill;
            return FeatureNecessarySkillRet;
        }

        // 指定した特殊能力を持っているか？
        public bool IsFeatureAvailable(string fname)
        {
            bool IsFeatureAvailableRet = default;
            IsFeatureAvailableRet = Data.IsFeatureAvailable(fname);
            return IsFeatureAvailableRet;
        }

        // 武器データ
        public WeaponData Weapon(string Index)
        {
            WeaponData WeaponRet = default;
            WeaponRet = Data.Weapon(Index);
            return WeaponRet;
        }

        // 武器の総数
        public int CountWeapon()
        {
            int CountWeaponRet = default;
            CountWeaponRet = Data.CountWeapon();
            return CountWeaponRet;
        }

        // アビリティデータ
        public AbilityData Ability(string Index)
        {
            AbilityData AbilityRet = default;
            AbilityRet = Data.Ability(Index);
            return AbilityRet;
        }

        // アビリティの総数
        public int CountAbility()
        {
            int CountAbilityRet = default;
            CountAbilityRet = Data.CountAbility();
            return CountAbilityRet;
        }

        // サイズ(アイテムが消費するアイテムスロット数)
        public int Size()
        {
            int SizeRet = default;
            SizeRet = Data.Size();
            return SizeRet;
        }


        // アイテムが使用可能か？
        public bool IsAvailable(Unit u)
        {
            bool IsAvailableRet = default;
            int j, i, k;
            string iclass;
            string sname, fdata;
            IsAvailableRet = false;

            // イベントコマンド「Disable」
            if (Expression.IsGlobalVariableDefined("Disable(" + Name + ")"))
            {
                return IsAvailableRet;
            }

            // 装備個所に適合しているか
            switch (Part() ?? "")
            {
                case "片手":
                case "両手":
                case "盾":
                    {
                        if (Strings.InStr(u.FeatureData("装備個所"), "腕") == 0)
                        {
                            return IsAvailableRet;
                        }

                        break;
                    }

                case "肩":
                case "両肩":
                    {
                        if (Strings.InStr(u.FeatureData("装備個所"), "肩") == 0)
                        {
                            return IsAvailableRet;
                        }

                        break;
                    }

                case "体":
                    {
                        if (Strings.InStr(u.FeatureData("装備個所"), "体") == 0)
                        {
                            return IsAvailableRet;
                        }

                        break;
                    }

                case "頭":
                    {
                        if (Strings.InStr(u.FeatureData("装備個所"), "頭") == 0)
                        {
                            return IsAvailableRet;
                        }

                        break;
                    }
            }

            // 武器クラス or 防具クラスに属しているか？
            switch (Part() ?? "")
            {
                case "武器":
                case "片手":
                case "両手":
                    {
                        iclass = u.WeaponProficiency() + " 固定 汎用";
                        var loopTo = GeneralLib.LLength(iclass);
                        for (i = 1; i <= loopTo; i++)
                        {
                            if ((Class0() ?? "") == (GeneralLib.LIndex(iclass, i) ?? ""))
                            {
                                IsAvailableRet = true;
                                break;
                            }
                        }

                        break;
                    }

                case "盾":
                case "体":
                case "頭":
                    {
                        iclass = u.ArmorProficiency() + " 固定 汎用";
                        var loopTo1 = GeneralLib.LLength(iclass);
                        for (i = 1; i <= loopTo1; i++)
                        {
                            if ((Class0() ?? "") == (GeneralLib.LIndex(iclass, i) ?? ""))
                            {
                                IsAvailableRet = true;
                                break;
                            }
                        }

                        break;
                    }

                default:
                    {
                        // その他のアイテムは常に利用可能
                        IsAvailableRet = true;
                        break;
                    }
            }

            if (!IsAvailableRet)
            {
                return IsAvailableRet;
            }

            // 技能チェックが必要？
            if (!IsFeatureAvailable("必要技能") && !IsFeatureAvailable("不必要技能"))
            {
                return IsAvailableRet;
            }
            // 必要技能をチェック
            foreach (var fd in Features)
            {
                switch (fd.Name)
                {
                    case "必要技能":
                        {
                            if (!u.IsNecessarySkillSatisfied(fd.Data))
                            {
                                // アイテム自身により必要技能に指定された能力が封印されていた場合は
                                // 必要技能を満たしていると判定させるため、チェックする必要がある。
                                foreach (var itm in u.ItemList)
                                {
                                    if (ReferenceEquals(this, itm))
                                    {
                                        break;
                                    }
                                }

                                if (!u.ItemList.Any(x => x == this))
                                {
                                    // 既に装備しているのでなければ装備しない
                                    IsAvailableRet = false;
                                    return IsAvailableRet;
                                }

                                // TODO Impl 必要技能
                                //if (u.CountPilot() > 0)
                                //{
                                //    sname = u.MainPilot().SkillType(fd.Data);
                                //}
                                //else
                                //{
                                //    sname = fd.Data;
                                //}

                                //// 必要技能が「～装備」？
                                //if (Strings.Right(sname, 2) == "装備")
                                //{
                                //    if ((Strings.Left(sname, Strings.Len(sname) - 2) ?? "") == (Name ?? "") || (Strings.Left(sname, Strings.Len(sname) - 2) ?? "") == (Class0() ?? ""))
                                //    {
                                //        goto NextLoop;
                                //    }
                                //}

                                //// 封印する能力が必要技能になっている？
                                //var loopTo4 = CountFeature();
                                //for (j = 1; j <= loopTo4; j++)
                                //{
                                //    switch (Feature(j) ?? "")
                                //    {
                                //        case "パイロット能力付加":
                                //        case "パイロット能力強化":
                                //            {
                                //                break;
                                //            }

                                //        default:
                                //            {
                                //                goto NextLoop1;
                                //                break;
                                //            }
                                //    }

                                //    // 封印する能力名
                                //    fdata = FeatureData(j);
                                //    if (Strings.Left(fdata, 1) == "\"")
                                //    {
                                //        fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
                                //    }

                                //    if (Strings.InStr(fdata, "=") > 0)
                                //    {
                                //        fdata = Strings.Left(fdata, Strings.InStr(fdata, "=") - 1);
                                //    }

                                //    // 必要技能と封印する能力が一致している？
                                //    if ((fdata ?? "") == (sname ?? ""))
                                //    {
                                //        goto NextLoop;
                                //    }

                                //    if (u.CountPilot() > 0)
                                //    {
                                //        if (SRC.ALDList.IsDefined(fdata))
                                //        {
                                //            {
                                //                var withBlock = SRC.ALDList.Item(fdata);
                                //                var loopTo5 = withBlock.Count;
                                //                for (k = 1; k <= loopTo5; k++)
                                //                {
                                //                    if ((withBlock.get_AliasType(k) ?? "") == (sname ?? ""))
                                //                    {
                                //                        goto NextLoop;
                                //                    }
                                //                }
                                //            }
                                //        }
                                //        else if ((u.MainPilot().SkillType(fdata) ?? "") == (sname ?? ""))
                                //        {
                                //            goto NextLoop;
                                //        }
                                //    }

                                //NextLoop1:
                                //    ;
                                //}

                                // 必要技能が満たされていなかった
                                IsAvailableRet = false;
                                return IsAvailableRet;
                            }

                            break;
                        }

                    case "不必要技能":
                        {
                            if (u.IsNecessarySkillSatisfied(fd.Data))
                            {
                                // アイテム自身により不必要技能が満たされている場合は不必要技能を
                                // 無視させるため、チェックする必要がある。
                                foreach (var itm in u.ItemList)
                                {
                                    if (ReferenceEquals(this, itm))
                                    {
                                        break;
                                    }
                                }

                                if (!u.ItemList.Any(x => x == this))
                                {
                                    // 既に装備しているのでなければ装備しない
                                    IsAvailableRet = false;
                                    return IsAvailableRet;
                                }
                                // TODO Impl 不必要技能
                                //if (u.CountPilot() > 0)
                                //{
                                //    string localFeatureData2() { string argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

                                //    sname = u.MainPilot().SkillType(localFeatureData2());
                                //}
                                //else
                                //{
                                //    sname = fd.Data;
                                //}

                                //// 不必要技能が「～装備」？
                                //if (Strings.Right(sname, 2) == "装備")
                                //{
                                //    if ((Strings.Left(sname, Strings.Len(sname) - 2) ?? "") == (Name ?? "") || (Strings.Left(sname, Strings.Len(sname) - 2) ?? "") == (Class0() ?? ""))
                                //    {
                                //        goto NextLoop;
                                //    }
                                //}

                                //// 付加する能力が不必要技能になっている？
                                //var loopTo7 = CountFeature();
                                //for (j = 1; j <= loopTo7; j++)
                                //{
                                //    switch (Feature(j) ?? "")
                                //    {
                                //        case "パイロット能力付加":
                                //        case "パイロット能力強化":
                                //            {
                                //                break;
                                //            }

                                //        default:
                                //            {
                                //                goto NextLoop2;
                                //                break;
                                //            }
                                //    }

                                //    // 付加する能力名
                                //    fdata = FeatureData(j);
                                //    if (Strings.Left(fdata, 1) == "\"")
                                //    {
                                //        fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
                                //    }

                                //    if (Strings.InStr(fdata, "=") > 0)
                                //    {
                                //        fdata = Strings.Left(fdata, Strings.InStr(fdata, "=") - 1);
                                //    }

                                //    // 必要技能と付加する能力が一致している？
                                //    if ((fdata ?? "") == (sname ?? ""))
                                //    {
                                //        goto NextLoop;
                                //    }

                                //    if (u.CountPilot() > 0)
                                //    {
                                //        if (SRC.ALDList.IsDefined(fdata))
                                //        {
                                //            {
                                //                var withBlock1 = SRC.ALDList.Item(fdata);
                                //                var loopTo8 = withBlock1.Count;
                                //                for (k = 1; k <= loopTo8; k++)
                                //                {
                                //                    if ((withBlock1.get_AliasType(k) ?? "") == (sname ?? ""))
                                //                    {
                                //                        goto NextLoop;
                                //                    }
                                //                }
                                //            }
                                //        }
                                //        else if ((u.MainPilot().SkillType(fdata) ?? "") == (sname ?? ""))
                                //        {
                                //            goto NextLoop;
                                //        }
                                //    }

                                //NextLoop2:
                                //    ;
                                //}

                                // 不必要技能が満たされていた
                                IsAvailableRet = false;
                                return IsAvailableRet;
                            }

                            break;
                        }
                }

            NextLoop:
                ;
            }

            return IsAvailableRet;
        }
    }
}
