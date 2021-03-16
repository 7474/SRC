// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Models;
using SRCCore.Units;
using SRCCore.VB;
using System;

namespace SRCCore.Pilots
{
    public partial class Pilot
    {
        // 能力値を更新
        public void Update()
        { }
        // TODO Impl
        //    short skill_num;
        //    var skill_name = new string[65];
        //    var skill_data = new SkillData[65];
        //    short i, j;
        //    double lv;
        //    SkillData sd;

        //    // 現在のレベルで使用可能な特殊能力の一覧を作成

        //    // 以前の一覧を削除
        //    {
        //        var withBlock = colSkill;
        //        var loopTo = (short)withBlock.Count;
        //        for (i = 1; i <= loopTo; i++)
        //            withBlock.Remove(1);
        //    }

        //    // パイロットデータを参照しながら使用可能な特殊能力を検索
        //    skill_num = 0;
        //    foreach (SkillData currentSd in Data.colSkill)
        //    {
        //        sd = currentSd;
        //        {
        //            var withBlock1 = sd;
        //            if (Level >= withBlock1.NecessaryLevel)
        //            {
        //                // 既に登録済み？
        //                if (withBlock1.Name == "ＳＰ消費減少" | withBlock1.Name == "スペシャルパワー自動発動" | withBlock1.Name == "ハンター")
        //                {
        //                    // これらの特殊能力は同種の能力を複数持つことが出来る
        //                    var loopTo1 = skill_num;
        //                    for (i = 1; i <= loopTo1; i++)
        //                    {
        //                        if ((withBlock1.Name ?? "") == (skill_name[i] ?? ""))
        //                        {
        //                            if ((withBlock1.StrData ?? "") == (skill_data[i].StrData ?? ""))
        //                            {
        //                                // ただしデータ指定まで同一であれば同じ能力と見なす
        //                                break;
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    var loopTo2 = skill_num;
        //                    for (i = 1; i <= loopTo2; i++)
        //                    {
        //                        if ((withBlock1.Name ?? "") == (skill_name[i] ?? ""))
        //                        {
        //                            break;
        //                        }
        //                    }
        //                }

        //                if (i > skill_num)
        //                {
        //                    // 未登録
        //                    skill_num = (short)(skill_num + 1);
        //                    skill_name[skill_num] = withBlock1.Name;
        //                    skill_data[skill_num] = sd;
        //                }
        //                else if (withBlock1.NecessaryLevel > skill_data[i].NecessaryLevel)
        //                {
        //                    // 登録済みである場合は習得レベルが高いものを優先
        //                    skill_data[i] = sd;
        //                }
        //            }
        //        }
        //    }

        //    // SetSkillコマンドで付加された特殊能力を検索
        //    string sname, alist, sdata;
        //    string buf;
        //    string argvname = "Ability(" + ID + ")";
        //    if (Expression.IsGlobalVariableDefined(ref argvname))
        //    {

        //        // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //        alist = Conversions.ToString(Event_Renamed.GlobalVariableList["Ability(" + ID + ")"].StringValue);
        //        var loopTo3 = GeneralLib.LLength(ref alist);
        //        for (i = 1; i <= loopTo3; i++)
        //        {
        //            sname = GeneralLib.LIndex(ref alist, i);
        //            // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //            buf = Conversions.ToString(Event_Renamed.GlobalVariableList["Ability(" + ID + "," + sname + ")"].StringValue);
        //            sdata = GeneralLib.ListTail(ref buf, 2);

        //            // 既に登録済み？
        //            if (sname == "ＳＰ消費減少" | sname == "スペシャルパワー自動発動" | sname == "ハンター")
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

        //            string argexpr1 = GeneralLib.LIndex(ref buf, 1);
        //            if (GeneralLib.StrToDbl(ref argexpr1) == 0d)
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
        //                string argexpr = GeneralLib.LIndex(ref buf, 1);
        //                sd.Level = GeneralLib.StrToDbl(ref argexpr);
        //                if (sd.Level == -1)
        //                {
        //                    sd.Level = SRC.DEFAULT_LEVEL;
        //                }

        //                sd.StrData = GeneralLib.ListTail(ref buf, 2);
        //                skill_data[j] = sd;
        //            }
        //        }
        //    }

        //    // 属性使用不能状態の際、対応する技能を封印する。
        //    if (Unit_Renamed is object)
        //    {
        //        var loopTo6 = skill_num;
        //        for (j = 1; j <= loopTo6; j++)
        //        {
        //            if (skill_data[j] is object)
        //            {
        //                object argIndex1 = skill_data[j].Name + "使用不能";
        //                if (Unit_Renamed.ConditionLifetime(ref argIndex1) > 0)
        //                {
        //                    // UPGRADE_NOTE: オブジェクト skill_data() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
        //                    skill_data[j] = null;
        //                }
        //            }
        //        }
        //    }

        //    // 使用可能な特殊能力を登録
        //    {
        //        var withBlock2 = colSkill;
        //        var loopTo7 = skill_num;
        //        for (i = 1; i <= loopTo7; i++)
        //        {
        //            if (skill_data[i] is object)
        //            {
        //                switch (skill_name[i] ?? "")
        //                {
        //                    case "ＳＰ消費減少":
        //                    case "スペシャルパワー自動発動":
        //                    case "ハンター":
        //                        {
        //                            var loopTo8 = (short)(i - 1);
        //                            for (j = 1; j <= loopTo8; j++)
        //                            {
        //                                if ((skill_name[i] ?? "") == (skill_name[j] ?? ""))
        //                                {
        //                                    break;
        //                                }
        //                            }

        //                            if (j >= i)
        //                            {
        //                                withBlock2.Add(skill_data[i], skill_name[i]);
        //                            }
        //                            else
        //                            {
        //                                withBlock2.Add(skill_data[i], skill_name[i] + ":" + SrcFormatter.Format(i));
        //                            }

        //                            break;
        //                        }

        //                    default:
        //                        {
        //                            withBlock2.Add(skill_data[i], skill_name[i]);
        //                            break;
        //                        }
        //                }
        //            }
        //        }
        //    }

