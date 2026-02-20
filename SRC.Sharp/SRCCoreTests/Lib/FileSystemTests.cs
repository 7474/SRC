using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using System;
using System.IO;

namespace SRCCore.Lib.Tests
{
    [TestClass]
    public class FileSystemTests
    {
        private string _tempDir;

        [TestInitialize]
        public void Setup()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "SRCCoreTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_tempDir))
            {
                Directory.Delete(_tempDir, true);
            }
        }

        [TestMethod]
        public void Dir_ReturnsFileInfo_WhenFileExists()
        {
            var filePath = Path.Combine(_tempDir, "test.txt");
            File.WriteAllText(filePath, "content");

            var result = FileSystem.Dir(filePath);
            Assert.AreNotEqual("", result);
        }

        [TestMethod]
        public void Dir_ReturnsEmptyString_WhenFileNotExists()
        {
            var result = FileSystem.Dir(Path.Combine(_tempDir, "nonexistent.txt"));
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void Dir_ReturnsDirectoryInfo_WhenDirectoryExistsAndFlagSet()
        {
            var result = FileSystem.Dir(_tempDir, FileAttribute.Directory);
            Assert.AreNotEqual("", result);
        }

        [TestMethod]
        public void Dir_ReturnsEmptyString_WhenDirectoryNotExistsAndFlagSet()
        {
            var result = FileSystem.Dir(Path.Combine(_tempDir, "nonexistent_dir"), FileAttribute.Directory);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void Dir_ThrowsNotSupported_WhenVolumeAttribute()
        {
            Assert.ThrowsException<NotSupportedException>(() =>
                FileSystem.Dir(_tempDir, FileAttribute.Volume));
        }

        [TestMethod]
        public void Dir_ReturnsEmptyString_ForDirectoryWithoutDirectoryFlag()
        {
            // Without Directory flag, a directory path should not match as a file
            var result = FileSystem.Dir(_tempDir);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void Dir_ReturnsFileInfo_WhenFileExistsWithDirectoryFlag()
        {
            var filePath = Path.Combine(_tempDir, "test.txt");
            File.WriteAllText(filePath, "content");

            // Directory flag also checks for files
            var result = FileSystem.Dir(filePath, FileAttribute.Directory);
            Assert.AreNotEqual("", result);
        }
    }
}
