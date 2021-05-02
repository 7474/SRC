using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RequireCmd : CmdData
    {
        public RequireCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RequireCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
