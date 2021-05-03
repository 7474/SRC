using SRCCore.Lib;
using SRCCore.VB;

namespace SRCCore.Expressions.Functions
{
    public class Count : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            var buf = @params[1] + "[";
            var num = 0;

            // �T�u���[�`�����[�J���ϐ�������
            if (SRC.Event.CallDepth > 0)
            {
                var loopTo5 = SRC.Event.VarIndex;
                for (var i = (SRC.Event.VarIndexStack[SRC.Event.CallDepth - 1] + 1); i <= loopTo5; i++)
                {
                    if (Strings.InStr(SRC.Event.VarStack[i].Name, buf) == 1)
                    {
                        num = (num + 1);
                    }
                }

                if (num > 0)
                {
                    if (etype == ValueType.StringType)
                    {
                        str_result = GeneralLib.FormatNum(num);
                        return ValueType.StringType;
                    }
                    else
                    {
                        num_result = num;
                        return ValueType.NumericType;
                    }
                }
            }

            // ���[�J���ϐ�������
            foreach (VarData currentVar in SRC.Event.LocalVariableList.Values)
            {
                if (Strings.InStr(currentVar.Name, buf) == 1)
                {
                    num = (num + 1);
                }
            }

            if (num > 0)
            {
                if (etype == ValueType.StringType)
                {
                    str_result = GeneralLib.FormatNum(num);
                    return ValueType.StringType;
                }
                else
                {
                    num_result = num;
                    return ValueType.NumericType;
                }
            }

