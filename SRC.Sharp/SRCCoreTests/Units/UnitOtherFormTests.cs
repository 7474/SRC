using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Reflection;

namespace SRCCore.Units.Tests
{
    [TestClass]
    public class UnitOtherFormTests
    {
        private SRC CreateSrc()
        {
            return new SRC
            {
                GUI = new MockGUI(),
            };
        }

        // Helper to set Unit.Name without triggering Update() (which requires Data)
        private static void SetUnitName(Unit unit, string name)
        {
            var field = typeof(Unit).GetField("strName", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(unit, name);
        }

        // ===== DeleteTemporaryOtherForm() =====

        [TestMethod]
        public void DeleteTemporaryOtherForm_NoOtherForms_DoesNotThrow()
        {
            var src = CreateSrc();
            var unit = new Unit(src) { Status = "待機" };
            SetUnitName(unit, "UnitA");

            // Should not throw even when there are no OtherForms
            unit.DeleteTemporaryOtherForm();

            Assert.AreEqual(0, unit.CountOtherForm());
        }

        [TestMethod]
        public void DeleteTemporaryOtherForm_RemovesUnnecessaryOtherForm()
        {
            var src = CreateSrc();

            var unitA = new Unit(src) { Status = "待機", ID = "A" };
            SetUnitName(unitA, "UnitA");

            var unitB = new Unit(src) { Status = "他形態", ID = "B" };
            SetUnitName(unitB, "UnitB");

            unitA.AddOtherForm(unitB);

            Assert.AreEqual(1, unitA.CountOtherForm());

            // Without features, only "UnitA" is in neededForms → "UnitB" is unnecessary
            unitA.DeleteTemporaryOtherForm();

            // UnitB should be removed from unitA's OtherForms
            Assert.AreEqual(0, unitA.CountOtherForm());

            // UnitB should be marked as discarded
            Assert.AreEqual("破棄", unitB.Status);
        }

        [TestMethod]
        public void DeleteTemporaryOtherForm_MultipleOtherForms_RemovesAllUnnecessary()
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

            unitA.DeleteTemporaryOtherForm();

            Assert.AreEqual(0, unitA.CountOtherForm());
            Assert.AreEqual("破棄", unitB.Status);
            Assert.AreEqual("破棄", unitC.Status);
        }

        [TestMethod]
        public void DeleteTemporaryOtherForm_RemovesLinksFromInactiveOtherForms()
        {
            var src = CreateSrc();

            var unitA = new Unit(src) { Status = "他形態", ID = "A" };
            SetUnitName(unitA, "UnitA");

            var unitB = new Unit(src) { Status = "他形態", ID = "B" };
            SetUnitName(unitB, "UnitB");

            var unitC = new Unit(src) { Status = "待機", ID = "C" };
            SetUnitName(unitC, "UnitC");

            // C is the active form; A and B are inactive forms
            // A links to B and C; C has A and B as OtherForms
            unitA.AddOtherForm(unitB);
            unitA.AddOtherForm(unitC);
            unitC.AddOtherForm(unitA);
            unitC.AddOtherForm(unitB);

            // C has no features, so neededForms = {"UnitC"}
            // Both A and B should be removed; A's cross-link to B should also be removed
            unitC.DeleteTemporaryOtherForm();

            Assert.AreEqual(0, unitC.CountOtherForm());
            Assert.AreEqual("破棄", unitA.Status);
            Assert.AreEqual("破棄", unitB.Status);
        }
    }
}
