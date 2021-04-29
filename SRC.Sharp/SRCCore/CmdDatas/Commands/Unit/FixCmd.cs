using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class FixCmd : CmdData
    {
        public FixCmd(SRC src, EventDataLine eventData) : base(src, CmdType.FixCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
