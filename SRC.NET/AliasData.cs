using System;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    internal class AliasDataType
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // エリアスデータのクラス

        // 名称
        public string Name;
        private string[] strAliasType;
        private double[] dblAliasLevel;
        private bool[] blnAliasLevelIsPlusMod;
        private bool[] blnAliasLevelIsMultMod;
        private string[] strAliasData;
        private string[] strAliasNecessarySkill;
        private string[] strAliasNecessaryCondition;



        // クラスの初期化
        // UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Initialize_Renamed()
        {
            strAliasType = new string[1];
            dblAliasLevel = new double[1];
            blnAliasLevelIsPlusMod = new bool[1];
            blnAliasLevelIsMultMod = new bool[1];
            strAliasData = new string[1];
            strAliasNecessarySkill = new string[1];
            strAliasNecessaryCondition = new string[1];
        }

        public AliasDataType() : base()
        {
            Class_Initialize_Renamed();
        }


        // エリアスの個数
        public short Count
        {
            get
            {
                short CountRet = default;
                CountRet = (short)Information.UBound(strAliasType);
                return CountRet;
            }
        }

        // エリアスの種類
        public string get_AliasType(short idx)
        {
            string AliasTypeRet = default;
            AliasTypeRet = strAliasType[idx];
            return AliasTypeRet;
        }

        // エリアスのレベル
        public double get_AliasLevel(short idx)
        {
            double AliasLevelRet = default;
            AliasLevelRet = dblAliasLevel[idx];
            return AliasLevelRet;
        }

        // エリアスのレベルが＋修正値かどうか
        public bool get_AliasLevelIsPlusMod(short idx)
        {
            bool AliasLevelIsPlusModRet = default;
            AliasLevelIsPlusModRet = blnAliasLevelIsPlusMod[idx];
            return AliasLevelIsPlusModRet;
        }

        // エリアスのレベルが×修正値かどうか
        public bool get_AliasLevelIsMultMod(short idx)
        {
            bool AliasLevelIsMultModRet = default;
            AliasLevelIsMultModRet = blnAliasLevelIsMultMod[idx];
            return AliasLevelIsMultModRet;
        }

        // エリアスのデータ
        public string get_AliasData(short idx)
        {
            string AliasDataRet = default;
            AliasDataRet = strAliasData[idx];
            return AliasDataRet;
        }

        // エリアスの必要技能
        public string get_AliasNecessarySkill(short idx)
        {
            string AliasNecessarySkillRet = default;
            AliasNecessarySkillRet = strAliasNecessarySkill[idx];
            return AliasNecessarySkillRet;
        }

        // エリアスの必要条件
        public string get_AliasNecessaryCondition(short idx)
        {
            string AliasNecessaryConditionRet = default;
            AliasNecessaryConditionRet = strAliasNecessaryCondition[idx];
            return AliasNecessaryConditionRet;
        }

        // エリアスを登録
        public void AddAlias(string adef)
        {
            string atype, adata = default;
            double alevel;
            short j, i, n;
            string buf;

            // エリアスの領域を確保
            n = (short)(Information.UBound(strAliasType) + 1);
            Array.Resize(ref strAliasType, n + 1);
            Array.Resize(ref dblAliasLevel, n + 1);
            Array.Resize(ref blnAliasLevelIsPlusMod, n + 1);
            Array.Resize(ref blnAliasLevelIsMultMod, n + 1);
            Array.Resize(ref strAliasData, n + 1);
            Array.Resize(ref strAliasNecessarySkill, n + 1);
            Array.Resize(ref strAliasNecessaryCondition, n + 1);

            // 必要技能の切り出し
            if (Strings.Right(adef, 1) == ")")
            {
                i = (short)Strings.InStr(adef, " (");
                if (i > 0)
                {
                    strAliasNecessarySkill[n] = Strings.Trim(Strings.Mid(adef, i + 2, Strings.Len(adef) - i - 2));
                    buf = Strings.Trim(Strings.Left(adef, i));
                }
                else if (Strings.Left(adef, 1) == "(")
                {
                    strAliasNecessarySkill[n] = Strings.Trim(Strings.Mid(adef, 2, Strings.Len(adef) - 2));
                    buf = "";
                }
                else
                {
                    buf = adef;
                }
            }
            else
            {
                buf = adef;
            }

            // 必要条件の切り出し
            if (Strings.Right(buf, 1) == ">")
            {
                i = (short)Strings.InStr(adef, " <");
                if (i > 0)
                {
                    strAliasNecessaryCondition[n] = Strings.Trim(Strings.Mid(buf, i + 2, Strings.Len(buf) - i - 2));
                    buf = Strings.Trim(Strings.Left(buf, i));
                }
                else if (Strings.Left(buf, 1) == "<")
                {
                    strAliasNecessaryCondition[n] = Strings.Trim(Strings.Mid(buf, 2, Strings.Len(buf) - 2));
                    buf = "";
                }
            }

            // レベル指定が修正値か？
            string args2 = "Lv+";
            string args3 = "Lv";
            string args21 = "Lv*";
            string args31 = "Lv";
            if (GeneralLib.ReplaceString(ref buf, ref args2, ref args3))
            {
                blnAliasLevelIsPlusMod[n] = true;
            }
            else if (GeneralLib.ReplaceString(ref buf, ref args21, ref args31))
            {
                blnAliasLevelIsMultMod[n] = true;
            }

            // エリアスの種類、レベル、データを切り出し
            alevel = SRC.DEFAULT_LEVEL;
            i = (short)Strings.InStr(buf, "Lv");
            j = (short)Strings.InStr(buf, "=");
            if (i > 0 & j > 0 & i > j)
            {
                i = 0;
            }

            if (i > 0)
            {
                atype = Strings.Left(buf, i - 1);
                if (j > 0)
                {
                    alevel = Conversions.ToDouble(Strings.Mid(buf, i + 2, j - (i + 2)));
                    adata = Strings.Mid(buf, j + 1);
                }
                else
                {
                    alevel = Conversions.ToDouble(Strings.Mid(buf, i + 2));
                }
            }
            else if (j > 0)
            {
                atype = Strings.Left(buf, j - 1);
                adata = Strings.Mid(buf, j + 1);
            }
            else
            {
                atype = buf;
            }

            // エリアスを登録
            strAliasType[n] = atype;
            dblAliasLevel[n] = alevel;
            strAliasData[n] = adata;
        }
    }
}