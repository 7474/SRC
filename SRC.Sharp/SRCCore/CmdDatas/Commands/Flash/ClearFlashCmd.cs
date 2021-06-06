using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ClearFlashCmd : CmdData
    {
        public ClearFlashCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ClearFlashCmd, eventData)
        {
            throw new NotSupportedException("ClearFlashコマンドはサポートされていません。");
        }

        protected override int ExecInternal()
        {
            throw new NotSupportedException();
        }
    }
}
