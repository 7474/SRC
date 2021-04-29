using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ClearFlashCmd : CmdData
    {
        public ClearFlashCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ClearFlashCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
