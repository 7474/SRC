using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// UnitData フィールド割り当ての追加ユニットテスト（UnitDataMoreTests3）
    /// </summary>
    [TestClass]
    public class UnitDataMoreTests3
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // HP / EN / Speed / Size
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HP_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { HP = 9999 };
            Assert.AreEqual(9999, ud.HP);
        }

        [TestMethod]
        public void HP_Zero_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { HP = 0 };
            Assert.AreEqual(0, ud.HP);
        }

        [TestMethod]
        public void EN_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { EN = 200 };
            Assert.AreEqual(200, ud.EN);
        }

        [TestMethod]
        public void Speed_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Speed = 12 };
            Assert.AreEqual(12, ud.Speed);
        }

        [TestMethod]
        public void Size_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Size = "L" };
            Assert.AreEqual("L", ud.Size);
        }

        [TestMethod]
        public void Size_SizeS_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Size = "S" };
            Assert.AreEqual("S", ud.Size);
        }

        // ──────────────────────────────────────────────
        // Armor / Mobility / Value / ExpValue
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Armor_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Armor = 2000 };
            Assert.AreEqual(2000, ud.Armor);
        }

        [TestMethod]
        public void Mobility_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Mobility = 150 };
            Assert.AreEqual(150, ud.Mobility);
        }

        [TestMethod]
        public void Value_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Value = 50000 };
            Assert.AreEqual(50000, ud.Value);
        }

        [TestMethod]
        public void ExpValue_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { ExpValue = 300 };
            Assert.AreEqual(300, ud.ExpValue);
        }

        // ──────────────────────────────────────────────
        // Name / Class
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "ガンダムMk-II" };
            Assert.AreEqual("ガンダムMk-II", ud.Name);
        }

        [TestMethod]
        public void Class_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Class = "モビルスーツ" };
            Assert.AreEqual("モビルスーツ", ud.Class);
        }

        // ──────────────────────────────────────────────
        // Nickname / KanaName
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Nickname_SimpleValue_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Nickname = "白い悪魔" };
            Assert.AreEqual("白い悪魔", ud.Nickname);
        }

        [TestMethod]
        public void KanaName_SimpleValue_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { KanaName = "がんだむ" };
            Assert.AreEqual("がんだむ", ud.KanaName);
        }
    }
}
