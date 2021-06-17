using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SpecialPowerCmd : CmdData
    {
        public SpecialPowerCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SpecialPowerCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            Unit u, t = default;
            //            string sname;
            //            SpecialPowerData sd;
            //            bool need_target = default, msg_window_visible;
            //            short prev_action;
            //            switch (ArgNum)
            //            {
            //                case 4:
            //                    {
            //                        u = GetArgAsUnit(2);
            //                        sname = GetArgAsString(3);
            //                        t = GetArgAsUnit(4);
            //                        break;
            //                    }

            //                case 3:
            //                    {
            //                        if (SRC.SPDList.IsDefined((object)GetArgAsString(2)))
            //                        {
            //                            {
            //                                var withBlock = SRC.SPDList.Item((object)GetArgAsString(2));
            //                                if (Conversions.ToBoolean(Operators.OrObject(withBlock.IsEffectAvailable("みがわり"), withBlock.IsEffectAvailable("挑発"))))
            //                                {
            //                                    need_target = true;
            //                                }
            //                            }
            //                        }

            //                        if (need_target)
            //                        {
            //                            u = Event.SelectedUnitForEvent;
            //                            sname = GetArgAsString(2);
            //                            t = GetArgAsUnit(3);
            //                        }
            //                        else
            //                        {
            //                            u = GetArgAsUnit(2);
            //                            sname = GetArgAsString(3);
            //                        }

            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        u = Event.SelectedUnitForEvent;
            //                        sname = GetArgAsString(2);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "SpecialPowerコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 492305


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            bool localIsDefined() { object argIndex1 = sname; var ret = SRC.SPDList.IsDefined(argIndex1); return ret; }

            //            if (!localIsDefined())
            //            {
            //                Event.EventErrorMessage = "SpecialPowerコマンドで指定されたスペシャルパワー「" + sname + "」が見つかりません";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 492506


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            sd = SRC.SPDList.Item(sname);
            //            msg_window_visible = My.MyProject.Forms.frmMessage.Visible;
            //            Unit prev_target;
            //            if (sd.Duration == "即効")
            //            {
            //                prev_target = Commands.SelectedTarget;
            //                if (t is object)
            //                {
            //                    Commands.SelectedTarget = t;
            //                }
            //                else
            //                {
            //                    Commands.SelectedTarget = Event.SelectedTargetForEvent;
            //                }

            //                prev_action = u.Action;
            //                sd.Execute(u.MainPilot(), true);
            //                if (prev_target is object)
            //                {
            //                    Commands.SelectedTarget = prev_target.CurrentForm();
            //                }

            //                if (prev_action == 0 && u.Action > 0 || prev_action > 0 && u.Action == 0)
            //                {
            //                    GUI.RedrawScreen();
            //                }
            //            }
            //            else if (t is object)
            //            {
            //                prev_action = t.Action;
            //                t.MakeSpecialPowerInEffect(sname, u.MainPilot().ID);
            //                if (prev_action == 0 && t.Action > 0 || prev_action > 0 && t.Action == 0)
            //                {
            //                    GUI.RedrawScreen();
            //                }
            //            }
            //            else
            //            {
            //                prev_action = u.Action;
            //                u.MakeSpecialPowerInEffect(sname, sdata: "");
            //                if (prev_action == 0 && u.Action > 0 || prev_action > 0 && u.Action == 0)
            //                {
            //                    GUI.RedrawScreen();
            //                }
            //            }

            //            if (!msg_window_visible)
            //            {
            //                GUI.CloseMessageForm();
            //            }

            //return EventData.NextID;
        }
    }
}
