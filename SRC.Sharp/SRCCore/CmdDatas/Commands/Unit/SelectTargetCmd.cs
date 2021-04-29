using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SelectTargetCmd : CmdData
    {
        public SelectTargetCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SelectTargetCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
