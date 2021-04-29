using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RecoverHPCmd : CmdData
    {
        public RecoverHPCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RecoverHPCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
