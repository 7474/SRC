// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Extensions;
using SRCCore.Units;
using System.Collections.Generic;

namespace SRCCore.Models
{
    public interface ISituationItem
    {
        string Situation { get; }
        bool IsAvailable(Unit u, bool ignore_condition);
    }

    public class MessageDataItem : ISituationItem
    {
        public string Situation { get; }
        public string Message { get; }

        public MessageDataItem(string situation, string message)
        {
            Situation = situation;
            Message = message;
        }

        public bool IsAvailable(Unit u, bool ignore_condition)
        {
            // XXX MessageDataItem#IsAvailable 常に真でいいか
            return true;
        }
    }

    // メッセージデータのクラス
    // (戦闘アニメデータ及び特殊効果データのクラスも兼用)
    public class MessageData
    {
        // データのパイロット名
        // 戦闘アニメデータ及び特殊効果データの場合はユニット名またはユニットクラス
        public string Name;

        public IList<MessageDataItem> Items { get; } = new List<MessageDataItem>();

        private SRC SRC;
        private Commands.Command Commands => SRC.Commands;
        private Maps.Map Map => SRC.Map;
        public MessageData(SRC src)
        {
            SRC = src;
        }

        // メッセージを追加
        public void AddMessage(string sit, string msg)
        {
            Items.Add(new MessageDataItem(sit, msg));
        }

        // ユニット u のシチュエーション msg_situation におけるメッセージを選択
        public string SelectMessage(string msg_situation, Unit u = null)
        {
            return Items.SelectMessage(SRC, msg_situation, u)?.Message ?? "";
        }
    }
}
