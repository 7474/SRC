using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class ElseIfCmd : AIfCmd
    {
        public ElseIfCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ElseIfCmd, eventData)
        {
            PrepareArgs();
        }

        protected override int ExecInternal()
        {
            // 直前の If や ElseIf コマンドの実行の終了を意味する。
            return AIfCmd.ToEnd(this);
        }
    }
}
