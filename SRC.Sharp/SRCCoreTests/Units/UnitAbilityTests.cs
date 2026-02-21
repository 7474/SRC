using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// UnitAbility クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class UnitAbilityTests
    {
        private SRC CreateSrc()
        {
            return new SRC
            {
                GUI = new MockGUI(),
            };
        }

        private UnitAbility CreateAbility(SRC src, string abilityName = "回復",
            int minRange = 1, int maxRange = 2, int enConsumption = 10, int stock = 3)
        {
            var unit = new Unit(src);
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

        // ──────────────────────────────────────────────
        // Name プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_ReturnsAbilityName()
        {
            var src = CreateSrc();
            var ability = CreateAbility(src, "回復");
            Assert.AreEqual("回復", ability.Data.Name);
        }

        [TestMethod]
        public void Name_DifferentAbility_ReturnsDifferentName()
        {
            var src = CreateSrc();
            var ability = CreateAbility(src, "修理");
            Assert.AreEqual("修理", ability.Data.Name);
        }

        // ──────────────────────────────────────────────
        // MinRange / MaxRange
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AbilityMinRange_ReturnsCorrectValue()
        {
            var src = CreateSrc();
            var ability = CreateAbility(src, minRange: 1, maxRange: 3);
            Assert.AreEqual(1, ability.AbilityMinRange());
        }

        [TestMethod]
        public void AbilityMaxRange_ReturnsCorrectValue()
        {
            var src = CreateSrc();
            var ability = CreateAbility(src, minRange: 1, maxRange: 3);
            Assert.AreEqual(3, ability.AbilityMaxRange());
        }

        [TestMethod]
        public void AbilityMinRange_SameAsMax_ReturnsMinValue()
        {
            var src = CreateSrc();
            var ability = CreateAbility(src, minRange: 2, maxRange: 2);
            Assert.AreEqual(2, ability.AbilityMinRange());
        }

        // ──────────────────────────────────────────────
        // AbilityENConsumption
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AbilityENConsumption_NoPilot_ReturnsDataValue()
        {
            var src = CreateSrc();
            var ability = CreateAbility(src, enConsumption: 20);
            // ユニットにパイロットがいない場合はデータの消費ENをそのまま返す
            Assert.AreEqual(20, ability.AbilityENConsumption());
        }

        [TestMethod]
        public void AbilityENConsumption_Zero_ReturnsZero()
        {
            var src = CreateSrc();
            var ability = CreateAbility(src, enConsumption: 0);
            Assert.AreEqual(0, ability.AbilityENConsumption());
        }

        // ──────────────────────────────────────────────
        // Stock / MaxStock
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MaxStock_ReturnsDataStockValue()
        {
            var src = CreateSrc();
            var ability = CreateAbility(src, stock: 5);
            Assert.AreEqual(5, ability.MaxStock());
        }

        [TestMethod]
        public void Stock_Initially_EqualsMaxStock()
        {
            var src = CreateSrc();
            var ability = CreateAbility(src, stock: 4);
            // 初期状態では dblStock = 0、Stock() = 0 * MaxStock = 0
            // SetStockFull で満タンにしてから確認
            ability.SetStockFull();
            Assert.AreEqual(ability.MaxStock(), ability.Stock());
        }

        [TestMethod]
        public void SetStock_SetsCorrectValue()
        {
            var src = CreateSrc();
            var ability = CreateAbility(src, stock: 6);
            ability.SetStockFull();
            ability.SetStock(3);
            Assert.AreEqual(3, ability.Stock());
        }

        [TestMethod]
        public void SetStock_Zero_MakesStockZero()
        {
            var src = CreateSrc();
            var ability = CreateAbility(src, stock: 5);
            ability.SetStockFull();
            ability.SetStock(0);
            Assert.AreEqual(0, ability.Stock());
        }

        [TestMethod]
        public void MaxStock_Zero_WhenDataStockIsZero()
        {
            var src = CreateSrc();
            var ability = CreateAbility(src, stock: 0);
            Assert.AreEqual(0, ability.MaxStock());
        }

        [TestMethod]
        public void SetStockFull_RestoresStockToMax()
        {
            var src = CreateSrc();
            var ability = CreateAbility(src, stock: 4);
            ability.SetStockFull();
            ability.SetStock(1);
            ability.SetStockFull();
            Assert.AreEqual(ability.MaxStock(), ability.Stock());
        }
    }
}
