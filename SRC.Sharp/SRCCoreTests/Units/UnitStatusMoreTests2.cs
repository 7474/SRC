using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    [TestClass]
    public class UnitStatusMoreTests2
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ===== Status defaults =====

        [TestMethod]
        public void Status_Default_IsWaiting()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.AreEqual("待機", unit.Status);
        }

        // ===== Status setter =====

        [TestMethod]
        public void Status_SetToSortie_Works()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Status = "出撃";
            Assert.AreEqual("出撃", unit.Status);
        }

        [TestMethod]
        public void Status_SetToDestroyed_Works()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Status = "破壊";
            Assert.AreEqual("破壊", unit.Status);
        }

        [TestMethod]
        public void Status_SetToHangar_Works()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Status = "格納";
            Assert.AreEqual("格納", unit.Status);
        }

        [TestMethod]
        public void Status_SetToRetired_Works()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Status = "離脱";
            Assert.AreEqual("離脱", unit.Status);
        }

        // ===== Party defaults and setter =====

        [TestMethod]
        public void Party_Default_IsNullOrEmpty()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsTrue(string.IsNullOrEmpty(unit.Party));
        }

        [TestMethod]
        public void Party_SetToAlly_Works()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Party = "味方";
            Assert.AreEqual("味方", unit.Party);
        }

        [TestMethod]
        public void Party_SetToEnemy_Works()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Party = "敵";
            Assert.AreEqual("敵", unit.Party);
        }

        [TestMethod]
        public void Party_SetToNPC_Works()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Party = "ＮＰＣ";
            Assert.AreEqual("ＮＰＣ", unit.Party);
        }

        // ===== Mode property =====

        [TestMethod]
        public void Mode_SetAndGet_Works()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Mode = "通常";
            Assert.AreEqual("通常", unit.Mode);
        }

        [TestMethod]
        public void Mode_SetToAlly_Works()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Mode = "味方";
            Assert.AreEqual("味方", unit.Mode);
        }

        [TestMethod]
        public void Mode_SetToEnemy_Works()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Mode = "敵";
            Assert.AreEqual("敵", unit.Mode);
        }
    }
}
