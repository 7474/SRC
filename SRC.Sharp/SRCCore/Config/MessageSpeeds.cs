using System;
using System.Collections.Generic;

namespace SRCCore.Config
{
    /// <summary>
    /// メッセージスピード（設定変更の画面の選択肢と、そのメッセージのウェイト）。
    /// 元の SRC の Configuration.frm（cboMessageSpeed）・SRC.bas（Src.ini の [Option] MessageWait）と同じ。
    /// </summary>
    public static class MessageSpeeds
    {
        /// <summary>メッセージのウェイトの既定値（普通）</summary>
        public const int DefaultWait = 700;

        /// <summary>メッセージのウェイトの上限（手動送り。元の SRC はこれより大きい値をこの値にする）</summary>
        public const int MaxWait = 10000000;

        /// <summary>選択肢（元の SRC の設定変更の画面の並び）。Name は元の SRC の表示</summary>
        public static readonly IReadOnlyList<(string Name, int Wait)> Items = new[]
        {
            ("手動送り", MaxWait),
            ("低速", 1000),
            ("普通", DefaultWait),
            ("高速", 400),
            ("超高速", 200),
            ("神の領域", 0),
        };

        /// <summary>
        /// ウェイトに当たる選択肢の位置。どれにも当たらなければ -1
        /// （元の SRC は選択肢に無い値なら何も選ばず、そのまま OK を押しても値を変えない）
        /// </summary>
        public static int IndexOf(int wait)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                if (Items[i].Wait == wait)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>設定の値をメッセージのウェイトにする（元の SRC と同じく上限を超えたら上限にする）</summary>
        public static int Normalize(int wait)
        {
            return Math.Min(wait, MaxWait);
        }
    }
}
