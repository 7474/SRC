// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.VB;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCCore.Models
{
    // 全地形データを管理するリストのクラス
    public class TerrainDataList
    {
        private SRC SRC;
        public TerrainDataList(SRC src)
        {
            SRC = src;
        }

        // 地形データの配列
        // 他のリスト管理用クラスと異なり配列を使っているのはアクセスを高速化するため
        private TerrainData[] TerrainList = new TerrainData[(Map.MAX_TERRAIN_DATA_NUM + 1)];

        // 地形データの登録順を記録するための配列
        private readonly List<int> OrderList = new List<int>();

        // 地形データの登録数
        public int Count => OrderList.Count;

        // 指定したデータは登録されているか？
        public bool IsDefined(int ID)
        {
            // 範囲外の番号は登録されていない（Item が返す EmptyTerrain は ID が -1）
            return ID >= 0 && ID < TerrainList.Length && TerrainList[ID] != null && TerrainList[ID].ID >= 0;
        }

        // Ｎ番目に登録したデータの番号
        public int OrderedID(int n)
        {
            return OrderList[n - 1];
        }

        // 地形データリストから指定したデータを取り出す
        public TerrainData Item(int ID)
        {
            return ID >= 0 && ID < TerrainList.Length
                ? TerrainList[ID]
                : TerrainData.EmptyTerrain;
        }

        /// <summary>
        /// 地形データが定義されていない番号に、仮の地形（陸・移動コスト1・修正なし）を登録する。
        /// 素材なしで実行する場合（SRC.AllowMissingAssets）に使う。
        /// </summary>
        public TerrainData DefinePlaceholder(int ID)
        {
            if (IsDefined(ID))
            {
                return Item(ID);
            }
            var td = new TerrainData
            {
                ID = ID,
                Name = "地形" + ID,
                Bitmap = "",
                Class = "陸",
                MoveCost = 1,
                HitMod = 0,
                DamageMod = 0,
            };
            TerrainList[ID] = td;
            OrderList.Add(ID);
            return td;
        }

        /// <summary>
        /// 指定したビットマップ名の地形が登録されているか（大文字・小文字を区別する）。
        /// </summary>
        public bool HasBitmap(string bitmap)
        {
            return TerrainList.Any(x => x != null && x.Bitmap == bitmap);
        }

        // 名前を指定してデータを取り出す
        // VB（ChangeTerrain・ChangeLayer など）と同じく登録した順に探す
        public TerrainData ItemByName(string name)
        {
            return OrderList.Select(x => TerrainList[x]).FirstOrDefault(x => x.Name == name);
        }

        // データファイル fname からデータをロード
        public void Load(string fname)
        {
            using (var stream = SRC.FileSystem.OpenText(SRC.Config.SRCCompatibilityMode, fname))
            {
                Load(fname, stream);
            }
        }
        public void Load(string fname, Stream stream)
        {
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
                        throw reader.InvalidDataException("番号の設定が間違っています。", "");
                    }

                    if (data_id < 0 || data_id >= Map.MAX_TERRAIN_DATA_NUM)
                    {
                        throw reader.InvalidDataException("番号の設定が間違っています。", "");
                    }

                    // VB は番号ごとの TerrainData を使い回す。登録済みなら地形効果を消して上書きする
                    // （登録順は最初に登録したときのまま）
                    TerrainData td = TerrainList[data_id];
                    if (td == null || td.ID < 0)
                    {
                        td = td ?? new TerrainData();
                        TerrainList[data_id] = td;
                        OrderList.Add(data_id);
                    }
                    else
                    {
                        td.Clear();
                    }
                    td.ID = data_id;

                    // 名称, 画像ファイル名
                    line_buf = reader.GetLine();

                    // 名称
                    var ret = Strings.InStr(line_buf, ",");
                    if (ret == 0)
                    {
                        throw reader.InvalidDataException("画像ファイル名が抜けています。", "");
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
                        td.MoveCost = VBMath.CLng(2d * Conversions.ToDouble(buf2));
                    }

                    if (td.MoveCost <= 0)
                    {
                        SRC.AddDataError(reader.InvalidData("移動コストの設定が間違っています。", data_name));
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
                        SRC.AddDataError(reader.InvalidData("命中修正の設定が間違っています。", data_name));
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
                        SRC.AddDataError(reader.InvalidData("ダメージ修正の設定が間違っています。", data_name));
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
                            // VB は引用符の外の最初の「,」で Exit For する
                            for (var j = 1; j <= loopTo && ret == 0; j++)
                            {
                                switch (Strings.Mid(buf, j, 1) ?? "")
                                {
                                    case ",":
                                        {
                                            if (!in_quote)
                                            {
                                                ret = j;
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
                                SRC.AddDataError(reader.InvalidData("行頭から" + SrcFormatter.Format((object)i) + "番目の地形効果の設定が間違っています。", data_name));
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
            OrderList.Clear();
        }
    }
}
