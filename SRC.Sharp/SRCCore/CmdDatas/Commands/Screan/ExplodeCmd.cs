using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ExplodeCmd : CmdData
    {
        public ExplodeCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ExplodeCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
