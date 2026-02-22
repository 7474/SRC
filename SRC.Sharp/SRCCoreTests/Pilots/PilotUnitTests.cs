using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Pilots.Tests
{
    [TestClass]
    public class PilotUnitTests
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private Pilot CreatePilot(SRC src, string name = "テストパイロット", int level = 1, string party = "味方")
        {
            if (!src.PDList.IsDefined(name))
            {
                var pd = src.PDList.Add(name);
                pd.SP = 10;
            }
            return src.PList.Add(name, level, party);
        }

        [TestMethod]
        public void IsMainPilot_WhenNotBoardedOnUnit_ReturnsFalse()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "パイロット1");
            Assert.IsFalse(pilot.IsMainPilot);
        }

        [TestMethod]
        public void Unit_WhenNotBoarded_IsNull()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "パイロット2");
            Assert.IsNull(pilot.Unit);
        }

        [TestMethod]
        public void IsMainAdditionalPilot_WhenNotBoarded_ReturnsFalse()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "パイロット3");
            Assert.IsFalse(pilot.IsMainAdditionalPilot);
        }

        [TestMethod]
        public void IsMainPilot_WhenBoardedAsMainPilot_ReturnsTrue()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.ID = src.UList.CreateID("テストユニット");
            src.UList.Add2(unit);
            var pilot = CreatePilot(src, "メインパイロット");
            unit.AddPilot(pilot);
            pilot.Unit = unit;
            Assert.IsTrue(pilot.IsMainPilot);
        }

        [TestMethod]
        public void Unit_WhenManuallySet_ReturnsUnit()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.ID = src.UList.CreateID("搭乗テストユニット");
            src.UList.Add2(unit);
            var pilot = CreatePilot(src, "搭乗パイロット");
            pilot.Unit = unit;
            Assert.IsNotNull(pilot.Unit);
        }

        [TestMethod]
        public void Party_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "中立パイロット", 1, "中立");
            Assert.AreEqual("中立", pilot.Party);
        }

        [TestMethod]
        public void PilotName_MatchesCreationName()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "アムロ");
            Assert.AreEqual("アムロ", pilot.PilotName);
        }

        [TestMethod]
        public void Level_MatchesCreationLevel()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "レベル5パイロット", 5);
            Assert.AreEqual(5, pilot.Level);
        }

        [TestMethod]
        public void Name_MatchesPilotName()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "テスト名前確認");
            Assert.AreEqual("テスト名前確認", pilot.Name);
        }

        [TestMethod]
        public void IsMainPilot_WhenUnitSetButNoPilots_ReturnsFalse()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var pilot = CreatePilot(src, "パイロット確認");
            // Set Unit but do not add to pilot list
            pilot.Unit = unit;
            // MainPilot() would throw if no pilots, so IsMainPilot returns false via null check
            Assert.IsFalse(pilot.IsMainPilot);
        }
    }
}
