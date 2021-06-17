using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class MapAbilityCmd : CmdData
    {
        public MapAbilityCmd(SRC src, EventDataLine eventData) : base(src, CmdType.MapAbilityCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            Unit u;
            //            short tx, ty;
            //            short a;
            //            switch (ArgNum)
            //            {
            //                case 5:
            //                    {
            //                        u = GetArgAsUnit(2);
            //                        {
            //                            var withBlock = u;
            //                            var loopTo = withBlock.CountAbility();
            //                            for (a = 1; a <= loopTo; a++)
            //                            {
            //                                if ((GetArgAsString(3) ?? "") == (withBlock.Ability(a).Name ?? "") && withBlock.IsAbilityClassifiedAs(a, "Ｍ"))
            //                                {
            //                                    break;
            //                                }
            //                            }

            //                            if (a > withBlock.CountAbility())
            //                            {
            //                                Event.EventErrorMessage = "アビリティ名が間違っています";
            //                                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 339287


            //                                Input:
            //                                                        Error(0)

            //                                 */
            //                            }
            //                        }

            //                        tx = GetArgAsLong(4);
            //                        if (tx < 1)
            //                        {
            //                            tx = 1;
            //                        }
            //                        else if (tx > Map.MapWidth)
            //                        {
            //                            tx = Map.MapWidth;
            //                        }

            //                        ty = GetArgAsLong(5);
            //                        if (ty < 1)
            //                        {
            //                            ty = 1;
            //                        }
            //                        else if (ty > Map.MapHeight)
            //                        {
            //                            ty = Map.MapHeight;
            //                        }

            //                        break;
            //                    }

            //                case 4:
            //                    {
            //                        u = Event.SelectedUnitForEvent;
            //                        var loopTo1 = u.CountAbility();
            //                        for (a = 1; a <= loopTo1; a++)
            //                        {
            //                            if ((GetArgAsString(2) ?? "") == (u.Ability(a).Name ?? "") && u.IsAbilityClassifiedAs(a, "Ｍ"))
            //                            {
            //                                break;
            //                            }
            //                        }

            //                        if (a > u.CountAbility())
            //                        {
            //                            Event.EventErrorMessage = "アビリティ名が間違っています";
            //                            ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 340037


            //                            Input:
            //                                                    Error(0)

            //                             */
            //                        }

            //                        tx = GetArgAsLong(3);
            //                        if (tx < 1)
            //                        {
            //                            tx = 1;
            //                        }
            //                        else if (tx > Map.MapWidth)
            //                        {
            //                            tx = Map.MapWidth;
            //                        }

            //                        ty = GetArgAsLong(4);
            //                        if (ty < 1)
            //                        {
            //                            ty = 1;
            //                        }
            //                        else if (ty > Map.MapHeight)
            //                        {
            //                            ty = Map.MapHeight;
            //                        }

            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "MapAbilityコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 340520


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            if (u.Status != "出撃")
            //            {
            //                Event.EventErrorMessage = u.Nickname + "は出撃していません";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 340677


            //                Input:
            //                                Error(0)

            //                 */
            //            }

            //            GUI.OpenMessageForm(u1: null, u2: null);
            //            u.ExecuteMapAbility(a, tx, ty, true);
            //            GUI.CloseMessageForm();
            //            GUI.RedrawScreen();
            //return EventData.NextID;
        }
    }
}
