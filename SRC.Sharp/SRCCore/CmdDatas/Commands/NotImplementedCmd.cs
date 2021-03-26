using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class NotImplementedCmd : CmdData
    {
        public NotImplementedCmd(SRC src, EventDataLine eventData) : base(src, CmdType.NopCmd, eventData)
        {
            //throw new NotImplementedException();
        }

        protected override int ExecInternal()
        {
            return EventData.ID + 1;
        }
    }
}
