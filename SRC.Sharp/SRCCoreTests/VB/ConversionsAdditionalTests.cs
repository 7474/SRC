using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;
using System;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// Conversions クラスの追加ユニットテスト
    /// </summary>
    [TestClass]
    public class ConversionsAdditionalTests
    {
        // ──────────────────────────────────────────────
        // ToString の追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_Bool_ReturnsString()
        {
            Assert.AreEqual("True", Conversions.ToString(true));
            Assert.AreEqual("False", Conversions.ToString(false));
        }

        [TestMethod]
        public void ToString_Float_ReturnsString()
        {
            var result = Conversions.ToString(1.5f);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
        }

        [TestMethod]
        public void ToString_LargeInteger_ReturnsString()
        {
            Assert.AreEqual("1000000", Conversions.ToString(1000000));
        }

        [TestMethod]
        public void ToString_NegativeDouble_ReturnsString()
        {
            Assert.AreEqual("-3.14", Conversions.ToString(-3.14));
        }

        [TestMethod]
        public void ToString_ZeroDouble_ReturnsZeroString()
        {
            Assert.AreEqual("0", Conversions.ToString(0.0));
        }

        // ──────────────────────────────────────────────
        // ToInteger の追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToInteger_LeadingSpaces_ParsesCorrectly()
        {
            // 先頭の空白はTryParseが無視する
            var result = Conversions.ToInteger("  42");
            Assert.AreEqual(42, result);
        }

        [TestMethod]
        public void ToInteger_TrailingSpaces_ParsesCorrectly()
        {
            var result = Conversions.ToInteger("42  ");
            Assert.AreEqual(42, result);
        }

        [TestMethod]
        public void ToInteger_MaxInt_ReturnsMaxInt()
        {
            var result = Conversions.ToInteger("2147483647");
            Assert.AreEqual(int.MaxValue, result);
        }

        [TestMethod]
        public void ToInteger_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0, Conversions.ToInteger(""));
        }

        [TestMethod]
        public void ToInteger_NegativeFloat_TruncatesTowardZero()
        {
            // -3.7 → (int)-3.7 = -3
            var result = Conversions.ToInteger("-3.7");
            Assert.AreEqual(-3, result);
        }

        // ──────────────────────────────────────────────
        // ToDouble の追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToDouble_ValidPositiveFloat_ReturnsDouble()
        {
            Assert.AreEqual(3.14, Conversions.ToDouble("3.14"), 1e-10);
        }

        [TestMethod]
        public void ToDouble_NegativeFloat_ReturnsDouble()
        {
            Assert.AreEqual(-2.5, Conversions.ToDouble("-2.5"), 1e-10);
        }

        [TestMethod]
        public void ToDouble_Zero_ReturnsZero()
        {
            Assert.AreEqual(0d, Conversions.ToDouble("0"));
        }

        [TestMethod]
        public void ToDouble_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0d, Conversions.ToDouble(""));
        }

        [TestMethod]
        public void ToDouble_NonNumericString_ReturnsZero()
        {
            Assert.AreEqual(0d, Conversions.ToDouble("hello"));
        }

        [TestMethod]
        public void ToDouble_LargeNumber_ReturnsDouble()
        {
            Assert.AreEqual(1e15, Conversions.ToDouble("1000000000000000"), 1e5);
        }

        // ──────────────────────────────────────────────
        // TryToDateTime の追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TryToDateTime_ValidDate_ReturnsTrueAndDateTime()
        {
            var result = Conversions.TryToDateTime("2024/01/15", out var dt);
            Assert.IsTrue(result);
            Assert.AreEqual(2024, dt.Year);
            Assert.AreEqual(1, dt.Month);
            Assert.AreEqual(15, dt.Day);
        }

        [TestMethod]
        public void TryToDateTime_ValidDateTime_ReturnsTrueAndDateTime()
        {
            var result = Conversions.TryToDateTime("2024/06/30 12:30:00", out var dt);
            Assert.IsTrue(result);
            Assert.AreEqual(2024, dt.Year);
            Assert.AreEqual(6, dt.Month);
            Assert.AreEqual(30, dt.Day);
        }

        [TestMethod]
        public void TryToDateTime_InvalidString_ReturnsFalse()
        {
            var result = Conversions.TryToDateTime("not a date", out var dt);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TryToDateTime_EmptyString_ReturnsFalse()
        {
            var result = Conversions.TryToDateTime("", out var dt);
            Assert.IsFalse(result);
        }

        // ──────────────────────────────────────────────
        // GetNow のテスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetNow_ReturnsCurrentDateTime()
        {
            var before = DateTime.Now.AddSeconds(-1);
            var now = Conversions.GetNow();
            var after = DateTime.Now.AddSeconds(1);

            Assert.IsTrue(now >= before && now <= after);
        }
    }
}
