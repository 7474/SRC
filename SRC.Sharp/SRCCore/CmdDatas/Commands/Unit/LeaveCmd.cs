using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    // ���ޏ�̓p�C���b�g����A���j�b�g����
    public class LeaveCmd : CmdData
    {
        public LeaveCmd(SRC src, EventDataLine eventData) : base(src, CmdType.LeaveCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}