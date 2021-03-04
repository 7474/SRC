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
            return EventData.ID + 1;
        }
    }
}
