using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class CombineCmd : CmdData
    {
        public CombineCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CombineCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
