using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ColorFilterCmd : CmdData
    {
        public ColorFilterCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ColorFilterCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
