using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RecoverENCmd : CmdData
    {
        public RecoverENCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RecoverENCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
