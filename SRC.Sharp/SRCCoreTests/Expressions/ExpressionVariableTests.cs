using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression クラスの変数管理 (IsVariableDefined, UndefineVariable, DumpVariables) のユニットテスト
    /// </summary>
    [TestClass]
    public class ExpressionVariableTests
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
        // IsVariableDefined
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsVariableDefined_ReturnsFalse_BeforeSet()
        {
            var exp = Create();
            Assert.IsFalse(exp.IsVariableDefined("x"));
        }

        [TestMethod]
        public void IsVariableDefined_ReturnsTrue_AfterSet()
        {
            var exp = Create();
            exp.SetVariableAsDouble("x", 5d);
            Assert.IsTrue(exp.IsVariableDefined("x"));
        }

        [TestMethod]
        public void IsVariableDefined_ArrayElement_ReturnsTrue_AfterSet()
        {
            var exp = Create();
            exp.SetVariableAsDouble("arr[1]", 10d);
            Assert.IsTrue(exp.IsVariableDefined("arr[1]"));
        }

        // ──────────────────────────────────────────────
        // IsGlobalVariableDefined / IsLocalVariableDefined
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsGlobalVariableDefined_AfterDefine_ReturnsTrue()
        {
            var exp = Create();
            exp.DefineGlobalVariable("GlobalVar");
            Assert.IsTrue(exp.IsGlobalVariableDefined("GlobalVar"));
        }

        [TestMethod]
        public void IsGlobalVariableDefined_BeforeDefine_ReturnsFalse()
        {
            var exp = Create();
            Assert.IsFalse(exp.IsGlobalVariableDefined("GlobalVar"));
        }

        [TestMethod]
        public void IsLocalVariableDefined_AfterDefine_ReturnsTrue()
        {
            var exp = Create();
            exp.DefineLocalVariable("LocalVar");
            Assert.IsTrue(exp.IsLocalVariableDefined("LocalVar"));
        }

        [TestMethod]
        public void IsLocalVariableDefined_BeforeDefine_ReturnsFalse()
        {
            var exp = Create();
            Assert.IsFalse(exp.IsLocalVariableDefined("NotDefined"));
        }

        // ──────────────────────────────────────────────
        // UndefineVariable
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UndefineVariable_RemovesVariable()
        {
            var exp = Create();
            exp.SetVariableAsDouble("toRemove", 42d);
            Assert.IsTrue(exp.IsVariableDefined("toRemove"));
            exp.UndefineVariable("toRemove");
            Assert.IsFalse(exp.IsVariableDefined("toRemove"));
        }

        [TestMethod]
        public void UndefineVariable_NonExistent_DoesNotThrow()
        {
            var exp = Create();
            // Should not throw for non-existent variable
            exp.UndefineVariable("notExists");
        }

        // ──────────────────────────────────────────────
        // SetVariableAsLong
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetVariableAsLong_SetsIntegerValue()
        {
            var exp = Create();
            exp.SetVariableAsLong("count", 7);
            Assert.AreEqual(7d, exp.GetValueAsDouble("count"));
            Assert.AreEqual(7, exp.GetValueAsLong("count"));
        }

        // ──────────────────────────────────────────────
        // DumpVariables
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DumpVariables_ContainsSetVariables()
        {
            var exp = Create();
            exp.SetVariableAsDouble("score", 100d);
            var dump = exp.DumpVariables();
            Assert.IsNotNull(dump);
            Assert.IsTrue(dump.Contains("score"));
        }

        [TestMethod]
        public void DumpVariables_EmptyWhenNoVariables()
        {
            var exp = Create();
            var dump = exp.DumpVariables();
            Assert.IsNotNull(dump);
        }

        // ──────────────────────────────────────────────
        // 追加テスト: エッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetVariableAsDouble_OverwriteExisting_UpdatesValue()
        {
            var exp = Create();
            exp.SetVariableAsDouble("v", 10d);
            exp.SetVariableAsDouble("v", 20d);
            Assert.AreEqual(20d, exp.GetValueAsDouble("v"));
        }

        [TestMethod]
        public void SetVariableAsString_OverwriteExisting_UpdatesValue()
        {
            var exp = Create();
            exp.SetVariableAsString("s", "old");
            exp.SetVariableAsString("s", "new");
            Assert.AreEqual("new", exp.GetValueAsString("s"));
        }

        [TestMethod]
        public void SetVariableAsDouble_Zero_IsDefinedAndZero()
        {
            var exp = Create();
            exp.SetVariableAsDouble("z", 0d);
            Assert.IsTrue(exp.IsVariableDefined("z"));
            Assert.AreEqual(0d, exp.GetValueAsDouble("z"));
        }

        [TestMethod]
        public void SetVariableAsDouble_NegativeValue_Stored()
        {
            var exp = Create();
            exp.SetVariableAsDouble("neg", -99d);
            Assert.AreEqual(-99d, exp.GetValueAsDouble("neg"));
        }

        [TestMethod]
        public void GlobalVariable_SetValue_PersistsAfterSet()
        {
            var exp = Create();
            exp.DefineGlobalVariable("g");
            exp.SetVariableAsDouble("g", 123d);
            Assert.AreEqual(123d, exp.GetValueAsDouble("g"));
            Assert.IsTrue(exp.IsGlobalVariableDefined("g"));
        }

        [TestMethod]
        public void LocalVariable_DefineThenSet_Works()
        {
            var exp = Create();
            exp.DefineLocalVariable("lv");
            Assert.IsTrue(exp.IsLocalVariableDefined("lv"));
            exp.SetVariableAsDouble("lv", 55d);
            Assert.AreEqual(55d, exp.GetValueAsDouble("lv"));
        }

        [TestMethod]
        public void ArrayVariable_MultipleElements_AllRetained()
        {
            var exp = Create();
            for (var i = 1; i <= 5; i++)
            {
                exp.SetVariableAsDouble("arr[" + i + "]", i * 10d);
            }
            for (var i = 1; i <= 5; i++)
            {
                Assert.AreEqual(i * 10d, exp.GetValueAsDouble("arr[" + i + "]"));
            }
        }

        [TestMethod]
        public void SetVariableAsLong_LargeValue_Stored()
        {
            var exp = Create();
            exp.SetVariableAsLong("big", 100000);
            Assert.AreEqual(100000, exp.GetValueAsLong("big"));
        }

        [TestMethod]
        public void DumpVariables_MultipleVariables_ContainsAll()
        {
            var exp = Create();
            exp.SetVariableAsDouble("a", 1d);
            exp.SetVariableAsString("b", "hello");
            var dump = exp.DumpVariables();
            Assert.IsTrue(dump.Contains("a"));
            Assert.IsTrue(dump.Contains("b"));
        }
    }
}
