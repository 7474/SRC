﻿// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRC.Core.VB;
using System;
using SRC.Core.Expressions;

namespace SRC.Core.Models
{
    // ユニットデータのクラス
    public class UnitData
    {
        // 名称
        public string Name;
        // 識別子
        public int ID;
        // クラス
        // UPGRADE_NOTE: Class は Class_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public string Class_Renamed;
        // パイロット数 (マイナスの場合は括弧つきの指定)
        public short PilotNum;
        // アイテム数
        public short ItemNum;
        // 地形適応
        public string Adaption;
        // ＨＰ
        public int HP;
        // ＥＮ
        public short EN;
        // 移動タイプ
        public string Transportation;
        // 移動力
        public short Speed;
        // サイズ
        public string Size;
        // 装甲
        public int Armor;
        // 運動性
        public short Mobility;
        // 修理費
        public int Value;
        // 経験値
        public short ExpValue;

        // 愛称
        private string proNickname;
        // 読み仮名
        private string proKanaName;

        // ビットマップ名
        private string proBitmap;
        // ビットマップが存在するか
        public bool IsBitmapMissing;

        // 特殊能力
        public SrcCollection<FeatureData> colFeature;
        // 武器データ
        private SrcCollection<WeaponData> colWeaponData;
        // アビリティデータ
        private SrcCollection<AbilityData> colAbilityData;

        public UnitData()
        {
            colFeature = new SrcCollection<FeatureData>();
            colWeaponData = new SrcCollection<WeaponData>();
            colAbilityData = new SrcCollection<AbilityData>();
        }

        // 愛称
        public string Nickname
        {
            get
            {
                string NicknameRet = proNickname;
                // TODO Impl
                //if (Strings.InStr(NicknameRet, "主人公") == 1 | Strings.InStr(NicknameRet, "ヒロイン") == 1)
                //{
                //    string argexpr = NicknameRet + "愛称";
                //    NicknameRet = Expression.GetValueAsString(ref argexpr);
                //}

                return Expression.ReplaceSubExpression(NicknameRet);
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
                string KanaNameRet = proKanaName;
                // TODO Impl
                //if (Strings.InStr(KanaNameRet, "主人公") == 1 | Strings.InStr(KanaNameRet, "ヒロイン") == 1 | Strings.InStr(KanaNameRet, "ひろいん") == 1)
                //{
                //    string argvar_name = KanaNameRet + "読み仮名";
                //    if (Expression.IsVariableDefined(ref argvar_name))
                //    {
                //        string argexpr = KanaNameRet + "読み仮名";
                //        KanaNameRet = Expression.GetValueAsString(ref argexpr);
                //    }
                //    else
                //    {
                //        string localGetValueAsString() { string argexpr = KanaNameRet + "愛称"; var ret = Expression.GetValueAsString(ref argexpr); return ret; }

                //        string argstr_Renamed = localGetValueAsString();
                //        KanaNameRet = GeneralLib.StrToHiragana(ref argstr_Renamed);
                //    }
                //}

                return Expression.ReplaceSubExpression(KanaNameRet);
            }

            set
            {
                proKanaName = value;
            }
        }

        // ビットマップ
        public string Bitmap0 => proBitmap;

        public string Bitmap
        {
            get
            {
                return IsBitmapMissing ? "-.bmp" : proBitmap;
            }

            set
            {
                proBitmap = value;
            }
        }

