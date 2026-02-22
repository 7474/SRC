using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// AbilityData の追加ユニットテスト（その3）
    /// </summary>
    [TestClass]
    public class AbilityDataMoreTests3
    {
        private SRC CreateSRC() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // フィールド初期値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Class_DefaultIsEmptyString()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src);
            Assert.AreEqual("", ad.Class);
        }

        [TestMethod]
        public void NecessarySkill_DefaultIsEmptyString()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src);
            Assert.AreEqual("", ad.NecessarySkill);
        }

        [TestMethod]
        public void NecessaryCondition_DefaultIsEmptyString()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src);
            Assert.AreEqual("", ad.NecessaryCondition);
        }

        // ──────────────────────────────────────────────
        // フィールドの読み書き
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src) { Name = "特殊アビリティ" };
            Assert.AreEqual("特殊アビリティ", ad.Name);
        }

        [TestMethod]
        public void Stock_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src) { Stock = 7 };
            Assert.AreEqual(7, ad.Stock);
        }

        [TestMethod]
        public void ENConsumption_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src) { ENConsumption = 25 };
            Assert.AreEqual(25, ad.ENConsumption);
        }

        [TestMethod]
        public void NecessaryMorale_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src) { NecessaryMorale = 130 };
            Assert.AreEqual(130, ad.NecessaryMorale);
        }

        [TestMethod]
        public void MinRange_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src) { MinRange = 2 };
            Assert.AreEqual(2, ad.MinRange);
        }

        [TestMethod]
        public void MaxRange_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src) { MaxRange = 5 };
            Assert.AreEqual(5, ad.MaxRange);
        }

        // ──────────────────────────────────────────────
        // Nickname
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Nickname_SimpleName_ReturnsSameName()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src) { Name = "回復" };
            Assert.AreEqual("回復", ad.Nickname());
        }

        [TestMethod]
        public void Nickname_NameWithParentheses_TrimsParenthesesPart()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src) { Name = "修理(フル)" };
            Assert.AreEqual("修理", ad.Nickname());
        }

        [TestMethod]
        public void Nickname_NameWithMultipleParentheses_TrimsFromFirst()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src) { Name = "強化(A)(B)" };
            Assert.AreEqual("強化", ad.Nickname());
        }

        // ──────────────────────────────────────────────
        // IsItem
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsItem_WithItemSkill_ReturnsTrue()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src) { NecessarySkill = "アイテム" };
            Assert.IsTrue(ad.IsItem());
        }

        [TestMethod]
        public void IsItem_WithoutItemSkill_ReturnsFalse()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src) { NecessarySkill = "修理Lv1" };
            Assert.IsFalse(ad.IsItem());
        }

        // ──────────────────────────────────────────────
        // Effects
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Effects_DefaultIsEmpty()
        {
            var src = CreateSRC();
            var ad = new AbilityData(src);
            Assert.IsNotNull(ad.Effects);
            Assert.AreEqual(0, ad.Effects.Count);
        }
    }
}
