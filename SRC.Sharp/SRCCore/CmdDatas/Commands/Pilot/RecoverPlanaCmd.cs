using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Pilots;

namespace SRCCore.CmdDatas.Commands
{
    public class RecoverPlanaCmd : CmdData
    {
        public RecoverPlanaCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RecoverPlanaCmd, eventData)
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
                    throw new EventErrorException(this, "RecoverPlanaコマンドの引数の数が違います");
            }

            if (p is null)
            {
                return EventData.NextID;
            }

            if (p.MaxPlana() == 0)
            {
                return EventData.NextID;
            }

            var hp_ratio = 0d;
            var en_ratio = 0d;
            if (p.Unit is object)
            {
                {
                    var withBlock1 = p.Unit;
                    hp_ratio = 100 * withBlock1.HP / (double)withBlock1.MaxHP;
                    en_ratio = 100 * withBlock1.EN / (double)withBlock1.MaxEN;
                }
            }

            p.Plana = ((int)(p.Plana + per * p.MaxPlana() / 100d));
            if (p.Unit is object)
            {
                {
                    var u = p.Unit;
                    u.HP = (int)(u.MaxHP * hp_ratio / 100d);
                    u.EN = (int)(u.MaxEN * en_ratio / 100d);
                }
            }

            return EventData.NextID;
        }
    }
}
