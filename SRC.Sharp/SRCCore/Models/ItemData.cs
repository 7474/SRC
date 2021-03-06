using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    internal class ItemData
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // アイテムデータのクラス

        // 名称
        public string Name;
        // クラス
        // UPGRADE_NOTE: Class は Class_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public string Class_Renamed;
        // 装備個所
        public string Part;
        // ＨＰ修正値
        public int HP;
        // ＥＮ修正値
        public short EN;
        // 装甲修正値
        public int Armor;
        // 運動性修正値
        public short Mobility;
        // 移動力修正値
        public short Speed;
        // 解説
        public string Comment;

        // 愛称
        private string proNickname;
        // 読み仮名
        private string proKanaName;

        // 特殊能力
        public Collection colFeature;
        // 武器データ
        private Collection colWeaponData;
        // アビリティデータ
        private Collection colAbilityData;

        // クラスの解放
        // UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Terminate_Renamed()
        {
            short i;
            if (colFeature is object)
            {
                {
                    var withBlock = colFeature;
                    var loopTo = (short)withBlock.Count;
                    for (i = 1; i <= loopTo; i++)
                        withBlock.Remove(1);
                }
                // UPGRADE_NOTE: オブジェクト colFeature をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                colFeature = null;
            }

            if (colWeaponData is object)
            {
                {
                    var withBlock1 = colWeaponData;
                    var loopTo1 = (short)withBlock1.Count;
                    for (i = 1; i <= loopTo1; i++)
                        withBlock1.Remove(1);
                }
                // UPGRADE_NOTE: オブジェクト colWeaponData をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                colWeaponData = null;
            }

            if (colAbilityData is object)
            {
                {
                    var withBlock2 = colAbilityData;
                    var loopTo2 = (short)withBlock2.Count;
                    for (i = 1; i <= loopTo2; i++)
                        withBlock2.Remove(1);
                }
                // UPGRADE_NOTE: オブジェクト colAbilityData をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                colAbilityData = null;
            }
        }

        ~ItemData()
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
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 10711


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
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 11088


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

        // 特殊能力の必要技能
        public string FeatureNecessarySkill(ref object Index)
        {
            string FeatureNecessarySkillRet = default;
            FeatureData fd;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 11386


            Input:

                    On Error GoTo ErrorHandler

             */
            fd = (FeatureData)colFeature[Index];
            FeatureNecessarySkillRet = fd.NecessarySkill;
            return FeatureNecessarySkillRet;
            ErrorHandler:
            ;
            FeatureNecessarySkillRet = "";
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

        // 指定した特殊能力はレベル指定がされているか？
        public bool IsFeatureLevelSpecified(ref object Index)
        {
            bool IsFeatureLevelSpecifiedRet = default;
            FeatureData fd;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 12107


            Input:

                    On Error GoTo ErrorHandler

             */
            fd = (FeatureData)colFeature[Index];
            if (fd.Level == SRC.DEFAULT_LEVEL)
            {
                IsFeatureLevelSpecifiedRet = false;
            }
            else
            {
                IsFeatureLevelSpecifiedRet = true;
            }

            return IsFeatureLevelSpecifiedRet;
            ErrorHandler:
            ;
            IsFeatureLevelSpecifiedRet = false;
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
            colAbilityData.Add(new_adata, Name + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(CountAbility()));
            AddAbilityRet = new_adata;
            return AddAbilityRet;
        }

        // サイズ(アイテムが消費するアイテムスロット数)
        public short Size()
        {
            short SizeRet = default;
            object argIndex1 = "大型アイテム";
            SizeRet = (short)(1d + FeatureLevel(ref argIndex1));
            return SizeRet;
        }
    }
}