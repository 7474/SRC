using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RenameFileCmd : CmdData
    {
        public RenameFileCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RenameFileCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
