// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using Newtonsoft.Json;
using SRCCore.Exceptions;
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace SRCCore.Maps
{
    // マップデータに関する各種処理を行うモジュール
    [JsonObject(MemberSerialization.OptIn)]
    public class Map
    {
        protected SRC SRC { get; }
        private IGUI GUI => SRC.GUI;
        private IGUIMap GUIMap => SRC.GUIMap;
        private Commands.Command Commands => SRC.Commands;
        private Expressions.Expression Expression => SRC.Expression;

        public Map(SRC src)
        {
            SRC = src;
        }
        public void Restore(SRCSuspendData data)
        {
            var map = data.Map;
            // XXX シリアライズから復元するのか保存用データクラス作るのかはっきりしたほうがよさそう
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
            MapFileName = map.MapFileName;
            MapDrawMode = map.MapDrawMode;
            MapDrawIsMapOnly = map.MapDrawIsMapOnly;
            IsMapDirty = map.IsMapDirty;

            // マップ幅, マップ高さ
            var mw = map.MapWidth;
            var mh = map.MapHeight;
            SetMapSize(mw, mh);

            // 各地形

            // 表示位置
            // SetupBackgroundでMapX,MapYが書き換えられてしまうため、この位置で
            // 値を参照する必要がある。
            // TODO Impl 表示位置
            //GUI.MapX = data.MapX;
            //GUI.MapY = data.MapY;

            // ユニット配置
            foreach (Unit u in SRC.UList.Items)
            {
                if (u.Status == "出撃")
                {
                    MapDataForUnit[u.x, u.y] = u;
                }
            }

            MapData = map.MapData;
            MapData.All.ToList().ForEach(cell => cell.Restore(SRC));
        }

        // 管理可能な地形データの総数
        public const int MAX_TERRAIN_DATA_NUM = 2000;

        // マップファイル名
        [JsonProperty]
        public string MapFileName;
        // XXX
        public bool IsStatusView => string.IsNullOrEmpty(MapFileName);
        // マップの横サイズ
        [JsonProperty]
        public int MapWidth;
        // マップの縦サイズ
        [JsonProperty]
        public int MapHeight;

        // XXX この辺完全に未精査
        // マップの描画モード
        [JsonProperty]
        public string MapDrawMode;
        // フィルタ色
        [JsonProperty]
        public Color MapDrawFilterColor;
        // フィルタの透過度
        [JsonProperty]
        public double MapDrawFilterTransPercent;
        // フィルタやSepiaコマンドなどでユニットの色を変更するか
        [JsonProperty]
        public bool MapDrawIsMapOnly;

        // マップに画像の書き込みがなされたか
        public bool IsMapDirty;

        // マップデータを記録する配列
        [JsonProperty]
        public Src2DArray<MapCell> MapData;

        //public Src2DArray<MapImageFileType> MapImageFileTypeData;
        public Src2DArray<string> MapUnderImageFilePath;
        public Src2DArray<string> MapUpperImageFilePath;

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

        public IList<Unit> AdjacentUnit(Unit u)
        {
            return AdjacentUnit(u.x, u.y);
        }

        public IList<Unit> AdjacentUnit(int X, int Y)
        {
            return new Unit[]
                {
                    UnitAtPoint(X +1, Y),
                    UnitAtPoint(X -1, Y),
                    UnitAtPoint(X, Y +1),
                    UnitAtPoint(X, Y -1),
                }.Where(u => u != null).ToList();
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
                            MapUnderImageFilePath[cell.X, cell.Y] = fpath;
                            // MapUpperImageFilePath
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
            if (string.IsNullOrEmpty(fname) || !SRC.FileSystem.FileExists(fname))
            {
                // ファイルが存在しない場合
                SetupStatusView();
            }
            else
            {
                using (var stream = SRC.FileSystem.OpenText(SRC.SystemConfig.SRCCompatibilityMode, fname))
                {
                    LoadMapData(fname, stream);
                }
            }
        }

        private void SetupStatusView()
        {
            MapFileName = "";
            if (Strings.InStr(SRC.ScenarioFileName, "ステータス表示.") > 0
                || Strings.InStr(SRC.ScenarioFileName, "ランキング.") > 0)
            {
                SetMapSize(GUI.MainWidth, 40);
            }
            else
            {
                SetMapSize(GUI.MainWidth, GUI.MainHeight);
            }

            for (var i = 1; i <= MapWidth; i++)
            {
                for (var j = 1; j <= MapHeight; j++)
                {
                    MapData[i, j] = MapCell.EmptyCell;
                }
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
            MapUnderImageFilePath = new Src2DArray<string>(MapWidth, MapHeight);
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
            // XXX 1というよりは0にしたいが0長配列は作れない
            SetMapSize(1, 1);
            MapDrawMode = "";
            MapDrawIsMapOnly = false;
            MapDrawFilterColor = Color.Black;
            MapDrawFilterTransPercent = 0d;

            // マップ画面を消去
            GUI.RedrawScreen();

            // XXX
            //// ユニット画像をリセット
            //if (!string.IsNullOrEmpty(MapDrawMode) && !MapDrawIsMapOnly)
            //{
            //    SRC.UList.ClearUnitBitmap();
            //}
        }

        // (X,Y)を中心とする min_range - max_range のエリアを選択
        // エリア内のユニットは uparty の指示に従い選択
        public void AreaInRange(int X, int Y, int max_range, int min_range, string uparty)
        {
            // 選択情報をクリア
            ClearMask();

            var x1 = Math.Max(X - max_range, 1);
            var x2 = Math.Min(X + max_range, MapWidth);
            var y1 = Math.Max(Y - max_range, 1);
            var y2 = Math.Min(Y + max_range, MapHeight);

            // max_range内かつmin_range外のエリアを選択
            for (var i = x1; i <= x2; i++)
            {
                for (var j = y1; j <= y2; j++)
                {
                    var n = (Math.Abs((X - i)) + Math.Abs((Y - j)));
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
            SelectUnitInMask(uparty);

            // エリアの中心は常に選択
            MaskData[X, Y] = false;
        }

        public void ClearMask()
        {
            for (var i = 1; i <= MapWidth; i++)
            {
                for (var j = 1; j <= MapHeight; j++)
                {
                    MaskData[i, j] = true;
                }
            }
        }

        private void SelectUnitInMask(string uparty)
        {
            switch (uparty ?? "")
            {
                case "味方":
                case "ＮＰＣ":
                    for (var i = 1; i <= MapWidth; i++)
                    {
                        for (var j = 1; j <= MapWidth; j++)
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
                    for (var i = 1; i <= MapWidth; i++)
                    {
                        for (var j = 1; j <= MapWidth; j++)
                        {
                            if (!MaskData[i, j])
                            {
                                if (MapDataForUnit[i, j] is object)
                                {
                                    {
                                        var u = MapDataForUnit[i, j];
                                        if ((u.Party == "味方" || u.Party == "ＮＰＣ")
                                            && !u.IsConditionSatisfied("暴走")
                                            && !u.IsConditionSatisfied("魅了")
                                            && !u.IsConditionSatisfied("混乱")
                                            && !u.IsConditionSatisfied("憑依")
                                            && !u.IsConditionSatisfied("睡眠")
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
                    for (var i = 1; i <= MapWidth; i++)
                    {
                        for (var j = 1; j <= MapWidth; j++)
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
                    for (var i = 1; i <= MapWidth; i++)
                    {
                        for (var j = 1; j <= MapWidth; j++)
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
                    for (var i = 1; i <= MapWidth; i++)
                    {
                        for (var j = 1; j <= MapWidth; j++)
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
                    for (var i = 1; i <= MapWidth; i++)
                    {
                        for (var j = 1; j <= MapWidth; j++)
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
                    for (var i = 1; i <= MapWidth; i++)
                    {
                        for (var j = 1; j <= MapWidth; j++)
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
        }

        // ユニット u から移動後使用可能な射程 max_range の武器／アビリティを使う場合の効果範囲
        // エリア内のユニットは Party の指示に従い選択
        public void AreaInReachable(Unit u, int max_range, string uparty)
        {
            // まずは移動範囲を選択
            AreaInSpeed(u);

            // 選択範囲をmax_rangeぶんだけ拡大
            var tmp_mask_data = new bool[MapWidth + 1 + 1, MapHeight + 1 + 1];
            var loopTo = (MapWidth + 1);
            for (var i = 0; i <= loopTo; i++)
            {
                var loopTo1 = (MapHeight + 1);
                for (var j = 0; j <= loopTo1; j++)
                    tmp_mask_data[i, j] = true;
            }

            var loopTo2 = max_range;
            for (var i = 1; i <= loopTo2; i++)
            {
                var loopTo3 = MapWidth;
                for (var j = 1; j <= loopTo3; j++)
                {
                    var loopTo4 = MapHeight;
                    for (var k = 1; k <= loopTo4; k++)
                        tmp_mask_data[j, k] = MaskData[j, k];
                }

                var loopTo5 = MapWidth;
                for (var j = 1; j <= loopTo5; j++)
                {
                    var loopTo6 = MapHeight;
                    for (var k = 1; k <= loopTo6; k++)
                        MaskData[j, k] = tmp_mask_data[j, k] && tmp_mask_data[j - 1, k] && tmp_mask_data[j + 1, k] && tmp_mask_data[j, k - 1] && tmp_mask_data[j, k + 1];
                }
            }

            // エリア内のユニットを選択するかそれぞれ判定
            SelectUnitInMask(uparty);

            // エリアの中心は常に選択
            MaskData[u.x, u.y] = false;
        }

        // マップ全域に渡ってupartyに属するユニットが存在する場所を選択
        public void AreaWithUnit(string uparty)
        {
            ClearMask();
            SelectUnitInMask(uparty);
        }

        // 十字状のエリアを選択 (Ｍ直の攻撃方向選択用)
        public void AreaInCross(int X, int Y, int min_range, int max_range)
        {
            ClearMask();

            var loopTo2 = (Y - min_range);
            for (var i = (Y - max_range); i <= loopTo2; i++)
            {
                if (i >= 1)
                {
                    MaskData[X, i] = false;
                }
            }

            var loopTo3 = (Y + max_range);
            for (var i = (Y + min_range); i <= loopTo3; i++)
            {
                if (i <= MapHeight)
                {
                    MaskData[X, i] = false;
                }
            }

            var loopTo4 = (X - min_range);
            for (var i = (X - max_range); i <= loopTo4; i++)
            {
                if (i >= 1)
                {
                    MaskData[i, Y] = false;
                }
            }

            var loopTo5 = (X + max_range);
            for (var i = (X + min_range); i <= loopTo5; i++)
            {
                if (i <= MapWidth)
                {
                    MaskData[i, Y] = false;
                }
            }

            MaskData[X, Y] = false;
        }

        // 直線状のエリアを選択 (Ｍ直の攻撃範囲設定用)
        public void AreaInLine(int X, int Y, int min_range, int max_range, string direction)
        {
            ClearMask();

            switch (direction ?? "")
            {
                case "N":
                    {
                        var loopTo2 = (Y - min_range);
                        for (var i = (Y - max_range); i <= loopTo2; i++)
                        {
                            if (i >= 1)
                            {
                                MaskData[X, i] = false;
                            }
                        }

                        break;
                    }

                case "S":
                    {
                        var loopTo3 = (Y + max_range);
                        for (var i = (Y + min_range); i <= loopTo3; i++)
                        {
                            if (i <= MapHeight)
                            {
                                MaskData[X, i] = false;
                            }
                        }

                        break;
                    }

                case "W":
                    {
                        var loopTo4 = (X - min_range);
                        for (var i = (X - max_range); i <= loopTo4; i++)
                        {
                            if (i >= 1)
                            {
                                MaskData[i, Y] = false;
                            }
                        }

                        break;
                    }

                case "E":
                    {
                        var loopTo5 = (X + max_range);
                        for (var i = (X + min_range); i <= loopTo5; i++)
                        {
                            if (i <= MapWidth)
                            {
                                MaskData[i, Y] = false;
                            }
                        }

                        break;
                    }
            }

            MaskData[X, Y] = false;
        }

        // 幅３マスの十字状のエリアを選択 (Ｍ拡の攻撃方向選択用)
        public void AreaInWideCross(int X, int Y, int min_range, int max_range)
        {
            ClearMask();

            var loopTo2 = (Y - min_range);
            for (var i = (Y - max_range); i <= loopTo2; i++)
            {
                if (i >= 1)
                {
                    MaskData[X, i] = false;
                }
            }

            var loopTo3 = (Y - (min_range + 1));
            for (var i = (Y - max_range + 1); i <= loopTo3; i++)
            {
                if (i >= 1)
                {
                    if (X > 1)
                    {
                        MaskData[X - 1, i] = false;
                    }

                    if (X < MapWidth)
                    {
                        MaskData[X + 1, i] = false;
                    }
                }
            }

            var loopTo4 = (Y + max_range);
            for (var i = (Y + min_range); i <= loopTo4; i++)
            {
                if (i <= MapHeight)
                {
                    MaskData[X, i] = false;
                }
            }

            var loopTo5 = (Y + max_range - 1);
            for (var i = (Y + (min_range + 1)); i <= loopTo5; i++)
            {
                if (i <= MapHeight)
                {
                    if (X > 1)
                    {
                        MaskData[X - 1, i] = false;
                    }

                    if (X < MapWidth)
                    {
                        MaskData[X + 1, i] = false;
                    }
                }
            }

            var loopTo6 = (X - min_range);
            for (var i = (X - max_range); i <= loopTo6; i++)
            {
                if (i >= 1)
                {
                    MaskData[i, Y] = false;
                }
            }

            var loopTo7 = (X - (min_range + 1));
            for (var i = (X - max_range + 1); i <= loopTo7; i++)
            {
                if (i >= 1)
                {
                    if (Y > 1)
                    {
                        MaskData[i, Y - 1] = false;
                    }

                    if (Y < MapHeight)
                    {
                        MaskData[i, Y + 1] = false;
                    }
                }
            }

            var loopTo8 = (X + max_range);
            for (var i = (X + min_range); i <= loopTo8; i++)
            {
                if (i <= MapWidth)
                {
                    MaskData[i, Y] = false;
                }
            }

            var loopTo9 = (X + max_range - 1);
            for (var i = (X + (min_range + 1)); i <= loopTo9; i++)
            {
                if (i <= MapWidth)
                {
                    if (Y > 1)
                    {
                        MaskData[i, Y - 1] = false;
                    }

                    if (Y < MapHeight)
                    {
                        MaskData[i, Y + 1] = false;
                    }
                }
            }

            MaskData[X, Y] = false;
        }

        // 幅３マスの直線状のエリアを選択 (Ｍ拡の攻撃範囲設定用)
        public void AreaInCone(int X, int Y, int min_range, int max_range, string direction)
        {
            ClearMask();

            switch (direction ?? "")
            {
                case "N":
                    {
                        var loopTo2 = (Y - min_range);
                        for (var i = (Y - max_range); i <= loopTo2; i++)
                        {
                            if (i >= 1)
                            {
                                MaskData[X, i] = false;
                            }
                        }

                        var loopTo3 = (Y - (min_range + 1));
                        for (var i = (Y - max_range + 1); i <= loopTo3; i++)
                        {
                            if (i >= 1)
                            {
                                if (X > 1)
                                {
                                    MaskData[X - 1, i] = false;
                                }

                                if (X < MapWidth)
                                {
                                    MaskData[X + 1, i] = false;
                                }
                            }
                        }

                        break;
                    }

                case "S":
                    {
                        var loopTo4 = (Y + max_range);
                        for (var i = (Y + min_range); i <= loopTo4; i++)
                        {
                            if (i <= MapHeight)
                            {
                                MaskData[X, i] = false;
                            }
                        }

                        var loopTo5 = (Y + max_range - 1);
                        for (var i = (Y + (min_range + 1)); i <= loopTo5; i++)
                        {
                            if (i <= MapHeight)
                            {
                                if (X > 1)
                                {
                                    MaskData[X - 1, i] = false;
                                }

                                if (X < MapWidth)
                                {
                                    MaskData[X + 1, i] = false;
                                }
                            }
                        }

                        break;
                    }

                case "W":
                    {
                        var loopTo6 = (X - min_range);
                        for (var i = (X - max_range); i <= loopTo6; i++)
                        {
                            if (i >= 1)
                            {
                                MaskData[i, Y] = false;
                            }
                        }

                        var loopTo7 = (X - (min_range + 1));
                        for (var i = (X - max_range + 1); i <= loopTo7; i++)
                        {
                            if (i >= 1)
                            {
                                if (Y > 1)
                                {
                                    MaskData[i, Y - 1] = false;
                                }

                                if (Y < MapHeight)
                                {
                                    MaskData[i, Y + 1] = false;
                                }
                            }
                        }

                        break;
                    }

                case "E":
                    {
                        var loopTo8 = (X + max_range);
                        for (var i = (X + min_range); i <= loopTo8; i++)
                        {
                            if (i <= MapWidth)
                            {
                                MaskData[i, Y] = false;
                            }
                        }

                        var loopTo9 = (X + max_range - 1);
                        for (var i = (X + (min_range + 1)); i <= loopTo9; i++)
                        {
                            if (i <= MapWidth)
                            {
                                if (Y > 1)
                                {
                                    MaskData[i, Y - 1] = false;
                                }

                                if (Y < MapHeight)
                                {
                                    MaskData[i, Y + 1] = false;
                                }
                            }
                        }

                        break;
                    }
            }

            MaskData[X, Y] = false;
        }

        // 扇状のエリアを選択 (Ｍ扇の攻撃範囲設定用)
        public void AreaInSector(int X, int Y, int min_range, int max_range, string direction, int lv, bool without_refresh = false)
        {
            if (!without_refresh)
            {
                ClearMask();
            }

            switch (direction ?? "")
            {
                case "N":
                    {
                        var loopTo2 = max_range;
                        for (var i = min_range; i <= loopTo2; i++)
                        {
                            var yy = (Y - i);
                            if (yy < 1)
                            {
                                break;
                            }

                            switch (lv)
                            {
                                case 1:
                                    {
                                        var loopTo3 = Math.Min(X + i / 3, MapWidth);
                                        for (var xx = Math.Max(X - i / 3, 1); xx <= loopTo3; xx++)
                                            MaskData[xx, yy] = false;
                                        break;
                                    }

                                case 2:
                                    {
                                        var loopTo4 = Math.Min(X + i / 2, MapWidth);
                                        for (var xx = Math.Max(X - i / 2, 1); xx <= loopTo4; xx++)
                                            MaskData[xx, yy] = false;
                                        break;
                                    }

                                case 3:
                                    {
                                        var loopTo5 = Math.Min(X + (i - 1), MapWidth);
                                        for (var xx = Math.Max(X - (i - 1), 1); xx <= loopTo5; xx++)
                                            MaskData[xx, yy] = false;
                                        break;
                                    }

                                case 4:
                                    {
                                        var loopTo6 = Math.Min(X + i, MapWidth);
                                        for (var xx = Math.Max(X - i, 1); xx <= loopTo6; xx++)
                                            MaskData[xx, yy] = false;
                                        break;
                                    }
                            }
                        }

                        break;
                    }

                case "S":
                    {
                        var loopTo7 = max_range;
                        for (var i = min_range; i <= loopTo7; i++)
                        {
                            var yy = (Y + i);
                            if (yy > MapHeight)
                            {
                                break;
                            }

                            switch (lv)
                            {
                                case 1:
                                    {
                                        var loopTo8 = Math.Min(X + i / 3, MapWidth);
                                        for (var xx = Math.Max(X - i / 3, 1); xx <= loopTo8; xx++)
                                            MaskData[xx, yy] = false;
                                        break;
                                    }

                                case 2:
                                    {
                                        var loopTo9 = Math.Min(X + i / 2, MapWidth);
                                        for (var xx = Math.Max(X - i / 2, 1); xx <= loopTo9; xx++)
                                            MaskData[xx, yy] = false;
                                        break;
                                    }

                                case 3:
                                    {
                                        var loopTo10 = Math.Min(X + (i - 1), MapWidth);
                                        for (var xx = Math.Max(X - (i - 1), 1); xx <= loopTo10; xx++)
                                            MaskData[xx, yy] = false;
                                        break;
                                    }

                                case 4:
                                    {
                                        var loopTo11 = Math.Min(X + i, MapWidth);
                                        for (var xx = Math.Max(X - i, 1); xx <= loopTo11; xx++)
                                            MaskData[xx, yy] = false;
                                        break;
                                    }
                            }
                        }

                        break;
                    }

                case "W":
                    {
                        var loopTo12 = max_range;
                        for (var i = min_range; i <= loopTo12; i++)
                        {
                            var xx = (X - i);
                            if (xx < 1)
                            {
                                break;
                            }

                            switch (lv)
                            {
                                case 1:
                                    {
                                        var loopTo13 = Math.Min(Y + i / 3, MapHeight);
                                        for (var yy = Math.Max(Y - i / 3, 1); yy <= loopTo13; yy++)
                                            MaskData[xx, yy] = false;
                                        break;
                                    }

                                case 2:
                                    {
                                        var loopTo14 = Math.Min(Y + i / 2, MapHeight);
                                        for (var yy = Math.Max(Y - i / 2, 1); yy <= loopTo14; yy++)
                                            MaskData[xx, yy] = false;
                                        break;
                                    }

                                case 3:
                                    {
                                        var loopTo15 = Math.Min(Y + (i - 1), MapHeight);
                                        for (var yy = Math.Max(Y - (i - 1), 1); yy <= loopTo15; yy++)
                                            MaskData[xx, yy] = false;
                                        break;
                                    }

                                case 4:
                                    {
                                        var loopTo16 = Math.Min(Y + i, MapHeight);
                                        for (var yy = Math.Max(Y - i, 1); yy <= loopTo16; yy++)
                                            MaskData[xx, yy] = false;
                                        break;
                                    }
                            }
                        }

                        break;
                    }

                case "E":
                    {
                        var loopTo17 = max_range;
                        for (var i = min_range; i <= loopTo17; i++)
                        {
                            var xx = (X + i);
                            if (xx > MapWidth)
                            {
                                break;
                            }

                            switch (lv)
                            {
                                case 1:
                                    {
                                        var loopTo18 = Math.Min(Y + i / 3, MapHeight);
                                        for (var yy = Math.Max(Y - i / 3, 1); yy <= loopTo18; yy++)
                                            MaskData[xx, yy] = false;
                                        break;
                                    }

                                case 2:
                                    {
                                        var loopTo19 = Math.Min(Y + i / 2, MapHeight);
                                        for (var yy = Math.Max(Y - i / 2, 1); yy <= loopTo19; yy++)
                                            MaskData[xx, yy] = false;
                                        break;
                                    }

                                case 3:
                                    {
                                        var loopTo20 = Math.Min(Y + (i - 1), MapHeight);
                                        for (var yy = Math.Max(Y - (i - 1), 1); yy <= loopTo20; yy++)
                                            MaskData[xx, yy] = false;
                                        break;
                                    }

                                case 4:
                                    {
                                        var loopTo21 = Math.Min(Y + i, MapHeight);
                                        for (var yy = Math.Max(Y - i, 1); yy <= loopTo21; yy++)
                                            MaskData[xx, yy] = false;
                                        break;
                                    }
                            }
                        }

                        break;
                    }
            }

            MaskData[X, Y] = false;
        }

        // 十字状の扇状のエリアを選択 (Ｍ扇の攻撃方向選択用)
        public void AreaInSectorCross(int X, int Y, int min_range, int max_range, int lv)
        {
            ClearMask();

            AreaInSector(X, Y, min_range, max_range, "N", lv, true);
            AreaInSector(X, Y, min_range, max_range, "S", lv, true);
            AreaInSector(X, Y, min_range, max_range, "W", lv, true);
            AreaInSector(X, Y, min_range, max_range, "E", lv, true);
        }

        // ２点間を結ぶ直線状のエリアを選択 (Ｍ線の範囲設定用)
        public void AreaInPointToPoint(int x1, int y1, int x2, int y2)
        {
            //// まず全領域をマスク
            ClearMask();

            // 起点のマスクを解除
            MaskData[x1, y1] = false;
            var xx = x1;
            var yy = y1;
            if (Math.Abs((x1 - x2)) > Math.Abs((y1 - y2)))
            {
                do
                {
                    if (x1 > x2)
                    {
                        xx = (xx - 1);
                    }
                    else
                    {
                        xx = (xx + 1);
                    }

                    MaskData[xx, yy] = false;
                    yy = (int)(y1 + (y2 - y1) * (x1 - xx + 0d) / (x1 - x2));
                    MaskData[xx, yy] = false;
                }
                while (xx != x2);
            }
            else
            {
                do
                {
                    if (y1 > y2)
                    {
                        yy = (yy - 1);
                    }
                    else
                    {
                        yy = (yy + 1);
                    }

                    MaskData[xx, yy] = false;
                    xx = (int)(x1 + (x2 - x1) * (y1 - yy + 0d) / (y1 - y2));
                    MaskData[xx, yy] = false;
                }
                while (yy != y2);
            }
        }

        // ユニット u の移動範囲を選択
        // ジャンプする場合は ByJump = True
        public void AreaInSpeed(Unit u, bool ByJump = false)
        {
            // 移動能力の可否を調べておく
            var unitProps = new UnitMoveProps(u);

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
            if (ByJump)
            {
                uspeed = (int)(currentUnit.Speed + currentUnit.FeatureLevel("ジャンプ"));
            }
            else
            {
                uspeed = currentUnit.Speed;
            }

            // 移動コストは実際の２倍の値で記録されているため、移動力もそれに合わせて
            // ２倍にして移動範囲を計算する
            uspeed = (2 * uspeed);

            // 再移動時は最初の移動の分だけ移動力を減少させる
            if (Commands.SelectedCommand == "再移動")
            {
                uspeed = (uspeed - Commands.SelectedUnitMoveCost);
            }

            if (currentUnit.IsConditionSatisfied("移動不能"))
            {
                uspeed = 0;
            }

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
            FillMoveCost(u, move_cost, move_area, x1, y1, x2, y2, ByJump);

            // ユニットがいるため通り抜け出来ない場所をチェック
            if (!currentUnit.IsFeatureAvailable("すり抜け移動") && !currentUnit.IsUnderSpecialPowerEffect("すり抜け移動"))
            {
                foreach (Unit u2 in SRC.UList.Items)
                {
                    if (u2.Status == "出撃")
                    {
                        var blocked = false;

                        // 敵対する場合は通り抜け不可
                        if (u2.IsEnemy(u, true))
                        {
                            blocked = true;
                        }

                        // 陣営が合わない場合も通り抜け不可
                        switch (u2.Party0 ?? "")
                        {
                            case "味方":
                            case "ＮＰＣ":
                                {
                                    if (u.Party0 != "味方" && u.Party0 != "ＮＰＣ")
                                    {
                                        blocked = true;
                                    }

                                    break;
                                }

                            default:
                                {
                                    if ((u2.Party0 ?? "") != (u.Party0 ?? ""))
                                    {
                                        blocked = true;
                                    }

                                    break;
                                }
                        }

                        // 通り抜けられない場合
                        if (blocked)
                        {
                            move_cost[u2.x, u2.y] = 1000000;
                        }

                        // ＺＯＣ
                        var zarea = 0;
                        if (blocked && !ByJump)
                        {
                            var is_zoc = false;
                            int n = 0;
                            if (u2.IsFeatureAvailable("ＺＯＣ") || Expression.IsOptionDefined("ＺＯＣ"))
                            {
                                is_zoc = true;
                                zarea = 1;

                                // ＺＯＣ側のＺＯＣレベル
                                n = (int)u2.FeatureLevel("ＺＯＣ");
                                if (n == 1)
                                    n = 10000;

                                // Option「ＺＯＣ」が指定されている
                                n = Math.Max(1, n);
                                if (u.IsFeatureAvailable("ＺＯＣ無効化"))
                                {
                                    // 移動側のＺＯＣ無効化レベル
                                    // レベル指定なし、またはLv1はLv10000として扱う
                                    var l = u.FeatureLevel("ＺＯＣ無効化");
                                    if (l == 1)
                                        l = 10000;

                                    // 移動側のＺＯＣ無効化レベルの方が高い場合、
                                    // ＺＯＣ不可能
                                    if (l >= n)
                                    {
                                        is_zoc = false;
                                    }
                                }

                                // 隣接するユニットが「隣接ユニットＺＯＣ無効化」を持っている場合
                                if (is_zoc)
                                {
                                    for (var i = -1; i <= 1; i++)
                                    {
                                        var loopTo44 = Math.Abs(Math.Abs(i) - 1);
                                        for (var j = (Math.Abs(i) - 1); j <= loopTo44; j++)
                                        {
                                            if ((i != 0 || j != 0) && u2.x + i >= 1 && (u2.x + i) <= MapWidth && u2.y + j >= 1 && (u2.y + j) <= MapHeight)
                                            {
                                                // 隣接ユニットが存在する？
                                                if (MapDataForUnit[(u2.x + i), (u2.y + j)] is object)
                                                {
                                                    var buf = u2.Party0;
                                                    {
                                                        var withBlock2 = MapDataForUnit[(u2.x + i), (u2.y + j)];
                                                        // 敵対陣営？
                                                        switch (withBlock2.Party0 ?? "")
                                                        {
                                                            case "味方":
                                                            case "ＮＰＣ":
                                                                {
                                                                    if (buf == "味方" || buf == "ＮＰＣ")
                                                                    {
                                                                        break;
                                                                    }

                                                                    break;
                                                                }

                                                            default:
                                                                {
                                                                    if ((withBlock2.Party0 ?? "") == (buf ?? ""))
                                                                    {
                                                                        break;
                                                                    }

                                                                    break;
                                                                }
                                                        }

                                                        var l = withBlock2.FeatureLevel("隣接ユニットＺＯＣ無効化");
                                                        if (l == 1)
                                                            l = 10000;

                                                        // 移動側のＺＯＣ無効化レベルの方が高い場合、
                                                        // ＺＯＣ不可能
                                                        if (l >= n)
                                                        {
                                                            is_zoc = false;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (is_zoc)
                            {
                                // 特殊能力「ＺＯＣ」が指定されているなら、そのデータの2つ目の値をＺＯＣの範囲に設定
                                // 2つ目の値が省略されている場合は1を設定
                                // ＺＯＣLvが0以下の場合、オプション「ＺＯＣ」が指定されていても範囲を0に設定
                                if (GeneralLib.LLength(u2.FeatureData("ＺＯＣ")) >= 2)
                                {
                                    zarea = Math.Max(Conversions.ToInteger(GeneralLib.LIndex(u2.FeatureData("ＺＯＣ"), 2)), 0);
                                }

                                // 相対距離＋ＺＯＣの範囲が移動力以内のとき、ＺＯＣを設定
                                if (((Math.Abs((int)(u.x - u2.x)) + Math.Abs((int)(u.y - u2.y))) - zarea) <= uspeed)
                                {
                                    // 水平・垂直方向のみのＺＯＣかどうかを判断
                                    var is_hzoc = false;
                                    var is_vzoc = false;
                                    if (Strings.InStr(u2.FeatureData("ＺＯＣ"), "直線") > 0)
                                    {
                                        is_hzoc = true;
                                        is_vzoc = true;
                                    }
                                    else
                                    {
                                        if (Strings.InStr(u2.FeatureData("ＺＯＣ"), "水平") > 0)
                                        {
                                            is_hzoc = true;
                                        }

                                        if (Strings.InStr(u2.FeatureData("ＺＯＣ"), "垂直") > 0)
                                        {
                                            is_vzoc = true;
                                        }
                                    }

                                    if (is_hzoc || is_vzoc)
                                    {
                                        var loopTo45 = zarea;
                                        for (var i = (zarea * -1); i <= loopTo45; i++)
                                        {
                                            if (i == 0)
                                            {
                                                if (PointInZOC[(int)u2.x, (int)u2.y] < 0)
                                                {
                                                    if (n > Math.Abs(PointInZOC[(int)u2.x, (int)u2.y]))
                                                    {
                                                        PointInZOC[(int)u2.x, (int)u2.y] = n;
                                                    }
                                                }
                                                else
                                                {
                                                    PointInZOC[(int)u2.x, (int)u2.y] = Math.Max(n, PointInZOC[u2.x, u2.y]);
                                                }
                                            }
                                            else
                                            {
                                                // 水平ＺＯＣ
                                                if (is_hzoc && u2.x + i >= 1 && (u2.x + i) <= MapWidth)
                                                {
                                                    if (PointInZOC[(u2.x + i), (int)u2.y] < 0)
                                                    {
                                                        if (n > Math.Abs(PointInZOC[(u2.x + i), (int)u2.y]))
                                                        {
                                                            PointInZOC[(u2.x + i), (int)u2.y] = n;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        PointInZOC[(u2.x + i), (int)u2.y] = Math.Max(n, PointInZOC[(u2.x + i), u2.y]);
                                                    }
                                                }
                                                // 垂直ＺＯＣ
                                                if (is_vzoc && u2.y + i >= 1 && (u2.y + i) <= MapHeight)
                                                {
                                                    if (PointInZOC[(int)u2.x, (u2.y + i)] < 0)
                                                    {
                                                        if (n > Math.Abs(PointInZOC[(int)u2.x, (u2.y + i)]))
                                                        {
                                                            PointInZOC[(int)u2.x, (u2.y + i)] = n;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        PointInZOC[(int)u2.x, (u2.y + i)] = Math.Max(n, PointInZOC[u2.x, (u2.y + i)]);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // 全方位ＺＯＣ
                                        var loopTo46 = zarea;
                                        for (var i = (zarea * -1); i <= loopTo46; i++)
                                        {
                                            var loopTo47 = Math.Abs((Math.Abs(i) - zarea));
                                            for (var j = (Math.Abs(i) - zarea); j <= loopTo47; j++)
                                            {
                                                if (u2.x + i >= 1 && (u2.x + i) <= MapWidth && u2.y + j >= 1 && (u2.y + j) <= MapHeight)
                                                {
                                                    if (PointInZOC[(u2.x + i), (u2.y + j)] < 0)
                                                    {
                                                        if (n > Math.Abs(PointInZOC[(u2.x + i), (u2.y + j)]))
                                                        {
                                                            PointInZOC[(u2.x + i), (u2.y + j)] = n;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        PointInZOC[(u2.x + i), (u2.y + j)] = Math.Max(n, PointInZOC[(u2.x + i), (u2.y + j)]);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        // 「広域ＺＯＣ無効化」を所持している場合の処理
                        else if (((Math.Abs((int)(u.x - u2.x)) + Math.Abs((int)(u.y - u2.y))) - zarea) <= uspeed)
                        {
                            // レベル指定なし、またはLv1はLv10000として扱う
                            int l = (int)u2.FeatureLevel("広域ＺＯＣ無効化");
                            if (l == 1)
                                l = 10000;
                            if (l > 0)
                            {
                                var n = Math.Max(GeneralLib.StrToLng(GeneralLib.LIndex(u2.FeatureData("広域ＺＯＣ無効化"), 2)), 1);
                                var loopTo48 = n;
                                for (var i = (n * -1); i <= loopTo48; i++)
                                {
                                    var loopTo49 = Math.Abs((Math.Abs(i) - n));
                                    for (var j = (Math.Abs(i) - n); j <= loopTo49; j++)
                                    {
                                        if (u2.x + i >= 1 && (u2.x + i) <= MapWidth && u2.y + j >= 1 && (u2.y + j) <= MapHeight)
                                        {
                                            PointInZOC[(u2.x + i), (u2.y + j)] = PointInZOC[(u2.x + i), (u2.y + j)] - l;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // 移動停止地形はＺＯＣして扱う
            if (!ByJump)
            {
                for (var i = x1; i <= x2; i++)
                {
                    for (var j = y1; j <= y2; j++)
                    {
                        if (Terrain(i, j).HasMoveStop())
                        {
                            PointInZOC[i, j] = 20000;
                        }
                    }
                }
            }

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
                        if (sp > 1)
                        {
                            tmp = Math.Min(tmp, cur_cost[x - 1, y] + (PointInZOC[x - 1, y] > 0 ? 10000 : 0));
                            tmp = Math.Min(tmp, cur_cost[x + 1, y] + (PointInZOC[x + 1, y] > 0 ? 10000 : 0));
                            tmp = Math.Min(tmp, cur_cost[x, y - 1] + (PointInZOC[x, y - 1] > 0 ? 10000 : 0));
                            tmp = Math.Min(tmp, cur_cost[x, y + 1] + (PointInZOC[x, y + 1] > 0 ? 10000 : 0));
                        }
                        else
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

                    MaskAlreadyUnitExist(currentUnit, i, j, u2);
                }
            }

            // ジャンプ＆透過移動先は進入可能？
            if (ByJump || currentUnit.IsFeatureAvailable("透過移動") || currentUnit.IsUnderSpecialPowerEffect("透過移動"))
            {
                for (var i = x1; i <= x2; i++)
                {
                    for (var j = y1; j <= y2; j++)
                    {
                        if (MaskData[i, j])
                        {
                            goto NextLoop2;
                        }

                        // ユニットがいる地形に進入出来るということは
                        // 合体or着艦可能ということなので地形は無視
                        if (MapDataForUnit[i, j] is object)
                        {
                            goto NextLoop2;
                        }

                        var td = Terrain(i, j);
                        switch (currentUnit.Area ?? "")
                        {
                            case "地上":
                                {
                                    switch (td.Class ?? "")
                                    {
                                        case "空":
                                            {
                                                MaskData[i, j] = true;
                                                break;
                                            }

                                        case "水":
                                            {
                                                if (!unitProps.is_adaptable_in_water && !unitProps.is_trans_available_on_water && !unitProps.is_trans_available_in_water)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }

                                        case "深水":
                                            {
                                                if (!unitProps.is_trans_available_on_water && !unitProps.is_trans_available_in_water)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }

                                        case "宇宙":
                                            {
                                                if (!unitProps.is_adaptable_in_space)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }
                                    }

                                    break;
                                }

                            case "水上":
                                {
                                    switch (td.Class ?? "")
                                    {
                                        case "空":
                                            {
                                                MaskData[i, j] = true;
                                                break;
                                            }

                                        case "宇宙":
                                            {
                                                if (!unitProps.is_adaptable_in_space)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }
                                    }

                                    break;
                                }

                            case "水中":
                                {
                                    switch (td.Class ?? "")
                                    {
                                        case "空":
                                            {
                                                MaskData[i, j] = true;
                                                break;
                                            }

                                        case "深水":
                                            {
                                                if (!unitProps.is_trans_available_on_water && !unitProps.is_trans_available_in_water)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }

                                        case "宇宙":
                                            {
                                                if (!unitProps.is_adaptable_in_space)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }
                                    }

                                    break;
                                }

                            case "空中":
                                {
                                    switch (td.Class ?? "")
                                    {
                                        case "空":
                                            {
                                                if (td.MoveCost > 100)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }

                                        case "宇宙":
                                            {
                                                if (!unitProps.is_adaptable_in_space)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }
                                    }

                                    break;
                                }

                            case "地中":
                                {
                                    if (td.Class != "陸")
                                    {
                                        MaskData[i, j] = true;
                                    }

                                    break;
                                }

                            case "宇宙":
                                {
                                    switch (td.Class ?? "")
                                    {
                                        case "陸":
                                        case "屋内":
                                            {
                                                if (!unitProps.is_trans_available_in_sky && !unitProps.is_trans_available_on_ground)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }

                                        case "空":
                                            {
                                                if (!unitProps.is_trans_available_in_sky || td.MoveCost > 10)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }

                                        case "水":
                                            {
                                                if (!unitProps.is_trans_available_in_water && !unitProps.is_trans_available_on_water && !unitProps.is_adaptable_in_water)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }

                                        case "深水":
                                            {
                                                if (!unitProps.is_trans_available_on_water && !unitProps.is_trans_available_in_water)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }
                                    }

                                    break;
                                }
                        }

                        // 移動制限
                        if (unitProps.allowed_terrains.Any())
                        {
                            MaskData[i, j] = MaskData[i, j] || !unitProps.allowed_terrains.Any(x => x == Terrain(i, j).Name);
                        }

                        // 進入不可
                        if (unitProps.prohibited_terrains.Any())
                        {
                            MaskData[i, j] = MaskData[i, j] || unitProps.prohibited_terrains.Any(x => x == Terrain(i, j).Name);
                        }

                    NextLoop2:
                        ;
                    }
                }
            }

            // 現在いる場所は常に進入可能
            MaskData[currentUnit.x, currentUnit.y] = false;
        }

        private void MaskAlreadyUnitExist(Unit currentUnit, int x, int y, Unit u2)
        {
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
                                    MaskData[x, y] = false;
                                }
                            }
                        }
                        else if (currentUnit.IsFeatureAvailable("合体") && u2.IsFeatureAvailable("合体"))
                        {
                            // ２体合体？
                            MaskData[x, y] = true;
                            foreach (var cfd in currentUnit.TwoUnitCombineFeatures(SRC)
                                .Where(x =>
                                {
                                    // XXX 合体制限は変形してる時だけでいい？
                                    var pu = SRC.UList.Item(x.PartUnitNames.First());
                                    return u2.Name == x.PartUnitNames.First()
                                        || u2.Name == pu.CurrentForm().Name && !u2.IsFeatureAvailable("合体制限");
                                }))
                            {
                                MaskData[x, y] = false;
                            }
                        }
                        break;
                    }

                case "ＮＰＣ":
                    {
                        if (currentUnit.IsFeatureAvailable("合体") && u2.IsFeatureAvailable("合体"))
                        {
                            // ２体合体？
                            MaskData[x, y] = true;
                            foreach (var cfd in currentUnit.TwoUnitCombineFeatures(SRC)
                                .Where(x =>
                                {
                                    // XXX 合体制限は変形してる時だけでいい？
                                    var pu = SRC.UList.Item(x.PartUnitNames.First());
                                    return u2.Name == x.PartUnitNames.First()
                                        || u2.Name == pu.CurrentForm().Name && !u2.IsFeatureAvailable("合体制限");
                                }))
                            {
                                MaskData[x, y] = false;
                            }
                        }
                        break;
                    }
            }
        }
        private void FillMoveCost(Unit u, int[,] move_cost, string move_area, int x1, int y1, int x2, int y2, bool ByJump)
        {
            // 移動能力の可否を調べておく
            var unitProps = new UnitMoveProps(u);

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
                                    if (unitProps.is_adaptable_in_space)
                                    {
                                        move_cost[i, j] = Terrain(i, j).MoveCost;
                                        if (unitProps.IsAdopted(Terrain(i, j)))
                                        {
                                            move_cost[i, j] = Math.Min(move_cost[i, j], 2);
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

                case "地上":
                    for (var x = x1; x <= x2; x++)
                    {
                        for (var y = y1; y <= y2; y++)
                        {
                            switch (MapData[x, y].Terrain.Class)
                            {
                                case "陸":
                                case "屋内":
                                case "月面":
                                    if (unitProps.is_trans_available_on_ground)
                                    {
                                        move_cost[x, y] = Terrain(x, y).MoveCost;
                                        if (unitProps.IsAdopted(Terrain(x, y)))
                                        {
                                            move_cost[x, y] = Math.Min(move_cost[x, y], 2);
                                        }
                                    }
                                    else
                                    {
                                        move_cost[x, y] = 1000000;
                                    }

                                    break;

                                case "水":
                                    if (unitProps.is_trans_available_in_water || unitProps.is_trans_available_on_water)
                                    {
                                        move_cost[x, y] = 2;
                                    }
                                    else if (unitProps.is_adaptable_in_water)
                                    {
                                        move_cost[x, y] = Terrain(x, y).MoveCost;
                                        if (unitProps.IsAdopted(Terrain(x, y)))
                                        {
                                            move_cost[x, y] = Math.Min(move_cost[x, y], 2);
                                        }
                                    }
                                    else
                                    {
                                        move_cost[x, y] = 1000000;
                                    }

                                    break;

                                case "深水":
                                    if (unitProps.is_trans_available_in_water || unitProps.is_trans_available_on_water)
                                    {
                                        move_cost[x, y] = 2;
                                    }
                                    else if (unitProps.is_swimable)
                                    {
                                        move_cost[x, y] = Terrain(x, y).MoveCost;
                                    }
                                    else
                                    {
                                        move_cost[x, y] = 1000000;
                                    }

                                    break;

                                case "空":
                                    move_cost[x, y] = 1000000;
                                    break;

                                case "宇宙":
                                    if (unitProps.is_adaptable_in_space)
                                    {
                                        move_cost[x, y] = Terrain(x, y).MoveCost;
                                        if (unitProps.IsAdopted(Terrain(x, y)))
                                        {
                                            move_cost[x, y] = Math.Min(move_cost[x, y], 2);
                                        }
                                    }
                                    else
                                    {
                                        move_cost[x, y] = 1000000;
                                    }

                                    break;
                            }
                        }
                    }

                    break;

                case "水上":
                    {
                        var loopTo12 = x2;
                        for (var i = x1; i <= loopTo12; i++)
                        {
                            var loopTo13 = y2;
                            for (var j = y1; j <= loopTo13; j++)
                            {
                                var td = Terrain(i, j);
                                switch (td.Class ?? "")
                                {
                                    case "陸":
                                    case "屋内":
                                    case "月面":
                                        {
                                            if (unitProps.is_trans_available_on_ground)
                                            {
                                                move_cost[i, j] = td.MoveCost;
                                                if (unitProps.IsAdopted(Terrain(i, j)))
                                                {
                                                    move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                                                }
                                            }
                                            else
                                            {
                                                move_cost[i, j] = 1000000;
                                            }

                                            break;
                                        }

                                    case "水":
                                    case "深水":
                                        {
                                            move_cost[i, j] = 2;
                                            break;
                                        }

                                    case "空":
                                        {
                                            move_cost[i, j] = 1000000;
                                            break;
                                        }

                                    case "宇宙":
                                        {
                                            if (unitProps.is_adaptable_in_space)
                                            {
                                                move_cost[i, j] = td.MoveCost;
                                                if (unitProps.IsAdopted(Terrain(i, j)))
                                                {
                                                    move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                                                }
                                            }
                                            else
                                            {
                                                move_cost[i, j] = 1000000;
                                            }

                                            break;
                                        }
                                }
                            }
                        }

                        break;
                    }

                case "水中":
                    {
                        var loopTo16 = x2;
                        for (var i = x1; i <= loopTo16; i++)
                        {
                            var loopTo17 = y2;
                            for (var j = y1; j <= loopTo17; j++)
                            {
                                var td = Terrain(i, j);
                                switch (td.Class ?? "")
                                {
                                    case "陸":
                                    case "屋内":
                                    case "月面":
                                        {
                                            if (unitProps.is_trans_available_on_ground)
                                            {
                                                move_cost[i, j] = td.MoveCost;
                                                if (unitProps.IsAdopted(Terrain(i, j)))
                                                {
                                                    move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                                                }
                                            }
                                            else
                                            {
                                                move_cost[i, j] = 1000000;
                                            }

                                            break;
                                        }

                                    case "水":
                                        {
                                            if (unitProps.is_trans_available_in_water)
                                            {
                                                move_cost[i, j] = 2;
                                            }
                                            else
                                            {
                                                move_cost[i, j] = td.MoveCost;
                                                if (unitProps.IsAdopted(Terrain(i, j)))
                                                {
                                                    move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                                                }
                                            }

                                            break;
                                        }

                                    case "深水":
                                        {
                                            if (unitProps.is_trans_available_in_water)
                                            {
                                                move_cost[i, j] = 2;
                                            }
                                            else if (unitProps.is_swimable)
                                            {
                                                move_cost[i, j] = td.MoveCost;
                                            }
                                            else
                                            {
                                                move_cost[i, j] = 1000000;
                                            }

                                            break;
                                        }

                                    case "空":
                                        {
                                            move_cost[i, j] = 1000000;
                                            break;
                                        }

                                    case "宇宙":
                                        {
                                            if (unitProps.is_adaptable_in_space)
                                            {
                                                move_cost[i, j] = td.MoveCost;
                                                if (unitProps.IsAdopted(Terrain(i, j)))
                                                {
                                                    move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                                                }
                                            }
                                            else
                                            {
                                                move_cost[i, j] = 1000000;
                                            }

                                            break;
                                        }
                                }
                            }
                        }

                        break;
                    }

                case "宇宙":
                    {
                        var loopTo21 = x2;
                        for (var i = x1; i <= loopTo21; i++)
                        {
                            var loopTo22 = y2;
                            for (var j = y1; j <= loopTo22; j++)
                            {
                                var td = Terrain(i, j);
                                switch (td.Class ?? "")
                                {
                                    case "宇宙":
                                        {
                                            move_cost[i, j] = td.MoveCost;
                                            if (unitProps.IsAdopted(Terrain(i, j)))
                                            {
                                                move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                                            }

                                            break;
                                        }

                                    case "陸":
                                    case "屋内":
                                        {
                                            if (unitProps.is_trans_available_in_sky)
                                            {
                                                move_cost[i, j] = 2;
                                            }
                                            else if (unitProps.is_trans_available_on_ground)
                                            {
                                                move_cost[i, j] = td.MoveCost;
                                                if (unitProps.IsAdopted(Terrain(i, j)))
                                                {
                                                    move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                                                }
                                            }
                                            else
                                            {
                                                move_cost[i, j] = 1000000;
                                            }

                                            break;
                                        }

                                    case "月面":
                                        {
                                            if (unitProps.is_trans_available_in_moon_sky)
                                            {
                                                move_cost[i, j] = 2;
                                            }
                                            else if (unitProps.is_trans_available_on_ground)
                                            {
                                                move_cost[i, j] = td.MoveCost;
                                                if (unitProps.IsAdopted(Terrain(i, j)))
                                                {
                                                    move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                                                }
                                            }
                                            else
                                            {
                                                move_cost[i, j] = 1000000;
                                            }

                                            break;
                                        }

                                    case "水":
                                        {
                                            if (unitProps.is_trans_available_in_water || unitProps.is_trans_available_on_water)
                                            {
                                                move_cost[i, j] = 2;
                                            }
                                            else if (unitProps.is_adaptable_in_water)
                                            {
                                                move_cost[i, j] = td.MoveCost;
                                                if (unitProps.IsAdopted(Terrain(i, j)))
                                                {
                                                    move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                                                }
                                            }
                                            else
                                            {
                                                move_cost[i, j] = 1000000;
                                            }

                                            break;
                                        }

                                    case "深水":
                                        {
                                            if (unitProps.is_trans_available_in_water || unitProps.is_trans_available_on_water)
                                            {
                                                move_cost[i, j] = 2;
                                            }
                                            else if (unitProps.is_swimable)
                                            {
                                                move_cost[i, j] = td.MoveCost;
                                            }
                                            else
                                            {
                                                move_cost[i, j] = 1000000;
                                            }

                                            break;
                                        }

                                    case "空":
                                        {
                                            if (unitProps.is_trans_available_in_sky)
                                            {
                                                move_cost[i, j] = td.MoveCost;
                                                if (unitProps.IsAdopted(Terrain(i, j)))
                                                {
                                                    move_cost[i, j] = Math.Min(move_cost[i, j], 2);
                                                }
                                            }
                                            else
                                            {
                                                move_cost[i, j] = 1000000;
                                            }

                                            break;
                                        }
                                }
                            }
                        }

                        break;
                    }

                case "地中":
                    {
                        var loopTo28 = x2;
                        for (var i = x1; i <= loopTo28; i++)
                        {
                            var loopTo29 = y2;
                            for (var j = y1; j <= loopTo29; j++)
                            {
                                var td = Terrain(i, j);
                                switch (td.Class ?? "")
                                {
                                    case "陸":
                                    case "月面":
                                        {
                                            move_cost[i, j] = 2;
                                            break;
                                        }

                                    default:
                                        {
                                            move_cost[i, j] = 1000000;
                                            break;
                                        }
                                }
                            }
                        }
                        break;
                    }
                default:
                    throw new NotImplementedException("Invalid terrain.");
            }

            // 線路移動
            if (u.IsFeatureAvailable("線路移動"))
            {
                if (u.Area == "地上" && !ByJump)
                {
                    for (var i = x1; i <= x2; i++)
                    {
                        for (var j = y1; j <= y2; j++)
                        {
                            if (Terrain(i, j).Name == "線路")
                            {
                                move_cost[i, j] = 2;
                            }
                            else
                            {
                                move_cost[i, j] = 1000000;
                            }
                        }
                    }
                }
            }

            for (var i = x1; i <= x2; i++)
            {
                for (var j = y1; j <= y2; j++)
                {
                    // 移動制限
                    if (unitProps.allowed_terrains.Any())
                    {
                        MaskData[i, j] = MaskData[i, j] || !unitProps.allowed_terrains.Any(x => x == Terrain(i, j).Name);
                    }

                    // 進入不可
                    if (unitProps.prohibited_terrains.Any())
                    {
                        MaskData[i, j] = MaskData[i, j] || unitProps.prohibited_terrains.Any(x => x == Terrain(i, j).Name);
                    }
                }
            }

            // ホバー移動
            if (u.IsFeatureAvailable("ホバー移動"))
            {
                if (move_area == "地上" || move_area == "水上")
                {
                    for (var i = x1; i <= x2; i++)
                    {
                        for (var j = y1; j <= y2; j++)
                        {
                            switch (Terrain(i, j).Name ?? "")
                            {
                                case "砂漠":
                                case "雪原":
                                    {
                                        move_cost[i, j] = 2;
                                        break;
                                    }
                            }
                        }
                    }
                }
            }

            // 透過移動
            if (u.IsFeatureAvailable("透過移動") || u.IsUnderSpecialPowerEffect("透過移動"))
            {
                for (var i = x1; i <= x2; i++)
                {
                    for (var j = y1; j <= y2; j++)
                        move_cost[i, j] = 2;
                }
            }
        }

        // ユニット u がテレポートして移動できる範囲を選択
        // 最大距離 lv を指定可能。(省略時は移動力＋テレポートレベル)
        public void AreaInTeleport(Unit currentUnit, int lv = 0)
        {
            // 移動能力の可否を調べておく
            var unitProps = new UnitMoveProps(currentUnit);

            // テレポートによる移動距離を算出
            int r;
            if (lv > 0)
            {
                r = lv;
            }
            else
            {
                r = (int)(currentUnit.Speed + currentUnit.FeatureLevel("テレポート"));
            }

            if (currentUnit.IsConditionSatisfied("移動不能"))
            {
                r = 0;
            }

            // 選択解除
            ClearMask();

            // 移動可能な地点を調べる
            var loopTo4 = Math.Min(MapWidth, currentUnit.x + r);
            for (var i = Math.Max(1, currentUnit.x - r); i <= loopTo4; i++)
            {
                var loopTo5 = Math.Min(MapHeight, currentUnit.y + r);
                for (var j = Math.Max(1, currentUnit.y - r); j <= loopTo5; j++)
                {
                    // 移動範囲内？
                    if ((Math.Abs((currentUnit.x - i)) + Math.Abs((currentUnit.y - j))) > r)
                    {
                        goto NextLoop;
                    }

                    var u2 = MapDataForUnit[i, j];
                    if (u2 is null)
                    {
                        // ユニットがいない地点は地形から進入可能かチェック
                        MaskData[i, j] = false;
                        switch (currentUnit.Area ?? "")
                        {
                            case "地上":
                                {
                                    switch (Terrain(i, j).Class ?? "")
                                    {
                                        case "空":
                                            {
                                                MaskData[i, j] = true;
                                                break;
                                            }

                                        case "水":
                                            {
                                                if (!unitProps.is_adaptable_in_water && !unitProps.is_trans_available_on_water && !unitProps.is_trans_available_in_water)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }

                                        case "深水":
                                            {
                                                if (!unitProps.is_trans_available_on_water && !unitProps.is_trans_available_in_water)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }

                                        case "宇宙":
                                            {
                                                if (!unitProps.is_adaptable_in_space)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }
                                    }

                                    break;
                                }

                            case "水中":
                                {
                                    switch (Terrain(i, j).Class ?? "")
                                    {
                                        case "空":
                                            {
                                                MaskData[i, j] = true;
                                                break;
                                            }

                                        case "深水":
                                            {
                                                if (!unitProps.is_trans_available_on_water && !unitProps.is_trans_available_in_water)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }

                                        case "宇宙":
                                            {
                                                if (!unitProps.is_adaptable_in_space)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }
                                    }

                                    break;
                                }

                            case "空中":
                                {
                                    switch (Terrain(i, j).Class ?? "")
                                    {
                                        case "空":
                                            {
                                                if (Terrain(i, j).MoveCost > 100)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }

                                        case "宇宙":
                                            {
                                                if (!unitProps.is_adaptable_in_space)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }
                                    }

                                    break;
                                }

                            case "地中":
                                {
                                    if (Terrain(i, j).Class != "陸")
                                    {
                                        MaskData[i, j] = true;
                                    }

                                    break;
                                }

                            case "宇宙":
                                {
                                    switch (Terrain(i, j).Class ?? "")
                                    {
                                        case "陸":
                                        case "屋内":
                                            {
                                                if (!unitProps.is_trans_available_in_sky && !unitProps.is_trans_available_on_ground)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }

                                        case "空":
                                            {
                                                if (!unitProps.is_trans_available_in_sky || Terrain(i, j).MoveCost > 100)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }

                                        case "水":
                                            {
                                                if (!unitProps.is_trans_available_in_water && !unitProps.is_trans_available_on_water && !unitProps.is_adaptable_in_water)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }

                                        case "深水":
                                            {
                                                if (!unitProps.is_trans_available_on_water && !unitProps.is_trans_available_in_water)
                                                {
                                                    MaskData[i, j] = true;
                                                }

                                                break;
                                            }
                                    }

                                    break;
                                }
                        }

                        // 移動制限
                        if (unitProps.allowed_terrains.Any())
                        {
                            MaskData[i, j] = MaskData[i, j] || !unitProps.allowed_terrains.Any(x => x == Terrain(i, j).Name);
                        }

                        // 進入不可
                        if (unitProps.prohibited_terrains.Any())
                        {
                            MaskData[i, j] = MaskData[i, j] || unitProps.prohibited_terrains.Any(x => x == Terrain(i, j).Name);
                        }

                        goto NextLoop;
                    }

                    // 合体＆着艦するのは味方のみ
                    if (currentUnit.Party0 != "味方")
                    {
                        goto NextLoop;
                    }

                    MaskAlreadyUnitExist(currentUnit, i, j, u2);
                NextLoop:
                    ;
                }
            }

            // 現在いる場所は常に進入可能
            MaskData[currentUnit.x, currentUnit.y] = false;
        }

        // ユニット u のＭ移武器、アビリティのターゲット座標選択用
        public void AreaInMoveAction(Unit u, int max_range)
        {
            // 全領域マスク
            ClearMask();

            // 移動能力の可否を調べておく
            var unitProps = new UnitMoveProps(u);

            // 移動範囲をチェックすべき領域
            var x1 = Math.Max(1, u.x - max_range);
            var y1 = Math.Max(1, u.y - max_range);
            var x2 = Math.Min(u.x + max_range, MapWidth);
            var y2 = Math.Min(u.y + max_range, MapHeight);

            // 進入可能か判定
            for (var i = x1; i <= x2; i++)
            {
                for (var j = y1; j <= y2; j++)
                {
                    // 移動力の範囲内？
                    if ((Math.Abs((u.x - i)) + Math.Abs((u.y - j))) > max_range)
                    {
                        goto NextLoop;
                    }

                    // ユニットが存在？
                    if (MapDataForUnit[i, j] is object)
                    {
                        goto NextLoop;
                    }

                    var td = Terrain(i, j);
                    // 適応あり？
                    switch (u.Area ?? "")
                    {
                        case "地上":
                            {
                                switch (td.Class ?? "")
                                {
                                    case "空":
                                        {
                                            goto NextLoop;
                                        }

                                    case "水":
                                        {
                                            if (!unitProps.is_adaptable_in_water && !unitProps.is_trans_available_on_water && !unitProps.is_trans_available_in_water)
                                            {
                                                goto NextLoop;
                                            }
                                            break;
                                        }

                                    case "深水":
                                        {
                                            if (!unitProps.is_trans_available_on_water && !unitProps.is_trans_available_in_water)
                                            {
                                                goto NextLoop;
                                            }
                                            break;
                                        }

                                    case "宇宙":
                                        {
                                            if (!unitProps.is_adaptable_in_space)
                                            {
                                                goto NextLoop;
                                            }
                                            break;
                                        }
                                }

                                break;
                            }

                        case "水中":
                            {
                                switch (td.Class ?? "")
                                {
                                    case "空":
                                        {
                                            goto NextLoop;
                                        }

                                    case "深水":
                                        {
                                            if (!unitProps.is_trans_available_on_water && !unitProps.is_trans_available_in_water)
                                            {
                                                goto NextLoop;
                                            }
                                            break;
                                        }

                                    case "宇宙":
                                        {
                                            if (!unitProps.is_adaptable_in_space)
                                            {
                                                goto NextLoop;
                                            }
                                            break;
                                        }
                                }

                                break;
                            }

                        case "空中":
                            {
                                switch (td.Class ?? "")
                                {
                                    case "空":
                                        {
                                            if (td.MoveCost > 100)
                                            {
                                                goto NextLoop;
                                            }
                                            break;
                                        }

                                    case "宇宙":
                                        {
                                            if (!unitProps.is_adaptable_in_space)
                                            {
                                                goto NextLoop;
                                            }
                                            break;
                                        }
                                }

                                break;
                            }

                        case "地中":
                            {
                                if (td.Class != "陸")
                                {
                                    goto NextLoop;
                                }
                                break;
                            }

                        case "宇宙":
                            {
                                switch (td.Class ?? "")
                                {
                                    case "陸":
                                    case "屋内":
                                        {
                                            if (!unitProps.is_trans_available_in_sky && !unitProps.is_trans_available_on_ground)
                                            {
                                                goto NextLoop;
                                            }

                                            break;
                                        }

                                    case "空":
                                        {
                                            if (!unitProps.is_trans_available_in_sky || td.MoveCost > 100)
                                            {
                                                goto NextLoop;
                                            }

                                            break;
                                        }

                                    case "水":
                                        {
                                            if (!unitProps.is_trans_available_in_water && !unitProps.is_trans_available_on_water && !unitProps.is_adaptable_in_water)
                                            {
                                                goto NextLoop;
                                            }

                                            break;
                                        }

                                    case "深水":
                                        {
                                            if (!unitProps.is_trans_available_on_water && !unitProps.is_trans_available_in_water)
                                            {
                                                goto NextLoop;
                                            }

                                            break;
                                        }
                                }

                                // 移動制限
                                if (!unitProps.IsAllowed(td))
                                {
                                    goto NextLoop;
                                }

                                // 進入不可
                                if (unitProps.IsProhibited(td))
                                {
                                    goto NextLoop;
                                }

                                break;
                            }
                    }

                    if (td.DoNotEnter())
                    {
                        if (!unitProps.IsAdopted(td))
                        {
                            goto NextLoop;
                        }
                    }

                    // 進路上に壁がある？
                    if (!unitProps.is_able_to_penetrate)
                    {
                        if (IsLineBlocked(u.x, u.y, i, j, u.Area == "空中"))
                        {
                            goto NextLoop;
                        }
                    }

                    // マスク解除
                    MaskData[i, j] = false;
                NextLoop:
                    ;
                }
            }

            // 現在いる場所は常に進入可能
            MaskData[u.x, u.y] = false;
        }

        // ２点間を結ぶ直線が壁でブロックされているか判定
        public bool IsLineBlocked(int x1, int y1, int x2, int y2, bool is_flying = false)
        {
            bool IsLineBlockedRet = default;
            int xx, yy;
            int xx2, yy2;
            xx = x1;
            yy = y1;
            if (Math.Abs((x1 - x2)) > Math.Abs((y1 - y2)))
            {
                do
                {
                    if (x1 > x2)
                    {
                        xx = (xx - 1);
                    }
                    else
                    {
                        xx = (xx + 1);
                    }

                    yy2 = yy;
                    yy = (int)(y1 + (y2 - y1) * (x1 - xx + 0d) / (x1 - x2));

                    // 壁？
                    if (is_flying)
                    {
                        if (Terrain(xx, yy).Name == "壁" || Terrain(xx, yy2).Name == "壁")
                        {
                            IsLineBlockedRet = true;
                            return IsLineBlockedRet;
                        }
                    }
                    else
                    {
                        switch (Terrain(xx, yy).Name ?? "")
                        {
                            case "壁":
                            case "防壁":
                                {
                                    IsLineBlockedRet = true;
                                    return IsLineBlockedRet;
                                }
                        }

                        switch (Terrain(xx, yy2).Name ?? "")
                        {
                            case "壁":
                            case "防壁":
                                {
                                    IsLineBlockedRet = true;
                                    return IsLineBlockedRet;
                                }
                        }
                    }
                }
                while (xx != x2);
            }
            else
            {
                do
                {
                    if (y1 > y2)
                    {
                        yy = (yy - 1);
                    }
                    else
                    {
                        yy = (yy + 1);
                    }

                    xx2 = xx;
                    xx = (int)(x1 + (x2 - x1) * (y1 - yy + 0d) / (y1 - y2));

                    // 壁？
                    if (is_flying)
                    {
                        if (Terrain(xx, yy).Name == "壁" || Terrain(xx2, yy).Name == "壁")
                        {
                            IsLineBlockedRet = true;
                            return IsLineBlockedRet;
                        }
                    }
                    else
                    {
                        switch (Terrain(xx, yy).Name ?? "")
                        {
                            case "壁":
                            case "防壁":
                                {
                                    IsLineBlockedRet = true;
                                    return IsLineBlockedRet;
                                }
                        }

                        switch (Terrain(xx2, yy).Name ?? "")
                        {
                            case "壁":
                            case "防壁":
                                {
                                    IsLineBlockedRet = true;
                                    return IsLineBlockedRet;
                                }
                        }
                    }
                }
                while (yy != y2);
            }

            IsLineBlockedRet = false;
            return IsLineBlockedRet;
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
            // XXX ジャンプとかテレポートは？
            FillMoveCost(u, move_cost, u.Area, 0, 0, 51, 51, false);

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
                            if (TotalMoveCost[xx, yy - 1] < tmp && PointInZOC[xx, yy - 1] <= 0)
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
                            if (TotalMoveCost[xx, yy + 1] < tmp && PointInZOC[xx, yy + 1] <= 0)
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
                            if (TotalMoveCost[xx - 1, yy] < tmp && PointInZOC[xx - 1, yy] <= 0)
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
                            if (TotalMoveCost[xx + 1, yy] < tmp && PointInZOC[xx + 1, yy] <= 0)
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
                    if (TotalMoveCost[xx, yy - 1] < tmp && PointInZOC[xx, yy - 1] <= 0)
                    {
                        tmp = TotalMoveCost[xx, yy - 1];
                        nx = xx;
                        ny = (yy - 1);
                        direction = "N";
                    }

                    if (TotalMoveCost[xx, yy + 1] < tmp && PointInZOC[xx, yy + 1] <= 0)
                    {
                        tmp = TotalMoveCost[xx, yy + 1];
                        nx = xx;
                        ny = (yy + 1);
                        direction = "S";
                    }

                    if (TotalMoveCost[xx - 1, yy] < tmp && PointInZOC[xx - 1, yy] <= 0)
                    {
                        tmp = TotalMoveCost[xx - 1, yy];
                        nx = (xx - 1);
                        ny = yy;
                        direction = "W";
                    }

                    if (TotalMoveCost[xx + 1, yy] < tmp && PointInZOC[xx + 1, yy] <= 0)
                    {
                        tmp = TotalMoveCost[xx + 1, yy];
                        nx = (xx + 1);
                        ny = yy;
                        direction = "E";
                    }
                }
                else
                {
                    if (TotalMoveCost[xx - 1, yy] < tmp && PointInZOC[xx - 1, yy] <= 0)
                    {
                        tmp = TotalMoveCost[xx - 1, yy];
                        nx = (xx - 1);
                        ny = yy;
                        direction = "W";
                    }

                    if (TotalMoveCost[xx + 1, yy] < tmp && PointInZOC[xx + 1, yy] <= 0)
                    {
                        tmp = TotalMoveCost[xx + 1, yy];
                        nx = (xx + 1);
                        ny = yy;
                        direction = "E";
                    }

                    if (TotalMoveCost[xx, yy - 1] < tmp && PointInZOC[xx, yy - 1] <= 0)
                    {
                        tmp = TotalMoveCost[xx, yy - 1];
                        nx = xx;
                        ny = (yy - 1);
                        direction = "N";
                    }

                    if (TotalMoveCost[xx, yy + 1] < tmp && PointInZOC[xx, yy + 1] <= 0)
                    {
                        tmp = TotalMoveCost[xx, yy + 1];
                        nx = xx;
                        ny = (yy + 1);
                        direction = "S";
                    }
                }

                if (nx == xx && ny == yy)
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
