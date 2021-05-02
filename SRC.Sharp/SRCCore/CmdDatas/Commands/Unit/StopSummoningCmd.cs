using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class StopSummoningCmd : CmdData
    {
        public StopSummoningCmd(SRC src, EventDataLine eventData) : base(src, CmdType.StopSummoningCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
