using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    // ���ޏ�̓p�C���b�g����A���j�b�g����
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
