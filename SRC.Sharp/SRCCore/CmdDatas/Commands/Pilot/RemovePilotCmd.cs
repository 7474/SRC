using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RemovePilotCmd : CmdData
    {
        public RemovePilotCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RemovePilotCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
