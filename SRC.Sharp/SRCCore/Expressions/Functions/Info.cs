using SRCCore.Lib;
using SRCCore.Models;

namespace SRCCore.Expressions.Functions
{
    // TODO Impl Info
    public class Info : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            Units.Unit u = null;
            UnitData ud = null;
            Pilots.Pilot p = null;
            PilotData pd = null;
            NonPilotData nd = null;
            Items.Item it = null;
            ItemData itd = null;
            SpecialPowerData spd = null;
            int idx;

            // �e�I�u�W�F�N�g�̐ݒ�
            switch (@params[1])
            {
                case "���j�b�g":
                    {
                        u = SRC.UList.Item(@params[2]);
                        idx = 3;
                        break;
                    }

                case "���j�b�g�f�[�^":
                    {
                        ud = SRC.UDList.Item(@params[2]);
                        idx = 3;
                        break;
                    }

                case "�p�C���b�g":
                    {
                        p = SRC.PList.Item(@params[2]);
                        idx = 3;
                        break;
                    }

                case "�p�C���b�g�f�[�^":
                    {
                        pd = SRC.PDList.Item(@params[2]);
                        idx = 3;
                        break;
                    }

                case "��퓬��":
                    {
                        nd = SRC.NPDList.Item(@params[2]);
                        idx = 3;
                        break;
                    }

                case "�A�C�e��":
                    {
                        if (SRC.IList.IsDefined(@params[2]))
                        {
                            it = SRC.IList.Item(@params[2]);
                        }
                        else
                        {
                            itd = SRC.IDList.Item(@params[2]);
                        }

                        idx = 3;
                        break;
                    }

                case "�A�C�e���f�[�^":
                    {
                        itd = SRC.IDList.Item(@params[2]);
                        idx = 3;
                        break;
                    }

                case "�X�y�V�����p���[":
                    {
                        spd = SRC.SPDList.Item(@params[2]);
                        idx = 3;
                        break;
                    }

                case "�}�b�v":
                case "�I�v�V����":
                    {
                        idx = 1;
                        break;
                    }

                case var @case when @case == "":
                    {
                        return etype;
                    }

                default:
                    {
                        u = SRC.UList.Item(@params[1]);
                        ud = SRC.UDList.Item(@params[1]);
                        p = SRC.PList.Item(@params[1]);
                        pd = SRC.PDList.Item(@params[1]);
                        nd = SRC.NPDList.Item(@params[1]);
                        it = SRC.IList.Item(@params[1]);
                        itd = SRC.IDList.Item(@params[1]);
                        spd = SRC.SPDList.Item(@params[1]);
                        idx = 2;
                        break;
                    }
            }

            //int mx = default, my_Renamed = default;
            //switch (@params[idx] ?? "")
            //{
            //    case "����":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = u.Name;
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = ud.Name;
            //            }
            //            else if (p is object)
            //            {
            //                EvalInfoFuncRet = p.Name;
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = pd.Name;
            //            }
            //            else if (nd is object)
            //            {
            //                EvalInfoFuncRet = nd.Name;
            //            }
            //            else if (it is object)
            //            {
            //                EvalInfoFuncRet = it.Name;
            //            }
            //            else if (itd is object)
            //            {
            //                EvalInfoFuncRet = itd.Name;
            //            }
            //            else if (spd is object)
            //            {
            //                EvalInfoFuncRet = spd.Name;
            //            }

            //            break;
            //        }

            //    case "�ǂ݉���":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = u.KanaName;
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = ud.KanaName;
            //            }
            //            else if (p is object)
            //            {
            //                EvalInfoFuncRet = p.KanaName;
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = pd.KanaName;
            //            }
            //            else if (it is object)
            //            {
            //                EvalInfoFuncRet = it.Data.KanaName;
            //            }
            //            else if (itd is object)
            //            {
            //                EvalInfoFuncRet = itd.KanaName;
            //            }
            //            else if (spd is object)
            //            {
            //                EvalInfoFuncRet = spd.KanaName;
            //            }

            //            break;
            //        }

            //    case "����":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = u.Nickname0;
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = ud.Nickname;
            //            }
            //            else if (p is object)
            //            {
            //                EvalInfoFuncRet = p.get_Nickname(false);
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = pd.Nickname;
            //            }
            //            else if (nd is object)
            //            {
            //                EvalInfoFuncRet = nd.Nickname;
            //            }
            //            else if (it is object)
            //            {
            //                EvalInfoFuncRet = it.Nickname();
            //            }
            //            else if (itd is object)
            //            {
            //                EvalInfoFuncRet = itd.Nickname;
            //            }

            //            break;
            //        }

            //    case "����":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = p.Sex;
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = pd.Sex;
            //            }

            //            return EvalInfoFuncRet;
            //        }

            //    case "���j�b�g�N���X":
            //    case "�@�̃N���X":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = u.Class;
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = ud.Class;
            //            }
            //            else if (p is object)
            //            {
            //                EvalInfoFuncRet = p.Class;
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = pd.Class;
            //            }

            //            break;
            //        }

            //    case "�n�`�K��":
            //        {
            //            if (u is object)
            //            {
            //                for (i = 1; i <= 4; i++)
            //                {
            //                    switch (u.get_Adaption(i))
            //                    {
            //                        case 5:
            //                            {
            //                                EvalInfoFuncRet = EvalInfoFuncRet + "S";
            //                                break;
            //                            }

            //                        case 4:
            //                            {
            //                                EvalInfoFuncRet = EvalInfoFuncRet + "A";
            //                                break;
            //                            }

            //                        case 3:
            //                            {
            //                                EvalInfoFuncRet = EvalInfoFuncRet + "B";
            //                                break;
            //                            }

            //                        case 2:
            //                            {
            //                                EvalInfoFuncRet = EvalInfoFuncRet + "C";
            //                                break;
            //                            }

            //                        case 1:
            //                            {
            //                                EvalInfoFuncRet = EvalInfoFuncRet + "D";
            //                                break;
            //                            }

            //                        default:
            //                            {
            //                                EvalInfoFuncRet = EvalInfoFuncRet + "E";
            //                                break;
            //                            }
            //                    }
            //                }
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = ud.Adaption;
            //            }
            //            else if (p is object)
            //            {
            //                EvalInfoFuncRet = p.Adaption;
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = pd.Adaption;
            //            }

            //            break;
            //        }

            //    case "�o���l":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = u.ExpValue.ToString();
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = ud.ExpValue.ToString();
            //            }
            //            else if (p is object)
            //            {
            //                EvalInfoFuncRet = p.ExpValue.ToString();
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = pd.ExpValue.ToString();
            //            }

            //            break;
            //        }

            //    case "�i��":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.Infight);
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(pd.Infight);
            //            }

            //            break;
            //        }

            //    case "�ˌ�":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.Shooting);
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(pd.Shooting);
            //            }

            //            return EvalInfoFuncRet;
            //        }

            //    case "����":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.Hit);
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(pd.Hit);
            //            }

            //            break;
            //        }

            //    case "���":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.Dodge);
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(pd.Dodge);
            //            }

            //            break;
            //        }

            //    case "�Z��":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.Technique);
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(pd.Technique);
            //            }

            //            break;
            //        }

            //    case "����":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.Intuition);
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(pd.Intuition);
            //            }

            //            break;
            //        }

            //    case "�h��":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.Defense);
            //            }

            //            break;
            //        }

            //    case "�i����{�l":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.InfightBase);
            //            }

            //            break;
            //        }

            //    case "�ˌ���{�l":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.ShootingBase);
            //            }

            //            break;
            //        }

            //    case "������{�l":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.HitBase);
            //            }

            //            break;
            //        }

            //    case "�����{�l":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.DodgeBase);
            //            }

            //            break;
            //        }

            //    case "�Z�ʊ�{�l":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.TechniqueBase);
            //            }

            //            break;
            //        }

            //    case "������{�l":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.IntuitionBase);
            //            }

            //            break;
            //        }

            //    case "�i���C���l":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.InfightMod);
            //            }

            //            break;
            //        }

            //    case "�ˌ��C���l":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.ShootingMod);
            //            }

            //            break;
            //        }

            //    case "�����C���l":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.HitMod);
            //            }

            //            break;
            //        }

            //    case "����C���l":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.DodgeMod);
            //            }

            //            break;
            //        }

            //    case "�Z�ʏC���l":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.TechniqueMod);
            //            }

            //            break;
            //        }

            //    case "�����C���l":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.IntuitionMod);
            //            }

            //            break;
            //        }

            //    case "�i���x���C���l":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.InfightMod2);
            //            }

            //            break;
            //        }

            //    case "�ˌ��x���C���l":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.ShootingMod2);
            //            }

            //            break;
            //        }

            //    case "�����x���C���l":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.HitMod2);
            //            }

            //            break;
            //        }

            //    case "����x���C���l":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.DodgeMod2);
            //            }

            //            break;
            //        }

            //    case "�Z�ʎx���C���l":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.TechniqueMod2);
            //            }

            //            break;
            //        }

            //    case "�����x���C���l":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.IntuitionMod2);
            //            }

            //            break;
            //        }

            //    case "���i":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = p.Personality;
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = pd.Personality;
            //            }

            //            break;
            //        }

            //    case "�ő�r�o":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.MaxSP);
            //                if (p.MaxSP == 0 & p.Unit is object)
            //                {
            //                    if (ReferenceEquals(p, p.Unit.MainPilot()))
            //                    {
            //                        EvalInfoFuncRet = SrcFormatter.Format(p.Unit.Pilot(1).MaxSP);
            //                    }
            //                }
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(pd.SP);
            //            }

            //            break;
            //        }

            //    case "�r�o":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.SP);
            //                if (p.MaxSP == 0 & p.Unit is object)
            //                {
            //                    if (ReferenceEquals(p, p.Unit.MainPilot()))
            //                    {
            //                        EvalInfoFuncRet = SrcFormatter.Format(p.Unit.Pilot(1).SP);
            //                    }
            //                }
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(pd.SP);
            //            }

            //            break;
            //        }

            //    case "�O���t�B�b�N":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = u.get_Bitmap(true);
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = ud.Bitmap0;
            //            }
            //            else if (p is object)
            //            {
            //                EvalInfoFuncRet = p.get_Bitmap(true);
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = pd.Bitmap0;
            //            }
            //            else if (nd is object)
            //            {
            //                EvalInfoFuncRet = nd.Bitmap0;
            //            }

            //            break;
            //        }

            //    case "�l�h�c�h":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = p.BGM;
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = pd.BGM;
            //            }

            //            break;
            //        }

            //    case "���x��":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.Level);
            //            }

            //            break;
            //        }

            //    case "�ݐόo���l":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.Exp);
            //            }

            //            break;
            //        }

            //    case "�C��":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.Morale);
            //            }

            //            break;
            //        }

            //    case "�ő���":
            //    case "�ő�v���[�i":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.MaxPlana());
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(pd.SkillLevel(0, "���"));
            //            }

            //            break;
            //        }

            //    case "���":
            //    case "�v���[�i":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.Plana);
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(pd.SkillLevel(0, "���"));
            //            }

            //            break;
            //        }

            //    case "������":
            //    case "�V���N����":
            //        {
            //            if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.SynchroRate());
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(pd.SkillLevel(0, "������"));
            //            }

            //            break;
            //        }

            //    case "�X�y�V�����p���[":
            //    case "���_�R�}���h":
            //    case "���_":
            //        {
            //            if (p is object)
            //            {
            //                if (p.MaxSP == 0 & p.Unit is object)
            //                {
            //                    if (ReferenceEquals(p, p.Unit.MainPilot()))
            //                    {
            //                        p = p.Unit.Pilot(1);
            //                    }
            //                }

            //                {
            //                    var withBlock = p;
            //                    var loopTo = withBlock.CountSpecialPower;
            //                    for (i = 1; i <= loopTo; i++)
            //                        EvalInfoFuncRet = EvalInfoFuncRet + " " + withBlock.get_SpecialPower(i);
            //                }

            //                EvalInfoFuncRet = Strings.Trim(EvalInfoFuncRet);
            //            }
            //            else if (pd is object)
            //            {
            //                var loopTo1 = pd.CountSpecialPower(100);
            //                for (i = 1; i <= loopTo1; i++)
            //                    EvalInfoFuncRet = EvalInfoFuncRet + " " + pd.SpecialPower(100, i);
            //                EvalInfoFuncRet = Strings.Trim(EvalInfoFuncRet);
            //            }

            //            break;
            //        }

            //    case "�X�y�V�����p���[���L":
            //    case "���_�R�}���h���L":
            //        {
            //            if (p is object)
            //            {
            //                if (p.MaxSP == 0 & p.Unit is object)
            //                {
            //                    if (ReferenceEquals(p, p.Unit.MainPilot()))
            //                    {
            //                        p = p.Unit.Pilot(1);
            //                    }
            //                }

            //                if (p.IsSpecialPowerAvailable(@params[idx + 1]))
            //                {
            //                    EvalInfoFuncRet = "1";
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = "0";
            //                }
            //            }
            //            else if (pd is object)
            //            {
            //                if (pd.IsSpecialPowerAvailable(100, @params[idx + 1]))
            //                {
            //                    EvalInfoFuncRet = "1";
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = "0";
            //                }
            //            }

            //            break;
            //        }

            //    case "�X�y�V�����p���[�R�X�g":
            //    case "���_�R�}���h�R�X�g":
            //        {
            //            if (p is object)
            //            {
            //                if (p.MaxSP == 0 & p.Unit is object)
            //                {
            //                    if (ReferenceEquals(p, p.Unit.MainPilot()))
            //                    {
            //                        p = p.Unit.Pilot(1);
            //                    }
            //                }

