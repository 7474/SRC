using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class SelectTargetCmd : CmdData
    {
        public SelectTargetCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SelectTargetCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string pname;
            if (ArgNum != 2)
            {
                throw new EventErrorException(this, "SelectTargetコマンドの引数の数が違います");
            }

            Event.SelectedTargetForEvent = GetArgAsUnit(2);
            return EventData.NextID;
        }
    }
}
