using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Pilots.Tests
{
    [TestClass]
    public class PilotsMoreTests3
    {
        private SRC CreateSRC() => new SRC { GUI = new MockGUI() };

        private Pilot AddPilot(SRC src, string name = "テストパイロット3", int level = 1, string party = "味方")
        {
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            return src.PList.Add(name, level, party);
        }

        [TestMethod]
        public void PList_Initially_CountIsZero()
        {
            var src = CreateSRC();
            Assert.AreEqual(0, src.PList.Count());
        }

        [TestMethod]
        public void PList_Items_InitiallyEmpty()
        {
            var src = CreateSRC();
            Assert.AreEqual(0, src.PList.Items.Count);
        }

        [TestMethod]
        public void Add_RegisteredPilot_ReturnsNonNull()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "P3A");
            Assert.IsNotNull(p);
        }

        [TestMethod]
        public void Add_IncreasesCount()
        {
            var src = CreateSRC();
            AddPilot(src, "P3B");
            Assert.AreEqual(1, src.PList.Count());
        }

        [TestMethod]
        public void Add_SetsPartyCorrectly()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "P3C", 1, "敵");
            Assert.AreEqual("敵", p.Party);
        }

        [TestMethod]
        public void Add_SetsLevelCorrectly()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "P3D", 5);
            Assert.AreEqual(5, p.Level);
        }

        [TestMethod]
        public void IsDefined_AfterAdd_ReturnsTrue()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "P3E");
            Assert.IsTrue(src.PList.IsDefined(p.ID));
        }

        [TestMethod]
        public void IsDefined_NonExistingID_ReturnsFalse()
        {
            var src = CreateSRC();
            Assert.IsFalse(src.PList.IsDefined("noSuchPilot"));
        }

        [TestMethod]
        public void Item_AfterAdd_ReturnsCorrectPilot()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "P3F");
            var found = src.PList.Item(p.ID);
            Assert.IsNotNull(found);
            Assert.AreEqual(p.ID, found.ID);
        }

        [TestMethod]
        public void Item_NonExisting_ReturnsNull()
        {
            var src = CreateSRC();
            Assert.IsNull(src.PList.Item("noneXYZ"));
        }

        [TestMethod]
        public void AddMultiplePilots_CountIsCorrect()
        {
            var src = CreateSRC();
            AddPilot(src, "P3G");
            AddPilot(src, "P3H");
            Assert.AreEqual(2, src.PList.Count());
        }

        [TestMethod]
        public void AlivePilots_AfterAdd_ContainsPilot()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "P3I");
            Assert.IsTrue(p.Alive);
        }

        [TestMethod]
        public void Delete_RemovesPilot()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "P3J");
            var id = p.ID;
            src.PList.Delete(id);
            Assert.IsFalse(src.PList.IsDefined(id));
        }
    }
}
