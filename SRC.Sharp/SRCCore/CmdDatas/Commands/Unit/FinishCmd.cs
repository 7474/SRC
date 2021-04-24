using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;

namespace SRCCore.CmdDatas.Commands
{
    public class FinishCmd : CmdData
    {
        public FinishCmd(SRC src, EventDataLine eventData) : base(src, CmdType.FinishCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
            switch (ArgNum)
            {
                case 2:
                    u = GetArgAsUnit(2, true);
                    break;

                case 1:
                    u = Event.SelectedUnitForEvent;
                    break;

                default:
                    throw new EventErrorException(this, "Finishコマンドの引数の数が違います");
            }

            if (u != null)
            {
                switch (u.Action)
                {
                    case 1:
                        u.UseAction();
                        if (u.Status == "出撃")
                        {
                            GUI.PaintUnitBitmap(u);
                        }

                        break;
                    // なにもしない
                    case 0:
                        break;

                    default:
                        u.UseAction();
                        break;
                }
            }

            return EventData.ID + 1;
        }
    }
}
