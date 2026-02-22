using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Collections.Generic;
using System.Reflection;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// UnitWeapon の追加ユニットテスト（その3）
    /// </summary>
    [TestClass]
    public class UnitWeaponMoreTests3
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        private UnitWeapon CreateWeapon(SRC src, Unit unit, string weaponName = "バルカン",
            int minRange = 1, int maxRange = 3, int bullet = 6, string wclass = "実")
        {
            var wd = new WeaponData(src)
            {
                Name = weaponName,
                MinRange = minRange,
                MaxRange = maxRange,
                Bullet = bullet,
                Precision = 80,
                Critical = 10,
                Class = wclass,
            };
            return new UnitWeapon(src, unit, wd);
        }

        private Unit CreateUnitWithWeapons(SRC src, int count)
        {
            var unit = new Unit(src);
            var wDataField = typeof(Unit).GetField("WData", BindingFlags.NonPublic | BindingFlags.Instance);
            var wData = (List<UnitWeapon>)wDataField.GetValue(unit);
            for (var i = 1; i <= count; i++)
            {
                var wd = new WeaponData(src) { Name = $"武器{i}", Bullet = 5, Class = "実", MinRange = 1, MaxRange = 3 };
                var w = new UnitWeapon(src, unit, wd);
                wData.Add(w);
            }
            return unit;
        }

        // ──────────────────────────────────────────────
        // Name
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_ReturnsWeaponDataName()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var w = CreateWeapon(src, unit, "ミサイル");
            Assert.AreEqual("ミサイル", w.Name);
        }

        // ──────────────────────────────────────────────
        // WeaponNo / IsLastWeapon
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WeaponNo_FirstWeapon_ReturnsOne()
        {
            var src = CreateSrc();
            var unit = CreateUnitWithWeapons(src, 3);
            Assert.AreEqual(1, unit.Weapons[0].WeaponNo());
        }

        [TestMethod]
        public void WeaponNo_SecondWeapon_ReturnsTwo()
        {
            var src = CreateSrc();
            var unit = CreateUnitWithWeapons(src, 3);
            Assert.AreEqual(2, unit.Weapons[1].WeaponNo());
        }

        [TestMethod]
        public void WeaponNo_ThirdWeapon_ReturnsThree()
        {
            var src = CreateSrc();
            var unit = CreateUnitWithWeapons(src, 3);
            Assert.AreEqual(3, unit.Weapons[2].WeaponNo());
        }

        [TestMethod]
        public void IsLastWeapon_LastWeapon_ReturnsTrue()
        {
            var src = CreateSrc();
            var unit = CreateUnitWithWeapons(src, 3);
            Assert.IsTrue(unit.Weapons[2].IsLastWeapon());
        }

        [TestMethod]
        public void IsLastWeapon_FirstWeapon_ReturnsFalse()
        {
            var src = CreateSrc();
            var unit = CreateUnitWithWeapons(src, 3);
            Assert.IsFalse(unit.Weapons[0].IsLastWeapon());
        }

        [TestMethod]
        public void IsLastWeapon_SingleWeapon_ReturnsTrue()
        {
            var src = CreateSrc();
            var unit = CreateUnitWithWeapons(src, 1);
            Assert.IsTrue(unit.Weapons[0].IsLastWeapon());
        }

        // ──────────────────────────────────────────────
        // Bullet management
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Bullet_Initial_EqualsMaxBullet()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var w = CreateWeapon(src, unit, bullet: 6);
            Assert.AreEqual(6, w.Bullet());
            Assert.AreEqual(6, w.MaxBullet());
        }

        [TestMethod]
        public void SetBullet_ReducesBullet()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var w = CreateWeapon(src, unit, bullet: 6);
            w.SetBullet(3);
            Assert.AreEqual(3, w.Bullet());
        }

        [TestMethod]
        public void SetBullet_Zero_ResultsInZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var w = CreateWeapon(src, unit, bullet: 6);
            w.SetBullet(0);
            Assert.AreEqual(0, w.Bullet());
        }

        [TestMethod]
        public void SetBulletFull_RestoresToMax()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var w = CreateWeapon(src, unit, bullet: 6);
            w.SetBullet(2);
            w.SetBulletFull();
            Assert.AreEqual(6, w.Bullet());
        }

        [TestMethod]
        public void MaxBullet_ReturnsWeaponDataBulletByDefault()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var w = CreateWeapon(src, unit, bullet: 10);
            Assert.AreEqual(10, w.MaxBullet());
        }

        // ──────────────────────────────────────────────
        // WeaponClass
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WeaponClass_ReturnsInitialClass()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var w = CreateWeapon(src, unit, wclass: "魔");
            Assert.AreEqual("魔", w.WeaponClass());
        }

        [TestMethod]
        public void SetWeaponClass_ChangesClass()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var w = CreateWeapon(src, unit, wclass: "実");
            w.SetWeaponClass("熱");
            Assert.AreEqual("熱", w.WeaponClass());
        }
    }
}
