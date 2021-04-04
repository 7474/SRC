//using System;
//using System.Collections;
//using Microsoft.VisualBasic;
//using Microsoft.VisualBasic.CompilerServices;

//namespace Project1
//{
//    internal class Items : IEnumerable
//    {

//        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
//        // 本プログラムはフリーソフトであり、無保証です。
//        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
//        // 再頒布または改変することができます。

//        // アイテムＩＤ作成用カウンタ
//        private int IDCount;

//        // アイテム一覧
//        private Collection colItems = new Collection();

//        // クラスの解放
//        // UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
//        private void Class_Terminate_Renamed()
//        {
//            short i;
//            {
//                var withBlock = colItems;
//                var loopTo = (short)withBlock.Count;
//                for (i = 1; i <= loopTo; i++)
//                    withBlock.Remove(1);
//            }
//            // UPGRADE_NOTE: オブジェクト colItems をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
//            colItems = null;
//        }

//        ~Items()
//        {
//            Class_Terminate_Renamed();
//        }

//        // ForEach用関数
//        // UPGRADE_NOTE: NewEnum プロパティがコメント アウトされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3FC1610-34F3-43F5-86B7-16C984F0E88E"' をクリックしてください。
//        // Public Function NewEnum() As stdole.IUnknown
//        // NewEnum = colItems.GetEnumerator
//        // End Function

//        public IEnumerator GetEnumerator()
//        {
//            return default;
//            // UPGRADE_TODO: コレクション列挙子を返すには、コメントを解除して以下の行を変更してください。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="95F9AAD0-1319-4921-95F0-B9D3C4FF7F1C"' をクリックしてください。
//            // GetEnumerator = colItems.GetEnumerator
//        }


//        // リストにアイテムを追加
//        public Item Add(ref string Name)
//        {
//            Item AddRet = default;
//            Item new_item;
//            bool localIsDefined() { object argIndex1 = Name; var ret = SRC.IDList.IsDefined(ref argIndex1); return ret; }

//            if (!localIsDefined())
//            {
//                // UPGRADE_NOTE: オブジェクト Add をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
//                AddRet = null;
//                return AddRet;
//            }

//            new_item = new Item();
//            AddRet = new_item;
//            new_item.Name = Name;
//            new_item.ID = CreateID(ref Name);
//            colItems.Add(new_item, new_item.ID);
//            return AddRet;
//        }

//        // 新しいアイテムＩＤを作成
//        private string CreateID(ref string iname)
//        {
//            string CreateIDRet = default;
//            bool localIsDefined2() { object argIndex1 = (object)(iname + "_" + SrcFormatter.Format(IDCount)); var ret = IsDefined2(ref argIndex1); return ret; }

//            do
//                IDCount = IDCount + 1;
//            while (localIsDefined2());
//            CreateIDRet = iname + "_" + SrcFormatter.Format(IDCount);
//            return CreateIDRet;
//        }

//        // リストに登録されているアイテムの総数
//        public short Count()
//        {
//            short CountRet = default;
//            CountRet = (short)colItems.Count;
//            return CountRet;
//        }

//        // リストからアイテムを削除
//        public void Delete(ref object Index)
//        {
//            colItems.Remove(Index);
//        }

//        // 指定されたアイテムを検索
//        public Item Item(ref object Index)
//        {
//            Item ItemRet = default;
//            string iname;
//            ;
//#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
//            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 3074


//            Input:

//                    On Error GoTo ErrorHandler

//             */
//            ItemRet = (Item)colItems[Index];

//            // 破棄されていない？
//            if (ItemRet.Exist)
//            {
//                return ItemRet;
//            }

//            ErrorHandler:
//            ;

//            // 見つからなければアイテム名で検索
//            // UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
//            iname = Conversions.ToString(Index);
//            foreach (Item it in colItems)
//            {
//                if ((it.Name ?? "") == (iname ?? "") & it.Exist)
//                {
//                    ItemRet = it;
//                    return ItemRet;
//                }
//            }
//            // UPGRADE_NOTE: オブジェクト Item をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
//            ItemRet = null;
//            return ItemRet;
//        }

//        // 指定されたアイテムが登録されているか？
//        public bool IsDefined(ref object Index)
//        {
//            bool IsDefinedRet = default;
//            Item it;
//            string iname;
//            ;
//#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
//            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 3972


//            Input:

//                    On Error GoTo ErrorHandler

//             */
//            it = (Item)colItems[Index];

//            // 破棄されたアイテムは登録されていないとみなす
//            if (it.Exist)
//            {
//                IsDefinedRet = true;
//                return IsDefinedRet;
//            }

//            ErrorHandler:
//            ;

//            // 見つからなければアイテム名で検索
//            // UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
//            iname = Conversions.ToString(Index);
//            foreach (Item currentIt in colItems)
//            {
//                it = currentIt;
//                if ((it.Name ?? "") == (iname ?? "") & it.Exist)
//                {
//                    IsDefinedRet = true;
//                    return IsDefinedRet;
//                }
//            }

//            IsDefinedRet = false;
//            return IsDefinedRet;
//        }

