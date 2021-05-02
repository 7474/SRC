using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ItemCmd : CmdData
    {
        public ItemCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ItemCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
