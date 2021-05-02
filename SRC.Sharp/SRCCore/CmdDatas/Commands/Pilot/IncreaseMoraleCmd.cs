using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class IncreaseMoraleCmd : CmdData
    {
        public IncreaseMoraleCmd(SRC src, EventDataLine eventData) : base(src, CmdType.IncreaseMoraleCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
