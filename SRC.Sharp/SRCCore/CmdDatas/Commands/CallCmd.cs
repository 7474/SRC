using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class CallCmd : CmdData
    {
        public CallCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CallCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            int ExecCallCmdRet = default;
            int ret;
            short i;
            var @params = new string[(Event_Renamed.MaxArgIndex + 1)];

            // サブルーチンを探す
            string arglname = GetArgAsString(2);
            ret = Event_Renamed.FindNormalLabel(ref arglname);

            // 見つかった？
            if (ret == 0)
            {
                Event_Renamed.EventErrorMessage = "サブルーチンの呼び出し先ラベルである「" + GetArgAsString((short)2) + "」がみつかりません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 104107


                Input:
                            Error(0)

                 */
            }

            // 呼び出し階層をチェック
            if (Event_Renamed.CallDepth > Event_Renamed.MaxCallDepth)
            {
                Event_Renamed.CallDepth = Event_Renamed.MaxCallDepth;
                Event_Renamed.EventErrorMessage = Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)Event_Renamed.MaxCallDepth) + "階層を越えるサブルーチンの呼び出しは出来ません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 104515


                Input:
                            Error(0)

                 */
            }

            // 引数用スタックが溢れないかチェック
            if (Event_Renamed.ArgIndex + ArgNum - 2 > (int)Event_Renamed.MaxArgIndex)
            {
                Event_Renamed.EventErrorMessage = "サブルーチンの引数の総数が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)Event_Renamed.MaxArgIndex) + "個を超えています";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 104856


                Input:
                            Error(0)

                 */
            }

            // 引数の値を先に求めておく
            // (スタックに積みながら計算すると、引数での関数呼び出しで不正になる)
            var loopTo = ArgNum;
            for (i = 3; i <= loopTo; i++)
                @params[i] = GetArgAsString(i);

            // 現在の状態を保存
            Event_Renamed.CallStack[Event_Renamed.CallDepth] = LineNum;
            Event_Renamed.ArgIndexStack[Event_Renamed.CallDepth] = Event_Renamed.ArgIndex;
            Event_Renamed.VarIndexStack[Event_Renamed.CallDepth] = Event_Renamed.VarIndex;
            Event_Renamed.ForIndexStack[Event_Renamed.CallDepth] = Event_Renamed.ForIndex;

            // UpVarが実行された場合、UpVar実行数は累計する
            if (Event_Renamed.UpVarLevel > 0)
            {
                Event_Renamed.UpVarLevelStack[Event_Renamed.CallDepth] = (short)(Event_Renamed.UpVarLevel + Event_Renamed.UpVarLevelStack[Event_Renamed.CallDepth - 1]);
            }
            else
            {
                Event_Renamed.UpVarLevelStack[Event_Renamed.CallDepth] = 0;
            }

            // UpVarの階層数を初期化
            Event_Renamed.UpVarLevel = 0;

            // 引数をスタックに積む
            var loopTo1 = ArgNum;
            for (i = 3; i <= loopTo1; i++)
                Event_Renamed.ArgStack[(short)(Event_Renamed.ArgIndex + ArgNum) - i + 1] = @params[i];
            Event_Renamed.ArgIndex = (short)(Event_Renamed.ArgIndex + ArgNum - 2);

            // 呼び出し階層数をインクリメント
            Event_Renamed.CallDepth = (short)(Event_Renamed.CallDepth + 1);
            ExecCallCmdRet = ret + 1;
            return ExecCallCmdRet;
            return EventData.ID + 1;
        }
    }
}
