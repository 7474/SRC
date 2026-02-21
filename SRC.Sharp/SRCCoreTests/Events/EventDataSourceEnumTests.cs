using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// EventDataSource 列挙型のユニットテスト
    /// </summary>
    [TestClass]
    public class EventDataSourceEnumTests
    {
        // ──────────────────────────────────────────────
        // 各値の IsDefined
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsDefined_Unknown_ReturnsTrue()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(EventDataSource), EventDataSource.Unknown));
        }

        [TestMethod]
        public void IsDefined_System_ReturnsTrue()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(EventDataSource), EventDataSource.System));
        }

        [TestMethod]
        public void IsDefined_Scenario_ReturnsTrue()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(EventDataSource), EventDataSource.Scenario));
        }

        // ──────────────────────────────────────────────
        // 数値確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Unknown_IsZero()
        {
            Assert.AreEqual(0, (int)EventDataSource.Unknown);
        }

        [TestMethod]
        public void System_IsOne()
        {
            Assert.AreEqual(1, (int)EventDataSource.System);
        }

        [TestMethod]
        public void Scenario_IsTwo()
        {
            Assert.AreEqual(2, (int)EventDataSource.Scenario);
        }

        // ──────────────────────────────────────────────
        // 相互比較
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Unknown_NotEqualTo_System()
        {
            Assert.AreNotEqual(EventDataSource.Unknown, EventDataSource.System);
        }

        [TestMethod]
        public void System_NotEqualTo_Scenario()
        {
            Assert.AreNotEqual(EventDataSource.System, EventDataSource.Scenario);
        }

        // ──────────────────────────────────────────────
        // 全値が異なる
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AllValuesAreDistinct()
        {
            var values = System.Enum.GetValues(typeof(EventDataSource));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (EventDataSource v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複: {v}={v:D}");
            }
        }
    }
}
