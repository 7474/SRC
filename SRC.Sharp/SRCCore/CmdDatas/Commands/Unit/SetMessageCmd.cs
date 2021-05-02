using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SetMessageCmd : CmdData
    {
        public SetMessageCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetMessageCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
