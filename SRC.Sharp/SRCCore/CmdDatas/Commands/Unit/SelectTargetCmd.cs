using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SelectTargetCmd : CmdData
    {
        public SelectTargetCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SelectTargetCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string pname;
            //            if (ArgNum != 2)
            //            {
            //                Event.EventErrorMessage = "SelectTargetコマンドの引数の数が違います";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 447068


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            Event.SelectedTargetForEvent = GetArgAsUnit(2);
            //return EventData.NextID;
        }
    }
}
