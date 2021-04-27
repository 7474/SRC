// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using Newtonsoft.Json;
using SRCCore.VB;
using System;
using System.Collections.Generic;

namespace SRCCore.Items
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Items
    {
        // アイテムＩＤ作成用カウンタ
        [JsonProperty]
        private int IDCount;

        // アイテム一覧
        [JsonProperty]
        private SrcCollection<Item> colItems = new SrcCollection<Item>();
        public IList<Item> List => colItems.List;

        private SRC SRC;
        public Items(SRC src)
        {
            SRC = src;
        }

        public void Restore(SRC src)
        {
            SRC = src;
            foreach (var i in List)
            {
                i.Restore(src);
            }
        }

        // リストにアイテムを追加
        public Item Add(string Name)
        {
            if (!SRC.IDList.IsDefined(Name))
            {
                return null;
            }

            var new_item = new Item(SRC);
            new_item.Name = Name;
            new_item.ID = CreateID(Name);
            colItems.Add(new_item, new_item.ID);
            return new_item;
        }

        // 新しいアイテムＩＤを作成
        private string CreateID(string iname)
        {
            string CreateIDRet = default;
            bool localIsDefined2() { string argIndex1 = (string)(iname + "_" + SrcFormatter.Format(IDCount)); var ret = IsDefined2(argIndex1); return ret; }

            do
                IDCount = IDCount + 1;
            while (localIsDefined2());
            CreateIDRet = iname + "_" + SrcFormatter.Format(IDCount);
            return CreateIDRet;
        }

        // リストに登録されているアイテムの総数
        public short Count()
        {
            short CountRet = default;
            CountRet = (short)colItems.Count;
            return CountRet;
        }

        // リストからアイテムを削除
        public void Delete(string Index)
        {
            colItems.Remove(Index);
        }

        // 指定されたアイテムを検索
        public Item Item(string Index)
        {
            Item ItemRet = colItems[Index];

            // 破棄されていない？
            if (ItemRet?.Exist ?? false)
            {
                return ItemRet;
            }

            // 見つからなければアイテム名で検索
            var iname = Conversions.ToString(Index);
            foreach (Item it in colItems)
            {
                if ((it.Name ?? "") == (iname ?? "") & it.Exist)
                {
                    return it;
                }
            }
            return null;
        }

        // 指定されたアイテムが登録されているか？
        public bool IsDefined(string Index)
        {
            return Item(Index) != null;
        }

        // アイテム名とExitフラグを無視してアイテムを検索
        public bool IsDefined2(string Index)
        {
            return colItems[Index] != null;
        }

        // リストに登録されたアイテムをアップデート
        public void Update()
        {
            Item it;

            // 破棄されたアイテムを削除
            foreach (Item currentIt in colItems)
            {
                it = currentIt;
                if (!it.Exist)
                {
                    colItems.Remove(it.ID);
                }
            }

            // リンクデータの整合性を取る
            foreach (Item currentIt1 in colItems)
            {
                it = currentIt1;
                if (it.Unit != null)
                {
                    it.Unit = SRC.UList.Item((string)it.Unit.ID);
                }
            }
        }

        // リストをクリア
        public void Clear()
        {
            colItems.Clear();
        }
    }
}
