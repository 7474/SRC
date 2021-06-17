using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ColorCmd : CmdData
    {
        public ColorCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ColorCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string opt, cname;
            //            if (ArgNum != 2)
            //            {
            //                Event.EventErrorMessage = "Colorコマンドの引数の数が違います";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 192276


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            opt = GetArgAsString(2);
            //            if (Strings.Asc(opt) != 35 || Strings.Len(opt) != 7)
            //            {
            //                Event.EventErrorMessage = "色指定が不正です";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 192515


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            cname = new string(Conversions.ToChar(Constants.vbNullChar), 8);
            //            StringType.MidStmtStr(cname, 1, 2, "&H");
            //            var midTmp = Strings.Mid(opt, 6, 2);
            //            StringType.MidStmtStr(cname, 3, 2, midTmp);
            //            var midTmp1 = Strings.Mid(opt, 4, 2);
            //            StringType.MidStmtStr(cname, 5, 2, midTmp1);
            //            var midTmp2 = Strings.Mid(opt, 2, 2);
            //            StringType.MidStmtStr(cname, 7, 2, midTmp2);
            //            if (!Information.IsNumeric(cname))
            //            {
            //                Event.EventErrorMessage = "色指定が不正です";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 193007


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            Event.ObjColor = Conversions.ToInteger(cname);
            //return EventData.NextID;
        }
    }
}
