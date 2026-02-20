using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// WeaponData クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class WeaponDataTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // フィールドの設定・読み取り
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Fields_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src)
            {
                Name = "ビームライフル",
                Power = 5000,
                MinRange = 1,
                MaxRange = 5,
                Precision = 80,
                Bullet = 10,
                ENConsumption = 20,
                NecessaryMorale = 100,
                Adaption = "空陸-宇",
                Critical = 15,
                Class = "射撃",
                NecessarySkill = "射撃Lv5",
                NecessaryCondition = ""
            };

            Assert.AreEqual("ビームライフル", wd.Name);
            Assert.AreEqual(5000, wd.Power);
            Assert.AreEqual(1, wd.MinRange);
            Assert.AreEqual(5, wd.MaxRange);
            Assert.AreEqual(80, wd.Precision);
            Assert.AreEqual(10, wd.Bullet);
            Assert.AreEqual(20, wd.ENConsumption);
            Assert.AreEqual(100, wd.NecessaryMorale);
            Assert.AreEqual("空陸-宇", wd.Adaption);
            Assert.AreEqual(15, wd.Critical);
            Assert.AreEqual("射撃", wd.Class);
            Assert.AreEqual("射撃Lv5", wd.NecessarySkill);
        }

        // ──────────────────────────────────────────────
        // Nickname
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Nickname_NameWithoutParentheses_ReturnsName()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { Name = "ビームサーベル" };
            Assert.AreEqual("ビームサーベル", wd.Nickname());
        }

        [TestMethod]
        public void Nickname_NameWithParentheses_RemovesParenthesisPart()
        {
            var src = CreateSRC();
            // 括弧部分は愛称として使われるため削除される
            var wd = new WeaponData(src) { Name = "ビームサーベル(格闘)" };
            Assert.AreEqual("ビームサーベル", wd.Nickname());
        }

        [TestMethod]
        public void Nickname_NameWithParenthesesAtStart_RemovesFromStart()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { Name = "武器(テスト)" };
            Assert.AreEqual("武器", wd.Nickname());
        }

        // ──────────────────────────────────────────────
        // IsItem
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsItem_WithItemSkill_ReturnsTrue()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src)
            {
                Name = "消耗品武器",
                NecessarySkill = "アイテム"
            };
            Assert.IsTrue(wd.IsItem());
        }

        [TestMethod]
        public void IsItem_WithItemSkillAmongOthers_ReturnsTrue()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src)
            {
                Name = "消耗品武器",
                NecessarySkill = "射撃 アイテム 格闘"
            };
            Assert.IsTrue(wd.IsItem());
        }

        [TestMethod]
        public void IsItem_WithoutItemSkill_ReturnsFalse()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src)
            {
                Name = "ビームライフル",
                NecessarySkill = "射撃Lv5"
            };
            Assert.IsFalse(wd.IsItem());
        }

        [TestMethod]
        public void IsItem_EmptySkill_ReturnsFalse()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src)
            {
                Name = "ビームライフル",
                NecessarySkill = ""
            };
            Assert.IsFalse(wd.IsItem());
        }
    }
}
