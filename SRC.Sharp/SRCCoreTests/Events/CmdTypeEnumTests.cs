using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;
using System;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// CmdType 列挙型のユニットテスト
    /// </summary>
    [TestClass]
    public class CmdTypeEnumTests
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
        public void NopCmd_IsOne()
        {
            Assert.AreEqual(1, (int)CmdType.NopCmd);
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

        [TestMethod]
        public void IsDefined_CallCmd_ReturnsTrue()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(CmdType), CmdType.CallCmd));
        }

        [TestMethod]
        public void IsDefined_ReturnCmd_ReturnsTrue()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(CmdType), CmdType.ReturnCmd));
        }

        // ──────────────────────────────────────────────
        // 相互比較
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NullCmd_NotEqual_NopCmd()
        {
            Assert.AreNotEqual(CmdType.NullCmd, CmdType.NopCmd);
        }

        [TestMethod]
        public void SetCmd_NotEqual_IfCmd()
        {
            Assert.AreNotEqual(CmdType.SetCmd, CmdType.IfCmd);
        }

        // ──────────────────────────────────────────────
        // GetValues テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AllValues_HaveUniqueIntegers()
        {
            var values = Enum.GetValues(typeof(CmdType));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (CmdType v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複: {v}={v:D}");
            }
        }

        [TestMethod]
        public void GetValues_HasManyValues()
        {
            var values = Enum.GetValues(typeof(CmdType));
            Assert.IsTrue(values.Length > 50, $"定義数が少ない: {values.Length}");
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
        public void Parse_SetCmd_Succeeds()
        {
            Assert.AreEqual(CmdType.SetCmd, Enum.Parse<CmdType>("SetCmd"));
        }

        [TestMethod]
        public void Parse_InvalidString_Throws()
        {
            Assert.ThrowsException<ArgumentException>(() => Enum.Parse<CmdType>("InvalidCmdXyz"));
        }
    }
}
