using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class MoneyCmd : CmdData
    {
        public MoneyCmd(SRC src, EventDataLine eventData) : base(src, CmdType.MoneyCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
