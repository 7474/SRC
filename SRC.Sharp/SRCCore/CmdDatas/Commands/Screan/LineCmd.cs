using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class LineCmd : CmdData
    {
        public LineCmd(SRC src, EventDataLine eventData) : base(src, CmdType.LineCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
