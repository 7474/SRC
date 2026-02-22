using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression の変数操作の追加ユニットテスト（ExpressionVariableMoreTests3）
    /// </summary>
    [TestClass]
    public class ExpressionVariableMoreTests3
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // DefineGlobalVariable + GetValueAsString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DefineGlobalVariable_ThenSet_GetValueAsStringReturnsValue()
        {
            var exp = Create();
            exp.DefineGlobalVariable("gVar1");
            exp.SetVariableAsString("gVar1", "こんにちは");
            Assert.AreEqual("こんにちは", exp.GetValueAsString("gVar1"));
        }

        [TestMethod]
        public void DefineGlobalVariable_ThenSetDouble_GetValueAsStringReturnsNumeric()
        {
            var exp = Create();
            exp.DefineGlobalVariable("gNum");
            exp.SetVariableAsDouble("gNum", 42d);
            Assert.AreEqual("42", exp.GetValueAsString("gNum"));
        }

        [TestMethod]
        public void DefineGlobalVariable_IsGlobal_IsVariableDefinedReturnsTrue()
        {
            var exp = Create();
            exp.DefineGlobalVariable("globalCheck");
            Assert.IsTrue(exp.IsGlobalVariableDefined("globalCheck"));
        }

        [TestMethod]
        public void DefineGlobalVariable_MultipleVars_EachAccessible()
        {
            var exp = Create();
            exp.DefineGlobalVariable("gA");
            exp.DefineGlobalVariable("gB");
            exp.SetVariableAsDouble("gA", 1d);
            exp.SetVariableAsDouble("gB", 2d);
            Assert.AreEqual(1d, exp.GetValueAsDouble("gA"));
            Assert.AreEqual(2d, exp.GetValueAsDouble("gB"));
        }

        // ──────────────────────────────────────────────
        // SetVariableAsString + GetValueAsString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetVariableAsString_EmptyString_ReturnsEmpty()
        {
            var exp = Create();
            exp.SetVariableAsString("emptyVar", "");
            Assert.AreEqual("", exp.GetValueAsString("emptyVar"));
        }

        [TestMethod]
        public void SetVariableAsString_JapaneseValue_ReturnsCorrectValue()
        {
            var exp = Create();
            exp.SetVariableAsString("jp", "日本語テスト");
            Assert.AreEqual("日本語テスト", exp.GetValueAsString("jp"));
        }

        [TestMethod]
        public void SetVariableAsString_Overwrite_ReturnsNewValue()
        {
            var exp = Create();
            exp.SetVariableAsString("v", "old");
            exp.SetVariableAsString("v", "new");
            Assert.AreEqual("new", exp.GetValueAsString("v"));
        }

        // ──────────────────────────────────────────────
        // SetVariableAsDouble + GetValueAsLong
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetVariableAsDouble_GetValueAsLong_ReturnsInteger()
        {
            var exp = Create();
            exp.SetVariableAsDouble("intVar", 99d);
            Assert.AreEqual(99, exp.GetValueAsLong("intVar"));
        }

        [TestMethod]
        public void SetVariableAsDouble_NegativeValue_GetValueAsLongReturnsNegative()
        {
            var exp = Create();
            exp.SetVariableAsDouble("negVar", -10d);
            Assert.AreEqual(-10, exp.GetValueAsLong("negVar"));
        }

        [TestMethod]
        public void SetVariableAsDouble_ZeroValue_GetValueAsLongReturnsZero()
        {
            var exp = Create();
            exp.SetVariableAsDouble("zeroVar", 0d);
            Assert.AreEqual(0, exp.GetValueAsLong("zeroVar"));
        }

        [TestMethod]
        public void SetVariableAsDouble_FractionalValue_GetValueAsLongTruncates()
        {
            var exp = Create();
            exp.SetVariableAsDouble("fracVar", 3.9d);
            Assert.AreEqual(3, exp.GetValueAsLong("fracVar"));
        }

        // ──────────────────────────────────────────────
        // 変数を式として評価
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Variable_UsedInExpression_ReturnsCorrectSum()
        {
            var exp = Create();
            exp.SetVariableAsDouble("x", 5d);
            exp.SetVariableAsDouble("y", 3d);
            Assert.AreEqual(8, exp.GetValueAsLong("x + y"));
        }

        [TestMethod]
        public void Variable_UsedInMultiplication_ReturnsProduct()
        {
            var exp = Create();
            exp.SetVariableAsDouble("a", 6d);
            exp.SetVariableAsDouble("b", 7d);
            Assert.AreEqual(42, exp.GetValueAsLong("a * b"));
        }

        [TestMethod]
        public void Variable_Undefined_GetValueAsLong_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0, exp.GetValueAsLong("undefinedXYZ"));
        }

        [TestMethod]
        public void Variable_Undefined_GetValueAsString_ReturnsVariableName()
        {
            // 未定義変数は式として評価され、変数名がそのまま文字列として返る
            var exp = Create();
            Assert.AreEqual("undefinedStr", exp.GetValueAsString("undefinedStr"));
        }
    }
}
