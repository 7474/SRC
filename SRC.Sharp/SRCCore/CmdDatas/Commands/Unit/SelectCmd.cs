using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SelectCmd : CmdData
    {
        public SelectCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SelectCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
