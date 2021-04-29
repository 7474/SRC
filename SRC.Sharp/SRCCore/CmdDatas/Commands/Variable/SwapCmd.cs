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
            //                Event.EventErrorMessage = "Swap�R�}���h�̈����̐����Ⴂ�܂�";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 500350


            //                Input:
            //                            Error(0)

            //                 */
            //            }
            //            else
            //            {
            //                // ����ւ���O�̕ϐ��̒l��ۑ�
            //                // ����1�̕ϐ�
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
            //                // ����2�̕ϐ�
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

            //                // ����2�̕ϐ�������1�̕ϐ��ɑ��
            //                {
            //                    var withBlock2 = old_var1;
            //                    // ����1���T�u���[�`�����[�J���ϐ��̏ꍇ
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

            //                    // ���[�J���E�܂��̓O���[�o���ϐ��̏ꍇ
            //                    withBlock2.VariableType = new_var1.VariableType;
            //                    withBlock2.StringValue = new_var1.StringValue;
            //                    withBlock2.NumericValue = new_var1.NumericValue;
            //                }

            //                Swap_Var2toVar1_End:
            //                ;

            //                // ����1�̕ϐ�������2�̕ϐ��ɑ��
            //                {
            //                    var withBlock4 = old_var2;
            //                    // ����2���T�u���[�`�����[�J���ϐ��̏ꍇ
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

            //                    // ���[�J���E�܂��̓O���[�o���ϐ��̏ꍇ
            //                    withBlock4.VariableType = new_var2.VariableType;
            //                    withBlock4.StringValue = new_var2.StringValue;
            //                    withBlock4.NumericValue = new_var2.NumericValue;
            //                } 

            //                Swap_Var1toVar2_End:
            //                ;
            //            }

            //            // �I�u�W�F�N�g�̉��
            //            // UPGRADE_NOTE: �I�u�W�F�N�g old_var1 ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
            //            old_var1 = null;
            //            // UPGRADE_NOTE: �I�u�W�F�N�g old_var2 ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
            //            old_var2 = null;
            //            // UPGRADE_NOTE: �I�u�W�F�N�g new_var1 ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
            //            new_var1 = null;
            //            // UPGRADE_NOTE: �I�u�W�F�N�g new_var2 ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
            //            new_var2 = null;
            //            ExecSwapCmdRet = LineNum + 1;
            //            return ExecSwapCmdRet;
            return EventData.NextID;
        }
    }
}
