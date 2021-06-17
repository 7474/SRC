using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class UseAbilityCmd : CmdData
    {
        public UseAbilityCmd(SRC src, EventDataLine eventData) : base(src, CmdType.UseAbilityCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u1;
            Unit u2;
            string aname;
            UnitAbility ua;
            switch (ArgNum)
            {
                case 4:
                    {
                        u1 = GetArgAsUnit(2);
                        aname = GetArgAsString(3);
                        ua = u1.Abilities.FirstOrDefault(x => x.Data.Name == aname);
                        if (ua == null)
                        {
                            throw new EventErrorException(this, "アビリティ名が間違っています");
                        }
                        u2 = GetArgAsUnit(4);
                        break;
                    }

                case 3:
                    {
                        u1 = Event.SelectedUnitForEvent;
                        if (u1 is object)
                        {
                            aname = GetArgAsString(2);
                            ua = u1.Abilities.FirstOrDefault(x => x.Data.Name == aname);

                            if (ua == null)
                            {
                                u2 = GetArgAsUnit(3);
                            }
                            else
                            {
                                u1 = GetArgAsUnit(2);
                                aname = GetArgAsString(3);
                                ua = u1.Abilities.FirstOrDefault(x => x.Data.Name == aname);

                                if (ua == null)
                                {
                                    throw new EventErrorException(this, "アビリティ名が間違っています");
                                }
                                u2 = u1;
                            }
                        }
                        else
                        {
                            u1 = GetArgAsUnit(2);
                            aname = GetArgAsString(3);
                            ua = u1.Abilities.FirstOrDefault(x => x.Data.Name == aname);
                            if (ua == null)
                            {
                                throw new EventErrorException(this, "アビリティ名が間違っています");
                            }
                            u2 = u1;
                        }

                        break;
                    }

                case 2:
                    {
                        u1 = Event.SelectedUnitForEvent;
                        aname = GetArgAsString(2);
                        ua = u1.Abilities.FirstOrDefault(x => x.Data.Name == aname);
                        if (ua == null)
                        {
                            throw new EventErrorException(this, "アビリティ名が間違っています");
                        }
                        u2 = Event.SelectedUnitForEvent;
                        break;
                    }

                default:
                    throw new EventErrorException(this, "UseAbilityコマンドの引数の数が違います");
            }

            if (u1.Status != "出撃")
            {
                throw new EventErrorException(this, u1.Nickname + "は出撃していません");
            }

            u1.ExecuteAbility(ua, u2, false, true);
            GUI.CloseMessageForm();
            GUI.RedrawScreen();
            return EventData.NextID;
        }
    }
}
