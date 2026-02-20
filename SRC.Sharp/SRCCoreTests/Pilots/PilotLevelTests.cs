using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Pilots.Tests
{
    /// <summary>
    /// Pilot のレベル・経験値・気力のユニットテスト
    /// </summary>
    [TestClass]
    public class PilotLevelTests
    {
        private SRC CreateSrc()
        {
            return new SRC
            {
                GUI = new MockGUI(),
            };
        }

        private Pilot CreatePilot(SRC src, string name = "テストパイロット", int level = 1)
        {
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            return src.PList.Add(name, level, "味方");
        }

        // ──────────────────────────────────────────────
        // Level
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Level_InitialLevel_IsCorrect()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, level: 5);
            Assert.AreEqual(5, pilot.Level);
        }

        [TestMethod]
        public void Level_SetLevel_Updates()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            pilot.Level = 10;
            Assert.AreEqual(10, pilot.Level);
        }

        [TestMethod]
        public void Level_SetSameLevel_DoesNotChange()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, level: 5);
            pilot.Level = 5; // setting same level
            Assert.AreEqual(5, pilot.Level);
        }

        // ──────────────────────────────────────────────
        // Exp
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Exp_AddingExpLessThan500_StaysAtLevel()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, level: 1);
            var initialLevel = pilot.Level;
            pilot.Exp += 200;
            Assert.AreEqual(initialLevel, pilot.Level);
            Assert.AreEqual(200, pilot.Exp);
        }

        [TestMethod]
        public void Exp_Adding500_IncrementsLevel()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, level: 1);
            pilot.Exp += 500;
            Assert.AreEqual(2, pilot.Level);
            Assert.AreEqual(0, pilot.Exp);
        }

        [TestMethod]
        public void Exp_Adding1000_IncrementsLevelBy2()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, level: 1);
            pilot.Exp += 1000;
            Assert.AreEqual(3, pilot.Level);
            Assert.AreEqual(0, pilot.Exp);
        }

        [TestMethod]
        public void TotalExp_ReturnsLevelTimesFiveHundredPlusExp()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, level: 3);
            pilot.Exp += 250;
            // TotalExp = 3 * 500 + 250 = 1750
            Assert.AreEqual(1750, pilot.TotalExp);
        }

        // ──────────────────────────────────────────────
        // Morale
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Morale_SetWithinRange_SetsValue()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            pilot.Morale = 100;
            Assert.AreEqual(100, pilot.Morale);
        }

        [TestMethod]
        public void Morale_SetAboveMax_ClampedToMax()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            pilot.Morale = 200; // above MaxMorale=150
            Assert.AreEqual(150, pilot.Morale);
        }

        [TestMethod]
        public void Morale_SetBelowMin_ClampedToMin()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            pilot.Morale = 10; // below MinMorale=50
            Assert.AreEqual(50, pilot.Morale);
        }

        [TestMethod]
        public void MaxMorale_DefaultIs150()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            Assert.AreEqual(150, pilot.MaxMorale);
        }

        [TestMethod]
        public void MinMorale_DefaultIs50()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            Assert.AreEqual(50, pilot.MinMorale);
        }

        // ──────────────────────────────────────────────
        // 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TotalExp_AtLevel1_WithZeroExp_Is500()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, level: 1);
            // TotalExp = 1 * 500 + 0 = 500
            Assert.AreEqual(500, pilot.TotalExp);
        }

        [TestMethod]
        public void TotalExp_AtLevel1_WithPartialExp_IsCorrect()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, level: 1);
            pilot.Exp += 100;
            // TotalExp = 1 * 500 + 100 = 600
            Assert.AreEqual(600, pilot.TotalExp);
        }

        [TestMethod]
        public void Morale_SetToExactMin_StaysAtMin()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            pilot.Morale = 50;
            Assert.AreEqual(50, pilot.Morale);
        }

        [TestMethod]
        public void Morale_SetToExactMax_StaysAtMax()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            pilot.Morale = 150;
            Assert.AreEqual(150, pilot.Morale);
        }

        [TestMethod]
        public void Level_IncreasedByExp_UpdatesCorrectly()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, level: 5);
            pilot.Exp += 500;
            Assert.AreEqual(6, pilot.Level);
        }
    }
}
