using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    // 分類上はパイロット操作、ユニット操作
    public class JoinCmd : CmdData
    {
        public JoinCmd(SRC src, EventDataLine eventData) : base(src, CmdType.JoinCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
