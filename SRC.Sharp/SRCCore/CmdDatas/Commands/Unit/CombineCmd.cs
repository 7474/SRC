using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class CombineCmd : CmdData
    {
        public CombineCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CombineCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            Unit u;
            //            string uname;
            //            short anum;
            //            switch (ArgNum)
            //            {
            //                case 2:
            //                    {
            //                        u = Event.SelectedUnitForEvent;
            //                        uname = GetArgAsString(2);
            //                        break;
            //                    }

            //                case 3:
            //                    {
            //                        u = GetArgAsUnit(2);
            //                        uname = GetArgAsString(3);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "Combineコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 196660


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            bool localIsDefined() { object argIndex1 = uname; var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //            if (!localIsDefined())
            //            {
            //                Event.EventErrorMessage = "「" + uname + "」というユニットが見つかりません";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 196836


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            Unit localItem() { object argIndex1 = uname; var ret = SRC.UList.Item(argIndex1); return ret; }

            //            if ((u.CurrentForm().ID ?? "") != (localItem().CurrentForm().ID ?? ""))
            //            {
            //                anum = u.UsedAction;
            //                u.Combine(uname, true);
            //                if (Commands.SelectedUnit is object)
            //                {
            //                    if ((u.ID ?? "") == (Commands.SelectedUnit.ID ?? ""))
            //                    {
            //                        Commands.SelectedUnit = SRC.UList.Item(uname);
            //                    }
            //                }

            //                if (Event.SelectedUnitForEvent is object)
            //                {
            //                    if ((u.ID ?? "") == (Event.SelectedUnitForEvent.ID ?? ""))
            //                    {
            //                        Event.SelectedUnitForEvent = SRC.UList.Item(uname);
            //                    }
            //                }

            //                if (Commands.SelectedTarget is object)
            //                {
            //                    if ((u.ID ?? "") == (Commands.SelectedTarget.ID ?? ""))
            //                    {
            //                        Commands.SelectedTarget = SRC.UList.Item(uname);
            //                    }
            //                }

            //                if (Event.SelectedTargetForEvent is object)
            //                {
            //                    if ((u.ID ?? "") == (Event.SelectedTargetForEvent.ID ?? ""))
            //                    {
            //                        Event.SelectedTargetForEvent = SRC.UList.Item(uname);
            //                    }
            //                }

            //                {
            //                    var withBlock = SRC.UList.Item(uname);
            //                    withBlock.UsedAction = anum;
            //                    if (withBlock.Status == "出撃")
            //                    {
            //                        GUI.RedrawScreen();
            //                    }
            //                }
            //            }

            //return EventData.NextID;
        }
    }
}
