using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class NoonCmd : CmdData
    {
        public NoonCmd(SRC src, EventDataLine eventData) : base(src, CmdType.NoonCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
