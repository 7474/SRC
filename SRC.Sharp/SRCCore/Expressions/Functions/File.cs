using SRCCore.Lib;

namespace SRCCore.Expressions.Functions
{
    //�t�@�C�������֐�
    // TODO Impl ���ʂ͎������Ȃ�
    //Dir�t�@�C���̑��ݔ���
    //EOF�t�@�C���̖���������
    //LoadFileDialog�ǂݏo���p�t�@�C���̑I���_�C�A���O��\��
    //SaveFileDialog�������ݗp�t�@�C���̑I���_�C�A���O��\��

    public class Dir : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Dir
            //                        CallFunctionRet = ValueType.StringType;
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    fname = GetValueAsString(@params[1], is_term[1]);

            //                                    // �t���p�X�w��łȂ���΃V�i���I�t�H���_���N�_�Ɍ���
            //                                    if (Strings.Mid(fname, 2, 1) != ":")
            //                                    {
            //                                        fname = SRC.ScenarioPath + fname;
            //                                    }

            //                                    switch (GetValueAsString(@params[2], is_term[2]) ?? "")
            //                                    {
            //                                        case "�t�@�C��":
            //                                            {
            //                                                num = Constants.vbNormal;
            //                                                break;
            //                                            }

            //                                        case "�t�H���_":
            //                                            {
            //                                                num = FileAttribute.Directory;
            //                                                break;
            //                                            }
            //                                    }
            //                                    // UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
            //                                    str_result = FileSystem.Dir(fname, (FileAttribute)num);
            //                                    if (Strings.Len(str_result) == 0)
            //                                    {
            //                                        return CallFunctionRet;
            //                                    }

            //                                    // �t�@�C�������`�F�b�N�p�Ɍ����p�X���쐬
            //                                    dir_path = fname;
            //                                    if (num == FileAttribute.Directory)
            //                                    {
            //                                        i = GeneralLib.InStr2(fname, @"\");
            //                                        if (i > 0)
            //                                        {
            //                                            dir_path = Strings.Left(fname, i);
            //                                        }
            //                                    }

            //                                    // �P��t�@�C���̌����H
            //                                    if (Strings.InStr(fname, "*") == 0)
            //                                    {
            //                                        // �t�H���_�̌����̏ꍇ�͌��������t�@�C�����t�H���_
            //                                        // ���ǂ����`�F�b�N����
            //                                        if (num == FileAttribute.Directory)
            //                                        {
            //                                            if ((FileSystem.GetAttr(dir_path + str_result) & num) == 0)
            //                                            {
            //                                                str_result = "";
            //                                            }
            //                                        }

            //                                        return CallFunctionRet;
            //                                    }

            //                                    if (str_result == ".")
            //                                    {
            //                                        // UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
            //                                        str_result = FileSystem.Dir();
            //                                    }

            //                                    if (str_result == "..")
            //                                    {
            //                                        // UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
            //                                        str_result = FileSystem.Dir();
            //                                    }

            //                                    // �������ꂽ�t�@�C���ꗗ���쐬
            //                                    dir_list = new string[1];
            //                                    if (num == FileAttribute.Directory)
            //                                    {
            //                                        while (Strings.Len(str_result) > 0)
            //                                        {
            //                                            // �t�H���_�̌����̏ꍇ�͌��������t�@�C�����t�H���_
            //                                            // ���ǂ����`�F�b�N����
            //                                            if ((FileSystem.GetAttr(dir_path + str_result) & num) != 0)
            //                                            {
            //                                                Array.Resize(dir_list, Information.UBound(dir_list) + 1 + 1);
            //                                                dir_list[Information.UBound(dir_list)] = str_result;
            //                                            }
            //                                            // UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
            //                                            str_result = FileSystem.Dir();
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        while (Strings.Len(str_result) > 0)
            //                                        {
            //                                            Array.Resize(dir_list, Information.UBound(dir_list) + 1 + 1);
            //                                            dir_list[Information.UBound(dir_list)] = str_result;
            //                                            // UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
            //                                            str_result = FileSystem.Dir();
            //                                        }
            //                                    }

