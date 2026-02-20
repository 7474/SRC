using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using System;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression経由で呼び出す三角関数・Max/Min/Random のユニットテスト
    /// </summary>
    [TestClass]
    public class MathTrigFunctionTests
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
        // Atn (Arc tangent)
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
            Assert.AreEqual(Math.PI / 4, exp.GetValueAsDouble("Atn(1)"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // Cos
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Cos_Zero_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("Cos(0)"), 1e-10);
        }

        [TestMethod]
        public void Cos_Pi_ReturnsMinusOne()
        {
            var exp = Create();
            var pi = Math.PI.ToString("R");
            Assert.AreEqual(-1d, exp.GetValueAsDouble($"Cos({pi})"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // Sin
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Sin_Zero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Sin(0)"), 1e-10);
        }

        [TestMethod]
        public void Sin_PiOver2_ReturnsOne()
        {
            var exp = Create();
            var piOver2 = (Math.PI / 2).ToString("R");
            Assert.AreEqual(1d, exp.GetValueAsDouble($"Sin({piOver2})"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // Tan
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Tan_Zero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Tan(0)"), 1e-10);
        }

        [TestMethod]
        public void Tan_PiOver4_ReturnsOne()
        {
            var exp = Create();
            var piOver4 = (Math.PI / 4).ToString("R");
            Assert.AreEqual(1d, exp.GetValueAsDouble($"Tan({piOver4})"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // Max
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Max_TwoArgs_ReturnsLarger()
        {
            var exp = Create();
            Assert.AreEqual(10d, exp.GetValueAsDouble("Max(3,10)"));
            Assert.AreEqual(10d, exp.GetValueAsDouble("Max(10,3)"));
        }

        [TestMethod]
        public void Max_EqualArgs_ReturnsSame()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("Max(5,5)"));
        }

        [TestMethod]
        public void Max_ThreeArgs_ReturnsLargest()
        {
            var exp = Create();
            Assert.AreEqual(15d, exp.GetValueAsDouble("Max(10,15,5)"));
        }

        [TestMethod]
        public void Max_NegativeNumbers_ReturnsLessNegative()
        {
            var exp = Create();
            Assert.AreEqual(-1d, exp.GetValueAsDouble("Max(-5,-1)"));
        }

        // ──────────────────────────────────────────────
        // Min
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Min_TwoArgs_ReturnsSmaller()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("Min(3,10)"));
            Assert.AreEqual(3d, exp.GetValueAsDouble("Min(10,3)"));
        }

        [TestMethod]
        public void Min_EqualArgs_ReturnsSame()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("Min(5,5)"));
        }

        [TestMethod]
        public void Min_ThreeArgs_ReturnsSmallest()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("Min(10,15,5)"));
        }

        [TestMethod]
        public void Min_NegativeNumbers_ReturnsMostNegative()
        {
            var exp = Create();
            Assert.AreEqual(-5d, exp.GetValueAsDouble("Min(-5,-1)"));
        }

        // ──────────────────────────────────────────────
        // Random
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Random_ReturnsValueInRange()
        {
            Lib.GeneralLib.RndSeed = 42;
            Lib.GeneralLib.RndReset();
            var exp = Create();
            for (int i = 0; i < 50; i++)
            {
                var result = exp.GetValueAsDouble("Random(10)");
                Assert.IsTrue(result >= 1 && result <= 10,
                    $"Random(10) returned {result} which is out of range [1, 10]");
            }
        }

        [TestMethod]
        public void Random_One_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("Random(1)"));
        }

        // ──────────────────────────────────────────────
        // Sqr (square root)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Sqr_Zero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Sqr(0)"), 1e-10);
        }

        [TestMethod]
        public void Sqr_One_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("Sqr(1)"), 1e-10);
        }
    }
}