        //    // これから下は能力値の計算

        //    // 基本値
        //    {
        //        var withBlock3 = Data;
        //        InfightBase = withBlock3.Infight;
        //        ShootingBase = withBlock3.Shooting;
        //        HitBase = withBlock3.Hit;
        //        DodgeBase = withBlock3.Dodge;
        //        TechniqueBase = withBlock3.Technique;
        //        IntuitionBase = withBlock3.Intuition;
        //        Adaption = withBlock3.Adaption;
        //    }

        //    // レベルによる追加分
        //    object argIndex2 = "追加レベル";
        //    string argref_mode = "";
        //    lv = Level + SkillLevel(ref argIndex2, ref_mode: ref argref_mode);
        //    string argoname = "攻撃力低成長";
        //    if (Expression.IsOptionDefined(ref argoname))
        //    {
        //        object argIndex3 = "格闘成長";
        //        string argref_mode1 = "";
        //        InfightBase = (short)(InfightBase + (long)(lv * (1d + 2d * SkillLevel(ref argIndex3, ref_mode: ref argref_mode1))) / 2L);
        //        object argIndex4 = "射撃成長";
        //        string argref_mode2 = "";
        //        ShootingBase = (short)(ShootingBase + (long)(lv * (1d + 2d * SkillLevel(ref argIndex4, ref_mode: ref argref_mode2))) / 2L);
        //    }
        //    else
        //    {
        //        object argIndex5 = "格闘成長";
        //        string argref_mode3 = "";
        //        InfightBase = (short)(InfightBase + Conversion.Int(lv * (1d + SkillLevel(ref argIndex5, ref_mode: ref argref_mode3))));
        //        object argIndex6 = "射撃成長";
        //        string argref_mode4 = "";
        //        ShootingBase = (short)(ShootingBase + Conversion.Int(lv * (1d + SkillLevel(ref argIndex6, ref_mode: ref argref_mode4))));
        //    }

        //    object argIndex7 = "命中成長";
        //    string argref_mode5 = "";
        //    HitBase = (short)(HitBase + Conversion.Int(lv * (2d + SkillLevel(ref argIndex7, ref_mode: ref argref_mode5))));
        //    object argIndex8 = "回避成長";
        //    string argref_mode6 = "";
        //    DodgeBase = (short)(DodgeBase + Conversion.Int(lv * (2d + SkillLevel(ref argIndex8, ref_mode: ref argref_mode6))));
        //    object argIndex9 = "技量成長";
        //    string argref_mode7 = "";
        //    TechniqueBase = (short)(TechniqueBase + Conversion.Int(lv * (1d + SkillLevel(ref argIndex9, ref_mode: ref argref_mode7))));
        //    object argIndex10 = "反応成長";
        //    string argref_mode8 = "";
        //    IntuitionBase = (short)(IntuitionBase + Conversion.Int(lv * (1d + SkillLevel(ref argIndex10, ref_mode: ref argref_mode8))));

        //    // 能力ＵＰ
        //    object argIndex11 = "格闘ＵＰ";
        //    string argref_mode9 = "";
        //    InfightBase = (short)(InfightBase + SkillLevel(ref argIndex11, ref_mode: ref argref_mode9));
        //    object argIndex12 = "射撃ＵＰ";
        //    string argref_mode10 = "";
        //    ShootingBase = (short)(ShootingBase + SkillLevel(ref argIndex12, ref_mode: ref argref_mode10));
        //    object argIndex13 = "命中ＵＰ";
        //    string argref_mode11 = "";
        //    HitBase = (short)(HitBase + SkillLevel(ref argIndex13, ref_mode: ref argref_mode11));
        //    object argIndex14 = "回避ＵＰ";
        //    string argref_mode12 = "";
        //    DodgeBase = (short)(DodgeBase + SkillLevel(ref argIndex14, ref_mode: ref argref_mode12));
        //    object argIndex15 = "技量ＵＰ";
        //    string argref_mode13 = "";
        //    TechniqueBase = (short)(TechniqueBase + SkillLevel(ref argIndex15, ref_mode: ref argref_mode13));
        //    object argIndex16 = "反応ＵＰ";
        //    string argref_mode14 = "";
        //    IntuitionBase = (short)(IntuitionBase + SkillLevel(ref argIndex16, ref_mode: ref argref_mode14));

        //    // 能力ＤＯＷＮ
        //    object argIndex17 = "格闘ＤＯＷＮ";
        //    string argref_mode15 = "";
        //    InfightBase = (short)(InfightBase - SkillLevel(ref argIndex17, ref_mode: ref argref_mode15));
        //    object argIndex18 = "射撃ＤＯＷＮ";
        //    string argref_mode16 = "";
        //    ShootingBase = (short)(ShootingBase - SkillLevel(ref argIndex18, ref_mode: ref argref_mode16));
        //    object argIndex19 = "命中ＤＯＷＮ";
        //    string argref_mode17 = "";
        //    HitBase = (short)(HitBase - SkillLevel(ref argIndex19, ref_mode: ref argref_mode17));
        //    object argIndex20 = "回避ＤＯＷＮ";
        //    string argref_mode18 = "";
        //    DodgeBase = (short)(DodgeBase - SkillLevel(ref argIndex20, ref_mode: ref argref_mode18));
        //    object argIndex21 = "技量ＤＯＷＮ";
        //    string argref_mode19 = "";
        //    TechniqueBase = (short)(TechniqueBase - SkillLevel(ref argIndex21, ref_mode: ref argref_mode19));
        //    object argIndex22 = "反応ＤＯＷＮ";
        //    string argref_mode20 = "";
        //    IntuitionBase = (short)(IntuitionBase - SkillLevel(ref argIndex22, ref_mode: ref argref_mode20));

