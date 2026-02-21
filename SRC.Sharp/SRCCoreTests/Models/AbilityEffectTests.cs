using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// AbilityEffect クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class AbilityEffectTests
    {
        // ──────────────────────────────────────────────
        // EffectType プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EffectType_ReturnsSameAsName()
        {
            var effect = new AbilityEffect { Name = "HP回復" };
            Assert.AreEqual("HP回復", effect.EffectType);
        }

        [TestMethod]
        public void EffectType_ReturnsNull_WhenNameIsNull()
        {
            var effect = new AbilityEffect { Name = null };
            Assert.IsNull(effect.EffectType);
        }

        [TestMethod]
        public void EffectType_ReturnsEmptyString_WhenNameIsEmpty()
        {
            var effect = new AbilityEffect { Name = "" };
            Assert.AreEqual("", effect.EffectType);
        }

        [TestMethod]
        public void EffectType_ReturnsJapanese_WhenNameIsJapanese()
        {
            var effect = new AbilityEffect { Name = "EN回復" };
            Assert.AreEqual("EN回復", effect.EffectType);
        }

        // ──────────────────────────────────────────────
        // フィールド設定テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Fields_CanBeSetAndRead()
        {
            var effect = new AbilityEffect
            {
                Name = "HP回復",
                Level = 5.0,
                Data = "50"
            };

            Assert.AreEqual("HP回復", effect.Name);
            Assert.AreEqual(5.0, effect.Level);
            Assert.AreEqual("50", effect.Data);
        }

        [TestMethod]
        public void DefaultLevel_IsZero()
        {
            var effect = new AbilityEffect { Name = "回復" };
            Assert.AreEqual(0.0, effect.Level);
        }

        [TestMethod]
        public void DefaultData_IsNull()
        {
            var effect = new AbilityEffect { Name = "回復" };
            Assert.IsNull(effect.Data);
        }

        [TestMethod]
        public void Level_CanBeNegative()
        {
            var effect = new AbilityEffect { Level = -1.5 };
            Assert.AreEqual(-1.5, effect.Level);
        }

        [TestMethod]
        public void Name_CanBeOverwritten()
        {
            var effect = new AbilityEffect { Name = "初期値" };
            effect.Name = "更新値";
            Assert.AreEqual("更新値", effect.Name);
            Assert.AreEqual("更新値", effect.EffectType);
        }
    }
}
