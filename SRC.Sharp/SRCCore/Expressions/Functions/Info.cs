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

            // 各オブジェクトの設定
            switch (@params[1])
            {
                case "ユニット":
                    {
                        u = SRC.UList.Item(@params[2]);
                        idx = 3;
                        break;
                    }

                case "ユニットデータ":
                    {
                        ud = SRC.UDList.Item(@params[2]);
                        idx = 3;
                        break;
                    }

                case "パイロット":
                    {
                        p = SRC.PList.Item(@params[2]);
                        idx = 3;
                        break;
                    }

                case "パイロットデータ":
                    {
                        pd = SRC.PDList.Item(@params[2]);
                        idx = 3;
                        break;
                    }

                case "非戦闘員":
                    {
                        nd = SRC.NPDList.Item(@params[2]);
                        idx = 3;
                        break;
                    }

                case "アイテム":
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

                case "アイテムデータ":
                    {
                        itd = SRC.IDList.Item(@params[2]);
                        idx = 3;
                        break;
                    }

                case "スペシャルパワー":
                    {
                        spd = SRC.SPDList.Item(@params[2]);
                        idx = 3;
                        break;
                    }

                case "マップ":
                case "オプション":
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
                case "名称":
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

                case "読み仮名":
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

                case "愛称":
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

                case "性別":
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

                case "ユニットクラス":
                case "機体クラス":
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

                case "地形適応":
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

                case "経験値":
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

                case "格闘":
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

                case "射撃":
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

                case "命中":
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

                case "回避":
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

                case "技量":
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

                case "反応":
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

                case "防御":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.Defense);
                        }

                        break;
                    }

                case "格闘基本値":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.InfightBase);
                        }

                        break;
                    }

                case "射撃基本値":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.ShootingBase);
                        }

                        break;
                    }

                case "命中基本値":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.HitBase);
                        }

                        break;
                    }

                case "回避基本値":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.DodgeBase);
                        }

                        break;
                    }

                case "技量基本値":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.TechniqueBase);
                        }

                        break;
                    }

                case "反応基本値":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.IntuitionBase);
                        }

                        break;
                    }

                case "格闘修正値":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.InfightMod);
                        }

                        break;
                    }

                case "射撃修正値":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.ShootingMod);
                        }

                        break;
                    }

                case "命中修正値":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.HitMod);
                        }

                        break;
                    }

                case "回避修正値":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.DodgeMod);
                        }

                        break;
                    }

                case "技量修正値":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.TechniqueMod);
                        }

                        break;
                    }

                case "反応修正値":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.IntuitionMod);
                        }

                        break;
                    }

                case "格闘支援修正値":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.InfightMod2);
                        }

                        break;
                    }

                case "射撃支援修正値":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.ShootingMod2);
                        }

                        break;
                    }

                case "命中支援修正値":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.HitMod2);
                        }

                        break;
                    }

                case "回避支援修正値":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.DodgeMod2);
                        }

                        break;
                    }

                case "技量支援修正値":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.TechniqueMod2);
                        }

                        break;
                    }

                case "反応支援修正値":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.IntuitionMod2);
                        }

                        break;
                    }

                case "性格":
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

                case "最大ＳＰ":
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

                case "ＳＰ":
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

                case "グラフィック":
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

                case "ＭＩＤＩ":
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

                case "レベル":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.Level);
                        }

                        break;
                    }

                case "累積経験値":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.Exp);
                        }

                        break;
                    }

                case "気力":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.Morale);
                        }

                        break;
                    }

                case "最大霊力":
                case "最大プラーナ":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.MaxPlana());
                        }
                        else if (pd != null)
                        {
                            str_result = SrcFormatter.Format(pd.SkillLevel(0, "霊力"));
                        }

                        break;
                    }

                case "霊力":
                case "プラーナ":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.Plana);
                        }
                        else if (pd != null)
                        {
                            str_result = SrcFormatter.Format(pd.SkillLevel(0, "霊力"));
                        }

                        break;
                    }

                case "同調率":
                case "シンクロ率":
                    {
                        if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.SynchroRate());
                        }
                        else if (pd != null)
                        {
                            str_result = SrcFormatter.Format(pd.SkillLevel(0, "同調率"));
                        }

                        break;
                    }

                case "スペシャルパワー":
                case "精神コマンド":
                case "精神":
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

                case "スペシャルパワー所有":
                case "精神コマンド所有":
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

                case "スペシャルパワーコスト":
                case "精神コマンドコスト":
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

                case "特殊能力数":
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

                case "特殊能力":
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

                case "特殊能力名称":
                    {
                        var aname = @params[idx + 1];

                        // エリアスが定義されている？
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

                case "特殊能力所有":
                    {
                        var aname = @params[idx + 1];

                        // エリアスが定義されている？
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

                    //case "特殊能力レベル":
                    //    {
                    //        var aname = @params[idx + 1];

                    //        // エリアスが定義されている？
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

                    //case "特殊能力データ":
                    //    {
                    //        aname = @params[idx + 1];

                    //        // エリアスが定義されている？
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

                    //case "特殊能力必要技能":
                    //    {
                    //        aname = @params[idx + 1];

                    //        // エリアスが定義されている？
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

                    //case "特殊能力解説":
                    //    {
                    //        aname = @params[idx + 1];

                    //        // エリアスが定義されている？
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

                    //case "規定パイロット数":
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

                    //case "パイロット数":
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

                    //case "サポート数":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.CountSupport());
                    //        }

                    //        break;
                    //    }

                    //case "最大アイテム数":
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

                    //case "アイテム数":
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

                    //case "アイテム":
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

                    //case "アイテムＩＤ":
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

                    //case "移動可能地形":
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

                    //case "移動力":
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

                    //case "サイズ":
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

                    //case "修理費":
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

                    //case "最大ＨＰ":
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

                    //case "ＨＰ":
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

                    //case "最大ＥＮ":
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

                    //case "ＥＮ":
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

                    //case "装甲":
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

                    //case "運動性":
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

                    //case "武器数":
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

                    //case "武器":
                    //    {
                    //        idx = (idx + 1);
                    //        if (u != null)
                    //        {
                    //            {
                    //                var withBlock7 = u;
                    //                // 何番目の武器かを判定
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
                    //                // 指定した武器を持っていない
                    //                if (i <= 0 || withBlock7.CountWeapon() < i)
                    //                {
                    //                    return str_result;
                    //                }

                    //                idx = (idx + 1);
                    //                switch (@params[idx] ?? "")
                    //                {
                    //                    case var case1 when case1 == "":
                    //                    case "名称":
                    //                        {
                    //                            str_result = withBlock7.Weapon(i).Name;
                    //                            break;
                    //                        }

                    //                    case "攻撃力":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock7.WeaponPower(i, ""));
                    //                            break;
                    //                        }

                    //                    case "射程":
                    //                    case "最大射程":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock7.WeaponMaxRange(i));
                    //                            break;
                    //                        }

                    //                    case "最小射程":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock7.Weapon(i).MinRange);
                    //                            break;
                    //                        }

                    //                    case "命中率":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock7.WeaponPrecision(i));
                    //                            break;
                    //                        }

                    //                    case "最大弾数":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock7.MaxBullet(i));
                    //                            break;
                    //                        }

                    //                    case "弾数":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock7.Bullet(i));
                    //                            break;
                    //                        }

                    //                    case "消費ＥＮ":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock7.WeaponENConsumption(i));
                    //                            break;
                    //                        }

                    //                    case "必要気力":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock7.Weapon(i).NecessaryMorale);
                    //                            break;
                    //                        }

                    //                    case "地形適応":
                    //                        {
                    //                            str_result = withBlock7.Weapon(i).Adaption;
                    //                            break;
                    //                        }

                    //                    case "クリティカル率":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock7.WeaponCritical(i));
                    //                            break;
                    //                        }

                    //                    case "属性":
                    //                        {
                    //                            str_result = withBlock7.WeaponClass(i);
                    //                            break;
                    //                        }

                    //                    case "属性所有":
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

                    //                    case "属性レベル":
                    //                        {
                    //                            str_result = withBlock7.WeaponLevel(i, @params[idx + 1]).ToString();
                    //                            break;
                    //                        }

                    //                    case "属性名称":
                    //                        {
                    //                            str_result = Help.AttributeName(u, @params[idx + 1], false);
                    //                            break;
                    //                        }

                    //                    case "属性解説":
                    //                        {
                    //                            str_result = Help.AttributeHelpMessage(u, @params[idx + 1], i, false);
                    //                            break;
                    //                        }

                    //                    case "必要技能":
                    //                        {
                    //                            str_result = withBlock7.Weapon(i).NecessarySkill;
                    //                            break;
                    //                        }

                    //                    case "使用可":
                    //                        {
                    //                            if (withBlock7.IsWeaponAvailable(i, "ステータス"))
                    //                            {
                    //                                str_result = "1";
                    //                            }
                    //                            else
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "修得":
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
                    //            // 何番目の武器かを判定
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
                    //            // 指定した武器を持っていない
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
                    //                    case "名称":
                    //                        {
                    //                            str_result = withBlock8.Name;
                    //                            break;
                    //                        }

                    //                    case "攻撃力":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock8.Power);
                    //                            break;
                    //                        }

                    //                    case "射程":
                    //                    case "最大射程":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock8.MaxRange);
                    //                            break;
                    //                        }

                    //                    case "最小射程":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock8.MinRange);
                    //                            break;
                    //                        }

                    //                    case "命中率":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock8.Precision);
                    //                            break;
                    //                        }

                    //                    case "最大弾数":
                    //                    case "弾数":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock8.Bullet);
                    //                            break;
                    //                        }

                    //                    case "消費ＥＮ":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock8.ENConsumption);
                    //                            break;
                    //                        }

                    //                    case "必要気力":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock8.NecessaryMorale);
                    //                            break;
                    //                        }

                    //                    case "地形適応":
                    //                        {
                    //                            str_result = withBlock8.Adaption;
                    //                            break;
                    //                        }

                    //                    case "クリティカル率":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock8.Critical);
                    //                            break;
                    //                        }

                    //                    case "属性":
                    //                        {
                    //                            str_result = withBlock8.Class;
                    //                            break;
                    //                        }

                    //                    case "属性所有":
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

                    //                    case "属性レベル":
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

                    //                    case "必要技能":
                    //                        {
                    //                            str_result = withBlock8.NecessarySkill;
                    //                            break;
                    //                        }

                    //                    case "使用可":
                    //                    case "修得":
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
                    //                // 何番目の武器かを判定
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
                    //                // 指定した武器を持っていない
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
                    //                        case "名称":
                    //                            {
                    //                                str_result = withBlock10.Name;
                    //                                break;
                    //                            }

                    //                        case "攻撃力":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock10.Power);
                    //                                break;
                    //                            }

                    //                        case "射程":
                    //                        case "最大射程":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock10.MaxRange);
                    //                                break;
                    //                            }

                    //                        case "最小射程":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock10.MinRange);
                    //                                break;
                    //                            }

                    //                        case "命中率":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock10.Precision);
                    //                                break;
                    //                            }

                    //                        case "最大弾数":
                    //                        case "弾数":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock10.Bullet);
                    //                                break;
                    //                            }

                    //                        case "消費ＥＮ":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock10.ENConsumption);
                    //                                break;
                    //                            }

                    //                        case "必要気力":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock10.NecessaryMorale);
                    //                                break;
                    //                            }

                    //                        case "地形適応":
                    //                            {
                    //                                str_result = withBlock10.Adaption;
                    //                                break;
                    //                            }

                    //                        case "クリティカル率":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock10.Critical);
                    //                                break;
                    //                            }

                    //                        case "属性":
                    //                            {
                    //                                str_result = withBlock10.Class;
                    //                                break;
                    //                            }

                    //                        case "属性所有":
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

                    //                        case "属性レベル":
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

                    //                        case "必要技能":
                    //                            {
                    //                                str_result = withBlock10.NecessarySkill;
                    //                                break;
                    //                            }

                    //                        case "使用可":
                    //                        case "修得":
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
                    //            // 何番目の武器かを判定
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
                    //            // 指定した武器を持っていない
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
                    //                    case "名称":
                    //                        {
                    //                            str_result = withBlock11.Name;
                    //                            break;
                    //                        }

                    //                    case "攻撃力":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock11.Power);
                    //                            break;
                    //                        }

                    //                    case "射程":
                    //                    case "最大射程":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock11.MaxRange);
                    //                            break;
                    //                        }

                    //                    case "最小射程":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock11.MinRange);
                    //                            break;
                    //                        }

                    //                    case "命中率":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock11.Precision);
                    //                            break;
                    //                        }

                    //                    case "最大弾数":
                    //                    case "弾数":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock11.Bullet);
                    //                            break;
                    //                        }

                    //                    case "消費ＥＮ":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock11.ENConsumption);
                    //                            break;
                    //                        }

                    //                    case "必要気力":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock11.NecessaryMorale);
                    //                            break;
                    //                        }

                    //                    case "地形適応":
                    //                        {
                    //                            str_result = withBlock11.Adaption;
                    //                            break;
                    //                        }

                    //                    case "クリティカル率":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock11.Critical);
                    //                            break;
                    //                        }

                    //                    case "属性":
                    //                        {
                    //                            str_result = withBlock11.Class;
                    //                            break;
                    //                        }

                    //                    case "属性所有":
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

                    //                    case "属性レベル":
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

                    //                    case "必要技能":
                    //                        {
                    //                            str_result = withBlock11.NecessarySkill;
                    //                            break;
                    //                        }

                    //                    case "使用可":
                    //                    case "修得":
                    //                        {
                    //                            str_result = "1";
                    //                            break;
                    //                        }
                    //                }
                    //            }
                    //        }
                    //        else if (it != null)
                    //        {
                    //            // 何番目の武器かを判定
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
                    //            // 指定した武器を持っていない
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
                    //                    case "名称":
                    //                        {
                    //                            str_result = withBlock12.Name;
                    //                            break;
                    //                        }

                    //                    case "攻撃力":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock12.Power);
                    //                            break;
                    //                        }

                    //                    case "射程":
                    //                    case "最大射程":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock12.MaxRange);
                    //                            break;
                    //                        }

                    //                    case "最小射程":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock12.MinRange);
                    //                            break;
                    //                        }

                    //                    case "命中率":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock12.Precision);
                    //                            break;
                    //                        }

                    //                    case "最大弾数":
                    //                    case "弾数":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock12.Bullet);
                    //                            break;
                    //                        }

                    //                    case "消費ＥＮ":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock12.ENConsumption);
                    //                            break;
                    //                        }

                    //                    case "必要気力":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock12.NecessaryMorale);
                    //                            break;
                    //                        }

                    //                    case "地形適応":
                    //                        {
                    //                            str_result = withBlock12.Adaption;
                    //                            break;
                    //                        }

                    //                    case "クリティカル率":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock12.Critical);
                    //                            break;
                    //                        }

                    //                    case "属性":
                    //                        {
                    //                            str_result = withBlock12.Class;
                    //                            break;
                    //                        }

                    //                    case "属性所有":
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

                    //                    case "属性レベル":
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

                    //                    case "必要技能":
                    //                        {
                    //                            str_result = withBlock12.NecessarySkill;
                    //                            break;
                    //                        }

                    //                    case "使用可":
                    //                    case "修得":
                    //                        {
                    //                            str_result = "1";
                    //                            break;
                    //                        }
                    //                }
                    //            }
                    //        }
                    //        else if (itd != null)
                    //        {
                    //            // 何番目の武器かを判定
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
                    //            // 指定した武器を持っていない
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
                    //                    case "名称":
                    //                        {
                    //                            str_result = withBlock13.Name;
                    //                            break;
                    //                        }

                    //                    case "攻撃力":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock13.Power);
                    //                            break;
                    //                        }

                    //                    case "射程":
                    //                    case "最大射程":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock13.MaxRange);
                    //                            break;
                    //                        }

                    //                    case "最小射程":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock13.MinRange);
                    //                            break;
                    //                        }

                    //                    case "命中率":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock13.Precision);
                    //                            break;
                    //                        }

                    //                    case "最大弾数":
                    //                    case "弾数":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock13.Bullet);
                    //                            break;
                    //                        }

                    //                    case "消費ＥＮ":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock13.ENConsumption);
                    //                            break;
                    //                        }

                    //                    case "必要気力":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock13.NecessaryMorale);
                    //                            break;
                    //                        }

                    //                    case "地形適応":
                    //                        {
                    //                            str_result = withBlock13.Adaption;
                    //                            break;
                    //                        }

                    //                    case "クリティカル率":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock13.Critical);
                    //                            break;
                    //                        }

                    //                    case "属性":
                    //                        {
                    //                            str_result = withBlock13.Class;
                    //                            break;
                    //                        }

                    //                    case "属性所有":
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

                    //                    case "属性レベル":
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

                    //                    case "必要技能":
                    //                        {
                    //                            str_result = withBlock13.NecessarySkill;
                    //                            break;
                    //                        }

                    //                    case "使用可":
                    //                    case "修得":
                    //                        {
                    //                            str_result = "1";
                    //                            break;
                    //                        }
                    //                }
                    //            }
                    //        }

                    //        break;
                    //    }

                    //case "アビリティ数":
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

                    //case "アビリティ":
                    //    {
                    //        idx = (idx + 1);
                    //        if (u != null)
                    //        {
                    //            {
                    //                var withBlock14 = u;
                    //                // 何番目のアビリティかを判定
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
                    //                // 指定したアビリティを持っていない
                    //                if (i <= 0 || withBlock14.CountAbility() < i)
                    //                {
                    //                    return str_result;
                    //                }

                    //                idx = (idx + 1);
                    //                switch (@params[idx] ?? "")
                    //                {
                    //                    case var case7 when case7 == "":
                    //                    case "名称":
                    //                        {
                    //                            str_result = withBlock14.Ability(i).Name;
                    //                            break;
                    //                        }

                    //                    case "効果数":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock14.Ability(i).CountEffect());
                    //                            break;
                    //                        }

                    //                    case "効果タイプ":
                    //                        {
                    //                            // 何番目の効果かを判定
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

                    //                    case "効果レベル":
                    //                        {
                    //                            // 何番目の効果かを判定
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

                    //                    case "効果データ":
                    //                        {
                    //                            // 何番目の効果かを判定
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

                    //                    case "射程":
                    //                    case "最大射程":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock14.AbilityMaxRange(i));
                    //                            break;
                    //                        }

                    //                    case "最小射程":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock14.AbilityMinRange(i));
                    //                            break;
                    //                        }

                    //                    case "最大使用回数":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock14.MaxStock(i));
                    //                            break;
                    //                        }

                    //                    case "使用回数":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock14.Stock(i));
                    //                            break;
                    //                        }

                    //                    case "消費ＥＮ":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock14.AbilityENConsumption(i));
                    //                            break;
                    //                        }

                    //                    case "必要気力":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock14.Ability(i).NecessaryMorale);
                    //                            break;
                    //                        }

                    //                    case "属性":
                    //                        {
                    //                            str_result = withBlock14.Ability(i).Class;
                    //                            break;
                    //                        }

                    //                    case "属性所有":
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

                    //                    case "属性レベル":
                    //                        {
                    //                            str_result = withBlock14.AbilityLevel(i, @params[idx + 1]).ToString();
                    //                            break;
                    //                        }

                    //                    case "属性名称":
                    //                        {
                    //                            str_result = Help.AttributeName(u, @params[idx + 1], true);
                    //                            break;
                    //                        }

                    //                    case "属性解説":
                    //                        {
                    //                            str_result = Help.AttributeHelpMessage(u, @params[idx + 1], i, true);
                    //                            break;
                    //                        }

                    //                    case "必要技能":
                    //                        {
                    //                            str_result = withBlock14.Ability(i).NecessarySkill;
                    //                            break;
                    //                        }

                    //                    case "使用可":
                    //                        {
                    //                            if (withBlock14.IsAbilityAvailable(i, "移動前"))
                    //                            {
                    //                                str_result = "1";
                    //                            }
                    //                            else
                    //                            {
                    //                                str_result = "0";
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "修得":
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
                    //            // 何番目のアビリティかを判定
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
                    //            // 指定したアビリティを持っていない
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
                    //                    case "名称":
                    //                        {
                    //                            str_result = withBlock15.Name;
                    //                            break;
                    //                        }

                    //                    case "効果数":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock15.CountEffect());
                    //                            break;
                    //                        }

                    //                    case "効果タイプ":
                    //                        {
                    //                            // 何番目の効果かを判定
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

                    //                    case "効果レベル":
                    //                        {
                    //                            // 何番目の効果かを判定
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

                    //                    case "効果データ":
                    //                        {
                    //                            // 何番目の効果かを判定
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

                    //                    case "射程":
                    //                    case "最大射程":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock15.MaxRange);
                    //                            break;
                    //                        }

                    //                    case "最小射程":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock15.MinRange);
                    //                            break;
                    //                        }

                    //                    case "最大使用回数":
                    //                    case "使用回数":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock15.Stock);
                    //                            break;
                    //                        }

                    //                    case "消費ＥＮ":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock15.ENConsumption);
                    //                            break;
                    //                        }

                    //                    case "必要気力":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock15.NecessaryMorale);
                    //                            break;
                    //                        }

                    //                    case "属性":
                    //                        {
                    //                            str_result = withBlock15.Class;
                    //                            break;
                    //                        }

                    //                    case "属性所有":
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

                    //                    case "属性レベル":
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

                    //                    case "必要技能":
                    //                        {
                    //                            str_result = withBlock15.NecessarySkill;
                    //                            break;
                    //                        }

                    //                    case "使用可":
                    //                    case "修得":
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
                    //                // 何番目のアビリティかを判定
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
                    //                // 指定したアビリティを持っていない
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
                    //                        case "名称":
                    //                            {
                    //                                str_result = withBlock17.Name;
                    //                                break;
                    //                            }

                    //                        case "効果数":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock17.CountEffect());
                    //                                break;
                    //                            }

                    //                        case "効果タイプ":
                    //                            {
                    //                                // 何番目の効果かを判定
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

                    //                        case "効果レベル":
                    //                            {
                    //                                // 何番目の効果かを判定
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

                    //                        case "効果データ":
                    //                            {
                    //                                // 何番目の効果かを判定
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

                    //                        case "射程":
                    //                        case "最大射程":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock17.MaxRange);
                    //                                break;
                    //                            }

                    //                        case "最小射程":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock17.MinRange);
                    //                                break;
                    //                            }

                    //                        case "最大使用回数":
                    //                        case "使用回数":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock17.Stock);
                    //                                break;
                    //                            }

                    //                        case "消費ＥＮ":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock17.ENConsumption);
                    //                                break;
                    //                            }

                    //                        case "必要気力":
                    //                            {
                    //                                str_result = SrcFormatter.Format(withBlock17.NecessaryMorale);
                    //                                break;
                    //                            }

                    //                        case "属性":
                    //                            {
                    //                                str_result = withBlock17.Class;
                    //                                break;
                    //                            }

                    //                        case "属性所有":
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

                    //                        case "属性レベル":
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

                    //                        case "必要技能":
                    //                            {
                    //                                str_result = withBlock17.NecessarySkill;
                    //                                break;
                    //                            }

                    //                        case "使用可":
                    //                        case "修得":
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
                    //            // 何番目のアビリティかを判定
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
                    //            // 指定したアビリティを持っていない
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
                    //                    case "名称":
                    //                        {
                    //                            str_result = withBlock18.Name;
                    //                            break;
                    //                        }

                    //                    case "効果数":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock18.CountEffect());
                    //                            break;
                    //                        }

                    //                    case "効果タイプ":
                    //                        {
                    //                            // 何番目の効果かを判定
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

                    //                    case "効果レベル":
                    //                        {
                    //                            // 何番目の効果かを判定
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

                    //                    case "効果データ":
                    //                        {
                    //                            // 何番目の効果かを判定
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

                    //                    case "射程":
                    //                    case "最大射程":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock18.MaxRange);
                    //                            break;
                    //                        }

                    //                    case "最小射程":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock18.MinRange);
                    //                            break;
                    //                        }

                    //                    case "最大使用回数":
                    //                    case "使用回数":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock18.Stock);
                    //                            break;
                    //                        }

                    //                    case "消費ＥＮ":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock18.ENConsumption);
                    //                            break;
                    //                        }

                    //                    case "必要気力":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock18.NecessaryMorale);
                    //                            break;
                    //                        }

                    //                    case "属性":
                    //                        {
                    //                            str_result = withBlock18.Class;
                    //                            break;
                    //                        }

                    //                    case "属性所有":
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

                    //                    case "属性レベル":
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

                    //                    case "必要技能":
                    //                        {
                    //                            str_result = withBlock18.NecessarySkill;
                    //                            break;
                    //                        }

                    //                    case "使用可":
                    //                    case "修得":
                    //                        {
                    //                            str_result = "1";
                    //                            break;
                    //                        }
                    //                }
                    //            }
                    //        }
                    //        else if (it != null)
                    //        {
                    //            // 何番目のアビリティかを判定
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
                    //            // 指定したアビリティを持っていない
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
                    //                    case "名称":
                    //                        {
                    //                            str_result = withBlock19.Name;
                    //                            break;
                    //                        }

                    //                    case "効果数":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock19.CountEffect());
                    //                            break;
                    //                        }

                    //                    case "効果タイプ":
                    //                        {
                    //                            // 何番目の効果かを判定
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

                    //                    case "効果レベル":
                    //                        {
                    //                            // 何番目の効果かを判定
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

                    //                    case "効果データ":
                    //                        {
                    //                            // 何番目の効果かを判定
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

                    //                    case "射程":
                    //                    case "最大射程":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock19.MaxRange);
                    //                            break;
                    //                        }

                    //                    case "最小射程":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock19.MinRange);
                    //                            break;
                    //                        }

                    //                    case "最大使用回数":
                    //                    case "使用回数":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock19.Stock);
                    //                            break;
                    //                        }

                    //                    case "消費ＥＮ":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock19.ENConsumption);
                    //                            break;
                    //                        }

                    //                    case "必要気力":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock19.NecessaryMorale);
                    //                            break;
                    //                        }

                    //                    case "属性":
                    //                        {
                    //                            str_result = withBlock19.Class;
                    //                            break;
                    //                        }

                    //                    case "属性所有":
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

                    //                    case "属性レベル":
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

                    //                    case "必要技能":
                    //                        {
                    //                            str_result = withBlock19.NecessarySkill;
                    //                            break;
                    //                        }

                    //                    case "使用可":
                    //                    case "修得":
                    //                        {
                    //                            str_result = "1";
                    //                            break;
                    //                        }
                    //                }
                    //            }
                    //        }
                    //        else if (itd != null)
                    //        {
                    //            // 何番目のアビリティかを判定
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
                    //            // 指定したアビリティを持っていない
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
                    //                    case "名称":
                    //                        {
                    //                            str_result = withBlock20.Name;
                    //                            break;
                    //                        }

                    //                    case "効果数":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock20.CountEffect());
                    //                            break;
                    //                        }

                    //                    case "効果タイプ":
                    //                        {
                    //                            // 何番目の効果かを判定
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

                    //                    case "効果レベル":
                    //                        {
                    //                            // 何番目の効果かを判定
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

                    //                    case "効果データ":
                    //                        {
                    //                            // 何番目の効果かを判定
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

                    //                    case "射程":
                    //                    case "最大射程":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock20.MaxRange);
                    //                            break;
                    //                        }

                    //                    case "最小射程":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock20.MinRange);
                    //                            break;
                    //                        }

                    //                    case "最大使用回数":
                    //                    case "使用回数":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock20.Stock);
                    //                            break;
                    //                        }

                    //                    case "消費ＥＮ":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock20.ENConsumption);
                    //                            break;
                    //                        }

                    //                    case "必要気力":
                    //                        {
                    //                            str_result = SrcFormatter.Format(withBlock20.NecessaryMorale);
                    //                            break;
                    //                        }

                    //                    case "属性":
                    //                        {
                    //                            str_result = withBlock20.Class;
                    //                            break;
                    //                        }

                    //                    case "属性所有":
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

                    //                    case "属性レベル":
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

                    //                    case "必要技能":
                    //                        {
                    //                            str_result = withBlock20.NecessarySkill;
                    //                            break;
                    //                        }

                    //                    case "使用可":
                    //                    case "修得":
                    //                        {
                    //                            str_result = "1";
                    //                            break;
                    //                        }
                    //                }
                    //            }
                    //        }

                    //        break;
                    //    }

                    //case "ランク":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.Rank);
                    //        }

                    //        break;
                    //    }

                    //case "ボスランク":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.BossRank);
                    //        }

                    //        break;
                    //    }

                    //case "エリア":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = u.Area;
                    //        }

                    //        break;
                    //    }

                    //case "思考モード":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = u.Mode;
                    //        }

                    //        break;
                    //    }

                    //case "最大攻撃力":
                    //    {
                    //        if (u != null)
                    //        {
                    //            {
                    //                var withBlock21 = u;
                    //                max_value = 0;
                    //                var loopTo20 = withBlock21.CountWeapon();
                    //                for (i = 1; i <= loopTo20; i++)
                    //                {
                    //                    if (withBlock21.IsWeaponMastered(i) && !withBlock21.IsDisabled(withBlock21.Weapon(i).Name) && !withBlock21.IsWeaponClassifiedAs(i, "合"))
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

                    //                if (Strings.InStr(localWeapon7().Class, "合") == 0)
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

                    //case "最長射程":
                    //    {
                    //        if (u != null)
                    //        {
                    //            {
                    //                var withBlock22 = u;
                    //                max_value = 0;
                    //                var loopTo22 = withBlock22.CountWeapon();
                    //                for (i = 1; i <= loopTo22; i++)
                    //                {
                    //                    if (withBlock22.IsWeaponMastered(i) && !withBlock22.IsDisabled(withBlock22.Weapon(i).Name) && !withBlock22.IsWeaponClassifiedAs(i, "合"))
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

                    //                if (Strings.InStr(localWeapon10().Class, "合") == 0)
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

                    //case "残りサポートアタック数":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.MaxSupportAttack() - u.UsedSupportAttack);
                    //        }

                    //        break;
                    //    }

                    //case "残りサポートガード数":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.MaxSupportGuard() - u.UsedSupportGuard);
                    //        }

                    //        break;
                    //    }

                    //case "残り同時援護攻撃数":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.MaxSyncAttack() - u.UsedSyncAttack);
                    //        }

                    //        break;
                    //    }

                    //case "残りカウンター攻撃数":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(u.MaxCounterAttack() - u.UsedCounterAttack);
                    //        }

                    //        break;
                    //    }

                    //case "改造費":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(InterMission.RankUpCost(u));
                    //        }

                    //        break;
                    //    }

                    //case "最大改造数":
                    //    {
                    //        if (u != null)
                    //        {
                    //            str_result = SrcFormatter.Format(InterMission.MaxRank(u));
                    //        }

                    //        break;
                    //    }

                    //case "アイテムクラス":
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

                    //case "装備個所":
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

                    //case "最大ＨＰ修正値":
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

                    //case "最大ＥＮ修正値":
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

                    //case "装甲修正値":
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

                    //case "運動性修正値":
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

                    //case "移動力修正値":
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

                    //case "解説文":
                    //case "コメント":
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

                    //case "短縮名":
                    //    {
                    //        if (spd != null)
                    //        {
                    //            str_result = spd.ShortName;
                    //        }

                    //        break;
                    //    }

                    //case "消費ＳＰ":
                    //    {
                    //        if (spd != null)
                    //        {
                    //            str_result = SrcFormatter.Format(spd.SPConsumption);
                    //        }

                    //        break;
                    //    }

                    //case "対象":
                    //    {
                    //        if (spd != null)
                    //        {
                    //            str_result = spd.TargetType;
                    //        }

                    //        break;
                    //    }

                    //case "持続期間":
                    //    {
                    //        if (spd != null)
                    //        {
                    //            str_result = spd.Duration;
                    //        }

                    //        break;
                    //    }

                    //case "適用条件":
                    //    {
                    //        if (spd != null)
                    //        {
                    //            str_result = spd.NecessaryCondition;
                    //        }

                    //        break;
                    //    }

                    //case "アニメ":
                    //    {
                    //        if (spd != null)
                    //        {
                    //            str_result = spd.Animation;
                    //        }

                    //        break;
                    //    }

                    //case "効果数":
                    //    {
                    //        if (spd != null)
                    //        {
                    //            str_result = SrcFormatter.Format(spd.CountEffect());
                    //        }

                    //        break;
                    //    }

                    //case "効果タイプ":
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

                    //case "効果レベル":
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

                    //case "効果データ":
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

                    //case "マップ":
                    //    {
                    //        idx = (idx + 1);
                    //        switch (@params[idx] ?? "")
                    //        {
                    //            case "ファイル名":
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

                    //            case "幅":
                    //                {
                    //                    str_result = SrcFormatter.Format(SRC.Map.MapWidth);
                    //                    break;
                    //                }

                    //            case "時間帯":
                    //                {
                    //                    if (!string.IsNullOrEmpty(SRC.Map.MapDrawMode))
                    //                    {
                    //                        if (SRC.Map.MapDrawMode == "フィルタ")
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
                    //                            buf = buf + " マップ限定";
                    //                        }

                    //                        str_result = buf;
                    //                    }
                    //                    else
                    //                    {
                    //                        str_result = "昼";
                    //                    }

                    //                    break;
                    //                }

                    //            case "高さ":
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
                    //                        case "地形名":
                    //                            {
                    //                                str_result = SRC.Map.TerrainName(mx, my);
                    //                                break;
                    //                            }

                    //                        case "地形タイプ":
                    //                        case "地形クラス":
                    //                            {
                    //                                str_result = SRC.Map.TerrainClass(mx, my);
                    //                                break;
                    //                            }

                    //                        case "移動コスト":
                    //                            {
                    //                                // 0.5刻みの移動コストを使えるようにするため、移動コストは
                    //                                // 実際の２倍の値で記録されている
                    //                                str_result = SrcFormatter.Format(SRC.Map.TerrainMoveCost(mx, my) / 2d);
                    //                                break;
                    //                            }

                    //                        case "回避修正":
                    //                            {
                    //                                str_result = SrcFormatter.Format(SRC.Map.TerrainEffectForHit(mx, my));
                    //                                break;
                    //                            }

                    //                        case "ダメージ修正":
                    //                            {
                    //                                str_result = SrcFormatter.Format(SRC.Map.TerrainEffectForDamage(mx, my));
                    //                                break;
                    //                            }

                    //                        case "ＨＰ回復量":
                    //                            {
                    //                                str_result = SrcFormatter.Format(SRC.Map.TerrainEffectForHPRecover(mx, my));
                    //                                break;
                    //                            }

                    //                        case "ＥＮ回復量":
                    //                            {
                    //                                str_result = SrcFormatter.Format(SRC.Map.TerrainEffectForENRecover(mx, my));
                    //                                break;
                    //                            }

                    //                        case "ビットマップ名":
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
                    //                        case "レイヤービットマップ名":
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
                    //                        case "ユニットＩＤ":
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

                    //case "オプション":
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
                    //            // 「NewGUI」で探しに来たらINIの状態を返す。「新ＧＵＩ」で探しに来たらOptionの状態を返す。
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
                    //                    // Optionコマンドのオプションを参照
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