        //    // 上限を超えないように
        //    InfightBase = (short)GeneralLib.MinLng(InfightBase, 9999);
        //    ShootingBase = (short)GeneralLib.MinLng(ShootingBase, 9999);
        //    HitBase = (short)GeneralLib.MinLng(HitBase, 9999);
        //    DodgeBase = (short)GeneralLib.MinLng(DodgeBase, 9999);
        //    TechniqueBase = (short)GeneralLib.MinLng(TechniqueBase, 9999);
        //    IntuitionBase = (short)GeneralLib.MinLng(IntuitionBase, 9999);

        //    // これから下は特殊能力による修正値の計算

        //    // まずは修正値を初期化
        //    InfightMod = 0;
        //    ShootingMod = 0;
        //    HitMod = 0;
        //    DodgeMod = 0;
        //    TechniqueMod = 0;
        //    IntuitionMod = 0;

        //    // パイロット用特殊能力による修正

        //    object argIndex23 = "超感覚";
        //    string argref_mode21 = "";
        //    lv = SkillLevel(ref argIndex23, ref_mode: ref argref_mode21);
        //    if (lv > 0d)
        //    {
        //        HitMod = (short)(HitMod + 2d * lv + 3d);
        //        DodgeMod = (short)(DodgeMod + 2d * lv + 3d);
        //    }

        //    object argIndex24 = "知覚強化";
        //    string argref_mode22 = "";
        //    lv = SkillLevel(ref argIndex24, ref_mode: ref argref_mode22);
        //    if (lv > 0d)
        //    {
        //        HitMod = (short)(HitMod + 2d * lv + 3d);
        //        DodgeMod = (short)(DodgeMod + 2d * lv + 3d);
        //    }

        //    object argIndex25 = "念力";
        //    string argref_mode23 = "";
        //    lv = SkillLevel(ref argIndex25, ref_mode: ref argref_mode23);
        //    if (lv > 0d)
        //    {
        //        HitMod = (short)(HitMod + 2d * lv + 3d);
        //        DodgeMod = (short)(DodgeMod + 2d * lv + 3d);
        //    }

        //    object argIndex26 = "超反応";
        //    string argref_mode24 = "";
        //    lv = SkillLevel(ref argIndex26, ref_mode: ref argref_mode24);
        //    HitMod = (short)(HitMod + 2d * lv);
        //    DodgeMod = (short)(DodgeMod + 2d * lv);
        //    string argsname = "サイボーグ";
        //    if (IsSkillAvailable(ref argsname))
        //    {
        //        HitMod = (short)(HitMod + 5);
        //        DodgeMod = (short)(DodgeMod + 5);
        //    }

        //    string argsname1 = "悟り";
        //    if (IsSkillAvailable(ref argsname1))
        //    {
        //        HitMod = (short)(HitMod + 10);
        //        DodgeMod = (short)(DodgeMod + 10);
        //    }

        //    string argsname2 = "超能力";
        //    if (IsSkillAvailable(ref argsname2))
        //    {
        //        HitMod = (short)(HitMod + 5);
        //        DodgeMod = (short)(DodgeMod + 5);
        //    }

        //    // これから下はユニットによる修正値の計算

        //    // ユニットに乗っていない？
        //    if (Unit_Renamed is null)
        //    {
        //        goto SkipUnitMod;
        //    }

        //    var padaption = new short[5];
        //    {
        //        var withBlock4 = Unit_Renamed;
        //        // クイックセーブ処理などで実際には乗っていない場合
        //        if (withBlock4.CountPilot() == 0)
        //        {
        //            return;
        //        }

        //        // サブパイロット＆サポートパイロットによるサポート
        //        if (ReferenceEquals(this, withBlock4.MainPilot()) & withBlock4.Status == "出撃")
        //        {
        //            var loopTo9 = withBlock4.CountPilot();
        //            for (i = 2; i <= loopTo9; i++)
        //            {
        //                object argIndex36 = i;
        //                {
        //                    var withBlock5 = withBlock4.Pilot(ref argIndex36);
        //                    object argIndex27 = "格闘サポート";
        //                    string argref_mode25 = "";
        //                    InfightMod = (short)(InfightMod + 2d * withBlock5.SkillLevel(ref argIndex27, ref_mode: ref argref_mode25));
        //                    if (HasMana())
        //                    {
        //                        object argIndex28 = "魔力サポート";
        //                        string argref_mode26 = "";
        //                        ShootingMod = (short)(ShootingMod + 2d * withBlock5.SkillLevel(ref argIndex28, ref_mode: ref argref_mode26));
        //                    }
        //                    else
        //                    {
        //                        object argIndex29 = "射撃サポート";
        //                        string argref_mode27 = "";
        //                        ShootingMod = (short)(ShootingMod + 2d * withBlock5.SkillLevel(ref argIndex29, ref_mode: ref argref_mode27));
        //                    }

        //                    object argIndex30 = "サポート";
        //                    string argref_mode28 = "";
        //                    HitMod = (short)(HitMod + 3d * withBlock5.SkillLevel(ref argIndex30, ref_mode: ref argref_mode28));
        //                    object argIndex31 = "命中サポート";
        //                    string argref_mode29 = "";
        //                    HitMod = (short)(HitMod + 2d * withBlock5.SkillLevel(ref argIndex31, ref_mode: ref argref_mode29));
        //                    object argIndex32 = "サポート";
        //                    string argref_mode30 = "";
        //                    DodgeMod = (short)(DodgeMod + 3d * withBlock5.SkillLevel(ref argIndex32, ref_mode: ref argref_mode30));
        //                    object argIndex33 = "回避サポート";
        //                    string argref_mode31 = "";
        //                    DodgeMod = (short)(DodgeMod + 2d * withBlock5.SkillLevel(ref argIndex33, ref_mode: ref argref_mode31));
        //                    object argIndex34 = "技量サポート";
        //                    string argref_mode32 = "";
        //                    TechniqueMod = (short)(TechniqueMod + 2d * withBlock5.SkillLevel(ref argIndex34, ref_mode: ref argref_mode32));
        //                    object argIndex35 = "反応サポート";
        //                    string argref_mode33 = "";
        //                    IntuitionMod = (short)(IntuitionMod + 2d * withBlock5.SkillLevel(ref argIndex35, ref_mode: ref argref_mode33));
        //                }
        //            }

