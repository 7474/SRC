using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression 経由の日付・時刻関数テスト追加
    /// </summary>
    [TestClass]
    public class DateTimeFunctionTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // Now / Date / Time 関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Now_ReturnsNonEmptyString()
        {
            var exp = Create();
            var result = exp.GetValueAsString("Now");
            Assert.IsFalse(string.IsNullOrEmpty(result), $"Now returned empty: '{result}'");
        }

        [TestMethod]
        public void Now_ContainsYear()
        {
            var exp = Create();
            var result = exp.GetValueAsString("Now");
            var currentYear = System.DateTime.Now.Year.ToString();
            Assert.IsTrue(result.Contains(currentYear), $"Now doesn't contain year {currentYear}: '{result}'");
        }
    }

    /// <summary>
    /// Expression 経由のIF/IIF関数テスト追加
    /// </summary>
    [TestClass]
    public class ConditionalFunctionTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // IIf
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IIf_TrueCondition_ReturnsTrueValue()
        {
            var exp = Create();
            Assert.AreEqual("yes", exp.GetValueAsString("IIf(1 = 1,\"yes\",\"no\")"));
        }

        [TestMethod]
        public void IIf_FalseCondition_ReturnsFalseValue()
        {
            var exp = Create();
            Assert.AreEqual("no", exp.GetValueAsString("IIf(1 = 2,\"yes\",\"no\")"));
        }

        [TestMethod]
        public void IIf_NumericCondition_ReturnsCorrect()
        {
            var exp = Create();
            Assert.AreEqual(100d, exp.GetValueAsDouble("IIf(5 > 3, 100, 0)"));
        }

        [TestMethod]
        public void IIf_ZeroCondition_ReturnsFalseValue()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("IIf(0, 100, 0)"));
        }

        [TestMethod]
        public void IIf_NonZeroCondition_ReturnsTrueValue()
        {
            var exp = Create();
            Assert.AreEqual(100d, exp.GetValueAsDouble("IIf(1, 100, 0)"));
        }
    }

    /// <summary>
    /// Expression での変数操作追加テスト
    /// </summary>
    [TestClass]
    public class ExpressionVariableTests2
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // 変数の型変換
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NumericVariable_InArithmetic_UsesNumericValue()
        {
            var exp = Create();
            exp.SetVariableAsDouble("x", 10d);
            Assert.AreEqual(15d, exp.GetValueAsDouble("x + 5"));
        }

        [TestMethod]
        public void StringVariable_InArithmetic_UsedAsNumber()
        {
            var exp = Create();
            exp.SetVariableAsString("s", "10");
            Assert.AreEqual(15d, exp.GetValueAsDouble("s + 5"));
        }

        [TestMethod]
        public void NumericVariable_InConcatenation_ConvertsToString()
        {
            var exp = Create();
            exp.SetVariableAsDouble("n", 42d);
            Assert.AreEqual("x=42", exp.GetValueAsString("\"x=\" & n"));
        }

        [TestMethod]
        public void SetVariable_UpdatesExistingVariable()
        {
            var exp = Create();
            exp.SetVariableAsDouble("val", 5d);
            exp.SetVariableAsDouble("val", 20d);
            Assert.AreEqual(20d, exp.GetValueAsDouble("val"));
        }

        [TestMethod]
        public void MultipleVariables_DoNotInterfere()
        {
            var exp = Create();
            exp.SetVariableAsDouble("a", 1d);
            exp.SetVariableAsDouble("b", 2d);
            exp.SetVariableAsDouble("c", 3d);
            Assert.AreEqual(1d, exp.GetValueAsDouble("a"));
            Assert.AreEqual(2d, exp.GetValueAsDouble("b"));
            Assert.AreEqual(3d, exp.GetValueAsDouble("c"));
            Assert.AreEqual(6d, exp.GetValueAsDouble("a + b + c"));
        }

        // ──────────────────────────────────────────────
        // グローバル変数配列
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GlobalArrayVariable_SetMultipleIndices_AllAccessible()
        {
            var exp = Create();
            exp.SetVariableAsDouble("arr[1]", 10d);
            exp.SetVariableAsDouble("arr[2]", 20d);
            exp.SetVariableAsDouble("arr[3]", 30d);
            Assert.AreEqual(10d, exp.GetValueAsDouble("arr[1]"));
            Assert.AreEqual(20d, exp.GetValueAsDouble("arr[2]"));
            Assert.AreEqual(30d, exp.GetValueAsDouble("arr[3]"));
        }

        [TestMethod]
        public void GlobalArrayVariable_StringIndex_IsAccessible()
        {
            var exp = Create();
            exp.SetVariableAsString("map[key1]", "value1");
            Assert.AreEqual("value1", exp.GetValueAsString("map[key1]"));
        }

        // ──────────────────────────────────────────────
        // IsVariableDefined
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsVariableDefined_AfterSet_ReturnsTrue()
        {
            var exp = Create();
            exp.SetVariableAsDouble("defined", 5d);
            Assert.IsTrue(exp.IsVariableDefined("defined"));
        }

        [TestMethod]
        public void IsVariableDefined_NotSet_ReturnsFalse()
        {
            var exp = Create();
            Assert.IsFalse(exp.IsVariableDefined("notdefined_xyz_abc"));
        }
    }
}
