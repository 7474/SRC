using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    [TestClass]
    public class ItemDataMoreTests3
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var item = new ItemData(src) { Name = "テストアイテム" };
            Assert.AreEqual("テストアイテム", item.Name);
        }

        [TestMethod]
        public void Class_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var item = new ItemData(src) { Class = "防具" };
            Assert.AreEqual("防具", item.Class);
        }

        [TestMethod]
        public void Part_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var item = new ItemData(src) { Part = "腕" };
            Assert.AreEqual("腕", item.Part);
        }

        [TestMethod]
        public void HP_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var item = new ItemData(src) { HP = 100 };
            Assert.AreEqual(100, item.HP);
        }

        [TestMethod]
        public void EN_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var item = new ItemData(src) { EN = 50 };
            Assert.AreEqual(50, item.EN);
        }

        [TestMethod]
        public void Armor_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var item = new ItemData(src) { Armor = 200 };
            Assert.AreEqual(200, item.Armor);
        }

        [TestMethod]
        public void Mobility_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var item = new ItemData(src) { Mobility = 30 };
            Assert.AreEqual(30, item.Mobility);
        }

        [TestMethod]
        public void Speed_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var item = new ItemData(src) { Speed = 5 };
            Assert.AreEqual(5, item.Speed);
        }

        [TestMethod]
        public void Comment_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var item = new ItemData(src) { Comment = "説明文" };
            Assert.AreEqual("説明文", item.Comment);
        }

        [TestMethod]
        public void Features_InitiallyEmpty()
        {
            var src = CreateSrc();
            var item = new ItemData(src);
            Assert.IsNotNull(item.Features);
            Assert.AreEqual(0, item.Features.Count);
        }

        [TestMethod]
        public void Weapons_InitiallyEmpty()
        {
            var src = CreateSrc();
            var item = new ItemData(src);
            Assert.IsNotNull(item.Weapons);
            Assert.AreEqual(0, item.Weapons.Count);
        }

        [TestMethod]
        public void Abilities_InitiallyEmpty()
        {
            var src = CreateSrc();
            var item = new ItemData(src);
            Assert.IsNotNull(item.Abilities);
            Assert.AreEqual(0, item.Abilities.Count);
        }

        [TestMethod]
        public void Raw_DefaultIsEmptyString()
        {
            var src = CreateSrc();
            var item = new ItemData(src);
            Assert.AreEqual("", item.Raw);
        }

        [TestMethod]
        public void TwoInstances_AreIndependent()
        {
            var src = CreateSrc();
            var item1 = new ItemData(src) { Name = "剣", HP = 100 };
            var item2 = new ItemData(src) { Name = "盾", HP = 50 };
            Assert.AreNotEqual(item1.Name, item2.Name);
            Assert.AreNotEqual(item1.HP, item2.HP);
        }
    }
}
