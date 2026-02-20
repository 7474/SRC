using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Pilots.Tests
{
    /// <summary>
    /// Pilot のスキル関連のユニットテスト
    /// </summary>
    [TestClass]
    public class PilotSkillTests
    {
        private SRC CreateSrc()
        {
            return new SRC
            {
                GUI = new MockGUI(),
            };
        }

        private Pilot CreatePilot(SRC src, string name = "テストパイロット")
        {
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            return src.PList.Add(name, 1, "味方");
        }

        // ──────────────────────────────────────────────
        // CountSkill
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CountSkill_NoSkills_ReturnsZero()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            Assert.AreEqual(0, pilot.CountSkill());
        }

        // ──────────────────────────────────────────────
        // IsSkillAvailable
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsSkillAvailable_NonExistentSkill_ReturnsFalse()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            Assert.IsFalse(pilot.IsSkillAvailable("気合"));
        }

        // ──────────────────────────────────────────────
        // Pilot Name / Nickname
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PilotName_ReturnsDataName()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "アムロ・レイ");
            Assert.AreEqual("アムロ・レイ", pilot.Name);
        }

        [TestMethod]
        public void Party_SetAndGet_Works()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            pilot.Party = "敵";
            Assert.AreEqual("敵", pilot.Party);
        }

        [TestMethod]
        public void Alive_DefaultIsTrue()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            Assert.IsTrue(pilot.Alive);
        }

        [TestMethod]
        public void Away_DefaultIsFalse()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            Assert.IsFalse(pilot.Away);
        }

        // ──────────────────────────────────────────────
        // FullRecover
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FullRecover_SetsMoraleToHundred()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            pilot.Morale = 80;
            pilot.FullRecover();
            Assert.AreEqual(100, pilot.Morale);
        }

        [TestMethod]
        public void FullRecover_SetsSPToMaxSP()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("FullRecoverSPTest");
            pd.SP = 20;
            var pilot = src.PList.Add("FullRecoverSPTest", 1, "味方");
            var maxSP = pilot.MaxSP;
            pilot.SP = 0;
            pilot.FullRecover();
            Assert.AreEqual(maxSP, pilot.SP);
        }
    }
}
