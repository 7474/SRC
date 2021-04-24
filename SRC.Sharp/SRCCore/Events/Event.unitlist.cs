using SRCCore.Maps;

namespace SRCCore.Events
{
    public partial class Event
    {
        // �C���^�[�~�b�V�����R�}���h�u���j�b�g���X�g�v�ɂ����郆�j�b�g���X�g���쐬����
        public static void MakeUnitList([Optional, DefaultParameterValue("")] ref string smode)
        {
            Unit u;
            Pilot p;
            short xx, yy;
            var key_list = default(int[]);
            short max_item;
            int max_value;
            string max_str;
            Unit[] unit_list;
            short i, j;
            ;

            // ���X�g�̃\�[�g���ڂ�ݒ�
            if (!string.IsNullOrEmpty(smode))
            {
                key_type = smode;
            }

            if (string.IsNullOrEmpty(key_type))
            {
                key_type = "�g�o";
            }

            // �}�E�X�J�[�\���������v��
            // UPGRADE_WARNING: Screen �v���p�e�B Screen.MousePointer �ɂ͐V�������삪�܂܂�܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' ���N���b�N���Ă��������B
            Cursor.Current = Cursors.WaitCursor;

            // ���炩���ߓP�ނ����Ă���
            foreach (Unit currentU in SRC.UList)
            {
                u = currentU;
                {
                    var withBlock = u;
                    if (withBlock.Status_Renamed == "�o��")
                    {
                        withBlock.Escape();
                    }
                }
            }

            // �}�b�v���N���A
            string argfname = "";
            Map.LoadMapData(ref argfname);
            string argdraw_mode = "";
            string argdraw_option = "�X�e�[�^�X";
            int argfilter_color = 0;
            double argfilter_trans_par = 0d;
            GUI.SetupBackground(ref argdraw_mode, ref argdraw_option, filter_color: ref argfilter_color, filter_trans_par: ref argfilter_trans_par);

            // ���j�b�g�ꗗ���쐬
            if (key_type != "����")
            {
                // �z��쐬
                unit_list = new Unit[(SRC.UList.Count() + 1)];
                key_list = new int[(SRC.UList.Count() + 1)];
                i = 0;
                foreach (Unit currentU1 in SRC.UList)
                {
                    u = currentU1;
                    {
                        var withBlock1 = u;
                        if (withBlock1.Status_Renamed == "�o��" | withBlock1.Status_Renamed == "�ҋ@")
                        {
                            i = (short)(i + 1);
                            unit_list[i] = u;

                            // �\�[�g���鍀�ڂɂ��킹�ă\�[�g�̍ۂ̗D��x������
                            switch (key_type ?? "")
                            {
                                case "�����N":
                                    {
                                        key_list[i] = withBlock1.Rank;
                                        break;
                                    }

                                case "�g�o":
                                    {
                                        key_list[i] = withBlock1.HP;
                                        break;
                                    }

                                case "�d�m":
                                    {
                                        key_list[i] = withBlock1.EN;
                                        break;
                                    }

                                case "���b":
                                    {
                                        key_list[i] = withBlock1.get_Armor("");
                                        break;
                                    }

                                case "�^����":
                                    {
                                        key_list[i] = withBlock1.get_Mobility("");
                                        break;
                                    }

                                case "�ړ���":
                                    {
                                        key_list[i] = withBlock1.Speed;
                                        break;
                                    }

                                case "�ő�U����":
                                    {
                                        var loopTo = withBlock1.CountWeapon();
                                        for (j = 1; j <= loopTo; j++)
                                        {
                                            string argattr = "��";
                                            if (withBlock1.IsWeaponMastered(j) & !withBlock1.IsDisabled(ref withBlock1.Weapon(j).Name) & !withBlock1.IsWeaponClassifiedAs(j, ref argattr))
                                            {
                                                string argtarea1 = "";
                                                if (withBlock1.WeaponPower(j, ref argtarea1) > key_list[i])
                                                {
                                                    string argtarea = "";
                                                    key_list[i] = withBlock1.WeaponPower(j, ref argtarea);
                                                }
                                            }
                                        }

                                        break;
                                    }

                                case "�Œ��˒�":
                                    {
                                        var loopTo1 = withBlock1.CountWeapon();
                                        for (j = 1; j <= loopTo1; j++)
                                        {
                                            string argattr1 = "��";
                                            if (withBlock1.IsWeaponMastered(j) & !withBlock1.IsDisabled(ref withBlock1.Weapon(j).Name) & !withBlock1.IsWeaponClassifiedAs(j, ref argattr1))
                                            {
                                                if (withBlock1.WeaponMaxRange(j) > key_list[i])
                                                {
                                                    key_list[i] = withBlock1.WeaponMaxRange(j);
                                                }
                                            }
                                        }

                                        break;
                                    }

                                case "���x��":
                                    {
                                        key_list[i] = withBlock1.MainPilot().Level;
                                        break;
                                    }

                                case "�r�o":
                                    {
                                        key_list[i] = withBlock1.MainPilot().MaxSP;
                                        break;
                                    }

                                case "�i��":
                                    {
                                        key_list[i] = withBlock1.MainPilot().Infight;
                                        break;
                                    }

                                case "�ˌ�":
                                    {
                                        key_list[i] = withBlock1.MainPilot().Shooting;
                                        break;
                                    }

                                case "����":
                                    {
                                        key_list[i] = withBlock1.MainPilot().Hit;
                                        break;
                                    }

                                case "���":
                                    {
                                        key_list[i] = withBlock1.MainPilot().Dodge;
                                        break;
                                    }

                                case "�Z��":
                                    {
                                        key_list[i] = withBlock1.MainPilot().Technique;
                                        break;
                                    }

                                case "����":
                                    {
                                        key_list[i] = withBlock1.MainPilot().Intuition;
                                        break;
                                    }
                            }
                        }
                    }
                }

                Array.Resize(ref unit_list, i + 1);
                Array.Resize(ref key_list, i + 1);

                // �\�[�g
                var loopTo2 = (short)(Information.UBound(key_list) - 1);
                for (i = 1; i <= loopTo2; i++)
                {
                    max_item = i;
                    max_value = key_list[i];
                    var loopTo3 = (short)Information.UBound(unit_list);
                    for (j = (short)(i + 1); j <= loopTo3; j++)
                    {
                        if (key_list[j] > max_value)
                        {
                            max_item = j;
                            max_value = key_list[j];
                        }
                    }

                    if (max_item != i)
                    {
                        u = unit_list[i];
                        unit_list[i] = unit_list[max_item];
                        unit_list[max_item] = u;
                        max_value = key_list[max_item];
                        key_list[max_item] = key_list[i];
                        key_list[i] = max_value;
                    }
                }
            }
            else
            {
                // �z��쐬
                unit_list = new Unit[(SRC.UList.Count() + 1)];
                var strkey_list = new object[(SRC.UList.Count() + 1)];
                i = 0;
                foreach (Unit currentU2 in SRC.UList)
                {
                    u = currentU2;
                    {
                        var withBlock2 = u;
                        if (withBlock2.Status_Renamed == "�o��" | withBlock2.Status_Renamed == "�ҋ@")
                        {
                            i = (short)(i + 1);
                            unit_list[i] = u;
                            string argoname = "���g��";
                            if (Expression.IsOptionDefined(ref argoname))
                            {
                                // UPGRADE_WARNING: �I�u�W�F�N�g strkey_list(i) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
                                strkey_list[i] = withBlock2.MainPilot().KanaName;
                            }
                            else
                            {
                                // UPGRADE_WARNING: �I�u�W�F�N�g strkey_list(i) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
                                strkey_list[i] = withBlock2.KanaName;
                            }
                        }
                    }
                }

                Array.Resize(ref unit_list, i + 1);
                Array.Resize(ref strkey_list, i + 1);

                // �\�[�g
                var loopTo4 = (short)(Information.UBound(strkey_list) - 1);
                for (i = 1; i <= loopTo4; i++)
                {
                    max_item = i;
                    // UPGRADE_WARNING: �I�u�W�F�N�g strkey_list(i) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
                    max_str = Conversions.ToString(strkey_list[i]);
                    var loopTo5 = (short)Information.UBound(strkey_list);
                    for (j = (short)(i + 1); j <= loopTo5; j++)
                    {
                        // UPGRADE_WARNING: �I�u�W�F�N�g strkey_list() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
                        if (Strings.StrComp(Conversions.ToString(strkey_list[j]), max_str, (CompareMethod)1) == -1)
                        {
                            max_item = j;
                            // UPGRADE_WARNING: �I�u�W�F�N�g strkey_list(j) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
                            max_str = Conversions.ToString(strkey_list[j]);
                        }
                    }

                    if (max_item != i)
                    {
                        u = unit_list[i];
                        unit_list[i] = unit_list[max_item];
                        unit_list[max_item] = u;

                        // UPGRADE_WARNING: �I�u�W�F�N�g strkey_list(i) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
                        // UPGRADE_WARNING: �I�u�W�F�N�g strkey_list(max_item) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
                        strkey_list[max_item] = strkey_list[i];
                    }
                }
            }

            // Font Regular 9pt �w�i
            // UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
            {
                var withBlock3 = GUI.MainForm.picMain(0).Font;
                // UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
                withBlock3.Size = 9;
                // UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
                withBlock3.Bold = false;
                // UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
                withBlock3.Italic = false;
            }

            GUI.PermanentStringMode = true;
            GUI.HCentering = false;
            GUI.VCentering = false;

            // ���j�b�g�̃��X�g���쐬
            xx = 1;
            yy = 1;
            var loopTo6 = (short)Information.UBound(unit_list);
            for (i = 1; i <= loopTo6; i++)
            {
                u = unit_list[i];
                {
                    var withBlock4 = u;
                    // ���j�b�g�o���ʒu��܂�Ԃ�
                    if (xx > 15)
                    {
                        xx = 1;
                        yy = (short)(yy + 1);
                        if (yy > 40)
                        {
                            // ���j�b�g�����������邽�߁A�ꕔ�̃p�C���b�g���\���o���܂���
                            break;
                        }
                    }

                    // �p�C���b�g������Ă��Ȃ��ꍇ�̓_�~�[�p�C���b�g���悹��
                    if (withBlock4.CountPilot() == 0)
                    {
                        string argpname = "�X�e�[�^�X�\���p�_�~�[�p�C���b�g(�U�R)";
                        string argpparty = "����";
                        string arggid = "";
                        p = SRC.PList.Add(ref argpname, 1, ref argpparty, gid: ref arggid);
                        p.Ride(ref u);
                    }

                    // �o��
                    withBlock4.UsedAction = 0;
                    withBlock4.StandBy(xx, yy);

                    // �v���C���[������ł��Ȃ��悤��
                    string argcname = "�񑀍�";
                    string argcdata = "";
                    withBlock4.AddCondition(ref argcname, -1, cdata: ref argcdata);

                    // ���j�b�g�̈��̂�\��
                    string argmsg = withBlock4.Nickname;
                    GUI.DrawString(ref argmsg, 32 * xx + 2, 32 * yy - 31);
                    withBlock4.Nickname = argmsg;

                    // �\�[�g���ڂɂ��킹�ă��j�b�g�̃X�e�[�^�X��\��
                    switch (key_type ?? "")
                    {
                        case "�����N":
                            {
                                string argtname = "HP";
                                string argtname1 = "EN";
                                string argmsg1 = "RK" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]) + " " + Expression.Term(ref argtname, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.HP) + " " + Expression.Term(ref argtname1, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.EN);
                                GUI.DrawString(ref argmsg1, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "�g�o":
                        case "�d�m":
                        case "����":
                            {
                                string argtname2 = "HP";
                                string argtname3 = "EN";
                                string argmsg2 = Expression.Term(ref argtname2, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.HP) + " " + Expression.Term(ref argtname3, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.EN);
                                GUI.DrawString(ref argmsg2, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "���b":
                            {
                                string argtname4 = "���b";
                                string argmsg3 = Expression.Term(ref argtname4, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg3, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "�^����":
                            {
                                string argtname5 = "�^����";
                                string argmsg4 = Expression.Term(ref argtname5, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg4, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "�ړ���":
                            {
                                string argtname6 = "�ړ���";
                                string argmsg5 = Expression.Term(ref argtname6, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg5, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "�ő�U����":
                            {
                                string argmsg6 = "�U����" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg6, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "�Œ��˒�":
                            {
                                string argmsg7 = "�˒�" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg7, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "���x��":
                            {
                                string argmsg8 = "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg8, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "�r�o":
                            {
                                string argtname7 = "SP";
                                string argmsg9 = Expression.Term(ref argtname7, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg9, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "�i��":
                            {
                                string argtname8 = "�i��";
                                string argmsg10 = Expression.Term(ref argtname8, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg10, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "�ˌ�":
                            {
                                if (withBlock4.MainPilot().HasMana())
                                {
                                    string argtname9 = "����";
                                    string argmsg11 = Expression.Term(ref argtname9, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                    GUI.DrawString(ref argmsg11, 32 * xx + 2, 32 * yy - 15);
                                }
                                else
                                {
                                    string argtname10 = "�ˌ�";
                                    string argmsg12 = Expression.Term(ref argtname10, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                    GUI.DrawString(ref argmsg12, 32 * xx + 2, 32 * yy - 15);
                                }

                                break;
                            }

                        case "����":
                            {
                                string argtname11 = "����";
                                string argmsg13 = Expression.Term(ref argtname11, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg13, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "���":
                            {
                                string argtname12 = "���";
                                string argmsg14 = Expression.Term(ref argtname12, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg14, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "�Z��":
                            {
                                string argtname13 = "�Z��";
                                string argmsg15 = Expression.Term(ref argtname13, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg15, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "����":
                            {
                                string argtname14 = "����";
                                string argmsg16 = Expression.Term(ref argtname14, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg16, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }
                    }

                    // �\���ʒu���E��5�}�X���炷
                    xx = (short)(xx + 5);
                }
            }

            // �t�H���g�̐ݒ��߂��Ă���
            // UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
            {
                var withBlock5 = GUI.MainForm.picMain(0).Font;
                // UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
                withBlock5.Size = 16;
                // UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
                withBlock5.Bold = true;
                // UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
                withBlock5.Italic = false;
            }

            GUI.PermanentStringMode = false;
            GUI.RedrawScreen();

            // �}�E�X�J�[�\�������ɖ߂�
            // UPGRADE_WARNING: Screen �v���p�e�B Screen.MousePointer �ɂ͐V�������삪�܂܂�܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' ���N���b�N���Ă��������B
            Cursor.Current = Cursors.Default;
        }
    }
}
