using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace SRCCore.Filesystem.Tests
{
    /// <summary>
    /// LocalFileSystem の書き込みの通知（FileWritten）のテスト
    /// </summary>
    [TestClass]
    public class FileWrittenTests
    {
        private string dir;

        [TestInitialize]
        public void Setup()
        {
            dir = Path.Combine(Path.GetTempPath(), "srcfw-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete(dir, true);
        }

        [TestMethod]
        public void Create_RaisesFileWrittenOnClose()
        {
            var fs = new LocalFileSystem();
            var written = new List<string>();
            fs.FileWritten += written.Add;
            var path = Path.Combine(dir, "a.srcs");

            using (var stream = fs.Create(path))
            {
                stream.WriteByte(1);
                Assert.AreEqual(0, written.Count);
            }

            CollectionAssert.AreEqual(new[] { Path.GetFullPath(path) }, written);
            Assert.AreEqual(1, new FileInfo(path).Length);
        }

        [TestMethod]
        public void Create_TruncatesExistingFile()
        {
            var fs = new LocalFileSystem();
            var path = Path.Combine(dir, "a.srcs");
            File.WriteAllBytes(path, new byte[10]);

            using (var stream = fs.Create(path))
            {
                stream.WriteByte(1);
            }

            Assert.AreEqual(1, new FileInfo(path).Length);
        }

        [TestMethod]
        public void OpenSafe_WriteAndAppend_RaiseFileWritten_ReadDoesNot()
        {
            var fs = new LocalFileSystem();
            fs.AddSafePath(dir);
            var written = new List<string>();
            fs.FileWritten += written.Add;

            using (var stream = fs.OpenSafe(SafeOpenMode.Write, "b.txt"))
            {
                stream.WriteByte(1);
            }
            using (var stream = fs.OpenSafe(SafeOpenMode.Append, "b.txt"))
            {
                stream.WriteByte(2);
            }
            using (var stream = fs.OpenSafe(SafeOpenMode.Read, "b.txt"))
            {
                Assert.AreEqual(1, stream.ReadByte());
            }

            var full = Path.GetFullPath(Path.Combine(dir, "b.txt"));
            CollectionAssert.AreEqual(new[] { full, full }, written);
        }

        [TestMethod]
        public void Dispose_Twice_RaisesOnce()
        {
            var fs = new LocalFileSystem();
            var count = 0;
            fs.FileWritten += _ => count++;

            var stream = fs.Create(Path.Combine(dir, "c.srcq"));
            stream.Dispose();
            stream.Dispose();

            Assert.AreEqual(1, count);
        }
    }
}
