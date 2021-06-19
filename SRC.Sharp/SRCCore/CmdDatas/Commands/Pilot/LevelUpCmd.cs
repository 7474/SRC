using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.Pilots;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class LevelUpCmd : CmdData
    {
        public LevelUpCmd(SRC src, EventDataLine eventData) : base(src, CmdType.LevelUpCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Pilot p = null;
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
                        var u = Event.SelectedUnitForEvent;
                        if (u.CountPilot() > 0)
                        {
                            p = u.Pilots.First();
                        }

                        num = GetArgAsLong(2);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "LevelUpコマンドの引数の数が違います");
            }

            if (p != null)
            {
                var hp_ratio = 0d;
                var en_ratio = 0d;
                if (p.Unit is object)
                {
                    {
                        var u = p.Unit;
                        hp_ratio = 100 * u.HP / (double)u.MaxHP;
                        en_ratio = 100 * u.EN / (double)u.MaxEN;
                    }
                }

                if (Expression.IsOptionDefined("レベル限界突破"))
                {
                    p.Level = GeneralLib.MinLng(GeneralLib.MaxLng(p.Level + num, 1), 999);
                }
                else
                {
                    p.Level = GeneralLib.MinLng(GeneralLib.MaxLng(p.Level + num, 1), 99);
                }

                // 闘争本能入手？
                if (p.IsSkillAvailable("闘争本能"))
                {
                    if (p.MinMorale > 100)
                    {
                        if (p.Morale == p.MinMorale)
                        {
                            p.Morale = ((int)(p.MinMorale + 5d * p.SkillLevel("闘争本能", ref_mode: "")));
                        }
                    }
                    else if (p.Morale == 100)
                    {
                        p.Morale = ((int)(100d + 5d * p.SkillLevel("闘争本能", ref_mode: "")));
                    }
                }

                // ＳＰ＆霊力をアップデート
                p.SP = p.SP;
                p.Plana = p.Plana;
                if (p.Unit is object)
                {
                    {
                        var u = p.Unit;
                        u.Update();
                        u.HP = (int)(u.MaxHP * hp_ratio / 100d);
                        u.EN = (int)(u.MaxEN * en_ratio / 100d);
                    }

                    SRC.PList.UpdateSupportMod(p.Unit);
                }
            }

            return EventData.NextID;
        }
    }
}