            //                                    if (Information.UBound(dir_list) > 0)
            //                                    {
            //                                        str_result = dir_list[1];
            //                                        dir_index = 2;
            //                                    }
            //                                    else
            //                                    {
            //                                        str_result = "";
            //                                        dir_index = 1;
            //                                    }

            //                                    break;
            //                                }

            //                            case 1:
            //                                {
            //                                    fname = GetValueAsString(@params[1], is_term[1]);

            //                                    // �t���p�X�w��łȂ���΃V�i���I�t�H���_���N�_�Ɍ���
            //                                    if (Strings.Mid(fname, 2, 1) != ":")
            //                                    {
            //                                        fname = SRC.ScenarioPath + fname;
            //                                    }

            //                                    // UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
            //                                    str_result = FileSystem.Dir(fname, FileAttribute.Directory);
            //                                    if (Strings.Len(str_result) == 0)
            //                                    {
            //                                        return CallFunctionRet;
            //                                    }

            //                                    // �P��t�@�C���̌����H
            //                                    if (Strings.InStr(fname, "*") == 0)
            //                                    {
            //                                        return CallFunctionRet;
            //                                    }

            //                                    if (str_result == ".")
            //                                    {
            //                                        // UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
            //                                        str_result = FileSystem.Dir();
            //                                    }

            //                                    if (str_result == "..")
            //                                    {
            //                                        // UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
            //                                        str_result = FileSystem.Dir();
            //                                    }

            //                                    // �������ꂽ�t�@�C���ꗗ���쐬
            //                                    dir_list = new string[1];
            //                                    while (Strings.Len(str_result) > 0)
            //                                    {
            //                                        Array.Resize(dir_list, Information.UBound(dir_list) + 1 + 1);
            //                                        dir_list[Information.UBound(dir_list)] = str_result;
            //                                        // UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
            //                                        str_result = FileSystem.Dir();
            //                                    }

            //                                    if (Information.UBound(dir_list) > 0)
            //                                    {
            //                                        str_result = dir_list[1];
            //                                        dir_index = 2;
            //                                    }
            //                                    else
            //                                    {
            //                                        str_result = "";
            //                                        dir_index = 1;
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    if (dir_index <= Information.UBound(dir_list))
            //                                    {
            //                                        str_result = dir_list[dir_index];
            //                                        dir_index = (dir_index + 1);
            //                                    }
            //                                    else
            //                                    {
            //                                        str_result = "";
            //                                    }

            //                                    break;
            //                                }
            //                        }

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

    public class EOF : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Eof
            //                        if (etype == ValueType.StringType)
            //                        {
            //                            if (FileSystem.EOF(GetValueAsLong(@params[1], is_term[1])))
            //                            {
            //                                str_result = "1";
            //                            }
            //                            else
            //                            {
            //                                str_result = "0";
            //                            }

            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            if (FileSystem.EOF(GetValueAsLong(@params[1], is_term[1])))
            //                            {
            //                                num_result = 1d;
            //                            }

            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

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


