// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.VB;
using System.Linq;

namespace SRCCore.Models
{
    // 地形データのクラス
    public class TerrainData
    {
        public static readonly TerrainData EmptyTerrain = new TerrainData();

        // 識別番号
        public int ID = -1;
        // 名称
        public string Name;
        // ビットマップ名
        public string Bitmap;
        // 地形タイプ
        public string Class;
        // 移動コスト
        public int MoveCost;
        // 命中修正
        public int HitMod;
        // ダメージ修正
        public int DamageMod;

        // 地形効果
        public SrcCollection<FeatureData> colFeature = new SrcCollection<FeatureData>();

        // 地形効果を追加
        public void AddFeature(string fdef)
        {
            FeatureData fd;
            string ftype, fdata = default;
            double flevel;
            int i, j;
            string buf;

            buf = fdef;

            // 地形効果の種類、レベル、データを切り出し
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

            // 地形効果を登録
            fd = new FeatureData();
            fd.Name = ftype;
            fd.Level = flevel;
            fd.StrData = fdata;
            if (IsFeatureAvailable(ftype))
            {
                colFeature.Add(fd, ftype + SrcFormatter.Format(CountFeature()));
            }
            else
            {
                colFeature.Add(fd, ftype);
            }
        }

        // 地形効果の総数
        public int CountFeature()
        {
            return colFeature.Count;
        }

        // 地形効果
        public FeatureData Feature(string Index)
        {
            return colFeature[Index];
        }

        //// 地形効果の名称
        //public string FeatureName(string Index)
        //{
        //    string FeatureNameRet = default;
        //    FeatureData fd;
        //    fd = (FeatureData)colFeature[Index];
        //    if (Strings.Len(fd.StrData) > 0)
        //    {
        //        FeatureNameRet = GeneralLib.ListIndex(fd.StrData, 1);
        //    }
        //    else if (fd.Level != Constants.DEFAULT_LEVEL)
        //    {
        //        FeatureNameRet = fd.Name + "Lv" + SrcFormatter.Format(fd.Level);
        //    }
        //    else
        //    {
        //        FeatureNameRet = fd.Name;
        //    }

        //    return FeatureNameRet;
        //}

        //// 地形効果のレベル
        //public double FeatureLevel(string Index)
        //{
        //    double FeatureLevelRet = default;
        //    FeatureData fd;
        //    fd = (FeatureData)colFeature[Index];
        //    FeatureLevelRet = fd.Level;
        //    if (FeatureLevelRet == Constants.DEFAULT_LEVEL)
        //    {
        //        FeatureLevelRet = 1d;
        //    }

        //    return FeatureLevelRet;
        //    ErrorHandler:
        //    ;
        //    FeatureLevelRet = 0d;
        //}

        //// 地形効果のデータ
        //public string FeatureData(string Index)
        //{
        //    string FeatureDataRet = default;
        //    FeatureData fd;
        //    fd = (FeatureData)colFeature[Index];
        //    FeatureDataRet = fd.StrData;
        //    return FeatureDataRet;
        //    ErrorHandler:
        //    ;
        //    FeatureDataRet = "";
        //}

        // 指定した地形効果を持っているか？
        public bool IsFeatureAvailable(string fname)
        {
            return colFeature.Values.Any(x => (x.Name ?? "") == (fname ?? ""));
        }

        //// 指定した地形効果はレベル指定がされているか？
        //public bool IsFeatureLevelSpecified(string Index)
        //{
        //    bool IsFeatureLevelSpecifiedRet = default;
        //    FeatureData fd;
        //    fd = (FeatureData)colFeature[Index];
        //    if (fd.Level == Constants.DEFAULT_LEVEL)
        //    {
        //        IsFeatureLevelSpecifiedRet = false;
        //    }
        //    else
        //    {
        //        IsFeatureLevelSpecifiedRet = true;
        //    }

        //    return IsFeatureLevelSpecifiedRet;
        //    ErrorHandler:
        //    ;
        //    IsFeatureLevelSpecifiedRet = false;
        //}

        //// データをクリア
        //public void Clear()
        //{

        //    int i;
        //    ID = -1;
        //    if (colFeature is object)
        //    {
        //        {
        //            var withBlock = colFeature;
        //            var loopTo = withBlock.Count;
        //            for (i = 1; i <= loopTo; i++)
        //                withBlock.Remove(1);
        //        }
        //        // UPGRADE_NOTE: オブジェクト colFeature をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
        //        colFeature = null;
        //    }
        //}
    }
}
