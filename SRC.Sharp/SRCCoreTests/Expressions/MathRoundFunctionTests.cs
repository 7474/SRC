using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression 経由で呼び出す Round/RoundUp/RoundDown/Int/Abs の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class MathRoundFunctionTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // Round
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Round_NearestInt_RoundsCorrectly()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("Round(2.7,0)"));
        }

        [TestMethod]
        public void Round_OneDecimal_RoundsToOneDecimal()
        {
            var exp = Create();
            Assert.AreEqual(2.8, exp.GetValueAsDouble("Round(2.75,1)"), 1e-10);
        }

        [TestMethod]
        public void Round_Zero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Round(0,0)"), 1e-10);
        }

        [TestMethod]
        public void Round_NegativeNumber_RoundsCorrectly()
        {
            var exp = Create();
            var result = exp.GetValueAsDouble("Round(-2.5,0)");
            // -2.5 → -2 or -3 depending on banker's rounding
            Assert.IsTrue(result == -2d || result == -3d);
        }

        // ──────────────────────────────────────────────
        // RoundUp (切り上げ)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RoundUp_PositiveNumber_CeilsUp()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("RoundUp(2.1,0)"));
        }

        [TestMethod]
        public void RoundUp_ExactValue_ReturnsSame()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("RoundUp(3.0,0)"));
        }

        [TestMethod]
        public void RoundUp_OneDecimal_CeilsToOneDecimal()
        {
            var exp = Create();
            Assert.AreEqual(2.2d, exp.GetValueAsDouble("RoundUp(2.11,1)"), 1e-10);
        }

        [TestMethod]
        public void RoundUp_Zero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("RoundUp(0,0)"));
        }

        // ──────────────────────────────────────────────
        // RoundDown (切り捨て)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RoundDown_PositiveNumber_FloorsDown()
        {
            var exp = Create();
            Assert.AreEqual(2d, exp.GetValueAsDouble("RoundDown(2.9,0)"));
        }

        [TestMethod]
        public void RoundDown_ExactValue_ReturnsSame()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("RoundDown(3.0,0)"));
        }

        [TestMethod]
        public void RoundDown_OneDecimal_FloorsToOneDecimal()
        {
            var exp = Create();
            Assert.AreEqual(2.1d, exp.GetValueAsDouble("RoundDown(2.19,1)"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // Int
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Int_PositiveDecimal_ReturnsFloor()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("Int(3.9)"));
        }

        [TestMethod]
        public void Int_NegativeDecimal_ReturnsFloor()
        {
            var exp = Create();
            // Int(-3.1) = -4 (VBのIntは切り捨て)
            Assert.AreEqual(-4d, exp.GetValueAsDouble("Int(-3.1)"));
        }

        [TestMethod]
        public void Int_Zero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Int(0)"));
        }

        [TestMethod]
        public void Int_ExactInteger_ReturnsSame()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("Int(5)"));
        }

        // ──────────────────────────────────────────────
        // Abs (絶対値)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Abs_PositiveNumber_ReturnsSame()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("Abs(5)"));
        }

        [TestMethod]
        public void Abs_NegativeNumber_ReturnsPositive()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("Abs(-5)"));
        }

        [TestMethod]
        public void Abs_Zero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Abs(0)"));
        }

        [TestMethod]
        public void Abs_NegativeDecimal_ReturnsPositiveDecimal()
        {
            var exp = Create();
            Assert.AreEqual(3.14, exp.GetValueAsDouble("Abs(-3.14)"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // Sqr (平方根)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Sqr_Four_ReturnsTwo()
        {
            var exp = Create();
            Assert.AreEqual(2d, exp.GetValueAsDouble("Sqr(4)"), 1e-10);
        }

        [TestMethod]
        public void Sqr_Nine_ReturnsThree()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("Sqr(9)"), 1e-10);
        }

        [TestMethod]
        public void Sqr_100_ReturnsTen()
        {
            var exp = Create();
            Assert.AreEqual(10d, exp.GetValueAsDouble("Sqr(100)"), 1e-10);
        }
    }
}
