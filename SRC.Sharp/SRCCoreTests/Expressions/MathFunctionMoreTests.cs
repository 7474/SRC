using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using System;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression 経由で呼び出す数学関数のさらなる追加テスト
    /// （既存テストファイル未カバー分）
    /// </summary>
    [TestClass]
    public class MathFunctionMoreTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // Sin: 追加ケース（MathTrigFunctionTests 未カバー）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Sin_Pi_ReturnsApproximatelyZero()
        {
            var exp = Create();
            var pi = Math.PI.ToString("R");
            Assert.AreEqual(0d, exp.GetValueAsDouble($"Sin({pi})"), 1e-10);
        }

        [TestMethod]
        public void Sin_StringResult_ReturnsFormattedNumber()
        {
            var exp = Create();
            Assert.AreEqual("0", exp.GetValueAsString("Sin(0)"));
        }

        // ──────────────────────────────────────────────
        // Cos: 追加ケース（MathTrigFunctionTests 未カバー）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Cos_PiOverTwo_ReturnsApproximatelyZero()
        {
            var exp = Create();
            var piOver2 = (Math.PI / 2).ToString("R");
            Assert.AreEqual(0d, exp.GetValueAsDouble($"Cos({piOver2})"), 1e-10);
        }

        [TestMethod]
        public void Cos_StringResult_ReturnsFormattedNumber()
        {
            var exp = Create();
            Assert.AreEqual("1", exp.GetValueAsString("Cos(0)"));
        }

        // ──────────────────────────────────────────────
        // Atn: 追加ケース（MathTrigFunctionTests 未カバー）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Atn_MinusOne_ReturnsNegativePiOverFour()
        {
            var exp = Create();
            Assert.AreEqual(-Math.PI / 4, exp.GetValueAsDouble("Atn(-1)"), 1e-10);
        }

        [TestMethod]
        public void Atn_StringResult_ReturnsFormattedNumber()
        {
            var exp = Create();
            Assert.AreEqual("0", exp.GetValueAsString("Atn(0)"));
        }

        // ──────────────────────────────────────────────
        // Tan: 追加ケース（既存未カバー）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Tan_NegativePiOverFour_ReturnsMinusOne()
        {
            var exp = Create();
            var val = (-Math.PI / 4).ToString("R");
            Assert.AreEqual(-1d, exp.GetValueAsDouble($"Tan({val})"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // Abs: 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Abs_VerySmallPositive_ReturnsSame()
        {
            var exp = Create();
            Assert.AreEqual(0.0001d, exp.GetValueAsDouble("Abs(0.0001)"), 1e-9);
        }

        // ──────────────────────────────────────────────
        // Sqr: 追加ケース（MathTrigFunctionTests/MathFunctionAdditionalTests 未カバー）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Sqr_100_ReturnsTen()
        {
            var exp = Create();
            Assert.AreEqual(10d, exp.GetValueAsDouble("Sqr(100)"), 1e-10);
        }

        [TestMethod]
        public void Sqr_DecimalInput_ReturnsCorrectRoot()
        {
            var exp = Create();
            Assert.AreEqual(Math.Sqrt(2.25), exp.GetValueAsDouble("Sqr(2.25)"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // Int: 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Int_LargePositive_ReturnsFloor()
        {
            var exp = Create();
            Assert.AreEqual(99d, exp.GetValueAsDouble("Int(99.99)"));
        }

        [TestMethod]
        public void Int_StringResult_ReturnsFormattedNumber()
        {
            var exp = Create();
            Assert.AreEqual("3", exp.GetValueAsString("Int(3.9)"));
        }

        // ──────────────────────────────────────────────
        // Max / Min: 5 引数・ゼロ混合
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Max_WithZero_ReturnsPositive()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("Max(0,5)"));
        }

        [TestMethod]
        public void Min_WithZero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Min(0,5)"));
        }

        [TestMethod]
        public void Max_FiveArgs_ReturnsLargest()
        {
            var exp = Create();
            Assert.AreEqual(99d, exp.GetValueAsDouble("Max(1,2,99,3,4)"));
        }

        [TestMethod]
        public void Min_FiveArgs_ReturnsSmallest()
        {
            var exp = Create();
            Assert.AreEqual(-10d, exp.GetValueAsDouble("Min(0,-10,5,-3,2)"));
        }

        // ──────────────────────────────────────────────
        // RoundUp / RoundDown: ゼロ・文字列返却
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RoundUp_Zero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("RoundUp(0,0)"), 1e-9);
        }

        [TestMethod]
        public void RoundDown_Zero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("RoundDown(0,0)"), 1e-9);
        }

        [TestMethod]
        public void RoundUp_StringResult_ReturnsFormattedNumber()
        {
            var exp = Create();
            Assert.AreEqual("4", exp.GetValueAsString("RoundUp(3.1,0)"));
        }

        [TestMethod]
        public void RoundDown_StringResult_ReturnsFormattedNumber()
        {
            var exp = Create();
            Assert.AreEqual("3", exp.GetValueAsString("RoundDown(3.9,0)"));
        }
    }
}
