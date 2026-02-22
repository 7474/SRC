using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Reflection;

namespace SRCCore.Units.Tests
{
    [TestClass]
    public class UnitOtherFormMoreTests2
    {
        private SRC CreateSrc()
        {
            return new SRC
            {
                GUI = new MockGUI(),
            };
        }

        private static void SetUnitName(Unit unit, string name)
        {
            var field = typeof(Unit).GetField("strName", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(unit, name);
        }

        // ──────────────────────────────────────────────
        // CountOtherForm
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CountOtherForm_Initially_ReturnsZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src) { Status = "待機" };
            SetUnitName(unit, "UnitA");
            Assert.AreEqual(0, unit.CountOtherForm());
        }

        // ──────────────────────────────────────────────
        // AddOtherForm / CountOtherForm
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddOtherForm_IncreasesCount()
        {
            var src = CreateSrc();
            var unitA = new Unit(src) { Status = "待機", ID = "A" };
            SetUnitName(unitA, "UnitA");

            var unitB = new Unit(src) { Status = "他形態", ID = "B" };
            SetUnitName(unitB, "UnitB");

            unitA.AddOtherForm(unitB);
            Assert.AreEqual(1, unitA.CountOtherForm());
        }

        [TestMethod]
        public void AddOtherForm_MultipleFormsIncreasesCount()
        {
            var src = CreateSrc();
            var unitA = new Unit(src) { Status = "待機", ID = "A" };
            SetUnitName(unitA, "UnitA");

            var unitB = new Unit(src) { Status = "他形態", ID = "B" };
            SetUnitName(unitB, "UnitB");

            var unitC = new Unit(src) { Status = "他形態", ID = "C" };
            SetUnitName(unitC, "UnitC");

            unitA.AddOtherForm(unitB);
            unitA.AddOtherForm(unitC);
            Assert.AreEqual(2, unitA.CountOtherForm());
        }

        // ──────────────────────────────────────────────
        // CurrentForm
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CurrentForm_WhenNotOtherForm_ReturnsSelf()
        {
            var src = CreateSrc();
            var unit = new Unit(src) { Status = "待機", ID = "A" };
            SetUnitName(unit, "UnitA");
            Assert.AreSame(unit, unit.CurrentForm());
        }

        // ──────────────────────────────────────────────
        // DeleteOtherForm
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DeleteOtherForm_ById_DecreasesCount()
        {
            var src = CreateSrc();
            var unitA = new Unit(src) { Status = "待機", ID = "A" };
            SetUnitName(unitA, "UnitA");

            var unitB = new Unit(src) { Status = "他形態", ID = "B" };
            SetUnitName(unitB, "UnitB");

            unitA.AddOtherForm(unitB);
            Assert.AreEqual(1, unitA.CountOtherForm());

            unitA.DeleteOtherForm("B");
            Assert.AreEqual(0, unitA.CountOtherForm());
        }

        [TestMethod]
        public void DeleteOtherForm_NonExistent_DoesNotThrow()
        {
            var src = CreateSrc();
            var unit = new Unit(src) { Status = "待機", ID = "A" };
            SetUnitName(unit, "UnitA");

            // Should not throw even when ID doesn't exist
            unit.DeleteOtherForm("NonExistentID");
            Assert.AreEqual(0, unit.CountOtherForm());
        }
    }
}
