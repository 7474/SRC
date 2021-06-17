using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SelectCmd : CmdData
    {
        public SelectCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SelectCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            if (ArgNum != 2)
            //            {
            //                Event.EventErrorMessage = "Selectコマンドの引数の数が違います";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 446722


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            Event.SelectedUnitForEvent = GetArgAsUnit(2);
            //return EventData.NextID;
        }
    }
}