            //                EvalInfoFuncRet = SrcFormatter.Format(p.SpecialPowerCost(@params[idx + 1]));
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(pd.SpecialPowerCost(@params[idx + 1]));
            //            }

            //            break;
            //        }

            //    case "����\�͐�":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(u.CountFeature());
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(ud.CountFeature());
            //            }
            //            else if (p is object)
            //            {
            //                EvalInfoFuncRet = p.CountSkill().ToString();
            //            }
            //            else if (pd is object)
            //            {
            //                int localLLength() { string arglist = pd.Skill(100); var ret = GeneralLib.LLength(arglist); return ret; }

            //                int localLLength1() { string arglist = pd.Skill(100); var ret = GeneralLib.LLength(arglist); return ret; }

            //                EvalInfoFuncRet = localLLength1().ToString();
            //            }
            //            else if (it is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(it.CountFeature());
            //            }
            //            else if (itd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(itd.CountFeature());
            //            }

            //            break;
            //        }

            //    case "����\��":
            //        {
            //            if (u is object)
            //            {
            //                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                {
            //                    EvalInfoFuncRet = u.Feature(Conversions.Toint(@params[idx + 1]));
            //                }
            //            }
            //            else if (ud is object)
            //            {
            //                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                {
            //                    EvalInfoFuncRet = ud.Feature(Conversions.Toint(@params[idx + 1]));
            //                }
            //            }
            //            else if (p is object)
            //            {
            //                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                {
            //                    EvalInfoFuncRet = p.Skill(Conversions.Toint(@params[idx + 1]));
            //                }
            //            }
            //            else if (pd is object)
            //            {
            //                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                {
            //                    EvalInfoFuncRet = GeneralLib.LIndex(pd.Skill(100), Conversions.Toint(@params[idx + 1]));
            //                }
            //            }
            //            else if (it is object)
            //            {
            //                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                {
            //                    EvalInfoFuncRet = it.Feature(Conversions.Toint(@params[idx + 1]));
            //                }
            //            }
            //            else if (itd is object)
            //            {
            //                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                {
            //                    EvalInfoFuncRet = itd.Feature(Conversions.Toint(@params[idx + 1]));
            //                }
            //            }

            //            break;
            //        }

            //    case "����\�͖���":
            //        {
            //            aname = @params[idx + 1];

            //            // �G���A�X����`����Ă���H
            //            if (SRC.ALDList.IsDefined(aname))
            //            {
            //                {
            //                    var withBlock1 = SRC.ALDList.Item(aname);
            //                    var loopTo2 = withBlock1.Count;
            //                    for (i = 1; i <= loopTo2; i++)
            //                    {
            //                        string localLIndex() { string arglist = withBlock1.get_AliasData(i); var ret = GeneralLib.LIndex(arglist, 1); withBlock1.get_AliasData(i) = arglist; return ret; }

            //                        if ((localLIndex() ?? "") == (aname ?? ""))
            //                        {
            //                            aname = withBlock1.get_AliasType(i);
            //                            break;
            //                        }
            //                    }

            //                    if (i > withBlock1.Count)
            //                    {
            //                        aname = withBlock1.get_AliasType(1);
            //                    }
            //                }
            //            }

            //            if (u is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    EvalInfoFuncRet = u.FeatureName(Conversions.Toint(@params[idx + 1]));
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = u.FeatureName(aname);
            //                }
            //            }
            //            else if (ud is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    EvalInfoFuncRet = ud.FeatureName(Conversions.Toint(aname));
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = ud.FeatureName(aname);
            //                }
            //            }
            //            else if (p is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    EvalInfoFuncRet = p.SkillName(Conversions.Toint(aname));
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = p.SkillName(aname);
            //                }
            //            }
            //            else if (pd is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    string localLIndex1() { string arglist = pd.Skill(100); var ret = GeneralLib.LIndex(arglist, Conversions.Toint(aname)); return ret; }

            //                    EvalInfoFuncRet = pd.SkillName(100, localLIndex1());
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = pd.SkillName(100, aname);
            //                }
            //            }
            //            else if (it is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    EvalInfoFuncRet = it.FeatureName(Conversions.Toint(aname));
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = it.FeatureName(aname);
            //                }
            //            }
            //            else if (itd is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    EvalInfoFuncRet = itd.FeatureName(Conversions.Toint(aname));
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = itd.FeatureName(aname);
            //                }
            //            }

            //            break;
            //        }

            //    case "����\�͏��L":
            //        {
            //            aname = @params[idx + 1];

            //            // �G���A�X����`����Ă���H
            //            if (SRC.ALDList.IsDefined(aname))
            //            {
            //                {
            //                    var withBlock2 = SRC.ALDList.Item(aname);
            //                    var loopTo3 = withBlock2.Count;
            //                    for (i = 1; i <= loopTo3; i++)
            //                    {
            //                        string localLIndex2() { string arglist = withBlock2.get_AliasData(i); var ret = GeneralLib.LIndex(arglist, 1); withBlock2.get_AliasData(i) = arglist; return ret; }

            //                        if ((localLIndex2() ?? "") == (aname ?? ""))
            //                        {
            //                            aname = withBlock2.get_AliasType(i);
            //                            break;
            //                        }
            //                    }

            //                    if (i > withBlock2.Count)
            //                    {
            //                        aname = withBlock2.get_AliasType(1);
            //                    }
            //                }
            //            }

            //            if (u is object)
            //            {
            //                if (u.IsFeatureAvailable(aname))
            //                {
            //                    EvalInfoFuncRet = "1";
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = "0";
            //                }
            //            }
            //            else if (ud is object)
            //            {
            //                if (ud.IsFeatureAvailable(aname))
            //                {
            //                    EvalInfoFuncRet = "1";
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = "0";
            //                }
            //            }
            //            else if (p is object)
            //            {
            //                if (p.IsSkillAvailable(aname))
            //                {
            //                    EvalInfoFuncRet = "1";
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = "0";
            //                }
            //            }
            //            else if (pd is object)
            //            {
            //                if (pd.IsSkillAvailable(100, aname))
            //                {
            //                    EvalInfoFuncRet = "1";
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = "0";
            //                }
            //            }
            //            else if (it is object)
            //            {
            //                if (it.IsFeatureAvailable(aname))
            //                {
            //                    EvalInfoFuncRet = "1";
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = "0";
            //                }
            //            }
            //            else if (itd is object)
            //            {
            //                if (itd.IsFeatureAvailable(aname))
            //                {
            //                    EvalInfoFuncRet = "1";
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = "0";
            //                }
            //            }

            //            break;
            //        }

            //    case "����\�̓��x��":
            //        {
            //            aname = @params[idx + 1];

            //            // �G���A�X����`����Ă���H
            //            if (SRC.ALDList.IsDefined(aname))
            //            {
            //                {
            //                    var withBlock3 = SRC.ALDList.Item(aname);
            //                    var loopTo4 = withBlock3.Count;
            //                    for (i = 1; i <= loopTo4; i++)
            //                    {
            //                        string localLIndex3() { string arglist = withBlock3.get_AliasData(i); var ret = GeneralLib.LIndex(arglist, 1); withBlock3.get_AliasData(i) = arglist; return ret; }

            //                        if ((localLIndex3() ?? "") == (aname ?? ""))
            //                        {
            //                            aname = withBlock3.get_AliasType(i);
            //                            break;
            //                        }
            //                    }

            //                    if (i > withBlock3.Count)
            //                    {
            //                        aname = withBlock3.get_AliasType(1);
            //                    }
            //                }
            //            }

            //            if (u is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    double localFeatureLevel() { object argIndex1 = Conversions.Toint(aname); var ret = u.FeatureLevel(argIndex1); return ret; }

            //                    EvalInfoFuncRet = SrcFormatter.Format(localFeatureLevel());
            //                }
            //                else
            //                {
            //                    double localFeatureLevel1() { object argIndex1 = aname; var ret = u.FeatureLevel(argIndex1); return ret; }

            //                    EvalInfoFuncRet = SrcFormatter.Format(localFeatureLevel1());
            //                }
            //            }
            //            else if (ud is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    double localFeatureLevel2() { object argIndex1 = Conversions.Toint(aname); var ret = ud.FeatureLevel(argIndex1); return ret; }

            //                    EvalInfoFuncRet = SrcFormatter.Format(localFeatureLevel2());
            //                }
            //                else
            //                {
            //                    double localFeatureLevel3() { object argIndex1 = aname; var ret = ud.FeatureLevel(argIndex1); return ret; }

            //                    EvalInfoFuncRet = SrcFormatter.Format(localFeatureLevel3());
            //                }
            //            }
            //            else if (p is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    double localSkillLevel() { object argIndex1 = Conversions.Toint(aname); string argref_mode = ""; var ret = p.SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

            //                    EvalInfoFuncRet = SrcFormatter.Format(localSkillLevel());
            //                }
            //                else
            //                {
            //                    double localSkillLevel1() { object argIndex1 = aname; string argref_mode = ""; var ret = p.SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

            //                    EvalInfoFuncRet = SrcFormatter.Format(localSkillLevel1());
            //                }
            //            }
            //            else if (pd is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    string localLIndex4() { string arglist = pd.Skill(100); var ret = GeneralLib.LIndex(arglist, Conversions.Toint(aname)); return ret; }

            //                    double localSkillLevel2() { string argsname = hs69a3321344d140f8b91f1e9add379ed5(); var ret = pd.SkillLevel(100, argsname); return ret; }

            //                    EvalInfoFuncRet = SrcFormatter.Format(localSkillLevel2());
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = SrcFormatter.Format(pd.SkillLevel(100, aname));
            //                }
            //            }
            //            else if (it is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    double localFeatureLevel4() { object argIndex1 = Conversions.Toint(aname); var ret = it.FeatureLevel(argIndex1); return ret; }

            //                    EvalInfoFuncRet = SrcFormatter.Format(localFeatureLevel4());
            //                }
            //                else
            //                {
            //                    double localFeatureLevel5() { object argIndex1 = aname; var ret = it.FeatureLevel(argIndex1); return ret; }

            //                    EvalInfoFuncRet = SrcFormatter.Format(localFeatureLevel5());
            //                }
            //            }
            //            else if (itd is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    double localFeatureLevel6() { object argIndex1 = Conversions.Toint(aname); var ret = itd.FeatureLevel(argIndex1); return ret; }

            //                    EvalInfoFuncRet = SrcFormatter.Format(localFeatureLevel6());
            //                }
            //                else
            //                {
            //                    double localFeatureLevel7() { object argIndex1 = aname; var ret = itd.FeatureLevel(argIndex1); return ret; }

            //                    EvalInfoFuncRet = SrcFormatter.Format(localFeatureLevel7());
            //                }
            //            }

            //            break;
            //        }

            //    case "����\�̓f�[�^":
            //        {
            //            aname = @params[idx + 1];

            //            // �G���A�X����`����Ă���H
            //            if (SRC.ALDList.IsDefined(aname))
            //            {
            //                {
            //                    var withBlock4 = SRC.ALDList.Item(aname);
            //                    var loopTo5 = withBlock4.Count;
            //                    for (i = 1; i <= loopTo5; i++)
            //                    {
            //                        string localLIndex5() { string arglist = withBlock4.get_AliasData(i); var ret = GeneralLib.LIndex(arglist, 1); withBlock4.get_AliasData(i) = arglist; return ret; }

            //                        if ((localLIndex5() ?? "") == (aname ?? ""))
            //                        {
            //                            aname = withBlock4.get_AliasType(i);
            //                            break;
            //                        }
            //                    }

            //                    if (i > withBlock4.Count)
            //                    {
            //                        aname = withBlock4.get_AliasType(1);
            //                    }
            //                }
            //            }

            //            if (u is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    EvalInfoFuncRet = u.FeatureData(Conversions.Toint(aname));
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = u.FeatureData(aname);
            //                }
            //            }
            //            else if (ud is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    EvalInfoFuncRet = ud.FeatureData(Conversions.Toint(aname));
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = ud.FeatureData(aname);
            //                }
            //            }
            //            else if (p is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    EvalInfoFuncRet = p.SkillData(Conversions.Toint(aname));
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = p.SkillData(aname);
            //                }
            //            }
            //            else if (pd is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    string localLIndex6() { string arglist = pd.Skill(100); var ret = GeneralLib.LIndex(arglist, Conversions.Toint(aname)); return ret; }

            //                    EvalInfoFuncRet = pd.SkillData(100, localLIndex6());
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = pd.SkillData(100, aname);
            //                }
            //            }
            //            else if (it is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    EvalInfoFuncRet = it.FeatureData(Conversions.Toint(aname));
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = it.FeatureData(aname);
            //                }
            //            }
            //            else if (itd is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    EvalInfoFuncRet = itd.FeatureData(Conversions.Toint(aname));
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = itd.FeatureData(aname);
            //                }
            //            }

            //            break;
            //        }

            //    case "����\�͕K�v�Z�\":
            //        {
            //            aname = @params[idx + 1];

            //            // �G���A�X����`����Ă���H
            //            if (SRC.ALDList.IsDefined(aname))
            //            {
            //                {
            //                    var withBlock5 = SRC.ALDList.Item(aname);
            //                    var loopTo6 = withBlock5.Count;
            //                    for (i = 1; i <= loopTo6; i++)
            //                    {
            //                        string localLIndex7() { string arglist = withBlock5.get_AliasData(i); var ret = GeneralLib.LIndex(arglist, 1); withBlock5.get_AliasData(i) = arglist; return ret; }

            //                        if ((localLIndex7() ?? "") == (aname ?? ""))
            //                        {
            //                            aname = withBlock5.get_AliasType(i);
            //                            break;
            //                        }
            //                    }

            //                    if (i > withBlock5.Count)
            //                    {
            //                        aname = withBlock5.get_AliasType(1);
            //                    }
            //                }
            //            }

