using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Pilots.Tests
{
    /// <summary>
    /// Pilot の SP 追加テスト (PilotSPMoreTests3)
    /// </summary>
    [TestClass]
    public class PilotSPMoreTests3
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        private Pilot CreatePilot(SRC src, string name, int sp, int level = 1)
        {
            var pd = src.PDList.Add(name);
            pd.SP = sp;
            return src.PList.Add(name, level, "味方");
        }

        // ──────────────────────────────────────────────
        // MaxSP — 各レベルの検証
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MaxSP_Level1_CorrectValue()
        {
            var src = CreateSrc();
            // level 1 <= 40 → 2*1=2; +SP(5) = 7
            var pilot = CreatePilot(src, "lv1", sp: 5, level: 1);
            Assert.AreEqual(7, pilot.MaxSP);
        }

        [TestMethod]
        public void MaxSP_Level20_CorrectValue()
        {
            var src = CreateSrc();
            // level 20 <= 40 → 2*20=40; +SP(10) = 50
            var pilot = CreatePilot(src, "lv20", sp: 10, level: 20);
            Assert.AreEqual(50, pilot.MaxSP);
        }

        [TestMethod]
        public void MaxSP_Level99_CorrectValue()
        {
            var src = CreateSrc();
            // level 99 <= 99 → 99+40=139; +SP(5) = 144
            var pilot = CreatePilot(src, "lv99", sp: 5, level: 99);
            Assert.AreEqual(144, pilot.MaxSP);
        }

        [TestMethod]
        public void MaxSP_Level100_CorrectValue()
        {
            var src = CreateSrc();
            // level 100 > 99 → lv capped at 100, 100+40=140; +SP(5) = 145
            var pilot = CreatePilot(src, "lv100", sp: 5, level: 100);
            Assert.AreEqual(145, pilot.MaxSP);
        }

        [TestMethod]
        public void MaxSP_WithLargeSP_IncludesBaseSP()
        {
            var src = CreateSrc();
            // level 1: 2*1=2; +SP(100) = 102
            var pilot = CreatePilot(src, "largeSP", sp: 100, level: 1);
            Assert.AreEqual(102, pilot.MaxSP);
        }

        // ──────────────────────────────────────────────
        // SP clamping 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SP_SetToMaxSP_StaysAtMaxSP()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "maxSPTest", sp: 10, level: 1);
            pilot.SP = pilot.MaxSP;
            Assert.AreEqual(pilot.MaxSP, pilot.SP);
        }

        [TestMethod]
        public void SP_SetBelowZero_ClampsToZero()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "belowZero", sp: 5, level: 1);
            pilot.SP = -100;
            Assert.AreEqual(0, pilot.SP);
        }

        [TestMethod]
        public void SP_SetAboveMaxSP_ClampsToMaxSP()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "aboveMax", sp: 5, level: 5);
            int max = pilot.MaxSP;
            pilot.SP = max + 100;
            Assert.AreEqual(max, pilot.SP);
        }

        [TestMethod]
        public void SP_AfterDecrease_Decreased()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "decrease", sp: 20, level: 1);
            int initial = pilot.SP;
            pilot.SP = initial - 5;
            Assert.AreEqual(initial - 5, pilot.SP);
        }

        [TestMethod]
        public void SP_SetToOne_StaysOne()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "one", sp: 5, level: 5);
            pilot.SP = 1;
            Assert.AreEqual(1, pilot.SP);
        }

        [TestMethod]
        public void MaxSP_Level5_CorrectValue()
        {
            var src = CreateSrc();
            // level 5 <= 40 → 2*5=10; +SP(3) = 13
            var pilot = CreatePilot(src, "lv5", sp: 3, level: 5);
            Assert.AreEqual(13, pilot.MaxSP);
        }

        [TestMethod]
        public void MaxSP_Level60_CorrectValue()
        {
            var src = CreateSrc();
            // level 60: 60+40=100; +SP(5) = 105
            var pilot = CreatePilot(src, "lv60", sp: 5, level: 60);
            Assert.AreEqual(105, pilot.MaxSP);
        }
    }
}
