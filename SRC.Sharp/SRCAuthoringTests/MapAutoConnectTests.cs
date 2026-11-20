using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCAuthoring.Maps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCAuthoring.Tests
{
    [TestClass]
    public class MapAutoConnectTests
    {
        private static MapDocument MakeMap(int w, int h, int fill = 0) =>
            new MapDocument(w, h, new MapTile(fill, 0));

        private static string BasicDataMapDir()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (!File.Exists(Path.Combine(dir.FullName, "SRC.Sharp.sln")))
            {
                dir = dir.Parent;
            }
            return Path.Combine(dir.FullName, "Content", "basic-data", "Bitmap", "Map");
        }

        [TestMethod]
        public void Road_ConnectsAllFourWayAndCorners()
        {
            // 5x5 map with cross + corner + isolated road
            // Center at (3,3) -> 4-way (10)
            // Straight N-S at (3,2) when (3,1) is road -> 1
            // Straight E-W at (2,3) when (1,3) is road -> 0
            // T-junctions:
            //   (3,2) with E=(4,2), N=(3,1), S=(3,3) -> N+E+S (7) -> chip 9
            var doc = MakeMap(7, 7, 0);
            // Horizontal (1..5, 4) -> 1-based
            for (int x = 1; x <= 5; x++) doc[x, 4] = new MapTile(2, 0);
            // Vertical (4, 2..6) -> wait: (4,2) has S only; let's build explicit patterns
            doc = MakeMap(9, 9, 0);
            // 4-way at (3,3): N=(3,2), E=(4,3), S=(3,4), W=(2,3)
            doc[3, 3] = new MapTile(2, 0);
            doc[3, 2] = new MapTile(2, 0);
            doc[4, 3] = new MapTile(2, 0);
            doc[3, 4] = new MapTile(2, 0);
            doc[2, 3] = new MapTile(2, 0);

            // Corner N+E -> 4 at (7,3): N=(7,2), E=(8,3)
            doc[7, 3] = new MapTile(2, 0);
            doc[7, 2] = new MapTile(2, 0);
            doc[8, 3] = new MapTile(2, 0);

            // Corner S+W -> 2 at (8,2) when W=(7,2), S=(8,3)
            doc[8, 2] = new MapTile(2, 0);

            // Corner E+S -> 5 at (2,7), S+W->2 at (4,7), N+E->4 at (2,8), N+W->3 at (4,8)
            doc[2, 7] = new MapTile(2, 0);
            doc[3, 7] = new MapTile(2, 0);
            doc[4, 7] = new MapTile(2, 0);
            doc[2, 8] = new MapTile(2, 0);
            doc[4, 8] = new MapTile(2, 0);
            doc[3, 8] = new MapTile(2, 0);

            MapAutoConnect.Apply(doc);

            Assert.AreEqual(10, doc[3, 3].Bitmap, "4方向交差");
            Assert.AreEqual(1, doc[3, 2].Bitmap, "南北直進（北端はSのみ接続 -> 1）");
            Assert.AreEqual(0, doc[2, 3].Bitmap, "東西直進（西端はEのみ接続 -> 0）");
            Assert.AreEqual(4, doc[7, 3].Bitmap, "北東角");
            Assert.AreEqual(2, doc[8, 2].Bitmap, "南西角");
            Assert.AreEqual(5, doc[2, 7].Bitmap, "東南角");
            Assert.AreEqual(3, doc[4, 8].Bitmap, "北西角");
            Assert.AreEqual(6, doc[3, 7].Bitmap, "東・南・西 T字");
        }

        [TestMethod]
        public void Bridge_OrientationMatchesRoadAndWater_WithFallback()
        {
            var doc = MakeMap(7, 7, 0);
            // Horizontal road (2..6, 3) crossing vertical river (4, 1..5)
            for (int y = 1; y <= 5; y++) doc[4, y] = new MapTile(19, 0);
            doc[2, 3] = new MapTile(2, 0);
            doc[3, 3] = new MapTile(2, 0);
            doc[4, 3] = new MapTile(25, 0);
            doc[5, 3] = new MapTile(2, 0);
            doc[6, 3] = new MapTile(2, 0);

            // Vertical road (2, 4..7) crossing horizontal river (1..3, 6)
            for (int x = 1; x <= 3; x++) doc[x, 6] = new MapTile(19, 0);
            doc[2, 5] = new MapTile(2, 0);
            doc[2, 6] = new MapTile(25, 0);
            doc[2, 7] = new MapTile(2, 0);

            MapAutoConnect.Apply(doc);
            Assert.AreEqual(501, doc[4, 3].Bitmap, "東西道路の橋は横向き道路橋 501");
            Assert.AreEqual(0, doc[2, 6].Bitmap, "南北道路の橋は縦向き道路橋 0");
            Assert.AreEqual(11, doc[4, 2].Bitmap, "橋の上下も川としてつながる");

            // Fallback when bridge0501 is missing (only 0, 1 exist)
            MapAutoConnect.Apply(doc, hasImage: (bmp, no) => bmp != "bridge" || no <= 1);
            Assert.AreEqual(1, doc[4, 3].Bitmap, "501 が無いパックでは横向き 1 にフォールバック");
        }

        [TestMethod]
        public void River_SelectsEndsStripsCornersAndCenter()
        {
            var doc = MakeMap(7, 7, 0);
            // L-shaped vertical to horizontal river: (2,2)->(2,3)->(3,3)
            doc[2, 2] = new MapTile(19, 0);
            doc[2, 3] = new MapTile(19, 0);
            doc[3, 3] = new MapTile(19, 0);

            // Wide 3x3 pond at (5..7, 4..6) — bottom-right is at edge (7,6) so wait: place pond at (4..6, 4..6) inside 7x7 plain border? Wait border at 7 is plain
            doc[2, 5] = new MapTile(19, 0); // isolated pond

            MapAutoConnect.Apply(doc);
            Assert.AreEqual(147, doc[2, 2].Bitmap, "南だけ開く川の先端 147");
            Assert.AreEqual(6, doc[2, 3].Bitmap, "北東へ曲がる南西角 6");
            Assert.AreEqual(146, doc[3, 3].Bitmap, "西だけ開く川の先端 146");
            Assert.AreEqual(4, doc[2, 5].Bitmap, "孤立した水たまり 4");

            // Test 3x3 river pool at (3..5, 3..5) in 7x7 map
            var pool = MakeMap(7, 7, 0);
            for (int y = 3; y <= 5; y++)
                for (int x = 3; x <= 5; x++)
                    pool[x, y] = new MapTile(19, 0);
            MapAutoConnect.Apply(pool);
            Assert.AreEqual(0, pool[3, 3].Bitmap, "池の北西角 0");
            Assert.AreEqual(1, pool[4, 3].Bitmap, "池の北岸 1");
            Assert.AreEqual(2, pool[5, 3].Bitmap, "池の北東角 2");
            Assert.AreEqual(3, pool[3, 4].Bitmap, "池の西岸 3");
            Assert.AreEqual(82, pool[4, 4].Bitmap, "池の中央（斜めも水）82");
            Assert.AreEqual(5, pool[5, 4].Bitmap, "池の東岸 5");
            Assert.AreEqual(6, pool[3, 5].Bitmap, "池の南西角 6");
            Assert.AreEqual(7, pool[4, 5].Bitmap, "池の南岸 7");
            Assert.AreEqual(8, pool[5, 5].Bitmap, "池の南東角 8");
        }

        [TestMethod]
        public void Sea_HandlesCoastsDiagonalsDeepSeaAndEdgeClamping()
        {
            // 10x10 sea with 2x2 island at (5..6, 5..6) in 1-based -> deep sea at (1,1)
            var doc = MakeMap(10, 10, 17);
            doc[5, 5] = new MapTile(0, 0);
            doc[6, 5] = new MapTile(0, 0);
            doc[5, 6] = new MapTile(0, 0);
            doc[6, 6] = new MapTile(0, 0);

            MapAutoConnect.Apply(doc);

            Assert.AreEqual(6, doc[1, 1].Bitmap, "端クランプにより端でも陸が無ければ深海 6");
            Assert.AreEqual(1, doc[5, 4].Bitmap, "南が陸の海 1");
            Assert.AreEqual(2, doc[7, 5].Bitmap, "西が陸の海 2");
            Assert.AreEqual(3, doc[5, 7].Bitmap, "北が陸の海 3");
            Assert.AreEqual(4, doc[4, 5].Bitmap, "東が陸の海 4");
            Assert.AreEqual(32, doc[4, 4].Bitmap, "南東が斜め陸 32");
            Assert.AreEqual(31, doc[7, 4].Bitmap, "南西が斜め陸 31");
            Assert.AreEqual(29, doc[4, 7].Bitmap, "北東が斜め陸 29");
            Assert.AreEqual(28, doc[7, 7].Bitmap, "北西が斜め陸 28");

            // Edge clamping: road reaching map edge continues straight instead of turning into corner
            var edgeRoad = MakeMap(5, 5, 0);
            edgeRoad[1, 3] = new MapTile(2, 0);
            edgeRoad[2, 3] = new MapTile(2, 0);
            edgeRoad[3, 1] = new MapTile(2, 0);
            edgeRoad[3, 2] = new MapTile(2, 0);
            MapAutoConnect.Apply(edgeRoad);
            Assert.AreEqual(0, edgeRoad[1, 3].Bitmap, "左端の東西道路");
            Assert.AreEqual(1, edgeRoad[3, 1].Bitmap, "上端の南北道路");
        }

        [TestMethod]
        public void WoodsMountainLavaWasteAndSpaceRules_WorkAndRespectHasImage()
        {
            var doc = MakeMap(10, 10, 0);
            // Isolated mountain at (2,2) -> 49
            doc[2, 2] = new MapTile(15, 0);
            // Mountain block at (4..6, 2..3)
            for (int y = 2; y <= 3; y++)
                for (int x = 4; x <= 6; x++)
                    doc[x, y] = new MapTile(15, 0);

            // Isolated woods at (2,5) -> 8
            doc[2, 5] = new MapTile(11, 0);

            // Lava 3x3 at (4..6, 5..7)
            for (int y = 5; y <= 7; y++)
                for (int x = 4; x <= 6; x++)
                    doc[x, y] = new MapTile(83, 0);

            // Waste land 3x3 at (7..9, 5..7)
            for (int y = 5; y <= 7; y++)
                for (int x = 7; x <= 9; x++)
                    doc[x, y] = new MapTile(10, 0);

            // Asteroids at (2,8)-(3,8)
            doc[2, 8] = new MapTile(12, 0);
            doc[3, 8] = new MapTile(12, 0);

            MapAutoConnect.Apply(doc);

            Assert.AreEqual(49, doc[2, 2].Bitmap, "孤立した山 49");
            Assert.AreEqual(0, doc[4, 2].Bitmap, "山の北西角 0");
            Assert.AreEqual(1, doc[5, 2].Bitmap, "山の北辺 1");
            Assert.AreEqual(3, doc[4, 3].Bitmap, "山の南西角 3");
            Assert.AreEqual(8, doc[2, 5].Bitmap, "孤立した林 8");
            Assert.AreEqual(0, doc[4, 5].Bitmap, "溶岩の北西 0");
            Assert.AreEqual(4, doc[5, 6].Bitmap, "溶岩の中央 4");
            Assert.AreEqual(8, doc[6, 7].Bitmap, "溶岩の南東 8");
            Assert.AreEqual(17, doc[7, 5].Bitmap, "荒れ地の北西 17");
            Assert.AreEqual(0, doc[8, 6].Bitmap, "荒れ地の中央 0");
            Assert.AreEqual(24, doc[9, 7].Bitmap, "荒れ地の南東 24");
            Assert.AreEqual(1, doc[2, 8].Bitmap, "暗礁 1隣 -> 1");

            // Plain variants never pick 10..13 (avoid floating puddles)
            for (int y = 1; y <= 10; y++)
            {
                for (int x = 1; x <= 10; x++)
                {
                    if (doc[x, y].Terrain == 0)
                    {
                        Assert.IsTrue(doc[x, y].Bitmap <= 5, $"平地の変種に 10..13 を選ばない: {doc[x, y].Bitmap}");
                    }
                }
            }

            // Custom pack lacking variants falls back cleanly to 0
            MapAutoConnect.Apply(doc, hasImage: (bmp, no) => no == 0);
            Assert.AreEqual(0, doc[2, 2].Bitmap);
            Assert.AreEqual(0, doc[5, 6].Bitmap);
            Assert.AreEqual(0, doc[7, 5].Bitmap);
        }

        [TestMethod]
        public void Layer_UsesLayerOrUnderlyingTileForNeighborsAndPreservesEmptyCells()
        {
            // Terrain: road running E-W at y=2 on base tiles; river running N-S at x=3 on base tiles; bridge placed on layer at (3,2)
            var doc = MakeMap(5, 5, 0);
            doc[2, 2] = new MapTile(2, 0);
            doc[4, 2] = new MapTile(2, 0);
            doc[3, 1] = new MapTile(19, 0);
            doc[3, 2] = new MapTile(19, 0);
            doc[3, 3] = new MapTile(19, 0);
            doc.SetLayerEnabled(true);
            doc.Layer[2, 1] = new MapTile(25, 0); // (3,2) in 1-based -> (2,1) in 0-based

            MapAutoConnect.Apply(doc);

            Assert.AreEqual(501, doc.Layer[2, 1].Bitmap, "レイヤー上の橋も下の道路とつながって横向き 501");
            Assert.IsTrue(doc.Layer[0, 0].IsEmpty, "空のレイヤーマスは空のまま");

            // Terrains without rules keep their existing bitmap
            doc[1, 1] = new MapTile(22, 3); // building with custom bitmap 3
            MapAutoConnect.Apply(doc);
            Assert.AreEqual(3, doc[1, 1].Bitmap);
        }

        [TestMethod]
        public void BasicData_ContainsEveryGeneratedBitmap()
        {
            var mapDir = BasicDataMapDir();
            var exists = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var dir in Directory.GetDirectories(mapDir))
            {
                foreach (var file in Directory.GetFiles(dir, "*.bmp"))
                {
                    exists.Add(Path.GetFileNameWithoutExtension(file));
                }
            }

            var doc = MakeMap(16, 16, 0);
            // Paint a mix of all auto-connected terrains
            for (int y = 1; y <= 6; y++)
                for (int x = 1; x <= 6; x++)
                    doc[x, y] = new MapTile(17, 0); // sea
            for (int x = 1; x <= 14; x++) doc[x, 8] = new MapTile(2, 0); // road
            for (int y = 7; y <= 14; y++) doc[8, y] = new MapTile(19, 0); // river
            doc[8, 8] = new MapTile(25, 0); // bridge
            for (int y = 10; y <= 13; y++)
                for (int x = 10; x <= 13; x++)
                    doc[x, y] = new MapTile(11, 0); // woods
            for (int y = 2; y <= 4; y++)
                for (int x = 10; x <= 12; x++)
                    doc[x, y] = new MapTile(15, 0); // mountain
            doc[14, 2] = new MapTile(15, 0); // isolated peak
            doc[10, 6] = new MapTile(83, 0);
            doc[11, 6] = new MapTile(83, 0);
            doc[10, 7] = new MapTile(10, 0);
            doc[14, 14] = new MapTile(5, 0);
            doc[15, 14] = new MapTile(12, 0);
            doc[14, 15] = new MapTile(21, 0);
            doc[15, 15] = new MapTile(8, 0);
            doc[16, 15] = new MapTile(14, 0);

            MapAutoConnect.Apply(doc);

            for (int y = 1; y <= doc.Height; y++)
            {
                for (int x = 1; x <= doc.Width; x++)
                {
                    var tile = doc[x, y];
                    var bmp = MapAutoConnect.DefaultBitmapName(tile.Terrain);
                    if (bmp != null)
                    {
                        var file = $"{bmp}{tile.Bitmap:0000}";
                        Assert.IsTrue(exists.Contains(file), $"basic-data に {file}.bmp がありません ({x},{y}, terrain={tile.Terrain})");
                    }
                }
            }
        }
    }
}
