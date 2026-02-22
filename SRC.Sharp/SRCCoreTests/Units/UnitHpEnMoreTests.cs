using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Unit の HP/EN 管理に関する追加ユニットテスト
    /// </summary>
    [TestClass]
    public class UnitHpEnMoreTests
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // HP クランプテスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HP_SetToMax_EqualsMaxHP()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.HP = unit.MaxHP;
            Assert.AreEqual(unit.MaxHP, unit.HP);
        }

        [TestMethod]
        public void HP_SetToOne_IsOne()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.HP = 1;
            Assert.AreEqual(1, unit.HP);
        }

        [TestMethod]
        public void HP_SetToNegativeNumber_ClampsToZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.HP = -100;
            Assert.AreEqual(0, unit.HP);
        }

        [TestMethod]
        public void HP_SetToZero_IsZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.HP = 0;
            Assert.AreEqual(0, unit.HP);
        }

        // ──────────────────────────────────────────────
        // MaxHP
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MaxHP_DefaultUnit_IsAtLeastOne()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsTrue(unit.MaxHP >= 1);
        }

        // ──────────────────────────────────────────────
        // EN
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EN_SetPositive_IsStored()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.EN = unit.MaxEN;
            Assert.AreEqual(unit.MaxEN, unit.EN);
        }

        [TestMethod]
        public void EN_SetNegative_ClampsToZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.EN = -50;
            Assert.AreEqual(0, unit.EN);
        }

        [TestMethod]
        public void EN_SetToZero_IsZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.EN = 0;
            Assert.AreEqual(0, unit.EN);
        }

        // ──────────────────────────────────────────────
        // MaxEN
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MaxEN_DefaultUnit_IsNonNegative()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsTrue(unit.MaxEN >= 0);
        }

        // ──────────────────────────────────────────────
        // HP 超過クランプ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HP_SetAboveMax_ClampsToMax()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.HP = int.MaxValue;
            Assert.AreEqual(unit.MaxHP, unit.HP);
        }

        [TestMethod]
        public void EN_SetAboveMax_ClampsToMax()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.EN = int.MaxValue;
            Assert.AreEqual(unit.MaxEN, unit.EN);
        }
    }
}
