using Microsoft.Extensions.Logging;
using SRCCore.Exceptions;

namespace SRCCore.Events
{
    public partial class Event
    {
        // イベントエラー表示
        public void DisplayEventErrorMessage(int lnum, string msg)
        {
            try
            {
                DisplayEventErrorMessage(EventData[lnum], msg);
            }
            catch
            {
                throw;
            }
        }

        public void DisplayEventErrorMessage(EventErrorException ex)
        {
            DisplayEventErrorMessage(ex?.EventData ?? EventDataLine.Empty, ex.Message);
        }

        public void DisplayEventErrorMessage(EventDataLine line, string msg)
        {
            SRC.DataErrors.Add(new InvalidSrcData(msg, line.File, line.LineNum, line.Data, "eve"));
            Debugger?.OnError(line, msg);

            string buf;

            // エラーが起こったファイル、行番号、エラーメッセージを表示
            buf = line.File + "：" + line.LineNum + "行目" + Constants.vbCr + Constants.vbLf + msg + Constants.vbCr + Constants.vbLf;

            // エラーが起こった行とその前後の行の内容を表示
            if (line.ID > 0 && line.ID - 1 < EventData.Count)
            {
                var preLine = EventData[line.ID - 1];
                buf = buf + preLine.LineNum + ": " + preLine.Data + Constants.vbCr + Constants.vbLf;
            }
            // VB 版はイベントデータの行をそのまま出す。サブルーチンの呼び出しとして読んだ行（CmdParser が「Call 」を前に付ける）も、
            // 書いたとおりに出す（元の SRC の実機で確かめた。probe-errors/eve-unknown。#1550）
            var data = line.Data;
            if (line.ID >= 0 && line.ID < EventData.Count && EventData[line.ID].LineNum == line.LineNum && EventData[line.ID].File == line.File)
            {
                data = EventData[line.ID].Data;
            }
            buf = buf + line.LineNum + ": " + data + Constants.vbCr + Constants.vbLf;
            if (line.ID < EventData.Count - 1)
            {
                var afterLine = EventData[line.ID + 1];
                buf = buf + afterLine.LineNum + ": " + afterLine.Data + Constants.vbCr + Constants.vbLf;
            }
            SRC.EventLog.LogError("イベントのエラー {File}({Line}): {Message}\n{Lines}", line.File, line.LineNum, msg, buf);
            GUI.ErrorMessage(buf);
        }
    }
}
