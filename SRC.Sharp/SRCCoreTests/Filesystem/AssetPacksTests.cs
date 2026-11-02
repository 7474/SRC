using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Filesystem;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace SRCCore.Filesystem.Tests
{
    /// <summary>
    /// 素材パック（SRC のフォルダの Packs フォルダの zip を、展開せずに SRC のフォルダに重ねる）。
    /// </summary>
    [TestClass]
    public class AssetPacksTests
    {
        private string appPath;

        [TestInitialize]
        public void Initialize()
        {
            appPath = Path.Combine(Path.GetTempPath(), "srcs-packs-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(appPath, AssetPacks.FolderName));
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete(appPath, true);
        }

        /// <summary>Packs フォルダに zip を作る。名前は encoding（Shift_JIS なら UTF-8 の印なし）で書く</summary>
        private string WritePack(string name, Encoding encoding, params (string Name, string Text)[] entries)
        {
            var path = Path.Combine(appPath, AssetPacks.FolderName, name);
            using (var archive = ZipFile.Open(path, ZipArchiveMode.Create, encoding))
            {
                foreach (var (entryName, text) in entries)
                {
                    using (var writer = new StreamWriter(archive.CreateEntry(entryName).Open()))
                    {
                        writer.Write(text);
                    }
                }
            }
            return path;
        }

        private string Read(LocalFileSystem fs, params string[] paths)
        {
            using (var reader = new StreamReader(fs.Open(paths)))
            {
                return reader.ReadToEnd();
            }
        }

        [TestMethod]
        public void FindRoot_DetectsTopLevelOrSingleFolder()
        {
            var top = WritePack("top.zip", Encoding.UTF8, ("Bitmap/Unit/a.bmp", "a"));
            var nested = WritePack("nested.zip", Encoding.UTF8, ("汎用グラフィック集/Bitmap/Unit/a.bmp", "a"), ("汎用グラフィック集/readme.txt", "r"));
            var none = WritePack("none.zip", Encoding.UTF8, ("readme.txt", "r"), ("other/x.txt", "x"));

            Assert.AreEqual("", AssetPacks.FindRoot(top));
            Assert.AreEqual("汎用グラフィック集/", AssetPacks.FindRoot(nested));
            Assert.IsNull(AssetPacks.FindRoot(none));
        }

        [TestMethod]
        public void Mount_OverlaysPacksOnAppPath()
        {
            WritePack("a.zip", Encoding.UTF8,
                ("Pack/Bitmap/Unit/Unit.bmp", "a"),
                ("Pack/Data/System/terrain.txt", "terrain-a"),
                ("Pack/Midi/Map1.mid", "a"));
            // Windows で作った zip（Shift_JIS の名前）。名前が後ろなので a.zip より優先する
            WritePack("b.zip", Encoding.GetEncoding(932),
                ("Bitmap/Event/爆発.bmp", "b"),
                ("Midi/Map1.mid", "b"));
            WritePack("readme.zip", Encoding.UTF8, ("readme.txt", "r"));
            // SRC のフォルダに実在するファイルは素材パックより優先する
            Directory.CreateDirectory(Path.Combine(appPath, "Data", "System"));
            File.WriteAllText(Path.Combine(appPath, "Data", "System", "terrain.txt"), "terrain-app");

            using var fs = new LocalFileSystem();
            fs.AddPath(appPath);
            var skipped = new System.Collections.Generic.List<string>();
            var mounted = AssetPacks.Mount(fs, appPath, (zip, ex) => skipped.Add(Path.GetFileName(zip)));

            CollectionAssert.AreEqual(new[] { "a.zip", "b.zip" }, mounted.Select(Path.GetFileName).ToArray());
            CollectionAssert.AreEqual(new[] { "readme.zip" }, skipped);
            // 大文字・小文字を区別しない
            Assert.IsTrue(fs.FileExists(appPath, "Bitmap", "unit", "UNIT.bmp"));
            Assert.AreEqual("a", Read(fs, appPath, "Bitmap", "Unit", "Unit.bmp"));
            Assert.AreEqual("b", Read(fs, appPath, "Bitmap", "Event", "爆発.bmp"));
            Assert.AreEqual("b", Read(fs, appPath, "Midi", "Map1.mid"));
            Assert.AreEqual("terrain-app", Read(fs, appPath, "Data", "System", "terrain.txt"));
            // zip の中の最上位のフォルダ（Pack）は見えない
            Assert.IsFalse(fs.FileExists(appPath, "Pack", "Midi", "Map1.mid"));
        }

        [TestMethod]
        public void Archive_ListsFilesAndFoldersUnderRoot()
        {
            var zip = WritePack("a.zip", Encoding.UTF8,
                ("Pack/Bitmap/Unit/a.bmp", "a"),
                ("Pack/Bitmap/Unit/b.bmp", "b"),
                ("Pack/Bitmap/Map/plain.bmp", "p"));
            using (var archive = ZipFile.OpenRead(zip))
            {
                var set = new LocalFileSystemArchive(appPath, archive, "Pack");
                var files = set.GetFileSystemEntries(Path.Combine(appPath, "Bitmap", "Unit"), "*.bmp", EntryOption.File).Select(Path.GetFileName).ToArray();
                CollectionAssert.AreEqual(new[] { "a.bmp", "b.bmp" }, files);
                // フォルダの項目が無い zip でもフォルダを列挙する
                var dirs = set.GetFileSystemEntries(Path.Combine(appPath, "Bitmap"), "*", EntryOption.Directory).Select(Path.GetFileName).ToArray();
                CollectionAssert.AreEqual(new[] { "Map", "Unit" }, dirs);
            }
        }
    }
}
