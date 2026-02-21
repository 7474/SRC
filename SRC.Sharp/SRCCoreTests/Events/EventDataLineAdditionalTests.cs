using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// EventDataLine クラスの追加ユニットテスト
    /// </summary>
    [TestClass]
    public class EventDataLineFurtherTests
    {
        // ──────────────────────────────────────────────
        // コンストラクタ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_SetsAllFields()
        {
            var line = new EventDataLine(42, EventDataSource.Scenario, "test.eve", 10, "スタート");
            Assert.AreEqual(42, line.ID);
            Assert.AreEqual(EventDataSource.Scenario, line.Source);
            Assert.AreEqual("test.eve", line.File);
            Assert.AreEqual(10, line.LineNum);
            Assert.AreEqual("スタート", line.Data);
        }

        [TestMethod]
        public void Constructor_EmptyData_DataIsEmpty()
        {
            var line = new EventDataLine(1, EventDataSource.Scenario, "file.eve", 1, "");
            Assert.AreEqual("", line.Data);
        }

        [TestMethod]
        public void Constructor_JapaneseData_DataIsJapanese()
        {
            var line = new EventDataLine(1, EventDataSource.Scenario, "file.eve", 1, "日本語データ");
            Assert.AreEqual("日本語データ", line.Data);
        }

        // ──────────────────────────────────────────────
        // ID プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ID_IsSetCorrectly()
        {
            var line = new EventDataLine(999, EventDataSource.Scenario, "f.eve", 1, "data");
            Assert.AreEqual(999, line.ID);
        }

        [TestMethod]
        public void ID_ZeroIsValid()
        {
            var line = new EventDataLine(0, EventDataSource.Scenario, "f.eve", 1, "data");
            Assert.AreEqual(0, line.ID);
        }

        [TestMethod]
        public void ID_NegativeIsValid()
        {
            var line = new EventDataLine(-1, EventDataSource.Unknown, "-", -1, "-");
            Assert.AreEqual(-1, line.ID);
        }

        // ──────────────────────────────────────────────
        // NextID プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NextID_IsIDPlusOne()
        {
            var line = new EventDataLine(5, EventDataSource.Scenario, "f.eve", 1, "data");
            Assert.AreEqual(6, line.NextID);
        }

        [TestMethod]
        public void NextID_ZeroID_IsOne()
        {
            var line = new EventDataLine(0, EventDataSource.Scenario, "f.eve", 1, "data");
            Assert.AreEqual(1, line.NextID);
        }

        // ──────────────────────────────────────────────
        // LineNum プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LineNum_IsSetCorrectly()
        {
            var line = new EventDataLine(1, EventDataSource.Scenario, "f.eve", 100, "data");
            Assert.AreEqual(100, line.LineNum);
        }

        // ──────────────────────────────────────────────
        // File プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void File_IsSetCorrectly()
        {
            var line = new EventDataLine(1, EventDataSource.Scenario, "event_file.eve", 1, "data");
            Assert.AreEqual("event_file.eve", line.File);
        }

        // ──────────────────────────────────────────────
        // Source プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Source_Scenario_IsCorrect()
        {
            var line = new EventDataLine(1, EventDataSource.Scenario, "f.eve", 1, "data");
            Assert.AreEqual(EventDataSource.Scenario, line.Source);
        }

        [TestMethod]
        public void Source_System_IsCorrect()
        {
            var line = new EventDataLine(1, EventDataSource.System, "f.eve", 1, "data");
            Assert.AreEqual(EventDataSource.System, line.Source);
            Assert.IsTrue(line.IsSystemData);
        }

        [TestMethod]
        public void Source_Scenario_IsSystemData_IsFalse()
        {
            var line = new EventDataLine(1, EventDataSource.Scenario, "f.eve", 1, "data");
            Assert.IsFalse(line.IsSystemData);
        }

        // ──────────────────────────────────────────────
        // IsAlwaysEventLabel プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsAlwaysEventLabel_StartsWithAsterisk_ReturnsTrue()
        {
            var line = new EventDataLine(1, EventDataSource.Scenario, "f.eve", 1, "*スタート");
            Assert.IsTrue(line.IsAlwaysEventLabel);
        }

        [TestMethod]
        public void IsAlwaysEventLabel_DoesNotStartWithAsterisk_ReturnsFalse()
        {
            var line = new EventDataLine(1, EventDataSource.Scenario, "f.eve", 1, "スタート");
            Assert.IsFalse(line.IsAlwaysEventLabel);
        }

        [TestMethod]
        public void IsAlwaysEventLabel_EmptyData_ReturnsFalse()
        {
            var line = new EventDataLine(1, EventDataSource.Scenario, "f.eve", 1, "");
            Assert.IsFalse(line.IsAlwaysEventLabel);
        }

        // ──────────────────────────────────────────────
        // Empty 静的フィールド
        // ──────────────────────────────────────────────

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

        [TestMethod]
        public void Empty_IsSameInstance()
        {
            Assert.AreSame(EventDataLine.Empty, EventDataLine.Empty);
        }

        // ──────────────────────────────────────────────
        // ToString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_ContainsID()
        {
            var line = new EventDataLine(42, EventDataSource.Scenario, "f.eve", 5, "ターン");
            var str = line.ToString();
            Assert.IsTrue(str.Contains("42"), $"Expected '42' in '{str}'");
        }

        [TestMethod]
        public void ToString_ContainsData()
        {
            var line = new EventDataLine(1, EventDataSource.Scenario, "f.eve", 1, "スタート");
            var str = line.ToString();
            Assert.IsTrue(str.Contains("スタート"), $"Expected 'スタート' in '{str}'");
        }
    }
}
