using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RenameTermCmd : CmdData
    {
        public RenameTermCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RenameTermCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string tname, vname;
            //            if (ArgNum != 3)
            //            {
            //                Event.EventErrorMessage = "RenameTermの引数の数が違います";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 432775


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            tname = GetArgAsString(2);
            //            switch (tname ?? "")
            //            {
            //                case "HP":
            //                case "EN":
            //                case "SP":
            //                case "CT":
            //                    {
            //                        vname = "ShortTerm(" + tname + ")";
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        vname = "Term(" + tname + ")";
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
