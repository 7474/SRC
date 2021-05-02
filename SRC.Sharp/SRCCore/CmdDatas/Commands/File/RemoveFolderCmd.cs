using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RemoveFolderCmd : CmdData
    {
        public RemoveFolderCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RemoveFolderCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
