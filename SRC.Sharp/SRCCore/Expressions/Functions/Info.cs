using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.Units;
using SRCCore.VB;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Expressions.Functions
{
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

                case "特殊能力レベル":
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
                                str_result = SrcFormatter.Format(u.FeatureLevel(Conversions.ToInteger(aname)));
                            }
                            else
                            {
                                str_result = SrcFormatter.Format(u.FeatureLevel(aname));
                            }
                        }
                        else if (ud != null)
                        {
                            if (GeneralLib.IsNumber(aname))
                            {
                                str_result = SrcFormatter.Format(ud.FeatureLevel(Conversions.ToInteger(aname)));
                            }
                            else
                            {
                                str_result = SrcFormatter.Format(ud.FeatureLevel(aname));
                            }
                        }
                        else if (p != null)
                        {
                            if (GeneralLib.IsNumber(aname))
                            {
                                str_result = SrcFormatter.Format(p.SkillLevel(Conversions.ToInteger(aname)));
                            }
                            else
                            {
                                str_result = SrcFormatter.Format(p.SkillLevel(aname));
                            }
                        }
                        else if (pd != null)
                        {
                            if (GeneralLib.IsNumber(aname))
                            {
                                str_result = SrcFormatter.Format(pd.SkillLevel(100, GeneralLib.LIndex(pd.Skill(100), Conversions.ToInteger(aname))));
                            }
                            else
                            {
                                str_result = SrcFormatter.Format(pd.SkillLevel(100, aname));
                            }
                        }
                        else if (it != null)
                        {
                            if (GeneralLib.IsNumber(aname))
                            {
                                str_result = SrcFormatter.Format(it.FeatureLevel(Conversions.ToInteger(aname)));
                            }
                            else
                            {
                                str_result = SrcFormatter.Format(it.FeatureLevel(aname));
                            }
                        }
                        else if (itd != null)
                        {
                            if (GeneralLib.IsNumber(aname))
                            {
                                str_result = SrcFormatter.Format(itd.FeatureLevel(Conversions.ToInteger(aname)));
                            }
                            else
                            {
                                str_result = SrcFormatter.Format(itd.FeatureLevel(aname));
                            }
                        }

                        break;
                    }

                case "特殊能力データ":
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
                                str_result = u.FeatureData(Conversions.ToInteger(aname));
                            }
                            else
                            {
                                str_result = u.FeatureData(aname);
                            }
                        }
                        else if (ud != null)
                        {
                            if (GeneralLib.IsNumber(aname))
                            {
                                str_result = ud.FeatureData(Conversions.ToInteger(aname));
                            }
                            else
                            {
                                str_result = ud.FeatureData(aname);
                            }
                        }
                        else if (p != null)
                        {
                            if (GeneralLib.IsNumber(aname))
                            {
                                str_result = p.SkillData(Conversions.ToInteger(aname));
                            }
                            else
                            {
                                str_result = p.SkillData(aname);
                            }
                        }
                        else if (pd != null)
                        {
                            if (GeneralLib.IsNumber(aname))
                            {
                                string localLIndex6() { string arglist = pd.Skill(100); var ret = GeneralLib.LIndex(arglist, Conversions.ToInteger(aname)); return ret; }

                                str_result = pd.SkillData(100, localLIndex6());
                            }
                            else
                            {
                                str_result = pd.SkillData(100, aname);
                            }
                        }
                        else if (it != null)
                        {
                            if (GeneralLib.IsNumber(aname))
                            {
                                str_result = it.FeatureData(Conversions.ToInteger(aname));
                            }
                            else
                            {
                                str_result = it.FeatureData(aname);
                            }
                        }
                        else if (itd != null)
                        {
                            if (GeneralLib.IsNumber(aname))
                            {
                                str_result = itd.FeatureData(Conversions.ToInteger(aname));
                            }
                            else
                            {
                                str_result = itd.FeatureData(aname);
                            }
                        }

                        break;
                    }

                case "特殊能力必要技能":
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
                                str_result = u.Feature(Conversions.ToInteger(aname))?.NecessarySkill ?? "";
                            }
                            else
                            {
                                str_result = u.Feature(aname)?.NecessarySkill ?? "";
                            }
                        }
                        else if (ud != null)
                        {
                            if (GeneralLib.IsNumber(aname))
                            {
                                str_result = ud.Feature(Conversions.ToInteger(aname))?.NecessarySkill ?? "";
                            }
                            else
                            {
                                str_result = ud.Feature(aname)?.NecessarySkill ?? "";
                            }
                        }
                        else if (it != null)
                        {
                            if (GeneralLib.IsNumber(aname))
                            {
                                str_result = it.Feature(Conversions.ToInteger(aname))?.NecessarySkill ?? "";
                            }
                            else
                            {
                                str_result = it.Feature(aname)?.NecessarySkill ?? "";
                            }
                        }
                        else if (itd != null)
                        {
                            if (GeneralLib.IsNumber(aname))
                            {
                                str_result = itd.Feature(Conversions.ToInteger(aname))?.NecessarySkill ?? "";
                            }
                            else
                            {
                                str_result = itd.Feature(aname)?.NecessarySkill ?? "";
                            }
                        }

                        break;
                    }

                case "特殊能力解説":
                    {
                        // TODO Impl Help
                        var aname = @params[idx + 1];

                        // エリアスが定義されている？
                        if (SRC.ALDList.IsDefined(aname))
                        {
                            aname = SRC.ALDList.Item(aname).ReplaceTypeName(aname);
                        }

                        //if (u != null)
                        //{
                        //    if (GeneralLib.IsNumber(aname))
                        //    {
                        //        str_result = Help.FeatureHelpMessage(u, Conversions.ToInteger(aname), false);
                        //    }
                        //    else
                        //    {
                        //        str_result = Help.FeatureHelpMessage(u, aname, false);
                        //    }

                        //    if (string.IsNullOrEmpty(str_result) && p != null)
                        //    {
                        //        str_result = Help.SkillHelpMessage(p, aname);
                        //    }
                        //}
                        //else if (p != null)
                        //{
                        //    str_result = Help.SkillHelpMessage(p, aname);
                        //    if (string.IsNullOrEmpty(str_result) && u != null)
                        //    {
                        //        if (GeneralLib.IsNumber(aname))
                        //        {
                        //            str_result = Help.FeatureHelpMessage(u, Conversions.ToInteger(aname), false);
                        //        }
                        //        else
                        //        {
                        //            str_result = Help.FeatureHelpMessage(u, aname, false);
                        //        }
                        //    }
                        //}

                        break;
                    }

                case "規定パイロット数":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(u.Data.PilotNum);
                        }
                        else if (ud != null)
                        {
                            str_result = SrcFormatter.Format(ud.PilotNum);
                        }

                        break;
                    }

                case "パイロット数":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(u.CountPilot());
                        }
                        else if (ud != null)
                        {
                            str_result = SrcFormatter.Format(ud.PilotNum);
                        }

                        break;
                    }

                case "サポート数":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(u.CountSupport());
                        }

                        break;
                    }

                case "最大アイテム数":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(u.Data.ItemNum);
                        }
                        else if (ud != null)
                        {
                            str_result = SrcFormatter.Format(ud.ItemNum);
                        }

                        break;
                    }

                case "アイテム数":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(u.CountItem());
                        }
                        else if (ud != null)
                        {
                            str_result = SrcFormatter.Format(ud.ItemNum);
                        }

                        break;
                    }

                case "アイテム":
                    {
                        if (u != null)
                        {
                            if (GeneralLib.IsNumber(@params[idx + 1]))
                            {
                                var i = Conversions.ToInteger(@params[idx + 1]);
                                if (0 < i && i <= u.CountItem())
                                {
                                    str_result = SrcFormatter.Format(u.Item(i).Name);
                                }
                            }
                        }

                        break;
                    }

                case "アイテムＩＤ":
                    {
                        if (u != null)
                        {
                            if (GeneralLib.IsNumber(@params[idx + 1]))
                            {
                                var i = Conversions.ToInteger(@params[idx + 1]);
                                if (0 < i && i <= u.CountItem())
                                {
                                    str_result = SrcFormatter.Format(u.Item(i).ID);
                                }
                            }
                        }

                        break;
                    }

                case "移動可能地形":
                    {
                        if (u != null)
                        {
                            str_result = u.Transportation;
                        }
                        else if (ud != null)
                        {
                            str_result = ud.Transportation;
                        }

                        break;
                    }

                case "移動力":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(u.Speed);
                        }
                        else if (ud != null)
                        {
                            str_result = SrcFormatter.Format(ud.Speed);
                        }

                        break;
                    }

                case "サイズ":
                    {
                        if (u != null)
                        {
                            str_result = u.Size;
                        }
                        else if (ud != null)
                        {
                            str_result = ud.Size;
                        }

                        break;
                    }

                case "修理費":
                    {
                        if (u != null)
                        {
                            str_result = u.Value.ToString();
                        }
                        else if (ud != null)
                        {
                            str_result = ud.Value.ToString();
                        }

                        break;
                    }

                case "最大ＨＰ":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(u.MaxHP);
                        }
                        else if (ud != null)
                        {
                            str_result = SrcFormatter.Format(ud.HP);
                        }

                        break;
                    }

                case "ＨＰ":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(u.HP);
                        }
                        else if (ud != null)
                        {
                            str_result = SrcFormatter.Format(ud.HP);
                        }

                        break;
                    }

                case "最大ＥＮ":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(u.MaxEN);
                        }
                        else if (ud != null)
                        {
                            str_result = SrcFormatter.Format(ud.EN);
                        }

                        break;
                    }

                case "ＥＮ":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(u.EN);
                        }
                        else if (ud != null)
                        {
                            str_result = SrcFormatter.Format(ud.EN);
                        }

                        break;
                    }

                case "装甲":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(u.get_Armor(""));
                        }
                        else if (ud != null)
                        {
                            str_result = SrcFormatter.Format(ud.Armor);
                        }

                        break;
                    }

                case "運動性":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(u.get_Mobility(""));
                        }
                        else if (ud != null)
                        {
                            str_result = SrcFormatter.Format(ud.Mobility);
                        }

                        break;
                    }

                case "武器数":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(u.CountWeapon());
                        }
                        else if (ud != null)
                        {
                            str_result = SrcFormatter.Format(ud.CountWeapon());
                        }
                        else if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.Data.CountWeapon());
                        }
                        else if (pd != null)
                        {
                            str_result = SrcFormatter.Format(pd.CountWeapon());
                        }
                        else if (it != null)
                        {
                            str_result = SrcFormatter.Format(it.CountWeapon());
                        }
                        else if (itd != null)
                        {
                            str_result = SrcFormatter.Format(itd.CountWeapon());
                        }

                        break;
                    }

                case "武器":
                    {
                        idx = (idx + 1);
                        if (u != null)
                        {
                            // 何番目の武器かを判定
                            var weapons = u.Weapons;
                            UnitWeapon uw;
                            if (GeneralLib.IsNumber(@params[idx]))
                            {
                                uw = weapons.SafeRefOneOffset(Conversions.ToInteger(@params[idx]));
                            }
                            else
                            {
                                uw = weapons.FirstOrDefault(x => x.Name == @params[idx]);
                            }
                            // 指定した武器を持っていない
                            if (uw == null)
                            {
                                return ValueType.StringType;
                            }

                            idx = (idx + 1);
                            switch (@params[idx] ?? "")
                            {
                                case var case1 when case1 == "":
                                case "名称":
                                    {
                                        str_result = uw.Name;
                                        break;
                                    }

                                case "攻撃力":
                                    {
                                        str_result = SrcFormatter.Format(uw.WeaponPower(""));
                                        break;
                                    }

                                case "射程":
                                case "最大射程":
                                    {
                                        str_result = SrcFormatter.Format(uw.WeaponMaxRange());
                                        break;
                                    }

                                case "最小射程":
                                    {
                                        str_result = SrcFormatter.Format(uw.WeaponMinRange());
                                        break;
                                    }

                                case "命中率":
                                    {
                                        str_result = SrcFormatter.Format(uw.WeaponPrecision());
                                        break;
                                    }

                                case "最大弾数":
                                    {
                                        str_result = SrcFormatter.Format(uw.MaxBullet());
                                        break;
                                    }

                                case "弾数":
                                    {
                                        str_result = SrcFormatter.Format(uw.Bullet());
                                        break;
                                    }

                                case "消費ＥＮ":
                                    {
                                        str_result = SrcFormatter.Format(uw.WeaponENConsumption());
                                        break;
                                    }

                                case "必要気力":
                                    {
                                        str_result = SrcFormatter.Format(uw.WeaponData.NecessaryMorale);
                                        break;
                                    }

                                case "地形適応":
                                    {
                                        str_result = uw.WeaponData.Adaption;
                                        break;
                                    }

                                case "クリティカル率":
                                    {
                                        str_result = SrcFormatter.Format(uw.WeaponCritical());
                                        break;
                                    }

                                case "属性":
                                    {
                                        str_result = uw.WeaponClass();
                                        break;
                                    }

                                case "属性所有":
                                    {
                                        if (uw.IsWeaponClassifiedAs(@params[idx + 1]))
                                        {
                                            str_result = "1";
                                        }
                                        else
                                        {
                                            str_result = "0";
                                        }

                                        break;
                                    }

                                case "属性レベル":
                                    {
                                        str_result = uw.WeaponLevel(@params[idx + 1]).ToString();
                                        break;
                                    }

                                // TODO Impl Help
                                //case "属性名称":
                                //    {
                                //        str_result = Help.AttributeName(u, @params[idx + 1], false);
                                //        break;
                                //    }

                                //case "属性解説":
                                //    {
                                //        str_result = Help.AttributeHelpMessage(u, @params[idx + 1], i, false);
                                //        break;
                                //    }

                                case "必要技能":
                                    {
                                        str_result = uw.WeaponData.NecessarySkill;
                                        break;
                                    }

                                case "使用可":
                                    {
                                        if (uw.IsWeaponAvailable("ステータス"))
                                        {
                                            str_result = "1";
                                        }
                                        else
                                        {
                                            str_result = "0";
                                        }

                                        break;
                                    }

                                case "修得":
                                    {
                                        if (uw.IsWeaponMastered())
                                        {
                                            str_result = "1";
                                        }
                                        else
                                        {
                                            str_result = "0";
                                        }

                                        break;
                                    }
                            }
                        }
                        else if (ud != null)
                        {
                            str_result = InfoWeaponData(@params, idx, ud.Weapons);
                        }
                        else if (p != null)
                        {
                            str_result = InfoWeaponData(@params, idx, p.Data.Weapons);
                        }
                        else if (pd != null)
                        {
                            str_result = InfoWeaponData(@params, idx, pd.Weapons);
                        }
                        else if (it != null)
                        {
                            str_result = InfoWeaponData(@params, idx, it.Data.Weapons);
                        }
                        else if (itd != null)
                        {
                            str_result = InfoWeaponData(@params, idx, itd.Weapons);
                        }
                        break;
                    }

                case "アビリティ数":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(u.CountAbility());
                        }
                        else if (ud != null)
                        {
                            str_result = SrcFormatter.Format(ud.CountAbility());
                        }
                        else if (p != null)
                        {
                            str_result = SrcFormatter.Format(p.Data.CountAbility());
                        }
                        else if (pd != null)
                        {
                            str_result = SrcFormatter.Format(pd.CountAbility());
                        }
                        else if (it != null)
                        {
                            str_result = SrcFormatter.Format(it.CountAbility());
                        }
                        else if (itd != null)
                        {
                            str_result = SrcFormatter.Format(itd.CountAbility());
                        }

                        break;
                    }

                case "アビリティ":
                    {
                        idx = (idx + 1);
                        if (u != null)
                        {
                            // 何番目のアビリティかを判定
                            UnitAbility ua;
                            if (GeneralLib.IsNumber(@params[idx]))
                            {
                                var i = Conversions.ToInteger(@params[idx]);
                                ua = u.Abilities.SafeRefOneOffset(i);
                            }
                            else
                            {
                                ua = u.Abilities.FirstOrDefault(x => x.Data.Name == @params[idx]);
                            }
                            // 指定したアビリティを持っていない
                            if (ua == null)
                            {
                                return ValueType.StringType;
                            }

                            idx = (idx + 1);
                            switch (@params[idx] ?? "")
                            {
                                case var case7 when case7 == "":
                                case "名称":
                                    {
                                        str_result = ua.Data.Name;
                                        break;
                                    }

                                case "効果数":
                                    {
                                        str_result = SrcFormatter.Format(ua.Data.Effects.Count);
                                        break;
                                    }

                                case "効果タイプ":
                                    {
                                        // 何番目の効果かを判定
                                        if (GeneralLib.IsNumber(@params[idx + 1]))
                                        {
                                            var j = Conversions.ToInteger(@params[idx + 1]);
                                            str_result = ua.Data.Effects.SafeRefOneOffset(j).EffectType;
                                        }
                                        break;
                                    }

                                case "効果レベル":
                                    {
                                        // 何番目の効果かを判定
                                        if (GeneralLib.IsNumber(@params[idx + 1]))
                                        {
                                            var j = Conversions.ToInteger(@params[idx + 1]);
                                            str_result = SrcFormatter.Format(ua.Data.Effects.SafeRefOneOffset(j).Level);
                                        }
                                        break;
                                    }

                                case "効果データ":
                                    {
                                        // 何番目の効果かを判定
                                        if (GeneralLib.IsNumber(@params[idx + 1]))
                                        {
                                            var j = Conversions.ToInteger(@params[idx + 1]);
                                            str_result = ua.Data.Effects.SafeRefOneOffset(j).Data;
                                        }
                                        break;
                                    }

                                case "射程":
                                case "最大射程":
                                    {
                                        str_result = SrcFormatter.Format(ua.AbilityMaxRange());
                                        break;
                                    }

                                case "最小射程":
                                    {
                                        str_result = SrcFormatter.Format(ua.AbilityMinRange());
                                        break;
                                    }

                                case "最大使用回数":
                                    {
                                        str_result = SrcFormatter.Format(ua.MaxStock());
                                        break;
                                    }

                                case "使用回数":
                                    {
                                        str_result = SrcFormatter.Format(ua.Stock());
                                        break;
                                    }

                                case "消費ＥＮ":
                                    {
                                        str_result = SrcFormatter.Format(ua.AbilityENConsumption());
                                        break;
                                    }

                                case "必要気力":
                                    {
                                        str_result = SrcFormatter.Format(ua.Data.NecessaryMorale);
                                        break;
                                    }

                                case "属性":
                                    {
                                        str_result = ua.Data.Class;
                                        break;
                                    }

                                case "属性所有":
                                    {
                                        if (ua.IsAbilityClassifiedAs(@params[idx + 1]))
                                        {
                                            str_result = "1";
                                        }
                                        else
                                        {
                                            str_result = "0";
                                        }

                                        break;
                                    }

                                case "属性レベル":
                                    {
                                        str_result = ua.AbilityLevel(@params[idx + 1]).ToString();
                                        break;
                                    }

                                // TODO Impl Help
                                //case "属性名称":
                                //    {
                                //        str_result = Help.AttributeName(u, @params[idx + 1], true);
                                //        break;
                                //    }

                                //case "属性解説":
                                //    {
                                //        str_result = Help.AttributeHelpMessage(u, @params[idx + 1], i, true);
                                //        break;
                                //    }

                                case "必要技能":
                                    {
                                        str_result = ua.Data.NecessarySkill;
                                        break;
                                    }

                                case "使用可":
                                    {
                                        if (ua.IsAbilityAvailable("移動前"))
                                        {
                                            str_result = "1";
                                        }
                                        else
                                        {
                                            str_result = "0";
                                        }

                                        break;
                                    }

                                case "修得":
                                    {
                                        if (ua.IsAbilityMastered())
                                        {
                                            str_result = "1";
                                        }
                                        else
                                        {
                                            str_result = "0";
                                        }

                                        break;
                                    }
                            }
                        }
                        else if (ud != null)
                        {
                            str_result = InfoAbilitiyData(@params, idx, ud.Abilities);
                        }
                        else if (p != null)
                        {
                            str_result = InfoAbilitiyData(@params, idx, p.Data.Abilities);
                        }
                        else if (pd != null)
                        {
                            str_result = InfoAbilitiyData(@params, idx, pd.Abilities);
                        }
                        else if (it != null)
                        {
                            str_result = InfoAbilitiyData(@params, idx, it.Abilities);
                        }
                        else if (itd != null)
                        {
                            str_result = InfoAbilitiyData(@params, idx, itd.Abilities);
                        }

                        break;
                    }

                case "ランク":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(u.Rank);
                        }

                        break;
                    }

                case "ボスランク":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(u.BossRank);
                        }

                        break;
                    }

                case "エリア":
                    {
                        if (u != null)
                        {
                            str_result = u.Area;
                        }

                        break;
                    }

                case "思考モード":
                    {
                        if (u != null)
                        {
                            str_result = u.Mode;
                        }

                        break;
                    }

                case "最大攻撃力":
                    {
                        if (u != null)
                        {
                            var max_value = u.Weapons.Where(x => x.IsEnableForInfo())
                                .Select(x => x.WeaponPower(""))
                                .Append(0)
                                .Max();
                            str_result = SrcFormatter.Format(max_value);
                        }
                        else if (ud != null)
                        {
                            var max_value = ud.Weapons.Where(x => Strings.InStr(x.Class, "合") == 0)
                                .Select(x => x.Power)
                                .Append(0)
                                .Max();
                            str_result = SrcFormatter.Format(max_value);
                        }
                        break;
                    }

                case "最長射程":
                    {
                        if (u != null)
                        {
                            var max_value = u.Weapons.Where(x => x.IsEnableForInfo())
                                .Select(x => x.WeaponMaxRange())
                                .Append(0)
                                .Max();
                            str_result = SrcFormatter.Format(max_value);
                        }
                        else if (ud != null)
                        {
                            var max_value = ud.Weapons.Where(x => Strings.InStr(x.Class, "合") == 0)
                                .Select(x => x.MaxRange)
                                .Append(0)
                                .Max();
                            str_result = SrcFormatter.Format(max_value);
                        }
                        break;
                    }

                case "残りサポートアタック数":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(u.MaxSupportAttack() - u.UsedSupportAttack);
                        }

                        break;
                    }

                case "残りサポートガード数":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(u.MaxSupportGuard() - u.UsedSupportGuard);
                        }

                        break;
                    }

                case "残り同時援護攻撃数":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(u.MaxSyncAttack() - u.UsedSyncAttack);
                        }

                        break;
                    }

                case "残りカウンター攻撃数":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(u.MaxCounterAttack() - u.UsedCounterAttack);
                        }

                        break;
                    }

                case "改造費":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(SRC.InterMission.RankUpCost(u));
                        }

                        break;
                    }

                case "最大改造数":
                    {
                        if (u != null)
                        {
                            str_result = SrcFormatter.Format(SRC.InterMission.MaxRank(u));
                        }

                        break;
                    }

                case "アイテムクラス":
                    {
                        if (it != null)
                        {
                            str_result = it.Class();
                        }
                        else if (itd != null)
                        {
                            str_result = itd.Class;
                        }

                        break;
                    }

                case "装備個所":
                    {
                        if (it != null)
                        {
                            str_result = it.Part();
                        }
                        else if (itd != null)
                        {
                            str_result = itd.Part;
                        }

                        break;
                    }

                case "最大ＨＰ修正値":
                    {
                        if (it != null)
                        {
                            str_result = SrcFormatter.Format(it.HP());
                        }
                        else if (itd != null)
                        {
                            str_result = SrcFormatter.Format(itd.HP);
                        }

                        break;
                    }

                case "最大ＥＮ修正値":
                    {
                        if (it != null)
                        {
                            str_result = SrcFormatter.Format(it.EN());
                        }
                        else if (itd != null)
                        {
                            str_result = SrcFormatter.Format(itd.EN);
                        }

                        break;
                    }

                case "装甲修正値":
                    {
                        if (it != null)
                        {
                            str_result = SrcFormatter.Format(it.Armor());
                        }
                        else if (itd != null)
                        {
                            str_result = SrcFormatter.Format(itd.Armor);
                        }

                        break;
                    }

                case "運動性修正値":
                    {
                        if (it != null)
                        {
                            str_result = SrcFormatter.Format(it.Mobility());
                        }
                        else if (itd != null)
                        {
                            str_result = SrcFormatter.Format(itd.Mobility);
                        }

                        break;
                    }

                case "移動力修正値":
                    {
                        if (it != null)
                        {
                            str_result = SrcFormatter.Format(it.Speed());
                        }
                        else if (itd != null)
                        {
                            str_result = SrcFormatter.Format(itd.Speed);
                        }

                        break;
                    }

                case "解説文":
                case "コメント":
                    {
                        if (it != null)
                        {
                            str_result = it.Data.Comment?.ReplaceNewLine(" ");
                        }
                        else if (itd != null)
                        {
                            str_result = itd.Comment?.ReplaceNewLine(" ");
                        }
                        else if (spd != null)
                        {
                            str_result = spd.Comment;
                        }

                        break;
                    }

                case "短縮名":
                    {
                        if (spd != null)
                        {
                            str_result = spd.ShortName;
                        }

                        break;
                    }

                case "消費ＳＰ":
                    {
                        if (spd != null)
                        {
                            str_result = SrcFormatter.Format(spd.SPConsumption);
                        }

                        break;
                    }

                case "対象":
                    {
                        if (spd != null)
                        {
                            str_result = spd.TargetType;
                        }

                        break;
                    }

                case "持続期間":
                    {
                        if (spd != null)
                        {
                            str_result = spd.Duration;
                        }

                        break;
                    }

                case "適用条件":
                    {
                        if (spd != null)
                        {
                            str_result = spd.NecessaryCondition;
                        }

                        break;
                    }

                case "アニメ":
                    {
                        if (spd != null)
                        {
                            str_result = spd.Animation;
                        }

                        break;
                    }

                case "効果数":
                    {
                        if (spd != null)
                        {
                            str_result = SrcFormatter.Format(spd.CountEffect());
                        }

                        break;
                    }

                case "効果タイプ":
                    {
                        if (spd != null)
                        {
                            idx = (idx + 1);
                            var i = GeneralLib.StrToLng(@params[idx]);
                            if (1 <= i && i <= spd.CountEffect())
                            {
                                str_result = spd.Effects.SafeRefOneOffset(i).strEffectType;
                            }
                        }

                        break;
                    }

                case "効果レベル":
                    {
                        if (spd != null)
                        {
                            idx = (idx + 1);
                            var i = GeneralLib.StrToLng(@params[idx]);
                            if (1 <= i && i <= spd.CountEffect())
                            {
                                str_result = SrcFormatter.Format(spd.Effects.SafeRefOneOffset(i).dblEffectLevel);
                            }
                        }

                        break;
                    }

                case "効果データ":
                    {
                        if (spd != null)
                        {
                            idx = (idx + 1);
                            var i = GeneralLib.StrToLng(@params[idx]);
                            if (1 <= i && i <= spd.CountEffect())
                            {
                                str_result = spd.Effects.SafeRefOneOffset(i).strEffectData;
                            }
                        }

                        break;
                    }

                case "マップ":
                    {
                        idx = (idx + 1);
                        switch (@params[idx] ?? "")
                        {
                            case "ファイル名":
                                {
                                    str_result = SRC.Map.MapFileName;
                                    if (Strings.Len(str_result) > Strings.Len(SRC.ScenarioPath))
                                    {
                                        if ((Strings.Left(str_result, Strings.Len(SRC.ScenarioPath)) ?? "") == (SRC.ScenarioPath ?? ""))
                                        {
                                            str_result = Strings.Mid(str_result, Strings.Len(SRC.ScenarioPath) + 1);
                                        }
                                    }

                                    break;
                                }

                            case "幅":
                                {
                                    str_result = SrcFormatter.Format(SRC.Map.MapWidth);
                                    break;
                                }

                            case "時間帯":
                                {
                                    if (!string.IsNullOrEmpty(SRC.Map.MapDrawMode))
                                    {
                                        string buf;
                                        if (SRC.Map.MapDrawMode == "フィルタ")
                                        {
                                            buf = SRC.Map.MapDrawFilterColor.ToHexString();
                                        }
                                        else
                                        {
                                            buf = SRC.Map.MapDrawMode;
                                        }

                                        if (SRC.Map.MapDrawIsMapOnly)
                                        {
                                            buf = buf + " マップ限定";
                                        }

                                        str_result = buf;
                                    }
                                    else
                                    {
                                        str_result = "昼";
                                    }

                                    break;
                                }

                            case "高さ":
                                {
                                    str_result = SrcFormatter.Format(SRC.Map.MapHeight);
                                    break;
                                }

                            default:
                                {
                                    if (GeneralLib.IsNumber(@params[idx]))
                                    {
                                        mx = Conversions.ToInteger(@params[idx]);
                                    }

                                    idx = (idx + 1);
                                    if (GeneralLib.IsNumber(@params[idx]))
                                    {
                                        my = Conversions.ToInteger(@params[idx]);
                                    }

                                    if (mx < 1 || SRC.Map.MapWidth < mx || my < 1 || SRC.Map.MapHeight < my)
                                    {
                                        return ValueType.StringType;
                                    }

                                    idx = (idx + 1);
                                    switch (@params[idx] ?? "")
                                    {
                                        case "地形名":
                                            {
                                                str_result = SRC.Map.Terrain(mx, my).Name;
                                                break;
                                            }

                                        case "地形タイプ":
                                        case "地形クラス":
                                            {
                                                str_result = SRC.Map.Terrain(mx, my).Class;
                                                break;
                                            }

                                        case "移動コスト":
                                            {
                                                // 0.5刻みの移動コストを使えるようにするため、移動コストは
                                                // 実際の２倍の値で記録されている
                                                str_result = SrcFormatter.Format(SRC.Map.Terrain(mx, my).MoveCost / 2d);
                                                break;
                                            }

                                        case "回避修正":
                                            {
                                                str_result = SrcFormatter.Format(SRC.Map.Terrain(mx, my).HitMod);
                                                break;
                                            }

                                        case "ダメージ修正":
                                            {
                                                str_result = SrcFormatter.Format(SRC.Map.Terrain(mx, my).DamageMod);
                                                break;
                                            }

                                        case "ＨＰ回復量":
                                            {
                                                str_result = SrcFormatter.Format(SRC.Map.Terrain(mx, my).EffectForHPRecover());
                                                break;
                                            }

                                        case "ＥＮ回復量":
                                            {
                                                str_result = SrcFormatter.Format(SRC.Map.Terrain(mx, my).EffectForENRecover());
                                                break;
                                            }

                                        case "ビットマップ名":
                                            {
                                                // XXX 多分一度レンダリングしてないと取れない
                                                str_result = SRC.Map.MapUnderImageFilePath[mx, my];

                                                break;
                                            }
                                        case "レイヤービットマップ名":
                                            {
                                                // XXX 未対応
                                                str_result = SRC.Map.MapUpperImageFilePath[mx, my];
                                                break;
                                            }
                                        case "ユニットＩＤ":
                                            {
                                                if (SRC.Map.MapDataForUnit[mx, my] != null)
                                                {
                                                    str_result = SRC.Map.MapDataForUnit[mx, my].ID;
                                                }

                                                break;
                                            }
                                    }

                                    break;
                                }
                        }

                        break;
                    }

                case "オプション":
                    {
                        idx = (idx + 1);
                        switch (@params[idx] ?? "")
                        {
                            case "MessageWait":
                                {
                                    str_result = SrcFormatter.Format(SRC.GUI.MessageWait);
                                    break;
                                }

                            case "BattleAnimation":
                                {
                                    if (SRC.BattleAnimation)
                                    {
                                        str_result = "On";
                                    }
                                    else
                                    {
                                        str_result = "Off";
                                    }

                                    break;
                                }
                            case "ExtendedAnimation":
                                {
                                    if (SRC.ExtendedAnimation)
                                    {
                                        str_result = "On";
                                    }
                                    else
                                    {
                                        str_result = "Off";
                                    }

                                    break;
                                }
                            case "SpecialPowerAnimation":
                                {
                                    if (SRC.SpecialPowerAnimation)
                                    {
                                        str_result = "On";
                                    }
                                    else
                                    {
                                        str_result = "Off";
                                    }

                                    break;
                                }

                            case "AutoDeffence":
                                {
                                    if (SRC.SystemConfig.AutoDefense)
                                    {
                                        str_result = "On";
                                    }
                                    else
                                    {
                                        str_result = "Off";
                                    }

                                    break;
                                }

                            case "UseDirectMusic":
                                {
                                    // SRC# に UseDirectMusic という概念はない
                                    str_result = "Off";
                                    break;
                                }
                            case "Turn":
                            case "Square":
                            case "KeepEnemyBGM":
                            case "MidiReset":
                            case "AutoMoveCursor":
                            case "DebugMode":
                            case "LastFolder":
                            case "MIDIPortID":
                            case "MP3Volume":
                            case var case13 when case13 == "BattleAnimation":
                            case "WeaponAnimation":
                            case "MoveAnimation":
                            case "ImageBufferNum":
                            case "MaxImageBufferSize":
                            case "KeepStretchedImage":
                            case "UseTransparentBlt":
                            case "NewGUI":
                                {
                                    str_result = SRC.SystemConfig.GetItem("Option", @params[idx]);
                                    break;
                                }

                            default:
                                {
                                    // Optionコマンドのオプションを参照
                                    if (SRC.Expression.IsOptionDefined(@params[idx]))
                                    {
                                        str_result = "On";
                                    }
                                    else
                                    {
                                        str_result = "Off";
                                    }

                                    break;
                                }
                        }

                        break;
                    }
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

        private static string InfoWeaponData(string[] @params, int idx, IList<WeaponData> weapons)
        {
            // 何番目の武器かを判定
            var str_result = "";
            WeaponData wd;
            if (GeneralLib.IsNumber(@params[idx]))
            {
                wd = weapons.SafeRefOneOffset(Conversions.ToInteger(@params[idx]));
            }
            else
            {
                wd = weapons.FirstOrDefault(x => x.Name == @params[idx]);
            }
            // 指定した武器を持っていない
            if (wd == null)
            {
                return "";
            }

            idx = (idx + 1);
            switch (@params[idx] ?? "")
            {
                case var case2 when case2 == "":
                case "名称":
                    {
                        str_result = wd.Name;
                        break;
                    }

                case "攻撃力":
                    {
                        str_result = SrcFormatter.Format(wd.Power);
                        break;
                    }

                case "射程":
                case "最大射程":
                    {
                        str_result = SrcFormatter.Format(wd.MaxRange);
                        break;
                    }

                case "最小射程":
                    {
                        str_result = SrcFormatter.Format(wd.MinRange);
                        break;
                    }

                case "命中率":
                    {
                        str_result = SrcFormatter.Format(wd.Precision);
                        break;
                    }

                case "最大弾数":
                case "弾数":
                    {
                        str_result = SrcFormatter.Format(wd.Bullet);
                        break;
                    }

                case "消費ＥＮ":
                    {
                        str_result = SrcFormatter.Format(wd.ENConsumption);
                        break;
                    }

                case "必要気力":
                    {
                        str_result = SrcFormatter.Format(wd.NecessaryMorale);
                        break;
                    }

                case "地形適応":
                    {
                        str_result = wd.Adaption;
                        break;
                    }

                case "クリティカル率":
                    {
                        str_result = SrcFormatter.Format(wd.Critical);
                        break;
                    }

                case "属性":
                    {
                        str_result = wd.Class;
                        break;
                    }

                case "属性所有":
                    {
                        if (GeneralLib.InStrNotNest(wd.Class, @params[idx + 1]) > 0)
                        {
                            str_result = "1";
                        }
                        else
                        {
                            str_result = "0";
                        }

                        break;
                    }

                case "属性レベル":
                    {
                        var j = GeneralLib.InStrNotNest(wd.Class, @params[idx + 1] + "L");
                        if (j == 0)
                        {
                            str_result = "0";
                            return str_result;
                        }

                        str_result = "";
                        j = (j + Strings.Len(@params[idx + 1]) + 1);
                        do
                        {
                            str_result = str_result + Strings.Mid(wd.Class, j, 1);
                            j = (j + 1);
                        }
                        while (GeneralLib.IsNumber(Strings.Mid(wd.Class, j, 1)));
                        if (!GeneralLib.IsNumber(str_result))
                        {
                            str_result = "0";
                        }

                        break;
                    }

                case "必要技能":
                    {
                        str_result = wd.NecessarySkill;
                        break;
                    }

                case "使用可":
                case "修得":
                    {
                        str_result = "1";
                        break;
                    }
            }

            return str_result;
        }

        private static string InfoAbilitiyData(string[] @params, int idx, IList<AbilityData> abilities)
        {
            // 何番目のアビリティかを判定
            var str_result = "";
            AbilityData ad;
            if (GeneralLib.IsNumber(@params[idx]))
            {
                ad = abilities.SafeRefOneOffset(Conversions.ToInteger(@params[idx]));
            }
            else
            {
                var name = @params[idx];
                ad = abilities.FirstOrDefault(x => x.Name == name);
            }
            // 指定したアビリティを持っていない
            if (ad == null)
            {
                return "";
            }

            idx = (idx + 1);
            switch (@params[idx] ?? "")
            {
                case var case12 when case12 == "":
                case "名称":
                    {
                        str_result = ad.Name;
                        break;
                    }

                case "効果数":
                    {
                        str_result = SrcFormatter.Format(ad.Effects.Count);
                        break;
                    }

                case "効果タイプ":
                    {
                        // 何番目の効果かを判定
                        if (GeneralLib.IsNumber(@params[idx + 1]))
                        {
                            var j = Conversions.ToInteger(@params[idx + 1]);
                            str_result = ad.Effects.SafeRefOneOffset(j).EffectType;
                        }
                        break;
                    }

                case "効果レベル":
                    {
                        // 何番目の効果かを判定
                        if (GeneralLib.IsNumber(@params[idx + 1]))
                        {
                            var j = Conversions.ToInteger(@params[idx + 1]);
                            str_result = SrcFormatter.Format(ad.Effects.SafeRefOneOffset(j).Level);
                        }
                        break;
                    }

                case "効果データ":
                    {
                        // 何番目の効果かを判定
                        if (GeneralLib.IsNumber(@params[idx + 1]))
                        {
                            var j = Conversions.ToInteger(@params[idx + 1]);
                            str_result = ad.Effects.SafeRefOneOffset(j).Data;
                        }
                        break;
                    }

                case "射程":
                case "最大射程":
                    {
                        str_result = SrcFormatter.Format(ad.MaxRange);
                        break;
                    }

                case "最小射程":
                    {
                        str_result = SrcFormatter.Format(ad.MinRange);
                        break;
                    }

                case "最大使用回数":
                case "使用回数":
                    {
                        str_result = SrcFormatter.Format(ad.Stock);
                        break;
                    }

                case "消費ＥＮ":
                    {
                        str_result = SrcFormatter.Format(ad.ENConsumption);
                        break;
                    }

                case "必要気力":
                    {
                        str_result = SrcFormatter.Format(ad.NecessaryMorale);
                        break;
                    }

                case "属性":
                    {
                        str_result = ad.Class;
                        break;
                    }

                case "属性所有":
                    {
                        if (GeneralLib.InStrNotNest(ad.Class, @params[idx + 1]) > 0)
                        {
                            str_result = "1";
                        }
                        else
                        {
                            str_result = "0";
                        }

                        break;
                    }

                case "属性レベル":
                    {
                        var j = GeneralLib.InStrNotNest(ad.Class, @params[idx + 1] + "L");
                        if (j == 0)
                        {
                            str_result = "0";
                            return str_result;
                        }

                        str_result = "";
                        j = (j + Strings.Len(@params[idx + 1]) + 1);
                        do
                        {
                            str_result = str_result + Strings.Mid(ad.Class, j, 1);
                            j = (j + 1);
                        }
                        while (GeneralLib.IsNumber(Strings.Mid(ad.Class, j, 1)));
                        if (!GeneralLib.IsNumber(str_result))
                        {
                            str_result = "0";
                        }

                        break;
                    }

                case "必要技能":
                    {
                        str_result = ad.NecessarySkill;
                        break;
                    }

                case "使用可":
                case "修得":
                    {
                        str_result = "1";
                        break;
                    }
            }

            return str_result;
        }
    }
}