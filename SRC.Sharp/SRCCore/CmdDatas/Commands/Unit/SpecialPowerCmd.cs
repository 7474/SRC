using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SpecialPowerCmd : CmdData
    {
        public SpecialPowerCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SpecialPowerCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
