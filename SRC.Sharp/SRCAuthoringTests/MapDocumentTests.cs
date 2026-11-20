using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCAuthoring.Maps;
using SRCCore;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCAuthoring.Tests
{
    /// <summary>マップ作成ツール（SRC#Studio）が使うマップの読み書きと地形データ（7474/SRC#1641）</summary>
    [TestClass]
    public class MapDocumentTests
    {
        private static string SolutionDir()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (!File.Exists(Path.Combine(dir.FullName, "SRC.Sharp.sln")))
            {
                dir = dir.Parent;
            }
            return dir.FullName;
        }

        private static SRC NewSrc()
        {
            var src = new SRC { FileSystem = new LocalFileSystem() };
            new HeadlessGUI().Install(src);
            src.AllowMissingAssets = true;
            return src;
        }

        private static IEnumerable<string> RepositoryMaps() =>
            new[] { "Content", "E2E" }.SelectMany(d => Directory.EnumerateFiles(Path.Combine(SolutionDir(), d), "*.map", SearchOption.AllDirectories));

        [TestMethod]
        public void RepositoryMaps_RoundTripByteForByte()
        {
            var maps = RepositoryMaps().ToList();
            Assert.IsTrue(maps.Count >= 5, "マップが見つからない");
            foreach (var path in maps)
            {
                var bytes = File.ReadAllBytes(path);
                var doc = MapDocument.Parse(bytes);
                CollectionAssert.AreEqual(bytes, Encoding.UTF8.GetBytes(doc.Serialize()), path);
            }
        }

        [TestMethod]
        public void RepositoryMaps_ReadTheSameAsSrcCore()
        {
            foreach (var path in RepositoryMaps())
            {
                var doc = MapDocument.Parse(File.ReadAllBytes(path));
                var src = NewSrc();
                src.Map.LoadMapData(path);
                Assert.AreEqual(src.Map.MapWidth, doc.Width, path);
                Assert.AreEqual(src.Map.MapHeight, doc.Height, path);
                for (var x = 1; x <= doc.Width; x++)
                {
                    for (var y = 1; y <= doc.Height; y++)
                    {
                        var cell = src.Map.MapData[x, y];
                        Assert.AreEqual(new MapTile(cell.TerrainType, cell.BitmapNo), doc[x, y], $"{path} ({x},{y})");
                        var layer = doc.Layer == null ? MapTile.Empty : doc.Layer[x - 1, y - 1];
                        Assert.AreEqual(new MapTile(cell.LayerType, cell.LayerBitmapNo), layer, $"{path} ({x},{y}) のレイヤー");
                    }
                }
            }
        }

        [TestMethod]
        public void NewMap_IsReadBySrcCore()
        {
            var doc = new MapDocument(15, 16, new MapTile(0, 0));
            doc[3, 4] = new MapTile(2, 1);
            doc.SetLayerEnabled(true);
            doc.Layer[4, 5] = new MapTile(11, 0);
            var path = Path.Combine(Path.GetTempPath(), "srcs-map-" + Guid.NewGuid() + ".map");
            try
            {
                File.WriteAllText(path, doc.Serialize());
                var src = NewSrc();
                src.Map.LoadMapData(path);
                Assert.AreEqual(15, src.Map.MapWidth);
                Assert.AreEqual(16, src.Map.MapHeight);
                Assert.AreEqual(2, src.Map.MapData[3, 4].TerrainType);
                Assert.AreEqual(1, src.Map.MapData[3, 4].BitmapNo);
                Assert.AreEqual(11, src.Map.MapData[5, 6].LayerType);
                Assert.AreEqual(MapTile.NoLayer, src.Map.MapData[1, 1].LayerType);
                StringAssert.StartsWith(doc.Serialize(), "MapData\r\n\r\n15,16\r\n");
            }
            finally
            {
                File.Delete(path);
            }
        }

        [TestMethod]
        public void Resize_KeepsTopLeftAndFills()
        {
            var doc = MapDocument.Parse("MapData\n\n2,2\n1,0\n2,0\n3,0\n4,0\n");
            Assert.AreEqual(new MapTile(2, 0), doc[1, 2]);
            doc.Resize(3, 1, new MapTile(9, 9));
            Assert.AreEqual("MapData\n\n3,1\n1,0\n3,0\n9,9\n", doc.Serialize());
        }

        [TestMethod]
        public void Legacy_Is20x20WithoutHeader()
        {
            var text = string.Join("\n", Enumerable.Repeat("0,0", 400));
            var doc = MapDocument.Parse(text);
            Assert.IsTrue(doc.Legacy);
            Assert.AreEqual(20, doc.Width);
            Assert.AreEqual(text, doc.Serialize());
        }

        [TestMethod]
        public void Parse_ReportsTheLine()
        {
            var ex = Assert.ThrowsExactly<InvalidDataException>(() => MapDocument.Parse("MapData\n\n1,2\n0,0\nx\n"));
            StringAssert.Contains(ex.Message, "5 行目");
        }

        [TestMethod]
        public void TerrainCatalog_ScenarioOverridesAndImagesFollowSrcOrder()
        {
            var app = Encoding.UTF8.GetBytes("0\n平地, plain\n陸, 1, 0, 0\n\n15\n山, mountain\n陸, 3, 30, 30\n");
            var scenario = Encoding.UTF8.GetBytes("15\n高山, peak\n陸, -, 40, 40\n");
            var errors = new List<string>();
            var catalog = TerrainCatalog.Load(new[] { ("app/Data/System/terrain.txt", app), ("scenario/Data/System/terrain.txt", scenario) }, errors);
            Assert.AreEqual(0, errors.Count, string.Join("\n", errors));
            var t = catalog.Terrains;
            Assert.AreEqual(2, t.Count);
            Assert.AreEqual("高山", t[1].Name);
            Assert.AreEqual("-", t[1].MoveCost);
            Assert.AreEqual("1", t[0].MoveCost);

            var roots = new[]
            {
                new ImageIndex(new[] { "Bitmap/Map/plain0001.bmp" }),
                new ImageIndex(new[] { @"Bitmap\Map\plain\plain0000.bmp", "Bitmap/Map/plain/plain0001.bmp", "Bitmap/Map/plain2.bmp" }),
            };
            // フォルダ（シナリオ → SRC 本体）の順が、候補の順より先
            Assert.AreEqual((0, "Bitmap/Map/plain0001.bmp"), TerrainCatalog.FindImage("plain", 1, roots));
            Assert.AreEqual((1, @"Bitmap\Map\plain\plain0000.bmp"), TerrainCatalog.FindImage("PLAIN", 0, roots));
            Assert.AreEqual((1, "Bitmap/Map/plain2.bmp"), TerrainCatalog.FindImage("plain", 2, roots));
            Assert.IsNull(TerrainCatalog.FindImage("plain", 3, roots));
            CollectionAssert.AreEqual(new[] { 0, 1, 2 }, TerrainCatalog.ImageNumbers("plain", roots).ToArray());
        }
    }
}
