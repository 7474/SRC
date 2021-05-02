using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class OptionCmd : CmdData
    {
        public OptionCmd(SRC src, EventDataLine eventData) : base(src, CmdType.OptionCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
