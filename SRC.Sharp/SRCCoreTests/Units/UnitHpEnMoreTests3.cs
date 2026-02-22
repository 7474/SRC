using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    [TestClass]
    public class UnitHpEnMoreTests3
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ===== HP edge cases =====

        [TestMethod]
        public void HP_StaysAtZero_AfterNegativeSet()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.HP = 0;
            unit.HP = -5;
            Assert.AreEqual(0, unit.HP);
        }

        [TestMethod]
        public void HP_SetExactlyToMaxHP_EqualsMaxHP()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.HP = unit.MaxHP;
            Assert.AreEqual(unit.MaxHP, unit.HP);
        }

        [TestMethod]
        public void HP_SetAboveMaxHP_ClampsToMaxHP()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.HP = unit.MaxHP + 100;
            Assert.AreEqual(unit.MaxHP, unit.HP);
        }

        [TestMethod]
        public void MaxHP_ReturnsAtLeastOne()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsTrue(unit.MaxHP >= 1);
        }

        // ===== EN edge cases =====

        [TestMethod]
        public void EN_ClampedAtMaxEN_WhenSetHigher()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.EN = unit.MaxEN + 100;
            Assert.AreEqual(unit.MaxEN, unit.EN);
        }

        [TestMethod]
        public void EN_SetExactlyToMaxEN_EqualsMaxEN()
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
            unit.EN = -100;
            Assert.AreEqual(0, unit.EN);
        }

        [TestMethod]
        public void MaxEN_ReturnsAtLeastZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsTrue(unit.MaxEN >= 0);
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
        public void EN_SetToZero_IsZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.EN = 0;
            Assert.AreEqual(0, unit.EN);
        }
    }
}