            // �O���[�o���ϐ�������
            foreach (VarData currentVar1 in SRC.Event.GlobalVariableList.Values)
            {
                if (Strings.InStr(currentVar1.Name, buf) == 1)
                {
                    num = (num + 1);
                }
            }

            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num);
                return ValueType.StringType;
            }
            else
            {
                num_result = num;
                return ValueType.NumericType;
            }
        }
    }

    public class Eval : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            var buf = Strings.Trim(SRC.Expression.GetValueAsString(@params[1], is_term[1]));
            return SRC.Expression.EvalExpr(buf, etype, out str_result, out num_result);
        }
    }

    public class IIf : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // XXX ���� expr �؂�o���Ă�����ǂȂ�ł���
            var list = GeneralLib.ToList(@params[1]);
            var num = list.Count;
            var flag = false;
            // XXX ����Switch�����Ӑ}�������̂Ȃ̂��������B�B�B
            switch (num)
            {
                case 1:
                    {
                        if (SRC.PList.IsDefined(list[0]))
                        {
                            {
                                var withBlock17 = SRC.PList.Item(list[0]);
                                if (withBlock17.Unit is null)
                                {
                                    flag = false;
                                }
                                else
                                {
                                    {
                                        var withBlock18 = withBlock17.Unit;
                                        if (withBlock18.Status == "�o��" || withBlock18.Status == "�i�[")
                                        {
                                            flag = true;
                                        }
                                        else
                                        {
                                            flag = false;
                                        }
                                    }
                                }
                            }
                        }
                        else if (SRC.Expression.GetValueAsLong(@params[1]) != 0)
                        {
                            flag = true;
                        }
                        else
                        {
                            flag = false;
                        }
                        break;
                    }

                case 2:
                    {
                        if (SRC.PList.IsDefined(list[1]))
                        {
                            var withBlock19 = SRC.PList.Item(list[1]);
                            if (withBlock19.Unit is null)
                            {
                                flag = true;
                            }
                            else
                            {
                                {
                                    var withBlock20 = withBlock19.Unit;
                                    if (withBlock20.Status == "�o��" || withBlock20.Status == "�i�[")
                                    {
                                        flag = false;
                                    }
                                    else
                                    {
                                        flag = true;
                                    }
                                }
                            }
                        }
                        else if (SRC.Expression.GetValueAsLong(@params[1], true) == 0)
                        {
                            flag = true;
                        }
                        else
                        {
                            flag = false;
                        }

                        break;
                    }

                default:
                    {
                        if (SRC.Expression.GetValueAsLong(@params[1]) != 0)
                        {
                            flag = true;
                        }
                        else
                        {
                            flag = false;
                        }

                        break;
                    }
            }

            if (flag)
            {
                str_result = SRC.Expression.GetValueAsString(@params[2], is_term[2]);
            }
            else
            {
                str_result = SRC.Expression.GetValueAsString(@params[3], is_term[3]);
            }

            if (etype == ValueType.NumericType)
            {
                num_result = GeneralLib.StrToDbl(str_result);
                return ValueType.NumericType;
            }
            else
            {
                return ValueType.StringType;
            }
        }
    }

    public class IsDefined : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            var pname = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
            switch (pcount)
            {
                case 2:
                    {
                        switch (SRC.Expression.GetValueAsString(@params[2], is_term[2]) ?? "")
                        {
                            case "�p�C���b�g":
                                {
                                    if (SRC.PList.IsDefined(pname))
                                    {
                                        if (SRC.PList.Item(pname).Alive)
                                        {
                                            num_result = 1d;
                                        }
                                    }

                                    break;
                                }

                            case "���j�b�g":
                                {
                                    if (SRC.UList.IsDefined(pname))
                                    {
                                        if (SRC.UList.Item(pname).Status != "�j��")
                                        {
                                            num_result = 1d;
                                        }
                                    }

                                    break;
                                }

                            case "�A�C�e��":
                                {
                                    if (SRC.IList.IsDefined(pname))
                                    {
                                        num_result = 1d;
                                    }

                                    break;
                                }
                        }

                        break;
                    }

                case 1:
                    {
                        if (SRC.PList.IsDefined(pname))
                        {
                            if (SRC.PList.Item(pname).Alive)
                            {
                                num_result = 1d;
                            }
                        }
                        else if (SRC.UList.IsDefined(pname))
                        {
                            if (SRC.UList.Item(pname).Status != "�j��")
                            {
                                num_result = 1d;
                            }
                        }
                        else if (SRC.IList.IsDefined(pname))
                        {
                            num_result = 1d;
                        }

                        break;
                    }
            }

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

    public class IsVarDefined : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // XXX ���� expr �؂�o���Ă�����ǂȂ�ł���
            if (SRC.Expression.IsVariableDefined(Strings.Trim(@params[1])))
            {
                if (etype == ValueType.StringType)
                {
                    str_result = "1";
                    return ValueType.StringType;
                }
                else
                {
                    num_result = 1d;
                    return ValueType.NumericType;
                }
            }
            else if (etype == ValueType.StringType)
            {
                str_result = "0";
                return ValueType.StringType;
            }
            else
            {
                num_result = 0d;
                return ValueType.NumericType;
            }
        }
    }

    public class KeyState : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Keystate GUI�ɉ��������邱�ƂɂȂ肻��
            //                        if (pcount != 1)
            //                        {
            //                            return CallFunctionRet;
            //                        }

            //                        // �L�[�ԍ�
            //                        i = GetValueAsLong(@params[1], is_term[1]);

            //                        // �������ݒ�ɑΉ�
            //                        switch (i)
            //                        {
            //                            case Keys.LButton:
            //                                {
            //                                    i = GUI.LButtonID;
            //                                    break;
            //                                }

            //                            case Keys.RButton:
            //                                {
            //                                    i = GUI.RButtonID;
            //                                    break;
            //                                }
            //                        }

            //                        if (i == Keys.LButton || i == Keys.RButton)
            //                        {
            //                            // �}�E�X�J�[�\���̈ʒu���Q��
            //                            GUI.GetCursorPos(PT);

            //                            // ���C���E�C���h�E��Ń}�E�X�{�^���������Ă���H
            //                            if (ReferenceEquals(Form.ActiveForm, GUI.MainForm))
            //                            {
            //                                {
            //                                    var withBlock15 = GUI.MainForm;
            //                                    x1 = (long)SrcFormatter.PixelsToTwipsX((double)withBlock15.Left) / (long)SrcFormatter.TwipsPerPixelX() + withBlock15.picMain(0).Left + 3;
            //                                    y1 = (long)SrcFormatter.PixelsToTwipsY((double)withBlock15.Top) / (long)SrcFormatter.TwipsPerPixelY() + withBlock15.picMain(0).Top + 28;
            //                                    x2 = x1 + withBlock15.picMain(0).Width;
            //                                    y2 = y1 + withBlock15.picMain(0).Height;
            //                                }

            //                                if (x1 <= PT.X & PT.X <= x2 & y1 <= PT.Y & PT.Y <= y2)
            //                                {
            //                                    in_window = true;
            //                                }
            //                            }
            //                        }
            //                        // ���C���E�B���h�E���A�N�e�B�u�ɂȂ��Ă���H
            //                        else if (ReferenceEquals(Form.ActiveForm, GUI.MainForm))
            //                        {
            //                            in_window = true;
            //                        }

            //                        // �E�B���h�E���I������Ă��Ȃ��ꍇ�͏��0��Ԃ�
            //                        if (!in_window)
            //                        {
            //                            num_result = 0d;
            //                            if (etype == ValueType.StringType)
            //                            {
            //                                str_result = "0";
            //                                CallFunctionRet = ValueType.StringType;
            //                            }
            //                            else
            //                            {
            //                                CallFunctionRet = ValueType.NumericType;
            //                            }

            //                            return CallFunctionRet;
            //                        }

            //                        // �L�[�̏�Ԃ��Q��
            //                        if (Conversions.ToBoolean(GUI.GetAsyncKeyState(i) & 0x8000))
            //                        {
            //                            num_result = 1d;
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
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

    public class Nickname : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            switch (pcount)
            {
                case 1:
                    {
                        var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);

                        if (SRC.PList.IsDefined(buf))
                        {
                            str_result = SRC.PList.Item(buf).get_Nickname(false);
                        }
                        else if (SRC.PDList.IsDefined(buf))
                        {
                            str_result = SRC.PDList.Item(buf).Nickname;
                        }
                        else if (SRC.NPDList.IsDefined(buf))
                        {
                            str_result = SRC.NPDList.Item(buf).Nickname;
                        }
                        else if (SRC.UList.IsDefined(buf))
                        {
                            str_result = SRC.UList.Item(buf).Nickname0;
                        }
                        else if (SRC.UDList.IsDefined(buf))
                        {
                            str_result = SRC.UDList.Item(buf).Nickname;
                        }
                        else if (SRC.IDList.IsDefined(buf))
                        {
                            str_result = SRC.IDList.Item(buf).Nickname;
                        }

                        break;
                    }

                case 0:
                    {
                        if (SRC.Event.SelectedUnitForEvent != null)
                        {
                            str_result = SRC.Event.SelectedUnitForEvent.Nickname0;
                        }

                        break;
                    }
            }
            return ValueType.StringType;
        }
    }

    public class Term : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            switch (pcount)
            {
                case 2:
                    {
                        var pname = SRC.Expression.GetValueAsString(@params[2], is_term[2]);
                        if (SRC.UList.IsDefined2(pname))
                        {
                            str_result = SRC.Expression.Term(SRC.Expression.GetValueAsString(@params[1], is_term[1]), SRC.UList.Item2(pname));
                        }
                        else
                        {
                            str_result = SRC.Expression.Term(SRC.Expression.GetValueAsString(@params[1], is_term[1]), u: null);
                        }
                        break;
                    }

                case 1:
                    {
                        str_result = SRC.Expression.Term(SRC.Expression.GetValueAsString(@params[1], is_term[1]), u: null);
                        break;
                    }
            }

            return ValueType.StringType;
        }
    }

    public class WindowWidth : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Windowwidth

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

    public class WindowHeight : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Windowheight

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