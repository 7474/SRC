using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class MapAbilityCmd : CmdData
    {
        public MapAbilityCmd(SRC src, EventDataLine eventData) : base(src, CmdType.MapAbilityCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
            int tx, ty;
            string aname;
            UnitAbility ua;
            switch (ArgNum)
            {
                case 5:
                    {
                        u = GetArgAsUnit(2);
                        aname = GetArgAsString(3);
                        ua = u.Abilities.FirstOrDefault(x => x.Data.Name == aname && x.IsAbilityClassifiedAs("Ｍ"));
                        if (ua == null)
                        {
                            throw new EventErrorException(this, "アビリティ名が間違っています");
                        }

                        tx = GetArgAsLong(4);
                        if (tx < 1)
                        {
                            tx = 1;
                        }
                        else if (tx > Map.MapWidth)
                        {
                            tx = Map.MapWidth;
                        }

                        ty = GetArgAsLong(5);
                        if (ty < 1)
                        {
                            ty = 1;
                        }
                        else if (ty > Map.MapHeight)
                        {
                            ty = Map.MapHeight;
                        }

                        break;
                    }

                case 4:
                    {
                        u = Event.SelectedUnitForEvent;
                        aname = GetArgAsString(2);
                        ua = u.Abilities.FirstOrDefault(x => x.Data.Name == aname && x.IsAbilityClassifiedAs("Ｍ"));

                        tx = GetArgAsLong(3);
                        if (tx < 1)
                        {
                            tx = 1;
                        }
                        else if (tx > Map.MapWidth)
                        {
                            tx = Map.MapWidth;
                        }

                        ty = GetArgAsLong(4);
                        if (ty < 1)
                        {
                            ty = 1;
                        }
                        else if (ty > Map.MapHeight)
                        {
                            ty = Map.MapHeight;
                        }

                        break;
                    }

                default:
                    throw new EventErrorException(this, "MapAbilityコマンドの引数の数が違います");
            }

            if (u.Status != "出撃")
            {
                throw new EventErrorException(this, u.Nickname + "は出撃していません");
            }

            GUI.OpenMessageForm(u1: null, u2: null);
            u.ExecuteMapAbility(ua, tx, ty, true);
            GUI.CloseMessageForm();
            GUI.RedrawScreen();
            return EventData.NextID;
        }
    }
}
