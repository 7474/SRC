using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;

namespace SRCCore.CmdDatas.Commands
{
    public class SpecialPowerCmd : CmdData
    {
        public SpecialPowerCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SpecialPowerCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u = null;
            Unit t = null;
            string sname;
            bool need_target = false;
            switch (ArgNum)
            {
                case 4:
                    {
                        u = GetArgAsUnit(2);
                        sname = GetArgAsString(3);
                        t = GetArgAsUnit(4);
                        break;
                    }

                case 3:
                    {
                        if (SRC.SPDList.IsDefined(GetArgAsString(2)))
                        {
                            {
                                var withBlock = SRC.SPDList.Item(GetArgAsString(2));
                                if (withBlock.IsEffectAvailable("みがわり") || withBlock.IsEffectAvailable("挑発"))
                                {
                                    need_target = true;
                                }
                            }
                        }

                        if (need_target)
                        {
                            u = Event.SelectedUnitForEvent;
                            sname = GetArgAsString(2);
                            t = GetArgAsUnit(3);
                        }
                        else
                        {
                            u = GetArgAsUnit(2);
                            sname = GetArgAsString(3);
                        }

                        break;
                    }

                case 2:
                    {
                        u = Event.SelectedUnitForEvent;
                        sname = GetArgAsString(2);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "SpecialPowerコマンドの引数の数が違います");
            }

            if (!SRC.SPDList.IsDefined(sname))
            {
                throw new EventErrorException(this, "SpecialPowerコマンドで指定されたスペシャルパワー「" + sname + "」が見つかりません");
            }

            var sd = SRC.SPDList.Item(sname);
            var msg_window_visible = GUI.MessageFormVisible;
            int prev_action;
            Unit prev_target;
            if (sd.Duration == "即効")
            {
                prev_target = Commands.SelectedTarget;
                if (t is object)
                {
                    Commands.SelectedTarget = t;
                }
                else
                {
                    Commands.SelectedTarget = Event.SelectedTargetForEvent;
                }

                prev_action = u.Action;
                sd.Execute(u.MainPilot(), true);
                if (prev_target is object)
                {
                    Commands.SelectedTarget = prev_target.CurrentForm();
                }

                if (prev_action == 0 && u.Action > 0 || prev_action > 0 && u.Action == 0)
                {
                    GUI.RedrawScreen();
                }
            }
            else if (t is object)
            {
                prev_action = t.Action;
                t.MakeSpecialPowerInEffect(sname, u.MainPilot().ID);
                if (prev_action == 0 && t.Action > 0 || prev_action > 0 && t.Action == 0)
                {
                    GUI.RedrawScreen();
                }
            }
            else
            {
                prev_action = u.Action;
                u.MakeSpecialPowerInEffect(sname, sdata: "");
                if (prev_action == 0 && u.Action > 0 || prev_action > 0 && u.Action == 0)
                {
                    GUI.RedrawScreen();
                }
            }

            if (!msg_window_visible)
            {
                GUI.CloseMessageForm();
            }

            return EventData.NextID;
        }
    }
}
