using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Expressions;
using SRCCore.VB;
using System;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Commands
{
    public class SortCmd : CmdData
    {
        public SortCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SortCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();

            // TODO �z�񑀍�̃��[�e�B���e�B�i������Ƃ��邭�Ȃ����j
            //object ExecSortCmdRet = default;
            //short j, i, k;
            //bool isStringkey, isStringValue;
            //bool isSwap, isAscOrder, isKeySort;
            //string vname, buf;
            //object value_buf;
            //short num;
            //VarData var;
            //var array_buf = default(object[]);
            //var var_buf = new object[3];

            // array_buf(opt, value)
            // opt=0�c�z��̓Y��
            // =1�c�ϐ���ValueTyep
            // =2�c�ϐ��̒l

            //if (ArgNum < 2)
            //{
            //    throw new EventErrorException(this, "Sort�R�}���h�̈����̐����Ⴂ�܂�");
            //}

            //// �����l
            //var isAscOrder = true; // �\�[�g�����������ݒ�
            //var isStringkey = false; // �z��̃C���f�b�N�X�𐔒l�Ƃ��Ĉ���
            //var isStringValue = false; // �z��̗v�f�𐔒l�Ƃ��Ĉ���
            //var isKeySort = false; // �C���f�b�N�X�݂̂̃\�[�g�ł͂Ȃ�
            //var loopTo = ArgNum;
            //for (var i = 3; i <= loopTo; i++)
            //{
            //    var buf = GetArgAsString(i);
            //    switch (buf ?? "")
            //    {
            //        case "����":
            //            {
            //                isAscOrder = true;
            //                break;
            //            }

            //        case "�~��":
            //            {
            //                isAscOrder = false;
            //                break;
            //            }

            //        case "���l":
            //            {
            //                isStringValue = false;
            //                break;
            //            }

            //        case "����":
            //            {
            //                isStringValue = true;
            //                break;
            //            }

            //        case "�C���f�b�N�X�̂�":
            //            {
            //                isKeySort = true;
            //                break;
            //            }

            //        case "�����C���f�b�N�X":
            //            {
            //                isStringkey = true;
            //                break;
            //            }

            //        default:
            //            throw new EventErrorException(this, "Sort�R�}���h�ɕs���ȃI�v�V�����u" + buf + "�v���g���Ă��܂�");
            //    }
            //}

            //// �\�[�g����z��ϐ���
            //var vname = GetArg(2);
            //if (Strings.Left(vname, 1) == "$")
            //{
            //    vname = Strings.Mid(vname, 2);
            //}
            //// Eval�֐�
            //if (Strings.LCase(Strings.Left(vname, 5)) == "eval(")
            //{
            //    if (Strings.Right(vname, 1) == ")")
            //    {
            //        vname = Strings.Mid(vname, 6, Strings.Len(vname) - 6);
            //        vname = Expression.GetValueAsString(vname);
            //    }
            //}

            //// �z����������A�z��v�f��������
            //var num = 0;
            //IList<VarData> array_buf = new List<VarData>();
            //if (Expression.IsSubLocalVariableDefined(vname))
            //{
            //    // �T�u���[�`�����[�J���Ȕz��
            //    // TODO �����Ɍ��炸 VarIndexStack �Q�ƌ�����
            //    var loopTo1 = Event.VarIndex;
            //    for (var i = (Event.VarIndexStack[Event.CallDepth - 1] + 1); i <= loopTo1; i++)
            //    {
            //        {
            //            var withBlock = Event.VarStack[i];
            //            if (Strings.InStr(withBlock.Name, vname + "[") == 1)
            //            {
            //                var oldArray_buf = array_buf;
            //                array_buf = new object[3, (num + 1)];
            //                if (oldArray_buf is object)
            //                    for (var i1 = 0; i1 <= oldArray_buf.Length / oldArray_buf.GetLength(1) - 1; ++i1)
            //                        Array.Copy(oldArray_buf, i1 * oldArray_buf.GetLength(1), array_buf, i1 * array_buf.GetLength(1), Math.Min(oldArray_buf.GetLength(1), array_buf.GetLength(1)));
            //                buf = Strings.Mid(withBlock.Name, Strings.InStr(withBlock.Name, "[") + 1, GeneralLib.InStr2(withBlock.Name, "]") - Strings.InStr(withBlock.Name, "[") - 1);
            //                if (!Information.IsNumeric(buf))
            //                {
            //                    isStringkey = true;
            //                }

            //                if (withBlock.VariableType == Expressions.ValueType.StringType)
            //                {
            //                    value_buf = withBlock.StringValue;
            //                }
            //                else
            //                {
            //                    value_buf = withBlock.NumericValue;
            //                }

            //                if (!Information.IsNumeric(value_buf))
            //                {
            //                    isStringValue = true;
            //                }

            //                array_buf[0, num] = buf;
            //                array_buf[1, num] = withBlock.VariableType;
            //                array_buf[2, num] = value_buf;
            //                num = (num + 1);
            //            }
            //        }
            //    }

            //    if (num == 0)
            //    {
            //        return EventData.NextID;
            //    }
            //}
            //else if (Expression.IsLocalVariableDefined(vname))
            //{
            //    // ���[�J���Ȕz��
            //    foreach (VarData currentVar in Event.LocalVariableList)
            //    {
            //        var = currentVar;
            //        if (Strings.InStr(var.Name, vname + "[") == 1)
            //        {
            //            var oldArray_buf1 = array_buf;
            //            array_buf = new object[3, (num + 1)];
            //            if (oldArray_buf1 is object)
            //                for (var i2 = 0; i2 <= oldArray_buf1.Length / oldArray_buf1.GetLength(1) - 1; ++i2)
            //                    Array.Copy(oldArray_buf1, i2 * oldArray_buf1.GetLength(1), array_buf, i2 * array_buf.GetLength(1), Math.Min(oldArray_buf1.GetLength(1), array_buf.GetLength(1)));
            //            buf = Strings.Mid(var.Name, Strings.InStr(var.Name, "[") + 1, GeneralLib.InStr2(var.Name, "]") - Strings.InStr(var.Name, "[") - 1);
            //            if (!Information.IsNumeric(buf))
            //            {
            //                isStringkey = true;
            //            }

            //            if (var.VariableType == Expression.ValueType.StringType)
            //            {
            //                // UPGRADE_WARNING: �I�u�W�F�N�g value_buf �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                value_buf = var.StringValue;
            //            }
            //            else
            //            {
            //                // UPGRADE_WARNING: �I�u�W�F�N�g value_buf �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                value_buf = var.NumericValue;
            //            }

            //            if (!Information.IsNumeric(value_buf))
            //            {
            //                isStringValue = true;
            //            }

            //            // UPGRADE_WARNING: �I�u�W�F�N�g array_buf(0, num) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //            array_buf[0, num] = buf;
            //            // UPGRADE_WARNING: �I�u�W�F�N�g array_buf(1, num) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //            array_buf[1, num] = var.VariableType;
            //            // UPGRADE_WARNING: �I�u�W�F�N�g value_buf �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //            // UPGRADE_WARNING: �I�u�W�F�N�g array_buf(2, num) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //            array_buf[2, num] = value_buf;
            //            num = (num + 1);
            //        }
            //    }

            //    if (num == 0)
            //    {
            //        return EventData.NextID;
            //    }
            //}
            //else if (Expression.IsGlobalVariableDefined(vname))
            //{
            //    // �O���[�o���Ȕz��
            //    foreach (VarData currentVar1 in Event.GlobalVariableList)
            //    {
            //        var = currentVar1;
            //        if (Strings.InStr(var.Name, vname + "[") == 1)
            //        {
            //            var oldArray_buf2 = array_buf;
            //            array_buf = new object[3, (num + 1)];
            //            if (oldArray_buf2 is object)
            //                for (var i3 = 0; i3 <= oldArray_buf2.Length / oldArray_buf2.GetLength(1) - 1; ++i3)
            //                    Array.Copy(oldArray_buf2, i3 * oldArray_buf2.GetLength(1), array_buf, i3 * array_buf.GetLength(1), Math.Min(oldArray_buf2.GetLength(1), array_buf.GetLength(1)));
            //            buf = Strings.Mid(var.Name, Strings.InStr(var.Name, "[") + 1, GeneralLib.InStr2(var.Name, "]") - Strings.InStr(var.Name, "[") - 1);
            //            if (!Information.IsNumeric(buf))
            //            {
            //                isStringkey = true;
            //            }

            //            if (var.VariableType == Expression.ValueType.StringType)
            //            {
            //                // UPGRADE_WARNING: �I�u�W�F�N�g value_buf �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                value_buf = var.StringValue;
            //            }
            //            else
            //            {
            //                // UPGRADE_WARNING: �I�u�W�F�N�g value_buf �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                value_buf = var.NumericValue;
            //            }

            //            if (!Information.IsNumeric(value_buf))
            //            {
            //                isStringValue = true;
            //            }

            //            // UPGRADE_WARNING: �I�u�W�F�N�g array_buf(0, num) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //            array_buf[0, num] = buf;
            //            // UPGRADE_WARNING: �I�u�W�F�N�g array_buf(1, num) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //            array_buf[1, num] = var.VariableType;
            //            // UPGRADE_WARNING: �I�u�W�F�N�g value_buf �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //            // UPGRADE_WARNING: �I�u�W�F�N�g array_buf(2, num) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //            array_buf[2, num] = value_buf;
            //            num = (num + 1);
            //        }
            //    }

            //    if (num == 0)
            //    {
            //        return EventData.NextID;
            //    }
            //}

            //num = (num - 1);
            //if (!isStringkey || isKeySort)
            //{
            //    // �Y�������l�̏ꍇ�A�܂��̓C���f�b�N�X�݂̂̃\�[�g�̏ꍇ�A
            //    // ��ɓY���̏����ɕ��ёւ���
            //    var loopTo2 = (num - 1);
            //    for (i = 0; i <= loopTo2; i++)
            //    {
            //        var loopTo3 = (i + 1);
            //        for (j = num; j >= loopTo3; j += -1)
            //        {
            //            isSwap = false;
            //            if (isStringkey)
            //            {
            //                if (isAscOrder)
            //                {
            //                    // UPGRADE_WARNING: �I�u�W�F�N�g array_buf(0, j) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                    // UPGRADE_WARNING: �I�u�W�F�N�g array_buf() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                    isSwap = Conversions.ToBoolean(Interaction.IIf(Strings.StrComp(array_buf[0, i], array_buf[0, j], CompareMethod.Text) == 1, true, false));
            //                }
            //                else
            //                {
            //                    // UPGRADE_WARNING: �I�u�W�F�N�g array_buf(0, j) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                    // UPGRADE_WARNING: �I�u�W�F�N�g array_buf() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                    isSwap = Conversions.ToBoolean(Interaction.IIf(Strings.StrComp(array_buf[0, i], array_buf[0, j], CompareMethod.Text) == -1, true, false));
            //                }
            //            }
            //            else if (isAscOrder)
            //            {
            //                // UPGRADE_WARNING: �I�u�W�F�N�g array_buf() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                isSwap = Conversions.ToBoolean(Interaction.IIf(Conversions.ToDouble(array_buf[0, i]) > Conversions.ToDouble(array_buf[0, j]), true, false));
            //            }
            //            else
            //            {
            //                // UPGRADE_WARNING: �I�u�W�F�N�g array_buf() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                isSwap = Conversions.ToBoolean(Interaction.IIf(Conversions.ToDouble(array_buf[0, i]) < Conversions.ToDouble(array_buf[0, j]), true, false));
            //            }

            //            if (isSwap)
            //            {
            //                for (k = 0; k <= 2; k++)
            //                {
            //                    // UPGRADE_WARNING: �I�u�W�F�N�g array_buf(k, i) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                    // UPGRADE_WARNING: �I�u�W�F�N�g var_buf(k) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                    var_buf[k] = array_buf[k, i];
            //                    // UPGRADE_WARNING: �I�u�W�F�N�g array_buf(k, j) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                    // UPGRADE_WARNING: �I�u�W�F�N�g array_buf(k, i) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                    array_buf[k, i] = array_buf[k, j];
            //                    // UPGRADE_WARNING: �I�u�W�F�N�g var_buf(k) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                    // UPGRADE_WARNING: �I�u�W�F�N�g array_buf(k, j) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                    array_buf[k, j] = var_buf[k];
            //                }
            //            }
            //        }
            //    }
            //}

            //if (!isKeySort)
            //{
            //    // ���߂ėv�f���\�[�g
            //    var loopTo4 = (num - 1);
            //    for (i = 0; i <= loopTo4; i++)
            //    {
            //        var loopTo5 = (i + 1);
            //        for (j = num; j >= loopTo5; j += -1)
            //        {
            //            isSwap = false;
            //            if (isStringValue)
            //            {
            //                if (isAscOrder)
            //                {
            //                    // UPGRADE_WARNING: �I�u�W�F�N�g array_buf(2, j) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                    // UPGRADE_WARNING: �I�u�W�F�N�g array_buf() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                    isSwap = Conversions.ToBoolean(Interaction.IIf(Strings.StrComp(array_buf[2, i], array_buf[2, j], CompareMethod.Text) == 1, true, false));
            //                }
            //                else
            //                {
            //                    // UPGRADE_WARNING: �I�u�W�F�N�g array_buf(2, j) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                    // UPGRADE_WARNING: �I�u�W�F�N�g array_buf() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                    isSwap = Conversions.ToBoolean(Interaction.IIf(Strings.StrComp(array_buf[2, i], array_buf[2, j], CompareMethod.Text) == -1, true, false));
            //                }
            //            }
            //            else if (isAscOrder)
            //            {
            //                // UPGRADE_WARNING: �I�u�W�F�N�g array_buf() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                isSwap = Conversions.ToBoolean(Interaction.IIf(Conversions.ToDouble(array_buf[2, i]) > Conversions.ToDouble(array_buf[2, j]), true, false));
            //            }
            //            else
            //            {
            //                // UPGRADE_WARNING: �I�u�W�F�N�g array_buf() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                isSwap = Conversions.ToBoolean(Interaction.IIf(Conversions.ToDouble(array_buf[2, i]) < Conversions.ToDouble(array_buf[2, j]), true, false));
            //            }

            //            if (isSwap)
            //            {
            //                for (k = Conversions.ToShort(Interaction.IIf(isStringkey, 0, 1)); k <= 2; k++)
            //                {
            //                    // UPGRADE_WARNING: �I�u�W�F�N�g array_buf(k, i) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                    // UPGRADE_WARNING: �I�u�W�F�N�g var_buf(k) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                    var_buf[k] = array_buf[k, i];
            //                    // UPGRADE_WARNING: �I�u�W�F�N�g array_buf(k, j) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                    // UPGRADE_WARNING: �I�u�W�F�N�g array_buf(k, i) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                    array_buf[k, i] = array_buf[k, j];
            //                    // UPGRADE_WARNING: �I�u�W�F�N�g var_buf(k) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                    // UPGRADE_WARNING: �I�u�W�F�N�g array_buf(k, j) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //                    array_buf[k, j] = var_buf[k];
            //                }
            //            }
            //        }
            //    }
            //}

            //// SRC�ϐ��ɍĔz�u
            //var loopTo6 = num;
            //for (i = 0; i <= loopTo6; i++)
            //{
            //    // UPGRADE_WARNING: �I�u�W�F�N�g array_buf() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //    buf = vname + "[" + Conversions.ToString(array_buf[0, i]) + "]";
            //    Expression.UndefineVariable(buf);
            //    if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(array_buf[1, i], Expression.ValueType.StringType, false)))
            //    {
            //        // UPGRADE_WARNING: �I�u�W�F�N�g array_buf() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //        Expression.SetVariable(buf, Expression.ValueType.StringType, Conversions.ToString(array_buf[2, i]), 0);
            //    }
            //    else
            //    {
            //        // UPGRADE_WARNING: �I�u�W�F�N�g array_buf() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
            //        Expression.SetVariable(buf, Expression.ValueType.NumericType, "", Conversions.ToDouble(array_buf[2, i]));
            //    }
            //}

            //return EventData.NextID;
        }
    }
}
