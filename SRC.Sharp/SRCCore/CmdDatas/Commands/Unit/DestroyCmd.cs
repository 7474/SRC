using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class DestroyCmd : CmdData
    {
        public DestroyCmd(SRC src, EventDataLine eventData) : base(src, CmdType.DestroyCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
