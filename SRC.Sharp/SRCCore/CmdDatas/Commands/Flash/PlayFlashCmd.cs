using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class PlayFlashCmd : CmdData
    {
        public PlayFlashCmd(SRC src, EventDataLine eventData) : base(src, CmdType.PlayFlashCmd, eventData)
        {
            throw new NotImplementedException("PlayFlashコマンドはサポートされていません。");
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
        }
    }
}