        //            var loopTo10 = withBlock4.CountSupport();
        //            for (i = 1; i <= loopTo10; i++)
        //            {
        //                object argIndex46 = i;
        //                {
        //                    var withBlock6 = withBlock4.Support(ref argIndex46);
        //                    object argIndex37 = "格闘サポート";
        //                    string argref_mode34 = "";
        //                    InfightMod = (short)(InfightMod + 2d * withBlock6.SkillLevel(ref argIndex37, ref_mode: ref argref_mode34));
        //                    if (HasMana())
        //                    {
        //                        object argIndex38 = "魔力サポート";
        //                        string argref_mode35 = "";
        //                        ShootingMod = (short)(ShootingMod + 2d * withBlock6.SkillLevel(ref argIndex38, ref_mode: ref argref_mode35));
        //                    }
        //                    else
        //                    {
        //                        object argIndex39 = "射撃サポート";
        //                        string argref_mode36 = "";
        //                        ShootingMod = (short)(ShootingMod + 2d * withBlock6.SkillLevel(ref argIndex39, ref_mode: ref argref_mode36));
        //                    }

        //                    object argIndex40 = "サポート";
        //                    string argref_mode37 = "";
        //                    HitMod = (short)(HitMod + 3d * withBlock6.SkillLevel(ref argIndex40, ref_mode: ref argref_mode37));
        //                    object argIndex41 = "命中サポート";
        //                    string argref_mode38 = "";
        //                    HitMod = (short)(HitMod + 2d * withBlock6.SkillLevel(ref argIndex41, ref_mode: ref argref_mode38));
        //                    object argIndex42 = "サポート";
        //                    string argref_mode39 = "";
        //                    DodgeMod = (short)(DodgeMod + 3d * withBlock6.SkillLevel(ref argIndex42, ref_mode: ref argref_mode39));
        //                    object argIndex43 = "回避サポート";
        //                    string argref_mode40 = "";
        //                    DodgeMod = (short)(DodgeMod + 2d * withBlock6.SkillLevel(ref argIndex43, ref_mode: ref argref_mode40));
        //                    object argIndex44 = "技量サポート";
        //                    string argref_mode41 = "";
        //                    TechniqueMod = (short)(TechniqueMod + 2d * withBlock6.SkillLevel(ref argIndex44, ref_mode: ref argref_mode41));
        //                    object argIndex45 = "反応サポート";
        //                    string argref_mode42 = "";
        //                    IntuitionMod = (short)(IntuitionMod + 2d * withBlock6.SkillLevel(ref argIndex45, ref_mode: ref argref_mode42));
        //                }
        //            }

        //            string argfname = "追加サポート";
        //            if (withBlock4.IsFeatureAvailable(ref argfname))
        //            {
        //                {
        //                    var withBlock7 = withBlock4.AdditionalSupport();
        //                    object argIndex47 = "格闘サポート";
        //                    string argref_mode43 = "";
        //                    InfightMod = (short)(InfightMod + 2d * withBlock7.SkillLevel(ref argIndex47, ref_mode: ref argref_mode43));
        //                    if (HasMana())
        //                    {
        //                        object argIndex48 = "魔力サポート";
        //                        string argref_mode44 = "";
        //                        ShootingMod = (short)(ShootingMod + 2d * withBlock7.SkillLevel(ref argIndex48, ref_mode: ref argref_mode44));
        //                    }
        //                    else
        //                    {
        //                        object argIndex49 = "射撃サポート";
        //                        string argref_mode45 = "";
        //                        ShootingMod = (short)(ShootingMod + 2d * withBlock7.SkillLevel(ref argIndex49, ref_mode: ref argref_mode45));
        //                    }

        //                    object argIndex50 = "サポート";
        //                    string argref_mode46 = "";
        //                    HitMod = (short)(HitMod + 3d * withBlock7.SkillLevel(ref argIndex50, ref_mode: ref argref_mode46));
        //                    object argIndex51 = "命中サポート";
        //                    string argref_mode47 = "";
        //                    HitMod = (short)(HitMod + 2d * withBlock7.SkillLevel(ref argIndex51, ref_mode: ref argref_mode47));
        //                    object argIndex52 = "サポート";
        //                    string argref_mode48 = "";
        //                    DodgeMod = (short)(DodgeMod + 3d * withBlock7.SkillLevel(ref argIndex52, ref_mode: ref argref_mode48));
        //                    object argIndex53 = "回避サポート";
        //                    string argref_mode49 = "";
        //                    DodgeMod = (short)(DodgeMod + 2d * withBlock7.SkillLevel(ref argIndex53, ref_mode: ref argref_mode49));
        //                    object argIndex54 = "技量サポート";
        //                    string argref_mode50 = "";
        //                    TechniqueMod = (short)(TechniqueMod + 2d * withBlock7.SkillLevel(ref argIndex54, ref_mode: ref argref_mode50));
        //                    object argIndex55 = "反応サポート";
        //                    string argref_mode51 = "";
        //                    IntuitionMod = (short)(IntuitionMod + 2d * withBlock7.SkillLevel(ref argIndex55, ref_mode: ref argref_mode51));
        //                }
        //            }
        //        }

        //        // ユニット＆アイテムによる強化
        //        var loopTo11 = withBlock4.CountFeature();
        //        for (i = 1; i <= loopTo11; i++)
        //        {
        //            object argIndex56 = i;
        //            switch (withBlock4.Feature(ref argIndex56) ?? "")
        //            {
        //                case "格闘強化":
        //                    {
        //                        string localFeatureData() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

        //                        string localLIndex() { string arglist = hs2cde20588fb24d85bea5cf26fad46fbc(); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

        //                        int localStrToLng() { string argexpr = hsc0c629727e364218a343e72f775d5378(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

        //                        if (Morale >= localStrToLng())
        //                        {
        //                            double localFeatureLevel() { object argIndex1 = i; var ret = withBlock4.FeatureLevel(ref argIndex1); return ret; }

