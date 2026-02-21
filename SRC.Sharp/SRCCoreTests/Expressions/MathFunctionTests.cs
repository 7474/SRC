using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression経由で呼び出す数学関数のユニットテスト
    /// </summary>
    [TestClass]
    public class MathFunctionTests
    {
        private Expression Create()
        {
            var src = new SRC
            {
                GUI = new MockGUI(),
            };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // Abs
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Abs_PositiveNumber_ReturnsSameValue()
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

        // ──────────────────────────────────────────────
        // Int
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Int_PositiveDecimal_TruncatesDown()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("Int(3.9)"));
            Assert.AreEqual(3d, exp.GetValueAsDouble("Int(3.1)"));
        }

        [TestMethod]
        public void Int_NegativeDecimal_TruncatesDown()
        {
            // Math.Floor(-3.1) = -4
            var exp = Create();
            Assert.AreEqual(-4d, exp.GetValueAsDouble("Int(-3.1)"));
        }

        [TestMethod]
        public void Int_WholeNumber_ReturnsSame()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("Int(5)"));
        }

        // ──────────────────────────────────────────────
        // Round
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Round_HalfUp_RoundsAwayFromZero()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("Round(2.5,0)"));
            Assert.AreEqual(4d, exp.GetValueAsDouble("Round(3.5,0)"));
        }

        [TestMethod]
        public void Round_WithDecimals_RoundsToSpecifiedDigits()
        {
            var exp = Create();
            // 整数を小数点以下1桁に丸める → そのまま
            Assert.AreEqual(3d, exp.GetValueAsDouble("Round(3.0,1)"));
            // 1.5 を丸め → MidpointRounding.AwayFromZero で 2
            Assert.AreEqual(2d, exp.GetValueAsDouble("Round(1.5,0)"));
        }

        // ──────────────────────────────────────────────
        // RoundUp
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RoundUp_CeilingBehavior()
        {
            var exp = Create();
            // RoundUp(3.14, 1) → ceiling at 1 decimal → 3.2
            Assert.AreEqual(3.2d, exp.GetValueAsDouble("RoundUp(3.14,1)"), 1e-10);
        }

        [TestMethod]
        public void RoundUp_WholeDigits()
        {
            var exp = Create();
            // RoundUp(31.4, -1) → ceiling to tens → 40
            Assert.AreEqual(40d, exp.GetValueAsDouble("RoundUp(31.4,-1)"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // RoundDown
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RoundDown_FloorBehavior()
        {
            var exp = Create();
            // RoundDown(3.19, 1) → floor at 1 decimal → 3.1
            Assert.AreEqual(3.1d, exp.GetValueAsDouble("RoundDown(3.19,1)"), 1e-10);
        }

        [TestMethod]
        public void RoundDown_WholeDigits()
        {
            var exp = Create();
            // RoundDown(39, -1) → floor to tens → 30
            Assert.AreEqual(30d, exp.GetValueAsDouble("RoundDown(39,-1)"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // Sqr
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Sqr_PerfectSquare_ReturnsExactRoot()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("Sqr(9)"), 1e-10);
            Assert.AreEqual(5d, exp.GetValueAsDouble("Sqr(25)"), 1e-10);
        }

        [TestMethod]
        public void Sqr_Two_ReturnsCorrectApproximation()
        {
            var exp = Create();
            var result = exp.GetValueAsDouble("Sqr(2)");
            Assert.AreEqual(System.Math.Sqrt(2), result, 1e-10);
        }

        // ──────────────────────────────────────────────
        // StringType evaluation
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Abs_StringResult_ReturnsFormattedNumber()
        {
            var exp = Create();
            Assert.AreEqual("5", exp.GetValueAsString("Abs(-5)"));
        }

        [TestMethod]
        public void Round_StringResult_ReturnsFormattedNumber()
        {
            var exp = Create();
            Assert.AreEqual("3", exp.GetValueAsString("Round(2.5,0)"));
        }

        // ──────────────────────────────────────────────
        // Min
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Min_TwoArgs_ReturnsSmaller()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("Min(3,5)"));
            Assert.AreEqual(3d, exp.GetValueAsDouble("Min(5,3)"));
        }

        [TestMethod]
        public void Min_NegativeValues_ReturnsMostNegative()
        {
            var exp = Create();
            Assert.AreEqual(-10d, exp.GetValueAsDouble("Min(-10,0)"));
        }

        // ──────────────────────────────────────────────
        // Max
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Max_TwoArgs_ReturnsLarger()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("Max(3,5)"));
            Assert.AreEqual(5d, exp.GetValueAsDouble("Max(5,3)"));
        }

        [TestMethod]
        public void Max_NegativeValues_ReturnsLessNegative()
        {
            var exp = Create();
            Assert.AreEqual(-1d, exp.GetValueAsDouble("Max(-10,-1)"));
        }

        // ──────────────────────────────────────────────
        // Abs with decimal
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Abs_Decimal_ReturnsPositive()
        {
            var exp = Create();
            Assert.AreEqual(3.14d, exp.GetValueAsDouble("Abs(-3.14)"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // Int with negative
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Int_NegativeHalf_FloorsBelowNegative()
        {
            var exp = Create();
            Assert.AreEqual(-3d, exp.GetValueAsDouble("Int(-2.5)"));
        }
    }
}
