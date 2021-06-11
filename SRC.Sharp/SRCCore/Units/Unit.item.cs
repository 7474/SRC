using SRCCore.Items;
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.VB;
using System.Linq;

namespace SRCCore.Units
{
    // === アイテム関連処理 ===
    public partial class Unit
    {
        // アイテム装備可能数
        public int MaxItemNum()
        {
            int MaxItemNumRet = default;
            MaxItemNumRet = Data.ItemNum;
            if (IsFeatureAvailable("ハードポイント"))
            {
                foreach (var fd in Features)
                {
                    if (fd.Name == "ハードポイント" && (fd.Data == "強化パーツ" || fd.Data == "アイテム"))
                    {
                        MaxItemNumRet = (int)(MaxItemNumRet + fd.FeatureLevel);
                        break;
                    }
                }
            }

            return MaxItemNumRet;
        }

        // 装備しているアイテムの総数
        public int CountItem()
        {
            int CountItemRet = default;
            CountItemRet = colItem.Count;
            return CountItemRet;
        }

        // アイテム
        public Item Item(int Index)
        {
            try
            {
                return colItem[Index];
            }
            catch
            {
                return null;
            }
        }
        public Item Item(string Index)
        {
            Item ItemRet = colItem[Index];
            if (ItemRet != null)
            {
                return ItemRet;
            }

            // 見つからなければアイテム名で検索
            foreach (Item itm in colItem.List)
            {
                // XXX ケース考慮など
                if (itm.Name == Index)
                {
                    ItemRet = itm;
                    return ItemRet;
                }
            }
            return null;
        }

