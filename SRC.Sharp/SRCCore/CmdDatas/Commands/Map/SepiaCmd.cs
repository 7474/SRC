using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SepiaCmd : CmdData
    {
        public SepiaCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SepiaCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
