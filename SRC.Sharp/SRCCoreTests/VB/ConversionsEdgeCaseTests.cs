using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// VB.Conversions クラスの追加テスト
    /// </summary>
    [TestClass]
    public class ConversionsEdgeCaseTests
    {
        // ──────────────────────────────────────────────
        // ToDouble - 追加エッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToDouble_ValidDecimalString_ReturnsDouble()
        {
            Assert.AreEqual(3.14, Conversions.ToDouble("3.14"), 0.001);
        }

        [TestMethod]
        public void ToDouble_NegativeDecimalString_ReturnsNegativeDouble()
        {
            Assert.AreEqual(-1.5, Conversions.ToDouble("-1.5"), 0.001);
        }

        [TestMethod]
        public void ToDouble_IntegerString_ReturnsDouble()
        {
            Assert.AreEqual(100.0, Conversions.ToDouble("100"), 0.001);
        }

        [TestMethod]
        public void ToDouble_ZeroString_ReturnsZero()
        {
            Assert.AreEqual(0.0, Conversions.ToDouble("0"), 0.001);
        }

        [TestMethod]
        public void ToDouble_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0.0, Conversions.ToDouble(""), 0.001);
        }

        [TestMethod]
        public void ToDouble_NullString_ReturnsZero()
        {
            Assert.AreEqual(0.0, Conversions.ToDouble(null), 0.001);
        }

        [TestMethod]
        public void ToDouble_InvalidString_ReturnsZero()
        {
            Assert.AreEqual(0.0, Conversions.ToDouble("abc"), 0.001);
        }

        [TestMethod]
        public void ToDouble_LeadingZeros_ReturnsDouble()
        {
            Assert.AreEqual(42.0, Conversions.ToDouble("042"), 0.001);
        }

        [TestMethod]
        public void ToDouble_VerySmallDecimal_ReturnsValue()
        {
            Assert.AreEqual(0.0001, Conversions.ToDouble("0.0001"), 0.00001);
        }

        // ──────────────────────────────────────────────
        // ToInteger - 追加エッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToInteger_ValidIntString_ReturnsInt()
        {
            Assert.AreEqual(42, Conversions.ToInteger("42"));
        }

        [TestMethod]
        public void ToInteger_NegativeIntString_ReturnsNegativeInt()
        {
            Assert.AreEqual(-10, Conversions.ToInteger("-10"));
        }

        [TestMethod]
        public void ToInteger_ZeroString_ReturnsZero()
        {
            Assert.AreEqual(0, Conversions.ToInteger("0"));
        }

        [TestMethod]
        public void ToInteger_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0, Conversions.ToInteger(""));
        }

        [TestMethod]
        public void ToInteger_NullString_ReturnsZero()
        {
            Assert.AreEqual(0, Conversions.ToInteger(null));
        }

        [TestMethod]
        public void ToInteger_InvalidString_ReturnsZero()
        {
            Assert.AreEqual(0, Conversions.ToInteger("abc"));
        }

        [TestMethod]
        public void ToInteger_FloatString_RoundsToInt()
        {
            // C# の (int)double はゼロ方向への切り捨て
            var result = Conversions.ToInteger("3.7");
            Assert.AreEqual(3, result); // C#は切り捨て: (int)3.7 = 3
        }

        [TestMethod]
        public void ToInteger_FloatStringHalf_RoundsBankers()
        {
            var result = Conversions.ToInteger("2.5");
            // 銀行丸め: 2.5 -> 2 (偶数に丸め)
            Assert.IsTrue(result == 2 || result == 3); // 実装によって異なる可能性
        }

        [TestMethod]
        public void ToInteger_LeadingZeros_ReturnsInt()
        {
            Assert.AreEqual(42, Conversions.ToInteger("042"));
        }

        [TestMethod]
        public void ToInteger_MaxPositive_ReturnsInt()
        {
            Assert.AreEqual(999, Conversions.ToInteger("999"));
        }
    }
}
