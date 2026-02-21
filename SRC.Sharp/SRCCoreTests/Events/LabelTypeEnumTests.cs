using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// LabelType enum のユニットテスト
    /// </summary>
    [TestClass]
    public class LabelTypeEnumTests
    {
        // ──────────────────────────────────────────────
        // 各値の IsDefined 確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsDefined_NormalLabel()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(LabelType), LabelType.NormalLabel));
        }

        [TestMethod]
        public void IsDefined_PrologueEventLabel()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(LabelType), LabelType.PrologueEventLabel));
        }

        [TestMethod]
        public void IsDefined_StartEventLabel()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(LabelType), LabelType.StartEventLabel));
        }

        [TestMethod]
        public void IsDefined_EpilogueEventLabel()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(LabelType), LabelType.EpilogueEventLabel));
        }

        [TestMethod]
        public void IsDefined_TurnEventLabel()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(LabelType), LabelType.TurnEventLabel));
        }

        [TestMethod]
        public void IsDefined_AttackEventLabel()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(LabelType), LabelType.AttackEventLabel));
        }

        [TestMethod]
        public void IsDefined_DestructionEventLabel()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(LabelType), LabelType.DestructionEventLabel));
        }

        [TestMethod]
        public void IsDefined_LevelUpEventLabel()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(LabelType), LabelType.LevelUpEventLabel));
        }

        // ──────────────────────────────────────────────
        // 全ての値が異なる整数値を持つ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AllValuesAreDistinct()
        {
            var values = System.Enum.GetValues(typeof(LabelType));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (LabelType v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複した値が見つかりました: {v} = {(int)v}");
            }
        }

        // ──────────────────────────────────────────────
        // 文字列からのパース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CanBeParsedFromString_NormalLabel()
        {
            Assert.AreEqual(LabelType.NormalLabel, System.Enum.Parse<LabelType>("NormalLabel"));
        }

        [TestMethod]
        public void CanBeParsedFromString_StartEventLabel()
        {
            Assert.AreEqual(LabelType.StartEventLabel, System.Enum.Parse<LabelType>("StartEventLabel"));
        }

        [TestMethod]
        public void CanBeParsedFromString_AttackEventLabel()
        {
            Assert.AreEqual(LabelType.AttackEventLabel, System.Enum.Parse<LabelType>("AttackEventLabel"));
        }

        // ──────────────────────────────────────────────
        // 各ラベルは互いに等しくない
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NormalLabel_NotEqualTo_PrologueEventLabel()
        {
            Assert.AreNotEqual(LabelType.NormalLabel, LabelType.PrologueEventLabel);
        }

        [TestMethod]
        public void StartEventLabel_NotEqualTo_EpilogueEventLabel()
        {
            Assert.AreNotEqual(LabelType.StartEventLabel, LabelType.EpilogueEventLabel);
        }

        [TestMethod]
        public void AttackEventLabel_NotEqualTo_DestructionEventLabel()
        {
            Assert.AreNotEqual(LabelType.AttackEventLabel, LabelType.DestructionEventLabel);
        }
    }
}
