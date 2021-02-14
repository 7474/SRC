// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.VB;
using System.Collections.Generic;
using System.IO;

namespace SRCCore.Models
{
    // 全地形データを管理するリストのクラス
    public class TerrainDataList
    {
        // 地形データの配列
        // 他のリスト管理用クラスと異なり配列を使っているのはアクセスを高速化するため
        private TerrainData[] TerrainList = new TerrainData[(Map.MAX_TERRAIN_DATA_NUM + 1)];

        // 指定したデータは登録されているか？
        public bool IsDefined(int ID)
        {
            return Item(ID) != null;
        }

        // 地形データリストから指定したデータを取り出す
        public TerrainData Item(int ID)
        {
            return TerrainList[ID];
        }

        //// 指定したデータの名称
        //public string Name(int ID)
        //{
        //    string NameRet = default;
        //    NameRet = TerrainDataList_Renamed[ID].Name;
        //    return NameRet;
        //}

        //// 指定したデータの画像ファイル名
        //public string Bitmap(int ID)
        //{
        //    string BitmapRet = default;
        //    BitmapRet = TerrainDataList_Renamed[ID].Bitmap_Renamed;
        //    return BitmapRet;
        //}

        //// 指定したデータのクラス
        //// UPGRADE_NOTE: Class は Class_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        //public string Class_Renamed(int ID)
        //{
        //    string Class_RenamedRet = default;
        //    Class_RenamedRet = TerrainDataList_Renamed[ID].Class_Renamed;
        //    return Class_RenamedRet;
        //}

        //// 指定したデータの移動コスト
        //public int MoveCost(int ID)
        //{
        //    int MoveCostRet = default;
        //    MoveCostRet = TerrainDataList_Renamed[ID].MoveCost;
        //    return MoveCostRet;
        //}

        //// 指定したデータの命中修正
        //public int HitMod(int ID)
        //{
        //    int HitModRet = default;
        //    HitModRet = TerrainDataList_Renamed[ID].HitMod;
        //    return HitModRet;
        //}

        //// 指定したデータのダメージ修正
        //public int DamageMod(int ID)
        //{
        //    int DamageModRet = default;
        //    DamageModRet = TerrainDataList_Renamed[ID].DamageMod;
        //    return DamageModRet;
        //}


        //// 指定したデータの特殊能力

        //public bool IsFeatureAvailable(int ID, string ftype)
        //{
        //    bool IsFeatureAvailableRet = default;
        //    IsFeatureAvailableRet = TerrainDataList_Renamed[ID].IsFeatureAvailable(ftype);
        //    return IsFeatureAvailableRet;
        //}

        //public double FeatureLevel(int ID, string ftype)
        //{
        //    double FeatureLevelRet = default;
        //    object argIndex1 = ftype;
        //    FeatureLevelRet = TerrainDataList_Renamed[ID].FeatureLevel(argIndex1);
        //    return FeatureLevelRet;
        //}

        //public string FeatureData(int ID, string ftype)
        //{
        //    string FeatureDataRet = default;
        //    object argIndex1 = ftype;
        //    FeatureDataRet = TerrainDataList_Renamed[ID].FeatureData(argIndex1);
        //    return FeatureDataRet;
        //}

        //// Ｎ番目に登録したデータの番号
        //public int OrderedID(int n)
        //{
        //    int OrderedIDRet = default;
        //    OrderedIDRet = OrderList[n];
        //    return OrderedIDRet;
        //}

