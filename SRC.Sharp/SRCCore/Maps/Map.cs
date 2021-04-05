// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using Microsoft.Extensions.Logging;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCCore.Maps
{
    // マップデータに関する各種処理を行うモジュール
    public class Map
    {
        protected SRC SRC { get; }
        private IGUI GUI => SRC.GUI;
        private IGUIMap GUIMap => SRC.GUIMap;
        private Commands.Command Commands => SRC.Commands;

        public Map(SRC src)
        {
            SRC = src;
        }

        // 管理可能な地形データの総数
        public const int MAX_TERRAIN_DATA_NUM = 2000;

        // マップファイル名
        public string MapFileName;
        // XXX
        public bool IsStatusView => string.IsNullOrEmpty(MapFileName);
        // マップの横サイズ
        public int MapWidth;
        // マップの縦サイズ
        public int MapHeight;

        // マップの描画モード
        public string MapDrawMode;
        // フィルタ色
        public int MapDrawFilterColor;
        // フィルタの透過度
        public double MapDrawFilterTransPercent;
        // フィルタやSepiaコマンドなどでユニットの色を変更するか
        public bool MapDrawIsMapOnly;

        // マップに画像の書き込みがなされたか
        public bool IsMapDirty;

        // マップデータを記録する配列
        public Src2DArray<MapCell> MapData;

        public Src2DArray<MapImageFileType> MapImageFileTypeData;

        // マップ上に存在するユニットを記録する配列
        public Src2DArray<Unit> MapDataForUnit;

        // マップ上でターゲットを選択する際のマスク情報
        public Src2DArray<bool> MaskData;

        // 現在地点からその地点まで移動するのに必要な移動力の配列
        public int[,] TotalMoveCost;

        // 各地点がＺＯＣの影響下にあるかどうか
        public int[,] PointInZOC;

        // 地形情報テーブルを初期化
        public void InitMap()
        {
            SetMapSize(GUI.MainWidth, GUI.MainHeight);
        }

        //// (X,Y)地点の命中修正
        //public int TerrainEffectForHit(int X, int Y)
        //{
        //    throw new NotImplementedException();
        //    //    int TerrainEffectForHitRet = default;
        //    //    // MOD START 240a
        //    //    // TerrainEffectForHit = TDList.HitMod(MapData(X, Y, 0))
        //    //    switch (MapData[X, Y, MapDataIndex.BoxType])
        //    //    {
        //    //        case BoxTypes.Under:
        //    //        case BoxTypes.UpperBmpOnly:
        //    //            {
        //    //                // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
        //    //                TerrainEffectForHitRet = SRC.TDList.HitMod(MapData[X, Y, MapDataIndex.TerrainType]);
        //    //                break;
        //    //            }

        //    //        default:
        //    //            {
        //    //                // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
        //    //                TerrainEffectForHitRet = SRC.TDList.HitMod(MapData[X, Y, MapDataIndex.LayerType]);
        //    //                break;
        //    //            }
        //    //    }

        //    //    return TerrainEffectForHitRet;
        //    //    // MOD  END  240a
        //}

        //// (X,Y)地点のダメージ修正
        //public int TerrainEffectForDamage(int X, int Y)
        //{
        //    throw new NotImplementedException();
        //    //    int TerrainEffectForDamageRet = default;
        //    //    // MOD START 240a
        //    //    // TerrainEffectForDamage = TDList.DamageMod(MapData(X, Y, 0))
        //    //    switch (MapData[X, Y, MapDataIndex.BoxType])
        //    //    {
        //    //        case BoxTypes.Under:
        //    //        case BoxTypes.UpperBmpOnly:
        //    //            {
        //    //                // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
        //    //                TerrainEffectForDamageRet = SRC.TDList.DamageMod(MapData[X, Y, MapDataIndex.TerrainType]);
        //    //                break;
        //    //            }

        //    //        default:
        //    //            {
        //    //                // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
        //    //                TerrainEffectForDamageRet = SRC.TDList.DamageMod(MapData[X, Y, MapDataIndex.LayerType]);
        //    //                break;
        //    //            }
        //    //    }

        //    //    return TerrainEffectForDamageRet;
        //    //    // MOD  END  240a
        //}

        //// (X,Y)地点のＨＰ回復率
        //public int TerrainEffectForHPRecover(int X, int Y)
        //{
        //    throw new NotImplementedException();
        //    //int TerrainEffectForHPRecoverRet = default;
        //    //// MOD START 240a
        //    //// TerrainEffectForHPRecover = 10 * TDList.FeatureLevel(MapData(X, Y, 0), "ＨＰ回復")
        //    //switch (MapData[X, Y, MapDataIndex.BoxType])
        //    //{
        //    //    case BoxTypes.Under:
        //    //    case BoxTypes.UpperBmpOnly:
        //    //        {
        //    //            // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
        //    //            TerrainEffectForHPRecoverRet = (10d * SRC.TDList.FeatureLevel(MapData[X, Y, MapDataIndex.TerrainType], "ＨＰ回復"));
        //    //            break;
        //    //        }

        //    //    default:
        //    //        {
        //    //            // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
        //    //            TerrainEffectForHPRecoverRet = (10d * SRC.TDList.FeatureLevel(MapData[X, Y, MapDataIndex.LayerType], "ＨＰ回復"));
        //    //            break;
        //    //        }
        //    //}

        //    //return TerrainEffectForHPRecoverRet;
        //    //// MOD  END  240a
        //}

        //// (X,Y)地点のＥＮ回復率
        //public int TerrainEffectForENRecover(int X, int Y)
        //{
        //    throw new NotImplementedException();
        //    //int TerrainEffectForENRecoverRet = default;
        //    //// MOD START 240a
        //    //// TerrainEffectForENRecover = 10 * TDList.FeatureLevel(MapData(X, Y, 0), "ＥＮ回復")
        //    //switch (MapData[X, Y, MapDataIndex.BoxType])
        //    //{
        //    //    case BoxTypes.Under:
        //    //    case BoxTypes.UpperBmpOnly:
        //    //        {
        //    //            // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
        //    //            TerrainEffectForENRecoverRet = (10d * SRC.TDList.FeatureLevel(MapData[X, Y, MapDataIndex.TerrainType], "ＥＮ回復"));
        //    //            break;
        //    //        }

        //    //    default:
        //    //        {
        //    //            // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
        //    //            TerrainEffectForENRecoverRet = (10d * SRC.TDList.FeatureLevel(MapData[X, Y, MapDataIndex.LayerType], "ＥＮ回復"));
        //    //            break;
        //    //        }
        //    //}

        //    //return TerrainEffectForENRecoverRet;
        //    //// MOD  END  240a
        //}

        //// (X,Y)地点の地形名称
        //public string TerrainName(int X, int Y)
        //{
        //    throw new NotImplementedException();
        //    //string TerrainNameRet = default;
        //    //// MOD START 240a
        //    //// TerrainName = TDList.Name(MapData(X, Y, 0))
        //    //switch (MapData[X, Y, MapDataIndex.BoxType])
        //    //{
        //    //    case BoxTypes.Under:
        //    //    case BoxTypes.UpperBmpOnly:
        //    //        {
        //    //            // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
        //    //            TerrainNameRet = SRC.TDList.Name(MapData[X, Y, MapDataIndex.TerrainType]);
        //    //            break;
        //    //        }

        //    //    default:
        //    //        {
        //    //            // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
        //    //            TerrainNameRet = SRC.TDList.Name(MapData[X, Y, MapDataIndex.LayerType]);
        //    //            break;
        //    //        }
        //    //}

        //    //return TerrainNameRet;
        //    //// MOD  END  240a
        //}

        //// (X,Y)地点の地形クラス
        //public string TerrainClass(int X, int Y)
        //{
        //    throw new NotImplementedException();
        //    //string TerrainClassRet = default;
        //    //// MOD START 240a
        //    //// TerrainClass = TDList.Class(MapData(X, Y, 0))
        //    //switch (MapData[X, Y, MapDataIndex.BoxType])
        //    //{
        //    //    case BoxTypes.Under:
        //    //    case BoxTypes.UpperBmpOnly:
        //    //        {
        //    //            // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
        //    //            TerrainClassRet = SRC.TDList.Class_Renamed(MapData[X, Y, MapDataIndex.TerrainType]);
        //    //            break;
        //    //        }

        //    //    default:
        //    //        {
        //    //            // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
        //    //            TerrainClassRet = SRC.TDList.Class_Renamed(MapData[X, Y, MapDataIndex.LayerType]);
        //    //            break;
        //    //        }
        //    //}

        //    //return TerrainClassRet;
        //    //// MOD  END  240a
        //}

        //// (X,Y)地点の移動コスト
        //public int TerrainMoveCost(int X, int Y)
        //{
        //    throw new NotImplementedException();
        //    //int TerrainMoveCostRet = default;
        //    //// MOD START 240a
        //    //// TerrainMoveCost = TDList.MoveCost(MapData(X, Y, 0))
        //    //switch (MapData[X, Y, MapDataIndex.BoxType])
        //    //{
        //    //    case BoxTypes.Under:
        //    //    case BoxTypes.UpperBmpOnly:
        //    //        {
        //    //            // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
        //    //            TerrainMoveCostRet = SRC.TDList.MoveCost(MapData[X, Y, MapDataIndex.TerrainType]);
        //    //            break;
        //    //        }

        //    //    default:
        //    //        {
        //    //            // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
        //    //            TerrainMoveCostRet = SRC.TDList.MoveCost(MapData[X, Y, MapDataIndex.LayerType]);
        //    //            break;
        //    //        }
        //    //}

        //    //return TerrainMoveCostRet;
        //    //// MOD  END  240a
        //}

        //// (X,Y)地点に障害物があるか (吹き飛ばし時に衝突するか)
        //public bool TerrainHasObstacle(int X, int Y)
        //{
        //    throw new NotImplementedException();
        //    //bool TerrainHasObstacleRet = default;
        //    //// MOD START 240a
        //    //// TerrainHasObstacle = TDList.IsFeatureAvailable(MapData(X, Y, 0), "衝突")
        //    //switch (MapData[X, Y, MapDataIndex.BoxType])
        //    //{
        //    //    case BoxTypes.Under:
        //    //    case BoxTypes.UpperBmpOnly:
        //    //        {
        //    //            // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
        //    //            TerrainHasObstacleRet = SRC.TDList.IsFeatureAvailable(MapData[X, Y, MapDataIndex.TerrainType], "衝突");
        //    //            break;
        //    //        }

        //    //    default:
        //    //        {
        //    //            // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
        //    //            TerrainHasObstacleRet = SRC.TDList.IsFeatureAvailable(MapData[X, Y, MapDataIndex.LayerType], "衝突");
        //    //            break;
        //    //        }
        //    //}

        //    //return TerrainHasObstacleRet;
        //    //// MOD  END  240a
        //}

        //// ADD START 240a
        //// (X,Y)地点が移動停止か
        //public bool TerrainHasMoveStop(int X, int Y)
        //{
        //    throw new NotImplementedException();
        //    //bool TerrainHasMoveStopRet = default;
        //    //switch (MapData[X, Y, MapDataIndex.BoxType])
        //    //{
        //    //    case BoxTypes.Under:
        //    //    case BoxTypes.UpperBmpOnly:
        //    //        {
        //    //            // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
        //    //            TerrainHasMoveStopRet = SRC.TDList.IsFeatureAvailable(MapData[X, Y, MapDataIndex.TerrainType], "移動停止");
        //    //            break;
        //    //        }

        //    //    default:
        //    //        {
        //    //            // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
        //    //            TerrainHasMoveStopRet = SRC.TDList.IsFeatureAvailable(MapData[X, Y, MapDataIndex.LayerType], "移動停止");
        //    //            break;
        //    //        }
        //    //}

        //    //return TerrainHasMoveStopRet;
        //}

        //// (X,Y)地点が進入禁止か
        //public bool TerrainDoNotEnter(int X, int Y)
        //{
        //    throw new NotImplementedException();
        //    //bool ret;
        //    //switch (MapData[X, Y, MapDataIndex.BoxType])
        //    //{
        //    //    case BoxTypes.Under:
        //    //    case BoxTypes.UpperBmpOnly:
        //    //        {
        //    //            // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
        //    //            ret = SRC.TDList.IsFeatureAvailable(MapData[X, Y, MapDataIndex.TerrainType], "進入禁止");
        //    //            if (!ret)
        //    //            {
        //    //                // 互換性維持のため残している
        //    //                ret = SRC.TDList.IsFeatureAvailable(MapData[X, Y, MapDataIndex.TerrainType], "侵入禁止");
        //    //            }

        //    //            break;
        //    //        }

        //    //    default:
        //    //        {
        //    //            // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
        //    //            ret = SRC.TDList.IsFeatureAvailable(MapData[X, Y, MapDataIndex.LayerType], "進入禁止");
        //    //            if (!ret)
        //    //            {
        //    //                // 互換性維持のため残している
        //    //                ret = SRC.TDList.IsFeatureAvailable(MapData[X, Y, MapDataIndex.LayerType], "侵入禁止");
        //    //            }

        //    //            break;
        //    //        }
        //    //}

        //    //return default;
        //}

        //// (X,Y)地点が指定した能力を持っているか
        //public bool TerrainHasFeature(int X, int Y, string Feature)
        //{
        //    throw new NotImplementedException();
        //    //bool TerrainHasFeatureRet = default;
        //    //switch (MapData[X, Y, MapDataIndex.BoxType])
        //    //{
        //    //    case BoxTypes.Under:
        //    //    case BoxTypes.UpperBmpOnly:
        //    //        {
        //    //            // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
        //    //            TerrainHasFeatureRet = SRC.TDList.IsFeatureAvailable(MapData[X, Y, MapDataIndex.TerrainType], Feature);
        //    //            break;
        //    //        }

        //    //    default:
        //    //        {
        //    //            // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
        //    //            TerrainHasFeatureRet = SRC.TDList.IsFeatureAvailable(MapData[X, Y, MapDataIndex.LayerType], Feature);
        //    //            break;
        //    //        }
        //    //}

        //    //return TerrainHasFeatureRet;
        //}
        //// ADD  END  240a

        // (X,Y)地点の地形
        public TerrainData Terrain(int X, int Y)
        {
            return CellAtPoint(X, Y)?.Terrain;
        }

        public bool IsInside(int x, int y)
        {
            return x >= 1
                && MapWidth >= x
                && y >= 1
                && MapHeight >= y;
        }

        public MapCell CellAtPoint(int x, int y)
        {
            return IsInside(x, y) ? MapData[x, y] : null;
        }

        // (X,Y)地点にいるユニット
        public Unit UnitAtPoint(int X, int Y)
        {
            return IsInside(X, Y) ? MapDataForUnit[X, Y] : null;
        }

        // 指定したマップ画像を検索する
        // XXX レイヤ対応はしてない
        public string SearchTerrainImageFile(MapCell cell)
        {
            //// ADD START 240a
            //// 画像無が確定してるなら処理しない
            //if (tid == NO_LAYER_NUM)
            //{
            //    return SearchTerrainImageFileRet;
            //}
            //else if (tbitmap == NO_LAYER_NUM)
            //{
            //    return SearchTerrainImageFileRet;
            //}
            //// ADD  END  240a

            //// 初めて実行する際に、各フォルダにBitmap\Mapフォルダがあるかチェック
            //if (!init_setup_background)
            //{
            //    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //    if (Strings.Len(FileSystem.Dir(SRC.ScenarioPath + @"Bitmap\Map", FileAttribute.Directory)) > 0)
            //    {
            //        scenario_map_dir_exists = true;
            //    }
            //    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //    if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath + @"Bitmap\Map", FileAttribute.Directory)) > 0)
            //    {
            //        extdata_map_dir_exists = true;
            //    }
            //    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //    if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath2 + @"Bitmap\Map", FileAttribute.Directory)) > 0)
            //    {
            //        extdata2_map_dir_exists = true;
            //    }

            //    init_setup_background = true;
            //}

            // マップ画像のファイル名を作成
            var tbmpname = cell.UnderTerrain.Bitmap;
            var tbitmap = cell.BitmapNo;
            var fnames = new string[]
            {
                SRC.FileSystem.PathCombine("Bitmap", "Map", tbmpname, string.Format("{0}{1:0000}.bmp", tbmpname, tbitmap)),
                SRC.FileSystem.PathCombine("Bitmap", "Map", string.Format("{0}{1:0000}.bmp", tbmpname, tbitmap)),
                SRC.FileSystem.PathCombine("Bitmap", "Map", string.Format("{0}{1}.bmp", tbmpname, tbitmap)),
            };
            var dnames = new string[]
            {
                SRC.ScenarioPath,
                SRC.ExtDataPath,
                SRC.ExtDataPath2,
                SRC.AppPath,
            };
            // ビットマップを探す
            foreach (var dir in dnames)
            {
                //if (scenario_map_dir_exists)
                //if (extdata_map_dir_exists)
                //if (extdata2_map_dir_exists)
                if (Directory.Exists(dir))
                {
                    foreach (var file in fnames)
                    {
                        var fpath = SRC.FileSystem.PathCombine(dir, file);
                        if (File.Exists(fpath))
                        {
                            // TODO ビットマップ名記録
                            //MapImageFileTypeData[tx, ty] = MapImageFileType.SeparateDirMapImageFileType;
                            return fpath;
                        }
                    }
                }
            }
            return null;
        }

        // マップファイル fname のデータをロード
        public void LoadMapData(string fname)
        {
            try
            {
                using (var stream = SRC.FileSystem.Open(fname))
                {
                    LoadMapData(fname, stream);
                }
            }
            catch (FileNotFoundException ex)
            {
                SRC.Log.LogError(ex.Message, ex);
                // TODO Impl
                //    // ファイルが存在しない場合
                //    if (string.IsNullOrEmpty(fname) | !GeneralLib.FileExists(fname))
                //    {
                //        MapFileName = "";
                //        if (Strings.InStr(SRC.ScenarioFileName, "ステータス表示.") > 0 | Strings.InStr(SRC.ScenarioFileName, "ランキング.") > 0)
                //        {
                //            SetMapSize(GUI.MainWidth, 40);
                //        }
                //        else
                //        {
                //            SetMapSize(GUI.MainWidth, GUI.MainHeight);
                //        }

                //        var loopTo = MapWidth;
                //        for (i = 1; i <= loopTo; i++)
                //        {
                //            var loopTo1 = MapHeight;
                //            for (j = 1; j <= loopTo1; j++)
                //            {
                //                // MOD START 240a
                //                // MapData(i, j, 0) = MAX_TERRAIN_DATA_NUM
                //                // MapData(i, j, 1) = 0
                //                // ファイルが無い場合
                //                MapData[i, j, MapDataIndex.TerrainType] = MAX_TERRAIN_DATA_NUM;
                //                MapData[i, j, MapDataIndex.BitmapNo] = 0;
                //                MapData[i, j, MapDataIndex.LayerType] = NO_LAYER_NUM;
                //                MapData[i, j, MapDataIndex.LayerBitmapNo] = NO_LAYER_NUM;
                //                MapData[i, j, MapDataIndex.BoxType] = BoxTypes.Under;
                //                // MOD  END  240a
                //            }
                //        }

                //        return;
                //    };
            }
        }
        public void LoadMapData(string fname, Stream stream)
        {
            // ファイル名を記録しておく
            MapFileName = fname;
            try
            {
                using (var reader = new SrcDataReader(fname, stream))
                {
                    string buf;

                    // ファイルの先頭にあるマップサイズ情報を収得
                    var lineReaded = false;
                    buf = reader.GetLine().Trim('"', '\'');
                    if (buf != "MapData")
                    {
                        // 旧形式のマップデータ
                        SetMapSize(20, 20);
                        lineReaded = true;
                    }
                    else
                    {
                        // 2行目は予約行
                        buf = reader.GetLine();
                        // 3行目がマップサイズ
                        buf = reader.GetLine();
                        var mapWH = buf.Split(',');
                        SetMapSize(int.Parse(mapWH[0]), int.Parse(mapWH[1]));
                    }

                    // マップデータを読み込み
                    for (var x = 1; x <= MapWidth; x++)
                    {
                        for (var y = 1; y <= MapHeight; y++)
                        {
                            if (lineReaded)
                            {
                                lineReaded = false;
                            }
                            else
                            {
                                buf = reader.GetLine();
                            }
                            var cell = MapData[x, y];
                            var cellTrrainBitmapNo = buf.Split(',');
                            cell.TerrainType = int.Parse(cellTrrainBitmapNo[0]);
                            cell.BitmapNo = int.Parse(cellTrrainBitmapNo[1]);
                            if (SRC.TDList.IsDefined(cell.TerrainType))
                            {
                                cell.UnderTerrain = SRC.TDList.Item(cell.TerrainType);
                            }
                            else
                            {
                                throw reader.InvalidDataException("定義されていない" + cell.TerrainType + "番の地形データが使われています", fname);
                            }
                        }
                    }

                    // TODO Impl
                    //// ADD START 240a
                    //// レイヤーデータ読み込み
                    //if (!FileSystem.EOF(FileNumber))
                    //{
                    //    FileSystem.Input(FileNumber, buf);
                    //    if (buf == "Layer")
                    //    {
                    //        var loopTo4 = MapWidth;
                    //        for (i = 1; i <= loopTo4; i++)
                    //        {
                    //            var loopTo5 = MapHeight;
                    //            for (j = 1; j <= loopTo5; j++)
                    //            {
                    //                Input(FileNumber, MapData[i, j, MapDataIndex.LayerType]);
                    //                Input(FileNumber, MapData[i, j, MapDataIndex.LayerBitmapNo]);
                    //                if (MapData[i, j, MapDataIndex.LayerType] != NO_LAYER_NUM)
                    //                {
                    //                    // 定義されていたらデータの妥当性チェック
                    //                    if (!SRC.TDList.IsDefined(MapData[i, j, MapDataIndex.LayerType]))
                    //                    {
                    //                        Interaction.MsgBox("定義されていない" + SrcFormatter.Format(MapData[i, j, MapDataIndex.LayerType]) + "番の地形データが使われています");
                    //                        FileSystem.FileClose(FileNumber);
                    //                        Environment.Exit(0);
                    //                    }
                    //                    // マスのタイプを上層に
                    //                    MapData[i, j, MapDataIndex.BoxType] = BoxTypes.Upper;
                    //                }
                    //                else
                    //                {
                    //                    // マスのタイプを下層に（初期化時下層だが、再度明示的に設定する）
                    //                    MapData[i, j, MapDataIndex.BoxType] = BoxTypes.Under;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    //// ADD  END  240a
                }
            }
            catch (Exception ex)
            {
                throw new TerminateException("マップファイル「" + fname + "」のデータが不正です", ex);
            }
        }

        // マップサイズを設定
        public void SetMapSize(int w, int h)
        {
            MapWidth = w;
            MapHeight = h;

            //// マップデータ用配列の領域確保
            //// マップデータ配列の初期化
            MapData = new Src2DArray<MapCell>(MapWidth, MapHeight);
            foreach (var x in MapData.XRange)
            {
                foreach (var y in MapData.YRange)
                {
                    MapData[x, y] = new MapCell
                    {
                        X = x,
                        Y = y,
                    };
                }
            }
            MapDataForUnit = new Src2DArray<Unit>(MapWidth, MapHeight);
            MaskData = new Src2DArray<bool>(MapWidth, MapHeight);
            TotalMoveCost = new int[MapWidth + 2, MapHeight + 2];
            PointInZOC = new int[MapWidth + 2, MapHeight + 2];

            // XXX MapのGUIインタフェースと実装に追い出す
            //GUI.MapPWidth = (32 * w);
            //GUI.MapPHeight = (32 * h);
            GUI.MapX = ((GUI.MainWidth + 1) / 2);
            GUI.MapY = ((GUI.MainHeight + 1) / 2);
            GUIMap.SetMapSize(w, h);
        }

        // マップデータをクリア
        public void ClearMap()
        {
            throw new NotImplementedException();
            //int ret;
            //MapFileName = "";
            //MapWidth = 1;
            //MapHeight = 1;

            //// MOD START 240a
            //// ReDim MapData(1, 1, 1)
            //MapData = new int[2, 2, 5];
            //// MOD  END  240a
            //MapDataForUnit = new Unit[2, 2];
            //MaskData = new bool[2, 2];

            //// MOD START 240a
            //// MapData(1, 1, 0) = 0
            //// MapData(1, 1, 1) = 0
            //MapData[1, 1, MapDataIndex.TerrainType] = 0;
            //MapData[1, 1, MapDataIndex.BitmapNo] = 0;
            //MapData[1, 1, MapDataIndex.LayerType] = 0;
            //MapData[1, 1, MapDataIndex.LayerBitmapNo] = 0;
            //MapData[1, 1, MapDataIndex.BoxType] = 0;
            //// MOD  END  240a
            //// UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //MapDataForUnit[1, 1] = null;

            //// マップ画面を消去
            //{
            //    var withBlock = GUI.MainForm.picBack;
            //    ret = GUI.PatBlt(withBlock.hDC, 0, 0, withBlock.width, withBlock.Height, GUI.BLACKNESS);
            //}
            //{
            //    var withBlock1 = GUI.MainForm.picMain(0);
            //    ret = GUI.PatBlt(withBlock1.hDC, 0, 0, withBlock1.width, withBlock1.Height, GUI.BLACKNESS);
            //}

            //// ユニット画像をリセット
            //if (!string.IsNullOrEmpty(MapDrawMode) & !MapDrawIsMapOnly)
            //{
            //    SRC.UList.ClearUnitBitmap();
            //}

            //MapDrawMode = "";
            //MapDrawIsMapOnly = false;
            //MapDrawFilterColor = 0;
            //MapDrawFilterTransPercent = 0d;
        }

        // 中断用セーブデータにマップデータをセーブ
        public void DumpMapData()
        {
            throw new NotImplementedException();
            //int i, j;
            //string fname;
            //if (Strings.InStr(MapFileName, SRC.ScenarioPath) == 1)
            //{
            //    fname = Strings.Right(MapFileName, Strings.Len(MapFileName) - Strings.Len(SRC.ScenarioPath));
            //}
            //else
            //{
            //    fname = MapFileName;
            //}

            //if (MapDrawIsMapOnly)
            //{
            //    FileSystem.WriteLine(SRC.SaveDataFileNumber, fname, MapDrawMode + "(マップ限定)");
            //}
            //else
            //{
            //    FileSystem.WriteLine(SRC.SaveDataFileNumber, fname, MapDrawMode);
            //}

            //FileSystem.WriteLine(SRC.SaveDataFileNumber, MapWidth, MapHeight);
            //var loopTo = MapWidth;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (j = 1; j <= loopTo1; j++)
            //        // MOD START 240a
            //        // Write #SaveDataFileNumber, MapData(i, j, 0), MapData(i, j, 1)
            //        // ADD  END  240a
            //        FileSystem.WriteLine(SRC.SaveDataFileNumber, MapData[i, j, MapDataIndex.TerrainType], MapData[i, j, MapDataIndex.BitmapNo]);
            //}

            //FileSystem.WriteLine(SRC.SaveDataFileNumber, GUI.MapX, GUI.MapY);

            //// ADD START 240a
            //// レイヤ情報を書き込む
            //FileSystem.WriteLine(SRC.SaveDataFileNumber, "Layer");
            //var loopTo2 = MapWidth;
            //for (i = 1; i <= loopTo2; i++)
            //{
            //    var loopTo3 = MapHeight;
            //    for (j = 1; j <= loopTo3; j++)
            //        FileSystem.WriteLine(SRC.SaveDataFileNumber, MapData[i, j, MapDataIndex.LayerType], MapData[i, j, MapDataIndex.LayerBitmapNo], MapData[i, j, MapDataIndex.BoxType]);
            //}
            //// ADD  END  240a

        }

        // 中断用セーブデータからマップデータをロード
        // MOD START 240a
        // Sub→Functionに
        // Public Sub RestoreMapData() As String
        public string RestoreMapData()
        {
            throw new NotImplementedException();
            //string RestoreMapDataRet = default;
            //// MOD  END  240a
            //string sbuf1 = default, sbuf2 = default;
            //int ibuf1 = default, ibuf2 = default;
            //// ADD START 240a
            //int ibuf3 = default, ibuf4;
            //var buf = default(string);
            //// ADD  END  240a
            //int i, j;
            //var is_map_changed = default(bool);

            //// マップファイル名, マップ描画方法
            //FileSystem.Input(SRC.SaveDataFileNumber, sbuf1);
            //FileSystem.Input(SRC.SaveDataFileNumber, sbuf2);
            //if (Strings.InStr(sbuf1, ":") == 0)
            //{
            //    sbuf1 = SRC.ScenarioPath + sbuf1;
            //}

            //if ((sbuf1 ?? "") != (MapFileName ?? ""))
            //{
            //    MapFileName = sbuf1;
            //    is_map_changed = true;
            //}

            //if (MapDrawIsMapOnly)
            //{
            //    if ((sbuf2 ?? "") != (MapDrawMode + "(マップ限定)" ?? ""))
            //    {
            //        if (Strings.Right(sbuf2, 7) == "(マップ限定)")
            //        {
            //            MapDrawMode = Strings.Left(sbuf2, Strings.Len(sbuf2) - 7);
            //            MapDrawIsMapOnly = true;
            //        }
            //        else
            //        {
            //            MapDrawMode = sbuf2;
            //            MapDrawIsMapOnly = false;
            //        }

            //        SRC.UList.ClearUnitBitmap();
            //        is_map_changed = true;
            //    }
            //}
            //else if ((sbuf2 ?? "") != (MapDrawMode ?? ""))
            //{
            //    if (Strings.Right(sbuf2, 7) == "(マップ限定)")
            //    {
            //        MapDrawMode = Strings.Left(sbuf2, Strings.Len(sbuf2) - 7);
            //        MapDrawIsMapOnly = true;
            //    }
            //    else
            //    {
            //        MapDrawMode = sbuf2;
            //        MapDrawIsMapOnly = false;
            //    }

            //    SRC.UList.ClearUnitBitmap();
            //    is_map_changed = true;
            //}

            //// マップ幅, マップ高さ
            //FileSystem.Input(SRC.SaveDataFileNumber, ibuf1);
            //FileSystem.Input(SRC.SaveDataFileNumber, ibuf2);
            //if (ibuf1 != MapWidth | ibuf2 != MapHeight)
            //{
            //    SetMapSize(ibuf1, ibuf2);
            //}

            //// 各地形
            //var loopTo = MapWidth;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (j = 1; j <= loopTo1; j++)
            //    {
            //        FileSystem.Input(SRC.SaveDataFileNumber, ibuf1);
            //        FileSystem.Input(SRC.SaveDataFileNumber, ibuf2);
            //        // MOD START 240a
            //        // If ibuf1 <> MapData(i, j, 0) Then
            //        // MapData(i, j, 0) = ibuf1
            //        // is_map_changed = True
            //        // End If
            //        // If ibuf2 <> MapData(i, j, 1) Then
            //        // MapData(i, j, 1) = ibuf2
            //        // is_map_changed = True
            //        // End If
            //        if (ibuf1 != MapData[i, j, MapDataIndex.TerrainType])
            //        {
            //            MapData[i, j, MapDataIndex.TerrainType] = ibuf1;
            //            is_map_changed = true;
            //        }

            //        if (ibuf2 != MapData[i, j, MapDataIndex.BitmapNo])
            //        {
            //            MapData[i, j, MapDataIndex.BitmapNo] = ibuf2;
            //            is_map_changed = true;
            //        }
            //        // MOD  END  240a
            //    }
            //}

            //// MOV START 240a
            //// '背景書き換えの必要有り？
            //// If is_map_changed Then
            //// IsMapDirty = True
            //// End If
            //// MOV  END  240a

            //// 表示位置
            //// SetupBackgroundでMapX,MapYが書き換えられてしまうため、この位置で
            //// 値を参照する必要がある。
            //FileSystem.Input(SRC.SaveDataFileNumber, GUI.MapX);
            //FileSystem.Input(SRC.SaveDataFileNumber, GUI.MapY);

            //// ADD START 240a
            //FileSystem.Input(SRC.SaveDataFileNumber, buf);
            //if ("Layer" == buf)
            //{
            //    // 各レイヤ
            //    var loopTo2 = MapWidth;
            //    for (i = 1; i <= loopTo2; i++)
            //    {
            //        var loopTo3 = MapHeight;
            //        for (j = 1; j <= loopTo3; j++)
            //        {
            //            FileSystem.Input(SRC.SaveDataFileNumber, ibuf1);
            //            FileSystem.Input(SRC.SaveDataFileNumber, ibuf2);
            //            FileSystem.Input(SRC.SaveDataFileNumber, ibuf3);
            //            if (ibuf1 != MapData[i, j, MapDataIndex.LayerType])
            //            {
            //                MapData[i, j, MapDataIndex.LayerType] = ibuf1;
            //                is_map_changed = true;
            //            }

            //            if (ibuf2 != MapData[i, j, MapDataIndex.LayerBitmapNo])
            //            {
            //                MapData[i, j, MapDataIndex.LayerBitmapNo] = ibuf2;
            //                is_map_changed = true;
            //            }

            //            if (ibuf3 != MapData[i, j, MapDataIndex.BoxType])
            //            {
            //                MapData[i, j, MapDataIndex.BoxType] = ibuf3;
            //                is_map_changed = true;
            //            }
            //        }
            //    }
            //    // ＢＧＭ関連情報の1行目を読み込む
            //    FileSystem.Input(SRC.SaveDataFileNumber, buf);
            //}

            //RestoreMapDataRet = buf;
            //// 背景書き換えの必要有り？
            //if (is_map_changed)
            //{
            //    IsMapDirty = true;
            //}
            //// ADD  END  240a

            //// ユニット配置
            //var loopTo4 = MapWidth;
            //for (i = 1; i <= loopTo4; i++)
            //{
            //    var loopTo5 = MapHeight;
            //    for (j = 1; j <= loopTo5; j++)
            //        // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //        MapDataForUnit[i, j] = null;
            //}

            //foreach (Unit u in SRC.UList)
            //{
            //    if (u.Status == "出撃")
            //    {
            //        MapDataForUnit[u.x, u.y] = u;
            //    }
            //}

            //return RestoreMapDataRet;
        }

        // (X,Y)を中心とする min_range - max_range のエリアを選択
        // エリア内のユニットは uparty の指示に従い選択
        public void AreaInRange(int X, int Y, int max_range, int min_range, string uparty)
        {
            int x1, y1;
            int x2, y2;
            int i, j;
            int n;

            // 選択情報をクリア
            for (i = 1; i <= MapWidth; i++)
            {
                for (j = 1; j <= MapHeight; j++)
                    MaskData[i, j] = true;
            }

            x1 = Math.Max(X - max_range, 1);
            x2 = Math.Min(X + max_range, MapWidth);
            y1 = Math.Max(Y - max_range, 1);
            y2 = Math.Min(Y + max_range, MapHeight);

            // max_range内かつmin_range外のエリアを選択
            for (i = x1; i <= x2; i++)
            {
                for (j = y1; j <= y2; j++)
                {
                    n = (Math.Abs((X - i)) + Math.Abs((Y - j)));
                    if (n <= max_range)
                    {
                        if (n >= min_range)
                        {
                            MaskData[i, j] = false;
                        }
                    }
                }
            }

            // エリア内のユニットを選択するかそれぞれ判定
            switch (uparty ?? "")
            {
                case "味方":
                case "ＮＰＣ":
                    for (i = x1; i <= x2; i++)
                    {
                        for (j = y1; j <= y2; j++)
                        {
                            if (!MaskData[i, j])
                            {
                                if (MapDataForUnit[i, j] != null)
                                {
                                    if (!(MapDataForUnit[i, j].Party == "味方")
                                        && !(MapDataForUnit[i, j].Party == "ＮＰＣ"))
                                    {
                                        MaskData[i, j] = true;
                                    }
                                }
                            }
                        }
                    }
                    break;

                case "味方の敵":
                case "ＮＰＣの敵":
                    for (i = x1; i <= x2; i++)
                    {
                        for (j = y1; j <= y2; j++)
                        {
                            if (!MaskData[i, j])
                            {
                                if (MapDataForUnit[i, j] is object)
                                {
                                    {
                                        var u = MapDataForUnit[i, j];
                                        if ((u.Party == "味方" || u.Party == "ＮＰＣ")
                                            // TODO Impl
                                            //&& !u.IsConditionSatisfied("暴走")
                                            //&& !u.IsConditionSatisfied("魅了")
                                            //&& !u.IsConditionSatisfied("混乱")
                                            //&& !u.IsConditionSatisfied("憑依")
                                            //&& !u.IsConditionSatisfied("睡眠")
                                            )
                                        {
                                            MaskData[i, j] = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    break;

                case "敵":
                    for (i = x1; i <= x2; i++)
                    {
                        for (j = y1; j <= y2; j++)
                        {
                            if (!MaskData[i, j])
                            {
                                if (MapDataForUnit[i, j] is object)
                                {
                                    if (!(MapDataForUnit[i, j].Party == "敵"))
                                    {
                                        MaskData[i, j] = true;
                                    }
                                }
                            }
                        }
                    }

                    break;

                case "敵の敵":
                    for (i = x1; i <= x2; i++)
                    {
                        for (j = y1; j <= y2; j++)
                        {
                            if (!MaskData[i, j])
                            {
                                if (MapDataForUnit[i, j] is object)
                                {
                                    {
                                        var withBlock1 = MapDataForUnit[i, j];
                                        if (withBlock1.Party == "敵")
                                        {
                                            MaskData[i, j] = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    break;

                case "中立":
                    for (i = x1; i <= x2; i++)
                    {
                        for (j = y1; j <= y2; j++)
                        {
                            if (!MaskData[i, j])
                            {
                                if (MapDataForUnit[i, j] is object)
                                {
                                    if (!(MapDataForUnit[i, j].Party == "中立"))
                                    {
                                        MaskData[i, j] = true;
                                    }
                                }
                            }
                        }
                    }

                    break;

                case "中立の敵":
                    for (i = x1; i <= x2; i++)
                    {
                        for (j = y1; j <= y2; j++)
                        {
                            if (!MaskData[i, j])
                            {
                                if (MapDataForUnit[i, j] is object)
                                {
                                    {
                                        var withBlock2 = MapDataForUnit[i, j];
                                        if (withBlock2.Party == "中立")
                                        {
                                            MaskData[i, j] = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    break;

                case "空間":
                    for (i = x1; i <= x2; i++)
                    {
                        for (j = y1; j <= y2; j++)
                        {
                            if (!MaskData[i, j])
                            {
                                if (MapDataForUnit[i, j] is object)
                                {
                                    MaskData[i, j] = true;
                                }
                            }
                        }
                    }
                    break;

                case "全て":
                case "無差別":
                    break;
            }

            // エリアの中心は常に選択
            MaskData[X, Y] = false;
        }

        // ユニット u から移動後使用可能な射程 max_range の武器／アビリティを使う場合の効果範囲
        // エリア内のユニットは Party の指示に従い選択
        public void AreaInReachable(Unit u, int max_range, string uparty)
        {
            throw new NotImplementedException();
            //bool[] tmp_mask_data;
            //int j, i, k;

            //// まずは移動範囲を選択
            //AreaInSpeed(u);

            //// 選択範囲をmax_rangeぶんだけ拡大
            //tmp_mask_data = new bool[MapWidth + 1 + 1, MapHeight + 1 + 1];
            //var loopTo = (MapWidth + 1);
            //for (i = 0; i <= loopTo; i++)
            //{
            //    var loopTo1 = (MapHeight + 1);
            //    for (j = 0; j <= loopTo1; j++)
            //        tmp_mask_data[i, j] = true;
            //}

            //var loopTo2 = max_range;
            //for (i = 1; i <= loopTo2; i++)
            //{
            //    var loopTo3 = MapWidth;
            //    for (j = 1; j <= loopTo3; j++)
            //    {
            //        var loopTo4 = MapHeight;
            //        for (k = 1; k <= loopTo4; k++)
            //            tmp_mask_data[j, k] = MaskData[j, k];
            //    }

            //    var loopTo5 = MapWidth;
            //    for (j = 1; j <= loopTo5; j++)
            //    {
            //        var loopTo6 = MapHeight;
            //        for (k = 1; k <= loopTo6; k++)
            //            MaskData[j, k] = tmp_mask_data[j, k] & tmp_mask_data[j - 1, k] & tmp_mask_data[j + 1, k] & tmp_mask_data[j, k - 1] & tmp_mask_data[j, k + 1];
            //    }
            //}

            //// エリア内のユニットを選択するかそれぞれ判定
            //switch (uparty ?? "")
            //{
            //    case "味方":
            //    case "ＮＰＣ":
            //        {
            //            var loopTo7 = MapWidth;
            //            for (i = 1; i <= loopTo7; i++)
            //            {
            //                var loopTo8 = MapHeight;
            //                for (j = 1; j <= loopTo8; j++)
            //                {
            //                    if (!MaskData[i, j])
            //                    {
            //                        if (MapDataForUnit[i, j] is object)
            //                        {
            //                            if (!(MapDataForUnit[i, j].Party == "味方") & !(MapDataForUnit[i, j].Party == "ＮＰＣ"))
            //                            {
            //                                MaskData[i, j] = true;
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "味方の敵":
            //    case "ＮＰＣの敵":
            //        {
            //            var loopTo9 = MapWidth;
            //            for (i = 1; i <= loopTo9; i++)
            //            {
            //                var loopTo10 = MapHeight;
            //                for (j = 1; j <= loopTo10; j++)
            //                {
            //                    if (!MaskData[i, j])
            //                    {
            //                        if (MapDataForUnit[i, j] is object)
            //                        {
            //                            {
            //                                var withBlock = MapDataForUnit[i, j];
            //                                if ((withBlock.Party == "味方" | withBlock.Party == "ＮＰＣ") & !withBlock.IsConditionSatisfied("暴走") & !withBlock.IsConditionSatisfied("魅了") & !withBlock.IsConditionSatisfied("憑依"))
            //                                {
            //                                    MaskData[i, j] = true;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "敵":
            //        {
            //            var loopTo11 = MapWidth;
            //            for (i = 1; i <= loopTo11; i++)
            //            {
            //                var loopTo12 = MapHeight;
            //                for (j = 1; j <= loopTo12; j++)
            //                {
            //                    if (!MaskData[i, j])
            //                    {
            //                        if (MapDataForUnit[i, j] is object)
            //                        {
            //                            if (!(MapDataForUnit[i, j].Party == "敵"))
            //                            {
            //                                MaskData[i, j] = true;
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "敵の敵":
            //        {
            //            var loopTo13 = MapWidth;
            //            for (i = 1; i <= loopTo13; i++)
            //            {
            //                var loopTo14 = MapHeight;
            //                for (j = 1; j <= loopTo14; j++)
            //                {
            //                    if (!MaskData[i, j])
            //                    {
            //                        if (MapDataForUnit[i, j] is object)
            //                        {
            //                            {
            //                                var withBlock1 = MapDataForUnit[i, j];
            //                                if (withBlock1.Party == "敵")
            //                                {
            //                                    MaskData[i, j] = true;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "中立":
            //        {
            //            var loopTo15 = MapWidth;
            //            for (i = 1; i <= loopTo15; i++)
            //            {
            //                var loopTo16 = MapHeight;
            //                for (j = 1; j <= loopTo16; j++)
            //                {
            //                    if (!MaskData[i, j])
            //                    {
            //                        if (MapDataForUnit[i, j] is object)
            //                        {
            //                            if (!(MapDataForUnit[i, j].Party == "中立"))
            //                            {
            //                                MaskData[i, j] = true;
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "中立の敵":
            //        {
            //            var loopTo17 = MapWidth;
            //            for (i = 1; i <= loopTo17; i++)
            //            {
            //                var loopTo18 = MapHeight;
            //                for (j = 1; j <= loopTo18; j++)
            //                {
            //                    if (!MaskData[i, j])
            //                    {
            //                        if (MapDataForUnit[i, j] is object)
            //                        {
            //                            {
            //                                var withBlock2 = MapDataForUnit[i, j];
            //                                if (withBlock2.Party == "中立")
            //                                {
            //                                    MaskData[i, j] = true;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "空間":
            //        {
            //            var loopTo19 = MapWidth;
            //            for (i = 1; i <= loopTo19; i++)
            //            {
            //                var loopTo20 = MapHeight;
            //                for (j = 1; j <= loopTo20; j++)
            //                {
            //                    if (!MaskData[i, j])
            //                    {
            //                        if (MapDataForUnit[i, j] is object)
            //                        {
            //                            MaskData[i, j] = true;
            //                        }
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "全て":
            //    case "無差別":
            //        {
            //            break;
            //        }
            //}

            //// エリアの中心は常に選択
            //MaskData[u.x, u.y] = false;
        }

        // マップ全域に渡ってupartyに属するユニットが存在する場所を選択
        public void AreaWithUnit(string uparty)
        {
            throw new NotImplementedException();
            //int i, j;
            //Unit u;
            //var loopTo = MapWidth;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (j = 1; j <= loopTo1; j++)
            //        MaskData[i, j] = true;
            //}

            //var loopTo2 = MapWidth;
            //for (i = 1; i <= loopTo2; i++)
            //{
            //    var loopTo3 = MapHeight;
            //    for (j = 1; j <= loopTo3; j++)
            //    {
            //        u = MapDataForUnit[i, j];
            //        if (u is null)
            //        {
            //            goto NextLoop;
            //        }

            //        switch (uparty ?? "")
            //        {
            //            case "味方":
            //                {
            //                    if (u.Party == "味方" | u.Party == "ＮＰＣ")
            //                    {
            //                        MaskData[i, j] = false;
            //                    }

            //                    break;
            //                }

            //            case "味方の敵":
            //                {
            //                    if (u.Party != "味方" & u.Party != "ＮＰＣ")
            //                    {
            //                        MaskData[i, j] = false;
            //                    }

            //                    break;
            //                }

            //            case "敵":
            //                {
            //                    if (u.Party == "敵")
            //                    {
            //                        MaskData[i, j] = false;
            //                    }

            //                    break;
            //                }

            //            case "敵の敵":
            //                {
            //                    if (u.Party != "敵")
            //                    {
            //                        MaskData[i, j] = false;
            //                    }

            //                    break;
            //                }

            //            case "中立":
            //                {
            //                    if (u.Party == "中立")
            //                    {
            //                        MaskData[i, j] = false;
            //                    }

            //                    break;
            //                }

            //            case "中立の敵":
            //                {
            //                    if (u.Party != "中立")
            //                    {
            //                        MaskData[i, j] = false;
            //                    }

            //                    break;
            //                }

            //            default:
            //                {
            //                    MaskData[i, j] = false;
            //                    break;
            //                }
            //        }

            //    NextLoop:
            //        ;
            //    }
            //}
        }

        // 十字状のエリアを選択 (Ｍ直の攻撃方向選択用)
        public void AreaInCross(int X, int Y, int min_range, int max_range)
        {
            throw new NotImplementedException();
            //int i, j;
            //var loopTo = MapWidth;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (j = 1; j <= loopTo1; j++)
            //        MaskData[i, j] = true;
            //}

            //var loopTo2 = (Y - min_range);
            //for (i = (Y - max_range); i <= loopTo2; i++)
            //{
            //    if (i >= 1)
            //    {
            //        MaskData[X, i] = false;
            //    }
            //}

            //var loopTo3 = (Y + max_range);
            //for (i = (Y + min_range); i <= loopTo3; i++)
            //{
            //    if (i <= MapHeight)
            //    {
            //        MaskData[X, i] = false;
            //    }
            //}

            //var loopTo4 = (X - min_range);
            //for (i = (X - max_range); i <= loopTo4; i++)
            //{
            //    if (i >= 1)
            //    {
            //        MaskData[i, Y] = false;
            //    }
            //}

            //var loopTo5 = (X + max_range);
            //for (i = (X + min_range); i <= loopTo5; i++)
            //{
            //    if (i <= MapWidth)
            //    {
            //        MaskData[i, Y] = false;
            //    }
            //}

            //MaskData[X, Y] = false;
        }

        // 直線状のエリアを選択 (Ｍ直の攻撃範囲設定用)
        public void AreaInLine(int X, int Y, int min_range, int max_range, string direction)
        {
            throw new NotImplementedException();
            //int i, j;
            //var loopTo = MapWidth;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (j = 1; j <= loopTo1; j++)
            //        MaskData[i, j] = true;
            //}

            //switch (direction ?? "")
            //{
            //    case "N":
            //        {
            //            var loopTo2 = (Y - min_range);
            //            for (i = (Y - max_range); i <= loopTo2; i++)
            //            {
            //                if (i >= 1)
            //                {
            //                    MaskData[X, i] = false;
            //                }
            //            }

            //            break;
            //        }

            //    case "S":
            //        {
            //            var loopTo3 = (Y + max_range);
            //            for (i = (Y + min_range); i <= loopTo3; i++)
            //            {
            //                if (i <= MapHeight)
            //                {
            //                    MaskData[X, i] = false;
            //                }
            //            }

            //            break;
            //        }

            //    case "W":
            //        {
            //            var loopTo4 = (X - min_range);
            //            for (i = (X - max_range); i <= loopTo4; i++)
            //            {
            //                if (i >= 1)
            //                {
            //                    MaskData[i, Y] = false;
            //                }
            //            }

            //            break;
            //        }

            //    case "E":
            //        {
            //            var loopTo5 = (X + max_range);
            //            for (i = (X + min_range); i <= loopTo5; i++)
            //            {
            //                if (i <= MapWidth)
            //                {
            //                    MaskData[i, Y] = false;
            //                }
            //            }

            //            break;
            //        }
            //}

            //MaskData[X, Y] = false;
        }

        // 幅３マスの十字状のエリアを選択 (Ｍ拡の攻撃方向選択用)
        public void AreaInWideCross(int X, int Y, int min_range, int max_range)
        {
            throw new NotImplementedException();
            //int i, j;
            //var loopTo = MapWidth;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (j = 1; j <= loopTo1; j++)
            //        MaskData[i, j] = true;
            //}

            //var loopTo2 = (Y - min_range);
            //for (i = (Y - max_range); i <= loopTo2; i++)
            //{
            //    if (i >= 1)
            //    {
            //        MaskData[X, i] = false;
            //    }
            //}

            //var loopTo3 = (Y - (min_range + 1));
            //for (i = (Y - max_range + 1); i <= loopTo3; i++)
            //{
            //    if (i >= 1)
            //    {
            //        if (X > 1)
            //        {
            //            MaskData[X - 1, i] = false;
            //        }

            //        if (X < MapWidth)
            //        {
            //            MaskData[X + 1, i] = false;
            //        }
            //    }
            //}

            //var loopTo4 = (Y + max_range);
            //for (i = (Y + min_range); i <= loopTo4; i++)
            //{
            //    if (i <= MapHeight)
            //    {
            //        MaskData[X, i] = false;
            //    }
            //}

            //var loopTo5 = (Y + max_range - 1);
            //for (i = (Y + (min_range + 1)); i <= loopTo5; i++)
            //{
            //    if (i <= MapHeight)
            //    {
            //        if (X > 1)
            //        {
            //            MaskData[X - 1, i] = false;
            //        }

            //        if (X < MapWidth)
            //        {
            //            MaskData[X + 1, i] = false;
            //        }
            //    }
            //}

            //var loopTo6 = (X - min_range);
            //for (i = (X - max_range); i <= loopTo6; i++)
            //{
            //    if (i >= 1)
            //    {
            //        MaskData[i, Y] = false;
            //    }
            //}

            //var loopTo7 = (X - (min_range + 1));
            //for (i = (X - max_range + 1); i <= loopTo7; i++)
            //{
            //    if (i >= 1)
            //    {
            //        if (Y > 1)
            //        {
            //            MaskData[i, Y - 1] = false;
            //        }

            //        if (Y < MapHeight)
            //        {
            //            MaskData[i, Y + 1] = false;
            //        }
            //    }
            //}

            //var loopTo8 = (X + max_range);
            //for (i = (X + min_range); i <= loopTo8; i++)
            //{
            //    if (i <= MapWidth)
            //    {
            //        MaskData[i, Y] = false;
            //    }
            //}

            //var loopTo9 = (X + max_range - 1);
            //for (i = (X + (min_range + 1)); i <= loopTo9; i++)
            //{
            //    if (i <= MapWidth)
            //    {
            //        if (Y > 1)
            //        {
            //            MaskData[i, Y - 1] = false;
            //        }

            //        if (Y < MapHeight)
            //        {
            //            MaskData[i, Y + 1] = false;
            //        }
            //    }
            //}

            //MaskData[X, Y] = false;
        }

        // 幅３マスの直線状のエリアを選択 (Ｍ拡の攻撃範囲設定用)
        public void AreaInCone(int X, int Y, int min_range, int max_range, string direction)
        {
            throw new NotImplementedException();
            //int i, j;
            //var loopTo = MapWidth;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (j = 1; j <= loopTo1; j++)
            //        MaskData[i, j] = true;
            //}

            //switch (direction ?? "")
            //{
            //    case "N":
            //        {
            //            var loopTo2 = (Y - min_range);
            //            for (i = (Y - max_range); i <= loopTo2; i++)
            //            {
            //                if (i >= 1)
            //                {
            //                    MaskData[X, i] = false;
            //                }
            //            }

            //            var loopTo3 = (Y - (min_range + 1));
            //            for (i = (Y - max_range + 1); i <= loopTo3; i++)
            //            {
            //                if (i >= 1)
            //                {
            //                    if (X > 1)
            //                    {
            //                        MaskData[X - 1, i] = false;
            //                    }

            //                    if (X < MapWidth)
            //                    {
            //                        MaskData[X + 1, i] = false;
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "S":
            //        {
            //            var loopTo4 = (Y + max_range);
            //            for (i = (Y + min_range); i <= loopTo4; i++)
            //            {
            //                if (i <= MapHeight)
            //                {
            //                    MaskData[X, i] = false;
            //                }
            //            }

            //            var loopTo5 = (Y + max_range - 1);
            //            for (i = (Y + (min_range + 1)); i <= loopTo5; i++)
            //            {
            //                if (i <= MapHeight)
            //                {
            //                    if (X > 1)
            //                    {
            //                        MaskData[X - 1, i] = false;
            //                    }

            //                    if (X < MapWidth)
            //                    {
            //                        MaskData[X + 1, i] = false;
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "W":
            //        {
            //            var loopTo6 = (X - min_range);
            //            for (i = (X - max_range); i <= loopTo6; i++)
            //            {
            //                if (i >= 1)
            //                {
            //                    MaskData[i, Y] = false;
            //                }
            //            }

            //            var loopTo7 = (X - (min_range + 1));
            //            for (i = (X - max_range + 1); i <= loopTo7; i++)
            //            {
            //                if (i >= 1)
            //                {
            //                    if (Y > 1)
            //                    {
            //                        MaskData[i, Y - 1] = false;
            //                    }

            //                    if (Y < MapHeight)
            //                    {
            //                        MaskData[i, Y + 1] = false;
            //                    }
            //                }
            //            }

            //            break;
            //        }

            //    case "E":
            //        {
            //            var loopTo8 = (X + max_range);
            //            for (i = (X + min_range); i <= loopTo8; i++)
            //            {
            //                if (i <= MapWidth)
            //                {
            //                    MaskData[i, Y] = false;
            //                }
            //            }

            //            var loopTo9 = (X + max_range - 1);
            //            for (i = (X + (min_range + 1)); i <= loopTo9; i++)
            //            {
            //                if (i <= MapWidth)
            //                {
            //                    if (Y > 1)
            //                    {
            //                        MaskData[i, Y - 1] = false;
            //                    }

            //                    if (Y < MapHeight)
            //                    {
            //                        MaskData[i, Y + 1] = false;
            //                    }
            //                }
            //            }

            //            break;
            //        }
            //}

            //MaskData[X, Y] = false;
        }

        // 扇状のエリアを選択 (Ｍ扇の攻撃範囲設定用)
        public void AreaInSector(int X, int Y, int min_range, int max_range, string direction, int lv, bool without_refresh = false)
        {
            throw new NotImplementedException();
            //int xx, i, yy;
            //if (!without_refresh)
            //{
            //    var loopTo = MapWidth;
            //    for (xx = 1; xx <= loopTo; xx++)
            //    {
            //        var loopTo1 = MapHeight;
            //        for (yy = 1; yy <= loopTo1; yy++)
            //            MaskData[xx, yy] = true;
            //    }
            //}

            //switch (direction ?? "")
            //{
            //    case "N":
            //        {
            //            var loopTo2 = max_range;
            //            for (i = min_range; i <= loopTo2; i++)
            //            {
            //                yy = (Y - i);
            //                if (yy < 1)
            //                {
            //                    break;
            //                }

            //                switch (lv)
            //                {
            //                    case 1:
            //                        {
            //                            var loopTo3 = Math.Min(X + i / 3, MapWidth);
            //                            for (xx = Math.Max(X - i / 3, 1); xx <= loopTo3; xx++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 2:
            //                        {
            //                            var loopTo4 = Math.Min(X + i / 2, MapWidth);
            //                            for (xx = Math.Max(X - i / 2, 1); xx <= loopTo4; xx++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 3:
            //                        {
            //                            var loopTo5 = Math.Min(X + (i - 1), MapWidth);
            //                            for (xx = Math.Max(X - (i - 1), 1); xx <= loopTo5; xx++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 4:
            //                        {
            //                            var loopTo6 = Math.Min(X + i, MapWidth);
            //                            for (xx = Math.Max(X - i, 1); xx <= loopTo6; xx++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }
            //                }
            //            }

            //            break;
            //        }

            //    case "S":
            //        {
            //            var loopTo7 = max_range;
            //            for (i = min_range; i <= loopTo7; i++)
            //            {
            //                yy = (Y + i);
            //                if (yy > MapHeight)
            //                {
            //                    break;
            //                }

            //                switch (lv)
            //                {
            //                    case 1:
            //                        {
            //                            var loopTo8 = Math.Min(X + i / 3, MapWidth);
            //                            for (xx = Math.Max(X - i / 3, 1); xx <= loopTo8; xx++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 2:
            //                        {
            //                            var loopTo9 = Math.Min(X + i / 2, MapWidth);
            //                            for (xx = Math.Max(X - i / 2, 1); xx <= loopTo9; xx++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 3:
            //                        {
            //                            var loopTo10 = Math.Min(X + (i - 1), MapWidth);
            //                            for (xx = Math.Max(X - (i - 1), 1); xx <= loopTo10; xx++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 4:
            //                        {
            //                            var loopTo11 = Math.Min(X + i, MapWidth);
            //                            for (xx = Math.Max(X - i, 1); xx <= loopTo11; xx++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }
            //                }
            //            }

            //            break;
            //        }

            //    case "W":
            //        {
            //            var loopTo12 = max_range;
            //            for (i = min_range; i <= loopTo12; i++)
            //            {
            //                xx = (X - i);
            //                if (xx < 1)
            //                {
            //                    break;
            //                }

            //                switch (lv)
            //                {
            //                    case 1:
            //                        {
            //                            var loopTo13 = Math.Min(Y + i / 3, MapHeight);
            //                            for (yy = Math.Max(Y - i / 3, 1); yy <= loopTo13; yy++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 2:
            //                        {
            //                            var loopTo14 = Math.Min(Y + i / 2, MapHeight);
            //                            for (yy = Math.Max(Y - i / 2, 1); yy <= loopTo14; yy++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 3:
            //                        {
            //                            var loopTo15 = Math.Min(Y + (i - 1), MapHeight);
            //                            for (yy = Math.Max(Y - (i - 1), 1); yy <= loopTo15; yy++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 4:
            //                        {
            //                            var loopTo16 = Math.Min(Y + i, MapHeight);
            //                            for (yy = Math.Max(Y - i, 1); yy <= loopTo16; yy++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }
            //                }
            //            }

            //            break;
            //        }

            //    case "E":
            //        {
            //            var loopTo17 = max_range;
            //            for (i = min_range; i <= loopTo17; i++)
            //            {
            //                xx = (X + i);
            //                if (xx > MapWidth)
            //                {
            //                    break;
            //                }

            //                switch (lv)
            //                {
            //                    case 1:
            //                        {
            //                            var loopTo18 = Math.Min(Y + i / 3, MapHeight);
            //                            for (yy = Math.Max(Y - i / 3, 1); yy <= loopTo18; yy++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 2:
            //                        {
            //                            var loopTo19 = Math.Min(Y + i / 2, MapHeight);
            //                            for (yy = Math.Max(Y - i / 2, 1); yy <= loopTo19; yy++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 3:
            //                        {
            //                            var loopTo20 = Math.Min(Y + (i - 1), MapHeight);
            //                            for (yy = Math.Max(Y - (i - 1), 1); yy <= loopTo20; yy++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }

            //                    case 4:
            //                        {
            //                            var loopTo21 = Math.Min(Y + i, MapHeight);
            //                            for (yy = Math.Max(Y - i, 1); yy <= loopTo21; yy++)
            //                                MaskData[xx, yy] = false;
            //                            break;
            //                        }
            //                }
            //            }

            //            break;
            //        }
            //}

            //MaskData[X, Y] = false;
        }

        // 十字状の扇状のエリアを選択 (Ｍ扇の攻撃方向選択用)
        public void AreaInSectorCross(int X, int Y, int min_range, int max_range, int lv)
        {
            throw new NotImplementedException();
            //int xx, yy;
            //var loopTo = MapWidth;
            //for (xx = 1; xx <= loopTo; xx++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (yy = 1; yy <= loopTo1; yy++)
            //        MaskData[xx, yy] = true;
            //}

            //AreaInSector(X, Y, min_range, max_range, "N", lv, true);
            //AreaInSector(X, Y, min_range, max_range, "S", lv, true);
            //AreaInSector(X, Y, min_range, max_range, "W", lv, true);
            //AreaInSector(X, Y, min_range, max_range, "E", lv, true);
        }

        // ２点間を結ぶ直線状のエリアを選択 (Ｍ線の範囲設定用)
        public void AreaInPointToPoint(int x1, int y1, int x2, int y2)
        {
            throw new NotImplementedException();
            //int xx, yy;

            //// まず全領域をマスク
            //var loopTo = MapWidth;
            //for (xx = 1; xx <= loopTo; xx++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (yy = 1; yy <= loopTo1; yy++)
            //        MaskData[xx, yy] = true;
            //}

            //// 起点のマスクを解除
            //MaskData[x1, y1] = false;
            //xx = x1;
            //yy = y1;
            //if (Math.Abs((x1 - x2)) > Math.Abs((y1 - y2)))
            //{
            //    do
            //    {
            //        if (x1 > x2)
            //        {
            //            xx = (xx - 1);
            //        }
            //        else
            //        {
            //            xx = (xx + 1);
            //        }

            //        MaskData[xx, yy] = false;
            //        yy = (y1 + (y2 - y1) * (x1 - xx + 0d) / (x1 - x2));
            //        MaskData[xx, yy] = false;
            //    }
            //    while (xx != x2);
            //}
            //else
            //{
            //    do
            //    {
            //        if (y1 > y2)
            //        {
            //            yy = (yy - 1);
            //        }
            //        else
            //        {
            //            yy = (yy + 1);
            //        }

            //        MaskData[xx, yy] = false;
            //        xx = (x1 + (x2 - x1) * (y1 - yy + 0d) / (y1 - y2));
            //        MaskData[xx, yy] = false;
            //    }
            //    while (yy != y2);
            //}
        }

        // ユニット u の移動範囲を選択
        // ジャンプする場合は ByJump = True
        public void AreaInSpeed(Unit u, bool ByJump = false)
        {
            //int tmp;
            //string buf;
            //bool is_trans_available_on_ground;
            //bool is_trans_available_in_water;
            //var is_trans_available_on_water = default(bool);
            //bool is_trans_available_in_sky;
            //bool is_trans_available_in_moon_sky;
            //var is_adaptable_in_water = default(bool);
            //var is_adaptable_in_space = false;
            //var is_swimable = default(bool);
            //string[] adopted_terrain;
            //string[] allowed_terrains;
            //string[] prohibited_terrains;
            //Unit u2;
            //int x1, y1;
            //int x2, y2;
            //var zarea = default;
            //bool is_zoc;
            //bool is_vzoc, is_hzoc;
            //TerrainData td;
            //bool is_terrain_effective;
            //bool blocked;

            var cur_cost = new int[TotalMoveCost.GetLength(0), TotalMoveCost.GetLength(1)];
            var move_cost = new int[TotalMoveCost.GetLength(0), TotalMoveCost.GetLength(1)];
            string move_area;
            int uspeed;
            var currentUnit = u;

            // 移動時に使用するエリア
            if (ByJump)
            {
                move_area = "空中";
            }
            else
            {
                move_area = currentUnit.Area;
            }

            // 移動力
            uspeed = currentUnit.Speed;
            //if (ByJump)
            //{
            //    uspeed = (currentUnit.Speed + currentUnit.FeatureLevel("ジャンプ"));
            //}
            //else
            //{
            //    uspeed = currentUnit.Speed;
            //}

            //if (currentUnit.IsConditionSatisfied("移動不能"))
            //{
            //    uspeed = 0;
            //}

            // 移動コストは実際の２倍の値で記録されているため、移動力もそれに合わせて
            // ２倍にして移動範囲を計算する
            uspeed = (2 * uspeed);

            //// ADD START MARGE
            //// 再移動時は最初の移動の分だけ移動力を減少させる
            //if (Commands.SelectedCommand == "再移動")
            //{
            //    uspeed = (uspeed - Commands.SelectedUnitMoveCost);
            //}

            //if (currentUnit.IsConditionSatisfied("移動不能"))
            //{
            //    uspeed = 0;
            //}
            //// ADD END MARGE

            // 移動範囲をチェックすべき領域
            var x1 = Math.Max(1, currentUnit.x - uspeed);
            var y1 = Math.Max(1, currentUnit.y - uspeed);
            var x2 = Math.Min(currentUnit.x + uspeed, MapWidth);
            var y2 = Math.Min(currentUnit.y + uspeed, MapHeight);

            // 移動コストとＺＯＣをリセット
            for (var i = 0; i <= MapWidth + 1; i++)
            {
                for (var j = 0; j <= MapHeight + 1; j++)
                {
                    move_cost[i, j] = 1000000;
                    PointInZOC[i, j] = 0;
                }
            }

            // 各地形の移動コストを算出しておく
            FillMoveCost(u, move_cost, move_area, x1, y1, x2, y2);

            //// ユニットがいるため通り抜け出来ない場所をチェック
            //if (!currentUnit.IsFeatureAvailable("すり抜け移動") & !currentUnit.IsUnderSpecialPowerEffect("すり抜け移動"))
            //{
            //    foreach (Unit currentU2 in SRC.UList)
            //    {
            //        u2 = currentU2;
            //        {
            //            var withBlock1 = u2;
            //            if (withBlock1.Status == "出撃")
            //            {
            //                blocked = false;

            //                // 敵対する場合は通り抜け不可
            //                if (withBlock1.IsEnemy(u, true))
            //                {
            //                    blocked = true;
            //                }

            //                // 陣営が合わない場合も通り抜け不可
            //                switch (withBlock1.Party0 ?? "")
            //                {
            //                    case "味方":
            //                    case "ＮＰＣ":
            //                        {
            //                            if (u.Party0 != "味方" & u.Party0 != "ＮＰＣ")
            //                            {
            //                                blocked = true;
            //                            }

            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            if ((withBlock1.Party0 ?? "") != (u.Party0 ?? ""))
            //                            {
            //                                blocked = true;
            //                            }

            //                            break;
            //                        }
            //                }

            //                // 通り抜けられない場合
            //                if (blocked)
            //                {
            //                    move_cost[withBlock1.x, withBlock1.y] = 1000000;
            //                }

            //                // ＺＯＣ
            //                if (blocked & !ByJump)
            //                {
            //                    is_zoc = false;
            //                    zarea = 0;
            //                    if (withBlock1.IsFeatureAvailable("ＺＯＣ") | Expression.IsOptionDefined("ＺＯＣ"))
            //                    {
            //                        is_zoc = true;
            //                        zarea = 1;

            //                        // ＺＯＣ側のＺＯＣレベル
            //                        n = withBlock1.FeatureLevel("ＺＯＣ");
            //                        if (n == 1)
            //                            n = 10000;

            //                        // Option「ＺＯＣ」が指定されている
            //                        n = Math.Max(1, n);
            //                        if (u.IsFeatureAvailable("ＺＯＣ無効化"))
            //                        {
            //                            // 移動側のＺＯＣ無効化レベル
            //                            // レベル指定なし、またはLv1はLv10000として扱う
            //                            l = u.FeatureLevel("ＺＯＣ無効化");
            //                            if (l == 1)
            //                                l = 10000;

            //                            // 移動側のＺＯＣ無効化レベルの方が高い場合、
            //                            // ＺＯＣ不可能
            //                            if (l >= n)
            //                            {
            //                                is_zoc = false;
            //                            }
            //                        }

            //                        // 隣接するユニットが「隣接ユニットＺＯＣ無効化」を持っている場合
            //                        if (is_zoc)
            //                        {
            //                            for (i = -1; i <= 1; i++)
            //                            {
            //                                var loopTo44 = Math.Abs(Math.Abs(i) - 1);
            //                                for (j = (Math.Abs(i) - 1); j <= loopTo44; j++)
            //                                {
            //                                    if ((i != 0 | j != 0) & withBlock1.x + i >= 1 & (withBlock1.x + i) <= MapWidth & withBlock1.y + j >= 1 & (withBlock1.y + j) <= MapHeight)
            //                                    {
            //                                        // 隣接ユニットが存在する？
            //                                        if (MapDataForUnit[(withBlock1.x + i), (withBlock1.y + j)] is object)
            //                                        {
            //                                            buf = withBlock1.Party0;
            //                                            {
            //                                                var withBlock2 = MapDataForUnit[(withBlock1.x + i), (withBlock1.y + j)];
            //                                                // 敵対陣営？
            //                                                switch (withBlock2.Party0 ?? "")
            //                                                {
            //                                                    case "味方":
            //                                                    case "ＮＰＣ":
            //                                                        {
            //                                                            if (buf == "味方" | buf == "ＮＰＣ")
            //                                                            {
            //                                                                break;
            //                                                            }

            //                                                            break;
            //                                                        }

            //                                                    default:
            //                                                        {
            //                                                            if ((withBlock2.Party0 ?? "") == (buf ?? ""))
            //                                                            {
            //                                                                break;
            //                                                            }

            //                                                            break;
            //                                                        }
            //                                                }

            //                                                l = withBlock2.FeatureLevel("隣接ユニットＺＯＣ無効化");
            //                                                if (l == 1)
            //                                                    l = 10000;

            //                                                // 移動側のＺＯＣ無効化レベルの方が高い場合、
            //                                                // ＺＯＣ不可能
            //                                                if (l >= n)
            //                                                {
            //                                                    is_zoc = false;
            //                                                    break;
            //                                                }
            //                                            }
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }

            //                    if (is_zoc)
            //                    {
            //                        // 特殊能力「ＺＯＣ」が指定されているなら、そのデータの2つ目の値をＺＯＣの範囲に設定
            //                        // 2つ目の値が省略されている場合は1を設定
            //                        // ＺＯＣLvが0以下の場合、オプション「ＺＯＣ」が指定されていても範囲を0に設定
            //                        if (GeneralLib.LLength(withBlock1.FeatureData("ＺＯＣ")) >= 2)
            //                        {
            //                            string localLIndex() { object argIndex1 = "ＺＯＣ"; string arglist = withBlock1.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                            string localLIndex1() { object argIndex1 = "ＺＯＣ"; string arglist = withBlock1.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                            zarea = Math.Max(Conversions.ToInteger(localLIndex1()), 0);
            //                        }

            //                        // 相対距離＋ＺＯＣの範囲が移動力以内のとき、ＺＯＣを設定
            //                        if (((Math.Abs((u.x - withBlock1.x)) + Math.Abs((u.y - withBlock1.y))) - zarea) <= uspeed)
            //                        {
            //                            // 水平・垂直方向のみのＺＯＣかどうかを判断
            //                            is_hzoc = false;
            //                            is_vzoc = false;
            //                            if (Conversions.ToBoolean(Strings.InStr(withBlock1.FeatureData("ＺＯＣ"), "直線")))
            //                            {
            //                                is_hzoc = true;
            //                                is_vzoc = true;
            //                            }
            //                            else
            //                            {
            //                                if (Conversions.ToBoolean(Strings.InStr(withBlock1.FeatureData("ＺＯＣ"), "水平")))
            //                                {
            //                                    is_hzoc = true;
            //                                }

            //                                if (Conversions.ToBoolean(Strings.InStr(withBlock1.FeatureData("ＺＯＣ"), "垂直")))
            //                                {
            //                                    is_vzoc = true;
            //                                }
            //                            }

            //                            if (is_hzoc | is_vzoc)
            //                            {
            //                                var loopTo45 = zarea;
            //                                for (i = (zarea * -1); i <= loopTo45; i++)
            //                                {
            //                                    if (i == 0)
            //                                    {
            //                                        if (PointInZOC[withBlock1.x, withBlock1.y] < 0)
            //                                        {
            //                                            if (n > Math.Abs(PointInZOC[withBlock1.x, withBlock1.y]))
            //                                            {
            //                                                PointInZOC[withBlock1.x, withBlock1.y] = n;
            //                                            }
            //                                        }
            //                                        else
            //                                        {
            //                                            PointInZOC[withBlock1.x, withBlock1.y] = Math.Max(n, PointInZOC[withBlock1.x, withBlock1.y]);
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        // 水平ＺＯＣ
            //                                        if (is_hzoc & withBlock1.x + i >= 1 & (withBlock1.x + i) <= MapWidth)
            //                                        {
            //                                            if (PointInZOC[(withBlock1.x + i), withBlock1.y] < 0)
            //                                            {
            //                                                if (n > Math.Abs(PointInZOC[(withBlock1.x + i), withBlock1.y]))
            //                                                {
            //                                                    PointInZOC[(withBlock1.x + i), withBlock1.y] = n;
            //                                                }
            //                                            }
            //                                            else
            //                                            {
            //                                                PointInZOC[(withBlock1.x + i), withBlock1.y] = Math.Max(n, PointInZOC[(withBlock1.x + i), withBlock1.y]);
            //                                            }
            //                                        }
            //                                        // 垂直ＺＯＣ
            //                                        if (is_vzoc & withBlock1.y + i >= 1 & (withBlock1.y + i) <= MapHeight)
            //                                        {
            //                                            if (PointInZOC[withBlock1.x, (withBlock1.y + i)] < 0)
            //                                            {
            //                                                if (n > Math.Abs(PointInZOC[withBlock1.x, (withBlock1.y + i)]))
            //                                                {
            //                                                    PointInZOC[withBlock1.x, (withBlock1.y + i)] = n;
            //                                                }
            //                                            }
            //                                            else
            //                                            {
            //                                                PointInZOC[withBlock1.x, (withBlock1.y + i)] = Math.Max(n, PointInZOC[withBlock1.x, (withBlock1.y + i)]);
            //                                            }
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                            else
            //                            {
            //                                // 全方位ＺＯＣ
            //                                var loopTo46 = zarea;
            //                                for (i = (zarea * -1); i <= loopTo46; i++)
            //                                {
            //                                    var loopTo47 = Math.Abs((Math.Abs(i) - zarea));
            //                                    for (j = (Math.Abs(i) - zarea); j <= loopTo47; j++)
            //                                    {
            //                                        if (withBlock1.x + i >= 1 & (withBlock1.x + i) <= MapWidth & withBlock1.y + j >= 1 & (withBlock1.y + j) <= MapHeight)
            //                                        {
            //                                            if (PointInZOC[(withBlock1.x + i), (withBlock1.y + j)] < 0)
            //                                            {
            //                                                if (n > Math.Abs(PointInZOC[(withBlock1.x + i), (withBlock1.y + j)]))
            //                                                {
            //                                                    PointInZOC[(withBlock1.x + i), (withBlock1.y + j)] = n;
            //                                                }
            //                                            }
            //                                            else
            //                                            {
            //                                                PointInZOC[(withBlock1.x + i), (withBlock1.y + j)] = Math.Max(n, PointInZOC[(withBlock1.x + i), (withBlock1.y + j)]);
            //                                            }
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //                // 「広域ＺＯＣ無効化」を所持している場合の処理
            //                else if (((Math.Abs((u.x - withBlock1.x)) + Math.Abs((u.y - withBlock1.y))) - zarea) <= uspeed)
            //                {
            //                    // レベル指定なし、またはLv1はLv10000として扱う
            //                    l = withBlock1.FeatureLevel("広域ＺＯＣ無効化");
            //                    if (l == 1)
            //                        l = 10000;
            //                    if (l > 0)
            //                    {
            //                        string localLIndex2() { object argIndex1 = "広域ＺＯＣ無効化"; string arglist = withBlock1.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                        int localStrToLng() { string argexpr = hs1020e5bbaf214f5a820fb8d152076551(); var ret = GeneralLib.StrToLng(argexpr); return ret; }

            //                        n = Math.Max(localStrToLng(), 1);
            //                        var loopTo48 = n;
            //                        for (i = (n * -1); i <= loopTo48; i++)
            //                        {
            //                            var loopTo49 = Math.Abs((Math.Abs(i) - n));
            //                            for (j = (Math.Abs(i) - n); j <= loopTo49; j++)
            //                            {
            //                                if (withBlock1.x + i >= 1 & (withBlock1.x + i) <= MapWidth & withBlock1.y + j >= 1 & (withBlock1.y + j) <= MapHeight)
            //                                {
            //                                    PointInZOC[(withBlock1.x + i), (withBlock1.y + j)] = PointInZOC[(withBlock1.x + i), (withBlock1.y + j)] - l;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            //// 移動停止地形はＺＯＣして扱う
            //if (!ByJump)
            //{
            //    {
            //        var withBlock3 = SRC.TDList;
            //        var loopTo50 = x2;
            //        for (i = x1; i <= loopTo50; i++)
            //        {
            //            var loopTo51 = y2;
            //            for (j = y1; j <= loopTo51; j++)
            //            {
            //                // MOD START 240a
            //                // If .IsFeatureAvailable(MapData(i, j, 0), "移動停止") Then
            //                // PointInZOC(i, j) = 20000
            //                // End If
            //                if (TerrainHasMoveStop(i, j))
            //                {
            //                    PointInZOC[i, j] = 20000;
            //                }
            //                // MOD  END  240a
            //            }
            //        }
            //    }
            //}

            // マップ上の各地点に到達するのに必要な移動力を計算する

            // まず移動コスト計算用の配列を初期化
            for (var i = 0; i <= MapWidth + 1; i++)
            {
                for (var j = 0; j <= MapHeight + 1; j++)
                {
                    TotalMoveCost[i, j] = 1000000;
                }
            }

            // 現在いる場所は移動する必要がないため、必要移動力が0
            TotalMoveCost[currentUnit.x, currentUnit.y] = 0;

            // 必要移動力の計算
            for (var sp = 1; sp <= uspeed; sp++)
            {
                // 現在の必要移動力を保存
                for (var i = 0; i <= MapWidth + 1; i++)
                {
                    for (var j = 0; j <= MapHeight + 1; j++)
                    {
                        cur_cost[i, j] = TotalMoveCost[i, j];
                    }
                }

                var xmax = Math.Min(currentUnit.x + sp, MapWidth);
                var xmin = Math.Max(1, currentUnit.x - sp);
                for (var x = xmin; x <= xmax; x++)
                {
                    var ymax = Math.Min(currentUnit.y + sp, MapHeight);
                    var ymin = Math.Max(1, currentUnit.y - sp);
                    for (var y = ymin; y <= ymax; y++)
                    {
                        // 隣接する地点と比較して最も低い必要移動力を求める
                        var tmp = cur_cost[x, y];
                        // TODO ZOCとかもろもろ
                        //if (sp > 1)
                        //{
                        //    {
                        //        var withBlock4 = SRC.TDList;
                        //        tmp = Math.Min(tmp, Operators.AddObject(cur_cost[x - 1, y], Interaction.IIf(PointInZOC[x - 1, y] > 0, 10000, 0)));
                        //        tmp = Math.Min(tmp, Operators.AddObject(cur_cost[x + 1, y], Interaction.IIf(PointInZOC[x + 1, y] > 0, 10000, 0)));
                        //        tmp = Math.Min(tmp, Operators.AddObject(cur_cost[x, y - 1], Interaction.IIf(PointInZOC[x, y - 1] > 0, 10000, 0)));
                        //        tmp = Math.Min(tmp, Operators.AddObject(cur_cost[x, y + 1], Interaction.IIf(PointInZOC[x, y + 1] > 0, 10000, 0)));
                        //    }
                        //}
                        //else
                        {
                            tmp = Math.Min(tmp, cur_cost[x - 1, y]);
                            tmp = Math.Min(tmp, cur_cost[x + 1, y]);
                            tmp = Math.Min(tmp, cur_cost[x, y - 1]);
                            tmp = Math.Min(tmp, cur_cost[x, y + 1]);
                        }
                        // 地形に進入するのに必要な移動力を加算
                        tmp = tmp + move_cost[x, y];
                        // 前回の値とどちらが低い？
                        TotalMoveCost[x, y] = Math.Min(tmp, cur_cost[x, y]);
                    }
                }
            }


            // 算出された必要移動力を元に進入可能か判定
            for (var i = 1; i <= MapWidth; i++)
            {
                for (var j = 1; j <= MapHeight; j++)
                {
                    MaskData[i, j] = true;

                    // 必要移動力が移動力以内？
                    if (TotalMoveCost[i, j] > uspeed)
                    {
                        continue;
                    }

                    var u2 = MapDataForUnit[i, j];

                    // ユニットが存在？
                    if (u2 is null)
                    {
                        MaskData[i, j] = false;
                        continue;
                    }

                    // 合体＆着艦するのは味方のみ
                    if (currentUnit.Party0 != "味方")
                    {
                        continue;
                    }

                    // TODO
                    switch (u2.Party0 ?? "")
                    {
                        case "味方":
                            {
                                if (u2.IsFeatureAvailable("母艦"))
                                {
                                    // 母艦に着艦？
                                    if (!currentUnit.IsFeatureAvailable("母艦") && u2.Area != "地中")
                                    {
                                        if (!currentUnit.IsFeatureAvailable("格納不可"))
                                        {
                                            MaskData[i, j] = false;
                                        }
                                    }
                                }
                                else if (currentUnit.IsFeatureAvailable("合体") && u2.IsFeatureAvailable("合体"))
                                {
                                    // ２体合体？
                                    MaskData[i, j] = true;
                                    // TODO Impl
                                    //var loopTo61 = currentUnit.CountFeature();
                                    //for (k = 1; k <= loopTo61; k++)
                                    //{
                                    //    if (currentUnit.Feature() == "合体" && !string.IsNullOrEmpty(currentUnit.FeatureName()))
                                    //    {
                                    //        buf = currentUnit.FeatureData(k);
                                    //        bool localIsDefined() { object argIndex1 = GeneralLib.LIndex(buf, 2); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

                                    //        bool localIsDefined1() { object argIndex1 = GeneralLib.LIndex(buf, 3); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

                                    //        if (GeneralLib.LLength(buf) == 3 & localIsDefined() & localIsDefined1())
                                    //        {
                                    //            {
                                    //                var withBlock5 = SRC.UList.Item(GeneralLib.LIndex(buf, 2));
                                    //                if (withBlock5.IsConditionSatisfied("行動不能"))
                                    //                {
                                    //                    break;
                                    //                }

                                    //                if (withBlock5.Status == "破棄")
                                    //                {
                                    //                    break;
                                    //                }
                                    //            }

                                    //            Unit localItem() { object argIndex1 = GeneralLib.LIndex(buf, 3); var ret = SRC.UList.Item(argIndex1); return ret; }

                                    //            if ((u2.Name ?? "") == (GeneralLib.LIndex(buf, 3) ?? ""))
                                    //            {
                                    //                MaskData[i, j] = false;
                                    //                break;
                                    //            }
                                    //            else if ((u2.Name ?? "") == (localItem().CurrentForm().Name ?? "") & !u2.IsFeatureAvailable("合体制限"))
                                    //            {
                                    //                MaskData[i, j] = false;
                                    //                break;
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                }

                                break;
                            }

                        case "ＮＰＣ":
                            {
                                if (currentUnit.IsFeatureAvailable("合体") && u2.IsFeatureAvailable("合体"))
                                {
                                    // ２体合体？
                                    MaskData[i, j] = true;
                                    //var loopTo62 = currentUnit.CountFeature();
                                    //for (k = 1; k <= loopTo62; k++)
                                    //{
                                    //    if (currentUnit.Feature(k) == "合体")
                                    //    {
                                    //        buf = currentUnit.FeatureData(k);
                                    //        bool localIsDefined2() { object argIndex1 = GeneralLib.LIndex(buf, 2); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

                                    //        bool localIsDefined3() { object argIndex1 = GeneralLib.LIndex(buf, 3); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

                                    //        if (GeneralLib.LLength(buf) == 3 & localIsDefined2() & localIsDefined3())
                                    //        {
                                    //            {
                                    //                var withBlock6 = SRC.UList.Item(GeneralLib.LIndex(buf, 2));
                                    //                if (withBlock6.IsConditionSatisfied("行動不能"))
                                    //                {
                                    //                    break;
                                    //                }

                                    //                if (withBlock6.Status == "破棄")
                                    //                {
                                    //                    break;
                                    //                }
                                    //            }

                                    //            Unit localItem1() { object argIndex1 = GeneralLib.LIndex(buf, 3); var ret = SRC.UList.Item(argIndex1); return ret; }

                                    //            if ((u2.Name ?? "") == (GeneralLib.LIndex(buf, 3) ?? ""))
                                    //            {
                                    //                MaskData[i, j] = false;
                                    //                break;
                                    //            }
                                    //            else if ((u2.Name ?? "") == (localItem1().CurrentForm().Name ?? "") & !u2.IsFeatureAvailable("合体制限"))
                                    //            {
                                    //                MaskData[i, j] = false;
                                    //                break;
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                }

                                break;
                            }
                    }
                }
            }

            //// ジャンプ＆透過移動先は進入可能？
            //if (ByJump | currentUnit.IsFeatureAvailable("透過移動") | currentUnit.IsUnderSpecialPowerEffect("透過移動"))
            //{
            //    var loopTo63 = x2;
            //    for (i = x1; i <= loopTo63; i++)
            //    {
            //        var loopTo64 = y2;
            //        for (j = y1; j <= loopTo64; j++)
            //        {
            //            if (MaskData[i, j])
            //            {
            //                goto NextLoop2;
            //            }

            //            // ユニットがいる地形に進入出来るということは
            //            // 合体or着艦可能ということなので地形は無視
            //            if (MapDataForUnit[i, j] is object)
            //            {
            //                goto NextLoop2;
            //            }

            //            switch (currentUnit.Area ?? "")
            //            {
            //                case "地上":
            //                    {
            //                        switch (TerrainClass(i, j) ?? "")
            //                        {
            //                            case "空":
            //                                {
            //                                    MaskData[i, j] = true;
            //                                    break;
            //                                }

            //                            case "水":
            //                                {
            //                                    if (!is_adaptable_in_water & !is_trans_available_on_water & !is_trans_available_in_water)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }

            //                            case "深水":
            //                                {
            //                                    if (!is_trans_available_on_water & !is_trans_available_in_water)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }

            //                            case "宇宙":
            //                                {
            //                                    if (!is_adaptable_in_space)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        break;
            //                    }

            //                case "水上":
            //                    {
            //                        switch (TerrainClass(i, j) ?? "")
            //                        {
            //                            case "空":
            //                                {
            //                                    MaskData[i, j] = true;
            //                                    break;
            //                                }

            //                            case "宇宙":
            //                                {
            //                                    if (!is_adaptable_in_space)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        break;
            //                    }

            //                case "水中":
            //                    {
            //                        switch (TerrainClass(i, j) ?? "")
            //                        {
            //                            case "空":
            //                                {
            //                                    MaskData[i, j] = true;
            //                                    break;
            //                                }

            //                            case "深水":
            //                                {
            //                                    if (!is_trans_available_on_water & !is_trans_available_in_water)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }

            //                            case "宇宙":
            //                                {
            //                                    if (!is_adaptable_in_space)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        break;
            //                    }

            //                case "空中":
            //                    {
            //                        switch (TerrainClass(i, j) ?? "")
            //                        {
            //                            case "空":
            //                                {
            //                                    if (TerrainMoveCost(i, j) > 100)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }

            //                            case "宇宙":
            //                                {
            //                                    if (!is_adaptable_in_space)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        break;
            //                    }

            //                case "地中":
            //                    {
            //                        if (TerrainClass(i, j) != "陸")
            //                        {
            //                            MaskData[i, j] = true;
            //                        }

            //                        break;
            //                    }

            //                case "宇宙":
            //                    {
            //                        switch (TerrainClass(i, j) ?? "")
            //                        {
            //                            case "陸":
            //                            case "屋内":
            //                                {
            //                                    if (!is_trans_available_in_sky & !is_trans_available_on_ground)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }

            //                            case "空":
            //                                {
            //                                    if (!is_trans_available_in_sky | TerrainMoveCost(i, j) > 10)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }

            //                            case "水":
            //                                {
            //                                    if (!is_trans_available_in_water & !is_trans_available_on_water & !is_adaptable_in_water)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }

            //                            case "深水":
            //                                {
            //                                    if (!is_trans_available_on_water & !is_trans_available_in_water)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        break;
            //                    }
            //            }

            //            // 移動制限
            //            if (Information.UBound(allowed_terrains) > 0)
            //            {
            //                var loopTo65 = Information.UBound(allowed_terrains);
            //                for (k = 2; k <= loopTo65; k++)
            //                {
            //                    if ((TerrainName(i, j) ?? "") == (allowed_terrains[k] ?? ""))
            //                    {
            //                        break;
            //                    }
            //                }

            //                if (k > Information.UBound(allowed_terrains))
            //                {
            //                    MaskData[i, j] = true;
            //                }
            //            }

            //            // 進入不可
            //            var loopTo66 = Information.UBound(prohibited_terrains);
            //            for (k = 2; k <= loopTo66; k++)
            //            {
            //                if ((TerrainName(i, j) ?? "") == (prohibited_terrains[k] ?? ""))
            //                {
            //                    MaskData[i, j] = true;
            //                    break;
            //                }
            //            }

            //        NextLoop2:
            //            ;
            //        }
            //    }
            //}

            // 現在いる場所は常に進入可能
            MaskData[currentUnit.x, currentUnit.y] = false;
        }

        private void FillMoveCost(Unit u, int[,] move_cost, string move_area, int x1, int y1, int x2, int y2)
        {
            // 移動能力の可否を調べておく
            var is_trans_available_on_ground = u.IsTransAvailable("陸") && u.get_Adaption(2) != 0;
            var is_trans_available_in_water = u.IsTransAvailable("水") && u.get_Adaption(3) != 0;
            var is_adaptable_in_water = Strings.Mid(u.Data.Adaption, 3, 1) != "-" || u.IsFeatureAvailable("水中移動");
            var is_adaptable_in_space = Strings.Mid(u.Data.Adaption, 4, 1) != "-" || u.IsFeatureAvailable("宇宙移動");
            var is_trans_available_on_water = u.IsFeatureAvailable("水上移動") || u.IsFeatureAvailable("ホバー移動");
            var adopted_terrain = GeneralLib.ToL(u.FeatureData("地形適応")).Skip(1).ToList();

            x1 = Math.Max(1, x1);
            x2 = Math.Min(MapWidth, x2);
            y1 = Math.Max(1, y1);
            y2 = Math.Min(MapHeight, y2);

            // 各地形の移動コストを算出しておく
            switch (move_area ?? "")
            {
                case "空中":
                    for (var i = x1; i <= x2; i++)
                    {
                        for (var j = y1; j <= y2; j++)
                        {
                            switch (Terrain(i, j).Class ?? "")
                            {
                                case "空":
                                    move_cost[i, j] = Terrain(i, j).MoveCost;
                                    break;

                                case "宇宙":
                                    if (is_adaptable_in_space)
                                    {
                                        move_cost[i, j] = Terrain(i, j).MoveCost;
                                        for (var k = 0; k < adopted_terrain.Count; k++)
                                        {
                                            if ((Terrain(i, j).Name ?? "") == (adopted_terrain[k] ?? ""))
                                            {
                                                move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        move_cost[i, j] = 1000000;
                                    }

                                    break;

                                default:
                                    move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                                    break;
                            }
                        }
                    }

                    break;


                // XXX とりあえず地上で
                case "地上":
                default:
                    for (var x = x1; x <= x2; x++)
                    {
                        for (var y = y1; y <= y2; y++)
                        {
                            move_cost[x, y] = MapData[x, y].Terrain.MoveCost;
                            //switch (MapData[x, y].TerrainClass)
                            //{
                            //    case "陸":
                            //    case "屋内":
                            //    case "月面":
                            //        if (is_trans_available_on_ground)
                            //        {
                            //            move_cost[x, y] = TerrainMoveCost(x, y);
                            //            var loopTo9 = Information.UBound(adopted_terrain);
                            //            for (k = 1; k <= loopTo9; k++)
                            //            {
                            //                if ((TerrainName(x, y) ?? "") == (adopted_terrain[k] ?? ""))
                            //                {
                            //                    move_cost[x, y] = Math.Min(move_cost[x, y], 2);
                            //                    break;
                            //                }
                            //            }
                            //        }
                            //        else
                            //        {
                            //            move_cost[x, y] = 1000000;
                            //        }

                            //        break;

                            //    case "水":
                            //        if (is_trans_available_in_water | is_trans_available_on_water)
                            //        {
                            //            move_cost[x, y] = 2;
                            //        }
                            //        else if (is_adaptable_in_water)
                            //        {
                            //            move_cost[x, y] = TerrainMoveCost(x, y);
                            //            var loopTo10 = Information.UBound(adopted_terrain);
                            //            for (k = 1; k <= loopTo10; k++)
                            //            {
                            //                if ((TerrainName(x, y) ?? "") == (adopted_terrain[k] ?? ""))
                            //                {
                            //                    move_cost[x, y] = Math.Min(move_cost[x, y], 2);
                            //                    break;
                            //                }
                            //            }
                            //        }
                            //        else
                            //        {
                            //            move_cost[x, y] = 1000000;
                            //        }

                            //        break;

                            //    case "深水":
                            //        if (is_trans_available_in_water | is_trans_available_on_water)
                            //        {
                            //            move_cost[x, y] = 2;
                            //        }
                            //        else if (is_swimable)
                            //        {
                            //            move_cost[x, y] = TerrainMoveCost(x, y);
                            //        }
                            //        else
                            //        {
                            //            move_cost[x, y] = 1000000;
                            //        }

                            //        break;

                            //    case "空":
                            //        move_cost[x, y] = 1000000;
                            //        break;

                            //    case "宇宙":
                            //        if (is_adaptable_in_space)
                            //        {
                            //            move_cost[x, y] = TerrainMoveCost(x, y);
                            //            var loopTo11 = Information.UBound(adopted_terrain);
                            //            for (k = 1; k <= loopTo11; k++)
                            //            {
                            //                if ((TerrainName(x, y) ?? "") == (adopted_terrain[k] ?? ""))
                            //                {
                            //                    move_cost[x, y] = Math.Min(move_cost[x, y], 2);
                            //                    break;
                            //                }
                            //            }
                            //        }
                            //        else
                            //        {
                            //            move_cost[x, y] = 1000000;
                            //        }

                            //        break;
                            //}
                        }
                    }

                    break;

                    //case "水上":
                    //    {
                    //        var loopTo12 = x2;
                    //        for (i = x1; i <= loopTo12; i++)
                    //        {
                    //            var loopTo13 = y2;
                    //            for (j = y1; j <= loopTo13; j++)
                    //            {
                    //                switch (TerrainClass(i, j) ?? "")
                    //                {
                    //                    case "陸":
                    //                    case "屋内":
                    //                    case "月面":
                    //                        {
                    //                            if (is_trans_available_on_ground)
                    //                            {
                    //                                move_cost[i, j] = TerrainMoveCost(i, j);
                    //                                var loopTo14 = Information.UBound(adopted_terrain);
                    //                                for (k = 1; k <= loopTo14; k++)
                    //                                {
                    //                                    if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
                    //                                    {
                    //                                        move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                    //                                        break;
                    //                                    }
                    //                                }
                    //                            }
                    //                            else
                    //                            {
                    //                                move_cost[i, j] = 1000000;
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "水":
                    //                    case "深水":
                    //                        {
                    //                            move_cost[i, j] = 2;
                    //                            break;
                    //                        }

                    //                    case "空":
                    //                        {
                    //                            move_cost[i, j] = 1000000;
                    //                            break;
                    //                        }

                    //                    case "宇宙":
                    //                        {
                    //                            if (is_adaptable_in_space)
                    //                            {
                    //                                move_cost[i, j] = TerrainMoveCost(i, j);
                    //                                var loopTo15 = Information.UBound(adopted_terrain);
                    //                                for (k = 1; k <= loopTo15; k++)
                    //                                {
                    //                                    if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
                    //                                    {
                    //                                        move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                    //                                        break;
                    //                                    }
                    //                                }
                    //                            }
                    //                            else
                    //                            {
                    //                                move_cost[i, j] = 1000000;
                    //                            }

                    //                            break;
                    //                        }
                    //                }
                    //            }
                    //        }

                    //        break;
                    //    }

                    //case "水中":
                    //    {
                    //        var loopTo16 = x2;
                    //        for (i = x1; i <= loopTo16; i++)
                    //        {
                    //            var loopTo17 = y2;
                    //            for (j = y1; j <= loopTo17; j++)
                    //            {
                    //                switch (TerrainClass(i, j) ?? "")
                    //                {
                    //                    case "陸":
                    //                    case "屋内":
                    //                    case "月面":
                    //                        {
                    //                            if (is_trans_available_on_ground)
                    //                            {
                    //                                move_cost[i, j] = TerrainMoveCost(i, j);
                    //                                var loopTo18 = Information.UBound(adopted_terrain);
                    //                                for (k = 1; k <= loopTo18; k++)
                    //                                {
                    //                                    if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
                    //                                    {
                    //                                        move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                    //                                        break;
                    //                                    }
                    //                                }
                    //                            }
                    //                            else
                    //                            {
                    //                                move_cost[i, j] = 1000000;
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "水":
                    //                        {
                    //                            if (is_trans_available_in_water)
                    //                            {
                    //                                move_cost[i, j] = 2;
                    //                            }
                    //                            else
                    //                            {
                    //                                move_cost[i, j] = TerrainMoveCost(i, j);
                    //                                var loopTo19 = Information.UBound(adopted_terrain);
                    //                                for (k = 1; k <= loopTo19; k++)
                    //                                {
                    //                                    if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
                    //                                    {
                    //                                        move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                    //                                        break;
                    //                                    }
                    //                                }
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "深水":
                    //                        {
                    //                            if (is_trans_available_in_water)
                    //                            {
                    //                                move_cost[i, j] = 2;
                    //                            }
                    //                            else if (is_swimable)
                    //                            {
                    //                                move_cost[i, j] = TerrainMoveCost(i, j);
                    //                            }
                    //                            else
                    //                            {
                    //                                move_cost[i, j] = 1000000;
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "空":
                    //                        {
                    //                            move_cost[i, j] = 1000000;
                    //                            break;
                    //                        }

                    //                    case "宇宙":
                    //                        {
                    //                            if (is_adaptable_in_space)
                    //                            {
                    //                                move_cost[i, j] = TerrainMoveCost(i, j);
                    //                                var loopTo20 = Information.UBound(adopted_terrain);
                    //                                for (k = 1; k <= loopTo20; k++)
                    //                                {
                    //                                    if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
                    //                                    {
                    //                                        move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                    //                                        break;
                    //                                    }
                    //                                }
                    //                            }
                    //                            else
                    //                            {
                    //                                move_cost[i, j] = 1000000;
                    //                            }

                    //                            break;
                    //                        }
                    //                }
                    //            }
                    //        }

                    //        break;
                    //    }

                    //case "宇宙":
                    //    {
                    //        var loopTo21 = x2;
                    //        for (var i = x1; i <= loopTo21; i++)
                    //        {
                    //            var loopTo22 = y2;
                    //            for (var j = y1; j <= loopTo22; j++)
                    //            {
                    //                switch (TerrainClass(i, j) ?? "")
                    //                {
                    //                    case "宇宙":
                    //                        {
                    //                            move_cost[i, j] = TerrainMoveCost(i, j);
                    //                            var loopTo23 = Information.UBound(adopted_terrain);
                    //                            for (k = 1; k <= loopTo23; k++)
                    //                            {
                    //                                if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
                    //                                {
                    //                                    move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                    //                                    break;
                    //                                }
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "陸":
                    //                    case "屋内":
                    //                        {
                    //                            if (is_trans_available_in_sky)
                    //                            {
                    //                                move_cost[i, j] = 2;
                    //                            }
                    //                            else if (is_trans_available_on_ground)
                    //                            {
                    //                                move_cost[i, j] = TerrainMoveCost(i, j);
                    //                                var loopTo24 = Information.UBound(adopted_terrain);
                    //                                for (k = 1; k <= loopTo24; k++)
                    //                                {
                    //                                    if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
                    //                                    {
                    //                                        move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                    //                                        break;
                    //                                    }
                    //                                }
                    //                            }
                    //                            else
                    //                            {
                    //                                move_cost[i, j] = 1000000;
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "月面":
                    //                        {
                    //                            if (is_trans_available_in_moon_sky)
                    //                            {
                    //                                move_cost[i, j] = 2;
                    //                            }
                    //                            else if (is_trans_available_on_ground)
                    //                            {
                    //                                move_cost[i, j] = TerrainMoveCost(i, j);
                    //                                var loopTo25 = Information.UBound(adopted_terrain);
                    //                                for (k = 1; k <= loopTo25; k++)
                    //                                {
                    //                                    if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
                    //                                    {
                    //                                        move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                    //                                        break;
                    //                                    }
                    //                                }
                    //                            }
                    //                            else
                    //                            {
                    //                                move_cost[i, j] = 1000000;
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "水":
                    //                        {
                    //                            if (is_trans_available_in_water | is_trans_available_on_water)
                    //                            {
                    //                                move_cost[i, j] = 2;
                    //                            }
                    //                            else if (is_adaptable_in_water)
                    //                            {
                    //                                move_cost[i, j] = TerrainMoveCost(i, j);
                    //                                var loopTo26 = Information.UBound(adopted_terrain);
                    //                                for (k = 1; k <= loopTo26; k++)
                    //                                {
                    //                                    if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
                    //                                    {
                    //                                        move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                    //                                        break;
                    //                                    }
                    //                                }
                    //                            }
                    //                            else
                    //                            {
                    //                                move_cost[i, j] = 1000000;
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "深水":
                    //                        {
                    //                            if (is_trans_available_in_water | is_trans_available_on_water)
                    //                            {
                    //                                move_cost[i, j] = 2;
                    //                            }
                    //                            else if (is_swimable)
                    //                            {
                    //                                move_cost[i, j] = TerrainMoveCost(i, j);
                    //                            }
                    //                            else
                    //                            {
                    //                                move_cost[i, j] = 1000000;
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "空":
                    //                        {
                    //                            if (is_trans_available_in_sky)
                    //                            {
                    //                                move_cost[i, j] = TerrainMoveCost(i, j);
                    //                                var loopTo27 = Information.UBound(adopted_terrain);
                    //                                for (k = 1; k <= loopTo27; k++)
                    //                                {
                    //                                    if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
                    //                                    {
                    //                                        move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                    //                                        break;
                    //                                    }
                    //                                }
                    //                            }
                    //                            else
                    //                            {
                    //                                move_cost[i, j] = 1000000;
                    //                            }

                    //                            break;
                    //                        }
                    //                }
                    //            }
                    //        }

                    //        break;
                    //    }

                    //case "地中":
                    //    {
                    //        var loopTo28 = x2;
                    //        for (var i = x1; i <= loopTo28; i++)
                    //        {
                    //            var loopTo29 = y2;
                    //            for (var j = y1; j <= loopTo29; j++)
                    //            {
                    //                switch (TerrainClass(i, j) ?? "")
                    //                {
                    //                    case "陸":
                    //                    case "月面":
                    //                        {
                    //                            move_cost[i, j] = 2;
                    //                            break;
                    //                        }

                    //                    default:
                    //                        {
                    //                            move_cost[i, j] = 1000000;
                    //                            break;
                    //                        }
                    //                }
                    //            }
                    //        }

                    //        break;
                    //    }
            }

            //// 線路移動
            //if (currentUnit.IsFeatureAvailable("線路移動"))
            //{
            //    if (currentUnit.Area == "地上" & !ByJump)
            //    {
            //        var loopTo30 = x2;
            //        for (i = x1; i <= loopTo30; i++)
            //        {
            //            var loopTo31 = y2;
            //            for (j = y1; j <= loopTo31; j++)
            //            {
            //                if (TerrainName(i, j) == "線路")
            //                {
            //                    move_cost[i, j] = 2;
            //                }
            //                else
            //                {
            //                    move_cost[i, j] = 1000000;
            //                }
            //            }
            //        }
            //    }
            //}

            //// 移動制限
            //allowed_terrains = new string[1];
            //if (currentUnit.IsFeatureAvailable("移動制限"))
            //{
            //    if (currentUnit.Area != "空中" & currentUnit.Area != "地中")
            //    {
            //        n = GeneralLib.LLength(currentUnit.FeatureData("移動制限"));
            //        allowed_terrains = new string[(n + 1)];
            //        var loopTo32 = n;
            //        for (i = 2; i <= loopTo32; i++)
            //        {
            //            allowed_terrains[i] = GeneralLib.LIndex(currentUnit.FeatureData("移動制限"), i);
            //        }

            //        if (!ByJump)
            //        {
            //            var loopTo33 = x2;
            //            for (i = x1; i <= loopTo33; i++)
            //            {
            //                var loopTo34 = y2;
            //                for (j = y1; j <= loopTo34; j++)
            //                {
            //                    var loopTo35 = n;
            //                    for (k = 2; k <= loopTo35; k++)
            //                    {
            //                        if ((TerrainName(i, j) ?? "") == (allowed_terrains[k] ?? ""))
            //                        {
            //                            break;
            //                        }
            //                    }

            //                    if (k > n)
            //                    {
            //                        move_cost[i, j] = 1000000;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            //// 進入不可
            //prohibited_terrains = new string[1];
            //if (currentUnit.IsFeatureAvailable("進入不可"))
            //{
            //    if (currentUnit.Area != "空中" & currentUnit.Area != "地中")
            //    {
            //        n = GeneralLib.LLength(currentUnit.FeatureData("進入不可"));
            //        prohibited_terrains = new string[(n + 1)];
            //        var loopTo36 = n;
            //        for (i = 2; i <= loopTo36; i++)
            //        {
            //            prohibited_terrains[i] = GeneralLib.LIndex(currentUnit.FeatureData("進入不可"), i);
            //        }

            //        if (!ByJump)
            //        {
            //            var loopTo37 = x2;
            //            for (i = x1; i <= loopTo37; i++)
            //            {
            //                var loopTo38 = y2;
            //                for (j = y1; j <= loopTo38; j++)
            //                {
            //                    var loopTo39 = n;
            //                    for (k = 2; k <= loopTo39; k++)
            //                    {
            //                        if ((TerrainName(i, j) ?? "") == (prohibited_terrains[k] ?? ""))
            //                        {
            //                            move_cost[i, j] = 1000000;
            //                            break;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            //// ホバー移動
            //if (currentUnit.IsFeatureAvailable("ホバー移動"))
            //{
            //    if (move_area == "地上" | move_area == "水上")
            //    {
            //        var loopTo40 = x2;
            //        for (i = x1; i <= loopTo40; i++)
            //        {
            //            var loopTo41 = y2;
            //            for (j = y1; j <= loopTo41; j++)
            //            {
            //                switch (TerrainName(i, j) ?? "")
            //                {
            //                    case "砂漠":
            //                    case "雪原":
            //                        {
            //                            move_cost[i, j] = 2;
            //                            break;
            //                        }
            //                }
            //            }
            //        }
            //    }
            //}

            //// 透過移動
            //if (currentUnit.IsFeatureAvailable("透過移動") | currentUnit.IsUnderSpecialPowerEffect("透過移動"))
            //{
            //    var loopTo42 = x2;
            //    for (i = x1; i <= loopTo42; i++)
            //    {
            //        var loopTo43 = y2;
            //        for (j = y1; j <= loopTo43; j++)
            //            move_cost[i, j] = 2;
            //    }
            //}
        }

        // ユニット u がテレポートして移動できる範囲を選択
        // 最大距離 lv を指定可能。(省略時は移動力＋テレポートレベル)
        public void AreaInTeleport(Unit u, int lv = 0)
        {
            throw new NotImplementedException();
            //bool is_trans_available_on_ground;
            //bool is_trans_available_in_water;
            //var is_trans_available_on_water = default(bool);
            //bool is_trans_available_in_sky;
            //var is_adaptable_in_water = default(bool);
            //var is_adaptable_in_space = default(bool);
            //string[] allowed_terrains;
            //string[] prohibited_terrains;
            //int n, j, i, k, r;
            //string buf;
            //Unit u2;
            //// 移動能力の可否を調べておく
            //is_trans_available_on_ground = u.IsTransAvailable("陸") & u.get_Adaption(2) != 0;
            //is_trans_available_in_water = u.IsTransAvailable("水") & u.get_Adaption(3) != 0;
            //is_trans_available_in_sky = u.IsTransAvailable("空") & u.get_Adaption(1) != 0;
            //if (Strings.Mid(u.Data.Adaption, 3, 1) != "-" | u.IsFeatureAvailable("水中移動"))
            //{
            //    is_adaptable_in_water = true;
            //}

            //if (Strings.Mid(u.Data.Adaption, 4, 1) != "-" | u.IsFeatureAvailable("宇宙移動"))
            //{
            //    is_adaptable_in_space = true;
            //}

            //if (u.IsFeatureAvailable("水上移動") | u.IsFeatureAvailable("ホバー移動"))
            //{
            //    is_trans_available_on_water = true;
            //}

            //// 移動制限
            //allowed_terrains = new string[1];
            //if (u.IsFeatureAvailable("移動制限"))
            //{
            //    if (u.Area != "空中" & u.Area != "地中")
            //    {
            //        n = GeneralLib.LLength(u.FeatureData("移動制限"));
            //        allowed_terrains = new string[(n + 1)];
            //        var loopTo = n;
            //        for (i = 2; i <= loopTo; i++)
            //        {
            //            allowed_terrains[i] = GeneralLib.LIndex(u.FeatureData("移動制限"), i);
            //        }
            //    }
            //}

            //// 進入不可
            //prohibited_terrains = new string[1];
            //if (u.IsFeatureAvailable("進入不可"))
            //{
            //    if (u.Area != "空中" & u.Area != "地中")
            //    {
            //        n = GeneralLib.LLength(u.FeatureData("進入不可"));
            //        prohibited_terrains = new string[(n + 1)];
            //        var loopTo1 = n;
            //        for (i = 2; i <= loopTo1; i++)
            //        {
            //            prohibited_terrains[i] = GeneralLib.LIndex(u.FeatureData("進入不可"), i);
            //        }
            //    }
            //}

            //// テレポートによる移動距離を算出
            //if (lv > 0)
            //{
            //    r = lv;
            //}
            //else
            //{
            //    r = (u.Speed + u.FeatureLevel("テレポート"));
            //}

            //if (u.IsConditionSatisfied("移動不能"))
            //{
            //    r = 0;
            //}

            //// 選択解除
            //var loopTo2 = MapWidth;
            //for (i = 1; i <= loopTo2; i++)
            //{
            //    var loopTo3 = MapHeight;
            //    for (j = 1; j <= loopTo3; j++)
            //        MaskData[i, j] = true;
            //}

            //// 移動可能な地点を調べる
            //var loopTo4 = Math.Min(MapWidth, u.x + r);
            //for (i = Math.Max(1, u.x - r); i <= loopTo4; i++)
            //{
            //    var loopTo5 = Math.Min(MapHeight, u.y + r);
            //    for (j = Math.Max(1, u.y - r); j <= loopTo5; j++)
            //    {
            //        // 移動範囲内？
            //        if ((Math.Abs((u.x - i)) + Math.Abs((u.y - j))) > r)
            //        {
            //            goto NextLoop;
            //        }

            //        u2 = MapDataForUnit[i, j];
            //        if (u2 is null)
            //        {
            //            // ユニットがいない地点は地形から進入可能かチェック
            //            MaskData[i, j] = false;
            //            switch (u.Area ?? "")
            //            {
            //                case "地上":
            //                    {
            //                        switch (TerrainClass(i, j) ?? "")
            //                        {
            //                            case "空":
            //                                {
            //                                    MaskData[i, j] = true;
            //                                    break;
            //                                }

            //                            case "水":
            //                                {
            //                                    if (!is_adaptable_in_water & !is_trans_available_on_water & !is_trans_available_in_water)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }

            //                            case "深水":
            //                                {
            //                                    if (!is_trans_available_on_water & !is_trans_available_in_water)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }

            //                            case "宇宙":
            //                                {
            //                                    if (!is_adaptable_in_space)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        break;
            //                    }

            //                case "水中":
            //                    {
            //                        switch (TerrainClass(i, j) ?? "")
            //                        {
            //                            case "空":
            //                                {
            //                                    MaskData[i, j] = true;
            //                                    break;
            //                                }

            //                            case "深水":
            //                                {
            //                                    if (!is_trans_available_on_water & !is_trans_available_in_water)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }

            //                            case "宇宙":
            //                                {
            //                                    if (!is_adaptable_in_space)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        break;
            //                    }

            //                case "空中":
            //                    {
            //                        switch (TerrainClass(i, j) ?? "")
            //                        {
            //                            case "空":
            //                                {
            //                                    if (TerrainMoveCost(i, j) > 100)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }

            //                            case "宇宙":
            //                                {
            //                                    if (!is_adaptable_in_space)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        break;
            //                    }

            //                case "地中":
            //                    {
            //                        if (TerrainClass(i, j) != "陸")
            //                        {
            //                            MaskData[i, j] = true;
            //                        }

            //                        break;
            //                    }

            //                case "宇宙":
            //                    {
            //                        switch (TerrainClass(i, j) ?? "")
            //                        {
            //                            case "陸":
            //                            case "屋内":
            //                                {
            //                                    if (!is_trans_available_in_sky & !is_trans_available_on_ground)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }

            //                            case "空":
            //                                {
            //                                    if (!is_trans_available_in_sky | TerrainMoveCost(i, j) > 100)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }

            //                            case "水":
            //                                {
            //                                    if (!is_trans_available_in_water & !is_trans_available_on_water & !is_adaptable_in_water)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }

            //                            case "深水":
            //                                {
            //                                    if (!is_trans_available_on_water & !is_trans_available_in_water)
            //                                    {
            //                                        MaskData[i, j] = true;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        break;
            //                    }
            //            }

            //            // 移動制限
            //            if (Information.UBound(allowed_terrains) > 0)
            //            {
            //                var loopTo6 = Information.UBound(allowed_terrains);
            //                for (k = 2; k <= loopTo6; k++)
            //                {
            //                    if ((TerrainName(i, j) ?? "") == (allowed_terrains[k] ?? ""))
            //                    {
            //                        break;
            //                    }
            //                }

            //                if (k > Information.UBound(allowed_terrains))
            //                {
            //                    MaskData[i, j] = true;
            //                }
            //            }

            //            // 進入不可
            //            var loopTo7 = Information.UBound(prohibited_terrains);
            //            for (k = 2; k <= loopTo7; k++)
            //            {
            //                if ((TerrainName(i, j) ?? "") == (prohibited_terrains[k] ?? ""))
            //                {
            //                    MaskData[i, j] = true;
            //                    break;
            //                }
            //            }

            //            goto NextLoop;
            //        }

            //        // 合体＆着艦するのは味方のみ
            //        if (u.Party0 != "味方")
            //        {
            //            goto NextLoop;
            //        }

            //        switch (u2.Party0 ?? "")
            //        {
            //            case "味方":
            //                {
            //                    if (u2.IsFeatureAvailable("母艦"))
            //                    {
            //                        // 母艦に着艦？
            //                        if (!u.IsFeatureAvailable("母艦") & !u.IsFeatureAvailable("格納不可") & u2.Area != "地中" & !u2.IsDisabled("母艦"))
            //                        {
            //                            MaskData[i, j] = false;
            //                        }
            //                    }
            //                    else if (u.IsFeatureAvailable("合体") & u2.IsFeatureAvailable("合体"))
            //                    {
            //                        // ２体合体？
            //                        MaskData[i, j] = true;
            //                        var loopTo8 = u.CountFeature();
            //                        for (k = 1; k <= loopTo8; k++)
            //                        {
            //                            string localFeature() { object argIndex1 = k; var ret = u.Feature(argIndex1); return ret; }

            //                            string localFeatureName() { object argIndex1 = k; var ret = u.FeatureName(argIndex1); return ret; }

            //                            if (localFeature() == "合体" & !string.IsNullOrEmpty(localFeatureName()))
            //                            {
            //                                buf = u.FeatureData(k);
            //                                bool localIsDefined() { object argIndex1 = GeneralLib.LIndex(buf, 2); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //                                bool localIsDefined1() { object argIndex1 = GeneralLib.LIndex(buf, 3); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //                                if (GeneralLib.LLength(buf) == 3 & localIsDefined() & localIsDefined1())
            //                                {
            //                                    {
            //                                        var withBlock = SRC.UList.Item(GeneralLib.LIndex(buf, 2));
            //                                        if (withBlock.IsConditionSatisfied("行動不能"))
            //                                        {
            //                                            break;
            //                                        }

            //                                        if (withBlock.Status == "破棄")
            //                                        {
            //                                            break;
            //                                        }
            //                                    }

            //                                    Unit localItem() { object argIndex1 = GeneralLib.LIndex(buf, 3); var ret = SRC.UList.Item(argIndex1); return ret; }

            //                                    if ((u2.Name ?? "") == (GeneralLib.LIndex(buf, 3) ?? ""))
            //                                    {
            //                                        MaskData[i, j] = false;
            //                                        break;
            //                                    }
            //                                    else if ((u2.Name ?? "") == (localItem().CurrentForm().Name ?? "") & !u2.IsFeatureAvailable("合体制限"))
            //                                    {
            //                                        MaskData[i, j] = false;
            //                                        break;
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }

            //                    break;
            //                }

            //            case "ＮＰＣ":
            //                {
            //                    if (u.IsFeatureAvailable("合体") & u2.IsFeatureAvailable("合体"))
            //                    {
            //                        // ２体合体？
            //                        MaskData[i, j] = true;
            //                        var loopTo9 = u.CountFeature();
            //                        for (k = 1; k <= loopTo9; k++)
            //                        {
            //                            if (u.Feature(k) == "合体")
            //                            {
            //                                buf = u.FeatureData(k);
            //                                bool localIsDefined2() { object argIndex1 = GeneralLib.LIndex(buf, 2); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //                                bool localIsDefined3() { object argIndex1 = GeneralLib.LIndex(buf, 3); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //                                if (GeneralLib.LLength(buf) == 3 & localIsDefined2() & localIsDefined3())
            //                                {
            //                                    {
            //                                        var withBlock1 = SRC.UList.Item(GeneralLib.LIndex(buf, 2));
            //                                        if (withBlock1.IsConditionSatisfied("行動不能"))
            //                                        {
            //                                            break;
            //                                        }

            //                                        if (withBlock1.Status == "破棄")
            //                                        {
            //                                            break;
            //                                        }
            //                                    }

            //                                    Unit localItem1() { object argIndex1 = GeneralLib.LIndex(buf, 3); var ret = SRC.UList.Item(argIndex1); return ret; }

            //                                    if ((u2.Name ?? "") == (GeneralLib.LIndex(buf, 3) ?? ""))
            //                                    {
            //                                        MaskData[i, j] = false;
            //                                        break;
            //                                    }
            //                                    else if ((u2.Name ?? "") == (localItem1().CurrentForm().Name ?? "") & !u2.IsFeatureAvailable("合体制限"))
            //                                    {
            //                                        MaskData[i, j] = false;
            //                                        break;
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }

            //                    break;
            //                }
            //        }

            //    NextLoop:
            //        ;
            //    }
            //}

            //// 現在いる場所は常に進入可能
            //MaskData[u.x, u.y] = false;
        }

        // ユニット u のＭ移武器、アビリティのターゲット座標選択用
        public void AreaInMoveAction(Unit u, int max_range)
        {
            throw new NotImplementedException();
            //int k, i, j, n;
            //// ADD START MARGE
            //string buf;
            //// ADD END MARGE
            //bool is_trans_available_on_ground;
            //bool is_trans_available_in_water;
            //var is_trans_available_on_water = default(bool);
            //bool is_trans_available_in_sky;
            //var is_adaptable_in_water = default(bool);
            //var is_adaptable_in_space = default(bool);
            //var is_able_to_penetrate = default(bool);
            //// ADD START MARGE
            //string[] adopted_terrain;
            //// ADD END MARGE
            //string[] allowed_terrains;
            //string[] prohibited_terrains;
            //int x1, y1;
            //int x2, y2;
            //// ADD START MARGE
            //TerrainData td;
            //// ADD END MARGE

            //// 全領域マスク
            //var loopTo = MapWidth;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    var loopTo1 = MapHeight;
            //    for (j = 1; j <= loopTo1; j++)
            //        MaskData[i, j] = true;
            //}

            //// 移動能力の可否を調べておく
            //is_trans_available_on_ground = u.IsTransAvailable("陸") & u.get_Adaption(2) != 0;
            //is_trans_available_in_water = u.IsTransAvailable("水") & u.get_Adaption(3) != 0;
            //is_trans_available_in_sky = u.IsTransAvailable("空") & u.get_Adaption(1) != 0;
            //if (Strings.Mid(u.Data.Adaption, 3, 1) != "-" | u.IsFeatureAvailable("水中移動"))
            //{
            //    is_adaptable_in_water = true;
            //}

            //if (Strings.Mid(u.Data.Adaption, 4, 1) != "-" | u.IsFeatureAvailable("宇宙移動"))
            //{
            //    is_adaptable_in_space = true;
            //}

            //if (u.IsFeatureAvailable("水上移動") | u.IsFeatureAvailable("ホバー移動"))
            //{
            //    is_trans_available_on_water = true;
            //}

            //if (u.IsFeatureAvailable("透過移動") | u.IsUnderSpecialPowerEffect("透過移動"))
            //{
            //    is_able_to_penetrate = true;
            //}

            //// ADD START MARGE
            //// 地形適応のある地形のリストを作成
            //adopted_terrain = new string[1];
            //if (u.IsFeatureAvailable("地形適応"))
            //{
            //    var loopTo2 = u.CountFeature();
            //    for (i = 1; i <= loopTo2; i++)
            //    {
            //        if (u.Feature(i) == "地形適応")
            //        {
            //            buf = u.FeatureData(i);
            //            if (GeneralLib.LLength(buf) == 0)
            //            {
            //                GUI.ErrorMessage("ユニット「" + u.Name + "」の地形適応能力に対応地形が指定されていません");
            //                SRC.TerminateSRC();
            //            }

            //            n = GeneralLib.LLength(buf);
            //            Array.Resize(adopted_terrain, Information.UBound(adopted_terrain) + n);
            //            var loopTo3 = n;
            //            for (j = 2; j <= loopTo3; j++)
            //                adopted_terrain[Information.UBound(adopted_terrain) - j + 2] = GeneralLib.LIndex(buf, j);
            //        }
            //    }
            //}
            //// ADD END MARGE

            //// 移動制限
            //allowed_terrains = new string[1];
            //if (u.IsFeatureAvailable("移動制限"))
            //{
            //    if (u.Area != "空中" & u.Area != "地中")
            //    {
            //        n = GeneralLib.LLength(u.FeatureData("移動制限"));
            //        allowed_terrains = new string[(n + 1)];
            //        var loopTo4 = n;
            //        for (i = 2; i <= loopTo4; i++)
            //        {
            //            allowed_terrains[i] = GeneralLib.LIndex(u.FeatureData("移動制限"), i);
            //        }
            //    }
            //}

            //// 進入不可
            //prohibited_terrains = new string[1];
            //if (u.IsFeatureAvailable("進入不可"))
            //{
            //    if (u.Area != "空中" & u.Area != "地中")
            //    {
            //        n = GeneralLib.LLength(u.FeatureData("進入不可"));
            //        prohibited_terrains = new string[(n + 1)];
            //        var loopTo5 = n;
            //        for (i = 2; i <= loopTo5; i++)
            //        {
            //            prohibited_terrains[i] = GeneralLib.LIndex(u.FeatureData("進入不可"), i);
            //        }
            //    }
            //}

            //// 移動範囲をチェックすべき領域
            //x1 = Math.Max(1, u.x - max_range);
            //y1 = Math.Max(1, u.y - max_range);
            //x2 = Math.Min(u.x + max_range, MapWidth);
            //y2 = Math.Min(u.y + max_range, MapHeight);

            //// 進入可能か判定
            //var loopTo6 = x2;
            //for (i = x1; i <= loopTo6; i++)
            //{
            //    var loopTo7 = y2;
            //    for (j = y1; j <= loopTo7; j++)
            //    {
            //        // 移動力の範囲内？
            //        if ((Math.Abs((u.x - i)) + Math.Abs((u.y - j))) > max_range)
            //        {
            //            goto NextLoop;
            //        }

            //        // ユニットが存在？
            //        if (MapDataForUnit[i, j] is object)
            //        {
            //            goto NextLoop;
            //        }

            //        // 適応あり？
            //        switch (u.Area ?? "")
            //        {
            //            case "地上":
            //                {
            //                    switch (TerrainClass(i, j) ?? "")
            //                    {
            //                        case "空":
            //                            {
            //                                goto NextLoop;
            //                                break;
            //                            }

            //                        case "水":
            //                            {
            //                                if (!is_adaptable_in_water & !is_trans_available_on_water & !is_trans_available_in_water)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }

            //                        case "深水":
            //                            {
            //                                if (!is_trans_available_on_water & !is_trans_available_in_water)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }

            //                        case "宇宙":
            //                            {
            //                                if (!is_adaptable_in_space)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }
            //                    }

            //                    break;
            //                }

            //            case "水中":
            //                {
            //                    switch (TerrainClass(i, j) ?? "")
            //                    {
            //                        case "空":
            //                            {
            //                                goto NextLoop;
            //                                break;
            //                            }

            //                        case "深水":
            //                            {
            //                                if (!is_trans_available_on_water & !is_trans_available_in_water)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }

            //                        case "宇宙":
            //                            {
            //                                if (!is_adaptable_in_space)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }
            //                    }

            //                    break;
            //                }

            //            case "空中":
            //                {
            //                    switch (TerrainClass(i, j) ?? "")
            //                    {
            //                        case "空":
            //                            {
            //                                if (TerrainMoveCost(i, j) > 100)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }

            //                        case "宇宙":
            //                            {
            //                                if (!is_adaptable_in_space)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }
            //                    }

            //                    break;
            //                }

            //            case "地中":
            //                {
            //                    if (TerrainClass(i, j) != "陸")
            //                    {
            //                        goto NextLoop;
            //                    }

            //                    break;
            //                }

            //            case "宇宙":
            //                {
            //                    switch (TerrainClass(i, j) ?? "")
            //                    {
            //                        case "陸":
            //                        case "屋内":
            //                            {
            //                                if (!is_trans_available_in_sky & !is_trans_available_on_ground)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }

            //                        case "空":
            //                            {
            //                                if (!is_trans_available_in_sky | TerrainMoveCost(i, j) > 100)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }

            //                        case "水":
            //                            {
            //                                if (!is_trans_available_in_water & !is_trans_available_on_water & !is_adaptable_in_water)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }

            //                        case "深水":
            //                            {
            //                                if (!is_trans_available_on_water & !is_trans_available_in_water)
            //                                {
            //                                    goto NextLoop;
            //                                }

            //                                break;
            //                            }
            //                    }

            //                    // 移動制限
            //                    if (Information.UBound(allowed_terrains) > 0)
            //                    {
            //                        var loopTo8 = Information.UBound(allowed_terrains);
            //                        for (k = 2; k <= loopTo8; k++)
            //                        {
            //                            if ((TerrainName(i, j) ?? "") == (allowed_terrains[k] ?? ""))
            //                            {
            //                                break;
            //                            }
            //                        }

            //                        if (k > Information.UBound(allowed_terrains))
            //                        {
            //                            goto NextLoop;
            //                        }
            //                    }

            //                    // 進入不可
            //                    var loopTo9 = Information.UBound(prohibited_terrains);
            //                    for (k = 2; k <= loopTo9; k++)
            //                    {
            //                        if ((TerrainName(i, j) ?? "") == (prohibited_terrains[k] ?? ""))
            //                        {
            //                            goto NextLoop;
            //                        }
            //                    }

            //                    break;
            //                }
            //        }

            //        // 侵入（進入）禁止地形？
            //        // MOD START 240a
            //        // Set td = TDList.Item(MapData(i, j, 0))
            //        // With td
            //        // If .IsFeatureAvailable("侵入禁止") Then
            //        // For k = 1 To UBound(adopted_terrain)
            //        // If .Name = adopted_terrain(k) Then
            //        // Exit For
            //        // End If
            //        // Next
            //        // If k > UBound(adopted_terrain) Then
            //        // GoTo NextLoop
            //        // End If
            //        // End If
            //        // End With
            //        if (TerrainDoNotEnter(i, j))
            //        {
            //            var loopTo10 = Information.UBound(adopted_terrain);
            //            for (k = 1; k <= loopTo10; k++)
            //            {
            //                if ((TerrainName(i, j) ?? "") == (adopted_terrain[k] ?? ""))
            //                {
            //                    break;
            //                }
            //            }

            //            if (k > Information.UBound(adopted_terrain))
            //            {
            //                goto NextLoop;
            //            }
            //        }
            //        // MOD START 240a

            //        // 進路上に壁がある？
            //        if (!is_able_to_penetrate)
            //        {
            //            if (IsLineBlocked(u.x, u.y, i, j, u.Area == "空中"))
            //            {
            //                goto NextLoop;
            //            }
            //        }

            //        // マスク解除
            //        MaskData[i, j] = false;
            //    NextLoop:
            //        ;
            //    }
            //}

            //// 現在いる場所は常に進入可能
            //MaskData[u.x, u.y] = false;
        }

        // ２点間を結ぶ直線が壁でブロックされているか判定
        public bool IsLineBlocked(int x1, int y1, int x2, int y2, bool is_flying = false)
        {
            throw new NotImplementedException();
            //bool IsLineBlockedRet = default;
            //int xx, yy;
            //int xx2, yy2;
            //xx = x1;
            //yy = y1;
            //if (Math.Abs((x1 - x2)) > Math.Abs((y1 - y2)))
            //{
            //    do
            //    {
            //        if (x1 > x2)
            //        {
            //            xx = (xx - 1);
            //        }
            //        else
            //        {
            //            xx = (xx + 1);
            //        }

            //        yy2 = yy;
            //        yy = (y1 + (y2 - y1) * (x1 - xx + 0d) / (x1 - x2));

            //        // 壁？
            //        if (is_flying)
            //        {
            //            if (TerrainName(xx, yy) == "壁" | TerrainName(xx, yy2) == "壁")
            //            {
            //                IsLineBlockedRet = true;
            //                return IsLineBlockedRet;
            //            }
            //        }
            //        else
            //        {
            //            switch (TerrainName(xx, yy) ?? "")
            //            {
            //                case "壁":
            //                case "防壁":
            //                    {
            //                        IsLineBlockedRet = true;
            //                        return IsLineBlockedRet;
            //                    }
            //            }

            //            switch (TerrainName(xx, yy2) ?? "")
            //            {
            //                case "壁":
            //                case "防壁":
            //                    {
            //                        IsLineBlockedRet = true;
            //                        return IsLineBlockedRet;
            //                    }
            //            }
            //        }
            //    }
            //    while (xx != x2);
            //}
            //else
            //{
            //    do
            //    {
            //        if (y1 > y2)
            //        {
            //            yy = (yy - 1);
            //        }
            //        else
            //        {
            //            yy = (yy + 1);
            //        }

            //        xx2 = xx;
            //        xx = (x1 + (x2 - x1) * (y1 - yy + 0d) / (y1 - y2));

            //        // 壁？
            //        if (is_flying)
            //        {
            //            if (TerrainName(xx, yy) == "壁" | TerrainName(xx2, yy) == "壁")
            //            {
            //                IsLineBlockedRet = true;
            //                return IsLineBlockedRet;
            //            }
            //        }
            //        else
            //        {
            //            switch (TerrainName(xx, yy) ?? "")
            //            {
            //                case "壁":
            //                case "防壁":
            //                    {
            //                        IsLineBlockedRet = true;
            //                        return IsLineBlockedRet;
            //                    }
            //            }

            //            switch (TerrainName(xx2, yy) ?? "")
            //            {
            //                case "壁":
            //                case "防壁":
            //                    {
            //                        IsLineBlockedRet = true;
            //                        return IsLineBlockedRet;
            //                    }
            //            }
            //        }
            //    }
            //    while (yy != y2);
            //}

            //IsLineBlockedRet = false;
            //return IsLineBlockedRet;
        }

        // ユニット u が (dst_x,dst_y) に行くのに最も近い移動範囲内の場所 (X,Y) はどこか検索
        public void NearestPoint(Unit u, int dst_x, int dst_y, out int X, out int Y)
        {
            X = u.x;
            Y = u.y;
            // 目的地がマップ外にならないように
            dst_x = Math.Max(Math.Min(dst_x, MapWidth), 1);
            dst_y = Math.Max(Math.Min(dst_y, MapHeight), 1);

            // XXX マップの最大サイズの制限になるはず
            var total_cost = new int[52, 52];
            var move_cost = new int[52, 52];
            var cur_speed = new int[52, 52];

            // 各地形の移動コストを算出しておく
            FillMoveCost(u, move_cost, u.Area, 0, 0, 51, 51);
            // XXX FillMoveCost で考慮する能力のすり合わせ
            // 線路移動
            // 移動制限
            // 進入不可
            // ホバー移動
            // ジャンプ移動

            var loopTo34 = (MapWidth + 1);
            for (var i = 0; i <= loopTo34; i++)
            {
                var loopTo35 = (MapHeight + 1);
                for (var j = 0; j <= loopTo35; j++)
                    total_cost[i, j] = 1000000;
            }

            total_cost[dst_x, dst_y] = 0;

            // 目的地から各地点に到達するのにかかる移動力を計算
            {
                var i = 0;
                int min_x, max_x;
                int min_y, max_y;
                bool is_changed;
                do
                {
                    i = (i + 1);

                    // タイムアウト
                    if (i > 3 * (MapWidth + MapHeight))
                    {
                        break;
                    }

                    is_changed = false;
                    var loopTo36 = (MapWidth + 1);
                    for (var j = 0; j <= loopTo36; j++)
                    {
                        var loopTo37 = (MapHeight + 1);
                        for (var k = 0; k <= loopTo37; k++)
                            cur_speed[j, k] = total_cost[j, k];
                    }

                    min_x = Math.Max(1, dst_x - i);
                    max_x = Math.Min(dst_x + i, MapWidth);
                    var loopTo38 = max_x;
                    for (var j = min_x; j <= loopTo38; j++)
                    {
                        min_y = Math.Max(1, dst_y - (i - Math.Abs((dst_x - j))));
                        max_y = Math.Min(dst_y + (i - Math.Abs((dst_x - j))), MapHeight);
                        var loopTo39 = max_y;
                        for (var k = min_y; k <= loopTo39; k++)
                        {
                            var tmp = cur_speed[j, k];
                            tmp = Math.Min(tmp, cur_speed[j - 1, k]);
                            tmp = Math.Min(tmp, cur_speed[j + 1, k]);
                            tmp = Math.Min(tmp, cur_speed[j, k - 1]);
                            tmp = Math.Min(tmp, cur_speed[j, k + 1]);
                            tmp = tmp + move_cost[j, k];
                            if (tmp < cur_speed[j, k])
                            {
                                is_changed = true;
                                total_cost[j, k] = tmp;
                            }
                        }
                    }

                    // 最短経路を発見した
                    if (total_cost[X, Y] <= Math.Abs((dst_x - X)) + Math.Abs((dst_y - Y)) + 2)
                    {
                        break;
                    }
                }
                while (is_changed);
            }

            // 移動可能範囲内で目的地に最も近い場所を見付ける
            {
                var tmp = total_cost[X, Y];
                var loopTo40 = MapWidth;
                for (var i = 1; i <= loopTo40; i++)
                {
                    var loopTo41 = MapHeight;
                    for (var j = 1; j <= loopTo41; j++)
                    {
                        // XXX MaskData の更新してない気がする
                        if (!MaskData[i, j])
                        {
                            if (MapDataForUnit[i, j] is null)
                            {
                                if (total_cost[i, j] < tmp)
                                {
                                    X = i;
                                    Y = j;
                                    tmp = total_cost[i, j];
                                }
                                else if (total_cost[i, j] == tmp)
                                {
                                    if (Math.Pow(Math.Abs((dst_x - i)), 2d) + Math.Pow(Math.Abs((dst_y - j)), 2d) < Math.Pow(Math.Abs((dst_x - X)), 2d) + Math.Pow(Math.Abs((dst_y - Y)), 2d))
                                    {
                                        X = i;
                                        Y = j;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // ユニット u が敵から最も遠くなる場所(X,Y)を検索
        public void SafetyPoint(Unit u, int X, int Y)
        {
            throw new NotImplementedException();
            //    int i, j;
            //    var total_cost = new int[52, 52];
            //    var cur_cost = new int[52, 52];
            //    int tmp;
            //    bool is_changed;

            //    // 作業用配列を初期化
            //    var loopTo = (MapWidth + 1);
            //    for (i = 0; i <= loopTo; i++)
            //    {
            //        var loopTo1 = (MapHeight + 1);
            //        for (j = 0; j <= loopTo1; j++)
            //            total_cost[i, j] = 1000000;
            //    }

            //    foreach (Unit t in SRC.UList)
            //    {
            //        if (u.IsEnemy(t))
            //        {
            //            total_cost[t.x, t.y] = 0;
            //        }
            //    }

            //    // 各地点の敵からの距離を計算
            //    do
            //    {
            //        is_changed = false;
            //        var loopTo2 = (MapWidth + 1);
            //        for (i = 0; i <= loopTo2; i++)
            //        {
            //            var loopTo3 = (MapHeight + 1);
            //            for (j = 0; j <= loopTo3; j++)
            //                cur_cost[i, j] = total_cost[i, j];
            //        }

            //        var loopTo4 = MapWidth;
            //        for (i = 1; i <= loopTo4; i++)
            //        {
            //            var loopTo5 = MapHeight;
            //            for (j = 1; j <= loopTo5; j++)
            //            {
            //                tmp = cur_cost[i, j];
            //                tmp = Math.Min(cur_cost[i - 1, j] + 1, tmp);
            //                tmp = Math.Min(cur_cost[i + 1, j] + 1, tmp);
            //                tmp = Math.Min(cur_cost[i, j - 1] + 1, tmp);
            //                tmp = Math.Min(cur_cost[i, j + 1] + 1, tmp);
            //                if (tmp < cur_cost[i, j])
            //                {
            //                    is_changed = true;
            //                    total_cost[i, j] = tmp;
            //                }
            //            }
            //        }
            //    }
            //    while (is_changed);

            //    // 移動可能範囲内で敵から最も遠い場所を見付ける
            //    tmp = 0;
            //    var loopTo6 = MapWidth;
            //    for (i = 1; i <= loopTo6; i++)
            //    {
            //        var loopTo7 = MapHeight;
            //        for (j = 1; j <= loopTo7; j++)
            //        {
            //            if (!MaskData[i, j])
            //            {
            //                if (MapDataForUnit[i, j] is null)
            //                {
            //                    if (total_cost[i, j] > tmp)
            //                    {
            //                        X = i;
            //                        Y = j;
            //                        tmp = total_cost[i, j];
            //                    }
            //                    else if (total_cost[i, j] == tmp)
            //                    {
            //                        if (Math.Pow(Math.Abs((u.x - i)), 2d) + Math.Pow(Math.Abs((u.y - j)), 2d) < Math.Pow(Math.Abs((u.x - X)), 2d) + Math.Pow(Math.Abs((u.y - Y)), 2d))
            //                        {
            //                            X = i;
            //                            Y = j;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
        }

        // 現在位置から指定した場所までの移動経路を調べる
        // 事前にAreaInSpeedを実行しておく事が必要
        public void SearchMoveRoute(int tx, int ty, out IList<int> move_route_x, out IList<int> move_route_y)
        {
            int xx, yy;
            int nx, ny;
            int ox, oy;
            int tmp;
            string direction = "";
            string prev_direction = "";
            var move_direction = new List<string>();
            move_route_x = new List<int>();
            move_route_y = new List<int>();
            move_route_x.Add(tx);
            move_route_y.Add(ty);

            // 現在位置を調べる
            ox = tx;
            oy = ty;
            for (xx = 1; xx <= MapWidth; xx++)
            {
                for (yy = 1; yy <= MapHeight; yy++)
                {
                    if (TotalMoveCost[xx, yy] == 0)
                    {
                        ox = xx;
                        oy = yy;
                    }
                }
            }

            // 現在位置のＺＯＣは無効化する
            PointInZOC[ox, oy] = 0;
            xx = tx;
            yy = ty;
            nx = tx;
            ny = ty;
            while (TotalMoveCost[xx, yy] > 0)
            {
                tmp = TotalMoveCost[xx, yy];

                // 周りの場所から最も必要移動力が低い場所を探す

                // なるべく直線方向に移動させるため、前回と同じ移動方向を優先させる
                switch (prev_direction ?? "")
                {
                    case "N":
                        {
                            if (TotalMoveCost[xx, yy - 1] < tmp & PointInZOC[xx, yy - 1] <= 0)
                            {
                                tmp = TotalMoveCost[xx, yy - 1];
                                nx = xx;
                                ny = (yy - 1);
                                direction = "N";
                            }

                            break;
                        }

                    case "S":
                        {
                            if (TotalMoveCost[xx, yy + 1] < tmp & PointInZOC[xx, yy + 1] <= 0)
                            {
                                tmp = TotalMoveCost[xx, yy + 1];
                                nx = xx;
                                ny = (yy + 1);
                                direction = "S";
                            }

                            break;
                        }

                    case "W":
                        {
                            if (TotalMoveCost[xx - 1, yy] < tmp & PointInZOC[xx - 1, yy] <= 0)
                            {
                                tmp = TotalMoveCost[xx - 1, yy];
                                nx = (xx - 1);
                                ny = yy;
                                direction = "W";
                            }

                            break;
                        }

                    case "E":
                        {
                            if (TotalMoveCost[xx + 1, yy] < tmp & PointInZOC[xx + 1, yy] <= 0)
                            {
                                tmp = TotalMoveCost[xx + 1, yy];
                                nx = (xx + 1);
                                ny = yy;
                                direction = "E";
                            }

                            break;
                        }
                }

                // なるべく目標位置付近で直進させるため、目標位置との距離差の小さい
                // 座標軸方向に優先して移動させる
                if (Math.Abs((xx - ox)) <= Math.Abs((yy - oy)))
                {
                    if (TotalMoveCost[xx, yy - 1] < tmp & PointInZOC[xx, yy - 1] <= 0)
                    {
                        tmp = TotalMoveCost[xx, yy - 1];
                        nx = xx;
                        ny = (yy - 1);
                        direction = "N";
                    }

                    if (TotalMoveCost[xx, yy + 1] < tmp & PointInZOC[xx, yy + 1] <= 0)
                    {
                        tmp = TotalMoveCost[xx, yy + 1];
                        nx = xx;
                        ny = (yy + 1);
                        direction = "S";
                    }

                    if (TotalMoveCost[xx - 1, yy] < tmp & PointInZOC[xx - 1, yy] <= 0)
                    {
                        tmp = TotalMoveCost[xx - 1, yy];
                        nx = (xx - 1);
                        ny = yy;
                        direction = "W";
                    }

                    if (TotalMoveCost[xx + 1, yy] < tmp & PointInZOC[xx + 1, yy] <= 0)
                    {
                        tmp = TotalMoveCost[xx + 1, yy];
                        nx = (xx + 1);
                        ny = yy;
                        direction = "E";
                    }
                }
                else
                {
                    if (TotalMoveCost[xx - 1, yy] < tmp & PointInZOC[xx - 1, yy] <= 0)
                    {
                        tmp = TotalMoveCost[xx - 1, yy];
                        nx = (xx - 1);
                        ny = yy;
                        direction = "W";
                    }

                    if (TotalMoveCost[xx + 1, yy] < tmp & PointInZOC[xx + 1, yy] <= 0)
                    {
                        tmp = TotalMoveCost[xx + 1, yy];
                        nx = (xx + 1);
                        ny = yy;
                        direction = "E";
                    }

                    if (TotalMoveCost[xx, yy - 1] < tmp & PointInZOC[xx, yy - 1] <= 0)
                    {
                        tmp = TotalMoveCost[xx, yy - 1];
                        nx = xx;
                        ny = (yy - 1);
                        direction = "N";
                    }

                    if (TotalMoveCost[xx, yy + 1] < tmp & PointInZOC[xx, yy + 1] <= 0)
                    {
                        tmp = TotalMoveCost[xx, yy + 1];
                        nx = xx;
                        ny = (yy + 1);
                        direction = "S";
                    }
                }

                if (nx == xx & ny == yy)
                {
                    // これ以上必要移動力が低い場所が見つからなかったので終了
                    break;
                }

                // 見つかった場所を記録
                move_route_x.Add(nx);
                move_route_y.Add(ny);

                // 移動方向を記録
                move_direction.Add(direction);
                prev_direction = direction;

                // 次回は今回見つかった場所を起点に検索する
                xx = nx;
                yy = ny;
            }

            // 直線を走った距離を計算
            Commands.MovedUnitSpeed = 1;
            for (var i = 1; i < move_direction.Count - 1; i++)
            {
                if (move_direction[i] == move_direction[i + 1])
                {
                    break;
                }

                Commands.MovedUnitSpeed = (Commands.MovedUnitSpeed + 1);
            }
        }
    }
}
