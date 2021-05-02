using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class EquipCmd : CmdData
    {
        public EquipCmd(SRC src, EventDataLine eventData) : base(src, CmdType.EquipCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
