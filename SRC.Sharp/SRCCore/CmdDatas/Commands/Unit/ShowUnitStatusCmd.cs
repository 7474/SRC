using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ShowUnitStatusCmd : CmdData
    {
        public ShowUnitStatusCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ShowUnitStatusCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
