using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class DrawWidthCmd : CmdData
    {
        public DrawWidthCmd(SRC src, EventDataLine eventData) : base(src, CmdType.DrawWidthCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
