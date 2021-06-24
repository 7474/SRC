using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;

namespace SRCCore.CmdDatas.Commands
{
    public class ClearSpecialPowerCmd : CmdData
    {
        public ClearSpecialPowerCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ClearSpecialPowerCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string sname;
            Unit u;
            switch (ArgNum)
            {
                case 3:
                    {
                        u = GetArgAsUnit(2);
                        sname = GetArgAsString(3);
                        break;
                    }

                case 2:
                    {
                        u = Event.SelectedUnitForEvent;
                        sname = GetArgAsString(2);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "ClearSpecialPowerコマンドの引数の数が違います");
            }

            if (u.IsSpecialPowerInEffect(sname))
            {
                u.RemoveSpecialPowerInEffect2(sname);
            }

            return EventData.NextID;
        }
    }
}
