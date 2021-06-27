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

        public void DisplayEventErrorMessage(EventDataLine line, string msg)
        {
            SRC.DataErrors.Add(new InvalidSrcData(msg, line.File, line.LineNum, line.Data, "eve"));

            string buf;

            // エラーが起こったファイル、行番号、エラーメッセージを表示
            buf = line.File + "：" + line.LineNum + "行目" + Constants.vbCr + Constants.vbLf + msg + Constants.vbCr + Constants.vbLf;

            // エラーが起こった行とその前後の行の内容を表示
            if (line.ID > 0)
            {
                var preLine = EventData[line.ID - 1];
                buf = buf + preLine.LineNum + ": " + preLine.Data + Constants.vbCr + Constants.vbLf;
            }
            buf = buf + line.LineNum + ": " + line.Data + Constants.vbCr + Constants.vbLf;
            if (line.ID < EventData.Count - 1)
            {
                var afterLine = EventData[line.ID + 1];
                buf = buf + afterLine.LineNum + ": " + afterLine.Data + Constants.vbCr + Constants.vbLf;
            }
            SRC.LogError(buf);
            GUI.ErrorMessage(buf);
        }
    }
}
