using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression のIf関数・Switch関数・その他ユーティリティ関数のユニットテスト
    /// </summary>
    [TestClass]
    public class UtilityFunctionTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // IIf 関数 (インラインIF)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IIf_ConditionTrue_ReturnsFirstValue()
        {
            var exp = Create();
            Assert.AreEqual(10d, exp.GetValueAsDouble("IIf(1,10,20)"));
        }

        [TestMethod]
        public void IIf_ConditionFalse_ReturnsSecondValue()
        {
            var exp = Create();
            Assert.AreEqual(20d, exp.GetValueAsDouble("IIf(0,10,20)"));
        }

        [TestMethod]
        public void IIf_ConditionOne_ReturnsFirstString()
        {
            var exp = Create();
            Assert.AreEqual("yes", exp.GetValueAsString("IIf(1,\"yes\",\"no\")"));
        }

        [TestMethod]
        public void IIf_ConditionZero_ReturnsSecondString()
        {
            var exp = Create();
            Assert.AreEqual("no", exp.GetValueAsString("IIf(0,\"yes\",\"no\")"));
        }

        [TestMethod]
        public void IIf_ExpressionCondition_EvaluatesCorrectly()
        {
            var exp = Create();
            Assert.AreEqual(100d, exp.GetValueAsDouble("IIf(5 > 3, 100, 200)"));
        }

        [TestMethod]
        public void IIf_ExpressionFalseCondition_EvaluatesCorrectly()
        {
            var exp = Create();
            Assert.AreEqual(200d, exp.GetValueAsDouble("IIf(5 < 3, 100, 200)"));
        }

        // ──────────────────────────────────────────────
        // Asc 関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Asc_UppercaseZ_Returns90()
        {
            var exp = Create();
            Assert.AreEqual(90d, exp.GetValueAsDouble("Asc(\"Z\")"));
        }

        [TestMethod]
        public void Asc_Digit0_Returns48()
        {
            var exp = Create();
            Assert.AreEqual(48d, exp.GetValueAsDouble("Asc(\"0\")"));
        }

        // ──────────────────────────────────────────────
        // Count 関数（配列変数の要素数）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Count_NoArrayElements_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Count(myArr)"));
        }

        [TestMethod]
        public void Count_WithArrayElements_ReturnsCount()
        {
            var exp = Create();
            exp.SetVariableAsDouble("myArr[1]", 10d);
            exp.SetVariableAsDouble("myArr[2]", 20d);
            exp.SetVariableAsDouble("myArr[3]", 30d);
            Assert.AreEqual(3d, exp.GetValueAsDouble("Count(myArr)"));
        }

        [TestMethod]
        public void Count_EmptyArrayName_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Count(nonexistent)"));
        }
    }
}
