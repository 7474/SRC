using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RenameTermCmd : CmdData
    {
        public RenameTermCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RenameTermCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
