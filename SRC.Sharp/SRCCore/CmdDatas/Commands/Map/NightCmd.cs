using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class NightCmd : CmdData
    {
        public NightCmd(SRC src, EventDataLine eventData) : base(src, CmdType.NightCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
