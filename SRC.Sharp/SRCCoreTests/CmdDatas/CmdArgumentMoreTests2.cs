using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Expressions;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// CmdArgument クラスの追加エッジケーステスト（その2）
    /// </summary>
    [TestClass]
    public class CmdArgumentEdgeCaseTests
    {
        // ──────────────────────────────────────────────
        // Empty singleton
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Empty_IsSameInstanceEachTime()
        {
            var e1 = CmdArgument.Empty;
            var e2 = CmdArgument.Empty;
            Assert.AreSame(e1, e2);
        }

        [TestMethod]
        public void Empty_ArgType_IsUndefined()
        {
            Assert.AreEqual(ValueType.UndefinedType, CmdArgument.Empty.argType);
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
        public void Empty_StrArg_IsEmptyString()
        {
            Assert.AreEqual("", CmdArgument.Empty.strArg);
        }

        // ──────────────────────────────────────────────
        // 各 ValueType を設定したケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArgType_NumericType_CanBeSetAndRead()
        {
            var arg = new CmdArgument { argType = ValueType.NumericType };
            Assert.AreEqual(ValueType.NumericType, arg.argType);
        }

        [TestMethod]
        public void ArgType_StringType_CanBeSetAndRead()
        {
            var arg = new CmdArgument { argType = ValueType.StringType };
            Assert.AreEqual(ValueType.StringType, arg.argType);
        }

        [TestMethod]
        public void ArgType_UndefinedType_CanBeSetAndRead()
        {
            var arg = new CmdArgument { argType = ValueType.UndefinedType };
            Assert.AreEqual(ValueType.UndefinedType, arg.argType);
        }

        // ──────────────────────────────────────────────
        // 数値フィールド境界値テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LngArg_MaxValue_CanBeSet()
        {
            var arg = new CmdArgument { lngArg = int.MaxValue };
            Assert.AreEqual(int.MaxValue, arg.lngArg);
        }

        [TestMethod]
        public void LngArg_MinValue_CanBeSet()
        {
            var arg = new CmdArgument { lngArg = int.MinValue };
            Assert.AreEqual(int.MinValue, arg.lngArg);
        }

        [TestMethod]
        public void LngArg_NegativeValue_CanBeSet()
        {
            var arg = new CmdArgument { lngArg = -100 };
            Assert.AreEqual(-100, arg.lngArg);
        }

        [TestMethod]
        public void DblArg_NegativeValue_CanBeSet()
        {
            var arg = new CmdArgument { dblArg = -3.14 };
            Assert.AreEqual(-3.14, arg.dblArg);
        }

        [TestMethod]
        public void DblArg_LargeValue_CanBeSet()
        {
            var arg = new CmdArgument { dblArg = 1e15 };
            Assert.AreEqual(1e15, arg.dblArg);
        }

        // ──────────────────────────────────────────────
        // StrArg フィールド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrArg_JapaneseString_CanBeSetAndRead()
        {
            var arg = new CmdArgument { strArg = "ユニット名" };
            Assert.AreEqual("ユニット名", arg.strArg);
        }

        [TestMethod]
        public void StrArg_EmptyString_CanBeSetAndRead()
        {
            var arg = new CmdArgument { strArg = "" };
            Assert.AreEqual("", arg.strArg);
        }

        [TestMethod]
        public void StrArg_Null_CanBeSetAndRead()
        {
            var arg = new CmdArgument { strArg = null };
            Assert.IsNull(arg.strArg);
        }

        // ──────────────────────────────────────────────
        // ToString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_ContainsStrArg()
        {
            var arg = new CmdArgument { strArg = "attack", lngArg = 10, dblArg = 1.5 };
            var str = arg.ToString();
            Assert.IsTrue(str.Contains("attack"), $"Expected 'attack' in '{str}'");
        }

        [TestMethod]
        public void ToString_ContainsLngArg()
        {
            var arg = new CmdArgument { strArg = "x", lngArg = 999, dblArg = 0d };
            var str = arg.ToString();
            Assert.IsTrue(str.Contains("999"), $"Expected '999' in '{str}'");
        }

        [TestMethod]
        public void ToString_ContainsDblArg()
        {
            var arg = new CmdArgument { strArg = "y", lngArg = 0, dblArg = 2.5 };
            var str = arg.ToString();
            Assert.IsTrue(str.Contains("2.5"), $"Expected '2.5' in '{str}'");
        }

        [TestMethod]
        public void ToString_Empty_ReturnsNonNull()
        {
            Assert.IsNotNull(CmdArgument.Empty.ToString());
        }

        [TestMethod]
        public void ToString_Format_ContainsBothParentheses()
        {
            var arg = new CmdArgument { strArg = "abc", lngArg = 1, dblArg = 1d };
            var str = arg.ToString();
            // フォーマットは "abc(1L)(1d)" のような形式
            Assert.IsTrue(str.Contains("("), $"Expected '(' in '{str}'");
        }
    }
}
