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
            // TODO 動作確認
            var new_var1 = new VarData();
            var new_var2 = new VarData();
            VarData old_var1;
            VarData old_var2;
            if (ArgNum != 3)
            {
                throw new EventErrorException(this, "Swapコマンドの引数の数が違います");
            }
            else
            {
                // 入れ替える前の変数の値を保存
                // 引数1の変数
                old_var1 = Expression.GetVariableObject(GetArg(2));
                if (old_var1 is object)
                {
                    new_var2.SetFrom(old_var1);
                }
                // 引数2の変数
                old_var2 = Expression.GetVariableObject(GetArg(3));
                if (old_var2 is object)
                {
                    new_var1.SetFrom(old_var2);
                }

                // 引数2の変数を引数1の変数に代入
                {
                    // 引数1がサブルーチンローカル変数の場合
                    if (Event.CallDepth > 0)
                    {
                        var loopTo = Event.VarIndex;
                        for (var i = (Event.VarIndexStack[Event.CallDepth - 1] + 1); i <= loopTo; i++)
                        {
                            if ((old_var1.Name ?? "") == (Event.VarStack[i].Name ?? ""))
                            {
                                Event.VarStack[i].SetFrom(new_var1);
                                goto Swap_Var2toVar1_End;
                            }
                        }
                    }

                    // ローカル・またはグローバル変数の場合
                    old_var1.SetFrom(new_var1);
                }

            Swap_Var2toVar1_End:
                ;

                // 引数1の変数を引数2の変数に代入
                {
                    // 引数2がサブルーチンローカル変数の場合
                    if (Event.CallDepth > 0)
                    {
                        var loopTo1 = Event.VarIndex;
                        for (var i = (Event.VarIndexStack[Event.CallDepth - 1] + 1); i <= loopTo1; i++)
                        {
                            if ((old_var2.Name ?? "") == (Event.VarStack[i].Name ?? ""))
                            {
                                Event.VarStack[i].SetFrom(new_var2);
                                goto Swap_Var1toVar2_End;
                            }
                        }
                    }

                    // ローカル・またはグローバル変数の場合
                    old_var2.SetFrom(new_var2);
                }

            Swap_Var1toVar2_End:
                ;
            }

            return EventData.NextID;
        }
    }
}
