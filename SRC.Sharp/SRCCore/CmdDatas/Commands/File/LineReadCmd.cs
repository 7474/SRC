using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class LineReadCmd : CmdData
    {
        public LineReadCmd(SRC src, EventDataLine eventData) : base(src, CmdType.LineReadCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
