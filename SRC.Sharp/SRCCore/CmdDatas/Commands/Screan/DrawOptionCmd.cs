using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class DrawOptionCmd : CmdData
    {
        public DrawOptionCmd(SRC src, EventDataLine eventData) : base(src, CmdType.DrawOptionCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 2)
            {
                throw new EventErrorException(this, "DrawOptionコマンドの引数の数が違います");
            }

            Event.ObjDrawOption = GetArgAsString(2);
            return EventData.NextID;
        }
    }
}
