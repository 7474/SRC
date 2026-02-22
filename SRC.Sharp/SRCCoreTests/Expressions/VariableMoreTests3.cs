using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    [TestClass]
    public class VariableMoreTests3
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        [TestMethod]
        public void SetVariableAsDouble_ThenGet_ReturnsValue()
        {
            var exp = Create();
            exp.SetVariableAsDouble("v1", 123d);
            Assert.AreEqual(123d, exp.GetValueAsDouble("v1"));
        }

        [TestMethod]
        public void SetVariableAsString_ThenGet_ReturnsValue()
        {
            var exp = Create();
            exp.SetVariableAsString("s1", "hello");
            Assert.AreEqual("hello", exp.GetValueAsString("s1"));
        }

        [TestMethod]
        public void SetVariableAsLong_ThenGetAsLong_ReturnsValue()
        {
            var exp = Create();
            exp.SetVariableAsLong("l1", 50);
            Assert.AreEqual(50, exp.GetValueAsLong("l1"));
        }

        [TestMethod]
        public void UndefinedVariable_GetValueAsString_ReturnsVarName()
        {
            var exp = Create();
            var result = exp.GetValueAsString("undefinedVar");
            Assert.AreEqual("undefinedVar", result);
        }

        [TestMethod]
        public void UndefinedVariable_GetValueAsDouble_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("unknownVar"));
        }

        [TestMethod]
        public void GetVariableObject_AfterSetDouble_NotNull()
        {
            var exp = Create();
            exp.SetVariableAsDouble("x", 10d);
            Assert.IsNotNull(exp.GetVariableObject("x"));
        }

        [TestMethod]
        public void GetVariableObject_Undefined_IsNull()
        {
            var exp = Create();
            Assert.IsNull(exp.GetVariableObject("noSuchVar"));
        }

        [TestMethod]
        public void IsVariableDefined_AfterSet_ReturnsTrue()
        {
            var exp = Create();
            exp.SetVariableAsString("defined", "yes");
            Assert.IsTrue(exp.IsVariableDefined("defined"));
        }

        [TestMethod]
        public void IsVariableDefined_BeforeSet_ReturnsFalse()
        {
            var exp = Create();
            Assert.IsFalse(exp.IsVariableDefined("notDefined"));
        }

        [TestMethod]
        public void DefineGlobalVariable_ThenIsDefined_ReturnsTrue()
        {
            var exp = Create();
            exp.DefineGlobalVariable("gVar");
            Assert.IsTrue(exp.IsVariableDefined("gVar"));
        }

        [TestMethod]
        public void DefineLocalVariable_ThenIsDefined_ReturnsTrue()
        {
            var exp = Create();
            exp.DefineLocalVariable("lVar");
            Assert.IsTrue(exp.IsVariableDefined("lVar"));
        }

        [TestMethod]
        public void SetVariableAsDouble_Overwrite_UpdatesValue()
        {
            var exp = Create();
            exp.SetVariableAsDouble("n", 5d);
            exp.SetVariableAsDouble("n", 99d);
            Assert.AreEqual(99d, exp.GetValueAsDouble("n"));
        }

        [TestMethod]
        public void SetVariableAsString_Overwrite_UpdatesValue()
        {
            var exp = Create();
            exp.SetVariableAsString("str", "old");
            exp.SetVariableAsString("str", "new");
            Assert.AreEqual("new", exp.GetValueAsString("str"));
        }

        [TestMethod]
        public void GetValueAsLong_DoubleValue_Truncates()
        {
            var exp = Create();
            exp.SetVariableAsDouble("d", 7.9d);
            Assert.AreEqual(7, exp.GetValueAsLong("d"));
        }

        [TestMethod]
        public void GetValueAsString_DoubleVariable_ReturnsFormattedString()
        {
            var exp = Create();
            exp.SetVariableAsDouble("num", 5d);
            Assert.AreEqual("5", exp.GetValueAsString("num"));
        }
    }
}
