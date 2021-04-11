// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SRCCore.Items
{
    internal class Items
    {
        // アイテムＩＤ作成用カウンタ
        private int IDCount;

        // アイテム一覧
        private SrcCollection<Item> colItems = new SrcCollection<Item>();
        public IList<Item> List => colItems.List;

        private SRC SRC { get; }
        public Items(SRC src)
        {
            SRC = src;
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
                if (it.Unit is string)
                {
                    it.Unit = SRC.UList.Item((string)it.Unit.ID);
                }
            }
        }


        // データをファイルにセーブ
        public void Save()
        {
            short i;
            FileSystem.WriteLine(SRC.SaveDataFileNumber, (string)IDCount);
            FileSystem.WriteLine(SRC.SaveDataFileNumber, (string)Count());
            var loopTo = Count();
            for (i = 1; i <= loopTo; i++)
            {
                {
                    var withBlock = Item(i);
                    FileSystem.WriteLine(SRC.SaveDataFileNumber, withBlock.Name);
                    if (withBlock.Unit is null)
                    {
                        FileSystem.WriteLine(SRC.SaveDataFileNumber, withBlock.ID, "-");
                    }
                    else
                    {
                        FileSystem.WriteLine(SRC.SaveDataFileNumber, withBlock.ID, withBlock.Unit.ID);
                    }
                }
            }
        }

        // データをファイルからロード
        public void Load()
        {
            var num = default(short);
            Item new_item;
            var iname = default(string);
            var iid = default(string);
            short i;
            var dummy = default(string);
            if (FileSystem.EOF(SRC.SaveDataFileNumber))
            {
                return;
            }

            FileSystem.Input(SRC.SaveDataFileNumber, IDCount);
            FileSystem.Input(SRC.SaveDataFileNumber, num);
            var loopTo = num;
            for (i = 1; i <= loopTo; i++)
            {
                new_item = new Item();
                // Name
                FileSystem.Input(SRC.SaveDataFileNumber, iname);
                // ID, Unit
                FileSystem.Input(SRC.SaveDataFileNumber, iid);
                FileSystem.Input(SRC.SaveDataFileNumber, dummy);
                bool localIsDefined() { string argIndex1 = iname; var ret = SRC.IDList.IsDefined(argIndex1); return ret; }

                if (!localIsDefined())
                {
                    GUI.ErrorMessage(iname + "のデータが定義されていません");
                    Sound.StopBGM();
                    Environment.Exit(0);
                }

                new_item.Name = iname;
                new_item.ID = iid;
                colItems.Add(new_item, iid);
            }
        }

        // リンク情報をファイルからロード
        public void LoadLinkInfo()
        {
            short num = default, i;
            string dummy;
            if (FileSystem.EOF(SRC.SaveDataFileNumber))
            {
                return;
            }

            // IDCount
            dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);
            FileSystem.Input(SRC.SaveDataFileNumber, num);
            var loopTo = num;
            for (i = 1; i <= loopTo; i++)
            {
                // Name
                dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);
                // ID, Unit
                dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);
            }
        }


        // 一時中断用データをファイルにセーブする
        public void Dump()
        {
            FileSystem.WriteLine(SRC.SaveDataFileNumber, (string)Count());
            foreach (Item it in colItems)
                it.Dump();
        }

        // 一時中断用データをファイルからロードする
        public void Restore()
        {
            short i, num = default;
            Item it;
            {
                var withBlock = colItems;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }

            FileSystem.Input(SRC.SaveDataFileNumber, num);
            var loopTo1 = num;
            for (i = 1; i <= loopTo1; i++)
            {
                it = new Item();
                it.Restore();
                colItems.Add(it, it.ID);
            }
        }

        // 一時中断用データのリンク情報をファイルからロードする
        public void RestoreLinkInfo()
        {
            var num = default(short);
            FileSystem.Input(SRC.SaveDataFileNumber, num);
            foreach (Item it in colItems)
                it.RestoreLinkInfo();
        }

        // 一時中断用データのパラメータ情報をファイルからロードする
        public void RestoreParameter()
        {
            var num = default(short);
            FileSystem.Input(SRC.SaveDataFileNumber, num);
            foreach (Item it in colItems)
                it.RestoreParameter();
        }


        // リストをクリア
        public void Clear()
        {
            short i;
            var loopTo = Count();
            for (i = 1; i <= loopTo; i++)
            {
                Delete(1);
            }
        }
    }
}
