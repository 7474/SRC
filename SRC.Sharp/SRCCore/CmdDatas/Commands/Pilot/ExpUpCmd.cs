using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Pilots;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class ExpUpCmd : CmdData
    {
        public ExpUpCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ExpUpCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Pilot p;
            int num;
            switch (ArgNum)
            {
                case 3:
                    {
                        p = GetArgAsPilot(2);
                        num = GetArgAsLong(3);
                        break;
                    }

                case 2:
                    {
                        {
                            var u = Event.SelectedUnitForEvent;
                            if (u.CountPilot() > 0)
                            {
                                p = u.Pilots.First();
                            }
                            else
                            {
                                return EventData.NextID;
                            }
                        }

                        num = GetArgAsLong(2);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "ExpUpコマンドの引数の数が違います");
            }

            var hp_ratio = 0d;
            var en_ratio = 0d;
            if (p.Unit is object)
            {
                var u = p.Unit;
                hp_ratio = 100 * u.HP / (double)u.MaxHP;
                en_ratio = 100 * u.EN / (double)u.MaxEN;
            }

            var prev_lv = p.Level;
            p.Exp = p.Exp + num;
            if (p.Level == prev_lv)
            {
                return EventData.NextID;
            }

            p.Update();

            // ＳＰ＆霊力をアップデート
            p.SP = p.SP;
            p.Plana = p.Plana;
            if (p.Unit is object)
            {
                {
                    var u = p.Unit;
                    u.Update();
                    u.HP = ((int)(u.MaxHP * hp_ratio / 100d));
                    u.EN = ((int)(u.MaxEN * en_ratio / 100d));
                }

                SRC.PList.UpdateSupportMod(p.Unit);
            }

            return EventData.NextID;
        }
    }
}
