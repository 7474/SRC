using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Pilots.Tests
{
    /// <summary>
    /// Pilot クラスの各種プロパティのさらなるユニットテスト
    /// </summary>
    [TestClass]
    public class PilotPropsMoreTests
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private Pilot CreatePilot(SRC src, string name = "テストパイロット", int level = 1)
        {
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            pd.Infight = 150;
            pd.Shooting = 130;
            pd.Hit = 120;
            pd.Dodge = 110;
            pd.Intuition = 100;
            pd.Technique = 90;
            return src.PList.Add(name, level, "味方");
        }

        // ──────────────────────────────────────────────
        // Name
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_MatchesPilotDataName()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "アムロ");
            Assert.AreEqual("アムロ", pilot.Name);
        }

        // ──────────────────────────────────────────────
        // Party
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Party_InitiallySet()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "テスト");
            Assert.AreEqual("味方", pilot.Party);
        }

        // ──────────────────────────────────────────────
        // Level
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Level_MatchesCreationLevel()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "レベル5パイロット", level: 5);
            Assert.AreEqual(5, pilot.Level);
        }

        [TestMethod]
        public void Level_Level1_IsOne()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "新人");
            Assert.AreEqual(1, pilot.Level);
        }

        // ──────────────────────────────────────────────
        // Morale
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Morale_Initial_IsDefaultValue()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "テスト");
            Assert.IsTrue(pilot.Morale >= 0);
        }

        // ──────────────────────────────────────────────
        // PilotData プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Infight_MatchesPilotDataInfight()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "格闘家");
            Assert.AreEqual(150, pilot.Data.Infight);
        }

        [TestMethod]
        public void Shooting_MatchesPilotDataShooting()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "射撃家");
            Assert.AreEqual(130, pilot.Data.Shooting);
        }

        [TestMethod]
        public void Hit_MatchesPilotDataHit()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "命中家");
            Assert.AreEqual(120, pilot.Data.Hit);
        }

        [TestMethod]
        public void Dodge_MatchesPilotDataDodge()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "回避家");
            Assert.AreEqual(110, pilot.Data.Dodge);
        }

        // ──────────────────────────────────────────────
        // Plana
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Plana_Initial_IsNonNegative()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "霊力家");
            Assert.IsTrue(pilot.Plana >= 0);
        }
    }
}
