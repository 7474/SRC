using SRC.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRC.Core.CmdDatas.Commands
{
    public class EndCmd : CmdData
    {
        public EndCmd(SRC src, EventDataLine eventData) : base(src, CmdType.EndCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            return EventData.ID + 1;
        }
    }
}
