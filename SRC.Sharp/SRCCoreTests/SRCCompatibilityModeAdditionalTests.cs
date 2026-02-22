using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using System;

namespace SRCCore.Tests
{
    /// <summary>
    /// SRCCompatibilityMode enum の追加テスト
    /// 基本テストは SRCCompatibilityModeTests にて実施済み
    /// </summary>
    [TestClass]
    public class SRCCompatibilityModeAdditionalTests
    {
        // ──────────────────────────────────────────────
        // フラグ組み合わせ: XOR
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReadWrite_XOR_Read_EqualsWrite()
        {
            var result = SRCCompatibilityMode.ReadWrite ^ SRCCompatibilityMode.Read;
            Assert.AreEqual(SRCCompatibilityMode.Write, result);
        }

        [TestMethod]
        public void ReadWrite_XOR_Write_EqualsRead()
        {
            var result = SRCCompatibilityMode.ReadWrite ^ SRCCompatibilityMode.Write;
            Assert.AreEqual(SRCCompatibilityMode.Read, result);
        }

        // ──────────────────────────────────────────────
        // フラグ組み合わせ: NOT マスク
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReadWrite_RemoveRead_EqualsWrite()
        {
            var result = SRCCompatibilityMode.ReadWrite & ~SRCCompatibilityMode.Read;
            Assert.AreEqual(SRCCompatibilityMode.Write, result);
        }

        [TestMethod]
        public void ReadWrite_RemoveWrite_EqualsRead()
        {
            var result = SRCCompatibilityMode.ReadWrite & ~SRCCompatibilityMode.Write;
            Assert.AreEqual(SRCCompatibilityMode.Read, result);
        }

        // ──────────────────────────────────────────────
        // TryParse
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TryParse_ValidName_ReturnsTrue()
        {
            Assert.IsTrue(Enum.TryParse<SRCCompatibilityMode>("ReadWrite", out var result));
            Assert.AreEqual(SRCCompatibilityMode.ReadWrite, result);
        }

        [TestMethod]
        public void TryParse_InvalidName_ReturnsFalse()
        {
            Assert.IsFalse(Enum.TryParse<SRCCompatibilityMode>("Invalid", out _));
        }

        // ──────────────────────────────────────────────
        // int キャスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CastFromInt_ReturnsCorrectValue()
        {
            Assert.AreEqual(SRCCompatibilityMode.None, (SRCCompatibilityMode)0);
            Assert.AreEqual(SRCCompatibilityMode.Read, (SRCCompatibilityMode)1);
            Assert.AreEqual(SRCCompatibilityMode.Write, (SRCCompatibilityMode)2);
            Assert.AreEqual(SRCCompatibilityMode.ReadWrite, (SRCCompatibilityMode)3);
        }

        // ──────────────────────────────────────────────
        // ToString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_ReturnsEnumName()
        {
            Assert.AreEqual("None", SRCCompatibilityMode.None.ToString());
            Assert.AreEqual("Read", SRCCompatibilityMode.Read.ToString());
            Assert.AreEqual("Write", SRCCompatibilityMode.Write.ToString());
            Assert.AreEqual("ReadWrite", SRCCompatibilityMode.ReadWrite.ToString());
        }

        // ──────────────────────────────────────────────
        // デフォルト値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Default_IsNone()
        {
            Assert.AreEqual(SRCCompatibilityMode.None, default(SRCCompatibilityMode));
        }

        // ──────────────────────────────────────────────
        // None との OR は元の値を保持
        // ──────────────────────────────────────────────

        [TestMethod]
        public void None_OR_Read_EqualsRead()
        {
            Assert.AreEqual(SRCCompatibilityMode.Read, SRCCompatibilityMode.None | SRCCompatibilityMode.Read);
        }

        [TestMethod]
        public void None_OR_Write_EqualsWrite()
        {
            Assert.AreEqual(SRCCompatibilityMode.Write, SRCCompatibilityMode.None | SRCCompatibilityMode.Write);
        }
    }
}
