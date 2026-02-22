using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;
using System;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// EventDataSource 列挙型のユニットテスト
    /// </summary>
    [TestClass]
    public class EventDataSourceTests
    {
        // ──────────────────────────────────────────────
        // 基本値確認
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
        // IsDefined テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsDefined_Unknown_ReturnsTrue()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(EventDataSource), EventDataSource.Unknown));
        }

        [TestMethod]
        public void IsDefined_System_ReturnsTrue()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(EventDataSource), EventDataSource.System));
        }

        [TestMethod]
        public void IsDefined_Scenario_ReturnsTrue()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(EventDataSource), EventDataSource.Scenario));
        }

        // ──────────────────────────────────────────────
        // メンバー数確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MemberCount_IsThree()
        {
            var values = Enum.GetValues(typeof(EventDataSource));
            Assert.AreEqual(3, values.Length);
        }

        // ──────────────────────────────────────────────
        // 全ての値が異なる整数値を持つ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AllValues_HaveUniqueIntegers()
        {
            var values = Enum.GetValues(typeof(EventDataSource));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (EventDataSource v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複した値が見つかりました: {v} = {(int)v}");
            }
        }

        // ──────────────────────────────────────────────
        // Parse テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Parse_Unknown_Succeeds()
        {
            Assert.AreEqual(EventDataSource.Unknown, Enum.Parse<EventDataSource>("Unknown"));
        }

        [TestMethod]
        public void Parse_System_Succeeds()
        {
            Assert.AreEqual(EventDataSource.System, Enum.Parse<EventDataSource>("System"));
        }

        [TestMethod]
        public void Parse_Scenario_Succeeds()
        {
            Assert.AreEqual(EventDataSource.Scenario, Enum.Parse<EventDataSource>("Scenario"));
        }

        // ──────────────────────────────────────────────
        // 相互比較
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Unknown_NotEqual_System()
        {
            Assert.AreNotEqual(EventDataSource.Unknown, EventDataSource.System);
        }

        [TestMethod]
        public void System_NotEqual_Scenario()
        {
            Assert.AreNotEqual(EventDataSource.System, EventDataSource.Scenario);
        }
    }
}
