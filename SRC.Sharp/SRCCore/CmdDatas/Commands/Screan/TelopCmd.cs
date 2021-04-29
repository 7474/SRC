using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class TelopCmd : CmdData
    {
        public TelopCmd(SRC src, EventDataLine eventData) : base(src, CmdType.TelopCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
