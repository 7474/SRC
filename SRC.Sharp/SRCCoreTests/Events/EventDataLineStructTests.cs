using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// EventDataLine クラスの追加ユニットテスト
    /// </summary>
    [TestClass]
    public class EventDataLineStructTests
    {
        // ──────────────────────────────────────────────
        // コンストラクタ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_SetsAllProperties()
        {
            var line = new EventDataLine(10, EventDataSource.Scenario, "test.eve", 5, "Talk テスト");
            Assert.AreEqual(10, line.ID);
            Assert.AreEqual(EventDataSource.Scenario, line.Source);
            Assert.AreEqual("test.eve", line.File);
            Assert.AreEqual(5, line.LineNum);
            Assert.AreEqual("Talk テスト", line.Data);
        }

        [TestMethod]
        public void Constructor_WithSystemSource_SetsCorrectly()
        {
            var line = new EventDataLine(0, EventDataSource.System, "system.eve", 1, "Set x 0");
            Assert.AreEqual(EventDataSource.System, line.Source);
            Assert.IsTrue(line.IsSystemData);
        }

        // ──────────────────────────────────────────────
        // IsSystemData プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsSystemData_ReturnsTrue_WhenSourceIsSystem()
        {
            var line = new EventDataLine(1, EventDataSource.System, "f", 1, "data");
            Assert.IsTrue(line.IsSystemData);
        }

        [TestMethod]
        public void IsSystemData_ReturnsFalse_WhenSourceIsScenario()
        {
            var line = new EventDataLine(1, EventDataSource.Scenario, "f", 1, "data");
            Assert.IsFalse(line.IsSystemData);
        }

        [TestMethod]
        public void IsSystemData_ReturnsFalse_WhenSourceIsUnknown()
        {
            var line = new EventDataLine(1, EventDataSource.Unknown, "f", 1, "data");
            Assert.IsFalse(line.IsSystemData);
        }

        // ──────────────────────────────────────────────
        // IsAlwaysEventLabel プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsAlwaysEventLabel_ReturnsTrue_WhenDataStartsWithAsterisk()
        {
            var line = new EventDataLine(1, EventDataSource.Scenario, "f", 1, "*イベント");
            Assert.IsTrue(line.IsAlwaysEventLabel);
        }

        [TestMethod]
        public void IsAlwaysEventLabel_ReturnsFalse_WhenDataDoesNotStartWithAsterisk()
        {
            var line = new EventDataLine(1, EventDataSource.Scenario, "f", 1, "Set x 0");
            Assert.IsFalse(line.IsAlwaysEventLabel);
        }

        [TestMethod]
        public void IsAlwaysEventLabel_ReturnsFalse_WhenDataIsEmpty()
        {
            var line = new EventDataLine(1, EventDataSource.Scenario, "f", 1, "");
            Assert.IsFalse(line.IsAlwaysEventLabel);
        }

        // ──────────────────────────────────────────────
        // NextID プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NextID_IsIDPlusOne()
        {
            var line = new EventDataLine(5, EventDataSource.Scenario, "f", 1, "data");
            Assert.AreEqual(6, line.NextID);
        }

        [TestMethod]
        public void NextID_WhenIDIsZero_IsOne()
        {
            var line = new EventDataLine(0, EventDataSource.Scenario, "f", 1, "data");
            Assert.AreEqual(1, line.NextID);
        }

        // ──────────────────────────────────────────────
        // Empty 静的フィールド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Empty_IsNotNull()
        {
            Assert.IsNotNull(EventDataLine.Empty);
        }

        [TestMethod]
        public void Empty_HasNegativeID()
        {
            Assert.AreEqual(-1, EventDataLine.Empty.ID);
        }

        [TestMethod]
        public void Empty_HasUnknownSource()
        {
            Assert.AreEqual(EventDataSource.Unknown, EventDataLine.Empty.Source);
        }

        // ──────────────────────────────────────────────
        // ToString テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_ReturnsFormattedString()
        {
            var line = new EventDataLine(7, EventDataSource.Scenario, "test.eve", 3, "Talk こんにちは");
            var result = line.ToString();
            Assert.AreEqual("7: Talk こんにちは", result);
        }
    }
}
