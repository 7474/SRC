using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RemoveUnitCmd : CmdData
    {
        public RemoveUnitCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RemoveUnitCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
