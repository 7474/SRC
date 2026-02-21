using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Extensions;
using System.IO;
using System.Text;

namespace SRCCore.Extensions.Tests
{
    /// <summary>
    /// StreamExtension クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class StreamExtensionTests
    {
        // ──────────────────────────────────────────────
        // ToMemoryStream
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToMemoryStream_CopiesAllContent()
        {
            var content = "Hello, World!"u8.ToArray();
            var sourceStream = new MemoryStream(content);
            var result = sourceStream.ToMemoryStream();

            Assert.AreEqual(content.Length, result.Length);
            Assert.AreEqual(0, result.Position);
            var resultBytes = result.ToArray();
            CollectionAssert.AreEqual(content, resultBytes);
        }

        [TestMethod]
        public void ToMemoryStream_PositionIsResetToZero()
        {
            var content = "Test content"u8.ToArray();
            var sourceStream = new MemoryStream(content);
            var result = sourceStream.ToMemoryStream();

            Assert.AreEqual(0, result.Position);
        }

        [TestMethod]
        public void ToMemoryStream_EmptyStream_ReturnsEmptyMemoryStream()
        {
            var sourceStream = new MemoryStream();
            var result = sourceStream.ToMemoryStream();

            Assert.AreEqual(0, result.Length);
            Assert.AreEqual(0, result.Position);
        }

        [TestMethod]
        public void ToMemoryStream_CanReadCopiedContent()
        {
            var originalText = "SRC Unit Test Stream Data";
            var content = Encoding.UTF8.GetBytes(originalText);
            var sourceStream = new MemoryStream(content);

            var result = sourceStream.ToMemoryStream();

            using var reader = new StreamReader(result);
            var readText = reader.ReadToEnd();
            Assert.AreEqual(originalText, readText);
        }

        [TestMethod]
        public void ToMemoryStream_JapaneseContent_PreservesEncoding()
        {
            var originalText = "テスト用日本語テキスト";
            var content = Encoding.UTF8.GetBytes(originalText);
            var sourceStream = new MemoryStream(content);

            var result = sourceStream.ToMemoryStream();

            using var reader = new StreamReader(result, Encoding.UTF8);
            var readText = reader.ReadToEnd();
            Assert.AreEqual(originalText, readText);
        }

        [TestMethod]
        public void ToMemoryStream_LargeContent_CopiesCorrectly()
        {
            // 1KB of data
            var content = new byte[1024];
            for (int i = 0; i < content.Length; i++)
            {
                content[i] = (byte)(i % 256);
            }
            var sourceStream = new MemoryStream(content);

            var result = sourceStream.ToMemoryStream();

            Assert.AreEqual(1024, result.Length);
            var resultBytes = result.ToArray();
            CollectionAssert.AreEqual(content, resultBytes);
        }

        [TestMethod]
        public void ToMemoryStream_ResultIsWritable()
        {
            var content = "initial content"u8.ToArray();
            var sourceStream = new MemoryStream(content);
            var result = sourceStream.ToMemoryStream();

            // Seek to end and write additional data
            result.Seek(0, SeekOrigin.End);
            result.WriteByte(0xFF);

            // The memory stream should now be larger
            Assert.AreEqual(content.Length + 1, result.Length);
        }

        [TestMethod]
        public void ToMemoryStream_MultiLineContent_CopiesAllLines()
        {
            var lines = "line1\nline2\nline3\n";
            var content = Encoding.UTF8.GetBytes(lines);
            var sourceStream = new MemoryStream(content);

            var result = sourceStream.ToMemoryStream();

            using var reader = new StreamReader(result, Encoding.UTF8);
            Assert.AreEqual("line1", reader.ReadLine());
            Assert.AreEqual("line2", reader.ReadLine());
            Assert.AreEqual("line3", reader.ReadLine());
        }
    }
}
