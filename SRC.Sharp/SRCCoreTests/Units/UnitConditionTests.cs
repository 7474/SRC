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

        // ──────────────────────────────────────────────
        // 追加テスト
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
            unit.ClearCondition();

            Assert.AreEqual(0, unit.CountCondition());
        }

        [TestMethod]
        public void ConditionLifetime_ReturnsCorrectLifetime()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            unit.AddCondition("麻痺", 5);

            Assert.AreEqual(5, unit.ConditionLifetime("麻痺"));
        }

        [TestMethod]
        public void ConditionLifetime_ReturnsMinusOne_WhenNotPresent()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            Assert.AreEqual(-1, unit.ConditionLifetime("存在しない状態"));
        }

        [TestMethod]
        public void ConditionLevel_ReturnsLevel_WhenConditionHasLevel()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            unit.AddCondition("強化", 3, clevel: 2.0);

            Assert.AreEqual(2.0, unit.ConditionLevel("強化"));
        }

        [TestMethod]
        public void ConditionLevel_ReturnsZero_WhenConditionNotPresent()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            Assert.AreEqual(0d, unit.ConditionLevel("存在しない状態"));
        }

        [TestMethod]
        public void AddCondition_SameConditionTwice_UpdatesLifetimeToMax()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            unit.AddCondition("麻痺", 2);
            unit.AddCondition("麻痺", 5);

            // 残りターンは Max(2,5) = 5
            Assert.AreEqual(5, unit.ConditionLifetime("麻痺"));
            // カウント変わらず
            Assert.AreEqual(1, unit.CountCondition());
        }

        [TestMethod]
        public void AddCondition_PermanentCondition_ReturnsMinusOneLifetime()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            unit.AddCondition("永続状態", -1);

            Assert.AreEqual(-1, unit.ConditionLifetime("永続状態"));
        }
    }
}
