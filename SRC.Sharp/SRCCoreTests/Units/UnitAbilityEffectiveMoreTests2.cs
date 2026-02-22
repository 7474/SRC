using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    [TestClass]
    public class UnitAbilityEffectiveMoreTests2
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private (Unit unit, UnitAbility ability) CreateAbilityWithRange(SRC src, int minRange, int maxRange)
        {
            var unit = new Unit(src);
            unit.Party = "味方";

            var ad = new AbilityData(src);
            ad.MinRange = minRange;
            ad.MaxRange = maxRange;
            ad.SetEffect("回復Lv1");

            var ability = new UnitAbility(src, unit, ad);
            return (unit, ability);
        }

        [TestMethod]
        public void AbilityMinRange_ReturnsDataMinRange()
        {
            var src = CreateSrc();
            var (_, ability) = CreateAbilityWithRange(src, 1, 5);
            Assert.AreEqual(1, ability.AbilityMinRange());
        }

        [TestMethod]
        public void AbilityMaxRange_ReturnsDataMaxRange()
        {
            var src = CreateSrc();
            var (_, ability) = CreateAbilityWithRange(src, 1, 5);
            Assert.AreEqual(5, ability.AbilityMaxRange());
        }

        [TestMethod]
        public void AbilityMinRange_ZeroMinRange_ReturnsZero()
        {
            var src = CreateSrc();
            var (_, ability) = CreateAbilityWithRange(src, 0, 3);
            Assert.AreEqual(0, ability.AbilityMinRange());
        }

        [TestMethod]
        public void AbilityMaxRange_ZeroMaxRange_ReturnsZero()
        {
            var src = CreateSrc();
            var (_, ability) = CreateAbilityWithRange(src, 0, 0);
            Assert.AreEqual(0, ability.AbilityMaxRange());
        }

        [TestMethod]
        public void AbilityMinRange_MinEqualsMax_ReturnsSameValue()
        {
            var src = CreateSrc();
            var (_, ability) = CreateAbilityWithRange(src, 3, 3);
            Assert.AreEqual(3, ability.AbilityMinRange());
            Assert.AreEqual(3, ability.AbilityMaxRange());
        }

        [TestMethod]
        public void AbilityMaxRange_LargeValue_ReturnsCorrectly()
        {
            var src = CreateSrc();
            var (_, ability) = CreateAbilityWithRange(src, 1, 99);
            Assert.AreEqual(99, ability.AbilityMaxRange());
        }

        [TestMethod]
        public void IsAbilityEffective_ENResupply_WhenENFull_ReturnsFalse()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Party = "味方";

            var ad = new AbilityData(src);
            ad.SetEffect("補給Lv1");
            var ability = new UnitAbility(src, unit, ad);

            var target = new Unit(src);
            target.Party = "味方";
            target.EN = target.MaxEN;

            Assert.IsFalse(ability.IsAbilityEffective(target));
        }

        [TestMethod]
        public void IsAbilityEffective_ENResupply_WhenENNotFull_ReturnsTrue()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Party = "味方";

            var ad = new AbilityData(src);
            ad.SetEffect("補給Lv1");
            var ability = new UnitAbility(src, unit, ad);

            var target = new Unit(src);
            target.Party = "味方";
            // Set MaxEN > 0 via reflection, keep EN = 0 so EN < MaxEN
            var intMaxENField = typeof(Unit).GetField("intMaxEN", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            intMaxENField.SetValue(target, 100);
            target.EN = 0;

            Assert.IsTrue(ability.IsAbilityEffective(target));
        }

        [TestMethod]
        public void AbilityNo_ReturnsZeroBasedIndexPlusOne()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            var ad = new AbilityData(src);
            ad.SetEffect("回復Lv1");
            var ability = new UnitAbility(src, unit, ad);

            // Use reflection to add to private AData list
            var aDataField = typeof(Unit).GetField("AData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var aData = (System.Collections.Generic.List<UnitAbility>)aDataField.GetValue(unit);
            aData.Add(ability);

            Assert.AreEqual(1, ability.AbilityNo());
        }

        [TestMethod]
        public void AbilityMaxRange_DefaultAbilityData_ReturnsZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var ad = new AbilityData(src);
            var ability = new UnitAbility(src, unit, ad);
            Assert.AreEqual(0, ability.AbilityMaxRange());
        }
    }
}
