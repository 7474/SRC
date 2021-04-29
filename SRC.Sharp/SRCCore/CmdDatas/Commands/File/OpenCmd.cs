using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class OpenCmd : CmdData
    {
        public OpenCmd(SRC src, EventDataLine eventData) : base(src, CmdType.OpenCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
