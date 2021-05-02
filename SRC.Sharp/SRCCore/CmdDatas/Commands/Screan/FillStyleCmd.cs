using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class FillStyleCmd : CmdData
    {
        public FillStyleCmd(SRC src, EventDataLine eventData) : base(src, CmdType.FillStyleCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
