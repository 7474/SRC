using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Pilots.Tests
{
    [TestClass]
    public class PilotLevelMoreTests3
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private Pilot CreatePilot(SRC src, string name = "テストパイロット", int level = 1)
        {
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            return src.PList.Add(name, level, "味方");
        }

        // ===== Exp setter - large values =====

        [TestMethod]
        public void Exp_LargePositiveValue_CapsLevelAt99()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, level: 1);
            pilot.Exp = 500 * 200; // 200 level-ups worth
            Assert.AreEqual(99, pilot.Level);
        }

        [TestMethod]
        public void Exp_ExactlyAtLevel99Cap_SetsExpTo500()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, level: 1);
            pilot.Exp = 500 * 200;
            Assert.AreEqual(500, pilot.Exp);
        }

        [TestMethod]
        public void Exp_Adding500_FromLevel1_BecomesLevel2()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, level: 1);
            pilot.Exp = 500;
            Assert.AreEqual(2, pilot.Level);
            Assert.AreEqual(0, pilot.Exp);
        }

        [TestMethod]
        public void Exp_Adding1000_FromLevel1_BecomesLevel3()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, level: 1);
            pilot.Exp = 1000;
            Assert.AreEqual(3, pilot.Level);
            Assert.AreEqual(0, pilot.Exp);
        }

        // ===== TotalExp =====

        [TestMethod]
        public void TotalExp_Level1Exp0_Returns500()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, level: 1);
            pilot.Exp = 0;
            // TotalExp = Level * 500 + Exp = 1*500 + 0 = 500
            Assert.AreEqual(500, pilot.TotalExp);
        }

        [TestMethod]
        public void TotalExp_Level5Exp250_Returns2750()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, level: 5);
            pilot.Exp = 250;
            Assert.AreEqual(2750, pilot.TotalExp);
        }

        [TestMethod]
        public void TotalExp_Level10Exp0_Returns5000()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, level: 10);
            pilot.Exp = 0;
            Assert.AreEqual(5000, pilot.TotalExp);
        }

        // ===== Morale clamping =====

        [TestMethod]
        public void Morale_SetAboveMaxMorale_ClampsToMaxMorale()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            pilot.Morale = 9999;
            Assert.AreEqual(pilot.MaxMorale, pilot.Morale);
        }

        [TestMethod]
        public void Morale_SetBelowMinMorale_ClampsToMinMorale()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            pilot.Morale = -9999;
            Assert.AreEqual(pilot.MinMorale, pilot.Morale);
        }

        [TestMethod]
        public void Morale_SetToValidValue_Stored()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            pilot.Morale = 100;
            Assert.AreEqual(100, pilot.Morale);
        }

        // ===== MaxMorale / MinMorale defaults =====

        [TestMethod]
        public void MaxMorale_Default_Is150()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            Assert.AreEqual(150, pilot.MaxMorale);
        }

        [TestMethod]
        public void MinMorale_Default_Is50()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            Assert.AreEqual(50, pilot.MinMorale);
        }

        [TestMethod]
        public void Morale_SetToMaxMorale_StaysAtMax()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            pilot.Morale = pilot.MaxMorale;
            Assert.AreEqual(pilot.MaxMorale, pilot.Morale);
        }

        [TestMethod]
        public void Morale_SetToMinMorale_StaysAtMin()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            pilot.Morale = pilot.MinMorale;
            Assert.AreEqual(pilot.MinMorale, pilot.Morale);
        }
    }
}
