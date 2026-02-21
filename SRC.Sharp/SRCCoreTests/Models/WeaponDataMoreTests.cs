using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// WeaponData の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class WeaponDataMoreTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // Bullet=0 / Critical=0 エッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Bullet_Zero_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { Name = "EN消費武器", Bullet = 0 };
            Assert.AreEqual(0, wd.Bullet);
        }

        [TestMethod]
        public void Critical_Zero_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { Name = "通常武器", Critical = 0 };
            Assert.AreEqual(0, wd.Critical);
        }

        [TestMethod]
        public void Bullet_NegativeNotBlocked_CanBeSetAndRead()
        {
            // 弾数が設定可能かどうかを確認（バリデーションなし）
            var src = CreateSRC();
            var wd = new WeaponData(src) { Name = "テスト", Bullet = -1 };
            Assert.AreEqual(-1, wd.Bullet);
        }

        [TestMethod]
        public void Critical_ZeroBulletAndZeroCritical_BothAreZero()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { Name = "テスト", Bullet = 0, Critical = 0 };
            Assert.AreEqual(0, wd.Bullet);
            Assert.AreEqual(0, wd.Critical);
        }

        // ──────────────────────────────────────────────
        // Nickname と括弧のエッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Nickname_NameStartingWithParenthesis_ReturnsEmpty()
        {
            var src = CreateSRC();
            // 名前が「(」で始まる場合、Nicknameは空文字列になる
            var wd = new WeaponData(src) { Name = "(格闘)" };
            Assert.AreEqual("", wd.Nickname());
        }

        [TestMethod]
        public void Nickname_NameWithMultipleParentheses_ReturnsPartBeforeFirst()
        {
            var src = CreateSRC();
            // 複数の括弧がある場合は最初の「(」より前を返す
            var wd = new WeaponData(src) { Name = "武器(A)(B)" };
            Assert.AreEqual("武器", wd.Nickname());
        }

        [TestMethod]
        public void Nickname_NameWithSpaceBeforeParenthesis_ReturnsPartBeforeFirst()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { Name = "ビーム砲 (改)" };
            // 「(」の前まで返す（スペース含む）
            var nick = wd.Nickname();
            Assert.IsFalse(nick.Contains("("), $"括弧が含まれていた: {nick}");
        }

        // ──────────────────────────────────────────────
        // IsItem 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsItem_MultiWordSkillWithoutItem_ReturnsFalse()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src)
            {
                Name = "ビームガン",
                NecessarySkill = "射撃 格闘 命中"
            };
            Assert.IsFalse(wd.IsItem());
        }

        [TestMethod]
        public void IsItem_ItemAsSecondToken_ReturnsTrue()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src)
            {
                Name = "消耗品",
                NecessarySkill = "格闘 アイテム"
            };
            Assert.IsTrue(wd.IsItem());
        }
    }
}
