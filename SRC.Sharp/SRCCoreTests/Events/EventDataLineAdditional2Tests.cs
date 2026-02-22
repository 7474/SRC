using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// EventDataLine, EventDataSource の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class EventDataLineAdditional2Tests
    {
        // ──────────────────────────────────────────────
        // EventDataLine - IsAlwaysEventLabel
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsAlwaysEventLabel_WithAsterisk_IsTrue()
        {
            var line = new EventDataLine(0, EventDataSource.Scenario, "test.eve", 0, "*スタート");
            Assert.IsTrue(line.IsAlwaysEventLabel);
        }

        [TestMethod]
        public void IsAlwaysEventLabel_WithoutAsterisk_IsFalse()
        {
            var line = new EventDataLine(0, EventDataSource.Scenario, "test.eve", 0, "スタート");
            Assert.IsFalse(line.IsAlwaysEventLabel);
        }

        [TestMethod]
        public void IsAlwaysEventLabel_EmptyString_IsFalse()
        {
            var line = new EventDataLine(0, EventDataSource.Scenario, "test.eve", 0, "");
            Assert.IsFalse(line.IsAlwaysEventLabel);
        }

        [TestMethod]
        public void IsAlwaysEventLabel_RegularCommand_IsFalse()
        {
            var line = new EventDataLine(0, EventDataSource.Scenario, "test.eve", 0, "Set x 1");
            Assert.IsFalse(line.IsAlwaysEventLabel);
        }

        // ──────────────────────────────────────────────
        // EventDataLine - NextID
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NextID_IsIdPlusOne()
        {
            var line = new EventDataLine(5, EventDataSource.Scenario, "test.eve", 10, "nop");
            Assert.AreEqual(6, line.NextID);
        }

        [TestMethod]
        public void NextID_ForId0_IsOne()
        {
            var line = new EventDataLine(0, EventDataSource.Scenario, "test.eve", 0, "nop");
            Assert.AreEqual(1, line.NextID);
        }

        [TestMethod]
        public void NextID_ForNegativeId_IsZero()
        {
            var line = new EventDataLine(-1, EventDataSource.Unknown, "-", -1, "-");
            Assert.AreEqual(0, line.NextID);
        }

        // ──────────────────────────────────────────────
        // EventDataLine - ToString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_ContainsId()
        {
            var line = new EventDataLine(42, EventDataSource.Scenario, "test.eve", 10, "Set x 1");
            var str = line.ToString();
            Assert.IsTrue(str.Contains("42"));
        }

        [TestMethod]
        public void ToString_ContainsData()
        {
            var line = new EventDataLine(0, EventDataSource.Scenario, "test.eve", 0, "Set x 1");
            var str = line.ToString();
            Assert.IsTrue(str.Contains("Set x 1"));
        }

        [TestMethod]
        public void ToString_FormattedCorrectly()
        {
            var line = new EventDataLine(3, EventDataSource.Scenario, "test.eve", 5, "Nop");
            var str = line.ToString();
            Assert.AreEqual("3: Nop", str);
        }

        // ──────────────────────────────────────────────
        // EventDataLine - Empty
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Empty_IsSingletonLike()
        {
            var e1 = EventDataLine.Empty;
            var e2 = EventDataLine.Empty;
            Assert.AreSame(e1, e2);
        }

        [TestMethod]
        public void Empty_File_IsDash()
        {
            Assert.AreEqual("-", EventDataLine.Empty.File);
        }

        [TestMethod]
        public void Empty_LineNum_IsNegativeOne()
        {
            Assert.AreEqual(-1, EventDataLine.Empty.LineNum);
        }

        [TestMethod]
        public void Empty_Source_IsUnknown()
        {
            Assert.AreEqual(EventDataSource.Unknown, EventDataLine.Empty.Source);
        }

        // ──────────────────────────────────────────────
        // EventDataSource
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EventDataSource_Unknown_IsZero()
        {
            Assert.AreEqual(0, (int)EventDataSource.Unknown);
        }

        [TestMethod]
        public void EventDataSource_System_IsOne()
        {
            Assert.AreEqual(1, (int)EventDataSource.System);
        }

        [TestMethod]
        public void EventDataSource_Scenario_IsTwo()
        {
            Assert.AreEqual(2, (int)EventDataSource.Scenario);
        }

        [TestMethod]
        public void EventDataSource_HasThreeValues()
        {
            Assert.AreEqual(3, System.Enum.GetValues(typeof(EventDataSource)).Length);
        }
    }
}
