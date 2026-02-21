using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// FileAttribute enum のユニットテスト
    /// </summary>
    [TestClass]
    public class FileAttributeEnumTests
    {
        // ──────────────────────────────────────────────
        // 各値の数値確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Normal_IsZero()
        {
            Assert.AreEqual(0, (int)FileAttribute.Normal);
        }

        [TestMethod]
        public void ReadOnly_IsOne()
        {
            Assert.AreEqual(1, (int)FileAttribute.ReadOnly);
        }

        [TestMethod]
        public void Hidden_IsTwo()
        {
            Assert.AreEqual(2, (int)FileAttribute.Hidden);
        }

        [TestMethod]
        public void System_IsFour()
        {
            Assert.AreEqual(4, (int)FileAttribute.System);
        }

        [TestMethod]
        public void Volume_IsEight()
        {
            Assert.AreEqual(8, (int)FileAttribute.Volume);
        }

        [TestMethod]
        public void Directory_IsSixteen()
        {
            Assert.AreEqual(16, (int)FileAttribute.Directory);
        }

        [TestMethod]
        public void Archive_IsThirtyTwo()
        {
            Assert.AreEqual(32, (int)FileAttribute.Archive);
        }

        // ──────────────────────────────────────────────
        // Flags 属性テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HasFlag_ReadOnly_OnReadOnly()
        {
            Assert.IsTrue(FileAttribute.ReadOnly.HasFlag(FileAttribute.ReadOnly));
        }

        [TestMethod]
        public void HasFlag_Directory_OnDirectory()
        {
            Assert.IsTrue(FileAttribute.Directory.HasFlag(FileAttribute.Directory));
        }

        [TestMethod]
        public void HasFlag_Normal_DoesNotHaveReadOnly()
        {
            Assert.IsFalse(FileAttribute.Normal.HasFlag(FileAttribute.ReadOnly));
        }

        [TestMethod]
        public void Combined_ReadOnlyAndHidden_HasBothFlags()
        {
            var combined = FileAttribute.ReadOnly | FileAttribute.Hidden;
            Assert.IsTrue(combined.HasFlag(FileAttribute.ReadOnly));
            Assert.IsTrue(combined.HasFlag(FileAttribute.Hidden));
            Assert.IsFalse(combined.HasFlag(FileAttribute.Directory));
        }

        [TestMethod]
        public void Combined_AllAttributes_HasAllFlags()
        {
            var all = FileAttribute.ReadOnly | FileAttribute.Hidden | FileAttribute.System
                      | FileAttribute.Volume | FileAttribute.Directory | FileAttribute.Archive;
            Assert.IsTrue(all.HasFlag(FileAttribute.ReadOnly));
            Assert.IsTrue(all.HasFlag(FileAttribute.Hidden));
            Assert.IsTrue(all.HasFlag(FileAttribute.System));
            Assert.IsTrue(all.HasFlag(FileAttribute.Volume));
            Assert.IsTrue(all.HasFlag(FileAttribute.Directory));
            Assert.IsTrue(all.HasFlag(FileAttribute.Archive));
        }

        // ──────────────────────────────────────────────
        // Enum 基本操作
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsDefined_ForAllValues()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(FileAttribute), FileAttribute.Normal));
            Assert.IsTrue(System.Enum.IsDefined(typeof(FileAttribute), FileAttribute.ReadOnly));
            Assert.IsTrue(System.Enum.IsDefined(typeof(FileAttribute), FileAttribute.Hidden));
            Assert.IsTrue(System.Enum.IsDefined(typeof(FileAttribute), FileAttribute.System));
            Assert.IsTrue(System.Enum.IsDefined(typeof(FileAttribute), FileAttribute.Volume));
            Assert.IsTrue(System.Enum.IsDefined(typeof(FileAttribute), FileAttribute.Directory));
            Assert.IsTrue(System.Enum.IsDefined(typeof(FileAttribute), FileAttribute.Archive));
        }

        [TestMethod]
        public void CanBeParsedFromString_Normal()
        {
            Assert.AreEqual(FileAttribute.Normal, System.Enum.Parse<FileAttribute>("Normal"));
        }

        [TestMethod]
        public void CanBeParsedFromString_Directory()
        {
            Assert.AreEqual(FileAttribute.Directory, System.Enum.Parse<FileAttribute>("Directory"));
        }
    }
}
