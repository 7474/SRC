using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// Condition クラスと Unit のコンディション管理の追加テスト（その3）
    /// </summary>
    [TestClass]
    public class ConditionMoreTests3
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // Condition プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Condition_Name_NullByDefault()
        {
            var c = new Condition();
            Assert.IsNull(c.Name);
        }

        [TestMethod]
        public void Condition_Lifetime_ZeroByDefault()
        {
            var c = new Condition();
            Assert.AreEqual(0, c.Lifetime);
        }

        [TestMethod]
        public void Condition_Level_ZeroByDefault()
        {
            var c = new Condition();
            Assert.AreEqual(0d, c.Level);
        }

        [TestMethod]
        public void Condition_StrData_NullByDefault()
        {
            var c = new Condition();
            Assert.IsNull(c.StrData);
        }

        [TestMethod]
        public void Condition_IsEnable_FalseWhenLifetimeZero()
        {
            var c = new Condition { Lifetime = 0 };
            Assert.IsFalse(c.IsEnable);
        }

        [TestMethod]
        public void Condition_IsEnable_TrueWhenLifetimePositive()
        {
            var c = new Condition { Lifetime = 1 };
            Assert.IsTrue(c.IsEnable);
        }

        [TestMethod]
        public void Condition_IsEnable_TrueWhenLifetimeNegative()
        {
            var c = new Condition { Lifetime = -1 };
            Assert.IsTrue(c.IsEnable);
        }

        // ──────────────────────────────────────────────
        // Unit.AddCondition / IsConditionSatisfied
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddCondition_NewCondition_IsConditionSatisfiedReturnsTrue()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("麻痺", 3);
            Assert.IsTrue(unit.IsConditionSatisfied("麻痺"));
        }

        [TestMethod]
        public void AddCondition_SameConditionTwice_UpdatesLifetime()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("睡眠", 2);
            unit.AddCondition("睡眠", 5);
            // Lifetime should be max(2,5)=5
            Assert.AreEqual(5, unit.ConditionLifetime("睡眠"));
        }

        [TestMethod]
        public void AddCondition_SetsData()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("援護強化", 3, cdata: "援護攻撃");
            Assert.AreEqual("援護攻撃", unit.ConditionData("援護強化"));
        }

        [TestMethod]
        public void AddCondition_SetsLevel()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("強化", 5, clevel: 2.0);
            Assert.AreEqual(2.0, unit.ConditionLevel("強化"));
        }

        // ──────────────────────────────────────────────
        // Unit.DeleteCondition0
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DeleteCondition0_RemovesCondition()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("毒", 5);
            unit.DeleteCondition0("毒");
            Assert.IsFalse(unit.IsConditionSatisfied("毒"));
        }

        // ──────────────────────────────────────────────
        // Unit.ClearCondition
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ClearCondition_RemovesAllConditions()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("麻痺", 3);
            unit.AddCondition("睡眠", 2);
            unit.ClearCondition();
            Assert.AreEqual(0, unit.CountCondition());
        }

        // ──────────────────────────────────────────────
        // Unit.CountCondition
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CountCondition_NoConditions_ReturnsZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.AreEqual(0, unit.CountCondition());
        }

        [TestMethod]
        public void CountCondition_TwoConditions_ReturnsTwo()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("麻痺", 3);
            unit.AddCondition("睡眠", 2);
            Assert.AreEqual(2, unit.CountCondition());
        }

        // ──────────────────────────────────────────────
        // Unit.ConditionLifetime
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ConditionLifetime_ExistingCondition_ReturnsLifetime()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("毒", 4);
            Assert.AreEqual(4, unit.ConditionLifetime("毒"));
        }

        [TestMethod]
        public void ConditionLifetime_MissingCondition_ReturnsMinusOne()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.AreEqual(-1, unit.ConditionLifetime("存在しない"));
        }

        // ──────────────────────────────────────────────
        // Unit.IsConditionSatisfied
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsConditionSatisfied_MissingCondition_ReturnsFalse()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsFalse(unit.IsConditionSatisfied("なし"));
        }
    }
}
