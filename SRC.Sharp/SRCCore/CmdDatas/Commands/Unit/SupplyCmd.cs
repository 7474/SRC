using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SupplyCmd : CmdData
    {
        public SupplyCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SupplyCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
