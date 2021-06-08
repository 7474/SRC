using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class DrawWidthCmd : CmdData
    {
        public DrawWidthCmd(SRC src, EventDataLine eventData) : base(src, CmdType.DrawWidthCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 2)
            {
                throw new EventErrorException(this, "DrawWidthコマンドの引数の数が違います");
            }

            Event.ObjDrawWidth = GetArgAsLong(2);
            return EventData.NextID;
        }
    }
}
