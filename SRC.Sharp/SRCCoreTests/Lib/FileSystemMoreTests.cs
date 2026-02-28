using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using System;
using System.IO;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// FileSystem および FileAttribute の追加テスト
    /// </summary>
    [TestClass]
    public class FileSystemMoreTests
    {
        private string _tempDir;

        [TestInitialize]
        public void Setup()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "SRCCoreTests_More_" + Guid.NewGuid().ToString("N"));
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
        // FileAttribute 定数値の検証
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FileAttribute_ReadOnly_HasExpectedValue()
        {
            Assert.AreEqual(1, (int)FileAttribute.ReadOnly);
        }

        [TestMethod]
        public void FileAttribute_Hidden_HasExpectedValue()
        {
            Assert.AreEqual(2, (int)FileAttribute.Hidden);
        }

        [TestMethod]
        public void FileAttribute_System_HasExpectedValue()
        {
            Assert.AreEqual(4, (int)FileAttribute.System);
        }

        [TestMethod]
        public void FileAttribute_Archive_HasExpectedValue()
        {
            Assert.AreEqual(32, (int)FileAttribute.Archive);
        }

        // ──────────────────────────────────────────────
        // FileAttribute フラグ組み合わせ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FileAttribute_Flags_CanBeCombined()
        {
            var combined = FileAttribute.ReadOnly | FileAttribute.Archive;
            Assert.IsTrue(combined.HasFlag(FileAttribute.ReadOnly));
            Assert.IsTrue(combined.HasFlag(FileAttribute.Archive));
            Assert.IsFalse(combined.HasFlag(FileAttribute.Hidden));
        }

        [TestMethod]
        public void FileAttribute_NormalIsZero_HasNoFlags()
        {
            var attr = FileAttribute.Normal;
            Assert.IsFalse(attr.HasFlag(FileAttribute.ReadOnly));
            Assert.IsFalse(attr.HasFlag(FileAttribute.Hidden));
            Assert.IsFalse(attr.HasFlag(FileAttribute.System));
            Assert.IsFalse(attr.HasFlag(FileAttribute.Volume));
            Assert.IsFalse(attr.HasFlag(FileAttribute.Directory));
            Assert.IsFalse(attr.HasFlag(FileAttribute.Archive));
        }

        [TestMethod]
        public void FileAttribute_AllNonVolumeFlags_DoNotThrowWhenCombined()
        {
            // Volume フラグが含まれなければ組み合わせても例外が発生しない
            var filePath = Path.Combine(_tempDir, "test.txt");
            File.WriteAllText(filePath, "data");

            var attr = FileAttribute.ReadOnly | FileAttribute.Hidden | FileAttribute.System | FileAttribute.Archive | FileAttribute.Directory;
            // Directory フラグが含まれるためディレクトリチェックも行われる
            var result = FileSystem.Dir(filePath, attr);
            Assert.AreNotEqual("", result);
        }

        // ──────────────────────────────────────────────
        // Dir – ReadOnly 属性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Dir_WithReadOnlyAttribute_FileExists_ReturnsNonEmpty()
        {
            var filePath = Path.Combine(_tempDir, "readonly.txt");
            File.WriteAllText(filePath, "data");

            var result = FileSystem.Dir(filePath, FileAttribute.ReadOnly);
            Assert.AreNotEqual("", result);
        }

        [TestMethod]
        public void Dir_WithReadOnlyAttribute_FileMissing_ReturnsEmpty()
        {
            var result = FileSystem.Dir(Path.Combine(_tempDir, "missing.txt"), FileAttribute.ReadOnly);
            Assert.AreEqual("", result);
        }

        // ──────────────────────────────────────────────
        // Dir – Hidden 属性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Dir_WithHiddenAttribute_FileExists_ReturnsNonEmpty()
        {
            var filePath = Path.Combine(_tempDir, "hidden.txt");
            File.WriteAllText(filePath, "data");

            var result = FileSystem.Dir(filePath, FileAttribute.Hidden);
            Assert.AreNotEqual("", result);
        }

        // ──────────────────────────────────────────────
        // Dir – System 属性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Dir_WithSystemAttribute_FileExists_ReturnsNonEmpty()
        {
            var filePath = Path.Combine(_tempDir, "system.txt");
            File.WriteAllText(filePath, "data");

            var result = FileSystem.Dir(filePath, FileAttribute.System);
            Assert.AreNotEqual("", result);
        }

        // ──────────────────────────────────────────────
        // Dir – Volume フラグ含む組み合わせは常に例外
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Dir_VolumeWithReadOnly_ThrowsNotSupported()
        {
            Assert.Throws<NotSupportedException>(() =>
                FileSystem.Dir(_tempDir, FileAttribute.Volume | FileAttribute.ReadOnly));
        }

        [TestMethod]
        public void Dir_VolumeWithHidden_ThrowsNotSupported()
        {
            Assert.Throws<NotSupportedException>(() =>
                FileSystem.Dir(_tempDir, FileAttribute.Volume | FileAttribute.Hidden));
        }

        // ──────────────────────────────────────────────
        // Dir – Directory フラグで既存ディレクトリの確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Dir_DirectoryFlag_SubDirectory_ReturnsNonEmpty()
        {
            var subDir = Path.Combine(_tempDir, "subdir");
            Directory.CreateDirectory(subDir);

            var result = FileSystem.Dir(subDir, FileAttribute.Directory);
            Assert.AreNotEqual("", result);
        }

        [TestMethod]
        public void Dir_DirectoryFlag_NonExistentDirectory_ReturnsEmpty()
        {
            var nonExistent = Path.Combine(_tempDir, "no_such_dir");
            var result = FileSystem.Dir(nonExistent, FileAttribute.Directory);
            Assert.AreEqual("", result);
        }
    }
}
