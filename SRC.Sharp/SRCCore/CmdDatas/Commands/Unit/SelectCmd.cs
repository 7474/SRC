using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class SelectCmd : CmdData
    {
        public SelectCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SelectCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 2)
            {
                throw new EventErrorException(this, "Selectコマンドの引数の数が違います");
            }

            Event.SelectedUnitForEvent = GetArgAsUnit(2);
            return EventData.NextID;
        }
    }
}