        //                            InfightMod = (short)(InfightMod + 5d * localFeatureLevel());
        //                        }

        //                        break;
        //                    }

        //                case "射撃強化":
        //                    {
        //                        string localFeatureData1() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

        //                        string localLIndex1() { string arglist = hs8f37be02bf88436e950e311a12d5b37e(); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

        //                        int localStrToLng1() { string argexpr = hs75f1e2fd554a46828757d5c3791b5757(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

        //                        if (Morale >= localStrToLng1())
        //                        {
        //                            double localFeatureLevel1() { object argIndex1 = i; var ret = withBlock4.FeatureLevel(ref argIndex1); return ret; }

        //                            ShootingMod = (short)(ShootingMod + 5d * localFeatureLevel1());
        //                        }

        //                        break;
        //                    }

        //                case "命中強化":
        //                    {
        //                        string localFeatureData2() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

        //                        string localLIndex2() { string arglist = hs11e93ac1f3284a4f93380ee6473c818c(); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

        //                        int localStrToLng2() { string argexpr = hsa2ad2f0f65c84d6f8292a2b9a1fd3fd3(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

        //                        if (Morale >= localStrToLng2())
        //                        {
        //                            double localFeatureLevel2() { object argIndex1 = i; var ret = withBlock4.FeatureLevel(ref argIndex1); return ret; }

        //                            HitMod = (short)(HitMod + 5d * localFeatureLevel2());
        //                        }

        //                        break;
        //                    }

        //                case "回避強化":
        //                    {
        //                        string localFeatureData3() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

        //                        string localLIndex3() { string arglist = hsacf802b5acb245cfbca1d3ca09fa7324(); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

        //                        int localStrToLng3() { string argexpr = hs9f2075d9bdc24b7f9cf3f7845c40a78a(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

        //                        if (Morale >= localStrToLng3())
        //                        {
        //                            double localFeatureLevel3() { object argIndex1 = i; var ret = withBlock4.FeatureLevel(ref argIndex1); return ret; }

        //                            DodgeMod = (short)(DodgeMod + 5d * localFeatureLevel3());
        //                        }

        //                        break;
        //                    }

        //                case "技量強化":
        //                    {
        //                        string localFeatureData4() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

        //                        string localLIndex4() { string arglist = hs3db15ab6c56047db9b6236f45e649ebd(); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

        //                        int localStrToLng4() { string argexpr = hsd8d478d12f3941fca4b635b001945700(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

        //                        if (Morale >= localStrToLng4())
        //                        {
        //                            double localFeatureLevel4() { object argIndex1 = i; var ret = withBlock4.FeatureLevel(ref argIndex1); return ret; }

        //                            TechniqueMod = (short)(TechniqueMod + 5d * localFeatureLevel4());
        //                        }

        //                        break;
        //                    }

        //                case "反応強化":
        //                    {
        //                        string localFeatureData5() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

        //                        string localLIndex5() { string arglist = hs0419ff33196e4e48af748fdce12580b1(); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

        //                        int localStrToLng5() { string argexpr = hsabf5cf29dc0446c781dea0167fb94b44(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

        //                        if (Morale >= localStrToLng5())
        //                        {
        //                            double localFeatureLevel5() { object argIndex1 = i; var ret = withBlock4.FeatureLevel(ref argIndex1); return ret; }

        //                            IntuitionMod = (short)(IntuitionMod + 5d * localFeatureLevel5());
        //                        }

        //                        break;
        //                    }
        //            }
        //        }

        //        // 地形適応変更
        //        string argfname1 = "パイロット地形適応変更";
        //        if (withBlock4.IsFeatureAvailable(ref argfname1))
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
        //                object argIndex57 = i;
        //                if (withBlock4.Feature(ref argIndex57) == "パイロット地形適応変更")
        //                {
        //                    for (j = 1; j <= 4; j++)
        //                    {
        //                        string localFeatureData8() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

        //                        string localLIndex8() { string arglist = hsb8c32b5b97d840f1a1974442798ae710(); var ret = GeneralLib.LIndex(ref arglist, j); return ret; }

        //                        string argexpr2 = localLIndex8();
        //                        if (GeneralLib.StrToLng(ref argexpr2) >= 0)
        //                        {
        //                            // 修正値がプラスのとき
        //                            if (padaption[j] < 4)
        //                            {
        //                                string localFeatureData6() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

        //                                string localLIndex6() { string arglist = hs8f1b1c4829bf4c8ea402eb72f2930e63(); var ret = GeneralLib.LIndex(ref arglist, j); return ret; }

        //                                int localStrToLng6() { string argexpr = hsdf089849003448bebc02763c21e401ad(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

        //                                padaption[j] = (short)(padaption[j] + localStrToLng6());
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
        //                            string localFeatureData7() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

        //                            string localLIndex7() { string arglist = hs8e6f6c053bcd4824a879080031dfde79(); var ret = GeneralLib.LIndex(ref arglist, j); return ret; }

        //                            int localStrToLng7() { string argexpr = hs210e4a3c7ff64bf2865702c94fd878d3(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

        //                            padaption[j] = (short)(padaption[j] + localStrToLng7());
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


        //    // 基本値と修正値の合計から実際の能力値を算出
        //    Infight = (short)GeneralLib.MinLng((short)(InfightBase + InfightMod) + InfightMod2, 9999);
        //    Shooting = (short)GeneralLib.MinLng((short)(ShootingBase + ShootingMod) + ShootingMod2, 9999);
        //    Hit = (short)GeneralLib.MinLng((short)(HitBase + HitMod) + HitMod2, 9999);
        //    Dodge = (short)GeneralLib.MinLng((short)(DodgeBase + DodgeMod) + DodgeMod2, 9999);
        //    Technique = (short)GeneralLib.MinLng((short)(TechniqueBase + TechniqueMod) + TechniqueMod2, 9999);
        //    Intuition = (short)GeneralLib.MinLng((short)(IntuitionBase + IntuitionMod) + IntuitionMod2, 9999);
        //}

