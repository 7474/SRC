using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class CircleCmd : CmdData
    {
        public CircleCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CircleCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
