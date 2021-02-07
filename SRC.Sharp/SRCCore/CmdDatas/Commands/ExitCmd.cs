using SRCCore.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.CmdDatas.Commands
{
    public class ExitCmd : CmdData
    {
        public ExitCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ExitCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            return -1;
        }
    }
}
