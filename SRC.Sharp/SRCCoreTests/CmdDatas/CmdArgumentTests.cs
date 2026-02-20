using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Expressions;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// CmdArgument クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class CmdArgumentTests
    {
        // ──────────────────────────────────────────────
        // Empty
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Empty_HasDefaultValues()
        {
            var empty = CmdArgument.Empty;
            Assert.IsNotNull(empty);
            Assert.AreEqual(ValueType.UndefinedType, empty.argType);
            Assert.AreEqual(0, empty.lngArg);
            Assert.AreEqual(0d, empty.dblArg);
            Assert.AreEqual("", empty.strArg);
        }

        // ──────────────────────────────────────────────
        // Properties
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Properties_CanBeSetAndRead()
        {
            var arg = new CmdArgument
            {
                argType = ValueType.NumericType,
                lngArg = 42,
                dblArg = 3.14,
                strArg = "hello"
            };

            Assert.AreEqual(ValueType.NumericType, arg.argType);
            Assert.AreEqual(42, arg.lngArg);
            Assert.AreEqual(3.14, arg.dblArg);
            Assert.AreEqual("hello", arg.strArg);
        }

        // ──────────────────────────────────────────────
        // ToString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_ReturnsFormattedString()
        {
            var arg = new CmdArgument
            {
                strArg = "test",
                lngArg = 5,
                dblArg = 1.5
            };
            var s = arg.ToString();
            Assert.IsTrue(s.Contains("test"));
            Assert.IsTrue(s.Contains("5"));
        }

        [TestMethod]
        public void Empty_ToString_ReturnsFormattedEmpty()
        {
            var s = CmdArgument.Empty.ToString();
            Assert.IsNotNull(s);
        }
    }
}
