using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Expressions;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class UnSetCmd : CmdData
    {
        public UnSetCmd(SRC src, EventDataLine eventData) : base(src, CmdType.UnsetCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Expression.UndefineVariable(GetArg(2));

            return EventData.NextID;
        }
    }
}
