using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class LoadCmd : CmdData
    {
        public LoadCmd(SRC src, EventDataLine eventData) : base(src, CmdType.LoadCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
