using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RankUpCmd : CmdData
    {
        public RankUpCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RankUpCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
