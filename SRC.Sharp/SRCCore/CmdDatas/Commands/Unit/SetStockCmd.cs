using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SetStockCmd : CmdData
    {
        public SetStockCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetStockCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
