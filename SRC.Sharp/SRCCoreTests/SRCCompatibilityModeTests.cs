using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;

namespace SRCCore.Tests
{
    /// <summary>
    /// SRCCompatibilityMode enum のユニットテスト
    /// </summary>
    [TestClass]
    public class SRCCompatibilityModeTests
    {
        // ──────────────────────────────────────────────
        // 各値の数値確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void None_IsZero()
        {
            Assert.AreEqual(0, (int)SRCCompatibilityMode.None);
        }

        [TestMethod]
        public void Read_IsOne()
        {
            Assert.AreEqual(1, (int)SRCCompatibilityMode.Read);
        }

        [TestMethod]
        public void Write_IsTwo()
        {
            Assert.AreEqual(2, (int)SRCCompatibilityMode.Write);
        }

        [TestMethod]
        public void ReadWrite_IsThree()
        {
            Assert.AreEqual(3, (int)SRCCompatibilityMode.ReadWrite);
        }

        // ──────────────────────────────────────────────
        // HasFlag テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReadWrite_HasFlag_Read()
        {
            Assert.IsTrue(SRCCompatibilityMode.ReadWrite.HasFlag(SRCCompatibilityMode.Read));
        }

        [TestMethod]
        public void ReadWrite_HasFlag_Write()
        {
            Assert.IsTrue(SRCCompatibilityMode.ReadWrite.HasFlag(SRCCompatibilityMode.Write));
        }

        [TestMethod]
        public void Read_HasFlag_Read()
        {
            Assert.IsTrue(SRCCompatibilityMode.Read.HasFlag(SRCCompatibilityMode.Read));
        }

        [TestMethod]
        public void Read_DoesNotHaveFlag_Write()
        {
            Assert.IsFalse(SRCCompatibilityMode.Read.HasFlag(SRCCompatibilityMode.Write));
        }

        [TestMethod]
        public void Write_HasFlag_Write()
        {
            Assert.IsTrue(SRCCompatibilityMode.Write.HasFlag(SRCCompatibilityMode.Write));
        }

        [TestMethod]
        public void Write_DoesNotHaveFlag_Read()
        {
            Assert.IsFalse(SRCCompatibilityMode.Write.HasFlag(SRCCompatibilityMode.Read));
        }

        [TestMethod]
        public void None_DoesNotHaveFlag_Read()
        {
            Assert.IsFalse(SRCCompatibilityMode.None.HasFlag(SRCCompatibilityMode.Read));
        }

        [TestMethod]
        public void None_DoesNotHaveFlag_Write()
        {
            Assert.IsFalse(SRCCompatibilityMode.None.HasFlag(SRCCompatibilityMode.Write));
        }

        // ──────────────────────────────────────────────
        // Enum 基本操作
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AllValuesAreDistinct()
        {
            var values = System.Enum.GetValues(typeof(SRCCompatibilityMode));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (SRCCompatibilityMode v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複した値: {v} = {(int)v}");
            }
        }

        [TestMethod]
        public void HasFourValues()
        {
            Assert.AreEqual(4, System.Enum.GetValues(typeof(SRCCompatibilityMode)).Length);
        }

        [TestMethod]
        public void CanBeParsedFromString()
        {
            Assert.AreEqual(SRCCompatibilityMode.None, System.Enum.Parse<SRCCompatibilityMode>("None"));
            Assert.AreEqual(SRCCompatibilityMode.Read, System.Enum.Parse<SRCCompatibilityMode>("Read"));
            Assert.AreEqual(SRCCompatibilityMode.Write, System.Enum.Parse<SRCCompatibilityMode>("Write"));
            Assert.AreEqual(SRCCompatibilityMode.ReadWrite, System.Enum.Parse<SRCCompatibilityMode>("ReadWrite"));
        }

        [TestMethod]
        public void IsDefined_ForAllValues()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(SRCCompatibilityMode), SRCCompatibilityMode.None));
            Assert.IsTrue(System.Enum.IsDefined(typeof(SRCCompatibilityMode), SRCCompatibilityMode.Read));
            Assert.IsTrue(System.Enum.IsDefined(typeof(SRCCompatibilityMode), SRCCompatibilityMode.Write));
            Assert.IsTrue(System.Enum.IsDefined(typeof(SRCCompatibilityMode), SRCCompatibilityMode.ReadWrite));
        }

        // ──────────────────────────────────────────────
        // ビット演算確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Read_OR_Write_EqualsReadWrite()
        {
            var combined = SRCCompatibilityMode.Read | SRCCompatibilityMode.Write;
            Assert.AreEqual(SRCCompatibilityMode.ReadWrite, combined);
        }

        [TestMethod]
        public void ReadWrite_AND_Read_EqualsRead()
        {
            var masked = SRCCompatibilityMode.ReadWrite & SRCCompatibilityMode.Read;
            Assert.AreEqual(SRCCompatibilityMode.Read, masked);
        }

        [TestMethod]
        public void ReadWrite_AND_Write_EqualsWrite()
        {
            var masked = SRCCompatibilityMode.ReadWrite & SRCCompatibilityMode.Write;
            Assert.AreEqual(SRCCompatibilityMode.Write, masked);
        }
    }
}
