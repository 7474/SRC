using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ForgetCmd : CmdData
    {
        public ForgetCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ForgetCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
