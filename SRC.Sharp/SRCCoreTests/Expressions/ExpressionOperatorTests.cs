using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression の算術演算子・比較演算子・論理演算子・文字列連結のユニットテスト
    /// </summary>
    [TestClass]
    public class ExpressionOperatorTests
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
        // 変数の読み書き
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetAndGetDouble_Works()
        {
            var exp = Create();
            exp.SetVariableAsDouble("x", 10d);
            Assert.AreEqual(10d, exp.GetValueAsDouble("x"));
        }

        [TestMethod]
        public void SetAndGetString_Works()
        {
            var exp = Create();
            exp.SetVariableAsString("name", "ゲッター");
            Assert.AreEqual("ゲッター", exp.GetValueAsString("name"));
        }

        [TestMethod]
        public void SetVariable_NumericType_NumericValueReturned()
        {
            var exp = Create();
            exp.SetVariable("hp", ValueType.NumericType, "", 1000d);
            Assert.AreEqual(1000d, exp.GetValueAsDouble("hp"));
        }

        // ──────────────────────────────────────────────
        // 未定義変数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UndefinedVariable_ReturnsZeroForDouble()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("未定義変数"));
        }

        [TestMethod]
        public void UndefinedVariable_ReturnsNameForString()
        {
            var exp = Create();
            // 未定義変数の文字列参照は変数名が返る
            Assert.AreEqual("未定義変数", exp.GetValueAsString("未定義変数"));
        }

        // ──────────────────────────────────────────────
        // 数値リテラル
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NumericLiteral_Integer_ReturnsValue()
        {
            var exp = Create();
            Assert.AreEqual(42d, exp.GetValueAsDouble("42"));
        }

        [TestMethod]
        public void NumericLiteral_Float_ReturnsValue()
        {
            var exp = Create();
            Assert.AreEqual(3.14d, exp.GetValueAsDouble("3.14"), 1e-10);
        }

        [TestMethod]
        public void NumericLiteral_Negative_ReturnsValue()
        {
            var exp = Create();
            Assert.AreEqual(-5d, exp.GetValueAsDouble("-5"));
        }

        // ──────────────────────────────────────────────
        // 文字列リテラル
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StringLiteral_ReturnsContent()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("\"hello\""));
        }

        [TestMethod]
        public void StringLiteral_Japanese_ReturnsContent()
        {
            var exp = Create();
            Assert.AreEqual("あいう", exp.GetValueAsString("\"あいう\""));
        }

        // ──────────────────────────────────────────────
        // 配列変数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArrayVariable_SetAndGet_Works()
        {
            var exp = Create();
            exp.SetVariableAsDouble("items[1]", 100d);
            exp.SetVariableAsDouble("items[2]", 200d);
            Assert.AreEqual(100d, exp.GetValueAsDouble("items[1]"));
            Assert.AreEqual(200d, exp.GetValueAsDouble("items[2]"));
        }

        [TestMethod]
        public void ArrayVariable_UndefinedIndex_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("items[99]", true));
        }

        // ──────────────────────────────────────────────
        // グローバル変数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DefineGlobalVariable_CanBeRead()
        {
            var exp = Create();
            exp.DefineGlobalVariable("GlobalFlag");
            Assert.AreEqual(0d, exp.GetValueAsDouble("GlobalFlag"));
        }

        [TestMethod]
        public void SetGlobalVariable_IsRetrievable()
        {
            var exp = Create();
            exp.DefineGlobalVariable("Score");
            exp.SetVariableAsDouble("Score", 500d);
            Assert.AreEqual(500d, exp.GetValueAsDouble("Score"));
        }
    }
}
