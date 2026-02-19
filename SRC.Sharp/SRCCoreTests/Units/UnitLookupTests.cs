using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    [TestClass]
    public class UnitLookupTests
    {
        private SRC CreateSrc()
        {
            return new SRC
            {
                GUI = new MockGUI(),
            };
        }

        // ===== IsAvailable() =====

        [TestMethod]
        public void IsAvailable_ReturnsTrue_WhenNoPilotsAndNotDisabled()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            // No name → IsDisabled returns false; no pilots → returns true early
            Assert.IsTrue(unit.IsAvailable());
        }

        [TestMethod]
        public void IsDisabled_ReturnsTrue_WhenGlobalDisableVariableDefined()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            // Define a global Disable variable and verify IsDisabled returns true
            src.Expression.DefineGlobalVariable("Disable(テスト)");
            Assert.IsTrue(unit.IsDisabled("テスト"));
        }

        [TestMethod]
        public void IsAvailable_ReturnsFalse_WhenStatus他形態AndActionDisabled()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            unit.Status = "他形態";
            unit.AddCondition("行動不能", 3);

            Assert.IsFalse(unit.IsAvailable());
        }

        [TestMethod]
        public void IsAvailable_ReturnsTrue_WhenStatus他形態ButNoActionDisabled()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            unit.Status = "他形態";
            // No 行動不能 condition → still true when no pilots

            Assert.IsTrue(unit.IsAvailable());
        }

        // ===== IsNecessarySkillSatisfied() =====

        [TestMethod]
        public void IsNecessarySkillSatisfied_ReturnsTrue_WhenNabilitiesIsEmpty()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            Assert.IsTrue(unit.IsNecessarySkillSatisfied(""));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied_ReturnsTrue_WhenNabilitiesIsNull()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            Assert.IsTrue(unit.IsNecessarySkillSatisfied(null));
        }

        // ===== IsNecessarySkillSatisfied2() =====

        [TestMethod]
        public void IsNecessarySkillSatisfied2_ReturnsFalse_WhenNoMatchAndNoMp()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            // No pilot, sname = "格闘", nlevel = 1 → slevel = 0 < 1 → false
            Assert.IsFalse(unit.IsNecessarySkillSatisfied2("格闘", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_ReturnsTrue_ForItemSkill()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            // "アイテム" always has slevel = 1d >= 1 → true
            Assert.IsTrue(unit.IsNecessarySkillSatisfied2("アイテム", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_ReturnsFalse_ForAtemiSkill()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            // "当て身技" always returns false
            Assert.IsFalse(unit.IsNecessarySkillSatisfied2("当て身技", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_ReturnsFalse_ForAutoCounterSkill()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            // "自動反撃" always returns false
            Assert.IsFalse(unit.IsNecessarySkillSatisfied2("自動反撃", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_ReturnsFalse_ForNotSkillNegation()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            // "!アイテム" negates アイテム (true) → false
            Assert.IsFalse(unit.IsNecessarySkillSatisfied2("!アイテム", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_ReturnsTrue_ForNotSkillNegationFalse()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            // "!格闘" negates 格闘 (false) → true
            Assert.IsTrue(unit.IsNecessarySkillSatisfied2("!格闘", null));
        }

        // ===== IsAbleToEnter() =====

        [TestMethod]
        public void IsAbleToEnter_ReturnsTrue_WhenNoMapLoaded()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            // Map.MapFileName is "" (empty) → IsAbleToEnter returns true immediately after IsAvailable check

            Assert.IsTrue(unit.IsAbleToEnter(1, 1));
        }

        [TestMethod]
        public void IsAbleToEnter_ReturnsFalse_WhenUnitIsNotAvailable()
        {
            var src = CreateSrc();
            var unit = new Unit(src);

            // Make unit unavailable via 他形態 + 行動不能
            unit.Status = "他形態";
            unit.AddCondition("行動不能", 3);

            Assert.IsFalse(unit.IsAbleToEnter(1, 1));
        }
    }
}
