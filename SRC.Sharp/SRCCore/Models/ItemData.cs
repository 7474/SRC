// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.VB;
using System.Collections.Generic;

namespace SRCCore.Models
{
    // アイテムデータのクラス
    public class ItemData : IUnitDataElements
    {
        // 名称
        public string Name;
        // クラス
        public string Class;
        // 装備個所
        public string Part;
        // ＨＰ修正値
        public int HP;
        // ＥＮ修正値
        public int EN;
        // 装甲修正値
        public int Armor;
        // 運動性修正値
        public int Mobility;
        // 移動力修正値
        public int Speed;
        // 解説
        public string Comment;

        // 愛称
        private string proNickname;
        // 読み仮名
        private string proKanaName;

        // ユニットに付加するデータ
        // 特殊能力
        private SrcCollection<FeatureData> colFeature;
        // 武器データ
        private SrcCollection<WeaponData> colWeaponData;
        // アビリティデータ
        private SrcCollection<AbilityData> colAbilityData;

        public IList<FeatureData> Features => colFeature.List;
        public IList<WeaponData> Weapons => colWeaponData.List;
        public IList<AbilityData> Abilities => colAbilityData.List;

        public string Raw = "";
        public string DataComment = "";

        public ItemData() : base()
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
                string NicknameRet = default;
                NicknameRet = proNickname;
                // TODO Impl
                //if (Strings.InStr(NicknameRet, "主人公") == 1 | Strings.InStr(NicknameRet, "ヒロイン") == 1)
                //{
                //    NicknameRet = Expression.GetValueAsString(ref NicknameRet + "愛称");
                //}

                //Expression.ReplaceSubExpression(ref NicknameRet);
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
                //    if (Expression.IsVariableDefined(ref KanaNameRet + "読み仮名"))
                //    {
                //        KanaNameRet = Expression.GetValueAsString(ref KanaNameRet + "読み仮名");
                //    }
                //    else
                //    {
                //        string localGetValueAsString() { string argexpr = KanaNameRet + "愛称"; var ret = Expression.GetValueAsString(ref argexpr); return ret; }

                //        KanaNameRet = GeneralLib.StrToHiragana(ref localGetValueAsString());
                //    }
                //}

                //Expression.ReplaceSubExpression(ref KanaNameRet);
                return KanaNameRet;
            }

            set
            {
                proKanaName = value;
            }
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
            //if (SRC.ALDList.IsDefined(ftype))
            //{
            //    if (GeneralLib.LIndex(fdata, 1) != "解説")
            //    {
            //        {
            //            var withBlock = SRC.ALDList.Item(ftype);
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
            //                        GeneralLib.ReplaceString(buf, "Lv1", "Lv" + SrcFormatter.Format(flevel));
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
        public FeatureData Feature(string Index)
        {
            return colFeature[Index];
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

        // サイズ(アイテムが消費するアイテムスロット数)
        public int Size()
        {
            return (int)(1d + FeatureLevel("大型アイテム"));
        }
    }
}
