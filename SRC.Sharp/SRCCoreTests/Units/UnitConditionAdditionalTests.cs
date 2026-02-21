using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Unit のコンディション関連メソッドの追加ユニットテスト
    /// </summary>
    [TestClass]
    public class UnitConditionAdditionalTests
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // CountCondition
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CountCondition_DefaultIsZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.AreEqual(0, unit.CountCondition());
        }

        [TestMethod]
        public void CountCondition_IncreasesAfterAdd()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("麻痺", 3);
            Assert.AreEqual(1, unit.CountCondition());
        }

        [TestMethod]
        public void CountCondition_MultipleConditions_CountsAll()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("麻痺", 3);
            unit.AddCondition("毒", 2);
            unit.AddCondition("混乱", 1);
            Assert.AreEqual(3, unit.CountCondition());
        }

        [TestMethod]
        public void CountCondition_SameConditionAddedTwice_OnlyCountsOnce()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("麻痺", 3);
            unit.AddCondition("麻痺", 5);
            Assert.AreEqual(1, unit.CountCondition());
        }

        // ──────────────────────────────────────────────
        // ConditionLifetime
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ConditionLifetime_NotPresent_ReturnsMinusOne()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.AreEqual(-1, unit.ConditionLifetime("存在しない状態"));
        }

        [TestMethod]
        public void ConditionLifetime_ReturnsSetLifetime()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("麻痺", 5);
            Assert.AreEqual(5, unit.ConditionLifetime("麻痺"));
        }

        [TestMethod]
        public void ConditionLifetime_PermanentCondition_ReturnsMinusOne()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("永続状態", -1);
            Assert.AreEqual(-1, unit.ConditionLifetime("永続状態"));
        }

        // ──────────────────────────────────────────────
        // ConditionLevel
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ConditionLevel_NotPresent_ReturnsZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.AreEqual(0d, unit.ConditionLevel("存在しない状態"));
        }

        [TestMethod]
        public void ConditionLevel_WithLevel_ReturnsCorrectLevel()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("強化", 3, clevel: 2.5);
            Assert.AreEqual(2.5, unit.ConditionLevel("強化"));
        }

        // ──────────────────────────────────────────────
        // DeleteCondition0
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DeleteCondition0_RemovesCondition()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("テスト", 3);
            unit.DeleteCondition0("テスト");
            Assert.IsFalse(unit.IsConditionSatisfied("テスト"));
        }

        [TestMethod]
        public void DeleteCondition0_DecreasesCount()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("テスト1", 3);
            unit.AddCondition("テスト2", 3);
            unit.DeleteCondition0("テスト1");
            Assert.AreEqual(1, unit.CountCondition());
        }

        // ──────────────────────────────────────────────
        // ClearCondition
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ClearCondition_RemovesAllConditions()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.AddCondition("麻痺", 3);
            unit.AddCondition("毒", 2);
            unit.ClearCondition();
            Assert.AreEqual(0, unit.CountCondition());
        }

        [TestMethod]
        public void ClearCondition_EmptyConditions_NoError()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.ClearCondition();
            Assert.AreEqual(0, unit.CountCondition());
        }
    }
}
