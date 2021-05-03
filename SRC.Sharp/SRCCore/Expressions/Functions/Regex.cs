using SRCCore.Lib;

namespace SRCCore.Expressions.Functions
{
    //RegExp���K�\���ŕ����������
    //RegExpReplace���K�\���Ō��������������u��

    public class RegExp : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Regexp
            //                        ;
            //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            //                        /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo RegExp_Error' at character 111360


            //                        Input:
            //                                        On Error GoTo RegExp_Error

            //                         */
            //                        if (RegEx is null)
            //                        {
            //                            RegEx = Interaction.CreateObject("VBScript.RegExp");
            //                        }

            //                        // RegExp(������, �p�^�[��[,�召��ʂ���|�召��ʂȂ�])
            //                        buf = "";
            //                        if (pcount > 0)
            //                        {
            //                            // ������S�̂�����
            //                            // UPGRADE_WARNING: �I�u�W�F�N�g RegEx.Global �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                            RegEx.Global = (object)true;
            //                            // �啶���������̋�ʁiTrue=��ʂ��Ȃ��j
            //                            // UPGRADE_WARNING: �I�u�W�F�N�g RegEx.IgnoreCase �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                            RegEx.IgnoreCase = (object)false;
            //                            if (pcount >= 3)
            //                            {
            //                                if (GetValueAsString(@params[3], is_term[3]) == "�召��ʂȂ�")
            //                                {
            //                                    // UPGRADE_WARNING: �I�u�W�F�N�g RegEx.IgnoreCase �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                                    RegEx.IgnoreCase = (object)true;
            //                                }
            //                            }
            //                            // �����p�^�[��
            //                            // UPGRADE_WARNING: �I�u�W�F�N�g RegEx.Pattern �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                            RegEx.Pattern = GetValueAsString(@params[2], is_term[2]);
            //                            // UPGRADE_WARNING: �I�u�W�F�N�g RegEx.Execute �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                            Matches = RegEx.Execute(GetValueAsString(@params[1], is_term[1]));
            //                            // UPGRADE_WARNING: �I�u�W�F�N�g Matches.Count �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(Matches.Count, 0, false)))
            //                            {
            //                                regexp_index = -1;
            //                            }
            //                            else
            //                            {
            //                                regexp_index = 0;
            //                                // UPGRADE_WARNING: �I�u�W�F�N�g Matches() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                                buf = Conversions.ToString(Expression.Matches((object)regexp_index));
            //                            }
            //                        }
            //                        else if (regexp_index >= 0)
            //                        {
            //                            regexp_index = (regexp_index + 1);
            //                            // UPGRADE_WARNING: �I�u�W�F�N�g Matches.Count �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectLessEqual(regexp_index, Operators.SubtractObject(Matches.Count, 1), false)))
            //                            {
            //                                // UPGRADE_WARNING: �I�u�W�F�N�g Matches() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                                buf = Conversions.ToString(Expression.Matches((object)regexp_index));
            //                            }
            //                        }

            //                        str_result = buf;
            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    RegExp_Error:
            //                        ;
            //                        Event.DisplayEventErrorMessage(Event.CurrentLineNum, "VBScript���C���X�g�[������Ă��܂���");
            //                        return CallFunctionRet;

            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class RegExpReplace : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Regexpreplace
            //                // RegExpReplace(������, �����p�^�[��, �u���p�^�[��[,�召��ʂ���|�召��ʂȂ�])

            //                case "regexpreplace":
            //                    {
            //                        ;
            //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            //                        /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo RegExpReplace...' at character 114835


            //                        Input:
            //                                        'RegExpReplace(������, �����p�^�[��, �u���p�^�[��[,�召��ʂ���|�召��ʂȂ�])

            //                                        On Error GoTo RegExpReplace_Error

            //                         */
            //                        if (RegEx is null)
            //                        {
            //                            RegEx = Interaction.CreateObject("VBScript.RegExp");
            //                        }

            //                        // ������S�̂�����
            //                        // UPGRADE_WARNING: �I�u�W�F�N�g RegEx.Global �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                        RegEx.Global = (object)true;
            //                        // �啶���������̋�ʁiTrue=��ʂ��Ȃ��j
            //                        // UPGRADE_WARNING: �I�u�W�F�N�g RegEx.IgnoreCase �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                        RegEx.IgnoreCase = (object)false;
            //                        if (pcount >= 4)
            //                        {
            //                            if (GetValueAsString(@params[4], is_term[4]) == "�召��ʂȂ�")
            //                            {
            //                                // UPGRADE_WARNING: �I�u�W�F�N�g RegEx.IgnoreCase �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                                RegEx.IgnoreCase = (object)true;
            //                            }
            //                        }
            //                        // �����p�^�[��
            //                        // UPGRADE_WARNING: �I�u�W�F�N�g RegEx.Pattern �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                        RegEx.Pattern = GetValueAsString(@params[2], is_term[2]);

            //                        // �u�����s
            //                        // UPGRADE_WARNING: �I�u�W�F�N�g RegEx.Replace �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                        buf = Conversions.ToString(RegEx.Replace(GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[3], is_term[3])));
            //                        str_result = buf;
            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    RegExpReplace_Error:
            //                        ;
            //                        Event.DisplayEventErrorMessage(Event.CurrentLineNum, "VBScript���C���X�g�[������Ă��܂���");
            //                        return CallFunctionRet;

            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }
}