using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Filesystem;
using System.IO;
using System.Text;

namespace SRCCore.Filesystem.Tests
{
    /// <summary>
    /// FileHandle クラスの直接テスト（FileHandleManagerTests で未カバーの項目）
    /// </summary>
    [TestClass]
    public class FileHandleTests
    {
        // ──────────────────────────────────────────────
        // FileHandle コンストラクタ - Read モード
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FileHandle_Constructor_ReadMode_HandleIsSet()
        {
            var content = Encoding.UTF8.GetBytes("hello");
            using var stream = new MemoryStream(content);
            var fh = new FileHandle(3, stream, SafeOpenMode.Read, SRCCompatibilityMode.None);
            Assert.AreEqual(3, fh.Handle);
        }

        [TestMethod]
        public void FileHandle_Constructor_ReadMode_ReaderIsNotNull()
        {
            var content = Encoding.UTF8.GetBytes("line1\nline2");
            using var stream = new MemoryStream(content);
            var fh = new FileHandle(1, stream, SafeOpenMode.Read, SRCCompatibilityMode.None);
            Assert.IsNotNull(fh.Reader);
            Assert.IsNull(fh.Writer);
        }

        [TestMethod]
        public void FileHandle_Constructor_ReadMode_StreamIsSet()
        {
            var content = Encoding.UTF8.GetBytes("data");
            using var stream = new MemoryStream(content);
            var fh = new FileHandle(1, stream, SafeOpenMode.Read, SRCCompatibilityMode.None);
            Assert.IsNotNull(fh.Stream);
        }

        // ──────────────────────────────────────────────
        // FileHandle コンストラクタ - Write モード
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FileHandle_Constructor_WriteMode_HandleIsSet()
        {
            using var stream = new MemoryStream();
            var fh = new FileHandle(5, stream, SafeOpenMode.Write, SRCCompatibilityMode.None);
            Assert.AreEqual(5, fh.Handle);
        }

        [TestMethod]
        public void FileHandle_Constructor_WriteMode_WriterIsNotNull()
        {
            using var stream = new MemoryStream();
            var fh = new FileHandle(1, stream, SafeOpenMode.Write, SRCCompatibilityMode.None);
            Assert.IsNotNull(fh.Writer);
            Assert.IsNull(fh.Reader);
        }

        [TestMethod]
        public void FileHandle_Constructor_WriteMode_StreamIsSet()
        {
            using var stream = new MemoryStream();
            var fh = new FileHandle(2, stream, SafeOpenMode.Write, SRCCompatibilityMode.None);
            Assert.IsNotNull(fh.Stream);
        }

        // ──────────────────────────────────────────────
        // FileHandle.Handle プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FileHandle_Handle_ReturnsExactValuePassedToConstructor()
        {
            using var stream = new MemoryStream();
            var fh = new FileHandle(42, stream, SafeOpenMode.Write, SRCCompatibilityMode.None);
            Assert.AreEqual(42, fh.Handle);
        }

        [TestMethod]
        public void FileHandle_Handle_DifferentValues_AreIndependent()
        {
            using var s1 = new MemoryStream();
            using var s2 = new MemoryStream();
            var fh1 = new FileHandle(10, s1, SafeOpenMode.Write, SRCCompatibilityMode.None);
            var fh2 = new FileHandle(20, s2, SafeOpenMode.Write, SRCCompatibilityMode.None);
            Assert.AreEqual(10, fh1.Handle);
            Assert.AreEqual(20, fh2.Handle);
        }

        // ──────────────────────────────────────────────
        // FileHandle.Dispose
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FileHandle_Dispose_DoesNotThrow()
        {
            var stream = new MemoryStream();
            var fh = new FileHandle(1, stream, SafeOpenMode.Write, SRCCompatibilityMode.None);
            // Dispose は例外を投げないこと
            fh.Dispose();
        }

        [TestMethod]
        public void FileHandle_Dispose_ClosesStream()
        {
            var stream = new MemoryStream();
            var fh = new FileHandle(1, stream, SafeOpenMode.Write, SRCCompatibilityMode.None);
            fh.Dispose();
            // MemoryStream は Dispose 後 CanRead=false
            Assert.IsFalse(stream.CanRead);
        }

        // ──────────────────────────────────────────────
        // FileHandleManager - 複数ハンドルの追加とクローズ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FileHandleManager_AddMultiple_AllRetrievable()
        {
            var manager = new FileHandleManager();
            var h1 = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, new MemoryStream());
            var h2 = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, new MemoryStream());
            var h3 = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, new MemoryStream());

            Assert.IsNotNull(manager.Get(h1.Handle));
            Assert.IsNotNull(manager.Get(h2.Handle));
            Assert.IsNotNull(manager.Get(h3.Handle));
        }

        [TestMethod]
        public void FileHandleManager_CloseFirst_OthersRemain()
        {
            var manager = new FileHandleManager();
            var h1 = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, new MemoryStream());
            var h2 = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, new MemoryStream());
            var h3 = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, new MemoryStream());

            manager.Close(h1.Handle);

            Assert.IsNull(manager.Get(h1.Handle));
            Assert.IsNotNull(manager.Get(h2.Handle));
            Assert.IsNotNull(manager.Get(h3.Handle));
        }

        [TestMethod]
        public void FileHandleManager_CloseLast_OthersRemain()
        {
            var manager = new FileHandleManager();
            var h1 = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, new MemoryStream());
            var h2 = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, new MemoryStream());
            var h3 = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, new MemoryStream());

            manager.Close(h3.Handle);

            Assert.IsNotNull(manager.Get(h1.Handle));
            Assert.IsNotNull(manager.Get(h2.Handle));
            Assert.IsNull(manager.Get(h3.Handle));
        }

        [TestMethod]
        public void FileHandleManager_HandleNumbering_StartsAtOne()
        {
            var manager = new FileHandleManager();
            var h = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, new MemoryStream());
            Assert.AreEqual(1, h.Handle);
        }

        [TestMethod]
        public void FileHandleManager_GetUnknown_ReturnsNull()
        {
            var manager = new FileHandleManager();
            // まだ何も追加していない状態で Get
            Assert.IsNull(manager.Get(0));
            Assert.IsNull(manager.Get(-1));
        }

        // ──────────────────────────────────────────────
        // FileHandle - 読み取り動作 (直接構築)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FileHandle_ReadMode_CanReadLine()
        {
            var content = Encoding.UTF8.GetBytes("直接構築テスト\n");
            using var stream = new MemoryStream(content);
            var fh = new FileHandle(1, stream, SafeOpenMode.Read, SRCCompatibilityMode.None);
            var line = fh.Reader.ReadLine();
            Assert.AreEqual("直接構築テスト", line);
        }

        // ──────────────────────────────────────────────
        // FileHandle - 書き込み動作 (直接構築)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FileHandle_WriteMode_CanWriteAndFlush()
        {
            using var stream = new MemoryStream();
            var fh = new FileHandle(1, stream, SafeOpenMode.Write, SRCCompatibilityMode.None);
            fh.Writer.Write("書き込みテスト");
            fh.Writer.Flush();
            var result = Encoding.UTF8.GetString(stream.ToArray()).TrimStart('\uFEFF');
            Assert.AreEqual("書き込みテスト", result);
        }
    }
}
