using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Expressions;

namespace SRCCore.CmdDatas.Commands
{
    public class SwapCmd : CmdData
    {
        public SwapCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SwapCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            //            int ExecSwapCmdRet = default;
            //            var new_var1 = new VarData();
            //            var new_var2 = new VarData();
            //            VarData old_var1;
            //            VarData old_var2;
            //            short i;
            //            if (ArgNum != 3)
            //            {
            //                Event.EventErrorMessage = "Swapコマンドの引数の数が違います";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 500350


            //                Input:
            //                            Error(0)

            //                 */
            //            }
            //            else
            //            {
            //                // 入れ替える前の変数の値を保存
            //                // 引数1の変数
            //                old_var1 = Expression.GetVariableObject(GetArg(2));
            //                if (old_var1 is object)
            //                {
            //                    {
            //                        var withBlock = new_var2;
            //                        withBlock.Name = old_var1.Name;
            //                        withBlock.VariableType = old_var1.VariableType;
            //                        withBlock.StringValue = old_var1.StringValue;
            //                        withBlock.NumericValue = old_var1.NumericValue;
            //                    }
            //                }
            //                // 引数2の変数
            //                old_var2 = Expression.GetVariableObject(GetArg(3));
            //                if (old_var2 is object)
            //                {
            //                    {
            //                        var withBlock1 = new_var1;
            //                        withBlock1.Name = old_var2.Name;
            //                        withBlock1.VariableType = old_var2.VariableType;
            //                        withBlock1.StringValue = old_var2.StringValue;
            //                        withBlock1.NumericValue = old_var2.NumericValue;
            //                    }
            //                }

            //                // 引数2の変数を引数1の変数に代入
            //                {
            //                    var withBlock2 = old_var1;
            //                    // 引数1がサブルーチンローカル変数の場合
            //                    if (Event.CallDepth > 0)
            //                    {
            //                        var loopTo = Event.VarIndex;
            //                        for (i = (Event.VarIndexStack[Event.CallDepth - 1] + 1); i <= loopTo; i++)
            //                        {
            //                            if ((withBlock2.Name ?? "") == (Event.VarStack[i].Name ?? ""))
            //                            {
            //                                {
            //                                    var withBlock3 = Event.VarStack[i];
            //                                    withBlock3.VariableType = new_var1.VariableType;
            //                                    withBlock3.StringValue = new_var1.StringValue;
            //                                    withBlock3.NumericValue = new_var1.NumericValue;
            //                                }

            //                                goto Swap_Var2toVar1_End;
            //                            }
            //                        }
            //                    }

            //                    // ローカル・またはグローバル変数の場合
            //                    withBlock2.VariableType = new_var1.VariableType;
            //                    withBlock2.StringValue = new_var1.StringValue;
            //                    withBlock2.NumericValue = new_var1.NumericValue;
            //                }

            //                Swap_Var2toVar1_End:
            //                ;

            //                // 引数1の変数を引数2の変数に代入
            //                {
            //                    var withBlock4 = old_var2;
            //                    // 引数2がサブルーチンローカル変数の場合
            //                    if (Event.CallDepth > 0)
            //                    {
            //                        var loopTo1 = Event.VarIndex;
            //                        for (i = (Event.VarIndexStack[Event.CallDepth - 1] + 1); i <= loopTo1; i++)
            //                        {
            //                            if ((withBlock4.Name ?? "") == (Event.VarStack[i].Name ?? ""))
            //                            {
            //                                {
            //                                    var withBlock5 = Event.VarStack[i];
            //                                    withBlock5.VariableType = new_var2.VariableType;
            //                                    withBlock5.StringValue = new_var2.StringValue;
            //                                    withBlock5.NumericValue = new_var2.NumericValue;
            //                                }

            //                                goto Swap_Var1toVar2_End;
            //                            }
            //                        }
            //                    }

            //                    // ローカル・またはグローバル変数の場合
            //                    withBlock4.VariableType = new_var2.VariableType;
            //                    withBlock4.StringValue = new_var2.StringValue;
            //                    withBlock4.NumericValue = new_var2.NumericValue;
            //                } 

            //                Swap_Var1toVar2_End:
            //                ;
            //            }

            //            // オブジェクトの解放
            //            // UPGRADE_NOTE: オブジェクト old_var1 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //            old_var1 = null;
            //            // UPGRADE_NOTE: オブジェクト old_var2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //            old_var2 = null;
            //            // UPGRADE_NOTE: オブジェクト new_var1 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //            new_var1 = null;
            //            // UPGRADE_NOTE: オブジェクト new_var2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //            new_var2 = null;
            //            ExecSwapCmdRet = LineNum + 1;
            //            return ExecSwapCmdRet;
            return EventData.NextID;
        }
    }
}
