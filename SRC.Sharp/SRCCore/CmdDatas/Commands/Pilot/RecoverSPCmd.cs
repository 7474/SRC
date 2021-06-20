using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Pilots;

namespace SRCCore.CmdDatas.Commands
{
    public class RecoverSPCmd : CmdData
    {
        public RecoverSPCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RecoverSPCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Pilot p = null;
            double per;
            switch (ArgNum)
            {
                case 3:
                    {
                        p = GetArgAsPilot(2);
                        per = GetArgAsDouble(3);
                        break;
                    }

                case 2:
                    {
                        {
                            var withBlock = Event.SelectedUnitForEvent;
                            if (withBlock.CountPilot() > 0)
                            {
                                p = withBlock.MainPilot();
                            }
                        }

                        per = GetArgAsDouble(2);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "RecoverSPコマンドの引数の数が違います");
            }

            if (p is object)
            {
                if (p.MaxSP > 0)
                {
                    p.SP = (int)(p.SP + per * p.MaxSP / 100d);
                }
            }

            return EventData.NextID;
        }
    }
}
