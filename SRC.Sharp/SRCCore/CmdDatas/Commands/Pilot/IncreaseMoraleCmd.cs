using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;
using SRCCore.VB;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class IncreaseMoraleCmd : CmdData
    {
        public IncreaseMoraleCmd(SRC src, EventDataLine eventData) : base(src, CmdType.IncreaseMoraleCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
            string num;
            switch (ArgNum)
            {
                case 3:
                    {
                        u = GetArgAsUnit(2, true);
                        num = GetArgAsLong(3).ToString();
                        break;
                    }

                case 2:
                    {
                        u = Event.SelectedUnitForEvent;
                        num = GetArgAsLong(2).ToString();
                        break;
                    }

                default:
                    throw new EventErrorException(this, "IncreaseMoraleコマンドの引数の数が違います");
            }

            if (u is object)
            {
                u.IncreaseMorale(Conversions.ToInteger(num), true);
                u.CurrentForm().CheckAutoHyperMode();
                u.CurrentForm().CheckAutoNormalMode();
            }

            return EventData.NextID;
        }
    }
}
