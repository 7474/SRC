using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Filesystem;
using System.IO;
using System.Text;

namespace SRCCore.Filesystem.Tests
{
    /// <summary>
    /// FileHandleManager と FileHandle クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class FileHandleManagerTests
    {
        // ──────────────────────────────────────────────
        // FileHandleManager.Add
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Add_Read_ReturnsHandleWithReader()
        {
            var manager = new FileHandleManager();
            var content = "test content"u8.ToArray();
            using var stream = new MemoryStream(content);

            var handle = manager.Add(SafeOpenMode.Read, SRCCompatibilityMode.None, stream);

            Assert.IsNotNull(handle);
            Assert.IsNotNull(handle.Reader);
            Assert.IsNull(handle.Writer);
        }

        [TestMethod]
        public void Add_Write_ReturnsHandleWithWriter()
        {
            var manager = new FileHandleManager();
            using var stream = new MemoryStream();

            var handle = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, stream);

            Assert.IsNotNull(handle);
            Assert.IsNull(handle.Reader);
            Assert.IsNotNull(handle.Writer);
        }

        [TestMethod]
        public void Add_AssignsUniqueHandleNumbers()
        {
            var manager = new FileHandleManager();

            var h1 = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, new MemoryStream());
            var h2 = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, new MemoryStream());
            var h3 = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, new MemoryStream());

            Assert.AreNotEqual(h1.Handle, h2.Handle);
            Assert.AreNotEqual(h2.Handle, h3.Handle);
            Assert.AreNotEqual(h1.Handle, h3.Handle);
        }

        [TestMethod]
        public void Add_HandleNumbersAreIncremental()
        {
            var manager = new FileHandleManager();

            var h1 = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, new MemoryStream());
            var h2 = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, new MemoryStream());

            Assert.AreEqual(h1.Handle + 1, h2.Handle);
        }

        // ──────────────────────────────────────────────
        // FileHandleManager.Get
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Get_ExistingHandle_ReturnsHandle()
        {
            var manager = new FileHandleManager();
            using var stream = new MemoryStream();
            var added = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, stream);

            var retrieved = manager.Get(added.Handle);

            Assert.IsNotNull(retrieved);
            Assert.AreEqual(added.Handle, retrieved.Handle);
        }

        [TestMethod]
        public void Get_NonExistingHandle_ReturnsNull()
        {
            var manager = new FileHandleManager();

            var result = manager.Get(9999);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Get_AfterClose_ReturnsNull()
        {
            var manager = new FileHandleManager();
            using var stream = new MemoryStream();
            var handle = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, stream);
            int handleNum = handle.Handle;

            manager.Close(handleNum);

            Assert.IsNull(manager.Get(handleNum));
        }

        // ──────────────────────────────────────────────
        // FileHandleManager.Close
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Close_ExistingHandle_RemovesFromManager()
        {
            var manager = new FileHandleManager();
            using var stream = new MemoryStream();
            var handle = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, stream);

            manager.Close(handle.Handle);

            Assert.IsNull(manager.Get(handle.Handle));
        }

        [TestMethod]
        public void Close_NonExistingHandle_NoException()
        {
            var manager = new FileHandleManager();
            // Should not throw
            manager.Close(9999);
        }

        [TestMethod]
        public void Close_OtherHandlesUnaffected()
        {
            var manager = new FileHandleManager();
            var h1 = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, new MemoryStream());
            var h2 = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, new MemoryStream());

            manager.Close(h1.Handle);

            Assert.IsNull(manager.Get(h1.Handle));
            Assert.IsNotNull(manager.Get(h2.Handle));
        }

        // ──────────────────────────────────────────────
        // FileHandle - Read functionality
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FileHandle_Read_CanReadContent()
        {
            var manager = new FileHandleManager();
            var content = Encoding.UTF8.GetBytes("Hello, World!\n");
            using var stream = new MemoryStream(content);
            var handle = manager.Add(SafeOpenMode.Read, SRCCompatibilityMode.None, stream);

            var line = handle.Reader.ReadLine();

            Assert.AreEqual("Hello, World!", line);
        }

        [TestMethod]
        public void FileHandle_Read_MultipleLines()
        {
            var manager = new FileHandleManager();
            var content = Encoding.UTF8.GetBytes("Line1\nLine2\nLine3\n");
            using var stream = new MemoryStream(content);
            var handle = manager.Add(SafeOpenMode.Read, SRCCompatibilityMode.None, stream);

            Assert.AreEqual("Line1", handle.Reader.ReadLine());
            Assert.AreEqual("Line2", handle.Reader.ReadLine());
            Assert.AreEqual("Line3", handle.Reader.ReadLine());
        }

        // ──────────────────────────────────────────────
        // FileHandle - Write functionality
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FileHandle_Write_CanWriteContent()
        {
            var manager = new FileHandleManager();
            using var stream = new MemoryStream();
            var handle = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, stream);

            handle.Writer.Write("Hello");
            handle.Writer.Flush();

            // UTF-8 BOM を除いて内容を確認
            var result = Encoding.UTF8.GetString(stream.ToArray()).TrimStart('\uFEFF');
            Assert.AreEqual("Hello", result);
        }

        [TestMethod]
        public void FileHandle_Write_MultipleWrites()
        {
            var manager = new FileHandleManager();
            using var stream = new MemoryStream();
            var handle = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, stream);

            handle.Writer.WriteLine("Line1");
            handle.Writer.WriteLine("Line2");
            handle.Writer.Flush();

            var result = Encoding.UTF8.GetString(stream.ToArray());
            Assert.IsTrue(result.Contains("Line1"));
            Assert.IsTrue(result.Contains("Line2"));
        }

        // ──────────────────────────────────────────────
        // FileHandle.Handle プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FileHandle_HandleProperty_ReturnsAssignedNumber()
        {
            var manager = new FileHandleManager();
            using var stream = new MemoryStream();
            var handle = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, stream);

            Assert.IsTrue(handle.Handle > 0);
        }

        [TestMethod]
        public void FileHandle_StreamProperty_ReturnsStream()
        {
            var manager = new FileHandleManager();
            using var stream = new MemoryStream();
            var handle = manager.Add(SafeOpenMode.Write, SRCCompatibilityMode.None, stream);

            Assert.IsNotNull(handle.Stream);
        }
    }
}
