using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// UnitData クラスの追加ユニットテスト
    /// </summary>
    [TestClass]
    public class UnitDataFurtherTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // 基本フィールド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Fields_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src)
            {
                Name = "テストユニット",
                HP = 1000,
                EN = 200,
                Mobility = 60,
                Armor = 80,
                Size = "M",
                Adaption = "陸"
            };

            Assert.AreEqual("テストユニット", ud.Name);
            Assert.AreEqual(1000, ud.HP);
            Assert.AreEqual(200, ud.EN);
            Assert.AreEqual(60, ud.Mobility);
            Assert.AreEqual(80, ud.Armor);
            Assert.AreEqual("M", ud.Size);
            Assert.AreEqual("陸", ud.Adaption);
        }

        [TestMethod]
        public void HP_Zero_IsAllowed()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { HP = 0 };
            Assert.AreEqual(0, ud.HP);
        }

        [TestMethod]
        public void EN_Zero_IsAllowed()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { EN = 0 };
            Assert.AreEqual(0, ud.EN);
        }

        // ──────────────────────────────────────────────
        // Nickname プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Nickname_NoParentheses_ReturnsFullName()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "ガンダム", Nickname = "ガンダム" };
            Assert.AreEqual("ガンダム", ud.Nickname);
        }

        [TestMethod]
        public void Nickname_EmptyString_ReturnsEmpty()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "", Nickname = "" };
            Assert.AreEqual("", ud.Nickname);
        }

        // ──────────────────────────────────────────────
        // Speed プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Speed_CanBeSet()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Speed = 7 };
            Assert.AreEqual(7, ud.Speed);
        }

        // ──────────────────────────────────────────────
        // Transportation プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Transportation_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Transportation = "地" };
            Assert.AreEqual("地", ud.Transportation);
        }
    }
}