        // アイテムを装備
        public void AddItem(Item itm, bool without_refresh = false)
        {
            int i = default, num;
            var itm2 = default(Item);
            int empty_slot;
            bool found_item;

            // 既に装備していたらそのまま終了
            if (string.ReferenceEquals(itm.Unit, this))
            {
                return;
            }

            // イベント専用アイテムは装備個所を消費しない
            if (itm.Class() == "固定")
            {
                if (itm.IsFeatureAvailable("非表示"))
                {
                    goto EquipItem;
                }
            }

            // 装備個所が足りない場合に元のアイテムを外す
            switch (itm.Part() ?? "")
            {
                case "強化パーツ":
                case "アイテム":
                    {
                        if (itm.FeatureData("ハードポイント") != "強化パーツ" && itm.FeatureData("ハードポイント") != "アイテム")
                        {
                            // 装備している強化パーツ数をカウント
                            num = 0;
                            foreach (Item currentItm2 in colItem)
                            {
                                itm2 = currentItm2;
                                {
                                    var withBlock = itm2;
                                    if (withBlock.Part() == "強化パーツ" || withBlock.Part() == "アイテム")
                                    {
                                        num = (num + withBlock.Size());
                                    }
                                }
                            }

                            // 大型アイテムの場合は余分に外す
                            num = (int)(num + itm.FeatureLevel("大型アイテム"));

                            // 何れかを外さなければならない場合
                            while (num >= MaxItemNum() && num > 0)
                            {
                                found_item = false;

                                // まずはハードポイントを持たないものから
                                foreach (Item currentItm21 in colItem)
                                {
                                    itm2 = currentItm21;
                                    {
                                        var withBlock1 = itm2;
                                        if (withBlock1.Part() == "強化パーツ" || withBlock1.Part() == "アイテム")
                                        {
                                            if (!withBlock1.IsFeatureAvailable("ハードポイント"))
                                            {
                                                num = (num - withBlock1.Size());
                                                if (Party0 != "味方")
                                                {
                                                    withBlock1.Exist = false;
                                                }

                                                DeleteItem(Item(withBlock1.ID));
                                                itm2 = null;
                                                found_item = true;
                                                break;
                                            }
                                        }
                                    }
                                }

                                // ハードポイント付きのものしかない場合
                                if (itm2 != null)
                                {
                                    num = (num - Item(1).Size());
                                    if (Party0 != "味方")
                                    {
                                        Item(1).Exist = false;
                                    }

                                    DeleteItem(Item(1));
                                    found_item = true;
                                }

                                if (!found_item)
                                {
                                    // 外せるアイテムがない
                                    break;
                                }
                            }

                            if (MaxItemNum() == 0)
                            {
                                // 装備出来ません…
                                return;
                            }
                        }

                        break;
                    }

                case "両手":
                    {
                        foreach (Item currentItm22 in colItem)
                        {
                            itm2 = currentItm22;
                            if (itm2.Part() == "両手" || itm2.Part() == "片手" || itm2.Part() == "盾")
                            {
                                if (Party0 != "味方")
                                {
                                    itm2.Exist = false;
                                }

                                DeleteItem(itm2);
                                break;
                            }
                        }

                        break;
                    }

                case "片手":
                    {
                        if (IsFeatureAvailable("両手利き"))
                        {
                            num = 0;
                            foreach (Item currentItm23 in colItem)
                            {
                                itm2 = currentItm23;
                                switch (itm2.Part() ?? "")
                                {
                                    case "両手":
                                        {
                                            if (Party0 != "味方")
                                            {
                                                itm2.Exist = false;
                                            }

                                            DeleteItem(itm2);
                                            break;
                                        }

                                    case "片手":
                                    case "盾":
                                        {
                                            num = (num + 1);
                                            if (num > 1)
                                            {
                                                if (Party0 != "味方")
                                                {
                                                    itm2.Exist = false;
                                                }

                                                DeleteItem(itm2);
                                                break;
                                            }

                                            break;
                                        }
                                }
                            }
                        }
                        else
                        {
                            foreach (Item currentItm24 in colItem)
                            {
                                itm2 = currentItm24;
                                switch (itm2.Part() ?? "")
                                {
                                    case "両手":
                                    case "片手":
                                        {
                                            if (Party0 != "味方")
                                            {
                                                itm2.Exist = false;
                                            }

                                            DeleteItem(itm2);
                                            break;
                                        }
                                }
                            }
                        }

                        break;
                    }

                case "盾":
                    {
                        foreach (Item currentItm25 in colItem)
                        {
                            itm2 = currentItm25;
                            switch (itm2.Part() ?? "")
                            {
                                case "両手":
                                case "盾":
                                    {
                                        if (Party0 != "味方")
                                        {
                                            itm2.Exist = false;
                                        }

                                        DeleteItem(itm2);
                                        break;
                                    }

                                case "片手":
                                    {
                                        i = (i + 1);
                                        if (i > 1)
                                        {
                                            if (Party0 != "味方")
                                            {
                                                itm2.Exist = false;
                                            }

                                            DeleteItem(itm2);
                                            break;
                                        }

                                        break;
                                    }
                            }
                        }

                        break;
                    }

                case "両肩":
                    {
                        foreach (Item currentItm26 in colItem)
                        {
                            itm2 = currentItm26;
                            if (itm2.Part() == "両肩" || itm2.Part() == "肩")
                            {
                                if (Party0 != "味方")
                                {
                                    itm2.Exist = false;
                                }

                                DeleteItem(itm2);
                            }
                        }

                        break;
                    }

                case "肩":
                    {
                        num = 0;
                        foreach (Item currentItm27 in colItem)
                        {
                            itm2 = currentItm27;
                            switch (itm2.Part() ?? "")
                            {
                                case "両肩":
                                    {
                                        if (Party0 != "味方")
                                        {
                                            itm2.Exist = false;
                                        }

                                        DeleteItem(itm2);
                                        break;
                                    }

                                case "肩":
                                    {
                                        num = (num + 1);
                                        if (num > 1)
                                        {
                                            if (Party0 != "味方")
                                            {
                                                itm2.Exist = false;
                                            }

                                            DeleteItem(itm2);
                                            break;
                                        }

                                        break;
                                    }
                            }
                        }

                        break;
                    }
                // 装備個所が「非表示」のアイテムは装備数に制限なし

                case "非表示":
                    {
                        break;
                    }

                default:
                    {
                        // ハードポイントに装備する場合
                        foreach (var fd in Features)
                        {
                            if (fd.Name == "ハードポイント" && fd.Data == itm.Part())
                            {
                                // まず空きスロット数を計算
                                empty_slot = ItemSlotSize(itm.Part());
                                foreach (Item currentItm28 in colItem)
                                {
                                    itm2 = currentItm28;
                                    if ((itm2.Part() ?? "") == (itm.Part() ?? ""))
                                    {
                                        empty_slot = (empty_slot - itm2.Size());
                                    }
                                }
                                // 足らないスロット数分、アイテムを外す
                                if (empty_slot < itm.Size())
                                {
                                    foreach (Item currentItm29 in colItem)
                                    {
                                        itm2 = currentItm29;
                                        if ((itm2.Part() ?? "") == (itm.Part() ?? ""))
                                        {
                                            if (Party0 != "味方")
                                            {
                                                itm2.Exist = false;
                                            }

                                            DeleteItem(itm2);
                                            empty_slot = (empty_slot + itm2.Size());
                                            if (empty_slot >= itm.Size())
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }

                                i = 0;
                                break;
                            }
                        }
                        // そうでない場合
                        if (i > 0)
                        {
                            foreach (Item currentItm210 in colItem)
                            {
                                itm2 = currentItm210;
                                if ((itm.Part() ?? "") == (itm2.Part() ?? ""))
                                {
                                    if (Party0 != "味方")
                                    {
                                        itm2.Exist = false;
                                    }

                                    DeleteItem(itm2);
                                    break;
                                }
                            }
                        }

                        break;
                    }
            }

        EquipItem:
            ;


            // 装備されたアイテムは常に存在するとみなす
            if (Status != "破棄")
            {
                itm.Exist = true;
            }

            colItem.Add(itm, itm.ID);
            itm.Unit = this;

            // アイテムを装備したことによるステータスの変化
            Update(without_refresh);
        }

        public void AddItem0(Item itm)
        {
            colItem.Add(itm, itm.ID);
            itm.Unit = this;
        }

        // アイテムをはずす
        public void DeleteItem(Item itm, bool without_refresh = false)
        {
            // 存在しないアイテム？
            if (itm is null)
            {
                return;
            }

            // この辺のシーケンスはSRCと比べて簡便化している
            DeleteItemInternal(itm);
            Update();
            // 元は以下のようなことをしていた
            // 削除するアイテムの武器・アビリティの残弾数が引き継がれるのを防ぐため、
            // 削除するアイテムによって付加された武器・アビリティのデータを削除する。

            // ハードポイントを持つアイテムをはずした場合は他のアイテムを連続してはずす必要がある
            // XXX 未疎通
            bool is_changed;
            do
            {
                is_changed = false;
                var is_ambidextrous = IsFeatureAvailable("両手利き");
                // ハードポイントを持たないアイテムから選んで削除されるようにFill順を指定
                var slots = new ItemSlots(this).FillSlot((x) => x.IsFeatureAvailable("ハードポイント") ? 0 : 1);
                var deleteItems = ItemList.Except(slots.Slots.Where(x => !x.IsEmpty).Select(x => x.Item))
                    .Where(x => x.IsVisible)
                    .ToList();

                foreach (var delItm in deleteItems)
                {
                    DeleteItemInternal(delItm);
                    is_changed = true;
                }

                // 両手利きで無くなってしまった場合は二個目の片手アイテムを外す
                if (is_ambidextrous && !IsFeatureAvailable("両手利き"))
                {
                    DeleteItemInternal(ItemList.Where(x => x.Part() == "片手").Skip(1).FirstOrDefault());
                    is_changed = true;
                }
            }
            while (is_changed);
        }

        private void DeleteItemInternal(Item itm)
        {
            if (itm == null) { return; }
            colItem.Remove(itm.ID);
            if (itm.Unit != null)
            {
                if (itm.Unit.ID == ID)
                {
                    itm.Unit = null;
                }
                // 追加パイロットを持つアイテムを削除する場合
                if (itm.IsFeatureAvailable("追加パイロット"))
                {
                    if (SRC.PList.IsDefined(itm.FeatureData("追加パイロット")))
                    {
                        {
                            var p = SRC.PList.Item(itm.FeatureData("追加パイロット"));
                            p.Alive = false;
                            p.Unit = null;
                        }
                    }
                }
            }
        }

        // 装備個所が ipart のアイテムの装備可能数
        public int ItemSlotSize(string ipart)
        {
            int ItemSlotSizeRet = default;
            int i;
            switch (ipart ?? "")
            {
                case "強化パーツ":
                case "アイテム":
                    {
                        ItemSlotSizeRet = Data.ItemNum;
                        if (!IsFeatureAvailable("ハードポイント"))
                        {
                            return ItemSlotSizeRet;
                        }

                        foreach (var fd in Features)
                        {
                            if (fd.Name == "ハードポイント")
                            {
                                switch (fd.Data)
                                {
                                    case "強化パーツ":
                                    case "アイテム":
                                        {
                                            ItemSlotSizeRet = ((int)(ItemSlotSizeRet + fd.FeatureLevel));
                                            break;
                                        }
                                }
                            }
                        }

                        break;
                    }

                default:
                    {
                        if (!IsFeatureAvailable("ハードポイント"))
                        {
                            ItemSlotSizeRet = 1;
                            return ItemSlotSizeRet;
                        }

                        foreach (var fd in Features)
                        {
                            if (fd.Name == "ハードポイント")
                            {
                                if (fd.Data == ipart)
                                {
                                    ItemSlotSizeRet = ((int)(ItemSlotSizeRet + fd.FeatureLevel));
                                }
                            }
                        }

                        break;
                    }
            }

            return ItemSlotSizeRet;
        }

        // アイテム iname を装備しているか？
        public bool IsEquiped(string iname)
        {
            return ItemList.Any(x => x.Name == iname);
        }

        // 装備可能な武器クラス
        public string WeaponProficiency()
        {
            string WeaponProficiencyRet = default;
            foreach (FeatureData fd in colFeature)
            {
                if (fd.Name == "武器クラス")
                {
                    WeaponProficiencyRet = WeaponProficiencyRet + " " + fd.StrData;
                }
            }

            return WeaponProficiencyRet;
        }

        // 装備可能な防具クラス
        public string ArmorProficiency()
        {
            string ArmorProficiencyRet = default;
            foreach (FeatureData fd in colFeature)
            {
                if (fd.Name == "防具クラス")
                {
                    ArmorProficiencyRet = ArmorProficiencyRet + " " + fd.StrData;
                }
            }

            return ArmorProficiencyRet;
        }

        // アイテムitを装備できるかどうかを判定
        public bool IsAbleToEquip(Item it)
        {
            bool IsAbleToEquipRet = default;
            string iclass;
            string eclass0, uclass, eclass;
            int i, j;
            // 既に装備済みのアイテムは装備できない
            if (it.Unit != null)
            {
                if (it.Unit.ID == ID)
                {
                    IsAbleToEquipRet = false;
                    return IsAbleToEquipRet;
                }
            }

            // Fixコマンドで固定されたアイテムは装備不可能
            if (Expression.IsGlobalVariableDefined("Fix(" + it.Name + ")"))
            {
                IsAbleToEquipRet = false;
                return IsAbleToEquipRet;
            }

            // 必要技能は満たしているか？
            if (!it.IsAvailable(this))
            {
                IsAbleToEquipRet = false;
                return IsAbleToEquipRet;
            }

            // アイテムのクラスを記録
            iclass = it.Class();

            // 汎用ならばユニットの種類に関わらず装備可能
            if (iclass == "汎用")
            {
                // ただし強化パーツのチェックは必要
                if (it.Part() == "強化パーツ" && IsHero())
                {
                    IsAbleToEquipRet = false;
                    return IsAbleToEquipRet;
                }

                IsAbleToEquipRet = true;
                return IsAbleToEquipRet;
            }

            // 固定アイテムは装備不能とみなす
            if (iclass == "固定")
            {
                IsAbleToEquipRet = false;
                return IsAbleToEquipRet;
            }

            // ユニットクラスから余分な指定を取り除く
            uclass = Class0;

            // 装備個所
            switch (it.Part() ?? "")
            {
                case "武器":
                case "片手":
                case "両手":
                    {
                        eclass = WeaponProficiency();
                        var loopTo = GeneralLib.LLength(eclass);
                        for (i = 1; i <= loopTo; i++)
                        {
                            eclass0 = GeneralLib.LIndex(eclass, i);
                            if ((iclass ?? "") == (eclass0 ?? ""))
                            {
                                IsAbleToEquipRet = true;
                                return IsAbleToEquipRet;
                            }
                            else if (Strings.InStr(iclass, "専用)") > 0)
                            {
                                // ユニットクラス、ユニット名による専用指定？
                                if (Strings.InStr(iclass, eclass0 + "(") == 1 && (Strings.InStr(iclass, "(" + uclass + "専用)") > 0 || Strings.InStr(iclass, "(" + Name + "専用)") > 0 || Strings.InStr(iclass, "(" + Nickname + "専用)") > 0))
                                {
                                    IsAbleToEquipRet = true;
                                    return IsAbleToEquipRet;
                                }

                                // 性別による専用指定？
                                if (CountPilot() > 0)
                                {
                                    if ((iclass ?? "") == (eclass0 + "(" + MainPilot().Sex + "専用)" ?? ""))
                                    {
                                        IsAbleToEquipRet = true;
                                        return IsAbleToEquipRet;
                                    }
                                }
                            }
                        }

                        // 一部の形態でのみ利用可能な武器の判定
                        foreach (var of in OtherForms)
                        {
                            uclass = of.Class0;
                            eclass = of.WeaponProficiency();
                            var loopTo2 = GeneralLib.LLength(eclass);
                            for (j = 1; j <= loopTo2; j++)
                            {
                                eclass0 = GeneralLib.LIndex(eclass, j);
                                if ((iclass ?? "") == (eclass0 ?? ""))
                                {
                                    IsAbleToEquipRet = true;
                                    return IsAbleToEquipRet;
                                }
                                else if (Strings.InStr(iclass, "専用)") > 0)
                                {
                                    // ユニットクラス、ユニット名による専用指定？
                                    if (Strings.InStr(iclass, eclass0 + "(") == 1 && (Strings.InStr(iclass, "(" + uclass + "専用)") > 0 || Strings.InStr(iclass, "(" + of.Name + "専用)") > 0 || Strings.InStr(iclass, "(" + of.Nickname + "専用)") > 0))
                                    {
                                        IsAbleToEquipRet = true;
                                        return IsAbleToEquipRet;
                                    }

                                    // 性別による専用指定？
                                    if (CountPilot() > 0)
                                    {
                                        if ((iclass ?? "") == (eclass0 + "(" + MainPilot().Sex + "専用)" ?? ""))
                                        {
                                            IsAbleToEquipRet = true;
                                            return IsAbleToEquipRet;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }

                case "体":
                case "頭":
                case "盾":
                    {
                        eclass = ArmorProficiency();
                        var loopTo3 = GeneralLib.LLength(eclass);
                        for (i = 1; i <= loopTo3; i++)
                        {
                            eclass0 = GeneralLib.LIndex(eclass, i);
                            if ((iclass ?? "") == (eclass0 ?? ""))
                            {
                                IsAbleToEquipRet = true;
                                return IsAbleToEquipRet;
                            }
                            else if (Strings.InStr(iclass, "専用)") > 0)
                            {
                                // ユニットクラス、ユニット名による専用指定？
                                if (Strings.InStr(iclass, eclass0 + "(") == 1 && (Strings.InStr(iclass, "(" + uclass + "専用)") > 0 || Strings.InStr(iclass, "(" + Name + "専用)") > 0 || Strings.InStr(iclass, "(" + Nickname + "専用)") > 0))
                                {
                                    IsAbleToEquipRet = true;
                                    return IsAbleToEquipRet;
                                }

                                // 性別による専用指定？
                                if (CountPilot() > 0)
                                {
                                    if ((iclass ?? "") == (eclass0 + "(" + MainPilot().Sex + "専用)" ?? ""))
                                    {
                                        IsAbleToEquipRet = true;
                                        return IsAbleToEquipRet;
                                    }
                                }
                            }
                        }

                        // 一部の形態でのみ利用可能な防具の判定
                        foreach (var of in OtherForms)
                        {
                            uclass = of.Class0;
                            eclass = of.ArmorProficiency();
                            var loopTo5 = GeneralLib.LLength(eclass);
                            for (j = 1; j <= loopTo5; j++)
                            {
                                eclass0 = GeneralLib.LIndex(eclass, j);
                                if ((iclass ?? "") == (eclass0 ?? ""))
                                {
                                    IsAbleToEquipRet = true;
                                    return IsAbleToEquipRet;
                                }
                                else if (Strings.InStr(iclass, "専用)") > 0)
                                {
                                    // ユニットクラス、ユニット名による専用指定？
                                    if (Strings.InStr(iclass, eclass0 + "(") == 1 && (Strings.InStr(iclass, "(" + uclass + "専用)") > 0 || Strings.InStr(iclass, "(" + of.Name + "専用)") > 0 || Strings.InStr(iclass, "(" + of.Nickname + "専用)") > 0))
                                    {
                                        IsAbleToEquipRet = true;
                                        return IsAbleToEquipRet;
                                    }

                                    // 性別による専用指定？
                                    if (CountPilot() > 0)
                                    {
                                        if ((iclass ?? "") == (eclass0 + "(" + MainPilot().Sex + "専用)" ?? ""))
                                        {
                                            IsAbleToEquipRet = true;
                                            return IsAbleToEquipRet;
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    }

                case "アイテム":
                case "強化パーツ":
                    {
                        // 強化パーツは人間ユニットには装備できない
                        if (Strings.InStr(it.Part(), "強化パーツ") == 1)
                        {
                            if (IsHero())
                            {
                                IsAbleToEquipRet = false;
                                return IsAbleToEquipRet;
                            }
                        }

                        // これらのアイテムは専用アイテムでない限り必ず装備可能
                        if (Strings.InStr(iclass, "専用)") == 0)
                        {
                            IsAbleToEquipRet = true;
                            return IsAbleToEquipRet;
                        }

                        // ユニットクラス、ユニット名による専用指定？
                        if (Strings.InStr(iclass, "(" + uclass + "専用)") > 0 || Strings.InStr(iclass, "(" + Name + "専用)") > 0 || Strings.InStr(iclass, "(" + Nickname + "専用)") > 0)
                        {
                            IsAbleToEquipRet = true;
                            return IsAbleToEquipRet;
                        }

                        // 性別による専用指定？
                        if (CountPilot() > 0)
                        {
                            if (Strings.InStr(iclass, "(" + MainPilot().Sex + "専用)") > 0)
                            {
                                IsAbleToEquipRet = true;
                                return IsAbleToEquipRet;
                            }
                        }

                        // 他の形態の名前で専用指定されている？
                        foreach (var of in OtherForms)
                        {
                            if (Strings.InStr(iclass, "(" + of.Class0 + "専用)") > 0 || Strings.InStr(iclass, "(" + of.Name + "専用)") > 0 || Strings.InStr(iclass, "(" + of.Nickname + "専用)") > 0)
                            {
                                IsAbleToEquipRet = true;
                                return IsAbleToEquipRet;
                            }
                        }

                        break;
                    }

                default:
                    {
                        // 創作された装備個所のアイテムは専用アイテムでない限り必ず装備可能
                        if (Strings.InStr(iclass, "専用)") == 0)
                        {
                            IsAbleToEquipRet = true;
                            return IsAbleToEquipRet;
                        }

                        // ユニットクラス、ユニット名による専用指定？
                        if (Strings.InStr(iclass, "(" + uclass + "専用)") > 0 || Strings.InStr(iclass, "(" + Name + "専用)") > 0 || Strings.InStr(iclass, "(" + Nickname + "専用)") > 0)
                        {
                            IsAbleToEquipRet = true;
                            return IsAbleToEquipRet;
                        }

                        // 性別による専用指定？
                        if (CountPilot() > 0)
                        {
                            if (Strings.InStr(iclass, "(" + MainPilot().Sex + "専用)") > 0)
                            {
                                IsAbleToEquipRet = true;
                                return IsAbleToEquipRet;
                            }
                        }

                        // 他の形態の名前で専用指定されている？
                        foreach (var of in OtherForms)
                        {
                            if (Strings.InStr(iclass, "(" + of.Class0 + "専用)") > 0 || Strings.InStr(iclass, "(" + of.Name + "専用)") > 0 || Strings.InStr(iclass, "(" + of.Nickname + "専用)") > 0)
                            {
                                IsAbleToEquipRet = true;
                                return IsAbleToEquipRet;
                            }
                        }

                        break;
                    }
            }
            return false;
        }
    }
}
