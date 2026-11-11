// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Extensions;
using SRCCore.Units;
using System.Collections.Generic;

namespace SRCCore.Models
{
    // 特定のパイロットに指定された全ダイアログのクラス
    public class DialogData
    {
        // パイロット一覧
        public string Name = "";

        // ダイアログ一覧
        private IList<Dialog> Dialoges = new List<Dialog>();

        public IList<Dialog> Items => Dialoges;

        private SRC SRC;
        public DialogData(SRC src)
        {
            SRC = src;
        }

        // ダイアログを追加
        public Dialog AddDialog(string msg_situation)
        {
            var new_dialog = new Dialog(SRC)
            {
                Situation = msg_situation,
            };
            Dialoges.Add(new_dialog);
            return new_dialog;
        }

        // ダイアログの総数
        public int CountDialog()
        {
            return Dialoges.Count;
        }

        // ユニット u のシチュエーション msg_situation におけるダイアログを選択
        public Dialog SelectDialog(string msg_situation, Unit u, bool ignore_condition = false)
        {
            return Items.SelectMessage(SRC, msg_situation, u, ignore_condition);
        }
    }
}
