using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;

namespace SRCCore.CmdDatas.Commands
{
    public class CombineCmd : CmdData
    {
        public CombineCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CombineCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
            string uname;
            switch (ArgNum)
            {
                case 2:
                    {
                        u = Event.SelectedUnitForEvent;
                        uname = GetArgAsString(2);
                        break;
                    }

                case 3:
                    {
                        u = GetArgAsUnit(2);
                        uname = GetArgAsString(3);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "Combineコマンドの引数の数が違います");
            }

            if (!SRC.UList.IsDefined(uname))
            {
                throw new EventErrorException(this, "「" + uname + "」というユニットが見つかりません");
            }

            if ((u.CurrentForm().ID ?? "") != (SRC.UList.Item(uname).CurrentForm().ID ?? ""))
            {
                var anum = u.UsedAction;
                u.Combine(uname, true);
                if (Commands.SelectedUnit is object)
                {
                    if ((u.ID ?? "") == (Commands.SelectedUnit.ID ?? ""))
                    {
                        Commands.SelectedUnit = SRC.UList.Item(uname);
                    }
                }

                if (Event.SelectedUnitForEvent is object)
                {
                    if ((u.ID ?? "") == (Event.SelectedUnitForEvent.ID ?? ""))
                    {
                        Event.SelectedUnitForEvent = SRC.UList.Item(uname);
                    }
                }

                if (Commands.SelectedTarget is object)
                {
                    if ((u.ID ?? "") == (Commands.SelectedTarget.ID ?? ""))
                    {
                        Commands.SelectedTarget = SRC.UList.Item(uname);
                    }
                }

                if (Event.SelectedTargetForEvent is object)
                {
                    if ((u.ID ?? "") == (Event.SelectedTargetForEvent.ID ?? ""))
                    {
                        Event.SelectedTargetForEvent = SRC.UList.Item(uname);
                    }
                }

                {
                    var withBlock = SRC.UList.Item(uname);
                    withBlock.UsedAction = anum;
                    if (withBlock.Status == "出撃")
                    {
                        GUI.RedrawScreen();
                    }
                }
            }

            return EventData.NextID;
        }
    }
}
