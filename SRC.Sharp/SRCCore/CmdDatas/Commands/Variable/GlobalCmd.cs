using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Expressions;

namespace SRCCore.CmdDatas.Commands
{
    public class GlobalCmd   : CmdData
    {
        public GlobalCmd(SRC src, EventDataLine eventData) : base(src, CmdType.GlobalCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            //            int ExecGlobalCmdRet = default;
            //            string vname;
            //            short i;
            //            var loopTo = ArgNum;
            //            for (i = 2; i <= loopTo; i++)
            //            {
            //                vname = GetArg(i);
            //                if (Strings.InStr(vname, "\"") > 0)
            //                {
            //                    Event.EventErrorMessage = "ïœêîñºÅu" + vname + "ÅvÇ™ïsê≥Ç≈Ç∑";
            //                    ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 285764


            //                    Input:
            //                                    Error(0)

            //                     */
            //                }

            //                if (Strings.Asc(vname) == 36) // $
            //                {
            //                    vname = Strings.Mid(vname, 2);
            //                }

            //                if (!Expression.IsGlobalVariableDefined(vname))
            //                {
            //                    Expression.DefineGlobalVariable(vname);
            //                }
            //            }

            //            ExecGlobalCmdRet = LineNum + 1;
            //            return ExecGlobalCmdRet;
            return EventData.NextID;
        }
    }
}
