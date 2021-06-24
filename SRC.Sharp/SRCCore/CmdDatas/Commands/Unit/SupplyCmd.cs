using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;

namespace SRCCore.CmdDatas.Commands
{
    public class SupplyCmd : CmdData
    {
        public SupplyCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SupplyCmd, eventData)
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
                    throw new EventErrorException(this, "Supplyコマンドの引数の数が違います");
            }

            if (u is object)
            {
                u.FullSupply();
            }

            return EventData.NextID;
        }
    }
}
