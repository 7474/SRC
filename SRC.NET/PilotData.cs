﻿using System;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    internal class PilotData
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // パイロットデータのクラス

        // 名称
        public string Name;
        // 性別
        public string Sex;
        // クラス
        // UPGRADE_NOTE: Class は Class_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public string Class_Renamed;
        // 地形適応
        public string Adaption;
        // 経験値
        public short ExpValue;
        // 格闘
        public short Infight;
        // 射撃
        public short Shooting;
        // 命中
        public short Hit;
        // 回避
        public short Dodge;
        // 反応
        public short Intuition;
        // 技量
        public short Technique;
        // 性格
        public string Personality;
        // ＳＰ
        public short SP;
        // ＭＩＤＩ
        public string BGM;

        // 愛称
        private string proNickname;
        // 読み仮名
        private string proKanaName;

        // ビットマップ名
        private string proBitmap;
        // ビットマップが存在するか
        public bool IsBitmapMissing;

        // スペシャルパワー
        private string[] SpecialPowerName;
        private short[] SpecialPowerNecessaryLevel;
        private short[] SpecialPowerSPConsumption;

        // 特殊能力
        public Collection colSkill = new Collection();

        // ユニットに付加するデータ
        // 特殊能力
        public Collection colFeature;
        // 武器データ
        private Collection colWeaponData;
        // アビリティデータ
        private Collection colAbilityData;


        // クラスの初期化
        // UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Initialize_Renamed()
        {
            SpecialPowerName = new string[1];
            SpecialPowerNecessaryLevel = new short[1];
            SpecialPowerSPConsumption = new short[1];
        }

        public PilotData() : base()
        {
            Class_Initialize_Renamed();
        }

        // クラスの解放
        // UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Terminate_Renamed()
        {
            short i;
            {
                var withBlock = colSkill;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }
            // UPGRADE_NOTE: オブジェクト colSkill をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            colSkill = null;
            if (colFeature is object)
            {
                {
                    var withBlock1 = colFeature;
                    var loopTo1 = (short)withBlock1.Count;
                    for (i = 1; i <= loopTo1; i++)
                        withBlock1.Remove(1);
                }
                // UPGRADE_NOTE: オブジェクト colFeature をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                colFeature = null;
            }

            if (colWeaponData is object)
            {
                {
                    var withBlock2 = colWeaponData;
                    var loopTo2 = (short)withBlock2.Count;
                    for (i = 1; i <= loopTo2; i++)
                        withBlock2.Remove(1);
                }
                // UPGRADE_NOTE: オブジェクト colWeaponData をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                colWeaponData = null;
            }

            if (colAbilityData is object)
            {
                {
                    var withBlock3 = colAbilityData;
                    var loopTo3 = (short)withBlock3.Count;
                    for (i = 1; i <= loopTo3; i++)
                        withBlock3.Remove(1);
                }
                // UPGRADE_NOTE: オブジェクト colAbilityData をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                colAbilityData = null;
            }
        }

        ~PilotData()
        {
            Class_Terminate_Renamed();
        }

        // 愛称

        public string Nickname
        {
            get
            {
                string NicknameRet = default;
                NicknameRet = proNickname;
                if (Strings.InStr(NicknameRet, "主人公") == 1 | Strings.InStr(NicknameRet, "ヒロイン") == 1)
                {
                    string argexpr = NicknameRet + "愛称";
                    NicknameRet = Expression.GetValueAsString(ref argexpr);
                }

                Expression.ReplaceSubExpression(ref NicknameRet);
                return NicknameRet;
            }

            set
            {
                proNickname = value;
            }
        }

        // 読み仮名

        public string KanaName
        {
            get
            {
                string KanaNameRet = default;
                KanaNameRet = proKanaName;
                if (Strings.InStr(KanaNameRet, "主人公") == 1 | Strings.InStr(KanaNameRet, "ヒロイン") == 1 | Strings.InStr(KanaNameRet, "ひろいん") == 1)
                {
                    string argvar_name = KanaNameRet + "読み仮名";
                    if (Expression.IsVariableDefined(ref argvar_name))
                    {
                        string argexpr = KanaNameRet + "読み仮名";
                        KanaNameRet = Expression.GetValueAsString(ref argexpr);
                    }
                    else
                    {
                        string localGetValueAsString() { string argexpr = KanaNameRet + "愛称"; var ret = Expression.GetValueAsString(ref argexpr); return ret; }

                        string argstr_Renamed = localGetValueAsString();
                        KanaNameRet = GeneralLib.StrToHiragana(ref argstr_Renamed);
                    }
                }

                Expression.ReplaceSubExpression(ref KanaNameRet);
                return KanaNameRet;
            }

            set
            {
                proKanaName = value;
            }
        }

        // ビットマップ
        public string Bitmap0
        {
            get
            {
                string Bitmap0Ret = default;
                Bitmap0Ret = proBitmap;
                return Bitmap0Ret;
            }
        }

        public string Bitmap
        {
            get
            {
                string BitmapRet = default;
                if (IsBitmapMissing)
                {
                    BitmapRet = "-.bmp";
                }
                else
                {
                    BitmapRet = proBitmap;
                }

                return BitmapRet;
            }

            set
            {
                proBitmap = value;
            }
        }


        // 特殊能力を追加
        public void AddSkill(ref string sname, double slevel, string sdata, short lv)
        {
            SkillData sd;
            short i;
            ;

            // データ定義が省略されている場合は前回と同じ物を使う
            if ((last_sname ?? "") == (sname ?? "") & Strings.Len(sdata) == 0)
            {
                sdata = last_sdata;
            }

            last_sname = sname;
            last_sdata = sdata;

            // エリアスが定義されている？
            object argIndex2 = sname;
            if (SRC.ALDList.IsDefined(ref argIndex2))
            {
                if (GeneralLib.LIndex(ref sdata, 1) != "解説")
                {
                    object argIndex1 = sname;
                    {
                        var withBlock = SRC.ALDList.Item(ref argIndex1);
                        var loopTo = withBlock.Count;
                        for (i = 1; i <= loopTo; i++)
                        {
                            // エリアスの定義に従って特殊能力定義を置き換える
                            sd = new SkillData();
                            sd.Name = withBlock.get_AliasType(i);
                            string localLIndex() { string arglist = withBlock.get_AliasData(i); var ret = GeneralLib.LIndex(ref arglist, 1); withBlock.get_AliasData(i) = arglist; return ret; }

                            if (localLIndex() == "解説")
                            {
                                if (!string.IsNullOrEmpty(sdata))
                                {
                                    sd.Name = GeneralLib.LIndex(ref sdata, 1);
                                }
                            }

                            if (withBlock.get_AliasLevelIsPlusMod(i))
                            {
                                if (slevel == SRC.DEFAULT_LEVEL)
                                {
                                    slevel = 1d;
                                }

                                sd.Level = slevel + withBlock.get_AliasLevel(i);
                            }
                            else if (withBlock.get_AliasLevelIsMultMod(i))
                            {
                                if (slevel == SRC.DEFAULT_LEVEL)
                                {
                                    slevel = 1d;
                                }

                                sd.Level = slevel * withBlock.get_AliasLevel(i);
                            }
                            else if (slevel != SRC.DEFAULT_LEVEL)
                            {
                                sd.Level = slevel;
                            }
                            else
                            {
                                sd.Level = withBlock.get_AliasLevel(i);
                            }

                            sd.StrData = withBlock.get_AliasData(i);
                            if (!string.IsNullOrEmpty(sdata))
                            {
                                string localLIndex1() { string arglist = withBlock.get_AliasData(i); var ret = GeneralLib.LIndex(ref arglist, 1); withBlock.get_AliasData(i) = arglist; return ret; }

                                if (withBlock.get_AliasData(i) != "非表示" & localLIndex1() != "解説")
                                {
                                    string localListTail() { string arglist = withBlock.get_AliasData(i); var ret = GeneralLib.ListTail(ref arglist, 2); withBlock.get_AliasData(i) = arglist; return ret; }

                                    sd.StrData = Strings.Trim(sdata + " " + localListTail());
                                }
                            }

                            if (withBlock.get_AliasLevelIsPlusMod(i) | withBlock.get_AliasLevelIsMultMod(i))
                            {
                                sd.StrData = sd.StrData + "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(slevel);
                            }

                            sd.NecessaryLevel = lv;
                            colSkill.Add(sd, sname + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(colSkill.Count));
                        }
                    }

                    return;
                }
            }

            // 特殊能力を登録
            sd = new SkillData();
            sd.Name = sname;
            sd.Level = slevel;
            sd.StrData = sdata;
            sd.NecessaryLevel = lv;
            colSkill.Add(sd, sname + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(colSkill.Count));
        }

        // 指定したレベルの時点で所有しているパイロット用特殊能力を列挙
        public string Skill(short lv)
        {
            string SkillRet = default;
            short skill_num;
            var skill_name = new string[33];
            short i;
            skill_num = 0;
            foreach (SkillData sd in colSkill)
            {
                if (lv >= sd.NecessaryLevel)
                {
                    var loopTo = skill_num;
                    for (i = 1; i <= loopTo; i++)
                    {
                        if ((sd.Name ?? "") == (skill_name[i] ?? ""))
                        {
                            goto NextLoop;
                        }
                    }

                    skill_num = (short)(skill_num + 1);
                    skill_name[skill_num] = sd.Name;
                }

                NextLoop:
                ;
            }

            SkillRet = "";
            var loopTo1 = skill_num;
            for (i = 1; i <= loopTo1; i++)
                SkillRet = SkillRet + skill_name[i] + " ";
            return SkillRet;
        }

        // 指定したレベルで特殊能力snameが使えるか？
        public bool IsSkillAvailable(short lv, ref string sname)
        {
            bool IsSkillAvailableRet = default;
            foreach (SkillData sd in colSkill)
            {
                if ((sname ?? "") == (sd.Name ?? ""))
                {
                    if (lv >= sd.NecessaryLevel)
                    {
                        IsSkillAvailableRet = true;
                        return IsSkillAvailableRet;
                    }
                }
            }

            return IsSkillAvailableRet;
        }

        // 指定したレベルでの特殊能力snameのレベル
        public double SkillLevel(short lv, ref string sname)
        {
            double SkillLevelRet = default;
            short lv2;
            lv2 = 0;
            foreach (SkillData sd in colSkill)
            {
                if ((sname ?? "") == (sd.Name ?? ""))
                {
                    if (sd.NecessaryLevel > lv)
                    {
                        break;
                    }

                    if (sd.NecessaryLevel > lv2)
                    {
                        lv2 = sd.NecessaryLevel;
                        SkillLevelRet = sd.Level;
                    }
                }
            }

            if (SkillLevelRet == SRC.DEFAULT_LEVEL)
            {
                SkillLevelRet = 1d;
            }

            return SkillLevelRet;
        }

        // 必要技能判定用(別名でも判定)
        public short SkillLevel2(short lv, ref string sname)
        {
            short SkillLevel2Ret = default;
            short lv2;
            lv2 = 0;
            foreach (SkillData sd in colSkill)
            {
                if ((sname ?? "") == (sd.Name ?? "") | (sname ?? "") == (sd.StrData ?? ""))
                {
                    if (sd.NecessaryLevel > lv)
                    {
                        break;
                    }

                    if (sd.NecessaryLevel > lv2)
                    {
                        lv2 = sd.NecessaryLevel;
                        SkillLevel2Ret = (short)sd.Level;
                    }
                }
            }

            if (SkillLevel2Ret == SRC.DEFAULT_LEVEL)
            {
                SkillLevel2Ret = 1;
            }

            return SkillLevel2Ret;
        }

        // 指定したレベルでの特殊能力Nameのデータ
        public string SkillData(short lv, ref string sname)
        {
            string SkillDataRet = default;
            short lv2;
            lv2 = 0;
            foreach (SkillData sd in colSkill)
            {
                if ((sname ?? "") == (sd.Name ?? ""))
                {
                    if (sd.NecessaryLevel > lv)
                    {
                        break;
                    }

                    if (sd.NecessaryLevel > lv2)
                    {
                        lv2 = sd.NecessaryLevel;
                        SkillDataRet = sd.StrData;
                    }
                }
            }

            return SkillDataRet;
        }

        // 指定したレベルでの特殊能力Nameの名称
        public string SkillName(short lv, ref string sname)
        {
            string SkillNameRet = default;
            short lv2;
            SkillNameRet = sname;
            lv2 = 0;
            foreach (SkillData sd in colSkill)
            {
                if ((sname ?? "") == (sd.Name ?? ""))
                {
                    if (sd.NecessaryLevel > lv)
                    {
                        break;
                    }

                    if (sd.NecessaryLevel > lv2)
                    {
                        lv2 = sd.NecessaryLevel;
                        if (Strings.Len(sd.StrData) > 0)
                        {
                            SkillNameRet = GeneralLib.LIndex(ref sd.StrData, 1);
                            switch (SkillNameRet ?? "")
                            {
                                case "非表示":
                                    {
                                        return SkillNameRet;
                                    }

                                case "解説":
                                    {
                                        SkillNameRet = "非表示";
                                        return SkillNameRet;
                                    }
                            }

                            if (sname == "階級")
                            {
                                goto NextLoop;
                            }
                        }
                        else
                        {
                            SkillNameRet = sname;
                        }

                        if (sname != "同調率" & sname != "霊力")
                        {
                            if (sd.Level != SRC.DEFAULT_LEVEL & Strings.InStr(SkillNameRet, "Lv") == 0)
                            {
                                SkillNameRet = SkillNameRet + "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(sd.Level);
                            }
                        }
                    }
                }

                NextLoop:
                ;
            }

            // レベル非表示用の括弧を削除
            if (Strings.Left(SkillNameRet, 1) == "(")
            {
                SkillNameRet = Strings.Mid(SkillNameRet, 2, Strings.Len(SkillNameRet) - 2);
            }

            return SkillNameRet;
        }

        // 特殊能力Nameの種類 (愛称=>名称)
        public string SkillType(ref string sname)
        {
            string SkillTypeRet = default;
            foreach (SkillData sd in colSkill)
            {
                if ((sname ?? "") == (sd.Name ?? "") | (sname ?? "") == (sd.StrData ?? ""))
                {
                    SkillTypeRet = sd.Name;
                    return SkillTypeRet;
                }
            }

            // その能力を修得していない場合
            SkillTypeRet = sname;
            return SkillTypeRet;
        }


        // スペシャルパワーを追加
        public void AddSpecialPower(ref string sname, short lv, short sp_consumption)
        {
            Array.Resize(ref SpecialPowerName, Information.UBound(SpecialPowerName) + 1 + 1);
            Array.Resize(ref SpecialPowerNecessaryLevel, Information.UBound(SpecialPowerName) + 1);
            Array.Resize(ref SpecialPowerSPConsumption, Information.UBound(SpecialPowerName) + 1);
            SpecialPowerName[Information.UBound(SpecialPowerName)] = sname;
            SpecialPowerNecessaryLevel[Information.UBound(SpecialPowerName)] = lv;
            SpecialPowerSPConsumption[Information.UBound(SpecialPowerName)] = sp_consumption;
        }

        // 指定したレベルで使用可能なスペシャルパワーの個数
        public short CountSpecialPower(short lv)
        {
            short CountSpecialPowerRet = default;
            short i;
            var loopTo = (short)Information.UBound(SpecialPowerName);
            for (i = 1; i <= loopTo; i++)
            {
                if (SpecialPowerNecessaryLevel[i] <= lv)
                {
                    CountSpecialPowerRet = (short)(CountSpecialPowerRet + 1);
                }
            }

            return CountSpecialPowerRet;
        }

        // 指定したレベルで使用可能なidx番目のスペシャルパワー
        public string SpecialPower(short lv, short idx)
        {
            string SpecialPowerRet = default;
            short i, n;
            n = 0;
            var loopTo = (short)Information.UBound(SpecialPowerName);
            for (i = 1; i <= loopTo; i++)
            {
                if (SpecialPowerNecessaryLevel[i] <= lv)
                {
                    n = (short)(n + 1);
                    if (idx == n)
                    {
                        SpecialPowerRet = SpecialPowerName[i];
                        return SpecialPowerRet;
                    }
                }
            }

            return SpecialPowerRet;
        }

        // 指定したレベルでスペシャルパワーsnameを使えるか？
        public bool IsSpecialPowerAvailable(short lv, ref string sname)
        {
            bool IsSpecialPowerAvailableRet = default;
            short i;
            var loopTo = (short)Information.UBound(SpecialPowerName);
            for (i = 1; i <= loopTo; i++)
            {
                if ((SpecialPowerName[i] ?? "") == (sname ?? ""))
                {
                    if (SpecialPowerNecessaryLevel[i] <= lv)
                    {
                        IsSpecialPowerAvailableRet = true;
                    }

                    return IsSpecialPowerAvailableRet;
                }
            }

            IsSpecialPowerAvailableRet = false;
            return IsSpecialPowerAvailableRet;
        }

        // 指定したスペシャルパワーsnameのＳＰ消費量
        public short SpecialPowerCost(string sname)
        {
            short SpecialPowerCostRet = default;
            short i;

            // パイロットデータ側でＳＰ消費量が定義されているか検索
            var loopTo = (short)Information.UBound(SpecialPowerName);
            for (i = 1; i <= loopTo; i++)
            {
                if ((SpecialPowerName[i] ?? "") == (sname ?? ""))
                {
                    SpecialPowerCostRet = SpecialPowerSPConsumption[i];
                    break;
                }
            }

            // パイロットデータ側でＳＰ消費量が定義されていなければ
            // デフォルトの値を使う
            if (SpecialPowerCostRet == 0)
            {
                SpecialPowerData localItem() { object argIndex1 = sname; var ret = SRC.SPDList.Item(ref argIndex1); return ret; }

                SpecialPowerCostRet = localItem().SPConsumption;
            }

            return SpecialPowerCostRet;
        }


        // データを消去
        public void Clear()
        {
            short i;
            SpecialPowerName = new string[1];
            SpecialPowerNecessaryLevel = new short[1];
            SpecialPowerSPConsumption = new short[1];
            {
                var withBlock = colSkill;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }

            if (colFeature is object)
            {
                {
                    var withBlock1 = colFeature;
                    var loopTo1 = (short)withBlock1.Count;
                    for (i = 1; i <= loopTo1; i++)
                        withBlock1.Remove(1);
                }
            }

            if (colWeaponData is object)
            {
                {
                    var withBlock2 = colWeaponData;
                    var loopTo2 = (short)withBlock2.Count;
                    for (i = 1; i <= loopTo2; i++)
                        withBlock2.Remove(1);
                }
            }

            if (colAbilityData is object)
            {
                {
                    var withBlock3 = colAbilityData;
                    var loopTo3 = (short)withBlock3.Count;
                    for (i = 1; i <= loopTo3; i++)
                        withBlock3.Remove(1);
                }
            }
        }


        // 特殊能力を追加
        public void AddFeature(ref string fdef)
        {
            FeatureData fd;
            string ftype, fdata = default;
            double flevel;
            string nskill = default, ncondition = default;
            short i, j;
            string buf;
            if (colFeature is null)
            {
                colFeature = new Collection();
            }

            // 必要技能の切り出し
            if (Strings.Right(fdef, 1) == ")")
            {
                i = (short)Strings.InStr(fdef, " (");
                if (i > 0)
                {
                    nskill = Strings.Trim(Strings.Mid(fdef, i + 2, Strings.Len(fdef) - i - 2));
                    buf = Strings.Trim(Strings.Left(fdef, i));
                }
                else if (Strings.Left(fdef, 1) == "(")
                {
                    nskill = Strings.Trim(Strings.Mid(fdef, 2, Strings.Len(fdef) - 2));
                    buf = "";
                }
                else
                {
                    buf = fdef;
                }
            }
            else
            {
                buf = fdef;
            }

            // 必要条件の切り出し
            if (Strings.Right(buf, 1) == ">")
            {
                i = (short)Strings.InStr(buf, " <");
                if (i > 0)
                {
                    ncondition = Strings.Trim(Strings.Mid(buf, i + 2, Strings.Len(buf) - i - 2));
                    buf = Strings.Trim(Strings.Left(buf, i));
                }
                else if (Strings.Left(buf, 1) == "<")
                {
                    ncondition = Strings.Trim(Strings.Mid(buf, 2, Strings.Len(buf) - 2));
                    buf = "";
                }
            }

            // 特殊能力の種類、レベル、データを切り出し
            flevel = SRC.DEFAULT_LEVEL;
            i = (short)Strings.InStr(buf, "Lv");
            j = (short)Strings.InStr(buf, "=");
            if (i > 0 & j > 0 & i > j)
            {
                i = 0;
            }

            if (i > 0)
            {
                ftype = Strings.Left(buf, i - 1);
                if (j > 0)
                {
                    flevel = Conversions.ToDouble(Strings.Mid(buf, i + 2, j - (i + 2)));
                    fdata = Strings.Mid(buf, j + 1);
                }
                else
                {
                    flevel = Conversions.ToDouble(Strings.Mid(buf, i + 2));
                }
            }
            else if (j > 0)
            {
                ftype = Strings.Left(buf, j - 1);
                fdata = Strings.Mid(buf, j + 1);
            }
            else
            {
                ftype = buf;
            }

            // データが「"」で囲まれている場合、「"」を削除
            if (Strings.Left(fdata, 1) == "\"")
            {
                if (Strings.Right(fdata, 1) == "\"")
                {
                    fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
                }
            }

            // エリアスが定義されている？
            object argIndex2 = ftype;
            if (SRC.ALDList.IsDefined(ref argIndex2))
            {
                if (GeneralLib.LIndex(ref fdata, 1) != "解説")
                {
                    object argIndex1 = ftype;
                    {
                        var withBlock = SRC.ALDList.Item(ref argIndex1);
                        var loopTo = withBlock.Count;
                        for (i = 1; i <= loopTo; i++)
                        {
                            fd = new FeatureData();

                            // エリアスの定義に従って特殊能力定義を置き換える
                            fd.Name = withBlock.get_AliasType(i);
                            if ((withBlock.get_AliasType(i) ?? "") != (ftype ?? ""))
                            {
                                if (withBlock.get_AliasLevelIsPlusMod(i))
                                {
                                    if (flevel == SRC.DEFAULT_LEVEL)
                                    {
                                        flevel = 1d;
                                    }

                                    if (withBlock.get_AliasLevel(i) == SRC.DEFAULT_LEVEL)
                                    {
                                        fd.Level = flevel + 1d;
                                    }
                                    else
                                    {
                                        fd.Level = flevel + withBlock.get_AliasLevel(i);
                                    }
                                }
                                else if (withBlock.get_AliasLevelIsMultMod(i))
                                {
                                    if (flevel == SRC.DEFAULT_LEVEL)
                                    {
                                        flevel = 1d;
                                    }

                                    if (withBlock.get_AliasLevel(i) == SRC.DEFAULT_LEVEL)
                                    {
                                        fd.Level = flevel;
                                    }
                                    else
                                    {
                                        fd.Level = flevel * withBlock.get_AliasLevel(i);
                                    }
                                }
                                else if (flevel != SRC.DEFAULT_LEVEL)
                                {
                                    fd.Level = flevel;
                                }
                                else
                                {
                                    fd.Level = withBlock.get_AliasLevel(i);
                                }

                                if (!string.IsNullOrEmpty(fdata) & Strings.InStr(withBlock.get_AliasData(i), "非表示") != 1)
                                {
                                    string localListTail() { string arglist = withBlock.get_AliasData(i); var ret = GeneralLib.ListTail(ref arglist, (short)(GeneralLib.LLength(ref fdata) + 1)); withBlock.get_AliasData(i) = arglist; return ret; }

                                    fd.StrData = fdata + " " + localListTail();
                                }
                                else
                                {
                                    fd.StrData = withBlock.get_AliasData(i);
                                }

                                if (withBlock.get_AliasLevelIsMultMod(i))
                                {
                                    buf = fd.StrData;
                                    string args2 = "Lv1";
                                    string args3 = "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(flevel);
                                    GeneralLib.ReplaceString(ref buf, ref args2, ref args3);
                                    fd.StrData = buf;
                                }
                            }
                            else
                            {
                                // 特殊能力解説の定義
                                if (!string.IsNullOrEmpty(fdata) & GeneralLib.LIndex(ref fdata, 1) != "非表示")
                                {
                                    fd.Name = GeneralLib.LIndex(ref fdata, 1);
                                }

                                fd.StrData = withBlock.get_AliasData(i);
                            }

                            if (!string.IsNullOrEmpty(nskill))
                            {
                                fd.NecessarySkill = nskill;
                            }
                            else
                            {
                                fd.NecessarySkill = withBlock.get_AliasNecessarySkill(i);
                            }

                            if (!string.IsNullOrEmpty(ncondition))
                            {
                                fd.NecessaryCondition = ncondition;
                            }
                            else
                            {
                                fd.NecessaryCondition = withBlock.get_AliasNecessaryCondition(i);
                            }

                            // 特殊能力を登録
                            if (IsFeatureAvailable(ref fd.Name))
                            {
                                colFeature.Add(fd, fd.Name + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(CountFeature()));
                            }
                            else
                            {
                                colFeature.Add(fd, fd.Name);
                            }
                        }
                    }

                    return;
                }
            }

            // 特殊能力を登録
            fd = new FeatureData();
            fd.Name = ftype;
            fd.Level = flevel;
            fd.StrData = fdata;
            fd.NecessarySkill = nskill;
            fd.NecessaryCondition = ncondition;
            if (IsFeatureAvailable(ref ftype))
            {
                colFeature.Add(fd, ftype + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(CountFeature()));
            }
            else
            {
                colFeature.Add(fd, ftype);
            }
        }

        // 特殊能力の総数
        public short CountFeature()
        {
            short CountFeatureRet = default;
            if (colFeature is null)
            {
                return CountFeatureRet;
            }

            CountFeatureRet = (short)colFeature.Count;
            return CountFeatureRet;
        }

        // 特殊能力
        public string Feature(ref object Index)
        {
            string FeatureRet = default;
            FeatureData fd;
            fd = (FeatureData)colFeature[Index];
            FeatureRet = fd.Name;
            return FeatureRet;
        }

        // 特殊能力の名称
        public string FeatureName(ref object Index)
        {
            string FeatureNameRet = default;
            FeatureData fd;
            fd = (FeatureData)colFeature[Index];
            if (Strings.Len(fd.StrData) > 0)
            {
                FeatureNameRet = GeneralLib.ListIndex(ref fd.StrData, 1);
            }
            else if (fd.Level != SRC.DEFAULT_LEVEL)
            {
                FeatureNameRet = fd.Name + "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(fd.Level);
            }
            else
            {
                FeatureNameRet = fd.Name;
            }

            return FeatureNameRet;
        }

        // 特殊能力のレベル
        public double FeatureLevel(ref object Index)
        {
            double FeatureLevelRet = default;
            FeatureData fd;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 22565


            Input:

                    On Error GoTo ErrorHandler

             */
            fd = (FeatureData)colFeature[Index];
            FeatureLevelRet = fd.Level;
            if (FeatureLevelRet == SRC.DEFAULT_LEVEL)
            {
                FeatureLevelRet = 1d;
            }

            return FeatureLevelRet;
            ErrorHandler:
            ;
            FeatureLevelRet = 0d;
        }

        // 特殊能力のデータ
        public string FeatureData(ref object Index)
        {
            string FeatureDataRet = default;
            FeatureData fd;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 22942


            Input:

                    On Error GoTo ErrorHandler

             */
            fd = (FeatureData)colFeature[Index];
            FeatureDataRet = fd.StrData;
            return FeatureDataRet;
            ErrorHandler:
            ;
            FeatureDataRet = "";
        }

        // 指定した特殊能力を持っているか？
        public bool IsFeatureAvailable(ref string fname)
        {
            bool IsFeatureAvailableRet = default;
            if (colFeature is null)
            {
                return IsFeatureAvailableRet;
            }

            foreach (FeatureData fd in colFeature)
            {
                if ((fd.Name ?? "") == (fname ?? ""))
                {
                    IsFeatureAvailableRet = true;
                    return IsFeatureAvailableRet;
                }
            }

            IsFeatureAvailableRet = false;
            return IsFeatureAvailableRet;
        }


        // 武器データ
        public WeaponData Weapon(ref object Index)
        {
            WeaponData WeaponRet = default;
            WeaponRet = (WeaponData)colWeaponData[Index];
            return WeaponRet;
        }

        // 武器の総数
        public short CountWeapon()
        {
            short CountWeaponRet = default;
            if (colWeaponData is null)
            {
                return CountWeaponRet;
            }

            CountWeaponRet = (short)colWeaponData.Count;
            return CountWeaponRet;
        }

        // 武器を追加
        public WeaponData AddWeapon(ref string wname)
        {
            WeaponData AddWeaponRet = default;
            var new_wdata = new WeaponData();
            if (colWeaponData is null)
            {
                colWeaponData = new Collection();
            }

            new_wdata.Name = wname;
            colWeaponData.Add(new_wdata, wname + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(CountWeapon()));
            AddWeaponRet = new_wdata;
            return AddWeaponRet;
        }


        // アビリティデータ
        public AbilityData Ability(ref object Index)
        {
            AbilityData AbilityRet = default;
            AbilityRet = (AbilityData)colAbilityData[Index];
            return AbilityRet;
        }

        // アビリティの総数
        public short CountAbility()
        {
            short CountAbilityRet = default;
            if (colAbilityData is null)
            {
                return CountAbilityRet;
            }

            CountAbilityRet = (short)colAbilityData.Count;
            return CountAbilityRet;
        }

        // アビリティを追加
        public AbilityData AddAbility(ref string aname)
        {
            AbilityData AddAbilityRet = default;
            var new_adata = new AbilityData();
            if (colAbilityData is null)
            {
                colAbilityData = new Collection();
            }

            new_adata.Name = aname;
            colAbilityData.Add(new_adata, aname + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(CountAbility()));
            AddAbilityRet = new_adata;
            return AddAbilityRet;
        }
    }
}