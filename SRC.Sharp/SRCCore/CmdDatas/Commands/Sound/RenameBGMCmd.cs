using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RenameBGMCmd : CmdData
    {
        public RenameBGMCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RenameBGMCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string bname, vname;
            //            if (ArgNum != 3)
            //            {
            //                Event.EventErrorMessage = "RenameBGMの引数の数が違います";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 430445


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            bname = GetArgAsString(2);
            //            switch (bname ?? "")
            //            {
            //                case "Map1":
            //                case "Map2":
            //                case "Map3":
            //                case "Map4":
            //                case "Map5":
            //                case "Map6":
            //                case "Briefing":
            //                case "Intermission":
            //                case "Subtitle":
            //                case "End":
            //                case "default":
            //                    {
            //                        vname = "BGM(" + bname + ")";
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "BGM名が不正です";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 430755


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            if (!Expression.IsGlobalVariableDefined(vname))
            //            {
            //                Expression.DefineGlobalVariable(vname);
            //            }

            //            Expression.SetVariableAsString(vname, GetArgAsString(3));
            //return EventData.NextID;
        }
    }
}
