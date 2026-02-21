using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// UnitWeapon クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class UnitWeaponTests
    {
        private SRC CreateSrc()
        {
            return new SRC
            {
                GUI = new MockGUI(),
            };
        }

        private UnitWeapon CreateWeapon(SRC src, string weaponName = "バルカン",
            int minRange = 1, int maxRange = 3, int bullet = 6,
            int precision = 80, int critical = 10, string wclass = "実")
        {
            var unit = new Unit(src);
            var wd = new WeaponData(src)
            {
                Name = weaponName,
                MinRange = minRange,
                MaxRange = maxRange,
                Bullet = bullet,
                Precision = precision,
                Critical = critical,
                Class = wclass,
            };
            return new UnitWeapon(src, unit, wd);
        }

        // ──────────────────────────────────────────────
        // Name プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_ReturnsWeaponName()
        {
            var src = CreateSrc();
            var weapon = CreateWeapon(src, "バルカン");
            Assert.AreEqual("バルカン", weapon.Name);
        }

        [TestMethod]
        public void Name_DifferentWeapon_ReturnsDifferentName()
        {
            var src = CreateSrc();
            var weapon = CreateWeapon(src, "ミサイル");
            Assert.AreEqual("ミサイル", weapon.Name);
        }

        // ──────────────────────────────────────────────
        // WeaponClass メソッド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WeaponClass_ReturnsClassFromWeaponData()
        {
            var src = CreateSrc();
            var weapon = CreateWeapon(src, wclass: "実");
            Assert.AreEqual("実", weapon.WeaponClass());
        }

        [TestMethod]
        public void WeaponClass_AfterSetWeaponClass_ReturnsNewClass()
        {
            var src = CreateSrc();
            var weapon = CreateWeapon(src, wclass: "実");
            weapon.SetWeaponClass("魔");
            Assert.AreEqual("魔", weapon.WeaponClass());
        }

        [TestMethod]
        public void IsWeaponClassifiedAs_ReturnsTrue_WhenMatches()
        {
            var src = CreateSrc();
            var weapon = CreateWeapon(src, wclass: "実");
            Assert.IsTrue(weapon.IsWeaponClassifiedAs("実"));
        }

        [TestMethod]
        public void IsWeaponClassifiedAs_ReturnsFalse_WhenNoMatch()
        {
            var src = CreateSrc();
            var weapon = CreateWeapon(src, wclass: "実");
            Assert.IsFalse(weapon.IsWeaponClassifiedAs("魔"));
        }

        // ──────────────────────────────────────────────
        // MinRange / MaxRange
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WeaponMinRange_ReturnsCorrectValue()
        {
            var src = CreateSrc();
            var weapon = CreateWeapon(src, minRange: 2, maxRange: 5);
            Assert.AreEqual(2, weapon.WeaponMinRange());
        }

        [TestMethod]
        public void WeaponMaxRange_ReturnsCorrectValue()
        {
            var src = CreateSrc();
            var weapon = CreateWeapon(src, minRange: 1, maxRange: 4);
            Assert.AreEqual(4, weapon.WeaponMaxRange());
        }

        [TestMethod]
        public void WeaponMinRange_One_ReturnsOne()
        {
            var src = CreateSrc();
            var weapon = CreateWeapon(src, minRange: 1, maxRange: 1);
            Assert.AreEqual(1, weapon.WeaponMinRange());
        }

        // ──────────────────────────────────────────────
        // Bullet / MaxBullet / SetBulletFull
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MaxBullet_ReturnsWeaponDataBullet()
        {
            var src = CreateSrc();
            var weapon = CreateWeapon(src, bullet: 6);
            Assert.AreEqual(6, weapon.MaxBullet());
        }

        [TestMethod]
        public void Bullet_InitiallyEqualsMaxBullet()
        {
            var src = CreateSrc();
            var weapon = CreateWeapon(src, bullet: 6);
            Assert.AreEqual(weapon.MaxBullet(), weapon.Bullet());
        }

        [TestMethod]
        public void SetBulletFull_RestoresBulletToMax()
        {
            var src = CreateSrc();
            var weapon = CreateWeapon(src, bullet: 6);
            weapon.SetBullet(2);
            weapon.SetBulletFull();
            Assert.AreEqual(weapon.MaxBullet(), weapon.Bullet());
        }

        [TestMethod]
        public void SetBullet_ReducesBullet()
        {
            var src = CreateSrc();
            var weapon = CreateWeapon(src, bullet: 6);
            weapon.SetBullet(3);
            Assert.AreEqual(3, weapon.Bullet());
        }

        [TestMethod]
        public void SetBullet_Zero_MakesBulletZero()
        {
            var src = CreateSrc();
            var weapon = CreateWeapon(src, bullet: 6);
            weapon.SetBullet(0);
            Assert.AreEqual(0, weapon.Bullet());
        }

        [TestMethod]
        public void MaxBullet_Zero_WhenDataBulletIsZero()
        {
            var src = CreateSrc();
            var weapon = CreateWeapon(src, bullet: 0);
            Assert.AreEqual(0, weapon.MaxBullet());
        }

        // ──────────────────────────────────────────────
        // Critical
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WeaponCritical_ReturnsCorrectValue()
        {
            var src = CreateSrc();
            var weapon = CreateWeapon(src, critical: 15);
            Assert.AreEqual(15, weapon.WeaponCritical());
        }

        [TestMethod]
        public void WeaponCritical_Zero_WhenDataCriticalIsZero()
        {
            var src = CreateSrc();
            var weapon = CreateWeapon(src, critical: 0);
            Assert.AreEqual(0, weapon.WeaponCritical());
        }

        [TestMethod]
        public void SetCriticalCorrection_AdjustsCritical()
        {
            var src = CreateSrc();
            var weapon = CreateWeapon(src, critical: 10);
            weapon.SetCriticalCorrection(5);
            Assert.AreEqual(15, weapon.WeaponCritical());
        }

        // ──────────────────────────────────────────────
        // Precision (命中)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WeaponPrecision_ReturnsCorrectValue()
        {
            var src = CreateSrc();
            var weapon = CreateWeapon(src, precision: 80);
            Assert.AreEqual(80, weapon.WeaponPrecision());
        }

        [TestMethod]
        public void SetPrecisionCorrection_AdjustsPrecision()
        {
            var src = CreateSrc();
            var weapon = CreateWeapon(src, precision: 70);
            weapon.SetPrecisionCorrection(10);
            Assert.AreEqual(80, weapon.WeaponPrecision());
        }

        [TestMethod]
        public void WeaponPrecision_Zero_WhenDataPrecisionIsZero()
        {
            var src = CreateSrc();
            var weapon = CreateWeapon(src, precision: 0);
            Assert.AreEqual(0, weapon.WeaponPrecision());
        }
    }
}
