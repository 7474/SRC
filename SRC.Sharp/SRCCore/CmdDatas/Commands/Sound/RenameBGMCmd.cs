using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RenameBGMCmd : CmdData
    {
        public RenameBGMCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RenameBGMCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
