using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;

namespace SRCCore.CmdDatas.Commands
{
    public class LandCmd : CmdData
    {
        public LandCmd(SRC src, EventDataLine eventData) : base(src, CmdType.LandCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u1, u2;
            switch (ArgNum)
            {
                case 2:
                    {
                        u1 = Event.SelectedUnitForEvent;
                        u2 = GetArgAsUnit(2);
                        break;
                    }

                case 3:
                    {
                        u1 = GetArgAsUnit(2);
                        u2 = GetArgAsUnit(3);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "Landコマンドの引数の数が違います");
            }

            if (u1.IsFeatureAvailable("母艦"))
            {
                throw new EventErrorException(this, u1.Name + "は母艦なので格納出来ません");
            }

            if (!u2.IsFeatureAvailable("母艦"))
            {
                throw new EventErrorException(this, u2.Name + "は母艦能力を持っていません");
            }

            u1.Land(u2, true, true);
            return EventData.NextID;
        }
    }
}
