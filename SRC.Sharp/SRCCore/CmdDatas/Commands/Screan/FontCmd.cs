using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class FontCmd : CmdData
    {
        public FontCmd(SRC src, EventDataLine eventData) : base(src, CmdType.FontCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
