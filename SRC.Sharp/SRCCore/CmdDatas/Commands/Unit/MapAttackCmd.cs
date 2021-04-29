using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class MapAttackCmd : CmdData
    {
        public MapAttackCmd(SRC src, EventDataLine eventData) : base(src, CmdType.MapAttackCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
