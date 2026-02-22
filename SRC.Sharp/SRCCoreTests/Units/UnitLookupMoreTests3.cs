using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Unit のルックアップ追加テスト (UnitLookupMoreTests3)
    /// </summary>
    [TestClass]
    public class UnitLookupMoreTests3
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // IsConditionSatisfied
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsConditionSatisfied_InitiallyFalse_ForParalysis()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsFalse(unit.IsConditionSatisfied("麻痺"));
        }

        [TestMethod]
        public void IsConditionSatisfied_TrueAfterAdd()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("石化", 2);
            Assert.IsTrue(unit.IsConditionSatisfied("石化"));
        }

        [TestMethod]
        public void IsConditionSatisfied_FalseAfterDelete()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("石化", 2);
            unit.DeleteCondition0("石化");
            Assert.IsFalse(unit.IsConditionSatisfied("石化"));
        }

        [TestMethod]
        public void IsConditionSatisfied_TrueForActionDisabledAfterAdd()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("行動不能", 5);
            Assert.IsTrue(unit.IsConditionSatisfied("行動不能"));
        }

        // ──────────────────────────────────────────────
        // IsDisabled
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsDisabled_ReturnsFalse_WhenNoDisableVariable()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsFalse(unit.IsDisabled("存在しないユニット"));
        }

        [TestMethod]
        public void IsDisabled_ReturnsTrue_AfterDefinition()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            src.Expression.DefineGlobalVariable("Disable(テスト)");
            Assert.IsTrue(unit.IsDisabled("テスト"));
        }

        // ──────────────────────────────────────────────
        // IsNecessarySkillSatisfied
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsNecessarySkillSatisfied_ReturnsTrue_ForEmptyString()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsTrue(unit.IsNecessarySkillSatisfied(""));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_ReturnsTrue_ItemSkill()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsTrue(unit.IsNecessarySkillSatisfied2("アイテム", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_ReturnsFalse_AutoCounterSkill()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsFalse(unit.IsNecessarySkillSatisfied2("自動反撃", null));
        }

        // ──────────────────────────────────────────────
        // IsAvailable の追加シナリオ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsAvailable_ReturnsTrue_NoConditions()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsTrue(unit.IsAvailable());
        }

        [TestMethod]
        public void IsAvailable_WithStatus他形態AndSleepCondition_ReturnsTrue()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Status = "他形態";
            unit.AddCondition("睡眠", 3);
            // 睡眠(sleep) alone should not disable 他形態 units
            Assert.IsTrue(unit.IsAvailable());
        }

        [TestMethod]
        public void IsAbleToEnter_ReturnsTrue_ForNewUnit()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsTrue(unit.IsAbleToEnter(1, 1));
        }
    }
}
