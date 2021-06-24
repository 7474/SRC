using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;

namespace SRCCore.CmdDatas.Commands
{
    public class ClearStatusCmd : CmdData
    {
        public ClearStatusCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ClearStatusCmd, eventData)
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
                    throw new EventErrorException(this, "ClearStatusコマンドの引数の数が違います");
            }

            {
                var withBlock = u;
                if (withBlock.IsConditionSatisfied(sname))
                {
                    withBlock.DeleteCondition(sname);
                    withBlock.Update();
                    if (withBlock.Status == "出撃")
                    {
                        GUI.PaintUnitBitmap(u);
                    }
                }
            }
            return EventData.NextID;
        }
    }
}
