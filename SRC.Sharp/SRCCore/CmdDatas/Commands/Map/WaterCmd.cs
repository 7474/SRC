using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class WaterCmd : CmdData
    {
        public WaterCmd(SRC src, EventDataLine eventData) : base(src, CmdType.WaterCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
