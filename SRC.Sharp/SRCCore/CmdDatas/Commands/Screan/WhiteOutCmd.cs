using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class WhiteOutCmd : CmdData
    {
        public WhiteOutCmd(SRC src, EventDataLine eventData) : base(src, CmdType.WhiteOutCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
