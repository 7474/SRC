using SRCCore.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.CmdDatas.Commands
{
    public class SuspendCmd : CmdData
    {
        public SuspendCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SuspendCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            return EventData.ID + 1;
        }
    }
}
