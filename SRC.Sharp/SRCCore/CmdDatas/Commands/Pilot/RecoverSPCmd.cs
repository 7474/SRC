using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RecoverSPCmd : CmdData
    {
        public RecoverSPCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RecoverSPCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
