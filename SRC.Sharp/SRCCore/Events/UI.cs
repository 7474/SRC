using System;
using System.Collections.Generic;
using System.Text;

namespace SRC.Core.Events
{
    public partial class Event
    {
        // イベントエラー表示
        public void DisplayEventErrorMessage(int lnum, string msg)
        {
            string buf;

            // XXX 多分配列のIndex処理しないと落ちる
            // エラーが起こったファイル、行番号、エラーメッセージを表示
            buf = EventFileNames[EventFileID[lnum]] + "：" + EventLineNum[lnum] + "行目" + Constants.vbCr + Constants.vbLf + msg + Constants.vbCr + Constants.vbLf;

            // エラーが起こった行とその前後の行の内容を表示
            if (lnum > 1)
            {
                buf = buf + EventLineNum[lnum - 1] + ": " + EventData[lnum - 1] + Constants.vbCr + Constants.vbLf;
            }

            buf = buf + EventLineNum[lnum] + ": " + EventData[lnum] + Constants.vbCr + Constants.vbLf;
            if (lnum < EventData.Length)
            {
                buf = buf + EventLineNum[lnum + 1] + ": " + EventData[lnum + 1] + Constants.vbCr + Constants.vbLf;
            }

            GUI.ErrorMessage(buf);
        }

    }
}
