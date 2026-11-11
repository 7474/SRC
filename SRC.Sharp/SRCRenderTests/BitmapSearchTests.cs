using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using SRCCore;
using SRCCore.Filesystem;
using System;
using System.IO;
using System.Text;

namespace SRCRender.Tests
{
    /// <summary>
    /// 画像の検索の順番（7474/SRC#1468・#1482）。
    /// 元の SRC（GUI.bas の DrawPicture・FindUnitBitmap、Map.bas の SearchTerrainImageFile）と同じく、
    /// シナリオのフォルダ → ExtDataPath → SRC のフォルダの順にフォルダを外側にして探し、
    /// 地形画像のフォルダ（Bitmap\Map\plain など）と 4 桁の番号付きのファイル名も探す。
    /// </summary>
    [TestClass]
    public class BitmapSearchTests
    {
        private string root;
        private string appDir;
        private string extDir;
        private string scenarioDir;
        private LocalFileSystem fs;
        private SRC src;

        [TestInitialize]
        public void Setup()
        {
            root = Path.Combine(Path.GetTempPath(), "srcs-bitmapsearch-" + Guid.NewGuid());
            appDir = Path.Combine(root, "app");
            extDir = Path.Combine(root, "ext");
            scenarioDir = Path.Combine(root, "scenario");
            foreach (var dir in new[] { appDir, extDir, scenarioDir })
            {
                Directory.CreateDirectory(dir);
            }
            fs = new LocalFileSystem();
            // 元の SRC と同じ並び（後に加えたものほど先に探す）
            fs.AddPath(appDir);
            fs.AddPath(extDir);
            fs.AddPath(scenarioDir);
            src = new SRC { FileSystem = fs };
            var terrain = "0\r\n平地, plain\r\n陸, 1, 0, 0\r\n\r\n";
            using (var stream = new MemoryStream(new UTF8Encoding(false).GetBytes(terrain)))
            {
                src.TDList.Load("terrain.txt", stream);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            fs.Dispose();
            Directory.Delete(root, true);
        }

        private void Touch(string dir, params string[] paths)
        {
            var path = Path.Combine(dir, Path.Combine(paths));
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, dir);
        }

        /// <summary>見つかったファイルの中身（置いたフォルダ）</summary>
        private string FoundIn(string path)
        {
            Assert.IsNotNull(path);
            using (var reader = new StreamReader(fs.Open(path)))
            {
                return reader.ReadToEnd();
            }
        }

        private static string Native(string path) => path.Replace('\\', Path.DirectorySeparatorChar);

        [TestMethod]
        public void FindFirstFile_SearchesFolderBeforeCandidates()
        {
            Touch(appDir, "Bitmap", "Map", "plain", "plain0000.bmp");
            Touch(scenarioDir, "Bitmap", "Map", "plain0000.bmp");
            var found = fs.FindFirstFile(new[]
            {
                Path.Combine("Bitmap", "Map", "plain", "plain0000.bmp"),
                Path.Combine("Bitmap", "Map", "plain0000.bmp"),
            });
            Assert.AreEqual(Path.Combine("Bitmap", "Map", "plain0000.bmp"), found);
            Assert.AreEqual(scenarioDir, FoundIn(found));
            Assert.IsNull(fs.FindFirstFile(new[] { "missing.bmp" }));
        }

        [TestMethod]
        public void FindPicture_ScenarioFolderComesFirst()
        {
            // SRC のフォルダの Bitmap\Event より、シナリオのフォルダの Bitmap\Unit を使う
            Touch(appDir, "Bitmap", "Event", "a.bmp");
            Touch(scenarioDir, "Bitmap", "Unit", "a.bmp");
            Assert.AreEqual(scenarioDir, FoundIn(BitmapSearch.FindPicture(src, "a.bmp")));

            // 拡張データのフォルダは SRC のフォルダより先
            Touch(appDir, "Bitmap", "Event", "b.bmp");
            Touch(extDir, "Bitmap", "Pilot", "b.bmp");
            Assert.AreEqual(extDir, FoundIn(BitmapSearch.FindPicture(src, "b.bmp")));

            // 同じフォルダの中では Anime、Event、Cutin、Pilot、Unit、Map の順
            Touch(scenarioDir, "Bitmap", "Unit", "c.bmp");
            Touch(scenarioDir, "Bitmap", "Pilot", "c.bmp");
            Assert.AreEqual(Native(@"Bitmap\Pilot\c.bmp"), Native(BitmapSearch.FindPicture(src, "c.bmp")));

            // フォルダ指定あり
            Touch(appDir, "Bitmap", "Pilot", "d.bmp");
            Assert.AreEqual(appDir, FoundIn(BitmapSearch.FindPicture(src, @"Pilot\d.bmp")));
            Assert.IsNull(BitmapSearch.FindPicture(src, "missing.bmp"));
        }

