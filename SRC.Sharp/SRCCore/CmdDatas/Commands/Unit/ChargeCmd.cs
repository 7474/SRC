using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;

namespace SRCCore.CmdDatas.Commands
{
    public class ChargeCmd : CmdData
    {
        public ChargeCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ChargeCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
            switch (ArgNum)
            {
                case 2:
                    {
                        u = GetArgAsUnit(2);
                        break;
                    }

                case 1:
                    {
                        u = Event.SelectedUnitForEvent;
                        break;
                    }

                default:
                    throw new EventErrorException(this, "Chargeコマンドの引数の数が違います");
            }

            u.AddCondition("チャージ", 1, cdata: "");
            return EventData.NextID;
        }
    }
}
