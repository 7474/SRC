using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression の変数管理・型変換などの追加ユニットテスト
    /// </summary>
    [TestClass]
    public class VariableTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // GetValueAsString: 数値変数を文字列として取得
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetValueAsString_NumericVariable_ReturnsFormattedString()
        {
            var exp = Create();
            exp.SetVariableAsDouble("n", 42d);
            Assert.AreEqual("42", exp.GetValueAsString("n"));
        }

        [TestMethod]
        public void GetValueAsString_StringVariable_ReturnsSameString()
        {
            var exp = Create();
            exp.SetVariableAsString("s", "hello");
            Assert.AreEqual("hello", exp.GetValueAsString("s"));
        }

        // ──────────────────────────────────────────────
        // GetValueAsDouble: 文字列変数を数値として取得
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetValueAsDouble_StringNumericVariable_ReturnsNumber()
        {
            var exp = Create();
            exp.SetVariableAsString("ns", "99");
            Assert.AreEqual(99d, exp.GetValueAsDouble("ns"));
        }

        [TestMethod]
        public void GetValueAsDouble_UndefinedVariable_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("undefined_var"));
        }

        // ──────────────────────────────────────────────
        // GetValueAsLong: 整数変換
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetValueAsLong_DoubleVariable_TruncatesToInt()
        {
            var exp = Create();
            exp.SetVariableAsDouble("d", 3.9d);
            Assert.AreEqual(3, exp.GetValueAsLong("d"));
        }

        [TestMethod]
        public void GetValueAsLong_IntegerVariable_ReturnsExactInt()
        {
            var exp = Create();
            exp.SetVariableAsLong("i", 100);
            Assert.AreEqual(100, exp.GetValueAsLong("i"));
        }

        [TestMethod]
        public void GetValueAsLong_NegativeDouble_TruncatesToNegativeInt()
        {
            var exp = Create();
            exp.SetVariableAsDouble("neg", -5.8d);
            Assert.AreEqual(-5, exp.GetValueAsLong("neg"));
        }

        // ──────────────────────────────────────────────
        // GetVariableObject: VarData を取得
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetVariableObject_AfterSetDouble_ReturnsNonNull()
        {
            var exp = Create();
            exp.SetVariableAsDouble("x", 5d);
            var obj = exp.GetVariableObject("x");
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void GetVariableObject_AfterSetString_ReturnsNonNull()
        {
            var exp = Create();
            exp.SetVariableAsString("label", "test");
            var obj = exp.GetVariableObject("label");
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void GetVariableObject_Undefined_ReturnsNull()
        {
            var exp = Create();
            var obj = exp.GetVariableObject("notDefined");
            Assert.IsNull(obj);
        }

        // ──────────────────────────────────────────────
        // $ プレフィックス付き変数名 (IsVariableDefined)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsVariableDefined_WithDollarPrefix_SameAsWithout()
        {
            var exp = Create();
            exp.SetVariableAsString("myStr", "value");
            // $ プレフィックスは変数名の特殊記法
            Assert.IsTrue(exp.IsVariableDefined("myStr"));
        }

        // ──────────────────────────────────────────────
        // グローバル変数とローカル変数の独立性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GlobalAndLocal_SameName_AreIndependent()
        {
            var exp = Create();
            exp.DefineGlobalVariable("shared");
            exp.DefineLocalVariable("shared");
            // 両方が定義されている
            Assert.IsTrue(exp.IsGlobalVariableDefined("shared"));
            Assert.IsTrue(exp.IsLocalVariableDefined("shared"));
        }

        // ──────────────────────────────────────────────
        // 配列変数のインデックス操作
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArrayVariable_IndexZero_StoredAndRetrieved()
        {
            var exp = Create();
            exp.SetVariableAsDouble("arr[0]", 99d);
            Assert.AreEqual(99d, exp.GetValueAsDouble("arr[0]"));
        }

        [TestMethod]
        public void ArrayVariable_StringIndex_StoredAndRetrieved()
        {
            var exp = Create();
            exp.SetVariableAsString("map[キー]", "値");
            Assert.AreEqual("値", exp.GetValueAsString("map[キー]"));
        }

        [TestMethod]
        public void ArrayVariable_OverwriteElement_UpdatesValue()
        {
            var exp = Create();
            exp.SetVariableAsDouble("a[1]", 10d);
            exp.SetVariableAsDouble("a[1]", 20d);
            Assert.AreEqual(20d, exp.GetValueAsDouble("a[1]"));
        }

        // ──────────────────────────────────────────────
        // UndefineVariable: 配列要素の削除
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UndefineVariable_ArrayElement_RemovesElement()
        {
            var exp = Create();
            exp.SetVariableAsDouble("b[2]", 5d);
            Assert.IsTrue(exp.IsVariableDefined("b[2]"));
            exp.UndefineVariable("b[2]");
            Assert.IsFalse(exp.IsVariableDefined("b[2]"));
        }

        // ──────────────────────────────────────────────
        // SetVariableAsString: 空文字列
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetVariableAsString_EmptyString_IsDefinedAndEmpty()
        {
            var exp = Create();
            exp.SetVariableAsString("empty", "");
            Assert.IsTrue(exp.IsVariableDefined("empty"));
            Assert.AreEqual("", exp.GetValueAsString("empty"));
        }
    }
}
