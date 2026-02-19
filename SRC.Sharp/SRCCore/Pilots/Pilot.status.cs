// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Pilots
{
    public partial class Pilot
    {
        // 能力値を更新
        public void Update()
        {
            #region // 現在のレベルで使用可能な特殊能力の一覧を作成

            // 以前の一覧を削除
            colSkill.Clear();

            // パイロットデータを参照しながら使用可能な特殊能力を検索
            var effectiveSkills = new List<SkillData>();
            foreach (SkillData sd in Data.Skills)
            {
                if (Level >= sd.NecessaryLevel)
                {
                    SkillData registerd;
                    // 既に登録済み？
                    if (sd.Name == "ＳＰ消費減少" || sd.Name == "スペシャルパワー自動発動" || sd.Name == "ハンター")
                    {
                        // これらの特殊能力は同種の能力を複数持つことが出来る
                        // ただしデータ指定まで同一であれば同じ能力と見なす
                        registerd = effectiveSkills.FirstOrDefault(x => x.Name == sd.Name && x.StrData == x.StrData);
                    }
                    else
                    {
                        registerd = effectiveSkills.FirstOrDefault(x => x.Name == sd.Name);
                    }

                    if (registerd == null)
                    {
                        // 未登録
                        effectiveSkills.Add(sd);
                    }
                    else if (sd.NecessaryLevel > registerd.NecessaryLevel)
                    {
                        // 登録済みである場合は習得レベルが高いものを優先
                        effectiveSkills.Remove(registerd);
                        effectiveSkills.Add(sd);
                    }
                }
            }

            // TODO Impl SetSkillコマンドで付加された特殊能力を検索
            //    // SetSkillコマンドで付加された特殊能力を検索
            //    string sname, alist, sdata;
            //    string buf;
            //    if (Expression.IsGlobalVariableDefined("Ability(" + ID + ")"))
            //    {

            //        // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //        alist = Conversions.ToString(Event.GlobalVariableList["Ability(" + ID + ")"].StringValue);
            //        var loopTo3 = GeneralLib.LLength(alist);
            //        for (i = 1; i <= loopTo3; i++)
            //        {
            //            sname = GeneralLib.LIndex(alist, i);
            //            // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //            buf = Conversions.ToString(Event.GlobalVariableList["Ability(" + ID + "," + sname + ")"].StringValue);
            //            sdata = GeneralLib.ListTail(buf, 2);

            //            // 既に登録済み？
            //            if (sname == "ＳＰ消費減少" || sname == "スペシャルパワー自動発動" || sname == "ハンター")
            //            {
            //                // これらの特殊能力は同種の能力を複数持つことが出来る
            //                var loopTo4 = skill_num;
            //                for (j = 1; j <= loopTo4; j++)
            //                {
            //                    if ((sname ?? "") == (skill_name[j] ?? ""))
            //                    {
            //                        if ((sdata ?? "") == (skill_data[j].StrData ?? ""))
            //                        {
            //                            // ただしデータ指定まで同一であれば同じ能力と見なす
            //                            break;
            //                        }
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                var loopTo5 = skill_num;
            //                for (j = 1; j <= loopTo5; j++)
            //                {
            //                    if ((sname ?? "") == (skill_name[j] ?? ""))
            //                    {
            //                        break;
            //                    }
            //                }
            //            }

            //            if (j > skill_num)
            //            {
            //                // 未登録
            //                skill_num = j;
            //                skill_name[j] = sname;
            //            }

            //            if (GeneralLib.StrToDbl(GeneralLib.LIndex(buf, 1)) == 0d)
            //            {
            //                // レベル0の場合は能力を封印
            //                // UPGRADE_NOTE: オブジェクト skill_data() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                skill_data[j] = null;
            //            }
            //            else
            //            {
            //                // PDListのデータを書き換えるわけにはいかないので
            //                // アビリティデータを新規に作成
            //                sd = new SkillData();
            //                sd.Name = sname;
            //                sd.Level = GeneralLib.StrToDbl(GeneralLib.LIndex(buf, 1));
            //                if (sd.Level == -1)
            //                {
            //                    sd.Level = Constants.DEFAULT_LEVEL;
            //                }

            //                sd.StrData = GeneralLib.ListTail(buf, 2);
            //                skill_data[j] = sd;
            //            }
            //        }
            //    }

            //    // 属性使用不能状態の際、対応する技能を封印する。
            //    if (Unit is object)
            //    {
            //        var loopTo6 = skill_num;
            //        for (j = 1; j <= loopTo6; j++)
            //        {
            //            if (skill_data[j] is object)
            //            {
            //                if (Unit.ConditionLifetime(skill_data[j].Name + "使用不能") > 0)
            //                {
            //                    // UPGRADE_NOTE: オブジェクト skill_data() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                    skill_data[j] = null;
            //                }
            //            }
            //        }
            //    }

            // 使用可能な特殊能力を登録
            {
                var i = 0;
                foreach (var sd in effectiveSkills)
                {
                    i++;
                    switch (sd.Name ?? "")
                    {
                        case "ＳＰ消費減少":
                        case "スペシャルパワー自動発動":
                        case "ハンター":
                            if (colSkill.ContainsKey(sd.Name))
                            {
                                colSkill.Add(sd, sd.Name + ":" + i);
                            }
                            else
                            {
                                colSkill.Add(sd, sd.Name);
                            }
                            break;

                        default:
                            colSkill.Add(sd, sd.Name);
                            break;
                    }
                }
            }
            #endregion

            // これから下は能力値の計算

            #region 基本値
            InfightBase = Data.Infight;
            ShootingBase = Data.Shooting;
            HitBase = Data.Hit;
            DodgeBase = Data.Dodge;
            TechniqueBase = Data.Technique;
            IntuitionBase = Data.Intuition;
            Adaption = Data.Adaption;

            // レベルによる追加分
            var lv = Level + SkillLevel("追加レベル", "");
            if (Expression.IsOptionDefined("攻撃力低成長"))
            {
                InfightBase = (InfightBase + (int)(lv * (1d + 2d * SkillLevel("格闘成長", ""))) / 2);
                ShootingBase = (ShootingBase + (int)(lv * (1d + 2d * SkillLevel("射撃成長", ""))) / 2);
            }
            else
            {
                InfightBase = (InfightBase + (int)(lv * (1d + SkillLevel("格闘成長", ""))));
                ShootingBase = (ShootingBase + (int)(lv * (1d + SkillLevel("射撃成長", ""))));
            }

            HitBase = (HitBase + (int)(lv * (2d + SkillLevel("命中成長", ""))));
            DodgeBase = (DodgeBase + (int)(lv * (2d + SkillLevel("回避成長", ""))));
            TechniqueBase = (TechniqueBase + (int)(lv * (1d + SkillLevel("技量成長", ""))));
            IntuitionBase = (IntuitionBase + (int)(lv * (1d + SkillLevel("反応成長", ""))));

            // 能力ＵＰ
            InfightBase = (InfightBase + (int)SkillLevel("格闘ＵＰ", ""));
            ShootingBase = (ShootingBase + (int)SkillLevel("射撃ＵＰ", ""));
            HitBase = (HitBase + (int)SkillLevel("命中ＵＰ", ""));
            DodgeBase = (DodgeBase + (int)SkillLevel("回避ＵＰ", ""));
            TechniqueBase = (TechniqueBase + (int)SkillLevel("技量ＵＰ", ""));
            IntuitionBase = (IntuitionBase + (int)SkillLevel("反応ＵＰ", ""));

            // 能力ＤＯＷＮ
            InfightBase = (InfightBase - (int)SkillLevel("格闘ＤＯＷＮ", ""));
            ShootingBase = (ShootingBase - (int)SkillLevel("射撃ＤＯＷＮ", ""));
            HitBase = (HitBase - (int)SkillLevel("命中ＤＯＷＮ", ""));
            DodgeBase = (DodgeBase - (int)SkillLevel("回避ＤＯＷＮ", ""));
            TechniqueBase = (TechniqueBase - (int)SkillLevel("技量ＤＯＷＮ", ""));
            IntuitionBase = (IntuitionBase - (int)SkillLevel("反応ＤＯＷＮ", ""));

            // 上限を超えないように
            InfightBase = GeneralLib.MinLng(InfightBase, 9999);
            ShootingBase = GeneralLib.MinLng(ShootingBase, 9999);
            HitBase = GeneralLib.MinLng(HitBase, 9999);
            DodgeBase = GeneralLib.MinLng(DodgeBase, 9999);
            TechniqueBase = GeneralLib.MinLng(TechniqueBase, 9999);
            IntuitionBase = GeneralLib.MinLng(IntuitionBase, 9999);
            #endregion

            // これから下は特殊能力による修正値の計算

            // まずは修正値を初期化
            InfightMod = 0;
            ShootingMod = 0;
            HitMod = 0;
            DodgeMod = 0;
            TechniqueMod = 0;
            IntuitionMod = 0;

            #region パイロット用特殊能力による修正
            lv = SkillLevel("超感覚", "");
            if (lv > 0d)
            {
                HitMod = (int)(HitMod + 2d * lv + 3d);
                DodgeMod = (int)(DodgeMod + 2d * lv + 3d);
            }

            lv = SkillLevel("知覚強化", "");
            if (lv > 0d)
            {
                HitMod = (int)(HitMod + 2d * lv + 3d);
                DodgeMod = (int)(DodgeMod + 2d * lv + 3d);
            }

            lv = SkillLevel("念力", "");
            if (lv > 0d)
            {
                HitMod = (int)(HitMod + 2d * lv + 3d);
                DodgeMod = (int)(DodgeMod + 2d * lv + 3d);
            }

            lv = SkillLevel("超反応", "");
            HitMod = (int)(HitMod + 2d * lv);
            DodgeMod = (int)(DodgeMod + 2d * lv);

            if (IsSkillAvailable("サイボーグ"))
            {
                HitMod = (HitMod + 5);
                DodgeMod = (DodgeMod + 5);
            }

            if (IsSkillAvailable("悟り"))
            {
                HitMod = (HitMod + 10);
                DodgeMod = (DodgeMod + 10);
            }

            if (IsSkillAvailable("超能力"))
            {
                HitMod = (HitMod + 5);
                DodgeMod = (DodgeMod + 5);
            }
            #endregion

            #region これから下はユニットによる修正値の計算
            // ユニットに乗っていない？
            if (Unit is null)
            {
                goto SkipUnitMod;
            }

            {
                var u = Unit;
                // クイックセーブ処理などで実際には乗っていない場合
                if (u.CountPilot() == 0)
                {
                    return;
                }

                // サブパイロット＆サポートパイロットによるサポート
                if (ReferenceEquals(this, u.MainPilot()) && u.Status == "出撃")
                {
                    foreach (var sp in u.SubPilots)
                    {
                        InfightMod = (int)(InfightMod + 2d * sp.SkillLevel("格闘サポート", ref_mode: ""));
                        if (HasMana())
                        {
                            ShootingMod = (int)(ShootingMod + 2d * sp.SkillLevel("魔力サポート", ref_mode: ""));
                        }
                        else
                        {
                            ShootingMod = (int)(ShootingMod + 2d * sp.SkillLevel("射撃サポート", ref_mode: ""));
                        }

                        HitMod = (int)(HitMod + 3d * sp.SkillLevel("サポート", ref_mode: ""));
                        HitMod = (int)(HitMod + 2d * sp.SkillLevel("命中サポート", ref_mode: ""));
                        DodgeMod = (int)(DodgeMod + 3d * sp.SkillLevel("サポート", ref_mode: ""));
                        DodgeMod = (int)(DodgeMod + 2d * sp.SkillLevel("回避サポート", ref_mode: ""));
                        TechniqueMod = (int)(TechniqueMod + 2d * sp.SkillLevel("技量サポート", ref_mode: ""));
                        IntuitionMod = (int)(IntuitionMod + 2d * sp.SkillLevel("反応サポート", ref_mode: ""));
                    }

                    foreach (var sp in u.Supports)
                    {
                        InfightMod = (int)(InfightMod + 2d * sp.SkillLevel("格闘サポート", ref_mode: ""));
                        if (HasMana())
                        {
                            ShootingMod = (int)(ShootingMod + 2d * sp.SkillLevel("魔力サポート", ref_mode: ""));
                        }
                        else
                        {
                            ShootingMod = (int)(ShootingMod + 2d * sp.SkillLevel("射撃サポート", ref_mode: ""));
                        }

                        HitMod = (int)(HitMod + 3d * sp.SkillLevel("サポート", ref_mode: ""));
                        HitMod = (int)(HitMod + 2d * sp.SkillLevel("命中サポート", ref_mode: ""));
                        DodgeMod = (int)(DodgeMod + 3d * sp.SkillLevel("サポート", ref_mode: ""));
                        DodgeMod = (int)(DodgeMod + 2d * sp.SkillLevel("回避サポート", ref_mode: ""));
                        TechniqueMod = (int)(TechniqueMod + 2d * sp.SkillLevel("技量サポート", ref_mode: ""));
                        IntuitionMod = (int)(IntuitionMod + 2d * sp.SkillLevel("反応サポート", ref_mode: ""));
                    }

                    if (u.IsFeatureAvailable("追加サポート"))
                    {
                        var asp = u.AdditionalSupport();
                        if (asp != null)
                        {
                            InfightMod = (int)(InfightMod + 2d * asp.SkillLevel("格闘サポート", ref_mode: ""));
                            if (HasMana())
                            {
                                ShootingMod = (int)(ShootingMod + 2d * asp.SkillLevel("魔力サポート", ref_mode: ""));
                            }
                            else
                            {
                                ShootingMod = (int)(ShootingMod + 2d * asp.SkillLevel("射撃サポート", ref_mode: ""));
                            }

                            HitMod = (int)(HitMod + 3d * asp.SkillLevel("サポート", ref_mode: ""));
                            HitMod = (int)(HitMod + 2d * asp.SkillLevel("命中サポート", ref_mode: ""));
                            DodgeMod = (int)(DodgeMod + 3d * asp.SkillLevel("サポート", ref_mode: ""));
                            DodgeMod = (int)(DodgeMod + 2d * asp.SkillLevel("回避サポート", ref_mode: ""));
                            TechniqueMod = (int)(TechniqueMod + 2d * asp.SkillLevel("技量サポート", ref_mode: ""));
                            IntuitionMod = (int)(IntuitionMod + 2d * asp.SkillLevel("反応サポート", ref_mode: ""));
                        }
                    }
                }

                // ユニット＆アイテムによる強化
                foreach (var f in u.Features)
                {
                    switch (f.Name)
                    {
                        case "格闘強化":
                            if (Morale >= GeneralLib.StrToLng(GeneralLib.LIndex(f.Data, 2)))
                            {
                                InfightMod = (int)(InfightMod + 5d * f.FeatureLevel);
                            }
                            break;
                        case "射撃強化":
                            if (Morale >= GeneralLib.StrToLng(GeneralLib.LIndex(f.Data, 2)))
                            {
                                ShootingMod = (int)(ShootingMod + 5d * f.FeatureLevel);
                            }
                            break;
                        case "命中強化":
                            if (Morale >= GeneralLib.StrToLng(GeneralLib.LIndex(f.Data, 2)))
                            {
                                HitMod = (int)(HitMod + 5d * f.FeatureLevel);
                            }
                            break;
                        case "回避強化":
                            if (Morale >= GeneralLib.StrToLng(GeneralLib.LIndex(f.Data, 2)))
                            {
                                DodgeMod = (int)(DodgeMod + 5d * f.FeatureLevel);
                            }
                            break;
                        case "技量強化":
                            if (Morale >= GeneralLib.StrToLng(GeneralLib.LIndex(f.Data, 2)))
                            {
                                TechniqueMod = (int)(TechniqueMod + 5d * f.FeatureLevel);
                            }
                            break;
                        case "反応強化":
                            if (Morale >= GeneralLib.StrToLng(GeneralLib.LIndex(f.Data, 2)))
                            {
                                IntuitionMod = (int)(IntuitionMod + 5d * f.FeatureLevel);
                            }
                            break;
                    }
                }
            }
        SkipUnitMod:
            ;

            //        // 地形適応変更
            //        if (withBlock4.IsFeatureAvailable("パイロット地形適応変更"))
            //        {
            //            for (i = 1; i <= 4; i++)
            //            {
            //                switch (Strings.Mid(Adaption, i, 1) ?? "")
            //                {
            //                    case "S":
            //                        {
            //                            padaption[i] = 5;
            //                            break;
            //                        }

            //                    case "A":
            //                        {
            //                            padaption[i] = 4;
            //                            break;
            //                        }

            //                    case "B":
            //                        {
            //                            padaption[i] = 3;
            //                            break;
            //                        }

            //                    case "C":
            //                        {
            //                            padaption[i] = 2;
            //                            break;
            //                        }

            //                    case "D":
            //                        {
            //                            padaption[i] = 1;
            //                            break;
            //                        }

            //                    case "E":
            //                    case "-":
            //                        {
            //                            padaption[i] = 0;
            //                            break;
            //                        }
            //                }
            //            }

            //            // 地形適応変更能力による修正
            //            var loopTo12 = withBlock4.CountFeature();
            //            for (i = 1; i <= loopTo12; i++)
            //            {
            //                if (withBlock4.Feature(i) == "パイロット地形適応変更")
            //                {
            //                    for (j = 1; j <= 4; j++)
            //                    {
            //                        string localFeatureData8() { object argIndex1 = i; var ret = withBlock4.FeatureData(argIndex1); return ret; }

            //                        string localLIndex8() { string arglist = hsb8c32b5b97d840f1a1974442798ae710(); var ret = GeneralLib.LIndex(arglist, j); return ret; }

            //                        if (GeneralLib.StrToLng(localLIndex8()) >= 0)
            //                        {
            //                            // 修正値がプラスのとき
            //                            if (padaption[j] < 4)
            //                            {
            //                                string localFeatureData6() { object argIndex1 = i; var ret = withBlock4.FeatureData(argIndex1); return ret; }

            //                                string localLIndex6() { string arglist = hs8f1b1c4829bf4c8ea402eb72f2930e63(); var ret = GeneralLib.LIndex(arglist, j); return ret; }

            //                                int localStrToLng6() { string argexpr = hsdf089849003448bebc02763c21e401ad(); var ret = GeneralLib.StrToLng(argexpr); return ret; }

            //                                padaption[j] = (padaption[j] + localStrToLng6());
            //                                // 地形適応はAより高くはならない
            //                                if (padaption[j] > 4)
            //                                {
            //                                    padaption[j] = 4;
            //                                }
            //                            }
            //                        }
            //                        else
            //                        {
            //                            // 修正値がマイナスのときは本来の地形適応が"A"以上でも処理を行なう
            //                            string localFeatureData7() { object argIndex1 = i; var ret = withBlock4.FeatureData(argIndex1); return ret; }

            //                            string localLIndex7() { string arglist = hs8e6f6c053bcd4824a879080031dfde79(); var ret = GeneralLib.LIndex(arglist, j); return ret; }

            //                            int localStrToLng7() { string argexpr = hs210e4a3c7ff64bf2865702c94fd878d3(); var ret = GeneralLib.StrToLng(argexpr); return ret; }

            //                            padaption[j] = (padaption[j] + localStrToLng7());
            //                        }
            //                    }
            //                }
            //            }

            //            Adaption = "";
            //            for (i = 1; i <= 4; i++)
            //            {
            //                switch (padaption[i])
            //                {
            //                    case var @case when @case >= 5:
            //                        {
            //                            Adaption = Adaption + "S";
            //                            break;
            //                        }

            //                    case 4:
            //                        {
            //                            Adaption = Adaption + "A";
            //                            break;
            //                        }

            //                    case 3:
            //                        {
            //                            Adaption = Adaption + "B";
            //                            break;
            //                        }

            //                    case 2:
            //                        {
            //                            Adaption = Adaption + "C";
            //                            break;
            //                        }

            //                    case 1:
            //                        {
            //                            Adaption = Adaption + "D";
            //                            break;
            //                        }

            //                    case var case1 when case1 <= 0:
            //                        {
            //                            Adaption = Adaption + "-";
            //                            break;
            //                        }
            //                }
            //            }
            //        }
            //    }

            //    // 気力の値を気力上限・気力下限の範囲にする
            //    SetMorale(Morale);
            //SkipUnitMod:
            //    ;
            #endregion

            // 基本値と修正値の合計から実際の能力値を算出
            Infight = GeneralLib.MinLng((InfightBase + InfightMod) + InfightMod2, 9999);
            Shooting = GeneralLib.MinLng((ShootingBase + ShootingMod) + ShootingMod2, 9999);
            Hit = GeneralLib.MinLng((HitBase + HitMod) + HitMod2, 9999);
            Dodge = GeneralLib.MinLng((DodgeBase + DodgeMod) + DodgeMod2, 9999);
            Technique = GeneralLib.MinLng((TechniqueBase + TechniqueMod) + TechniqueMod2, 9999);
            Intuition = GeneralLib.MinLng((IntuitionBase + IntuitionMod) + IntuitionMod2, 9999);
        }

        // 周りのユニットによる支援効果を更新
        public void UpdateSupportMod()
        {
            Unit u, my_unit;
            string my_party;
            int my_cmd_rank;
            int cmd_rank, cmd_rank2;
            double cmd_level = Constants.DEFAULT_LEVEL;
            double cs_level;
            int range, max_range;
            bool mod_stack;
            int rel_lv = 0;
            string team, uteam;
            int j, i, k;

            // 支援効果による修正値を初期化
            Infight = (InfightBase + InfightMod);
            Shooting = (ShootingBase + ShootingMod);
            Hit = (HitBase + HitMod);
            Dodge = (DodgeBase + DodgeMod);
            Technique = (TechniqueBase + TechniqueMod);
            Intuition = (IntuitionBase + IntuitionMod);
            InfightMod2 = 0;
            ShootingMod2 = 0;
            HitMod2 = 0;
            DodgeMod2 = 0;
            TechniqueMod2 = 0;
            IntuitionMod2 = 0;
            MoraleMod = 0;

            // ステータス表示時には支援効果を無視
            if (string.IsNullOrEmpty(SRC.Map.MapFileName))
            {
                return;
            }

            // ユニットに乗っていなければここで終了
            if (Unit is null)
            {
                return;
            }

            // 一旦乗っているユニットを記録しておく
            my_unit = Unit;
            var withBlock = Unit;
            
            // ユニットが出撃していなければここで終了
            if (withBlock.Status != "出撃")
            {
                return;
            }

            if (!ReferenceEquals(Unit, SRC.Map.MapDataForUnit[withBlock.x, withBlock.y]))
            {
                return;
            }

            // メインパイロットでなければここで終了
            if (withBlock.CountPilot() == 0)
            {
                return;
            }

            if (!ReferenceEquals(this, withBlock.MainPilot()))
            {
                return;
            }

            // 正常な判断が出来ないユニットは支援を受けられない
            if (withBlock.IsConditionSatisfied("暴走"))
            {
                return;
            }

            if (withBlock.IsConditionSatisfied("混乱"))
            {
                return;
            }

            // 支援を受けられるかどうかの判定用に陣営を参照しておく
            my_party = withBlock.Party;

            // 指揮効果判定用に自分の階級レベルを算出
            if (IsSkillAvailable("階級"))
            {
                my_cmd_rank = (int)SkillLevel("階級", ref_mode: "");
                cmd_rank = my_cmd_rank;
            }
            else
            {
                if (Strings.InStr(Name, "(ザコ)") == 0 && Strings.InStr(Name, "(汎用)") == 0)
                {
                    my_cmd_rank = Constants.DEFAULT_LEVEL;
                }
                else
                {
                    my_cmd_rank = 0;
                }

                cmd_rank = 0;
            }

            // 自分が所属しているチーム名
            team = SkillData("チーム");

            // 周りのユニットを調べる
            cs_level = Constants.DEFAULT_LEVEL;
            max_range = 5;
            
            for (i = GeneralLib.MaxLng(withBlock.x - max_range, 1); i <= GeneralLib.MinLng(withBlock.x + max_range, SRC.Map.MapWidth); i++)
            {
                for (j = GeneralLib.MaxLng(withBlock.y - max_range, 1); j <= GeneralLib.MinLng(withBlock.y + max_range, SRC.Map.MapHeight); j++)
                {
                    // ユニット間の距離が範囲内？
                    range = (Math.Abs((withBlock.x - i)) + Math.Abs((withBlock.y - j)));
                    if (range > max_range)
                    {
                        continue;
                    }

                    u = SRC.Map.MapDataForUnit[i, j];
                    if (u is null)
                    {
                        continue;
                    }

                    if (ReferenceEquals(u, Unit))
                    {
                        continue;
                    }

                    var withBlock1 = u;
                    // ユニットにパイロットが乗っていなければ無視
                    if (withBlock1.CountPilot() == 0)
                    {
                        continue;
                    }

                    // 陣営が一致していないと支援は受けられない
                    switch (my_party ?? "")
                    {
                        case "味方":
                        case "ＮＰＣ":
                            switch (withBlock1.Party ?? "")
                            {
                                case "敵":
                                case "中立":
                                    goto NextUnit;
                            }
                            break;

                        default:
                            if ((my_party ?? "") != (withBlock1.Party ?? ""))
                            {
                                goto NextUnit;
                            }
                            break;
                    }

                    // 相手が正常な判断能力を持っていない場合も支援は受けられない
                    if (withBlock1.IsConditionSatisfied("暴走"))
                    {
                        goto NextUnit;
                    }

                    if (withBlock1.IsConditionSatisfied("混乱"))
                    {
                        goto NextUnit;
                    }

                    var withBlock2 = u.MainPilot(true);
                    // 同じチームに所属している？
                    uteam = withBlock2.SkillData("チーム");
                    if ((team ?? "") != (uteam ?? "") && !string.IsNullOrEmpty(uteam))
                    {
                        goto NextUnit;
                    }

                    // 広域サポート
                    if (range <= 2)
                    {
                        cs_level = GeneralLib.MaxDbl(cs_level, withBlock2.SkillLevel("広域サポート", ref_mode: ""));
                    }

                    // 指揮効果
                    if (my_cmd_rank >= 0)
                    {
                        if (range > withBlock2.CommandRange())
                        {
                            goto NextUnit;
                        }

                        cmd_rank2 = (int)withBlock2.SkillLevel("階級", ref_mode: "");
                        if (cmd_rank2 > cmd_rank)
                        {
                            cmd_rank = cmd_rank2;
                            cmd_level = withBlock2.SkillLevel("指揮", ref_mode: "");
                        }
                        else if (cmd_rank2 == cmd_rank)
                        {
                            cmd_level = GeneralLib.MaxDbl(cmd_level, withBlock2.SkillLevel("指揮", ref_mode: ""));
                        }
                    }

                NextUnit:;
                }
            }

            // 追加パイロットの場合は乗っているユニットが変化してしまうことがあるので
            // 変化してしまった場合は元に戻しておく
            if (!ReferenceEquals(my_unit, Unit))
            {
                my_unit.MainPilot();
            }

            // 広域サポートによる修正
            if (cs_level != Constants.DEFAULT_LEVEL)
            {
                HitMod2 = (int)(HitMod2 + 5.0 * cs_level);
                DodgeMod2 = (int)(DodgeMod2 + 5.0 * cs_level);
            }

            // 指揮能力による修正
            switch (my_cmd_rank)
            {
                case Constants.DEFAULT_LEVEL:
                    // 修正なし
                    break;
                case 0:
                    HitMod2 = (int)(HitMod2 + 5.0 * cmd_level);
                    DodgeMod2 = (int)(DodgeMod2 + 5.0 * cmd_level);
                    break;

                default:
                    // 自分が階級レベルを持っている場合はより高い階級レベルを
                    // 持つパイロットの指揮効果のみを受ける
                    if (cmd_rank > my_cmd_rank)
                    {
                        HitMod2 = (int)(HitMod2 + 5.0 * cmd_level);
                        DodgeMod2 = (int)(DodgeMod2 + 5.0 * cmd_level);
                    }
                    break;
            }

            // 支援効果による修正を能力値に加算
            Infight = (Infight + InfightMod2);
            Shooting = (Shooting + ShootingMod2);
            Hit = (Hit + HitMod2);
            Dodge = (Dodge + DodgeMod2);
            Technique = (Technique + TechniqueMod2);
            Intuition = (Intuition + IntuitionMod2);

            // 信頼補正
            if (!Expression.IsOptionDefined("信頼補正"))
            {
                return;
            }

            if (Strings.InStr(Name, "(ザコ)") > 0)
            {
                return;
            }

            // 信頼補正が重複する？
            mod_stack = Expression.IsOptionDefined("信頼補正重複");

            // 同じユニットに乗っているサポートパイロットからの補正
            if (mod_stack)
            {
                foreach (var supportPilot in withBlock.Supports)
                {
                    rel_lv = (rel_lv + Relation(supportPilot));
                }

                if (withBlock.IsFeatureAvailable("追加サポート"))
                {
                    rel_lv = (rel_lv + Relation(withBlock.AdditionalSupport()));
                }
            }
            else
            {
                foreach (var supportPilot in withBlock.Supports)
                {
                    rel_lv = GeneralLib.MaxLng(Relation(supportPilot), rel_lv);
                }

                if (withBlock.IsFeatureAvailable("追加サポート"))
                {
                    rel_lv = GeneralLib.MaxLng(Relation(withBlock.AdditionalSupport()), rel_lv);
                }
            }

            // 周囲のユニットからの補正
            if (Expression.IsOptionDefined("信頼補正範囲拡大"))
            {
                max_range = 2;
            }
            else
            {
                max_range = 1;
            }

            for (i = GeneralLib.MaxLng(withBlock.x - max_range, 1); i <= GeneralLib.MinLng(withBlock.x + max_range, SRC.Map.MapWidth); i++)
            {
                for (j = GeneralLib.MaxLng(withBlock.y - max_range, 1); j <= GeneralLib.MinLng(withBlock.y + max_range, SRC.Map.MapHeight); j++)
                {
                    // ユニット間の距離が範囲内？
                    range = (Math.Abs((withBlock.x - i)) + Math.Abs((withBlock.y - j)));
                    if (range > max_range)
                    {
                        continue;
                    }

                    u = SRC.Map.MapDataForUnit[i, j];
                    if (u is null)
                    {
                        continue;
                    }

                    if (ReferenceEquals(u, Unit))
                    {
                        continue;
                    }
                    
                    // ユニットにパイロットが乗っていなければ無視
                    if (u.CountPilot() == 0)
                    {
                        continue;
                    }

                    // 味方かどうか判定
                    switch (my_party ?? "")
                    {
                        case "味方":
                        case "ＮＰＣ":
                            switch (u.Party ?? "")
                            {
                                case "敵":
                                case "中立":
                                    goto NextUnit2;
                            }
                            break;

                        default:
                            if ((my_party ?? "") != (u.Party ?? ""))
                            {
                                goto NextUnit2;
                            }
                            break;
                    }

                    if (mod_stack)
                    {
                        rel_lv = (rel_lv + Relation(u.MainPilot(true)));
                        foreach (var subPilot in u.SubPilots)
                        {
                            rel_lv = (rel_lv + Relation(subPilot));
                        }

                        foreach (var supportPilot in u.Supports)
                        {
                            rel_lv = (rel_lv + Relation(supportPilot));
                        }

                        if (u.IsFeatureAvailable("追加サポート"))
                        {
                            rel_lv = (rel_lv + Relation(u.AdditionalSupport()));
                        }
                    }
                    else
                    {
                        rel_lv = GeneralLib.MaxLng(Relation(u.MainPilot(true)), rel_lv);
                        foreach (var subPilot in u.SubPilots)
                        {
                            rel_lv = GeneralLib.MaxLng(Relation(subPilot), rel_lv);
                        }

                        foreach (var supportPilot in u.Supports)
                        {
                            rel_lv = GeneralLib.MaxLng(Relation(supportPilot), rel_lv);
                        }

                        if (u.IsFeatureAvailable("追加サポート"))
                        {
                            rel_lv = GeneralLib.MaxLng(Relation(u.AdditionalSupport()), rel_lv);
                        }
                    }

                NextUnit2:;
                }
            }

            // 追加パイロットの場合は乗っているユニットが変化してしまうことがあるので
            // 変化してしまった場合は元に戻しておく
            if (!ReferenceEquals(my_unit, Unit))
            {
                my_unit.MainPilot();
            }

            // 信頼補正を設定
            switch (rel_lv)
            {
                case 1:
                    MoraleMod = (MoraleMod + 5);
                    break;

                case 2:
                    MoraleMod = (MoraleMod + 8);
                    break;

                case var @case when @case > 2:
                    MoraleMod = (MoraleMod + 2 * rel_lv + 4);
                    break;
            }
        }
    }
}
