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
            // TODO ����m�F
            var new_var1 = new VarData();
            var new_var2 = new VarData();
            VarData old_var1;
            VarData old_var2;
            if (ArgNum != 3)
            {
                throw new EventErrorException(this, "Swap�R�}���h�̈����̐����Ⴂ�܂�");
            }
            else
            {
                // ����ւ���O�̕ϐ��̒l��ۑ�
                // ����1�̕ϐ�
                old_var1 = Expression.GetVariableObject(GetArg(2));
                if (old_var1 is object)
                {
                    {
                        var withBlock = new_var2;
                        withBlock.Name = old_var1.Name;
                        withBlock.VariableType = old_var1.VariableType;
                        withBlock.StringValue = old_var1.StringValue;
                        withBlock.NumericValue = old_var1.NumericValue;
                    }
                }
                // ����2�̕ϐ�
                old_var2 = Expression.GetVariableObject(GetArg(3));
                if (old_var2 is object)
                {
                    {
                        var withBlock1 = new_var1;
                        withBlock1.Name = old_var2.Name;
                        withBlock1.VariableType = old_var2.VariableType;
                        withBlock1.StringValue = old_var2.StringValue;
                        withBlock1.NumericValue = old_var2.NumericValue;
                    }
                }

                // ����2�̕ϐ�������1�̕ϐ��ɑ��
                {
                    var withBlock2 = old_var1;
                    // ����1���T�u���[�`�����[�J���ϐ��̏ꍇ
                    if (Event.CallDepth > 0)
                    {
                        var loopTo = Event.VarIndex;
                        for (var i = (Event.VarIndexStack[Event.CallDepth - 1] + 1); i <= loopTo; i++)
                        {
                            if ((withBlock2.Name ?? "") == (Event.VarStack[i].Name ?? ""))
                            {
                                {
                                    var withBlock3 = Event.VarStack[i];
                                    withBlock3.VariableType = new_var1.VariableType;
                                    withBlock3.StringValue = new_var1.StringValue;
                                    withBlock3.NumericValue = new_var1.NumericValue;
                                }

                                goto Swap_Var2toVar1_End;
                            }
                        }
                    }

                    // ���[�J���E�܂��̓O���[�o���ϐ��̏ꍇ
                    withBlock2.VariableType = new_var1.VariableType;
                    withBlock2.StringValue = new_var1.StringValue;
                    withBlock2.NumericValue = new_var1.NumericValue;
                }

            Swap_Var2toVar1_End:
                ;

                // ����1�̕ϐ�������2�̕ϐ��ɑ��
                {
                    var withBlock4 = old_var2;
                    // ����2���T�u���[�`�����[�J���ϐ��̏ꍇ
                    if (Event.CallDepth > 0)
                    {
                        var loopTo1 = Event.VarIndex;
                        for (var i = (Event.VarIndexStack[Event.CallDepth - 1] + 1); i <= loopTo1; i++)
                        {
                            if ((withBlock4.Name ?? "") == (Event.VarStack[i].Name ?? ""))
                            {
                                {
                                    var withBlock5 = Event.VarStack[i];
                                    withBlock5.VariableType = new_var2.VariableType;
                                    withBlock5.StringValue = new_var2.StringValue;
                                    withBlock5.NumericValue = new_var2.NumericValue;
                                }

                                goto Swap_Var1toVar2_End;
                            }
                        }
                    }

                    // ���[�J���E�܂��̓O���[�o���ϐ��̏ꍇ
                    withBlock4.VariableType = new_var2.VariableType;
                    withBlock4.StringValue = new_var2.StringValue;
                    withBlock4.NumericValue = new_var2.NumericValue;
                }

            Swap_Var1toVar2_End:
                ;
            }

            return EventData.NextID;
        }
    }
}
