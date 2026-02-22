using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression の Max/Min/Clamp/Between 関数の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class MaxMinFunctionFurtherTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // Max 関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Max_LargerFirst_ReturnsFirst()
        {
            var exp = Create();
            Assert.AreEqual(10d, exp.GetValueAsDouble("Max(10,5)"));
        }

        [TestMethod]
        public void Max_LargerSecond_ReturnsSecond()
        {
            var exp = Create();
            Assert.AreEqual(10d, exp.GetValueAsDouble("Max(5,10)"));
        }

        [TestMethod]
        public void Max_EqualValues_ReturnsThatValue()
        {
            var exp = Create();
            Assert.AreEqual(7d, exp.GetValueAsDouble("Max(7,7)"));
        }

        [TestMethod]
        public void Max_NegativeValues_ReturnsLessNegative()
        {
            var exp = Create();
            Assert.AreEqual(-1d, exp.GetValueAsDouble("Max(-5,-1)"));
        }

        [TestMethod]
        public void Max_ZeroAndNegative_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Max(-3,0)"));
        }

        [TestMethod]
        public void Max_Decimals_ReturnsLarger()
        {
            var exp = Create();
            Assert.AreEqual(3.5, exp.GetValueAsDouble("Max(3.5,2.1)"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // Min 関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Min_SmallerFirst_ReturnsFirst()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("Min(5,10)"));
        }

        [TestMethod]
        public void Min_SmallerSecond_ReturnsSecond()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("Min(10,5)"));
        }

        [TestMethod]
        public void Min_EqualValues_ReturnsThatValue()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("Min(3,3)"));
        }

        [TestMethod]
        public void Min_NegativeValues_ReturnsMostNegative()
        {
            var exp = Create();
            Assert.AreEqual(-5d, exp.GetValueAsDouble("Min(-5,-1)"));
        }

        [TestMethod]
        public void Min_ZeroAndPositive_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Min(0,5)"));
        }

        [TestMethod]
        public void Min_Decimals_ReturnsSmaller()
        {
            var exp = Create();
            Assert.AreEqual(1.5, exp.GetValueAsDouble("Min(1.5,3.5)"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // MaxとMinの組み合わせ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MaxOfMin_ComputesCorrectly()
        {
            var exp = Create();
            // Max(Min(5,10), 3) = Max(5,3) = 5
            Assert.AreEqual(5d, exp.GetValueAsDouble("Max(Min(5,10),3)"));
        }

        [TestMethod]
        public void MinOfMax_ComputesCorrectly()
        {
            var exp = Create();
            // Min(Max(5,3), 10) = Min(5,10) = 5
            Assert.AreEqual(5d, exp.GetValueAsDouble("Min(Max(5,3),10)"));
        }
    }
}
