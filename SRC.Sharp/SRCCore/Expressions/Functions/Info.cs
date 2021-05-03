using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.VB;
using System.Linq;

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

            int mx = default, my = default;
            switch (@params[idx] ?? "")
            {
                case "����":
                    {
                        if (u != null)
                        {
                            str_result = u.Name;
                        }
                        else if (ud != null)
                        {
                            str_result = ud.Name;
                        }
                        else if (p != null)
                        {
                            str_result = p.Name;
                        }
                        else if (pd != null)
                        {
                            str_result = pd.Name;
                        }
                        else if (nd != null)
                        {
                            str_result = nd.Name;
                        }
                        else if (it != null)
                        {
                            str_result = it.Name;
                        }
                        else if (itd != null)
                        {
                            str_result = itd.Name;
                        }
                        else if (spd != null)
                        {
                            str_result = spd.Name;
                        }

                        break;
                    }

                case "�ǂ݉���":
                    {
                        if (u != null)
                        {
                            str_result = u.KanaName;
                        }
                        else if (ud != null)
                        {
                            str_result = ud.KanaName;
                        }
                        else if (p != null)
                        {
                            str_result = p.KanaName;
                        }
                        else if (pd != null)
                        {
                            str_result = pd.KanaName;
                        }
                        else if (it != null)
                        {
                            str_result = it.Data.KanaName;
                        }
                        else if (itd != null)
                        {
                            str_result = itd.KanaName;
                        }
                        else if (spd != null)
                        {
                            str_result = spd.KanaName;
                        }

                        break;
                    }

                case "����":
                    {
                        if (u != null)
                        {
                            str_result = u.Nickname0;
                        }
                        else if (ud != null)
                        {
                            str_result = ud.Nickname;
                        }
                        else if (p != null)
                        {
                            str_result = p.get_Nickname(false);
                        }
                        else if (pd != null)
                        {
                            str_result = pd.Nickname;
                        }
                        else if (nd != null)
                        {
                            str_result = nd.Nickname;
                        }
                        else if (it != null)
                        {
                            str_result = it.Nickname();
                        }
                        else if (itd != null)
                        {
                            str_result = itd.Nickname;
                        }

                        break;
                    }

                case "����":
                    {
                        if (p != null)
                        {
                            str_result = p.Sex;
                        }
                        else if (pd != null)
                        {
                            str_result = pd.Sex;
                        }
                        return ValueType.StringType;
                    }

                case "���j�b�g�N���X":
                case "�@�̃N���X":
                    {
                        if (u != null)
                        {
                            str_result = u.Class;
                        }
                        else if (ud != null)
                        {
                            str_result = ud.Class;
                        }
                        else if (p != null)
                        {
                            str_result = p.Class;
                        }
                        else if (pd != null)
                        {
                            str_result = pd.Class;
                        }

                        break;
                    }

                case "�n�`�K��":
                    {
                        if (u != null)
                        {
                            for (var i = 1; i <= 4; i++)
                            {
                                switch (u.get_Adaption(i))
                                {
                                    case 5:
                                        {
                                            str_result = str_result + "S";
                                            break;
                                        }

                                    case 4:
                                        {
                                            str_result = str_result + "A";
                                            break;
                                        }

                                    case 3:
                                        {
                                            str_result = str_result + "B";
                                            break;
                                        }

                                    case 2:
                                        {
                                            str_result = str_result + "C";
                                            break;
                                        }

                                    case 1:
                                        {
                                            str_result = str_result + "D";
                                            break;
                                        }

                                    default:
                                        {
                                            str_result = str_result + "E";
                                            break;
                                        }
                                }
                            }
                        }
                        else if (ud != null)
                        {
                            str_result = ud.Adaption;
                        }
                        else if (p != null)
                        {
                            str_result = p.Adaption;
                        }
                        else if (pd != null)
                        {
                            str_result = pd.Adaption;
                        }

                        break;
                    }

                case "�o���l":
                    {
                        if (u != null)
                        {
                            str_result = u.ExpValue.ToString();
                        }
                        else if (ud != null)
                        {
                            str_result = ud.ExpValue.ToString();
                        }
                        else if (p != null)
                        {
                            str_result = p.ExpValue.ToString();
                        }
                        else if (pd != null)
                        {
                            str_result = pd.ExpValue.ToString();
                        }

                        break;
                    }

                case "�i��":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.Infight);
                        }
                        else if (pd != null)
                        {
                            str_result = SrcFormatter.Format(pd.Infight);
                        }

                        break;
                    }

                case "�ˌ�":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.Shooting);
                        }
                        else if (pd != null)
                        {
                            str_result = SrcFormatter.Format(pd.Shooting);
                        }

                        break;
                    }

                case "����":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.Hit);
                        }
                        else if (pd != null)
                        {
                            str_result = SrcFormatter.Format(pd.Hit);
                        }

                        break;
                    }

                case "���":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.Dodge);
                        }
                        else if (pd != null)
                        {
                            str_result = SrcFormatter.Format(pd.Dodge);
                        }

                        break;
                    }

                case "�Z��":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.Technique);
                        }
                        else if (pd != null)
                        {
                            str_result = SrcFormatter.Format(pd.Technique);
                        }

                        break;
                    }

                case "����":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.Intuition);
                        }
                        else if (pd != null)
                        {
                            str_result = SrcFormatter.Format(pd.Intuition);
                        }

                        break;
                    }

                case "�h��":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.Defense);
                        }

                        break;
                    }

                case "�i����{�l":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.InfightBase);
                        }

                        break;
                    }

                case "�ˌ���{�l":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.ShootingBase);
                        }

                        break;
                    }

                case "������{�l":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.HitBase);
                        }

                        break;
                    }

                case "�����{�l":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.DodgeBase);
                        }

                        break;
                    }

                case "�Z�ʊ�{�l":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.TechniqueBase);
                        }

                        break;
                    }

                case "������{�l":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.IntuitionBase);
                        }

                        break;
                    }

                case "�i���C���l":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.InfightMod);
                        }

                        break;
                    }

                case "�ˌ��C���l":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.ShootingMod);
                        }

                        break;
                    }

                case "�����C���l":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.HitMod);
                        }

                        break;
                    }

                case "����C���l":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.DodgeMod);
                        }

                        break;
                    }

                case "�Z�ʏC���l":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.TechniqueMod);
                        }

                        break;
                    }

                case "�����C���l":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.IntuitionMod);
                        }

                        break;
                    }

                case "�i���x���C���l":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.InfightMod2);
                        }

                        break;
                    }

                case "�ˌ��x���C���l":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.ShootingMod2);
                        }

                        break;
                    }

                case "�����x���C���l":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.HitMod2);
                        }

                        break;
                    }

                case "����x���C���l":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.DodgeMod2);
                        }

                        break;
                    }

                case "�Z�ʎx���C���l":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.TechniqueMod2);
                        }

                        break;
                    }

                case "�����x���C���l":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.IntuitionMod2);
                        }

                        break;
                    }

                case "���i":
                    {
                        if (p != null)
                        {
                            str_result = p.Personality;
                        }
                        else if (pd != null)
                        {
                            str_result = pd.Personality;
                        }

                        break;
                    }

                case "�ő�r�o":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.MaxSP);
                            if (p.MaxSP == 0 && p.Unit != null)
                            {
                                if (ReferenceEquals(p, p.Unit.MainPilot()))
                                {
                                    str_result = SrcFormatter.Format(p.Unit.Pilots.First().MaxSP);
                                }
                            }
                        }
                        else if (pd != null)
                        {
                            str_result = SrcFormatter.Format(pd.SP);
                        }

                        break;
                    }

                case "�r�o":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.SP);
                            if (p.MaxSP == 0 && p.Unit != null)
                            {
                                if (ReferenceEquals(p, p.Unit.MainPilot()))
                                {
                                    str_result = SrcFormatter.Format(p.Unit.Pilots.First().SP);
                                }
                            }
                        }
                        else if (pd != null)
                        {
                            str_result = SrcFormatter.Format(pd.SP);
                        }

                        break;
                    }

                case "�O���t�B�b�N":
                    {
                        if (u != null)
                        {
                            str_result = u.get_Bitmap(true);
                        }
                        else if (ud != null)
                        {
                            str_result = ud.Bitmap0;
                        }
                        else if (p != null)
                        {
                            str_result = p.get_Bitmap(true);
                        }
                        else if (pd != null)
                        {
                            str_result = pd.Bitmap0;
                        }
                        else if (nd != null)
                        {
                            str_result = nd.Bitmap0;
                        }

                        break;
                    }

                case "�l�h�c�h":
                    {
                        if (p != null)
                        {
                            str_result = p.BGM;
                        }
                        else if (pd != null)
                        {
                            str_result = pd.BGM;
                        }

                        break;
                    }

                case "���x��":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.Level);
                        }

                        break;
                    }

                case "�ݐόo���l":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.Exp);
                        }

                        break;
                    }

                case "�C��":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.Morale);
                        }

                        break;
                    }

                case "�ő���":
                case "�ő�v���[�i":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.MaxPlana());
                        }
                        else if (pd != null)
                        {
                            str_result = SrcFormatter.Format(pd.SkillLevel(0, "���"));
                        }

                        break;
                    }

                case "���":
                case "�v���[�i":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.Plana);
                        }
                        else if (pd != null)
                        {
                            str_result = SrcFormatter.Format(pd.SkillLevel(0, "���"));
                        }

                        break;
                    }

                case "������":
                case "�V���N����":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.SynchroRate());
                        }
                        else if (pd != null)
                        {
                            str_result = SrcFormatter.Format(pd.SkillLevel(0, "������"));
                        }

                        break;
                    }

                case "�X�y�V�����p���[":
                case "���_�R�}���h":
                case "���_":
                    {
                        if (p != null)
                        {
                            if (p.MaxSP == 0 && p.Unit != null)
                            {
                                if (ReferenceEquals(p, p.Unit.MainPilot()))
                                {
                                    p = p.Unit.Pilots.First();
                                }
                            }

                            {
                                var withBlock = p;
                                var loopTo = withBlock.CountSpecialPower;
                                for (var i = 1; i <= loopTo; i++)
                                    str_result = str_result + " " + withBlock.get_SpecialPower(i);
                            }

                            str_result = Strings.Trim(str_result);
                        }
                        else if (pd != null)
                        {
                            var loopTo1 = pd.CountSpecialPower(100);
                            for (var i = 1; i <= loopTo1; i++)
                                str_result = str_result + " " + pd.SpecialPower(100, i);
                            str_result = Strings.Trim(str_result);
                        }

                        break;
                    }

                case "�X�y�V�����p���[���L":
                case "���_�R�}���h���L":
                    {
                        if (p != null)
                        {
                            if (p.MaxSP == 0 && p.Unit != null)
                            {
                                if (ReferenceEquals(p, p.Unit.MainPilot()))
                                {
                                    p = p.Unit.Pilots.First();
                                }
                            }

                            if (p.IsSpecialPowerAvailable(@params[idx + 1]))
                            {
                                str_result = "1";
                            }
                            else
                            {
                                str_result = "0";
                            }
                        }
                        else if (pd != null)
                        {
                            if (pd.IsSpecialPowerAvailable(100, @params[idx + 1]))
                            {
                                str_result = "1";
                            }
                            else
                            {
                                str_result = "0";
                            }
                        }

                        break;
                    }

                case "�X�y�V�����p���[�R�X�g":
                case "���_�R�}���h�R�X�g":
                    {
                        if (p != null)
                        {
                            if (p.MaxSP == 0 && p.Unit != null)
                            {
                                if (ReferenceEquals(p, p.Unit.MainPilot()))
                                {
                                    p = p.Unit.Pilots.First();
                                }
                            }

                            str_result = SrcFormatter.Format(p.SpecialPowerCost(@params[idx + 1]));
                        }
                        else if (pd != null)
                        {
                            str_result = SrcFormatter.Format(pd.SpecialPowerCost(@params[idx + 1]));
                        }

                        break;
                    }

                case "����\�͐�":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(u.CountFeature());
                        }
                        else if (ud != null)
                        {
                            str_result = SrcFormatter.Format(ud.CountFeature());
                        }
                        else if (p != null)
                        {
                            str_result = p.CountSkill().ToString();
                        }
                        else if (pd != null)
                        {
                            str_result = GeneralLib.LLength(pd.Skill(100)).ToString();
                        }
                        else if (it != null)
                        {
                            str_result = SrcFormatter.Format(it.CountFeature());
                        }
                        else if (itd != null)
                        {
                            str_result = SrcFormatter.Format(itd.CountFeature());
                        }

                        break;
                    }

                case "����\��":
                    {
                        if (u != null)
                        {
                            if (GeneralLib.IsNumber(@params[idx + 1]))
                            {
                                str_result = u.Feature(Conversions.ToInteger(@params[idx + 1]))?.Name;
                            }
                        }
                        else if (ud != null)
                        {
                            if (GeneralLib.IsNumber(@params[idx + 1]))
                            {
                                str_result = ud.Feature(Conversions.ToInteger(@params[idx + 1]))?.Name;
                            }
                        }
                        else if (p != null)
                        {
                            if (GeneralLib.IsNumber(@params[idx + 1]))
                            {
                                str_result = p.Skill(Conversions.ToInteger(@params[idx + 1]));
                            }
                        }
                        else if (pd != null)
                        {
                            if (GeneralLib.IsNumber(@params[idx + 1]))
                            {
                                str_result = GeneralLib.LIndex(pd.Skill(100), Conversions.ToInteger(@params[idx + 1]));
                            }
                        }
                        else if (it != null)
                        {
                            if (GeneralLib.IsNumber(@params[idx + 1]))
                            {
                                str_result = it.Feature(Conversions.ToInteger(@params[idx + 1]))?.Name;
                            }
                        }
                        else if (itd != null)
                        {
                            if (GeneralLib.IsNumber(@params[idx + 1]))
                            {
                                str_result = itd.Feature(Conversions.ToInteger(@params[idx + 1]))?.Name;
                            }
                        }

                        break;
                    }

                case "����\�͖���":
                    {
                        var aname = @params[idx + 1];

                        // �G���A�X����`����Ă���H
                        if (SRC.ALDList.IsDefined(aname))
                        {
                            aname = SRC.ALDList.Item(aname).ReplaceTypeName(aname);
                        }

                        if (u != null)
                        {
                            if (GeneralLib.IsNumber(aname))
                            {
                                str_result = u.FeatureName(Conversions.ToInteger(@params[idx + 1]));
                            }
                            else
                            {
                                str_result = u.FeatureName(aname);
                            }
                        }
                        else if (ud != null)
                        {
                            if (GeneralLib.IsNumber(aname))
                            {
                                str_result = ud.FeatureName(Conversions.ToInteger(aname));
                            }
                            else
                            {
                                str_result = ud.FeatureName(aname);
                            }
                        }
                        else if (p != null)
                        {
                            if (GeneralLib.IsNumber(aname))
                            {
                                str_result = p.SkillName(Conversions.ToInteger(aname));
                            }
                            else
                            {
                                str_result = p.SkillName(aname);
                            }
                        }
                        else if (pd != null)
                        {
                            if (GeneralLib.IsNumber(aname))
                            {
                                str_result = pd.SkillName(100, GeneralLib.LIndex(pd.Skill(100), Conversions.ToInteger(aname)));
                            }
                            else
                            {
                                str_result = pd.SkillName(100, aname);
                            }
                        }
                        else if (it != null)
                        {
                            if (GeneralLib.IsNumber(aname))
                            {
                                str_result = it.FeatureName(Conversions.ToInteger(aname));
                            }
                            else
                            {
                                str_result = it.FeatureName(aname);
                            }
                        }
                        else if (itd != null)
                        {
                            if (GeneralLib.IsNumber(aname))
                            {
                                str_result = itd.FeatureName(Conversions.ToInteger(aname));
                            }
                            else
                            {
                                str_result = itd.FeatureName(aname);
                            }
                        }

                        break;
                    }

                case "����\�͏��L":
                    {
                        var aname = @params[idx + 1];

                        // �G���A�X����`����Ă���H
                        if (SRC.ALDList.IsDefined(aname))
                        {
                            aname = SRC.ALDList.Item(aname).ReplaceTypeName(aname);
                        }

                        if (u != null)
                        {
                            if (u.IsFeatureAvailable(aname))
                            {
                                str_result = "1";
                            }
                            else
                            {
                                str_result = "0";
                            }
                        }
                        else if (ud != null)
                        {
                            if (ud.IsFeatureAvailable(aname))
                            {
                                str_result = "1";
                            }
                            else
                            {
                                str_result = "0";
                            }
                        }
                        else if (p != null)
                        {
                            if (p.IsSkillAvailable(aname))
                            {
                                str_result = "1";
                            }
                            else
                            {
                                str_result = "0";
                            }
                        }
                        else if (pd != null)
                        {
                            if (pd.IsSkillAvailable(100, aname))
                            {
                                str_result = "1";
                            }
                            else
                            {
                                str_result = "0";
                            }
                        }
                        else if (it != null)
                        {
                            if (it.IsFeatureAvailable(aname))
                            {
                                str_result = "1";
                            }
                            else
                            {
                                str_result = "0";
                            }
                        }
                        else if (itd != null)
                        {
                            if (itd.IsFeatureAvailable(aname))
                            {
                                str_result = "1";
                            }
                            else
                            {
                                str_result = "0";
                            }
                        }

                        break;
                    }

                    //case "����\�̓��x��":
                    //    {
                    //        var aname = @params[idx + 1];

                    //        // �G���A�X����`����Ă���H
                    //        if (SRC.ALDList.IsDefined(aname))
                    //        {
                    //            var alias = SRC.ALDList.Item(aname);
                    //            var aliasElem = alias.Elements.FirstOrDefault(x => GeneralLib.LIndex(x.strAliasData, 1) == aname);

                    //            if (aliasElem != null)
                    //            {
                    //                aname = aliasElem.strAliasType;
                    //            }
                    //            else
                    //            {
                    //                aname = alias.Elements.First().strAliasType;
                    //            }
                    //        }

                    //        if (u != null)
                    //        {
                    //            if (GeneralLib.IsNumber(aname))
                    //            {
                    //                str_result = SrcFormatter.Format(u.FeatureLevel(Conversions.ToInteger(aname)));
                    //            }
                    //            else
                    //            {
                    //                str_result = SrcFormatter.Format(u.FeatureLevel(aname));
                    //            }
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            if (GeneralLib.IsNumber(aname))
                    //            {
                    //                str_result = SrcFormatter.Format(ud.FeatureLevel(Conversions.ToInteger(aname)));
                    //            }
                    //            else
                    //            {
                    //                str_result = SrcFormatter.Format(ud.FeatureLevel(aname));
                    //            }
                    //        }
                    //        else if (p != null)
                    //        {
                    //            if (GeneralLib.IsNumber(aname))
                    //            {
                    //                str_result = SrcFormatter.Format(p.SkillLevel(Conversions.ToInteger(aname)));
                    //            }
                    //            else
                    //            {
                    //                str_result = SrcFormatter.Format(p.SkillLevel(aname));
                    //            }
                    //        }
                    //        else if (pd != null)
                    //        {
                    //            if (GeneralLib.IsNumber(aname))
                    //            {
                    //                string localLIndex4() { string arglist = pd.Skill(100); var ret = GeneralLib.LIndex(arglist, Conversions.ToInteger(aname)); return ret; }

                    //                double localSkillLevel2() { string argsname = hs69a3321344d140f8b91f1e9add379ed5(); var ret = pd.SkillLevel(100, argsname); return ret; }

                    //                str_result = SrcFormatter.Format(localSkillLevel2());
                    //            }
                    //            else
                    //            {
                    //                str_result = SrcFormatter.Format(pd.SkillLevel(100, aname));
                    //            }
                    //        }
                    //        else if (it != null)
                    //        {
                    //            if (GeneralLib.IsNumber(aname))
                    //            {
                    //                double localFeatureLevel4() { object argIndex1 = Conversions.ToInteger(aname); var ret = it.FeatureLevel(argIndex1); return ret; }

                    //                str_result = SrcFormatter.Format(localFeatureLevel4());
                    //            }
                    //            else
                    //            {
                    //                double localFeatureLevel5() { object argIndex1 = aname; var ret = it.FeatureLevel(argIndex1); return ret; }

                    //                str_result = SrcFormatter.Format(localFeatureLevel5());
                    //            }
                    //        }
                    //        else if (itd != null)
                    //        {
                    //            if (GeneralLib.IsNumber(aname))
                    //            {
                    //                double localFeatureLevel6() { object argIndex1 = Conversions.ToInteger(aname); var ret = itd.FeatureLevel(argIndex1); return ret; }

                    //                str_result = SrcFormatter.Format(localFeatureLevel6());
                    //            }
                    //            else
                    //            {
                    //                double localFeatureLevel7() { object argIndex1 = aname; var ret = itd.FeatureLevel(argIndex1); return ret; }

                    //                str_result = SrcFormatter.Format(localFeatureLevel7());
                    //            }
                    //        }

                    //        break;
                    //    }

                    //case "����\�̓f�[�^":
                    //    {
                    //        aname = @params[idx + 1];

                    //        // �G���A�X����`����Ă���H
                    //        if (SRC.ALDList.IsDefined(aname))
                    //        {
                    //            {
                    //                var withBlock4 = SRC.ALDList.Item(aname);
                    //                var loopTo5 = withBlock4.Count;
                    //                for (i = 1; i <= loopTo5; i++)
                    //                {
                    //                    string localLIndex5() { string arglist = withBlock4.get_AliasData(i); var ret = GeneralLib.LIndex(arglist, 1); withBlock4.get_AliasData(i) = arglist; return ret; }

                    //                    if ((localLIndex5() ?? "") == (aname ?? ""))
                    //                    {
                    //                        aname = withBlock4.get_AliasType(i);
                    //                        break;
                    //                    }
                    //                }

                    //                if (i > withBlock4.Count)
                    //                {
                    //                    aname = withBlock4.get_AliasType(1);
                    //                }
                    //            }
                    //        }

                    //        if (u != null)
                    //        {
                    //            if (GeneralLib.IsNumber(aname))
                    //            {
                    //                str_result = u.FeatureData(Conversions.ToInteger(aname));
                    //            }
                    //            else
                    //            {
                    //                str_result = u.FeatureData(aname);
                    //            }
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            if (GeneralLib.IsNumber(aname))
                    //            {
                    //                str_result = ud.FeatureData(Conversions.ToInteger(aname));
                    //            }
                    //            else
                    //            {
                    //                str_result = ud.FeatureData(aname);
                    //            }
                    //        }
                    //        else if (p != null)
                    //        {
                    //            if (GeneralLib.IsNumber(aname))
                    //            {
                    //                str_result = p.SkillData(Conversions.ToInteger(aname));
                    //            }
                    //            else
                    //            {
                    //                str_result = p.SkillData(aname);
                    //            }
                    //        }
                    //        else if (pd != null)
                    //        {
                    //            if (GeneralLib.IsNumber(aname))
                    //            {
                    //                string localLIndex6() { string arglist = pd.Skill(100); var ret = GeneralLib.LIndex(arglist, Conversions.ToInteger(aname)); return ret; }

                    //                str_result = pd.SkillData(100, localLIndex6());
                    //            }
                    //            else
                    //            {
                    //                str_result = pd.SkillData(100, aname);
                    //            }
                    //        }
                    //        else if (it != null)
                    //        {
                    //            if (GeneralLib.IsNumber(aname))
                    //            {
                    //                str_result = it.FeatureData(Conversions.ToInteger(aname));
                    //            }
                    //            else
                    //            {
                    //                str_result = it.FeatureData(aname);
                    //            }
                    //        }
                    //        else if (itd != null)
                    //        {
                    //            if (GeneralLib.IsNumber(aname))
                    //            {
                    //                str_result = itd.FeatureData(Conversions.ToInteger(aname));
                    //            }
                    //            else
                    //            {
                    //                str_result = itd.FeatureData(aname);
                    //            }
                    //        }

                    //        break;
                    //    }

                    //case "����\�͕K�v�Z�\":
                    //    {
                    //        aname = @params[idx + 1];

                    //        // �G���A�X����`����Ă���H
                    //        if (SRC.ALDList.IsDefined(aname))
                    //        {
                    //            {
                    //                var withBlock5 = SRC.ALDList.Item(aname);
                    //                var loopTo6 = withBlock5.Count;
                    //                for (i = 1; i <= loopTo6; i++)
                    //                {
                    //                    string localLIndex7() { string arglist = withBlock5.get_AliasData(i); var ret = GeneralLib.LIndex(arglist, 1); withBlock5.get_AliasData(i) = arglist; return ret; }

                    //                    if ((localLIndex7() ?? "") == (aname ?? ""))
                    //                    {
                    //                        aname = withBlock5.get_AliasType(i);
                    //                        break;
                    //                    }
                    //                }

                    //                if (i > withBlock5.Count)
                    //                {
                    //                    aname = withBlock5.get_AliasType(1);
                    //                }
                    //            }
                    //        }

                    //        if (u != null)
                    //        {
                    //            if (GeneralLib.IsNumber(aname))
                    //            {
                    //                str_result = u.FeatureNecessarySkill(Conversions.ToInteger(aname));
                    //            }
                    //            else
                    //            {
                    //                str_result = u.FeatureNecessarySkill(aname);
                    //            }
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            if (GeneralLib.IsNumber(aname))
                    //            {
                    //                str_result = ud.FeatureNecessarySkill(Conversions.ToInteger(aname));
                    //            }
                    //            else
                    //            {
                    //                str_result = ud.FeatureNecessarySkill(aname);
                    //            }
                    //        }
                    //        else if (it != null)
                    //        {
                    //            if (GeneralLib.IsNumber(aname))
                    //            {
                    //                str_result = it.FeatureNecessarySkill(Conversions.ToInteger(aname));
                    //            }
                    //            else
                    //            {
                    //                str_result = it.FeatureNecessarySkill(aname);
                    //            }
                    //        }
                    //        else if (itd != null)
                    //        {
                    //            if (GeneralLib.IsNumber(aname))
                    //            {
                    //                str_result = itd.FeatureNecessarySkill(Conversions.ToInteger(aname));
                    //            }
                    //            else
                    //            {
                    //                str_result = itd.FeatureNecessarySkill(aname);
                    //            }
                    //        }

                    //        break;
                    //    }

                    //case "����\�͉��":
                    //    {
                    //        aname = @params[idx + 1];

                    //        // �G���A�X����`����Ă���H
                    //        if (SRC.ALDList.IsDefined(aname))
                    //        {
                    //            {
                    //                var withBlock6 = SRC.ALDList.Item(aname);
                    //                var loopTo7 = withBlock6.Count;
                    //                for (i = 1; i <= loopTo7; i++)
                    //                {
                    //                    string localLIndex8() { string arglist = withBlock6.get_AliasData(i); var ret = GeneralLib.LIndex(arglist, 1); withBlock6.get_AliasData(i) = arglist; return ret; }

                    //                    if ((localLIndex8() ?? "") == (aname ?? ""))
                    //                    {
                    //                        aname = withBlock6.get_AliasType(i);
                    //                        break;
                    //                    }
                    //                }

                    //                if (i > withBlock6.Count)
                    //                {
                    //                    aname = withBlock6.get_AliasType(1);
                    //                }
                    //            }
                    //        }

                    //        if (u != null)
                    //        {
                    //            if (GeneralLib.IsNumber(aname))
                    //            {
                    //                str_result = Help.FeatureHelpMessage(u, Conversions.ToInteger(aname), false);
                    //            }
                    //            else
                    //            {
                    //                str_result = Help.FeatureHelpMessage(u, aname, false);
                    //            }

                    //            if (string.IsNullOrEmpty(str_result) && p != null)
                    //            {
                    //                str_result = Help.SkillHelpMessage(p, aname);
                    //            }
                    //        }
                    //        else if (p != null)
                    //        {
                    //            str_result = Help.SkillHelpMessage(p, aname);
                    //            if (string.IsNullOrEmpty(str_result) && u != null)
                    //            {
                    //                if (GeneralLib.IsNumber(aname))
                    //                {
                    //                    str_result = Help.FeatureHelpMessage(u, Conversions.ToInteger(aname), false);
                    //                }
                    //                else
                    //                {
                    //                    str_result = Help.FeatureHelpMessage(u, aname, false);
                    //                }
                    //            }
                    //        }

                    //        break;
                    //    }

                    //case "�K��p�C���b�g��":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.Data.PilotNum);
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            str_result = SrcFormatter.Format(ud.PilotNum);
                    //        }

                    //        break;
                    //    }

                    //case "�p�C���b�g��":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.CountPilot());
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            str_result = SrcFormatter.Format(ud.PilotNum);
                    //        }

                    //        break;
                    //    }

                    //case "�T�|�[�g��":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.CountSupport());
                    //        }

                    //        break;
                    //    }

                    //case "�ő�A�C�e����":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.Data.ItemNum);
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            str_result = SrcFormatter.Format(ud.ItemNum);
                    //        }

                    //        break;
                    //    }

                    //case "�A�C�e����":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.CountItem());
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            str_result = SrcFormatter.Format(ud.ItemNum);
                    //        }

                    //        break;
                    //    }

                    //case "�A�C�e��":
                    //    {
                    //        if (u != null)
                    //        {
                    //            if (GeneralLib.IsNumber(@params[idx + 1]))
                    //            {
                    //                i = Conversions.ToInteger(@params[idx + 1]);
                    //                if (0 < i && i <= u.CountItem())
                    //                {
                    //                    Item localItem() { object argIndex1 = i; var ret = u.Item(argIndex1); return ret; }

                    //                    str_result = SrcFormatter.Format(localItem().Name);
                    //                }
                    //            }
                    //        }

                    //        break;
                    //    }

                    //case "�A�C�e���h�c":
                    //    {
                    //        if (u != null)
                    //        {
                    //            if (GeneralLib.IsNumber(@params[idx + 1]))
                    //            {
                    //                i = Conversions.ToInteger(@params[idx + 1]);
                    //                if (0 < i && i <= u.CountItem())
                    //                {
                    //                    Item localItem1() { object argIndex1 = i; var ret = u.Item(argIndex1); return ret; }

                    //                    str_result = SrcFormatter.Format(localItem1().ID);
                    //                }
                    //            }
                    //        }

                    //        break;
                    //    }

                    //case "�ړ��\�n�`":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = u.Transportation;
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            str_result = ud.Transportation;
                    //        }

                    //        break;
                    //    }

                    //case "�ړ���":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.Speed);
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            str_result = SrcFormatter.Format(ud.Speed);
                    //        }

                    //        break;
                    //    }

                    //case "�T�C�Y":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = u.Size;
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            str_result = ud.Size;
                    //        }

                    //        break;
                    //    }

                    //case "�C����":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = u.Value.ToString();
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            str_result = ud.Value.ToString();
                    //        }

                    //        break;
                    //    }

                    //case "�ő�g�o":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.MaxHP);
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            str_result = SrcFormatter.Format(ud.HP);
                    //        }

                    //        break;
                    //    }

                    //case "�g�o":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.HP);
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            str_result = SrcFormatter.Format(ud.HP);
                    //        }

                    //        break;
                    //    }

                    //case "�ő�d�m":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.MaxEN);
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            str_result = SrcFormatter.Format(ud.EN);
                    //        }

                    //        break;
                    //    }

                    //case "�d�m":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.EN);
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            str_result = SrcFormatter.Format(ud.EN);
                    //        }

                    //        break;
                    //    }

                    //case "���b":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.get_Armor(""));
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            str_result = SrcFormatter.Format(ud.Armor);
                    //        }

                    //        break;
                    //    }

                    //case "�^����":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.get_Mobility(""));
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            str_result = SrcFormatter.Format(ud.Mobility);
                    //        }

                    //        break;
                    //    }

                    //case "���퐔":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.CountWeapon());
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            str_result = SrcFormatter.Format(ud.CountWeapon());
                    //        }
                    //        else if (p != null)
                    //        {
                    //            str_result = SrcFormatter.Format(p.Data.CountWeapon());
                    //        }
                    //        else if (pd != null)
                    //        {
                    //            str_result = SrcFormatter.Format(pd.CountWeapon());
                    //        }
                    //        else if (it != null)
                    //        {
                    //            str_result = SrcFormatter.Format(it.CountWeapon());
                    //        }
                    //        else if (itd != null)
                    //        {
                    //            str_result = SrcFormatter.Format(itd.CountWeapon());
                    //        }

                    //        break;
                    //    }

                    //case "����":
                    //    {
                    //        idx = (idx + 1);
                    //        if (u != null)
                    //        {
                    //            {
                    //                var withBlock7 = u;
                    //                // ���Ԗڂ̕��킩�𔻒�
                    //                if (GeneralLib.IsNumber(@params[idx]))
                    //                {
                    //                    i = Conversions.ToInteger(@params[idx]);
                    //                }
                    //                else
                    //                {
                    //                    var loopTo8 = withBlock7.CountWeapon();
                    //                    for (i = 1; i <= loopTo8; i++)
                    //                    {
                    //                        if ((@params[idx] ?? "") == (withBlock7.Weapon(i).Name ?? ""))
                    //                        {
                    //                            break;
                    //                        }
                    //                    }
                    //                }
                    //                // �w�肵������������Ă��Ȃ�
                    //                if (i <= 0 || withBlock7.CountWeapon() < i)
                    //                {
                    //                    return str_result;
                    //                }

                    //                idx = (idx + 1);
                    //                switch (@params[idx] ?? "")
                    //                {
                    //                    case var case1 when case1 == "":
                    //                    case "����":
                    //                        {
                    //                            str_result = withBlock7.Weapon(i).Name;
                    //                            break;
                    //                        }

                    //                    case "�U����":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock7.WeaponPower(i, ""));
                    //                            break;
                    //                        }

                    //                    case "�˒�":
                    //                    case "�ő�˒�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock7.WeaponMaxRange(i));
                    //                            break;
                    //                        }

                    //                    case "�ŏ��˒�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock7.Weapon(i).MinRange);
                    //                            break;
                    //                        }

                    //                    case "������":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock7.WeaponPrecision(i));
                    //                            break;
                    //                        }

                    //                    case "�ő�e��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock7.MaxBullet(i));
                    //                            break;
                    //                        }

                    //                    case "�e��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock7.Bullet(i));
                    //                            break;
                    //                        }

                    //                    case "����d�m":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock7.WeaponENConsumption(i));
                    //                            break;
                    //                        }

                    //                    case "�K�v�C��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock7.Weapon(i).NecessaryMorale);
                    //                            break;
                    //                        }

                    //                    case "�n�`�K��":
                    //                        {
                    //                            str_result = withBlock7.Weapon(i).Adaption;
                    //                            break;
                    //                        }

                    //                    case "�N���e�B�J����":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock7.WeaponCritical(i));
                    //                            break;
                    //                        }

                    //                    case "����":
                    //                        {
                    //                            str_result = withBlock7.WeaponClass(i);
                    //                            break;
                    //                        }

                    //                    case "�������L":
                    //                        {
                    //                            if (withBlock7.IsWeaponClassifiedAs(i, @params[idx + 1]))
                    //                            {
                    //                                str_result = "1";
                    //                            }
                    //                            else
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "�������x��":
                    //                        {
                    //                            str_result = withBlock7.WeaponLevel(i, @params[idx + 1]).ToString();
                    //                            break;
                    //                        }

                    //                    case "��������":
                    //                        {
                    //                            str_result = Help.AttributeName(u, @params[idx + 1], false);
                    //                            break;
                    //                        }

                    //                    case "�������":
                    //                        {
                    //                            str_result = Help.AttributeHelpMessage(u, @params[idx + 1], i, false);
                    //                            break;
                    //                        }

                    //                    case "�K�v�Z�\":
                    //                        {
                    //                            str_result = withBlock7.Weapon(i).NecessarySkill;
                    //                            break;
                    //                        }

                    //                    case "�g�p��":
                    //                        {
                    //                            if (withBlock7.IsWeaponAvailable(i, "�X�e�[�^�X"))
                    //                            {
                    //                                str_result = "1";
                    //                            }
                    //                            else
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "�C��":
                    //                        {
                    //                            if (withBlock7.IsWeaponMastered(i))
                    //                            {
                    //                                str_result = "1";
                    //                            }
                    //                            else
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }
                    //                }
                    //            }
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            // ���Ԗڂ̕��킩�𔻒�
                    //            if (GeneralLib.IsNumber(@params[idx]))
                    //            {
                    //                i = Conversions.ToInteger(@params[idx]);
                    //            }
                    //            else
                    //            {
                    //                var loopTo9 = ud.CountWeapon();
                    //                for (i = 1; i <= loopTo9; i++)
                    //                {
                    //                    WeaponData localWeapon() { object argIndex1 = i; var ret = ud.Weapon(argIndex1); return ret; }

                    //                    if ((@params[idx] ?? "") == (localWeapon().Name ?? ""))
                    //                    {
                    //                        break;
                    //                    }
                    //                }
                    //            }
                    //            // �w�肵������������Ă��Ȃ�
                    //            if (i <= 0 || ud.CountWeapon() < i)
                    //            {
                    //                return str_result;
                    //            }

                    //            idx = (idx + 1);
                    //            {
                    //                var withBlock8 = ud.Weapon(i);
                    //                switch (@params[idx] ?? "")
                    //                {
                    //                    case var case2 when case2 == "":
                    //                    case "����":
                    //                        {
                    //                            str_result = withBlock8.Name;
                    //                            break;
                    //                        }

                    //                    case "�U����":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock8.Power);
                    //                            break;
                    //                        }

                    //                    case "�˒�":
                    //                    case "�ő�˒�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock8.MaxRange);
                    //                            break;
                    //                        }

                    //                    case "�ŏ��˒�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock8.MinRange);
                    //                            break;
                    //                        }

                    //                    case "������":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock8.Precision);
                    //                            break;
                    //                        }

                    //                    case "�ő�e��":
                    //                    case "�e��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock8.Bullet);
                    //                            break;
                    //                        }

                    //                    case "����d�m":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock8.ENConsumption);
                    //                            break;
                    //                        }

                    //                    case "�K�v�C��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock8.NecessaryMorale);
                    //                            break;
                    //                        }

                    //                    case "�n�`�K��":
                    //                        {
                    //                            str_result = withBlock8.Adaption;
                    //                            break;
                    //                        }

                    //                    case "�N���e�B�J����":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock8.Critical);
                    //                            break;
                    //                        }

                    //                    case "����":
                    //                        {
                    //                            str_result = withBlock8.Class;
                    //                            break;
                    //                        }

                    //                    case "�������L":
                    //                        {
                    //                            if (GeneralLib.InStrNotNest(withBlock8.Class, @params[idx + 1]) > 0)
                    //                            {
                    //                                str_result = "1";
                    //                            }
                    //                            else
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "�������x��":
                    //                        {
                    //                            j = GeneralLib.InStrNotNest(withBlock8.Class, @params[idx + 1] + "L");
                    //                            if (j == 0)
                    //                            {
                    //                                str_result = "0";
                    //                                return str_result;
                    //                            }

                    //                            str_result = "";
                    //                            j = (j + Strings.Len(@params[idx + 1]) + 1);
                    //                            do
                    //                            {
                    //                                str_result = str_result + Strings.Mid(withBlock8.Class, j, 1);
                    //                                j = (j + 1);
                    //                            }
                    //                            while (GeneralLib.IsNumber(Strings.Mid(withBlock8.Class, j, 1)));
                    //                            if (!GeneralLib.IsNumber(str_result))
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "�K�v�Z�\":
                    //                        {
                    //                            str_result = withBlock8.NecessarySkill;
                    //                            break;
                    //                        }

                    //                    case "�g�p��":
                    //                    case "�C��":
                    //                        {
                    //                            str_result = "1";
                    //                            break;
                    //                        }
                    //                }
                    //            }
                    //        }
                    //        else if (p != null)
                    //        {
                    //            {
                    //                var withBlock9 = p.Data;
                    //                // ���Ԗڂ̕��킩�𔻒�
                    //                if (GeneralLib.IsNumber(@params[idx]))
                    //                {
                    //                    i = Conversions.ToInteger(@params[idx]);
                    //                }
                    //                else
                    //                {
                    //                    var loopTo10 = withBlock9.CountWeapon();
                    //                    for (i = 1; i <= loopTo10; i++)
                    //                    {
                    //                        WeaponData localWeapon1() { object argIndex1 = i; var ret = withBlock9.Weapon(argIndex1); return ret; }

                    //                        if ((@params[idx] ?? "") == (localWeapon1().Name ?? ""))
                    //                        {
                    //                            break;
                    //                        }
                    //                    }
                    //                }
                    //                // �w�肵������������Ă��Ȃ�
                    //                if (i <= 0 || withBlock9.CountWeapon() < i)
                    //                {
                    //                    return str_result;
                    //                }

                    //                idx = (idx + 1);
                    //                {
                    //                    var withBlock10 = withBlock9.Weapon(i);
                    //                    switch (@params[idx] ?? "")
                    //                    {
                    //                        case var case3 when case3 == "":
                    //                        case "����":
                    //                            {
                    //                                str_result = withBlock10.Name;
                    //                                break;
                    //                            }

                    //                        case "�U����":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock10.Power);
                    //                                break;
                    //                            }

                    //                        case "�˒�":
                    //                        case "�ő�˒�":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock10.MaxRange);
                    //                                break;
                    //                            }

                    //                        case "�ŏ��˒�":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock10.MinRange);
                    //                                break;
                    //                            }

                    //                        case "������":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock10.Precision);
                    //                                break;
                    //                            }

                    //                        case "�ő�e��":
                    //                        case "�e��":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock10.Bullet);
                    //                                break;
                    //                            }

                    //                        case "����d�m":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock10.ENConsumption);
                    //                                break;
                    //                            }

                    //                        case "�K�v�C��":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock10.NecessaryMorale);
                    //                                break;
                    //                            }

                    //                        case "�n�`�K��":
                    //                            {
                    //                                str_result = withBlock10.Adaption;
                    //                                break;
                    //                            }

                    //                        case "�N���e�B�J����":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock10.Critical);
                    //                                break;
                    //                            }

                    //                        case "����":
                    //                            {
                    //                                str_result = withBlock10.Class;
                    //                                break;
                    //                            }

                    //                        case "�������L":
                    //                            {
                    //                                if (GeneralLib.InStrNotNest(withBlock10.Class, @params[idx + 1]) > 0)
                    //                                {
                    //                                    str_result = "1";
                    //                                }
                    //                                else
                    //                                {
                    //                                    str_result = "0";
                    //                                }

                    //                                break;
                    //                            }

                    //                        case "�������x��":
                    //                            {
                    //                                j = GeneralLib.InStrNotNest(withBlock10.Class, @params[idx + 1] + "L");
                    //                                if (j == 0)
                    //                                {
                    //                                    str_result = "0";
                    //                                    return str_result;
                    //                                }

                    //                                str_result = "";
                    //                                j = (j + Strings.Len(@params[idx + 1]) + 1);
                    //                                do
                    //                                {
                    //                                    str_result = str_result + Strings.Mid(withBlock10.Class, j, 1);
                    //                                    j = (j + 1);
                    //                                }
                    //                                while (GeneralLib.IsNumber(Strings.Mid(withBlock10.Class, j, 1)));
                    //                                if (!GeneralLib.IsNumber(str_result))
                    //                                {
                    //                                    str_result = "0";
                    //                                }

                    //                                break;
                    //                            }

                    //                        case "�K�v�Z�\":
                    //                            {
                    //                                str_result = withBlock10.NecessarySkill;
                    //                                break;
                    //                            }

                    //                        case "�g�p��":
                    //                        case "�C��":
                    //                            {
                    //                                str_result = "1";
                    //                                break;
                    //                            }
                    //                    }
                    //                }
                    //            }
                    //        }
                    //        else if (pd != null)
                    //        {
                    //            // ���Ԗڂ̕��킩�𔻒�
                    //            if (GeneralLib.IsNumber(@params[idx]))
                    //            {
                    //                i = Conversions.ToInteger(@params[idx]);
                    //            }
                    //            else
                    //            {
                    //                var loopTo11 = pd.CountWeapon();
                    //                for (i = 1; i <= loopTo11; i++)
                    //                {
                    //                    WeaponData localWeapon2() { object argIndex1 = i; var ret = pd.Weapon(argIndex1); return ret; }

                    //                    if ((@params[idx] ?? "") == (localWeapon2().Name ?? ""))
                    //                    {
                    //                        break;
                    //                    }
                    //                }
                    //            }
                    //            // �w�肵������������Ă��Ȃ�
                    //            if (i <= 0 || pd.CountWeapon() < i)
                    //            {
                    //                return str_result;
                    //            }

                    //            idx = (idx + 1);
                    //            {
                    //                var withBlock11 = pd.Weapon(i);
                    //                switch (@params[idx] ?? "")
                    //                {
                    //                    case var case4 when case4 == "":
                    //                    case "����":
                    //                        {
                    //                            str_result = withBlock11.Name;
                    //                            break;
                    //                        }

                    //                    case "�U����":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock11.Power);
                    //                            break;
                    //                        }

                    //                    case "�˒�":
                    //                    case "�ő�˒�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock11.MaxRange);
                    //                            break;
                    //                        }

                    //                    case "�ŏ��˒�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock11.MinRange);
                    //                            break;
                    //                        }

                    //                    case "������":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock11.Precision);
                    //                            break;
                    //                        }

                    //                    case "�ő�e��":
                    //                    case "�e��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock11.Bullet);
                    //                            break;
                    //                        }

                    //                    case "����d�m":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock11.ENConsumption);
                    //                            break;
                    //                        }

                    //                    case "�K�v�C��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock11.NecessaryMorale);
                    //                            break;
                    //                        }

                    //                    case "�n�`�K��":
                    //                        {
                    //                            str_result = withBlock11.Adaption;
                    //                            break;
                    //                        }

                    //                    case "�N���e�B�J����":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock11.Critical);
                    //                            break;
                    //                        }

                    //                    case "����":
                    //                        {
                    //                            str_result = withBlock11.Class;
                    //                            break;
                    //                        }

                    //                    case "�������L":
                    //                        {
                    //                            if (GeneralLib.InStrNotNest(withBlock11.Class, @params[idx + 1]) > 0)
                    //                            {
                    //                                str_result = "1";
                    //                            }
                    //                            else
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "�������x��":
                    //                        {
                    //                            j = GeneralLib.InStrNotNest(withBlock11.Class, @params[idx + 1] + "L");
                    //                            if (j == 0)
                    //                            {
                    //                                str_result = "0";
                    //                                return str_result;
                    //                            }

                    //                            str_result = "";
                    //                            j = (j + Strings.Len(@params[idx + 1]) + 1);
                    //                            do
                    //                            {
                    //                                str_result = str_result + Strings.Mid(withBlock11.Class, j, 1);
                    //                                j = (j + 1);
                    //                            }
                    //                            while (GeneralLib.IsNumber(Strings.Mid(withBlock11.Class, j, 1)));
                    //                            if (!GeneralLib.IsNumber(str_result))
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "�K�v�Z�\":
                    //                        {
                    //                            str_result = withBlock11.NecessarySkill;
                    //                            break;
                    //                        }

                    //                    case "�g�p��":
                    //                    case "�C��":
                    //                        {
                    //                            str_result = "1";
                    //                            break;
                    //                        }
                    //                }
                    //            }
                    //        }
                    //        else if (it != null)
                    //        {
                    //            // ���Ԗڂ̕��킩�𔻒�
                    //            if (GeneralLib.IsNumber(@params[idx]))
                    //            {
                    //                i = Conversions.ToInteger(@params[idx]);
                    //            }
                    //            else
                    //            {
                    //                var loopTo12 = it.CountWeapon();
                    //                for (i = 1; i <= loopTo12; i++)
                    //                {
                    //                    WeaponData localWeapon3() { object argIndex1 = i; var ret = it.Weapon(argIndex1); return ret; }

                    //                    if ((@params[idx] ?? "") == (localWeapon3().Name ?? ""))
                    //                    {
                    //                        break;
                    //                    }
                    //                }
                    //            }
                    //            // �w�肵������������Ă��Ȃ�
                    //            if (i <= 0 || it.CountWeapon() < i)
                    //            {
                    //                return str_result;
                    //            }

                    //            idx = (idx + 1);
                    //            {
                    //                var withBlock12 = it.Weapon(i);
                    //                switch (@params[idx] ?? "")
                    //                {
                    //                    case var case5 when case5 == "":
                    //                    case "����":
                    //                        {
                    //                            str_result = withBlock12.Name;
                    //                            break;
                    //                        }

                    //                    case "�U����":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock12.Power);
                    //                            break;
                    //                        }

                    //                    case "�˒�":
                    //                    case "�ő�˒�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock12.MaxRange);
                    //                            break;
                    //                        }

                    //                    case "�ŏ��˒�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock12.MinRange);
                    //                            break;
                    //                        }

                    //                    case "������":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock12.Precision);
                    //                            break;
                    //                        }

                    //                    case "�ő�e��":
                    //                    case "�e��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock12.Bullet);
                    //                            break;
                    //                        }

                    //                    case "����d�m":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock12.ENConsumption);
                    //                            break;
                    //                        }

                    //                    case "�K�v�C��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock12.NecessaryMorale);
                    //                            break;
                    //                        }

                    //                    case "�n�`�K��":
                    //                        {
                    //                            str_result = withBlock12.Adaption;
                    //                            break;
                    //                        }

                    //                    case "�N���e�B�J����":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock12.Critical);
                    //                            break;
                    //                        }

                    //                    case "����":
                    //                        {
                    //                            str_result = withBlock12.Class;
                    //                            break;
                    //                        }

                    //                    case "�������L":
                    //                        {
                    //                            if (GeneralLib.InStrNotNest(withBlock12.Class, @params[idx + 1]) > 0)
                    //                            {
                    //                                str_result = "1";
                    //                            }
                    //                            else
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "�������x��":
                    //                        {
                    //                            j = GeneralLib.InStrNotNest(withBlock12.Class, @params[idx + 1] + "L");
                    //                            if (j == 0)
                    //                            {
                    //                                str_result = "0";
                    //                                return str_result;
                    //                            }

                    //                            str_result = "";
                    //                            j = (j + Strings.Len(@params[idx + 1]) + 1);
                    //                            do
                    //                            {
                    //                                str_result = str_result + Strings.Mid(withBlock12.Class, j, 1);
                    //                                j = (j + 1);
                    //                            }
                    //                            while (GeneralLib.IsNumber(Strings.Mid(withBlock12.Class, j, 1)));
                    //                            if (!GeneralLib.IsNumber(str_result))
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "�K�v�Z�\":
                    //                        {
                    //                            str_result = withBlock12.NecessarySkill;
                    //                            break;
                    //                        }

                    //                    case "�g�p��":
                    //                    case "�C��":
                    //                        {
                    //                            str_result = "1";
                    //                            break;
                    //                        }
                    //                }
                    //            }
                    //        }
                    //        else if (itd != null)
                    //        {
                    //            // ���Ԗڂ̕��킩�𔻒�
                    //            if (GeneralLib.IsNumber(@params[idx]))
                    //            {
                    //                i = Conversions.ToInteger(@params[idx]);
                    //            }
                    //            else
                    //            {
                    //                var loopTo13 = itd.CountWeapon();
                    //                for (i = 1; i <= loopTo13; i++)
                    //                {
                    //                    WeaponData localWeapon4() { object argIndex1 = i; var ret = itd.Weapon(argIndex1); return ret; }

                    //                    if ((@params[idx] ?? "") == (localWeapon4().Name ?? ""))
                    //                    {
                    //                        break;
                    //                    }
                    //                }
                    //            }
                    //            // �w�肵������������Ă��Ȃ�
                    //            if (i <= 0 || itd.CountWeapon() < i)
                    //            {
                    //                return str_result;
                    //            }

                    //            idx = (idx + 1);
                    //            {
                    //                var withBlock13 = itd.Weapon(i);
                    //                switch (@params[idx] ?? "")
                    //                {
                    //                    case var case6 when case6 == "":
                    //                    case "����":
                    //                        {
                    //                            str_result = withBlock13.Name;
                    //                            break;
                    //                        }

                    //                    case "�U����":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock13.Power);
                    //                            break;
                    //                        }

                    //                    case "�˒�":
                    //                    case "�ő�˒�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock13.MaxRange);
                    //                            break;
                    //                        }

                    //                    case "�ŏ��˒�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock13.MinRange);
                    //                            break;
                    //                        }

                    //                    case "������":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock13.Precision);
                    //                            break;
                    //                        }

                    //                    case "�ő�e��":
                    //                    case "�e��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock13.Bullet);
                    //                            break;
                    //                        }

                    //                    case "����d�m":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock13.ENConsumption);
                    //                            break;
                    //                        }

                    //                    case "�K�v�C��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock13.NecessaryMorale);
                    //                            break;
                    //                        }

                    //                    case "�n�`�K��":
                    //                        {
                    //                            str_result = withBlock13.Adaption;
                    //                            break;
                    //                        }

                    //                    case "�N���e�B�J����":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock13.Critical);
                    //                            break;
                    //                        }

                    //                    case "����":
                    //                        {
                    //                            str_result = withBlock13.Class;
                    //                            break;
                    //                        }

                    //                    case "�������L":
                    //                        {
                    //                            if (GeneralLib.InStrNotNest(withBlock13.Class, @params[idx + 1]) > 0)
                    //                            {
                    //                                str_result = "1";
                    //                            }
                    //                            else
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "�������x��":
                    //                        {
                    //                            j = GeneralLib.InStrNotNest(withBlock13.Class, @params[idx + 1] + "L");
                    //                            if (j == 0)
                    //                            {
                    //                                str_result = "0";
                    //                                return str_result;
                    //                            }

                    //                            str_result = "";
                    //                            j = (j + Strings.Len(@params[idx + 1]) + 1);
                    //                            do
                    //                            {
                    //                                str_result = str_result + Strings.Mid(withBlock13.Class, j, 1);
                    //                                j = (j + 1);
                    //                            }
                    //                            while (GeneralLib.IsNumber(Strings.Mid(withBlock13.Class, j, 1)));
                    //                            if (!GeneralLib.IsNumber(str_result))
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "�K�v�Z�\":
                    //                        {
                    //                            str_result = withBlock13.NecessarySkill;
                    //                            break;
                    //                        }

                    //                    case "�g�p��":
                    //                    case "�C��":
                    //                        {
                    //                            str_result = "1";
                    //                            break;
                    //                        }
                    //                }
                    //            }
                    //        }

                    //        break;
                    //    }

                    //case "�A�r���e�B��":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.CountAbility());
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            str_result = SrcFormatter.Format(ud.CountAbility());
                    //        }
                    //        else if (p != null)
                    //        {
                    //            str_result = SrcFormatter.Format(p.Data.CountAbility());
                    //        }
                    //        else if (pd != null)
                    //        {
                    //            str_result = SrcFormatter.Format(pd.CountAbility());
                    //        }
                    //        else if (it != null)
                    //        {
                    //            str_result = SrcFormatter.Format(it.CountAbility());
                    //        }
                    //        else if (itd != null)
                    //        {
                    //            str_result = SrcFormatter.Format(itd.CountAbility());
                    //        }

                    //        break;
                    //    }

                    //case "�A�r���e�B":
                    //    {
                    //        idx = (idx + 1);
                    //        if (u != null)
                    //        {
                    //            {
                    //                var withBlock14 = u;
                    //                // ���Ԗڂ̃A�r���e�B���𔻒�
                    //                if (GeneralLib.IsNumber(@params[idx]))
                    //                {
                    //                    i = Conversions.ToInteger(@params[idx]);
                    //                }
                    //                else
                    //                {
                    //                    var loopTo14 = withBlock14.CountAbility();
                    //                    for (i = 1; i <= loopTo14; i++)
                    //                    {
                    //                        if ((@params[idx] ?? "") == (withBlock14.Ability(i).Name ?? ""))
                    //                        {
                    //                            break;
                    //                        }
                    //                    }
                    //                }
                    //                // �w�肵���A�r���e�B�������Ă��Ȃ�
                    //                if (i <= 0 || withBlock14.CountAbility() < i)
                    //                {
                    //                    return str_result;
                    //                }

                    //                idx = (idx + 1);
                    //                switch (@params[idx] ?? "")
                    //                {
                    //                    case var case7 when case7 == "":
                    //                    case "����":
                    //                        {
                    //                            str_result = withBlock14.Ability(i).Name;
                    //                            break;
                    //                        }

                    //                    case "���ʐ�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock14.Ability(i).CountEffect());
                    //                            break;
                    //                        }

                    //                    case "���ʃ^�C�v":
                    //                        {
                    //                            // ���Ԗڂ̌��ʂ��𔻒�
                    //                            if (GeneralLib.IsNumber(@params[idx + 1]))
                    //                            {
                    //                                j = Conversions.ToInteger(@params[idx + 1]);
                    //                            }

                    //                            if (j <= 0 && withBlock14.Ability(i).CountEffect() < j)
                    //                            {
                    //                                return str_result;
                    //                            }

                    //                            str_result = withBlock14.Ability(i).EffectType(j);
                    //                            break;
                    //                        }

                    //                    case "���ʃ��x��":
                    //                        {
                    //                            // ���Ԗڂ̌��ʂ��𔻒�
                    //                            if (GeneralLib.IsNumber(@params[idx + 1]))
                    //                            {
                    //                                j = Conversions.ToInteger(@params[idx + 1]);
                    //                            }

                    //                            if (j <= 0 && withBlock14.Ability(i).CountEffect() < j)
                    //                            {
                    //                                return str_result;
                    //                            }

                    //                            double localEffectLevel() { object argIndex1 = j; var ret = withBlock14.Ability(i).EffectLevel(argIndex1); return ret; }

                    //                            str_result = SrcFormatter.Format(localEffectLevel());
                    //                            break;
                    //                        }

                    //                    case "���ʃf�[�^":
                    //                        {
                    //                            // ���Ԗڂ̌��ʂ��𔻒�
                    //                            if (GeneralLib.IsNumber(@params[idx + 1]))
                    //                            {
                    //                                j = Conversions.ToInteger(@params[idx + 1]);
                    //                            }

                    //                            if (j <= 0 && withBlock14.Ability(i).CountEffect() < j)
                    //                            {
                    //                                return str_result;
                    //                            }

                    //                            str_result = withBlock14.Ability(i).EffectData(j);
                    //                            break;
                    //                        }

                    //                    case "�˒�":
                    //                    case "�ő�˒�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock14.AbilityMaxRange(i));
                    //                            break;
                    //                        }

                    //                    case "�ŏ��˒�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock14.AbilityMinRange(i));
                    //                            break;
                    //                        }

                    //                    case "�ő�g�p��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock14.MaxStock(i));
                    //                            break;
                    //                        }

                    //                    case "�g�p��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock14.Stock(i));
                    //                            break;
                    //                        }

                    //                    case "����d�m":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock14.AbilityENConsumption(i));
                    //                            break;
                    //                        }

                    //                    case "�K�v�C��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock14.Ability(i).NecessaryMorale);
                    //                            break;
                    //                        }

                    //                    case "����":
                    //                        {
                    //                            str_result = withBlock14.Ability(i).Class;
                    //                            break;
                    //                        }

                    //                    case "�������L":
                    //                        {
                    //                            if (withBlock14.IsAbilityClassifiedAs(i, @params[idx + 1]))
                    //                            {
                    //                                str_result = "1";
                    //                            }
                    //                            else
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "�������x��":
                    //                        {
                    //                            str_result = withBlock14.AbilityLevel(i, @params[idx + 1]).ToString();
                    //                            break;
                    //                        }

                    //                    case "��������":
                    //                        {
                    //                            str_result = Help.AttributeName(u, @params[idx + 1], true);
                    //                            break;
                    //                        }

                    //                    case "�������":
                    //                        {
                    //                            str_result = Help.AttributeHelpMessage(u, @params[idx + 1], i, true);
                    //                            break;
                    //                        }

                    //                    case "�K�v�Z�\":
                    //                        {
                    //                            str_result = withBlock14.Ability(i).NecessarySkill;
                    //                            break;
                    //                        }

                    //                    case "�g�p��":
                    //                        {
                    //                            if (withBlock14.IsAbilityAvailable(i, "�ړ��O"))
                    //                            {
                    //                                str_result = "1";
                    //                            }
                    //                            else
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "�C��":
                    //                        {
                    //                            if (withBlock14.IsAbilityMastered(i))
                    //                            {
                    //                                str_result = "1";
                    //                            }
                    //                            else
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }
                    //                }
                    //            }
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            // ���Ԗڂ̃A�r���e�B���𔻒�
                    //            if (GeneralLib.IsNumber(@params[idx]))
                    //            {
                    //                i = Conversions.ToInteger(@params[idx]);
                    //            }
                    //            else
                    //            {
                    //                var loopTo15 = ud.CountAbility();
                    //                for (i = 1; i <= loopTo15; i++)
                    //                {
                    //                    AbilityData localAbility() { object argIndex1 = i; var ret = ud.Ability(argIndex1); return ret; }

                    //                    if ((@params[idx] ?? "") == (localAbility().Name ?? ""))
                    //                    {
                    //                        break;
                    //                    }
                    //                }
                    //            }
                    //            // �w�肵���A�r���e�B�������Ă��Ȃ�
                    //            if (i <= 0 || ud.CountAbility() < i)
                    //            {
                    //                return str_result;
                    //            }

                    //            idx = (idx + 1);
                    //            {
                    //                var withBlock15 = ud.Ability(i);
                    //                switch (@params[idx] ?? "")
                    //                {
                    //                    case var case8 when case8 == "":
                    //                    case "����":
                    //                        {
                    //                            str_result = withBlock15.Name;
                    //                            break;
                    //                        }

                    //                    case "���ʐ�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock15.CountEffect());
                    //                            break;
                    //                        }

                    //                    case "���ʃ^�C�v":
                    //                        {
                    //                            // ���Ԗڂ̌��ʂ��𔻒�
                    //                            if (GeneralLib.IsNumber(@params[idx + 1]))
                    //                            {
                    //                                j = Conversions.ToInteger(@params[idx + 1]);
                    //                            }

                    //                            if (j <= 0 || withBlock15.CountEffect() < j)
                    //                            {
                    //                                return str_result;
                    //                            }

                    //                            str_result = withBlock15.EffectType(j);
                    //                            break;
                    //                        }

                    //                    case "���ʃ��x��":
                    //                        {
                    //                            // ���Ԗڂ̌��ʂ��𔻒�
                    //                            if (GeneralLib.IsNumber(@params[idx + 1]))
                    //                            {
                    //                                j = Conversions.ToInteger(@params[idx + 1]);
                    //                            }

                    //                            if (j <= 0 || withBlock15.CountEffect() < j)
                    //                            {
                    //                                return str_result;
                    //                            }

                    //                            double localEffectLevel1() { object argIndex1 = j; var ret = withBlock15.EffectLevel(argIndex1); return ret; }

                    //                            str_result = SrcFormatter.Format(localEffectLevel1());
                    //                            break;
                    //                        }

                    //                    case "���ʃf�[�^":
                    //                        {
                    //                            // ���Ԗڂ̌��ʂ��𔻒�
                    //                            if (GeneralLib.IsNumber(@params[idx + 1]))
                    //                            {
                    //                                j = Conversions.ToInteger(@params[idx + 1]);
                    //                            }

                    //                            if (j <= 0 || withBlock15.CountEffect() < j)
                    //                            {
                    //                                return str_result;
                    //                            }

                    //                            str_result = withBlock15.EffectData(j);
                    //                            break;
                    //                        }

                    //                    case "�˒�":
                    //                    case "�ő�˒�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock15.MaxRange);
                    //                            break;
                    //                        }

                    //                    case "�ŏ��˒�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock15.MinRange);
                    //                            break;
                    //                        }

                    //                    case "�ő�g�p��":
                    //                    case "�g�p��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock15.Stock);
                    //                            break;
                    //                        }

                    //                    case "����d�m":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock15.ENConsumption);
                    //                            break;
                    //                        }

                    //                    case "�K�v�C��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock15.NecessaryMorale);
                    //                            break;
                    //                        }

                    //                    case "����":
                    //                        {
                    //                            str_result = withBlock15.Class;
                    //                            break;
                    //                        }

                    //                    case "�������L":
                    //                        {
                    //                            if (GeneralLib.InStrNotNest(withBlock15.Class, @params[idx + 1]) > 0)
                    //                            {
                    //                                str_result = "1";
                    //                            }
                    //                            else
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "�������x��":
                    //                        {
                    //                            j = GeneralLib.InStrNotNest(withBlock15.Class, @params[idx + 1] + "L");
                    //                            if (j == 0)
                    //                            {
                    //                                str_result = "0";
                    //                                return str_result;
                    //                            }

                    //                            str_result = "";
                    //                            j = (j + Strings.Len(@params[idx + 1]) + 1);
                    //                            do
                    //                            {
                    //                                str_result = str_result + Strings.Mid(withBlock15.Class, j, 1);
                    //                                j = (j + 1);
                    //                            }
                    //                            while (GeneralLib.IsNumber(Strings.Mid(withBlock15.Class, j, 1)));
                    //                            if (!GeneralLib.IsNumber(str_result))
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "�K�v�Z�\":
                    //                        {
                    //                            str_result = withBlock15.NecessarySkill;
                    //                            break;
                    //                        }

                    //                    case "�g�p��":
                    //                    case "�C��":
                    //                        {
                    //                            str_result = "1";
                    //                            break;
                    //                        }
                    //                }
                    //            }
                    //        }
                    //        else if (p != null)
                    //        {
                    //            {
                    //                var withBlock16 = p.Data;
                    //                // ���Ԗڂ̃A�r���e�B���𔻒�
                    //                if (GeneralLib.IsNumber(@params[idx]))
                    //                {
                    //                    i = Conversions.ToInteger(@params[idx]);
                    //                }
                    //                else
                    //                {
                    //                    var loopTo16 = withBlock16.CountAbility();
                    //                    for (i = 1; i <= loopTo16; i++)
                    //                    {
                    //                        AbilityData localAbility1() { object argIndex1 = i; var ret = withBlock16.Ability(argIndex1); return ret; }

                    //                        if ((@params[idx] ?? "") == (localAbility1().Name ?? ""))
                    //                        {
                    //                            break;
                    //                        }
                    //                    }
                    //                }
                    //                // �w�肵���A�r���e�B�������Ă��Ȃ�
                    //                if (i <= 0 || withBlock16.CountAbility() < i)
                    //                {
                    //                    return str_result;
                    //                }

                    //                idx = (idx + 1);
                    //                {
                    //                    var withBlock17 = withBlock16.Ability(i);
                    //                    switch (@params[idx] ?? "")
                    //                    {
                    //                        case var case9 when case9 == "":
                    //                        case "����":
                    //                            {
                    //                                str_result = withBlock17.Name;
                    //                                break;
                    //                            }

                    //                        case "���ʐ�":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock17.CountEffect());
                    //                                break;
                    //                            }

                    //                        case "���ʃ^�C�v":
                    //                            {
                    //                                // ���Ԗڂ̌��ʂ��𔻒�
                    //                                if (GeneralLib.IsNumber(@params[idx + 1]))
                    //                                {
                    //                                    j = Conversions.ToInteger(@params[idx + 1]);
                    //                                }

                    //                                if (j <= 0 || withBlock17.CountEffect() < j)
                    //                                {
                    //                                    return str_result;
                    //                                }

                    //                                str_result = withBlock17.EffectType(j);
                    //                                break;
                    //                            }

                    //                        case "���ʃ��x��":
                    //                            {
                    //                                // ���Ԗڂ̌��ʂ��𔻒�
                    //                                if (GeneralLib.IsNumber(@params[idx + 1]))
                    //                                {
                    //                                    j = Conversions.ToInteger(@params[idx + 1]);
                    //                                }

                    //                                if (j <= 0 || withBlock17.CountEffect() < j)
                    //                                {
                    //                                    return str_result;
                    //                                }

                    //                                double localEffectLevel2() { object argIndex1 = j; var ret = withBlock17.EffectLevel(argIndex1); return ret; }

                    //                                str_result = SrcFormatter.Format(localEffectLevel2());
                    //                                break;
                    //                            }

                    //                        case "���ʃf�[�^":
                    //                            {
                    //                                // ���Ԗڂ̌��ʂ��𔻒�
                    //                                if (GeneralLib.IsNumber(@params[idx + 1]))
                    //                                {
                    //                                    j = Conversions.ToInteger(@params[idx + 1]);
                    //                                }

                    //                                if (j <= 0 || withBlock17.CountEffect() < j)
                    //                                {
                    //                                    return str_result;
                    //                                }

                    //                                str_result = withBlock17.EffectData(j);
                    //                                break;
                    //                            }

                    //                        case "�˒�":
                    //                        case "�ő�˒�":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock17.MaxRange);
                    //                                break;
                    //                            }

                    //                        case "�ŏ��˒�":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock17.MinRange);
                    //                                break;
                    //                            }

                    //                        case "�ő�g�p��":
                    //                        case "�g�p��":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock17.Stock);
                    //                                break;
                    //                            }

                    //                        case "����d�m":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock17.ENConsumption);
                    //                                break;
                    //                            }

                    //                        case "�K�v�C��":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock17.NecessaryMorale);
                    //                                break;
                    //                            }

                    //                        case "����":
                    //                            {
                    //                                str_result = withBlock17.Class;
                    //                                break;
                    //                            }

                    //                        case "�������L":
                    //                            {
                    //                                if (GeneralLib.InStrNotNest(withBlock17.Class, @params[idx + 1]) > 0)
                    //                                {
                    //                                    str_result = "1";
                    //                                }
                    //                                else
                    //                                {
                    //                                    str_result = "0";
                    //                                }

                    //                                break;
                    //                            }

                    //                        case "�������x��":
                    //                            {
                    //                                j = GeneralLib.InStrNotNest(withBlock17.Class, @params[idx + 1] + "L");
                    //                                if (j == 0)
                    //                                {
                    //                                    str_result = "0";
                    //                                    return str_result;
                    //                                }

                    //                                str_result = "";
                    //                                j = (j + Strings.Len(@params[idx + 1]) + 1);
                    //                                do
                    //                                {
                    //                                    str_result = str_result + Strings.Mid(withBlock17.Class, j, 1);
                    //                                    j = (j + 1);
                    //                                }
                    //                                while (GeneralLib.IsNumber(Strings.Mid(withBlock17.Class, j, 1)));
                    //                                if (!GeneralLib.IsNumber(str_result))
                    //                                {
                    //                                    str_result = "0";
                    //                                }

                    //                                break;
                    //                            }

                    //                        case "�K�v�Z�\":
                    //                            {
                    //                                str_result = withBlock17.NecessarySkill;
                    //                                break;
                    //                            }

                    //                        case "�g�p��":
                    //                        case "�C��":
                    //                            {
                    //                                str_result = "1";
                    //                                break;
                    //                            }
                    //                    }
                    //                }
                    //            }
                    //        }
                    //        else if (pd != null)
                    //        {
                    //            // ���Ԗڂ̃A�r���e�B���𔻒�
                    //            if (GeneralLib.IsNumber(@params[idx]))
                    //            {
                    //                i = Conversions.ToInteger(@params[idx]);
                    //            }
                    //            else
                    //            {
                    //                var loopTo17 = pd.CountAbility();
                    //                for (i = 1; i <= loopTo17; i++)
                    //                {
                    //                    AbilityData localAbility2() { object argIndex1 = i; var ret = pd.Ability(argIndex1); return ret; }

                    //                    if ((@params[idx] ?? "") == (localAbility2().Name ?? ""))
                    //                    {
                    //                        break;
                    //                    }
                    //                }
                    //            }
                    //            // �w�肵���A�r���e�B�������Ă��Ȃ�
                    //            if (i <= 0 || pd.CountAbility() < i)
                    //            {
                    //                return str_result;
                    //            }

                    //            idx = (idx + 1);
                    //            {
                    //                var withBlock18 = pd.Ability(i);
                    //                switch (@params[idx] ?? "")
                    //                {
                    //                    case var case10 when case10 == "":
                    //                    case "����":
                    //                        {
                    //                            str_result = withBlock18.Name;
                    //                            break;
                    //                        }

                    //                    case "���ʐ�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock18.CountEffect());
                    //                            break;
                    //                        }

                    //                    case "���ʃ^�C�v":
                    //                        {
                    //                            // ���Ԗڂ̌��ʂ��𔻒�
                    //                            if (GeneralLib.IsNumber(@params[idx + 1]))
                    //                            {
                    //                                j = Conversions.ToInteger(@params[idx + 1]);
                    //                            }

                    //                            if (j <= 0 || withBlock18.CountEffect() < j)
                    //                            {
                    //                                return str_result;
                    //                            }

                    //                            str_result = withBlock18.EffectType(j);
                    //                            break;
                    //                        }

                    //                    case "���ʃ��x��":
                    //                        {
                    //                            // ���Ԗڂ̌��ʂ��𔻒�
                    //                            if (GeneralLib.IsNumber(@params[idx + 1]))
                    //                            {
                    //                                j = Conversions.ToInteger(@params[idx + 1]);
                    //                            }

                    //                            if (j <= 0 || withBlock18.CountEffect() < j)
                    //                            {
                    //                                return str_result;
                    //                            }

                    //                            double localEffectLevel3() { object argIndex1 = j; var ret = withBlock18.EffectLevel(argIndex1); return ret; }

                    //                            str_result = SrcFormatter.Format(localEffectLevel3());
                    //                            break;
                    //                        }

                    //                    case "���ʃf�[�^":
                    //                        {
                    //                            // ���Ԗڂ̌��ʂ��𔻒�
                    //                            if (GeneralLib.IsNumber(@params[idx + 1]))
                    //                            {
                    //                                j = Conversions.ToInteger(@params[idx + 1]);
                    //                            }

                    //                            if (j <= 0 || withBlock18.CountEffect() < j)
                    //                            {
                    //                                return str_result;
                    //                            }

                    //                            str_result = withBlock18.EffectData(j);
                    //                            break;
                    //                        }

                    //                    case "�˒�":
                    //                    case "�ő�˒�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock18.MaxRange);
                    //                            break;
                    //                        }

                    //                    case "�ŏ��˒�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock18.MinRange);
                    //                            break;
                    //                        }

                    //                    case "�ő�g�p��":
                    //                    case "�g�p��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock18.Stock);
                    //                            break;
                    //                        }

                    //                    case "����d�m":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock18.ENConsumption);
                    //                            break;
                    //                        }

                    //                    case "�K�v�C��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock18.NecessaryMorale);
                    //                            break;
                    //                        }

                    //                    case "����":
                    //                        {
                    //                            str_result = withBlock18.Class;
                    //                            break;
                    //                        }

                    //                    case "�������L":
                    //                        {
                    //                            if (GeneralLib.InStrNotNest(withBlock18.Class, @params[idx + 1]) > 0)
                    //                            {
                    //                                str_result = "1";
                    //                            }
                    //                            else
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "�������x��":
                    //                        {
                    //                            j = GeneralLib.InStrNotNest(withBlock18.Class, @params[idx + 1] + "L");
                    //                            if (j == 0)
                    //                            {
                    //                                str_result = "0";
                    //                                return str_result;
                    //                            }

                    //                            str_result = "";
                    //                            j = (j + Strings.Len(@params[idx + 1]) + 1);
                    //                            do
                    //                            {
                    //                                str_result = str_result + Strings.Mid(withBlock18.Class, j, 1);
                    //                                j = (j + 1);
                    //                            }
                    //                            while (GeneralLib.IsNumber(Strings.Mid(withBlock18.Class, j, 1)));
                    //                            if (!GeneralLib.IsNumber(str_result))
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "�K�v�Z�\":
                    //                        {
                    //                            str_result = withBlock18.NecessarySkill;
                    //                            break;
                    //                        }

                    //                    case "�g�p��":
                    //                    case "�C��":
                    //                        {
                    //                            str_result = "1";
                    //                            break;
                    //                        }
                    //                }
                    //            }
                    //        }
                    //        else if (it != null)
                    //        {
                    //            // ���Ԗڂ̃A�r���e�B���𔻒�
                    //            if (GeneralLib.IsNumber(@params[idx]))
                    //            {
                    //                i = Conversions.ToInteger(@params[idx]);
                    //            }
                    //            else
                    //            {
                    //                var loopTo18 = it.CountAbility();
                    //                for (i = 1; i <= loopTo18; i++)
                    //                {
                    //                    AbilityData localAbility3() { object argIndex1 = i; var ret = it.Ability(argIndex1); return ret; }

                    //                    if ((@params[idx] ?? "") == (localAbility3().Name ?? ""))
                    //                    {
                    //                        break;
                    //                    }
                    //                }
                    //            }
                    //            // �w�肵���A�r���e�B�������Ă��Ȃ�
                    //            if (i <= 0 || it.CountAbility() < i)
                    //            {
                    //                return str_result;
                    //            }

                    //            idx = (idx + 1);
                    //            {
                    //                var withBlock19 = it.Ability(i);
                    //                switch (@params[idx] ?? "")
                    //                {
                    //                    case var case11 when case11 == "":
                    //                    case "����":
                    //                        {
                    //                            str_result = withBlock19.Name;
                    //                            break;
                    //                        }

                    //                    case "���ʐ�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock19.CountEffect());
                    //                            break;
                    //                        }

                    //                    case "���ʃ^�C�v":
                    //                        {
                    //                            // ���Ԗڂ̌��ʂ��𔻒�
                    //                            if (GeneralLib.IsNumber(@params[idx + 1]))
                    //                            {
                    //                                j = Conversions.ToInteger(@params[idx + 1]);
                    //                            }

                    //                            if (j <= 0 || withBlock19.CountEffect() < j)
                    //                            {
                    //                                return str_result;
                    //                            }

                    //                            str_result = withBlock19.EffectType(j);
                    //                            break;
                    //                        }

                    //                    case "���ʃ��x��":
                    //                        {
                    //                            // ���Ԗڂ̌��ʂ��𔻒�
                    //                            if (GeneralLib.IsNumber(@params[idx + 1]))
                    //                            {
                    //                                j = Conversions.ToInteger(@params[idx + 1]);
                    //                            }

                    //                            if (j <= 0 || withBlock19.CountEffect() < j)
                    //                            {
                    //                                return str_result;
                    //                            }

                    //                            double localEffectLevel4() { object argIndex1 = j; var ret = withBlock19.EffectLevel(argIndex1); return ret; }

                    //                            str_result = SrcFormatter.Format(localEffectLevel4());
                    //                            break;
                    //                        }

                    //                    case "���ʃf�[�^":
                    //                        {
                    //                            // ���Ԗڂ̌��ʂ��𔻒�
                    //                            if (GeneralLib.IsNumber(@params[idx + 1]))
                    //                            {
                    //                                j = Conversions.ToInteger(@params[idx + 1]);
                    //                            }

                    //                            if (j <= 0 || withBlock19.CountEffect() < j)
                    //                            {
                    //                                return str_result;
                    //                            }

                    //                            str_result = withBlock19.EffectData(j);
                    //                            break;
                    //                        }

                    //                    case "�˒�":
                    //                    case "�ő�˒�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock19.MaxRange);
                    //                            break;
                    //                        }

                    //                    case "�ŏ��˒�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock19.MinRange);
                    //                            break;
                    //                        }

                    //                    case "�ő�g�p��":
                    //                    case "�g�p��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock19.Stock);
                    //                            break;
                    //                        }

                    //                    case "����d�m":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock19.ENConsumption);
                    //                            break;
                    //                        }

                    //                    case "�K�v�C��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock19.NecessaryMorale);
                    //                            break;
                    //                        }

                    //                    case "����":
                    //                        {
                    //                            str_result = withBlock19.Class;
                    //                            break;
                    //                        }

                    //                    case "�������L":
                    //                        {
                    //                            if (GeneralLib.InStrNotNest(withBlock19.Class, @params[idx + 1]) > 0)
                    //                            {
                    //                                str_result = "1";
                    //                            }
                    //                            else
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "�������x��":
                    //                        {
                    //                            j = GeneralLib.InStrNotNest(withBlock19.Class, @params[idx + 1] + "L");
                    //                            if (j == 0)
                    //                            {
                    //                                str_result = "0";
                    //                                return str_result;
                    //                            }

                    //                            str_result = "";
                    //                            j = (j + Strings.Len(@params[idx + 1]) + 1);
                    //                            do
                    //                            {
                    //                                str_result = str_result + Strings.Mid(withBlock19.Class, j, 1);
                    //                                j = (j + 1);
                    //                            }
                    //                            while (GeneralLib.IsNumber(Strings.Mid(withBlock19.Class, j, 1)));
                    //                            if (!GeneralLib.IsNumber(str_result))
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "�K�v�Z�\":
                    //                        {
                    //                            str_result = withBlock19.NecessarySkill;
                    //                            break;
                    //                        }

                    //                    case "�g�p��":
                    //                    case "�C��":
                    //                        {
                    //                            str_result = "1";
                    //                            break;
                    //                        }
                    //                }
                    //            }
                    //        }
                    //        else if (itd != null)
                    //        {
                    //            // ���Ԗڂ̃A�r���e�B���𔻒�
                    //            if (GeneralLib.IsNumber(@params[idx]))
                    //            {
                    //                i = Conversions.ToInteger(@params[idx]);
                    //            }
                    //            else
                    //            {
                    //                var loopTo19 = itd.CountAbility();
                    //                for (i = 1; i <= loopTo19; i++)
                    //                {
                    //                    AbilityData localAbility4() { object argIndex1 = i; var ret = itd.Ability(argIndex1); return ret; }

                    //                    if ((@params[idx] ?? "") == (localAbility4().Name ?? ""))
                    //                    {
                    //                        break;
                    //                    }
                    //                }
                    //            }
                    //            // �w�肵���A�r���e�B�������Ă��Ȃ�
                    //            if (i <= 0 || itd.CountAbility() < i)
                    //            {
                    //                return str_result;
                    //            }

                    //            idx = (idx + 1);
                    //            {
                    //                var withBlock20 = itd.Ability(i);
                    //                switch (@params[idx] ?? "")
                    //                {
                    //                    case var case12 when case12 == "":
                    //                    case "����":
                    //                        {
                    //                            str_result = withBlock20.Name;
                    //                            break;
                    //                        }

                    //                    case "���ʐ�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock20.CountEffect());
                    //                            break;
                    //                        }

                    //                    case "���ʃ^�C�v":
                    //                        {
                    //                            // ���Ԗڂ̌��ʂ��𔻒�
                    //                            if (GeneralLib.IsNumber(@params[idx + 1]))
                    //                            {
                    //                                j = Conversions.ToInteger(@params[idx + 1]);
                    //                            }

                    //                            if (j <= 0 || withBlock20.CountEffect() < j)
                    //                            {
                    //                                return str_result;
                    //                            }

                    //                            str_result = withBlock20.EffectType(j);
                    //                            break;
                    //                        }

                    //                    case "���ʃ��x��":
                    //                        {
                    //                            // ���Ԗڂ̌��ʂ��𔻒�
                    //                            if (GeneralLib.IsNumber(@params[idx + 1]))
                    //                            {
                    //                                j = Conversions.ToInteger(@params[idx + 1]);
                    //                            }

                    //                            if (j <= 0 || withBlock20.CountEffect() < j)
                    //                            {
                    //                                return str_result;
                    //                            }

                    //                            double localEffectLevel5() { object argIndex1 = j; var ret = withBlock20.EffectLevel(argIndex1); return ret; }

                    //                            str_result = SrcFormatter.Format(localEffectLevel5());
                    //                            break;
                    //                        }

                    //                    case "���ʃf�[�^":
                    //                        {
                    //                            // ���Ԗڂ̌��ʂ��𔻒�
                    //                            if (GeneralLib.IsNumber(@params[idx + 1]))
                    //                            {
                    //                                j = Conversions.ToInteger(@params[idx + 1]);
                    //                            }

                    //                            if (j <= 0 || withBlock20.CountEffect() < j)
                    //                            {
                    //                                return str_result;
                    //                            }

                    //                            str_result = withBlock20.EffectData(j);
                    //                            break;
                    //                        }

                    //                    case "�˒�":
                    //                    case "�ő�˒�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock20.MaxRange);
                    //                            break;
                    //                        }

                    //                    case "�ŏ��˒�":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock20.MinRange);
                    //                            break;
                    //                        }

                    //                    case "�ő�g�p��":
                    //                    case "�g�p��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock20.Stock);
                    //                            break;
                    //                        }

                    //                    case "����d�m":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock20.ENConsumption);
                    //                            break;
                    //                        }

                    //                    case "�K�v�C��":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock20.NecessaryMorale);
                    //                            break;
                    //                        }

                    //                    case "����":
                    //                        {
                    //                            str_result = withBlock20.Class;
                    //                            break;
                    //                        }

                    //                    case "�������L":
                    //                        {
                    //                            if (GeneralLib.InStrNotNest(withBlock20.Class, @params[idx + 1]) > 0)
                    //                            {
                    //                                str_result = "1";
                    //                            }
                    //                            else
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "�������x��":
                    //                        {
                    //                            j = GeneralLib.InStrNotNest(withBlock20.Class, @params[idx + 1] + "L");
                    //                            if (j == 0)
                    //                            {
                    //                                str_result = "0";
                    //                                return str_result;
                    //                            }

                    //                            str_result = "";
                    //                            j = (j + Strings.Len(@params[idx + 1]) + 1);
                    //                            do
                    //                            {
                    //                                str_result = str_result + Strings.Mid(withBlock20.Class, j, 1);
                    //                                j = (j + 1);
                    //                            }
                    //                            while (GeneralLib.IsNumber(Strings.Mid(withBlock20.Class, j, 1)));
                    //                            if (!GeneralLib.IsNumber(str_result))
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "�K�v�Z�\":
                    //                        {
                    //                            str_result = withBlock20.NecessarySkill;
                    //                            break;
                    //                        }

                    //                    case "�g�p��":
                    //                    case "�C��":
                    //                        {
                    //                            str_result = "1";
                    //                            break;
                    //                        }
                    //                }
                    //            }
                    //        }

                    //        break;
                    //    }

                    //case "�����N":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.Rank);
                    //        }

                    //        break;
                    //    }

                    //case "�{�X�����N":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.BossRank);
                    //        }

                    //        break;
                    //    }

                    //case "�G���A":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = u.Area;
                    //        }

                    //        break;
                    //    }

                    //case "�v�l���[�h":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = u.Mode;
                    //        }

                    //        break;
                    //    }

                    //case "�ő�U����":
                    //    {
                    //        if (u != null)
                    //        {
                    //            {
                    //                var withBlock21 = u;
                    //                max_value = 0;
                    //                var loopTo20 = withBlock21.CountWeapon();
                    //                for (i = 1; i <= loopTo20; i++)
                    //                {
                    //                    if (withBlock21.IsWeaponMastered(i) && !withBlock21.IsDisabled(withBlock21.Weapon(i).Name) && !withBlock21.IsWeaponClassifiedAs(i, "��"))
                    //                    {
                    //                        if (withBlock21.WeaponPower(i, "") > max_value)
                    //                        {
                    //                            max_value = withBlock21.WeaponPower(i, "");
                    //                        }
                    //                    }
                    //                }

                    //                str_result = SrcFormatter.Format(max_value);
                    //            }
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            max_value = 0;
                    //            var loopTo21 = ud.CountWeapon();
                    //            for (i = 1; i <= loopTo21; i++)
                    //            {
                    //                WeaponData localWeapon7() { object argIndex1 = i; var ret = ud.Weapon(argIndex1); return ret; }

                    //                if (Strings.InStr(localWeapon7().Class, "��") == 0)
                    //                {
                    //                    WeaponData localWeapon6() { object argIndex1 = i; var ret = ud.Weapon(argIndex1); return ret; }

                    //                    if (localWeapon6().Power > max_value)
                    //                    {
                    //                        WeaponData localWeapon5() { object argIndex1 = i; var ret = ud.Weapon(argIndex1); return ret; }

                    //                        max_value = localWeapon5().Power;
                    //                    }
                    //                }
                    //            }

                    //            str_result = SrcFormatter.Format(max_value);
                    //        }

                    //        break;
                    //    }

                    //case "�Œ��˒�":
                    //    {
                    //        if (u != null)
                    //        {
                    //            {
                    //                var withBlock22 = u;
                    //                max_value = 0;
                    //                var loopTo22 = withBlock22.CountWeapon();
                    //                for (i = 1; i <= loopTo22; i++)
                    //                {
                    //                    if (withBlock22.IsWeaponMastered(i) && !withBlock22.IsDisabled(withBlock22.Weapon(i).Name) && !withBlock22.IsWeaponClassifiedAs(i, "��"))
                    //                    {
                    //                        if (withBlock22.WeaponMaxRange(i) > max_value)
                    //                        {
                    //                            max_value = withBlock22.WeaponMaxRange(i);
                    //                        }
                    //                    }
                    //                }

                    //                str_result = SrcFormatter.Format(max_value);
                    //            }
                    //        }
                    //        else if (ud != null)
                    //        {
                    //            max_value = 0;
                    //            var loopTo23 = ud.CountWeapon();
                    //            for (i = 1; i <= loopTo23; i++)
                    //            {
                    //                WeaponData localWeapon10() { object argIndex1 = i; var ret = ud.Weapon(argIndex1); return ret; }

                    //                if (Strings.InStr(localWeapon10().Class, "��") == 0)
                    //                {
                    //                    WeaponData localWeapon9() { object argIndex1 = i; var ret = ud.Weapon(argIndex1); return ret; }

                    //                    if (localWeapon9().MaxRange > max_value)
                    //                    {
                    //                        WeaponData localWeapon8() { object argIndex1 = i; var ret = ud.Weapon(argIndex1); return ret; }

                    //                        max_value = localWeapon8().MaxRange;
                    //                    }
                    //                }
                    //            }

                    //            str_result = SrcFormatter.Format(max_value);
                    //        }

                    //        break;
                    //    }

                    //case "�c��T�|�[�g�A�^�b�N��":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.MaxSupportAttack() - u.UsedSupportAttack);
                    //        }

                    //        break;
                    //    }

                    //case "�c��T�|�[�g�K�[�h��":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.MaxSupportGuard() - u.UsedSupportGuard);
                    //        }

                    //        break;
                    //    }

                    //case "�c�蓯������U����":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.MaxSyncAttack() - u.UsedSyncAttack);
                    //        }

                    //        break;
                    //    }

                    //case "�c��J�E���^�[�U����":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.MaxCounterAttack() - u.UsedCounterAttack);
                    //        }

                    //        break;
                    //    }

                    //case "������":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(InterMission.RankUpCost(u));
                    //        }

                    //        break;
                    //    }

                    //case "�ő������":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(InterMission.MaxRank(u));
                    //        }

                    //        break;
                    //    }

                    //case "�A�C�e���N���X":
                    //    {
                    //        if (it != null)
                    //        {
                    //            str_result = it.Class();
                    //        }
                    //        else if (itd != null)
                    //        {
                    //            str_result = itd.Class;
                    //        }

                    //        break;
                    //    }

                    //case "������":
                    //    {
                    //        if (it != null)
                    //        {
                    //            str_result = it.Part();
                    //        }
                    //        else if (itd != null)
                    //        {
                    //            str_result = itd.Part;
                    //        }

                    //        break;
                    //    }

                    //case "�ő�g�o�C���l":
                    //    {
                    //        if (it != null)
                    //        {
                    //            str_result = SrcFormatter.Format(it.HP());
                    //        }
                    //        else if (itd != null)
                    //        {
                    //            str_result = SrcFormatter.Format(itd.HP);
                    //        }

                    //        break;
                    //    }

                    //case "�ő�d�m�C���l":
                    //    {
                    //        if (it != null)
                    //        {
                    //            str_result = SrcFormatter.Format(it.EN());
                    //        }
                    //        else if (itd != null)
                    //        {
                    //            str_result = SrcFormatter.Format(itd.EN);
                    //        }

                    //        break;
                    //    }

                    //case "���b�C���l":
                    //    {
                    //        if (it != null)
                    //        {
                    //            str_result = SrcFormatter.Format(it.Armor());
                    //        }
                    //        else if (itd != null)
                    //        {
                    //            str_result = SrcFormatter.Format(itd.Armor);
                    //        }

                    //        break;
                    //    }

                    //case "�^�����C���l":
                    //    {
                    //        if (it != null)
                    //        {
                    //            str_result = SrcFormatter.Format(it.Mobility());
                    //        }
                    //        else if (itd != null)
                    //        {
                    //            str_result = SrcFormatter.Format(itd.Mobility);
                    //        }

                    //        break;
                    //    }

                    //case "�ړ��͏C���l":
                    //    {
                    //        if (it != null)
                    //        {
                    //            str_result = SrcFormatter.Format(it.Speed());
                    //        }
                    //        else if (itd != null)
                    //        {
                    //            str_result = SrcFormatter.Format(itd.Speed);
                    //        }

                    //        break;
                    //    }

                    //case "�����":
                    //case "�R�����g":
                    //    {
                    //        if (it != null)
                    //        {
                    //            str_result = it.Data.Comment;
                    //            GeneralLib.ReplaceString(str_result, Constants.vbCr + Constants.vbLf, " ");
                    //        }
                    //        else if (itd != null)
                    //        {
                    //            str_result = itd.Comment;
                    //            GeneralLib.ReplaceString(str_result, Constants.vbCr + Constants.vbLf, " ");
                    //        }
                    //        else if (spd != null)
                    //        {
                    //            str_result = spd.Comment;
                    //        }

                    //        break;
                    //    }

                    //case "�Z�k��":
                    //    {
                    //        if (spd != null)
                    //        {
                    //            str_result = spd.ShortName;
                    //        }

                    //        break;
                    //    }

                    //case "����r�o":
                    //    {
                    //        if (spd != null)
                    //        {
                    //            str_result = SrcFormatter.Format(spd.SPConsumption);
                    //        }

                    //        break;
                    //    }

                    //case "�Ώ�":
                    //    {
                    //        if (spd != null)
                    //        {
                    //            str_result = spd.TargetType;
                    //        }

                    //        break;
                    //    }

                    //case "��������":
                    //    {
                    //        if (spd != null)
                    //        {
                    //            str_result = spd.Duration;
                    //        }

                    //        break;
                    //    }

                    //case "�K�p����":
                    //    {
                    //        if (spd != null)
                    //        {
                    //            str_result = spd.NecessaryCondition;
                    //        }

                    //        break;
                    //    }

                    //case "�A�j��":
                    //    {
                    //        if (spd != null)
                    //        {
                    //            str_result = spd.Animation;
                    //        }

                    //        break;
                    //    }

                    //case "���ʐ�":
                    //    {
                    //        if (spd != null)
                    //        {
                    //            str_result = SrcFormatter.Format(spd.CountEffect());
                    //        }

                    //        break;
                    //    }

                    //case "���ʃ^�C�v":
                    //    {
                    //        if (spd != null)
                    //        {
                    //            idx = (idx + 1);
                    //            i = GeneralLib.StrToLng(@params[idx]);
                    //            if (1 <= i && i <= spd.CountEffect())
                    //            {
                    //                str_result = spd.EffectType(i);
                    //            }
                    //        }

                    //        break;
                    //    }

                    //case "���ʃ��x��":
                    //    {
                    //        if (spd != null)
                    //        {
                    //            idx = (idx + 1);
                    //            i = GeneralLib.StrToLng(@params[idx]);
                    //            if (1 <= i && i <= spd.CountEffect())
                    //            {
                    //                str_result = SrcFormatter.Format(spd.EffectLevel(i));
                    //            }
                    //        }

                    //        break;
                    //    }

                    //case "���ʃf�[�^":
                    //    {
                    //        if (spd != null)
                    //        {
                    //            idx = (idx + 1);
                    //            i = GeneralLib.StrToLng(@params[idx]);
                    //            if (1 <= i && i <= spd.CountEffect())
                    //            {
                    //                str_result = spd.EffectData(i);
                    //            }
                    //        }

                    //        break;
                    //    }

                    //case "�}�b�v":
                    //    {
                    //        idx = (idx + 1);
                    //        switch (@params[idx] ?? "")
                    //        {
                    //            case "�t�@�C����":
                    //                {
                    //                    str_result = SRC.Map.MapFileName;
                    //                    if (Strings.Len(str_result) > Strings.Len(SRC.ScenarioPath))
                    //                    {
                    //                        if ((Strings.Left(str_result, Strings.Len(SRC.ScenarioPath)) ?? "") == (SRC.ScenarioPath ?? ""))
                    //                        {
                    //                            str_result = Strings.Mid(str_result, Strings.Len(SRC.ScenarioPath) + 1);
                    //                        }
                    //                    }

                    //                    break;
                    //                }

                    //            case "��":
                    //                {
                    //                    str_result = SrcFormatter.Format(SRC.Map.MapWidth);
                    //                    break;
                    //                }

                    //            case "���ԑ�":
                    //                {
                    //                    if (!string.IsNullOrEmpty(SRC.Map.MapDrawMode))
                    //                    {
                    //                        if (SRC.Map.MapDrawMode == "�t�B���^")
                    //                        {
                    //                            buf = Conversion.Hex(SRC.Map.MapDrawFilterColor);
                    //                            var loopTo24 = (6 - Strings.Len(buf));
                    //                            for (i = 1; i <= loopTo24; i++)
                    //                                buf = "0" + buf;
                    //                            buf = "#" + Strings.Mid(buf, 5, 2) + Strings.Mid(buf, 3, 2) + Strings.Mid(buf, 1, 2) + " " + (SRC.Map.MapDrawFilterTransPercent * 100d).ToString() + "%";
                    //                        }
                    //                        else
                    //                        {
                    //                            buf = SRC.Map.MapDrawMode;
                    //                        }

                    //                        if (SRC.Map.MapDrawIsMapOnly)
                    //                        {
                    //                            buf = buf + " �}�b�v����";
                    //                        }

                    //                        str_result = buf;
                    //                    }
                    //                    else
                    //                    {
                    //                        str_result = "��";
                    //                    }

                    //                    break;
                    //                }

                    //            case "����":
                    //                {
                    //                    str_result = SrcFormatter.Format(SRC.Map.MapHeight);
                    //                    break;
                    //                }

                    //            default:
                    //                {
                    //                    if (GeneralLib.IsNumber(@params[idx]))
                    //                    {
                    //                        mx = Conversions.ToInteger(@params[idx]);
                    //                    }

                    //                    idx = (idx + 1);
                    //                    if (GeneralLib.IsNumber(@params[idx]))
                    //                    {
                    //                        my = Conversions.ToInteger(@params[idx]);
                    //                    }

                    //                    if (mx < 1 || SRC.Map.MapWidth < mx || my < 1 || SRC.Map.MapHeight < my)
                    //                    {
                    //                        return str_result;
                    //                    }

                    //                    idx = (idx + 1);
                    //                    switch (@params[idx] ?? "")
                    //                    {
                    //                        case "�n�`��":
                    //                            {
                    //                                str_result = SRC.Map.TerrainName(mx, my);
                    //                                break;
                    //                            }

                    //                        case "�n�`�^�C�v":
                    //                        case "�n�`�N���X":
                    //                            {
                    //                                str_result = SRC.Map.TerrainClass(mx, my);
                    //                                break;
                    //                            }

                    //                        case "�ړ��R�X�g":
                    //                            {
                    //                                // 0.5���݂̈ړ��R�X�g���g����悤�ɂ��邽�߁A�ړ��R�X�g��
                    //                                // ���ۂ̂Q�{�̒l�ŋL�^����Ă���
                    //                                str_result = SrcFormatter.Format(SRC.Map.TerrainMoveCost(mx, my) / 2d);
                    //                                break;
                    //                            }

                    //                        case "����C��":
                    //                            {
                    //                                str_result = SrcFormatter.Format(SRC.Map.TerrainEffectForHit(mx, my));
                    //                                break;
                    //                            }

                    //                        case "�_���[�W�C��":
                    //                            {
                    //                                str_result = SrcFormatter.Format(SRC.Map.TerrainEffectForDamage(mx, my));
                    //                                break;
                    //                            }

                    //                        case "�g�o�񕜗�":
                    //                            {
                    //                                str_result = SrcFormatter.Format(SRC.Map.TerrainEffectForHPRecover(mx, my));
                    //                                break;
                    //                            }

                    //                        case "�d�m�񕜗�":
                    //                            {
                    //                                str_result = SrcFormatter.Format(SRC.Map.TerrainEffectForENRecover(mx, my));
                    //                                break;
                    //                            }

                    //                        case "�r�b�g�}�b�v��":
                    //                            {
                    //                                // MOD START 240a
                    //                                // Select Case MapImageFileTypeData(mx, my)
                    //                                // Case SeparateDirMapImageFileType
                    //                                // EvalInfoFunc = _
                    //                                // '                                        TDList.Bitmap(MapData(mx, my, 0)) && "\" && _
                    //                                // '                                        TDList.Bitmap(MapData(mx, my, 0)) && _
                    //                                // '                                        Format$(MapData(mx, my, 1), "0000") && ".bmp"
                    //                                // Case FourFiguresMapImageFileType
                    //                                // EvalInfoFunc = _
                    //                                // '                                        TDList.Bitmap(MapData(mx, my, 0)) && _
                    //                                // '                                        Format$(MapData(mx, my, 1), "0000") && ".bmp"
                    //                                // Case OldMapImageFileType
                    //                                // EvalInfoFunc = _
                    //                                // '                                        TDList.Bitmap(MapData(mx, my, 0)) && _
                    //                                // '                                        Format$(MapData(mx, my, 1)) && ".bmp"
                    //                                // End Select
                    //                                switch (SRC.Map.MapImageFileTypeData[mx, my])
                    //                                {
                    //                                    case SRC.Map.MapImageFileType.SeparateDirMapImageFileType:
                    //                                        {
                    //                                            str_result = SRC.TDList.Bitmap(SRC.Map.MapData[mx, my, SRC.Map.MapDataIndex.TerrainType]) + @"\" + SRC.TDList.Bitmap(SRC.Map.MapData[mx, my, SRC.Map.MapDataIndex.TerrainType]) + SrcFormatter.Format(SRC.Map.MapData[mx, my, SRC.Map.MapDataIndex.BitmapNo], "0000") + ".bmp";
                    //                                            break;
                    //                                        }

                    //                                    case SRC.Map.MapImageFileType.FourFiguresMapImageFileType:
                    //                                        {
                    //                                            str_result = SRC.TDList.Bitmap(SRC.Map.MapData[mx, my, SRC.Map.MapDataIndex.TerrainType]) + SrcFormatter.Format(SRC.Map.MapData[mx, my, SRC.Map.MapDataIndex.BitmapNo], "0000") + ".bmp";
                    //                                            break;
                    //                                        }

                    //                                    case SRC.Map.MapImageFileType.OldMapImageFileType:
                    //                                        {
                    //                                            str_result = SRC.TDList.Bitmap(SRC.Map.MapData[mx, my, SRC.Map.MapDataIndex.TerrainType]) + SrcFormatter.Format(SRC.Map.MapData[mx, my, SRC.Map.MapDataIndex.BitmapNo]) + ".bmp";
                    //                                            break;
                    //                                        }
                    //                                }

                    //                                break;
                    //                            }
                    //                        // MOD  END  240a
                    //                        // ADD START 240a
                    //                        case "���C���[�r�b�g�}�b�v��":
                    //                            {
                    //                                switch (SRC.Map.MapImageFileTypeData[mx, my])
                    //                                {
                    //                                    case SRC.Map.MapImageFileType.SeparateDirMapImageFileType:
                    //                                        {
                    //                                            str_result = SRC.TDList.Bitmap(SRC.Map.MapData[mx, my, SRC.Map.MapDataIndex.LayerType]) + @"\" + SRC.TDList.Bitmap(SRC.Map.MapData[mx, my, SRC.Map.MapDataIndex.LayerType]) + SrcFormatter.Format(SRC.Map.MapData[mx, my, SRC.Map.MapDataIndex.LayerBitmapNo], "0000") + ".bmp";
                    //                                            break;
                    //                                        }

                    //                                    case SRC.Map.MapImageFileType.FourFiguresMapImageFileType:
                    //                                        {
                    //                                            str_result = SRC.TDList.Bitmap(SRC.Map.MapData[mx, my, SRC.Map.MapDataIndex.LayerType]) + SrcFormatter.Format(SRC.Map.MapData[mx, my, SRC.Map.MapDataIndex.LayerBitmapNo], "0000") + ".bmp";
                    //                                            break;
                    //                                        }

                    //                                    case SRC.Map.MapImageFileType.OldMapImageFileType:
                    //                                        {
                    //                                            str_result = SRC.TDList.Bitmap(SRC.Map.MapData[mx, my, SRC.Map.MapDataIndex.LayerType]) + SrcFormatter.Format(SRC.Map.MapData[mx, my, SRC.Map.MapDataIndex.LayerBitmapNo]) + ".bmp";
                    //                                            break;
                    //                                        }
                    //                                }

                    //                                break;
                    //                            }
                    //                        // ADD  END  240a
                    //                        case "���j�b�g�h�c":
                    //                            {
                    //                                if (SRC.Map.MapDataForUnit[mx, my] != null)
                    //                                {
                    //                                    str_result = SRC.Map.MapDataForUnit[mx, my].ID;
                    //                                }

                    //                                break;
                    //                            }
                    //                    }

                    //                    break;
                    //                }
                    //        }

                    //        break;
                    //    }

                    //case "�I�v�V����":
                    //    {
                    //        idx = (idx + 1);
                    //        switch (@params[idx] ?? "")
                    //        {
                    //            case "MessageWait":
                    //                {
                    //                    str_result = SrcFormatter.Format(SRC.GUI.MessageWait);
                    //                    break;
                    //                }

                    //            case "BattleAnimation":
                    //                {
                    //                    if (SRC.BattleAnimation)
                    //                    {
                    //                        str_result = "On";
                    //                    }
                    //                    else
                    //                    {
                    //                        str_result = "Off";
                    //                    }

                    //                    break;
                    //                }
                    //            // ADD START MARGE
                    //            case "ExtendedAnimation":
                    //                {
                    //                    if (SRC.ExtendedAnimation)
                    //                    {
                    //                        str_result = "On";
                    //                    }
                    //                    else
                    //                    {
                    //                        str_result = "Off";
                    //                    }

                    //                    break;
                    //                }
                    //            // ADD END MARGE
                    //            case "SpecialPowerAnimation":
                    //                {
                    //                    if (SRC.SpecialPowerAnimation)
                    //                    {
                    //                        str_result = "On";
                    //                    }
                    //                    else
                    //                    {
                    //                        str_result = "Off";
                    //                    }

                    //                    break;
                    //                }

                    //            case "AutoDeffence":
                    //                {
                    //                    if (SRC.SystemConfig.AutoDefense)
                    //                    {
                    //                        str_result = "On";
                    //                    }
                    //                    else
                    //                    {
                    //                        str_result = "Off";
                    //                    }

                    //                    break;
                    //                }

                    //            case "UseDirectMusic":
                    //                {
                    //                    if (Sound.UseDirectMusic)
                    //                    {
                    //                        str_result = "On";
                    //                    }
                    //                    else
                    //                    {
                    //                        str_result = "Off";
                    //                    }

                    //                    break;
                    //                }
                    //            // MOD START MARGE
                    //            // Case "Turn", "Square", "KeepEnemyBGM", "MidiReset", _
                    //            // '                    "AutoMoveCursor", "DebugMode", "LastFolder", _
                    //            // '                    "MIDIPortID", "MP3Volume", _
                    //            // '                    "BattleAnimation", "WeaponAnimation", "MoveAnimation", _
                    //            // '                    "ImageBufferNum", "MaxImageBufferSize", "KeepStretchedImage", _
                    //            // '                    "UseTransparentBlt"
                    //            // �uNewGUI�v�ŒT���ɗ�����INI�̏�Ԃ�Ԃ��B�u�V�f�t�h�v�ŒT���ɗ�����Option�̏�Ԃ�Ԃ��B
                    //            case "Turn":
                    //            case "Square":
                    //            case "KeepEnemyBGM":
                    //            case "MidiReset":
                    //            case "AutoMoveCursor":
                    //            case "DebugMode":
                    //            case "LastFolder":
                    //            case "MIDIPortID":
                    //            case "MP3Volume":
                    //            case var case13 when case13 == "BattleAnimation":
                    //            case "WeaponAnimation":
                    //            case "MoveAnimation":
                    //            case "ImageBufferNum":
                    //            case "MaxImageBufferSize":
                    //            case "KeepStretchedImage":
                    //            case "UseTransparentBlt":
                    //            case "NewGUI":
                    //                {
                    //                    // MOD END MARGE
                    //                    str_result = GeneralLib.ReadIni("Option", @params[idx]);
                    //                    break;
                    //                }

                    //            default:
                    //                {
                    //                    // Option�R�}���h�̃I�v�V�������Q��
                    //                    if (IsOptionDefined(@params[idx]))
                    //                    {
                    //                        str_result = "On";
                    //                    }
                    //                    else
                    //                    {
                    //                        str_result = "Off";
                    //                    }

                    //                    break;
                    //                }
                    //        }

                    //        break;
                    //    }
            }

            str_result = str_result ?? "";
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
}