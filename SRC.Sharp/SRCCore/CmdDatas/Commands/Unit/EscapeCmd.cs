using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class EscapeCmd : CmdData
    {
        public EscapeCmd(SRC src, EventDataLine eventData) : base(src, CmdType.EscapeCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
