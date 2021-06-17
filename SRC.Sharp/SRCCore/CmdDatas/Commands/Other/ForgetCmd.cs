using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ForgetCmd : CmdData
    {
        public ForgetCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ForgetCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string tname;
            //            short i, j;
            //            if (ArgNum != 2)
            //            {
            //                Event.EventErrorMessage = "Forgetコマンドの引数の数が違います";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 282918


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            tname = GetArgAsString(2);
            //            var loopTo = Information.UBound(SRC.Titles);
            //            for (i = 1; i <= loopTo; i++)
            //            {
            //                if ((tname ?? "") == (SRC.Titles[i] ?? ""))
            //                {
            //                    break;
            //                }
            //            }

            //            if (i <= Information.UBound(SRC.Titles))
            //            {
            //                var loopTo1 = Information.UBound(SRC.Titles);
            //                for (j = (i + 1); j <= loopTo1; j++)
            //                    SRC.Titles[j - 1] = SRC.Titles[j];
            //                Array.Resize(SRC.Titles, Information.UBound(SRC.Titles));
            //            }

            //return EventData.NextID;
        }
    }
}
