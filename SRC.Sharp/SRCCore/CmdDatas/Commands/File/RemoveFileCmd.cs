using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RemoveFileCmd : CmdData
    {
        public RemoveFileCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RemoveFileCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
