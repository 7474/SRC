using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class MonotoneCmd : CmdData
    {
        public MonotoneCmd(SRC src, EventDataLine eventData) : base(src, CmdType.MonotoneCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
