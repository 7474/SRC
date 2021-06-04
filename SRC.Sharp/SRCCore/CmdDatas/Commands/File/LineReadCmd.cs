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
            Expression.SetVariableAsString(GetArg(3), buf);
            return EventData.NextID;
        }
    }
}
