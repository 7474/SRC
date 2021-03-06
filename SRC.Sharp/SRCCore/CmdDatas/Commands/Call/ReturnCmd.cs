using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class ReturnCmd : CmdData
    {
        public ReturnCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ReturnCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            int ExecReturnCmdRet = default;
            if ((int)Event_Renamed.CallDepth <= 0)
            {
                Event_Renamed.EventErrorMessage = "CallコマンドとReturnコマンドが対応していません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 106629


                Input:
                            Error(0)

                 */
            }
            else if ((int)Event_Renamed.CallDepth == 1 & Event_Renamed.CallStack[(int)Event_Renamed.CallDepth] == 0)
            {
                Event_Renamed.EventErrorMessage = "CallコマンドとReturnコマンドが対応していません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 106876


                Input:
                            Error(0)

                 */
            }

            // 呼び出し階層数をデクリメント
            Event_Renamed.CallDepth = (short)(Event_Renamed.CallDepth - 1);

            // サブルーチン実行前の状態に復帰
            Event_Renamed.ArgIndex = Event_Renamed.ArgIndexStack[Event_Renamed.CallDepth];
            Event_Renamed.VarIndex = Event_Renamed.VarIndexStack[Event_Renamed.CallDepth];
            Event_Renamed.ForIndex = Event_Renamed.ForIndexStack[Event_Renamed.CallDepth];
            Event_Renamed.UpVarLevel = Event_Renamed.UpVarLevelStack[Event_Renamed.CallDepth];
            ExecReturnCmdRet = Event_Renamed.CallStack[Event_Renamed.CallDepth] + 1;
            return ExecReturnCmdRet;
        }
    }
}
