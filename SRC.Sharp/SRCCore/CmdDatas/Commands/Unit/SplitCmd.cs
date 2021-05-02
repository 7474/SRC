using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SplitCmd : CmdData
    {
        public SplitCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SplitCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