        [TestMethod]
        public void FindPicture_TerrainImageFolderAndFourFigures()
        {
            Touch(appDir, "Bitmap", "Map", "plain", "plain0001.bmp");
            Touch(appDir, "Bitmap", "Map", "plain0002.bmp");
            // 地形画像のフォルダ・4 桁の番号付きのファイル名
            Assert.AreEqual(Native(@"Bitmap\Map\plain\plain0001.bmp"), Native(BitmapSearch.FindPicture(src, "plain1.bmp")));
            Assert.AreEqual(Native(@"Bitmap\Map\plain0002.bmp"), Native(BitmapSearch.FindPicture(src, "plain2.bmp")));
            // Map\ 付きの指定
            Assert.AreEqual(Native(@"Bitmap\Map\plain\plain0001.bmp"), Native(BitmapSearch.FindPicture(src, @"Map\plain1.bmp")));
            // 地形のビットマップ名でなければ読み替えない
            Touch(appDir, "Bitmap", "Map", "rock", "rock0001.bmp");
            Assert.IsNull(BitmapSearch.FindPicture(src, "rock1.bmp"));
            // シナリオのフォルダにあればそちらを使う
            Touch(scenarioDir, "Bitmap", "Map", "plain1.bmp");
            Assert.AreEqual(scenarioDir, FoundIn(BitmapSearch.FindPicture(src, "plain1.bmp")));
        }

        [TestMethod]
        public void TerrainUnitBitmap_UsesMapFolder()
        {
            Touch(appDir, "Bitmap", "Map", "wall.bmp");
            Assert.AreEqual(Native(@"Bitmap\Map\wall.bmp"), Native(BitmapSearch.TerrainUnitBitmap(src, "wall.bmp")));
            // 地形画像の 4 桁の番号付きのファイル名に読み替える
            Assert.AreEqual(Native(@"Bitmap\Map\plain\plain0003.bmp"), Native(BitmapSearch.TerrainUnitBitmap(src, "plain3.bmp")));
            Touch(appDir, "Bitmap", "Map", "plain0004.bmp");
            Assert.AreEqual(Native(@"Bitmap\Map\plain0004.bmp"), Native(BitmapSearch.TerrainUnitBitmap(src, "plain4.bmp")));
            // Bitmap\Map に無ければユニット画像
            Assert.AreEqual(Native(@"Bitmap\Unit\unit.bmp"), Native(BitmapSearch.TerrainUnitBitmap(src, "unit.bmp")));
            Assert.AreEqual(Native(@"Bitmap\Other\unit.bmp"), Native(BitmapSearch.TerrainUnitBitmap(src, @"Other\unit.bmp")));
        }

        [TestMethod]
        public void ImageCache_UsesSearchOrder()
        {
            void WriteImage(string dir, SKColor color, params string[] paths)
            {
                var path = Path.Combine(dir, Path.Combine(paths));
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                using (var bitmap = new SKBitmap(4, 4))
                using (var file = File.Create(path))
                {
                    bitmap.Erase(color);
                    bitmap.Encode(file, SKEncodedImageFormat.Png, 100);
                }
            }
            WriteImage(appDir, SKColors.Red, "Bitmap", "Event", "e.png");
            WriteImage(scenarioDir, SKColors.Blue, "Bitmap", "Map", "e.png");
            var cache = new ImageCache(src);
            Assert.AreEqual(SKColors.Blue, cache.Get("e.png").GetPixel(0, 0));
        }
    }
}
