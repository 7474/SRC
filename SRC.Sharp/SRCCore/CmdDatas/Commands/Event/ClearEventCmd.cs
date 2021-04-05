using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class ClearEventCmd : CmdData
    {
        public ClearEventCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ClearEventCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            switch (ArgNum)
            {
                case 2:
                    var ret = Event.FindLabel(GetArgAsString(2));
                    if (ret >= 0)
                    {
                        Event.ClearLabel(ret);
                    }

                    break;

                case 1:
                    if (Event.CurrentLabel >= 0)
                    {
                        Event.ClearLabel(Event.CurrentLabel);
                    }
                    break;

                default:
                    throw new EventErrorException(this, "ClearEventコマンドの引数の数が違います");
            }

            return EventData.ID + 1;
        }
    }
}
