using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class CreateFolderCmd : CmdData
    {
        public CreateFolderCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CreateFolderCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