        // データファイル fname からデータをロード
        public void Load(string fname)
        {
            using (var stream = new FileStream(fname, FileMode.Open))
            {
                Load(fname, stream);
            }
        }
        public void Load(string fname, Stream stream)
        {
            // TODO Log
            var continuesErrors = new List<InvalidSrcData>();

            using (var reader = new SrcDataReader(fname, stream))
            {
                bool in_quote;
                string line_buf;
                while (reader.HasMore)
                {
                    line_buf = reader.GetLine();
                    if (string.IsNullOrEmpty(line_buf))
                    {
                        continue;
                    }

                    // 番号
                    int data_id;
                    if (Information.IsNumeric(line_buf))
                    {
                        data_id = Conversions.ToInteger(line_buf);
                    }
                    else
                    {
                        throw reader.InvalidDataException("番号の設定が間違っています。", line_buf);
                    }

                    if (data_id < 0 | data_id >= Map.MAX_TERRAIN_DATA_NUM)
                    {
                        throw reader.InvalidDataException("番号の設定が間違っています。", line_buf);
                    }

                    TerrainData td = new TerrainData
                    {
                        ID = data_id,
                    };

                    // 名称, 画像ファイル名
                    line_buf = reader.GetLine();

                    // 名称
                    var ret = Strings.InStr(line_buf, ",");
                    if (ret == 0)
                    {
                        throw reader.InvalidDataException("画像ファイル名が抜けています。", line_buf);
                    }

                    var data_name = Strings.Trim(Strings.Left(line_buf, ret - 1));
                    td.Name = data_name;
                    var buf = Strings.Mid(line_buf, ret + 1);

                    // 画像ファイル名
                    td.Bitmap = Strings.Trim(buf);
                    if (Strings.Len(td.Bitmap) == 0)
                    {
                        throw reader.InvalidDataException("画像ファイル名が指定されていません。", data_name);
                    }

                    // 地形タイプ, 移動コスト, 命中修正, ダメージ修正
                    line_buf = reader.GetLine();

                    // 地形タイプ
                    ret = Strings.InStr(line_buf, ",");
                    if (ret == 0)
                    {
                        throw reader.InvalidDataException("移動コストが抜けています。", data_name);
                    }

                    var buf2 = Strings.Trim(Strings.Left(line_buf, ret - 1));
                    buf = Strings.Mid(line_buf, ret + 1);
                    td.Class = buf2;

                    // 移動コスト
                    ret = Strings.InStr(buf, ",");
                    if (ret == 0)
                    {
                        throw reader.InvalidDataException("命中修正が抜けています。", data_name);
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
                    if (buf2 == "-")
                    {
                        td.MoveCost = 1000;
                    }
                    else if (Information.IsNumeric(buf2))
                    {
                        // 0.5刻みの移動コストを使えるようにするため、実際の２倍の値で記録する
                        td.MoveCost = (int)(2d * Conversions.ToDouble(buf2));
                    }

                    if (td.MoveCost <= 0)
                    {
                        continuesErrors.Add(reader.InvalidData("移動コストの設定が間違っています。", data_name));
                    }

                    // 命中修正
                    ret = Strings.InStr(buf, ",");
                    if (ret == 0)
                    {
                        throw reader.InvalidDataException("ダメージ修正が抜けています。", data_name);
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
                    if (Information.IsNumeric(buf2))
                    {
                        td.HitMod = Conversions.ToInteger(buf2);
                    }
                    else
                    {
                        continuesErrors.Add(reader.InvalidData("命中修正の設定が間違っています。", data_name));
                    }

                    // ダメージ修正
                    ret = Strings.InStr(buf, ",");
                    if (ret > 0)
                    {
                        throw reader.InvalidDataException("余分な「,」が指定されています。", data_name);
                    }

                    buf2 = Strings.Trim(buf);
                    if (Information.IsNumeric(buf2))
                    {
                        td.DamageMod = Conversions.ToInteger(buf2);
                    }
                    else
                    {
                        continuesErrors.Add(reader.InvalidData("ダメージ修正の設定が間違っています。", data_name));
                    }

                    // 地形効果
                    line_buf = reader.GetLine();
                    while (Strings.Len(line_buf) > 0)
                    {
                        buf = line_buf;
                        var i = 0;
                        while (Strings.Len(buf) > 0)
                        {
                            i = (i + 1);
                            ret = 0;
                            in_quote = false;
                            var loopTo = Strings.Len(buf);
                            for (var j = 1; j <= loopTo; j++)
                            {
                                switch (Strings.Mid(buf, j, 1) ?? "")
                                {
                                    case ",":
                                        {
                                            if (!in_quote)
                                            {
                                                ret = j;
                                                break;
                                            }

                                            break;
                                        }

                                    case "\"":
                                        {
                                            in_quote = !in_quote;
                                            break;
                                        }
                                }
                            }

                            if (ret > 0)
                            {
                                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                                buf = Strings.Trim(Strings.Mid(buf, ret + 1));
                            }
                            else
                            {
                                buf2 = buf;
                                buf = "";
                            }

                            if (!string.IsNullOrEmpty(buf2))
                            {
                                td.AddFeature(buf2);
                            }
                            else
                            {
                                continuesErrors.Add(reader.InvalidData("行頭から" + SrcFormatter.Format((object)i) + "番目の地形効果の設定が間違っています。", data_name));
                            }
                        }
                        if (reader.HasMore)
                        {
                            line_buf = reader.GetLine();
                        }
                        else
                        {
                            break;
                        }
                    }
                    TerrainList[data_id] = td;
                }
            }
        }

        // リストをクリア
        public void Clear()
        {
            for (int i = 0; i < TerrainList.Length; i++)
            {
                TerrainList[i] = null;
            }
        }
    }
}