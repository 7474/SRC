using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;

namespace SRCCore.CmdDatas.Commands
{
    public class ExchangeItemCmd : CmdData
    {
        public ExchangeItemCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ExchangeItemCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
            string ipart = "";
            switch (ArgNum)
            {
                case 1:
                    {
                        u = Event.SelectedUnitForEvent;
                        break;
                    }

                case 2:
                    {
                        u = GetArgAsUnit(2);
                        break;
                    }

                case 3:
                    {
                        u = GetArgAsUnit(2);
                        ipart = GetArgAsString(3);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "ExchangeItemコマンドの引数の数が違います");
            }

            SRC.InterMission.ExchangeItemCommand(u, ipart);
            return EventData.NextID;
        }
    }
}
