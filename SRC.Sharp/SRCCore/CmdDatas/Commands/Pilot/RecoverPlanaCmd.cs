using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RecoverPlanaCmd : CmdData
    {
        public RecoverPlanaCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RecoverPlanaCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
