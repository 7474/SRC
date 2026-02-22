using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    [TestClass]
    public class UnitsMoreTests3
    {
        private SRC CreateSRC() => new SRC { GUI = new MockGUI() };

        [TestMethod]
        public void UList_InitialCount_IsZero()
        {
            var src = CreateSRC();
            Assert.AreEqual(0, src.UList.Count());
        }

        [TestMethod]
        public void UList_Items_InitiallyEmpty()
        {
            var src = CreateSRC();
            Assert.AreEqual(0, src.UList.Items.Count);
        }

        [TestMethod]
        public void Add_UnknownUnit_ReturnsNull()
        {
            var src = CreateSRC();
            var result = src.UList.Add("存在しない機体XYZ", 1, "味方");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Add_UnknownUnit_CountStaysZero()
        {
            var src = CreateSRC();
            src.UList.Add("ない機体", 1, "味方");
            Assert.AreEqual(0, src.UList.Count());
        }

        [TestMethod]
        public void Add_KnownUnit_ReturnsUnit()
        {
            var src = CreateSRC();
            src.UDList.Add("テスト機体3A");
            var u = src.UList.Add("テスト機体3A", 1, "味方");
            Assert.IsNotNull(u);
        }

        [TestMethod]
        public void Add_KnownUnit_IncreasesCount()
        {
            var src = CreateSRC();
            src.UDList.Add("テスト機体3B");
            src.UList.Add("テスト機体3B", 1, "味方");
            Assert.AreEqual(1, src.UList.Count());
        }

        [TestMethod]
        public void Add_KnownUnit_SetsParty()
        {
            var src = CreateSRC();
            src.UDList.Add("テスト機体3C");
            var u = src.UList.Add("テスト機体3C", 1, "敵");
            Assert.AreEqual("敵", u.Party);
        }

        [TestMethod]
        public void Add_KnownUnit_SetsRank()
        {
            var src = CreateSRC();
            src.UDList.Add("テスト機体3D");
            var u = src.UList.Add("テスト機体3D", 7, "味方");
            Assert.AreEqual(7, u.Rank);
        }

        [TestMethod]
        public void IsDefined_NonExistingID_ReturnsFalse()
        {
            var src = CreateSRC();
            Assert.IsFalse(src.UList.IsDefined("nonexistentID"));
        }

        [TestMethod]
        public void IsDefined_AfterAdd_ReturnsTrue()
        {
            var src = CreateSRC();
            src.UDList.Add("テスト機体3E");
            var u = src.UList.Add("テスト機体3E", 1, "味方");
            Assert.IsTrue(src.UList.IsDefined(u.ID));
        }

        [TestMethod]
        public void Item_NonExistingID_ReturnsNull()
        {
            var src = CreateSRC();
            Assert.IsNull(src.UList.Item("noneXYZ"));
        }

        [TestMethod]
        public void Clear_RemovesAllUnits()
        {
            var src = CreateSRC();
            src.UDList.Add("テスト機体3F");
            src.UList.Add("テスト機体3F", 1, "味方");
            src.UList.Clear();
            Assert.AreEqual(0, src.UList.Count());
        }

        [TestMethod]
        public void AddMultipleUnits_CountIsCorrect()
        {
            var src = CreateSRC();
            src.UDList.Add("機体3G");
            src.UDList.Add("機体3H");
            src.UList.Add("機体3G", 1, "味方");
            src.UList.Add("機体3H", 1, "敵");
            Assert.AreEqual(2, src.UList.Count());
        }
    }
}
