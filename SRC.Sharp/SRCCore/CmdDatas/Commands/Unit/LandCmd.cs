using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class LandCmd : CmdData
    {
        public LandCmd(SRC src, EventDataLine eventData) : base(src, CmdType.LandCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
