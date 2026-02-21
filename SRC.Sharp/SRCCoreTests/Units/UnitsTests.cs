using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Units クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class UnitsTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // 初期状態
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Count_Initial_IsZero()
        {
            var src = CreateSRC();
            Assert.AreEqual(0, src.UList.Count());
        }

        [TestMethod]
        public void Items_Initial_IsEmpty()
        {
            var src = CreateSRC();
            Assert.AreEqual(0, src.UList.Items.Count);
        }

        // ──────────────────────────────────────────────
        // Add (UDListに定義がない場合は null を返す)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Add_WithoutUnitData_ReturnsNull()
        {
            var src = CreateSRC();
            var unit = src.UList.Add("存在しないユニット_テスト", 1, "味方");
            Assert.IsNull(unit);
        }

        [TestMethod]
        public void Add_WithoutUnitData_DoesNotIncreaseCount()
        {
            var src = CreateSRC();
            src.UList.Add("存在しないユニット", 1, "味方");
            Assert.AreEqual(0, src.UList.Count());
        }

        // ──────────────────────────────────────────────
        // Add (UDListに定義がある場合)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Add_WithRegisteredUnitData_CreatesUnit()
        {
            var src = CreateSRC();
            var ud = src.UDList.Add("テスト機体");
            var unit = src.UList.Add("テスト機体", 1, "味方");
            Assert.IsNotNull(unit);
        }

        [TestMethod]
        public void Add_WithRegisteredUnitData_IncreasesCount()
        {
            var src = CreateSRC();
            src.UDList.Add("機体A");
            src.UList.Add("機体A", 1, "味方");
            Assert.AreEqual(1, src.UList.Count());
        }

        [TestMethod]
        public void Add_WithRegisteredUnitData_SetsPartyCorrectly()
        {
            var src = CreateSRC();
            src.UDList.Add("機体B");
            var unit = src.UList.Add("機体B", 1, "敵");
            Assert.AreEqual("敵", unit.Party);
        }

        [TestMethod]
        public void Add_WithRegisteredUnitData_SetsRankCorrectly()
        {
            var src = CreateSRC();
            src.UDList.Add("機体C");
            var unit = src.UList.Add("機体C", 5, "味方");
            Assert.AreEqual(5, unit.Rank);
        }

        // ──────────────────────────────────────────────
        // IsDefined / IsDefined2
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsDefined_ForNonExistingUnit_ReturnsFalse()
        {
            var src = CreateSRC();
            Assert.IsFalse(src.UList.IsDefined("存在しないID"));
        }

        [TestMethod]
        public void IsDefined2_ForNonExistingUnit_ReturnsFalse()
        {
            var src = CreateSRC();
            Assert.IsFalse(src.UList.IsDefined2("存在しないID"));
        }

        [TestMethod]
        public void IsDefined_AfterAdd_ReturnsTrue()
        {
            var src = CreateSRC();
            src.UDList.Add("機体D");
            var unit = src.UList.Add("機体D", 1, "味方");
            Assert.IsTrue(src.UList.IsDefined(unit.ID));
        }

        // ──────────────────────────────────────────────
        // Item
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Item_ForNonExistingUnit_ReturnsNull()
        {
            var src = CreateSRC();
            Assert.IsNull(src.UList.Item("存在しないID"));
        }

        [TestMethod]
        public void Item_ForExistingUnit_ReturnsUnit()
        {
            var src = CreateSRC();
            src.UDList.Add("機体E");
            var unit = src.UList.Add("機体E", 1, "味方");
            unit.Status = "出撃";
            var found = src.UList.Item(unit.ID);
            Assert.IsNotNull(found);
            Assert.AreEqual(unit.ID, found.ID);
        }

        // ──────────────────────────────────────────────
        // Clear
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Clear_RemovesAllUnits()
        {
            var src = CreateSRC();
            src.UDList.Add("機体F");
            src.UList.Add("機体F", 1, "味方");
            src.UList.Clear();
            Assert.AreEqual(0, src.UList.Count());
        }
    }
}