        // 周りのユニットによる支援効果を更新
        public void UpdateSupportMod()
        {
        }
        // TODO Impl
        //    Unit u, my_unit;
        //    string my_party;
        //    short my_cmd_rank;
        //    short cmd_rank, cmd_rank2;
        //    var cmd_level = default(double);
        //    double cs_level;
        //    short range, max_range;
        //    bool mod_stack;
        //    var rel_lv = default(short);
        //    string team, uteam;
        //    short j, i, k;

        //    // 支援効果による修正値を初期化

        //    Infight = (short)(InfightBase + InfightMod);
        //    Shooting = (short)(ShootingBase + ShootingMod);
        //    Hit = (short)(HitBase + HitMod);
        //    Dodge = (short)(DodgeBase + DodgeMod);
        //    Technique = (short)(TechniqueBase + TechniqueMod);
        //    Intuition = (short)(IntuitionBase + IntuitionMod);
        //    InfightMod2 = 0;
        //    ShootingMod2 = 0;
        //    HitMod2 = 0;
        //    DodgeMod2 = 0;
        //    TechniqueMod2 = 0;
        //    IntuitionMod2 = 0;
        //    MoraleMod = 0;

        //    // ステータス表示時には支援効果を無視
        //    if (string.IsNullOrEmpty(Map.MapFileName))
        //    {
        //        return;
        //    }

        //    // ユニットに乗っていなければここで終了
        //    if (Unit_Renamed is null)
        //    {
        //        return;
        //    }

        //    // 一旦乗っているユニットを記録しておく
        //    my_unit = Unit_Renamed;
        //    {
        //        var withBlock = Unit_Renamed;
        //        // ユニットが出撃していなければここで終了
        //        if (withBlock.Status != "出撃")
        //        {
        //            return;
        //        }

        //        if (!ReferenceEquals(Unit_Renamed, Map.MapDataForUnit[withBlock.x, withBlock.y]))
        //        {
        //            return;
        //        }

        //        // メインパイロットでなければここで終了
        //        if (withBlock.CountPilot() == 0)
        //        {
        //            return;
        //        }

        //        if (!ReferenceEquals(this, withBlock.MainPilot()))
        //        {
        //            return;
        //        }

        //        // 正常な判断が出来ないユニットは支援を受けられない
        //        object argIndex1 = "暴走";
        //        if (withBlock.IsConditionSatisfied(ref argIndex1))
        //        {
        //            return;
        //        }

        //        object argIndex2 = "混乱";
        //        if (withBlock.IsConditionSatisfied(ref argIndex2))
        //        {
        //            return;
        //        }

        //        // 支援を受けられるかどうかの判定用に陣営を参照しておく
        //        my_party = withBlock.Party;

        //        // 指揮効果判定用に自分の階級レベルを算出
        //        string argsname = "階級";
        //        if (IsSkillAvailable(ref argsname))
        //        {
        //            object argIndex3 = "階級";
        //            string argref_mode = "";
        //            my_cmd_rank = (short)SkillLevel(ref argIndex3, ref_mode: ref argref_mode);
        //            cmd_rank = my_cmd_rank;
        //        }
        //        else
        //        {
        //            if (Strings.InStr(Name, "(ザコ)") == 0 & Strings.InStr(Name, "(汎用)") == 0)
        //            {
        //                my_cmd_rank = SRC.DEFAULT_LEVEL;
        //            }
        //            else
        //            {
        //                my_cmd_rank = 0;
        //            }

        //            cmd_rank = 0;
        //        }

        //        // 自分が所属しているチーム名
        //        object argIndex4 = "チーム";
        //        team = SkillData(ref argIndex4);

        //        // 周りのユニットを調べる
        //        cs_level = SRC.DEFAULT_LEVEL;
        //        max_range = 5;
        //        var loopTo = (short)GeneralLib.MinLng(withBlock.x + max_range, Map.MapWidth);
        //        for (i = (short)GeneralLib.MaxLng(withBlock.x - max_range, 1); i <= loopTo; i++)
        //        {
        //            var loopTo1 = (short)GeneralLib.MinLng(withBlock.y + max_range, Map.MapHeight);
        //            for (j = (short)GeneralLib.MaxLng(withBlock.y - max_range, 1); j <= loopTo1; j++)
        //            {
        //                // ユニット間の距離が範囲内？
        //                range = (short)(Math.Abs((short)(withBlock.x - i)) + Math.Abs((short)(withBlock.y - j)));
        //                if (range > max_range)
        //                {
        //                    goto NextUnit;
        //                }

        //                u = Map.MapDataForUnit[i, j];
        //                if (u is null)
        //                {
        //                    goto NextUnit;
        //                }

        //                if (ReferenceEquals(u, Unit_Renamed))
        //                {
        //                    goto NextUnit;
        //                }

        //                {
        //                    var withBlock1 = u;
        //                    // ユニットにパイロットが乗っていなければ無視
        //                    if (withBlock1.CountPilot() == 0)
        //                    {
        //                        goto NextUnit;
        //                    }

        //                    // 陣営が一致していないと支援は受けられない
        //                    switch (my_party ?? "")
        //                    {
        //                        case "味方":
        //                        case "ＮＰＣ":
        //                            {
        //                                switch (withBlock1.Party ?? "")
        //                                {
        //                                    case "敵":
        //                                    case "中立":
        //                                        {
        //                                            goto NextUnit;
        //                                            break;
        //                                        }
        //                                }

        //                                break;
        //                            }

        //                        default:
        //                            {
        //                                if ((my_party ?? "") != (withBlock1.Party ?? ""))
        //                                {
        //                                    goto NextUnit;
        //                                }

        //                                break;
        //                            }
        //                    }

        //                    // 相手が正常な判断能力を持っていない場合も支援は受けられない
        //                    object argIndex5 = "暴走";
        //                    if (withBlock1.IsConditionSatisfied(ref argIndex5))
        //                    {
        //                        goto NextUnit;
        //                    }

        //                    object argIndex6 = "混乱";
        //                    if (withBlock1.IsConditionSatisfied(ref argIndex6))
        //                    {
        //                        goto NextUnit;
        //                    }
        //                }

