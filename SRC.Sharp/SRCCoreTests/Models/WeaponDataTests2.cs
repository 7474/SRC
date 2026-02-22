using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    [TestClass]
    public class WeaponDataTests2
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ===== IsItem() =====

        [TestMethod]
        public void IsItem_ReturnsTrue_WhenNecessarySkillContainsItem()
        {
            var src = CreateSrc();
            var wd = new WeaponData(src) { Name = "テスト武器", NecessarySkill = "アイテム" };
            Assert.IsTrue(wd.IsItem());
        }

        [TestMethod]
        public void IsItem_ReturnsFalse_WhenNecessarySkillIsEmpty()
        {
            var src = CreateSrc();
            var wd = new WeaponData(src) { Name = "テスト武器", NecessarySkill = "" };
            Assert.IsFalse(wd.IsItem());
        }

        [TestMethod]
        public void IsItem_ReturnsFalse_WhenNecessarySkillIsNull()
        {
            var src = CreateSrc();
            var wd = new WeaponData(src) { Name = "テスト武器", NecessarySkill = null };
            Assert.IsFalse(wd.IsItem());
        }

        [TestMethod]
        public void IsItem_ReturnsFalse_WhenNecessarySkillIsOtherSkill()
        {
            var src = CreateSrc();
            var wd = new WeaponData(src) { Name = "テスト武器", NecessarySkill = "格闘" };
            Assert.IsFalse(wd.IsItem());
        }

        [TestMethod]
        public void IsItem_ReturnsTrue_WhenNecessarySkillContainsItemAmongOthers()
        {
            var src = CreateSrc();
            var wd = new WeaponData(src) { Name = "テスト武器", NecessarySkill = "格闘 アイテム" };
            Assert.IsTrue(wd.IsItem());
        }

        // ===== Field assignments =====

        [TestMethod]
        public void WeaponData_Name_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var wd = new WeaponData(src) { Name = "バルカン" };
            Assert.AreEqual("バルカン", wd.Name);
        }

        [TestMethod]
        public void WeaponData_Power_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var wd = new WeaponData(src) { Power = 3000 };
            Assert.AreEqual(3000, wd.Power);
        }

        [TestMethod]
        public void WeaponData_Range_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var wd = new WeaponData(src) { MinRange = 1, MaxRange = 5 };
            Assert.AreEqual(1, wd.MinRange);
            Assert.AreEqual(5, wd.MaxRange);
        }

        [TestMethod]
        public void WeaponData_Bullet_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var wd = new WeaponData(src) { Bullet = 6 };
            Assert.AreEqual(6, wd.Bullet);
        }

        [TestMethod]
        public void WeaponData_ENConsumption_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var wd = new WeaponData(src) { ENConsumption = 20 };
            Assert.AreEqual(20, wd.ENConsumption);
        }

        [TestMethod]
        public void WeaponData_NecessaryMorale_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var wd = new WeaponData(src) { NecessaryMorale = 110 };
            Assert.AreEqual(110, wd.NecessaryMorale);
        }
    }
}
