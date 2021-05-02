using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class DisableCmd : CmdData
    {
        public DisableCmd(SRC src, EventDataLine eventData) : base(src, CmdType.DisableCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
