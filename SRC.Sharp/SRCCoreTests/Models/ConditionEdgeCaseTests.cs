using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// Condition クラスの追加ユニットテスト（詳細なケース）
    /// </summary>
    [TestClass]
    public class ConditionEdgeCaseTests
    {
        // ──────────────────────────────────────────────
        // IsEnable - 境界値テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsEnable_LifetimeIsMinusOne_ReturnsTrue()
        {
            var cond = new Condition { Lifetime = -1 };
            Assert.IsTrue(cond.IsEnable);
        }

        [TestMethod]
        public void IsEnable_LifetimeIsHundred_ReturnsTrue()
        {
            var cond = new Condition { Lifetime = 100 };
            Assert.IsTrue(cond.IsEnable);
        }

        [TestMethod]
        public void IsEnable_LifetimeIsOne_ReturnsTrue()
        {
            var cond = new Condition { Lifetime = 1 };
            Assert.IsTrue(cond.IsEnable);
        }

        [TestMethod]
        public void IsEnable_LifetimeIsZero_ReturnsFalse()
        {
            var cond = new Condition { Lifetime = 0 };
            Assert.IsFalse(cond.IsEnable);
        }

        // ──────────────────────────────────────────────
        // 名称フィールドの各種テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_SetToJapaneseName_CanBeRead()
        {
            var cond = new Condition { Name = "麻痺" };
            Assert.AreEqual("麻痺", cond.Name);
        }

        [TestMethod]
        public void Name_SetToEmptyString_CanBeRead()
        {
            var cond = new Condition { Name = "" };
            Assert.AreEqual("", cond.Name);
        }

        [TestMethod]
        public void Name_SetToNull_ReturnsNull()
        {
            var cond = new Condition { Name = null };
            Assert.IsNull(cond.Name);
        }

        [TestMethod]
        public void Name_LongName_CanBeRead()
        {
            var longName = new string('あ', 100);
            var cond = new Condition { Name = longName };
            Assert.AreEqual(longName, cond.Name);
        }

        // ──────────────────────────────────────────────
        // Level フィールド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Level_MaxDouble_CanBeSet()
        {
            var cond = new Condition { Level = double.MaxValue };
            Assert.AreEqual(double.MaxValue, cond.Level);
        }

        [TestMethod]
        public void Level_MinDouble_CanBeSet()
        {
            var cond = new Condition { Level = double.MinValue };
            Assert.AreEqual(double.MinValue, cond.Level);
        }

        [TestMethod]
        public void Level_DecimalValue_CanBeSet()
        {
            var cond = new Condition { Level = 1.5 };
            Assert.AreEqual(1.5, cond.Level);
        }

        [TestMethod]
        public void Level_NegativeValue_CanBeSet()
        {
            var cond = new Condition { Level = -3.0 };
            Assert.AreEqual(-3.0, cond.Level);
        }

        // ──────────────────────────────────────────────
        // StrData フィールド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrData_SetToJapaneseString_CanBeRead()
        {
            var cond = new Condition { StrData = "攻撃力アップ" };
            Assert.AreEqual("攻撃力アップ", cond.StrData);
        }

        [TestMethod]
        public void StrData_SetToNull_ReturnsNull()
        {
            var cond = new Condition { StrData = null };
            Assert.IsNull(cond.StrData);
        }

        // ──────────────────────────────────────────────
        // Lifetime 変更テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Lifetime_CanBeDecrementedToZero()
        {
            var cond = new Condition { Lifetime = 1 };
            Assert.IsTrue(cond.IsEnable);
            cond.Lifetime = 0;
            Assert.IsFalse(cond.IsEnable);
        }

        [TestMethod]
        public void Lifetime_CanBeIncrementedFromZero()
        {
            var cond = new Condition { Lifetime = 0 };
            Assert.IsFalse(cond.IsEnable);
            cond.Lifetime = 1;
            Assert.IsTrue(cond.IsEnable);
        }

        // ──────────────────────────────────────────────
        // 複数フィールドの独立性テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AllFields_AreIndependent_NoInterference()
        {
            var c1 = new Condition { Name = "麻痺", Lifetime = 3, Level = 1.0, StrData = "データA" };
            var c2 = new Condition { Name = "毒", Lifetime = 0, Level = 2.0, StrData = "データB" };

            Assert.AreNotEqual(c1.Name, c2.Name);
            Assert.AreNotEqual(c1.Lifetime, c2.Lifetime);
            Assert.AreNotEqual(c1.Level, c2.Level);
            Assert.AreNotEqual(c1.StrData, c2.StrData);
            Assert.IsTrue(c1.IsEnable);
            Assert.IsFalse(c2.IsEnable);
        }
    }
}
