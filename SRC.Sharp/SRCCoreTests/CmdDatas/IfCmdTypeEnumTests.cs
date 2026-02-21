using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas.Commands;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// IfCmdType enum のユニットテスト
    /// </summary>
    [TestClass]
    public class IfCmdTypeEnumTests
    {
        // ──────────────────────────────────────────────
        // 各値の数値確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Undefined_IsZero()
        {
            Assert.AreEqual(0, (int)IfCmdType.Undefined);
        }

        [TestMethod]
        public void Then_IsOne()
        {
            Assert.AreEqual(1, (int)IfCmdType.Then);
        }

        [TestMethod]
        public void Exit_IsTwo()
        {
            Assert.AreEqual(2, (int)IfCmdType.Exit);
        }

        [TestMethod]
        public void GoTo_IsThree()
        {
            Assert.AreEqual(3, (int)IfCmdType.GoTo);
        }

        // ──────────────────────────────────────────────
        // Enum 基本操作
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HasFourValues()
        {
            Assert.AreEqual(4, System.Enum.GetValues(typeof(IfCmdType)).Length);
        }

        [TestMethod]
        public void AllValuesDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(IfCmdType), IfCmdType.Undefined));
            Assert.IsTrue(System.Enum.IsDefined(typeof(IfCmdType), IfCmdType.Then));
            Assert.IsTrue(System.Enum.IsDefined(typeof(IfCmdType), IfCmdType.Exit));
            Assert.IsTrue(System.Enum.IsDefined(typeof(IfCmdType), IfCmdType.GoTo));
        }

        [TestMethod]
        public void AllValuesAreDistinct()
        {
            var values = System.Enum.GetValues(typeof(IfCmdType));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (IfCmdType v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複した値: {v} = {(int)v}");
            }
        }
    }
}
