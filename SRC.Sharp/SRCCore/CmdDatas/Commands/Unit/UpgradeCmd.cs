using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class UpgradeCmd : CmdData
    {
        public UpgradeCmd(SRC src, EventDataLine eventData) : base(src, CmdType.UpgradeCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
