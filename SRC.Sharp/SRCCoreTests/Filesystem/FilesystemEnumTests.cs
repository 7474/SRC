using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Filesystem;

namespace SRCCore.Filesystem.Tests
{
    /// <summary>
    /// EntryOption / SafeOpenMode enum のユニットテスト
    /// </summary>
    [TestClass]
    public class FilesystemEnumTests
    {
        // ──────────────────────────────────────────────
        // EntryOption
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EntryOption_File_IsOne()
        {
            Assert.AreEqual(1, (int)EntryOption.File);
        }

        [TestMethod]
        public void EntryOption_Directory_IsTwo()
        {
            Assert.AreEqual(2, (int)EntryOption.Directory);
        }

        [TestMethod]
        public void EntryOption_All_IsThree()
        {
            Assert.AreEqual(3, (int)EntryOption.All);
        }

        [TestMethod]
        public void EntryOption_HasThreeValues()
        {
            Assert.AreEqual(3, System.Enum.GetValues(typeof(EntryOption)).Length);
        }

        [TestMethod]
        public void EntryOption_AllValues_AreDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(EntryOption), EntryOption.File));
            Assert.IsTrue(System.Enum.IsDefined(typeof(EntryOption), EntryOption.Directory));
            Assert.IsTrue(System.Enum.IsDefined(typeof(EntryOption), EntryOption.All));
        }

        [TestMethod]
        public void EntryOption_CanBeParsedFromString()
        {
            Assert.AreEqual(EntryOption.File, System.Enum.Parse<EntryOption>("File"));
            Assert.AreEqual(EntryOption.Directory, System.Enum.Parse<EntryOption>("Directory"));
            Assert.AreEqual(EntryOption.All, System.Enum.Parse<EntryOption>("All"));
        }

        // ──────────────────────────────────────────────
        // SafeOpenMode
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SafeOpenMode_Read_IsZero()
        {
            Assert.AreEqual(0, (int)SafeOpenMode.Read);
        }

        [TestMethod]
        public void SafeOpenMode_Write_IsOne()
        {
            Assert.AreEqual(1, (int)SafeOpenMode.Write);
        }

        [TestMethod]
        public void SafeOpenMode_Append_IsTwo()
        {
            Assert.AreEqual(2, (int)SafeOpenMode.Append);
        }

        [TestMethod]
        public void SafeOpenMode_HasThreeValues()
        {
            Assert.AreEqual(3, System.Enum.GetValues(typeof(SafeOpenMode)).Length);
        }

        [TestMethod]
        public void SafeOpenMode_AllValues_AreDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(SafeOpenMode), SafeOpenMode.Read));
            Assert.IsTrue(System.Enum.IsDefined(typeof(SafeOpenMode), SafeOpenMode.Write));
            Assert.IsTrue(System.Enum.IsDefined(typeof(SafeOpenMode), SafeOpenMode.Append));
        }

        [TestMethod]
        public void SafeOpenMode_CanBeParsedFromString()
        {
            Assert.AreEqual(SafeOpenMode.Read, System.Enum.Parse<SafeOpenMode>("Read"));
            Assert.AreEqual(SafeOpenMode.Write, System.Enum.Parse<SafeOpenMode>("Write"));
            Assert.AreEqual(SafeOpenMode.Append, System.Enum.Parse<SafeOpenMode>("Append"));
        }
    }
}
