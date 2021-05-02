using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class DrawOptionCmd : CmdData
    {
        public DrawOptionCmd(SRC src, EventDataLine eventData) : base(src, CmdType.DrawOptionCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
