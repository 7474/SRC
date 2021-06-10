using SRCCore.Events;
using SRCCore.Lib;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class NotSupportedCmd : CmdData
    {
        public NotSupportedCmd(SRC src, EventDataLine eventData) : base(src, CmdType.NopCmd, eventData)
        {
            throw new NotSupportedException(GeneralLib.ListIndex(eventData.Data, 1) + "コマンドはサポートされていません。");
        }

        protected override int ExecInternal()
        {
            throw new NotSupportedException();
        }
    }
}
