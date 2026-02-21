using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// UnitData クラスの追加テスト
    /// </summary>
    [TestClass]
    public class UnitDataAdditionalTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // 基本フィールド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var ud = new UnitData(CreateSrc()) { Name = "ゲッターロボ" };
            Assert.AreEqual("ゲッターロボ", ud.Name);
        }

        [TestMethod]
        public void HP_CanBeSetAndRead()
        {
            var ud = new UnitData(CreateSrc()) { HP = 5000 };
            Assert.AreEqual(5000, ud.HP);
        }

        [TestMethod]
        public void EN_CanBeSetAndRead()
        {
            var ud = new UnitData(CreateSrc()) { EN = 200 };
            Assert.AreEqual(200, ud.EN);
        }

        [TestMethod]
        public void Mobility_CanBeSetAndRead()
        {
            var ud = new UnitData(CreateSrc()) { Mobility = 80 };
            Assert.AreEqual(80, ud.Mobility);
        }

        [TestMethod]
        public void Armor_CanBeSetAndRead()
        {
            var ud = new UnitData(CreateSrc()) { Armor = 1500 };
            Assert.AreEqual(1500, ud.Armor);
        }

        [TestMethod]
        public void Adaption_CanBeSetAndRead()
        {
            var ud = new UnitData(CreateSrc()) { Adaption = "AABA" };
            Assert.AreEqual("AABA", ud.Adaption);
        }

        [TestMethod]
        public void Class_CanBeSetAndRead()
        {
            var ud = new UnitData(CreateSrc()) { Class = "超合金" };
            Assert.AreEqual("超合金", ud.Class);
        }

        // ──────────────────────────────────────────────
        // IsFeatureAvailable
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsFeatureAvailable_WhenNotAdded_ReturnsFalse()
        {
            var ud = new UnitData(CreateSrc());
            Assert.IsFalse(ud.IsFeatureAvailable("シールド"));
        }

        [TestMethod]
        public void AddFeature_ThenIsAvailable_ReturnsTrue()
        {
            var ud = new UnitData(CreateSrc()) { Name = "テスト機" };
            ud.AddFeature("シールド");
            Assert.IsTrue(ud.IsFeatureAvailable("シールド"));
        }

        [TestMethod]
        public void CountFeature_Initially_IsZero()
        {
            var ud = new UnitData(CreateSrc());
            Assert.AreEqual(0, ud.CountFeature());
        }

        [TestMethod]
        public void CountFeature_AfterTwoFeatures_IsTwo()
        {
            var ud = new UnitData(CreateSrc()) { Name = "テスト機" };
            ud.AddFeature("シールド");
            ud.AddFeature("バリア");
            Assert.AreEqual(2, ud.CountFeature());
        }

        // ──────────────────────────────────────────────
        // 複数インスタンスの独立性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TwoUnits_AreIndependent()
        {
            var src = CreateSrc();
            var ud1 = new UnitData(src) { Name = "ユニットA", HP = 3000 };
            var ud2 = new UnitData(src) { Name = "ユニットB", HP = 5000 };
            Assert.AreNotEqual(ud1.Name, ud2.Name);
            Assert.AreNotEqual(ud1.HP, ud2.HP);
        }
    }
}
