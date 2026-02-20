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

        // ──────────────────────────────────────────────
        // 追加テスト: 比較・論理・文字列
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Compare_LessThanOrEqual_WhenLess_ReturnsTrue()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("3 <= 4"));
        }

        [TestMethod]
        public void Compare_GreaterThanOrEqual_WhenEqual_ReturnsTrue()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("5 >= 5"));
        }

        [TestMethod]
        public void Compare_GreaterThanOrEqual_WhenLess_ReturnsFalse()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("3 >= 4"));
        }

        [TestMethod]
        public void Compare_LessThanOrEqual_WhenGreater_ReturnsFalse()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("5 <= 4"));
        }

        [TestMethod]
        public void Compare_Equal_Strings_ReturnsTrue()
        {
            var exp = Create();
            exp.SetVariableAsString("s", "hello");
            Assert.AreEqual(1d, exp.GetValueAsDouble("\"hello\" = \"hello\""));
        }

        [TestMethod]
        public void Compare_NotEqual_Strings_ReturnsTrue()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("\"abc\" <> \"def\""));
        }

        [TestMethod]
        public void Logic_And_BothFalse_ReturnsFalse()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("0 And 0"));
        }

        [TestMethod]
        public void Logic_Or_BothFalse_ReturnsFalse()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("0 Or 0"));
        }

        [TestMethod]
        public void Logic_Or_BothTrue_ReturnsTrue()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("1 Or 1"));
        }

        [TestMethod]
        public void Logic_Not_NonZero_ReturnsFalse()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Not 5"));
        }

        [TestMethod]
        public void Concatenate_EmptyStrings_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("\"\" & \"\""));
        }

        [TestMethod]
        public void Concatenate_NumberAndString_Works()
        {
            var exp = Create();
            Assert.AreEqual("100点", exp.GetValueAsString("100 & \"点\""));
        }

        [TestMethod]
        public void ArrayVariable_StringValue_SetAndGet()
        {
            var exp = Create();
            exp.SetVariableAsString("names[1]", "Alice");
            exp.SetVariableAsString("names[2]", "Bob");
            Assert.AreEqual("Alice", exp.GetValueAsString("names[1]"));
            Assert.AreEqual("Bob", exp.GetValueAsString("names[2]"));
        }

        [TestMethod]
        public void SetVariableAsLong_ReturnsCorrectLongValue()
        {
            var exp = Create();
            exp.SetVariableAsLong("cnt", 42);
            Assert.AreEqual(42, exp.GetValueAsLong("cnt"));
        }

        [TestMethod]
        public void IsVariableDefined_AfterUndefine_ReturnsFalse()
        {
            var exp = Create();
            exp.SetVariableAsDouble("tmp", 1d);
            exp.UndefineVariable("tmp");
            Assert.IsFalse(exp.IsVariableDefined("tmp"));
        }
    }
}
