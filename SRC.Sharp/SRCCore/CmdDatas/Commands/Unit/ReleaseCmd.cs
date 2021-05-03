using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    // 分類上はパイロット操作、アイテム操作
    public class ReleaseCmd : CmdData
    {
        public ReleaseCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ReleaseCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