        //                {
        //                    var withBlock2 = u.MainPilot(true);
        //                    // 同じチームに所属している？
        //                    object argIndex7 = "チーム";
        //                    uteam = withBlock2.SkillData(ref argIndex7);
        //                    if ((team ?? "") != (uteam ?? "") & !string.IsNullOrEmpty(uteam))
        //                    {
        //                        goto NextUnit;
        //                    }

        //                    // 広域サポート
        //                    if (range <= 2)
        //                    {
        //                        object argIndex8 = "広域サポート";
        //                        string argref_mode1 = "";
        //                        cs_level = GeneralLib.MaxDbl(cs_level, withBlock2.SkillLevel(ref argIndex8, ref_mode: ref argref_mode1));
        //                    }

        //                    // 指揮効果
        //                    if (my_cmd_rank >= 0)
        //                    {
        //                        if (range > withBlock2.CommandRange())
        //                        {
        //                            goto NextUnit;
        //                        }

        //                        object argIndex9 = "階級";
        //                        string argref_mode2 = "";
        //                        cmd_rank2 = (short)withBlock2.SkillLevel(ref argIndex9, ref_mode: ref argref_mode2);
        //                        if (cmd_rank2 > cmd_rank)
        //                        {
        //                            cmd_rank = cmd_rank2;
        //                            object argIndex10 = "指揮";
        //                            string argref_mode3 = "";
        //                            cmd_level = withBlock2.SkillLevel(ref argIndex10, ref_mode: ref argref_mode3);
        //                        }
        //                        else if (cmd_rank2 == cmd_rank)
        //                        {
        //                            object argIndex11 = "指揮";
        //                            string argref_mode4 = "";
        //                            cmd_level = GeneralLib.MaxDbl(cmd_level, withBlock2.SkillLevel(ref argIndex11, ref_mode: ref argref_mode4));
        //                        }
        //                    }
        //                }

        //            NextUnit:
        //                ;
        //            }
        //        }

        //        // 追加パイロットの場合は乗っているユニットが変化してしまうことがあるので
        //        // 変化してしまった場合は元に戻しておく
        //        if (!ReferenceEquals(my_unit, Unit_Renamed))
        //        {
        //            my_unit.MainPilot();
        //        }

        //        // 広域サポートによる修正
        //        if (cs_level != SRC.DEFAULT_LEVEL)
        //        {
        //            HitMod2 = (short)(HitMod2 + 5d * cs_level);
        //            DodgeMod2 = (short)(DodgeMod2 + 5d * cs_level);
        //        }

        //        // 指揮能力による修正
        //        switch (my_cmd_rank)
        //        {
        //            case SRC.DEFAULT_LEVEL:
        //                {
        //                    break;
        //                }
        //            // 修正なし
        //            case 0:
        //                {
        //                    HitMod2 = (short)(HitMod2 + 5d * cmd_level);
        //                    DodgeMod2 = (short)(DodgeMod2 + 5d * cmd_level);
        //                    break;
        //                }

        //            default:
        //                {
        //                    // 自分が階級レベルを持っている場合はより高い階級レベルを
        //                    // 持つパイロットの指揮効果のみを受ける
        //                    if (cmd_rank > my_cmd_rank)
        //                    {
        //                        HitMod2 = (short)(HitMod2 + 5d * cmd_level);
        //                        DodgeMod2 = (short)(DodgeMod2 + 5d * cmd_level);
        //                    }

        //                    break;
        //                }
        //        }

        //        // 支援効果による修正を能力値に加算
        //        Infight = (short)(Infight + InfightMod2);
        //        Shooting = (short)(Shooting + ShootingMod2);
        //        Hit = (short)(Hit + HitMod2);
        //        Dodge = (short)(Dodge + DodgeMod2);
        //        Technique = (short)(Technique + TechniqueMod2);
        //        Intuition = (short)(Intuition + IntuitionMod2);

        //        // 信頼補正
        //        string argoname = "信頼補正";
        //        if (!Expression.IsOptionDefined(ref argoname))
        //        {
        //            return;
        //        }

        //        if (Strings.InStr(Name, "(ザコ)") > 0)
        //        {
        //            return;
        //        }

        //        // 信頼補正が重複する？
        //        string argoname1 = "信頼補正重複";
        //        mod_stack = Expression.IsOptionDefined(ref argoname1);

        //        // 同じユニットに乗っているサポートパイロットからの補正
        //        if (mod_stack)
        //        {
        //            var loopTo2 = withBlock.CountSupport();
        //            for (i = 1; i <= loopTo2; i++)
        //            {
        //                Pilot localSupport() { object argIndex1 = i; var ret = withBlock.Support(ref argIndex1); return ret; }

        //                short localRelation() { var argt = hs3f1fac77d5e34a6ea4e03c266c7e9333(); var ret = this.Relation(ref argt); return ret; }

        //                rel_lv = (short)(rel_lv + localRelation());
        //            }

        //            string argfname = "追加サポート";
        //            if (withBlock.IsFeatureAvailable(ref argfname))
        //            {
        //                rel_lv = (short)(rel_lv + Relation(ref withBlock.AdditionalSupport()));
        //            }
        //        }
        //        else
        //        {
        //            var loopTo3 = withBlock.CountSupport();
        //            for (i = 1; i <= loopTo3; i++)
        //            {
        //                Pilot localSupport1() { object argIndex1 = i; var ret = withBlock.Support(ref argIndex1); return ret; }

        //                short localRelation1() { var argt = hsb76f4c2df155488baa39bba755dc1adf(); var ret = this.Relation(ref argt); return ret; }

        //                rel_lv = (short)GeneralLib.MaxLng(localRelation1(), rel_lv);
        //            }

        //            string argfname1 = "追加サポート";
        //            if (withBlock.IsFeatureAvailable(ref argfname1))
        //            {
        //                rel_lv = (short)GeneralLib.MaxLng(Relation(ref withBlock.AdditionalSupport()), rel_lv);
        //            }
        //        }

