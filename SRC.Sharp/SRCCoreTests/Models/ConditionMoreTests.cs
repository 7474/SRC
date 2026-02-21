using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// Condition クラスの追加ユニットテスト（エッジケース）
    /// </summary>
    [TestClass]
    public class ConditionMoreTests
    {
        // ──────────────────────────────────────────────
        // IsEnable 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsEnable_LargePositiveLifetime_IsTrue()
        {
            var c = new Condition { Lifetime = 1000 };
            Assert.IsTrue(c.IsEnable);
        }

        [TestMethod]
        public void IsEnable_MinusOneLifetime_IsTrue()
        {
            var c = new Condition { Lifetime = -1 };
            Assert.IsTrue(c.IsEnable);
        }

        [TestMethod]
        public void IsEnable_OneLifetime_IsTrue()
        {
            var c = new Condition { Lifetime = 1 };
            Assert.IsTrue(c.IsEnable);
        }

        // ──────────────────────────────────────────────
        // Name フィールドの各パターン
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_Sleep_CanBeSet()
        {
            var c = new Condition { Name = "睡眠", Lifetime = 3 };
            Assert.AreEqual("睡眠", c.Name);
            Assert.IsTrue(c.IsEnable);
        }

        [TestMethod]
        public void Name_Paralysis_CanBeSet()
        {
            var c = new Condition { Name = "麻痺", Lifetime = 2 };
            Assert.AreEqual("麻痺", c.Name);
        }

        [TestMethod]
        public void Name_Strengthening_CanBeSet()
        {
            var c = new Condition { Name = "強化", Lifetime = 5 };
            Assert.AreEqual("強化", c.Name);
        }

        [TestMethod]
        public void Name_LongName_CanBeSet()
        {
            var longName = new string('あ', 100);
            var c = new Condition { Name = longName, Lifetime = 1 };
            Assert.AreEqual(longName, c.Name);
        }

        // ──────────────────────────────────────────────
        // Level フィールドのパターン
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Level_Zero_IsAllowed()
        {
            var c = new Condition { Level = 0.0, Lifetime = 1 };
            Assert.AreEqual(0.0, c.Level);
        }

        [TestMethod]
        public void Level_Large_IsAllowed()
        {
            var c = new Condition { Level = 999.9, Lifetime = 1 };
            Assert.AreEqual(999.9, c.Level);
        }

        [TestMethod]
        public void Level_Negative_IsAllowed()
        {
            var c = new Condition { Level = -1.0, Lifetime = 1 };
            Assert.AreEqual(-1.0, c.Level);
        }

        // ──────────────────────────────────────────────
        // StrData フィールドのパターン
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrData_Null_IsAllowed()
        {
            var c = new Condition { StrData = null, Lifetime = 1 };
            Assert.IsNull(c.StrData);
        }

        [TestMethod]
        public void StrData_Empty_IsAllowed()
        {
            var c = new Condition { StrData = "", Lifetime = 1 };
            Assert.AreEqual("", c.StrData);
        }

        [TestMethod]
        public void StrData_WithContent_CanBeSet()
        {
            var c = new Condition { StrData = "援護攻撃 3", Lifetime = 3 };
            Assert.AreEqual("援護攻撃 3", c.StrData);
        }

        // ──────────────────────────────────────────────
        // 全フィールドの独立性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TwoConditions_AreIndependent()
        {
            var c1 = new Condition { Name = "A", Lifetime = 1, Level = 1.0, StrData = "d1" };
            var c2 = new Condition { Name = "B", Lifetime = 2, Level = 2.0, StrData = "d2" };
            Assert.AreNotEqual(c1.Name, c2.Name);
            Assert.AreNotEqual(c1.Lifetime, c2.Lifetime);
            Assert.AreNotEqual(c1.Level, c2.Level);
        }
    }
}
