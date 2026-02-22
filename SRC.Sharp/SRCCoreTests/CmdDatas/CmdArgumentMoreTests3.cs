using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Expressions;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// CmdArgument クラスの追加ユニットテスト（その3）
    /// </summary>
    [TestClass]
    public class CmdArgumentMoreTests3
    {
        // ──────────────────────────────────────────────
        // Empty singleton の不変性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Empty_StrArg_IsEmptyString()
        {
            Assert.AreEqual("", CmdArgument.Empty.strArg);
        }

        [TestMethod]
        public void Empty_LngArg_IsZero()
        {
            Assert.AreEqual(0, CmdArgument.Empty.lngArg);
        }

        [TestMethod]
        public void Empty_DblArg_IsZero()
        {
            Assert.AreEqual(0d, CmdArgument.Empty.dblArg);
        }

        [TestMethod]
        public void Empty_ArgType_IsUndefined()
        {
            Assert.AreEqual(ValueType.UndefinedType, CmdArgument.Empty.argType);
        }

        [TestMethod]
        public void Empty_IsSameSingleton()
        {
            Assert.AreSame(CmdArgument.Empty, CmdArgument.Empty);
        }

        // ──────────────────────────────────────────────
        // NumericType でのプロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NumericType_LngArg_CanBePositive()
        {
            var arg = new CmdArgument { argType = ValueType.NumericType, lngArg = 999, dblArg = 999d, strArg = "999" };
            Assert.AreEqual(999, arg.lngArg);
        }

        [TestMethod]
        public void NumericType_LngArg_CanBeNegative()
        {
            var arg = new CmdArgument { argType = ValueType.NumericType, lngArg = -50, dblArg = -50d, strArg = "-50" };
            Assert.AreEqual(-50, arg.lngArg);
        }

        [TestMethod]
        public void NumericType_DblArg_CanBeDecimal()
        {
            var arg = new CmdArgument { argType = ValueType.NumericType, lngArg = 0, dblArg = 3.14, strArg = "3.14" };
            Assert.AreEqual(3.14, arg.dblArg, 1e-9);
        }

        // ──────────────────────────────────────────────
        // StringType でのプロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StringType_StrArg_CanBeMultibyte()
        {
            var arg = new CmdArgument { argType = ValueType.StringType, strArg = "テスト文字列", lngArg = 0, dblArg = 0d };
            Assert.AreEqual("テスト文字列", arg.strArg);
        }

        [TestMethod]
        public void StringType_StrArg_CanBeEmptyString()
        {
            var arg = new CmdArgument { argType = ValueType.StringType, strArg = "", lngArg = 0, dblArg = 0d };
            Assert.AreEqual("", arg.strArg);
        }

        // ──────────────────────────────────────────────
        // ToString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_ContainsStrArg()
        {
            var arg = new CmdArgument { strArg = "hello", lngArg = 1, dblArg = 1d };
            Assert.IsTrue(arg.ToString().Contains("hello"));
        }

        [TestMethod]
        public void ToString_ContainsLngArg()
        {
            var arg = new CmdArgument { strArg = "x", lngArg = 42, dblArg = 42d };
            Assert.IsTrue(arg.ToString().Contains("42"));
        }

        [TestMethod]
        public void ToString_EmptyArg_DoesNotThrow()
        {
            var arg = CmdArgument.Empty;
            var s = arg.ToString();
            Assert.IsNotNull(s);
        }

        // ──────────────────────────────────────────────
        // 上書き更新
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Properties_CanBeOverwritten()
        {
            var arg = new CmdArgument { lngArg = 1, dblArg = 1d, strArg = "first", argType = ValueType.NumericType };
            arg.lngArg = 2;
            arg.dblArg = 2d;
            arg.strArg = "second";
            arg.argType = ValueType.StringType;

            Assert.AreEqual(2, arg.lngArg);
            Assert.AreEqual(2d, arg.dblArg);
            Assert.AreEqual("second", arg.strArg);
            Assert.AreEqual(ValueType.StringType, arg.argType);
        }
    }
}
