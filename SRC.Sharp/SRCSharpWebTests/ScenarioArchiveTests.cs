using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;

namespace SRCSharpWeb.Tests
{
    /// <summary>シナリオ・素材の zip の展開（ScenarioArchive）</summary>
    [TestClass]
    public class ScenarioArchiveTests
    {
        private string dir;

        [TestInitialize]
        public void Setup()
        {
            dir = Path.Combine(Path.GetTempPath(), "srcweb-zip-" + Guid.NewGuid().ToString("N"));
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
        }

        /// <summary>zip を作る。encoding を指定するとファイル名をその文字コードで書く（UTF-8 以外なら UTF-8 の印を付けない）</summary>
        private static byte[] MakeZip(Encoding encoding, params (string Name, string Text)[] entries)
        {
            using (var stream = new MemoryStream())
            {
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true, encoding))
                {
                    foreach (var (name, text) in entries)
                    {
                        var entry = archive.CreateEntry(name);
                        if (text != null)
                        {
                            using (var writer = new StreamWriter(entry.Open(), new UTF8Encoding(false)))
                            {
                                writer.Write(text);
                            }
                        }
                    }
                }
                return stream.ToArray();
            }
        }

        /// <summary>UTF-8 の印（汎用フラグのビット 11）を消す。macOS・Linux の zip のように、UTF-8 の名前に印が無い zip にする</summary>
        private static byte[] ClearUtf8Flag(byte[] zip)
        {
            for (var i = 0; i + 4 <= zip.Length; i++)
            {
                var signature = BitConverter.ToUInt32(zip, i);
                // ローカルファイルヘッダ（フラグは +6）・セントラルディレクトリ（フラグは +8）
                var offset = signature == 0x04034b50 ? 6 : signature == 0x02014b50 ? 8 : -1;
                if (offset > 0)
                {
                    zip[i + offset + 1] &= unchecked((byte)~0x08);
                }
            }
            return zip;
        }

        private static void Extract(string dir, byte[] zip, string root = null, CancellationToken token = default)
        {
            using (var stream = new MemoryStream(zip, false))
            {
                ScenarioArchive.ExtractTo(dir, stream, root, token);
            }
        }

        private string Read(string name) => File.ReadAllText(Path.Combine(dir, name));

        [TestMethod]
        public void ExtractTo_ReadsShiftJisNames()
        {
            // Windows で作った zip（Shift_JIS の名前、UTF-8 の印なし）
            Extract(dir, MakeZip(ScenarioArchive.ShiftJis, ("シナリオ/第一話.eve", "一"), ("Data/ﾃｽﾄ.txt", "半角")));
            Assert.AreEqual("一", Read("シナリオ/第一話.eve"));
            Assert.AreEqual("半角", Read("Data/ﾃｽﾄ.txt"));
        }

        [TestMethod]
        public void ExtractTo_ReadsUtf8NamesWithoutFlag()
        {
            var zip = ClearUtf8Flag(MakeZip(Encoding.UTF8, ("第一話.eve", "一"), ("Bitmap/顔.bmp", "画")));
            Extract(dir, zip);
            Assert.AreEqual("一", Read("第一話.eve"));
            Assert.AreEqual("画", Read("Bitmap/顔.bmp"));
        }

        [TestMethod]
        public void ExtractTo_ReadsUtf8NamesWithFlag()
        {
            Extract(dir, MakeZip(Encoding.UTF8, ("第一話.eve", "一")));
            Assert.AreEqual("一", Read("第一話.eve"));
        }

        [TestMethod]
        public void ExtractTo_RootExtractsOnlyThatFolder()
        {
            var zip = MakeZip(Encoding.UTF8, ("game/", null), ("game/test.eve", "e"), ("game/Data/a.txt", "a"), ("other/b.txt", "b"), ("readme.txt", "r"));
            // 区切りの \ と前後の / は許す
            Extract(dir, zip, "\\game/");
            Assert.AreEqual("e", Read("test.eve"));
            Assert.AreEqual("a", Read("Data/a.txt"));
            CollectionAssert.AreEquivalent(new[] { "test.eve", "Data/a.txt" },
                Directory.GetFiles(dir, "*", SearchOption.AllDirectories).Select(x => Path.GetRelativePath(dir, x)).ToArray());
        }

        [TestMethod]
        public void ExtractTo_IgnoresEntriesOutsideTheFolder()
        {
            var zip = MakeZip(Encoding.UTF8, ("../evil.srcs", "x"), ("a/../../evil2.srcs", "x"), ("ok.eve", "o"), ("Data\\b.txt", "b"));
            Extract(dir, zip);
            Assert.AreEqual("o", Read("ok.eve"));
            // Windows の区切り（\）のエントリも / として展開する
            Assert.AreEqual("b", Read("Data/b.txt"));
            Assert.IsFalse(File.Exists(Path.Combine(dir, "..", "evil.srcs")));
            Assert.IsFalse(File.Exists(Path.Combine(dir, "..", "evil2.srcs")));
        }

        [TestMethod]
        public void ExtractTo_OverwritesAndOverlays()
        {
            // 素材を重ねる（ExtractAsset）ときは、後から展開したものが優先する
            Extract(dir, MakeZip(Encoding.UTF8, ("Bitmap/a.bmp", "1"), ("Bitmap/b.bmp", "1")));
            Extract(dir, MakeZip(Encoding.UTF8, ("Bitmap/a.bmp", "2")));
            Assert.AreEqual("2", Read("Bitmap/a.bmp"));
            Assert.AreEqual("1", Read("Bitmap/b.bmp"));
        }

        [TestMethod]
        public void ExtractTo_StopsWhenCancelled()
        {
            using (var cts = new CancellationTokenSource())
            {
                cts.Cancel();
                Assert.ThrowsExactly<OperationCanceledException>(() => Extract(dir, MakeZip(Encoding.UTF8, ("a.eve", "a")), null, cts.Token));
            }
            Assert.IsFalse(File.Exists(Path.Combine(dir, "a.eve")));
        }

        [TestMethod]
        public void ExtractTo_RejectsBrokenZip()
        {
            Assert.ThrowsExactly<InvalidDataException>(() => Extract(dir, Encoding.ASCII.GetBytes("not a zip")));
        }

        [TestMethod]
        public void FindEventFiles_ShallowFirstThenByName()
        {
            Extract(dir, MakeZip(Encoding.UTF8, ("sub/deep/c.eve", ""), ("sub/a.eve", ""), ("b.eve", ""), ("a.eve", ""), ("Data/x.txt", "")));
            CollectionAssert.AreEqual(new[] { "a.eve", "b.eve", "sub/a.eve", "sub/deep/c.eve" }, ScenarioArchive.FindEventFiles(dir).ToArray());
        }

        [TestMethod]
        public void ShiftJisIsCodePage932()
        {
            Assert.AreEqual(932, ScenarioArchive.ShiftJis.CodePage);
        }
    }
}
