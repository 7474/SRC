using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// AbilityEffect クラスの追加ユニットテスト (AbilityEffectMoreTests3)
    /// </summary>
    [TestClass]
    public class AbilityEffectMoreTests3
    {
        [TestMethod]
        public void Level_Zero_IsAllowed()
        {
            var effect = new AbilityEffect { Level = 0 };
            Assert.AreEqual(0.0, effect.Level);
        }

        [TestMethod]
        public void Level_VeryLarge_IsAllowed()
        {
            var effect = new AbilityEffect { Level = 1e15 };
            Assert.AreEqual(1e15, effect.Level, 1.0);
        }

        [TestMethod]
        public void Level_Fractional_IsAllowed()
        {
            var effect = new AbilityEffect { Level = 0.001 };
            Assert.AreEqual(0.001, effect.Level, 1e-12);
        }

        [TestMethod]
        public void Name_WithSpecialCharacters_IsAllowed()
        {
            var effect = new AbilityEffect { Name = "HP回復+EN補給" };
            Assert.AreEqual("HP回復+EN補給", effect.Name);
        }

        [TestMethod]
        public void Data_NullAllowed()
        {
            var effect = new AbilityEffect { Data = null };
            Assert.IsNull(effect.Data);
        }

        [TestMethod]
        public void Data_CanBeUpdated()
        {
            var effect = new AbilityEffect { Data = "旧データ" };
            effect.Data = "新データ";
            Assert.AreEqual("新データ", effect.Data);
        }

        [TestMethod]
        public void EffectType_AfterNameUpdate_ReflectsNewName()
        {
            var effect = new AbilityEffect { Name = "最初" };
            effect.Name = "更新後";
            Assert.AreEqual("更新後", effect.EffectType);
        }

        [TestMethod]
        public void ThreeInstances_AllIndependent()
        {
            var e1 = new AbilityEffect { Name = "A", Level = 1.0, Data = "d1" };
            var e2 = new AbilityEffect { Name = "B", Level = 2.0, Data = "d2" };
            var e3 = new AbilityEffect { Name = "C", Level = 3.0, Data = "d3" };

            Assert.AreEqual("A", e1.Name);
            Assert.AreEqual("B", e2.Name);
            Assert.AreEqual("C", e3.Name);
            Assert.AreEqual(1.0, e1.Level, 1e-10);
            Assert.AreEqual(2.0, e2.Level, 1e-10);
            Assert.AreEqual(3.0, e3.Level, 1e-10);
        }

        [TestMethod]
        public void Level_Integer_StoredAsDouble()
        {
            var effect = new AbilityEffect { Level = 7 };
            Assert.AreEqual(7.0, effect.Level, 1e-10);
        }

        [TestMethod]
        public void Name_CanContainNumbers()
        {
            var effect = new AbilityEffect { Name = "効果1" };
            Assert.AreEqual("効果1", effect.Name);
        }

        [TestMethod]
        public void EffectType_NullName_ReturnsNull()
        {
            var effect = new AbilityEffect();
            // 初期値はnull
            Assert.IsNull(effect.EffectType);
        }
    }
}
