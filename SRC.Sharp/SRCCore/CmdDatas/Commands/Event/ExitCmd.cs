using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class ExitCmd : CmdData
    {
        public ExitCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ExitCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            return -1;
        }
    }
}
