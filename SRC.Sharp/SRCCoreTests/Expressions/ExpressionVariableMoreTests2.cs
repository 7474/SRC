using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression の変数配列操作 (SetVar, GlobalVar) の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class ExpressionVariableMoreTests2
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // 配列変数の設定と読み取り
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetArrayVariable_CanBeRead()
        {
            var exp = Create();
            exp.SetVariableAsDouble("scores[1]", 100d);
            Assert.AreEqual(100d, exp.GetValueAsDouble("scores[1]"));
        }

        [TestMethod]
        public void SetArrayVariable_MultipleElements_IndependentValues()
        {
            var exp = Create();
            exp.SetVariableAsDouble("items[1]", 10d);
            exp.SetVariableAsDouble("items[2]", 20d);
            exp.SetVariableAsDouble("items[3]", 30d);
            Assert.AreEqual(10d, exp.GetValueAsDouble("items[1]"));
            Assert.AreEqual(20d, exp.GetValueAsDouble("items[2]"));
            Assert.AreEqual(30d, exp.GetValueAsDouble("items[3]"));
        }

        [TestMethod]
        public void SetArrayVariableAsString_CanBeRead()
        {
            var exp = Create();
            exp.SetVariableAsString("names[1]", "アムロ");
            Assert.AreEqual("アムロ", exp.GetValueAsString("names[1]"));
        }

        [TestMethod]
        public void ArrayVariable_Overwrite_ReturnsNewValue()
        {
            var exp = Create();
            exp.SetVariableAsDouble("x[1]", 10d);
            exp.SetVariableAsDouble("x[1]", 99d);
            Assert.AreEqual(99d, exp.GetValueAsDouble("x[1]"));
        }

        // ──────────────────────────────────────────────
        // グローバル変数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DefineGlobalVariable_CanBeSetAndRead()
        {
            var exp = Create();
            exp.DefineGlobalVariable("gv1");
            exp.SetVariableAsDouble("gv1", 42d);
            Assert.AreEqual(42d, exp.GetValueAsDouble("gv1"));
        }

        [TestMethod]
        public void DefineGlobalVariable_IsGlobalAfterDefine()
        {
            var exp = Create();
            exp.DefineGlobalVariable("globalTest");
            Assert.IsTrue(exp.IsGlobalVariableDefined("globalTest"));
        }

        [TestMethod]
        public void GlobalVariable_StillDefinedAfterSetValue()
        {
            var exp = Create();
            exp.DefineGlobalVariable("gv2");
            exp.SetVariableAsDouble("gv2", 100d);
            Assert.IsTrue(exp.IsGlobalVariableDefined("gv2"));
        }

        // ──────────────────────────────────────────────
        // ローカル変数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DefineLocalVariable_CanBeRead()
        {
            var exp = Create();
            exp.DefineLocalVariable("lv1");
            exp.SetVariableAsDouble("lv1", 55d);
            Assert.AreEqual(55d, exp.GetValueAsDouble("lv1"));
        }

        [TestMethod]
        public void LocalVariable_IsLocalAfterDefine()
        {
            var exp = Create();
            exp.DefineLocalVariable("lv2");
            Assert.IsTrue(exp.IsLocalVariableDefined("lv2"));
        }

        // ──────────────────────────────────────────────
        // 未定義変数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UndefinedVariable_GetValueAsDouble_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("undefinedVar"));
        }

        [TestMethod]
        public void UndefinedVariable_IsVariableDefined_ReturnsFalse()
        {
            var exp = Create();
            Assert.IsFalse(exp.IsVariableDefined("undefinedVar"));
        }

        // ──────────────────────────────────────────────
        // 変数のアンdefine
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UndefineVariable_VariableBecomesUndefined()
        {
            var exp = Create();
            exp.SetVariableAsDouble("removeMe", 99d);
            Assert.IsTrue(exp.IsVariableDefined("removeMe"));
            exp.UndefineVariable("removeMe");
            Assert.IsFalse(exp.IsVariableDefined("removeMe"));
        }

        [TestMethod]
        public void UndefineVariable_NonExistent_DoesNotThrow()
        {
            var exp = Create();
            // 存在しない変数をundefineしてもエラーにならない
            exp.UndefineVariable("notExist");
        }
    }
}
