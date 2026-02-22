using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;
using System;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// LabelType 列挙型のユニットテスト
    /// </summary>
    [TestClass]
    public class LabelTypeTests
    {
        // ──────────────────────────────────────────────
        // 基本値確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NormalLabel_IsZero()
        {
            Assert.AreEqual(0, (int)LabelType.NormalLabel);
        }

        [TestMethod]
        public void MemberCount_Is27()
        {
            var values = Enum.GetValues(typeof(LabelType));
            Assert.AreEqual(27, values.Length);
        }

        // ──────────────────────────────────────────────
        // IsDefined テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsDefined_NormalLabel_ReturnsTrue()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(LabelType), LabelType.NormalLabel));
        }

        [TestMethod]
        public void IsDefined_PrologueEventLabel_ReturnsTrue()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(LabelType), LabelType.PrologueEventLabel));
        }

        [TestMethod]
        public void IsDefined_EffectEventLabel_ReturnsTrue()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(LabelType), LabelType.EffectEventLabel));
        }

        // ──────────────────────────────────────────────
        // 全ての値が異なる整数値を持つ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AllValues_HaveUniqueIntegers()
        {
            var values = Enum.GetValues(typeof(LabelType));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (LabelType v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複した値が見つかりました: {v} = {(int)v}");
            }
        }

        // ──────────────────────────────────────────────
        // Parse テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Parse_NormalLabel_Succeeds()
        {
            Assert.AreEqual(LabelType.NormalLabel, Enum.Parse<LabelType>("NormalLabel"));
        }

        [TestMethod]
        public void Parse_InvalidString_Throws()
        {
            Assert.ThrowsException<ArgumentException>(() => Enum.Parse<LabelType>("InvalidLabelXyz"));
        }

        // ──────────────────────────────────────────────
        // 相互比較
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NormalLabel_NotEqual_PrologueEventLabel()
        {
            Assert.AreNotEqual(LabelType.NormalLabel, LabelType.PrologueEventLabel);
        }

        [TestMethod]
        public void StartEventLabel_NotEqual_EpilogueEventLabel()
        {
            Assert.AreNotEqual(LabelType.StartEventLabel, LabelType.EpilogueEventLabel);
        }
    }
}
