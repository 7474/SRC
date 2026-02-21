using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// AbilityListMode enum のユニットテスト
    /// </summary>
    [TestClass]
    public class AbilityListModeEnumTests
    {
        // ──────────────────────────────────────────────
        // 各値の数値確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void List_IsZero()
        {
            Assert.AreEqual(0, (int)AbilityListMode.List);
        }

        [TestMethod]
        public void BeforeMove_IsOne()
        {
            Assert.AreEqual(1, (int)AbilityListMode.BeforeMove);
        }

        [TestMethod]
        public void AfterMove_IsTwo()
        {
            Assert.AreEqual(2, (int)AbilityListMode.AfterMove);
        }

        // ──────────────────────────────────────────────
        // Enum 基本操作
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HasThreeValues()
        {
            Assert.AreEqual(3, System.Enum.GetValues(typeof(AbilityListMode)).Length);
        }

        [TestMethod]
        public void AllValuesDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(AbilityListMode), AbilityListMode.List));
            Assert.IsTrue(System.Enum.IsDefined(typeof(AbilityListMode), AbilityListMode.BeforeMove));
            Assert.IsTrue(System.Enum.IsDefined(typeof(AbilityListMode), AbilityListMode.AfterMove));
        }

        [TestMethod]
        public void AllValuesAreDistinct()
        {
            var values = System.Enum.GetValues(typeof(AbilityListMode));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (AbilityListMode v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複した値: {v} = {(int)v}");
            }
        }

        [TestMethod]
        public void CanBeParsedFromString()
        {
            Assert.AreEqual(AbilityListMode.List, System.Enum.Parse<AbilityListMode>("List"));
            Assert.AreEqual(AbilityListMode.BeforeMove, System.Enum.Parse<AbilityListMode>("BeforeMove"));
            Assert.AreEqual(AbilityListMode.AfterMove, System.Enum.Parse<AbilityListMode>("AfterMove"));
        }
    }
}
