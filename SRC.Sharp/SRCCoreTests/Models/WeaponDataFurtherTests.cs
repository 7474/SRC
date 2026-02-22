using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// WeaponData クラスのさらなるユニットテスト
    /// </summary>
    [TestClass]
    public class WeaponDataFurtherTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // 基本プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { Name = "バルカン" };
            Assert.AreEqual("バルカン", wd.Name);
        }

        [TestMethod]
        public void Power_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { Power = 3000 };
            Assert.AreEqual(3000, wd.Power);
        }

        [TestMethod]
        public void MinRange_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { MinRange = 1 };
            Assert.AreEqual(1, wd.MinRange);
        }

        [TestMethod]
        public void MaxRange_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { MaxRange = 3 };
            Assert.AreEqual(3, wd.MaxRange);
        }

        [TestMethod]
        public void EN_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { ENConsumption = 30 };
            Assert.AreEqual(30, wd.ENConsumption);
        }

        [TestMethod]
        public void MP_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { Bullet = 5 };
            Assert.AreEqual(5, wd.Bullet);
        }

        // ──────────────────────────────────────────────
        // EN消費
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ENConsumption_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { ENConsumption = 30 };
            Assert.AreEqual(30, wd.ENConsumption);
        }

        [TestMethod]
        public void Bullet_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { Bullet = 5 };
            Assert.AreEqual(5, wd.Bullet);
        }

        // ──────────────────────────────────────────────
        // 命中率 (Precision)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Precision_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { Precision = 20 };
            Assert.AreEqual(20, wd.Precision);
        }

        [TestMethod]
        public void Precision_Negative_IsAllowed()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { Precision = -10 };
            Assert.AreEqual(-10, wd.Precision);
        }

        // ──────────────────────────────────────────────
        // NecessaryMorale
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NecessaryMorale_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { NecessaryMorale = 120 };
            Assert.AreEqual(120, wd.NecessaryMorale);
        }

        [TestMethod]
        public void NecessaryMorale_ZeroMeansAlwaysUsable()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { NecessaryMorale = 0 };
            Assert.AreEqual(0, wd.NecessaryMorale);
        }

        // ──────────────────────────────────────────────
        // 地形適応 (Adaption)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Adaption_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { Adaption = "AABB" };
            Assert.AreEqual("AABB", wd.Adaption);
        }

        // ──────────────────────────────────────────────
        // 種別
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Class_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { Class = "射撃" };
            Assert.AreEqual("射撃", wd.Class);
        }
    }
}
