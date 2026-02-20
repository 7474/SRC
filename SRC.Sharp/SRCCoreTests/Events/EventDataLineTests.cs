using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// EventDataLine クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class EventDataLineTests
    {
        // ──────────────────────────────────────────────
        // コンストラクタ・プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_SetsAllProperties()
        {
            var line = new EventDataLine(5, EventDataSource.Scenario, "test.eve", 10, "Set x 1");
            Assert.AreEqual(5, line.ID);
            Assert.AreEqual(EventDataSource.Scenario, line.Source);
            Assert.AreEqual("test.eve", line.File);
            Assert.AreEqual(10, line.LineNum);
            Assert.AreEqual("Set x 1", line.Data);
        }

        [TestMethod]
        public void Empty_HasDefaultValues()
        {
            var empty = EventDataLine.Empty;
            Assert.IsNotNull(empty);
            Assert.AreEqual(-1, empty.ID);
            Assert.AreEqual(EventDataSource.Unknown, empty.Source);
            Assert.AreEqual("-", empty.Data);
        }

        // ──────────────────────────────────────────────
        // IsSystemData
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsSystemData_ReturnsTrue_WhenSourceIsSystem()
        {
            var line = new EventDataLine(0, EventDataSource.System, "sys", 0, "cmd");
            Assert.IsTrue(line.IsSystemData);
        }

        [TestMethod]
        public void IsSystemData_ReturnsFalse_WhenSourceIsScenario()
        {
            var line = new EventDataLine(0, EventDataSource.Scenario, "test.eve", 0, "cmd");
            Assert.IsFalse(line.IsSystemData);
        }

        [TestMethod]
        public void IsSystemData_ReturnsFalse_WhenSourceIsUnknown()
        {
            var line = new EventDataLine(0, EventDataSource.Unknown, "", 0, "cmd");
            Assert.IsFalse(line.IsSystemData);
        }

        // ──────────────────────────────────────────────
        // IsAlwaysEventLabel
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsAlwaysEventLabel_ReturnsTrue_WhenDataStartsWithAsterisk()
        {
            var line = new EventDataLine(0, EventDataSource.Scenario, "test.eve", 0, "*ラベル");
            Assert.IsTrue(line.IsAlwaysEventLabel);
        }

        [TestMethod]
        public void IsAlwaysEventLabel_ReturnsFalse_WhenDataDoesNotStartWithAsterisk()
        {
            var line = new EventDataLine(0, EventDataSource.Scenario, "test.eve", 0, "Set x 1");
            Assert.IsFalse(line.IsAlwaysEventLabel);
        }

        // ──────────────────────────────────────────────
        // NextID
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NextID_ReturnsIdPlusOne()
        {
            var line = new EventDataLine(5, EventDataSource.Scenario, "test.eve", 0, "cmd");
            Assert.AreEqual(6, line.NextID);
        }

        [TestMethod]
        public void NextID_ForZeroId_ReturnsOne()
        {
            var line = new EventDataLine(0, EventDataSource.Scenario, "test.eve", 0, "cmd");
            Assert.AreEqual(1, line.NextID);
        }

        // ──────────────────────────────────────────────
        // ToString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_ContainsIdAndData()
        {
            var line = new EventDataLine(3, EventDataSource.Scenario, "test.eve", 0, "TestCmd");
            var s = line.ToString();
            Assert.IsTrue(s.Contains("3"));
            Assert.IsTrue(s.Contains("TestCmd"));
        }
    }
}