        // 特殊能力を追加
        public void AddFeature(ref string fdef)
        {
            throw new NotImplementedException();
            //    FeatureData fd;
            //    string ftype, fdata = default;
            //    double flevel;
            //    string nskill = default, ncondition = default;
            //    short i, j;
            //    string buf;
            //    if (colFeature is null)
            //    {
            //        colFeature = new Collection();
            //    }

            //    // 必要技能の切り出し
            //    if (Strings.Right(fdef, 1) == ")")
            //    {
            //        i = (short)Strings.InStr(fdef, " (");
            //        if (i > 0)
            //        {
            //            nskill = Strings.Trim(Strings.Mid(fdef, i + 2, Strings.Len(fdef) - i - 2));
            //            buf = Strings.Trim(Strings.Left(fdef, i));
            //        }
            //        else if (Strings.Left(fdef, 1) == "(")
            //        {
            //            nskill = Strings.Trim(Strings.Mid(fdef, 2, Strings.Len(fdef) - 2));
            //            buf = "";
            //        }
            //        else
            //        {
            //            buf = fdef;
            //        }
            //    }
            //    else
            //    {
            //        buf = fdef;
            //    }

            //    // 必要条件の切り出し
            //    if (Strings.Right(buf, 1) == ">")
            //    {
            //        i = (short)Strings.InStr(buf, " <");
            //        if (i > 0)
            //        {
            //            ncondition = Strings.Trim(Strings.Mid(buf, i + 2, Strings.Len(buf) - i - 2));
            //            buf = Strings.Trim(Strings.Left(buf, i));
            //        }
            //        else if (Strings.Left(buf, 1) == "<")
            //        {
            //            ncondition = Strings.Trim(Strings.Mid(buf, 2, Strings.Len(buf) - 2));
            //            buf = "";
            //        }
            //    }

            //    // 特殊能力の種類、レベル、データを切り出し
            //    flevel = SRC.DEFAULT_LEVEL;
            //    i = (short)Strings.InStr(buf, "Lv");
            //    j = (short)Strings.InStr(buf, "=");
            //    if (i > 0 & j > 0 & i > j)
            //    {
            //        i = 0;
            //    }

            //    if (i > 0)
            //    {
            //        ftype = Strings.Left(buf, i - 1);
            //        if (j > 0)
            //        {
            //            flevel = Conversions.ToDouble(Strings.Mid(buf, i + 2, j - (i + 2)));
            //            fdata = Strings.Mid(buf, j + 1);
            //        }
            //        else
            //        {
            //            flevel = Conversions.ToDouble(Strings.Mid(buf, i + 2));
            //        }
            //    }
            //    else if (j > 0)
            //    {
            //        ftype = Strings.Left(buf, j - 1);
            //        fdata = Strings.Mid(buf, j + 1);
            //    }
            //    else
            //    {
            //        ftype = buf;
            //    }

            //    // データが「"」で囲まれている場合、「"」を削除
            //    if (Strings.Left(fdata, 1) == "\"")
            //    {
            //        if (Strings.Right(fdata, 1) == "\"")
            //        {
            //            fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
            //        }
            //    }

            //    // エリアスが定義されている？
            //    object argIndex2 = ftype;
            //    if (SRC.ALDList.IsDefined(ref argIndex2))
            //    {
            //        if (GeneralLib.LIndex(ref fdata, 1) != "解説")
            //        {
            //            object argIndex1 = ftype;
            //            {
            //                var withBlock = SRC.ALDList.Item(ref argIndex1);
            //                var loopTo = withBlock.Count;
            //                for (i = 1; i <= loopTo; i++)
            //                {
            //                    fd = new FeatureData();

            //                    // エリアスの定義に従って特殊能力定義を置き換える
            //                    fd.Name = withBlock.get_AliasType(i);
            //                    if ((withBlock.get_AliasType(i) ?? "") != (ftype ?? ""))
            //                    {
            //                        if (withBlock.get_AliasLevelIsPlusMod(i))
            //                        {
            //                            if (flevel == SRC.DEFAULT_LEVEL)
            //                            {
            //                                flevel = 1d;
            //                            }

            //                            if (withBlock.get_AliasLevel(i) == SRC.DEFAULT_LEVEL)
            //                            {
            //                                fd.Level = flevel + 1d;
            //                            }
            //                            else
            //                            {
            //                                fd.Level = flevel + withBlock.get_AliasLevel(i);
            //                            }
            //                        }
            //                        else if (withBlock.get_AliasLevelIsMultMod(i))
            //                        {
            //                            if (flevel == SRC.DEFAULT_LEVEL)
            //                            {
            //                                flevel = 1d;
            //                            }

            //                            if (withBlock.get_AliasLevel(i) == SRC.DEFAULT_LEVEL)
            //                            {
            //                                fd.Level = flevel;
            //                            }
            //                            else
            //                            {
            //                                fd.Level = flevel * withBlock.get_AliasLevel(i);
            //                            }
            //                        }
            //                        else if (flevel != SRC.DEFAULT_LEVEL)
            //                        {
            //                            fd.Level = flevel;
            //                        }
            //                        else
            //                        {
            //                            fd.Level = withBlock.get_AliasLevel(i);
            //                        }

            //                        if (!string.IsNullOrEmpty(fdata) & Strings.InStr(withBlock.get_AliasData(i), "非表示") != 1)
            //                        {
            //                            string localListTail() { string arglist = withBlock.get_AliasData(i); var ret = GeneralLib.ListTail(ref arglist, (short)(GeneralLib.LLength(ref fdata) + 1)); withBlock.get_AliasData(i) = arglist; return ret; }

            //                            fd.StrData = fdata + " " + localListTail();
            //                        }
            //                        else
            //                        {
            //                            fd.StrData = withBlock.get_AliasData(i);
            //                        }

            //                        if (withBlock.get_AliasLevelIsMultMod(i))
            //                        {
            //                            buf = fd.StrData;
            //                            string args2 = "Lv1";
            //                            string args3 = "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(flevel);
            //                            GeneralLib.ReplaceString(ref buf, ref args2, ref args3);
            //                            fd.StrData = buf;
            //                        }
            //                    }
            //                    else
            //                    {
            //                        // 特殊能力解説の定義
            //                        if (!string.IsNullOrEmpty(fdata) & GeneralLib.LIndex(ref fdata, 1) != "非表示")
            //                        {
            //                            fd.Name = GeneralLib.LIndex(ref fdata, 1);
            //                        }

            //                        fd.StrData = withBlock.get_AliasData(i);
            //                    }

            //                    if (!string.IsNullOrEmpty(nskill))
            //                    {
            //                        fd.NecessarySkill = nskill;
            //                    }
            //                    else
            //                    {
            //                        fd.NecessarySkill = withBlock.get_AliasNecessarySkill(i);
            //                    }

            //                    if (!string.IsNullOrEmpty(ncondition))
            //                    {
            //                        fd.NecessaryCondition = ncondition;
            //                    }
            //                    else
            //                    {
            //                        fd.NecessaryCondition = withBlock.get_AliasNecessaryCondition(i);
            //                    }

            //                    // 特殊能力を登録
            //                    if (IsFeatureAvailable(ref fd.Name))
            //                    {
            //                        colFeature.Add(fd, fd.Name + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(CountFeature()));
            //                    }
            //                    else
            //                    {
            //                        colFeature.Add(fd, fd.Name);
            //                    }
            //                }
            //            }

            //            return;
            //        }
            //    }

            //    // 特殊能力を登録
            //    fd = new FeatureData();
            //    fd.Name = ftype;
            //    fd.Level = flevel;
            //    fd.StrData = fdata;
            //    fd.NecessarySkill = nskill;
            //    fd.NecessaryCondition = ncondition;
            //    if (IsFeatureAvailable(ref ftype))
            //    {
            //        colFeature.Add(fd, ftype + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(CountFeature()));
            //    }
            //    else
            //    {
            //        colFeature.Add(fd, ftype);
            //    }
        }

