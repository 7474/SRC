using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;

namespace SRCCore.CmdDatas.Commands
{
    public class RecoverENCmd : CmdData
    {
        public RecoverENCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RecoverENCmd, eventData)
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
                    throw new EventErrorException(this, "RecoverENコマンドの引数の数が違います");
            }

            if (u is object)
            {
                {
                    var withBlock = u;
                    withBlock.RecoverEN(per);
                    withBlock.Update();
                    if (withBlock.EN == 0 && withBlock.Status == "出撃")
                    {
                        GUI.PaintUnitBitmap(u);
                    }

                    withBlock.CheckAutoHyperMode();
                    withBlock.CheckAutoNormalMode();
                }
            }
            return EventData.NextID;
        }
    }
}
