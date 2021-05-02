using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SetStatusCmd : CmdData
    {
        public SetStatusCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetStatusCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