    public class Loadfiledialog : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Loadfiledialog
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    str_result = FileDialog.LoadFileDialog("�t�@�C�����J��", SRC.ScenarioPath, "", 2, GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]), ftype2: GetValueAsString(@params[1], is_term[1])2, fsuffix2: GetValueAsString(@params[2], is_term[2])2, ftype3: GetValueAsString(@params[1], is_term[1])3, fsuffix3: GetValueAsString(@params[2], is_term[2])3);
            //                                    break;
            //                                }

            //                            case 3:
            //                                {
            //                                    str_result = FileDialog.LoadFileDialog("�t�@�C�����J��", SRC.ScenarioPath, GetValueAsString(@params[3], is_term[3]), 2, GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]), ftype2: ""1, fsuffix2: ""1, ftype3: ""1, fsuffix3: ""1);
            //                                    break;
            //                                }

            //                            case 4:
            //                                {
            //                                    str_result = FileDialog.LoadFileDialog("�t�@�C�����J��", SRC.ScenarioPath + GetValueAsString(@params[4], is_term[4]), GetValueAsString(@params[3], is_term[3]), 2, GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]), ftype2: "", fsuffix2: "", ftype3: "", fsuffix3: "");
            //                                    break;
            //                                }
            //                        }

            //                        CallFunctionRet = ValueType.StringType;

            //                        // �{���͂��ꂾ���ł����͂������ǁc�c
            //                        if (Strings.InStr(str_result, SRC.ScenarioPath) > 0)
            //                        {
            //                            str_result = Strings.Mid(str_result, Strings.Len(SRC.ScenarioPath) + 1);
            //                            return CallFunctionRet;
            //                        }

            //                        // �t���p�X�w��Ȃ炱���ŏI��
            //                        if (Strings.Right(Strings.Left(str_result, 3), 2) == @":\")
            //                        {
            //                            str_result = "";
            //                            return CallFunctionRet;
            //                        }

            //                        // UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
            //                        while (string.IsNullOrEmpty(FileSystem.Dir(SRC.ScenarioPath + str_result, FileAttribute.Normal)))
            //                        {
            //                            if (Strings.InStr(str_result, @"\") == 0)
            //                            {
            //                                // �V�i���I�t�H���_�O�̃t�@�C��������
            //                                str_result = "";
            //                                return CallFunctionRet;
            //                            }

            //                            str_result = Strings.Mid(str_result, Strings.InStr(str_result, @"\") + 1);
            //                        }

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

    public class Savefiledialog : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Savefiledialog
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    str_result = FileDialog.SaveFileDialog("�t�@�C����ۑ�", SRC.ScenarioPath, "", 2, GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]), ftype2: "", fsuffix2: "", ftype3: "", fsuffix3: "");
            //                                    break;
            //                                }

            //                            case 3:
            //                                {
            //                                    str_result = FileDialog.SaveFileDialog("�t�@�C����ۑ�", SRC.ScenarioPath, GetValueAsString(@params[3], is_term[3]), 2, GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]), ftype2: "", fsuffix2: "", ftype3: "", fsuffix3: "");
            //                                    break;
            //                                }

            //                            case 4:
            //                                {
            //                                    str_result = FileDialog.SaveFileDialog("�t�@�C����ۑ�", SRC.ScenarioPath + GetValueAsString(@params[4], is_term[4]), GetValueAsString(@params[3], is_term[3]), 2, GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]), ftype2: "", fsuffix2: "", ftype3: "", fsuffix3: "");
            //                                    break;
            //                                }
            //                        }

            //                        CallFunctionRet = ValueType.StringType;

            //                        // �{���͂��ꂾ���ł����͂������ǁc�c
            //                        if (Strings.InStr(str_result, SRC.ScenarioPath) > 0)
            //                        {
            //                            str_result = Strings.Mid(str_result, Strings.Len(SRC.ScenarioPath) + 1);
            //                            return CallFunctionRet;
            //                        }

            //                        if (Strings.InStr(str_result, @"\") == 0)
            //                        {
            //                            return CallFunctionRet;
            //                        }

            //                        var loopTo13 = Strings.Len(str_result);
            //                        for (i = 1; i <= loopTo13; i++)
            //                        {
            //                            if (Strings.Mid(str_result, Strings.Len(str_result) - i + 1, 1) == @"\")
            //                            {
            //                                break;
            //                            }
            //                        }

            //                        buf = Strings.Left(str_result, Strings.Len(str_result) - i);
            //                        str_result = Strings.Mid(str_result, Strings.Len(str_result) - i + 2);
            //                        while (Strings.InStr(buf, @"\") > 0)
            //                        {
            //                            buf = Strings.Mid(buf, Strings.InStr(buf, @"\") + 1);
            //                            // UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
            //                            if (!string.IsNullOrEmpty(FileSystem.Dir(SRC.ScenarioPath + buf, FileAttribute.Directory)))
            //                            {
            //                                str_result = buf + @"\" + str_result;
            //                                return CallFunctionRet;
            //                            }
            //                        }

            //                        // UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
            //                        if (!string.IsNullOrEmpty(FileSystem.Dir(SRC.ScenarioPath + buf, FileAttribute.Directory)))
            //                        {
            //                            str_result = buf + @"\" + str_result;
            //                        }

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