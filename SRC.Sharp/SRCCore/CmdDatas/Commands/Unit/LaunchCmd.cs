using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class LaunchCmd : CmdData
    {
        public LaunchCmd(SRC src, EventDataLine eventData) : base(src, CmdType.LaunchCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
            int uy, ux;
            string opt;
            int num = ArgNum;
            switch (GetArgAsString(num) ?? "")
            {
                case "非同期":
                        opt = "非同期";
                        num = (num - 1);
                        break;

                case "アニメ非表示":
                        opt = "";
                        num = (num - 1);
                        break;

                default:
                        opt = "出撃";
                        break;
            }

            switch (num)
            {
                case 3:
                    {
                        u = Event.SelectedUnitForEvent;
                        ux = GetArgAsLong(2);
                        if (ux < 1)
                        {
                            ux = 1;
                        }
                        else if (ux > Map.MapWidth)
                        {
                            ux = Map.MapWidth;
                        }

                        uy = GetArgAsLong(3);
                        if (uy < 1)
                        {
                            uy = 1;
                        }
                        else if (uy > Map.MapHeight)
                        {
                            uy = Map.MapHeight;
                        }

                        break;
                    }

                case 4:
                    {
                        u = GetArgAsUnit(2);
                        ux = GetArgAsLong(3);
                        if (ux < 1)
                        {
                            ux = 1;
                        }
                        else if (ux > Map.MapWidth)
                        {
                            ux = Map.MapWidth;
                        }

                        uy = GetArgAsLong(4);
                        if (uy < 1)
                        {
                            uy = 1;
                        }
                        else if (uy > Map.MapHeight)
                        {
                            uy = Map.MapHeight;
                        }

                        break;
                    }

                default:
                    throw new EventErrorException(this, "Launchコマンドの引数の数が違います");
            }

            if (opt != "非同期" & GUI.MainFormVisible & !GUI.IsPictureVisible)
            {
                GUI.Center(ux, uy);
                GUI.RefreshScreen();
            }

            switch (u.Status ?? "")
            {
                case "出撃":
                    throw new EventErrorException(this, u.MainPilot().get_Nickname(false) + "はすでに出撃しています");

                case "離脱":
                    throw new EventErrorException(this, u.MainPilot().get_Nickname(false) + "はまだ離脱しています");
            }

            u.UsedAction = 0;
            u.UsedSupportAttack = 0;
            u.UsedSupportGuard = 0;
            u.UsedSyncAttack = 0;
            u.UsedCounterAttack = 0;
            if (u.HP <= 0)
            {
                u.HP = 1;
            }

            u.StandBy(ux, uy, opt);
            u.CheckAutoHyperMode();
            Event.SelectedUnitForEvent = u.CurrentForm();

            return EventData.NextID;
        }
    }
}
