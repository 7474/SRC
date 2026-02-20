using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Pilots.Tests
{
    [TestClass]
    public class PilotSPTests
    {
        private SRC CreateSrc()
        {
            return new SRC
            {
                GUI = new MockGUI(),
            };
        }

        private Pilot CreatePilot(SRC src, string name, int sp, int level = 1)
        {
            var pd = src.PDList.Add(name);
            pd.SP = sp;
            return src.PList.Add(name, level, "味方");
        }

        // ===== MaxSP =====

        [TestMethod]
        public void MaxSP_ReturnsZero_WhenDataSPIsZero()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "ゼロSPパイロット", sp: 0, level: 10);

            Assert.AreEqual(0, pilot.MaxSP);
        }

        [TestMethod]
        public void MaxSP_ReturnsZero_WhenDataSPIsNegative()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "負SPパイロット", sp: -5, level: 10);

            Assert.AreEqual(0, pilot.MaxSP);
        }

        [TestMethod]
        public void MaxSP_ReturnsPositive_WhenDataSPIsPositive()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "標準パイロット", sp: 10, level: 1);

            // Level 1: lv = 1 <= 40 → MaxSPRet = 2*1 = 2; +SP(10) = 12
            Assert.AreEqual(12, pilot.MaxSP);
        }

        [TestMethod]
        public void MaxSP_IncludesBaseSpAndLevelValue()
        {
            var src = CreateSrc();
            // level 50: lv = 50 (>99? no) → 50 > 40 → MaxSPRet = 50+40=90; + SP(5) = 95
            var pilot = CreatePilot(src, "高レベルパイロット", sp: 5, level: 50);

            Assert.AreEqual(95, pilot.MaxSP);
        }

        // ===== SP getter and setter (basic, non-additional-pilot) =====

        [TestMethod]
        public void SP_AfterFullRecover_IsZero_WhenMaxSPIsZero()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "ゼロSPパイロット2", sp: 0, level: 5);

            // FullRecover is called during PList.Add → SP = MaxSP = 0
            Assert.AreEqual(0, pilot.SP);
        }

        [TestMethod]
        public void SP_Setter_ClampsToMaxSP()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "標準パイロット2", sp: 10, level: 1);

            pilot.SP = 9999;

            Assert.AreEqual(pilot.MaxSP, pilot.SP);
        }

        [TestMethod]
        public void SP_Setter_ClampsToZeroWhenNegative()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "標準パイロット3", sp: 10, level: 1);

            pilot.SP = -10;

            Assert.AreEqual(0, pilot.SP);
        }

        [TestMethod]
        public void SP_Setter_SetsCorrectValue()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "標準パイロット4", sp: 10, level: 1);
            var newSP = pilot.MaxSP / 2;

            pilot.SP = newSP;

            Assert.AreEqual(newSP, pilot.SP);
        }

        [TestMethod]
        public void SP_AfterFullRecover_EqualsMaxSP_WhenDataSPIsPositive()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "標準パイロット5", sp: 20, level: 10);

            pilot.SP = 0;
            pilot.FullRecover();

            Assert.AreEqual(pilot.MaxSP, pilot.SP);
        }
    }
}
