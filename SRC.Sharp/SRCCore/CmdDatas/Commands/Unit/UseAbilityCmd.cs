using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class UseAbilityCmd : CmdData
    {
        public UseAbilityCmd(SRC src, EventDataLine eventData) : base(src, CmdType.UseAbilityCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            switch (ArgNum)
            {
                case 4:
                    {
                        u1 = GetArgAsUnit(2);
                        aname = GetArgAsString(3);
                        var loopTo = u1.CountAbility();
                        for (a = 1; a <= loopTo; a++)
                        {
                            if ((aname ?? "") == (u1.Ability(a).Name ?? ""))
                            {
                                break;
                            }
                        }

                        if (a > u1.CountAbility())
                        {
                            Event.EventErrorMessage = "アビリティ名が間違っています";
                            ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 522554


                            Input:
                                                Error(0)

                             */
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
                            var loopTo1 = u1.CountAbility();
                            for (a = 1; a <= loopTo1; a++)
                            {
                                if ((aname ?? "") == (u1.Ability(a).Name ?? ""))
                                {
                                    break;
                                }
                            }

                            if (a <= u1.CountAbility())
                            {
                                u2 = GetArgAsUnit(3);
                            }
                            else
                            {
                                u1 = GetArgAsUnit(2);
                                aname = GetArgAsString(3);
                                var loopTo2 = u1.CountAbility();
                                for (a = 1; a <= loopTo2; a++)
                                {
                                    if ((aname ?? "") == (u1.Ability(a).Name ?? ""))
                                    {
                                        break;
                                    }
                                }

                                if (a > u1.CountAbility())
                                {
                                    Event.EventErrorMessage = "アビリティ名が間違っています";
                                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 523270


                                    Input:
                                                                Error(0)

                                     */
                                }

                                u2 = u1;
                            }
                        }
                        else
                        {
                            u1 = GetArgAsUnit(2);
                            aname = GetArgAsString(3);
                            var loopTo3 = u1.CountAbility();
                            for (a = 1; a <= loopTo3; a++)
                            {
                                if ((aname ?? "") == (u1.Ability(a).Name ?? ""))
                                {
                                    break;
                                }
                            }

                            if (a > u1.CountAbility())
                            {
                                Event.EventErrorMessage = "アビリティ名が間違っています";
                                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 523646


                                Input:
                                                        Error(0)

                                 */
                            }

                            u2 = u1;
                        }

                        break;
                    }

                case 2:
                    {
                        u1 = Event.SelectedUnitForEvent;
                        aname = GetArgAsString(2);
                        var loopTo4 = u1.CountAbility();
                        for (a = 1; a <= loopTo4; a++)
                        {
                            if ((aname ?? "") == (u1.Ability(a).Name ?? ""))
                            {
                                break;
                            }
                        }

                        if (a > u1.CountAbility())
                        {
                            Event.EventErrorMessage = "アビリティ名が間違っています";
                            ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 524040


                            Input:
                                                Error(0)

                             */
                        }

                        u2 = Event.SelectedUnitForEvent;
                        break;
                    }

                default:
                    {
                        Event.EventErrorMessage = "UseAbilityコマンドの引数の数が違います";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 524229


                        Input:
                                        Error(0)

                         */
                        break;
                    }
            }

            if (u1.Status != "出撃")
            {
                Event.EventErrorMessage = u1.Nickname + "は出撃していません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 524387


                Input:
                                Error(0)

                 */
            }

            u1.ExecuteAbility(a, u2, false, true);
            GUI.CloseMessageForm();
            GUI.RedrawScreen();
            return EventData.NextID;
        }
    }
}
