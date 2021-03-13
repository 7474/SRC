using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class CancelCmd : CmdData
    {
        public CancelCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CancelCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            SRC.IsCanceled = true;
            return EventData.ID + 1;
        }
    }
}
