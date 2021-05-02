using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class MapAbilityCmd : CmdData
    {
        public MapAbilityCmd(SRC src, EventDataLine eventData) : base(src, CmdType.MapAbilityCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
