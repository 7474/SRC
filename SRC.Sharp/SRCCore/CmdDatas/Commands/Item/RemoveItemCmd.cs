using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RemoveItemCmd : CmdData
    {
        public RemoveItemCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RemoveItemCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