        //        // 周囲のユニットからの補正
        //        string argoname2 = "信頼補正範囲拡大";
        //        if (Expression.IsOptionDefined(ref argoname2))
        //        {
        //            max_range = 2;
        //        }
        //        else
        //        {
        //            max_range = 1;
        //        }

        //        var loopTo4 = (short)GeneralLib.MinLng(withBlock.x + max_range, Map.MapWidth);
        //        for (i = (short)GeneralLib.MaxLng(withBlock.x - max_range, 1); i <= loopTo4; i++)
        //        {
        //            var loopTo5 = (short)GeneralLib.MinLng(withBlock.y + max_range, Map.MapHeight);
        //            for (j = (short)GeneralLib.MaxLng(withBlock.y - max_range, 1); j <= loopTo5; j++)
        //            {
        //                // ユニット間の距離が範囲内？
        //                range = (short)(Math.Abs((short)(withBlock.x - i)) + Math.Abs((short)(withBlock.y - j)));
        //                if (range > max_range)
        //                {
        //                    goto NextUnit2;
        //                }

        //                u = Map.MapDataForUnit[i, j];
        //                if (u is null)
        //                {
        //                    goto NextUnit2;
        //                }

        //                if (ReferenceEquals(u, Unit_Renamed))
        //                {
        //                    goto NextUnit2;
        //                }
        //                // ユニットにパイロットが乗っていなければ無視
        //                if (u.CountPilot() == 0)
        //                {
        //                    goto NextUnit2;
        //                }

        //                // 味方かどうか判定
        //                switch (my_party ?? "")
        //                {
        //                    case "味方":
        //                    case "ＮＰＣ":
        //                        {
        //                            switch (u.Party ?? "")
        //                            {
        //                                case "敵":
        //                                case "中立":
        //                                    {
        //                                        goto NextUnit2;
        //                                        break;
        //                                    }
        //                            }

        //                            break;
        //                        }

        //                    default:
        //                        {
        //                            if ((my_party ?? "") != (u.Party ?? ""))
        //                            {
        //                                goto NextUnit2;
        //                            }

        //                            break;
        //                        }
        //                }

        //                if (mod_stack)
        //                {
        //                    short localRelation2() { var argt = u.MainPilot(true); var ret = Relation(ref argt); return ret; }

        //                    rel_lv = (short)(rel_lv + localRelation2());
        //                    var loopTo6 = u.CountPilot();
        //                    for (k = 2; k <= loopTo6; k++)
        //                    {
        //                        Pilot localPilot() { object argIndex1 = k; var ret = u.Pilot(ref argIndex1); return ret; }

        //                        short localRelation3() { var argt = hs36c94c32aee741e3bafc14d32f76052f(); var ret = this.Relation(ref argt); return ret; }

        //                        rel_lv = (short)(rel_lv + localRelation3());
        //                    }

        //                    var loopTo7 = u.CountSupport();
        //                    for (k = 1; k <= loopTo7; k++)
        //                    {
        //                        Pilot localSupport2() { object argIndex1 = k; var ret = u.Support(ref argIndex1); return ret; }

        //                        short localRelation4() { var argt = hsa741d7124c89423e8fb9729b82c97548(); var ret = this.Relation(ref argt); return ret; }

        //                        rel_lv = (short)(rel_lv + localRelation4());
        //                    }

        //                    string argfname2 = "追加サポート";
        //                    if (u.IsFeatureAvailable(ref argfname2))
        //                    {
        //                        rel_lv = (short)(rel_lv + Relation(ref u.AdditionalSupport()));
        //                    }
        //                }
        //                else
        //                {
        //                    short localRelation5() { var argt = u.MainPilot(true); var ret = Relation(ref argt); return ret; }

        //                    rel_lv = (short)GeneralLib.MaxLng(localRelation5(), rel_lv);
        //                    var loopTo8 = u.CountPilot();
        //                    for (k = 2; k <= loopTo8; k++)
        //                    {
        //                        Pilot localPilot1() { object argIndex1 = k; var ret = u.Pilot(ref argIndex1); return ret; }

        //                        short localRelation6() { var argt = hse54d99b562974d6badbd51c8be741cb0(); var ret = this.Relation(ref argt); return ret; }

        //                        rel_lv = (short)GeneralLib.MaxLng(localRelation6(), rel_lv);
        //                    }

        //                    var loopTo9 = u.CountSupport();
        //                    for (k = 1; k <= loopTo9; k++)
        //                    {
        //                        Pilot localSupport3() { object argIndex1 = k; var ret = u.Support(ref argIndex1); return ret; }

        //                        short localRelation7() { var argt = hse623540d85d84776ba4c0ea7daeb9431(); var ret = this.Relation(ref argt); return ret; }

        //                        rel_lv = (short)GeneralLib.MaxLng(localRelation7(), rel_lv);
        //                    }

        //                    string argfname3 = "追加サポート";
        //                    if (u.IsFeatureAvailable(ref argfname3))
        //                    {
        //                        rel_lv = (short)GeneralLib.MaxLng(Relation(ref u.AdditionalSupport()), rel_lv);
        //                    }
        //                }

        //            NextUnit2:
        //                ;
        //            }
        //        }

        //        // 追加パイロットの場合は乗っているユニットが変化してしまうことがあるので
        //        // 変化してしまった場合は元に戻しておく
        //        if (!ReferenceEquals(my_unit, Unit_Renamed))
        //        {
        //            my_unit.MainPilot();
        //        }

        //        // 信頼補正を設定
        //        switch (rel_lv)
        //        {
        //            case 1:
        //                {
        //                    MoraleMod = (short)(MoraleMod + 5);
        //                    break;
        //                }

        //            case 2:
        //                {
        //                    MoraleMod = (short)(MoraleMod + 8);
        //                    break;
        //                }

        //            case var @case when @case > 2:
        //                {
        //                    MoraleMod = (short)(MoraleMod + 2 * rel_lv + 4);
        //                    break;
        //                }
        //        }
        //    }
        //}
    }
}
