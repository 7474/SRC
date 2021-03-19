// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Models
{
    // パイロットデータのクラス
    public class PilotData : IUnitDataElements
    {
        // 名称
        public string Name;
        // 性別
        public string Sex;
        // クラス
        public string Class;
        // 地形適応
        public string Adaption;
        // 経験値
        public int ExpValue;
        // 格闘
        public int Infight;
        // 射撃
        public int Shooting;
        // 命中
        public int Hit;
        // 回避
        public int Dodge;
        // 反応
        public int Intuition;
        // 技量
        public int Technique;
        // 性格
        public string Personality;
        // ＳＰ
        public int SP;
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
        private IList<PilotDataSpecialPower> SpecialPowers;

        // 特殊能力
        private SrcCollection<SkillData> colSkill;

        // ユニットに付加するデータ
        // 特殊能力
        private SrcCollection<FeatureData> colFeature;
        // 武器データ
        private SrcCollection<WeaponData> colWeaponData;
        // アビリティデータ
        private SrcCollection<AbilityData> colAbilityData;

        public IList<SkillData> Skills => colSkill.List;
        public IList<FeatureData> Features => colFeature.List;
        public IList<WeaponData> Weapons => colWeaponData.List;
        public IList<AbilityData> Abilities => colAbilityData.List;

        public string Raw = "";
        public string DataComment = "";

        private SRC SRC;
        public PilotData(SRC src)
        {
            SRC = src;

            SpecialPowers = new List<PilotDataSpecialPower>();
            colSkill = new SrcCollection<SkillData>();
            colFeature = new SrcCollection<FeatureData>();
            colWeaponData = new SrcCollection<WeaponData>();
            colAbilityData = new SrcCollection<AbilityData>();
        }

        // 愛称
        public string Nickname
        {
            get
            {
                string NicknameRet = default;
                NicknameRet = proNickname;
                // TODO Impl
                //if (Strings.InStr(NicknameRet, "主人公") == 1 | Strings.InStr(NicknameRet, "ヒロイン") == 1)
                //{
                //    string argexpr = NicknameRet + "愛称";
                //    NicknameRet = Expression.GetValueAsString(argexpr);
                //}

                //Expression.ReplaceSubExpression(NicknameRet);
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
                // TODO Impl
                //if (Strings.InStr(KanaNameRet, "主人公") == 1 | Strings.InStr(KanaNameRet, "ヒロイン") == 1 | Strings.InStr(KanaNameRet, "ひろいん") == 1)
                //{
                //    string argvar_name = KanaNameRet + "読み仮名";
                //    if (Expression.IsVariableDefined(argvar_name))
                //    {
                //        string argexpr = KanaNameRet + "読み仮名";
                //        KanaNameRet = Expression.GetValueAsString(argexpr);
                //    }
                //    else
                //    {
                //        string localGetValueAsString() { string argexpr = KanaNameRet + "愛称"; var ret = Expression.GetValueAsString(argexpr); return ret; }

                //        string argstr_Renamed = localGetValueAsString();
                //        KanaNameRet = GeneralLib.StrToHiragana(argstr_Renamed);
                //    }
                //}
                //Expression.ReplaceSubExpression(KanaNameRet);
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


        private string last_sname;
        private string last_sdata;
        // 特殊能力を追加
        public void AddSkill(string sname, double slevel, string sdata, int lv)
        {
            // データ定義が省略されている場合は前回と同じ物を使う
            if ((last_sname ?? "") == (sname ?? "") & Strings.Len(sdata) == 0)
            {
                sdata = last_sdata;
            }
            last_sname = sname;
            last_sdata = sdata;

            // TODO Impl
            //// エリアスが定義されている？
            //object argIndex2 = sname;
            //if (SRC.ALDList.IsDefined(argIndex2))
            //{
            //    if (GeneralLib.LIndex(sdata, 1) != "解説")
            //    {
            //        object argIndex1 = sname;
            //        {
            //            var withBlock = SRC.ALDList.Item(argIndex1);
            //            var loopTo = withBlock.Count;
            //            for (i = 1; i <= loopTo; i++)
            //            {
            //                // エリアスの定義に従って特殊能力定義を置き換える
            //                sd = new SkillData();
            //                sd.Name = withBlock.get_AliasType(i);
            //                string localLIndex() { string arglist = withBlock.get_AliasData(i); var ret = GeneralLib.LIndex(arglist, 1); withBlock.get_AliasData(i) = arglist; return ret; }

            //                if (localLIndex() == "解説")
            //                {
            //                    if (!string.IsNullOrEmpty(sdata))
            //                    {
            //                        sd.Name = GeneralLib.LIndex(sdata, 1);
            //                    }
            //                }

            //                if (withBlock.get_AliasLevelIsPlusMod(i))
            //                {
            //                    if (slevel == Constants.DEFAULT_LEVEL)
            //                    {
            //                        slevel = 1d;
            //                    }

            //                    sd.Level = slevel + withBlock.get_AliasLevel(i);
            //                }
            //                else if (withBlock.get_AliasLevelIsMultMod(i))
            //                {
            //                    if (slevel == Constants.DEFAULT_LEVEL)
            //                    {
            //                        slevel = 1d;
            //                    }

            //                    sd.Level = slevel * withBlock.get_AliasLevel(i);
            //                }
            //                else if (slevel != Constants.DEFAULT_LEVEL)
            //                {
            //                    sd.Level = slevel;
            //                }
            //                else
            //                {
            //                    sd.Level = withBlock.get_AliasLevel(i);
            //                }

            //                sd.StrData = withBlock.get_AliasData(i);
            //                if (!string.IsNullOrEmpty(sdata))
            //                {
            //                    string localLIndex1() { string arglist = withBlock.get_AliasData(i); var ret = GeneralLib.LIndex(arglist, 1); withBlock.get_AliasData(i) = arglist; return ret; }

            //                    if (withBlock.get_AliasData(i) != "非表示" & localLIndex1() != "解説")
            //                    {
            //                        string localListTail() { string arglist = withBlock.get_AliasData(i); var ret = GeneralLib.ListTail(arglist, 2); withBlock.get_AliasData(i) = arglist; return ret; }

            //                        sd.StrData = Strings.Trim(sdata + " " + localListTail());
            //                    }
            //                }

            //                if (withBlock.get_AliasLevelIsPlusMod(i) | withBlock.get_AliasLevelIsMultMod(i))
            //                {
            //                    sd.StrData = sd.StrData + "Lv" + SrcFormatter.Format(slevel);
            //                }

            //                sd.NecessaryLevel = lv;
            //                colSkill.Add(sd, sname + SrcFormatter.Format(colSkill.Count));
            //            }
            //        }

            //        return;
            //    }
            //}

            // 特殊能力を登録
            var sd = new SkillData();
            sd.Name = sname;
            sd.Level = slevel;
            sd.StrData = sdata;
            sd.NecessaryLevel = lv;
            colSkill.Add(sd, sname + SrcFormatter.Format(colSkill.Count));
        }

        // 指定したレベルの時点で所有しているパイロット用特殊能力を列挙
        public string Skill(int lv)
        {
            string SkillRet = default;
            int skill_num;
            var skill_name = new string[33];
            int i;
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
                            continue;
                        }
                    }

                    skill_num = (skill_num + 1);
                    skill_name[skill_num] = sd.Name;
                }
            }

            SkillRet = "";
            var loopTo1 = skill_num;
            for (i = 1; i <= loopTo1; i++)
                SkillRet = SkillRet + skill_name[i] + " ";
            return SkillRet;
        }

        // 指定したレベルで特殊能力snameが使えるか？
        public bool IsSkillAvailable(int lv, string sname)
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
        public double SkillLevel(int lv, string sname)
        {
            double SkillLevelRet = default;
            int lv2;
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

            if (SkillLevelRet == Constants.DEFAULT_LEVEL)
            {
                SkillLevelRet = 1d;
            }

            return SkillLevelRet;
        }

        // 必要技能判定用(別名でも判定)
        public int SkillLevel2(int lv, string sname)
        {
            int SkillLevel2Ret = default;
            int lv2;
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
                        SkillLevel2Ret = (int)sd.Level;
                    }
                }
            }

            if (SkillLevel2Ret == Constants.DEFAULT_LEVEL)
            {
                SkillLevel2Ret = 1;
            }

            return SkillLevel2Ret;
        }

        // 指定したレベルでの特殊能力Nameのデータ
        public string SkillData(int lv, string sname)
        {
            string SkillDataRet = default;
            int lv2;
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
        public string SkillName(int lv, string sname)
        {
            string SkillNameRet = default;
            int lv2;
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
                            SkillNameRet = GeneralLib.LIndex(sd.StrData, 1);
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
                            if (sd.Level != Constants.DEFAULT_LEVEL & Strings.InStr(SkillNameRet, "Lv") == 0)
                            {
                                SkillNameRet = SkillNameRet + "Lv" + SrcFormatter.Format(sd.Level);
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
        public string SkillType(string sname)
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
        public void AddSpecialPower(string sname, int lv, int sp_consumption)
        {
            SpecialPowers.Add(new PilotDataSpecialPower(sname, lv, sp_consumption));
        }

        // 定義されているスペシャルパワー
        public IList<PilotDataSpecialPower> DefinedSpecialPowers()
        {
            // XXX Readonlyにするとか
            return SpecialPowers;
        }

        // 指定したレベルで使用可能なスペシャルパワーの個数
        public int CountSpecialPower(int lv)
        {
            return SpecialPowers.Count(x => x.NecessaryLevel <= lv);
        }

        // 指定したレベルで使用可能なidx番目のスペシャルパワー
        public string SpecialPower(int lv, int idx)
        {
            return SpecialPowerDef(lv, idx)?.Name;
        }
        public PilotDataSpecialPower SpecialPowerDef(int lv, int idx)
        {
            return SpecialPowers.Where(x => x.NecessaryLevel <= lv).Skip(idx - 1).FirstOrDefault();
        }

        // 指定したレベルでスペシャルパワーsnameを使えるか？
        public bool IsSpecialPowerAvailable(int lv, string sname)
        {
            return SpecialPowers
                .Where(x => x.NecessaryLevel <= lv)
                .Where(x => x.Name == sname)
                .Any();
        }

        // 指定したスペシャルパワーsnameのＳＰ消費量
        public int SpecialPowerCost(string sname)
        {
            // パイロットデータ側でＳＰ消費量が定義されているか検索
            var sp = SpecialPowers.FirstOrDefault(x => x.Name == sname);
            int SpecialPowerCostRet = sp?.SPConsumption ?? 0;

            // TODO Impl
            //// パイロットデータ側でＳＰ消費量が定義されていなければ
            //// デフォルトの値を使う
            //if (SpecialPowerCostRet == 0)
            //{
            //    SpecialPowerData localItem() { object argIndex1 = sname; var ret = SRC.SPDList.Item(argIndex1); return ret; }

            //    SpecialPowerCostRet = localItem().SPConsumption;
            //}

            return SpecialPowerCostRet;
        }


        // データを消去
        public void Clear()
        {
            SpecialPowers.Clear();
            colSkill.Clear();
            colFeature.Clear();
            colWeaponData.Clear();
            colAbilityData.Clear();
        }

        // 特殊能力を追加
        public void AddFeature(string fdef)
        {
            string ftype, fdata = default;
            double flevel;
            string nskill = default, ncondition = default;
            int i, j;
            string buf;

            // 必要技能の切り出し
            if (Strings.Right(fdef, 1) == ")")
            {
                i = Strings.InStr(fdef, " (");
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
                i = Strings.InStr(buf, " <");
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
            flevel = Constants.DEFAULT_LEVEL;
            i = Strings.InStr(buf, "Lv");
            j = Strings.InStr(buf, "=");
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

            // Imple
            //エリアスが定義されている？
            //object argIndex2 = ftype;
            //if (SRC.ALDList.IsDefined(argIndex2))
            //{
            //    if (GeneralLib.LIndex(fdata, 1) != "解説")
            //    {
            //        object argIndex1 = ftype;
            //        {
            //            var withBlock = SRC.ALDList.Item(argIndex1);
            //            var loopTo = withBlock.Count;
            //            for (i = 1; i <= loopTo; i++)
            //            {
            //                fd = new FeatureData();

            //                エリアスの定義に従って特殊能力定義を置き換える
            //                fd.Name = withBlock.get_AliasType(i);
            //                if ((withBlock.get_AliasType(i) ?? "") != (ftype ?? ""))
            //                {
            //                    if (withBlock.get_AliasLevelIsPlusMod(i))
            //                    {
            //                        if (flevel == Constants.DEFAULT_LEVEL)
            //                        {
            //                            flevel = 1d;
            //                        }

            //                        if (withBlock.get_AliasLevel(i) == Constants.DEFAULT_LEVEL)
            //                        {
            //                            fd.Level = flevel + 1d;
            //                        }
            //                        else
            //                        {
            //                            fd.Level = flevel + withBlock.get_AliasLevel(i);
            //                        }
            //                    }
            //                    else if (withBlock.get_AliasLevelIsMultMod(i))
            //                    {
            //                        if (flevel == Constants.DEFAULT_LEVEL)
            //                        {
            //                            flevel = 1d;
            //                        }

            //                        if (withBlock.get_AliasLevel(i) == Constants.DEFAULT_LEVEL)
            //                        {
            //                            fd.Level = flevel;
            //                        }
            //                        else
            //                        {
            //                            fd.Level = flevel * withBlock.get_AliasLevel(i);
            //                        }
            //                    }
            //                    else if (flevel != Constants.DEFAULT_LEVEL)
            //                    {
            //                        fd.Level = flevel;
            //                    }
            //                    else
            //                    {
            //                        fd.Level = withBlock.get_AliasLevel(i);
            //                    }

            //                    if (!string.IsNullOrEmpty(fdata) & Strings.InStr(withBlock.get_AliasData(i), "非表示") != 1)
            //                    {
            //                        string localListTail() { string arglist = withBlock.get_AliasData(i); var ret = GeneralLib.ListTail(arglist, (GeneralLib.LLength(fdata) + 1)); withBlock.get_AliasData(i) = arglist; return ret; }

            //                        fd.StrData = fdata + " " + localListTail();
            //                    }
            //                    else
            //                    {
            //                        fd.StrData = withBlock.get_AliasData(i);
            //                    }

            //                    if (withBlock.get_AliasLevelIsMultMod(i))
            //                    {
            //                        buf = fd.StrData;
            //                        string args2 = "Lv1";
            //                        string args3 = "Lv" + SrcFormatter.Format(flevel);
            //                        GeneralLib.ReplaceString(buf, args2, args3);
            //                        fd.StrData = buf;
            //                    }
            //                }
            //                else
            //                {
            //                    特殊能力解説の定義
            //                    if (!string.IsNullOrEmpty(fdata) & GeneralLib.LIndex(fdata, 1) != "非表示")
            //                    {
            //                        fd.Name = GeneralLib.LIndex(fdata, 1);
            //                    }

            //                    fd.StrData = withBlock.get_AliasData(i);
            //                }

            //                if (!string.IsNullOrEmpty(nskill))
            //                {
            //                    fd.NecessarySkill = nskill;
            //                }
            //                else
            //                {
            //                    fd.NecessarySkill = withBlock.get_AliasNecessarySkill(i);
            //                }

            //                if (!string.IsNullOrEmpty(ncondition))
            //                {
            //                    fd.NecessaryCondition = ncondition;
            //                }
            //                else
            //                {
            //                    fd.NecessaryCondition = withBlock.get_AliasNecessaryCondition(i);
            //                }

            //                特殊能力を登録
            //                if (IsFeatureAvailable(fd.Name))
            //                {
            //                    colFeature.Add(fd, fd.Name + SrcFormatter.Format(CountFeature()));
            //                }
            //                else
            //                {
            //                    colFeature.Add(fd, fd.Name);
            //                }
            //            }
            //        }

            //        return;
            //    }
            //}

            // 特殊能力を登録
            var fd = new FeatureData();
            fd.Name = ftype;
            fd.Level = flevel;
            fd.StrData = fdata;
            fd.NecessarySkill = nskill;
            fd.NecessaryCondition = ncondition;
            if (IsFeatureAvailable(ftype))
            {
                colFeature.Add(fd, ftype + SrcFormatter.Format(CountFeature()));
            }
            else
            {
                colFeature.Add(fd, ftype);
            }
        }

        // 特殊能力の総数
        public int CountFeature()
        {
            return colFeature.Count;
        }

        // 特殊能力
        public string Feature(string Index)
        {
            // XXX 例外処理要らんの？
            return colFeature[Index].Name;
        }

        // 特殊能力の名称
        public string FeatureName(string Index)
        {
            string FeatureNameRet = default;
            FeatureData fd = colFeature[Index];
            if (Strings.Len(fd.StrData) > 0)
            {
                FeatureNameRet = GeneralLib.ListIndex(fd.StrData, 1);
            }
            else if (fd.Level != Constants.DEFAULT_LEVEL)
            {
                FeatureNameRet = fd.Name + "Lv" + SrcFormatter.Format(fd.Level);
            }
            else
            {
                FeatureNameRet = fd.Name;
            }

            return FeatureNameRet;
        }

        // 特殊能力のレベル
        public double FeatureLevel(string Index)
        {
            try
            {
                var level = colFeature[Index]?.Level ?? 0d;
                return level == Constants.DEFAULT_LEVEL ? 1d : level;
            }
            catch
            {
                return 0d;
            }
        }

        // 特殊能力のデータ
        public string FeatureData(string Index)
        {
            try
            {
                return colFeature[Index]?.StrData ?? "";
            }
            catch
            {
                return "";
            }
        }

        // 指定した特殊能力を持っているか？
        public bool IsFeatureAvailable(string fname)
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
        public WeaponData Weapon(string Index)
        {
            return colWeaponData[Index];
        }

        // 武器の総数
        public int CountWeapon()
        {
            return colWeaponData.Count;
        }

        // 武器を追加
        public WeaponData AddWeapon(string wname)
        {
            var new_wdata = new WeaponData();
            new_wdata.Name = wname;
            colWeaponData.Add(new_wdata, wname + SrcFormatter.Format(CountWeapon()));
            return new_wdata;
        }

        // アビリティデータ
        public AbilityData Ability(string Index)
        {
            return colAbilityData[Index];
        }

        // アビリティの総数
        public int CountAbility()
        {
            return colAbilityData.Count;
        }

        // アビリティを追加
        public AbilityData AddAbility(string aname)
        {
            var new_adata = new AbilityData();
            new_adata.Name = aname;
            colAbilityData.Add(new_adata, aname + SrcFormatter.Format(CountAbility()));
            return new_adata;
        }
    }
}