using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class EnableCmd : CmdData
    {
        public EnableCmd(SRC src, EventDataLine eventData) : base(src, CmdType.EnableCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
