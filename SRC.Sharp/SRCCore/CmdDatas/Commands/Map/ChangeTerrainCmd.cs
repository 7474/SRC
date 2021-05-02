using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ChangeTerrainCmd : CmdData
    {
        public ChangeTerrainCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ChangeTerrainCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
