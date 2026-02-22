using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Expressions;

namespace SRCCore.CmdDatas.Tests
{
    [TestClass]
    public class CmdArgumentTests
    {
        [TestMethod]
        public void Empty_HasCorrectDefaults()
        {
            var empty = CmdArgument.Empty;
            Assert.AreEqual(ValueType.UndefinedType, empty.argType);
            Assert.AreEqual(0, empty.lngArg);
            Assert.AreEqual(0d, empty.dblArg);
            Assert.AreEqual("", empty.strArg);
        }

        [TestMethod]
        public void Empty_IsSingleton()
        {
            var empty1 = CmdArgument.Empty;
            var empty2 = CmdArgument.Empty;
            Assert.AreSame(empty1, empty2);
        }

        [TestMethod]
        public void DefaultConstructor_InitializesProperties()
        {
            var arg = new CmdArgument();
            Assert.AreEqual(0, arg.lngArg);
            Assert.AreEqual(0d, arg.dblArg);
            Assert.IsNull(arg.strArg);
            Assert.AreEqual(ValueType.UndefinedType, arg.argType);
        }

        [TestMethod]
        public void LngArg_CanBeSetAndRead()
        {
            var arg = new CmdArgument();
            arg.lngArg = 42;
            Assert.AreEqual(42, arg.lngArg);
        }

        [TestMethod]
        public void LngArg_NegativeValue()
        {
            var arg = new CmdArgument();
            arg.lngArg = -100;
            Assert.AreEqual(-100, arg.lngArg);
        }

        [TestMethod]
        public void DblArg_CanBeSetAndRead()
        {
            var arg = new CmdArgument();
            arg.dblArg = 3.14;
            Assert.AreEqual(3.14, arg.dblArg);
        }

        [TestMethod]
        public void DblArg_NegativeValue()
        {
            var arg = new CmdArgument();
            arg.dblArg = -2.5;
            Assert.AreEqual(-2.5, arg.dblArg);
        }

        [TestMethod]
        public void StrArg_CanBeSetAndRead()
        {
            var arg = new CmdArgument();
            arg.strArg = "test";
            Assert.AreEqual("test", arg.strArg);
        }

        [TestMethod]
        public void StrArg_EmptyString()
        {
            var arg = new CmdArgument();
            arg.strArg = "";
            Assert.AreEqual("", arg.strArg);
        }

        [TestMethod]
        public void ArgType_StringType()
        {
            var arg = new CmdArgument();
            arg.argType = ValueType.StringType;
            Assert.AreEqual(ValueType.StringType, arg.argType);
        }

        [TestMethod]
        public void ArgType_NumericType()
        {
            var arg = new CmdArgument();
            arg.argType = ValueType.NumericType;
            Assert.AreEqual(ValueType.NumericType, arg.argType);
        }

        [TestMethod]
        public void ArgType_UndefinedType()
        {
            var arg = new CmdArgument();
            arg.argType = ValueType.UndefinedType;
            Assert.AreEqual(ValueType.UndefinedType, arg.argType);
        }

        [TestMethod]
        public void ToString_WithValues()
        {
            var arg = new CmdArgument();
            arg.strArg = "hello";
            arg.lngArg = 10;
            arg.dblArg = 1.5;
            Assert.AreEqual("hello(10L)(1.5d)", arg.ToString());
        }

        [TestMethod]
        public void ToString_WithDefaults()
        {
            var arg = new CmdArgument();
            arg.strArg = "";
            arg.lngArg = 0;
            arg.dblArg = 0d;
            Assert.AreEqual("(0L)(0d)", arg.ToString());
        }

        [TestMethod]
        public void ToString_WithNegativeValues()
        {
            var arg = new CmdArgument();
            arg.strArg = "neg";
            arg.lngArg = -5;
            arg.dblArg = -2.5;
            Assert.AreEqual("neg(-5L)(-2.5d)", arg.ToString());
        }

        [TestMethod]
        public void ToString_EmptyInstance()
        {
            Assert.AreEqual("(0L)(0d)", CmdArgument.Empty.ToString());
        }
    }
}
