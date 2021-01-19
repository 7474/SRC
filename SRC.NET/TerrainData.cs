using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    internal class TerrainData
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // 地形データのクラス

        // 識別番号
        public short ID;
        // 名称
        public string Name;
        // ビットマップ名
        // UPGRADE_NOTE: Bitmap は Bitmap_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public string Bitmap_Renamed;
        // 地形タイプ
        // UPGRADE_NOTE: Class は Class_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public string Class_Renamed;
        // 移動コスト
        public short MoveCost;
        // 命中修正
        public short HitMod;
        // ダメージ修正
        public short DamageMod;

        // 地形効果
        public Collection colFeature;


        // クラスの初期化
        // UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Initialize_Renamed()
        {
            ID = -1;
        }

        public TerrainData() : base()
        {
            Class_Initialize_Renamed();
        }

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
        }

        ~TerrainData()
        {
            Class_Terminate_Renamed();
        }


        // 地形効果を追加
        public void AddFeature(ref string fdef)
        {
            FeatureData fd;
            string ftype, fdata = default;
            double flevel;
            short i, j;
            string buf;
            if (colFeature is null)
            {
                colFeature = new Collection();
            }

            buf = fdef;

            // 地形効果の種類、レベル、データを切り出し
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

            // 地形効果を登録
            fd = new FeatureData();
            fd.Name = ftype;
            fd.Level = flevel;
            fd.StrData = fdata;
            if (IsFeatureAvailable(ref ftype))
            {
                colFeature.Add(fd, ftype + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(CountFeature()));
            }
            else
            {
                colFeature.Add(fd, ftype);
            }
        }

        // 地形効果の総数
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

        // 地形効果
        public string Feature(ref object Index)
        {
            string FeatureRet = default;
            FeatureData fd;
            fd = (FeatureData)colFeature[Index];
            FeatureRet = fd.Name;
            return FeatureRet;
        }

        // 地形効果の名称
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

        // 地形効果のレベル
        public double FeatureLevel(ref object Index)
        {
            double FeatureLevelRet = default;
            FeatureData fd;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 4536


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

        // 地形効果のデータ
        public string FeatureData(ref object Index)
        {
            string FeatureDataRet = default;
            FeatureData fd;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 4913


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

        // 指定した地形効果を持っているか？
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

        // 指定した地形効果はレベル指定がされているか？
        public bool IsFeatureLevelSpecified(ref object Index)
        {
            bool IsFeatureLevelSpecifiedRet = default;
            FeatureData fd;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 5607


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

        // データをクリア
        public void Clear()
        {
            short i;
            ID = -1;
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
        }
    }
}