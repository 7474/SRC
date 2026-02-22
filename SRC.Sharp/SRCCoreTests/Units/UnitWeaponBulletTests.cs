using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// UnitWeapon の弾数管理に関するユニットテスト
    /// </summary>
    [TestClass]
    public class UnitWeaponBulletTests
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private UnitWeapon CreateWeapon(SRC src, string name = "バルカン",
            int bullet = 6, string wclass = "実", int minRange = 1, int maxRange = 3)
        {
            var unit = new Unit(src);
            var wd = new WeaponData(src)
            {
                Name = name,
                Bullet = bullet,
                Class = wclass,
                MinRange = minRange,
                MaxRange = maxRange,
            };
            return new UnitWeapon(src, unit, wd);
        }

        // ──────────────────────────────────────────────
        // Bullet() / MaxBullet()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Bullet_Initially_EqualsMaxBullet()
        {
            var src = CreateSrc();
            var w = CreateWeapon(src, bullet: 10);
            Assert.AreEqual(10, w.Bullet());
        }

        [TestMethod]
        public void MaxBullet_ReturnsWeaponDataBullet()
        {
            var src = CreateSrc();
            var w = CreateWeapon(src, bullet: 8);
            Assert.AreEqual(8, w.MaxBullet());
        }

        [TestMethod]
        public void MaxBullet_ZeroBullet_ReturnsZero()
        {
            var src = CreateSrc();
            var w = CreateWeapon(src, bullet: 0);
            Assert.AreEqual(0, w.MaxBullet());
        }

        [TestMethod]
        public void Bullet_ZeroBullet_ReturnsZero()
        {
            var src = CreateSrc();
            var w = CreateWeapon(src, bullet: 0);
            Assert.AreEqual(0, w.Bullet());
        }

        // ──────────────────────────────────────────────
        // SetBullet
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetBullet_ReducesBullet()
        {
            var src = CreateSrc();
            var w = CreateWeapon(src, bullet: 6);
            w.SetBullet(3);
            Assert.AreEqual(3, w.Bullet());
        }

        [TestMethod]
        public void SetBullet_Zero_ReturnsBulletZero()
        {
            var src = CreateSrc();
            var w = CreateWeapon(src, bullet: 6);
            w.SetBullet(0);
            Assert.AreEqual(0, w.Bullet());
        }

        [TestMethod]
        public void SetBullet_Negative_ReturnsBulletZero()
        {
            var src = CreateSrc();
            var w = CreateWeapon(src, bullet: 6);
            w.SetBullet(-1);
            Assert.AreEqual(0, w.Bullet());
        }

        [TestMethod]
        public void SetBullet_Full_ReturnsMaxBullet()
        {
            var src = CreateSrc();
            var w = CreateWeapon(src, bullet: 6);
            w.SetBullet(2);
            w.SetBullet(6);
            Assert.AreEqual(6, w.Bullet());
        }

        // ──────────────────────────────────────────────
        // SetBulletFull
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetBulletFull_RestoresBulletToMax()
        {
            var src = CreateSrc();
            var w = CreateWeapon(src, bullet: 6);
            w.SetBullet(1);
            w.SetBulletFull();
            Assert.AreEqual(6, w.Bullet());
        }

        [TestMethod]
        public void SetBulletFull_AfterZero_RestoresMax()
        {
            var src = CreateSrc();
            var w = CreateWeapon(src, bullet: 4);
            w.SetBullet(0);
            w.SetBulletFull();
            Assert.AreEqual(4, w.Bullet());
        }

        // ──────────────────────────────────────────────
        // SetMaxBulletRate
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetMaxBulletRate_HalfRate_HalvesMaxBullet()
        {
            var src = CreateSrc();
            var w = CreateWeapon(src, bullet: 10);
            w.SetMaxBulletRate(0.5);
            Assert.AreEqual(5, w.MaxBullet());
        }

        [TestMethod]
        public void SetMaxBulletRate_DoubleRate_DoublesMaxBullet()
        {
            var src = CreateSrc();
            var w = CreateWeapon(src, bullet: 10);
            w.SetMaxBulletRate(2.0);
            // 最大値は99
            Assert.AreEqual(20, w.MaxBullet());
        }

        [TestMethod]
        public void SetMaxBulletRate_ExceedsMax99_CappedAt99()
        {
            var src = CreateSrc();
            var w = CreateWeapon(src, bullet: 60);
            w.SetMaxBulletRate(2.0);
            Assert.AreEqual(99, w.MaxBullet());
        }

        [TestMethod]
        public void SetMaxBulletRate_Zero_SetsMaxBulletToZero()
        {
            var src = CreateSrc();
            var w = CreateWeapon(src, bullet: 10);
            w.SetMaxBulletRate(0.0);
            Assert.AreEqual(0, w.MaxBullet());
        }

        // ──────────────────────────────────────────────
        // 弾数管理: SetBullet with MaxBullet = 0
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetBullet_WhenMaxIsZero_Positive_SetsBulletViaRate1()
        {
            var src = CreateSrc();
            var w = CreateWeapon(src, bullet: 0);
            // MaxBullet = 0 のとき、SetBullet(1) は rate=1 になる
            w.SetBullet(1);
            // Bullet = rate * maxBullet = 1 * 0 = 0
            Assert.AreEqual(0, w.Bullet());
        }

        [TestMethod]
        public void Bullet_AfterMultipleSetBulletCalls_ReturnsLastValue()
        {
            var src = CreateSrc();
            var w = CreateWeapon(src, bullet: 10);
            w.SetBullet(7);
            w.SetBullet(5);
            w.SetBullet(3);
            Assert.AreEqual(3, w.Bullet());
        }
    }
}
