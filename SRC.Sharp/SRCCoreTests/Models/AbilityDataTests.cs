using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// AbilityData クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class AbilityDataTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // フィールドの設定・読み取り
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Fields_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src)
            {
                Name = "修理装置",
                Stock = 3,
                ENConsumption = 10,
                NecessaryMorale = 100,
                MinRange = 1,
                MaxRange = 1,
                Class = "修理",
                NecessarySkill = "修理Lv1",
                NecessaryCondition = ""
            };

            Assert.AreEqual("修理装置", ad.Name);
            Assert.AreEqual(3, ad.Stock);
            Assert.AreEqual(10, ad.ENConsumption);
            Assert.AreEqual(100, ad.NecessaryMorale);
            Assert.AreEqual(1, ad.MinRange);
            Assert.AreEqual(1, ad.MaxRange);
            Assert.AreEqual("修理", ad.Class);
            Assert.AreEqual("修理Lv1", ad.NecessarySkill);
        }

        // ──────────────────────────────────────────────
        // Nickname
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Nickname_NameWithoutParentheses_ReturnsName()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src) { Name = "修理装置" };
            Assert.AreEqual("修理装置", ad.Nickname());
        }

        [TestMethod]
        public void Nickname_NameWithParentheses_RemovesParenthesisPart()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src) { Name = "修理装置(フル)" };
            Assert.AreEqual("修理装置", ad.Nickname());
        }

        // ──────────────────────────────────────────────
        // IsItem
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsItem_WithItemSkill_ReturnsTrue()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src)
            {
                Name = "消耗品アビリティ",
                NecessarySkill = "アイテム"
            };
            Assert.IsTrue(ad.IsItem());
        }

        [TestMethod]
        public void IsItem_WithoutItemSkill_ReturnsFalse()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src)
            {
                Name = "修理装置",
                NecessarySkill = "修理Lv1"
            };
            Assert.IsFalse(ad.IsItem());
        }

        [TestMethod]
        public void IsItem_EmptySkill_ReturnsFalse()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src)
            {
                Name = "修理装置",
                NecessarySkill = ""
            };
            Assert.IsFalse(ad.IsItem());
        }

        // ──────────────────────────────────────────────
        // SetEffect / Effects
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetEffect_SingleEffect_AddsToEffects()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src) { Name = "修理装置" };
            ad.SetEffect("修理");
            Assert.AreEqual(1, ad.Effects.Count);
            Assert.AreEqual("修理", ad.Effects[0].Name);
        }

        [TestMethod]
        public void SetEffect_EffectWithLevel_ParsesLevel()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src) { Name = "修理装置" };
            ad.SetEffect("修理Lv2");
            Assert.AreEqual(1, ad.Effects.Count);
            Assert.AreEqual("修理", ad.Effects[0].Name);
            Assert.AreEqual(2d, ad.Effects[0].Level);
        }

        [TestMethod]
        public void SetEffect_EmptyEffect_NoEffectsAdded()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src) { Name = "修理装置" };
            ad.SetEffect("");
            Assert.AreEqual(0, ad.Effects.Count);
        }

        [TestMethod]
        public void InitialState_EffectsEmpty()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src) { Name = "修理装置" };
            Assert.AreEqual(0, ad.Effects.Count);
        }
    }
}
