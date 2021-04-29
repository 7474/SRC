using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ExchangeItemCmd : CmdData
    {
        public ExchangeItemCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ExchangeItemCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
