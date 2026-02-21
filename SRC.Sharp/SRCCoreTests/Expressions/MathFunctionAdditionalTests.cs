using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using System;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression 経由で呼び出す数学関数の追加テスト（エッジケース・未カバー機能）
    /// </summary>
    [TestClass]
    public class MathFunctionAdditionalTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // Abs (追加ケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Abs_LargeNegativeNumber_ReturnsPositive()
        {
            var exp = Create();
            Assert.AreEqual(1000000d, exp.GetValueAsDouble("Abs(-1000000)"));
        }

        [TestMethod]
        public void Abs_SmallDecimal_ReturnsPositive()
        {
            var exp = Create();
            Assert.AreEqual(0.001d, exp.GetValueAsDouble("Abs(-0.001)"), 1e-9);
        }

        // ──────────────────────────────────────────────
        // Int (追加ケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Int_Zero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Int(0)"));
        }

        [TestMethod]
        public void Int_PositiveJustBelowOne_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Int(0.999)"));
        }

        [TestMethod]
        public void Int_NegativeOne_ReturnsMinusOne()
        {
            var exp = Create();
            Assert.AreEqual(-1d, exp.GetValueAsDouble("Int(-1)"));
        }

        // ──────────────────────────────────────────────
        // Round (追加ケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Round_Zero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Round(0,0)"));
        }

        [TestMethod]
        public void Round_ExactHalf_RoundsAwayFromZero()
        {
            var exp = Create();
            // MidpointRounding.AwayFromZero: 2.5 → 3
            Assert.AreEqual(3d, exp.GetValueAsDouble("Round(2.5,0)"));
        }

        [TestMethod]
        public void Round_NegativeHalf_RoundsAwayFromZero()
        {
            var exp = Create();
            // -2.5 → -3
            Assert.AreEqual(-3d, exp.GetValueAsDouble("Round(-2.5,0)"));
        }

        [TestMethod]
        public void Round_TwoDecimalPlaces_RoundsCorrectly()
        {
            var exp = Create();
            // 3.145 → MidpointRounding.AwayFromZero → 3.15
            Assert.AreEqual(3.15d, exp.GetValueAsDouble("Round(3.145,2)"), 1e-9);
        }

        [TestMethod]
        public void Round_StringReturn_ReturnsFormattedNumber()
        {
            var exp = Create();
            Assert.AreEqual("3", exp.GetValueAsString("Round(3.14,0)"));
        }

        // ──────────────────────────────────────────────
        // RoundUp (追加ケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RoundUp_AlreadyWholeNumber_Unchanged()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("RoundUp(5,0)"), 1e-9);
        }

        [TestMethod]
        public void RoundUp_SmallDecimal_CeilsUp()
        {
            var exp = Create();
            // 3.01 → ceil to 4
            Assert.AreEqual(4d, exp.GetValueAsDouble("RoundUp(3.01,0)"), 1e-9);
        }

        [TestMethod]
        public void RoundUp_NegativeNumber_CeilsTowardZero()
        {
            var exp = Create();
            // -3.1 → ceil → -3
            Assert.AreEqual(-3d, exp.GetValueAsDouble("RoundUp(-3.1,0)"), 1e-9);
        }

        [TestMethod]
        public void RoundUp_TwoDecimalPlaces()
        {
            var exp = Create();
            Assert.AreEqual(3.15d, exp.GetValueAsDouble("RoundUp(3.141,2)"), 1e-9);
        }

        // ──────────────────────────────────────────────
        // RoundDown (追加ケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RoundDown_AlreadyWholeNumber_Unchanged()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("RoundDown(5,0)"), 1e-9);
        }

        [TestMethod]
        public void RoundDown_SmallDecimal_FloorsDown()
        {
            var exp = Create();
            // 3.99 → floor to 3
            Assert.AreEqual(3d, exp.GetValueAsDouble("RoundDown(3.99,0)"), 1e-9);
        }

        [TestMethod]
        public void RoundDown_NegativeNumber_FloorsAwayFromZero()
        {
            var exp = Create();
            // -3.1 → floor → -4
            Assert.AreEqual(-4d, exp.GetValueAsDouble("RoundDown(-3.1,0)"), 1e-9);
        }

        [TestMethod]
        public void RoundDown_TwoDecimalPlaces()
        {
            var exp = Create();
            Assert.AreEqual(3.14d, exp.GetValueAsDouble("RoundDown(3.149,2)"), 1e-9);
        }

        // ──────────────────────────────────────────────
        // Sqr (追加ケース)
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

        [TestMethod]
        public void Sqr_FourteenPow2_ReturnsCorrect()
        {
            var exp = Create();
            Assert.AreEqual(Math.Sqrt(144d), exp.GetValueAsDouble("Sqr(144)"), 1e-10);
        }

        [TestMethod]
        public void Sqr_StringReturn_ReturnsFormattedNumber()
        {
            var exp = Create();
            Assert.AreEqual("4", exp.GetValueAsString("Sqr(16)"));
        }

        // ──────────────────────────────────────────────
        // Max (追加ケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Max_ThreeArgs_ReturnsLargest()
        {
            var exp = Create();
            Assert.AreEqual(100d, exp.GetValueAsDouble("Max(10,100,50)"));
        }

        [TestMethod]
        public void Max_AllEqual_ReturnsValue()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("Max(5,5,5)"));
        }

        [TestMethod]
        public void Max_SingleArg_ReturnsArg()
        {
            var exp = Create();
            Assert.AreEqual(42d, exp.GetValueAsDouble("Max(42)"));
        }

        [TestMethod]
        public void Max_NegativeAndPositive_ReturnsPositive()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("Max(-100,-50,1)"));
        }

        [TestMethod]
        public void Max_StringReturn_ReturnsFormattedNumber()
        {
            var exp = Create();
            Assert.AreEqual("10", exp.GetValueAsString("Max(5,10,3)"));
        }

        // ──────────────────────────────────────────────
        // Min (追加ケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Min_ThreeArgs_ReturnsSmallest()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("Min(10,1,50)"));
        }

        [TestMethod]
        public void Min_AllEqual_ReturnsValue()
        {
            var exp = Create();
            Assert.AreEqual(7d, exp.GetValueAsDouble("Min(7,7,7)"));
        }

        [TestMethod]
        public void Min_SingleArg_ReturnsArg()
        {
            var exp = Create();
            Assert.AreEqual(42d, exp.GetValueAsDouble("Min(42)"));
        }

        [TestMethod]
        public void Min_Decimals_ReturnsSmallest()
        {
            var exp = Create();
            Assert.AreEqual(0.1d, exp.GetValueAsDouble("Min(0.5,0.1,0.9)"), 1e-10);
        }

        [TestMethod]
        public void Min_StringReturn_ReturnsFormattedNumber()
        {
            var exp = Create();
            Assert.AreEqual("1", exp.GetValueAsString("Min(5,1,3)"));
        }

        // ──────────────────────────────────────────────
        // Tan (追加ケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Tan_Zero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Tan(0)"), 1e-10);
        }

        [TestMethod]
        public void Tan_PiOver4_ReturnsApproximatelyOne()
        {
            var exp = Create();
            var piOver4 = (Math.PI / 4).ToString("R");
            Assert.AreEqual(1d, exp.GetValueAsDouble($"Tan({piOver4})"), 1e-10);
        }

        [TestMethod]
        public void Tan_StringReturn_ReturnsFormattedNumber()
        {
            var exp = Create();
            Assert.AreEqual("0", exp.GetValueAsString("Tan(0)"));
        }
    }
}
