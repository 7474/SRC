using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Unit の特殊状態 (Condition) に関する追加ユニットテスト
    /// </summary>
    [TestClass]
    public class UnitConditionAdditionalMoreTests
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // AddCondition / CountCondition
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddCondition_InitiallyZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.AreEqual(0, unit.CountCondition());
        }

        [TestMethod]
        public void AddCondition_SingleCondition_CountIsOne()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("麻痺", 3);
            Assert.AreEqual(1, unit.CountCondition());
        }

        [TestMethod]
        public void AddCondition_TwoConditions_CountIsTwo()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("麻痺", 3);
            unit.AddCondition("睡眠", 2);
            Assert.AreEqual(2, unit.CountCondition());
        }

        [TestMethod]
        public void AddCondition_SameConditionTwice_CountRemainsOne()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("強化", 3);
            unit.AddCondition("強化", 5);
            Assert.AreEqual(1, unit.CountCondition());
        }

        // ──────────────────────────────────────────────
        // IsConditionSatisfied
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsConditionSatisfied_NoConditions_ReturnsFalse()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsFalse(unit.IsConditionSatisfied("麻痺"));
        }

        [TestMethod]
        public void IsConditionSatisfied_MatchingCondition_ReturnsTrue()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("麻痺", 3);
            Assert.IsTrue(unit.IsConditionSatisfied("麻痺"));
        }

        [TestMethod]
        public void IsConditionSatisfied_DifferentCondition_ReturnsFalse()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("麻痺", 3);
            Assert.IsFalse(unit.IsConditionSatisfied("睡眠"));
        }

        // ──────────────────────────────────────────────
        // ConditionLevel
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ConditionLevel_MatchingCondition_ReturnsLevel()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("強化", 3, clevel: 2.5);
            Assert.AreEqual(2.5, unit.ConditionLevel("強化"), 1e-10);
        }

        [TestMethod]
        public void ConditionLevel_NoCondition_ReturnsZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.AreEqual(0d, unit.ConditionLevel("未設定"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // DeleteCondition0 / ClearCondition
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DeleteCondition0_RemovesCondition()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("麻痺", 3);
            unit.DeleteCondition0("麻痺");
            Assert.AreEqual(0, unit.CountCondition());
        }

        [TestMethod]
        public void ClearCondition_RemovesAllConditions()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("麻痺", 3);
            unit.AddCondition("睡眠", 2);
            unit.AddCondition("強化", 5);
            unit.ClearCondition();
            Assert.AreEqual(0, unit.CountCondition());
        }

        [TestMethod]
        public void ClearCondition_EmptyConditions_StillZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.ClearCondition();
            Assert.AreEqual(0, unit.CountCondition());
        }

        // ──────────────────────────────────────────────
        // ConditionData
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ConditionData_WithData_ReturnsData()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("援護強化", 3, cdata: "援護攻撃");
            Assert.AreEqual("援護攻撃", unit.ConditionData("援護強化"));
        }

        [TestMethod]
        public void ConditionData_NoCondition_ReturnsEmpty()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.AreEqual("", unit.ConditionData("存在しない"));
        }
    }
}
