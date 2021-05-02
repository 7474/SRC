using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ArcCmd : CmdData
    {
        public ArcCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ArcCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
