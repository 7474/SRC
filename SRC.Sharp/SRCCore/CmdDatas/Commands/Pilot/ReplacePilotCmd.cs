using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ReplacePilotCmd : CmdData
    {
        public ReplacePilotCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ReplacePilotCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