//        // アイテム名とExitフラグを無視してアイテムを検索
//        public bool IsDefined2(ref object Index)
//        {
//            bool IsDefined2Ret = default;
//            Item it;
//            ;
//#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
//            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 4704


//            Input:

//                    On Error GoTo ErrorHandler

//             */
//            it = (Item)colItems[Index];
//            IsDefined2Ret = true;
//            return IsDefined2Ret;
//            ErrorHandler:
//            ;
//            IsDefined2Ret = false;
//        }

//        // リストに登録されたアイテムをアップデート
//        public void Update()
//        {
//            Item it;

//            // 破棄されたアイテムを削除
//            foreach (Item currentIt in colItems)
//            {
//                it = currentIt;
//                if (!it.Exist)
//                {
//                    colItems.Remove(it.ID);
//                }
//            }

//            // リンクデータの整合性を取る
//            foreach (Item currentIt1 in colItems)
//            {
//                it = currentIt1;
//                if (it.Unit_Renamed is object)
//                {
//                    it.Unit_Renamed = SRC.UList.Item(ref (object)it.Unit_Renamed.ID);
//                }
//            }
//        }


//        // データをファイルにセーブ
//        public void Save()
//        {
//            short i;
//            FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)IDCount);
//            FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)Count());
//            var loopTo = Count();
//            for (i = 1; i <= loopTo; i++)
//            {
//                {
//                    var withBlock = Item(ref i);
//                    FileSystem.WriteLine(SRC.SaveDataFileNumber, withBlock.Name);
//                    if (withBlock.Unit_Renamed is null)
//                    {
//                        FileSystem.WriteLine(SRC.SaveDataFileNumber, withBlock.ID, "-");
//                    }
//                    else
//                    {
//                        FileSystem.WriteLine(SRC.SaveDataFileNumber, withBlock.ID, withBlock.Unit_Renamed.ID);
//                    }
//                }
//            }
//        }

//        // データをファイルからロード
//        public void Load()
//        {
//            var num = default(short);
//            Item new_item;
//            var iname = default(string);
//            var iid = default(string);
//            short i;
//            var dummy = default(string);
//            if (FileSystem.EOF(SRC.SaveDataFileNumber))
//            {
//                return;
//            }

//            FileSystem.Input(SRC.SaveDataFileNumber, ref IDCount);
//            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
//            var loopTo = num;
//            for (i = 1; i <= loopTo; i++)
//            {
//                new_item = new Item();
//                // Name
//                FileSystem.Input(SRC.SaveDataFileNumber, ref iname);
//                // ID, Unit
//                FileSystem.Input(SRC.SaveDataFileNumber, ref iid);
//                FileSystem.Input(SRC.SaveDataFileNumber, ref dummy);
//                bool localIsDefined() { object argIndex1 = iname; var ret = SRC.IDList.IsDefined(ref argIndex1); return ret; }

//                if (!localIsDefined())
//                {
//                    GUI.ErrorMessage(ref iname + "のデータが定義されていません");
//                    Sound.StopBGM();
//                    Environment.Exit(0);
//                }

//                new_item.Name = iname;
//                new_item.ID = iid;
//                colItems.Add(new_item, iid);
//            }
//        }

//        // リンク情報をファイルからロード
//        public void LoadLinkInfo()
//        {
//            short num = default, i;
//            string dummy;
//            if (FileSystem.EOF(SRC.SaveDataFileNumber))
//            {
//                return;
//            }

//            // IDCount
//            dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);
//            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
//            var loopTo = num;
//            for (i = 1; i <= loopTo; i++)
//            {
//                // Name
//                dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);
//                // ID, Unit
//                dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);
//            }
//        }


//        // 一時中断用データをファイルにセーブする
//        public void Dump()
//        {
//            FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)Count());
//            foreach (Item it in colItems)
//                it.Dump();
//        }

//        // 一時中断用データをファイルからロードする
//        public void Restore()
//        {
//            short i, num = default;
//            Item it;
//            {
//                var withBlock = colItems;
//                var loopTo = (short)withBlock.Count;
//                for (i = 1; i <= loopTo; i++)
//                    withBlock.Remove(1);
//            }

//            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
//            var loopTo1 = num;
//            for (i = 1; i <= loopTo1; i++)
//            {
//                it = new Item();
//                it.Restore();
//                colItems.Add(it, it.ID);
//            }
//        }

//        // 一時中断用データのリンク情報をファイルからロードする
//        public void RestoreLinkInfo()
//        {
//            var num = default(short);
//            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
//            foreach (Item it in colItems)
//                it.RestoreLinkInfo();
//        }

//        // 一時中断用データのパラメータ情報をファイルからロードする
//        public void RestoreParameter()
//        {
//            var num = default(short);
//            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
//            foreach (Item it in colItems)
//                it.RestoreParameter();
//        }


//        // リストをクリア
//        public void Clear()
//        {
//            short i;
//            var loopTo = Count();
//            for (i = 1; i <= loopTo; i++)
//            {
//                Delete(ref 1);
//            }
//        }
//    }
//}
