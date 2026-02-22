using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;
using System;

namespace SRCCore.VB.Tests
{
    [TestClass]
    public class ConversionsMoreTests
    {
        // ──────────────────────────────────────────────
        // ToString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_Null_ReturnsEmpty()
        {
            Assert.AreEqual("", Conversions.ToString(null));
        }

        [TestMethod]
        public void ToString_Int_ReturnsStringRepresentation()
        {
            Assert.AreEqual("42", Conversions.ToString(42));
        }

        [TestMethod]
        public void ToString_Double_ReturnsStringRepresentation()
        {
            var result = Conversions.ToString(3.14);
            Assert.IsTrue(result.Contains("3.14") || result.Contains("3,14"));
        }

        // ──────────────────────────────────────────────
        // ToInteger
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToInteger_ValidString_ReturnsInt()
        {
            Assert.AreEqual(123, Conversions.ToInteger("123"));
        }

        [TestMethod]
        public void ToInteger_DecimalString_TruncatesToInt()
        {
            Assert.AreEqual(3, Conversions.ToInteger("3.7"));
        }

        [TestMethod]
        public void ToInteger_NonNumericString_ReturnsZero()
        {
            Assert.AreEqual(0, Conversions.ToInteger("abc"));
        }

        [TestMethod]
        public void ToInteger_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0, Conversions.ToInteger(""));
        }

        [TestMethod]
        public void ToInteger_NegativeString_ReturnsNegative()
        {
            Assert.AreEqual(-5, Conversions.ToInteger("-5"));
        }

        // ──────────────────────────────────────────────
        // ToDouble
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToDouble_ValidString_ReturnsDouble()
        {
            Assert.AreEqual(1.5, Conversions.ToDouble("1.5"), 0.0001);
        }

        [TestMethod]
        public void ToDouble_NonNumericString_ReturnsZero()
        {
            Assert.AreEqual(0.0, Conversions.ToDouble("abc"));
        }

        [TestMethod]
        public void ToDouble_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0.0, Conversions.ToDouble(""));
        }

        [TestMethod]
        public void ToDouble_IntegerString_ReturnsDouble()
        {
            Assert.AreEqual(42.0, Conversions.ToDouble("42"));
        }

        // ──────────────────────────────────────────────
        // GetNow
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetNow_ReturnsCurrentTime()
        {
            var before = DateTime.Now;
            var result = Conversions.GetNow();
            var after = DateTime.Now;
            Assert.IsTrue(result >= before && result <= after);
        }

        // ──────────────────────────────────────────────
        // TryToDateTime
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TryToDateTime_ValidDateString_ReturnsTrueAndDate()
        {
            var result = Conversions.TryToDateTime("2023-01-15", out DateTime dt);
            Assert.IsTrue(result);
            Assert.AreEqual(2023, dt.Year);
            Assert.AreEqual(1, dt.Month);
            Assert.AreEqual(15, dt.Day);
        }

        [TestMethod]
        public void TryToDateTime_InvalidDateString_ReturnsFalse()
        {
            var result = Conversions.TryToDateTime("not-a-date", out DateTime dt);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TryToDateTime_EmptyString_ReturnsFalse()
        {
            var result = Conversions.TryToDateTime("", out DateTime dt);
            Assert.IsFalse(result);
        }
    }
}
