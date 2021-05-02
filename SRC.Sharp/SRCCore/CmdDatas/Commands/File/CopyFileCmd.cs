using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class CopyFileCmd : CmdData
    {
        public CopyFileCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CopyFileCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
