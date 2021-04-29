using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Expressions;

namespace SRCCore.CmdDatas.Commands
{
    public class CopyArrayCmd : CmdData
    {
        public CopyArrayCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CopyArrayCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            //            int ExecCopyArrayCmdRet = default;
            //            int i;
            //            short j;
            //            string buf;
            //            VarData var;
            //            string name1, name2;
            //            if (ArgNum != 3)
            //            {
            //                Event.EventErrorMessage = "CopyArray�R�}���h�̈����̐����Ⴂ�܂�";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 203889


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            // �R�s�[���̕ϐ���
            //            name1 = GetArg(2);
            //            if (Strings.Left(name1, 1) == "$")
            //            {
            //                name1 = Strings.Mid(name1, 2);
            //            }
            //            // Eval�֐�
            //            if (Strings.LCase(Strings.Left(name1, 5)) == "eval(")
            //            {
            //                if (Strings.Right(name1, 1) == ")")
            //                {
            //                    name1 = Strings.Mid(name1, 6, Strings.Len(name1) - 6);
            //                    name1 = Expression.GetValueAsString(name1);
            //                }
            //            }

            //            // �R�s�[��̕ϐ���
            //            name2 = GetArg(3);
            //            if (Strings.Left(name2, 1) == "$")
            //            {
            //                name1 = Strings.Mid(name2, 2);
            //            }
            //            // Eval�֐�
            //            if (Strings.LCase(Strings.Left(name2, 5)) == "eval(")
            //            {
            //                if (Strings.Right(name2, 1) == ")")
            //                {
            //                    name2 = Strings.Mid(name2, 6, Strings.Len(name2) - 6);
            //                    name2 = Expression.GetValueAsString(name2);
            //                }
            //            }

            //            // �R�s�[��̕ϐ���������
            //            // �T�u���[�`�����[�J���ϐ��̏ꍇ
            //            if (Expression.IsSubLocalVariableDefined(name2))
            //            {
            //                Expression.UndefineVariable(name2);
            //                Event.VarIndex = (Event.VarIndex + 1);
            //                {
            //                    var withBlock = Event.VarStack[Event.VarIndex];
            //                    withBlock.Name = name2;
            //                    withBlock.VariableType = Expression.ValueType.StringType;
            //                    withBlock.StringValue = "";
            //                }
            //            }
            //            // ���[�J���ϐ��̏ꍇ
            //            else if (Expression.IsLocalVariableDefined(name2))
            //            {
            //                Expression.UndefineVariable(name2);
            //                Expression.DefineLocalVariable(name2);
            //            }
            //            // �O���[�o���ϐ��̏ꍇ
            //            else if (Expression.IsGlobalVariableDefined(name2))
            //            {
            //                Expression.UndefineVariable(name2);
            //                Expression.DefineGlobalVariable(name2);
            //            }

            //            // �z����������A�z��v�f��������
            //            buf = "";
            //            if (Expression.IsSubLocalVariableDefined(name1))
            //            {
            //                // �T�u���[�`�����[�J���Ȕz��ɑ΂���CopyArray
            //                var loopTo = Event.VarIndex;
            //                for (i = Event.VarIndexStack[Event.CallDepth - 1] + 1; i <= loopTo; i++)
            //                {
            //                    {
            //                        var withBlock1 = Event.VarStack[i];
            //                        if (Strings.InStr(withBlock1.Name, name1 + "[") == 1)
            //                        {
            //                            buf = name2 + Strings.Mid(withBlock1.Name, Strings.InStr(withBlock1.Name, "["));
            //                            Expression.SetVariable(buf, withBlock1.VariableType, withBlock1.StringValue, withBlock1.NumericValue);
            //                        }
            //                    }
            //                }

            //                if (string.IsNullOrEmpty(buf))
            //                {
            //                    var = Expression.GetVariableObject(name1);
            //                    {
            //                        var withBlock2 = var;
            //                        Expression.SetVariable(name2, withBlock2.VariableType, withBlock2.StringValue, withBlock2.NumericValue);
            //                    }
            //                }
            //            }
            //            else if (Expression.IsLocalVariableDefined(name1))
            //            {
            //                // ���[�J���Ȕz��ɑ΂���CopyArray
            //                foreach (VarData currentVar in Event.LocalVariableList)
            //                {
            //                    var = currentVar;
            //                    {
            //                        var withBlock3 = var;
            //                        if (Strings.InStr(withBlock3.Name, name1 + "[") == 1)
            //                        {
            //                            buf = name2 + Strings.Mid(withBlock3.Name, Strings.InStr(withBlock3.Name, "["));
            //                            Expression.SetVariable(buf, withBlock3.VariableType, withBlock3.StringValue, withBlock3.NumericValue);
            //                        }
            //                    }
            //                }

            //                if (string.IsNullOrEmpty(buf))
            //                {
            //                    var = Expression.GetVariableObject(name1);
            //                    {
            //                        var withBlock4 = var;
            //                        Expression.SetVariable(name2, withBlock4.VariableType, withBlock4.StringValue, withBlock4.NumericValue);
            //                    }
            //                }
            //            }
            //            else if (Expression.IsGlobalVariableDefined(name1))
            //            {
            //                // �O���[�o���Ȕz��ɑ΂���CopyArray
            //                foreach (VarData currentVar1 in Event.GlobalVariableList)
            //                {
            //                    var = currentVar1;
            //                    {
            //                        var withBlock5 = var;
            //                        if (Strings.InStr(withBlock5.Name, name1 + "[") == 1)
            //                        {
            //                            buf = name2 + Strings.Mid(withBlock5.Name, Strings.InStr(withBlock5.Name, "["));
            //                            Expression.SetVariable(buf, withBlock5.VariableType, withBlock5.StringValue, withBlock5.NumericValue);
            //                        }
            //                    }
            //                }

            //                if (string.IsNullOrEmpty(buf))
            //                {
            //                    var = Expression.GetVariableObject(name1);
            //                    {
            //                        var withBlock6 = var;
            //                        Expression.SetVariable(name2, withBlock6.VariableType, withBlock6.StringValue, withBlock6.NumericValue);
            //                    }
            //                }
            //            }

            //            // UPGRADE_NOTE: �I�u�W�F�N�g var ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
            //            var = null;
            //            ExecCopyArrayCmdRet = LineNum + 1;
            //            return ExecCopyArrayCmdRet;
            return EventData.NextID;
        }
    }
}
