using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;

namespace SRCCore.CmdDatas.Commands
{
    public class StopSummoningCmd : CmdData
    {
        public StopSummoningCmd(SRC src, EventDataLine eventData) : base(src, CmdType.StopSummoningCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
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

                default:
                    throw new EventErrorException(this, "StopSummoningコマンドの引数の数が違います");
            }

            // 召喚したユニットを解放
            u.DismissServant();
            return EventData.NextID;
        }
    }
}
