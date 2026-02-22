using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// GetValueAsString / GetValueAsDouble の is_term フラグ、
    /// および SetVariableAsLong / DefineGlobalVariable 等のユニットテスト
    /// </summary>
    [TestClass]
    public class ExpressionGetValueIsTermTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // GetValueAsString with is_term=true
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetValueAsString_IsTerm_Literal_ReturnsString()
        {
            var exp = Create();
            Assert.AreEqual("42", exp.GetValueAsString("42", is_term: true));
        }

        [TestMethod]
        public void GetValueAsString_IsTerm_Variable_ReturnsValue()
        {
            var exp = Create();
            exp.SetVariableAsString("name", "テスト");
            Assert.AreEqual("テスト", exp.GetValueAsString("name", is_term: true));
        }

        [TestMethod]
        public void GetValueAsString_IsTerm_QuotedString_ReturnsUnquoted()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("\"hello\"", is_term: true));
        }

        // ──────────────────────────────────────────────
        // GetValueAsDouble with is_term=true
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetValueAsDouble_IsTerm_NumericLiteral_ReturnsNumber()
        {
            var exp = Create();
            Assert.AreEqual(3.14d, exp.GetValueAsDouble("3.14", is_term: true), 1e-10);
        }

        [TestMethod]
        public void GetValueAsDouble_IsTerm_Variable_ReturnsValue()
        {
            var exp = Create();
            exp.SetVariableAsDouble("val", 99d);
            Assert.AreEqual(99d, exp.GetValueAsDouble("val", is_term: true));
        }

        [TestMethod]
        public void GetValueAsDouble_IsTerm_Zero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("0", is_term: true));
        }

        // ──────────────────────────────────────────────
        // SetVariableAsLong
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetVariableAsLong_ThenGetAsLong_ReturnsExactInt()
        {
            var exp = Create();
            exp.SetVariableAsLong("count", 7);
            Assert.AreEqual(7, exp.GetValueAsLong("count"));
        }

        [TestMethod]
        public void SetVariableAsLong_ThenGetAsDouble_ReturnsDouble()
        {
            var exp = Create();
            exp.SetVariableAsLong("count", 15);
            Assert.AreEqual(15d, exp.GetValueAsDouble("count"));
        }

        [TestMethod]
        public void SetVariableAsLong_NegativeValue_StoredCorrectly()
        {
            var exp = Create();
            exp.SetVariableAsLong("neg", -5);
            Assert.AreEqual(-5, exp.GetValueAsLong("neg"));
        }

        [TestMethod]
        public void SetVariableAsLong_Zero_StoredCorrectly()
        {
            var exp = Create();
            exp.SetVariableAsLong("z", 0);
            Assert.AreEqual(0, exp.GetValueAsLong("z"));
        }

        // ──────────────────────────────────────────────
        // DefineGlobalVariable
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DefineGlobalVariable_IsVariableDefined_ReturnsTrue()
        {
            var exp = Create();
            exp.DefineGlobalVariable("gVar");
            Assert.IsTrue(exp.IsVariableDefined("gVar"));
        }

        [TestMethod]
        public void DefineGlobalVariable_IsGlobalVariableDefined_ReturnsTrue()
        {
            var exp = Create();
            exp.DefineGlobalVariable("gTest");
            Assert.IsTrue(exp.IsGlobalVariableDefined("gTest"));
        }

        [TestMethod]
        public void DefineGlobalVariable_IsLocalVariableDefined_ReturnsFalse()
        {
            var exp = Create();
            exp.DefineGlobalVariable("gOnly");
            Assert.IsFalse(exp.IsLocalVariableDefined("gOnly"));
        }

        [TestMethod]
        public void LocalVariable_IsGlobalVariableDefined_ReturnsFalse()
        {
            var exp = Create();
            exp.SetVariableAsDouble("localVar", 1d);
            Assert.IsFalse(exp.IsGlobalVariableDefined("localVar"));
        }

        // ──────────────────────────────────────────────
        // IsVariableDefined with $ prefix
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsVariableDefined_DollarPrefix_FindsVariable()
        {
            var exp = Create();
            exp.SetVariableAsString("myStr", "value");
            Assert.IsTrue(exp.IsVariableDefined("$myStr"));
        }

        [TestMethod]
        public void IsVariableDefined_DollarPrefix_Undefined_ReturnsFalse()
        {
            var exp = Create();
            Assert.IsFalse(exp.IsVariableDefined("$noSuchVar"));
        }
    }
}
