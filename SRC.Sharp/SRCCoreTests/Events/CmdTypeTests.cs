using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;
using System;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// CmdType 列挙型のユニットテスト
    /// </summary>
    [TestClass]
    public class CmdTypeTests
    {
        // ──────────────────────────────────────────────
        // 基本値確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NullCmd_IsZero()
        {
            Assert.AreEqual(0, (int)CmdType.NullCmd);
        }

        [TestMethod]
        public void NopCmd_IsDefined()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(CmdType), CmdType.NopCmd));
        }

        [TestMethod]
        public void PlayFlashCmd_IsDefined()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(CmdType), CmdType.PlayFlashCmd));
        }

        [TestMethod]
        public void MemberCount_Is190()
        {
            var values = Enum.GetValues(typeof(CmdType));
            Assert.AreEqual(190, values.Length);
        }

        // ──────────────────────────────────────────────
        // IsDefined テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsDefined_NullCmd_ReturnsTrue()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(CmdType), CmdType.NullCmd));
        }

        [TestMethod]
        public void IsDefined_SetCmd_ReturnsTrue()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(CmdType), CmdType.SetCmd));
        }

        [TestMethod]
        public void IsDefined_IfCmd_ReturnsTrue()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(CmdType), CmdType.IfCmd));
        }

        // ──────────────────────────────────────────────
        // 全ての値が異なる整数値を持つ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AllValues_HaveUniqueIntegers()
        {
            var values = Enum.GetValues(typeof(CmdType));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (CmdType v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複した値が見つかりました: {v} = {(int)v}");
            }
        }

        // ──────────────────────────────────────────────
        // Parse テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Parse_NullCmd_Succeeds()
        {
            Assert.AreEqual(CmdType.NullCmd, Enum.Parse<CmdType>("NullCmd"));
        }

        [TestMethod]
        public void Parse_InvalidString_Throws()
        {
            Assert.Throws<ArgumentException>(() => Enum.Parse<CmdType>("InvalidCmdXyz"));
        }

        // ──────────────────────────────────────────────
        // 相互比較
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NullCmd_NotEqual_NopCmd()
        {
            Assert.AreNotEqual(CmdType.NullCmd, CmdType.NopCmd);
        }
    }
}
