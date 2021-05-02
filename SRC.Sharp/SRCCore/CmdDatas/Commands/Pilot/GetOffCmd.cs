using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class GetOffCmd : CmdData
    {
        public GetOffCmd(SRC src, EventDataLine eventData) : base(src, CmdType.GetOffCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
