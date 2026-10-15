using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Exceptions;
using SRCCore.Filesystem;
using SRCCore.Maps;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Map.LoadMapData / SetMapSize / ClearMap / 座標参照系のテスト。
    /// VB: Map.bas LoadMapData, SetMapSize, ClearMap, UnitAtPoint
    /// </summary>
    [TestClass]
    public class MapLoadDataCoverageTests
    {
        [TestMethod]
        public void LoadMapData_NewFormat_ReadsSizeAndColumnMajorCells()
        {
            // 新形式: 1行目 MapData, 2行目予約, 3行目 幅,高さ。以降 x 外側・y 内側の順に「地形,画像番号」
            var f = new MapCoverageFixture();
            var text = "\"MapData\"\r\n\"\"\r\n3,2\r\n"
                + "0,1\r\n1,2\r\n"   // x=1: y=1, y=2
                + "2,3\r\n0,4\r\n"   // x=2
                + "1,5\r\n2,6\r\n";  // x=3
            f.Map.LoadMapData("stage.map", MapCoverageFixture.ToStream(text));

            Assert.AreEqual("stage.map", f.Map.MapFileName);
            Assert.IsFalse(f.Map.IsStatusView);
            Assert.AreEqual(3, f.Map.MapWidth);
            Assert.AreEqual(2, f.Map.MapHeight);
            Assert.AreEqual("山", f.Map.Terrain(1, 2).Name);
            Assert.AreEqual(2, f.Map.MapData[1, 2].BitmapNo);
            Assert.AreEqual("海", f.Map.Terrain(2, 1).Name);
            Assert.AreEqual(3, f.Map.MapData[2, 1].BitmapNo);
            Assert.AreEqual("海", f.Map.Terrain(3, 2).Name);
            Assert.AreEqual(6, f.Map.MapData[3, 2].BitmapNo);
            // レイヤー無しは下層
            Assert.AreEqual(BoxTypes.Under, f.Map.MapData[3, 2].BoxType);
            // SetMapSize が GUI 側へサイズを通知する
            CollectionAssert.Contains(f.GUIMap.SetMapSizeCalls, (3, 2));
        }

        [TestMethod]
        public void LoadMapData_OldFormat_Is20x20AndFirstLineIsData()
        {
            // 先頭行が MapData でなければ旧形式として 20x20 とし、先頭行もデータとして読む
            var f = new MapCoverageFixture();
            var sb = new StringBuilder();
            sb.Append("1,7\r\n");
            for (var i = 1; i < 400; i++)
            {
                sb.Append("0,0\r\n");
            }
            f.Map.LoadMapData("old.map", MapCoverageFixture.ToStream(sb.ToString()));

            Assert.AreEqual(20, f.Map.MapWidth);
            Assert.AreEqual(20, f.Map.MapHeight);
            Assert.AreEqual("山", f.Map.Terrain(1, 1).Name);
            Assert.AreEqual(7, f.Map.MapData[1, 1].BitmapNo);
            Assert.AreEqual("平地", f.Map.Terrain(20, 20).Name);
        }

        [TestMethod]
        public void LoadMapData_LayerSection_SetsUpperTerrainAndBoxType()
        {
            // Layer 節: NO_LAYER_NUM 以外は上層地形を持ち、マスの地形は上層が優先される
            var f = new MapCoverageFixture();
            var text = "MapData\r\n\r\n2,1\r\n0,0\r\n0,0\r\nLayer\r\n"
                + "4,9\r\n"
                + MapCell.NO_LAYER_NUM + "," + MapCell.NO_LAYER_NUM + "\r\n";
            f.Map.LoadMapData("layer.map", MapCoverageFixture.ToStream(text));

            var upper = f.Map.MapData[1, 1];
            Assert.AreEqual(4, upper.LayerType);
            Assert.AreEqual(9, upper.LayerBitmapNo);
            Assert.AreEqual(BoxTypes.Upper, upper.BoxType);
            Assert.AreEqual("壁", f.Map.Terrain(1, 1).Name);

            var under = f.Map.MapData[2, 1];
            Assert.AreEqual(BoxTypes.Under, under.BoxType);
            Assert.AreEqual("平地", f.Map.Terrain(2, 1).Name);
        }

        [TestMethod]
        public void LoadMapData_UndefinedTerrain_ThrowsTerminate()
        {
            // 未定義の地形番号はデータ不正として終了する
            var f = new MapCoverageFixture();
            var text = "MapData\r\n\r\n1,1\r\n99,0\r\n";
            var ex = Assert.Throws<TerminateException>(
                () => f.Map.LoadMapData("bad.map", MapCoverageFixture.ToStream(text)));
            StringAssert.Contains(ex.Message, "bad.map");
        }

        [TestMethod]
        public void LoadMapData_UndefinedLayerTerrain_ThrowsTerminate()
        {
            // レイヤーの地形番号も定義済みである必要がある
            var f = new MapCoverageFixture();
            var text = "MapData\r\n\r\n1,1\r\n0,0\r\nLayer\r\n98,0\r\n";
            Assert.Throws<TerminateException>(
                () => f.Map.LoadMapData("bad.map", MapCoverageFixture.ToStream(text)));
        }

        [TestMethod]
        public void LoadMapData_AllowMissingAssets_DefinesPlaceholderTerrain()
        {
            // 素材なし実行では未定義の地形番号を仮の地形（陸）として扱う
            var f = new MapCoverageFixture();
            f.Src.AllowMissingAssets = true;
            var text = "MapData\r\n\r\n1,1\r\n99,0\r\nLayer\r\n98,0\r\n";
            f.Map.LoadMapData("missing.map", MapCoverageFixture.ToStream(text));

            Assert.AreEqual("陸", f.Map.MapData[1, 1].UnderTerrain.Class);
            Assert.IsTrue(f.Src.TDList.IsDefined(99));
            Assert.IsTrue(f.Src.TDList.IsDefined(98));
            Assert.AreEqual(BoxTypes.Upper, f.Map.MapData[1, 1].BoxType);
        }

        [TestMethod]
        public void LoadMapData_NoFile_SetsUpStatusViewWithMainSize()
        {
            // ファイル名が空ならマップ無し（ステータス表示用）として画面サイズのマップを用意する
            var f = new MapCoverageFixture();
            f.GUI.MainWidth = 15;
            f.GUI.MainHeight = 12;
            f.Map.LoadMapData("");

            Assert.AreEqual("", f.Map.MapFileName);
            Assert.IsTrue(f.Map.IsStatusView);
            Assert.AreEqual(15, f.Map.MapWidth);
            Assert.AreEqual(12, f.Map.MapHeight);
            Assert.AreEqual(Maps.Map.MAX_TERRAIN_DATA_NUM, f.Map.MapData[15, 12].TerrainType);
        }

        [TestMethod]
        public void LoadMapData_NoFile_StatusScenario_Uses40Rows()
        {
            // 「ステータス表示.」「ランキング.」シナリオではマップの高さを 40 にする
            var f = new MapCoverageFixture();
            f.Src.ScenarioFileName = @"C:\scenario\ステータス表示.eve";
            f.Map.LoadMapData("");
            Assert.AreEqual(f.GUI.MainWidth, f.Map.MapWidth);
            Assert.AreEqual(40, f.Map.MapHeight);

            var f2 = new MapCoverageFixture();
            f2.Src.ScenarioFileName = "ランキング.eve";
            f2.Map.LoadMapData("");
            Assert.AreEqual(40, f2.Map.MapHeight);
        }

        [TestMethod]
        public void SetMapSize_AllocatesArraysAndCentersView()
        {
            // マップサイズ設定で各配列を確保し、表示中心を画面中央にする
            var f = new MapCoverageFixture();
            f.GUI.MainWidth = 15;
            f.GUI.MainHeight = 14;
            f.Map.SetMapSize(4, 3);

            Assert.AreEqual(4, f.Map.MapData[4, 3].X);
            Assert.AreEqual(3, f.Map.MapData[4, 3].Y);
            Assert.IsNull(f.Map.MapDataForUnit[4, 3]);
            Assert.AreEqual(6, f.Map.TotalMoveCost.GetLength(0));
            Assert.AreEqual(5, f.Map.PointInZOC.GetLength(1));
            Assert.AreEqual(8, f.GUI.MapX);
            Assert.AreEqual(7, f.GUI.MapY);
        }

        [TestMethod]
        public void ClearMap_ResetsSizeAndDrawSettingsAndRedraws()
        {
            // ClearMap はマップを 1x1 にして描画モード類を初期化し、画面を再描画する
            var f = new MapCoverageFixture();
            f.LoadGrid("...", "...");
            f.Map.MapDrawMode = "セピア";
            f.Map.MapDrawIsMapOnly = true;
            f.Map.MapDrawFilterColor = Color.Red;
            f.Map.MapDrawFilterTransPercent = 0.5;
            var redrawn = false;
            f.GUI.RedrawScreenHandler = _ => redrawn = true;

            f.Map.ClearMap();

            Assert.AreEqual(1, f.Map.MapWidth);
            Assert.AreEqual(1, f.Map.MapHeight);
            Assert.AreEqual("", f.Map.MapDrawMode);
            Assert.IsFalse(f.Map.MapDrawIsMapOnly);
            Assert.AreEqual(Color.Black, f.Map.MapDrawFilterColor);
            Assert.AreEqual(0d, f.Map.MapDrawFilterTransPercent);
            Assert.IsTrue(redrawn);
        }

        [TestMethod]
        public void InitMap_UsesMainScreenSize()
        {
            // InitMap は画面サイズのマップを用意する
            var f = new MapCoverageFixture();
            f.GUI.MainWidth = 13;
            f.GUI.MainHeight = 11;
            f.Map.InitMap();
            Assert.AreEqual(13, f.Map.MapWidth);
            Assert.AreEqual(11, f.Map.MapHeight);
        }

        [TestMethod]
        public void PointLookups_ReturnNullOutsideMap()
        {
            // マップ外の座標は地形もユニットも存在しない
            var f = new MapCoverageFixture();
            f.LoadGrid("..M", "...");
            f.DefineUnit("歩兵", "陸", 2);
            var u = f.Place("歩兵", "味方", 3, 2);

            Assert.IsTrue(f.Map.IsInside(3, 2));
            Assert.IsFalse(f.Map.IsInside(0, 1));
            Assert.IsFalse(f.Map.IsInside(4, 1));
            Assert.IsFalse(f.Map.IsInside(1, 3));
            Assert.IsNull(f.Map.Terrain(4, 1));
            Assert.IsNull(f.Map.CellAtPoint(1, 0));
            Assert.AreEqual("山", f.Map.Terrain(3, 1).Name);
            Assert.AreSame(u, f.Map.UnitAtPoint(3, 2));
            Assert.IsNull(f.Map.UnitAtPoint(3, 3));
        }

        [TestMethod]
        public void AdjacentUnit_ReturnsOnlyOrthogonalNeighbors()
        {
            // 隣接ユニットは上下左右の４方向のみ（斜めは含まない）
            var f = new MapCoverageFixture();
            f.LoadGrid("...", "...", "...");
            f.DefineUnit("歩兵", "陸", 2);
            var center = f.Place("歩兵", "味方", 2, 2);
            var east = f.Place("歩兵", "敵", 3, 2);
            var north = f.Place("歩兵", "味方", 2, 1);
            f.Place("歩兵", "敵", 1, 1); // 斜め

            var adj = f.Map.AdjacentUnit(center);
            Assert.AreEqual(2, adj.Count);
            CollectionAssert.Contains(adj.ToArray(), east);
            CollectionAssert.Contains(adj.ToArray(), north);

            // 角では範囲外の方向を無視する
            Assert.AreEqual(0, f.Map.AdjacentUnit(1, 3).Count);
        }

        [TestMethod]
        public void SearchTerrainImageFile_PrefersSubfolderThenFlatThenUnpadded()
        {
            // 地形画像は Bitmap/Map/<名>/<名>0000.bmp → Bitmap/Map/<名>0000.bmp → Bitmap/Map/<名>N.bmp の順で探す
            var dir = Path.Combine(Path.GetTempPath(), "srcs-mapcov-" + Guid.NewGuid());
            try
            {
                Directory.CreateDirectory(Path.Combine(dir, "Bitmap", "Map", "mountain"));
                File.WriteAllBytes(Path.Combine(dir, "Bitmap", "Map", "plain3.bmp"), new byte[0]);
                File.WriteAllBytes(Path.Combine(dir, "Bitmap", "Map", "mountain0002.bmp"), new byte[0]);
                File.WriteAllBytes(Path.Combine(dir, "Bitmap", "Map", "mountain", "mountain0002.bmp"), new byte[0]);

                var f = new MapCoverageFixture();
                var fs = new LocalFileSystem();
                fs.AddPath(dir);
                f.Src.FileSystem = fs;
                var text = "MapData\r\n\r\n3,1\r\n0,3\r\n1,2\r\n0,5\r\n";
                f.Map.LoadMapData("img.map", MapCoverageFixture.ToStream(text));

                var plain = f.Map.SearchTerrainImageFile(f.Map.MapData[1, 1]);
                StringAssert.EndsWith(plain.Replace('\\', '/'), "Bitmap/Map/plain3.bmp");
                Assert.AreEqual(plain, f.Map.MapUnderImageFilePath[1, 1]);

                var mountain = f.Map.SearchTerrainImageFile(f.Map.MapData[2, 1]);
                StringAssert.EndsWith(mountain.Replace('\\', '/'), "Bitmap/Map/mountain/mountain0002.bmp");

                // どの候補も無ければ null
                Assert.IsNull(f.Map.SearchTerrainImageFile(f.Map.MapData[3, 1]));
                Assert.IsNull(f.Map.MapUnderImageFilePath[3, 1]);
                // 下層地形が無いセルは null
                Assert.IsNull(f.Map.SearchTerrainImageFile(new MapCell()));
                // 上層地形が無いセルの上層画像も null
                Assert.IsNull(f.Map.SearchLayerImageFile(f.Map.MapData[1, 1]));
            }
            finally
            {
                try { Directory.Delete(dir, true); } catch { }
            }
        }
    }
}
