using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;

namespace SRCCore.CmdDatas.Commands
{
    public class ShowUnitStatusCmd : CmdData
    {
        public ShowUnitStatusCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ShowUnitStatusCmd, eventData)
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
                        if (GetArgAsString(2) == "終了")
                        {
                            SRC.GUIStatus.ClearUnitStatus();
                            return EventData.NextID;
                        }

                        u = GetArgAsUnit(2);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "ShowUnitStatusコマンドの引数の数が違います");
            }

            if (u is object)
            {
                SRC.GUIStatus.DisplayUnitStatus(u);
            }

            return EventData.NextID;
        }
    }
}
