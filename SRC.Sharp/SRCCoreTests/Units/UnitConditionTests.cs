using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    [TestClass]
    public class UnitConditionTests
    {
        private SRC CreateSrc()
        {
            return new SRC
            {
                GUI = new MockGUI(),
            };
        }

        [TestMethod]
        public void ConditionData_ReturnsEmptyString_WhenConditionNotFound()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            Assert.AreEqual("", unit.ConditionData("存在しない状態"));
        }

        [TestMethod]
        public void ConditionData_ReturnsEmptyString_WhenConditionHasNoData()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            unit.AddCondition("麻痺", 3);

            Assert.AreEqual("", unit.ConditionData("麻痺"));
        }

        [TestMethod]
        public void ConditionData_ReturnsData_WhenConditionHasData()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            unit.AddCondition("援護強化", 3, cdata: "援護攻撃");

            Assert.AreEqual("援護攻撃", unit.ConditionData("援護強化"));
        }

        [TestMethod]
        public void ConditionData_ReturnsLatestData_WhenConditionUpdated()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            unit.AddCondition("テスト状態", 3, cdata: "初期値");
            // Adding the same condition updates its data
            unit.AddCondition("テスト状態", 5, cdata: "更新値");

            Assert.AreEqual("更新値", unit.ConditionData("テスト状態"));
        }

        [TestMethod]
        public void IsConditionSatisfied_ReturnsFalse_WhenConditionNotPresent()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            Assert.IsFalse(unit.IsConditionSatisfied("麻痺"));
        }

        [TestMethod]
        public void IsConditionSatisfied_ReturnsTrue_WhenConditionPresent()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            unit.AddCondition("麻痺", 3);

            Assert.IsTrue(unit.IsConditionSatisfied("麻痺"));
        }

        [TestMethod]
        public void CountCondition_ReturnsZero_WhenNoConditions()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            Assert.AreEqual(0, unit.CountCondition());
        }

        [TestMethod]
        public void CountCondition_ReturnsCorrectCount_WhenConditionsAdded()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            unit.AddCondition("麻痺", 3);
            unit.AddCondition("睡眠", 2);

            Assert.AreEqual(2, unit.CountCondition());
        }
    }
}
