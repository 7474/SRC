using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class NotImplementedCmd : CmdData
    {
        public NotImplementedCmd(SRC src, EventDataLine eventData) : base(src, CmdType.NopCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
        }
    }
}
