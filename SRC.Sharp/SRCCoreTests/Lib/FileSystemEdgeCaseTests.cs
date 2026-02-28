using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using System.IO;
using System;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// FileSystem クラスのさらに詳細なエッジケーステスト
    /// </summary>
    [TestClass]
    public class FileSystemEdgeCaseTests
    {
        private string _tempDir;

        [TestInitialize]
        public void Setup()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "SRCCoreTests_Edge_" + Guid.NewGuid().ToString("N"));
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

        // ──────────────────────────────────────────────
        // Dir - 各種ファイル属性テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Dir_ExistingFile_ReturnsNonEmpty()
        {
            var file = Path.Combine(_tempDir, "test.dat");
            File.WriteAllText(file, "data");
            var result = FileSystem.Dir(file);
            Assert.AreNotEqual("", result);
        }

        [TestMethod]
        public void Dir_NonExistentFile_ReturnsEmpty()
        {
            var file = Path.Combine(_tempDir, "missing_file.dat");
            var result = FileSystem.Dir(file);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void Dir_DirectoryWithDirectoryFlag_ReturnsNonEmpty()
        {
            var result = FileSystem.Dir(_tempDir, FileAttribute.Directory);
            Assert.AreNotEqual("", result);
        }

        [TestMethod]
        public void Dir_FileWithDirectoryFlag_ReturnsNonEmpty()
        {
            var file = Path.Combine(_tempDir, "file_with_dir_flag.txt");
            File.WriteAllText(file, "content");
            var result = FileSystem.Dir(file, FileAttribute.Directory);
            Assert.AreNotEqual("", result);
        }

        [TestMethod]
        public void Dir_NonExistentDirectory_ReturnsEmpty()
        {
            var dir = Path.Combine(_tempDir, "non_existent_subdir");
            var result = FileSystem.Dir(dir, FileAttribute.Directory);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void Dir_VolumeAttribute_ThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(
                () => FileSystem.Dir(_tempDir, FileAttribute.Volume));
        }

        // ──────────────────────────────────────────────
        // FileAttribute 定数テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FileAttribute_Normal_IsZero()
        {
            Assert.AreEqual(0, (int)FileAttribute.Normal);
        }

        [TestMethod]
        public void FileAttribute_ReadOnly_IsOne()
        {
            Assert.AreEqual(1, (int)FileAttribute.ReadOnly);
        }

        [TestMethod]
        public void FileAttribute_Hidden_IsTwo()
        {
            Assert.AreEqual(2, (int)FileAttribute.Hidden);
        }

        [TestMethod]
        public void FileAttribute_System_IsFour()
        {
            Assert.AreEqual(4, (int)FileAttribute.System);
        }

        [TestMethod]
        public void FileAttribute_Volume_IsEight()
        {
            Assert.AreEqual(8, (int)FileAttribute.Volume);
        }

        [TestMethod]
        public void FileAttribute_Directory_Is16()
        {
            Assert.AreEqual(16, (int)FileAttribute.Directory);
        }

        [TestMethod]
        public void FileAttribute_Archive_Is32()
        {
            Assert.AreEqual(32, (int)FileAttribute.Archive);
        }

        // ──────────────────────────────────────────────
        // ファイル名チェック
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Dir_FileName_ContainsExpectedName()
        {
            var file = Path.Combine(_tempDir, "myfile.txt");
            File.WriteAllText(file, "hello");
            var result = FileSystem.Dir(file);
            Assert.IsTrue(result.Contains("myfile"), $"Expected 'myfile' in '{result}'");
        }

        [TestMethod]
        public void Dir_DirectoryName_ReturnsNonEmptyForExistingDir()
        {
            var subDir = Path.Combine(_tempDir, "subdir1");
            Directory.CreateDirectory(subDir);
            var result = FileSystem.Dir(subDir, FileAttribute.Directory);
            Assert.AreNotEqual("", result);
        }

        // ──────────────────────────────────────────────
        // FileAttribute ビット演算テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FileAttribute_Directory_OR_Archive_CanBeCombined()
        {
            var combined = FileAttribute.Directory | FileAttribute.Archive;
            Assert.IsTrue(combined.HasFlag(FileAttribute.Directory));
            Assert.IsTrue(combined.HasFlag(FileAttribute.Archive));
        }

        [TestMethod]
        public void FileAttribute_Normal_NotEqualTo_Archive()
        {
            Assert.AreNotEqual(FileAttribute.Normal, FileAttribute.Archive);
        }
    }
}
