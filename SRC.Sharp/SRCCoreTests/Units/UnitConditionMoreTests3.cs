using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    [TestClass]
    public class UnitConditionMoreTests3
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ===== AddCondition with negative lifetime (permanent) =====

        [TestMethod]
        public void AddCondition_NegativeLifetime_IsPermanent()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("強化", -1);
            Assert.IsTrue(unit.IsConditionSatisfied("強化"));
            Assert.AreEqual(-1, unit.ConditionLifetime("強化"));
        }

        [TestMethod]
        public void AddCondition_PositiveLifetime_IsEnabled()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("麻痺", 3);
            Assert.IsTrue(unit.IsConditionSatisfied("麻痺"));
            Assert.AreEqual(3, unit.ConditionLifetime("麻痺"));
        }

        // ===== AddCondition updating existing condition lifetime to max =====

        [TestMethod]
        public void AddCondition_UpdateExistingLifetime_TakesMax()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("麻痺", 2);
            unit.AddCondition("麻痺", 5); // higher lifetime
            Assert.AreEqual(5, unit.ConditionLifetime("麻痺"));
        }

        [TestMethod]
        public void AddCondition_UpdateExistingLifetime_KeepsHigher()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("麻痺", 5);
            unit.AddCondition("麻痺", 2); // lower lifetime, max stays at 5
            Assert.AreEqual(5, unit.ConditionLifetime("麻痺"));
        }

        // ===== AddCondition with permanent overwrites finite =====

        [TestMethod]
        public void AddCondition_PermanentOverwritesFinite()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("麻痺", 3); // finite
            unit.AddCondition("麻痺", -1); // permanent
            Assert.AreEqual(-1, unit.ConditionLifetime("麻痺"));
        }

        [TestMethod]
        public void AddCondition_FiniteDoesNotOverwritePermanent()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("麻痺", -1); // permanent
            unit.AddCondition("麻痺", 3); // finite
            Assert.AreEqual(-1, unit.ConditionLifetime("麻痺"));
        }

        // ===== ConditionLevel =====

        [TestMethod]
        public void ConditionLevel_ReturnsCorrectLevel()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("強化", 3, 2.5);
            Assert.AreEqual(2.5, unit.ConditionLevel("強化"));
        }

        [TestMethod]
        public void ConditionLevel_NonExistentCondition_ReturnsZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.AreEqual(0d, unit.ConditionLevel("存在しない状態"));
        }

        // ===== CountCondition =====

        [TestMethod]
        public void CountCondition_NoConditions_ReturnsZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.AreEqual(0, unit.CountCondition());
        }

        [TestMethod]
        public void CountCondition_AfterAddingTwo_ReturnsTwo()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("麻痺", 3);
            unit.AddCondition("強化", 2);
            Assert.AreEqual(2, unit.CountCondition());
        }

        [TestMethod]
        public void CountCondition_AfterRemove_DecreasesCount()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("麻痺", 3);
            unit.AddCondition("強化", 2);
            unit.DeleteCondition0("麻痺");
            Assert.AreEqual(1, unit.CountCondition());
        }
    }
}
