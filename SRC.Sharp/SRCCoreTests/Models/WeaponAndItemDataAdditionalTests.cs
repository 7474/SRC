using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// WeaponData クラスの追加テスト
    /// </summary>
    [TestClass]
    public class WeaponDataAdditionalTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var wd = new WeaponData(CreateSrc()) { Name = "メガバスター" };
            Assert.AreEqual("メガバスター", wd.Name);
        }

        [TestMethod]
        public void Power_CanBeSetAndRead()
        {
            var wd = new WeaponData(CreateSrc()) { Power = 3000 };
            Assert.AreEqual(3000, wd.Power);
        }

        [TestMethod]
        public void ENConsumption_CanBeSetAndRead()
        {
            var wd = new WeaponData(CreateSrc()) { ENConsumption = 50 };
            Assert.AreEqual(50, wd.ENConsumption);
        }

        [TestMethod]
        public void MinRange_CanBeSetAndRead()
        {
            var wd = new WeaponData(CreateSrc()) { MinRange = 1 };
            Assert.AreEqual(1, wd.MinRange);
        }

        [TestMethod]
        public void MaxRange_CanBeSetAndRead()
        {
            var wd = new WeaponData(CreateSrc()) { MaxRange = 5 };
            Assert.AreEqual(5, wd.MaxRange);
        }

        [TestMethod]
        public void Adaption_CanBeSetAndRead()
        {
            var wd = new WeaponData(CreateSrc()) { Adaption = "AAAA" };
            Assert.AreEqual("AAAA", wd.Adaption);
        }

        [TestMethod]
        public void Class_CanBeSetAndRead()
        {
            var wd = new WeaponData(CreateSrc()) { Class = "格闘" };
            Assert.AreEqual("格闘", wd.Class);
        }

        [TestMethod]
        public void NecessaryCondition_CanBeSetAndRead()
        {
            var wd = new WeaponData(CreateSrc()) { NecessaryCondition = "ハイパーモード" };
            Assert.AreEqual("ハイパーモード", wd.NecessaryCondition);
        }

        [TestMethod]
        public void NecessarySkill_CanBeSetAndRead()
        {
            var wd = new WeaponData(CreateSrc()) { NecessarySkill = "射撃" };
            Assert.AreEqual("射撃", wd.NecessarySkill);
        }

        [TestMethod]
        public void Bullet_CanBeSetAndRead()
        {
            var wd = new WeaponData(CreateSrc()) { Bullet = 12 };
            Assert.AreEqual(12, wd.Bullet);
        }

        [TestMethod]
        public void Precision_CanBeSetAndRead()
        {
            var wd = new WeaponData(CreateSrc()) { Precision = 85 };
            Assert.AreEqual(85, wd.Precision);
        }

        [TestMethod]
        public void Critical_CanBeSetAndRead()
        {
            var wd = new WeaponData(CreateSrc()) { Critical = 10 };
            Assert.AreEqual(10, wd.Critical);
        }

        [TestMethod]
        public void NecessaryMorale_CanBeSetAndRead()
        {
            var wd = new WeaponData(CreateSrc()) { NecessaryMorale = 120 };
            Assert.AreEqual(120, wd.NecessaryMorale);
        }

        [TestMethod]
        public void IsItem_WithItemSkill_ReturnsTrue()
        {
            var wd = new WeaponData(CreateSrc()) { NecessarySkill = "アイテム" };
            Assert.IsTrue(wd.IsItem());
        }

        [TestMethod]
        public void IsItem_WithNoItemSkill_ReturnsFalse()
        {
            var wd = new WeaponData(CreateSrc()) { NecessarySkill = "射撃" };
            Assert.IsFalse(wd.IsItem());
        }

        [TestMethod]
        public void IsItem_WithNullSkill_ReturnsFalse()
        {
            var wd = new WeaponData(CreateSrc()) { NecessarySkill = null };
            Assert.IsFalse(wd.IsItem());
        }

        [TestMethod]
        public void TwoWeapons_AreIndependent()
        {
            var src = CreateSrc();
            var wd1 = new WeaponData(src) { Name = "ビームサーベル", Power = 2500 };
            var wd2 = new WeaponData(src) { Name = "バスターライフル", Power = 5000 };
            Assert.AreNotEqual(wd1.Name, wd2.Name);
            Assert.AreNotEqual(wd1.Power, wd2.Power);
        }
    }

    /// <summary>
    /// ItemData クラスの追加テスト
    /// </summary>
    [TestClass]
    public class ItemDataAdditionalTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        [TestMethod]
        public void HP_CanBeSetAndRead()
        {
            var item = new ItemData(CreateSrc()) { HP = 200 };
            Assert.AreEqual(200, item.HP);
        }

        [TestMethod]
        public void EN_CanBeSetAndRead()
        {
            var item = new ItemData(CreateSrc()) { EN = 50 };
            Assert.AreEqual(50, item.EN);
        }

        [TestMethod]
        public void Armor_CanBeSetAndRead()
        {
            var item = new ItemData(CreateSrc()) { Armor = 100 };
            Assert.AreEqual(100, item.Armor);
        }

        [TestMethod]
        public void Mobility_CanBeSetAndRead()
        {
            var item = new ItemData(CreateSrc()) { Mobility = 5 };
            Assert.AreEqual(5, item.Mobility);
        }

        [TestMethod]
        public void Speed_CanBeSetAndRead()
        {
            var item = new ItemData(CreateSrc()) { Speed = 3 };
            Assert.AreEqual(3, item.Speed);
        }

        [TestMethod]
        public void Comment_CanBeSetAndRead()
        {
            var item = new ItemData(CreateSrc()) { Comment = "テストコメント" };
            Assert.AreEqual("テストコメント", item.Comment);
        }

        [TestMethod]
        public void Part_CanBeSetAndRead()
        {
            var item = new ItemData(CreateSrc()) { Part = "本体" };
            Assert.AreEqual("本体", item.Part);
        }

        [TestMethod]
        public void TwoItems_AreIndependent()
        {
            var src = CreateSrc();
            var item1 = new ItemData(src) { Name = "アイテムA", HP = 100 };
            var item2 = new ItemData(src) { Name = "アイテムB", HP = 200 };
            Assert.AreNotEqual(item1.Name, item2.Name);
            Assert.AreNotEqual(item1.HP, item2.HP);
        }
    }
}
