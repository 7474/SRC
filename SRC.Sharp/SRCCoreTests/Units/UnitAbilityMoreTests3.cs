using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Collections.Generic;
using System.Reflection;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// UnitAbility の追加ユニットテスト（その3）
    /// </summary>
    [TestClass]
    public class UnitAbilityMoreTests3
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        private UnitAbility CreateAbility(SRC src, Unit unit, string abilityName = "回復",
            int minRange = 1, int maxRange = 2, int enConsumption = 10, int stock = 3)
        {
            var ad = new AbilityData(src)
            {
                Name = abilityName,
                MinRange = minRange,
                MaxRange = maxRange,
                ENConsumption = enConsumption,
                Stock = stock,
            };
            return new UnitAbility(src, unit, ad);
        }

        private Unit CreateUnitWithAbilities(SRC src, int count)
        {
            var unit = new Unit(src);
            var aDataField = typeof(Unit).GetField("AData", BindingFlags.NonPublic | BindingFlags.Instance);
            var aData = (List<UnitAbility>)aDataField.GetValue(unit);
            for (var i = 1; i <= count; i++)
            {
                var ad = new AbilityData(src) { Name = $"アビリティ{i}", MinRange = 1, MaxRange = 2, ENConsumption = 10, Stock = 3 };
                var a = new UnitAbility(src, unit, ad);
                aData.Add(a);
            }
            return unit;
        }

        // ──────────────────────────────────────────────
        // Name
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_ReturnsAbilityDataName()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var a = CreateAbility(src, unit, "修理");
            Assert.AreEqual("修理", a.Data.Name);
        }

        // ──────────────────────────────────────────────
        // AbilityNo
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AbilityNo_FirstAbility_ReturnsOne()
        {
            var src = CreateSrc();
            var unit = CreateUnitWithAbilities(src, 3);
            Assert.AreEqual(1, unit.Abilities[0].AbilityNo());
        }

        [TestMethod]
        public void AbilityNo_SecondAbility_ReturnsTwo()
        {
            var src = CreateSrc();
            var unit = CreateUnitWithAbilities(src, 3);
            Assert.AreEqual(2, unit.Abilities[1].AbilityNo());
        }

        [TestMethod]
        public void AbilityNo_ThirdAbility_ReturnsThree()
        {
            var src = CreateSrc();
            var unit = CreateUnitWithAbilities(src, 3);
            Assert.AreEqual(3, unit.Abilities[2].AbilityNo());
        }

        // ──────────────────────────────────────────────
        // AbilityMinRange / AbilityMaxRange
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AbilityMinRange_ReturnsCorrectValue()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var a = CreateAbility(src, unit, minRange: 1, maxRange: 3);
            Assert.AreEqual(1, a.AbilityMinRange());
        }

        [TestMethod]
        public void AbilityMaxRange_ReturnsCorrectValue()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var a = CreateAbility(src, unit, minRange: 1, maxRange: 4);
            Assert.AreEqual(4, a.AbilityMaxRange());
        }

        [TestMethod]
        public void AbilityMinRange_EqualToMax_ReturnsMinValue()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var a = CreateAbility(src, unit, minRange: 2, maxRange: 2);
            Assert.AreEqual(2, a.AbilityMinRange());
        }

        [TestMethod]
        public void AbilityMaxRange_ZeroRange_ReturnsZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var a = CreateAbility(src, unit, minRange: 0, maxRange: 0);
            Assert.AreEqual(0, a.AbilityMaxRange());
        }

        // ──────────────────────────────────────────────
        // Unit reference
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Unit_ReferencesCorrectUnit()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var a = CreateAbility(src, unit);
            Assert.AreSame(unit, a.Unit);
        }

        // ──────────────────────────────────────────────
        // Data reference
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Data_IsNotNull()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var a = CreateAbility(src, unit, "修理");
            Assert.IsNotNull(a.Data);
        }

        [TestMethod]
        public void Data_Stock_ReturnsCorrectValue()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var a = CreateAbility(src, unit, stock: 5);
            Assert.AreEqual(5, a.Data.Stock);
        }

        [TestMethod]
        public void Data_ENConsumption_ReturnsCorrectValue()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var a = CreateAbility(src, unit, enConsumption: 20);
            Assert.AreEqual(20, a.Data.ENConsumption);
        }
    }
}
