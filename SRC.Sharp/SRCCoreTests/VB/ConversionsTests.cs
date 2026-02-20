using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    [TestClass()]
    public class ConversionsTests
    {
        // ──────────────────────────────────────────────
        // ToString
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void ToStringTest_Null()
        {
            Assert.AreEqual("", Conversions.ToString(null));
        }

        [TestMethod()]
        public void ToStringTest_Int()
        {
            Assert.AreEqual("42", Conversions.ToString(42));
        }

        [TestMethod()]
        public void ToStringTest_Double()
        {
            Assert.AreEqual("3.14", Conversions.ToString(3.14));
        }

        [TestMethod()]
        public void ToStringTest_String()
        {
            Assert.AreEqual("hello", Conversions.ToString("hello"));
        }

        // ──────────────────────────────────────────────
        // ToInteger
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void ToIntegerTest_ValidInt()
        {
            Assert.AreEqual(42, Conversions.ToInteger("42"));
            Assert.AreEqual(-10, Conversions.ToInteger("-10"));
            Assert.AreEqual(0, Conversions.ToInteger("0"));
        }

        [TestMethod()]
        public void ToIntegerTest_Double_Truncates()
        {
            Assert.AreEqual(3, Conversions.ToInteger("3.7"));
            Assert.AreEqual(3, Conversions.ToInteger("3.14"));
        }

        [TestMethod()]
        public void ToIntegerTest_NonNumeric_ReturnsZero()
        {
            Assert.AreEqual(0, Conversions.ToInteger("abc"));
            Assert.AreEqual(0, Conversions.ToInteger(""));
        }

        // ──────────────────────────────────────────────
        // ToDouble
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void ToDoubleTest_ValidDouble()
        {
            Assert.AreEqual(3.14, Conversions.ToDouble("3.14"));
            Assert.AreEqual(42d, Conversions.ToDouble("42"));
            Assert.AreEqual(-1.5, Conversions.ToDouble("-1.5"));
        }

        [TestMethod()]
        public void ToDoubleTest_NonNumeric_ReturnsZero()
        {
            Assert.AreEqual(0d, Conversions.ToDouble("abc"));
            Assert.AreEqual(0d, Conversions.ToDouble(""));
        }
    }
}
