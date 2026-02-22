using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// Condition クラスの追加ユニットテスト
    /// </summary>
    [TestClass]
    public class ConditionFurtherTests
    {
        // ──────────────────────────────────────────────
        // プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var c = new Condition { Name = "麻痺" };
            Assert.AreEqual("麻痺", c.Name);
        }

        [TestMethod]
        public void Lifetime_CanBeSetAndRead()
        {
            var c = new Condition { Lifetime = 5 };
            Assert.AreEqual(5, c.Lifetime);
        }

        [TestMethod]
        public void Level_CanBeSetAndRead()
        {
            var c = new Condition { Level = 2.5 };
            Assert.AreEqual(2.5, c.Level, 1e-10);
        }

        [TestMethod]
        public void StrData_CanBeSetAndRead()
        {
            var c = new Condition { StrData = "追加データ" };
            Assert.AreEqual("追加データ", c.StrData);
        }

        [TestMethod]
        public void DefaultValues_AreDefault()
        {
            var c = new Condition();
            Assert.IsNull(c.Name);
            Assert.AreEqual(0, c.Lifetime);
            // Level のデフォルト値は0（DEFAULT_LEVELではない）
            Assert.AreEqual(0d, c.Level, 1e-10);
            Assert.IsNull(c.StrData);
        }

        [TestMethod]
        public void Lifetime_Negative_Indicates_Permanent()
        {
            var c = new Condition { Lifetime = -1 };
            Assert.AreEqual(-1, c.Lifetime);
        }

        [TestMethod]
        public void Lifetime_Zero_IsAllowed()
        {
            var c = new Condition { Lifetime = 0 };
            Assert.AreEqual(0, c.Lifetime);
        }

        [TestMethod]
        public void AllProperties_SetAtOnce()
        {
            var c = new Condition
            {
                Name = "睡眠",
                Lifetime = 3,
                Level = 1.0,
                StrData = "スリープ"
            };

            Assert.AreEqual("睡眠", c.Name);
            Assert.AreEqual(3, c.Lifetime);
            Assert.AreEqual(1.0, c.Level, 1e-10);
            Assert.AreEqual("スリープ", c.StrData);
        }

        [TestMethod]
        public void Name_EmptyString_IsAllowed()
        {
            var c = new Condition { Name = "" };
            Assert.AreEqual("", c.Name);
        }

        [TestMethod]
        public void Level_Zero_IsAllowed()
        {
            var c = new Condition { Level = 0 };
            Assert.AreEqual(0, c.Level, 1e-10);
        }
    }
}
