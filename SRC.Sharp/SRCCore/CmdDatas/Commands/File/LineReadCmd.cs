using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class LineReadCmd : CmdData
    {
        public LineReadCmd(SRC src, EventDataLine eventData) : base(src, CmdType.LineReadCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 3)
            {
                throw new EventErrorException(this, "LineReadコマンドの引数の数が違います");
            }

            var buf = SRC.FileHandleManager.Get(GetArgAsLong(2)).Reader.ReadLine();
            if (buf == null)
            {
                // ファイルの最後を超えて読み出した。元の SRC は Line Input # が実行時エラーになり、
                // イベントを止めて「イベントデータが不正です」を表示する
                throw new EventErrorException(this, "イベントデータが不正です");
            }
            Expression.SetVariableAsString(GetArg(3), buf);
            return EventData.NextID;
        }
    }
}
