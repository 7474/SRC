using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// AbilityEffect クラスの追加ユニットテスト
    /// </summary>
    [TestClass]
    public class AbilityEffectMoreTests
    {
        // ──────────────────────────────────────────────
        // プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var effect = new AbilityEffect { Name = "修理" };
            Assert.AreEqual("修理", effect.Name);
        }

        [TestMethod]
        public void Level_CanBeSetAndRead()
        {
            var effect = new AbilityEffect { Level = 3.5 };
            Assert.AreEqual(3.5, effect.Level, 1e-10);
        }

        [TestMethod]
        public void Data_CanBeSetAndRead()
        {
            var effect = new AbilityEffect { Data = "テストデータ" };
            Assert.AreEqual("テストデータ", effect.Data);
        }

        [TestMethod]
        public void EffectType_MatchesName()
        {
            var effect = new AbilityEffect { Name = "補給" };
            Assert.AreEqual("補給", effect.EffectType);
        }

        [TestMethod]
        public void DefaultValues_AreNull()
        {
            var effect = new AbilityEffect();
            Assert.IsNull(effect.Name);
            Assert.AreEqual(0d, effect.Level);
            Assert.IsNull(effect.Data);
        }

        [TestMethod]
        public void Name_EmptyString_IsAllowed()
        {
            var effect = new AbilityEffect { Name = "" };
            Assert.AreEqual("", effect.Name);
        }

        [TestMethod]
        public void Level_Negative_IsAllowed()
        {
            var effect = new AbilityEffect { Level = -2.0 };
            Assert.AreEqual(-2.0, effect.Level, 1e-10);
        }

        [TestMethod]
        public void AllProperties_SetAtOnce()
        {
            var effect = new AbilityEffect
            {
                Name = "修理",
                Level = 1.5,
                Data = "供給データ"
            };
            Assert.AreEqual("修理", effect.Name);
            Assert.AreEqual(1.5, effect.Level, 1e-10);
            Assert.AreEqual("供給データ", effect.Data);
            Assert.AreEqual("修理", effect.EffectType);
        }

        [TestMethod]
        public void TwoInstances_AreIndependent()
        {
            var e1 = new AbilityEffect { Name = "修理", Level = 1.0 };
            var e2 = new AbilityEffect { Name = "補給", Level = 2.0 };
            Assert.AreNotEqual(e1.Name, e2.Name);
            Assert.AreNotEqual(e1.Level, e2.Level);
        }
    }
}
