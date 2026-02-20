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
    }
}
