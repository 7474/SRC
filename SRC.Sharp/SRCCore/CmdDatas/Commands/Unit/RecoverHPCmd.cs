using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;

namespace SRCCore.CmdDatas.Commands
{
    public class RecoverHPCmd : CmdData
    {
        public RecoverHPCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RecoverHPCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
            double per;
            switch (ArgNum)
            {
                case 3:
                    {
                        u = GetArgAsUnit(2, true);
                        per = GetArgAsDouble(3);
                        break;
                    }

                case 2:
                    {
                        u = Event.SelectedUnitForEvent;
                        per = GetArgAsDouble(2);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "RecoverHPコマンドの引数の数が違います");
            }

            if (u is object)
            {
                u.RecoverHP(per);
                u.Update();
                u.CheckAutoHyperMode();
                u.CheckAutoNormalMode();
            }

            return EventData.NextID;
        }
    }
}
