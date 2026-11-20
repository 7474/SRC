// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;

namespace SRCCore.Items
{
    // 元の SRC（VB5）のセーブデータのアイテムを読む（Items.cls の Load・LoadLinkInfo・Restore・RestoreLinkInfo・RestoreParameter。#1577）
    public partial class Items
    {
        /// <summary>アイテムＩＤ作成用カウンタ（VB: Items.cls の Save が書く IDCount）</summary>
        internal int CurrentIDCount => IDCount;

        // データをファイルからロード（VB: Load）
        public void Load(VbSaveDataReader reader)
        {
            // アイテムを書いていない古いクリアデータ
            if (reader.EOF)
            {
                return;
            }

            IDCount = reader.InputInt();
            var num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                // Name
                var iname = reader.Input();
                // ID, Unit
                var iid = reader.Input();
                reader.Input();

                if (!SRC.IDList.IsDefined(iname))
                {
                    // 元の SRC はＢＧＭを止めて End する
                    SRC.Sound.StopBGM();
                    throw SRC.TerminateLoading(iname + "のデータが定義されていません");
                }

                var new_item = new Item(SRC)
                {
                    Name = iname,
                    ID = iid,
                };
                colItems.Add(new_item, iid);
            }
        }

        // リンク情報をファイルからロード（VB: LoadLinkInfo。読み飛ばすだけ）
        public void LoadLinkInfo(VbSaveDataReader reader)
        {
            if (reader.EOF)
            {
                return;
            }

            // IDCount
            reader.LineInput();

            var num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                // Name
                reader.LineInput();
                // ID, Unit
                reader.LineInput();
            }
        }

        // 一時中断用データをファイルからロードする（VB: Restore）
        public void Restore(VbSaveDataReader reader)
        {
            colItems.Clear();

            var num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                var it = new Item(SRC);
                it.Restore(reader);
                colItems.Add(it, it.ID);
            }
        }

        // 一時中断用データのリンク情報をファイルからロードする（VB: RestoreLinkInfo）
        public void RestoreLinkInfo(VbSaveDataReader reader)
        {
            reader.InputInt();
            foreach (var it in colItems.List)
            {
                it.RestoreLinkInfo(reader);
            }
        }

        // 一時中断用データのパラメータ情報をファイルからロードする（VB: RestoreParameter）
        public void RestoreParameter(VbSaveDataReader reader)
        {
            reader.InputInt();
            foreach (var it in colItems.List)
            {
                it.RestoreParameter(reader);
            }
        }
    }
}
