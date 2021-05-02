using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class HotPointCmd : CmdData
    {
        public HotPointCmd(SRC src, EventDataLine eventData) : base(src, CmdType.HotPointCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