            //            if (u is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    EvalInfoFuncRet = u.FeatureNecessarySkill(Conversions.Toint(aname));
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = u.FeatureNecessarySkill(aname);
            //                }
            //            }
            //            else if (ud is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    EvalInfoFuncRet = ud.FeatureNecessarySkill(Conversions.Toint(aname));
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = ud.FeatureNecessarySkill(aname);
            //                }
            //            }
            //            else if (it is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    EvalInfoFuncRet = it.FeatureNecessarySkill(Conversions.Toint(aname));
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = it.FeatureNecessarySkill(aname);
            //                }
            //            }
            //            else if (itd is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    EvalInfoFuncRet = itd.FeatureNecessarySkill(Conversions.Toint(aname));
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = itd.FeatureNecessarySkill(aname);
            //                }
            //            }

            //            break;
            //        }

            //    case "����\�͉��":
            //        {
            //            aname = @params[idx + 1];

            //            // �G���A�X����`����Ă���H
            //            if (SRC.ALDList.IsDefined(aname))
            //            {
            //                {
            //                    var withBlock6 = SRC.ALDList.Item(aname);
            //                    var loopTo7 = withBlock6.Count;
            //                    for (i = 1; i <= loopTo7; i++)
            //                    {
            //                        string localLIndex8() { string arglist = withBlock6.get_AliasData(i); var ret = GeneralLib.LIndex(arglist, 1); withBlock6.get_AliasData(i) = arglist; return ret; }

            //                        if ((localLIndex8() ?? "") == (aname ?? ""))
            //                        {
            //                            aname = withBlock6.get_AliasType(i);
            //                            break;
            //                        }
            //                    }

            //                    if (i > withBlock6.Count)
            //                    {
            //                        aname = withBlock6.get_AliasType(1);
            //                    }
            //                }
            //            }

            //            if (u is object)
            //            {
            //                if (GeneralLib.IsNumber(aname))
            //                {
            //                    EvalInfoFuncRet = Help.FeatureHelpMessage(u, Conversions.Toint(aname), false);
            //                }
            //                else
            //                {
            //                    EvalInfoFuncRet = Help.FeatureHelpMessage(u, aname, false);
            //                }

            //                if (string.IsNullOrEmpty(EvalInfoFuncRet) & p is object)
            //                {
            //                    EvalInfoFuncRet = Help.SkillHelpMessage(p, aname);
            //                }
            //            }
            //            else if (p is object)
            //            {
            //                EvalInfoFuncRet = Help.SkillHelpMessage(p, aname);
            //                if (string.IsNullOrEmpty(EvalInfoFuncRet) & u is object)
            //                {
            //                    if (GeneralLib.IsNumber(aname))
            //                    {
            //                        EvalInfoFuncRet = Help.FeatureHelpMessage(u, Conversions.Toint(aname), false);
            //                    }
            //                    else
            //                    {
            //                        EvalInfoFuncRet = Help.FeatureHelpMessage(u, aname, false);
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "�K��p�C���b�g��":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(u.Data.PilotNum);
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(ud.PilotNum);
            //            }

            //            break;
            //        }

            //    case "�p�C���b�g��":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(u.CountPilot());
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(ud.PilotNum);
            //            }

            //            break;
            //        }

            //    case "�T�|�[�g��":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(u.CountSupport());
            //            }

            //            break;
            //        }

            //    case "�ő�A�C�e����":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(u.Data.ItemNum);
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(ud.ItemNum);
            //            }

            //            break;
            //        }

            //    case "�A�C�e����":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(u.CountItem());
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(ud.ItemNum);
            //            }

            //            break;
            //        }

            //    case "�A�C�e��":
            //        {
            //            if (u is object)
            //            {
            //                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                {
            //                    i = Conversions.Toint(@params[idx + 1]);
            //                    if (0 < i & i <= u.CountItem())
            //                    {
            //                        Item localItem() { object argIndex1 = i; var ret = u.Item(argIndex1); return ret; }

            //                        EvalInfoFuncRet = SrcFormatter.Format(localItem().Name);
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "�A�C�e���h�c":
            //        {
            //            if (u is object)
            //            {
            //                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                {
            //                    i = Conversions.Toint(@params[idx + 1]);
            //                    if (0 < i & i <= u.CountItem())
            //                    {
            //                        Item localItem1() { object argIndex1 = i; var ret = u.Item(argIndex1); return ret; }

            //                        EvalInfoFuncRet = SrcFormatter.Format(localItem1().ID);
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "�ړ��\�n�`":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = u.Transportation;
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = ud.Transportation;
            //            }

            //            break;
            //        }

            //    case "�ړ���":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(u.Speed);
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(ud.Speed);
            //            }

            //            break;
            //        }

            //    case "�T�C�Y":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = u.Size;
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = ud.Size;
            //            }

            //            break;
            //        }

            //    case "�C����":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = u.Value.ToString();
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = ud.Value.ToString();
            //            }

            //            break;
            //        }

            //    case "�ő�g�o":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(u.MaxHP);
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(ud.HP);
            //            }

            //            break;
            //        }

            //    case "�g�o":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(u.HP);
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(ud.HP);
            //            }

            //            break;
            //        }

            //    case "�ő�d�m":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(u.MaxEN);
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(ud.EN);
            //            }

            //            break;
            //        }

            //    case "�d�m":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(u.EN);
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(ud.EN);
            //            }

            //            break;
            //        }

            //    case "���b":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(u.get_Armor(""));
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(ud.Armor);
            //            }

            //            break;
            //        }

            //    case "�^����":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(u.get_Mobility(""));
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(ud.Mobility);
            //            }

            //            break;
            //        }

            //    case "���퐔":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(u.CountWeapon());
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(ud.CountWeapon());
            //            }
            //            else if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.Data.CountWeapon());
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(pd.CountWeapon());
            //            }
            //            else if (it is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(it.CountWeapon());
            //            }
            //            else if (itd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(itd.CountWeapon());
            //            }

            //            break;
            //        }

            //    case "����":
            //        {
            //            idx = (idx + 1);
            //            if (u is object)
            //            {
            //                {
            //                    var withBlock7 = u;
            //                    // ���Ԗڂ̕��킩�𔻒�
            //                    if (GeneralLib.IsNumber(@params[idx]))
            //                    {
            //                        i = Conversions.Toint(@params[idx]);
            //                    }
            //                    else
            //                    {
            //                        var loopTo8 = withBlock7.CountWeapon();
            //                        for (i = 1; i <= loopTo8; i++)
            //                        {
            //                            if ((@params[idx] ?? "") == (withBlock7.Weapon(i).Name ?? ""))
            //                            {
            //                                break;
            //                            }
            //                        }
            //                    }
            //                    // �w�肵������������Ă��Ȃ�
            //                    if (i <= 0 | withBlock7.CountWeapon() < i)
            //                    {
            //                        return EvalInfoFuncRet;
            //                    }

            //                    idx = (idx + 1);
            //                    switch (@params[idx] ?? "")
            //                    {
            //                        case var case1 when case1 == "":
            //                        case "����":
            //                            {
            //                                EvalInfoFuncRet = withBlock7.Weapon(i).Name;
            //                                break;
            //                            }

            //                        case "�U����":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock7.WeaponPower(i, ""));
            //                                break;
            //                            }

            //                        case "�˒�":
            //                        case "�ő�˒�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock7.WeaponMaxRange(i));
            //                                break;
            //                            }

            //                        case "�ŏ��˒�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock7.Weapon(i).MinRange);
            //                                break;
            //                            }

            //                        case "������":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock7.WeaponPrecision(i));
            //                                break;
            //                            }

            //                        case "�ő�e��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock7.MaxBullet(i));
            //                                break;
            //                            }

            //                        case "�e��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock7.Bullet(i));
            //                                break;
            //                            }

            //                        case "����d�m":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock7.WeaponENConsumption(i));
            //                                break;
            //                            }

            //                        case "�K�v�C��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock7.Weapon(i).NecessaryMorale);
            //                                break;
            //                            }

            //                        case "�n�`�K��":
            //                            {
            //                                EvalInfoFuncRet = withBlock7.Weapon(i).Adaption;
            //                                break;
            //                            }

            //                        case "�N���e�B�J����":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock7.WeaponCritical(i));
            //                                break;
            //                            }

            //                        case "����":
            //                            {
            //                                EvalInfoFuncRet = withBlock7.WeaponClass(i);
            //                                break;
            //                            }

