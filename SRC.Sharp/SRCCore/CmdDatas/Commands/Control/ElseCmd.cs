using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class ElseCmd : CmdData
    {
        public ElseCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ElseCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            // 直前の If や ElseIf コマンドの実行の終了を意味する。
            return AIfCmd.ToEnd(this);
        }
    }
}
