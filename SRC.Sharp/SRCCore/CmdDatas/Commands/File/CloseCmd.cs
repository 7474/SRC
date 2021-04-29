using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class CloseCmd : CmdData
    {
        public CloseCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CloseCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
