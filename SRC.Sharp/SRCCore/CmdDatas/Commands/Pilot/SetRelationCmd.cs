using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SetRelationCmd : CmdData
    {
        public SetRelationCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetRelationCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string pname1, pname2;
            //            string vname;
            //            short rel;
            //            switch (ArgNum)
            //            {
            //                case 3:
            //                    {
            //                        pname1 = Event.SelectedUnitForEvent.MainPilot().Name;
            //                        pname2 = GetArgAsString(2);
            //                        bool localIsDefined() { object argIndex1 = (object)pname2; var ret = SRC.PDList.IsDefined(argIndex1); return ret; }

            //                        if (!localIsDefined())
            //                        {
            //                            Event.EventErrorMessage = "キャラクター名が間違っています";
            //                            ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 454582


            //                            Input:
            //                                                Error(0)

            //                             */
            //                        }

            //                        PilotData localItem() { object argIndex1 = (object)pname2; var ret = SRC.PDList.Item(argIndex1); return ret; }

            //                        pname2 = localItem().Name;
            //                        rel = GetArgAsLong(3);
            //                        break;
            //                    }

            //                case 4:
            //                    {
            //                        pname1 = GetArgAsString(2);
            //                        bool localIsDefined1() { object argIndex1 = (object)pname1; var ret = SRC.PDList.IsDefined(argIndex1); return ret; }

            //                        if (!localIsDefined1())
            //                        {
            //                            Event.EventErrorMessage = "キャラクター名が間違っています";
            //                            ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 454886


            //                            Input:
            //                                                Error(0)

            //                             */
            //                        }

            //                        PilotData localItem1() { object argIndex1 = (object)pname1; var ret = SRC.PDList.Item(argIndex1); return ret; }

            //                        pname1 = localItem1().Name;
            //                        pname2 = GetArgAsString(3);
            //                        bool localIsDefined2() { object argIndex1 = (object)pname2; var ret = SRC.PDList.IsDefined(argIndex1); return ret; }

            //                        if (!localIsDefined2())
            //                        {
            //                            Event.EventErrorMessage = "キャラクター名が間違っています";
            //                            ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 455149


            //                            Input:
            //                                                Error(0)

            //                             */
            //                        }

            //                        PilotData localItem2() { object argIndex1 = (object)pname2; var ret = SRC.PDList.Item(argIndex1); return ret; }

            //                        pname2 = localItem2().Name;
            //                        rel = GetArgAsLong(4);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "SetRelationコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 455367


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            vname = "関係:" + pname1 + ":" + pname2;
            //            if (rel != 0)
            //            {
            //                if (!Expression.IsGlobalVariableDefined(vname))
            //                {
            //                    Expression.DefineGlobalVariable(vname);
            //                }

            //                Expression.SetVariableAsLong(vname, rel);
            //            }
            //            else if (Expression.IsGlobalVariableDefined(vname))
            //            {
            //                Expression.UndefineVariable(vname);
            //            }

            //            // 信頼度補正による気力修正を更新
            //            if (Expression.IsOptionDefined("信頼度補正"))
            //            {
            //                if (SRC.PList.IsDefined(pname1))
            //                {
            //                    Pilot localItem3() { object argIndex1 = pname1; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                    localItem3().UpdateSupportMod();
            //                }

            //                if (SRC.PList.IsDefined(pname2))
            //                {
            //                    Pilot localItem4() { object argIndex1 = pname2; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                    localItem4().UpdateSupportMod();
            //                }
            //            }
            //return EventData.NextID;
        }
    }
}
