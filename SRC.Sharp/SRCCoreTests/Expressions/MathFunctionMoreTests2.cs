using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression 経由で呼び出す数学関数の更なる追加テスト2
    /// </summary>
    [TestClass]
    public class MathFunctionMoreTests2
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // Sin / Cos 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Sin_Zero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Sin(0)"), 1e-10);
        }

        [TestMethod]
        public void Cos_Zero_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("Cos(0)"), 1e-10);
        }

        [TestMethod]
        public void Sin_Pi_ReturnsZero()
        {
            var exp = Create();
            var pi = System.Math.PI.ToString("R");
            Assert.AreEqual(0d, exp.GetValueAsDouble($"Sin({pi})"), 1e-10);
        }

        [TestMethod]
        public void Cos_Pi_ReturnsMinusOne()
        {
            var exp = Create();
            var pi = System.Math.PI.ToString("R");
            Assert.AreEqual(-1d, exp.GetValueAsDouble($"Cos({pi})"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // Atn (逆タンジェント)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Atn_Zero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Atn(0)"), 1e-10);
        }

        [TestMethod]
        public void Atn_One_ReturnsPiOver4()
        {
            var exp = Create();
            Assert.AreEqual(System.Math.PI / 4, exp.GetValueAsDouble("Atn(1)"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // Abs / Int 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Abs_Zero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Abs(0)"));
        }

        [TestMethod]
        public void Abs_PositiveNumber_ReturnsUnchanged()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("Abs(5)"));
        }

        [TestMethod]
        public void Int_LargeNegative_ReturnsFloor()
        {
            var exp = Create();
            // Int(-1.1) = -2 (VB: Int always rounds toward negative infinity)
            Assert.AreEqual(-2d, exp.GetValueAsDouble("Int(-1.1)"));
        }

        [TestMethod]
        public void Int_ExactInteger_ReturnsUnchanged()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("Int(5)"));
        }

        // ──────────────────────────────────────────────
        // Sqr 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Sqr_NinetyEight_ReturnsApproximateValue()
        {
            var exp = Create();
            Assert.AreEqual(System.Math.Sqrt(98d), exp.GetValueAsDouble("Sqr(98)"), 1e-10);
        }

        [TestMethod]
        public void Sqr_TwoHundred_ReturnsApproximateValue()
        {
            var exp = Create();
            Assert.AreEqual(System.Math.Sqrt(200d), exp.GetValueAsDouble("Sqr(200)"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // Max / Min 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Max_TwoArgs_ReturnsLarger()
        {
            var exp = Create();
            Assert.AreEqual(7d, exp.GetValueAsDouble("Max(3,7)"));
        }

        [TestMethod]
        public void Min_TwoArgs_ReturnsSmaller()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("Min(3,7)"));
        }

        [TestMethod]
        public void Max_NegativeNumbers_ReturnsLessNegative()
        {
            var exp = Create();
            Assert.AreEqual(-1d, exp.GetValueAsDouble("Max(-5,-1)"));
        }

        [TestMethod]
        public void Min_NegativeNumbers_ReturnsMoreNegative()
        {
            var exp = Create();
            Assert.AreEqual(-5d, exp.GetValueAsDouble("Min(-5,-1)"));
        }

        // ──────────────────────────────────────────────
        // Rnd → Random (追加ケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Random_ReturnsBetweenZeroAndOne()
        {
            var exp = Create();
            var result = exp.GetValueAsDouble("Random(1)");
            Assert.IsTrue(result >= 0d && result <= 1d, $"Random(1) should be between 0 and 1, was: {result}");
        }
    }
}
