using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SunsetCmd : CmdData
    {
        public SunsetCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SunsetCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
