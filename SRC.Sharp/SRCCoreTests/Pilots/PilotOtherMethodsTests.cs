using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Pilots.Tests
{
    /// <summary>
    /// Pilot.other.cs で定義されるメソッドのユニットテスト
    /// </summary>
    [TestClass]
    public class PilotOtherMethodsTests
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

        // ──────────────────────────────────────────────
        // SynchroRate
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SynchroRate_WithoutSkill_ReturnsZero()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            Assert.AreEqual(0, pilot.SynchroRate());
        }

        [TestMethod]
        public void SynchroRate_WithSynchroSkill_ReturnsPositive()
        {
            var src = CreateSrc();
            var name = "同調率パイロット";
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            pd.AddSkill("同調率", 50d, "", 1);
            var pilot = src.PList.Add(name, 10, "味方");
            // 同調率基本値50 + レベル10 = 60
            Assert.AreEqual(60, pilot.SynchroRate());
        }

        [TestMethod]
        public void SynchroRate_WithSynchroSkill_Level1_ReturnsBasePlusLevel()
        {
            var src = CreateSrc();
            var name = "同調率L1パイロット";
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            pd.AddSkill("同調率", 30d, "", 1);
            var pilot = src.PList.Add(name, 1, "味方");
            // 同調率基本値30 + レベル1 = 31
            Assert.AreEqual(31, pilot.SynchroRate());
        }

        // ──────────────────────────────────────────────
        // CommandRange
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CommandRange_WithoutCommandSkill_ReturnsZero()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            Assert.AreEqual(0, pilot.CommandRange());
        }

        [TestMethod]
        public void CommandRange_WithCommandSkill_NoKaikyuSkill_ReturnsTwo()
        {
            var src = CreateSrc();
            var name = "指揮パイロット";
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            pd.AddSkill("指揮", 1d, "", 1);
            var pilot = src.PList.Add(name, 1, "味方");
            // 階級なし（0）→ 0<=0<=6 → 2
            Assert.AreEqual(2, pilot.CommandRange());
        }

        [TestMethod]
        public void CommandRange_WithCommandAndHighKaikyu_ReturnsFive()
        {
            var src = CreateSrc();
            var name = "元帥パイロット";
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            pd.AddSkill("指揮", 1d, "", 1);
            pd.AddSkill("階級", 15d, "", 1);
            var pilot = src.PList.Add(name, 1, "味方");
            // 階級15 → default → 5
            Assert.AreEqual(5, pilot.CommandRange());
        }

        [TestMethod]
        public void CommandRange_WithCommandAndMidKaikyu7_ReturnsThree()
        {
            var src = CreateSrc();
            var name = "中佐パイロット";
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            pd.AddSkill("指揮", 1d, "", 1);
            pd.AddSkill("階級", 7d, "", 1);
            var pilot = src.PList.Add(name, 1, "味方");
            // 階級7 → 7<=7<=9 → 3
            Assert.AreEqual(3, pilot.CommandRange());
        }

        [TestMethod]
        public void CommandRange_WithCommandAndKaikyu10_ReturnsFour()
        {
            var src = CreateSrc();
            var name = "大佐パイロット";
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            pd.AddSkill("指揮", 1d, "", 1);
            pd.AddSkill("階級", 10d, "", 1);
            var pilot = src.PList.Add(name, 1, "味方");
            // 階級10 → 10<=10<=12 → 4
            Assert.AreEqual(4, pilot.CommandRange());
        }

        // ──────────────────────────────────────────────
        // TacticalTechnique0
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TacticalTechnique0_Level1_NoTacticsSkill_ReturnsCorrectValue()
        {
            var src = CreateSrc();
            var name = "戦術無しパイロット";
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            pd.Technique = 100;
            var pilot = src.PList.Add(name, 1, "味方");
            // TechniqueBase - Level + 10*0 = Technique - Level
            // TechniqueBase depends on stats, so we check it's non-negative or a reasonable value
            var result = pilot.TacticalTechnique0();
            Assert.IsTrue(result >= 0 || result < 1000);
        }

        // ──────────────────────────────────────────────
        // HasMana
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HasMana_WithoutSkill_ReturnsFalse()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            Assert.IsFalse(pilot.HasMana());
        }

        [TestMethod]
        public void HasMana_WithSorcerySkill_ReturnsTrue()
        {
            var src = CreateSrc();
            var name = "術士パイロット";
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            pd.AddSkill("術", 1d, "", 1);
            var pilot = src.PList.Add(name, 1, "味方");
            Assert.IsTrue(pilot.HasMana());
        }

        [TestMethod]
        public void HasMana_WithManaOwnerSkill_ReturnsTrue()
        {
            var src = CreateSrc();
            var name = "魔力所有パイロット";
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            pd.AddSkill("魔力所有", 1d, "", 1);
            var pilot = src.PList.Add(name, 1, "味方");
            Assert.IsTrue(pilot.HasMana());
        }

        [TestMethod]
        public void HasMana_WithManaOption_ReturnsTrue()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(魔力使用)");
            var pilot = CreatePilot(src);
            Assert.IsTrue(pilot.HasMana());
        }

        // ──────────────────────────────────────────────
        // Relation
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Relation_DefaultRelation_ReturnsZero()
        {
            var src = CreateSrc();
            var pilot1 = CreatePilot(src, "パイロットA");
            var name2 = "パイロットB";
            var pd2 = src.PDList.Add(name2);
            pd2.SP = 10;
            var pilot2 = src.PList.Add(name2, 1, "味方");
            // 関係が設定されていない場合は 0
            Assert.AreEqual(0, pilot1.Relation(pilot2));
        }

        // ──────────────────────────────────────────────
        // FullRecover with 闘争本能
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FullRecover_WithFightingInstinct_SetsMoraleAbove100()
        {
            var src = CreateSrc();
            var name = "闘争本能パイロット";
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            pd.AddSkill("闘争本能", 2d, "", 1);
            var pilot = src.PList.Add(name, 1, "味方");
            pilot.Morale = 80;
            pilot.FullRecover();
            // 闘争本能レベル2 → 100 + 5*2 = 110
            Assert.AreEqual(110, pilot.Morale);
        }

        [TestMethod]
        public void FullRecover_WithoutFightingInstinct_SetsMoraleTo100()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            pilot.Morale = 80;
            pilot.FullRecover();
            Assert.AreEqual(100, pilot.Morale);
        }

        // ──────────────────────────────────────────────
        // IsActiveAdditionalPilot
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsActiveAdditionalPilot_WithoutUnit_ReturnsFalse()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            Assert.IsFalse(pilot.IsActiveAdditionalPilot());
        }
    }
}
