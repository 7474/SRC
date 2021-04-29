using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class PSetCmd : CmdData
    {
        public PSetCmd(SRC src, EventDataLine eventData) : base(src, CmdType.PSetCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
