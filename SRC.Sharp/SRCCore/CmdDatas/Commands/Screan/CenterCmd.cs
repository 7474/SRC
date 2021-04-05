using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class CenterCmd : CmdData
    {
        public CenterCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CenterCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            var late_refresh = false;
            var num = ArgNum;
            if (num > 1)
            {
                if (GetArgAsString(num) == "非同期")
                {
                    late_refresh = true;
                    num = (num - 1);
                }
            }

            switch (num)
            {
                case 3:
                    var ux = GetArgAsLong(2);
                    if (ux < 1)
                    {
                        ux = 1;
                    }
                    else if (ux > Map.MapWidth)
                    {
                        ux = Map.MapWidth;
                    }

                    var uy = GetArgAsLong(3);
                    if (uy < 1)
                    {
                        uy = 1;
                    }
                    else if (uy > Map.MapHeight)
                    {
                        uy = Map.MapHeight;
                    }

                    GUI.Center(ux, uy);
                    break;

                case 2:
                    var u = GetArgAsUnit(2, true);
                    if (u is object)
                    {
                        if (u.Status == "出撃")
                        {
                            GUI.Center(u.x, u.y);
                            Event.IsUnitCenter = true;
                        }
                    }
                    break;

                default:
                    throw new EventErrorException(this, "Centerコマンドの引数の数が違います");
            }

            GUI.RedrawScreen(late_refresh);
            return EventData.ID + 1;
        }
    }
}
