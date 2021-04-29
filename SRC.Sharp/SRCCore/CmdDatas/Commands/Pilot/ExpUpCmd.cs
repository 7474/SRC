using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ExpUpCmd : CmdData
    {
        public ExpUpCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ExpUpCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
