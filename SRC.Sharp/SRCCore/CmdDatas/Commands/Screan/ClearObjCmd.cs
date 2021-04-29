using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ClearObjCmd : CmdData
    {
        public ClearObjCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ClearObjCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
