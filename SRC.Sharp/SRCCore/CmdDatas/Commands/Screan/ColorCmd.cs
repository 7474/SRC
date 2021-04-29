using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ColorCmd : CmdData
    {
        public ColorCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ColorCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