        // 特殊能力の総数
        public int CountFeature()
        {
            return colFeature.Count;
        }

        // 特殊能力
        public string Feature(int Index)
        {
            return colFeature[Index].Name;
        }

        // 特殊能力の名称
        public string FeatureName(int Index)
        {
            FeatureData fd = colFeature[Index];
            if (Strings.Len(fd.StrData) > 0)
            {
                throw new NotImplementedException();
                // TODO
                //FeatureNameRet = GeneralLib.ListIndex(ref fd.StrData, 1);
            }
            else if (fd.Level > 0d)
            {
                return fd.Name + "Lv" + SrcFormatter.Format(fd.Level);
            }
            else
            {
                return fd.Name;
            }
        }

        // 特殊能力のレベル
        public double FeatureLevel(int Index)
        {
            try
            {
                var level = colFeature[Index].Level;
                return level == Constants.DEFAULT_LEVEL ? 0 : level;
            }
            catch
            {
                return 0;
            }
        }

        // 特殊能力のデータ
        public string FeatureData(int Index)
        {
            try
            {
                return colFeature[Index].StrData;
            }
            catch
            {
                return "";
            }
        }

        // 特殊能力の必要技能
        public string FeatureNecessarySkill(int Index)
        {
            try
            {
                return colFeature[Index].NecessarySkill;
            }
            catch
            {
                return "";
            }
        }

        // 指定した特殊能力を持っているか？
        public bool IsFeatureAvailable(string fname)
        {
            return colFeature.ContainsKey(fname);
        }

        // 指定した特殊能力がレベル指定されているか？
        public bool IsFeatureLevelSpecified(int Index)
        {
            try
            {
                return colFeature[Index].Level != Constants.DEFAULT_LEVEL;
            }
            catch
            {
                return false;
            }
        }

        // 武器を追加
        public WeaponData AddWeapon(ref string wname)
        {
            var new_wdata = new WeaponData();

            new_wdata.Name = wname;
            colWeaponData.Add(new_wdata, wname + SrcFormatter.Format(CountWeapon()));
            return new_wdata;
        }

        // 武器の総数
        public int CountWeapon()
        {
            return colWeaponData.Count;
        }

        // 武器データ
        public WeaponData Weapon(int Index)
        {
            return colWeaponData[Index];
        }

        // アビリティを追加
        public AbilityData AddAbility(ref string aname)
        {
            var new_sadata = new AbilityData();

            new_sadata.Name = aname;
            colAbilityData.Add(new_sadata, aname + SrcFormatter.Format(CountAbility()));
            return new_sadata;
        }

        // アビリティの総数
        public int CountAbility()
        {
            return colAbilityData.Count;
        }

        // アビリティデータ
        public AbilityData Ability(int Index)
        {
            return colAbilityData[Index];
        }

        // 特殊能力、武器データ、アビリティデータを削除する
        public void Clear()
        {
            colFeature.Clear();
            colWeaponData.Clear();
            colAbilityData.Clear();
        }
    }
}