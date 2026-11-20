// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;

namespace SRCCore.Items
{
    // 元の SRC（VB5）の中断データのアイテムを読む（Item.cls の Restore・RestoreLinkInfo・RestoreParameter。#1577）
    public partial class Item
    {
        // 一時中断用データをファイルからロードする（VB: Restore）
        public void Restore(VbSaveDataReader reader)
        {
            // Name, ID, Exist
            Name = reader.Input();
            ID = reader.Input();
            Exist = reader.InputBool();

            // Unit
            reader.LineInput();
        }

        // 一時中断用データのリンク情報をファイルからロードする（VB: RestoreLinkInfo）
        public void RestoreLinkInfo(VbSaveDataReader reader)
        {
            // Name, ID, Exist
            reader.LineInput();

            // Unit
            var sbuf = reader.Input();
            if (SRC.UList.IsDefined(sbuf))
            {
                Unit = SRC.UList.Item(sbuf);
            }
        }

        // 一時中断用データのパラメータ情報をファイルからロードする（VB: RestoreParameter）
        public void RestoreParameter(VbSaveDataReader reader)
        {
            // Name, ID, Exist
            reader.LineInput();
            // Unit
            reader.LineInput();
        }
    }
}
