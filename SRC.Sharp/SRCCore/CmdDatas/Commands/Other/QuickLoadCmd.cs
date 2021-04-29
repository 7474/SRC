using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class QuickLoadCmd : CmdData
    {
        public QuickLoadCmd(SRC src, EventDataLine eventData) : base(src, CmdType.QuickLoadCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
