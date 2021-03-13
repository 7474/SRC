using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class RestoreEventCmd : CmdData
    {
        public RestoreEventCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RestoreEventCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if ((int)ArgNum != 2)
            {
                throw new EventErrorException(this, "RestoreEventコマンドの引数の数が違います");
            }

            Event.RestoreLabel(GetArgAsString(2));

            return EventData.ID + 1;
        }
    }
}