            //                        case "�������L":
            //                            {
            //                                if (withBlock7.IsWeaponClassifiedAs(i, @params[idx + 1]))
            //                                {
            //                                    EvalInfoFuncRet = "1";
            //                                }
            //                                else
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }

            //                        case "�������x��":
            //                            {
            //                                EvalInfoFuncRet = withBlock7.WeaponLevel(i, @params[idx + 1]).ToString();
            //                                break;
            //                            }

            //                        case "��������":
            //                            {
            //                                EvalInfoFuncRet = Help.AttributeName(u, @params[idx + 1], false);
            //                                break;
            //                            }

            //                        case "�������":
            //                            {
            //                                EvalInfoFuncRet = Help.AttributeHelpMessage(u, @params[idx + 1], i, false);
            //                                break;
            //                            }

            //                        case "�K�v�Z�\":
            //                            {
            //                                EvalInfoFuncRet = withBlock7.Weapon(i).NecessarySkill;
            //                                break;
            //                            }

            //                        case "�g�p��":
            //                            {
            //                                if (withBlock7.IsWeaponAvailable(i, "�X�e�[�^�X"))
            //                                {
            //                                    EvalInfoFuncRet = "1";
            //                                }
            //                                else
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }

            //                        case "�C��":
            //                            {
            //                                if (withBlock7.IsWeaponMastered(i))
            //                                {
            //                                    EvalInfoFuncRet = "1";
            //                                }
            //                                else
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }
            //                    }
            //                }
            //            }
            //            else if (ud is object)
            //            {
            //                // ���Ԗڂ̕��킩�𔻒�
            //                if (GeneralLib.IsNumber(@params[idx]))
            //                {
            //                    i = Conversions.Toint(@params[idx]);
            //                }
            //                else
            //                {
            //                    var loopTo9 = ud.CountWeapon();
            //                    for (i = 1; i <= loopTo9; i++)
            //                    {
            //                        WeaponData localWeapon() { object argIndex1 = i; var ret = ud.Weapon(argIndex1); return ret; }

            //                        if ((@params[idx] ?? "") == (localWeapon().Name ?? ""))
            //                        {
            //                            break;
            //                        }
            //                    }
            //                }
            //                // �w�肵������������Ă��Ȃ�
            //                if (i <= 0 | ud.CountWeapon() < i)
            //                {
            //                    return EvalInfoFuncRet;
            //                }

            //                idx = (idx + 1);
            //                {
            //                    var withBlock8 = ud.Weapon(i);
            //                    switch (@params[idx] ?? "")
            //                    {
            //                        case var case2 when case2 == "":
            //                        case "����":
            //                            {
            //                                EvalInfoFuncRet = withBlock8.Name;
            //                                break;
            //                            }

            //                        case "�U����":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock8.Power);
            //                                break;
            //                            }

            //                        case "�˒�":
            //                        case "�ő�˒�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock8.MaxRange);
            //                                break;
            //                            }

            //                        case "�ŏ��˒�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock8.MinRange);
            //                                break;
            //                            }

            //                        case "������":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock8.Precision);
            //                                break;
            //                            }

            //                        case "�ő�e��":
            //                        case "�e��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock8.Bullet);
            //                                break;
            //                            }

            //                        case "����d�m":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock8.ENConsumption);
            //                                break;
            //                            }

            //                        case "�K�v�C��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock8.NecessaryMorale);
            //                                break;
            //                            }

            //                        case "�n�`�K��":
            //                            {
            //                                EvalInfoFuncRet = withBlock8.Adaption;
            //                                break;
            //                            }

            //                        case "�N���e�B�J����":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock8.Critical);
            //                                break;
            //                            }

            //                        case "����":
            //                            {
            //                                EvalInfoFuncRet = withBlock8.Class;
            //                                break;
            //                            }

            //                        case "�������L":
            //                            {
            //                                if (GeneralLib.InStrNotNest(withBlock8.Class, @params[idx + 1]) > 0)
            //                                {
            //                                    EvalInfoFuncRet = "1";
            //                                }
            //                                else
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }

            //                        case "�������x��":
            //                            {
            //                                j = GeneralLib.InStrNotNest(withBlock8.Class, @params[idx + 1] + "L");
            //                                if (j == 0)
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                EvalInfoFuncRet = "";
            //                                j = (j + Strings.Len(@params[idx + 1]) + 1);
            //                                do
            //                                {
            //                                    EvalInfoFuncRet = EvalInfoFuncRet + Strings.Mid(withBlock8.Class, j, 1);
            //                                    j = (j + 1);
            //                                }
            //                                while (GeneralLib.IsNumber(Strings.Mid(withBlock8.Class, j, 1)));
            //                                if (!GeneralLib.IsNumber(EvalInfoFuncRet))
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }

            //                        case "�K�v�Z�\":
            //                            {
            //                                EvalInfoFuncRet = withBlock8.NecessarySkill;
            //                                break;
            //                            }

            //                        case "�g�p��":
            //                        case "�C��":
            //                            {
            //                                EvalInfoFuncRet = "1";
            //                                break;
            //                            }
            //                    }
            //                }
            //            }
            //            else if (p is object)
            //            {
            //                {
            //                    var withBlock9 = p.Data;
            //                    // ���Ԗڂ̕��킩�𔻒�
            //                    if (GeneralLib.IsNumber(@params[idx]))
            //                    {
            //                        i = Conversions.Toint(@params[idx]);
            //                    }
            //                    else
            //                    {
            //                        var loopTo10 = withBlock9.CountWeapon();
            //                        for (i = 1; i <= loopTo10; i++)
            //                        {
            //                            WeaponData localWeapon1() { object argIndex1 = i; var ret = withBlock9.Weapon(argIndex1); return ret; }

            //                            if ((@params[idx] ?? "") == (localWeapon1().Name ?? ""))
            //                            {
            //                                break;
            //                            }
            //                        }
            //                    }
            //                    // �w�肵������������Ă��Ȃ�
            //                    if (i <= 0 | withBlock9.CountWeapon() < i)
            //                    {
            //                        return EvalInfoFuncRet;
            //                    }

            //                    idx = (idx + 1);
            //                    {
            //                        var withBlock10 = withBlock9.Weapon(i);
            //                        switch (@params[idx] ?? "")
            //                        {
            //                            case var case3 when case3 == "":
            //                            case "����":
            //                                {
            //                                    EvalInfoFuncRet = withBlock10.Name;
            //                                    break;
            //                                }

            //                            case "�U����":
            //                                {
            //                                    EvalInfoFuncRet = SrcFormatter.Format(withBlock10.Power);
            //                                    break;
            //                                }

            //                            case "�˒�":
            //                            case "�ő�˒�":
            //                                {
            //                                    EvalInfoFuncRet = SrcFormatter.Format(withBlock10.MaxRange);
            //                                    break;
            //                                }

            //                            case "�ŏ��˒�":
            //                                {
            //                                    EvalInfoFuncRet = SrcFormatter.Format(withBlock10.MinRange);
            //                                    break;
            //                                }

            //                            case "������":
            //                                {
            //                                    EvalInfoFuncRet = SrcFormatter.Format(withBlock10.Precision);
            //                                    break;
            //                                }

            //                            case "�ő�e��":
            //                            case "�e��":
            //                                {
            //                                    EvalInfoFuncRet = SrcFormatter.Format(withBlock10.Bullet);
            //                                    break;
            //                                }

            //                            case "����d�m":
            //                                {
            //                                    EvalInfoFuncRet = SrcFormatter.Format(withBlock10.ENConsumption);
            //                                    break;
            //                                }

            //                            case "�K�v�C��":
            //                                {
            //                                    EvalInfoFuncRet = SrcFormatter.Format(withBlock10.NecessaryMorale);
            //                                    break;
            //                                }

            //                            case "�n�`�K��":
            //                                {
            //                                    EvalInfoFuncRet = withBlock10.Adaption;
            //                                    break;
            //                                }

            //                            case "�N���e�B�J����":
            //                                {
            //                                    EvalInfoFuncRet = SrcFormatter.Format(withBlock10.Critical);
            //                                    break;
            //                                }

            //                            case "����":
            //                                {
            //                                    EvalInfoFuncRet = withBlock10.Class;
            //                                    break;
            //                                }

            //                            case "�������L":
            //                                {
            //                                    if (GeneralLib.InStrNotNest(withBlock10.Class, @params[idx + 1]) > 0)
            //                                    {
            //                                        EvalInfoFuncRet = "1";
            //                                    }
            //                                    else
            //                                    {
            //                                        EvalInfoFuncRet = "0";
            //                                    }

            //                                    break;
            //                                }

            //                            case "�������x��":
            //                                {
            //                                    j = GeneralLib.InStrNotNest(withBlock10.Class, @params[idx + 1] + "L");
            //                                    if (j == 0)
            //                                    {
            //                                        EvalInfoFuncRet = "0";
            //                                        return EvalInfoFuncRet;
            //                                    }

            //                                    EvalInfoFuncRet = "";
            //                                    j = (j + Strings.Len(@params[idx + 1]) + 1);
            //                                    do
            //                                    {
            //                                        EvalInfoFuncRet = EvalInfoFuncRet + Strings.Mid(withBlock10.Class, j, 1);
            //                                        j = (j + 1);
            //                                    }
            //                                    while (GeneralLib.IsNumber(Strings.Mid(withBlock10.Class, j, 1)));
            //                                    if (!GeneralLib.IsNumber(EvalInfoFuncRet))
            //                                    {
            //                                        EvalInfoFuncRet = "0";
            //                                    }

            //                                    break;
            //                                }

            //                            case "�K�v�Z�\":
            //                                {
            //                                    EvalInfoFuncRet = withBlock10.NecessarySkill;
            //                                    break;
            //                                }

            //                            case "�g�p��":
            //                            case "�C��":
            //                                {
            //                                    EvalInfoFuncRet = "1";
            //                                    break;
            //                                }
            //                        }
            //                    }
            //                }
            //            }
            //            else if (pd is object)
            //            {
            //                // ���Ԗڂ̕��킩�𔻒�
            //                if (GeneralLib.IsNumber(@params[idx]))
            //                {
            //                    i = Conversions.Toint(@params[idx]);
            //                }
            //                else
            //                {
            //                    var loopTo11 = pd.CountWeapon();
            //                    for (i = 1; i <= loopTo11; i++)
            //                    {
            //                        WeaponData localWeapon2() { object argIndex1 = i; var ret = pd.Weapon(argIndex1); return ret; }

            //                        if ((@params[idx] ?? "") == (localWeapon2().Name ?? ""))
            //                        {
            //                            break;
            //                        }
            //                    }
            //                }
            //                // �w�肵������������Ă��Ȃ�
            //                if (i <= 0 | pd.CountWeapon() < i)
            //                {
            //                    return EvalInfoFuncRet;
            //                }

            //                idx = (idx + 1);
            //                {
            //                    var withBlock11 = pd.Weapon(i);
            //                    switch (@params[idx] ?? "")
            //                    {
            //                        case var case4 when case4 == "":
            //                        case "����":
            //                            {
            //                                EvalInfoFuncRet = withBlock11.Name;
            //                                break;
            //                            }

            //                        case "�U����":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock11.Power);
            //                                break;
            //                            }

            //                        case "�˒�":
            //                        case "�ő�˒�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock11.MaxRange);
            //                                break;
            //                            }

            //                        case "�ŏ��˒�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock11.MinRange);
            //                                break;
            //                            }

            //                        case "������":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock11.Precision);
            //                                break;
            //                            }

            //                        case "�ő�e��":
            //                        case "�e��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock11.Bullet);
            //                                break;
            //                            }

            //                        case "����d�m":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock11.ENConsumption);
            //                                break;
            //                            }

            //                        case "�K�v�C��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock11.NecessaryMorale);
            //                                break;
            //                            }

            //                        case "�n�`�K��":
            //                            {
            //                                EvalInfoFuncRet = withBlock11.Adaption;
            //                                break;
            //                            }

            //                        case "�N���e�B�J����":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock11.Critical);
            //                                break;
            //                            }

            //                        case "����":
            //                            {
            //                                EvalInfoFuncRet = withBlock11.Class;
            //                                break;
            //                            }

            //                        case "�������L":
            //                            {
            //                                if (GeneralLib.InStrNotNest(withBlock11.Class, @params[idx + 1]) > 0)
            //                                {
            //                                    EvalInfoFuncRet = "1";
            //                                }
            //                                else
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }

            //                        case "�������x��":
            //                            {
            //                                j = GeneralLib.InStrNotNest(withBlock11.Class, @params[idx + 1] + "L");
            //                                if (j == 0)
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                EvalInfoFuncRet = "";
            //                                j = (j + Strings.Len(@params[idx + 1]) + 1);
            //                                do
            //                                {
            //                                    EvalInfoFuncRet = EvalInfoFuncRet + Strings.Mid(withBlock11.Class, j, 1);
            //                                    j = (j + 1);
            //                                }
            //                                while (GeneralLib.IsNumber(Strings.Mid(withBlock11.Class, j, 1)));
            //                                if (!GeneralLib.IsNumber(EvalInfoFuncRet))
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }

            //                        case "�K�v�Z�\":
            //                            {
            //                                EvalInfoFuncRet = withBlock11.NecessarySkill;
            //                                break;
            //                            }

            //                        case "�g�p��":
            //                        case "�C��":
            //                            {
            //                                EvalInfoFuncRet = "1";
            //                                break;
            //                            }
            //                    }
            //                }
            //            }
            //            else if (it is object)
            //            {
            //                // ���Ԗڂ̕��킩�𔻒�
            //                if (GeneralLib.IsNumber(@params[idx]))
            //                {
            //                    i = Conversions.Toint(@params[idx]);
            //                }
            //                else
            //                {
            //                    var loopTo12 = it.CountWeapon();
            //                    for (i = 1; i <= loopTo12; i++)
            //                    {
            //                        WeaponData localWeapon3() { object argIndex1 = i; var ret = it.Weapon(argIndex1); return ret; }

            //                        if ((@params[idx] ?? "") == (localWeapon3().Name ?? ""))
            //                        {
            //                            break;
            //                        }
            //                    }
            //                }
            //                // �w�肵������������Ă��Ȃ�
            //                if (i <= 0 | it.CountWeapon() < i)
            //                {
            //                    return EvalInfoFuncRet;
            //                }

            //                idx = (idx + 1);
            //                {
            //                    var withBlock12 = it.Weapon(i);
            //                    switch (@params[idx] ?? "")
            //                    {
            //                        case var case5 when case5 == "":
            //                        case "����":
            //                            {
            //                                EvalInfoFuncRet = withBlock12.Name;
            //                                break;
            //                            }

            //                        case "�U����":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock12.Power);
            //                                break;
            //                            }

            //                        case "�˒�":
            //                        case "�ő�˒�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock12.MaxRange);
            //                                break;
            //                            }

            //                        case "�ŏ��˒�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock12.MinRange);
            //                                break;
            //                            }

            //                        case "������":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock12.Precision);
            //                                break;
            //                            }

            //                        case "�ő�e��":
            //                        case "�e��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock12.Bullet);
            //                                break;
            //                            }

            //                        case "����d�m":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock12.ENConsumption);
            //                                break;
            //                            }

            //                        case "�K�v�C��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock12.NecessaryMorale);
            //                                break;
            //                            }

            //                        case "�n�`�K��":
            //                            {
            //                                EvalInfoFuncRet = withBlock12.Adaption;
            //                                break;
            //                            }

            //                        case "�N���e�B�J����":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock12.Critical);
            //                                break;
            //                            }

            //                        case "����":
            //                            {
            //                                EvalInfoFuncRet = withBlock12.Class;
            //                                break;
            //                            }

            //                        case "�������L":
            //                            {
            //                                if (GeneralLib.InStrNotNest(withBlock12.Class, @params[idx + 1]) > 0)
            //                                {
            //                                    EvalInfoFuncRet = "1";
            //                                }
            //                                else
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }

            //                        case "�������x��":
            //                            {
            //                                j = GeneralLib.InStrNotNest(withBlock12.Class, @params[idx + 1] + "L");
            //                                if (j == 0)
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                EvalInfoFuncRet = "";
            //                                j = (j + Strings.Len(@params[idx + 1]) + 1);
            //                                do
            //                                {
            //                                    EvalInfoFuncRet = EvalInfoFuncRet + Strings.Mid(withBlock12.Class, j, 1);
            //                                    j = (j + 1);
            //                                }
            //                                while (GeneralLib.IsNumber(Strings.Mid(withBlock12.Class, j, 1)));
            //                                if (!GeneralLib.IsNumber(EvalInfoFuncRet))
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }

            //                        case "�K�v�Z�\":
            //                            {
            //                                EvalInfoFuncRet = withBlock12.NecessarySkill;
            //                                break;
            //                            }

            //                        case "�g�p��":
            //                        case "�C��":
            //                            {
            //                                EvalInfoFuncRet = "1";
            //                                break;
            //                            }
            //                    }
            //                }
            //            }
            //            else if (itd is object)
            //            {
            //                // ���Ԗڂ̕��킩�𔻒�
            //                if (GeneralLib.IsNumber(@params[idx]))
            //                {
            //                    i = Conversions.Toint(@params[idx]);
            //                }
            //                else
            //                {
            //                    var loopTo13 = itd.CountWeapon();
            //                    for (i = 1; i <= loopTo13; i++)
            //                    {
            //                        WeaponData localWeapon4() { object argIndex1 = i; var ret = itd.Weapon(argIndex1); return ret; }

            //                        if ((@params[idx] ?? "") == (localWeapon4().Name ?? ""))
            //                        {
            //                            break;
            //                        }
            //                    }
            //                }
            //                // �w�肵������������Ă��Ȃ�
            //                if (i <= 0 | itd.CountWeapon() < i)
            //                {
            //                    return EvalInfoFuncRet;
            //                }

            //                idx = (idx + 1);
            //                {
            //                    var withBlock13 = itd.Weapon(i);
            //                    switch (@params[idx] ?? "")
            //                    {
            //                        case var case6 when case6 == "":
            //                        case "����":
            //                            {
            //                                EvalInfoFuncRet = withBlock13.Name;
            //                                break;
            //                            }

            //                        case "�U����":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock13.Power);
            //                                break;
            //                            }

            //                        case "�˒�":
            //                        case "�ő�˒�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock13.MaxRange);
            //                                break;
            //                            }

            //                        case "�ŏ��˒�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock13.MinRange);
            //                                break;
            //                            }

            //                        case "������":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock13.Precision);
            //                                break;
            //                            }

            //                        case "�ő�e��":
            //                        case "�e��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock13.Bullet);
            //                                break;
            //                            }

            //                        case "����d�m":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock13.ENConsumption);
            //                                break;
            //                            }

            //                        case "�K�v�C��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock13.NecessaryMorale);
            //                                break;
            //                            }

            //                        case "�n�`�K��":
            //                            {
            //                                EvalInfoFuncRet = withBlock13.Adaption;
            //                                break;
            //                            }

            //                        case "�N���e�B�J����":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock13.Critical);
            //                                break;
            //                            }

            //                        case "����":
            //                            {
            //                                EvalInfoFuncRet = withBlock13.Class;
            //                                break;
            //                            }

            //                        case "�������L":
            //                            {
            //                                if (GeneralLib.InStrNotNest(withBlock13.Class, @params[idx + 1]) > 0)
            //                                {
            //                                    EvalInfoFuncRet = "1";
            //                                }
            //                                else
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }

            //                        case "�������x��":
            //                            {
            //                                j = GeneralLib.InStrNotNest(withBlock13.Class, @params[idx + 1] + "L");
            //                                if (j == 0)
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                EvalInfoFuncRet = "";
            //                                j = (j + Strings.Len(@params[idx + 1]) + 1);
            //                                do
            //                                {
            //                                    EvalInfoFuncRet = EvalInfoFuncRet + Strings.Mid(withBlock13.Class, j, 1);
            //                                    j = (j + 1);
            //                                }
            //                                while (GeneralLib.IsNumber(Strings.Mid(withBlock13.Class, j, 1)));
            //                                if (!GeneralLib.IsNumber(EvalInfoFuncRet))
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }

            //                        case "�K�v�Z�\":
            //                            {
            //                                EvalInfoFuncRet = withBlock13.NecessarySkill;
            //                                break;
            //                            }

            //                        case "�g�p��":
            //                        case "�C��":
            //                            {
            //                                EvalInfoFuncRet = "1";
            //                                break;
            //                            }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "�A�r���e�B��":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(u.CountAbility());
            //            }
            //            else if (ud is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(ud.CountAbility());
            //            }
            //            else if (p is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(p.Data.CountAbility());
            //            }
            //            else if (pd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(pd.CountAbility());
            //            }
            //            else if (it is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(it.CountAbility());
            //            }
            //            else if (itd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(itd.CountAbility());
            //            }

            //            break;
            //        }

            //    case "�A�r���e�B":
            //        {
            //            idx = (idx + 1);
            //            if (u is object)
            //            {
            //                {
            //                    var withBlock14 = u;
            //                    // ���Ԗڂ̃A�r���e�B���𔻒�
            //                    if (GeneralLib.IsNumber(@params[idx]))
            //                    {
            //                        i = Conversions.Toint(@params[idx]);
            //                    }
            //                    else
            //                    {
            //                        var loopTo14 = withBlock14.CountAbility();
            //                        for (i = 1; i <= loopTo14; i++)
            //                        {
            //                            if ((@params[idx] ?? "") == (withBlock14.Ability(i).Name ?? ""))
            //                            {
            //                                break;
            //                            }
            //                        }
            //                    }
            //                    // �w�肵���A�r���e�B�������Ă��Ȃ�
            //                    if (i <= 0 | withBlock14.CountAbility() < i)
            //                    {
            //                        return EvalInfoFuncRet;
            //                    }

            //                    idx = (idx + 1);
            //                    switch (@params[idx] ?? "")
            //                    {
            //                        case var case7 when case7 == "":
            //                        case "����":
            //                            {
            //                                EvalInfoFuncRet = withBlock14.Ability(i).Name;
            //                                break;
            //                            }

            //                        case "���ʐ�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock14.Ability(i).CountEffect());
            //                                break;
            //                            }

            //                        case "���ʃ^�C�v":
            //                            {
            //                                // ���Ԗڂ̌��ʂ��𔻒�
            //                                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                                {
            //                                    j = Conversions.Toint(@params[idx + 1]);
            //                                }

            //                                if (j <= 0 & withBlock14.Ability(i).CountEffect() < j)
            //                                {
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                EvalInfoFuncRet = withBlock14.Ability(i).EffectType(j);
            //                                break;
            //                            }

            //                        case "���ʃ��x��":
            //                            {
            //                                // ���Ԗڂ̌��ʂ��𔻒�
            //                                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                                {
            //                                    j = Conversions.Toint(@params[idx + 1]);
            //                                }

            //                                if (j <= 0 & withBlock14.Ability(i).CountEffect() < j)
            //                                {
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                double localEffectLevel() { object argIndex1 = j; var ret = withBlock14.Ability(i).EffectLevel(argIndex1); return ret; }

            //                                EvalInfoFuncRet = SrcFormatter.Format(localEffectLevel());
            //                                break;
            //                            }

            //                        case "���ʃf�[�^":
            //                            {
            //                                // ���Ԗڂ̌��ʂ��𔻒�
            //                                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                                {
            //                                    j = Conversions.Toint(@params[idx + 1]);
            //                                }

            //                                if (j <= 0 & withBlock14.Ability(i).CountEffect() < j)
            //                                {
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                EvalInfoFuncRet = withBlock14.Ability(i).EffectData(j);
            //                                break;
            //                            }

            //                        case "�˒�":
            //                        case "�ő�˒�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock14.AbilityMaxRange(i));
            //                                break;
            //                            }

            //                        case "�ŏ��˒�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock14.AbilityMinRange(i));
            //                                break;
            //                            }

            //                        case "�ő�g�p��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock14.MaxStock(i));
            //                                break;
            //                            }

            //                        case "�g�p��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock14.Stock(i));
            //                                break;
            //                            }

            //                        case "����d�m":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock14.AbilityENConsumption(i));
            //                                break;
            //                            }

            //                        case "�K�v�C��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock14.Ability(i).NecessaryMorale);
            //                                break;
            //                            }

            //                        case "����":
            //                            {
            //                                EvalInfoFuncRet = withBlock14.Ability(i).Class;
            //                                break;
            //                            }

            //                        case "�������L":
            //                            {
            //                                if (withBlock14.IsAbilityClassifiedAs(i, @params[idx + 1]))
            //                                {
            //                                    EvalInfoFuncRet = "1";
            //                                }
            //                                else
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }

            //                        case "�������x��":
            //                            {
            //                                EvalInfoFuncRet = withBlock14.AbilityLevel(i, @params[idx + 1]).ToString();
            //                                break;
            //                            }

            //                        case "��������":
            //                            {
            //                                EvalInfoFuncRet = Help.AttributeName(u, @params[idx + 1], true);
            //                                break;
            //                            }

            //                        case "�������":
            //                            {
            //                                EvalInfoFuncRet = Help.AttributeHelpMessage(u, @params[idx + 1], i, true);
            //                                break;
            //                            }

            //                        case "�K�v�Z�\":
            //                            {
            //                                EvalInfoFuncRet = withBlock14.Ability(i).NecessarySkill;
            //                                break;
            //                            }

            //                        case "�g�p��":
            //                            {
            //                                if (withBlock14.IsAbilityAvailable(i, "�ړ��O"))
            //                                {
            //                                    EvalInfoFuncRet = "1";
            //                                }
            //                                else
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }

            //                        case "�C��":
            //                            {
            //                                if (withBlock14.IsAbilityMastered(i))
            //                                {
            //                                    EvalInfoFuncRet = "1";
            //                                }
            //                                else
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }
            //                    }
            //                }
            //            }
            //            else if (ud is object)
            //            {
            //                // ���Ԗڂ̃A�r���e�B���𔻒�
            //                if (GeneralLib.IsNumber(@params[idx]))
            //                {
            //                    i = Conversions.Toint(@params[idx]);
            //                }
            //                else
            //                {
            //                    var loopTo15 = ud.CountAbility();
            //                    for (i = 1; i <= loopTo15; i++)
            //                    {
            //                        AbilityData localAbility() { object argIndex1 = i; var ret = ud.Ability(argIndex1); return ret; }

            //                        if ((@params[idx] ?? "") == (localAbility().Name ?? ""))
            //                        {
            //                            break;
            //                        }
            //                    }
            //                }
            //                // �w�肵���A�r���e�B�������Ă��Ȃ�
            //                if (i <= 0 | ud.CountAbility() < i)
            //                {
            //                    return EvalInfoFuncRet;
            //                }

            //                idx = (idx + 1);
            //                {
            //                    var withBlock15 = ud.Ability(i);
            //                    switch (@params[idx] ?? "")
            //                    {
            //                        case var case8 when case8 == "":
            //                        case "����":
            //                            {
            //                                EvalInfoFuncRet = withBlock15.Name;
            //                                break;
            //                            }

            //                        case "���ʐ�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock15.CountEffect());
            //                                break;
            //                            }

            //                        case "���ʃ^�C�v":
            //                            {
            //                                // ���Ԗڂ̌��ʂ��𔻒�
            //                                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                                {
            //                                    j = Conversions.Toint(@params[idx + 1]);
            //                                }

            //                                if (j <= 0 | withBlock15.CountEffect() < j)
            //                                {
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                EvalInfoFuncRet = withBlock15.EffectType(j);
            //                                break;
            //                            }

            //                        case "���ʃ��x��":
            //                            {
            //                                // ���Ԗڂ̌��ʂ��𔻒�
            //                                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                                {
            //                                    j = Conversions.Toint(@params[idx + 1]);
            //                                }

            //                                if (j <= 0 | withBlock15.CountEffect() < j)
            //                                {
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                double localEffectLevel1() { object argIndex1 = j; var ret = withBlock15.EffectLevel(argIndex1); return ret; }

            //                                EvalInfoFuncRet = SrcFormatter.Format(localEffectLevel1());
            //                                break;
            //                            }

            //                        case "���ʃf�[�^":
            //                            {
            //                                // ���Ԗڂ̌��ʂ��𔻒�
            //                                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                                {
            //                                    j = Conversions.Toint(@params[idx + 1]);
            //                                }

            //                                if (j <= 0 | withBlock15.CountEffect() < j)
            //                                {
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                EvalInfoFuncRet = withBlock15.EffectData(j);
            //                                break;
            //                            }

            //                        case "�˒�":
            //                        case "�ő�˒�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock15.MaxRange);
            //                                break;
            //                            }

            //                        case "�ŏ��˒�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock15.MinRange);
            //                                break;
            //                            }

            //                        case "�ő�g�p��":
            //                        case "�g�p��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock15.Stock);
            //                                break;
            //                            }

            //                        case "����d�m":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock15.ENConsumption);
            //                                break;
            //                            }

            //                        case "�K�v�C��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock15.NecessaryMorale);
            //                                break;
            //                            }

            //                        case "����":
            //                            {
            //                                EvalInfoFuncRet = withBlock15.Class;
            //                                break;
            //                            }

            //                        case "�������L":
            //                            {
            //                                if (GeneralLib.InStrNotNest(withBlock15.Class, @params[idx + 1]) > 0)
            //                                {
            //                                    EvalInfoFuncRet = "1";
            //                                }
            //                                else
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }

            //                        case "�������x��":
            //                            {
            //                                j = GeneralLib.InStrNotNest(withBlock15.Class, @params[idx + 1] + "L");
            //                                if (j == 0)
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                EvalInfoFuncRet = "";
            //                                j = (j + Strings.Len(@params[idx + 1]) + 1);
            //                                do
            //                                {
            //                                    EvalInfoFuncRet = EvalInfoFuncRet + Strings.Mid(withBlock15.Class, j, 1);
            //                                    j = (j + 1);
            //                                }
            //                                while (GeneralLib.IsNumber(Strings.Mid(withBlock15.Class, j, 1)));
            //                                if (!GeneralLib.IsNumber(EvalInfoFuncRet))
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }

            //                        case "�K�v�Z�\":
            //                            {
            //                                EvalInfoFuncRet = withBlock15.NecessarySkill;
            //                                break;
            //                            }

            //                        case "�g�p��":
            //                        case "�C��":
            //                            {
            //                                EvalInfoFuncRet = "1";
            //                                break;
            //                            }
            //                    }
            //                }
            //            }
            //            else if (p is object)
            //            {
            //                {
            //                    var withBlock16 = p.Data;
            //                    // ���Ԗڂ̃A�r���e�B���𔻒�
            //                    if (GeneralLib.IsNumber(@params[idx]))
            //                    {
            //                        i = Conversions.Toint(@params[idx]);
            //                    }
            //                    else
            //                    {
            //                        var loopTo16 = withBlock16.CountAbility();
            //                        for (i = 1; i <= loopTo16; i++)
            //                        {
            //                            AbilityData localAbility1() { object argIndex1 = i; var ret = withBlock16.Ability(argIndex1); return ret; }

            //                            if ((@params[idx] ?? "") == (localAbility1().Name ?? ""))
            //                            {
            //                                break;
            //                            }
            //                        }
            //                    }
            //                    // �w�肵���A�r���e�B�������Ă��Ȃ�
            //                    if (i <= 0 | withBlock16.CountAbility() < i)
            //                    {
            //                        return EvalInfoFuncRet;
            //                    }

            //                    idx = (idx + 1);
            //                    {
            //                        var withBlock17 = withBlock16.Ability(i);
            //                        switch (@params[idx] ?? "")
            //                        {
            //                            case var case9 when case9 == "":
            //                            case "����":
            //                                {
            //                                    EvalInfoFuncRet = withBlock17.Name;
            //                                    break;
            //                                }

            //                            case "���ʐ�":
            //                                {
            //                                    EvalInfoFuncRet = SrcFormatter.Format(withBlock17.CountEffect());
            //                                    break;
            //                                }

            //                            case "���ʃ^�C�v":
            //                                {
            //                                    // ���Ԗڂ̌��ʂ��𔻒�
            //                                    if (GeneralLib.IsNumber(@params[idx + 1]))
            //                                    {
            //                                        j = Conversions.Toint(@params[idx + 1]);
            //                                    }

            //                                    if (j <= 0 | withBlock17.CountEffect() < j)
            //                                    {
            //                                        return EvalInfoFuncRet;
            //                                    }

            //                                    EvalInfoFuncRet = withBlock17.EffectType(j);
            //                                    break;
            //                                }

            //                            case "���ʃ��x��":
            //                                {
            //                                    // ���Ԗڂ̌��ʂ��𔻒�
            //                                    if (GeneralLib.IsNumber(@params[idx + 1]))
            //                                    {
            //                                        j = Conversions.Toint(@params[idx + 1]);
            //                                    }

            //                                    if (j <= 0 | withBlock17.CountEffect() < j)
            //                                    {
            //                                        return EvalInfoFuncRet;
            //                                    }

            //                                    double localEffectLevel2() { object argIndex1 = j; var ret = withBlock17.EffectLevel(argIndex1); return ret; }

            //                                    EvalInfoFuncRet = SrcFormatter.Format(localEffectLevel2());
            //                                    break;
            //                                }

            //                            case "���ʃf�[�^":
            //                                {
            //                                    // ���Ԗڂ̌��ʂ��𔻒�
            //                                    if (GeneralLib.IsNumber(@params[idx + 1]))
            //                                    {
            //                                        j = Conversions.Toint(@params[idx + 1]);
            //                                    }

            //                                    if (j <= 0 | withBlock17.CountEffect() < j)
            //                                    {
            //                                        return EvalInfoFuncRet;
            //                                    }

            //                                    EvalInfoFuncRet = withBlock17.EffectData(j);
            //                                    break;
            //                                }

            //                            case "�˒�":
            //                            case "�ő�˒�":
            //                                {
            //                                    EvalInfoFuncRet = SrcFormatter.Format(withBlock17.MaxRange);
            //                                    break;
            //                                }

            //                            case "�ŏ��˒�":
            //                                {
            //                                    EvalInfoFuncRet = SrcFormatter.Format(withBlock17.MinRange);
            //                                    break;
            //                                }

            //                            case "�ő�g�p��":
            //                            case "�g�p��":
            //                                {
            //                                    EvalInfoFuncRet = SrcFormatter.Format(withBlock17.Stock);
            //                                    break;
            //                                }

            //                            case "����d�m":
            //                                {
            //                                    EvalInfoFuncRet = SrcFormatter.Format(withBlock17.ENConsumption);
            //                                    break;
            //                                }

            //                            case "�K�v�C��":
            //                                {
            //                                    EvalInfoFuncRet = SrcFormatter.Format(withBlock17.NecessaryMorale);
            //                                    break;
            //                                }

            //                            case "����":
            //                                {
            //                                    EvalInfoFuncRet = withBlock17.Class;
            //                                    break;
            //                                }

            //                            case "�������L":
            //                                {
            //                                    if (GeneralLib.InStrNotNest(withBlock17.Class, @params[idx + 1]) > 0)
            //                                    {
            //                                        EvalInfoFuncRet = "1";
            //                                    }
            //                                    else
            //                                    {
            //                                        EvalInfoFuncRet = "0";
            //                                    }

            //                                    break;
            //                                }

            //                            case "�������x��":
            //                                {
            //                                    j = GeneralLib.InStrNotNest(withBlock17.Class, @params[idx + 1] + "L");
            //                                    if (j == 0)
            //                                    {
            //                                        EvalInfoFuncRet = "0";
            //                                        return EvalInfoFuncRet;
            //                                    }

            //                                    EvalInfoFuncRet = "";
            //                                    j = (j + Strings.Len(@params[idx + 1]) + 1);
            //                                    do
            //                                    {
            //                                        EvalInfoFuncRet = EvalInfoFuncRet + Strings.Mid(withBlock17.Class, j, 1);
            //                                        j = (j + 1);
            //                                    }
            //                                    while (GeneralLib.IsNumber(Strings.Mid(withBlock17.Class, j, 1)));
            //                                    if (!GeneralLib.IsNumber(EvalInfoFuncRet))
            //                                    {
            //                                        EvalInfoFuncRet = "0";
            //                                    }

            //                                    break;
            //                                }

            //                            case "�K�v�Z�\":
            //                                {
            //                                    EvalInfoFuncRet = withBlock17.NecessarySkill;
            //                                    break;
            //                                }

            //                            case "�g�p��":
            //                            case "�C��":
            //                                {
            //                                    EvalInfoFuncRet = "1";
            //                                    break;
            //                                }
            //                        }
            //                    }
            //                }
            //            }
            //            else if (pd is object)
            //            {
            //                // ���Ԗڂ̃A�r���e�B���𔻒�
            //                if (GeneralLib.IsNumber(@params[idx]))
            //                {
            //                    i = Conversions.Toint(@params[idx]);
            //                }
            //                else
            //                {
            //                    var loopTo17 = pd.CountAbility();
            //                    for (i = 1; i <= loopTo17; i++)
            //                    {
            //                        AbilityData localAbility2() { object argIndex1 = i; var ret = pd.Ability(argIndex1); return ret; }

            //                        if ((@params[idx] ?? "") == (localAbility2().Name ?? ""))
            //                        {
            //                            break;
            //                        }
            //                    }
            //                }
            //                // �w�肵���A�r���e�B�������Ă��Ȃ�
            //                if (i <= 0 | pd.CountAbility() < i)
            //                {
            //                    return EvalInfoFuncRet;
            //                }

            //                idx = (idx + 1);
            //                {
            //                    var withBlock18 = pd.Ability(i);
            //                    switch (@params[idx] ?? "")
            //                    {
            //                        case var case10 when case10 == "":
            //                        case "����":
            //                            {
            //                                EvalInfoFuncRet = withBlock18.Name;
            //                                break;
            //                            }

            //                        case "���ʐ�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock18.CountEffect());
            //                                break;
            //                            }

            //                        case "���ʃ^�C�v":
            //                            {
            //                                // ���Ԗڂ̌��ʂ��𔻒�
            //                                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                                {
            //                                    j = Conversions.Toint(@params[idx + 1]);
            //                                }

            //                                if (j <= 0 | withBlock18.CountEffect() < j)
            //                                {
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                EvalInfoFuncRet = withBlock18.EffectType(j);
            //                                break;
            //                            }

            //                        case "���ʃ��x��":
            //                            {
            //                                // ���Ԗڂ̌��ʂ��𔻒�
            //                                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                                {
            //                                    j = Conversions.Toint(@params[idx + 1]);
            //                                }

            //                                if (j <= 0 | withBlock18.CountEffect() < j)
            //                                {
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                double localEffectLevel3() { object argIndex1 = j; var ret = withBlock18.EffectLevel(argIndex1); return ret; }

            //                                EvalInfoFuncRet = SrcFormatter.Format(localEffectLevel3());
            //                                break;
            //                            }

            //                        case "���ʃf�[�^":
            //                            {
            //                                // ���Ԗڂ̌��ʂ��𔻒�
            //                                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                                {
            //                                    j = Conversions.Toint(@params[idx + 1]);
            //                                }

            //                                if (j <= 0 | withBlock18.CountEffect() < j)
            //                                {
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                EvalInfoFuncRet = withBlock18.EffectData(j);
            //                                break;
            //                            }

            //                        case "�˒�":
            //                        case "�ő�˒�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock18.MaxRange);
            //                                break;
            //                            }

            //                        case "�ŏ��˒�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock18.MinRange);
            //                                break;
            //                            }

            //                        case "�ő�g�p��":
            //                        case "�g�p��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock18.Stock);
            //                                break;
            //                            }

            //                        case "����d�m":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock18.ENConsumption);
            //                                break;
            //                            }

            //                        case "�K�v�C��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock18.NecessaryMorale);
            //                                break;
            //                            }

            //                        case "����":
            //                            {
            //                                EvalInfoFuncRet = withBlock18.Class;
            //                                break;
            //                            }

            //                        case "�������L":
            //                            {
            //                                if (GeneralLib.InStrNotNest(withBlock18.Class, @params[idx + 1]) > 0)
            //                                {
            //                                    EvalInfoFuncRet = "1";
            //                                }
            //                                else
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }

            //                        case "�������x��":
            //                            {
            //                                j = GeneralLib.InStrNotNest(withBlock18.Class, @params[idx + 1] + "L");
            //                                if (j == 0)
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                EvalInfoFuncRet = "";
            //                                j = (j + Strings.Len(@params[idx + 1]) + 1);
            //                                do
            //                                {
            //                                    EvalInfoFuncRet = EvalInfoFuncRet + Strings.Mid(withBlock18.Class, j, 1);
            //                                    j = (j + 1);
            //                                }
            //                                while (GeneralLib.IsNumber(Strings.Mid(withBlock18.Class, j, 1)));
            //                                if (!GeneralLib.IsNumber(EvalInfoFuncRet))
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }

            //                        case "�K�v�Z�\":
            //                            {
            //                                EvalInfoFuncRet = withBlock18.NecessarySkill;
            //                                break;
            //                            }

            //                        case "�g�p��":
            //                        case "�C��":
            //                            {
            //                                EvalInfoFuncRet = "1";
            //                                break;
            //                            }
            //                    }
            //                }
            //            }
            //            else if (it is object)
            //            {
            //                // ���Ԗڂ̃A�r���e�B���𔻒�
            //                if (GeneralLib.IsNumber(@params[idx]))
            //                {
            //                    i = Conversions.Toint(@params[idx]);
            //                }
            //                else
            //                {
            //                    var loopTo18 = it.CountAbility();
            //                    for (i = 1; i <= loopTo18; i++)
            //                    {
            //                        AbilityData localAbility3() { object argIndex1 = i; var ret = it.Ability(argIndex1); return ret; }

            //                        if ((@params[idx] ?? "") == (localAbility3().Name ?? ""))
            //                        {
            //                            break;
            //                        }
            //                    }
            //                }
            //                // �w�肵���A�r���e�B�������Ă��Ȃ�
            //                if (i <= 0 | it.CountAbility() < i)
            //                {
            //                    return EvalInfoFuncRet;
            //                }

            //                idx = (idx + 1);
            //                {
            //                    var withBlock19 = it.Ability(i);
            //                    switch (@params[idx] ?? "")
            //                    {
            //                        case var case11 when case11 == "":
            //                        case "����":
            //                            {
            //                                EvalInfoFuncRet = withBlock19.Name;
            //                                break;
            //                            }

            //                        case "���ʐ�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock19.CountEffect());
            //                                break;
            //                            }

            //                        case "���ʃ^�C�v":
            //                            {
            //                                // ���Ԗڂ̌��ʂ��𔻒�
            //                                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                                {
            //                                    j = Conversions.Toint(@params[idx + 1]);
            //                                }

            //                                if (j <= 0 | withBlock19.CountEffect() < j)
            //                                {
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                EvalInfoFuncRet = withBlock19.EffectType(j);
            //                                break;
            //                            }

            //                        case "���ʃ��x��":
            //                            {
            //                                // ���Ԗڂ̌��ʂ��𔻒�
            //                                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                                {
            //                                    j = Conversions.Toint(@params[idx + 1]);
            //                                }

            //                                if (j <= 0 | withBlock19.CountEffect() < j)
            //                                {
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                double localEffectLevel4() { object argIndex1 = j; var ret = withBlock19.EffectLevel(argIndex1); return ret; }

            //                                EvalInfoFuncRet = SrcFormatter.Format(localEffectLevel4());
            //                                break;
            //                            }

            //                        case "���ʃf�[�^":
            //                            {
            //                                // ���Ԗڂ̌��ʂ��𔻒�
            //                                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                                {
            //                                    j = Conversions.Toint(@params[idx + 1]);
            //                                }

            //                                if (j <= 0 | withBlock19.CountEffect() < j)
            //                                {
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                EvalInfoFuncRet = withBlock19.EffectData(j);
            //                                break;
            //                            }

            //                        case "�˒�":
            //                        case "�ő�˒�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock19.MaxRange);
            //                                break;
            //                            }

            //                        case "�ŏ��˒�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock19.MinRange);
            //                                break;
            //                            }

            //                        case "�ő�g�p��":
            //                        case "�g�p��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock19.Stock);
            //                                break;
            //                            }

            //                        case "����d�m":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock19.ENConsumption);
            //                                break;
            //                            }

            //                        case "�K�v�C��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock19.NecessaryMorale);
            //                                break;
            //                            }

            //                        case "����":
            //                            {
            //                                EvalInfoFuncRet = withBlock19.Class;
            //                                break;
            //                            }

            //                        case "�������L":
            //                            {
            //                                if (GeneralLib.InStrNotNest(withBlock19.Class, @params[idx + 1]) > 0)
            //                                {
            //                                    EvalInfoFuncRet = "1";
            //                                }
            //                                else
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }

            //                        case "�������x��":
            //                            {
            //                                j = GeneralLib.InStrNotNest(withBlock19.Class, @params[idx + 1] + "L");
            //                                if (j == 0)
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                EvalInfoFuncRet = "";
            //                                j = (j + Strings.Len(@params[idx + 1]) + 1);
            //                                do
            //                                {
            //                                    EvalInfoFuncRet = EvalInfoFuncRet + Strings.Mid(withBlock19.Class, j, 1);
            //                                    j = (j + 1);
            //                                }
            //                                while (GeneralLib.IsNumber(Strings.Mid(withBlock19.Class, j, 1)));
            //                                if (!GeneralLib.IsNumber(EvalInfoFuncRet))
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }

            //                        case "�K�v�Z�\":
            //                            {
            //                                EvalInfoFuncRet = withBlock19.NecessarySkill;
            //                                break;
            //                            }

            //                        case "�g�p��":
            //                        case "�C��":
            //                            {
            //                                EvalInfoFuncRet = "1";
            //                                break;
            //                            }
            //                    }
            //                }
            //            }
            //            else if (itd is object)
            //            {
            //                // ���Ԗڂ̃A�r���e�B���𔻒�
            //                if (GeneralLib.IsNumber(@params[idx]))
            //                {
            //                    i = Conversions.Toint(@params[idx]);
            //                }
            //                else
            //                {
            //                    var loopTo19 = itd.CountAbility();
            //                    for (i = 1; i <= loopTo19; i++)
            //                    {
            //                        AbilityData localAbility4() { object argIndex1 = i; var ret = itd.Ability(argIndex1); return ret; }

            //                        if ((@params[idx] ?? "") == (localAbility4().Name ?? ""))
            //                        {
            //                            break;
            //                        }
            //                    }
            //                }
            //                // �w�肵���A�r���e�B�������Ă��Ȃ�
            //                if (i <= 0 | itd.CountAbility() < i)
            //                {
            //                    return EvalInfoFuncRet;
            //                }

            //                idx = (idx + 1);
            //                {
            //                    var withBlock20 = itd.Ability(i);
            //                    switch (@params[idx] ?? "")
            //                    {
            //                        case var case12 when case12 == "":
            //                        case "����":
            //                            {
            //                                EvalInfoFuncRet = withBlock20.Name;
            //                                break;
            //                            }

            //                        case "���ʐ�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock20.CountEffect());
            //                                break;
            //                            }

            //                        case "���ʃ^�C�v":
            //                            {
            //                                // ���Ԗڂ̌��ʂ��𔻒�
            //                                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                                {
            //                                    j = Conversions.Toint(@params[idx + 1]);
            //                                }

            //                                if (j <= 0 | withBlock20.CountEffect() < j)
            //                                {
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                EvalInfoFuncRet = withBlock20.EffectType(j);
            //                                break;
            //                            }

            //                        case "���ʃ��x��":
            //                            {
            //                                // ���Ԗڂ̌��ʂ��𔻒�
            //                                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                                {
            //                                    j = Conversions.Toint(@params[idx + 1]);
            //                                }

            //                                if (j <= 0 | withBlock20.CountEffect() < j)
            //                                {
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                double localEffectLevel5() { object argIndex1 = j; var ret = withBlock20.EffectLevel(argIndex1); return ret; }

            //                                EvalInfoFuncRet = SrcFormatter.Format(localEffectLevel5());
            //                                break;
            //                            }

            //                        case "���ʃf�[�^":
            //                            {
            //                                // ���Ԗڂ̌��ʂ��𔻒�
            //                                if (GeneralLib.IsNumber(@params[idx + 1]))
            //                                {
            //                                    j = Conversions.Toint(@params[idx + 1]);
            //                                }

            //                                if (j <= 0 | withBlock20.CountEffect() < j)
            //                                {
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                EvalInfoFuncRet = withBlock20.EffectData(j);
            //                                break;
            //                            }

            //                        case "�˒�":
            //                        case "�ő�˒�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock20.MaxRange);
            //                                break;
            //                            }

            //                        case "�ŏ��˒�":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock20.MinRange);
            //                                break;
            //                            }

            //                        case "�ő�g�p��":
            //                        case "�g�p��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock20.Stock);
            //                                break;
            //                            }

            //                        case "����d�m":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock20.ENConsumption);
            //                                break;
            //                            }

            //                        case "�K�v�C��":
            //                            {
            //                                EvalInfoFuncRet = SrcFormatter.Format(withBlock20.NecessaryMorale);
            //                                break;
            //                            }

            //                        case "����":
            //                            {
            //                                EvalInfoFuncRet = withBlock20.Class;
            //                                break;
            //                            }

            //                        case "�������L":
            //                            {
            //                                if (GeneralLib.InStrNotNest(withBlock20.Class, @params[idx + 1]) > 0)
            //                                {
            //                                    EvalInfoFuncRet = "1";
            //                                }
            //                                else
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }

            //                        case "�������x��":
            //                            {
            //                                j = GeneralLib.InStrNotNest(withBlock20.Class, @params[idx + 1] + "L");
            //                                if (j == 0)
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                    return EvalInfoFuncRet;
            //                                }

            //                                EvalInfoFuncRet = "";
            //                                j = (j + Strings.Len(@params[idx + 1]) + 1);
            //                                do
            //                                {
            //                                    EvalInfoFuncRet = EvalInfoFuncRet + Strings.Mid(withBlock20.Class, j, 1);
            //                                    j = (j + 1);
            //                                }
            //                                while (GeneralLib.IsNumber(Strings.Mid(withBlock20.Class, j, 1)));
            //                                if (!GeneralLib.IsNumber(EvalInfoFuncRet))
            //                                {
            //                                    EvalInfoFuncRet = "0";
            //                                }

            //                                break;
            //                            }

            //                        case "�K�v�Z�\":
            //                            {
            //                                EvalInfoFuncRet = withBlock20.NecessarySkill;
            //                                break;
            //                            }

            //                        case "�g�p��":
            //                        case "�C��":
            //                            {
            //                                EvalInfoFuncRet = "1";
            //                                break;
            //                            }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "�����N":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(u.Rank);
            //            }

            //            break;
            //        }

            //    case "�{�X�����N":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(u.BossRank);
            //            }

            //            break;
            //        }

            //    case "�G���A":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = u.Area;
            //            }

            //            break;
            //        }

            //    case "�v�l���[�h":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = u.Mode;
            //            }

            //            break;
            //        }

            //    case "�ő�U����":
            //        {
            //            if (u is object)
            //            {
            //                {
            //                    var withBlock21 = u;
            //                    max_value = 0;
            //                    var loopTo20 = withBlock21.CountWeapon();
            //                    for (i = 1; i <= loopTo20; i++)
            //                    {
            //                        if (withBlock21.IsWeaponMastered(i) & !withBlock21.IsDisabled(withBlock21.Weapon(i).Name) & !withBlock21.IsWeaponClassifiedAs(i, "��"))
            //                        {
            //                            if (withBlock21.WeaponPower(i, "") > max_value)
            //                            {
            //                                max_value = withBlock21.WeaponPower(i, "");
            //                            }
            //                        }
            //                    }

            //                    EvalInfoFuncRet = SrcFormatter.Format(max_value);
            //                }
            //            }
            //            else if (ud is object)
            //            {
            //                max_value = 0;
            //                var loopTo21 = ud.CountWeapon();
            //                for (i = 1; i <= loopTo21; i++)
            //                {
            //                    WeaponData localWeapon7() { object argIndex1 = i; var ret = ud.Weapon(argIndex1); return ret; }

            //                    if (Strings.InStr(localWeapon7().Class, "��") == 0)
            //                    {
            //                        WeaponData localWeapon6() { object argIndex1 = i; var ret = ud.Weapon(argIndex1); return ret; }

            //                        if (localWeapon6().Power > max_value)
            //                        {
            //                            WeaponData localWeapon5() { object argIndex1 = i; var ret = ud.Weapon(argIndex1); return ret; }

            //                            max_value = localWeapon5().Power;
            //                        }
            //                    }
            //                }

            //                EvalInfoFuncRet = SrcFormatter.Format(max_value);
            //            }

            //            break;
            //        }

            //    case "�Œ��˒�":
            //        {
            //            if (u is object)
            //            {
            //                {
            //                    var withBlock22 = u;
            //                    max_value = 0;
            //                    var loopTo22 = withBlock22.CountWeapon();
            //                    for (i = 1; i <= loopTo22; i++)
            //                    {
            //                        if (withBlock22.IsWeaponMastered(i) & !withBlock22.IsDisabled(withBlock22.Weapon(i).Name) & !withBlock22.IsWeaponClassifiedAs(i, "��"))
            //                        {
            //                            if (withBlock22.WeaponMaxRange(i) > max_value)
            //                            {
            //                                max_value = withBlock22.WeaponMaxRange(i);
            //                            }
            //                        }
            //                    }

            //                    EvalInfoFuncRet = SrcFormatter.Format(max_value);
            //                }
            //            }
            //            else if (ud is object)
            //            {
            //                max_value = 0;
            //                var loopTo23 = ud.CountWeapon();
            //                for (i = 1; i <= loopTo23; i++)
            //                {
            //                    WeaponData localWeapon10() { object argIndex1 = i; var ret = ud.Weapon(argIndex1); return ret; }

            //                    if (Strings.InStr(localWeapon10().Class, "��") == 0)
            //                    {
            //                        WeaponData localWeapon9() { object argIndex1 = i; var ret = ud.Weapon(argIndex1); return ret; }

            //                        if (localWeapon9().MaxRange > max_value)
            //                        {
            //                            WeaponData localWeapon8() { object argIndex1 = i; var ret = ud.Weapon(argIndex1); return ret; }

            //                            max_value = localWeapon8().MaxRange;
            //                        }
            //                    }
            //                }

            //                EvalInfoFuncRet = SrcFormatter.Format(max_value);
            //            }

            //            break;
            //        }

            //    case "�c��T�|�[�g�A�^�b�N��":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(u.MaxSupportAttack() - u.UsedSupportAttack);
            //            }

            //            break;
            //        }

            //    case "�c��T�|�[�g�K�[�h��":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(u.MaxSupportGuard() - u.UsedSupportGuard);
            //            }

            //            break;
            //        }

            //    case "�c�蓯������U����":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(u.MaxSyncAttack() - u.UsedSyncAttack);
            //            }

            //            break;
            //        }

            //    case "�c��J�E���^�[�U����":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(u.MaxCounterAttack() - u.UsedCounterAttack);
            //            }

            //            break;
            //        }

            //    case "������":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(InterMission.RankUpCost(u));
            //            }

            //            break;
            //        }

            //    case "�ő������":
            //        {
            //            if (u is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(InterMission.MaxRank(u));
            //            }

            //            break;
            //        }

            //    case "�A�C�e���N���X":
            //        {
            //            if (it is object)
            //            {
            //                EvalInfoFuncRet = it.Class();
            //            }
            //            else if (itd is object)
            //            {
            //                EvalInfoFuncRet = itd.Class;
            //            }

            //            break;
            //        }

            //    case "������":
            //        {
            //            if (it is object)
            //            {
            //                EvalInfoFuncRet = it.Part();
            //            }
            //            else if (itd is object)
            //            {
            //                EvalInfoFuncRet = itd.Part;
            //            }

            //            break;
            //        }

            //    case "�ő�g�o�C���l":
            //        {
            //            if (it is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(it.HP());
            //            }
            //            else if (itd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(itd.HP);
            //            }

            //            break;
            //        }

            //    case "�ő�d�m�C���l":
            //        {
            //            if (it is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(it.EN());
            //            }
            //            else if (itd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(itd.EN);
            //            }

            //            break;
            //        }

            //    case "���b�C���l":
            //        {
            //            if (it is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(it.Armor());
            //            }
            //            else if (itd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(itd.Armor);
            //            }

            //            break;
            //        }

            //    case "�^�����C���l":
            //        {
            //            if (it is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(it.Mobility());
            //            }
            //            else if (itd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(itd.Mobility);
            //            }

            //            break;
            //        }

            //    case "�ړ��͏C���l":
            //        {
            //            if (it is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(it.Speed());
            //            }
            //            else if (itd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(itd.Speed);
            //            }

            //            break;
            //        }

            //    case "�����":
            //    case "�R�����g":
            //        {
            //            if (it is object)
            //            {
            //                EvalInfoFuncRet = it.Data.Comment;
            //                GeneralLib.ReplaceString(EvalInfoFuncRet, Constants.vbCr + Constants.vbLf, " ");
            //            }
            //            else if (itd is object)
            //            {
            //                EvalInfoFuncRet = itd.Comment;
            //                GeneralLib.ReplaceString(EvalInfoFuncRet, Constants.vbCr + Constants.vbLf, " ");
            //            }
            //            else if (spd is object)
            //            {
            //                EvalInfoFuncRet = spd.Comment;
            //            }

            //            break;
            //        }

            //    case "�Z�k��":
            //        {
            //            if (spd is object)
            //            {
            //                EvalInfoFuncRet = spd.intName;
            //            }

            //            break;
            //        }

            //    case "����r�o":
            //        {
            //            if (spd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(spd.SPConsumption);
            //            }

            //            break;
            //        }

            //    case "�Ώ�":
            //        {
            //            if (spd is object)
            //            {
            //                EvalInfoFuncRet = spd.TargetType;
            //            }

            //            break;
            //        }

            //    case "��������":
            //        {
            //            if (spd is object)
            //            {
            //                EvalInfoFuncRet = spd.Duration;
            //            }

            //            break;
            //        }

            //    case "�K�p����":
            //        {
            //            if (spd is object)
            //            {
            //                EvalInfoFuncRet = spd.NecessaryCondition;
            //            }

            //            break;
            //        }

            //    case "�A�j��":
            //        {
            //            if (spd is object)
            //            {
            //                EvalInfoFuncRet = spd.Animation;
            //            }

            //            break;
            //        }

            //    case "���ʐ�":
            //        {
            //            if (spd is object)
            //            {
            //                EvalInfoFuncRet = SrcFormatter.Format(spd.CountEffect());
            //            }

            //            break;
            //        }

            //    case "���ʃ^�C�v":
            //        {
            //            if (spd is object)
            //            {
            //                idx = (idx + 1);
            //                i = GeneralLib.StrToLng(@params[idx]);
            //                if (1 <= i & i <= spd.CountEffect())
            //                {
            //                    EvalInfoFuncRet = spd.EffectType(i);
            //                }
            //            }

            //            break;
            //        }

            //    case "���ʃ��x��":
            //        {
            //            if (spd is object)
            //            {
            //                idx = (idx + 1);
            //                i = GeneralLib.StrToLng(@params[idx]);
            //                if (1 <= i & i <= spd.CountEffect())
            //                {
            //                    EvalInfoFuncRet = SrcFormatter.Format(spd.EffectLevel(i));
            //                }
            //            }

            //            break;
            //        }

            //    case "���ʃf�[�^":
            //        {
            //            if (spd is object)
            //            {
            //                idx = (idx + 1);
            //                i = GeneralLib.StrToLng(@params[idx]);
            //                if (1 <= i & i <= spd.CountEffect())
            //                {
            //                    EvalInfoFuncRet = spd.EffectData(i);
            //                }
            //            }

            //            break;
            //        }

            //    case "�}�b�v":
            //        {
            //            idx = (idx + 1);
            //            switch (@params[idx] ?? "")
            //            {
            //                case "�t�@�C����":
            //                    {
            //                        EvalInfoFuncRet = Map.MapFileName;
            //                        if (Strings.Len(EvalInfoFuncRet) > Strings.Len(SRC.ScenarioPath))
            //                        {
            //                            if ((Strings.Left(EvalInfoFuncRet, Strings.Len(SRC.ScenarioPath)) ?? "") == (SRC.ScenarioPath ?? ""))
            //                            {
            //                                EvalInfoFuncRet = Strings.Mid(EvalInfoFuncRet, Strings.Len(SRC.ScenarioPath) + 1);
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "��":
            //                    {
            //                        EvalInfoFuncRet = SrcFormatter.Format(Map.MapWidth);
            //                        break;
            //                    }

            //                case "���ԑ�":
            //                    {
            //                        if (!string.IsNullOrEmpty(Map.MapDrawMode))
            //                        {
            //                            if (Map.MapDrawMode == "�t�B���^")
            //                            {
            //                                buf = Conversion.Hex(Map.MapDrawFilterColor);
            //                                var loopTo24 = (6 - Strings.Len(buf));
            //                                for (i = 1; i <= loopTo24; i++)
            //                                    buf = "0" + buf;
            //                                buf = "#" + Strings.Mid(buf, 5, 2) + Strings.Mid(buf, 3, 2) + Strings.Mid(buf, 1, 2) + " " + (Map.MapDrawFilterTransPercent * 100d).ToString() + "%";
            //                            }
            //                            else
            //                            {
            //                                buf = Map.MapDrawMode;
            //                            }

            //                            if (Map.MapDrawIsMapOnly)
            //                            {
            //                                buf = buf + " �}�b�v����";
            //                            }

            //                            EvalInfoFuncRet = buf;
            //                        }
            //                        else
            //                        {
            //                            EvalInfoFuncRet = "��";
            //                        }

            //                        break;
            //                    }

            //                case "����":
            //                    {
            //                        EvalInfoFuncRet = SrcFormatter.Format(Map.MapHeight);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        if (GeneralLib.IsNumber(@params[idx]))
            //                        {
            //                            mx = Conversions.Toint(@params[idx]);
            //                        }

            //                        idx = (idx + 1);
            //                        if (GeneralLib.IsNumber(@params[idx]))
            //                        {
            //                            my_Renamed = Conversions.Toint(@params[idx]);
            //                        }

            //                        if (mx < 1 | Map.MapWidth < mx | my_Renamed < 1 | Map.MapHeight < my_Renamed)
            //                        {
            //                            return EvalInfoFuncRet;
            //                        }

            //                        idx = (idx + 1);
            //                        switch (@params[idx] ?? "")
            //                        {
            //                            case "�n�`��":
            //                                {
            //                                    EvalInfoFuncRet = Map.TerrainName(mx, my_Renamed);
            //                                    break;
            //                                }

            //                            case "�n�`�^�C�v":
            //                            case "�n�`�N���X":
            //                                {
            //                                    EvalInfoFuncRet = Map.TerrainClass(mx, my_Renamed);
            //                                    break;
            //                                }

            //                            case "�ړ��R�X�g":
            //                                {
            //                                    // 0.5���݂̈ړ��R�X�g���g����悤�ɂ��邽�߁A�ړ��R�X�g��
            //                                    // ���ۂ̂Q�{�̒l�ŋL�^����Ă���
            //                                    EvalInfoFuncRet = SrcFormatter.Format(Map.TerrainMoveCost(mx, my_Renamed) / 2d);
            //                                    break;
            //                                }

            //                            case "����C��":
            //                                {
            //                                    EvalInfoFuncRet = SrcFormatter.Format(Map.TerrainEffectForHit(mx, my_Renamed));
            //                                    break;
            //                                }

            //                            case "�_���[�W�C��":
            //                                {
            //                                    EvalInfoFuncRet = SrcFormatter.Format(Map.TerrainEffectForDamage(mx, my_Renamed));
            //                                    break;
            //                                }

            //                            case "�g�o�񕜗�":
            //                                {
            //                                    EvalInfoFuncRet = SrcFormatter.Format(Map.TerrainEffectForHPRecover(mx, my_Renamed));
            //                                    break;
            //                                }

            //                            case "�d�m�񕜗�":
            //                                {
            //                                    EvalInfoFuncRet = SrcFormatter.Format(Map.TerrainEffectForENRecover(mx, my_Renamed));
            //                                    break;
            //                                }

            //                            case "�r�b�g�}�b�v��":
            //                                {
            //                                    // MOD START 240a
            //                                    // Select Case MapImageFileTypeData(mx, my)
            //                                    // Case SeparateDirMapImageFileType
            //                                    // EvalInfoFunc = _
            //                                    // '                                        TDList.Bitmap(MapData(mx, my, 0)) & "\" & _
            //                                    // '                                        TDList.Bitmap(MapData(mx, my, 0)) & _
            //                                    // '                                        Format$(MapData(mx, my, 1), "0000") & ".bmp"
            //                                    // Case FourFiguresMapImageFileType
            //                                    // EvalInfoFunc = _
            //                                    // '                                        TDList.Bitmap(MapData(mx, my, 0)) & _
            //                                    // '                                        Format$(MapData(mx, my, 1), "0000") & ".bmp"
            //                                    // Case OldMapImageFileType
            //                                    // EvalInfoFunc = _
            //                                    // '                                        TDList.Bitmap(MapData(mx, my, 0)) & _
            //                                    // '                                        Format$(MapData(mx, my, 1)) & ".bmp"
            //                                    // End Select
            //                                    switch (Map.MapImageFileTypeData[mx, my_Renamed])
            //                                    {
            //                                        case Map.MapImageFileType.SeparateDirMapImageFileType:
            //                                            {
            //                                                EvalInfoFuncRet = SRC.TDList.Bitmap(Map.MapData[mx, my_Renamed, Map.MapDataIndex.TerrainType]) + @"\" + SRC.TDList.Bitmap(Map.MapData[mx, my_Renamed, Map.MapDataIndex.TerrainType]) + SrcFormatter.Format(Map.MapData[mx, my_Renamed, Map.MapDataIndex.BitmapNo], "0000") + ".bmp";
            //                                                break;
            //                                            }

            //                                        case Map.MapImageFileType.FourFiguresMapImageFileType:
            //                                            {
            //                                                EvalInfoFuncRet = SRC.TDList.Bitmap(Map.MapData[mx, my_Renamed, Map.MapDataIndex.TerrainType]) + SrcFormatter.Format(Map.MapData[mx, my_Renamed, Map.MapDataIndex.BitmapNo], "0000") + ".bmp";
            //                                                break;
            //                                            }

            //                                        case Map.MapImageFileType.OldMapImageFileType:
            //                                            {
            //                                                EvalInfoFuncRet = SRC.TDList.Bitmap(Map.MapData[mx, my_Renamed, Map.MapDataIndex.TerrainType]) + SrcFormatter.Format(Map.MapData[mx, my_Renamed, Map.MapDataIndex.BitmapNo]) + ".bmp";
            //                                                break;
            //                                            }
            //                                    }

            //                                    break;
            //                                }
            //                            // MOD  END  240a
            //                            // ADD START 240a
            //                            case "���C���[�r�b�g�}�b�v��":
            //                                {
            //                                    switch (Map.MapImageFileTypeData[mx, my_Renamed])
            //                                    {
            //                                        case Map.MapImageFileType.SeparateDirMapImageFileType:
            //                                            {
            //                                                EvalInfoFuncRet = SRC.TDList.Bitmap(Map.MapData[mx, my_Renamed, Map.MapDataIndex.LayerType]) + @"\" + SRC.TDList.Bitmap(Map.MapData[mx, my_Renamed, Map.MapDataIndex.LayerType]) + SrcFormatter.Format(Map.MapData[mx, my_Renamed, Map.MapDataIndex.LayerBitmapNo], "0000") + ".bmp";
            //                                                break;
            //                                            }

            //                                        case Map.MapImageFileType.FourFiguresMapImageFileType:
            //                                            {
            //                                                EvalInfoFuncRet = SRC.TDList.Bitmap(Map.MapData[mx, my_Renamed, Map.MapDataIndex.LayerType]) + SrcFormatter.Format(Map.MapData[mx, my_Renamed, Map.MapDataIndex.LayerBitmapNo], "0000") + ".bmp";
            //                                                break;
            //                                            }

            //                                        case Map.MapImageFileType.OldMapImageFileType:
            //                                            {
            //                                                EvalInfoFuncRet = SRC.TDList.Bitmap(Map.MapData[mx, my_Renamed, Map.MapDataIndex.LayerType]) + SrcFormatter.Format(Map.MapData[mx, my_Renamed, Map.MapDataIndex.LayerBitmapNo]) + ".bmp";
            //                                                break;
            //                                            }
            //                                    }

            //                                    break;
            //                                }
            //                            // ADD  END  240a
            //                            case "���j�b�g�h�c":
            //                                {
            //                                    if (Map.MapDataForUnit[mx, my_Renamed] is object)
            //                                    {
            //                                        EvalInfoFuncRet = Map.MapDataForUnit[mx, my_Renamed].ID;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        break;
            //                    }
            //            }

            //            break;
            //        }

            //    case "�I�v�V����":
            //        {
            //            idx = (idx + 1);
            //            switch (@params[idx] ?? "")
            //            {
            //                case "MessageWait":
            //                    {
            //                        EvalInfoFuncRet = SrcFormatter.Format(GUI.MessageWait);
            //                        break;
            //                    }

            //                case "BattleAnimation":
            //                    {
            //                        if (SRC.BattleAnimation)
            //                        {
            //                            EvalInfoFuncRet = "On";
            //                        }
            //                        else
            //                        {
            //                            EvalInfoFuncRet = "Off";
            //                        }

            //                        break;
            //                    }
            //                // ADD START MARGE
            //                case "ExtendedAnimation":
            //                    {
            //                        if (SRC.ExtendedAnimation)
            //                        {
            //                            EvalInfoFuncRet = "On";
            //                        }
            //                        else
            //                        {
            //                            EvalInfoFuncRet = "Off";
            //                        }

            //                        break;
            //                    }
            //                // ADD END MARGE
            //                case "SpecialPowerAnimation":
            //                    {
            //                        if (SRC.SpecialPowerAnimation)
            //                        {
            //                            EvalInfoFuncRet = "On";
            //                        }
            //                        else
            //                        {
            //                            EvalInfoFuncRet = "Off";
            //                        }

            //                        break;
            //                    }

            //                case "AutoDeffence":
            //                    {
            //                        if (SystemConfig.AutoDefense)
            //                        {
            //                            EvalInfoFuncRet = "On";
            //                        }
            //                        else
            //                        {
            //                            EvalInfoFuncRet = "Off";
            //                        }

            //                        break;
            //                    }

            //                case "UseDirectMusic":
            //                    {
            //                        if (Sound.UseDirectMusic)
            //                        {
            //                            EvalInfoFuncRet = "On";
            //                        }
            //                        else
            //                        {
            //                            EvalInfoFuncRet = "Off";
            //                        }

            //                        break;
            //                    }
            //                // MOD START MARGE
            //                // Case "Turn", "Square", "KeepEnemyBGM", "MidiReset", _
            //                // '                    "AutoMoveCursor", "DebugMode", "LastFolder", _
            //                // '                    "MIDIPortID", "MP3Volume", _
            //                // '                    "BattleAnimation", "WeaponAnimation", "MoveAnimation", _
            //                // '                    "ImageBufferNum", "MaxImageBufferSize", "KeepStretchedImage", _
            //                // '                    "UseTransparentBlt"
            //                // �uNewGUI�v�ŒT���ɗ�����INI�̏�Ԃ�Ԃ��B�u�V�f�t�h�v�ŒT���ɗ�����Option�̏�Ԃ�Ԃ��B
            //                case "Turn":
            //                case "Square":
            //                case "KeepEnemyBGM":
            //                case "MidiReset":
            //                case "AutoMoveCursor":
            //                case "DebugMode":
            //                case "LastFolder":
            //                case "MIDIPortID":
            //                case "MP3Volume":
            //                case var case13 when case13 == "BattleAnimation":
            //                case "WeaponAnimation":
            //                case "MoveAnimation":
            //                case "ImageBufferNum":
            //                case "MaxImageBufferSize":
            //                case "KeepStretchedImage":
            //                case "UseTransparentBlt":
            //                case "NewGUI":
            //                    {
            //                        // MOD END MARGE
            //                        EvalInfoFuncRet = GeneralLib.ReadIni("Option", @params[idx]);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        // Option�R�}���h�̃I�v�V�������Q��
            //                        if (IsOptionDefined(@params[idx]))
            //                        {
            //                            EvalInfoFuncRet = "On";
            //                        }
            //                        else
            //                        {
            //                            EvalInfoFuncRet = "Off";
            //                        }

            //                        break;
            //                    }
            //            }

            //            break;
            //        }
            //}

            //return EvalInfoFuncRet;

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