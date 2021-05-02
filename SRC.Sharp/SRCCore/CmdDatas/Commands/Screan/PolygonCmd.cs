using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class PolygonCmd : CmdData
    {
        public PolygonCmd(SRC src, EventDataLine eventData) : base(src, CmdType.PolygonCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
